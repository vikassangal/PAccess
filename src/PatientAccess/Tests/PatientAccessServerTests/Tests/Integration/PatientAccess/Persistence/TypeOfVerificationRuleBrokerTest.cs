using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for TypeOfVerificationRuleBrokerTest.
    /// </summary>

    //TODO: Create XML summary comment for TypeOfVerificationRuleBrokerTests
    [TestFixture()]
    public class TypeOfVerificationRuleBrokerTest : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown TypeOfVerificationRuleBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpTypeOfVerificationRuleBrokerTest()
        {
            i_TypeOfVerificationRuleBroker = BrokerFactory.BrokerOfType<ITypeOfVerificationRuleBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownTypeOfVerificationRuleBrokerTest()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestAllTypeOfVerificationRules()
        {
               
            IList allTypeOfVerificationRules = i_TypeOfVerificationRuleBroker.AllTypeOfVerificationRules();
            Assert.IsNotNull( allTypeOfVerificationRules, "No TypeOfVerificationRules returned");
            Assert.IsTrue( allTypeOfVerificationRules.Count == 4, "Incorrect number of TypeOfVerificationRules returned");

            foreach( Object o in allTypeOfVerificationRules)
            {
                Assert.AreEqual(typeof(TypeOfVerificationRule), o.GetType(), "Wrong Type");
            }
        }

            
        [Test()]
        public void TestTypeOfVerificationRuleWith() 
        {
            TypeOfVerificationRule rule1 = i_TypeOfVerificationRuleBroker.TypeOfVerificationRuleWith(1);
            // 1 is oid value for Type Of VerificationRule with desc='Birthday'
            Assert.IsNotNull( rule1, "No Type Of VerificationRule returned for oid=1");
            Assert.IsTrue( rule1.Description == "Birthday", "Wrong Type Of VerificationRule Description returned for oid=1");               
        }                 

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static ITypeOfVerificationRuleBroker i_TypeOfVerificationRuleBroker;
        #endregion
    }
}