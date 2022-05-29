using System;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AccommodationToInPat2Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AccommodationToInPat2Required : LeafRule
    {
        #region Events

        public event EventHandler AccommodationToInPat2RequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AccommodationToInPat2RequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AccommodationToInPat2RequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AccommodationToInPat2RequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( ComboBox ) )
            {                
                return true;
            } 	            
                        
            ComboBox cboACC = (ComboBox)context;

            if( cboACC.Enabled &&
                ( cboACC.SelectedIndex < 0 ||
                ((Accomodation)cboACC.SelectedItem).Code.Trim() == string.Empty )
                )            
            {
                if( this.FireEvents && AccommodationToInPat2RequiredEvent != null )
                {
                    this.AccommodationToInPat2RequiredEvent(this, null);
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

        public AccommodationToInPat2Required()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
