using System;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{

    /// <summary>
    /// 
    /// </summary>
    class ParseCommercialStrategy : ParseStrategy
    {

        #region Constants

        const string CATEGORY_CHEM_DEP = "AI";

        const string CATEGORY_GENERAL = "30";

        const string CATEGORY_INPATIENT = "48";

        const string CATEGORY_NEWBORN = "65";

        const string CATEGORY_OB = "69";

        const string CATEGORY_OUTPATIENT = "50";

        const string CATEGORY_PSYCH_IP = "A7";

        const string CATEGORY_PSYCH_OP = "A8";

        const string CATEGORY_REHAB_IP = "AB";

        const string CATEGORY_REHAB_OP = "AC";

        const string CATEGORY_SNF_SUBACUTE = "AG";

        const string CODE_BLANK = "B";

        const string CONFIG_COMMERCIAL_PAYORID_PATTERN = "RegexPatternForCommercialPayorId";

        const string COVERAGE_LEVEL_CODE_FAM = "FAM";

        const string COVERAGE_LEVEL_CODE_IND = "IND";

        const string ELIG_OR_BEN_CO_INSURANCE = "A";

        const string ELIG_OR_BEN_CO_PAYMENT = "B";

        const string ELIG_OR_BEN_DEDUCTIBLE = "C";

        const string ELIG_OR_BEN_LIMITATIONS = "F";

        const string ELIG_OR_BEN_OUT_OF_POCKET = "G";

        const string QUANTITY_QUALIFIER_VISITS = "VS";

        const string TIME_PERIOD_QUALIFIER_CALENDAR_YEAR = "23";

        const string TIME_PERIOD_QUALIFIER_LIFETIME_REMAINING = "33";

        const string TIME_PERIOD_QUALIFIER_LIFETIME = "32";

        const string TIME_PERIOD_QUALIFIER_REMAINING = "29";

        const string TIME_PERIOD_QUALIFIER_SERVICE_YEAR = "22";

        const string TIME_PERIOD_QUALIFIER_VISIT = "27";

        const string TIME_PERIOD_QUALIFIER_ADMISSION = "36";

        const string TIME_PERIOD_QUALIFIER_YEAR_TO_DATE = "24";

        const string TYPEOF_PRODUCT = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[YesNoConditionOrResponseCode2='{0}']/InsuranceTypeCode";

        #endregion

        #region Fields

        private static Regex c_CommercialPayorIdRegularExpression = null;

        private Hashtable i_BenefitCategories = new Hashtable();

        private ArrayList i_BenefitsCategoryDetails = new ArrayList();

        private CommercialConstraints i_InNetWorkCommercialConstraints = new CommercialConstraints();

        private CommercialConstraints i_OutOfNetWorkCommercialConstraints = new CommercialConstraints();

        private Hashtable i_ProductTypeCodeMapping = new Hashtable();

        #endregion

        #region Constructors

        static ParseCommercialStrategy()
        {

            string commercialRegularExpressionPattern =
                ConfigurationManager.AppSettings[CONFIG_COMMERCIAL_PAYORID_PATTERN];

            if( !string.IsNullOrEmpty( commercialRegularExpressionPattern ) )
            {

                c_CommercialPayorIdRegularExpression =
                    new Regex( commercialRegularExpressionPattern );

            }//if
            else
            {

                c_CommercialPayorIdRegularExpression = null;

            }//else

        }//method

        #endregion

        #region Non-Public Properties


        /// <summary>
        /// Gets the payor id regular expression.
        /// </summary>
        /// <value>The payor id regular expression.</value>
        protected override Regex PayorIdRegularExpression
        {

            get
            {

                return c_CommercialPayorIdRegularExpression;

            }//get

        }//property

        #endregion

        #region Public Methods

        public override void Execute()
        {

            base.Execute();

            if( this.IsPayorEnabledById )
            {

                if( null == TheBenefitsValidationResponse )
                {

                    throw new ArgumentNullException( "BenefitsValidationResponse",
                        "BenefitsValidationResponse can not be null" );

                }//if

                if( null != this.TheBenefitResponse )
                {

                    this.PopulateAndAddConstraints();

                }//if

            }//if

        }//method

        public override void SetBenefitsResponse( BenefitsValidationResponse response )
        {
            base.SetBenefitsResponse( response );
            if( response.GetParseCoverageType() != null )
            {
                if( response.GetParseCoverageType() == typeof( GovernmentOtherCoverage ) )
                {
                    IsGovernmentOther = true;
                }
            }
        }

        #endregion

        #region Non-Public Methods

        private BenefitsCategoryDetails BuildAndGetCategoryDetailsFor( string serviceTypeCode, string network, int categoryOid )
        {
            BenefitsCategoryDetails benefitCategoryDetails = null;

            if( DoesBenifitsCategoryExist( serviceTypeCode, network ) )
            {
                benefitCategoryDetails = new BenefitsCategoryDetails();

                // deductible values

                benefitCategoryDetails.Deductible = GetDeductibleCoPayOrOutOfPocket( ELIG_OR_BEN_DEDUCTIBLE, serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.Deductible < 0 )
                {
                    benefitCategoryDetails.Deductible = GetDeductibleCoPayOrOutOfPocket( ELIG_OR_BEN_DEDUCTIBLE, serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );

                    if( benefitCategoryDetails.Deductible != -1 )
                        benefitCategoryDetails.ForceChangedStatusFor( "Deductible" );
                }
                else
                {
                    benefitCategoryDetails.ForceChangedStatusFor( "Deductible" );
                }

                benefitCategoryDetails.TimePeriod = GetTimePeriod( serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.TimePeriod.Code == CODE_BLANK )
                {
                    benefitCategoryDetails.TimePeriod = GetTimePeriod( serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );

                    if( !benefitCategoryDetails.TimePeriod.Code.Equals( CODE_BLANK ) )
                        benefitCategoryDetails.ForceChangedStatusFor( "TimePeriod" );
                }
                else
                {
                    benefitCategoryDetails.ForceChangedStatusFor( "TimePeriod" );
                }

                float remaining = GetDeductibleCoPayOrOutOfPocketMet( ELIG_OR_BEN_DEDUCTIBLE, serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( remaining < 0 )
                {
                    remaining = GetDeductibleCoPayOrOutOfPocketMet( ELIG_OR_BEN_DEDUCTIBLE, serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );
                }

                if( remaining >= 0 && benefitCategoryDetails.Deductible > remaining )
                {
                    benefitCategoryDetails.DeductibleDollarsMet = benefitCategoryDetails.Deductible - remaining;
                    benefitCategoryDetails.ForceChangedStatusFor( "DeductibleDollarsMet" );
                }

                if( !benefitCategoryDetails.DeductibleMet.Equals( GetDeductibleMet( benefitCategoryDetails ) ))
                {
                    benefitCategoryDetails.DeductibleMet = GetDeductibleMet( benefitCategoryDetails );
                    benefitCategoryDetails.ForceChangedStatusFor("DeductibleMet");
                }

                benefitCategoryDetails.CoInsurance = GetCoInsurancePercentage( serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.CoInsurance < 0 )
                {
                    benefitCategoryDetails.CoInsurance = GetCoInsurancePercentage( serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );
                    if( benefitCategoryDetails.CoInsurance != -1 )
                        benefitCategoryDetails.ForceChangedStatusFor("CoInsurance");
                }
                else
                {
                    benefitCategoryDetails.ForceChangedStatusFor("CoInsurance");
                }

                // out-of-pocket values

                benefitCategoryDetails.OutOfPocket = GetDeductibleCoPayOrOutOfPocket( ELIG_OR_BEN_OUT_OF_POCKET, serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.OutOfPocket < 0 )
                {
                    benefitCategoryDetails.OutOfPocket = GetDeductibleCoPayOrOutOfPocket( ELIG_OR_BEN_OUT_OF_POCKET, serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );
                    if( benefitCategoryDetails.OutOfPocket != -1 )
                        benefitCategoryDetails.ForceChangedStatusFor("OutOfPocket");
                }
                else benefitCategoryDetails.ForceChangedStatusFor("OutOfPocket");

                remaining = GetDeductibleCoPayOrOutOfPocketMet( ELIG_OR_BEN_OUT_OF_POCKET, serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( remaining < 0 )
                {
                    remaining = GetDeductibleCoPayOrOutOfPocketMet( ELIG_OR_BEN_OUT_OF_POCKET, serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );
                }

                if( remaining > 0 && benefitCategoryDetails.OutOfPocket > remaining )
                {
                    benefitCategoryDetails.OutOfPocketDollarsMet = benefitCategoryDetails.OutOfPocket - remaining;
                    benefitCategoryDetails.ForceChangedStatusFor("OutOfPocketDollarsMet");
                }

                if( !benefitCategoryDetails.OutOfPocketMet.Equals( GetOutOfPocketMet( benefitCategoryDetails ) ))
                {
                    benefitCategoryDetails.OutOfPocketMet = GetOutOfPocketMet( benefitCategoryDetails );
                    benefitCategoryDetails.ForceChangedStatusFor("OutOfPocketMet");
                }

                // co-pay values

                benefitCategoryDetails.CoPay = GetDeductibleCoPayOrOutOfPocket( ELIG_OR_BEN_CO_PAYMENT, serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.CoPay < 0 )
                {
                    benefitCategoryDetails.CoPay = GetDeductibleCoPayOrOutOfPocket( ELIG_OR_BEN_CO_PAYMENT, serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );

                    if( benefitCategoryDetails.CoPay != -1 )
                        benefitCategoryDetails.ForceChangedStatusFor("CoPay");

                }
                else benefitCategoryDetails.ForceChangedStatusFor("CoPay");

                benefitCategoryDetails.VisitsPerYear = GetCoPayVisitsPerYear( serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.VisitsPerYear < 0 )
                {
                    benefitCategoryDetails.VisitsPerYear = GetCoPayVisitsPerYear( serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );
                    if( benefitCategoryDetails.VisitsPerYear != -1 )
                        benefitCategoryDetails.ForceChangedStatusFor("VisitsPerYear");

                }
                else benefitCategoryDetails.ForceChangedStatusFor("VisitsPerYear");

                benefitCategoryDetails.LifeTimeMaxBenefit = GetLifeTimeMaxBenefit( serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.LifeTimeMaxBenefit < 0 )
                {
                    benefitCategoryDetails.LifeTimeMaxBenefit = GetLifeTimeMaxBenefit( serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );
                    if( benefitCategoryDetails.LifeTimeMaxBenefit != -1 )
                        benefitCategoryDetails.ForceChangedStatusFor("LifeTimeMaxBenefit");
                }
                else benefitCategoryDetails.ForceChangedStatusFor("LifeTimeMaxBenefit");

                benefitCategoryDetails.RemainingLifetimeValue = GetRemainingLifeTimeValue( serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.RemainingLifetimeValue < 0 )
                {
                    benefitCategoryDetails.RemainingLifetimeValue = GetRemainingLifeTimeValue( serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );
                    if( benefitCategoryDetails.RemainingLifetimeValue != -1 )
                        benefitCategoryDetails.ForceChangedStatusFor("RemainingLifetimeValue");
                }
                else benefitCategoryDetails.ForceChangedStatusFor("RemainingLifetimeValue");

                benefitCategoryDetails.MaxBenefitPerVisit = GetMaxBenefitPerVisit( serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.MaxBenefitPerVisit < 0 )
                {
                    benefitCategoryDetails.MaxBenefitPerVisit = GetMaxBenefitPerVisit( serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );
                    if( benefitCategoryDetails.MaxBenefitPerVisit != -1 )
                        benefitCategoryDetails.ForceChangedStatusFor("MaxBenefitPerVisit");
                }
                else benefitCategoryDetails.ForceChangedStatusFor("MaxBenefitPerVisit");

                benefitCategoryDetails.RemainingBenefitPerVisits = GetRemainingBenefitPerVisit( serviceTypeCode, network, COVERAGE_LEVEL_CODE_IND );
                if( benefitCategoryDetails.RemainingBenefitPerVisits < 0 )
                {
                    benefitCategoryDetails.RemainingBenefitPerVisits = GetRemainingBenefitPerVisit( serviceTypeCode, network, COVERAGE_LEVEL_CODE_FAM );
                    if( benefitCategoryDetails.RemainingBenefitPerVisits != -1 )
                        benefitCategoryDetails.ForceChangedStatusFor("RemainingBenefitPerVisits");
                }
                else benefitCategoryDetails.ForceChangedStatusFor("RemainingBenefitPerVisits");

                benefitCategoryDetails.BenefitCategory = GetBeneiftCategoryById( TheBenefitsValidationResponse.FacilityCode, categoryOid );
            }

            return benefitCategoryDetails;
        }

        private void BuildBenifitCategories()
        {

            i_BenefitCategories[CATEGORY_INPATIENT] = 1;
            i_BenefitCategories[CATEGORY_OUTPATIENT] = 2;
            i_BenefitCategories[CATEGORY_OB] = 3;
            i_BenefitCategories[CATEGORY_NEWBORN] = 4;
            i_BenefitCategories[CATEGORY_PSYCH_IP] = 6;
            i_BenefitCategories[CATEGORY_PSYCH_OP] = 7;
            i_BenefitCategories[CATEGORY_CHEM_DEP] = 8;
            i_BenefitCategories[CATEGORY_SNF_SUBACUTE] = 9;
            i_BenefitCategories[CATEGORY_REHAB_IP] = 10;
            i_BenefitCategories[CATEGORY_REHAB_OP] = 11;
            i_BenefitCategories[CATEGORY_GENERAL] = 12;
        }

        private new bool DoesBenifitsCategoryExist( string code, string network )
        {
            bool result = false;
            if( !IsGovernmentOther )
            {
                foreach( BenefitsInformation bi in this.Benefits )
                {
                    if( bi.YesNoConditionOrResponseCode2 == network
                        && bi.ServiceTypeCode == code )
                    {
                        result = true;
                    }
                }
            }
            else
            {
                foreach( BenefitsInformation bi in this.Benefits )
                {
                    if( bi.ServiceTypeCode == code )
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private BenefitsCategory GetBeneiftCategoryById( string facilityCode, int Oid )
        {
            BenefitsCategory result = new BenefitsCategory();
            IBenefitsCategoryBroker benefitsCategoryBroker = BrokerFactory.BrokerOfType<IBenefitsCategoryBroker>();
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility = facilityBroker.FacilityWith( facilityCode );
            result = benefitsCategoryBroker.BenefitsCategoryWith( facility.Oid, Oid );
            return result;
        }

        private int GetCoInsurancePercentage( string serviceTypeCode, string network, string indOrFam )
        {
            int result = -1;
            string strPercent = "-1";
            bool blnInNetwork = false;

            if( network == "Y" )
            {
                blnInNetwork = true;
            }

            // Filter #1

            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )
            ArrayList matches = this.GetCoverages( ELIG_OR_BEN_CO_INSURANCE, indOrFam, serviceTypeCode, blnInNetwork, null );

            if( matches.Count == 1 )
            {
                strPercent = ( (BenefitsInformation)matches[0] ).Percent;
            }
            else if( matches.Count == 0 )
            {
                // Filter #2

                // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )
                matches = this.GetCoverages( ELIG_OR_BEN_CO_INSURANCE, indOrFam, serviceTypeCode, blnInNetwork, TIME_PERIOD_QUALIFIER_SERVICE_YEAR );

                if( matches.Count == 1 )
                {
                    strPercent = ( (BenefitsInformation)matches[0] ).Percent;
                }
            }

            if( matches.Count > 1 )
            {
                string prevValue = "None";
                bool blnSame = true;

                foreach( BenefitsInformation bi in matches )
                {
                    if( prevValue != "None" )
                    {
                        if( prevValue != bi.Percent )
                        {
                            blnSame = false;
                            break;
                        }
                    }
                    else
                    {
                        prevValue = bi.Percent;
                    }
                }

                if( blnSame && prevValue != null && prevValue != "None" )
                {
                    strPercent = prevValue;
                }
            }

            if( strPercent.Contains( "." ) )
            {
                float f = float.Parse( strPercent );
                f = f * 100;

                result = Convert.ToInt32( f );
            }
            else
            {
                result = int.Parse( strPercent );
            }

            return result;
        }

        private int GetCoPayVisitsPerYear( string serviceTypeCode, string network, string indOrFam )
        {
            string strVal = "-1";
            int result = 0;

            //getCoverages( elegibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier, quantityQualifier )

            bool blnInNetwork = false;

            if( network == "Y" )
            {
                blnInNetwork = true;
            }

            // Filter #1

            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )
            ArrayList matches = this.GetCoverages( ELIG_OR_BEN_CO_INSURANCE, indOrFam, serviceTypeCode, blnInNetwork,
                TIME_PERIOD_QUALIFIER_SERVICE_YEAR, QUANTITY_QUALIFIER_VISITS );

            if( matches.Count == 1 )
            {
                strVal = ( (BenefitsInformation)matches[0] ).Quantity;
            }

            if( matches.Count > 1 )
            {
                string prevValue = "None";
                bool blnSame = true;

                foreach( BenefitsInformation bi in matches )
                {
                    if( prevValue != "None" )
                    {
                        if( prevValue != bi.Quantity )
                        {
                            blnSame = false;
                            break;
                        }
                    }
                    else
                    {
                        prevValue = bi.Quantity;
                    }
                }

                if( blnSame && prevValue != null && prevValue != "None" )
                {
                    strVal = prevValue;
                }
            }

            if( strVal.IndexOf( "." ) > 0 )
            {
                strVal = strVal.Substring( 0, strVal.IndexOf( "." ) );
            }

            result = int.Parse( strVal );

            return result;
        }

        private float GetDeductibleCoPayOrOutOfPocket( string eligibilityType, string serviceTypeCode, string network, string indOrFam )
        {
            bool blnInNetwork = false;

            if( network == "Y" )
            {
                blnInNetwork = true;
            }
            // Filter #1

            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )
            ArrayList matches = this.GetCoverages( eligibilityType, indOrFam, serviceTypeCode, blnInNetwork, null );

            if( matches.Count == 1 )
            {
                return float.Parse( ( (BenefitsInformation)matches[0] ).MonetaryAmount );

            }
            else if( matches.Count == 0 )
            {
                // Filter #2 

                // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

                matches = this.GetCoverages( eligibilityType, indOrFam, serviceTypeCode,
                   blnInNetwork, TIME_PERIOD_QUALIFIER_SERVICE_YEAR );

                if( matches.Count == 1 )
                {
                    return float.Parse( ( (BenefitsInformation)matches[0] ).MonetaryAmount );
                }
                else if( matches.Count == 0 )
                {
                    // Filter #3 

                    // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

                    matches = this.GetCoverages( eligibilityType, indOrFam, serviceTypeCode,
                       blnInNetwork, TIME_PERIOD_QUALIFIER_CALENDAR_YEAR );

                    if( matches.Count == 1 )
                    {
                        return float.Parse( ( (BenefitsInformation)matches[0] ).MonetaryAmount );
                    }
                    else if( matches.Count == 0 )
                    {
                        // Filter #4

                        // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

                        matches = this.GetCoverages( eligibilityType, indOrFam, serviceTypeCode,
                           blnInNetwork, TIME_PERIOD_QUALIFIER_VISIT );

                        if( matches.Count == 1 )
                        {
                            return float.Parse( ( (BenefitsInformation)matches[0] ).MonetaryAmount );
                        }
                        else if( matches.Count == 0 )
                        {
                            // Filter #5

                            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

                            matches = this.GetCoverages( eligibilityType, indOrFam, serviceTypeCode,
                               blnInNetwork, TIME_PERIOD_QUALIFIER_ADMISSION );

                            if( matches.Count == 1 )
                            {
                                return float.Parse( ( (BenefitsInformation)matches[0] ).MonetaryAmount );
                            }
                            else if( matches.Count == 0 )
                            {
                                return -1;
                            }
                        }
                    }
                }
            }



            // if we make it this far, we have more than 1 match... check for same value

            string prevAmount = "None";
            bool blnSame = true;

            foreach( BenefitsInformation bi in matches )
            {
                if( prevAmount != "None" )
                {
                    if( prevAmount != bi.Quantity )
                    {
                        blnSame = false;
                        break;
                    }
                }
                else
                {
                    prevAmount = bi.Quantity;
                }
            }

            if( blnSame && prevAmount != null && prevAmount != "None" )
            {
                return float.Parse( prevAmount );
            }
            else
            {
                return -1;
            }

        }

        private float GetDeductibleCoPayOrOutOfPocketMet( string eligibilityType, string serviceTypeCode, string network,
                    string indOrFam )
        {
            bool blnInNetwork = false;

            if( network == "Y" )
            {
                blnInNetwork = true;
            }
            // Filter #1

            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

            ArrayList matches = this.GetCoverages( eligibilityType, indOrFam, serviceTypeCode,
               blnInNetwork, TIME_PERIOD_QUALIFIER_REMAINING );

            if( matches.Count == 1 )
            {
                return float.Parse( ( (BenefitsInformation)matches[0] ).MonetaryAmount );
            }
            else if( matches.Count == 0 )
            {
                // Filter #3

                // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

                matches = this.GetCoverages( eligibilityType, indOrFam, serviceTypeCode,
                   blnInNetwork, TIME_PERIOD_QUALIFIER_YEAR_TO_DATE );

                if( matches.Count == 1 )
                {
                    return float.Parse( ( (BenefitsInformation)matches[0] ).MonetaryAmount );
                }
                else if( matches.Count == 0 )
                {
                    return -1;
                }
            }

            // if we make it this far, we have more than 1 match... check for same value

            string prevAmount = "None";
            bool blnSame = true;

            foreach( BenefitsInformation bi in matches )
            {
                if( prevAmount != "None" )
                {
                    if( prevAmount != bi.Quantity )
                    {
                        blnSame = false;
                        break;
                    }
                }
                else
                {
                    prevAmount = bi.Quantity;
                }
            }

            if( blnSame && prevAmount != null && prevAmount != "None" )
            {
                return float.Parse( prevAmount );
            }
            else
            {
                return -1;
            }

        }

        private double GetLifeTimeMaxBenefit( string serviceTypeCode, string network, string indOrFam )
        {
            string strVal = "-1";
            double result = 0;

            //getCoverages( elegibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier, quantityQualifier )

            bool blnInNetwork = false;

            if( network == "Y" )
            {
                blnInNetwork = true;
            }

            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )
            ArrayList matches = this.GetCoverages( ELIG_OR_BEN_LIMITATIONS, indOrFam, serviceTypeCode, blnInNetwork,
                TIME_PERIOD_QUALIFIER_LIFETIME );

            if( matches.Count == 1 )
            {
                strVal = ( (BenefitsInformation)matches[0] ).MonetaryAmount;
            }

            if( matches.Count > 1 )
            {
                string prevValue = "None";
                bool blnSame = true;

                foreach( BenefitsInformation bi in matches )
                {
                    if( prevValue != "None" )
                    {
                        if( prevValue != bi.MonetaryAmount )
                        {
                            blnSame = false;
                            break;
                        }
                    }
                    else
                    {
                        prevValue = bi.MonetaryAmount;
                    }
                }

                if( blnSame && prevValue != null && prevValue != "None" )
                {
                    strVal = prevValue;
                }
            }

            result = double.Parse( strVal );

            return result;
        }

        private double GetMaxBenefitPerVisit( string serviceTypeCode, string network, string indOrFam )
        {
            string strVal = "-1";
            double result = 0;

            //getCoverages( elegibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier, quantityQualifier )

            bool blnInNetwork = false;

            if( network == "Y" )
            {
                blnInNetwork = true;
            }

            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )
            ArrayList matches = this.GetCoverages( ELIG_OR_BEN_LIMITATIONS, indOrFam, serviceTypeCode, blnInNetwork,
                TIME_PERIOD_QUALIFIER_VISIT );

            if( matches.Count == 1 )
            {
                strVal = ( (BenefitsInformation)matches[0] ).MonetaryAmount;
            }

            if( matches.Count > 1 )
            {
                string prevValue = "None";
                bool blnSame = true;

                foreach( BenefitsInformation bi in matches )
                {
                    if( prevValue != "None" )
                    {
                        if( prevValue != bi.MonetaryAmount )
                        {
                            blnSame = false;
                            break;
                        }
                    }
                    else
                    {
                        prevValue = bi.MonetaryAmount;
                    }
                }

                if( blnSame && prevValue != null && prevValue != "None" )
                {
                    strVal = prevValue;
                }
            }

            result = double.Parse( strVal );

            return result;
        }

        private double GetRemainingBenefitPerVisit( string serviceTypeCode, string network, string indOrFam )
        {
            string strVal = "-1";
            double result = 0;

            //getCoverages( elegibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier, quantityQualifier )

            bool blnInNetwork = false;

            if( network == "Y" )
            {
                blnInNetwork = true;
            }

            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )
            ArrayList matches = this.GetCoverages( ELIG_OR_BEN_LIMITATIONS, indOrFam, serviceTypeCode, blnInNetwork,
                TIME_PERIOD_QUALIFIER_REMAINING );

            if( matches.Count == 1 )
            {
                strVal = ( (BenefitsInformation)matches[0] ).MonetaryAmount;
            }

            if( matches.Count > 1 )
            {
                string prevValue = "None";
                bool blnSame = true;

                foreach( BenefitsInformation bi in matches )
                {
                    if( prevValue != "None" )
                    {
                        if( prevValue != bi.MonetaryAmount )
                        {
                            blnSame = false;
                            break;
                        }
                    }
                    else
                    {
                        prevValue = bi.MonetaryAmount;
                    }
                }

                if( blnSame && prevValue != null && prevValue != "None" )
                {
                    strVal = prevValue;
                }
            }

            result = double.Parse( strVal );

            return result;
        }

        private double GetRemainingLifeTimeValue( string serviceTypeCode, string network, string indOrFam )
        {
            string strVal = "-1";
            double result = 0;

            //getCoverages( elegibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier, quantityQualifier )

            bool blnInNetwork = false;

            if( network == "Y" )
            {
                blnInNetwork = true;
            }

            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )
            ArrayList matches = this.GetCoverages( ELIG_OR_BEN_LIMITATIONS, indOrFam, serviceTypeCode, blnInNetwork,
                TIME_PERIOD_QUALIFIER_LIFETIME_REMAINING );

            if( matches.Count == 1 )
            {
                strVal = ( (BenefitsInformation)matches[0] ).MonetaryAmount;
            }

            if( matches.Count > 1 )
            {
                string prevValue = "None";
                bool blnSame = true;

                foreach( BenefitsInformation bi in matches )
                {
                    if( prevValue != "None" )
                    {
                        if( prevValue != bi.MonetaryAmount )
                        {
                            blnSame = false;
                            break;
                        }
                    }
                    else
                    {
                        prevValue = bi.MonetaryAmount;
                    }
                }

                if( blnSame && prevValue != null && prevValue != "None" )
                {
                    strVal = prevValue;
                }
            }

            result = double.Parse( strVal );

            return result;
        }

        protected string GetServiceCodeAndNetWorkType( string code, string network )
        {
            return string.Format( "ServiceTypeCode='{0}' and YesNoConditionOrResponseCode2='{1}'",
                code, network );
        }

        private TimePeriodFlag GetTimePeriod( string serviceTypeCode, string network, string indOrFam )
        {
            TimePeriodFlag result = new TimePeriodFlag();
            string timePeriodCode = string.Empty;
            bool blnInNetwork = false;

            if( network == "Y" )
            {
                blnInNetwork = true;
            }
            // Filter #1

            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )
            ArrayList matches = this.GetCoverages( ELIG_OR_BEN_DEDUCTIBLE, indOrFam, serviceTypeCode, blnInNetwork, null );

            if( matches.Count == 1 )
            {
                timePeriodCode = TIME_PERIOD_QUALIFIER_YEAR_TO_DATE;
            }
            else if( matches.Count == 0 )
            {
                // Filter #2 

                // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

                matches = this.GetCoverages( ELIG_OR_BEN_DEDUCTIBLE, indOrFam, serviceTypeCode,
                   blnInNetwork, TIME_PERIOD_QUALIFIER_SERVICE_YEAR );

                if( matches.Count == 1 )
                {
                    timePeriodCode = TIME_PERIOD_QUALIFIER_YEAR_TO_DATE;
                }
                else if( matches.Count == 0 )
                {
                    // Filter #3 

                    // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

                    matches = this.GetCoverages( ELIG_OR_BEN_DEDUCTIBLE, indOrFam, serviceTypeCode,
                       blnInNetwork, TIME_PERIOD_QUALIFIER_CALENDAR_YEAR );

                    if( matches.Count == 1 )
                    {
                        timePeriodCode = TIME_PERIOD_QUALIFIER_YEAR_TO_DATE;
                    }
                    else if( matches.Count == 0 )
                    {
                        // Filter #4

                        // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

                        matches = this.GetCoverages( ELIG_OR_BEN_DEDUCTIBLE, indOrFam, serviceTypeCode,
                           blnInNetwork, TIME_PERIOD_QUALIFIER_VISIT );

                        if( matches.Count == 1 )
                        {
                            timePeriodCode = TIME_PERIOD_QUALIFIER_VISIT;
                        }
                        else if( matches.Count == 0 )
                        {
                            // Filter #5

                            // getCoverages( eligibilityType, coverageLevelCode, serviceTypeCode, inNetworkIndicator, timePeriodQualifier )

                            matches = this.GetCoverages( ELIG_OR_BEN_DEDUCTIBLE, indOrFam, serviceTypeCode,
                               blnInNetwork, TIME_PERIOD_QUALIFIER_ADMISSION );

                            if( matches.Count == 1 )
                            {
                                timePeriodCode = TIME_PERIOD_QUALIFIER_VISIT;
                            }
                        }
                    }
                }
            }



            // and, finally, set the value

            if( timePeriodCode == TIME_PERIOD_QUALIFIER_VISIT )
            {
                result.SetVisit();
            }
            else if( timePeriodCode == TIME_PERIOD_QUALIFIER_YEAR_TO_DATE )
            {
                result.SetYear();
            }
            else
            {
                result.SetBlank();
            }

            return result;
        }

        private TypeOfProduct GetTypeOfProduct( string network )
        {

            TypeOfProduct result = new TypeOfProduct();
            string path = string.Format( TYPEOF_PRODUCT, network );

            XmlNodeList nodes = XmlResponseDocument.SelectNodes( path );

            if( nodes.Count > 0 )
            {
                XmlNode node = nodes[0];
                if( NodeHasInnerText( node ) )
                {

                    int typeOfProductOid = GetTypeOfProductOidMappingFor( node.InnerText );
                    if( typeOfProductOid != 0 )
                    {
                        ITypeOfProductBroker typeOfProductBroker = BrokerFactory.BrokerOfType<ITypeOfProductBroker>();
                        result = typeOfProductBroker.TypeOfProductWith( typeOfProductOid );
                    }
                }
            }
            return result;
        }

        private int GetTypeOfProductOidMappingFor( string code )
        {
            int result = 0;
            switch( code )
            {
                case "PR":
                    result = 2;
                    break;
                case "PS":
                    result = 3;
                    break;
                case "HM":
                    result = 4;
                    break;
                case "EP":
                    result = 5;
                    break;
                case "":
                    result = 6;
                    break;
                case "HN":
                    result = 7;
                    break;
                case "IN":
                    result = 8;
                    break;
                default:
                    break;
            }
            return result;
        }

        private void PopulateAndAddConstraints()
        {
            BuildBenifitCategories();

            if( !IsGovernmentOther )
            {
                if( ResultHasInNetwork() )
                {
                    PopulateAndAddInNetwork();
                }

                if( ResultHasOutOfNetwork() )
                {
                    PopulateAndAddOutOfNetwork();
                }
            }
            else
            {
                //This has he same code as GovernmentOther
                PopulateAndAddInNetwork();
            }

        }

        private void PopulateAndAddInNetwork()
        {
            i_InNetWorkCommercialConstraints.BenefitsCategoryDetails.Clear();

            i_InNetWorkCommercialConstraints.KindOfConstraint.IsInNetwork = true;
            
            if( !i_InNetWorkCommercialConstraints.EffectiveDateForInsured.Equals( this.EffectiveDateForInsured ) )
            {
                i_InNetWorkCommercialConstraints.EffectiveDateForInsured = this.EffectiveDateForInsured;
                i_InNetWorkCommercialConstraints.ForceChangedStatusFor( "EffectiveDateForInsured" );
            }

            if( !i_InNetWorkCommercialConstraints.EligibilityPhone.Equals( this.EligibilityPhone ) )
            {
                i_InNetWorkCommercialConstraints.EligibilityPhone = this.EligibilityPhone;
                i_InNetWorkCommercialConstraints.ForceChangedStatusFor( "EligibilityPhone" );
            }

            if( !i_InNetWorkCommercialConstraints.InsuranceCompanyRepName.Equals( this.InsuranceCompanyRepName ) )
            {
                i_InNetWorkCommercialConstraints.InsuranceCompanyRepName = this.InsuranceCompanyRepName;
                i_InNetWorkCommercialConstraints.ForceChangedStatusFor( "InsuranceCompanyRepName" );
            }

            if( !i_InNetWorkCommercialConstraints.TerminationDateForInsured.Equals( this.TerminationDateForInsured ) )
            {
                i_InNetWorkCommercialConstraints.TerminationDateForInsured = this.TerminationDateForInsured;
                i_InNetWorkCommercialConstraints.ForceChangedStatusFor( "TerminationDateForInsured" );
            }

            if( !i_InNetWorkCommercialConstraints.TypeOfProduct.Equals( GetTypeOfProduct( IN_NETWORK ) ) )
            {
                i_InNetWorkCommercialConstraints.TypeOfProduct = GetTypeOfProduct( IN_NETWORK );
                i_InNetWorkCommercialConstraints.ForceChangedStatusFor("TypeOfProduct");
            }

            foreach( string key in i_BenefitCategories.Keys )
            {
                BenefitsCategoryDetails details = BuildAndGetCategoryDetailsFor( key, IN_NETWORK, (int)i_BenefitCategories[key] );

                if( details != null )
                {
                    i_InNetWorkCommercialConstraints.BenefitsCategoryDetails.Add( details );
                }
            }

            TheBenefitsValidationResponse.CoverageConstraintsCollection.Add(
                i_InNetWorkCommercialConstraints );
        }

        private void PopulateAndAddOutOfNetwork()
        {
            i_OutOfNetWorkCommercialConstraints.BenefitsCategoryDetails.Clear();

            i_OutOfNetWorkCommercialConstraints.KindOfConstraint.IsInNetwork = false;

            if( !i_OutOfNetWorkCommercialConstraints.EffectiveDateForInsured.Equals( this.EffectiveDateForInsured ) )
            {
                i_OutOfNetWorkCommercialConstraints.EffectiveDateForInsured = this.EffectiveDateForInsured;
                i_OutOfNetWorkCommercialConstraints.ForceChangedStatusFor( "EffectiveDateForInsured" );
            }

            if( !i_OutOfNetWorkCommercialConstraints.EligibilityPhone.Equals( this.EligibilityPhone ) )
            {
                i_OutOfNetWorkCommercialConstraints.EligibilityPhone = this.EligibilityPhone;
                i_OutOfNetWorkCommercialConstraints.ForceChangedStatusFor( "EligibilityPhone" );
            }

            if( !i_OutOfNetWorkCommercialConstraints.InsuranceCompanyRepName.Equals( this.InsuranceCompanyRepName ) )
            {
                i_OutOfNetWorkCommercialConstraints.InsuranceCompanyRepName = this.InsuranceCompanyRepName;
                i_OutOfNetWorkCommercialConstraints.ForceChangedStatusFor( "InsuranceCompanyRepName" );
            }

            if( !i_OutOfNetWorkCommercialConstraints.TerminationDateForInsured.Equals( this.TerminationDateForInsured ) )
            {
                i_OutOfNetWorkCommercialConstraints.TerminationDateForInsured = this.TerminationDateForInsured;
                i_OutOfNetWorkCommercialConstraints.ForceChangedStatusFor( "TerminationDateForInsured" );
            }

            if( !i_OutOfNetWorkCommercialConstraints.TypeOfProduct.Equals( GetTypeOfProduct( IN_NETWORK ) ) )
            {
                i_OutOfNetWorkCommercialConstraints.TypeOfProduct = GetTypeOfProduct( IN_NETWORK );
                i_OutOfNetWorkCommercialConstraints.ForceChangedStatusFor( "TypeOfProduct" );
            }

            foreach( string key in i_BenefitCategories.Keys )
            {

                BenefitsCategoryDetails details = BuildAndGetCategoryDetailsFor( key, OUT_OF_NETWORK, (int)i_BenefitCategories[key] );
                if( details != null )
                {
                    i_OutOfNetWorkCommercialConstraints.BenefitsCategoryDetails.Add( details );
                }
            }

            TheBenefitsValidationResponse.CoverageConstraintsCollection.Add(
                i_OutOfNetWorkCommercialConstraints );
        }

        private new bool ResultHasInNetwork()
        {
            bool blnResult = false;

            foreach( BenefitsInformation bi in this.Benefits )
            {
                if( bi.YesNoConditionOrResponseCode2 == "Y" )
                {
                    blnResult = true;
                    break;
                }
            }

            return blnResult;
        }

        private new bool ResultHasOutOfNetwork()
        {
            bool blnResult = false;

            foreach( BenefitsInformation bi in this.Benefits )
            {
                if( bi.YesNoConditionOrResponseCode2 == "N" )
                {
                    blnResult = true;
                    break;
                }
            }

            return blnResult;
        }

        #endregion

    }//class

}//namespace
