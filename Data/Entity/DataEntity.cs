using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache;

namespace Zhengdi.Framework.Data.Entity
{
    [Serializable]
    public abstract class DataEntity : IEntity, ICloneable
    {
 
        public IField GetFilter(string fieldName)
        {
            return EntityMapperCacheManager.GetMapperCacheManager()[this.GetType().Name].Value.GetPropertField(fieldName);
        }
        public IEnumerable<IField> GetFieldItems()
        {
            return EntityMapperCacheManager.GetMapperCacheManager()[this.GetType().Name].Value.Propertys;
        }

        public T DeepClone<T>() where T : class, new()
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制     
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }
 
        public T ShallowClone<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
