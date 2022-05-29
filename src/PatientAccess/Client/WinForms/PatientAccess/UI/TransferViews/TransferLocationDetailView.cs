using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DocumentImagingViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Reports.FaceSheet;
using Role = Peradigm.Framework.Domain.Security.Role;

namespace PatientAccess.UI.TransferViews
{
    /// <summary>
    /// Summary description for TransferLocationDetailView.
    /// </summary>
    public class TransferLocationDetailView : ControlView
    {
        #region Events
        public event EventHandler RepeatActivity;
        public event EventHandler EditAccount;
        public event EventHandler CloseView;
        #endregion

        #region Event Handlers

        private void TransferLocationDetailView_Load( object sender, EventArgs e )
        {
            btnPrintFaceSheet.Visible = false;
            panelActions.Visible = false;
            gbScanDocuments.Visible = false;
        }

        private void locationView_BedSelected( object sender, EventArgs e )
        {
            i_Location = ( Location )( ( ( LooseArgs )e ).Context );
            Model.Location = i_Location;
            locationView.SetBedBackgroundNormal();

            if ( Model.KindOfVisit.Code == VisitType.INPATIENT )
            {
                LoadAccomodationCodes( i_Location.NursingStation.Code );

                if ( Model.Location.Bed.Accomodation != null )
                {
                    cmbAccomodations.SelectedItem = Model.Location.Bed.Accomodation;

                    if ( !Model.Location.Bed.Accomodation.IsReasonRequiredForSelectedAccommodation() )
                    {
                        cmbReasonForPrivateAccommodation.Enabled = false;
                        UIColors.SetNormalBgColor( cmbReasonForPrivateAccommodation );
                    }
                }
                else
                {
                    cmbReasonForPrivateAccommodation.Enabled = false;
                    UIColors.SetNormalBgColor( cmbReasonForPrivateAccommodation );
                }

                cmbAccomodations.Enabled = true;

                UIColors.SetNormalBgColor( locationView.field_AssignedBed );
                UIColors.SetNormalBgColor( cmbAccomodations );

                RuleEngine.GetInstance().EvaluateRule( typeof( LocationRequired ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( AccomodationRequired ), Model );
            }
            else // for other patient types, no need for Accomodation Codes
            {
                cmbAccomodations.Items.Clear();
                cmbAccomodations.Enabled = false;
            }
            RunRules();
            btnOk.Enabled = ValidRequiredFields();
        }

        private void dateTimePicker_CloseUp( object sender, EventArgs e )
        {
            mtbTransferDate.UnMaskedText = CommonFormatting.MaskedDateFormat( dateTimePicker.Value );
            UIColors.SetNormalBgColor( mtbTransferDate );

            if ( mtbTransferDate.BackColor != UIColors.TextFieldBackgroundError
                && mtbTransferTime.BackColor != UIColors.TextFieldBackgroundError )
            {
                Model.TransferDate = GetTransferDateTime();
                RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateRequired ), Model );

                btnOk.Enabled = ValidRequiredFields();
            }
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            OnCancelView();
        }

        private void btnCloseActivity_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            OnCloseView();
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

        private bool validateDates( string field )
        {
            if ( TransferService.IsTransferDateValid( mtbTransferDate ) )
            {
                if ( !TransferService.IsFutureTransferDate( mtbTransferDate,
                    mtbTransferTime,
                    FacilityDateTime,
                    field ) )
                {
                    if ( !TransferService.IsTransferDateBeforeAdmitDate( mtbTransferDate,
                        mtbTransferTime,
                        Model.AdmitDate,
                        field, false ) )
                    {
                        if ( DateValidator.IsValidTime( mtbTransferTime ) )
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void BeforeWork()
        {
            Cursor = Cursors.WaitCursor;

            progressPanel1.Visible = true;
            progressPanel1.BringToFront();

            panelTransferTo.Visible = false;
            btnOk.Visible = false;
            btnCancel.Visible = false;
            lineLabel_TransferFrom.Visible = false;
        }

        private void DoOk( object sender, DoWorkEventArgs e )
        {
            if ( RuleEngine.AccountHasFailedError() )
            {
                e.Cancel = true;
                return;
            }

            if ( !IsValidLocation() )
            {
                e.Cancel = true;
                btnOk.Enabled = true;
                return;
            }

            if ( isDatesValid )
            {
                UpdateModel();
                TransferService.QueueTransfer( Model );
            }

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
                panelTransferTo.Visible = true;
                btnOk.Visible = true;
                btnOk.Enabled = true;
                btnCancel.Visible = true;
                btnCancel.Enabled = true;
                lineLabel_TransferFrom.Visible = true;

                progressPanel1.Visible = false;
                progressPanel1.SendToBack();
                panel1.SendToBack();

                panelTransferInPatToOutPat.BringToFront();
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
            }
            else
            {
                // success
                lblFromPatientTypeVal.Text = Model.KindOfVisit.DisplayString;
                lblFromHospitalServiceVal.Text = lblFromHospitalServiceVal.Text = Model.HospitalService.DisplayString;
                lblFromLocationVal.Text = Model.Location.DisplayString;

                if ( Model.Location.Bed != null
                    && Model.Location.Bed.Accomodation != null )
                {
                    lblAccommodationVal.Text = Model.Location.Bed.Accomodation.DisplayString;
                }
                else
                {
                    lblAccommodationVal.Text = string.Empty;
                }

                lblTransDateVal.Text = mtbTransferDate.Text;
                lblTransTimeVal.Text = mtbTransferTime.Text;

                btnCloseActivity.Focus();

                lblTransDate.Visible = true;
                lblTransDateVal.Visible = true;
                lblTransTime.Visible = true;
                lblTransTimeVal.Visible = true;

                infoControl.DisplayErrorMessage( "Transfer Patient to New Location submitted for processing." );
                userContextView.Description = "Transfer Patient to New Location - Submitted";

                ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this, new LooseArgs( Model ) );

                progressPanel1.Visible = false;
                progressPanel1.SendToBack();
                panelTransferInPatToOutPat.BringToFront();
                panel1.SendToBack();
                btnPrintFaceSheet.Visible = true;
                panelActions.Visible = true;
                panelActions.BringToFront();
                gbScanDocuments.Visible = true;

                // clear any FUS notes that were manually entered so that - if the View FUS notes icon/menu option is selected,
                // the newly added notes do not show twice.

                if ( ViewFactory.Instance.CreateView<PatientAccessView>().Model != null )
                {
                    ( ( Account )ViewFactory.Instance.CreateView<PatientAccessView>().Model ).ClearFusNotes();
                }

                // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
            }

            // post-completion operations...
            Refresh();
            Cursor = Cursors.Default;
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            AccountView.CloseVIweb();
            SetViewForUserRole();
            locationView.SetBedBackgroundNormal();
            isDatesValid = validateDates( "DATE" );

            if( !isDatesValid )
            {
                mtbTransferDate.Focus();
                btnOk.Enabled = true;
                return;
            }

            if ( backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoOk;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
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

        private void mtbTransferDate_Validating( object sender, CancelEventArgs e )
        {
            if ( !dateTimePicker.Focused )
            {
                validateDates( "DATE" );

                if ( mtbTransferDate.BackColor != UIColors.TextFieldBackgroundError
                    && mtbTransferTime.BackColor != UIColors.TextFieldBackgroundError )
                {
                    UpdateTransferDateTimeAndRunRule();
                }
            }
        }

        private void mtbTransferTime_Validating( object sender, CancelEventArgs e )
        {
            validateDates( "TIME" );

            if ( mtbTransferDate.BackColor != UIColors.TextFieldBackgroundError
                && mtbTransferTime.BackColor != UIColors.TextFieldBackgroundError )
            {
                UpdateTransferDateTimeAndRunRule();
            }
        }

        private void PopulateReasonsForPrivateAccommodation()
        {
            ReasonForAccommodation reasonForAccommodation = new ReasonForAccommodation();
            ICollection reasonForAccommodationList = reasonForAccommodation.PopluateReasonForAccommodation();
            cmbReasonForPrivateAccommodation.Items.Clear();
            if ( reasonForAccommodationList.Count > 0 )
            {
                foreach ( ReasonForAccommodation reasonForAccomm in reasonForAccommodationList )
                {
                    cmbReasonForPrivateAccommodation.Items.Add( reasonForAccomm );
                }
            }
        }

        private void cmbAccomodations_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( Model.Location != null && Model.Location.IsLocationAssigned() )
            {
                PrivateRoomConditionCodeHelper privateRoomConditionCode = new PrivateRoomConditionCodeHelper();
                selectedAccomodation = cmbAccomodations.SelectedItem as Accomodation;
                if ( selectedAccomodation != null )
                {
                    privateRoomConditionCode.AddConditionCodeForPrivateRoomMedicallyNecessary( selectedAccomodation, Model );
                    UIColors.SetNormalBgColor( cmbAccomodations );
                    RuleEngine.GetInstance().EvaluateRule( typeof( AccomodationRequired ), Model );
                    if ( Model.IsReasonForAccommodationRequiredForSelectedActivity()
                    && Model.Location.Bed.Accomodation.IsReasonRequiredForSelectedAccommodation() )
                    {
                        cmbReasonForPrivateAccommodation.Enabled = true;
                        PopulateReasonsForPrivateAccommodation();
                    }
                    else
                    {
                        cmbReasonForPrivateAccommodation.Enabled = false;
                        ResetReasonForPrivateAccommodation();
                        Model.Diagnosis.isPrivateAccommodationRequested = false;

                        if ( !Model.Location.Bed.Accomodation.IsPrivateRoomMedicallyNecessary() )
                        {
                            privateRoomConditionCode.RemovePrivateRommConditionCodes( Model );
                        }
                    }
                }
                Model.Location.Bed.Accomodation.IsReasonForAccommodationSelected = false;
                RuleEngine.GetInstance().EvaluateRule( typeof( ReasonForAccomodationRequired ), Model );

                btnOk.Enabled = ValidRequiredFields();
            }
        }

        private void cmbReasonForPrivateAccommodation_SelectedIndexChanged( object sender, EventArgs e )
        {
            PrivateRoomConditionCodeHelper privateRoomConditionCode = new PrivateRoomConditionCodeHelper();
            if ( cmbReasonForPrivateAccommodation.SelectedItem != null )
            {
                privateRoomConditionCode.AddPrivateRoomConditionCode( ( ( ReasonForAccommodation )cmbReasonForPrivateAccommodation.SelectedItem ).Oid, Model );
                UIColors.SetNormalBgColor( cmbReasonForPrivateAccommodation );
            }
            btnOk.Enabled = ValidRequiredFields();
        }

        private void cmbReasonForPrivateAccommodation_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( cmbReasonForPrivateAccommodation );
            Refresh();
            RuleEngine.GetInstance().EvaluateRule( typeof( ReasonForAccomodationRequired ), Model );
            btnOk.Enabled = ValidRequiredFields();
        }

        private void cmbAccomodations_Validating( object sender, CancelEventArgs e )
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( AccomodationRequired ), Model );
        }

        private void cmbHospitalService_SelectedIndexChanged( object sender, EventArgs e )
        {
            locationView.SetBedBackgroundNormal();

            if ( cmbHospitalService.SelectedIndex > 0 )
            {
                Model.HospitalService = ( HospitalService )cmbHospitalService.SelectedItem;
                locationView.Reset();
                Model.Location = new Location();
                i_Location = new Location();
                if (CheckLocationBoxCanBeEnabled())
                {
                    locationView.EnableLocationControls();
                }
                ResetAccomodations();
                ResetReasonForPrivateAccommodation();
            }
            else
            {
                Model.HospitalService = null;
                locationView.DisableLocationControls();
            }

            RunRules();
            btnOk.Enabled = ValidRequiredFields();
        }

        private void cmbAccomodations_DropDown( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( cmbAccomodations );
        }

        private void HospitalServiceRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbHospitalService );
        }

        private void LocationRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( locationView.field_AssignedBed );
        }

        private void AccomodationRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbAccomodations );
        }

        private void TransferDateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbTransferDate );
        }


        private void ReasonForAccomodationRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbReasonForPrivateAccommodation );
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
        }

        public override void UpdateView()
        {
            RegisterRulesEvents();
            BackUpFromValues();

            if ( Model != null )
            {
                if ( Model.Patient != null )
                {
                    patientContextView1.Model = Model.Patient;
                    patientContextView1.Account = Model;
                    patientContextView1.UpdateView();

                    lblPatientNameVal.Text = Model.Patient.Name.ToString();
                    lblPatientNameVal.Text = Model.Patient.FormattedName;
                }

                lblAccountVal.Text = Model.AccountNumber.ToString();
                lblAdmitDateVal.Text = CommonFormatting.LongDateFormat( Model.AdmitDate );
                lblAdmitTimeVal.Text = CommonFormatting.DisplayedTimeFormat( Model.AdmitDate );

                if ( Model.KindOfVisit != null )
                {
                    lblFromPatientTypeVal.Text = Model.KindOfVisit.DisplayString;

                    DisplayTransferToPatientType();
                }

                if ( Model.TransferredFromHospitalService != null )
                {
                    lblFromHospitalServiceVal.Text = Model.TransferredFromHospitalService.DisplayString;
                }

                DisplayHsvCodes();
                i_FromLocation = Model.LocationFrom;
                if ( Model.LocationFrom != null )
                {
                    lblFromLocationVal.Text = Model.LocationFrom.DisplayString;

                    LoadAccomodationCodes( Model.LocationFrom.NursingStation.Code );
                }

                locationView.Model = Model;
                locationView.UpdateView();

                TransferService.PopulateDefaultTransferDateTime( mtbTransferDate, mtbTransferTime, FacilityDateTime );
                Model.TransferDate = GetTransferDateTime();

                if ( Model.LocationFrom != null )
                {
                    if ( Model.LocationFrom.Bed != null )
                    {
                        if ( Model.LocationFrom.Bed.Accomodation != null )
                        {
                            lblAccommodationVal.Text = Model.LocationFrom.Bed.Accomodation.Code + " " +
                            Model.LocationFrom.Bed.Accomodation.Description;
                        }
                    }
                }

                cmbHospitalService.Focus();
                if ( !CheckLocationBoxCanBeEnabled() )
                {
                    locationView.DisableLocationControls();
                }
                if ( Model.KindOfVisit != null && Model.KindOfVisit.Code == VisitType.INPATIENT &&
                    Model.LocationFrom != null && Model.Location != null &&
                    Model.Location.Bed != null )
                {
                    cmbAccomodations.Enabled = true;
                }
            }
            PrePopulateTransferFromValuesForInpatient();
            btnOk.Enabled = ValidRequiredFields();
            if (Model.DischargeDate != DateTime.MinValue)
            {
                infoControl.DisplayErrorMessage(UIErrorMessages.TRANSFER_HAS_DISCHARGEDATE);
                DisableControls();
                btnOk.Enabled = false;
            }
            else
            {
                RunRules();
            }
        }
       
        public override void UpdateModel()
        {
            if ( i_VisitType != null )
            {
                Model.KindOfVisit = i_VisitType;
            }

            Model.LocationTo = Model.Location;
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
            private get
            {
                return i_Model;
            }
            set
            {
                i_Model = value;
                if ( i_Model != null )
                {
                    OriginalVisitType = Model.KindOfVisit;
                }
                if ( Model.KindOfVisit != null && Model.KindOfVisit.Equals( VisitType.EMERGENCY_PATIENT ) )
                {
                    Model.KindOfVisit = new VisitType
                        ( PersistentModel.NEW_OID
                        , FacilityDateTime
                        , VisitType.OUTPATIENT_DESC
                        , VisitType.OUTPATIENT
                        );
                }
            }
        }

        #endregion

        #region Private Methods

        private bool IsValidLocation()
        {
            if ( Model.KindOfVisit.Code == VisitType.INPATIENT )
            {
                if ( !IsInpatientLocationValid() )
                {
                    return false;
                }
            }
            else
            {
                if ( !IsOutpatientLocationValid() )
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsInpatientLocationValid()
        {
            if ( i_FromLocation != null &&
                i_Location != null &&
                i_FromLocation.Bed != null &&
                i_FromLocation.Bed.Accomodation != null &&
                i_Location.Bed != null &&
                i_Location.Bed.Accomodation != null &&
                i_FromLocation.FormattedLocation == i_Location.FormattedLocation &&
                i_FromLocation.Bed.Accomodation.Code == i_Location.Bed.Accomodation.Code )
            {
                MessageBox.Show( UIErrorMessages.TRANSFER_IP_FROM_LOC_EQUALS_TO_LOC, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1 );
                locationView.SedBedBackgroundError();
                btnPrintFaceSheet.Visible = false;
                panelActions.Visible = false;
                gbScanDocuments.Visible = false;

                return false;
            }

            return true;
        }

        private bool IsOutpatientLocationValid()
        {
            if ( i_FromLocation != null &&
                i_Location != null &&
                i_FromLocation.FormattedLocation == i_Location.FormattedLocation
                && i_FromLocation.FormattedLocation.Trim() != string.Empty )
            {
                MessageBox.Show( UIErrorMessages.TRANSFER_OP_FROM_LOC_EQUALS_TO_LOC, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1 );
                locationView.SedBedBackgroundError();
                btnPrintFaceSheet.Visible = false;
                panelActions.Visible = false;
                gbScanDocuments.Visible = false;

                return false;
            }
            return true;
        }

        private void OnCancelView()
        {
            if ( AccountActivityService.ConfirmCancelActivity(
                this, new LooseArgs( Model ) ) )
            {
                CancelBackgroundWorker();

                if ( CloseView != null )
                {
                    CloseView( this, new EventArgs() );
                }
            }
            else
            {
                btnCancel.Enabled = true;
            }
        }

        private void CancelBackgroundWorker()
        {
            // cancel the background worker here...
            if ( backgroundWorker != null )
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void OnCloseView()
        {
            CancelBackgroundWorker();

            if ( CloseView != null )
            {
                CloseView( this, new EventArgs() );
            }
        }

        private bool ValidRequiredFields()
        {
            if ( cmbHospitalService.BackColor == UIColors.TextFieldBackgroundRequired ||
                locationView.field_AssignedBed.BackColor == UIColors.TextFieldBackgroundRequired ||
                cmbAccomodations.BackColor == UIColors.TextFieldBackgroundRequired ||
                mtbTransferDate.BackColor == UIColors.TextFieldBackgroundRequired ||
                mtbTransferDate.BackColor == UIColors.TextFieldBackgroundError ||
                mtbTransferTime.BackColor == UIColors.TextFieldBackgroundError ||
                cmbReasonForPrivateAccommodation.BackColor == UIColors.TextFieldBackgroundRequired )
            {
                return false;
            }
            if (!HasChangedForRequiredFields)
            {
                return false;
            }
            return true;
        }
        private bool HasChangedForRequiredFields
        {
            get
            {
                if (HasLocationChanged || HasAccomodationCodeChanged)
                {
                    return true;
                }
                return false;
            }
        }

        private bool HasLocationChanged
        {
            get { return Model.LocationFrom.FormattedLocation != Model.Location.FormattedLocation; }
        }
        private bool HasAccomodationCodeChanged
        {
            get
            {
                return (Model.Location.Bed != null) &&
                       Model.AccomodationFrom.Code != Model.Location.Bed.Accomodation.Code;
            }

        }

        private void DisplayTransferToPatientType()
        {
            i_VisitType = Model.KindOfVisit;

            lblToPatientTypeVal.Text = Model.KindOfVisit.DisplayString;
        }

        private void DisplayHsvCodes()
        {
            if ( ( cmbHospitalService.Items.Count == 0 ) && ( Model.KindOfVisit != null ) )
            {
                HSVBrokerProxy brokerProxy = new HSVBrokerProxy();
                ArrayList hsvCodes = new ArrayList();
                if (Model.KindOfVisit.IsInpatient || Model.KindOfVisit.IsEmergencyPatient || Model.HospitalService.IsDayCare())
                {
                    hsvCodes =
                       (ArrayList)
                           brokerProxy.HospitalServicesFor(User.GetCurrent().Facility.Oid, Model.KindOfVisit.Code );
                }
                else
                {
                    hsvCodes =
                        (ArrayList)
                            brokerProxy.HospitalServicesFor(User.GetCurrent().Facility.Oid, Model.KindOfVisit.Code, "Y");
                }
                cmbHospitalService.Items.Clear();
                foreach ( HospitalService hospitalService in hsvCodes )
                {
                    cmbHospitalService.Items.Add( hospitalService );
                }
                cmbHospitalService.Sorted = true;
            }
            if ( Model.HospitalService != null )
            {
                cmbHospitalService.SelectedItem = new HospitalService();
            }

            locationView.Reset();
            Model.Location = new Location();
        }

        private void LoadAccomodationCodes( string nursingStationCode )
        {
            cmbAccomodations.Items.Clear();

            LocationBrokerProxy locationBroker = new LocationBrokerProxy();

            ICollection accomodationCodes = locationBroker.AccomodationCodesFor(
                nursingStationCode, User.GetCurrent().Facility );

            cmbAccomodations.ValueMember = "Value";
            cmbAccomodations.DisplayMember = "Key";

            foreach ( Accomodation accomodation in accomodationCodes )
            {
                cmbAccomodations.Items.Add( accomodation );
            }

        }

        private DateTime GetTransferDateTime()
        {
            string date = mtbTransferDate.Text;
            string time = mtbTransferTime.UnMaskedText != string.Empty ? mtbTransferTime.Text : "00:00";

            return mtbTransferDate.UnMaskedText == string.Empty ? DateTime.MinValue : Convert.ToDateTime( date + " " + time );
        }

        private void UpdateTransferDateTimeAndRunRule()
        {
            Model.TransferDate = GetTransferDateTime();
            UIColors.SetNormalBgColor( mtbTransferDate );

            RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateRequired ), Model );
        }

        private void DisableControls()
        {
            panelTransferTo.Enabled = false;
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

        private bool CheckLocationBoxCanBeEnabled()
        {
            if ( i_VisitType != null &&
                cmbHospitalService.SelectedIndex != -1 )
            {
                HospitalService hsvCode = ( HospitalService )cmbHospitalService.SelectedItem;
                if ( hsvCode != null )
                {
                    return i_VisitType.CanHaveBedFor( hsvCode );
                }
            }
            return false;
        }

        private void BackUpFromValues()
        {
            Model.TransferredFromHospitalService = ( HospitalService )Model.HospitalService.Clone();
            var location = ( Location )Model.Location.Clone();
            Model.LocationFrom = location;
            Model.AccomodationFrom = location.Bed.Accomodation;
        }

        private void RegisterRulesEvents()
        {
            RuleEngine.LoadRules( Model );

            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceRequired ), Model, new EventHandler( HospitalServiceRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( LocationRequired ), Model, new EventHandler( LocationRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AccomodationRequired ), Model, new EventHandler( AccomodationRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( TransferDateRequired ), Model, new EventHandler( TransferDateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( ReasonForAccomodationRequired ), Model, new EventHandler( ReasonForAccomodationRequiredEventHandler ) );
        }

        private void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds          
            UIColors.SetNormalBgColor( cmbHospitalService );
            UIColors.SetNormalBgColor( locationView.field_AssignedBed );
            UIColors.SetNormalBgColor( cmbAccomodations );
            UIColors.SetNormalBgColor( mtbTransferDate );

            Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( LocationRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( AccomodationRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateRequired ), Model );
        }

        private void ResetAccomodations()
        {
            cmbAccomodations.SelectedIndex = -1;
            UIColors.SetNormalBgColor( cmbAccomodations );
            cmbAccomodations.Enabled = false;
        }

        private void ResetReasonForPrivateAccommodation()
        {
            cmbReasonForPrivateAccommodation.SelectedIndex = -1;
            UIColors.SetNormalBgColor( cmbReasonForPrivateAccommodation );
            cmbReasonForPrivateAccommodation.Enabled = false;
        }
        private void PrePopulateTransferFromValuesForInpatient()
        {
            if (Model.KindOfVisit.IsInpatient)
            {
                PrePopulateTransferFromValues();
            }
        }
        private void PrePopulateTransferFromValues()
        {
            this.cmbHospitalService.SelectedItem = Model.TransferredFromHospitalService;
            this.Model.Location = this.Model.LocationFrom;
            locationView.Model = Model;
            locationView.UpdateView();
            this.cmbAccomodations.SelectedItem = Model.LocationFrom.Bed.Accomodation;
            this.cmbAccomodations.Enabled = true;
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TransferLocationDetailView ) );
            this.userContextView = new PatientAccess.UI.UserContextView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnOk = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.panel_MainPadding = new System.Windows.Forms.Panel();
            this.panelTransferInPatToOutPat = new System.Windows.Forms.Panel();
            this.gbScanDocuments = new System.Windows.Forms.GroupBox();
            this.btnScanDocuments = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblScanDocuments = new System.Windows.Forms.Label();
            this.lblTransTimeVal = new System.Windows.Forms.Label();
            this.lblTransTime = new System.Windows.Forms.Label();
            this.lblTransDateVal = new System.Windows.Forms.Label();
            this.lblTransDate = new System.Windows.Forms.Label();
            this.lblAccommodationVal = new System.Windows.Forms.Label();
            this.lblAccommodation = new System.Windows.Forms.Label();
            this.panelActions = new System.Windows.Forms.Panel();
            this.btnPrintFaceSheet = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnEditAccount = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnRepeatActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCloseActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblAction = new System.Windows.Forms.Label();
            this.lblAdmitTimeVal = new System.Windows.Forms.Label();
            this.lblAdmitTime = new System.Windows.Forms.Label();
            this.infoControl = new PatientAccess.UI.CommonControls.InfoControl();
            this.lineLabel_TransferFrom = new PatientAccess.UI.CommonControls.LineLabel();
            this.lblFromLocationVal = new System.Windows.Forms.Label();
            this.lblFromHospitalServiceVal = new System.Windows.Forms.Label();
            this.lblFromPatientTypeVal = new System.Windows.Forms.Label();
            this.lblAdmitDateVal = new System.Windows.Forms.Label();
            this.lblAccountVal = new System.Windows.Forms.Label();
            this.lblPatientNameVal = new System.Windows.Forms.Label();
            this.lblFromLocation = new System.Windows.Forms.Label();
            this.lblFromHospitalService = new System.Windows.Forms.Label();
            this.lblFromPatientType = new System.Windows.Forms.Label();
            this.lblAdmitDate = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.panelTransferTo = new System.Windows.Forms.Panel();
            this.cmbReasonForPrivateAccommodation = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTransferDate = new System.Windows.Forms.Label();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.lblToHospitalService = new System.Windows.Forms.Label();
            this.mtbTransferTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblToPatientType = new System.Windows.Forms.Label();
            this.lblToPatientTypeVal = new System.Windows.Forms.Label();
            this.cmbAccomodations = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lineLabel1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.mtbTransferDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblAccomodation = new System.Windows.Forms.Label();
            this.cmbHospitalService = new System.Windows.Forms.ComboBox();
            this.locationView = new PatientAccess.UI.CommonControls.LocationView();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.panel_TopSpacer1 = new System.Windows.Forms.Panel();
            this.panelPatientContext = new System.Windows.Forms.Panel();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.panel1.SuspendLayout();
            this.panel_MainPadding.SuspendLayout();
            this.panelTransferInPatToOutPat.SuspendLayout();
            this.gbScanDocuments.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelTransferTo.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView
            // 
            this.userContextView.BackColor = System.Drawing.SystemColors.Control;
            this.userContextView.Description = "Transfer Patient to New Location";
            this.userContextView.Dock = System.Windows.Forms.DockStyle.Top;
            this.userContextView.Location = new System.Drawing.Point( 0, 0 );
            this.userContextView.Model = null;
            this.userContextView.Name = "userContextView";
            this.userContextView.Size = new System.Drawing.Size( 1024, 23 );
            this.userContextView.TabIndex = 0;
            this.userContextView.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.btnCancel );
            this.panel1.Controls.Add( this.btnOk );
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point( 0, 588 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 1024, 32 );
            this.panel1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point( 937, 4 );
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 24 );
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point( 851, 4 );
            this.btnOk.Message = null;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size( 75, 24 );
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler( this.btnOk_Click );
            // 
            // panel_MainPadding
            // 
            this.panel_MainPadding.Controls.Add( this.panelTransferInPatToOutPat );
            this.panel_MainPadding.Controls.Add( this.panel_TopSpacer1 );
            this.panel_MainPadding.Controls.Add( this.panelPatientContext );
            this.panel_MainPadding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_MainPadding.Location = new System.Drawing.Point( 0, 23 );
            this.panel_MainPadding.Name = "panel_MainPadding";
            this.panel_MainPadding.Padding = new System.Windows.Forms.Padding( 9, 9, 9, 0 );
            this.panel_MainPadding.Size = new System.Drawing.Size( 1024, 565 );
            this.panel_MainPadding.TabIndex = 0;
            // 
            // panelTransferInPatToOutPat
            // 
            this.panelTransferInPatToOutPat.BackColor = System.Drawing.Color.White;
            this.panelTransferInPatToOutPat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTransferInPatToOutPat.Controls.Add( this.gbScanDocuments );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblTransTimeVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblTransTime );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblTransDateVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblTransDate );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAccommodationVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAccommodation );
            this.panelTransferInPatToOutPat.Controls.Add( this.panelActions );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAdmitTimeVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAdmitTime );
            this.panelTransferInPatToOutPat.Controls.Add( this.infoControl );
            this.panelTransferInPatToOutPat.Controls.Add( this.lineLabel_TransferFrom );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblFromLocationVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblFromHospitalServiceVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblFromPatientTypeVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAdmitDateVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAccountVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblPatientNameVal );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblFromLocation );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblFromHospitalService );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblFromPatientType );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAdmitDate );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblAccount );
            this.panelTransferInPatToOutPat.Controls.Add( this.lblPatientName );
            this.panelTransferInPatToOutPat.Controls.Add( this.panelTransferTo );
            this.panelTransferInPatToOutPat.Controls.Add( this.progressPanel1 );
            this.panelTransferInPatToOutPat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTransferInPatToOutPat.Location = new System.Drawing.Point( 9, 42 );
            this.panelTransferInPatToOutPat.Name = "panelTransferInPatToOutPat";
            this.panelTransferInPatToOutPat.Padding = new System.Windows.Forms.Padding( 15 );
            this.panelTransferInPatToOutPat.Size = new System.Drawing.Size( 1006, 523 );
            this.panelTransferInPatToOutPat.TabIndex = 0;
            // 
            // gbScanDocuments
            // 
            this.gbScanDocuments.Controls.Add( this.btnScanDocuments );
            this.gbScanDocuments.Controls.Add( this.lblScanDocuments );
            this.gbScanDocuments.Location = new System.Drawing.Point( 15, 315 );
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
            // lblTransTimeVal
            // 
            this.lblTransTimeVal.Location = new System.Drawing.Point( 254, 283 );
            this.lblTransTimeVal.Name = "lblTransTimeVal";
            this.lblTransTimeVal.Size = new System.Drawing.Size( 56, 13 );
            this.lblTransTimeVal.TabIndex = 0;
            this.lblTransTimeVal.Visible = false;
            // 
            // lblTransTime
            // 
            this.lblTransTime.Location = new System.Drawing.Point( 204, 283 );
            this.lblTransTime.Name = "lblTransTime";
            this.lblTransTime.Size = new System.Drawing.Size( 35, 13 );
            this.lblTransTime.TabIndex = 0;
            this.lblTransTime.Text = "Time:";
            this.lblTransTime.Visible = false;
            // 
            // lblTransDateVal
            // 
            this.lblTransDateVal.Location = new System.Drawing.Point( 101, 283 );
            this.lblTransDateVal.Name = "lblTransDateVal";
            this.lblTransDateVal.Size = new System.Drawing.Size( 89, 15 );
            this.lblTransDateVal.TabIndex = 0;
            this.lblTransDateVal.Visible = false;
            // 
            // lblTransDate
            // 
            this.lblTransDate.Location = new System.Drawing.Point( 11, 283 );
            this.lblTransDate.Name = "lblTransDate";
            this.lblTransDate.Size = new System.Drawing.Size( 78, 15 );
            this.lblTransDate.TabIndex = 0;
            this.lblTransDate.Text = "Transfer Date:";
            this.lblTransDate.Visible = false;
            // 
            // lblAccommodationVal
            // 
            this.lblAccommodationVal.Location = new System.Drawing.Point( 101, 236 );
            this.lblAccommodationVal.Name = "lblAccommodationVal";
            this.lblAccommodationVal.Size = new System.Drawing.Size( 203, 15 );
            this.lblAccommodationVal.TabIndex = 0;
            // 
            // lblAccommodation
            // 
            this.lblAccommodation.Location = new System.Drawing.Point( 11, 236 );
            this.lblAccommodation.Name = "lblAccommodation";
            this.lblAccommodation.Size = new System.Drawing.Size( 89, 15 );
            this.lblAccommodation.TabIndex = 0;
            this.lblAccommodation.Text = "Accommodation:";
            // 
            // panelActions
            // 
            this.panelActions.Controls.Add( this.btnPrintFaceSheet );
            this.panelActions.Controls.Add( this.btnEditAccount );
            this.panelActions.Controls.Add( this.btnRepeatActivity );
            this.panelActions.Controls.Add( this.btnCloseActivity );
            this.panelActions.Controls.Add( this.lblAction );
            this.panelActions.Location = new System.Drawing.Point( 3, 442 );
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size( 998, 69 );
            this.panelActions.TabIndex = 0;
            // 
            // btnPrintFaceSheet
            // 
            this.btnPrintFaceSheet.Location = new System.Drawing.Point( 329, 40 );
            this.btnPrintFaceSheet.Message = null;
            this.btnPrintFaceSheet.Name = "btnPrintFaceSheet";
            this.btnPrintFaceSheet.Size = new System.Drawing.Size( 125, 23 );
            this.btnPrintFaceSheet.TabIndex = 4;
            this.btnPrintFaceSheet.Text = "Pr&int Face Sheet";
            this.btnPrintFaceSheet.Click += new System.EventHandler( this.btnPrintFaceSheet_Click );
            // 
            // btnEditAccount
            // 
            this.btnEditAccount.Location = new System.Drawing.Point( 197, 40 );
            this.btnEditAccount.Message = null;
            this.btnEditAccount.Name = "btnEditAccount";
            this.btnEditAccount.Size = new System.Drawing.Size( 125, 23 );
            this.btnEditAccount.TabIndex = 3;
            this.btnEditAccount.Text = "Edit/Maintain &Account";
            this.btnEditAccount.Click += new System.EventHandler( this.btnEditAccount_Click );
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.Location = new System.Drawing.Point( 102, 40 );
            this.btnRepeatActivity.Message = null;
            this.btnRepeatActivity.Name = "btnRepeatActivity";
            this.btnRepeatActivity.Size = new System.Drawing.Size( 88, 23 );
            this.btnRepeatActivity.TabIndex = 2;
            this.btnRepeatActivity.Text = "Repeat Acti&vity";
            this.btnRepeatActivity.Click += new System.EventHandler( this.btnRepeatActivity_Click );
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.CausesValidation = false;
            this.btnCloseActivity.Location = new System.Drawing.Point( 7, 40 );
            this.btnCloseActivity.Message = null;
            this.btnCloseActivity.Name = "btnCloseActivity";
            this.btnCloseActivity.Size = new System.Drawing.Size( 88, 23 );
            this.btnCloseActivity.TabIndex = 1;
            this.btnCloseActivity.Text = "C&lose Activity";
            this.btnCloseActivity.Click += new System.EventHandler( this.btnCloseActivity_Click );
            // 
            // lblAction
            // 
            this.lblAction.BackColor = System.Drawing.Color.White;
            this.lblAction.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblAction.ForeColor = System.Drawing.Color.Black;
            this.lblAction.Location = new System.Drawing.Point( 9, 10 );
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size( 986, 16 );
            this.lblAction.TabIndex = 0;
            this.lblAction.Text = "Next Action _____________________________________________________________________" +
                "________________________________________________________________________________" +
                "____";
            // 
            // lblAdmitTimeVal
            // 
            this.lblAdmitTimeVal.Location = new System.Drawing.Point( 254, 106 );
            this.lblAdmitTimeVal.Name = "lblAdmitTimeVal";
            this.lblAdmitTimeVal.Size = new System.Drawing.Size( 56, 13 );
            this.lblAdmitTimeVal.TabIndex = 0;
            // 
            // lblAdmitTime
            // 
            this.lblAdmitTime.Location = new System.Drawing.Point( 204, 106 );
            this.lblAdmitTime.Name = "lblAdmitTime";
            this.lblAdmitTime.Size = new System.Drawing.Size( 35, 13 );
            this.lblAdmitTime.TabIndex = 0;
            this.lblAdmitTime.Text = "Time:";
            // 
            // infoControl
            // 
            this.infoControl.Location = new System.Drawing.Point( 12, 10 );
            this.infoControl.Message = "";
            this.infoControl.Name = "infoControl";
            this.infoControl.Size = new System.Drawing.Size( 977, 35 );
            this.infoControl.TabIndex = 0;
            this.infoControl.TabStop = false;
            // 
            // lineLabel_TransferFrom
            // 
            this.lineLabel_TransferFrom.Caption = "Transfer from";
            this.lineLabel_TransferFrom.Location = new System.Drawing.Point( 11, 132 );
            this.lineLabel_TransferFrom.Name = "lineLabel_TransferFrom";
            this.lineLabel_TransferFrom.Size = new System.Drawing.Size( 356, 15 );
            this.lineLabel_TransferFrom.TabIndex = 0;
            this.lineLabel_TransferFrom.TabStop = false;
            // 
            // lblFromLocationVal
            // 
            this.lblFromLocationVal.Location = new System.Drawing.Point( 101, 210 );
            this.lblFromLocationVal.Name = "lblFromLocationVal";
            this.lblFromLocationVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblFromLocationVal.TabIndex = 0;
            // 
            // lblFromHospitalServiceVal
            // 
            this.lblFromHospitalServiceVal.Location = new System.Drawing.Point( 101, 184 );
            this.lblFromHospitalServiceVal.Name = "lblFromHospitalServiceVal";
            this.lblFromHospitalServiceVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblFromHospitalServiceVal.TabIndex = 0;
            // 
            // lblFromPatientTypeVal
            // 
            this.lblFromPatientTypeVal.Location = new System.Drawing.Point( 101, 158 );
            this.lblFromPatientTypeVal.Name = "lblFromPatientTypeVal";
            this.lblFromPatientTypeVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblFromPatientTypeVal.TabIndex = 0;
            // 
            // lblAdmitDateVal
            // 
            this.lblAdmitDateVal.Location = new System.Drawing.Point( 101, 106 );
            this.lblAdmitDateVal.Name = "lblAdmitDateVal";
            this.lblAdmitDateVal.Size = new System.Drawing.Size( 90, 16 );
            this.lblAdmitDateVal.TabIndex = 0;
            // 
            // lblAccountVal
            // 
            this.lblAccountVal.Location = new System.Drawing.Point( 101, 80 );
            this.lblAccountVal.Name = "lblAccountVal";
            this.lblAccountVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblAccountVal.TabIndex = 0;
            // 
            // lblPatientNameVal
            // 
            this.lblPatientNameVal.Location = new System.Drawing.Point( 101, 54 );
            this.lblPatientNameVal.Name = "lblPatientNameVal";
            this.lblPatientNameVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblPatientNameVal.TabIndex = 0;
            // 
            // lblFromLocation
            // 
            this.lblFromLocation.Location = new System.Drawing.Point( 11, 210 );
            this.lblFromLocation.Name = "lblFromLocation";
            this.lblFromLocation.Size = new System.Drawing.Size( 88, 15 );
            this.lblFromLocation.TabIndex = 0;
            this.lblFromLocation.Text = "Location:";
            // 
            // lblFromHospitalService
            // 
            this.lblFromHospitalService.Location = new System.Drawing.Point( 11, 184 );
            this.lblFromHospitalService.Name = "lblFromHospitalService";
            this.lblFromHospitalService.Size = new System.Drawing.Size( 88, 15 );
            this.lblFromHospitalService.TabIndex = 0;
            this.lblFromHospitalService.Text = "Hospital service:";
            // 
            // lblFromPatientType
            // 
            this.lblFromPatientType.Location = new System.Drawing.Point( 11, 158 );
            this.lblFromPatientType.Name = "lblFromPatientType";
            this.lblFromPatientType.Size = new System.Drawing.Size( 88, 15 );
            this.lblFromPatientType.TabIndex = 0;
            this.lblFromPatientType.Text = "Patient type:";
            // 
            // lblAdmitDate
            // 
            this.lblAdmitDate.Location = new System.Drawing.Point( 11, 106 );
            this.lblAdmitDate.Name = "lblAdmitDate";
            this.lblAdmitDate.Size = new System.Drawing.Size( 88, 15 );
            this.lblAdmitDate.TabIndex = 0;
            this.lblAdmitDate.Text = "Admit date:";
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point( 11, 80 );
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size( 88, 15 );
            this.lblAccount.TabIndex = 0;
            this.lblAccount.Text = "Account:";
            // 
            // lblPatientName
            // 
            this.lblPatientName.Location = new System.Drawing.Point( 11, 54 );
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size( 113, 15 );
            this.lblPatientName.TabIndex = 0;
            this.lblPatientName.Text = "Patient name:";
            // 
            // panelTransferTo
            // 
            this.panelTransferTo.Controls.Add( this.cmbReasonForPrivateAccommodation );
            this.panelTransferTo.Controls.Add( this.label1 );
            this.panelTransferTo.Controls.Add( this.lblTransferDate );
            this.panelTransferTo.Controls.Add( this.dateTimePicker );
            this.panelTransferTo.Controls.Add( this.lblToHospitalService );
            this.panelTransferTo.Controls.Add( this.mtbTransferTime );
            this.panelTransferTo.Controls.Add( this.lblToPatientType );
            this.panelTransferTo.Controls.Add( this.lblToPatientTypeVal );
            this.panelTransferTo.Controls.Add( this.cmbAccomodations );
            this.panelTransferTo.Controls.Add( this.lineLabel1 );
            this.panelTransferTo.Controls.Add( this.mtbTransferDate );
            this.panelTransferTo.Controls.Add( this.lblTime );
            this.panelTransferTo.Controls.Add( this.lblAccomodation );
            this.panelTransferTo.Controls.Add( this.cmbHospitalService );
            this.panelTransferTo.Controls.Add( this.locationView );
            this.panelTransferTo.Location = new System.Drawing.Point( 405, 117 );
            this.panelTransferTo.Name = "panelTransferTo";
            this.panelTransferTo.Size = new System.Drawing.Size( 398, 332 );
            this.panelTransferTo.TabIndex = 0;
            // 
            // cmbReasonForPrivateAccommodation
            // 
            this.cmbReasonForPrivateAccommodation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReasonForPrivateAccommodation.Enabled = false;
            this.cmbReasonForPrivateAccommodation.Location = new System.Drawing.Point( 128, 253 );
            this.cmbReasonForPrivateAccommodation.Name = "cmbReasonForPrivateAccommodation";
            this.cmbReasonForPrivateAccommodation.Size = new System.Drawing.Size( 217, 21 );
            this.cmbReasonForPrivateAccommodation.TabIndex = 4;
            this.cmbReasonForPrivateAccommodation.Validating += new System.ComponentModel.CancelEventHandler( this.cmbReasonForPrivateAccommodation_Validating );
            this.cmbReasonForPrivateAccommodation.SelectedIndexChanged += new System.EventHandler( this.cmbReasonForPrivateAccommodation_SelectedIndexChanged );
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 27, 253 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 101, 30 );
            this.label1.TabIndex = 64;
            this.label1.Text = "Reason For Private Accommodation:";
            // 
            // lblTransferDate
            // 
            this.lblTransferDate.Location = new System.Drawing.Point( 27, 300 );
            this.lblTransferDate.Name = "lblTransferDate";
            this.lblTransferDate.Size = new System.Drawing.Size( 88, 14 );
            this.lblTransferDate.TabIndex = 0;
            this.lblTransferDate.Text = "Transfer date:";
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point( 198, 300 );
            this.dateTimePicker.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size( 21, 20 );
            this.dateTimePicker.TabIndex = 4;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new System.EventHandler( this.dateTimePicker_CloseUp );
            // 
            // lblToHospitalService
            // 
            this.lblToHospitalService.AutoSize = true;
            this.lblToHospitalService.Location = new System.Drawing.Point( 27, 67 );
            this.lblToHospitalService.Name = "lblToHospitalService";
            this.lblToHospitalService.Size = new System.Drawing.Size( 85, 13 );
            this.lblToHospitalService.TabIndex = 0;
            this.lblToHospitalService.Text = "Hospital service:";
            // 
            // mtbTransferTime
            // 
            this.mtbTransferTime.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbTransferTime.KeyPressExpression = "^([0-2]?|[0-1][0-9]?|[0-1][0-9][0-5]?|[0-1][0-9][0-5][0-9]?|2[0-3]?|2[0-3][0-5]?|" +
                "2[0-3][0-5][0-9]?)$";
            this.mtbTransferTime.Location = new System.Drawing.Point( 293, 300 );
            this.mtbTransferTime.Mask = "  :  ";
            this.mtbTransferTime.MaxLength = 5;
            this.mtbTransferTime.Name = "mtbTransferTime";
            this.mtbTransferTime.Size = new System.Drawing.Size( 37, 20 );
            this.mtbTransferTime.TabIndex = 5;
            this.mtbTransferTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbTransferTime.Validating += new System.ComponentModel.CancelEventHandler( this.mtbTransferTime_Validating );

            // 
            // lblToPatientType
            // 
            this.lblToPatientType.Location = new System.Drawing.Point( 27, 41 );
            this.lblToPatientType.Name = "lblToPatientType";
            this.lblToPatientType.Size = new System.Drawing.Size( 88, 23 );
            this.lblToPatientType.TabIndex = 0;
            this.lblToPatientType.Text = "Patient type:";
            // 
            // lblToPatientTypeVal
            // 
            this.lblToPatientTypeVal.Location = new System.Drawing.Point( 128, 41 );
            this.lblToPatientTypeVal.Name = "lblToPatientTypeVal";
            this.lblToPatientTypeVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblToPatientTypeVal.TabIndex = 0;
            // 
            // cmbAccomodations
            // 
            this.cmbAccomodations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccomodations.Enabled = false;
            this.cmbAccomodations.Location = new System.Drawing.Point( 128, 222 );
            this.cmbAccomodations.Name = "cmbAccomodations";
            this.cmbAccomodations.Size = new System.Drawing.Size( 110, 21 );
            this.cmbAccomodations.TabIndex = 3;
            this.cmbAccomodations.Validating += new System.ComponentModel.CancelEventHandler( this.cmbAccomodations_Validating );
            this.cmbAccomodations.SelectedIndexChanged += new System.EventHandler( this.cmbAccomodations_SelectedIndexChanged );
            this.cmbAccomodations.DropDown += new System.EventHandler( this.cmbAccomodations_DropDown );
            // 
            // lineLabel1
            // 
            this.lineLabel1.Caption = "Transfer to";
            this.lineLabel1.Location = new System.Drawing.Point( 27, 15 );
            this.lineLabel1.Name = "lineLabel1";
            this.lineLabel1.Size = new System.Drawing.Size( 356, 18 );
            this.lineLabel1.TabIndex = 0;
            this.lineLabel1.TabStop = false;
            // 
            // mtbTransferDate
            // 
            this.mtbTransferDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbTransferDate.KeyPressExpression = resources.GetString( "mtbTransferDate.KeyPressExpression" );
            this.mtbTransferDate.Location = new System.Drawing.Point( 128, 300 );
            this.mtbTransferDate.Mask = "  /  /";
            this.mtbTransferDate.MaxLength = 10;
            this.mtbTransferDate.Name = "mtbTransferDate";
            this.mtbTransferDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbTransferDate.TabIndex = 4;
            this.mtbTransferDate.ValidationExpression = resources.GetString( "mtbTransferDate.ValidationExpression" );
            this.mtbTransferDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbTransferDate_Validating );
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point( 248, 300 );
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size( 40, 15 );
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "Time:";
            // 
            // lblAccomodation
            // 
            this.lblAccomodation.AutoSize = true;
            this.lblAccomodation.Location = new System.Drawing.Point( 27, 226 );
            this.lblAccomodation.Name = "lblAccomodation";
            this.lblAccomodation.Size = new System.Drawing.Size( 86, 13 );
            this.lblAccomodation.TabIndex = 0;
            this.lblAccomodation.Text = "Accommodation:";
            // 
            // cmbHospitalService
            // 
            this.cmbHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHospitalService.Location = new System.Drawing.Point( 128, 63 );
            this.cmbHospitalService.Name = "cmbHospitalService";
            this.cmbHospitalService.Size = new System.Drawing.Size( 224, 21 );
            this.cmbHospitalService.TabIndex = 1;
            this.cmbHospitalService.SelectedIndexChanged += new System.EventHandler( this.cmbHospitalService_SelectedIndexChanged );
            // 
            // locationView
            // 
            this.locationView.EditFindButtonText = "Find...";
            this.locationView.EditVerifyButtonText = "Verify";
            this.locationView.Location = new System.Drawing.Point( 29, 93 );
            this.locationView.Model = null;
            this.locationView.Name = "locationView";
            this.locationView.Size = new System.Drawing.Size( 332, 123 );
            this.locationView.TabIndex = 2;
            this.locationView.BedSelected += new System.EventHandler( this.locationView_BedSelected );
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point( 4, 4 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size( 984, 508 );
            this.progressPanel1.TabIndex = 1;
            // 
            // panel_TopSpacer1
            // 
            this.panel_TopSpacer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_TopSpacer1.Location = new System.Drawing.Point( 9, 33 );
            this.panel_TopSpacer1.Name = "panel_TopSpacer1";
            this.panel_TopSpacer1.Size = new System.Drawing.Size( 1006, 9 );
            this.panel_TopSpacer1.TabIndex = 0;
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPatientContext.Controls.Add( this.patientContextView1 );
            this.panelPatientContext.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPatientContext.Location = new System.Drawing.Point( 9, 9 );
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new System.Drawing.Size( 1006, 24 );
            this.panelPatientContext.TabIndex = 0;
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
            this.patientContextView1.Size = new System.Drawing.Size( 1004, 22 );
            this.patientContextView1.SocialSecurityNumber = "";
            this.patientContextView1.TabIndex = 0;
            this.patientContextView1.TabStop = false;
            // 
            // TransferLocationDetailView
            // 
            this.AcceptButton = this.btnOk;
            this.BackColor = System.Drawing.Color.FromArgb( ( ( int )( ( ( byte )( 209 ) ) ) ), ( ( int )( ( ( byte )( 228 ) ) ) ), ( ( int )( ( ( byte )( 243 ) ) ) ) );
            this.Controls.Add( this.panel_MainPadding );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.userContextView );
            this.Name = "TransferLocationDetailView";
            this.Size = new System.Drawing.Size( 1024, 620 );
            this.Load += new System.EventHandler( this.TransferLocationDetailView_Load );
            this.panel1.ResumeLayout( false );
            this.panel_MainPadding.ResumeLayout( false );
            this.panelTransferInPatToOutPat.ResumeLayout( false );
            this.gbScanDocuments.ResumeLayout( false );
            this.panelActions.ResumeLayout( false );
            this.panelTransferTo.ResumeLayout( false );
            this.panelTransferTo.PerformLayout();
            this.panelPatientContext.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Private Properties
        private VisitType OriginalVisitType
        {
            set
            {
                i_OriginalVisitType = value;
            }
        }

        private RuleEngine RuleEngine
        {
            get { return i_RuleEngine ?? (i_RuleEngine = RuleEngine.GetInstance()); }
        }
        #endregion

        #region Construction and Finalization
        public TransferLocationDetailView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            locationView.EditFindButtonText = "F&ind...";
            locationView.EditVerifyButtonText = "&Verify";

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

        #region Data Elements

        private Container components = null;

        private ProgressPanel progressPanel1;
        private BackgroundWorker backgroundWorker;

        private UserContextView userContextView;
        private PatientContextView patientContextView1;
        private LocationView locationView;
        private LineLabel lineLabel1;
        private LineLabel lineLabel_TransferFrom;
        private InfoControl infoControl;
        private VisitType i_OriginalVisitType;
        private VisitType i_VisitType;
        private Location i_Location;
        private Location i_FromLocation;
        private Account i_Model;
        private RuleEngine i_RuleEngine;
        private DateTime i_FacilityDateTime;

        private ClickOnceLoggingButton btnCancel;
        private ClickOnceLoggingButton btnOk;
        private LoggingButton btnCloseActivity;
        private LoggingButton btnRepeatActivity;
        private ClickOnceLoggingButton btnEditAccount;
        private LoggingButton btnPrintFaceSheet;

        private Panel panel1;
        private Panel panelPatientContext;
        private Panel panel_MainPadding;
        private Panel panel_TopSpacer1;
        private Panel panelTransferInPatToOutPat;
        private Panel panelActions;
        private Panel panelTransferTo;

        private DateTimePicker dateTimePicker;

        private MaskedEditTextBox mtbTransferDate;
        private MaskedEditTextBox mtbTransferTime;

        private Label lblTransferDate;
        private Label lblTime;
        private Label lblAccomodation;
        private Label lblAdmitDateVal;
        private Label lblAccountVal;
        private Label lblPatientNameVal;
        private Label lblAdmitDate;
        private Label lblAccount;
        private Label lblPatientName;
        private Label lblToHospitalService;
        private Label lblFromLocationVal;
        private Label lblFromHospitalServiceVal;
        private Label lblFromPatientTypeVal;
        private Label lblFromLocation;
        private Label lblFromHospitalService;
        private Label lblFromPatientType;
        private Label lblAdmitTimeVal;
        private Label lblAdmitTime;
        private Label lblToPatientTypeVal;
        private Label lblToPatientType;
        private Label lblAccommodation;
        private Label lblAccommodationVal;
        private Label lblTransDate;
        private Label lblTransDateVal;
        private Label lblTransTime;
        private Label lblTransTimeVal;
        private Label lblAction;

        private ComboBox cmbHospitalService;
        private PatientAccessComboBox cmbAccomodations;

        private bool isDatesValid;

        #endregion

        #region Constants

        #endregion

        #region Data Elements
        private GroupBox gbScanDocuments;
        private LoggingButton btnScanDocuments;
        private PatientAccessComboBox cmbReasonForPrivateAccommodation;
        private Label label1;
        private Label lblScanDocuments;
        private Accomodation selectedAccomodation;
        #endregion
    }
}
