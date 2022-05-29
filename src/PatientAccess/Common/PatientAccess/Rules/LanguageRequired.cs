using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for LanguageRequired.
    /// </summary>
    //TODO: Create XML summary comment for LanguageRequired
    [Serializable]
    [UsedImplicitly]
    public class LanguageRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler LanguageRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            LanguageRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            LanguageRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.LanguageRequiredEvent = null;    
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
                ( anAccount.Patient.Language == null
                    || anAccount.Patient.Language.Code.Trim() == string.Empty ))
            {
                if( this.FireEvents && LanguageRequiredEvent != null )
                {
                    LanguageRequiredEvent( this, null );
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
        public LanguageRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

