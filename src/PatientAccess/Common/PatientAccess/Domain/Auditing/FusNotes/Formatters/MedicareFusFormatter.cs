using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    //TODO: Create XML summary comment for MedicareFusFormatter
    [Serializable]
    [UsedImplicitly]
    public  class MedicareFusFormatter : FusFormatterStrategy
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

            GovernmentMedicareCoverage mcCov = note.Context as GovernmentMedicareCoverage ;
            GovernmentMedicareCoverage origCov = note.Context2 as GovernmentMedicareCoverage;

            base.CheckOriginalCoverage( mcCov, origCov );

            messages = this.CreateFusNameValueList( mcCov, origCov, code );
            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( GovernmentMedicareCoverage cov, GovernmentMedicareCoverage origCov, string code )
        {
            string formattedString = String.Empty;
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
                        nameValueList.Add( this.FormatNameValuePair( FusLabel.PAYOR_NAME, cov.InsurancePlan.Payor.Name ) );
                    }
                }
                writeNote = true;

            }
            else if( cov.InsurancePlan != null )
            {
                if( cov.InsurancePlan.Payor != null )
                {
                    nameValueList.Add( this.FormatNameValuePair( FusLabel.PAYOR_NAME, cov.InsurancePlan.Payor.Name ) );
                }
            }

            if( cov.ConstraintHasChangedFor("PartACoverage") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.PART_A_COVERAGE, cov.PartACoverage.Description ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("PartACoverageEffectiveDate") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.PART_A_EFFECTIVE_DATE, cov.PartACoverageEffectiveDate.ToShortDateString() ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("PartBCoverage") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.PART_B_COVERAGE, cov.PartBCoverage.Description ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("PartBCoverageEffectiveDate") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.PART_B_EFFECTIVE_DATE, cov.PartBCoverageEffectiveDate.ToShortDateString() ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("PatientHasMedicareHMOCoverage") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.HAS_MEDICARE_HMO, cov.PatientHasMedicareHMOCoverage.Description ) );
                writeNote = true;
            }
            if( cov.HasChangedFor("MedicareIsSecondary") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.IS_MEDICARE_SECONDARY, cov.MedicareIsSecondary.Description ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("DateOfLastBillingActivity") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.LAST_BILLING_DATE, cov.DateOfLastBillingActivity.ToShortDateString() ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("RemainingBenefitPeriod") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.REMAINING_HOSP_DAYS, cov.RemainingBenefitPeriod.ToString() ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("RemainingCoInsurance") )
            {
                formattedString = FormatNameValuePair( FusLabel.REMAINING_CO_INS_DAYS, cov.RemainingCoInsurance.ToString() );
                nameValueList.Add( formattedString );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("RemainingLifeTimeReserve") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.REMAINING_LIFITIME_RESV, cov.RemainingLifeTimeReserve.ToString() ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("RemainingSNF") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.REMAINING_SNF_DAYS, cov.RemainingSNF.ToString() ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("RemainingSNFCoInsurance") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.REMAINING_SNF_CO_INS_DAYS, cov.RemainingSNFCoInsurance.ToString() ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("PatientIsPartOfHospiceProgram" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.IS_PATIENT_IN_HOSPICE, cov.PatientIsPartOfHospiceProgram.Description ) );
                writeNote = true;
            }
            if( cov.HasChangedFor("VerifiedBeneficiaryName") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.VERIFIED_BENEFICIARY_NAME, cov.VerifiedBeneficiaryName.Description ) );
                writeNote = true;
            }
            if( cov.HasChangedFor("InformationReceivedSource") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INFO_RECEIVED_FROM, cov.InformationReceivedSource.Description ) );
                writeNote = true;
            }
            if( cov.HasChangedFor("Remarks") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.LABEL_REMARKS, cov.Remarks ) );
                writeNote = true;
            }
            if( cov.ConstraintHasChangedFor("RemainingPartADeductible") )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.REMAINING_PART_A_DEDUCTIBLE, cov.RemainingPartADeductible.ToString( "C" ) ) );
                writeNote = true;
            }
            if (cov.ConstraintHasChangedFor("RemainingPartBDeductible"))
            {
                nameValueList.Add(FormatNameValuePair( FusLabel.REMAINING_PART_B_DEDUCTIBLE, cov.RemainingPartBDeductible.ToString( "C" ) ) );
                writeNote = true;
            }

            return this.writeNote ? nameValueList : new ArrayList();
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MedicareFusFormatter()
        {
        }
        #endregion

        #region Data Elements
        private bool writeNote = false;
        #endregion

        #region Constants
        #endregion
    }
}
