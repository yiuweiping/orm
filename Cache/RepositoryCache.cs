using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache.Enum;

namespace Zhengdi.Framework.Cache
{
    public class RepositoryCache : ICacheBase
    {
        public DateTime Expire { get; set; }
        public string Key { get; private set; }
        public CacheType Type { get; set; }
        public dynamic Value { get; private set; }
        public RepositoryCache(string key, dynamic value)
        {
            this.Key = key;
            this.Value = value;
        }
    }


    public class RepositoryCacheManager : ICacheManager<RepositoryCache>
    {
        readonly HybridDictionary _cache;
        static RepositoryCacheManager instance;
        private RepositoryCacheManager()
        {
            this._cache = new HybridDictionary();
        }
        public static RepositoryCacheManager GetRepositoryCacheManager()
        {
            instance = instance ?? new RepositoryCacheManager();
            return instance;
        }
        public RepositoryCache this[string key]
        {
            get
            {
                return this._cache[key] as RepositoryCache;
            }
        }
        public void Add(string key, RepositoryCache value)
        {
            value.Expire = DateTime.MaxValue;
            value.Type = CacheType.Application;
            this._cache.Add(key, value);
        }
        public bool ContainsKey(string key)
        {
            return this._cache.Contains(key);
        }
        public void Remove(string key)
        {
            this._cache.Remove(key);
        }
    }
}
