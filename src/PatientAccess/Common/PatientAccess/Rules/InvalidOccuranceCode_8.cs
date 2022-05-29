using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidOccuranceCode_8 : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidOccurrenceCode_8Event;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidOccurrenceCode_8Event += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidOccurrenceCode_8Event -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidOccurrenceCode_8Event = null;   
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
                &&  (account.OccurrenceCodes.Count > 7 ))
            {
                OccurrenceCode occ =  (OccurrenceCode)account.OccurrenceCodes[7] ;
                if(!(occ.IsValid))
                          
                {
                    if( this.FireEvents && this.InvalidOccurrenceCode_8Event != null )
                    {
                        this.InvalidOccurrenceCode_8Event(this, null);
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

        public InvalidOccuranceCode_8()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

