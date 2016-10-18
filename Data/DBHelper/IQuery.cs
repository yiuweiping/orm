using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Entity;

namespace Zhengdi.Framework.Data.DBHelper
{
    public interface IQuery<T> where T : DataEntity, new()
    {
        void Select<K>(IList<T> entitys, int num, IWhereGroup where, JoinType Type, IEnumerable<IProperty> disabled, IList<ISorting> sortin = null) where K : DataEntity, new();
        void Select<K1, K2>(IList<T> entitys, int num, IWhereGroup where, JoinType Type, IEnumerable<IProperty> disabled) where K1 : DataEntity, new() where K2 : DataEntity, new();
        void Select(T entity, IWhereGroup wheres, IEnumerable<Polymerize> polymerizes, IList<IProperty> disabled = null, IList<ISorting> sortin = null);
        void Select(IList<T> entitys, IWhereGroup wheres, IList<IProperty> disabled = null, IList<ISorting> sortin = null);
        void Select(IList<T> entitys, int num, IWhereGroup wheres, IEnumerable<Polymerize> polymerizes, IList<IProperty> disable, IList<ISorting> sortin);
        int PagingByList(IList<T> entitys, IPaing paing, IWhereGroup wheres, IList<ISorting> sortin = null, IList<IProperty> disabled = null);
        int PagingByList<K>(IList<T> entitys, IWhereGroup where, IPaing paing, JoinType Type, IList<ISorting> sortin = null, IList<IProperty> disabled = null) where K : DataEntity, new();
      
        T SelectSingle( IWhereGroup wheres, IEnumerable<Polymerize> polymerizes, IList<IProperty> disable );
        T SelectSingle<K>(IWhereGroup wheres, JoinType join, IEnumerable<IProperty> disable) where K : DataEntity, new();
    }
}
