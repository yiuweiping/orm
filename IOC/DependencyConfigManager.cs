using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Zhengdi.Framework.Data;

namespace Zhengdi.Framework.IOC
{
    public class DependencyConfigManager : ConfigurationManager, IConfig
    {
        readonly string _key;
        static DependencyConfigManager obj;
        public string Key => this._key;
        private DependencyConfigManager(string key)
        {
            this._key = key;
            this.Load();
        }
        public static DependencyConfigManager GetDependencyManager(string key = "DependencyItem")
        {
            obj = obj ?? new DependencyConfigManager(key);
            return obj;
        }
        public override void Load()
        {
            var str = $"{AppDomain.CurrentDomain.BaseDirectory}bin\\Resource\\Dependency.xml";
            this.Load(str, XmlLoadMethod.Local);
        }
        protected override IEnumerable<IConfig> Load(XDocument doc)
        {
            var query = from t in doc.Descendants(this.Key)
                        select new DependencyItme()
                        {
                            Key = t.Element("key").Value,
                            FullName = t.Element("fullName").Value
                        };
            return query;
        }

        public DependencyItme GetDependencyItme(string diName)
        {
            return this.GetConfig(diName) as DependencyItme;
        }
    }
}
