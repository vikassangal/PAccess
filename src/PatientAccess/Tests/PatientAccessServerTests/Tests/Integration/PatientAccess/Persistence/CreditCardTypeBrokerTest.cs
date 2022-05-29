using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class CreditCardTypeBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown GenderBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpCreditCardTypeBrokerTests()
        {
            ccBroker = BrokerFactory.BrokerOfType<ICreditCardTypeBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownCreditCardTypeBrokerTests()
        {
            ccBroker = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestGetAllCreditCard()
        {
            IList list = ccBroker.AllCreditCardTypes();
            
            Assert.IsNotNull(list,"Failed to retreive Credit card type list");

            CreditCardProvider ccFound = null;
            foreach( CreditCardProvider cc in list )
            {
                if( cc.Code == "1" )
                {
                    ccFound = cc;
                }
            }
            Assert.IsNotNull(ccFound,"Did not find Credit Card of type '1'");
            Assert.AreEqual("Discover",ccFound.Description,"Wrong discription for Discover Credit Card");
        }

        [Test()]
        public void TestCreditCardTypeWith()
        {
            CreditCardProvider cc = ccBroker.CreditCardTypeWith("1");
            Assert.IsNotNull(cc, "Did not find credit card of type '1'");
            Assert.AreEqual("Discover", cc.Description, "Wrong discription for Discover Credit Card");
        }

        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) ) ]
        public void TestCreditCardTypeWithNull()
        {
            CreditCardProvider cc = ccBroker.CreditCardTypeWith( null ) ;
        }

        [Test()]
        [ExpectedException( typeof( BrokerException ) ) ] 
        public void TestCreditCardTypeWithInvalidCode()
        {
            CreditCardProvider cc = ccBroker.CreditCardTypeWith( "BLAH" ) ;
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static ICreditCardTypeBroker ccBroker = null;
        #endregion
    }
}