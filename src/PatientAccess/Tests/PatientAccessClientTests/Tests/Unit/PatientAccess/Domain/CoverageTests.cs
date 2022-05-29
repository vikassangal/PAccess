using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CoverageTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CoverageTests
        [TestFixtureSetUp()]
        public static void SetUpCoverageTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCoverageTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testCommercialCoverage()
        {
            Account account = new Account();
            account.AccountNumber = 345622;
            account.Facility = new Facility(1L, DateTime.Now, "ACO","ACO");
            CommercialInsurancePlan plan = new CommercialInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "PRIVATE PAY";
            Insured insured = new Insured(); 
            insured.FirstName = "Sam";

            
            Coverage coverage = Coverage.CoverageFor(plan,insured);
            coverage.Account = account ;
            if( typeof(CommercialCoverage)== coverage.GetType() )
            {
                CommercialCoverage commCoverage = (CommercialCoverage)coverage;
                commCoverage.PreCertNumber = 123 ;
                commCoverage.TrackingNumber = "1331332";
            
                coverage.BillingInformation.Address = this.address;
                Assert.AreEqual(
                    345622,
                    commCoverage.Account.AccountNumber
                    );
                Assert.AreEqual(
                    "ACO",
                    commCoverage.Account.Facility.Code
                    );
           
                Assert.AreEqual(
                    0,
                    commCoverage.Oid 
                    );
       
                Assert.AreEqual(
                    "PRIVATE PAY",
                    commCoverage.InsurancePlan.PlanName 
                    );
                Assert.AreEqual(
                    "Sam",
                    commCoverage.Insured.FirstName
                    );
           
                Assert.AreEqual(
                    typeof(CommercialCoverage),
                    coverage.GetType()
                    );
            
                Assert.AreEqual(
                    this.address,
                    commCoverage.BillingInformation.Address 
                    );
       
            }
        }
        [Test()]
        public void testGovernmentMedicaidCoverage()
        {
            
            GovernmentMedicaidInsurancePlan plan = new GovernmentMedicaidInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "Medicaid";
            Insured insured = new Insured(); 
            insured.FirstName = "Sam";
            
            Coverage coverage = Coverage.CoverageFor(plan,insured);
            if (typeof(GovernmentMedicaidCoverage) == coverage.GetType())
            {
                GovernmentMedicaidCoverage govtCoverage = (GovernmentMedicaidCoverage) coverage;
                govtCoverage.TrackingNumber = "1331332";
            }

            coverage.BillingInformation.Address = this.address;

            Assert.AreEqual(
                0,
                coverage.Oid 
                );
       
            Assert.AreEqual(
                "Medicaid",
                coverage.InsurancePlan.PlanName 
                );
            Assert.AreEqual(
                "Sam",
                coverage.Insured.FirstName
                );

            Assert.AreEqual(
                typeof(GovernmentMedicaidCoverage),
                coverage.GetType()
            );
            Assert.AreEqual(
                ((GovernmentMedicaidCoverage) coverage).TrackingNumber,
                "1331332");
            Assert.AreEqual(
                this.address,
                coverage.BillingInformation.Address
            );

        }
        [Test()]
        public void TestGovernmentMedicareCoverage()
        {
            
            GovernmentMedicareInsurancePlan plan = new GovernmentMedicareInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "Medicare";
            Insured insured = new Insured(); 
            insured.FirstName = "Sam";
            
            Coverage coverage = Coverage.CoverageFor(plan,insured);
            coverage.BillingInformation.Address = this.address;
       
            Assert.AreEqual(
                0,
                coverage.Oid 
                );
       
            Assert.AreEqual(
                "Medicare",
                coverage.InsurancePlan.PlanName 
                );
            Assert.AreEqual(
                "Sam",
                coverage.Insured.FirstName
                );
           
            Assert.AreEqual(
                typeof(GovernmentMedicareCoverage),
                coverage.GetType()
                );
            Assert.AreEqual(
                this.address,
                coverage.BillingInformation.Address 
                );
                 
        }
        [Test()]
        public void TestGovernmentOtherCoverage()
        {
            
            GovernmentOtherInsurancePlan plan = new GovernmentOtherInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "GovtOther";
            Insured insured = new Insured(); 
            insured.FirstName = "Sam";
            
            Coverage coverage = Coverage.CoverageFor(plan,insured);
            coverage.BillingInformation.Address = this.address;
            Assert.AreEqual(
                0,
                coverage.Oid 
                );
       
            Assert.AreEqual(
                "GovtOther",
                coverage.InsurancePlan.PlanName 
                );
            Assert.AreEqual(
                "Sam",
                coverage.Insured.FirstName
                );
           
            Assert.AreEqual(
                typeof(GovernmentOtherCoverage),
                coverage.GetType()
                );
            Assert.AreEqual(
                this.address,
                coverage.BillingInformation.Address 
                );
        }
        [Test()]
        public void TestOtherCoverage()
        {
            
            OtherInsurancePlan plan = new OtherInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "Other";
            Insured insured = new Insured(); 
            insured.FirstName = "Sam";
            
            OtherCoverage coverage = (OtherCoverage)Coverage.CoverageFor(plan,insured);
            coverage.Authorization.AuthorizationNumber = "auth 001";
            coverage.Authorization.NumberOfDaysAuthorized = 45;
            coverage.TrackingNumber = "Trct 001";
            coverage.PreCertNumber = 092627328;
          
            
            coverage.BillingInformation.Address = this.address;
            Assert.AreEqual(
                0,
                coverage.Oid 
                );
       
            Assert.AreEqual(
                "Other",
                coverage.InsurancePlan.PlanName 
                );
            Assert.AreEqual(
                "Sam",
                coverage.Insured.FirstName
                );
           
            Assert.AreEqual(
                typeof(OtherCoverage),
                coverage.GetType()
                );
            Assert.AreEqual(
                this.address,
                coverage.BillingInformation.Address 
                );
                 
        }
        [Test()]
        public void TestSelfPayCoverage()
        {
            
            SelfPayInsurancePlan plan = new SelfPayInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "SelfPay";
            Insured insured = new Insured(); 
            insured.FirstName = "Sam";
            
            SelfPayCoverage coverage = (SelfPayCoverage)Coverage.CoverageFor(plan,insured);
            coverage.BillingInformation.Address = this.address;
            
            Assert.AreEqual(
                0,
                coverage.Oid 
                );
       
            Assert.AreEqual(
                "SelfPay",
                coverage.InsurancePlan.PlanName 
                );
            Assert.AreEqual(
                "Sam",
                coverage.Insured.FirstName
                );
           
            Assert.AreEqual(
                typeof(SelfPayCoverage),
                coverage.GetType()
                );
            Assert.AreEqual(
                this.address,
                coverage.BillingInformation.Address 
                );
                 
        }
        [Test()]
        public void TestWorkersCompensationCoverage()
        {
            
            WorkersCompensationInsurancePlan plan = new WorkersCompensationInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "WorkersComp";
            Insured insured = new Insured(); 
            insured.FirstName = "Sam";
            
            Coverage coverage = Coverage.CoverageFor(plan,insured);
            coverage.BillingInformation.Address = this.address;
            WorkersCompensationCoverage workersComp = (WorkersCompensationCoverage)coverage;
            workersComp.InsuranceAdjuster = "adjuster 1" ;
            workersComp.PatientsSupervisor = " supervisor ";
            workersComp.PolicyNumber = " Policy 1001";

            Assert.AreEqual(
                "adjuster 1",
                workersComp.InsuranceAdjuster 
                );

            Assert.AreEqual(
                " supervisor ",
                workersComp.PatientsSupervisor 
                );

            Assert.AreEqual(
                " Policy 1001",
                workersComp.PolicyNumber 
                );


            Assert.AreEqual(
                0,
                workersComp.Oid 
                );
       
            Assert.AreEqual(
                "WorkersComp",
                workersComp.InsurancePlan.PlanName 
                );
            Assert.AreEqual(
                "Sam",
                workersComp.Insured.FirstName
                );
           
            Assert.AreEqual(
                typeof(WorkersCompensationCoverage),
                coverage.GetType()
                );
            Assert.AreEqual(
                this.address,
                workersComp.BillingInformation.Address 
                );
                 
        }
        [Test()]
        public void testBillingInformations()
        {
            
            CommercialInsurancePlan plan = new CommercialInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "PRIVATE PAY";
            Insured insured = new Insured(); 
            insured.FirstName = "Sam";
            BillingInformation aBillingInformation = new BillingInformation(this.address,
                                                                            new PhoneNumber("8005235800")
                                                                            ,new EmailAddress("Bill.Person@claims.net"), new TypeOfContactPoint(0L,"work"));    
            
            aBillingInformation.BillingCOName = "Medicare Part A Claims";
        
         
            
            Coverage coverage = Coverage.CoverageFor(plan,insured);
            coverage.BillingInformation.Address = this.address;

          
            coverage.BillingInformation = aBillingInformation;
            
           
            Assert.AreEqual(
                aBillingInformation,
                coverage.BillingInformation
                );
                 
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        const string ADDRESS1 = "335 Nicholson Dr.",
                     ADDRESS2 = "#303",
                     CITY = "Austin",    
                     POSTALCODE = "60505";

        readonly Address address = new Address( ADDRESS1,
                                       ADDRESS2,
                                       CITY,
                                       new ZipCode( POSTALCODE ),
                                       new State( 0L,
                                                  ReferenceValue.NEW_VERSION,
                                                  "TEXAS",
                                                  "TX"),
                                       new Country( 0L,
                                                    ReferenceValue.NEW_VERSION,
                                                    "United States",
                                                    "USA"),
                                       new County( 0L,
                                                   ReferenceValue.NEW_VERSION,
                                                   "ORANGE",
                                                   "122")); 

        #endregion
    }
}