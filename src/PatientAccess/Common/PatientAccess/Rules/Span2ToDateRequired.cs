using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for Span2ToDateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class Span2ToDateRequired : LeafRule
    {
        #region Events

        public event EventHandler Span2ToDateRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.Span2ToDateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.Span2ToDateRequiredEvent -= eventHandler;
            return true;
        }
        
        public override void UnregisterHandlers()
        {   
            this.Span2ToDateRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) ||
                ((Account)context).OccurrenceSpans.Count < 2 )
            {                
                return true;
            } 
            
            OccurrenceSpan span = (OccurrenceSpan)((Account)context).OccurrenceSpans[1];

            if( span != null &&
                span.SpanCode != null &&
                span.SpanCode.Code.Trim() != string.Empty &&
                span.ToDate == DateTime.MinValue )
            {
                if( this.FireEvents && Span2ToDateRequiredEvent != null )
                {
                    this.Span2ToDateRequiredEvent(this, null);
                }
            
                return false;
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

        public Span2ToDateRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
