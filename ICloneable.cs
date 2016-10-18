using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework
{
    public interface ICloneable
    {
        T DeepClone<T>() where T : class, new();
        T ShallowClone<T>() where T: class, new();

    }
}
