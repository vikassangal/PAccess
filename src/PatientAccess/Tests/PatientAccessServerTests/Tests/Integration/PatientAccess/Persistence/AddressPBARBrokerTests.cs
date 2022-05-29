using System;
using System.Collections;
using System.Data;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for AddressPBARBrokerTests.
    /// </summary>
	
    [TestFixture()]
    public class AddressPBARBrokerTests : AbstractPBARBroker
    {
        #region Constants
        private const int ACO_FACILITY_ID   = 900 ;
        #endregion

        #region Setup and TearDown
        [SetUp]
        public void SetUpAddressPBARBrokerTests()
        {
            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_ACOFacility = fb.FacilityWith( ACO_FACILITY_ID );

            i_AddressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
            dbConnection = new iDB2Connection();
            dbConnection.ConnectionString = i_ACOFacility.ConnectionSpec.ConnectionString;
            dbConnection.Open(); 
        }

        [TestFixtureTearDown()]
        public static void TearDownAddressPBARBrokerTests()
        {
            i_AddressBroker = null;

            if( dbConnection != null )
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestSaveEmployerAddress()
        {
            try
            {
                Employer employer = new Employer();
                employer.Name = "PA Nunit Test Employer";

                State st = new State("TX");
                Country cntry = new Country("USA");

                Address address =  new Address( 
                    "Address1","Address2","City",new ZipCode( "12345" ),st,cntry );
                PhoneNumber phone = new PhoneNumber("999","999999");
                EmailAddress email = new EmailAddress("PANUnitTest@PA.com");

                employer.EmployerCode = 530;
                EmployerContactPoint aContactPoint =  new EmployerContactPoint( 
                    address, phone, email, TypeOfContactPoint.NewEmployerContactPointType() );
                employer.PartyContactPoint =  aContactPoint;

                dbTransaction = dbConnection.BeginTransaction();
                IAddressBroker addressBroker = new AddressPBARBroker(dbTransaction);
                
                addressBroker.SaveEmployerAddress( 
                    employer, i_ACOFacility.Code );
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                dbTransaction.Rollback();
            }
        }

        [Test()]
        public void AllCountries()
        {
            Country afghanistan = null,
                    albania = null,
                    china = null;

            IList countries = i_AddressBroker.AllCountries( ACO_FACILITY_ID );

            // this test is OK. US must be 1st in this list.
            Assert.AreEqual(
                "USA",
                ( (Country)countries[1] ).Code,
                "First entry should be USA" );

            foreach( Country country in countries )
            {
                switch( country.Code )
                {
                    case "AFG":
                        afghanistan = country;
                        break;
                    case "ALB":
                        albania = country;
                        break;
                    case "CHN":
                        china = country;
                        break;
                    default:
                        break;
                }
            }

            Assert.IsNotNull( afghanistan, "Afghanistan not found" );
            Assert.AreEqual(
                "AFG",
                afghanistan.Code,
                "Afghanistan code incorrect" );

            Assert.IsNotNull( albania, "Albania not found" );
            Assert.AreEqual(
                "ALB",
                albania.Code,
                "Albania code incorrect" );

            Assert.IsNotNull( china, "China not found" );
            Assert.AreEqual(
                "CHN",
                china.Code.Trim(),
                "China code incorrect" );
        }

        [Test()]
        public void TestCountryByCode()
        {
            Country canada = i_AddressBroker.CountryWith( ACO_FACILITY_ID, "CAN" );
            Assert.IsNotNull( canada, "Country object for Canada not found" );
            Assert.AreEqual( "CANADA", canada.Description.ToUpper(), "Country Description not correct" );

            Country badCountry = i_AddressBroker.CountryWith(ACO_FACILITY_ID, "CCP" );
            Assert.IsNotNull( badCountry, "No unknown country returned for 'CCP'" );
            Assert.AreEqual( badCountry.Code, string.Empty );
        }

        [Test(), ExpectedException( typeof( ArgumentNullException) ) ]
        public void TestCountryWithNullCode()
        {
            Country canada = i_AddressBroker.CountryWith(ACO_FACILITY_ID, null );
        }

        [Test(), ExpectedException( typeof( ArgumentNullException ) ) ] 
        public void TestTwoParameterCountryWithMethodForNullCode()
        {
            Country canada = i_AddressBroker.CountryWith( ACO_FACILITY_ID, null );
        }

        [Test(), ExpectedException( typeof( ArgumentNullException ) ) ] 
        public void TestCountyWithNullCode()
        {
            County someCounty = i_AddressBroker.CountyWith( ACO_FACILITY_ID,null, null ) ;
        }

        [Test(), ExpectedException( typeof( ArgumentNullException ) ) ]
        public void TestStateWithNullCode()
        {
            State someState = i_AddressBroker.StateWith( null, i_ACOFacility);
        }
        
        [Test()]
        public void TestStateWithMethod()
        {
            const string stateCode = "OH";
            const string expectedStateDescription = "OHIO";

            State State = i_AddressBroker.StateWith(stateCode, i_ACOFacility);

            Assert.AreEqual(expectedStateDescription, State.Description.ToUpper(),"Expected value for the state description is not correct.");
            Assert.IsNotNull(State.StateNumber);
        }

        [Test()]
        public void AllStates()
        {
            const int facilityNumber = 98;
            ICollection states = i_AddressBroker.AllStates(facilityNumber);
            State alabama = null, alaska = null, nevada = null;
            foreach( State state in states )
            {
                switch( state.Code )
                {
                    case "AL":
                        alabama = state;
                        break;
                    case "AK":
                        alaska = state;
                        break;
                    case "NV":
                        nevada = state;
                        break;
                    default:
                        break;
                }
            }

            Assert.IsNotNull( alabama, "Did not find state object for Alabama" );
            Assert.AreEqual(
                "ALABAMA",
                alabama.Description,
                "Description for ALABAMA state not correct" );

            Assert.IsNotNull( alaska, "Did not find state object for Alaska" );
            Assert.AreEqual(
                "ALASKA",
                alaska.Description,
                "Description for ALASKA state not correct" );

            Assert.IsNotNull( nevada, "Did not find state object for Nevada" );
            Assert.AreEqual(
                "NEVADA",
                nevada.Description,
                "Description for NEVADA state not correct" );
            Assert.IsNotNull(nevada.StateNumber, "The StateNumber property is not initialized");
        }

        [Test()]
        public void TestOneStateByCode()
        {
            State state = i_AddressBroker.StateWith( 98,"TX" );
            Assert.IsNotNull( state, "State object of Texas not found" );
            Assert.AreEqual( "TX", state.Code );
            Assert.AreEqual( "TEXAS", state.Description.ToUpper() );
            Assert.IsNotNull(state.StateNumber);

            state = i_AddressBroker.StateWith( 98,"Z1" );
            Assert.AreEqual(  state.Description, string.Empty );
        }

        [Test()]
        public void AllCounties()
        {
            ArrayList counties = (ArrayList)i_AddressBroker.AllCountiesFor( 98 );
            Assert.IsNotNull( counties, "Did not find counties for facility: 98" );
            Assert.IsTrue( counties.Count > 0, "Did not find any counties for facility 98" );
        }

        [Test()]
        public void TestCountyWith()
        {
            const string countyCode = "006";
            const string expectedCountyDescription = "CIBOLA"; 
            const string expectedStateCode = "NM"; 
            const int facilityNumber = 98;

            var county = i_AddressBroker.CountyWith( facilityNumber,expectedStateCode, countyCode );

            Assert.IsNotNull( county, "Did not find county with country code " + countyCode ); 
            Assert.AreEqual( expectedCountyDescription, county.Description, string.Format("Did not find county with description = {0} for county code {1}", expectedCountyDescription, countyCode  ));
            Assert.AreEqual(expectedStateCode,county.StateCode,"The expected state code for the county is not correct");
        }
       
        [Test()]
        public void TestUnkownCountry()
        {
            Country aCountry = i_AddressBroker.CountryWith( ACO_FACILITY_ID, "SOME BIG FAT FOO" );

            Assert.AreEqual(
                typeof( UnknownCountry ),
                aCountry.GetType()
                );

            Assert.AreEqual(
                String.Empty,
                aCountry.Code
                );

            Assert.AreEqual(
                "SOME BIG FAT FOO",
                aCountry.Description
                );

            Assert.AreEqual(
                "SOME BIG FAT FOO",
                aCountry.ToCodedString().Trim()
                );

            Assert.AreEqual(
                "SOME BIG FAT FOO",
                aCountry.ToString()
                );
        }
     
        [Test()]
        public void TestStateForBlank()
        {
            string blank = String.Empty;

            State state = i_AddressBroker.StateWith( 98,blank );

            Assert.AreEqual(
                blank,
                state.Code,
                "Code  should be blank" );

            Assert.AreEqual
                ( blank,
                  state.Description,
                  "Description should be blank"
                );

            Assert.IsTrue(
                state.IsValid
                );
        }

        [Test()]
        public void TestCountryForBlank()
        {
            string blank = String.Empty;

            Country country = i_AddressBroker.CountryWith( ACO_FACILITY_ID, blank );

            Assert.AreEqual(
                blank,
                country.Code,
                "Code  should be blank" );

            Assert.AreEqual
                ( blank,
                  country.Description,
                  "Description should be blank"
                );

            Assert.IsTrue(
                country.IsValid
                );
        }

        [Test()]
        public void TestGetCountiesForState()
        {
            const string ohioStateCode = "OH";
            const string texasStateCode = "TX";
            const long facilityNumber = 98;

            var listOfCountiesForOhio = i_AddressBroker.GetCountiesForAState(ohioStateCode, facilityNumber);
            var listOfCountiesForTexas = i_AddressBroker.GetCountiesForAState(texasStateCode, facilityNumber);

            Assert.IsNotNull(listOfCountiesForOhio, "The list of counties should not be null");
            Assert.AreEqual(ohioStateCode, listOfCountiesForOhio[0].StateCode, "The populated list of Counties for the state is invaild");
            Assert.AreNotSame(listOfCountiesForOhio, listOfCountiesForTexas, "The populated list of Counties for both the states should not be same");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  IAddressBroker  i_AddressBroker;
        private static  Facility                    i_ACOFacility;
        private static  IDbConnection               dbConnection;
        private static  IDbTransaction              dbTransaction;   

        #endregion
    }
}