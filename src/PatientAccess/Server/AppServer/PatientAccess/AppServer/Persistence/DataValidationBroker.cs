using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Xsl;
using Extensions.Persistence;
using PatientAccess.AddressValidationProxy;
using PatientAccess.BenefitsValidationFusProxy;
using PatientAccess.BenefitsValidation5010Proxy;
using PatientAccess.BrokerInterfaces;
using PatientAccess.ComplianceCheckerProxy;
using PatientAccess.CreditValidationProxy;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.PriorAccountBalanceProxy;
using PatientAccess.Services;
using PatientAccess.Utilities;
using log4net;
using Account = PatientAccess.Domain.Account;
using Address = PatientAccess.Domain.Address;
using AddressValidationResult = PatientAccess.BrokerInterfaces.AddressValidationResult;
using Patient = PatientAccess.BenefitsValidation5010Proxy.patient;
using ServiceKey = PatientAccess.CreditValidationProxy.ServiceKey;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for DataValidationBroker.
    /// </summary>
    /// 	
    [Serializable]
    public class DataValidationBroker : AbstractBroker, IDataValidationBroker
    {
        #region Constants

        private const string RELATIONSHIP_CODE_SELF = "01";

        private const string RELATIONSHIP_CODE_SPOUSE = "02";

        private const string RELATIONSHIP_CODE_NATURAL_CHILD = "03";

        private const string RELATIONSHIP_CODE_STEPCHILD = "05";

        private const string RELATIONSHIP_CODE_FOSTER_CHILD = "06";

        private const string RELATIONSHIP_CODE_PARENT = "18";

        private const string RELATIONSHIP_CODE_GRANDPARENT = "19";

        private const string RELATIONSHIP_CODE_NEIGHBOR = "34";

        private const string DBCOLUMN_ACCOUNT_NUMBER = "AccountNumber";

        private const string DBCOLUMN_AVALIABLE = "ResultsAreAvailable";

        private const string DBCOLUMN_PARSE_STRATEGY = "ResponseParseStrategy";

        private const string DBCOLUMN_COVERAGE_ORDER_OID = "CoverageOrderId";

        private const string DBCOLUMN_PLAN_CATEGORY_OID = "PlanCategoryId";

        private const string DBCOLUMN_INSURANCE_PLAN_CODE = "InsurancePlanCode";

        private const string DBCOLUMN_RESPONSE_GROUP_NUMBER = "ResponseGroupNumber";

        private const string DBCOLUMN_RESPONSE_INSURED_DOB = "ResponseInsuredDob";

        private const string DBCOLUMN_RESPONSE_INSURED_FIRST_NAME = "ResponseInsuredFirstName";

        private const string DBCOLUMN_RESPONSE_INSURED_MIDDLE_INIT = "ResponseInsuredMiddleInit";

        private const string DBCOLUMN_RESPONSE_INSURED_LAST_NAME = "ResponseInsuredLastName";

        private const string DBCOLUMN_RESPONSE_PAYOR_NAME = "ResponsePayor";

        private const string DBCOLUMN_RESPONSE_SUBSCRIBER_ID = "ResponseSubscriberId";

        private const string DBCOLUMN_RESPONSE_STATUS = "ResponseStatus";

        private const string DBCOLUMN_REQUEST_INSURED_DOB = "RequestInsuredDob";

        private const string DBCOLUMN_REQUEST_INSURED_FIRST_NAME = "RequestInsuredFirstName";

        private const string DBCOLUMN_REQUEST_INSURED_MIDDLE_INIT = "RequestInsuredMiddleInit";

        private const string DBCOLUMN_REQUEST_INSURED_LAST_NAME = "RequestInsuredLastName";

        private const string DBCOLUMN_REQUEST_PAYOR_NAME = "RequestPayor";

        private const string DBCOLUMN_RESPONSE_AUTH_CO = "ResponseAuthCo";

        private const string DBCOLUMN_RESPONSE_AUTH_CO_PHONE = "ResponseAuthCoPhone";

        private const string DBCOLUMN_REQUEST_SUBSCRIBER_ID = "RequestSubscriberId";

        private const string DBCOLUMN_FACILITYID = "FacilityId";

        private const string DBCOLUMN_FUSNOTESENT = "FusNoteSent";

        private const string DBCOLUMN_INITIATEDONDATE = "InitiatedOnDate";

        private const string DBCOLUMN_MEDICALRECORDNUMBER = "MedicalRecordNumber";

        private const string DBCOLUMN_TICKETID = "TicketId";

        private const string DBCOLUMN_VALIDATION_TICKETTYPE = "TicketTypeId";

        private const string DBCOLUMN_VIEWED = "Viewed";

        private const string DV_INVALID_RESPONSE = "Unable to process credit response returned by the partner.";

        private const string EDV_UNAVAILABLE_ERROR = "Unable to read data from the transport connection";

        private const int LENGTH_OF_ACCOUNT_NUMBER = 9;

        private const int LENGTH_OF_MRN = 9;
        private const string CONFIG_BENEFITSVALIDATION5010SERVICE_ENTRY = "BenefitsValidation5010ServiceSEIPort";

        private const string DBPARAMETER_ACCOUNTNUM = "@AccountNumber";

        private const string DBPARAMETER_AVAILABILITY = "@IsAvailable";

        private const string DBPARAMETER_PARSE_STRATEGY = "@ParseStrategy";

        private const string DBPARAMETER_COVERAGE_ORDER_OID = "@CoverageOrderOid";

        private const string DBPARAMETER_PLAN_CATEGORY_OID = "@PlanCategoryOid";

        private const string DBPARAMETER_INSURANCE_PLAN_CODE = "@InsurancePlanCode";

        private const string DBPARAMETER_RESPONSE_GROUP_NUMBER = "@ResponseGroupNumber";

        private const string DBPARAMETER_RESPONSE_INSURED_DOB = "@ResponseInsuredDob";

        private const string DBPARAMETER_RESPONSE_INSURED_FIRST_NAME = "@ResponseInsuredFirstName";

        private const string DBPARAMETER_RESPONSE_INSURED_MIDDLE_INIT = "@ResponseInsuredMiddleInit";

        private const string DBPARAMETER_RESPONSE_INSURED_LAST_NAME = "@ResponseInsuredLastName";

        private const string DBPARAMETER_RESPONSE_PAYOR_NAME = "@ResponsePayorName";

        private const string DBPARAMETER_RESPONSE_SUBSCRIBER_ID = "@ResponseSubscriberId";

        private const string DBPARAMETER_REQUEST_INSURED_DOB = "@RequestInsuredDob";

        private const string DBPARAMETER_REQUEST_INSURED_FIRST_NAME = "@RequestInsuredFirstName";

        private const string DBPARAMETER_REQUEST_INSURED_MIDDLE_INIT = "@RequestInsuredMiddleInit";

        private const string DBPARAMETER_REQUEST_INSURED_LAST_NAME = "@RequestInsuredLastName";

        private const string DBPARAMETER_REQUEST_PAYOR_NAME = "@RequestPayorName";

        private const string DBPARAMETER_REQUEST_SUBSCRIBER_ID = "@RequestSubscriberID";

        private const string DBPARAMETER_RESPONSE_AUTH_CO = "@ResponseAuthCo";

        private const string DBPARAMETER_RESPONSE_AUTH_CO_PHONE = "@ResponseAuthCoPhone";

        private const string DBPARAMETER_RESPONSE_STATUS = "@ResponseStatus";

        private const string DBPARAMETER_SUCCESSFUL_UPDATE = "@SuccessfulUpdate";

        private const string DBPARAMETER_FACILITY = "@FacilityId";

        private const string DBPARAMETER_FUSNOTESENT = "@IsFusNoteSent";

        private const string DBPARAMETER_INITIATED_ON = "@InitiatedOn";

        private const string DBPARAMETER_ISVIEWED = "@IsViewed";

        private const string DBPARAMETER_MEDRECORDNUM = "@MedicalRecordNumber";

        private const string DBPARAMETER_TICKET_ID = "@TicketId";

        private const string DBPARAMETER_TICKET_TYPE_ID = "@TicketTypeId";

        private const string DBPROCEDURE_GET_DATA_VALIDATION_TICKET = "DataValidation.GetDataValidationTicket";

        private const string DBPROCEDURE_GET_DATA_VALIDATION_TICKET_BY_ID = "DataValidation.GetDataValidationTicketById";

        private const string DBPROCEDURE_INSERT_NEW_TICKET = "DataValidation.InsertTicket";

        private const string DBPROCEDURE_UPDATE_TICKET = "DataValidation.UpdateDataValidationTicket";

        private const string DBPROCEDURE_UPDATE_TICKET_AVAILABLE = "DataValidation.UpdateAvailability";

        private const int TENET_CUSTOMER_ID = 1;

        #endregion Constants

        #region Fields

        private readonly IAddressBroker _addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
        private static readonly ILog _logger = LogManager.GetLogger( typeof( DataValidationBroker ) );
        private readonly IFacilityBroker _facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        private readonly IFinancialClassesBroker _financialClassesBroker = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();
        private readonly IInsuranceBroker _insuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
        private readonly IPatientBroker _patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();

        #endregion Fields

        #region Constructors

        public DataValidationBroker( string cxnString )
            : base( cxnString )
        {

            Initialize();

        }

        public DataValidationBroker( SqlTransaction transaction )
            : base( transaction )
        {
            Initialize();
        }

        public DataValidationBroker()
            : base()
        {

            Initialize();

        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the address validation service.
        /// </summary>
        /// <value>The address validation service.</value>
        protected virtual IAddressValidationService AddressValidationService { get; set; }


        /// <summary>
        /// Gets or sets the benefits validation fus service.
        /// </summary>
        /// <value>The benefits validation fus service.</value>
        protected virtual IBenefitsValidationFusService BenefitsValidationFusService { get; set; }


        /// <summary>
        /// Gets or sets the benefits validation service.
        /// </summary>
        /// <value>The benefits validation service.</value>
        internal protected virtual IBenefitsValidation5010ServiceSoapClient BenefitsValidation5010Service { get; set; }

        /// <summary>
        /// Gets or sets the compliance checker service.
        /// </summary>
        /// <value>The compliance checker service.</value>
        protected virtual IComplianceCheckerService ComplianceCheckerService { get; set; }


        /// <summary>
        /// Gets or sets the credit validation service.
        /// </summary>
        /// <value>The credit validation service.</value>
        protected virtual ICreditValidationService CreditValidationService { get; set; }


        /// <summary>
        /// Gets or sets the prior account balance service.
        /// </summary>
        /// <value>The prior account balance service.</value>
        protected virtual IPriorAccountBalanceService PriorAccountBalanceService { get; set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static ILog Logger
        {
            get
            {
                return _logger;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ares the benefits validation results available for.
        /// </summary>
        /// <param name="ticketId">The ticket id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="hospitalCode">The hospital code.</param>
        /// <returns></returns>
        public bool AreBenefitsValidationResultsAvailableFor( string ticketId, string userId, string hospitalCode )
        {
            var resultRequest5010 = new obtainResultRequest();
            resultRequest5010.ServiceKey_1 = CreateServiceKeyFor(ticketId, userId, hospitalCode);

            bool areResultsAvailable = false;

            try
            {
               var resultResponse5010 = new BenefitsValidation5010ServiceSoapClient().obtainResult(resultRequest5010.ServiceKey_1);
                   
                // This is pretty fragile. EDV needs to return a code in the response that is
                // less likely to change
                areResultsAvailable = !resultResponse5010.payorMessage.Contains("No Result Is Available");
            }
            catch ( Exception anyException )
            {

                Logger.Error(
                    String.Format(
                        "Error while polling for benefits response for [ticket]: {0}, [user]: {1}, [hostpital]: {2}",
                        ticketId,
                        userId,
                        hospitalCode ),
                    anyException );

            }

            return areResultsAvailable;

        }


        /// <summary>
        /// Gets the benefits validation response.
        /// </summary>
        /// <param name="ticketId">The ticket id.</param>
        /// <param name="upn">The upn.</param>
        /// <param name="currentCoverageType">Type of the current coverage.</param>
        /// <returns></returns>
        public BenefitsValidationResponse GetBenefitsValidationResponse( string ticketId, string upn, Type currentCoverageType )
        {
            DataValidationTicket latestTicket = GetDataValidationTicketFor( ticketId );
            BenefitsValidationResponse benefitsValidationResponse = null;

            obtainResultRequest result5010 = new obtainResultRequest();
            
            if ( latestTicket != null && latestTicket.ResultsAvailable )
            {
                try
                {
                    result5010.ServiceKey_1 = CreateServiceKeyFor(ticketId, upn, latestTicket.Facility.Code);
                    var resultResponse5010 = new BenefitsValidation5010ServiceSoapClient().obtainResult(result5010.ServiceKey_1);
                  
                    // InvalidCastException in get_Constraints of GovernmentMedicaidCoverage
                    if ( latestTicket.BenefitsResponse.BenefitsResponseParseStrategy == currentCoverageType.ToString() )
                    {
                        // if the account's current coverage type is the same as
                        // when the benefits validation was previously performed
                       latestTicket.ResponseText = resultResponse5010.payorMessage;
                    }
                    else
                    {
                        // Fix - If the account's current coverage type does not match the coverage type on the latest
                        // ticket in the database do not get the old response. The following parameters on the resultResponse 
                        // and the latest ticket should be modified so that it will not parse any available constraints 
                        // and force the user to re-initiate benefits validation for the new coverage.

                        resultResponse5010.success = false;
                        resultResponse5010.eligible = string.Empty;
                        resultResponse5010.messageUUID = string.Empty;
                        resultResponse5010.payorMessage = BenefitsValidationResponse.RESPONSE_TEXT_ON_MISMATCH;
                        resultResponse5010.payorXmlMessage = string.Empty;

                        Facility facility = latestTicket.Facility;
                        latestTicket = new DataValidationTicket();
                        latestTicket.Facility = facility;
                        latestTicket.ResponseText = BenefitsValidationResponse.RESPONSE_TEXT_ON_MISMATCH;
                        latestTicket.ResultsAvailable = true;
                    }

                    benefitsValidationResponse = new BenefitsValidationResponse( latestTicket.Facility.Code,
                                                      latestTicket,
                                                      resultResponse5010.eligible,
                                                      resultResponse5010.messageUUID,
                                                      resultResponse5010.payorMessage,
                                                      resultResponse5010.payorXmlMessage,
                                                      resultResponse5010.success );

                    ParseCoverageContext parseContext = new ParseCoverageContext();

                    if ( HasParseStrategyDefinedFor( benefitsValidationResponse ) )
                    {
                        parseContext.ParseAndSetCoverageConstraints( benefitsValidationResponse );
                    }
                    else
                    {
                        Logger.WarnFormat( "Unable to get parse strategy for benefits validation response ( ticket id: {0} )", ticketId );
                    }
                }
                catch (SoapException soapException)
                {
                    Logger.ErrorFormat("DataValidation - Failed to retrieve response for TicketID: {0}, User: {1} Mesage:{2}", ticketId, upn, soapException.Message);
                    return benefitsValidationResponse;
                }
            }

            return benefitsValidationResponse;
        }


        /// <summary>
        /// Gets the credit validation response.
        /// </summary>
        /// <param name="ticketId">The ticket id.</param>
        /// <param name="upn">The upn.</param>
        /// <param name="hspCode">The HSP code.</param>
        /// <returns></returns>
        /// <exception cref="Exception">DataValidationBroker failed to call credit web service:</exception>
        public CreditValidationResponse GetCreditValidationResponse( string ticketId, string upn, string hspCode )
        {

            CreditValidationResponse creditResult = new CreditValidationResponse();

            try
            {
                DataValidationTicket latestTicket = GetDataValidationTicketFor( ticketId );
                obtainCreditResultResponse response;
                obtainCreditResult result = new obtainCreditResult();

                result.ServiceKey_1 = new ServiceKey();
                result.ServiceKey_1.customerId = TENET_CUSTOMER_ID;
                result.ServiceKey_1.hspcd = hspCode;
                result.ServiceKey_1.userId = upn;
                result.ServiceKey_1.uuid = latestTicket.TicketId;

                creditResult.ReturnedDataValidationTicket = latestTicket;

                if ( latestTicket.ResultsAvailable )
                {
                    response = CreditValidationService.obtainCreditResult( result );

                    if ( response != null
                        && response.result != null
                        && response.result != DV_INVALID_RESPONSE
                        )
                    {
                        creditResult.ResponseCreditReport =
                            BuildCreditReport( response.result, _facilityBroker.FacilityWith( hspCode ).Oid );
                    }
                }
            }
            catch ( SoapException soapException )
            {
                Logger.ErrorFormat(
                    "Error Getting Credit Validation: {0} {1}",
                    soapException.ToString(),
                    soapException.Detail.InnerText );

                return new CreditValidationResponse();
            }
            catch ( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    "DataValidationBroker failed to call credit web service:",
                    anyException,
                    Logger );
            }

            return creditResult;
        }


        /// <summary>
        /// Gets the data validation ticket for.
        /// </summary>
        /// <param name="aTicketId">A ticket id.</param>
        /// <returns></returns>
        /// <exception cref="Exception">DataValidation Broker failed to initialize</exception>
        public virtual DataValidationTicket GetDataValidationTicketFor( string aTicketId )
        {

            SafeReader reader = null;
            SqlCommand sqlCommand = null;
            DataValidationTicket aTicket = new DataValidationTicket();

            try
            {
                sqlCommand = CommandFor( DBPROCEDURE_GET_DATA_VALIDATION_TICKET_BY_ID );

                SqlParameter ticketIdParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_TICKET_ID, SqlDbType.VarChar ) );
                ticketIdParam.Value = aTicketId;

                reader = ExecuteReader( sqlCommand );

                if ( reader.Read() )
                {
                    aTicket = GetTicketFromReader( reader );
                }
            }
            catch ( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    "DataValidation Broker failed to initialize",
                    anyException,
                    Logger );
            }
            finally
            {
                Close( reader );
                Close( sqlCommand );
            }

            return aTicket;

        }


        /// <exception cref="Exception">DataValidation Broker failed to get Ticket for coverage</exception>
        public DataValidationTicket GetDataValidationTicketFor( Account anAccount, DataValidationTicketType type )
        {
            DataValidationTicket aTicket = null;
            SafeReader reader = null;
            SqlCommand sqlCommand = null;

            try
            {
                sqlCommand = CommandFor( DBPROCEDURE_GET_DATA_VALIDATION_TICKET );

                SqlParameter accountnumParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_ACCOUNTNUM, SqlDbType.Int ) );
                accountnumParam.Value = anAccount.AccountNumber;

                SqlParameter facilityIdParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_FACILITY, SqlDbType.Int ) );
                facilityIdParam.Value = anAccount.Facility.Oid;

                SqlParameter medicalRecordIdParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_MEDRECORDNUM, SqlDbType.Int ) );
                medicalRecordIdParam.Value = anAccount.Patient.MedicalRecordNumber;

                SqlParameter ticketTypeIdParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_TICKET_TYPE_ID, SqlDbType.Int ) );
                ticketTypeIdParam.Value = type.Oid;

                reader = ExecuteReader( sqlCommand );

                if ( reader.Read() )
                {
                    aTicket = GetTicketFromReader( reader );
                }
            }
            catch ( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    "DataValidation Broker failed to get Ticket for coverage",
                    anyException,
                    Logger );
            }
            finally
            {
                base.Close( reader );
                base.Close( sqlCommand );
            }

            return aTicket;

        }


        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public ArrayList GetPriorBalanceAccounts( PriorAccountsRequest request )
        {

            ArrayList accounts = new ArrayList();

            if ( request.IsAccountNull
                || request.IsPatientNull
                || request.MedicalRecordNumber <= 0 )
            {
                return accounts;
            }

            //PriorAccountBalanceRequest serviceRequest = new PriorAccountBalanceRequest();
            //identifyPriorAccountBalancesResponse response;
            //PriorAccountBalanceResult result;
            //identifyPriorAccountBalances priorBalances = new identifyPriorAccountBalances();

            priorAccountBalanceRequest serviceRequest = new priorAccountBalanceRequest();      
         
            serviceRequest.customerId = 1;
            serviceRequest.hspcd = request.Facility.Code;
            serviceRequest.userId = request.Upn;

            string paddedMrn = request.MedicalRecordNumber.ToString().PadLeft( 9, '0' );
            paddedMrn = paddedMrn.PadRight( 12, ' ' );
            serviceRequest.medicalRecordNumber = paddedMrn;
            // identifyPriorAccountBalances priorBalances = new identifyPriorAccountBalances();
            //priorBalances.PriorAccountBalanceRequest_1 = serviceRequest;
            priorAccountBalanceResult result;
            try
            {             
                //response =
                //    PriorAccountBalanceService.identifyPriorAccountBalances( priorBalances );
                //result = response.result;
          
                using (var client = new PriorAccountBalanceServiceSoapClient())
                {
                    result = client.identifyPriorAccountBalances(serviceRequest);
                }
            }
            catch ( SoapException se )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Error retrieving SOS Prior Accounts " +
                    se.Detail.InnerText + se, se, Logger );
            }

            if ( result.status )
            {
                CultureInfo en = new CultureInfo( "en-US" );
                const string format = "yyyyMMdd";
                foreach (var a in result.accounts )
                {
                    AccountProxy aPrevAccount = new AccountProxy();

                    DateTime dischargeDate = DateTime.ParseExact( a.dischargeDate, format, en.DateTimeFormat );
                    long accountNumber = Convert.ToInt64( a.patientAccountNumber );
                    string financialClassCode = a.financialClass.code;
                    string patientTypeCode = a.patientType.code;
                    decimal balDue = (decimal)a.totalBalance;

                    VisitType kindOfVisit = _patientBroker.PatientTypeWith( request.Facility.Oid, patientTypeCode );

                    Domain.FinancialClass financialClass = _financialClassesBroker.FinancialClassWith( request.Facility.Oid,
                        financialClassCode );

                    aPrevAccount.DischargeDate = dischargeDate;
                    aPrevAccount.AccountNumber = accountNumber;
                    aPrevAccount.FinancialClass = financialClass;
                    aPrevAccount.KindOfVisit = kindOfVisit;
                    aPrevAccount.BalanceDue = balDue;
                    aPrevAccount.HasPaymentPlan = a.paymentPlan;

                    aPrevAccount.Facility = (Facility)request.Facility.Clone();

                    accounts.Add( aPrevAccount );
                }
            }

            return accounts;
        }


        /// <exception cref="Exception">Invalid Benefits Validation Request;  Coverage, Account, or User not specified.</exception>
        public DataValidationTicket InitiateBenefitsValidation( AccountDetailsRequest benefitsRequest )
        {
            initiateRequest initiateRequest = new initiateRequest();

            //initiateResponse initiateResponse;

            benefitsValidation5010Request request = new benefitsValidation5010Request();

            DataValidationTicket aTicket;

            if ( benefitsRequest.IsCoverageNull ||
                benefitsRequest.IsAccountNull ||
                benefitsRequest.IsUserNull )
            {
                throw new Exception( "Invalid Benefits Validation Request;  Coverage, Account, or User not specified." );
            }

            try
            {

                Facility facility = _facilityBroker.FacilityWith( benefitsRequest.FacilityOid );

                request.customerId = TENET_CUSTOMER_ID;

                // The override is present because our generic test accounts do not have access to
                // Passport. If these values are present in the web.config, then they are used instead
                // of the current user and facility.
                if ( String.IsNullOrEmpty( ConfigurationManager.AppSettings["EdvFacilityOverride"] ) )
                {
                    request.hspcd = facility.Code;
                }
                else
                {
                    request.hspcd = ConfigurationManager.AppSettings["EdvFacilityOverride"];
                }

                if ( String.IsNullOrEmpty( ConfigurationManager.AppSettings["EdvUserOverride"] ) )
                {
                    request.userId = benefitsRequest.Upn;
                }
                else
                {
                    request.userId = ConfigurationManager.AppSettings["EdvUserOverride"];

                }

                request.patient = new patient();
                request.subscriber = new subscriber();

                request.subscriber.cardIssueDate = string.Empty;
                request.subscriber.zip = string.Empty;

                // what shall we do?

                RelationshipType aRel = benefitsRequest.PatientInsuredRelationship;

                if ( aRel == null )
                {
                    request.patient.relationshipToSubscriber = RELATIONSHIP_CODE_NEIGHBOR;
                }
                else
                {
                    if ( aRel.Code == RELATIONSHIP_CODE_SELF )
                    {
                        request.patient.relationshipToSubscriber = RELATIONSHIP_CODE_PARENT;
                    }
                    else if ( aRel.Code == RELATIONSHIP_CODE_SPOUSE )
                    {
                        request.patient.relationshipToSubscriber = RELATIONSHIP_CODE_SELF;
                    }
                    else if ( aRel.Code == RELATIONSHIP_CODE_NATURAL_CHILD ||
                             aRel.Code == RELATIONSHIP_CODE_STEPCHILD ||
                             aRel.Code == RELATIONSHIP_CODE_FOSTER_CHILD )
                    {
                        request.patient.relationshipToSubscriber = RELATIONSHIP_CODE_GRANDPARENT;
                    }
                    else
                    {
                        request.patient.relationshipToSubscriber = RELATIONSHIP_CODE_NEIGHBOR;
                    }
                }

                // patient
                if ( benefitsRequest.PatientDOB == DateTime.MinValue )
                {
                    request.patient.dateOfBirth = string.Empty;
                }
                else
                {
                    request.patient.dateOfBirth = benefitsRequest.PatientDOB.ToString( "yyyyMMdd" );
                }

                request.patient.firstName = benefitsRequest.PatientFirstName;
                request.patient.lastName = benefitsRequest.PatientLastName;
                request.patient.middleName = benefitsRequest.PatientMidInitial;
                request.patient.sex = benefitsRequest.PatientSex;
                request.patient.ssn = benefitsRequest.PatientSSN.UnformattedSocialSecurityNumber;

                // insured
                if ( benefitsRequest.InsuredDOB == DateTime.MinValue )
                {
                    request.subscriber.dateOfBirth = string.Empty;
                }
                else
                {
                    request.subscriber.dateOfBirth = benefitsRequest.InsuredDOB.ToString( "yyyyMMdd" );
                }
                request.subscriber.firstName = benefitsRequest.InsuredFirstName;
                request.subscriber.groupId = benefitsRequest.InsuredGroupNumber;
                request.subscriber.lastName = benefitsRequest.InsuredLastName;
                request.subscriber.memberId = String.Empty;

                if ( benefitsRequest.CoverageMemberId != String.Empty )
                {
                    request.subscriber.memberId = benefitsRequest.CoverageMemberId;
                }

                request.subscriber.middleName = string.Empty;
                request.subscriber.planId = benefitsRequest.CoverageInsurancePlanId;
                request.subscriber.sex = benefitsRequest.InsuredSex;

                request.subscriber.ssn = string.Empty;

                ContactPoint cp = benefitsRequest.InsuredPhysicalCP;
                Address addr = null;

                if ( cp != null )
                {
                    addr = cp.Address;
                }

                if ( ( null != addr ) && ( null != addr.State ) )
                {
                    request.subscriber.state = addr.State.Code ?? string.Empty;
                }
                else
                {
                    request.subscriber.state = string.Empty;
                }

                request.beginServiceDate = benefitsRequest.AdmitDate.ToString( "yyyyMMdd" );
                request.endServiceDate = request.beginServiceDate;

                aTicket = new DataValidationTicket(
                    benefitsRequest.AccountNumber, benefitsRequest.MedicalRecordNumber, facility );
                aTicket.ResultsAvailable = false;
                aTicket.Facility = facility;
                initiateRequest.BenefitsValidation5010Request_1 = request;
                string responseTicketId =  new BenefitsValidation5010ServiceSoapClient().initiate(initiateRequest.BenefitsValidation5010Request_1);

                aTicket = new DataValidationTicket( responseTicketId );
                aTicket.ResultsAvailable = false;
                aTicket.Facility = facility;

                aTicket.InitiatedOn = GetCurrentFacilityDateTime( aTicket.Facility.GMTOffset, aTicket.Facility.DSTOffset );

                if ( benefitsRequest.CoverageOrderOid == CoverageOrder.PRIMARY_OID )
                {
                    aTicket.TicketType = DataValidationTicketType.GetNewPrimaryCoveragTicketType();
                }
                else
                {
                    aTicket.TicketType = DataValidationTicketType.GetNewSecondaryCoveragTicketType();
                }
                aTicket.AccountNumber = benefitsRequest.AccountNumber;
                aTicket.MedicalRecordNumber = benefitsRequest.MedicalRecordNumber;

                SaveNewTicket( aTicket );

                aTicket.BenefitsResponse.BenefitsResponseParseStrategy = benefitsRequest.TypeOfCoverage;
                // persist the 'match' values we sent on the request... these will later be compared to the values 
                // returned on the response

                aTicket.BenefitsResponse.RequestInsuredFirstName = request.subscriber.firstName;
                // set this to the value on the UI even though it is not passed to DV... this allows for a 'match' 
                // condition on the response.
                aTicket.BenefitsResponse.RequestInsuredMiddleInitial = benefitsRequest.InsuredMiddleInitial;
                aTicket.BenefitsResponse.RequestInsuredLastName = request.subscriber.lastName;

                aTicket.BenefitsResponse.RequestInsuredDOB = request.subscriber.dateOfBirth;
                aTicket.BenefitsResponse.RequestPayorName = benefitsRequest.RequestPayorName;
                aTicket.BenefitsResponse.RequestSubscriberID = request.subscriber.memberId;

                SaveDataValidationTicket( aTicket );
            }
            catch ( SoapException soapException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    soapException.Detail.InnerText,
                    soapException,
                    Logger );
            }
            catch ( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    "DataValidationBroker failed to call web service:",
                    anyException,
                    Logger );
            }

            return aTicket;
        }


        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public DataValidationTicket InitiateGuarantorValidation( Guarantor aGuarantor, string upn, string hspCode, long accountNumber, long mrn )
        {

            initiateValidateCredit request =
                new initiateValidateCredit();
            CreditValidationRequest cvr =
                new CreditValidationRequest();
            initiateValidateCreditResponse ivcr;

            cvr.customerId = TENET_CUSTOMER_ID;
            cvr.hspcd = hspCode;
            cvr.userId = upn;

            ContactPoint cp = aGuarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );

            cvr.lastName = aGuarantor.LastName;
            cvr.firstName = aGuarantor.FirstName;
            cvr.ssn = aGuarantor.SocialSecurityNumber.UnformattedSocialSecurityNumber;
            cvr.middleName = aGuarantor.Name.MiddleInitial;
            cvr.areaCode = cp.PhoneNumber.AreaCode;
            cvr.dateOfBirth = aGuarantor.DateOfBirth.ToString( "yyyyMMdd" );
            cvr.phone = cp.PhoneNumber.Number;
            cvr.prefix = string.Empty;
            cvr.suffix = string.Empty;

            cvr.address = new CreditValidationProxy.Address();

            cvr.address.street = cp.Address.Address1;
            cvr.address.city = cp.Address.City;
            cvr.address.state = cp.Address.State.Code;
            cvr.address.zip = cp.Address.ZipCode.ZipCodePrimary;
            cvr.address.zipExtension = cp.Address.ZipCode.ZipCodeExtended;

            request.CreditValidationRequest_1 = cvr;

            try
            {
                Facility facility = _facilityBroker.FacilityWith( hspCode );

                ivcr = CreditValidationService.initiateValidateCredit( request );

                string ticketId = ivcr.result;

                DataValidationTicket ticket = new DataValidationTicket(
                   accountNumber, mrn, facility );

                ticket.TicketId = ticketId;
                ticket.ResultsAvailable = false;
                ticket.InitiatedOn = GetCurrentFacilityDateTime( facility.GMTOffset, facility.DSTOffset );
                ticket.TicketType = DataValidationTicketType.GetNewGuarantorTicketType();
                ticket.Facility = facility;

                SaveNewTicket( ticket );
                SaveDataValidationTicket( ticket );

                return ticket;
            }
            catch ( SoapException se )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( se.Detail.InnerText, se, Logger );
            }
            catch ( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Logger );
            }
        }


        /// <exception cref="Exception">DataValidation Broker failed to update the availability</exception>
        public virtual void SaveDataValidationTicket( DataValidationTicket aTicket )
        {
            SqlCommand sqlCommand = null;

            try
            {
                sqlCommand = CommandFor( DBPROCEDURE_UPDATE_TICKET );

                SqlParameter ticketIdParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_TICKET_ID, SqlDbType.VarChar ) );
                ticketIdParam.Value = aTicket.TicketId.Trim();

                SqlParameter facilityIdParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_FACILITY, SqlDbType.Int ) );
                facilityIdParam.Value = aTicket.Facility.Oid;

                SqlParameter accountnumParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_ACCOUNTNUM, SqlDbType.Int ) );
                accountnumParam.Value = aTicket.AccountNumber;

                SqlParameter medicalRecordIdParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_MEDRECORDNUM, SqlDbType.Int ) );
                medicalRecordIdParam.Value = aTicket.MedicalRecordNumber;

                SqlParameter ticketAvailabilityParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_AVAILABILITY, SqlDbType.Char ) );
                ticketAvailabilityParam.Value = GetYesNoValue( aTicket.ResultsAvailable );

                SqlParameter isViewedParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_ISVIEWED, SqlDbType.Char ) );
                isViewedParam.Value = GetYesNoValue( aTicket.ResultsReviewed );

                SqlParameter fusNoteSentParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_FUSNOTESENT, SqlDbType.Char ) );
                fusNoteSentParam.Value = GetYesNoValue( aTicket.FUSNoteSent );

                SqlParameter ticketTypeParam = sqlCommand.Parameters.Add(
                     new SqlParameter( DBPARAMETER_TICKET_TYPE_ID, SqlDbType.Int ) );
                if ( aTicket.TicketType != null )
                {
                    ticketTypeParam.Value = aTicket.TicketType.Oid;
                }
                else
                {
                    ticketTypeParam.Value = 0;
                }

                // parms for the Data_Validation_Benefits_Response

                if ( aTicket.BenefitsResponse != null )
                {
                    SqlParameter parseStrategy = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_PARSE_STRATEGY, SqlDbType.VarChar ) );
                    parseStrategy.Value = aTicket.BenefitsResponse.BenefitsResponseParseStrategy;

                    SqlParameter coverageOrderOid = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_COVERAGE_ORDER_OID, SqlDbType.Int ) );
                    coverageOrderOid.Value = aTicket.BenefitsResponse.CoverageOrder.Oid;

                    SqlParameter planCategoryOid = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_PLAN_CATEGORY_OID, SqlDbType.Int ) );
                    planCategoryOid.Value = aTicket.BenefitsResponse.PlanCategory.Oid;

                    SqlParameter insurancePlanCode = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_INSURANCE_PLAN_CODE, SqlDbType.VarChar ) );
                    insurancePlanCode.Value = aTicket.BenefitsResponse.PlanCode;

                    SqlParameter responseGroupNumber = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_GROUP_NUMBER, SqlDbType.VarChar ) );
                    responseGroupNumber.Value = aTicket.BenefitsResponse.ResponseGroupNumber;

                    SqlParameter responseInsuredDob = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_INSURED_DOB, SqlDbType.VarChar ) );
                    responseInsuredDob.Value = aTicket.BenefitsResponse.ResponseInsuredDOB;

                    SqlParameter responseInsuredFirstName = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_INSURED_FIRST_NAME, SqlDbType.VarChar ) );
                    responseInsuredFirstName.Value = aTicket.BenefitsResponse.ResponseInsuredFirstName;

                    SqlParameter responseInsuredMiddleInit = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_INSURED_MIDDLE_INIT, SqlDbType.VarChar ) );
                    responseInsuredMiddleInit.Value = aTicket.BenefitsResponse.ResponseInsuredMiddleInitial;

                    SqlParameter responseInsuredLastName = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_INSURED_LAST_NAME, SqlDbType.VarChar ) );
                    responseInsuredLastName.Value = aTicket.BenefitsResponse.ResponseInsuredLastName;

                    SqlParameter responsePayorName = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_PAYOR_NAME, SqlDbType.VarChar ) );
                    responsePayorName.Value = aTicket.BenefitsResponse.ResponsePayorName;

                    SqlParameter responseSubscriberId = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_SUBSCRIBER_ID, SqlDbType.VarChar ) );
                    responseSubscriberId.Value = aTicket.BenefitsResponse.ResponseSubscriberID;

                    SqlParameter requestInsuredDob = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_REQUEST_INSURED_DOB, SqlDbType.VarChar ) );
                    requestInsuredDob.Value = aTicket.BenefitsResponse.RequestInsuredDOB;

                    SqlParameter requestInsuredFirstName = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_REQUEST_INSURED_FIRST_NAME, SqlDbType.VarChar ) );
                    requestInsuredFirstName.Value = aTicket.BenefitsResponse.RequestInsuredFirstName;

                    SqlParameter requestInsuredMiddleInit = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_REQUEST_INSURED_MIDDLE_INIT, SqlDbType.VarChar ) );
                    requestInsuredMiddleInit.Value = aTicket.BenefitsResponse.RequestInsuredMiddleInitial;

                    SqlParameter requestInsuredLastName = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_REQUEST_INSURED_LAST_NAME, SqlDbType.VarChar ) );
                    requestInsuredLastName.Value = aTicket.BenefitsResponse.RequestInsuredLastName;

                    SqlParameter requestPayorName = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_REQUEST_PAYOR_NAME, SqlDbType.VarChar ) );
                    requestPayorName.Value = aTicket.BenefitsResponse.RequestPayorName;

                    SqlParameter requestSubscriberId = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_REQUEST_SUBSCRIBER_ID, SqlDbType.VarChar ) );
                    requestSubscriberId.Value = aTicket.BenefitsResponse.RequestSubscriberID;

                    SqlParameter responseStatus = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_STATUS, SqlDbType.Int ) );
                    responseStatus.Value = aTicket.BenefitsResponse.ResponseStatus.Oid;

                    SqlParameter responseAuthCo = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_AUTH_CO, SqlDbType.VarChar ) );
                    responseAuthCo.Value = aTicket.BenefitsResponse.ResponseAuthCo;

                    SqlParameter responseAuthCoPhone = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_RESPONSE_AUTH_CO_PHONE, SqlDbType.VarChar ) );
                    responseAuthCoPhone.Value = aTicket.BenefitsResponse.ResponseAuthCoPhone;
                }

                sqlCommand.ExecuteNonQuery();

            }
            catch ( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    "DataValidation Broker failed to update the availability",
                    anyException,
                    Logger );
            }
            finally
            {
                Close( sqlCommand );
            }

        }


        /// <exception cref="Exception">DataValidation Broker failed to update the availability</exception>
        public virtual void SaveResponseIndicator( string aTicketId, bool responseAvailable )
        {
            Logger.InfoFormat( "DataValidation - Saving response indicator for TicketID: {0}", aTicketId );

            SqlCommand sqlCommand = null;

            try
            {
                Logger.InfoFormat( "Save response intiated for {0}", aTicketId );

                sqlCommand = CommandFor( DBPROCEDURE_UPDATE_TICKET_AVAILABLE );

                SqlParameter ticketIdParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_TICKET_ID, SqlDbType.VarChar ) );
                ticketIdParam.Value = aTicketId.Trim();

                SqlParameter availableParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_AVAILABILITY, SqlDbType.Char ) );
                availableParam.Value = GetYesNoValue( responseAvailable );

                SqlParameter successfulUpdateParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_SUCCESSFUL_UPDATE, SqlDbType.Int ) );
                successfulUpdateParam.Direction = ParameterDirection.Output;

                sqlCommand.ExecuteNonQuery();

                // You have to coerce the output parameter's value attribute to the SqlDecimal type to
                // be able to get at the methods that let you convert the value to native c# types
                bool updateWasSuccessful = ( (int)successfulUpdateParam.Value == 1 );
                if ( updateWasSuccessful )
                {
                    Logger.InfoFormat( "DataValidation - Save response succeeded - Saved response indicator for TicketID: {0} with " +
                        "value of {1} at {2}", aTicketId, responseAvailable, DateTime.Now );
                }
                else
                {
                    Logger.InfoFormat( "DataValidation - Failed to update response availablity - 1st try for TicketID: {0} with " +
                            "value of {1} at {2}", aTicketId, responseAvailable, DateTime.Now );

                    Thread.Sleep( 2000 );

                    sqlCommand.ExecuteNonQuery();

                    updateWasSuccessful = ( (int)successfulUpdateParam.Value == 1 );
                    if ( updateWasSuccessful )
                    {
                        Logger.InfoFormat( "DataValidation - Save response succeeded - Saved response indicator for TicketID: {0} with " +
                            "value of {1} at {2}", aTicketId, responseAvailable, DateTime.Now );
                    }
                    else
                    {
                        Logger.InfoFormat( "DataValidation - Failed to update response availablity - 2nd try for TicketID: {0} with " +
                            "value of {1} at {2}", aTicketId, responseAvailable, DateTime.Now );

                        Thread.Sleep( 2000 );

                        sqlCommand.ExecuteNonQuery();

                        updateWasSuccessful = ( (int)successfulUpdateParam.Value == 1 );
                        if ( updateWasSuccessful )
                        {
                            Logger.InfoFormat( "DataValidation - Save response succeeded - Saved response indicator for TicketID: {0} with " +
                                "value of {1} at {2}", aTicketId, responseAvailable, DateTime.Now );
                        }
                        else
                        {
                            Logger.InfoFormat( "DataValidation - Failed to update response availablity - 3rd try for TicketID: {0} with " +
                                "value of {1} at {2}", aTicketId, responseAvailable, DateTime.Now );

                            Thread.Sleep( 2000 );

                            sqlCommand.ExecuteNonQuery();

                            updateWasSuccessful = ( (int)successfulUpdateParam.Value == 1 );
                            if ( updateWasSuccessful )
                            {
                                Logger.InfoFormat( "DataValidation - Save response succeeded - Saved response indicator for TicketID: {0} with " +
                                    "value of {1} at {2}", aTicketId, responseAvailable, DateTime.Now );
                            }
                            else
                            {
                                Logger.InfoFormat( "DataValidation - No rows updated - Failed to update response for TicketID: {0} with " +
                                    "value of {1} at {2}", aTicketId, responseAvailable, DateTime.Now );
                                Logger.Info( "TicketID: #" + aTicketId + "#" );
                                Logger.Info( "The value returned from the update was : " + successfulUpdateParam.Value );
                            }
                        }
                    }
                }

                sqlCommand.Dispose();
            }
            catch ( Exception anyException )
            {
                Logger.InfoFormat( "DataValidation - Saved response indicator failed for TicketID: {0} with " +
                    "value of {1} at {2}", aTicketId, responseAvailable, DateTime.Now );

                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    "DataValidation Broker failed to update the availability",
                    anyException,
                    Logger );
            }
            finally
            {
                Close( sqlCommand );
            }

            // retrieve the ticket to send to the SendFUSInfo method


            DataValidationTicket dvt = GetDataValidationTicketFor( aTicketId );

            SendFUSInfo( dvt, null );

        }


        /// <summary>
        /// Sends the account for compliance check.
        /// </summary>
        /// <param name="accountRequest">The account request.</param>
        /// <returns></returns>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public bool SendAccountForComplianceCheck( AccountDetailsRequest accountRequest )
        {

            bool isCompliant = true;

            var aCreateComplianceCheckerRequest = new createComplianceCheckerRequest();
            var serviceRequest = new ComplianceCheckerRequest();
            createComplianceCheckerRequestResponse aCreateComplianceCheckerRequestResponse;

            try
            {

                Facility facility = _facilityBroker.FacilityWith( accountRequest.FacilityOid );

                serviceRequest.customerId = TENET_CUSTOMER_ID;
                serviceRequest.hspcd = facility.Code;
                serviceRequest.userId = accountRequest.Upn;
                serviceRequest.accRefDocCode = accountRequest.ReferringPhysicianNumber;

                ICollection coverages = accountRequest.Coverages;

                foreach ( Coverage coverage in coverages )
                {

                    if ( coverage.GetType() == typeof( GovernmentMedicareCoverage ) )
                    {

                        GovernmentMedicareCoverage govMedicareCov =
                            coverage as GovernmentMedicareCoverage;

                        if ( govMedicareCov.MBINumber != null )
                        {

                            serviceRequest.insuredMemberID = govMedicareCov.MBINumber;

                        }

                        serviceRequest.insuredPlanCd = govMedicareCov.InsurancePlan.PlanID;

                        break;

                    }

                }

                serviceRequest.autoTrigger = "Y";
                serviceRequest.patientAccount =
                    StringFilter.PadString( accountRequest.AccountNumber.ToString(), '0', LENGTH_OF_ACCOUNT_NUMBER, false );
                serviceRequest.patientDOB = accountRequest.PatientDOB.ToString( "yyyyMMdd" );
                serviceRequest.patientFirstName = accountRequest.PatientFirstName;
                serviceRequest.patientLastName = accountRequest.PatientLastName;
                serviceRequest.patientMedRecNum =
                    StringFilter.PadString( accountRequest.MedicalRecordNumber.ToString(), '0', LENGTH_OF_MRN, false );
                serviceRequest.patientMiddleName = accountRequest.PatientMidInitial;

                ContactPoint patientMailingContactPoint = accountRequest.PatientMailingCP;

                if ( patientMailingContactPoint != null &&
                    patientMailingContactPoint.PhoneNumber != null )
                {
                    serviceRequest.patientPhone = patientMailingContactPoint.PhoneNumber.AsUnformattedString();
                }

                serviceRequest.patientSSN = accountRequest.PatientSSN.UnformattedSocialSecurityNumber;
                serviceRequest.patientSex = accountRequest.PatientSex;

                aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1 = serviceRequest;

                Logger.Info( "Calling Compliance Checker Services for Patient - Acct: " +
                    aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.patientAccount +
                    ", MRN:" + aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.patientMedRecNum +
                    ", HSP:" + aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.hspcd +
                    ", Name:" + aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.patientFirstName +
                    " " + aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.patientLastName );

                aCreateComplianceCheckerRequestResponse = ComplianceCheckerService.createComplianceCheckerRequest( aCreateComplianceCheckerRequest );

                isCompliant = aCreateComplianceCheckerRequestResponse.result;

                if ( !isCompliant )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "Compliance checker returned false", new Exception(), Logger );
                }

                Logger.Info( "Completed Compliance Checker Processing for Patient - Acct: " +
                    aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.patientAccount +
                    ", MRN:" + aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.patientMedRecNum +
                    ", HSP:" + aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.hspcd +
                    ", Name:" + aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.patientFirstName +
                    " " + aCreateComplianceCheckerRequest.ComplianceCheckerRequest_1.patientLastName );
            }
            catch ( SoapException aSoapException )
            {

                isCompliant = false;

                if ( null != aSoapException.Detail )
                {

                    throw BrokerExceptionFactory.BrokerExceptionFrom(
                        aSoapException.Detail.InnerText,
                        aSoapException,
                        Logger );

                }

                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    aSoapException,
                    Logger );


            }
            catch ( Exception ex )
            {
                isCompliant = false;
                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, Logger );
            }

            return isCompliant;
        }


        public bool SendFUSInfo( DataValidationTicket aTicket, User aUser )
        {
            if ( aTicket.AccountNumber == 0
                || !aTicket.ResultsAvailable
                || aTicket.FUSNoteSent )
            {
                Logger.InfoFormat( "DataValidation - Not sending FUS note for {0}-{3}, ResultsAvailable={1}, FUSNoteSent={2}",
                    aTicket.AccountNumber.ToString(), aTicket.ResultsAvailable.ToString(), aTicket.FUSNoteSent.ToString(),
                    aTicket.TicketId.Trim() );
                return true;
            }

            try
            {
                Logger.InfoFormat( "DataValidation - SendFUSInfo for TicketID: {0} Account: {1}", aTicket.TicketId, aTicket.AccountNumber );

                dropFusNote
                    aNoteObj = new dropFusNote();
                BenefitsValidationFusRequest
                    benefitsValidationFusRequest = new BenefitsValidationFusRequest();

                benefitsValidationFusRequest.accountNumber = aTicket.AccountNumber.ToString();
                benefitsValidationFusRequest.hspcd = aTicket.Facility.Code;
                benefitsValidationFusRequest.benefitsValidationUniqueId = aTicket.TicketId;
                benefitsValidationFusRequest.customerId = TENET_CUSTOMER_ID;
                benefitsValidationFusRequest.userId = aUser.SecurityUser.UPN;

                aNoteObj.BenefitsValidationFusRequest_1 = benefitsValidationFusRequest;

                dropFusNoteResponse response = BenefitsValidationFusService.dropFusNote( aNoteObj );

                aTicket.FUSNoteSent = true;
                SaveDataValidationTicket( aTicket );

                Logger.InfoFormat( "SendFUSInfo successful" );

                return response.result;
            }
            catch ( SoapException soapException )
            {
                if ( soapException.Detail != null && soapException.InnerException != null )
                {
                    Logger.InfoFormat( "SendFUSInfo failed: " + soapException.Detail.InnerText );
                }
                else
                {
                    Logger.InfoFormat( "SendFUSInfo failed: " + soapException );
                }
                return false;
            }
            catch ( Exception )
            {
                return false;
            }
        }


        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public AddressValidationResult ValidAddressesMatching(
            Address anAddress, string upn, string hspCode )
        {
            var results = new ArrayList();
            addressValidationResult result = null;

            try
            {
                result = GetValidAddresses( anAddress, upn, hspCode );
               
                foreach ( var address in result.matchingAddresses )
                {
                    var eDVResposneAddress = new Address()
                    {
                        Address1 = address.street,
                        Address2 = address.street2,
                        City = address.city,
                        ZipCode = new ZipCode(String.Concat(address.zip, address.zipExtension)),
                        State = StateWithDescription(address.state, hspCode),
                        Country = Country.NewUnitedStatesCountry()
                    };

                    var addressForResult = ExtractAddress(eDVResposneAddress);
                   
                    if ( address.fipsStateAndCounty != null )
                    {
                        if(!string.IsNullOrEmpty(address.fipsStateAndCounty.countyCode) && !string.IsNullOrEmpty(address.fipsStateAndCounty.stateCode))
                        {
                            var county = CountyWith(address.fipsStateAndCounty.countyCode, address.state, hspCode);
                            addressForResult.County = county;
                        }

                        else
                        {
                            Logger.InfoFormat( "Null or empty is returned for county or sate code by EDV for the address", anAddress );
                        }
                    }

                    else
                    {
                        Logger.InfoFormat("Null is returned for FIPS State and County by EDV for input address", anAddress);
                    }

                    results.Add( addressForResult );
                }

                return new AddressValidationResult( results,
                    result.status.statusCode,
                    result.status.statusDescription );
            }
            catch ( Exception ex )
            {
                // if Data Validation Service is unavailable, throw the exception to handle it in the UI
                if ( ex.ToString().Contains( EDV_UNAVAILABLE_ERROR ) )
                {
                    throw;
                }

                if ( result != null && result.status != null )
                {
                    return new AddressValidationResult( results,
                        result.status.statusCode,
                        result.status.statusDescription );
                }

                return new AddressValidationResult( results, string.Empty, string.Empty );

            }
        }

        public Address ExtractAddress(Address eDVResposneAddress)
        {
            var PASAddress = new Address();
            PASAddress = eDVResposneAddress.DeepCopy() as Address;
            var eDVAddress1 = eDVResposneAddress.Address1.Trim();
            var eDVAddress2 = eDVResposneAddress.Address2.Trim();
            if (eDVAddress1.Length <=  Address.ADDRESS1_LENGTH && eDVAddress2.Length <= Address.ADDRESS2_LENGTH)
            {
                PASAddress.Address1 = eDVAddress1;
                PASAddress.Address2 = eDVAddress2;
            }
            if (eDVAddress1.Length > Address.ADDRESS1_LENGTH && eDVAddress1.Length <= Address.ADDRESS_MAXIMUM_LENGTH)
            {
                var street1 = eDVAddress1.Substring(0, Address.ADDRESS1_LENGTH).Trim();
                PASAddress.Address1 = street1;
                eDVAddress2 = (eDVAddress1.Substring(street1.Length, eDVAddress1.Length - street1.Length) + eDVAddress2).Trim();

            }
            if (eDVAddress1.Length > Address.ADDRESS_MAXIMUM_LENGTH)
            {
                var street1 = eDVAddress1.Substring(0, Address.ADDRESS1_LENGTH).Trim();
                PASAddress.Address1 = street1;
                eDVAddress2 = eDVAddress1.Substring(street1.Length, eDVAddress1.Length - street1.Length).Trim();
            }
            if (eDVAddress2.Length > Address.ADDRESS2_LENGTH)
            {
                PASAddress.Address2 = eDVAddress2.Substring(0, Address.ADDRESS2_LENGTH).Trim();
            }
            else if (eDVAddress2.Length > 0 && eDVAddress2.Length <= Address.ADDRESS2_LENGTH )
            {
                PASAddress.Address2 = eDVAddress2.Substring(0, eDVAddress2.Length).Trim();
            }

            return PASAddress;
        }
        
        /// <exception cref="Exception">DataValidation Broker failed to insert new Ticket</exception>
        protected virtual void SaveNewTicket( DataValidationTicket aTicket )
        {

            SqlCommand sqlCommand = null;

            try
            {
                sqlCommand = CommandFor( DBPROCEDURE_INSERT_NEW_TICKET );


                SqlParameter ticketIdParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_TICKET_ID, SqlDbType.VarChar ) );
                ticketIdParam.Value = aTicket.TicketId.Trim();

                SqlParameter availableParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_AVAILABILITY, SqlDbType.Char ) );
                availableParam.Value = GetYesNoValue( aTicket.ResultsAvailable );

                SqlParameter facilityParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_FACILITY, SqlDbType.Int ) );
                if ( aTicket.Facility != null )
                {
                    facilityParam.Value = aTicket.Facility.Oid;
                }
                else
                {
                    facilityParam.Value = 0;
                }

                SqlParameter ticketTypeParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_TICKET_TYPE_ID, SqlDbType.Int ) );
                if ( aTicket.TicketType != null )
                {
                    ticketTypeParam.Value = aTicket.TicketType.Oid;
                }
                else
                {
                    ticketTypeParam.Value = 0;
                }

                SqlParameter initiatedOnParam = sqlCommand.Parameters.Add(
                    new SqlParameter( DBPARAMETER_INITIATED_ON, SqlDbType.DateTime ) );
                initiatedOnParam.Value = aTicket.InitiatedOn;

                sqlCommand.ExecuteNonQuery();

            }
            catch ( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    "DataValidation Broker failed to insert new Ticket", anyException );
            }
            finally
            {
                Close( sqlCommand );
            }

        }
        
        /// <exception cref="Exception">There was a problem parsing the Credit Report XML from Data Validation.</exception>
        private CreditReport BuildCreditReport( string xml, long facilityId )
        {
            XmlDocument xmlCreditReport = new XmlDocument();
            CreditReport creditReport = new CreditReport();
            try
            {
                xmlCreditReport.LoadXml( xml );
            }
            catch ( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    "There was a problem parsing the Credit Report XML from Data Validation.",
                    anyException,
                    Logger );
            }

            if ( xmlCreditReport.SelectSingleNode( "//Response/Subject[@fileHit='Y']" ) != null )
            {
                XmlNode firstname = null;
                XmlNode middlename = null;
                XmlNode lastname = null;
                XmlNode ssn = null;

                if ( xmlCreditReport.SelectSingleNode( "//Names/Name[@source='F']" ) != null )
                {
                    firstname = xmlCreditReport.SelectSingleNode( "//Names/Name/First" );
                    middlename = xmlCreditReport.SelectSingleNode( "//Names/Name/Middle" );
                    lastname = xmlCreditReport.SelectSingleNode( "//Names/Name/Last" );
                }

                if ( xmlCreditReport.SelectSingleNode( "//PersonalInformation[@source='F']" ) != null )
                {
                    ssn = xmlCreditReport.SelectSingleNode( "//PersonalInformation/SSN" );
                }

                XmlNode creditScore = xmlCreditReport.SelectSingleNode( "//AddOn[@addOnServiceCode='00219']/ScoringModel/Score" );
                XmlNode phone = xmlCreditReport.SelectSingleNode( "//AddOn[@addOnServiceCode='07030']//Number" );
                XmlNode areaCode = xmlCreditReport.SelectSingleNode( "//AddOn[@addOnServiceCode='07030']//AreaCode" );
                XmlNodeList addresses = xmlCreditReport.SelectNodes( "//Subject/Addresses/Address" );
                XmlNodeList hawkAlerts = xmlCreditReport.SelectNodes( "//HighRiskFraudAlert/Messages/Message" );


                creditReport.ServiceSSN = GetNodeText( ssn );
                creditReport.ServiceFirstName = GetNodeText( firstname );
                creditReport.ServiceMiddleName = GetNodeText( middlename );
                creditReport.ServiceLastName = GetNodeText( lastname );

                string phoneNum = GetNodeText( phone );
                if ( phoneNum.Length > 0 )
                {
                    string areacode = GetNodeText( areaCode );
                    if ( areacode.Length > 0 )
                    {
                        creditReport.ServicePhoneNumber = new PhoneNumber( areacode, phoneNum );
                    }
                    else
                    {
                        creditReport.ServicePhoneNumber = new PhoneNumber( "   ", phoneNum );
                    }
                }
                else
                {
                    creditReport.ServicePhoneNumber = new PhoneNumber();
                }

                string score = GetNodeText( creditScore );
                if ( score != string.Empty )
                {
                    creditReport.CreditScore = Convert.ToInt32( score );
                }

                if ( addresses != null )
                {
                    creditReport.ServiceAddresses = GetAddresses( addresses, facilityId );
                }

                if ( hawkAlerts != null )
                {
                    creditReport.ServiceHawkAlerts = GetHawkAlerts( hawkAlerts );
                    creditReport.FormatedHawkAlert = GetFormatedHawkAlert( xmlCreditReport );
                }

                creditReport.Report = GetFormatedCreditReport( xmlCreditReport );

            }
            return creditReport;
        }


        private ArrayList GetAddresses( XmlNodeList addresses, long facilityId )
        {
            ArrayList result = new ArrayList();
            foreach ( XmlNode addressNode in addresses )
            {
                State state = new State();
                XmlNode stateNode = addressNode.SelectSingleNode( "State" );
                if ( stateNode != null )
                {
                    state = _addressBroker.StateWith( facilityId,stateNode.InnerText );
                }

                Country country = _addressBroker.CountryWith( facilityId, "US" );

                ZipCode zipCode = new ZipCode();
                XmlNode zipNode = addressNode.SelectSingleNode( "ZipCode" );
                if ( zipNode != null )
                {
                    zipCode = new ZipCode( zipNode.InnerText );
                }

                XmlNode houseNumberNode = addressNode.SelectSingleNode( "HouseNumber" );
                string houseNumber = GetNodeText( houseNumberNode );

                XmlNode streetNameNode = addressNode.SelectSingleNode( "StreetName" );
                string streetName = GetNodeText( streetNameNode );

                string street = houseNumber + " " + streetName;

                XmlNode cityNode = addressNode.SelectSingleNode( "City" );
                string city = GetNodeText( cityNode );

                Address address = new Address( street, string.Empty,
                    city, zipCode, state, country );

                result.Add( address );
            }
            return result;
        }


        /// <summary>
        /// Gets the current facility date time.
        /// </summary>
        /// <param name="gmtOffset">The GMT offset.</param>
        /// <param name="dstOffset">The DST offset.</param>
        /// <returns></returns>
        private static DateTime GetCurrentFacilityDateTime( int gmtOffset, int dstOffset )
        {

            ITimeBroker timeBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
            return timeBroker.TimeAt( gmtOffset, dstOffset );
        }

        
        /// <summary>
        /// Gets the formated credit report.
        /// </summary>
        /// <param name="xmlDoc">The XML doc.</param>
        /// <returns></returns>
        internal static string GetFormatedCreditReport( XmlDocument xmlDoc )
        {
            XmlTextReader reader = new XmlTextReader( Assembly.GetExecutingAssembly().
                GetManifestResourceStream( "PatientAccess.PatientAccess.AppServer.Persistence.CreditReport.xslt" ) );

            XslCompiledTransform iTransformer = new XslCompiledTransform();

            iTransformer.Load( reader, null, null );

            StringWriter htmlOutput = new StringWriter();

            iTransformer.Transform( xmlDoc, null, htmlOutput );
            string result = htmlOutput.ToString();

            reader.Close();
            return result;
        }


        internal static string GetFormatedHawkAlert( XmlDocument xmlDoc )
        {
            XmlTextReader reader = new XmlTextReader( Assembly.GetExecutingAssembly().
                GetManifestResourceStream( "PatientAccess.PatientAccess.AppServer.Persistence.HawkAlert.xslt" ) );

            XslCompiledTransform iTransformer = new XslCompiledTransform();

            iTransformer.Load( reader, null, null );

            StringWriter htmlOutput = new StringWriter();

            iTransformer.Transform( xmlDoc, null, htmlOutput );
            string result = htmlOutput.ToString();

            reader.Close();
            return result;
        }


        private static ArrayList GetHawkAlerts( XmlNodeList hawkAlerts )
        {
            ArrayList result = new ArrayList();
            foreach ( XmlNode hawkNode in hawkAlerts )
            {
                string code = hawkNode.Attributes["code"].Value;
                XmlNode testNode = hawkNode.SelectSingleNode( "Text" );
                string message = GetNodeText( testNode );

                XmlNode recommendation = hawkNode.SelectSingleNode( "Recommendation" );

                HawkAlert hawkAlert;
                if ( recommendation != null )
                {
                    hawkAlert = new HawkAlert( code, message, GetRecommendation( recommendation ) );
                }
                else
                {
                    hawkAlert = new HawkAlert( code, message );
                }

                result.Add( hawkAlert );
            }

            return result;
        }


        private static string GetNodeText( XmlNode node )
        {
            string result = String.Empty;
            if ( node != null )
            {
                result = node.InnerText;
            }
            return result;
        }


        private static HawkAlertRecommendation GetRecommendation( XmlNode recommendation )
        {
            XmlNode titleNode = recommendation.SelectSingleNode( "Title" );
            string title = GetNodeText( titleNode );

            XmlNodeList steps = recommendation.SelectNodes( "Steps/Step" );

            HawkAlertRecommendation result = new HawkAlertRecommendation( title );

            if ( steps != null )
            {
                foreach ( XmlNode step in steps )
                {
                    string message = step.InnerText;
                    result.Steps.Add( message );
                }
            }

            return result;
        }


        private DataValidationTicket GetTicketFromReader( SafeReader reader )
        {
            IFacilityBroker facBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            DataValidationTicket aTicket = new DataValidationTicket();
            long facId = reader.GetInt32( DBCOLUMN_FACILITYID );
            Facility facility = facBroker.FacilityWith( facId );

            aTicket.AccountNumber = reader.GetInt32( DBCOLUMN_ACCOUNT_NUMBER );
            aTicket.Facility = facility;
            aTicket.TicketId = reader.GetString( DBCOLUMN_TICKETID );
            aTicket.ResultsAvailable = GetYesNoBoolValue( reader, DBCOLUMN_AVALIABLE );
            aTicket.MedicalRecordNumber = reader.GetInt32( DBCOLUMN_MEDICALRECORDNUMBER );
            aTicket.InitiatedOn = reader.GetDateTime( DBCOLUMN_INITIATEDONDATE );
            aTicket.ResultsReviewed = GetYesNoBoolValue( reader, DBCOLUMN_VIEWED );
            aTicket.FUSNoteSent = GetYesNoBoolValue( reader, DBCOLUMN_FUSNOTESENT );

            long ticketType = reader.GetInt32( DBCOLUMN_VALIDATION_TICKETTYPE );

            if ( ticketType == DataValidationTicketType.TICKKETTYPE_PRIMARY_COVERAGE )
            {
                aTicket.TicketType = DataValidationTicketType.GetNewPrimaryCoveragTicketType();
            }
            else if ( ticketType == DataValidationTicketType.TICKKETTYPE_SECONDARY_COVERAGE )
            {
                aTicket.TicketType = DataValidationTicketType.GetNewSecondaryCoveragTicketType();
            }
            else if ( ticketType == DataValidationTicketType.TICKKETTYPE_GUARANTOR )
            {
                aTicket.TicketType = DataValidationTicketType.GetNewGuarantorTicketType();
            }

            // If values present, build out the BenefitsResponse object
            if ( reader.GetString( DBCOLUMN_REQUEST_INSURED_LAST_NAME ) != null )
            {
                aTicket.BenefitsResponse.BenefitsResponseParseStrategy = reader.GetString( DBCOLUMN_PARSE_STRATEGY );
                if ( reader.GetInt32( DBCOLUMN_PLAN_CATEGORY_OID ) > 0 )
                {
                    long planCategoryId = reader.GetInt64( DBCOLUMN_PLAN_CATEGORY_OID );
                    InsurancePlanCategory planCategory = _insuranceBroker.InsurancePlanCategoryWith( planCategoryId, facility.Oid );

                    aTicket.BenefitsResponse.PlanCategory = planCategory;
                }

                if ( reader.GetInt32( DBCOLUMN_COVERAGE_ORDER_OID ) > 0 )
                {
                    long coverageOrderOid = reader.GetInt32( DBCOLUMN_COVERAGE_ORDER_OID );

                    CoverageOrder coverageOrder;

                    if ( coverageOrderOid == CoverageOrder.NewPrimaryCoverageOrder().Oid )
                    {
                        coverageOrder = CoverageOrder.NewPrimaryCoverageOrder();
                    }
                    else
                    {
                        coverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
                    }

                    aTicket.BenefitsResponse.CoverageOrder = coverageOrder;
                }

                aTicket.BenefitsResponse.PlanCode = reader.GetString( DBCOLUMN_INSURANCE_PLAN_CODE );

                aTicket.BenefitsResponse.RequestInsuredFirstName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen(
                        reader.GetString( DBCOLUMN_REQUEST_INSURED_FIRST_NAME ) );
                aTicket.BenefitsResponse.RequestInsuredMiddleInitial = StringFilter.
                    RemoveFirstCharNonLetter( reader.GetString( DBCOLUMN_REQUEST_INSURED_MIDDLE_INIT ) );
                aTicket.BenefitsResponse.RequestInsuredLastName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen(
                        reader.GetString( DBCOLUMN_REQUEST_INSURED_LAST_NAME ) ); 
                aTicket.BenefitsResponse.RequestInsuredDOB = reader.GetString( DBCOLUMN_REQUEST_INSURED_DOB );
                aTicket.BenefitsResponse.RequestPayorName = reader.GetString( DBCOLUMN_REQUEST_PAYOR_NAME );
                aTicket.BenefitsResponse.RequestSubscriberID = reader.GetString( DBCOLUMN_REQUEST_SUBSCRIBER_ID );

                aTicket.BenefitsResponse.ResponseInsuredDOB = reader.GetString( DBCOLUMN_RESPONSE_INSURED_DOB );
                aTicket.BenefitsResponse.ResponseInsuredFirstName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen(
                        reader.GetString( DBCOLUMN_RESPONSE_INSURED_FIRST_NAME ) );
                aTicket.BenefitsResponse.ResponseInsuredMiddleInitial = StringFilter.
                    RemoveFirstCharNonLetter( reader.GetString( DBCOLUMN_RESPONSE_INSURED_MIDDLE_INIT ) );
                aTicket.BenefitsResponse.ResponseInsuredLastName = StringFilter.
                    RemoveFirstCharNonLetterAndRestNonLetterNumberSpaceAndHyphen(
                        reader.GetString( DBCOLUMN_RESPONSE_INSURED_LAST_NAME ) );
                aTicket.BenefitsResponse.ResponseGroupNumber = reader.GetString( DBCOLUMN_RESPONSE_GROUP_NUMBER );
                aTicket.BenefitsResponse.ResponsePayorName = reader.GetString( DBCOLUMN_RESPONSE_PAYOR_NAME );
                aTicket.BenefitsResponse.ResponseSubscriberID = reader.GetString( DBCOLUMN_RESPONSE_SUBSCRIBER_ID );
                aTicket.BenefitsResponse.ResponseAuthCo = reader.GetString( DBCOLUMN_RESPONSE_AUTH_CO );
                aTicket.BenefitsResponse.ResponseAuthCoPhone = reader.GetString( DBCOLUMN_RESPONSE_AUTH_CO_PHONE );

                long status = reader.GetInt32( DBCOLUMN_RESPONSE_STATUS );

                switch ( status )
                {
                    case BenefitResponseStatus.ACCEPTED_OID:
                        {
                            aTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewAcceptedStatus();
                            break;
                        }
                    case BenefitResponseStatus.REJECTED_OID:
                        {
                            aTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewRejectedStatus();
                            break;
                        }
                    case BenefitResponseStatus.AUTO_ACCEPTED_OID:
                        {
                            aTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewAutoAcceptedStatus();
                            break;
                        }
                    case BenefitResponseStatus.DEFERRED_OID:
                        {
                            aTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewDeferredStatus();
                            break;
                        }
                    case BenefitResponseStatus.UNKNOWN_OID:
                        {
                            aTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewUnknownStatus();
                            break;
                        }
                    default:
                        {
                            aTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewUnknownStatus();
                            break;
                        }
                }
            }

            return aTicket;
        }


        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private addressValidationResult GetValidAddresses( Address address, string upn, string hspCode )
        { 
            var serviceRequest = new addressValidationRequest(); 
            
            var serviceResult = new addressValidationResult();

            try
            {
                if ( string.IsNullOrEmpty( address.ZipCode.ZipCodePrimary ) )
                {
                    address.ZipCode.PostalCode = "00000";
                }

                if ( address.ZipCode.ZipCodePrimary.Length == 5 )
                {
                    serviceRequest.customerId = TENET_CUSTOMER_ID;
                    serviceRequest.hspcd = hspCode;
                    serviceRequest.userId = upn;

                    serviceRequest.truncationLimit = -1;

                    serviceRequest.address = new AddressValidationProxy.address();
                    serviceRequest.address.street = address.Address1;
                    serviceRequest.address.street2 = address.Address2;
                    serviceRequest.address.city = address.City;

                    serviceRequest.address.zip = address.ZipCode.ZipCodePrimary;

                    if ( address.State != null )
                    {
                        serviceRequest.address.state = address.State.Code;
                    }

                    serviceResult = AddressValidationService.validateAddress( serviceRequest );
                }
            }
            catch ( SoapException se )
            {
                string msg = "DataValidationBroker web service failed: " + se.Detail.InnerText;
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, se, Logger );
            }

            catch ( Exception ex )
            {
                string msg = "DataValidationBroker failed to call web service:";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, Logger );
            }

            return serviceResult;
        }


        private static bool GetYesNoBoolValue( SafeReader reader, string column )
        {

            bool results = false;
            string isAvaliable = reader.GetString( column );

            if ( isAvaliable == "Y" )
            {
                results = true;
            }

            return results;
        }


        private static string GetYesNoValue( bool val )
        {
            string result;
            if ( val )
            {
                result = "Y";
            }
            else
            {
                result = "N";
            }
            return result;
        }


        /// <summary>
        /// Determines whether [has parse strategy defined for] [the specified the benefits validation respose].
        /// </summary>
        /// <param name="theBenefitsValidationRespose">The benefits validation respose.</param>
        /// <returns>
        /// 	<c>true</c> if [has parse strategy defined for] [the specified the benefits validation respose]; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasParseStrategyDefinedFor( BenefitsValidationResponse theBenefitsValidationRespose )
        {

            bool isStrategyDefined = false;

            if ( null != theBenefitsValidationRespose &&
                 null != theBenefitsValidationRespose.ReturnedDataValidationTicket &&
                 null != theBenefitsValidationRespose.ReturnedDataValidationTicket.BenefitsResponse )
            {

                isStrategyDefined =
                    !String.IsNullOrEmpty(
                        theBenefitsValidationRespose.ReturnedDataValidationTicket
                                                    .BenefitsResponse
                                                    .BenefitsResponseParseStrategy );

            }

            return isStrategyDefined;

        }


        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {

            string complianceCheckerServiceUrl =
                ConfigurationManager.AppSettings["ComplianceCheckerServiceUrl"];
            string creditValidationServiceUrl =
                ConfigurationManager.AppSettings["CreditValidationServiceUrl"];
            string priorAccountBalanceServiceUrl =
                ConfigurationManager.AppSettings["PriorAccountBalanceServiceUrl"];
            string addressValidationServiceUrl =
                ConfigurationManager.AppSettings["AddressValidationServiceUrl"];
            string benefitsValidationFusServiceUrl =
                ConfigurationManager.AppSettings["BenefitsValidationFusServiceUrl"];

            ComplianceCheckerService =
                new ComplianceCheckerService( complianceCheckerServiceUrl );
            CreditValidationService =
                new CreditValidationService( creditValidationServiceUrl );
            PriorAccountBalanceService =
                new PriorAccountBalanceService( priorAccountBalanceServiceUrl );
            AddressValidationService =
                new AddressValidationService( addressValidationServiceUrl );

            BenefitsValidation5010Service = new BenefitsValidation5010ServiceSoapClient(CONFIG_BENEFITSVALIDATION5010SERVICE_ENTRY);


            BenefitsValidationFusService =
                new BenefitsValidationFusService( benefitsValidationFusServiceUrl );

        }


        /// <summary>
        /// StateWithDescription - lookup and retrieve a State instance by the state name (not code)
        /// </summary>
        /// <param name="stateDescription"></param>
        /// <param name="facilityCode"></param>
        /// <returns></returns>
        private static State StateWithDescription(string stateDescription, string facilityCode)
        {
            IAddressBroker addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var facility = facilityBroker.FacilityWith(facilityCode);

            foreach (State state in addressBroker.AllStates(facility.Oid))
            {
                if ( state.Code == stateDescription )
                {
                    return state;
                }
            }

            return new State();
        }

        private County CountyWith(string countyCode, string stateCode, string facilityId )
        {
            var addressBroker = BrokerFactory.BrokerOfType<IAddressBroker>();
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var facility = facilityBroker.FacilityWith( facilityId );
            var county = addressBroker.CountyWith(facility.Oid,stateCode, countyCode);
            return county;
        }

        private BenefitsValidation5010Proxy.serviceKey CreateServiceKeyFor(string ticketId, string userId, string hospitalCode)
        {
            // The override is present because our generic test accounts do not have access to
            // Passport. If these values are present in the web.config, then they are used instead
            // of the current user and facility.
            var serviceKey = new BenefitsValidation5010Proxy.serviceKey
            {
                customerId = TENET_CUSTOMER_ID,
                uuid = ticketId,
                hspcd = hospitalCode,
                userId = userId
            };

            return serviceKey;
        }
        #endregion Methods
    }
}
