using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache.Enum;

namespace Zhengdi.Framework.Cache
{
    public class EntityMapperCacheItem : ICacheBase
    {
        readonly string _key;
        public string Key => this._key;
        public dynamic Value { get; internal set; }
        public DateTime Expire { get; set; }
        public CacheType Type { get; set; }
        public EntityMapperCacheItem(string key) { this._key = key; }

    }
}
