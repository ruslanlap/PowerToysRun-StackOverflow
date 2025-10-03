using System;
using System.Collections.Generic;

namespace Community.PowerToys.Run.Plugin.StackOverflow.Models
{
    /// <summary>
    /// Stores search results with metadata for LRU eviction and TTL expiration.
    /// </summary>
    public class CacheEntry
    {
        public string QueryKey { get; init; }
        public List<StackOverflowQuestion> Results { get; init; } = new();
        public DateTime CachedAt { get; init; }
        public DateTime LastAccessedAt { get; set; }

        /// <summary>
        /// Entry is expired if older than 1 hour.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow - CachedAt > TimeSpan.FromHours(1);
    }
}
