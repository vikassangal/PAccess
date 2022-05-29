using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicaidEligibilityDatePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicaidEligibilityDatePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicaidEligibilityDatePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicaidEligibilityDatePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicaidEligibilityDatePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicaidEligibilityDatePreferredEvent = null;   
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
            if( context == null )
            {
                return true;
            }
            if( context.GetType() != typeof( GovernmentMedicaidCoverage ) )
            {
                return true;
            }
            GovernmentMedicaidCoverage aCoverage = context as GovernmentMedicaidCoverage;
            if( aCoverage == null )
            {
                return false;
            }
            if( aCoverage.EligibilityDate == DateTime.MinValue )
            {
                if( this.FireEvents && MedicaidEligibilityDatePreferredEvent != null )
                {
                    MedicaidEligibilityDatePreferredEvent( this, null );
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
        public MedicaidEligibilityDatePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
