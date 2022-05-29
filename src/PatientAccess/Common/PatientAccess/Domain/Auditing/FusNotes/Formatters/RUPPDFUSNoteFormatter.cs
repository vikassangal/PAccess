using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
	/// <summary>
	/// Summary description for RCALCFUSNoteFormatter.
	/// </summary>
	[UsedImplicitly]
    public class RUPPDFUSNoteFormatter : FusFormatterStrategy
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
            string remarks = String.Empty;
            ArrayList nameValueList = new ArrayList();

            decimal totalCurrentAmountDue = account.TotalCurrentAmtDue;
            remarks =FormatNameValuePair(  FusLabel.TOTAL_CURRENT_AMOUNT_DUE,
                totalCurrentAmountDue.ToString() );
            nameValueList.Add( remarks );

            return nameValueList;
        }

        private new string FormatNameValuePair( string key, string val )
        {
            return key + FusLabel.LABEL_COLON + val + FusLabel.LABEL_SPACE;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RUPPDFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}