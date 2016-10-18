using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data;

namespace Zhengdi.Framework.IOC
{
    public class DependencyItme : IConfig
    {
        public string FullName { get; set; }
        public string Key { get; set; }
    }
}
