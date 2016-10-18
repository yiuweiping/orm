using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Data;

namespace Zhengdi.Framework.Data.DBHelper.MySql
{
     public  class MySqlDbConntionService:IDbConntion
    {
        private  MySqlConnection _con;
        public IDbCommand GetIDbCommand(string key)
        {
            var s = MySqlDbConfigManager.GetDBconfigManager().GetConntonString(key);
            this._con = this._con ?? new MySqlConnection(s);
            this._con.Open();
            return this._con.CreateCommand();
        }
        public void Close()
        {
            if (this._con != null && this._con.State == ConnectionState.Open)
                this._con.Close();
        }
    }
}
