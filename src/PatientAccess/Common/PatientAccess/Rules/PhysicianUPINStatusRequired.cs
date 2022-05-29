using System;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PhysicianUPINStatusRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PhysicianUPINStatusRequired : LeafRule
    {
        #region Events

        public event EventHandler PhysicianUPINStatusRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PhysicianUPINStatusRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PhysicianUPINStatusRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PhysicianUPINStatusRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context.GetType() != typeof( ComboBox ) )
            {                
                return true;
            } 

            ComboBox status = (ComboBox)context;

            if( status.SelectedIndex == -1 )
            {
                if( this.FireEvents && PhysicianUPINStatusRequiredEvent != null )
                {
                    this.PhysicianUPINStatusRequiredEvent(this, null);
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

        public PhysicianUPINStatusRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
