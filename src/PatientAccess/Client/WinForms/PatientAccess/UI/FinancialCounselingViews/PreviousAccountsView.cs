using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.FinancialCounselingViews
{
	/// <summary>
	/// Summary description for PreviousAccountsView.
	/// </summary>
	//TODO: Create XML summary comment for PreviousAccountsView
	[Serializable]
	public class PreviousAccountsView : ControlView
	{
		#region Event Handlers
        public event EventHandler ChangeCursorToWaiting;
        public event EventHandler ChangeCursorToDefault;
		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
        public override void UpdateView()
        {
            this.i_OtherAccountsCallStarted = false;

            BreadCrumbLogger.GetInstance.Log( "Retrieving liability for acccounts with and without payment plans" );

            this.PopulateAccountsAsync();
        }

		/// <summary>
		/// UpdateModel with data entered in controls
		/// </summary>
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

		public decimal AccountsWithoutPlanTotalBalance
		{
			get
			{
				return i_AccountsWithoutPlanTotalBalance;
			}
			set
			{
				i_AccountsWithoutPlanTotalBalance = value;
			}
		}
		#endregion

		#region Private Methods

        private void BeforeWork()
        {
            this.lblMessage1.Font = new Font( "Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ( (Byte)( 0 ) ) );
            this.lblMessage2.Font = new Font( "Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ( (Byte)( 0 ) ) );

            this.lblMessage1.Text = UIErrorMessages.PATIENT_LIABILITY_RETRIEVING;
            this.lblMessage2.Text = UIErrorMessages.PATIENT_LIABILITY_RETRIEVING;

            this.pictureBox1.Visible = false;
            this.pictureBox2.Visible = false;
        }

        private void PopulateAccountsAsync()
        {
            accountsWithPlanTotalBalance = 0;
            acctsWithoutPlanTotalBalance = 0;

            this.lvPreviousAccountsWith.Items.Clear();
            this.lvPreviousAccountsWithout.Items.Clear();

            //            if( this.Model.Patient.AccountsWithPaymentPlan.Count == 0 )
            if( !FinancialCouncelingService.PriorAccountsRetrieved )  
            {
                this.ChangeCursorToWait();

                this.BeforeWork();

                if( this.backgroundWorker == null
                   ||
                   ( this.backgroundWorker != null
                   && !this.backgroundWorker.IsBusy )
                   )
                {
                    this.backgroundWorker = new BackgroundWorker();
                    this.backgroundWorker.WorkerSupportsCancellation = true;

                    backgroundWorker.DoWork +=
                        new DoWorkEventHandler( GetAccounts );
                    backgroundWorker.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler( AfterWork );

                    backgroundWorker.RunWorkerAsync();

                }
            }
            else
            {
                this.PopulateAccountsWithPlan();
                this.PopulateAccountsWithoutPlan();
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return ;

            if( e.Cancelled )
            {
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here

                // When the system communicates with ACE Prior Balance service through 
                // eDV, if an error occurs which would generate a connection related 
                // crash report then PAS should handle the error gracefully by:
                // a) Not displaying a crash report
                // b) Display the screen with same behavior as when connection cannot 
                //    be made to PBAR or SOS.

                this.DisplayLiabilityIncompleteErrorMessage();
            }
            else
            {
                // success
                try
                {
                    FinancialCouncelingService.PriorAccountsRetrieved = true;

                    ArrayList accountsWithPlan = (ArrayList)prevAccounts[0];
                    this.Model.Patient.AddAccountsWithPaymentPlan( accountsWithPlan );

                    ArrayList accountsWithoutPlan = (ArrayList)prevAccounts[1];
                    this.Model.Patient.AddAccountsWithNoPaymentPlan( accountsWithoutPlan );

                    this.lblMessage1.Text = string.Empty;
                    this.PopulateAccountsWithPlan();

                    this.lblMessage2.Text = string.Empty;
                    this.PopulateAccountsWithoutPlan();
                }
                catch( Exception )
                {
                    this.DisplayLiabilityIncompleteErrorMessage();
                }
            }

            this.BackCursorToDefault();
        }
        
        //private ArrayList GetAccounts( Account anAccount )
        private void GetAccounts( object sender, DoWorkEventArgs e )
        {
            Account anAccount = this.Model as Account;

            prevAccounts = new ArrayList();

            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            try
            {
                PriorAccountsRequest request = new PriorAccountsRequest();
                if( anAccount != null )
                {
                    // poll CancellationPending and set e.Cancel to true and return 
                    if( this.backgroundWorker.CancellationPending )
                    {
                        e.Cancel = true;
                        return;
                    }

                    anAccount.Activity.AppUser = User.GetCurrent();

                    request.AccountNumber = anAccount.AccountNumber;
                    request.Upn = anAccount.Activity.AppUser.SecurityUser.UPN;
                    request.FacilityOid = anAccount.Facility.Oid;
                    if( anAccount.Patient != null )
                    {
                        request.MedicalRecordNumber = anAccount.Patient.MedicalRecordNumber;
                    }
                    else
                    {
                        request.IsPatientNull = true;
                    }
                    prevAccounts = broker.GetPriorAccounts( request );
                }
                else
                {
                    // poll CancellationPending and set e.Cancel to true and return 
                    if( this.backgroundWorker.CancellationPending )
                    {
                        e.Cancel = true;
                        return;
                    }

                    request.IsAccountNull = true;
                    request.IsPatientNull = true;
                    prevAccounts = broker.GetPriorAccounts( request );
                }
            }
            catch( Exception )
            {
                throw;
            }


/*
            try
            {            
                PriorAccountsRequest request = new PriorAccountsRequest();
                if( anAccount !=  null )
                {
                    anAccount.Activity.AppUser = User.GetCurrent();

                    request.AccountNumber = anAccount.AccountNumber;
                    request.Upn = anAccount.Activity.AppUser.SecurityUser.UPN;
                    request.FacilityOid = anAccount.Facility.Oid;
                    if( anAccount.Patient != null )
                    {
                        request.MedicalRecordNumber = anAccount.Patient.MedicalRecordNumber;
                    }
                    else
                    {
                        request.IsPatientNull = true;
                    }
                    prevAccounts = broker.GetPriorAccounts( request  );
                }
                else
                {
                    request.IsAccountNull = true;
                    request.IsPatientNull = true;
                    prevAccounts = broker.GetPriorAccounts( request  );
                }
            }
            catch( Exception )
            {
                throw; 
            }
 */
        }

        private void DisplayLiabilityIncompleteErrorMessage()
        {
            this.lblMessage1.ForeColor = Color.Red;
            this.lblMessage2.ForeColor = Color.Red;

            this.lblMessage1.Font = new Font( "Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ( (Byte)( 0 ) ) );
            this.lblMessage2.Font = new Font( "Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ( (Byte)( 0 ) ) );

            this.pictureBox1.Visible = true;
            this.pictureBox2.Visible = true;

            this.lblMessage1.Text = UIErrorMessages.PATIENT_LIABILITY_INCOMPLETE;
            this.lblMessage2.Text = UIErrorMessages.PATIENT_LIABILITY_INCOMPLETE;
        }

		private void PopulateAccountsWithPlan()
		{

			foreach( AccountProxy aAccount in this.Model.Patient.AccountsWithPaymentPlan )
			{
                if( aAccount.AccountNumber != this.Model.AccountNumber  )
                {
                    ListViewItem item = new ListViewItem();

				    item.Tag = aAccount;

				    if( aAccount.DischargeDate == ( DateTime.MaxValue ) )
				    {
					    item.SubItems.Add( String.Empty );
				    }
				    else
				    {
					    item.SubItems.Add( aAccount.DischargeDate.ToString( "MM/dd/yyyy" ) );
				    }

				    item.SubItems.Add( aAccount.AccountNumber.ToString() );
				    item.SubItems.Add( aAccount.FinancialClass.Code + " " + aAccount.FinancialClass.Description );
				    //item.SubItems.Add( aAccount.FinancialClass.Description.ToString() );

				    if( aAccount.KindOfVisit == null )
				    {
					    item.SubItems.Add( String.Empty );
				    }
				    else
				    {
					    item.SubItems.Add( aAccount.KindOfVisit.Code + " " + aAccount.KindOfVisit.Description );
					    //item.SubItems.Add( aAccount.KindOfVisit.Description.ToString() );
				    }

				    item.SubItems.Add( aAccount.BalanceDue.ToString( "c", i_US ) );

				    lvPreviousAccountsWith.Items.Add( item );                                

				    accountsWithPlanTotalBalance += aAccount.BalanceDue;
                }
			}

			this.lblGroup1TotalAmt.Text = accountsWithPlanTotalBalance.ToString( "c", i_US );
			this.CheckForAccountsWithPlan();
		}

		private void PopulateAccountsWithoutPlan()
		{
			foreach( AccountProxy aAccount in this.Model.Patient.AccountsWithNoPaymentPlan )
			{
                if( aAccount.AccountNumber != this.Model.AccountNumber  )
                {				
                    ListViewItem item = new ListViewItem();

                    item.Tag = aAccount;

                    if( aAccount.DischargeDate == ( DateTime.MaxValue ) )
                    {
                        item.SubItems.Add( String.Empty );
                    }
                    else
                    {
                        item.SubItems.Add( aAccount.DischargeDate.ToString( "MM/dd/yyyy" ) );
                    }

                    item.SubItems.Add( aAccount.AccountNumber.ToString() );
                    item.SubItems.Add( aAccount.FinancialClass.Code + " " + aAccount.FinancialClass.Description );

                    if( aAccount.KindOfVisit == null )
                    {
                        item.SubItems.Add( String.Empty );
                    }
                    else
                    {
                        item.SubItems.Add( aAccount.KindOfVisit.Code + " " + aAccount.KindOfVisit.Description );
                    }

                    item.SubItems.Add( aAccount.BalanceDue.ToString( "c", i_US ) );

                    lvPreviousAccountsWithout.Items.Add( item );
                    				
                    acctsWithoutPlanTotalBalance += aAccount.BalanceDue;
                }
			}

			AccountsWithoutPlanTotalBalance = acctsWithoutPlanTotalBalance;
			this.lblGroup2TotalAmt.Text = acctsWithoutPlanTotalBalance.ToString( "c", i_US );
			this.CheckForAccountsWithoutPlan();
		}

		private void CheckForAccountsWithoutPlan()
		{

            BreadCrumbLogger.GetInstance.Log( "Finished retrieving liability for acccounts without payment plans" );

            if( this.Model.Patient.AccountsWithNoPaymentPlan.Count == 0 
                ||
                    (
                    this.Model.Patient.AccountsWithNoPaymentPlan.Count == 1
                    && ((AccountProxy)this.Model.Patient.AccountsWithNoPaymentPlan[0]).AccountNumber == this.Model.AccountNumber
                    ) 
              )
            {
                //TODO:why setting to true cause view blocking???
//                this.lblNoAccountsWithoutPlan.Visible = true;
                this.lblNoAccountsWithoutPlan.Size = new Size(266, 37);
                this.lblNoAccountsWithoutPlan.Text = "No accounts to display.";
            }
            else
            {
//                this.lblNoAccountsWithoutPlan.Visible = false;
                this.lblNoAccountsWithoutPlan.Size = new Size(0, 0);
                this.lblNoAccountsWithoutPlan.Text = "";
            }
            this.Refresh();
		}

		private void CheckForAccountsWithPlan()
		{
            BreadCrumbLogger.GetInstance.Log( "Finished retrieving liability for acccounts with payment plans" );

            if( this.Model.Patient.AccountsWithPaymentPlan.Count == 0                 
                ||
                    (
                    this.Model.Patient.AccountsWithNoPaymentPlan.Count == 1
                    && 
                    ((AccountProxy)this.Model.Patient.AccountsWithNoPaymentPlan[0]).AccountNumber == this.Model.AccountNumber 
                    ) 
              )
			{
//                this.lblNoAccountsWithPlan.Visible = true;
                this.lblNoAccountsWithPlan.Size = new Size(266, 37);
                this.lblNoAccountsWithPlan.Text = "No accounts to display.";
			}
			else
			{
//				this.lblNoAccountsWithPlan.Visible = false;
                this.lblNoAccountsWithPlan.Size = new Size(0, 0);
                this.lblNoAccountsWithPlan.Text = "";
			}
            this.Refresh();
		}

        private void ChangeCursorToWait()
        {      
            if( !this.i_OtherAccountsCallStarted )
            {
                this.Cursor = Cursors.AppStarting;

                this.ChangeCursorToWaiting( null, EventArgs.Empty );
                this.i_OtherAccountsCallStarted = true;
            }           
        }

        private void BackCursorToDefault()
        {
            this.Cursor = Cursors.Default;
            this.ChangeCursorToDefault( null, EventArgs.Empty );
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PreviousAccountsView));
            this.lvPreviousAccountsWith = new System.Windows.Forms.ListView();
            this.ch1 = new System.Windows.Forms.ColumnHeader();
            this.chDischDateG1 = new System.Windows.Forms.ColumnHeader();
            this.chAccountG1 = new System.Windows.Forms.ColumnHeader();
            this.chFinancialClassG1 = new System.Windows.Forms.ColumnHeader();
            this.chPatientTypeG1 = new System.Windows.Forms.ColumnHeader();
            this.chBalanceDueG1 = new System.Windows.Forms.ColumnHeader();
            this.lvPreviousAccountsWithout = new System.Windows.Forms.ListView();
            this.ch2 = new System.Windows.Forms.ColumnHeader();
            this.chDischDateG2 = new System.Windows.Forms.ColumnHeader();
            this.chAccountG2 = new System.Windows.Forms.ColumnHeader();
            this.chFinancialClassG2 = new System.Windows.Forms.ColumnHeader();
            this.chPatientTypeG2 = new System.Windows.Forms.ColumnHeader();
            this.chBalanceDueG2 = new System.Windows.Forms.ColumnHeader();
            this.gbAccountsWith = new System.Windows.Forms.GroupBox();
            this.lblGroup1TotalAmt = new System.Windows.Forms.Label();
            this.lblNoAccountsWithPlan = new System.Windows.Forms.Label();
            this.lblTotalGroup1 = new System.Windows.Forms.Label();
            this.lblMessage1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gbAccountswithout = new System.Windows.Forms.GroupBox();
            this.lblGroup2TotalAmt = new System.Windows.Forms.Label();
            this.lblNoAccountsWithoutPlan = new System.Windows.Forms.Label();
            this.lblTotalGroup2 = new System.Windows.Forms.Label();
            this.lblMessage2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.gbAccountsWith.SuspendLayout();
            this.gbAccountswithout.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvPreviousAccountsWith
            // 
            this.lvPreviousAccountsWith.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                                     this.ch1,
                                                                                                     this.chDischDateG1,
                                                                                                     this.chAccountG1,
                                                                                                     this.chFinancialClassG1,
                                                                                                     this.chPatientTypeG1,
                                                                                                     this.chBalanceDueG1});
            this.lvPreviousAccountsWith.FullRowSelect = true;
            this.lvPreviousAccountsWith.Location = new System.Drawing.Point(10, 17);
            this.lvPreviousAccountsWith.Name = "lvPreviousAccountsWith";
            this.lvPreviousAccountsWith.Size = new System.Drawing.Size(580, 114);
            this.lvPreviousAccountsWith.TabIndex = 1;
            this.lvPreviousAccountsWith.View = System.Windows.Forms.View.Details;
            // 
            // ch1
            // 
            this.ch1.Text = "";
            this.ch1.Width = 0;
            // 
            // chDischDateG1
            // 
            this.chDischDateG1.Text = "Disch Date";
            this.chDischDateG1.Width = 71;
            // 
            // chAccountG1
            // 
            this.chAccountG1.Text = "Account";
            this.chAccountG1.Width = 67;
            // 
            // chFinancialClassG1
            // 
            this.chFinancialClassG1.Text = "Financial Class";
            this.chFinancialClassG1.Width = 198;
            // 
            // chPatientTypeG1
            // 
            this.chPatientTypeG1.Text = "Patient Type";
            this.chPatientTypeG1.Width = 124;
            // 
            // chBalanceDueG1
            // 
            this.chBalanceDueG1.Text = "Balance Due            ";
            this.chBalanceDueG1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chBalanceDueG1.Width = 116;
            // 
            // lvPreviousAccountsWithout
            // 
            this.lvPreviousAccountsWithout.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                                        this.ch2,
                                                                                                        this.chDischDateG2,
                                                                                                        this.chAccountG2,
                                                                                                        this.chFinancialClassG2,
                                                                                                        this.chPatientTypeG2,
                                                                                                        this.chBalanceDueG2});
            this.lvPreviousAccountsWithout.FullRowSelect = true;
            this.lvPreviousAccountsWithout.Location = new System.Drawing.Point(10, 17);
            this.lvPreviousAccountsWithout.Name = "lvPreviousAccountsWithout";
            this.lvPreviousAccountsWithout.Size = new System.Drawing.Size(580, 114);
            this.lvPreviousAccountsWithout.TabIndex = 1;
            this.lvPreviousAccountsWithout.View = System.Windows.Forms.View.Details;
            // 
            // ch2
            // 
            this.ch2.Text = "";
            this.ch2.Width = 0;
            // 
            // chDischDateG2
            // 
            this.chDischDateG2.Text = "Disch Date";
            this.chDischDateG2.Width = 71;
            // 
            // chAccountG2
            // 
            this.chAccountG2.Text = "Account";
            this.chAccountG2.Width = 67;
            // 
            // chFinancialClassG2
            // 
            this.chFinancialClassG2.Text = "Financial Class";
            this.chFinancialClassG2.Width = 198;
            // 
            // chPatientTypeG2
            // 
            this.chPatientTypeG2.Text = "Patient Type";
            this.chPatientTypeG2.Width = 124;
            // 
            // chBalanceDueG2
            // 
            this.chBalanceDueG2.Text = "Balance Due            ";
            this.chBalanceDueG2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chBalanceDueG2.Width = 116;
            // 
            // gbAccountsWith
            // 
            this.gbAccountsWith.Controls.Add(this.lblGroup1TotalAmt);
            this.gbAccountsWith.Controls.Add(this.lblNoAccountsWithPlan);
            this.gbAccountsWith.Controls.Add(this.lblTotalGroup1);
            this.gbAccountsWith.Controls.Add(this.lblMessage1);
            this.gbAccountsWith.Controls.Add(this.pictureBox1);
            this.gbAccountsWith.Controls.Add(this.lvPreviousAccountsWith);
            this.gbAccountsWith.Location = new System.Drawing.Point(2, 8);
            this.gbAccountsWith.Name = "gbAccountsWith";
            this.gbAccountsWith.Size = new System.Drawing.Size(599, 161);
            this.gbAccountsWith.TabIndex = 0;
            this.gbAccountsWith.TabStop = false;
            this.gbAccountsWith.Text = "Other accounts with payment plan (group 1)";
            // 
            // lblGroup1TotalAmt
            // 
            this.lblGroup1TotalAmt.Location = new System.Drawing.Point(491, 137);
            this.lblGroup1TotalAmt.Name = "lblGroup1TotalAmt";
            this.lblGroup1TotalAmt.Size = new System.Drawing.Size(91, 13);
            this.lblGroup1TotalAmt.TabIndex = 0;
            this.lblGroup1TotalAmt.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblNoAccountsWithPlan
            // 
            this.lblNoAccountsWithPlan.Location = new System.Drawing.Point(120, 57);
            this.lblNoAccountsWithPlan.Name = "lblNoAccountsWithPlan";
            this.lblNoAccountsWithPlan.Size = new System.Drawing.Size(266, 37);
            this.lblNoAccountsWithPlan.TabIndex = 0;
            this.lblNoAccountsWithPlan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalGroup1
            // 
            this.lblTotalGroup1.Location = new System.Drawing.Point(457, 137);
            this.lblTotalGroup1.Name = "lblTotalGroup1";
            this.lblTotalGroup1.Size = new System.Drawing.Size(35, 15);
            this.lblTotalGroup1.TabIndex = 0;
            this.lblTotalGroup1.Text = "Total:";
            // 
            // lblMessage1
            // 
            this.lblMessage1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((System.Byte)(0)));
            this.lblMessage1.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(51)), ((System.Byte)(0)));
            this.lblMessage1.Location = new System.Drawing.Point(32, 137);
            this.lblMessage1.Name = "lblMessage1";
            this.lblMessage1.Size = new System.Drawing.Size(419, 15);
            this.lblMessage1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(10, 135);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(18, 18);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // gbAccountswithout
            // 
            this.gbAccountswithout.Controls.Add(this.lblGroup2TotalAmt);
            this.gbAccountswithout.Controls.Add(this.lblNoAccountsWithoutPlan);
            this.gbAccountswithout.Controls.Add(this.lblTotalGroup2);
            this.gbAccountswithout.Controls.Add(this.lblMessage2);
            this.gbAccountswithout.Controls.Add(this.pictureBox2);
            this.gbAccountswithout.Controls.Add(this.lvPreviousAccountsWithout);
            this.gbAccountswithout.Location = new System.Drawing.Point(2, 183);
            this.gbAccountswithout.Name = "gbAccountswithout";
            this.gbAccountswithout.Size = new System.Drawing.Size(599, 156);
            this.gbAccountswithout.TabIndex = 1;
            this.gbAccountswithout.TabStop = false;
            this.gbAccountswithout.Text = "Other accounts without payment plan (group 2)";
            // 
            // lblGroup2TotalAmt
            // 
            this.lblGroup2TotalAmt.Location = new System.Drawing.Point(491, 136);
            this.lblGroup2TotalAmt.Name = "lblGroup2TotalAmt";
            this.lblGroup2TotalAmt.Size = new System.Drawing.Size(91, 13);
            this.lblGroup2TotalAmt.TabIndex = 0;
            this.lblGroup2TotalAmt.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblNoAccountsWithoutPlan
            // 
            this.lblNoAccountsWithoutPlan.Location = new System.Drawing.Point(120, 57);
            this.lblNoAccountsWithoutPlan.Name = "lblNoAccountsWithoutPlan";
            this.lblNoAccountsWithoutPlan.Size = new System.Drawing.Size(266, 37);
            this.lblNoAccountsWithoutPlan.TabIndex = 0;
            this.lblNoAccountsWithoutPlan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalGroup2
            // 
            this.lblTotalGroup2.Location = new System.Drawing.Point(457, 136);
            this.lblTotalGroup2.Name = "lblTotalGroup2";
            this.lblTotalGroup2.Size = new System.Drawing.Size(35, 15);
            this.lblTotalGroup2.TabIndex = 0;
            this.lblTotalGroup2.Text = "Total:";
            // 
            // lblMessage2
            // 
            this.lblMessage2.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((System.Byte)(0)));
            this.lblMessage2.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(51)), ((System.Byte)(0)));
            this.lblMessage2.Location = new System.Drawing.Point(32, 136);
            this.lblMessage2.Name = "lblMessage2";
            this.lblMessage2.Size = new System.Drawing.Size(420, 14);
            this.lblMessage2.TabIndex = 0;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(10, 134);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(18, 18);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // PreviousAccountsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.gbAccountswithout);
            this.Controls.Add(this.gbAccountsWith);
            this.Name = "PreviousAccountsView";
            this.Size = new System.Drawing.Size(604, 348);
            this.gbAccountsWith.ResumeLayout(false);
            this.gbAccountswithout.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public PreviousAccountsView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
//			this.lblMessage1.ForeColor = WARNING_MSG_COLOR;
//			this.lblMessage2.ForeColor = WARNING_MSG_COLOR;
			base.EnableThemesOn( this );

			//CodeReview: Modify view to display only 1 list view so it can be reused.
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
                CancelBackgroundWorker();
            }
			base.Dispose( disposing );
		}

        private void CancelBackgroundWorker()
        {
            // cancel the background worker here...
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.CancelAsync();
            }
        }
		#endregion

		#region Data Elements
		private Container         components  = null;
        private BackgroundWorker  backgroundWorker = new BackgroundWorker();

		private GroupBox           gbAccountsWith;
		private GroupBox           gbAccountswithout;
        private ArrayList                               prevAccounts = null;

		private PictureBox         pictureBox1;
		private PictureBox         pictureBox2;

		private Label              lblMessage1;
		private Label              lblMessage2;
		private Label              lblTotalGroup1;
		private Label              lblTotalGroup2;

		private ColumnHeader       chDischDateG1;
		private ColumnHeader       chAccountG1;
		private ColumnHeader       chFinancialClassG1;
		private ColumnHeader       chPatientTypeG1;
		private ColumnHeader       chBalanceDueG1;
		private ColumnHeader       chDischDateG2;
		private ColumnHeader       chAccountG2;
		private ColumnHeader       chFinancialClassG2;
		private ColumnHeader       chPatientTypeG2;

        private ColumnHeader       ch2;
        private ColumnHeader       ch1;
        private ColumnHeader       chBalanceDueG2;

		private ListView           lvPreviousAccountsWith;
		private ListView           lvPreviousAccountsWithout;


		private CultureInfo                             i_US = new CultureInfo("en-US");
		private decimal                                 i_AccountsWithoutPlanTotalBalance;

		private Label              lblNoAccountsWithPlan;
		private Label              lblGroup1TotalAmt;
		private Label              lblGroup2TotalAmt;
		private Label              lblNoAccountsWithoutPlan;
		
        private bool                                    i_OtherAccountsCallStarted = false;

        private decimal                                 acctsWithoutPlanTotalBalance;
        private decimal                                 accountsWithPlanTotalBalance;

        #endregion

		#region Constants
		//private const string NO_ACCOUNTS_ERRMSG = "No accounts to display.";
		#endregion
	}
}
