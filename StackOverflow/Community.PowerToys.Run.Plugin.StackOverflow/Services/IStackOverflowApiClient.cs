using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;

namespace Community.PowerToys.Run.Plugin.StackOverflow.Services
{
    /// <summary>
    /// Contract for interacting with Stack Exchange API v2.3+.
    /// </summary>
    public interface IStackOverflowApiClient
    {
        /// <summary>
        /// Search StackOverflow questions by query text.
        /// </summary>
        /// <param name="query">Search query string (2-200 characters, validated by caller).</param>
        /// <param name="cancellationToken">Cancellation token for request timeout/cancellation.</param>
        /// <returns>List of up to 5 StackOverflow questions ordered by relevance.</returns>
        Task<List<StackOverflowQuestion>> SearchAsync(string query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get current API rate limit status.
        /// </summary>
        /// <returns>Tuple containing remaining requests, max requests, and reset time.</returns>
        Task<(int remaining, int max, DateTime reset)> GetRateLimitStatusAsync();
    }
}
