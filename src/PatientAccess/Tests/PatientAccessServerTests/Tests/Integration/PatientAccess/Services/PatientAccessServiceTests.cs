using System;
using System.Configuration;
using PatientAccess.Domain;
using PatientAccess.HDI;
using PatientAccess.Services;
using PatientAccess.Services.Hdi;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Services
{
    /// <summary>
    /// Summary description for PatientAccessServiceTests
    /// </summary>
    [TestFixture]
    public class PatientAccessServiceTests
    {

        #region Data elements
        
        #endregion


        #region Constants

        private const string    ACO_FACILITY_CODE       = "ACO",
                                SLU_FACILITY_CODE       = "SLU", 
                                PATIENT_ACCESS_USER_ID  = "PACCESS",
                                TEST_FACILITY           = "Test Facility",
                                INVALID_HSP_MSG         = "Invalid HSP Code found",
                                CIEGUID                 = "CIEGUID",
                                URL                     = "http://www.perotsystem.net/" ;

        #endregion
        
        #region SetUp and TearDown HDIServiceBrokerTests

        [SetUp]
        public void SetUpPatientAccessServiceTests()
        {
            
        }

        [TearDown]
        public void TearDownPatientAccessServiceTests()
        {
        }

        #endregion

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestConnectionSpec()
        {
            HDIService hdiService = new HDIService();
            Facility facility = new Facility();
            facility.Code = ACO_FACILITY_CODE;
            facility.Description = TEST_FACILITY;

            //ConnectionSpec spec = new ConnectionSpec();

            ConnectionSpecRequest connectionSpecRequest =
                new ConnectionSpecRequest();
            connectionSpecRequest.hospitalCode = ACO_FACILITY_CODE; 
            connectionSpecRequest.userID = PATIENT_ACCESS_USER_ID;
            connectionSpecRequest.clientGUID = ConfigurationManager.AppSettings[ CIEGUID ];             

            try
            {
                ConnectionSpecification connSpec = hdiService.PatientAccessService.getConnectionSpecFor( connectionSpecRequest ) ;
                Assert.AreEqual( ACO_FACILITY_CODE, connSpec.hospitalCode, INVALID_HSP_MSG ) ;

                connectionSpecRequest.hospitalCode = SLU_FACILITY_CODE;
                connSpec = hdiService.PatientAccessService.getConnectionSpecFor( connectionSpecRequest ) ; 
                Assert.AreEqual( SLU_FACILITY_CODE, connSpec.hospitalCode, INVALID_HSP_MSG );

                Console.WriteLine(facility.Description);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void TestURLPropertys()
        {
            IPatientAccessService lPatientAccessService = new PatientAccessService() ;
            lPatientAccessService.Url = URL ;
            Assert.AreEqual( URL, lPatientAccessService.Url ) ;
        }

        [Test()]
        public void TestUseDefaultCredentialsPropertys()
        {
            IPatientAccessService lPatientAccessService = new PatientAccessService();
            lPatientAccessService.UseDefaultCredentials = true ;
            Assert.AreEqual( true, lPatientAccessService.UseDefaultCredentials ) ;
        }
    }
}