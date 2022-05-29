using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for RSSVSFUSNoteFormatter.
    /// </summary>
    [UsedImplicitly]
    public class RSSVSFUSNoteFormatter : FusFormatterStrategy
    {

        #region Methods 

        public override IList Format()
        {
            ArrayList messages = new ArrayList();
            string msg = string.Empty;
            FusNote note = this.Context as FusNote;
            string code = note.FusActivity.Code;

            Account account = (Account)note.Context;
            messages = this.CreateFusNameValueList( account );

            return messages;
        }

        private ArrayList CreateFusNameValueList( Account account )
        {
            string formattedString = String.Empty;
            ArrayList nameValueList = new ArrayList();
            if( account.ValuablesAreTaken.Code == YesNoFlag.CODE_YES )
            {
                formattedString = FormatNameValuePair( FusLabel.VALUABLES_COLLECTED, "Yes" );
                nameValueList.Add( formattedString );
            }
            return nameValueList;
        }

        private new string FormatNameValuePair( string key, string val )
        {
            return key + FusLabel.LABEL_COLON + val + FusLabel.LABEL_SPACE;
        }

		#endregion Methods 

    }
}