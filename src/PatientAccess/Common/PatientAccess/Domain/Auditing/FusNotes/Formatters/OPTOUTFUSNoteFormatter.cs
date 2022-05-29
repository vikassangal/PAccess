using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for OPTOUTFUSNoteFormatter.
    /// </summary>
    [UsedImplicitly]
    public class OptOutFUSNoteFormatter : FusFormatterStrategy
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
            if( account.OptOutName )
            {
                value = NAMEANDALLINFORMATION;
            }
            else
            {
                if( account.OptOutLocation )
                {
                    value = LOCATION;
                }
                if( account.OptOutHealthInformation )
                {
                    if( value.Trim() != string.Empty )
                    {
                        value = value + FusLabel.LABEL_SEMI_COLON_SPACE;
                    }
                    value = value + HEALTHINFORMATION;
                }
                if( account.OptOutReligion )
                {
                    if( value.Trim() != string.Empty )
                    {
                        value = value + FusLabel.LABEL_SEMI_COLON_SPACE;
                    }
                    value = value + RELIGION;
                }
            }
            if( value != string.Empty )
            {
                formattedString = FormatNameValuePair( FusLabel.OPT_OUT_OPTIONS_SELECTED, value );
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
        public OptOutFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        private const string NAMEANDALLINFORMATION = "Name and All Information" , 
                     RELIGION  = "RELIGION",
                     LOCATION  = "LOCATION" ,
                     HEALTHINFORMATION = "HEALTH INFORMATION";
        #endregion
    }
}