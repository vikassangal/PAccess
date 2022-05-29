using System;
using System.Collections;
using System.Data;
using Extensions.DB2Persistence;
using Extensions.PersistenceCommon;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements religion related data loading.
    /// </summary>
    [Serializable]
    public class ReligionPBARBroker : PBARCodesBroker, IReligionBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            AllStoredProcName = string.Empty;
            WithStoredProcName = string.Empty;
        }
        
        /// <summary>
        /// Get a list of all PlacesOfWorship objects including oid, code and description.
        /// </summary>
        /// <returns></returns>
        public IList AllPlacesOfWorshipFor(long facilityNumber)
        {
            ICollection facilityPlacesOfWorship;
            string key = CacheKeys.CACHE_KEY_FOR_RELIGIOUSPLACESOFWORSHIP;
            InitFacility(facilityNumber);

            try
            {
                CacheManager cacheManager = new CacheManager();
                AllStoredProcName = SP_SELECTALLPLACESOFWORSHIPFOR;
                facilityPlacesOfWorship = cacheManager.GetCollectionBy<PlaceOfWorship>(key, 
                    facilityNumber, 
                    LoadDataToArrayList);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("ReligionBroker(PlacesOfWorship) failed to initialize", e, c_log);
            }
            return (IList)facilityPlacesOfWorship;
        }

        /// <summary>
        /// Get one PlacesOfWorship object based on the facility and oid.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public PlaceOfWorship PlaceOfWorshipWith( long facilityNumber, long oid )
        {
            throw new BrokerException("This method not implemeted in PBAR Version");
        }
    
        /// <summary>
        /// Get one PlacesOfWorship object based on the facility and code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public PlaceOfWorship PlaceOfWorshipWith( long facilityNumber, string code )
        {
            PlaceOfWorship selectedPlaceOfWorship = null;
            code = code.Trim();

            try
            {
                ArrayList allPlacesOfWorship = (ArrayList)AllPlacesOfWorshipFor(facilityNumber);
                foreach (PlaceOfWorship placeOfWorship in allPlacesOfWorship)
                {
                    if (placeOfWorship.Code.Equals(code))
                    {
                        selectedPlaceOfWorship = placeOfWorship;
                        break;
                    }
                }

                if (selectedPlaceOfWorship == null)
                {
                    if (code.Length != 0 &&
                        (code.Length >= PLACE_OF_WORSHIP_CODE_MAX_LENGTH)
                        )
                        code = code.Substring(0, 3);
                    WithStoredProcName = SP_SELECTPLACEOFWORSHIPWITH;
                    selectedPlaceOfWorship = CodeWith<PlaceOfWorship>( facilityNumber,code);                }
            }
            catch (Exception e)
            {
                string msg = "ReligionBroker(PlacesOfWorship) failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            return selectedPlaceOfWorship;
        }

        /// <summary>
        /// Get a list of all Religion objects including oid, code and description.
        /// </summary>
        public ICollection AllReligionsdsd(long facilityID)
        {
            ICollection allReligions;
            string key = CacheKeys.CACHE_KEY_FOR_RELIGIONS;
            InitFacility(facilityID);

            try
            {
                CacheManager cacheManager = new CacheManager();
                AllStoredProcName = SP_SELECTALLRELIGIONS;
                allReligions = cacheManager.GetCollectionBy<Religion>(
                    key, 
                    HubName,
                    LoadDataToArrayList);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("ReligionBroker(Religion) failed to initialize", e, c_log);
            }
            return allReligions;
        }
        public ICollection AllReligions(long facilityID)
        {
            ICollection allReligions = null;
            var key = CacheKeys.CACHE_KEY_FOR_RELIGIONS;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    allReligions = LoadDataToArrayList<Religion>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("ReligionBroker(Religion) failed to initialize", e, c_log);
                }
                return allReligions;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECTALLRELIGIONS;
                allReligions = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("ReligionBroker(Religion) failed to initialize", e, c_log);
            }
            return allReligions;
        }
        /// <summary>
        /// Get one Religion object based on the oid.
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        public Religion ReligionWith( long facilityID, long oid )
        {            
            throw new BrokerException("This method not Implemented in PBAR version");
        }
    
        /// <summary>
        /// Get one Religion object based on the code.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="facilityID"></param>
        /// <returns></returns>
         
        public Religion ReligionWith(long facilityID, string code)
        {
            Religion selectedReligion = null;
            if (code == null)
            {
                throw new ArgumentNullException("code cannot be null or empty");
            }
            code = code.Trim().ToUpper();
            this.InitFacility(facilityID);
            try
            {
                ICollection allReligions =  AllReligions(facilityID);

                foreach (Religion religion in allReligions)
                {
                    if (religion.Code.Equals(code))
                    {
                        selectedReligion = religion;
                        break;
                    }
                }

                if (selectedReligion == null)
                {
                    this.WithStoredProcName = SP_SELECTRELIGIONWITH;
                    selectedReligion = CodeWith<Religion>(facilityID, code);
                }
            }
            catch (Exception e)
            {
                string msg = "ReligionBroker(PlacesOfWorship) failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e);
            }
            return selectedReligion;
        }       
        /// <summary>
        /// This method is called for populating the Summary Grid 
        /// in Religion Census Inquiry Screen
        /// </summary>
        /// <param name="facility"></param>
        /// <param name="religionCode"></param>       
        /// <returns>Collection of Religion objects</returns>
        public ICollection ReligionSummaryFor( Facility facility, string religionCode )
        {
            iDB2Command cmd   = null;
            SafeReader reader = null;   
            long facilityId = facility.Oid;
   
            ArrayList religions = new ArrayList();

            try
            {
                cmd = CommandFor( "CALL " + SP_SELECTRELIGIONSSUMMARY +
                    "(" + PARAM_FACILITYID + 
                    "," + PARAM_RELIGION + ")",
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_FACILITYID].Value = facilityId;
                cmd.Parameters[PARAM_RELIGION].Value= religionCode;
              
                reader = ExecuteReader( cmd );

                while( reader.Read() )
                {
                    string religionDesc = reader.GetString( COL_RELIGIONDESC );
                    int religionCount = Convert.ToInt32(
                        reader.GetValue( COL_RELIGIONCOUNT ) );
                    
                    Religion religion = new Religion(
                        PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, 
                        religionDesc, religionCount );
                    religions.Add( religion );
                }
            }
            catch( Exception ex )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unexpected Exception", ex, c_log );
            }
            finally
            {
                Close( reader );
                Close( cmd );
            }
            return religions;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReligionPBARBroker()
        {
        }
        public ReligionPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public ReligionPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ReligionPBARBroker ) );
        #endregion

        #region Constants
        private const string 
            SP_SELECTRELIGIONSSUMMARY               = "RELIGIONSUMMARYFOR",
            SP_SELECTALLPLACESOFWORSHIPFOR          = "SELECTALLPLACESOFWORSHIPFOR",
            SP_SELECTPLACEOFWORSHIPWITH             = "SELECTPLACEOFWORSHIPWITH",
            SP_SELECTALLRELIGIONS                   = "SELECTALLRELIGIONS",
            SP_SELECTRELIGIONWITH                   = "SELECTRELIGIONWITH";

        private const string
            PARAM_RELIGION                          = "@P_ReligionCode";       
        
        private const string
            COL_RELIGIONDESC                        = "ReligionDescription",
            COL_RELIGIONCOUNT                       = "ReligionTotal";

        private const long
            PLACE_OF_WORSHIP_CODE_MAX_LENGTH = 3;

        #endregion
    }
}
