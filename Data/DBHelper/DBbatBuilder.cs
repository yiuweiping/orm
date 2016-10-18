using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Zhengdi.Framework.Data.Entity;
using Zhengdi.Framework.Reflection;
using Zhengdi.Framework.Extensions;

namespace Zhengdi.Framework.Data.DBHelper
{
    public abstract class DBbatBuilder<T> : IDBbatBuilder
    {
        protected IWhereGroup where;
        protected StringBuilder _sqltext;
        public CommandType CommandType { get; }

        public abstract void DeleteBatBuilder(IWhereGroup where);
        public abstract void IncrementBatBuilder();
        public abstract void InsertBatBuilder(IList<IProperty> disabled = null);
        public abstract void SelectBatBuilder(IWhereGroup where, IEnumerable<Polymerize> polymerizes);
        public abstract void SelectBatBuilder(IWhereGroup where, int num, IEnumerable<Polymerize> polymerizes, IList<ISorting> sortin, IList<IProperty> disabled);
        public abstract void SelectBatBuilder<K>(IWhereGroup where, int num, JoinType Type, IEnumerable<IProperty> disabled, IList<ISorting> sortin);
        public abstract void ScalarBatBuilder<K>(IWhereGroup where, int num, JoinType Type, IEnumerable<Polymerize> polymerizes, IEnumerable<IProperty> disabled, IList<ISorting> sortin);
        public void SetCommandParameter(string text, IDataParameterCollection Parameters, object entity = null)
        {
            var s = Regex.Matches(text, "@\\w+(?=[\\s|,|)])|@\\w+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var d = new DynamicHandlerCompiler<object>(entity);
            foreach (Match a in s)
            {
                var name = a.Value.Replace("@", string.Empty);

                if (this.where != null)
                {
                    if (where.Contains(name))
                    {
                  
                        var p = CreaterParamger(a.Value, where.GetValue(name)) as IDataParameter;
                      
                        Parameters.Add(p);
                        continue;
                    }

                }
                if (entity != null)
                {
                    var f = d.CreaterGetPropertyHandler<dynamic>(name);
                    if (Parameters.Contains(a.Value))
                        (Parameters[a.Value] as DbParameter).Value = f(entity)??string.Empty;
                    else
                        Parameters.Add(CreaterParamger(a.Value, f(entity) ?? string.Empty) as IDataParameter);
                }
            }
        }
        public static void SetCommandParameter(string text, IDataParameterCollection Parameters,Func<string,object,IDataParameter> CreaterParamger, object entity = null)
        {
            var s = Regex.Matches(text, "@\\w+(?=[\\s|,|)])|@\\w+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var d = new DynamicHandlerCompiler<object>(entity);
            foreach (Match a in s)
            {
                var name = a.Value.Replace("@", string.Empty);
                if (entity != null)
                {
                    var f = d.CreaterGetPropertyHandler<dynamic>(name);
                    if (Parameters.Contains(a.Value))
                        (Parameters[a.Value] as DbParameter).Value = f(entity) ?? string.Empty;
                    else
                        Parameters.Add(CreaterParamger(a.Value, f(entity) ?? string.Empty) as IDataParameter);
                }
            }
        }
        public abstract void UpdateBatBuilder(IWhereGroup where, IList<UpdateProperty> Property);
        public abstract void UpdateBatBuilder(IWhereGroup where, IList<IProperty> disabled = null);
        public void Union(IDBbatBuilder source)
        {
            var a = source as DBbatBuilder<T>;
            this._sqltext.AppendLine("union");
            this._sqltext.AppendLine(source.ToString());
            if (a.where != null)
                this.where.ChildItem = a.where;
        }
        public void Additional(string str, IWhereGroup where)
        {
            this._sqltext = this._sqltext ?? new StringBuilder();
            this._sqltext.AppendLine(str);
            this.where = where;
        }
        
        protected abstract object CreaterParamger(string name, object value);
        protected abstract object CreaterParamger(string name, object value,DbType type);
        internal static IList<ISorting> InitiSorting(IEntityMapper<T> mapper, IList<ISorting> sortin )
        {
            if(sortin ==null)
            {
                sortin = sortin ?? new List<ISorting>();
                sortin.Add(new SortGroup(new IProperty[] { mapper.PrimaryKey.IsJoin(false) }, SortType.Desc));
                sortin = (from t in sortin where t.SetTableName(mapper.TableName) select t).ToList();
            }
            return sortin;
        }
        protected static IEnumerable<string> CreaterParamersByName(IEnumerable<IProperty> property)
        {
            return from t in property select $"@{t.Name}";
        }
        internal static IList<IProperty> FilterProperty<K>(IEntityMapper<T> mapper, IEntityMapper<K> kMapper, IRelevance relevance, JoinType join, out string relevanceStr, IEnumerable<IProperty> disabled = null, IEnumerable<Polymerize> polymerizes = null)
        {
            disabled = disabled ?? new IProperty[0];
            var p1 = polymerizes == null ? from t in mapper.Propertys where !(t.Ignore || disabled.Contains((m) => m.Equals(t))) && t.SetTableName(mapper.TableName) select t.IsJoin(true) :
                from t in mapper.Propertys
                where !(t.Ignore || disabled.Contains((m) => m.Equals(t)) || polymerizes.Contains((m) => m.Property.Name == t.Name)) && t.SetTableName(mapper.TableName)
                select t.IsJoin(true)
                ;
            var p2 = polymerizes == null ? from t in kMapper.Propertys
                                           where !(t.Ignore || disabled.Contains((m) => m.Equals(t))) && t.SetAliaName(relevance)
                                           select t.IsJoin(true) :
               from t in kMapper.Propertys
               where !(t.Ignore || disabled.Contains((m) => m.Equals(t)) || !polymerizes.Contains((m) => m.Property.Name == t.Name)) && t.SetAliaName(relevance)
               select t.IsJoin(true)
                ;
            relevanceStr = $"{mapper.TableName}  { JoinString(join)} {kMapper.TableName} as {relevance.AliaName} on {relevance.AliaName}.{relevance.PrimaryKey.Name}= {mapper.TableName}.{relevance.ForeignKey.Name}";
            return p1.Union(p2).ToList();
        }
        internal static IList<IProperty> FilterProperty<K1, K2>(IEntityMapper<T> mapper, IEntityMapper<K1> k1Mapper, IEntityMapper<K2> k2Mapper, IRelevance[] relevances, JoinType join, out string relevanceStr, IEnumerable<IProperty> disabled = null)
        {
            var s = new string[2];
            var p1 = DBbatBuilder<T>.FilterProperty<K1>(mapper, k1Mapper, relevances[0], join, out s[0], disabled);
            var p2 = DBbatBuilder<T>.FilterProperty<K2>(mapper, k2Mapper, relevances[1], join, out s[1], disabled);
            int index = s[1].IndexOf(JoinString(join));
            s[1] = s[1].Substring(index - 1);
            relevanceStr = string.Join(" ", s);
            return p1.Union(p2).ToList();
        }
        static string JoinString(JoinType join)
        {
            switch (join)
            {
                case JoinType.Innert:
                    return "inner join";
                case JoinType.Left:
                    return "left join";
                case JoinType.Right:
                    return "right join";
                default:
                    return "";
            }
        }
        internal static IList<IProperty> FilterProperty(IEntityMapper<T> mapper, IEnumerable<Polymerize> polymerizes, IList<IProperty> disabled, out string polymerizeText )
        {
            var property = polymerizes == null ? (from t in mapper.Propertys where !t.Ignore select t.IsJoin(false)).ToList() : (from t in mapper.Propertys
                                                                                                                   where !(polymerizes.All((m) => m.Property.Name == t.Name) &&
                                                                                                                   disabled.All((m) => m.Name == t.Name) && t.Ignore)
                                                                                                                                              select t.IsJoin(false)).ToList();
            polymerizeText = polymerizes == null ? string.Empty : $"{string.Join(",", polymerizes)} ,";
            return property;
        }
        internal static IList<IProperty> FilterProperty(IEntityMapper<T> mapper, IList<IProperty> disabled, bool disablePK = false)
        {
            var property = disabled == null ? (from t in mapper.Propertys where !t.Ignore && !(disablePK || t.PrimaryType == Enum.PrimaryType.Increment) select t.IsJoin(false)).ToList() : (from t in mapper.Propertys
                                                                                                                                                                               where !(disabled.All((m) => m.Name == t.Name) && t.Ignore)
                                                                                                                                                                               select t.IsJoin(false)).ToList();

            return property;
        }
        protected static IList<UpdateProperty> FilterUpdateProperty(IEntityMapper<T> mapper, IList<IProperty> disabled)
        {
            var property = disabled == null ?
                (from t in mapper.Propertys where !(t.Ignore || t.PrimaryKey) select new UpdateProperty(t, UpdateAction.等于)).ToList() :
                (from t in mapper.Propertys
                 where !(disabled.Contains((m) =>string.Compare(m.Name, t.Name) == 0)  ||t.Ignore || t.PrimaryKey)
                 select new UpdateProperty(t, UpdateAction.等于)).ToList();
            return property;
        }

        public abstract void SelectBatBuilder<K1, K2>(IWhereGroup where, int num, JoinType Type, IEnumerable<IProperty> disabled, IList<ISorting> sortin);
    }
}
