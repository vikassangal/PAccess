using System;
using System.Collections;
using Extensions.SecurityService.Domain;
using PatientAccess.BrokerInterfaces;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for RoleBrokerTest.
    /// </summary>

    //TODO: Create XML summary comment for RoleBrokerTests
    [TestFixture()]
    public class RoleBrokerTest : AbstractBrokerTests
    {
        #region Constants
        const int TOTAL_NUMBER_OF_ROLES = 10;

        #endregion

        #region SetUp and TearDown RoleBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpRoleBrokerTest()
        {
            i_RoleBroker = BrokerFactory.BrokerOfType<IRoleBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownRoleBrokerTest()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestAllRoles()
        {
            Hashtable allRoles = i_RoleBroker.AllRoles();
            Assert.IsNotNull( allRoles, "No Roles returned");
            Assert.AreEqual( TOTAL_NUMBER_OF_ROLES, allRoles.Count, "Incorrect number of Roles returned" );

            foreach( Object o in allRoles.Values)
            {
                Assert.AreEqual(typeof(Role), o.GetType(), "Wrong Type");
            }            
        }
            
        [Test()]
        public void TestRoleWith() 
        {
            Role role1 = i_RoleBroker.RoleWith(1);
            // 1 is oid value for the Role with the name 'SystemAdmin' and with the desc='System Administrator'
            Assert.IsNotNull( role1, "No Role returned for oid=1");
            Assert.IsTrue( role1.Name == "SystemAdmin", "Wrong Role Name returned for oid=1");               
            Assert.IsTrue( role1.Description == "System Administrator", "Wrong Role Description returned for oid=1");               

        }
          
       

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        
        private static IRoleBroker i_RoleBroker;
        
        #endregion
    }
}