using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;
using PatientAccess.Persistence.Utilities;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for OccurrenceCodePBARBrokerTests.
    /// </summary>
    [TestFixture()]
    public class OccurrenceCodePBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        const string OCCURRENCECODE_RETURNED_MESSAGE = "No OccurrenceCodes Returned";
        const string OCCURRENCECODE_NOTFOUND_MESSAGE = "OccurrenceCode with code of 18 not found";
        const string WRONG_OCCURRENCECODE__MESSAGE = "Wrong OccurrenceCode for accidentType";
        private const string DEL_FAC_CODE = "DEL";
        #endregion
        
        #region SetUp and TearDown OccurrenceCodePBARBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpOccurrenceCodePBARBrokerTests()
        {
            i_broker = BrokerFactory.BrokerOfType<IOccuranceCodeBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownOccurrenceCodePBARBrokerTests()
        {
             
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestOccurrenceCodes()
        {
            ArrayList list = (ArrayList)i_broker.AllOccurrenceCodes(ACO_FACILITYID);
            Assert.IsNotNull( list, OCCURRENCECODE_RETURNED_MESSAGE );
        }

        [Test()]
        public void TestSelectableOccurrenceCodes()
        {
            ArrayList list = (ArrayList)i_broker.AllSelectableOccurrenceCodes(ACO_FACILITYID);
            Assert.IsNotNull( list, OCCURRENCECODE_RETURNED_MESSAGE );
        }       

        [Test()]
        public void TestSpecificOccurrenceCode()
        {
            OccurrenceCode oc = i_broker.OccurrenceCodeWith(ACO_FACILITYID,"18");
            Assert.IsNotNull( oc, OCCURRENCECODE_NOTFOUND_MESSAGE );
            Assert.AreEqual( "DT OF RETIREMENT PT/BENFY",oc.Description,
                             "Invalid Description for OccurrenceCode 18" );
            Assert.IsTrue( oc.IsValid, "Should build an instance with IsValid = false" );
        }

        [Test()]
        public void TestOccurrenceCodeForBlank()
        {
            string blank = string.Empty;
            OccurrenceCode oc = i_broker.OccurrenceCodeWith(ACO_FACILITYID, blank );
           
            Assert.AreEqual( blank, oc.Code, "Code should be blank" );
            Assert.AreEqual( blank, oc.Description, "Description should be blank" );
            Assert.IsTrue( oc.IsValid );
        }

        [Test()]
        public void TestOccurrenceCodeForInvalid()
        {            
            string inValid = "99";
            OccurrenceCode oc = i_broker.OccurrenceCodeWith(ACO_FACILITYID, inValid);
           
          
            Assert.IsFalse(
                oc.IsValid            
                );
        }

        [Test(), ExpectedException( typeof( ArgumentNullException ) )]
        public void TestOccurrenceCodeForNULL()
        {
            string invalid = null;
            OccurrenceCode oc = i_broker.OccurrenceCodeWith( ACO_FACILITYID, invalid );
        }

        [Test()]
        public void TestAccidentTypes()
        {
            ArrayList accidentTypes = (ArrayList)i_broker.GetAccidentTypes(ACO_FACILITYID);
            Assert.AreEqual(
                5,
                accidentTypes.Count,
                "Wrong number of accident Types" );
            foreach( TypeOfAccident typeOfAccident in accidentTypes  )
            {
                if( typeOfAccident.DisplayString == "Auto" )
                {
                    Assert.AreEqual(
                        "01",
                        typeOfAccident.OccurrenceCode.Code,
                        WRONG_OCCURRENCECODE__MESSAGE
                        );
                }
            }
        }
        [Test]
        public void TestAddOccurrenceCodeToAccount()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Account anAccount = new Account { AccountNumber = 4477677 };
            Facility facility = facilityBroker.FacilityWith(DEL_FAC_CODE);
            anAccount.Facility = facility;
            anAccount.KindOfVisit = VisitType.Inpatient;
            anAccount.HospitalService = new HospitalService(0L, PersistentModel.NEW_VERSION, "SNF", HospitalService.SNF_EXTENDED_CARE);
           
            string occCode1 = "01";
            long occCodeDate1 = 10213;

            string occCode2 = "11";
            long occCodeDate2 = 10313;

            string occCode3 = "50";
            long occCodeDate3 = 10413;

            string occCode4 = "50";
            long occCodeDate4 = 10513;

            var occCodeAdded = i_broker.CreateOccurrenceCode(anAccount.Facility.Oid,occCode1, occCodeDate1);
            anAccount.AddOccurrenceCode(occCodeAdded);
            OccurrenceCode occCode1FromAccount = (OccurrenceCode)anAccount.OccurrenceCodes[0];
            DateTime occDate1 = DateTimeUtilities.DateTimeFromPacked(occCodeDate1);
            Assert.IsTrue(occCode1FromAccount.Code == occCode1, "OccurrenceCode 01 is not Added");
            Assert.IsTrue(occCode1FromAccount.OccurrenceDate == occDate1, "OccurrenceDate1(01) is not Added");
            Assert.IsTrue(occCode1FromAccount.IsAccidentCrimeOccurrenceCode());

            occCodeAdded = i_broker.CreateOccurrenceCode(anAccount.Facility.Oid, occCode2, occCodeDate2);
            anAccount.AddOccurrenceCode(occCodeAdded);
            OccurrenceCode occCode2FromAccount = (OccurrenceCode)anAccount.OccurrenceCodes[1];
            DateTime occDate2 = DateTimeUtilities.DateTimeFromPacked(occCodeDate2);
            Assert.IsTrue(occCode2FromAccount.Code == occCode2, "OccurrenceCode 11 is not Added");
            Assert.IsTrue(occCode2FromAccount.OccurrenceDate == occDate2, "OccurrenceDate2 is not Added");
            Assert.IsFalse(occCode2FromAccount.IsAccidentCrimeOccurrenceCode());

            occCodeAdded = i_broker.CreateOccurrenceCode(anAccount.Facility.Oid, occCode3, occCodeDate3);
            anAccount.AddOccurrenceCode(occCodeAdded);
            OccurrenceCode occCode3FromAccount = (OccurrenceCode)anAccount.OccurrenceCodes[2];
            DateTime occDate3 = DateTimeUtilities.DateTimeFromPacked(occCodeDate3);
            Assert.IsTrue(occCode3FromAccount.Code == occCode3, "OccurrenceCode 50 is not Added");
            Assert.IsTrue(occCode3FromAccount.OccurrenceDate == occDate3, "OccurrenceDate3 is not Added");
            Assert.IsFalse(occCode3FromAccount.IsAccidentCrimeOccurrenceCode());

            occCodeAdded = i_broker.CreateOccurrenceCode(anAccount.Facility.Oid, occCode4, occCodeDate4);
            anAccount.AddOccurrenceCode(occCodeAdded);
            OccurrenceCode occCode4FromAccount = (OccurrenceCode)anAccount.OccurrenceCodes[3];
            DateTime occDate4 = DateTimeUtilities.DateTimeFromPacked(occCodeDate4);
            Assert.IsTrue(occCode4FromAccount.Code == occCode4, "Second OccurrenceCode 50 is not Added");
            Assert.IsTrue(occCode4FromAccount.OccurrenceDate == occDate4, "OccurrenceDate4 is not Added");


        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        
        private static IOccuranceCodeBroker i_broker = null;
        
        #endregion
    }
}