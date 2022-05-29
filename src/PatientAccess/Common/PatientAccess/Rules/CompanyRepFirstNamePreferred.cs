using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for CompanyRepFirstNamePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class CompanyRepFirstNamePreferred : InsuranceVerificationRule
    {
        #region Event Handlers
        public event EventHandler CompanyRepFirstNamePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            CompanyRepFirstNamePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            CompanyRepFirstNamePreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.CompanyRepFirstNamePreferredEvent = null;
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
            if ( aCoverage == null)
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

                            if ( coverageGroup.Authorization.AuthorizationNumber.Trim().Length > 0 && coverageGroup.Authorization.NameOfCompanyRepresentative.FirstName.Length == 0 ||
                                 coverageGroup.Authorization.NameOfCompanyRepresentative.FirstName == null )
                            {
                                if ( this.FireEvents && CompanyRepFirstNamePreferredEvent != null )
                                {
                                    CompanyRepFirstNamePreferredEvent(this, null);
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
        public CompanyRepFirstNamePreferred()
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        #endregion
    }
}
