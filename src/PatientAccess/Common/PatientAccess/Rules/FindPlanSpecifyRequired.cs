using System;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for FindPlanSpecifyRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class FindPlanSpecifyRequired : LeafRule
    {
        #region Events

        public event EventHandler FindPlanSpecifyRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.FindPlanSpecifyRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.FindPlanSpecifyRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.FindPlanSpecifyRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context.GetType() != typeof( CheckBox ) )
            {
                return true;
            }              
            
            CheckBox IsOther = (CheckBox)context;

            if( IsOther.Checked ) 
            {
                if( this.FireEvents && FindPlanSpecifyRequiredEvent != null )
                {
                    this.FindPlanSpecifyRequiredEvent(this, null);
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

        public FindPlanSpecifyRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
