using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ChiefComplaintPreferred.
    /// </summary>
    //TODO: Create XML summary comment for ChiefComplaintPreferred
    [Serializable]
    [UsedImplicitly]
    public class ChiefComplaintPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler ChiefComplaintPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.ChiefComplaintPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.ChiefComplaintPreferredEvent -= eventHandler;
            return true;
        }
                        
        public override void UnregisterHandlers()
        {
            this.ChiefComplaintPreferredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 			
            
            if( ((Account)context).Diagnosis.ChiefComplaint == null 
                || ((Account)context).Diagnosis.ChiefComplaint.Trim() == string.Empty )
            {
                if( this.FireEvents && ChiefComplaintPreferredEvent != null )
                {
                    this.ChiefComplaintPreferredEvent(this, null);
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
        public ChiefComplaintPreferred()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
