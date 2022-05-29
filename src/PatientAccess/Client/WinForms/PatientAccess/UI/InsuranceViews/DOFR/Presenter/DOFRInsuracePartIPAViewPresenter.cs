using System;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.InsuranceViews.DOFR.View;
using PatientAccess.Utilities;

namespace PatientAccess.UI.InsuranceViews.DOFR.Presenter
{
    public class DOFRInsuracePartIPAViewPresenter
    {
        #region Events

        #endregion

        #region Fields

        private readonly IRuleEngine _ruleEngine = Rules.RuleEngine.GetInstance();

        #endregion Fields

        #region Constructors
        public DOFRInsuracePartIPAViewPresenter(IDOFRInsuracePartIPAView view, Account account, IDOFRFeatureManager dOFRFeatureManager)
        {
            Guard.ThrowIfArgumentIsNull(view, "view");
            Guard.ThrowIfArgumentIsNull(account, "account");

            this.DOFRInsuracePartIPAView = view;
            this.ModelAccount = account;
            this.DOFRInsuracePartIPAView.DOFRInsuracePartIPAViewPresenter = this;
            this.DOFRFeatureManager = dOFRFeatureManager;
        }

        #endregion Constructors

        #region Properties

        private IDOFRInsuracePartIPAView DOFRInsuracePartIPAView { get; set; }
        private IRuleEngine RuleEngine
        {
            get { return _ruleEngine; }
        }
        private Account ModelAccount { get; set; }
 
        private IDOFRFeatureManager DOFRFeatureManager { get; set; }
        private CoverageOrder CoverageOrder { get; set; }

        #endregion Properties

        #region Event Handlers

        #endregion Event Handlers

        #region Public Methods
        public void UpdateView(Coverage coverage, MedicalGroupIPA medicalGroupIPA)
        {
            if (coverage == null) return;

            this.CoverageOrder = coverage.CoverageOrder;
            
            if (IsShowInsurancePlanPartOfIPA())
            {
                ShowInsuracePartIPAView();
                if (ModelAccount.KindOfVisit != null && !string.IsNullOrEmpty(ModelAccount.KindOfVisit.Code))
                {
                    if (ModelAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT || ModelAccount.KindOfVisit.Code == VisitType.NON_PATIENT)
                    {
                        DOFRInsuracePartIPAView.rbYesEnabled = false;
                        DOFRInsuracePartIPAView.rbNoEnabled = false;
                        return;
                    }
                }
                RegisterRulesEvents();
                RunRules();

                if(medicalGroupIPA !=null && !string.IsNullOrEmpty(medicalGroupIPA.Code) && this.ModelAccount.Patient !=null && this.ModelAccount.Patient.MedicalRecordNumber != 0)
                {
                    DOFRInsuracePartIPAView.rbYesChecked = true;
                    DOFRInsuracePartIPAView.rbNoChecked = false;
                }
                else if (coverage != null && ((CommercialCoverage)coverage).IsInsurancePlanPartOfIPA != null)
                {
                    if (((CommercialCoverage)coverage).IsInsurancePlanPartOfIPA == true)
                    {
                        DOFRInsuracePartIPAView.rbYesChecked = true;
                        DOFRInsuracePartIPAView.rbNoChecked = false;
                    }
                    else if (((CommercialCoverage)coverage).IsInsurancePlanPartOfIPA == false)
                    {
                        DOFRInsuracePartIPAView.rbNoChecked = true;
                        DOFRInsuracePartIPAView.rbYesChecked = false;
                    }
                }
               
                if (ValidateRadioOptions())
                {
                    DOFRInsuracePartIPAView.SetNormalBgColor();
                    DOFRInsuracePartIPAView.rbYesChecked = true;
                    DOFRInsuracePartIPAView.rbNoChecked = false;
                    DOFRInsuracePartIPAView.rbYesEnabled = false;
                    DOFRInsuracePartIPAView.rbNoEnabled = false;
                }
            }
            else
            {
                HideInsuracePartIPAView();
            }
        }

        public void ValidateIPACodeForDOFR(MedicalGroupIPA medicalGroupIPA,Coverage coverage)
        {
            if (coverage == null) return;

            if(this.CoverageOrder ==null)
                this.CoverageOrder = coverage.CoverageOrder;

            if (!IsShowInsurancePlanPartOfIPA())
                return;

            if (medicalGroupIPA != null && medicalGroupIPA.Code == "NONE")
            {
                MessageBox.Show("The 'Insurance Plan part of an IPA/Medical Group' field response will be set to 'No' since the IPA code selected is 'None' ", "IPA Selection Alert!",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                DOFRInsuracePartIPAView.rbYesChecked = false;
                DOFRInsuracePartIPAView.rbNoChecked = true;
            }
        }

        public void Reset()
        {
            if (!IsShowInsurancePlanPartOfIPA())
                return;

            if (!ValidateRadioOptions())
            {
                DOFRInsuracePartIPAView.SetNormalBgColor();
                DOFRInsuracePartIPAView.rbYesChecked = false;
                DOFRInsuracePartIPAView.rbNoChecked = false;
            }
        }
        #endregion Public Methods

        #region Private Methods
        private bool IsShowInsurancePlanPartOfIPA()
        {
            bool IsDOFREnabledForFacility = DOFRFeatureManager.IsDOFREnabledForFacility(this.ModelAccount);
            bool IsPrimaryInsuranceCoverage = DOFRFeatureManager.IsPrimaryInsuranceCoverage(this.CoverageOrder);
            bool IsInsurancePlanCommercial = DOFRFeatureManager.IsInsurancePlanCommercial(this.ModelAccount);
            if (IsDOFREnabledForFacility && IsPrimaryInsuranceCoverage && IsInsurancePlanCommercial)
            {
                if (PatientAccess.Rules.DOFRFeatureManager.IsAnyNewBornActivity(this.ModelAccount) || PatientAccess.Rules.DOFRFeatureManager.IsTransferOPToIPActivity(this.ModelAccount))
                    return false;
                return true;
            }
            return false;
        }
        private void RegisterRulesEvents()
        {
            _ruleEngine.RegisterEvent(typeof(DOFRInsurancePartIPARequired), this.ModelAccount, new EventHandler(DOFRInsurancePartIPARequiredEventHandler));
        }

        public void UnRegisterRulesEvents()
        {
            _ruleEngine.UnregisterEvent(typeof(DOFRInsurancePartIPARequired), this.ModelAccount, DOFRInsurancePartIPARequiredEventHandler);
        }

        public void RunRules()
        {
            DOFRInsuracePartIPAView.SetNormalBgColor();
            _ruleEngine.EvaluateRule(typeof(DOFRInsurancePartIPARequired), this.ModelAccount);
        }

        private void DOFRInsurancePartIPARequiredEventHandler(object sender, EventArgs e)
        {
            DOFRInsuracePartIPAView.MakeRequiredBgColor();
        }

        private bool ValidateRadioOptions()
        {
            return DOFRFeatureManager.IsDOFRInsurancePartOfIPAValid(this.ModelAccount);
        }

        private void ShowInsuracePartIPAView()
        {
            DOFRInsuracePartIPAView.ShowMe = true;
        }

        private void HideInsuracePartIPAView()
        {
            DOFRInsuracePartIPAView.ShowMe = false;
        }

        #endregion Private Methods


    }
}