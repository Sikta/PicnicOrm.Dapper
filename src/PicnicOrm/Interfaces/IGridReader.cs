using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PicnicOrm.Data
{
    /// <summary>
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PicnicOrm.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGridReader : IDisposable
    {
        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> Read<T>() where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<IEnumerable<T>> ReadAsync<T>() where T : class;

        #endregion
    }
}