using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RemoteWorklistBrokerTests
    {
        [Test()]
        public void TestWorklistSettingsRemote()
        {
            IWorklistSettingsBroker wb = BrokerFactory.BrokerOfType<IWorklistSettingsBroker>();

            User u = User.GetCurrent();
            u.Facility = null;
            u.SecurityUser = new Extensions.SecurityService.Domain.User();
            u.SecurityUser.TenetID = 9999999;
            u.Oid = 9999999;

            WorklistSettings ws = wb.PreRegWorklistSettings( u.SecurityUser.TenetID );
            Assert.IsNotNull( ws, "No WorklistSetting found" );
            Assert.AreEqual( "A", ws.BeginningWithLetter, "Wrong StartLetters found" );
            Assert.AreEqual( "Z", ws.EndingWithLetter, "Wrong EndingLetters found" );
            Assert.AreEqual( 3, ws.WorklistSelectionRange.Oid, "Wrong RangeType found" );
            Assert.AreEqual( 3, ws.SortedColumn );
        }
    }
}