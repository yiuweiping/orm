using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Entity;
using Zhengdi.Framework.Data.Enum;
using Zhengdi.Framework.Reflection;

namespace Zhengdi.Framework.Data.DBHelper
{
    public interface IDBbatBuilder
    {
        CommandType CommandType { get; }
        void UpdateBatBuilder( IWhereGroup where, IList<IProperty> disabled = null);
        void UpdateBatBuilder(IWhereGroup where, IList<UpdateProperty> Property);
        void DeleteBatBuilder(IWhereGroup where);
        void SelectBatBuilder<K>( IWhereGroup where, int num, JoinType Type,IEnumerable<IProperty> disabled, IList<ISorting> sortin);
        void SelectBatBuilder<K1, K2>(IWhereGroup where, int num, JoinType Type, IEnumerable<IProperty> disabled, IList<ISorting> sortin);

        void ScalarBatBuilder<K>(IWhereGroup where, int num, JoinType Type, IEnumerable<Polymerize> polymerizes, IEnumerable<IProperty> disabled, IList<ISorting> sortin);
        void SelectBatBuilder(IWhereGroup where, int num, IEnumerable<Polymerize> polymerizes, IList<ISorting> sortin, IList<IProperty> disabled);
        void SelectBatBuilder(IWhereGroup where, IEnumerable<Polymerize> polymerizes);
        void InsertBatBuilder(IList<IProperty> disabled = null);
        void SetCommandParameter(string text,   IDataParameterCollection Parameters,object entity=null);
        void IncrementBatBuilder();
        void Union(IDBbatBuilder source);
        void Additional(string str, IWhereGroup where);
 
    }

    public class UpdateProperty : IProperty
    {  
        private string _aliaName;
        private string _table;
        public bool Ignore { get; private set; }
        public string Name { get; private set; }
        public bool PrimaryKey { get; private set; }
 
        public PrimaryType PrimaryType { get; set; }

        public Type Type { get; private set; }

        public dynamic Value { get; private set; }

        public UpdateProperty(IProperty property, UpdateAction action)
        {
            this.Name = property.Name;
            this.PrimaryKey = property.PrimaryKey;
            this.PrimaryType = property.PrimaryType;
            this.Type = property.Type;
            this.Value = property.Value;
            this.Ignore = property.Ignore;
            this.Action = action;

        }
        public UpdateAction Action { get; set; }

        public override string ToString()
        {
            string str = this._aliaName == null || this._aliaName == string.Empty ? (this._table == null || this._table == string.Empty ? this.Name : $"{this._table}.[{this.Name}] ") : $" {this._aliaName}.[{this.Name}] as {$"{this._aliaName}_{this.Name}"}";
            return  $" { str} {ConverToString(Action)} @{this.Name}";
        }
        public bool SetAliaName(IRelevance relevanc)
        {
            this._aliaName = relevanc.AliaName;
            return true;
        }
        public bool SetTableName(string name)
        {
            this._table = name;
            return true;
        }
        static string ConverToString(UpdateAction action)
        {
            switch (action)
            {
                case UpdateAction.等于:
                    return "=";
                case UpdateAction.累减:
                    return "-=";
                case UpdateAction.累加:
                    return "+=";
                default:
                    return "=";
            }
        }

        public void SetPrimaryKey()
        {
            PrimaryKey = true;
        }

        public void SetPrimaryKey(PrimaryType Type)
        {
            PrimaryKey = true;
            PrimaryType = Type;
        }

        public bool Equals(IProperty property)
        {
            throw new NotImplementedException();
        }

        public IProperty IsJoin(bool status)
        {
            throw new NotImplementedException();
        }
    }
    public enum UpdateAction
    {
        等于 = 1,
        累加 = 2,
        累减 = 3,
    }
    public enum JoinType : uint
    {
        None = 0,
        Innert = 1,
        Left = 2,
        Right = 3,
        Full = 4,

    }
    public enum PolymerizeType : uint
    {
        None = 0,
        Max = 1,
        Sum = 2,
        Min = 3,
        Count = 4
    }
    public class Polymerize
    {
        public IProperty Property { get; private set; }
        public PolymerizeType Type { get; private set; }

        public string Alias { get; private set; }
        private string _tableName;
        public Polymerize(IProperty property, PolymerizeType type, string alias, string tableName=null)
        {
            this.Property = property;
            this.Type = type;
            this.Alias = alias ?? this.Property.Name;
            this._tableName = tableName;
        }
 
        public override string ToString()
        {
            switch (Type)
            {
                case PolymerizeType.Count:
                    return $"Count( {(string.IsNullOrWhiteSpace(this._tableName) ? string.Empty : $"{this._tableName}.")}{Property.Name}) as {this.Alias}";
                case PolymerizeType.Max:
                    return $"Max({(string.IsNullOrWhiteSpace(this._tableName) ? string.Empty : $"{this._tableName}.")}{Property.Name}) as {this.Alias}";
                case PolymerizeType.Min:
                    return $"Min({(string.IsNullOrWhiteSpace(this._tableName) ? string.Empty : $"{this._tableName}.")}{Property.Name}) as {this.Alias}";
                case PolymerizeType.Sum:
                    return $"Sum({(string.IsNullOrWhiteSpace(this._tableName) ? string.Empty : $"{this._tableName}.")}{Property.Name}) as {this.Alias}";
                default:
                    return Property.Name;
            }
        }
    }
    public class Where : IWhere 
    {
        public string TableName { get; set; }
        public CompareType Compare { get; private set; }

        public string Name { get; private set; }

        public Type Type { get; private set; }
 
        public dynamic Value { get; private set; }

        public Where(string name, dynamic value, CompareType compare, string tableName =null)
        {
            this.Name = name;
            this.Type = value.GetType();
            this.Value = value;
            this.Compare = compare;
            this.TableName = tableName;
        }
        public override string ToString()
        {
            string str = string.Empty;
            switch (Compare)
            {
                case CompareType.Like:
                    str = $"{(string.IsNullOrWhiteSpace(this.TableName) ? this.Name : $"{this.TableName}.{this.Name}")} like @{  this.Name  }";
                    break;
                case CompareType.In:
                    str = $"{(string.IsNullOrWhiteSpace(this.TableName) ? this.Name : $"{this.TableName}.{this.Name}")} in({this.Value})";
                    break;
                case CompareType.NotIn:
                    str = $"{(string.IsNullOrWhiteSpace(this.TableName) ? this.Name : $"{this.TableName}.{this.Name}")} not in ( { this.Value})";
                    break;
                default:
                    str = $"{(string.IsNullOrWhiteSpace(this.TableName) ? this.Name : $"{this.TableName}.{this.Name}")} {(Char)((int)Compare)} @{ this.Name }";
                    break;
            }
            return str;
        }
    }
    public interface IWhere: IField
    {

        string TableName { get; set; }
        CompareType Compare { get; }
    }

    public class WhereGrouping : IWhereGroup
    {
        public IWhereGroup ChildItem { get; set; }
        public WhereSpliceType SpliceType { get; private set; }
        public IList<IWhere> WhereItems { get; private set; }
        public WhereGrouping()
        {
            this.WhereItems = new List<IWhere>();
        }
        public WhereGrouping(WhereSpliceType spliceType) : this()
        {
            this.SpliceType = spliceType;
        }
        public override string ToString()
        {
            string str = $"where {GetWherestring(this)}   ";
            return string.Format("{0} {1}", str, (this.ChildItem == null ? string.Empty : $" {this.ChildItem.SpliceType.ToString()} {GetWherestring(this.ChildItem)} "));

        }

        static string GetWherestring(IWhereGroup wg)
        {
            var s = from t in wg.WhereItems select t.ToString();
            return string.Join($" {wg.SpliceType.ToString()} ", s);
        }
        public dynamic GetValue(string name, IWhereGroup group = null)
        {
            group = group == null ? this : group;
            var v = FindItem(group, name);
            if (v == null && group.ChildItem != null)
            {
                GetValue(name, group.ChildItem);
            }
            return v;
        }
        private static dynamic FindItem(IWhereGroup Group, string name)
        {
            var s = from t in Group.WhereItems where t.Name == name  select t;
            if (s.Count() > 0)
            {
                return s.SingleOrDefault().Value;
          
            }
            return null;
        }

        public IEnumerable<CreaterDynamicClassProperty> GetCreaterDynamicClassPropertys()
        {
            var list = from t in WhereItems select new CreaterDynamicClassProperty(t.Name, t.Value);
            if (ChildItem != null)
            {
                var s = from c in this.ChildItem.WhereItems select new CreaterDynamicClassProperty(c.Name, c.Value);
                list = list.Union(s);
            }
            return list;
        }

        public bool Contains(string filedName)
        {
            var a = from t in this.WhereItems where string.Compare(t.Name, filedName) == 0   select t;
            return a.Count() > 0;
        }
        public bool Contains(IWhere item)
        {
            var a = from t in this.WhereItems where string.Compare(t.Name, item.Name) == 0 && string.Compare(t.TableName, item.TableName) == 0 select t;
            return a.Count() > 0;
        }
    }
    public interface IWhereGroup
    {
        WhereSpliceType SpliceType { get; }
        IList<IWhere> WhereItems { get; }
        bool Contains(string filedName);
        bool Contains(IWhere item);
        IWhereGroup ChildItem { get; set; }

        dynamic GetValue(string name, IWhereGroup group = null);

        IEnumerable<CreaterDynamicClassProperty> GetCreaterDynamicClassPropertys();
    }

    public enum CompareType : uint
    {
        等于 = 0x3D,
        大于 = 0x3E,
        小于 = 0x3C,
        Like = 1,
        In = 2,
        NotIn = 4,
 
    }
    public enum WhereSpliceType : uint
    {
        And = 1,
        OR = 2,
    }
    public class SortGroup : ISorting
    {
        public IEnumerable<IProperty> Propertys { get; private set; }
        private string _tableName;
        public SortType Type { get; private set; }
        public SortGroup(IEnumerable<IProperty> propertys, SortType type)
        {
            this.Propertys = propertys;
            this.Type = type;
        }
        public override string ToString()
        {
            return $" {string.Join(",", (from t in Propertys where t.SetTableName(_tableName) select t))} {this.Type.ToString()}";
        }

        public bool SetTableName(string name)
        {
            this._tableName = name;
            return true;
        }
    }
    public interface ISorting
    {
        bool SetTableName(string name);
        SortType Type { get; }
        IEnumerable<IProperty> Propertys { get; }

    }

    public enum SortType : uint
    {
        Desc = 0,
        Asc = 1,
    }
}
