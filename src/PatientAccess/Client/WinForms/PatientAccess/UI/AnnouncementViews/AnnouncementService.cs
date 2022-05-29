using System;
using System.Collections;
using Extensions.SecurityService.Domain;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using Facility = Peradigm.Framework.Domain.Parties.Facility;
using User = PatientAccess.Domain.User;

namespace PatientAccess.UI.AnnouncementViews
{
    [Serializable]
    public class AnnouncementService 
    {
        #region Event Handlers
        #endregion

        #region Methods

        public ICollection GetAllRoles()
        {
            IRoleBroker broker = BrokerFactory.BrokerOfType<IRoleBroker>();
            ICollection roles = broker.AllRoles();
            return roles;
        }


        public bool HasAddEditPrivileges()
        {
            User patientAccessUser = User.GetCurrent();
            Extensions.SecurityService.Domain.User securityUser = patientAccessUser.SecurityUser;

            Facility securityFrameworkFacility = new Facility( patientAccessUser.Facility.Code, patientAccessUser.Facility.Description );
            bool hasAddPermission  = securityUser.HasPermissionTo( Privilege.Actions.Add, typeof( Announcement ), securityFrameworkFacility );
            bool hasEditPermission = securityUser.HasPermissionTo( Privilege.Actions.Edit, typeof( Announcement ), securityFrameworkFacility );

            return ( hasAddPermission && hasEditPermission );
        }


        public ICollection GetAllAnnouncementsFor( ICollection facilitiesToManage )
        {
            IAnnouncementBroker broker = BrokerFactory.BrokerOfType<IAnnouncementBroker>();
            ICollection announcements = broker.AllAnnouncementsFor( facilitiesToManage );
            return announcements;
        }


        public ICollection GetFacilitiesToManage()
        {
            ArrayList facilities = new ArrayList();
            User patientAccessUser = User.GetCurrent();
            Extensions.SecurityService.Domain.User securityUser = patientAccessUser.SecurityUser;
            ICollection securityFrameworkFacilities = securityUser.AllFacilities();
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            foreach( Facility securityFrameworkFacility in securityFrameworkFacilities )
            {
                bool hasAddPermission  = securityUser.HasPermissionTo( Privilege.Actions.Add, typeof( Announcement ), securityFrameworkFacility );
                bool hasEditPermission = securityUser.HasPermissionTo( Privilege.Actions.Edit, typeof( Announcement ), securityFrameworkFacility );
                if( hasAddPermission && hasEditPermission )
                {
                    Domain.Facility facility = facilityBroker.FacilityWith( securityFrameworkFacility.Code );
                    facilities.Add( facility );
                }
            }
            return facilities;
        }


        public ICollection GetCurrentAnnouncements()
        {
            ICollection announcments = null;
            ICollection facilityRoles = null;

            User user = User.GetCurrent();
            Domain.Facility currentFacility = user.Facility;
            Facility securityFrameworkFacility = new Facility( currentFacility.Code, currentFacility.Description );
            facilityRoles = user.SecurityUser.RolesFor( securityFrameworkFacility );

            IAnnouncementBroker abroker = BrokerFactory.BrokerOfType<IAnnouncementBroker>();
            announcments  = abroker.CurrentAnnouncementsFor( facilityRoles, currentFacility );

            return announcments;
        }


        public void DisplaySelectedAnnouncement( Announcement announcement )
        {
            this.AnnouncementDetailsView.Model = announcement;
            this.AnnouncementDetailsView.UpdateView();
        }


        public void DisplayAnnouncmentForEdit( Announcement announcement )
        {
            this.AddEditAnnouncementView.Model = announcement;
            this.AddEditAnnouncementView.UpdateView();
        }


        public DateTime GetCurrentFacilityTime()
        {
            User currentUser = User.GetCurrent();
            Domain.Facility currentFacility = currentUser.Facility;
            ITimeBroker broker = ProxyFactory.GetTimeBroker();
            DateTime facilityDate = broker.TimeAt( User.GetCurrent().Facility.GMTOffset, 
                                                   User.GetCurrent().Facility.DSTOffset );
            return facilityDate;
        }


        public void Save( Announcement announcementToSave )
        {
            DateTime currentFacilityTime = this.GetCurrentFacilityTime();
            announcementToSave.SaveDate = currentFacilityTime;
            IAnnouncementBroker broker =
                BrokerFactory.BrokerOfType<IAnnouncementBroker>();
            broker.SaveAnnouncement( announcementToSave );
            this.AddEditAnnouncementsView.UpdateView();
        }


        public string GetCurrentUserName()
        {
           return User.GetCurrent().FormattedName;
        }


        public void ResetControls()
        {
           this.AddEditAnnouncementsView.ResetControls();
        }

        #endregion

        #region Properties
        public AnnouncementDetailsView AnnouncementDetailsView
        {
            private get
            {
                return i_AnnouncementDetailsView;
            }
            set
            {
                i_AnnouncementDetailsView = value;
            }
        }

        public AddEditAnnouncementsView AddEditAnnouncementsView
        {
            private get
            {
                return i_AddEditAnnouncementsView;
            }
            set
            {
                i_AddEditAnnouncementsView = value;
            }
        }

        public AddEditAnnouncementView AddEditAnnouncementView
        {
            private get
            {
                return i_AddEditAnnouncementView;
            }
            set
            {
                i_AddEditAnnouncementView = value;
            }
        }   

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AnnouncementService()
        {
        }
        #endregion

        #region Data Elements
        private AnnouncementDetailsView i_AnnouncementDetailsView;
        private AddEditAnnouncementView i_AddEditAnnouncementView;
        private AddEditAnnouncementsView i_AddEditAnnouncementsView;

        #endregion

        #region Constants
        #endregion
    }
}
