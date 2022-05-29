using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidReferralFacilityEvent.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidReferralFacility : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidReferralFacilityEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidReferralFacilityEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidReferralFacilityEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidReferralFacilityEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account  account = (Account)context ;
                                    
            if( account.DischargeDate == DateTime.MinValue )
            {
                return true;
            }

            if( ( account != null ) &&  (account.ReferralFacility  != null ))
            {
                if( (account.Activity.GetType().Equals( typeof( PostMSERegistrationActivity)))||
                    (account.Activity.GetType().Equals( typeof( MaintenanceActivity)) &&
                    ((account.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT ) && 
                    ((account.FinancialClass == null)||
                    ((account.FinancialClass != null)&&
                    (!account.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE)))))))
                {
                    if(!(account.ReferralFacility.IsValid))
                          
                    {
                        if( this.FireEvents && InvalidReferralFacilityEvent != null )
                        {
                            this.InvalidReferralFacilityEvent(this, null);
                        }
            
                        return false ;
                    }
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
            return true;
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

        public InvalidReferralFacility()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


