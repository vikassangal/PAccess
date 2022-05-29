using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.DischargeViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CancelInpatientStatusViews
{
	/// <summary>
	/// Summary description for CancelInpatientStatusView.
	/// </summary>
	//TODO: Create XML summary comment for CancelInpatientStatusView
	[Serializable]
	public class CancelInpatientStatusView : DischargeBaseView
	{
		#region Event Handlers
		private void cmbPatientType_SelectedIndexChanged(object sender, EventArgs e)
		{
            VisitType selectedPatientType = this.cmbPatientType.SelectedItem as VisitType;
            if( selectedPatientType != null )
            {
                this.Model.KindOfVisit      = selectedPatientType;
                this.Model.HospitalService  = new HospitalService();
            }

			this.PopulateHospitalServices();
			this.CheckForRequiredFields();
		}

		private void cmbHospitalServices_SelectedIndexChanged(object sender, EventArgs e)
		{
			HospitalService hospitalSrvc = cmbHospitalServices.SelectedItem as HospitalService;
			if( hospitalSrvc != null )
			{
				this.Model.HospitalService = hospitalSrvc;
			}
			this.CheckForRequiredFields();
		}

		private new void btnOk_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			this.CancelInpatientStatus();
			this.Cursor = Cursors.Arrow;
		}

		private void CancelInpatientStatusView_Disposed(object sender, EventArgs e)
		{
			this.unregisterEvents();
		}

	    private void PatientTypeRequiredEventHandler(object sender, EventArgs e)
		{
			UIColors.SetRequiredBgColor( this.cmbPatientType );
            this.btnOk.Enabled = false;
		}

	    private void HospitalServiceRequiredEventHandler(object sender, EventArgs e)
		{
			UIColors.SetRequiredBgColor( this.cmbHospitalServices );
            this.btnOk.Enabled = false;
		}

		private void cmbPatientType_Validating(object sender, CancelEventArgs e)
		{
            this.CheckForRequiredFields();
		}

		private void cmbHospitalServices_Validating(object sender, CancelEventArgs e)
		{
            this.CheckForRequiredFields();
		}

		#endregion

		#region Methods
		public override void UpdateView()
		{
            this.btnOk.Enabled = true;

			this.DisplayPatientContext();

			this.userContextView1.Description = "Cancel Inpatient Status";
			this.lblMessages.Text = UIErrorMessages.INPATIENT_STATUS_BED_ASSIGNMENT_MSG;
			this.lblOutstandingActionItemsMsg.Text = UIErrorMessages.INPATIENT_STATUS_CANCEL_VISIT;

			this.lblPatientName.Text = this.Model.Patient.Name.AsFormattedName();
			this.lblAccount.Text = this.Model.AccountNumber.ToString();
			
			this.lblAdmitDateTime.Text = CommonFormatting.LongDateFormat( this.Model.AdmitDate ) + "  " + 
						CommonFormatting.DisplayedTimeFormat( this.Model.AdmitDate );

			this.lblHospitalService.Text = this.Model.HospitalService.DisplayString;
			this.lblPatienttypeFromTxt.Text = this.Model.KindOfVisit.DisplayString;
			this.lblHospitalServiceFromTxt.Text = this.Model.HospitalService.DisplayString;
			this.lblLocationFromTxt.Text = this.Model.Location.FormattedLocation;
			
			this.PopulatePatientTypes();

			this.lblInstructionalMessage.Text = String.Empty;

			if( !this.IsAccountValidToCancelInpatientStatus() )
			{
				this.cmbPatientType.Enabled = false;
				this.cmbHospitalServices.Enabled = false;
				this.btnOk.Enabled = false;
				return;
			}

			this.Model.KindOfVisit = new VisitType();
			this.Model.HospitalService = new HospitalService();

			this.registerEvents();
			this.CheckForRequiredFields();
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
		private void registerEvents()
		{
			if( i_Registered )
			{
				return;
			}

            this.RuleEngine.LoadRules( this.Model );
			i_Registered = true;

			RuleEngine.GetInstance().RegisterEvent(typeof(PatientTypeRequired), new EventHandler(PatientTypeRequiredEventHandler));
			RuleEngine.GetInstance().RegisterEvent(typeof(HospitalServiceRequired), new EventHandler(HospitalServiceRequiredEventHandler));
		}

		private void unregisterEvents()
		{
			this.i_Registered = false;

			RuleEngine.GetInstance().UnregisterEvent(typeof(PatientTypeRequired), new EventHandler(PatientTypeRequiredEventHandler));
			RuleEngine.GetInstance().UnregisterEvent(typeof(HospitalServiceRequired), new EventHandler(HospitalServiceRequiredEventHandler));
		}

		private void CheckForRequiredFields()
		{
			// reset all fields that might have error, preferred, or required backgrounds

            this.btnOk.Enabled = true;

			UIColors.SetNormalBgColor( this.cmbPatientType );
			UIColors.SetNormalBgColor( this.cmbHospitalServices );
			
			RuleEngine.GetInstance().EvaluateRule(typeof(PatientTypeRequired), this.Model );
			RuleEngine.GetInstance().EvaluateRule(typeof(HospitalServiceRequired), this.Model );
		}

		private bool IsAccountValidToCancelInpatientStatus()
		{
			string instructionalMsg = String.Empty;
			bool isValid = true;
            
			if( this.Model.KindOfVisit.Code != VisitType.INPATIENT )
			{
				instructionalMsg = this.AddNewLineCharacter( instructionalMsg );
				instructionalMsg += UIErrorMessages.INPATIENT_STATUS_NO_LONGER_INPATIENT;
				isValid = false;
			}

			if( this.Model.AccountLock != null && !this.Model.AccountLock.IsLockAcquiredByCurrentUser())   
			{
				instructionalMsg = this.AddNewLineCharacter( instructionalMsg );
				instructionalMsg += UIErrorMessages.INPATIENT_STATUS_RECORD_LOCKED;
				isValid = false;
			}

			if( this.PatientAdmittedBeforeToday() )
			{
				instructionalMsg = this.AddNewLineCharacter( instructionalMsg );
				instructionalMsg += UIErrorMessages.INPATIENT_STATUS_ADMITDATE_BEFORE_TODAY;
				isValid = false;
			}

			//Check for charges
			if( this.Model.BalanceDue > 0 )
			{
				instructionalMsg = this.AddNewLineCharacter( instructionalMsg );
				instructionalMsg += UIErrorMessages.INPATIENT_STATUS_HAS_CHARGES;
				isValid = false;
			}

			//Check to see if the bill has dropped
			if( this.Model.BillHasDropped )
			{
				instructionalMsg = this.AddNewLineCharacter( instructionalMsg );
				instructionalMsg += UIErrorMessages.INPATIENT_STATUS_BILL_HAS_DROPPED;
				isValid = false;
			}

			if( this.Model.DischargeDate != DateTime.MinValue ||
				this.Model.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT )
			{
				instructionalMsg = this.AddNewLineCharacter( instructionalMsg );
				instructionalMsg += UIErrorMessages.INPATIENT_STATUS_BEEN_DISCHARGED;
				isValid = false;
			}

			if( this.Model.AbstractExists )
			{
				instructionalMsg = this.AddNewLineCharacter( instructionalMsg );
				instructionalMsg += UIErrorMessages.INPATIENT_STATUS_ABSTRACT_COMPLETE;
				isValid = false;
			}

			this.lblInstructionalMessage.Text = instructionalMsg;
			return isValid;
		}

		private string AddNewLineCharacter( string message )
		{
			if( message.Length > 0 )
			{
				message += "\n";
			}

			return message;
		}

		private DateTime GetCurrentFacilityDateTime()
		{
			ITimeBroker timeBroker  = ProxyFactory.GetTimeBroker();
			return timeBroker.TimeAt( User.GetCurrent().Facility.GMTOffset, 
                                      User.GetCurrent().Facility.DSTOffset );
		}

		private bool PatientAdmittedBeforeToday()
		{
			return ( this.Model.AdmitDate.Date < this.GetCurrentFacilityDateTime().Date );
		}

		private void PopulatePatientTypes()
		{
			this.cmbPatientType.Items.Clear();

			if( patientTypes == null )
			{
				PatientBrokerProxy patientBroker = new PatientBrokerProxy( );
                patientTypes = (ArrayList)patientBroker.AllPatientTypes( this.Model.Facility.Oid );
			}

			cmbPatientType.Items.Add( new VisitType() );

			foreach( VisitType patType in patientTypes )
			{
				if( patType.Code == VisitType.OUTPATIENT || 
					patType.Code == VisitType.EMERGENCY_PATIENT ||
					patType.Code == VisitType.NON_PATIENT )
				{
					cmbPatientType.Items.Add(patType);
				}
			}
		}

		private void PopulateHospitalServices()
		{
			ArrayList hospitalServices = null;

			this.cmbHospitalServices.Items.Clear();
            

			if( facilityHospitalServices == null )
			{
				HSVBrokerProxy brokerProxy = new HSVBrokerProxy();
				facilityHospitalServices = ( ArrayList ) brokerProxy.SelectHospitalServicesFor( User.GetCurrent().Facility.Oid );
			}

			HospitalService hospitalService = new HospitalService();

			VisitType inVisitType = (VisitType) this.cmbPatientType.SelectedItem;
			if( inVisitType != null && 
                !string.IsNullOrEmpty( inVisitType.Code ) )
			{                
				hospitalServices = (ArrayList)hospitalService.HospitalServicesFor( facilityHospitalServices,
					inVisitType.Code );
			}

			if( hospitalServices != null && hospitalServices.Count > 0 )
			{
				foreach( HospitalService hs in hospitalServices )
				{
					cmbHospitalServices.Items.Add( hs );
				}
                this.cmbHospitalServices.Sorted = true;

				if( this.Model.HospitalService != null )
				{
					this.cmbHospitalServices.SelectedItem = this.Model.HospitalService;
				}
			}
            
			if( this.cmbPatientType.SelectedItem != null )
			{
				this.cmbHospitalServices.Enabled = true;
			}
		}

		private void CancelInpatientStatus()
		{
			this.lblInstructionalMessage.Text = "Cancel Inpatient Status submitted for processing.";
			this.userContextView1.Description = "Cancel Inpatient Status - Submitted";
			
			this.lblPatientTypeSubmittedTxt.Text = this.Model.KindOfVisit.DisplayString;
			this.lblHspSrvSubmittedTxt.Text = this.Model.HospitalService.DisplayString;
			this.lblLocationSubmittedTxt.Text = this.Model.Location.FormattedLocation;

            this.Model.TransferDate = this.Model.AdmitDate;

			this.panelSubmitted.Visible = true;
			this.panelSubmitted.BringToFront();

			this.lblMessages.Text = UIErrorMessages.INPATIENT_STATUS_CANCEL_SUBMIT;
			this.lblOutstandingActionItemsMsg.Text = String.Empty;
			this.SaveAccount();

			this.lblNextAction.Visible = true;
			this.lblNextActionLine.Visible = true;

			this.panelButtons.Hide();

			this.panelActions.Show();
            this.gbScanDocuments.Visible = true;
			
			this.btnCloseActivity.Focus();

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.panelChangeStatus = new System.Windows.Forms.Panel();
            this.lblHospitalService = new System.Windows.Forms.Label();
            this.lblAdmitDateTime = new System.Windows.Forms.Label();
            this.cmbHospitalServices = new System.Windows.Forms.ComboBox();
            this.cmbPatientType = new System.Windows.Forms.ComboBox();
            this.lblLocationFromTxt = new System.Windows.Forms.Label();
            this.lblHospitalServiceFromTxt = new System.Windows.Forms.Label();
            this.lblPatienttypeFromTxt = new System.Windows.Forms.Label();
            this.lblHospitalServiceTo = new System.Windows.Forms.Label();
            this.lblPatientTypeTo = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblLocationFrom = new System.Windows.Forms.Label();
            this.lblHospitalServiceFrom = new System.Windows.Forms.Label();
            this.lblPatientTypeFrom = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelSubmitted = new System.Windows.Forms.Panel();
            this.lblLocationSubmitted = new System.Windows.Forms.Label();
            this.lblLocationSubmittedTxt = new System.Windows.Forms.Label();
            this.lblHspSrvSubmittedTxt = new System.Windows.Forms.Label();
            this.lblHospitalSvcSubmitted = new System.Windows.Forms.Label();
            this.lblPatientTypeSubmittedTxt = new System.Windows.Forms.Label();
            this.lblPatientTypeSubmitted = new System.Windows.Forms.Label();
            this.panelActions.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelMessages.SuspendLayout();
            this.gbScanDocuments.SuspendLayout();
            this.panelChangeStatus.SuspendLayout();
            this.panelSubmitted.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView1
            // 
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
            this.panelActions.Location = new System.Drawing.Point( 10, 123 );
            // 
            // panel2
            // 
            this.panel2.Controls.Add( this.panelChangeStatus );
            this.panel2.Controls.Add( this.panelSubmitted );
            this.panel2.TabStop = false;
            this.panel2.Controls.SetChildIndex( this.gbScanDocuments, 0 );
            this.panel2.Controls.SetChildIndex( this.panelSubmitted, 0 );
            this.panel2.Controls.SetChildIndex( this.lblDischargeDateVal, 0 );
            this.panel2.Controls.SetChildIndex( this.lblDischargeTimeVal, 0 );
            this.panel2.Controls.SetChildIndex( this.lblDischargeDispositionVal, 0 );
            this.panel2.Controls.SetChildIndex( this.panelMessages, 0 );
            this.panel2.Controls.SetChildIndex( this.lblInstructionalMessage, 0 );
            this.panel2.Controls.SetChildIndex( this.label1, 0 );
            this.panel2.Controls.SetChildIndex( this.lblPatientName, 0 );
            this.panel2.Controls.SetChildIndex( this.label3, 0 );
            this.panel2.Controls.SetChildIndex( this.lblAccount, 0 );
            this.panel2.Controls.SetChildIndex( this.label5, 0 );
            this.panel2.Controls.SetChildIndex( this.label6, 0 );
            this.panel2.Controls.SetChildIndex( this.label7, 0 );
            this.panel2.Controls.SetChildIndex( this.label8, 0 );
            this.panel2.Controls.SetChildIndex( this.label9, 0 );
            this.panel2.Controls.SetChildIndex( this.lblPatientType, 0 );
            this.panel2.Controls.SetChildIndex( this.lblAdmitDate, 0 );
            this.panel2.Controls.SetChildIndex( this.label12, 0 );
            this.panel2.Controls.SetChildIndex( this.label13, 0 );
            this.panel2.Controls.SetChildIndex( this.lblAdmitTime, 0 );
            this.panel2.Controls.SetChildIndex( this.lblDischargeLocation, 0 );
            this.panel2.Controls.SetChildIndex( this.mtbDischargeTime, 0 );
            this.panel2.Controls.SetChildIndex( this.dtpDischargeDate, 0 );
            this.panel2.Controls.SetChildIndex( this.mtbDischargeDate, 0 );
            this.panel2.Controls.SetChildIndex( this.cmbDischargeDisposition, 0 );
            this.panel2.Controls.SetChildIndex( this.lblCurOccupant, 0 );
            this.panel2.Controls.SetChildIndex( this.lblCurrentOccupant, 0 );
            this.panel2.Controls.SetChildIndex( this.panelChangeStatus, 0 );
            // 
            // panelMessages
            // 
            this.panelMessages.Location = new System.Drawing.Point( 6, 358 );
            this.panelMessages.Size = new System.Drawing.Size( 987, 157 );
            // 
            // label2
            // 
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point( 72, 6 );
            // 
            // label5
            // 
            this.label5.Visible = false;
            // 
            // label55
            // 
            this.label55.Location = new System.Drawing.Point( 6, 7 );
            this.label55.Size = new System.Drawing.Size( 70, 13 );
            this.label55.Text = "Next Action";
            // 
            // label6
            // 
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.Visible = false;
            // 
            // label8
            // 
            this.label8.Visible = false;
            // 
            // label9
            // 
            this.label9.Visible = false;
            // 
            // label12
            // 
            this.label12.Visible = false;
            // 
            // label13
            // 
            this.label13.Visible = false;
            // 
            // lblDischargeLocation
            // 
            this.lblDischargeLocation.Visible = false;
            // 
            // lblPatientName
            // 
            this.lblPatientName.Location = new System.Drawing.Point( 108, 74 );
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point( 108, 95 );
            // 
            // lblPatientType
            // 
            this.lblPatientType.Visible = false;
            // 
            // lblAdmitDate
            // 
            this.lblAdmitDate.Visible = false;
            // 
            // lblAdmitTime
            // 
            this.lblAdmitTime.Visible = false;
            // 
            // lblOutstandingActionItemsMsg
            // 
            this.lblOutstandingActionItemsMsg.Location = new System.Drawing.Point( 12, 71 );
            // 
            // lblMessages
            // 
            this.lblMessages.Location = new System.Drawing.Point( 12, 30 );
            this.lblMessages.Size = new System.Drawing.Size( 907, 34 );
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
            this.gbScanDocuments.Location = new System.Drawing.Point( 14, 273 );
            this.gbScanDocuments.Size = new System.Drawing.Size( 322, 82 );
            // 
            // panelChangeStatus
            // 
            this.panelChangeStatus.Controls.Add( this.lblHospitalService );
            this.panelChangeStatus.Controls.Add( this.lblAdmitDateTime );
            this.panelChangeStatus.Controls.Add( this.cmbHospitalServices );
            this.panelChangeStatus.Controls.Add( this.cmbPatientType );
            this.panelChangeStatus.Controls.Add( this.lblLocationFromTxt );
            this.panelChangeStatus.Controls.Add( this.lblHospitalServiceFromTxt );
            this.panelChangeStatus.Controls.Add( this.lblPatienttypeFromTxt );
            this.panelChangeStatus.Controls.Add( this.lblHospitalServiceTo );
            this.panelChangeStatus.Controls.Add( this.lblPatientTypeTo );
            this.panelChangeStatus.Controls.Add( this.label14 );
            this.panelChangeStatus.Controls.Add( this.lblLocationFrom );
            this.panelChangeStatus.Controls.Add( this.lblHospitalServiceFrom );
            this.panelChangeStatus.Controls.Add( this.lblPatientTypeFrom );
            this.panelChangeStatus.Controls.Add( this.label11 );
            this.panelChangeStatus.Controls.Add( this.label10 );
            this.panelChangeStatus.Controls.Add( this.label4 );
            this.panelChangeStatus.Location = new System.Drawing.Point( 4, 131 );
            this.panelChangeStatus.Name = "panelChangeStatus";
            this.panelChangeStatus.Size = new System.Drawing.Size( 729, 142 );
            this.panelChangeStatus.TabIndex = 0;
            // 
            // lblHospitalService
            // 
            this.lblHospitalService.Location = new System.Drawing.Point( 104, 26 );
            this.lblHospitalService.Name = "lblHospitalService";
            this.lblHospitalService.Size = new System.Drawing.Size( 190, 13 );
            this.lblHospitalService.TabIndex = 0;
            // 
            // lblAdmitDateTime
            // 
            this.lblAdmitDateTime.Location = new System.Drawing.Point( 105, 5 );
            this.lblAdmitDateTime.Name = "lblAdmitDateTime";
            this.lblAdmitDateTime.Size = new System.Drawing.Size( 176, 13 );
            this.lblAdmitDateTime.TabIndex = 0;
            // 
            // cmbHospitalServices
            // 
            this.cmbHospitalServices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHospitalServices.Location = new System.Drawing.Point( 438, 100 );
            this.cmbHospitalServices.Name = "cmbHospitalServices";
            this.cmbHospitalServices.Size = new System.Drawing.Size( 202, 21 );
            this.cmbHospitalServices.TabIndex = 2;
            this.cmbHospitalServices.Validating += new System.ComponentModel.CancelEventHandler( this.cmbHospitalServices_Validating );
            this.cmbHospitalServices.SelectedIndexChanged += new System.EventHandler( this.cmbHospitalServices_SelectedIndexChanged );
            // 
            // cmbPatientType
            // 
            this.cmbPatientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatientType.Location = new System.Drawing.Point( 438, 75 );
            this.cmbPatientType.Name = "cmbPatientType";
            this.cmbPatientType.Size = new System.Drawing.Size( 121, 21 );
            this.cmbPatientType.TabIndex = 1;
            this.cmbPatientType.Validating += new System.ComponentModel.CancelEventHandler( this.cmbPatientType_Validating );
            this.cmbPatientType.SelectedIndexChanged += new System.EventHandler( this.cmbPatientType_SelectedIndexChanged );
            // 
            // lblLocationFromTxt
            // 
            this.lblLocationFromTxt.Location = new System.Drawing.Point( 104, 121 );
            this.lblLocationFromTxt.Name = "lblLocationFromTxt";
            this.lblLocationFromTxt.Size = new System.Drawing.Size( 180, 13 );
            this.lblLocationFromTxt.TabIndex = 0;
            // 
            // lblHospitalServiceFromTxt
            // 
            this.lblHospitalServiceFromTxt.Location = new System.Drawing.Point( 104, 99 );
            this.lblHospitalServiceFromTxt.Name = "lblHospitalServiceFromTxt";
            this.lblHospitalServiceFromTxt.Size = new System.Drawing.Size( 176, 13 );
            this.lblHospitalServiceFromTxt.TabIndex = 0;
            // 
            // lblPatienttypeFromTxt
            // 
            this.lblPatienttypeFromTxt.Location = new System.Drawing.Point( 104, 78 );
            this.lblPatienttypeFromTxt.Name = "lblPatienttypeFromTxt";
            this.lblPatienttypeFromTxt.Size = new System.Drawing.Size( 192, 13 );
            this.lblPatienttypeFromTxt.TabIndex = 0;
            // 
            // lblHospitalServiceTo
            // 
            this.lblHospitalServiceTo.Location = new System.Drawing.Point( 352, 101 );
            this.lblHospitalServiceTo.Name = "lblHospitalServiceTo";
            this.lblHospitalServiceTo.Size = new System.Drawing.Size( 88, 13 );
            this.lblHospitalServiceTo.TabIndex = 0;
            this.lblHospitalServiceTo.Text = "Hospital service:";
            // 
            // lblPatientTypeTo
            // 
            this.lblPatientTypeTo.Location = new System.Drawing.Point( 352, 78 );
            this.lblPatientTypeTo.Name = "lblPatientTypeTo";
            this.lblPatientTypeTo.Size = new System.Drawing.Size( 69, 13 );
            this.lblPatientTypeTo.TabIndex = 0;
            this.lblPatientTypeTo.Text = "Patient type:";
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point( 352, 52 );
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size( 319, 13 );
            this.label14.TabIndex = 0;
            this.label14.Text = "Change Inpatient Status to _________________________";
            // 
            // lblLocationFrom
            // 
            this.lblLocationFrom.Location = new System.Drawing.Point( 8, 120 );
            this.lblLocationFrom.Name = "lblLocationFrom";
            this.lblLocationFrom.Size = new System.Drawing.Size( 52, 15 );
            this.lblLocationFrom.TabIndex = 0;
            this.lblLocationFrom.Text = "Location:";
            // 
            // lblHospitalServiceFrom
            // 
            this.lblHospitalServiceFrom.Location = new System.Drawing.Point( 8, 99 );
            this.lblHospitalServiceFrom.Name = "lblHospitalServiceFrom";
            this.lblHospitalServiceFrom.Size = new System.Drawing.Size( 87, 13 );
            this.lblHospitalServiceFrom.TabIndex = 0;
            this.lblHospitalServiceFrom.Text = "Hospital service:";
            // 
            // lblPatientTypeFrom
            // 
            this.lblPatientTypeFrom.Location = new System.Drawing.Point( 8, 78 );
            this.lblPatientTypeFrom.Name = "lblPatientTypeFrom";
            this.lblPatientTypeFrom.Size = new System.Drawing.Size( 67, 13 );
            this.lblPatientTypeFrom.TabIndex = 0;
            this.lblPatientTypeFrom.Text = "Patient type:";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point( 8, 52 );
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size( 333, 13 );
            this.label11.TabIndex = 0;
            this.label11.Text = "Change Inpatient Status from ___________________________";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point( 8, 26 );
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size( 87, 13 );
            this.label10.TabIndex = 0;
            this.label10.Text = "Hospital service:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point( 8, 5 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 88, 14 );
            this.label4.TabIndex = 0;
            this.label4.Text = "Admit date/time:";
            // 
            // panelSubmitted
            // 
            this.panelSubmitted.Controls.Add( this.lblLocationSubmitted );
            this.panelSubmitted.Controls.Add( this.lblLocationSubmittedTxt );
            this.panelSubmitted.Controls.Add( this.lblHspSrvSubmittedTxt );
            this.panelSubmitted.Controls.Add( this.lblHospitalSvcSubmitted );
            this.panelSubmitted.Controls.Add( this.lblPatientTypeSubmittedTxt );
            this.panelSubmitted.Controls.Add( this.lblPatientTypeSubmitted );
            this.panelSubmitted.Location = new System.Drawing.Point( 6, 153 );
            this.panelSubmitted.Name = "panelSubmitted";
            this.panelSubmitted.Size = new System.Drawing.Size( 793, 120 );
            this.panelSubmitted.TabIndex = 0;
            this.panelSubmitted.Visible = false;
            // 
            // lblLocationSubmitted
            // 
            this.lblLocationSubmitted.Location = new System.Drawing.Point( 5, 55 );
            this.lblLocationSubmitted.Name = "lblLocationSubmitted";
            this.lblLocationSubmitted.Size = new System.Drawing.Size( 87, 14 );
            this.lblLocationSubmitted.TabIndex = 0;
            this.lblLocationSubmitted.Text = "Location:";
            // 
            // lblLocationSubmittedTxt
            // 
            this.lblLocationSubmittedTxt.Location = new System.Drawing.Point( 102, 55 );
            this.lblLocationSubmittedTxt.Name = "lblLocationSubmittedTxt";
            this.lblLocationSubmittedTxt.Size = new System.Drawing.Size( 192, 13 );
            this.lblLocationSubmittedTxt.TabIndex = 0;
            // 
            // lblHspSrvSubmittedTxt
            // 
            this.lblHspSrvSubmittedTxt.Location = new System.Drawing.Point( 102, 30 );
            this.lblHspSrvSubmittedTxt.Name = "lblHspSrvSubmittedTxt";
            this.lblHspSrvSubmittedTxt.Size = new System.Drawing.Size( 192, 13 );
            this.lblHspSrvSubmittedTxt.TabIndex = 0;
            // 
            // lblHospitalSvcSubmitted
            // 
            this.lblHospitalSvcSubmitted.Location = new System.Drawing.Point( 5, 30 );
            this.lblHospitalSvcSubmitted.Name = "lblHospitalSvcSubmitted";
            this.lblHospitalSvcSubmitted.Size = new System.Drawing.Size( 87, 14 );
            this.lblHospitalSvcSubmitted.TabIndex = 0;
            this.lblHospitalSvcSubmitted.Text = "Hospital service:";
            // 
            // lblPatientTypeSubmittedTxt
            // 
            this.lblPatientTypeSubmittedTxt.Location = new System.Drawing.Point( 102, 5 );
            this.lblPatientTypeSubmittedTxt.Name = "lblPatientTypeSubmittedTxt";
            this.lblPatientTypeSubmittedTxt.Size = new System.Drawing.Size( 192, 13 );
            this.lblPatientTypeSubmittedTxt.TabIndex = 0;
            // 
            // lblPatientTypeSubmitted
            // 
            this.lblPatientTypeSubmitted.Location = new System.Drawing.Point( 5, 5 );
            this.lblPatientTypeSubmitted.Name = "lblPatientTypeSubmitted";
            this.lblPatientTypeSubmitted.Size = new System.Drawing.Size( 87, 14 );
            this.lblPatientTypeSubmitted.TabIndex = 0;
            this.lblPatientTypeSubmitted.Text = "Patient type:";
            // 
            // CancelInpatientStatusView
            // 
            this.AcceptButton = this.btnOk;
            this.Name = "CancelInpatientStatusView";
            this.Size = new System.Drawing.Size( 1022, 619 );
            this.Disposed += new System.EventHandler( this.CancelInpatientStatusView_Disposed );
            this.panelActions.ResumeLayout( false );
            this.panel2.ResumeLayout( false );
            this.panel2.PerformLayout();
            this.panelPatientContext.ResumeLayout( false );
            this.panelButtons.ResumeLayout( false );
            this.panelMessages.ResumeLayout( false );
            this.gbScanDocuments.ResumeLayout( false );
            this.panelChangeStatus.ResumeLayout( false );
            this.panelSubmitted.ResumeLayout( false );
            this.ResumeLayout( false );

		}
		#endregion

		#endregion

		#region Private Properties
		private RuleEngine RuleEngine
		{
			get
			{
				if( i_RuleEngine == null )
				{
					i_RuleEngine = RuleEngine.GetInstance();
				}
				return i_RuleEngine;
			}
		}
		#endregion

		#region Construction and Finalization
		public CancelInpatientStatusView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            btnPrintFaceSheet.Visible = false;
			// TODO: Add any initialization after the InitializeComponent call
			base.EnableThemesOn( this );
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
		private Label label4;
		private Label label10;
		private Label label11;
		private Label lblPatientTypeFrom;
		private Label lblHospitalServiceFrom;
		private Label lblLocationFrom;
		private Label label14;
		private Label lblPatientTypeTo;
		private Label lblHospitalServiceTo;
		private Label lblPatienttypeFromTxt;
		private Label lblHospitalServiceFromTxt;
		private Label lblLocationFromTxt;
		private ComboBox cmbPatientType;
		private Label lblAdmitDateTime;
		private Label lblHospitalService;
		private ComboBox cmbHospitalServices;

		private ArrayList patientTypes = null;
		private Panel	panelChangeStatus;
		private Label	lblPatientTypeSubmitted;
		private Label	lblPatientTypeSubmittedTxt;
		private Label	lblHospitalSvcSubmitted;
		private Label	lblLocationSubmittedTxt;
		private Label	lblLocationSubmitted;
		private Panel	panelSubmitted;
		private Label	lblHspSrvSubmittedTxt;
		private ArrayList					facilityHospitalServices = null;
		private bool                        i_Registered = false;
		private RuleEngine                  i_RuleEngine;
		#endregion
	}
}
