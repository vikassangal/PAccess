using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using log4net;

namespace PatientAccess.Persistence
{

    /// <summary>
    /// This class is responsible for retrieving all dialysis center records from the PBAR database
    /// </summary>
    [Serializable]
    public class DialysisCenterPbarBroker : AbstractPBARBroker, IDialysisCenterBroker
    {
        #region Constants

        private const string MESSAGE_FAIL_TO_INITIALIZE_ERROR = "DialysisCenterPbarBroker failed to initialize.";
        private const string MESSAGE_ERROR_RETRIEVING_FACILITIES = "Error retrieving alternate care facilities";
        private const string DBPARAM_FACILITY_ID = "@P_FACILITYID";
        private const string DBCOLUMN_DESCRIPTION = "DESCRIPTION";
        private const string SP_SELECT_ALL_DIALYSISCENTERNAMES = "CALL SELECTALLDIALYSISCENTERNAMES( " + DBPARAM_FACILITY_ID + " )";

        #endregion Constants

        #region Fields

        private static readonly ILog Log = LogManager.GetLogger( typeof( DialysisCenterPbarBroker ) );

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DialysisCenterPbarBroker"/> class.
        /// </summary>
        /// <param name="cxnString">The CXN string.</param>
        public DialysisCenterPbarBroker( string cxnString ) : base( cxnString )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialysisCenterPbarBroker"/> class.
        /// </summary>
        /// <param name="txn">The TXN.</param>
        public DialysisCenterPbarBroker( IDbTransaction txn ) : base( txn )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialysisCenterPbarBroker"/> class.
        /// </summary>
        public DialysisCenterPbarBroker()
        {
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        ///  This method returns a collection of all AlternateCareFacilities in a given facility
        ///  querying through HPADQTNH PBAR table.
        /// </summary>
        /// <param name="facilityId">The facility ID.</param>
        /// <returns>ICollection</returns>
        /// <exception cref="Exception">DialysisCenterPbarBroker failed to initialize.</exception>
        public ICollection<string> AllDialysisCenterNames( long facilityId )
        {
            ICollection allDialysisCenterNames;

            try
            {
                var cacheManager = new CacheManager();

                allDialysisCenterNames = cacheManager.GetCollectionBy(
                        CacheKeys.CACHE_KEY_FOR_DIALYSISCENTERNAMES,
                        facilityId,
                        () => LoadDataValuesUsing( facilityId ) );
            }
            catch ( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( MESSAGE_FAIL_TO_INITIALIZE_ERROR, e, Log );
            }

            return allDialysisCenterNames.OfType<string>().ToList();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Loads the data values using facility number
        /// </summary>
        /// <param name="facilityNumber">The facility number.</param>
        /// <returns></returns>
        private ICollection LoadDataValuesUsing( long facilityNumber )
        {
            iDB2Command db2Command = null;
            SafeReader safeReader = null;
            var facilityDescriptions = new ArrayList();

            try
            {
                var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                var facility = facilityBroker.FacilityWith( facilityNumber );

                db2Command = CommandFor( SP_SELECT_ALL_DIALYSISCENTERNAMES, CommandType.Text, facility );

                db2Command.Parameters[DBPARAM_FACILITY_ID].Value = facility.Oid;
                safeReader = ExecuteReader( db2Command );

                while ( safeReader.Read() )
                {
                    facilityDescriptions.Add( safeReader.GetString( DBCOLUMN_DESCRIPTION ) );
                }
            }

            catch ( Exception exception )
            {
                Log.Error( MESSAGE_ERROR_RETRIEVING_FACILITIES, exception );
                throw;
            }

            finally
            {
                Close( safeReader );
                Close( db2Command );
            }

            return facilityDescriptions;
        }

        #endregion Private Methods

    }
}
