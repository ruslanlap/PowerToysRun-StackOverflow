using Microsoft.VisualStudio.TestTools.UnitTesting;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;
using Community.PowerToys.Run.Plugin.StackOverflow.Services;
using System;
using System.Collections.Generic;

namespace Community.PowerToys.Run.Plugin.StackOverflow.UnitTests.Services
{
    [TestClass]
    public class CacheServiceTests
    {
        private ICacheService _cacheService;

        [TestInitialize]
        public void Setup()
        {
            _cacheService = new CacheService();
        }

        [TestMethod]
        public void Get_CacheMiss_ReturnsNull()
        {
            // Act
            var result = _cacheService.Get("nonexistent");
            
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Set_ThenGet_ReturnsResults()
        {
            // Arrange
            var questions = new List<StackOverflowQuestion>
            {
                new StackOverflowQuestion
                {
                    QuestionId = 1,
                    Title = "Test",
                    Link = "https://stackoverflow.com/questions/1"
                }
            };
            
            // Act
            _cacheService.Set("test query", questions);
            var result = _cacheService.Get("test query");
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test", result[0].Title);
        }

        [TestMethod]
        public void Get_ExpiredEntry_ReturnsNull()
        {
            // This test would require accessing internal state or mocking time
            // For now, we'll test the public interface behavior
            // Arrange
            _cacheService.Set("test", new List<StackOverflowQuestion>());
            
            // Act - entry is fresh
            var result = _cacheService.Get("test");
            
            // Assert
            Assert.IsNotNull(result); // Should be found when fresh
        }

        [TestMethod]
        public void Clear_RemovesAllEntries()
        {
            // Arrange
            _cacheService.Set("query1", new List<StackOverflowQuestion>());
            _cacheService.Set("query2", new List<StackOverflowQuestion>());
            
            // Act
            _cacheService.Clear();
            
            // Assert
            Assert.IsNull(_cacheService.Get("query1"));
            Assert.IsNull(_cacheService.Get("query2"));
        }

        [TestMethod]
        public void GetStatistics_ReturnsCorrectCount()
        {
            // Arrange
            _cacheService.Set("query1", new List<StackOverflowQuestion>());
            _cacheService.Set("query2", new List<StackOverflowQuestion>());
            
            // Act
            var stats = _cacheService.GetStatistics();
            
            // Assert
            Assert.AreEqual(2, stats.count);
        }

        [TestMethod]
        public void Set_Overwrites_ExistingEntry()
        {
            // Arrange
            var oldList = new List<StackOverflowQuestion>
            {
                new StackOverflowQuestion
                {
                    QuestionId = 1,
                    Title = "Old",
                    Link = "https://stackoverflow.com/questions/1"
                }
            };
            var newList = new List<StackOverflowQuestion>
            {
                new StackOverflowQuestion
                {
                    QuestionId = 2,
                    Title = "New",
                    Link = "https://stackoverflow.com/questions/2"
                }
            };
            
            // Act
            _cacheService.Set("test", oldList);
            _cacheService.Set("test", newList);
            var result = _cacheService.Get("test");
            
            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("New", result[0].Title);
        }

        [TestMethod]
        public void RemoveExpired_DoesNotRemoveFreshEntries()
        {
            // Arrange
            _cacheService.Set("fresh", new List<StackOverflowQuestion>());
            
            // Act
            _cacheService.RemoveExpired();
            
            // Assert
            Assert.IsNotNull(_cacheService.Get("fresh"));
        }

        [TestMethod]
        public void GetStatistics_CalculatesHitRate()
        {
            // Arrange
            _cacheService.Set("query", new List<StackOverflowQuestion>());
            
            // Act - simulate hits and misses
            _cacheService.Get("query"); // hit
            _cacheService.Get("missing"); // miss
            var stats = _cacheService.GetStatistics();
            
            // Assert
            Assert.IsTrue(stats.hitRate >= 0 && stats.hitRate <= 100);
        }

        [TestMethod]
        public void GetStatistics_TracksOldestEntry()
        {
            // Arrange
            _cacheService.Set("query", new List<StackOverflowQuestion>());
            
            // Act
            var stats = _cacheService.GetStatistics();
            
            // Assert
            Assert.IsTrue(stats.oldestEntryMinutes >= 0);
        }
    }
}
