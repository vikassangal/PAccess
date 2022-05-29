using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// 
    ///         IList AllOccurrenceCodes();
    ///         IList AllSelectableOccurrenceCodes();
    ///         OccurrenceCode OccurrenceCodeWith( long oid );
    ///         OccurrenceCode OccurrenceCodeWith( string OccCode );
    ///         IList GetAccidentTypes();
    /// 
    /// </summary>
	


    [TestFixture()]
    public class OccurrenceCodeBrokerProxyTests
    {
        #region Constants

        const string OCCURRENCECODE_RETURNED_MESSAGE = "No OccurrenceCodes Returned";
        const string OCCURRENCECODE_NOTFOUND_MESSAGE = "OccurrenceCode with code of 18 not found";
        const string WRONG_OCCURRENCECODE__MESSAGE = "Wrong OccurrenceCode for accidentType";
        private long ACO_FacilityID = 900;

        #endregion

        #region SetUp and TearDown

        [TestFixtureSetUp()]
        public static void SetUpOccurrenceCodeBrokerProxyTests()
        {
            i_BrokerProxy = new OccuranceCodeBrokerProxy();
        }

        [TestFixtureTearDown()]
        public static void TearDownOccurrenceCodeBrokerProxyTests()
        {
            i_BrokerProxy = null;
        }

        #region Data Elements

        private static OccuranceCodeBrokerProxy i_BrokerProxy = null;

        #endregion


        #endregion
		
        #region Tests

        [Test()]
        public void TestAllOccurrenceCodes()
        {
            ArrayList list = (ArrayList)i_BrokerProxy.AllOccurrenceCodes( this.ACO_FacilityID );
            Assert.IsNotNull( list, OCCURRENCECODE_RETURNED_MESSAGE );
        }

        [Test()]
        public void TestSelectableOccurrenceCodes()
        {
            ArrayList list = (ArrayList)i_BrokerProxy.AllSelectableOccurrenceCodes( this.ACO_FacilityID );
            Assert.IsNotNull( list, OCCURRENCECODE_RETURNED_MESSAGE );
        }

        [Test()]
        public void TestSpecificOccurrenceCode()
        {
            OccurrenceCode oc = i_BrokerProxy.OccurrenceCodeWith( this.ACO_FacilityID, "18" );
            Assert.IsNotNull( oc, OCCURRENCECODE_NOTFOUND_MESSAGE );
            Assert.AreEqual( "DT OF RETIREMENT PT/BENFY",oc.Description,
                             "Invalid Description for OccurrenceCode 18" );
            Assert.IsTrue(
                oc.IsValid            
                );
        }

        [Test()]
        public void TestOccurrenceCodeForBlank()
        {            
            string blank = string.Empty;
            OccurrenceCode oc = i_BrokerProxy.OccurrenceCodeWith( this.ACO_FacilityID, blank );
           
            Assert.AreEqual( blank, oc.Code, "Code should be blank" );
            Assert.AreEqual( blank, oc.Description, "Description should be blank" );
            Assert.IsTrue( oc.IsValid );
        }

        [Test()]
        public void TestOccurrenceCodeForInvalid()
        {            
            string inValid = "99";
            OccurrenceCode oc = i_BrokerProxy.OccurrenceCodeWith( this.ACO_FacilityID, inValid );
                     
            Assert.IsFalse(	oc.IsValid 	);
        }

        [Test()]
        [Ignore()] //GOO
        public void TestSpecificOccurrenceID()
        {
            OccurrenceCode oc = i_BrokerProxy.OccurrenceCodeWith( this.ACO_FacilityID, 18 );
            Assert.IsNotNull( oc, OCCURRENCECODE_NOTFOUND_MESSAGE );
            Assert.AreEqual( "ONSET OF SYMPTOMS/ILLNESS",oc.Description, 
                             "Invalid Description for OccurrenceCode ID=18" );
            Assert.AreEqual( "11",oc.Code, "Invalid Vode for Occurance Code ID=18" );

        }

        [Test()]
        public void TestAccidentTypes()
        {
            ArrayList accidentTypes = (ArrayList)i_BrokerProxy.GetAccidentTypes( this.ACO_FacilityID );
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


        #endregion	
    }
}