using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Entity;

namespace Zhengdi.Framework.Data.DBHelper
{
    public abstract  class DALBase<T> : IDbRepository<T> where T : DataEntity, new()  
    {
        protected DBHelperBase<T> DbService { get; private set; }
        public DALBase()
        {
            this.DbService = this.Creater();
         

        }
        protected abstract DBHelperBase<T> Creater();

        public dynamic Delete(dynamic pkValue)
        {
            var where = new WhereGrouping(WhereSpliceType.And);
            where.WhereItems.Add(new Where(this.DbService.Mapper.PrimaryKey.Name, pkValue, CompareType.等于));
            return this.DbService.Delete(where);
        }
        public virtual dynamic Delete(IWhereGroup where)
        {
            return this.DbService.Delete(where);
        }
        public IEnumerable<T> GetList()
        {
            var es = new List<T>();
            this.DbService.Select(es, null);
            return es;
        }


        public IEnumerable<T> GetList<TForeign>(IWhereGroup where, JoinType join, IEnumerable<IProperty> disabled, IList<ISorting> sortin = null) where TForeign : DataEntity, new()
        {
            var es = new List<T>();
            this.DbService.Select<TForeign>(es, -1, where, join, disabled, sortin);
            return es;
        }
        public IEnumerable<T> GetList<TForeign>(IWhereGroup where, int num, JoinType join, IEnumerable<IProperty> disabled, IList<ISorting> Sorting) where TForeign : DataEntity, new()
        {

            var es = new List<T>();
            this.DbService.Select<TForeign>(es, num, where, join, disabled, Sorting);
            return es;
        }
        public IEnumerable<T> GetList<TForeign>(JoinType join) where TForeign : DataEntity, new()
        {
            return this.GetList<TForeign>(null, join, null);
        }
        public IEnumerable<T> GetList(IWhereGroup where,int num=-1)
        {
            var es = new List<T>();
            this.DbService.Select(es, num, where, null, null, null);
            return es;
        }
        public T Get(dynamic pkValue)
        {
            var where = new WhereGrouping(WhereSpliceType.And);
            where.WhereItems.Add(new Where(this.DbService.Mapper.PrimaryKey.Name, pkValue, CompareType.等于));
            return this.DbService.SelectSingle(where, null, null);
        }
         
        public T Get<TForeign>(dynamic pkValue, JoinType join) where TForeign : DataEntity, new()
        {
            var where = new WhereGrouping(WhereSpliceType.And);
            where.WhereItems.Add(new Where(this.DbService.Mapper.PrimaryKey.Name, pkValue, CompareType.等于));
            return this.DbService.SelectSingle<TForeign>(where, join, null);
        }
        public virtual dynamic Insert(T entity)
        {
            return this.DbService.Insert(entity);
        }
        public virtual dynamic Update(T entity, IList<UpdateProperty> propertys, IWhereGroup where)
        {
            return this.DbService.Update(entity, propertys, where);
        }
        public dynamic Update(T entity, IWhereGroup where, IList<IProperty> disabled = null)
        {
            return this.DbService.Update(entity, where, disabled);
        }
        public int PagingByList(IList<T> entitys, IPaing paing, IWhereGroup wheres, IList<ISorting> sortin = null, IList<IProperty> disabled = null)
        {
           return   this.DbService.PagingByList(entitys, paing, wheres, sortin, disabled);
        }
        public int PagingByList<TForeign>(IList<T> entitys, IWhereGroup wheres, IPaing paing, JoinType Type, IList<ISorting> sortin = null, IList<IProperty> disabled = null)
         where TForeign : DataEntity, new()
        {
           return   this.DbService.PagingByList<TForeign>(entitys, wheres, paing, Type, sortin, disabled);
        }

         
    }
}
