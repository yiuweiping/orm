using System;
using System.Collections.Specialized;
using System.Text;
using Zhengdi.Framework.Cache;
using Zhengdi.Framework.Data.Enum;
using Zhengdi.Framework.Reflection;

namespace Zhengdi.Framework.Data.Entity
{
    public interface IDynameicHandle
    {
        HybridDictionary DynamicPropertyHandles { get; }
        Type ParamType { get; }
    }

    public class PropertyMap : IProperty
    {

        readonly string entityName;
        private string _aliaName;
        private string _table;
        private bool _join = false;
        public bool Ignore { get; internal set; }
        public string Name { get; private set; }
        public bool PrimaryKey { get; private set; }
        public Type Type { get; private set; }

        public dynamic Value { get; private set; }

        public PrimaryType PrimaryType { get; set; } = PrimaryType.Empty;

 

        public PropertyMap(string name, string entityName)
        {
            this.entityName = entityName;
            this.Name = name;
            this._aliaName = string.Empty;
        }
        public PropertyMap(string name, string entityName, Type type, dynamic value = null, bool ignore = false)
        {
            this.entityName = entityName;
            this.Ignore = ignore;
            this.Name = name;
            this.Type = type;
            this.Value = value;

        }
        public override string ToString()
        {
            string s = !string.IsNullOrWhiteSpace(this._aliaName)&&  this._join  ?  $" {this._aliaName}.{this.Name} as {$"{this._aliaName}_{this.Name}"}": (string.IsNullOrWhiteSpace(this._table) ? this.Name : $"{this._table}.{this.Name}");
            return s;
        }

        public bool SetAliaName(IRelevance relevanc)
        {
            this._aliaName = relevanc.AliaName;
            return true;
        }
        public bool SetTableName(string name)
        {
            this._aliaName = null;
            this._table = name;
            return true;
        }
        public void SetPrimaryKey(PrimaryType type)
        {
            this.PrimaryType = type;
            this.PrimaryKey = true;
        }
 

        public bool Equals(IProperty property)
        {
            var a = property as PropertyMap;
            if (null != a)
            {
                return a.Name == this.Name && this.entityName == a.entityName;
            }
            return false;
        }

        public IProperty IsJoin(bool status)
        {
            this._join = status;
            return this;
        }
    }
    


}
