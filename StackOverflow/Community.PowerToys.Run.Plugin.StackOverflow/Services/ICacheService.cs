#nullable enable
using System.Collections.Generic;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;

namespace Community.PowerToys.Run.Plugin.StackOverflow.Services
{
    /// <summary>
    /// Contract for in-memory LRU cache with TTL expiration.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieve cached search results for a query.
        /// </summary>
        /// <param name="queryKey">Normalized query string (lowercase, trimmed) used as cache key.</param>
        /// <returns>Cached list of StackOverflow questions if found and not expired, null if cache miss or expired.</returns>
        List<StackOverflowQuestion>? Get(string queryKey);

        /// <summary>
        /// Store search results in cache.
        /// </summary>
        /// <param name="queryKey">Normalized query string (lowercase, trimmed) used as cache key.</param>
        /// <param name="results">List of StackOverflow questions to cache (0-5 items typically).</param>
        void Set(string queryKey, List<StackOverflowQuestion> results);

        /// <summary>
        /// Remove all cached entries immediately.
        /// </summary>
        void Clear();

        /// <summary>
        /// Remove expired entries (>1 hour old) from cache.
        /// </summary>
        void RemoveExpired();

        /// <summary>
        /// Get current cache statistics.
        /// </summary>
        /// <returns>Tuple containing count, hitRate, and oldestEntryMinutes.</returns>
        (int count, double hitRate, int oldestEntryMinutes) GetStatistics();
    }
}
