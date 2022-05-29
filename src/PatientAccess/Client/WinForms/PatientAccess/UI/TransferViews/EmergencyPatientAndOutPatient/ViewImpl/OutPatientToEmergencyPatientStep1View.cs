using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient.ViewImpl
{
    public class OutPatientToEmergencyPatientStep1View : ControlView, IOutPatientToEmergencyPatientStep1View
    {
        #region Events

        public event EventHandler CancelButtonClicked;
        public event EventHandler NextButtonClicked;

        #endregion

        #region Event Handlers

        private void TransferOutPatientToERPatStep1View_Validating(object sender, CancelEventArgs e)
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
                if (OutPatientToEmergencyPatientPresenter != null)
                {
                    OutPatientToEmergencyPatientPresenter.HandleAdmitSourceSelectedIndexChanged(
                        cb.SelectedItem as AdmitSource);
                }
            }
        }

        private void cboAdmitSource_Validating(object sender, CancelEventArgs e)
        {
            if (OutPatientToEmergencyPatientPresenter != null)
            {
                OutPatientToEmergencyPatientPresenter.HandleAdmitSourceSelectedIndexChanged(
                    cboAdmitSource.SelectedItem as AdmitSource);
            }
            Refresh();
        }

        //selected a HSV
        private void cboHospitalService_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(cboHospitalService);
            var cb = (ComboBox) sender;
            if (cb.SelectedIndex != -1)
            {
                OutPatientToEmergencyPatientPresenter.UpdateHospitalService(
                    (HospitalService) cboHospitalService.SelectedItem);
            }
        }


        private void mtbChiefComplaint_Validating(object sender, CancelEventArgs e)
        {
            ValidateChiefComplaint();
        }

        private void mtbChiefComplaint_Leave(object sender, EventArgs e)
        {
            ValidateChiefComplaint();
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

            inErrorState = OutPatientToEmergencyPatientPresenter.UpdateTransferDate();

            Refresh();
        }

        private void mtbTransferTime_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(mtbTransferTime);
            UIColors.SetNormalBgColor(mtbTransferDate);

            inErrorState = false;
            if (!OutPatientToEmergencyPatientPresenter.UpdateTransferTime())
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

        private void Application_Idle(object sender, EventArgs e)
        {
        }

        private void TransferERPatToOutPatView_LeaveView(object sender, EventArgs e)
        {
            Application.Idle -= Application_Idle;
        }

        private void TransferOutPatientToERPatStep1View_Disposed(object sender, EventArgs e)
        {
            OutPatientToEmergencyPatientPresenter.UnRegisterRulesEvents();
        }

        #endregion

        #region Methods

        private void ValidateChiefComplaint()
        {
            UIColors.SetNormalBgColor(mtbChiefComplaint);
            if (OutPatientToEmergencyPatientPresenter != null)
            {
                OutPatientToEmergencyPatientPresenter.UpdateChiefComplaint(mtbChiefComplaint.UnMaskedText);
            }
        }

        public void EnableNextButton(bool enable)
        {
            btnNext.Enabled = enable;
        }

        public void DisplayInfoMessage(string message)
        {
            infoControl1.DisplayInfoMessage(message);
        }

        public void ClearAdmitSourcesComboBox()
        {
            cboAdmitSource.Items.Clear();
        }

        public void ClearHospitalServiceComboBox()
        {
            cboHospitalService.Items.Clear();
        }

        public void DisableControls()
        {
            panelToArea.Enabled = false;
            mtbChiefComplaint.Enabled = false;
            btnNext.Enabled = false;
        }

        public void SetHospitalServiceRequired()
        {
            UIColors.SetRequiredBgColor(cboHospitalService);
        }

        public void SetTransferDateRequired()
        {
            UIColors.SetRequiredBgColor(mtbTransferDate);
        }

        public void SetTransferTimeRequired()
        {
            UIColors.SetRequiredBgColor(mtbTransferTime);
        }

        public void SetAdmitSourceRequired()
        {
            UIColors.SetRequiredBgColor(cboAdmitSource);
        }

        public void SetChiefComplaintRequired()
        {
            UIColors.SetRequiredBgColor(mtbChiefComplaint);
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

                if (aControl != null && aControl.Equals(OutPatientToEmergencyPatientPresenter.TRANSFER_DATE))
                {
                    UIColors.SetErrorBgColor(mtbTransferDate);
                    dateTimePicker.Focus();

                    MessageBox.Show(UIErrorMessages.TRANSFER_DATE_IN_FUTURE_MSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
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
                }
            }
        }

        public void DisplayTransferDateTimeBeforeAdmitDateTimeMessage(EventArgs e)
        {
            if (e != null)
            {
                var args = (PropertyChangedArgs) e;
                var aControl = args.Context as string;

                if (aControl != null && aControl.Equals(OutPatientToEmergencyPatientPresenter.TRANSFER_DATE))
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

        public void ShowPanel()
        {
            ProgressPanel1.Visible = true;
            ProgressPanel1.BringToFront();
        }

        public void HidePanel()
        {
            ProgressPanel1.Visible = false;
        }

        public override void UpdateView()
        {
            OutPatientToEmergencyPatientPresenter = new OutPatientToEmergencyPatientStep1Presenter(this, new MessageBoxAdapter(), Model,
                                                                                                   RuleEngine.
                                                                                                       GetInstance());
            OutPatientToEmergencyPatientPresenter.UpdateView();
            cboAdmitSource.Focus();
        }

        public bool HasUpdatedChiefComplaint()
        {
            return mtbChiefComplaint.UnMaskedText.Trim() != OriginalChiefComplaint;
        }

        public override void UpdateModel()
        {
            OutPatientToEmergencyPatientPresenter.UpdateModel();
        }

        #endregion

        #region Properties

        public new Account Model
        {
            get { return (Account) base.Model; }
            set { base.Model = value; }
        }

        private OutPatientToEmergencyPatientStep1Presenter OutPatientToEmergencyPatientPresenter { get; set; }

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
            get { return lblPatientNameVal.Text ; }
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

        public string AdmitSource
        {
            get { return lblAdmitSourceVal.Text; }
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
            get { return cboAdmitSource.SelectedItem as AdmitSource; }
            set { cboAdmitSource.SelectedItem = value; }
        }

        public HospitalService HospitalServiceSelectedItem
        {
            get { return cboHospitalService.SelectedItem as HospitalService; }
            set { cboHospitalService.SelectedItem = value; }
        }

        public int AdmitSourcesSelectedIndex
        {
            set { cboAdmitSource.SelectedIndex = value; }
        }
        public int HospitalServicesSelectedIndex
        {
            set { cboHospitalService.SelectedIndex = value; }
        }

        public void AddAdmitSources(AdmitSource admitSource)
        {
            cboAdmitSource.Items.Add(admitSource);
        }

        public void AddHospitalService(HospitalService hospitalService)
        {
            cboHospitalService.Items.Add(hospitalService);
        }
        public void SetHospitalServiceListSorted()
        {
            cboHospitalService.Sorted = true;
        }
        public bool IsTransferDateValid()
        {
            return TransferService.IsTransferDateValid(mtbTransferDate);
        }

        #endregion

        #region private method

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

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureChiefComplaint(mtbChiefComplaint);
        }

        #endregion

        #region Private Properties

        public string OriginalChiefComplaint { private get; set; }

        #endregion

        public OutPatientToEmergencyPatientStep1View()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            ConfigureControls();
            // TODO: Add any initialization after the InitializeComponent call
            EnableThemesOn(this);
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

        #region Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutPatientToEmergencyPatientStep1View));
            this.panelTransferERPatToOutPat = new System.Windows.Forms.Panel();
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
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.lblPatientTypeToVal = new System.Windows.Forms.Label();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
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
            // mtbChiefComplaint
            // 
            this.mtbChiefComplaint.BackColor = System.Drawing.SystemColors.Window;
            this.mtbChiefComplaint.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbChiefComplaint.Location = new System.Drawing.Point(687, 171);
            this.mtbChiefComplaint.Mask = "";
            this.mtbChiefComplaint.MaxLength = 74;
            this.mtbChiefComplaint.Multiline = true;
            this.mtbChiefComplaint.Name = "mtbChiefComplaint";
            this.mtbChiefComplaint.Size = new System.Drawing.Size(297, 46);
            this.mtbChiefComplaint.TabIndex = 10;
            this.mtbChiefComplaint.Leave += new System.EventHandler(this.mtbChiefComplaint_Leave);
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
            this.lblChiefComplaint.Location = new System.Drawing.Point(685, 140);
            this.lblChiefComplaint.Name = "lblChiefComplaint";
            this.lblChiefComplaint.Size = new System.Drawing.Size(100, 16);
            this.lblChiefComplaint.TabIndex = 62;
            this.lblChiefComplaint.Text = "Chief complaint:";
            // 
            // llblTransferFrom
            // 
            this.llblTransferFrom.Caption = "Transfer from";
            this.llblTransferFrom.Location = new System.Drawing.Point(12, 139);
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
            this.panelFromBottomArea.Location = new System.Drawing.Point(4, 171);
            this.panelFromBottomArea.Name = "panelFromBottomArea";
            this.panelFromBottomArea.Size = new System.Drawing.Size(290, 107);
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
            this.panelToArea.Controls.Add(this.dateTimePicker);
            this.panelToArea.Controls.Add(this.lblPatientTypeToVal);
            this.panelToArea.Controls.Add(this.lineLabel2);
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
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point(170, 115);
            this.dateTimePicker.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(21, 20);
            this.dateTimePicker.TabIndex = 6;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new System.EventHandler(this.dateTimePicker_CloseUp);
            // 
            // lblPatientTypeToVal
            // 
            this.lblPatientTypeToVal.Location = new System.Drawing.Point(110, 67);
            this.lblPatientTypeToVal.Name = "lblPatientTypeToVal";
            this.lblPatientTypeToVal.Size = new System.Drawing.Size(214, 16);
            this.lblPatientTypeToVal.TabIndex = 61;
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
            // mtbTransferTime
            // 
            this.mtbTransferTime.KeyPressExpression = "^\\d*$";
            this.mtbTransferTime.Location = new System.Drawing.Point(234, 115);
            this.mtbTransferTime.Mask = "  :";
            this.mtbTransferTime.MaxLength = 5;
            this.mtbTransferTime.Multiline = true;
            this.mtbTransferTime.Name = "mtbTransferTime";
            this.mtbTransferTime.Size = new System.Drawing.Size(43, 20);
            this.mtbTransferTime.TabIndex = 3;
            this.mtbTransferTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbTransferTime.Validating += new System.ComponentModel.CancelEventHandler(this.mtbTransferTime_Validating);
            // 
            // cboAdmitSource
            // 
            this.cboAdmitSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAdmitSource.Location = new System.Drawing.Point(107, 38);
            this.cboAdmitSource.Name = "cboAdmitSource";
            this.cboAdmitSource.Size = new System.Drawing.Size(192, 21);
            this.cboAdmitSource.TabIndex = 0;
            this.cboAdmitSource.SelectedIndexChanged += new System.EventHandler(this.cboAdmitSource_SelectIndexChanged);
            this.cboAdmitSource.Validating += new System.ComponentModel.CancelEventHandler(this.cboAdmitSource_Validating);
            // 
            // lblAdmitSource2
            // 
            this.lblAdmitSource2.Location = new System.Drawing.Point(13, 41);
            this.lblAdmitSource2.Name = "lblAdmitSource2";
            this.lblAdmitSource2.Size = new System.Drawing.Size(88, 23);
            this.lblAdmitSource2.TabIndex = 32;
            this.lblAdmitSource2.Text = "Admit source:";
            // 
            // lblHospitalService2
            // 
            this.lblHospitalService2.Location = new System.Drawing.Point(13, 91);
            this.lblHospitalService2.Name = "lblHospitalService2";
            this.lblHospitalService2.Size = new System.Drawing.Size(88, 23);
            this.lblHospitalService2.TabIndex = 37;
            this.lblHospitalService2.Text = "Hospital service:";
            // 
            // mtbTransferDate
            // 
            this.mtbTransferDate.KeyPressExpression = "^\\d*$";
            this.mtbTransferDate.Location = new System.Drawing.Point(107, 115);
            this.mtbTransferDate.Mask = "  /  /";
            this.mtbTransferDate.MaxLength = 10;
            this.mtbTransferDate.Name = "mtbTransferDate";
            this.mtbTransferDate.Size = new System.Drawing.Size(70, 20);
            this.mtbTransferDate.TabIndex = 2;
            this.mtbTransferDate.ValidationExpression = resources.GetString("mtbTransferDate.ValidationExpression");
            this.mtbTransferDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbTransferDate_Validating);
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point(198, 118);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(39, 16);
            this.lblTime.TabIndex = 6;
            this.lblTime.Text = "Time:";
            // 
            // lblPatientType2
            // 
            this.lblPatientType2.Location = new System.Drawing.Point(13, 67);
            this.lblPatientType2.Name = "lblPatientType2";
            this.lblPatientType2.Size = new System.Drawing.Size(88, 23);
            this.lblPatientType2.TabIndex = 34;
            this.lblPatientType2.Text = "Patient type:";
            // 
            // lblTransferDate
            // 
            this.lblTransferDate.Location = new System.Drawing.Point(13, 115);
            this.lblTransferDate.Name = "lblTransferDate";
            this.lblTransferDate.Size = new System.Drawing.Size(88, 18);
            this.lblTransferDate.TabIndex = 42;
            this.lblTransferDate.Text = "Transfer date:";
            // 
            // cboHospitalService
            // 
            this.cboHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHospitalService.Location = new System.Drawing.Point(107, 86);
            this.cboHospitalService.Name = "cboHospitalService";
            this.cboHospitalService.Size = new System.Drawing.Size(224, 21);
            this.cboHospitalService.TabIndex = 1;
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
            this.lblPatientNameVal.Size = new System.Drawing.Size(400, 16);
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
            this.btnNext.TabIndex = 17;
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
            this.btnCancel.TabIndex = 15;
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
            this.btnBack.TabIndex = 16;
            this.btnBack.Text = "< &Back";
            this.btnBack.UseVisualStyleBackColor = false;
            // 
            // OutPatientToEmergencyPatientStep1View
            // 
            this.AcceptButton = this.btnNext;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.panelTransferERPatToOutPat);
            this.Controls.Add(this.panelUserContext);
            this.Name = "OutPatientToEmergencyPatientStep1View";
            this.Size = new System.Drawing.Size(1024, 632);
            this.Leave += new System.EventHandler(this.TransferERPatToOutPatView_LeaveView);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.TransferOutPatientToERPatStep1View_Validating);
            this.Disposed += new System.EventHandler(this.TransferOutPatientToERPatStep1View_Disposed);
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

        #endregion

        #region Constants

        #endregion
    }
}