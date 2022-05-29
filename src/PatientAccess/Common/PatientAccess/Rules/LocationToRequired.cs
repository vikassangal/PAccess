using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for LocationToRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class LocationToRequired : LeafRule
    {
        #region Events

        public event EventHandler LocationToRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.LocationToRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.LocationToRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.LocationToRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }
            
            Account anAccount = (Account)context;

            if( anAccount.KindOfVisit != null &&
                anAccount.HospitalService != null &&
                anAccount.HospitalService.Code != null &&
                ( anAccount.Activity is TransferOutToInActivity ||
                  anAccount.Activity is AdmitNewbornActivity ||
                  ( anAccount.Activity is RegistrationActivity &&                    
                    ( anAccount.KindOfVisit.IsInpatient || 
                      ( anAccount.KindOfVisit.IsOutpatient ||
                        anAccount.KindOfVisit.IsRecurringPatient ) &&
                      ( anAccount.HospitalService.Code == HospitalService.HSV57 ||
                        anAccount.HospitalService.Code == HospitalService.HSV58 ||
                        anAccount.HospitalService.Code == HospitalService.HSV59 ||
                        anAccount.HospitalService.Code == "FO" ||
                        anAccount.HospitalService.Code == "LD" ) ||
                      ( anAccount.KindOfVisit.IsNonPatient &&
                        anAccount.HospitalService.Code == "LB"  ) 
                    )
                  )
                )
              )        
            {
                if( anAccount.Location                                  == null 
                    || anAccount.Location.Room                          == null
                    || anAccount.Location.Room.Code                     == null
                    || anAccount.Location.Bed                           == null
                    || anAccount.Location.Bed.Code                      == null
                    || anAccount.Location.NursingStation                == null
                    || anAccount.Location.NursingStation.Code           == null
                    || anAccount.Location.NursingStation.Code.Trim()    == string.Empty
                    || anAccount.Location.Room.Code.Trim()              == string.Empty
                    || anAccount.Location.Bed.Code.Trim()               == string.Empty )
                {
                    if( this.FireEvents && LocationToRequiredEvent != null )
                    {
                        this.LocationToRequiredEvent(this, null);
                    }
            
                    return false;
                }
                else
                {
                    return true;
                }
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

        public LocationToRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
