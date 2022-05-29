using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for REIFAFUSNoteFormatter.
    /// </summary>
    [UsedImplicitly]
    public class REIFAFUSNoteFormatter : FusFormatterStrategy
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

            ArrayList nameValueList = new ArrayList();
            string value = string.Empty;
            if( account.Insurance != null )
            {

                Coverage primaryCoverage = account.Insurance.PrimaryCoverage;
                Coverage secondaryCoverage = account.Insurance.SecondaryCoverage;

                if( primaryCoverage != null &&
                    primaryCoverage.InsurancePlan["IsTenetPlan"] != null &&
                    account.Insurance.HasChangedFor( "PrimaryCoverage" ) )
                {
                    value = FormatNameValuePair( PRIMARYPLANID, primaryCoverage.InsurancePlan.PlanID );
                }

                if( secondaryCoverage != null &&
                    secondaryCoverage.InsurancePlan["IsTenetPlan"] != null &&
                    account.Insurance.HasChangedFor( "SecondaryCoverage" ) )
                {
                    
                    if( value.Trim() != string.Empty )
                    {
                        value = value + FusLabel.LABEL_SEMI_COLON_SPACE;
                    }

                    value = value + FormatNameValuePair( SECONDARYPLANID, secondaryCoverage.InsurancePlan.PlanID );

                }
            }

            nameValueList.Add( value );

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
        public REIFAFUSNoteFormatter()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        private const string SECONDARYPLANID = "SECONDARY PLAN ID",
                             PRIMARYPLANID   = "PRIMARY PLAN ID" ;
        #endregion
    }
}