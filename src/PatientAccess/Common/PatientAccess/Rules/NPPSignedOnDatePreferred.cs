using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class NPPSignedOnDatePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler NPPSignedOnDatePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            NPPSignedOnDatePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            NPPSignedOnDatePreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            NPPSignedOnDatePreferredEvent = null;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }

            NoticeOfPrivacyPracticeDocument nppDocument = ( (Account)context ).Patient.NoticeOfPrivacyPracticeDocument;
            if ( nppDocument != null
                && nppDocument.NPPVersion != null
                && nppDocument.NPPVersion.Code.Trim() != String.Empty
                && nppDocument.SignatureStatus != null
                && nppDocument.SignatureStatus.IsSignedStatus() )
            {
                if ( nppDocument.SignedOnDate == DateTime.MinValue )
                {
                    if ( FireEvents && NPPSignedOnDatePreferredEvent != null )
                    {
                        NPPSignedOnDatePreferredEvent( this, null );
                    }

                    return false;
                }
                else
                {
                    return true;
                }
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
        public NPPSignedOnDatePreferred()
        {
           
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}