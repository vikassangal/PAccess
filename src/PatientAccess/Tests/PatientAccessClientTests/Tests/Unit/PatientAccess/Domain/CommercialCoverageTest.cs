using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CommercialCoverageTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CommercialCoverageTests
        [TestFixtureSetUp()]
        public static void SetUpCommercialCoverageTests()
        {
        }

        [SetUp()]
        public void SetUpCommercialCoverage()
        {
            BuildCoverage( );
        }
        [TestFixtureTearDown()]
        public static void TearDownCommercialCoverageTests( )
        {

        }
        #endregion

        #region Test Methods
        [Test()]
        public void testCommercialCoverage( )
        {

            Assert.AreEqual(
                i_Coverage.PPOPricingOrBroker,
                "HMO"
                );

            Assert.AreEqual(
                i_Coverage.ServiceForPreExistingCondition.Code,
                "N"
                );
            Assert.AreEqual(
                i_Coverage.InsuranceCompanyRepName,
                "JoAnn Smith"
                );
        }

        [Test()]
        public void TestNullConstraint( )
        {
            i_Coverage.SetCoverageConstraints( null );

        }

        [Test()]
        public void TestConstraintsExpectedValue( )
        {
            CommercialConstraints constraints = new CommercialConstraints( );
            constraints.EligibilityPhone = "(111)111-1111";
            constraints.InsuranceCompanyRepName = "Bob Joe";
            constraints.EffectiveDateForInsured = DateTime.MinValue;
            constraints.TerminationDateForInsured = DateTime.MinValue;
            
            i_Coverage.SetCoverageConstraints( constraints );
            Assert.AreEqual( i_Coverage.EligibilityPhone, "(111)111-1111" );
            Assert.AreEqual( i_Coverage.InsuranceCompanyRepName, "Bob Joe" );
            Assert.AreEqual( i_Coverage.EffectiveDateForInsured, DateTime.MinValue );
            Assert.AreEqual( i_Coverage.TerminationDateForInsured, DateTime.MinValue );
        }

        #endregion

        #region Support Methods

        private static void BuildCoverage( )
        {
            YesNoFlag falseFlag = new YesNoFlag( );
            falseFlag.SetNo( );
            YesNoFlag trueFlag = new YesNoFlag( );
            trueFlag.SetYes( );
            i_Coverage = new CommercialCoverage( );
            i_Coverage.ServiceForPreExistingCondition = falseFlag;
            i_Coverage.ServiceIsCoveredBenefit = trueFlag;
            i_Coverage.ClaimsAddressVerified = falseFlag;
            i_Coverage.CoordinationOfbenefits = trueFlag;
            i_Coverage.TypeOfProduct = new TypeOfProduct( );
            i_Coverage.PPOPricingOrBroker = "HMO";
            i_Coverage.FacilityContractedProvider = trueFlag;
            i_Coverage.AutoInsuranceClaimNumber = "jsdh8we";
            i_Coverage.AutoMedPayCoverage = falseFlag;
            i_Coverage.TypeOfVerificationRule = new TypeOfVerificationRule( );
            
            i_Coverage.EligibilityPhone = "(999)222-2222";
            i_Coverage.InsuranceCompanyRepName = "JoAnn Smith";
            i_Coverage.EffectiveDateForInsured = new DateTime( 1990, 12, 12 );
            i_Coverage.TerminationDateForInsured = new DateTime( 1980, 1, 1 );


        }
        #endregion

        #region Data Elements
        private static CommercialCoverage i_Coverage = null;        
        #endregion
    }
}