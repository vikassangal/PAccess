using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    //TODO: Create XML summary comment for MedicaidFusFormatter
    [Serializable]
    [UsedImplicitly]
    public class MedicaidFusFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            FusNote note = Context;
            string code = note.FusActivity.Code;

            GovernmentMedicaidCoverage mdCov = note.Context as GovernmentMedicaidCoverage;
            GovernmentMedicaidCoverage origCov = note.Context2 as GovernmentMedicaidCoverage;

            CheckOriginalCoverage( mdCov, origCov );

            ArrayList messages = CreateFusNameValueList( mdCov, code );
            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( GovernmentMedicaidCoverage cov, string code )
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

            if( cov.ConstraintChangedFor( "EligibilityDate" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.ELIGIBILITY_DATE, cov.EligibilityDate.ToShortDateString() ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "PatienthasMedicare" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.PATIENT_HAS_MEDICARE, cov.PatienthasMedicare.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "PatienthasOtherInsuranceCoverage" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.THIRD_PARTY_INSURANCE, cov.PatienthasOtherInsuranceCoverage.Description ) );
                writeNote = true;
            }

            if( cov.ConstraintChangedFor( "MedicaidCopay" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.CO_PAY_AMOUNT, cov.MedicaidCopay.ToString( "C" ) ) );
                writeNote = true;
            }

            if( cov.ConstraintChangedFor( "EVCNumber" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.EVC_NUMBER, cov.EVCNumber ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "InformationReceivedSource" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INFO_RECEIVED_FROM, cov.InformationReceivedSource.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "Remarks" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.LABEL_REMARKS, cov.Remarks ) );
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
