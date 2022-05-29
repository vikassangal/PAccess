using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls.ServiceCategory.View;
using PatientAccess.Utilities;

namespace PatientAccess.UI.CommonControls.ServiceCategory.Presenter
{
    public class ServiceCategoryPresenter
    {
        #region Events

        #endregion

        #region Fields

        private readonly IRuleEngine _ruleEngine = Rules.RuleEngine.GetInstance();
      
        #endregion Fields

        #region Constructors
        public ServiceCategoryPresenter(IServiceCategoryView view, Account account, IServiceCategoryBroker serviceCategoryBroker, IDOFRFeatureManager dOFRFeatureManager)
        {
            Guard.ThrowIfArgumentIsNull(view, "view");
            Guard.ThrowIfArgumentIsNull(account, "account");
            this.ServiceCategoryView = view;
            this.ServiceCategoryView.Model_Account = account;
            this.ModelAccount = account;
            this.ServiceCategoryView.ServiceCategoryPresenter = this;
            this.ServiceCategoryBroker = serviceCategoryBroker;
            this.DOFRFeatureManager = dOFRFeatureManager;
        }

        #endregion Constructors

        #region Properties

        private IServiceCategoryView ServiceCategoryView { get; set; }
        private IRuleEngine RuleEngine
        {
            get { return _ruleEngine; }
        }
        private Account ModelAccount { get; set; }
        private IServiceCategoryBroker ServiceCategoryBroker { get; set; }
        private IDOFRFeatureManager DOFRFeatureManager { get; set; }
        #endregion Properties

        #region Event Handlers

        #endregion Event Handlers

        #region Public Methods
        public void UpdateView()
        {
            bool IsDOFREnabledForFacility = DOFRFeatureManager.IsDOFREnabledForFacility(this.ModelAccount);
            if (IsDOFREnabledForFacility && !PatientAccess.Rules.DOFRFeatureManager.IsAnyNewBornActivity(this.ModelAccount))
            {
                ShowServiceCategoryView();
                if (ValidateServiceCategory())
                {
                    EnableOrDisableServiceCategory(true);
                    ModelAccount.ServiceCategory = new ClinicServiceCategory();
                    PopulateServiceCategory();
                    RegisterRulesEvents();
                    RunRules();
                }
                else
                    EnableOrDisableServiceCategory(false);
            }
            else
            {
                HideServiceCategoryView();
            }
        }
        public void EnableOrDisableServiceCategory(bool value)
        {
            ServiceCategoryView.EnableServiceCategory = value;
        }
        public void SetSelectedServiceCategory(string value)
        {
            ServiceCategoryView.SelectedServiceCategory = value;
            
        }
        public void ClearServiceCategory()
        {
            ServiceCategoryView.ClearServiceCategory();
        }
        #endregion Public Methods

        #region Private Methods
        private void RegisterRulesEvents()
        {
            _ruleEngine.RegisterEvent(typeof(ServiceCategoryRequired), this.ModelAccount, new EventHandler(ServiceCategoryRequiredEventHandler));
        }

        public void UnRegisterRulesEvents()
        {
            _ruleEngine.UnregisterEvent(typeof(ServiceCategoryRequired), this.ModelAccount, ServiceCategoryRequiredEventHandler);
        }

        public void RunRules()
        {
            ServiceCategoryView.SetNormalBgColor();

            _ruleEngine.EvaluateRule(typeof(ServiceCategoryRequired), this.ModelAccount);
        }

        private void ServiceCategoryRequiredEventHandler(object sender, EventArgs e)
        {
            ServiceCategoryView.MakeRequiredBgColor();
        }

        private void PopulateServiceCategory()
        {
            long facilityId = ModelAccount.Facility.Oid;
            string clincCode = ModelAccount.HospitalClinic.Code;

            var listOfServiceCategory = ServiceCategoryBroker.GetServiceCategoryForClinicCode(facilityId, clincCode);

            if (listOfServiceCategory != null)
            {
                this.ServiceCategoryView.LoadServiceCategory(listOfServiceCategory);
            }
        }

        private bool ValidateServiceCategory()
        {
            return DOFRFeatureManager.IsDOFRServiceCategoryValid(this.ModelAccount);
        }

        private void ShowServiceCategoryView()
        {
            ServiceCategoryView.ShowMe = true;
        }

        private void HideServiceCategoryView()
        {
            ServiceCategoryView.ShowMe = false;
        }

        #endregion Private Methods


    }
}