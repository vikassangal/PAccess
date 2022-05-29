using System;
using System.Collections;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for COSSignedFUSNoteFormatter.
    /// </summary>
    public class RCRPFUSNoteFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            ArrayList messages = new ArrayList();
            FusNote note = Context as FusNote;
            string code = note.FusActivity.Code;

            Account account = ( Account )note.Context;
            messages = CreateFusNameValueList( account, code );

            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( Account account, string fuscode )
        {
            string formattedString = String.Empty;
            ArrayList nameValueList = new ArrayList();
            string value = string.Empty;
            if ( account != null && account.RightCareRightPlace != null && account.RightCareRightPlace.LeftOrStayed != null &&
                 account.RightCareRightPlace.LeftOrStayed.Code != null &&
                 !account.RightCareRightPlace.LeftOrStayed.Code.Trim().Equals( LeftOrStayed.BLANK ) )
            {
                string leftOrStayed = account.RightCareRightPlace.LeftOrStayed.Code;
                if ( ( fuscode == RCRPL_ACTIVITY_CODE ) ||
                     ( fuscode == RCRPS_ACTIVITY_CODE ) )
                {
                    if ( leftOrStayed == LeftOrStayed.LEFT )
                    {
                        value = LeftOrStayed.LEFT_DESCRIPTION;
                    }
                    else if ( leftOrStayed == LeftOrStayed.STAYED )
                    {
                        value = LeftOrStayed.STAYED_DESCRIPTION;
                    }
                    
                    if (  value != string.Empty  )
                    {
                        formattedString = FormatNameValuePair( FusLabel.RCRP, value  );
                        nameValueList.Add( formattedString );
                    }
                }
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
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string RCRPL_ACTIVITY_CODE = "RCRPL";
        private const string RCRPS_ACTIVITY_CODE = "RCRPS";

        #endregion
    }
}