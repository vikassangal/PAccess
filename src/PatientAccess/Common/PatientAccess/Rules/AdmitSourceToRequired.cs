using System;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitSourceToRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitSourceToRequired : LeafRule
    {
        #region Events

        public event EventHandler AdmitSourceToRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AdmitSourceToRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AdmitSourceToRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmitSourceToRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context.GetType() != typeof( ComboBox ) )
            {                
                return true;
            } 

            ComboBox cboAdmitSource = (ComboBox)context;

            if( cboAdmitSource.Enabled == true && cboAdmitSource.SelectedIndex < 0 )
            {
                if( this.FireEvents && AdmitSourceToRequiredEvent != null )
                {
                    this.AdmitSourceToRequiredEvent(this, null);
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

        public AdmitSourceToRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
