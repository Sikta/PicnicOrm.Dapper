using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using PicnicOrm.Data;

namespace PicnicOrm.Mapping
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PicnicOrm.Data;

namespace PicnicOrm.Mapping
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TParent">ex: User</typeparam>
    /// <typeparam name="TChild">ex: Car</typeparam>
    /// <typeparam name="TLink">ex: UserCar</typeparam>
    /// <typeparam name="TParentKey"></typeparam>
    /// <typeparam name="TChildKey"></typeparam>
    public class ManyToManyMapping<TParent, TChild, TLink, TParentKey, TChildKey> : BaseChildMapping<TParent, TChild, TParentKey, TChildKey>
        where TParent : class where TChild : class where TLink : class
    {
        #region Fields

        /// <summary>
        /// </summary>
        protected readonly Func<TLink, TChildKey> _childLinkKeySelector;

        /// <summary>
        /// </summary>
        protected readonly Func<TLink, TParentKey> _parentLinkKeySelector;

        /// <summary>
        /// </summary>
        protected readonly Action<TParent, IEnumerable<TChild>> _parentSetter;

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="childKeySelector"></param>
        /// <param name="childLinkKeySelector"></param>
        /// <param name="parentKeyLinkSelector"></param>
        /// <param name="parentSetter"></param>
        public ManyToManyMapping(Func<TChild, TChildKey> childKeySelector, Func<TLink, TChildKey> childLinkKeySelector, Func<TLink, TParentKey> parentKeyLinkSelector, Action<TParent, IEnumerable<TChild>> parentSetter)
            : base(childKeySelector)
        {
            _childLinkKeySelector = childLinkKeySelector;
            _parentLinkKeySelector = parentKeyLinkSelector;
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

            //Organize the link entities by parent key
            var groupedLinks = GetGroupedLinks(gridReader);

            if (shouldContinueThroughEmptyTables || (groupedLinks != null && groupedLinks.Any()))
            {
                var children = gridReader.Read<TChild>();

                //map children and parents using the grouped links and return the children in a dictionary if there were any
                childDictionary = Map(children, groupedLinks, parents);
            }

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

            //Organize the link entities by parent key
            var groupedLinks = await GetGroupedLinksAsync(gridReader);

            if (shouldContinueThroughEmptyTables || (groupedLinks != null && groupedLinks.Any()))
            {
                var children = await gridReader.ReadAsync<TChild>();

                //map children and parents using the grouped links and return the children in a dictionary if there were any
                childDictionary = Map(children, groupedLinks, parents);
            }

            await MapChildrenAsync(gridReader, childDictionary, shouldContinueThroughEmptyTables);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <returns></returns>
        private IDictionary<TParentKey, List<TChildKey>> GetGroupedLinks(IGridReader gridReader)
        {
            IDictionary<TParentKey, List<TChildKey>> groupedLinks = null;

            var links = gridReader.Read<TLink>();

            if (links != null)
            {
                groupedLinks = links.GroupBy(_parentLinkKeySelector, _childLinkKeySelector).ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());
            }

            return groupedLinks != null && groupedLinks.Any() ? groupedLinks : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="gridReader"></param>
        /// <returns></returns>
        private async Task<IDictionary<TParentKey, List<TChildKey>>> GetGroupedLinksAsync(IGridReader gridReader)
        {
            IDictionary<TParentKey, List<TChildKey>> groupedLinks = null;

            var links = await gridReader.ReadAsync<TLink>();

            if (links != null)
            {
                groupedLinks = links.GroupBy(_parentLinkKeySelector, _childLinkKeySelector).ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());
            }

            return groupedLinks != null && groupedLinks.Any() ? groupedLinks : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="childDictionary"></param>
        /// <param name="parents"></param>
        /// <param name="groupedLinks"></param>
        private void MapParents(IDictionary<TChildKey, TChild> childDictionary, IDictionary<TParentKey, TParent> parents, IDictionary<TParentKey, List<TChildKey>> groupedLinks)
        {
            if (parents == null)
            {
                return;
            }

            //Organize the children entities by parent key (children can belong to more than one parent)
            // Note: ToGrouping is an extension method, need to check if it supports generic keys or if I need to update it.
            // Assuming ToGrouping needs update or replacement.
            // Let's implement the logic inline or assume ToGrouping works if it's generic enough.
            // Actually, ToGrouping is likely in Extensions.cs. I should check it.
            
            // For now, let's implement the logic here to be safe and optimize.
            var manyToManyGroupedChildren = new Dictionary<TParentKey, List<TChild>>();
            foreach (var link in groupedLinks)
            {
                var parentKey = link.Key;
                var childKeys = link.Value;
                var childrenList = new List<TChild>(childKeys.Count);
                foreach(var childKey in childKeys)
                {
                    if(childDictionary.TryGetValue(childKey, out var child))
                    {
                        childrenList.Add(child);
                    }
                }
                manyToManyGroupedChildren.Add(parentKey, childrenList);
            }

            //Map the children collections to their parents
            foreach (var parent in parents)
            {
                if (manyToManyGroupedChildren.ContainsKey(parent.Key))
                {
                    _parentSetter(parent.Value, manyToManyGroupedChildren[parent.Key]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="children"></param>
        /// <param name="groupedLinks"></param>
        /// <param name="parents"></param>
        /// <returns></returns>
        private IDictionary<TChildKey, TChild> Map(IEnumerable<TChild> children, IDictionary<TParentKey, List<TChildKey>> groupedLinks, IDictionary<TParentKey, TParent> parents)
        {
            IDictionary<TChildKey, TChild> childDictionary = null;

            if (children != null && groupedLinks != null)
            {
                var childrenList = children.ToList();
                childDictionary = new Dictionary<TChildKey, TChild>(childrenList.Count);
                foreach(var child in childrenList)
                {
                    childDictionary.Add(_childKeySelector(child), child);
                }

                MapParents(childDictionary, parents, groupedLinks);
            }

            return childDictionary != null && childDictionary.Any() ? childDictionary : null;
        }

        #endregion
    }
}