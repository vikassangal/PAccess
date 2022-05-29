using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AuthorizationRequiredForSecondary.
    /// </summary>
    //TODO: Create XML summary comment for AuthorizationRequiredForSecondary
    [Serializable]
    [UsedImplicitly]
    public class AuthorizationRequiredForSecondary : InsuranceVerificationRule
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
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	            
            
            bool preReqsApply = base.CanBeAppliedTo(context);
            if(!preReqsApply)
            {
                return true;
            }
               
            string authorizationNumber = string.Empty ;
            Account anAccount = context as Account;
            
            Coverage coverage = anAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID);
            
            if( coverage != null)
            {
                CoverageGroup coverageGroup = this.ExtractCoverageGroupFrom(coverage);
                if (coverageGroup != null)
                {
                    if ( coverageGroup.Authorization.AuthorizationRequired == null
                        || coverageGroup.Authorization.AuthorizationRequired.Code == string.Empty
                        || coverageGroup.Authorization.AuthorizationRequired.Code == YesNoFlag.CODE_BLANK
                        ||(
                        coverageGroup.Authorization.AuthorizationRequired.Code == "Y" 
                        && ( coverageGroup.Authorization.AuthorizationNumber == null
                        || coverageGroup.Authorization.AuthorizationNumber == string.Empty )
                        )
                        )                             
                    {
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
        public AuthorizationRequiredForSecondary()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

