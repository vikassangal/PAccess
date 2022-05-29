using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicaidIssueDateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicaidIssueDateRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicaidIssueDateRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicaidIssueDateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicaidIssueDateRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicaidIssueDateRequiredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            bool blnRC = true;

            if( context == null || context.GetType() != typeof( GovernmentMedicaidCoverage ) )
            {
                return true;
            }
            GovernmentMedicaidCoverage aCoverage = context as GovernmentMedicaidCoverage;
            if( aCoverage == null )
            {
                return false;
            }
            Facility fac = null;
            if( this.AssociatedControl == null )
            {
                if( aCoverage.Account != null )
                {
                    fac = aCoverage.Account.Facility;
                }                    
            }
            else
            {
                fac = (Facility)this.AssociatedControl;
            }
            
            if( fac == null )
            {
                return true;
            }

            if( !blnRC
                || (    aCoverage.IssueDate == DateTime.MinValue 
                &&  fac.MedicaidIssueDateRequired))
            {
                if( this.FireEvents && MedicaidIssueDateRequiredEvent != null )
                {
                    MedicaidIssueDateRequiredEvent( this, null );
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MedicaidIssueDateRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
