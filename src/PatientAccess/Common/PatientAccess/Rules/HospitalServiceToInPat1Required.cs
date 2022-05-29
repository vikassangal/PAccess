using System;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for HospitalServiceToInPat1Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class HospitalServiceToInPat1Required : LeafRule
    {
        #region Events

        public event EventHandler HospitalServiceToInPat1RequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.HospitalServiceToInPat1RequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.HospitalServiceToInPat1RequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.HospitalServiceToInPat1RequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( ComboBox ) )
            {                
                return true;
            } 	            
                        
            ComboBox cboHsv = (ComboBox)context;

            if( cboHsv.SelectedIndex < 0 ||
                ((HospitalService)cboHsv.SelectedItem).Code.Trim() == string.Empty )          
            {
                if( this.FireEvents && HospitalServiceToInPat1RequiredEvent != null )
                {
                    this.HospitalServiceToInPat1RequiredEvent(this, null);
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

        public HospitalServiceToInPat1Required()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
