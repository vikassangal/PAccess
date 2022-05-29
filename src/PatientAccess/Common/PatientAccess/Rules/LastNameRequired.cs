using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for LastNameRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class LastNameRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler LastNameRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            LastNameRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            LastNameRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.LastNameRequiredEvent = null;   
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
            if (context != null && context.GetType() != typeof(Account))
            {
                return true;
            }
            Account anAccount = context as Account;
            if( anAccount == null )
            {
                return false;
            }
            if( anAccount.Patient != null
                && anAccount.Patient.Name != null
                && anAccount.Patient.Name.LastName.Length > 0 )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && LastNameRequiredEvent != null )
                {
                    LastNameRequiredEvent( this, new PropertyChangedArgs(this.AssociatedControl) );
                }
                return false;
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LastNameRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
