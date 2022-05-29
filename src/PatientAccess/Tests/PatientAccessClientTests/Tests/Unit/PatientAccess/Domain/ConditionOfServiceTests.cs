using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ConditionOfServiceTests
    {
        #region Constants
        private const int PARAM_OID = 0;
        #endregion

        #region SetUp and TearDown ConditionOfServiceTests
        [TestFixtureSetUp()]
        public static void SetUpConditionOfServiceTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownConditionOfServiceTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestConditionOfServiceForBlank()
        {

             cs = new ConditionOfService( PARAM_OID, ReferenceValue.NEW_VERSION,
                                                            ConditionOfService.BLANK, ConditionOfService.BLANK );

            Assert.AreEqual( String.Empty, cs.Description.Trim() );
            Assert.AreEqual( String.Empty, cs.Code.Trim() );
            Assert.AreEqual( 0, cs.Oid );
        }


        [Test()]
        public void TestConditionOfServiceForYes()
        {
            cs = new ConditionOfService( PARAM_OID, ReferenceValue.NEW_VERSION, ConditionOfService.YES_DESCRIPTION,
                                         ConditionOfService.YES );
           Assert.AreEqual( "Yes", cs.Description );
           Assert.AreEqual( "Y", cs.Code );
           Assert.AreEqual( 0, cs.Oid );
        }
        [Test()]
        public void TestConditionOfServiceForUnable()
        {

             cs = new ConditionOfService( PARAM_OID, ReferenceValue.NEW_VERSION,
                                                           ConditionOfService.UNABLE_DESCRIPTION, ConditionOfService.UNABLE );
            Assert.AreEqual( "No, Patient Medically Unable to Sign", cs.Description );
            Assert.AreEqual( "U", cs.Code );
            Assert.AreEqual( 0, cs.Oid );
        }


        [Test()]
        public void TestConditionOfServiceForRefused()
        {

             cs = new ConditionOfService( PARAM_OID, ReferenceValue.NEW_VERSION,
                                                             ConditionOfService.REFUSED_DESCRIPTION, ConditionOfService.REFUSED );
            Assert.AreEqual( "No, Patient Refused to Sign", cs.Description );
            Assert.AreEqual( "R", cs.Code );
            Assert.AreEqual( 0, cs.Oid );
        }


        [Test()]
        public void TestConditionOfServiceForNotAvailable()
        {

             cs = new ConditionOfService( PARAM_OID, ReferenceValue.NEW_VERSION,
                                                            ConditionOfService.NOT_AVAILABLE_DESCRIPTION, ConditionOfService.NOT_AVAILABLE );
            Assert.AreEqual( "No, Patient Not Available to Sign", cs.Description );
            Assert.AreEqual( "N", cs.Code );
            Assert.AreEqual( 0, cs.Oid );
        }

    

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private ConditionOfService cs = new ConditionOfService();
        #endregion
    }
}