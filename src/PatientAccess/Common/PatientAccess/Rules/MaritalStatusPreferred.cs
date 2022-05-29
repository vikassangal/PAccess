using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MaritalStatusRequired.
    /// </summary>
    //TODO: Create XML summary comment for MaritalStatusRequired
    [Serializable]
    [UsedImplicitly]
    public class MaritalStatusPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MaritalStatusPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            MaritalStatusPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            MaritalStatusPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MaritalStatusPreferredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context.GetType() != typeof( Account ) )
            {
                return true;
            }
            Account anAccount = context as Account;
            if( anAccount == null )
            {
                return false;
            }
            if( anAccount.Patient != null &&
                ( anAccount.Patient.MaritalStatus == null
                    || anAccount.Patient.MaritalStatus.Code.Trim() == string.Empty)
                )
            {
                if( this.FireEvents && MaritalStatusPreferredEvent != null )
                {
                    MaritalStatusPreferredEvent( this, null );
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MaritalStatusPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

