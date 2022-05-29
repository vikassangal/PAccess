using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DischargeViews
{
    /// <summary>
    /// Summary description for CancelDischarge.
    /// </summary>
    //TODO: Create XML summary comment for CancelDischarge
    [Serializable]
    public class CancelInpatientDischargeView : DischargeBaseView
    {
        #region Events
        #endregion

        #region Event Handlers
        private new void btnOk_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
			this.CancelInpatientDischarge();
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

		        if( this.Model.DischargeDate.Year.ToString() != "1" &&
			        this.Model.KindOfVisit.Code == VisitType.INPATIENT )
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

			        this.mtbDischargeDate.Visible = false;
			        this.mtbDischargeTime.Visible = false;
			        this.dtpDischargeDate.Visible = false;
			        this.cmbDischargeDisposition.Visible = false;
			        this.lblDischargeDispositionVal.Text = this.Model.DischargeDisposition.Description;

                    if( this.Model.Location != null )
                    {
	                    this.lblDischargeLocation.Text = this.Model.Location.DisplayString;
                    }

			        /*  TODO: 
				        *  Check for pending admissions or Occupied by patient
			        */
                    DisplayCurrentOccupant();

                    if( !this.IsMedicalAbstractComplete() )
                    {
	                    if( !this.IsDischargePastThreeMidnights( UIErrorMessages.CANCEL_DISCHARGE_PAST_THREE_MIDNIGHTS_INSTRUCTIONAL_MSG ) )
	                    {
		                    if( !this.CheckForDischargeToday() )
		                    {
			                    this.IsPendingAdmission( UIErrorMessages.CANCEL_DISCHARGE_BED_PENDING_OR_OCCUPIED_INSTRUCTIONAL_MSG );
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
			        //Discharge date not available or patient not INPATIENT
			        this.lblInstructionalMessage.Text = UIErrorMessages.CANCEL_DISCHARGE_NOT_INPATIENT_MSG;
			        this.IsDischargeCanceled = true;
			        this.btnOk.Enabled = false;
		        }

		        this.btnOk.Focus();
	        }
        }

		public override void UpdateModel()
		{
		}
		#endregion

        #region Properties
        public new Account Model
        {
	        private get
	        {
		        return (Account)base.Model;
	        }
	        set
	        {
		        base.Model = value;
	        }
        }
		#endregion

        #region Private Methods

        private void CancelInpatientDischarge()
        {
	        if( this.IsDischargePastThreeMidnights( UIErrorMessages.CANCEL_DISCHARGE_PAST_THREE_MIDNIGHTS_OK_BUTTON_MSG ) )
	        {
                this.btnOk.Enabled = true;
		        return;
	        }

            this.Cursor = Cursors.WaitCursor;

            this.reserveBed();   

	        this.Model.DischargeStatus = DischargeStatus.NotDischarged();
	        this.Model.DischargeDate = DateTime.MinValue;

            this.SaveAccount();

			this.lblNextAction.Visible = true;
			this.lblNextActionLine.Visible = true;

	        this.panelButtons.Hide();

	        this.panelActions.Show();
            this.gbScanDocuments.Visible = true;
        	
	        this.lblInstructionalMessage.Text = "Cancel Inpatient Discharge submitted.";
	        this.userContextView1.Description = "Cancel Inpatient Discharge - Submitted";

	        this.Cursor = Cursors.Default;

	        this.btnCloseActivity.Focus();

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
        }

        private void DisplayCurrentOccupant()
        {
            if( reservationResult == null )
            {
                LocationBrokerProxy broker =  new LocationBrokerProxy( );

                reservationResult = broker.GetBedStatus ( this.Model.Location, this.Model.Facility );
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
                reservationCriteria.NewLocation = this.Model.Location;
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

                reservationResult = broker.GetBedStatus ( this.Model.Location, this.Model.Facility );
            }

            if( reservationResult.Message == ReservationResults.MSG_RESERVED )
            {
                string newLine = this.lblInstructionalMessage.Text == String.Empty? string.Empty : "\n\r";

                this.lblInstructionalMessage.Text = this.lblInstructionalMessage.Text 
                    + newLine + errorMsg;
                this.btnOk.Enabled = false;

                return true;
            }
            else if( reservationResult.Message != ReservationResults.MSG_AVAILABLE )
            {                
                string newLine = this.lblInstructionalMessage.Text == String.Empty? string.Empty : "\n\r";

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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelActions.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelMessages.SuspendLayout();
            this.gbScanDocuments.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView1
            // 
            this.userContextView1.Description = "Cancel Inpatient Discharge";
            // 
            // mtbDischargeDate
            // 
            this.mtbDischargeDate.MaxLength = 10;
            this.mtbDischargeDate.Size = new System.Drawing.Size( 89, 20 );
            // 
            // mtbDischargeTime
            // 
            this.mtbDischargeTime.Location = new System.Drawing.Point( 245, 163 );
            // 
            // btnOk
            // 
            this.btnOk.Click += new System.EventHandler( this.btnOk_Click );
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.TabIndex = 1;
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.TabIndex = 2;
            // 
            // btnEditAccount
            // 
            this.btnEditAccount.Size = new System.Drawing.Size( 125, 23 );
            this.btnEditAccount.TabIndex = 3;
            // 
            // dtpDischargeDate
            // 
            this.dtpDischargeDate.TabStop = false;
            this.dtpDischargeDate.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Size = new System.Drawing.Size( 999, 515 );
            this.panel2.TabIndex = 2;
            // 
            // lblInstructionalMessage
            // 
            this.lblInstructionalMessage.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, ( (System.Drawing.FontStyle)( ( System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic ) ) ), System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblInstructionalMessage.Location = new System.Drawing.Point( 12, 10 );
            // 
            // label2
            // 
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Size = new System.Drawing.Size( 937, 16 );
            // 
            // lblOutstandingActionItemsMsg
            // 
            this.lblOutstandingActionItemsMsg.Location = new System.Drawing.Point( 12, 67 );
            this.lblOutstandingActionItemsMsg.Size = new System.Drawing.Size( 912, 35 );
            // 
            // lblMessages
            // 
            this.lblMessages.Size = new System.Drawing.Size( 957, 28 );
            // 
            // lblDischargeDispositionVal
            // 
            this.lblDischargeDispositionVal.Location = new System.Drawing.Point( 128, 189 );
            this.lblDischargeDispositionVal.Size = new System.Drawing.Size( 213, 20 );
            this.lblDischargeDispositionVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDischargeDispositionVal.Visible = true;
            // 
            // lblDischargeDateVal
            // 
            this.lblDischargeDateVal.Location = new System.Drawing.Point( 98, 163 );
            this.lblDischargeDateVal.Size = new System.Drawing.Size( 99, 19 );
            this.lblDischargeDateVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDischargeDateVal.Visible = true;
            // 
            // lblDischargeTimeVal
            // 
            this.lblDischargeTimeVal.Size = new System.Drawing.Size( 56, 17 );
            this.lblDischargeTimeVal.Text = "";
            this.lblDischargeTimeVal.Visible = true;
            // 
            // CancelInpatientDischargeView
            // 
            this.AcceptButton = this.btnOk;
            this.Name = "CancelInpatientDischargeView";
            this.Size = new System.Drawing.Size( 1037, 618 );
            this.panelActions.ResumeLayout( false );
            this.panel2.ResumeLayout( false );
            this.panel2.PerformLayout();
            this.panelPatientContext.ResumeLayout( false );
            this.panelButtons.ResumeLayout( false );
            this.panelMessages.ResumeLayout( false );
            this.gbScanDocuments.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #region Private Properties
		#endregion

        #region Construction and Finalization
        public CancelInpatientDischargeView()
        {
	        // This call is required by the Windows.Forms Form Designer.
	        InitializeComponent();
            EnableThemesOn( this );

	        // TODO: Add any initialization after the InitializeComponent call
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
	            if(components != null)
	            {
		            components.Dispose();
	            }
            }
            base.Dispose( disposing );
        }
		#endregion

        #region Data Elements
        private Container components = null;
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
