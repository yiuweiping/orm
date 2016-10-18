using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Zhengdi.Framework.Data.DBHelper.MyMongo
{
    public class MyMongoConfigManager : ConfigurationManager, IConfig, IDbConntionConfig
    {
        readonly string _key;
        static MyMongoConfigManager obj;
        public string Key => this._key;
        private MyMongoConfigManager(string key)
        {
            this._key = key;
            this.Load();
        }
        public static MyMongoConfigManager GetDBconfigManager(string key = "mongoConfig")
        {
            obj = obj ?? new MyMongoConfigManager(key);
            return obj;
        }

        public string GetConntonString(string key)
        {
            return GetConfig(key).ToString();
        }
        public override void Load()
        {
            this.Load($"{AppDomain.CurrentDomain.BaseDirectory}/Resource/DbConfig.xml", XmlLoadMethod.Local);
        }
        protected override IEnumerable<IConfig> Load(XDocument doc)
        {
            var query = from t in doc.Descendants(this.Key)
                        select new MongoDbConfig(t.Element("Key").Value)
                        {
                            Address = t.Element("Address").Value,
                            Password = t.Element("Password").Value,
                            DbName = t.Element("DbName").Value,
                            Port = t.Element("Port").Value,
                            UserName = t.Element("Account").Value
                        };
            return query;
        }
    }
}
