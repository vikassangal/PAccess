using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
	/// <summary>
	/// Summary description for RCALCFUSNoteFormatter.
	/// </summary>
    [UsedImplicitly]
    public class IRATAFUSNoteFormatter : FusFormatterStrategy
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
            
            if( account != null && account.Insurance != null )
            {

                CoverageGroup primaryCoverage = account.Insurance.PrimaryCoverage as CoverageGroup;
                
                if( primaryCoverage != null )
                {

                    if( primaryCoverage.Authorization.AuthorizationStatus.Code == "A" )
                    {

                        formattedString = FormatNameValuePair( FusLabel.AUTHORIZATION_NUMBER, primaryCoverage.Authorization.AuthorizationNumber );
                        nameValueList.Add( formattedString );

                    }

                }
                else
                {
                    var medicareCoverage = account.Insurance.PrimaryCoverage as GovernmentMedicareCoverage;
                    if (medicareCoverage != null && medicareCoverage.IsMedicareCoverageValidForAuthorization)
                    {
                        if (medicareCoverage.Authorization.AuthorizationStatus.Code == "A")
                        {

                            formattedString = FormatNameValuePair(FusLabel.AUTHORIZATION_NUMBER, medicareCoverage.Authorization.AuthorizationNumber);
                            nameValueList.Add(formattedString);

                        }

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
        public IRATAFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}