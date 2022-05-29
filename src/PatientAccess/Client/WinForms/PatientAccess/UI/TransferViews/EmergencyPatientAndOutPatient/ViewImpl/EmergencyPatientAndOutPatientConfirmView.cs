using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DocumentImagingViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Reports.FaceSheet;

namespace PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient.ViewImpl
{
    public class EmergencyPatientAndOutPatientConfirmView : ControlView
    {
        #region Events

        public event EventHandler RepeatActivity;
        public event EventHandler EditAccount;
        public event EventHandler CloseView;

        #endregion

        #region Event Handlers

        private void btnCloseActivity_Click(object sender, EventArgs e)
        {
            AccountView.CloseVIweb();
            CloseView(this, new EventArgs());
        }

        private void btnRepeatActivity_Click(object sender, EventArgs e)
        {
            AccountView.CloseVIweb();
            RepeatActivity(this, new LooseArgs(new Patient()));
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            try
            {
                AccountView.CloseVIweb();
                if (AccountLockStatus.IsAccountLocked(Model, User.GetCurrent()))
                {
                    MessageBox.Show(UIErrorMessages.NO_EDIT_RECORD_LOCKED, "Warning",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1);
                    btnEditAccount.Enabled = true;
                    return;
                }

                Cursor = Cursors.WaitCursor;

                if (Model.IsShortRegisteredNonDayCareAccount())
                {
                    // Setting this property on AccountView will direct the application to the 
                    // 8-tab view for a Short-Registered account instead of the regular 12-tab view
                    AccountView.IsShortRegAccount = true;
                    Model.Activity = new ShortMaintenanceActivity();
                }
                else
                {
                    Model.Activity = new MaintenanceActivity();
                }

                if (!Model.Activity.ReadOnlyAccount())
                {
                    if (!AccountActivityService.PlaceLockOn(Model, UIErrorMessages.PATIENT_ACCTS_LOCKED))
                    {
                        Cursor = Cursors.Default;
                        btnEditAccount.Enabled = true;
                        return;
                    }
                }

                EditAccount(this, new LooseArgs(Model));

                Cursor = Cursors.Default;
            }
            catch (AccountNotFoundException)
            {
                Cursor = Cursors.Default;
                btnEditAccount.Enabled = true;
                MessageBox.Show(UIErrorMessages.ACTIVITY_CANNOT_PROCEED, "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button1);
            }
        }

        private void btnPrintFaceSheet_Click(object sender, EventArgs e)
        {
            AccountView.CloseVIweb();
            var faceSheetPrintService = new PrintService(Model);
            faceSheetPrintService.Print();
        }

        private void btnScanDocuments_Click(object sender, EventArgs e)
        {
            OpenScanDocumentsForm();
        }

        #endregion

        #region Methods

        public EmergencyPatientAndOutPatientConfirmView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn(this);
        }

        public override void UpdateView()
        {
            var primaryCarePhysicianFeatureManager = new PrimaryCarePhysicianFeatureManager();
            lblPcp.Text =
                primaryCarePhysicianFeatureManager.IsPrimaryCarePhysicianEnabledForDate(Model.AccountCreatedDate)
                    ? PhysicianRole.PRIMARYCAREPHYSICIAN_LABEL
                    : PhysicianRole.OTHERPHYSICIAN_LABEL;

            if (Model != null)
            {
                if (Model.Activity != null)
                {
                    userContextView1.Description = Model.Activity.ContextDescription + "-Submitted";
                    DisplayInformationMessage();
                }
                if (Model.Patient != null)
                {
                    patientContextView1.Model = Model.Patient;
                    patientContextView1.Account = Model;
                    patientContextView1.UpdateView();

                    lblPatientNameVal.Text = Model.Patient.FormattedName;
                }

                lblAccountVal.Text = Model.AccountNumber.ToString();

                if (Model.Insurance.Coverages.Count > 0)
                {
                    foreach (Coverage coverage in Model.Insurance.Coverages)
                    {
                        if (coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID)
                        {
                            lblPrimaryPlanVal.Text = coverage.InsurancePlan.PlanID + " " +
                                                     coverage.InsurancePlan.Payor.Name;
                        }
                    }
                }

                if (Model.FinancialClass != null)
                {
                    lblFinancialClassVal.Text = Model.FinancialClass.ToCodedString();
                }

                if (Model.AdmitSource != null)
                {
                    lblAdmitSourceVal.Text = Model.AdmitSource.DisplayString;
                }

                if (Model.KindOfVisit != null)
                {
                    lblPatientTypeVal.Text = Model.KindOfVisit.DisplayString;
                }

                if (Model.HospitalService != null)
                {
                    lblHospitalServiceVal.Text = Model.HospitalService.ToCodedString();
                }

                if (Model.Location != null)
                {
                    lblLocationVal.Text = Model.Location.DisplayString;

                    if (Model.Location.Bed != null && Model.Location.Bed.Accomodation != null)
                    {
                        lblAccommodationVal.Text = Model.Location.Bed.Accomodation.Code + " " +
                                                   Model.Location.Bed.Accomodation.Description;
                    }
                }


                lblTransferDateVal.Text = CommonFormatting.LongDateFormat(Model.TransferDate);
                lblTransferTimeVal.Text = CommonFormatting.DisplayedTimeFormat(Model.TransferDate);

                lblCommentsVal.Text = Model.ClinicalComments;

                PopulatePhysicians();

                btnCloseActivity.Focus();
            }

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions(false);
        }

        public override void UpdateModel()
        {
        }

        private void DisplayInformationMessage()
        {
            if (Model.Activity.IsTransferERToOutpatientActivity())
            {
                infoControl1.Message = UIErrorMessages.TRANSFER_ER_TO_OUT_SUBMITTED;
            }
            else if (Model.Activity.IsTransferOutpatientToERActivity())
            {
                infoControl1.Message = UIErrorMessages.TRANSFER_OUT_TO_ER_SUBMITTED;
            }
        }

        #endregion

        #region Properties

        public new Account Model
        {
            private get { return (Account) base.Model; }
            set { base.Model = value; }
        }

        #endregion

        #region private method

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

        private void PopulatePhysicians()
        {
            if (Model.ReferringPhysician != null)
            {
                lblRefVal.Text = String.Format("{0:00000} {1}", Model.ReferringPhysician.PhysicianNumber,
                                               Model.ReferringPhysician.FormattedName);
            }

            if (Model.AdmittingPhysician != null)
            {
                lblAdmVal.Text = String.Format("{0:00000} {1}", Model.AdmittingPhysician.PhysicianNumber,
                                               Model.AdmittingPhysician.FormattedName);
            }

            if (Model.AttendingPhysician != null)
            {
                lblAttVal.Text = String.Format("{0:00000} {1}", Model.AttendingPhysician.PhysicianNumber,
                                               Model.AttendingPhysician.FormattedName);
            }

            if (Model.OperatingPhysician != null)
            {
                lblOprVal.Text = String.Format("{0:00000} {1}", Model.OperatingPhysician.PhysicianNumber,
                                               Model.OperatingPhysician.FormattedName);
            }

            if (Model.PrimaryCarePhysician != null)
            {
                lblPcpVal.Text = String.Format("{0:00000} {1}", Model.PrimaryCarePhysician.PhysicianNumber,
                                               Model.PrimaryCarePhysician.FormattedName);
            }
        }

        /// <summary>
        ///   OpenViewDocumentsForm - launch the VIWeb browser window to allow the user to scan documents
        ///   associated with the Registratered patient
        /// </summary>
        private void OpenScanDocumentsForm()
        {
            ListOfDocumentsView lst = new ListOfDocumentsView();
            lst.Model = this.Model;
            lst.OpenViewDocumentsForm("SCAN");
        }

        #endregion

        #region Private Properties

        #endregion

        #region Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelTransferERPatToOutPat = new System.Windows.Forms.Panel();
            this.lblAccommodationVal = new System.Windows.Forms.Label();
            this.lineLabel1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.panelActions = new System.Windows.Forms.Panel();
            this.btnPrintFaceSheet = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.btnEditAccount = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnRepeatActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCloseActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.grpPhysicians = new System.Windows.Forms.GroupBox();
            this.lblPcpVal = new System.Windows.Forms.Label();
            this.lblPcp = new System.Windows.Forms.Label();
            this.lblOprVal = new System.Windows.Forms.Label();
            this.lblAttVal = new System.Windows.Forms.Label();
            this.lblAdmVal = new System.Windows.Forms.Label();
            this.lblRefVal = new System.Windows.Forms.Label();
            this.lblOpr = new System.Windows.Forms.Label();
            this.lblAtt = new System.Windows.Forms.Label();
            this.lblAdm = new System.Windows.Forms.Label();
            this.lblRef = new System.Windows.Forms.Label();
            this.infoControl1 = new PatientAccess.UI.CommonControls.InfoControl();
            this.lblTransferTimeVal = new System.Windows.Forms.Label();
            this.lblTransferTime = new System.Windows.Forms.Label();
            this.lblTransferDateVal = new System.Windows.Forms.Label();
            this.lblAccountVal = new System.Windows.Forms.Label();
            this.lblPatientNameVal = new System.Windows.Forms.Label();
            this.lblTransferDate = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblAdmitSource = new System.Windows.Forms.Label();
            this.lblAdmitSourceVal = new System.Windows.Forms.Label();
            this.lblPatientTypeVal = new System.Windows.Forms.Label();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.lblHospitalServiceVal = new System.Windows.Forms.Label();
            this.lblHospitalService = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblLocationVal = new System.Windows.Forms.Label();
            this.lblPrimaryPlanVal = new System.Windows.Forms.Label();
            this.lblPrimaryPlan = new System.Windows.Forms.Label();
            this.lblFinancialClass = new System.Windows.Forms.Label();
            this.lblFinancialClassVal = new System.Windows.Forms.Label();
            this.lblComments = new System.Windows.Forms.Label();
            this.lblCommentsVal = new System.Windows.Forms.Label();
            this.infoControl2 = new PatientAccess.UI.CommonControls.InfoControl();
            this.lblAccommodation = new System.Windows.Forms.Label();
            this.panelUserContext = new System.Windows.Forms.Panel();
            this.userContextView1 = new PatientAccess.UI.UserContextView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.gbScanDocuments = new System.Windows.Forms.GroupBox();
            this.btnScanDocuments = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblScanDocuments = new System.Windows.Forms.Label();
            this.panelTransferERPatToOutPat.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.grpPhysicians.SuspendLayout();
            this.panelUserContext.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbScanDocuments.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTransferERPatToOutPat
            // 
            this.panelTransferERPatToOutPat.BackColor = System.Drawing.Color.White;
            this.panelTransferERPatToOutPat.Controls.Add(this.gbScanDocuments);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAccommodationVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lineLabel1);
            this.panelTransferERPatToOutPat.Controls.Add(this.panelActions);
            this.panelTransferERPatToOutPat.Controls.Add(this.grpPhysicians);
            this.panelTransferERPatToOutPat.Controls.Add(this.infoControl1);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblTransferTimeVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblTransferTime);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblTransferDateVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAccountVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPatientNameVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblTransferDate);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAccount);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPatientName);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAdmitSource);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAdmitSourceVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPatientTypeVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPatientType);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblHospitalServiceVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblHospitalService);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblLocation);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblLocationVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPrimaryPlanVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblPrimaryPlan);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblFinancialClass);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblFinancialClassVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblComments);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblCommentsVal);
            this.panelTransferERPatToOutPat.Controls.Add(this.infoControl2);
            this.panelTransferERPatToOutPat.Controls.Add(this.lblAccommodation);
            this.panelTransferERPatToOutPat.Location = new System.Drawing.Point(8, 64);
            this.panelTransferERPatToOutPat.Name = "panelTransferERPatToOutPat";
            this.panelTransferERPatToOutPat.Size = new System.Drawing.Size(1008, 560);
            this.panelTransferERPatToOutPat.TabIndex = 2;
            // 
            // lblAccommodationVal
            // 
            this.lblAccommodationVal.Location = new System.Drawing.Point(104, 241);
            this.lblAccommodationVal.Name = "lblAccommodationVal";
            this.lblAccommodationVal.Size = new System.Drawing.Size(213, 13);
            this.lblAccommodationVal.TabIndex = 17;
            // 
            // lineLabel1
            // 
            this.lineLabel1.Caption = "Messages";
            this.lineLabel1.Location = new System.Drawing.Point(11, 434);
            this.lineLabel1.Name = "lineLabel1";
            this.lineLabel1.Size = new System.Drawing.Size(975, 18);
            this.lineLabel1.TabIndex = 57;
            this.lineLabel1.TabStop = false;
            // 
            // panelActions
            // 
            this.panelActions.Controls.Add(this.btnPrintFaceSheet);
            this.panelActions.Controls.Add(this.lineLabel2);
            this.panelActions.Controls.Add(this.btnEditAccount);
            this.panelActions.Controls.Add(this.btnRepeatActivity);
            this.panelActions.Controls.Add(this.btnCloseActivity);
            this.panelActions.Location = new System.Drawing.Point(4, 493);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(990, 60);
            this.panelActions.TabIndex = 56;
            // 
            // btnPrintFaceSheet
            // 
            this.btnPrintFaceSheet.Location = new System.Drawing.Point(337, 31);
            this.btnPrintFaceSheet.Message = null;
            this.btnPrintFaceSheet.Name = "btnPrintFaceSheet";
            this.btnPrintFaceSheet.Size = new System.Drawing.Size(125, 23);
            this.btnPrintFaceSheet.TabIndex = 59;
            this.btnPrintFaceSheet.Text = "Pr&int Face Sheet";
            this.btnPrintFaceSheet.Click += new System.EventHandler(this.btnPrintFaceSheet_Click);
            // 
            // lineLabel2
            // 
            this.lineLabel2.Caption = "Next Action";
            this.lineLabel2.Location = new System.Drawing.Point(9, 7);
            this.lineLabel2.Name = "lineLabel2";
            this.lineLabel2.Size = new System.Drawing.Size(971, 18);
            this.lineLabel2.TabIndex = 58;
            this.lineLabel2.TabStop = false;
            // 
            // btnEditAccount
            // 
            this.btnEditAccount.Location = new System.Drawing.Point(204, 31);
            this.btnEditAccount.Message = null;
            this.btnEditAccount.Name = "btnEditAccount";
            this.btnEditAccount.Size = new System.Drawing.Size(125, 23);
            this.btnEditAccount.TabIndex = 2;
            this.btnEditAccount.Text = "Edit/Maintain &Account";
            this.btnEditAccount.Click += new System.EventHandler(this.btnEditAccount_Click);
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.Location = new System.Drawing.Point(109, 31);
            this.btnRepeatActivity.Message = null;
            this.btnRepeatActivity.Name = "btnRepeatActivity";
            this.btnRepeatActivity.Size = new System.Drawing.Size(88, 23);
            this.btnRepeatActivity.TabIndex = 1;
            this.btnRepeatActivity.Text = "Repeat Acti&vity";
            this.btnRepeatActivity.Click += new System.EventHandler(this.btnRepeatActivity_Click);
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.Location = new System.Drawing.Point(14, 31);
            this.btnCloseActivity.Message = null;
            this.btnCloseActivity.Name = "btnCloseActivity";
            this.btnCloseActivity.Size = new System.Drawing.Size(88, 23);
            this.btnCloseActivity.TabIndex = 0;
            this.btnCloseActivity.Text = "C&lose Activity";
            this.btnCloseActivity.Click += new System.EventHandler(this.btnCloseActivity_Click);
            // 
            // grpPhysicians
            // 
            this.grpPhysicians.Controls.Add(this.lblPcpVal);
            this.grpPhysicians.Controls.Add(this.lblPcp);
            this.grpPhysicians.Controls.Add(this.lblOprVal);
            this.grpPhysicians.Controls.Add(this.lblAttVal);
            this.grpPhysicians.Controls.Add(this.lblAdmVal);
            this.grpPhysicians.Controls.Add(this.lblRefVal);
            this.grpPhysicians.Controls.Add(this.lblOpr);
            this.grpPhysicians.Controls.Add(this.lblAtt);
            this.grpPhysicians.Controls.Add(this.lblAdm);
            this.grpPhysicians.Controls.Add(this.lblRef);
            this.grpPhysicians.Location = new System.Drawing.Point(398, 43);
            this.grpPhysicians.Name = "grpPhysicians";
            this.grpPhysicians.Size = new System.Drawing.Size(360, 112);
            this.grpPhysicians.TabIndex = 55;
            this.grpPhysicians.TabStop = false;
            this.grpPhysicians.Text = "Physicians";
            // 
            // lblPcpVal
            // 
            this.lblPcpVal.Location = new System.Drawing.Point(49, 91);
            this.lblPcpVal.Name = "lblPcpVal";
            this.lblPcpVal.Size = new System.Drawing.Size(271, 14);
            this.lblPcpVal.TabIndex = 33;
            // 
            // lblPcp
            // 
            this.lblPcp.Location = new System.Drawing.Point(8, 88);
            this.lblPcp.Name = "lblPcp";
            this.lblPcp.Size = new System.Drawing.Size(32, 16);
            this.lblPcp.TabIndex = 32;
            this.lblPcp.Text = "PCP:";
            // 
            // lblOprVal
            // 
            this.lblOprVal.Location = new System.Drawing.Point(49, 74);
            this.lblOprVal.Name = "lblOprVal";
            this.lblOprVal.Size = new System.Drawing.Size(271, 14);
            this.lblOprVal.TabIndex = 31;
            // 
            // lblAttVal
            // 
            this.lblAttVal.Location = new System.Drawing.Point(49, 56);
            this.lblAttVal.Name = "lblAttVal";
            this.lblAttVal.Size = new System.Drawing.Size(271, 14);
            this.lblAttVal.TabIndex = 30;
            // 
            // lblAdmVal
            // 
            this.lblAdmVal.Location = new System.Drawing.Point(49, 40);
            this.lblAdmVal.Name = "lblAdmVal";
            this.lblAdmVal.Size = new System.Drawing.Size(271, 14);
            this.lblAdmVal.TabIndex = 29;
            // 
            // lblRefVal
            // 
            this.lblRefVal.Location = new System.Drawing.Point(48, 24);
            this.lblRefVal.Name = "lblRefVal";
            this.lblRefVal.Size = new System.Drawing.Size(280, 13);
            this.lblRefVal.TabIndex = 28;
            // 
            // lblOpr
            // 
            this.lblOpr.Location = new System.Drawing.Point(8, 72);
            this.lblOpr.Name = "lblOpr";
            this.lblOpr.Size = new System.Drawing.Size(32, 16);
            this.lblOpr.TabIndex = 25;
            this.lblOpr.Text = "Opr:";
            // 
            // lblAtt
            // 
            this.lblAtt.Location = new System.Drawing.Point(8, 56);
            this.lblAtt.Name = "lblAtt";
            this.lblAtt.Size = new System.Drawing.Size(24, 16);
            this.lblAtt.TabIndex = 24;
            this.lblAtt.Text = "Att:";
            // 
            // lblAdm
            // 
            this.lblAdm.Location = new System.Drawing.Point(8, 40);
            this.lblAdm.Name = "lblAdm";
            this.lblAdm.Size = new System.Drawing.Size(32, 16);
            this.lblAdm.TabIndex = 23;
            this.lblAdm.Text = "Adm:";
            // 
            // lblRef
            // 
            this.lblRef.Location = new System.Drawing.Point(8, 24);
            this.lblRef.Name = "lblRef";
            this.lblRef.Size = new System.Drawing.Size(32, 16);
            this.lblRef.TabIndex = 22;
            this.lblRef.Text = "Ref:";
            // 
            // infoControl1
            // 
            this.infoControl1.Location = new System.Drawing.Point(11, 7);
            this.infoControl1.Message = "";
            this.infoControl1.Name = "infoControl1";
            this.infoControl1.Size = new System.Drawing.Size(965, 32);
            this.infoControl1.TabIndex = 54;
            this.infoControl1.TabStop = false;
            // 
            // lblTransferTimeVal
            // 
            this.lblTransferTimeVal.Location = new System.Drawing.Point(272, 263);
            this.lblTransferTimeVal.Name = "lblTransferTimeVal";
            this.lblTransferTimeVal.Size = new System.Drawing.Size(45, 16);
            this.lblTransferTimeVal.TabIndex = 49;
            // 
            // lblTransferTime
            // 
            this.lblTransferTime.Location = new System.Drawing.Point(233, 262);
            this.lblTransferTime.Name = "lblTransferTime";
            this.lblTransferTime.Size = new System.Drawing.Size(33, 22);
            this.lblTransferTime.TabIndex = 48;
            this.lblTransferTime.Text = "Time:";
            // 
            // lblTransferDateVal
            // 
            this.lblTransferDateVal.Location = new System.Drawing.Point(104, 263);
            this.lblTransferDateVal.Name = "lblTransferDateVal";
            this.lblTransferDateVal.Size = new System.Drawing.Size(88, 17);
            this.lblTransferDateVal.TabIndex = 18;
            // 
            // lblAccountVal
            // 
            this.lblAccountVal.Location = new System.Drawing.Point(104, 70);
            this.lblAccountVal.Name = "lblAccountVal";
            this.lblAccountVal.Size = new System.Drawing.Size(216, 16);
            this.lblAccountVal.TabIndex = 17;
            // 
            // lblPatientNameVal
            // 
            this.lblPatientNameVal.Location = new System.Drawing.Point(104, 46);
            this.lblPatientNameVal.Name = "lblPatientNameVal";
            this.lblPatientNameVal.Size = new System.Drawing.Size(212, 16);
            this.lblPatientNameVal.TabIndex = 16;
            // 
            // lblTransferDate
            // 
            this.lblTransferDate.Location = new System.Drawing.Point(11, 264);
            this.lblTransferDate.Name = "lblTransferDate";
            this.lblTransferDate.Size = new System.Drawing.Size(79, 15);
            this.lblTransferDate.TabIndex = 2;
            this.lblTransferDate.Text = "Transfer date:";
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point(11, 69);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(74, 15);
            this.lblAccount.TabIndex = 1;
            this.lblAccount.Text = "Account:";
            // 
            // lblPatientName
            // 
            this.lblPatientName.Location = new System.Drawing.Point(11, 46);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(81, 23);
            this.lblPatientName.TabIndex = 0;
            this.lblPatientName.Text = "Patient name:";
            // 
            // lblAdmitSource
            // 
            this.lblAdmitSource.Location = new System.Drawing.Point(11, 152);
            this.lblAdmitSource.Name = "lblAdmitSource";
            this.lblAdmitSource.Size = new System.Drawing.Size(76, 15);
            this.lblAdmitSource.TabIndex = 12;
            this.lblAdmitSource.Text = "Admit Source:";
            // 
            // lblAdmitSourceVal
            // 
            this.lblAdmitSourceVal.Location = new System.Drawing.Point(104, 152);
            this.lblAdmitSourceVal.Name = "lblAdmitSourceVal";
            this.lblAdmitSourceVal.Size = new System.Drawing.Size(214, 15);
            this.lblAdmitSourceVal.TabIndex = 21;
            // 
            // lblPatientTypeVal
            // 
            this.lblPatientTypeVal.Location = new System.Drawing.Point(104, 175);
            this.lblPatientTypeVal.Name = "lblPatientTypeVal";
            this.lblPatientTypeVal.Size = new System.Drawing.Size(216, 16);
            this.lblPatientTypeVal.TabIndex = 22;
            // 
            // lblPatientType
            // 
            this.lblPatientType.Location = new System.Drawing.Point(11, 175);
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size(75, 16);
            this.lblPatientType.TabIndex = 13;
            this.lblPatientType.Text = "Patient type:";
            // 
            // lblHospitalServiceVal
            // 
            this.lblHospitalServiceVal.Location = new System.Drawing.Point(104, 198);
            this.lblHospitalServiceVal.Name = "lblHospitalServiceVal";
            this.lblHospitalServiceVal.Size = new System.Drawing.Size(211, 14);
            this.lblHospitalServiceVal.TabIndex = 23;
            // 
            // lblHospitalService
            // 
            this.lblHospitalService.Location = new System.Drawing.Point(11, 198);
            this.lblHospitalService.Name = "lblHospitalService";
            this.lblHospitalService.Size = new System.Drawing.Size(87, 14);
            this.lblHospitalService.TabIndex = 14;
            this.lblHospitalService.Text = "Hospital service:";
            // 
            // lblLocation
            // 
            this.lblLocation.Location = new System.Drawing.Point(11, 220);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(67, 14);
            this.lblLocation.TabIndex = 15;
            this.lblLocation.Text = "Location:";
            // 
            // lblLocationVal
            // 
            this.lblLocationVal.Location = new System.Drawing.Point(104, 220);
            this.lblLocationVal.Name = "lblLocationVal";
            this.lblLocationVal.Size = new System.Drawing.Size(212, 13);
            this.lblLocationVal.TabIndex = 24;
            // 
            // lblPrimaryPlanVal
            // 
            this.lblPrimaryPlanVal.Location = new System.Drawing.Point(104, 109);
            this.lblPrimaryPlanVal.Name = "lblPrimaryPlanVal";
            this.lblPrimaryPlanVal.Size = new System.Drawing.Size(216, 13);
            this.lblPrimaryPlanVal.TabIndex = 17;
            // 
            // lblPrimaryPlan
            // 
            this.lblPrimaryPlan.Location = new System.Drawing.Point(11, 108);
            this.lblPrimaryPlan.Name = "lblPrimaryPlan";
            this.lblPrimaryPlan.Size = new System.Drawing.Size(77, 14);
            this.lblPrimaryPlan.TabIndex = 1;
            this.lblPrimaryPlan.Text = "Primary plan:";
            // 
            // lblFinancialClass
            // 
            this.lblFinancialClass.Location = new System.Drawing.Point(11, 129);
            this.lblFinancialClass.Name = "lblFinancialClass";
            this.lblFinancialClass.Size = new System.Drawing.Size(82, 16);
            this.lblFinancialClass.TabIndex = 1;
            this.lblFinancialClass.Text = "Financial class:";
            // 
            // lblFinancialClassVal
            // 
            this.lblFinancialClassVal.Location = new System.Drawing.Point(104, 130);
            this.lblFinancialClassVal.Name = "lblFinancialClassVal";
            this.lblFinancialClassVal.Size = new System.Drawing.Size(216, 13);
            this.lblFinancialClassVal.TabIndex = 17;
            // 
            // lblComments
            // 
            this.lblComments.Location = new System.Drawing.Point(11, 285);
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size(67, 17);
            this.lblComments.TabIndex = 1;
            this.lblComments.Text = "Comments:";
            // 
            // lblCommentsVal
            // 
            this.lblCommentsVal.Location = new System.Drawing.Point(104, 286);
            this.lblCommentsVal.Name = "lblCommentsVal";
            this.lblCommentsVal.Size = new System.Drawing.Size(284, 45);
            this.lblCommentsVal.TabIndex = 17;
            // 
            // infoControl2
            // 
            this.infoControl2.Location = new System.Drawing.Point(17, 463);
            this.infoControl2.Message =
                "Update privacy options, emergency contact, patient liability, or any other inform" +
                "ation, if needed, by clicking the Edit Account button.";
            this.infoControl2.Name = "infoControl2";
            this.infoControl2.Size = new System.Drawing.Size(955, 22);
            this.infoControl2.TabIndex = 54;
            this.infoControl2.TabStop = false;
            // 
            // lblAccommodation
            // 
            this.lblAccommodation.Location = new System.Drawing.Point(11, 241);
            this.lblAccommodation.Name = "lblAccommodation";
            this.lblAccommodation.Size = new System.Drawing.Size(89, 13);
            this.lblAccommodation.TabIndex = 1;
            this.lblAccommodation.Text = "Accommodation:";
            // 
            // panelUserContext
            // 
            this.panelUserContext.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (209)))),
                                                                            ((int) (((byte) (228)))),
                                                                            ((int) (((byte) (243)))));
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
            //this.userContextView1.Description = "Transfer ER to Outpatient - Submitted";
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
            // gbScanDocuments
            // 
            this.gbScanDocuments.Controls.Add(this.btnScanDocuments);
            this.gbScanDocuments.Controls.Add(this.lblScanDocuments);
            this.gbScanDocuments.Location = new System.Drawing.Point(14, 342);
            this.gbScanDocuments.Name = "gbScanDocuments";
            this.gbScanDocuments.Size = new System.Drawing.Size(322, 84);
            this.gbScanDocuments.TabIndex = 58;
            this.gbScanDocuments.TabStop = false;
            this.gbScanDocuments.Text = "Scan documents";
            // 
            // btnScanDocuments
            // 
            this.btnScanDocuments.Location = new System.Drawing.Point(8, 53);
            this.btnScanDocuments.Message = null;
            this.btnScanDocuments.Name = "btnScanDocuments";
            this.btnScanDocuments.Size = new System.Drawing.Size(105, 23);
            this.btnScanDocuments.TabIndex = 1;
            this.btnScanDocuments.Text = "Scan Document...";
            this.btnScanDocuments.Click += new System.EventHandler(this.btnScanDocuments_Click);
            // 
            // lblScanDocuments
            // 
            this.lblScanDocuments.Location = new System.Drawing.Point(8, 18);
            this.lblScanDocuments.Name = "lblScanDocuments";
            this.lblScanDocuments.Size = new System.Drawing.Size(290, 29);
            this.lblScanDocuments.TabIndex = 0;
            this.lblScanDocuments.Text =
                "Scan available documents now for this account. To scan documents later, use the E" +
                "dit/Maintain Account activity.";
            // 
            // TransferERPatToOutPatConfirmView
            // 
            this.AcceptButton = this.btnCloseActivity;
            this.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (209)))), ((int) (((byte) (228)))),
                                                           ((int) (((byte) (243)))));
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelTransferERPatToOutPat);
            this.Controls.Add(this.panelUserContext);
            this.Name = "EmergencyPatientAndOutPatientConfirmView";
            this.Size = new System.Drawing.Size(1024, 632);
            this.panelTransferERPatToOutPat.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.grpPhysicians.ResumeLayout(false);
            this.panelUserContext.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.gbScanDocuments.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        #region Data Elements

        private readonly IContainer components = null;
        private LoggingButton btnCloseActivity;
        private ClickOnceLoggingButton btnEditAccount;
        private LoggingButton btnPrintFaceSheet;
        private LoggingButton btnRepeatActivity;
        private LoggingButton btnScanDocuments;
        private GroupBox gbScanDocuments;
        private GroupBox grpPhysicians;
        private InfoControl infoControl1;
        private InfoControl infoControl2;
        private Label lblAccommodation;
        private Label lblAccommodationVal;
        private Label lblAccount;
        private Label lblAccountVal;
        private Label lblAdm;
        private Label lblAdmVal;
        private Label lblAdmitSource;
        private Label lblAdmitSourceVal;
        private Label lblAtt;
        private Label lblAttVal;
        private Label lblComments;
        private Label lblCommentsVal;
        private Label lblFinancialClass;
        private Label lblFinancialClassVal;
        private Label lblHospitalService;
        private Label lblHospitalServiceVal;
        private Label lblLocation;
        private Label lblLocationVal;
        private Label lblOpr;
        private Label lblOprVal;
        private Label lblPatientName;
        private Label lblPatientNameVal;
        private Label lblPatientType;
        private Label lblPatientTypeVal;
        private Label lblPcp;
        private Label lblPcpVal;
        private Label lblPrimaryPlan;
        private Label lblPrimaryPlanVal;
        private Label lblRef;
        private Label lblRefVal;
        private Label lblScanDocuments;
        private Label lblTransferDate;
        private Label lblTransferDateVal;
        private Label lblTransferTime;
        private Label lblTransferTimeVal;
        private LineLabel lineLabel1;
        private LineLabel lineLabel2;
        private Panel panel1;
        private Panel panelActions;
        private Panel panelTransferERPatToOutPat;
        private Panel panelUserContext;
        private PatientContextView patientContextView1;
        private UserContextView userContextView1;

        #endregion

        #region Constants

        #endregion
    }
}