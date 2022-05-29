using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.PhysicianSearchViews;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for PhysicianSelectionView.
    /// </summary>
    public class PhysicianSelectionView : ControlView
    {
        #region Events
        #endregion

        #region Event Handlers

        private void PhysicianSelectionView_Leave( object sender, EventArgs e )
        {
            AcceptButton = null;
            Refresh();
        }

        private void btnFind_Click( object sender, EventArgs e )
        {
            ClearSpecifyPhysicianPanel();

            if ( !ShowPhysicianPrereqsMessage() )
            {
                return;
            }

            PhysicianSearchFormView physicianSearchForm = new PhysicianSearchFormView();
            Cursor = Cursors.WaitCursor;

            physicianSearchForm.Model = Model;
            physicianSearchForm.UpdateView();
            physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex = ( int )SelectedPhysicianTab.SEARCH_BY_NAME;

            try
            {
                if ( physicianSearchForm.ShowDialog( this ) == DialogResult.OK )
                {
                    Model = physicianSearchForm.Model;
                    UpdateViewDetail();
                }
            }
            finally
            {
                physicianSearchForm.Dispose();
                Cursor = Cursors.Default;
            }
        }

        private void btnRecNonStaff_Click( object sender, EventArgs e )
        {
            ClearSpecifyPhysicianPanel();

            if ( !ShowPhysicianPrereqsMessage() )
            {
                return;
            }

            PhysicianSearchFormView physicianSearchForm = new PhysicianSearchFormView();
            Cursor = Cursors.WaitCursor;

            physicianSearchForm.CallingObject = "NONSTAFF";
            physicianSearchForm.Model = Model;
            physicianSearchForm.UpdateView();
            physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex = ( int )SelectedPhysicianTab.REC_NONSTAFF_PHYSICIAN;

            try
            {
                if ( physicianSearchForm.ShowDialog( this ) == DialogResult.OK )
                {
                    Model = physicianSearchForm.Model;
                    UpdateViewDetail();
                }
            }
            finally
            {
                physicianSearchForm.Dispose();
                Cursor = Cursors.Default;
            }
        }

        private void btnRefViewDetails_Click( object sender, EventArgs e )
        {
            i_PhysicianRelationship = PhysicianRelationship.REFERRING_PHYSICIAN;
            ShowDetails( Model.ReferringPhysician.PhysicianNumber );
        }

        private void btnAdmViewDetails_Click( object sender, EventArgs e )
        {
            i_PhysicianRelationship = PhysicianRelationship.ADMITTING_PHYSICIAN;
            ShowDetails( Model.AdmittingPhysician.PhysicianNumber );
        }

        private void btnAttViewDetails_Click( object sender, EventArgs e )
        {
            i_PhysicianRelationship = PhysicianRelationship.ATTENDING_PHYSICIAN;
            ShowDetails( Model.AttendingPhysician.PhysicianNumber );
        }

        private void btnOprViewDetails_Click( object sender, EventArgs e )
        {
            i_PhysicianRelationship = PhysicianRelationship.OPERATING_PHYSICIAN;
            ShowDetails( Model.OperatingPhysician.PhysicianNumber );
        }

        private void btnPcpViewDetails_Click( object sender, EventArgs e )
        {
            i_PhysicianRelationship = PhysicianRelationship.PRIMARYCARE_PHYSICIAN;
            ShowDetails( Model.PrimaryCarePhysician.PhysicianNumber );
        }

        private void btnRefClear_Click( object sender, EventArgs e )
        {
            lblRefDisplayVal.Text = String.Empty;
            btnRefViewDetails.Enabled = false;
            btnRefClear.Enabled = false;

            PhysicianRelationship aRelationship = new PhysicianRelationship( PhysicianRole.Referring(), Model.ReferringPhysician );
            Model.RemovePhysicianRelationship( aRelationship );

            RunRules();
        }

        private void btnAdmClear_Click( object sender, EventArgs e )
        {
            lblAdmDisplayVal.Text = String.Empty;
            btnAdmViewDetails.Enabled = false;
            btnAdmClear.Enabled = false;

            PhysicianRelationship aRelationship = new PhysicianRelationship( PhysicianRole.Admitting(), Model.AdmittingPhysician );
            Model.RemovePhysicianRelationship( aRelationship );

            RunRules();
        }

        private void btnAttClear_Click( object sender, EventArgs e )
        {
            lblAttDisplayVal.Text = String.Empty;
            btnAttViewDetails.Enabled = false;
            btnAttClear.Enabled = false;

            PhysicianRelationship aRelationship = new PhysicianRelationship( PhysicianRole.Attending(), Model.AttendingPhysician );
            Model.RemovePhysicianRelationship( aRelationship );

            RunRules();
        }

        private void btnOprClear_Click( object sender, EventArgs e )
        {
            lblOprDisplayVal.Text = String.Empty;
            btnOprViewDetails.Enabled = false;
            btnOprClear.Enabled = false;

            PhysicianRelationship aRelationship = new PhysicianRelationship( PhysicianRole.Operating(), Model.OperatingPhysician );
            Model.RemovePhysicianRelationship( aRelationship );

            RunRules();
        }

        private void btnOthClear_Click( object sender, EventArgs e )
        {
            lblPcpDisplayVal.Text = String.Empty;
            btnOthViewDetails.Enabled = false;
            btnOthClear.Enabled = false;

            PhysicianRelationship aRelationship = new PhysicianRelationship( PhysicianRole.PrimaryCare(), Model.PrimaryCarePhysician );
            Model.RemovePhysicianRelationship( aRelationship );

            RunRules();
        }
        #endregion

        #region Rule Event Handlers
        private void ReferringPhysicianRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( lblRefDisplayVal );
        }

        private void ReferringPhysicianPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( lblRefDisplayVal );
        }

        private void AdmittingPhysicianRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( lblAdmDisplayVal );
        }

        private void AdmittingPhysicianPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( lblAdmDisplayVal );
        }

        private void AttendingPhysicianRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( lblAttDisplayVal );
        }

        private void AttendingPhysicianPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( lblAttDisplayVal );
        }

        #endregion

        #region Public Methods
    
        public override void UpdateView()
        {
            RegisterRulesEvents();
            UpdateViewDetail();
        }

        private void RunRules()
        {

            if (Model.IsEDorUrgentCarePremseAccount ||
                Model.Activity.GetType() == typeof (UCCPostMseRegistrationActivity)
                )
            {
                lblOpr.Enabled = false;
                lblPcp.Enabled = enablePrimaryCarePhysicianForPreMse;
                lblPcpDisplayVal.Enabled = enablePrimaryCarePhysicianForPreMse;
                mtbOpr.Enabled = false;
                mtbPCP.Enabled = enablePrimaryCarePhysicianForPreMse;
                lblOprInfo.Enabled = false;
                lblPcpInfo.Enabled = enablePrimaryCarePhysicianForPreMse;

                btnOprViewDetails.Enabled = false;
                btnOprClear.Enabled = false;

                btnOthViewDetails.Enabled = enablePrimaryCarePhysicianForPreMse;
                btnOthClear.Enabled = enablePrimaryCarePhysicianForPreMse;
            }

            // reset all fields that might have error, preferred, or required backgrounds
            UIColors.SetNormalBgColor( lblRefDisplayVal );
            if ( !PhysicianIsValid( Model.ReferringPhysician ) )
            {
                btnRefViewDetails.Enabled = false;
                btnRefClear.Enabled = false;
            }
            else
            {
                btnRefViewDetails.Enabled = true;
                btnRefClear.Enabled = true;
            }

            UIColors.SetNormalBgColor( lblAdmDisplayVal );
            if ( !PhysicianIsValid( Model.AdmittingPhysician ) )
            {
                btnAdmViewDetails.Enabled = false;
                btnAdmClear.Enabled = false;
            }
            else
            {
                btnAdmViewDetails.Enabled = true;
                btnAdmClear.Enabled = true;
            }

            UIColors.SetNormalBgColor( lblAttDisplayVal );
            if ( !PhysicianIsValid( Model.AttendingPhysician ) )
            {
                btnAttViewDetails.Enabled = false;
                btnAttClear.Enabled = false;
            }
            else
            {
                btnAttViewDetails.Enabled = true;
                btnAttClear.Enabled = true;
            }

            UIColors.SetNormalBgColor( lblOprDisplayVal );
            if ( !PhysicianIsValid( Model.OperatingPhysician ) )
            {
                btnOprViewDetails.Enabled = false;
                btnOprClear.Enabled = false;
            }
            else
            {
                btnOprViewDetails.Enabled = true;
                btnOprClear.Enabled = true;
            }

            UIColors.SetNormalBgColor( lblPcpDisplayVal );
            if ( !PhysicianIsValid( Model.PrimaryCarePhysician ) )
            {
                btnOthViewDetails.Enabled = false;
                btnOthClear.Enabled = false;
            }
            else
            {
                btnOthViewDetails.Enabled = true;
                btnOthClear.Enabled = true;
            }
            
            RegisterRulesEvents();
            
            RuleEngine.GetInstance().EvaluateRule( typeof( ReferringPhysicianRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( AdmittingPhysicianRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( AttendingPhysicianRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( ReferringPhysicianPreferred ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( AdmittingPhysicianPreferred ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( AttendingPhysicianPreferred ), Model );
            RuleEngine.GetInstance().EvaluateRule(typeof(PrimaryCarePhysicianRequired), Model);
            UnRegisterRulesEvents();
        }
       
        public void SetDefaultFocus()
        {
            mtbRef.Focus();
        }
        #endregion

        #region public Properties
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
        #endregion

        #region Private Methods

        private void DisplayNonstaffPhysicianDetails()
        {
            PhysicianSearchFormView physicianSearchForm = new PhysicianSearchFormView();
            Cursor = Cursors.WaitCursor;

            physicianSearchForm.Model = Model;
            physicianSearchForm.PhysicianRelationshipToView = i_PhysicianRelationship;
            physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex = ( int )SelectedPhysicianTab.REC_NONSTAFF_PHYSICIAN;
            physicianSearchForm.CallingObject = "VIEWDETAIL";
            physicianSearchForm.UpdateView();

            try
            {
                if ( physicianSearchForm.ShowDialog( this ) == DialogResult.OK )
                {
                    Model = physicianSearchForm.Model;
                    UpdateViewDetail();
                }
            }
            finally
            {
                physicianSearchForm.Dispose();
                Cursor = Cursors.Default;
            }
        }

        private void ShowDetails( long physicianNumber )
        {
            if ( physicianNumber == NONSTAFFPHYSICIAN_NBR )
            {
                DisplayNonstaffPhysicianDetails();
            }
            else
            {
                PhysicianDetailView physicianDetailView = new PhysicianDetailView();

                Cursor = Cursors.WaitCursor;
                physicianDetailView.SelectPhysicians = physicianNumber;
                try
                {
                    physicianDetailView.ShowDialog( this );
                }
                finally
                {
                    Cursor = Cursors.Default;
                    physicianDetailView.Dispose();
                }
            }
        }

        private void PhysicianNum_Enter( object sender, EventArgs e )
        {
            if ( AcceptButton != null )
            {
                i_btnOriginalAcceptButton = ( LoggingButton )AcceptButton;
            }
            AcceptButton = btnVerify;
        }

        private void PhysicianNum_Validating( object sender, CancelEventArgs e )
        {
            IsPhysicianNumberValid( ( TextBox )sender );

            if ( i_btnOriginalAcceptButton != null )
            {
                AcceptButton = i_btnOriginalAcceptButton;
            }
        }

        private bool IsPhysicianNumberValid( TextBox PhysicianNumTextBox )
        {
            if ( ( PhysicianNumTextBox.Text == "8888" || PhysicianNumTextBox.Text == "08888" ) )
            {
                invalidPhysicianEntry = true;

                UIColors.SetErrorBgColor( PhysicianNumTextBox );
                MessageBox.Show( UIErrorMessages.PHYSICIAN_NUMBER_8888_ERRMSG2, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return false;
            }
            invalidPhysicianEntry = false;

            UIColors.SetNormalBgColor( PhysicianNumTextBox );
            Refresh();
            return true;
        }

        private bool ShowPhysicianPrereqsMessage()
        {
            if ( !RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianSelectionPreRequisites ), Model ) )
            {
                if ( Model.Activity != null )
                {
                    // if PreMSE
                    if ( Model.Activity.GetType() == typeof( PreMSERegisterActivity )
                        || ( Model.Activity.GetType() == typeof( MaintenanceActivity )
                        && Model.Activity.AssociatedActivityType == typeof( PreMSERegisterActivity ) ) ||
                        Model.Activity.GetType() == typeof( UCCPreMSERegistrationActivity ) ||
                        Model.Activity.GetType() == typeof( EditUCCPreMSEActivity ) )
                    {
                        MessageBox.Show( UIErrorMessages.PHYSICIAN_SELECTION_PREREQS_PREMSE, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                    else if ( Model.Activity.GetType() == typeof( TransferOutToInActivity ) ||
                        Model.Activity.GetType() == typeof(TransferERToOutpatientActivity) ||
                        Model.Activity.GetType() == typeof(TransferOutpatientToERActivity))
                    {
                        MessageBox.Show( UIErrorMessages.PHYSICIAN_SELECTION_PREREQS_TRANSFER, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                    else
                    {
                        MessageBox.Show( UIErrorMessages.PHYSICIAN_SELECTION_PREREQS, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                    return false;
                }

                return true;
            }

            return true;
        }

        private void btnVerify_Click( object sender, EventArgs e )
        {
            if ( !ShowPhysicianPrereqsMessage() )
            {
                return;
            }

            if ( invalidPhysicianEntry )
            {
                invalidPhysicianEntry = false;
                return;
            }

            if ( !IsPhysicianNumberValid( mtbRef ) )
                return;

            if ( !IsPhysicianNumberValid( mtbAdm ) )
                return;

            if ( !IsPhysicianNumberValid( mtbAtt ) )
                return;
            if (!(Model.Activity.GetType().Equals(typeof(PreMSERegisterActivity)) ||
                Model.Activity.GetType().Equals(typeof(UCCPreMSERegistrationActivity))||
                Model.Activity.GetType().Equals(typeof(UCCPostMseRegistrationActivity)))
                )
            {
                if (!IsPhysicianNumberValid(mtbOpr))
                    return;
                 
                if (!IsPhysicianNumberValid(mtbPCP))
                    return;
            }
            if (Model.Activity.GetType().Equals(typeof(PreMSERegisterActivity)) && enablePrimaryCarePhysicianForPreMse )
            {
               if (!IsPhysicianNumberValid(mtbPCP))
                    return;
            }
            VerifyPhysicians();
        }

        private void VerifyPhysicians()
        {

            if ( mtbRef.UnMaskedText != string.Empty )
            {
                try
                {
                    PhysicianService.SearchAndAssignPhysician( Convert.ToInt64( mtbRef.UnMaskedText ),
                        PhysicianRole.Referring(),
                        Model );

                    UIColors.SetNormalBgColor( mtbRef );
                    mtbRef.BackColor = Color.White;
                    mtbRef.UnMaskedText = string.Empty;
                    UpdateViewDetail();
                }
                catch ( PhysicianNotFoundException )
                {
                    UIColors.SetErrorBgColor( mtbRef );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    UIColors.SetErrorBgColor( mtbRef );
                }
            }

            if ( mtbAdm.UnMaskedText != string.Empty )
            {
                try
                {
                    PhysicianService.SearchAndAssignPhysician( Convert.ToInt64( mtbAdm.UnMaskedText ),
                        PhysicianRole.Admitting(),
                        Model );

                    UIColors.SetNormalBgColor( mtbAdm );
                    mtbAdm.UnMaskedText = string.Empty;
                    UpdateViewDetail();
                }
                catch ( PhysicianNotFoundException )
                {
                    UIColors.SetErrorBgColor( mtbAdm );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    UIColors.SetErrorBgColor( mtbAdm );
                }
            }

            if ( mtbAtt.UnMaskedText != string.Empty )
            {
                try
                {
                    PhysicianService.SearchAndAssignPhysician( Convert.ToInt64( mtbAtt.UnMaskedText ),
                        PhysicianRole.Attending(),
                        Model );

                    UIColors.SetNormalBgColor( mtbAtt );
                    mtbAtt.UnMaskedText = string.Empty;
                    UpdateViewDetail();
                }
                catch ( PhysicianNotFoundException )
                {
                    UIColors.SetErrorBgColor( mtbAtt );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    UIColors.SetErrorBgColor( mtbAtt );
                }
            }

            if ( mtbOpr.UnMaskedText != string.Empty )
            {
                try
                {
                    PhysicianService.SearchAndAssignPhysician( Convert.ToInt64( mtbOpr.UnMaskedText ),
                        PhysicianRole.Operating(),
                        Model );

                    UIColors.SetNormalBgColor( mtbOpr );
                    mtbOpr.UnMaskedText = string.Empty;
                    UpdateViewDetail();
                }
                catch ( PhysicianNotFoundException )
                {
                    UIColors.SetErrorBgColor( mtbOpr );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    UIColors.SetErrorBgColor( mtbOpr );
                }
            }

            if ( mtbPCP.UnMaskedText != string.Empty )
            {
                try
                {
                    PhysicianService.SearchAndAssignPhysician( Convert.ToInt64( mtbPCP.UnMaskedText ),
                        PhysicianRole.PrimaryCare(),
                        Model );

                    UIColors.SetNormalBgColor( mtbPCP );
                    mtbPCP.UnMaskedText = string.Empty;
                    UpdateViewDetail();
                }
                catch ( PhysicianNotFoundException )
                {
                    UIColors.SetErrorBgColor( mtbPCP );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    UIColors.SetErrorBgColor( mtbPCP );
                }
            }

            Refresh();

            PhysicianService.VerifyPhysicians( Model, mtbRef.UnMaskedText, mtbAdm.UnMaskedText,
                mtbAtt.UnMaskedText, mtbOpr.UnMaskedText, mtbPCP.UnMaskedText );
        }

        private void RegisterRulesEvents()
        {
            //RuleEngine.LoadRules( Model );            
            RuleEngine.GetInstance().RegisterEvent( typeof( ReferringPhysicianRequired ), Model, new EventHandler( ReferringPhysicianRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmittingPhysicianRequired ), Model, new EventHandler( AdmittingPhysicianRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AttendingPhysicianRequired ), Model, new EventHandler( AttendingPhysicianRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( ReferringPhysicianPreferred ), Model, new EventHandler( ReferringPhysicianPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmittingPhysicianPreferred ), Model, new EventHandler( AdmittingPhysicianPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AttendingPhysicianPreferred ), Model, new EventHandler( AttendingPhysicianPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent(typeof(PrimaryCarePhysicianRequired), Model, new EventHandler(PrimaryCarePhysicianRequiredEvent));
        }

        private void UnRegisterRulesEvents()
        {
            // UNREGISTER EVENTS  
            RuleEngine.GetInstance().UnregisterEvent( typeof( ReferringPhysicianRequired ), Model, ReferringPhysicianRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmittingPhysicianRequired ), Model, AdmittingPhysicianRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AttendingPhysicianRequired ), Model, AttendingPhysicianRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ReferringPhysicianPreferred ), Model, ReferringPhysicianPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmittingPhysicianPreferred ), Model, AdmittingPhysicianPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AttendingPhysicianPreferred ), Model, AttendingPhysicianPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent(typeof(PrimaryCarePhysicianRequired), Model, PrimaryCarePhysicianRequiredEvent);
        }
        private void PrimaryCarePhysicianRequiredEvent(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(lblPcpDisplayVal);
        }

        private void setPrimaryCarePhysicianLabel()
        {
            this.lblPcpInfo.Text = PhysicianRole.PRIMARYCAREPHYSICIAN_LABEL;
            this.lblPcp.Text = PhysicianRole.PRIMARYCAREPHYSICIAN_LABEL;
        }
        private void setOtherPhysicianLabel()
        {
            this.lblPcpInfo.Text = PhysicianRole.OTHERPHYSICIAN_LABEL;
            this.lblPcp.Text = PhysicianRole.OTHERPHYSICIAN_LABEL ;
        }
        private void UpdateViewDetail()
        {
            var primaryCarePhysicianFeatureManager = new PrimaryCarePhysicianFeatureManager();
            if (primaryCarePhysicianFeatureManager.IsPrimaryCarePhysicianEnabledForDate(Model.AccountCreatedDate))
            {
                setPrimaryCarePhysicianLabel();
            }
            else
            {
                setOtherPhysicianLabel();
            }
            var primaryCarePhysicianForPremseFeatureManager = new PrimaryCarePhysicianForPreMseFeatureManager(ConfigurationManager.AppSettings);
            if (primaryCarePhysicianForPremseFeatureManager.IsEnabledFor(Model.AccountCreatedDate))
            {
                enablePrimaryCarePhysicianForPreMse = true;
            }
            else
            {
                enablePrimaryCarePhysicianForPreMse = false;
            }

            if ( PhysicianIsValid( Model.ReferringPhysician ) )
            {
                lblRefDisplayVal.Text = String.Format( PHYSICIAN_DISPAY_FORMAT, Model.ReferringPhysician.PhysicianNumber,
                    Model.ReferringPhysician.FormattedName );
            }

            if ( PhysicianIsValid( Model.AdmittingPhysician ) )
            {
                lblAdmDisplayVal.Text = String.Format( PHYSICIAN_DISPAY_FORMAT, Model.AdmittingPhysician.PhysicianNumber,
                    Model.AdmittingPhysician.FormattedName );
            }

            if ( PhysicianIsValid( Model.AttendingPhysician ) )
            {
                lblAttDisplayVal.Text = String.Format( PHYSICIAN_DISPAY_FORMAT, Model.AttendingPhysician.PhysicianNumber,
                    Model.AttendingPhysician.FormattedName );
            }

            if ( PhysicianIsValid( Model.OperatingPhysician ) )
            {
                lblOprDisplayVal.Text = String.Format( PHYSICIAN_DISPAY_FORMAT, Model.OperatingPhysician.PhysicianNumber,
                    Model.OperatingPhysician.FormattedName );
            }

            if ( PhysicianIsValid( Model.PrimaryCarePhysician ) )
            {
                lblPcpDisplayVal.Text = String.Format( PHYSICIAN_DISPAY_FORMAT, Model.PrimaryCarePhysician.PhysicianNumber,
                    Model.PrimaryCarePhysician.FormattedName );
            }

            RunRules();
        }
        private void ClearSpecifyPhysicianPanel()
        {
            if ( mtbRef.UnMaskedText != string.Empty )
            {
                UIColors.SetNormalBgColor( mtbRef );
                mtbRef.BackColor = Color.White;
                mtbRef.UnMaskedText = string.Empty;
            }
            if ( mtbAdm.UnMaskedText != string.Empty )
            {
                UIColors.SetNormalBgColor( mtbAdm );
                mtbAdm.UnMaskedText = string.Empty;
            }
            if ( mtbAtt.UnMaskedText != string.Empty )
            {
                UIColors.SetNormalBgColor( mtbAtt );
                mtbAtt.UnMaskedText = string.Empty;
            }
            if ( mtbOpr.UnMaskedText != string.Empty )
            {
                UIColors.SetNormalBgColor( mtbOpr );
                mtbOpr.UnMaskedText = string.Empty;
            }
            if ( mtbPCP.UnMaskedText != string.Empty )
            {
                UIColors.SetNormalBgColor( mtbPCP );
                mtbPCP.UnMaskedText = string.Empty;
            }
        }
        #endregion

        #region private Properties

        private bool PhysicianIsValid( Physician aPhysician )
        {
            return aPhysician != null && aPhysician.PhysicianNumber != 0;
        }

        #endregion

        #region Construction and Finalization
        public PhysicianSelectionView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            UnRegisterRulesEvents();

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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpPhysicians = new System.Windows.Forms.GroupBox();
            this.lblPcpInfo = new System.Windows.Forms.Label();
            this.lblPcpDisplayVal = new System.Windows.Forms.Label();
            this.lblOprInfo = new System.Windows.Forms.Label();
            this.lblOprDisplayVal = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblAttDisplayVal = new System.Windows.Forms.Label();
            this.lblRefDisplayVal = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblAdmDisplayVal = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOthClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnOthViewDetails = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnOprClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnOprViewDetails = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnAttClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnAttViewDetails = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnAdmClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnAdmViewDetails = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnRefClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnRefViewDetails = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnRecNonStaff = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblRecNonStaffPhysician = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblFindPhysician = new System.Windows.Forms.Label();
            this.btnFind = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelSpecifyPhysician = new System.Windows.Forms.Panel();
            this.lblPcp = new System.Windows.Forms.Label();
            this.mtbPCP = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblOpr = new System.Windows.Forms.Label();
            this.mtbOpr = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblAtt = new System.Windows.Forms.Label();
            this.mtbAtt = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblAdm = new System.Windows.Forms.Label();
            this.mtbAdm = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblRef = new System.Windows.Forms.Label();
            this.lblSpecifyByName = new System.Windows.Forms.Label();
            this.btnVerify = new PatientAccess.UI.CommonControls.LoggingButton();
            this.mtbRef = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lineLabel1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel3 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel4 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel5 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel7 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lblFind = new System.Windows.Forms.Label();
            this.grpPhysicians.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelSpecifyPhysician.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPhysicians
            // 
            this.grpPhysicians.Controls.Add(this.lblPcpInfo);
            this.grpPhysicians.Controls.Add(this.lblPcpDisplayVal);
            this.grpPhysicians.Controls.Add(this.lblOprInfo);
            this.grpPhysicians.Controls.Add(this.lblOprDisplayVal);
            this.grpPhysicians.Controls.Add(this.label11);
            this.grpPhysicians.Controls.Add(this.lblAttDisplayVal);
            this.grpPhysicians.Controls.Add(this.lblRefDisplayVal);
            this.grpPhysicians.Controls.Add(this.label8);
            this.grpPhysicians.Controls.Add(this.lblAdmDisplayVal);
            this.grpPhysicians.Controls.Add(this.label2);
            this.grpPhysicians.Controls.Add(this.btnOthClear);
            this.grpPhysicians.Controls.Add(this.btnOthViewDetails);
            this.grpPhysicians.Controls.Add(this.btnOprClear);
            this.grpPhysicians.Controls.Add(this.btnOprViewDetails);
            this.grpPhysicians.Controls.Add(this.btnAttClear);
            this.grpPhysicians.Controls.Add(this.btnAttViewDetails);
            this.grpPhysicians.Controls.Add(this.btnAdmClear);
            this.grpPhysicians.Controls.Add(this.btnAdmViewDetails);
            this.grpPhysicians.Controls.Add(this.btnRefClear);
            this.grpPhysicians.Controls.Add(this.btnRefViewDetails);
            this.grpPhysicians.Controls.Add(this.panel2);
            this.grpPhysicians.Controls.Add(this.panel1);
            this.grpPhysicians.Controls.Add(this.panelSpecifyPhysician);
            this.grpPhysicians.Controls.Add(this.lineLabel1);
            this.grpPhysicians.Controls.Add(this.lineLabel2);
            this.grpPhysicians.Controls.Add(this.lineLabel3);
            this.grpPhysicians.Controls.Add(this.lineLabel4);
            this.grpPhysicians.Controls.Add(this.lineLabel5);
            this.grpPhysicians.Controls.Add(this.lineLabel7);
            this.grpPhysicians.Location = new System.Drawing.Point(0, 0);
            this.grpPhysicians.Name = "grpPhysicians";
            this.grpPhysicians.Size = new System.Drawing.Size(546, 273);
            this.grpPhysicians.TabIndex = 0;
            this.grpPhysicians.TabStop = false;
            this.grpPhysicians.Text = "Physicians";
            // 
            // lblPcpInfo
            // 
            this.lblPcpInfo.Location = new System.Drawing.Point(15, 237);
            this.lblPcpInfo.Name = "lblPcpInfo";
            this.lblPcpInfo.Size = new System.Drawing.Size(32, 16);
            this.lblPcpInfo.TabIndex = 81;
            this.lblPcpInfo.Text = "PCP:";
            // 
            // lblPcpDisplayVal
            // 
            this.lblPcpDisplayVal.Location = new System.Drawing.Point(53, 237);
            this.lblPcpDisplayVal.Name = "lblPcpDisplayVal";
            this.lblPcpDisplayVal.Size = new System.Drawing.Size(288, 14);
            this.lblPcpDisplayVal.TabIndex = 83;
            // 
            // lblOprInfo
            // 
            this.lblOprInfo.Location = new System.Drawing.Point(15, 205);
            this.lblOprInfo.Name = "lblOprInfo";
            this.lblOprInfo.Size = new System.Drawing.Size(27, 18);
            this.lblOprInfo.TabIndex = 76;
            this.lblOprInfo.Text = "Opr:";
            // 
            // lblOprDisplayVal
            // 
            this.lblOprDisplayVal.Location = new System.Drawing.Point(53, 206);
            this.lblOprDisplayVal.Name = "lblOprDisplayVal";
            this.lblOprDisplayVal.Size = new System.Drawing.Size(288, 15);
            this.lblOprDisplayVal.TabIndex = 78;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(16, 176);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(23, 14);
            this.label11.TabIndex = 71;
            this.label11.Text = "Att:";
            // 
            // lblAttDisplayVal
            // 
            this.lblAttDisplayVal.Location = new System.Drawing.Point(53, 176);
            this.lblAttDisplayVal.Name = "lblAttDisplayVal";
            this.lblAttDisplayVal.Size = new System.Drawing.Size(288, 14);
            this.lblAttDisplayVal.TabIndex = 73;
            // 
            // lblRefDisplayVal
            // 
            this.lblRefDisplayVal.Location = new System.Drawing.Point(53, 116);
            this.lblRefDisplayVal.Name = "lblRefDisplayVal";
            this.lblRefDisplayVal.Size = new System.Drawing.Size(288, 17);
            this.lblRefDisplayVal.TabIndex = 63;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(16, 145);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 66;
            this.label8.Text = "Adm:";
            // 
            // lblAdmDisplayVal
            // 
            this.lblAdmDisplayVal.Location = new System.Drawing.Point(53, 145);
            this.lblAdmDisplayVal.Name = "lblAdmDisplayVal";
            this.lblAdmDisplayVal.Size = new System.Drawing.Size(288, 15);
            this.lblAdmDisplayVal.TabIndex = 68;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 12);
            this.label2.TabIndex = 61;
            this.label2.Text = "Ref:";
            // 
            // btnOthClear
            // 
            this.btnOthClear.Location = new System.Drawing.Point(456, 232);
            this.btnOthClear.Message = null;
            this.btnOthClear.Name = "btnOthClear";
            this.btnOthClear.Size = new System.Drawing.Size(71, 23);
            this.btnOthClear.TabIndex = 18;
            this.btnOthClear.Text = "Clear";
            this.btnOthClear.Click += new System.EventHandler(this.btnOthClear_Click);
            // 
            // btnOthViewDetails
            // 
            this.btnOthViewDetails.Location = new System.Drawing.Point(360, 232);
            this.btnOthViewDetails.Message = null;
            this.btnOthViewDetails.Name = "btnOthViewDetails";
            this.btnOthViewDetails.Size = new System.Drawing.Size(87, 23);
            this.btnOthViewDetails.TabIndex = 17;
            this.btnOthViewDetails.Text = "View Details";
            this.btnOthViewDetails.Click += new System.EventHandler(this.btnPcpViewDetails_Click);
            // 
            // btnOprClear
            // 
            this.btnOprClear.Location = new System.Drawing.Point(456, 202);
            this.btnOprClear.Message = null;
            this.btnOprClear.Name = "btnOprClear";
            this.btnOprClear.Size = new System.Drawing.Size(71, 23);
            this.btnOprClear.TabIndex = 16;
            this.btnOprClear.Text = "Clear";
            this.btnOprClear.Click += new System.EventHandler(this.btnOprClear_Click);
            // 
            // btnOprViewDetails
            // 
            this.btnOprViewDetails.Location = new System.Drawing.Point(360, 202);
            this.btnOprViewDetails.Message = null;
            this.btnOprViewDetails.Name = "btnOprViewDetails";
            this.btnOprViewDetails.Size = new System.Drawing.Size(87, 23);
            this.btnOprViewDetails.TabIndex = 15;
            this.btnOprViewDetails.Text = "View Details";
            this.btnOprViewDetails.Click += new System.EventHandler(this.btnOprViewDetails_Click);
            // 
            // btnAttClear
            // 
            this.btnAttClear.Location = new System.Drawing.Point(456, 172);
            this.btnAttClear.Message = null;
            this.btnAttClear.Name = "btnAttClear";
            this.btnAttClear.Size = new System.Drawing.Size(71, 23);
            this.btnAttClear.TabIndex = 14;
            this.btnAttClear.Text = "Clear";
            this.btnAttClear.Click += new System.EventHandler(this.btnAttClear_Click);
            // 
            // btnAttViewDetails
            // 
            this.btnAttViewDetails.Location = new System.Drawing.Point(360, 172);
            this.btnAttViewDetails.Message = null;
            this.btnAttViewDetails.Name = "btnAttViewDetails";
            this.btnAttViewDetails.Size = new System.Drawing.Size(87, 23);
            this.btnAttViewDetails.TabIndex = 13;
            this.btnAttViewDetails.Text = "View Details";
            this.btnAttViewDetails.Click += new System.EventHandler(this.btnAttViewDetails_Click);
            // 
            // btnAdmClear
            // 
            this.btnAdmClear.Location = new System.Drawing.Point(456, 142);
            this.btnAdmClear.Message = null;
            this.btnAdmClear.Name = "btnAdmClear";
            this.btnAdmClear.Size = new System.Drawing.Size(71, 23);
            this.btnAdmClear.TabIndex = 12;
            this.btnAdmClear.Text = "Clear";
            this.btnAdmClear.Click += new System.EventHandler(this.btnAdmClear_Click);
            // 
            // btnAdmViewDetails
            // 
            this.btnAdmViewDetails.Location = new System.Drawing.Point(360, 141);
            this.btnAdmViewDetails.Message = null;
            this.btnAdmViewDetails.Name = "btnAdmViewDetails";
            this.btnAdmViewDetails.Size = new System.Drawing.Size(87, 23);
            this.btnAdmViewDetails.TabIndex = 11;
            this.btnAdmViewDetails.Text = "View Details";
            this.btnAdmViewDetails.Click += new System.EventHandler(this.btnAdmViewDetails_Click);
            // 
            // btnRefClear
            // 
            this.btnRefClear.Location = new System.Drawing.Point(456, 111);
            this.btnRefClear.Message = null;
            this.btnRefClear.Name = "btnRefClear";
            this.btnRefClear.Size = new System.Drawing.Size(71, 23);
            this.btnRefClear.TabIndex = 10;
            this.btnRefClear.Text = "Clear";
            this.btnRefClear.Click += new System.EventHandler(this.btnRefClear_Click);
            // 
            // btnRefViewDetails
            // 
            this.btnRefViewDetails.Location = new System.Drawing.Point(360, 111);
            this.btnRefViewDetails.Message = null;
            this.btnRefViewDetails.Name = "btnRefViewDetails";
            this.btnRefViewDetails.Size = new System.Drawing.Size(87, 23);
            this.btnRefViewDetails.TabIndex = 9;
            this.btnRefViewDetails.Text = "View Details";
            this.btnRefViewDetails.Click += new System.EventHandler(this.btnRefViewDetails_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(142)))));
            this.panel2.Controls.Add(this.btnRecNonStaff);
            this.panel2.Controls.Add(this.lblRecNonStaffPhysician);
            this.panel2.Location = new System.Drawing.Point(420, 21);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(112, 80);
            this.panel2.TabIndex = 2;
            // 
            // btnRecNonStaff
            // 
            this.btnRecNonStaff.Location = new System.Drawing.Point(8, 43);
            this.btnRecNonStaff.Message = "Click record nonstaff";
            this.btnRecNonStaff.Name = "btnRecNonStaff";
            this.btnRecNonStaff.Size = new System.Drawing.Size(96, 23);
            this.btnRecNonStaff.TabIndex = 8;
            this.btnRecNonStaff.Text = "&Record Nonstaff";
            this.btnRecNonStaff.Click += new System.EventHandler(this.btnRecNonStaff_Click);
            // 
            // lblRecNonStaffPhysician
            // 
            this.lblRecNonStaffPhysician.Location = new System.Drawing.Point(12, 8);
            this.lblRecNonStaffPhysician.Name = "lblRecNonStaffPhysician";
            this.lblRecNonStaffPhysician.Size = new System.Drawing.Size(95, 26);
            this.lblRecNonStaffPhysician.TabIndex = 50;
            this.lblRecNonStaffPhysician.Text = "Record nonstaff physician";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(142)))));
            this.panel1.Controls.Add(this.lblFindPhysician);
            this.panel1.Controls.Add(this.btnFind);
            this.panel1.Location = new System.Drawing.Point(312, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(103, 80);
            this.panel1.TabIndex = 1;
            // 
            // lblFindPhysician
            // 
            this.lblFindPhysician.Location = new System.Drawing.Point(7, 8);
            this.lblFindPhysician.Name = "lblFindPhysician";
            this.lblFindPhysician.Size = new System.Drawing.Size(94, 31);
            this.lblFindPhysician.TabIndex = 54;
            this.lblFindPhysician.Text = "Find physician by name or specialty";
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(20, 44);
            this.btnFind.Message = "Click find physician";
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(66, 23);
            this.btnFind.TabIndex = 7;
            this.btnFind.Text = "F&ind";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // panelSpecifyPhysician
            // 
            this.panelSpecifyPhysician.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(142)))));
            this.panelSpecifyPhysician.Controls.Add(this.lblPcp);
            this.panelSpecifyPhysician.Controls.Add(this.mtbPCP);
            this.panelSpecifyPhysician.Controls.Add(this.lblOpr);
            this.panelSpecifyPhysician.Controls.Add(this.mtbOpr);
            this.panelSpecifyPhysician.Controls.Add(this.lblAtt);
            this.panelSpecifyPhysician.Controls.Add(this.mtbAtt);
            this.panelSpecifyPhysician.Controls.Add(this.lblAdm);
            this.panelSpecifyPhysician.Controls.Add(this.mtbAdm);
            this.panelSpecifyPhysician.Controls.Add(this.lblRef);
            this.panelSpecifyPhysician.Controls.Add(this.lblSpecifyByName);
            this.panelSpecifyPhysician.Controls.Add(this.btnVerify);
            this.panelSpecifyPhysician.Controls.Add(this.mtbRef);
            this.panelSpecifyPhysician.Location = new System.Drawing.Point(15, 21);
            this.panelSpecifyPhysician.Name = "panelSpecifyPhysician";
            this.panelSpecifyPhysician.Size = new System.Drawing.Size(293, 80);
            this.panelSpecifyPhysician.TabIndex = 0;
            // 
            // lblPcp
            // 
            this.lblPcp.Location = new System.Drawing.Point(185, 29);
            this.lblPcp.Name = "lblPcp";
            this.lblPcp.Size = new System.Drawing.Size(38, 17);
            this.lblPcp.TabIndex = 61;
            this.lblPcp.Text = "PCP:";
            // 
            // mtbPCP
            // 
            this.mtbPCP.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbPCP.KeyPressExpression = "^\\d*$";
            this.mtbPCP.Location = new System.Drawing.Point(186, 47);
            this.mtbPCP.Mask = "";
            this.mtbPCP.MaxLength = 5;
            this.mtbPCP.Multiline = true;
            this.mtbPCP.Name = "mtbPCP";
            this.mtbPCP.Size = new System.Drawing.Size(37, 20);
            this.mtbPCP.TabIndex = 5;
            this.mtbPCP.ValidationExpression = "^\\d*$";
            this.mtbPCP.Enter += new System.EventHandler(this.PhysicianNum_Enter);
            this.mtbPCP.Validating += new System.ComponentModel.CancelEventHandler(this.PhysicianNum_Validating);
            // 
            // lblOpr
            // 
            this.lblOpr.Location = new System.Drawing.Point(142, 29);
            this.lblOpr.Name = "lblOpr";
            this.lblOpr.Size = new System.Drawing.Size(28, 14);
            this.lblOpr.TabIndex = 59;
            this.lblOpr.Text = "Opr:";
            // 
            // mtbOpr
            // 
            this.mtbOpr.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOpr.KeyPressExpression = "^\\d*$";
            this.mtbOpr.Location = new System.Drawing.Point(143, 47);
            this.mtbOpr.Mask = "";
            this.mtbOpr.MaxLength = 5;
            this.mtbOpr.Multiline = true;
            this.mtbOpr.Name = "mtbOpr";
            this.mtbOpr.Size = new System.Drawing.Size(37, 20);
            this.mtbOpr.TabIndex = 4;
            this.mtbOpr.ValidationExpression = "^\\d*$";
            this.mtbOpr.Enter += new System.EventHandler(this.PhysicianNum_Enter);
            this.mtbOpr.Validating += new System.ComponentModel.CancelEventHandler(this.PhysicianNum_Validating);
            // 
            // lblAtt
            // 
            this.lblAtt.Location = new System.Drawing.Point(99, 30);
            this.lblAtt.Name = "lblAtt";
            this.lblAtt.Size = new System.Drawing.Size(29, 13);
            this.lblAtt.TabIndex = 57;
            this.lblAtt.Text = "Att:";
            // 
            // mtbAtt
            // 
            this.mtbAtt.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAtt.KeyPressExpression = "^\\d*$";
            this.mtbAtt.Location = new System.Drawing.Point(99, 47);
            this.mtbAtt.Mask = "";
            this.mtbAtt.MaxLength = 5;
            this.mtbAtt.Multiline = true;
            this.mtbAtt.Name = "mtbAtt";
            this.mtbAtt.Size = new System.Drawing.Size(37, 20);
            this.mtbAtt.TabIndex = 3;
            this.mtbAtt.ValidationExpression = "^\\d*$";
            this.mtbAtt.Enter += new System.EventHandler(this.PhysicianNum_Enter);
            this.mtbAtt.Validating += new System.ComponentModel.CancelEventHandler(this.PhysicianNum_Validating);
            // 
            // lblAdm
            // 
            this.lblAdm.Location = new System.Drawing.Point(56, 30);
            this.lblAdm.Name = "lblAdm";
            this.lblAdm.Size = new System.Drawing.Size(29, 13);
            this.lblAdm.TabIndex = 55;
            this.lblAdm.Text = "Adm:";
            // 
            // mtbAdm
            // 
            this.mtbAdm.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdm.KeyPressExpression = "^\\d*$";
            this.mtbAdm.Location = new System.Drawing.Point(55, 47);
            this.mtbAdm.Mask = "";
            this.mtbAdm.MaxLength = 5;
            this.mtbAdm.Multiline = true;
            this.mtbAdm.Name = "mtbAdm";
            this.mtbAdm.Size = new System.Drawing.Size(37, 20);
            this.mtbAdm.TabIndex = 2;
            this.mtbAdm.ValidationExpression = "^\\d*$";
            this.mtbAdm.Enter += new System.EventHandler(this.PhysicianNum_Enter);
            this.mtbAdm.Validating += new System.ComponentModel.CancelEventHandler(this.PhysicianNum_Validating);
            // 
            // lblRef
            // 
            this.lblRef.Location = new System.Drawing.Point(11, 30);
            this.lblRef.Name = "lblRef";
            this.lblRef.Size = new System.Drawing.Size(32, 13);
            this.lblRef.TabIndex = 0;
            this.lblRef.Text = "Ref:";
            // 
            // lblSpecifyByName
            // 
            this.lblSpecifyByName.Location = new System.Drawing.Point(11, 10);
            this.lblSpecifyByName.Name = "lblSpecifyByName";
            this.lblSpecifyByName.Size = new System.Drawing.Size(204, 16);
            this.lblSpecifyByName.TabIndex = 50;
            this.lblSpecifyByName.Text = "Specify physician by number";
            // 
            // btnVerify
            // 
            this.btnVerify.Location = new System.Drawing.Point(229, 45);
            this.btnVerify.Message = "Click physician verify";
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(56, 23);
            this.btnVerify.TabIndex = 6;
            this.btnVerify.Text = "&Verify";
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // mtbRef
            // 
            this.mtbRef.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRef.KeyPressExpression = "^\\d*$";
            this.mtbRef.Location = new System.Drawing.Point(12, 47);
            this.mtbRef.Mask = "";
            this.mtbRef.MaxLength = 5;
            this.mtbRef.Multiline = true;
            this.mtbRef.Name = "mtbRef";
            this.mtbRef.Size = new System.Drawing.Size(37, 20);
            this.mtbRef.TabIndex = 1;
            this.mtbRef.ValidationExpression = "^\\d*$";
            this.mtbRef.Enter += new System.EventHandler(this.PhysicianNum_Enter);
            this.mtbRef.Validating += new System.ComponentModel.CancelEventHandler(this.PhysicianNum_Validating);
            // 
            // lineLabel1
            // 
            this.lineLabel1.Caption = "";
            this.lineLabel1.Location = new System.Drawing.Point(14, 95);
            this.lineLabel1.Name = "lineLabel1";
            this.lineLabel1.Size = new System.Drawing.Size(519, 18);
            this.lineLabel1.TabIndex = 84;
            this.lineLabel1.TabStop = false;
            // 
            // lineLabel2
            // 
            this.lineLabel2.Caption = "";
            this.lineLabel2.Location = new System.Drawing.Point(16, 125);
            this.lineLabel2.Name = "lineLabel2";
            this.lineLabel2.Size = new System.Drawing.Size(517, 15);
            this.lineLabel2.TabIndex = 85;
            this.lineLabel2.TabStop = false;
            // 
            // lineLabel3
            // 
            this.lineLabel3.Caption = "";
            this.lineLabel3.Location = new System.Drawing.Point(15, 155);
            this.lineLabel3.Name = "lineLabel3";
            this.lineLabel3.Size = new System.Drawing.Size(517, 18);
            this.lineLabel3.TabIndex = 86;
            this.lineLabel3.TabStop = false;
            // 
            // lineLabel4
            // 
            this.lineLabel4.Caption = "";
            this.lineLabel4.Location = new System.Drawing.Point(15, 186);
            this.lineLabel4.Name = "lineLabel4";
            this.lineLabel4.Size = new System.Drawing.Size(518, 15);
            this.lineLabel4.TabIndex = 87;
            this.lineLabel4.TabStop = false;
            // 
            // lineLabel5
            // 
            this.lineLabel5.Caption = "";
            this.lineLabel5.Location = new System.Drawing.Point(15, 216);
            this.lineLabel5.Name = "lineLabel5";
            this.lineLabel5.Size = new System.Drawing.Size(518, 14);
            this.lineLabel5.TabIndex = 88;
            this.lineLabel5.TabStop = false;
            // 
            // lineLabel7
            // 
            this.lineLabel7.Caption = "";
            this.lineLabel7.Location = new System.Drawing.Point(14, 246);
            this.lineLabel7.Name = "lineLabel7";
            this.lineLabel7.Size = new System.Drawing.Size(518, 14);
            this.lineLabel7.TabIndex = 89;
            this.lineLabel7.TabStop = false;
            // 
            // lblFind
            // 
            this.lblFind.Location = new System.Drawing.Point(0, 0);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(100, 23);
            this.lblFind.TabIndex = 0;
            // 
            // PhysicianSelectionView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.grpPhysicians);
            this.Name = "PhysicianSelectionView";
            this.Size = new System.Drawing.Size(559, 279);
            this.Leave += new System.EventHandler(this.PhysicianSelectionView_Leave);
            this.grpPhysicians.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panelSpecifyPhysician.ResumeLayout(false);
            this.panelSpecifyPhysician.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        #region Data Elements
        private Container components = null;
        private GroupBox grpPhysicians;
        private Label lblPcp;
        private MaskedEditTextBox mtbPCP;
        private Label lblOpr;
        private MaskedEditTextBox mtbOpr;
        private Label lblAtt;
        private MaskedEditTextBox mtbAtt;
        private Label lblAdm;
        private MaskedEditTextBox mtbAdm;
        private Label lblRef;
        private Label lblSpecifyByName;
        private LoggingButton btnVerify;
        private MaskedEditTextBox mtbRef;
        private Panel panelSpecifyPhysician;
        private Panel panel1;
        private LoggingButton btnFind;
        private Label lblFind;
        private Panel panel2;
        private LoggingButton btnRecNonStaff;
        private Label label2;
        private Label label8;
        private Label label11;
        private LoggingButton btnAdmViewDetails;
        private LoggingButton btnRefViewDetails;
        private LoggingButton btnOthViewDetails;
        private LoggingButton btnOprViewDetails;
        private LoggingButton btnAttViewDetails;
        private LoggingButton btnRefClear;
        private LoggingButton btnOthClear;
        private LoggingButton btnOprClear;
        private LoggingButton btnAttClear;
        private LoggingButton btnAdmClear;
        private Label lblPcpDisplayVal;
        private Label lblOprDisplayVal;
        private Label lblAttDisplayVal;
        private Label lblAdmDisplayVal;
        private Label lblRefDisplayVal;
        private Label lblRecNonStaffPhysician;
        private Label lblFindPhysician;
        private Label lblPcpInfo;
        private Label lblOprInfo;
        private LineLabel lineLabel1;
        private LineLabel lineLabel2;
        private LineLabel lineLabel3;
        private LineLabel lineLabel4;
        private LineLabel lineLabel5;
        private LineLabel lineLabel7;
        private string i_PhysicianRelationship;
        private LoggingButton i_btnOriginalAcceptButton;

        private bool invalidPhysicianEntry;
        private bool enablePrimaryCarePhysicianForPreMse;
        private enum SelectedPhysicianTab
        {
            SEARCH_BY_NAME = 0,
            REC_NONSTAFF_PHYSICIAN = 2,
        }
        #endregion

        #region Constants
        private const long NONSTAFFPHYSICIAN_NBR = 8888L;
        private const string PHYSICIAN_DISPAY_FORMAT = "{0:00000} {1}";
        #endregion
    }
}
