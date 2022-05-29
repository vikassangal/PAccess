using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements re-admit code related data loading.
    /// </summary>
    [Serializable]
    public class ReAdmitCodePBARBroker : PBARCodesBroker, IReAdmitCodeBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_ALL_READMITCODES_FOR;
            this.WithStoredProcName = SP_SELECT_READMITCODE_WITH;
        }
        /// <summary>
        /// Get a list of all ReAdmitCode objects including code and description.
        /// </summary>
        /// <returns></returns>
        public ICollection ReAdmitCodesFor(long facilityNumber)
        {
            ICollection facilityReAdmitCodes = null;
            string key = CacheKeys.CACHE_KEY_FOR_READMITCODES;
            this.InitFacility( facilityNumber );
            try
            {
                CacheManager cacheManager = new CacheManager();
                facilityReAdmitCodes = cacheManager.GetCollectionBy<ReAdmitCode>(key,
                    facilityNumber, 
                    LoadDataToArrayList<ReAdmitCode>);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("ReAdmitCodePBARBroker failed to initialize", e, c_log);
            }
            return facilityReAdmitCodes;
        }

        /// <summary>
        /// Get one ReAdmitCode object based on the facility and code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ReAdmitCode ReAdmitCodeWith( long facilityNumber, string code )
        {
                ReAdmitCode selectedReadmitCode = null;
                if( code == null )
                {
                    throw new ArgumentNullException( "code cannot be null or empty" );
                }
                code = code.Trim().ToUpper();
                this.InitFacility( facilityNumber );
                try
                {
                    ICollection allReadmitCodes = this.ReAdmitCodesFor( facilityNumber );
                    foreach( ReAdmitCode readmitcode in allReadmitCodes )
                    {
                        if( readmitcode.Code.Equals( code ) )
                        {
                            selectedReadmitCode = readmitcode;
                            break;
                        }
                    }
                    if( selectedReadmitCode == null )
                    {
                        selectedReadmitCode = CodeWith<ReAdmitCode>( facilityNumber, code );
                    }
                }
                catch( Exception e )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "ReAdmitCodePBARBroker failed to initialize.", e, c_log );
                }
           
            return selectedReadmitCode;
        }     

       
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReAdmitCodePBARBroker()
            : base()
        {
        }
        public ReAdmitCodePBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public ReAdmitCodePBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ReAdmitCodePBARBroker ) );
        #endregion

        #region Constants
        private const string
            SP_SELECT_ALL_READMITCODES_FOR = "SELECTALLREADMITCODESFOR",
            SP_SELECT_READMITCODE_WITH = "SELECTREADMITCODEWITH";
        #endregion
    }
}
