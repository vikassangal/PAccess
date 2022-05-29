using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.InsuranceViews.DOFR.View;
using PatientAccess.Utilities;

namespace PatientAccess.UI.InsuranceViews.DOFR.Presenter
{
    public class DOFRAidCodePresenter
    {
        #region Events

        #endregion

        #region Fields

        private readonly IRuleEngine _ruleEngine = Rules.RuleEngine.GetInstance();
      
        #endregion Fields

        #region Constructors
        public DOFRAidCodePresenter(IDOFRAidCodeView view, Account account, IAidCodeBroker aidCodeBroker, IDOFRFeatureManager dOFRFeatureManager)
        {
            Guard.ThrowIfArgumentIsNull(view, "view");
            Guard.ThrowIfArgumentIsNull(account, "account");
            this.DOFRAidCodeView = view;
            this.ModelAccount = account;
            this.DOFRAidCodeView.DOFRAidCodePresenter = this;
            this.AidCodeBroker = aidCodeBroker;
            this.DOFRFeatureManager = dOFRFeatureManager;
        }
        #endregion Constructors

        #region Properties

        private IDOFRAidCodeView DOFRAidCodeView { get; set; }
        private IRuleEngine RuleEngine
        {
            get { return _ruleEngine; }
        }
        private Account ModelAccount { get; set; }
        private IAidCodeBroker AidCodeBroker { get; set; }
        private IDOFRFeatureManager DOFRFeatureManager { get; set; }
        private CoverageOrder CoverageOrder { get; set; }
        #endregion Properties

        #region Event Handlers

        #endregion Event Handlers

        #region Public Methods
        public void UpdateView(Coverage coverage)
        {
            if (coverage == null) return;

            SetCoverageOrder(coverage.CoverageOrder);
            
            if (IsShowAidCode())
            {
                    ShowDOFRAidCodeView();
                    PopulateAidCodes();
                    RegisterRulesEvents();
                    RunRules();

                    if (coverage != null
                        && !string.IsNullOrEmpty(((CommercialCoverage)coverage).AidCode)
                        && !string.IsNullOrEmpty(((CommercialCoverage)coverage).AidCodeType))
                    {
                        DOFRAidCodeView.SetSelectedAidCode = ((CommercialCoverage)coverage).AidCode;
                        if (((CommercialCoverage)coverage).AidCodeType == DOFRAPIRequest.Expansion)
                        {
                            DOFRAidCodeView.rbExpansionChecked = true;
                            DOFRAidCodeView.rbNonExpansionChecked = false;
                        }
                        else if (((CommercialCoverage)coverage).AidCodeType == DOFRAPIRequest.NonExpansion)
                        {
                            DOFRAidCodeView.rbNonExpansionChecked = true;
                            DOFRAidCodeView.rbExpansionChecked = false;
                        }
                    }
            }
            else
            {
                HideDOFRAidCodeView();
            }
        }
       
        public void ClearAidCode()
        {
            if (!IsShowAidCode()) return;

            DOFRAidCodeView.ClearAidCodes();
            DOFRAidCodeView.MakeRequiredBgColor();
        }
        private void SetCoverageOrder(CoverageOrder coverageOrder)
        {
            this.CoverageOrder = coverageOrder;
        }
        private bool IsShowAidCode()
        {
            bool IsDOFREnabledForFacility = DOFRFeatureManager.IsDOFREnabledForFacility(this.ModelAccount);
            bool IsPrimaryInsuranceCoverage = DOFRFeatureManager.IsPrimaryInsuranceCoverage(this.CoverageOrder);
            bool IsInsurancePlanCommercial = DOFRFeatureManager.IsInsurancePlanCommercial(this.ModelAccount);
            if (IsDOFREnabledForFacility && IsPrimaryInsuranceCoverage && IsInsurancePlanCommercial)
            {
                if(PatientAccess.Rules.DOFRFeatureManager.IsAnyNewBornActivity(this.ModelAccount) || PatientAccess.Rules.DOFRFeatureManager.IsTransferOPToIPActivity(this.ModelAccount))
                    return false;
                if (DOFRFeatureManager.IsCalOptimaPlanID(this.ModelAccount))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion Public Methods

        #region Private Methods

        private void RegisterRulesEvents()
        {
          _ruleEngine.RegisterEvent(typeof(DOFRAidCodeRequired), this.ModelAccount, new EventHandler(DOFRAidCodeRequiredEventHandler));
        }

        public void UnRegisterRulesEvents()
        {
           _ruleEngine.UnregisterEvent(typeof(DOFRAidCodeRequired), this.ModelAccount, DOFRAidCodeRequiredEventHandler);
        }

        public void RunRules()
        {
            DOFRAidCodeView.SetNormalBgColor();

           _ruleEngine.EvaluateRule(typeof(DOFRAidCodeRequired), this.ModelAccount);
        }

        private void DOFRAidCodeRequiredEventHandler(object sender, EventArgs e)
        {
            DOFRAidCodeView.MakeRequiredBgColor();
        }

        private void PopulateAidCodes()
        {
            int facilityId = (int)ModelAccount.Facility.Oid;

            var listOfAidCodes = AidCodeBroker.GetAidCode(facilityId);

            if (listOfAidCodes != null)
            {
                this.DOFRAidCodeView.LoadAidCodes(listOfAidCodes);
            }
        }

        private void ShowDOFRAidCodeView()
        {
            DOFRAidCodeView.ShowMe = true;
        }

        private void HideDOFRAidCodeView()
        {
            DOFRAidCodeView.ShowMe = false;
        }

        #endregion Private Methods


    }
}