using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Builder;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews;
using PatientAccess.UI.PhysicianSearchViews;

namespace PatientAccess.UI.TransferViews
{
    public class TransferOutPatToInPatStep3View : ControlView
    {
        #region Events

        public event EventHandler CancelButtonClicked;
        public event EventHandler BackButtonClicked;
        public event EventHandler FinishButtonClicked;
        public event EventHandler TabSelectedEvent;

        #endregion

        #region Event Handlers

        void RequiredFieldSummaryTabSelectedEvent( object sender, EventArgs e )
        {
            // throw this event up to it's parent

            int index = ( int )( ( LooseArgs )e ).Context;

            if ( index < 3 || ( index >= 7 && index <= 11 ) )
            {
                if ( TabSelectedEvent != null )
                {
                    TabSelectedEvent( sender, e );
                }
            }
            else if ( index >= 3 && index <= 6 )
            {
                InsuranceDetails insuranceDetails = new InsuranceDetails
                    {
                        Model = insuranceView1.Model_Account,
                        insuranceDetailsView = {Account = insuranceView1.Model_Account}
                    };

                switch ( index )
                {
                    case 3: // Primary Insured

                        insuranceDetails.insuranceDetailsView.Active_Tab = InsuranceDetailsView.INSURED_DETAILS_PAGE;
                        insuranceDetails.insuranceDetailsView.Model_Coverage = GetPrimaryCoverage();

                        insuranceDetails.UpdateView();
                        insuranceDetails.ShowDialog( this );

                        break;

                    case 4: // Secondary Insured

                        insuranceDetails.insuranceDetailsView.Active_Tab = InsuranceDetailsView.INSURED_DETAILS_PAGE;
                        insuranceDetails.insuranceDetailsView.Model_Coverage = GetSecondaryCoverage();

                        insuranceDetails.UpdateView();
                        insuranceDetails.ShowDialog( this );

                        break;

                    case 5: // Primary Payor

                        insuranceDetails.insuranceDetailsView.Active_Tab = InsuranceDetailsView.PLAN_DETAILS_PAGE;
                        insuranceDetails.insuranceDetailsView.Model_Coverage = GetPrimaryCoverage();

                        insuranceDetails.UpdateView();
                        insuranceDetails.ShowDialog( this );

                        break;

                    case 6: // Secondary Payor

                        insuranceDetails.insuranceDetailsView.Active_Tab = InsuranceDetailsView.PLAN_DETAILS_PAGE;
                        insuranceDetails.insuranceDetailsView.Model_Coverage = GetSecondaryCoverage();

                        insuranceDetails.UpdateView();
                        insuranceDetails.ShowDialog( this );

                        break;
                }
            }
        }

        /// <summary>
        /// Returns the primary coverage on the account.
        /// </summary>
        private Coverage GetPrimaryCoverage()
        {
            Coverage coverageReturned = null;
            Insurance insurance = Model.Insurance;

            if ( insurance == null )
            {   // TODO: Add error log message here
                return coverageReturned;
            }
            // Iterate over the coverage collection and populate
            // the primary and secondary insurance screens.

            ICollection coverageCollection = insurance.Coverages;

            if ( coverageCollection == null )
            {
                return coverageReturned;
            }

            foreach ( Coverage coverage in coverageCollection )
            {
                if ( coverage == null )
                {
                    continue;
                }

                if ( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                {
                    coverageReturned = coverage;
                }
            }
            return coverageReturned;
        }

        /// <summary>
        /// Returns the secondary coverage on the account.
        /// </summary>
        private Coverage GetSecondaryCoverage()
        {
            Coverage coverageReturned = null;
            Insurance insurance = Model.Insurance;

            if ( insurance == null )
            {   // TODO: Add error log message here
                return coverageReturned;
            }
            // Iterate over the coverage collection and populate
            // the primary and secondary insurance screens.

            ICollection coverageCollection = insurance.Coverages;

            if ( coverageCollection == null )
            {
                return coverageReturned;
            }

            foreach ( Coverage coverage in coverageCollection )
            {
                if ( coverage == null )
                {
                    continue;
                }

                if ( coverage.CoverageOrder.Oid.Equals( CoverageOrder.SECONDARY_OID ) )
                {
                    coverageReturned = coverage;
                }
            }
            return coverageReturned;
        }

        private void TransferOutPatToInPatStep3View_Load( object sender, EventArgs e )
        {
            HidePanel();

            insuranceView1.Model_Account = Model;
        }

        private void NoPrimaryMedicarePayorForAutoAccidentEventHandler( object sender, EventArgs e )
        {
            MessageDisplayHandler = new ErrorMessageDisplayHandler( Model );
            DialogResult warningResult =
                MessageDisplayHandler.DisplayYesNoErrorMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );

            if ( warningResult == DialogResult.No )
            {
                return;
            }

            doNotProceedWithFinish = true;
        }

        private void EvaluateMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage()
        {
            bool ruleWasViolated = !RuleEngine.OneShotRuleEvaluation<MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage>(
                Model, MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler );

            doNotProceedWithFinish = ruleWasViolated;

        }

        private void MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler( object sender, EventArgs e )
        {
            MessageDisplayHandler = new ErrorMessageDisplayHandler( Model );
            MessageDisplayHandler.DisplayOkWarningMessageFor( typeof( MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage ) );
        }

        private void btnFinish_Click( object sender, EventArgs e )
        {
            doNotProceedWithFinish = false;

            //TODO-AC we need to improve this design.
            EvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish( NoPrimaryMedicarePayorForAutoAccidentEventHandler );

            if ( doNotProceedWithFinish )
            {
                EnableFinishButton();
                return;
            }

            EvaluateMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage();

            if ( doNotProceedWithFinish )
            {
                EnableFinishButton();
                return;
            }
            //TODO-AC end todo

            if ( !RuleEngine.EvaluateAllRules( Model ) )
            {
                // TLG 01/24/07 Modified for SR 41126 - link to tabs from Summary

                ICollection<CompositeAction> collection = RuleEngine.GetCompositeItemsCollection();

                if ( collection.Count > 0
                    && Rules.RuleEngine.AccountHasRequiredFields( collection ) )
                {
                    requiredFieldSummaryView = new RequiredFieldsSummaryView();
                    requiredFieldsSummaryPresenter = new RequiredFieldsSummaryPresenter( requiredFieldSummaryView, collection )
                                                     {
                                                         Model = Model,
                                                         Header = RequiredFieldsSummaryPresenter.REQUIRED_FIELDS_HEADER
                                                     };

                    requiredFieldsSummaryPresenter.TabSelectedEvent += RequiredFieldSummaryTabSelectedEvent;

                    try
                    {
                        requiredFieldsSummaryPresenter.ShowViewAsDialog( this );
                    }

                    finally
                    {
                        btnFinish.Enabled = true;
                        requiredFieldSummaryView.Dispose();
                    }

                    return;
                }
            }

            //Raise FinishButtonClicked event.
            FinishButtonClicked( this, new LooseArgs( Model ) );
        }

        private void btnBack_Click( object sender, EventArgs e )
        {
            BackButtonClicked( this, new LooseArgs( Model ) );
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            if ( AccountActivityService.ConfirmCancelActivity( sender, new LooseArgs( Model ) ) )
            {
                CancelButtonClicked( this, new EventArgs() );
            }
            else
            {
                btnCancel.Enabled = true;
            }
        }

        private void ErrorMessageShownForRuleEventHandler( object sender, EventArgs e )
        {
            LooseArgs args = e as LooseArgs;
            if ( args != null && args.Context != null )
            {
                string rule = args.Context.ToString();

                if ( rule == null )
                {
                    return;
                }

                MessageStateManager.SetErrorMessageDisplayedFor( rule, true );
            }
        }

        public bool PhysiciansValidated()
        {
            return PhysicianService.VerifyPhysicians( Model,
                GetReferringPhysicianId(), GetAdmittingPhysicianId(),
                GetAttendingPhysicianId(), GetOperatingPhysicianId(),
                GetPrimaryCarePhysicianId() );
        }

        private string GetAdmittingPhysicianId()
        {
            string result = string.Empty;
            if ( Model != null && Model.AdmittingPhysician != null )
            {
                result = Model.AdmittingPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetAttendingPhysicianId()
        {
            string result = string.Empty;
            if ( Model != null && Model.AttendingPhysician != null )
            {
                result = Model.AttendingPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetOperatingPhysicianId()
        {
            string result = string.Empty;
            if ( Model != null && Model.OperatingPhysician != null )
            {
                result = Model.OperatingPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetPrimaryCarePhysicianId()
        {
            string result = string.Empty;
            if ( Model != null && Model.PrimaryCarePhysician != null )
            {
                result = Model.PrimaryCarePhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetReferringPhysicianId()
        {
            string result = string.Empty;

            if ( Model != null && Model.ReferringPhysician != null )
            {
                result = Model.ReferringPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        #endregion

        #region Methods

        public void ShowPanel()
        {
            progressPanel1.Visible = true;
            progressPanel1.BringToFront();
        }

        public void HidePanel()
        {
            progressPanel1.Visible = false;
            progressPanel1.SendToBack();
        }

        public override void UpdateView()
        {
            if ( Model != null )
            {
                RuleEngine.LoadRules( Model );

                btnFinish.Enabled = true;

                if ( Model.Patient != null )
                {
                    patientContextView1.Model = Model.Patient;
                    patientContextView1.Account = Model;
                    patientContextView1.UpdateView();
                }

                insuranceView1.Model = Model;
                insuranceView1.UpdateView();

                insuranceView1.SetDefaultFocus();
            }
        }

        public override void UpdateModel()
        {
            insuranceView1.UpdateModel();
        }

        public static DialogResult ShowChiefComplaintWarning()
        {
            return MessageBox.Show( UIErrorMessages.TRANSFER_CHIEFCOMPLAINT_WARNING_MSG,
                                    "Unchanged Chief Complaint Warning",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1 );
        }

        public void EnableFinishButton()
        {
            btnFinish.Enabled = true;
        }

        internal void EvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish( EventHandler eventHandler )
        {
            bool noPrimaryMedicareForAutoAccidentRuleChecked = MessageStateManager.HasErrorMessageBeenDisplayedEarlierFor(
                typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() );
            if ( !noPrimaryMedicareForAutoAccidentRuleChecked )
            {
                RuleEngine.OneShotRuleEvaluation<NoMedicarePrimaryPayorForAutoAccident>( Model, eventHandler );
            }
        }
        #endregion

        #region Properties
        public new Account Model
        {
            private get
            {
                return ( Account )base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        private RequiredFieldsSummaryPresenter requiredFieldsSummaryPresenter { get; set; }

        private IErrorMessageDisplayHandler MessageDisplayHandler { get; set; }

        internal IMessageDisplayStateManager MessageStateManager { get; private set; }

        internal IRuleEngine RuleEngine { get; private set; }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelTransferOutPatToInPat = new System.Windows.Forms.Panel();
            this.insuranceView1 = new PatientAccess.UI.InsuranceViews.InsuranceView();
            this.lblStep3 = new System.Windows.Forms.Label();
            this.btnFinish = new ClickOnceLoggingButton();
            this.btnCancel = new ClickOnceLoggingButton();
            this.panelUserContext = new System.Windows.Forms.Panel();
            this.userContextView1 = new PatientAccess.UI.UserContextView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.btnBack = new LoggingButton();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.panelTransferOutPatToInPat.SuspendLayout();
            this.panelUserContext.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTransferOutPatToInPat
            // 
            this.panelTransferOutPatToInPat.BackColor = System.Drawing.Color.White;
            this.panelTransferOutPatToInPat.Controls.Add( this.insuranceView1 );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblStep3 );
            this.panelTransferOutPatToInPat.Controls.Add( this.progressPanel1 );
            this.panelTransferOutPatToInPat.Location = new System.Drawing.Point( 8, 64 );
            this.panelTransferOutPatToInPat.Name = "panelTransferOutPatToInPat";
            this.panelTransferOutPatToInPat.Size = new System.Drawing.Size( 1008, 520 );
            this.panelTransferOutPatToInPat.TabIndex = 0;
            // 
            // insuranceView1
            // 
            this.insuranceView1.BackColor = System.Drawing.Color.White;
            this.insuranceView1.Location = new System.Drawing.Point( 9, 38 );
            this.insuranceView1.Model = null;
            this.insuranceView1.Name = "insuranceView1";
            this.insuranceView1.Size = new System.Drawing.Size( 1024, 380 );
            this.insuranceView1.TabIndex = 0;
            // 
            // lblStep3
            // 
            this.lblStep3.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.lblStep3.Location = new System.Drawing.Point( 13, 8 );
            this.lblStep3.Name = "lblStep3";
            this.lblStep3.Size = new System.Drawing.Size( 163, 23 );
            this.lblStep3.TabIndex = 56;
            this.lblStep3.Text = "Step 3 of 3: Insurance";
            // 
            // btnFinish
            // 
            this.btnFinish.BackColor = System.Drawing.SystemColors.Control;
            this.btnFinish.Location = new System.Drawing.Point( 941, 594 );
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.TabIndex = 3;
            this.btnFinish.Text = "Fini&sh";
            this.btnFinish.Click += new System.EventHandler( this.btnFinish_Click );
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point( 775, 594 );
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // panelUserContext
            // 
            this.panelUserContext.BackColor = System.Drawing.Color.FromArgb( ( ( System.Byte )( 209 ) ), ( ( System.Byte )( 228 ) ), ( ( System.Byte )( 243 ) ) );
            this.panelUserContext.Controls.Add( this.userContextView1 );
            this.panelUserContext.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUserContext.Location = new System.Drawing.Point( 0, 0 );
            this.panelUserContext.Name = "panelUserContext";
            this.panelUserContext.Size = new System.Drawing.Size( 1024, 22 );
            this.panelUserContext.TabIndex = 0;
            // 
            // userContextView1
            // 
            this.userContextView1.BackColor = System.Drawing.SystemColors.Control;
            this.userContextView1.Description = "Transfer Outpatient to Inpatient";
            this.userContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.userContextView1.Model = null;
            this.userContextView1.Name = "userContextView1";
            this.userContextView1.Size = new System.Drawing.Size( 1024, 23 );
            this.userContextView1.TabIndex = 0;
            this.userContextView1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.patientContextView1 );
            this.panel1.Location = new System.Drawing.Point( 8, 32 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 1008, 24 );
            this.panel1.TabIndex = 5;
            // 
            // patientContextView1
            // 
            this.patientContextView1.Account = null;
            this.patientContextView1.BackColor = System.Drawing.Color.White;
            this.patientContextView1.DateOfBirthText = "";
            this.patientContextView1.GenderLabelText = "";
            this.patientContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.patientContextView1.Model = null;
            this.patientContextView1.Name = "patientContextView1";
            this.patientContextView1.PatientNameText = "";
            this.patientContextView1.Size = new System.Drawing.Size( 1008, 53 );
            this.patientContextView1.SocialSecurityNumber = "";
            this.patientContextView1.TabIndex = 0;
            this.patientContextView1.TabStop = false;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.SystemColors.Control;
            this.btnBack.Location = new System.Drawing.Point( 861, 594 );
            this.btnBack.Name = "btnBack";
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "< &Back";
            this.btnBack.Click += new System.EventHandler( this.btnBack_Click );
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point( 5, 5 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size( 998, 505 );
            this.progressPanel1.TabIndex = 57;
            // 
            // TransferOutPatToInPatStep3View
            // 
            this.Load += new EventHandler( TransferOutPatToInPatStep3View_Load );
            this.AcceptButton = this.btnFinish;
            this.BackColor = System.Drawing.Color.FromArgb( ( ( System.Byte )( 209 ) ), ( ( System.Byte )( 228 ) ), ( ( System.Byte )( 243 ) ) );
            this.Controls.Add( this.btnBack );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.btnCancel );
            this.Controls.Add( this.btnFinish );
            this.Controls.Add( this.panelTransferOutPatToInPat );
            this.Controls.Add( this.panelUserContext );
            this.Name = "TransferOutPatToInPatStep3View";
            this.Size = new System.Drawing.Size( 1024, 632 );
            this.panelTransferOutPatToInPat.ResumeLayout( false );
            this.panelUserContext.ResumeLayout( false );
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #region Construction and Finalization

        public TransferOutPatToInPatStep3View()
            : this( new MessageDisplayStateManager(), Rules.RuleEngine.GetInstance() )
        {
        }

        public TransferOutPatToInPatStep3View( IMessageDisplayStateManager messageStateManager, IRuleEngine ruleEngine )
        {
            InitializeComponent();

            EnableThemesOn( this );
            ActivityEventAggregator.GetInstance().ErrorMessageDisplayed +=
                ErrorMessageShownForRuleEventHandler;

            MessageStateManager = messageStateManager;
            RuleEngine = ruleEngine;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Data Elements

        private Panel panelTransferOutPatToInPat;
        private LoggingButton btnCancel;
        private Panel panelUserContext;
        private Panel panel1;
        private UserContextView userContextView1;
        private PatientContextView patientContextView1;
        private LoggingButton btnBack;
        private Label lblStep3;
        private ClickOnceLoggingButton btnFinish;
        private InsuranceView insuranceView1;
        private ProgressPanel progressPanel1;
        private IContainer components = null;
        private RequiredFieldsSummaryView requiredFieldSummaryView;
        private bool doNotProceedWithFinish;

        #endregion

        #region Constants

        #endregion

    }
}

