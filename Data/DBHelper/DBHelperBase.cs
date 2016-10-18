using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;
using Zhengdi.Framework.Cache;
using Zhengdi.Framework.Data.Entity;
using Zhengdi.Framework.Reflection;

namespace Zhengdi.Framework.Data.DBHelper
{
   
    public abstract class DBHelperBase<T>: IQuery<T>, IRenewal<T> where T :  DataEntity, new()
    {
        readonly IDbConntion _conn;
        readonly IEntityMapper<T>  _mapper;
        readonly IDBbatBuilder _bat;
        readonly T _entity;
        public IEntityMapper<T> Mapper => this._mapper;
        protected IDBbatBuilder BatBuilder => this._bat;
        public DBHelperBase()
        {
            this._conn = this.GetIDbConntion();
            this._mapper = EntityMapperCacheManager.GetMapperCacheManager()[typeof(T).Name].Value;
            this._bat = this.GetDBbatBuilder();
            this._entity = new T();
        }
        protected abstract IDBbatBuilder GetDBbatBuilder();
        protected abstract IDbConntion GetIDbConntion();
        protected void ExecuteReader(IList<T> entity)
        {
            try
            {
                IDbCommand com = InitiCommand();
                using (var read = com.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var rs = ReaderDataEntity.CreaterReaderDataEntity<T>(this._entity, read, this._mapper);
                    rs.FullDataEntity<T>(entity, read);
                }
            }
            finally
            {
                this._conn.Close();
            }

        }
        protected void ExecuteReader<K>(IList<T> entity) where K:DataEntity,new ()
        {
            try
            {
                IDbCommand com = InitiCommand();
                using (var read = com.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var rs = ReaderDataEntity.CreaterReaderDataEntity<T, K>(this._entity, read, this._mapper);
                    rs.FullDataEntity<T>(entity, read);
                }
            }
            finally
            {
                this._conn.Close();
            }

        }
        protected void ExecuteReader<K1, K2>(IList<T> entity) where K2 : DataEntity, new()
             where K1 : DataEntity, new()
        {
            try
            {
                IDbCommand com = InitiCommand();
                using (var read = com.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var rs = ReaderDataEntity.CreaterReaderDataEntity<T, K1, K2>(this._entity, read, this._mapper);
                    rs.FullDataEntity<T>(entity, read);
                }
            }
            finally
            {
                this._conn.Close();
            }

        }
        protected void ExecuteNonQuery(object entity, out int rowNum)
        {
            try
            {
                IDbCommand com = InitiCommand(entity);
                rowNum = com.ExecuteNonQuery();
            }
            finally
            {
                this._conn.Close();
            }

        }
        public int ExecuteNonQuery(string commandText, object paramObj = null)
        {
            try
            {
                var com = this._conn.GetIDbCommand(this._mapper.ServiceKey);
                com.CommandText = commandText;
                DBbatBuilder<T>.SetCommandParameter(commandText, com.Parameters, this.CreaterParamger, paramObj);
                return com.ExecuteNonQuery();
            }
            finally
            {
                this._conn.Close();
            }
        }
        protected abstract IDataParameter CreaterParamger(string name, object value);
        protected void ExecuteScalar(object entity, out object outParam)
        {
            try
            {
                IDbCommand com = InitiCommand(entity);
                outParam = com.ExecuteScalar();
            }
            finally
            {
                this._conn.Close();
            }

        }
        private IDbCommand InitiCommand(object entity = null)
        {
            var com = this._conn.GetIDbCommand(this._mapper.ServiceKey);
            com.CommandText = this._bat.ToString();
            com.CommandType = this._bat.CommandType;
           
            _bat.SetCommandParameter(com.CommandText, com.Parameters, entity);
            return com;
        }
        private IDbCommand InitiCommand(IDBbatBuilder commandText, object paramObj)
        {
            var com = this._conn.GetIDbCommand(this._mapper.ServiceKey);
            com.CommandText = commandText.ToString();
            com.CommandType = CommandType.Text;
            commandText.SetCommandParameter(com.CommandText, com.Parameters, paramObj);
            return com;
        }
        public void Execute(IList<T> entitys, IDBbatBuilder commandText, object paramObj = null)
        {
            try
            {
                IDbCommand com = InitiCommand(commandText, paramObj);
                using (var read = com.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var rs = ReaderDataEntity.CreaterReaderDataEntity<T>(this._entity, read, this._mapper);
                    rs.FullDataEntity<T>(entitys, read);
                }
            }
            finally
            {
                this._conn.Close();
            }
        }
        public void Execute(IList<T> entitys, string commandText, object paramObj = null)
        {
            try
            {
                var com = this._conn.GetIDbCommand(this._mapper.ServiceKey);
                com.CommandText = commandText;
                DBbatBuilder<T>.SetCommandParameter(commandText, com.Parameters, this.CreaterParamger, paramObj);
                using (var read = com.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var rs = ReaderDataEntity.CreaterReaderDataEntity<T>(this._entity, read, this._mapper);
                    rs.FullDataEntity<T>(entitys, read);
                }
            }
            finally
            {
                this._conn.Close();
            }
        }
        public void Execute<K>(IList<T> entitys, IDBbatBuilder commandText, object paramObj = null) where K : DataEntity, new()
        {
            try
            {
                IDbCommand com = InitiCommand(commandText, paramObj);
                using (var read = com.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var rs = ReaderDataEntity.CreaterReaderDataEntity<T, K>(this._entity, read, this._mapper);
                    rs.FullDataEntity<T>(entitys, read);
                }
            }
            finally
            {
                this._conn.Close();
            }
        }
        public void Execute<K>(IList<T> entitys, string commandText, object paramObj = null) where K : DataEntity, new()
        {
            try
            {
                var com = this._conn.GetIDbCommand(this._mapper.ServiceKey);
                com.CommandText = commandText;
                DBbatBuilder<T>.SetCommandParameter(commandText, com.Parameters, this.CreaterParamger, paramObj);
                using (var read = com.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var rs = ReaderDataEntity.CreaterReaderDataEntity<T, K>(this._entity, read, this._mapper);
                    rs.FullDataEntity<T>(entitys, read);
                }
            }
            finally
            {
                this._conn.Close();
            }
        }

        public abstract void Select<K>(IList<T> entitys, int num, IWhereGroup where, JoinType Type, IEnumerable<IProperty> disabled) where K : DataEntity, new();
        public abstract void Select<K>(IList<T> entitys, int num, IWhereGroup where, JoinType Type, IEnumerable<IProperty> disabled, IList<ISorting> sortin = null) where K : DataEntity, new();

        public abstract void Select(T entity, IWhereGroup wheres, IEnumerable<Polymerize> polymerizes, IList<IProperty> disabled = null, IList<ISorting> sortin = null);
        public abstract void Select(IList<T> entitys, IWhereGroup wheres, IList<IProperty> disabled = null, IList<ISorting> sortin = null);
        public abstract void Select(IList<T> entitys, int num, IWhereGroup wheres, IEnumerable<Polymerize> polymerizes, IList<IProperty> disable, IList<ISorting> sortin);
        public abstract int PagingByList(IList<T> entitys, IPaing paing, IWhereGroup wheres, IList<ISorting> sortin = null, IList<IProperty> disabled = null);
        public abstract int PagingByList<K>(IList<T> entitys, IWhereGroup where, IPaing paing, JoinType Type, IList<ISorting> sortin = null, IList<IProperty> disabled = null) where K : DataEntity, new();
        public abstract T SelectSingle(IWhereGroup wheres, IEnumerable<Polymerize> polymerizes, IList<IProperty> disable);
        public abstract T SelectSingle<K>(IWhereGroup wheres, JoinType join, IEnumerable<IProperty> disable) where K : DataEntity, new();
        public abstract dynamic Insert(T entity);
        public abstract dynamic Update(T entity, IWhereGroup where, IList<IProperty> disabled = null);
        public abstract dynamic Update(T entity, IList<UpdateProperty> propertys, IWhereGroup where);
        public abstract dynamic Delete(IWhereGroup where);
        public abstract void Select<K1, K2>(IList<T> entitys, int num, IWhereGroup where, JoinType Type, IEnumerable<IProperty> disabled)
            where K1 : DataEntity, new()
            where K2 : DataEntity, new();
    }
}
