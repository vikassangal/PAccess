using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.ViewImpl;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using PatientAccess.Utilities;
using PatientAccess.UI.CommonControls.Email.Presenters;

namespace PatientAccess.UI.TransferViews
{
    public class TransferOutPatToInPatStep1View : ControlView, IAlternateCareFacilityView, ITransferOutPatToInPatStep1View
    {
        #region Events
        public event EventHandler CancelButtonClicked;
        public event EventHandler NextButtonClicked;
        #endregion

        #region Event Handlers

        private void TransferOutPatToInPatStep1View_Validating( object sender, CancelEventArgs e )
        {
            inErrorState = false;
        }

        private void btnNext_Click( object sender, EventArgs e )
        {
            // When leaving this view, check both of the span 'from' & 'to' dates to insure that 
            // they're not dates in the future.  If multiple errors exist, only report one.
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            DateTime todaysDate = timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset,
                                                     User.GetCurrent().Facility.DSTOffset );

            blnLeave = true;
            ValidateSpanDates();

            #region Sanjeev Kumar DEFECT 34904 8-15-2007
            // Have added null check to Model OccurrenceSpans. Could also be related to threading issues as in 
            // DEFECTS: 34881, 34743, 34882, 34899, 34096.
            if ( Model.OccurrenceSpans != null )
            {
                if ( inErrorState == false && Model.OccurrenceSpans[0] != null )
                {
                    OccurrenceSpan sp = ( OccurrenceSpan )Model.OccurrenceSpans[0];
                    DateTime span1FromDate = sp.FromDate;
                    DateTime span1ToDate = sp.ToDate;

                    if ( span1FromDate > todaysDate )
                    {
                        inErrorState = true;
                        UIColors.SetErrorBgColor( mtbSpan1FromDate );
                        MessageBox.Show( UIErrorMessages.SPANCODE_FROMDATE_IN_FUTURE_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                    else if ( span1ToDate > todaysDate )
                    {
                        inErrorState = true;
                        UIColors.SetErrorBgColor( mtbSpan1ToDate );
                        MessageBox.Show( UIErrorMessages.SPANCODE_TOATE_IN_FUTURE_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                }
                if ( inErrorState == false && Model.OccurrenceSpans[1] != null )
                {
                    OccurrenceSpan sp = ( OccurrenceSpan )Model.OccurrenceSpans[1];
                    DateTime span2FromDate = sp.FromDate;
                    DateTime span2ToDate = sp.ToDate;

                    if ( span2FromDate > todaysDate )
                    {
                        inErrorState = true;
                        UIColors.SetErrorBgColor( mtbSpan2FromDate );
                        MessageBox.Show( UIErrorMessages.SPANCODE_FROMDATE_IN_FUTURE_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                    else if ( span2ToDate > todaysDate )
                    {
                        inErrorState = true;
                        UIColors.SetErrorBgColor( mtbSpan2ToDate );
                        MessageBox.Show( UIErrorMessages.SPANCODE_TOATE_IN_FUTURE_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                }
            }
            #endregion

            if ( inErrorState == false )
            {
                if ( NextButtonClicked != null )
                {
                    NextButtonClicked( this, new LooseArgs( Model ) );
                }
            }
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            if ( AccountActivityService.ConfirmCancelActivity(
                sender, new LooseArgs( Model ) ) )
            {
                TransferOutPatToInPatView_LeaveView( sender, e );
                if ( CancelButtonClicked != null )
                {
                    CancelButtonClicked( this, new EventArgs() );
                }
            }
        }

        private void cboAdmitSource_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cboAdmitSource_SelectIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if ( cb != null && cb.SelectedIndex != -1 )
            {
                HandleAdmitSourceSelectedIndexChanged( cb.SelectedItem as AdmitSource );
            }
        }
        private void cboAdmitSource_Validating( object sender, CancelEventArgs e )
        {
            HandleAdmitSourceSelectedIndexChanged( cboAdmitSource.SelectedItem as AdmitSource );


            UIColors.SetNormalBgColor( cboAdmitSource );
            Refresh();

            RuleEngine.EvaluateRule( typeof( AdmitSourceRequired ), Model );
            RuleEngine.EvaluateRule( typeof( AdmitSourcePreferred ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidAdmitSourceCode ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidAdmitSourceCodeChange ), Model );

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

        private void AdmitSourceRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboAdmitSource );
        }

        private void AlternateCareFacilityRequiredEventHandler( object sender, EventArgs e )
        {
            cmbAlternateCareFacility.Enabled = true;
            UIColors.SetRequiredBgColor( cmbAlternateCareFacility );
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
     
       
        private void cmbAlternateCareFacility_SelectedIndexChanged( object sender, EventArgs e )
        {
            
            string selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if ( selectedAlternateCare != null )
            {
                AlternateCareFacilityPresenter.UpdateAlternateCareFacility( selectedAlternateCare );
            }
            UIColors.SetNormalBgColor( cmbAlternateCareFacility );
            AlternateCareFacilityPresenter.EvaluateAlternateCareFacilityRule();
        }

        private void cboHospitalService_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        //selected a HSV
        private void cboHospitalService_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = ( ComboBox )sender;
            if ( cb.SelectedIndex != -1 )
            {
                locationView1.TabStop = true;
                locationView1.EnableLocationControls();
                Model.HospitalService = ( HospitalService )cboHospitalService.SelectedItem;
                BreadCrumbLogger.GetInstance.Log( String.Format( "{0} selected", Model.HospitalService.Description ) );
            }

            UIColors.SetNormalBgColor( cboHospitalService );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( LocationRequired ), Model );
        }

        private void HospitalServiceRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboHospitalService );

            locationView1.TabStop = false;
            locationView1.DisableLocationControls();
        }

        private void locationView1_BedSelected( object sender, EventArgs e )
        {
            Model.Location = locationView1.Model.Location;
            FillAccommodationList();

            if ( Model.Location.Bed.Accomodation != null )
            {
                cboAccommodations.SelectedItem = Model.Location.Bed.Accomodation;
            }

            UIColors.SetNormalBgColor( locationView1.field_AssignedBed );
            RuleEngine.GetInstance().EvaluateRule( typeof( LocationRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( AccomodationRequired ), Model );
        }

        private void LocationRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( locationView1.field_AssignedBed );
        }

        private void cboAccommodations_SelectedIndexChanged( object sender, EventArgs e )
        {
            PrivateRoomConditionCodeHelper privateRoomConditionCode = new PrivateRoomConditionCodeHelper();
            selectedAccomodation = cboAccommodations.SelectedItem as Accomodation;
            if ( selectedAccomodation != null && cboAccommodations.Enabled )
            {

                privateRoomConditionCode.AddConditionCodeForPrivateRoomMedicallyNecessary( selectedAccomodation, Model );

                if ( Model.IsReasonForAccommodationRequiredForSelectedActivity()
                && Model.Location.Bed.Accomodation.IsReasonRequiredForSelectedAccommodation() )
                {
                    cmbReasonForPrivateAccommodation.Enabled = true;
                    PopulateReasonsForPrivateAccommodation();
                }
                else
                {
                    UIColors.SetNormalBgColor( cmbReasonForPrivateAccommodation );
                    cmbReasonForPrivateAccommodation.SelectedIndex = -1;
                    cmbReasonForPrivateAccommodation.Enabled = false;
                    Model.Diagnosis.isPrivateAccommodationRequested = false;
                    if ( !Model.Location.Bed.Accomodation.IsPrivateRoomMedicallyNecessary() )
                    {
                        privateRoomConditionCode.RemovePrivateRommConditionCodes( Model );
                    }
                }
                UIColors.SetNormalBgColor( cboAccommodations );
                RuleEngine.GetInstance().EvaluateRule( typeof( AccomodationRequired ), Model );
            }
            Model.Location.Bed.Accomodation.IsReasonForAccommodationSelected = false;
            RuleEngine.GetInstance().EvaluateRule( typeof( ReasonForAccomodationRequired ), Model );
        }

        private void cmbReasonForPrivateAccommodation_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( cmbReasonForPrivateAccommodation );
            Refresh();
            RuleEngine.GetInstance().EvaluateRule( typeof( ReasonForAccomodationRequired ), Model );
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

        private void cmbReasonForPrivateAccommodation_SelectedIndexChanged( object sender, EventArgs e )
        {
            PrivateRoomConditionCodeHelper privateRoomConditionCode = new PrivateRoomConditionCodeHelper();
            if ( cmbReasonForPrivateAccommodation.SelectedItem != null )
            {
                privateRoomConditionCode.AddPrivateRoomConditionCode( ( ( ReasonForAccommodation )cmbReasonForPrivateAccommodation.SelectedItem ).Oid, Model );
            }
        }

        private void AccomodationRequiredEventHandler( object sender, EventArgs e )
        {
            // Don't bother if there are no accomodation codes for the selected bed
            if ( cboAccommodations.Items.Count != NO_ACCOMMODATION_CODES )
            {
                UIColors.SetRequiredBgColor( cboAccommodations );
                cboAccommodations.Enabled = true;
            }
        }

        private void mtbChiefComplaint_Validating( object sender, CancelEventArgs e )
        {
            Model.Diagnosis.ChiefComplaint = mtbChiefComplaint.UnMaskedText;

            UIColors.SetNormalBgColor( mtbChiefComplaint );
            RuleEngine.GetInstance().EvaluateRule( typeof( ChiefComplaintRequired ), Model );
        }

        private void ChiefComplaintRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbChiefComplaint );
        }

        private void dateTimePicker_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbTransferDate );
            DateTime dt = dateTimePicker.Value;
            mtbTransferDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbTransferDate.Focus();
        }

        private bool EvaluateDateTimeRules()
        {
            return RuleEngine.GetInstance().EvaluateRule(typeof (TransferDateRequired), Model) &&
                   RuleEngine.GetInstance().EvaluateRule(typeof (TransferDateFutureDate), Model) &&
                   RuleEngine.GetInstance().EvaluateRule(typeof (TransferTimeFutureTime), Model, mtbTransferDate) &&
                   RuleEngine.GetInstance()
                             .EvaluateRule(typeof (TransferDateTimeBeforeAdmitDateTime), Model, mtbTransferDate);

        }

        private void mtbTransferDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbTransferDate );
            UIColors.SetNormalBgColor( mtbTransferTime );

            inErrorState = false;

            if ( mtbTransferDate.UnMaskedText != String.Empty )
            {
                if (TransferService.IsTransferDateValid(mtbTransferDate))
                {
                    Model.TransferDate = GetTransferDateTime();
                    inErrorState = ! EvaluateDateTimeRules();
                }
                else
                {
                    inErrorState = true;
                }
                Refresh();
            }
            else
            {
                Model.TransferDate = GetTransferDateTime();

                UIColors.SetNormalBgColor( mtbTransferDate );
                UIColors.SetNormalBgColor(mtbTransferTime); 
                EvaluateDateTimeRules();
            }
        }

        private void mtbTransferTime_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbTransferTime );
            inErrorState = false;

            if ( mtbTransferTime.UnMaskedText != String.Empty )
            {
                if ( DateValidator.IsValidTime( mtbTransferTime ) )
                {
                    Model.TransferDate = GetTransferDateTime();
                    UIColors.SetNormalBgColor( mtbTransferDate );
                    UIColors.SetNormalBgColor( mtbTransferTime );
                    EvaluateDateTimeRules();
                }
                else
                {
                    UIColors.SetErrorBgColor( mtbTransferTime );
                    if ( !dateTimePicker.Focused )
                    {
                        mtbTransferTime.Focus();
                    }
                    inErrorState = true;
                }
                Refresh();
            }
            else
            {
                Model.TransferDate = GetTransferDateTime();
                UIColors.SetNormalBgColor( mtbTransferDate );
                UIColors.SetNormalBgColor( mtbTransferTime );
                EvaluateDateTimeRules();
            }
        }

        private void TransferDateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbTransferDate );
        }

        private void cboSpan1_SelectedIndexChanged( object sender, EventArgs e )
        {
            UpdateSpan1InModel();
            if ( ( ( SpanCode )cboSpan1.SelectedItem ).Oid != BLANK_OPTION_OID )
            {
                SetSpan1ControlsEnableStatus( true );
                SetSpan1ControlsDisableBGColor( false );

                RuleEngine.GetInstance().EvaluateRule( typeof( OnTransferToForm ), Model );
            }
            else
            {
                SetSpan1ControlsEnableStatus( false );
                SetSpan1ControlsDisableBGColor( true );
            }
        }

        private void dtpSpan1From_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan1FromDate );
            DateTime dt = dtpSpan1From.Value;
            mtbSpan1FromDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbSpan1FromDate.Focus();
            UpdateSpan1InModel();
        }

        private void dtpSpan1To_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan1ToDate );
            DateTime dt = dtpSpan1To.Value;
            mtbSpan1ToDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbSpan1ToDate.Focus();
            UpdateSpan1InModel();
        }

        private void mtbSpan1FromDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan1FromDate );
            UIColors.SetNormalBgColor( mtbSpan1ToDate );

            if ( !dtpSpan1To.Focused
                && !dtpSpan1From.Focused
                && !dtpSpan2To.Focused
                && !dtpSpan2From.Focused )
            {
                ValidateSpan1Date( ref mtbSpan1FromDate );
            }
        }

        private void mtbSpan1ToDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan1FromDate );
            UIColors.SetNormalBgColor( mtbSpan1ToDate );

            if ( !dtpSpan1To.Focused
                && !dtpSpan1From.Focused
                && !dtpSpan2To.Focused
                && !dtpSpan2From.Focused )
            {
                ValidateSpan1Date( ref mtbSpan1ToDate );
            }
        }

        private void mtbFacility_Validating( object sender, CancelEventArgs e )
        {
            UpdateSpan1InModel();
        }

        private void cboSpan2_SelectedIndexChanged( object sender, EventArgs e )
        {
            UpdateSpan2InModel();

            if ( ( ( SpanCode )cboSpan2.SelectedItem ).Oid != BLANK_OPTION_OID )
            {
                SetSpan2ControlsEnableStatus( true );
                SetSpan2ControlsDisableBGColor( false );

                RuleEngine.GetInstance().EvaluateRule( typeof( OnTransferToForm ), Model );
            }
            else
            {
                SetSpan2ControlsEnableStatus( false );
                SetSpan2ControlsDisableBGColor( true );
            }
        }

        private void dtpSpan2From_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan2FromDate );
            DateTime dt = dtpSpan2From.Value;
            mtbSpan2FromDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbSpan2FromDate.Focus();
            UpdateSpan2InModel();
        }

        private void dtpSpan2To_CloseUp( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan2ToDate );
            DateTime dt = dtpSpan2To.Value;
            mtbSpan2ToDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            mtbSpan2ToDate.Focus();
            UpdateSpan2InModel();
        }

        private void mtbSpan2FromDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan2FromDate );
            UIColors.SetNormalBgColor( mtbSpan2ToDate );

            if ( !dtpSpan1To.Focused
                && !dtpSpan1From.Focused
                && !dtpSpan2To.Focused
                && !dtpSpan2From.Focused )
            {
                ValidateSpan2Date( ref mtbSpan2FromDate );
            }
        }

        private void mtbSpan2ToDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbSpan2FromDate );
            UIColors.SetNormalBgColor( mtbSpan2ToDate );

            if ( !dtpSpan1To.Focused
                && !dtpSpan1From.Focused
                && !dtpSpan2To.Focused
                && !dtpSpan2From.Focused )
            {
                ValidateSpan2Date( ref mtbSpan2ToDate );
            }
        }

        private void ValidateSpanDates()
        {
            ValidateSpan1Date( ref mtbSpan1FromDate );
            ValidateSpan1Date( ref mtbSpan1ToDate );
            ValidateSpan2Date( ref mtbSpan2FromDate );
            ValidateSpan2Date( ref mtbSpan2ToDate );
        }

        private void ValidateSpan1Date( ref MaskedEditTextBox mtbSpanDate )
        {
            if ( mtbSpanDate.UnMaskedText != String.Empty )
            {
                if ( TransferService.IsTransferDateValid( mtbSpanDate ) )
                {
                    UpdateSpan1InModel();
                    CheckForValidSpan1Range();
                    if ( !blnLeave )
                    {
                        RuleEngine.GetInstance().EvaluateRule( typeof( OnTransferToForm ), Model );
                    }
                }
                else
                {
                    inErrorState = true;
                }
                Refresh();
            }
            else
            {
                UpdateSpan1InModel();
                UIColors.SetNormalBgColor( mtbSpan1FromDate );
                UIColors.SetNormalBgColor( mtbSpan1ToDate );
                if ( !blnLeave )
                {
                    RuleEngine.GetInstance().EvaluateRule( typeof( OnTransferToForm ), Model );
                }
            }
        }

        private void ValidateSpan2Date( ref MaskedEditTextBox mtbSpanDate )
        {
            if ( mtbSpanDate.UnMaskedText != String.Empty )
            {
                if ( TransferService.IsTransferDateValid( mtbSpanDate ) )
                {
                    UpdateSpan2InModel();
                    CheckForValidSpan2Range();
                    if ( !blnLeave )
                    {
                        RuleEngine.GetInstance().EvaluateRule( typeof( OnTransferToForm ), Model );
                    }
                }
                else
                {
                    inErrorState = true;
                }
                Refresh();
            }
            else
            {
                UpdateSpan2InModel();
                UIColors.SetNormalBgColor( mtbSpan2FromDate );
                UIColors.SetNormalBgColor( mtbSpan2ToDate );
                if ( !blnLeave )
                {
                    RuleEngine.GetInstance().EvaluateRule( typeof( OnTransferToForm ), Model );
                }
            }
        }

        private void Span1FromDateRequiredEventHandler( object sender, EventArgs e )
        {
            dtpSpan1From.Enabled = true;
            mtbSpan1FromDate.Enabled = true;

            UIColors.SetRequiredBgColor( mtbSpan1FromDate );
        }

        private void ReasonForAccomodationRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbReasonForPrivateAccommodation );
        }

        private void Span1ToDateRequiredEventHandler( object sender, EventArgs e )
        {
            dtpSpan1To.Enabled = true;
            mtbSpan1ToDate.Enabled = true;

            UIColors.SetRequiredBgColor( mtbSpan1ToDate );
        }

        private void Span2FromDateRequiredEventHandler( object sender, EventArgs e )
        {
            dtpSpan2From.Enabled = true;
            mtbSpan2FromDate.Enabled = true;

            UIColors.SetRequiredBgColor( mtbSpan2FromDate );
        }

        private void Span2ToDateRequiredEventHandler( object sender, EventArgs e )
        {
            dtpSpan2To.Enabled = true;
            mtbSpan2ToDate.Enabled = true;

            UIColors.SetRequiredBgColor( mtbSpan2ToDate );
        }

        private void TransferDateFutureDateEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbTransferDate );

            MessageBox.Show( UIErrorMessages.TRANSFER_DATE_IN_FUTURE_MSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1 );
            dateTimePicker.Focus();
        }

        private void TransferTimeFutureTimeEventHandler( object sender, EventArgs e )
        {
            if ( e != null )
            {
                PropertyChangedArgs args = ( PropertyChangedArgs )e;
                Control aControl = args.Context as Control;

                if ( aControl == mtbTransferDate )
                {
                    UIColors.SetErrorBgColor( mtbTransferDate );
                    dateTimePicker.Focus();

                    MessageBox.Show( UIErrorMessages.TRANSFER_DATE_IN_FUTURE_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );

                }
                else
                {
                    UIColors.SetErrorBgColor( mtbTransferTime );
                    if ( !dateTimePicker.Focused )
                    {
                        mtbTransferTime.Focus();
                    }

                    MessageBox.Show( UIErrorMessages.TRANSFER_TIME_IN_FUTURE_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
            }
        }

        private void TransferDateTimeBeforeAdmitDateTimeEventHandler( object sender, EventArgs e )
        {
            if ( e != null )
            {
                PropertyChangedArgs args = ( PropertyChangedArgs )e;
                Control aControl = args.Context as Control;
               
                if ( aControl == mtbTransferDate )
                {
                    UIColors.SetErrorBgColor( mtbTransferDate );
                    if ( !dateTimePicker.Focused )
                    {
                        mtbTransferDate.Focus();
                        MessageBox.Show(UIErrorMessages.TRANSFER_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                        MessageBoxDefaultButton.Button1);
                    }
                }
                else
                {
                    UIColors.SetErrorBgColor( mtbTransferTime );
                    if ( !dateTimePicker.Focused )
                    {
                        mtbTransferTime.Focus();
                    }

                    MessageBox.Show( UIErrorMessages.TRANSFER_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
            }
        }

        private void Span1FromDateIsFutureEventHandler( object sender, EventArgs e )
        {
            TransferService.SetErrBgColor( mtbSpan1FromDate );

            MessageBox.Show( UIErrorMessages.SPAN_FROMDATE_IN_FUTURE_MSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1 );
            mtbSpan1FromDate.Focus();
        }

        private void Span1ToDateIsFutureEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbSpan1ToDate );

            MessageBox.Show( UIErrorMessages.SPAN_TODATE_IN_FUTURE_MSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1 );

            mtbSpan1ToDate.Focus();
        }

        private void Span2FromDateIsFutureEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbSpan2FromDate );

            MessageBox.Show( UIErrorMessages.SPAN_FROMDATE_IN_FUTURE_MSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1 );
            mtbSpan2FromDate.Focus();
        }

        private void Span2ToDateIsFutureEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbSpan2ToDate );

            MessageBox.Show( UIErrorMessages.SPAN_TODATE_IN_FUTURE_MSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1 );
            mtbSpan2ToDate.Focus();
        }

        internal void EmailAddressRequiredEventHandler(object sender, EventArgs e)
        {
           UIColors.SetRequiredBgColor(mtbEmail);
        }

        private void mtbEmailAddress_Leave(object sender, EventArgs e)
        {
            mtbEmailAddress_Validating(sender, e);
        }
		
        private void mtbEmailAddress_Validating(object sender, EventArgs e)
        {
            btnNext.Enabled = false;
            var mtb = (MaskedEditTextBox)sender;
            UIColors.SetNormalBgColor(mtb);

            // check if only valid email special characters have been entered or pasted
            if (mtb.Text != string.Empty && emailKeyPressExpression.IsMatch(mtb.Text) == false)
            {   // Prevent cursor from advancing to the next control
              
                UIColors.SetErrorBgColor(mtb);
                MessageBox.Show(UIErrorMessages.ONLY_VALID_EMAIL_CHARACTERS_ALLOWED, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                mtb.Focus();
                return;
            }

            // check if entered email is in the correct email format
            if (mtb.Text != string.Empty &&
                (emailValidationExpression.IsMatch(mtb.Text) == false ||
                 EmailAddressPresenter.IsGenericEmailAddress(mtb.Text))
                )
            {
                // Prevent cursor from advancing to the next control                
                UIColors.SetErrorBgColor(mtb);
                MessageBox.Show(UIErrorMessages.EMAIL_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                mtb.Focus();
            }
			
            else
            {
               ContactPoint mailingContactPoint = Model.Patient.ContactPointWith(
                       TypeOfContactPoint.NewMailingContactPointType());
               mailingContactPoint.EmailAddress = (mtb.Text == String.Empty) ? new EmailAddress() : new EmailAddress(mtb.Text);
               RuleEngine.OneShotRuleEvaluation<EmailAddressRequired>(Model, EmailAddressRequiredEventHandler);
               btnNext.Enabled = true;
            }
        }

        private void Application_Idle( object sender, EventArgs e )
        {
        }

        private void TransferOutPatToInPatView_LeaveView( object sender, EventArgs e )
        {
            Application.Idle -= Application_Idle;
        }

        private void TransferOutPatToInPatStep1View_Disposed( object sender, EventArgs e )
        {
            UnRegisterRulesEvents();
        }

        #endregion

        #region Methods

        public void ShowPanel()
        {
            ProgressPanel1.Visible = true;
            ProgressPanel1.BringToFront();
        }

        public void HidePanel()
        {
            ProgressPanel1.Visible = false;
        }

        private void SetEmailAddressNormalColor(object sender, EventArgs e)
        {
           UIColors.SetNormalBgColor( mtbEmail );
        }
		
        public override void UpdateView()
        {
           
            patientPortalOptInView.PortalOptedOutEvent += SetEmailAddressNormalColor;
            RegisterRulesEvents();

            SysGenSpan1 = null;
            SysGenSpan2 = null;

            if ( Model != null )
            {
                btnNext.Enabled = true;

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

                if ( PreValidationSuccess )
                {
                    PopulateAdmitSources();
                    TransferOutPatToInPatPresenter.SetAdmittingCategory();
                    lblPatientTypeToVal.Text = "1 INPATIENT";
                    Model.KindOfVisit = new VisitType( 0L, PersistentModel.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT );

                    PopulateHsvCodes();
                    Model.HospitalService = null;

                    locationView1.Model = Model;
                    locationView1.UpdateView();

                    locationView1.TabStop = false;
                    locationView1.DisableLocationControls();

                    PopulateAccommodation();

                    mtbChiefComplaint.UnMaskedText = Model.Diagnosis.ChiefComplaint.Trim();

                    OriginalChiefComplaint = Model.Diagnosis.ChiefComplaint.Trim();
                    Model.TransferDate = FacilityDateTime;
                    mtbTransferDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", FacilityDateTime.Month, FacilityDateTime.Day, FacilityDateTime.Year );
                    mtbTransferTime.UnMaskedText = String.Format( "{0:D2}{1:D2}", FacilityDateTime.Hour, FacilityDateTime.Minute );

                    SetSpan1ControlsDisableBGColor( true );
                    SetSpan2ControlsDisableBGColor( true );

                    SetSpan1ControlsEnableStatus( false );
                    SetSpan2ControlsEnableStatus( false );

                    PopulateSpanLists();
                    PopulateSysGenSpans();

                    // Populate the EmailAddress into mtbEmail and make the control visible
                    PatientPortalOptInPresenter = new PatientPortalOptInPresenter(patientPortalOptInView, new MessageBoxAdapter(), Model,
                    new PatientPortalOptInFeatureManager(), RuleEngine.GetInstance());

                    if (Model.Patient != null)
                    {
                        PatientPortalOptInPresenter.UpdateView();

                        if (PatientPortalOptInPresenter.IsFeatureEnabled())
                        {
                            MakeEmailAddressVisible();
                            PopulateEmailAddress();
                        }
                    }
					
                    RunRules();

                    cboAdmitSource.Focus();
                }
                else
                {
                    DisableControls();
                }
            }
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

        public bool HasUpdatedChiefComplaint()
        {
            if ( mtbChiefComplaint.UnMaskedText.Trim() != OriginalChiefComplaint )
            {
                return true;
            }
            return false;
        }

        public override void UpdateModel()
        {
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

        private ITransferOutPatToInPatStep1Presenter TransferOutPatToInPatPresenter { get; set; }
        private PatientPortalOptInPresenter PatientPortalOptInPresenter { get; set; }

        #endregion

        #region private method
        private void SetAdmittingCategory()
        {
            if ( Model.KindOfVisit != null )
            {
                if ( Model.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT )
                    Model.AdmittingCategory = ADMITTING_CATEGORY_EMERGENCY;
                else
                    Model.AdmittingCategory = ADMITTING_CATEGORY_URGENT;
            }
            else
            {
                Model.AdmittingCategory = ADMITTING_CATEGORY_URGENT;
            }
        }

        private void PopulateAdmitSources()
        {
            AdmitSourceBrokerProxy brokerProxy = new AdmitSourceBrokerProxy();
            ArrayList allSources = ( ArrayList )brokerProxy.AllTypesOfAdmitSources( User.GetCurrent().Facility.Oid );

            cboAdmitSource.Items.Clear();

            AdmitSource source = new AdmitSource();
            for ( int i = 0; i < allSources.Count; i++ )
            {
                source = ( AdmitSource )allSources[i];

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

        private void PopulateHsvCodes()
        {
            HSVBrokerProxy brokerProxy = new HSVBrokerProxy();
            ICollection hsvCodes = brokerProxy.HospitalServicesFor( User.GetCurrent().Facility.Oid, VisitType.INPATIENT );

            cboHospitalService.Items.Clear();

            if ( hsvCodes != null && hsvCodes.Count > 0 )
            {
                foreach ( HospitalService hospitalService in hsvCodes )
                {
                    cboHospitalService.Items.Add( hospitalService );
                }
                cboHospitalService.Sorted = true;
            }

            cboHospitalService.SelectedItem = null;
        }

        private void PopulateAccommodation()
        {
            if ( Model.Location == null ||
                Model.Location.NursingStation == null ||
                Model.Location.Room == null ||
                Model.Location.Bed == null ||
                Model.Location.NursingStation.Code.Trim() == string.Empty ||
                Model.Location.Room.Code.Trim() == string.Empty ||
                Model.Location.Bed.Code.Trim() == string.Empty )
            {
                cboAccommodations.SelectedItem = null;
                cboAccommodations.Enabled = false;
            }
            else
            {
                FillAccommodationList();
                locationView1.Model.Location.Bed.Accomodation = null; //default is blank
            }
        }

        private void FillAccommodationList()
        {
            LocationBrokerProxy broker = new LocationBrokerProxy();
            ArrayList accommodationsCodes = ( ArrayList )broker.AccomodationCodesFor(
                locationView1.Model.Location.NursingStation.Code,
                User.GetCurrent().Facility );
            cboAccommodations.Items.Clear();

            foreach ( Accomodation accommodation in accommodationsCodes )
            {
                cboAccommodations.Items.Add( accommodation );
            }
            cboAccommodations.SelectedItem = null;
            cboAccommodations.Enabled = cboAccommodations.Items.Count != NO_ACCOMMODATION_CODES ? true : false;
        }

        private void PopulateSpanLists()
        {
            ISpanCodeBroker spanCodeBroker = new SpanCodeBrokerProxy();
            ArrayList allSpanCodes = ( ArrayList )spanCodeBroker.AllSpans( User.GetCurrent().Facility.Oid );

            cboSpan1.Items.Clear();
            cboSpan2.Items.Clear();

            SpanCode blankSpan = new SpanCode( BLANK_OPTION_OID, PersistentModel.NEW_VERSION, string.Empty, string.Empty );

            cboSpan1.Items.Add( blankSpan );
            cboSpan2.Items.Add( blankSpan );

            SpanCode spanCode = new SpanCode();
            for ( int i = 0; i < allSpanCodes.Count; i++ )
            {
                spanCode = ( SpanCode )allSpanCodes[i];

                if ( IsValidSpanListOption( spanCode ) )
                {
                    cboSpan1.Items.Add( spanCode );
                    cboSpan2.Items.Add( spanCode );
                }
            }

            cboSpan1.SelectedItem = blankSpan;
            cboSpan2.SelectedItem = blankSpan;

            if ( Model.OccurrenceSpans[0] != null )
            {
                OccurrenceSpan occurrenceSpan1 = ( OccurrenceSpan )Model.OccurrenceSpans[0];

                if ( IsValidSpanListOption( occurrenceSpan1.SpanCode ) )
                {
                    cboSpan1.SelectedItem = occurrenceSpan1.SpanCode;
                }
                else
                {
                    cboSpan1.Items.Add( occurrenceSpan1.SpanCode );
                    cboSpan1.SelectedItem = occurrenceSpan1.SpanCode;
                }

                DateTime span1FromDate = occurrenceSpan1.FromDate;
                DateTime span1ToDate = occurrenceSpan1.ToDate;

                mtbSpan1FromDate.UnMaskedText = span1FromDate != DateTime.MinValue ?
                    CommonFormatting.MaskedDateFormat( span1FromDate ) : string.Empty;

                UIColors.SetNormalBgColor( mtbSpan1FromDate );
                mtbSpan1FromDate.Enabled = true;
                dtpSpan1From.Enabled = true;

                mtbSpan1ToDate.UnMaskedText = span1ToDate != DateTime.MinValue ?
                    CommonFormatting.MaskedDateFormat( span1ToDate ) : string.Empty;

                UIColors.SetNormalBgColor( mtbSpan1ToDate );
                mtbSpan1ToDate.Enabled = true;
                dtpSpan1To.Enabled = true;

                UIColors.SetNormalBgColor( mtbFacility );
                mtbFacility.UnMaskedText = occurrenceSpan1.Facility;
                mtbFacility.Enabled = true;
            }

            if ( Model.OccurrenceSpans[1] != null )
            {
                OccurrenceSpan occurrenceSpan2 = ( OccurrenceSpan )Model.OccurrenceSpans[1];

                if ( IsValidSpanListOption( occurrenceSpan2.SpanCode ) )
                {
                    cboSpan2.SelectedItem = occurrenceSpan2.SpanCode;
                }
                else
                {
                    cboSpan2.Items.Add( occurrenceSpan2.SpanCode );
                    cboSpan2.SelectedItem = occurrenceSpan2.SpanCode;
                }

                DateTime span2FromDate = occurrenceSpan2.FromDate;
                DateTime span2ToDate = occurrenceSpan2.ToDate;

                mtbSpan2FromDate.UnMaskedText = span2FromDate != DateTime.MinValue ?
                    CommonFormatting.MaskedDateFormat( span2FromDate ) : string.Empty;

                UIColors.SetNormalBgColor( mtbSpan2FromDate );
                mtbSpan2FromDate.Enabled = true;
                dtpSpan2From.Enabled = true;

                mtbSpan2ToDate.UnMaskedText = span2ToDate != DateTime.MinValue ?
                    CommonFormatting.MaskedDateFormat( span2ToDate ) : string.Empty;

                UIColors.SetNormalBgColor( mtbSpan2ToDate );
                mtbSpan2ToDate.Enabled = true;
                dtpSpan2To.Enabled = true;
            }
        }

        private bool IsValidSpanListOption( SpanCode spanCode )
        {
            if ( spanCode.Code == "70" ||
                spanCode.Code == "71" ||
                spanCode.Code == "74" )
            {
                return true;
            }
            return false;
        }

        private void PopulateSysGenSpans()
        {
            ArrayList sortedAccountHistory = new ArrayList( Model.Patient.Accounts );
            SortAccountsByDischarge sortAccounts = new SortAccountsByDischarge();

            sortedAccountHistory.Sort( sortAccounts );

            if ( sortedAccountHistory.Count > 0 )
            {
                //using foreach causes error due to the collection change.
                for ( int i = 0; i < sortedAccountHistory.Count; i++ )
                {
                    AccountProxy proxy = ( AccountProxy )sortedAccountHistory[i];
                    if ( proxy.AccountNumber == Model.AccountNumber ||
                        proxy.KindOfVisit.Code != VisitType.INPATIENT ||
                        Passed60Days( proxy.DischargeDate ) )
                    {
                        sortedAccountHistory.Remove( proxy );
                        i -= 1;
                    }
                }
            }

            if ( sortedAccountHistory.Count > 0 )
            {
                FindSysGenSpan1( sortedAccountHistory );

                if ( sortedAccountHistory.Count > 0 )
                {
                    FindSysGenSpan2( sortedAccountHistory );
                }

                ShowSpans();
            }
        }

        private void FindSysGenSpan1( ArrayList sortedAccounts )
        {
            if ( !FindSpan70ForSysGenSpan1( sortedAccounts ) )
            {
                FindSpan71ForSysGenSpans( sortedAccounts, SysGenSpan1 );
            }
        }

        private void FindSysGenSpan2( ArrayList sortedAccounts )
        {
            FindSpan71ForSysGenSpans( sortedAccounts, SysGenSpan2 );
        }

        private bool FindSpan70ForSysGenSpan1( ArrayList sortedAccounts )
        {
            if ( IsCurrentAccountValiadForSpan70() )
            {
                foreach ( AccountProxy proxy in sortedAccounts )
                {
                    if ( IsPrevAccountValidForSpan70( proxy ) )
                    {
                        SysGenSpan1 = new OccurrenceSpan
                        {
                            SpanCode = scBroker.SpanCodeWith(User.GetCurrent().Facility.Oid, "70"),
                            FromDate = proxy.AdmitDate,
                            ToDate = proxy.DischargeDate,
                            Facility = proxy.Facility.Description
                        };

                        sortedAccounts.Remove( proxy );
                    }
                }

                return true;
            }
            return false;
        }

        private void FindSpan71ForSysGenSpans( ArrayList sortedAccounts, OccurrenceSpan sysGenSpan )
        {
            AccountProxy proxy = ( AccountProxy )sortedAccounts[0];

            sysGenSpan = new OccurrenceSpan
            {
                SpanCode = scBroker.SpanCodeWith(User.GetCurrent().Facility.Oid, "71"),
                FromDate = proxy.AdmitDate,
                ToDate = proxy.DischargeDate,
                Facility = proxy.Facility.Description
            };

            sortedAccounts.Remove( proxy );
        }

        private bool IsCurrentAccountValiadForSpan70()
        {
            string primaryPlanID = string.Empty;

            if ( Model.Insurance.Coverages.Count > 0 )
            {
                foreach ( Coverage coverage in Model.Insurance.Coverages )
                {
                    if ( coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                    {
                        primaryPlanID = coverage.InsurancePlan.PlanID + " " + coverage.InsurancePlan.Payor.Name;
                        break;
                    }
                }
            }

            return IsPlanIDHSVValidForSpan70( primaryPlanID );
        }

        private bool IsPlanIDHSVValidForSpan70( string primaryPlanID )
        {
            if ( IsPlanIdValidForSpan70( primaryPlanID ) &&
                Model.HospitalService != null &&
                ( Model.HospitalService.Code == "95" ||
                Model.HospitalService.Code == "96" ||
                Model.HospitalService.Code == "97" ) )
            {
                return true;
            }

            return false;
        }

        private bool IsPrevAccountValidForSpan70( AccountProxy proxy )
        {
            if ( IsPlanIDHSVValidForSpan70( proxy.PrimaryInsurancePlan ) &&
                proxy.DischargeDate.Date == Model.AdmitDate.Date &&
                proxy.DischargeDate >= proxy.AdmitDate.AddDays( 3 ) )
            {
                return true;
            }

            return false;
        }

        private bool IsPlanIdValidForSpan70( string primaryPlanID )
        {
            bool valid = false;

            if ( primaryPlanID.Length >= 2 )
            {
                if ( primaryPlanID.Substring( 0, 2 ) == "VE" )
                {
                    valid = true;
                }
                else if ( primaryPlanID.Substring( 0, 2 ) == "53" )
                {
                    if ( primaryPlanID.Length >= 3 && primaryPlanID.Substring( 2, 1 ) == "5" )
                    {
                        valid = true;
                    }
                }
            }

            return valid;

        }

        private bool Passed60Days( DateTime dischargeDate )
        {
            if ( FacilityDateTime > dischargeDate.AddDays( 60 ) )
            {
                return true;
            }
            return false;
        }

        private void ShowSpans()
        {
            if ( SysGenSpan1 != null && SysGenSpan2 != null )
            {
                SysGenSpan1FillSpan1();
                SysGenSpan2FillSpan2();
            }
            else if ( SysGenSpan1 != null &&
                     Model.OccurrenceSpans[0] != null )
            {
                SysGenSpan1FillSpan2();
            }
        }

        private void SysGenSpan1FillSpan1()
        {
            cboSpan1.SelectedItem = SysGenSpan1;
            mtbSpan1FromDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SysGenSpan1.FromDate );
            mtbSpan1ToDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SysGenSpan1.ToDate );
            mtbFacility.UnMaskedText = SysGenSpan1.Facility;
        }

        private void SysGenSpan1FillSpan2()
        {
            cboSpan2.SelectedItem = SysGenSpan1;
            mtbSpan2FromDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SysGenSpan1.FromDate );
            mtbSpan2ToDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SysGenSpan1.ToDate );
        }

        private void SysGenSpan2FillSpan2()
        {
            cboSpan2.SelectedItem = SysGenSpan2;
            mtbSpan2FromDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SysGenSpan2.FromDate );
            mtbSpan2ToDate.UnMaskedText = CommonFormatting.MaskedDateFormat( SysGenSpan2.ToDate );
        }

        private void DoPreValidation()
        {
            if ( IsValidPatientType() &&
                NoDischargePassed3MidNights() &&
                HasAbstractNotCompleted() &&
                IsLockOK()
                )
                PreValidationSuccess = true;
            else
            {
                PreValidationSuccess = false;
            }
        }

        private bool IsValidPatientType()
        {
            if ( Model.KindOfVisit.Code != VisitType.OUTPATIENT &&
                ( Model.KindOfVisit.Code != VisitType.EMERGENCY_PATIENT ||
                Model.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) ) )
            {
                infoControl1.DisplayInfoMessage( UIErrorMessages.TRANSFER_NOT_OUTPATIENT_MSG );
                return false;
            }
            return true;
        }

        private bool NoDischargePassed3MidNights()
        {
            if ( Model.DischargeDate != DateTime.MinValue && FacilityDateTime > Model.DischargeDate.AddDays( 3 ) )
            {
                infoControl1.DisplayInfoMessage( UIErrorMessages.TRANSFER_DISCHARGED_3_MIDNIGHTS_AGO_MSG );
                return false;
            }
            return true;
        }

        private bool HasAbstractNotCompleted()
        {
            if ( Model.DischargeDate != DateTime.MinValue && Model.AbstractExists )
            {
                infoControl1.DisplayInfoMessage( UIErrorMessages.TRANSFER_ABSTRACT_COMPLETED_MSG );
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

        private void SetSpan1ControlsDisableBGColor( bool disable )
        {
            if ( !disable )
            {

                UIColors.SetNormalBgColor( mtbSpan1FromDate );
                UIColors.SetNormalBgColor( mtbSpan1ToDate );
            }
            else
            {
                mtbSpan1FromDate.UnMaskedText = string.Empty;
                mtbSpan1ToDate.UnMaskedText = string.Empty;
                mtbFacility.UnMaskedText = string.Empty;

                mtbSpan1FromDate.BackColor = SystemColors.Control;
                mtbSpan1ToDate.BackColor = SystemColors.Control;
                Refresh();
            }
        }

        private void SetSpan2ControlsDisableBGColor( bool disable )
        {
            if ( !disable )
            {
                UIColors.SetNormalBgColor( mtbSpan2FromDate );
                UIColors.SetNormalBgColor( mtbSpan2ToDate );
            }
            else
            {
                mtbSpan2FromDate.UnMaskedText = string.Empty;
                mtbSpan2ToDate.UnMaskedText = string.Empty;

                mtbSpan2FromDate.BackColor = SystemColors.Control;
                mtbSpan2ToDate.BackColor = SystemColors.Control;
                Refresh();
            }
        }

        private void SetSpan1ControlsEnableStatus( bool status )
        {
            dtpSpan1From.Enabled = status;
            dtpSpan1To.Enabled = status;
            mtbSpan1FromDate.Enabled = status;
            mtbSpan1ToDate.Enabled = status;

            mtbFacility.Enabled = status;
        }

        private void SetSpan2ControlsEnableStatus( bool status )
        {
            dtpSpan2From.Enabled = status;
            dtpSpan2To.Enabled = status;
            mtbSpan2FromDate.Enabled = status;
            mtbSpan2ToDate.Enabled = status;
        }

        private DateTime GetTransferDateTime()
        {
            string date = mtbTransferDate.Text;
            string time = ( mtbTransferTime.UnMaskedText != string.Empty ) ? mtbTransferTime.Text : "00:00";
            DateTime transferDate = DateTime.MinValue;
            if ( mtbTransferDate.UnMaskedText != string.Empty )
            {
                try
                {
                    transferDate = Convert.ToDateTime( date + " " + time );
                }
                catch
                {
                    transferDate = Convert.ToDateTime( date );
                }
            }
            return transferDate;
        }

        private void UpdateSpan1InModel()
        {
            Model.OccurrenceSpans.RemoveAt( 0 );

            if ( ( ( SpanCode )cboSpan1.SelectedItem ).Oid != BLANK_OPTION_OID )
            {
                OccurrenceSpan os1 = new OccurrenceSpan
                {
                    SpanCode = (SpanCode) cboSpan1.SelectedItem,
                    FromDate = mtbSpan1FromDate.UnMaskedText != string.Empty
                                ? Convert.ToDateTime(mtbSpan1FromDate.Text)
                                : DateTime.MinValue,
                    ToDate = mtbSpan1ToDate.UnMaskedText != string.Empty
                                ? Convert.ToDateTime(mtbSpan1ToDate.Text)
                                : DateTime.MinValue,
                    Facility = mtbFacility.UnMaskedText
                };

                Model.OccurrenceSpans.Insert( 0, os1 );
                // Make sure the 'to' date is not earlier than the 'from' date
            }
            else
            {
                Model.OccurrenceSpans.Insert( 0, null );
            }
        }

        private void UpdateSpan2InModel()
        {
            Model.OccurrenceSpans.RemoveAt( 1 );

            if ( ( ( SpanCode )cboSpan2.SelectedItem ).Oid != BLANK_OPTION_OID )
            {
                OccurrenceSpan os2 = new OccurrenceSpan
                {
                    SpanCode = (SpanCode) cboSpan2.SelectedItem,
                    FromDate = mtbSpan2FromDate.UnMaskedText != string.Empty
                                ? Convert.ToDateTime(mtbSpan2FromDate.Text)
                                : DateTime.MinValue,
                    ToDate = mtbSpan2ToDate.UnMaskedText != string.Empty
                                ? Convert.ToDateTime(mtbSpan2ToDate.Text)
                                : DateTime.MinValue
                };

                Model.OccurrenceSpans.Add( os2 );
                // Make sure the 'to' date is not earlier than the 'from' date
            }
            else
            {
                Model.OccurrenceSpans.Add( null );
            }
        }

        private void CheckForValidSpan1Range()
        {
            if ( Model.OccurrenceSpans[0] != null )
            {
                OccurrenceSpan sp = ( OccurrenceSpan )Model.OccurrenceSpans[0];
                DateTime spanFromDate = sp.FromDate;
                DateTime spanToDate = sp.ToDate;

                if ( spanFromDate == DateTime.MinValue || spanToDate == DateTime.MinValue )
                {
                    return;
                }

                if ( spanFromDate > spanToDate )
                {
                    inErrorState = true;
                    mtbSpan1ToDate.Focus();
                    UIColors.SetErrorBgColor( mtbSpan1ToDate );
                    MessageBox.Show( UIErrorMessages.TRANSFER_RANGE_INVALID_MSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                }
                else
                {
                    inErrorState = false;
                }
            }
        }

        private void CheckForValidSpan2Range()
        {
            if ( Model.OccurrenceSpans[1] != null )
            {
                OccurrenceSpan sp = ( OccurrenceSpan )Model.OccurrenceSpans[1];
                DateTime spanFromDate = sp.FromDate;
                DateTime spanToDate = sp.ToDate;

                if ( spanFromDate == DateTime.MinValue || spanToDate == DateTime.MinValue )
                {
                    return;
                }

                if ( spanFromDate > spanToDate )
                {
                    inErrorState = true;
                    mtbSpan2ToDate.Focus();
                    UIColors.SetErrorBgColor( mtbSpan2ToDate );
                    MessageBox.Show( UIErrorMessages.TRANSFER_RANGE_INVALID_MSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                }
                else
                {
                    inErrorState = false;
                }
            }
        }

        private void DisableControls()
        {
            panelToArea.Enabled = false;
            gpgSpan.Enabled = false;
            mtbChiefComplaint.Enabled = false;
            btnNext.Enabled = false;
        }

        private void RegisterRulesEvents()
        {
            RuleEngine.LoadRules( Model );

            RuleEngine.GetInstance().RegisterEvent( typeof( EmailAddressRequired ), Model,  EmailAddressRequiredEventHandler);
            RuleEngine.GetInstance().RegisterEvent( typeof( AdmitSourceRequired ), Model, new EventHandler( AdmitSourceRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AlternateCareFacilityRequired ), new EventHandler( AlternateCareFacilityRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( HospitalServiceRequired ), Model, new EventHandler( HospitalServiceRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( LocationRequired ), Model, new EventHandler( LocationRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( AccomodationRequired ), Model, new EventHandler( AccomodationRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( ChiefComplaintRequired ), Model, new EventHandler( ChiefComplaintRequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( TransferDateRequired ), Model, new EventHandler( TransferDateRequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( Span1FromDateRequired ), Model, new EventHandler( Span1FromDateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( Span1ToDateRequired ), Model, new EventHandler( Span1ToDateRequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( Span2FromDateRequired ), Model, new EventHandler( Span2FromDateRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( Span2ToDateRequired ), Model, new EventHandler( Span2ToDateRequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( TransferDateFutureDate ), Model, new EventHandler( TransferDateFutureDateEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( TransferTimeFutureTime ), Model, new EventHandler( TransferTimeFutureTimeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( TransferDateTimeBeforeAdmitDateTime ), Model, new EventHandler( TransferDateTimeBeforeAdmitDateTimeEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( Span1FromDateIsFuture ), Model, new EventHandler( Span1FromDateIsFutureEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( Span1ToDateIsFuture ), Model, new EventHandler( Span1ToDateIsFutureEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( Span2FromDateIsFuture ), Model, new EventHandler( Span2FromDateIsFutureEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( Span2ToDateIsFuture ), Model, new EventHandler( Span2ToDateIsFutureEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( ReasonForAccomodationRequired ), Model, new EventHandler( ReasonForAccomodationRequiredEventHandler ) );
        }

        private void UnRegisterRulesEvents()
        {
            // UNREGISTER EVENTS     
            RuleEngine.GetInstance().UnregisterEvent(typeof( EmailAddressRequired ), Model, EmailAddressRequiredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent( typeof( AdmitSourceRequired ), Model, AdmitSourceRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AlternateCareFacilityRequired ), Model, AlternateCareFacilityRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( HospitalServiceRequired ), Model, HospitalServiceRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( LocationRequired ), Model, LocationRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( AccomodationRequired ), Model, AccomodationRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( ChiefComplaintRequired ), Model, ChiefComplaintRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( TransferDateRequired ), Model, TransferDateRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( Span1FromDateRequired ), Model, Span1FromDateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( Span1ToDateRequired ), Model, Span1ToDateRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( Span2FromDateRequired ), Model, Span2FromDateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( Span2ToDateRequired ), Model, Span2ToDateRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( TransferDateFutureDate ), Model, TransferDateFutureDateEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( TransferTimeFutureTime ), Model, TransferTimeFutureTimeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( TransferDateTimeBeforeAdmitDateTime ), Model, TransferDateTimeBeforeAdmitDateTimeEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( Span1FromDateIsFuture ), Model, Span1FromDateIsFutureEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( Span1ToDateIsFuture ), Model, Span1ToDateIsFutureEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( Span2FromDateIsFuture ), Model, Span2FromDateIsFutureEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( Span2ToDateIsFuture ), Model, Span2ToDateIsFutureEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ReasonForAccomodationRequired ), Model, ReasonForAccomodationRequiredEventHandler );
        }

        private void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds
            UIColors.SetNormalBgColor( cboAdmitSource );
            UIColors.SetNormalBgColor( cboHospitalService );
            UIColors.SetNormalBgColor( locationView1.field_AssignedBed );
            UIColors.SetNormalBgColor( cboAccommodations );
            UIColors.SetNormalBgColor( mtbChiefComplaint );
            UIColors.SetNormalBgColor( mtbTransferDate );
            UIColors.SetNormalBgColor( mtbTransferTime );
            UIColors.SetNormalBgColor( mtbEmail );

            Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof( EmailAddressRequired ), Model);
            RuleEngine.GetInstance().EvaluateRule( typeof( AdmitSourceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( HospitalServiceRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( LocationRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( AccomodationRequired ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( ChiefComplaintRequired ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateRequired ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( Span1FromDateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( Span1ToDateRequired ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( Span2FromDateRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( Span2ToDateRequired ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateFutureDate ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( TransferTimeFutureTime ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( TransferDateTimeBeforeAdmitDateTime ), Model );

            RuleEngine.GetInstance().EvaluateRule( typeof( Span1FromDateIsFuture ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( Span1ToDateIsFuture ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( Span2FromDateIsFuture ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( Span2ToDateIsFuture ), Model );
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureChiefComplaint( mtbChiefComplaint );
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

        private OccurrenceSpan SysGenSpan1
        {
            get
            {
                return i_SysGenSpan1;
            }
            set
            {
                i_SysGenSpan1 = value;
            }
        }

        private OccurrenceSpan SysGenSpan2
        {
            get
            {
                return i_SysGenSpan2;
            }
            set
            {
                i_SysGenSpan2 = value;
            }
        }

        private string OriginalChiefComplaint
        {
            get
            {
                return i_OriginalChiefComplaint;
            }
            set
            {
                i_OriginalChiefComplaint = value;
            }
        }

        private RuleEngine RuleEngine
        {
            get { return i_RuleEngine ?? (i_RuleEngine = RuleEngine.GetInstance()); }
        }

        private void MakeEmailAddressVisible()
        {
            mtbEmail.Visible = true;
            lblEmail.Visible = true;
        }

        private void PopulateEmailAddress()
        {
            var mailingContactPoint = Model.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            mtbEmail.UnMaskedText = mailingContactPoint.EmailAddress.ToString();
        }

        #endregion

        public TransferOutPatToInPatStep1View()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            ConfigureControls();
            AlternateCareFacilityPresenter = new AlternateCareFacilityPresenter( this, new AlternateCareFacilityFeatureManager() );
            TransferOutPatToInPatPresenter = new TransferOutPatToInPatStep1Presenter( this );
            // TODO: Add any initialization after the InitializeComponent call
            EnableThemesOn( this );
            emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            emailKeyPressExpression = new Regex(RegularExpressions.EmailValidCharactersExpression);
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

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TransferOutPatToInPatStep1View ) );
            this.panelTransferOutPatToInPat = new System.Windows.Forms.Panel();
            this.lblEmail = new System.Windows.Forms.Label();
            this.mtbEmail = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.patientPortalOptInView = new PatientAccess.UI.RegulatoryViews.ViewImpl.PatientPortalOptInView();
            this.gpgSpan = new System.Windows.Forms.GroupBox();
            this.mtbFacility = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblFac = new System.Windows.Forms.Label();
            this.dtpSpan2To = new System.Windows.Forms.DateTimePicker();
            this.dtpSpan2From = new System.Windows.Forms.DateTimePicker();
            this.dtpSpan1To = new System.Windows.Forms.DateTimePicker();
            this.mtbSpan2ToDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSpan2ToDate = new System.Windows.Forms.Label();
            this.mtbSpan2FromDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSpan2FromDate = new System.Windows.Forms.Label();
            this.mtbSpan1ToDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSpan1ToDate = new System.Windows.Forms.Label();
            this.dtpSpan1From = new System.Windows.Forms.DateTimePicker();
            this.mtbSpan1FromDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSpan1FromDate = new System.Windows.Forms.Label();
            this.cboSpan2 = new System.Windows.Forms.ComboBox();
            this.lblSpan2 = new System.Windows.Forms.Label();
            this.cboSpan1 = new System.Windows.Forms.ComboBox();
            this.lblSpan1 = new System.Windows.Forms.Label();
            this.mtbChiefComplaint = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStep1 = new System.Windows.Forms.Label();
            this.lblChiefComplaint = new System.Windows.Forms.Label();
            this.llblTransferFrom = new PatientAccess.UI.CommonControls.LineLabel();
            this.infoControl1 = new PatientAccess.UI.CommonControls.InfoControl();
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
            this.cmbReasonForPrivateAccommodation = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblReasonforAccommodation = new System.Windows.Forms.Label();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.lblPatientTypeToVal = new System.Windows.Forms.Label();
            this.cboAccommodations = new System.Windows.Forms.ComboBox();
            this.lblAccomodation = new System.Windows.Forms.Label();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.locationView1 = new PatientAccess.UI.CommonControls.LocationView();
            this.mtbTransferTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cboAdmitSource = new System.Windows.Forms.ComboBox();
            this.lblAdmitSource2 = new System.Windows.Forms.Label();
            this.lblHospitalService2 = new System.Windows.Forms.Label();
            this.mtbTransferDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblPatientType2 = new System.Windows.Forms.Label();
            this.lblTransferDate = new System.Windows.Forms.Label();
            this.cboHospitalService = new System.Windows.Forms.ComboBox();
            this.lblAdmitTimeVal = new System.Windows.Forms.Label();
            this.lblAdmitTime = new System.Windows.Forms.Label();
            this.lblAdmitDateVal = new System.Windows.Forms.Label();
            this.lblAccountVal = new System.Windows.Forms.Label();
            this.lblPatientNameVal = new System.Windows.Forms.Label();
            this.lblAdmitDate = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.ProgressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.btnNext = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelUserContext = new System.Windows.Forms.Panel();
            this.userContextView1 = new PatientAccess.UI.UserContextView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.btnBack = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelTransferOutPatToInPat.SuspendLayout();
            this.gpgSpan.SuspendLayout();
            this.panelFromBottomArea.SuspendLayout();
            this.panelToArea.SuspendLayout();
            this.panelUserContext.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTransferOutPatToInPat
            // 
            this.panelTransferOutPatToInPat.BackColor = System.Drawing.Color.White;
            this.panelTransferOutPatToInPat.Controls.Add(this.lblEmail);
            this.panelTransferOutPatToInPat.Controls.Add(this.mtbEmail);
            this.panelTransferOutPatToInPat.Controls.Add(this.patientPortalOptInView);
            this.panelTransferOutPatToInPat.Controls.Add( this.gpgSpan );
            this.panelTransferOutPatToInPat.Controls.Add( this.mtbChiefComplaint );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblStep1 );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblChiefComplaint );
            this.panelTransferOutPatToInPat.Controls.Add( this.llblTransferFrom );
            this.panelTransferOutPatToInPat.Controls.Add( this.infoControl1 );
            this.panelTransferOutPatToInPat.Controls.Add( this.panelFromBottomArea );
            this.panelTransferOutPatToInPat.Controls.Add( this.panelToArea );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblAdmitTimeVal );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblAdmitTime );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblAdmitDateVal );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblAccountVal );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblPatientNameVal );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblAdmitDate );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblAccount );
            this.panelTransferOutPatToInPat.Controls.Add( this.lblPatientName );
            this.panelTransferOutPatToInPat.Controls.Add( this.ProgressPanel1 );
            this.panelTransferOutPatToInPat.Location = new System.Drawing.Point( 8, 64 );
            this.panelTransferOutPatToInPat.Name = "panelTransferOutPatToInPat";
            this.panelTransferOutPatToInPat.Size = new System.Drawing.Size( 1008, 520 );
            this.panelTransferOutPatToInPat.TabIndex = 0;

            // 
            // patientPortalOptInView
            // 
            this.patientPortalOptInView.Location = new System.Drawing.Point(672, 445);
            this.patientPortalOptInView.Model = null;
            this.patientPortalOptInView.Name = "patientPortalOptInView";
            this.patientPortalOptInView.PatientPortalOptInPresenter = null;
            this.patientPortalOptInView.Size = new System.Drawing.Size(275, 36);
            this.patientPortalOptInView.TabIndex = 18;
            this.patientPortalOptInView.Visible = false;
           
            // 
            // lblEmail
            this.lblEmail.Location = new System.Drawing.Point(672, 490);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(44, 23);
            this.lblEmail.TabIndex = 65;
            this.lblEmail.Text = "Email:";
            this.lblEmail.Visible = false;
            // 
            // mtbEmail
            // 
            this.mtbEmail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbEmail.Location = new System.Drawing.Point( 722, 488 );
            this.mtbEmail.Mask = "";
            this.mtbEmail.MaxLength = 64;
            this.mtbEmail.Name = "mtbEmail";
            this.mtbEmail.Size = new System.Drawing.Size( 158, 20 );
            this.mtbEmail.TabIndex = 66;
            this.mtbEmail.Visible = false;
            this.mtbEmail.Validating += new System.ComponentModel.CancelEventHandler( this.mtbEmailAddress_Validating );
            this.mtbEmail.Leave += new System.EventHandler( this.mtbEmailAddress_Leave );

            // 
            // gpgSpan
            // 
            this.gpgSpan.Controls.Add( this.mtbFacility );
            this.gpgSpan.Controls.Add( this.lblFac );
            this.gpgSpan.Controls.Add( this.dtpSpan2To );
            this.gpgSpan.Controls.Add( this.dtpSpan2From );
            this.gpgSpan.Controls.Add( this.dtpSpan1To );
            this.gpgSpan.Controls.Add( this.mtbSpan2ToDate );
            this.gpgSpan.Controls.Add( this.lblSpan2ToDate );
            this.gpgSpan.Controls.Add( this.mtbSpan2FromDate );
            this.gpgSpan.Controls.Add( this.lblSpan2FromDate );
            this.gpgSpan.Controls.Add( this.mtbSpan1ToDate );
            this.gpgSpan.Controls.Add( this.lblSpan1ToDate );
            this.gpgSpan.Controls.Add( this.dtpSpan1From );
            this.gpgSpan.Controls.Add( this.mtbSpan1FromDate );
            this.gpgSpan.Controls.Add( this.lblSpan1FromDate );
            this.gpgSpan.Controls.Add( this.cboSpan2 );
            this.gpgSpan.Controls.Add( this.lblSpan2 );
            this.gpgSpan.Controls.Add( this.cboSpan1 );
            this.gpgSpan.Controls.Add( this.lblSpan1 );
            this.gpgSpan.Location = new System.Drawing.Point( 672, 220 );
            this.gpgSpan.Name = "gpgSpan";
            this.gpgSpan.Size = new System.Drawing.Size( 304, 220 );
            this.gpgSpan.TabIndex = 3;
            this.gpgSpan.TabStop = false;
            this.gpgSpan.Text = "Occurrence spans";
            // 
            // mtbFacility
            // 
            this.mtbFacility.KeyPressExpression = "^[a-zA-Z0-9]*";
            this.mtbFacility.Location = new System.Drawing.Point( 147, 105 );
            this.mtbFacility.Mask = "";
            this.mtbFacility.MaxLength = 13;
            this.mtbFacility.Name = "mtbFacility";
            this.mtbFacility.Size = new System.Drawing.Size( 69, 20 );
            this.mtbFacility.TabIndex = 11;
            this.mtbFacility.ValidationExpression = "^[a-zA-Z0-9]*$";
            this.mtbFacility.Validating += new System.ComponentModel.CancelEventHandler( this.mtbFacility_Validating );
            // 
            // lblFac
            // 
            this.lblFac.Location = new System.Drawing.Point( 86, 108 );
            this.lblFac.Name = "lblFac";
            this.lblFac.Size = new System.Drawing.Size( 51, 20 );
            this.lblFac.TabIndex = 56;
            this.lblFac.Text = "Facility:";
            // 
            // dtpSpan2To
            // 
            this.dtpSpan2To.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpSpan2To.Checked = false;
            this.dtpSpan2To.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSpan2To.Location = new System.Drawing.Point( 216, 187 );
            this.dtpSpan2To.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpSpan2To.Name = "dtpSpan2To";
            this.dtpSpan2To.Size = new System.Drawing.Size( 21, 20 );
            this.dtpSpan2To.TabIndex = 55;
            this.dtpSpan2To.TabStop = false;
            this.dtpSpan2To.CloseUp += new System.EventHandler( this.dtpSpan2To_CloseUp );
            // 
            // dtpSpan2From
            // 
            this.dtpSpan2From.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpSpan2From.Checked = false;
            this.dtpSpan2From.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSpan2From.Location = new System.Drawing.Point( 216, 160 );
            this.dtpSpan2From.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpSpan2From.Name = "dtpSpan2From";
            this.dtpSpan2From.Size = new System.Drawing.Size( 21, 20 );
            this.dtpSpan2From.TabIndex = 54;
            this.dtpSpan2From.TabStop = false;
            this.dtpSpan2From.CloseUp += new System.EventHandler( this.dtpSpan2From_CloseUp );
            // 
            // dtpSpan1To
            // 
            this.dtpSpan1To.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpSpan1To.Checked = false;
            this.dtpSpan1To.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSpan1To.Location = new System.Drawing.Point( 214, 78 );
            this.dtpSpan1To.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpSpan1To.Name = "dtpSpan1To";
            this.dtpSpan1To.Size = new System.Drawing.Size( 22, 20 );
            this.dtpSpan1To.TabIndex = 53;
            this.dtpSpan1To.TabStop = false;
            this.dtpSpan1To.CloseUp += new System.EventHandler( this.dtpSpan1To_CloseUp );
            // 
            // mtbSpan2ToDate
            // 
            this.mtbSpan2ToDate.KeyPressExpression = "^\\d*$";
            this.mtbSpan2ToDate.Location = new System.Drawing.Point( 150, 187 );
            this.mtbSpan2ToDate.Mask = "  /  /";
            this.mtbSpan2ToDate.MaxLength = 10;
            this.mtbSpan2ToDate.Name = "mtbSpan2ToDate";
            this.mtbSpan2ToDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbSpan2ToDate.TabIndex = 14;
            this.mtbSpan2ToDate.ValidationExpression = resources.GetString( "mtbSpan2ToDate.ValidationExpression" );
            this.mtbSpan2ToDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpan2ToDate_Validating );
            // 
            // lblSpan2ToDate
            // 
            this.lblSpan2ToDate.Location = new System.Drawing.Point( 83, 188 );
            this.lblSpan2ToDate.Name = "lblSpan2ToDate";
            this.lblSpan2ToDate.Size = new System.Drawing.Size( 59, 23 );
            this.lblSpan2ToDate.TabIndex = 52;
            this.lblSpan2ToDate.Text = "To date:";
            // 
            // mtbSpan2FromDate
            // 
            this.mtbSpan2FromDate.KeyPressExpression = "^\\d*$";
            this.mtbSpan2FromDate.Location = new System.Drawing.Point( 150, 160 );
            this.mtbSpan2FromDate.Mask = "  /  /";
            this.mtbSpan2FromDate.MaxLength = 10;
            this.mtbSpan2FromDate.Name = "mtbSpan2FromDate";
            this.mtbSpan2FromDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbSpan2FromDate.TabIndex = 13;
            this.mtbSpan2FromDate.ValidationExpression = resources.GetString( "mtbSpan2FromDate.ValidationExpression" );
            this.mtbSpan2FromDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpan2FromDate_Validating );
            // 
            // lblSpan2FromDate
            // 
            this.lblSpan2FromDate.Location = new System.Drawing.Point( 84, 163 );
            this.lblSpan2FromDate.Name = "lblSpan2FromDate";
            this.lblSpan2FromDate.Size = new System.Drawing.Size( 77, 23 );
            this.lblSpan2FromDate.TabIndex = 50;
            this.lblSpan2FromDate.Text = "From date:";
            // 
            // mtbSpan1ToDate
            // 
            this.mtbSpan1ToDate.KeyPressExpression = "^\\d*$";
            this.mtbSpan1ToDate.Location = new System.Drawing.Point( 147, 78 );
            this.mtbSpan1ToDate.Mask = "  /  /";
            this.mtbSpan1ToDate.MaxLength = 10;
            this.mtbSpan1ToDate.Name = "mtbSpan1ToDate";
            this.mtbSpan1ToDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbSpan1ToDate.TabIndex = 10;
            this.mtbSpan1ToDate.ValidationExpression = resources.GetString( "mtbSpan1ToDate.ValidationExpression" );
            this.mtbSpan1ToDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpan1ToDate_Validating );
            // 
            // lblSpan1ToDate
            // 
            this.lblSpan1ToDate.Location = new System.Drawing.Point( 84, 83 );
            this.lblSpan1ToDate.Name = "lblSpan1ToDate";
            this.lblSpan1ToDate.Size = new System.Drawing.Size( 61, 21 );
            this.lblSpan1ToDate.TabIndex = 48;
            this.lblSpan1ToDate.Text = "To date:";
            // 
            // dtpSpan1From
            // 
            this.dtpSpan1From.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpSpan1From.Checked = false;
            this.dtpSpan1From.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSpan1From.Location = new System.Drawing.Point( 214, 52 );
            this.dtpSpan1From.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpSpan1From.Name = "dtpSpan1From";
            this.dtpSpan1From.Size = new System.Drawing.Size( 21, 20 );
            this.dtpSpan1From.TabIndex = 46;
            this.dtpSpan1From.TabStop = false;
            this.dtpSpan1From.CloseUp += new System.EventHandler( this.dtpSpan1From_CloseUp );
            // 
            // mtbSpan1FromDate
            // 
            this.mtbSpan1FromDate.KeyPressExpression = "^\\d*$";
            this.mtbSpan1FromDate.Location = new System.Drawing.Point( 147, 52 );
            this.mtbSpan1FromDate.Mask = "  /  /";
            this.mtbSpan1FromDate.MaxLength = 10;
            this.mtbSpan1FromDate.Name = "mtbSpan1FromDate";
            this.mtbSpan1FromDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbSpan1FromDate.TabIndex = 9;
            this.mtbSpan1FromDate.ValidationExpression = resources.GetString( "mtbSpan1FromDate.ValidationExpression" );
            this.mtbSpan1FromDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpan1FromDate_Validating );
            // 
            // lblSpan1FromDate
            // 
            this.lblSpan1FromDate.Location = new System.Drawing.Point( 84, 55 );
            this.lblSpan1FromDate.Name = "lblSpan1FromDate";
            this.lblSpan1FromDate.Size = new System.Drawing.Size( 61, 23 );
            this.lblSpan1FromDate.TabIndex = 44;
            this.lblSpan1FromDate.Text = "From date:";
            // 
            // cboSpan2
            // 
            this.cboSpan2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSpan2.Location = new System.Drawing.Point( 85, 132 );
            this.cboSpan2.Name = "cboSpan2";
            this.cboSpan2.Size = new System.Drawing.Size( 202, 21 );
            this.cboSpan2.TabIndex = 12;
            this.cboSpan2.SelectedIndexChanged += new System.EventHandler( this.cboSpan2_SelectedIndexChanged );
            // 
            // lblSpan2
            // 
            this.lblSpan2.Location = new System.Drawing.Point( 9, 135 );
            this.lblSpan2.Name = "lblSpan2";
            this.lblSpan2.Size = new System.Drawing.Size( 74, 21 );
            this.lblSpan2.TabIndex = 36;
            this.lblSpan2.Text = "Span code 2:";
            // 
            // cboSpan1
            // 
            this.cboSpan1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSpan1.Location = new System.Drawing.Point( 85, 24 );
            this.cboSpan1.Name = "cboSpan1";
            this.cboSpan1.Size = new System.Drawing.Size( 202, 21 );
            this.cboSpan1.TabIndex = 8;
            this.cboSpan1.SelectedIndexChanged += new System.EventHandler( this.cboSpan1_SelectedIndexChanged );
            // 
            // lblSpan1
            // 
            this.lblSpan1.Location = new System.Drawing.Point( 9, 28 );
            this.lblSpan1.Name = "lblSpan1";
            this.lblSpan1.Size = new System.Drawing.Size( 71, 20 );
            this.lblSpan1.TabIndex = 34;
            this.lblSpan1.Text = "Span code 1:";
            // 
            // mtbChiefComplaint
            // 
            this.mtbChiefComplaint.BackColor = System.Drawing.SystemColors.Window;
            this.mtbChiefComplaint.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbChiefComplaint.Location = new System.Drawing.Point( 679, 164 );
            this.mtbChiefComplaint.Mask = "";
            this.mtbChiefComplaint.MaxLength = 74;
            this.mtbChiefComplaint.Multiline = true;
            this.mtbChiefComplaint.Name = "mtbChiefComplaint";
            this.mtbChiefComplaint.Size = new System.Drawing.Size( 297, 46 );
            this.mtbChiefComplaint.TabIndex = 2;
            this.mtbChiefComplaint.Validating += new System.ComponentModel.CancelEventHandler( this.mtbChiefComplaint_Validating );
            // 
            // lblStep1
            // 
            this.lblStep1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblStep1.Location = new System.Drawing.Point( 13, 8 );
            this.lblStep1.Name = "lblStep1";
            this.lblStep1.Size = new System.Drawing.Size( 163, 23 );
            this.lblStep1.TabIndex = 56;
            this.lblStep1.Text = "Step 1 of 3: Transfer to";
            // 
            // lblChiefComplaint
            // 
            this.lblChiefComplaint.Location = new System.Drawing.Point( 669, 147 );
            this.lblChiefComplaint.Name = "lblChiefComplaint";
            this.lblChiefComplaint.Size = new System.Drawing.Size( 100, 16 );
            this.lblChiefComplaint.TabIndex = 62;
            this.lblChiefComplaint.Text = "Chief complaint:";
            // 
            // llblTransferFrom
            // 
            this.llblTransferFrom.Caption = "Transfer from";
            this.llblTransferFrom.Location = new System.Drawing.Point( 12, 139 );
            this.llblTransferFrom.Name = "llblTransferFrom";
            this.llblTransferFrom.Size = new System.Drawing.Size( 286, 24 );
            this.llblTransferFrom.TabIndex = 55;
            this.llblTransferFrom.TabStop = false;
            // 
            // infoControl1
            // 
            this.infoControl1.Location = new System.Drawing.Point( 24, 32 );
            this.infoControl1.Message = "";
            this.infoControl1.Name = "infoControl1";
            this.infoControl1.Size = new System.Drawing.Size( 960, 22 );
            this.infoControl1.TabIndex = 54;
            this.infoControl1.TabStop = false;
            // 
            // panelFromBottomArea
            // 
            this.panelFromBottomArea.Controls.Add( this.lblHospitalServiceVal );
            this.panelFromBottomArea.Controls.Add( this.lblPatientTypeVal );
            this.panelFromBottomArea.Controls.Add( this.lblAdmitSourceVal );
            this.panelFromBottomArea.Controls.Add( this.lblLocation );
            this.panelFromBottomArea.Controls.Add( this.lblHospitalService );
            this.panelFromBottomArea.Controls.Add( this.lblPatientType );
            this.panelFromBottomArea.Controls.Add( this.lblAdmitSource );
            this.panelFromBottomArea.Controls.Add( this.lblLocationVal );
            this.panelFromBottomArea.Location = new System.Drawing.Point( 4, 171 );
            this.panelFromBottomArea.Name = "panelFromBottomArea";
            this.panelFromBottomArea.Size = new System.Drawing.Size( 290, 96 );
            this.panelFromBottomArea.TabIndex = 52;
            // 
            // lblHospitalServiceVal
            // 
            this.lblHospitalServiceVal.Location = new System.Drawing.Point( 96, 53 );
            this.lblHospitalServiceVal.Name = "lblHospitalServiceVal";
            this.lblHospitalServiceVal.Size = new System.Drawing.Size( 187, 16 );
            this.lblHospitalServiceVal.TabIndex = 23;
            // 
            // lblPatientTypeVal
            // 
            this.lblPatientTypeVal.Location = new System.Drawing.Point( 96, 30 );
            this.lblPatientTypeVal.Name = "lblPatientTypeVal";
            this.lblPatientTypeVal.Size = new System.Drawing.Size( 194, 16 );
            this.lblPatientTypeVal.TabIndex = 22;
            // 
            // lblAdmitSourceVal
            // 
            this.lblAdmitSourceVal.Location = new System.Drawing.Point( 96, 3 );
            this.lblAdmitSourceVal.Name = "lblAdmitSourceVal";
            this.lblAdmitSourceVal.Size = new System.Drawing.Size( 192, 17 );
            this.lblAdmitSourceVal.TabIndex = 21;
            // 
            // lblLocation
            // 
            this.lblLocation.Location = new System.Drawing.Point( 10, 76 );
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size( 71, 25 );
            this.lblLocation.TabIndex = 15;
            this.lblLocation.Text = "Location:";
            // 
            // lblHospitalService
            // 
            this.lblHospitalService.Location = new System.Drawing.Point( 9, 53 );
            this.lblHospitalService.Name = "lblHospitalService";
            this.lblHospitalService.Size = new System.Drawing.Size( 88, 23 );
            this.lblHospitalService.TabIndex = 14;
            this.lblHospitalService.Text = "Hospital service:";
            // 
            // lblPatientType
            // 
            this.lblPatientType.Location = new System.Drawing.Point( 9, 29 );
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size( 78, 23 );
            this.lblPatientType.TabIndex = 13;
            this.lblPatientType.Text = "Patient type:";
            // 
            // lblAdmitSource
            // 
            this.lblAdmitSource.Location = new System.Drawing.Point( 9, 3 );
            this.lblAdmitSource.Name = "lblAdmitSource";
            this.lblAdmitSource.Size = new System.Drawing.Size( 88, 23 );
            this.lblAdmitSource.TabIndex = 12;
            this.lblAdmitSource.Text = "Admit source:";
            // 
            // lblLocationVal
            // 
            this.lblLocationVal.Location = new System.Drawing.Point( 96, 77 );
            this.lblLocationVal.Name = "lblLocationVal";
            this.lblLocationVal.Size = new System.Drawing.Size( 186, 16 );
            this.lblLocationVal.TabIndex = 24;
            // 
            // panelToArea
            // 
            this.panelToArea.Controls.Add( this.cmbAlternateCareFacility );
            this.panelToArea.Controls.Add( this.lblAlternateCareFacility );
            this.panelToArea.Controls.Add( this.cmbReasonForPrivateAccommodation );
            this.panelToArea.Controls.Add( this.lblReasonforAccommodation );
            this.panelToArea.Controls.Add( this.dateTimePicker );
            this.panelToArea.Controls.Add( this.lblPatientTypeToVal );
            this.panelToArea.Controls.Add( this.cboAccommodations );
            this.panelToArea.Controls.Add( this.lblAccomodation );
            this.panelToArea.Controls.Add( this.lineLabel2 );
            this.panelToArea.Controls.Add( this.locationView1 );
            this.panelToArea.Controls.Add( this.mtbTransferTime );
            this.panelToArea.Controls.Add( this.cboAdmitSource );
            this.panelToArea.Controls.Add( this.lblAdmitSource2 );
            this.panelToArea.Controls.Add( this.lblHospitalService2 );
            this.panelToArea.Controls.Add( this.mtbTransferDate );
            this.panelToArea.Controls.Add( this.lblTime );
            this.panelToArea.Controls.Add( this.lblPatientType2 );
            this.panelToArea.Controls.Add( this.lblTransferDate );
            this.panelToArea.Controls.Add( this.cboHospitalService );
            this.panelToArea.Location = new System.Drawing.Point( 309, 133 );
            this.panelToArea.Name = "panelToArea";
            this.panelToArea.Size = new System.Drawing.Size( 344, 376 );
            this.panelToArea.TabIndex = 0;
            // 
            // cmbAlternateCareFacility
            // 
            this.cmbAlternateCareFacility.DisplayMember = "Description";
            this.cmbAlternateCareFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlternateCareFacility.Location = new System.Drawing.Point( 109, 62 );
            this.cmbAlternateCareFacility.MaxLength = 27;
            this.cmbAlternateCareFacility.Name = "cmbAlternateCareFacility";
            this.cmbAlternateCareFacility.Size = new System.Drawing.Size( 192, 21 );
            this.cmbAlternateCareFacility.TabIndex = 1;
            this.cmbAlternateCareFacility.SelectedIndexChanged += new System.EventHandler( this.cmbAlternateCareFacility_SelectedIndexChanged );
            this.cmbAlternateCareFacility.Validating += new System.ComponentModel.CancelEventHandler( this.cmbAlternateCareFacility_Validating );
            // 
            // lblAlternateCareFacility
            // 
            this.lblAlternateCareFacility.Location = new System.Drawing.Point( 15, 57 );
            this.lblAlternateCareFacility.Name = "lblAlternateCareFacility";
            this.lblAlternateCareFacility.Size = new System.Drawing.Size( 82, 27 );
            this.lblAlternateCareFacility.TabIndex = 64;
            this.lblAlternateCareFacility.Text = "Nursing home/ Alt care facility";
            // 
            // cmbReasonForPrivateAccommodation
            // 
            this.cmbReasonForPrivateAccommodation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReasonForPrivateAccommodation.Enabled = false;
            this.cmbReasonForPrivateAccommodation.Items.AddRange( new object[] {
            "Private room is medically necessary",
            "Semi - private room is not available",
            "Patient requested a private room"} );
            this.cmbReasonForPrivateAccommodation.Location = new System.Drawing.Point( 118, 312 );
            this.cmbReasonForPrivateAccommodation.Name = "cmbReasonForPrivateAccommodation";
            this.cmbReasonForPrivateAccommodation.Size = new System.Drawing.Size( 217, 21 );
            this.cmbReasonForPrivateAccommodation.TabIndex = 5;
            this.cmbReasonForPrivateAccommodation.SelectedIndexChanged += new System.EventHandler( this.cmbReasonForPrivateAccommodation_SelectedIndexChanged );
            this.cmbReasonForPrivateAccommodation.Validating += new System.ComponentModel.CancelEventHandler( this.cmbReasonForPrivateAccommodation_Validating );
            // 
            // lblReasonforAccommodation
            // 
            this.lblReasonforAccommodation.Location = new System.Drawing.Point( 15, 313 );
            this.lblReasonforAccommodation.Name = "lblReasonforAccommodation";
            this.lblReasonforAccommodation.Size = new System.Drawing.Size( 101, 30 );
            this.lblReasonforAccommodation.TabIndex = 62;
            this.lblReasonforAccommodation.Text = "Reason For Private Accommodation:";
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker.Location = new System.Drawing.Point( 153, 349 );
            this.dateTimePicker.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size( 21, 20 );
            this.dateTimePicker.TabIndex = 6;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.CloseUp += new System.EventHandler( this.dateTimePicker_CloseUp );
            // 
            // lblPatientTypeToVal
            // 
            this.lblPatientTypeToVal.Location = new System.Drawing.Point( 110, 96 );
            this.lblPatientTypeToVal.Name = "lblPatientTypeToVal";
            this.lblPatientTypeToVal.Size = new System.Drawing.Size( 214, 16 );
            this.lblPatientTypeToVal.TabIndex = 61;
            // 
            // cboAccommodations
            // 
            this.cboAccommodations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAccommodations.Enabled = false;
            this.cboAccommodations.Location = new System.Drawing.Point( 118, 282 );
            this.cboAccommodations.Name = "cboAccommodations";
            this.cboAccommodations.Size = new System.Drawing.Size( 110, 21 );
            this.cboAccommodations.TabIndex = 4;
            this.cboAccommodations.SelectedIndexChanged += new System.EventHandler( this.cboAccommodations_SelectedIndexChanged );
            // 
            // lblAccomodation
            // 
            this.lblAccomodation.AutoSize = true;
            this.lblAccomodation.Location = new System.Drawing.Point( 16, 286 );
            this.lblAccomodation.Name = "lblAccomodation";
            this.lblAccomodation.Size = new System.Drawing.Size( 86, 13 );
            this.lblAccomodation.TabIndex = 57;
            this.lblAccomodation.Text = "Accommodation:";
            // 
            // lineLabel2
            // 
            this.lineLabel2.Caption = "Transfer to";
            this.lineLabel2.Location = new System.Drawing.Point( 12, 7 );
            this.lineLabel2.Name = "lineLabel2";
            this.lineLabel2.Size = new System.Drawing.Size( 321, 18 );
            this.lineLabel2.TabIndex = 0;
            this.lineLabel2.TabStop = false;
            // 
            // locationView1
            // 
            this.locationView1.EditFindButtonText = "Find...";
            this.locationView1.EditVerifyButtonText = "Verify";
            this.locationView1.Location = new System.Drawing.Point( 13, 140 );
            this.locationView1.Model = null;
            this.locationView1.Name = "locationView1";
            this.locationView1.Size = new System.Drawing.Size( 328, 133 );
            this.locationView1.TabIndex = 3;
            this.locationView1.BedSelected += new System.EventHandler( this.locationView1_BedSelected );
            // 
            // mtbTransferTime
            // 
            this.mtbTransferTime.KeyPressExpression = "^\\d*$";
            this.mtbTransferTime.Location = new System.Drawing.Point( 227, 348 );
            this.mtbTransferTime.Mask = "  :";
            this.mtbTransferTime.MaxLength = 5;
            this.mtbTransferTime.Multiline = true;
            this.mtbTransferTime.Name = "mtbTransferTime";
            this.mtbTransferTime.Size = new System.Drawing.Size( 43, 20 );
            this.mtbTransferTime.TabIndex = 7;
            this.mtbTransferTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbTransferTime.Validating += new System.ComponentModel.CancelEventHandler( this.mtbTransferTime_Validating );
            // 
            // cboAdmitSource
            // 
            this.cboAdmitSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAdmitSource.Location = new System.Drawing.Point( 109, 31 );
            this.cboAdmitSource.Name = "cboAdmitSource";
            this.cboAdmitSource.Size = new System.Drawing.Size( 192, 21 );
            this.cboAdmitSource.TabIndex = 0;
            this.cboAdmitSource.DropDown += new System.EventHandler( this.cboAdmitSource_DropDown );
            this.cboAdmitSource.SelectedIndexChanged += new System.EventHandler( this.cboAdmitSource_SelectIndexChanged );
            this.cboAdmitSource.Validating += new System.ComponentModel.CancelEventHandler( this.cboAdmitSource_Validating );
            // 
            // lblAdmitSource2
            // 
            this.lblAdmitSource2.Location = new System.Drawing.Point( 13, 31 );
            this.lblAdmitSource2.Name = "lblAdmitSource2";
            this.lblAdmitSource2.Size = new System.Drawing.Size( 88, 23 );
            this.lblAdmitSource2.TabIndex = 32;
            this.lblAdmitSource2.Text = "Admit source:";
            // 
            // lblHospitalService2
            // 
            this.lblHospitalService2.Location = new System.Drawing.Point( 13, 117 );
            this.lblHospitalService2.Name = "lblHospitalService2";
            this.lblHospitalService2.Size = new System.Drawing.Size( 88, 23 );
            this.lblHospitalService2.TabIndex = 37;
            this.lblHospitalService2.Text = "Hospital service:";
            // 
            // mtbTransferDate
            // 
            this.mtbTransferDate.KeyPressExpression = "^\\d*$";
            this.mtbTransferDate.Location = new System.Drawing.Point( 87, 349 );
            this.mtbTransferDate.Mask = "  /  /";
            this.mtbTransferDate.MaxLength = 10;
            this.mtbTransferDate.Name = "mtbTransferDate";
            this.mtbTransferDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbTransferDate.TabIndex = 6;
            this.mtbTransferDate.ValidationExpression = resources.GetString( "mtbTransferDate.ValidationExpression" );
            this.mtbTransferDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbTransferDate_Validating );
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point( 191, 353 );
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size( 39, 16 );
            this.lblTime.TabIndex = 6;
            this.lblTime.Text = "Time:";
            // 
            // lblPatientType2
            // 
            this.lblPatientType2.Location = new System.Drawing.Point( 13, 93 );
            this.lblPatientType2.Name = "lblPatientType2";
            this.lblPatientType2.Size = new System.Drawing.Size( 88, 23 );
            this.lblPatientType2.TabIndex = 34;
            this.lblPatientType2.Text = "Patient type:";
            // 
            // lblTransferDate
            // 
            this.lblTransferDate.Location = new System.Drawing.Point( 13, 350 );
            this.lblTransferDate.Name = "lblTransferDate";
            this.lblTransferDate.Size = new System.Drawing.Size( 88, 18 );
            this.lblTransferDate.TabIndex = 42;
            this.lblTransferDate.Text = "Transfer date:";
            // 
            // cboHospitalService
            // 
            this.cboHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHospitalService.Location = new System.Drawing.Point( 109, 117 );
            this.cboHospitalService.Name = "cboHospitalService";
            this.cboHospitalService.Size = new System.Drawing.Size( 224, 21 );
            this.cboHospitalService.TabIndex = 2;
            this.cboHospitalService.DropDown += new System.EventHandler( this.cboHospitalService_DropDown );
            this.cboHospitalService.SelectedIndexChanged += new System.EventHandler( this.cboHospitalService_SelectedIndexChanged );
            // 
            // lblAdmitTimeVal
            // 
            this.lblAdmitTimeVal.Location = new System.Drawing.Point( 243, 107 );
            this.lblAdmitTimeVal.Name = "lblAdmitTimeVal";
            this.lblAdmitTimeVal.Size = new System.Drawing.Size( 56, 13 );
            this.lblAdmitTimeVal.TabIndex = 49;
            // 
            // lblAdmitTime
            // 
            this.lblAdmitTime.Location = new System.Drawing.Point( 203, 107 );
            this.lblAdmitTime.Name = "lblAdmitTime";
            this.lblAdmitTime.Size = new System.Drawing.Size( 35, 13 );
            this.lblAdmitTime.TabIndex = 48;
            this.lblAdmitTime.Text = "Time:";
            // 
            // lblAdmitDateVal
            // 
            this.lblAdmitDateVal.Location = new System.Drawing.Point( 100, 107 );
            this.lblAdmitDateVal.Name = "lblAdmitDateVal";
            this.lblAdmitDateVal.Size = new System.Drawing.Size( 88, 16 );
            this.lblAdmitDateVal.TabIndex = 18;
            // 
            // lblAccountVal
            // 
            this.lblAccountVal.Location = new System.Drawing.Point( 100, 83 );
            this.lblAccountVal.Name = "lblAccountVal";
            this.lblAccountVal.Size = new System.Drawing.Size( 216, 16 );
            this.lblAccountVal.TabIndex = 17;
            // 
            // lblPatientNameVal
            // 
            this.lblPatientNameVal.Location = new System.Drawing.Point( 100, 59 );
            this.lblPatientNameVal.Name = "lblPatientNameVal";
            this.lblPatientNameVal.Size = new System.Drawing.Size( 224, 16 );
            this.lblPatientNameVal.TabIndex = 16;
            // 
            // lblAdmitDate
            // 
            this.lblAdmitDate.Location = new System.Drawing.Point( 12, 107 );
            this.lblAdmitDate.Name = "lblAdmitDate";
            this.lblAdmitDate.Size = new System.Drawing.Size( 88, 23 );
            this.lblAdmitDate.TabIndex = 2;
            this.lblAdmitDate.Text = "Admit date:";
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point( 12, 83 );
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size( 88, 23 );
            this.lblAccount.TabIndex = 1;
            this.lblAccount.Text = "Account:";
            // 
            // lblPatientName
            // 
            this.lblPatientName.Location = new System.Drawing.Point( 12, 59 );
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size( 80, 23 );
            this.lblPatientName.TabIndex = 0;
            this.lblPatientName.Text = "Patient name:";
            // 
            // ProgressPanel1
            // 
            this.ProgressPanel1.BackColor = System.Drawing.Color.White;
            this.ProgressPanel1.Location = new System.Drawing.Point( 7, 6 );
            this.ProgressPanel1.Name = "ProgressPanel1";
            this.ProgressPanel1.Size = new System.Drawing.Size( 993, 503 );
            this.ProgressPanel1.TabIndex = 57;
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.SystemColors.Control;
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point( 944, 594 );
            this.btnNext.Message = null;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size( 72, 23 );
            this.btnNext.TabIndex = 17;
            this.btnNext.Text = "&Next >";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler( this.btnNext_Click );
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point( 776, 594 );
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 23 );
            this.btnCancel.TabIndex = 15;
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
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point( 864, 594 );
            this.btnBack.Message = null;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size( 75, 23 );
            this.btnBack.TabIndex = 16;
            this.btnBack.Text = "< &Back";
            this.btnBack.UseVisualStyleBackColor = false;
            // 
            // TransferOutPatToInPatStep1View
            // 
            this.AcceptButton = this.btnNext;
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.Controls.Add( this.btnBack );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.btnCancel );
            this.Controls.Add( this.btnNext );
            this.Controls.Add( this.panelTransferOutPatToInPat );
            this.Controls.Add( this.panelUserContext );
            this.Name = "TransferOutPatToInPatStep1View";
            this.Size = new System.Drawing.Size( 1024, 632 );
            this.Leave += new System.EventHandler( this.TransferOutPatToInPatView_LeaveView );
            this.Validating += new System.ComponentModel.CancelEventHandler( this.TransferOutPatToInPatStep1View_Validating );
            this.Disposed += new System.EventHandler( this.TransferOutPatToInPatStep1View_Disposed );
            this.panelTransferOutPatToInPat.ResumeLayout( false );
            this.panelTransferOutPatToInPat.PerformLayout();
            this.gpgSpan.ResumeLayout( false );
            this.gpgSpan.PerformLayout();
            this.panelFromBottomArea.ResumeLayout( false );
            this.panelToArea.ResumeLayout( false );
            this.panelToArea.PerformLayout();
            this.panelUserContext.ResumeLayout( false );
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #region Data Elements

        private IContainer components = null;

        private ProgressPanel ProgressPanel1;
        private LocationView locationView1;
        private UserContextView userContextView1;
        private PatientContextView patientContextView1;
        private InfoControl infoControl1;
        private LineLabel lineLabel2;
        private LineLabel llblTransferFrom;
        private OccurrenceSpan i_SysGenSpan1;
        private OccurrenceSpan i_SysGenSpan2;
        private RuleEngine i_RuleEngine;
        private ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();

        private MaskedEditTextBox mtbFacility;
        private MaskedEditTextBox mtbTransferDate;
        private MaskedEditTextBox mtbSpan1FromDate;
        private MaskedEditTextBox mtbSpan1ToDate;
        private MaskedEditTextBox mtbSpan2FromDate;
        private MaskedEditTextBox mtbSpan2ToDate;

        public MaskedEditTextBox mtbChiefComplaint;
        private MaskedEditTextBox mtbTransferTime;

        private DateTimePicker dateTimePicker;
        private DateTimePicker dtpSpan1From;
        private DateTimePicker dtpSpan1To;
        private DateTimePicker dtpSpan2From;
        private DateTimePicker dtpSpan2To;

        private Panel panelTransferOutPatToInPat;
        private Panel panelToArea;
        private Panel panelFromBottomArea;
        private Panel panelUserContext;
        private Panel panel1;

        private LoggingButton btnCancel;
        private LoggingButton btnNext;
        private LoggingButton btnBack;

        private DateTime i_FacilityDateTime;

        private ComboBox cboSpan1;
        private ComboBox cboSpan2;
        private ComboBox cboAdmitSource;
        private ComboBox cboHospitalService;
        private ComboBox cboAccommodations;
        private PatientAccessComboBox cmbAlternateCareFacility;

        private GroupBox gpgSpan;

        private Label lblAlternateCareFacility;
        private Label lblPatientName;
        private Label lblAccount;
        private Label lblAdmitDate;
        private Label lblHospitalService;
        private Label lblPatientType;
        private Label lblAdmitSource;
        private Label lblLocation;
        private Label lblPatientNameVal;
        private Label lblAccountVal;
        private Label lblAdmitDateVal;
        private Label lblAdmitSourceVal;
        private Label lblPatientTypeVal;
        private Label lblHospitalServiceVal;
        private Label lblLocationVal;
        private Label lblTransferDate;
        private Label lblHospitalService2;
        private Label lblPatientType2;
        private Label lblAdmitSource2;
        private Label lblTime;
        private Label lblAccomodation;
        private Label lblStep1;
        private Label lblSpan1;
        private Label lblPatientTypeToVal;
        private Label lblSpan2;
        private Label lblFac;
        private Label lblSpan1FromDate;
        private Label lblSpan1ToDate;
        private Label lblSpan2FromDate;
        private Label lblSpan2ToDate;
        private Label lblChiefComplaint;

        private Label lblAdmitTime;
        private Label lblAdmitTimeVal;

        // Flag to prevent multiple error message boxes from appearing
        private bool inErrorState;
        private bool i_PreValidationSuccess = true;
        private bool blnLeave;

        private string i_OriginalChiefComplaint;
        private PatientAccessComboBox cmbReasonForPrivateAccommodation;
        private Label lblReasonforAccommodation;

        private const long BLANK_OPTION_OID = -1L;

        private IAlternateCareFacilityPresenter alternateCareFacilityPresenter;
        private readonly Regex emailValidationExpression;
        private readonly Regex emailKeyPressExpression;

        #endregion


        #region Data Elements
        private Accomodation selectedAccomodation;
        #endregion

        #region Constants
        // The list of codes will always have a blank item in it, so a count
        //   of one means there are no codes
        private const int NO_ACCOMMODATION_CODES = 1;
        private const string ADMITTING_CATEGORY_URGENT = "3";
        private PatientPortalOptInView patientPortalOptInView;
        private Label lblEmail;
        private MaskedEditTextBox mtbEmail;
        private const string ADMITTING_CATEGORY_EMERGENCY = "2";
        
        #endregion



    }

    class SortAccountsByDischarge : IComparer
    {
        public int Compare( object obj1, object obj2 )
        {
            AccountProxy a = ( AccountProxy )obj1;
            AccountProxy b = ( AccountProxy )obj2;

            return a.DischargeDate.CompareTo( b.DischargeDate );
        }
    }
}

