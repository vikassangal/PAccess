using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for HospitalServicePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class HospitalServicePreferred : LeafRule
    {
         #region Events

        public event EventHandler HospitalServicePreferredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            HospitalServicePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            HospitalServicePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            HospitalServicePreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            
            if( ((Account)context).HospitalService == null
                || ((Account)context).HospitalService.Code == null
                || ((Account)context).HospitalService.Code.Trim() == string.Empty)
            {
                if ( FireEvents && HospitalServicePreferredEvent != null )
                {
                    HospitalServicePreferredEvent( this, null );
                }
            
                return false;
            }
            return true;
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

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
