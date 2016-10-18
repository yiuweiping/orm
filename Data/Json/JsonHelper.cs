using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Zhengdi.Framework.Cache;
using Zhengdi.Framework.Data.Entity;

namespace Zhengdi.Framework.Data.Json
{
    public static  class JsonHelper
    {
        //冒号
        const string SEMICOLON = "@scolon";
        //分号
        const string ESCAPED = "@escaped";
        public static IEnumerable<JsonObject> Parse(string str)
        {
            return from t in JsonHelper.ParseItem(str) select ParseJsonObject(t);
        }
        public static IEnumerable<IEnumerable<JsonObject>> ParseArray(string str)
        {
            return from t in JsonHelper.ParseItem(str) select JsonHelper.Parse(t);
        }
        static JsonObject ParseJsonObject(string source)
        {
            JsonObject obj = new JsonObject();
            int index = source.IndexOf(SEMICOLON);
            if (index > 0)
            {
                obj.Key = source.Substring(0, index);
                index = index + SEMICOLON.Length;
                obj.Value = ReplaceStr(source.Substring(index, source.Length - index));
            }
            return obj;
        }
        public static T Parse<T>(string str) where T : IEntity, new()
        {
            return Regex.IsMatch(str, @"^\{[^\{|\}|\[|\]]+\}$", RegexOptions.Compiled) ? Parse<T>(JsonHelper.Parse(str)) : new T();
        }
        public static T Parse<T>(IEnumerable<JsonObject> items) where T : IEntity, new()
        {
            var e = new T();
  
            var c = EntityMapperCacheManager.GetMapperCacheManager()[e.GetType().Name].Value;
            foreach (var a in e.GetFieldItems())
            {
                foreach (var b in items)
                {
                    if (string.Compare(a.Name, b.Key) == 0)
                    {
                        var s = Convert.ChangeType(b.Value, a.Type);
                        c.FindSetDynamicHandle(a.Name)(e, s, a.Type.IsEnum ? typeof(int) : a.Type);
                    }
                }
            }
            return e;
        }
 
        internal static string[] ParseItem(string str)
        {
            return JsonHelper.EditorCharacter(str).Split(new string[] { ESCAPED }, StringSplitOptions.None);
        }
        static object ReplaceStr(string str)
        {
            string value = str.Replace(SEMICOLON, ":");
            value = value.Replace("\\", ":");
            value = value.Replace("&gh;", ",");
            value = value.Replace("&fh;", ":");
            return value;
        }

        public static string Stringify( IEnumerable<JsonObject> items )
        {
            StringBuilder json = new StringBuilder();
            json.Append("{");
            foreach (JsonObject obj in items)
            {
                json.AppendFormat("{0},", obj.ToString());
            }
            json.Replace(',', '}', json.Length - 1, 1);
            return json.ToString();
        }

        public static string Stringify(IEntity entity)
        {
 
            var s = from t in entity.GetFieldItems()  select new JsonObject(t.Name, t.Value);
            return JsonHelper.Stringify(s);
        }
        public static string Stringify<T>(IEnumerable<T> models, string[] Removes) where T: IEntity,new ()
        {
            StringBuilder array = new StringBuilder();
            array.Append("[");
            foreach (IEntity model in models)
            {
                array.AppendFormat("{0},", Stringify(model));
            }
            array.Replace(',', ']', array.Length - 1, 1);
            return array.ToString();
        }
     
        static string EditorCharacter(string value)
        {
            if (value.EndsWith("\""))
                value = value.Substring(1, value.Length - 2);
            if (value.Length > 3)
                value = value.Substring(1, value.Length - 2);
            StringBuilder str = new StringBuilder();
            bool IsStr = false;
            bool isObj = false;
            bool isArray = false;
            int count = 0;
            foreach(char a in value)
            {
                switch (a)
                {
                    case ':':
                        if (!IsStr &&!isObj && !isArray&& count==0)
                            str.Append(SEMICOLON);
                        else
                            str.Append(a);
                        break;
                    case ',':
                        if (!IsStr && !isObj && !isArray&&count==0)
                            str.Append(ESCAPED);
                        else
                            str.Append(a);
                        break;
                    case '{':
                        isObj = true;
                        count++;
                        str.Append(a);
                        break;
                    case '}':
                        isObj = false;
                        count--;
                        str.Append(a);
                        break;
                    case '[':
                        isArray = true;
                        count++;
                        str.Append(a);
                        break;
                    case ']':
                        isArray = false;
                        count--;
                        str.Append(a);
                        break;
                    case '\"':
                        IsStr = IsStr ? false : true;
                        break;
                    case '\n':
                        break;
                    case '\r':
                        break;
                    case '\\':
                        break;
                    default:
                        str.Append(a);
                        break;
                }
            }
             
            return str.ToString();
        }
        public static JsonObject Find(IEnumerable<JsonObject> array, string key)
        {
            var s = from t in array where t.Key == key select t;
            return s.Count() > 0 ? s.SingleOrDefault() : null;
        }
        
    }
}
