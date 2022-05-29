using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net; 

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements origin related data loading.
    /// </summary>
    [Serializable]
    public class OriginPBARBroker : PBARCodesBroker, IOriginBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = string.Empty;
            this.WithStoredProcName = string.Empty;
        }
        /// <summary>
        /// Get a list of all Race and Nationality objects including oid, code and description for the facility.
        /// </summary>
        /// <returns>RaceNationalityCollection</returns>
        public ICollection AllRaces(long facilityID)
        {
            ArrayList oList = new ArrayList();
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();
            SafeReader reader = null;
            ICollection allRaces;
            const string key = CacheKeys.CACHE_KEY_FOR_RACES;
            this.AllStoredProcName = SP_SELECT_ALL_RACES;
            InitFacility(facilityID);
            LoadCacheDelegate loadData =
                delegate
                {
                    try
                    {
                        IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                        Facility facility = facilityBroker.FacilityWith(facilityID);
                        sb.Append("CALL " + SP_SELECT_ALL_RACES + "(");
                        if (facilityID!=0)
                            {
                                sb.Append(PARAM_FACILITYID);
                            }
                        sb.Append(")");

                        cmd = this.CommandFor(sb.ToString(), CommandType.Text, facility);
                        if (facilityID != 0)
                            cmd.Parameters[PARAM_FACILITYID].Value = facilityID;
                        reader = this.ExecuteReader(cmd);
                        while (reader.Read())
                        {
                            Race race = new Race();
                            race.Code = reader.GetString(COL_RACECODE).TrimEnd();
                            race.Description= reader.GetString(COL_RACEDESCRIPTION).TrimEnd();
                            race.ParentRaceCode = reader.GetString(COL_PARENTRACECODE).TrimEnd();
                            oList.Add(race);
                        }
                        allRaces = oList;
                    }
                    catch (Exception e)
                    {
                        throw BrokerExceptionFactory.BrokerExceptionFrom(FAIL_TO_INITIALIZE_ERROR_MESSAGE, e, c_log);
                    }
                    return allRaces;
                   
                };

            try
            {
                CacheManager cacheManager = new CacheManager();
                allRaces = cacheManager.GetCollectionBy(key, facilityID, loadData);
            }

            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(FAIL_TO_INITIALIZE_ERROR_MESSAGE, e, c_log);
            }

            return allRaces.OfType<Race>().ToList();
        }


        /// <summary>
        /// Get a list of all Race objects including oid, code and description for the facility.
        /// </summary>
        /// <returns>RaceCollection</returns>
        public ICollection LoadRaces(long facilityID)
        {
            ArrayList allRaces = new ArrayList();
            var racesCollection = this.AllRaces(facilityID);
            if (racesCollection == null)
            {
                return null;
            }

            foreach (Race race in racesCollection)
            {
                if (race != null && String.IsNullOrEmpty(race.ParentRaceCode.Trim()))
                {
                    allRaces.Add(race);
                }
            }

            return allRaces;
        }
        /// <summary>
        /// Get a list of all Nationality objects including oid, code and description for the facility.
        /// </summary>
        /// <returns>NationalityCollection</returns>
        public ICollection LoadNationalities(long facilityID)
        {
            ArrayList allNationalities = new ArrayList();
            var nationalitiesCollection = this.AllRaces(facilityID);
            if (nationalitiesCollection == null)
            {
                return null;
            }

            foreach (Race race in nationalitiesCollection)
            {
                if (race != null && !String.IsNullOrEmpty(race.ParentRaceCode.Trim()))
                {
                    allNationalities.Add(race);
                }
            }

            return allNationalities;
        }

        /// <summary>
        /// Get one Race object based on the oid.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Race RaceWith(long facilityID, long oid )
        {
            throw new BrokerException( "This method not implemeted in PBAR Version" );
        }

        /// <summary>
        /// Get one Race object based on the code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Race RaceWith( long facilityID, string code )
        {
            Race selectedRace = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim();
            this.InitFacility( facilityID );
            try
            {
                ICollection races = this.AllRaces( facilityID );

                foreach( Race race in races )
                {
                    if( race.Code.Equals( code ) )
                    {
                        selectedRace = race;
                        break;
                    }
                }

                if( selectedRace == null )
                {
                    this.WithStoredProcName = SP_SELECT_RACE_WITH;
                    selectedRace = CodeWith<Race>(facilityID,code);
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "OriginBroker(Race) failed to initialize.", e, c_log );
            }
            return selectedRace;
        }

        /// <summary>
        /// Get a list of all Ethnicity objects including oid, code and description.
        /// </summary>
        public ICollection AllEthnicities(long facilityID)
        {
            ArrayList oList = new ArrayList();
            iDB2Command cmd = null;
            StringBuilder sb = new StringBuilder();
            SafeReader reader = null;
            ICollection allEthnicities;
            this.AllStoredProcName = SP_SELECT_ALL_ETHNICITIES;
            const string key = CacheKeys.CACHE_KEY_FOR_ETHNICITIES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData =
                delegate
                {
                    try
                    {
                        IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                        Facility facility = facilityBroker.FacilityWith(facilityID);
                        sb.Append("CALL " + SP_SELECT_ALL_ETHNICITIES + "(");
                        if (facilityID!=0)
                        {
                            sb.Append(PARAM_FACILITYID);
                        }
                        sb.Append(")");

                        cmd = this.CommandFor(sb.ToString(), CommandType.Text, facility);
                        if (facilityID != 0)
                            cmd.Parameters[PARAM_FACILITYID].Value = facilityID;
                        reader = this.ExecuteReader(cmd);
                        while (reader.Read())
                        {
                            Ethnicity ethnicity = new Ethnicity();
                            ethnicity.Code = reader.GetString(COL_ETHNICITYCODE).TrimEnd();
                            ethnicity.Description = reader.GetString(COL_ETHNICITYDESCRIPTION).TrimEnd();
                            ethnicity.ParentEthnicityCode = reader.GetString(COL_PARENTETHNICITYCODE).TrimEnd();
                            oList.Add(ethnicity);
                        }
                        allEthnicities = oList;
                    }
                    catch (Exception e)
                    {
                        throw BrokerExceptionFactory.BrokerExceptionFrom(FAIL_TO_INITIALIZE_ERROR_MESSAGE, e, c_log);
                    }
                    return allEthnicities;
                };
            try
            {
                CacheManager cacheManager = new CacheManager();

                allEthnicities = cacheManager.GetCollectionBy(key, facilityID, loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("OriginPBARBroker(ethnicities) failed to initialize", e, c_log);
            }

            return allEthnicities.OfType<Ethnicity>().ToList();
        }


        public ICollection LoadEthnicities(long facilityID)
        {
            ArrayList allEthnicities = new ArrayList();
            var ethnicitiesCollection = this.AllEthnicities(facilityID);
            if (ethnicitiesCollection == null)
            {
                return null;
            }

            foreach (Ethnicity ethnicity in ethnicitiesCollection)
            {
                if (ethnicity != null && String.IsNullOrEmpty(ethnicity.ParentEthnicityCode.Trim()))
                {
                    allEthnicities.Add(ethnicity);
                }
            }

            return allEthnicities;
        }
        public ICollection LoadDescent(long facilityID)
        {
            ArrayList allDescent = new ArrayList();
            var descentCollection = this.AllEthnicities(facilityID);
            if (descentCollection == null)
            {
                return null;
            }

            foreach (Ethnicity descent in descentCollection)
            {
                if (descent != null && !String.IsNullOrEmpty(descent.ParentEthnicityCode.Trim()))
                {
                    allDescent.Add(descent);
                }
            }

            return allDescent;
        }


        /// <summary>
        /// Get one Ethnicity object based on the code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Ethnicity EthnicityWith( long facilityID, string code )
        {
            Ethnicity selectedEthnicity = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim();
            this.InitFacility( facilityID );
            try
            {
                ICollection ethnicities = this.AllEthnicities( facilityID );
                foreach (Ethnicity ethnicity in ethnicities)
                {
                    if (ethnicity.Code.Equals(code))
                    {
                        selectedEthnicity = ethnicity;
                        break;
                    }
                }
                if (selectedEthnicity == null)
                {
                    if( code.Length >= ETHNICITY_CODE_MAX_LENGTH )
                    {
                        code = code.Substring( 0, 1 );
                    }
                    this.WithStoredProcName = SP_SELECT_ETHNICITY_WITH;
                    selectedEthnicity = CodeWith<Ethnicity>(facilityID,code);
                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("OriginPBARBroker(Race) failed to initialize.", e, c_log);
            }
            return selectedEthnicity;
        }

        /// <summary>
        /// Get one Ethnicity object based on the oid.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public Ethnicity EthnicityWith(long facilityID, long oid)
        {
            throw new BrokerException("This method not implemeted in PBAR Version");
        }


        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OriginPBARBroker()
            : base()
        {
        }
        public OriginPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public OriginPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( OriginPBARBroker ) );
        #endregion

        #region Constants

        private const string
            SP_SELECT_ALL_RACES = "SELECTALLRACES",
            SP_SELECT_RACE_WITH = "SELECTRACEWITH",
            SP_SELECT_ALL_ETHNICITIES = "SELECTALLETHNICITIES",
            SP_SELECT_ETHNICITY_WITH = "SELECTETHNICITYWITH",
            FAIL_TO_INITIALIZE_ERROR_MESSAGE = "OriginPBARBroker(Race) failed to initialize.";

        private const string
            COL_RACECODE = "CODE",
            COL_RACEDESCRIPTION = "DESCRIPTION",
            COL_PARENTRACECODE = "PARENTRACECODE",
            COL_ETHNICITYCODE = "CODE",
            COL_ETHNICITYDESCRIPTION = "DESCRIPTION",
            COL_PARENTETHNICITYCODE = "PARENTETHNICITYCODE";

        private const int ETHNICITY_CODE_MAX_LENGTH = 1;


        #endregion


    }
}
