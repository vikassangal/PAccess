using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PatientAccess.Domain;
using PatientAccess.Services.BenefitsValidation;
using log4net;

namespace PatientAccess.Persistence
{

    /// <summary>
    /// 
    /// </summary>
    abstract class ParseStrategy : IParseCoverageStrategy
    {

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="ParseStrategy"/> class.
        /// </summary>
        static ParseStrategy()
        {

            XmlSchema benefitsValidationSchema =
                XmlSchema.Read(
                    Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream(BENEFITS_XML_SCHEMA_PATH),
                    HandleSchemaValidationEvent);

            TheXmlSchemaSet.Add(benefitsValidationSchema);

        }//method

        #endregion

        #region Non-Public Properties

        /// <summary>
        /// Gets or sets the benefits.
        /// </summary>
        /// <value>The benefits.</value>
        protected ArrayList Benefits
        {

            get 
            { 

                return this.i_Benefits; 

            }//get
            set 
            { 

                this.i_Benefits = value; 

            }//set

        }//property


        /// <summary>
        /// Gets the effective date for insured.
        /// </summary>
        /// <value>The effective date for insured.</value>
        protected DateTime EffectiveDateForInsured
        {
            get
            {
                DateTime result = DateTime.MinValue;

                if (this.TheBenefitResponse.InformationSourceLevel != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.DatePeriod != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.DatePeriod.DateTimePeriod != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.DatePeriod.DateTimeQualifier != null)
                {

                    SubscriberName aSubscriberName = TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName;

                    for( int i = 0; i < aSubscriberName.SubscriberBenefitInformation.Length; i++ )
                    {
                        SubscriberBenefitInformation subscriberBenefitInfo = aSubscriberName.SubscriberBenefitInformation[i];

                        if( subscriberBenefitInfo.BenefitInformation != null && subscriberBenefitInfo.BenefitInformation.ServiceTypeCode != null )
                        {
                            if( subscriberBenefitInfo.BenefitInformation.ServiceTypeCode.Text == SERVICE_TYPE_CODE_GENERAL )
                            {

                                if( subscriberBenefitInfo.DatePeriod != null
                                    && subscriberBenefitInfo.DatePeriod[0].DateTimeQualifier != null
                                    && subscriberBenefitInfo.DatePeriod[0].DateTimePeriod != null )
                                {
                                    if( subscriberBenefitInfo.DatePeriod[0].DateTimeQualifier.Text == DATE_TIME_QUALIFIER_BENEFIT_EFFECTIVE )
                                    {
                                        try
                                        {
                                            if( subscriberBenefitInfo.DatePeriod[0].DateTimePeriod.Text.Length > 0 )
                                            {
                                                result = DateTime.Parse( this.FormatDate( subscriberBenefitInfo.DatePeriod[0].DateTimePeriod.Text ) );
                                                break;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if( result == DateTime.MinValue )
                    {
                        DatePeriod aDatePeriod = this.TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.DatePeriod;
                        if( aDatePeriod != null && aDatePeriod.DateTimeQualifier.Text == DATE_TIME_QUALIFIER_POLICY_EFFECTIVE )
                        {
                            try
                            {
                                if( aDatePeriod.DateTimePeriod.Text.Length > 0 )
                                {
                                    result = DateTime.Parse( this.FormatDate( aDatePeriod.DateTimePeriod.Text ) );
                                }

                            }
                            catch
                            {
                            }
                        }
                        else if( aDatePeriod != null && DATE_TIME_QUALIFIER_POLICY_DATE_RANGE.Contains(aDatePeriod.DateTimeQualifier.Text) )
                        {
                            int dashIndex = aDatePeriod.DateTimePeriod.Text.IndexOf( "-" );
                            if( dashIndex != -1 )
                            {
                                try
                                {
                                    string date = aDatePeriod.DateTimePeriod.Text.Substring( 0, dashIndex - 1 );
                                    result = DateTime.Parse( this.FormatDate( date ) );
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                string date = this.FormatDate( aDatePeriod.DateTimePeriod.Text );
                                result = DateTime.Parse( this.FormatDate( date ) );
                            }
                        }
                    }

                    if (result == DateTime.MinValue)
                    {
                        for (int i = 0; i < aSubscriberName.SubscriberBenefitInformation.Length; i++)
                        {
                            SubscriberBenefitInformation subscriberBenefitInfo = aSubscriberName.SubscriberBenefitInformation[i];

                            if (subscriberBenefitInfo.BenefitInformation != null && subscriberBenefitInfo.BenefitInformation.ServiceTypeCode != null)
                            {
                                if (subscriberBenefitInfo.BenefitInformation.ServiceTypeCode.Text == SERVICE_TYPE_CODE_GENERAL)
                                {

                                    if (subscriberBenefitInfo.DatePeriod != null
                                        && subscriberBenefitInfo.DatePeriod[0].DateTimeQualifier != null
                                        && subscriberBenefitInfo.DatePeriod[0].DateTimePeriod != null )
                                    {
                                        if( subscriberBenefitInfo.DatePeriod[0].DateTimeQualifier.Text == DATE_TIME_QUALIFIER_POLICY_EFFECTIVE )
                                        {
                                            try
                                            {
                                                if( subscriberBenefitInfo.DatePeriod[0].DateTimePeriod.Text.Length > 0 )
                                                {
                                                    result = DateTime.Parse( this.FormatDate( subscriberBenefitInfo.DatePeriod[0].DateTimePeriod.Text ) );
                                                    break;
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// Gets the eligibility phone.
        /// </summary>
        /// <value>The eligibility phone.</value>
        protected string EligibilityPhone
        {
            get
            {
                string result = string.Empty;

                if (this.TheBenefitResponse.InformationSourceLevel != null
                    && this.TheBenefitResponse.InformationSourceLevel.InformationSourceName != null
                    && this.TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation != null
                    && this.TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation.CommunicationNumberQualifier1.Text == COMM_NUMBER_QUALIFIER_TE
                    && this.TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation.ContactFunctionCode.Text == CONTACT_FUNCTION_CODE_IC)
                {
                    result = this.TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation.CommunicationNumber1.Text;
                }

                return result;
            }
        }


        /// <summary>
        /// Gets the name of the insurance company rep.
        /// </summary>
        /// <value>The name of the insurance company rep.</value>
        protected string InsuranceCompanyRepName
        {
            get
            {
                string result = string.Empty;

                if (this.TheBenefitResponse.InformationSourceLevel != null
                    && this.TheBenefitResponse.InformationSourceLevel.InformationSourceName != null
                    && this.TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation != null
                    && this.TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation.ContactName != null
                    && this.TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation.ContactFunctionCode.Text == CONTACT_FUNCTION_CODE_IC)
                {
                    result = this.TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation.ContactName.Text;
                }

                return result;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is government other.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is government other; otherwise, <c>false</c>.
        /// </value>
        protected bool IsGovernmentOther
        {
            get
            {
                return i_IsGovernmentOther;
            }
            set
            {
                i_IsGovernmentOther = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is payor enabled by id.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled by id; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If there is no regular expression present, then all payors are assumed to
        /// be enabled
        /// </remarks>
        protected bool IsPayorEnabledById
        {

            get
            {

                bool isPayorEnabled = true;

                if ( null != this.PayorIdRegularExpression
                     && this.ResponsePayorId != null )
                {

                    isPayorEnabled =
                        this.PayorIdRegularExpression.IsMatch(this.ResponsePayorId.Trim());

                }//if

                return isPayorEnabled;

            }//get

        }//property


        /// <summary>
        /// Gets or sets a value indicating whether this instance is XML response valid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is XML response valid; otherwise, <c>false</c>.
        /// </value>
        private bool IsXmlResponseValid
        {

            get
            {

                return this.i_IsXmlResponseValid;

            }//get
            set
            {

                this.i_IsXmlResponseValid = value;

            }//set

        }//property


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

        }//Logger


        /// <summary>
        /// Gets the payor id regular expression.
        /// </summary>
        /// <value>The payor id regular expression.</value>
        /// <remarks>
        /// Template for child classes
        /// </remarks>
        protected abstract Regex PayorIdRegularExpression
        {

            get;

        }//property


        /// <summary>
        /// Gets the name of the response auth co.
        /// </summary>
        /// <value>The name of the response auth co.</value>
        private string ResponseAuthCoName
        {

            get
            {
                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation != null)
                {

                    ContactInformation contactInformation = TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation;

                    if (contactInformation.ContactFunctionCode != null && contactInformation.ContactName != null)
                    {
                        if (contactInformation.ContactFunctionCode.Text == AUTHORIZATION_CO_NAME)
                        {
                            result = contactInformation.ContactName.Text;
                        }
                    }
                }
                return result;
            }

        }


        /// <summary>
        /// Gets the response auth co phone.
        /// </summary>
        /// <value>The response auth co phone.</value>
        private string ResponseAuthCoPhone
        {

            get
            {
                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation != null)
                {

                    ContactInformation contactInformation = TheBenefitResponse.InformationSourceLevel.InformationSourceName.ContactInformation;

                    if (contactInformation.ContactFunctionCode != null && contactInformation.CommunicationNumber1 != null &&
                        contactInformation.CommunicationNumberQualifier1 != null)
                    {
                        if (contactInformation.ContactFunctionCode.Text == AUTHORIZATION_CO_PHONE &&
                            contactInformation.CommunicationNumberQualifier1.Text == AUTHORIZATION_CO_NAME_QUALIFIER)
                        {
                            result = contactInformation.CommunicationNumber1.Text;
                        }
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// Gets the response group number.
        /// </summary>
        /// <value>The response group number.</value>
        private string ResponseGroupNumber
        {
            get
            {
                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                        SubscriberName != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                        SubscriberName.AdditionalIdentification != null)
                {

                    AdditionalIdentification additionalIdentification = TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                        SubscriberName.AdditionalIdentification;

                    if (additionalIdentification.ReferenceIdentificationQualifier != null && additionalIdentification.ReferenceIdentification != null)
                    {
                        if (additionalIdentification.ReferenceIdentificationQualifier.Text == GROUP_NUMBER)
                        {
                            result = additionalIdentification.ReferenceIdentification.Text;
                        }
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// Gets the response insured DOB.
        /// </summary>
        /// <value>The response insured DOB.</value>
        private string ResponseInsuredDOB
        {
            get
            {
                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                        SubscriberName != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                        SubscriberName.Demographic != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                        SubscriberName.Demographic.DateTimePeriod != null)
                {

                    result = TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                         SubscriberName.Demographic.DateTimePeriod.Text;

                    try
                    {
                        result = DateTime.Parse( result ).ToShortDateString();
                    }
                    catch
                    {
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// Gets the name of the response insured first.
        /// </summary>
        /// <value>The name of the response insured first.</value>
        private string ResponseInsuredFirstName
        {
            get
            {
                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName
                        .Name != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName
                        .Name.NameFirst != null)
                {

                    result = TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.
                        SubscriberLevel.SubscriberName.Name.NameFirst.Text;
                }
                return result;
            }
        }


        /// <summary>
        /// Gets the name of the response insured last.
        /// </summary>
        /// <value>The name of the response insured last.</value>
        private string ResponseInsuredLastName
        {
            get
            {
                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName
                        .Name != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName
                        .Name.NameLastOrOrganizationName != null)
                {

                    result = TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.
                        SubscriberLevel.SubscriberName.Name.NameLastOrOrganizationName.Text;
                }
                return result;
            }
        }


        /// <summary>
        /// Gets the response insured middle initial.
        /// </summary>
        /// <value>The response insured middle initial.</value>
        private string ResponseInsuredMiddleInitial
        {
            get
            {
                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName
                        .Name != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName
                        .Name.NameMiddle != null)
                {

                    result = TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.
                        SubscriberLevel.SubscriberName.Name.NameMiddle.Text;
                }
                return result;
            }
        }


        /// <summary>
        /// Gets the response payor id.
        /// </summary>
        /// <value>The response payor id.</value>
        protected string ResponsePayorId
        {
            get
            {

                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName.Name != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName.Name.IdentificationCode != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName.Name.IdentificationCodeQualifier != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName.Name.IdentificationCodeQualifier.Text.Equals("PI")
                    )
                {

                    result = TheBenefitResponse.InformationSourceLevel.InformationSourceName.Name.IdentificationCode.Text;

                }

                return result;

            }
        }


        /// <summary>
        /// Gets the name of the response payor.
        /// </summary>
        /// <value>The name of the response payor.</value>
        private string ResponsePayorName
        {
            get
            {
                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName.Name != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationSourceName.Name.
                        NameLastOrOrganizationName != null)
                {

                    result = TheBenefitResponse.InformationSourceLevel.InformationSourceName.Name.
                        NameLastOrOrganizationName.Text;
                }
                return result;
            }
        }


        /// <summary>
        /// Gets the response subscriber ID.
        /// </summary>
        /// <value>The response subscriber ID.</value>
        private string ResponseSubscriberID
        {
            get
            {
                string result = string.Empty;

                if (TheBenefitResponse != null &&
                    TheBenefitResponse.InformationSourceLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                        SubscriberName != null &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                        SubscriberName.Name != null)
                {

                    Name name = TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.
                        SubscriberName.Name;

                    if (name.IdentificationCodeQualifier != null && name.IdentificationCode != null)
                    {
                        if (name.IdentificationCodeQualifier.Text == SUBSCRIBER_ID)
                        {
                            result = name.IdentificationCode.Text;
                        }
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// Gets the termination date for insured.
        /// </summary>
        /// <value>The termination date for insured.</value>
        protected DateTime TerminationDateForInsured
        {
            get
            {
                DateTime result = DateTime.MinValue;

                if( this.TheBenefitResponse.InformationSourceLevel != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName != null
                    && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.DatePeriod != null )
                {
                    SubscriberName aSubscriberName = TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName;

                    for( int i = 0; i < aSubscriberName.SubscriberBenefitInformation.Length; i++ )
                    {
                        SubscriberBenefitInformation subscriberBenefitInfo = aSubscriberName.SubscriberBenefitInformation[i];

                        if( subscriberBenefitInfo.BenefitInformation != null && subscriberBenefitInfo.BenefitInformation.ServiceTypeCode != null )
                        {
                            if( subscriberBenefitInfo.BenefitInformation.ServiceTypeCode.Text == SERVICE_TYPE_CODE_GENERAL )
                            {

                                if( subscriberBenefitInfo.DatePeriod != null
                                    && subscriberBenefitInfo.DatePeriod[0] != null
                                    && subscriberBenefitInfo.DatePeriod[0].DateTimeQualifier != null
                                    && subscriberBenefitInfo.DatePeriod[0].DateTimePeriod != null )
                                {
                                    if( subscriberBenefitInfo.DatePeriod[0].DateTimeQualifier.Text == DATE_TIME_QUALIFIER_BENEFIT_EXPIRATION )
                                    {
                                        try
                                        {
                                            if( subscriberBenefitInfo.DatePeriod[0].DateTimePeriod.Text.Length > 0 )
                                            {
                                                result = DateTime.Parse( this.FormatDate( subscriberBenefitInfo.DatePeriod[0].DateTimePeriod.Text ) );
                                                break;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if( result == DateTime.MinValue )
                    {
                        DatePeriod aDatePeriod = this.TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.DatePeriod;

                        if( TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.DatePeriod.DateTimeQualifier.Text == DATE_TIME_QUALIFIER_POLICY_EXPIRATION )
                        {

                            string strDate = this.TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.DatePeriod.DateTimePeriod.Text;
                            if( strDate != string.Empty )
                            {
                                try
                                {
                                    result = DateTime.Parse( this.FormatDate( strDate ) );
                                }
                                catch
                                {
                                }
                            }
                        }
                        else if (aDatePeriod != null && DATE_TIME_QUALIFIER_POLICY_DATE_RANGE.Contains(aDatePeriod.DateTimeQualifier.Text))
                        {
                            int dashIndex = aDatePeriod.DateTimePeriod.Text.IndexOf( "-" );
                            if( dashIndex != -1 )
                            {
                                try
                                {
                                    string date = aDatePeriod.DateTimePeriod.Text.Substring( dashIndex + 1 );
                                    result = DateTime.Parse( this.FormatDate( date ) );
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                result = DateTime.MinValue;
                            }
                        }
                    }

                    if( result == DateTime.MinValue
                        && TheBenefitResponse.InformationSourceLevel != null
                        && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel != null
                        && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel != null
                        && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName != null
                        && TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.SubscriberBenefitInformation != null )
                    {
                        for( int i = 0; i < aSubscriberName.SubscriberBenefitInformation.Length; i++ )
                        {
                            SubscriberBenefitInformation subscriberBenefitInfo = aSubscriberName.SubscriberBenefitInformation[i];

                            if( subscriberBenefitInfo.BenefitInformation != null && subscriberBenefitInfo.BenefitInformation.ServiceTypeCode != null )
                            {
                                if( subscriberBenefitInfo.BenefitInformation.ServiceTypeCode.Text == SERVICE_TYPE_CODE_GENERAL )
                                {

                                    if( subscriberBenefitInfo.DatePeriod != null
                                        && subscriberBenefitInfo.DatePeriod[0] != null
                                        && subscriberBenefitInfo.DatePeriod[0].DateTimeQualifier != null
                                        && subscriberBenefitInfo.DatePeriod[0].DateTimePeriod != null )
                                    {
                                        if( subscriberBenefitInfo.DatePeriod[0].DateTimeQualifier.Text == DATE_TIME_QUALIFIER_POLICY_EXPIRATION )
                                        {
                                            try
                                            {
                                                if( subscriberBenefitInfo.DatePeriod[0].DateTimePeriod.Text.Length > 0 )
                                                {
                                                    result = DateTime.Parse( this.FormatDate( subscriberBenefitInfo.DatePeriod[0].DateTimePeriod.Text ) );
                                                    break;
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// Gets or sets the benefit response.
        /// </summary>
        /// <value>The benefit response.</value>
        protected BenefitResponse TheBenefitResponse
        {

            get 
            { 

                return this.i_BenefitResponse; 

            }//get
            private set 
            { 

                this.i_BenefitResponse = value; 

            }//set

        }//property


        /// <summary>
        /// Gets or sets the benefits validation response.
        /// </summary>
        /// <value>The benefits validation response.</value>
        protected BenefitsValidationResponse TheBenefitsValidationResponse
        {

            get 
            { 

                return this.i_BenefitsValidationResponse; 

            }//get
            private set 
            { 

                this.i_BenefitsValidationResponse = value; 

            }//set

        }//protected


        /// <summary>
        /// Gets the XML schema set.
        /// </summary>
        /// <value>The XML schema set.</value>
        private static XmlSchemaSet TheXmlSchemaSet
        {

            get
            {

                return i_XmlSchemaSet;

            }//get

        }//property


        /// <summary>
        /// Gets or sets the XML response document.
        /// </summary>
        /// <value>The XML response document.</value>
        protected XmlDocument XmlResponseDocument
        {

            get
            {

                return this.i_XmlResponse;

            }//get

        }//property

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public virtual void Execute()
        {

            this.LoadXmlResponseDocument();

            if (this.IsXmlResponseValid)
            {

                this.DeserializeBenefitResponse();

                if (null != TheBenefitResponse &&
                    null != TheBenefitResponse.InformationSourceLevel &&
                    null != TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel &&
                    null != TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel &&
                    null != TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName &&
                    null != TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.SubscriberBenefitInformation &&
                    TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.SubscriberBenefitInformation.Length > 0)
                {

                    foreach (SubscriberBenefitInformation aSubscriberBenefitInformation in TheBenefitResponse.InformationSourceLevel.InformationReceiverLevel.SubscriberLevel.SubscriberName.SubscriberBenefitInformation)
                    {

                        BenefitsInformation aBenefitsInformation = new BenefitsInformation();

                        if (null != aSubscriberBenefitInformation.BenefitInformation.CoverageLevelCode)
                        {

                            aBenefitsInformation.CoverageLevelCode =
                                aSubscriberBenefitInformation.BenefitInformation.CoverageLevelCode.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.EligibilityOrBenefitInformation)
                        {

                            aBenefitsInformation.EligibilityOrBenefitInformation =
                                aSubscriberBenefitInformation.BenefitInformation.EligibilityOrBenefitInformation.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.InsuranceTypeCode)
                        {

                            aBenefitsInformation.InsuranceTypeCode =
                                aSubscriberBenefitInformation.BenefitInformation.InsuranceTypeCode.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.MonetaryAmount)
                        {

                            aBenefitsInformation.MonetaryAmount =
                                aSubscriberBenefitInformation.BenefitInformation.MonetaryAmount.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.Percent)
                        {

                            aBenefitsInformation.Percent =
                                aSubscriberBenefitInformation.BenefitInformation.Percent.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.PlanCoverageDescription)
                        {

                            aBenefitsInformation.PlanCoverageDescription =
                                aSubscriberBenefitInformation.BenefitInformation.PlanCoverageDescription.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.Quantity)
                        {

                            aBenefitsInformation.Quantity =
                                aSubscriberBenefitInformation.BenefitInformation.Quantity.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.QuantityQualifier)
                        {

                            aBenefitsInformation.QuantityQualifier =
                                aSubscriberBenefitInformation.BenefitInformation.QuantityQualifier.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.ServiceTypeCode)
                        {

                            aBenefitsInformation.ServiceTypeCode =
                                aSubscriberBenefitInformation.BenefitInformation.ServiceTypeCode.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.TimePeriodQualifier)
                        {

                            aBenefitsInformation.TimePeriodQualifier =
                                aSubscriberBenefitInformation.BenefitInformation.TimePeriodQualifier.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.YesNoConditionOrResponseCode1)
                        {

                            aBenefitsInformation.YesNoConditionOrResponseCode1 =
                                aSubscriberBenefitInformation.BenefitInformation.YesNoConditionOrResponseCode1.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.BenefitInformation.YesNoConditionOrResponseCode2)
                        {

                            aBenefitsInformation.YesNoConditionOrResponseCode2 =
                                aSubscriberBenefitInformation.BenefitInformation.YesNoConditionOrResponseCode2.Text;

                        }//if

                        if (null != aSubscriberBenefitInformation.MessageText &&
                            null != aSubscriberBenefitInformation.MessageText.FreeFormMessageText)
                        {

                            aBenefitsInformation.Message =
                                aSubscriberBenefitInformation.MessageText.FreeFormMessageText.Text;

                        }//if

                        if( null != aSubscriberBenefitInformation.DatePeriod && aSubscriberBenefitInformation.DatePeriod.Length > 0 && null != aSubscriberBenefitInformation.DatePeriod[0] )
                        {

                            if( null != aSubscriberBenefitInformation.DatePeriod[0].DateTimePeriod )
                            {

                                aBenefitsInformation.DateTimePeriod = aSubscriberBenefitInformation.DatePeriod[0].DateTimePeriod.Text;

                            }//if

                            if( null != aSubscriberBenefitInformation.DatePeriod[0].DateTimePeriodFormatQualifier )
                            {

                                aBenefitsInformation.DateTimePeriodFormatQualifier =
                                    aSubscriberBenefitInformation.DatePeriod[0].DateTimePeriodFormatQualifier.Text;

                            }//if

                            if( null != aSubscriberBenefitInformation.DatePeriod[0].DateTimeQualifier )
                            {

                                aBenefitsInformation.DateTimeQualifier =
                                    aSubscriberBenefitInformation.DatePeriod[0].DateTimeQualifier.Text;

                            }//if

                        }//if

                        if( null != aSubscriberBenefitInformation.DatePeriod &&  aSubscriberBenefitInformation.DatePeriod.Length > 1 && null != aSubscriberBenefitInformation.DatePeriod[1] )
                        {

                            if( null != aSubscriberBenefitInformation.DatePeriod[1].DateTimePeriod )
                            {

                                aBenefitsInformation.DateTimePeriod2 = aSubscriberBenefitInformation.DatePeriod[1].DateTimePeriod.Text;

                            }//if

                            if( null != aSubscriberBenefitInformation.DatePeriod[1].DateTimePeriodFormatQualifier )
                            {

                                aBenefitsInformation.DateTimePeriodFormatQualifier2 =
                                    aSubscriberBenefitInformation.DatePeriod[1].DateTimePeriodFormatQualifier.Text;

                            }//if

                            if( null != aSubscriberBenefitInformation.DatePeriod[1].DateTimeQualifier )
                            {

                                aBenefitsInformation.DateTimeQualifier2 =
                                    aSubscriberBenefitInformation.DatePeriod[1].DateTimeQualifier.Text;

                            }//if

                        }//if

                        this.Benefits.Add(aBenefitsInformation);

                    }//foreach

                }//if

                string benefitsResponseParseStrategyName =
                    TheBenefitsValidationResponse.ReturnedDataValidationTicket
                                                .BenefitsResponse
                                                .BenefitsResponseParseStrategy;

                if (benefitsResponseParseStrategyName.Equals(COMMERCIAL_COVERAGE_NAME) ||
                    benefitsResponseParseStrategyName.Equals(GOVERNMENT_OTHER_NAME) ||
                    benefitsResponseParseStrategyName.Equals(WORKERS_COMP_NAME))
                {

                    TheBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseAuthCo = this.ResponseAuthCoName;
                    TheBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseAuthCoPhone = this.ResponseAuthCoPhone;

                }//if

                TheBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseGroupNumber = this.ResponseGroupNumber;
                TheBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseInsuredDOB = this.ResponseInsuredDOB;
                TheBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseInsuredFirstName = this.ResponseInsuredFirstName;
                TheBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseInsuredMiddleInitial = this.ResponseInsuredMiddleInitial;
                TheBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseInsuredLastName = this.ResponseInsuredLastName;
                TheBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponsePayorName = this.ResponsePayorName;
                TheBenefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.ResponseSubscriberID = this.ResponseSubscriberID;

            }//if

        }

        public ArrayList GetCoverages(string elegibilityType, string insTypeCode, string serviceTypeCode,
                            string timePeriodQualifier, string quantityQualifier)
        {
            ArrayList list = new ArrayList();

            foreach (BenefitsInformation bi in this.Benefits)
            {
                if ( bi.EligibilityOrBenefitInformation == elegibilityType
                    && bi.InsuranceTypeCode == insTypeCode
                    && bi.ServiceTypeCode == serviceTypeCode
                    && bi.TimePeriodQualifier == timePeriodQualifier
                    && bi.QuantityQualifier == quantityQualifier )
                {
                    list.Add(bi);
                }
            }

            return list;
        }

        public ArrayList GetCoverages(string elegibilityType)
        {
            ArrayList list = new ArrayList();

            foreach (BenefitsInformation bi in this.Benefits)
            {
                if (bi.EligibilityOrBenefitInformation == elegibilityType)
                {
                    list.Add(bi);
                }
            }

            return list;
        }

        public ArrayList GetCoverages(string elegibilityType, string coverageLevelCode, string serviceTypeCode,
                            bool inNetworkIndicator, string timePeriodQualifier)
        {
            ArrayList list = new ArrayList();

            foreach (BenefitsInformation bi in this.Benefits)
            {
                if (!IsGovernmentOther)
                {
                    if ( bi.EligibilityOrBenefitInformation == elegibilityType
                    && bi.CoverageLevelCode == coverageLevelCode
                    && bi.ServiceTypeCode == serviceTypeCode
                    && ((inNetworkIndicator && bi.YesNoConditionOrResponseCode2 == "Y")
                            ||
                         (!inNetworkIndicator && bi.YesNoConditionOrResponseCode2 == "N")
                       )
                    && bi.TimePeriodQualifier == timePeriodQualifier ) 
                    {
                        list.Add(bi);
                    }
                }
                else
                {
                    if ( bi.EligibilityOrBenefitInformation == elegibilityType
                    && bi.CoverageLevelCode == coverageLevelCode
                    && bi.ServiceTypeCode == serviceTypeCode
                    && bi.TimePeriodQualifier == timePeriodQualifier  ) 
                    {
                        list.Add(bi);
                    }
                }

            }

            return list;
        }

        public ArrayList GetCoverages(string elegibilityType, string insTypeCode, string dateTimeQualifier)
        {
            ArrayList list = new ArrayList();

            foreach (BenefitsInformation bi in this.Benefits)
            {
                if ( ( bi.EligibilityOrBenefitInformation == elegibilityType
                        && bi.InsuranceTypeCode == insTypeCode
                        && (
                            (bi.DateTimeQualifier != null && dateTimeQualifier.Contains(bi.DateTimeQualifier))
                            ||
                            (bi.DateTimeQualifier2 != null && dateTimeQualifier.Contains(bi.DateTimeQualifier2)) 
                            ) ) )                                       
                {
                    list.Add(bi);
                }
            }

            return list;
        }

        public ArrayList GetCoverages(string elegibilityType, string coverageLevelCode, string serviceTypeCode,
                            bool inNetworkIndicator, string timePeriodQualifier, string quantityQualifier)
        {
            ArrayList list = new ArrayList();

            foreach (BenefitsInformation bi in this.Benefits)
            {
                if ( !IsGovernmentOther )
                {
                    if ( bi.EligibilityOrBenefitInformation == elegibilityType
                    && bi.CoverageLevelCode == coverageLevelCode
                    && bi.ServiceTypeCode == serviceTypeCode
                    && ((inNetworkIndicator && bi.YesNoConditionOrResponseCode2 == "Y")
                            ||
                         (!inNetworkIndicator && bi.YesNoConditionOrResponseCode2 == "N")
                       )
                    && bi.TimePeriodQualifier == timePeriodQualifier
                    && bi.QuantityQualifier == quantityQualifier)
                    {
                        list.Add(bi);
                    }
                }
                else
                {
                    if ( bi.EligibilityOrBenefitInformation == elegibilityType
                    && bi.CoverageLevelCode == coverageLevelCode
                    && bi.ServiceTypeCode == serviceTypeCode
                    && bi.TimePeriodQualifier == timePeriodQualifier
                    && bi.QuantityQualifier == quantityQualifier)
                    {
                        list.Add(bi);
                    }
                }
            }

            return list;
        }

        public ArrayList GetCoverages(string elegibilityType, string coverageLevelCode, string serviceTypeCode,
                            bool inNetworkIndicator)
        {
            ArrayList list = new ArrayList();

            foreach (BenefitsInformation bi in this.Benefits)
            {
                if (!IsGovernmentOther)
                {
                    if (bi.EligibilityOrBenefitInformation == elegibilityType
                        && bi.CoverageLevelCode == coverageLevelCode
                        && bi.ServiceTypeCode == serviceTypeCode
                        && ((inNetworkIndicator && bi.YesNoConditionOrResponseCode2 == "Y")
                                ||
                             (!inNetworkIndicator && bi.YesNoConditionOrResponseCode2 == "N")
                           )
                       )
                    {
                        list.Add(bi);
                    }

                }
                else
                {
                    if (bi.EligibilityOrBenefitInformation == elegibilityType
                       && bi.CoverageLevelCode == coverageLevelCode
                       && bi.ServiceTypeCode == serviceTypeCode)
                    {
                        list.Add(bi);
                    }
                }

            }

            return list;
        }

        public ArrayList GetCoverages(string elegibilityType, string insTypeCode)
        {
            ArrayList list = new ArrayList();

            foreach (BenefitsInformation bi in this.Benefits)
            {
                if (bi.EligibilityOrBenefitInformation == elegibilityType
                    && bi.InsuranceTypeCode == insTypeCode)
                {
                    list.Add(bi);
                }
            }

            return list;
        }

        public virtual void SetBenefitsResponse(BenefitsValidationResponse response)
        {
            TheBenefitsValidationResponse = response;
        }

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Deserializes the benefit response.
        /// </summary>
        /// <returns></returns>
        private void DeserializeBenefitResponse()
        {

            XmlSerializer anXmlSerializer = null;
            XmlReaderSettings anXmlReaderSettings = null;
            StringReader aStringReader = null;

            try
            {

                anXmlSerializer =
                    new XmlSerializer(typeof(BenefitResponse));
                anXmlReaderSettings =
                    new XmlReaderSettings();
                aStringReader =
                    new StringReader(TheBenefitsValidationResponse.PayorXmlMessage);

                anXmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
                anXmlReaderSettings.IgnoreWhitespace = true;
                anXmlReaderSettings.IgnoreComments = true;

                this.TheBenefitResponse =
                    anXmlSerializer.Deserialize(aStringReader) as BenefitResponse;

            }//try
            finally
            {

                if (null != aStringReader) aStringReader.Close();
                if (null != aStringReader) aStringReader.Dispose();

            }//finally

        }//method


        /// <summary>
        /// Doeses the benifits category exist.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="network">The network.</param>
        /// <returns></returns>
        protected bool DoesBenifitsCategoryExist(string code, string network)
        {
            foreach (BenefitsInformation bi in this.Benefits)
            {
                if (bi.YesNoConditionOrResponseCode2 == network
                    && bi.ServiceTypeCode == code)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Doeses the node exist.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private bool DoesNodeExist(string path)
        {
            bool result = false;
            XmlNodeList nodeList = XmlResponseDocument.SelectNodes(path);
            if (nodeList.Count > 0)
            {
                result = true;
            }
            return result;
        }


        /// <summary>
        /// Doeses the response have XML.
        /// </summary>
        /// <returns></returns>
        protected bool DoesResponseHaveXml()
        {
            bool result = false;

            if (TheBenefitsValidationResponse != null &&
                TheBenefitsValidationResponse.PayorXmlMessage != null)
            {
                result = ( TheBenefitsValidationResponse.PayorXmlMessage.Trim().Length > 0 );
            }
            return result;
        }


        /// <summary>
        /// Formats the date.
        /// </summary>
        /// <param name="unformattedDate">The unformatted date.</param>
        /// <returns></returns>
        protected string FormatDate(string unformattedDate)
        {
            string formattedDate = string.Empty;

            if (unformattedDate != null
                || unformattedDate != string.Empty)
            {
                if (unformattedDate.Length < 8)
                {
                    formattedDate = DateTime.MinValue.ToString();
                }
                else if (unformattedDate.Length == 10)
                {
                    formattedDate = unformattedDate;
                }
                else if (unformattedDate.Length == 8)
                {
                    string year = string.Empty;
                    string month = string.Empty;
                    string day = string.Empty;

                    year = unformattedDate.Substring(0, 4);

                    if (int.Parse(year) > 1900 && int.Parse(year) < 2999)
                    {
                        month = unformattedDate.Substring(4, 2);

                        if (int.Parse(month) > 0 && int.Parse(month) < 13)
                        {
                            day = unformattedDate.Substring(6, 2);
                        }
                        else
                        {
                            month = unformattedDate.Substring(6, 2);
                            day = unformattedDate.Substring(4, 2);
                        }

                        if (int.Parse(month) > 0 && int.Parse(month) < 13)
                        {
                            formattedDate = month + "/" + day + "/" + year;
                        }
                    }
                    else
                    {
                        year = unformattedDate.Substring(4, 4);

                        if (int.Parse(year) > 1900 && int.Parse(year) < 2999)
                        {
                            month = unformattedDate.Substring(4, 2);

                            if (int.Parse(month) > 0 && int.Parse(month) < 13)
                            {
                                day = unformattedDate.Substring(6, 2);
                            }
                            else
                            {
                                month = unformattedDate.Substring(6, 2);
                                day = unformattedDate.Substring(4, 2);
                            }

                            if (int.Parse(month) > 0 && int.Parse(month) < 13)
                            {
                                formattedDate = month + "/" + day + "/" + year;
                            }
                        }
                        else
                        {
                            formattedDate = DateTime.MinValue.ToString();
                        }
                    }
                }
            }
            else
            {
                formattedDate = DateTime.MinValue.ToString();
            }

            return formattedDate;
        }


        /// <summary>
        /// Gets the deductible met.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <returns></returns>
        protected YesNoFlag GetDeductibleMet(BenefitsCategoryDetails details)
        {
            YesNoFlag result = new YesNoFlag();
            if (details.Deductible <= 0 && details.DeductibleDollarsMet <= 0)
            {
                result.SetBlank();
            }
            else if (details.Deductible > details.DeductibleDollarsMet)
            {
                result.SetNo();
            }
            else
            {
                result.SetYes();
            }
            return result;
        }


        /// <summary>
        /// Gets the out of pocket met.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <returns></returns>
        protected YesNoFlag GetOutOfPocketMet(BenefitsCategoryDetails details)
        {
            YesNoFlag result = new YesNoFlag();
            if (details.OutOfPocket <= 0 && details.OutOfPocketDollarsMet <= 0)
            {
                result.SetBlank();
            }
            else if (details.OutOfPocket > details.OutOfPocketDollarsMet)
            {
                result.SetNo();
            }
            else
            {
                result.SetYes();
            }
            return result;
        }

        /// <summary>
        /// Handles the schema validation event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.Xml.Schema.ValidationEventArgs"/> instance containing the event data.</param>
        private static void HandleSchemaValidationEvent(object sender, ValidationEventArgs eventArgs)
        {

            Logger.Fatal("XSD for benefits response is not valid", eventArgs.Exception);
            throw eventArgs.Exception;

        }//method


        /// <summary>
        /// Handles the XML validation event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.Xml.Schema.ValidationEventArgs"/> instance containing the event data.</param>
        private void HandleXmlValidationEvent(object sender, ValidationEventArgs eventArgs)
        {

            Logger.Warn("Validation of benefits response failed to parse", eventArgs.Exception);
            this.IsXmlResponseValid = false;

        }//method


        /// <summary>
        /// Loads the XML response document.
        /// </summary>
        private void LoadXmlResponseDocument()
        {

            try
            {

                this.XmlResponseDocument.Schemas = TheXmlSchemaSet;
                this.XmlResponseDocument
                    .LoadXml(TheBenefitsValidationResponse.PayorXmlMessage);
                this.XmlResponseDocument
                    .Validate(HandleXmlValidationEvent);

            }//try
            catch (Exception theException)
            {

                Logger.Warn("Validation of benefits response failed to parse", theException);
                this.IsXmlResponseValid = false;

            }//catch

        }//method


        /// <summary>
        /// Nodes the has inner text.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected bool NodeHasInnerText(XmlNode node)
        {

            return (node != null && node.InnerText != null);

        }


        /// <summary>
        /// Requires the authorization.
        /// </summary>
        /// <returns></returns>
        protected bool RequireAuthorization()
        {

            //TODO: Implement this. The xpath do not seem to be correct
            //talk to Scott T.
            return true;

        }


        /// <summary>
        /// Results the has in network.
        /// </summary>
        /// <returns></returns>
        protected bool ResultHasInNetwork()
        {
            return DoesNodeExist("//BenefitInformation[YesNoConditionOrResponseCode2='Y']");
        }


        /// <summary>
        /// Results the has out of network.
        /// </summary>
        /// <returns></returns>
        protected bool ResultHasOutOfNetwork()
        {

            return DoesNodeExist("//BenefitInformation[YesNoConditionOrResponseCode2='N']");
        }

        #endregion

        #region Constants

        private const string AUTHORIZATION_CO_NAME_QUALIFIER = "TE";
        private const string AUTHORIZATION_CO_NAME = "1C";
        private const string AUTHORIZATION_CO_PHONE = "1C";
        protected const string BEN_CATAGORY_CO_INSURANCE = "/BenefitResponse/InformationSourceLevel/InformationReceiverLevel/SubscriberLevel/SubscriberName/SubscriberBenefitInformation/BenefitInformation[EligibilityOrBenefitInformation='A' and TimePeriodQualifier='22' {0}]/Percent";
        private const string BENEFITS_XML_SCHEMA_PATH = "PatientAccess.Resources.BenefitResponseSchema.xsd";
        private const string COMM_NUMBER_QUALIFIER_TE = "TE";
        private const string COMMERCIAL_COVERAGE_NAME = "PatientAccess.Domain.CommercialCoverage";
        private const string CONTACT_FUNCTION_CODE_IC = "IC";
        private const string DATE_TIME_QUALIFIER_POLICY_DATE_RANGE = "307,291";
        private const string DATE_TIME_QUALIFIER_BENEFIT_EFFECTIVE = "348";
        private const string DATE_TIME_QUALIFIER_BENEFIT_EXPIRATION = "349";
        private const string DATE_TIME_QUALIFIER_POLICY_EFFECTIVE = "356";
        private const string DATE_TIME_QUALIFIER_POLICY_EXPIRATION = "357";
        private const string GOVERNMENT_OTHER_NAME = "PatientAccess.Domain.GovernmentOtherCoverage";
        private const string GROUP_NUMBER = "6P";
        protected const string IN_NETWORK = "Y";
        protected const string OUT_OF_NETWORK = "N";
        protected const string PAYOR_NAME = "/BenefitResponse/InformationSourceLevel/InformationSourceName/Name[EntityIdentifierCode1='PR' and EntityTypeQualifier='1' or EntityTypeQualifier='2']/NameLastOrOrganizationName";
        private const string SERVICE_TYPE_CODE_GENERAL = "30";
        private const string SUBSCRIBER_ID = "MI";
        private const string WORKERS_COMP_NAME = "PatientAccess.Domain.WorkersCompensationCoverage";

        #endregion

        #region Fields

        private static readonly ILog c_Logger = LogManager.GetLogger( typeof( ParseStrategy ) );
        private BenefitResponse i_BenefitResponse = new BenefitResponse();
        private ArrayList i_Benefits = new ArrayList();
        private BenefitsValidationResponse i_BenefitsValidationResponse = null;
        private bool i_IsGovernmentOther = false;
        private bool i_IsXmlResponseValid = true;
        private readonly XmlDocument i_XmlResponse = new XmlDocument();
        private static readonly XmlSchemaSet i_XmlSchemaSet = new XmlSchemaSet();

        #endregion

    }//class

}//namespace