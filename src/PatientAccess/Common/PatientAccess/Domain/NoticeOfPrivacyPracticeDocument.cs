using System;
using System.Diagnostics;
using System.Reflection;
using Extensions;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Class to represent Notice Of Privacy Practice Document.
	/// </summary>
    [Serializable]
    public class NoticeOfPrivacyPracticeDocument : Model
	{

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public DateTime SignedOnDate
        {
            get
            {
                return i_SignedOnDate;
            }
            set
            {
                this.SetAndTrack<DateTime>( ref this.i_SignedOnDate, value, MethodBase.GetCurrentMethod() );
            }
        }

        public SignatureStatus SignatureStatus
        {
            get
            {
                return i_SignatureStatus;
            }
            set
            {

                Debug.Assert( value != null );
                this.SetAndTrack<SignatureStatus>( ref this.i_SignatureStatus, value, MethodBase.GetCurrentMethod() );
                
            }
        }

        public NPPVersion NPPVersion
        {
            get
            {
                return i_NPPVersion;
            }
            set
            {
                i_NPPVersion = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public NoticeOfPrivacyPracticeDocument()
        {
            this.SignatureStatus = new SignatureStatus();
        }

        #endregion

        #region Data Elements        
        private NPPVersion i_NPPVersion = new NPPVersion();
        private DateTime i_SignedOnDate;
        private SignatureStatus i_SignatureStatus;
        #endregion

        #region Constants
        #endregion
	}
}