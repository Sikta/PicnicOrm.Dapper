using System.Collections.Generic;

namespace PicnicOrm
{
using System.Collections.Generic;

namespace PicnicOrm
{
    /// <summary>
    /// </summary>
    public static class Extensions
    {
        #region Public Methods

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TParentKey"></typeparam>
        /// <typeparam name="TChildKey"></typeparam>
        /// <param name="items"></param>
        /// <param name="linkDictionary"></param>
        /// <returns></returns>
        public static IDictionary<TParentKey, IList<T>> ToGrouping<T, TParentKey, TChildKey>(this IDictionary<TChildKey, T> items, IDictionary<TParentKey, List<TChildKey>> linkDictionary)
        {
            IDictionary<TParentKey, IList<T>> dictionary = new Dictionary<TParentKey, IList<T>>();

            foreach (var parentLinkKey in linkDictionary.Keys)
            {
                var list = new List<T>();
                foreach (var childLinkKey in linkDictionary[parentLinkKey])
                {
                    if (items.ContainsKey(childLinkKey))
                    {
                        list.Add(items[childLinkKey]);
                    }
                }
                dictionary.Add(parentLinkKey, list);
            }

            return dictionary;
        }

        #endregion
    }
}