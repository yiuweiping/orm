using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Extensions
{
    public static class IEnumerableExtensions
    {
        public static HybridDictionary ToHybridDictionary<TSource, TKey>(this IEnumerable<TSource> target, Func<TSource, TKey> selectedKey)
        {
            var dic = new HybridDictionary(target.Count());
            foreach (var s in target)
            {
                dic.Add(selectedKey(s), s);
            }
            return dic;
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> target, Func<  TSource,bool> method)
        {
            foreach (var a in target)
            {
                if (method(a))
                {
                    return true;
                }
            }
            return false;
        }

    }
    

}
