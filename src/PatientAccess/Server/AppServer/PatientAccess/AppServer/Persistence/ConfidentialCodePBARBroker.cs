using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for ConfidentialCodeBroker.
	/// </summary>
	//TODO: Create XML summary comment for ConfidentialCodeBroker
    [Serializable]
    public class ConfidentialCodePBARBroker : PBARCodesBroker, IConfidentialCodeBroker
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECTALLCONFIDENTIALCODESFOR;
            this.WithStoredProcName = SP_SELECTCONFIDENTIALCODEWITH;
        }

        /// <summary>
        /// Get a list of all ConfidentialCode objects including oid, code and description.
        /// </summary>
        /// <returns></returns>
        public IList ConfidentialCodesFor( long facilityNumber )
        {                                    
            string key = CacheKeys.CACHE_KEY_FOR_CONFIDENTIALCODES;
            ICollection facilityConfidentialCodes = null;
            this.InitFacility(facilityNumber);
            try
            {
                CacheManager cacheManager = new CacheManager();
                facilityConfidentialCodes = cacheManager.GetCollectionBy<ConfidentialCode>(key, 
                    facilityNumber, LoadDataToArrayList<ConfidentialCode>);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("ConfidentialCodes broker failed to initialize", e, c_log);
            }
            return (IList)facilityConfidentialCodes;
        }

        public ConfidentialCode ConfidentialCodeWith(long facilityNumber, long id)
        {
            throw new BrokerException("This method not implemented in PBAR version");
        }
        /// <summary>
        /// Get one ConfidentialCode object based on the facility and code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ConfidentialCode ConfidentialCodeWith( long facilityNumber, string code )
        {
            ConfidentialCode selectedConfidentialCode = null;
            if (null == code)
            {
                throw new ArgumentNullException( "Argument, code, should not be null" ) ;
            }
            code = code.Trim();
            InitFacility((facilityNumber));
            try
            {
                ArrayList confidentialCodes = (ArrayList)this.ConfidentialCodesFor(facilityNumber);
                foreach (ConfidentialCode confidentialCode in confidentialCodes)
                {
                    if (confidentialCode.Code.Equals(code))
                    {
                        selectedConfidentialCode = confidentialCode;
                        break;
                    }
                }
                if (selectedConfidentialCode == null)
                {
                    selectedConfidentialCode = CodeWith<ConfidentialCode>( facilityNumber, code );
                }
            }
            catch (Exception e)
            {
                string msg = "ConfidentialCodeBroker failed to initialize";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }

            return selectedConfidentialCode;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ConfidentialCodePBARBroker() : base()
        {
        }

        public ConfidentialCodePBARBroker( string cxnString ) : base( cxnString )
        {
        }

        public ConfidentialCodePBARBroker( IDbTransaction txn ) : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( ConfidentialCodePBARBroker ) );
        #endregion

        #region Constants
        private const string
            SP_SELECTALLCONFIDENTIALCODESFOR    = "SelectAllConfidentialCodesFor",
            SP_SELECTCONFIDENTIALCODEWITH       = "SelectConfidentialCodeWith";
           
        #endregion
    }
}
