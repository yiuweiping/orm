using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Error
{
    public class DbExecuteException : Exception
    {
        public DbExecuteException(string message) : base(message) { }
        
    }
}
