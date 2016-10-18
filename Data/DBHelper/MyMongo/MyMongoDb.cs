using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Zhengdi.Framework.Data.DBHelper.MyMongo
{
    public class MyMongoDb : IDisposable
    {
        private  MongoClient _mongo;
        private IMongoDatabase _db;
        public MyMongoDb(string key="test")
        {
           //var str =   MyMongoConfigManager.GetDBconfigManager().GetConntonString(key);
           // if (string.IsNullOrEmpty(str))
           //     throw new ArgumentNullException("connectionString");

           // _mongo = new MongoClient(str);

           // // 立即连接 MongoDB
           // _mongo.Connect();

           // if (string.IsNullOrEmpty(dbName) == false)
           //     _db = _mongo.GetDatabase(dbName);
        }
       

        /// <summary>
        /// 切换到指定的数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public IMongoDatabase UseDb(string dbName)
        {
            if (string.IsNullOrEmpty(dbName))
                throw new ArgumentNullException("dbName");

            _db = _mongo.GetDatabase(dbName);
            return _db;
        }

        /// <summary>
        /// 获取当前连接的数据库
        /// </summary>
        public IMongoDatabase CurrentDb
        {
            get
            {
                if (_db == null)
                    throw new Exception("当前连接没有指定任何数据库。请在构造函数中指定数据库名或者调用UseDb()方法切换数据库。");

                return _db;
            }
        }

        /// <summary>
        /// 获取当前连接数据库的指定集合【依据类型】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>() where T : class
        {
            //return this.CurrentDb.GetCollection<T>();
            return null;
        }

        /// <summary>
        /// 获取当前连接数据库的指定集合【根据指定名称】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">集合名称</param>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>(string name) where T : class
        {
            return this.CurrentDb.GetCollection<T>(name);
        }

        public void Dispose()
        {
            //if (_mongo != null)
            //{
            //    _mongo.Dispose();
            //    _mongo = null;
            //}
        }
    }
}
