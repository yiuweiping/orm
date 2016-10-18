using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Zhengdi.Framework.Data.DBHelper.Sql
{
    public class SqlDbConfigManager: ConfigurationManager, IConfig, IDbConntionConfig
    {
        readonly string _key;
        static SqlDbConfigManager obj;
        public string Key => this._key;
        private SqlDbConfigManager( string key ) {
            this._key = key;
            this.Load(); }
        public static SqlDbConfigManager GetDBconfigManager(string key = "DbConfiguration")
        {
            obj = obj ?? new SqlDbConfigManager(key);
            return obj;
        }

        public string GetConntonString(string key)
        {
            return GetConfig(key).ToString();
        }
        public override void Load()
        {
            this.Load($"{AppDomain.CurrentDomain.BaseDirectory}/bin/Resource/DbConfig.xml", XmlLoadMethod.Local);
        }

        protected override IEnumerable<IConfig> Load(XDocument doc)
        {
            var query = from t in doc.Descendants(this.Key)
                        select new SqlDbConfig(t.Element("Key").Value)
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
