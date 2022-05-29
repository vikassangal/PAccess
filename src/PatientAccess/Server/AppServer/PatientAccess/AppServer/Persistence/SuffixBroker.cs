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
    /// This class is responsoble to retrieve all SuffixCodes records from the PBAR table
    /// </summary>
    [Serializable]
    public class SuffixBroker : AbstractPBARBroker, ISuffixBroker
    {
        #region Constants

        private const string MESSAGE_FAIL_TO_INITIALIZE_ERROR =
            "SuffixBroker failed to initialize.";
        private const string MESSAGE_ERROR_RETRIEVING_SUFFIXCODES =
            "Error retrieving Suffix Codes";
        private const string DBPARAM_FACILITY_ID =
            "@P_FACILITYID";
        private const string DBCOLUMN_CODE =
            "CODE";
        private const string SP_SELECT_ALL_SUFFIXCODES =
            "CALL SELECTALLSUFFIXCODES( " + DBPARAM_FACILITY_ID + " )";

        #endregion Constants

        #region Fields

        private static readonly ILog Log =
            LogManager.GetLogger( typeof( SuffixBroker ) );

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SuffixBroker"/> class.
        /// </summary>
        /// <param name="cxnString">The CXN string.</param>
        public SuffixBroker( string cxnString )
            : base( cxnString )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuffixBroker"/> class.
        /// </summary>
        /// <param name="txn">The TXN.</param>
        public SuffixBroker( IDbTransaction txn )
            : base( txn )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuffixBroker"/> class.
        /// </summary>
        public SuffixBroker()
        {
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        ///  This method returns a collection of all SuffixCodes in a given facility
        ///  querying through HPADQTNH PBAR table.
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns>ICollection</returns>
        /// <exception cref="Exception">SuffixBroker failed to initialize.</exception>
        public ICollection<string> AllSuffixCodes(long facilityId)
        {
            ICollection allSuffixCodes;

            try
            {
                var cacheManager = new CacheManager();

                allSuffixCodes = cacheManager.GetCollectionBy(
                        CacheKeys.CACHE_KEY_FOR_SUFFIXCODES,
                        facilityId,
                        () => LoadDataValuesUsing(facilityId));
            }
            catch ( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( MESSAGE_FAIL_TO_INITIALIZE_ERROR, e, Log );
            }

            return allSuffixCodes.OfType<string>().ToList();
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

                db2Command = CommandFor(SP_SELECT_ALL_SUFFIXCODES,
                                     CommandType.Text,
                                     facility );

                db2Command.Parameters[DBPARAM_FACILITY_ID].Value = facility.Oid;
                safeReader = ExecuteReader( db2Command );

                while ( safeReader.Read() )
                {
                    facilityDescriptions.Add( safeReader.GetString( DBCOLUMN_CODE ) );
                }
            }
            catch ( Exception exception )
            {
                Log.Error(MESSAGE_ERROR_RETRIEVING_SUFFIXCODES, exception);
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
