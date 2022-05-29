using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.AddressViews.Presenters
{
    public class CellPhoneConsentPresenter
    {
        private ICellPhoneConsentBroker CellPhoneConsentBroker { get; set; }

        #region Construction and Finalization

        public CellPhoneConsentPresenter(ICellPhoneConsentView consentView, Account account, ContactPoint contactPoint, ICellPhoneConsentBroker cellPhoneConsentBroker)
        {
            Guard.ThrowIfArgumentIsNull(consentView, "addressView");
            Guard.ThrowIfArgumentIsNull(contactPoint, "contactPoint");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(cellPhoneConsentBroker, "cellPhoneConsentBroker");
 
            ConsentView = consentView;
            ContactPoint = contactPoint;
            Account = account;
            CellPhoneConsentBroker = cellPhoneConsentBroker;
        }

        #endregion

        #region Public Methods
        public void Initialize()
        {
            ConsentView.ClearConsentSelectionValues();
            ReadConsentValues();
            ConsentView.PopulateConsentSelections(ConsentValues);
            EnableOrDisableConsent();
        }

        public void UpdateConsent()
        {
            ContactPoint.CellPhoneConsent = ConsentView.CellPhoneConsent;
            CellPhoneConsentChanged();
        }

        public void CellPhoneConsentChanged()
        {
            ConsentView.CellPhoneConsentUpdated();
            ValidateConsent();
        }

        public void CellPhoneNumberChanged()
        {
            ConsentView.CellPhoneNumberUpdated();
        }

        public void EnableOrDisableConsent()
        {             
            var guarantorCellPhoneConsentFeatureManager = new GuarantorCellPhoneConsentFeatureManager();
            var featureIsEnabledToDefaultForCOSSigned = guarantorCellPhoneConsentFeatureManager.IsCellPhoneConsentRuleForCOSEnabledforaccount(Account);
            if (!featureIsEnabledToDefaultForCOSSigned)
             {
                ConsentView.Disable();
                if (ShouldFeatureBeEnabled)
                {
                    ConsentView.Enable();
                }
             }
            else
            {
                ConsentView.Enable();
            }
           ValidateConsent();
        }

        public void ValidateConsent()
        {
            ConsentView.SetCellPhoneConsentNormal();
            RuleEngine.GetInstance().EvaluateRule(typeof(GuarantorConsentRequired), Account); 
            RuleEngine.GetInstance().OneShotRuleEvaluation<GuarantorConsentPreferred>(Account.Guarantor, ConsentView.GuarantorConsentPreferredEventHandler);
        }

        #endregion

        #region Private Methods

        private void ReadConsentValues()
        {
            ConsentValues = CellPhoneConsentBroker.AllCellPhoneConsents();
        }
       
        private bool ShouldFeatureBeEnabled
        {
            get
            {
                return ConsentView.MobileAreaCodeLength + ConsentView.MobilePhoneNumberLength == 10;
            }
        }
        #endregion

        #region Data Elements

        public ICellPhoneConsentView ConsentView { get; set; }
        private ContactPoint ContactPoint { get; set; }
        private Account Account { get; set; }
        private ICollection<CellPhoneConsent> ConsentValues { get; set; }

        #endregion
    }
}