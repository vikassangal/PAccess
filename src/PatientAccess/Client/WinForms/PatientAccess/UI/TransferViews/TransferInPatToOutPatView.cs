using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.DocumentImagingViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using PatientAccess.UI.Reports.FaceSheet;

namespace PatientAccess.UI.TransferViews
{
    public class TransferInPatToOutPatView : ControlView, IAlternateCareFacilityView
    {
        #region Events
        public event EventHandler RepeatActivity;
        public event EventHandler EditAccount;
        public event EventHandler CloseView;
        #endregion

        #region Event Handlers

        private void TransferInPatToOutPatView_Load( object sender, EventArgs e )
        {
            panelActions.Visible = false;
            panelConfirmBottomArea.Visible = false;
            gbScanDocuments.Visible = false;
        }

        private void btnOK_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            if ( RuleEngine.AccountHasFailedError() )
            {
                btnOK.Enabled = true;
                return;
            }

            if ( !IsPBARAvailable() )
            {
                infoControl1.DisplayErrorMessage( UIErrorMessages.PBAR_UNAVAILABLE_MSG );
                btnOK.Enabled = true;
                Cursor = Cursors.Default;
                return;
            }

            if ( TransferService.IsTransferDateValid( mtbTransferDate ) )
            {
                if ( !TransferService.IsFutureTransferDate( mtbTransferDate,
                                     mtbTransferTime, FacilityDateTime, "NONE" ) )
                {
                    if ( !TransferService.IsTransferDateBeforeAdmitDate( mtbTransferDate,
                                          mtbTransferTime, Model.AdmitDate, "NONE", false ) )
                    {
                        if ( DateValidator.IsValidTime( mtbTransferTime ) )
                        {
                            Cursor = Cursors.WaitCursor;

                            // SR 804 - Remove Condition Code P7 when Inpatient is transferred to any other patient type
                            EmergencyToInPatientTransferCodeManager.UpdateConditionCodes();

                            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this, new LooseArgs( Model ) );
                            TransferInPatToOutPat();
                        }
                        else
                        {
                            UIColors.SetErrorBgColor( mtbTransferTime );
                            if ( !dateTimePicker.Focused )
                            {
                                mtbTransferTime.Focus();
                            }
                        }
                    }
                }
            }
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            if ( AccountActivityService.ConfirmCancelActivity(
                sender, new LooseArgs( Model ) ) )
            {
                if ( locationView1.Model != null
                    && locationView1.Model.Location != null
                    && locationView1.Model.Location != Model.LocationFrom )
                {
                    AccountActivityService.ReleaseBedLock( locationView1.Model.Location );
                }

                CancelBackgroundWorker();

                if ( CloseView != null )
                {
                    CloseView( this, new EventArgs() );
                }
                Dispose();
            }
            else
            {
                btnCancel.Enabled = true;
            }
        }

        private void CancelBackgroundWorker()
        {
            if ( backgroundWorker != null )
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void btnCloseActivity_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            CancelBackgroundWorker();

            CloseView( this, new EventArgs() );
            Dispose();
        }

        private void btnRepeatActivity_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            RepeatActivity( this, new LooseArgs( new Patient() ) );
        }

        private void btnEditAccount_Click( object sender, EventArgs e )
        {
            try
            {
                AccountView.CloseVIweb();
                if ( AccountLockStatus.IsAccountLocked( Model, User.GetCurrent() ) )
                {
                    MessageBox.Show( UIErrorMessages.NO_EDIT_RECORD_LOCKED, "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );
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
                else
                {
                    Model.Activity = new MaintenanceActivity();
                }

                if ( !Model.Activity.ReadOnlyAccount() )
                {
                    if ( !AccountActivityService.PlaceLockOn( Model, UIErrorMessages.PATIENT_ACCTS_LOCKED ) )
                    {
                        Cursor = Cursors.Default;
                        btnEditAccount.Enabled = true;
                        return;
                    }
                }

                EditAccount( this, new LooseArgs( Model ) );

                Cursor = Cursors.Default;
            }
            catch ( AccountNotFoundException )
            {
                Cursor = Cursors.Default;
                btnEditAccount.Enabled = true;
                MessageBox.Show( UIErrorMessages.ACTIVITY_CANNOT_PROCEED, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
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

        private void dateTimePicker_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbTransferDate );

            mtbTransferDate.UnMaskedText = CommonFormatting.MaskedDateFormat( dateTimePicker.Value );
            SetTransferDateOnModel();
            mtbTransferDate.Focus();

            RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateRequired ), Model );

            EnableDisableBtnOK();
        }

        private void mtbTransferDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbTransferDate );

            if ( !dateTimePicker.Focused )
            {
                if ( mtbTransferDate.UnMaskedText != String.Empty )
                {
                    if ( TransferService.IsTransferDateValid( mtbTransferDate ) )
                    {
                        if ( !TransferService.IsFutureTransferDate( mtbTransferDate,
                                mtbTransferTime, FacilityDateTime, "DATE" ) )
                        {
                            if ( TransferService.IsTransferDateBeforeAdmitDate( mtbTransferDate,
                                    mtbTransferTime, Model.AdmitDate, "DATE", false ) )
                            {
                                SetTransferDateOnModel();
                            }
                        }
                        else
                        {
                            if ( mtbTransferDate.Focused )
                            {
                                mtbTransferTime.Focus();
                            }
                        }
                    }
                    Refresh();
                }
                else
                {
                    SetTransferDateOnModel();
                }
            }

            EnableDisableBtnOK();
        }

        private void mtbTransferTime_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbTransferDate );
            UIColors.SetNormalBgColor( mtbTransferTime );

            if ( mtbTransferTime.UnMaskedText != String.Empty )
            {
                if ( DateValidator.IsValidTime( mtbTransferTime ) )
                {
                    if ( !TransferService.IsFutureTransferDate( mtbTransferDate,
                            mtbTransferTime, FacilityDateTime, "TIME" ) )
                    {
                        if ( TransferService.IsTransferDateBeforeAdmitDate( mtbTransferDate,
                                mtbTransferTime, Model.AdmitDate, "TIME", false ) )
                        {
                            SetTransferDateOnModel();
                        }
                    }
                }
                else
                {
                    if ( !dateTimePicker.Focused )
                    {
                        mtbTransferTime.Focus();
                    }
                    UIColors.SetErrorBgColor( mtbTransferTime );
                }
            }
            else
            {
                SetTransferDateOnModel();
            }
            Refresh();

            EnableDisableBtnOK();
            mtbRemarks.Focus();
        }
        private void SetTransferDateOnModel()
        {
            Model.TransferDate = GetTransferDateTime();
            RuleEngine.GetInstance().EvaluateRule(typeof(TransferDateRequired), Model);
        }
        
        private void cboAdmitSource_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if ( cb != null && cb.SelectedIndex != -1 )
            {
                HandleAdmitSourceSelectedIndexChanged( cb.SelectedItem as AdmitSource );
            }

            EnableDisableBtnOK();
        }
        private void cmbAdmitSource_Validating( object sender, CancelEventArgs e )
        {
            HandleAdmitSourceSelectedIndexChanged( cboAdmitSource.SelectedItem as AdmitSource );


            UIColors.SetNormalBgColor( cboAdmitSource );
            Refresh();

            RuleEngine.EvaluateRule( typeof( AdmitSourceRequired ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitSourcePreferred ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidAdmitSourceCode ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidAdmitSourceCodeChange ), Model );
            AlternateCareFacilityPresenter.HandleAlternateCareFacility();

        }

        private void HandleAdmitSourceSelectedIndexChanged( AdmitSource newAdmitSource )
        {
            if ( newAdmitSource != null )
            {
                Model.AdmitSource = newAdmitSource;
                BreadCrumbLogger.GetInstance.Log( String.Format( "{0} selected", Model.AdmitSource.Description ) );
            }

            UIColors.SetNormalBgColor( cboAdmitSource );
            RuleEngine.EvaluateRule( typeof( AdmitSourceRequired ), Model );

            AlternateCareFacilityPresenter.HandleAlternateCareFacility();
        }

        private void AlternateCareFacilityRequiredEventHandler( object sender, EventArgs e )
        {
            cmbAlternateCareFacility.Enabled = true;
            UIColors.SetRequiredBgColor( cmbAlternateCareFacility );
        }

        private void cmbAlternateCareFacility_SelectedIndexChanged( object sender, EventArgs e )
        {
            string selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if ( selectedAlternateCare != null )
            {
                AlternateCareFacilityPresenter.UpdateAlternateCareFacility( selectedAlternateCare );
            }
            UIColors.SetNormalBgColor( cmbAlternateCareFacility );
            AlternateCareFacilityPresenter.EvaluateAlternateCareFacilityRule();

            EnableDisableBtnOK();
        }

        private void cmbAlternateCareFacility_Validating( object sender, CancelEventArgs e )
        {
            string selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if ( selectedAlternateCare != null )
            {
                AlternateCareFacilityPresenter.UpdateAlternateCareFacility( selectedAlternateCare );
            }
            UIColors.SetNormalBgColor( cmbAlternateCareFacility );
            AlternateCareFacilityPresenter.EvaluateAlternateCareFacilityRule();
        }
        private void cboPatientType_SelectedIndexChanged( object sender, EventArgs e )
        {
            Model.KindOfVisit = ( VisitType )cboPatientType.SelectedItem;

            RunRules();

            if ( Model.DischargeDate == DateTime.MinValue &&
               ( cboHospitalService.SelectedIndex > 0 ) )
            {
                cboHospitalService.SelectedIndex = 0;
                return;
            }
        }

        private void cboHospitalService_SelectedIndexChanged( object sender, EventArgs e )
        {
            Model.HospitalService = ( HospitalService )cboHospitalService.SelectedItem;

            UnLockNewReservedBed();
            RestoreOldLocation();

            if ( cboHospitalService.SelectedIndex <= 0 )
            {
                locationView1.DisableLocationControls();
            }
            else
            {
                BreadCrumbLogger.GetInstance.Log( String.Format( "{0} selected", cboHospitalService.SelectedItem ) );

                if ( Model.DischargeDate == DateTime.MinValue )
                {
                    locationView1.EnableLocationControls();
                }
                else
                {
                    locationView1.DisableLocationControls();
                }
            }
            RunRules();
            EnableDisableBtnOK();
        }

        private void locationView1_BedSelected( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( locationView1.field_AssignedBed );
            RuleEngine.GetInstance().EvaluateRule( typeof( LocationRequired ), Model );

            EnableDisableBtnOK();
        }

        private void cboClinic1_SelectedIndexChanged( object sender, EventArgs e )
        {
            Model.HospitalClinic = ( HospitalClinic )cboClinic1.SelectedItem;

            UIColors.SetNormalBgColor( cboClinic1 );
            RuleEngine.GetInstance().EvaluateRule( typeof( ClinicOneRequired ), Model );

            EnableDisableBtnOK();
        }

        private void AdmitSourceRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboAdmitSource );
        }

        private void PatientTypeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboPatientType );
        }

        private void HospitalServiceRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboHospitalService );
            locationView1.DisableLocationControls();
        }

        private void LocationRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( locationView1.field_AssignedBed );
        }

        private void ClinicOneRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboClinic1 );
        }

        private void TransferDateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbTransferDate );
        }

        private void TransferInPatToOutPatView_Disposed( object sender, EventArgs e )
        {
            UnRegisterRulesEvents();
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
            AlternateCareFacilityPresenter = new AlternateCareFacilityPresenter( this, new AlternateCareFacilityFeatureManager() );

            var primaryCarePhysicianFeatureManager = new PrimaryCarePhysicianFeatureManager();
            lblPcp.Text = 
                primaryCarePhysicianFeatureManager.IsPrimaryCarePhysicianEnabledForDate( Model.AccountCreatedDate ) ? 
                    PhysicianRole.PRIMARYCAREPHYSICIAN_LABEL : 
                    PhysicianRole.OTHERPHYSICIAN_LABEL;
            RegisterRulesEvents();

            if ( Model != null )
            {
                btnOK.Enabled = true;

                if ( Model.Patient != null )
                {
                    patientContextView1.Model = Model.Patient;
                    patientContextView1.Account = Model;
                    patientContextView1.UpdateView();

                    lblPatientNameVal.Text = Model.Patient.FormattedName;
                }

                DoPreValidation();

                lblAccountVal.Text = Model.AccountNumber.ToString();
                lblAdmitDateVal.Text = CommonFormatting.LongDateFormat( Model.AdmitDate );
                lblAdmitTimeVal.Text = CommonFormatting.DisplayedTimeFormat( Model.AdmitDate );

                if ( Model.Insurance.Coverages.Count > 0 )
                {
                    foreach ( Coverage coverage in Model.Insurance.Coverages )
                    {
                        if ( coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                        {
                            lblPrimaryPlanVal.Text = coverage.InsurancePlan.PlanID + " " + coverage.InsurancePlan.Payor.Name;
                        }
                    }
                }

                if ( Model.FinancialClass != null )
                {
                    lblFinancialClassVal.Text = Model.FinancialClass.ToCodedString();
                }

                if ( Model.AdmitSource != null )
                {
                    lblAdmitSourceVal.Text = Model.AdmitSource.DisplayString;
                }

                if ( Model.KindOfVisit != null )
                {
                    lblPatientTypeVal.Text = Model.KindOfVisit.DisplayString;
                }

                if ( Model.HospitalService != null )
                {
                    lblHospitalServiceVal.Text = Model.HospitalService.DisplayString;
                }

                if ( Model.Location != null )
                {
                    lblLocationVal.Text = Model.Location.DisplayString;
                }

                PopulatePhysicians();

                if ( PreValidationSuccess )
                {
                    BackUpFromValues();

                    PopulateAdmitSources();
                    PopulatePatientTypes();
                    PopulateHsvCodes();

                    locationView1.Model = Model;
                    locationView1.UpdateView();

                    ManageLocationCtrlStatus();

                    //Erase any existing Hospital clinic from model and force user to select a new Hospital clinic. 
                    Model.ClearHospitalClinic();
                    
                    PopulateClinics();
                    TransferService.PopulateDefaultTransferDateTime( mtbTransferDate, mtbTransferTime, FacilityDateTime );
                    SetTransferDateOnModel();

                    cboAdmitSource.Focus();
                    mtbRemarks.Text = Model.ClinicalComments;
                    RunRules();

                    EnableDisableBtnOK();
                }
                else
                {
                    DisableControls();
                }
            }
        }

        public override void UpdateModel()
        {
            Model.LocationTo = Model.Location;
            ( ( TransferInToOutActivity )Model.Activity ).Remarks = mtbRemarks.UnMaskedText;
            SetTransferDateOnModel();
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
            if ( !cmbAlternateCareFacility.Items.Contains( Model.AlternateCareFacility ) )
            {
                cmbAlternateCareFacility.Items.Add( Model.AlternateCareFacility );
            }

            cmbAlternateCareFacility.SelectedItem = Model.AlternateCareFacility;
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

        public DateTime GetTransferDateTime()
        {
            string date = mtbTransferDate.Text;

            string time = "00:00";
            if ( mtbTransferTime.Text.Length == 5 )
            {
                int admitHour = Convert.ToInt32( mtbTransferTime.Text.Substring( 0, 2 ) );
                int admitMinute = Convert.ToInt32( mtbTransferTime.Text.Substring( 3, 2 ) );

                try
                {
                    if ( ( admitHour >= 0 && admitHour < 25 ) && ( admitMinute >= 0 && admitMinute < 61 ) )
                    {
                        time = admitHour + ":" + admitMinute;
                    }
                }
                catch
                {
                }
            }
            return mtbTransferDate.UnMaskedText == String.Empty ? DateTime.MinValue : Convert.ToDateTime( date + " " + time );
        }

        public void SetTransferDateUnMaskedText( string unMaskedText )
        {
            mtbTransferDate.UnMaskedText = unMaskedText;
        }

        public void SetTransferTimeUnMaskedText( string unMaskedText )
        {
            mtbTransferTime.UnMaskedText = unMaskedText;
        }

        #endregion

        #region Properties

        private DateTime FacilityDateTime
        {
            get
            {
                if ( i_FacilityDateTime == DateTime.MinValue )
                {
                    i_FacilityDateTime = TransferService.GetLocalDateTime( User.GetCurrent().Facility.GMTOffset,
                                                                           User.GetCurrent().Facility.DSTOffset );
                }

                return i_FacilityDateTime;
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
        #endregion

        #region Private Methods

        private bool IsPBARAvailable()
        {
            Cursor = Cursors.WaitCursor;
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            return facilityBroker.IsDatabaseAvailableFor( User.GetCurrent().Facility.ConnectionSpec.ServerIP );
        }

        private void PopulatePhysicians()
        {
            if ( Model.ReferringPhysician != null )
            {
                lblRefVal.Text = String.Format( "{0:00000} {1}", Model.ReferringPhysician.PhysicianNumber,
                                                 Model.ReferringPhysician.FormattedName );
            }

            if ( Model.AdmittingPhysician != null )
            {
                lblAdmVal.Text = String.Format( "{0:00000} {1}", Model.AdmittingPhysician.PhysicianNumber,
                                                 Model.AdmittingPhysician.FormattedName );
            }

            if ( Model.AttendingPhysician != null )
            {
                lblAttVal.Text = String.Format( "{0:00000} {1}", Model.AttendingPhysician.PhysicianNumber,
                                                 Model.AttendingPhysician.FormattedName );
            }

            if ( Model.OperatingPhysician != null )
            {
                lblOprVal.Text = String.Format( "{0:00000} {1}", Model.OperatingPhysician.PhysicianNumber,
                                                 Model.OperatingPhysician.FormattedName );
            }

            if ( Model.PrimaryCarePhysician != null )
            {
                lblPcpVal.Text = String.Format( "{0:00000} {1}", Model.PrimaryCarePhysician.PhysicianNumber,
                                                 Model.PrimaryCarePhysician.FormattedName );
            }
        }

        private void PopulateAdmitSources()
        {
            AdmitSourceBrokerProxy brokerProxy = new AdmitSourceBrokerProxy();
            ArrayList allSources = ( ArrayList )brokerProxy.AllTypesOfAdmitSources( User.GetCurrent().Facility.Oid );

            cboAdmitSource.Items.Clear();

            foreach ( object t in allSources )
            {
                AdmitSource source = ( AdmitSource )t;

                cboAdmitSource.Items.Add( source );
            }

            if ( Model.AdmitSource != null )
            {
                cboAdmitSource.SelectedItem = Model.AdmitSource;
            }
            else
            {
                cboAdmitSource.SelectedIndex = 0;
            }
        }

        private void PopulatePatientTypes()
        {
            PatientBrokerProxy patientBroker = new PatientBrokerProxy();
            ArrayList allPatientTypes = ( ArrayList )patientBroker.AllPatientTypes( Model.Facility.Oid );

            cboPatientType.Items.Clear();

            foreach ( object t in allPatientTypes )
            {
                VisitType patType = ( VisitType )t;

                if ( patType.Code == VisitType.OUTPATIENT || patType.Code == "3" && Model.DischargeDate != DateTime.MinValue )
                {
                    cboPatientType.Items.Add( patType );
                }
            }

            cboPatientType.SelectedItem = Model.KindOfVisit;

            if ( cboPatientType.Items.Count <= 1 )
            {
                cboPatientType.Enabled = false;
            }
        }

        private void PopulateHsvCodes()
        {
            if ( ( cboHospitalService.Items.Count == 0 ) && ( Model.KindOfVisit != null ) )
            {
                HSVBrokerProxy brokerProxy = new HSVBrokerProxy();
                ICollection hsvCodes = brokerProxy.HospitalServicesFor( User.GetCurrent().Facility.Oid, VisitType.OUTPATIENT, "Y" );

                cboHospitalService.Items.Clear();
                foreach ( HospitalService hospitalService in hsvCodes )
                {
                    if ( hospitalService.Code != "LB" )
                    {
                        cboHospitalService.Items.Add( hospitalService );
                    }
                }
                cboHospitalService.Sorted = true;
            }

            cboHospitalService.SelectedItem = Model.HospitalService;

        }

        private void PopulateClinics()
        {
            IHospitalClinicsBroker clinicBroker = new HospitalClinicsBrokerProxy();
            ArrayList allClinics = ( ArrayList )clinicBroker.HospitalClinicsFor( Model.Facility.Oid );

            cboClinic1.Items.Clear();

            foreach ( object t in allClinics )
            {
                HospitalClinic clinic = ( HospitalClinic )t;

                cboClinic1.Items.Add( clinic );
            }

            cboClinic1.SelectedItem = null;
        }

        private void BeforeWork()
        {
            ShowPanel();
        }

        private void DoTransfer( object sender, DoWorkEventArgs e )
        {
            UpdateModel();

            TransferService.QueueTransfer( Model );

            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( IsDisposed || Disposing )
                return;

            if ( e.Cancelled )
            {
                // user cancelled
                // Due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
                btnOK.Enabled = true;
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
            }
            else
            {
                // success
                lblAdmitSourceVal.Text = cboAdmitSource.SelectedIndex != -1 ? cboAdmitSource.SelectedItem.ToString() : string.Empty;
                lblPatientTypeVal.Text = cboPatientType.SelectedIndex != -1 ? cboPatientType.SelectedItem.ToString() : string.Empty;
                lblHospitalServiceVal.Text = cboHospitalService.SelectedIndex > 0 ? cboHospitalService.SelectedItem.ToString() : string.Empty;
                lblLocationVal.Text = locationView1.Model.Location.PrintString;
                lblConfirmClinicVal.Text = cboClinic1.SelectedIndex != -1 ? cboClinic1.SelectedItem.ToString() : string.Empty;
                lblConfirmTransferDateVal.Text = mtbTransferDate.Text;
                lblConfirmTransferTimeVal.Text = mtbTransferTime.Text;

                userContextView1.Description = "Transfer Inpatient to Outpatient - Submitted";
                infoControl1.DisplayErrorMessage( "Transfer Inpatient to Outpatient submitted for processing." );

                grpPhysicians.Visible = false;
                panelToArea.Visible = false;
                panelRemarks.Visible = false;

                llblTransferFrom.Visible = false;

                panelFromBottomArea.Location = new Point( panelFromBottomArea.Location.X, 165 );

                panelConfirmBottomArea.Location = new Point( panelConfirmBottomArea.Location.X, 260 );
                panelConfirmBottomArea.Visible = true;

                btnOK.Visible = false;
                btnCancel.Visible = false;

                // clear any FUS notes that were manually entered so that - if the View FUS notes icon/menu option is selected,
                // the newly added notes do not show twice.

                if ( ViewFactory.Instance.CreateView<PatientAccessView>().Model != null )
                {
                    ( ( Account )ViewFactory.Instance.CreateView<PatientAccessView>().Model ).ClearFusNotes();
                }

                panelActions.Visible = true;
                panelActions.BringToFront();
                gbScanDocuments.Visible = true;

                btnCloseActivity.Focus();

                // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
            }

            // post-completion operations...
            HidePanel();
            Refresh();

            Cursor = Cursors.Default;
        }

        private void TransferInPatToOutPat()
        {
            if ( backgroundWorker == null ||
                !backgroundWorker.IsBusy )
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoTransfer;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void DoPreValidation()
        {
            if ( IsValidPatientType() &&
                HasValidDischargeStatus() &&
                HasNotPassed3MidNights() &&
                IsLockOK()
                )
                PreValidationSuccess = true;
            else
            {
                PreValidationSuccess = false;
                btnOK.Enabled = false;
            }
        }

        private bool IsValidPatientType()
        {
            if ( Model.KindOfVisit.Code != VisitType.INPATIENT )
            {
                infoControl1.DisplayInfoMessage( UIErrorMessages.TRANSFER_NOT_INPATIENT_MSG );
                return false;
            }

            return true;
        }

        private bool HasValidDischargeStatus()
        {
            if ( Model.DischargeDate != DateTime.MinValue && Model.AbstractExists )
            {
                infoControl1.DisplayInfoMessage( UIErrorMessages.TRANSFER_ABSTRACT_COMPLETED_MSG );
                return false;
            }

            if ( Model.DischargeDate != DateTime.MinValue && Model.BillHasDropped )
            {
                infoControl1.DisplayInfoMessage( UIErrorMessages.TRANSFER_BILL_DROPPED_MSG );
                return false;
            }

            return true;
        }

        private bool HasNotPassed3MidNights()
        {
            if ( FacilityDateTime > Model.AdmitDate.AddDays( 3 ) )
            {
                infoControl1.DisplayInfoMessage( UIErrorMessages.TRANSFER_ADMITTED_3_MIDNIGHTS_AGO_MSG );
                return false;
            }

            return true;
        }

        private bool IsLockOK()
        {
            bool blnRC = false;

            if ( Model.AccountLock.IsLocked )
            {
                if ( !Model.AccountLock.AcquiredLock )
                {
                    infoControl1.DisplayInfoMessage( UIErrorMessages.DISCHARGE_ACCOUNT_LOCKED_MSG );
                }
                else
                {
                    blnRC = true;
                }
            }

            return blnRC;
        }

        private void ManageLocationCtrlStatus()
        {
            if ( ( ( VisitType )cboPatientType.SelectedItem ).Code == VisitType.OUTPATIENT &&
                Model.DischargeDate == DateTime.MinValue &&
                cboHospitalService.SelectedIndex > 0 )
            {
                locationView1.EnableLocationControls();
            }
            else
            {
                locationView1.DisableLocationControls();
            }
        }

        private void RegisterRulesEvents()
        {
            RuleEngine.LoadRules( Model );

            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitSourceRequired ), Model, new EventHandler( AdmitSourceRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AlternateCareFacilityRequired ), new EventHandler( AlternateCareFacilityRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( PatientTypeRequired ), Model, new EventHandler( PatientTypeRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceRequired ), Model, new EventHandler( HospitalServiceRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( LocationRequired ), Model, new EventHandler( LocationRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( ClinicOneRequired ), Model, new EventHandler( ClinicOneRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( TransferDateRequired ), Model, new EventHandler( TransferDateRequiredEventHandler ) );
        }

        private void UnRegisterRulesEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitSourceRequired ), Model, AdmitSourceRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AlternateCareFacilityRequired ), Model, AlternateCareFacilityRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PatientTypeRequired ), Model, PatientTypeRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( HospitalServiceRequired ), Model, HospitalServiceRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( LocationRequired ), Model, LocationRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ClinicOneRequired ), Model, ClinicOneRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( TransferDateRequired ), Model, TransferDateRequiredEventHandler );
        }

        private void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds
            UIColors.SetNormalBgColor( cboAdmitSource );
            if ( !cboPatientType.Enabled )
            {
                UIColors.SetNormalBgColor( cboPatientType );
            }
            UIColors.SetNormalBgColor( cboHospitalService );
            UIColors.SetNormalBgColor( locationView1.field_AssignedBed );
            UIColors.SetNormalBgColor( cboClinic1 );
            UIColors.SetNormalBgColor( mtbTransferDate );

            Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof( AdmitSourceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( PatientTypeRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( LocationRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( ClinicOneRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateRequired ), Model );
        }

        private void DisableControls()
        {
            panelToArea.Enabled = false;
            panelRemarks.Enabled = false;
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

        private void BackUpFromValues()
        {
            i_AdmitSourceFrom = Model.AdmitSource;
            i_VisitTypeFrom = Model.KindOfVisit;

            Model.TransferredFromHospitalService = ( HospitalService )Model.HospitalService.Clone();
            Model.LocationFrom = Model.Location != null ? ( Location )Model.Location.Clone() : null;


            Model.KindOfVisit = new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT );
            Model.HospitalService = null;
            Model.LocationTo = Model.Location;
        }

        private void UnLockNewReservedBed()
        {
            if ( Model.Location != Model.LocationFrom )
            {
                TransferService.ReleaseBed( Model.Location, User.GetCurrent().Facility );
            }
        }

        private void RestoreOldLocation()
        {
            if ( Model.LocationFrom != null )
            {
                Model.Location = ( Location )Model.LocationFrom.Clone();
            }
            else
            {
                Model.Location = null;
            }

            locationView1.UpdateView();
        }

        private void EnableDisableBtnOK()
        {
            if ( cboAdmitSource.BackColor == UIColors.TextFieldBackgroundRequired ||
                cmbAlternateCareFacility.BackColor == UIColors.TextFieldBackgroundRequired ||
                cboPatientType.BackColor == UIColors.TextFieldBackgroundRequired ||
                cboHospitalService.BackColor == UIColors.TextFieldBackgroundRequired ||
                locationView1.field_AssignedBed.BackColor == UIColors.TextFieldBackgroundRequired ||
                cboClinic1.BackColor == UIColors.TextFieldBackgroundRequired ||
                mtbTransferDate.BackColor == UIColors.TextFieldBackgroundRequired ||
                mtbTransferTime.BackColor == UIColors.TextFieldBackgroundRequired ||
                mtbTransferDate.BackColor == UIColors.TextFieldBackgroundError ||
                mtbTransferTime.BackColor == UIColors.TextFieldBackgroundError )
            {
                btnOK.Enabled = false;
            }
            else
            {
                btnOK.Enabled = true;
            }
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureTransferInpatientToOutpatientRemarks( mtbRemarks );
        }

        #endregion

        #region Private Properties
        private bool PreValidationSuccess
        {
            get
            {
                return i_PreValidationSuccess;
            }
            set
            {
                i_PreValidationSuccess = value;
            }
        }

        private RuleEngine RuleEngine
        {
            get { return i_RuleEngine ?? (i_RuleEngine = RuleEngine.GetInstance()); }
        }

        private EmergencyToInPatientTransferCodeManager EmergencyToInPatientTransferCodeManager
        {
            get
            {
                if ( emergencyToInpatientTransferCodeManager == null )
                {
                    var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                    var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
                    emergencyToInpatientTransferCodeManager = new EmergencyToInPatientTransferCodeManager(
                        DateTime.Parse( ConfigurationManager.AppSettings[ApplicationConfigurationKeys.ER_TO_IP_CONDITION_CODE_START_DATE] ),
                        Model,
                        accountBroker, conditionCodeBroker );
                }

                return emergencyToInpatientTransferCodeManager;
            }
        }

        #endregion

        #region Construction and Finalization

        public TransferInPatToOutPatView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

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

                CancelBackgroundWorker();
            }
            base.Dispose( disposing );
        }


        #endregion

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TransferInPatToOutPatView ) );
            this.panelTransferInPatToOutPat = new System.Windows.Forms.Panel();
            this.gbScanDocuments = new System.Windows.Forms.GroupBox();
            this.btnScanDocuments = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblScanDocuments = new System.Windows.Forms.Label();
            this.llblTransferFrom = new PatientAccess.UI.CommonControls.LineLabel();
            this.infoControl1 = new PatientAccess.UI.CommonControls.InfoControl();
            this.panelConfirmBottomArea = new System.Windows.Forms.Panel();
            this.lblConfirmClinicVal = new System.Windows.Forms.Label();
            this.lblConfirmClinic = new System.Windows.Forms.Label();
            this.lblConfirmTransferTimeVal = new System.Windows.Forms.Label();
            this.lblConfirmTransferTime = new System.Windows.Forms.Label();
            this.lblConfirmTransferDateVal = new System.Windows.Forms.Label();
            this.lblConfirmTransferDate = new System.Windows.Forms.Label();
            this.panelFromBottomArea = new System.Windows.Forms.Panel();
            this.lblHospitalServiceVal = new System.Windows.Forms.Label();
            this.lblPatientTypeVal = new System.Windows.Forms.Label();
            this.lblAdmitSourceVal = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblHospitalService = new System.Windows.Forms.Label();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.lblAdmitSource = new System.Windows.Forms.Label();
            this.lblLocationVal = new System.Windows.Forms.Label();
            this.panelToArea = new System.Windows.Forms.Panel();
            this.cmbAlternateCareFacility = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblAlternateCareFacility = new System.Windows.Forms.Label();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.locationView1 = new PatientAccess.UI.CommonControls.LocationView();
            this.mtbTransferTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cboAdmitSource = new System.Windows.Forms.ComboBox();
            this.lblAdmitSource2 = new System.Windows.Forms.Label();
            this.cboClinic1 = new System.Windows.Forms.ComboBox();
            this.lblHospitalService2 = new System.Windows.Forms.Label();
            this.mtbTransferDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cboPatientType = new System.Windows.Forms.ComboBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.lblPatientType2 = new System.Windows.Forms.Label();
            this.lblTransferDate = new System.Windows.Forms.Label();
            this.cboHospitalService = new System.Windows.Forms.ComboBox();
            this.lblClinic1 = new System.Windows.Forms.Label();
            this.panelActions = new System.Windows.Forms.Panel();
            this.btnPrintFaceSheet = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnEditAccount = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnRepeatActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCloseActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblAction = new System.Windows.Forms.Label();
            this.panelRemarks = new System.Windows.Forms.Panel();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.mtbRemarks = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblAdmitTimeVal = new System.Windows.Forms.Label();
            this.lblAdmitTime = new System.Windows.Forms.Label();
            this.lblFinancialClassVal = new System.Windows.Forms.Label();
            this.lblPrimaryPlanVal = new System.Windows.Forms.Label();
            this.lblAdmitDateVal = new System.Windows.Forms.Label();
            this.lblAccountVal = new System.Windows.Forms.Label();
            this.lblPatientNameVal = new System.Windows.Forms.Label();
            this.lblFinancialClass = new System.Windows.Forms.Label();
            this.lblPrimaryPlan = new System.Windows.Forms.Label();
            this.lblAdmitDate = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
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
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.label13 = new System.Windows.Forms.Label();
            this.btnOK = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnCancel = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.panelUserContext = new System.Windows.Forms.Panel();
            this.userContextView1 = new PatientAccess.UI.UserContextView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.panelTransferInPatToOutPat.SuspendLayout();
            this.gbScanDocuments.SuspendLayout();
            this.panelConfirmBottomArea.SuspendLayout();
            this.panelFromBottomArea.SuspendLayout();
            this.panelToArea.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelRemarks.SuspendLayout();
            this.grpPhysicians.SuspendLayout();
            this.panelUserContext.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTransferInPatToOutPat
            // 
            this.panelTransferInPatToOutPat.BackColor = System.Drawing.Color.White;
            this.panelTransferInPatToOutPat.Controls.Add( this.gbScanDocuments );
            this.panelTransferInPatToOutPat.Controls.Add( this.llblTransferFrom );
            this.panelTransferInPatToOutPat.Controls.Add( this.infoControl1 );
            this.panelTransferInPatToOutPat.Controls.Add( this.panelConfirmBottomArea );
            this.panelTransferInPatToOutPat.Controls.Add( this.panelFromBottomArea );
            this.panelTransferInPatToOutPat.Controls.Add( this.panelToArea );
            this.panelTransferInPatToOutPat.Controls.Add( this.panelActions );
            this.panelTransferInPatToOutPat.Controls.Add( this.panelRemarks );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAdmitTimeVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAdmitTime );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblFinancialClassVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblPrimaryPlanVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAdmitDateVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAccountVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblPatientNameVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblFinancialClass );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblPrimaryPlan );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAdmitDate );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAccount );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblPatientName );
            this.panelTransferInPatToOutPat.Controls.Add( this.grpPhysicians );
            this.panelTransferInPatToOutPat.Controls.Add( this.progressPanel1 );
            this.panelTransferInPatToOutPat.Location = new System.Drawing.Point( 8, 64 );
            this.panelTransferInPatToOutPat.Name = "panelTransferInPatToOutPat";
            this.panelTransferInPatToOutPat.Size = new System.Drawing.Size( 1008, 520 );
            this.panelTransferInPatToOutPat.TabIndex = 0;
            // 
            // gbScanDocuments
            // 
            this.gbScanDocuments.Controls.Add( this.btnScanDocuments );
            this.gbScanDocuments.Controls.Add( this.lblScanDocuments );
            this.gbScanDocuments.Location = new System.Drawing.Point( 16, 353 );
            this.gbScanDocuments.Name = "gbScanDocuments";
            this.gbScanDocuments.Size = new System.Drawing.Size( 322, 84 );
            this.gbScanDocuments.TabIndex = 9;
            this.gbScanDocuments.TabStop = false;
            this.gbScanDocuments.Text = "Scan documents";
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
            // llblTransferFrom
            // 
            this.llblTransferFrom.Caption = "Transfer from";
            this.llblTransferFrom.Location = new System.Drawing.Point( 24, 174 );
            this.llblTransferFrom.Name = "llblTransferFrom";
            this.llblTransferFrom.Size = new System.Drawing.Size( 328, 18 );
            this.llblTransferFrom.TabIndex = 0;
            this.llblTransferFrom.TabStop = false;
            // 
            // infoControl1
            // 
            this.infoControl1.Location = new System.Drawing.Point( 24, 8 );
            this.infoControl1.Message = "";
            this.infoControl1.Name = "infoControl1";
            this.infoControl1.Size = new System.Drawing.Size( 960, 32 );
            this.infoControl1.TabIndex = 0;
            // 
            // panelConfirmBottomArea
            // 
            this.panelConfirmBottomArea.Controls.Add( this.lblConfirmClinicVal );
            this.panelConfirmBottomArea.Controls.Add( this.lblConfirmClinic );
            this.panelConfirmBottomArea.Controls.Add( this.lblConfirmTransferTimeVal );
            this.panelConfirmBottomArea.Controls.Add( this.lblConfirmTransferTime );
            this.panelConfirmBottomArea.Controls.Add( this.lblConfirmTransferDateVal );
            this.panelConfirmBottomArea.Controls.Add( this.lblConfirmTransferDate );
            this.panelConfirmBottomArea.Location = new System.Drawing.Point( 16, 291 );
            this.panelConfirmBottomArea.Name = "panelConfirmBottomArea";
            this.panelConfirmBottomArea.Size = new System.Drawing.Size( 336, 56 );
            this.panelConfirmBottomArea.TabIndex = 0;
            // 
            // lblConfirmClinicVal
            // 
            this.lblConfirmClinicVal.Location = new System.Drawing.Point( 96, 8 );
            this.lblConfirmClinicVal.Name = "lblConfirmClinicVal";
            this.lblConfirmClinicVal.Size = new System.Drawing.Size( 224, 16 );
            this.lblConfirmClinicVal.TabIndex = 0;
            // 
            // lblConfirmClinic
            // 
            this.lblConfirmClinic.Location = new System.Drawing.Point( 9, 8 );
            this.lblConfirmClinic.Name = "lblConfirmClinic";
            this.lblConfirmClinic.Size = new System.Drawing.Size( 71, 16 );
            this.lblConfirmClinic.TabIndex = 0;
            this.lblConfirmClinic.Text = "Clinic 1:";
            // 
            // lblConfirmTransferTimeVal
            // 
            this.lblConfirmTransferTimeVal.Location = new System.Drawing.Point( 256, 32 );
            this.lblConfirmTransferTimeVal.Name = "lblConfirmTransferTimeVal";
            this.lblConfirmTransferTimeVal.Size = new System.Drawing.Size( 56, 13 );
            this.lblConfirmTransferTimeVal.TabIndex = 0;
            // 
            // lblConfirmTransferTime
            // 
            this.lblConfirmTransferTime.Location = new System.Drawing.Point( 216, 32 );
            this.lblConfirmTransferTime.Name = "lblConfirmTransferTime";
            this.lblConfirmTransferTime.Size = new System.Drawing.Size( 35, 13 );
            this.lblConfirmTransferTime.TabIndex = 0;
            this.lblConfirmTransferTime.Text = "Time:";
            // 
            // lblConfirmTransferDateVal
            // 
            this.lblConfirmTransferDateVal.Location = new System.Drawing.Point( 96, 32 );
            this.lblConfirmTransferDateVal.Name = "lblConfirmTransferDateVal";
            this.lblConfirmTransferDateVal.Size = new System.Drawing.Size( 88, 16 );
            this.lblConfirmTransferDateVal.TabIndex = 0;
            // 
            // lblConfirmTransferDate
            // 
            this.lblConfirmTransferDate.Location = new System.Drawing.Point( 8, 32 );
            this.lblConfirmTransferDate.Name = "lblConfirmTransferDate";
            this.lblConfirmTransferDate.Size = new System.Drawing.Size( 88, 23 );
            this.lblConfirmTransferDate.TabIndex = 0;
            this.lblConfirmTransferDate.Text = "Transfer date:";
            // 
            // panelFromBottomArea
            // 
            this.panelFromBottomArea.BackColor = System.Drawing.Color.White;
            this.panelFromBottomArea.Controls.Add( this.lblHospitalServiceVal );
            this.panelFromBottomArea.Controls.Add( this.lblPatientTypeVal );
            this.panelFromBottomArea.Controls.Add( this.lblAdmitSourceVal );
            this.panelFromBottomArea.Controls.Add( this.lblLocation );
            this.panelFromBottomArea.Controls.Add( this.lblHospitalService );
            this.panelFromBottomArea.Controls.Add( this.lblPatientType );
            this.panelFromBottomArea.Controls.Add( this.lblAdmitSource );
            this.panelFromBottomArea.Controls.Add( this.lblLocationVal );
            this.panelFromBottomArea.Location = new System.Drawing.Point( 16, 192 );
            this.panelFromBottomArea.Name = "panelFromBottomArea";
            this.panelFromBottomArea.Size = new System.Drawing.Size( 336, 96 );
            this.panelFromBottomArea.TabIndex = 0;
            // 
            // lblHospitalServiceVal
            // 
            this.lblHospitalServiceVal.Location = new System.Drawing.Point( 96, 53 );
            this.lblHospitalServiceVal.Name = "lblHospitalServiceVal";
            this.lblHospitalServiceVal.Size = new System.Drawing.Size( 214, 16 );
            this.lblHospitalServiceVal.TabIndex = 0;
            // 
            // lblPatientTypeVal
            // 
            this.lblPatientTypeVal.Location = new System.Drawing.Point( 96, 30 );
            this.lblPatientTypeVal.Name = "lblPatientTypeVal";
            this.lblPatientTypeVal.Size = new System.Drawing.Size( 214, 16 );
            this.lblPatientTypeVal.TabIndex = 0;
            // 
            // lblAdmitSourceVal
            // 
            this.lblAdmitSourceVal.Location = new System.Drawing.Point( 96, 3 );
            this.lblAdmitSourceVal.Name = "lblAdmitSourceVal";
            this.lblAdmitSourceVal.Size = new System.Drawing.Size( 214, 17 );
            this.lblAdmitSourceVal.TabIndex = 0;
            // 
            // lblLocation
            // 
            this.lblLocation.Location = new System.Drawing.Point( 10, 76 );
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size( 85, 17 );
            this.lblLocation.TabIndex = 0;
            this.lblLocation.Text = "Location:";
            // 
            // lblHospitalService
            // 
            this.lblHospitalService.Location = new System.Drawing.Point( 9, 53 );
            this.lblHospitalService.Name = "lblHospitalService";
            this.lblHospitalService.Size = new System.Drawing.Size( 86, 16 );
            this.lblHospitalService.TabIndex = 0;
            this.lblHospitalService.Text = "Hospital service:";
            // 
            // lblPatientType
            // 
            this.lblPatientType.Location = new System.Drawing.Point( 9, 29 );
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size( 86, 17 );
            this.lblPatientType.TabIndex = 0;
            this.lblPatientType.Text = "Patient type:";
            // 
            // lblAdmitSource
            // 
            this.lblAdmitSource.Location = new System.Drawing.Point( 9, 6 );
            this.lblAdmitSource.Name = "lblAdmitSource";
            this.lblAdmitSource.Size = new System.Drawing.Size( 87, 14 );
            this.lblAdmitSource.TabIndex = 0;
            this.lblAdmitSource.Text = "Admit Source:";
            // 
            // lblLocationVal
            // 
            this.lblLocationVal.Location = new System.Drawing.Point( 96, 77 );
            this.lblLocationVal.Name = "lblLocationVal";
            this.lblLocationVal.Size = new System.Drawing.Size( 214, 16 );
            this.lblLocationVal.TabIndex = 0;
            // 
            // panelToArea
            // 
            this.panelToArea.Controls.Add( this.cmbAlternateCareFacility );
            this.panelToArea.Controls.Add( this.lblAlternateCareFacility );
            this.panelToArea.Controls.Add( this.lineLabel2 );
            this.panelToArea.Controls.Add( this.locationView1 );
            this.panelToArea.Controls.Add( this.mtbTransferTime );
            this.panelToArea.Controls.Add( this.cboAdmitSource );
            this.panelToArea.Controls.Add( this.lblAdmitSource2 );
            this.panelToArea.Controls.Add( this.cboClinic1 );
            this.panelToArea.Controls.Add( this.lblHospitalService2 );
            this.panelToArea.Controls.Add( this.mtbTransferDate );
            this.panelToArea.Controls.Add( this.cboPatientType );
            this.panelToArea.Controls.Add( this.lblTime );
            this.panelToArea.Controls.Add( this.dateTimePicker );
            this.panelToArea.Controls.Add( this.lblPatientType2 );
            this.panelToArea.Controls.Add( this.lblTransferDate );
            this.panelToArea.Controls.Add( this.cboHospitalService );
            this.panelToArea.Controls.Add( this.lblClinic1 );
            this.panelToArea.Location = new System.Drawing.Point( 376, 168 );
            this.panelToArea.Name = "panelToArea";
            this.panelToArea.Size = new System.Drawing.Size( 392, 349 );
            this.panelToArea.TabIndex = 0;
            // 
            // cmbAlternateCareFacility
            // 
            this.cmbAlternateCareFacility.DisplayMember = "Description";
            this.cmbAlternateCareFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlternateCareFacility.Location = new System.Drawing.Point( 112, 59 );
            this.cmbAlternateCareFacility.MaxLength = 27;
            this.cmbAlternateCareFacility.Name = "cmbAlternateCareFacility";
            this.cmbAlternateCareFacility.Size = new System.Drawing.Size( 192, 21 );
            this.cmbAlternateCareFacility.TabIndex = 1;
            this.cmbAlternateCareFacility.SelectedIndexChanged += new System.EventHandler( this.cmbAlternateCareFacility_SelectedIndexChanged );
            this.cmbAlternateCareFacility.Validating += new System.ComponentModel.CancelEventHandler( this.cmbAlternateCareFacility_Validating );
            // 
            // lblAlternateCareFacility
            // 
            this.lblAlternateCareFacility.Location = new System.Drawing.Point( 16, 54 );
            this.lblAlternateCareFacility.Name = "lblAlternateCareFacility";
            this.lblAlternateCareFacility.Size = new System.Drawing.Size( 82, 27 );
            this.lblAlternateCareFacility.TabIndex = 66;
            this.lblAlternateCareFacility.Text = "Nursing home/ Alt care facility";
            // 
            // lineLabel2
            // 
            this.lineLabel2.Caption = "Transfer to";
            this.lineLabel2.Location = new System.Drawing.Point( 15, 6 );
            this.lineLabel2.Name = "lineLabel2";
            this.lineLabel2.Size = new System.Drawing.Size( 321, 18 );
            this.lineLabel2.TabIndex = 0;
            this.lineLabel2.TabStop = false;
            // 
            // locationView1
            // 
            this.locationView1.EditFindButtonText = "Find...";
            this.locationView1.EditVerifyButtonText = "Verify";
            this.locationView1.Location = new System.Drawing.Point( 16, 145 );
            this.locationView1.Model = null;
            this.locationView1.Name = "locationView1";
            this.locationView1.Size = new System.Drawing.Size( 328, 133 );
            this.locationView1.TabIndex = 4;
            this.locationView1.BedSelected += new System.EventHandler( this.locationView1_BedSelected );
            // 
            // mtbTransferTime
            // 
            this.mtbTransferTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbTransferTime.KeyPressExpression = "^\\d*$";
            this.mtbTransferTime.Location = new System.Drawing.Point( 272, 313 );
            this.mtbTransferTime.Mask = "  :";
            this.mtbTransferTime.MaxLength = 5;
            this.mtbTransferTime.Multiline = true;
            this.mtbTransferTime.Name = "mtbTransferTime";
            this.mtbTransferTime.Size = new System.Drawing.Size( 37, 20 );
            this.mtbTransferTime.TabIndex = 7;
            this.mtbTransferTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbTransferTime.Validating += new System.ComponentModel.CancelEventHandler( this.mtbTransferTime_Validating );
            // 
            // cboAdmitSource
            // 
            this.cboAdmitSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAdmitSource.Location = new System.Drawing.Point( 112, 30 );
            this.cboAdmitSource.Name = "cboAdmitSource";
            this.cboAdmitSource.Size = new System.Drawing.Size( 192, 21 );
            this.cboAdmitSource.TabIndex = 0; 
            this.cboAdmitSource.SelectedIndexChanged += new System.EventHandler( this.cboAdmitSource_SelectedIndexChanged );
            this.cboAdmitSource.Validating += new System.ComponentModel.CancelEventHandler( this.cmbAdmitSource_Validating );
            // 
            // lblAdmitSource2
            // 
            this.lblAdmitSource2.Location = new System.Drawing.Point( 16, 31 );
            this.lblAdmitSource2.Name = "lblAdmitSource2";
            this.lblAdmitSource2.Size = new System.Drawing.Size( 88, 23 );
            this.lblAdmitSource2.TabIndex = 0;
            this.lblAdmitSource2.Text = "Admit source:";
            // 
            // cboClinic1
            // 
            this.cboClinic1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClinic1.Location = new System.Drawing.Point( 112, 289 );
            this.cboClinic1.Name = "cboClinic1";
            this.cboClinic1.Size = new System.Drawing.Size( 224, 21 );
            this.cboClinic1.TabIndex = 5; 
            this.cboClinic1.SelectedIndexChanged += new System.EventHandler( this.cboClinic1_SelectedIndexChanged );
            // 
            // lblHospitalService2
            // 
            this.lblHospitalService2.Location = new System.Drawing.Point( 16, 115 );
            this.lblHospitalService2.Name = "lblHospitalService2";
            this.lblHospitalService2.Size = new System.Drawing.Size( 88, 23 );
            this.lblHospitalService2.TabIndex = 0;
            this.lblHospitalService2.Text = "Hospital service:";
            // 
            // mtbTransferDate
            // 
            this.mtbTransferDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbTransferDate.KeyPressExpression = "^\\d*$";
            this.mtbTransferDate.Location = new System.Drawing.Point( 112, 313 );
            this.mtbTransferDate.Mask = "  /  /";
            this.mtbTransferDate.MaxLength = 10;
            this.mtbTransferDate.Name = "mtbTransferDate";
            this.mtbTransferDate.Size = new System.Drawing.Size( 66, 20 );
            this.mtbTransferDate.TabIndex = 6;
            this.mtbTransferDate.ValidationExpression = resources.GetString( "mtbTransferDate.ValidationExpression" );
            this.mtbTransferDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbTransferDate_Validating );
            // 
            // cboPatientType
            // 
            this.cboPatientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPatientType.Location = new System.Drawing.Point( 112, 87 );
            this.cboPatientType.Name = "cboPatientType";
            this.cboPatientType.Size = new System.Drawing.Size( 144, 21 );
            this.cboPatientType.TabIndex = 2; 
            this.cboPatientType.SelectedIndexChanged += new System.EventHandler( this.cboPatientType_SelectedIndexChanged );
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point( 226, 317 );
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size( 39, 23 );
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "Time:";
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point( 176, 313 );
            this.dateTimePicker.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size( 21, 20 );
            this.dateTimePicker.TabIndex = 0;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new System.EventHandler( this.dateTimePicker_CloseUp );
            // 
            // lblPatientType2
            // 
            this.lblPatientType2.Location = new System.Drawing.Point( 16, 89 );
            this.lblPatientType2.Name = "lblPatientType2";
            this.lblPatientType2.Size = new System.Drawing.Size( 88, 23 );
            this.lblPatientType2.TabIndex = 0;
            this.lblPatientType2.Text = "Patient type:";
            // 
            // lblTransferDate
            // 
            this.lblTransferDate.Location = new System.Drawing.Point( 16, 316 );
            this.lblTransferDate.Name = "lblTransferDate";
            this.lblTransferDate.Size = new System.Drawing.Size( 88, 23 );
            this.lblTransferDate.TabIndex = 0;
            this.lblTransferDate.Text = "Transfer date:";
            // 
            // cboHospitalService
            // 
            this.cboHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHospitalService.Location = new System.Drawing.Point( 112, 115 );
            this.cboHospitalService.Name = "cboHospitalService";
            this.cboHospitalService.Size = new System.Drawing.Size( 224, 21 );
            this.cboHospitalService.TabIndex = 3; 
            this.cboHospitalService.SelectedIndexChanged += new System.EventHandler( this.cboHospitalService_SelectedIndexChanged );
            // 
            // lblClinic1
            // 
            this.lblClinic1.Location = new System.Drawing.Point( 16, 289 );
            this.lblClinic1.Name = "lblClinic1";
            this.lblClinic1.Size = new System.Drawing.Size( 88, 23 );
            this.lblClinic1.TabIndex = 0;
            this.lblClinic1.Text = "Clinic 1:";
            // 
            // panelActions
            // 
            this.panelActions.Controls.Add( this.btnPrintFaceSheet );
            this.panelActions.Controls.Add( this.btnEditAccount );
            this.panelActions.Controls.Add( this.btnRepeatActivity );
            this.panelActions.Controls.Add( this.btnCloseActivity );
            this.panelActions.Controls.Add( this.lblAction );
            this.panelActions.Location = new System.Drawing.Point( 16, 446 );
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size( 968, 58 );
            this.panelActions.TabIndex = 0;
            // 
            // btnPrintFaceSheet
            // 
            this.btnPrintFaceSheet.Location = new System.Drawing.Point( 336, 31 );
            this.btnPrintFaceSheet.Message = null;
            this.btnPrintFaceSheet.Name = "btnPrintFaceSheet";
            this.btnPrintFaceSheet.Size = new System.Drawing.Size( 125, 23 );
            this.btnPrintFaceSheet.TabIndex = 3;
            this.btnPrintFaceSheet.Text = "Pr&int Face Sheet";
            this.btnPrintFaceSheet.Click += new System.EventHandler( this.btnPrintFaceSheet_Click );
            // 
            // btnEditAccount
            // 
            this.btnEditAccount.Location = new System.Drawing.Point( 204, 31 );
            this.btnEditAccount.Message = null;
            this.btnEditAccount.Name = "btnEditAccount";
            this.btnEditAccount.Size = new System.Drawing.Size( 125, 23 );
            this.btnEditAccount.TabIndex = 2;
            this.btnEditAccount.Text = "Edit/Maintain &Account";
            this.btnEditAccount.Click += new System.EventHandler( this.btnEditAccount_Click );
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.Location = new System.Drawing.Point( 109, 31 );
            this.btnRepeatActivity.Message = null;
            this.btnRepeatActivity.Name = "btnRepeatActivity";
            this.btnRepeatActivity.Size = new System.Drawing.Size( 88, 23 );
            this.btnRepeatActivity.TabIndex = 1;
            this.btnRepeatActivity.Text = "Repeat Acti&vity";
            this.btnRepeatActivity.Click += new System.EventHandler( this.btnRepeatActivity_Click );
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.Location = new System.Drawing.Point( 14, 31 );
            this.btnCloseActivity.Message = null;
            this.btnCloseActivity.Name = "btnCloseActivity";
            this.btnCloseActivity.Size = new System.Drawing.Size( 88, 23 );
            this.btnCloseActivity.TabIndex = 0;
            this.btnCloseActivity.Text = "C&lose Activity";
            this.btnCloseActivity.Click += new System.EventHandler( this.btnCloseActivity_Click );
            // 
            // lblAction
            // 
            this.lblAction.BackColor = System.Drawing.Color.White;
            this.lblAction.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblAction.ForeColor = System.Drawing.Color.Black;
            this.lblAction.Location = new System.Drawing.Point( 12, 7 );
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size( 977, 16 );
            this.lblAction.TabIndex = 0;
            this.lblAction.Text = "Next Action _____________________________________________________________________" +
                "________________________________________________________________________________" +
                "____";
            // 
            // panelRemarks
            // 
            this.panelRemarks.Controls.Add( this.lblRemarks );
            this.panelRemarks.Controls.Add( this.mtbRemarks );
            this.panelRemarks.Location = new System.Drawing.Point( 16, 353 );
            this.panelRemarks.Name = "panelRemarks";
            this.panelRemarks.Size = new System.Drawing.Size( 336, 66 );
            this.panelRemarks.TabIndex = 10;
            // 
            // lblRemarks
            // 
            this.lblRemarks.Location = new System.Drawing.Point( 10, 0 );
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size( 62, 18 );
            this.lblRemarks.TabIndex = 35;
            this.lblRemarks.Text = "Remarks:";
            // 
            // mtbRemarks
            // 
            this.mtbRemarks.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRemarks.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemarks.Location = new System.Drawing.Point( 11, 18 );
            this.mtbRemarks.Mask = "";
            this.mtbRemarks.MaxLength = 60;
            this.mtbRemarks.Multiline = true;
            this.mtbRemarks.Name = "mtbRemarks";
            this.mtbRemarks.Size = new System.Drawing.Size( 305, 48 );
            this.mtbRemarks.TabIndex = 10;
            // 
            // lblAdmitTimeVal
            // 
            this.lblAdmitTimeVal.Location = new System.Drawing.Point( 272, 96 );
            this.lblAdmitTimeVal.Name = "lblAdmitTimeVal";
            this.lblAdmitTimeVal.Size = new System.Drawing.Size( 56, 13 );
            this.lblAdmitTimeVal.TabIndex = 0;
            // 
            // lblAdmitTime
            // 
            this.lblAdmitTime.Location = new System.Drawing.Point( 232, 96 );
            this.lblAdmitTime.Name = "lblAdmitTime";
            this.lblAdmitTime.Size = new System.Drawing.Size( 35, 13 );
            this.lblAdmitTime.TabIndex = 0;
            this.lblAdmitTime.Text = "Time:";
            // 
            // lblFinancialClassVal
            // 
            this.lblFinancialClassVal.Location = new System.Drawing.Point( 112, 144 );
            this.lblFinancialClassVal.Name = "lblFinancialClassVal";
            this.lblFinancialClassVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblFinancialClassVal.TabIndex = 0;
            // 
            // lblPrimaryPlanVal
            // 
            this.lblPrimaryPlanVal.Location = new System.Drawing.Point( 112, 120 );
            this.lblPrimaryPlanVal.Name = "lblPrimaryPlanVal";
            this.lblPrimaryPlanVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblPrimaryPlanVal.TabIndex = 0;
            // 
            // lblAdmitDateVal
            // 
            this.lblAdmitDateVal.Location = new System.Drawing.Point( 112, 96 );
            this.lblAdmitDateVal.Name = "lblAdmitDateVal";
            this.lblAdmitDateVal.Size = new System.Drawing.Size( 88, 16 );
            this.lblAdmitDateVal.TabIndex = 0;
            // 
            // lblAccountVal
            // 
            this.lblAccountVal.Location = new System.Drawing.Point( 112, 72 );
            this.lblAccountVal.Name = "lblAccountVal";
            this.lblAccountVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblAccountVal.TabIndex = 0;
            // 
            // lblPatientNameVal
            // 
            this.lblPatientNameVal.Location = new System.Drawing.Point( 112, 48 );
            this.lblPatientNameVal.Name = "lblPatientNameVal";
            this.lblPatientNameVal.Size = new System.Drawing.Size( 224, 16 );
            this.lblPatientNameVal.TabIndex = 0;
            // 
            // lblFinancialClass
            // 
            this.lblFinancialClass.Location = new System.Drawing.Point( 24, 144 );
            this.lblFinancialClass.Name = "lblFinancialClass";
            this.lblFinancialClass.Size = new System.Drawing.Size( 88, 23 );
            this.lblFinancialClass.TabIndex = 0;
            this.lblFinancialClass.Text = "Financial class:";
            // 
            // lblPrimaryPlan
            // 
            this.lblPrimaryPlan.Location = new System.Drawing.Point( 24, 120 );
            this.lblPrimaryPlan.Name = "lblPrimaryPlan";
            this.lblPrimaryPlan.Size = new System.Drawing.Size( 88, 23 );
            this.lblPrimaryPlan.TabIndex = 0;
            this.lblPrimaryPlan.Text = "Primary plan:";
            // 
            // lblAdmitDate
            // 
            this.lblAdmitDate.Location = new System.Drawing.Point( 24, 96 );
            this.lblAdmitDate.Name = "lblAdmitDate";
            this.lblAdmitDate.Size = new System.Drawing.Size( 88, 23 );
            this.lblAdmitDate.TabIndex = 0;
            this.lblAdmitDate.Text = "Admit date:";
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point( 24, 72 );
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size( 88, 23 );
            this.lblAccount.TabIndex = 0;
            this.lblAccount.Text = "Account:";
            // 
            // lblPatientName
            // 
            this.lblPatientName.Location = new System.Drawing.Point( 24, 48 );
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size( 80, 23 );
            this.lblPatientName.TabIndex = 0;
            this.lblPatientName.Text = "Patient name:";
            // 
            // grpPhysicians
            // 
            this.grpPhysicians.Controls.Add( this.lblPcpVal );
            this.grpPhysicians.Controls.Add( this.lblPcp );
            this.grpPhysicians.Controls.Add( this.lblOprVal );
            this.grpPhysicians.Controls.Add( this.lblAttVal );
            this.grpPhysicians.Controls.Add( this.lblAdmVal );
            this.grpPhysicians.Controls.Add( this.lblRefVal );
            this.grpPhysicians.Controls.Add( this.lblOpr );
            this.grpPhysicians.Controls.Add( this.lblAtt );
            this.grpPhysicians.Controls.Add( this.lblAdm );
            this.grpPhysicians.Controls.Add( this.lblRef );
            this.grpPhysicians.Location = new System.Drawing.Point( 392, 48 );
            this.grpPhysicians.Name = "grpPhysicians";
            this.grpPhysicians.Size = new System.Drawing.Size( 360, 112 );
            this.grpPhysicians.TabIndex = 0;
            this.grpPhysicians.TabStop = false;
            this.grpPhysicians.Text = "Physicians";
            // 
            // lblPcpVal
            // 
            this.lblPcpVal.Location = new System.Drawing.Point( 49, 91 );
            this.lblPcpVal.Name = "lblPcpVal";
            this.lblPcpVal.Size = new System.Drawing.Size( 271, 14 );
            this.lblPcpVal.TabIndex = 0;
            // 
            // lblPcp
            // 
            this.lblPcp.Location = new System.Drawing.Point( 8, 88 );
            this.lblPcp.Name = "lblPcp";
            this.lblPcp.Size = new System.Drawing.Size( 32, 16 );
            this.lblPcp.TabIndex = 0;
            this.lblPcp.Text = "PCP:";
            // 
            // lblOprVal
            // 
            this.lblOprVal.Location = new System.Drawing.Point( 49, 74 );
            this.lblOprVal.Name = "lblOprVal";
            this.lblOprVal.Size = new System.Drawing.Size( 271, 14 );
            this.lblOprVal.TabIndex = 0;
            // 
            // lblAttVal
            // 
            this.lblAttVal.Location = new System.Drawing.Point( 49, 56 );
            this.lblAttVal.Name = "lblAttVal";
            this.lblAttVal.Size = new System.Drawing.Size( 271, 14 );
            this.lblAttVal.TabIndex = 0;
            // 
            // lblAdmVal
            // 
            this.lblAdmVal.Location = new System.Drawing.Point( 49, 40 );
            this.lblAdmVal.Name = "lblAdmVal";
            this.lblAdmVal.Size = new System.Drawing.Size( 271, 14 );
            this.lblAdmVal.TabIndex = 0;
            // 
            // lblRefVal
            // 
            this.lblRefVal.Location = new System.Drawing.Point( 48, 24 );
            this.lblRefVal.Name = "lblRefVal";
            this.lblRefVal.Size = new System.Drawing.Size( 280, 13 );
            this.lblRefVal.TabIndex = 0;
            // 
            // lblOpr
            // 
            this.lblOpr.Location = new System.Drawing.Point( 8, 72 );
            this.lblOpr.Name = "lblOpr";
            this.lblOpr.Size = new System.Drawing.Size( 32, 16 );
            this.lblOpr.TabIndex = 0;
            this.lblOpr.Text = "Opr:";
            // 
            // lblAtt
            // 
            this.lblAtt.Location = new System.Drawing.Point( 8, 56 );
            this.lblAtt.Name = "lblAtt";
            this.lblAtt.Size = new System.Drawing.Size( 24, 16 );
            this.lblAtt.TabIndex = 0;
            this.lblAtt.Text = "Att:";
            // 
            // lblAdm
            // 
            this.lblAdm.Location = new System.Drawing.Point( 8, 40 );
            this.lblAdm.Name = "lblAdm";
            this.lblAdm.Size = new System.Drawing.Size( 32, 16 );
            this.lblAdm.TabIndex = 0;
            this.lblAdm.Text = "Adm:";
            // 
            // lblRef
            // 
            this.lblRef.Location = new System.Drawing.Point( 8, 24 );
            this.lblRef.Name = "lblRef";
            this.lblRef.Size = new System.Drawing.Size( 32, 16 );
            this.lblRef.TabIndex = 0;
            this.lblRef.Text = "Ref:";
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point( 8, 8 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size( 984, 496 );
            this.progressPanel1.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point( 0, 0 );
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size( 100, 23 );
            this.label13.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.Location = new System.Drawing.Point( 864, 592 );
            this.btnOK.Message = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size( 72, 23 );
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler( this.btnOK_Click );
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point( 944, 592 );
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 72, 23 );
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // panelUserContext
            // 
            this.panelUserContext.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
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
            this.userContextView1.Description = "Transfer Inpatient to Outpatient";
            this.userContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.userContextView1.Model = null;
            this.userContextView1.Name = "userContextView1";
            this.userContextView1.Size = new System.Drawing.Size( 1024, 23 );
            this.userContextView1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.patientContextView1 );
            this.panel1.Location = new System.Drawing.Point( 8, 32 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 1008, 24 );
            this.panel1.TabIndex = 0;
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
            // 
            // TransferInPatToOutPatView
            // 
            this.AcceptButton = this.btnOK;
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.panelTransferInPatToOutPat );
            this.Controls.Add( this.panelUserContext );
            this.Controls.Add( this.btnCancel );
            this.Controls.Add( this.btnOK );
            this.Name = "TransferInPatToOutPatView";
            this.Size = new System.Drawing.Size( 1024, 632 );
            this.Load += new System.EventHandler( this.TransferInPatToOutPatView_Load );
            this.Disposed += new System.EventHandler( this.TransferInPatToOutPatView_Disposed );
            this.panelTransferInPatToOutPat.ResumeLayout( false );
            this.gbScanDocuments.ResumeLayout( false );
            this.panelConfirmBottomArea.ResumeLayout( false );
            this.panelFromBottomArea.ResumeLayout( false );
            this.panelToArea.ResumeLayout( false );
            this.panelToArea.PerformLayout();
            this.panelActions.ResumeLayout( false );
            this.panelRemarks.ResumeLayout( false );
            this.panelRemarks.PerformLayout();
            this.grpPhysicians.ResumeLayout( false );
            this.panelUserContext.ResumeLayout( false );
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #region Data Elements

        private IContainer components = null;

        private BackgroundWorker backgroundWorker;
        private RuleEngine i_RuleEngine = null;
        private AdmitSource i_AdmitSourceFrom = new AdmitSource();
        private VisitType i_VisitTypeFrom = new VisitType();
        private ProgressPanel progressPanel1;
        private LocationView locationView1;
        private UserContextView userContextView1;
        private PatientContextView patientContextView1;
        private InfoControl infoControl1;
        private LineLabel lineLabel2;
        private LineLabel llblTransferFrom;

        private MaskedEditTextBox mtbTransferDate;

        private MaskedEditTextBox mtbTransferTime;
        private MaskedEditTextBox mtbRemarks;

        private ClickOnceLoggingButton btnOK;
        private ClickOnceLoggingButton btnCancel;
        private ClickOnceLoggingButton btnEditAccount;
        private LoggingButton btnRepeatActivity;
        private LoggingButton btnCloseActivity;
        private LoggingButton btnPrintFaceSheet;
        private LoggingButton btnScanDocuments;

        private ComboBox cboAdmitSource;
        private ComboBox cboClinic1;
        private ComboBox cboHospitalService;
        private ComboBox cboPatientType;
        private PatientAccessComboBox cmbAlternateCareFacility;

        private GroupBox grpPhysicians;
        private GroupBox gbScanDocuments;

        private DateTimePicker dateTimePicker;

        private Panel panelActions;
        private Panel panelTransferInPatToOutPat;
        private Panel panelToArea;
        private Panel panelFromBottomArea;
        private Panel panelConfirmBottomArea;
        private Panel panelUserContext;
        private Panel panel1;
        private Panel panelRemarks;

        private Label lblPatientName;
        private Label lblAccount;
        private Label lblAdmitDate;
        private Label lblPrimaryPlan;
        private Label lblFinancialClass;
        private Label lblHospitalService;
        private Label lblPatientType;
        private Label lblAdmitSource;
        private Label lblLocation;
        private Label lblPatientNameVal;
        private Label lblAccountVal;
        private Label lblAdmitDateVal;
        private Label lblPrimaryPlanVal;
        private Label lblFinancialClassVal;
        private Label lblAdmitSourceVal;
        private Label lblPatientTypeVal;
        private Label lblHospitalServiceVal;
        private Label lblLocationVal;
        private Label label13;
        private Label lblPcpVal;
        private Label lblPcp;
        private Label lblOprVal;
        private Label lblAttVal;
        private Label lblAdmVal;
        private Label lblRefVal;
        private Label lblOpr;
        private Label lblAtt;
        private Label lblAdm;
        private Label lblRef;
        private Label lblTransferDate;
        private Label lblClinic1;
        private Label lblHospitalService2;
        private Label lblPatientType2;
        private Label lblAdmitSource2;
        private Label lblTime;
        private Label lblAction;
        private Label lblConfirmTransferDateVal;
        private Label lblConfirmTransferDate;
        private Label lblConfirmClinicVal;
        private Label lblConfirmClinic;
        private Label lblScanDocuments;
        private Label lblRemarks;
        private Label lblAlternateCareFacility;

        private Label lblAdmitTime;
        private Label lblAdmitTimeVal;
        private Label lblConfirmTransferTimeVal;
        private Label lblConfirmTransferTime;

        private bool i_PreValidationSuccess = true;

        private DateTime i_FacilityDateTime;

        private EmergencyToInPatientTransferCodeManager emergencyToInpatientTransferCodeManager;

        private IAlternateCareFacilityPresenter alternateCareFacilityPresenter;

        #endregion



        #region Constants
        #endregion

    }
}

