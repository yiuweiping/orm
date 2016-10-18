using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache;
using Zhengdi.Framework.Data.Entity;
using Zhengdi.Framework.Reflection;

namespace Zhengdi.Framework.Data.DBHelper
{
    internal class ReaderDataEntity
    {

        readonly IEnumerable<MpperRelevanceItem> _mpperItems;
        readonly DataEntity _entitiy;
        private ReaderDataEntity(DataEntity entitiy, IEnumerable<MpperRelevanceItem> mpperItems)
        {
            this._mpperItems = mpperItems;
            this._entitiy = entitiy;
        }
        internal static ReaderDataEntity CreaterReaderDataEntity<T>(T entity, IDataReader read, IEntityMapper<T> mapper) where T : DataEntity, new()
        {
            List<MpperRelevanceItem> s = GetMpperRelevanceItems(entity, read, mapper, (Isrex, Kv) =>
            {
                var p = mapper.GetPropertField(Kv.Value);
                return p == null ? null : new MpperRelevanceItem(entity, p, Kv.Key);
            });
            return new ReaderDataEntity(entity, s);
        }
        internal static ReaderDataEntity CreaterReaderDataEntity<T, K>(T entity, IDataReader read, IEntityMapper<T> mapper) where T : DataEntity, new() where K : DataEntity, new()
        {
            var dic = new Dictionary<string, DataEntity>();
            Type kt1 = typeof(K);
            foreach (var a in mapper.Relevances)
            {
                if (a.Type == kt1)
                {
                    dic.Add(a.AliaName, new K());
                }
            }
            List<MpperRelevanceItem> s = GetMpperRelevanceItems(entity, read, mapper, (isrex, Kv) =>
            {
                var p = mapper.GetPropertField(Kv.Value);
                return isrex ? GetForeignDataEntity<T>(Kv.Value, Kv.Key, mapper, dic,kt1.Name) : p == null ? null : new MpperRelevanceItem(entity, p, Kv.Key);
            });
            var fk = (from t in mapper.Relevances where t.Type == typeof(K) select t).SingleOrDefault();
            mapper.FindSetDynamicHandle(fk.EntityProperty.Name)(entity, dic[fk.AliaName], fk.EntityProperty.Type);
           // fk.EntityProperty.SetValue(entity, dic[fk.AliaName]);
            return new ReaderDataEntity(entity, s);
        }
        internal static ReaderDataEntity CreaterReaderDataEntity<T, K1, K2>(T entity, IDataReader read, IEntityMapper<T> mapper)
            where T : DataEntity, new()
            where K1 : DataEntity, new()
            where K2 : DataEntity, new()
        {
            var dic = new Dictionary<string, DataEntity>();
            Type kt1 = typeof(K1), kt2 = typeof(K2);
            foreach (var a in mapper.Relevances)
            {
                if (a.Type == kt1)
                {
                    dic.Add(a.AliaName, new K1());
                }
                if (a.Type == kt2)
                {
                    dic.Add(a.AliaName, new K2());
                }
            }
            List<MpperRelevanceItem> s = GetMpperRelevanceItems(entity, read, mapper, (isrex, Kv) =>
            {
                var p = mapper.GetPropertField(Kv.Value);
                return isrex ? GetForeignDataEntity<T>(Kv.Value, Kv.Key, mapper, dic,kt1.Name,kt2.Name) : p == null ? null : new MpperRelevanceItem(entity, p, Kv.Key);
            });
             
          
            var fk = (from t in mapper.Relevances where t.Type == kt2 || t.Type == kt1 select t).ToArray();
            mapper.FindSetDynamicHandle(fk[0].EntityProperty.Name)(entity, dic[fk[0].AliaName], fk[0].EntityProperty.Type);
            mapper.FindSetDynamicHandle(fk[1].EntityProperty.Name)(entity, dic[fk[1].AliaName], fk[1].EntityProperty.Type);
            // fk.EntityProperty.SetValue(entity, dic[fk.AliaName]);
            return new ReaderDataEntity(entity, s);

        }
        private static List<MpperRelevanceItem> GetMpperRelevanceItems<T>(T entity, IDataReader read, IEntityMapper<T> mapper, Func<bool, KeyValuePair<int, string>, MpperRelevanceItem> method) where T : DataEntity, new()
        {
            var rex = mapper.Relevances.Count() > 0 ? $"^[{DataEntityMapper<T>.GetRelevanceAlias(mapper)}]_.$*" : ".*";
            var s = new List<MpperRelevanceItem>(read.GetSchemaTable().Columns.Count);
            var t = read.GetSchemaTable();
            foreach (DataRow r in t.Rows)
            {
                var keyValue = new KeyValuePair<int, string>((int)r["ColumnOrdinal"], r["ColumnName"].ToString());
                var p = method(Regex.IsMatch(keyValue.Value, rex, RegexOptions.IgnoreCase), keyValue);
                if (p != null)
                    s.Add(p);
            }
            return s;
        }
        private static MpperRelevanceItem GetForeignDataEntity<T>(string columnName, int index, IEntityMapper<T> mapper, Dictionary<string, DataEntity> cache, params string[] typename)
        {
            int indexStr = columnName.IndexOf("_");
            var a = columnName.Substring(0, indexStr);
            var fieldName = columnName.Substring(indexStr += 1, columnName.Length - indexStr);
            IProperty p;
            foreach (var t in typename)
            {
                p = EntityMapperCacheManager.GetMapperCacheManager()[t].Value.GetPropertField(fieldName) as IProperty;
                if (p != null)
                    return new MpperRelevanceItem(cache[a], p, index);
            }
            return null;
        }
 

        public void FullDataEntity<T>(IList<T> entitys, IDataReader read) where T : DataEntity, new()
        {
            while (read.Read() && !read.IsClosed)
            {
                var objs = new object[this._mpperItems.Count()];
                for (var i = 0; i < objs.Length; i++)
                {
                    this._mpperItems.ElementAt(i).SetValue(read[i]);
                }
                entitys.Add(_entitiy.DeepClone<T>());
            }
        }
        class MpperRelevanceItem 
        {
            readonly Action<DataEntity,dynamic,Type> setHandler;
            readonly DataEntity entity;
            readonly IProperty property;
            readonly int index;
            public DataEntity Entity => entity;
            public int Index => index;
            public IProperty Property => property;
            public MpperRelevanceItem(DataEntity entity, IProperty property, int index)
            {
 
                this.entity = entity;
                this.property = property;
                this.index = index;
                this.setHandler = EntityMapperCacheManager.GetMapperCacheManager()[entity.GetType().Name].Value.FindSetDynamicHandle(property.Name);
            }
            public void SetValue(object value)
            {
                try
                {
                    var t = value.GetType();
                    value = t == typeof(DBNull) ? DbDataType.DefaultValue(property.Type) : Convert.ChangeType(value, t);
                    this.setHandler(this.entity, value, property.Type.IsEnum ? typeof(int) : property.Type);
                }
                catch (Exception ee)
                {
                    throw ee;
                }

            }
        }
    }
}
