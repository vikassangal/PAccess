using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.ClinicalViews
{
    /// <summary>
    /// Summary description for ClinicalView.
    /// </summary>
    public class ClinicalView : ControlView, IRightCareRigtPlaceView, IClinicalTrialsView
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

        private void BloodlessPreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( cboBloodless );
        }

        private void PatientInClinicalResearchStudyRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboPatientInClinicalResearch );
        }

        private void PatientInClinicalResearchStudyPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cboPatientInClinicalResearch );
        }

        private void LeftOrStayedRequiredEvent( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboLeftOrStayed );
        }

        private void RightCareRightPlaceRequiredEvent( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboRightCareRightPlace );
        }

        private void cboRightCareRightPlace_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleRCRPSelectedIndexChanged( (YesNoFlag)cboRightCareRightPlace.SelectedItem );
        }
        private void HandleRCRPSelectedIndexChanged( YesNoFlag newSelection )

        {
            RightCareRightPlacePresenter.UpdateRightCareRightPlace( newSelection );
        }

        private void cboLeftOrStayed_SelectedIndexChanged( object sender, EventArgs e )
        {
            RightCareRightPlacePresenter.UpdateLeftOrStayed( (LeftOrStayed)cboLeftOrStayed.SelectedItem );
        }

        void cboRightCareRightPlace_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( cboRightCareRightPlace );
            if ( cboRightCareRightPlace.SelectedItem != null )
            {
                HandleRCRPSelectedIndexChanged( (YesNoFlag)cboRightCareRightPlace.SelectedItem );
            }
        }

        private void cboLeftOrStayed_Validating( object sender, CancelEventArgs e )
        {
            if ( cboLeftOrStayed.SelectedItem != null )
            {
                RightCareRightPlacePresenter.UpdateLeftOrStayed( (LeftOrStayed)cboLeftOrStayed.SelectedItem );
            }
            UIColors.SetNormalBgColor( cboLeftOrStayed );
            RightCareRightPlacePresenter.EvaluateViewRules();
        }

        private void cboLeftWithoutBeingSeen_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( cboLeftWithoutBeingSeen.SelectedItem != null )
            {
                RightCareRightPlacePresenter.UpdateLeftWithoutBeingSeen(
                    (YesNoFlag)cboLeftWithoutBeingSeen.SelectedItem );
            }
        }

        private void cboLeftWithoutFinancialClearance_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( cboLeftWithoutFinancialClearance.SelectedItem != null )
            {
                RightCareRightPlacePresenter.UpdateLeftWithoutFinancialClearance(
                    (YesNoFlag)cboLeftWithoutFinancialClearance.SelectedItem );
            }
        }

        private void cboLeftWithoutBeingSeen_validating( object sender, CancelEventArgs e )
        {
            if ( cboLeftWithoutBeingSeen.SelectedItem != null )
            {
                RightCareRightPlacePresenter.UpdateLeftWithoutBeingSeen(
                    (YesNoFlag)cboLeftWithoutBeingSeen.SelectedItem );
            }
        }

        private void cboLeftWithoutFinancialClearance_Validating( object sender, CancelEventArgs e )
        {
            if ( cboLeftWithoutFinancialClearance.SelectedItem != null )
            {
                RightCareRightPlacePresenter.UpdateLeftWithoutFinancialClearance(
                    (YesNoFlag)cboLeftWithoutFinancialClearance.SelectedItem );
            }
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
            RuleEngine.GetInstance().EvaluateRule( typeof( BloodlessPreferred ), Model_Account);
        }

        private void ClinicalView_Validating( object sender, CancelEventArgs e )
        {
            if ( !DesignMode )
            {
                UpdateModel();
            }
        }

        private void cboSmoker_Enter( object sender, EventArgs e )
        {
            FocusOutOfPhysicianSelectArea( null, EventArgs.Empty );
        }

        private void cboBloodless_Enter( object sender, EventArgs e )
        {
            FocusOutOfPhysicianSelectArea( null, EventArgs.Empty );
        }

        private void cboPregnant_Enter( object sender, EventArgs e )
        {
            FocusOutOfPhysicianSelectArea( null, EventArgs.Empty );
        }

        private void mtbComments_Enter( object sender, EventArgs e )
        {
            FocusOutOfPhysicianSelectArea( null, EventArgs.Empty );
        }

        private void mtbEmbosserCard_Enter( object sender, EventArgs e )
        {
            FocusOutOfPhysicianSelectArea( null, EventArgs.Empty );
        }

        private void btnViewClinicalTrialsDetails_Click( object sender, EventArgs e )
        {
            this.ClinicalTrialsPresenter.ShowDetails();
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

        public bool RCRPVisible
        {
            set
            {
                lblRightCareRightPlace.Visible = value;
                cboRightCareRightPlace.Visible = value;
            }
            get
            {
                return lblRightCareRightPlace.Visible && cboRightCareRightPlace.Visible;
            }

        }

        public bool RCRPEnabled
        {
            set
            {
                cboRightCareRightPlace.Enabled = value;
                if ( !value )
                {
                    UIColors.SetDisabledDarkBgColor( cboRightCareRightPlace );
                    return;
                }
                UIColors.SetNormalBgColor( cboRightCareRightPlace );
            }
            get
            {
                return cboRightCareRightPlace.Enabled;
            }
        }

        public bool LeftOrStayedVisible
        {
            set
            {
                lblLeftOrStayed.Visible = value;
                cboLeftOrStayed.Visible = value;
            }

            get
            {
                return lblLeftOrStayed.Visible && cboLeftOrStayed.Visible;
            }
        }

        public bool LeftOrStayedEnabled
        {
            set
            {
                cboLeftOrStayed.Enabled = value;
                if ( !value )
                {
                    UIColors.SetDisabledDarkBgColor( cboLeftOrStayed );
                    return;
                }
                UIColors.SetNormalBgColor( cboLeftOrStayed );
            }
            get
            {
                return cboLeftOrStayed.Enabled;
            }
        }

        public bool LeftWithoutBeingSeenVisible
        {
            set
            {
                LblLeftWithoutBeingSeen.Visible = value;
                cboLeftWithoutBeingSeen.Visible = value;
            }

            get
            {
                return LblLeftWithoutBeingSeen.Visible && cboLeftWithoutBeingSeen.Visible;

            }
        }

        public bool LeftWithoutBeingSeenEnabled
        {
            set
            {
                cboLeftWithoutBeingSeen.Enabled = value;
                if ( !value )
                {
                    UIColors.SetDisabledDarkBgColor( cboLeftWithoutBeingSeen );
                    return;
                }
                UIColors.SetNormalBgColor( cboLeftWithoutBeingSeen );
            }
            get
            {
                return cboLeftWithoutBeingSeen.Enabled;
            }
        }

        public bool LeftWithoutFinancialClearanceVisible
        {
            set
            {
                lblLeftWithoutFinClearance.Visible = value;
                cboLeftWithoutFinancialClearance.Visible = value;
            }
            get
            {
                return lblLeftWithoutFinClearance.Visible && cboLeftWithoutFinancialClearance.Visible;
            }
        }

        public bool LeftWithoutFinancialClearanceEnabled
        {
            set
            {
                cboLeftWithoutFinancialClearance.Enabled = value;
                if ( !value )
                {
                    UIColors.SetDisabledDarkBgColor( cboLeftWithoutFinancialClearance );
                    return;
                }
                UIColors.SetNormalBgColor( cboLeftWithoutFinancialClearance );
            }
            get
            {
                return cboLeftWithoutFinancialClearance.Enabled;
            }
        }

        public void ClearRCRP()
        {
            if ( cboRightCareRightPlace.Items.Count > 0 )
            {
                cboRightCareRightPlace.SelectedIndex = 0;
            }
        }
        public void ClearLeftOrStayed()
        {
            if ( cboLeftOrStayed.Items.Count > 0 )
            {
                cboLeftOrStayed.SelectedIndex = 0;
            }
        }
        public void ClearLeftWithoutBeingSeen()
        {
            if ( cboLeftWithoutBeingSeen.Items.Count > 0 )
            {
                cboLeftWithoutBeingSeen.SelectedIndex = 0;
            }
        }
        public void ClearLeftWithoutFinancialClearance()
        {
            if ( cboLeftWithoutFinancialClearance.Items.Count > 0 )
            {
                cboLeftWithoutFinancialClearance.SelectedIndex = 0;
            }
        }
        public void PopulateRCRP()
        {
            cboRightCareRightPlace.Items.Clear();

            var blank = new YesNoFlag();
            blank.SetBlank( string.Empty );
            cboRightCareRightPlace.Items.Add( blank );

            var yes = new YesNoFlag();
            yes.SetYes();
            cboRightCareRightPlace.Items.Add( yes );

            var no = new YesNoFlag();
            no.SetNo();
            cboRightCareRightPlace.Items.Add( no );

            if ( Model_Account.RightCareRightPlace.RCRP != null )
            {
                cboRightCareRightPlace.SelectedIndex =
                    cboRightCareRightPlace.FindString( Model_Account.RightCareRightPlace.RCRP.Description );
            }
            else
            {
                cboRightCareRightPlace.SelectedIndex = 0;
            }
        }

        public void PopulateLeftOrStayed()
        {
            ILeftOrStayedBroker broker = BrokerFactory.BrokerOfType<ILeftOrStayedBroker>();
            cboLeftOrStayed.Items.Clear();

            foreach ( LeftOrStayed ls in broker.AllLeftOrStayed() )
            {
                cboLeftOrStayed.Items.Add( ls );
            }
            if ( Model_Account.RightCareRightPlace.LeftOrStayed != null )
            {
                this.cboLeftOrStayed.SelectedItem = this.Model_Account.RightCareRightPlace.LeftOrStayed;
            }
            else
            {
                this.cboLeftOrStayed.SelectedIndex = 0;
            }
        }

        public void PopulateLeftWithoutBeingSeenField()
        {
            cboLeftWithoutBeingSeen.Items.Clear();

            var blank = new YesNoFlag();
            blank.SetBlank( string.Empty );
            cboLeftWithoutBeingSeen.Items.Add( blank );

            var yes = new YesNoFlag();
            yes.SetYes();
            cboLeftWithoutBeingSeen.Items.Add( yes );

            var no = new YesNoFlag();
            no.SetNo();
            cboLeftWithoutBeingSeen.Items.Add( no );

            if ( Model_Account.LeftWithOutBeingSeen != null )
            {
                cboLeftWithoutBeingSeen.SelectedIndex =
                    cboLeftWithoutBeingSeen.FindString( Model_Account.LeftWithOutBeingSeen.Description );
            }
            else
            {
                cboLeftWithoutBeingSeen.SelectedIndex = 0;
            }
        }

        public void PopulateLeftWithoutFinancialClearance()
        {
            cboLeftWithoutFinancialClearance.Items.Clear();

            var blank = new YesNoFlag();
            blank.SetBlank( string.Empty );
            cboLeftWithoutFinancialClearance.Items.Add( blank );

            var yes = new YesNoFlag();
            yes.SetYes();
            cboLeftWithoutFinancialClearance.Items.Add( yes );

            var no = new YesNoFlag();
            no.SetNo();
            cboLeftWithoutFinancialClearance.Items.Add( no );

            if ( Model_Account.LeftWithoutFinancialClearance != null )
            {
                cboLeftWithoutFinancialClearance.SelectedIndex =
                    cboLeftWithoutFinancialClearance.FindString( Model_Account.LeftWithoutFinancialClearance.Description );
            }
            else
            {
                cboLeftWithoutFinancialClearance.SelectedIndex = 0;
            }
        }

        public override void UpdateView()
        {
            ClinicalTrialsPresenter = new ClinicalTrialsPresenter( this, ClinicalTrialsDetailsView, new ClinicalTrialsFeatureManager( ConfigurationManager.AppSettings ), Model_Account, User.GetCurrent().Facility.Oid, BrokerFactory.BrokerOfType<IResearchStudyBroker>() );

            RightCareRightPlacePresenter = new RightCareRightPlacePresenter( this, new RightCareRightPlaceFeatureManager( ConfigurationManager.AppSettings ), this.Model_Account );
            physicianSelectionView1.Model = Model_Account;
            physicianSelectionView1.UpdateView();

            PopulateSmokerList();
            PopulateBloodlessList();
            
            Activity thisActivity = this.Model_Account.Activity;
            
            if (thisActivity!=null && thisActivity.IsNewBornRelatedActivity() || thisActivity is AdmitNewbornWithOfflineActivity
                    || thisActivity is PreAdmitNewbornWithOfflineActivity )
            {
                this.lblResistantOrganism.Visible = false;
                this.txtResistantOrganism.Visible = false;
                
            }
            else
            {
                this.lblResistantOrganism.Visible = true;
                this.txtResistantOrganism.Visible = true;
                this.txtResistantOrganism.Text = ( Model_Account.ResistantOrganism.Length > 25 )
                                                     ? Model_Account.ResistantOrganism.Substring(0, 25)
                                                     : Model_Account.ResistantOrganism;

            }
            
            PopulatePregnantList();
            ClinicalTrialsPresenter.HandleClinicalResearchFields( Model_Account.AdmitDate );
            RightCareRightPlacePresenter.UpdateView();
            mtbComments.Text = Model_Account.ClinicalComments;

            if (DOFRFeatureManager.GetInstance().IsDOFREnabledForFacility(Model_Account))
            {
                mtbEmbosserCard.Visible = false;
                lblEmbosserCard.Visible = false;
            }
            else
            {
                mtbEmbosserCard.Text = Model_Account.EmbosserCard;
            }

            registerRules();
            runRules();
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
            Model_Account.Smoker = (YesNoFlag)cboSmoker.SelectedItem;
            Model_Account.Bloodless = (YesNoFlag)cboBloodless.SelectedItem;

            if ( cboPregnant.SelectedIndex > 0 )
            {
                Model_Account.Pregnant = (YesNoFlag)cboPregnant.SelectedItem;
            }
            Model_Account.ClinicalComments = mtbComments.UnMaskedText;
            if(!DOFRFeatureManager.GetInstance().IsDOFREnabledForFacility(Model_Account))
            Model_Account.EmbosserCard = mtbEmbosserCard.UnMaskedText;
        }

        #endregion

        #region Properties

        private IClinicalTrialsDetailsView ClinicalTrialsDetailsView { get; set; }
        private IClinicalTrialsPresenter ClinicalTrialsPresenter { get; set; }

        private IRightCareRightPlacePresenter RightCareRightPlacePresenter { get; set; }

        public Account Model_Account
        {
            get { return (Account)Model; }
            set { Model = value; }
        }

        public YesNoFlag IsPatientInClinicalResearchStudy
        {
            get { return (YesNoFlag)this.cboPatientInClinicalResearch.SelectedItem; }
            set
            {
                if ( this.cboPatientInClinicalResearch.Items.Count != 0 )
                {
                    this.cboPatientInClinicalResearch.SelectedIndex =
                        cboPatientInClinicalResearch.FindString( value.Description );

                }
            }
        }

        public bool ViewDetailsCommandVisible
        {
            get
            {
                return this.btnViewClinicalTrialsDetails.Visible;

            }

            set
            {
                this.btnViewClinicalTrialsDetails.Visible = value;
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
            else
            {
                return false;
            }
        }

        #endregion

        #region Private Methods

        private void registerRules()
        {
            RuleEngine.GetInstance().RegisterEvent( typeof( BloodlessRequired ),
                                                   new EventHandler( BloodlessRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( BloodlessPreferred ),
                                                   new EventHandler( BloodlessPreferredEventHandler ));
            RuleEngine.GetInstance().RegisterEvent( typeof( PatientInClinicalstudyPreferred ),
                                                   new EventHandler( PatientInClinicalResearchStudyPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( PatientInClinicalstudyRequired ),
                                                   new EventHandler( PatientInClinicalResearchStudyRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( RightCareRightPlaceRequired ),
                                                   new EventHandler( RightCareRightPlaceRequiredEvent ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( LeftOrStayedRequired ),
                                                 new EventHandler( LeftOrStayedRequiredEvent ) );

        }

        private void unregisterRules()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( BloodlessRequired ),
                                                     new EventHandler( BloodlessRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent(typeof( BloodlessPreferred ),
                                                     new EventHandler( BloodlessPreferredEventHandler ));
            RuleEngine.GetInstance().UnregisterEvent( typeof( PatientInClinicalstudyPreferred ),
                                                     new EventHandler( PatientInClinicalResearchStudyPreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PatientInClinicalstudyRequired ),
                                                     new EventHandler( PatientInClinicalResearchStudyRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( RightCareRightPlace ),
                                                     new EventHandler( RightCareRightPlaceRequiredEvent ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( LeftOrStayed ),
                                                     new EventHandler( LeftOrStayedRequiredEvent ) );
        }

        private void runRules()
        {
            UIColors.SetNormalBgColor( cboBloodless );
            RuleEngine.GetInstance().EvaluateRule( typeof( OnClinicalForm ), Model );
        }

        private void PopulateSmokerList()
        {
            cboSmoker.Items.Clear();

            var blank = new YesNoFlag();
            blank.SetBlank( string.Empty );
            cboSmoker.Items.Add( blank );

            var yes = new YesNoFlag();
            yes.SetYes();
            cboSmoker.Items.Add( yes );

            var no = new YesNoFlag();
            no.SetNo();
            cboSmoker.Items.Add( no );

            if ( Model_Account.Smoker != null )
            {
                cboSmoker.SelectedItem = Model_Account.Smoker;
            }
            else
            {
                cboSmoker.SelectedIndex = 0;
            }
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

            if ( Model_Account.Bloodless != null )
            {
                cboBloodless.SelectedIndex = cboBloodless.FindString( Model_Account.Bloodless.Description.ToUpper() );
            }
            else
            {
                cboBloodless.SelectedIndex = 0;
            }
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
                || Model_Account.Activity!= null && Model_Account.Activity.IsNewBornRelatedActivity())
            {
                cboPregnant.Enabled = false;
            }
            else
            {
                cboPregnant.Enabled = true;
                if ( ( Model_Account.Pregnant == null ) ||
                    ( Model_Account.Pregnant != null &&
                     Model_Account.Pregnant.Code != YesNoFlag.CODE_YES &&
                     Model_Account.Pregnant.Code != YesNoFlag.CODE_NO ) )
                {
                    cboPregnant.SelectedItem = blank;
                }
                else
                {
                    cboPregnant.SelectedItem = Model_Account.Pregnant;
                }
            }

            if ( Model_Account.Patient.Sex.Code == Gender.FEMALE_CODE
                && ( Model_Account.Activity!= null && Model_Account.Activity.IsNewBornRelatedActivity()) )
            {
                cboPregnant.SelectedItem = no;
            }
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureClinicalViewComments( mtbComments );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSmoker = new System.Windows.Forms.Label();
            this.cboBloodless = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblBloodless = new System.Windows.Forms.Label();
            this.cboPregnant = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblPregnant = new System.Windows.Forms.Label();
            this.lblComments = new System.Windows.Forms.Label();
            this.lblEmbosserCard = new System.Windows.Forms.Label();
            this.mtbEmbosserCard = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cboSmoker = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.physicianSelectionView1 = new PatientAccess.UI.CommonControls.PhysicianSelectionView();
            this.mtbComments = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cboPatientInClinicalResearch = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblPatientUnderResearchStudy = new System.Windows.Forms.Label();
            this.cboRightCareRightPlace = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblRightCareRightPlace = new System.Windows.Forms.Label();
            this.cboLeftOrStayed = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblLeftOrStayed = new System.Windows.Forms.Label();
            this.cboLeftWithoutBeingSeen = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.LblLeftWithoutBeingSeen = new System.Windows.Forms.Label();
            this.cboLeftWithoutFinancialClearance = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblLeftWithoutFinClearance = new System.Windows.Forms.Label();
            this.btnViewClinicalTrialsDetails = new System.Windows.Forms.Button();
            this.lblResistantOrganism = new System.Windows.Forms.Label();
            this.txtResistantOrganism = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblSmoker
            // 
            this.lblSmoker.BackColor = System.Drawing.Color.White;
            this.lblSmoker.Location = new System.Drawing.Point(575, 25);
            this.lblSmoker.Name = "lblSmoker";
            this.lblSmoker.Size = new System.Drawing.Size(59, 22);
            this.lblSmoker.TabIndex = 25;
            this.lblSmoker.Text = "Smoker:";
            // 
            // cboBloodless
            // 
            this.cboBloodless.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBloodless.Location = new System.Drawing.Point(674, 49);
            this.cboBloodless.Name = "cboBloodless";
            this.cboBloodless.Size = new System.Drawing.Size(265, 21);
            this.cboBloodless.TabIndex = 2;
            this.cboBloodless.SelectedIndexChanged += new System.EventHandler(this.cboBloodless_SelectedIndexChanged);
            this.cboBloodless.Enter += new System.EventHandler(this.cboBloodless_Enter);
            // 
            // lblBloodless
            // 
            this.lblBloodless.Location = new System.Drawing.Point(575, 53);
            this.lblBloodless.Name = "lblBloodless";
            this.lblBloodless.Size = new System.Drawing.Size(75, 19);
            this.lblBloodless.TabIndex = 27;
            this.lblBloodless.Text = "Bloodless:";
            // 
            // cboPregnant
            // 
            this.cboPregnant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPregnant.Location = new System.Drawing.Point(674, 76);
            this.cboPregnant.Name = "cboPregnant";
            this.cboPregnant.Size = new System.Drawing.Size(49, 21);
            this.cboPregnant.TabIndex = 3;
            this.cboPregnant.SelectedIndexChanged += new System.EventHandler(this.cboPregnant_SelectedIndexChanged);
            this.cboPregnant.Enter += new System.EventHandler(this.cboPregnant_Enter);
            // 
            // lblPregnant
            // 
            this.lblPregnant.Location = new System.Drawing.Point(575, 78);
            this.lblPregnant.Name = "lblPregnant";
            this.lblPregnant.Size = new System.Drawing.Size(76, 19);
            this.lblPregnant.TabIndex = 30;
            this.lblPregnant.Text = "Pregnant:";
            // 
            // lblComments
            // 
            this.lblComments.Location = new System.Drawing.Point(578, 129);
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size(83, 21);
            this.lblComments.TabIndex = 34;
            this.lblComments.Text = "Comments:";
            // 
            // lblEmbosserCard
            // 
            this.lblEmbosserCard.Location = new System.Drawing.Point(578, 200);
            this.lblEmbosserCard.Name = "lblEmbosserCard";
            this.lblEmbosserCard.Size = new System.Drawing.Size(91, 13);
            this.lblEmbosserCard.TabIndex = 36;
            this.lblEmbosserCard.Text = "Embosser card:";
            // 
            // mtbEmbosserCard
            // 
            this.mtbEmbosserCard.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbEmbosserCard.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEmbosserCard.KeyPressExpression = "^[a-zA-Z0-9]*";
            this.mtbEmbosserCard.Location = new System.Drawing.Point(677, 198);
            this.mtbEmbosserCard.Mask = "";
            this.mtbEmbosserCard.MaxLength = 10;
            this.mtbEmbosserCard.Name = "mtbEmbosserCard";
            this.mtbEmbosserCard.Size = new System.Drawing.Size(103, 20);
            this.mtbEmbosserCard.TabIndex = 6;
            this.mtbEmbosserCard.ValidationExpression = "^[a-zA-Z0-9]*$";
            this.mtbEmbosserCard.Enter += new System.EventHandler(this.mtbEmbosserCard_Enter);
            // 
            // cboSmoker
            // 
            this.cboSmoker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSmoker.Location = new System.Drawing.Point(674, 22);
            this.cboSmoker.Name = "cboSmoker";
            this.cboSmoker.Size = new System.Drawing.Size(47, 21);
            this.cboSmoker.TabIndex = 1;
            this.cboSmoker.Enter += new System.EventHandler(this.cboSmoker_Enter);
            // 
            // physicianSelectionView1
            // 
            this.physicianSelectionView1.BackColor = System.Drawing.Color.White;
            this.physicianSelectionView1.Location = new System.Drawing.Point(10, 14);
            this.physicianSelectionView1.Model = null;
            this.physicianSelectionView1.Name = "physicianSelectionView1";
            this.physicianSelectionView1.Size = new System.Drawing.Size(559, 279);
            this.physicianSelectionView1.TabIndex = 0;
            // 
            // mtbComments
            // 
            this.mtbComments.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbComments.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbComments.Location = new System.Drawing.Point(578, 148);
            this.mtbComments.Mask = "";
            this.mtbComments.MaxLength = 120;
            this.mtbComments.Multiline = true;
            this.mtbComments.Name = "mtbComments";
            this.mtbComments.Size = new System.Drawing.Size(366, 44);
            this.mtbComments.TabIndex = 5;
            this.mtbComments.Enter += new System.EventHandler(this.mtbComments_Enter);
            // 
            // cboPatientInClinicalResearch
            // 
            this.cboPatientInClinicalResearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPatientInClinicalResearch.DropDownWidth = 3;
            this.cboPatientInClinicalResearch.Location = new System.Drawing.Point(778, 222);
            this.cboPatientInClinicalResearch.MaxLength = 3;
            this.cboPatientInClinicalResearch.Name = "cboPatientInClinicalResearch";
            this.cboPatientInClinicalResearch.Size = new System.Drawing.Size(47, 21);
            this.cboPatientInClinicalResearch.TabIndex = 37;
            this.cboPatientInClinicalResearch.SelectedIndexChanged += new System.EventHandler(this.cboPatientInClinicalResearch_SelectedIndexChanged);
            this.cboPatientInClinicalResearch.SelectionChangeCommitted += new System.EventHandler(this.cboPatientInClinicalResearch_SelectionChangeCommitted);
            this.cboPatientInClinicalResearch.DropDownClosed += new System.EventHandler(this.cboPatientInClinicalResearch_DropDownClosed);
            this.cboPatientInClinicalResearch.Validating += new System.ComponentModel.CancelEventHandler(this.cboPatientInClinicalResearch_Validating);
            // 
            // lblPatientUnderResearchStudy
            // 
            this.lblPatientUnderResearchStudy.BackColor = System.Drawing.Color.White;
            this.lblPatientUnderResearchStudy.Location = new System.Drawing.Point(578, 225);
            this.lblPatientUnderResearchStudy.Name = "lblPatientUnderResearchStudy";
            this.lblPatientUnderResearchStudy.Size = new System.Drawing.Size(202, 16);
            this.lblPatientUnderResearchStudy.TabIndex = 0;
            this.lblPatientUnderResearchStudy.Text = "Is patient in a Clinical Research Study? :";
            // 
            // cboRightCareRightPlace
            // 
            this.cboRightCareRightPlace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRightCareRightPlace.DropDownWidth = 3;
            this.cboRightCareRightPlace.Location = new System.Drawing.Point(778, 247);
            this.cboRightCareRightPlace.MaxLength = 3;
            this.cboRightCareRightPlace.Name = "cboRightCareRightPlace";
            this.cboRightCareRightPlace.Size = new System.Drawing.Size(47, 21);
            this.cboRightCareRightPlace.TabIndex = 39;
            this.cboRightCareRightPlace.SelectedIndexChanged += new System.EventHandler(this.cboRightCareRightPlace_SelectedIndexChanged);
            this.cboRightCareRightPlace.Validating += new System.ComponentModel.CancelEventHandler(this.cboRightCareRightPlace_Validating);
            // 
            // lblRightCareRightPlace
            // 
            this.lblRightCareRightPlace.BackColor = System.Drawing.Color.White;
            this.lblRightCareRightPlace.Location = new System.Drawing.Point(578, 250);
            this.lblRightCareRightPlace.Name = "lblRightCareRightPlace";
            this.lblRightCareRightPlace.Size = new System.Drawing.Size(181, 16);
            this.lblRightCareRightPlace.TabIndex = 0;
            this.lblRightCareRightPlace.Text = "Right Care Right Place:";
            // 
            // cboLeftOrStayed
            // 
            this.cboLeftOrStayed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLeftOrStayed.DropDownWidth = 3;
            this.cboLeftOrStayed.Location = new System.Drawing.Point(778, 272);
            this.cboLeftOrStayed.MaxLength = 3;
            this.cboLeftOrStayed.Name = "cboLeftOrStayed";
            this.cboLeftOrStayed.Size = new System.Drawing.Size(57, 21);
            this.cboLeftOrStayed.TabIndex = 40;
            this.cboLeftOrStayed.SelectedIndexChanged += new System.EventHandler(this.cboLeftOrStayed_SelectedIndexChanged);
            this.cboLeftOrStayed.Validating += new System.ComponentModel.CancelEventHandler(this.cboLeftOrStayed_Validating);
            // 
            // lblLeftOrStayed
            // 
            this.lblLeftOrStayed.BackColor = System.Drawing.Color.White;
            this.lblLeftOrStayed.Location = new System.Drawing.Point(578, 276);
            this.lblLeftOrStayed.Name = "lblLeftOrStayed";
            this.lblLeftOrStayed.Size = new System.Drawing.Size(181, 16);
            this.lblLeftOrStayed.TabIndex = 0;
            this.lblLeftOrStayed.Text = "Left or Stayed:";
            // 
            // cboLeftWithoutBeingSeen
            // 
            this.cboLeftWithoutBeingSeen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLeftWithoutBeingSeen.DropDownWidth = 3;
            this.cboLeftWithoutBeingSeen.Location = new System.Drawing.Point(778, 297);
            this.cboLeftWithoutBeingSeen.MaxLength = 3;
            this.cboLeftWithoutBeingSeen.Name = "cboLeftWithoutBeingSeen";
            this.cboLeftWithoutBeingSeen.Size = new System.Drawing.Size(47, 21);
            this.cboLeftWithoutBeingSeen.TabIndex = 41;
            this.cboLeftWithoutBeingSeen.SelectedIndexChanged += new System.EventHandler(this.cboLeftWithoutBeingSeen_SelectedIndexChanged);
            this.cboLeftWithoutBeingSeen.Validating += new System.ComponentModel.CancelEventHandler(this.cboLeftWithoutBeingSeen_validating);
            // 
            // LblLeftWithoutBeingSeen
            // 
            this.LblLeftWithoutBeingSeen.BackColor = System.Drawing.Color.White;
            this.LblLeftWithoutBeingSeen.Location = new System.Drawing.Point(578, 300);
            this.LblLeftWithoutBeingSeen.Name = "LblLeftWithoutBeingSeen";
            this.LblLeftWithoutBeingSeen.Size = new System.Drawing.Size(181, 16);
            this.LblLeftWithoutBeingSeen.TabIndex = 0;
            this.LblLeftWithoutBeingSeen.Text = "Left without being seen:";
            // 
            // cboLeftWithoutFinancialClearance
            // 
            this.cboLeftWithoutFinancialClearance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLeftWithoutFinancialClearance.DropDownWidth = 3;
            this.cboLeftWithoutFinancialClearance.Location = new System.Drawing.Point(778, 322);
            this.cboLeftWithoutFinancialClearance.MaxLength = 3;
            this.cboLeftWithoutFinancialClearance.Name = "cboLeftWithoutFinancialClearance";
            this.cboLeftWithoutFinancialClearance.Size = new System.Drawing.Size(47, 21);
            this.cboLeftWithoutFinancialClearance.TabIndex = 42;
            this.cboLeftWithoutFinancialClearance.SelectedIndexChanged += new System.EventHandler(this.cboLeftWithoutFinancialClearance_SelectedIndexChanged);
            this.cboLeftWithoutFinancialClearance.Validating += new System.ComponentModel.CancelEventHandler(this.cboLeftWithoutFinancialClearance_Validating);
            // 
            // lblLeftWithoutFinClearance
            // 
            this.lblLeftWithoutFinClearance.BackColor = System.Drawing.Color.White;
            this.lblLeftWithoutFinClearance.Location = new System.Drawing.Point(578, 322);
            this.lblLeftWithoutFinClearance.Name = "lblLeftWithoutFinClearance";
            this.lblLeftWithoutFinClearance.Size = new System.Drawing.Size(181, 16);
            this.lblLeftWithoutFinClearance.TabIndex = 0;
            this.lblLeftWithoutFinClearance.Text = "Left without financial clearance:";
            // 
            // btnViewClinicalTrialsDetails
            // 
            this.btnViewClinicalTrialsDetails.Location = new System.Drawing.Point(867, 220);
            this.btnViewClinicalTrialsDetails.Name = "btnViewClinicalTrialsDetails";
            this.btnViewClinicalTrialsDetails.Size = new System.Drawing.Size(75, 23);
            this.btnViewClinicalTrialsDetails.TabIndex = 38;
            this.btnViewClinicalTrialsDetails.Text = "View Details";
            this.btnViewClinicalTrialsDetails.UseVisualStyleBackColor = true;
            this.btnViewClinicalTrialsDetails.Visible = false;
            this.btnViewClinicalTrialsDetails.Click += new System.EventHandler(this.btnViewClinicalTrialsDetails_Click);
            // 
            // lblResistantOrganism
            // 
            this.lblResistantOrganism.Location = new System.Drawing.Point(575, 103);
            this.lblResistantOrganism.Name = "lblResistantOrganism";
            this.lblResistantOrganism.Size = new System.Drawing.Size(101, 21);
            this.lblResistantOrganism.TabIndex = 43;
            this.lblResistantOrganism.Text = "Resistant Organism:";
            // 
            // txtResistantOrganism
            // 
            this.txtResistantOrganism.AutoSize = true;
            this.txtResistantOrganism.Location = new System.Drawing.Point(674, 103);
            this.txtResistantOrganism.Name = "txtResistantOrganism";
            this.txtResistantOrganism.Size = new System.Drawing.Size(0, 13);
            this.txtResistantOrganism.TabIndex = 4;
            // 
            // ClinicalView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.txtResistantOrganism);
            this.Controls.Add(this.lblResistantOrganism);
            this.Controls.Add(this.btnViewClinicalTrialsDetails);
            this.Controls.Add(this.cboLeftWithoutFinancialClearance);
            this.Controls.Add(this.lblLeftWithoutFinClearance);
            this.Controls.Add(this.cboLeftWithoutBeingSeen);
            this.Controls.Add(this.LblLeftWithoutBeingSeen);
            this.Controls.Add(this.cboLeftOrStayed);
            this.Controls.Add(this.lblLeftOrStayed);
            this.Controls.Add(this.cboRightCareRightPlace);
            this.Controls.Add(this.lblRightCareRightPlace);
            this.Controls.Add(this.cboPatientInClinicalResearch);
            this.Controls.Add(this.lblPatientUnderResearchStudy);
            this.Controls.Add(this.mtbComments);
            this.Controls.Add(this.physicianSelectionView1);
            this.Controls.Add(this.cboSmoker);
            this.Controls.Add(this.mtbEmbosserCard);
            this.Controls.Add(this.lblEmbosserCard);
            this.Controls.Add(this.lblComments);
            this.Controls.Add(this.cboPregnant);
            this.Controls.Add(this.lblPregnant);
            this.Controls.Add(this.cboBloodless);
            this.Controls.Add(this.lblBloodless);
            this.Controls.Add(this.lblSmoker);
            this.Name = "ClinicalView";
            this.Size = new System.Drawing.Size(992, 332);
            this.Enter += new System.EventHandler(this.ClinicalView_Enter);
            this.Leave += new System.EventHandler(this.ClinicalView_Leave);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.ClinicalView_Validating);
            this.Disposed += new System.EventHandler(this.ClinicalView_Disposed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #endregion

        #region Construction and Finalization

        public ClinicalView()
        {
            InitializeComponent();

            ConfigureControls();

            this.ClinicalTrialsDetailsView = new ClinicalTrialsDetailsView();
        }

        #endregion

        #region Data Elements

        private PatientAccessComboBox cboBloodless;
        private PatientAccessComboBox cboLeftOrStayed;
        private PatientAccessComboBox cboLeftWithoutBeingSeen;
        private PatientAccessComboBox cboLeftWithoutFinancialClearance;
        private PatientAccessComboBox cboPatientInClinicalResearch;
        private PatientAccessComboBox cboPregnant;
        private PatientAccessComboBox cboRightCareRightPlace;
        private PatientAccessComboBox cboSmoker;
        private Label lblBloodless;
        private Label lblComments;
        private Label lblEmbosserCard;
        private Label lblLeftOrStayed;
        private Label LblLeftWithoutBeingSeen;
        private Label lblLeftWithoutFinClearance;
        private Label lblPatientUnderResearchStudy;
        private Label lblPregnant;
        private Label lblRightCareRightPlace;
        private Label lblSmoker;
        private MaskedEditTextBox mtbComments;
        private MaskedEditTextBox mtbEmbosserCard;
        private Button btnViewClinicalTrialsDetails;
        private PhysicianSelectionView physicianSelectionView1;
        private Label lblResistantOrganism;
        private Label txtResistantOrganism;
        private bool userChangedIsPatientInClinicalResearchStudy = false;

        #endregion


        #region Constants

        #endregion
    }
}
