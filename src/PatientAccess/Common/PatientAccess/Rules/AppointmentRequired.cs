using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Rule to enforce that the Appointment field is populated (for certain activities).
    /// Note, Appointment refers to the Account property ScheduleCode.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AppointmentRequired : LeafRule
    {
        #region Events
        
        public event EventHandler AppointmentRequiredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AppointmentRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AppointmentRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AppointmentRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	            
            
            //SR 1557, Appointment Required for Edit Pre-Admit Newborn
            Account Model = (Account)context;
            if ((Model.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT) ||
                    ( (Model.Activity.GetType().Equals(typeof(EditAccountActivity)) ||
                      Model.Activity.GetType().Equals(typeof(MaintenanceActivity))) &&
                      (Model.AdmitSource.Code == AdmitSource.NEWBORNADMITSOURCE) &&
                       !(Model.Activity.AssociatedActivityType !=null && Model.Activity.AssociatedActivityType == typeof(PreAdmitNewbornActivity)) 
                    )
                )
            {
                return true;
            }
            if( Model != null
                && (Model.ScheduleCode == null
                    ||
                    Model.ScheduleCode.Code.Trim() == string.Empty ))
            {
                if( this.FireEvents && AppointmentRequiredEvent != null )
                {
                    this.AppointmentRequiredEvent(this, null);
                }
            
                return false;
            }
            else
            {
                return true;
            }           
        }

        public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public AppointmentRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
