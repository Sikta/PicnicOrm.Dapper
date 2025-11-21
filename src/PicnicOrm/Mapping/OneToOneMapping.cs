using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for Task in MapAsync

using PicnicOrm.Data;

namespace PicnicOrm.Mapping
{
    /// <summary>
    /// </summary>
    public class OneToOneMapping<TParent, TChild, TParentKey, TChildKey> : BaseChildMapping<TParent, TChild, TParentKey, TChildKey>
        where TParent : class where TChild : class
    {
        #region Fields

        /// <summary>
        /// </summary>
        protected readonly Action<TParent, TChild> _childSetter;

        /// <summary>
        /// </summary>
        protected readonly Func<TParent, TChildKey> _parentChildKeySelector;

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="childKeySelector"></param>
        /// <param name="parentChildKeySelector"></param>
        /// <param name="childSetter"></param>
        public OneToOneMapping(Func<TChild, TChildKey> childKeySelector, Func<TParent, TChildKey> parentChildKeySelector, Action<TParent, TChild> childSetter)
            : base(childKeySelector)
        {
            _parentChildKeySelector = parentChildKeySelector;
            _childSetter = childSetter;
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
                //if we have children then put them in a dictionary
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
                //if we have children then put them in a dictionary
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
        /// 
        /// </summary>
        /// <param name="childDictionary"></param>
        /// <param name="parents"></param>
        private void MapParents(IDictionary<TChildKey, TChild> childDictionary, IDictionary<TParentKey, TParent> parents)
        {
            if (parents == null)
            {
                return;
            }

            //iterate through each parent and map parent/child relationship
            foreach (var parent in parents.Values)
            {
                var parentChildKey = _parentChildKeySelector(parent);
                if (childDictionary.ContainsKey(parentChildKey))
                {
                    _childSetter(parent, childDictionary[parentChildKey]);
                }
            }
        }

        #endregion
    }
}