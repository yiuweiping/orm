using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Cache.Enum;

namespace Zhengdi.Framework.Cache
{
    public interface ICacheBase
    {
        string Key { get; }
        dynamic Value { get;    }
        DateTime Expire { get; set; }
        CacheType Type { get; set; }

       
    }
     
}
