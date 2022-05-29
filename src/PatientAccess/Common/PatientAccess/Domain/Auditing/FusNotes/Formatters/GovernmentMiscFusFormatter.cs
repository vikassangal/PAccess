using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    //TODO: Create XML summary comment for GovernmentMiscFusFormatter
    [Serializable]
    [UsedImplicitly]
    public class GovernmentMiscFusFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            FusNote note = Context;
            string code = note.FusActivity.Code;

            GovernmentOtherCoverage goCov = note.Context as GovernmentOtherCoverage;
            GovernmentOtherCoverage origCov = note.Context2 as GovernmentOtherCoverage;

            CheckOriginalCoverage( goCov, origCov );

            ArrayList messages = CreateFusNameValueList( goCov, code );
            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( GovernmentOtherCoverage cov, string code )
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

            if( cov.HasChangedFor( "InformationReceivedSource" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INFO_RECEIVED_FROM, cov.InformationReceivedSource.Description ) );
                writeNote = true;
            }

            if( cov.ConstraintHasChangedFor( "EligibilityPhone" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.ELIGIBILITY_PHONE, cov.EligibilityPhone ) );
                writeNote = true;
            }

            if( cov.ConstraintHasChangedFor( "TypeOfCoverage" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.TYPE_OF_COVERAGE, cov.TypeOfCoverage ) );
                writeNote = true;
            }

            if( cov.ConstraintHasChangedFor( "EffectiveDateForInsured" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INSURED_EFF_DATE, cov.EffectiveDateForInsured.ToShortDateString() ) );
                writeNote = true;
            }

            if( cov.ConstraintHasChangedFor( "TerminationDateForInsured" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INSURED_TERM_DATE, cov.TerminationDateForInsured.ToShortDateString() ) );
                writeNote = true;
            }

            if( cov.ConstraintHasChangedFor( "InsuranceCompanyRepName" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INS_COMPANY_REP_NAME, cov.InsuranceCompanyRepName.Trim() ) );
                writeNote = true;
            }

            if( cov.BenefitsCategoryDetails.HaveVerificationDetailsChanged )
            {

                if( cov.BenefitsCategoryDetails.HasChangedFor( "Deductible" ) )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.DEDUCTIBLE, cov.BenefitsCategoryDetails.Deductible.ToString( "C" ) ) );
                    writeNote = true;
                }

                if( cov.BenefitsCategoryDetails.HasChangedFor( "DeductibleDollarsMet" ) )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.DOLLAR_MET, cov.BenefitsCategoryDetails.DeductibleDollarsMet.ToString( "C" ) ) );
                    writeNote = true;
                }

                if( cov.BenefitsCategoryDetails.HasChangedFor( "DeductibleMet" ) )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.IS_MET, cov.BenefitsCategoryDetails.DeductibleMet.Description ) );
                    writeNote = true;
                }

                if( cov.BenefitsCategoryDetails.HasChangedFor( "CoInsurance" ) )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.CO_INSURANCE, cov.BenefitsCategoryDetails.CoInsurance.ToString() ) );
                    writeNote = true;
                }

                if( cov.BenefitsCategoryDetails.HasChangedFor( "OutOfPocket" ) )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.OUT_OF_POCKET, cov.BenefitsCategoryDetails.OutOfPocket.ToString( "C" ) ) );
                    writeNote = true;
                }

                if( cov.BenefitsCategoryDetails.HasChangedFor( "OutOfPocketMet" ) )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.IS_MET, cov.BenefitsCategoryDetails.OutOfPocketMet.Description ) );
                    writeNote = true;
                }

                if( cov.BenefitsCategoryDetails.HasChangedFor( "OutOfPocketDollarsMet" ) )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.DOLLAR_MET, cov.BenefitsCategoryDetails.OutOfPocketDollarsMet.ToString( "C" ) ) );
                    writeNote = true;
                }

                if( cov.BenefitsCategoryDetails.HasChangedFor( "AfterOutOfPocketPercent" ) )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.PERCENTAGE_OUT_OF_POCKET, cov.BenefitsCategoryDetails.AfterOutOfPocketPercent.ToString() ) );
                    writeNote = true;
                }

                if( cov.BenefitsCategoryDetails.HasChangedFor( "CoPay" ) )
                {
                    nameValueList.Add( FormatNameValuePair( FusLabel.CO_PAY_AMOUNT, cov.BenefitsCategoryDetails.CoPay.ToString( "C" ) ) );
                    writeNote = true;
                }

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
