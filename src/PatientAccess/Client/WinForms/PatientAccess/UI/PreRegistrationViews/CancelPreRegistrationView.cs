using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.WorklistViews;

namespace PatientAccess.UI.PreRegistrationViews
{
    /// <summary>
    /// Summary description for CancelPreRegistrationView.
    /// </summary>
    [Serializable]
    public class CancelPreRegistrationView : ControlView
    {
        #region Event Handlers
        private void CancelPreRegistrationView_Load( object sender, EventArgs e )
        {
            UpdateView();
        }

        private void txtMailingAddress_TextChanged( object sender, EventArgs e )
        {
            TextBox tb = sender as TextBox;
            tb.SelectionLength = 0;
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            if( AccountActivityService.ConfirmCancelActivity( 
                sender, new LooseArgs( this.Model_Account ) ) )
            {
                if( preRegistrationSearchView != null )
                {
                    preRegistrationSearchView.Dispose();
                }
                else if( worklistsView != null )
                {
                    worklistsView.Dispose();
                }
                CancelBackgroundWorker();
                Dispose();
            }
        }

        private void CancelBackgroundWorker()
        {
            // cancel the background worker here...
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.CancelAsync();
            }
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
			this.Cursor = Cursors.WaitCursor;

            try
            {
                Account anAccount = this.Model_Account;
                
                Activity currentActivity = new CancelPreRegActivity();
                currentActivity.AppUser = User.GetCurrent();

                anAccount.Activity = currentActivity;
                anAccount.Facility = User.GetCurrent().Facility;

                IAccountBroker broker  = BrokerFactory.BrokerOfType<IAccountBroker>();

                //anAccount = broker.Save( anAccount, currentActivity );
                AccountSaveResults results = broker.Save( anAccount, currentActivity );
                results.SetResultsTo(Model_Account);
                
                CancelPreRegConfirmationView confirmation = new CancelPreRegConfirmationView( this );
                confirmation.CloseActivity  += new EventHandler(confirmationView_CloseActivity);
                confirmation.Model = Model_Account;
                confirmation.Dock  = DockStyle.Fill;
                Controls.Add( confirmation );

                foreach( Control control in Controls )
                {
                    if( control != null )
                    {
                        control.Hide();
                    }
                }

                int index = Controls.IndexOf( confirmation );
                if( index != -1 )
                {
                    Controls[ index ].Show();
                    Controls[ index ].BringToFront();
                    Controls[ index ].Update();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            //Raise Activitycomplete event
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( 
                this, 
                new LooseArgs( this.Model_Account ) 
                );
        }

        private void confirmationView_CloseActivity(object sender, EventArgs e)
        {
            if( this.ParentForm.GetType() == typeof(PatientAccessView) )
            {
                ((PatientAccessView)this.ParentForm).ReLoad();
            }
        }
        #endregion

        #region Methods

        // Sanjeev Kumar: do not add any try/catch statements within the DoWork.
        // Exceptions are caught in backgroundworkers automatically in the RunWorkerComplete
        // through e.Error, which can be checked.
        //
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // do lengthy processing here...

            this.Model_Account = AccountActivityService.SelectedAccountFor(this.SelectedAccount);
            this.Model_Account.Activity = this.SelectedAccount.Activity;

            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, new LooseArgs( this.Model_Account ) );

            // Sanjeev Kumar: This background worker supports cancellation (WorkerSupportsCancellation = true ).
            // So need to poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            //
            // Note that due to a race condition in the DoWork event handler, the Cancelled
            // flag may not have been set, even though CancelAsync was called.
            //
            if ( this.backgroundWorker.CancellationPending )
            {
                e.Cancel = true ;
                return ;
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return ;

            // Handle the cancelled case first
            if ( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                this.progressPanel1.Visible = false;
                throw e.Error;
            }
            else
            {
                // success
                this.DoUpdateView();
            }

            // place post completion operations here...
            this.progressPanel1.Visible = false;
            this.Cursor = Cursors.Default;
        }

        public override void UpdateView()
        {
            if( this.SelectedAccount != null )
            {
                this.progressPanel1.Visible = true;

                this.Cursor = Cursors.WaitCursor;

                if( this.backgroundWorker == null
                ||
                ( this.backgroundWorker != null
                && !this.backgroundWorker.IsBusy )
                )
                {
                    this.backgroundWorker = new BackgroundWorker();
                    this.backgroundWorker.WorkerSupportsCancellation = true;

                    backgroundWorker.DoWork +=
                        new DoWorkEventHandler( DoWork );
                    backgroundWorker.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler( AfterWork );

                    backgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                this.DoUpdateView();
            }
        }

        private void DoUpdateView()
        {            
            patientContextView.Model   = Model_Account.Patient;
            patientContextView.Account = Model_Account;
            patientContextView.UpdateView();

            lblPatientName.Text        = Model_Account.Patient.FormattedName;
            lblAccount.Text            = Model_Account.AccountNumber.ToString();
			if( this.Model_Account.DerivedVisitType != null &&
				this.Model_Account.DerivedVisitType.Trim() != String.Empty)
			{
				lblPatientType.Text    = Model_Account.DerivedVisitType;				
			}
			else
			{
				lblPatientType.Text    = Model_Account.KindOfVisit.ToString();
			}

            lblDOB.Text                = Model_Account.Patient.DateOfBirth.ToString( "d", DateTimeFormatInfo.InvariantInfo );
            if( Model_Account.AdmittingPhysician != null )
            {   // returns Account.PhysicianWithRole( PhysicianRole.Admitting().Role() );
                // which may be null if a physician is not found
                lblAdmittingPhysician.Text = String.Format("{0:00000} {1}", 
                                                           Model_Account.AdmittingPhysician.PhysicianNumber, 
                                                           Model_Account.AdmittingPhysician.FormattedName );                      
            }
            if( Model_Account.HospitalClinic != null )
            {
                lblClinic1.Text            = Model_Account.HospitalClinic.ToString();
            }
            lblContact1.Text           = Model_Account.EmergencyContact1.Name;
            DateTime admitDate         = Model_Account.AdmitDate;
            lblAdmitDate.Text          = Model_Account.AdmitDate.ToString( "d", DateTimeFormatInfo.InvariantInfo );
            lblTime.Text               = Model_Account.AdmitDate.ToString( "t", DateTimeFormatInfo.InvariantInfo);
            ContactPoint contactPoint  = Model_Account.Patient.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() );
            txtMailingAddress.Text     = contactPoint.Address.AsMailingLabel();            

            if( this.Model_Account.KindOfVisit.IsPreRegistrationPatient == false )
            {
                btnOk.Enabled = false;
                lblMessage.Text = UIErrorMessages.PREREG_NOT_PREREG_PATIENT_ERRMSG;
            }
            else if( this.Model_Account.AccountLock != null && !this.Model_Account.AccountLock.IsLockAcquiredByCurrentUser() )
            {
                btnOk.Enabled = false;
                lblMessage.Text = UIErrorMessages.PREREG_ACCOUNT_LOCKED_ERRMSG;
            }
            else if( Model_Account.IsValidForCancel() == false )
            {
                btnOk.Enabled = false;
                lblMessage.Text = UIErrorMessages.PREREG_PATIENT_HAS_CHARGES_ERRMSG;
            }
            Cursor = Cursors.Default;
        }
        #endregion

        #region Properties

        public IAccount SelectedAccount
        {
            private get
            {
                return this.selectedAccount;
            }
            set
            {
                this.selectedAccount = value;
            }
        }

        private Account Model_Account
        {
            get
            {
                return (Account)this.Model;
            }
            set
            {
                this.Model = value as Account;
            }
        }
        #endregion

        #region Private Methods
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.userContextView = new PatientAccess.UI.UserContextView();
            this.panelPatientContext = new System.Windows.Forms.Panel();
            this.patientContextView = new PatientAccess.UI.PatientContextView();
            this.panelControls = new System.Windows.Forms.Panel();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblStaticTime = new System.Windows.Forms.Label();
            this.lblAdmitDate = new System.Windows.Forms.Label();
            this.lblStaticAdmitDate = new System.Windows.Forms.Label();
            this.lblContact1 = new System.Windows.Forms.Label();
            this.lblStaticContact1 = new System.Windows.Forms.Label();
            this.lblClinic5 = new System.Windows.Forms.Label();
            this.lblStaticClinic5 = new System.Windows.Forms.Label();
            this.lblClinic4 = new System.Windows.Forms.Label();
            this.lblStaticClinic4 = new System.Windows.Forms.Label();
            this.lblClinic3 = new System.Windows.Forms.Label();
            this.lblStaticClinic3 = new System.Windows.Forms.Label();
            this.lblClinic2 = new System.Windows.Forms.Label();
            this.lblStaticClinic2 = new System.Windows.Forms.Label();
            this.lblClinic1 = new System.Windows.Forms.Label();
            this.lblStaticClinic1 = new System.Windows.Forms.Label();
            this.lblAdmittingPhysician = new System.Windows.Forms.Label();
            this.lblStaticPhysician = new System.Windows.Forms.Label();
            this.txtMailingAddress = new System.Windows.Forms.TextBox();
            this.lblDOB = new System.Windows.Forms.Label();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblStaticAddress = new System.Windows.Forms.Label();
            this.lblStaticDOB = new System.Windows.Forms.Label();
            this.lblStaticPatientType = new System.Windows.Forms.Label();
            this.lblStaticAccount = new System.Windows.Forms.Label();
            this.lblStaticPatientName = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnOk = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.panelPatientContext.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView
            // 
            this.userContextView.BackColor = System.Drawing.SystemColors.Control;
            this.userContextView.Description = "Cancel Preregistration";
            this.userContextView.Dock = System.Windows.Forms.DockStyle.Top;
            this.userContextView.Location = new System.Drawing.Point( 0, 0 );
            this.userContextView.Model = null;
            this.userContextView.Name = "userContextView";
            this.userContextView.Size = new System.Drawing.Size( 1024, 23 );
            this.userContextView.TabIndex = 0;
            this.userContextView.TabStop = false;
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.panelPatientContext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPatientContext.Controls.Add( this.patientContextView );
            this.panelPatientContext.Location = new System.Drawing.Point( 8, 24 );
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new System.Drawing.Size( 1008, 26 );
            this.panelPatientContext.TabIndex = 0;
            // 
            // patientContextView
            // 
            this.patientContextView.Account = null;
            this.patientContextView.BackColor = System.Drawing.Color.White;
            this.patientContextView.DateOfBirthText = "";
            this.patientContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView.ForeColor = System.Drawing.SystemColors.ControlText;
            this.patientContextView.GenderLabelText = "";
            this.patientContextView.Location = new System.Drawing.Point( 0, 0 );
            this.patientContextView.Model = null;
            this.patientContextView.Name = "patientContextView";
            this.patientContextView.PatientNameText = "";
            this.patientContextView.Size = new System.Drawing.Size( 1006, 24 );
            this.patientContextView.SocialSecurityNumber = "";
            this.patientContextView.TabIndex = 0;
            this.patientContextView.TabStop = false;
            // 
            // panelControls
            // 
            this.panelControls.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.panelControls.BackColor = System.Drawing.Color.White;
            this.panelControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelControls.Controls.Add( this.progressPanel1 );
            this.panelControls.Controls.Add( this.lblTime );
            this.panelControls.Controls.Add( this.lblStaticTime );
            this.panelControls.Controls.Add( this.lblAdmitDate );
            this.panelControls.Controls.Add( this.lblStaticAdmitDate );
            this.panelControls.Controls.Add( this.lblContact1 );
            this.panelControls.Controls.Add( this.lblStaticContact1 );
            this.panelControls.Controls.Add( this.lblClinic5 );
            this.panelControls.Controls.Add( this.lblStaticClinic5 );
            this.panelControls.Controls.Add( this.lblClinic4 );
            this.panelControls.Controls.Add( this.lblStaticClinic4 );
            this.panelControls.Controls.Add( this.lblClinic3 );
            this.panelControls.Controls.Add( this.lblStaticClinic3 );
            this.panelControls.Controls.Add( this.lblClinic2 );
            this.panelControls.Controls.Add( this.lblStaticClinic2 );
            this.panelControls.Controls.Add( this.lblClinic1 );
            this.panelControls.Controls.Add( this.lblStaticClinic1 );
            this.panelControls.Controls.Add( this.lblAdmittingPhysician );
            this.panelControls.Controls.Add( this.lblStaticPhysician );
            this.panelControls.Controls.Add( this.txtMailingAddress );
            this.panelControls.Controls.Add( this.lblDOB );
            this.panelControls.Controls.Add( this.lblPatientType );
            this.panelControls.Controls.Add( this.lblAccount );
            this.panelControls.Controls.Add( this.lblPatientName );
            this.panelControls.Controls.Add( this.lblStaticAddress );
            this.panelControls.Controls.Add( this.lblStaticDOB );
            this.panelControls.Controls.Add( this.lblStaticPatientType );
            this.panelControls.Controls.Add( this.lblStaticAccount );
            this.panelControls.Controls.Add( this.lblStaticPatientName );
            this.panelControls.Controls.Add( this.lblMessage );
            this.panelControls.Location = new System.Drawing.Point( 8, 56 );
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size( 1008, 524 );
            this.panelControls.TabIndex = 1;
            // 
            // lblTime
            // 
            this.lblTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTime.Location = new System.Drawing.Point( 242, 406 );
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size( 56, 23 );
            this.lblTime.TabIndex = 0;
            // 
            // lblStaticTime
            // 
            this.lblStaticTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticTime.Location = new System.Drawing.Point( 208, 406 );
            this.lblStaticTime.Name = "lblStaticTime";
            this.lblStaticTime.Size = new System.Drawing.Size( 33, 23 );
            this.lblStaticTime.TabIndex = 0;
            this.lblStaticTime.Text = "Time:";
            // 
            // lblAdmitDate
            // 
            this.lblAdmitDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAdmitDate.Location = new System.Drawing.Point( 130, 406 );
            this.lblAdmitDate.Name = "lblAdmitDate";
            this.lblAdmitDate.Size = new System.Drawing.Size( 75, 23 );
            this.lblAdmitDate.TabIndex = 0;
            // 
            // lblStaticAdmitDate
            // 
            this.lblStaticAdmitDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticAdmitDate.Location = new System.Drawing.Point( 16, 406 );
            this.lblStaticAdmitDate.Name = "lblStaticAdmitDate";
            this.lblStaticAdmitDate.Size = new System.Drawing.Size( 118, 23 );
            this.lblStaticAdmitDate.TabIndex = 0;
            this.lblStaticAdmitDate.Text = "Scheduled admit date:";
            // 
            // lblContact1
            // 
            this.lblContact1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblContact1.Location = new System.Drawing.Point( 130, 380 );
            this.lblContact1.Name = "lblContact1";
            this.lblContact1.Size = new System.Drawing.Size( 300, 23 );
            this.lblContact1.TabIndex = 0;
            // 
            // lblStaticContact1
            // 
            this.lblStaticContact1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticContact1.Location = new System.Drawing.Point( 16, 380 );
            this.lblStaticContact1.Name = "lblStaticContact1";
            this.lblStaticContact1.Size = new System.Drawing.Size( 113, 23 );
            this.lblStaticContact1.TabIndex = 0;
            this.lblStaticContact1.Text = "Emergency contact 1:";
            // 
            // lblClinic5
            // 
            this.lblClinic5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClinic5.Location = new System.Drawing.Point( 130, 354 );
            this.lblClinic5.Name = "lblClinic5";
            this.lblClinic5.Size = new System.Drawing.Size( 300, 23 );
            this.lblClinic5.TabIndex = 0;
            // 
            // lblStaticClinic5
            // 
            this.lblStaticClinic5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic5.Location = new System.Drawing.Point( 16, 354 );
            this.lblStaticClinic5.Name = "lblStaticClinic5";
            this.lblStaticClinic5.Size = new System.Drawing.Size( 100, 23 );
            this.lblStaticClinic5.TabIndex = 0;
            this.lblStaticClinic5.Text = "Clinic 5:";
            // 
            // lblClinic4
            // 
            this.lblClinic4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClinic4.Location = new System.Drawing.Point( 130, 334 );
            this.lblClinic4.Name = "lblClinic4";
            this.lblClinic4.Size = new System.Drawing.Size( 300, 23 );
            this.lblClinic4.TabIndex = 0;
            // 
            // lblStaticClinic4
            // 
            this.lblStaticClinic4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic4.Location = new System.Drawing.Point( 16, 334 );
            this.lblStaticClinic4.Name = "lblStaticClinic4";
            this.lblStaticClinic4.Size = new System.Drawing.Size( 100, 23 );
            this.lblStaticClinic4.TabIndex = 0;
            this.lblStaticClinic4.Text = "Clinic 4:";
            // 
            // lblClinic3
            // 
            this.lblClinic3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClinic3.Location = new System.Drawing.Point( 130, 314 );
            this.lblClinic3.Name = "lblClinic3";
            this.lblClinic3.Size = new System.Drawing.Size( 300, 23 );
            this.lblClinic3.TabIndex = 0;
            // 
            // lblStaticClinic3
            // 
            this.lblStaticClinic3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic3.Location = new System.Drawing.Point( 16, 314 );
            this.lblStaticClinic3.Name = "lblStaticClinic3";
            this.lblStaticClinic3.Size = new System.Drawing.Size( 100, 23 );
            this.lblStaticClinic3.TabIndex = 0;
            this.lblStaticClinic3.Text = "Clinic 3:";
            // 
            // lblClinic2
            // 
            this.lblClinic2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblClinic2.Location = new System.Drawing.Point( 130, 294 );
            this.lblClinic2.Name = "lblClinic2";
            this.lblClinic2.Size = new System.Drawing.Size( 300, 23 );
            this.lblClinic2.TabIndex = 0;
            // 
            // lblStaticClinic2
            // 
            this.lblStaticClinic2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic2.Location = new System.Drawing.Point( 16, 294 );
            this.lblStaticClinic2.Name = "lblStaticClinic2";
            this.lblStaticClinic2.Size = new System.Drawing.Size( 100, 23 );
            this.lblStaticClinic2.TabIndex = 0;
            this.lblStaticClinic2.Text = "Clinic 2:";
            // 
            // lblClinic1
            // 
            this.lblClinic1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClinic1.Location = new System.Drawing.Point( 130, 274 );
            this.lblClinic1.Name = "lblClinic1";
            this.lblClinic1.Size = new System.Drawing.Size( 300, 23 );
            this.lblClinic1.TabIndex = 0;
            // 
            // lblStaticClinic1
            // 
            this.lblStaticClinic1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic1.Location = new System.Drawing.Point( 16, 274 );
            this.lblStaticClinic1.Name = "lblStaticClinic1";
            this.lblStaticClinic1.Size = new System.Drawing.Size( 100, 23 );
            this.lblStaticClinic1.TabIndex = 0;
            this.lblStaticClinic1.Text = "Clinic 1:";
            // 
            // lblAdmittingPhysician
            // 
            this.lblAdmittingPhysician.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAdmittingPhysician.Location = new System.Drawing.Point( 130, 248 );
            this.lblAdmittingPhysician.Name = "lblAdmittingPhysician";
            this.lblAdmittingPhysician.Size = new System.Drawing.Size( 300, 23 );
            this.lblAdmittingPhysician.TabIndex = 0;
            // 
            // lblStaticPhysician
            // 
            this.lblStaticPhysician.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticPhysician.Location = new System.Drawing.Point( 16, 248 );
            this.lblStaticPhysician.Name = "lblStaticPhysician";
            this.lblStaticPhysician.Size = new System.Drawing.Size( 105, 23 );
            this.lblStaticPhysician.TabIndex = 0;
            this.lblStaticPhysician.Text = "Admitting physician:";
            // 
            // txtMailingAddress
            // 
            this.txtMailingAddress.BackColor = System.Drawing.Color.White;
            this.txtMailingAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMailingAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtMailingAddress.Location = new System.Drawing.Point( 130, 171 );
            this.txtMailingAddress.Multiline = true;
            this.txtMailingAddress.Name = "txtMailingAddress";
            this.txtMailingAddress.ReadOnly = true;
            this.txtMailingAddress.Size = new System.Drawing.Size( 200, 55 );
            this.txtMailingAddress.TabIndex = 0;
            this.txtMailingAddress.TabStop = false;
            this.txtMailingAddress.TextChanged += new System.EventHandler( this.txtMailingAddress_TextChanged );
            // 
            // lblDOB
            // 
            this.lblDOB.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDOB.Location = new System.Drawing.Point( 130, 145 );
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size( 150, 23 );
            this.lblDOB.TabIndex = 0;
            // 
            // lblPatientType
            // 
            this.lblPatientType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPatientType.Location = new System.Drawing.Point( 130, 119 );
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size( 200, 23 );
            this.lblPatientType.TabIndex = 0;
            // 
            // lblAccount
            // 
            this.lblAccount.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAccount.Location = new System.Drawing.Point( 130, 93 );
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size( 200, 23 );
            this.lblAccount.TabIndex = 0;
            // 
            // lblPatientName
            // 
            this.lblPatientName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPatientName.Location = new System.Drawing.Point( 130, 66 );
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size( 500, 23 );
            this.lblPatientName.TabIndex = 0;
            // 
            // lblStaticAddress
            // 
            this.lblStaticAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticAddress.Location = new System.Drawing.Point( 16, 171 );
            this.lblStaticAddress.Name = "lblStaticAddress";
            this.lblStaticAddress.Size = new System.Drawing.Size( 90, 23 );
            this.lblStaticAddress.TabIndex = 0;
            this.lblStaticAddress.Text = "Mailing address:";
            // 
            // lblStaticDOB
            // 
            this.lblStaticDOB.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticDOB.Location = new System.Drawing.Point( 16, 145 );
            this.lblStaticDOB.Name = "lblStaticDOB";
            this.lblStaticDOB.Size = new System.Drawing.Size( 90, 23 );
            this.lblStaticDOB.TabIndex = 0;
            this.lblStaticDOB.Text = "DOB:";
            // 
            // lblStaticPatientType
            // 
            this.lblStaticPatientType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticPatientType.Location = new System.Drawing.Point( 16, 119 );
            this.lblStaticPatientType.Name = "lblStaticPatientType";
            this.lblStaticPatientType.Size = new System.Drawing.Size( 90, 23 );
            this.lblStaticPatientType.TabIndex = 0;
            this.lblStaticPatientType.Text = "Patient type:";
            // 
            // lblStaticAccount
            // 
            this.lblStaticAccount.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticAccount.Location = new System.Drawing.Point( 16, 93 );
            this.lblStaticAccount.Name = "lblStaticAccount";
            this.lblStaticAccount.Size = new System.Drawing.Size( 90, 23 );
            this.lblStaticAccount.TabIndex = 0;
            this.lblStaticAccount.Text = "Account:";
            // 
            // lblStaticPatientName
            // 
            this.lblStaticPatientName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticPatientName.Location = new System.Drawing.Point( 16, 66 );
            this.lblStaticPatientName.Name = "lblStaticPatientName";
            this.lblStaticPatientName.Size = new System.Drawing.Size( 90, 23 );
            this.lblStaticPatientName.TabIndex = 0;
            this.lblStaticPatientName.Text = "Patient name:";
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblMessage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMessage.Location = new System.Drawing.Point( 16, 16 );
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size( 975, 23 );
            this.lblMessage.TabIndex = 0;
            // 
            // panelButtons
            // 
            this.panelButtons.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.panelButtons.Controls.Add( this.btnOk );
            this.panelButtons.Controls.Add( this.btnCancel );
            this.panelButtons.Location = new System.Drawing.Point( 0, 585 );
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size( 1024, 35 );
            this.panelButtons.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnOk.BackColor = System.Drawing.SystemColors.Control;
            this.btnOk.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOk.Location = new System.Drawing.Point( 851, 6 );
            this.btnOk.Message = null;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size( 75, 23 );
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler( this.btnOk_Click );
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.Location = new System.Drawing.Point( 937, 6 );
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 23 );
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point( 3, 3 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size( 1000, 516 );
            this.progressPanel1.TabIndex = 1;
            this.progressPanel1.Visible = false;
            // 
            // CancelPreRegistrationView
            // 
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.Controls.Add( this.panelButtons );
            this.Controls.Add( this.panelControls );
            this.Controls.Add( this.panelPatientContext );
            this.Controls.Add( this.userContextView );
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "CancelPreRegistrationView";
            this.Size = new System.Drawing.Size( 1024, 620 );
            this.Load += new System.EventHandler( this.CancelPreRegistrationView_Load );
            this.panelPatientContext.ResumeLayout( false );
            this.panelControls.ResumeLayout( false );
            this.panelControls.PerformLayout();
            this.panelButtons.ResumeLayout( false );
            this.ResumeLayout( false );

		}
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CancelPreRegistrationView( PreRegistrationSearchView parent )
        {
            preRegistrationSearchView = parent;
            InitializeComponent();
			base.EnableThemesOn( this );
        }

        public CancelPreRegistrationView( WorklistsView parent )
        {
            worklistsView = parent;
            InitializeComponent();
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
            
            // cancel the background worker here...
            CancelBackgroundWorker();

            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container         components = null;

        private LoggingButton             btnOk;
        private LoggingButton             btnCancel;

        private Panel              panelPatientContext;
        private Panel              panelControls;
        private Panel              panelButtons;

        private Label              lblStaticPatientName;
        private Label              lblStaticAccount;
        private Label              lblStaticPatientType;
        private Label              lblStaticDOB;
        private Label              lblMessage;
        private Label              lblStaticAddress;
        private Label              lblPatientName;
        private Label              lblAccount;
        private Label              lblPatientType;
        private Label              lblDOB;
        private Label              lblStaticPhysician;
        private Label              lblAdmittingPhysician;
        private Label              lblStaticClinic1;
        private Label              lblClinic1;
        private Label              lblStaticClinic2;
        private Label              lblClinic2;
        private Label              lblStaticClinic3;
        private Label              lblClinic3;
        private Label              lblStaticClinic4;
        private Label              lblClinic4;
        private Label              lblStaticClinic5;
        private Label              lblClinic5;
        private Label              lblStaticContact1;
        private Label              lblContact1;
        private Label              lblStaticAdmitDate;
        private Label              lblAdmitDate;
        private Label              lblStaticTime;
        private Label              lblTime;

        private TextBox            txtMailingAddress;

        private UserContextView                userContextView;
        private PatientContextView             patientContextView;
        private PreRegistrationSearchView preRegistrationSearchView;
        private WorklistsView    worklistsView;
        private ProgressPanel progressPanel1;
        private IAccount                                        selectedAccount = null;
        private BackgroundWorker          backgroundWorker = null;

        #endregion

        #region Constants
        #endregion
    }
}
