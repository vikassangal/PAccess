using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidNPPDateSigned.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class NPPSignedOnDateInvalid : LeafRule
    {
        #region Events

        public event EventHandler NPPSignedOnDateInvalidEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            this.NPPSignedOnDateInvalidEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            this.NPPSignedOnDateInvalidEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.NPPSignedOnDateInvalidEvent = null;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || !(context is Account) )
            {
                return true;
            }
            Account account = (Account)context;

            if( account != null  &&
                account.Patient != null &&
                account.Patient.NoticeOfPrivacyPracticeDocument != null &&
                account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion != null &&
                account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate != DateTime.MinValue &&
                account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate < 
                  account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion.NPPDate )
            {
                if( this.FireEvents && NPPSignedOnDateInvalidEvent != null )
                {
                    this.NPPSignedOnDateInvalidEvent( this, null );
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        public override void ApplyTo( object context )
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public NPPSignedOnDateInvalid()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


