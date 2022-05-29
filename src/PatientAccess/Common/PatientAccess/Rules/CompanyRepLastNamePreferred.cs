using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for CompanyRepLastNamePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class CompanyRepLastNamePreferred : InsuranceVerificationRule
    {
        #region Event Handlers
        public event EventHandler CompanyRepLastNamePreferredPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            CompanyRepLastNamePreferredPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            CompanyRepLastNamePreferredPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.CompanyRepLastNamePreferredPreferredEvent = null;
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

            if ( aCoverage.GetType().IsSubclassOf(typeof(CoverageGroup)))
                {
                    CoverageGroup coverageGroup = aCoverage as CoverageGroup;
                    if ( coverageGroup != null )
                    {
                        if ( coverageGroup.Authorization != null )
                        {
                            if ( coverageGroup.Authorization.AuthorizationNumber.Trim().Length > 0 && coverageGroup.Authorization.NameOfCompanyRepresentative.LastName.Length == 0 ||
                                 coverageGroup.Authorization.NameOfCompanyRepresentative.LastName == null )
                            {
                                if ( this.FireEvents && CompanyRepLastNamePreferredPreferredEvent != null )
                                {
                                    CompanyRepLastNamePreferredPreferredEvent(this, null);
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
        public CompanyRepLastNamePreferred()
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        #endregion
    }
}
