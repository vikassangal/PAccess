using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for WorkersCompPolicyNumberRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompPolicyNumberRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler WorkersCompPolicyNumberRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            WorkersCompPolicyNumberRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            WorkersCompPolicyNumberRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.WorkersCompPolicyNumberRequiredEvent = null;  
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
            if( context.GetType() != typeof( WorkersCompensationCoverage ) )
            {
                return true;
            }

            WorkersCompensationCoverage aCoverage = context as WorkersCompensationCoverage;

            if( aCoverage != null && aCoverage.PolicyNumber != String.Empty )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && WorkersCompPolicyNumberRequiredEvent != null )
                {
                    WorkersCompPolicyNumberRequiredEvent( this, null );
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
        public WorkersCompPolicyNumberRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
