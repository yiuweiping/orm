using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache;

namespace Zhengdi.Framework.Data.Entity
{
    public class RelevanceMap : IRelevance
    {
        public string AliaName { get; private set; }
        public IProperty ForeignKey { get; private set; }
        public IProperty PrimaryKey { get; private set; }
        public IProperty EntityProperty { get; private set; }
        public Type Type { get; private set; }

        public RelevanceMap(string aliaName, IProperty foreignKey, IProperty entity, string primaryKeyName =null)
        {
            this.AliaName = aliaName;
            this.ForeignKey = foreignKey;
            this.Type = entity.Type;
            this.EntityProperty = entity;
            var a =   EntityMapperCacheManager.GetMapperCacheManager()[entity.Type.Name];
            this.PrimaryKey = string.IsNullOrWhiteSpace(primaryKeyName) ? a.Value.PrimaryKey : a.Value.GetPropertField(primaryKeyName);
        }

 
    }
}
