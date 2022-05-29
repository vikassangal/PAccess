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
    public class InvalidClinicForPreMSEChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidClinicForPreMSEChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidClinicForPreMSEChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidClinicForPreMSEChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidClinicForPreMSEChangeEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account  account = (Account)context ;

                                    
            if( account.DischargeDate != DateTime.MinValue )
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
                        if( this.FireEvents && InvalidClinicForPreMSEChangeEvent != null )
                        {
                            this.InvalidClinicForPreMSEChangeEvent(this, null);
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

        public InvalidClinicForPreMSEChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


