using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Entity;

namespace Zhengdi.Framework.Data.DBHelper
{

    public interface IDbRepository<T> : IRenewal<T> where T : DataEntity, new()
    {
        IEnumerable<T> GetList();
        IEnumerable<T> GetList(IWhereGroup where,int num =-1);
  
        IEnumerable<T> GetList<TForeign>(IWhereGroup where, JoinType join, IEnumerable<IProperty> disabled, IList<ISorting> sortin = null) where TForeign : DataEntity, new();
        IEnumerable<T> GetList<TForeign>(IWhereGroup where,int num, JoinType join, IEnumerable<IProperty> disabled, IList<ISorting> sortin = null) where TForeign : DataEntity, new();
 
        IEnumerable<T> GetList<TForeign>(JoinType join) where TForeign : DataEntity, new();
        T Get(dynamic pkValue);
        T Get<TForeign>(dynamic pkValue, JoinType join) where TForeign : DataEntity, new();
        int PagingByList(IList<T> entitys, IPaing paing, IWhereGroup wheres, IList<ISorting> sortin = null, IList<IProperty> disabled = null);
        int PagingByList<TForeign>(IList<T> entitys, IWhereGroup wheres, IPaing paing, JoinType Type, IList<ISorting> sortin = null, IList<IProperty> disabled = null)
          where TForeign : DataEntity, new();
    }
    
}
