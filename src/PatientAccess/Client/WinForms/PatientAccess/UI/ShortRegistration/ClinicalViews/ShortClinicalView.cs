using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.ClinicalViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.ShortRegistration.ClinicalViews
{
    /// <summary>
    /// Summary description for ShortClinicalView.
    /// </summary>
    public class ShortClinicalView : ControlView, IClinicalTrialsView
    {
        #region Events

        public event EventHandler FocusOutOfPhysicianSelectArea;
        public event EventHandler EnableInsuranceTab;

        #endregion

        #region Event Handlers

        private void ClinicalView_Enter( object sender, EventArgs e )
        {
            IAccountView accountView = AccountView.GetInstance();

            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if ( accountView.IsMedicareAdvisedForPatient() )
            {
                accountView.MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                                                             UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                                                             MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

                if ( warningResult == DialogResult.Yes )
                {
                    if ( EnableInsuranceTab != null )
                    {
                        EnableInsuranceTab( this, new LooseArgs( Model_Account ) );
                        ClinicalView_Leave( sender, e );
                    }
                }
            }
        }

        private void ClinicalView_Leave( object sender, EventArgs e )
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( OnClinicalForm ), Model );
        }

        private void BloodlessRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboBloodless );
        }

        private void PatientInClinicalResearchStudyRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboPatientInClinicalResearch );
        }

        private void PatientInClinicalResearchStudyPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cboPatientInClinicalResearch );
        }

        private void ClinicalView_Disposed( object sender, EventArgs e )
        {
            unregisterRules();
        }

        private void cboPregnant_SelectedIndexChanged( object sender, EventArgs e )
        {
            Model_Account.Pregnant = (YesNoFlag)cboPregnant.SelectedItem;
        }

        private void cboBloodless_SelectedIndexChanged( object sender, EventArgs e )
        {
            Model_Account.Bloodless = (YesNoFlag)cboBloodless.SelectedItem;
            UIColors.SetNormalBgColor( cboBloodless );

            RuleEngine.GetInstance().EvaluateRule( typeof( BloodlessRequired ), Model_Account );
        }

        private void ClinicalView_Validating( object sender, CancelEventArgs e )
        {
            if ( !DesignMode )
            {
                UpdateModel();
            }
        }

        private void cboBloodless_Enter( object sender, EventArgs e )
        {
            FocusOutOfPhysicianSelectArea( null, EventArgs.Empty );
        }

        private void cboPregnant_Enter( object sender, EventArgs e )
        {
            FocusOutOfPhysicianSelectArea( null, EventArgs.Empty );
        }

        private void btnViewClinicalTrialsDetails_Click( object sender, EventArgs e )
        {
            ClinicalTrialsPresenter.ShowDetails();
        }

        private void cboPatientInClinicalResearch_SelectedIndexChanged( object sender, EventArgs e )
        {
            ClinicalTrialsPresenter.EvaluateClinicalResearchFieldRules();
        }

        private void cboPatientInClinicalResearch_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( cboPatientInClinicalResearch );

            ClinicalTrialsPresenter.EvaluateClinicalResearchFieldRules();
        }

        void cboPatientInClinicalResearch_SelectionChangeCommitted( object sender, EventArgs e )
        {
            // This variable is set to avoid issues caused by using a mouse to get focus of the combo box 
            // and then tabbing out of the control to make a selection, or using an arrow key and pressing
            // enter to make a selection. The SelectionChangeCommitted event does not fire if the user tabs
            // in and out. This is a known defect with the combo box control. The DropDownClosed event
            // handler and boolean variable are used as a work-around under the above defined scenarios.
            // References:
            // http://social.msdn.microsoft.com/Forums/en-US/winformsdatacontrols/thread/ad430abc-5ebc-4309-bc43-4e0b2fa8f327
            // https://connect.microsoft.com/VisualStudio/feedback/details/95320/dropdownlist-event-selectionchangecommitted-broken

            userChangedIsPatientInClinicalResearchStudy = true;

            if ( cboPatientInClinicalResearch.SelectedItem != null )
            {
                ClinicalTrialsPresenter.UserChangedPatientInClinicalTrialsTo(
                    ( (YesNoFlag)cboPatientInClinicalResearch.SelectedItem ) );
            }
        }

        void cboPatientInClinicalResearch_DropDownClosed( object sender, EventArgs e )
        {
            if ( !userChangedIsPatientInClinicalResearchStudy )
            {
                if ( cboPatientInClinicalResearch.SelectedItem != null )
                {
                    ClinicalTrialsPresenter.UserChangedPatientInClinicalTrialsTo(
                        ( (YesNoFlag)cboPatientInClinicalResearch.SelectedItem ) );

                    UIColors.SetNormalBgColor( cboPatientInClinicalResearch );

                    ClinicalTrialsPresenter.EvaluateClinicalResearchFieldRules();
                }
            }
            userChangedIsPatientInClinicalResearchStudy = false;
        }

        #endregion // Event Handlers

        #region Public Methods

        public void PopulateClinicalResearchField()
        {
            cboPatientInClinicalResearch.Items.Clear();

            var blank = new YesNoFlag();
            blank.SetBlank( String.Empty );
            cboPatientInClinicalResearch.Items.Add( blank );

            var yes = new YesNoFlag();
            yes.SetYes( "Yes" );
            cboPatientInClinicalResearch.Items.Add( yes );

            var no = new YesNoFlag();
            no.SetNo( "No" );
            cboPatientInClinicalResearch.Items.Add( no );

            if ( Model_Account.IsPatientInClinicalResearchStudy != null )
            {
                cboPatientInClinicalResearch.SelectedIndex =
                    cboPatientInClinicalResearch.FindString(
                        Model_Account.IsPatientInClinicalResearchStudy.Description.ToUpper() );
            }
            else
            {
                cboPatientInClinicalResearch.SelectedIndex = 0;
            }
        }

        public void ShowClinicalResearchFieldDisabled()
        {
            if ( cboPatientInClinicalResearch.Items.Count > 0 )
            {
                if ( Model_Account.IsPatientInClinicalResearchStudy != null )
                {
                    cboPatientInClinicalResearch.SelectedIndex =
                        cboPatientInClinicalResearch.FindString(
                            Model_Account.IsPatientInClinicalResearchStudy.Description.ToUpper() );
                }
                else
                {
                    cboPatientInClinicalResearch.SelectedIndex = 0;
                }
            }

            UIColors.SetDisabledDarkBgColor( cboPatientInClinicalResearch );

            cboPatientInClinicalResearch.Enabled = false;
        }

        public void ShowClinicalResearchFieldEnabled()
        {
            cboPatientInClinicalResearch.Enabled = true;
            UIColors.SetNormalBgColor( cboPatientInClinicalResearch );
        }

        public void ShowClinicalResearchFieldsAsVisible( bool show )
        {
            lblPatientUnderResearchStudy.Visible = show;
            cboPatientInClinicalResearch.Visible = show;
        }

        public override void UpdateView()
        {
            ClinicalTrialsPresenter = new ClinicalTrialsPresenter( this, ClinicalTrialsDetailsView, new ClinicalTrialsFeatureManager( ConfigurationManager.AppSettings ), Model_Account, User.GetCurrent().Facility.Oid, BrokerFactory.BrokerOfType<IResearchStudyBroker>() );

            physicianSelectionView1.Model = Model_Account;
            physicianSelectionView1.UpdateView();

            PopulateBloodlessList();
            PopulatePregnantList();
            ClinicalTrialsPresenter.HandleClinicalResearchFields( Model_Account.AdmitDate );

            registerRules();
            runRules();
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
            Model_Account.Bloodless = (YesNoFlag)cboBloodless.SelectedItem;

            if ( cboPregnant.SelectedIndex > 0 )
            {
                Model_Account.Pregnant = (YesNoFlag)cboPregnant.SelectedItem;
            }
        }

        #endregion

        #region Properties

        private IClinicalTrialsDetailsView ClinicalTrialsDetailsView { get; set; }
        private IClinicalTrialsPresenter ClinicalTrialsPresenter { get; set; }

        private Account Model_Account
        {
            get { return (Account)Model; }
            set { Model = value; }
        }

        public YesNoFlag IsPatientInClinicalResearchStudy
        {
            get { return (YesNoFlag)cboPatientInClinicalResearch.SelectedItem; }
            set
            {
                if ( cboPatientInClinicalResearch.Items.Count != 0 )
                {
                    cboPatientInClinicalResearch.SelectedIndex =
                        cboPatientInClinicalResearch.FindString( value.Description );
                }
            }
        }

        public bool ViewDetailsCommandVisible
        {
            get
            {
                return btnViewClinicalTrialsDetails.Visible;
            }

            set
            {
                btnViewClinicalTrialsDetails.Visible = value;
            }
        }

        public bool ViewDetailsCommandEnabled
        {
            get
            {
                return btnViewClinicalTrialsDetails.Enabled;
            }
            set
            {
                btnViewClinicalTrialsDetails.Enabled = value;
            }
        }


        public bool GetConfirmationForDiscardingPatientStudies()
        {
            var result = MessageBox.Show( UIErrorMessages.WILL_LOSE_CLINICALTRIALS_DATA_ON_CLINICAL_VIEW_SCREEN_WARNING_MESSAGE, "Warning!",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2 );
            if ( result == DialogResult.Yes )
            {
                return true;
            }
            
            return false;
        }

        #endregion

        #region Private Methods

        private void registerRules()
        {
            RuleEngine.GetInstance().RegisterEvent( typeof( BloodlessRequired ), new EventHandler( BloodlessRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( PatientInClinicalstudyPreferred ),
                                                   new EventHandler( PatientInClinicalResearchStudyPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( PatientInClinicalstudyRequired ),
                                                   new EventHandler( PatientInClinicalResearchStudyRequiredEventHandler ) );
        }

        private void unregisterRules()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( BloodlessRequired ), new EventHandler( BloodlessRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PatientInClinicalstudyPreferred ),
                                                     new EventHandler( PatientInClinicalResearchStudyPreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PatientInClinicalstudyRequired ),
                                                     new EventHandler( PatientInClinicalResearchStudyRequiredEventHandler ) );
        }

        private void runRules()
        {
            UIColors.SetNormalBgColor( cboBloodless );
            RuleEngine.GetInstance().EvaluateRule( typeof( OnClinicalForm ), Model );
        }

        private void PopulateBloodlessList()
        {
            cboBloodless.Items.Clear();

            var blank = new YesNoFlag();
            blank.SetBlank( String.Empty );
            cboBloodless.Items.Add( blank );

            var yes = new YesNoFlag();
            yes.SetYes( "Yes, desires treatment without blood" );
            cboBloodless.Items.Add( yes );

            var no = new YesNoFlag();
            no.SetNo( "No, desires treatment with blood" );
            cboBloodless.Items.Add( no );

            cboBloodless.SelectedIndex = 
                Model_Account.Bloodless != null ? cboBloodless.FindString( Model_Account.Bloodless.Description.ToUpper() ) : 0;
        }

        private void PopulatePregnantList()
        {
            cboPregnant.Items.Clear();

            var blank = new YesNoFlag();
            blank.SetBlank();
            cboPregnant.Items.Add( blank );

            var yes = new YesNoFlag();
            yes.SetYes();
            cboPregnant.Items.Add( yes );

            var no = new YesNoFlag();
            no.SetNo();
            cboPregnant.Items.Add( no );

            if ( Model_Account.Patient == null
                || Model_Account.Patient.Sex == null
                || Model_Account.Patient.Sex.Code != Gender.FEMALE_CODE
                || Model_Account.Activity.GetType().Equals( typeof( AdmitNewbornActivity ) ) )
            {
                cboPregnant.Enabled = false;
            }
            else
            {
                cboPregnant.Enabled = true;
                if ( ( Model_Account.Pregnant == null ) ||
                    ( Model_Account.Pregnant.Code != YesNoFlag.CODE_YES &&
                      Model_Account.Pregnant.Code != YesNoFlag.CODE_NO ) )
                {
                    cboPregnant.SelectedItem = blank;
                }
                else
                {
                    cboPregnant.SelectedItem = Model_Account.Pregnant;
                }
            }

            if ( Model_Account.Patient != null && 
                 Model_Account.Patient.Sex != null && 
                 Model_Account.Patient.Sex.Code == Gender.FEMALE_CODE &&
                 Model_Account.Activity.GetType().Equals( typeof( AdmitNewbornActivity ) ) )
            {
                cboPregnant.SelectedItem = no;
            }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboBloodless = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblBloodless = new System.Windows.Forms.Label();
            this.cboPregnant = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblPregnant = new System.Windows.Forms.Label();
            this.physicianSelectionView1 = new PatientAccess.UI.CommonControls.PhysicianSelectionView();
            this.cboPatientInClinicalResearch = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblPatientUnderResearchStudy = new System.Windows.Forms.Label();
            this.btnViewClinicalTrialsDetails = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cboBloodless
            // 
            this.cboBloodless.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBloodless.Location = new System.Drawing.Point( 674, 24 );
            this.cboBloodless.Name = "cboBloodless";
            this.cboBloodless.Size = new System.Drawing.Size( 265, 21 );
            this.cboBloodless.TabIndex = 2;
            this.cboBloodless.SelectedIndexChanged += new System.EventHandler( this.cboBloodless_SelectedIndexChanged );
            this.cboBloodless.Enter += new System.EventHandler( this.cboBloodless_Enter );
            // 
            // lblBloodless
            // 
            this.lblBloodless.Location = new System.Drawing.Point( 575, 28 );
            this.lblBloodless.Name = "lblBloodless";
            this.lblBloodless.Size = new System.Drawing.Size( 75, 19 );
            this.lblBloodless.TabIndex = 27;
            this.lblBloodless.Text = "Bloodless:";
            // 
            // cboPregnant
            // 
            this.cboPregnant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPregnant.Location = new System.Drawing.Point( 674, 51 );
            this.cboPregnant.Name = "cboPregnant";
            this.cboPregnant.Size = new System.Drawing.Size( 49, 21 );
            this.cboPregnant.TabIndex = 3;
            this.cboPregnant.SelectedIndexChanged += new System.EventHandler( this.cboPregnant_SelectedIndexChanged );
            this.cboPregnant.Enter += new System.EventHandler( this.cboPregnant_Enter );
            // 
            // lblPregnant
            // 
            this.lblPregnant.Location = new System.Drawing.Point( 575, 53 );
            this.lblPregnant.Name = "lblPregnant";
            this.lblPregnant.Size = new System.Drawing.Size( 76, 19 );
            this.lblPregnant.TabIndex = 30;
            this.lblPregnant.Text = "Pregnant:";
            // 
            // physicianSelectionView1
            // 
            this.physicianSelectionView1.BackColor = System.Drawing.Color.White;
            this.physicianSelectionView1.Location = new System.Drawing.Point( 10, 14 );
            this.physicianSelectionView1.Model = null;
            this.physicianSelectionView1.Name = "physicianSelectionView1";
            this.physicianSelectionView1.Size = new System.Drawing.Size( 559, 279 );
            this.physicianSelectionView1.TabIndex = 0;
            // 
            // cboPatientInClinicalResearch
            // 
            this.cboPatientInClinicalResearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPatientInClinicalResearch.DropDownWidth = 3;
            this.cboPatientInClinicalResearch.Location = new System.Drawing.Point( 775, 77 );
            this.cboPatientInClinicalResearch.MaxLength = 3;
            this.cboPatientInClinicalResearch.Name = "cboPatientInClinicalResearch";
            this.cboPatientInClinicalResearch.Size = new System.Drawing.Size( 47, 21 );
            this.cboPatientInClinicalResearch.TabIndex = 37;
            this.cboPatientInClinicalResearch.SelectedIndexChanged += new System.EventHandler( this.cboPatientInClinicalResearch_SelectedIndexChanged );
            this.cboPatientInClinicalResearch.SelectionChangeCommitted += new System.EventHandler( this.cboPatientInClinicalResearch_SelectionChangeCommitted );
            this.cboPatientInClinicalResearch.DropDownClosed += new System.EventHandler( this.cboPatientInClinicalResearch_DropDownClosed );
            this.cboPatientInClinicalResearch.Validating += new System.ComponentModel.CancelEventHandler( this.cboPatientInClinicalResearch_Validating );
            // 
            // lblPatientUnderResearchStudy
            // 
            this.lblPatientUnderResearchStudy.BackColor = System.Drawing.Color.White;
            this.lblPatientUnderResearchStudy.Location = new System.Drawing.Point( 575, 80 );
            this.lblPatientUnderResearchStudy.Name = "lblPatientUnderResearchStudy";
            this.lblPatientUnderResearchStudy.Size = new System.Drawing.Size( 202, 16 );
            this.lblPatientUnderResearchStudy.TabIndex = 0;
            this.lblPatientUnderResearchStudy.Text = "Is patient in a Clinical Research Study? :";
            // 
            // btnViewClinicalTrialsDetails
            // 
            this.btnViewClinicalTrialsDetails.Location = new System.Drawing.Point( 864, 75 );
            this.btnViewClinicalTrialsDetails.Name = "btnViewClinicalTrialsDetails";
            this.btnViewClinicalTrialsDetails.Size = new System.Drawing.Size( 75, 23 );
            this.btnViewClinicalTrialsDetails.TabIndex = 38;
            this.btnViewClinicalTrialsDetails.Text = "View Details";
            this.btnViewClinicalTrialsDetails.UseVisualStyleBackColor = true;
            this.btnViewClinicalTrialsDetails.Visible = false;
            this.btnViewClinicalTrialsDetails.Click += new System.EventHandler( this.btnViewClinicalTrialsDetails_Click );
            // 
            // ShortClinicalView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.btnViewClinicalTrialsDetails );
            this.Controls.Add( this.cboPatientInClinicalResearch );
            this.Controls.Add( this.lblPatientUnderResearchStudy );
            this.Controls.Add( this.physicianSelectionView1 );
            this.Controls.Add( this.cboPregnant );
            this.Controls.Add( this.lblPregnant );
            this.Controls.Add( this.cboBloodless );
            this.Controls.Add( this.lblBloodless );
            this.Name = "ShortClinicalView";
            this.Size = new System.Drawing.Size( 992, 332 );
            this.Enter += new System.EventHandler( this.ClinicalView_Enter );
            this.Leave += new System.EventHandler( this.ClinicalView_Leave );
            this.Validating += new System.ComponentModel.CancelEventHandler( this.ClinicalView_Validating );
            this.Disposed += new System.EventHandler( this.ClinicalView_Disposed );
            this.ResumeLayout( false );

        }

        #endregion

        #endregion

        #region Construction and Finalization

        public ShortClinicalView()
        {
            InitializeComponent();

            ClinicalTrialsDetailsView = new ClinicalTrialsDetailsView();
        }

        #endregion

        #region Data Elements

        private PatientAccessComboBox cboBloodless;
        private PatientAccessComboBox cboPatientInClinicalResearch;
        private PatientAccessComboBox cboPregnant;
        private Label lblBloodless;
        private Label lblPatientUnderResearchStudy;
        private Label lblPregnant;
        private Button btnViewClinicalTrialsDetails;
        private PhysicianSelectionView physicianSelectionView1;
        private bool userChangedIsPatientInClinicalResearchStudy;

        #endregion


        #region Constants

        #endregion
    }
}
