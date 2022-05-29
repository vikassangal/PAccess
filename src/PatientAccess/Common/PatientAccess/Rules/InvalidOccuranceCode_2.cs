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
    public class InvalidOccuranceCode_2 : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidOccurrenceCode_2Event;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidOccurrenceCode_2Event += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidOccurrenceCode_2Event -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidOccurrenceCode_2Event = null;   
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

            if( ( account != null ) &&  (account.OccurrenceCodes  != null )
                &&  (account.OccurrenceCodes.Count > 1  ))
            {
                OccurrenceCode occ =  (OccurrenceCode)account.OccurrenceCodes[1] ;
                if(!(occ.IsValid))
                          
                {
                    if( this.FireEvents && this.InvalidOccurrenceCode_2Event != null )
                    {
                        this.InvalidOccurrenceCode_2Event(this, null);
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

        public InvalidOccuranceCode_2()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

