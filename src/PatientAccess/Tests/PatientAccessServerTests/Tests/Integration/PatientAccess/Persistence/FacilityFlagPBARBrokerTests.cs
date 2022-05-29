using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for FacilityFlagPBARBrokerTests.
    /// </summary>

    //TODO: Create XML summary comment for FacilityFlagPBARBrokerTests
    [TestFixture()]
    public class FacilityFlagPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown FacilityFlagPBARBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpFacilityFlagPBARBrokerTests()
        {
            i_FacilityFlagBroker = BrokerFactory.BrokerOfType<IFacilityFlagBroker>();
            
        }

        [TestFixtureTearDown()]
        public static void TearDownFacilityFlagPBARBrokerTests()
        {
           
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestGetFacilityFlags()
        {

            IList ffs = i_FacilityFlagBroker.FacilityFlagsFor( 900 );
            Assert.IsNotNull( ffs, "No FacilityFlag list returned" );
            Assert.IsTrue( ffs.Count > 0, "No Flags found for facility" );

            bool TESTFlagFound = false;
            foreach( FacilityDeterminedFlag flag in ffs )
            {
                if( flag.Code == "BLOODLESS" )
                    TESTFlagFound = true;
            }
            Assert.IsTrue( TESTFlagFound, "BLOODLESS facility flag not found" );
        }

        [Test()]
        public void TestSingleFacilityFlag()
        {
            FacilityDeterminedFlag flag = i_FacilityFlagBroker.FacilityFlagWith( 900, "BLOODLESS" );
            Assert.IsNotNull( flag, "Did not find Flag 'BLOODLESS'" );
            Assert.AreEqual( "BLOODLESS", flag.Description, "Invalid Description" );

            flag = i_FacilityFlagBroker.FacilityFlagWith( 900, "ZIPPY" );
            Assert.IsFalse( flag.IsValid, "Should not have found Flag with code of ZIPPY" );
        }


        [Test()]
        public void TestFacilityFlagForBlank()
        {
            string blank = String.Empty;
            FacilityDeterminedFlag flag = i_FacilityFlagBroker.FacilityFlagWith( 98, blank );

            Assert.AreEqual(
                blank,
                flag.Code,
                "Code  should be blank" );

            Assert.AreEqual
                ( blank,
                  flag.Description,
                  "Description should be blank"
                );

            Assert.IsTrue(
                flag.IsValid
                );
        }      
       
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IFacilityFlagBroker i_FacilityFlagBroker;

        #endregion
    }
}