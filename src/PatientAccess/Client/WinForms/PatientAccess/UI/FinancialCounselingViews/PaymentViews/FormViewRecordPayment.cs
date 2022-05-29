using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
	/// <summary>
	/// Summary description for FormViewRecordPayment.
	/// </summary>
	[Serializable]
	public class FormViewRecordPayment : TimeOutFormView
	{
		#region Event Handlers
        private void btnOk_Click(object sender, EventArgs e)
		{
            this.recordPaymentView1.SetPaymentFieldsToNormalColor();

			PaymentAmount aCashPaymentAmount = new PaymentAmount( this.recordPaymentView1.CashAmount, new CashPayment() );

			if( this.recordPaymentView1.CheckAmount > 0 )
			{
				if( this.recordPaymentView1.CheckNumber == String.Empty || this.recordPaymentView1.CheckNumber == null )
				{
					MessageBox.Show( UIErrorMessages.CHECK_NUMBER_MISSING_ERRMSG, String.Empty,
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					this.DialogResult = DialogResult.None;
					return;
				}
			}
			PaymentAmount aCheckPaymentAmount = new PaymentAmount( this.recordPaymentView1.CheckAmount, new CheckPayment( this.recordPaymentView1.CheckNumber ) );

			PaymentAmount aCreditCardPaymentAmount1;
			if( this.recordPaymentView1.CreditCardAmount1 == 0 )
			{
				aCreditCardPaymentAmount1 = new PaymentAmount( this.recordPaymentView1.CreditCardAmount1, new CreditCardPayment() );
			}
			else
			{
				if( this.recordPaymentView1.CreditCardProvider1 == null )
				{
					MessageBox.Show( UIErrorMessages.CREDITCARDTYPE_MISSING_ERRMSG, String.Empty,
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					this.DialogResult = DialogResult.None;
					return;
				}
				aCreditCardPaymentAmount1 = new PaymentAmount( this.recordPaymentView1.CreditCardAmount1, new CreditCardPayment( this.recordPaymentView1.CreditCardProvider1 ) );
			}

            PaymentAmount aCreditCardPaymentAmount2;
            if (this.recordPaymentView1.CreditCardAmount2 == 0)
            {
                aCreditCardPaymentAmount2 = new PaymentAmount(this.recordPaymentView1.CreditCardAmount2, new CreditCardPayment());
            }
            else
            {
                if (this.recordPaymentView1.CreditCardProvider2 == null)
                {
                    MessageBox.Show(UIErrorMessages.CREDITCARDTYPE_MISSING_ERRMSG, String.Empty,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    this.DialogResult = DialogResult.None;
                    return;
                }
                aCreditCardPaymentAmount2 = new PaymentAmount(this.recordPaymentView1.CreditCardAmount2, new CreditCardPayment(this.recordPaymentView1.CreditCardProvider2));
            }

            PaymentAmount aCreditCardPaymentAmount3;
            if (this.recordPaymentView1.CreditCardAmount3 == 0)
            {
                aCreditCardPaymentAmount3 = new PaymentAmount(this.recordPaymentView1.CreditCardAmount3, new CreditCardPayment());
            }
            else
            {
                if (this.recordPaymentView1.CreditCardProvider3 == null)
                {
                    MessageBox.Show(UIErrorMessages.CREDITCARDTYPE_MISSING_ERRMSG, String.Empty,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    this.DialogResult = DialogResult.None;
                    return;
                }
                aCreditCardPaymentAmount3 = new PaymentAmount(this.recordPaymentView1.CreditCardAmount3, new CreditCardPayment(this.recordPaymentView1.CreditCardProvider3));
            }

			PaymentAmount aMoneyOrderPaymentAmount = new PaymentAmount( this.recordPaymentView1.MoneyOrderAmount, new MoneyOrderPayment() );

            decimal totalPaymentRecorded = 0.00m;
            totalPaymentRecorded = aCashPaymentAmount.Money.Amount + aCheckPaymentAmount.Money.Amount + 
                                   aCreditCardPaymentAmount1.Money.Amount + aMoneyOrderPaymentAmount.Money.Amount +
                                   aCreditCardPaymentAmount2.Money.Amount + aCreditCardPaymentAmount3.Money.Amount;

            if( this.IsPaymentAmountValid( totalPaymentRecorded ) )
            {
                this.Model.AddPayment( aCashPaymentAmount,Payment.PaymentType.Cash );
                this.Model.AddPayment( aCheckPaymentAmount, Payment.PaymentType.Check );
                this.Model.AddPayment( aCreditCardPaymentAmount1, Payment.PaymentType.CreditCard1 );
                this.Model.AddPayment( aMoneyOrderPaymentAmount, Payment.PaymentType.MoneyOrder );
                this.Model.AddPayment( aCreditCardPaymentAmount2, Payment.PaymentType.CreditCard2 );
                this.Model.AddPayment( aCreditCardPaymentAmount3, Payment.PaymentType.CreditCard3 );
                this.Model.ReceiptNumber = this.recordPaymentView1.ReceiptNumber;
            }
		}

		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
		public override void UpdateView()
		{
			this.recordPaymentView1.Model = this.Model;
			this.recordPaymentView1.UpdateView();
		}

		/// <summary>
		/// UpdateModel method
		/// </summary>
		public override void UpdateModel()
		{  			
		}

        public void SetButtonsTo ( bool availability )
        {
            this.btnOk.Enabled = availability;
        }

        public void SetPaymentService ( PaymentService paymentService )
        {
            this.recordPaymentView1.PaymentService = paymentService;
        }

        #endregion

		#region Properties
		public new Payment Model
		{
			get
			{
				return (Payment)base.Model;
			}
			set
			{
				base.Model = value;
			}
		}

		public decimal TotalPaymentsForAccount
		{
			get
			{
				return i_TotalPaymentsForAccount;
			}
			set
			{
				i_TotalPaymentsForAccount = value;
			}
		}

		public decimal TotalCurrentAmountDue
		{
			private get
			{
				return i_CurrentAmountDue;
			}
			set
			{
				i_CurrentAmountDue = value;
			}
		}

		public bool IsCurrentAccount
		{
			private get
			{
				return i_IsCurrentAccount;
			}
			set
			{
				i_IsCurrentAccount = value;
			}
		}

		public decimal BalanceDue
		{
			private get
			{
				return i_BalanceDue;
			}
			set
			{
				i_BalanceDue = value;
			}
		}
		#endregion

		#region Private Methods
		private bool IsPaymentAmountValid( decimal totalPaymentRecorded)
		{
            bool result = true;
			if( IsCurrentAccount )
			{
                if( totalPaymentRecorded > this.TotalCurrentAmountDue )
                {
                    DialogResult dialogResult;

                    dialogResult = MessageBox.Show( UIErrorMessages.PAYMENT_WARNING_MSG, "Warning",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1 );

                    if( dialogResult == DialogResult.No )
                    {
                        this.DialogResult = DialogResult.None;
                        result = false;
                    }
                }
                // note: Model.CalculateTotalPayments() - still holds previous value, totalPaymentRecorded - current values
                if( ( totalPaymentRecorded + i_TotalPaymentsForAccount - this.Model.CalculateTotalPayments() ) > MAX_TOTAL_AMOUNT )
                {
                    MessageBox.Show( UIErrorMessages.PAYMENT_EXCEEDS_LIMIT_ERRMSG, String.Empty,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );

                    this.recordPaymentView1.SetCursorPosition();
                    this.recordPaymentView1.SetPaymentFieldsToErrorColor();
                    this.DialogResult = DialogResult.None;
                    result = false;
                }                    
				
			}
			else
			{
				if( totalPaymentRecorded > this.BalanceDue )
				{
					this.recordPaymentView1.ValidateBalanceDueSum();
					this.DialogResult = DialogResult.None;
                    result = false;
				}	
			}
            return result;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.recordPaymentView1 = new PatientAccess.UI.FinancialCounselingViews.PaymentViews.RecordPaymentView();
            this.btnCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnOk = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelPayment = new System.Windows.Forms.Panel();
            this.panelPayment.SuspendLayout();
            this.SuspendLayout();
            // 
            // recordPaymentView1
            // 
            this.recordPaymentView1.BackColor = System.Drawing.Color.White;
            this.recordPaymentView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recordPaymentView1.Location = new System.Drawing.Point( 0, 0 );
            this.recordPaymentView1.Model = null;
            this.recordPaymentView1.Name = "recordPaymentView1";
            this.recordPaymentView1.PaymentService = null;
            this.recordPaymentView1.Size = new System.Drawing.Size( 425, 351 );
            this.recordPaymentView1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point( 359, 366 );
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 23 );
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.Control;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point( 279, 366 );
            this.btnOk.Message = null;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size( 75, 23 );
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler( this.btnOk_Click );
            // 
            // panelPayment
            // 
            this.panelPayment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPayment.Controls.Add( this.recordPaymentView1 );
            this.panelPayment.Location = new System.Drawing.Point( 7, 7 );
            this.panelPayment.Name = "panelPayment";
            this.panelPayment.Size = new System.Drawing.Size( 427, 353 );
            this.panelPayment.TabIndex = 0;
            // 
            // FormViewRecordPayment
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.ClientSize = new System.Drawing.Size( 441, 395 );
            this.Controls.Add( this.panelPayment );
            this.Controls.Add( this.btnOk );
            this.Controls.Add( this.btnCancel );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormViewRecordPayment";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Record Payment";
            this.panelPayment.ResumeLayout( false );
            this.ResumeLayout( false );

        }
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public FormViewRecordPayment()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
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
		private RecordPaymentView recordPaymentView1;
		private LoggingButton btnCancel;
		private LoggingButton btnOk;
		private decimal i_CurrentAmountDue;
		private Panel panelPayment;
		private decimal i_BalanceDue;
		private decimal i_TotalPaymentsForAccount;
		private bool i_IsCurrentAccount;
		#endregion
        
		#region Constants
        private const decimal MAX_TOTAL_AMOUNT = 9999999.99m;
		#endregion
	}
}
