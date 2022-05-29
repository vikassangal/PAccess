using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class DriversLicenseTests
    {
        #region SetUp and TearDown DriversLicenseTests
        [TestFixtureSetUp()]
        public static void SetUpDriversLicenseTests()
        {         
        }

        [TestFixtureTearDown()]
        public static void TearDownDriversLicenseTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestValidDriverLicenses()
        {   

            DriversLicense dl1 = new DriversLicense( VALID_DRIVERSLICENSE1, 
                                                     new State( 0L, DateTime.Now, VALID_STATEDESCRIPTION1, VALID_STATECODE1));

            DriversLicense dl2 = new DriversLicense( VALID_DRIVERSLICENSE2, 
                                                     new State( 0L, DateTime.Now, VALID_STATEDESCRIPTION2, VALID_STATECODE2));

            Assert.AreEqual(
                VALID_DRIVERSLICENSE1,
                dl1.Number
                );

            Assert.AreEqual(
                VALID_STATECODE1,
                dl1.State.Code
                );           

            Assert.AreEqual(
                VALID_STATEDESCRIPTION1,
                dl1.State.Description
                );

            Assert.AreEqual(
                false,
                dl1.Equals(dl2)
                );

            Assert.AreEqual(
                false,
                dl1.Equals(null)
                );

            Assert.AreNotEqual(
                dl1.GetHashCode(),
                dl2.GetHashCode()
                );

        }

        [Test()]
        public void TestINullable()
        {
            DriversLicense dl = new DriversLicense( string.Empty);
            Assert.AreEqual( dl.IsNull(), false );

            DriversLicense nullDl = DriversLicense.NewMissingDriversLicense();
            Assert.AreEqual( nullDl.IsNull(), true );
        }

        [Test()]
        public void TestMissingDriversLicense()
        {
            DriversLicense missing = DriversLicense.NewMissingDriversLicense();
            
            Assert.AreEqual(
                missing.Number,
                String.Empty
                );

            Assert.IsNotNull( missing.State );
        }
        #endregion


        #region Data Elements
        const string
            VALID_DRIVERSLICENSE1    = "12345678",
            VALID_STATECODE1         = "FL",          
            VALID_STATEDESCRIPTION1  = "FLORIDA",          

            VALID_DRIVERSLICENSE2    = "87654321",
            VALID_STATECODE2         = "TX",  
            VALID_STATEDESCRIPTION2  = "TEXAS";          
  
        #endregion
    }
}