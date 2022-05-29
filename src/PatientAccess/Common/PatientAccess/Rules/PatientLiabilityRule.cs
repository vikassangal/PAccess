using System;
using System.Collections;
using Extensions.UI.Builder;
using PatientAccess.BrokerProxies;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PatientLiabilityRule.
    /// </summary>
    //TODO: Create XML summary comment for PatientLiabilityRule
    [Serializable]
    [UsedImplicitly]
    public class PatientLiabilityRule : LeafRule
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
            long primaryID = CoverageOrder.PRIMARY_OID ;

            Coverage coverage = null;

            if( anAccount != null && anAccount.Insurance != null )
            {
                coverage = anAccount.Insurance.CoverageFor(primaryID) ;
            }
            
            if(coverage != null)
            {
                 
                string  benefitsVerified = string.Empty ;
                bool noLiability = false ;
                if( coverage.BenefitsVerified !=  null 
                    && coverage.BenefitsVerified.Code != null)
                {
                    benefitsVerified = coverage.BenefitsVerified.Code ;
                    noLiability  = coverage.NoLiability;
                }
                string HospitalSerViceCode = string.Empty;

                if(    anAccount.HospitalService != null &&
                       anAccount.HospitalService.Code != null                   
                    )
                {
                    HospitalSerViceCode =  anAccount.HospitalService.Code.Trim()  ;
                }
             
                if(CheckPatientType(anAccount.KindOfVisit))
                {
                    if(IsInsured(anAccount.FinancialClass))   // Financial class is Insured
                    {
                        if(( benefitsVerified == YesNotApplicableFlag.CODE_YES) &&
                            (HospitalSerViceCode != "SP" || HospitalSerViceCode != "CV") &&
                            (!noLiability ||
                            (anAccount.TotalCurrentAmtDue > 0))
                            )
                        {
                            return true;
                        }
                                              
                    }
                    else  
                        if(IsUninsured(anAccount.FinancialClass))// Financial class is UnInsured
                    {
                        if(!noLiability ||
                            (anAccount.BalanceDue > 0))
                        {
                            return true;
                        }
                 
                    }
                }
            }

                                    
            return false;
     
        }

        public  static bool IsUninsured( FinancialClass aFinancialClass )
        {
            ArrayList unInsuredFinancialClasses = new ArrayList();
            unInsuredFinancialClasses.Add( "70" );
            unInsuredFinancialClasses.Add( "72" );
            unInsuredFinancialClasses.Add( "73" );
            unInsuredFinancialClasses.Add( "96" );

            return unInsuredFinancialClasses.Contains( aFinancialClass.Code );
        }
        public static bool IsInsured( FinancialClass aFinancialClass )
        {
            ArrayList insuredFinancialClasses = new ArrayList();
            insuredFinancialClasses.Add( "02" );
            insuredFinancialClasses.Add( "04" );
            insuredFinancialClasses.Add( "05" );
            insuredFinancialClasses.Add( "08" );
            insuredFinancialClasses.Add( "13" );
            insuredFinancialClasses.Add( "14" );
            insuredFinancialClasses.Add( "16" );
            insuredFinancialClasses.Add( "17" );
            insuredFinancialClasses.Add( "18" );

            insuredFinancialClasses.Add( "20" );
            insuredFinancialClasses.Add( "22" );
            insuredFinancialClasses.Add( "23" );
            insuredFinancialClasses.Add( "25" );
            insuredFinancialClasses.Add( "26" );
            insuredFinancialClasses.Add( "29" );
            insuredFinancialClasses.Add( "40" );
            insuredFinancialClasses.Add( "44" );

            
            insuredFinancialClasses.Add( "48" );
            insuredFinancialClasses.Add( "50" );
            insuredFinancialClasses.Add( "54" );
            insuredFinancialClasses.Add( "55" );
            insuredFinancialClasses.Add( "80" );
            insuredFinancialClasses.Add( "81" );
            insuredFinancialClasses.Add( "82" );
            insuredFinancialClasses.Add( "84" );
            insuredFinancialClasses.Add( "85" );
            insuredFinancialClasses.Add( "87" );
            return insuredFinancialClasses.Contains( aFinancialClass.Code );
        }

        public static bool CheckPatientType( VisitType aKindOfVisit )
        {
            if(( aKindOfVisit.Code == PT_0 )||
                ( aKindOfVisit.Code == PT_1 )||
                ( aKindOfVisit.Code == PT_2 )||
                ( aKindOfVisit.Code == PT_3)||
                ( aKindOfVisit.Code == PT_4 )
                )

            {
                return true;
            }
            else
            {
                return false;
            }
          
        }
        public decimal getBalanceDue(Account anAccount)
        {
            Coverage coverage =  anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
            if(coverage != null)
            {
                return coverage.Deductible + coverage.CoPay ;
            }
            return 0;
        }

        public void CheckFinancialClassForUnInsured( Account anAccount )
        {
            if ( anAccount.FinancialClass.Code == SELFPAY_UNINSURED && ( anAccount.TotalPaid > 0 || anAccount.NumberOfMonthlyPayments > 0 ) )
            {
                //IFinancialClassesBroker financialClassesBroker  = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();
                FinancialClassesBrokerProxy financialClassesBroker = new FinancialClassesBrokerProxy();
                anAccount.FinancialClass = financialClassesBroker.FinancialClassWith(anAccount.Facility.Oid,FINANCIALCLASS_UNINSURED );
            }
        }
        #endregion

        #region Properties
      
        #endregion

        #region Private Methods
 
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PatientLiabilityRule()
        {
        }
        #endregion

        #region Data Elements
      
        #endregion

        #region Constants
        private const string      
            PT_0   = "0",
            PT_1   = "1",
            PT_2   = "2",
            PT_3   = "3",
            PT_4   = "4";
        private const string SELFPAY_UNINSURED = "70";
        private const string FINANCIALCLASS_UNINSURED = "73";
        #endregion
    }
}

