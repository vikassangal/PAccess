using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AuthorizationRequired.
    /// </summary>
    //TODO: Create XML summary comment for AuthorizationRequired
    [Serializable]
    [UsedImplicitly]
    public class AdmitDateLaterThanSecondaryAuthExpDate : InsuranceVerificationRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override void UnregisterHandlers()
        {

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
                context.GetType() != typeof(Account))
            {
                return true;
            }

            bool preReqsApply = base.CanBeAppliedTo(context);
            if (!preReqsApply)
            {
                return true;
            }


            Account anAccount = context as Account;
            Coverage coverage = anAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID);
            if ( coverage != null )
            {
                if ( coverage.GetType().IsSubclassOf( typeof( CoverageGroup) ))
                {
                    CoverageGroup coverageGroup = coverage as CoverageGroup;
                    if ( coverageGroup != null )
                    {
                        if ( coverageGroup.Authorization.ExpirationDate != DateTime.MinValue && coverageGroup.Authorization.EffectiveDate == DateTime.MinValue)
                        {
                            if ( anAccount.AdmitDate > coverageGroup.Authorization.ExpirationDate )
                            {
                                return false;
                            }
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
        public AdmitDateLaterThanSecondaryAuthExpDate()
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        #endregion
    }
}

