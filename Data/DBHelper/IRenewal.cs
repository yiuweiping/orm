using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Entity;

namespace Zhengdi.Framework.Data.DBHelper
{
    public interface IRenewal<T> where T : DataEntity, new()
    {
        dynamic Insert(T entity);
        dynamic Update(T entity, IWhereGroup where, IList<IProperty> disabled = null);
        dynamic Update(T entity, IList<UpdateProperty> propertys, IWhereGroup where);
        dynamic Delete(IWhereGroup where);
     }
}
