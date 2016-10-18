using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.IOC
{
    public interface IInjection
    {
        Type ParameterType { get; }
        Func<T> Creater<T>();
    }
}
