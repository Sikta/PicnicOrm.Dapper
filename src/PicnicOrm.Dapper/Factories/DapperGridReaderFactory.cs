using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using PicnicOrm.Dapper.Data;
using PicnicOrm.Data;
using PicnicOrm.Factories;

namespace PicnicOrm.Dapper.Factories
{
    /// <summary>
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using PicnicOrm.Dapper.Data;
using PicnicOrm.Data;
using PicnicOrm.Factories;

namespace PicnicOrm.Dapper.Factories
{
    /// <summary>
    /// 
    /// </summary>
    public class DapperGridReaderFactory : IGridReaderFactory
    {
        #region Interfaces

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public IGridReader Create(IDbConnection dbConnection, string storedProcName, object parameters = null, IDbTransaction dbTransaction = null, int? commandTimeout = null,
                                  CommandType? commandType = null)
        {
            return new DapperGridReader(dbConnection.QueryMultiple(storedProcName, parameters, dbTransaction, commandTimeout, commandType));
        }

        #endregion
    }
}