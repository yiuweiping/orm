using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Data.DBHelper.MySql
{
     public class MySqlDbConfig:IConfig
    {
        public string Address { get; set; }
        public string DbName { get; set; }

        private string _key;
        public string Key => this._key;
        public string Password { get; set; }
        public string Port { get; set; }

        public string UserName { get; set; }
        public MySqlDbConfig(string key)
        {
            this._key = key;
        }
        public override string ToString()
        {
            return $"Database={DbName};Data Source={Address};User Id={UserName};Password={Password};pooling=false;CharSet=utf8;port={Port}";
        }
    }
}
