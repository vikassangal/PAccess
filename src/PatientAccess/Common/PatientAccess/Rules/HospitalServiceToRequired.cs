using System;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for HospitalServiceToRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class HospitalServiceToRequired : LeafRule
    {
        #region Events

        public event EventHandler HospitalServiceToRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.HospitalServiceToRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.HospitalServiceToRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.HospitalServiceToRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context.GetType() != typeof( ComboBox ) )
            {                
                return true;
            } 

            ComboBox cboHSV = (ComboBox)context;

            if( cboHSV.Enabled == true && cboHSV.SelectedIndex <= 0 )
            {
                if( this.FireEvents && HospitalServiceToRequiredEvent != null )
                {
                    this.HospitalServiceToRequiredEvent(this, null);
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

        public HospitalServiceToRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
