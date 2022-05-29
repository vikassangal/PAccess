using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RemoteRoleBrokerTests
    {
        #region Constants
        
        const int TOTAL_NUMBER_OF_ROLES = 10;
        
        #endregion

        [Test()]
        public void TestRoleBrokerTestRemote()
        {
            IRoleBroker roleBroker = BrokerFactory.BrokerOfType<IRoleBroker>();
            Assert.IsNotNull( roleBroker, "Could not create remote RoleBrokerTest" );
            Hashtable allRoles = roleBroker.AllRoles();
            Assert.IsNotNull( allRoles, "No Roles returned" );
            Assert.AreEqual( TOTAL_NUMBER_OF_ROLES, allRoles.Count, "Incorrect number of Roles returned" );
        }
    }
}