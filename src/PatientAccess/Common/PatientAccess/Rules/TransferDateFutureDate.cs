using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for TransferDateFutureDate.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class TransferDateFutureDate : DateCannotBeFutureDate
    {

        #region Events
        
        public event EventHandler TransferDateFutureDateEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.TransferDateFutureDateEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.TransferDateFutureDateEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.TransferDateFutureDateEvent = null;   
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
                if( this.FireEvents && TransferDateFutureDateEvent != null )
                {
                    this.TransferDateFutureDateEvent(this, null);
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
        public TransferDateFutureDate()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
        
    }
}
