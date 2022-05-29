using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for SpanCode_1 Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidSpanCode_1 : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidSpanCode_1Event;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidSpanCode_1Event += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidSpanCode_1Event -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidSpanCode_1Event = null;   
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

            if( ( account != null ) &&  (account.OccurrenceSpans  != null )
                &&  (account.OccurrenceSpans.Count > 0))
            {
                OccurrenceSpan  occSpan     =  (OccurrenceSpan)account.OccurrenceSpans[0] ;
                SpanCode        spanCode    = null;

                if( occSpan != null
                    && occSpan.SpanCode != null)
                {
                    spanCode = occSpan.SpanCode ;
                }
                
                if( spanCode != null 
                    && !spanCode.IsValid )
                          
                {
                    if( this.FireEvents && InvalidSpanCode_1Event != null )
                    {
                        this.InvalidSpanCode_1Event(this, null);
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

        public InvalidSpanCode_1()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


