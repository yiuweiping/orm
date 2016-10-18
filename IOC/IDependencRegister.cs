using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.IOC
{
    public interface IDependencRegister
    {
        string TypeName { get; }
        IList<object> Paremas { get; }
        T Creater<T>();

    }
}
