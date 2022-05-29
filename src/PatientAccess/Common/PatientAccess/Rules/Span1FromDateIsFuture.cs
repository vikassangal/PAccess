using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for Span1FromDateIsFuture.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class Span1FromDateIsFuture : DateCannotBeFutureDate
    {

        #region Events
        
        public event EventHandler Span1FromDateIsFutureEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.Span1FromDateIsFutureEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.Span1FromDateIsFutureEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.Span1FromDateIsFutureEvent = null;   
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

            if( anAccount.OccurrenceSpans[0] != null )
            {                
                OccurrenceSpan occurrenceSpan1 = (OccurrenceSpan)anAccount.OccurrenceSpans[0];
            
                if( occurrenceSpan1.FromDate == DateTime.MinValue )
                {
                    return true;
                }

                base.DateToEvaluate = occurrenceSpan1.FromDate;

                if( !base.CanBeAppliedTo( context ) )
                {
                    if( this.FireEvents && Span1FromDateIsFutureEvent != null )
                    {
                        this.Span1FromDateIsFutureEvent(this, null);
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
        public Span1FromDateIsFuture()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
        
    }
}
