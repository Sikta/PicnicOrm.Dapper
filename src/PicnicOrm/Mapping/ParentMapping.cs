using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using PicnicOrm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PicnicOrm.Data;

namespace PicnicOrm.Mapping
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class ParentMapping<TParent, TKey> : IParentMapping<TParent, TKey>
        where TParent : class
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly IList<IChildMapping<TParent, TKey>> _childMappings;

        /// <summary>
        /// </summary>
        private readonly Func<TParent, TKey> _keySelector;

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="keySelector"></param>
        public ParentMapping(Func<TParent, TKey> keySelector)
        {
            _keySelector = keySelector;
            _childMappings = new List<IChildMapping<TParent, TKey>>();
        }

        #endregion

        #region Interfaces

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        /// <returns></returns>
        public IEnumerable<TParent> Read(IGridReader gridReader, bool shouldContinueThroughEmptyTables = true)
        {
            IList<TParent> parentList = null;
            var parents = gridReader.Read<TParent>();

            if (parents != null)
            {
                parentList = parents.ToList();
                var parentDictionary = new Dictionary<TKey, TParent>(parentList.Count);
                foreach (var parent in parentList)
                {
                    parentDictionary.Add(_keySelector(parent), parent);
                }

                MapChildren(gridReader, parentDictionary, shouldContinueThroughEmptyTables);
            }

            return parentList ?? new List<TParent>();
        }

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TParent>> ReadAsync(IGridReader gridReader, bool shouldContinueThroughEmptyTables = true)
        {
            IList<TParent> parentList = null;
            var parents = await gridReader.ReadAsync<TParent>();

            if (parents != null)
            {
                parentList = parents.ToList();
                var parentDictionary = new Dictionary<TKey, TParent>(parentList.Count);
                foreach (var parent in parentList)
                {
                    parentDictionary.Add(_keySelector(parent), parent);
                }

                await MapChildrenAsync(gridReader, parentDictionary, shouldContinueThroughEmptyTables);
            }

            return parentList ?? new List<TParent>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="childMapping"></param>
        public void AddMapping(IChildMapping<TParent, TKey> childMapping)
        {
            _childMappings.Add(childMapping);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="parents"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        protected void MapChildren(IGridReader gridReader, IDictionary<TKey, TParent> parents, bool shouldContinueThroughEmptyTables)
        {
            foreach (var childMapping in _childMappings)
            {
                childMapping.Map(gridReader, parents, shouldContinueThroughEmptyTables);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="parents"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        protected async Task MapChildrenAsync(IGridReader gridReader, IDictionary<TKey, TParent> parents, bool shouldContinueThroughEmptyTables)
        {
            foreach (var childMapping in _childMappings)
            {
                await childMapping.MapAsync(gridReader, parents, shouldContinueThroughEmptyTables);
            }
        }

        #endregion
    }
}