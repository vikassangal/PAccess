using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for EmployerStatusBroker.
    /// </summary>
    [Serializable]
    public class EmploymentStatusPBARBroker : PBARCodesBroker, IEmploymentStatusBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_ALL_EMPLOYMENT_STATUSES;
            this.WithStoredProcName = SP_SELECT_EMPLOYMENT_STATUS_WITH;
        }

        /// <summary>
        /// Get all Employmentstatuses for a facility.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        public ICollection AllTypesOfEmploymentStatuses(long facilityID)
        {
            ICollection employmentStatuses = null;
            var key = CacheKeys.CACHE_KEY_FOR_EMPLOYMENT_STATUSES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    employmentStatuses = LoadDataToArrayList<EmploymentStatus>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("EmploymentStatusPBARBroker failed to initialize", e, c_log);
                }
                return employmentStatuses;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_ALL_EMPLOYMENT_STATUSES;
                employmentStatuses = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("EmploymentStatusPBARBroker failed to initialize", e, c_log);
            }
            return employmentStatuses;
        }

        /// <summary>
        /// Obsolute after migrating to PBAR
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public EmploymentStatus EmploymentStatusWith( long oid )
        {
            throw new BrokerException( "This method not implemeted in PBAR Version" );
        }

        /// <summary>
        /// Get Employment status  corresponding to a code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public EmploymentStatus EmploymentStatusWith( long facilityID, string code )
        {
            if( code == null )
            {
                throw new ArgumentNullException( "EmploymentStatus code cannot be null." );
            }
            code = code.Trim();
            EmploymentStatus selectedEmploymentStatus = null;
            this.InitFacility( facilityID );
            try
            {
                ICollection employmentStatuses = this.AllTypesOfEmploymentStatuses( facilityID );
                foreach( EmploymentStatus employmentStatus in employmentStatuses )
                {
                    if( employmentStatus.Code.Equals( code ) )
                    {
                        selectedEmploymentStatus = employmentStatus;
                        break;
                    }
                }

                // didn't find it in the list see if it is in the database
                if( selectedEmploymentStatus == null )
                {
                    selectedEmploymentStatus = this.CodeWith<EmploymentStatus>(facilityID, code);
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "EmploymentStatusPBARBroker failed to initialize", e, c_log );
            }
            return selectedEmploymentStatus;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public EmploymentStatusPBARBroker()
            : base()
        {
        }
        public EmploymentStatusPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public EmploymentStatusPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
           LogManager.GetLogger( typeof( EmploymentStatusPBARBroker ) );
        #endregion

        #region Constants

        private const string
            SP_SELECT_ALL_EMPLOYMENT_STATUSES = "SELECTALLEMPLOYMENTSTATUSES",
            SP_SELECT_EMPLOYMENT_STATUS_WITH = "SELECTEMPLOYMENTSTATUSWITH";

        #endregion

    }
}
