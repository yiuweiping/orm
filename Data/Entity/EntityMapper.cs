using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Enum;
using Zhengdi.Framework.Extensions;
using Zhengdi.Framework.Reflection;

namespace Zhengdi.Framework.Data.Entity
{
    public abstract class DataEntityMapper<T> : IEntityMapper<T>   where T:DataEntity,new ()
    {
      
        private IEnumerable<DynamicPropertyHandle<DataEntity>> _dynamicPropertyHandles;
       
        public virtual string ServiceKey { get; set; } = "main";
        public abstract  string  TableName { get;  }
 
        public IProperty[] Propertys { get; private set; }
        public IEnumerable<IRelevance> Relevances { get; private set; }

        public virtual IProperty PrimaryKey => this.GetPropertField("Id");

        public DataEntityMapper()
        {
            var t = new T();
            this.Propertys = this.PropertyIniti();
            this.Relevances = this.RelevanceIniti() ?? new RelevanceMap[0];
      ;
        }
        
        public IProperty GetPropertField(string fieldName)
        {
           return  ( from t in this.Propertys where t.Name == fieldName select t).SingleOrDefault();
        }
        public IRelevance GetRelevanceField(string fieldName)
        {
            return (from t in this.Relevances where t.ForeignKey.Name == fieldName select t).SingleOrDefault();
        }

        protected IProperty[] PropertyIniti()
        {
            var ps = this.IgnoreItem() ?? new string[0];
            var query = from p in typeof(T).GetProperties()
                        select new PropertyMap(p.Name, typeof(T).Name, p.PropertyType, null, (ps.Any((m) => p.Name == m) && ps.Count() > 0));
            var d = new DynamicHandlerCompiler<DataEntity>(typeof(T));
            this._dynamicPropertyHandles = from t in query
                                           select new DynamicPropertyHandle<DataEntity>()
                                           {
                                               PropertyName = t.Name,
                                               SetDynamicPropertyHandle = d.CreaterSetPropertyHandler<dynamic>(t.Name),
                                               GetDynamicPropertyHandle = d.CreaterGetPropertyHandler<dynamic>(t.Name)
                                           };

            return query.ToArray();
        }

        protected virtual IEnumerable<string> IgnoreItem() { return null; }
        protected virtual IEnumerable<IRelevance> RelevanceIniti() { return new IRelevance[0]; }
        internal static string GetRelevanceAlias(IEntityMapper<T> mpper)
        {
            return string.Join("|", from t in mpper.Relevances select t.AliaName);
        }
        private DynamicPropertyHandle<DataEntity> GetDynamicHandle(string propertyName)
        {
            return (from t in this._dynamicPropertyHandles where t.PropertyName == propertyName select t).SingleOrDefault();
        }
 
        public Action<T, dynamic,Type> FindSetDynamicHandle(string propertyName)
        {
            return GetDynamicHandle(propertyName).SetDynamicPropertyHandle;
        }

        public Func<T, dynamic> FindGetDynamicHandle(string propertyName)
        {
            return GetDynamicHandle(propertyName).GetDynamicPropertyHandle;
        }

        class DynamicPropertyHandle<K>
        {
            public string PropertyName { get; set; }
            public Action<K, dynamic,Type> SetDynamicPropertyHandle { get; set; }
            public Func<K, dynamic> GetDynamicPropertyHandle { get; set; }

        }
    }
}
