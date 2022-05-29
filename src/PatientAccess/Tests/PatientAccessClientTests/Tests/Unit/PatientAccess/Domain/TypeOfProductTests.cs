using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class TypeOfProductTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown TypeOfProductTests
        [TestFixtureSetUp()]
        public static void SetUpTypeOfProductTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownTypeOfProductTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testTypeOfProduct()
        {
           
            TypeOfProduct type1 = new TypeOfProduct(3,ReferenceValue.NEW_VERSION,"PPO");
           
            Assert.AreEqual(
                typeof(TypeOfProduct),
                type1.GetType()
                );
                 
            Assert.AreEqual(
                "PPO",
                type1.Description
                );
            
            Assert.AreEqual(
                3,
                type1.Oid
                );
            TypeOfProduct type2 = new TypeOfProduct(3,ReferenceValue.NEW_VERSION,"POS");
            TypeOfProduct type3 = new TypeOfProduct(3,ReferenceValue.NEW_VERSION,"HMO");
            ArrayList types = new ArrayList();
            types.Add(type1);
            types.Add(type2);
            types.Add(type3);
            Assert.AreEqual(3,
                            types.Count                         
                );
            Assert.IsTrue (types.Contains(type2) );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}