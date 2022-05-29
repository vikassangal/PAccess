using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for AdmitDateFutureDate.
	/// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateFutureDate : DateCannotBeFutureDate
    {

        #region Events
        
        public event EventHandler AdmitDateFutureDateEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AdmitDateFutureDateEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AdmitDateFutureDateEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmitDateFutureDateEvent = null;  
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
            // pass in the AdmitDate as context to the base class       
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	

			Acct = (Account)context;               

			if( Acct.Activity == null ||
				Acct.Activity.GetType() == typeof(MaintenanceActivity) ||
                Acct.Activity.GetType() == typeof( ShortMaintenanceActivity )||
                    Acct.Activity.GetType() == typeof( QuickAccountMaintenanceActivity ))
			{
				return true;
			}


            base.DateToEvaluate = ((Account)context).AdmitDate;

            if( !base.CanBeAppliedTo( context ) )
            {
                if( this.FireEvents && AdmitDateFutureDateEvent != null )
                {
                     this.AdmitDateFutureDateEvent(this, null);
                }
                return false;
            }
            else
            {
                return true;
            }
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
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AdmitDateFutureDate()
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
