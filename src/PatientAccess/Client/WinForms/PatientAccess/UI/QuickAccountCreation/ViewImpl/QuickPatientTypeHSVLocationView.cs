using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    public partial class QuickPatientTypeHSVLocationView : ControlView
    {
        #region Constructor

        public QuickPatientTypeHSVLocationView()
        {
            InitializeComponent();
        }

        #endregion

        #region Events 
        #endregion

        #region public methods

        public override void UpdateView()
        {
            try
            {
                SetNormalColor();
                registerEvents();
                SetKindOfVisit();
                PopulatePatientTypes();
                UpdateHospitalService();
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

        
        private void InvalidHospitalServiceCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbHospitalService );
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
            cmbHospitalService.Enabled = ( Model.KindOfVisit != null && Model.KindOfVisit.Code != String.Empty );

            RuleEngine.GetInstance().EvaluateRule( typeof( PatientTypeRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model ); 

            if ( Model.HospitalService == null )
            {
                Model.HospitalService = new HospitalService();
            }

            RuleEngine.GetInstance().RegisterEvent( typeof( PatientTypeRequired ), PatientTypeRequiredEventHandler );
            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceRequired ), HospitalServiceRequiredEventHandler );
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


            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidPatientTypeCode ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidPatientTypeCodeChange ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( PatientTypeRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
           
            Model.HospitalService = ( HospitalService )cmbHospitalService.SelectedItem ?? new HospitalService();
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

        private void UpdateHospitalService()
        { 
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model ); 
        } 

        private void cmbHospitalService_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( cmbHospitalService );
            Model.HospitalService = ( HospitalService )cmbHospitalService.SelectedItem;
            registerEvents();
            
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidHospitalServiceCode ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidHospitalServiceCodeChange ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( PatientTypeRequired ), Model );
             
        }

        /// <summary>
        /// Combo Box Hospital Service Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbHospitalService_SelectedIndexChanged( object sender, EventArgs e )
        {
            var hospitalService = cmbHospitalService.SelectedItem as HospitalService;

            // don't reset if the selected index changed event was fired on assignment of the model value
            if ( ( cmbHospitalService.SelectedIndex > 0 && Model.HospitalService != null &&
                   ( ( HospitalService )cmbHospitalService.SelectedItem ).Code != Model.HospitalService.Code )
                ||
                 ( cmbHospitalService.SelectedIndex < 1 && Model.HospitalService != null &&
                   Model.HospitalService.Code != string.Empty )
               )
            {
                if ( hospitalService != null && Model.KindOfVisit != null )
                {
                    Model.HospitalService = hospitalService;
                }
            }

            else
            {
                if ( facilityHospitalServices != null && facilityHospitalServices.Count > 0 )
                {
                    if ( hospitalService != null && Model.KindOfVisit != null )
                    {
                        Model.HospitalService = hospitalService;
                    }
                }
            }
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

            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
        }

        private void SetNormalColor()
        {
            UIColors.SetNormalBgColor( cmbHospitalService );
            UIColors.SetNormalBgColor( cmbPatientType );
        }

        private void PopulateHsvCodes()
        {
            var hospitalServices = GetHospitalServiceCodes( User.GetCurrent().Facility.Oid, Model.KindOfVisit, Model.Activity, Model.HospitalService, Model.FinancialClass );

            if ( hospitalServices != null && hospitalServices.Count > 0 )
            {
                foreach ( HospitalService hs in hospitalServices )
                {
                    cmbHospitalService.Items.Add( hs );
                }

                cmbHospitalService.Sorted = true;
                
                if ( Model.HospitalService != null )
                {
                    cmbHospitalService.SelectedItem = Model.HospitalService;
                }

                RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceRequired ), HospitalServiceRequiredEventHandler );
            }

            // Disable Hospital Service field while creating or editing a Short Pre-reg account
            if ( cmbPatientType.SelectedItem != null )
            {
                cmbHospitalService.Enabled = true;
            }
        } 

        private ArrayList GetHospitalServiceCodes( long facilityOid, VisitType kindOfVisit, Activity activity, HospitalService service, FinancialClass financialClass )
        {
            ArrayList hospitalServices = null;

            cmbHospitalService.Items.Clear();

            if ( facilityHospitalServices == null )
            {
                var hsvBrokerProxy = new HSVBrokerProxy();
                facilityHospitalServices = ( ArrayList )hsvBrokerProxy.SelectHospitalServicesFor( facilityOid );
            }

            var hospitalService = new HospitalService();

            if ( activity is QuickAccountCreationActivity || 
                 activity is QuickAccountMaintenanceActivity ||
                activity is PAIWalkinOutpatientCreationActivity )
            {
                hospitalServices = ( ArrayList )hospitalService.HospitalServicesFor( facilityHospitalServices,
                                                                                     kindOfVisit, activity, service,
                                                                                     financialClass );
            }

            return hospitalServices;
        }

        private void UnRegisterRequiredRuleEvents()
        {
            
            RuleEngine.GetInstance().UnregisterEvent( typeof( PatientTypeRequired ), PatientTypeRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( HospitalServiceRequired ), HospitalServiceRequiredEventHandler );
         
        }

        private void RegisterRequiredRuleEvents()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof (PatientTypeRequired), PatientTypeRequiredEventHandler);
            RuleEngine.GetInstance().RegisterEvent(typeof (HospitalServiceRequired), HospitalServiceRequiredEventHandler);
        }

        private void unregisterEvents()
        {
            i_Registered = false;
            UnRegisterRequiredRuleEvents();
            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidHospitalServiceCode), InvalidHospitalServiceCodeEventHandler);
            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidHospitalServiceCodeChange), InvalidHospitalServiceCodeChangeEventHandler);
        }

        private void registerEvents()
        {
            RegisterRequiredRuleEvents();
            if (i_Registered)

            {
                return;
            }

            i_Registered = true;
            RuleEngine.GetInstance().RegisterEvent(typeof (InvalidHospitalServiceCode), InvalidHospitalServiceCodeEventHandler);
            RuleEngine.GetInstance().RegisterEvent(typeof (InvalidHospitalServiceCodeChange), InvalidHospitalServiceCodeChangeEventHandler);
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

        private ArrayList PatientTypes
        {
            get
            {
                if ( i_PatientTypes == null )
                {
                    var activityType = string.Empty;
                    var associatedActivityType = string.Empty;
                    var kindOfVisitCode = string.Empty;
                    var financialClassCode = string.Empty;
                    var locationBedCode = string.Empty;

                    var activity = Model.Activity;
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

                    var patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
                    i_PatientTypes = patientBroker.PatientTypesFor( activityType, associatedActivityType, kindOfVisitCode,
                                                                    financialClassCode, locationBedCode, Model.Facility.Oid );
                }

                return i_PatientTypes;
            }
        }

        #endregion

        #region Data Elements

        private VisitType selectedPatientType; 
        private bool i_Registered;
        private ArrayList i_PatientTypes;
        private ArrayList facilityHospitalServices;

        #endregion
    }
}
