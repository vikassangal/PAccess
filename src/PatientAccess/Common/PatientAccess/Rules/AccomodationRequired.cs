using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AccomodationRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AccomodationRequired : LeafRule
    {
        #region Events
        
        public event EventHandler AccomodationRequiredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AccomodationRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AccomodationRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AccomodationRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	            
                        
            Account Model = (Account)context;

            if( Model != null
                && Model.KindOfVisit != null
                && Model.KindOfVisit.Code != null
                && Model.KindOfVisit.Code == VisitType.INPATIENT
                && Model.Location != null 
                && Model.Location.Bed != null
                && Model.Location.DisplayString != string.Empty
                && (
                   Model.Location.Bed.Accomodation              == null
                || Model.Location.Bed.Accomodation.Code.Trim()  == string.Empty ) )
            {
                if( this.FireEvents && AccomodationRequiredEvent != null )
                {
                    this.AccomodationRequiredEvent(this, null);
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

        public AccomodationRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
