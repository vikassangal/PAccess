using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DocumentImagingViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Reports.FaceSheet;

namespace PatientAccess.UI.Registration
{
    /// <summary>
    /// Summary description for RegisterConfirmationView.
    /// </summary>
    [Serializable]
    public class RegisterConfirmationView : ControlView
    {
        #region Events
        public event EventHandler RepeatActivity;
        public event EventHandler EditAccount;
        public event EventHandler CloseActivity;
        #endregion

        #region Event Handlers

        /// <summary>
        /// RegisterConfirmationView_Load - set the focus to the 'Close Activity' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterConfirmationView_Load(object sender, EventArgs e)
        {
            // clear any FUS notes that were manually entered so that - if the View FUS notes icon/menu option is selected,
            // the newly added notes do not show twice.

            if (ViewFactory.Instance.CreateView<PatientAccessView>().Model != null)
            {
                (ViewFactory.Instance.CreateView<PatientAccessView>().Model as Account).ClearFusNotes();
            }

            btnCloseActivity.Focus();
        }

        /// <summary>
        /// btnScanDocuments_Click - handle the click event for the 'Scan Documents...' button.
        /// Note, this button (indeed, the whole panel) is conditionally displayed based on the
        /// account status and\or activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScanDocuments_Click(object sender, EventArgs e)
        {
            OpenScanDocumentsForm();

            //Enable btnScanDocuments to be able to scan the next available document, if any
            btnScanDocuments.Enabled = true;

            // update prev document icons/menu options
            ViewFactory.Instance.CreateView<PatientAccessView>().SetPreviousDocumentOptions(Model);
        }

        private void btnCloseActivity_Click(object sender, EventArgs e)
        {
            if (CloseActivity != null)
            {
                CloseActivity(this, new LooseArgs(Model));
                AccountView.CloseVIweb();
            }
        }

        private void btnRepeatActivity_Click(object sender, EventArgs e)
        {
            if (RepeatActivity != null)
            {
                RepeatActivity(this, new LooseArgs(new Patient()));
                AccountView.CloseVIweb();
            }
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            AccountView.CloseVIweb();
            if (AccountLockStatus.IsAccountLocked(Model, User.GetCurrent()))
            {
                MessageBox.Show(UIErrorMessages.NO_EDIT_RECORD_LOCKED, "Warning", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                btnEditAccount.Enabled = true;
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
            else if ( Model.IsQuickRegistered )
            {
                AccountView.IsQuickRegistered = true;
                Model.Activity = new QuickAccountMaintenanceActivity();
            }
            else
            {
                Model.Activity = new MaintenanceActivity();
                if ( Model.IsNewBorn && Model.KindOfVisit != null && Model.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                    Model.Activity.AssociatedActivityType = typeof (PreAdmitNewbornActivity);
            }
            
            if ( !Model.Activity.ReadOnlyAccount() )
            {
                if (!AccountActivityService.PlaceLockOn(Model, String.Empty))
                {
                    Cursor = Cursors.Default;
                    btnEditAccount.Enabled = true;
                    return;
                }
            }

            // if we make it this far, the account is available to edit (i.e. not locked) and not New any more.
            Model.IsNew = false;
            EditAccount(this, new LooseArgs(Model));

            Cursor = Cursors.Default;
        }

        private void btnComplianceChecker_Click(object sender, EventArgs e)
        {
            try
            {
                AccountView.CloseVIweb();
                Cursor = Cursors.WaitCursor;
                SubmitComplianceCheck();
                Cursor = Cursors.Default;
            }
            catch (Exception)
            {
                MessageBox.Show(UIErrorMessages.DATAVALIDATION_SERVICE_EXCEPTION, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                btnComplianceChecker.Enabled = true;
            }
        }

        private void btnPrintFaceSheet_Click(object sender, EventArgs e)
        {
            try
            {
                AccountView.CloseVIweb();
                var faceSheetPrintService = new PrintService( Model );
                faceSheetPrintService.Print();
            }
            catch (Exception)
            {
                MessageBox.Show(UIErrorMessages.FACE_SHEET_PRINT_ERROR, "Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error,
                   MessageBoxDefaultButton.Button1);
                btnPrintFaceSheet.Enabled = true;
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if (Model != null)
            {
                setInstructionalText();

                lblPatientNameVal.Text = Model.Patient.Name.AsFormattedName();
                lblAccountVal.Text = Model.AccountNumber.ToString();
                lblMrnVal.Text = Model.Patient.MedicalRecordNumber.ToString();

                if (Model.KindOfVisit != null && Model.Activity != null)
                {
                    isComplianceCheckerValid = IsComplianceCheckerValid();
                    btnComplianceChecker.Visible = isComplianceCheckerValid;
                }

                if (Model.Activity != null &&
                    (Model.IsNew ||
                     Model.Activity.GetType().Equals(typeof (ActivatePreRegistrationActivity)) ||
                     Model.Activity.GetType().Equals(typeof (CancelInpatientStatusActivity)) ||
                     Model.Activity.GetType().Equals(typeof (PostMSERegistrationActivity)) ||
                     Model.Activity.GetType().Equals(typeof (PreMSERegisterActivity)) ||
                     Model.Activity.GetType().Equals(typeof (UCCPostMseRegistrationActivity)) ||
                     Model.Activity.GetType().Equals(typeof (RegistrationActivity)) ||
                     Model.Activity.GetType().Equals(typeof (MaintenanceActivity)) ||
                     Model.Activity.GetType().Equals(typeof (EditAccountActivity)) ||
                     Model.Activity.GetType().Equals(typeof (PreRegistrationActivity)) ||
                     Model.Activity.GetType().Equals(typeof (ShortRegistrationActivity)) ||
                     Model.Activity.GetType().Equals(typeof (ShortMaintenanceActivity)) ||
                     Model.Activity.GetType().Equals(typeof (QuickAccountCreationActivity)) ||
                     Model.Activity.GetType().Equals(typeof (QuickAccountMaintenanceActivity)) ||
                     Model.Activity.GetType().Equals(typeof (ShortPreRegistrationActivity)) ||
                     Model.Activity.GetType().Equals(typeof (AdmitNewbornActivity))  ||
                     Model.Activity.IsCreateUCCPreMSEActivity() ||
                     Model.Activity.IsEditUCCPreMSEActivity() ))
                {
                    pnlScan.Location = new Point(0, 109);

                    if (isComplianceCheckerValid)
                    {
                        pnlCompliance.Visible = true;
                    }
                    else
                    {
                        pnlButtons.Location = new Point(0, 220);
                    }

                    pnlScan.Visible = true;
                }
                else
                {
                    pnlScan.Visible = false;

                    if (isComplianceCheckerValid)
                    {
                        pnlCompliance.Visible = true;
                        pnlCompliance.Location = new Point(0, 109);
                        pnlButtons.Location = new Point(0, 172);
                    }
                    else
                    {
                        pnlButtons.Location = new Point(0, 109);
                    }
                }

                if ( Model.OnlinePreRegistered )
                {
                    var backgroundWorker = new BackgroundWorker();

                    backgroundWorker.DoWork += DeleteOnlineRegistrationSubmission;

                    backgroundWorker.RunWorkerAsync();

                }

                if (Model.Facility.IsSATXEnabled)
                {
                    // Show Interfacility Transfer Details
                    InterFacilityTransferFeatureManager interFacilityTransferFeatureManager = new InterFacilityTransferFeatureManager();
                    bool IsITFREnabled = interFacilityTransferFeatureManager.IsITFREnabled(User.GetCurrent().Facility, Model);
                    Model.Patient.InterFacilityTransferAccount.Activity = Model.Activity;

                    if (IsITFREnabled && Model.Patient.InterFacilityTransferAccount.FromAccountNumber != 0 && (Model.KindOfVisit.IsPreRegistrationPatient
                        || Model.Activity.AssociatedActivityType == typeof(ActivatePreRegistrationActivity)))
                    {
                        Patient FromHospitalPatient = PatientFrom(Model.Patient,Model.Activity);
                        
                        interfacilityPannel.SetValueForConfirmationScreen(Model.Patient.InterFacilityTransferAccount);
                        interfacilityPannel.SetAccountLabel = "From Account:";
                        interfacilityPannel.SetHospitalLabel = "From Hospital:";
                        //interfacilityPannel.SetERPhysician = ((PatientAccess.BrokerInterfaces.AccountProxy)FromHospitalPatient.Accounts[0]).ReferringPhysician.DisplayString;
                        interfacilityPannel.SetHSV = ((PatientAccess.BrokerInterfaces.AccountProxy)FromHospitalPatient.Accounts[0]).HospitalService.DisplayString;
                        interfacilityPannel.SetPT = ((PatientAccess.BrokerInterfaces.AccountProxy)FromHospitalPatient.Accounts[0]).KindOfVisit.DisplayString;
                        interfacilityPannel.BringToFront();
                    }
                    else
                    {
                        interfacilityPannel.VisibilityPanel(false);
                    }
                }
                else
                {
                    interfacilityPannel.VisibilityPanel(false);
                }
            }

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions(false);
        }

        public Patient PatientFrom(Patient patient,Activity anActivity)
        {
            Patient newPatient =
                new Patient(PersistentModel.NEW_OID,
                    PersistentModel.NEW_VERSION,
                    patient.Name,
                    patient.MedicalRecordNumber,
                    patient.DateOfBirth,
                    new SocialSecurityNumber(patient.SocialSecurityNumber.ToString()),
                    patient.Sex,
                    patient.InterFacilityTransferAccount.FromFacility);
            newPatient.InterFacilityTransferAccount = patient.InterFacilityTransferAccount;
            IAccountBroker acctBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            ArrayList accounts = acctBroker.AccountsFor(newPatient);
            //foreach (var acct in accounts)
            //{
            //   Account PhyAcc= acctBroker.AccountFor(newPatient, patient.InterFacilityTransferAccount.FromAccountNumber, anActivity);
            //    PhysicianRelationship referringPhysicianRelationship = new PhysicianRelationship();
            //    referringPhysicianRelationship.Physician = PhyAcc.ReferringPhysician;
               newPatient.AddAccounts(accounts);
            //    ((PatientAccess.BrokerInterfaces.AccountProxy)newPatient.Accounts[0]).AddPhysicianRelationship(referringPhysicianRelationship);
            //}
            return newPatient;
        }

        private void DeleteOnlineRegistrationSubmission(object sender, DoWorkEventArgs e)
        {
            var onlinePreRegistrationActivity = ( OnlinePreRegistrationActivity )Model.Activity.AssociatedActivity;
            if ( onlinePreRegistrationActivity != null && onlinePreRegistrationActivity.HasPreRegistrationData() )
            {
                var submissionToDelete = onlinePreRegistrationActivity.PreRegistrationData.SubmissionId;

                var preRegistrationSubmissionsBroker = BrokerFactory.BrokerOfType<IPreRegistrationSubmissionsBroker>();
                preRegistrationSubmissionsBroker.DeleteSubmission( submissionToDelete );
            }
        }

        /// <summary>
        /// UpdateModel method
        /// </summary>
        public override void UpdateModel()
        {

        }
        #endregion

        #region Properties
        public new Account Model
        {
            private get
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

        private void SubmitComplianceCheck()
        {
            try
            {
                var dataValidationBroker = BrokerFactory.BrokerOfType<IDataValidationBroker>();

                Model.Activity.AppUser = User.GetCurrent();
                var accountDetailsRequest = new AccountDetailsRequest();
                accountDetailsRequest = CreateComplianceCheckRequestFrom(accountDetailsRequest);
                dataValidationBroker.SendAccountForComplianceCheck(accountDetailsRequest);
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            btnComplianceChecker.Enabled = false;

            MessageBox.Show(UIErrorMessages.COMPLIANCE_CHECKER_MSG, "Compliance Checker",
                MessageBoxButtons.OK, MessageBoxIcon.None,
                MessageBoxDefaultButton.Button1);

            btnCloseActivity.Focus();
        }

        private AccountDetailsRequest CreateComplianceCheckRequestFrom(AccountDetailsRequest request)
        {
            var account = Model;

            if (account != null)
            {
                request.AccountNumber = account.AccountNumber;
                request.FacilityOid = account.Facility.Oid;
                request.Upn = account.Activity.AppUser.SecurityUser.UPN;
                if (account.ReferringPhysician != null)
                {
                    request.ReferringPhysicianNumber =
                        account.ReferringPhysician.PhysicianNumber.ToString().PadLeft(5, '0');
                }

                request.MedicalRecordNumber = account.Patient.MedicalRecordNumber;
                request.PatientDOB = account.Patient.DateOfBirth;
                request.PatientFirstName = account.Patient.FirstName;
                request.PatientLastName = account.Patient.LastName;
                request.PatientMidInitial = account.Patient.MiddleInitial;
                request.PatientSex = account.Patient.Sex.Code;
                request.PatientSSN = account.Patient.SocialSecurityNumber;
                request.Coverages = account.Insurance.Coverages;
                request.PatientMailingCP =
                    account.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
            }
            else
            {
                request.IsAccountNull = true;
            }

            return request;
        }

        private bool IsComplianceCheckerValid()
        {
            return ((Model.Activity.GetType().Equals(typeof(PreRegistrationActivity)) ||
                  Model.Activity.GetType().Equals(typeof(RegistrationActivity)) ||
                  Model.Activity.GetType().Equals(typeof(MaintenanceActivity)) ||
                    Model.Activity.GetType().Equals( typeof( QuickAccountCreationActivity ) ) ||
                  Model.Activity.GetType().Equals( typeof( QuickAccountMaintenanceActivity ) ) ||
                  Model.Activity.GetType().Equals( typeof( ShortPreRegistrationActivity ) ) ||
                  Model.Activity.GetType().Equals( typeof( ShortRegistrationActivity ) ) ||
                  Model.Activity.GetType().Equals( typeof( ShortMaintenanceActivity ) ) ||
                  Model.Activity.GetType().Equals( typeof( PreAdmitNewbornActivity ) )) &&
                IsOutPatient() &&
                IsPayorMedicare());
        }

        private bool IsPayorMedicare()
        {
            var payorIsMedicare = false;
            var coverages = Model.Insurance.Coverages;

            foreach (Coverage coverage in coverages)
            {
                if (coverage.InsurancePlan.GetType() == typeof(GovernmentMedicareInsurancePlan))
                {
                    payorIsMedicare = true;
                    break;
                }
            }

            return payorIsMedicare;
        }

        private bool IsOutPatient()
        {
            return (Model.KindOfVisit.Code == VisitType.OUTPATIENT ||
                Model.KindOfVisit.Code == VisitType.PREREG_PATIENT ||
                Model.KindOfVisit.Code == VisitType.RECURRING_PATIENT);
        }

        private void setInstructionalText()
        {
            if (Model == null)
            {
                lblInstructionalMessage.DisplayInfoMessage(string.Empty);
                return;
            }

            if (Model.Activity.GetType() == typeof(PreRegistrationActivity))
            {
                lblInstructionalMessage.DisplayErrorMessage(PRE_REG_CONFIRMED);
            }
            else if (Model.Activity.GetType() == typeof(RegistrationActivity))
            {
                lblInstructionalMessage.DisplayErrorMessage(REG_CONFIRMED);
            }
            else if ( Model.Activity.GetType() == typeof( ShortPreRegistrationActivity ) )
            {
                lblInstructionalMessage.DisplayErrorMessage( PRE_REG_CONFIRMED );
            }
            else if ( Model.Activity.GetType() == typeof( ShortRegistrationActivity ) )
            {
                lblInstructionalMessage.DisplayErrorMessage( REG_CONFIRMED );
            }
            else if ( Model.Activity.GetType() == typeof( ShortMaintenanceActivity ) )
            {
                lblInstructionalMessage.DisplayErrorMessage( MAINTENANCE_CONFIRMED );
            }
            else if ( Model.Activity.GetType() == typeof( AdmitNewbornActivity ) )
            {
                lblInstructionalMessage.DisplayErrorMessage(NEWBORN_CONFIRMED);
            }
            else if ( Model.Activity.GetType() == typeof( PreAdmitNewbornActivity ) )
            {
                lblInstructionalMessage.DisplayErrorMessage( PRENEWBORN_CONFIRMED );
            }
            else if (Model.Activity.GetType() == typeof(PreMSERegisterActivity))
            {
                lblInstructionalMessage.DisplayErrorMessage(PRE_MSE_CONFIRMED);
            }
          
            else if (Model.Activity.GetType() == typeof(EditPreMseActivity))
            {
                lblInstructionalMessage.DisplayErrorMessage(EDIT_PRE_MSE_CONFIRMED);
            }
            else if (Model.Activity.GetType() == typeof(PostMSERegistrationActivity))
            {
                lblInstructionalMessage.DisplayErrorMessage(POST_MSE_CONFIRMED);
            }
            else if (Model.Activity.GetType() == typeof(MaintenanceActivity))
            {
                lblInstructionalMessage.DisplayErrorMessage(MAINTENANCE_CONFIRMED);
            }
            else if ( Model.Activity.GetType() == typeof( QuickAccountCreationActivity ) )
            {
                lblInstructionalMessage.DisplayErrorMessage( PRE_REG_CONFIRMED );
            }
            else if (Model.Activity.IsCreateUCCPreMSEActivity())
            {
                lblInstructionalMessage.DisplayErrorMessage(UCC_PRE_MSE_CONFIRMED);
            }
            else if (Model.Activity.IsEditUCCPreMSEActivity())
            {
                lblInstructionalMessage.DisplayErrorMessage(EDIT_UCC_PRE_MSE_CONFIRMED);
            }
            else if (Model.Activity.GetType() == typeof(UCCPostMseRegistrationActivity))
            {
                lblInstructionalMessage.DisplayErrorMessage(UCC_POST_MSE_CONFIRMED);
            }
            else if ( Model.Activity.GetType() == typeof( QuickAccountMaintenanceActivity ) )
            {
                lblInstructionalMessage.DisplayErrorMessage( PRE_REG_CONFIRMED );
            }
            else
            {
                lblInstructionalMessage.DisplayErrorMessage(string.Empty);
            }
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
            this.interfacilityPannel = new PatientAccess.UI.InterfacilityTransfer.InterfacilityPannel();
            this.lblInstructionalMessage = new PatientAccess.UI.CommonControls.InfoControl();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblMrn = new System.Windows.Forms.Label();
            this.lblPatientNameVal = new System.Windows.Forms.Label();
            this.lblAccountVal = new System.Windows.Forms.Label();
            this.lblMrnVal = new System.Windows.Forms.Label();
            this.lblNextAction = new System.Windows.Forms.Label();
            this.lblLine = new System.Windows.Forms.Label();
            this.btnCloseActivity = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnRepeatActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnEditAccount = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnPrintFaceSheet = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnComplianceChecker = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.pnlScan = new System.Windows.Forms.Panel();
            this.gbScanDocuments = new System.Windows.Forms.GroupBox();
            this.btnScanDocuments = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.lblScanDocuments = new System.Windows.Forms.Label();
            this.pnlCompliance = new System.Windows.Forms.Panel();
            this.lblComplianceCheckerMsg = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMsgs = new System.Windows.Forms.Label();
            this.pnlButtons.SuspendLayout();
            this.pnlScan.SuspendLayout();
            this.gbScanDocuments.SuspendLayout();
            this.pnlCompliance.SuspendLayout();
            this.SuspendLayout();
            // 
            // interfacilityPannel
            // 
            this.interfacilityPannel.Location = new System.Drawing.Point(523, -1);
            this.interfacilityPannel.Name = "interfacilityPannel";
            this.interfacilityPannel.SetAccountLabel = "To Account:";
            this.interfacilityPannel.Size = new System.Drawing.Size(534, 237);
            this.interfacilityPannel.TabIndex = 2;
            this.interfacilityPannel.TranferToAccount = null;
            // 
            // lblInstructionalMessage
            // 
            this.lblInstructionalMessage.Location = new System.Drawing.Point(8, 6);
            this.lblInstructionalMessage.Message = "";
            this.lblInstructionalMessage.Name = "lblInstructionalMessage";
            this.lblInstructionalMessage.Size = new System.Drawing.Size(895, 26);
            this.lblInstructionalMessage.TabIndex = 0;
            this.lblInstructionalMessage.TabStop = false;
            // 
            // lblPatientName
            // 
            this.lblPatientName.Location = new System.Drawing.Point(10, 38);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(74, 13);
            this.lblPatientName.TabIndex = 0;
            this.lblPatientName.Text = "Patient name:";
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point(10, 62);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(51, 13);
            this.lblAccount.TabIndex = 0;
            this.lblAccount.Text = "Account:";
            // 
            // lblMrn
            // 
            this.lblMrn.Location = new System.Drawing.Point(10, 86);
            this.lblMrn.Name = "lblMrn";
            this.lblMrn.Size = new System.Drawing.Size(33, 13);
            this.lblMrn.TabIndex = 0;
            this.lblMrn.Text = "MRN:";
            // 
            // lblPatientNameVal
            // 
            this.lblPatientNameVal.Location = new System.Drawing.Point(86, 38);
            this.lblPatientNameVal.Name = "lblPatientNameVal";
            this.lblPatientNameVal.Size = new System.Drawing.Size(271, 13);
            this.lblPatientNameVal.TabIndex = 0;
            // 
            // lblAccountVal
            // 
            this.lblAccountVal.Location = new System.Drawing.Point(86, 61);
            this.lblAccountVal.Name = "lblAccountVal";
            this.lblAccountVal.Size = new System.Drawing.Size(271, 13);
            this.lblAccountVal.TabIndex = 0;
            // 
            // lblMrnVal
            // 
            this.lblMrnVal.Location = new System.Drawing.Point(86, 86);
            this.lblMrnVal.Name = "lblMrnVal";
            this.lblMrnVal.Size = new System.Drawing.Size(271, 13);
            this.lblMrnVal.TabIndex = 0;
            // 
            // lblNextAction
            // 
            this.lblNextAction.Location = new System.Drawing.Point(10, 21);
            this.lblNextAction.Name = "lblNextAction";
            this.lblNextAction.Size = new System.Drawing.Size(62, 13);
            this.lblNextAction.TabIndex = 0;
            this.lblNextAction.Text = "Next Action";
            // 
            // lblLine
            // 
            this.lblLine.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblLine.Location = new System.Drawing.Point(67, 21);
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(901, 13);
            this.lblLine.TabIndex = 0;
            this.lblLine.Text = "_________________________________________________________________________________" +
    "________________________________________________________________________________" +
    "__";
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.BackColor = System.Drawing.SystemColors.Control;
            this.btnCloseActivity.Location = new System.Drawing.Point(11, 56);
            this.btnCloseActivity.Message = null;
            this.btnCloseActivity.Name = "btnCloseActivity";
            this.btnCloseActivity.Size = new System.Drawing.Size(89, 23);
            this.btnCloseActivity.TabIndex = 1;
            this.btnCloseActivity.Text = "C&lose Activity";
            this.btnCloseActivity.UseVisualStyleBackColor = false;
            this.btnCloseActivity.Click += new System.EventHandler(this.btnCloseActivity_Click);
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.BackColor = System.Drawing.SystemColors.Control;
            this.btnRepeatActivity.Location = new System.Drawing.Point(107, 56);
            this.btnRepeatActivity.Message = null;
            this.btnRepeatActivity.Name = "btnRepeatActivity";
            this.btnRepeatActivity.Size = new System.Drawing.Size(89, 23);
            this.btnRepeatActivity.TabIndex = 2;
            this.btnRepeatActivity.Text = "Repeat Acti&vity";
            this.btnRepeatActivity.UseVisualStyleBackColor = false;
            this.btnRepeatActivity.Click += new System.EventHandler(this.btnRepeatActivity_Click);
            // 
            // btnEditAccount
            // 
            this.btnEditAccount.BackColor = System.Drawing.SystemColors.Control;
            this.btnEditAccount.Location = new System.Drawing.Point(203, 56);
            this.btnEditAccount.Message = null;
            this.btnEditAccount.Name = "btnEditAccount";
            this.btnEditAccount.Size = new System.Drawing.Size(132, 23);
            this.btnEditAccount.TabIndex = 3;
            this.btnEditAccount.Text = "Edit/Maintain &Account...";
            this.btnEditAccount.UseVisualStyleBackColor = false;
            this.btnEditAccount.Click += new System.EventHandler(this.btnEditAccount_Click);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnPrintFaceSheet);
            this.pnlButtons.Controls.Add(this.btnComplianceChecker);
            this.pnlButtons.Controls.Add(this.btnCloseActivity);
            this.pnlButtons.Controls.Add(this.btnRepeatActivity);
            this.pnlButtons.Controls.Add(this.btnEditAccount);
            this.pnlButtons.Controls.Add(this.lblNextAction);
            this.pnlButtons.Controls.Add(this.lblLine);
            this.pnlButtons.Location = new System.Drawing.Point(0, 283);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(968, 91);
            this.pnlButtons.TabIndex = 0;
            // 
            // btnPrintFaceSheet
            // 
            this.btnPrintFaceSheet.BackColor = System.Drawing.SystemColors.Control;
            this.btnPrintFaceSheet.Location = new System.Drawing.Point(341, 56);
            this.btnPrintFaceSheet.Message = null;
            this.btnPrintFaceSheet.Name = "btnPrintFaceSheet";
            this.btnPrintFaceSheet.Size = new System.Drawing.Size(123, 23);
            this.btnPrintFaceSheet.TabIndex = 4;
            this.btnPrintFaceSheet.Text = "Pr&int Face Sheet";
            this.btnPrintFaceSheet.UseVisualStyleBackColor = false;
            this.btnPrintFaceSheet.Click += new System.EventHandler(this.btnPrintFaceSheet_Click);
            // 
            // btnComplianceChecker
            // 
            this.btnComplianceChecker.BackColor = System.Drawing.SystemColors.Control;
            this.btnComplianceChecker.Location = new System.Drawing.Point(472, 56);
            this.btnComplianceChecker.Message = null;
            this.btnComplianceChecker.Name = "btnComplianceChecker";
            this.btnComplianceChecker.Size = new System.Drawing.Size(123, 23);
            this.btnComplianceChecker.TabIndex = 5;
            this.btnComplianceChecker.Text = "Compliance Chec&ker";
            this.btnComplianceChecker.UseVisualStyleBackColor = false;
            this.btnComplianceChecker.Visible = false;
            this.btnComplianceChecker.Click += new System.EventHandler(this.btnComplianceChecker_Click);
            // 
            // pnlScan
            // 
            this.pnlScan.Controls.Add(this.gbScanDocuments);
            this.pnlScan.Location = new System.Drawing.Point(0, 109);
            this.pnlScan.Name = "pnlScan";
            this.pnlScan.Size = new System.Drawing.Size(968, 113);
            this.pnlScan.TabIndex = 1;
            // 
            // gbScanDocuments
            // 
            this.gbScanDocuments.Controls.Add(this.btnScanDocuments);
            this.gbScanDocuments.Controls.Add(this.lblScanDocuments);
            this.gbScanDocuments.Location = new System.Drawing.Point(10, 10);
            this.gbScanDocuments.Name = "gbScanDocuments";
            this.gbScanDocuments.Size = new System.Drawing.Size(322, 100);
            this.gbScanDocuments.TabIndex = 0;
            this.gbScanDocuments.TabStop = false;
            this.gbScanDocuments.Text = "Scan documents";
            // 
            // btnScanDocuments
            // 
            this.btnScanDocuments.Location = new System.Drawing.Point(8, 59);
            this.btnScanDocuments.Message = null;
            this.btnScanDocuments.Name = "btnScanDocuments";
            this.btnScanDocuments.Size = new System.Drawing.Size(105, 23);
            this.btnScanDocuments.TabIndex = 1;
            this.btnScanDocuments.Text = "Scan Document...";
            this.btnScanDocuments.Click += new System.EventHandler(this.btnScanDocuments_Click);
            // 
            // lblScanDocuments
            // 
            this.lblScanDocuments.Location = new System.Drawing.Point(8, 24);
            this.lblScanDocuments.Name = "lblScanDocuments";
            this.lblScanDocuments.Size = new System.Drawing.Size(290, 29);
            this.lblScanDocuments.TabIndex = 0;
            this.lblScanDocuments.Text = "Scan available documents now for this account. To scan documents later, use the E" +
    "dit/Maintain Account activity.";
            // 
            // pnlCompliance
            // 
            this.pnlCompliance.Controls.Add(this.lblComplianceCheckerMsg);
            this.pnlCompliance.Controls.Add(this.label1);
            this.pnlCompliance.Controls.Add(this.lblMsgs);
            this.pnlCompliance.Location = new System.Drawing.Point(0, 220);
            this.pnlCompliance.Name = "pnlCompliance";
            this.pnlCompliance.Size = new System.Drawing.Size(968, 62);
            this.pnlCompliance.TabIndex = 0;
            this.pnlCompliance.Visible = false;
            // 
            // lblComplianceCheckerMsg
            // 
            this.lblComplianceCheckerMsg.Location = new System.Drawing.Point(10, 35);
            this.lblComplianceCheckerMsg.Name = "lblComplianceCheckerMsg";
            this.lblComplianceCheckerMsg.Size = new System.Drawing.Size(953, 13);
            this.lblComplianceCheckerMsg.TabIndex = 0;
            this.lblComplianceCheckerMsg.Text = "You have selected Medicare as a primary or secondary payor.  To initiate verifica" +
    "tion of covered procedures, click Compliance Checker.";
            // 
            // label1
            // 
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(68, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(901, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "_________________________________________________________________________________" +
    "________________________________________________________________________________" +
    "__";
            // 
            // lblMsgs
            // 
            this.lblMsgs.Location = new System.Drawing.Point(10, 10);
            this.lblMsgs.Name = "lblMsgs";
            this.lblMsgs.Size = new System.Drawing.Size(56, 13);
            this.lblMsgs.TabIndex = 0;
            this.lblMsgs.Text = "Messages";
            // 
            // RegisterConfirmationView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.interfacilityPannel);
            this.Controls.Add(this.pnlCompliance);
            this.Controls.Add(this.pnlScan);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.lblMrnVal);
            this.Controls.Add(this.lblAccountVal);
            this.Controls.Add(this.lblPatientNameVal);
            this.Controls.Add(this.lblMrn);
            this.Controls.Add(this.lblAccount);
            this.Controls.Add(this.lblPatientName);
            this.Controls.Add(this.lblInstructionalMessage);
            this.Name = "RegisterConfirmationView";
            this.Size = new System.Drawing.Size(968, 402);
            this.Load += new System.EventHandler(this.RegisterConfirmationView_Load);
            this.pnlButtons.ResumeLayout(false);
            this.pnlScan.ResumeLayout(false);
            this.gbScanDocuments.ResumeLayout(false);
            this.pnlCompliance.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RegisterConfirmationView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            EnableThemesOn(this);
        }

        /// <summary> 
        /// Clean up any resources being used.
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
        #endregion

        #region Data Elements

        private Container components = null;

        private InfoControl lblInstructionalMessage;

        private Panel pnlButtons;
        private Panel pnlScan;

        private GroupBox gbScanDocuments;

        private ClickOnceLoggingButton btnScanDocuments;
        private ClickOnceLoggingButton btnEditAccount;
        private ClickOnceLoggingButton btnComplianceChecker;
        private ClickOnceLoggingButton btnPrintFaceSheet;
        private ClickOnceLoggingButton btnCloseActivity;

        private LoggingButton btnRepeatActivity;

        private Label lblMsgs;
        private Label label1;
        private Label lblComplianceCheckerMsg;
        private Label lblScanDocuments;
        private Label lblPatientName;
        private Label lblAccount;
        private Label lblMrn;
        private Label lblPatientNameVal;
        private Label lblAccountVal;
        private Label lblMrnVal;
        private Label lblNextAction;
        private Label lblLine;

        private Panel pnlCompliance;

        private bool isComplianceCheckerValid;

        #endregion

        #region Constants

        private const string PRE_REG_CONFIRMED = "Preregister Patient submitted for processing.";
        private const string REG_CONFIRMED = "Register Patient submitted for processing.";
        private const string NEWBORN_CONFIRMED = "Register Newborn submitted for processing.";
        private const string PRENEWBORN_CONFIRMED = "Pre-Admit Newborn submitted for processing.";
        private const string PRE_MSE_CONFIRMED = "Register ED Patient Pre-MSE submitted for processing.";
        private const string EDIT_PRE_MSE_CONFIRMED = "Edit Pre-MSE Demographic Information submitted for processing.";
        private const string POST_MSE_CONFIRMED = "Register ED Patient Post-MSE submitted for processing.";
        private const string MAINTENANCE_CONFIRMED = "Edit/Maintain Account submitted for processing.";
        private const string UCC_PRE_MSE_CONFIRMED = "Urgent Care Pre-MSE submitted for processing.";
        private const string UCC_POST_MSE_CONFIRMED = "Urgent Care Post-MSE submitted for processing.";
        private InterfacilityTransfer.InterfacilityPannel interfacilityPannel;
        private const string EDIT_UCC_PRE_MSE_CONFIRMED = "Edit Urgent Care Pre-MSE  Information submitted for processing.";
        #endregion
    }
}
