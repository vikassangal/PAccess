using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls; 
using PatientAccess.UI.FinancialCounselingViews.Presenters;
using PatientAccess.UI.HelperClasses;
 
namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
	/// <summary>
	/// Summary description for PaymentCollectedView.
	/// </summary>
	[Serializable]
    public class PaymentCollectedView : ControlView, IPaymentCollectedView
	{
		#region Event Handlers
        private void PaymentCollectedView_Load( object sender, EventArgs e )
        {
            if( !this.DesignMode )
            {
                this.PaymentService  = new PaymentService();
                this.CanOverrideMonthlyPayment = this.PaymentService.CanOverrideMonthlyPayment();
                this.RegisterRulesEvents();
            }
        }

		private void btnRecordPayment_Click(object sender, EventArgs e)
		{
			this.RecordPayment();
		}

        private void btnOverride_Click(object sender, EventArgs e)
        {
            this.Override();
        }

        private void cmbReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Model.Payment.ZeroPaymentReason.Description = this.cmbReason.SelectedItem.ToString().Trim();

        }

		private void cmbNumberOfMonths_SelectedIndexChanged(object sender, EventArgs e)
		{
			decimal monthlyPayment = 0m;
			this.Model.NumberOfMonthlyPayments = Convert.ToInt16( this.cmbNumberOfMonths.SelectedItem.ToString() );

			if( this.Model.NumberOfMonthlyPayments == 0 )
			{
				monthlyPayment = 0;
			}
			else
			{
                monthlyPayment = this.Model.BalanceDue / this.Model.NumberOfMonthlyPayments;
			}

			this.lblMonthlyPayment.Text = monthlyPayment.ToString( "n", i_NumberFormat );
            PaymentCollectedViewPresenter.EvaluateMonthlyDueDate();
		}

        private void mtbTotalPaymentsCollected_Enter(object sender, EventArgs e)
        {
            decimal totalPaymentCollected = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbTotalPaymentsCollected );	
            if( totalPaymentCollected == 0 )
            {
                this.mtbTotalPaymentsCollected.UnMaskedText = "0.00";
            }
            else
            {
                this.mtbTotalPaymentsCollected.UnMaskedText = totalPaymentCollected.ToString();
            }        
        }

        private void mtbTotalPaymentsCollected_Validating(object sender, CancelEventArgs e)
        {
            CommonFormatting.FormatTextBoxCurrency( mtbTotalPaymentsCollected, "###,###,##0.00" );
            
            // call operation asynchronously
            try
            {
                TotalPaymentsCollectedLeaveDelegate d = new TotalPaymentsCollectedLeaveDelegate ( TotalPaymentsCollectedLeaveAsync );
                this.mtbTotalPaymentsCollected.BeginInvoke ( d );                  
            }
            catch
            {
                // intentionally left blank - we get exception when async call for 
                // previous account hasn't returned back with results yet and we already jumped to another activity 
            }
        }

		private void mtbTotalPaymentsCollected_TextChanged(object sender, EventArgs e)
		{
            decimal totalPaymentCollected = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbTotalPaymentsCollected );	
            if( totalPaymentCollected == 0 )
            {
                this.cmbReason.Enabled = true;
            }
            else
            {
                this.cmbReason.SelectedIndex = 0;
                this.cmbReason.Enabled = false;
            }

            this.UpdateCollectionValues();
            PaymentCollectedViewPresenter.EvaluateMonthlyDueDate();
		}

		private void lblNetBalance_TextChanged(object sender, EventArgs e)
		{
            if( this.PaymentService != null )
            {
                this.NumberOfMonths = this.PaymentService.NumberOfMonthsToDisplay( this.Model.BalanceDue, this.CanOverrideMonthlyPayment );
            }
            this.PopulateNumberOfMonthlyPayments( this.Model.NumberOfMonthlyPayments );
            
            if( Convert.ToDecimal( lblNetBalance.Text ) <= 0 )
            {
                this.btnOverride.Enabled = false;
                this.cmbNumberOfMonths.Enabled = false;
            }
            else
            {   //if user already has override MonthlyPayment privilege then he/she doesn't need Override button to be Enabled
                if( this.CanOverrideMonthlyPayment )
                {
                    this.btnOverride.Enabled = false;
                }
                else
                {
                    this.btnOverride.Enabled = true;                                
                }
            }
            if( Convert.ToDecimal( lblNetBalance.Text ) < 0 )
            {
                this.lblNetBalance.ForeColor = Color.Red;
                this.lblMonthlyPayment.ForeColor = Color.Red;
            }
            else
            {
                this.lblNetBalance.ForeColor = SystemColors.WindowText;
                this.lblMonthlyPayment.ForeColor = SystemColors.WindowText;
            
            }
            PaymentCollectedViewPresenter.EvaluateMonthlyDueDate();
		}
		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
		public override void UpdateView()
		{
            PaymentCollectedViewPresenter = new PaymentCollectedViewPresenter(this, new MessageBoxAdapter());
            decimal	netBalance;
            if( this.Model.Payment != null )
            {
                this.lblTodaysPayment.Text = this.Model.Payment.TotalRecordedPayment.ToString( "n", i_NumberFormat );
            }
            else
            {
                this.lblTodaysPayment.Text = "0.00";
            }
		
            this.mtbTotalPaymentsCollected.Text = this.Model.TotalPaid.ToString( "###,###,##0.00" );
            netBalance = ( this.Model.TotalCurrentAmtDue - this.Model.TotalPaid );
            this.Model.BalanceDue = this.Model.TotalCurrentAmtDue - this.Model.TotalPaid;
            this.lblNetBalance.Text = netBalance.ToString( "###,###,##0.00" );
            PaymentCollectedViewPresenter.UpdateMonthlyDueDate();
		 
            this.RunRules();
		}

		/// <summary>
		/// UpdateModel method
		/// </summary>
		public override void UpdateModel()
		{   
			
		}
      
		#endregion

		#region Properties
        private IPaymentCollectedViewPresenter PaymentCollectedViewPresenter
        {
            get
            {
                return paymentCollectedViewPresenter;
            }

            set
            {
                paymentCollectedViewPresenter = value;
            }
        }
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

	    public void PopulateMonthlyDueDate()
	    {
            mtbMonthlyDueDate.Text = Model.DayOfMonthPaymentDue;
	    }

	    public void ShowMonthlyDueDate()
	    {
	        lblMonthlyDueDate.Visible = true;
	        mtbMonthlyDueDate.Visible = true;
	    }

	    public void DoNotShowMonthlyDueDate()
	    {
            lblMonthlyDueDate.Visible = false;
            mtbMonthlyDueDate.Visible = false;
	    }

	    public void MakeMonthlyDueDateRequired()
	    {
            UIColors.SetRequiredBgColor(mtbMonthlyDueDate);
	    }
          public void MakeMonthlyDueDateNormal()
	    {
            UIColors.SetNormalBgColor(mtbMonthlyDueDate);
	    }
         
	    public PaymentService PaymentService
        {
            private get
            {
                return i_PaymentService;
            }
            set
            {
                i_PaymentService = value;
            }
        }

 		#endregion

		#region Private Methods
        private void UpdateCollectionValues()
        {
            decimal totalPaymentCollected = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbTotalPaymentsCollected );	

            this.Model.BalanceDue = this.Model.TotalCurrentAmtDue - totalPaymentCollected;
            this.lblNetBalance.Text = this.Model.BalanceDue.ToString( "n", i_NumberFormat );
            this.Model.TotalPaid = totalPaymentCollected;        
        }

        private void TotalPaymentsCollectedLeaveAsync ()
        {
            this.RunRules();
        }

		private decimal TotalPaymentsCollected( string totalPaymentsCollected )
		{
			decimal paymentsCollected = 0m;
			if( totalPaymentsCollected != string.Empty )
			{
				paymentsCollected = Convert.ToDecimal( totalPaymentsCollected );
			}

			return paymentsCollected;
		}

		private void RecordPayment()
		{
			FormViewRecordPayment formViewRecordPayment = new FormViewRecordPayment();
            this.PaymentService.FormViewRecordPayment = formViewRecordPayment;
            formViewRecordPayment.SetPaymentService( this.PaymentService );

			if( this.Model.Payment == null )
			{
				this.Model.Payment = new Payment();
			}

            decimal prevTotalRecordedPayment = this.Model.Payment.TotalRecordedPayment;  //TG - save previous payment

			formViewRecordPayment.TotalCurrentAmountDue = this.Model.TotalCurrentAmtDue;
			//formViewRecordPayment.BalanceDue = this.Model.BalanceDue;
			formViewRecordPayment.IsCurrentAccount = true;
			formViewRecordPayment.TotalPaymentsForAccount = this.Model.TotalPaid;
			formViewRecordPayment.Model = this.Model.Payment;

			formViewRecordPayment.UpdateView();

			try
			{
				formViewRecordPayment.ShowDialog( this );

				if( formViewRecordPayment.DialogResult == DialogResult.OK )
				{
					decimal netBalance;
					decimal totalAmountCollected;

					Payment payment = new Payment();
					payment = formViewRecordPayment.Model;
					this.lblTodaysPayment.Text = payment.TotalRecordedPayment.ToString( "n", i_NumberFormat );
                
					totalAmountCollected = ( this.Model.TotalPaid + payment.TotalRecordedPayment - prevTotalRecordedPayment);
				
					this.mtbTotalPaymentsCollected.Text = totalAmountCollected.ToString( "###,###,##0.00" );
				
					if( Convert.ToDecimal( payment.TotalRecordedPayment ) == 0m )
					{
						this.cmbReason.Enabled = true;
					}
					else
					{
						this.cmbReason.SelectedIndex = 0;
					}

					netBalance = this.Model.TotalCurrentAmtDue - totalAmountCollected;
					this.Model.BalanceDue = netBalance;
					this.lblNetBalance.Text = netBalance.ToString("n", i_NumberFormat );
					this.Model.Payment = payment;
					this.Model.TotalPaid = totalAmountCollected;
					this.Refresh();
				}
			}
			finally
			{
				formViewRecordPayment.Dispose();
			}
		}
		
		private void PopulateNumberOfMonthlyPayments( int numOfMonthlyPayments )
		{
			int range = 0;
            if( numOfMonthlyPayments > this.NumberOfMonths )
            {
                this.cmbNumberOfMonths.Items.Clear();
                range = 0;
                do
                {
                    this.cmbNumberOfMonths.Items.Add( range );
                    range++;
                } while ( range <= MAX_NUMBER_OF_MONTHS );
                
                this.cmbNumberOfMonths.SelectedIndex = cmbNumberOfMonths.FindString( numOfMonthlyPayments.ToString() );
                this.cmbNumberOfMonths.Enabled = false;
            }
            else
            {
                this.cmbNumberOfMonths.Items.Clear();
                range = 0;
                do
                {
                    this.cmbNumberOfMonths.Items.Add( range );
                    range++;
                } while ( range <= this.NumberOfMonths );

                this.cmbNumberOfMonths.SelectedIndex = cmbNumberOfMonths.FindString( numOfMonthlyPayments.ToString() );
                this.cmbNumberOfMonths.Enabled = true;
            }
		}


        private void Override()
        {
            FormViewMonthlyPaymentOverride formViewMonthlyPaymentOverride = new FormViewMonthlyPaymentOverride();
            formViewMonthlyPaymentOverride.PaymentService = this.PaymentService;

			decimal totalPaymentsCollected = this.TotalPaymentsCollected( this.mtbTotalPaymentsCollected.Text );
			this.Model.BalanceDue = this.Model.TotalCurrentAmtDue - totalPaymentsCollected;
			this.Model.NumberOfMonthlyPayments = Convert.ToInt16( this.cmbNumberOfMonths.SelectedItem.ToString() );
            formViewMonthlyPaymentOverride.Model = this.Model;  

            formViewMonthlyPaymentOverride.UpdateView();

			try
			{
				formViewMonthlyPaymentOverride.ShowDialog( this );

				if( formViewMonthlyPaymentOverride.DialogResult == DialogResult.OK )
				{
					int numOfMonthlyPayments = formViewMonthlyPaymentOverride.Model.NumberOfMonthlyPayments;
					this.PopulateNumberOfMonthlyPayments( numOfMonthlyPayments );

					this.Refresh();
				}
			}
			finally
			{
				formViewMonthlyPaymentOverride.Dispose();
			}
        }

        private void RegisterRulesEvents()
        {
            if( !this.Registered )
            {
                this.Registered = true;
                //RuleEngine.GetInstance().LoadRules( this.Model );
                RuleEngine.GetInstance().RegisterEvent( typeof( TotalPaymentsCollectedRule ), this.Model, new EventHandler( TotalPaymentsCollectedEventHandler ));     
            }
        }

        private void UnRegisterRulesEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( TotalPaymentsCollectedRule ), this.Model, new EventHandler( TotalPaymentsCollectedEventHandler ));     
        }

        private void TotalPaymentsCollectedEventHandler(object sender, EventArgs e)
        {
            UIColors.SetErrorBgColor( this.mtbTotalPaymentsCollected );
            MessageBox.Show( UIErrorMessages.PAYMENT_TOTAL_COLLECTED_ERRMSG, "Error",
                             MessageBoxButtons.OK, MessageBoxIcon.Warning,
                             MessageBoxDefaultButton.Button1 );
            this.mtbTotalPaymentsCollected.Focus();
        }

        private void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds
            UIColors.SetNormalBgColor( this.mtbTotalPaymentsCollected );
            this.Refresh();        

            RuleEngine.GetInstance().EvaluateRule( typeof(TotalPaymentsCollectedRule), this.Model );
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbReason = new System.Windows.Forms.ComboBox();
            this.gbxPaymentTerms = new System.Windows.Forms.GroupBox();
            this.lblNetBalance = new System.Windows.Forms.Label();
            this.lblMonthlyPayment = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnOverride = new LoggingButton();
            this.cmbNumberOfMonths = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblTodaysPayment = new System.Windows.Forms.Label();
            this.mtbTotalPaymentsCollected = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbMonthlyDueDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblMonthlyDueDate = new System.Windows.Forms.Label();
            this.btnRecordPayment = new LoggingButton();
            this.gbxPaymentTerms.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(239, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Today\'s payment collected for current account:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(230, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "Total payments collected for current account:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(30, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "If $0.00, specify reason:";
            // 
            // cmbReason
            // 
            this.cmbReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReason.Enabled = false;
            this.cmbReason.Items.AddRange(new object[] {
                                                           "",
                                                           "No payments as agreed",
                                                           "Discharged before contact",
                                                           "Left against medical advice",
                                                           "Refused to pay",
                                                           "Transferred to another facility"});
            this.cmbReason.Location = new System.Drawing.Point(152, 57);
            this.cmbReason.Name = "cmbReason";
            this.cmbReason.Size = new System.Drawing.Size(195, 21);
            this.cmbReason.TabIndex = 3;
            this.cmbReason.SelectedIndexChanged += new System.EventHandler(this.cmbReason_SelectedIndexChanged);
           
            // 
            // gbxPaymentTerms
            // 
            this.gbxPaymentTerms.Controls.Add(this.mtbMonthlyDueDate);
            this.gbxPaymentTerms.Controls.Add(this.lblMonthlyDueDate);
            this.gbxPaymentTerms.Controls.Add(this.lblNetBalance);
            this.gbxPaymentTerms.Controls.Add(this.lblMonthlyPayment);
            this.gbxPaymentTerms.Controls.Add(this.label8);
            this.gbxPaymentTerms.Controls.Add(this.label7);
            this.gbxPaymentTerms.Controls.Add(this.btnOverride);
            this.gbxPaymentTerms.Controls.Add(this.cmbNumberOfMonths);
            this.gbxPaymentTerms.Controls.Add(this.label6);
            this.gbxPaymentTerms.Controls.Add(this.label5);
            this.gbxPaymentTerms.Controls.Add(this.label4);
            this.gbxPaymentTerms.Location = new System.Drawing.Point(3, 90);
            this.gbxPaymentTerms.Name = "gbxPaymentTerms";
            this.gbxPaymentTerms.Size = new System.Drawing.Size(304, 125);
            this.gbxPaymentTerms.TabIndex = 4;
            this.gbxPaymentTerms.TabStop = false;
            this.gbxPaymentTerms.Text = "Payment terms for current account";
            // 
            // lblNetBalance
            // 
            this.lblNetBalance.Location = new System.Drawing.Point(167, 26);
            this.lblNetBalance.Name = "lblNetBalance";
            this.lblNetBalance.Size = new System.Drawing.Size(100, 19);
            this.lblNetBalance.TabIndex = 0;
            this.lblNetBalance.TextChanged += new System.EventHandler(this.lblNetBalance_TextChanged);
            // 
            // lblMonthlyPayment
            // 
            this.lblMonthlyPayment.Location = new System.Drawing.Point(167, 80);
            this.lblMonthlyPayment.Name = "lblMonthlyPayment";
            this.lblMonthlyPayment.Size = new System.Drawing.Size(100, 16);
            this.lblMonthlyPayment.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(149, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "$";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(149, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "$";
            // 
            // btnOverride
            // 
            this.btnOverride.Enabled = false;
            this.btnOverride.Location = new System.Drawing.Point(210, 49);
            this.btnOverride.Name = "btnOverride";
            this.btnOverride.TabIndex = 6;
            this.btnOverride.Text = "Override...";
            this.btnOverride.Click += new System.EventHandler(this.btnOverride_Click);
            // 
            // cmbNumberOfMonths
            // 
            this.cmbNumberOfMonths.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNumberOfMonths.Location = new System.Drawing.Point(159, 49);
            this.cmbNumberOfMonths.Name = "cmbNumberOfMonths";
            this.cmbNumberOfMonths.Size = new System.Drawing.Size(45, 21);
            this.cmbNumberOfMonths.TabIndex = 3;
            this.cmbNumberOfMonths.SelectedIndexChanged += new System.EventHandler(this.cmbNumberOfMonths_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(9, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Monthly payment:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(9, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(158, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "Number of monthly payments:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(9, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 14);
            this.label4.TabIndex = 0;
            this.label4.Text = "Net Balance:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(241, 34);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(11, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "$";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(241, 8);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(8, 15);
            this.label10.TabIndex = 0;
            this.label10.Text = "$";
            // 
            // lblTodaysPayment
            // 
            this.lblTodaysPayment.Location = new System.Drawing.Point(257, 10);
            this.lblTodaysPayment.Name = "lblTodaysPayment";
            this.lblTodaysPayment.Size = new System.Drawing.Size(97, 17);
            this.lblTodaysPayment.TabIndex = 0;
            this.lblTodaysPayment.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mtbTotalPaymentsCollected
            // 
            this.mtbTotalPaymentsCollected.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbTotalPaymentsCollected.KeyPressExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbTotalPaymentsCollected.Location = new System.Drawing.Point(257, 31);
            this.mtbTotalPaymentsCollected.Mask = "";
            this.mtbTotalPaymentsCollected.MaxLength = 14;
            this.mtbTotalPaymentsCollected.Name = "mtbTotalPaymentsCollected";
            this.mtbTotalPaymentsCollected.TabIndex = 2;
            this.mtbTotalPaymentsCollected.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbTotalPaymentsCollected.ValidationExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbTotalPaymentsCollected.Validating += new System.ComponentModel.CancelEventHandler(this.mtbTotalPaymentsCollected_Validating);
            this.mtbTotalPaymentsCollected.TextChanged += new System.EventHandler(this.mtbTotalPaymentsCollected_TextChanged);
            this.mtbTotalPaymentsCollected.Enter += new System.EventHandler(this.mtbTotalPaymentsCollected_Enter);
            // 
            // btnRecordPayment
            // 
            this.btnRecordPayment.Location = new System.Drawing.Point(365, 5);
            this.btnRecordPayment.Name = "btnRecordPayment";
            this.btnRecordPayment.Size = new System.Drawing.Size(111, 23);
            this.btnRecordPayment.TabIndex = 1;
            this.btnRecordPayment.Text = "Record Payment...";
            this.btnRecordPayment.Click += new System.EventHandler(this.btnRecordPayment_Click);
            // 
            // lblMonthlyDueDate
            // 
            this.lblMonthlyDueDate.Location = new System.Drawing.Point(9, 106);
            this.lblMonthlyDueDate.Name = "lblMonthlyDueDate";
            this.lblMonthlyDueDate.Size = new System.Drawing.Size(100, 17);
            this.lblMonthlyDueDate.TabIndex = 7;
            this.lblMonthlyDueDate.Text = "Monthly Due Date:";
            // 
            // mtbMonthlyDueDate
            // 
            this.mtbMonthlyDueDate.KeyPressExpression = "^[0-2][0-9]*|3[0-1]*|[0-9]$";
            this.mtbMonthlyDueDate.Location = new System.Drawing.Point(159, 101);
            this.mtbMonthlyDueDate.Mask = "";
            this.mtbMonthlyDueDate.MaxLength = 2;
            this.mtbMonthlyDueDate.Name = "mtbMonthlyDueDate";
            this.mtbMonthlyDueDate.Size = new System.Drawing.Size(30, 20);
            this.mtbMonthlyDueDate.TabIndex = 8;
            this.mtbMonthlyDueDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbMonthlyDueDate.ValidationExpression = "^[0-2][0-9]*|3[0-1]*|[0-9]$";
            this.mtbMonthlyDueDate.Enter += new System.EventHandler(this.mtbMonthlyDueDate_Enter);
            mtbMonthlyDueDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbMonthlyDueDate_Validating);
            mtbMonthlyDueDate.TextChanged += new System.EventHandler(this.mtbMonthlyDueDate_TextChanged);
            // 
            // PaymentCollectedView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnRecordPayment);
            this.Controls.Add(this.mtbTotalPaymentsCollected);
            this.Controls.Add(this.lblTodaysPayment);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.gbxPaymentTerms);
            this.Controls.Add(this.cmbReason);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PaymentCollectedView";
            this.Size = new System.Drawing.Size(479, 220);
            this.Load += new System.EventHandler(this.PaymentCollectedView_Load);
            this.gbxPaymentTerms.ResumeLayout(false);
            this.ResumeLayout(false);

        }

	    private void mtbMonthlyDueDate_TextChanged(object sender, EventArgs e)
	    {
            if (PaymentCollectedViewPresenter.IsValidMonthlyDueDate)
            {
                PaymentCollectedViewPresenter.EvaluateMonthlyDueDate();
            }
	    }

	    private void mtbMonthlyDueDate_Validating(object sender, CancelEventArgs e)
	    {
	        if (mtbMonthlyDueDate.Text != String.Empty && mtbMonthlyDueDate.Text != "0" &&
	            mtbMonthlyDueDate.TextLength == 1)
	        {
	            mtbMonthlyDueDate.Text = "0" + mtbMonthlyDueDate.Text;
	        }
	        else if (mtbMonthlyDueDate.Text != String.Empty && Convert.ToInt16(mtbMonthlyDueDate.Text) == 0)
	        {
	            mtbMonthlyDueDate.Text = String.Empty;
	        }

	        if (mtbMonthlyDueDate.Text != String.Empty && mtbMonthlyDueDate.Text != "0")
	        {
	            PaymentCollectedViewPresenter.SetMonthlyDueDate();
	        }
	        else
	        {
	            PaymentCollectedViewPresenter.SetMonthlyDueDate();
	        }
	        if (PaymentCollectedViewPresenter.IsValidMonthlyDueDate)
	        {
	            PaymentCollectedViewPresenter.EvaluateMonthlyDueDate();
	        }
	    }

	    private void mtbMonthlyDueDate_Enter(object sender, EventArgs e)
	    {
            mtbMonthlyDueDate.SelectionStart = mtbMonthlyDueDate.TextLength;
	    }

	    #endregion

		#endregion

		#region Private Properties

        private bool CanOverrideMonthlyPayment
        {
            get
            {
                return i_CanOverrideMonthlyPayment;
            }
            set
            {
                i_CanOverrideMonthlyPayment = value;
            }
        }

        private int NumberOfMonths
        {
            get
            {
                return i_NumberOfMonths;
            }
            set
            {
                i_NumberOfMonths = value;
            }
        }

        private bool Registered
        {
            get
            {
                return i_Registered;
            }
            set
            {
                i_Registered = value;
            }
        }
        public string MonthlyDueDate
        {
            get { return mtbMonthlyDueDate.Text; }
        }

        public void SetErrorBgColorForMonthlyDueDate()
        {
            UIColors.SetErrorBgColor(mtbMonthlyDueDate);
        }

        public void SetFocusToMonthlyDueDate()
        {
            mtbMonthlyDueDate.Focus();
        }

		#endregion

		#region Construction and Finalization
		public PaymentCollectedView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			base.EnableThemesOn( this );
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			this.UnRegisterRulesEvents();

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

		private Container             components = null;

		private ComboBox               cmbReason;
        private ComboBox               cmbNumberOfMonths;
        
        private LoggingButton                 btnOverride;
        private LoggingButton                 btnRecordPayment;
		
        private GroupBox               gbxPaymentTerms;

        private Label                  label1;
        private Label                  label2;
        private Label                  label3;
		private Label                  label4;
		private Label                  label5;
		private Label                  label6;
		private Label                  label7;
		private Label                  label8;
		private Label                  label9;
		private Label                  label10;
		private Label                  lblTodaysPayment;
        private Label                  lblMonthlyPayment;
        private Label                  lblNetBalance;

		private MaskedEditTextBox    mtbTotalPaymentsCollected;

		private NumberFormatInfo                            i_NumberFormat = new CultureInfo( "en-US", true ).NumberFormat;
        private PaymentService                              i_PaymentService;

        private bool                                        i_CanOverrideMonthlyPayment = false;
        private bool                                        i_Registered = false;
        private int                                         i_NumberOfMonths = 0;
        private MaskedEditTextBox mtbMonthlyDueDate;
        private Label lblMonthlyDueDate;

        private delegate void TotalPaymentsCollectedLeaveDelegate ();
        private IPaymentCollectedViewPresenter paymentCollectedViewPresenter;
		#endregion


		#region Constants

        private const int MAX_NUMBER_OF_MONTHS = 48;
		#endregion

	    Account IPaymentCollectedView.Model
	    {
	        get { return Model; }
	        set { Model = value; }
	    }
	}
}
