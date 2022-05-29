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
    public class InvalidOccuranceCode_6Change : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidOccurrenceCode_6ChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidOccurrenceCode_6ChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidOccurrenceCode_6ChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidOccurrenceCode_6ChangeEvent = null;   
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

            if( ( account != null ) &&  (account.OccurrenceCodes  != null )
                &&  (account.OccurrenceCodes.Count > 5 ))
            {
                OccurrenceCode occ =  (OccurrenceCode)account.OccurrenceCodes[5] ;
                if(!(occ.IsValid))
                          
                {
                    if( this.FireEvents && this.InvalidOccurrenceCode_6ChangeEvent != null )
                    {
                        this.InvalidOccurrenceCode_6ChangeEvent(this, null);
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

        public InvalidOccuranceCode_6Change()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

