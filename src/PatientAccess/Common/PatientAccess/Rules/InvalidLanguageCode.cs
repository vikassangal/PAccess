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
    public class InvalidLanguageCode : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidLanguageCodeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidLanguageCodeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidLanguageCodeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidLanguageCodeEvent = null;   
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

            if( ( account.Patient  != null ) &&  (account.Patient.Language  != null ) )
            {
                if(!(account.Patient.Language.IsValid))
                          
                {
                    if( this.FireEvents && InvalidLanguageCodeEvent != null )
                    {
                        this.InvalidLanguageCodeEvent(this, null);
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

        public InvalidLanguageCode()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
