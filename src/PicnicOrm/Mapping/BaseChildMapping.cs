using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using PicnicOrm.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for Task

using PicnicOrm.Data;

namespace PicnicOrm.Mapping
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    /// <typeparam name="TParentKey"></typeparam>
    /// <typeparam name="TChildKey"></typeparam>
    public abstract class BaseChildMapping<TParent, TChild, TParentKey, TChildKey> : IChildMapping<TParent, TParentKey>
        where TParent : class where TChild : class
    {
        #region Fields

        /// <summary>
        /// </summary>
        protected readonly Func<TChild, TChildKey> _childKeySelector;

        /// <summary>
        /// </summary>
        protected readonly IList<IChildMapping<TChild, TChildKey>> _childMappings;

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="childKeySelector"></param>
        public BaseChildMapping(Func<TChild, TChildKey> childKeySelector)
        {
            _childKeySelector = childKeySelector;

            _childMappings = new List<IChildMapping<TChild, TChildKey>>();
        }

        #endregion

        #region Interfaces

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="parents"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        public virtual void Map(IGridReader gridReader, IDictionary<TParentKey, TParent> parents, bool shouldContinueThroughEmptyTables)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="parents"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        public virtual Task MapAsync(IGridReader gridReader, IDictionary<TParentKey, TParent> parents, bool shouldContinueThroughEmptyTables)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="childMapping"></param>
        public void AddMapping(IChildMapping<TChild, TChildKey> childMapping)
        {
            _childMappings.Add(childMapping);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="childDictionary"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        protected void MapChildren(IGridReader gridReader, IDictionary<TChildKey, TChild> childDictionary, bool shouldContinueThroughEmptyTables)
        {
            if (shouldContinueThroughEmptyTables || (childDictionary != null && childDictionary.Any()))
            {
                foreach (var childMapping in _childMappings)
                {
                    childMapping.Map(gridReader, childDictionary, shouldContinueThroughEmptyTables);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="childDictionary"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        protected async Task MapChildrenAsync(IGridReader gridReader, IDictionary<TChildKey, TChild> childDictionary, bool shouldContinueThroughEmptyTables)
        {
            if (shouldContinueThroughEmptyTables || (childDictionary != null && childDictionary.Any()))
            {
                foreach (var childMapping in _childMappings)
                {
                    await childMapping.MapAsync(gridReader, childDictionary, shouldContinueThroughEmptyTables);
                }
            }
        }

        #endregion
    }
}