using System;
using System.Collections;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for OccurrenceCode3DateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class OccurrenceCode3DateRequired : LeafRule
    {
        #region Events

        public event EventHandler OccurrenceCode3DateRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.OccurrenceCode3DateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.OccurrenceCode3DateRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.OccurrenceCode3DateRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) ||
                ((ArrayList)((Account)context).OccurrenceCodes).Count < 3 )
            {                
                return true;
            }             
            
            ArrayList occs = (ArrayList)((Account)context).OccurrenceCodes;
            OccurrenceCode occ = (OccurrenceCode)occs[2];

            if( occ.Code.Trim() != String.Empty &&
                occ.OccurrenceDate == DateTime.MinValue )
            {
                if( this.FireEvents && OccurrenceCode3DateRequiredEvent != null )
                {
                    this.OccurrenceCode3DateRequiredEvent(this, null);
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

        public OccurrenceCode3DateRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
