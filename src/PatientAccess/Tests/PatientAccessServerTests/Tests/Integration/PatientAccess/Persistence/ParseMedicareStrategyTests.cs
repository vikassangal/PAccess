using PatientAccess.Domain;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{

    /// <summary>
    ///This is a test class for ParseMedicareStrategyTests and is intended
    ///to contain all ParseMedicareStrategyTests Unit Tests
    ///</summary>
    [TestFixture]
    [Category( "Fast" )]
    public class ParseMedicareStrategyTests
    {
        #region Constants 

        const string ELIG_OR_BEN_ACTIVE_COVERAGE = "1";
        const string ELIG_OR_BEN_ACTIVE_COVERAGE_LIMITATIONS = "F";
        const string PART_A = "MA";
        const string QUANTITY_QUALIFIER_DAYS = "DY";
        const string SERVICE_TYPE_CODE_SKILLED_NURSING_CARE = "AG";
        const string TIME_PERIOD_QUALIFIER_REMAINING = "29";

        const string ELIG_OR_BEN_CO_PAYMENT = "B";
        const string SERVICE_TYPE_CODE_HOSPITAL = "47";
        const string QUANTITY_COVERAGE_LIMITATIONS = "57";
        const string QUANTITY_COPAYMENT_SNF_DAYS = "80";

        #endregion Constants 

        /// <summary>
        /// A test for ParseCoveragesFor to ensure that the method does not
        /// throw an IndexOutOfBoundsException if none of the parsing rules
        /// match. We use a copy of the rules from the ParseMedicareStrategy
        /// class and call the class without giving it initialization data.
        /// This will cause the method to exhaust its rules and trigger the
        /// exception ( or not, if the bug is fixed )
        ///</summary>
        /// <remarks>A test for Bug 1975 - Medicare Parser throws 
        /// exception if unable to match SNF or Hospital Days value
        /// </remarks>
        [Test]
        public void ParseCoveragesFor_NoMatch_ShouldNotThrowIndexException()
        {

            var snfDaysRemainingParseRules =
                new string[2, 5] {                                                                          
                                     {   ELIG_OR_BEN_ACTIVE_COVERAGE_LIMITATIONS, 
                                         PART_A, 
                                         SERVICE_TYPE_CODE_SKILLED_NURSING_CARE,
                                         TIME_PERIOD_QUALIFIER_REMAINING, 
                                         QUANTITY_QUALIFIER_DAYS 
                                     },

                                     {   ELIG_OR_BEN_ACTIVE_COVERAGE, 
                                         PART_A, 
                                         SERVICE_TYPE_CODE_SKILLED_NURSING_CARE,
                                         TIME_PERIOD_QUALIFIER_REMAINING, 
                                         QUANTITY_QUALIFIER_DAYS 
                                     }
                                 };

            var strategyUnderTest =  new ParseMedicareStrategy();

            strategyUnderTest.ParseCoveragesFor( snfDaysRemainingParseRules );

        }

        /// <summary>
        /// A test for GetCoInsSNFDaysRemaining to ensure that the method returns the result of 
        /// GetCoPaymentSNFDaysRemaining method when there is no match within itself i.e., 
        /// when there is no Co-Insurance SNF days in the response ( if the bug is fixed ).
        ///</summary>
        /// <remarks>A test for 'Bug 5160 - Need SNF days parsed out whether they are 
        /// Co-Insurance or Co-Payment SNF days being returned in the response'. 
        /// 
        /// Only benefits information that matches the criteria for GetCoPaymentSNFDaysRemaining method
        /// is added, so that even when GetCoInsSNFDaysRemaining method is called, since there would
        /// be a mismatch for its criteria as there are no Co-Insurance SNF days in the test data
        /// but only Co-Payment SNF days, it would get the results from GetCoPaymentSNFDaysRemaining.
        /// </remarks>
        [Test]
        public void Test_GetCoInsSNFDaysRemaining_NoMatch_ShouldGetCoPaymentSNFDaysRemaining()
        {
            var benefit1 = new BenefitsInformation
            {
                EligibilityOrBenefitInformation = ELIG_OR_BEN_ACTIVE_COVERAGE_LIMITATIONS,
                InsuranceTypeCode = PART_A,
                ServiceTypeCode = SERVICE_TYPE_CODE_HOSPITAL,
                TimePeriodQualifier = TIME_PERIOD_QUALIFIER_REMAINING,
                QuantityQualifier = QUANTITY_QUALIFIER_DAYS,
                Quantity = QUANTITY_COVERAGE_LIMITATIONS
            };

            var benefit2 = new BenefitsInformation
            {
                EligibilityOrBenefitInformation = ELIG_OR_BEN_CO_PAYMENT,
                InsuranceTypeCode = PART_A,
                ServiceTypeCode = SERVICE_TYPE_CODE_SKILLED_NURSING_CARE,
                TimePeriodQualifier = TIME_PERIOD_QUALIFIER_REMAINING,
                QuantityQualifier = QUANTITY_QUALIFIER_DAYS,
                Quantity = QUANTITY_COPAYMENT_SNF_DAYS
            };

            var medicareStrategy = new ParseMedicareStrategy();
            medicareStrategy.AddBenefits( benefit1 );
            medicareStrategy.AddBenefits( benefit2 );

            int snfDays = medicareStrategy.GetCoInsSNFDaysRemaining();
            Assert.IsTrue( snfDays == int.Parse( QUANTITY_COPAYMENT_SNF_DAYS ) );
        }
    }
}