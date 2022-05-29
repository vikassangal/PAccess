using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Domain.InterFacilityTransfer;
using PatientAccess.Rules;
using PatientAccess.UI.InterfacilityTransfer;

namespace PatientAccess.UI.DischargeViews
{
	/// <summary>
	/// Summary description for DischargePatientView.
	/// </summary>
	[Serializable]
	public class DischargePatientView : DischargeBaseView
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

		private void cmbDischargeDisposition_SelectedIndexChanged(object sender, EventArgs e)
		{
			ValidateRequiredFields();
			if( cmbDischargeDisposition.SelectedIndex != -1 && cmbDischargeDisposition.SelectedItem.ToString() != String.Empty)
            {
                ValidateIftrDischargeDisposition(cmbDischargeDisposition.SelectedItem.ToString());
				Model.DischargeDisposition = (DischargeDisposition)cmbDischargeDisposition.SelectedItem;
			}
		}

        private void ValidateIftrDischargeDisposition(string dischargeDisposition)
        {
            if (isItfrEnabled && hasInterfacilityAccount)
            {
                interfacilityPannel.Visible = true;
                if (dischargeDisposition != "ACUTE-ANOTHER HOSP")
                {
                    InterfacilityPopup interfacilityPopup = new InterfacilityPopup();
                    interfacilityPopup.HeaderText = "This patient has a transfer attached with another hospital and changes to discharge disposition will cancel the transfer. Do you still want to change the discharge disposition and continue with the discharge?";
                    interfacilityPopup.SetOKButton = false;
                    interfacilityPopup.SetYesButton = true;
                    interfacilityPopup.SetNoButton = true;
                    interfacilityPopup.SetCancelButton = false;
                    interfacilityPopup.ShowDialog(this);

                    if (interfacilityPopup.CancelActivity)
                    {
                        cmbDischargeDisposition.SelectedIndex= cmbDischargeDisposition.FindStringExact("ACUTE-ANOTHER HOSP");
                    }
                    else
                    {
                        interfacilityPannel.Visible = false;
                    }
                }
            }
        }

        private new void btnOk_Click(object sender, EventArgs e)
		{
		    bool isDateTimeValid = validateDateAndTime("NONE");

		    if( isDateTimeValid )
		    {
		        Cursor = Cursors.WaitCursor;
		        DischargePatient();
		        Cursor = Cursors.Default;
		    }

		    else
		    {
		        btnOk.Enabled = true;
		        return;
		    }

		    base.btnOk_Click(sender, e);
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

		private void dtpDischargeDate_CloseUp(object sender, EventArgs e)
		{
            mtbDischargeDate.UnMaskedText = dtpDischargeDate.Value.ToString("MMddyyyy");
			SetDischargeDateNormalBgColor();            
            ValidateRequiredFields();
            mtbDischargeDate.Focus();
		}
		#endregion

		#region Methods
		public override void UpdateView()
		{
			if( Model != null )
			{
				DisplayPatientContext();
				DisplayDischargeDispositions();

				//PopulateControls
				lblPatientName.Text = Model.Patient.FormattedName;
				lblAccount.Text = Model.AccountNumber.ToString();
				lblPatientType.Text = Model.KindOfVisit.DisplayString;

                lblAdmitDate.Text = CommonFormatting.LongDateFormat( Model.AdmitDate );
                lblAdmitTime.Text = CommonFormatting.DisplayedTimeFormat( Model.AdmitDate );

				if( Model.Location != null )
				{
					lblDischargeLocation.Text = Model.Location.DisplayString;
				}

				if( Model.DischargeDate.Year.ToString() != "1" || !ValidPatientType() )
				{
                    mtbDischargeDate.UnMaskedText = CommonFormatting.MaskedDateFormat( Model.DischargeDate );
                    
                    if( Model.DischargeDate.Hour != 0 ||
                        Model.DischargeDate.Minute != 0 )
                    {
                        mtbDischargeTime.UnMaskedText = CommonFormatting.MaskedTimeFormat( Model.DischargeDate );
                    }
                    else
                    {
                        mtbDischargeTime.UnMaskedText = "";
                    }                   

					lblInstructionalMessage.Text = UIErrorMessages.DISCHARGE_NOT_AVAILABLE_MSG;

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

					cmbDischargeDisposition.Enabled = false;
					mtbDischargeDate.Enabled = false;
					mtbDischargeTime.Enabled = false;
					dtpDischargeDate.Enabled = false;
				}
				else
				{
					cmbDischargeDisposition.SelectedIndex = -1;
					CheckForValuables();
					CheckForRemainingActionItems();
					PopulateDefaultDischargeDateTime();

				}

                mtbDischargeDate.Focus();

                if (Model.Facility.IsSATXEnabled)
                {
                    InterFacilityTransferFeatureManager interFacilityTransferFeatureManager =
                        new InterFacilityTransferFeatureManager();
                    isItfrEnabled =
                        interFacilityTransferFeatureManager.IsITFREnabled(User.GetCurrent().Facility,
                            this.Model as Account);
                    BindInterfacility();
                }
                else
                {
                    interfacilityPannel.VisibilityPanel(false);
                }

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

		#endregion

		#region Private Methods

		private void ValidateRequiredFields()
		{
			DischargeDisposition dischargeDisposition = cmbDischargeDisposition.SelectedItem as DischargeDisposition;

			if( mtbDischargeDate.UnMaskedText != String.Empty && 
				mtbDischargeTime.UnMaskedText != String.Empty &&
				dischargeDisposition != null
                && dischargeDisposition.Code.Trim() != string.Empty )
			{
				btnOk.Enabled = true;
			}
			else
			{
				btnOk.Enabled = false;
			}
		}

		private bool IsDischargeDateBeforeAdmitDate(string field)
		{
            SetDischargeDateNormalBgColor();
            SetDischargeTimeNormalBgColor();

            // admit date/time
		    int iAdmitMonth = Model.AdmitDate.Month;
            int iAdmitDay = Model.AdmitDate.Day;
            int iAdmitYear = Model.AdmitDate.Year;

            int iAdmitHour = Model.AdmitDate.Hour;
            int iAdmitMinute = Model.AdmitDate.Minute;

            DateTime admitDate      = new DateTime( iAdmitYear, iAdmitMonth, iAdmitDay );
            DateTime admitDateTime  = new DateTime( iAdmitYear, iAdmitMonth, iAdmitDay, iAdmitHour, iAdmitMinute, 0 );

            // discharge date/time

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
                    ( iDischargeHour == 0 && iDischargeMinute == 0 && dischargeDate.Date < admitDate.Date ) 
                    ||
                    ( ( iDischargeHour != 0 || iDischargeMinute != 0 ) && dischargeDate < admitDate )
                  )
                {                
                    if( field == "DATE" )
                    {
                        SetDischargeDateErrBgColor();
                        if( !mtbDischargeTime.Focused )
                        {
                            mtbDischargeDate.Focus();
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
                        if( !mtbDischargeTime.Focused )
                        {
                            mtbDischargeDate.Focus();
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
				if( Convert.ToInt16(hour) > 23 )
				{
					SetDischargeTimeErrBgColor();
					MessageBox.Show( UIErrorMessages.HOUR_INVALID_ERRMSG, "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					return true;
				}

                minute = mtbDischargeTime.Text.Substring( 3, 2 );
				if( Convert.ToInt16(minute) > 59 )
				{
					SetDischargeTimeErrBgColor();
					MessageBox.Show( UIErrorMessages.MINUTE_INVALID_ERRMSG, "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					return true;
				}

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

                if( ( verifyHour == 0 && verifyMinute == 0 && theDate.Date > currentFacilityTime.Date )
                    ||
                    ( ( verifyHour != 0 || verifyMinute != 0 ) && theDate > currentFacilityTime )
                  )
				{
                    if( field == "DATE" )
                    {
                        SetDischargeDateErrBgColor();
                        if( !mtbDischargeTime.Focused )
                        {
                            mtbDischargeDate.Focus();
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

		private void DischargePatient()
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

			lblInstructionalMessage.Text = "Discharge Patient submitted for processing.";
			userContextView1.Description = "Discharge Patient - Submitted";

			CheckForValuables();
			CheckForRemainingActionItems();

            if ( CheckAgeOver65AtDischarge() )
            {
                if ( Model.Insurance != null )
                {
                    Coverage primaryCoverage = Model.Insurance.PrimaryCoverage;
                    Coverage secondaryCoverage = Model.Insurance.SecondaryCoverage;

                    if (( primaryCoverage != null && primaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ))
                        && ( secondaryCoverage == null
                            || ( secondaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ))))
                    {
                        MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_MSG,
                            UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Information );
                    }
                }
            }

			btnCloseActivity.Focus();

			RemoveMaskedTextBorder();

			//Disable controls now that the discharge is complete.
			mtbDischargeDate.Enabled = false;
			dtpDischargeDate.Enabled = false;
			mtbDischargeTime.Enabled = false;
			cmbDischargeDisposition.Enabled = false;

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );

            if (Model.Facility.IsSATXEnabled)
            {
                // Handle Discharge Confirmation screen for IFTR
                interfacilityPannel.SetValueForConfirmationScreen();
            }
            else
            {
                interfacilityPannel.VisibilityPanel(false);
            }

        }

        private void PopulateDefaultDischargeDateTime()
        {
			ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
			DateTime facilityDateTime = timeBroker.TimeAt( Model.Facility.GMTOffset, 
			                                            Model.Facility.DSTOffset );

            mtbDischargeDate.UnMaskedText = CommonFormatting.MaskedDateFormat( facilityDateTime );
            mtbDischargeTime.UnMaskedText = CommonFormatting.MaskedTimeFormat( facilityDateTime );

        }  

        private void BindInterfacility()
        {
            if (isItfrEnabled)
            {
                var transferPatient = Model.Patient;
                var transferAccount = Model;
                IInterfacilityTransferBroker interFacilityTransferBroker = BrokerFactory.BrokerOfType<IInterfacilityTransferBroker>();
                InterFacilityTransferAccount interFacilityTransferAccount = new InterFacilityTransferAccount();
                interFacilityTransferAccount = interFacilityTransferBroker.Accountsfromtransferlogfordischarge(transferPatient.MedicalRecordNumber, transferAccount.Facility, transferAccount.AccountNumber);
                if (interFacilityTransferAccount != null)
                {
                    var dtToAccount = interFacilityTransferBroker.GetAllAccountsForPatient(transferPatient.MedicalRecordNumber, interFacilityTransferAccount.ToFacility);
                    interFacilityTransferAccount.Activity = this.Model.Activity;
                    interfacilityPannel.TranferToAccount = interFacilityTransferAccount;
                    interfacilityPannel.SetHospitalLabel = "To Hospital: ";
                    interfacilityPannel.SetAccountLabel = "To Account: ";
                    interfacilityPannel.SetHSV = dtToAccount[0].HospitalService.DisplayString;
                    interfacilityPannel.SetPT = dtToAccount[0].KindOfVisit.DisplayString;
                    interfacilityPannel.UpdateView();
                    hasInterfacilityAccount = true;
                    cmbDischargeDisposition.SelectedIndex =
                        cmbDischargeDisposition.FindStringExact("ACUTE-ANOTHER HOSP");
                }
                else
                {
                    hasInterfacilityAccount = false;
                    interfacilityPannel.Visible = false;
                }
            }
            else
            {
                hasInterfacilityAccount = false;
                interfacilityPannel.Visible = false;
            }
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DischargePatientView));
            this.interfacilityPannel = new PatientAccess.UI.InterfacilityTransfer.InterfacilityPannel();
            this.panelActions.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelMessages.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView1
            // 
            this.userContextView1.Description = "Discharge Patient";
            this.userContextView1.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.TabIndex = 5;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cmbDischargeDisposition
            // 
            this.cmbDischargeDisposition.Size = new System.Drawing.Size(208, 24);
            this.cmbDischargeDisposition.SelectedIndexChanged += new System.EventHandler(this.cmbDischargeDisposition_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.interfacilityPannel);
            this.panel2.Controls.SetChildIndex(this.lblDischargeDateVal, 0);
            this.panel2.Controls.SetChildIndex(this.lblDischargeTimeVal, 0);
            this.panel2.Controls.SetChildIndex(this.lblDischargeDispositionVal, 0);
            this.panel2.Controls.SetChildIndex(this.panelMessages, 0);
            this.panel2.Controls.SetChildIndex(this.lblInstructionalMessage, 0);
            this.panel2.Controls.SetChildIndex(this.label1, 0);
            this.panel2.Controls.SetChildIndex(this.lblPatientName, 0);
            this.panel2.Controls.SetChildIndex(this.label3, 0);
            this.panel2.Controls.SetChildIndex(this.lblAccount, 0);
            this.panel2.Controls.SetChildIndex(this.label5, 0);
            this.panel2.Controls.SetChildIndex(this.label6, 0);
            this.panel2.Controls.SetChildIndex(this.label7, 0);
            this.panel2.Controls.SetChildIndex(this.label8, 0);
            this.panel2.Controls.SetChildIndex(this.label9, 0);
            this.panel2.Controls.SetChildIndex(this.lblPatientType, 0);
            this.panel2.Controls.SetChildIndex(this.lblAdmitDate, 0);
            this.panel2.Controls.SetChildIndex(this.label12, 0);
            this.panel2.Controls.SetChildIndex(this.label13, 0);
            this.panel2.Controls.SetChildIndex(this.lblAdmitTime, 0);
            this.panel2.Controls.SetChildIndex(this.lblDischargeLocation, 0);
            this.panel2.Controls.SetChildIndex(this.mtbDischargeTime, 0);
            this.panel2.Controls.SetChildIndex(this.dtpDischargeDate, 0);
            this.panel2.Controls.SetChildIndex(this.mtbDischargeDate, 0);
            this.panel2.Controls.SetChildIndex(this.cmbDischargeDisposition, 0);
            this.panel2.Controls.SetChildIndex(this.lblCurOccupant, 0);
            this.panel2.Controls.SetChildIndex(this.lblCurrentOccupant, 0);
            this.panel2.Controls.SetChildIndex(this.gbScanDocuments, 0);
            this.panel2.Controls.SetChildIndex(this.interfacilityPannel, 0);
            // 
            // interfacilityPannel
            // 
            this.interfacilityPannel.Location = new System.Drawing.Point(440, 19);
            this.interfacilityPannel.Name = "interfacilityPannel";
            this.interfacilityPannel.SetAccountLabel = "To Account:";
            this.interfacilityPannel.Size = new System.Drawing.Size(553, 289);
            this.interfacilityPannel.TabIndex = 9;
            this.interfacilityPannel.TranferToAccount = ((PatientAccess.Domain.InterFacilityTransfer.InterFacilityTransferAccount)(resources.GetObject("interfacilityPannel.TranferToAccount")));
            // 
            // DischargePatientView
            // 
            this.Name = "DischargePatientView";
            this.panelActions.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelPatientContext.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.panelMessages.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public DischargePatientView()
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
		private int			verifyMonth;
		private int			verifyDay;
		private int			verifyYear;
		private int			verifyHour;
		private int			verifyMinute;
        private InterfacilityTransfer.InterfacilityPannel interfacilityPannel;
        private bool hasInterfacilityAccount = false;
        private bool isItfrEnabled = false;
        #endregion

        #region Constants
        #endregion

    }
}
