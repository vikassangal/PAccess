using System;
using System.Linq;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using PatientAccess.Utilities;

namespace PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient
{
    public class OutPatientToEmergencyPatientStep1Presenter
    {
        #region Event Handlers

        private void AdmitSourceRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetAdmitSourceRequired();
        }

        private void HospitalServiceRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetHospitalServiceRequired();
        }

        private void ChiefComplaintRequiredEventHandler(object sender, EventArgs e)
        {
            Step1View.SetChiefComplaintRequired();
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

        #endregion

        public readonly IOutPatientToEmergencyPatientStep1View Step1View;
        private DateTime i_FacilityDateTime;
        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        public OutPatientToEmergencyPatientStep1Presenter(IOutPatientToEmergencyPatientStep1View step1View, IMessageBoxAdapter messageBoxAdapter,
                                                          Account account, RuleEngine ruleEngine)
        {
            Guard.ThrowIfArgumentIsNull(step1View, "OutPatientToEmergencyPatientStep1View");
            Guard.ThrowIfArgumentIsNull(account, "Account");
            Guard.ThrowIfArgumentIsNull(ruleEngine, "RuleEngine");

            Step1View = step1View;
            Account = account;
            MessageBoxAdapter = messageBoxAdapter;
            RuleEngine = ruleEngine;
        }

        public RuleEngine RuleEngine { get; private set; }
        public Account Account { get; private set; }

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
		
        public void UpdateModel()
        {
            Account.LocationTo = new Location();
            Account.Location = new Location();
        }
		
        public void UpdateView()
        {
            RegisterRulesEvents();

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

                Step1View.AdmitSource = Account.AdmitSource.DisplayString;
                Step1View.PatientType = Account.KindOfVisit.DisplayString;
                Step1View.HospitalService = Account.HospitalService.DisplayString;
                Step1View.LocationLabel = Account.Location.DisplayString;

                if (PreValidationSuccess)
                {
                    Account.LocationFrom = Account.Location != null ? (Location)Account.Location.Clone() : null; 
                    PopulateAdmitSources();
                    Step1View.TransferToPatientType = "3 ER PATIENT";

                    Account.KindOfVisit = new VisitType(0L, PersistentModel.NEW_VERSION,
                                                        VisitType.EMERGENCY_PATIENT_DESC, VisitType.EMERGENCY_PATIENT);
                    Account.HospitalService = null;
                    PopulateHsvCodes();
                    Step1View.ChiefComplaint = Account.Diagnosis.ChiefComplaint.Trim();
                    Step1View.OriginalChiefComplaint = Account.Diagnosis.ChiefComplaint.Trim();
                    Account.TransferDate = DateTime.MinValue;

                    Step1View.TransferDate = String.Empty;
                    Step1View.TransferTime = string.Empty; 
                    RunRules();
                }
                else
                {
                    Step1View.DisableControls();
                }
            }
        }

        private void RunRules()
        {
            RuleEngine.EvaluateRule(typeof (AdmitSourceRequired), Account);
            RuleEngine.EvaluateRule(typeof (HospitalServiceRequired), Account);

            RuleEngine.EvaluateRule(typeof (ChiefComplaintRequired), Account);
            RuleEngine.EvaluateRule(typeof (TransferDateRequired), Account);
            RuleEngine.EvaluateRule(typeof (TransferTimeRequired), Account);
            RuleEngine.EvaluateRule(typeof (TransferDateFutureDate), Account);
            RuleEngine.EvaluateRule(typeof (TransferTimeFutureTime), Account);
            RuleEngine.EvaluateRule(typeof (TransferDateTimeBeforeAdmitDateTime), Account);
        }

        private void PopulateAdmitSources()
        {
            var brokerProxy = new AdmitSourceBrokerProxy();
            var allAdmitSources = brokerProxy.AllTypesOfAdmitSources(User.GetCurrent().Facility.Oid);
            var allSources = allAdmitSources.Cast<AdmitSource>();
            Step1View.ClearAdmitSourcesComboBox();

            foreach (var admitSource in  allSources)
            {
                Step1View.AddAdmitSources(admitSource);
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

        public void PopulateHsvCodes()
        {
            var brokerProxy = new HSVBrokerProxy();
            var hsvCodes = brokerProxy.HospitalServicesFor(User.GetCurrent().Facility.Oid, VisitType.EMERGENCY_PATIENT);

            var allHSVCodes = hsvCodes.Cast<HospitalService>();
            Step1View.ClearHospitalServiceComboBox();

            foreach (var hospitalService in allHSVCodes)
            {
                Step1View.AddHospitalService(hospitalService);
            }
            Step1View.SetHospitalServiceListSorted();
            if (Account.HospitalService != null)
            {
                Step1View.HospitalServiceSelectedItem = Account.HospitalService;
            }
            else
            {
                Step1View.HospitalServicesSelectedIndex = 0;
            }
        }

        private void DoPreValidation()
        {
            if (!IsValidPatientType())
            {
                DisplayInfoMessage(UIErrorMessages.TRANSFER_NOT_OUTPATIENT_MSG);
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

        private bool IsBeddedHospitalService()
        {
            return Account.KindOfVisit.Code == VisitType.OUTPATIENT &&
                   Account.HospitalService.IsDayCare() &&
                   Account.HospitalService.DayCare == "Y";
        }

        private bool IsValidPatientType()
        {
            return Account.KindOfVisit.Code == VisitType.OUTPATIENT && IsBeddedHospitalService();
        }

        private bool NoDischargePassed3MidNights()
        {
            return Account.DischargeDate == DateTime.MinValue || FacilityDateTime <= Account.DischargeDate.AddDays(3);
        }

        private bool HasAbstractNotCompleted()
        {
            return Account.DischargeDate == DateTime.MinValue || !Account.AbstractExists;
        }

        private void DisplayInfoMessage(string infoMessage)
        {
            Step1View.DisplayInfoMessage(infoMessage);
        }

        public void HandleAdmitSourceSelectedIndexChanged(AdmitSource newAdmitSource)
        {
            if (newAdmitSource != null)
            {
                Account.AdmitSource = newAdmitSource;
                BreadCrumbLogger.GetInstance.Log(String.Format("{0} selected", Account.AdmitSource.Description));
            }
            EvaluateAdmitSourceRequired();
        }

        public void UpdateHospitalService(HospitalService hospitalService)
        {
            Account.HospitalService = hospitalService;
            UnLockNewReservedBed();
            BreadCrumbLogger.GetInstance.Log(String.Format("{0} selected", Account.HospitalService.Description));
            EvaluateHospitalServiceRequired();
        }

        private void UnLockNewReservedBed()
        {
            if (Account.Location != null)
            {
                TransferService.ReleaseBed(Account.Location, User.GetCurrent().Facility);
            }
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

        public void UpdateChiefComplaint(string chiefComplaint)
        {
            Account.Diagnosis.ChiefComplaint = chiefComplaint;
            RuleEngine.EvaluateRule(typeof (ChiefComplaintRequired), Account);
        }

        private bool evaluateTransferDate()
        {
            return
            RuleEngine.EvaluateRule(typeof (TransferDateRequired), Account) &&
            RuleEngine.EvaluateRule(typeof(TransferDateFutureDate), Account, TRANSFER_DATE) &&
            RuleEngine.EvaluateRule(typeof(TransferTimeFutureTime), Account, TRANSFER_DATE) &&
            RuleEngine.EvaluateRule(typeof(TransferDateTimeBeforeAdmitDateTime),Account, TRANSFER_DATE);
        }

        public bool UpdateTransferDate()
        {
            var transferDate = Step1View.TransferDate;
            if (transferDate != String.Empty)
            {
                if (Step1View.IsTransferDateValid())
                {
                    Account.TransferDate = Step1View.GetTransferDateTime();
                    return !evaluateTransferDate();
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Account.TransferDate = Step1View.GetTransferDateTime();
                return evaluateTransferDate(); 
            }
        }

        private void EvaluateTransferTime()
        {
            RuleEngine.EvaluateRule(typeof (TransferDateRequired), Account);
            RuleEngine.EvaluateRule(typeof(TransferDateFutureDate), Account, TRANSFER_TIME);
            RuleEngine.EvaluateRule(typeof(TransferTimeFutureTime), Account, TRANSFER_TIME);
            RuleEngine.EvaluateRule(typeof (TransferTimeRequired), Account);
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

        private void RegisterRulesEvents()
        {
            RuleEngine.LoadRules(Account);

            RuleEngine.RegisterEvent(typeof (AdmitSourceRequired), Account, AdmitSourceRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (HospitalServiceRequired), Account, HospitalServiceRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (ChiefComplaintRequired), Account, ChiefComplaintRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (TransferDateRequired), Account, TransferDateRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (TransferTimeRequired), Account, TransferTimeRequiredEventHandler);
            RuleEngine.RegisterEvent(typeof (TransferDateFutureDate), Account, TransferDateFutureDateEventHandler);
            RuleEngine.RegisterEvent(typeof (TransferTimeFutureTime), Account, TransferTimeFutureTimeEventHandler);
            RuleEngine.RegisterEvent(typeof (TransferDateTimeBeforeAdmitDateTime), Account, TransferDateTimeBeforeAdmitDateTimeEventHandler);
        }

        public void UnRegisterRulesEvents()
        {
            RuleEngine.UnregisterEvent(typeof (AdmitSourceRequired), Account, AdmitSourceRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof (HospitalServiceRequired), Account, HospitalServiceRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof (ChiefComplaintRequired), Account, ChiefComplaintRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof (TransferDateRequired), Account, TransferDateRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof (TransferDateFutureDate), Account, TransferDateFutureDateEventHandler);
            RuleEngine.UnregisterEvent(typeof (TransferTimeRequired), Account, TransferTimeRequiredEventHandler);
            RuleEngine.UnregisterEvent(typeof (TransferTimeFutureTime), Account, TransferTimeFutureTimeEventHandler);
            RuleEngine.UnregisterEvent(typeof (TransferDateTimeBeforeAdmitDateTime), Account, TransferDateTimeBeforeAdmitDateTimeEventHandler);
        }

        #region Constants

        private const string ZERO_TIME = "0000";
        public string TRANSFER_DATE = "TransferDate";
        private string TRANSFER_TIME = "TransferTime";
        #endregion Constants
    }
}