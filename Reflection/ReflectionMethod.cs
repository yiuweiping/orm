using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache;
using Zhengdi.Framework.Data.Entity;

namespace Zhengdi.Framework
{
    public static   class ReflectionMethod
    {
        public static T GetClassAttribute<T>(dynamic obj) where T : Attribute, new()
        {
            Type type = obj.GetType();
            object[] records = type.GetCustomAttributes(typeof(T), true);
            return records.Length > 0 ? (records[0] as T) : default(T);
        }
        public static object CreateObject(System.Reflection.Assembly assembly, Type type)
        {
            return   assembly.CreateInstance(type.FullName);
        }

        


    }
}
