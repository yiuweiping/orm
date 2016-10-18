using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Data.DBHelper.Sql
{
    public class SqlDbConfig : IConfig, IDbConfig
    {
        public string Address { get; set; }
        public string DbName { get; set; }

        private string _key;
        public string Key => this._key;
        public string Password { get; set; }
        public string Port { get; set; }

        public string UserName { get; set; }
        public SqlDbConfig(string key)
        {
            this._key = key;
        }
        public override string ToString()
        {
            return $"Data Source={Address}{  (Port == null ? string.Empty : $"{ (string.IsNullOrWhiteSpace(Port)?string.Empty:",") }{Port}") }; Initial Catalog={DbName}; User ID={UserName}; Password={Password}";
        }
        
    }
}
