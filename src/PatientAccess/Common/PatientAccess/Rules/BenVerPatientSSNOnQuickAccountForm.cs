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
    public class BenVerPatientSSNOnQuickAccountForm : BenVerOnQuickAccountCreationForm 
    {
        #region Event Handlers
        public event EventHandler BenVerPatientSSNEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BenVerPatientSSNEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BenVerPatientSSNEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            BenVerPatientSSNEvent   = null;  
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
            if( context == null )
            {
                return true;
            }
            
            if( AssociatedControl == null  ||
                ((InsurancePlanCategory)AssociatedControl).Oid != InsurancePlanCategory.PLANCATEGORY_SELF_PAY )
            {
                return true;
            }            
            
            Account anAccount = (Account)context;

            if( anAccount.Patient == null
                || anAccount.Patient.SocialSecurityNumber == null
                || anAccount.Patient.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim() == string.Empty)
            {
                if( FireEvents && BenVerPatientSSNEvent != null )
                {
                    BenVerPatientSSNEvent( this, new PropertyChangedArgs( AssociatedControl ) );
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
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
