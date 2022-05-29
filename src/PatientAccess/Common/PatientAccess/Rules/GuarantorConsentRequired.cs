using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for GuarantorConsentRequired.
	/// </summary>
	[Serializable]
	[UsedImplicitly]
    public class GuarantorConsentRequired : LeafRule
    {
        # region Event Handlers
        public event EventHandler GuarantorConsentRequiredEvent;
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            GuarantorConsentRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            GuarantorConsentRequiredEvent -= eventHandler;
            return true;
        }
               
        public override void UnregisterHandlers()
        {
            GuarantorConsentRequiredEvent = null;            
        }

	    public override bool CanBeAppliedTo(object context)
	    {
            if (context == null || context.GetType() != typeof(Account)
                           && context.GetType().BaseType != typeof(Account))
            {
                return true;
            }

            var account = context as Account;
            var guarantor = account.Guarantor;
            if (guarantor != null && guarantor.ContactPoints != null)
	        {
                ContactPoint contactPoint = guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
                var guarantorCellPhoneConsentFeatureManager = new GuarantorCellPhoneConsentFeatureManager();
                var featureIsEnabledToDefaultForCOSSigned =
                    guarantorCellPhoneConsentFeatureManager.IsCellPhoneConsentRuleForCOSEnabledforaccount(account);
                if (!featureIsEnabledToDefaultForCOSSigned &&
                    (account.Activity.IsPreRegistrationActivity() || account.IsPreRegisteredMaintenanceAccount))
                {
                    return true;
                }

                if (!featureIsEnabledToDefaultForCOSSigned)
                {
                    if (contactPoint.PhoneNumber == null ||
                        (contactPoint.PhoneNumber.AreaCode == string.Empty ||
                         contactPoint.PhoneNumber.Number == string.Empty))
                    {
                        return true;
                    }
                }

                if (contactPoint.CellPhoneConsent != null &&
                    contactPoint.CellPhoneConsent.Code != string.Empty &&
                    contactPoint.CellPhoneConsent.Code.Trim() != string.Empty)
                {
                    return true;
                }

                if (FireEvents && GuarantorConsentRequiredEvent != null)
                {
                    GuarantorConsentRequiredEvent(this, null);
                }

                return false;
	        }

	        return true;
	    }

	    public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion

        #region Properties
        public Account PatientAccount
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        private Account i_Account;
        #endregion

        #region Constants
        #endregion

    }
}
