using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{

    //TODO: Create XML summary comment for PatientProxyTests
    [TestFixture]
    [Category( "Fast" )]
    public class PatientProxyTests
    {
        #region Constants
        private const long
            TEST_PATIENT_OID = 2L;               
        #endregion

        #region SetUp and TearDown PatientProxyTests
        [TestFixtureSetUp()]
        public static void SetUpPatientProxyTests()
        {
            i_PatientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownPatientProxyTests()
        {
        }
        #endregion

        #region Test Methods

        #endregion

        #region Support Methods
        public IPatientBroker PatientBroker 
        {
            get
            {
                return i_PatientBroker;
            }
            set
            {
                i_PatientBroker = value;
            }
        }
        #endregion

        #region Data Elements
        private static  IPatientBroker i_PatientBroker;
        #endregion
    }
}
