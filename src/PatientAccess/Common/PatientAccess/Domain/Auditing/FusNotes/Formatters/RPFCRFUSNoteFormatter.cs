using System;
using System.Collections;
using PatientAccess.Annotations;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for RPFCRFUSNoteFormatter.
    /// </summary>
    //TODO: Create XML summary comment for RPFCRFUSNoteFormatter
    [Serializable]
    [UsedImplicitly]
    public class RPFCRFUSNoteFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            ArrayList messages = new ArrayList();
            string msg = string.Empty;
            FusNote note = this.Context as FusNote;
            string code = note.FusActivity.Code;

            Account account = (Account)note.Context;

            // Write RPFCR FUS Notes except if activity is of type PreMSERegistration
            if ((account.Activity != null) &&
                (account.Activity.GetType() == typeof(PreMSERegisterActivity)) ||
                (account.Activity.GetType() == typeof(UCCPreMSERegistrationActivity)))
            {
                return messages;
            }

            messages = this.CreateFusNameValueList(account);
            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList(Account account)
        {
            ArrayList nameValueList = new ArrayList();
            string formattedString = String.Empty;

            // Heading Information
            formattedString = FormatNameValuePair(FusLabel.TRANSACTION_DATE, DateTime.Now.ToLongDateString());
            nameValueList.Add(formattedString);

            if (account != null)
            {
                formattedString = FormatNameValuePair(FusLabel.ACCOUNT_NUMBER,
                    account.AccountNumber.ToString());
                nameValueList.Add(formattedString);

                if (account.Patient != null)
                {
                    formattedString = FormatNameValuePair(FusLabel.PATIENT_NAME,
                        account.Patient.Name.AsFormattedName());
                    nameValueList.Add(formattedString);
                }

                // Amount of Payment Received
                Payment payment = account.Payment;
                if (payment != null)
                {
                    if (payment.ReceiptNumber != null && payment.ReceiptNumber != String.Empty)
                    {
                        formattedString = FormatNameValuePair(FusLabel.CASH_RECEIPT_NUMBER, payment.ReceiptNumber);
                        nameValueList.Add(formattedString);
                    }

                   

                    if( payment.CardType( Payment.PaymentType.CreditCard1 ) != String.Empty )
                    {
                        formattedString = FormatNameValuePair(FusLabel.CREDIT_CARD_TYPE, payment.CardType(Payment.PaymentType.CreditCard1 ));
                        nameValueList.Add( formattedString );

                        decimal ccPayment1 = payment.AmountPaidWith(Payment.PaymentType.CreditCard1);
                        if (ccPayment1 != 0)
                        {
                            formattedString = FormatNameValuePair(FusLabel.CREDIT_CARD_PAYMENT, ccPayment1.ToString());
                            nameValueList.Add(formattedString);
                        }
                    }

                    
                    if( payment.CardType( Payment.PaymentType.CreditCard2 ) != String.Empty )
                    {
                        formattedString = FormatNameValuePair( FusLabel.CREDIT_CARD_TYPE1, payment.CardType( Payment.PaymentType.CreditCard2 ) );
                        nameValueList.Add( formattedString );
                        decimal ccPayment2 = payment.AmountPaidWith(Payment.PaymentType.CreditCard2);
                        if (ccPayment2 != 0)
                        {
                            formattedString = FormatNameValuePair(FusLabel.CREDIT_CARD_PAYMENT1, ccPayment2.ToString());
                            nameValueList.Add(formattedString);
                        }
                    }

                    
                    if( payment.CardType( Payment.PaymentType.CreditCard3 ) != String.Empty )
                    {
                        formattedString = FormatNameValuePair(FusLabel.CREDIT_CARD_TYPE2, payment.CardType( Payment.PaymentType.CreditCard3 ));
                        nameValueList.Add( formattedString);

                        decimal ccPayment3 = payment.AmountPaidWith(Payment.PaymentType.CreditCard3);
                        if (ccPayment3 != 0)
                        {
                            formattedString = FormatNameValuePair(FusLabel.CREDIT_CARD_PAYMENT2, ccPayment3.ToString());
                            nameValueList.Add(formattedString);
                        }
                    }

                    decimal ckPayment = payment.AmountPaidWith(Payment.PaymentType.Check);
                    if (ckPayment != 0)
                    {
                        formattedString = FormatNameValuePair(FusLabel.CHECK_PAYMENT, ckPayment.ToString());
                        nameValueList.Add(formattedString);
                    }

                    if( payment.CheckNumber() != null && payment.CheckNumber() != String.Empty )
                    {
                        formattedString = FormatNameValuePair(FusLabel.CHECK_NUMBER, payment.CheckNumber());
                        nameValueList.Add(formattedString);
                    }

                    decimal cashPayment = payment.AmountPaidWith(Payment.PaymentType.Cash);
                    if (cashPayment != 0)
                    {
                        formattedString = FormatNameValuePair(FusLabel.CASH_PAYMENT, cashPayment.ToString());
                        nameValueList.Add(formattedString);
                    }

                    decimal moPayment = payment.AmountPaidWith(Payment.PaymentType.MoneyOrder);
                    if (moPayment != 0)
                    {
                        formattedString = FormatNameValuePair(FusLabel.MONEY_ORDER_PAYMENT, moPayment.ToString());
                        nameValueList.Add(formattedString);
                    }

                    decimal totalPayment = payment.CalculateTotalPayments();
                    if (totalPayment != 0)
                    {
                        formattedString = FormatNameValuePair(FusLabel.SUM_OF_TODAYS_PAYMENT, totalPayment.ToString());
                        nameValueList.Add(formattedString);
                    }

                    if ((totalPayment == 0M || totalPayment == 0.00M) &&
                        (payment.ZeroPaymentReason != null)
                        //&& ( payment.ZeroPaymentReason.Description != String.Empty ) 
                        )
                    {
                        formattedString = FormatNameValuePair(FusLabel.ZERO_PAYMENT_REASON,
                            payment.ZeroPaymentReason.Description);
                        nameValueList.Add(formattedString);
                    }

                    // Payment Arrangements (Only for current account payment)
                    if (payment.IsCurrentAccountPayment)
                    {
                        if (account.NumberOfMonthlyPayments != -1)
                        {
                            formattedString = FormatNameValuePair(FusLabel.NUMBER_OF_PAYMENTS,
                                account.NumberOfMonthlyPayments.ToString());
                            nameValueList.Add(formattedString);
                        }
                        if (account.MonthlyPayment != 0M || account.MonthlyPayment != 0.00M)
                        {
                            formattedString = FormatNameValuePair(FusLabel.MONTHLY_PAYMENT_AMOUNT,
                                account.MonthlyPayment.ToString());
                            nameValueList.Add(formattedString);
                        }
                        if (account.DayOfMonthPaymentDue != null || account.DayOfMonthPaymentDue != String.Empty)
                        {
                            formattedString = FormatNameValuePair(FusLabel.MONTHLY_DUE_DATE, account.DayOfMonthPaymentDue);
                            nameValueList.Add(formattedString);
                        }
                    }
                }

                // User Details
                Activity activity = account.Activity;
                if (activity != null)
                {
                    if (activity.AppUser != null && activity.AppUser.Name != null)
                    {
                        formattedString = FormatNameValuePair(FusLabel.LOGGED_IN_USER_NAME, activity.AppUser.Name.AsFormattedName());
                        nameValueList.Add(formattedString);
                    }
                }
            }

            return nameValueList;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RPFCRFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
