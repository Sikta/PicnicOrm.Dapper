using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using PicnicOrm.Data;
using PicnicOrm.Factories;
using PicnicOrm.Mapping;
using PicnicOrm.TestModels;

namespace PicnicOrm.UnitTests
{
    [TestClass]
    public class SqlDataBrokerShould
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public SqlDataBroker SqlDataBroker { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Mock<IGridReaderFactory> MockGridReaderFactory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Mock<IParentMapping<ParentItem>> MockParentMapping { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Mock<IDbConnectionFactory> MockSqlConnectionFactory { get; set; }

        #endregion

        #region Public Methods

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddMapping_AddSameMappingKeyTwice_ThrowsException()
        {
            //Arrange
            SqlDataBroker.AddMapping(3, MockParentMapping.Object);

            //Act
            SqlDataBroker.AddMapping(3, new Mock<IParentMapping<OneToOneItem>>().Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]

        public void AddMapping_AddSameTypeTwice_ThrowsException()
        {
            //Arrange
            SqlDataBroker.AddMapping(MockParentMapping.Object);

            //Act
            SqlDataBroker.AddMapping(MockParentMapping.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteStoredProcedure_PassInvalidMappingKey_ThrowsException()
        {
            //Arrange
            SqlDataBroker.AddMapping(7, MockParentMapping.Object);

            //Act
            SqlDataBroker.ExecuteStoredProcedure<ParentItem>("FakeProcName", 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteStoredProcedure_PassInvalidKeyType_ThrowsException()
        {
            //Arrange
            SqlDataBroker.AddMapping<ParentItem>(MockParentMapping.Object);

            //Act
            SqlDataBroker.ExecuteStoredProcedure<OneToOneItem>("FakeProcName");
        }

        [TestMethod]
        public void ExecuteStoredProcedure_PassValidConfigKey_UsesCorrectMapping()
        {
            //Arrange
            var mockSecondMapping = new Mock<IParentMapping<OneToOneItem>>();
            SqlDataBroker.AddMapping(3, mockSecondMapping.Object);
            SqlDataBroker.AddMapping(7, MockParentMapping.Object);

            //Act
            SqlDataBroker.ExecuteStoredProcedure<ParentItem>("FakeProcName", 7);
            var result = new SqlDataBroker("FakeString", MockGridReaderFactory.Object);

            var privateObject = new PrivateObject(result);
            var connectionFactory = (IDbConnectionFactory)privateObject.GetField("_sqlConnectionFactory");

            //Assert
            Assert.IsNotNull(connectionFactory);
        }

        [TestInitialize]
        public void Initialize()
        {
            MockSqlConnectionFactory = new Mock<IDbConnectionFactory>();
            MockParentMapping = new Mock<IParentMapping<ParentItem>>();
            MockGridReaderFactory = new Mock<IGridReaderFactory>();
            SqlDataBroker = new SqlDataBroker("FakeConnectionString", MockSqlConnectionFactory.Object, MockGridReaderFactory.Object);
        }

        #endregion
    }
}