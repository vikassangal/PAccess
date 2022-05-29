using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for HospitalClinicPBARBrokerTests
    /// </summary>

    [TestFixture()]
    public class HospitalClinicsPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown HospitalClinicPBARBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpHospitalClinicsPBARBrokerTests()
        {
            hcBroker = BrokerFactory.BrokerOfType<IHospitalClinicsBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownHospitalClinicsPBARBrokerTests()
        {
            hcBroker = null;
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestPreAdmitHospitalClinic()
        {
            HospitalClinic clinic = hcBroker.PreTestHospitalClinicFor( 54 );

            Assert.IsNotNull( clinic, "Did not find preAdmitClinic for facility 54" );
            Assert.AreEqual( "Y", clinic.PreAdmitTest, "Pre admit flag is not correct" );
        }



        [Test()]
        [Ignore]
        public void TestHospitalClinicsFor()
        {
            HospitalClinic argonlaser = null;
            HospitalClinic bloodtransfusion = null;

            ArrayList clinics = (ArrayList)hcBroker.HospitalClinicsFor( 6 );

            foreach( HospitalClinic hospitalClinic in clinics )
            {
                switch( hospitalClinic.Code )
                {
                    case "AL":
                        argonlaser = hospitalClinic;
                        break;
                    case "BT":
                        bloodtransfusion = hospitalClinic;
                        break;
                    default:
                        break;
                }
            }

            Assert.IsNotNull( argonlaser, "HospitalClinic Argon Laser not found" );
            Assert.AreEqual( argonlaser.Description, "ARGON LASER",
                             "Description incorrect for ARGON LASER" );
            Assert.IsNotNull( bloodtransfusion, "HospitalClinic Blood transfusion not found" );
            Assert.AreEqual( bloodtransfusion.Description, "BLOOD TRANSFUSION",
                             "Description incorrect for BLOOD TRANSFUSION" );
        }

        [Test()]
        public void TestFindHospitalClinic()
        {
            HospitalClinic clinic = hcBroker.HospitalClinicWith( 900, "OR" );
            Assert.IsNotNull( clinic, "Did not find ORTHOPEDIC clinic" );
            Assert.AreEqual( clinic.Description, "ORTHOPEDICS 01", "Description of clinic is incorrect" );

            clinic = hcBroker.HospitalClinicWith( 900, "Z1" );
            Assert.IsFalse( clinic.IsValid, "Should not have found clinic with code = Z1" );
        }

        [Test()]
        public void TestHospitalClinicForBlank()
        {
            string blank = String.Empty;
            HospitalClinic clinic = hcBroker.HospitalClinicWith( 900, blank );


            Assert.AreEqual(
                blank,
                clinic.Code,
                "Code  should be blank" );

            Assert.AreEqual
                ( blank,
                  clinic.Description,
                  "Description should be blank"
                );

            Assert.IsTrue(
                clinic.IsValid
                );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  IHospitalClinicsBroker hcBroker = null;

        #endregion

    }
}