using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class OriginBrokerPBARTests : AbstractBrokerTests
    {
        #region Constants
        #endregion        

        #region SetUp and TearDown OriginBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpOriginBrokerTests()
        {
            i_OriginBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownOriginBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void AllRaces()
        {
            Race asianPacific   = null;
            Race black          = null;
            Race nativeAmerican = null;
            var races = i_OriginBroker.AllRaces( ACO_FACILITYID );

            foreach( Race race  in  races )
            {
                switch( race.Code.Trim() )
                {
                    case "3":
                        asianPacific = race;
                        break;
                    case "2":
                        black = race;
                        break;
                    case "4":
                        nativeAmerican = race;
                        break;
                    default:
                        break;
                }
            }
            Assert.IsNotNull(asianPacific,"Did not find Asian Pacific race");
            Assert.AreEqual(
                "PACIFC ISL",
                asianPacific.Description,
                "Description wrong for PACIFC ISL race"
                );
            Assert.IsNotNull(asianPacific,"Did not find black race");
            Assert.AreEqual(
                "BLACK",
                black.Description,
                "Description wrong for BLACK race"
                );
            Assert.IsNotNull(asianPacific,"Did not find native American race");
            Assert.AreEqual(
                "NATIV AMER",
                nativeAmerican.Description,
                "Description wrong for NATIV AMER race"
                ); 
        }

        [Test()]
        public void TestFindRace()
        {
            Race race = i_OriginBroker.RaceWith( ACO_FACILITYID, "3" );
            Assert.IsNotNull( race, "Did not find Race with code of 3" );
            Assert.AreEqual( race.Description, "PACIFC ISL", "Incorrect Description found" );

            race = i_OriginBroker.RaceWith( ACO_FACILITYID, "Z" );
            Assert.IsFalse( race.IsValid,"Should not have found race with code of Z" );
        }

        [Test()]
        public void TestFindNationality()
        {
            Race race = i_OriginBroker.RaceWith(ACO_FACILITYID, "204");
            Assert.IsNotNull(race, "Did not find Nationality with code of 204");
            Assert.AreEqual(race.Description, "BAHAMIAN", "Incorrect Description found");
       
        }
        
        [Test()]
        public void TestFindEthnicity()
        {
            Ethnicity ethnicity = i_OriginBroker.EthnicityWith( ACO_FACILITYID, "1" );
            Assert.IsNotNull(ethnicity,"Hispanic Ethnicity not found");
            Assert.AreEqual("HISPANIC",ethnicity.Description,
                            "Incorrect description found for Hispanic Ethnicity");

            ethnicity = i_OriginBroker.EthnicityWith( ACO_FACILITYID, "ZZ" );
            Assert.IsFalse(ethnicity.IsValid, "Should not have found ethnicity with code of ZZ");
        }

        [Test()]
        public void TestFindDescent()
        {
            Ethnicity ethnicity = i_OriginBroker.EthnicityWith(ACO_FACILITYID, "104");
            Assert.IsNotNull(ethnicity, "SOUTH AMERICAN  Descent not found");
            Assert.AreEqual("SOUTH AMERICAN", ethnicity.Description,
                "Incorrect description found for SOUTH AMERICAN Descent");

            ethnicity = i_OriginBroker.EthnicityWith(ACO_FACILITYID, "ZZ");
            Assert.IsFalse(ethnicity.IsValid, "Should not have found ethnicity with code of ZZ");
        }

        [Test()]
        [Ignore()]
        public void AllEthnicities()
        {            
            bool hispanicFound = false;
            bool nonHispanicFound = false;
            bool unknownFound = false;

            var ethnicities = i_OriginBroker.AllEthnicities( ACO_FACILITYID );

            foreach( Ethnicity ethnicity in ethnicities )
            {
                switch( ethnicity.Description )
                {
                    case "HISPANIC":
                        Assert.AreEqual("1",ethnicity.Code.Trim(), "Invalid code returned for Hispanic ethnicity");
                        hispanicFound = true;
                        break;

                    case "NON-HISPANIC":
                        Assert.AreEqual("2",ethnicity.Code.Trim(), "Invalid code returned for non-Hispanic ethnicity");
                        nonHispanicFound = true;
                        break;
                    case "UNKNOWN":
                        Assert.AreEqual("3",ethnicity.Code.Trim(), "Invalid code returned for unknown ethnicity");
                        unknownFound = true;
                        break;
                }
            }
            Assert.IsTrue(
                hispanicFound,
                "HISPANIC entry not found"
                );

            Assert.IsTrue( 
                nonHispanicFound,
                "NON-HISPANIC entry not found"
                );

            Assert.IsTrue( 
                unknownFound,
                "UNKNOWN entry not found"
                );
        }

       
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        
        private static  IOriginBroker i_OriginBroker = null;
        
        #endregion
    }
}