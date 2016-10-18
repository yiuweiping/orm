using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Entity;
using Zhengdi.Framework.Reflection;
using Zhengdi.Framework.Cache.Enum;

namespace Zhengdi.Framework.Cache
{
    public class EntityMapperCacheManager : ICacheManager<EntityMapperCacheItem>, IReflectScannble
    {
        System.Reflection.Assembly _assembly;
        HybridDictionary _dic;
        IEnumerable<Type> _types;
        bool statusBit = false;
        private static EntityMapperCacheManager instance;
        public EntityMapperCacheItem this[string key]
        {
            get
            {
                return this.ContainsKey(key) ? this._dic[key] as EntityMapperCacheItem : GetEntityMapperCacheItem(key, _assembly, _types);
            }
        }
        private EntityMapperCacheManager(System.Reflection.Assembly assembly) { this._dic = new HybridDictionary();  this._assembly = assembly; }
        public static EntityMapperCacheManager GetMapperCacheManager(System.Reflection.Assembly assembly =null)
        {
            instance = instance ?? new EntityMapperCacheManager(assembly); 
            return instance;
        }
        public void Run()
        {
            if (!this.statusBit)
            {
                this._types = from type in this._assembly.GetTypes()
                              let interfaceType = type.GetInterface(typeof(IEntityMapper<>).FullName)
                              where interfaceType != null && interfaceType.GetGenericArguments().Count()>0
                              select type;
                TaskAsyncLoad(this, this._types);
            }
            this.statusBit = true;
        }
        static async void TaskAsyncLoad(EntityMapperCacheManager cache, IEnumerable<Type> types) => await Task.Run(() =>
        {
            foreach (var t in types)
            {
                var et = t.BaseType.GetGenericArguments().SingleOrDefault();
                if (et != null)
                {
                    var entityName = et.Name;
                    if (!cache.ContainsKey(entityName))
                    {
                        var e = new EntityMapperCacheItem(entityName)
                        {
                            Value = ReflectionMethod.CreateObject(cache._assembly, t),
                            Expire = DateTime.MaxValue,
                            Type = CacheType.Application
                        };
                        cache.Add(e.Key, e);
                    }
                }
            }
        });
        static EntityMapperCacheItem GetEntityMapperCacheItem(string key, System.Reflection.Assembly assembly, IEnumerable<Type> types)
        { 
            foreach (var t in types)
            {
                var et = t.BaseType.GetGenericArguments().SingleOrDefault();
                var entityName = et.Name;
                if (entityName == key)
                {
                    var e = new EntityMapperCacheItem(entityName)
                    {
                        Value = ReflectionMethod.CreateObject(assembly, t),
                        Expire = DateTime.MaxValue,
                        Type = CacheType.Application
                    };
                    return e;
                }

            }
            return null;
        }
 
        public void Add(string key, EntityMapperCacheItem value)
        {
            this._dic.Add(key, value);
        }

        public void Remove(string key)
        {
            this._dic.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return  this._dic.Contains(key);
        }
    }

}
