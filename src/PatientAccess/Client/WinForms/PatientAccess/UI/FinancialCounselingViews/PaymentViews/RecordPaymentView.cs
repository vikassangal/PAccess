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

namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
	/// <summary>
	/// Summary description for RecordPaymentView.
	/// </summary>
	[Serializable]
	public class RecordPaymentView : ControlView
	{
		#region Event Handlers
		private void mtbCash_Enter(object sender, EventArgs e)
		{
			decimal cash = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbCash );	
			if( cash == 0 )
			{
				this.mtbCash.UnMaskedText = "0.00";
			}
			else
			{
				this.mtbCash.UnMaskedText = cash.ToString();
			}        
		}

		private void mtbCheck_Enter(object sender, EventArgs e)
		{
			decimal check = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbCheck );	
			if( check == 0 )
			{
				this.mtbCheck.UnMaskedText = "0.00";
			}
			else
			{
				this.mtbCheck.UnMaskedText = check.ToString();
			}        
		}

		private void mtbMoneyOrder_Enter(object sender, EventArgs e)
		{
			decimal moneyOrder = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbMoneyOrder );	
			if( moneyOrder == 0 )
			{
				this.mtbMoneyOrder.UnMaskedText = "0.00";
			}
			else
			{
				this.mtbMoneyOrder.UnMaskedText = moneyOrder.ToString();
			}        
		}

		private void mtbCreditCard1_Enter(object sender, EventArgs e)
		{
			decimal creditCard = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbCreditCard1 );	
			if( creditCard == 0 )
			{
				this.mtbCreditCard1.UnMaskedText = "0.00";
			}
			else
			{
				this.mtbCreditCard1.UnMaskedText = creditCard.ToString();
			}        
		}
        private void mtbCreditCard2_Enter(object sender, EventArgs e)
        {
            decimal creditCard = CommonFormatting.ConvertTextToCurrencyDecimal(this.mtbCreditCard2);
            if (creditCard == 0)
            {
                this.mtbCreditCard2.UnMaskedText = "0.00";
            }
            else
            {
                this.mtbCreditCard2.UnMaskedText = creditCard.ToString();
            }
        }

        private void mtbCreditCard3_Enter(object sender, EventArgs e)
        {
            decimal creditCard = CommonFormatting.ConvertTextToCurrencyDecimal(this.mtbCreditCard3);
            if (creditCard == 0)
            {
                this.mtbCreditCard3.UnMaskedText = "0.00";
            }
            else
            {
                this.mtbCreditCard3.UnMaskedText = creditCard.ToString();
            }
        }

        private void mtbCreditCard1_Validating(object sender, CancelEventArgs e)
        {
            CommonFormatting.FormatTextBoxCurrency( mtbCreditCard1, "###,###,##0.00" );    
        }

        private void mtbCheck_Validating(object sender, CancelEventArgs e)
        {
            CommonFormatting.FormatTextBoxCurrency( mtbCheck, "###,###,##0.00" );            
        }

        private void mtbCash_Validating(object sender, CancelEventArgs e)
        {
            CommonFormatting.FormatTextBoxCurrency( mtbCash, "###,###,##0.00" );   
        }

        private void mtbMoneyOrder_Validating(object sender, CancelEventArgs e)
        {
            CommonFormatting.FormatTextBoxCurrency( mtbMoneyOrder, "###,###,##0.00" );   
        }

        private void mtbCreditCard2_Validating(object sender, CancelEventArgs e)
        {
            CommonFormatting.FormatTextBoxCurrency(mtbCreditCard2, "###,###,##0.00");
        }

        private void mtbCreditCard3_Validating(object sender, CancelEventArgs e)
        {
            CommonFormatting.FormatTextBoxCurrency(mtbCreditCard3, "###,###,##0.00");
        }

		private void mtbCreditCard1_TextChanged(object sender, EventArgs e)
		{
			i_CreditCardAmt1 = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbCreditCard1 );
			
			if( i_CreditCardAmt1 > 0 )
			{
				this.cmbCardType1.Enabled = true;
                if (cmbCardType1.SelectedIndex <= 0)
                {
                    UIColors.SetRequiredBgColor( this.cmbCardType1 );
                    if (cmbCardType1.SelectedIndex <= 0)
                    {
                        UIColors.SetRequiredBgColor(this.cmbCardType1);
                    }
                    else
                    {
                        UIColors.SetNormalBgColor(this.cmbCardType1);
                    }
                }
                
			}
			else
			{
                UIColors.SetNormalBgColor( this.cmbCardType1 );
               	this.cmbCardType1.Enabled = false;
				this.cmbCardType1.SelectedIndex = -1;
			}

			this.CalculateTotalPayment();
            this.PaymentService.FormViewRecordPayment.SetButtonsTo( this.ValidateConditions() );
		}

		private void mtbCheck_TextChanged(object sender, EventArgs e)
		{
		    i_CheckAmt = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbCheck );		

			if( i_CheckAmt > 0 )
			{
				this.mtbCheckNumber.Enabled = true;
			}
			else
			{
				this.mtbCheckNumber.Enabled = false;
				this.mtbCheckNumber.Text = String.Empty;
			}

			this.CalculateTotalPayment();
            this.PaymentService.FormViewRecordPayment.SetButtonsTo( this.ValidateConditions() );
		}

		private void mtbCash_TextChanged(object sender, EventArgs e)
		{
		    i_CashAmt = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbCash );		
		
			this.CalculateTotalPayment();
            this.PaymentService.FormViewRecordPayment.SetButtonsTo( this.ValidateConditions() );
		}

		private void mtbMoneyOrder_TextChanged(object sender, EventArgs e)
		{
			i_MoneyOrderAmt = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbMoneyOrder );		

			this.CalculateTotalPayment();
            this.PaymentService.FormViewRecordPayment.SetButtonsTo( this.ValidateConditions() );
		}

		private void mtbCheckNumber_TextChanged(object sender, EventArgs e)
		{
			i_CheckNumber = this.mtbCheckNumber.Text;
            this.PaymentService.FormViewRecordPayment.SetButtonsTo( this.ValidateConditions() );
		}

		private void mtbReceiptNbr_TextChanged(object sender, EventArgs e)
		{
			i_ReceiptNumber = this.mtbReceiptNbr.Text;
            this.PaymentService.FormViewRecordPayment.SetButtonsTo( this.ValidateConditions() );
		}
        private void mtbCreditCard2_TextChanged(object sender, EventArgs e)
        {
            i_CreditCardAmt2 = CommonFormatting.ConvertTextToCurrencyDecimal(this.mtbCreditCard2);

            if (i_CreditCardAmt2 > 0)
            {

                this.cmbCardType2.Enabled = true;
                if (cmbCardType2.SelectedIndex <= 0)
                {
                    UIColors.SetRequiredBgColor(this.cmbCardType2);
                }
                else
                {
                    UIColors.SetNormalBgColor(this.cmbCardType2);
                }
            }
            else
            {
                UIColors.SetNormalBgColor(this.cmbCardType2);
                this.cmbCardType2.Enabled = false;
                this.cmbCardType2.SelectedIndex = -1;
            }

            this.CalculateTotalPayment();
            this.PaymentService.FormViewRecordPayment.SetButtonsTo(this.ValidateConditions());

        }

        private void mtbCreditCard3_TextChanged(object sender, EventArgs e)
        {
            i_CreditCardAmt3 = CommonFormatting.ConvertTextToCurrencyDecimal(this.mtbCreditCard3);

            if (i_CreditCardAmt3 > 0)
            {
                this.cmbCardType3.Enabled = true;
                if (cmbCardType2.SelectedIndex <= 0)
                {
                    UIColors.SetRequiredBgColor(this.cmbCardType3);
                }
                else
                {
                    UIColors.SetNormalBgColor(this.cmbCardType3);
                }
                
            }
            else
            {
                UIColors.SetNormalBgColor(this.cmbCardType3);
                this.cmbCardType3.Enabled = false;
                this.cmbCardType3.SelectedIndex = -1;
            }

            this.CalculateTotalPayment();
            this.PaymentService.FormViewRecordPayment.SetButtonsTo(this.ValidateConditions());
        }
		private void cmbCardType1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if( this.cmbCardType1.SelectedItem != null && this.cmbCardType1.SelectedItem.ToString() != String.Empty )
			{
				i_CreditCardProvider1 = (CreditCardProvider)this.cmbCardType1.SelectedItem;
                UIColors.SetNormalBgColor(this.cmbCardType1);
			}
			else
			{
				i_CreditCardProvider1 = null;
                UIColors.SetRequiredBgColor(this.cmbCardType1);
			}
            this.PaymentService.FormViewRecordPayment.SetButtonsTo( this.ValidateConditions() );
		}
        private void cmbCardtype2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbCardType2.SelectedItem != null && this.cmbCardType2.SelectedItem.ToString() != String.Empty)
            {
                i_CreditCardProvider2 = (CreditCardProvider)this.cmbCardType2.SelectedItem;
                UIColors.SetNormalBgColor(this.cmbCardType2);
            }
            else
            {
                i_CreditCardProvider2 = null;
                UIColors.SetRequiredBgColor(this.cmbCardType2);
            }
            this.PaymentService.FormViewRecordPayment.SetButtonsTo(this.ValidateConditions());
        }

        private void cmbCardType3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbCardType3.SelectedItem != null && this.cmbCardType3.SelectedItem.ToString() != String.Empty)
            {
                i_CreditCardProvider3 = (CreditCardProvider)this.cmbCardType3.SelectedItem;
                UIColors.SetNormalBgColor(this.cmbCardType3);
            }
            else
            {
                i_CreditCardProvider3 = null;
                UIColors.SetRequiredBgColor(this.cmbCardType3);
            }
            this.PaymentService.FormViewRecordPayment.SetButtonsTo(this.ValidateConditions());
        }

        private void radioButtonForCreditCardClicked(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;

            switch (radio.Text.ToUpper().Substring(0, 1))
            {
                case MULITPLE_CREDIT_YES:
                    this.CreditCardPanel.Visible = true;
                    this.otherPanel.Location = new Point(0, 240);
                    break;

                case MULTIPLE_CREDIT_NO:
                    this.mtbCreditCard2.UnMaskedText = "0.00";
                    this.cmbCardType2.SelectedIndex = -1;
                    this.mtbCreditCard3.UnMaskedText = "0.00";
                    this.cmbCardType3.SelectedIndex = -1;
                    this.CreditCardPanel.Visible = false;
                    this.otherPanel.Location = new Point(0, 190);
                    break;
            }

        }
        private void RecordPaymentView_Load(object sender, EventArgs e)
        {
            if( rdYes.Checked )
            {
                this.otherPanel.Location = new Point( 0, 240 );
            }
            else
            {
                this.otherPanel.Location = new Point( 0, 190 );
            }
        }

		#endregion

		#region Methods
		/// <summary>
		/// UpdateView with data from model
		/// </summary>
		public override void UpdateView()
		{
            decimal creditCardAmount2 = this.Model.AmountPaidWith( Payment.PaymentType.CreditCard2 );
            decimal creditCardAmount3 = this.Model.AmountPaidWith( Payment.PaymentType.CreditCard3 );
			this.mtbReceiptNbr.Text = this.Model.ReceiptNumber;
         	this.mtbCash.Text = this.Model.AmountPaidWith( Payment.PaymentType.Cash ).ToString( "n", i_NumberFormat );
			this.mtbCheck.Text = this.Model.AmountPaidWith( Payment.PaymentType.Check ).ToString( "n", i_NumberFormat );
			this.mtbCheckNumber.Text = this.Model.CheckNumber();
            this.mtbCreditCard1.Text = this.Model.AmountPaidWith( Payment.PaymentType.CreditCard1).ToString("n", i_NumberFormat);
            this.mtbCreditCard2.Text = creditCardAmount2.ToString();
            this.mtbCreditCard3.Text = creditCardAmount3.ToString();

            if( creditCardAmount2 > 0 || creditCardAmount3 > 0)
            {
                rdYes.Checked = true;
                radioButtonForCreditCardClicked( rdYes, EventArgs.Empty );
            }
            
			this.PopulateCreditCardTypes();

            if (this.Model.CardType( Payment.PaymentType.CreditCard1 ) != null && this.Model.CardType( Payment.PaymentType.CreditCard1 ) != String.Empty)
			{
				int cardTypeSelectedIndex = -1;
				cardTypeSelectedIndex =
                    cmbCardType1.FindString(this.Model.CardType( Payment.PaymentType.CreditCard1 ));

				if( cardTypeSelectedIndex != -1 )
				{
					this.cmbCardType1.SelectedIndex = cardTypeSelectedIndex;
				}
			}
            if (this.Model.CardType( Payment.PaymentType.CreditCard2 ) != null && this.Model.CardType( Payment.PaymentType.CreditCard2 ) != String.Empty)
            {
                int cardTypeSelectedIndex = -1;
                cardTypeSelectedIndex =
                    cmbCardType2.FindString(this.Model.CardType( Payment.PaymentType.CreditCard2 ));

                if (cardTypeSelectedIndex != -1)
                {
                    this.cmbCardType2.SelectedIndex = cardTypeSelectedIndex;
                }
            }
            if (this.Model.CardType( Payment.PaymentType.CreditCard3 ) != null && this.Model.CardType( Payment.PaymentType.CreditCard3 ) != String.Empty)
            {
                int cardTypeSelectedIndex = -1;
                cardTypeSelectedIndex =
                    cmbCardType1.FindString(this.Model.CardType( Payment.PaymentType.CreditCard3 ));

                if (cardTypeSelectedIndex != -1)
                {
                    this.cmbCardType3.SelectedIndex = cardTypeSelectedIndex;
                }
            }
			this.mtbMoneyOrder.Text = this.Model.AmountPaidWith( Payment.PaymentType.MoneyOrder ).ToString( "n", i_NumberFormat );

			this.lblTotalAmt.Text = this.Model.CalculateTotalPayments().ToString( "n", i_NumberFormat );
		}

		/// <summary>
		/// UpdateModel method
		/// </summary>
		public override void UpdateModel()
		{   
		}

		public void ValidateBalanceDueSum()
		{
			MessageBox.Show( UIErrorMessages.PAYMENT_EXCEEDS_BALANCE_ERRMSG, String.Empty,
				MessageBoxButtons.OK, MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button1 );
			
			this.SetCursorPosition();
		}

		public void SetCursorPosition()
		{
			if( this.i_CreditCardAmt1 > 0  )
			{
				this.mtbCreditCard1.Focus();
			}
            if (this.i_CreditCardAmt2 > 0)
            {
                this.mtbCreditCard2.Focus();
            }
            if (this.i_CreditCardAmt3 > 0)
            {
                this.mtbCreditCard3.Focus();
            }
			else if( this.i_CheckAmt > 0 )
			{
				this.mtbCheck.Focus();
			}
			else if( this.i_CashAmt > 0 )
			{
				this.mtbCash.Focus();
			}
			else if( this.i_MoneyOrderAmt > 0 )
			{
				this.mtbMoneyOrder.Focus();
			}
		}

        public void SetPaymentFieldsToErrorColor ()
        {
            if( this.i_CreditCardAmt1 > 0  )
            {
                UIColors.SetErrorBgColor( this.mtbCreditCard1 );
            }
            if (this.i_CreditCardAmt2 > 0)
            {
                UIColors.SetErrorBgColor(this.mtbCreditCard2);
            }
            if (this.i_CreditCardAmt3 > 0)
            {
                UIColors.SetErrorBgColor(this.mtbCreditCard3);
            }
            if( this.i_CheckAmt > 0 )
            {
                UIColors.SetErrorBgColor( this.mtbCheck );
            }
            if( this.i_CashAmt > 0 )
            {
                UIColors.SetErrorBgColor( this.mtbCash );
            }
            if( this.i_MoneyOrderAmt > 0 )
            {
                UIColors.SetErrorBgColor( this.mtbMoneyOrder );
            }
        }

        public void SetPaymentFieldsToNormalColor ()
        {
            UIColors.SetNormalBgColor( this.mtbCreditCard1 );
            UIColors.SetNormalBgColor( this.mtbCheck );
            UIColors.SetNormalBgColor( this.mtbCash );
            UIColors.SetNormalBgColor( this.mtbMoneyOrder );
        }


		#endregion

		#region Properties
		public new Payment Model
		{
			private get
			{
				return (Payment)base.Model;
			}
			set
			{
				base.Model = value;
			}
		}

		public decimal CheckAmount
		{
			get
			{
				return i_CheckAmt;
			}
		}

		public string CheckNumber
		{
			get
			{
				return i_CheckNumber;
			}
		}

		public string ReceiptNumber
		{
			get
			{
				return i_ReceiptNumber;
			}
		}

		public decimal CashAmount
		{
			get
			{
				return i_CashAmt;
			}
		}

		public decimal MoneyOrderAmount
		{
			get
			{
				return i_MoneyOrderAmt;
			}
		}

		public decimal CreditCardAmount1
		{
			get
			{
				return i_CreditCardAmt1;
			}
		}
        public decimal CreditCardAmount2
        {
            get
            {
                return i_CreditCardAmt2;
            }
        }
        public decimal CreditCardAmount3
        {
            get
            {
                return i_CreditCardAmt3;
            }
        }
		public CreditCardProvider CreditCardProvider1
		{
			get
			{
				return i_CreditCardProvider1;
			}
		}
        public CreditCardProvider CreditCardProvider2
        {
            get
            {
                return i_CreditCardProvider2;
            }
        }
        public CreditCardProvider CreditCardProvider3
        {
            get
            {
                return i_CreditCardProvider3;
            }
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
		private void CalculateTotalPayment() 
		{      
			decimal totalPayment;

            totalPayment = (Math.Round(i_CashAmt, 2) + Math.Round(i_CheckAmt, 2) + Math.Round(i_CreditCardAmt1, 2) + Math.Round(i_MoneyOrderAmt, 2) + Math.Round(i_CreditCardAmt2, 2) + Math.Round(i_CreditCardAmt3, 2)); 
			//float testTotal = float.Parse( totalPayment.ToString() ); 
 
			this.lblTotalAmt.Text = totalPayment.ToString( "###,###,##0.00" ); 
		} 

		private void PopulateCreditCardTypes()
		{
			if( this.cmbCardType1.Items.Count == 0 )
			{
				ICreditCardTypeBroker broker = BrokerFactory.BrokerOfType<ICreditCardTypeBroker>();
				ICollection creditCardTypeList = (ICollection)broker.AllCreditCardTypes();

				this.cmbCardType1.ValueMember   = "Value";
				this.cmbCardType1.DisplayMember = "Key";

				this.cmbCardType1.Items.Add("");
				this.cmbCardType1.SelectedItem = "";

                this.cmbCardType2.ValueMember = "Value";
                this.cmbCardType2.DisplayMember = "Key";

                this.cmbCardType2.Items.Add("");
                this.cmbCardType2.SelectedItem = "";

                this.cmbCardType3.ValueMember = "Value";
                this.cmbCardType3.DisplayMember = "Key";

                this.cmbCardType3.Items.Add("");
                this.cmbCardType3.SelectedItem = "";

				foreach( CreditCardProvider cardType in creditCardTypeList )
				{
					if( cardType.Description == "American Express" )
					{
						cardType.Description = "AMEX";
					}
					cmbCardType1.Items.Add( cardType );
                    cmbCardType2.Items.Add( cardType );
                    cmbCardType3.Items.Add( cardType );
				}
			}
		}

        private bool ValidateConditions()
        {
            if( this.CreditCardAmount1 > 0 && 
                ( this.cmbCardType1.SelectedItem == null ||
                    this.cmbCardType1.SelectedItem.ToString() == String.Empty ) )
            {
                return false;
            }
            if (this.CreditCardAmount2 > 0 &&
                (this.cmbCardType2.SelectedItem == null ||
                    this.cmbCardType2.SelectedItem.ToString() == string.Empty))
            {
                return false;
            }
            if (this.CreditCardAmount3 > 0 &&
                (this.cmbCardType3.SelectedItem == null ||
                    this.cmbCardType3.SelectedItem.ToString() == string.Empty))
            {
                return false;
            }
            if( this.CheckAmount > 0 && 
                ( this.mtbCheckNumber.Text == null ||
                    this.mtbCheckNumber.Text == String.Empty ) )
            {
                return false;
            }
            return true;
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblReceipt = new System.Windows.Forms.Label();
            this.lblPaymentMethod = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblInfo2 = new System.Windows.Forms.Label();
            this.lblCreditCard1 = new System.Windows.Forms.Label();
            this.lblCheck = new System.Windows.Forms.Label();
            this.lblCash = new System.Windows.Forms.Label();
            this.lblMoneyOrder = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblDollar4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDollar1 = new System.Windows.Forms.Label();
            this.lblDollar2 = new System.Windows.Forms.Label();
            this.lblDollar5 = new System.Windows.Forms.Label();
            this.lblCardType = new System.Windows.Forms.Label();
            this.lblCheckNumber = new System.Windows.Forms.Label();
            this.cmbCardType1 = new System.Windows.Forms.ComboBox();
            this.mtbCheckNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbReceiptNbr = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbMoneyOrder = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbCash = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbCheck = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbCreditCard1 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbCreditCard2 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCreditCard2 = new System.Windows.Forms.Label();
            this.lblCreditCard3 = new System.Windows.Forms.Label();
            this.lblDollar3 = new System.Windows.Forms.Label();
            this.mtbCreditCard3 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCardType1 = new System.Windows.Forms.Label();
            this.lblCardType3 = new System.Windows.Forms.Label();
            this.cmbCardType2 = new System.Windows.Forms.ComboBox();
            this.cmbCardType3 = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.rdYes = new System.Windows.Forms.RadioButton();
            this.rdNo = new System.Windows.Forms.RadioButton();
            this.CreditCardPanel = new System.Windows.Forms.Panel();
            this.otherPanel = new System.Windows.Forms.Panel();
            this.lblTotalAmt = new System.Windows.Forms.Label();
            this.lblDollar6 = new System.Windows.Forms.Label();
            this.CreditCardPanel.SuspendLayout();
            this.otherPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.Location = new System.Drawing.Point( 13, 10 );
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size( 400, 21 );
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "All entries will be recorded to a system note only.";
            // 
            // lblReceipt
            // 
            this.lblReceipt.Location = new System.Drawing.Point( 13, 54 );
            this.lblReceipt.Name = "lblReceipt";
            this.lblReceipt.Size = new System.Drawing.Size( 157, 16 );
            this.lblReceipt.TabIndex = 0;
            this.lblReceipt.Text = "Cash Receipt Journal number:";
            // 
            // lblPaymentMethod
            // 
            this.lblPaymentMethod.ForeColor = System.Drawing.Color.Blue;
            this.lblPaymentMethod.Location = new System.Drawing.Point( 13, 85 );
            this.lblPaymentMethod.Name = "lblPaymentMethod";
            this.lblPaymentMethod.Size = new System.Drawing.Size( 90, 15 );
            this.lblPaymentMethod.TabIndex = 0;
            this.lblPaymentMethod.Text = "Payment method";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 95, 85 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 304, 17 );
            this.label1.TabIndex = 0;
            this.label1.Text = "____________________________________________________";
            // 
            // lblInfo2
            // 
            this.lblInfo2.Location = new System.Drawing.Point( 13, 107 );
            this.lblInfo2.Name = "lblInfo2";
            this.lblInfo2.Size = new System.Drawing.Size( 385, 26 );
            this.lblInfo2.TabIndex = 0;
            this.lblInfo2.Text = "At least one type of payment  is required.  Card type is required for credit card" +
                " payment.  Check number is required for check payment.";
            // 
            // lblCreditCard1
            // 
            this.lblCreditCard1.Location = new System.Drawing.Point( 13, 170 );
            this.lblCreditCard1.Name = "lblCreditCard1";
            this.lblCreditCard1.Size = new System.Drawing.Size( 71, 16 );
            this.lblCreditCard1.TabIndex = 0;
            this.lblCreditCard1.Text = "Credit card 1:";
            // 
            // lblCheck
            // 
            this.lblCheck.Location = new System.Drawing.Point( 13, 7 );
            this.lblCheck.Name = "lblCheck";
            this.lblCheck.Size = new System.Drawing.Size( 71, 16 );
            this.lblCheck.TabIndex = 0;
            this.lblCheck.Text = "Check:";
            // 
            // lblCash
            // 
            this.lblCash.Location = new System.Drawing.Point( 13, 34 );
            this.lblCash.Name = "lblCash";
            this.lblCash.Size = new System.Drawing.Size( 71, 16 );
            this.lblCash.TabIndex = 0;
            this.lblCash.Text = "Cash:";
            // 
            // lblMoneyOrder
            // 
            this.lblMoneyOrder.Location = new System.Drawing.Point( 13, 60 );
            this.lblMoneyOrder.Name = "lblMoneyOrder";
            this.lblMoneyOrder.Size = new System.Drawing.Size( 71, 16 );
            this.lblMoneyOrder.TabIndex = 0;
            this.lblMoneyOrder.Text = "Money order:";
            // 
            // lblTotal
            // 
            this.lblTotal.Location = new System.Drawing.Point( 13, 83 );
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size( 84, 16 );
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "Total Payments:";
            // 
            // lblDollar4
            // 
            this.lblDollar4.Location = new System.Drawing.Point( 106, 7 );
            this.lblDollar4.Name = "lblDollar4";
            this.lblDollar4.Size = new System.Drawing.Size( 12, 15 );
            this.lblDollar4.TabIndex = 0;
            this.lblDollar4.Text = "$";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point( 106, 83 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 12, 15 );
            this.label3.TabIndex = 0;
            this.label3.Text = "$";
            // 
            // lblDollar1
            // 
            this.lblDollar1.Location = new System.Drawing.Point( 106, 170 );
            this.lblDollar1.Name = "lblDollar1";
            this.lblDollar1.Size = new System.Drawing.Size( 12, 15 );
            this.lblDollar1.TabIndex = 0;
            this.lblDollar1.Text = "$";
            // 
            // lblDollar2
            // 
            this.lblDollar2.Location = new System.Drawing.Point( 106, 5 );
            this.lblDollar2.Name = "lblDollar2";
            this.lblDollar2.Size = new System.Drawing.Size( 12, 15 );
            this.lblDollar2.TabIndex = 0;
            this.lblDollar2.Text = "$";
            // 
            // lblDollar5
            // 
            this.lblDollar5.Location = new System.Drawing.Point( 106, 34 );
            this.lblDollar5.Name = "lblDollar5";
            this.lblDollar5.Size = new System.Drawing.Size( 12, 15 );
            this.lblDollar5.TabIndex = 0;
            this.lblDollar5.Text = "$";
            // 
            // lblCardType
            // 
            this.lblCardType.Location = new System.Drawing.Point( 225, 170 );
            this.lblCardType.Name = "lblCardType";
            this.lblCardType.Size = new System.Drawing.Size( 74, 15 );
            this.lblCardType.TabIndex = 0;
            this.lblCardType.Text = "Card type 1:";
            // 
            // lblCheckNumber
            // 
            this.lblCheckNumber.Location = new System.Drawing.Point( 225, 8 );
            this.lblCheckNumber.Name = "lblCheckNumber";
            this.lblCheckNumber.Size = new System.Drawing.Size( 81, 13 );
            this.lblCheckNumber.TabIndex = 0;
            this.lblCheckNumber.Text = "Check number:";
            // 
            // cmbCardType1
            // 
            this.cmbCardType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCardType1.Location = new System.Drawing.Point( 306, 167 );
            this.cmbCardType1.Name = "cmbCardType1";
            this.cmbCardType1.Size = new System.Drawing.Size( 100, 21 );
            this.cmbCardType1.TabIndex = 5;
            this.cmbCardType1.SelectedIndexChanged += new System.EventHandler( this.cmbCardType1_SelectedIndexChanged );
            // 
            // mtbCheckNumber
            // 
            this.mtbCheckNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbCheckNumber.Location = new System.Drawing.Point( 306, 4 );
            this.mtbCheckNumber.Mask = "";
            this.mtbCheckNumber.MaxLength = 10;
            this.mtbCheckNumber.Name = "mtbCheckNumber";
            this.mtbCheckNumber.Size = new System.Drawing.Size( 85, 20 );
            this.mtbCheckNumber.TabIndex = 5;
            this.mtbCheckNumber.TextChanged += new System.EventHandler( this.mtbCheckNumber_TextChanged );
            // 
            // mtbReceiptNbr
            // 
            this.mtbReceiptNbr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbReceiptNbr.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbReceiptNbr.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbReceiptNbr.Location = new System.Drawing.Point( 177, 51 );
            this.mtbReceiptNbr.Mask = "";
            this.mtbReceiptNbr.MaxLength = 10;
            this.mtbReceiptNbr.Name = "mtbReceiptNbr";
            this.mtbReceiptNbr.Size = new System.Drawing.Size( 78, 20 );
            this.mtbReceiptNbr.TabIndex = 1;
            this.mtbReceiptNbr.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbReceiptNbr.TextChanged += new System.EventHandler( this.mtbReceiptNbr_TextChanged );
            // 
            // mtbMoneyOrder
            // 
            this.mtbMoneyOrder.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbMoneyOrder.KeyPressExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbMoneyOrder.Location = new System.Drawing.Point( 124, 58 );
            this.mtbMoneyOrder.Mask = "";
            this.mtbMoneyOrder.MaxLength = 11;
            this.mtbMoneyOrder.Name = "mtbMoneyOrder";
            this.mtbMoneyOrder.Size = new System.Drawing.Size( 80, 20 );
            this.mtbMoneyOrder.TabIndex = 7;
            this.mtbMoneyOrder.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbMoneyOrder.ValidationExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbMoneyOrder.Enter += new System.EventHandler( this.mtbMoneyOrder_Enter );
            this.mtbMoneyOrder.Validating += new System.ComponentModel.CancelEventHandler( this.mtbMoneyOrder_Validating );
            this.mtbMoneyOrder.TextChanged += new System.EventHandler( this.mtbMoneyOrder_TextChanged );
            // 
            // mtbCash
            // 
            this.mtbCash.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCash.KeyPressExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCash.Location = new System.Drawing.Point( 124, 31 );
            this.mtbCash.Mask = "";
            this.mtbCash.MaxLength = 11;
            this.mtbCash.Name = "mtbCash";
            this.mtbCash.Size = new System.Drawing.Size( 80, 20 );
            this.mtbCash.TabIndex = 6;
            this.mtbCash.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbCash.ValidationExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCash.Enter += new System.EventHandler( this.mtbCash_Enter );
            this.mtbCash.Validating += new System.ComponentModel.CancelEventHandler( this.mtbCash_Validating );
            this.mtbCash.TextChanged += new System.EventHandler( this.mtbCash_TextChanged );
            // 
            // mtbCheck
            // 
            this.mtbCheck.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCheck.KeyPressExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCheck.Location = new System.Drawing.Point( 124, 4 );
            this.mtbCheck.Mask = "";
            this.mtbCheck.MaxLength = 11;
            this.mtbCheck.Name = "mtbCheck";
            this.mtbCheck.Size = new System.Drawing.Size( 80, 20 );
            this.mtbCheck.TabIndex = 4;
            this.mtbCheck.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbCheck.ValidationExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCheck.Enter += new System.EventHandler( this.mtbCheck_Enter );
            this.mtbCheck.Validating += new System.ComponentModel.CancelEventHandler( this.mtbCheck_Validating );
            this.mtbCheck.TextChanged += new System.EventHandler( this.mtbCheck_TextChanged );
            // 
            // mtbCreditCard1
            // 
            this.mtbCreditCard1.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCreditCard1.KeyPressExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCreditCard1.Location = new System.Drawing.Point( 124, 167 );
            this.mtbCreditCard1.Mask = "";
            this.mtbCreditCard1.MaxLength = 11;
            this.mtbCreditCard1.Name = "mtbCreditCard1";
            this.mtbCreditCard1.Size = new System.Drawing.Size( 80, 20 );
            this.mtbCreditCard1.TabIndex = 4;
            this.mtbCreditCard1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbCreditCard1.ValidationExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCreditCard1.Enter += new System.EventHandler( this.mtbCreditCard1_Enter );
            this.mtbCreditCard1.Validating += new System.ComponentModel.CancelEventHandler( this.mtbCreditCard1_Validating );
            this.mtbCreditCard1.TextChanged += new System.EventHandler( this.mtbCreditCard1_TextChanged );
            // 
            // mtbCreditCard2
            // 
            this.mtbCreditCard2.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCreditCard2.KeyPressExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCreditCard2.Location = new System.Drawing.Point( 124, 2 );
            this.mtbCreditCard2.Mask = "";
            this.mtbCreditCard2.MaxLength = 11;
            this.mtbCreditCard2.Name = "mtbCreditCard2";
            this.mtbCreditCard2.Size = new System.Drawing.Size( 80, 20 );
            this.mtbCreditCard2.TabIndex = 6;
            this.mtbCreditCard2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbCreditCard2.ValidationExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCreditCard2.Enter += new System.EventHandler( this.mtbCreditCard2_Enter );
            this.mtbCreditCard2.Validating += new System.ComponentModel.CancelEventHandler( this.mtbCreditCard2_Validating );
            this.mtbCreditCard2.TextChanged += new System.EventHandler( this.mtbCreditCard2_TextChanged );
            // 
            // lblCreditCard2
            // 
            this.lblCreditCard2.Location = new System.Drawing.Point( 13, 5 );
            this.lblCreditCard2.Name = "lblCreditCard2";
            this.lblCreditCard2.Size = new System.Drawing.Size( 71, 16 );
            this.lblCreditCard2.TabIndex = 10;
            this.lblCreditCard2.Text = "Credit card 2:";
            // 
            // lblCreditCard3
            // 
            this.lblCreditCard3.Location = new System.Drawing.Point( 13, 31 );
            this.lblCreditCard3.Name = "lblCreditCard3";
            this.lblCreditCard3.Size = new System.Drawing.Size( 71, 16 );
            this.lblCreditCard3.TabIndex = 12;
            this.lblCreditCard3.Text = "Credit card 3:";
            // 
            // lblDollar3
            // 
            this.lblDollar3.Location = new System.Drawing.Point( 106, 31 );
            this.lblDollar3.Name = "lblDollar3";
            this.lblDollar3.Size = new System.Drawing.Size( 12, 15 );
            this.lblDollar3.TabIndex = 11;
            this.lblDollar3.Text = "$";
            // 
            // mtbCreditCard3
            // 
            this.mtbCreditCard3.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCreditCard3.KeyPressExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCreditCard3.Location = new System.Drawing.Point( 124, 29 );
            this.mtbCreditCard3.Mask = "";
            this.mtbCreditCard3.MaxLength = 11;
            this.mtbCreditCard3.Name = "mtbCreditCard3";
            this.mtbCreditCard3.Size = new System.Drawing.Size( 80, 20 );
            this.mtbCreditCard3.TabIndex = 8;
            this.mtbCreditCard3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbCreditCard3.ValidationExpression = "^[0-9]{0,8}(\\.[0-9]{0,2})?$";
            this.mtbCreditCard3.Enter += new System.EventHandler( this.mtbCreditCard3_Enter );
            this.mtbCreditCard3.Validating += new System.ComponentModel.CancelEventHandler( this.mtbCreditCard3_Validating );
            this.mtbCreditCard3.TextChanged += new System.EventHandler( this.mtbCreditCard3_TextChanged );
            // 
            // lblCardType1
            // 
            this.lblCardType1.Location = new System.Drawing.Point( 225, 5 );
            this.lblCardType1.Name = "lblCardType1";
            this.lblCardType1.Size = new System.Drawing.Size( 74, 15 );
            this.lblCardType1.TabIndex = 14;
            this.lblCardType1.Text = "Card type 2:";
            // 
            // lblCardType3
            // 
            this.lblCardType3.Location = new System.Drawing.Point( 225, 33 );
            this.lblCardType3.Name = "lblCardType3";
            this.lblCardType3.Size = new System.Drawing.Size( 72, 15 );
            this.lblCardType3.TabIndex = 15;
            this.lblCardType3.Text = "Card type 3:";
            // 
            // cmbCardType2
            // 
            this.cmbCardType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCardType2.Location = new System.Drawing.Point( 306, 2 );
            this.cmbCardType2.Name = "cmbCardType2";
            this.cmbCardType2.Size = new System.Drawing.Size( 100, 21 );
            this.cmbCardType2.TabIndex = 7;
            this.cmbCardType2.SelectedIndexChanged += new System.EventHandler( this.cmbCardtype2_SelectedIndexChanged );
            // 
            // cmbCardType3
            // 
            this.cmbCardType3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCardType3.Location = new System.Drawing.Point( 306, 29 );
            this.cmbCardType3.Name = "cmbCardType3";
            this.cmbCardType3.Size = new System.Drawing.Size( 100, 21 );
            this.cmbCardType3.TabIndex = 9;
            this.cmbCardType3.SelectedIndexChanged += new System.EventHandler( this.cmbCardType3_SelectedIndexChanged );
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point( 13, 137 );
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size( 385, 26 );
            this.label12.TabIndex = 18;
            this.label12.Text = "Payment received using more than one credit card?";
            // 
            // rdYes
            // 
            this.rdYes.AutoSize = true;
            this.rdYes.Location = new System.Drawing.Point( 278, 135 );
            this.rdYes.Name = "rdYes";
            this.rdYes.Size = new System.Drawing.Size( 43, 17 );
            this.rdYes.TabIndex = 2;
            this.rdYes.Text = "Yes";
            this.rdYes.UseVisualStyleBackColor = true;
            this.rdYes.Click += new System.EventHandler( this.radioButtonForCreditCardClicked );
            // 
            // rdNo
            // 
            this.rdNo.AutoSize = true;
            this.rdNo.Checked = true;
            this.rdNo.Location = new System.Drawing.Point( 342, 137 );
            this.rdNo.Name = "rdNo";
            this.rdNo.Size = new System.Drawing.Size( 39, 17 );
            this.rdNo.TabIndex = 3;
            this.rdNo.TabStop = true;
            this.rdNo.Text = "No";
            this.rdNo.UseVisualStyleBackColor = true;
            this.rdNo.Click += new System.EventHandler( this.radioButtonForCreditCardClicked );
            // 
            // CreditCardPanel
            // 
            this.CreditCardPanel.Controls.Add( this.lblCreditCard2 );
            this.CreditCardPanel.Controls.Add( this.lblDollar2 );
            this.CreditCardPanel.Controls.Add( this.mtbCreditCard2 );
            this.CreditCardPanel.Controls.Add( this.lblCardType1 );
            this.CreditCardPanel.Controls.Add( this.cmbCardType3 );
            this.CreditCardPanel.Controls.Add( this.cmbCardType2 );
            this.CreditCardPanel.Controls.Add( this.lblCardType3 );
            this.CreditCardPanel.Controls.Add( this.lblCreditCard3 );
            this.CreditCardPanel.Controls.Add( this.mtbCreditCard3 );
            this.CreditCardPanel.Controls.Add( this.lblDollar3 );
            this.CreditCardPanel.Location = new System.Drawing.Point( 0, 190 );
            this.CreditCardPanel.Name = "CreditCardPanel";
            this.CreditCardPanel.Size = new System.Drawing.Size( 431, 53 );
            this.CreditCardPanel.TabIndex = 21;
            this.CreditCardPanel.Visible = false;
            // 
            // otherPanel
            // 
            this.otherPanel.Controls.Add( this.lblTotalAmt );
            this.otherPanel.Controls.Add( this.lblDollar6 );
            this.otherPanel.Controls.Add( this.lblCheck );
            this.otherPanel.Controls.Add( this.lblDollar5 );
            this.otherPanel.Controls.Add( this.mtbCheck );
            this.otherPanel.Controls.Add( this.lblCheckNumber );
            this.otherPanel.Controls.Add( this.mtbCheckNumber );
            this.otherPanel.Controls.Add( this.mtbMoneyOrder );
            this.otherPanel.Controls.Add( this.lblCash );
            this.otherPanel.Controls.Add( this.mtbCash );
            this.otherPanel.Controls.Add( this.lblMoneyOrder );
            this.otherPanel.Controls.Add( this.lblDollar4 );
            this.otherPanel.Controls.Add( this.lblTotal );
            this.otherPanel.Controls.Add( this.label3 );
            this.otherPanel.Location = new System.Drawing.Point( 0, 240 );
            this.otherPanel.Name = "otherPanel";
            this.otherPanel.Size = new System.Drawing.Size( 431, 125 );
            this.otherPanel.TabIndex = 22;
            // 
            // lblTotalAmt
            // 
            this.lblTotalAmt.Location = new System.Drawing.Point( 124, 83 );
            this.lblTotalAmt.Name = "lblTotalAmt";
            this.lblTotalAmt.Size = new System.Drawing.Size( 77, 16 );
            this.lblTotalAmt.TabIndex = 24;
            this.lblTotalAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDollar6
            // 
            this.lblDollar6.Location = new System.Drawing.Point( 106, 60 );
            this.lblDollar6.Name = "lblDollar6";
            this.lblDollar6.Size = new System.Drawing.Size( 12, 15 );
            this.lblDollar6.TabIndex = 23;
            this.lblDollar6.Text = "$";
            // 
            // RecordPaymentView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.otherPanel );
            this.Controls.Add( this.CreditCardPanel );
            this.Controls.Add( this.rdNo );
            this.Controls.Add( this.rdYes );
            this.Controls.Add( this.label12 );
            this.Controls.Add( this.mtbCreditCard1 );
            this.Controls.Add( this.mtbReceiptNbr );
            this.Controls.Add( this.cmbCardType1 );
            this.Controls.Add( this.lblCardType );
            this.Controls.Add( this.lblDollar1 );
            this.Controls.Add( this.lblCreditCard1 );
            this.Controls.Add( this.lblInfo2 );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.lblPaymentMethod );
            this.Controls.Add( this.lblInfo );
            this.Controls.Add( this.lblReceipt );
            this.Name = "RecordPaymentView";
            this.Size = new System.Drawing.Size( 431, 383 );
            this.Load += new System.EventHandler( this.RecordPaymentView_Load );
            this.CreditCardPanel.ResumeLayout( false );
            this.CreditCardPanel.PerformLayout();
            this.otherPanel.ResumeLayout( false );
            this.otherPanel.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public RecordPaymentView()
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
		private Label lblInfo;
		private Label lblReceipt;
		private Label lblPaymentMethod;
		private Label label1;
		private Label lblInfo2;
		private Label lblCreditCard1;
		private Label lblCheck;
		private Label lblCash;
		private Label lblMoneyOrder;
		private Label lblTotal;
		private Label lblDollar4;
		private Label label3;
		private Label lblDollar1;
		private Label lblDollar2;
		private Label lblDollar5;
			private Label lblCardType;
		private Label lblCheckNumber;
		private ComboBox cmbCardType1;
		private MaskedEditTextBox mtbCheckNumber;
		private NumberFormatInfo i_NumberFormat = new CultureInfo( "en-US", true ).NumberFormat;
		private decimal i_CashAmt;
		private decimal i_CreditCardAmt1;
        private decimal i_CreditCardAmt2;
        private decimal i_CreditCardAmt3;
		private decimal i_MoneyOrderAmt;
		private decimal i_CheckAmt;
		private string i_CheckNumber;
		private string i_ReceiptNumber;
		private MaskedEditTextBox mtbReceiptNbr;
		private CreditCardProvider i_CreditCardProvider1;
        private CreditCardProvider i_CreditCardProvider2;
        private CreditCardProvider i_CreditCardProvider3;
		private MaskedEditTextBox mtbMoneyOrder;
		private MaskedEditTextBox mtbCash;
        private MaskedEditTextBox mtbCheck;
        private MaskedEditTextBox mtbCreditCard1;
        private MaskedEditTextBox mtbCreditCard2;
        private Label lblCreditCard2;
        private Label lblCreditCard3;
        private Label lblDollar3;
        private MaskedEditTextBox mtbCreditCard3;
        private Label lblCardType1;
        private Label lblCardType3;
        private ComboBox cmbCardType2;
        private ComboBox cmbCardType3;
        private Label label12;
        private RadioButton rdYes;
        private RadioButton rdNo;
        private Panel CreditCardPanel;
        private Panel otherPanel;
        private Label lblDollar6;

        private PaymentService i_PaymentService;

		#endregion

		#region Constants
        private const string MULITPLE_CREDIT_YES = "Y";
        private Label lblTotalAmt;
        private const string MULTIPLE_CREDIT_NO = "N";
		#endregion



    }
}

