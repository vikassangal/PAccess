using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerPatientSSN.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerPatientSSN : LeafRule
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
            this.BenVerPatientSSNEvent   = null;  
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
            
            if( this.AssociatedControl == null  ||
                ((InsurancePlanCategory)AssociatedControl).Oid != InsurancePlanCategory.PLANCATEGORY_SELF_PAY )
            {
                return true;
            }            
            
            Account anAccount = (Account)context;

            if( anAccount == null )
            {
                return false;
            }

            if( anAccount.Patient == null
                || anAccount.Patient.SocialSecurityNumber == null
                || anAccount.Patient.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim() == string.Empty)
            {
                if( this.FireEvents && BenVerPatientSSNEvent != null )
                {
                    BenVerPatientSSNEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
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
        public BenVerPatientSSN()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
