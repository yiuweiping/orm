using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Zhengdi.Framework.Data;
using Zhengdi.Framework.Data.DBHelper;
using Zhengdi.Framework.Reflection;
using System.Collections.Specialized;
using Zhengdi.Framework.Cache;
using Zhengdi.Framework.Cache.Enum;
using Zhengdi.Framework.Data.Entity;
using Zhengdi.Framework.Data.Enum;

namespace Zhengdi.Framework.IOC
{
 
    public abstract  class IOC
    {
        private  IDependencRegister _registeritem;
        private dynamic _dynamicHandler;
        readonly HybridDictionary RegisterCache;
        readonly Type[] _types;
        readonly Assembly _assembly;
        protected IOC(string key)
        {
            this.RegisterCache = new HybridDictionary();
            var di = DependencyConfigManager.GetDependencyManager();
            var itme = di.GetDependencyItme(key);
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}bin\\{itme.FullName}";
            this._assembly = Assembly.LoadFrom(path);
            this._types = this.GetTypes(this._assembly);
            this.initialization();
        }
        protected abstract  Type[] GetTypes(Assembly assembly);
        protected abstract  void initialization();
        public IOC Register<T>()
        {
            var name = typeof(T).Name;
            if (!this.RegisterCache.Contains(name))
            {
                _registeritem = this.CreaterIDependencRegister();
                this.RegisterCache.Add(name, _registeritem);
            }
            return this;
        }
        protected abstract IDependencRegister CreaterIDependencRegister();
        protected abstract IInjection CreaterIInjectionItem(Type type);

        public T Creater<T>() {
            this._dynamicHandler = new DynamicHandlerCompiler<T>();
            return (this.RegisterCache[typeof(T).Name] as IDependencRegister).Creater<T>();
        }
        public IOC DI<T>()
        {
    
            var pt = (from t in this._types where t.GetInterface(typeof(T).FullName) != null select t).SingleOrDefault();
            var a = CreaterIInjectionItem(pt);
            this._registeritem.Paremas.Add(a.Creater<T>());
            return this;
        }
       
    }
 

}
