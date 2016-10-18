using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Collections;

namespace Zhengdi.Framework.Data
{
    public interface IConfig
    {
        string Key { get; }
    }
    /// <summary>
    /// 配置文件管理
    /// </summary>
    public abstract class ConfigurationManager
    {
        static  HybridDictionary dic;
        static ConfigurationManager()
        {
            dic = new HybridDictionary();
        }
        public abstract void Load();

        protected   IConfig  GetConfig(string key)
        {
            return dic.Contains(key) ? dic[key] as IConfig : null;
        }
        //获取配置文件信息
        public void Load(string addres, XmlLoadMethod option)
        {
            XDocument x = null;
            switch (option)
            {
                case XmlLoadMethod.Local:
                    x = Local(addres);
                    break;
                case XmlLoadMethod.Web:
                    x = Web(addres);
                    break;
            }
            foreach (IConfig s in Load(x))
                dic.Add(s.Key, s);
        }
        //获取路径
        static XDocument Local(string addres)
        {
            //判断是否是虚拟路径,如果不是则转换为绝对路径
            string str = Path.IsPathRooted(addres) ? addres : System.IO.Path.GetFullPath(addres);
            if (!File.Exists(@addres))
                throw new Exception("文件不存在！");
            return XDocument.Load(str);
        }
        static XDocument Web(string url)
        {
            XDocument s;
            s = ConfigurationManager.Stream(null);
            return s;
        }
        static XDocument Stream(Stream stream)
        {
            return XDocument.Load(stream);
        }
        protected abstract IEnumerable<IConfig> Load(XDocument doc);

    }
}
