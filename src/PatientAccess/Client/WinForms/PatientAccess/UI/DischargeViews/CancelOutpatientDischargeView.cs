using System;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DischargeViews
{
    public partial class CancelOutpatientDischargeView : DischargeBaseView
    {
        #region Events
        #endregion

        #region Event Handlers
        private new void btnOk_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
			this.CancelOutpatientDischarge();
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
	        if( this.Model != null )
	        {
		        this.DisplayPatientContext();
        		
		        //PopulateControls
		        this.lblPatientName.Text = this.Model.Patient.FormattedName;
		        this.lblAccount.Text = this.Model.AccountNumber.ToString();
		        this.lblPatientType.Text = this.Model.KindOfVisit.DisplayString;

                this.lblAdmitDate.Text = CommonFormatting.LongDateFormat( this.Model.AdmitDate );
                this.lblAdmitTime.Text = CommonFormatting.DisplayedTimeFormat( this.Model.AdmitDate );

                // SR39492 - Validate that the selected patient has a discharge date and is either a:
                // a) Patient Type 2 (Outpatient) where the Daycare flag has been set to ‘Y’ (for the specified Hospital
                //    Service Code indicating a bed assignment, such as in the case of HSV’s 58, 59, FO, LD or LB) or a
                // b) Patient Type 3 (Outpatient - ER) Hospital Service Code 65.
		        if( this.Model.DischargeDate != DateTime.MinValue &&
                    this.Model.DischargeDate.Year.ToString() != "1" &&
				    ( IsPatientTypeOutpatientInBed() ||
					  ( this.Model.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT &&
                        this.Model.HospitalService.Code.Equals( HospitalService.EMERGENCY_ROOM ) ) ) )
		        {
                    this.mtbDischargeDate.UnMaskedText = CommonFormatting.MaskedDateFormat( this.Model.DischargeDate );
			        this.lblDischargeDateVal.Text = CommonFormatting.LongDateFormat( this.Model.DischargeDate );
                    
                    if( this.Model.DischargeDate.Hour != 0 ||
                        this.Model.DischargeDate.Minute != 0 )
                    {
                        this.mtbDischargeTime.UnMaskedText = CommonFormatting.MaskedTimeFormat( this.Model.DischargeDate );
                    }
                    else
                    {
                        this.mtbDischargeTime.UnMaskedText = "";
                    }			        
                    this.lblDischargeTimeVal.Text = CommonFormatting.DisplayedTimeFormat( this.Model.DischargeDate );

			        this.lblDischargeDispositionVal.Text = this.Model.DischargeDisposition.Description;
       
                    if( this.Model.PreDischargeLocation != null )
                    {
	                    this.lblDischargeLocation.Text = this.Model.PreDischargeLocation.DisplayString;
                    }

                    DisplayCurrentOccupant();

                    if( !this.IsMedicalAbstractComplete() )
                    {
	                    if( !this.IsDischargePastThreeMidnights( UIErrorMessages.CANCEL_DISCHARGE_PAST_THREE_MIDNIGHTS_INSTRUCTIONAL_MSG ) )
	                    {
		                    if( !this.CheckForDischargeToday() )
		                    {
                                if( this.Model.PreDischargeLocation != null )
                                {
                                    // If PatientType = 2 (OUTPATIENT), get Bed status to check its Pending Admission / Reserved status
                                    if( IsPatientTypeOutpatientInBed() )
                                    {
	    		                        this.IsPendingAdmission( UIErrorMessages.CANCEL_DISCHARGE_BED_PENDING_OR_OCCUPIED_INSTRUCTIONAL_MSG );
    		                        }
		                        }
		                    }
	                    }
                    }
        			
			        if( this.Model.Facility.IsOrderCommunicationFacility && !this.IsDischargeCanceled )
			        {
				        this.lblMessages.Text = UIErrorMessages.CANCEL_DISCHARGE_OC_SYSTEM_MSG;
			        }
		        }
		        else
		        {
			        //Discharge date not available or patient not OUTPATIENT with Bed or OP-ER PATIENT
			        this.lblInstructionalMessage.Text = UIErrorMessages.CANCEL_DISCHARGE_NOT_OP_OR_ER_PATIENT_MSG;
			        this.IsDischargeCanceled = true;
			        this.btnOk.Enabled = false;
		        }

                this.mtbDischargeDate.Visible = false;
			    this.mtbDischargeTime.Visible = false;
			    this.dtpDischargeDate.Visible = false;
			    this.cmbDischargeDisposition.Visible = false;
		        this.btnOk.Focus();
	        }
        }

		public override void UpdateModel()
		{
		}
        #endregion

        #region Private Methods

        private bool IsPatientTypeOutpatientInBed()
        {
            bool isValidHospitalService = 
                    ( this.Model.KindOfVisit.Code == VisitType.OUTPATIENT &&
                        this.Model.HospitalService.IsDayCare() &&
                        this.Model.HospitalService.DayCare == "Y" );

            bool isValidLocation = 
                    ( this.Model.PreDischargeLocation != null &&
                        this.Model.PreDischargeLocation.NursingStation != null && 
                        this.Model.PreDischargeLocation.NursingStation.Code != String.Empty &&
                        this.Model.PreDischargeLocation.Room != null && 
                        this.Model.PreDischargeLocation.Room.Code != String.Empty &&
                        this.Model.PreDischargeLocation.Bed != null && 
                        this.Model.PreDischargeLocation.Bed.Code != String.Empty );

			return isValidHospitalService && isValidLocation;
        }

        private void CancelOutpatientDischarge()
        {
	        if( this.IsDischargePastThreeMidnights( UIErrorMessages.CANCEL_DISCHARGE_PAST_THREE_MIDNIGHTS_OK_BUTTON_MSG ) )
	        {
                this.btnOk.Enabled = true;
		        return;
	        }

            this.Cursor = Cursors.WaitCursor;

            // Only for Patient Type 2 with Hospital Service Code ‘Day Care’ flag set to ‘Y’, 
            // reassign the patient to the bed in which they were located before discharge
		    if ( IsPatientTypeOutpatientInBed() )
            {
                this.reserveBed();   
            }

	        this.Model.DischargeStatus = DischargeStatus.NotDischarged();
	        this.Model.DischargeDate = DateTime.MinValue;
            this.Model.DischargeDisposition = new DischargeDisposition();

            this.SaveAccount();

			this.lblNextAction.Visible = true;
			this.lblNextActionLine.Visible = true;

            this.ResetDischargeFields();
	        this.panelButtons.Hide();

	        this.panelActions.Show();
            this.gbScanDocuments.Visible = true;
        	
	        this.lblInstructionalMessage.Text = "Cancel Outpatient Discharge submitted for processing.";
	        this.userContextView1.Description = "Cancel Outpatient Discharge - Submitted";
        	
            this.Cursor = Cursors.Default;

	        this.btnCloseActivity.Focus();

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
        }

        private void ResetDischargeFields()
        {
            // Reset Discharge related fields to empty strings as per wireframe
            this.mtbDischargeDate.UnMaskedText = String.Empty;
            this.lblDischargeDateVal.Text = String.Empty;
            
            this.mtbDischargeTime.UnMaskedText = String.Empty;
            this.lblDischargeTimeVal.Text = String.Empty;

	        this.lblDischargeDispositionVal.Text = this.Model.DischargeDisposition.Description;
            this.lblDischargeLocation.Text = String.Empty;
            this.lblCurrentOccupant.Text = String.Empty;
        }

        private void DisplayCurrentOccupant()
        {
            // If PatientType = 2 (OUTPATIENT), get Bed status to display current occupant in discharged from location
            if( IsPatientTypeOutpatientInBed() )
            {
                if( reservationResult == null )
                {
                    LocationBrokerProxy broker =  new LocationBrokerProxy( );

                    reservationResult = broker.GetBedStatus (this.Model.PreDischargeLocation, this.Model.Facility );
                }

                if( reservationResult.Message == ReservationResults.MSG_RESERVED )
                {
                    this.lblCurrentOccupant.Text = PENDING_ADMISSION;
                }
                else if( reservationResult.Message != ReservationResults.MSG_AVAILABLE )
                {
                    if( reservationResult.LastName != string.Empty )
                    {
                        this.lblCurrentOccupant.Text = reservationResult.LastName.Trim() + 
                            ", " + reservationResult.FirstName.Trim() + " " + 
                            reservationResult.MiddleInitial.Trim();
                    }                                    
                    else
                    {
                        this.lblCurrentOccupant.Text = reservationResult.Message;
                    }
                }
                else
                {
                    this.lblCurrentOccupant.Text = UNOCCUPIED;
                }
            }
            else
            {
                // If Patient Type = 3 (ER PATIENT)
                this.lblCurrentOccupant.Text = String.Empty;
            }
        }

        private bool IsMedicalAbstractComplete()
        {
	        if( this.Model.AbstractExists )
	        {
		        string newLine = "\n";
        		
		        if( this.lblInstructionalMessage.Text == String.Empty )
		        {
			        newLine = string.Empty;
		        }
		        this.lblInstructionalMessage.Text = this.lblInstructionalMessage.Text 
			        + newLine + UIErrorMessages.CANCEL_DISCHARGE_MEDICAL_ABSTRACT_COMPLETE_MSG;
		        this.btnOk.Enabled = false;
		        this.IsDischargeCanceled = true;
		        return true;
	        }
	        return false;
        }

        private bool IsDischargePastThreeMidnights( string errorMsg)
        {
	        if( DateTime.Today > this.Model.DischargeDate.AddDays( 4 ) )
	        {
		        string newLine = "\n";
        		
		        if( this.lblInstructionalMessage.Text == String.Empty )
		        {
			        newLine = string.Empty;
		        }
		        this.lblInstructionalMessage.Text = this.lblInstructionalMessage.Text 
			        + newLine + errorMsg;
		        this.btnOk.Enabled = false;
		        this.IsDischargeCanceled = true;
		        return true;
	        }
	        return false;
        }

        private bool CheckForDischargeToday()
        {
	        string newLine = "\n";


            IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            bool isTransactionAllowed = accountBroker.IsTxnAllowed(this.Model.Facility.Code, this.Model.AccountNumber, this.Model.Activity);

	        if( !isTransactionAllowed)
	        {
		        if( this.lblInstructionalMessage.Text == String.Empty )
		        {
			        newLine = string.Empty;
		        }
		        this.lblInstructionalMessage.Text = this.lblInstructionalMessage.Text 
			        + newLine + UIErrorMessages.CANCEL_DISCHARGE_PERFORMED_TODAY_MSG;
		        this.btnOk.Enabled = false;
		        this.IsDischargeCanceled = true;
		        return true;
	        }
	        return false;
        }

        private bool reserveBed()
        {
            if( !this.IsPendingAdmission(  UIErrorMessages.CANCEL_DISCHARGE_BED_PENDING_OR_OCCUPIED_OK_BTTTON_MSG ) )
            {
                LocationBrokerProxy broker =  new LocationBrokerProxy( );
            
                ReservationCriteria reservationCriteria = new ReservationCriteria();

                reservationCriteria.OldLocation = null;
                reservationCriteria.NewLocation = this.Model.PreDischargeLocation;
                reservationCriteria.PatientType = this.Model.KindOfVisit;
                reservationCriteria.Facility = this.Model.Facility;

                ReservationResults reservationResult = broker.Reserve( 
                    reservationCriteria );

                if( !reservationResult.ReservationSucceeded )
                {                
                    string newLine = this.lblInstructionalMessage.Text == String.Empty? "" : "\n\r";

                    this.lblInstructionalMessage.Text = this.lblInstructionalMessage.Text 
                        + newLine + "Bed reservation failed.";
                    this.btnOk.Enabled = false;

                    return true;
                }
                else
                {
                    return false;
                }
            }  
            else
            {
                return false;
            }
        }

        /// <summary>
        /// IsPendingAdmission - determines if the bed we are trying to reserve is already occupied
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool IsPendingAdmission( string errorMsg)
        {          
            if( reservationResult == null )
            {
                LocationBrokerProxy broker = new LocationBrokerProxy( ); 

                reservationResult = broker.GetBedStatus (this.Model.PreDischargeLocation, this.Model.Facility );
            }

            if( reservationResult.Message == ReservationResults.MSG_RESERVED )
            {
                string newLine = this.lblInstructionalMessage.Text == String.Empty? "" : "\n\r";

                this.lblInstructionalMessage.Text = this.lblInstructionalMessage.Text 
                    + newLine + errorMsg;
                this.btnOk.Enabled = false;

                return true;
            }
            else if( reservationResult.Message != ReservationResults.MSG_AVAILABLE )
            {                
                string newLine = this.lblInstructionalMessage.Text == String.Empty? "" : "\n\r";

                this.lblInstructionalMessage.Text = this.lblInstructionalMessage.Text 
                    + newLine + errorMsg;
                this.btnOk.Enabled = false;

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Properties
        public new Account Model
        {
	        private get
	        {
		        return base.Model;
	        }
	        set
	        {
		        base.Model = value;
	        }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CancelOutpatientDischargeView()
        {
	        // This call is required by the Windows.Forms Form Designer.
	        InitializeComponent();
            base.EnableThemesOn( this );
        }
        #endregion

        #region Data Elements
        private bool IsDischargeCanceled = false;
        private ReservationResults reservationResult = null;
        #endregion

        #region Constants
        private const string    
            PENDING_ADMISSION   = "PENDING ADMISSION",
            UNOCCUPIED          = "UNOCCUPIED";
        #endregion        
    }
}
