using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InsuranceAuthorizationRequiredPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsuranceAuthorizationRequiredPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler InsuranceAuthorizationRequiredPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InsuranceAuthorizationRequiredPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InsuranceAuthorizationRequiredPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            InsuranceAuthorizationRequiredPreferredEvent = null;
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
            bool result = true;

            if ( context == null )
                return false;

            if ( context.GetType() != typeof( Coverage ) &&
                 context.GetType().BaseType != typeof( Coverage ) &&
                 context.GetType().BaseType.BaseType != typeof( Coverage ) &&
                 context.GetType().BaseType.BaseType.BaseType != typeof( Coverage ) &&
                 context.GetType().BaseType.BaseType.BaseType.BaseType != typeof( Coverage )
               )
            {
                return true;
            }

            Coverage aCoverage = context as Coverage;

            if ( aCoverage != null && aCoverage.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
            {
                CoverageGroup coverageGroup = aCoverage as CoverageGroup;
                if ( coverageGroup != null && 
                     ( coverageGroup.Authorization.AuthorizationRequired == null ||
                       coverageGroup.Authorization.AuthorizationRequired.IsBlank ) )
                {
                    if ( FireEvents && InsuranceAuthorizationRequiredPreferredEvent != null )
                    {
                        InsuranceAuthorizationRequiredPreferredEvent( this, null );
                    }
                    result = false;
                }
            }

            return result;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
