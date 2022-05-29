using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for Clinic_1Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidClinicForPreMSE : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidClinicForPreMSEEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidClinicForPreMSEEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidClinicForPreMSEEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidClinicForPreMSEEvent = null;    
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

            if( account != null )
            {                
                //Clinic clinic =  account.Clinic;
                HospitalClinic clinic =  account.HospitalClinic;

                if (clinic != null)
                {
                    if(!(clinic.IsValid))                        
                    {
                        if( this.FireEvents && InvalidClinicForPreMSEEvent != null )
                        {
                            this.InvalidClinicForPreMSEEvent(this, null);
                        }
        
                        return false ;
                    }
                }
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

        public InvalidClinicForPreMSE()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


