using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Email.Presenters;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.ViewImpl;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;

namespace PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient.ViewImpl
{
    public class EmergencyPatientToOutPatientStep1View : ControlView, IEmergencyPatientToOutPatientStep1View,
                                                         IAlternateCareFacilityView
    {
        #region Events

        public event EventHandler CancelButtonClicked;
        public event EventHandler NextButtonClicked;

        #endregion

        #region Event Handlers

        public void SetAdmitSourceRequired()
        {
            UIColors.SetRequiredBgColor(cboAdmitSource);
        }

        public void SetAlternateCareFacilityRequired()
        {
            cmbAlternateCareFacility.Enabled = true;
            UIColors.SetRequiredBgColor(cmbAlternateCareFacility);
        }

        public void SetHospitalServiceRequired()
        {
            UIColors.SetRequiredBgColor(cboHospitalService);
            locationView1.DisableLocationControls();
        }
        public void SetClinicOneRequired()
        {
            UIColors.SetRequiredBgColor(cboClinic1);
        }

        public void SetLocationRequired()
        {
            UIColors.SetRequiredBgColor(locationView1.field_AssignedBed);
        }

        public void SetChiefComplaintRequired()
        {
            UIColors.SetRequiredBgColor(mtbChiefComplaint);
        }

        public void SetTransferDateRequired()
        {
            UIColors.SetRequiredBgColor(mtbTransferDate);
        }

        public void SetTransferTimeRequired()
        {
            UIColors.SetRequiredBgColor(mtbTransferTime);
        }

        public void SetEmailAddressRequired()
        {
            UIColors.SetRequiredBgColor(mtbEmail);
        }
        public void DisplayTransferDateIsFutureDateMessage()
        {
            UIColors.SetErrorBgColor(mtbTransferDate);

            MessageBox.Show(UIErrorMessages.TRANSFER_DATE_IN_FUTURE_MSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1);
            dateTimePicker.Focus();
        }

        public void DisplayTransferTimeIsFutureTimeMessage(EventArgs e)
        {
            if (e != null)
            {
                var args = (PropertyChangedArgs) e;
                var aControl = args.Context as string;

                if (aControl != null && aControl.Equals(EmergencyPatientToOutPatientPresenter.TRANSFER_DATE))
                {
                    UIColors.SetErrorBgColor(mtbTransferDate);
                    dateTimePicker.Focus();

                    MessageBox.Show(UIErrorMessages.TRANSFER_DATE_IN_FUTURE_MSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                    inErrorState = true;
                }
                else
                {
                    UIColors.SetErrorBgColor(mtbTransferTime);
                    if (!dateTimePicker.Focused)
                    {
                        mtbTransferTime.Focus();
                    }

                    MessageBox.Show(UIErrorMessages.TRANSFER_TIME_IN_FUTURE_MSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                    inErrorState = true;
                }
            }
        }

        public void DisplayTransferDateTimeBeforeAdmitDateTimeMessage(EventArgs e)
        {
            if (e != null)
            {
                var args = (PropertyChangedArgs) e;
                var aControl = args.Context as string;

                if (aControl != null  && aControl.Equals(EmergencyPatientToOutPatientPresenter.TRANSFER_DATE))
                {
                    UIColors.SetErrorBgColor(mtbTransferDate);
                    if (!dateTimePicker.Focused)
                    {
                        mtbTransferDate.Focus();
                        MessageBox.Show(UIErrorMessages.TRANSFER_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1);
                    }
                }
                else
                {
                    UIColors.SetErrorBgColor(mtbTransferTime);
                    if (!dateTimePicker.Focused)
                    {
                        mtbTransferTime.Focus();
                    }

                    MessageBox.Show(UIErrorMessages.TRANSFER_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                }
            }
        }

      

        private void TransferERPatToOutPatStep1View_Validating(object sender, CancelEventArgs e)
        {
            inErrorState = false;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (inErrorState == false)
            {
                if (NextButtonClicked != null)
                {
                    NextButtonClicked(this, new LooseArgs(Model));
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (AccountActivityService.ConfirmCancelActivity(
                sender, new LooseArgs(Model)))
            {
                TransferERPatToOutPatView_LeaveView(sender, e);
                if (CancelButtonClicked != null)
                {
                    CancelButtonClicked(this, new EventArgs());
                }
            }
        }

        private void cboAdmitSource_SelectIndexChanged(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb != null && cb.SelectedIndex != -1)
            {
                UIColors.SetNormalBgColor(cboAdmitSource);
                if (EmergencyPatientToOutPatientPresenter != null)
                {
                    EmergencyPatientToOutPatientPresenter.HandleAdmitSourceSelectedIndexChanged(
                        cb.SelectedItem as AdmitSource);
                }
            }
        }

        private void cboAdmitSource_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(cboAdmitSource);
            if (EmergencyPatientToOutPatientPresenter != null)
            {
                EmergencyPatientToOutPatientPresenter.HandleAdmitSourceSelectedIndexChanged(
                    cboAdmitSource.SelectedItem as AdmitSource);
            }
            Refresh();
        }

        private void cmbAlternateCareFacility_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(cmbAlternateCareFacility);
            var selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;

            if (selectedAlternateCare != null)
            {
                EmergencyPatientToOutPatientPresenter.UpdateAlternateCareFacility(selectedAlternateCare);
            }
        }

        private void cmbAlternateCareFacility_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(cmbAlternateCareFacility);
            var selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if (selectedAlternateCare != null)
            {
                EmergencyPatientToOutPatientPresenter.UpdateAlternateCareFacility(selectedAlternateCare);
            }
        }

        //selected a HSV
        private void cboHospitalService_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(cboHospitalService);
            var cb = (ComboBox) sender;
            if (cb.SelectedIndex != -1)
            {
                EmergencyPatientToOutPatientPresenter.UpdateHospitalService(
                    (HospitalService) cboHospitalService.SelectedItem);
            }
        }

        private void locationView1_BedSelected(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(locationView1.field_AssignedBed);
            EmergencyPatientToOutPatientPresenter.UpdateBedSelected(locationView1.Model.Location);
        }


        private void mtbChiefComplaint_Validating(object sender, CancelEventArgs e)
        {

            UIColors.SetNormalBgColor(mtbChiefComplaint);
            if (EmergencyPatientToOutPatientPresenter != null)
            {
                EmergencyPatientToOutPatientPresenter.UpdateChiefComplaimt(mtbChiefComplaint.UnMaskedText);
            }
        }

        private void dateTimePicker_CloseUp(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(mtbTransferDate);
            var dt = dateTimePicker.Value;
            mtbTransferDate.Text = String.Format("{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year);
            mtbTransferDate.Focus();
        }

        private void mtbTransferDate_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(mtbTransferDate);
            UIColors.SetNormalBgColor(mtbTransferTime);

            inErrorState = !EmergencyPatientToOutPatientPresenter.UpdateTransferDate();

            Refresh();
        }

        private void mtbTransferTime_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(mtbTransferTime);
            UIColors.SetNormalBgColor(mtbTransferDate);

            inErrorState = false;
            if (!EmergencyPatientToOutPatientPresenter.UpdateTransferTime())
            {
                if (!dateTimePicker.Focused)
                {
                    mtbTransferTime.Focus();
                }
                UIColors.SetErrorBgColor(mtbTransferTime);
                inErrorState = true;
            }
            Refresh();
        }

        private void mtbEmailAddress_Leave(object sender, EventArgs e)
        {
            mtbEmailAddress_Validating(sender, e);
        }

        private void mtbEmailAddress_Validating(object sender, EventArgs e)
        {
            btnNext.Enabled = false;
            var mtb = (MaskedEditTextBox)sender;
            UIColors.SetNormalBgColor(mtb);

            // check if only valid email special characters have been entered or pasted
            if (mtb.Text != string.Empty && emailKeyPressExpression.IsMatch(mtb.Text) == false)
            {   // Prevent cursor from advancing to the next control

                UIColors.SetErrorBgColor(mtb);
                MessageBox.Show(UIErrorMessages.ONLY_VALID_EMAIL_CHARACTERS_ALLOWED, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                mtb.Focus();
                return;
            }

            // check if entered email is in the correct email format
            if (mtb.Text != string.Empty && 
                emailValidationExpression.IsMatch(mtb.Text) == false ||
                EmailAddressPresenter.IsGenericEmailAddress(mtb.Text))
            {
                // Prevent cursor from advancing to the next control                
                UIColors.SetErrorBgColor(mtb);
                MessageBox.Show(UIErrorMessages.EMAIL_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                mtb.Focus();
            }

            else
            {
                ContactPoint mailingContactPoint = Model.Patient.ContactPointWith(
                        TypeOfContactPoint.NewMailingContactPointType());
                mailingContactPoint.EmailAddress = (mtb.Text == String.Empty) ? new EmailAddress() : new EmailAddress(mtb.Text);
                RuleEngine.OneShotRuleEvaluation<EmailAddressRequired>(Model, EmailAddressRequiredEventHandler);
                btnNext.Enabled = true;
            }
        }

        internal void EmailAddressRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(mtbEmail);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
        }

        private void TransferERPatToOutPatView_LeaveView(object sender, EventArgs e)
        {
            Application.Idle -= Application_Idle;
        }

        private void TransferERPatToOutPatStep1View_Disposed(object sender, EventArgs e)
        {
            EmergencyPatientToOutPatientPresenter.UnRegisterRulesEvents();
        }

        #endregion

        #region Methods

        public void EnableNextButton(bool enable)
        {
            btnNext.Enabled = enable;
        }

        public void DisplayInfoMessage(string message)
        {
            infoControl1.DisplayInfoMessage(message);
        }

        public void ShowPanel()
        {
            ProgressPanel1.Visible = true;
            ProgressPanel1.BringToFront();
        }

        public void HidePanel()
        {
            ProgressPanel1.Visible = false;
        }
        private void SetEmailAddressNormalColor(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(mtbEmail);
        }

        public override void UpdateView()
        {
            EmergencyPatientToOutPatientPresenter = new EmergencyPatientToOutPatientStep1Presenter(this, new MessageBoxAdapter(), Model, new AlternateCareFacilityPresenter
                                                                                                                                             (this, new AlternateCareFacilityFeatureManager()), RuleEngine.GetInstance());
            EmergencyPatientToOutPatientPresenter.RegisterRulesEvents();
            PatientPortalOptInPresenter = new PatientPortalOptInPresenter(patientPortalOptInView, new MessageBoxAdapter(), Model,
                 new PatientPortalOptInFeatureManager(), RuleEngine.GetInstance());

            EmergencyPatientToOutPatientPresenter.UpdateView();
            cboAdmitSource.Focus();

            patientPortalOptInView.PortalOptedOutEvent += SetEmailAddressNormalColor;
            EmergencyPatientToOutPatientPresenter.RegisterRulesEvents();
          
            if (Model.Patient != null)
            {
                PatientPortalOptInPresenter.UpdateView();

                if (PatientPortalOptInPresenter.IsFeatureEnabled())
                {
                    MakeEmailAddressVisible();
                    PopulateEmailAddress();
                }
            }
            EmergencyPatientToOutPatientPresenter.RunRules();
        }

        public bool HasUpdatedChiefComplaint()
        {
            return mtbChiefComplaint.UnMaskedText.Trim() != OriginalChiefComplaint;
        }

        public override void UpdateModel()
        {
            Model.LocationTo = Model.Location;
        }

        #endregion

        #region Properties

        private EmergencyPatientToOutPatientStep1Presenter EmergencyPatientToOutPatientPresenter { get; set; }
        private PatientPortalOptInPresenter PatientPortalOptInPresenter { get; set; }

        #region IAlternateCareFacilityView Members

        public new Account Model
        {
            get { return (Account) base.Model; }
            set { base.Model = value; }
        }

        #endregion

        #region IEmergencyPatientToOutPatientStep1View Members

        public string ChiefComplaint
        {
            set { mtbChiefComplaint.UnMaskedText = value; }
        }

        public string UserContextView
        {
            get { return userContextView1.Description; }
            set { userContextView1.Description = value; }
        }

        public PatientContextView PatientContextView1
        {
            get { return patientContextView1; }
            set { patientContextView1 = value; }
        }

        public string PatientName
        {
            set { lblPatientNameVal.Text = value; }
        }

        public string AccountNumber
        {
            set { lblAccountVal.Text = value; }
        }

        public string AdmitDate
        {
            set { lblAdmitDateVal.Text = value; }
        }

        public string AdmitTime
        {
            set { lblAdmitTimeVal.Text = value; }
        }

        public string PrimaryPlan
        {
            set { lblPrimaryPlanVal.Text = value; }
        }

        public string FinancialClass
        {
            set { lblFinancialClassVal.Text = value; }
        }

        public string AdmitSource
        {
            set { lblAdmitSourceVal.Text = value; }
        }

        public string PatientType
        {
            set { lblPatientTypeVal.Text = value; }
        }

        public string HospitalService
        {
            set { lblHospitalServiceVal.Text = value; }
        }

        public string LocationLabel
        {
            set { lblLocationVal.Text = value; }
        }

        public string TransferToPatientType
        {
            set { lblPatientTypeToVal.Text = value; }
        }

        public LocationView LocationView1
        {
            get { return locationView1; }
            set { locationView1 = value; }
        }

        public string TransferDate
        {
            get { return mtbTransferDate.UnMaskedText; }
            set { mtbTransferDate.Text = value; }
        }

        public string TransferTime
        {
            get { return mtbTransferTime.UnMaskedText; }
            set { mtbTransferTime.UnMaskedText = value; }
        }

        public AdmitSource AdmitSourcesSelectedItem
        {
           set { cboAdmitSource.SelectedItem = value; }
        }

        public int AdmitSourcesSelectedIndex
        {
            set { cboAdmitSource.SelectedItem = value; }
        }
        public HospitalClinic HospitalClinicSelectedItem
        {
             set { cboClinic1.SelectedItem = value; }
        }

        public int HospitalClinicsSelectedIndex
        {
            set { cboClinic1.SelectedItem = value; }
        }
        public int HospitalServiceSelectedIndex
        {
            set { cboHospitalService.SelectedItem = value; }
        }

        public HospitalService HospitalServiceSelectedItem
        {
            set { cboHospitalService.SelectedItem = value; }
        }

        public void AddAdmitSources(AdmitSource admitSource)
        {
            cboAdmitSource.Items.Add(admitSource);
        }

        public void AddClinicCode(HospitalClinic clinicCode)
        {
            cboClinic1.Items.Add(clinicCode);
        }

        public void AddHospitalService(HospitalService hospitalService)
        {
            cboHospitalService.Items.Add(hospitalService);
        }
        public void SetHospitalServiceListSorted()
        {
            cboHospitalService.Sorted = true;
        }
        #endregion

        #endregion

        #region private method

        #region IAlternateCareFacilityView Members

        public void PopulateAlternateCareFacility()
        {
            var alternateCareFacilityBroker = BrokerFactory.BrokerOfType<IAlternateCareFacilityBroker>();
            var allAlternateCareFacilities =
                alternateCareFacilityBroker.AllAlternateCareFacilities(User.GetCurrent().Facility.Oid);

            cmbAlternateCareFacility.Items.Clear();

            foreach (var alternateCareFacility in allAlternateCareFacilities)
            {
                cmbAlternateCareFacility.Items.Add(alternateCareFacility);
            }

            // If the value is not in the list, add it as a valid choice. This will
            // prevent data loss in the event that the value stored with the account
            // is removed from the lookup table
            if (!cmbAlternateCareFacility.Items.Contains(Model.AlternateCareFacility))
            {
                cmbAlternateCareFacility.Items.Add(Model.AlternateCareFacility);
            }

            cmbAlternateCareFacility.SelectedItem = Model.AlternateCareFacility;
        }

        public void ClearAlternateCareFacility()
        {
            if (cmbAlternateCareFacility.Items.Count > 0)
            {
                cmbAlternateCareFacility.SelectedIndex = 0;
            }
        }

        public void ShowAlternateCareFacilityDisabled()
        {
            UIColors.SetDisabledDarkBgColor(cmbAlternateCareFacility);
            cmbAlternateCareFacility.Enabled = false;
        }

        public void ShowAlternateCareFacilityEnabled()
        {
            UIColors.SetNormalBgColor(cmbAlternateCareFacility);
            cmbAlternateCareFacility.Enabled = true;
        }

        public void ShowAlternateCareFacilityVisible()
        {
            cmbAlternateCareFacility.Visible = true;
            lblAlternateCareFacility.Visible = true;
        }

        public void ShowAlternateCareFacilityNotVisible()
        {
            cmbAlternateCareFacility.Visible = false;
            lblAlternateCareFacility.Visible = false;
        }

        #endregion

        #region IEmergencyPatientToOutPatientStep1View Members

        public void ClearAdmitSourcesComboBox()
        {
            cboAdmitSource.Items.Clear();
        }

        public void ClearClinicCodesComboBox()
        {
            cboClinic1.Items.Clear();
        }

        public void ClearHospitalServicesComboBox()
        {
            cboHospitalService.Items.Clear();
        }

        public DateTime GetTransferDateTime()
        {
            var date = mtbTransferDate.Text;
            var time = (mtbTransferTime.UnMaskedText != string.Empty) ? mtbTransferTime.Text : "00:00";
            var transferDate = DateTime.MinValue;
            if (mtbTransferDate.UnMaskedText != string.Empty)
            {
                try
                {
                    transferDate = Convert.ToDateTime(date + " " + time);
                }
                catch
                {
                    transferDate = Convert.ToDateTime(date);
                }
            }
            return transferDate;
        }

        public bool IsTransferDateValid()
        {
            return TransferService.IsTransferDateValid(mtbTransferDate);
        }

        public void DisableControls()
        {
            panelToArea.Enabled = false;
            mtbChiefComplaint.Enabled = false;
            btnNext.Enabled = false;
        }

        #endregion

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureChiefComplaint(mtbChiefComplaint);
        }
        private void MakeEmailAddressVisible()
        {
            mtbEmail.Visible = true;
            lblEmail.Visible = true;
        }

        private void PopulateEmailAddress()
        {
            var mailingContactPoint = Model.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
            mtbEmail.UnMaskedText = mailingContactPoint.EmailAddress.ToString();
        }
        #endregion

        #region Private Properties

        public string OriginalChiefComplaint { private get; set; }

        private RuleEngine RuleEngine
        {
            get { return i_RuleEngine ?? (i_RuleEngine = RuleEngine.GetInstance()); }
        }

        #endregion

        public EmergencyPatientToOutPatientStep1View()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            ConfigureControls();
            // TODO: Add any initialization after the InitializeComponent call
            EnableThemesOn(this);
            emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            emailKeyPressExpression = new Regex(RegularExpressions.EmailValidCharactersExpression);
        }

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void cboClinic1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(cboClinic1);
            EmergencyPatientToOutPatientPresenter.UpdateClinicCode((HospitalClinic) cboClinic1.SelectedItem);
        }

        #region Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmergencyPatientToOutPatientStep1View));
            this.panelTransferERPatToOutPat = new System.Windows.Forms.Panel();
            this.lblEmail = new System.Windows.Forms.Label();
            this.mtbEmail = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.patientPortalOptInView = new PatientAccess.UI.RegulatoryViews.ViewImpl.PatientPortalOptInView();
            this.lblFinancialClassVal = new System.Windows.Forms.Label();
            this.lblPrimaryPlanVal = new System.Windows.Forms.Label();
            this.lblFinancialClass = new System.Windows.Forms.Label();
            this.lblPrimaryPlan = new System.Windows.Forms.Label();
            this.lblAlternateCareFacility = new System.Windows.Forms.Label();
            this.mtbChiefComplaint = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStep1 = new System.Windows.Forms.Label();
            this.lblChiefComplaint = new System.Windows.Forms.Label();
            this.llblTransferFrom = new PatientAccess.UI.CommonControls.LineLabel();
            this.infoControl1 = new PatientAccess.UI.CommonControls.InfoControl();
            this.panelFromBottomArea = new System.Windows.Forms.Panel();
            this.lblHospitalServiceVal = new System.Windows.Forms.Label();
            this.lblPatientTypeVal = new System.Windows.Forms.Label();
            this.lblAdmitSourceVal = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblHospitalService = new System.Windows.Forms.Label();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.lblAdmitSource = new System.Windows.Forms.Label();
            this.lblLocationVal = new System.Windows.Forms.Label();
            this.panelToArea = new System.Windows.Forms.Panel();
            this.cmbAlternateCareFacility = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cboClinic1 = new System.Windows.Forms.ComboBox();
            this.lblClinic1 = new System.Windows.Forms.Label();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.lblPatientTypeToVal = new System.Windows.Forms.Label();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.locationView1 = new PatientAccess.UI.CommonControls.LocationView();
            this.mtbTransferTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cboAdmitSource = new System.Windows.Forms.ComboBox();
            this.lblAdmitSource2 = new System.Windows.Forms.Label();
            this.lblHospitalService2 = new System.Windows.Forms.Label();
            this.mtbTransferDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblPatientType2 = new System.Windows.Forms.Label();
            this.lblTransferDate = new System.Windows.Forms.Label();
            this.cboHospitalService = new System.Windows.Forms.ComboBox();
            this.lblAdmitTimeVal = new System.Windows.Forms.Label();
            this.lblAdmitTime = new System.Windows.Forms.Label();
            this.lblAdmitDateVal = new System.Windows.Forms.Label();
            this.lblAccountVal = new System.Windows.Forms.Label();
            this.lblPatientNameVal = new System.Windows.Forms.Label();
            this.lblAdmitDate = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.ProgressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.btnNext = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelUserContext = new System.Windows.Forms.Panel();
            this.userContextView1 = new PatientAccess.UI.UserContextView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.btnBack = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelTransferERPatToOutPat.SuspendLayout();
            this.panelFromBottomArea.SuspendLayout();
            this.panelToArea.SuspendLayout();
            this.panelUserContext.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTransferERPatToOutPat
            // 
            this.panelTransferERPatToOutPat.BackColor = System.Drawing.Color.White;
            this.panelTransferERPatToOutPat.Controls.Add(this.lblEmail);
            this.panelTransferERPatToOutPat.Controls.Add(this.mtbEmail);
            this.panelTransferERPatToOutPat.Controls.Add(this.patientPortalOptInView);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblFinancialClassVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPrimaryPlanVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblFinancialClass);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPrimaryPlan);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAlternateCareFacility);
            this.panelTransferERPatToOutPat.Controls.Add(this.mtbChiefComplaint);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblStep1);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblChiefComplaint);
            this.panelTransferERPatToOutPat.Controls.Add(this.llblTransferFrom);
            this.panelTransferERPatToOutPat.Controls.Add(this.infoControl1);
            this.panelTransferERPatToOutPat.Controls.Add(this.panelFromBottomArea);
            this.panelTransferERPatToOutPat.Controls.Add(this.panelToArea);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAdmitTimeVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAdmitTime);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAdmitDateVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAccountVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPatientNameVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAdmitDate);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAccount);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPatientName);
            this.panelTransferERPatToOutPat.Controls.Add(this.ProgressPanel1);
            this.panelTransferERPatToOutPat.Location = new System.Drawing.Point(8, 64);
            this.panelTransferERPatToOutPat.Name = "panelTransferERPatToOutPat";
            this.panelTransferERPatToOutPat.Size = new System.Drawing.Size(1008, 520);
            this.panelTransferERPatToOutPat.TabIndex = 0;
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(679, 261);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(44, 23);
            this.lblEmail.TabIndex = 14;
            this.lblEmail.Text = "Email:";
            this.lblEmail.Visible = false;
            // 
            // mtbEmail
            // 
            this.mtbEmail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbEmail.Location = new System.Drawing.Point(729, 259);
            this.mtbEmail.Mask = "";
            this.mtbEmail.MaxLength = 64;
            this.mtbEmail.Name = "mtbEmail";
            this.mtbEmail.Size = new System.Drawing.Size(158, 20);
            this.mtbEmail.TabIndex = 14;
            this.mtbEmail.Visible = false;
            this.mtbEmail.Validating += new System.ComponentModel.CancelEventHandler(this.mtbEmailAddress_Validating);
            this.mtbEmail.Leave += new System.EventHandler(this.mtbEmailAddress_Leave);
            // 
            // patientPortalOptInView
            // 
            this.patientPortalOptInView.Location = new System.Drawing.Point(679, 217);
            this.patientPortalOptInView.Model = null;
            this.patientPortalOptInView.Name = "patientPortalOptInView";
            this.patientPortalOptInView.PatientPortalOptInPresenter = null;
            this.patientPortalOptInView.Size = new System.Drawing.Size(275, 36);
            this.patientPortalOptInView.TabIndex = 13;
            // 
            // lblFinancialClassVal
            // 
            this.lblFinancialClassVal.Location = new System.Drawing.Point(98, 155);
            this.lblFinancialClassVal.Name = "lblFinancialClassVal";
            this.lblFinancialClassVal.Size = new System.Drawing.Size(216, 16);
            this.lblFinancialClassVal.TabIndex = 69;
            // 
            // lblPrimaryPlanVal
            // 
            this.lblPrimaryPlanVal.Location = new System.Drawing.Point(98, 131);
            this.lblPrimaryPlanVal.Name = "lblPrimaryPlanVal";
            this.lblPrimaryPlanVal.Size = new System.Drawing.Size(216, 16);
            this.lblPrimaryPlanVal.TabIndex = 70;
            // 
            // lblFinancialClass
            // 
            this.lblFinancialClass.Location = new System.Drawing.Point(12, 155);
            this.lblFinancialClass.Name = "lblFinancialClass";
            this.lblFinancialClass.Size = new System.Drawing.Size(88, 23);
            this.lblFinancialClass.TabIndex = 67;
            this.lblFinancialClass.Text = "Financial class:";
            // 
            // lblPrimaryPlan
            // 
            this.lblPrimaryPlan.Location = new System.Drawing.Point(12, 131);
            this.lblPrimaryPlan.Name = "lblPrimaryPlan";
            this.lblPrimaryPlan.Size = new System.Drawing.Size(88, 23);
            this.lblPrimaryPlan.TabIndex = 68;
            this.lblPrimaryPlan.Text = "Primary plan:";
            // 
            // lblAlternateCareFacility
            // 
            this.lblAlternateCareFacility.Location = new System.Drawing.Point(323, 196);
            this.lblAlternateCareFacility.Name = "lblAlternateCareFacility";
            this.lblAlternateCareFacility.Size = new System.Drawing.Size(82, 27);
            this.lblAlternateCareFacility.TabIndex = 66;
            this.lblAlternateCareFacility.Text = "Nursing home/ Alt care facility";
            // 
            // mtbChiefComplaint
            // 
            this.mtbChiefComplaint.BackColor = System.Drawing.SystemColors.Window;
            this.mtbChiefComplaint.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbChiefComplaint.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbChiefComplaint.Location = new System.Drawing.Point(679, 164);
            this.mtbChiefComplaint.Mask = "";
            this.mtbChiefComplaint.MaxLength = 74;
            this.mtbChiefComplaint.Multiline = true;
            this.mtbChiefComplaint.Name = "mtbChiefComplaint";
            this.mtbChiefComplaint.Size = new System.Drawing.Size(297, 46);
            this.mtbChiefComplaint.TabIndex = 10;
            this.mtbChiefComplaint.Validating += new System.ComponentModel.CancelEventHandler(this.mtbChiefComplaint_Validating);
            // 
            // lblStep1
            // 
            this.lblStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStep1.Location = new System.Drawing.Point(13, 8);
            this.lblStep1.Name = "lblStep1";
            this.lblStep1.Size = new System.Drawing.Size(163, 23);
            this.lblStep1.TabIndex = 56;
            this.lblStep1.Text = "Step 1 of 3: Transfer to";
            // 
            // lblChiefComplaint
            // 
            this.lblChiefComplaint.Location = new System.Drawing.Point(669, 147);
            this.lblChiefComplaint.Name = "lblChiefComplaint";
            this.lblChiefComplaint.Size = new System.Drawing.Size(100, 16);
            this.lblChiefComplaint.TabIndex = 62;
            this.lblChiefComplaint.Text = "Chief complaint:";
            // 
            // llblTransferFrom
            // 
            this.llblTransferFrom.Caption = "Transfer from";
            this.llblTransferFrom.Location = new System.Drawing.Point(12, 186);
            this.llblTransferFrom.Name = "llblTransferFrom";
            this.llblTransferFrom.Size = new System.Drawing.Size(286, 24);
            this.llblTransferFrom.TabIndex = 55;
            this.llblTransferFrom.TabStop = false;
            // 
            // infoControl1
            // 
            this.infoControl1.Location = new System.Drawing.Point(24, 32);
            this.infoControl1.Message = "";
            this.infoControl1.Name = "infoControl1";
            this.infoControl1.Size = new System.Drawing.Size(960, 22);
            this.infoControl1.TabIndex = 54;
            this.infoControl1.TabStop = false;
            // 
            // panelFromBottomArea
            // 
            this.panelFromBottomArea.Controls.Add(this.lblHospitalServiceVal);
            this.panelFromBottomArea.Controls.Add(this.lblPatientTypeVal);
            this.panelFromBottomArea.Controls.Add(this.lblAdmitSourceVal);
            this.panelFromBottomArea.Controls.Add(this.lblLocation);
            this.panelFromBottomArea.Controls.Add(this.lblHospitalService);
            this.panelFromBottomArea.Controls.Add(this.lblPatientType);
            this.panelFromBottomArea.Controls.Add(this.lblAdmitSource);
            this.panelFromBottomArea.Controls.Add(this.lblLocationVal);
            this.panelFromBottomArea.Location = new System.Drawing.Point(4, 223);
            this.panelFromBottomArea.Name = "panelFromBottomArea";
            this.panelFromBottomArea.Size = new System.Drawing.Size(290, 96);
            this.panelFromBottomArea.TabIndex = 52;
            // 
            // lblHospitalServiceVal
            // 
            this.lblHospitalServiceVal.Location = new System.Drawing.Point(96, 53);
            this.lblHospitalServiceVal.Name = "lblHospitalServiceVal";
            this.lblHospitalServiceVal.Size = new System.Drawing.Size(187, 16);
            this.lblHospitalServiceVal.TabIndex = 23;
            // 
            // lblPatientTypeVal
            // 
            this.lblPatientTypeVal.Location = new System.Drawing.Point(96, 30);
            this.lblPatientTypeVal.Name = "lblPatientTypeVal";
            this.lblPatientTypeVal.Size = new System.Drawing.Size(194, 16);
            this.lblPatientTypeVal.TabIndex = 22;
            // 
            // lblAdmitSourceVal
            // 
            this.lblAdmitSourceVal.Location = new System.Drawing.Point(96, 3);
            this.lblAdmitSourceVal.Name = "lblAdmitSourceVal";
            this.lblAdmitSourceVal.Size = new System.Drawing.Size(192, 17);
            this.lblAdmitSourceVal.TabIndex = 21;
            // 
            // lblLocation
            // 
            this.lblLocation.Location = new System.Drawing.Point(10, 76);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(71, 25);
            this.lblLocation.TabIndex = 15;
            this.lblLocation.Text = "Location:";
            // 
            // lblHospitalService
            // 
            this.lblHospitalService.Location = new System.Drawing.Point(9, 53);
            this.lblHospitalService.Name = "lblHospitalService";
            this.lblHospitalService.Size = new System.Drawing.Size(88, 23);
            this.lblHospitalService.TabIndex = 14;
            this.lblHospitalService.Text = "Hospital service:";
            // 
            // lblPatientType
            // 
            this.lblPatientType.Location = new System.Drawing.Point(9, 29);
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size(78, 23);
            this.lblPatientType.TabIndex = 13;
            this.lblPatientType.Text = "Patient type:";
            // 
            // lblAdmitSource
            // 
            this.lblAdmitSource.Location = new System.Drawing.Point(9, 3);
            this.lblAdmitSource.Name = "lblAdmitSource";
            this.lblAdmitSource.Size = new System.Drawing.Size(88, 23);
            this.lblAdmitSource.TabIndex = 12;
            this.lblAdmitSource.Text = "Admit source:";
            // 
            // lblLocationVal
            // 
            this.lblLocationVal.Location = new System.Drawing.Point(96, 77);
            this.lblLocationVal.Name = "lblLocationVal";
            this.lblLocationVal.Size = new System.Drawing.Size(186, 16);
            this.lblLocationVal.TabIndex = 24;
            // 
            // panelToArea
            // 
            this.panelToArea.Controls.Add(this.cmbAlternateCareFacility);
            this.panelToArea.Controls.Add(this.cboClinic1);
            this.panelToArea.Controls.Add(this.lblClinic1);
            this.panelToArea.Controls.Add(this.dateTimePicker);
            this.panelToArea.Controls.Add(this.lblPatientTypeToVal);
            this.panelToArea.Controls.Add(this.lineLabel2);
            this.panelToArea.Controls.Add(this.locationView1);
            this.panelToArea.Controls.Add(this.mtbTransferTime);
            this.panelToArea.Controls.Add(this.cboAdmitSource);
            this.panelToArea.Controls.Add(this.lblAdmitSource2);
            this.panelToArea.Controls.Add(this.lblHospitalService2);
            this.panelToArea.Controls.Add(this.mtbTransferDate);
            this.panelToArea.Controls.Add(this.lblTime);
            this.panelToArea.Controls.Add(this.lblPatientType2);
            this.panelToArea.Controls.Add(this.lblTransferDate);
            this.panelToArea.Controls.Add(this.cboHospitalService);
            this.panelToArea.Location = new System.Drawing.Point(309, 133);
            this.panelToArea.Name = "panelToArea";
            this.panelToArea.Size = new System.Drawing.Size(344, 376);
            this.panelToArea.TabIndex = 0;
            // 
            // cmbAlternateCareFacility
            // 
            this.cmbAlternateCareFacility.DisplayMember = "Description";
            this.cmbAlternateCareFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlternateCareFacility.Location = new System.Drawing.Point(111, 68);
            this.cmbAlternateCareFacility.MaxLength = 27;
            this.cmbAlternateCareFacility.Name = "cmbAlternateCareFacility";
            this.cmbAlternateCareFacility.Size = new System.Drawing.Size(192, 21);
            this.cmbAlternateCareFacility.TabIndex = 1;
            this.cmbAlternateCareFacility.SelectedIndexChanged += new System.EventHandler(this.cmbAlternateCareFacility_SelectedIndexChanged);
            this.cmbAlternateCareFacility.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAlternateCareFacility_Validating);
            // 
            // cboClinic1
            // 
            this.cboClinic1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClinic1.Location = new System.Drawing.Point(111, 302);
            this.cboClinic1.Name = "cboClinic1";
            this.cboClinic1.Size = new System.Drawing.Size(192, 21);
            this.cboClinic1.TabIndex = 5;
            this.cboClinic1.SelectedIndexChanged += new System.EventHandler(this.cboClinic1_SelectedIndexChanged);
            // 
            // lblClinic1
            // 
            this.lblClinic1.Location = new System.Drawing.Point(13, 302);
            this.lblClinic1.Name = "lblClinic1";
            this.lblClinic1.Size = new System.Drawing.Size(88, 23);
            this.lblClinic1.TabIndex = 65;
            this.lblClinic1.Text = "Clinic 1:";
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point(153, 331);
            this.dateTimePicker.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(21, 20);
            this.dateTimePicker.TabIndex = 7;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new System.EventHandler(this.dateTimePicker_CloseUp);
            // 
            // lblPatientTypeToVal
            // 
            this.lblPatientTypeToVal.Location = new System.Drawing.Point(111, 102);
            this.lblPatientTypeToVal.Name = "lblPatientTypeToVal";
            this.lblPatientTypeToVal.Size = new System.Drawing.Size(192, 21);
            this.lblPatientTypeToVal.TabIndex = 2;
            // 
            // lineLabel2
            // 
            this.lineLabel2.Caption = "Transfer to";
            this.lineLabel2.Location = new System.Drawing.Point(12, 7);
            this.lineLabel2.Name = "lineLabel2";
            this.lineLabel2.Size = new System.Drawing.Size(321, 18);
            this.lineLabel2.TabIndex = 0;
            this.lineLabel2.TabStop = false;
            // 
            // locationView1
            // 
            this.locationView1.EditFindButtonText = "Find...";
            this.locationView1.EditVerifyButtonText = "Verify";
            this.locationView1.Location = new System.Drawing.Point(13, 157);
            this.locationView1.Model = null;
            this.locationView1.Name = "locationView1";
            this.locationView1.Size = new System.Drawing.Size(328, 133);
            this.locationView1.TabIndex = 4;
            this.locationView1.BedSelected += new System.EventHandler(this.locationView1_BedSelected);
            // 
            // mtbTransferTime
            // 
            this.mtbTransferTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbTransferTime.KeyPressExpression = "^\\d*$";
            this.mtbTransferTime.Location = new System.Drawing.Point(227, 330);
            this.mtbTransferTime.Mask = "  :";
            this.mtbTransferTime.MaxLength = 5;
            this.mtbTransferTime.Multiline = true;
            this.mtbTransferTime.Name = "mtbTransferTime";
            this.mtbTransferTime.Size = new System.Drawing.Size(43, 20);
            this.mtbTransferTime.TabIndex = 9;
            this.mtbTransferTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbTransferTime.Validating += new System.ComponentModel.CancelEventHandler(this.mtbTransferTime_Validating);
            // 
            // cboAdmitSource
            // 
            this.cboAdmitSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAdmitSource.Location = new System.Drawing.Point(111, 34);
            this.cboAdmitSource.Name = "cboAdmitSource";
            this.cboAdmitSource.Size = new System.Drawing.Size(192, 21);
            this.cboAdmitSource.TabIndex = 0;
            this.cboAdmitSource.SelectedIndexChanged += new System.EventHandler(this.cboAdmitSource_SelectIndexChanged);
            this.cboAdmitSource.Validating += new System.ComponentModel.CancelEventHandler(this.cboAdmitSource_Validating);
            // 
            // lblAdmitSource2
            // 
            this.lblAdmitSource2.Location = new System.Drawing.Point(13, 34);
            this.lblAdmitSource2.Name = "lblAdmitSource2";
            this.lblAdmitSource2.Size = new System.Drawing.Size(88, 23);
            this.lblAdmitSource2.TabIndex = 32;
            this.lblAdmitSource2.Text = "Admit source:";
            // 
            // lblHospitalService2
            // 
            this.lblHospitalService2.Location = new System.Drawing.Point(13, 129);
            this.lblHospitalService2.Name = "lblHospitalService2";
            this.lblHospitalService2.Size = new System.Drawing.Size(88, 23);
            this.lblHospitalService2.TabIndex = 37;
            this.lblHospitalService2.Text = "Hospital service:";
            // 
            // mtbTransferDate
            // 
            this.mtbTransferDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbTransferDate.KeyPressExpression = "^\\d*$";
            this.mtbTransferDate.Location = new System.Drawing.Point(87, 331);
            this.mtbTransferDate.Mask = "  /  /";
            this.mtbTransferDate.MaxLength = 10;
            this.mtbTransferDate.Name = "mtbTransferDate";
            this.mtbTransferDate.Size = new System.Drawing.Size(70, 20);
            this.mtbTransferDate.TabIndex = 6;
            this.mtbTransferDate.ValidationExpression = resources.GetString("mtbTransferDate.ValidationExpression");
            this.mtbTransferDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbTransferDate_Validating);
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point(191, 335);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(39, 16);
            this.lblTime.TabIndex = 8;
            this.lblTime.Text = "Time:";
            // 
            // lblPatientType2
            // 
            this.lblPatientType2.Location = new System.Drawing.Point(13, 99);
            this.lblPatientType2.Name = "lblPatientType2";
            this.lblPatientType2.Size = new System.Drawing.Size(88, 23);
            this.lblPatientType2.TabIndex = 34;
            this.lblPatientType2.Text = "Patient type:";
            // 
            // lblTransferDate
            // 
            this.lblTransferDate.Location = new System.Drawing.Point(13, 332);
            this.lblTransferDate.Name = "lblTransferDate";
            this.lblTransferDate.Size = new System.Drawing.Size(88, 18);
            this.lblTransferDate.TabIndex = 42;
            this.lblTransferDate.Text = "Transfer date:";
            // 
            // cboHospitalService
            // 
            this.cboHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHospitalService.Location = new System.Drawing.Point(111, 129);
            this.cboHospitalService.Name = "cboHospitalService";
            this.cboHospitalService.Size = new System.Drawing.Size(192, 21);
            this.cboHospitalService.TabIndex = 3;
            this.cboHospitalService.SelectedIndexChanged += new System.EventHandler(this.cboHospitalService_SelectedIndexChanged);
            // 
            // lblAdmitTimeVal
            // 
            this.lblAdmitTimeVal.Location = new System.Drawing.Point(243, 107);
            this.lblAdmitTimeVal.Name = "lblAdmitTimeVal";
            this.lblAdmitTimeVal.Size = new System.Drawing.Size(56, 13);
            this.lblAdmitTimeVal.TabIndex = 49;
            // 
            // lblAdmitTime
            // 
            this.lblAdmitTime.Location = new System.Drawing.Point(203, 107);
            this.lblAdmitTime.Name = "lblAdmitTime";
            this.lblAdmitTime.Size = new System.Drawing.Size(35, 13);
            this.lblAdmitTime.TabIndex = 48;
            this.lblAdmitTime.Text = "Time:";
            // 
            // lblAdmitDateVal
            // 
            this.lblAdmitDateVal.Location = new System.Drawing.Point(100, 107);
            this.lblAdmitDateVal.Name = "lblAdmitDateVal";
            this.lblAdmitDateVal.Size = new System.Drawing.Size(88, 16);
            this.lblAdmitDateVal.TabIndex = 18;
            // 
            // lblAccountVal
            // 
            this.lblAccountVal.Location = new System.Drawing.Point(100, 83);
            this.lblAccountVal.Name = "lblAccountVal";
            this.lblAccountVal.Size = new System.Drawing.Size(216, 16);
            this.lblAccountVal.TabIndex = 17;
            // 
            // lblPatientNameVal
            // 
            this.lblPatientNameVal.Location = new System.Drawing.Point(100, 59);
            this.lblPatientNameVal.Name = "lblPatientNameVal";
            this.lblPatientNameVal.Size = new System.Drawing.Size(224, 16);
            this.lblPatientNameVal.TabIndex = 16;
            // 
            // lblAdmitDate
            // 
            this.lblAdmitDate.Location = new System.Drawing.Point(12, 107);
            this.lblAdmitDate.Name = "lblAdmitDate";
            this.lblAdmitDate.Size = new System.Drawing.Size(88, 23);
            this.lblAdmitDate.TabIndex = 2;
            this.lblAdmitDate.Text = "Admit date:";
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point(12, 83);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(88, 23);
            this.lblAccount.TabIndex = 1;
            this.lblAccount.Text = "Account:";
            // 
            // lblPatientName
            // 
            this.lblPatientName.Location = new System.Drawing.Point(12, 59);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(80, 23);
            this.lblPatientName.TabIndex = 0;
            this.lblPatientName.Text = "Patient name:";
            // 
            // ProgressPanel1
            // 
            this.ProgressPanel1.BackColor = System.Drawing.Color.White;
            this.ProgressPanel1.Location = new System.Drawing.Point(7, 6);
            this.ProgressPanel1.Name = "ProgressPanel1";
            this.ProgressPanel1.Size = new System.Drawing.Size(993, 503);
            this.ProgressPanel1.TabIndex = 57;
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.SystemColors.Control;
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(944, 594);
            this.btnNext.Message = null;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(72, 23);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "&Next >";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point(776, 594);
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panelUserContext
            // 
            this.panelUserContext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.panelUserContext.Controls.Add(this.userContextView1);
            this.panelUserContext.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUserContext.Location = new System.Drawing.Point(0, 0);
            this.panelUserContext.Name = "panelUserContext";
            this.panelUserContext.Size = new System.Drawing.Size(1024, 22);
            this.panelUserContext.TabIndex = 0;
            // 
            // userContextView1
            // 
            this.userContextView1.BackColor = System.Drawing.SystemColors.Control;
            this.userContextView1.Description = "";
            this.userContextView1.Location = new System.Drawing.Point(0, 0);
            this.userContextView1.Model = null;
            this.userContextView1.Name = "userContextView1";
            this.userContextView1.Size = new System.Drawing.Size(1024, 23);
            this.userContextView1.TabIndex = 0;
            this.userContextView1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.patientContextView1);
            this.panel1.Location = new System.Drawing.Point(8, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1008, 24);
            this.panel1.TabIndex = 5;
            // 
            // patientContextView1
            // 
            this.patientContextView1.Account = null;
            this.patientContextView1.BackColor = System.Drawing.Color.White;
            this.patientContextView1.DateOfBirthText = "";
            this.patientContextView1.GenderLabelText = "";
            this.patientContextView1.Location = new System.Drawing.Point(0, 0);
            this.patientContextView1.Model = null;
            this.patientContextView1.Name = "patientContextView1";
            this.patientContextView1.PatientNameText = "";
            this.patientContextView1.Size = new System.Drawing.Size(1008, 53);
            this.patientContextView1.SocialSecurityNumber = "";
            this.patientContextView1.TabIndex = 0;
            this.patientContextView1.TabStop = false;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.SystemColors.Control;
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point(864, 594);
            this.btnBack.Message = null;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "< &Back";
            this.btnBack.UseVisualStyleBackColor = false;
            // 
            // EmergencyPatientToOutPatientStep1View
            // 
            this.AcceptButton = this.btnNext;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.panelTransferERPatToOutPat);
            this.Controls.Add(this.panelUserContext);
            this.Name = "EmergencyPatientToOutPatientStep1View";
            this.Size = new System.Drawing.Size(1024, 632);
            this.Leave += new System.EventHandler(this.TransferERPatToOutPatView_LeaveView);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.TransferERPatToOutPatStep1View_Validating);
            this.Disposed += new System.EventHandler(this.TransferERPatToOutPatStep1View_Disposed);
            this.panelTransferERPatToOutPat.ResumeLayout(false);
            this.panelTransferERPatToOutPat.PerformLayout();
            this.panelFromBottomArea.ResumeLayout(false);
            this.panelToArea.ResumeLayout(false);
            this.panelToArea.PerformLayout();
            this.panelUserContext.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Data Elements

        private readonly IContainer components = null;

        private ProgressPanel ProgressPanel1;
        private LoggingButton btnBack;

        private LoggingButton btnCancel;
        private LoggingButton btnNext;


        private ComboBox cboAdmitSource;
        private ComboBox cboHospitalService;
        private DateTimePicker dateTimePicker;
        private bool inErrorState;
        private InfoControl infoControl1;
        private Label lblAccount;
        private Label lblAccountVal;
        private Label lblAdmitDate;
        private Label lblAdmitDateVal;
        private Label lblAdmitSource;
        private Label lblAdmitSource2;
        private Label lblAdmitSourceVal;
        private Label lblAdmitTime;
        private Label lblAdmitTimeVal;
        private Label lblChiefComplaint;
        private Label lblHospitalService;
        private Label lblHospitalService2;
        private Label lblHospitalServiceVal;
        private Label lblLocation;
        private Label lblLocationVal;
        private Label lblPatientName;
        private Label lblPatientNameVal;
        private Label lblPatientType;
        private Label lblPatientType2;
        private Label lblPatientTypeToVal;
        private Label lblPatientTypeVal;
        private Label lblStep1;
        private Label lblTime;
        private Label lblTransferDate;
        private LineLabel lineLabel2;
        private LineLabel llblTransferFrom;
        private LocationView locationView1;
        public MaskedEditTextBox mtbChiefComplaint;
        private MaskedEditTextBox mtbTransferDate;
        private MaskedEditTextBox mtbTransferTime;
        private Panel panel1;
        private Panel panelFromBottomArea;
        private Panel panelToArea;
        private Panel panelTransferERPatToOutPat;
        private Panel panelUserContext;
        private PatientContextView patientContextView1;
        private UserContextView userContextView1;
        private readonly Regex emailValidationExpression;
        private readonly Regex emailKeyPressExpression;
        private RuleEngine i_RuleEngine;

        #endregion

        #region Data Elements

        #endregion

        #region Constants

        // The list of codes will always have a blank item in it, so a count
        //   of one means there are no codes

        private ComboBox cboClinic1;
        private PatientAccessComboBox cmbAlternateCareFacility;
        private Label lblAlternateCareFacility;
        private Label lblClinic1;
        private Label lblFinancialClass;
        private Label lblFinancialClassVal;
        private Label lblPrimaryPlan;
        private Label lblEmail;
        private MaskedEditTextBox mtbEmail;
        private Label lblPrimaryPlanVal;
        private PatientPortalOptInView patientPortalOptInView;

        #endregion
    }
}