using System.Collections.Generic;
using System.Threading.Tasks;

using Dapper;

using PicnicOrm.Data;

namespace PicnicOrm.Dapper.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DapperGridReader : IGridReader
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly SqlMapper.GridReader _gridReader;

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        public DapperGridReader(SqlMapper.GridReader gridReader)
        {
            _gridReader = gridReader;
        }

        #endregion

        #region Interfaces

        /// <summary>
using System.Collections.Generic;
using System.Threading.Tasks;

using Dapper;

using PicnicOrm.Data;

namespace PicnicOrm.Dapper.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DapperGridReader : IGridReader
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly SqlMapper.GridReader _gridReader;

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        public DapperGridReader(SqlMapper.GridReader gridReader)
        {
            _gridReader = gridReader;
        }

        #endregion

        #region Interfaces

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            _gridReader.Dispose();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Read<T>() where T : class
        {
            return _gridReader.Read<T>();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ReadAsync<T>() where T : class
        {
            return await _gridReader.ReadAsync<T>();
        }

        #endregion
    }
}