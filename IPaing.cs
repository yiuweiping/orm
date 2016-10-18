using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework
{
    public interface IPaing
    {
        int Showline { get; set; }
        int PageNumber { get; set; }
        int PageNum { get; }
        int Count { get; }
        void SetCount(int num);
 
    }
}
