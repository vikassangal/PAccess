using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
	/// <summary>
	/// Summary description for RCALCFUSNoteFormatter.
	/// </summary>
	[UsedImplicitly]
    public class RCALCFUSNoteFormatter : FusFormatterStrategy
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
                decimal prevTotalCurrentAmountDue = account.PreviousTotalCurrentAmtDue;
                decimal totalCurrentAmountDue = account.TotalCurrentAmtDue;

                if( totalCurrentAmountDue != prevTotalCurrentAmountDue )
                {
                    string remarks = String.Format( FusLabel.TOTAL_AMOUNT_DUE_CHANGED, 
                        prevTotalCurrentAmountDue, totalCurrentAmountDue );
                

                    nameValueList.Add(remarks);
                }
            }

            return nameValueList;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RCALCFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}