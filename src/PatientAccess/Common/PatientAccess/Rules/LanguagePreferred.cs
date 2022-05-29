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
    public class LanguagePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler LanguagePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            LanguagePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            LanguagePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.LanguagePreferredEvent = null;   
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
                if( this.FireEvents && LanguagePreferredEvent != null )
                {
                    LanguagePreferredEvent( this, null );
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
        public LanguagePreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

