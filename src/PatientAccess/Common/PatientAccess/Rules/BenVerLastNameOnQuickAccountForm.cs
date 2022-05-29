using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Rule for address City
    /// </summary>
	
    [Serializable]
    [UsedImplicitly]
    public class BenVerLastNameOnQuickAccountForm : BenVerOnQuickAccountCreationForm 
    {
          #region Events
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	

            if( AssociatedControl == null )
            {
                return true;
            }
            
            Account anAccount = (Account)context;

            if( anAccount.Patient == null
                || anAccount.Patient.LastName == null
                || anAccount.Patient.LastName.Trim().Length == 0 )
            {
                return false;
            }
            else
            {
                return true;
            }           
        }

        public override bool ShouldStopProcessing()
        {
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
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
