using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.ViewImpl;

namespace PatientAccess.UI.CommonControls
{
    public partial class PatientTypeHSVLocationView : ControlView
    {
        #region Constructor
        public PatientTypeHSVLocationView()
        {
            InitializeComponent();
            locationView.EditVerifyButtonText = "&Verify";
        }
        #endregion

        #region Events
        public event EventHandler SetTenetCare;
        public event EventHandler ClearSelectedClinics;
        public event EventHandler SelectPreviouslyStoredClinicValue;
        public event EventHandler<VisitTypeEventArgs> PatientTypeChanged;
        public event EventHandler<EventArgs> HSVChanged;
        #endregion

        #region public methods

        public override void UpdateView()
        {
            try
            {
                UpdateHospitalService();
                SetKindOfVisit();
                this.registerEvents();
                this.IsReregisterFacility();
                this.PopulatePatientTypes();
                EnableHospitalService();
                this.PopulateLocation();
                SetAccommodationCodes();
                SetNormalColor();
                cbxReregister.Visible = SetReregister();
                isLoadData = false;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        private void LoadAccomodationCodes(string nursingStationCode)
        {
            this.NursingStationCode = nursingStationCode;

            cmbAccommodation.Items.Clear();

            if (AccomodationCodes != null)
            {
                for (int count = 0; count < AccomodationCodes.Count; count++)
                {
                    accomodation = (Accomodation)AccomodationCodes[count];
                    cmbAccommodation.Items.Add(accomodation);
                }
            }
        }

        #endregion

        #region Event Handlers

        private void PatientTypeHSVLocationView_Load(object sender, EventArgs e)
        {
            this.locationView.EditFindButtonText = "F&ind...";
            this.locationView.BedSelected += this.locationView_BedSelected;
        }

        private void PatientTypeRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.cmbPatientType);
        }

        private void HospitalServiceRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.cmbHospitalService);
        }

        private void AccomodationRequiredEventHandler(object sender, EventArgs e)
		{
            UIColors.SetRequiredBgColor(this.cmbAccommodation);
        }
        private void HospitalServicePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( this.cmbHospitalService );
        }

        
        private void ReasonForAccomodationRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.cmbReasonForPrivateAccommodation);
        }

        private void PatientTypeHSVLocationView_Disposed(object sender, EventArgs e)
        {
            this.unregisterEvents();
        }

        private void ProcessInvalidCodeEvent(PatientAccessComboBox comboBox)
        {
            UIColors.SetDeactivatedBgColor(comboBox);

            MessageBox.Show(UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1);

            if (!comboBox.Focused)
            {
                comboBox.Focus();
            }
        }

        private void InvalidAccomodationCodeChangeEventHandler(object sender, EventArgs e)
        {
            this.ProcessInvalidCodeEvent(this.cmbAccommodation);
        }

        private void InvalidHospitalServiceCodeChangeEventHandler(object sender, EventArgs e)
        {
            this.ProcessInvalidCodeEvent(this.cmbHospitalService);
        }

        private void InvalidPatientTypeCodeChangeEventHandler(object sender, EventArgs e)
        {
            this.ProcessInvalidCodeEvent(this.cmbPatientType);
        }

        private void InvalidAccomodationCodeEventHandler(object sender, EventArgs e)
        {
            UIColors.SetDeactivatedBgColor(this.cmbAccommodation);
        }

        private void InvalidHospitalServiceCodeEventHandler(object sender, EventArgs e)
        {
            UIColors.SetDeactivatedBgColor(this.cmbHospitalService);
        }

        private void InvalidPatientTypeCodeEventHandler(object sender, EventArgs e)
        {
            UIColors.SetDeactivatedBgColor(this.cmbPatientType);
        }

        /// <summary>
        /// Combo Box Patient Type Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPatientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbHospitalService.ResetText();
            cmbHospitalService.Items.Clear();
            this.locationView.ReleaseReservedBed();
            ResetAll();

            savedPatientType = this.Model.KindOfVisit;
            selectedPatientType = this.cmbPatientType.SelectedItem as VisitType;
            if (selectedPatientType != null)
            {
                this.Model.KindOfVisit = selectedPatientType;

                if ((this.Model.IsNew) ||
                      ((this.Model.Activity != null) &&
                        (this.Model.Activity.AssociatedActivityType != null) &&
                        (this.Model.Activity.AssociatedActivityType == typeof(ActivatePreRegistrationActivity)
                            && this.Model.Activity.GetType()!=typeof(AdmitNewbornActivity))
                       )
                    )
                {
                    this.Model.HospitalService = new HospitalService();
                }
            }

            FirePatientTypeChanged( Model.KindOfVisit );

            if (SetTenetCare != null)
            {
                SetTenetCare(this, new LooseArgs(this.Model.KindOfVisit));
            }

            this.PopulateHsvCodes();

            if (this.Model.Activity != null && Model.Activity.IsNewBornRelatedActivity() ||
                this.Model.Activity is RegistrationActivity)
            {
                this.cmbHospitalService.Enabled = (this.Model.KindOfVisit != null && this.Model.KindOfVisit.Code != String.Empty);
            }

            this.PopulateLocation();

            this.setAccomodation();

            RuleEngine.GetInstance().EvaluateRule(typeof(InvalidPatientTypeCode), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(PatientTypeRequired), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(HospitalServiceRequired), this.Model);
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServicePreferred ), this.Model );
            RuleEngine.GetInstance().EvaluateRule(typeof(DiagnosisClinicOneRequired), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(AccomodationRequired), this.Model);

            if (this.Model.Activity is PreRegistrationActivity ||
                this.Model.Activity is PostMSERegistrationActivity ||
                this.Model.Activity is AdmitNewbornActivity ||
                this.Model.Activity is PreAdmitNewbornActivity ||
                this.Model.Activity is RegistrationActivity ||
                this.Model.Activity is MaintenanceActivity)
            {
                if (this.ClearSelectedClinics != null)
                {
                    ClearSelectedClinics(this, new LooseArgs(this.Model.KindOfVisit));
                }
                if (
                    (this.Model.Activity is RegistrationActivity && this.Model.KindOfVisit == null || this.Model.KindOfVisit.Code != VisitType.INPATIENT) ||
                    (this.Model.Activity is PostMSERegistrationActivity) ||
                    (this.Model.Activity is MaintenanceActivity && this.Model.KindOfVisit == null || this.Model.KindOfVisit.Code != VisitType.INPATIENT)
                   )
                {
                    if (this.SelectPreviouslyStoredClinicValue != null)
                    {
                        SelectPreviouslyStoredClinicValue(this, new EventArgs());
                    }
                }
            }
            

            if (this.Model.Facility.Reregister.Code == YesNoFlag.CODE_YES
                && this.Model.KindOfVisit != null && this.Model.KindOfVisit.Code == VisitType.RECURRING_PATIENT)
            {
                this.cbxReregister.Enabled = true;

                if (isLoadData)    // if loading model data from UpdateView for the first time
                {
                    this.cbxReregister.Checked = (this.Model.Reregister.Code != YesNoFlag.CODE_NO);
                }
                else
                {
                    this.cbxReregister.Checked = true;
                }
            }
            else
            {
                this.cbxReregister.Enabled = false;
                this.cbxReregister.Checked = false;
            }

            if (this.Model.HospitalService == null)
            {
                this.Model.HospitalService = new HospitalService();
            }
            hIEShareDataFlagView.PatientAccount = Model;
            if (selectedPatientType != savedPatientType)
            {
                HIEShareDataPresenter.HandleNotifyPCP();
            }
            Model.RemoveOccurrenceCode50IfNotApplicable();
            RuleEngine.GetInstance().RegisterEvent(typeof(PatientTypeRequired), new EventHandler(PatientTypeRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(HospitalServiceRequired), new EventHandler(HospitalServiceRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServicePreferred ), new EventHandler( HospitalServicePreferredEventHandler ) );
        }

        internal void FirePatientTypeChanged(VisitType visitType)
        {
            if (this.PatientTypeChanged != null)
            {
                this.PatientTypeChanged(this, new VisitTypeEventArgs(visitType));
            }
        }

       private void FireHSVChanged()
       {
           if (this.HSVChanged != null)
           {
               this.HSVChanged(this,new EventArgs());
           }
       }


        /// <summary>
        /// Patient Type Validatinng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPatientType_Validated(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(this.cmbPatientType);
            UIColors.SetNormalBgColor(this.cmbHospitalService);
            this.Model.KindOfVisit = (VisitType)this.cmbPatientType.SelectedItem;
            if (this.ClearSelectedClinics != null)
            {
                this.ClearSelectedClinics(this, new LooseArgs(this.Model.KindOfVisit));
            }

            if (!this.blnLeaveRun)
            {
                RuleEngine.GetInstance().EvaluateRule(typeof(InvalidPatientTypeCode), this.Model);
                RuleEngine.GetInstance().EvaluateRule(typeof(InvalidPatientTypeCodeChange), this.Model);
            }

            RuleEngine.GetInstance().EvaluateRule(typeof(PatientTypeRequired), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(HospitalServiceRequired), this.Model);
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServicePreferred ), this.Model );
            RuleEngine.GetInstance().EvaluateRule(typeof(DiagnosisClinicOneRequired), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(AccomodationRequired), this.Model);

            this.Model.HospitalService = (HospitalService)this.cmbHospitalService.SelectedItem;

            if (this.Model.HospitalService == null)
            {
                this.Model.HospitalService = new HospitalService();
            }

            if (this.Model.KindOfVisit != null && this.Model.HospitalService != null)
            {
                CheckForActivityAndPTSelected(this.Model.KindOfVisit.Code,
                    this.Model.HospitalService.Code);
            }
        }

        private void locationView_BedSelected(object sender, EventArgs e)
        {
            this.Model.Location = locationView.Model.Location;
            this.setAccomodation();

            if ((this.Model.Activity is AdmitNewbornActivity ||
                this.Model.Activity is RegistrationActivity)
                && this.Model.KindOfVisit != null
                && this.Model.KindOfVisit.Code == VisitType.INPATIENT)
            {
                this.cmbAccommodation.Enabled = true;
                if (this.Model.Location.Bed.Accomodation != null)
                {
                    this.cmbAccommodation.SelectedItem = this.Model.Location.Bed.Accomodation;
                }
            }
            else
            {
                this.cmbAccommodation.Enabled = false;
            }

            RuleEngine.GetInstance().EvaluateRule(typeof(AccomodationRequired), this.Model);
        }

        private void cbxReregister_CheckedChanged(object sender, EventArgs e)
        {
            YesNoFlag yesNoFlag = new YesNoFlag("N");
            yesNoFlag.SetNo();
            if (this.cbxReregister.Checked)
            {
                yesNoFlag.SetYes();
            }
            this.Model.Reregister = yesNoFlag;
        }

        private void cmbAccommodation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( Model.Activity == null || !( Model.Activity.IsNewBornRelatedActivity()))
            {
                PrivateRoomConditionCodeHelper privateRoomConditionCodeHelper = new PrivateRoomConditionCodeHelper();
                selectedAccomodation = this.cmbAccommodation.SelectedItem as Accomodation;
                if (selectedAccomodation != null)
                {
                    privateRoomConditionCodeHelper.AddConditionCodeForPrivateRoomMedicallyNecessary(selectedAccomodation, this.Model);

                    if (this.Model.IsReasonForAccommodationRequiredForSelectedActivity()
                    && this.Model.Location.Bed.Accomodation.IsReasonRequiredForSelectedAccommodation())
                    {
                        this.cmbReasonForPrivateAccommodation.Enabled = true;
                        PopulateReasonsForPrivateAccommodation();
                    }
                    else
                    {
                        this.cmbReasonForPrivateAccommodation.SelectedIndex = -1;
                        this.cmbReasonForPrivateAccommodation.Enabled = false;
                        this.Model.Diagnosis.isPrivateAccommodationRequested = false;
                        if (!this.Model.Location.Bed.Accomodation.IsPrivateRoomMedicallyNecessary())
                        {
                            privateRoomConditionCodeHelper.RemovePrivateRommConditionCodes(this.Model);
                        }
                    }
                    if (selectedAccomodation.Description.Length > 0)
                    {
                        Model.Location.Bed.Accomodation.IsReasonForAccommodationSelected = false;
                    }
                    else
                    {
                        Model.Location.Bed.Accomodation.IsReasonForAccommodationSelected = true;
                    }
                    UIColors.SetNormalBgColor(this.cmbReasonForPrivateAccommodation);
                    RuleEngine.GetInstance().EvaluateRule(typeof(ReasonForAccomodationRequired), this.Model);
                }
            }
        }

        private void cmbAccommodation_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(this.cmbAccommodation);
            this.Refresh();

            if (this.Model.Location != null &&
                this.Model.Location.Bed != null)
            {
                this.Model.Location.Bed.Accomodation = (Accomodation)this.cmbAccommodation.SelectedItem;
            }
            if (!this.blnLeaveRun)
            {
                RuleEngine.GetInstance().EvaluateRule(typeof(InvalidAccomodationCode), this.Model);
                RuleEngine.GetInstance().EvaluateRule(typeof(InvalidAccomodationCodeChange), this.Model);
            }
            RuleEngine.GetInstance().EvaluateRule(typeof(AccomodationRequired), this.Model);
        }

        private void cmbReasonForPrivateAccommodation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadData)
            {
                PrivateRoomConditionCodeHelper privateRoomConditionCode = new PrivateRoomConditionCodeHelper();
                if (this.cmbReasonForPrivateAccommodation.SelectedItem != null)
                {
                    privateRoomConditionCode.AddPrivateRoomConditionCode(((ReasonForAccommodation)this.cmbReasonForPrivateAccommodation.SelectedItem).Oid, this.Model);
                }
                else
                {
                    Model.Location.Bed.Accomodation.IsReasonForAccommodationSelected = false;
                }
            }
        }

        private void cmbReasonForPrivateAccommodation_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(this.cmbReasonForPrivateAccommodation);
            this.Refresh();
            RuleEngine.GetInstance().EvaluateRule(typeof(ReasonForAccomodationRequired), this.Model);
        }

        #endregion

        #region Private Methods

        private void PopulateReasonsForPrivateAccommodation()
        {
            ReasonForAccommodation reasonForAccommodation = new ReasonForAccommodation();
            ICollection reasonForAccommodationList = reasonForAccommodation.PopluateReasonForAccommodation();
            cmbReasonForPrivateAccommodation.Items.Clear();

            if (reasonForAccommodationList.Count > 0)
            {
                foreach (ReasonForAccommodation reasonForAccomm in reasonForAccommodationList)
                {
                    cmbReasonForPrivateAccommodation.Items.Add(reasonForAccomm);
                }
            }
        }

        private void SetKindOfVisit()
        {
            if (this.Model.Activity != null &&
                this.Model.Activity.AssociatedActivityType != null &&
                this.Model.Activity.AssociatedActivityType.GetType().Equals(typeof(ActivatePreRegistrationActivity)) &&
                this.Model.KindOfVisit != null &&
                this.Model.KindOfVisit.Code == VisitType.PREREG_PATIENT)
            {
                this.Model.KindOfVisit = (VisitType)this.PatientTypes[0];
            }
        }

        private void EnableHospitalService()
        {
            if (Model.Activity!=null && Model.Activity.IsNewBornRelatedActivity() ||
                   Model.Activity is RegistrationActivity)
            {
                if (Model.KindOfVisit != null && Model.KindOfVisit.Code != String.Empty)
                {
                    cmbHospitalService.Enabled = true;
                }
            }
            if (Model.Activity != null && Model.Activity.IsUccPostMSEActivity())
            {
                if (Model.KindOfVisit != null && Model.KindOfVisit.Code != String.Empty)
                {
                    cmbHospitalService.Enabled = false;
                }
            }
        }

        private void SetPrivateRoomAccommodationCodes()
        {
            foreach (ConditionCode conditionCode in this.Model.ConditionCodes)
            {
                if (conditionCode.Code == ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED)
                {
                    this.cmbReasonForPrivateAccommodation.SelectedIndex = this.cmbReasonForPrivateAccommodation.FindStringExact(PRIVATE_ROOM_MEDICALLY_NECESSARY);
                    Model.Location.Bed.Accomodation.IsReasonForAccommodationSelected = true;
                    isAccommodationPatientRequested = false;
                }
                else if (conditionCode.Code == ConditionCode.CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE)
                {
                    this.cmbReasonForPrivateAccommodation.SelectedIndex = this.cmbReasonForPrivateAccommodation.FindStringExact(SEMI_PRIVATE_ROOM_NOT_AVAILABLE);
                    Model.Location.Bed.Accomodation.IsReasonForAccommodationSelected = true;
                    isAccommodationPatientRequested = false;
                }
            }

            // OTD# 37630 fix - Make an assumption that if Reason for Private Accommodation is not 
            // "Medically necessary" nor "Semi-private room unavailable", then its "Patient requested…" 
            // This assumption is made only when -
            // 1) We are loading account data for the first time (through UpdateView) AND
            // 2) Activity is MaintenanceActivity AND
            // 3) Saved Accommodation is 'PRIVATE' AND
            // 4) Saved Reason for Accommodation is NOT 'Medically necessary' or 'Semi-private room unavailable'
            if (isLoadData &&
                this.Model.Activity is MaintenanceActivity &&
                this.Model.Location.Bed.Accomodation.IsReasonRequiredForSelectedAccommodation() &&
                isAccommodationPatientRequested)
            {
                this.cmbReasonForPrivateAccommodation.SelectedIndex = this.cmbReasonForPrivateAccommodation.FindStringExact(PATIENT_REQUESTED_PRIVATE_ROOM);
                Model.Location.Bed.Accomodation.IsReasonForAccommodationSelected = true;
            }
            else
            {
                if (this.Model.Diagnosis.isPrivateAccommodationRequested)
                {
                    this.cmbReasonForPrivateAccommodation.SelectedIndex = this.cmbReasonForPrivateAccommodation.FindStringExact(PATIENT_REQUESTED_PRIVATE_ROOM);
                    Model.Location.Bed.Accomodation.IsReasonForAccommodationSelected = true;
                }
            }

            // OTD# 37499 fix - Do not enable 'Reason for Private Accommodation' if already selected    
            if (cmbReasonForPrivateAccommodation.SelectedIndex != -1)
            {
                this.cmbReasonForPrivateAccommodation.Enabled = false;
            }
            else
            {
                // If not selected already, enable it only for 'PRIVATE' accomodation code
                if (this.Model.IsReasonForAccommodationRequiredForSelectedActivity() &&
                    this.Model.Location.Bed.Accomodation.IsReasonRequiredForSelectedAccommodation())
                {
                    this.cmbReasonForPrivateAccommodation.Enabled = true;
                }
                else
                {
                    this.cmbReasonForPrivateAccommodation.Enabled = false;
                }
            }

            UIColors.SetNormalBgColor(this.cmbReasonForPrivateAccommodation);
            RuleEngine.GetInstance().EvaluateRule(typeof(ReasonForAccomodationRequired), this.Model);
        }

        private void SetAccommodationCodes()
        {
            if (this.Model.Location != null
                   && this.Model.Location.IsLocationAssigned()
                   && this.Model.Location.Bed.Accomodation != null)
            {
                PrivateRoomConditionCodeHelper privateRoomConditionCodeHelper = new PrivateRoomConditionCodeHelper();
                this.cmbAccommodation.Enabled = privateRoomConditionCodeHelper.EnableAccommodation(this.Model);

                if (this.Model.Activity is MaintenanceActivity)
                {
                    this.cmbAccommodation.SelectedItem = this.Model.Location.Bed.Accomodation;
                }
                SetPrivateRoomAccommodationCodes();
            }
        }

        private bool SetReregister()
        {
            bool isVisible = true;
            if (this.Model.Activity != null
                && this.Model.Activity.AssociatedActivityType != null
                && this.Model.Activity.AssociatedActivityType.ToString().Contains("ActivatePreRegistrationActivity"))
            {
                isVisible = false;
            }
            return isVisible;
        }

        private void UpdateHospitalService()
        {
            if (this.Model.AccountNumber == 0 ||
                (this.Model.IsNew && Model.Activity is RegistrationActivity) ||
                (this.Model.Activity.AssociatedActivityType != null && 
                    this.Model.Activity.AssociatedActivityType == typeof(ActivatePreRegistrationActivity)))
            {
                return;
            }
            else
            {
                if (Model.HospitalService != null
                    && Model.HospitalService.Code != HospitalService.BLANK_CODE)
                {
                    this.savedHospitalService = Model.HospitalService;
                }
            }
        }

        private void IsReregisterFacility()
        {
            if (this.Model.KindOfVisit == null
                || this.Model.KindOfVisit.Code != VisitType.RECURRING_PATIENT)
            {
                return;
            }

            if (this.Model.Facility.Reregister.Code == YesNoFlag.CODE_NO &&
                this.Model.Reregister != null &&
                this.Model.Reregister.Code == YesNoFlag.CODE_BLANK)
            {
                this.cbxReregister.Visible = false;
            }
            else
            {
                this.SetReregisterStatus(true);

                if (this.Model.Facility.Reregister.Code != YesNoFlag.CODE_YES)
                {
                    this.cbxReregister.Enabled = false;
                }
            }
        }

        private void SetReregisterStatus(bool isVisible)
        {
            this.cbxReregister.Visible = isVisible;

            if (this.Model.Reregister != null)
            {
                this.cbxReregister.Checked = (this.Model.Reregister.Code != YesNoFlag.CODE_NO);
            }
        }

        private void cmbHospitalService_Validating(object sender, CancelEventArgs e)
        {
            locationView.RunRules();

            UIColors.SetNormalBgColor(this.cmbHospitalService);
            this.Model.HospitalService = (HospitalService)this.cmbHospitalService.SelectedItem;

            if (!this.blnLeaveRun)
            {
                RuleEngine.GetInstance().EvaluateRule(typeof(InvalidHospitalServiceCode), this.Model);
                RuleEngine.GetInstance().EvaluateRule(typeof(InvalidHospitalServiceCodeChange), this.Model);
            }
            RuleEngine.GetInstance().EvaluateRule(typeof(HospitalServiceRequired), this.Model);
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServicePreferred ), this.Model );
            RuleEngine.GetInstance().EvaluateRule(typeof(PatientTypeRequired), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(DiagnosisClinicOneRequired), this.Model);
        }

        /// <summary>
        /// Combo Box Hospital Service Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbHospitalService_SelectedIndexChanged(object sender, EventArgs e)
        {
            HospitalService hospitalSrvc = cmbHospitalService.SelectedItem as HospitalService;
            HospitalService selectedHSV = hospitalSrvc;
            HospitalService savedHSV = this.Model.HospitalService;
            // don't reset if the selected index changed event was fired on assignment of the model value
            if (
                (
                this.cmbHospitalService.SelectedIndex > 0
                && this.Model.HospitalService != null
                && ((HospitalService)this.cmbHospitalService.SelectedItem).Code != this.Model.HospitalService.Code
                )
                ||
                (
                this.cmbHospitalService.SelectedIndex < 1
                && this.Model.HospitalService != null
                && this.Model.HospitalService.Code != string.Empty
                )
                )
            {

       
                // Do not Release reserved Bed for MaintenanceActivity (AND)
                // Release it for all Patient types during Registration for HSV change
                if (!(this.Model.Activity is MaintenanceActivity)
                    //&& this.Model.KindOfVisit.Code != VisitType.INPATIENT
                    )
                {
                    this.locationView.ReleaseReservedBed();
                    this.Model.Location = null;
                    ResetAll();
                }

                if (hospitalSrvc != null
                    && this.Model.KindOfVisit != null)
                {
                    this.Model.HospitalService = hospitalSrvc;
                    CheckForActivityAndPTSelected(this.Model.KindOfVisit.Code, this.Model.HospitalService.Code);
                }

                PrivateRoomConditionCodeHelper privateRoomConditionCodeHelper = new PrivateRoomConditionCodeHelper();
                this.cmbAccommodation.Enabled = privateRoomConditionCodeHelper.EnableAccommodation(this.Model);
            }
            else
            {
                if (facilityHospitalServices != null
                    && facilityHospitalServices.Count > 0)
                {
                    if (hospitalSrvc != null
                        && this.Model.KindOfVisit != null)
                    {
                        this.Model.HospitalService = hospitalSrvc;
                        CheckForActivityAndPTSelected(this.Model.KindOfVisit.Code, this.Model.HospitalService.Code);
                    }
                }
            }

            FireHSVChanged();

            Model.RemoveOccurrenceCode50IfNotApplicable();
            hIEShareDataFlagView.PatientAccount = Model;
            if (selectedHSV.Code != savedHSV.Code)
            {
                HIEShareDataPresenter.HandleNotifyPCP();
            }
           
        }
       
        /// <summary>
        ///  PopulatePatientTypes - from the list of all patient types, derive those applicable based on
        /// the activity
        /// </summary>
        private void PopulatePatientTypes()
        {
            if (cmbPatientType.Items.Count == 0)
            {
                cmbPatientType.Items.Clear();

                foreach (VisitType patType in this.PatientTypes)
                {
                    cmbPatientType.Items.Add(patType);
                }

                // if there is only 1 PT in the list (other than the blank entry), default it
                if (cmbPatientType.Items.Count == 1)
                {
                    //this.cmbPatientType.SelectedItem = patientTypes[0];
                    this.cmbPatientType.SelectedIndex = 0;
                }

                if (this.Model.KindOfVisit != null)
                {
                    this.cmbPatientType.Enabled = this.Model.IsPatientTypeChangeable();
                    this.cmbPatientType.SelectedItem = this.Model.KindOfVisit;
                }
            }
        }

        private void SetNormalColor()
        {
            UIColors.SetNormalBgColor(this.cmbAccommodation);
            UIColors.SetNormalBgColor(this.cmbHospitalService);
            UIColors.SetNormalBgColor(this.cmbPatientType);
            UIColors.SetNormalBgColor(this.cmbReasonForPrivateAccommodation);
        }

        private void CheckForActivityAndPTSelected(string patientTypeCode, string hsvCode)
        {
            this.PopulateLocation();
        }

        private void ResetAll()
        {
            locationView.Reset();

            UIColors.SetNormalBgColor(this.cmbAccommodation);
            cmbAccommodation.ResetText();
            cmbAccommodation.Items.Clear();
            cmbAccommodation.Enabled = false;
            UIColors.SetNormalBgColor(this.cmbReasonForPrivateAccommodation);
            cmbReasonForPrivateAccommodation.ResetText();
            cmbReasonForPrivateAccommodation.Items.Clear();
            cmbReasonForPrivateAccommodation.Enabled = false;
            if (!(isLoadData))
            {
                PrivateRoomConditionCodeHelper privateRoomConditionCodeHelper = new PrivateRoomConditionCodeHelper();
                privateRoomConditionCodeHelper.RemovePrivateRommConditionCodes(this.Model);
            }
        }

        private void PopulateLocation()
        {
            this.locationView.TabStop = false;

            if (this.Model.Activity is MaintenanceActivity)
            {
                locationView.Model = this.Model;
                locationView.UpdateView();
                
                locationView.DisableLocationControls(); // OTD 36179
                if (this.Model.Location != null && this.Model.Location.Bed != null && this.Model.Location.Bed.Code != string.Empty)
                {
                    this.locationView.field_AssignedBed.Text = this.Model.Location.FormattedLocation;
                }
                return;
            }

            bool isValidPatientTypeForLocation = false;

            if (this.Model.Activity is RegistrationActivity || this.Model.Activity is AdmitNewbornActivity)
            {
                isValidPatientTypeForLocation = this.IsPatientTypeValidForLocation(this.Model.HospitalService, this.Model.KindOfVisit, this.cmbHospitalService.SelectedItem as HospitalService, this.Model.Activity);
            }

            if (isValidPatientTypeForLocation)
            {
                locationView.Model = this.Model;
                locationView.UpdateView();
                this.locationView.EnableLocationControls();
                this.locationView.TabStop = true;
            }
            else if(Model.Activity.IsPreAdmitNewbornActivity())
            {
                //SR 1557, disable location view when register a Pre-Admit Newborn account
                locationView.DisableLocationControls();
            }
        }

        private bool EnableHospitalService(HospitalService hospitalService)
        {
            if (hospitalService != null && HospitalService.CheckForBedAssignment(hospitalService.Code))
            {
                return true;
            }
            return false;
        }

        internal bool IsPatientTypeValidForLocation(HospitalService hospitalService, VisitType kindOfVisit, HospitalService selectedHospitalSevice, Activity activity)
        {
            if (kindOfVisit == null)
            {
                return false;
            }

            bool hospitalServiceValid = this.EnableHospitalService(selectedHospitalSevice);
            string kindOfVistCode = kindOfVisit.Code;

            if ((kindOfVistCode == VisitType.INPATIENT && hospitalService != null && hospitalService.Code.Length > 0) ||
                ((kindOfVistCode == VisitType.OUTPATIENT || kindOfVistCode == VisitType.RECURRING_PATIENT ||
                kindOfVistCode == VisitType.NON_PATIENT) && hospitalServiceValid) ||
                (activity is AdmitNewbornActivity &&
                kindOfVistCode == VisitType.INPATIENT && hospitalService != null) )
            {
                return true;
            }
            return false;
        }

        private void PopulateHsvCodes()
        {
            ArrayList hospitalServices = this.GetHospitalServiceCodes(User.GetCurrent().Facility.Oid, this.Model.KindOfVisit, this.Model.Activity, this.Model.HospitalService, this.Model.FinancialClass);

            if (hospitalServices != null && hospitalServices.Count > 0)
            {
                foreach (HospitalService hs in hospitalServices)
                {
                    cmbHospitalService.Items.Add(hs);
                }

                this.cmbHospitalService.Sorted = true;

                VisitType inVisitType = (VisitType)this.cmbPatientType.SelectedItem;

                // OTD# 37073 fix - If PatientType has changed from OUTPATIENT(Type=2) to 
                // OP-ER PATIENT(Type=3), clear previously specified Hospital Service value
                if (savedPatientType != null && savedPatientType.Code != null
                    && savedPatientType.Code != string.Empty && savedPatientType.Code == VisitType.OUTPATIENT &&
                    inVisitType != null && inVisitType.Code != null &&
                    inVisitType.Code != string.Empty && inVisitType.Code == VisitType.EMERGENCY_PATIENT)
                {
                    this.cmbHospitalService.SelectedIndex = 0;
                }
                else
                {
                    if (this.Model.HospitalService != null)
                    {
                        this.cmbHospitalService.SelectedItem = this.Model.HospitalService;
                    }
                }
                RuleEngine.GetInstance().RegisterEvent(typeof(HospitalServiceRequired), new EventHandler(HospitalServiceRequiredEventHandler));
                RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServicePreferred ), new EventHandler( HospitalServicePreferredEventHandler ) );
            }

            if (this.cmbPatientType.SelectedItem != null)
            {
                this.cmbHospitalService.Enabled =  (this.Model.KindOfVisit != null && this.Model.KindOfVisit.Code != String.Empty);
            }
        }


        internal ArrayList GetHospitalServiceCodes(long facilityOid, VisitType kindOfVisit, Activity activity, HospitalService service, FinancialClass financialClass)
        {
            ArrayList hospitalServices = null;

            this.cmbHospitalService.Items.Clear();

            if (this.facilityHospitalServices == null)
            {
                HSVBrokerProxy brokerProxy = new HSVBrokerProxy();
                this.facilityHospitalServices = (ArrayList)brokerProxy.SelectHospitalServicesFor(facilityOid);
            }

            HospitalService hospitalService = new HospitalService();

            if ( activity is AdmitNewbornActivity || activity is PreAdmitNewbornActivity )
                    
            {
                hospitalServices = (ArrayList)hospitalService.HospitalServicesFor(this.facilityHospitalServices,
                                                                                  kindOfVisit, activity, service,
                                                                                  financialClass);
            }
            else if ( activity is MaintenanceActivity && activity.AssociatedActivityType != null 
                    && activity.AssociatedActivityType == typeof( PreAdmitNewbornActivity ) )
            {
                //This is to Edit a Pre-Admit Newborn Acccount
                hospitalServices = (ArrayList)hospitalService.HospitalServicesFor( this.facilityHospitalServices,
                                                                                  kindOfVisit, new PreAdmitNewbornActivity(), service,
                                                                                  financialClass );
            }
            else if ( activity is RegistrationActivity && kindOfVisit.IsEmergencyPatient )
            {
                hospitalServices = (ArrayList)hospitalService.HospitalServicesFor( this.facilityHospitalServices, kindOfVisit.Code );
            }
            else
            {
                VisitType inVisitType = (VisitType)this.cmbPatientType.SelectedItem;
                if ( inVisitType != null && inVisitType.Code != null && inVisitType.Code != string.Empty )
                {
                    if ( inVisitType.Code != VisitType.INPATIENT &&
                        inVisitType.Code != VisitType.PREREG_PATIENT &&
                        (
                            ( this.savedHospitalService != null && this.savedHospitalService.Code != string.Empty ) ||
                            ( service != null && service.Code != string.Empty )
                        )
                        )
                    {
                        string dayCare = string.Empty;
                        if ( this.savedHospitalService != null &&
                            this.savedHospitalService.Code != string.Empty )
                        {
                            dayCare = this.savedHospitalService.DayCare;
                        }
                        else
                        {
                            dayCare = service.DayCare;
                        }

                        if ( dayCare == null )
                        {
                            dayCare = "N";
                        }
                        hospitalServices = (ArrayList)hospitalService.HospitalServicesFor( this.facilityHospitalServices,
                                                                                          inVisitType.Code, dayCare );
                    }
                    else
                    {
                        hospitalServices = (ArrayList)hospitalService.HospitalServicesFor( this.facilityHospitalServices,
                                                                                          kindOfVisit, activity, service,
                                                                                          financialClass );
                    }
                }
            }
            return hospitalServices;
        }


        private void getAccomodationCodes()
        {
            this.DoGetAccomodationCodes();
        }

        private void DoGetAccomodationCodes()
        {
            LocationBrokerProxy lpbProxy = new LocationBrokerProxy();

            this.accomodationCodes = (ArrayList)lpbProxy.AccomodationCodesFor(
                NursingStationCode, User.GetCurrent().Facility);

            this.nursingStationCodeForAccomodations = NursingStationCode;
        }

        private void setAccomodation()
        {
            if (this.Model.Activity is PreRegistrationActivity ||
                 this.Model.Activity is PostMSERegistrationActivity ||
                this.Model.Activity is MaintenanceActivity ||
                (this.Model.Activity is RegistrationActivity &&
                   this.Model.KindOfVisit != null && this.Model.KindOfVisit.Code != VisitType.INPATIENT) ||
                ( this.Model.Activity is AdmitNewbornActivity  &&
                   this.Model.KindOfVisit != null && this.Model.KindOfVisit.Code != VisitType.INPATIENT) )
            {
                this.cmbAccommodation.Items.Clear();
                this.cmbAccommodation.Enabled = false;
            }

            if (this.Model.Location != null &&
                this.Model.Location.NursingStation != null)
            {
                this.LoadAccomodationCodes(this.Model.Location.NursingStation.Code);

                if (this.Model.Location.Bed.Accomodation != null && this.Model.HospitalService != null
                    && this.Model.HospitalService.Code.Length > 0)
                {
                    this.cmbAccommodation.SelectedItem = this.Model.Location.Bed.Accomodation;
                }
            }
        }

        private void unregisterEvents()
        {
            this.i_Registered = false;

            RuleEngine.GetInstance().UnregisterEvent(typeof(PatientTypeRequired), new EventHandler(PatientTypeRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(HospitalServiceRequired), new EventHandler(HospitalServiceRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent( typeof( HospitalServicePreferred ), new EventHandler( HospitalServicePreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent(typeof(AccomodationRequired), new EventHandler(AccomodationRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(InvalidAccomodationCode), new EventHandler(InvalidAccomodationCodeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(InvalidHospitalServiceCode), new EventHandler(InvalidHospitalServiceCodeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(InvalidPatientTypeCode), new EventHandler(InvalidPatientTypeCodeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(InvalidAccomodationCodeChange), new EventHandler(InvalidAccomodationCodeChangeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(InvalidHospitalServiceCodeChange), new EventHandler(InvalidHospitalServiceCodeChangeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(InvalidPatientTypeCodeChange), new EventHandler(InvalidPatientTypeCodeChangeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(ReasonForAccomodationRequired), new EventHandler(ReasonForAccomodationRequiredEventHandler));
        }

        private void registerEvents()
        {
            if (i_Registered)
            {
                return;
            }

            i_Registered = true;

            RuleEngine.GetInstance().RegisterEvent(typeof(PatientTypeRequired), new EventHandler(PatientTypeRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(HospitalServiceRequired), new EventHandler(HospitalServiceRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServicePreferred ), new EventHandler( HospitalServicePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent(typeof(AccomodationRequired), new EventHandler(AccomodationRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(InvalidAccomodationCode), new EventHandler(InvalidAccomodationCodeEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(InvalidHospitalServiceCode), new EventHandler(InvalidHospitalServiceCodeEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(InvalidPatientTypeCode), new EventHandler(InvalidPatientTypeCodeEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(InvalidAccomodationCodeChange), new EventHandler(InvalidAccomodationCodeChangeEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(InvalidHospitalServiceCodeChange), new EventHandler(InvalidHospitalServiceCodeChangeEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(InvalidPatientTypeCodeChange), new EventHandler(InvalidPatientTypeCodeChangeEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(ReasonForAccomodationRequired), new EventHandler(ReasonForAccomodationRequiredEventHandler));
        }
      
        #endregion

        #region Properties
        public new Account Model
        {
            get
            {
                return (Account)base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public string NursingStationCode
        {
            private get
            {
                return this.i_NursingStationCode;
            }
            set
            {
                this.i_NursingStationCode = value;
            }
        }

        private ArrayList AccomodationCodes
        {
            get
            {
                if (this.accomodationCodes == null
                    || this.accomodationCodes.Count <= 1
                    || this.NursingStationCode != this.nursingStationCodeForAccomodations)
                {
                    this.getAccomodationCodes();
                }

                return this.accomodationCodes;
            }
        }

        private ArrayList PatientTypes
        {
            get
            {
                if (i_PatientTypes == null)
                {
                    string activityType = string.Empty;
                    string associatedActivityType = string.Empty;
                    string kindOfVisitCode = string.Empty;
                    string financialClassCode = string.Empty;
                    string locationBedCode = string.Empty;

                    Activity activity = this.Model.Activity;
                    if (activity != null)
                    {
                        activityType = activity.GetType().ToString();
                        if (activity.AssociatedActivityType != null)
                        {
                            associatedActivityType = activity.AssociatedActivityType.ToString();
                        }
                    }
                    if (this.Model.KindOfVisit != null)
                    {
                        kindOfVisitCode = this.Model.KindOfVisit.Code;
                    }
                    if (this.Model.FinancialClass != null)
                    {
                        financialClassCode = this.Model.FinancialClass.Code;
                    }
                    if (this.Model.Location != null &&
                        this.Model.Location.Bed != null)
                    {
                        locationBedCode = this.Model.Location.Bed.Code;
                    }

                    IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
                    if (Model.IsPAIWalkinRegistered)
                    {
                        i_PatientTypes = patientBroker.PatientTypesForWalkinAccount(Model.Facility.Oid);
                    }
                    else if ( Model.IsUrgentCarePreMse || Model.Activity.GetType() == typeof(UCCPostMseRegistrationActivity) )
                    {
                        i_PatientTypes = patientBroker.PatientTypesForUCCAccount(Model.Facility.Oid);
                    }
                    else
                    {
                        i_PatientTypes = patientBroker.PatientTypesFor(activityType, associatedActivityType,
                            kindOfVisitCode,
                            financialClassCode, locationBedCode, this.Model.Facility.Oid);
                    }
                }

                return i_PatientTypes;
            }
        }

        private IHIEShareDataPresenter HIEShareDataPresenter
        {
            get
            {
                return hIEConsentPresenter = HIEShareDataPresenterFactory.GetPresenter(hIEShareDataFlagView,
                    new HIEConsentFeatureManager(),
                    Model);
            }
        }

        public static Boolean IsHospitalServiceSelected
        {
            get
            {
                return false;
            }
            set
            {
                IsHospitalServiceSelected = value;
            }
        }

        public static Boolean IsPTSelected
        {
            get
            {
                return false;
            }
            set
            {
                IsPTSelected = value;
            }
        }
        #endregion

        #region Data Elements
        ShareHIEDataFeatureManager shareHieDataFeatureManager = new ShareHIEDataFeatureManager();
        NotifyPCPDataFeatureManager notifyPCPFeatureManager = new NotifyPCPDataFeatureManager();
        private IHIEShareDataPresenter hIEConsentPresenter;
        private HIEShareDataFlagView hIEShareDataFlagView =new PatientAccess.UI.RegulatoryViews.ViewImpl.HIEShareDataFlagView();
        private HospitalService savedHospitalService;
        private VisitType selectedPatientType;
        private VisitType savedPatientType;
        private bool blnLeaveRun = false;
        private bool isLoadData = true;
        private bool i_Registered = false;
        private bool isAccommodationPatientRequested = true;
        private ArrayList accomodationCodes;
        private ArrayList i_PatientTypes;
        private ArrayList facilityHospitalServices;
        private Accomodation accomodation;
        private Accomodation selectedAccomodation;
        string i_NursingStationCode;
        string nursingStationCodeForAccomodations;
        private const string PRIVATE_ROOM_MEDICALLY_NECESSARY = "Private room is medically necessary";
        private const string SEMI_PRIVATE_ROOM_NOT_AVAILABLE = "Semi-private room is not available";
        private const string PATIENT_REQUESTED_PRIVATE_ROOM = "Patient requested a private room";
        #endregion
    }
}
