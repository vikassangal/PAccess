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
	/// Summary description for EditDischargeView.
	/// </summary>
	[Serializable]
	public class EditDischargeView : DischargeBaseView
	{
		#region Events
		#endregion

		#region Event Handlers
        private void mtbDischargeDate_Validating(object sender, CancelEventArgs e)
        {
            if( !dtpDischargeDate.Focused )
            {
                validateDateAndTime("DATE");
                ValidateRequiredFields();
            }        
        }

        private void mtbDischargeTime_Validating(object sender, CancelEventArgs e)
        {
            validateDateAndTime("TIME");
            ValidateRequiredFields();        
        }

        private void dtpDischargeDate_CloseUp(object sender, EventArgs e)
        {
            mtbDischargeDate.UnMaskedText = dtpDischargeDate.Value.ToString("MMddyyyy");
            SetDischargeDateNormalBgColor();
            ValidateRequiredFields();
            mtbDischargeDate.Focus();
        }

		private void cmbDischargeDisposition_SelectedIndexChanged(object sender, EventArgs e)
		{
			ValidateRequiredFields();

			if( cmbDischargeDisposition.SelectedIndex != -1 && cmbDischargeDisposition.SelectedItem.ToString() != String.Empty)
			{
				Model.DischargeDisposition = (DischargeDisposition)cmbDischargeDisposition.SelectedItem;
			}
		}

		private new void btnOk_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

            bool val = false;
            val = validateDateAndTime("NONE");

            if( val )
            {
                Cursor = Cursors.WaitCursor;
                EditInpatientDischarge();
                Cursor = Cursors.Default;
            }
            
			Cursor = Cursors.Default;
		}
		#endregion

		#region Methods
		public override void UpdateView()
		{
			if( Model != null )
			{
				DisplayPatientContext();

				//PopulateControls
				lblPatientName.Text = Model.Patient.FormattedName;
				lblAccount.Text = Model.AccountNumber.ToString();
				lblPatientType.Text = Model.KindOfVisit.DisplayString;

                lblAdmitDate.Text = CommonFormatting.LongDateFormat( Model.AdmitDate );
                lblAdmitTime.Text = CommonFormatting.DisplayedTimeFormat( Model.AdmitDate );

				DisplayDischargeDispositions();

				if( Model.DischargeDisposition != null )
				{
					int dischargeDispositionSelectedIndex = -1;
					dischargeDispositionSelectedIndex = 
						cmbDischargeDisposition.FindString( Model.DischargeDisposition.ToString() );

					if( dischargeDispositionSelectedIndex != -1 )
					{
						cmbDischargeDisposition.SelectedIndex = dischargeDispositionSelectedIndex;
					}
				}

				if( Model.Location != null )
				{
					lblDischargeLocation.Text = Model.Location.DisplayString;
				}

				if( Model.DischargeDate.Year.ToString() != "1" &&
					Model.KindOfVisit.Code == VisitType.INPATIENT )
				{
					btnOk.Enabled = true;

                    mtbDischargeDate.UnMaskedText = CommonFormatting.MaskedDateFormat( Model.DischargeDate );
                    
                    if( Model.DischargeDate.Hour != 0 ||
                        Model.DischargeDate.Minute != 0 )
                    {
                        mtbDischargeTime.UnMaskedText = CommonFormatting.MaskedTimeFormat( Model.DischargeDate );
                    }
                    else
                    {
                        mtbDischargeTime.UnMaskedText = string.Empty;
                    }                                        

					if( IsMedicalAbstractComplete() )
					{
						mtbDischargeDate.Enabled = false;
						dtpDischargeDate.Enabled = false;
						mtbDischargeTime.Enabled = false;
						cmbDischargeDisposition.Enabled = false;
						btnOk.Enabled = false;
					}
				}
				else
				{
					//Discharge date not available or patient not INPATIENT
					lblInstructionalMessage.Text = UIErrorMessages.EDIT_DISCHARGE_NOT_INPATIENT_MSG;
					mtbDischargeDate.Enabled = false;
					dtpDischargeDate.Enabled = false;
					mtbDischargeTime.Enabled = false;
					cmbDischargeDisposition.Enabled = false;
					btnOk.Enabled = false;
				}
				

				
				mtbDischargeDate.Focus();
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
				return base.Model;
			}
			set
			{
				base.Model = value;
			}
		}

	    private Activity CurrentActivity
        {
            get { return i_CurrentActivity ?? (i_CurrentActivity = new EditDischargeDataActivity()); }
            set
            {
                i_CurrentActivity = value;
            }
        }
		#endregion

		#region Private Methods

		private void ValidateRequiredFields()
		{
			if( mtbDischargeDate.UnMaskedText != String.Empty && 
				mtbDischargeTime.UnMaskedText != String.Empty &&
				cmbDischargeDisposition.SelectedIndex != -1 &&
                cmbDischargeDisposition.SelectedItem != null &&
                cmbDischargeDisposition.SelectedItem.ToString() != string.Empty)
			{
				btnOk.Enabled = true;
			}
			else
			{
				btnOk.Enabled = false;
			}
		}

		private void EditInpatientDischarge()
		{
            if (!IsPBARAvailable())
            {
                lblInstructionalMessage.Text = UIErrorMessages.PBAR_UNAVAILABLE_MSG;
                btnOk.Enabled = true;
                return;
            }

			Model.DischargeStatus = DischargeStatus.Discharged();

			string date = mtbDischargeDate.Text;
			string time = "00:00";
			if( mtbDischargeTime.UnMaskedText != String.Empty )
			{
				time = mtbDischargeTime.Text;
			}
			Model.DischargeDate = Convert.ToDateTime( date + " " + time );

            SaveAccount();

			lblNextAction.Visible = true;
			lblNextActionLine.Visible = true;

			panelButtons.Hide();

			panelActions.Show();
            gbScanDocuments.Visible = true;

			lblInstructionalMessage.Text = "Edit Inpatient Discharge Information submitted for processing.";
			userContextView1.Description = "Edit Inpatient Discharge Information - Submitted";

			RemoveMaskedTextBorder();

			btnCloseActivity.Focus();

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
		}

		private bool IsMedicalAbstractComplete()
		{
		    if( Model.AbstractExists )
			{
				string newLine = "\n";
				
				if( lblInstructionalMessage.Text == String.Empty )
				{
					newLine = string.Empty;
				}
				lblInstructionalMessage.Text = lblInstructionalMessage.Text 
					+ newLine + UIErrorMessages.EDIT_DISCHARGE_MEDICAL_ABSTRACT_COMPLETE_MSG;
				cmbDischargeDisposition.Enabled = false;
				btnOk.Enabled = false;
				return true;
			}
		    
            return false;
		}

	    private bool validateDateAndTime(string field)
        {
            if( IsDischargeDateValid() )
            {
                if( !IsFutureDischargeDate(field) )
                {
                    if( !IsDischargeDateBeforeAdmitDate(field) )
                    {
                        if( mtbDischargeDate.UnMaskedText.Length != 0 )
                        {
                            if( DateValidator.IsValidTime( mtbDischargeTime ) )
                            {
                                return true;
                            }
                            
                            return false;
                        }
                    }
                }
            }

            return false;
        }
		
        private bool IsDischargeDateBeforeAdmitDate(string field)
        {
            SetDischargeDateNormalBgColor();
            SetDischargeTimeNormalBgColor();

            int iAdmitMonth = Model.AdmitDate.Month;
            int iAdmitDay = Model.AdmitDate.Day;
            int iAdmitYear = Model.AdmitDate.Year;

            int iAdmitHour = Model.AdmitDate.Hour;
            int iAdmitMinute = Model.AdmitDate.Minute;

            DateTime admitDate      = new DateTime( iAdmitYear, iAdmitMonth, iAdmitDay );
            DateTime admitDateTime  = new DateTime( iAdmitYear, iAdmitMonth, iAdmitDay, iAdmitHour, iAdmitMinute, 0 );

            string dischargeMonth = string.Empty;
            string dischargeDay   = string.Empty;
            string dischargeYear  = string.Empty;

            int iDischargeMonth;
            int iDischargeDay;
            int iDischargeYear;

            int iDischargeHour;
            int iDischargeMinute;

            if( mtbDischargeDate.Text.Length == 10 )
            {
                dischargeMonth  = mtbDischargeDate.Text.Substring( 0, 2 );
                dischargeDay    = mtbDischargeDate.Text.Substring( 3, 2 );
                dischargeYear   = mtbDischargeDate.Text.Substring( 6, 4 );

                iDischargeMonth  = Convert.ToInt32( dischargeMonth );
                iDischargeDay    = Convert.ToInt32( dischargeDay );
                iDischargeYear   = Convert.ToInt32( dischargeYear );
            }
            else
            {
                iDischargeMonth = 0;
                iDischargeDay   = 0;
                iDischargeYear  = 0;
            }
           
            string dischargeHour     = string.Empty;
            string dischargeMinute   = string.Empty;

            if( mtbDischargeTime.Text.Length == 5 )
            {
                dischargeHour   = mtbDischargeTime.Text.Substring( 0, 2 );
                dischargeMinute = mtbDischargeTime.Text.Substring( 3, 2 );

                iDischargeHour   = Convert.ToInt32( dischargeHour );
                iDischargeMinute = Convert.ToInt32( dischargeMinute );
            }
            else
            {
                iDischargeHour      = 0;
                iDischargeMinute    = 0;
            }

            DateTime dischargeDate      = new DateTime( iDischargeYear, iDischargeMonth, iDischargeDay );
            DateTime dischargeDateTime  = new DateTime( iDischargeYear, iDischargeMonth, iDischargeDay, iDischargeHour, iDischargeMinute, 0 );

            try
            {  				
                if(
                    ( iDischargeHour == 0 && iDischargeMinute  == 0 && dischargeDate.Date < admitDate.Date )
                    ||
                    ( ( iDischargeHour != 0 || iDischargeMinute != 0 ) && dischargeDate < admitDate )
                  )
                {                
                    if( field == "DATE" )
                    {
                        SetDischargeDateErrBgColor();
                        if( mtbDischargeDate.Focused )
                        {
                            mtbDischargeTime.Focus();
                        }
                    }
                    else
                    {					
                        SetDischargeTimeErrBgColor();
                        mtbDischargeTime.Focus();
                    }

                    MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                       
                    return true;
                }

                if( dischargeDateTime < admitDateTime )
                {
                    if( field == "DATE" )
                    {
                        SetDischargeDateErrBgColor();
                        if( mtbDischargeDate.Focused )
                        {
                            mtbDischargeTime.Focus();
                        }
                        
                    }
                    else
                    {					
                        SetDischargeTimeErrBgColor();
                        mtbDischargeTime.Focus();
                    }

                    MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    
                    return true;
                }
            }
            catch( ArgumentOutOfRangeException )
            {
                if( mtbDischargeDate.UnMaskedText.Length != 0 )
                {
                    SetDischargeDateErrBgColor();
                    MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_NOT_VALID_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
					Cursor = Cursors.Default;
                    return true;
                }
            }
			
            return false;
        }

        private bool IsFutureDischargeDate(string field)
        {
            SetDischargeDateNormalBgColor();
            SetDischargeTimeNormalBgColor();

            string month = string.Empty;
            string day   = string.Empty;
            string year  = string.Empty;

            if( mtbDischargeDate.Text.Length == 10 )
            {
                month  = mtbDischargeDate.Text.Substring( 0, 2 );
                day    = mtbDischargeDate.Text.Substring( 3, 2 );
                year   = mtbDischargeDate.Text.Substring( 6, 4 );

                verifyMonth  = Convert.ToInt32( month );
                verifyDay    = Convert.ToInt32( day );
                verifyYear   = Convert.ToInt32( year );
            }
            else
            {
                verifyMonth = 0;
                verifyDay   = 0;
                verifyYear  = 0;
            }
           
            string hour     = string.Empty;
            string minute   = string.Empty;

            if( mtbDischargeTime.Text.Length == 5 )
            {
                hour   = mtbDischargeTime.Text.Substring( 0, 2 );
                minute = mtbDischargeTime.Text.Substring( 3, 2 );

                verifyHour   = Convert.ToInt32( hour );
                verifyMinute = Convert.ToInt32( minute );
            }
            else
            {
                verifyHour      = 0;
                verifyMinute    = 0;
            }

            try
            {   // Check the date entered is not in the future
                DateTime theDate = new DateTime( verifyYear, verifyMonth, verifyDay, verifyHour, verifyMinute, 0 );
                DateTime currentFacilityTime = GetCurrentFacilityDateTime();
                
                if(     
                    ( verifyHour == 0 && verifyMinute == 0 && theDate.Date > currentFacilityTime.Date )
                    ||
                    ( ( verifyHour != 0 || verifyMinute != 0 ) && theDate > currentFacilityTime )
                  )

                {
                    if( field == "DATE" )
                    {
                        SetDischargeDateErrBgColor();
                        if( mtbDischargeDate.Focused )
                        {
                            mtbDischargeTime.Focus();
                        }
                    }
                    else
                    {					
                        SetDischargeTimeErrBgColor();
                        if( !dtpDischargeDate.Focused )
                        {
                            mtbDischargeTime.Focus();
                        }
                    }

                    MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_IN_FUTURE_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
					
                    return true;
                }
				
            }
            catch( ArgumentOutOfRangeException )
            {
                if( mtbDischargeDate.UnMaskedText.Length != 0 )
                {
                    SetDischargeDateErrBgColor();
                    MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_NOT_VALID_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
					Cursor = Cursors.Default;
                }
                return true;
            }
			
            return false;
        }

        private bool IsDischargeDateValid()
        {
            if( mtbDischargeDate.UnMaskedText.Length == 0 )
            {
                return true;
            }

            if( mtbDischargeDate.Text.Length != 10)
            {
                SetDischargeDateErrBgColor();
                MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_INCOMPLETE_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbDischargeDate.Focus();
                return false;
            }

            string month = mtbDischargeDate.Text.Substring( 0, 2 );
            string day   = mtbDischargeDate.Text.Substring( 3, 2 );
            string year  = mtbDischargeDate.Text.Substring( 6, 4 );
  
            verifyMonth = Convert.ToInt32( month );
            verifyDay   = Convert.ToInt32( day );
            verifyYear  = Convert.ToInt32( year );

            try
            {   // Check the date entered is not in the future
                DateTime theDate = new DateTime( verifyYear, verifyMonth, verifyDay );

                if( DateValidator.IsValidDate( theDate ) == false )
                {
                    SetDischargeDateErrBgColor();
                    MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_NOT_VALID_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbDischargeDate.Focus();
                    return false;
                }
            }
            catch( ArgumentOutOfRangeException )
            {
                if( mtbDischargeDate.UnMaskedText.Length != 0 )
                {
                    SetDischargeDateErrBgColor();
                    MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_NOT_VALID_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbDischargeDate.Focus();
					Cursor = Cursors.Default;
                }
                return false;
            }

            SetDischargeDateNormalBgColor();
            return true;
        }

        private static DateTime GetCurrentFacilityDateTime()
        {
            ITimeBroker timeBroker  = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset, 
                                      User.GetCurrent().Facility.DSTOffset );
        }

		private void SetDischargeDateErrBgColor()
		{
            UIColors.SetErrorBgColor( mtbDischargeDate );
		}

		private void SetDischargeDateNormalBgColor()
		{
			UIColors.SetNormalBgColor( mtbDischargeDate );
			Refresh();
		}

		private void SetDischargeTimeErrBgColor()
		{
            UIColors.SetErrorBgColor( mtbDischargeTime );
		}

		private void SetDischargeTimeNormalBgColor()
		{
			UIColors.SetNormalBgColor( mtbDischargeTime );
			Refresh();
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblNextAction = new System.Windows.Forms.Label();
            this.lblNextActionLine = new System.Windows.Forms.Label();
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
            this.userContextView1.Description = "Edit Inpatient Discharge Information";
            this.userContextView1.TabStop = false;
            // 
            // patientContextView1
            // 
            this.patientContextView1.TabStop = false;
            // 
            // mtbDischargeDate
            // 
            this.mtbDischargeDate.MaxLength = 10;
            this.mtbDischargeDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDischargeDate_Validating );
            // 
            // mtbDischargeTime
            // 
            this.mtbDischargeTime.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDischargeTime_Validating );
            // 
            // btnCancel
            // 
            this.btnCancel.TabIndex = 9;
            // 
            // btnOk
            // 
            this.btnOk.TabIndex = 8;
            this.btnOk.Click += new System.EventHandler( this.btnOk_Click );
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.TabIndex = 10;
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.TabIndex = 11;
            // 
            // btnEditAccount
            // 
            this.btnEditAccount.Size = new System.Drawing.Size( 130, 23 );
            this.btnEditAccount.TabIndex = 12;
            // 
            // cmbDischargeDisposition
            // 
            this.cmbDischargeDisposition.SelectedIndexChanged += new System.EventHandler( this.cmbDischargeDisposition_SelectedIndexChanged );
            // 
            // dtpDischargeDate
            // 
            this.dtpDischargeDate.TabStop = false;
            this.dtpDischargeDate.CloseUp += new System.EventHandler( this.dtpDischargeDate_CloseUp );
            // 
            // panelActions
            // 
            this.panelActions.Location = new System.Drawing.Point( 5, 100 );
            this.panelActions.Size = new System.Drawing.Size( 468, 29 );
            // 
            // panelButtons
            // 
            this.panelButtons.TabIndex = 0;
            // 
            // panelMessages
            // 
            this.panelMessages.Controls.Add( this.lblNextAction );
            this.panelMessages.Controls.Add( this.lblNextActionLine );
            this.panelMessages.TabIndex = 0;
            this.panelMessages.Controls.SetChildIndex( this.lblMessages, 0 );
            this.panelMessages.Controls.SetChildIndex( this.lblOutstandingActionItemsMsg, 0 );
            this.panelMessages.Controls.SetChildIndex( this.panelActions, 0 );
            this.panelMessages.Controls.SetChildIndex( this.lblNextActionLine, 0 );
            this.panelMessages.Controls.SetChildIndex( this.lblNextAction, 0 );
            this.panelMessages.Controls.SetChildIndex( this.label55, 0 );
            this.panelMessages.Controls.SetChildIndex( this.label2, 0 );
            // 
            // label2
            // 
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            // 
            // lblAdmitTime
            // 
            this.lblAdmitTime.Location = new System.Drawing.Point( 245, 141 );
            // 
            // lblOutstandingActionItemsMsg
            // 
            this.lblOutstandingActionItemsMsg.Size = new System.Drawing.Size( 912, 35 );
            // 
            // btnPrintFaceSheet
            // 
            this.btnPrintFaceSheet.Location = new System.Drawing.Point( 334, 3 );
            this.btnPrintFaceSheet.TabIndex = 13;
            // 
            // lblNextAction
            // 
            this.lblNextAction.Location = new System.Drawing.Point( 10, 40 );
            this.lblNextAction.Name = "lblNextAction";
            this.lblNextAction.Size = new System.Drawing.Size( 62, 13 );
            this.lblNextAction.TabIndex = 2;
            this.lblNextAction.Text = "Next Action";
            this.lblNextAction.Visible = false;
            // 
            // lblNextActionLine
            // 
            this.lblNextActionLine.Location = new System.Drawing.Point( 65, 41 );
            this.lblNextActionLine.Name = "lblNextActionLine";
            this.lblNextActionLine.Size = new System.Drawing.Size( 909, 16 );
            this.lblNextActionLine.TabIndex = 1;
            this.lblNextActionLine.Text = "_________________________________________________________________________________" +
                "_______________________________________________________________________________";
            this.lblNextActionLine.Visible = false;
            // 
            // EditDischargeView
            // 
            this.AcceptButton = this.btnOk;
            this.Name = "EditDischargeView";
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
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public EditDischargeView()
		{
            CurrentActivity = new EditDischargeDataActivity();

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            EnableThemesOn( this );

			lblCurOccupant.Visible = false;
			lblCurrentOccupant.Visible = false;
			label2.Visible = false;
			label55.Visible = false;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
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
		private int			verifyMonth;
		private int			verifyDay;
		private int			verifyYear;
		private int			verifyHour;
		private int			verifyMinute;
        private Activity    i_CurrentActivity;
		#endregion

		#region Constants
        #endregion

    }
}
