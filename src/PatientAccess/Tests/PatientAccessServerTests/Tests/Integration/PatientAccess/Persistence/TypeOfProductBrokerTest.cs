using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for TypeOfProductBrokerTest.
    /// </summary>

    //TODO: Create XML summary comment for TypeOfProductBrokerTests
    [TestFixture()]
    public class TypeOfProductBrokerTest : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown TypeOfProductBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpTypeOfProductBrokerTest()
        {
            i_TypeOfProductBroker =  BrokerFactory.BrokerOfType<ITypeOfProductBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownTypeOfProductBrokerTest()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestAllTypeOfProducts()
        {
               
            IList allTypeOfProducts = i_TypeOfProductBroker.AllTypeOfProducts();
            Assert.IsNotNull( allTypeOfProducts, "No TypeOfProducts returned");
            Assert.IsTrue( allTypeOfProducts.Count == 8, "Incorrect number of TypeOfProducts returned");

            foreach( Object o in allTypeOfProducts)
            {
                Assert.AreEqual(typeof(TypeOfProduct), o.GetType(), "Wrong Type");
            }
        }

            
        [Test()]
        public void TestTypeOfProductWith() 
        {
            TypeOfProduct product1 = i_TypeOfProductBroker.TypeOfProductWith(2);
            // 1 is oid value for Type Of Product with desc='PPO'
            Assert.IsNotNull( product1, "No Type Of Product returned for oid=2");
            Assert.IsTrue( product1.Description == "PPO", "Wrong Type Of Product Description returned for oid=2");               
        }
                      
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static ITypeOfProductBroker i_TypeOfProductBroker;
        #endregion
    }
}