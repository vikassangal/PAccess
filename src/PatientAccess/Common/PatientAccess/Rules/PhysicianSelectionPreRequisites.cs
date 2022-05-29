using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PhysicianSelectionPreRequisites.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PhysicianSelectionPreRequisites : LeafRule
    {
        #region Events

        public event EventHandler PhysicianSelectionPreRequisitesEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PhysicianSelectionPreRequisitesEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PhysicianSelectionPreRequisitesEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PhysicianSelectionPreRequisitesEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	            
                       
            Account anAccount = ((Account)context);

            if ( anAccount.Activity != null
                && (anAccount.Activity.GetType() == typeof(TransferOutToInActivity) ||
                anAccount.Activity.GetType() == typeof(TransferERToOutpatientActivity)||
                     anAccount.Activity.GetType() == typeof(TransferOutpatientToERActivity)))
            {
                if( anAccount == null
                    || anAccount.TransferDate == DateTime.MinValue
                    || anAccount.KindOfVisit == null
                    || anAccount.KindOfVisit.Code.Trim() == string.Empty
                    || anAccount.HospitalService == null
                    || anAccount.HospitalService.Code.Trim() == string.Empty)
                {
                    if( this.FireEvents && PhysicianSelectionPreRequisitesEvent != null )
                    {
                        this.PhysicianSelectionPreRequisitesEvent(this, null);
                    }
            
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if( anAccount == null
                    || anAccount.AdmitDate.Date == DateTime.MinValue.Date
                    || anAccount.KindOfVisit == null
                    || anAccount.KindOfVisit.Code.Trim() == string.Empty
                    || anAccount.HospitalService == null
                    || anAccount.HospitalService.Code.Trim() == string.Empty)
                {
                    if( this.FireEvents && PhysicianSelectionPreRequisitesEvent != null )
                    {
                        this.PhysicianSelectionPreRequisitesEvent(this, null);
                    }
            
                    return false;
                }
                else
                {
                    return true;
                }
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

        public PhysicianSelectionPreRequisites()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
