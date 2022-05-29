using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Xml;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence;
using PatientAccess.Services;
using Rhino.Mocks;
using NUnit.Framework;
using Is = Rhino.Mocks.Constraints.Is;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for FacilityBrokerTests.
    /// </summary>
    [TestFixture()]
    public class FacilityBrokerTests : AbstractBrokerTests
    {

        #region Constants

        private const long TESTFACILITYID = 6;
        private const string TESTFACILITYHSPCODE = "DEL";
        private const string EXPECTED_FACILITY_NAME = "DELRAY TEST HOSPITAL";
        private const string EXPECTED_FEDERALTAXID = "1";
        private const long EXPECTED_MODTYPE = 11;
        private const string EXPECTED_DATABASENAME = "DVLA";
        private const string EXPECTED_SERVERIP = "155.16.44.25";
        //private const string EXPECTED_CONNECTIONSTRING = "Datasource=155.16.44.25; LibraryList= PADEV,*USRLIBL,PACCESS;NAMING=System;UserID=PACCESS;Password=mat3rh7rn;Database=DVLA;CheckConnectionOnOpen=true;Pooling=true;QueryOptionsFileLibrary=PACCESS;";

        #endregion

        #region SetUp and TearDown Tests

        /// <summary>
        /// Run before each test is executed
        /// </summary>
        [SetUp]
        public void InstanceSetUp()
        {
            
            HttpRuntime.Cache.Remove( CacheKeys.CACHE_KEY_FOR_FACILITIES );

            this.i_MockRepository = new MockRepository();            

            this.i_ObjectUnderTest = new FacilityBroker();

            this.i_ObjectUnderTest.AddressBroker =
                this.i_MockRepository.Stub<IAddressBroker>();

            this.i_ObjectUnderTest.HdiService =
                this.i_MockRepository.Stub<IHDIService>();

            this.i_ObjectUnderTest.FollowUpUnitBroker =
                this.i_MockRepository.Stub<IFollowUpUnitBroker>();

            this.i_ObjectUnderTest.CFDBLookupService =
                this.i_MockRepository.Stub<ICFDBLookupService>();

            this.SetMockResponsesToGeneric();

        }//method

        [TearDown]
        public void InstanceCleanUp()
        {

            HttpRuntime.Cache.Remove( CacheKeys.CACHE_KEY_FOR_FACILITIES );

        }//method

        #endregion

        #region Test Methods

        /// <summary>
        /// Tests all facilities method.
        /// </summary>
        [Test]
        public void TestAllFacilitiesMethod()
        {

            ArrayList facilityList = 
                this.i_ObjectUnderTest.AllFacilities() as ArrayList;
            
            Assert.IsTrue( facilityList != null && facilityList.Count > 0 , 
                           "No Facilities were found" );
            
            foreach( Facility facility in facilityList )
            {
                
                Assert.IsTrue( ( facility.Oid != 999 && facility.FollowupUnit != null )
                               || facility.Oid == 999 );

            }//foreach

        }//method

        [Test()]
        public void TestSpecificHospitalByID()
        {

            Facility f = this.i_ObjectUnderTest.FacilityWith( TESTFACILITYID );
            Assert.IsNotNull( f, "Can not find Facility by ID" );
            Assert.AreEqual(
                TESTFACILITYHSPCODE,
                f.Code,
                "Facility HSPCode is not as expected" );
            Assert.AreEqual(
                EXPECTED_FACILITY_NAME,
                f.Description,
                "Facility Name should be: " + EXPECTED_FACILITY_NAME );
            Assert.AreEqual(
                EXPECTED_MODTYPE,
                f.ModType,
                "Facility ModType should be :" + EXPECTED_MODTYPE );
            Assert.AreEqual(
                EXPECTED_FEDERALTAXID,
                f.FederalTaxID,
                "Federal Tax Id should be: " + EXPECTED_FEDERALTAXID );
            Assert.AreEqual(
                EXPECTED_DATABASENAME,
                f.ConnectionSpec.DatabaseName,
                "Database name Id should be: " + EXPECTED_DATABASENAME);
            Assert.AreEqual(
                EXPECTED_SERVERIP,
                f.ConnectionSpec.ServerIP,
                "Database name Id should be: " + EXPECTED_SERVERIP);
            Assert.IsTrue(
                f.ConnectionSpec.ConnectionString.Contains( "DVLA" ),
                "Connection spec should be: " + f.ConnectionSpec.ConnectionString);
        
            Assert.IsNotNull( f.ContactPoints, "No Contact point for facility" );
            Assert.AreEqual( 1, f.ContactPoints.Count, "Wrong number of contact Points for facility" );
            Assert.AreEqual( f.IsOrderCommunicationFacility, true, "Facility should be an Order Communication (OC) facility" );
            Assert.AreEqual(
                "Y",
                f.Reregister.Code,
                "Facility reregister should be: Y" );
            Assert.AreEqual(
                false,
                f.UsesUSCMRN,
                "UsesUSCMRN should be false" );

        }

        [Test()]
        public void TestSpecificHospitalByHSPCode()
        {
            Facility f = this.i_ObjectUnderTest.FacilityWith( TESTFACILITYHSPCODE );
            Assert.IsNotNull( f, "Can not find Facility by HSPCode" );
            Assert.AreEqual(
                TESTFACILITYID, f.Oid,
                "Facility ID is not as expected" );
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestFacilityNotFoundByCode()
        {

            Facility f = this.i_ObjectUnderTest.FacilityWith( "_-NOTGONNAFINDIT-_" );

        }

        [Test]
        [ExpectedException( typeof( ArgumentException ) )]
        public void TestFacilityNotFoundByNumber()
        {

            Facility f = this.i_ObjectUnderTest.FacilityWith( int.MaxValue );

        }

        [Test()]
        public void GetAppServerDateTime()
        {

            TimeSpan difference =
                DateTime.Now.Subtract( this.i_ObjectUnderTest.GetAppServerDateTime() );
            
            // This is fairly imprecise, but allow for a second of slop
            Assert.IsTrue( difference.Seconds < 1 );

        }//method

        #endregion

        #region Support Methods

        /// <summary>
        /// Sets the mock responses to generic.
        /// </summary>
        private void SetMockResponsesToGeneric()
        {

            this.i_GenericMockState =
                new State( 0, "TX", "TEXAS" );

            this.i_GenericMockCountry =
                new Country( "US", "UNITED STATES" );

            this.i_GenericMockFollowUpUnit =
                new FollowupUnit( 000, DateTime.MaxValue, "Mock FollowUpUnit" );

            this.i_GenericMockFaciltyAddressResponse = new XmlDocument();
            this.i_GenericMockFaciltyAddressResponse.LoadXml( "<Addresses xmlns=\"\"><Address AddressID=\"16454\" TypeCD=\"HQ\" Address1=\"3280 Joe Battle Boulevard\" Address2=\"\" Address3=\"\" City=\"El Paso\" StateCD=\"TX\" ZipCD=\"79938\" County=\"El Paso\" CountryCD=\"\" MailToInd=\"false\"/></Addresses>" );

            this.i_GenericMockFacilityPhonesReponse = new XmlDocument();
            this.i_GenericMockFacilityPhonesReponse.LoadXml( "<Phones xmlns=\"\"><Phone PhoneID=\"85\" PhoneNumber=\"561-495-3103\" Extension=\"\" TypeCD=\"FAX\"/><Phone PhoneID=\"86\" PhoneNumber=\"561-498-4440\" Extension=\"\" TypeCD=\"MAIN\"/></Phones>" );

            using( this.i_MockRepository.Record() )
            {

                SetupResult.For( this.i_ObjectUnderTest.FollowUpUnitBroker.FollowUpUnitWith( 1 ) )
                    .IgnoreArguments()
                    .Constraints( Is.GreaterThan(0L) )
                    .Return( this.i_GenericMockFollowUpUnit );

                SetupResult.For( this.i_ObjectUnderTest.FollowUpUnitBroker.FollowUpUnitWith( 0 ) )
                    .IgnoreArguments()
                    .Constraints( Is.Equal(0L) )
                    .Return( null );
                
                SetupResult.For( this.i_ObjectUnderTest.CFDBLookupService.GetFacilityAddresses( string.Empty ) )
                    .IgnoreArguments()
                    .Return( this.i_GenericMockFaciltyAddressResponse );

                SetupResult.For( this.i_ObjectUnderTest.CFDBLookupService.GetFacilityPhones( string.Empty ) )
                    .IgnoreArguments()
                    .Return( this.i_GenericMockFacilityPhonesReponse );

                SetupResult.For( this.i_ObjectUnderTest.AddressBroker.CountryWith( string.Empty, null ) )
                    .IgnoreArguments()
                    .Return( this.i_GenericMockCountry );

                SetupResult.For( this.i_ObjectUnderTest.AddressBroker.StateWith( string.Empty, null ) )
                    .IgnoreArguments()
                    .Return( this.i_GenericMockState );

            }//using

        }//method

        #endregion

        #region Data Elements

        FacilityBroker i_ObjectUnderTest;
        MockRepository i_MockRepository;

        FollowupUnit i_GenericMockFollowUpUnit;
        XmlDocument i_GenericMockFaciltyAddressResponse;
        XmlDocument i_GenericMockFacilityPhonesReponse;
        State i_GenericMockState;
        Country i_GenericMockCountry;

        #endregion

    }
}