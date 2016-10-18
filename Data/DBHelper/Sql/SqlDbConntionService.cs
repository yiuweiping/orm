using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Zhengdi.Framework.Data.DBHelper.Sql
{
    public class SqlDbConntionService : IDbConntion
    {
        private  SqlConnection _con;
        public IDbCommand GetIDbCommand(string key)
        {
            var s = SqlDbConfigManager.GetDBconfigManager().GetConntonString(key);
            this._con = this._con ?? new SqlConnection(s);
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
