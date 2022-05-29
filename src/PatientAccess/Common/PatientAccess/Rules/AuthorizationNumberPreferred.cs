using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AuthorizationNumberPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AuthorizationNumberPreferred : InsuranceVerificationRule
    {
        #region Event Handlers
        public event EventHandler AuthorizationNumberPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            AuthorizationNumberPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            AuthorizationNumberPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.AuthorizationNumberPreferredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo(object context)
        {
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null ||
                context.GetType() != typeof(Coverage) &&
                context.GetType().BaseType != typeof(Coverage) &&
                context.GetType().BaseType.BaseType != typeof(Coverage) &&
                context.GetType().BaseType.BaseType.BaseType != typeof(Coverage) &&
                context.GetType().BaseType.BaseType.BaseType.BaseType != typeof(Coverage)
                )
            {
                return true;
            }
            Coverage aCoverage = context as Coverage;
            if (aCoverage == null)
            {
                return false;
            }
            if (aCoverage.GetType() != typeof(GovernmentMedicareCoverage) &&
                    aCoverage.GetType() != typeof(SelfPayCoverage))
            {
                CoverageGroup coverageGroup = this.ExtractCoverageGroupFrom(aCoverage);
                if (coverageGroup != null)
                {
                    if (coverageGroup.Authorization.AuthorizationNumber.Length == 0 ||
                         coverageGroup.Authorization.AuthorizationNumber == null)
                    {
                        if (this.FireEvents && AuthorizationNumberPreferredEvent != null)
                        {
                            AuthorizationNumberPreferredEvent(this, null);
                        }
                        return false;
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
        public AuthorizationNumberPreferred()
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        #endregion
    }
}
