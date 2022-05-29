using System;
using System.Collections;
using Extensions.SecurityService.Domain;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class AnnouncementBrokerTests : AbstractBrokerTests
    {
        #region Constants
        const long FACILITY_ID_ACO = 900,
                   FACILITY_ID_DEL = 6,
                   ROLE_ID1 = 4,
                   ROLE_ID2 = 5;
        #endregion        

        #region SetUp and TearDown AnnouncementBrokerTests
        [SetUp()]
        public void SetUpAnnouncementBrokerTests()
        {
            facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            announcementBroker = BrokerFactory.BrokerOfType<IAnnouncementBroker>();
            roleBroker = BrokerFactory.BrokerOfType<IRoleBroker>();
        }

        [TearDown()]
        public void TearDownAnnouncementBrokerTests()
        {
            facilityBroker = null;
            announcementBroker = null;
            roleBroker = null;

        }
        #endregion

        #region Test Methods
        [Test()]
        public void AllAnnouncementsFor()
        {
            Facility facility = facilityBroker.FacilityWith( FACILITY_ID_ACO );
            ArrayList facilitiesToManage = new ArrayList();
            facilitiesToManage.Add( facility );
            
            ICollection announcements = announcementBroker.AllAnnouncementsFor( facilitiesToManage );

            foreach( Announcement announcement in announcements )
            {
                foreach( Role role in announcement.Roles )
                {
                    Assert.IsNotNull( role );
                }
                foreach( Facility fac in announcement.Facilities )
                {
                    Assert.IsNotNull( fac );
                }
            }
            Assert.IsNotNull( announcements, "Returns Null until method is implemented" );
        }
        [Test()]
        public void SaveAnnouncement()
        {
            Facility facility = facilityBroker.FacilityWith( FACILITY_ID_ACO );
            
            Announcement announce = new Announcement();
            announce.StopDate = DateTime.Now;
            announce.SaveDate = DateTime.Now;
            announce.Description = "Test Description";
            announce.Author = "User03, Patientaccess ";
            announce.Roles = new ArrayList();
            announce.Facilities = new ArrayList();
            
            Role role = roleBroker.RoleWith( ROLE_ID1 );
            announce.Roles.Add( role );
            role = roleBroker.RoleWith( ROLE_ID2 );
            announce.Roles.Add( role );

            announce.Facilities.Add( facility );
            facility = facilityBroker.FacilityWith( FACILITY_ID_DEL );
            announce.Facilities.Add( facility );

            announcementBroker.SaveAnnouncement( announce );
            Assert.IsTrue( true, "Saved without any exceptions." );

        }

        [Test()]
        public void CurrentAnnouncementsFor()
        {
            ArrayList roleList = new ArrayList();
            Role role1 = roleBroker.RoleWith( ROLE_ID1 );
            Role role2 = roleBroker.RoleWith( ROLE_ID2 );
            roleList.Add( role1 );
            roleList.Add( role2 );

            Facility facility = facilityBroker.FacilityWith( FACILITY_ID_ACO );

            ICollection announcements = announcementBroker.CurrentAnnouncementsFor( roleList, facility );

            foreach( Announcement announcement in announcements )
            {
                foreach( Role role in announcement.Roles )
                {
                    Assert.IsNotNull( role );
                }
                foreach( Facility fac in announcement.Facilities )
                {
                    Assert.IsNotNull( fac );
                }
            }
            Assert.IsNotNull( announcements, "Returns Null until method is implemented" );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private IFacilityBroker facilityBroker;
        private IAnnouncementBroker announcementBroker;
        private IRoleBroker roleBroker;
        #endregion
    }
}