using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for Announcement.
	/// </summary>
    [Serializable]
    public class Announcement : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties

        public long AnnouncementID
        {
            get
            {
                return i_AnnouncementID;
            }
            private set
            {
                i_AnnouncementID = value;
            }
        }

        public DateTime StopDate
        {
            get
            {
                return i_StopDate;
            }
            set
            {
                i_StopDate = value;
            }
        }

        public IList Roles
        {
            get
            {
                return i_Roles;
            }
            set
            {
                i_Roles = value;
            }
        }

        public IList Facilities
        {
            get
            {
                return i_Facilities;
            }
            set
            {
                i_Facilities = value;
            }
        }

        public string Description
        {
            get
            {
                return i_Description;
            }
            set
            {
                i_Description = value;
            }
        }

        public string Author
        {
            get
            {
                return i_Author;
            }
            set
            {
                i_Author = value;
            }
        }

        public DateTime SaveDate
        {
            get
            {
                return i_SaveDate;
            }
            set
            {
                i_SaveDate = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Announcement()
        {
            this.Roles = new ArrayList();
            this.Facilities = new ArrayList();
        }

        public Announcement( long announcementID, DateTime stopDate, string description,
            string author, DateTime saveDate )
        {
            this.AnnouncementID = announcementID;
            this.StopDate = stopDate;
            this.Description = description;
            this.Author = author;
            this.SaveDate = saveDate;
            this.Roles = new ArrayList();
            this.Facilities = new ArrayList();
        }

        public Announcement(long announcementID, DateTime stopDate, IList roles,
            IList facilities, string description, string author, DateTime saveDate )
        {
            this.AnnouncementID = announcementID;
            this.StopDate = stopDate;
            this.Roles = roles;
            this.Facilities = facilities;
            this.Description = description;
            this.Author = author;
            this.SaveDate = saveDate;
        }

        #endregion

        #region Data Elements
        private IList i_Facilities;
        private string i_Description;
        private string i_Author;
        private DateTime i_SaveDate;
        private IList i_Roles;
        private DateTime i_StopDate;
        private long i_AnnouncementID;
        #endregion

        #region Constants
        #endregion
	}
}