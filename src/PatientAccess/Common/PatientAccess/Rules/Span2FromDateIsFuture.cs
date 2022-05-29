using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for Span2FromDateIsFuture.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class Span2FromDateIsFuture : DateCannotBeFutureDate
    {

        #region Events
        
        public event EventHandler Span2FromDateIsFutureEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.Span2FromDateIsFutureEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.Span2FromDateIsFutureEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.Span2FromDateIsFutureEvent = null;            
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || 
                context.GetType() != typeof( Account ) ||
                ((Account)context).OccurrenceSpans.Count == 0 )
            {                
                return true;
            } 
            
            Account anAccount = (Account)context;

            if( anAccount.OccurrenceSpans[1] != null )
            {                
                OccurrenceSpan occurrenceSpan2 = (OccurrenceSpan)anAccount.OccurrenceSpans[1];
            
                if( occurrenceSpan2.FromDate == DateTime.MinValue )
                {
                    return true;
                }

                base.DateToEvaluate = occurrenceSpan2.FromDate;

                if( !base.CanBeAppliedTo( context ) )
                {
                    if( this.FireEvents && Span2FromDateIsFutureEvent != null )
                    {
                        this.Span2FromDateIsFutureEvent(this, null);
                    }
                    return false;
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
            

        #endregion

        #region Properties
      
        #endregion

        #region Private Methods


        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Span2FromDateIsFuture()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
        
    }
}
