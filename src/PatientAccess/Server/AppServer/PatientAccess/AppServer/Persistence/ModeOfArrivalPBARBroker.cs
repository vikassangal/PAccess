using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    public class ModeOfArrivalPBARBroker : PBARCodesBroker, IModeOfArrivalBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_ALL_MODEOFARRIVAL_FOR;
            this.WithStoredProcName = SP_SELECT_MODEOFARRIVAL_WITH;
        }
        
        /// <summary>
        /// Get all Modes of Arrival for a facility
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        public ArrayList ModesOfArrivalFor(long facilityID)
        {
            ICollection modesOfArrival = new ArrayList();
            string key = CacheKeys.CACHE_KEY_FOR_MODESOFARRIVAL;
            InitFacility( facilityID );
            try
            {
                CacheManager cacheManager = new CacheManager();
                modesOfArrival = cacheManager.GetCollectionBy<ModeOfArrival>(key, 
                                                                             facilityID, 
                                                                             this.LoadDataToArrayList<ModeOfArrival>);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("ModeOfArrival Broker failed to initialize", e, c_log);
            }
            return (ArrayList)modesOfArrival;
        }

        /// <summary>
        /// Get Mode of Arrival for a facility and code
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ModeOfArrival ModeOfArrivalWith(long facilityID, string code)
        {
            ModeOfArrival SelectedModeOfArrival = null;
            if( code != null )
            {
                code = code.Trim();
            }
            InitFacility( facilityID );
            try
            {
                ArrayList modesOfArrival = this.ModesOfArrivalFor(facilityID);

                foreach (ModeOfArrival modeOfArrival in modesOfArrival)
                {
                    if (modeOfArrival.Code.Equals(code))
                    {
                        SelectedModeOfArrival = modeOfArrival;
                        break;
                    }
                }

                if (SelectedModeOfArrival == null)
                {
                    SelectedModeOfArrival = CodeWith<ModeOfArrival>(facilityID,code);
                }
            }
            catch (Exception e)
            {
                string msg = "ModeOfArrival Broker/Cache failed to initialize.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
        return SelectedModeOfArrival;
        }


        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ModeOfArrivalPBARBroker() : base()
        {
        }
        public ModeOfArrivalPBARBroker( string cxnString ) : base( cxnString )
        {
        }

        public ModeOfArrivalPBARBroker( IDbTransaction txn ) : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ModeOfArrivalPBARBroker ) );
        #endregion

        #region Constants

        private const string
            SP_SELECT_ALL_MODEOFARRIVAL_FOR = "SELECTALLMODEOFARRIVALFOR",
            SP_SELECT_MODEOFARRIVAL_WITH = "SELECTMODEOFARRIVALWITH";
        #endregion
     }
}
