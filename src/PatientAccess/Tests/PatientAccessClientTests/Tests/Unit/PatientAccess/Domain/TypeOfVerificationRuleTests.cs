using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class TypeOfVerificationRuleTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown TypeOfVerificationRuleTests
        [TestFixtureSetUp()]
        public static void SetUpTypeOfVerificationRuleTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownTypeOfVerificationRuleTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testTypeOfVerificationRule()
        {
           
            TypeOfVerificationRule type1 = new TypeOfVerificationRule(3,ReferenceValue.NEW_VERSION,"Birthday");
           
            Assert.AreEqual(
                typeof(TypeOfVerificationRule),
                type1.GetType()
                );
                 
            Assert.AreEqual(
                "Birthday",
                type1.Description
                );
            
            Assert.AreEqual(
                3,
                type1.Oid
                );
            TypeOfVerificationRule type2 = new TypeOfVerificationRule(3,ReferenceValue.NEW_VERSION,"Gender");
            TypeOfVerificationRule type3 = new TypeOfVerificationRule(3,ReferenceValue.NEW_VERSION,"Unknown");
            ArrayList types = new ArrayList();
            types.Add(type1);
            types.Add(type2);
            types.Add(type3);
            Assert.AreEqual(3,
                            types.Count                         
                );
            Assert.IsTrue( types.Contains(type2) );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}