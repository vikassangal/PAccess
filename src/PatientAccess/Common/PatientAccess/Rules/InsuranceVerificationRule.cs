using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{

    /// <summary>
    /// Summary description for InsuranceVerificationRule.
    /// </summary>
    //TODO: Create XML summary comment for InsuranceVerificationRule
    [Serializable]
    [UsedImplicitly]
    public class InsuranceVerificationRule : LeafRule
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

        private static bool InsuranceFinancialClassSelected( FinancialClass aFinancialClass )
        {

            if( aFinancialClass == null || aFinancialClass.Code == null )
            {
                return false;
            }

            if((aFinancialClass.Code == FC_03) ||
                (aFinancialClass.Code == FC_37) ||
                (aFinancialClass.Code == FC_52) ||
                (aFinancialClass.Code == FC_60) ||
                (aFinancialClass.Code == FC_98) 
                )
            {
                return false;
            }
            else
            {
                return true;
            }
           
        }
        public override bool ShouldStopProcessing()
        {
            return true;
        }
        
        public override void ApplyTo( object context )
        {
        }




        /// <summary>
        /// Determines whether this instance [can be applied to] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can be applied to] the specified context; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanBeAppliedTo( object context )
        {

            Account anAccount = null;                        
            Coverage coverage = null;
            bool canBeApplied = false;
           

            // Return immediately if the context does not apply to this rule
            if( context == null || !(context is Account) )
            {

                return true;

            }//if

            
            anAccount = context as Account;            
            coverage = GetCoverageFor( anAccount );

            if( coverage != null )
            {

                string planID =
                    GetPlanIdFrom( coverage );

                CoverageGroup coverageGroup =
                    this.ExtractCoverageGroupFrom( coverage );

                string authorizationNumber =
                    String.Empty;

                if( coverageGroup != null && 
                    coverageGroup.Authorization != null )
                {

                    authorizationNumber =
                        coverageGroup.Authorization.AuthorizationNumber;

                    if
                    (
                        ( !anAccount.BillHasDropped ) &&
                        ( !String.IsNullOrEmpty( planID ) ) &&
                        ( InsuranceFinancialClassSelected( anAccount.FinancialClass ) ) &&
                        ( anAccount.HospitalService == null || 
                          ( anAccount.HospitalService.Code != "SP"  || anAccount.HospitalService.Code != "CV")
                          ) &&
                        ( 
                            ( coverage.BenefitsVerified != null && coverage.BenefitsVerified.IsBlank ) ||  // this should be blank
                            ( coverageGroup.Authorization.AuthorizationRequired != null && coverageGroup.Authorization.AuthorizationRequired.IsYes ) &&
                            ( authorizationNumber == string.Empty ) 
                        )
                    )
                    {

                        canBeApplied = true;

                    }//if

                }//if

            }//if
            else
            {

                canBeApplied = false;

            }//else
                       
            return canBeApplied;
    
        }//method




        #endregion

        #region Properties
      
        #endregion

        #region Private Methods


        /// <summary>
        /// Gets the plan id from.
        /// </summary>
        /// <param name="coverage">The coverage.</param>
        /// <returns></returns>
        private static string GetPlanIdFrom( Coverage coverage )
        {

            string planID = String.Empty;

            if( coverage != null &&
                coverage.InsurancePlan != null &&
                !String.IsNullOrEmpty( coverage.InsurancePlan.PlanID ) )
            {

                planID = coverage.InsurancePlan.PlanID;

            }//if

            return planID;

        }//method


        /// <summary>
        /// Gets the coverage for account
        /// </summary>
        /// <param name="anAccount">An account.</param>
        /// <returns></returns>
        private static Coverage GetCoverageFor( Account anAccount )
        {

            Coverage coverage = null;

            if( anAccount != null && anAccount.Insurance != null )
            {

                coverage =
                    anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );

            }//if

            return coverage;

        }//method


        /// <summary>
        /// Extracts the coverage group from.
        /// </summary>
        /// <param name="coverage">The coverage.</param>
        /// <returns></returns>
        protected CoverageGroup ExtractCoverageGroupFrom( Coverage coverage )
        {

            CoverageGroup coverageGroup = null;

            // Attempt to upcast the coverage object
            coverageGroup = coverage as CoverageGroup;

            return coverageGroup;

        }//method

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsuranceVerificationRule()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        private const string      
            FC_03   = "03",
            FC_37   = "37",
            FC_52   = "52",
            FC_60   = "60",
            FC_98   = "98";
        #endregion

    }//class

}//namespace