using System;
using System.Collections;
using PatientAccess.Actions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class PatientLiabilityTests
    {
        #region Constants

     
        private DateTime expected_Dob = new DateTime(1953, 2, 7 );
        private readonly FinancialClass FC_14 = new FinancialClass(ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, "MEDICARE", "14");
        
        #endregion

        #region SetUp and TearDown PatientLiabilityTests
        [TestFixtureSetUp()]
        public static void SetUpRulesTests()
        {

            i_Facility = i_facilityBroker.FacilityWith(900);
            i_accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            patient.Oid = 46;
            patient.Facility = i_Facility;
            patient.MedicalRecordNumber = 785213;
            accounts = i_accountBroker.AccountsFor( patient );
           
        }

        [TestFixtureTearDown()]
        public static void TearDownRulesTests()
        {
        }
        #endregion

        #region Test Methods
        
        [Test()]
        public void TestDeterminePatientLiabilityRule()
        {
            
            foreach( AccountProxy ap in accounts )
            {
                Account anAccount = ap.AsAccount();
                anAccount.FinancialClass = new FinancialClass(299,ReferenceValue.NEW_VERSION,"MEDICADE","40");
                anAccount.Payment = new Payment();
                Coverage coverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                coverage.NoLiability = false ;
                coverage.CoPay = 10.0m;
                coverage.Deductible = 100.0m;
                PatientLiabilityToBeDetermined determinePatientLiability = new PatientLiabilityToBeDetermined();
                if(determinePatientLiability.CanBeAppliedTo(anAccount))
                {
                    DeterminePatientLiability determinePatientLiabilityAction = new DeterminePatientLiability();
                    determinePatientLiabilityAction.Context = ap;
                    determinePatientLiabilityAction.Execute();
                }
            }
        }
          
        [Test()]
        public void TestRedeterminePatientLiabilityRule()
        {
            foreach( AccountProxy ap in accounts )
            {
                Account anAccount = ap.AsAccount();
                anAccount.FinancialClass = new FinancialClass(299,ReferenceValue.NEW_VERSION,"MEDICADE","40");
                PatientLiabilityToBeRedetermined redeterminePatientLiability = new PatientLiabilityToBeRedetermined();
                if(redeterminePatientLiability.CanBeAppliedTo(anAccount))
                {
                    RedeterminePatientLiability redeterminePatientLiabilityAction = new RedeterminePatientLiability();
                    redeterminePatientLiabilityAction.Context = ap;
                    redeterminePatientLiabilityAction.Execute();
                }
            }
        }
        [Test()]
        public void TestDeterminePatientLiabilityForFinancialClass14()
        {
            var account = GetAccount();
            var determinePatientLiability = new PatientLiabilityToBeDetermined();
            var canBeApplied = determinePatientLiability.CanBeAppliedTo(account);
            Assert.IsFalse(canBeApplied);
        }

  
     [Test()]
        public void TestRedeterminePatientLiabilityForFinancialClass14()
        {
                var account = GetAccount();
                account.TotalPaid = 100;
                var redeterminePatientLiability = new PatientLiabilityToBeRedetermined();
                var canBeApplied = redeterminePatientLiability.CanBeAppliedTo(account);
                Assert.IsFalse(canBeApplied);
           }

        [Test()]
        public void TestPatientLiabilityRule()
        {
            foreach( AccountProxy ap in accounts )
            {
              
                Account anAccount = ap.AsAccount();
                anAccount.FinancialClass = new FinancialClass(299,ReferenceValue.NEW_VERSION,"MEDICADE","40");
                PatientLiabilityRule rule = new PatientLiabilityRule();

                rule.CanBeAppliedTo(anAccount);
               
            }
        }

        
        #endregion

        #region Support Methods
        private Account GetAccount()
        {
            var anAccount = new Account
                {
                    KindOfVisit = VisitType.Inpatient,
                    HospitalService = {Code = HospitalService.LEASED_BEDS},
                    FinancialClass = FC_14,
                    Payment = new Payment(),
                    Insurance = new Insurance(),
                    TotalCurrentAmtDue = 0
                };
            var coverage = new GovernmentOtherCoverage
                {
                    CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "Primary"),
                    BenefitsVerified = {Code = YesNotApplicableFlag.CODE_YES},
                    NoLiability = false,
                    CoPay = 0.0m,
                    Deductible = 0.0m
                };
            anAccount.Insurance.AddCoverage(coverage); 
            return anAccount;
        }
        #endregion

        #region Data Elements
        
        private static IAccountBroker i_accountBroker = null;
        private static IFacilityBroker i_facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        private static  Facility i_Facility;
        private static Patient patient = new Patient();
        private static ArrayList accounts  = new ArrayList();
                 
        #endregion

    }
}