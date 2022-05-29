using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for EmploymentStatusRequired.
    /// </summary>
    //TODO: Create XML summary comment for EmploymentStatusRequired
    [Serializable]
    [UsedImplicitly]
    public class EmploymentStatusRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler EmploymentStatusRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            EmploymentStatusRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            EmploymentStatusRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.EmploymentStatusRequiredEvent = null;   
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
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            Account anAccount = context as Account;

            if( anAccount != null &&
                anAccount.Patient != null &&
                anAccount.Patient.Employment != null &&
                anAccount.Patient.Employment.Status != null &&
                anAccount.Patient.Employment.Status.Code != string.Empty )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && EmploymentStatusRequiredEvent != null )
                {
                    EmploymentStatusRequiredEvent( this, null );
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
        public EmploymentStatusRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

