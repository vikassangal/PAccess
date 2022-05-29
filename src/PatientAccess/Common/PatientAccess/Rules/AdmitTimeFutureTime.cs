using System;
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitTimeFutureTime.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitTimeFutureTime : LeafRule
    {

        #region Events
        
        public event EventHandler AdmitTimeFutureTimeEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AdmitTimeFutureTimeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AdmitTimeFutureTimeEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmitTimeFutureTimeEvent = null;  
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
            DateTime dtAdmitTime;
            DateTime  today;
            
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	

            Acct = (Account)context;

            try
            {
                dtAdmitTime = Acct.AdmitDate.Date;
                today = this.GetCurrentFacilityDateTime( Acct.Facility.GMTOffset, 
                                                         Acct.Facility.DSTOffset );
            }
            catch
            {                                
                return false;
            }

            if( dtAdmitTime.Date == today.Date
                &&
                dtAdmitTime.TimeOfDay.Hours * 100 + dtAdmitTime.TimeOfDay.Minutes
                > today.TimeOfDay.Hours * 100 + today.TimeOfDay.Minutes)
            {
                if( this.FireEvents && this.AdmitTimeFutureTimeEvent != null )
                {
                    this.AdmitTimeFutureTimeEvent(this,null);
                }                
                
                return false;
            }            

            return true;
        }
            

        #endregion

        #region Properties
      
        private Account Acct
        {
            get
            {
                return i_Acct;
            }
            set
            {
                i_Acct = value;
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
            if( this.facDateTime == DateTime.MinValue )
            {
                ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
                facDateTime = timeBroker.TimeAt( gmtOffset, dstOffset );                
            }

            return facDateTime;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AdmitTimeFutureTime()
        {
        }
        #endregion

        #region Data Elements

        private DateTime    facDateTime;
        private Account     i_Acct;
        
        #endregion

        #region Constants
        #endregion
        
    }
}
