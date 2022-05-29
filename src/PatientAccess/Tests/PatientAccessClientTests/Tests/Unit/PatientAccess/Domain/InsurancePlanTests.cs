using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class InsurancePlanTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown InsurancePlanTests
        [TestFixtureSetUp()]
        public static void SetUpInsurancePlanTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownInsurancePlanTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCoverageFor()

        {
            Payor p = new Payor();
            p.Code = "AA";
          
            InsurancePlan commercial = new CommercialInsurancePlan();
            Assert.AreEqual(
                typeof( CommercialInsurancePlan),
                commercial.GetType()
                );
            commercial.Payor = p;
            Assert.AreEqual(
                "AA",
                commercial.Payor.Code
                );
            
            InsurancePlan medicaid = new GovernmentMedicaidInsurancePlan();
            Assert.AreEqual(
                typeof( GovernmentMedicaidInsurancePlan),
                medicaid.GetType()
                );
            medicaid.Payor = p;
            Assert.AreEqual(
                "AA",
                medicaid.Payor.Code
                );

            InsurancePlan medicare = new GovernmentMedicareInsurancePlan();
            Assert.AreEqual(
                typeof( GovernmentMedicareInsurancePlan),
                medicare.GetType()
                );
            medicare.Payor = p;
            Assert.AreEqual(
                "AA",
                medicare.Payor.Code
                );

            InsurancePlan govtOther = new GovernmentOtherInsurancePlan();
            Assert.AreEqual(
                typeof( GovernmentOtherInsurancePlan),
                govtOther.GetType()
                );
            govtOther.Payor = p;
            Assert.AreEqual(
                "AA",
                govtOther.Payor.Code
                );

            InsurancePlan selfPay = new SelfPayInsurancePlan();
            Assert.AreEqual(
                typeof( SelfPayInsurancePlan),
                selfPay.GetType()
                );
            selfPay.Payor = p;
            Assert.AreEqual(
                "AA",
                selfPay.Payor.Code
                );

            InsurancePlan other = new OtherInsurancePlan();
            other.Payor = p;
            Assert.AreEqual(
                "AA",
                other.Payor.Code
                );
            Assert.AreEqual(
                typeof( OtherInsurancePlan),
                other.GetType()
                );

            InsurancePlan workersComp = new WorkersCompensationInsurancePlan();
            Assert.AreEqual(
                typeof( WorkersCompensationInsurancePlan),
                workersComp.GetType()
                );
            workersComp.Payor = p;
            Assert.AreEqual(
                "AA",
                workersComp.Payor.Code
                );
        }

        [Test()]
        public void TestIsActiveIsTrue()
        {
            InsurancePlan plan = new CommercialInsurancePlan();
            plan.EffectiveOn = DateTime.Now.AddYears( -1 );
            plan.TerminatedOn = DateTime.Now.AddDays( 1 );

            Assert.IsTrue( plan.IsActive() );
        }

        [Test()]
        public void TestIsActiveIsFalse()
        {
            InsurancePlan plan = new CommercialInsurancePlan();
            plan.EffectiveOn = DateTime.Now.AddDays( 5 );
            plan.TerminatedOn = DateTime.Now.AddYears( 1 );

            Assert.IsTrue( !plan.IsActive() );

            plan = new CommercialInsurancePlan();
            plan.EffectiveOn = DateTime.Now.AddDays( -5 );
            plan.TerminatedOn = DateTime.Now.AddDays( -1 );
        }
        [Test()]
        public void testBillingInformations()
        {
            
            CommercialInsurancePlan plan = new CommercialInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "PRIVATE PAY";
           
            BillingInformation aBillingInformation = new BillingInformation(this.address,
                                                                            new PhoneNumber("8005235800")
                                                                            ,new EmailAddress("Bill.Person@claims.net"), new TypeOfContactPoint(0L,"work"));    
            
            aBillingInformation.BillingCOName = "Medicare Part A Claims";
        
            BillingInformation aBillingInformation2 = new BillingInformation(this.address2,
                                                                             new PhoneNumber("8003250058" )
                                                                             ,new EmailAddress("Bill.Person2@claims.net"), new TypeOfContactPoint(0L,"Home"));      
      
            
            aBillingInformation2.BillingCOName = "Medicare Part B Claims";
            
            plan.AddBillingInformation(aBillingInformation);
            plan.AddBillingInformation(aBillingInformation2);
            Insured insured =  new Insured();
            insured.FirstName = "Sam";
            Coverage coverage = Coverage.CoverageFor(plan, insured);
            coverage.BillingInformation = aBillingInformation ;
            
            Assert.AreEqual(
                "PRIVATE PAY",
                coverage.InsurancePlan.PlanName 
                );
            Assert.AreEqual(
                "Sam",
                coverage.Insured.FirstName
                );
           
            Assert.AreEqual(
                typeof(CommercialCoverage),
                coverage.GetType()
                );
            Assert.AreEqual(
                aBillingInformation,
                coverage.BillingInformation
                );
      

          
            Assert.AreEqual(
                2,
                plan.BillingInformations.Count,
                "Plan has two BillingInformations"
                );
            Console.WriteLine("Addresses from Coverage.InsurancePlan.BillingInformations ");
           
            foreach( BillingInformation billing in coverage.InsurancePlan.BillingInformations )
            {
                string formattedString = String.Format(
                    "{0}{1}{2}{1}{3}{1}", 
                    billing.BillingCOName,Environment.NewLine,
                    billing.Address.AsMailingLabel(),
                    billing.PhoneNumber
                    );
                Console.WriteLine( formattedString  );
             
            }
        
                 
        }

        [Test()]
        public void TestIsValidPlanForAdmitDateForGracePeriod()
        {
            CommercialInsurancePlan plan = new CommercialInsurancePlan();
            plan.ApprovedOn = new DateTime( 2006, 01, 01 );
            plan.EffectiveOn = new DateTime( 2006, 01, 01 );
            plan.TerminatedOn = DateTime.Today.AddDays( 10 );
            plan.CanceledOn = DateTime.Today.AddDays( 10 );

            Assert.IsTrue( plan.IsValidPlanForAdmitDate( DateTime.Today ) );
        }

        [Test()]
        public void TestIsValidPlanForAdmitDateWithOutTerminationOn()
        {
            CommercialInsurancePlan plan = new CommercialInsurancePlan();
            plan.ApprovedOn = new DateTime( 2006, 01, 01 );
            plan.EffectiveOn = new DateTime( 2006, 01, 01 );
            plan.TerminatedOn = DateTime.MinValue.AddDays( 10 );
            plan.CanceledOn = DateTime.Today.AddDays( 10 );

            Assert.IsFalse( plan.IsValidPlanForAdmitDate( DateTime.Today ) );
        }
        [Test()]
        public void TestIsDefaultPlan()
        {
            Payor payor = new Payor();
            payor.Code = "UN";
            InsurancePlan plan = new SelfPayInsurancePlan();
            plan.EffectiveOn = DateTime.Now.AddYears(-1);
            plan.TerminatedOn = DateTime.Now.AddDays(1);
            plan.PlanSuffix = "K81";
            plan.Payor = payor;
            Assert.IsTrue(plan.IsDefaultPlan());
        }
        [Test()]
        public void TestIsDefaultPlanIsFalse()
        {
            Payor payor = new Payor();
            payor.Code = "EN";
            InsurancePlan plan = new SelfPayInsurancePlan();
            plan.EffectiveOn = DateTime.Now.AddYears(-1);
            plan.TerminatedOn = DateTime.Now.AddDays(1);
            plan.PlanSuffix = "K81";
            plan.Payor = payor;
            Assert.IsFalse(plan.IsDefaultPlan());
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private const string ADDRESS1 = "335 Nicholson Dr.", 
                             ADDRESS2 = "#303", 
                             ADDRESS3 = "535 XYZ drive",
                             CITY = "Austin",
                             POSTALCODE = "60505";    
        private static readonly string ADDRESS4 = string.Empty;

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

        readonly Address address2 = new Address( ADDRESS3,
                                        ADDRESS4 ,
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