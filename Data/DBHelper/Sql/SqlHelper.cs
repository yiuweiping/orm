using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache;
using Zhengdi.Framework.Data.Entity;
using Zhengdi.Framework.Error;
using Zhengdi.Framework.Reflection;

namespace Zhengdi.Framework.Data.DBHelper.Sql
{

    public class SqlHelpe<T> : DBHelperBase<T> where T : DataEntity, new()
    {
        public override void Select<K>(IList<T> entitys, int num, IWhereGroup where, JoinType Type, IEnumerable<IProperty> disabled)
        {
            this.BatBuilder.SelectBatBuilder<K>(where, num, Type, disabled, null);
            this.ExecuteReader<K>(entitys);
        }
        public override void Select<K>(IList<T> entitys,int num ,IWhereGroup where, JoinType Type,IEnumerable<IProperty> disabled, IList<ISorting> sorting=null) 
        {
            this.BatBuilder.SelectBatBuilder<K>(where, num, Type, disabled, sorting);
            this.ExecuteReader<K>(entitys);
        }
        public override void Select<K1,K2>(IList<T> entitys, int num, IWhereGroup where, JoinType Type, IEnumerable<IProperty> disabled)
        {
            this.BatBuilder.SelectBatBuilder<K1,K2>(where, num, Type, disabled, null);
            this.ExecuteReader<K1,K2>(entitys);
        }
        public override void Select(IList<T> entitys, IWhereGroup wheres, IList<IProperty> disabled = null, IList<ISorting> sortin = null)
        {
            this.Select(entitys, -1, wheres, null, disabled, sortin);
        }
        public override void Select(T entity, IWhereGroup wheres, IEnumerable<Polymerize> polymerizes, IList<IProperty> disabled = null, IList<ISorting> sortin = null)
        {
            throw new NotImplementedException();
        }
        public override void Select(IList<T> entitys, int num, IWhereGroup wheres, IEnumerable<Polymerize> polymerizes, IList<IProperty> disable, IList<ISorting> sortin)
        {
            this.BatBuilder.SelectBatBuilder(wheres, num, polymerizes, sortin, disable);
            this.ExecuteReader(entitys);
        }
        protected override IDBbatBuilder GetDBbatBuilder()
        {
           return   new SqlDBbatBuilder<T>(this.Mapper);
        }
        protected override IDbConntion GetIDbConntion()
        {
            return new SqlDbConntionService();
        }
        public override dynamic Insert(T entity)
        {
            object rownum;
            this.BatBuilder.InsertBatBuilder();
            this.BatBuilder.IncrementBatBuilder();
            this.ExecuteScalar(entity, out rownum);
            return rownum;
        }
        public override dynamic Update(T entity, IWhereGroup where, IList<IProperty> disabled = null )
        {
            int rownum;
            this.BatBuilder.UpdateBatBuilder(where, disabled);
            this.ExecuteNonQuery(entity, out rownum);
            return rownum;
        }

        public override dynamic Update(T entity, IList<UpdateProperty> propertys, IWhereGroup where  )
        {
            int rownum;
            this.BatBuilder.UpdateBatBuilder(where, propertys);
            this.ExecuteNonQuery(entity, out rownum);
            return rownum;
        }

        public override dynamic Delete(IWhereGroup where)
        {
            if (where == null)
                throw new DbExecuteException("删除命令 条件参数不可以为空");
            int rownum;
            this.BatBuilder.DeleteBatBuilder(where);
            this.ExecuteNonQuery(null, out rownum);
            return rownum;

        }

        public override int PagingByList(IList<T> entitys, IPaing paing, IWhereGroup wheres, IList<ISorting> sortin = null, IList<IProperty> disabled = null)
        {
            object rownum,s = null;
            if (wheres != null)
            {
                var d = new DynamicHandlerCompiler<object>();
                var itmes = wheres.GetCreaterDynamicClassPropertys();
                var type = d.CreaterAnonEntity("anonClass", itmes);
                s = Activator.CreateInstance(type, CreaterDynamicClassProperty.GetValues(itmes));
            }
            this.BatBuilder.SelectBatBuilder(wheres, new Polymerize[] { new Polymerize(this.Mapper.PrimaryKey, PolymerizeType.Count, "a") });
            this.ExecuteScalar(s, out rownum);
            paing.SetCount((int)rownum);
            if (paing.PageNum > 1)
            {

                var p = SqlDBbatBuilder<T>.FilterProperty(this.Mapper, disabled);
                sortin = SqlDBbatBuilder<T>.InitiSorting(this.Mapper, sortin);
                var str = new StringBuilder($"select  * from  ");
                str.Append($"(select  top({paing.PageNumber * paing.Showline})  {string.Join(",", p)}, row_number() over(order by {string.Join(",", sortin)} ) as row  from {this.Mapper.TableName}    ");
                str.Append($"{(wheres == null ? string.Empty : wheres.ToString())}) as paging ");
                str.Append($"where row>{paing.Showline * (paing.PageNumber - 1)}");
                var bar = new SqlDBbatBuilder<T>(this.Mapper);
                bar.Additional(str.ToString(), wheres);
                this.Execute(entitys, bar, s);
            }
            else
            {
                this.Select(entitys, wheres, disabled, sortin);
            }
            return paing.Count;

        }
        public override T SelectSingle(IWhereGroup wheres, IEnumerable<Polymerize> polymerizes, IList<IProperty> disable)
        {
            IList<T> list = new List<T>();
            this.Select(list, 1, wheres, polymerizes, disable, null);
            return list.SingleOrDefault();
        }
        public override T SelectSingle<K>(IWhereGroup wheres, JoinType join, IEnumerable<IProperty> disable)  
        {
            IList<T> list = new List<T>();
            this.Select<K>(list, 1, wheres, join, disable);
            return list.SingleOrDefault();
        }

        public override int PagingByList<K>(IList<T> entitys, IWhereGroup wheres, IPaing paing, JoinType Type, IList<ISorting> sortin = null, IList<IProperty> disabled = null)
              
        {
            object rownum, s = null;
            if (wheres != null)
            {
                var d = new DynamicHandlerCompiler<object>();
                var itmes = wheres.GetCreaterDynamicClassPropertys();
                var type = d.CreaterAnonEntity("anonClass", itmes);
                s = Activator.CreateInstance(type, CreaterDynamicClassProperty.GetValues(itmes));
            }
            this.BatBuilder.ScalarBatBuilder<K>(wheres, -1, Type, new Polymerize[] { new Polymerize(this.Mapper.PrimaryKey, PolymerizeType.Count, "a",this.Mapper.TableName) }, null, null);
            this.ExecuteScalar(s, out rownum);
            paing.SetCount((int)rownum);
            if (paing.PageNum > 1)
            {
                sortin = SqlDBbatBuilder<T>.InitiSorting(this.Mapper, sortin);
                var kMapper = (IEntityMapper<K>)EntityMapperCacheManager.GetMapperCacheManager()[typeof(K).Name].Value;
                var fok = (from t in this.Mapper.Relevances where t.Type == typeof(K) select t).SingleOrDefault();
                var relevanceStr = string.Empty;
                var p = SqlDBbatBuilder<T>.FilterProperty<K>(this.Mapper, kMapper, fok, Type, out relevanceStr);
 
                var str = new StringBuilder($"select * from  ");
                str.Append($"(select  top({paing.PageNumber * paing.Showline}) {string.Join(",", p)}, row_number() over(order by {string.Join(",", sortin)} ) as row  from {relevanceStr}    ");
                str.Append($"{(wheres == null ? string.Empty : wheres.ToString())}) as paging ");
                str.Append($"where row>{paing.Showline * (paing.PageNumber - 1)}");
                var bar = new SqlDBbatBuilder<T>(this.Mapper);
                bar.Additional(str.ToString(), wheres);
                this.Execute<K>(entitys, bar, s);
            }
            else
            {
                this.Select<K>(entitys, paing.Showline, wheres, Type, disabled);
            }
            return paing.Count;
        }

        protected override IDataParameter CreaterParamger(string name, object value)
        {
            return new System.Data.SqlClient.SqlParameter(name, value);
        }
    }
}
