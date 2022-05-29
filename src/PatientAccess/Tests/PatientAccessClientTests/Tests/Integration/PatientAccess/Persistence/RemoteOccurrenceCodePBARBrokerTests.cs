using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RemoteOccurrenceCodePBARBrokerTests : RemoteAbstractBrokerTests
    {
        #region Constants
        
        const string OCCURRENCECODE_RETURNED_MESSAGE = "No OccurrenceCodes Returned";
        
        #endregion

        #region Tests

        [Test()]
        public void TestOccurrenceCodesRemote()
        {
            IOccuranceCodeBroker ocb = BrokerFactory.BrokerOfType<IOccuranceCodeBroker>();
            ArrayList list = (ArrayList)ocb.AllOccurrenceCodes( ACO_FACILITYID );
            Assert.IsNotNull( list, OCCURRENCECODE_RETURNED_MESSAGE );
        }

        #endregion
    }
}