using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Data.Entity
{
    public interface IEntity
    {
        IField GetFilter(string fieldName);
        IEnumerable<IField> GetFieldItems();
        
    }
}
