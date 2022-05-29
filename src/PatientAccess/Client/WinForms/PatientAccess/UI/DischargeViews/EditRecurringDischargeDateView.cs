using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Extensions.SecurityService.Domain;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using User = Extensions.SecurityService.Domain.User;

namespace PatientAccess.UI.DischargeViews
{
	/// <summary>
	/// Summary description for EditRecurringDischargeDate.
	/// </summary>
	[Serializable]
	public class EditRecurringDischargeDateView : DischargeBaseView
	{
		#region Event Handlers
        private void mtbRevisedDate_Validating(object sender, CancelEventArgs e)
        {
			if( btnCancel.Focused )
			{
				mtbRevisedDate.Validating -= mtbRevisedDate_Validating;
				return;
			}

            if( dtpRevisedDate.Focused == false )
            {
                if( IsRevisedDateValidOnLeave() )
                {
                    UIColors.SetNormalBgColor( mtbRevisedDate );
                }
            }        
        }

		private void dtpRevisedDate_ValueChanged(object sender, EventArgs e)
		{
			if( dtpRevisedDate.Checked )
			{
				DateTime dt = dtpRevisedDate.Value;
				mtbRevisedDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
			}
			else
			{
				mtbRevisedDate.Text = String.Empty;
			}
		}

		private new void btnOk_Click(object sender, EventArgs e)
		{
			if( IsRevisedDateValidOnLeave() )
			{
				if( IsRevisedDateValidOnOk() )
				{
                    Cursor = Cursors.WaitCursor;
					UpdateModel();
					EditRecurringDischargeDate();
                    Cursor = Cursors.Default;
				}
			}
            else
            {
                btnOk.Enabled = true;
                return;
            }
        }
		#endregion

		#region Methods
		public override void UpdateView()
		{
			if( Model != null )
			{
				GetUserRole();
				DisplayPatientContext();

				//populate controls.
				lblPatientName.Text = Model.Patient.FormattedName;
				lblAccount.Text = Model.AccountNumber.ToString();
				lblPatientType.Text = Model.KindOfVisit.DisplayString;

				lblAdmitDate.Text = CommonFormatting.LongDateFormat( Model.AdmitDate );
				lblAdmitTime.Text = CommonFormatting.DisplayedTimeFormat( Model.AdmitDate );

				if( Model.DischargeDate != DateTime.MinValue )
				{
					lblScheduledDischargeDateVal.Text = CommonFormatting.LongDateFormat( Model.DischargeDate );
					lblScheduledTimeVal.Text = CommonFormatting.DisplayedTimeFormat( Model.DischargeDate );
				}
				
				PopulateDefaultRevisedDateTime();

				/*
				 * Instructional Messages
				 */ 
				if( !IsPatientRecurringOutpatient() )
				{
					lblInstructionalMessage.Text = UIErrorMessages.EDIT_RECURRING_DISCHARGE_PATIENT_NOT_RECURRING_MSG;
					DisableRevisedDate();
					return;
				}
			    
                if( IsMedicalAbstractComplete() )
			    {
			        DisableRevisedDate();
			        return;
			    }
			    
                if( IsRecordLocked() )
			    {
			        DisableRevisedDate();
			        return;
			    }

			    /*
				 *Prompting Messages 
				 */
				CheckForValuables();
				CheckForRemainingActionItems();
				CheckUserTypeForReregister();

				//Enable the ok button.
				btnOk.Enabled = true;
				mtbRevisedDate.Enabled = true;
			}
		}

		public override void UpdateModel()
		{
			Model.DischargeDate = i_RevisedDate;
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

        public Activity CurrentActivity
        {
            get { return i_CurrentActivity ?? (i_CurrentActivity = new EditRecurringDischargeActivity()); }
            set
            {
                i_CurrentActivity = value;
            }
        }

	    #endregion

		#region Private Methods
		private void GetUserRole()
		{
			User secFrameworkUser = Domain.User.GetCurrent().SecurityUser;

			foreach( Role role in secFrameworkUser.Roles() )
			{
				userRole = role.Name;
				break;
			}
		}

		private void DisableRevisedDate()
		{
			mtbRevisedDate.Enabled = false;
			dtpRevisedDate.Enabled = false;
		}

		private void EditRecurringDischargeDate()
		{
            if (!IsPBARAvailable())
            {
                lblInstructionalMessage.Text = UIErrorMessages.PBAR_UNAVAILABLE_MSG;
                btnOk.Enabled = true;
                return;
            }
            
            CheckForValuables();
			CheckForRemainingActionItems();
			CheckUserTypeForReregister();

			panelDischarge.Visible = true;
			lblDischargeVal.Text = mtbRevisedDate.Text;
			lblTimeVal.Text = lblRevisedTimeVal.Text;
			panelRevised.Visible = false;
			lblNextAction.Visible = true;
			lblNextActionLine.Visible = true;
			panelButtons.Visible = false;
			panelActions.Visible = true;
            gbScanDocuments.Visible = true;

            SaveAccount();

			userContextView1.Description = "Edit Recurring Outpatient Discharge Date - Submitted";
			lblInstructionalMessage.Text = "Edit Recurring Outpatient Discharge Date submitted for processing.";

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
		}

		private void CheckUserTypeForReregister()
		{
			//Determine what the user-type is and display proper message.
			string msg = string.Empty;

			if( userRole == "ClinicalUser" )
			{
				msg = UIErrorMessages.EDIT_RECURRING_DISCHARGE_DATE_CLINICAL_USER_MSG;
			}
			else if( userRole == "FinancialUser" ||
					 userRole == "RegistrationUser"  || 
					 userRole == "RegistrationAdministrator" )
			{
				msg = UIErrorMessages.EDIT_RECURRING_DISCHARGE_DATE_REG_FIN_USER_MSG;
			}

			if( lblOutstandingActionItemsMsg.Text.Length > 0 )
			{
				lblOutstandingActionItemsMsg.Text = lblOutstandingActionItemsMsg.Text + "\n";
			}
			lblOutstandingActionItemsMsg.Text += msg;
		}

		private bool IsRecordLocked()
		{
			if( Model.AccountLock != null && !Model.AccountLock.IsLockAcquiredByCurrentUser())               
			{
				lblInstructionalMessage.Text = UIErrorMessages.CANCEL_DISCHARGE_ACCOUNT_LOCKED_MSG;
				btnOk.Enabled = false;
				return true;
			}
			return false;
		}

		private bool IsPatientRecurringOutpatient()
		{
			return Model.KindOfVisit.Code == VisitType.RECURRING_PATIENT;
		}

		private bool IsMedicalAbstractComplete()
		{
			if( Model.AbstractExists )
			{
				lblInstructionalMessage.Text = UIErrorMessages.EDIT_RECURRING_DISCHARGE_MEDICAL_ABSTRACT_COMPLETE_MSG;
				btnOk.Enabled = false;
				return true;
			}
			return false;
		}

		private new void CheckForRemainingActionItems()
		{
			string msg = String.Empty;

            // OTD# 37044 fix - Outstanding actions items message is not displayed
			// hasRemainingActionItems = ( RuleEngine.GetWorklistActionItems( Model ).Count > 0 );
			hasRemainingActionItems = ( ( Model.GetAllRemainingActions() ).Count > 0 );

			if( hasRemainingActionItems )
			{	//Check user type to determine what action items message.
				if( userRole == "ClinicalUser" )
				{
					msg = UIErrorMessages.CLINICAL_USER_OUTSTANDING_ACTION_ITEMS_MSG + "\n";
				}
				else if( userRole == "FinancialUser" ||
					userRole == "RegistrationUser" || 
					userRole == "RegistrationAdministrator" )
				{
					msg = UIErrorMessages.OUTSTANDING_ACTION_ITEMS_MSG + "\n";
				}
			}

			lblOutstandingActionItemsMsg.Text  = msg;
		}

		private void PopulateDefaultRevisedDateTime()
		{
			IFacilityBroker fBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
			DateTime localDateTime = fBroker.GetAppServerDateTime();
            
			Calendar myCal = CultureInfo.InvariantCulture.Calendar;

			localDateTime = myCal.AddHours( localDateTime, Model.Facility.GMTOffset );
			localDateTime = Convert.ToDateTime( localDateTime.Date.ToString() );
			localDateTime = myCal.AddHours( localDateTime, 23 );
			localDateTime = myCal.AddMinutes( localDateTime, 59 );
			i_CurrentDateTime = localDateTime;

			if( i_CurrentDateTime == DateTime.MinValue )
			{
				i_CurrentDateTime = DateTime.Now.Date;
			}

			mtbRevisedDate.UnMaskedText = CommonFormatting.MaskedDateFormat( localDateTime );
			lblRevisedTimeVal.Text = CommonFormatting.DisplayedTimeFormat( localDateTime );
		}
  
		private bool IsRevisedDateValidOnLeave()
		{
			if( mtbRevisedDate.Text.Length != 10 )
			{
				SetRevisedDischargeDateErrBgColor();
				MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_INCOMPLETE_MSG, "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1 );
				mtbRevisedDate.Focus();
				return false;
			}
			string month = mtbRevisedDate.Text.Substring( 0, 2 );
			string day   = mtbRevisedDate.Text.Substring( 3, 2 );
			string year  = mtbRevisedDate.Text.Substring( 6, 4 );
  
			verifyMonth = Convert.ToInt32( month );
			verifyDay   = Convert.ToInt32( day );
			verifyYear  = Convert.ToInt32( year );

			try
			{   // Check the date entered is not in the future
				DateTime theDate = new DateTime( verifyYear, verifyMonth, verifyDay );
				
				Calendar myCal = CultureInfo.InvariantCulture.Calendar;

				theDate = myCal.AddHours( theDate, 23 );
				theDate = myCal.AddMinutes( theDate, 59 );

				i_RevisedDate = theDate;

				if( DateValidator.IsValidDate( theDate ) == false )
				{
					SetRevisedDischargeDateErrBgColor();
					MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_NOT_VALID_MSG, "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					mtbRevisedDate.Focus();
					return false;
				}

				if( theDate < Model.AdmitDate )
				{
					SetRevisedDischargeDateErrBgColor();
					MessageBox.Show( UIErrorMessages.REVISED_DISCHARGE_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					mtbRevisedDate.Focus();
					return false;
				}

				if( theDate.Date > GetCurrentFacilityDate().Date )
				{
					SetRevisedDischargeDateErrBgColor();
					MessageBox.Show( UIErrorMessages.EDIT_DISCHARGE_FUTUREDATE_MSG, "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					mtbRevisedDate.Focus();
					return false;
				}
			}
			catch
			{
				SetRevisedDischargeDateErrBgColor();
				MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_NOT_VALID_MSG, "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1 );
				mtbRevisedDate.Focus();
				return false;
			}

            SetRevisedDischargeDateNormalBgColor();
            return true;
        }

		private static DateTime GetCurrentFacilityDate()
		{
			ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt( Domain.User.GetCurrent().Facility.GMTOffset, 
                                      Domain.User.GetCurrent().Facility.DSTOffset );
		}

		private bool IsRevisedDateValidOnOk()
		{
			if( mtbRevisedDate.Text.Length != 10 )
			{
				SetRevisedDischargeDateErrBgColor();
				MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_INCOMPLETE_MSG, "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1 );
				return false;
			}

			string month = mtbRevisedDate.Text.Substring( 0, 2 );
			string day   = mtbRevisedDate.Text.Substring( 3, 2 );
			string year  = mtbRevisedDate.Text.Substring( 6, 4 );
  
			verifyMonth = Convert.ToInt32( month );
			verifyDay   = Convert.ToInt32( day );
			verifyYear  = Convert.ToInt32( year );

			try
			{   // Check the date entered is not in the future
				DateTime theDate = new DateTime( verifyYear, verifyMonth, verifyDay );

				Calendar myCal = CultureInfo.InvariantCulture.Calendar;

				theDate = myCal.AddHours( theDate, 23 );
				theDate = myCal.AddMinutes( theDate, 59 );

				i_RevisedDate = theDate;

				if( DateValidator.IsValidDate( theDate ) == false )
				{
					SetRevisedDischargeDateErrBgColor();
					MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_NOT_VALID_MSG, "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					return false;
				}
			    
                if( theDate.Date > DateValidator.EndOfMonth( i_CurrentDateTime ) )
			    {
			        SetRevisedDischargeDateErrBgColor();
			        MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_IN_FUTURE_MSG, "Error",
			                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
			                         MessageBoxDefaultButton.Button1 );
			        return false;
			    }
			    
                if( theDate < Model.AdmitDate )
			    {
			        SetRevisedDischargeDateErrBgColor();
			        MessageBox.Show( UIErrorMessages.REVISED_DISCHARGE_DATE_BEFORE_ADMIT_DATE_MSG, "Error",
			                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
			                         MessageBoxDefaultButton.Button1 );
			        return false;
			    }
			    
                if( DischargeService.IsDischargeDateBeforeChargeDates( mtbRevisedDate, mtbRevisedDate, Model.LastChargeDate ) )
			    {
			        SetRevisedDischargeDateErrBgColor();
			        MessageBox.Show( UIErrorMessages.REVISED_DISCHARGE_DATE_BEFORE_LAST_SERVICE_DATE_MSG, "Error",
			                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
			                         MessageBoxDefaultButton.Button1 );
			        return false;
			    }
			}
			catch( ArgumentOutOfRangeException )
			{
				SetRevisedDischargeDateErrBgColor();
				MessageBox.Show( UIErrorMessages.DISCHARGE_DATE_NOT_VALID_MSG, "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1 );
				return false;
			}

			SetRevisedDischargeDateNormalBgColor();
			return true;
		}

		private void SetRevisedDischargeDateErrBgColor()
		{
            UIColors.SetErrorBgColor( mtbRevisedDate );
		}

		private void SetRevisedDischargeDateNormalBgColor()
		{
			UIColors.SetNormalBgColor( mtbRevisedDate );
			Refresh();
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.panelRevised = new System.Windows.Forms.Panel();
            this.lblRevisedTimeVal = new System.Windows.Forms.Label();
            this.lblRevisedTime = new System.Windows.Forms.Label();
            this.dtpRevisedDate = new System.Windows.Forms.DateTimePicker();
            this.mtbRevisedDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblScheduledTimeVal = new System.Windows.Forms.Label();
            this.lblScheduledTime = new System.Windows.Forms.Label();
            this.lblScheduledDischargeDateVal = new System.Windows.Forms.Label();
            this.lblRevisedDate = new System.Windows.Forms.Label();
            this.lblScheduledDate = new System.Windows.Forms.Label();
            this.lblDischargeDate = new System.Windows.Forms.Label();
            this.lblTimeVal = new System.Windows.Forms.Label();
            this.panelDischarge = new System.Windows.Forms.Panel();
            this.lblDichargeTime = new System.Windows.Forms.Label();
            this.lblDischargeVal = new System.Windows.Forms.Label();
            this.lblNextAction = new System.Windows.Forms.Label();
            this.lblNextActionLine = new System.Windows.Forms.Label();
            this.panelActions.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelMessages.SuspendLayout();
            this.gbScanDocuments.SuspendLayout();
            this.panelRevised.SuspendLayout();
            this.panelDischarge.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView1
            // 
            this.userContextView1.Description = "Edit Recurring Outpatient Discharge Date";
            this.userContextView1.TabStop = false;
            // 
            // patientContextView1
            // 
            this.patientContextView1.TabStop = false;
            // 
            // mtbDischargeDate
            // 
            this.mtbDischargeDate.Visible = false;
            // 
            // mtbDischargeTime
            // 
            this.mtbDischargeTime.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.Click += new System.EventHandler( this.btnOk_Click );
            // 
            // cmbDischargeDisposition
            // 
            this.cmbDischargeDisposition.Visible = false;
            // 
            // dtpDischargeDate
            // 
            this.dtpDischargeDate.Visible = false;
            // 
            // panelActions
            // 
            this.panelActions.Location = new System.Drawing.Point( 6, 134 );
            // 
            // panel2
            // 
            this.panel2.Controls.Add( this.panelRevised );
            this.panel2.Controls.Add( this.panelDischarge );
            this.panel2.Controls.SetChildIndex( this.gbScanDocuments, 0 );
            this.panel2.Controls.SetChildIndex( this.panelDischarge, 0 );
            this.panel2.Controls.SetChildIndex( this.label1, 0 );
            this.panel2.Controls.SetChildIndex( this.label3, 0 );
            this.panel2.Controls.SetChildIndex( this.label5, 0 );
            this.panel2.Controls.SetChildIndex( this.label6, 0 );
            this.panel2.Controls.SetChildIndex( this.label7, 0 );
            this.panel2.Controls.SetChildIndex( this.label8, 0 );
            this.panel2.Controls.SetChildIndex( this.label9, 0 );
            this.panel2.Controls.SetChildIndex( this.label12, 0 );
            this.panel2.Controls.SetChildIndex( this.label13, 0 );
            this.panel2.Controls.SetChildIndex( this.lblDischargeDateVal, 0 );
            this.panel2.Controls.SetChildIndex( this.lblDischargeTimeVal, 0 );
            this.panel2.Controls.SetChildIndex( this.lblDischargeDispositionVal, 0 );
            this.panel2.Controls.SetChildIndex( this.lblInstructionalMessage, 0 );
            this.panel2.Controls.SetChildIndex( this.lblPatientName, 0 );
            this.panel2.Controls.SetChildIndex( this.lblAccount, 0 );
            this.panel2.Controls.SetChildIndex( this.lblPatientType, 0 );
            this.panel2.Controls.SetChildIndex( this.lblAdmitDate, 0 );
            this.panel2.Controls.SetChildIndex( this.lblAdmitTime, 0 );
            this.panel2.Controls.SetChildIndex( this.lblDischargeLocation, 0 );
            this.panel2.Controls.SetChildIndex( this.mtbDischargeTime, 0 );
            this.panel2.Controls.SetChildIndex( this.dtpDischargeDate, 0 );
            this.panel2.Controls.SetChildIndex( this.mtbDischargeDate, 0 );
            this.panel2.Controls.SetChildIndex( this.cmbDischargeDisposition, 0 );
            this.panel2.Controls.SetChildIndex( this.lblCurOccupant, 0 );
            this.panel2.Controls.SetChildIndex( this.lblCurrentOccupant, 0 );
            this.panel2.Controls.SetChildIndex( this.panelMessages, 0 );
            this.panel2.Controls.SetChildIndex( this.panelRevised, 0 );
            // 
            // panelButtons
            // 
            this.panelButtons.Location = new System.Drawing.Point( 844, 593 );
            // 
            // panelMessages
            // 
            this.panelMessages.Controls.Add( this.lblNextAction );
            this.panelMessages.Controls.Add( this.lblNextActionLine );
            this.panelMessages.Location = new System.Drawing.Point( 7, 345 );
            this.panelMessages.Size = new System.Drawing.Size( 987, 169 );
            this.panelMessages.TabIndex = 0;
            this.panelMessages.Controls.SetChildIndex( this.lblNextActionLine, 0 );
            this.panelMessages.Controls.SetChildIndex( this.label55, 0 );
            this.panelMessages.Controls.SetChildIndex( this.label2, 0 );
            this.panelMessages.Controls.SetChildIndex( this.panelActions, 0 );
            this.panelMessages.Controls.SetChildIndex( this.lblOutstandingActionItemsMsg, 0 );
            this.panelMessages.Controls.SetChildIndex( this.lblMessages, 0 );
            this.panelMessages.Controls.SetChildIndex( this.lblNextAction, 0 );
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point( 57, 7 );
            // 
            // label55
            // 
            this.label55.Location = new System.Drawing.Point( 6, 8 );
            // 
            // label6
            // 
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.Visible = false;
            // 
            // label9
            // 
            this.label9.Visible = false;
            // 
            // label13
            // 
            this.label13.Visible = false;
            // 
            // lblDischargeLocation
            // 
            this.lblDischargeLocation.Visible = false;
            // 
            // lblOutstandingActionItemsMsg
            // 
            this.lblOutstandingActionItemsMsg.Location = new System.Drawing.Point( 12, 49 );
            this.lblOutstandingActionItemsMsg.Size = new System.Drawing.Size( 907, 54 );
            // 
            // lblMessages
            // 
            this.lblMessages.Location = new System.Drawing.Point( 12, 29 );
            this.lblMessages.Size = new System.Drawing.Size( 907, 12 );
            // 
            // lblCurOccupant
            // 
            this.lblCurOccupant.Visible = false;
            // 
            // lblCurrentOccupant
            // 
            this.lblCurrentOccupant.Visible = false;
            // 
            // gbScanDocuments
            // 
            this.gbScanDocuments.Location = new System.Drawing.Point( 14, 258 );
            // 
            // panelRevised
            // 
            this.panelRevised.Controls.Add( this.lblRevisedTimeVal );
            this.panelRevised.Controls.Add( this.lblRevisedTime );
            this.panelRevised.Controls.Add( this.dtpRevisedDate );
            this.panelRevised.Controls.Add( this.mtbRevisedDate );
            this.panelRevised.Controls.Add( this.lblScheduledTimeVal );
            this.panelRevised.Controls.Add( this.lblScheduledTime );
            this.panelRevised.Controls.Add( this.lblScheduledDischargeDateVal );
            this.panelRevised.Controls.Add( this.lblRevisedDate );
            this.panelRevised.Controls.Add( this.lblScheduledDate );
            this.panelRevised.Location = new System.Drawing.Point( 9, 180 );
            this.panelRevised.Name = "panelRevised";
            this.panelRevised.Size = new System.Drawing.Size( 363, 50 );
            this.panelRevised.TabIndex = 0;
            // 
            // lblRevisedTimeVal
            // 
            this.lblRevisedTimeVal.Location = new System.Drawing.Point( 301, 30 );
            this.lblRevisedTimeVal.Name = "lblRevisedTimeVal";
            this.lblRevisedTimeVal.Size = new System.Drawing.Size( 37, 13 );
            this.lblRevisedTimeVal.TabIndex = 0;
            // 
            // lblRevisedTime
            // 
            this.lblRevisedTime.Location = new System.Drawing.Point( 261, 30 );
            this.lblRevisedTime.Name = "lblRevisedTime";
            this.lblRevisedTime.Size = new System.Drawing.Size( 35, 15 );
            this.lblRevisedTime.TabIndex = 0;
            this.lblRevisedTime.Text = "Time:";
            // 
            // dtpRevisedDate
            // 
            this.dtpRevisedDate.Location = new System.Drawing.Point( 209, 26 );
            this.dtpRevisedDate.Name = "dtpRevisedDate";
            this.dtpRevisedDate.Size = new System.Drawing.Size( 23, 20 );
            this.dtpRevisedDate.TabIndex = 0;
            this.dtpRevisedDate.TabStop = false;
            this.dtpRevisedDate.ValueChanged += new System.EventHandler( this.dtpRevisedDate_ValueChanged );
            // 
            // mtbRevisedDate
            // 
            this.mtbRevisedDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRevisedDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbRevisedDate.Location = new System.Drawing.Point( 143, 26 );
            this.mtbRevisedDate.Mask = "  /  /";
            this.mtbRevisedDate.MaxLength = 10;
            this.mtbRevisedDate.Name = "mtbRevisedDate";
            this.mtbRevisedDate.Size = new System.Drawing.Size( 66, 20 );
            this.mtbRevisedDate.TabIndex = 1;
            this.mtbRevisedDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbRevisedDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRevisedDate_Validating );
            // 
            // lblScheduledTimeVal
            // 
            this.lblScheduledTimeVal.Location = new System.Drawing.Point( 301, 6 );
            this.lblScheduledTimeVal.Name = "lblScheduledTimeVal";
            this.lblScheduledTimeVal.Size = new System.Drawing.Size( 37, 13 );
            this.lblScheduledTimeVal.TabIndex = 0;
            // 
            // lblScheduledTime
            // 
            this.lblScheduledTime.Location = new System.Drawing.Point( 261, 5 );
            this.lblScheduledTime.Name = "lblScheduledTime";
            this.lblScheduledTime.Size = new System.Drawing.Size( 35, 15 );
            this.lblScheduledTime.TabIndex = 0;
            this.lblScheduledTime.Text = "Time:";
            // 
            // lblScheduledDischargeDateVal
            // 
            this.lblScheduledDischargeDateVal.Location = new System.Drawing.Point( 145, 6 );
            this.lblScheduledDischargeDateVal.Name = "lblScheduledDischargeDateVal";
            this.lblScheduledDischargeDateVal.Size = new System.Drawing.Size( 70, 13 );
            this.lblScheduledDischargeDateVal.TabIndex = 2;
            // 
            // lblRevisedDate
            // 
            this.lblRevisedDate.Location = new System.Drawing.Point( 2, 30 );
            this.lblRevisedDate.Name = "lblRevisedDate";
            this.lblRevisedDate.Size = new System.Drawing.Size( 126, 14 );
            this.lblRevisedDate.TabIndex = 0;
            this.lblRevisedDate.Text = "Revised discharge date:";
            // 
            // lblScheduledDate
            // 
            this.lblScheduledDate.Location = new System.Drawing.Point( 2, 5 );
            this.lblScheduledDate.Name = "lblScheduledDate";
            this.lblScheduledDate.Size = new System.Drawing.Size( 138, 15 );
            this.lblScheduledDate.TabIndex = 0;
            this.lblScheduledDate.Text = "Scheduled discharge date:";
            // 
            // lblDischargeDate
            // 
            this.lblDischargeDate.Location = new System.Drawing.Point( 1, 5 );
            this.lblDischargeDate.Name = "lblDischargeDate";
            this.lblDischargeDate.Size = new System.Drawing.Size( 83, 15 );
            this.lblDischargeDate.TabIndex = 0;
            this.lblDischargeDate.Text = "Discharge date:";
            // 
            // lblTimeVal
            // 
            this.lblTimeVal.Location = new System.Drawing.Point( 245, 5 );
            this.lblTimeVal.Name = "lblTimeVal";
            this.lblTimeVal.Size = new System.Drawing.Size( 52, 13 );
            this.lblTimeVal.TabIndex = 0;
            // 
            // panelDischarge
            // 
            this.panelDischarge.Controls.Add( this.lblDichargeTime );
            this.panelDischarge.Controls.Add( this.lblDischargeVal );
            this.panelDischarge.Controls.Add( this.lblDischargeDate );
            this.panelDischarge.Controls.Add( this.lblTimeVal );
            this.panelDischarge.Location = new System.Drawing.Point( 10, 180 );
            this.panelDischarge.Name = "panelDischarge";
            this.panelDischarge.Size = new System.Drawing.Size( 331, 30 );
            this.panelDischarge.TabIndex = 0;
            this.panelDischarge.Visible = false;
            // 
            // lblDichargeTime
            // 
            this.lblDichargeTime.Location = new System.Drawing.Point( 199, 5 );
            this.lblDichargeTime.Name = "lblDichargeTime";
            this.lblDichargeTime.Size = new System.Drawing.Size( 35, 15 );
            this.lblDichargeTime.TabIndex = 0;
            this.lblDichargeTime.Text = "Time:";
            // 
            // lblDischargeVal
            // 
            this.lblDischargeVal.Location = new System.Drawing.Point( 89, 5 );
            this.lblDischargeVal.Name = "lblDischargeVal";
            this.lblDischargeVal.Size = new System.Drawing.Size( 70, 13 );
            this.lblDischargeVal.TabIndex = 0;
            // 
            // lblNextAction
            // 
            this.lblNextAction.Location = new System.Drawing.Point( 6, 110 );
            this.lblNextAction.Name = "lblNextAction";
            this.lblNextAction.Size = new System.Drawing.Size( 62, 13 );
            this.lblNextAction.TabIndex = 0;
            this.lblNextAction.Text = "Next Action";
            this.lblNextAction.Visible = false;
            // 
            // lblNextActionLine
            // 
            this.lblNextActionLine.Location = new System.Drawing.Point( 61, 111 );
            this.lblNextActionLine.Name = "lblNextActionLine";
            this.lblNextActionLine.Size = new System.Drawing.Size( 901, 16 );
            this.lblNextActionLine.TabIndex = 0;
            this.lblNextActionLine.Text = "_________________________________________________________________________________" +
                "_______________________________________________________________________________";
            this.lblNextActionLine.Visible = false;
            // 
            // EditRecurringDischargeDateView
            // 
            this.AcceptButton = this.btnOk;
            this.Name = "EditRecurringDischargeDateView";
            this.Size = new System.Drawing.Size( 1019, 623 );
            this.panelActions.ResumeLayout( false );
            this.panel2.ResumeLayout( false );
            this.panel2.PerformLayout();
            this.panelPatientContext.ResumeLayout( false );
            this.panelButtons.ResumeLayout( false );
            this.panelMessages.ResumeLayout( false );
            this.gbScanDocuments.ResumeLayout( false );
            this.panelRevised.ResumeLayout( false );
            this.panelRevised.PerformLayout();
            this.panelDischarge.ResumeLayout( false );
            this.ResumeLayout( false );

		}
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public EditRecurringDischargeDateView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			EnableThemesOn( this );
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
		private Label lblScheduledDate;
		private Label lblRevisedDate;
		private Label lblScheduledDischargeDateVal;
		private MaskedEditTextBox mtbRevisedDate;
		private DateTimePicker dtpRevisedDate;
		private Label lblScheduledTimeVal;
		private Label lblScheduledTime;
		private Label lblRevisedTimeVal;
		private Label lblRevisedTime;
		private int			verifyMonth;
		private int			verifyDay;
		private int			verifyYear;
		private DateTime	i_RevisedDate;
		private Label lblDischargeDate;
		private Label lblTimeVal;
		private Panel panelDischarge;
		private Label lblDischargeVal;
		private Label lblDichargeTime;
		private Panel panelRevised;
		private DateTime	i_CurrentDateTime;
		private string		userRole = string.Empty;
		private bool		hasRemainingActionItems;
	    private Activity i_CurrentActivity;
		#endregion

		#region Constants
		#endregion
	}
}
