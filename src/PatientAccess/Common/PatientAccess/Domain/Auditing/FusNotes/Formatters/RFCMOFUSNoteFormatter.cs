using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
	/// <summary>
	/// Summary description for RFCMOFUSNoteFormatter.
	/// </summary>
	[UsedImplicitly]
    public class RFCMOFUSNoteFormatter : FusFormatterStrategy
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
            string formattedString = String.Empty;
            ArrayList nameValueList = new ArrayList();

            if( account != null )
            {
                decimal originalPayment = account.OriginalMonthlyPayment;
                decimal monthlyPayment = account.MonthlyPayment;
                if( originalPayment != monthlyPayment && monthlyPayment != 0)
                {
//                    formattedString = FormatNameValuePair( FusLabel.DOLLAR_AMOUNT1, monthlyPayment.ToString() );
//                    nameValueList.Add( formattedString );

                    string remarks = String.Format( FusLabel.TOTAL_MONTHLY_DUE_CHANGED, 
                        originalPayment, monthlyPayment );
                
//                    formattedString = FormatNameValuePair( FusLabel.LABEL_REMARKS, remarks );
//                    nameValueList.Add( formattedString );
                    nameValueList.Add(remarks);
                }
            }

            return nameValueList;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RFCMOFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}