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
    public class InvalidPlaceOfWorshipCodeChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidPlaceOfWorshipCodeChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidPlaceOfWorshipCodeChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidPlaceOfWorshipCodeChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidPlaceOfWorshipCodeChangeEvent = null;   
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

            if( ( account.Patient  != null ) &&  (account.Patient.PlaceOfWorship != null ) )
            {
                if(!(account.Patient.PlaceOfWorship.IsValid))
                          
                {
                    if( this.FireEvents && InvalidPlaceOfWorshipCodeChangeEvent != null )
                    {
                        this.InvalidPlaceOfWorshipCodeChangeEvent(this, null);
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
            return true;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public InvalidPlaceOfWorshipCodeChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
