#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;

namespace Community.PowerToys.Run.Plugin.StackOverflow.Services
{
    /// <summary>
    /// Client for interacting with Stack Exchange API v2.3+.
    /// </summary>
    public class StackOverflowApiClient : IStackOverflowApiClient
    {
        private const string ApiBaseUrl = "https://api.stackexchange.com/2.3/";
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public StackOverflowApiClient() : this(CreateDefaultHttpClient(), null)
        {
        }

        public StackOverflowApiClient(HttpClient httpClient) : this(httpClient, null)
        {
        }

        public StackOverflowApiClient(HttpClient httpClient, string? apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public static HttpClient CreateDefaultHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };
            
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiBaseUrl),
                Timeout = TimeSpan.FromSeconds(10)
            };
            
            // Add proper headers that Stack Exchange API expects
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "PowerToys-StackOverflow-Plugin/1.0");
            return client;
        }

        public async Task<List<StackOverflowQuestion>> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            try
            {
                var encodedQuery = HttpUtility.UrlEncode(query);
                var url = BuildSearchUrl(encodedQuery);

                var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"API request failed with status code: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse == null || apiResponse.Items == null)
                {
                    throw new InvalidOperationException("Failed to parse API response");
                }

                return apiResponse.Items.Select(MapToQuestion).ToList();
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error searching StackOverflow: {ex.Message}", ex);
            }
        }

        private string BuildSearchUrl(string encodedQuery)
        {
            var url = $"search/advanced?order=desc&sort=relevance&q={encodedQuery}&site=stackoverflow&pagesize=5";
            
            if (!string.IsNullOrEmpty(_apiKey))
            {
                url += $"&key={_apiKey}";
            }
            
            return url;
        }

        public async Task<(int remaining, int max, DateTime reset)> GetRateLimitStatusAsync()
        {
            try
            {
                var url = "info?site=stackoverflow";
                var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to get rate limit status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse == null)
                {
                    throw new InvalidOperationException("Failed to parse rate limit response");
                }

                // Reset time is midnight UTC
                var reset = DateTime.UtcNow.Date.AddDays(1);
                
                return (apiResponse.QuotaRemaining, apiResponse.QuotaMax, reset);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error getting rate limit status: {ex.Message}", ex);
            }
        }

        private static StackOverflowQuestion MapToQuestion(ApiQuestion apiQuestion)
        {
            return new StackOverflowQuestion
            {
                QuestionId = apiQuestion.QuestionId,
                Title = apiQuestion.Title ?? string.Empty,
                Link = apiQuestion.Link ?? string.Empty,
                Score = apiQuestion.Score,
                AnswerCount = apiQuestion.AnswerCount,
                HasAcceptedAnswer = apiQuestion.HasAcceptedAnswer,
                Tags = apiQuestion.Tags ?? Array.Empty<string>(),
                LastActivityDate = DateTimeOffset.FromUnixTimeSeconds(apiQuestion.LastActivityDate).UtcDateTime
            };
        }
    }
}
