using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for TransferTimeFutureTime.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class TransferTimeFutureTime : TimeCannotBeInFutureTime
    {

        #region Events
        
        public event EventHandler TransferTimeFutureTimeEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.TransferTimeFutureTimeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.TransferTimeFutureTimeEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.TransferTimeFutureTimeEvent = null;            
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
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            // pass in the TransferDate as context to the base class       

            base.DateToEvaluate = ((Account)context).TransferDate;

            if( !base.CanBeAppliedTo( context ) )
            {
                if( this.FireEvents && TransferTimeFutureTimeEvent != null )
                {
                    this.TransferTimeFutureTimeEvent(this, new PropertyChangedArgs( this.AssociatedControl ));
                }
                return false;
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
        public TransferTimeFutureTime()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
        
    }
}
