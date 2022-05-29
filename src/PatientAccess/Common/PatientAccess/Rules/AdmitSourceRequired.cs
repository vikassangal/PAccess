using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitSourceRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitSourceRequired : LeafRule
    {

        #region Events
        
        public event EventHandler AdmitSourceRequiredEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AdmitSourceRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AdmitSourceRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmitSourceRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	
            
            if( ((Account)context).AdmitSource == null 
                || ((Account)context).AdmitSource.Code == null
                || ((Account)context).AdmitSource.Code.Trim() == string.Empty)
            {
                if( this.FireEvents && AdmitSourceRequiredEvent != null)
                {
                    this.AdmitSourceRequiredEvent(this, null);
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
        public AdmitSourceRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
