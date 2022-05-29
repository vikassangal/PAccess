using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitDateTodayOrGreater.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateTodayOrGreater : DateCannotBePastDate
    {

        #region Events
        
        public event EventHandler AdmitDateTodayOrGreaterEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AdmitDateTodayOrGreaterEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AdmitDateTodayOrGreaterEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmitDateTodayOrGreaterEvent = null;  
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

			if( ((Account)context).Activity == null
				|| ((Account)context).Activity.GetType() == typeof(MaintenanceActivity) 
				|| ((Account)context).Activity.GetType() == typeof(AdmitNewbornActivity)
                || ( (Account)context ).Activity.GetType() == typeof( ShortMaintenanceActivity )
                  || ( (Account)context ).Activity.GetType() == typeof( QuickAccountMaintenanceActivity ) 
				|| ((Account)context).Activity.AssociatedActivityType == typeof(ActivatePreRegistrationActivity)
                || ( (Account)context ).Activity.AssociatedActivityType == typeof( QuickAccountMaintenanceActivity ) )
			{
				return true;
			}

            base.DateToEvaluate = ((Account)context).AdmitDate;

            if( !base.CanBeAppliedTo( context ) )
            {
                if( this.FireEvents && AdmitDateTodayOrGreaterEvent != null )
                {
                    this.AdmitDateTodayOrGreaterEvent(this, null);
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
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AdmitDateTodayOrGreater()
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        #endregion
        
    }
}
