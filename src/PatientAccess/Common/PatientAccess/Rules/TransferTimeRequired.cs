using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for TransferTimeRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class TransferTimeRequired : LeafRule
    {
        #region Events

        public event EventHandler TransferTimeRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.TransferTimeRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.TransferTimeRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.TransferTimeRequiredEvent = null;            
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            
            DateTime dtTransferDate = ((Account)context).TransferDate;

            if( dtTransferDate.TimeOfDay == TimeSpan.Zero
                || dtTransferDate.TimeOfDay == TimeSpan.MinValue )
            {
                if( this.FireEvents && TransferTimeRequiredEvent != null )
                {
                    this.TransferTimeRequiredEvent(this, null);
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

        public TransferTimeRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
