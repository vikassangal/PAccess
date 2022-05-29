using System;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AccommodationToInPat1Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AccommodationToInPat1Required : LeafRule
    {
        #region Events

        public event EventHandler AccommodationToInPat1RequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AccommodationToInPat1RequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AccommodationToInPat1RequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AccommodationToInPat1RequiredEvent = null;  
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
                if( this.FireEvents && AccommodationToInPat1RequiredEvent != null )
                {
                    this.AccommodationToInPat1RequiredEvent(this, null);
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

        public AccommodationToInPat1Required()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
