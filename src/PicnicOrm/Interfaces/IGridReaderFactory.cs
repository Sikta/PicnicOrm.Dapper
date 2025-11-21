using System.Collections.Generic;
using System.Data;

using PicnicOrm.Data;

namespace PicnicOrm.Factories
{
    /// <summary>
using System.Collections.Generic;
using System.Data;

using PicnicOrm.Data;

namespace PicnicOrm.Factories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGridReaderFactory
    {
        #region Public Methods

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
        IGridReader Create(IDbConnection dbConnection, string storedProcName, object parameters = null, IDbTransaction dbTransaction = null, int? commandTimeout = null,
                                  CommandType? commandType = null);

        #endregion
    }
}