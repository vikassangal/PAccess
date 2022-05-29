using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PatientRetiredDateRequired.
    /// </summary>
    //TODO: Create XML summary comment for PatientRetiredDateRequired
    [Serializable]
    [UsedImplicitly]
    public class PatientRetiredDateRequired : PostRegRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            return true;
        }
        public override bool ShouldStopProcessing()
        {
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if(context.GetType() != typeof(Account))
            {
                throw new ArgumentException( "Context in the rule is not an Account" );
            }
            Account anAccount = context as Account;
            if(base.CanBeAppliedTo(context) == true &&
                anAccount.Patient.Employment.RetiredDate== DateTime.MinValue)
            {
                return true ;
            }
            else
            {
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
        public PatientRetiredDateRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

