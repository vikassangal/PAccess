using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidRelForGuarantor Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidRelForGuarantor : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidRelForGuarantorEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidRelForGuarantorEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidRelForGuarantorEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidRelForGuarantorEvent = null;   
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

            if( ( account.Guarantor != null ) 
                &&( account.Patient != null ))
            {
               RelationshipType rel = account.Patient.RelationshipWith( account.Guarantor );
                if(rel != null)
                {
                    if(!rel.IsValid)              
                          
                    {
                        if( this.FireEvents && InvalidRelForGuarantorEvent != null )
                        {
                            this.InvalidRelForGuarantorEvent(this, null);
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

        public InvalidRelForGuarantor()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

