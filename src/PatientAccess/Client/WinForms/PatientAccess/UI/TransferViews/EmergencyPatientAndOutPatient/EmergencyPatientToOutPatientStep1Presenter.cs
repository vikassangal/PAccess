using System;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using PatientAccess.Utilities;

namespace PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient
{
    public class EmergencyPatientToOutPatientStep1Presenter
    {
        #region Event Handlers

        private void AdmitSourceRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetAdmitSourceRequired();
        }

        private void AlternateCareFacilityRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetAlternateCareFacilityRequired();
        }

        private void HospitalServiceRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetHospitalServiceRequired();
        }
        private void ClinicOneRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetClinicOneRequired();
        }

        private void ChiefComplaintRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetChiefComplaintRequired();
        }

        private void LocationRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetLocationRequired();
        }

        private void TransferDateRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetTransferDateRequired();
        }

        private void TransferTimeRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetTransferTimeRequired();
        }

        private void TransferDateFutureDateEventHandler(object sender, EventArgs e)
        {
            Step1View.DisplayTransferDateIsFutureDateMessage();
        }

        private void TransferTimeFutureTimeEventHandler(object sender, EventArgs e)
        {
            Step1View.DisplayTransferTimeIsFutureTimeMessage(e);
        }

        private void TransferDateTimeBeforeAdmitDateTimeEventHandler(object sender, EventArgs e)
        {
            Step1View.DisplayTransferDateTimeBeforeAdmitDateTimeMessage(e);
        }

        private void EmailAddressRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetEmailAddressRequired();
        }

        #endregion

        public readonly IEmergencyPatientToOutPatientStep1View Step1View;
        private DateTime i_FacilityDateTime;
        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        public EmergencyPatientToOutPatientStep1Presenter(IEmergencyPatientToOutPatientStep1View step1View, IMessageBoxAdapter messageBoxAdapter,
                                                          Account account, IAlternateCareFacilityPresenter alternateCareFacilityPresenter, RuleEngine ruleEngine)
        {
            Guard.ThrowIfArgumentIsNull(step1View, "EmergencyPatientToOutPatientStep1View");
            Guard.ThrowIfArgumentIsNull(account, "Account");
            Guard.ThrowIfArgumentIsNull(alternateCareFacilityPresenter, "AlternateCareFacilityPresenter");
            Guard.ThrowIfArgumentIsNull(ruleEngine, "RuleEngine");

            Step1View = step1View;
            Account = account;
            MessageBoxAdapter = messageBoxAdapter;
            AlternateCareFacilityPresenter = alternateCareFacilityPresenter;
            RuleEngine = ruleEngine;
        }

        public RuleEngine RuleEngine { get; private set; }

        public Account Account { get; private set; }

        public IAlternateCareFacilityPresenter AlternateCareFacilityPresenter { get; private set; }

        public bool PreValidationSuccess { get; private set; }

        private DateTime FacilityDateTime
        {
            get
            {
                if (i_FacilityDateTime == DateTime.MinValue)
                {
                    i_FacilityDateTime = TransferService.GetLocalDateTime(User.GetCurrent().Facility.GMTOffset,
                                                                          User.GetCurrent().Facility.DSTOffset);
                }

                return i_FacilityDateTime;
            }
        }

        public void UpdateView()
        {
            if (Account != null)
            {
                if (Account.Activity != null)
                {
                    Step1View.UserContextView = Account.Activity.ContextDescription;
                }
                Step1View.EnableNextButton(true);
                if (Account.Patient != null)
                {
                    Step1View.PatientContextView1.Model = Account.Patient;
                    Step1View.PatientContextView1.Account = Account;
                    Step1View.PatientContextView1.UpdateView();
                    Step1View.PatientName = Account.Patient.FormattedName;
                }

                DoPreValidation();
                Step1View.AccountNumber = Account.AccountNumber.ToString();
                Step1View.AdmitDate = CommonFormatting.LongDateFormat(Account.AdmitDate);
                Step1View.AdmitTime = CommonFormatting.DisplayedTimeFormat(Account.AdmitDate);

                if (Account.Insurance.Coverages.Count > 0)
                {
                    foreach (Coverage coverage in Account.Insurance.Coverages)
                    {
                        if (coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID)
                        {
                            Step1View.PrimaryPlan = coverage.InsurancePlan.PlanID + " " +
                                                    coverage.InsurancePlan.Payor.Name;
                        }
                    }
                }

                if (Account.FinancialClass != null)
                {
                    Step1View.FinancialClass = Account.FinancialClass.ToCodedString();
                }
                Step1View.AdmitSource = Account.AdmitSource.DisplayString;
                Step1View.PatientType = Account.KindOfVisit.DisplayString;
                Step1View.HospitalService = Account.HospitalService.DisplayString;
                Step1View.LocationLabel = Account.Location.DisplayString;

                if (PreValidationSuccess)
                {
                    PopulateAdmitSources();
                    SetAdmittingCategory();
                    Step1View.TransferToPatientType = "2 OUTPATIENT";

                    Account.KindOfVisit = new VisitType(0L, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC,
                                                        VisitType.OUTPATIENT);
                    Account.HospitalService = null;
                    PopulateHsvCodes();
                    Step1View.LocationView1.Model = Account;
                    Step1View.LocationView1.UpdateView();
                    Step1View.LocationView1.DisableLocationControls();
                    PopulateClinics();
                    Step1View.ChiefComplaint = Account.Diagnosis.ChiefComplaint.Trim();
                    Step1View.OriginalChiefComplaint = Account.Diagnosis.ChiefComplaint.Trim();
                    Step1View.TransferDate = String.Format("{0:D2}{1:D2}{2:D4}", FacilityDateTime.Month,
                                                           FacilityDateTime.Day, FacilityDateTime.Year);
                    Step1View.TransferTime = String.Format("{0:D2}{1:D2}", FacilityDateTime.Hour,
                                                           FacilityDateTime.Minute);
                    Account.TransferDate = FacilityDateTime;
                    RunRules();
                }
                else
                {
                    Step1View.DisableControls();
                }
            }
        }

        public void RunRules()
        {
            RuleEngine.EvaluateRule(typeof (AdmitSourceRequired), Account);
            RuleEngine.EvaluateRule(typeof (HospitalServiceRequired), Account);
            RuleEngine.EvaluateRule(typeof (ClinicOneRequired), Account);
            RuleEngine.EvaluateRule(typeof (LocationRequired), Account);
            RuleEngine.EvaluateRule(typeof (ChiefComplaintRequired), Account);
            RuleEngine.EvaluateRule(typeof (TransferDateRequired), Account);
            RuleEngine.EvaluateRule(typeof (TransferTimeRequired), Account);
            RuleEngine.EvaluateRule(typeof (TransferDateFutureDate), Account);
            RuleEngine.EvaluateRule(typeof (TransferTimeFutureTime), Account);
            RuleEngine.EvaluateRule(typeof (TransferDateTimeBeforeAdmitDateTime), Account);
            RuleEngine.EvaluateRule(typeof (EmailAddressRequired), Account);
        }

        private void PopulateClinics()
        {
            IHospitalClinicsBroker clinicBroker = new HospitalClinicsBrokerProxy();
            var allClinics = clinicBroker.HospitalClinicsFor(Account.Facility.Oid);
            Step1View.ClearClinicCodesComboBox();

            foreach (var clinic in allClinics)
            {
                Step1View.AddClinicCode(clinic as HospitalClinic);
            }
            if (Account.HospitalClinic != null)
            {
                Step1View.HospitalClinicSelectedItem = Account.HospitalClinic;
            }
            else
            {
                Step1View.HospitalClinicsSelectedIndex = 0;
            }
        }

        private void PopulateAdmitSources()
        {
            var brokerProxy = new AdmitSourceBrokerProxy();
            var allSources = brokerProxy.AllTypesOfAdmitSources(User.GetCurrent().Facility.Oid);
            Step1View.ClearAdmitSourcesComboBox();

            foreach (var admitSource in allSources)
            {
                Step1View.AddAdmitSources(admitSource as AdmitSource);
            }
            if (Account.AdmitSource != null)
            {
                Step1View.AdmitSourcesSelectedItem = Account.AdmitSource;
            }
            else
            {
                Step1View.AdmitSourcesSelectedIndex = 0;
            }
        }

        private void PopulateHsvCodes()
        {
            var brokerProxy = new HSVBrokerProxy();
            var hsvCodes = brokerProxy.HospitalServicesFor(User.GetCurrent().Facility.Oid, VisitType.OUTPATIENT,
                                                           "Y");
            Step1View.ClearHospitalServicesComboBox();

            foreach (var hospitalService in hsvCodes)
            {
                Step1View.AddHospitalService(hospitalService as HospitalService);
            }
            Step1View.SetHospitalServiceListSorted();
            if (Account.HospitalService != null)
            {
                Step1View.HospitalServiceSelectedItem = Account.HospitalService;
            }
            else
            {
                Step1View.HospitalServiceSelectedIndex = 0;
            }
        }

        private void DoPreValidation()
        {
            if (!IsValidPatientType())
            {
                DisplayInfoMessage(UIErrorMessages.TRANSFER_NOT_ERPATIENT_MSG);
                PreValidationSuccess = false;
            }
            else if (!NoDischargePassed3MidNights())
            {
                DisplayInfoMessage(UIErrorMessages.TRANSFER_DISCHARGED_3_MIDNIGHTS_AGO_MSG);
                PreValidationSuccess = false;
            }
            else if (!HasAbstractNotCompleted())
            {
                DisplayInfoMessage(UIErrorMessages.TRANSFER_ABSTRACT_COMPLETED_MSG);
                PreValidationSuccess = false;
            }
            else if (!IsLockOK())
            {
                DisplayInfoMessage(UIErrorMessages.DISCHARGE_ACCOUNT_LOCKED_MSG);
                PreValidationSuccess = false;
            }
            else
            {
                PreValidationSuccess = true;
            }
        }

        private void DisplayInfoMessage(string infoMessage)
        {
            Step1View.DisplayInfoMessage(infoMessage);
        }

        public void UpdateClinicCode(HospitalClinic hospitalClinic)
        {
            Account.HospitalClinic = hospitalClinic;
            RuleEngine.EvaluateRule(typeof (ClinicOneRequired), Account);
        }

        private bool IsLockOK()
        {
            var blnRC = false;
            if (Account.AccountLock.IsLocked)
            {
                if (Account.AccountLock.AcquiredLock)
                {
                    blnRC = true;
                }
            }
            return blnRC;
        }

        private bool IsValidPatientType()
        {
            return Account.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT &&
                   Account.FinancialClass.Code != FinancialClass.MED_SCREEN_EXM_CODE;
        }

        private bool NoDischargePassed3MidNights()
        {
            return Account.DischargeDate == DateTime.MinValue || FacilityDateTime <= Account.DischargeDate.AddDays(3);
        }

        private bool HasAbstractNotCompleted()
        {
            return Account.DischargeDate == DateTime.MinValue || !Account.AbstractExists;
        }

        private void SetAdmittingCategory()
        {
            if (Account.KindOfVisit != null)
            {
                if (Account.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT)
                    Account.AdmittingCategory = ADMITTING_CATEGORY_EMERGENCY;
                else
                    Account.AdmittingCategory = ADMITTING_CATEGORY_URGENT;
            }
            else
            {
                Account.AdmittingCategory = ADMITTING_CATEGORY_URGENT;
            }
        }

        public void HandleAdmitSourceSelectedIndexChanged(AdmitSource newAdmitSource)
        {
            if (newAdmitSource != null)
            {
                Account.AdmitSource = newAdmitSource;
                BreadCrumbLogger.GetInstance.Log(String.Format("{0} selected", Account.AdmitSource.Description));
            }
            EvaluateAdmitSourceRequired();

            AlternateCareFacilityPresenter.HandleAlternateCareFacility();
        }

        public void UpdateAlternateCareFacility(string selectedAlternateCare)
        {
            AlternateCareFacilityPresenter.UpdateAlternateCareFacility(selectedAlternateCare);
            AlternateCareFacilityPresenter.EvaluateAlternateCareFacilityRule();
        }

        public void UpdateHospitalService(HospitalService hospitalService)
        {
            Account.HospitalService = hospitalService;
            Step1View.LocationView1.EnableLocationControls();
            BreadCrumbLogger.GetInstance.Log(String.Format("{0} selected", Account.HospitalService.Description));
            EvaluateHospitalServiceRequired();
            EvaluationLocationRequired();
        }

        public void UpdateBedSelected(Location location)
        {
            Account.Location = location;
            EvaluationLocationRequired();
        }

        private void EvaluateAdmitSourceRequired()
        {
            RuleEngine.EvaluateRule(typeof (AdmitSourceRequired), Account);
            RuleEngine.EvaluateRule(typeof (AdmitSourceRequired), Account);
            RuleEngine.EvaluateRule(typeof (AdmitSourcePreferred), Account);
            RuleEngine.EvaluateRule(typeof (InvalidAdmitSourceCode), Account);
            RuleEngine.EvaluateRule(typeof (InvalidAdmitSourceCodeChange), Account);
        }

        private void EvaluateHospitalServiceRequired()
        {
            RuleEngine.EvaluateRule(typeof (HospitalServiceRequired), Account);
        }

        private void EvaluationLocationRequired()
        {
            RuleEngine.EvaluateRule(typeof (LocationRequired), Account);
        }

        public void UpdateChiefComplaimt(string chiefComplaint)
        {
            Account.Diagnosis.ChiefComplaint = chiefComplaint;
            RuleEngine.EvaluateRule(typeof (ChiefComplaintRequired), Account);
        }

        private void EvaluateTransferDate()
        {
            RuleEngine.EvaluateRule(typeof (TransferDateRequired), Account);
            RuleEngine.EvaluateRule(typeof(TransferDateFutureDate), Account, TRANSFER_DATE);
            RuleEngine.EvaluateRule(typeof(TransferTimeFutureTime), Account, TRANSFER_DATE);
            RuleEngine.EvaluateRule(typeof(TransferDateTimeBeforeAdmitDateTime), Account, TRANSFER_DATE);
        }

        public bool UpdateTransferDate()
        {
            var transferDate = Step1View.TransferDate;
            if (transferDate != String.Empty)
            {
                if (Step1View.IsTransferDateValid())
                {
                    Account.TransferDate = Step1View.GetTransferDateTime();
                    EvaluateTransferDate();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Account.TransferDate = Step1View.GetTransferDateTime();
                EvaluateTransferDate();
                return true;
            }
        }

        private void EvaluateTransferTime()
        {
            RuleEngine.EvaluateRule(typeof (TransferDateRequired), Account);
            RuleEngine.EvaluateRule(typeof(TransferDateFutureDate), Account, TRANSFER_TIME);
            RuleEngine.EvaluateRule(typeof(TransferTimeFutureTime), Account, TRANSFER_TIME);
            RuleEngine.EvaluateRule(typeof(TransferTimeRequired), Account);
            RuleEngine.EvaluateRule(typeof(TransferDateTimeBeforeAdmitDateTime), Account, TRANSFER_TIME);
        }

        public bool UpdateTransferTime()
        {
            var mtbTransferTime = Step1View.TransferTime;

            if (mtbTransferTime != string.Empty && mtbTransferTime != ZERO_TIME)
            {
                var timeValidationResult = DateValidator.IsValidTime(mtbTransferTime);

                switch (timeValidationResult)
                {
                    case TimeValidationResult.Valid:
                        Account.TransferDate = Step1View.GetTransferDateTime();
                        EvaluateTransferTime();
                        return true;
                    case TimeValidationResult.TimeIsInvalid:
                        MessageBoxAdapter.ShowMessageBox(UIErrorMessages.TIME_NOT_VALID_MSG,
                                                         UIErrorMessages.ERROR,
                                                         MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                         MessageBoxAdapterDefaultButton.Button1);
                        return false;

                    case TimeValidationResult.HourIsInvalid:
                        MessageBoxAdapter.ShowMessageBox(UIErrorMessages.HOUR_INVALID_ERRMSG,
                                                         UIErrorMessages.ERROR,
                                                         MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                         MessageBoxAdapterDefaultButton.Button1);
                        return false;

                    case TimeValidationResult.MinuteIsInvalid:
                        MessageBoxAdapter.ShowMessageBox(UIErrorMessages.MINUTE_INVALID_ERRMSG,
                                                         UIErrorMessages.ERROR,
                                                         MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                                         MessageBoxAdapterDefaultButton.Button1);
                        return false;
                    default:
                        return false;
                }
            }
            Account.TransferDate = Step1View.GetTransferDateTime();
            EvaluateTransferTime();
            return true;
        }

        public void RegisterRulesEvents()
        {
            RuleEngine.LoadRules(Account);

            RuleEngine.RegisterEvent(typeof (AdmitSourceRequired), Account, AdmitSourceRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (AlternateCareFacilityRequired), AlternateCareFacilityRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (HospitalServiceRequired), Account, HospitalServiceRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (LocationRequired), Account, LocationRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (ChiefComplaintRequired), Account, ChiefComplaintRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof(ClinicOneRequired), Account, ClinicOneRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (TransferDateRequired), Account, TransferDateRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (TransferTimeRequired), Account, TransferTimeRequiredEventHandler);


            RuleEngine.RegisterEvent(typeof (TransferDateFutureDate), Account, TransferDateFutureDateEventHandler);
            RuleEngine.RegisterEvent(typeof (TransferTimeFutureTime), Account, TransferTimeFutureTimeEventHandler);
            RuleEngine.RegisterEvent(typeof (TransferDateTimeBeforeAdmitDateTime), Account,
                                     TransferDateTimeBeforeAdmitDateTimeEventHandler);
            RuleEngine.RegisterEvent(typeof (EmailAddressRequired), Account, EmailAddressRequiredEventHandler);

        }

        public void UnRegisterRulesEvents()
        {
            RuleEngine.UnregisterEvent(typeof (AdmitSourceRequired), Account,
                                       AdmitSourceRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof (AlternateCareFacilityRequired), Account,
                                       AlternateCareFacilityRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof (HospitalServiceRequired), Account,
                                       HospitalServiceRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof(ClinicOneRequired), Account,
                                       ClinicOneRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof (LocationRequired), Account,
                                       (sender, e) => Step1View.SetLocationRequired());

            RuleEngine.UnregisterEvent(typeof (ChiefComplaintRequired), Account,
                                       ChiefComplaintRequiredEventHandler);

            RuleEngine.UnregisterEvent(typeof (TransferDateRequired), Account,
                                       TransferDateRequiredEventHandler);

            RuleEngine.UnregisterEvent(typeof (TransferDateFutureDate), Account,
                                       TransferDateFutureDateEventHandler);
            RuleEngine.UnregisterEvent(typeof (TransferTimeRequired), Account,
                                       TransferTimeRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof (TransferTimeFutureTime), Account,
                                       TransferTimeFutureTimeEventHandler);
            RuleEngine.UnregisterEvent(typeof (TransferDateTimeBeforeAdmitDateTime), Account,
                                       TransferDateTimeBeforeAdmitDateTimeEventHandler);
            RuleEngine.RegisterEvent(typeof(EmailAddressRequired), Account, EmailAddressRequiredEventHandler);
        }

        #region Constants

        private const string ZERO_TIME = "0000";
        private const string ADMITTING_CATEGORY_URGENT = "3";

        private const string ADMITTING_CATEGORY_EMERGENCY = "2";
        public  string TRANSFER_DATE = "TransferDate";
        private string TRANSFER_TIME = "TransferTime";
        #endregion Constants
    }
}