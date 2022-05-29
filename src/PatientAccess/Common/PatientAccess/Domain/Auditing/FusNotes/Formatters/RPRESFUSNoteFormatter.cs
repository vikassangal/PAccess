using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    [UsedImplicitly]
    public class RPRESFUSNoteFormatter : FusFormatterStrategy
    {

        #region Methods

        public override IList Format()
        {
            FusNote note = Context;

            var account = (Account)note.Context;

            var messages = CreateFusNameValueList( account );

            return messages;
        }

        private ArrayList CreateFusNameValueList( Account account )
        {
            var nameValueList = new ArrayList();

            if ( account.RightToRestrict.Code == YesNoFlag.CODE_YES )
            {
                string formattedString = FormatNameValuePair( FusLabel.RIGHT_TO_RESTRICT, FusLabel.IS_SELECTED );
                nameValueList.Add( formattedString );
            }

            return nameValueList;
        }

        private new string FormatNameValuePair( string key, string val )
        {
            return key + FusLabel.LABEL_SPACE + val + FusLabel.LABEL_SPACE;
        }

        #endregion Methods

    }
}