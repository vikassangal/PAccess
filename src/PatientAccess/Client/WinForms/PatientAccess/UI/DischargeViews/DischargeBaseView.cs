using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DocumentImagingViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Reports.FaceSheet;
using log4net;
using Role = Peradigm.Framework.Domain.Security.Role;

namespace PatientAccess.UI.DischargeViews
{
    /// <summary>
    /// Summary description for DischargeBaseView.
    /// </summary>
    [Serializable]
    public class DischargeBaseView : ControlView
    {
        #region Events
        public event EventHandler RepeatActivity;
        public event EventHandler EditAccount;
        #endregion

        #region Event Handlers

        private void btnCancel_Click( object sender, EventArgs e )
        {
            if (AccountActivityService.ConfirmCancelActivity(
                sender, new LooseArgs( Model ) ))
            {
                // if cancel button activated then cancel the worker...
                if (backgroundWorker != null)
                    backgroundWorker.CancelAsync();

                Dispose();
            }
            else
            {
                btnCancel.Enabled = true;
            }
        }

        protected void btnOk_Click( object sender, EventArgs e )
        {

            SetViewForUserRole();
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this,
                new LooseArgs( Model ) );
        }

        private void SetViewForUserRole()
        {
            if (DischargeTransferUser)
            {
                DisableScan();
                DisableEditAccount();
            }
        }

        private bool DischargeTransferUser
        {
            get { return User.GetCurrent().SecurityUser.IsInRole(Role.DISCHARGE_TRANSFER_USER); }
        }

        private void DisableEditAccount()
        {
            btnEditAccount.Enabled = false;
        }

        private void DisableScan()
        {
            gbScanDocuments.Enabled = false;
        }

        private void btnEditAccount_Click( object sender, EventArgs e )
        {
            try
            {
                AccountView.CloseVIweb();
                if (AccountLockStatus.IsAccountLocked( Model, User.GetCurrent() ))
                {
                    MessageBox.Show( UIErrorMessages.NO_EDIT_RECORD_LOCKED, "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                    return;
                }
                
                Cursor = Cursors.WaitCursor;

                if ( Model.IsShortRegisteredNonDayCareAccount() )
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
                    if (!AccountActivityService.PlaceLockOn( Model, String.Empty ))
                    {
                        Cursor = Cursors.Default;
                        return;
                    }
                }

                EditAccount( this, new LooseArgs( Model ) );

                Cursor = Cursors.Default;
            }
            catch (AccountNotFoundException)
            {
                Cursor = Cursors.Default;
                MessageBox.Show( UIErrorMessages.ACTIVITY_CANNOT_PROCEED, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
            }
        }

        private void btnRepeatActivity_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            RepeatActivity( this, new LooseArgs( new Patient() ) );
        }

        private void btnCloseActivity_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this, new LooseArgs( Model ) );

            if (backgroundWorker != null)
                backgroundWorker.CancelAsync();

            Dispose();
            ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen( this, null );
        }

        private void cmbDischargeDisposition_SelectedIndexChanged( object sender, EventArgs e )
        {

        }

        private void dtpDischargeDate_ValueChanged( object sender, EventArgs e )
        {
            if (dtpDischargeDate.Checked)
            {
                DateTime dt = dtpDischargeDate.Value;
                mtbDischargeDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            }
            else
            {
                mtbDischargeDate.Text = String.Empty;
            }
        }

        private void btnPrintFaceSheet_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            var faceSheetPrintService = new PrintService( Model );
            faceSheetPrintService.Print();
        }

        private void btnScanDocuments_Click( object sender, EventArgs e )
        {
            OpenScanDocumentsForm();
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if (Model != null)
            {
                DisplayPatientContext();

                //PopulateControls
                lblPatientName.Text = Model.Patient.FormattedName;
                lblAccount.Text = Model.AccountNumber.ToString();
                lblPatientType.Text = Model.KindOfVisit.DisplayString;

                lblAdmitDate.Text = CommonFormatting.LongDateFormat( Model.AdmitDate );

                lblAdmitTime.Text = Model.AdmitDate.TimeOfDay.Hours +
                    ":" + Model.AdmitDate.TimeOfDay.Minutes;

                ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
                Model.DischargeDate = timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                                              User.GetCurrent().Facility.DSTOffset );

                if (Model.DischargeDate.Year.ToString() != "1" || !ValidPatientType())
                {
                    mtbDischargeDate.UnMaskedText = CommonFormatting.MaskedDateFormat( Model.DischargeDate );

                    if (Model.DischargeDate.Hour != 0 ||
                        Model.DischargeDate.Minute != 0)
                    {
                        mtbDischargeTime.UnMaskedText = CommonFormatting.MaskedTimeFormat( Model.DischargeDate );
                    }
                    else
                    {
                        mtbDischargeTime.UnMaskedText = string.Empty;
                    }

                    lblInstructionalMessage.Text = UIErrorMessages.DISCHARGE_INTENT_MSG;

                    DisplayDischargeDispositions();

                    if (Model.DischargeDisposition != null)
                    {
                        int dischargeDispositionSelectedIndex = -1;
                        dischargeDispositionSelectedIndex =
                            cmbDischargeDisposition.FindString( Model.DischargeDisposition.ToString() );

                        if (dischargeDispositionSelectedIndex != -1)
                        {
                            cmbDischargeDisposition.SelectedIndex = dischargeDispositionSelectedIndex;
                        }
                    }

                    if (Model.Location != null)
                    {
                        lblDischargeLocation.Text = Model.Location.DisplayString;
                    }

                    btnOk.Enabled = false;
                }
                else
                {
                    CheckForValuables();
                    CheckForRemainingActionItems();
                }

                btnCancel.Focus();
            }
        }

        public override void UpdateModel()
        {
        }

        protected void RemoveMaskedTextBorder()
        {
            mtbDischargeDate.Visible = false;
            mtbDischargeTime.Visible = false;
            dtpDischargeDate.Visible = false;
            cmbDischargeDisposition.Visible = false;

            lblDischargeDateVal.Text = mtbDischargeDate.Text;
            lblDischargeDateVal.Visible = true;

            lblDischargeTimeVal.Text = mtbDischargeTime.Text;
            lblDischargeTimeVal.Visible = true;

            if (cmbDischargeDisposition.SelectedItem != null)
            {
                lblDischargeDispositionVal.Text = cmbDischargeDisposition.SelectedItem.ToString();
            }
            lblDischargeDispositionVal.Visible = true;
        }

        protected bool ValidPatientType()
        {
            if (Model.Activity.GetType().Name ==  "PreDischargeActivity" ||
                Model.Activity.GetType().Name == "DischargeActivity" ||
                Model.Activity.GetType().Name == "CancelOutpatientDischargeActivity")
            {
                if (Model.KindOfVisit.Code == VisitType.PREREG_PATIENT ||
                    Model.KindOfVisit.Code == VisitType.RECURRING_PATIENT)
                {
                    lblInstructionalMessage.Text = UIErrorMessages.DISCHARGE_INTENT_MSG;
                    btnOk.Enabled = false;
                    return false;
                }
            }
            else
            {
                if (Model.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT ||
                    Model.KindOfVisit.Code == VisitType.PREREG_PATIENT ||
                    Model.KindOfVisit.Code == VisitType.RECURRING_PATIENT)
                {
                    lblInstructionalMessage.Text = UIErrorMessages.DISCHARGE_INTENT_MSG;
                    btnOk.Enabled = false;
                    return false;
                }
            }

            return true;
        }

        protected void DisplayPatientContext()
        {
            if (Model.Patient != null)
            {
                patientContextView1.Model = Model.Patient;
                patientContextView1.Account = Model;
                patientContextView1.UpdateView();
            }
        }

        protected void CheckForRemainingActionItems()
        {
            Model.ActionsLoader = new ActionLoader( Model );

            ICollection remainingActions = Model.GetAllRemainingActions();
            lblOutstandingActionItemsMsg.Text = remainingActions.Count > 0 ? 
                                                UIErrorMessages.OUTSTANDING_ACTION_ITEMS_MSG : 
                                                String.Empty;
        }

        protected void CheckForValuables()
        {
            lblMessages.Text = Model.ValuablesAreTaken.Code == YesNoFlag.CODE_YES ? 
                                "Return valuables to the patient." : 
                                string.Empty;
        }

        protected bool CheckAgeOver65AtDischarge()
        {
            Patient patient = Model.Patient;
            int ageAtAdmission = patient.AgeInYearsFor( Model.AdmitDate );
            int ageAtDischarge = patient.AgeInYearsFor( Model.DischargeDate );

            bool blnAgeOver65 = ageAtAdmission < AGE_SIXTY_FIVE &&
                                ( ageAtDischarge == AGE_SIXTY_FIVE || ageAtDischarge > AGE_SIXTY_FIVE );

            return blnAgeOver65;
        }

        protected void DisplayDischargeDispositions()
        {
            if (cmbDischargeDisposition.Items.Count == 0)
            {
                try
                {
                    cmbDischargeDisposition.BeginUpdate();

                    IDischargeBroker broker = BrokerFactory.BrokerOfType<IDischargeBroker>();
                    ICollection dispositions = broker.AllDischargeDispositions( User.GetCurrent().Facility.Oid );

                    cmbDischargeDisposition.ValueMember = "Value";
                    cmbDischargeDisposition.DisplayMember = "Key";

                    foreach (DischargeDisposition dischargeDisposition in dispositions)
                    {
                        cmbDischargeDisposition.Items.Add( dischargeDisposition );
                    }
                }
                finally
                {
                    cmbDischargeDisposition.EndUpdate();
                }
            }
        }

        private void BeforeWork()
        {
            Cursor = Cursors.AppStarting;

            ProgressPanel1.Visible = true;
            ProgressPanel1.BringToFront();
        }

        private void DoSaveAccount( object sender, DoWorkEventArgs e )
        {
            // Sanjeev Kumar: do not add any try/catch statements within the DoWork.
            // Exceptions are caught in backgroundworkers automatically in the RunWorkerComplete
            // through e.Error, which can be checked.

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            Model.Facility = User.GetCurrent().Facility;

            Account anAccount = Model;
            anAccount.ActionsLoader = new ActionLoader( anAccount );

            Activity currentActivity = anAccount.Activity;
            currentActivity.AppUser = User.GetCurrent();

            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            if (broker != null)
            {
                AccountSaveResults results = broker.Save( anAccount, currentActivity );
                results.SetResultsTo( Model );
            }

            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if (IsDisposed || Disposing)
                return;

            // clear any FUS notes that were manually entered so that - if the View FUS notes icon/menu option is selected,
            // the newly added notes do not show twice.

            PatientAccessView patientAccessView = ViewFactory.Instance.CreateView<PatientAccessView>();

            if (patientAccessView.Model != null)
            {
                (( Account )patientAccessView.Model ).ClearFusNotes();
            }

            // place post completion operations here...
            ProgressPanel1.Visible = false;
            ProgressPanel1.SendToBack();

            Cursor = Cursors.Default;
        }

        protected void SaveAccount()
        {
            if (backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoSaveAccount;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }
        #endregion

        #region Properties

        protected new Account Model
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
        #endregion

        #region Private Methods

        protected bool IsPBARAvailable()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            return facilityBroker.IsDatabaseAvailableFor( User.GetCurrent().Facility.ConnectionSpec.ServerIP );
        }

        /// <summary>
        /// OpenViewDocumentsForm - launch the VIWeb browser window to allow the user to scan documents
        /// associated with the Registered patient
        /// </summary>
        private void OpenScanDocumentsForm()
        {
            ListOfDocumentsView lst = new ListOfDocumentsView();
            lst.Model = this.Model;
            lst.OpenViewDocumentsForm("SCAN");
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( DischargeBaseView ) );
            this.userContextView1 = new PatientAccess.UI.UserContextView();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.panelPatientContext = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gbScanDocuments = new System.Windows.Forms.GroupBox();
            this.btnScanDocuments = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblScanDocuments = new System.Windows.Forms.Label();
            this.lblCurrentOccupant = new System.Windows.Forms.Label();
            this.lblCurOccupant = new System.Windows.Forms.Label();
            this.cmbDischargeDisposition = new System.Windows.Forms.ComboBox();
            this.mtbDischargeDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.dtpDischargeDate = new System.Windows.Forms.DateTimePicker();
            this.mtbDischargeTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblDischargeLocation = new System.Windows.Forms.Label();
            this.lblAdmitTime = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblAdmitDate = new System.Windows.Forms.Label();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblInstructionalMessage = new System.Windows.Forms.Label();
            this.panelMessages = new System.Windows.Forms.Panel();
            this.panelActions = new System.Windows.Forms.Panel();
            this.btnPrintFaceSheet = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCloseActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnRepeatActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnEditAccount = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblOutstandingActionItemsMsg = new System.Windows.Forms.Label();
            this.lblMessages = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.lblDischargeDispositionVal = new System.Windows.Forms.Label();
            this.lblDischargeTimeVal = new System.Windows.Forms.Label();
            this.lblDischargeDateVal = new System.Windows.Forms.Label();
            this.btnCancel = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnOk = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.ProgressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.lblNextAction = new System.Windows.Forms.Label();
            this.lblNextActionLine = new System.Windows.Forms.Label();
            this.panelPatientContext.SuspendLayout();
            this.panel2.SuspendLayout();
            this.gbScanDocuments.SuspendLayout();
            this.panelMessages.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView1
            // 
            this.userContextView1.BackColor = System.Drawing.SystemColors.Control;
            this.userContextView1.Description = "";
            this.userContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.userContextView1.Model = null;
            this.userContextView1.Name = "userContextView1";
            this.userContextView1.Size = new System.Drawing.Size( 1024, 23 );
            this.userContextView1.TabIndex = 0;
            // 
            // patientContextView1
            // 
            this.patientContextView1.Account = null;
            this.patientContextView1.BackColor = System.Drawing.Color.White;
            this.patientContextView1.DateOfBirthText = "";
            this.patientContextView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView1.GenderLabelText = "";
            this.patientContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.patientContextView1.Model = null;
            this.patientContextView1.Name = "patientContextView1";
            this.patientContextView1.PatientNameText = "";
            this.patientContextView1.Size = new System.Drawing.Size( 997, 25 );
            this.patientContextView1.SocialSecurityNumber = "";
            this.patientContextView1.TabIndex = 0;
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.BackColor = System.Drawing.Color.White;
            this.panelPatientContext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPatientContext.Controls.Add( this.patientContextView1 );
            this.panelPatientContext.Location = new System.Drawing.Point( 10, 28 );
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new System.Drawing.Size( 999, 27 );
            this.panelPatientContext.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add( this.gbScanDocuments );
            this.panel2.Controls.Add( this.lblCurrentOccupant );
            this.panel2.Controls.Add( this.lblCurOccupant );
            this.panel2.Controls.Add( this.cmbDischargeDisposition );
            this.panel2.Controls.Add( this.mtbDischargeDate );
            this.panel2.Controls.Add( this.dtpDischargeDate );
            this.panel2.Controls.Add( this.mtbDischargeTime );
            this.panel2.Controls.Add( this.lblDischargeLocation );
            this.panel2.Controls.Add( this.lblAdmitTime );
            this.panel2.Controls.Add( this.label13 );
            this.panel2.Controls.Add( this.label12 );
            this.panel2.Controls.Add( this.lblAdmitDate );
            this.panel2.Controls.Add( this.lblPatientType );
            this.panel2.Controls.Add( this.label9 );
            this.panel2.Controls.Add( this.label8 );
            this.panel2.Controls.Add( this.label7 );
            this.panel2.Controls.Add( this.label6 );
            this.panel2.Controls.Add( this.label5 );
            this.panel2.Controls.Add( this.lblAccount );
            this.panel2.Controls.Add( this.label3 );
            this.panel2.Controls.Add( this.lblPatientName );
            this.panel2.Controls.Add( this.label1 );
            this.panel2.Controls.Add( this.lblInstructionalMessage );
            this.panel2.Controls.Add( this.panelMessages );
            this.panel2.Controls.Add( this.lblDischargeDispositionVal );
            this.panel2.Controls.Add( this.lblDischargeTimeVal );
            this.panel2.Controls.Add( this.lblDischargeDateVal );
            this.panel2.Location = new System.Drawing.Point( 10, 67 );
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size( 999, 519 );
            this.panel2.TabIndex = 0;
            this.panel2.TabStop = true;
            // 
            // gbScanDocuments
            // 
            this.gbScanDocuments.Controls.Add( this.btnScanDocuments );
            this.gbScanDocuments.Controls.Add( this.lblScanDocuments );
            this.gbScanDocuments.Location = new System.Drawing.Point( 14, 261 );
            this.gbScanDocuments.Name = "gbScanDocuments";
            this.gbScanDocuments.Size = new System.Drawing.Size( 322, 84 );
            this.gbScanDocuments.TabIndex = 8;
            this.gbScanDocuments.TabStop = false;
            this.gbScanDocuments.Text = "Scan documents";
            this.gbScanDocuments.Visible = false;
            // 
            // btnScanDocuments
            // 
            this.btnScanDocuments.Location = new System.Drawing.Point( 8, 53 );
            this.btnScanDocuments.Message = null;
            this.btnScanDocuments.Name = "btnScanDocuments";
            this.btnScanDocuments.Size = new System.Drawing.Size( 105, 23 );
            this.btnScanDocuments.TabIndex = 1;
            this.btnScanDocuments.Text = "Scan Document...";
            this.btnScanDocuments.Click += new System.EventHandler( this.btnScanDocuments_Click );
            // 
            // lblScanDocuments
            // 
            this.lblScanDocuments.Location = new System.Drawing.Point( 8, 18 );
            this.lblScanDocuments.Name = "lblScanDocuments";
            this.lblScanDocuments.Size = new System.Drawing.Size( 290, 29 );
            this.lblScanDocuments.TabIndex = 0;
            this.lblScanDocuments.Text = "Scan available documents now for this account. To scan documents later, use the E" +
                "dit/Maintain Account activity.";
            // 
            // lblCurrentOccupant
            // 
            this.lblCurrentOccupant.Location = new System.Drawing.Point( 176, 238 );
            this.lblCurrentOccupant.Name = "lblCurrentOccupant";
            this.lblCurrentOccupant.Size = new System.Drawing.Size( 174, 13 );
            this.lblCurrentOccupant.TabIndex = 0;
            // 
            // lblCurOccupant
            // 
            this.lblCurOccupant.Location = new System.Drawing.Point( 11, 238 );
            this.lblCurOccupant.Name = "lblCurOccupant";
            this.lblCurOccupant.Size = new System.Drawing.Size( 149, 13 );
            this.lblCurOccupant.TabIndex = 0;
            this.lblCurOccupant.Text = "Current occupant of location:";
            // 
            // cmbDischargeDisposition
            // 
            this.cmbDischargeDisposition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDischargeDisposition.Location = new System.Drawing.Point( 128, 188 );
            this.cmbDischargeDisposition.Name = "cmbDischargeDisposition";
            this.cmbDischargeDisposition.Size = new System.Drawing.Size( 208, 21 );
            this.cmbDischargeDisposition.TabIndex = 4;
            this.cmbDischargeDisposition.SelectedIndexChanged += new System.EventHandler( this.cmbDischargeDisposition_SelectedIndexChanged );
            // 
            // mtbDischargeDate
            // 
            this.mtbDischargeDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDischargeDate.KeyPressExpression = "^\\d*$";
            this.mtbDischargeDate.Location = new System.Drawing.Point( 99, 162 );
            this.mtbDischargeDate.Mask = "  /  /";
            this.mtbDischargeDate.Name = "mtbDischargeDate";
            this.mtbDischargeDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbDischargeDate.TabIndex = 1;
            this.mtbDischargeDate.ValidationExpression = resources.GetString( "mtbDischargeDate.ValidationExpression" );

            // 
            // dtpDischargeDate
            // 
            this.dtpDischargeDate.Location = new System.Drawing.Point( 168, 162 );
            this.dtpDischargeDate.Name = "dtpDischargeDate";
            this.dtpDischargeDate.Size = new System.Drawing.Size( 21, 20 );
            this.dtpDischargeDate.TabIndex = 2;
            this.dtpDischargeDate.ValueChanged += new System.EventHandler( this.dtpDischargeDate_ValueChanged );
            // 
            // mtbDischargeTime
            // 
            this.mtbDischargeTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDischargeTime.KeyPressExpression = "^\\d*$";
            this.mtbDischargeTime.Location = new System.Drawing.Point( 245, 162 );
            this.mtbDischargeTime.Mask = "  :";
            this.mtbDischargeTime.MaxLength = 5;
            this.mtbDischargeTime.Multiline = true;
            this.mtbDischargeTime.Name = "mtbDischargeTime";
            this.mtbDischargeTime.Size = new System.Drawing.Size( 37, 20 );
            this.mtbDischargeTime.TabIndex = 3;
            this.mtbDischargeTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";

            // 
            // lblDischargeLocation
            // 
            this.lblDischargeLocation.Location = new System.Drawing.Point( 176, 216 );
            this.lblDischargeLocation.Name = "lblDischargeLocation";
            this.lblDischargeLocation.Size = new System.Drawing.Size( 100, 13 );
            this.lblDischargeLocation.TabIndex = 0;
            // 
            // lblAdmitTime
            // 
            this.lblAdmitTime.Location = new System.Drawing.Point( 243, 141 );
            this.lblAdmitTime.Name = "lblAdmitTime";
            this.lblAdmitTime.Size = new System.Drawing.Size( 56, 13 );
            this.lblAdmitTime.TabIndex = 0;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point( 210, 166 );
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size( 33, 13 );
            this.label13.TabIndex = 0;
            this.label13.Text = "Time:";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point( 210, 141 );
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size( 33, 13 );
            this.label12.TabIndex = 0;
            this.label12.Text = "Time:";
            // 
            // lblAdmitDate
            // 
            this.lblAdmitDate.Location = new System.Drawing.Point( 98, 141 );
            this.lblAdmitDate.Name = "lblAdmitDate";
            this.lblAdmitDate.Size = new System.Drawing.Size( 100, 13 );
            this.lblAdmitDate.TabIndex = 0;
            // 
            // lblPatientType
            // 
            this.lblPatientType.Location = new System.Drawing.Point( 98, 117 );
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size( 192, 13 );
            this.lblPatientType.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point( 11, 193 );
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size( 115, 13 );
            this.label9.TabIndex = 0;
            this.label9.Text = "Discharge disposition:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point( 11, 141 );
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size( 61, 13 );
            this.label8.TabIndex = 0;
            this.label8.Text = "Admit date:";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point( 11, 166 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 83, 13 );
            this.label7.TabIndex = 0;
            this.label7.Text = "Discharge date:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point( 11, 216 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 166, 13 );
            this.label6.TabIndex = 0;
            this.label6.Text = "Location from which discharged:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point( 11, 117 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 67, 13 );
            this.label5.TabIndex = 0;
            this.label5.Text = "Patient type:";
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point( 98, 95 );
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size( 184, 13 );
            this.lblAccount.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point( 11, 95 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 49, 13 );
            this.label3.TabIndex = 0;
            this.label3.Text = "Account:";
            // 
            // lblPatientName
            // 
            this.lblPatientName.Location = new System.Drawing.Point( 98, 74 );
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size( 317, 13 );
            this.lblPatientName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 11, 74 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 74, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Patient name:";
            // 
            // lblInstructionalMessage
            // 
            this.lblInstructionalMessage.Font = new System.Drawing.Font( "Microsoft Sans Serif", 12F, ( (System.Drawing.FontStyle)( ( System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic ) ) ), System.Drawing.GraphicsUnit.Pixel, ( (byte)( 0 ) ) );
            this.lblInstructionalMessage.ForeColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 204 ) ) ) ), ( (int)( ( (byte)( 0 ) ) ) ), ( (int)( ( (byte)( 0 ) ) ) ) );
            this.lblInstructionalMessage.Location = new System.Drawing.Point( 12, 8 );
            this.lblInstructionalMessage.Name = "lblInstructionalMessage";
            this.lblInstructionalMessage.Size = new System.Drawing.Size( 912, 56 );
            this.lblInstructionalMessage.TabIndex = 0;
            // 
            // panelMessages
            // 
            this.panelMessages.Controls.Add( this.lblNextAction );
            this.panelMessages.Controls.Add( this.lblNextActionLine );
            this.panelMessages.Controls.Add( this.panelActions );
            this.panelMessages.Controls.Add( this.lblOutstandingActionItemsMsg );
            this.panelMessages.Controls.Add( this.lblMessages );
            this.panelMessages.Controls.Add( this.label2 );
            this.panelMessages.Controls.Add( this.label55 );
            this.panelMessages.Location = new System.Drawing.Point( 6, 356 );
            this.panelMessages.Name = "panelMessages";
            this.panelMessages.Size = new System.Drawing.Size( 977, 156 );
            this.panelMessages.TabIndex = 1;
            // 
            // panelActions
            // 
            this.panelActions.Controls.Add( this.btnPrintFaceSheet );
            this.panelActions.Controls.Add( this.btnCloseActivity );
            this.panelActions.Controls.Add( this.btnRepeatActivity );
            this.panelActions.Controls.Add( this.btnEditAccount );
            this.panelActions.Location = new System.Drawing.Point( 10, 119 );
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size( 471, 29 );
            this.panelActions.TabIndex = 0;
            this.panelActions.Visible = false;
            // 
            // btnPrintFaceSheet
            // 
            this.btnPrintFaceSheet.BackColor = System.Drawing.SystemColors.Control;
            this.btnPrintFaceSheet.Location = new System.Drawing.Point( 341, 3 );
            this.btnPrintFaceSheet.Message = null;
            this.btnPrintFaceSheet.Name = "btnPrintFaceSheet";
            this.btnPrintFaceSheet.Size = new System.Drawing.Size( 122, 23 );
            this.btnPrintFaceSheet.TabIndex = 8;
            this.btnPrintFaceSheet.Text = "Pr&int Face Sheet";
            this.btnPrintFaceSheet.UseVisualStyleBackColor = false;
            this.btnPrintFaceSheet.Click += new System.EventHandler( this.btnPrintFaceSheet_Click );
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.BackColor = System.Drawing.SystemColors.Control;
            this.btnCloseActivity.Location = new System.Drawing.Point( 7, 3 );
            this.btnCloseActivity.Message = null;
            this.btnCloseActivity.Name = "btnCloseActivity";
            this.btnCloseActivity.Size = new System.Drawing.Size( 88, 23 );
            this.btnCloseActivity.TabIndex = 5;
            this.btnCloseActivity.Text = "C&lose Activity";
            this.btnCloseActivity.UseVisualStyleBackColor = false;
            this.btnCloseActivity.Click += new System.EventHandler( this.btnCloseActivity_Click );
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.BackColor = System.Drawing.SystemColors.Control;
            this.btnRepeatActivity.Location = new System.Drawing.Point( 102, 3 );
            this.btnRepeatActivity.Message = null;
            this.btnRepeatActivity.Name = "btnRepeatActivity";
            this.btnRepeatActivity.Size = new System.Drawing.Size( 88, 23 );
            this.btnRepeatActivity.TabIndex = 6;
            this.btnRepeatActivity.Text = "Repeat Acti&vity";
            this.btnRepeatActivity.UseVisualStyleBackColor = false;
            this.btnRepeatActivity.Click += new System.EventHandler( this.btnRepeatActivity_Click );
            // 
            // btnEditAccount
            // 
            this.btnEditAccount.BackColor = System.Drawing.SystemColors.Control;
            this.btnEditAccount.Location = new System.Drawing.Point( 197, 3 );
            this.btnEditAccount.Message = null;
            this.btnEditAccount.Name = "btnEditAccount";
            this.btnEditAccount.Size = new System.Drawing.Size( 137, 23 );
            this.btnEditAccount.TabIndex = 7;
            this.btnEditAccount.Text = "Edit/Maintain &Account...";
            this.btnEditAccount.UseVisualStyleBackColor = false;
            this.btnEditAccount.Click += new System.EventHandler( this.btnEditAccount_Click );
            // 
            // lblOutstandingActionItemsMsg
            // 
            this.lblOutstandingActionItemsMsg.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblOutstandingActionItemsMsg.Location = new System.Drawing.Point( 12, 56 );
            this.lblOutstandingActionItemsMsg.Name = "lblOutstandingActionItemsMsg";
            this.lblOutstandingActionItemsMsg.Size = new System.Drawing.Size( 912, 46 );
            this.lblOutstandingActionItemsMsg.TabIndex = 0;
            // 
            // lblMessages
            // 
            this.lblMessages.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblMessages.Location = new System.Drawing.Point( 12, 33 );
            this.lblMessages.Name = "lblMessages";
            this.lblMessages.Size = new System.Drawing.Size( 907, 16 );
            this.lblMessages.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point( 63, 9 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 905, 16 );
            this.label2.TabIndex = 0;
            this.label2.Text = "_________________________________________________________________________________" +
                "________________________________________________________________________________" +
                "__";
            // 
            // label55
            // 
            this.label55.Location = new System.Drawing.Point( 6, 10 );
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size( 56, 13 );
            this.label55.TabIndex = 0;
            this.label55.Text = "Messages";
            // 
            // lblDischargeDispositionVal
            // 
            this.lblDischargeDispositionVal.Location = new System.Drawing.Point( 128, 193 );
            this.lblDischargeDispositionVal.Name = "lblDischargeDispositionVal";
            this.lblDischargeDispositionVal.Size = new System.Drawing.Size( 213, 18 );
            this.lblDischargeDispositionVal.TabIndex = 5;
            this.lblDischargeDispositionVal.Visible = false;
            // 
            // lblDischargeTimeVal
            // 
            this.lblDischargeTimeVal.Location = new System.Drawing.Point( 243, 166 );
            this.lblDischargeTimeVal.Name = "lblDischargeTimeVal";
            this.lblDischargeTimeVal.Size = new System.Drawing.Size( 43, 14 );
            this.lblDischargeTimeVal.TabIndex = 7;
            this.lblDischargeTimeVal.Text = "label";
            this.lblDischargeTimeVal.Visible = false;
            // 
            // lblDischargeDateVal
            // 
            this.lblDischargeDateVal.Location = new System.Drawing.Point( 99, 166 );
            this.lblDischargeDateVal.Name = "lblDischargeDateVal";
            this.lblDischargeDateVal.Size = new System.Drawing.Size( 88, 16 );
            this.lblDischargeDateVal.TabIndex = 6;
            this.lblDischargeDateVal.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point( 90, 3 );
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 23 );
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.Control;
            this.btnOk.Location = new System.Drawing.Point( 5, 3 );
            this.btnOk.Message = null;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size( 75, 23 );
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler( this.btnOk_Click );
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add( this.btnOk );
            this.panelButtons.Controls.Add( this.btnCancel );
            this.panelButtons.Location = new System.Drawing.Point( 844, 586 );
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size( 169, 28 );
            this.panelButtons.TabIndex = 1;
            // 
            // ProgressPanel1
            // 
            this.ProgressPanel1.BackColor = System.Drawing.Color.White;
            this.ProgressPanel1.Location = new System.Drawing.Point( 8, 65 );
            this.ProgressPanel1.Name = "ProgressPanel1";
            this.ProgressPanel1.Size = new System.Drawing.Size( 1004, 523 );
            this.ProgressPanel1.TabIndex = 0;
            // 
            // lblNextAction
            // 
            this.lblNextAction.Location = new System.Drawing.Point( 6, 92 );
            this.lblNextAction.Name = "lblNextAction";
            this.lblNextAction.Size = new System.Drawing.Size( 62, 13 );
            this.lblNextAction.TabIndex = 4;
            this.lblNextAction.Text = "Next Action";
            this.lblNextAction.Visible = false;
            // 
            // lblNextActionLine
            // 
            this.lblNextActionLine.Location = new System.Drawing.Point( 61, 93 );
            this.lblNextActionLine.Name = "lblNextActionLine";
            this.lblNextActionLine.Size = new System.Drawing.Size( 909, 16 );
            this.lblNextActionLine.TabIndex = 3;
            this.lblNextActionLine.Text = "_________________________________________________________________________________" +
                "_______________________________________________________________________________";
            this.lblNextActionLine.Visible = false;
            // 
            // DischargeBaseView
            // 
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.Controls.Add( this.panelButtons );
            this.Controls.Add( this.panel2 );
            this.Controls.Add( this.panelPatientContext );
            this.Controls.Add( this.userContextView1 );
            this.Controls.Add( this.ProgressPanel1 );
            this.Name = "DischargeBaseView";
            this.Size = new System.Drawing.Size( 1019, 619 );
            this.panelPatientContext.ResumeLayout( false );
            this.panel2.ResumeLayout( false );
            this.panel2.PerformLayout();
            this.gbScanDocuments.ResumeLayout( false );
            this.panelMessages.ResumeLayout( false );
            this.panelActions.ResumeLayout( false );
            this.panelButtons.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DischargeBaseView()
        {
            if (!DesignMode)
            {
                // This call is required by the Windows.Forms Form Designer.
                InitializeComponent();
                btnCancel.Message = "cancelled discharge activity";
                btnOk.Message = "completed discharge activity";
            }
        }

        protected override void Dispose( bool disposing )
        {
            if (disposing)
            {
                // Set model to null and Disable all icons/menu options for FUS notes and Previously scanned documents
                ViewFactory.Instance.CreateView<PatientAccessView>().Model = null;
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
                ViewFactory.Instance.CreateView<PatientAccessView>().DisablePreviousDocumentOptions();

                // if cancel button activated then cancel the worker...
                if (backgroundWorker != null)
                    backgroundWorker.CancelAsync();

                if (components != null)
                {
                    components.Dispose();
                }
            }

            //This is a temporary fix until the cause for tabcontrol 
            //ObjectDiposedException issue is figured out.
            try
            {
                base.Dispose( disposing );
            }
            catch (ObjectDisposedException objDispEx)
            {
                c_log.Error( String.Format( "Message - {0}, StackTrace - {1}", objDispEx.Message,
                    objDispEx.StackTrace ) );
            }
        }
        #endregion

        #region Data Elements

        private Container components = null;

        private BackgroundWorker backgroundWorker;
        public UserContextView userContextView1;
        public PatientContextView patientContextView1;
        public ProgressPanel ProgressPanel1;

        public MaskedEditTextBox mtbDischargeDate;
        public MaskedEditTextBox mtbDischargeTime;

        public ClickOnceLoggingButton btnCancel;
        public ClickOnceLoggingButton btnOk;
        public LoggingButton btnCloseActivity;
        public LoggingButton btnRepeatActivity;
        public LoggingButton btnEditAccount;

        public ComboBox cmbDischargeDisposition;

        public DateTimePicker dtpDischargeDate;

        public Panel panelActions;
        public Panel panel2;
        public Panel panelPatientContext;
        public Panel panelButtons;
        public Panel panelMessages;

        public Label lblInstructionalMessage;
        public Label label1;
        public Label label2;
        public Label label3;
        public Label label5;
        public Label label55;
        public Label label6;
        public Label label7;
        public Label label8;
        public Label label9;
        public Label label12;
        public Label label13;
        public Label lblDischargeLocation;
        public Label lblPatientName;
        public Label lblAccount;
        public Label lblPatientType;
        public Label lblAdmitDate;
        public Label lblAdmitTime;
        public Label lblOutstandingActionItemsMsg;
        public Label lblMessages;
        public Label lblCurOccupant;
        public Label lblCurrentOccupant;
        public Label lblDischargeDispositionVal;
        public Label lblDischargeDateVal;
        public LoggingButton btnPrintFaceSheet;
        public GroupBox gbScanDocuments;
        private LoggingButton btnScanDocuments;
        private Label lblScanDocuments;
        public Label lblDischargeTimeVal;
        public Label lblNextAction;
        public Label lblNextActionLine;

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( DischargeBaseView ) );

        #endregion

        #region Constants
        private const int AGE_SIXTY_FIVE = 65;
        #endregion
    }
}
