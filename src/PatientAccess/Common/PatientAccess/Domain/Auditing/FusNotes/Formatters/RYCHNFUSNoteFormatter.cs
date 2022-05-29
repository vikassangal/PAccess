using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for RYCHNFUSNoteFormatter.
    /// </summary>
    [UsedImplicitly]
    public class RYCHNFUSNoteFormatter : FusFormatterStrategy
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
            if( account.Patient != null && account.Patient.PreviousName != null && account.Patient.Name != null &&
                 ( ( account.Patient.PreviousName.FirstName != account.Patient.Name.FirstName ) ||
                  ( account.Patient.PreviousName.LastName != account.Patient.Name.LastName ) ) )
            {
                formattedString = String.Format( FusLabel.NAME_CHANGED_FROM, account.Patient.PreviousName.AsFormattedNameWithSuffix(), account.Patient.Name.AsFormattedNameWithSuffix() );
            }
            nameValueList.Add( formattedString );
            return nameValueList;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RYCHNFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
  