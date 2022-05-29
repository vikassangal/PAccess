using System;
using System.Configuration;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{

    /// <summary>
    /// 
    /// </summary>
    class ParseCoverageContext
    {

        #region Constants

        private const string COMMERCIAL_COVERAGE_INOUT = "PatientAccess.Domain.CommercialCoverage-InOut";
        private const string COMMERCIAL_COVERAGE = "PatientAccess.Domain.CommercialCoverage";
        private const string GOV_OTHER_COVERAGE = "PatientAccess.Domain.GovernmentOtherCoverage";
        private const string MEDICAID_COVERAGE = "PatientAccess.Domain.GovernmentMedicaidCoverage";
        private const string MEDICARE_COVERAGE = "PatientAccess.Domain.GovernmentMedicareCoverage";
        private const string SELF_PAY_COVERAGE = "PatientAccess.Domain.SelfPayCoverage";
        private const string WORKERS_COMP_COVERAGE = "PatientAccess.Domain.WorkersCompensationCoverage";
        private const string PARSE_COMMERCIAL = "PARSE_COMMERCIAL";
        private const string PARSE_GOVERNMENT_MEDICAID = "PARSE_GOVERNMENT_MEDICAID";
        private const string PARSE_GOVERNMENT_MEDICARE = "PARSE_GOVERNMENT_MEDICARE";
        private const string PARSE_GOVERNMENT_OTHER = "PARSE_GOVERNMENT_OTHER";        

        #endregion

        #region Fields

        private static readonly ILog c_Logger =
                                    LogManager.GetLogger( typeof( DataValidationBroker ) );

        #endregion

        #region Non-Public Properties

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static ILog Logger
        {

            get
            {

                return c_Logger;

            }//get

        }//property

        #endregion

        #region Public Methods

        /// <summary>
        /// Parse and set coverage constraints.
        /// </summary>
        /// <param name="aBenefitsValidationResponse"></param>
        public void ParseAndSetCoverageConstraints( BenefitsValidationResponse aBenefitsValidationResponse )
        {

            if( null == aBenefitsValidationResponse )
            {

                throw new ArgumentNullException( "BenefitsValidationResponse",
                    "BenefitsValidationResponse can not be null" );

            }//if

            if( null != aBenefitsValidationResponse.ReturnedDataValidationTicket )
            {

                DataValidationBenefitsResponse aDataValidationBenefitsResponse =
                    aBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse;

                if( aDataValidationBenefitsResponse.IsInParsableState &&
                    IsConfigParseCoverageOnFor( aDataValidationBenefitsResponse.BenefitsResponseParseStrategy ) )
                {

                    try
                    {

                        IParseCoverageStrategy aParseCoverageStrategy =
                            GetParseStrategyFor( aDataValidationBenefitsResponse.BenefitsResponseParseStrategy );

                        aParseCoverageStrategy.SetBenefitsResponse( aBenefitsValidationResponse );
                        aParseCoverageStrategy.Execute();

                    }//try
                    catch( Exception anException )
                    {

                        Logger.Error(
                            String.Format(
                                "Error parsing ticket {0}.",
                                aBenefitsValidationResponse.ReturnedDataValidationTicket.TicketId ),
                            anException );

                    }//catch

                }//if

            }//if

        }//method

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Gets the parse strategy
        /// </summary>
        /// <param name="nameOfStrategyToUse">The name of strategy to use.</param>
        /// <returns></returns>
        private static IParseCoverageStrategy GetParseStrategyFor( string nameOfStrategyToUse )
        {

            IParseCoverageStrategy aParseCoverageStrategy = null;

            switch( nameOfStrategyToUse )
            {

                case COMMERCIAL_COVERAGE:
                    aParseCoverageStrategy = new ParseCommercialStrategy();
                    break;

                case MEDICARE_COVERAGE:
                    aParseCoverageStrategy = new ParseMedicareStrategy();
                    break;

                case GOV_OTHER_COVERAGE:
                    aParseCoverageStrategy = new ParseGovernmentOtherStrategy();
                    break;

                case MEDICAID_COVERAGE:
                    aParseCoverageStrategy = new ParseMedicaidStrategy();
                    break;

                case COMMERCIAL_COVERAGE_INOUT:
                    aParseCoverageStrategy = new ParseCommercialStrategy();
                    break;

                default:
                    throw new ArgumentException(
                        String.Format(
                            "Unable to create a benefits parsing strategy for {0}",
                            nameOfStrategyToUse ),
                        "nameOfStrategyToUse" );

            }//switch

            return aParseCoverageStrategy;

        }//method


        /// <summary>
        /// Determines whether [is config parse coverage on for] [the specified name of strategy to use].
        /// </summary>
        /// <param name="nameOfStrategyToUse">The name of strategy to use.</param>
        /// <returns>
        /// 	<c>true</c> if [is config parse coverage on for] [the specified name of strategy to use]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsConfigParseCoverageOnFor( string nameOfStrategyToUse )
        {

            bool configParseIsOn = true;

            switch( nameOfStrategyToUse )
            {

                case COMMERCIAL_COVERAGE:
                    configParseIsOn =
                        bool.Parse( ConfigurationManager.AppSettings[PARSE_COMMERCIAL] );
                    break;

                case MEDICARE_COVERAGE:
                    configParseIsOn =
                        bool.Parse( ConfigurationManager.AppSettings[PARSE_GOVERNMENT_MEDICARE] );
                    break;

                case GOV_OTHER_COVERAGE:
                    configParseIsOn =
                        bool.Parse( ConfigurationManager.AppSettings[PARSE_GOVERNMENT_OTHER] );
                    break;

                case MEDICAID_COVERAGE:
                    configParseIsOn =
                        bool.Parse( ConfigurationManager.AppSettings[PARSE_GOVERNMENT_MEDICAID] );
                    break;

                case COMMERCIAL_COVERAGE_INOUT:
                    configParseIsOn =
                        bool.Parse( ConfigurationManager.AppSettings[PARSE_COMMERCIAL] );
                    break;

                //OTD# 37594 fix for - PAS crashes when trying to determine parsing strategy for WorkersCompensationCoverage 
                case WORKERS_COMP_COVERAGE:
                    configParseIsOn = false;
                    break;

                case SELF_PAY_COVERAGE:
                    configParseIsOn = false;
                    break;

                default:
                    throw new ArgumentException(
                        String.Format(
                            "Unable to determine enabled state of parsing strategy for {0}",
                            nameOfStrategyToUse ),
                        "nameOfStrategyToUse" );

            }//switch

            return configParseIsOn;

        }//method

        #endregion

    }//class

}//namespace
