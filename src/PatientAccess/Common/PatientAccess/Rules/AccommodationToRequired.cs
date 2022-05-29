using System;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AccommodationToRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AccommodationToRequired : LeafRule
    {
        #region Events

        public event EventHandler AccommodationToRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AccommodationToRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AccommodationToRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AccommodationToRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context.GetType() != typeof( ComboBox ) )
            {                
                return true;
            } 

            ComboBox cboACC = (ComboBox)context;

            if( cboACC.Enabled == true && cboACC.SelectedIndex <= 0 )
            {
                if( this.FireEvents && AccommodationToRequiredEvent != null )
                {
                    this.AccommodationToRequiredEvent(this, null);
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

        public AccommodationToRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
