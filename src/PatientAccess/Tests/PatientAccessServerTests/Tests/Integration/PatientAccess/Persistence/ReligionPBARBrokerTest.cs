using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class ReligionPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
     
        private const string REL_REL_CODE_ONE           = "BP";
        private const string REL_REL_CODE_ALL           = "ALL";
        private const string REL_REL_CODE_UNSPECIFIED   = "UNSPECIFIED";
        
        #endregion

        #region SetUp and TearDown ReligionBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpReligionBrokerTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_ACOFacility = facilityBroker.FacilityWith( ACO_FACILITYID );

            i_ReligionBroker = BrokerFactory.BrokerOfType<IReligionBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownReligionBrokerTests()
        {
          
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestRelSummCountForAll()
        {   
            
            ArrayList religionProxies;

            religionProxies = ( ArrayList )
                              this.ReligionBroker.ReligionSummaryFor( i_ACOFacility,
                                                                 REL_REL_CODE_ALL                         
                                  );

            Assert.IsNotNull( religionProxies, "Did not find any Accounts" );
                
        }

        [Test()]
        public void TestRelSummCountForUnspc()
        {   
            
            ArrayList religionProxies;

            religionProxies = ( ArrayList )
                              this.ReligionBroker.ReligionSummaryFor( i_ACOFacility, 
                                                                 REL_REL_CODE_UNSPECIFIED                           
                                  );

            Assert.IsNotNull( religionProxies, "Did not find any Accounts" );
                
        }

        [Test()]
        public void TestRelSummCountForOne()
        {   
            ArrayList religionProxies;

            religionProxies = ( ArrayList )
                              this.ReligionBroker.ReligionSummaryFor( i_ACOFacility,
                                                                 REL_REL_CODE_ONE );

            Assert.IsNotNull( religionProxies, "Did not find any Accounts" );
        }

        [Test()]
        public void AllPlacesOfWorship_CountShouldBeGreaterThanZero()
        {
            ArrayList placesOfWorship = (ArrayList) this.ReligionBroker.AllPlacesOfWorshipFor(98);

            Assert.IsTrue(placesOfWorship.Count > 0, "Did not find Religion Codes");
        }

        [Test()]
        public void testInValidSpecificPlaceOrWorship()
        {
            PlaceOfWorship placeOfWorship = this.ReligionBroker.PlaceOfWorshipWith(98,"SMC");
            Assert.IsFalse(placeOfWorship.IsValid, "Did not find SMC PARISH CODE for Facility 98");
        }
        [Test()]
        public void testValidSpecificPlaceOrWorship()
        {
            PlaceOfWorship placeOfWorship = this.ReligionBroker.PlaceOfWorshipWith(98, "JAD");
            Assert.IsTrue(placeOfWorship.IsValid, "Did not find JAD  CODE for Facility 98");
        }
        [Test()]
        //        [Ignore("OID Test - SKIP")]
        public void TestPlaceOrWorshipForBlank()
        {            
            string blank = String.Empty;
            PlaceOfWorship pow  = this.ReligionBroker.PlaceOfWorshipWith(ACO_FACILITYID,blank);

            Assert.AreEqual
                (blank,
                 pow.Code,
                 "code should be blank"
                );
            Assert.IsTrue(
                pow.IsValid            
                );
        }
        [Ignore()] // Oid test
        public void TestReligionForBlank()
        {            
            string blank = String.Empty ;
            Religion religion  = this.ReligionBroker.ReligionWith(ACO_FACILITYID,blank);

            Assert.AreEqual
                (blank,
                 religion.Code,
                 "code should be blank"
                );
            Assert.IsTrue(
                religion.IsValid            
                );
        }
        [Test()]
        public void TestSpecificReligion()
        {
            Religion religion = this.ReligionBroker.ReligionWith(ACO_FACILITYID, "CA");

            Assert.IsTrue(religion.IsValid,"Did not find Catholic Religion");
            Assert.AreEqual("CATHOLIC",religion.Description,
                            "Description incorrect for religion, Should be CATHOLIC");
        }

        [Test()]
        public void TestInvalidReligion()
        {
            Religion religion = this.ReligionBroker.ReligionWith(ACO_FACILITYID, "N0");

            Assert.IsFalse(religion.IsValid,"Should not have found Religion");
            
        }

        [Test()]
        public void AllReligions()
        {            
            ArrayList religions = (ArrayList)this.ReligionBroker.AllReligions(ACO_FACILITYID);

            Religion adventist = null;
            Religion assemblyOfGod = null;

            foreach( Religion rel in religions )
            {
                switch( rel.Code )
                {
                    case "AG":
                        assemblyOfGod = rel;
                        break;
                    case "AD":
                        adventist = rel;
                        break;
                    default:
                        break;
                }
            }
            Assert.IsNotNull(adventist,"Did not find Adventist Religion");
            Assert.AreEqual(
                "ADVENTIST",
                adventist.Description,
                "First entry should be ADVENTIST"
                );
            Assert.IsNotNull(assemblyOfGod,"Did not find ASSEMBLY OF GOD Religion");
            Assert.AreEqual(
                "ASSEMBLY OF GOD",
                assemblyOfGod.Description,
                "Second entry should be ASSEMBLY OF GOD"
                );
        }
        #endregion

        #region Support Methods

        private IReligionBroker ReligionBroker
        {
            get
            {
                return i_ReligionBroker;
            }
            set
            {
                i_ReligionBroker = value;
            }
        }
        #endregion

        #region Data Elements
       
        private static  IReligionBroker i_ReligionBroker;
        private static  Facility i_ACOFacility;

        #endregion

    }
}