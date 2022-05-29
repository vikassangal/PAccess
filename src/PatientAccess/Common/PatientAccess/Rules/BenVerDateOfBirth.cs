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
    public class BenVerDateOfBirth : BenVerOnDemographics
    {
        #region Events
        #endregion

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

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	

            if( AssociatedControl == null
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_COMMERCIAL
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_OTHER
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_WORKERS_COMPENSATION
                || ((InsurancePlanCategory)AssociatedControl).Oid == InsurancePlanCategory.PLANCATEGORY_OTHER )
            {
                return true;
            }
            
            Account anAccount = (Account)context;

            if( anAccount == null )
            {
                return false;
            }

            if( anAccount.Patient == null
                || anAccount.Patient.DateOfBirth == DateTime.MinValue )
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
