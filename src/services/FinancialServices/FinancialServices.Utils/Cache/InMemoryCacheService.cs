using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Utils.Cache
{

    public class InMemoryCacheService : IInMemoryCacheService
    {
        private class CacheEntry
        {
            public object Value { get; set; }
            public DateTime ExpireAt { get; set; }
        }

        private readonly Dictionary<string, CacheEntry> _cache = new();
        private readonly object _lock = new();

        public void Set(string key, object value, TimeSpan ttl)
        {
            lock (_lock)
            {
                _cache[key] = new CacheEntry { Value = value, ExpireAt = DateTime.UtcNow.Add(ttl) };
            }
        }

        public bool TryGet(string key, out object value)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var entry) && entry.ExpireAt > DateTime.UtcNow)
                {
                    value = entry.Value;
                    return true;
                }

                _cache.Remove(key);
                value = null;
                return false;
            }
        }

        public void Invalidate(string key)
        {
            lock (_lock)
                _cache.Remove(key);
        }

        public void InvalidateByPrefix(string prefix)
        {
            lock (_lock)
            {
                var keys = _cache.Keys.Where(k => k.StartsWith(prefix)).ToList();
                foreach (var key in keys)
                    _cache.Remove(key);
            }
        }
    }
}
