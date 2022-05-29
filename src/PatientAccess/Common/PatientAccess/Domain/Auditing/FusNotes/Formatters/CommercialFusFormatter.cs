using System;
using System.Collections;
using PatientAccess.Annotations;

namespace PatientAccess.Domain.Auditing.FusNotes.Formatters
{
    //TODO: Create XML summary comment for CommercialFusFormatter
    [Serializable]
    [UsedImplicitly]
    public class CommercialFusFormatter : FusFormatterStrategy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override IList Format()
        {
            FusNote note = Context;
            string code = note.FusActivity.Code;

            CommercialCoverage cCov = note.Context as CommercialCoverage;
            CommercialCoverage origCov = note.Context2 as CommercialCoverage;

            CheckOriginalCoverage( cCov, origCov );

            ArrayList messages = CreateFusNameValueList( cCov, origCov, code );
            return messages;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private ArrayList CreateFusNameValueList( CommercialCoverage cov, CommercialCoverage origCov, string code )
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

            if( cov.Attorney.HasChangedFor( "AttorneyName" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.ATTORNEYNAME, cov.Attorney.AttorneyName ) );
                writeNote = true;
            }

            ContactPoint attorneyInfo =
                cov.Attorney.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );

            if( attorneyInfo.PhoneNumber.HasChangedFor( "AreaCode" ) || attorneyInfo.PhoneNumber.HasChangedFor( "Number" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.ATTORNEYPHONE, attorneyInfo.PhoneNumber.AsFormattedString() ) );
                writeNote = true;
            }

            if( attorneyInfo.HasChangedFor( "Address" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.ATTORNEYADDRESS, attorneyInfo.Address.OneLineAddressLabel() ) );
                writeNote = true;
            }

            ContactPoint agentInfo =
                cov.InsuranceAgent.ContactPointWith( TypeOfContactPoint.NewBusinessContactPointType() );


            if( cov.InsuranceAgent.HasChangedFor( "AgentName" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INS_COMPANY_REP_NAME, cov.InsuranceAgent.AgentName ) );
                writeNote = true;
            }

            if( agentInfo.HasChangedFor( "Address" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INS_COMPANY_REP_ADDRESS, agentInfo.Address.OneLineAddressLabel() ) );
                writeNote = true;
            }

            if( agentInfo.PhoneNumber.HasChangedFor( "AreaCode" ) || agentInfo.PhoneNumber.HasChangedFor( "Number" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.INS_COMPANY_REP_PHONE, agentInfo.PhoneNumber.AsFormattedString() ) );
                writeNote = true;
            }


            Hashtable benefitsCategories = cov.BenefitsCategories;
            ICollection categories = new ArrayList();
            if( benefitsCategories.Count > 0 )
            {
                categories = benefitsCategories.Keys;
            }

            Hashtable origBenefitsCategories = origCov.BenefitsCategories;

            //TODO-AC remove the variable below as per resharper analysis
            ICollection origCategories = new ArrayList();
            if( origBenefitsCategories.Count > 0 )
            {
                origCategories = origBenefitsCategories.Keys;
            }

            BenefitsCategoryDetailsFusFormatter bcdFusFormatter = new BenefitsCategoryDetailsFusFormatter();
            foreach( BenefitsCategory benefitsCategory in categories )
            {
                BenefitsCategoryDetails origBenefitsCategoryDetails = new BenefitsCategoryDetails();

                BenefitsCategoryDetails benefitsCategoryDetails = cov.BenefitsCategoryDetailsFor( benefitsCategory );

                string benefitCategoryName = GetBenefitsCategoryFor( benefitsCategory );

                if( origBenefitsCategories.Contains( benefitsCategory ) )
                {
                    origBenefitsCategoryDetails = origCov.BenefitsCategoryDetailsFor( benefitsCategory );
                }

                if( bcdFusFormatter.AddFusBenefitsCategoryDetailsTo( benefitsCategoryDetails,
                                                                    origBenefitsCategoryDetails,
                                                                    benefitCategoryName,
                                                                    nameValueList ) )
                {
                    writeNote = true;
                }


            }

            if( cov.HasChangedFor( "ServiceForPreExistingCondition" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.IS_PRE_EXISTING, cov.ServiceForPreExistingCondition.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "ServiceIsCoveredBenefit" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.IS_COVERED_BENEFIT, cov.ServiceIsCoveredBenefit.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "ClaimsAddressVerified" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.CLAIMS_ADDR_VERIFIED, cov.ClaimsAddressVerified.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "CoordinationOfbenefits" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.COORDINATION_OF_BENEFITS, cov.CoordinationOfbenefits.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "TypeOfVerificationRule" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.COB_RULE, cov.TypeOfVerificationRule.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "TypeOfProduct" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.TYPE_OF_PRODUCT, cov.TypeOfProduct.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "PPOPricingOrBroker" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.PPO_NETWORK_NAME, cov.PPOPricingOrBroker ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "FacilityContractedProvider" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.IS_CONTRACTED_PROVIDER, cov.FacilityContractedProvider.Description ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "AutoInsuranceClaimNumber" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.AUTO_CLAIM_NUMBER, cov.AutoInsuranceClaimNumber ) );
                writeNote = true;
            }

            if( cov.HasChangedFor( "AutoMedPayCoverage" ) )
            {
                nameValueList.Add( FormatNameValuePair( FusLabel.AUTO_MEDPAY_COVERAGE, cov.AutoMedPayCoverage.Description ) );
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
