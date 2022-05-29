using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PlanIDForFindAPlanPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PlanIDForFindAPlanPreferred : LeafRule
    {
        #region Events

        public event EventHandler PlanIDForFindAPlanPreferredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PlanIDForFindAPlanPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PlanIDForFindAPlanPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PlanIDForFindAPlanPreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            //OccurrenceSpan span = (OccurrenceSpan)((Account)context).OccurrenceSpans[0];

            if( true )//span != null )//&&
                //span.FromDate == DateTime.MinValue )
            {
                if( this.FireEvents && PlanIDForFindAPlanPreferredEvent != null )
                {
                    this.PlanIDForFindAPlanPreferredEvent(this, null);
                }
            
                return false;
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

        public PlanIDForFindAPlanPreferred()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
