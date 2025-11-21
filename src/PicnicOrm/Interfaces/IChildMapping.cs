using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks;

using PicnicOrm.Data;

namespace PicnicOrm.Mapping
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IChildMapping<T, TKey>
        where T : class
    {
        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="parents"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        void Map(IGridReader gridReader, IDictionary<TKey, T> parents, bool shouldContinueThroughEmptyTables);

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="parents"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        Task MapAsync(IGridReader gridReader, IDictionary<TKey, T> parents, bool shouldContinueThroughEmptyTables);

        #endregion
    }
}