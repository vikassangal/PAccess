using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
	/// <summary>
	/// Summary description for RFCACFUSNoteFormatter.
	/// </summary>
	[UsedImplicitly]
    public class RFCACFUSNoteFormatter : FusFormatterStrategy
	{
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            ArrayList messages = new ArrayList();
            string msg = string.Empty ;
            FusNote note  = this.Context  as FusNote ;
            string code = note.FusActivity.Code ;

            Account account = ( Account )note.Context;
            messages = this.CreateFusNameValueList( account );

            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( Account account )
        {
            ArrayList nameValueList = new ArrayList();

            if( account != null )
            {
                Payment payment = account.Payment;
                if( payment != null )
                {
                    decimal totalPaid = account.TotalPaid;
                    decimal amountCollectedToday = payment.CalculateTotalPayments();
                    decimal previousTotalPaid = totalPaid - amountCollectedToday;

                    if( totalPaid != previousTotalPaid )
                    {
                        string remarks = String.Format( 
                            FusLabel.TOTAL_AMT_COLLECTED_CHANGED, 
                            previousTotalPaid, 
                            account.TotalPaid );

                        nameValueList.Add(remarks);
                    }
                }
            }

            return nameValueList;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RFCACFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}