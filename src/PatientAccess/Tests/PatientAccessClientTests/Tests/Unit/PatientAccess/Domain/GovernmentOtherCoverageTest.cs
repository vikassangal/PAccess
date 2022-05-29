using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class GovernmentOtherCoverageTest
    {
        #region Constants
        #endregion

        #region SetUp and TearDown GovernmentMedicareCoverageTests

        [SetUp]
        public void SetUp()
        {
            this.BuildCoverage();
        }

        [TestFixtureSetUp()]
        public static void SetUpGovernmentOtherCoverageTests()
        {
            
        }

        [TestFixtureTearDown()]
        public static void TearDownGovernmentOtherCoverageTests( )
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestGovernmentOtherCoverage( )
        {

          
            Assert.AreEqual(
                this.i_Coverage.Authorization.AuthorizationNumber,
                "545454"
                );

            Assert.AreEqual(
                this.i_Coverage.EligibilityPhone,
                "(444)444-4444"
                );
            Assert.AreEqual(
                this.i_Coverage.InsuranceCompanyRepName,
                "Jane Doe" 
                );
            Assert.AreEqual(
                this.i_Coverage.TerminationDateForInsured,
                new DateTime( 2004, 12, 20 )
                );
        }

        [Test()]
        public void TestNullConstraints( )
        {
            this.i_Coverage.SetCoverageConstraints( null );
            Assert.AreEqual( this.i_Coverage.InsuranceCompanyRepName,
                             "Jane Doe" );
        }

        [Test()]
        public void TestConstraintsExpectedValue( )
        {
            GovernmentOtherConstraints constraints = new GovernmentOtherConstraints( );
            constraints.InsuranceCompanyRepName = "Bob Smith";
            constraints.EligibilityPhone = "(111)111-1111";
            constraints.EffectiveDateForInsured = DateTime.MinValue;
            constraints.TerminationDateForInsured = DateTime.MinValue;
            
            constraints.Deductible = 30;
            constraints.TypeOfCoverage = "Known";
            constraints.DeductibleDollarsMet = 33;
            constraints.CoInsurance = 20;
            constraints.OutOfPocket = 10;
            constraints.OutOfPocketDollarsMet = 11;
            constraints.CoPay = 10;
            this.i_Coverage.SetCoverageConstraints( constraints );
           
            Assert.AreEqual( this.i_Coverage.InsuranceCompanyRepName, "Bob Smith" );
            Assert.AreEqual( this.i_Coverage.EligibilityPhone, "(111)111-1111" );
            Assert.AreEqual( this.i_Coverage.EffectiveDateForInsured, DateTime.MinValue );
            Assert.AreEqual( this.i_Coverage.TerminationDateForInsured, DateTime.MinValue );
            Assert.AreEqual( this.i_Coverage.TypeOfCoverage, "Known" );
            
            Assert.AreEqual( this.i_Coverage.BenefitsCategoryDetails.Deductible, 30 );
            Assert.AreEqual( this.i_Coverage.BenefitsCategoryDetails.DeductibleDollarsMet, 33 );
            Assert.AreEqual( this.i_Coverage.BenefitsCategoryDetails.CoInsurance, 20 );
            Assert.AreEqual( this.i_Coverage.BenefitsCategoryDetails.OutOfPocket, 10 );
            Assert.AreEqual( this.i_Coverage.BenefitsCategoryDetails.OutOfPocketDollarsMet, 11 );
            Assert.AreEqual( this.i_Coverage.BenefitsCategoryDetails.CoPay, 10 );
        }
        #endregion

        #region Support Methods

        private void BuildCoverage( )
        {
            this.i_Coverage = new GovernmentOtherCoverage( );
            this.i_Coverage.Authorization.AuthorizationNumber = "545454";
            this.i_Coverage.EligibilityPhone = "(444)444-4444";
            this.i_Coverage.InsuranceCompanyRepName = "Jane Doe";
            this.i_Coverage.EffectiveDateForInsured = new DateTime( 1990, 12, 12 );
            this.i_Coverage.TerminationDateForInsured = new DateTime( 2004, 12, 20 );
            this.i_Coverage.TypeOfCoverage = "Unknown";
            
            this.i_Coverage.BenefitsCategoryDetails = new BenefitsCategoryDetails( );
            this.i_Coverage.BenefitsCategoryDetails.Deductible = 250;
            this.i_Coverage.BenefitsCategoryDetails.DeductibleDollarsMet = 20;
            this.i_Coverage.BenefitsCategoryDetails.CoInsurance = 10;
            this.i_Coverage.BenefitsCategoryDetails.OutOfPocket = 100;
            this.i_Coverage.BenefitsCategoryDetails.OutOfPocketDollarsMet = 15;
            this.i_Coverage.BenefitsCategoryDetails.CoPay = 25;
        }

        #endregion

        #region Data Elements
        private GovernmentOtherCoverage i_Coverage = null;
        #endregion
    }
}