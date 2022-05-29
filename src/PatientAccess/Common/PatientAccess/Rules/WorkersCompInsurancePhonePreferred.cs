using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for WorkersCompInsurancePhonePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompInsurancePhonePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler WorkersCompInsurancePhonePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            WorkersCompInsurancePhonePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            WorkersCompInsurancePhonePreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.WorkersCompInsurancePhonePreferredEvent = null;            
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
            if( context.GetType() != typeof(WorkersCompensationCoverage) )
            {
                return true;
            }

            WorkersCompensationCoverage aCoverage = context as WorkersCompensationCoverage;

            if( aCoverage != null
                && aCoverage.InsurancePhone != null
                && aCoverage.InsurancePhone != String.Empty )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && WorkersCompInsurancePhonePreferredEvent != null )
                {
                    WorkersCompInsurancePhonePreferredEvent( this, null );
                }
                return false;
            }

        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public WorkersCompInsurancePhonePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
