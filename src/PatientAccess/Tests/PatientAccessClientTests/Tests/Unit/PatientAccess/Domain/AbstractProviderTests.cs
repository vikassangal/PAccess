using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class AbstractProviderTests
    {
        #region Constants
       
        #endregion

        #region SetUp and TearDown AbstractProviderTests
        [TestFixtureSetUp()]
        public static void SetUpAbstractProviderTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownAbstractProviderTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestPayorConstructor()
        {
            InsurancePlan plan1= new CommercialInsurancePlan();
            plan1.Oid = 1;
            plan1.PlanName = "Commercial";

            InsurancePlan plan2 = new GovernmentOtherInsurancePlan();
            plan2.Oid = 1;
            plan2.PlanName = "GovtOther";

            InsurancePlan plan3 = new GovernmentMedicareInsurancePlan();
            plan3.Oid = 1;
            plan3.PlanName = "Medicare";
            
            AbstractProvider provider =  new Payor();
            provider.Name = "Cigna";
            provider.Code = "CC";

            provider.AddInsurancePlan(plan1);
            provider.NumberOfActivePlans = provider.InsurancePlans.Count.ToString();
            provider.AddInsurancePlan(plan2);
            provider.NumberOfActivePlans =provider.InsurancePlans.Count.ToString();
            provider.AddInsurancePlan(plan3);
            provider.NumberOfActivePlans =provider.InsurancePlans.Count.ToString();

            provider.RemoveInsurancePlan(plan1);
            provider.NumberOfActivePlans = provider.InsurancePlans.Count.ToString();
            string planCount = provider.NumberOfActivePlans;
            
            Assert.AreEqual(
                typeof(Payor),
                provider.GetType()
                );
            
            Assert.AreEqual( 
                "2", provider.NumberOfActivePlans
                ,       "There should be 2 insurancePLans for this  provider" );

            Assert.AreEqual( 
                "Cigna", provider.Name
                ,       "Name of this provider should be cigna" );

            Assert.AreEqual( 
                "CC", provider.Code
                ,       "Code of this provider should be CC" );
            
            //  provider.PlansLoader = new PlansLoader(provider,facility);
          
           

        }

        [Test()]
        public void TestBrokerConstructor()
        {
            InsurancePlan plan1= new CommercialInsurancePlan();
            plan1.Oid = 1;
            plan1.PlanName = "Commercial";

            InsurancePlan plan2 = new GovernmentOtherInsurancePlan();
            plan2.Oid = 1;
            plan2.PlanName = "GovtOther";

            InsurancePlan plan3 = new GovernmentMedicareInsurancePlan();
            plan3.Oid = 1;
            plan3.PlanName = "Medicare";
            
            AbstractProvider provider =  new Broker();
            provider.Name = "Cigna";
            provider.Code = "CC";

            provider.AddInsurancePlan(plan1);
            provider.NumberOfActivePlans = provider.InsurancePlans.Count.ToString();
            provider.AddInsurancePlan(plan2);
            provider.NumberOfActivePlans =provider.InsurancePlans.Count.ToString();
            provider.AddInsurancePlan(plan3);
            provider.NumberOfActivePlans =provider.InsurancePlans.Count.ToString();

            provider.RemoveInsurancePlan(plan1);
            provider.NumberOfActivePlans = provider.InsurancePlans.Count.ToString();
            string planCount = provider.NumberOfActivePlans;
            
            Assert.AreEqual(
                typeof(Broker),
                provider.GetType()
                );
            
            Assert.AreEqual( 
                "2", provider.NumberOfActivePlans
                ,       "There should be 2 insurancePLans for this  provider" );

            Assert.AreEqual( 
                "Cigna", provider.Name
                ,       "Name of this provider should be cigna" );

            Assert.AreEqual( 
                "CC", provider.Code
                ,       "Code of this provider should be CC" );
            
            //  provider.PlansLoader = new PlansLoader(provider,facility);
          
           

        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
    
        
        #endregion
    }
}