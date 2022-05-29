using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for PatientBrokerProxyTest.
    /// </summary>

    [TestFixture()]
    public class PatientBrokerTests 
    {
        
        #region SetUp and TearDown PatientBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpPatientBrokerTests()
        {
            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_Facility = fb.FacilityWith( 900 );
            
            i_PatientBroker = new PatientBrokerProxy( );
        }

        [TestFixtureTearDown()]
        public static void TearDownPatientBrokerTests( )
        {
        }
        #endregion

        #region Test Methods
      
        [Test()]
        public void TestPatientTypes()
        {
            VisitType preadmit = null;
            VisitType inpatient = null;
            VisitType outpatient = null;

            ArrayList allTypes = (ArrayList)i_PatientBroker.AllPatientTypes( i_Facility.Oid );
            
            foreach( VisitType type in allTypes )
            {
                switch( type.Code )
                {
                    case "0": 
                        preadmit = type;
                        break;
                    case "1": 
                        inpatient = type;
                        break;
                    case "2": 
                        outpatient = type;
                        break;
                    default:
                        break;
                }
            }
            Assert.AreEqual(
                VisitType.PREREG_PATIENT_DESC,
                preadmit.Description,
                "PatientType 0 should be PREADMIT ");

            Assert.AreEqual(
                VisitType.INPATIENT_DESC,
                inpatient.Description,
                "PatientType 0 should be INPATIENT  ");

            Assert.AreEqual(
                VisitType.OUTPATIENT_DESC,
                i_PatientBroker.PatientTypeWith( "2", i_Facility.Oid ).Description,
                "PatientType 2 should be OUTPATIENT");
        }

        [Test()]
        //[Ignore("Blank Test, planned for Refactoring- SKIP")]
        public void TestPatientTypeForBlank()
        {            
            string blank = string.Empty;

            VisitType vt = i_PatientBroker.PatientTypeWith( blank, i_Facility.Oid );
         
            Assert.AreEqual( blank, vt.Code, "Code should be blank" );
            Assert.AreEqual( blank, vt.Description, "Description should be blank" );
            Assert.IsTrue( vt.IsValid );
        }

        [Test()]
        public void TestPatientTypeForInvalid()
        {            
            string inValid = "5";

            VisitType vt = i_PatientBroker.PatientTypeWith( inValid, i_Facility.Oid );
         
            Assert.IsFalse(
                vt.IsValid            
                );
        }
        #endregion

        #region Support Methods
        
        #endregion

        #region Data Elements

        private static PatientBrokerProxy          i_PatientBroker;
        private static Facility                    i_Facility;
        
        #endregion
    }
}