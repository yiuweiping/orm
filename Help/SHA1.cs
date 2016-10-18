using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Help
{
    public static  class SHA1
    {
        public static string GetSHA1(string str)
        {
            System.Security.Cryptography.SHA1 s1 = System.Security.Cryptography.SHA1.Create();
            byte[] sha1bytes = s1.ComputeHash(Encoding.UTF8.GetBytes(str));
            return BitConverter.ToString(sha1bytes).Replace("-", "");
        }
    }
}
