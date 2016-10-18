using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Zhengdi.Framework.Data
{
    public static class DbDataType
    {
        public static DbType DbTypeTo(object obj)
        {
            Type t = obj.GetType();
            if (typeof(string) == t)
                return DbType.String;
            if (typeof(int) == t)
                return DbType.Int32;
            if (typeof(Int16) == t)
                return DbType.Int32;
            if (typeof(Int64) == t)
                return DbType.Int64;
            if (typeof(uint) == t)
                return DbType.UInt32;
            if (typeof(char) == t)
                return DbType.StringFixedLength;
            if (typeof(DateTime) == t)
                return DbType.DateTime;
            if (typeof(double) == t)
                return DbType.Decimal;
            if (typeof(decimal) == t)
                return DbType.Decimal;
            if (typeof(byte) == t)
                return DbType.Byte;
            if (typeof(bool) == t)
                return DbType.Boolean;
            if (typeof(byte) == t)
                return DbType.Binary;
            return DbType.Object;
        }
        public static bool IsString(object obj)
        {
            
            Type t = obj.GetType();
            if (typeof(string) == t)
                return true;
            if (typeof(int) == t)
                return false;
            if (typeof(Int16) == t)
                return false;
            if (typeof(Int64) == t)
                return false;
            if (typeof(uint) == t)
                return false;
            if (typeof(char) == t)
                return true;
            if (typeof(DateTime) == t)
                return true; ;
            if (typeof(double) == t)
                return false; ;
            if (typeof(decimal) == t)
                return false; ;
            if (typeof(byte) == t)
                return false; ;
            if (typeof(bool) == t)
                return false;
            return true;
        }
        public static object DefaultValue(Type type)
        {
            Type t = type;
            if (typeof(string) == t)
                return string.Empty;
            if (typeof(int) == t)
                return 0;
            if (typeof(Int16) == t)
                return 0;
            if (typeof(Int64) == t)
                return 0;
            if (typeof(uint) == t)
                return 0;
            if (typeof(char) == t)
                return char.MinValue;
            if (typeof(DateTime) == t)
                return DateTime.MinValue;
            if (typeof(double) == t)
                return 0.0;
            if (typeof(decimal) == t)
                return 0.0;
            if (typeof(byte) == t)
                return 0;
            if (typeof(bool) == t)
                return false;
            if (typeof(System.Enum) == t.BaseType)
                return 0;
       
            return new object();
        }
        public static string ToSqlType(Type type)
        {
            Type t = type;
            if (typeof(string) == t)
                return "varchar";
            if (typeof(int) == t)
                return "int";
            if (typeof(Int16) == t)
                return "int";
            if (typeof(Int64) == t)
                return "int";
            if (typeof(uint) == t)
                return "int";
            if (typeof(char) == t)
                return "char";
            if (typeof(DateTime) == t)
                return "datetime";
            if (typeof(double) == t)
                return "decimal";
            if (typeof(decimal) == t)
                return "decimal";
            if (typeof(byte) == t)
                return "bit";
            if (typeof(bool) == t)
                return "bit";
            if (typeof(System.Enum) == t.BaseType)
                return "int";
      
            return "nvarchar";
        }
        public static string GetSize(Type type)
        {
            Type t = type;
            if (typeof(string) == t)
                return "300";
            if (typeof(int) == t)
                return "-1";
            if (typeof(Int16) == t)
                return "-1";
            if (typeof(Int64) == t)
                return "-1";
            if (typeof(uint) == t)
                return "-1";
            if (typeof(char) == t)
                return "-1";
            if (typeof(DateTime) == t)
                return "-1";
            if (typeof(double) == t)
                return "18,2";
            if (typeof(decimal) == t)
                return "18,2";
            if (typeof(byte) == t)
                return "-1";
            if (typeof(bool) == t)
                return "-1";
            if (typeof(System.Enum) == t.BaseType)
                return "-1";
            return "2000";
        }

    }
}
