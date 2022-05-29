using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ServicesAuthorizedPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ServicesAuthorizedPreferred : InsuranceVerificationRule
    {
        #region Event Handlers
        public event EventHandler ServicesAuthorizedPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            ServicesAuthorizedPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            ServicesAuthorizedPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            ServicesAuthorizedPreferredEvent = null;
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
            if ( context == null ||
                context.GetType() != typeof( Coverage ) &&
                context.GetType().BaseType != typeof( Coverage ) &&
                context.GetType().BaseType.BaseType != typeof( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType != typeof( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType.BaseType != typeof( Coverage )
                )
            {
                return true;
            }
            Coverage aCoverage = context as Coverage;
            if ( aCoverage == null )
            {
                return false;
            }
            if ( aCoverage.GetType() != typeof( GovernmentMedicareCoverage ) &&
                    aCoverage.GetType() != typeof( SelfPayCoverage ) )
            {
                CoverageGroup coverageGroup = ExtractCoverageGroupFrom( aCoverage );
                if ( coverageGroup != null )
                {
                    if ( coverageGroup.Authorization != null &&
                         coverageGroup.Authorization.AuthorizationNumber.Trim().Length > 0 )
                    {
                        if ( coverageGroup.Authorization.ServicesAuthorized.Trim().Length == 0 ||
                             coverageGroup.Authorization.ServicesAuthorized == null )
                        {
                            if ( FireEvents && ServicesAuthorizedPreferredEvent != null )
                            {
                                ServicesAuthorizedPreferredEvent( this, null );
                            }
                            return false;
                        }
                       
                    }

                    
                }
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
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        #endregion
    }
}
