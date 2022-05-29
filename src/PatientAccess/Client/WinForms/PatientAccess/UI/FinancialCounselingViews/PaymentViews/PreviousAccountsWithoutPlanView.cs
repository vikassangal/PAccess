using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
	/// <summary>
	/// Summary description for PreviousAccountsWithoutPlanView.
	/// </summary>
	[Serializable]
	public class PreviousAccountsWithoutPlanView : ControlView
	{
        #region Events
        public event EventHandler ChangeCursorToDefault;
        #endregion		
                
        #region Event Handlers
		private void btnRecordPayment_Click(object sender, EventArgs e)
		{
			if( lvPreviousAccounts.SelectedItems.Count < 1 )
			{
				return;
			}
			else
			{
				this.RecordPayment();
			}
		}

		private void lvPreviousAccounts_DoubleClick(object sender, EventArgs e)
		{
			this.RecordPayment();
		}

		private void lvPreviousAccounts_KeyDown(object sender, KeyEventArgs e)
		{
			if( e.KeyCode == Keys.Enter && lvPreviousAccounts.SelectedItems.Count > 0 )
			{
				this.RecordPayment();
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
        public override void UpdateView()
        {
            this.PopulateAccountsAsync();
        }

		/// <summary>
		/// UpdateModel method
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
		#endregion

		#region Private Methods
        
		private void RecordPayment()
		{
			if( IsAccountSelected() )
			{
				this.Cursor = Cursors.WaitCursor;

				Account selectedAccount;
				Type accountType = this.lvPreviousAccounts.SelectedItems[0].Tag.GetType();
				if( accountType.Name.Equals( "AccountProxy" ) ) 
				{
					AccountProxy aProxy = (AccountProxy)this.lvPreviousAccounts.SelectedItems[0].Tag;

                    // OTD# 37210 fix - Do not assign new Patient() to Proxy.Patient since it sets the MRN to zero
                    // and throws an 'AccountNotFoundException'. Assign the Patient object from the Model instead.
                    aProxy.Patient = this.Model.Patient;
                    aProxy.Facility = this.Model.Facility;
                    selectedAccount = aProxy.AsAccount();
				}
				else
				{
					selectedAccount = (Account)this.lvPreviousAccounts.SelectedItems[0].Tag;
				}
				
				this.Cursor = Cursors.Default;

				if( selectedAccount != null )
				{
					//Open RecordPayment form
					FormViewRecordPayment formViewRecordPayment = new FormViewRecordPayment();
					if( this.Model.Payment == null )
					{
						this.Model.Payment = new Payment();
					}
					formViewRecordPayment.TotalCurrentAmountDue = selectedAccount.TotalCurrentAmtDue;
					formViewRecordPayment.BalanceDue = selectedAccount.BalanceDue;

					PaymentService paymentService = new PaymentService();
					paymentService.FormViewRecordPayment = formViewRecordPayment;
					formViewRecordPayment.SetPaymentService( paymentService );

					formViewRecordPayment.Model = selectedAccount.Payment;
					formViewRecordPayment.IsCurrentAccount = false;
					formViewRecordPayment.UpdateView();

					try
					{
						formViewRecordPayment.ShowDialog( this );
						if( formViewRecordPayment.DialogResult == DialogResult.OK )
						{
							IList acountsWithoutPlan = this.Model.Patient.AccountsWithNoPaymentPlan;
							Payment payment = new Payment();
							payment = formViewRecordPayment.Model;
							this.lvPreviousAccounts.SelectedItems[0].SubItems[3].Text = payment.TotalRecordedPayment.ToString( "c", i_US );
							selectedAccount.Payment = payment;
							this.lvPreviousAccounts.SelectedItems[0].Tag = selectedAccount;


							/***************************/
							bool accountFound = false;
							foreach( Account account in this.Model.Patient.AccountsWithNoPaymentPlanWithPayments )
							{
								if( selectedAccount.AccountNumber == account.AccountNumber )
								{
									accountFound = true;
									account.Payment = payment;
									break;
								}
							}

							if( !accountFound )
							{
								this.Model.Patient.AddAccountWithNoPaymentPlanWithPayments( selectedAccount );
							}
							/***************************/


							//						foreach( Account account in acountsWithoutPlan )
							//						{
							//							if( selectedAccount.AccountNumber == account.AccountNumber )
							//							{
							//								account.Payment = payment;
							//								break;
							//							}
							//						}

							this.Refresh();
						}
					}
					finally
					{
						formViewRecordPayment.Dispose();
					}
				}
			}   
		}

        private void GetAccounts( object sender, DoWorkEventArgs e )
        {
            Account anAccount = this.Model as Account;

            al = new ArrayList();

            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            try
            {
                PriorAccountsRequest request = new PriorAccountsRequest();
                if (anAccount != null)
                {

                    // poll CancellationPending and set e.Cancel to true and return 
                    if (this.backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    anAccount.Activity.AppUser = User.GetCurrent();

                    request.AccountNumber = anAccount.AccountNumber;
                    request.Upn = anAccount.Activity.AppUser.SecurityUser.UPN;
                    request.FacilityOid = anAccount.Facility.Oid;
                    if (anAccount.Patient != null)
                    {
                        request.MedicalRecordNumber = anAccount.Patient.MedicalRecordNumber;
                    }
                    else
                    {
                        request.IsPatientNull = true;
                    }
                    al = broker.GetPriorAccounts(request);
                }
                else
                {

                    // poll CancellationPending and set e.Cancel to true and return 
                    if (this.backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    request.IsAccountNull = true;
                    request.IsPatientNull = true;
                    al = broker.GetPriorAccounts(request);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return ;

            if ( e.Cancelled )
            {
            }
            else if ( e.Error != null )
            {
                // When the system communicates with ACE Prior Balance service through  
                // eDV, if an error occurs which would generate a connection related 
                // crash report then PAS should handle the error gracefully by:
                // a) Not displaying a crash report
                // b) Display the screen with same behavior as when connection 
                //    cannot be made to PBAR or SOS.
            }
            else
            {
                FinancialCouncelingService.PriorAccountsRetrieved = true;

                ArrayList accountsWithPlan = (ArrayList)al[0];
                this.Model.Patient.AddAccountsWithPaymentPlan(accountsWithPlan);

                ArrayList accountsWithoutPlan = (ArrayList)al[1];
                this.Model.Patient.AddAccountsWithNoPaymentPlan(accountsWithoutPlan);

                this.PopulateAccountsWithoutPlan();
            }

            this.BackCursorToDefault();
        }

        private void BackCursorToDefault()
        {
            this.Cursor = Cursors.Default;
            this.ChangeCursorToDefault( null, EventArgs.Empty );
        }

        private void BeforeWork()
        {
        }

        private void PopulateAccountsAsync()
        {
            if( !FinancialCouncelingService.PriorAccountsRetrieved )                   
            {
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
                this.PopulateAccountsWithoutPlan();
            }            
        }

        /// <summary>
        /// This method needs documenting
        /// </summary>
        private void PopulateAccountsWithoutPlan()
        {
            decimal paymentCollected = 0;

            this.lvPreviousAccounts.Items.Clear();

            foreach( AccountProxy aAccount in this.Model.Patient.AccountsWithNoPaymentPlan )
            {
                if( aAccount.AccountNumber != this.Model.AccountNumber  )
                {	
                    ListViewItem item = new ListViewItem();

                    item.Tag = aAccount;

                    item.SubItems.Add( aAccount.AccountNumber.ToString() );
                    item.SubItems.Add( aAccount.BalanceDue.ToString( "c", i_US ) );
                    item.SubItems.Add( paymentCollected.ToString( "c", i_US ) );

                    lvPreviousAccounts.Items.Add( item );
                }
            }

            if( lvPreviousAccounts.Items.Count > 0 )
            {
                this.lvPreviousAccounts.Items[0].Selected = true;
                this.btnRecordPayment.Enabled = true;
            }
            else
            {
                this.btnRecordPayment.Enabled = false;
            }
        }

		private bool IsAccountSelected()
		{
			foreach( ListViewItem item in lvPreviousAccounts.Items )
			{
				if( item.Selected )
				{
					return true;
				}
			}
			return false;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRecordPayment = new LoggingButton();
            this.lvPreviousAccounts = new System.Windows.Forms.ListView();
            this.ch1 = new System.Windows.Forms.ColumnHeader();
            this.chAccount = new System.Windows.Forms.ColumnHeader();
            this.chBalanceDue = new System.Windows.Forms.ColumnHeader();
            this.chPaymentCollected = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.btnRecordPayment);
            this.groupBox1.Controls.Add(this.lvPreviousAccounts);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(7, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(365, 248);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Payment collected for other accounts without payment plan";
            // 
            // btnRecordPayment
            // 
            this.btnRecordPayment.Enabled = false;
            this.btnRecordPayment.Location = new System.Drawing.Point(241, 213);
            this.btnRecordPayment.Name = "btnRecordPayment";
            this.btnRecordPayment.Size = new System.Drawing.Size(110, 23);
            this.btnRecordPayment.TabIndex = 2;
            this.btnRecordPayment.Text = "Record Payment...";
            this.btnRecordPayment.Click += new System.EventHandler(this.btnRecordPayment_Click);
            // 
            // lvPreviousAccounts
            // 
            this.lvPreviousAccounts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                                 this.ch1,
                                                                                                 this.chAccount,
                                                                                                 this.chBalanceDue,
                                                                                                 this.chPaymentCollected});
            this.lvPreviousAccounts.FullRowSelect = true;
            this.lvPreviousAccounts.HideSelection = false;
            this.lvPreviousAccounts.Location = new System.Drawing.Point(10, 83);
            this.lvPreviousAccounts.MultiSelect = false;
            this.lvPreviousAccounts.Name = "lvPreviousAccounts";
            this.lvPreviousAccounts.Size = new System.Drawing.Size(341, 124);
            this.lvPreviousAccounts.TabIndex = 1;
            this.lvPreviousAccounts.View = System.Windows.Forms.View.Details;
            this.lvPreviousAccounts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvPreviousAccounts_KeyDown);
            this.lvPreviousAccounts.DoubleClick += new System.EventHandler(this.lvPreviousAccounts_DoubleClick);
            // 
            // ch1
            // 
            this.ch1.Text = "";
            this.ch1.Width = 0;
            // 
            // chAccount
            // 
            this.chAccount.Text = "Account";
            this.chAccount.Width = 75;
            // 
            // chBalanceDue
            // 
            this.chBalanceDue.Text = "Balance Due             ";
            this.chBalanceDue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chBalanceDue.Width = 120;
            // 
            // chPaymentCollected
            // 
            this.chPaymentCollected.Text = "Payment Collected           ";
            this.chPaymentCollected.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chPaymentCollected.Width = 142;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(353, 54);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a row and click Record Payment to record a payment.  Each payment will be " +
                "recorded to a system note only and will not appear on the Payment screen after c" +
                "ompletion of this registration or maintenance activity.";
            // 
            // PreviousAccountsWithoutPlanView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBox1);
            this.Name = "PreviousAccountsWithoutPlanView";
            this.Size = new System.Drawing.Size(379, 255);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        #endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public PreviousAccountsWithoutPlanView()
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
		private Container         components = null;
        private BackgroundWorker  backgroundWorker = new BackgroundWorker();
		private GroupBox           groupBox1;

		private Label              label1;

		private ListView           lvPreviousAccounts;
        private ArrayList                               al = null;
        private LoggingButton                           btnRecordPayment;

		private ColumnHeader       chAccount;
		private ColumnHeader       chBalanceDue;		
		private ColumnHeader       chPaymentCollected;
		private ColumnHeader       ch1;

		private CultureInfo                             i_US = new CultureInfo("en-US");

		#endregion

		#region Constants
		#endregion
	}
}
