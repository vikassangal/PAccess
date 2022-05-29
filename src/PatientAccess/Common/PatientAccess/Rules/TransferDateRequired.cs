using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for TransferDateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class TransferDateRequired : LeafRule
    {
        #region Events

        public event EventHandler TransferDateRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.TransferDateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.TransferDateRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.TransferDateRequiredEvent = null;            
        }

        public override bool CanBeAppliedTo(object context)
        {  
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            
            if( ((Account)context).TransferDate == DateTime.MinValue )                
            {
                if( this.FireEvents && TransferDateRequiredEvent != null )
                {
                    this.TransferDateRequiredEvent(this, null);
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

        public TransferDateRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
