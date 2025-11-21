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
    /// <typeparam name="TChild"></typeparam>
    /// <typeparam name="TParentKey"></typeparam>
    /// <typeparam name="TChildKey"></typeparam>
    public class OneToManyMapping<TParent, TChild, TParentKey, TChildKey> : BaseChildMapping<TParent, TChild, TParentKey, TChildKey>
        where TParent : class where TChild : class
    {
        #region Fields

        /// <summary>
        /// </summary>
        protected readonly Func<TChild, TParentKey> _childParentKeySelector;

        /// <summary>
        /// </summary>
        protected readonly Action<TParent, IEnumerable<TChild>> _parentSetter;

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="childKeySelector"></param>
        /// <param name="childParentKeySelector"></param>
        /// <param name="parentSetter"></param>
        public OneToManyMapping(Func<TChild, TChildKey> childKeySelector, Func<TChild, TParentKey> childParentKeySelector, Action<TParent, IEnumerable<TChild>> parentSetter)
            : base(childKeySelector)
        {
            _childParentKeySelector = childParentKeySelector;
            _parentSetter = parentSetter;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="parents"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        public override void Map(IGridReader gridReader, IDictionary<TParentKey, TParent> parents, bool shouldContinueThroughEmptyTables)
        {
            base.Map(gridReader, parents, shouldContinueThroughEmptyTables);
            IDictionary<TChildKey, TChild> childDictionary = null;

            var children = gridReader.Read<TChild>();

            if (children != null)
            {
                var childrenList = children.ToList();
                //if we have childrent then put them in a dictionary
                childDictionary = new Dictionary<TChildKey, TChild>(childrenList.Count);
                foreach (var child in childrenList)
                {
                    childDictionary.Add(_childKeySelector(child), child);
                }
                
                MapParents(childDictionary, parents);
            }

            //map children with their children
            MapChildren(gridReader, childDictionary, shouldContinueThroughEmptyTables);
        }

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <param name="parents"></param>
        /// <param name="shouldContinueThroughEmptyTables"></param>
        public override async Task MapAsync(IGridReader gridReader, IDictionary<TParentKey, TParent> parents, bool shouldContinueThroughEmptyTables)
        {
            await base.MapAsync(gridReader, parents, shouldContinueThroughEmptyTables);
            IDictionary<TChildKey, TChild> childDictionary = null;

            var children = await gridReader.ReadAsync<TChild>();

            if (children != null)
            {
                var childrenList = children.ToList();
                //if we have childrent then put them in a dictionary
                childDictionary = new Dictionary<TChildKey, TChild>(childrenList.Count);
                foreach (var child in childrenList)
                {
                    childDictionary.Add(_childKeySelector(child), child);
                }

                MapParents(childDictionary, parents);
            }

            //map children with their children
            await MapChildrenAsync(gridReader, childDictionary, shouldContinueThroughEmptyTables);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// </summary>
        /// <param name="childDictionary"></param>
        /// <param name="parents"></param>
        private void MapParents(IDictionary<TChildKey, TChild> childDictionary, IDictionary<TParentKey, TParent> parents)
        {
            if (parents != null)
            {
                //group the children by their parents
                // Optimization: Avoid GroupBy if possible, but for now just optimize dictionary creation
                var childGrouping = childDictionary.Values.GroupBy(_childParentKeySelector).ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());

                //iterate through each parent and map parent/child relationship
                foreach (var parentKey in parents.Keys)
                {
                    if (childGrouping.ContainsKey(parentKey))
                    {
                        _parentSetter(parents[parentKey], childGrouping[parentKey]);
                    }
                }
            }
        }

        #endregion
    }
}