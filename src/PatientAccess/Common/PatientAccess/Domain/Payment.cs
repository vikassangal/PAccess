using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Payment : PersistentModel
    {
        #region Event Handlers
        #endregion

		#region Methods
		public void AddPayment( PaymentAmount aPaymentAmount,PaymentType paymentTypeName )
		{
			Type paymentType = aPaymentAmount.PaymentMethod.GetType();
		//	decimal anAmount = aPaymentAmount.AmountPaid();

            switch (paymentType.Name)
            {
                case "CashPayment":
                    this.i_PaymentAmounts.Remove(CASH_PAYMENT);
                    this.i_PaymentAmounts.Add(CASH_PAYMENT, aPaymentAmount);
                    break;

                case "CheckPayment":
                    this.i_PaymentAmounts.Remove(CHECK_PAYMENT);
                    this.i_PaymentAmounts.Add(CHECK_PAYMENT, aPaymentAmount);
                    break;

				case "CreditCardPayment":
                    
                    if( paymentTypeName == PaymentType.CreditCard1 )
                    {
                        this.i_PaymentAmounts.Remove( CREDIT_CARD_PAYMENT1 );
                        this.i_PaymentAmounts.Add( CREDIT_CARD_PAYMENT1, aPaymentAmount );
                    }
                    if( paymentTypeName == PaymentType.CreditCard2 )
                    {
                        this.i_PaymentAmounts.Remove( CREDIT_CARD_PAYMENT2 );
                        this.i_PaymentAmounts.Add( CREDIT_CARD_PAYMENT2, aPaymentAmount );
                    }
                    if (paymentTypeName == PaymentType.CreditCard3)
                    {
                        this.i_PaymentAmounts.Remove(CREDIT_CARD_PAYMENT3);
                        this.i_PaymentAmounts.Add(CREDIT_CARD_PAYMENT3, aPaymentAmount);
                    }
                   
                 
                 	break;

                case "MoneyOrderPayment":
                    this.i_PaymentAmounts.Remove(MONEY_ORDER_PAYMENT);
                    this.i_PaymentAmounts.Add(MONEY_ORDER_PAYMENT, aPaymentAmount);
                    break;
                default:
                    break;
            }

            this.TotalRecordedPayment = this.TotalPayment + this.CalculateTotalPayments();
        }

        public decimal AmountPaidWith(PaymentType aPaymentType)
        {
            return ((PaymentAmount)this.i_PaymentAmounts[(int)aPaymentType]).AmountPaid();
        }

        public string CheckNumber()
        {
            //			CheckPayment aCheckPayment = (CheckPayment)((PaymentAmount)this.i_PaymentAmounts[(int)PaymentType.Check]).PaymentMethod;
            //			string checkNumber = aCheckPayment.CheckNumber;
            return ((CheckPayment)((PaymentAmount)this.i_PaymentAmounts[(int)PaymentType.Check]).PaymentMethod).CheckNumber;
        }

		public decimal CalculateTotalPayments()
		{
            return this.AmountPaidWith( PaymentType.Cash ) +
                this.AmountPaidWith( PaymentType.Check ) +
                this.AmountPaidWith( PaymentType.CreditCard1 ) +
                this.AmountPaidWith( PaymentType.MoneyOrder ) +
                this.AmountPaidWith( PaymentType.CreditCard2 ) +
                this.AmountPaidWith( PaymentType.CreditCard3 );
		}

        #region Properties
        public string ReceiptNumber
        {
            get
            {
                return i_ReceiptNumber;
            }
            set
            {
                i_ReceiptNumber = value;
            }
        }

		public string CardType(PaymentType paymentType) 
		{
            if( paymentType == PaymentType.CreditCard1 || paymentType == PaymentType.CreditCard2 || paymentType == PaymentType.CreditCard3 )
            {
                return ((CreditCardPayment)((PaymentAmount)this.i_PaymentAmounts[(int)paymentType]).PaymentMethod).CardType.Description;
            }
           else
            {
                return String.Empty;
            }

       
		}
		#endregion

        public decimal TotalRecordedPayment
        {
            get
            {
                return i_TotalRecordedPayment;
            }
            private set
            {
                i_TotalRecordedPayment = value;
            }
        }

        public decimal TotalPayment
        {
            get
            {
                return i_TotalPayment;
            }
            set
            {
                i_TotalPayment = value;
            }
        }

        public ZeroPaymentReason ZeroPaymentReason
        {
            get
            {
                return i_ZeroPaymentReason;
            }
            set
            {
                i_ZeroPaymentReason = value;

            }
        }

        public bool IsCurrentAccountPayment
        {
            get
            {
                return i_IsCurrentAccountPayment;
            }
            set
            {
                i_IsCurrentAccountPayment = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

		#region Construction and Finalization
		public Payment()
		{
			this.i_PaymentAmounts.Add( CASH_PAYMENT, new PaymentAmount( 0.00m, new CashPayment() ) );
			this.i_PaymentAmounts.Add( CHECK_PAYMENT, new PaymentAmount( 0.00m, new CheckPayment() ) );
			this.i_PaymentAmounts.Add( CREDIT_CARD_PAYMENT3, new PaymentAmount( 0.00m, new CreditCardPayment() ) );
			this.i_PaymentAmounts.Add( MONEY_ORDER_PAYMENT, new PaymentAmount( 0.00m, new MoneyOrderPayment() ) );
            this.i_PaymentAmounts.Add( CREDIT_CARD_PAYMENT1, new PaymentAmount( 0.00m, new CreditCardPayment() ) );
            this.i_PaymentAmounts.Add( CREDIT_CARD_PAYMENT2, new PaymentAmount(0.00m, new CreditCardPayment()) );
		}		
		#endregion

        #region Data Elements
        private string i_ReceiptNumber = string.Empty;
        private decimal i_TotalRecordedPayment;
        private decimal i_TotalPayment;
        private ZeroPaymentReason i_ZeroPaymentReason = new ZeroPaymentReason();
        private Hashtable i_PaymentAmounts = new Hashtable();
        private bool i_IsCurrentAccountPayment = false;
        #endregion

		#region Constants
        private const int
            CASH_PAYMENT = 0,
            CHECK_PAYMENT = 1,
            CREDIT_CARD_PAYMENT1 = 2,
            MONEY_ORDER_PAYMENT = 3,
            CREDIT_CARD_PAYMENT2 = 4,
            CREDIT_CARD_PAYMENT3 = 5;

		public enum PaymentType
		{
			Cash,
			Check,
			CreditCard1,
			MoneyOrder,
            CreditCard2,
            CreditCard3
          
		}
		#endregion
	}
}
