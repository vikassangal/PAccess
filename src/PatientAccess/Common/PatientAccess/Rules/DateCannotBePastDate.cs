using System;
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitDatePastDate.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class DateCannotBePastDate : LeafRule
    {

        #region Events
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {            
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }                          
            
            DateTime  facilityDateTime;
           
            Account acct = (Account)context;
            
            try
            {                
                facilityDateTime = this.GetCurrentFacilityDateTime( acct.Facility.GMTOffset, 
                                                                    acct.Facility.DSTOffset );
            }
            catch
            {                                
                return false;
            }

            if( DateToEvaluate != DateTime.MinValue
                && DateToEvaluate.Date < facilityDateTime.Date )
            {                                                       
                return false;
            }

            return true;
        }
            

        #endregion

        #region Properties

        public DateTime DateToEvaluate
        {
            get
            {
                return i_DateToEvaluate;
            }
            set
            {
                i_DateToEvaluate = value;
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// GetCurrentDateTime - return the current datetime stamp relative to the user's facility (from PBAR hub)
        /// </summary>
        /// <returns></returns>
        private DateTime GetCurrentFacilityDateTime( int gmtOffset, int dstOffset )
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt( gmtOffset, dstOffset );
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DateCannotBePastDate()
        {
        }
        #endregion

        #region Data Elements

        private DateTime    i_DateToEvaluate;
        
        #endregion

        #region Constants
        #endregion
        
    }
}
