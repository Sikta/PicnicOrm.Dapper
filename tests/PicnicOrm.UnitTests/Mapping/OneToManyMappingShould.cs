using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for async tests

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using PicnicOrm.Data;
using PicnicOrm.Mapping;
using PicnicOrm.TestModels;

namespace PicnicOrm.UnitTests.Mapping
{
    [TestClass]
    public class OneToManyMappingShould
    {
        #region Properties

        public Mock<IChildMapping<OneToManyItem, int>> MockChildMapping { get; set; }

        public Mock<IGridReader> MockGridReader { get; set; }

        public OneToManyMapping<ParentItem, OneToManyItem, int, int> OneToManyMapping { get; set; }

        #endregion

        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            MockChildMapping = new Mock<IChildMapping<OneToManyItem, int>>();
            MockGridReader = new Mock<IGridReader>();

            OneToManyMapping = new OneToManyMapping<ParentItem, OneToManyItem, int, int>(child => child.Id, child => child.ParentId, (parent, children) => parent.Children = children.ToList());
        }

        [TestMethod]
        public void Map_HasNoResultsAndShouldContinueThroughEmptyTablesIsFalse_DoesNotMapChildren()
        {
            //Arrange
            var oneToManyItemList = new List<OneToManyItem>();
            MockGridReader.Setup(gridReader => gridReader.Read<OneToManyItem>()).Returns(oneToManyItemList);
            OneToManyMapping.AddMapping(MockChildMapping.Object);
            IDictionary<int, ParentItem> parents = null;

            //Act
            OneToManyMapping.Map(MockGridReader.Object, parents, false);

            //Assert
            MockChildMapping.Verify(childMapping => childMapping.Map(It.IsAny<IGridReader>(), It.IsAny<IDictionary<int, OneToManyItem>>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void Map_HasNoResultsAndShouldContinueThroughEmptyTablesIsTrue_MapsChildren()
        {
            //Arrange
            var oneToManyItemList = new List<OneToManyItem>();
            MockGridReader.Setup(gridReader => gridReader.Read<OneToManyItem>()).Returns(oneToManyItemList);
            OneToManyMapping.AddMapping(MockChildMapping.Object);
            IDictionary<int, ParentItem> parents = null;

            //Act
            OneToManyMapping.Map(MockGridReader.Object, parents, true);

            //Assert
            MockChildMapping.Verify(childMapping => childMapping.Map(It.IsAny<IGridReader>(), It.IsAny<IDictionary<int, OneToManyItem>>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void Map_HasResultsAndParents_MapsParents()
        {
            //Arrange
            var oneToManyItem = new OneToManyItem { Id = 5, ParentId = 1 };
            var oneToManyItem2 = new OneToManyItem { Id = 18, ParentId = 5 };
            var oneToManyItem3 = new OneToManyItem { Id = 23, ParentId = 1 };
            var oneToManyItemList = new List<OneToManyItem> { oneToManyItem, oneToManyItem2, oneToManyItem3 };
            MockGridReader.Setup(gridReader => gridReader.Read<OneToManyItem>()).Returns(oneToManyItemList);
            var parent = new ParentItem { ChildId = 5 };
            var parentDictionary = new Dictionary<int, ParentItem> { [1] = parent };

            //Act
            OneToManyMapping.Map(MockGridReader.Object, parentDictionary, true);

            //Assert
            Assert.IsNull(parentDictionary[1].Child);
            Assert.AreEqual(2, parentDictionary[1].Children.Count);
            Assert.AreEqual(oneToManyItem, parentDictionary[1].Children[0]);
            Assert.AreEqual(oneToManyItem3, parentDictionary[1].Children[1]);
        }

        [TestMethod]
        public void Map_HasResultsAndShouldContinueThroughEmptyTablesIsFalse_MapsChildren()
        {
            //Arrange
            var oneToManyItem = new OneToManyItem();
            var oneToManyItemList = new List<OneToManyItem> { oneToManyItem };
            MockGridReader.Setup(gridReader => gridReader.Read<OneToManyItem>()).Returns(oneToManyItemList);
            OneToManyMapping.AddMapping(MockChildMapping.Object);
            IDictionary<int, ParentItem> parents = null;

            //Act
            OneToManyMapping.Map(MockGridReader.Object, parents, false);

            //Assert
            MockChildMapping.Verify(childMapping => childMapping.Map(It.IsAny<IGridReader>(), It.IsAny<IDictionary<int, OneToManyItem>>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task MapAsync_HasNoResultsAndShouldContinueThroughEmptyTablesIsFalse_DoesNotMapChildren()
        {
            //Arrange
            var oneToManyItemList = new List<OneToManyItem>();
            MockGridReader.Setup(gridReader => gridReader.ReadAsync<OneToManyItem>()).ReturnsAsync(oneToManyItemList);
            OneToManyMapping.AddMapping(MockChildMapping.Object);
            IDictionary<int, ParentItem> parents = null;

            //Act
            await OneToManyMapping.MapAsync(MockGridReader.Object, parents, false);

            //Assert
            MockChildMapping.Verify(childMapping => childMapping.MapAsync(It.IsAny<IGridReader>(), It.IsAny<IDictionary<int, OneToManyItem>>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public async Task MapAsync_HasNoResultsAndShouldContinueThroughEmptyTablesIsTrue_MapsChildren()
        {
            //Arrange
            var oneToManyItemList = new List<OneToManyItem>();
            MockGridReader.Setup(gridReader => gridReader.ReadAsync<OneToManyItem>()).ReturnsAsync(oneToManyItemList);
            OneToManyMapping.AddMapping(MockChildMapping.Object);
            IDictionary<int, ParentItem> parents = null;

            //Act
            await OneToManyMapping.MapAsync(MockGridReader.Object, parents, true);

            //Assert
            MockChildMapping.Verify(childMapping => childMapping.MapAsync(It.IsAny<IGridReader>(), It.IsAny<IDictionary<int, OneToManyItem>>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public async Task MapAsync_HasResultsAndParents_MapsParents()
        {
            //Arrange
            var oneToManyItem = new OneToManyItem { Id = 5, ParentId = 1 };
            var oneToManyItem2 = new OneToManyItem { Id = 18, ParentId = 5 };
            var oneToManyItem3 = new OneToManyItem { Id = 23, ParentId = 1 };
            var oneToManyItemList = new List<OneToManyItem> { oneToManyItem, oneToManyItem2, oneToManyItem3 };
            MockGridReader.Setup(gridReader => gridReader.ReadAsync<OneToManyItem>()).ReturnsAsync(oneToManyItemList);
            var parent = new ParentItem { ChildId = 5 };
            var parentDictionary = new Dictionary<int, ParentItem> { [1] = parent };

            //Act
            await OneToManyMapping.MapAsync(MockGridReader.Object, parentDictionary, true);

            //Assert
            Assert.IsNull(parentDictionary[1].Child);
            Assert.AreEqual(2, parentDictionary[1].Children.Count);
            Assert.AreEqual(oneToManyItem, parentDictionary[1].Children[0]);
            Assert.AreEqual(oneToManyItem3, parentDictionary[1].Children[1]);
        }

        [TestMethod]
        public async Task MapAsync_HasResultsAndShouldContinueThroughEmptyTablesIsFalse_MapsChildren()
        {
            //Arrange
            var oneToManyItem = new OneToManyItem();
            var oneToManyItemList = new List<OneToManyItem> { oneToManyItem };
            MockGridReader.Setup(gridReader => gridReader.ReadAsync<OneToManyItem>()).ReturnsAsync(oneToManyItemList);
            OneToManyMapping.AddMapping(MockChildMapping.Object);
            IDictionary<int, ParentItem> parents = null;

            //Act
            await OneToManyMapping.MapAsync(MockGridReader.Object, parents, false);

            //Assert
            MockChildMapping.Verify(childMapping => childMapping.MapAsync(It.IsAny<IGridReader>(), It.IsAny<IDictionary<int, OneToManyItem>>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion
    }
}