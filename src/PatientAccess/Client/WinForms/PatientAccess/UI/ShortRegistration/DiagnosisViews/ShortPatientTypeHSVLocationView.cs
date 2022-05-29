using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.ShortRegistration.DiagnosisViews
{
    public partial class ShortPatientTypeHSVLocationView : ControlView
    {
        #region Constructor
        public ShortPatientTypeHSVLocationView()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        public event EventHandler ClearSelectedClinics;
        public event EventHandler SelectPreviouslyStoredClinicValue;
        public event EventHandler<VisitTypeEventArgs> PatientTypeChanged;
        public event EventHandler<EventArgs> HSVChanged;
        #endregion

        #region public methods

        public override void UpdateView()
        {
            try
            {
                UpdateHospitalService();
                SetKindOfVisit();
                registerEvents();
                IsReregisterFacility();
                PopulatePatientTypes();
                SetNormalColor();
                cbxReregister.Visible = SetReregister();
                isLoadData = false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Event Handlers

        private void PatientTypeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbPatientType );
        }

        private void HospitalServiceRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbHospitalService );
        }

        private void PatientTypeHSVLocationView_Disposed( object sender, EventArgs e )
        {
            unregisterEvents();
        }

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

        private void InvalidHospitalServiceCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbHospitalService );
        }

        private void InvalidPatientTypeCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbPatientType );
        }

        private void InvalidHospitalServiceCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbHospitalService );
        }

        private void InvalidPatientTypeCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbPatientType );
        }

        /// <summary>
        /// Combo Box Patient Type Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPatientType_SelectedIndexChanged( object sender, EventArgs e )
        {
            cmbHospitalService.ResetText();
            cmbHospitalService.Items.Clear();

            savedPatientType = Model.KindOfVisit;
            selectedPatientType = cmbPatientType.SelectedItem as VisitType;
            if ( selectedPatientType != null )
            {
                Model.KindOfVisit = selectedPatientType;

                if ( ( Model.IsNew ) ||
                      ( ( Model.Activity != null ) &&
                        ( Model.Activity.AssociatedActivityType != null ) &&
                        ( Model.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) )
                       )
                    )
                {
                    Model.HospitalService = new HospitalService();
                }
            }

            PopulateHsvCodes();

            if ( Model.Activity is ShortRegistrationActivity )
            {
                cmbHospitalService.Enabled = ( Model.KindOfVisit != null && Model.KindOfVisit.Code != String.Empty );
            }

            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidPatientTypeCode ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( PatientTypeRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( DiagnosisClinicOneRequired ), Model );

            if ( Model.Activity is ShortPreRegistrationActivity ||
                Model.Activity is ShortRegistrationActivity ||
                Model.Activity is ShortMaintenanceActivity )
            {
                if ( ClearSelectedClinics != null )
                {
                    ClearSelectedClinics( this, new LooseArgs( Model.KindOfVisit ) );
                }
                if ( ( Model.Activity is ShortRegistrationActivity && Model.KindOfVisit == null ||
                       ( Model.KindOfVisit != null && Model.KindOfVisit.Code != VisitType.INPATIENT ) ) ||
                       ( Model.Activity is ShortMaintenanceActivity && Model.KindOfVisit == null || 
                         ( Model.KindOfVisit != null && Model.KindOfVisit.Code != VisitType.INPATIENT ) )
                   )
                {
                    if ( SelectPreviouslyStoredClinicValue != null )
                    {
                        SelectPreviouslyStoredClinicValue( this, new EventArgs() );
                    }
                }
            }

            FirePatientTypeChanged( Model.KindOfVisit );

            if ( Model.Facility.Reregister.Code == YesNoFlag.CODE_YES
                && Model.KindOfVisit != null && Model.KindOfVisit.Code == VisitType.RECURRING_PATIENT )
            {
                cbxReregister.Enabled = true;

                if ( isLoadData )    // if loading model data from UpdateView for the first time
                {
                    cbxReregister.Checked = ( Model.Reregister.Code != YesNoFlag.CODE_NO );
                }
                else
                {
                    cbxReregister.Checked = true;
                }
            }
            else
            {
                cbxReregister.Enabled = false;
                cbxReregister.Checked = false;
            }

            if ( Model.HospitalService == null )
            {
                Model.HospitalService = new HospitalService();
            }

            RuleEngine.GetInstance().RegisterEvent( typeof( PatientTypeRequired ), new EventHandler( PatientTypeRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceRequired ), new EventHandler( HospitalServiceRequiredEventHandler ) );
        }

        private void FirePatientTypeChanged( VisitType visitType )
        {
            if ( PatientTypeChanged != null )
            {
                PatientTypeChanged( this, new VisitTypeEventArgs( visitType ) );
            }
        }

        private void FireHSVChanged()
        {
            if (this.HSVChanged != null)
            {
                this.HSVChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Patient Type Validatinng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPatientType_Validated( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( cmbPatientType );
            UIColors.SetNormalBgColor( cmbHospitalService );
            Model.KindOfVisit = ( VisitType )cmbPatientType.SelectedItem;
            if ( ClearSelectedClinics != null )
            {
                ClearSelectedClinics( this, new LooseArgs( Model.KindOfVisit ) );
            }

            if ( !blnLeaveRun )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidPatientTypeCode ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidPatientTypeCodeChange ), Model );
            }

            RuleEngine.GetInstance().EvaluateRule( typeof( PatientTypeRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( DiagnosisClinicOneRequired ), Model );

            Model.HospitalService = ( HospitalService )cmbHospitalService.SelectedItem ?? new HospitalService();
        }

        private void cbxReregister_CheckedChanged( object sender, EventArgs e )
        {
            YesNoFlag yesNoFlag = new YesNoFlag( "N" );
            yesNoFlag.SetNo();
            if ( cbxReregister.Checked )
            {
                yesNoFlag.SetYes();
            }
            Model.Reregister = yesNoFlag;
        }

        #endregion

        #region Private Methods

        private void SetKindOfVisit()
        {
            if ( Model.Activity != null &&
                Model.Activity.AssociatedActivityType != null &&
                Model.Activity.AssociatedActivityType.GetType().Equals( typeof( ActivatePreRegistrationActivity ) ) &&
                Model.KindOfVisit != null &&
                Model.KindOfVisit.Code == VisitType.PREREG_PATIENT )
            {
                Model.KindOfVisit = ( VisitType )PatientTypes[0];
            }
        }

        private bool SetReregister()
        {
            bool isVisible = true;
            if ( Model.Activity != null
                && Model.Activity.AssociatedActivityType != null
                && Model.Activity.AssociatedActivityType.ToString().Contains( "ActivatePreRegistrationActivity" ) )
            {
                isVisible = false;
            }
            return isVisible;
        }

        private void UpdateHospitalService()
        {
            if ( Model.AccountNumber != 0 )
            {
                if ( Model.HospitalService != null
                    && Model.HospitalService.Code != HospitalService.BLANK_CODE )
                {
                    savedHospitalService = Model.HospitalService;
                }
            }
        }

        private void IsReregisterFacility()
        {
            if ( Model.KindOfVisit == null
                || Model.KindOfVisit.Code != VisitType.RECURRING_PATIENT )
            {
                return;
            }

            if ( Model.Facility.Reregister.Code == YesNoFlag.CODE_NO &&
                Model.Reregister != null &&
                Model.Reregister.Code == YesNoFlag.CODE_BLANK )
            {
                cbxReregister.Visible = false;
            }
            else
            {
                SetReregisterStatus( true );

                if ( Model.Facility.Reregister.Code != YesNoFlag.CODE_YES )
                {
                    cbxReregister.Enabled = false;
                }
            }
        }

        private void SetReregisterStatus( bool isVisible )
        {
            cbxReregister.Visible = isVisible;

            if ( Model.Reregister != null )
            {
                cbxReregister.Checked = ( Model.Reregister.Code != YesNoFlag.CODE_NO );
            }
        }

        private void cmbHospitalService_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( cmbHospitalService );
            Model.HospitalService = ( HospitalService )cmbHospitalService.SelectedItem;

            if ( !blnLeaveRun )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidHospitalServiceCode ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidHospitalServiceCodeChange ), Model );
            }
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( PatientTypeRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( DiagnosisClinicOneRequired ), Model );
        }

        /// <summary>
        /// Combo Box Hospital Service Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbHospitalService_SelectedIndexChanged( object sender, EventArgs e )
        {
            HospitalService hospitalSrvc = cmbHospitalService.SelectedItem as HospitalService;

            // don't reset if the selected index changed event was fired on assignment of the model value
            if ( ( cmbHospitalService.SelectedIndex > 0 && Model.HospitalService != null &&
                   ( ( HospitalService )cmbHospitalService.SelectedItem ).Code != Model.HospitalService.Code )
                ||
                 ( cmbHospitalService.SelectedIndex < 1 && Model.HospitalService != null &&
                   Model.HospitalService.Code != string.Empty )
               )
            {
                if ( hospitalSrvc != null && Model.KindOfVisit != null )
                {
                    Model.HospitalService = hospitalSrvc;
                }
            }
            else
            {
                if ( facilityHospitalServices != null && facilityHospitalServices.Count > 0 )
                {
                    if ( hospitalSrvc != null && Model.KindOfVisit != null )
                    {
                        Model.HospitalService = hospitalSrvc;
                    }
                }
            }
            FireHSVChanged();
        }

        /// <summary>
        ///  PopulatePatientTypes - from the list of all patient types, derive those applicable based on
        /// the activity
        /// </summary>
        private void PopulatePatientTypes()
        {
            if ( cmbPatientType.Items.Count == 0 )
            {
                cmbPatientType.Items.Clear();

                foreach ( VisitType patType in PatientTypes )
                {
                    cmbPatientType.Items.Add( patType );
                }

                // if there is only 1 PT in the list (other than the blank entry), default it
                if ( cmbPatientType.Items.Count == 1 )
                {
                    cmbPatientType.SelectedIndex = 0;
                }

                if ( Model.KindOfVisit != null )
                {
                    cmbPatientType.Enabled = Model.IsPatientTypeChangeable();
                    cmbPatientType.SelectedItem = Model.KindOfVisit;
                }
            }
        }

        private void SetNormalColor()
        {
            UIColors.SetNormalBgColor( cmbHospitalService );
            UIColors.SetNormalBgColor( cmbPatientType );
        }

        private void PopulateHsvCodes()
        {
            ArrayList hospitalServices = GetHospitalServiceCodes( User.GetCurrent().Facility.Oid, Model.KindOfVisit, Model.Activity, Model.HospitalService, Model.FinancialClass );

            if ( hospitalServices != null && hospitalServices.Count > 0 )
            {
                foreach ( HospitalService hs in hospitalServices )
                {
                    cmbHospitalService.Items.Add( hs );
                }

                cmbHospitalService.Sorted = true;

                VisitType inVisitType = ( VisitType )cmbPatientType.SelectedItem;

                // Auto-set HSV = 'PRE-REGISTER' during Short/Diagnostic Pre-Registration of an account
                if ( Model.Activity != null && Model.Activity is ShortPreRegistrationActivity &&
                     inVisitType != null && !string.IsNullOrEmpty( inVisitType.Code ) && 
                     inVisitType.Code == VisitType.PREREG_PATIENT )
                {
                    cmbHospitalService.SelectedIndex = 1;
                }

                if ( Model.HospitalService != null )
                {
                    cmbHospitalService.SelectedItem = Model.HospitalService;
                }

                RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceRequired ), new EventHandler( HospitalServiceRequiredEventHandler ) );
            }

            // Disable Hospital Service field while creating or editing a Short Pre-reg account
            if ( cmbPatientType.SelectedItem != null )
            {
                cmbHospitalService.Enabled = 
                    ( cmbHospitalService.SelectedItem != null && 
                    ( ( HospitalService )cmbHospitalService.SelectedItem ).Code != HospitalService.PRE_REGISTER );
            }
        }


        private ArrayList GetHospitalServiceCodes( long facilityOid, VisitType kindOfVisit, Activity activity, HospitalService service, FinancialClass financialClass )
        {
            ArrayList hospitalServices = null;

            cmbHospitalService.Items.Clear();

            if ( facilityHospitalServices == null )
            {
                HSVBrokerProxy brokerProxy = new HSVBrokerProxy();
                facilityHospitalServices = ( ArrayList )brokerProxy.SelectHospitalServicesFor( facilityOid );
            }

            HospitalService hospitalService = new HospitalService();

            if ( activity is ShortRegistrationActivity || 
                 activity is ShortPreRegistrationActivity ||
                 activity is ShortMaintenanceActivity)
            {
                hospitalServices = ( ArrayList )hospitalService.HospitalServicesFor( facilityHospitalServices,
                                                                                     kindOfVisit, activity, service,
                                                                                     financialClass );
            }
            else
            {
                VisitType inVisitType = ( VisitType )cmbPatientType.SelectedItem;
                if ( inVisitType != null && !string.IsNullOrEmpty( inVisitType.Code ) )
                {
                    if ( inVisitType.Code != VisitType.INPATIENT &&
                        inVisitType.Code != VisitType.PREREG_PATIENT &&
                        (
                            ( savedHospitalService != null && savedHospitalService.Code != string.Empty ) ||
                            ( service != null && service.Code != string.Empty )
                        )
                        )
                    {
                        string dayCare = string.Empty;
                        if ( savedHospitalService != null &&
                            savedHospitalService.Code != string.Empty )
                        {
                            dayCare = savedHospitalService.DayCare;
                        }
                        else
                        {
                            dayCare = service.DayCare;
                        }

                        if ( dayCare == null )
                        {
                            dayCare = "N";
                        }
                        hospitalServices = ( ArrayList )hospitalService.HospitalServicesFor( facilityHospitalServices,
                                                                                          inVisitType.Code, dayCare );
                    }
                    else
                    {
                        hospitalServices = ( ArrayList )hospitalService.HospitalServicesFor( facilityHospitalServices,
                                                                                          kindOfVisit, activity, service,
                                                                                          financialClass );
                    }
                }
            }
            return hospitalServices;
        }
        private void UnRegisterRequiredRuleEvents()
        {
            
            RuleEngine.GetInstance().UnregisterEvent( typeof( PatientTypeRequired ), new EventHandler( PatientTypeRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( HospitalServiceRequired ), new EventHandler( HospitalServiceRequiredEventHandler ) );
         
        }
        private void RegisterRequiredRuleEvents()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof (PatientTypeRequired),
                                                   new EventHandler(PatientTypeRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof (HospitalServiceRequired),
                                                   new EventHandler(HospitalServiceRequiredEventHandler));
        }

        private void unregisterEvents()
        {
            i_Registered = false;
            UnRegisterRequiredRuleEvents();
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidHospitalServiceCode ), new EventHandler( InvalidHospitalServiceCodeEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidPatientTypeCode ), new EventHandler( InvalidPatientTypeCodeEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidHospitalServiceCodeChange ), new EventHandler( InvalidHospitalServiceCodeChangeEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidPatientTypeCodeChange ), new EventHandler( InvalidPatientTypeCodeChangeEventHandler ) );
        }

        private void registerEvents()
        {
            RegisterRequiredRuleEvents();
            if ( i_Registered )
            {
                return;
            }
            i_Registered = true;
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidHospitalServiceCode ), new EventHandler( InvalidHospitalServiceCodeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidPatientTypeCode ), new EventHandler( InvalidPatientTypeCodeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidHospitalServiceCodeChange ), new EventHandler( InvalidHospitalServiceCodeChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidPatientTypeCodeChange ), new EventHandler( InvalidPatientTypeCodeChangeEventHandler ) );
        }
        #endregion

        #region Properties
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

        public string NursingStationCode
        {
            get
            {
                return i_NursingStationCode;
            }
            set
            {
                i_NursingStationCode = value;
            }
        }

        private ArrayList PatientTypes
        {
            get
            {
                if ( i_PatientTypes == null )
                {
                    string activityType = string.Empty;
                    string associatedActivityType = string.Empty;
                    string kindOfVisitCode = string.Empty;
                    string financialClassCode = string.Empty;
                    string locationBedCode = string.Empty;

                    Activity activity = Model.Activity;
                    if ( activity != null )
                    {
                        activityType = activity.GetType().ToString();
                        if ( activity.AssociatedActivityType != null )
                        {
                            associatedActivityType = activity.AssociatedActivityType.ToString();
                        }
                    }
                    if ( Model.KindOfVisit != null )
                    {
                        kindOfVisitCode = Model.KindOfVisit.Code;
                    }
                    if ( Model.FinancialClass != null )
                    {
                        financialClassCode = Model.FinancialClass.Code;
                    }
                    if ( Model.Location != null &&
                        Model.Location.Bed != null )
                    {
                        locationBedCode = Model.Location.Bed.Code;
                    }

                    IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
                    i_PatientTypes = patientBroker.PatientTypesFor( activityType, associatedActivityType, kindOfVisitCode,
                                                                    financialClassCode, locationBedCode, Model.Facility.Oid );
                }

                return i_PatientTypes;
            }
        }
        #endregion

        #region Data Elements
        private HospitalService savedHospitalService;
        private VisitType selectedPatientType;
        private VisitType savedPatientType;
        private bool blnLeaveRun = false;
        private bool isLoadData = true;
        private bool i_Registered;
        private ArrayList i_PatientTypes;
        private ArrayList facilityHospitalServices;
        string i_NursingStationCode;
        #endregion
    }
}
