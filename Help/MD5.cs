using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Help
{
     public static  class MD5
    {
        public static string Getbit32(string str)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            char[] temp = str.ToCharArray();
            byte[] buf = new byte[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                buf[i] = (byte)temp[i];
            }
            byte[] data = md5Hasher.ComputeHash(buf);

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        public static string Getbit16(string str)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            char[] temp = str.ToCharArray();
            byte[] buf = new byte[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                buf[i] = (byte)temp[i];
            }
            byte[] data = md5Hasher.ComputeHash(buf);

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        /** 获取大写的MD5签名结果 */
        public static string GetMD5Util(string encypStr, string charset)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception)
            {
                inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }

        /// <summary>
        /// 签名字符串
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果</returns>
        public static string AlipayMD5(string prestr, string key, string _input_charset)
        {
            StringBuilder sb = new StringBuilder(32);

            prestr = prestr + key;

            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="sign">签名结果</param>
        /// <param name="key">密钥</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>验证结果</returns>
        public static bool AlipayMD5Verify(string prestr, string sign, string key, string _input_charset)
        {
            string mysign = MD5.AlipayMD5(prestr, key, _input_charset);
            if (mysign == sign)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
