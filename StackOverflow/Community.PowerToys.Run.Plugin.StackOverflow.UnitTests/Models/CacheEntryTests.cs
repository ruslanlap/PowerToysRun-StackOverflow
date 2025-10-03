using Microsoft.VisualStudio.TestTools.UnitTesting;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;
using System;
using System.Collections.Generic;

namespace Community.PowerToys.Run.Plugin.StackOverflow.UnitTests.Models
{
    [TestClass]
    public class CacheEntryTests
    {
        [TestMethod]
        public void IsExpired_ReturnsTrueAfterOneHour()
        {
            // Arrange
            var entry = new CacheEntry
            {
                QueryKey = "test",
                Results = new List<StackOverflowQuestion>(),
                CachedAt = DateTime.UtcNow.AddHours(-2), // 2 hours ago
                LastAccessedAt = DateTime.UtcNow
            };
            
            // Act & Assert
            Assert.IsTrue(entry.IsExpired);
        }

        [TestMethod]
        public void IsExpired_ReturnsFalseWithinOneHour()
        {
            // Arrange
            var entry = new CacheEntry
            {
                QueryKey = "test",
                Results = new List<StackOverflowQuestion>(),
                CachedAt = DateTime.UtcNow.AddMinutes(-30), // 30 minutes ago
                LastAccessedAt = DateTime.UtcNow
            };
            
            // Act & Assert
            Assert.IsFalse(entry.IsExpired);
        }

        [TestMethod]
        public void LastAccessedAt_UpdatedOnAccess()
        {
            // Arrange
            var originalTime = DateTime.UtcNow.AddMinutes(-10);
            var entry = new CacheEntry
            {
                QueryKey = "test",
                Results = new List<StackOverflowQuestion>(),
                CachedAt = originalTime,
                LastAccessedAt = originalTime
            };
            
            // Act
            entry.LastAccessedAt = DateTime.UtcNow;
            
            // Assert
            Assert.IsTrue(entry.LastAccessedAt > originalTime);
        }

        [TestMethod]
        public void Results_CanBeEmptyList()
        {
            // Arrange & Act
            var entry = new CacheEntry
            {
                QueryKey = "test",
                Results = new List<StackOverflowQuestion>(),
                CachedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow
            };
            
            // Assert
            Assert.IsNotNull(entry.Results);
            Assert.AreEqual(0, entry.Results.Count);
        }
    }
}
