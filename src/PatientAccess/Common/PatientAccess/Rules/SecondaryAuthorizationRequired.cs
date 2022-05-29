using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for SecondaryAuthorizationRequired.
    /// </summary>
    //TODO: Create XML summary comment for SecondaryAuthorizationRequired
    [Serializable]
    [UsedImplicitly]
    public class SecondaryAuthorizationRequired : InsuranceVerificationRule
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

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {

            Account anAccount = context as Account;
            Coverage coverage = anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
            if ( coverage.GetType() == typeof( CoverageGroup ) )
            {

                CoverageGroup coverageGroup = (CoverageGroup)coverage;
                if ( ( coverageGroup.Authorization.AuthorizationRequired.Code == string.Empty ) ||
                    ( ( coverageGroup.Authorization.AuthorizationRequired.Code == "Y" ) &&
                    ( coverageGroup.Authorization.AuthorizationNumber == string.Empty ) ) )
                {
                    return true;
                }
            }


            return false;
            
        }
        #endregion

        #region Properties
      
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SecondaryAuthorizationRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

