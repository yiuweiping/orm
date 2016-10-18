using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache;
using Zhengdi.Framework.Data.Entity;
using Zhengdi.Framework.Reflection;

namespace Zhengdi.Framework.Data.DBHelper.Sql
{
    

    public class SqlDBbatBuilder<T> : DBbatBuilder<T>
    {
      
        readonly IEntityMapper<T> _mapper;
 
        private IList<IProperty> _propertys;
 
        public SqlDBbatBuilder(IEntityMapper<T> mapper ) {
            this._mapper = mapper;
        }
        public  override void DeleteBatBuilder( IWhereGroup where)
        {
            this._sqltext = new StringBuilder($"delete {this._mapper.TableName} {(where == null ? string.Empty : where.ToString())}");
            this.where = where;

        }
        public override void InsertBatBuilder(IList<IProperty> disabled = null)
        {
            this._propertys = SqlDBbatBuilder<T>.FilterProperty(_mapper, disabled);
            this._sqltext = new StringBuilder();
            this._sqltext.Append($"insert into {this._mapper.TableName}");
            this._sqltext.Append($"({string.Join(",", this._propertys)}) values");
            this._sqltext.AppendLine($"({string.Join(",", CreaterParamersByName(this._propertys))})");
             
        }
        public override void UpdateBatBuilder(IWhereGroup where, IList<UpdateProperty> Property)
        {
            this._sqltext = new StringBuilder($"update {this._mapper.TableName} set {string.Join(",", Property)}  {(where == null ? string.Empty : where.ToString())}");
            this.where = where;
        }
        public override void UpdateBatBuilder(IWhereGroup where, IList<IProperty> disabled = null)
        {
            var propertys = SqlDBbatBuilder<T>.FilterUpdateProperty(_mapper, disabled);
            this._sqltext = new StringBuilder();
            this._sqltext.Append($"update {this._mapper.TableName} set {string.Join(",", propertys)}  {(where == null ? string.Empty : where.ToString())}");
            this.where = where;
        }
        public override void SelectBatBuilder<K>( IWhereGroup where, int num, JoinType Type,IEnumerable<IProperty> disabled, IList<ISorting> sortin )
        {
            sortin = InitiSorting(this._mapper,sortin);
            var kMapper = (IEntityMapper<K>)EntityMapperCacheManager.GetMapperCacheManager()[typeof(K).Name].Value;
            var fok = (from t in _mapper.Relevances where t.Type == typeof(K) select t).SingleOrDefault();
            var relevanceStr = string.Empty;
            this._propertys = FilterProperty<K>(this._mapper, kMapper, fok, Type, out relevanceStr);
            this._sqltext = new StringBuilder();
            this._sqltext.Append($"select  {(num > 0 ? $" top({num})" : string.Empty) }   {string.Join(",", this._propertys)} ");
            this._sqltext.Append($"from {relevanceStr}  {(where == null ? string.Empty : where.ToString())}");
            this._sqltext.Append(sortin == null ? string.Empty : $" order by {string.Join(",", sortin)} ");
            this.where = where;
        }
        public override void SelectBatBuilder<K1, K2>(IWhereGroup where, int num, JoinType Type, IEnumerable<IProperty> disabled, IList<ISorting> sortin)
        {
            sortin = InitiSorting(this._mapper, sortin);
            var mp = EntityMapperCacheManager.GetMapperCacheManager();
            var k1Mapper = (IEntityMapper<K1>)mp[typeof(K1).Name].Value;
            var k2Mapper = (IEntityMapper<K2>)mp[typeof(K2).Name].Value;
            var FK = (from t in _mapper.Relevances where t.Type == typeof(K1) || t.Type == typeof(K2) select t).ToArray();
            var relevanceStr = string.Empty;
            this._propertys = FilterProperty<K1, K2>(this._mapper, k1Mapper, k2Mapper, FK, Type, out relevanceStr, disabled);
            var filed_str = string.Join(",", this._propertys);

            this._sqltext = new StringBuilder();
            this._sqltext.Append($"select  {(num > 0 ? $" top({num})" : string.Empty) }   {string.Join(",", this._propertys)} ");
            this._sqltext.Append($"from {relevanceStr}  {(where == null ? string.Empty : where.ToString())}");
            this._sqltext.Append(sortin == null ? string.Empty : $" order by {string.Join(",", sortin)} ");
            this.where = where;
        }

        public override void ScalarBatBuilder<K>(IWhereGroup where, int num, JoinType Type, IEnumerable<Polymerize> polymerizes, IEnumerable<IProperty> disabled, IList<ISorting> sortin)
        {
            sortin = InitiSorting(this._mapper, sortin);
            var kMapper = (IEntityMapper<K>)EntityMapperCacheManager.GetMapperCacheManager()[typeof(K).Name].Value;
            var fok = (from t in _mapper.Relevances where t.Type == typeof(K) select t).SingleOrDefault();
            var relevanceStr = string.Empty;
            this._propertys = FilterProperty<K>(this._mapper, kMapper, fok, Type, out relevanceStr, disabled, polymerizes);
            var filed_str = string.Join(",", this._propertys);
            var polymerizeText = polymerizes == null ? string.Empty : $"{string.Join(",", polymerizes)} {(string.IsNullOrWhiteSpace(filed_str)?string.Empty:",")}";
            this._sqltext = new StringBuilder();
            this._sqltext.Append($"select {string.Join(",", polymerizes)}  ");
            this._sqltext.Append($"from {relevanceStr}  {(where == null ? string.Empty : where.ToString())}");
            this.where = where;
        }
        public override void SelectBatBuilder(IWhereGroup where, int num, IEnumerable<Polymerize> polymerizes, IList<ISorting> sortin, IList<IProperty> disabled)
        {
            string polymerizeText;
            sortin = InitiSorting(this._mapper,sortin);
            this._propertys = SqlDBbatBuilder<T>.FilterProperty(_mapper, polymerizes, disabled ?? new List<IProperty>(), out polymerizeText);
            this._sqltext = new StringBuilder();
            this._sqltext.Append($"select  {(num > 0 ? $" top({num})" : string.Empty) } {polymerizeText} {string.Join(",", this._propertys)} ");
            this._sqltext.Append($"from {this._mapper.TableName} {(where == null ? string.Empty : where.ToString())}");
            this._sqltext.Append(sortin == null ? string.Empty : $" order by {string.Join(",", sortin)} ");
            this._sqltext.Append(polymerizes == null ? string.Empty : $" group by {string.Join(",", this._propertys)} ");
            this.where = where;
        }
        public override void SelectBatBuilder(IWhereGroup where, IEnumerable<Polymerize> polymerizes)
        {
            this._sqltext = new StringBuilder();
            this._sqltext.Append($"select {string.Join(",", polymerizes)}  from { this._mapper.TableName} {(where == null ? string.Empty : where.ToString())} ");
            this.where = where;
        }
        
        public override string ToString()
        {
            return this._sqltext.ToString();
        }
 
        public override void IncrementBatBuilder()
        {
            if (this._sqltext != null)
                this._sqltext.AppendLine($"select IDENT_CURRENT('{this._mapper.TableName}')");
            else
                new Exception("请先获取执行动作");
        }
        protected override object CreaterParamger(string name, object value)
        {
             return  new SqlParameter(name, value);
 
        }
        protected override object CreaterParamger(string name, object value,  DbType type)
        {
            return new SqlParameter(name, value) {    DbType= type };
        }
    }
}
