using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Extensions.Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.CFDBLookupProxy;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence.Utilities;
using PatientAccess.Services;
using PatientAccess.Services.Hdi;
using log4net;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace PatientAccess.Persistence
{

    /// <summary>
    /// This class is responsible for the construction of Facility objects
    /// from various sources of information.
    /// </summary>
    [Serializable]
    public class FacilityBroker : CodesBroker, IFacilityBroker
    {

        #region Constants

        private const string SP_SELECTALLFACILITIES = "Facility.SelectAllFacilities";
        private const string SP_SELECTEXTENDEDPROPERTIES = "Facility.SelectExtendedPropertiesFor";
        private const string COL_PROPERTY_NAME = "Name";
        private const string COL_PROPERTY_VALUE = "Value";
        private const string COL_FACILITYID = "Id";
        private const string COL_HSPCODE = "HospitalCode";
        private const string COL_FACILITYNAME = "Name";
        private const string COL_MODTYPE = "ModType";
        private const string COL_FOLLUPUNITID = "FollowupUnitId";
        private const string COL_UTCOFFSET = "UtcOffset";
        private const string COL_DSTOFFSET = "DstOffset";
        private const string COL_ORDERCOMMUNICATION = "OrderComm";
        private const string COL_TENET_CARE = "TenetCare";
        private const string COL_REREGISTER = "Reregister";
        private const string COL_FEDERALTAXID = "FederalTaxID";
        private const string COL_DATABASENAME = "DatabaseName";
        private const string COL_SERVERID = "ServerId";
        private const string COL_MEDICAID_ISSUE_DATE_REQUIRED = "MedicaidIssuedDateRequired";
        private const string COL_USESUSCMRN = "UsesUscMrn";
        private const string UNKNOWN_HOSPITAL_CODE = "UNK";
        private const string XML_ADDRESS = "//Address[@TypeCD='HQ']";
        private const string XML_ADDRESS1 = "Address1";
        private const string XML_CITY = "City";
        private const string XML_STATECD = "StateCD";
        private const string XML_ZIPCD = "ZipCD";
        private const string XML_COUNTRYCD = "CountryCD";
        private const string XML_PHONE = "//Phone[@TypeCD='MAIN']";
        private const string XML_NUMBER = "PhoneNumber";
        private const string DB2_CONNECTION_TEMPLATE_KEY = "DB2ConnectionTemplate";
        private const string DB2UTIL_PASSWORD = "DB2UTIL_PASSWORD";

        #endregion Constants

        #region Fields

        private static readonly ILog c_log = LogManager.GetLogger(typeof(FacilityBroker));
        private ICFDBLookupService i_CFDBLookupService = new CFDBLookupService(ConfigurationManager.AppSettings["CFDBLookupServiceUrl"]);
        private IHDIService i_HdiService = new HDIService();
        private IFollowUpUnitBroker i_FollowUpUnitBroker;
        private object i_FollowUpUnitBrokerLock = new object();
        private IAddressBroker i_AddressBroker;
        private object i_AddressUnitBrokerLock = new object();


        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FacilityBroker"/> class.
        /// </summary>
        /// <param name="cxnString">The CXN string.</param>
        public FacilityBroker(string cxnString)
            : base(cxnString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacilityBroker"/> class.
        /// </summary>
        /// <param name="txn">The TXN.</param>
        public FacilityBroker(SqlTransaction txn)
            : base(txn)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacilityBroker"/> class.
        /// </summary>
        public FacilityBroker()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the address broker.
        /// </summary>
        /// <value>The address broker.</value>
        public IAddressBroker AddressBroker
        {

            get
            {

                return CreateBrokerOnDemand(i_AddressBroker, i_AddressUnitBrokerLock);

            }//get
            set
            {

                i_AddressBroker = value;

            }//set

        }//proeprty


        /// <summary>
        /// Gets or sets the follow up unit broker.
        /// </summary>
        /// <value>The follow up unit broker.</value>
        internal IFollowUpUnitBroker FollowUpUnitBroker
        {

            get
            {

                return CreateBrokerOnDemand(i_FollowUpUnitBroker, i_FollowUpUnitBrokerLock);

            }//get
            set
            {

                i_FollowUpUnitBroker = value;

            }//set

        }//property


        /// <summary>
        /// Gets or sets the CFDB lookup service.
        /// </summary>
        /// <value>The CFDB lookup service.</value>
        internal ICFDBLookupService CFDBLookupService
        {
            get
            {
                return i_CFDBLookupService;

            }//get
            set
            {
                i_CFDBLookupService = value;

            }//set

        }//property


        /// <summary>
        /// Gets or sets the <see cref="IHDIService"/> service.
        /// </summary>
        /// <value>The <see cref="IHDIService"/> service.</value>
        internal IHDIService HdiService
        {

            get
            {

                return i_HdiService;

            }//get
            set
            {

                i_HdiService = value;

            }//set

        }//property

        #endregion Properties

        #region Methods

        /// <summary>
        /// Get a list of all patient type objects including oid, code and description.
        /// </summary>
        /// <returns></returns>
        public ICollection AllFacilities()
        {

            CacheManager cacheManager = new CacheManager();
            ArrayList facilities = null;

            try
            {

                facilities =
                    (ArrayList)cacheManager.GetCollectionBy(
                            CacheKeys.CACHE_KEY_FOR_FACILITIES,
                            new LoadCacheDelegate(LoadFacilityData));

            }//try
            catch (Exception anyException)
            {

                throw BrokerExceptionFactory.BrokerExceptionFrom(
                        "FacilityBroker(Facilities) failed to initialize cache ",
                        anyException,
                        c_log);

            }//catch

            return facilities;

        }//method


        /// <summary>
        /// Gets the Facility by number.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <returns></returns>
        public Facility FacilityWith(long oid)
        {

            Facility result = null;

            foreach (Facility facility in AllFacilities())
            {

                if (facility.Oid == oid)
                {

                    result = facility;
                    break;

                }//if

            }//foreach

            if (null == result)
            {

                throw new ArgumentException(
                    String.Format(
                        "Facility number {0} not found in collection of facilities",
                        oid));

            }//if

            return result;

        }//method


        /// <summary>
        /// Facilities the with.
        /// </summary>
        /// <param name="hospitalCode">The hospital code.</param>
        /// <returns></returns>
        public Facility FacilityWith(string hospitalCode)
        {

            Facility result = null;

            foreach (Facility facility in AllFacilities())
            {

                if (facility.Code == hospitalCode)
                {

                    result = facility;
                    break;

                }//if

            }//foreach

            if (null == result)
            {

                throw new ArgumentException(
                    String.Format(
                        "Facility code {0} not found in collection of facilities",
                        hospitalCode));

            }//if

            return result;

        }//method


        /// <summary>
        /// Gets the app server date time.
        /// </summary>
        /// <returns></returns>
        public DateTime GetAppServerDateTime()
        {

            return DateTime.Now;

        }//method

        public bool IsDatabaseAvailableFor( string facilityServerIP )
        {
            bool isDatabaseAvailable = true;

            try
            {
                isDatabaseAvailable = IBMUtilities.IsDatabaseAvailableFor(facilityServerIP);
            }
            catch( Exception )
            {
                isDatabaseAvailable = false;
            }

            return isDatabaseAvailable;
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (error == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }
            Console.WriteLine("X509Certificate [{0}] Policy Error: '{1}'",
              cert.Subject,
              error.ToString()); return false;
        }

        /// <summary>
        /// Gets the Facility address
        /// </summary>
        /// <param name="aFacility">A facility.</param>
        /// <returns></returns>
        private Address FacilityAddressFor(Facility aFacility)
        {
            string environment = ConfigurationManager.AppSettings["PASServerEnvironment"].ToString();
            if (environment.ToUpper() == "LOCAL"|| environment.ToUpper()== "DEVELOPMENT" || environment.ToUpper() == "TEST")
            {
                string cfdbURL = ConfigurationManager.AppSettings["CFDBLookupServiceUrl"].ToString();
                Uri address = new Uri(cfdbURL);
                ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
                System.Net.ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls | (SecurityProtocolType)3072;
                using (WebClient webClient = new WebClient())
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };
                    var stream = webClient.OpenRead(address);
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        var page = sr.ReadToEnd();
                    }
                }
            }

            Address facilityAddress = null;

            string address1 = string.Empty;
            string city = string.Empty;
            string stateCd = string.Empty;
            string zipCd = string.Empty;
            string countryCd = string.Empty;

            XmlNode addresses = CFDBLookupService.GetFacilityAddresses(aFacility.Code);
            XmlNode addressNode = addresses.SelectSingleNode(XML_ADDRESS);

            if (addressNode != null)
            {

                address1 = addressNode.Attributes.GetNamedItem(XML_ADDRESS1).Value;
                city = addressNode.Attributes.GetNamedItem(XML_CITY).Value;
                stateCd = addressNode.Attributes.GetNamedItem(XML_STATECD).Value;
                zipCd = addressNode.Attributes.GetNamedItem(XML_ZIPCD).Value;
                countryCd = addressNode.Attributes.GetNamedItem(XML_COUNTRYCD).Value;

            }//if

            ZipCode zip = new ZipCode(zipCd);
            State state = AddressBroker.StateWith(stateCd, aFacility);
            Country country = AddressBroker.CountryWith(countryCd, aFacility);

            facilityAddress =
                new Address(address1, string.Empty, city, zip, state, country, new County());

            return facilityAddress;

        }//method


        /// <summary>
        /// Facilities the phone for.
        /// </summary>
        /// <param name="aFacility">A facility.</param>
        /// <returns></returns>
        private PhoneNumber FacilityPhoneFor(Facility aFacility)
        {

            PhoneNumber facilityPhone = null;

            string phoneNumber = string.Empty;

            XmlNode phones = CFDBLookupService.GetFacilityPhones(aFacility.Code);
            XmlNode phoneNode = phones.SelectSingleNode(XML_PHONE);

            if (phoneNode != null)
            {

                phoneNumber = phoneNode.Attributes.GetNamedItem(XML_NUMBER).Value;

            }//if

            facilityPhone = new PhoneNumber(phoneNumber);

            return facilityPhone;

        }//method


        /// <summary>
        /// Gets the reader for facilties.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private Facility GetFaciltyFromReader(SafeReader reader)
        {

            Facility newFacility =
                new Facility(
                    reader.GetInt32(COL_FACILITYID),
                    ReferenceValue.NEW_VERSION,
                    reader.GetString(COL_FACILITYNAME),
                    reader.GetString(COL_HSPCODE),
                    reader.GetInt32(COL_MODTYPE),
                    reader.GetInt32(COL_UTCOFFSET),
                    reader.GetString(COL_ORDERCOMMUNICATION).ToUpper().Equals(YesNoFlag.CODE_YES));

            newFacility.DSTOffset = 
                reader.GetInt32(COL_DSTOFFSET);
            // TODO: Move this to extended properties
            newFacility.UsesUSCMRN =
                reader.GetString(COL_USESUSCMRN).ToUpper().Equals(YesNoFlag.CODE_YES);
            newFacility.FederalTaxID =
                reader.GetString(COL_FEDERALTAXID).Trim();
            newFacility.MedicaidIssueDateRequired =
                reader.GetString(COL_MEDICAID_ISSUE_DATE_REQUIRED).ToUpper().Equals(YesNoFlag.CODE_YES);
            // TODO: Move this to extended properties
            newFacility.TenetCare =
                new YesNoFlag(reader.GetString(COL_TENET_CARE) ?? String.Empty);
            newFacility.Reregister =
                new YesNoFlag(reader.GetString(COL_REREGISTER) ?? String.Empty);
            newFacility.FollowupUnit =
                FollowUpUnitBroker.FollowUpUnitWith(reader.GetInt32(COL_FOLLUPUNITID));            

            string databaseName =
                reader.GetString( COL_DATABASENAME ).Trim();
            string serverId =
                reader.GetString( COL_SERVERID ).Trim();

            newFacility.ConnectionSpec = 
                ConnectionSpecFor( newFacility, databaseName, serverId );
            
            return newFacility;

        }


        /// <summary>
        /// Adds the extended properties to a facility
        /// </summary>
        /// <param name="facility">The facility.</param>
        private void AddExtendedPropertiesTo( ref Facility facility )
        {
            SafeReader extendedPropertiesReader = null;
            SqlCommand oracleCommand = null;

            try
            {
                oracleCommand = GetCommandForFacilityExtendedProperties( facility.Oid );

                extendedPropertiesReader = ExecuteReader(oracleCommand);

                while (extendedPropertiesReader.Read())
                {
                    facility[extendedPropertiesReader.GetString( COL_PROPERTY_NAME )] =
                        extendedPropertiesReader.GetString( COL_PROPERTY_VALUE );
                }
            }
            catch (Exception anyException)
            {
                c_log.Error( 
                    string.Format( "Error processing extended properties for facility {0}", 
                                   facility.Code ),
                    anyException);   
            }
            finally
            {
                Close(extendedPropertiesReader);
                Close(oracleCommand);

            }            
        }


        /// <summary>
        /// Loads the facility data.
        /// </summary>
        /// <returns></returns>
        private ICollection LoadFacilityData()
        {
            SafeReader allFacilitiesReader = null;
            SqlCommand sqlCommand = null;
            string hospitalCodeForLogging = UNKNOWN_HOSPITAL_CODE;
            ArrayList facilities = new ArrayList();

            try
            {
                sqlCommand = GetCommandForAllFacilities();

                allFacilitiesReader = ExecuteReader(sqlCommand);

                c_log.InfoFormat("Loading facility list from peristent storage");

                while (allFacilitiesReader.Read())
                {
                    try
                    {
                        Facility newFacility = GetFaciltyFromReader(allFacilitiesReader);

                        hospitalCodeForLogging = newFacility.Code ?? UNKNOWN_HOSPITAL_CODE;

                        
                        AddAddressInformationTo(newFacility);

                        AddExtendedPropertiesTo(ref newFacility);

                        facilities.Add(newFacility);

                        c_log.InfoFormat("Facility [{0}] {1} loaded and available to the application",
                                          newFacility.Code,
                                          newFacility.Description);

                    }

                    catch (Exception anyException)
                    {
                        c_log.Error(String.Format("Error loading facility {0}. This facility will not be available to the application", hospitalCodeForLogging), anyException);
                    }
                }

                c_log.InfoFormat("Completed loading facility list from peristent storage");

                // If were were unable to build a list of facilities, 
                // this is fatal to the application
                if (0 == facilities.Count)
                {
                    const string fatalMessage = "No facilities were loaded. Application cannot continue";

                    c_log.Fatal(fatalMessage);
                    throw new ApplicationException(fatalMessage);

                }//if

            }//try
            finally
            {
                Close(allFacilitiesReader);
                Close(sqlCommand);

            }//finally    

            return facilities;

        }

        
        private static ConnectionSpec ConnectionSpecFor( Facility newFacility, string databaseName, string serverId )
        {
            try
            {
                 ConnectionSpec spec = new ConnectionSpec();

                if (string.IsNullOrEmpty( serverId ) ||
                    string.IsNullOrEmpty( databaseName ))
                {
                    throw new ArgumentException("No usable connection Spec info returned. Requested Facility = " +
                                                   newFacility.Code);
                }
                
                spec.ServerIP = serverId;
                spec.DatabaseName = databaseName;
                spec.HospitalName = newFacility.Description;
                spec.HospitalCode = newFacility.Code;
                spec.ConnectionString =
                    String.Format(
                        ConfigurationManager.ConnectionStrings[DB2_CONNECTION_TEMPLATE_KEY].ConnectionString,
                        serverId, databaseName, ConfigurationManager.AppSettings[DB2UTIL_PASSWORD]);
                
                return spec;
            }
            catch( Exception anException )
            {
                throw new ApplicationException( anException.ToString() );
            }
            
        }

        private SqlCommand GetCommandForFacilityExtendedProperties( long facilityId )
        {
            SqlCommand sqlCommand = CommandFor(SP_SELECTEXTENDEDPROPERTIES);

            SqlParameter facilityIdParameter =
                new SqlParameter( "facilityid", 
                                     SqlDbType.Int );
            facilityIdParameter.Value = facilityId;
            
            sqlCommand.Parameters.Add( facilityIdParameter );

            return sqlCommand;            
        }

        private SqlCommand GetCommandForAllFacilities()
        {
            SqlCommand sqlCommand = CommandFor(SP_SELECTALLFACILITIES);

            return sqlCommand;
        }


        private void AddAddressInformationTo(Facility newFacility)
        {
            Address facilityAddress = FacilityAddressFor(newFacility);
            PhoneNumber facilityPhone = FacilityPhoneFor(newFacility);
            ContactPoint facilityContactPoint = new ContactPoint(TypeOfContactPoint.NewPhysicalContactPointType());

            facilityContactPoint.Address = facilityAddress;
            facilityContactPoint.PhoneNumber = facilityPhone;
            newFacility.FacilityState = facilityAddress.State;
            newFacility.AddContactPoint(facilityContactPoint);
        }


        //method

        #endregion Methods

    }//class

}//namespace