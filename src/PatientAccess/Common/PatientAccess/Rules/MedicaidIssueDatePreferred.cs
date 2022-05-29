using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicaidIssueDatePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicaidIssueDatePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicaidIssueDatePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicaidIssueDatePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicaidIssueDatePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicaidIssueDatePreferredEvent = null;   
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
                if( this.FireEvents && MedicaidIssueDatePreferredEvent != null )
                {
                    MedicaidIssueDatePreferredEvent( this, null );
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
        public MedicaidIssueDatePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
