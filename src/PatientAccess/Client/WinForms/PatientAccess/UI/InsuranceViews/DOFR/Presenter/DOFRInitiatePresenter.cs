using System;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Rules;
using PatientAccess.UI.InsuranceViews.DOFR.View;
using PatientAccess.UI.InsuranceViews.InsuranceVerificationViews;
using PatientAccess.Utilities;
using System.Net;

namespace PatientAccess.UI.InsuranceViews.DOFR.Presenter
{
    public class DOFRInitiatePresenter
    {
        #region Events

        #endregion

        #region Fields

        private readonly IRuleEngine _ruleEngine = Rules.RuleEngine.GetInstance();
        private const string _APISuccessMessage = @"[newline]Plan ID:\b {0}\b0, Plan Name:\b {1}\b0, and Service Category:\b {2} \b0 has the highest probability of being accepted by the payer for this patient encounter. [newline]The decision was based on the current values for Facility, Patient Type (PT), Hospital Service Code (HSV), Plan ID, Financial Class (FC), Clinic Code, Service Category, Expansion/Non-Expansion, and IPA Code. Changes to any of these fields could result in a different analysis response.";
        private const string _APIEmptyMessage = "[newline]Decision Analysis Tool (DAT) was not able to find a recommended Plan ID, Plan Name and Service Category for this patient encounter. [newline]The decision was based on the current values for Facility, Patient Type (PT), Hospital Service Code (HSV), Plan ID, Financial Class (FC), Clinic Code, Service Category, Expansion/Non-Expansion, and IPA Code. Changes to any of these fields could result in a different analysis response.";
        private const string _APITimeoutMessage = "[newline]Request timed out. Please re-initiate DOFR or use the \"DOFR Matrix\" manual and complete the registration.";
        private const string _APIDownOrErrorMessage = "[newline]Not able to retrieve results from DAT since API is down or the API has returned an error message. Please use the \"DOFR Matrix\" manual and complete the registration.";
        #endregion Fields

        #region Constructors
        public DOFRInitiatePresenter(IDOFRInitiateView view, Account account, IDOFRInitiateBroker dOFRInitiateBroker, IDOFRFeatureManager dOFRFeatureManager)
        {
            Guard.ThrowIfArgumentIsNull(view, "view");
            Guard.ThrowIfArgumentIsNull(account, "account");

            this.DOFRInitiateView = view;
            this.ModelAccount = account;
            this.DOFRInitiateView.DOFRInitiatePresenter = this;
            this.DOFRInitiateBroker = dOFRInitiateBroker;
            this.DOFRFeatureManager = dOFRFeatureManager;
        }

        #endregion Constructors

        #region Properties

        private IDOFRInitiateView DOFRInitiateView { get; set; }
        private IRuleEngine RuleEngine
        {
            get { return _ruleEngine; }
        }
        private Account ModelAccount { get; set; }
        private IDOFRInitiateBroker DOFRInitiateBroker { get; set; }
        private IDOFRFeatureManager DOFRFeatureManager { get; set; }

        private DOFRAidCodePresenter DOFRAidCodePresenter{get;set;}
        public string APIEmptyMessage { get{ return _APIEmptyMessage;} }
        public string APITimeoutMessage { get { return _APITimeoutMessage; } }
        public string APIDownOrErrorMessage { get { return _APIDownOrErrorMessage; } }
        #endregion Properties

        #region Event Handlers

        #endregion Event Handlers

        #region Public Methods
        public void UpdateView()
        {
            bool IsDOFREnabledForFacility = DOFRFeatureManager.IsDOFREnabledForFacility(this.ModelAccount);
            if (IsDOFREnabledForFacility && !PatientAccess.Rules.DOFRFeatureManager.IsAnyNewBornActivity(this.ModelAccount) && !PatientAccess.Rules.DOFRFeatureManager.IsTransferOPToIPActivity(this.ModelAccount))
            {
                ShowInitiateView();
                if (ValidateInitiateButton() && DOFRFeatureManager.IsMedicalGroupIPACodeValid(this.ModelAccount) && DOFRFeatureManager.IsDOFRInsurancePlanPartOfIPAValid(this.ModelAccount))
                {
                    SetDOFRInitiated(this.ModelAccount);
                    EnableInitateButton();
                    RegisterRulesEvents();
                    RunRules();
                }
                else
                    DisableInitateButton();
            }
            else
            {
                HideInitiateView();
            }
        }

        public void HandleDOFRInitiateClick()
        {
            DOFRInitiateResponseView dialog = new DOFRInitiateResponseView(this);

            try
            {
                dialog.ShowDialog();
             

                if (dialog.DialogResult == DialogResult.OK)
                {
                    
                }
                else // Cancel change
                {
                   
                }
                this.ModelAccount.IsDOFRInitiated = true;

                UnRegisterRulesEvents();
            }
            finally
            {
                dialog.Dispose();
            }

        }

        public string CallDOFRAPI()
        {
            var facilitycode = string.IsNullOrEmpty(ModelAccount.Facility.DOFRFacilityMappingforTesting)?
                                    ModelAccount.Facility.Code :
                                    ModelAccount.Facility.DOFRFacilityMappingforTesting;

            string serviceCategory = string.Empty;
            if (ModelAccount.HospitalClinic !=null && !string.IsNullOrEmpty(ModelAccount.HospitalClinic.Code) && !string.IsNullOrEmpty(ModelAccount.EmbosserCard))   
                 serviceCategory = GetServiceCategoryForShorDesc(ModelAccount.EmbosserCard);

            var dOFRAPIRequest = new DOFRAPIRequest
            {
                facilityCode = facilitycode,
                patientType = GetDOFRPatientType(ModelAccount.KindOfVisit.Code),
                clinicCode = ModelAccount.KindOfVisit.Code != VisitType.INPATIENT ? ModelAccount.HospitalClinic.Code : string.Empty,
                serviceCode = ModelAccount.HospitalService.Code,
                ipaCode = ModelAccount.MedicalGroupIPA.Code,
                healthPlanCode = ModelAccount.Insurance.PrimaryCoverage.InsurancePlan.PlanID.Substring(0, 3),
                productType = ModelAccount.FinancialClass.Code,
                serviceCategory = serviceCategory,
                planType = ((CommercialCoverage)ModelAccount.Insurance.PrimaryCoverage).AidCodeType
               
            };

            var dOFRAPIResponse = DOFRInitiateBroker.DOFRInitiate(dOFRAPIRequest, ModelAccount);

            string returnMessage = string.Empty;

            if (dOFRAPIResponse.Item1 != null && !string.IsNullOrEmpty(dOFRAPIResponse.Item1.planId)
                && dOFRAPIResponse.Item2 == HttpStatusCode.OK)
            {
                returnMessage = string.Format(_APISuccessMessage, dOFRAPIResponse.Item1.planId, dOFRAPIResponse.Item1.planName, dOFRAPIResponse.Item1.serviceCategory);
            }
            else if (dOFRAPIResponse.Item1 == null && dOFRAPIResponse.Item2 == HttpStatusCode.OK)
            {
                returnMessage = _APIEmptyMessage;
            }
            else if (dOFRAPIResponse.Item1 == null
               && (dOFRAPIResponse.Item2 == HttpStatusCode.RequestTimeout || dOFRAPIResponse.Item2 == HttpStatusCode.GatewayTimeout))
            {
                returnMessage = _APITimeoutMessage;
            }
            else
                returnMessage = _APIDownOrErrorMessage;

            FusNoteFactory fusNoteFactory = new FusNoteFactory();
            fusNoteFactory.AddDOFRPFUSNote(ModelAccount, (dOFRAPIResponse.Item1 != null && !string.IsNullOrEmpty(dOFRAPIResponse.Item1.planId))? dOFRAPIResponse.Item1 : null);

            return returnMessage;
        }
        public static void SetDOFRInitiated(Account anAccount)
        {
            bool IsDOFREnabledForFacility = PatientAccess.Rules.DOFRFeatureManager.IsDOFREnabled(anAccount);
            if (IsDOFREnabledForFacility && anAccount.Activity.IsMaintenanceActivity())
            {
                if (anAccount.HasChangedFor("FinancialClass") ||
                anAccount.old_HospitalService != anAccount.HospitalService ||
                anAccount.HasChangedFor("EmbosserCard") ||
                anAccount.Insurance.HasChangedFor("PrimaryCoverage") ||
                anAccount.HasChangedFor("MedicalGroupIPA")
                )
                {
                    anAccount.IsDOFRInitiated = false;
                }
                else
                {
                    bool initiatedDOFR = true;
                    IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                    var hasPatientTypeChanged = accountBroker.HasPatientTypeChangedDuringTransfer(anAccount);
                    if (hasPatientTypeChanged)
                        initiatedDOFR = false;

                    anAccount.IsDOFRInitiated = initiatedDOFR;
                }

                if (PatientAccess.Rules.DOFRFeatureManager.IsPlanCommercial(anAccount))
                {
                    if(((CommercialCoverage)anAccount.Insurance.PrimaryCoverage).HasChangedFor("AidCodeType"))
                        anAccount.IsDOFRInitiated = false;
                }
            }
            else if(IsDOFREnabledForFacility && (
                anAccount.Activity.IsTransferOutToInActivity() ||
                  anAccount.Activity.IsTransferERToOutpatientActivity() ||
                   anAccount.Activity.IsTransferOutpatientToERActivity()
                ))
                {
                anAccount.IsDOFRInitiated = false;
            }

        }
        #endregion Public Methods

        #region Private Methods
        private string GetServiceCategoryForShorDesc(string shortDescCode)
        {
            string serviceCategoryLongDesc = String.Empty;
            long facilityId = ModelAccount.Facility.Oid;
            string clincCode = ModelAccount.HospitalClinic.Code;
            var serviceCategoryBroker = BrokerFactory.BrokerOfType<IServiceCategoryBroker>();

            var listOfServiceCategory = serviceCategoryBroker.GetServiceCategoryForClinicCode(facilityId, clincCode);

            if (listOfServiceCategory != null)
            {
                foreach (ClinicServiceCategory clinicServiceCategory in listOfServiceCategory)
                {
                    if (clinicServiceCategory.Code.Equals(shortDescCode))
                    {
                        serviceCategoryLongDesc = clinicServiceCategory.Description;
                        break;
                    }
                }
            }
            return serviceCategoryLongDesc;
        }
        private void RegisterRulesEvents()
        {
            _ruleEngine.RegisterEvent(typeof(DOFRInitiateRequired), this.ModelAccount, new EventHandler(DOFRInitiateRequiredEventHandler));
        }

        public void UnRegisterRulesEvents()
        {
            _ruleEngine.UnregisterEvent(typeof(DOFRInitiateRequired), this.ModelAccount, DOFRInitiateRequiredEventHandler);
        }

        public void RunRules()
        {
            _ruleEngine.EvaluateRule(typeof(DOFRInitiateRequired), this.ModelAccount);
        }

        private void DOFRInitiateRequiredEventHandler(object sender, EventArgs e)
        {

        }
        private static string GetDOFRPatientType(string patientTypeCode)
        {
            switch (patientTypeCode)
            {
                case VisitType.INPATIENT:
                    return DOFRAPIRequest.DOFRAPIPatientType.I.ToString();
                case VisitType.OUTPATIENT:
                case VisitType.RECURRING_PATIENT:
                    return DOFRAPIRequest.DOFRAPIPatientType.O.ToString();
                case VisitType.EMERGENCY_PATIENT:
                    return DOFRAPIRequest.DOFRAPIPatientType.E.ToString();
                default:
                    return string.Empty;
            }
        }

        private bool ValidateInitiateButton()
        {

            return DOFRFeatureManager.IsDOFRValid(this.ModelAccount);
        }

        private void ShowInitiateView()
        {
            DOFRInitiateView.ShowMe = true;
        }

        private void HideInitiateView()
        {
            DOFRInitiateView.ShowMe = false;
        }

        private void EnableInitateButton()
        {
            DOFRInitiateView.EnableInitiateButton = true;
        }

        private void DisableInitateButton()
        {
            DOFRInitiateView.EnableInitiateButton = false;
        }
        #endregion Private Methods


    }
}