using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{

    /// <summary>
    /// Parses the XML Response for Medicaid from the EDV service
    /// </summary>
    class ParseMedicaidStrategy : ParseStrategy
    {

        #region Constants

        private const string CONFIG_MEDICAID_PAYORID_PATTERN = "RegexPatternForMedicaidPayorId";

        private const string ELIGIBILITY_DATE_PATH_DATE_RANGE_1 = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/DatePeriod[DateTimeQualifier='307' or DateTimeQualifier='291']/DateTimePeriod";

        private const string ELIGIBILITY_DATE_PATH_DATE_RANGE_2 = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation[BenefitInformation/EligibilityOrBenefitInformation='1' and BenefitInformation/ServiceTypeCode='30']/DatePeriod[DateTimeQualifier='307' or DateTimeQualifier='291']/DateTimePeriod";

        private const string ELIGIBILITY_DATE_PATH_START_DATE_1 = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/DatePeriod[DateTimeQualifier='356']/DateTimePeriod";

        private const string ELIGIBILITY_DATE_PATH_START_DATE_2 = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation[BenefitInformation/EligibilityOrBenefitInformation='1' and BenefitInformation/ServiceTypeCode='30']/DatePeriod[DateTimeQualifier='356']/DateTimePeriod";

        private const string EVC_NUMBER_PATH = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/TraceNumber[TraceTypeCode=1 and OriginatingCompanyIdentifier=9610442]/ReferenceIdentification1";

        private const int LENGTH_OF_DATE_STRING = 8;

        private const string MEDICAID_CALIFORNIA_ID = "MEDICAID-CA";

        private const string MEDICAID_COPAY_PATH = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='B']/MonetaryAmount";

        #endregion

        #region Fields

        private static Regex c_MedicaidPayorIdRegularExpression;

        private readonly MedicaidConstraints i_MedicaidConstraints =
                            new MedicaidConstraints();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="ParseMedicaidStrategy"/> class.
        /// </summary>
        static ParseMedicaidStrategy()
        {

            string medicaidRegularExpressionPattern =
                ConfigurationManager.AppSettings[CONFIG_MEDICAID_PAYORID_PATTERN];

            if( !string.IsNullOrEmpty( medicaidRegularExpressionPattern ) )
            {
                c_MedicaidPayorIdRegularExpression =
                    new Regex( medicaidRegularExpressionPattern );
                if( c_MedicaidPayorIdRegularExpression == null ) {}
            } //if
            else
            {

                c_MedicaidPayorIdRegularExpression = null;

            }//else

        }//method

        #endregion

        #region Non-Public Properties

        /// <summary>
        /// Gets or sets the medicaid constraints.
        /// </summary>
        /// <value>The medicaid constraints.</value>
        private MedicaidConstraints MedicaidConstraints
        {

            get
            {

                return this.i_MedicaidConstraints;

            }//get

        }//property

        /// <summary>
        /// Gets the payor id regular expression.
        /// </summary>
        /// <value>The payor id regular expression.</value>
        protected override Regex PayorIdRegularExpression
        {

            get
            {

                return c_MedicaidPayorIdRegularExpression;

            }//get

        }//property

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {

            base.Execute();

            if( this.IsPayorEnabledById )
            {

                string evcNumber;
                float coPayAmount;
                DateTime eligibilityDate;

                if( !this.TryParseEvcNumber( out evcNumber ) )
                {

                    evcNumber = String.Empty;

                }//if
                this.MedicaidConstraints.EVCNumber = evcNumber;

                if( !this.TryParseCoPay( out coPayAmount ) )
                {

                    coPayAmount = 0.0F;

                }//if
                this.MedicaidConstraints.MedicaidCopay = coPayAmount;

                if( !this.TryParseEligibilityDate( out eligibilityDate ) )
                {

                    eligibilityDate = DateTime.MaxValue;

                }//if
                this.MedicaidConstraints.EligibilityDate = eligibilityDate;

                this.TheBenefitsValidationResponse
                    .CoverageConstraintsCollection
                    .Add( this.MedicaidConstraints );

            }//if

        }//method

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Tries the parse co pay.
        /// </summary>
        /// <param name="coPayAmount">The co pay amount.</param>
        /// <returns></returns>
        private bool TryParseCoPay( out float coPayAmount )
        {

            bool wasParseSuccessful = false;

            coPayAmount = 0.0F;

            try
            {

                XmlNode coPayAmountNode =
                    this.XmlResponseDocument.SelectSingleNode( MEDICAID_COPAY_PATH );

                if( null != coPayAmountNode )
                {

                    wasParseSuccessful =
                        float.TryParse( coPayAmountNode.InnerText.Trim(),
                                        out coPayAmount );

                }//if

            }//try
            catch
            {

                wasParseSuccessful = false;

            }//catch

            return wasParseSuccessful;

        }//method


        /// <summary>
        /// Tries the parse eligibility date.
        /// </summary>
        /// <param name="eligibilityDate">The eligibility date.</param>
        /// <returns></returns>
        private bool TryParseEligibilityDate( out DateTime eligibilityDate )
        {

            bool wasParseSuccessful = false;

            eligibilityDate = DateTime.MaxValue;

            try
            {

                if( !this.TryParseEligibilityDateAsDateRange(
                        ELIGIBILITY_DATE_PATH_DATE_RANGE_1,
                        out eligibilityDate ) )
                {

                    if( !this.TryParseEligibilityDateAsDateRange(
                            ELIGIBILITY_DATE_PATH_DATE_RANGE_2,
                            out eligibilityDate ) )
                    {

                        if( !this.TryParseEligibilityDateAsSingleValue(
                                ELIGIBILITY_DATE_PATH_START_DATE_1,
                                out eligibilityDate ) )
                        {

                            wasParseSuccessful =
                                this.TryParseEligibilityDateAsSingleValue(
                                    ELIGIBILITY_DATE_PATH_START_DATE_2,
                                    out eligibilityDate );

                        }//if

                    }//if

                }//if

            }//try
            catch
            {

                wasParseSuccessful = false;

            }//catch

            return wasParseSuccessful;

        }//method


        /// <summary>
        /// Tries the parse eligibility date as date range.
        /// </summary>
        /// <param name="xpathQuery">The xpath query.</param>
        /// <param name="eligibiltyDate">The eligibilty date.</param>
        /// <returns></returns>
        private bool TryParseEligibilityDateAsDateRange( string xpathQuery, out DateTime eligibiltyDate )
        {

            bool wasParseSuccessful = false;

            eligibiltyDate = DateTime.MaxValue;

            try
            {

                XmlNode eligbilityDateRangeNode =
                    this.XmlResponseDocument
                        .SelectSingleNode( xpathQuery );

                if( null != eligbilityDateRangeNode )
                {

                    string parsedStartDate =
                        eligbilityDateRangeNode.InnerText
                                               .Trim()
                                               .Substring( 0, LENGTH_OF_DATE_STRING );

                    wasParseSuccessful =
                        DateTime.TryParse( parsedStartDate,
                                           out eligibiltyDate );

                }//if

            }//try
            catch
            {

                wasParseSuccessful = false;

            }//catch

            return wasParseSuccessful;

        }//method


        /// <summary>
        /// Tries the parse eligibility date as single value.
        /// </summary>
        /// <param name="xpathQuery">The xpath query.</param>
        /// <param name="eligibiltyDate">The eligibilty date.</param>
        /// <returns></returns>
        private bool TryParseEligibilityDateAsSingleValue( string xpathQuery, out DateTime eligibiltyDate )
        {

            bool wasParseSuccessful = false;

            eligibiltyDate = DateTime.MaxValue;

            try
            {

                XmlNode eligbilityDateNode =
                    this.XmlResponseDocument
                        .SelectSingleNode( xpathQuery );

                if( null != eligbilityDateNode )
                {

                    wasParseSuccessful =
                        DateTime.TryParse( eligbilityDateNode.InnerText.Trim(),
                                           out eligibiltyDate );

                }//if

            }//try
            catch
            {

                wasParseSuccessful = false;

            }//catch

            return wasParseSuccessful;

        }//method


        /// <summary>
        /// Tries the parse evc number.
        /// </summary>
        /// <param name="evcNumber">The evc number.</param>
        /// <returns></returns>
        /// <remarks>
        /// This is only applicable in California
        /// </remarks>
        private bool TryParseEvcNumber( out string evcNumber )
        {

            bool wasParseSuccessful = false;

            evcNumber = String.Empty;

            try
            {

                // The EVC number is only applicable to Medicaid in California
                if( this.ResponsePayorId.Equals( MEDICAID_CALIFORNIA_ID ) )
                {

                    XmlNode evcNumberNode =
                        this.XmlResponseDocument.SelectSingleNode( EVC_NUMBER_PATH );

                    if( null != evcNumberNode )
                    {

                        evcNumber = evcNumberNode.InnerText.Trim();
                        wasParseSuccessful = true;

                    }//if

                }//if

            }//try
            catch
            {

                wasParseSuccessful = false;

            }//catch

            return wasParseSuccessful;

        }//method

        #endregion

    }//class

}//namespace
