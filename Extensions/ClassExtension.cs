using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Extensions
{
    public static class ClassExtension
    {
        /// <summary>
        /// 实体转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">目标实体</param>
        /// <param name="source">源实体</param>
        /// <returns></returns>
        public static T InEntityFrom<T, K>(this T target, K source) where T : class, new() where K : class, new()
        {
            Type t = typeof(T);
            PropertyInfo[] pros = t.GetProperties();
            PropertyInfo[] spros = typeof(K).GetProperties();
            HybridDictionary dic = new HybridDictionary();
            foreach (var p in spros)
                dic.Add(p.Name, p.GetValue(source, null));
            target = target ?? new T();
            foreach (PropertyInfo p in pros)
            {
                var o = dic[p.Name];
                if (o != null)
                {
                    p.SetValue(target, o, null);
                }
            }
            return target;
        }

        
 
    }
}
