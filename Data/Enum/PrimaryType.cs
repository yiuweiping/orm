using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Data.Enum
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PrimaryType : uint
    {
        Empty = 0,
        Increment = 1,
        Guid = 2,
        Customer = 3,
    }
}
