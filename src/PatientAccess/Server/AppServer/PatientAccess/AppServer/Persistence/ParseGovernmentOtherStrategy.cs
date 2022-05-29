using System.Xml;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    class ParseGovernmentOtherStrategy : ParseCommercialStrategy
    {

        #region Constants

        const string CO_INSURANCE_PATH = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='A' and TimePeriodQualifier='22']/Percent";

        const string CO_PAY_PATH = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='B' and TimePeriodQualifier='22']/MonetaryAmount";

        const string DEDUCTIBLE_DOLLARS_MET_PATH = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='C' and TimePeriodQualifier='24']/MonetaryAmount";

        const string DEDUCTIBLE_PATH = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='C' and TimePeriodQualifier='22']/MonetaryAmount";

        const string OUT_OF_POCKET_DOLLARS_MET_PATH = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='G' and TimePeriodQualifier='24']/MonetaryAmount";

        const string OUT_OF_POCKET_PATH = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='G' and TimePeriodQualifier='22']/MonetaryAmount";

        const string TYPEOF_COVERAGE_PATH = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation/CoverageLevelCode";

        #endregion

        #region Fields

        GovernmentOtherConstraints i_GovernmentOtherConstraints = new GovernmentOtherConstraints();

        #endregion

        #region Non-Public Properties

        private string TypeOfCoverage
        {
            get
            {
                string result = string.Empty;
                XmlNode node = XmlResponseDocument.SelectSingleNode( TYPEOF_COVERAGE_PATH );
                if( NodeHasInnerText( node ) )
                {
                    result = GetTypeOfCoverage( node.InnerText );
                }
                return result;
            }
        }

        #endregion

        #region Public Methods

        public override void Execute()
        {

            base.Execute();

            /*
             if( i_BenefitsValidationResponse == null )
            {
                throw new ArgumentNullException( "BenefitsValidationResponse",
                    "BenefitsValidationResponse can not be null" );
            }

            if( i_BenefitsValidationResponse.DataValidationResponse != null )
            {
                if( DoesResponseHaveXml( ) )
                {
                    i_XmlResponse.LoadXml( i_BenefitsValidationResponse.DataValidationResponse.payorXmlMessage );
                }
                PopulateAndAddConstraintsFromXml( );
            }

            base.Execute();
             
             */

            PopulateAndAddConstraintsFromXml();
        }

        #endregion

        #region Non-Public Methods

        private string GetTypeOfCoverage( string code )
        {
            string result = string.Empty;

            switch( code )
            {
                case "CHD":
                    result = "Children Only";
                    break;
                case "DEP":
                    result = "Dependents Only";
                    break;
                case "ECH":
                    result = "Employee and Children";
                    break;
                case "EMP":
                    result = "Employee Only";
                    break;
                case "ESP":
                    result = "Employee and Spouse";
                    break;
                case "FAM":
                    result = "Family";
                    break;
                case "IND":
                    result = "Individual";
                    break;
                case "SPC":
                    result = "Spouse and Children";
                    break;
                case "SPO":
                    result = "Spouse Only";
                    break;
                default:
                    break;
            }

            return result;
        }

        private void PopulateAndAddConstraintsFromXml()
        {

            if( TheBenefitsValidationResponse.CoverageConstraintsCollection.Count > 0 )
            {
                CommercialConstraints c = (CommercialConstraints)TheBenefitsValidationResponse.CoverageConstraintsCollection[0];

                if( c != null )
                {
                    if( c.BenefitsCategoryDetails.Count > 0 )
                    {
                        BenefitsCategoryDetails b = (BenefitsCategoryDetails)c.BenefitsCategoryDetails[0];
                        if( b != null )
                        {
                            i_GovernmentOtherConstraints.CoInsurance = b.CoInsurance;
                            i_GovernmentOtherConstraints.CoPay = b.CoPay;
                            i_GovernmentOtherConstraints.Deductible = b.Deductible;
                            i_GovernmentOtherConstraints.DeductibleDollarsMet = b.DeductibleDollarsMet;
                            i_GovernmentOtherConstraints.EffectiveDateForInsured = c.EffectiveDateForInsured;
                            i_GovernmentOtherConstraints.EligibilityPhone = c.EligibilityPhone;
                            i_GovernmentOtherConstraints.InsuranceCompanyRepName = c.InsuranceCompanyRepName;
                            i_GovernmentOtherConstraints.OutOfPocket = b.OutOfPocket;
                            i_GovernmentOtherConstraints.OutOfPocketDollarsMet = b.OutOfPocketDollarsMet;
                            i_GovernmentOtherConstraints.TerminationDateForInsured = c.TerminationDateForInsured;
                            i_GovernmentOtherConstraints.TypeOfCoverage = this.TypeOfCoverage;
                            TheBenefitsValidationResponse.CoverageConstraintsCollection.Clear();
                            TheBenefitsValidationResponse.CoverageConstraintsCollection.Add( i_GovernmentOtherConstraints );
                        }
                    }
                }
            }
        }

        #endregion

    }//class

}//namespace
