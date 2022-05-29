using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Extensions.DB2Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence.Utilities;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// This class is responsible for retrieving ResearchStudy records from the PBAR database
    /// </summary>
    [Serializable]
    public class ResearchStudyPBARBroker : PBARCodesBroker, IResearchStudyBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Inits the proc names.
        /// </summary>
        protected override void InitProcNames()
        {
            AllStoredProcName = SP_SELECT_ALL_RESEARCH_STUDIES;
            WithStoredProcName = SP_SELECT_RESEARCH_STUDY_WITH;
        }

        /// <summary>
        ///  This method returns a collection of all Research Studies 
        ///  in a given facility querying through HPADQTRZ PBAR table.
        /// </summary>
        /// <param name="facilityID">The facility ID.</param>
        /// <returns>ICollection</returns>
        /// <exception cref="Exception">ResearchStudyPBARBroker failed to initialize.</exception>
        public IEnumerable<ResearchStudy> AllResearchStudies( long facilityID )
        {
            ICollection allResearchStudies;
            const string key = CacheKeys.CACHE_KEY_FOR_RESEARCHSTUDIES;
            InitFacility( facilityID );
            LoadCacheDelegate loadData =
                delegate
                {
                    try
                    {
                        allResearchStudies = LoadDataToArrayList<ResearchStudy>( facilityID,
                                                ResearchStudyFrom );
                    }
                    catch ( Exception e )
                    {
                        throw BrokerExceptionFactory.BrokerExceptionFrom( FAIL_TO_INITIALIZE_ERROR_MESSAGE, e, c_log );
                    }
                    return allResearchStudies;
                };

            try
            {
                CacheManager cacheManager = new CacheManager();
                allResearchStudies = cacheManager.GetCollectionBy( key, facilityID, loadData );
            }

            catch ( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( FAIL_TO_INITIALIZE_ERROR_MESSAGE, e, c_log );
            }

            return allResearchStudies.OfType<ResearchStudy>().ToList();
        }

        private static ResearchStudy ResearchStudyFrom( SafeReader reader )
        {
            var researchSponsor = reader.GetString( COL_RESEARCHSPONSOR ).Trim();
            var code = reader.GetString( COL_RESEARCHSTUDYCODE ).TrimEnd();
            var description = reader.GetString( COL_DESCRIPTION ).TrimEnd();
            var registryNumber = reader.GetInt64(COL_REGISTRYNUMBER);
            var registryNumberStr = registryNumber == 0 ? string.Empty : registryNumber.ToString();
            var terminationDateRaw = reader.GetString( COL_TERMINATIONDATE );
            var terminationDate = DateTimeUtilities.DateTimeForYYYYMMDDFormat( terminationDateRaw );

            return new ResearchStudy( code, description, researchSponsor, registryNumberStr, terminationDate );
        }

        /// <summary>
        /// This methods selects the <see cref="ResearchStudy"/> for a given ResearchStudyCode 
        /// in a given facility by querying through the HPADQTRZ table in PBAR
        /// </summary>
        /// <param name="facilityID">The facility ID.</param>
        /// <param name="code">The code.</param>
        /// <returns><see cref="ResearchStudy"/></returns>
        /// <exception cref="ArgumentNullException"><c>code</c> is null.</exception>
        /// <exception cref="Exception">ResearchStudyPBARBroker failed to initialize.</exception>
        public ResearchStudy ResearchStudyWith( long facilityID, string code )
        {
            if ( code == null )
            {
                throw new ArgumentNullException( CODE_NULL_ERROR_MESSAGE );
            }
            ResearchStudy selectedResearchStudy = null;
            code = code.Trim().ToUpper();
            InitFacility( facilityID );

            try
            {
                IEnumerable<ResearchStudy> allResearchStudies = AllResearchStudies( facilityID );
                foreach ( ResearchStudy researchStudy in allResearchStudies )
                {
                    if ( researchStudy.Code.Equals( code ) )
                    {
                        selectedResearchStudy = researchStudy;
                        break;
                    }
                }
                if ( selectedResearchStudy == null )
                {
                    selectedResearchStudy = CodeWith<ResearchStudy>( facilityID, code );
                }
            }
            catch ( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( FAIL_TO_INITIALIZE_ERROR_MESSAGE, e, c_log );
            }
            return selectedResearchStudy;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ResearchStudyPBARBroker()
        {
        }

        public ResearchStudyPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public ResearchStudyPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ResearchStudyPBARBroker ) );
        #endregion

        #region Constants

        private const string
            SP_SELECT_ALL_RESEARCH_STUDIES = "SELECTALLRESEARCHSTUDIES",
            SP_SELECT_RESEARCH_STUDY_WITH = "SELECTRESEARCHSTUDYWITH",
            FAIL_TO_INITIALIZE_ERROR_MESSAGE = "ResearchStudyPBARBroker failed to initialize.",
            CODE_NULL_ERROR_MESSAGE = "ResearchStudy code cannot be null";

        private const string
            COL_FACILITYID = "FACILITYID",
            COL_RESEARCHSTUDYCODE = "CODE",
            COL_DESCRIPTION = "DESCRIPTION",
            COL_REGISTRYNUMBER = "REGISTRYNUMBER",
            COL_RESEARCHSPONSOR = "SPONSOR",
            COL_TERMINATIONDATE = "TERMINATIONDATE";

        #endregion
    }
}
