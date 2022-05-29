using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    /// <summary>
    /// Summary description for SelfPayFusFormatter.
    /// </summary>
    [UsedImplicitly]
    public class SelfPayFusFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            FusNote note = Context;
            string code = note.FusActivity.Code;

            SelfPayCoverage spCov = note.Context as SelfPayCoverage;
            SelfPayCoverage origCov = note.Context2 as SelfPayCoverage;

            CheckOriginalCoverage( spCov, origCov );

            ArrayList messages = CreateFusNameValueList( spCov, code );
            return messages;
        }
        #endregion

        #region Properties

        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( SelfPayCoverage cov, string code )
        {
            ArrayList nameValueList = new ArrayList();
            writeNote = false;

            if( code == RDOTVActivityCode )
            {
                if( cov.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                {
                    nameValueList.Add( FusLabel.PRIMARY_PAYOR_VERIFICATION );
                }
                else if( cov.CoverageOrder.Oid == CoverageOrder.SECONDARY_OID )
                {
                    nameValueList.Add( FusLabel.SECONDARY_PAYOR_VERIFICATION );
                }

                if( cov.InsurancePlan != null )
                {
                    if( cov.InsurancePlan.Payor != null )
                    {
                        nameValueList.Add( FormatNameValuePair( FusLabel.PAYOR_NAME, cov.InsurancePlan.Payor.Name ) );
                    }
                }
                writeNote = true;

            }
            else if( cov.InsurancePlan != null )
            {
                if( cov.InsurancePlan.Payor != null )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.PAYOR_NAME, cov.InsurancePlan.Payor.Name ) );
                }
            }

            if( cov.HasChangedFor( "PatientHasMedicaid" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.PATIENT_HAS_MEDICAID, cov.PatientHasMedicaid.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "InsuranceInfoUnavailable" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INS_INFO_UNAVAILABLE, cov.InsuranceInfoUnavailable.Description ) );
                writeNote = true;
            }

            if( writeNote )
            {
                return nameValueList;
            }

            return new ArrayList();
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        private bool writeNote;
        #endregion

        #region Constants
        #endregion
    }
}