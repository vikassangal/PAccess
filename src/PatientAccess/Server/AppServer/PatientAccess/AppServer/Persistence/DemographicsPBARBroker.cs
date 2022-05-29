using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements Demographics related methods.
    /// </summary>
    [Serializable]
    public class DemographicsPBARBroker : PBARCodesBroker, IDemographicsBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        // for brokers that support multiple objects these values must be set at the time of the call
        protected override void InitProcNames()
        {
            this.AllStoredProcName = string.Empty;
            this.WithStoredProcName = string.Empty;
        }
        
        /// <summary>
        /// Get a list of all Gender objects including oid, code and description.
        /// </summary>
        
        public ICollection AllTypesOfGenders(long facilityID)
        {
            ICollection typesOfGenders = null;
            var key = CacheKeys.CACHE_KEY_FOR_GENDERS;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    typesOfGenders = LoadDataToArrayList<Gender>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("DemographicsPBARBroker(Gender) failed to initialize", e, c_log);
                }
                return typesOfGenders;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_ALL_GENDERS;
                typesOfGenders = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("DemographicsPBARBroker(Gender) failed to initialize", e, c_log);
            }
            return typesOfGenders;
        }


        /// <summary>
        /// Get one Gender object based on the code
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Gender GenderWith( long facilityID,string code )
        {
            Gender returnGender = null;
            if( code == null)
            {
                throw new ArgumentNullException("code cannot be null or empty");
            }
            code = code.Trim().ToUpper();
            this.InitFacility( facilityID );
            try
            {
                ICollection genders = this.AllTypesOfGenders( facilityID );

                foreach (Gender gender in genders)
                {
                    if (gender.Code.Equals(code))
                    {
                        returnGender = gender;
                        break;
                    }
                }

                if (returnGender == null)
                {
                    this.WithStoredProcName = SP_SELECT_GENDER_WITH;
                    returnGender = CodeWith<Gender>(facilityID,code);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw BrokerExceptionFactory.BrokerExceptionFrom("DemographicsPBARBroker(Gender) failed to initialize", e, c_log);
            }
            return returnGender;
        }

        /// <summary>
        /// Get a list of all MaritalStatus objects including oid, code and description.
        /// </summary>
        public ICollection AllMaritalStatuses( long facilityID )
        {
            ICollection maritalStatuses = null;
            var key = CacheKeys.CACHE_KEY_FOR_MARITALSTATUSES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    maritalStatuses = LoadDataToArrayList<MaritalStatus>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("DemographicsPBARBroker(MaritalStatus) failed to initialize", e, c_log);
                }
                return maritalStatuses;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_ALL_MARITAL_STATUSES;
                maritalStatuses = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("DemographicsPBARBroker(maritalStatuses) failed to initialize", e, c_log);
            }
            return maritalStatuses;
        }
        
        /// <summary>
        /// Get one MaritalStatus object based on the code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public MaritalStatus MaritalStatusWith(long facilityID, string code)
        {
            MaritalStatus returnMaritalStatus = null;
            if (code == null)
            {
                throw new ArgumentNullException("code cannot be null or empty");
            }
            code = code.Trim().ToUpper();
            this.InitFacility(facilityID);
            try
            {
                ICollection maritalStatuses = this.AllMaritalStatuses(facilityID);

                foreach (MaritalStatus maritalStatus in maritalStatuses)
                {
                    if (maritalStatus.Code.Equals(code))
                    {
                        returnMaritalStatus = maritalStatus;
                        break;
                    }
                }

                if (returnMaritalStatus == null)
                {
                    this.WithStoredProcName = SP_SELECT_MARITAL_STATUS_WITH;
                    returnMaritalStatus = CodeWith<MaritalStatus>(facilityID, code);
                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("DemographicsPBARBroker(MaritalStatus) failed to initialize", e, c_log);
            }
            return returnMaritalStatus;
        }       
 

        /// <summary>
        /// Get a list of all Language objects including oid, code and description.
        /// </summary>
        public ICollection AllLanguages( long facilityID )
        {
            ICollection allLanguages = null;
            var key = CacheKeys.CACHE_KEY_FOR_LANGUAGES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    allLanguages = LoadDataToArrayList<Language>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("DemographicsPBARBroker(Languages) failed to initialize", e, c_log);
                }
                return allLanguages;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_ALL_LANGUAGES;
                allLanguages = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("DemographicsPBARBroker(Languages) failed to initialize", e, c_log);
            }
            return allLanguages;
        }


        /// <summary>
        /// Get one Language object based on the code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Language LanguageWith( long facilityID,string code )
        {
            Language selectedLanguage = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim().ToUpper();
            this.InitFacility( facilityID );
            try
            {
                ICollection languages = this.AllLanguages( facilityID );

                foreach (Language language in languages)
                {
                    if (language.Code.Equals(code))
                    {
                        selectedLanguage = language;
                        break; 
                    }
                }

                if (selectedLanguage == null)
                {
                    this.WithStoredProcName = SP_SELECT_LANGUAGE_WITH;
                    selectedLanguage = CodeWith<Language>(facilityID, code);
                }
             }
           catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("DemographicsPBARBroker(Languages) failed to initialize", e, c_log);
            }
            return selectedLanguage;
        }       
       
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DemographicsPBARBroker()
            : base()
        {
        }
        public DemographicsPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public DemographicsPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( DemographicsPBARBroker ) );
        #endregion

        #region Constants

        private const string
            SP_SELECT_ALL_GENDERS
            = "SELECTALLGENDERS",
            SP_SELECT_GENDER_WITH
            = "SELECTGENDERWITH",
            SP_SELECT_ALL_MARITAL_STATUSES
            = "SELECTALLMARITALSTATUSES",
            SP_SELECT_MARITAL_STATUS_WITH
            = "SELECTMARITALSTATUSWITH",
            SP_SELECT_LANGUAGE_WITH
                = "SELECTLANGUAGEWITH",
            SP_SELECT_ALL_LANGUAGES
            = "SELECTALLLANGUAGES";
        
        #endregion
    }
}
