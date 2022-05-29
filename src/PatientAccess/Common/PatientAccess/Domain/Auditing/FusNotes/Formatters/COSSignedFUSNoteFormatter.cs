using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for COSSignedFUSNoteFormatter.
    /// </summary>
    [UsedImplicitly]
    public class COSSignedFUSNoteFormatter : FusFormatterStrategy
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
            messages = this.CreateFusNameValueList( account,code );

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
            if( account != null && account.COSSigned != null &&
                account.COSSigned.Code != null &&
                !account.COSSigned.Code.Trim().Equals( string.Empty ) )
            {
                string cosSigned = account.COSSigned.Code;
                if( ( fuscode == ICOSCActivityCode ) ||
                    ( fuscode == IICOSActivityCode ) )
                {
                    if( cosSigned ==  ConditionOfService.YES )
                    {
                        value = ConditionOfService.YES_DESCRIPTION;
                    }

                    if( cosSigned ==  ConditionOfService.REFUSED )
                    {
                        value = ConditionOfService.REFUSED_DESCRIPTION;
                    }
                    else if( cosSigned ==  ConditionOfService.UNABLE )
                    {
                        value = ConditionOfService.UNABLE_DESCRIPTION;
                    }
                    else if( cosSigned ==  ConditionOfService.NOT_AVAILABLE )
                    {
                        value = ConditionOfService.NOT_AVAILABLE_DESCRIPTION;
                    }
                    if( value != string.Empty )
                    {
                        formattedString = FormatNameValuePair( FusLabel.COSSIGNED, value );
                        nameValueList.Add( formattedString );
                    }
                }
                else
                {
                    if( cosSigned ==  ConditionOfService.REFUSED ||
                        cosSigned ==  ConditionOfService.UNABLE ||
                        cosSigned ==  ConditionOfService.NOT_AVAILABLE )
                    {
                        nameValueList.Add( FusLabel.COSNOTSIGNED );
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
        public COSSignedFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const string ICOSCActivityCode = "ICOSC",
                             IICOSActivityCode = "IICOS";

        #endregion
    }
}