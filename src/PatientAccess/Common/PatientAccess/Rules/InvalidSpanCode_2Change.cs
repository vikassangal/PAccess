using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for SpanCode_2 Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidSpanCode_2Change : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidSpanCode_2ChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidSpanCode_2ChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidSpanCode_2ChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidSpanCode_2ChangeEvent = null;   
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

            if( ( account != null ) &&  (account.OccurrenceSpans  != null )
                &&  (account.OccurrenceSpans.Count > 1))
            {
                OccurrenceSpan  occSpan     =  (OccurrenceSpan)account.OccurrenceSpans[1] ;
                SpanCode        spanCode    = null;

                if( occSpan != null
                    && occSpan.SpanCode != null )
                {
                    spanCode = occSpan.SpanCode ;
                }
                
                if( spanCode != null 
                    && !spanCode.IsValid )
                          
                {
                    if( this.FireEvents && InvalidSpanCode_2ChangeEvent != null )
                    {
                        this.InvalidSpanCode_2ChangeEvent(this, null);
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

        public InvalidSpanCode_2Change()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


