using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for HospitalServiceRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidHospitalServiceCode : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidHospitalServiceCodeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidHospitalServiceCodeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidHospitalServiceCodeEvent -= eventHandler;
            return true;
        }
        
        public override void UnregisterHandlers()
        {
            this.InvalidHospitalServiceCodeEvent = null;    
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

            if( ( account != null ) &&  (account.HospitalService   != null ) )
            {
                if(!(account.HospitalService .IsValid))
                          
                {
                    if( this.FireEvents && InvalidHospitalServiceCodeEvent != null )
                    {
                        this.InvalidHospitalServiceCodeEvent(this, null);
                    }
            
                    return false ;
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

        public InvalidHospitalServiceCode()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

