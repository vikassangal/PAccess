using System;
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitDateFiveDaysPast.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateFiveDaysPast : LeafRule
    {
        #region Events
        
        public event EventHandler AdmitDateFiveDaysPastEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            if( this.AdmitDateFiveDaysPastEvent != null )
            {
                Delegate[] delegates = this.AdmitDateFiveDaysPastEvent.GetInvocationList();
                
                foreach( Delegate d in delegates )
                {
                    if( d.Target.GetType() == eventHandler.Target.GetType()
                        && d.Method.Name == eventHandler.Method.Name)
                    {
                        return true;
                    }
                }
            }

            this.AdmitDateFiveDaysPastEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AdmitDateFiveDaysPastEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmitDateFiveDaysPastEvent = null;   
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
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	                     
            
            Acct = (Account)context;               

            if( Acct.Activity == null ||
                Acct.Activity.GetType() == typeof(MaintenanceActivity) ||
                Acct.Activity.GetType() == typeof( ShortMaintenanceActivity )  ||
                Acct.Activity.GetType() == typeof( QuickAccountMaintenanceActivity )  
               )
            {
                return true;
            }

            DateTime dtAdmitDate;
            DateTime  today;
          
            try
            {
                dtAdmitDate = Acct.AdmitDate.Date;
                today = this.GetCurrentFacilityDateTime( Acct.Facility.GMTOffset, 
                                                         Acct.Facility.DSTOffset );
            }
            catch
            {                                
                return false;
            }

            if(  dtAdmitDate.Date != today.Date
                && dtAdmitDate.Date != DateTime.MinValue
                && dtAdmitDate.Date < today.Date.AddDays(-5).Date)
            {
                if( this.FireEvents && this.AdmitDateFiveDaysPastEvent != null )
                {
                    this.AdmitDateFiveDaysPastEvent(this,null);
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
        /// Gets the current facility date time.
        /// </summary>
        /// <param name="gmtOffset">The GMT offset.</param>
        /// <param name="dstOffset">The DST offset.</param>
        /// <returns> return the current date time stamp relative to the user's facility from PBAR hub</returns>
        private DateTime GetCurrentFacilityDateTime( int gmtOffset, int dstOffset )
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt( gmtOffset, dstOffset );
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AdmitDateFiveDaysPast()
        {
        }
        #endregion

        #region Data Elements
        private Account     i_Acct;
        #endregion

        #region Constants
        #endregion
        
    }
}
