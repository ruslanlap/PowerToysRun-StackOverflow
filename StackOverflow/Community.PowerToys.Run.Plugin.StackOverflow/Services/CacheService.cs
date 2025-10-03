#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;

namespace Community.PowerToys.Run.Plugin.StackOverflow.Services
{
    /// <summary>
    /// In-memory LRU cache with TTL expiration for StackOverflow search results.
    /// </summary>
    public class CacheService : ICacheService
    {
        private const int MaxCacheSize = 50;
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
        private readonly object _lockObject = new();
        private int _hits = 0;
        private int _misses = 0;

        public List<StackOverflowQuestion>? Get(string queryKey)
        {
            if (_cache.TryGetValue(queryKey, out var entry))
            {
                if (entry.IsExpired)
                {
                    // Remove expired entry
                    _cache.TryRemove(queryKey, out _);
                    System.Threading.Interlocked.Increment(ref _misses);
                    return null;
                }

                // Update last accessed time (for LRU tracking)
                entry.LastAccessedAt = DateTime.UtcNow;
                System.Threading.Interlocked.Increment(ref _hits);
                return new List<StackOverflowQuestion>(entry.Results);
            }

            System.Threading.Interlocked.Increment(ref _misses);
            return null;
        }

        public void Set(string queryKey, List<StackOverflowQuestion> results)
        {
            lock (_lockObject)
            {
                // Check if cache is at capacity and evict LRU entry if needed
                if (_cache.Count >= MaxCacheSize && !_cache.ContainsKey(queryKey))
                {
                    EvictLeastRecentlyUsed();
                }

                var now = DateTime.UtcNow;
                var entry = new CacheEntry
                {
                    QueryKey = queryKey,
                    Results = new List<StackOverflowQuestion>(results),
                    CachedAt = now,
                    LastAccessedAt = now
                };

                _cache[queryKey] = entry;
            }
        }

        public void Clear()
        {
            _cache.Clear();
            _hits = 0;
            _misses = 0;
        }

        public void RemoveExpired()
        {
            var expiredKeys = _cache
                .Where(kvp => kvp.Value.IsExpired)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _cache.TryRemove(key, out _);
            }
        }

        public (int count, double hitRate, int oldestEntryMinutes) GetStatistics()
        {
            var count = _cache.Count;
            
            var totalRequests = _hits + _misses;
            var hitRate = totalRequests > 0 ? (_hits * 100.0) / totalRequests : 0;

            var oldestEntryMinutes = 0;
            if (_cache.Any())
            {
                var oldestEntry = _cache.Values.MinBy(e => e.CachedAt);
                if (oldestEntry != null)
                {
                    oldestEntryMinutes = (int)(DateTime.UtcNow - oldestEntry.CachedAt).TotalMinutes;
                }
            }

            return (count, hitRate, oldestEntryMinutes);
        }

        private void EvictLeastRecentlyUsed()
        {
            if (_cache.IsEmpty)
                return;

            var lruEntry = _cache.Values.MinBy(e => e.LastAccessedAt);
            if (lruEntry != null)
            {
                _cache.TryRemove(lruEntry.QueryKey, out _);
            }
        }
    }
}
