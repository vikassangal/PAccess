using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for GuarentorValidationMustBeViewed.
	/// </summary>
	//TODO: Create XML summary comment for GuarentorValidationMustBeViewed
    [Serializable]
    [UsedImplicitly]
    public class GuarentorValidationMustBeViewed : LeafRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
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
            if( context.GetType() != typeof( Account ) )
            {
                return true;
            }
            Account anAccount = context as Account;
            if( anAccount == null )
            {
                return true;
            }
            Guarantor aGuarantor = anAccount.Guarantor;
            if( aGuarantor == null )
            {
                return true;
            }

            if( anAccount.FinancialClass == null
                || !anAccount.FinancialClass.WarrantsValidation() )
            {
                return true;
            }

            if( aGuarantor.DataValidationTicket != null &&
                aGuarantor.DataValidationTicket.ResultsAvailable == true &&
                aGuarantor.DataValidationTicket.ResultsReviewed == false )
            {
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
        public GuarentorValidationMustBeViewed()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
