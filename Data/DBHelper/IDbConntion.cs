using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Data.DBHelper
{
    public interface IDbConntion
    {
        System.Data.IDbCommand GetIDbCommand(string key);
        void Close();
    }
     
}
