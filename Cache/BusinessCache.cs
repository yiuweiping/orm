using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache.Enum;

namespace Zhengdi.Framework.Cache
{
    public class BusinessCache : ICacheBase
    {
        public DateTime Expire { get; set; }
        public string Key { get; private set; }
        public CacheType Type { get; set; }
        public dynamic Value { get; private set; }
        public BusinessCache(string key, dynamic value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
    public class BusinessCacheManager : ICacheManager<BusinessCache>
    {
        readonly HybridDictionary _cache;
        static BusinessCacheManager instance;
        private BusinessCacheManager()
        {
            this._cache = new HybridDictionary();
        }
        public static BusinessCacheManager GetBusinessCacheManager()
        {
            instance = instance ?? new BusinessCacheManager();
            return instance;
        }
        public BusinessCache this[string key]
        {
            get
            {
                return this._cache[key] as BusinessCache;
            }
        }
        public void Add(string key, BusinessCache value)
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
