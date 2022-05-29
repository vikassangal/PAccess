using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
	/// <summary>
	/// Summary description for RFCNLFUSNoteFormatter.
	/// </summary>
	[UsedImplicitly]
    public class RFCNLFUSNoteFormatter : FusFormatterStrategy
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
                decimal totalCurrentAmountDue = account.TotalCurrentAmtDue;
                decimal balanceDue = account.BalanceDue;

                // When 'Patient has No Liability' check box is checked on Liability screen
                // (or)
                // When Total Current Amount Due has become zero
                if(  account.Insurance.HasNoLiability ||
                    ( account.PreviousTotalCurrentAmtDue != 0 && 
                    ( totalCurrentAmountDue == 0M || totalCurrentAmountDue == 0.00M ) ) )
                {
                    string remarks = String.Format( FusLabel.PATIENT_LIABILITY_CHANGED, 
                        account.PreviousTotalCurrentAmtDue );

                    nameValueList.Add(remarks);
                }
            }

            return nameValueList;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RFCNLFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
