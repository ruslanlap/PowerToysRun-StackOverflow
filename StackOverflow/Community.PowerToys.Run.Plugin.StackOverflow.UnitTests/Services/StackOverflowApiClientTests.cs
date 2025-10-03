using Microsoft.VisualStudio.TestTools.UnitTesting;
using Community.PowerToys.Run.Plugin.StackOverflow.Services;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Community.PowerToys.Run.Plugin.StackOverflow.UnitTests.Services
{
    [TestClass]
    public class StackOverflowApiClientTests
    {
        [TestMethod]
        public async Task SearchAsync_ValidQuery_ReturnsResults()
        {
            // Arrange
            var mockHandler = CreateMockHttpHandler(GetValidApiResponse());
            var client = new StackOverflowApiClient(new HttpClient(mockHandler.Object));
            
            // Act
            var results = await client.SearchAsync("python lists");
            
            // Assert
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        public async Task SearchAsync_NoResults_ReturnsEmptyList()
        {
            // Arrange
            var mockHandler = CreateMockHttpHandler(GetEmptyApiResponse());
            var client = new StackOverflowApiClient(new HttpClient(mockHandler.Object));
            
            // Act
            var results = await client.SearchAsync("veryunlikelyquerywithnoResults");
            
            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task SearchAsync_NetworkError_ThrowsHttpRequestException()
        {
            // Arrange
            var mockHandler = CreateMockHttpHandler(null, HttpStatusCode.ServiceUnavailable);
            var client = new StackOverflowApiClient(new HttpClient(mockHandler.Object));
            
            // Act
            await client.SearchAsync("test");
            
            // Assert - expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task SearchAsync_Timeout_ThrowsTaskCanceledException()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException());
            
            var client = new StackOverflowApiClient(new HttpClient(mockHandler.Object));
            
            // Act
            await client.SearchAsync("test");
            
            // Assert - expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SearchAsync_InvalidJson_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockHandler = CreateMockHttpHandler("{ invalid json }");
            var client = new StackOverflowApiClient(new HttpClient(mockHandler.Object));
            
            // Act
            await client.SearchAsync("test");
            
            // Assert - expects exception
        }

        [TestMethod]
        public async Task GetRateLimitStatusAsync_ReturnsValidData()
        {
            // Arrange
            var mockHandler = CreateMockHttpHandler(GetRateLimitResponse());
            var client = new StackOverflowApiClient(new HttpClient(mockHandler.Object));
            
            // Act
            var status = await client.GetRateLimitStatusAsync();
            
            // Assert
            Assert.IsTrue(status.remaining >= 0);
            Assert.IsTrue(status.max > 0);
        }

        [TestMethod]
        public async Task SearchAsync_ReturnsMaxFiveResults()
        {
            // Arrange
            var mockHandler = CreateMockHttpHandler(GetValidApiResponse());
            var client = new StackOverflowApiClient(new HttpClient(mockHandler.Object));
            
            // Act
            var results = await client.SearchAsync("popular query");
            
            // Assert
            Assert.IsTrue(results.Count <= 5);
        }

        [TestMethod]
        public async Task SearchAsync_CancellationToken_Cancels()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            
            var mockHandler = CreateMockHttpHandler(GetValidApiResponse());
            var client = new StackOverflowApiClient(new HttpClient(mockHandler.Object));
            
            // Act & Assert
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(
                async () => await client.SearchAsync("test", cts.Token));
        }

        private Mock<HttpMessageHandler> CreateMockHttpHandler(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent ?? "")
                });
            return mockHandler;
        }

        private string GetValidApiResponse()
        {
            return @"{
                ""items"": [
                    {
                        ""question_id"": 123456,
                        ""title"": ""How to use Python lists?"",
                        ""link"": ""https://stackoverflow.com/questions/123456"",
                        ""score"": 100,
                        ""answer_count"": 5,
                        ""has_accepted_answer"": true,
                        ""tags"": [""python"", ""list""],
                        ""last_activity_date"": 1234567890
                    }
                ]
            }";
        }

        private string GetEmptyApiResponse()
        {
            return @"{""items"": []}";
        }

        private string GetRateLimitResponse()
        {
            return @"{
                ""quota_remaining"": 250,
                ""quota_max"": 300,
                ""has_more"": false
            }";
        }
    }
}
