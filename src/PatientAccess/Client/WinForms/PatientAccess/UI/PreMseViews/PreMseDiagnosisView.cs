using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.ContactViews;
using PatientAccess.UI.CptCodes.Presenters;
using PatientAccess.UI.CptCodes.ViewImpl;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;

namespace PatientAccess.UI.PreMSEViews
{
    /// <summary>
    /// Summary description for PreMseRegStep2View.
    /// </summary>
    [Serializable]
    public class PreMseDiagnosisView : ControlView, IAlternateCareFacilityView
    {
        #region Events

        public event EventHandler PhysicianSelectionLeaveEvent;

        #endregion

        #region Event Handlers

        /// <summary>
        /// physicianSelectionView_Leave - alert the parent to change button focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void physicianSelectionView_Leave( object sender, EventArgs e )
        {
            if ( PhysicianSelectionLeaveEvent != null )
            {
                AcceptButton = null;
                PhysicianSelectionLeaveEvent( this, null );
            }
        }

        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void PreMseDiagnosisView_Disposed( object sender, EventArgs e )
        {
            UnregisterEvents();
        }

        private void PreMseDiagnosisView_Leave( object sender, EventArgs e )
        {
            blnLeaveRun = true;
            RuleEngine.GetInstance().EvaluateRule( typeof( OnContactAndDiagnosisForm ), Model_Account );
            blnLeaveRun = false;
        }

        private void mtbChiefComplaint_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbChiefComplaint );
            var mtb = ( MaskedEditTextBox )sender;
            Model_Account.Diagnosis.ChiefComplaint = mtb.Text.Trim();
            RuleEngine.EvaluateRule( typeof( ChiefComplaintPreferred ), Model );
        }

        private void cmbModeOfArrival_SelectedIndexChanged( object sender, EventArgs e )
        {
            var selectedModeOfArrival = cmbModeOfArrival.SelectedItem as ModeOfArrival;
            if ( selectedModeOfArrival != null )
            {
                UpdateSelectedModeOfArrival( selectedModeOfArrival );
            }
        }

        /// <exception cref="ArgumentNullException"><c>modeOfArrival</c> is <see langword="null"/>.</exception>
        private void UpdateSelectedModeOfArrival( ModeOfArrival modeOfArrival )
        {
            Guard.ThrowIfArgumentIsNull( modeOfArrival, "modeOfArrival" );

            Model_Account.ModeOfArrival = modeOfArrival;
            RuleEngine.EvaluateRule( typeof( ModeOfArrivalPreferred ), Model );
            RuleEngine.EvaluateRule( typeof( ModeOfArrivalRequired ), Model );
        }

        private void cmbHospitalService_SelectedIndexChanged( object sender, EventArgs e )
        {
            var cb = sender as ComboBox;
            if ( cb != null && cb.SelectedIndex != -1 )
            {
                Model_Account.HospitalService = ( HospitalService )( ( HospitalService )cb.SelectedItem ).Clone();
            }
        }

        private void cmbAdmitSource_SelectedIndexChanged( object sender, EventArgs e )
        {
            var cb = sender as ComboBox;
            if ( cb != null && cb.SelectedIndex != -1 )
            {
                HandleAdmitSourceSelectedIndexChanged( cb.SelectedItem as AdmitSource );
            }
        }

        private void HandleAdmitSourceSelectedIndexChanged( AdmitSource newAdmitSource )
        {
            if ( newAdmitSource != null )
            {
                Model.AdmitSource = newAdmitSource;
            }

            RuleEngine.EvaluateRule( typeof( AdmitSourceRequired ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitSourcePreferred ), Model );
            
            AlternateCareFacilityPresenter.HandleAlternateCareFacility();
        }

        private void cmbHospitalClinic_SelectedIndexChanged( object sender, EventArgs e )
        {
            var cb = sender as ComboBox;
            if ( cb != null && cb.SelectedIndex != -1 )
            {
                Model_Account.HospitalClinic = ( HospitalClinic )( ( HospitalClinic )cb.SelectedItem ).Clone();
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cmbAlternateCareFacility control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmbAlternateCareFacility_SelectedIndexChanged( object sender, EventArgs e )
        {
            var selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if ( selectedAlternateCare != null )
            {
                AlternateCareFacilityPresenter.UpdateAlternateCareFacility( selectedAlternateCare );
            }
        }

        private void cmbAlternateCareFacility_Validating( object sender, CancelEventArgs e )
        {
            var selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if ( selectedAlternateCare != null )
            {
                AlternateCareFacilityPresenter.UpdateAlternateCareFacility( selectedAlternateCare );
            }
            UIColors.SetNormalBgColor( cmbAlternateCareFacility );
            AlternateCareFacilityPresenter.EvaluateAlternateCareFacilityRule();
        }

        #endregion

        #region Rule Event Handlers

        private void HospitalServiceCodeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbHospitalService );
        }

        private void AdmitSourceRequiredRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbAdmitSource );
        }

        private void AlternateCareFacilityRequiredEventHandler( object sender, EventArgs e )
        {
            cmbAlternateCareFacility.Enabled = true;
            UIColors.SetRequiredBgColor( cmbAlternateCareFacility );
        }

        private void DiagnosisClinicOneRequiredRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbHospitalClinic );
        }

        private void AdmitSourcePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbAdmitSource );
        }

        private void ChiefComplaintPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbChiefComplaint );
        }

        private void ModeOfArrivalPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbModeOfArrival );
        }
        private void ModeOfArrivalRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbModeOfArrival );
        }

        //---------------------Evaluate ComboBoxes -----------------------------------
        private void emergencyContactView_EmergencyRelationshipValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( emergencyContactView.RelationshipView.ComboBox );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmergContact_1Rel ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmergContact_1RelChange ), Model );
                emergencyContactView.RunRules();
            }
        }
        private void cmbModeOfArrival_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cmbModeOfArrival );
                var selectedModeOfArrival = cmbModeOfArrival.SelectedItem as ModeOfArrival;
                if ( selectedModeOfArrival != null )
                {
                    UpdateSelectedModeOfArrival( selectedModeOfArrival );
                }
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidModeOfArrival ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidModeOfArrivalChange ), Model );
                Refresh();
            }
        }
        private void cmbHospitalService_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cmbHospitalService );
                Refresh();
                RuleEngine.EvaluateRule( typeof( HospitalServiceRequired ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidHospitalServiceCode ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidHospitalServiceCodeChange ), Model );
            }
        }
        private void cmbAdmitSource_Validating( object sender, CancelEventArgs e )
        {
            HandleAdmitSourceSelectedIndexChanged( cmbAdmitSource.SelectedItem as AdmitSource );

            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cmbAdmitSource );
                Refresh();

                RuleEngine.EvaluateRule( typeof( AdmitSourceRequired ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitSourcePreferred ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidAdmitSourceCode ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidAdmitSourceCodeChange ), Model );
            }
        }
        private void cmbHospitalClinic_Validating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cmbHospitalClinic );
                Refresh();
                RuleEngine.EvaluateRule( typeof( DiagnosisClinicOneRequired ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidClinicForPreMSE ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidClinicForPreMSEChange ), Model );
            }
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if ( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }

        private void InvalidEmergContact1RelChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( emergencyContactView.RelationshipView.ComboBox );
        }
        private void InvalidModeOfArrivalChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbModeOfArrival );
        }
        private void InvalidHospitalServiceCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbHospitalService );
        }
        private void InvalidAdmitSourceCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbAdmitSource );
        }
        private void InvalidClinicForPreMseChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbHospitalClinic );
        }

        //----------------------------------------------------------------------

        private void InvalidEmergContact1RelEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( emergencyContactView.RelationshipView.ComboBox );
        }
        private void InvalidModeOfArrivalEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbModeOfArrival );
        }
        private void InvalidHospitalServiceCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbHospitalService );
        }
        private void InvalidAdmitSourceCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbAdmitSource );
        }
        private void InvalidClinicForPreMseEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbHospitalClinic );
        }

        //----------------------------------------------------------------------

        #endregion

        #region Methods

        /// <summary>
        /// runRules - determine if the user has entered all required fields
        /// Some are conditional based on other fields.
        /// </summary>
        private void RunRules()
        {
            UIColors.SetNormalBgColor( cmbHospitalService );
            UIColors.SetNormalBgColor( cmbAdmitSource );
            UIColors.SetNormalBgColor( cmbModeOfArrival );
            UIColors.SetNormalBgColor( cmbHospitalClinic );
            UIColors.SetNormalBgColor( cmbAdmitSource );
            UIColors.SetNormalBgColor( mtbChiefComplaint );

            Refresh();

            RuleEngine.EvaluateRule( typeof( AdmitSourcePreferred ), Model );
            RuleEngine.EvaluateRule( typeof( ModeOfArrivalPreferred ), Model );
            RuleEngine.EvaluateRule( typeof( ModeOfArrivalRequired ), Model );
            RuleEngine.EvaluateRule( typeof( ChiefComplaintPreferred ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( OnContactAndDiagnosisForm ), Model );

            emergencyContactView.RunRules();
        }

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            AlternateCareFacilityPresenter = new AlternateCareFacilityPresenter( this, new AlternateCareFacilityFeatureManager() );

            CptCodesPresenter = new CptCodesPresenter( cptCodesView1, Model, new CptCodesFeatureManager(), new MessageBoxAdapter() );

            
            if ( loadingModelData )
            {
                loadingModelData = false;
                PopulateModeOfArrivalCodes();
                PopulateClinicCodes();
                PopulateHsvCodes();
                PopulateAdmitSources();

                emergencyContactView.Model = Model_Account;
                if ( Model_Account.EmergencyContact1 != null )
                {
                    emergencyContactView.Model_EmergencyContact = Model_Account.EmergencyContact1;
                    emergencyContactView.AddressView.Context = "EmergencyContact1";
                    emergencyContactView.UpdateView();
                }

                mtbChiefComplaint.Text = Model_Account.Diagnosis.ChiefComplaint.Trim();
                if ( Model_Account.ModeOfArrival != null )
                {
                    cmbModeOfArrival.SelectedItem = Model_Account.ModeOfArrival;
                }
                if ( Model_Account.HospitalService != null )
                {
                    cmbHospitalService.SelectedItem = Model_Account.HospitalService;
                }
                if ( Model_Account.HospitalClinic != null )
                {
                    cmbHospitalClinic.SelectedItem = Model_Account.HospitalClinic;
                }
                if ( Model_Account.AdmitSource != null )
                {
                    cmbAdmitSource.SelectedItem = Model_Account.AdmitSource;
                }
                RegisterEvents();
            }
            physicianSelectionView.Model = Model_Account;
            physicianSelectionView.UpdateView();
            CptCodesPresenter.UpdateView();
            RunRules();
        }

        public void PopulateAlternateCareFacility()
        {
            var alternateCareFacilityBroker = BrokerFactory.BrokerOfType<IAlternateCareFacilityBroker>();
            var allAlternateCareFacilities =
                alternateCareFacilityBroker.AllAlternateCareFacilities( User.GetCurrent().Facility.Oid );

            cmbAlternateCareFacility.Items.Clear();

            foreach ( var alternateCareFacility in allAlternateCareFacilities )
            {
                cmbAlternateCareFacility.Items.Add( alternateCareFacility );
            }

            // If the value is not in the list, add it as a valid choice. This will
            // prevent data loss in the event that the value stored with the account
            // is removed from the lookup table
            if ( !cmbAlternateCareFacility.Items.Contains( Model_Account.AlternateCareFacility ) )
            {
                cmbAlternateCareFacility.Items.Add( Model_Account.AlternateCareFacility );
            }

            cmbAlternateCareFacility.SelectedItem = Model_Account.AlternateCareFacility;
        }

        public void ClearAlternateCareFacility()
        {
            if ( cmbAlternateCareFacility.Items.Count > 0 )
            {
                cmbAlternateCareFacility.SelectedIndex = 0;
            }
        }

        public void ShowAlternateCareFacilityDisabled()
        {
            UIColors.SetDisabledDarkBgColor( cmbAlternateCareFacility );
            cmbAlternateCareFacility.Enabled = false;
        }

        public void ShowAlternateCareFacilityEnabled()
        {
            UIColors.SetNormalBgColor( cmbAlternateCareFacility );
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

        #region Properties

        private Account Model_Account
        {
            get
            {
                return Model;
            }
        }

        public new Account Model
        {
            get
            {
                return ( Account )base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        private RuleEngine RuleEngine
        {
            get { return i_RuleEngine ?? (i_RuleEngine = RuleEngine.GetInstance()); }
        }

        private IAlternateCareFacilityPresenter AlternateCareFacilityPresenter
        {
            get
            {
                return alternateCareFacilityPresenter;
            }

            set
            {
                alternateCareFacilityPresenter = value;
            }
        }

        private CptCodesPresenter CptCodesPresenter { get; set; }

        #endregion

        #region Private Methods
        private void PopulateAdmitSources()
        {
            var brokerProxy = new AdmitSourceBrokerProxy();
            var admitSourceTable = ( ArrayList )brokerProxy.AdmitSourcesForNotNewBorn( User.GetCurrent().Facility.Oid );

            foreach ( AdmitSource src in admitSourceTable )
            {
                cmbAdmitSource.Items.Add( src );
            }
        }

        private void PopulateClinicCodes()
        {
            IHospitalClinicsBroker broker = new HospitalClinicsBrokerProxy();
            var clinicTable = ( ArrayList )broker.HospitalClinicsFor( Model_Account.Facility.Oid );

            foreach ( HospitalClinic hsv in clinicTable )
            {
                cmbHospitalClinic.Items.Add( hsv );
            }
        }

        private void PopulateHsvCodes()
        {
            if (Model_Account.IsUrgentCarePreMse )
            {
                var hospitalServiceCode = HospitalService.URGENT_CARE_HSV;
                cmbHospitalService.Items.Add(hospitalServiceCode);
                cmbHospitalService.SelectedIndex = 0;
                cmbHospitalService.Enabled = false;
            }
            else
            {
                var brokerProxy = new HSVBrokerProxy();
                var hospitalServiceCodes = (ArrayList) brokerProxy.HospitalServicesFor(
                    User.GetCurrent().Facility.Oid,
                    Model_Account.KindOfVisit.Code);

                if (hospitalServiceCodes != null && hospitalServiceCodes.Count > 0)
                {
                    foreach (HospitalService hsv in hospitalServiceCodes)
                    {
                        cmbHospitalService.Items.Add(hsv);
                    }
                    cmbHospitalService.Sorted = true;
                }
            }
        }

        private void PopulateModeOfArrivalCodes()
        {
            IModeOfArrivalBroker broker = new ModeOfArrivalBrokerProxy();
            ArrayList arrivalModes = broker.ModesOfArrivalFor( Model_Account.Facility.Oid );

            if ( arrivalModes.Count == 0 )
            {
                lblStaticArrival.Hide();
                cmbModeOfArrival.Hide();
                return;
            }

            foreach ( ModeOfArrival mode in arrivalModes )
            {
                cmbModeOfArrival.Items.Add( mode );
            }
        }

        private void RegisterEvents()
        {
            if ( eventsRegistered )
            {
                return;
            }

            eventsRegistered = true;

            RuleEngine.LoadRules( Model_Account );

            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceRequired ), Model, HospitalServiceCodeRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitSourceRequired ), Model, AdmitSourceRequiredRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( AlternateCareFacilityRequired ), AlternateCareFacilityRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( DiagnosisClinicOneRequired ), Model, DiagnosisClinicOneRequiredRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitSourcePreferred ), Model, AdmitSourcePreferredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( ChiefComplaintPreferred ), Model, ChiefComplaintPreferredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( ModeOfArrivalPreferred ), Model, ModeOfArrivalPreferredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( ModeOfArrivalRequired ), Model, ModeOfArrivalRequiredEventHandler );

            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmergContact_1Rel ), Model, InvalidEmergContact1RelEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmergContact_1RelChange ), Model, InvalidEmergContact1RelChangeEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidModeOfArrival ), Model, InvalidModeOfArrivalEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidModeOfArrivalChange ), Model, InvalidModeOfArrivalChangeEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidHospitalServiceCode ), Model, InvalidHospitalServiceCodeEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidHospitalServiceCodeChange ), Model, InvalidHospitalServiceCodeChangeEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidAdmitSourceCode ), Model, InvalidAdmitSourceCodeEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidAdmitSourceCodeChange ), Model, InvalidAdmitSourceCodeChangeEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidClinicForPreMSE ), Model, InvalidClinicForPreMseEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidClinicForPreMSEChange ), Model, InvalidClinicForPreMseChangeEventHandler );

            emergencyContactView.RegisterEvents();
        }

        private void UnregisterEvents()
        {
            eventsRegistered = false;

            RuleEngine.GetInstance().UnregisterEvent( typeof( HospitalServiceRequired ), Model, HospitalServiceCodeRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitSourceRequired ), Model, AdmitSourceRequiredRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AlternateCareFacilityRequired ), Model, AlternateCareFacilityRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( DiagnosisClinicOneRequired ), Model, DiagnosisClinicOneRequiredRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitSourcePreferred ), Model, AdmitSourcePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ChiefComplaintPreferred ), Model, ChiefComplaintPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ModeOfArrivalPreferred ), Model, ModeOfArrivalPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ModeOfArrivalRequired ), Model, ModeOfArrivalRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmergContact_1Rel ), Model, InvalidEmergContact1RelEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmergContact_1RelChange ), Model, InvalidEmergContact1RelChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidModeOfArrival ), Model, InvalidModeOfArrivalEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidModeOfArrivalChange ), Model, InvalidModeOfArrivalChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidHospitalServiceCode ), Model, InvalidHospitalServiceCodeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidHospitalServiceCodeChange ), Model, InvalidHospitalServiceCodeChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidAdmitSourceCode ), Model, InvalidAdmitSourceCodeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidAdmitSourceCodeChange ), Model, InvalidAdmitSourceCodeChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidClinicForPreMSE ), Model, InvalidClinicForPreMseEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidClinicForPreMSEChange ), Model, InvalidClinicForPreMseChangeEventHandler );

            emergencyContactView.UnregisterEvents();
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureChiefComplaint( mtbChiefComplaint );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreMseDiagnosisView));
            this.emergencyContactView = new PatientAccess.UI.ContactViews.EmergencyContactView();
            this.lblStaticComplaint = new System.Windows.Forms.Label();
            this.lblStaticArrival = new System.Windows.Forms.Label();
            this.cmbModeOfArrival = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblHospitalService = new System.Windows.Forms.Label();
            this.cmbHospitalService = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblAdmitSource = new System.Windows.Forms.Label();
            this.cmbAdmitSource = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblClinic = new System.Windows.Forms.Label();
            this.cmbHospitalClinic = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.physicianSelectionView = new PatientAccess.UI.CommonControls.PhysicianSelectionView();
            this.mtbChiefComplaint = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbAlternateCareFacility = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblAlternateCareFacility = new System.Windows.Forms.Label();
            this.cptCodesView1 = new PatientAccess.UI.CptCodes.ViewImpl.CptCodesView();
            this.SuspendLayout();
            // 
            // emergencyContactView
            // 
            this.emergencyContactView.GroupBoxText = "Emergency Contact 1";
            this.emergencyContactView.Location = new System.Drawing.Point(8, 8);
            this.emergencyContactView.Model = null;
            this.emergencyContactView.Name = "emergencyContactView";
            this.emergencyContactView.Size = new System.Drawing.Size(372, 310);
            this.emergencyContactView.TabIndex = 1;
            this.emergencyContactView.EmergencyRelationshipValidating += new System.ComponentModel.CancelEventHandler(this.emergencyContactView_EmergencyRelationshipValidating);
            // 
            // lblStaticComplaint
            // 
            this.lblStaticComplaint.Location = new System.Drawing.Point(408, 8);
            this.lblStaticComplaint.Name = "lblStaticComplaint";
            this.lblStaticComplaint.Size = new System.Drawing.Size(100, 23);
            this.lblStaticComplaint.TabIndex = 0;
            this.lblStaticComplaint.Text = "Chief complaint:";
            // 
            // lblStaticArrival
            // 
            this.lblStaticArrival.Location = new System.Drawing.Point(684, 57);
            this.lblStaticArrival.Name = "lblStaticArrival";
            this.lblStaticArrival.Size = new System.Drawing.Size(83, 30);
            this.lblStaticArrival.TabIndex = 0;
            this.lblStaticArrival.Text = "Mode of arrival:";
            // 
            // cmbModeOfArrival
            // 
            this.cmbModeOfArrival.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModeOfArrival.Location = new System.Drawing.Point(774, 54);
            this.cmbModeOfArrival.Name = "cmbModeOfArrival";
            this.cmbModeOfArrival.Size = new System.Drawing.Size(180, 28);
            this.cmbModeOfArrival.TabIndex = 4;
            this.cmbModeOfArrival.SelectedIndexChanged += new System.EventHandler(this.cmbModeOfArrival_SelectedIndexChanged);
            this.cmbModeOfArrival.Validating += new System.ComponentModel.CancelEventHandler(this.cmbModeOfArrival_Validating);
            // 
            // lblHospitalService
            // 
            this.lblHospitalService.Location = new System.Drawing.Point(684, 27);
            this.lblHospitalService.Name = "lblHospitalService";
            this.lblHospitalService.Size = new System.Drawing.Size(87, 30);
            this.lblHospitalService.TabIndex = 0;
            this.lblHospitalService.Text = "Hospital service:";
            // 
            // cmbHospitalService
            // 
            this.cmbHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHospitalService.Location = new System.Drawing.Point(774, 24);
            this.cmbHospitalService.MaxLength = 23;
            this.cmbHospitalService.Name = "cmbHospitalService";
            this.cmbHospitalService.Size = new System.Drawing.Size(180, 28);
            this.cmbHospitalService.TabIndex = 3;
            this.cmbHospitalService.SelectedIndexChanged += new System.EventHandler(this.cmbHospitalService_SelectedIndexChanged);
            this.cmbHospitalService.Validating += new System.ComponentModel.CancelEventHandler(this.cmbHospitalService_Validating);
            // 
            // lblAdmitSource
            // 
            this.lblAdmitSource.Location = new System.Drawing.Point(410, 388);
            this.lblAdmitSource.Name = "lblAdmitSource";
            this.lblAdmitSource.Size = new System.Drawing.Size(83, 30);
            this.lblAdmitSource.TabIndex = 0;
            this.lblAdmitSource.Text = "Admit source:";
            // 
            // cmbAdmitSource
            // 
            this.cmbAdmitSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdmitSource.Location = new System.Drawing.Point(498, 385);
            this.cmbAdmitSource.MaxLength = 23;
            this.cmbAdmitSource.Name = "cmbAdmitSource";
            this.cmbAdmitSource.Size = new System.Drawing.Size(192, 28);
            this.cmbAdmitSource.TabIndex = 6;
            this.cmbAdmitSource.SelectedIndexChanged += new System.EventHandler(this.cmbAdmitSource_SelectedIndexChanged);
            this.cmbAdmitSource.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAdmitSource_Validating);
            // 
            // lblClinic
            // 
            this.lblClinic.Location = new System.Drawing.Point(410, 456);
            this.lblClinic.Name = "lblClinic";
            this.lblClinic.Size = new System.Drawing.Size(83, 18);
            this.lblClinic.TabIndex = 0;
            this.lblClinic.Text = "Clinic 1:";
            // 
            // cmbHospitalClinic
            // 
            this.cmbHospitalClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHospitalClinic.Location = new System.Drawing.Point(498, 453);
            this.cmbHospitalClinic.MaxLength = 23;
            this.cmbHospitalClinic.Name = "cmbHospitalClinic";
            this.cmbHospitalClinic.Size = new System.Drawing.Size(180, 28);
            this.cmbHospitalClinic.TabIndex = 8;
            this.cmbHospitalClinic.SelectedIndexChanged += new System.EventHandler(this.cmbHospitalClinic_SelectedIndexChanged);
            this.cmbHospitalClinic.Validating += new System.ComponentModel.CancelEventHandler(this.cmbHospitalClinic_Validating);
            // 
            // physicianSelectionView
            // 
            this.physicianSelectionView.BackColor = System.Drawing.Color.White;
            this.physicianSelectionView.Location = new System.Drawing.Point(408, 100);
            this.physicianSelectionView.Model = null;
            this.physicianSelectionView.Name = "physicianSelectionView";
            this.physicianSelectionView.Size = new System.Drawing.Size(558, 273);
            this.physicianSelectionView.TabIndex = 5;
            this.physicianSelectionView.Leave += new System.EventHandler(this.physicianSelectionView_Leave);
            // 
            // mtbChiefComplaint
            // 
            this.mtbChiefComplaint.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbChiefComplaint.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbChiefComplaint.Location = new System.Drawing.Point(408, 24);
            this.mtbChiefComplaint.Mask = "";
            this.mtbChiefComplaint.MaxLength = 74;
            this.mtbChiefComplaint.Multiline = true;
            this.mtbChiefComplaint.Name = "mtbChiefComplaint";
            this.mtbChiefComplaint.Size = new System.Drawing.Size(269, 48);
            this.mtbChiefComplaint.TabIndex = 2;
            this.mtbChiefComplaint.Validating += new System.ComponentModel.CancelEventHandler(this.mtbChiefComplaint_Validating);
            // 
            // cmbAlternateCareFacility
            // 
            this.cmbAlternateCareFacility.DisplayMember = "Description";
            this.cmbAlternateCareFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlternateCareFacility.Location = new System.Drawing.Point(498, 418);
            this.cmbAlternateCareFacility.MaxLength = 27;
            this.cmbAlternateCareFacility.Name = "cmbAlternateCareFacility";
            this.cmbAlternateCareFacility.Size = new System.Drawing.Size(192, 28);
            this.cmbAlternateCareFacility.TabIndex = 7;
            this.cmbAlternateCareFacility.SelectedIndexChanged += new System.EventHandler(this.cmbAlternateCareFacility_SelectedIndexChanged);
            this.cmbAlternateCareFacility.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAlternateCareFacility_Validating);
            // 
            // lblAlternateCareFacility
            // 
            this.lblAlternateCareFacility.Location = new System.Drawing.Point(410, 418);
            this.lblAlternateCareFacility.Name = "lblAlternateCareFacility";
            this.lblAlternateCareFacility.Size = new System.Drawing.Size(82, 27);
            this.lblAlternateCareFacility.TabIndex = 0;
            this.lblAlternateCareFacility.Text = "Nursing home/ Alt care facility";
            // 
            // cptCodesView1
            // 
            this.cptCodesView1.CptCodesPresenter = null;
            this.cptCodesView1.Location = new System.Drawing.Point(705, 385);
            this.cptCodesView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cptCodesView1.Model = null;
            this.cptCodesView1.Name = "cptCodesView1";
            this.cptCodesView1.Size = new System.Drawing.Size(261, 25);
            this.cptCodesView1.TabIndex = 9;
            // 
            // PreMseDiagnosisView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cptCodesView1);
            this.Controls.Add(this.cmbAlternateCareFacility);
            this.Controls.Add(this.lblAlternateCareFacility);
            this.Controls.Add(this.mtbChiefComplaint);
            this.Controls.Add(this.physicianSelectionView);
            this.Controls.Add(this.cmbModeOfArrival);
            this.Controls.Add(this.cmbHospitalClinic);
            this.Controls.Add(this.cmbAdmitSource);
            this.Controls.Add(this.cmbHospitalService);
            this.Controls.Add(this.emergencyContactView);
            this.Controls.Add(this.lblStaticComplaint);
            this.Controls.Add(this.lblStaticArrival);
            this.Controls.Add(this.lblHospitalService);
            this.Controls.Add(this.lblAdmitSource);
            this.Controls.Add(this.lblClinic); 
            this.Name = "PreMseDiagnosisView";
            this.Size = new System.Drawing.Size(1000, 498);
            this.Leave += new System.EventHandler(this.PreMseDiagnosisView_Leave);
            this.Disposed += new System.EventHandler(this.PreMseDiagnosisView_Disposed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PreMseDiagnosisView()
        {
            loadingModelData = true;

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            emergencyContactView.AddressView.EditAddressButtonText = "Edit &Address..";
            ConfigureControls();

            EnableThemesOn( this );
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
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private PatientAccessComboBox cmbHospitalService;
        private PatientAccessComboBox cmbAdmitSource;
        private PatientAccessComboBox cmbHospitalClinic;
        private PatientAccessComboBox cmbModeOfArrival;
        private PatientAccessComboBox cmbAlternateCareFacility;

        private Label lblAlternateCareFacility;
        private Label lblAdmitSource;
        private Label lblHospitalService;
        private Label lblClinic;
        private Label lblStaticComplaint;
        private Label lblStaticArrival;

        private EmergencyContactView emergencyContactView;
        private PhysicianSelectionView physicianSelectionView;

        private bool eventsRegistered;
        private bool loadingModelData;
        private RuleEngine i_RuleEngine;
        private bool blnLeaveRun;
        private MaskedEditTextBox mtbChiefComplaint;

        private IAlternateCareFacilityPresenter alternateCareFacilityPresenter;
        private CptCodesView cptCodesView1;

        #endregion

        #region Constants

        

        #endregion

    }
}
