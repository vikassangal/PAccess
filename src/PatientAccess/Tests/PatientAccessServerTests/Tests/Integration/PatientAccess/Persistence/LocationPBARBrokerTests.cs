using System;
using System.Collections;
using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for LocationBrokerTests.
    /// </summary>
    [TestFixture()]
    public class LocationPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants

        private const string DHF_HSP_CODE = "DHF";
        private const string NURSING_STATION_CODE = "2S";
        private const string ROOM_CODE = "291";
        private const string ISOLATION_CODE = "C";

        #endregion        

        #region SetUp and TearDown LocationPBARBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpLocationPBARBrokerTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_ACOFacility = facilityBroker.FacilityWith( 900 );
        }

        [TestFixtureTearDown()]
        public static void TearDownLocationPBARBrokerTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        [Ignore("ignore the duplicate bed assignment tests until an approriate account is created")]
        public void TestDuplicateBedAssignmentByMRN()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

            DuplicateLocationResult dlr = locationBroker.CheckForDuplicateBedAssignments( i_ACOFacility,
                                                                                          "PUNXSUTAWNEY",
                                                                                          "PHIL",
                                                                                          0,
                                                                                          785343,
                                                                                          new SocialSecurityNumber("506401432"),
                                                                                          new DateTime(1918,9,12),
                                                                                          "92161");

            Assert.AreEqual( DuplicateBeds.MatchedDupes, dlr.dupeStatus, "Expected Matched Dupes enumeration." );
        }

        [Test()]
        [Ignore("ignore the duplicate bed assignment tests until an approriate account is created")]
        public void TestDuplicateBedAssignmentBySSN()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

            DuplicateLocationResult dlr = locationBroker.CheckForDuplicateBedAssignments( i_ACOFacility,
                                                                                          "PUNXSUTAWNEY",
                                                                                          "PHIL",
                                                                                          0,
                                                                                          0,
                                                                                          new SocialSecurityNumber( "506401432" ),
                                                                                          new DateTime( 1918, 9, 12 ),
                                                                                          "92161" );

            Assert.AreEqual( DuplicateBeds.MatchedDupes, dlr.dupeStatus, "Expected Matched Dupes enumeration." );
        }

        [Test()]
        [Ignore ("ignore the duplicate bed assignment tests until an approriate account is created")]
        public void TestDuplicateBedAssignmentByName()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

            DuplicateLocationResult dlr = locationBroker.CheckForDuplicateBedAssignments( i_ACOFacility,
                                                                                          "PUNXSUTAWNEY",
                                                                                          "PHIL",
                                                                                          0,
                                                                                          0,
                                                                                          new SocialSecurityNumber(),
                                                                                          new DateTime( 1918, 9, 12 ),
                                                                                          "92160" );

            Assert.AreEqual( DuplicateBeds.PotentialDupes, dlr.dupeStatus, "Expected Potential Dupes enumeration." );
        }

        [Test()]
        public void TestDuplicateBedAssignmentOmitCurrentAccount()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

            DuplicateLocationResult dlr = locationBroker.CheckForDuplicateBedAssignments( i_ACOFacility,
                                                                                          "PUNXSUTAWNEY",
                                                                                          "PHIL",
                                                                                          43125,
                                                                                          785343,
                                                                                          new SocialSecurityNumber( "506401432" ),
                                                                                          new DateTime( 1918, 9, 12 ),
                                                                                          "92161" );

            Assert.AreEqual( DuplicateBeds.NoDupes, dlr.dupeStatus, "Expected No Dupes enumeration." );
        }


        [Test()]
        public void TestReserveAndRelease()
        {
            try
            {

                ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

                NursingStation newns = new NursingStation( ReferenceValue.NEW_OID, 
                                                           ReferenceValue.NEW_VERSION,string.Empty," N" );
                Room newroom = new Room( ReferenceValue.NEW_OID, 
                                         ReferenceValue.NEW_VERSION, string.Empty, "0629" );
                Bed newbed = new Bed( ReferenceValue.NEW_OID, 
                                      ReferenceValue.NEW_VERSION,string.Empty, "G" );

                
                Location newlocation = new Location( ReferenceValue.NEW_OID, 
                                                     ReferenceValue.NEW_VERSION,string.Empty,string.Empty, newns, newroom, newbed );

                
                VisitType patientType = new VisitType(
                    ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT);

                ReservationCriteria reservationCriteria = new ReservationCriteria(
                    ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION,
                    null, newlocation, i_ACOFacility, patientType );

                ReservationResults reservationResult = locationBroker.Reserve( 
                    reservationCriteria );

                string msg = reservationResult.Message;


                //                Assert.IsTrueEquals( 
                //                    "The bed status should be RESERVED",
                //                    "Reserved",
                //                    reservationResult.Description.ToString()
                //                    );

                Assert.IsNotNull( 
                    reservationResult,
                    "Did not yeild a reservation result"
                    );

                locationBroker.ReleaseReservedBed( newlocation, i_ACOFacility );
            }
            catch( Exception ex )
            {
                Assert.Fail( 
                    "Reservation failed " + ex.Message );
            }

        }
       

       
        [Test()]
        [Ignore("temporarily ignore while PBAR fixes the entries")]
        public void TestAccomodationCodesFor()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
            NursingStation ns = new NursingStation( ReferenceValue.NEW_OID, 
                                                    ReferenceValue.NEW_VERSION,string.Empty,"N" );

            ArrayList accomodationsCodes = ( ArrayList )locationBroker.AccomodationCodesFor( ns.Code, i_ACOFacility );
            Assert.IsNotNull( 
                accomodationsCodes,
                "Did not find any accomodation codes for the location selected"
                );

            ns = new NursingStation( ReferenceValue.NEW_OID, 
                                     ReferenceValue.NEW_VERSION,string.Empty,"B4" );  //has 4 rtn

            accomodationsCodes = ( ArrayList )locationBroker.AccomodationCodesFor( ns.Code, i_ACOFacility );
            Assert.AreEqual( 7, accomodationsCodes.Count, "has codes:" );
                
        } 

        [Test()]
        public void TestRoomsFor()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
            string nursingStationCode = "4N";

            ArrayList rooms = ( ArrayList )locationBroker.RoomsFor( i_ACOFacility, nursingStationCode );
            Assert.IsNotNull( 
                rooms,
                "Did not find Rooms for the Nursing Station selected"
                );
        }

        [Test()]
        public void TestGetEntireNursingStationCode()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
            string longNSCode = locationBroker.GetEntireNursingStationCode(i_ACOFacility, "N");

            Assert.AreEqual(" N", longNSCode, "Long nursingStation name not found");
        }

        [Test()]
        public void TestGetAllNursingStationsFromCache()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
            IList<NursingStation> list = locationBroker.NursingStationsFor(i_ACOFacility);
            Assert.IsNotNull(list,"No nursing station list found for ACO");
            Assert.AreNotEqual(0,list.Count,"No nursing stations found for ACO");
        }
        [Test()]
        public void TestGetAllNursingStationsBypassingCache()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
            IList<NursingStation> list = locationBroker.NursingStationsFor( i_ACOFacility, false );
            Assert.IsNotNull(list, "No nursing station list found for ACO");
            Assert.AreNotEqual(0, list.Count, "No nursing stations found for ACO");
        }
        [Test()]
        public void TestLocationsMatchingWithGender()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
            LocationSearchCriteria locationSearchCriteria = 
                new LocationSearchCriteria( "ACO",
                                            "F",//null,
                                            "B1",
                                            "All",
                                            false );
            
            ICollection accountProxiesCollection = 
                locationBroker.LocationMatching( locationSearchCriteria) ;

        }


        [Test()]
        public void TestLocationsMatchingWithoutGender()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
            LocationSearchCriteria locationSearchCriteria = 
                new LocationSearchCriteria( "ACO",
                                            null,
                                            "B1",
                                            "All",
                                            false );
            
            ICollection accountProxiesCollection = 
                locationBroker.LocationMatching( locationSearchCriteria) ;
        }

        [Test()]
        public void Test_IsolationCode_ForMatchingNursingSations()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
            LocationSearchCriteria locationSearchCriteria =
                new LocationSearchCriteria(DHF_HSP_CODE,
                                            null,
                                            NURSING_STATION_CODE,
                                            ROOM_CODE,
                                            false);

            ICollection accountProxiesCollection =
                locationBroker.LocationMatching(locationSearchCriteria);
            
            ArrayList arrayList = new ArrayList(accountProxiesCollection);
            AccountProxy accountProxy = (AccountProxy)arrayList[0];

            Assert.AreEqual(ISOLATION_CODE, accountProxy.IsolationCode, "Isolation code is not matching");
        }

        [Test()]
        public void TestAccomodationsForFacility()
        {
            ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();
            ArrayList facilityAccomodationList = ( ArrayList )locationBroker.AccomodationsFor( 900 );
            Assert.IsNotNull( facilityAccomodationList, "Accomodation list is empty"  );
            Assert.IsTrue( 
                facilityAccomodationList.Count >= 1,
                "There are no results from the Accomodations Search"
                );
        }
        
        [Test()]
        public void TestGetBedStatus()
        {
            try
            {
                ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

                NursingStation ns = new NursingStation( ReferenceValue.NEW_OID, 
                                                        ReferenceValue.NEW_VERSION,string.Empty," N" );
                Room room         = new Room( ReferenceValue.NEW_OID, 
                                              ReferenceValue.NEW_VERSION, string.Empty, "0629" );
                Bed bed           = new Bed( ReferenceValue.NEW_OID, 
                                             ReferenceValue.NEW_VERSION,string.Empty, "G" );

                Location location = new Location( ReferenceValue.NEW_OID, 
                                                  ReferenceValue.NEW_VERSION,string.Empty,string.Empty, ns, room, bed );

                ReservationResults reservationResult = locationBroker.GetBedStatus( 
                    location, i_ACOFacility );

                string msg = reservationResult.Message;

                //                Assert.AreEqual(
                //                    "Reserved",
                //                    "Reserved",
                //                    reservationResult.Description.ToString()
                //                    );

                Assert.IsNotNull( 
                    reservationResult,
                    "Did not yeild a reservation result"
                    );
            }
            catch( Exception ex )
            {
                Assert.Fail( 
                    "getbedstatus failed " + ex.Message );
            }
        }
        [Test()]
        public void TestValidateLocation()
        {
            try
            {
                ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

                NursingStation ns = new NursingStation( ReferenceValue.NEW_OID, 
                                                        ReferenceValue.NEW_VERSION,string.Empty," N" );
                Room room         = new Room( ReferenceValue.NEW_OID, 
                                              ReferenceValue.NEW_VERSION, string.Empty, "0629" );
                Bed bed           = new Bed( ReferenceValue.NEW_OID, 
                                             ReferenceValue.NEW_VERSION,string.Empty, "G" );

                Location location = new Location( ReferenceValue.NEW_OID, 
                                                  ReferenceValue.NEW_VERSION,string.Empty,string.Empty, ns, room, bed );

                string locationResult = locationBroker.ValidateLocation(location, i_ACOFacility );

                Assert.IsNotNull( 
                    locationResult,
                    "Invalid Location"
                    );
            }
            catch( Exception ex )
            {
                Assert.Fail( 
                    "TestValidateLocation failed " + ex.Message );
            }
        }
        [Test()]
        public void TestValidateNursingStation()
        {
            try
            {
                ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

                NursingStation ns = new NursingStation( ReferenceValue.NEW_OID, 
                                                        ReferenceValue.NEW_VERSION,string.Empty,"Q" );
                Room room         = new Room( ReferenceValue.NEW_OID, 
                                              ReferenceValue.NEW_VERSION, string.Empty, "0629" );
                Bed bed           = new Bed( ReferenceValue.NEW_OID, 
                                             ReferenceValue.NEW_VERSION,string.Empty, "G" );

                Location location = new Location( ReferenceValue.NEW_OID, 
                                                  ReferenceValue.NEW_VERSION,string.Empty,string.Empty, ns, room, bed );

                string locationResult = locationBroker.ValidateLocation(location, i_ACOFacility );
                Assert.AreEqual( "Invalid Nursingstation", locationResult, "Did not received expected location result" );
            }
            catch( Exception ex )
            {
                Assert.Fail( 
                    "TestValidateLocation failed " + ex.Message );
            }
        }
        [Test()]
        public void TestValidateRoom()
        {
            try
            {
                ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

                NursingStation ns = new NursingStation( ReferenceValue.NEW_OID, 
                                                        ReferenceValue.NEW_VERSION,string.Empty," N" );
                Room room         = new Room( ReferenceValue.NEW_OID, 
                                              ReferenceValue.NEW_VERSION, string.Empty, "0666" );
                Bed bed           = new Bed( ReferenceValue.NEW_OID, 
                                             ReferenceValue.NEW_VERSION,string.Empty, "G" );

                Location location = new Location( ReferenceValue.NEW_OID, 
                                                  ReferenceValue.NEW_VERSION,string.Empty,string.Empty, ns, room, bed );

                string locationResult = locationBroker.ValidateLocation(location, i_ACOFacility );
                Assert.AreEqual( locationResult, "Invalid Room","Invalid Room" );
            }
            catch( Exception ex )
            {
                Assert.Fail( 
                    "TestValidateLocation failed " + ex.Message );
            }
        }
        [Test()]
        public void TestValidateBed()
        {
            try
            {
                ILocationBroker locationBroker = BrokerFactory.BrokerOfType<ILocationBroker>();

                NursingStation ns = new NursingStation( ReferenceValue.NEW_OID, 
                                                        ReferenceValue.NEW_VERSION,string.Empty," N" );
                Room room         = new Room( ReferenceValue.NEW_OID, 
                                              ReferenceValue.NEW_VERSION, string.Empty, "0629" );
                Bed bed           = new Bed( ReferenceValue.NEW_OID, 
                                             ReferenceValue.NEW_VERSION,string.Empty, "Z" );

                Location location = new Location( ReferenceValue.NEW_OID, 
                                                  ReferenceValue.NEW_VERSION,string.Empty,string.Empty, ns, room, bed );

                string locationResult = locationBroker.ValidateLocation(location, i_ACOFacility );
                Assert.AreEqual(  "Invalid Bed", locationResult, "Did not receive expected location result" );
            }
            catch( Exception ex )
            {
                Assert.Fail( 
                    "TestValidateLocation failed " + ex.Message );
            }
        }

        #endregion


        #region Support Methods
        #endregion

        #region Data Elements

        private static  Facility i_ACOFacility;
        
        #endregion

    }
}