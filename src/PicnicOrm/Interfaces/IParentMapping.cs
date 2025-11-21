using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks;

using PicnicOrm.Data;

namespace PicnicOrm.Mapping
{
    /// <summary>
    /// </summary>
    public interface IParentMapping
    {
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParentMapping<T> : IParentMapping
        where T : class
    {
        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        /// <returns></returns>
        IEnumerable<T> Read(IGridReader gridReader, bool shouldContinueThroughEmptyTables = true);

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> ReadAsync(IGridReader gridReader, bool shouldContinueThroughEmptyTables = true);

        #endregion
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IParentMapping<T, TKey> : IParentMapping<T>
        where T : class
    {
    }
}