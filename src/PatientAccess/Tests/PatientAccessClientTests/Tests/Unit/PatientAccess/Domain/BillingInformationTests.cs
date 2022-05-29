using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class BillingInformationTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown BillingInformationTests
        [TestFixtureSetUp()]
        public static void SetUpBillingInformationTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownBillingInformationTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void BillingInformationsTests()
        {
            CommercialInsurancePlan plan = new CommercialInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "PRIVATE PAY";
            Insured insured = new Insured(); 
            insured.FirstName = "Sam";
            BillingInformation aBillingInformation = new BillingInformation(this.address,
                                                                            new PhoneNumber("8005235800" )
                                                                            ,new EmailAddress("Bill.Person@claims.net"), new TypeOfContactPoint(0L,"work"));    
            
            aBillingInformation.BillingCOName = "Medicare Part A Claims";
        
            Coverage coverage = Coverage.CoverageFor(plan,insured);
            coverage.BillingInformation.Address = this.address;

            PhoneNumber ph = new PhoneNumber("8005235800");
            coverage.BillingInformation = aBillingInformation;
            Assert.AreEqual(
                ph.AreaCode,
                coverage.BillingInformation.PhoneNumber.AreaCode
                );
            Assert.AreEqual(
                ph.Number,
                coverage.BillingInformation.PhoneNumber.Number
                );
           
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
                                                   "122") ); 
        #endregion
    }
}