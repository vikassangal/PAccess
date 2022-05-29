using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    //TODO: Create XML summary comment for UnknownPatientGender
    [Serializable]
    [UsedImplicitly]
    public class UnknownPatientGender : LeafRule
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
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            Account anAccount = context as Account;
            Gender gender = anAccount.Patient.Sex;
            if( ( gender.Code == Gender.UNKNOWN_CODE) &&    
                (!anAccount.BillHasDropped) 
                )
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
        public UnknownPatientGender()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
