using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using Zhengdi.Framework.Data;

namespace Zhengdi.Framework.Data.DBHelper
{
 
    public interface IDbConntionConfig
    {
          string GetConntonString(string key);
    }
    public interface IDbConfig  
    {
        string DbName { get; set; }
        string Address { get; set; }
        string Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        
       
    }
     
}
