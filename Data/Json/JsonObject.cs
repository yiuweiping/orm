using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zhengdi.Framework.Data.Json
{
    public class JsonObject
    {
        public string Key { get; set; }
        private dynamic _value = string.Empty;

        public dynamic Value
        {
            get { return JsonObject.GetValue(this._value.ToString()); }
            set { this._value = value; }
        }
        public JsonObject() { }
        public JsonObject(string key, dynamic value)
        {
            this.Key = key;
            this.Value = value;
        }
        public JsonObject(string key, string[] array)
        {
            this.Key = key;
            this._value = array;
        }
        public override string ToString()
        {
            this._value = this._value ?? string.Empty;
            if (this._value is string[])
            {
                var str = new StringBuilder();
                foreach (string a in this._value)
                    str.Append(Regex.IsMatch(a, @"^\{.+\}$", RegexOptions.Compiled) ? $"{a}," : $"\"{a}\",");
                this._value = str.Remove(str.Length - 1, 1).ToString();
                this._value = string.Format("\"{0}\":[{1}]", this.Key, EditorCharacter(this._value));
            }
            else if (this._value is JsonObject)
                this._value = String.Format("\"{0}\":{{{1}}}", this.Key, this._value);
            else
                this._value = DbDataType.IsString(this._value) ? string.Format("\"{0}\":\"{1}\"", this.Key, EditorCharacter(this._value)) : string.Format("\"{0}\":{1}", this.Key, EditorCharacter(this._value.ToLower()));
            return this._value;
        }
        static string EditorCharacter(string value)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '<':
                        str.Append("&lt;");
                        break;
                    case '>':
                        str.Append("&gt;");
                        break;
                    case ',':
                        str.Append("&gh;");
                        break;
                    case ':':
                        str.Append("&fh;");
                        break;
                    default:
                        str.Append(value[i]);
                        break;
                }
            }
            return str.ToString();
        }
        static dynamic GetValue(string value)
        {
            value = value.Replace("&gh;", ",");
            if (Regex.IsMatch(value, @"^\{.+\}$", RegexOptions.Compiled))
                return JsonHelper.Parse(value);
            if (Regex.IsMatch(value, @"^\[.+\]$", RegexOptions.Compiled))
            {
                if (Regex.IsMatch(value, @"\{.+\}", RegexOptions.Compiled))
                    return JsonHelper.ParseArray(value);
                else
                    return JsonHelper.ParseItem(value);
            }
            return value;
        }
    }
}
