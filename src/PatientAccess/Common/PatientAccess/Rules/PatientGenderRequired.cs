using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    //TODO: Create XML summary comment for PatientGenderRequired
    [Serializable]
    [UsedImplicitly]
    public class PatientGenderRequired : PostRegRule
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
                
        public override void UnregisterHandlers()
        {
            
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
            if(context.GetType() != typeof(Account))
            {
                throw new ArgumentException( "Context in the rule is not an Account" );
            }
            Account anAccount = context as Account;
            Gender gender = anAccount.Patient.Sex;
            if( gender.Code == Gender.UNKNOWN_CODE)        
            {                        
                return false ;
            }
            else
            {
                return true;
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
        public PatientGenderRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
