using System;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PatientTypeToRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PatientTypeToRequired : LeafRule
    {
        #region Events

        public event EventHandler PatientTypeToRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PatientTypeToRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PatientTypeToRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PatientTypeToRequiredEvent = null;            
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context.GetType() != typeof( ComboBox ) )
            {                
                return true;
            } 

            ComboBox cboPatientType = (ComboBox)context;

            if( cboPatientType.Enabled == true && cboPatientType.SelectedIndex < 0 )
            {
                if( this.FireEvents && PatientTypeToRequiredEvent != null )
                {
                    this.PatientTypeToRequiredEvent(this, null);
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

        public PatientTypeToRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
