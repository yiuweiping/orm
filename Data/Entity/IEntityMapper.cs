using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Enum;
using Zhengdi.Framework.Reflection;

namespace Zhengdi.Framework.Data.Entity
{
    public interface IEntityMapper<T>
    {

        IProperty[] Propertys { get; }
        IEnumerable<IRelevance> Relevances { get; }
        string TableName { get; }
        IProperty PrimaryKey { get; }
        string ServiceKey { get; set; }
        IProperty GetPropertField(string fieldName);
        IRelevance GetRelevanceField(string fieldName);
        Action<T, dynamic,Type> FindSetDynamicHandle(string propertyName);

        Func<T, dynamic> FindGetDynamicHandle(string propertyName);
    }
}
