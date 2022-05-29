using System;
using System.Configuration;
using log4net;

namespace PatientAccess.Persistence
{
    public class CacheKeys
    {
        #region Event Handlers
        #endregion

        #region Methods
        public static string KeyFor( string keyName )
        {
            return keyName;
        }

        public static string KeyFor( string keyName, object subKey )
        {
            return keyName + "_" + subKey;
        }


        public static string KeyFor( string keyName, object subKey, object subKey2 )
        {
            return keyName + "_" + subKey + "_" + subKey2;
        }

        public static string CacheExpirationIntervalKeyFor(string cacheName)
        {
            return String.Format( cacheName + "_INTERVAL" );
        }

        public static double CacheExpirationIntervalFor(string cacheName)
        {
            double timeout = DEFAULT_CACHE_EXPIRATION_INTERVAL;

            try
            {
                string timeoutAsString = ConfigurationManager.AppSettings[CacheExpirationIntervalKeyFor(cacheName)];
                timeout = double.Parse(timeoutAsString);
            }
            catch(Exception ex)
            {
               c_log.ErrorFormat("Error reading timeout value from config file for {0}. Using Default", cacheName, ex);
            }
            return timeout;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion 

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( CacheKeys ) );
        #endregion

        #region Constants
        // default cache expiration is only used if there is an error reading from the config file
        private const double DEFAULT_CACHE_EXPIRATION_INTERVAL = 1.0;
        //  All strings used as keys for caching must be defined here as constants.
        //  Keep these constants in alpha order.  Keys that are defined by an HSP Code
        //  must follow the format SOME_KEY_BY_HSP_{0}, keys that are defined by facility id
        //  must follow the format SOME_KEY_BY_FACILITY_ID_{0}.  Follow this convention
        //  when other keys must be defined that are subclassified by some other type.

        public const string
            SOME_KEY_BY_FACILITY_ID = "SOME_KEY_BY_FACILITY_ID",
            SOME_KEY_BY_HSP_CODE = "SOME_KEY_BY_HSP",

            CACHE_KEY_FOR_ACTIVITY_CODES = "CACHE_KEY_FOR_ACTIVITY_CODES",
            CACHE_KEY_FOR_ADMITSOURCES = "CACHE_KEY_FOR_ADMITSOURCES",
            CACHE_KEY_FOR_BENEFITS = "CACHE_KEY_FOR_BENEFITS",
            CACHE_KEY_FOR_CONFIDENTIALCODES = "CACHE_KEY_FOR_CONFIDENTIALCODES",
            CACHE_KEY_FOR_CONDITIONCODES = "CACHE_KEY_FOR_CONDITIONCODES",
            CACHE_KEY_FOR_CONNECTION_SPECS = "CACHE_KEY_FOR_CONNECTION_SPECS",
            CACHE_KEY_FOR_CONDITIONOFSERVICE = "CACHE_KEY_FOR_CONDITIONOFSERVICE",
            CACHE_KEY_FOR_COUNTIES = "CACHE_KEY_FOR_COUNTIES",
            CACHE_KEY_FOR_COUNTRIES = "CACHE_KEY_FOR_COUNTRIES",
            CACHE_KEY_FOR_EMPLOYMENT_STATUSES = "CACHE_KEY_FOR_EMPLOYMENT_STATUSES",
            CACHE_KEY_FOR_FOLLOWUPUNITS = "CACHE_KEY_FOR_FOLLOWUPUNITS",
            CACHE_KEY_FOR_DISCHARGE_DISPOSITIONS = "CACHE_KEY_FOR_DISCHARGE_DISPOSITIONS",
            CACHE_KEY_FOR_DISCHARGE_STATUSES = "CACHE_KEY_FOR_DISCHARGE_STATUSES",
            CACHE_KEY_FOR_ETHNICITIES = "CACHE_KEY_FOR_ETHNICITIES",
            CACHE_KEY_FOR_FACILITIES = "CACHE_KEY_FOR_FACILITIES",
            CACHE_KEY_FOR_FINANCIALCLASSES = "CACHE_KEY_FOR_FINANCIALCLASSES",
            CACHE_KEY_FOR_CREDITCARDTYPES = "CACHE_KEY_FOR_CREDITCARDTYPES",
            CACHE_KEY_FOR_GENDERS = "CACHE_KEY_FOR_GENDERS",
            CACHE_KEY_FOR_HOSPITALCLINICS = "CACHE_KEY_FOR_HOSPITALCLINICS",
            CACHE_KEY_FOR_HSVS = "CACHE_KEY_FOR_HSVS",
            CACHE_KEY_FOR_LANGUAGES = "CACHE_KEY_FOR_LANGUAGES",
            CACHE_KEY_FOR_MARITALSTATUSES = "CACHE_KEY_FOR_MARITALSTATUSES",
            CACHE_KEY_FOR_MODESOFARRIVAL = "CACHE_KEY_FOR_MODESOFARRIVAL",
            CACHE_KEY_FOR_NPPVERSIONS = "CACHE_KEY_FOR_NPPVERSIONS",
            CACHE_KEY_FOR_OCCURANCECODES = "CACHE_KEY_FOR_OCCURANCECODES",
            CACHE_KEY_FOR_PATIENTTYPES = "CACHE_KEY_FOR_PATIENTTYPES",
            CACHE_KEY_FOR_PHYSICIANROLES = "CACHE_KEY_FOR_PHYSICIANROLES",
            CACHE_KEY_FOR_PHYSICIANSPECIALTIES = "CACHE_KEY_FOR_PHYSICIANSPECIALTIES",
            CACHE_KEY_FOR_PRETESTHOSPITALCLINICS = "CACHE_KEY_FOR_PRETESTHOSPITALCLINICS",
            CACHE_KEY_FOR_RACES = "CACHE_KEY_FOR_RACES",
            CACHE_KEY_FOR_READMITCODES = "CACHE_KEY_FOR_READMITCODES",
            CACHE_KEY_FOR_REFERRALSOURCES = "CACHE_KEY_FOR_REFERRALSOURCES",
            CACHE_KEY_FOR_REFERRALFACILITIES = "CACHE_KEY_FOR_REFERRALFACILITIES",
            CACHE_KEY_FOR_REFERRALTYPES = "CACHE_KEY_FOR_REFERRALTYPES",
            CACHE_KEY_FOR_RELIGIONS = "CACHE_KEY_FOR_RELIGIONS",
            CACHE_KEY_FOR_RELIGIOUSPLACESOFWORSHIP = "CACHE_KEY_FOR_RELIGIOUSPLACESOFWORSHIP",
            CACHE_KEY_FOR_ROLES = "CACHE_KEY_FOR_ROLES",
            CACHE_KEY_FOR_SCHEDULECODES = "CACHE_KEY_FOR_SCHEDULECODES",
            CACHE_KEY_FOR_SELECTABLEOCCURANCECODES = "CACHE_KEY_FOR_SELECTABLEOCCURANCECODES",
            CACHE_KEY_FOR_SPANCODES = "CACHE_KEY_FOR_SPANCODES",
            CACHE_KEY_FOR_STATES = "CACHE_KEY_FOR_STATES",
            CACHE_KEY_FOR_TYPESOFRELATIONSHIPS = "CACHE_KEY_FOR_TYPESOFRELATIONSHIPS",
            CACHE_KEY_FOR_WRITEABLE_ACTIVITY_CODES = "CACHE_KEY_FOR_WRITEABLE_ACTIVITY_CODES",
            CACHE_KEY_FOR_ZIPCODESTATUSES = "CACHE_KEY_FOR_ZIPCODESTATUSES",
            // cache keys for rules
            CACHE_RULEWORKLIST = "CACHE_KEY_FOR_RULEWORKLIST",
            CACHE_RULEACTIONS = "CACHE_KEY_FOR_RULEACTIONS",
            CACHE_ALLACTIONS = "CACHE_KEY_FOR_ALLACTIONS",
            CACHE_ALLRULES = "CACHE_KEY_FOR_ALLRULES",
            CACHE_ALLRULESBYID = "CACHE_KEY_FOR_ALLRULESBYID",
            CACHE_ACTIVITIESLOADED = "CACHE_KEY_FOR_ACTIVITIESLOADED",
            // cache keys for 
            CACHE_KEY_FOR_RANGES = "CACHE_KEY_FOR_WORKLISTSELECTIONRANGES",
            CACHE_KEY_FOR_WORKLISTRANGES = "CACHE_KEY_FOR_WORKLISTRANGES",
            CACHE_KEY_FOR_WORKLISTS = "CACHE_KEY_FOR_WORKLISTS",
            // cache keys for pbar brokers
            CACHE_KEY_FOR_ACCOMODATIONCODES = "CACHE_KEY_FOR_ACCOMODATIONCODES",
            CACHE_KEY_FOR_ALLACCOMODATIONS = "CACHE_KEY_FOR_ALLACCOMODATIONS",
            CACHE_KEY_FOR_NURSINGSTATIONS = "CACHE_KEY_FOR_NURSINGSTATIONS",
            CACHE_KEY_FOR_ROOMS = "CACHE_KEY_FOR_ROOMS",
            CACHE_KEY_TENET_PLAN_CODES = "CACHE_KEY_FOR_TENET_PLAN_CODES",
            CACHE_KEY_FOR_TYPESOFVERIFICATIONRULES = "CACHE_KEY_FOR_TYPESOFVERIFICATIONRULES",
            CACHE_KEY_FOR_TYPESOFPRODUCTS = "CACHE_KEY_FOR_TYPESOFPRODUCTS",
            CACHE_KEY_FOR_RESEARCHSTUDIES = "CACHE_KEY_FOR_RESEARCHSTUDIES",
            CACHE_KEY_FOR_ALTERNATECAREFACILITIES = "CACHE_KEY_FOR_ALTERNATECAREFACILITIES",
            CACHE_KEY_FOR_DIALYSISCENTERNAMES  = "CACHE_KEY_FOR_DIALYSISCENTERNAMES",
            CACHE_KEY_FOR_SUFFIXCODES       = "CACHE_KEY_FOR_SUFFIXCODES";
       
            
        #endregion
    }
}
