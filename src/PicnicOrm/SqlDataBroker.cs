using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

using PicnicOrm.Data;
using PicnicOrm.Factories;
using PicnicOrm.Mapping;

namespace PicnicOrm
{
    /// <summary>
    /// </summary>
    public class SqlDataBroker : IDataBroker
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// </summary>
        private readonly IGridReaderFactory _gridReaderFactory;

        /// <summary>
        /// </summary>
        private readonly IDictionary<int, IParentMapping> _parentMappings;

        /// <summary>
        /// </summary>
        private readonly IDbConnectionFactory _sqlConnectionFactory;

        /// <summary>
        /// </summary>
        private readonly IDictionary<Type, IParentMapping> _typeMappings;

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="gridReaderFactory"></param>
        public SqlDataBroker(string connectionString, IGridReaderFactory gridReaderFactory)
            : this(connectionString, new SqlConnectionFactory(), gridReaderFactory)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlConnectionFactory"></param>
        /// <param name="gridReaderFactory"></param>
        public SqlDataBroker(string connectionString, IDbConnectionFactory sqlConnectionFactory, IGridReaderFactory gridReaderFactory)
        {
            _connectionString = connectionString;
            _sqlConnectionFactory = sqlConnectionFactory;
            _gridReaderFactory = gridReaderFactory;
            _parentMappings = new Dictionary<int, IParentMapping>();
            _typeMappings = new Dictionary<Type, IParentMapping>();
        }

        #endregion

        #region Interfaces

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mapping"></param>
        public void AddMapping(int key, IParentMapping mapping)
        {
            if (_parentMappings.ContainsKey(key))
            {
                throw new ArgumentException($"{nameof(key)}: ({key}) has already been added.");
            }

            _parentMappings.Add(key, mapping);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapping"></param>
        public void AddMapping<T>(IParentMapping<T> mapping) where T : class
        {
            var key = typeof(T);
            if (_typeMappings.ContainsKey(key))
            {
                throw new ArgumentException($"{nameof(key)}: ({key.Name}) has already been added.");
            }

            _typeMappings.Add(key, mapping);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteStoredProcedure<T>(string storedProcName, object parameters = null) where T : class
        {
            var key = typeof(T);
            if (!_typeMappings.ContainsKey(key))
            {
                throw new ArgumentException($"Type: ({key.Name}) is not registered.");
            }

            return ExecuteMapping<T>(storedProcName, _typeMappings[key], parameters);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcName"></param>
        /// <param name="mappingKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteStoredProcedure<T>(string storedProcName, int mappingKey, object parameters = null) where T : class
        {
            if (!_parentMappings.ContainsKey(mappingKey))
            {
                throw new ArgumentException($"Parameter: ({nameof(mappingKey)}: {mappingKey}) is not a valid key.");
            }

            return ExecuteMapping<T>(storedProcName, _parentMappings[mappingKey], parameters);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string storedProcName, object parameters = null) where T : class
        {
            var key = typeof(T);
            if (!_typeMappings.ContainsKey(key))
            {
                throw new ArgumentException($"Type: ({key.Name}) is not registered.");
            }

            return ExecuteMappingAsync<T>(storedProcName, _typeMappings[key], parameters);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcName"></param>
        /// <param name="mappingKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string storedProcName, int mappingKey, object parameters = null) where T : class
        {
            if (!_parentMappings.ContainsKey(mappingKey))
            {
                throw new ArgumentException($"Parameter: ({nameof(mappingKey)}: {mappingKey}) is not a valid key.");
            }

            return ExecuteMappingAsync<T>(storedProcName, _parentMappings[mappingKey], parameters);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcName"></param>
        /// <param name="mapping"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private IEnumerable<T> ExecuteMapping<T>(string storedProcName, IParentMapping mapping, object parameters) where T : class
        {
            IEnumerable<T> results = null;

            using (var connection = _sqlConnectionFactory.Create(_connectionString))
            {
                using (var reader = _gridReaderFactory.Create(connection, storedProcName, parameters))
                {
                    var castedMapping = (IParentMapping<T>)mapping;
                    results = castedMapping.Read(reader);
                }
            }

            return results;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcName"></param>
        /// <param name="mapping"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task<IEnumerable<T>> ExecuteMappingAsync<T>(string storedProcName, IParentMapping mapping, object parameters) where T : class
        {
            IEnumerable<T> results = null;

            using (var connection = _sqlConnectionFactory.Create(_connectionString))
            {
                using (var reader = _gridReaderFactory.Create(connection, storedProcName, parameters))
                {
                    var castedMapping = (IParentMapping<T>)mapping;
                    results = await castedMapping.ReadAsync(reader);
                }
            }

            return results;
        }

        #endregion
    }
}