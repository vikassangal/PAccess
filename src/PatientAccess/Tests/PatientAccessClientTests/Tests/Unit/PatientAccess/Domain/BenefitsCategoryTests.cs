using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    /// <summary>
    /// Summary description for BenefitsCategoryTests.
    /// </summary>

    //TODO: Create XML summary comment for BenefitsCategoryTests
    [TestFixture]
    [Category( "Fast" )]
    public class BenefitsCategoryTests
    {
        #region Constants

        private const long   
            TESTOID                 = 234;

        private const string         
            TESTDESC                = "sdlkf8";

        public const string         
            TESTCODE                = "9dje";

        #endregion

        #region SetUp and TearDown BenefitsCategoryTests
        [TestFixtureSetUp()]
        public static void SetUpBenefitsCategoryTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownBenefitsCategoryTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestBenefitsCategory()
        {
            BenefitsCategory benefitsCategory = new BenefitsCategory(
                TESTOID,ReferenceValue.NEW_VERSION,
                TESTDESC);
            Assert.IsNotNull(benefitsCategory, "Create of VisitType failed");
            Assert.IsTrue(benefitsCategory.Oid == TESTOID, "OID is incorrect");
            //            Assert.IsTrue(benefitsCategory.Code == TESTCODE, "CODE is incorrect");
            Assert.IsTrue(benefitsCategory.Description == TESTDESC, "Description is incorrect");

            benefitsCategory = new BenefitsCategory(BenefitsCategory.INPATIENTOID, 
                                                    ReferenceValue.NEW_VERSION,
                                                    "Inpatient");
            Assert.IsTrue(benefitsCategory.Description == "Inpatient", 
                          "Manually created Inpatient Benefits category has incorrect member values");
            Assert.IsTrue(benefitsCategory.isInPatient(),
                          "Inpatient Benefit category is invalid");
            
            benefitsCategory = new BenefitsCategory(BenefitsCategory.OUTPATIENTOID, ReferenceValue.NEW_VERSION,
                                                    "OutPatient");
            Assert.IsTrue(benefitsCategory.Oid == BenefitsCategory.OUTPATIENTOID, 
                          "Manually created outpatient Benefits Category has incorrect oid");

            benefitsCategory = new BenefitsCategory(BenefitsCategory.OBOID, 
                                                    ReferenceValue.NEW_VERSION,
                                                    "OB");
            Assert.IsTrue(benefitsCategory.Oid ==  BenefitsCategory.OBOID, 
                          "OB BenefitsCategory has invalid OID");
            Assert.IsTrue(benefitsCategory.isOB(),"OB BenefitsCategory is not correct");
            
            benefitsCategory = new BenefitsCategory(BenefitsCategory.NEWBORNOID, 
                                                    ReferenceValue.NEW_VERSION,
                                                    "NewBorn");
            Assert.IsTrue(benefitsCategory.isNewBorn(),"New born Benefits category is not valid");
            Assert.IsTrue(benefitsCategory.Oid ==  BenefitsCategory.NEWBORNOID, 
                          "New born BenefitsCategory is invalid");
            
            benefitsCategory = new BenefitsCategory(BenefitsCategory.NICUOID, 
                                                    ReferenceValue.NEW_VERSION,
                                                    "NICU");
            Assert.IsTrue(benefitsCategory.isNICU(),"NICU BenefitsCategory has wrong OID");
            Assert.IsTrue(benefitsCategory.Oid ==  BenefitsCategory.NICUOID, 
                          "NICU BenefitsCategory has incorrect OID");
            
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}