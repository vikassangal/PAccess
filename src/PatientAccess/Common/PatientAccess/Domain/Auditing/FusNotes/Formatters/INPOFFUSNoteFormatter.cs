using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for INPOFFUSNoteFormatter.
    /// </summary>
    [UsedImplicitly]
    public class INPOFFUSNoteFormatter : FusFormatterStrategy
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
            string value = string.Empty ;
            SignatureStatus signatureStatus =
                account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus;

            if( signatureStatus.IsSignedStatus() )
            {
                value = account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate.ToShortDateString();
            }
            if( value != string.Empty )
            {
                formattedString = FormatNameValuePair( FusLabel.NPPSIGNED, value );
                nameValueList.Add( formattedString );
            }
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
        public INPOFFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}