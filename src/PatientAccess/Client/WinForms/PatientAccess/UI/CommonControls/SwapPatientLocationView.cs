using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.TransferViews;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// Summary description for SwapPatientLocationView.
	/// </summary>
	public class SwapPatientLocationView : ControlView
	{
        
        #region Events

        public event EventHandler VerificationSucceeded;
        public event EventHandler VerificationFailed;   
        
        public event EventHandler NewHSVCodeChanged;   
        public event EventHandler NewAccommodationCodeChanged;   
        
        public event EventHandler LeaveSearchField;   

        #endregion            
        
        #region Event Handlers

        private void ShowPanel()
        {
            if( !this.IsValidAccountNumber() )
            { 
                return;
            }
            else
            {
                TransferService.SetNormalBgColor( this.mtbSearchAccount );
                this.Refresh();
            }

            this.panelBaseInfoArea.Visible      = false;
            this.panelCurrentArea.Visible       = false;
            this.panelNewArea.Visible           = false;

            this.progressPanel1.Visible         = true; 
        }

        private void HidePanel()
        {
            this.progressPanel1.Visible         = false;    
 
            if( !this.lblSearchResultMsg.Visible )
            {
                this.panelBaseInfoArea.Visible      = true;
                this.panelCurrentArea.Visible       = true;
                this.panelNewArea.Visible           = true;
            }
        }

        private void BeforeWork()
        {
            this.ShowPanel();
            this.Cursor = Cursors.WaitCursor;      
        }


        private void DoWork( object sender, DoWorkEventArgs e )
        {
            // Sanjeev Kumar: do not add any try/catch statements within the DoWork.
            // Exceptions are caught in backgroundworkers automatically in the RunWorkerComplete
            // through e.Error, which can be checked.

            // pass in account number to be searched, avoid using UI elements in backgroundworker
            this.validateReturnCode = VerifyAccount(this.mtbSearchAccount.Text);

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

/*
            try
            {
                // pass in account number to be searched, avoid using UI elements in backgroundworker
                this.validateReturnCode = VerifyAccount(this.mtbSearchAccount.Text.ToString());
            }
            catch
            {
                throw;
            }
*/
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e)
        {
            if ( this.IsDisposed || this.Disposing )
                return ;

            if ( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if (e.Error.GetType() == typeof(RemotingTimeoutException))
                {
                    MessageBox.Show(UIErrorMessages.TIMEOUT_SWAP_PATIENT_LOCATION_ACCOUNT_SEARCH);
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success
                AccountVerifyValidateReturnCode();
            }

            this.Cursor = Cursors.Default;
            this.grpPatient1.BringToFront();
            this.HidePanel();
        }

        private void AccountVerifyValidateReturnCode()
        {
            if (aPatient != null)
                this.lblPatientNameVal.Text = aPatient.LastName + ", " + aPatient.FirstName + " " + aPatient.MiddleInitial;

            if (anAcctProxy != null)
            {
                this.lblAccountVal.Text = anAcctProxy.AccountNumber.ToString();

                this.lblAdmitDateVal.Text = CommonFormatting.LongDateFormat(anAcctProxy.AdmitDate);
                this.lblAdmitTimeVal.Text = CommonFormatting.DisplayedTimeFormat(anAcctProxy.AdmitDate);

                this.lblPatientTypeVal.Text = anAcctProxy.KindOfVisit.DisplayString;

                this.lblHospitalServiceVal.Text = anAcctProxy.HospitalService.DisplayString;

                this.lblLocationVal.Text = anAcctProxy.Location.DisplayString;

                if (anAcctProxy.Location.Bed.Accomodation != null)
                {
                    this.lblAccomodationVal.Text = anAcctProxy.Location.Bed.Accomodation.Code + " " + anAcctProxy.Location.Bed.Accomodation.Description;
                }
            }

            switch ((VerificationResult)this.validateReturnCode)
            {
                case VerificationResult.SUCCESS:
                    this.ViewStatus = "EditView";
                    this.PopulateNewHSV();
                    VerificationSucceeded(this, new LooseArgs(this.Model));
                    break;

                case VerificationResult.NOT_FOUND:
                    this.ViewStatus = "InitView";
                    this.lblSearchResultMsg.Text = UIErrorMessages.NO_PATIENT_FOUND_MSG;
                    this.lblSearchResultMsg.Visible = true;
                    this.VerificationFailed(this, new EventArgs());
                    this.mtbSearchAccount.Focus();
                    this.mtbSearchAccount.SelectionStart = 0;
                    this.mtbSearchAccount.SelectionLength = this.mtbSearchAccount.Text.Length;
                    break;

                case VerificationResult.NOT_IN_BED:
                    this.ViewStatus = "InitView";
                    this.lblSearchResultMsg.Text = UIErrorMessages.NOT_IN_BED_PATIENT_MSG;
                    this.lblSearchResultMsg.Visible = true;
                    this.VerificationFailed(this, new EventArgs());
                    break;

                case VerificationResult.REC_LOCKED:
                    this.ViewStatus = "InitView";
                    this.lblSearchResultMsg.Text = UIErrorMessages.PATIENT_RECORD_LOCKED_MSG;
                    this.lblSearchResultMsg.Visible = true;
                    this.VerificationFailed(this, new EventArgs());
                    break;

                case VerificationResult.REC_LOCK_FAILURE:
                    this.ViewStatus = "InitView";
                    this.VerificationFailed(this, new EventArgs());
                    break;

                case VerificationResult.DISCHARGED:
                    this.ViewStatus = "InitView";
                    this.lblSearchResultMsg.Text = UIErrorMessages.TRANSFER_HAS_DISCHARGEDATE;
                    this.lblSearchResultMsg.Visible = true;
                    this.VerificationFailed(this, new EventArgs());
                    break;

                default:
                    break;
            }
        }

        private void SwapPatientLocationView_Load(object sender, EventArgs e)
        {
            if(this.Name.Equals("swapPatientLocationView1"))
            {
                this.mtbSearchAccount.Focus();
            }
            else
            {
                this.btnVerify.Text = "&Verify Account";
            }

            this.progressPanel1.Visible = false;
            this.ShowInitialView();
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if( this.backgroundWorker == null
                ||
                ( this.backgroundWorker != null
                && !this.backgroundWorker.IsBusy )
              )
            {
                this.BeforeWork();

                this.backgroundWorker = new BackgroundWorker();
                this.backgroundWorker.WorkerSupportsCancellation = true;

                backgroundWorker.DoWork += new DoWorkEventHandler( DoWork );
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( AfterWork );

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void mtbSearchAccount_TextChanged( object sender, EventArgs e )
        {
            if( this.mtbSearchAccount.Text.Length != 0 )            
                btnVerify.Enabled = true;            
            else
                btnVerify.Enabled = false;
        }

        private void mtbSearchAccount_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = this.btnVerify;
        }

        private void mtbSearchAccount_Leave(object sender, EventArgs e)
        {
            this.LeaveSearchField( null, EventArgs.Empty );
            this.AcceptButton = null;

        }

        private void cboHospitalService_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cboAccomodations1.SelectedIndex = -1;
            this.NewHSVCodeChanged( null, EventArgs.Empty );   
        }

        private void cboAccomodations1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.NewAccommodationCodeChanged( null, EventArgs.Empty );       
        }

        private void cboAccomodations1_DropDown(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( this.cboAccomodations1 );    
        }

        private void cboHospitalService_DropDown(object sender, EventArgs e)
        {        
            UIColors.SetNormalBgColor( this.cboHospitalService );     
        }
        #endregion    
        
        #region Public Methods
  
        #endregion  
        
        #region Properties
        public new AccountProxy Model
        {
            get
            {
                return (AccountProxy)base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public AccountProxy OtherAccountProxy
        {
            private get
            {
                return i_OtherAccountProxy;
            }
            set
            {
                i_OtherAccountProxy = value;
            }
        }

        public string PatientLabel
        {
            get
            {
                return this.grpPatient1.Text;
            }
            set
            {
                this.grpPatient1.Text = value;
            }
        }

        public string ViewStatus
        {
            get
            {
                return i_ViewStatus;
            }
            set
            {
                i_ViewStatus = value;
                switch( i_ViewStatus )
                {
                    case "InitView":
                        this.ShowInitialView();
                        break;
                    case "EditView":
                        this.ShowEditView();
                        break;
                    case "ConfirmView":
                        this.ShowConfirmationView();
                        break;
                }   
            }
        }

        public Location NewLocation
        {
            private get
            {
                return i_NewLocation;
            }
            set
            {
                if( i_NewLocation != value )
                {
                    i_NewLocation = value; 
                    this.PopulateNewLocation();
                    this.PopulateNewAccommodation();
                }           
            }
        }

        #endregion      
       
        #region Private Methods

        private void ShowInitialView()
        {
            this.panelBaseInfoArea.Visible      = false;
            this.lblCurrent.Visible             = false;
            this.panelCurrentArea.Visible       = false;
            this.lblNew.Visible                 = false;
            this.panelNewArea.Visible           = false;
        }

        private void ShowEditView()
        {
            this.lblSearchResultMsg.Visible     = false;
            this.panelBaseInfoArea.Visible      = true;
            this.lblCurrent.Visible             = true;
            this.panelCurrentArea.Visible       = true;
            this.lblNew.Visible                 = true;
            this.panelNewArea.Visible           = true;           
        }

        private void ShowConfirmationView()
        {
            this.lblHospitalServiceVal.Text = this.cboHospitalService.SelectedItem.ToString();
            this.lblLocationVal.Text = this.lblNewLocationVal.Text;
            this.lblAccomodationVal.Text = this.cboAccomodations1.SelectedItem != null? this.cboAccomodations1.SelectedItem.ToString() : "";
                    
            this.panelPat1SearchArea.Visible = false;            
            this.lblCurrent.Visible = false;
            this.lblNew.Visible = false;
            this.panelNewArea.Visible = false;
            this.grpPatient1.Size = new Size(328, 193);
            this.panelBaseInfoArea.Location = new Point(  this.panelBaseInfoArea.Location.X, 25 );
            this.panelCurrentArea.Location = new Point(  this.panelCurrentArea.Location.X, 118 );

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
        }

        private int VerifyAccount(string acctNumSearch) 
        {
            if( this.OtherAccountProxy != null && this.OtherAccountProxy.AccountNumber.ToString() == acctNumSearch )
            {
                return (int)VerificationResult.REC_LOCKED;
            }
            
            PatientSearchCriteria patientCriteria = new PatientSearchCriteria( 
                User.GetCurrent().Facility.Code,
                "",
                "",
                "",
                null,
                0,
                0,
                "",
                acctNumSearch
                );

            //Check to see if search data entered is valid.
            ValidationResult result = patientCriteria.Validate();            
            
            if( result.IsValid )
            {                            

                PatientBrokerProxy patientBrokerProxy = new PatientBrokerProxy( );

                PatientSearchResponse patientSearchResponse = 
                    patientBrokerProxy.GetPatientSearchResponseFor( patientCriteria );
    
                if( patientSearchResponse.PatientSearchResults.Count > 0 )
                {

                    aPatient = 
                        patientBrokerProxy.PatientFrom( patientSearchResponse.PatientSearchResults[0] );

                    if (aPatient.Accounts != null)
                    {
                        foreach (AccountProxy aProxy in (ArrayList)aPatient.Accounts)
                        {
                            if (aProxy.AccountNumber.ToString() == acctNumSearch)
                            {
                                anAcctProxy = aProxy;
                                break;
                            }
                        }
                    }

                    if (anAcctProxy != null)
                        anAcctProxy.Activity = new TransferBedSwapActivity();

                    if( anAcctProxy == null )
                    {
                        return (int)VerificationResult.NOT_FOUND; 
                    }
                    else if( anAcctProxy.Location == null ||
                        anAcctProxy.Location.DisplayString == "" ||
                        (anAcctProxy.KindOfVisit != null && 
                        (anAcctProxy.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT ||
                        anAcctProxy.KindOfVisit.Code == VisitType.PREREG_PATIENT) )  )
                    {
                        return (int)VerificationResult.NOT_IN_BED;
                    }
                    else if( anAcctProxy.DischargeDate != DateTime.MinValue )
                    {
                        return (int)VerificationResult.DISCHARGED;
                    }
                    else if( !AccountLockStatus.IsAccountLocked( anAcctProxy, User.GetCurrent() ) )
                    {
                        if( !ManageAccountLock( anAcctProxy ) )
                        {
                            return (int)VerificationResult.REC_LOCK_FAILURE;
                        }
                    }
                    else if( !AccountLockStatus.IsLockAcquiredByCurrentUser( anAcctProxy, User.GetCurrent() ) )
                    {
                        return (int)VerificationResult.REC_LOCKED;
                    }
                
                    anAcctProxy.LocationFrom = TransferService.DeepCopyLocation( anAcctProxy.Location );                    

                    this.Model = anAcctProxy;
                    return (int)VerificationResult.SUCCESS; 
                }
                else
                {
                    return (int)VerificationResult.NOT_FOUND; 
                }
            }
            else
                return (int)VerificationResult.UNKNOWN;
        }

        private bool ManageAccountLock( AccountProxy acctProxy )
        {
            if( i_PreviousAccount != null )
            {
               AccountActivityService.ReleaseAccountlock( i_PreviousAccount );   
               ActivityEventAggregator.GetInstance().RaiseAccountUnLockEvent( 
                   this, new LooseArgs( acctProxy ) );
            }

            i_PreviousAccount = acctProxy;
            return AccountActivityService.PlaceLockOn( acctProxy, UIErrorMessages.PATIENT_RECORD_LOCKED_MSG );
        }

        private void PopulateNewHSV()
        {
            if( this.Model.KindOfVisit.Code == VisitType.INPATIENT )
            {
                HSVBrokerProxy brokerProxy = new HSVBrokerProxy();
                ICollection hsvCodes = (ICollection)brokerProxy.HospitalServicesFor( User.GetCurrent().Facility.Oid, "1" );

                cboHospitalService.Items.Clear();
                foreach( HospitalService hospitalService in hsvCodes )
                {
                    if(( hospitalService.IPTransferRestriction == this.Model.HospitalService.IPTransferRestriction )
                        ||(hospitalService.Code.Trim().Length == 0)
                        )
                    {
                        cboHospitalService.Items.Add( hospitalService );
                    }
                }
            } 
            else // TLG 02/02/2006 added
            {
                HSVBrokerProxy brokerProxy = new HSVBrokerProxy();
                ICollection hsvCodes = (ICollection)brokerProxy.HospitalServicesFor( User.GetCurrent().Facility.Oid, this.Model.KindOfVisit.Code, "Y" );

                cboHospitalService.Items.Clear();
                foreach (HospitalService hospitalService in hsvCodes)
                {
                    cboHospitalService.Items.Add(hospitalService);
                }
            }

            if( this.cboHospitalService.Items.Count > 0 )
            {
                this.cboHospitalService.Sorted = true;
                if( this.Model.HospitalService != null )
                {
                    cboHospitalService.SelectedItem = this.Model.HospitalService;
                }        
            }
        }

        private void PopulateNewAccommodation()
        {
            if( this.NewLocation == null || this.Model.KindOfVisit.Code != VisitType.INPATIENT )
            {
                cboAccomodations1.SelectedIndex = -1;
                cboAccomodations1.Enabled = false;
            }
            else
            {        
                LocationBrokerProxy broker = new LocationBrokerProxy( );
                ArrayList accomodationsCodes = ( ArrayList )broker.AccomodationCodesFor( this.NewLocation.NursingStation.Code, User.GetCurrent().Facility);
                cboAccomodations1.Items.Clear();

                foreach( Accomodation accomodation in accomodationsCodes )
                {
                    cboAccomodations1.Items.Add( accomodation );                    
                }
                cboAccomodations1.SelectedIndex = 0;
                cboAccomodations1.Enabled = cboAccomodations1.Items.Count != 0? true : false;
//                this.NewAccommodationCodeChanged( this, new EventArgs() );   
            }
        }

        private void PopulateNewLocation()
        {
            if( this.NewLocation != null )
            {
                this.lblNewLocationVal.Text = this.NewLocation.DisplayString;
            }
            else
                this.lblNewLocationVal.Text = "";
        }

        private bool IsValidAccountNumber()
        {
            Facility facility = User.GetCurrent().Facility;
            int modType = ( int )facility.ModType;     

            bool result = Account.IsValidAccountNumber( modType, Convert.ToInt64( mtbSearchAccount.Text ) );

            if( !result )
            {
                TransferService.SetErrBgColor( this.mtbSearchAccount );
                MessageBox.Show( UIErrorMessages.ERR_MSG_INVALID_ACCOUNT_NUMBER, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                this.mtbSearchAccount.Focus();               
            }  
            return result;
        }
        #endregion  
        
        #region Private Properties

        #endregion 

		public SwapPatientLocationView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
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

                CancelBackgroundWorker();

                //this.backgroundWorker = null;
			}
			base.Dispose( disposing );
		}

        private void CancelBackgroundWorker()
        {
            // cancel any background worker activity that was launched 
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.CancelAsync();
            }
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.grpPatient1 = new System.Windows.Forms.GroupBox();
            this.panelBaseInfoArea = new System.Windows.Forms.Panel();
            this.lblPatientTypeVal = new System.Windows.Forms.Label();
            this.lblAdmitTimeVal = new System.Windows.Forms.Label();
            this.lblAdmitTime = new System.Windows.Forms.Label();
            this.lblAdmitDateVal = new System.Windows.Forms.Label();
            this.lblAccountVal = new System.Windows.Forms.Label();
            this.lblPatientNameVal = new System.Windows.Forms.Label();
            this.lblAdmitDate = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.lblNew = new System.Windows.Forms.Label();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.panelPat1SearchArea = new System.Windows.Forms.Panel();
            this.lblSearchAccount = new System.Windows.Forms.Label();
            this.btnVerify = new LoggingButton();
            this.mtbSearchAccount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panelCurrentArea = new System.Windows.Forms.Panel();
            this.lblAccomodationVal = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblHospitalService = new System.Windows.Forms.Label();
            this.lblLocationVal = new System.Windows.Forms.Label();
            this.lblHospitalServiceVal = new System.Windows.Forms.Label();
            this.lblAccomodation = new System.Windows.Forms.Label();
            this.panelNewArea = new System.Windows.Forms.Panel();
            this.lblNewLocationVal = new System.Windows.Forms.Label();
            this.cboAccomodations1 = new System.Windows.Forms.ComboBox();
            this.cboHospitalService = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.lblSearchResultMsg = new System.Windows.Forms.Label();
            this.grpPatient1.SuspendLayout();
            this.panelBaseInfoArea.SuspendLayout();
            this.panelPat1SearchArea.SuspendLayout();
            this.panelCurrentArea.SuspendLayout();
            this.panelNewArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPatient1
            // 
            this.grpPatient1.Controls.Add(this.progressPanel1);
            this.grpPatient1.Controls.Add(this.panelBaseInfoArea);
            this.grpPatient1.Controls.Add(this.lblNew);
            this.grpPatient1.Controls.Add(this.lblCurrent);
            this.grpPatient1.Controls.Add(this.panelPat1SearchArea);
            this.grpPatient1.Controls.Add(this.panelCurrentArea);
            this.grpPatient1.Controls.Add(this.panelNewArea);            
            this.grpPatient1.Controls.Add(this.lblSearchResultMsg);
            this.grpPatient1.Location = new System.Drawing.Point(0, 8);
            this.grpPatient1.Name = "grpPatient1";
            this.grpPatient1.Size = new System.Drawing.Size(328, 384);
            this.grpPatient1.TabIndex = 57;
            this.grpPatient1.TabStop = false;
            this.grpPatient1.Text = "Patient 1";
            // 
            // panelBaseInfoArea
            // 
            this.panelBaseInfoArea.Controls.Add(this.lblPatientTypeVal);
            this.panelBaseInfoArea.Controls.Add(this.lblAdmitTimeVal);
            this.panelBaseInfoArea.Controls.Add(this.lblAdmitTime);
            this.panelBaseInfoArea.Controls.Add(this.lblAdmitDateVal);
            this.panelBaseInfoArea.Controls.Add(this.lblAccountVal);
            this.panelBaseInfoArea.Controls.Add(this.lblPatientNameVal);
            this.panelBaseInfoArea.Controls.Add(this.lblAdmitDate);
            this.panelBaseInfoArea.Controls.Add(this.lblAccount);
            this.panelBaseInfoArea.Controls.Add(this.lblPatientName);
            this.panelBaseInfoArea.Controls.Add(this.lblPatientType);
            this.panelBaseInfoArea.Location = new System.Drawing.Point(8, 80);
            this.panelBaseInfoArea.Name = "panelBaseInfoArea";
            this.panelBaseInfoArea.Size = new System.Drawing.Size(312, 96);
            this.panelBaseInfoArea.TabIndex = 56;
            this.panelBaseInfoArea.Visible = true;
            // 
            // lblPatientTypeVal
            // 
            this.lblPatientTypeVal.BackColor = System.Drawing.Color.White;
            this.lblPatientTypeVal.Location = new System.Drawing.Point(104, 72);
            this.lblPatientTypeVal.Name = "lblPatientTypeVal";
            this.lblPatientTypeVal.Size = new System.Drawing.Size(191, 16);
            this.lblPatientTypeVal.TabIndex = 22;
            // 
            // lblAdmitTimeVal
            // 
            this.lblAdmitTimeVal.BackColor = System.Drawing.Color.White;
            this.lblAdmitTimeVal.Location = new System.Drawing.Point(248, 48);
            this.lblAdmitTimeVal.Name = "lblAdmitTimeVal";
            this.lblAdmitTimeVal.Size = new System.Drawing.Size(47, 13);
            this.lblAdmitTimeVal.TabIndex = 49;
            // 
            // lblAdmitTime
            // 
            this.lblAdmitTime.BackColor = System.Drawing.Color.White;
            this.lblAdmitTime.Location = new System.Drawing.Point(208, 48);
            this.lblAdmitTime.Name = "lblAdmitTime";
            this.lblAdmitTime.Size = new System.Drawing.Size(35, 13);
            this.lblAdmitTime.TabIndex = 48;
            this.lblAdmitTime.Text = "Time:";
            // 
            // lblAdmitDateVal
            // 
            this.lblAdmitDateVal.BackColor = System.Drawing.Color.White;
            this.lblAdmitDateVal.Location = new System.Drawing.Point(104, 48);
            this.lblAdmitDateVal.Name = "lblAdmitDateVal";
            this.lblAdmitDateVal.Size = new System.Drawing.Size(88, 16);
            this.lblAdmitDateVal.TabIndex = 18;
            // 
            // lblAccountVal
            // 
            this.lblAccountVal.BackColor = System.Drawing.Color.White;
            this.lblAccountVal.Location = new System.Drawing.Point(104, 24);
            this.lblAccountVal.Name = "lblAccountVal";
            this.lblAccountVal.Size = new System.Drawing.Size(191, 16);
            this.lblAccountVal.TabIndex = 17;
            // 
            // lblPatientNameVal
            // 
            this.lblPatientNameVal.BackColor = System.Drawing.Color.White;
            this.lblPatientNameVal.Location = new System.Drawing.Point(104, 0);
            this.lblPatientNameVal.Name = "lblPatientNameVal";
            this.lblPatientNameVal.Size = new System.Drawing.Size(191, 16);
            this.lblPatientNameVal.TabIndex = 16;
            // 
            // lblAdmitDate
            // 
            this.lblAdmitDate.BackColor = System.Drawing.Color.White;
            this.lblAdmitDate.Location = new System.Drawing.Point(16, 48);
            this.lblAdmitDate.Name = "lblAdmitDate";
            this.lblAdmitDate.Size = new System.Drawing.Size(88, 23);
            this.lblAdmitDate.TabIndex = 2;
            this.lblAdmitDate.Text = "Admit date:";
            // 
            // lblAccount
            // 
            this.lblAccount.BackColor = System.Drawing.Color.White;
            this.lblAccount.Location = new System.Drawing.Point(16, 24);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(88, 23);
            this.lblAccount.TabIndex = 1;
            this.lblAccount.Text = "Account:";
            // 
            // lblPatientName
            // 
            this.lblPatientName.BackColor = System.Drawing.Color.White;
            this.lblPatientName.Location = new System.Drawing.Point(16, 0);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(88, 23);
            this.lblPatientName.TabIndex = 0;
            this.lblPatientName.Text = "Patient name:";
            // 
            // lblPatientType
            // 
            this.lblPatientType.BackColor = System.Drawing.Color.White;
            this.lblPatientType.Location = new System.Drawing.Point(16, 72);
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size(88, 23);
            this.lblPatientType.TabIndex = 13;
            this.lblPatientType.Text = "Patient type:";
            // 
            // lblNew
            // 
            this.lblNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblNew.Location = new System.Drawing.Point(24, 280);
            this.lblNew.Name = "lblNew";
            this.lblNew.Size = new System.Drawing.Size(48, 16);
            this.lblNew.TabIndex = 39;
            this.lblNew.Text = "New";
            this.lblNew.Visible = false;
            // 
            // lblCurrent
            // 
            this.lblCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblCurrent.Location = new System.Drawing.Point(24, 184);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(100, 16);
            this.lblCurrent.TabIndex = 53;
            this.lblCurrent.Text = "Current";
            this.lblCurrent.Visible = false;
            // 
            // panelPat1SearchArea
            // 
            this.panelPat1SearchArea.Controls.Add(this.lblSearchAccount);
            this.panelPat1SearchArea.Controls.Add(this.btnVerify);
            this.panelPat1SearchArea.Controls.Add(this.mtbSearchAccount);
            this.panelPat1SearchArea.Controls.Add(this.label4);
            this.panelPat1SearchArea.Location = new System.Drawing.Point(8, 16);
            this.panelPat1SearchArea.Name = "panelPat1SearchArea";
            this.panelPat1SearchArea.Size = new System.Drawing.Size(312, 56);
            this.panelPat1SearchArea.TabIndex = 53;
            // 
            // lblSearchAccount
            // 
            this.lblSearchAccount.Location = new System.Drawing.Point(16, 16);
            this.lblSearchAccount.Name = "lblSearchAccount";
            this.lblSearchAccount.Size = new System.Drawing.Size(48, 16);
            this.lblSearchAccount.TabIndex = 50;
            this.lblSearchAccount.Text = "Account:";
            // 
            // btnVerify
            // 
            this.btnVerify.Enabled = false;
            this.btnVerify.Location = new System.Drawing.Point(192, 16);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(95, 23);
            this.btnVerify.TabIndex = 1;
            this.btnVerify.Text = "Verify &Account";
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // mtbSearchAccount
            // 
            this.mtbSearchAccount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbSearchAccount.KeyPressExpression = "^\\d*$";
            this.mtbSearchAccount.Location = new System.Drawing.Point(72, 16);
            this.mtbSearchAccount.Mask = "";
            this.mtbSearchAccount.MaxLength = 9;
            this.mtbSearchAccount.Multiline = true;
            this.mtbSearchAccount.Name = "mtbSearchAccount";
            this.mtbSearchAccount.Size = new System.Drawing.Size(96, 20);
            this.mtbSearchAccount.TabIndex = 0;
            this.mtbSearchAccount.ValidationExpression = "^\\d*$";
            this.mtbSearchAccount.Leave += new System.EventHandler(this.mtbSearchAccount_Leave);
            this.mtbSearchAccount.TextChanged += new System.EventHandler(this.mtbSearchAccount_TextChanged);
            this.mtbSearchAccount.Enter += new System.EventHandler(this.mtbSearchAccount_Enter);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(298, 23);
            this.label4.TabIndex = 54;
            this.label4.Text = "_________________________________________________________";
            // 
            // panelCurrentArea
            // 
            this.panelCurrentArea.BackColor = System.Drawing.Color.White;
            this.panelCurrentArea.Controls.Add(this.lblAccomodationVal);
            this.panelCurrentArea.Controls.Add(this.lblLocation);
            this.panelCurrentArea.Controls.Add(this.lblHospitalService);
            this.panelCurrentArea.Controls.Add(this.lblLocationVal);
            this.panelCurrentArea.Controls.Add(this.lblHospitalServiceVal);
            this.panelCurrentArea.Controls.Add(this.lblAccomodation);
            this.panelCurrentArea.Location = new System.Drawing.Point(8, 200);
            this.panelCurrentArea.Name = "panelCurrentArea";
            this.panelCurrentArea.Size = new System.Drawing.Size(312, 72);
            this.panelCurrentArea.TabIndex = 52;
            this.panelCurrentArea.Visible = true;
            // 
            // lblAccomodationVal
            // 
            this.lblAccomodationVal.BackColor = System.Drawing.Color.White;
            this.lblAccomodationVal.Location = new System.Drawing.Point(104, 47);
            this.lblAccomodationVal.Name = "lblAccomodationVal";
            this.lblAccomodationVal.Size = new System.Drawing.Size(196, 15);
            this.lblAccomodationVal.TabIndex = 26;
            // 
            // lblLocation
            // 
            this.lblLocation.BackColor = System.Drawing.Color.White;
            this.lblLocation.Location = new System.Drawing.Point(16, 26);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(84, 16);
            this.lblLocation.TabIndex = 15;
            this.lblLocation.Text = "Location:";
            // 
            // lblHospitalService
            // 
            this.lblHospitalService.BackColor = System.Drawing.Color.White;
            this.lblHospitalService.Location = new System.Drawing.Point(16, 4);
            this.lblHospitalService.Name = "lblHospitalService";
            this.lblHospitalService.Size = new System.Drawing.Size(88, 23);
            this.lblHospitalService.TabIndex = 14;
            this.lblHospitalService.Text = "Hospital service:";
            // 
            // lblLocationVal
            // 
            this.lblLocationVal.BackColor = System.Drawing.Color.White;
            this.lblLocationVal.Location = new System.Drawing.Point(104, 27);
            this.lblLocationVal.Name = "lblLocationVal";
            this.lblLocationVal.Size = new System.Drawing.Size(196, 16);
            this.lblLocationVal.TabIndex = 24;
            // 
            // lblHospitalServiceVal
            // 
            this.lblHospitalServiceVal.BackColor = System.Drawing.Color.White;
            this.lblHospitalServiceVal.Location = new System.Drawing.Point(104, 4);
            this.lblHospitalServiceVal.Name = "lblHospitalServiceVal";
            this.lblHospitalServiceVal.Size = new System.Drawing.Size(196, 16);
            this.lblHospitalServiceVal.TabIndex = 23;
            // 
            // lblAccomodation
            // 
            this.lblAccomodation.BackColor = System.Drawing.Color.White;
            this.lblAccomodation.Location = new System.Drawing.Point(16, 47);
            this.lblAccomodation.Name = "lblAccomodation";
            this.lblAccomodation.Size = new System.Drawing.Size(91, 20);
            this.lblAccomodation.TabIndex = 25;
            this.lblAccomodation.Text = "Accommodation:";
            // 
            // panelNewArea
            // 
            this.panelNewArea.BackColor = System.Drawing.Color.White;
            this.panelNewArea.Controls.Add(this.lblNewLocationVal);
            this.panelNewArea.Controls.Add(this.cboAccomodations1);
            this.panelNewArea.Controls.Add(this.cboHospitalService);
            this.panelNewArea.Controls.Add(this.label3);
            this.panelNewArea.Controls.Add(this.label5);
            this.panelNewArea.Controls.Add(this.label6);
            this.panelNewArea.Location = new System.Drawing.Point(8, 288);
            this.panelNewArea.Name = "panelNewArea";
            this.panelNewArea.Size = new System.Drawing.Size(312, 88);
            this.panelNewArea.TabIndex = 55;
            this.panelNewArea.Visible = true;
            // 
            // lblNewLocationVal
            // 
            this.lblNewLocationVal.BackColor = System.Drawing.Color.White;
            this.lblNewLocationVal.Location = new System.Drawing.Point(102, 40);
            this.lblNewLocationVal.Name = "lblNewLocationVal";
            this.lblNewLocationVal.Size = new System.Drawing.Size(192, 16);
            this.lblNewLocationVal.TabIndex = 40;
            // 
            // cboAccomodations1
            // 
            this.cboAccomodations1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAccomodations1.Enabled = false;
            this.cboAccomodations1.Location = new System.Drawing.Point(102, 62);
            this.cboAccomodations1.Name = "cboAccomodations1";
            this.cboAccomodations1.Size = new System.Drawing.Size(116, 21);
            this.cboAccomodations1.TabIndex = 3;
            this.cboAccomodations1.DropDown += new System.EventHandler(this.cboAccomodations1_DropDown);
            this.cboAccomodations1.SelectedIndexChanged += new System.EventHandler(this.cboAccomodations1_SelectedIndexChanged);
            // 
            // cboHospitalService
            // 
            this.cboHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHospitalService.Location = new System.Drawing.Point(102, 11);
            this.cboHospitalService.Name = "cboHospitalService";
            this.cboHospitalService.Size = new System.Drawing.Size(191, 21);
            this.cboHospitalService.TabIndex = 2;
            this.cboHospitalService.DropDown += new System.EventHandler(this.cboHospitalService_DropDown);
            this.cboHospitalService.SelectedIndexChanged += new System.EventHandler(this.cboHospitalService_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(16, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 23);
            this.label3.TabIndex = 25;
            this.label3.Text = "Accommodation:";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(16, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 23);
            this.label5.TabIndex = 15;
            this.label5.Text = "Location:";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(16, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 23);
            this.label6.TabIndex = 14;
            this.label6.Text = "Hospital service:";
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(8, 80);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(312, 296);
            this.progressPanel1.TabIndex = 58;
            // 
            // lblSearchResultMsg
            // 
            this.lblSearchResultMsg.Location = new System.Drawing.Point(24, 104);
            this.lblSearchResultMsg.Name = "lblSearchResultMsg";
            this.lblSearchResultMsg.Size = new System.Drawing.Size(280, 80);
            this.lblSearchResultMsg.TabIndex = 57;
            // 
            // SwapPatientLocationView
            // 
            this.AcceptButton = this.btnVerify;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.grpPatient1);
            this.Name = "SwapPatientLocationView";
            this.Size = new System.Drawing.Size(336, 408);
            this.Load += new System.EventHandler(this.SwapPatientLocationView_Load);
            this.grpPatient1.ResumeLayout(false);
            this.panelBaseInfoArea.ResumeLayout(false);
            this.panelPat1SearchArea.ResumeLayout(false);
            this.panelCurrentArea.ResumeLayout(false);
            this.panelNewArea.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        #region Data Elements

        private AccountProxy                                    anAcctProxy = null;
        private Patient                                         aPatient = null;
        private Container components = null;

        private ProgressPanel   progressPanel1;
        
        private BackgroundWorker          backgroundWorker;        
        
        private Location                                        i_NewLocation = null;
        private AccountProxy                                    i_PreviousAccount = null;
        private AccountProxy                                    i_OtherAccountProxy = null; 

        public MaskedEditTextBox         mtbSearchAccount;

        private LoggingButton                     btnVerify;   

        public  ComboBox                   cboHospitalService;
        public  ComboBox                   cboAccomodations1;

        private GroupBox                   grpPatient1;

        private Panel                      panelPat1SearchArea;
        private Panel                      panelCurrentArea;
        private Panel                      panelNewArea;
        private Panel                      panelBaseInfoArea;

        private Label                      lblSearchAccount;    
        private Label                      label4;
        private Label                      lblAccountVal;
        private Label                      lblPatientNameVal;
        private Label                      lblAdmitDate;
        private Label                      lblAccount;
        private Label                      lblPatientName;
        private Label                      lblPatientType;
        private Label                      lblPatientTypeVal;
        private Label                      lblAdmitTimeVal;
        private Label                      lblAdmitTime;
        private Label                      lblAdmitDateVal;
        private Label                      lblAccomodation;
        private Label                      lblAccomodationVal;
        private Label                      lblLocation;
        private Label                      lblHospitalService;
        private Label                      lblLocationVal;
        private Label                      lblHospitalServiceVal;
        private Label                      label3;
        private Label                      label5;
        private Label                      label6;
        private Label                      lblNewLocationVal;
        private Label                      lblNew;
        private Label                      lblCurrent;
        private Label                      lblSearchResultMsg;       

        private string                                          i_ViewStatus;

        private int                                             validateReturnCode;

        #endregion

        #region Constants
       
        private enum VerificationResult 
        {
            SUCCESS            = 0,
            NOT_FOUND          = 1,
            NOT_IN_BED         = 2,
            REC_LOCKED         = 3,
            DISCHARGED         = 5,
            REC_LOCK_FAILURE   = 6,
            UNKNOWN            = 10
        }

        #endregion
	}
}
