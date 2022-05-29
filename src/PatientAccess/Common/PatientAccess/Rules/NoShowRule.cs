using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PreRegRule.
    /// </summary>
    //TODO: Create XML summary comment for PreRegRule
    [Serializable]
    [UsedImplicitly]
    public class NoShowRule : LeafRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            
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
           return true;     
       
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public NoShowRule()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
