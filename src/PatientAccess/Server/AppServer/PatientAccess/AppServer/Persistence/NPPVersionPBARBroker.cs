using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence.Utilities;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for NPPVersionPBARBroker.
    /// </summary>
    //TODO: Create XML summary comment for NPPVersionPBARBroker
    [Serializable]
    public class NPPVersionPBARBroker : PBARCodesBroker, INPPVersionBroker
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_ALL_SELECT_NPPVERSIONS_FOR;
            this.WithStoredProcName = SP_SELECT_NPPVERSION_WITH;
        }
        /// <summary>
        /// Get a list of VPP Version objects based on the facility.
        /// </summary>
        /// <returns></returns>
        public ICollection NPPVersionsFor( long facilityNumber )
        {
            ICollection facilityNPPVersions;
            string key = CacheKeys.CACHE_KEY_FOR_NPPVERSIONS;
            InitFacility( facilityNumber );
            LoadCacheDelegate LoadData = delegate()
            {
                try
                {
                    facilityNPPVersions = LoadDataToArrayList<NPPVersion>( facilityNumber );
                    foreach( NPPVersion npp in facilityNPPVersions )
                    {
                        if( npp.Description.Trim().Length > 0 )
                        {
                            npp.NPPDate = DateTimeUtilities.DateTimeForYYYYMMDDFormat( long.Parse( npp.Description ) );
                        }
                        else
                        {
                            npp.NPPDate = new DateTime( 0 );
                        }
                    }
                }
                catch( Exception e )
                {
                    string msg = "NPPVersionBroker failed to initialize.";
                    throw BrokerExceptionFactory.BrokerExceptionFrom( msg, e, c_log );
                }
                return facilityNPPVersions;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                facilityNPPVersions = cacheManager.GetCollectionBy( key, facilityNumber, LoadData );
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "NPPVersionBroker failed to initialize", e, c_log );
            }
            return facilityNPPVersions;
        }

        /// <summary>
        /// Get one NPPVersion object based on the facility and code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public NPPVersion NPPVersionWith( long facilityNumber, string code )
        {
            NPPVersion selectedNPPVersion = null;
            if( code == null )
            {
                throw new ArgumentException("NPPVersion code cannot be null.");
            }
            else
            {
                code = code.Trim();
                InitFacility( facilityNumber );
                try
                {
                    ArrayList nppVersions = (ArrayList)this.NPPVersionsFor( facilityNumber );

                    // because this table uses 0 for blank in DB2 we need to convert it so
                    // we can find the blank row
                    string searchCode = code;
                    if( code.Equals( "0" ) )
                    {
                        searchCode = string.Empty;
                    }

                    foreach( NPPVersion nppVersion in nppVersions )
                    {
                        if( nppVersion.Code.Equals( searchCode ) )
                        {
                            selectedNPPVersion = nppVersion;
                            break;
                        }
                    }

                    if( selectedNPPVersion == null )
                    {
                        selectedNPPVersion = CodeWith<NPPVersion>( facilityNumber, code );
                    }
                }
                catch( Exception e )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "NPPVersionPBARBroker failed to initialize.", e, c_log );
                }
            }
            return selectedNPPVersion;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public NPPVersionPBARBroker()
            : base()
        {
        }

        public NPPVersionPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public NPPVersionPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( NPPVersionPBARBroker ) );
        #endregion

        #region Constants

        private const string
            SP_ALL_SELECT_NPPVERSIONS_FOR = "SELECTALLNPPVERSIONFOR",
            SP_SELECT_NPPVERSION_WITH = "SELECTNPPVERSIONWITH";
        #endregion
    }

}
