using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    //TODO: Create XML summary comment for RefusedToSignCOSForm
    [Serializable]
    [UsedImplicitly]
    public class RefusedToSignCOSForm : PostRegRule
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
            if( context == null )
            {
                return true;
            }

            bool preReqsApply = base.CanBeAppliedTo(context);

            if(preReqsApply)
            {
                if(context.GetType() != typeof(Account))
                {
                    throw new ArgumentException( "Context in the rule is not an Account" );
                }
                
                Account account = (Account)context ;
                
                if
                (
                    ( account.COSSigned != null && account.COSSigned.Code == "N" ) && 
                    ( FinancialClassIsSelfpay(account) ) &&
                    ( PayorCodeIsNotBlueCross(account) ) &&
                    ( account.HospitalService != null && account.HospitalService.Code != "SP" ) &&
                    (!account.BillHasDropped) 
                )                                                               
                {
                    return false;
                }                                                               
            }

            return true;                
        }
//662, WN4, 66A, 66B, 66C, 66D 
        private bool PayorCodeIsNotBlueCross( Account anAccount )    
        {
            bool result = false;
            
            if( anAccount.Insurance != null &&
                anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID) != null &&
                anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).InsurancePlan != null )
            {
                string planID = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).InsurancePlan.PlanID.Trim();
    
                if( planID != null && planID.Length >= 2 )
                {
                    if(( planID.Length >= 3) && 
                        (planID.Substring( 0, 3 )!= "664" ||
                        planID.Substring( 0, 3 )!= "WN4" || 
                        planID.Substring( 0, 3 )!= "66B" ||
                        planID.Substring( 0, 3 )!= "66D" 
                       ))
                    {     
                        result = true;
                    }
                }
            }
        
            return result;       
        }    
        public  bool FinancialClassIsSelfpay( Account account )
        {
            if
            (
                ( account.FinancialClass != null ) &&
                ( account.FinancialClass.Code != "70" ||
                  account.FinancialClass.Code != "71" ||
                  account.FinancialClass.Code != "73" ||
                 account.FinancialClass.Code != "96")
            )
            {
                return true;
            }


            return false;;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RefusedToSignCOSForm()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
