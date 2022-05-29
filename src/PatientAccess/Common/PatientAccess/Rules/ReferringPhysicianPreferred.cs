using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules

{
    /// <summary>
    /// Summary description for ReferringPhysicianPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ReferringPhysicianPreferred : LeafRule
    {
        #region Events

        public event EventHandler ReferringPhysicianPreferredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            ReferringPhysicianPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            ReferringPhysicianPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            ReferringPhysicianPreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            
            var anAccount = ((Account)context);

            if(  anAccount.ReferringPhysician == null ||
                 anAccount.ReferringPhysician.LastName.Trim() + anAccount.ReferringPhysician.LastName.Trim() == string.Empty ) 
            {
                if( FireEvents && ReferringPhysicianPreferredEvent != null )
                {
                    ReferringPhysicianPreferredEvent(this, null);
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
