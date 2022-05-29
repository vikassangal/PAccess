using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PatientTypeRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PatientTypeRequired : LeafRule
    {
        #region Events
        
        public event EventHandler PatientTypeRequiredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PatientTypeRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PatientTypeRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PatientTypeRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
                        
            if( ((Account)context).KindOfVisit == null 
                || ((Account)context).KindOfVisit.Code == null
                || ((Account)context).KindOfVisit.Code.Trim() == string.Empty )
            {
                if( this.FireEvents && PatientTypeRequiredEvent != null )
                {
                    this.PatientTypeRequiredEvent(this, null);
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

        public PatientTypeRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
