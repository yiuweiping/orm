using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Cache
{
    public interface ICacheManager<T>
    {
        T this[string key] { get; }
        void Add(string key, T value);
        void Remove(string key);
        bool ContainsKey(string key);
    }
}
