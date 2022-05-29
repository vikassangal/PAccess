using System;
using PatientAccess.UI.RegulatoryViews.Views;
using PatientAccess.Domain;
using PatientAccess.Utilities;
using PatientAccess.Rules;


namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class COSSignedHandler : ICOSSignedHandler
    {
        #region Variables and Properties

        public readonly IRegulatoryView RegulatoryView;

        private Account Account { get; set; }
        
        public bool LoadingEditMaintain { get; set; }
        /// <summary>
        /// Prevent CellPhoneConset to be blank while loading account
        /// </summary>
        public bool LoadingModel { get; set; }

        #endregion

        #region Constructors

        public COSSignedHandler(IRegulatoryView regulatoryView, 
            RuleEngine ruleEngine, Account account)
        {
            Guard.ThrowIfArgumentIsNull(regulatoryView, "regulatoryView");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(ruleEngine, "ruleEngine");

            RegulatoryView = regulatoryView;
            Account = account;
            RuleEngine = ruleEngine;
        }

        #endregion

        #region Private Methods

        private RuleEngine RuleEngine { get; set; }

        public void HandleCOSSignedSelected()
        {
            UpdateGuarantorCellPhoneConsent();//COS signed Yes, select Guarantor-->cellphone consent to W-'Written'
                if (Account.COSSigned.IsYes) //COSSigned is Yes for new and existing accounts
                {
                    EnableControls();
                    if (LoadingEditMaintain) //COSSigned is no for new accounts
                    {
                        RegulatoryView.PopulateHospCommunicationOptInValue();
                        RegulatoryView.PopulatePatientPortalOptInValue();
                        LoadingEditMaintain = false;
                    }
                    else
                    {
                        AutoPopulateWithYes();
                        UpdateViewsWithModelData();
                    }

                }
                else if (Account.COSSigned.IsNotAvailable
                         || Account.COSSigned.IsUnable
                         || Account.COSSigned.IsRefused)
                {
                    if (Account.Patient.MedicalRecordNumber == 0) //COSSigned is no for new accounts
                    {
                        RegulatoryView.SetEmailReasonToPatientDeclined();
                        AutoPopulateWithNo();
                        UpdateViewsWithModelData();
                        DisableControls();
                    }
                    else if (Account.Patient.MedicalRecordNumber != 0) //COSSigned is no for existing accounts
                    {
                        if (LoadingEditMaintain)
                        {
                            RegulatoryView.PopulateHospCommunicationOptInValue();
                            RegulatoryView.PopulatePatientPortalOptInValue();
                            LoadingEditMaintain = false;
                            if (Account.COSSigned.IsRefused)
                            {
                                DisableControls();
                            }
                            else
                            {
                                EnableControls();
                            }
                        }
                        else 
                        {
                            if (Account.COSSigned.IsRefused)
                            {
                                AutoPopulateWithNo();
                                UpdateViewsWithModelData();
                                DisableControls();
                            }
                            else
                            {
                                EnableControls();
                            }
                           
                        }
                     }
                }
                else // COSSigned is blank
                {
                    EnableControls();
                }

                EvaluateEmailAddress();
        }

        private void UpdateGuarantorCellPhoneConsent()
        {
            var guarantorCellPhoneConsentFeatureManager = new GuarantorCellPhoneConsentFeatureManager();
            var featureIsEnabledToDefaultForCOSSigned =
                guarantorCellPhoneConsentFeatureManager.IsCellPhoneConsentRuleForCOSEnabledforaccount(Account);
            if (featureIsEnabledToDefaultForCOSSigned)
           {
               if (Account.COSSigned.Code != string.Empty)
               {
                   var guarantorMobileContactPoint = Account.Guarantor.ContactPointWith(
                       TypeOfContactPoint.NewMobileContactPointType());
                   if (!Account.COSSigned.IsYes)
                   {
                       if (guarantorMobileContactPoint != null &&
                           guarantorMobileContactPoint.CellPhoneConsent != null &&
                           guarantorMobileContactPoint.CellPhoneConsent.Code == CellPhoneConsent.WRITTEN_CONSENT && !LoadingModel)
                       {
                           guarantorMobileContactPoint.CellPhoneConsent.Code = "";
                           guarantorMobileContactPoint.CellPhoneConsent.Description = "";
                           guarantorMobileContactPoint.CellPhoneConsent = new CellPhoneConsent(0, DateTime.Now,
                               String.Empty, String.Empty);
                       }
                   }
                   else if (guarantorMobileContactPoint != null &&
                            guarantorMobileContactPoint.CellPhoneConsent != null &&
                            (guarantorMobileContactPoint.CellPhoneConsent.Code != CellPhoneConsent.DECLINE_CONSENT &&
                             guarantorMobileContactPoint.CellPhoneConsent.Code != CellPhoneConsent.REVOKE_CONSENT))
                   {
                       guarantorMobileContactPoint.CellPhoneConsent = new CellPhoneConsent(0, DateTime.Now,
                           CellPhoneConsent.WRITTEN_CONSENT_DESCRIPTION,
                           CellPhoneConsent.WRITTEN_CONSENT);

                   }
               }
           }
        }
        private void EvaluateEmailAddress()
        {
            if (Account != null)
            {
                if (Account.Patient != null)
                {
                    RegulatoryView.SetEmailAddressAsNormal();
                    RegulatoryView.SetEmailReasonAsNormal();
                    RuleEngine.EvaluateRule(typeof(EmailAddressRequired), Account);
                    RuleEngine.EvaluateRule(typeof(EmailAddressPreferred), Account);
                    RuleEngine.EvaluateRule(typeof(EmailReasonRequired), Account);
                    RuleEngine.EvaluateRule(typeof(EmailReasonPreferred), Account);
                }
            }
        }

        private void UpdateViewsWithModelData()
        {
            RegulatoryView.UpdateHospitalCommunicationView();
            RegulatoryView.UpdatePatientPortalView();
        }

        private void EnableControls()
        {
            RegulatoryView.EnableEmail();
            RegulatoryView.EnableEmailReason();
            RegulatoryView.EnableHospComm();
            RegulatoryView.EnablePatientPortal();
            RegulatoryView.EnableAuthorizeAdditionalPortalUser();
        }

        private void AutoPopulateWithYes()
        {
            RegulatoryView.HospCommOptIn();
            RegulatoryView.PatientPortalOptIn();
        }

        private void AutoPopulateWithNo()
        {
            RegulatoryView.UnSelectAuthorizePatientPortalUser();
            RegulatoryView.HospCommOptOut();
            RegulatoryView.PatientPortalOptOut();
        }

        private void DisableControls()
        {
            RegulatoryView.SetEmailAddressAsNormal();
            RegulatoryView.DisableHospComm();
            RegulatoryView.DisablePatientPortal();
            RegulatoryView.DisableEmailReason();
            RegulatoryView.DisableEmail();
            RegulatoryView.DisableAuthorizeAdditionalPortalUser();
        }

        #endregion
    }
}
