using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

        private List<ParentItem> GetParentListWithSingleItem()
        {
            var parentItem = new ParentItem();
            var parentList = new List<ParentItem> { parentItem };

            return parentList;
        }

        #endregion
    }
}