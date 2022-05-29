using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for DemographicsBrokerProxyTest.
    /// </summary>

    [TestFixture()]
    public class DemographicsBrokerProxyTests
    {
        #region Constants

        private const long ACO_FACILITYID = 900;
        #endregion

        #region SetUp and TearDown GenderBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpDemographicsBrokerTests()
        {
            i_Broker = new DemographicsBrokerProxy( );
        }

        [TestFixtureTearDown()]
        public static void TearDownDemographicsBrokerTests()
        {

        }
        #endregion

        #region Test Methods
        [Test()]
        public void AllTypesOfGenders()
        {
            //This is a case where the order of the return is specified.
            ArrayList genders = (ArrayList)i_Broker.AllTypesOfGenders( ACO_FACILITYID );

            Gender male = null;
            Gender female = null;
            Gender unknown = null;

            foreach( Gender gender in genders )
            {
                switch( gender.Code )
                {
                    case "M": 
                        male = gender;
                        break;
                    case "F": 
                        female = gender;
                        break;
                    case "U": 
                        unknown = gender;
                        break;
                    default:
                        break;
                }
            }
            Assert.IsNotNull(female,"Did not find female gender object");
            Assert.AreEqual(
                "FEMALE",
                female.Description,
                "FEMALE description is incorrect"
                );

            Assert.IsNotNull(male,"Did not find male gender object");
            Assert.AreEqual(
                "MALE",
                male.Description,
                "MALE description is incorrect"
                );
            Assert.IsNotNull(unknown,"Did not find unknown gender object");
            Assert.AreEqual(
                "UNKNOWN", 
                unknown.Description,
                "UNKNOWN description is incorrect"
                ); // No, UNKNOW is not a typo... PBAR really does define it as UNKNOW.
        }

        [Test()]
        public void AllMaritalStatuses()
        {

            ArrayList statuses = (ArrayList)i_Broker.AllMaritalStatuses( ACO_FACILITYID );

            MaritalStatus divorced = null;
            MaritalStatus lgSeparat = null;
            MaritalStatus lifePartner = null;

            foreach( MaritalStatus stat in statuses )
            {
                switch( stat.Description.Trim())
                {
                    case "DIVORCED" : 
                        divorced = stat;
                        break;
                    case "LG SEPARAT" : 
                        lgSeparat = stat;
                        break;
                    case "LIFE PARTN" : 
                        lifePartner = stat;
                        break;
                    default:
                        break;
                }
            }
            Assert.IsNotNull( divorced,"DIVORCED Marital Status not found");
            Assert.AreEqual( "D", divorced.Code.Trim(),"Incorrect code for Divorced MaritalStatus");
            Assert.IsNotNull( divorced,"Legaly Separated Marital Status not found");
            Assert.AreEqual( "X", lgSeparat.Code.Trim(),"Incorrect code for Divorced MaritalStatus");
            Assert.IsNotNull( lifePartner,"Life Partner Marital Status not found");
            Assert.AreEqual( "L", lifePartner.Code.Trim(),"Incorrect code for Divorced MaritalStatus");
        }

        [Test()]
        [Ignore()] //"Until Languages table is updated"
        public void AllLanguages()
        {
            ArrayList languages = (ArrayList)i_Broker.AllLanguages( ACO_FACILITYID );
            Language arabic = null;
            Language armenian = null;
            Language conversion = null;

            foreach( Language lang in languages )
            {
                switch( lang.Description )
                {
                    case "ARABIC":
                        arabic = lang;
                        break;
                    case "ARMENIAN":
                        armenian = lang;
                        break;
                    case "CONVERSION LANGUAGE":
                        conversion = lang;
                        break;
                    default:
                        break;
                }
            }
            Assert.IsNotNull(arabic,"Did not find arabic language");
            Assert.AreEqual("AR",arabic.Code,"Aribic Code is not correct");
            Assert.IsNotNull(armenian,"Did not find armenian language");
            Assert.AreEqual("08",armenian.Code,"armenian Code is not correct");
            Assert.IsNotNull(conversion,"Did not find arabic language");
            Assert.AreEqual("ZZ",conversion.Code,"conversion Code is not correct");
        }

        [Test()]
        [Ignore()] //"Until Languages table is updated"
        public void AllLanguages_SortOrder_Test()
        {
            ArrayList languages = (ArrayList) i_Broker.AllLanguages(ACO_FACILITYID);
            Language ENGLISH = null;
            Language SPANISH = null;

            ENGLISH = languages[1] as Language;
            SPANISH = languages[2] as Language;

            Assert.IsNotNull(ENGLISH, "Did not find arabic language");
            Assert.AreEqual("EN", ENGLISH.Code, "English Code is not correct");
            Assert.IsNotNull(SPANISH, "Did not find armenian language");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static DemographicsBrokerProxy i_Broker = null;
        #endregion

    }
}