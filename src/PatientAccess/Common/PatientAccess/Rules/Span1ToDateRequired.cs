using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for Span1ToDateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class Span1ToDateRequired : LeafRule
    {
        #region Events

        public event EventHandler Span1ToDateRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.Span1ToDateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.Span1ToDateRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.Span1ToDateRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) ||
                ((Account)context).OccurrenceSpans.Count == 0 )
            {                
                return true;
            }          
            
            OccurrenceSpan span = (OccurrenceSpan)((Account)context).OccurrenceSpans[0];

            if( span != null &&
                span.SpanCode != null &&
                span.SpanCode.Code.Trim() != string.Empty &&
                span.ToDate == DateTime.MinValue )
            {
                if( this.FireEvents && Span1ToDateRequiredEvent != null )
                {
                    this.Span1ToDateRequiredEvent(this, null);
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

        public Span1ToDateRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
