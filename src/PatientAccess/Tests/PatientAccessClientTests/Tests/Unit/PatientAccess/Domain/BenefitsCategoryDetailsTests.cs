using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    /// <summary>
    /// Summary description for BenefitsCategoryDetailsTests.
    /// </summary>

    //TODO: Create XML summary comment for BenefitsCategoryDetailsTests
    [TestFixture]
    [Category( "Fast" )]
    public class BenefitsCategoryDetailsTests
    {
        #region Constants
        public const long   
            TESTOID                 = 234;
        public const string         
            TESTDESC                = "sdlkf8",
            TESTCODE                = "9dje";
        #endregion

        #region SetUp and TearDown BenefitsCategoryDetailsTests
        [TestFixtureSetUp()]
        public static void SetUpBenefitsCategoryDetailsTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownBenefitsCategoryDetailsTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestBenefitsCategoryDetails()
        {
            BenefitsCategoryDetails  aBenefitsCategoryDetails = new BenefitsCategoryDetails();
            TimePeriodFlag time = new TimePeriodFlag();
            time.SetYear();
            aBenefitsCategoryDetails.TimePeriod= time;
            YesNoFlag trueFlag = new YesNoFlag();
            trueFlag.SetYes();
            YesNoFlag falseFlag = new YesNoFlag();
            falseFlag.SetNo();
            aBenefitsCategoryDetails.DeductibleMet = trueFlag ;
            aBenefitsCategoryDetails.DeductibleDollarsMet = 50.00F;
            aBenefitsCategoryDetails.CoInsurance = 100;
            aBenefitsCategoryDetails.OutOfPocket = 10.00F;
            aBenefitsCategoryDetails.OutOfPocketMet = falseFlag ;
            aBenefitsCategoryDetails.OutOfPocketDollarsMet= 0.0F;
            aBenefitsCategoryDetails.AfterOutOfPocketPercent = 90;
            aBenefitsCategoryDetails.WaiveCopayIfAdmitted =trueFlag;
            aBenefitsCategoryDetails.VisitsPerYear =2;
            aBenefitsCategoryDetails.LifeTimeMaxBenefit = 1000.0F;
            aBenefitsCategoryDetails.RemainingLifetimeValueMet = falseFlag ;
            aBenefitsCategoryDetails.MaxBenefitPerVisit = 200.0F;
            aBenefitsCategoryDetails.RemainingBenefitPerVisitsMet = falseFlag;

            Assert.AreEqual(
                aBenefitsCategoryDetails.TimePeriod.ToString() ,
                "Year"
                );

            Assert.AreEqual(
                aBenefitsCategoryDetails.VisitsPerYear  ,
                2
                );
            
            Assert.AreEqual(
                aBenefitsCategoryDetails.DeductibleMet.Code ,
                "Y"
                );

            Assert.AreEqual(
                aBenefitsCategoryDetails.DeductibleDollarsMet ,
                50.00F
                );

            Assert.AreEqual(
                aBenefitsCategoryDetails.CoInsurance ,
                100
                );
            
            Assert.AreEqual(
                aBenefitsCategoryDetails.OutOfPocket,
                10.0f
                );
            
            Assert.AreEqual(
                aBenefitsCategoryDetails.OutOfPocketDollarsMet,
                0.0F
                );
                      
            Assert.AreEqual(
                aBenefitsCategoryDetails.OutOfPocketMet.Code,
                "N"
                );
            Assert.AreEqual(
                aBenefitsCategoryDetails.AfterOutOfPocketPercent,
                90
                );
            Assert.AreEqual(
                aBenefitsCategoryDetails.WaiveCopayIfAdmitted.Code,
                "Y"
                );
            Assert.AreEqual(
                aBenefitsCategoryDetails.LifeTimeMaxBenefit,
                1000.0f
                );
            Assert.AreEqual(
                aBenefitsCategoryDetails.RemainingLifetimeValueMet.Code,
                "N"
                );
            Assert.AreEqual(
                aBenefitsCategoryDetails.MaxBenefitPerVisit,
                200.0f
                );
            Assert.AreEqual(
                aBenefitsCategoryDetails.RemainingBenefitPerVisitsMet.Code,
                "N"
                );
            
           
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}