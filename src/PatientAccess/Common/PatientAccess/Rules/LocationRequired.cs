using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for LocationRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class LocationRequired : LeafRule
    {
        #region Events
        
        public event EventHandler LocationRequiredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.LocationRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.LocationRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.LocationRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }
            
            Account anAccount = (Account)context;

            if( anAccount.Activity is TransferOutToInActivity ||
                  anAccount.Activity is AdmitNewbornActivity ||
                  ( ( anAccount.Activity is RegistrationActivity || 
                      anAccount.Activity is TransferInToOutActivity ||
                      anAccount.Activity is TransferActivity ||
                      anAccount.Activity is TransferERToOutpatientActivity 
                     ) &&                                      
                     ( anAccount.KindOfVisit != null &&
                       anAccount.KindOfVisit.IsInpatient || 
                       ( anAccount.KindOfVisit.IsOutpatient ||
                         anAccount.KindOfVisit.IsRecurringPatient ||
                         anAccount.KindOfVisit.IsNonPatient ) &&
                         anAccount.HospitalService != null &&
                         anAccount.HospitalService.IsDayCare()      // HSV = 58, 59, FO, LD or LB
                       )
                   )
                )
            {
                if ( anAccount.Location                                  == null 
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
                    if( this.FireEvents && LocationRequiredEvent != null )
                    {
                        this.LocationRequiredEvent(this, null);
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

        public LocationRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
