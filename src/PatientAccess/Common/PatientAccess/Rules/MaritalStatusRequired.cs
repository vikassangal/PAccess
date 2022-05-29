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
    public class MaritalStatusRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler MaritalStatusRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MaritalStatusRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MaritalStatusRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MaritalStatusRequiredEvent = null;   
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
                if( this.FireEvents && MaritalStatusRequiredEvent != null )
                {
                    MaritalStatusRequiredEvent( this, null );
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
        public MaritalStatusRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

