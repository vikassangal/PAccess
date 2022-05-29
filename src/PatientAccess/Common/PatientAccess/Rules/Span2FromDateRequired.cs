using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for Span2FromDateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class Span2FromDateRequired : LeafRule
    {
        #region Events

        public event EventHandler Span2FromDateRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.Span2FromDateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.Span2FromDateRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.Span2FromDateRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) ||
                ((Account)context).OccurrenceSpans.Count <= 1 )
            {                
                return true;
            } 
            
            OccurrenceSpan span = (OccurrenceSpan)((Account)context).OccurrenceSpans[1];

            if( span != null &&
                span.SpanCode != null &&
                span.SpanCode.Code.Trim() != string.Empty &&
                span.FromDate == DateTime.MinValue )
            {
                if( this.FireEvents && Span2FromDateRequiredEvent != null )
                {
                    this.Span2FromDateRequiredEvent(this, null);
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

        public Span2FromDateRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
