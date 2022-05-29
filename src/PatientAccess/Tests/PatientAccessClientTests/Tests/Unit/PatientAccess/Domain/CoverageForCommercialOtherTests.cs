using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CoverageForCommercialOtherTests
    {
        #region Constants
   
        #endregion

        #region SetUp and TearDown CoverageForCommercialOtherTests
        [TestFixtureSetUp()]
        public static void SetUpCoverageForCommercialOtherTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCoverageForCommercialOtherTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testBenefitsCategories()
        {
           
            BenefitsCategory aOutPatient = new BenefitsCategory();
            aOutPatient.Description = "OutPatient";
            //            aOutPatient.Oid = BenefitsCategory.OUTPATIENTOID;

            BenefitsCategoryDetails  aBenefitsCategoryDetails = new BenefitsCategoryDetails();
            TimePeriodFlag time = new TimePeriodFlag();
            time.SetYear();
            aBenefitsCategoryDetails.TimePeriod= time ;
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
            aBenefitsCategoryDetails.RemainingLifetimeValueMet = falseFlag;
            aBenefitsCategoryDetails.MaxBenefitPerVisit = 200.0F;
            aBenefitsCategoryDetails.RemainingBenefitPerVisitsMet = falseFlag;
            CommercialCoverage aCommercialCoverage = new CommercialCoverage();
       
 
            BenefitsCategory aNewBorn = new BenefitsCategory();
            aNewBorn.Description = "NewBorn";
            //            aNewBorn.Oid = BenefitsCategory.NEWBORNOID;
           
            time.SetVisit();
            BenefitsCategoryDetails  aBenefitsCategoryDetails1 = new BenefitsCategoryDetails();
            aBenefitsCategoryDetails1.TimePeriod= time;
            aBenefitsCategoryDetails1.DeductibleMet = trueFlag ;
            aBenefitsCategoryDetails1.DeductibleDollarsMet = 50.00F;
            aBenefitsCategoryDetails1.CoInsurance = 100;
            aBenefitsCategoryDetails1.OutOfPocket = 10.00F;
            aBenefitsCategoryDetails1.OutOfPocketMet = falseFlag ;
            aBenefitsCategoryDetails1.OutOfPocketDollarsMet= 0.0F;
            aBenefitsCategoryDetails1.AfterOutOfPocketPercent = 90;
            aBenefitsCategoryDetails1.WaiveCopayIfAdmitted =trueFlag;
            aBenefitsCategoryDetails1.VisitsPerYear =2;
            aBenefitsCategoryDetails1.LifeTimeMaxBenefit = 1000.0F;
            aBenefitsCategoryDetails1.RemainingLifetimeValueMet = falseFlag ;
            aBenefitsCategoryDetails1.MaxBenefitPerVisit = 200.0F;
            aBenefitsCategoryDetails1.RemainingBenefitPerVisitsMet = falseFlag;
   
            aCommercialCoverage.AddBenefitsCategory(aOutPatient , aBenefitsCategoryDetails);
            aCommercialCoverage.AddBenefitsCategory(aOutPatient , aBenefitsCategoryDetails);
          
            aCommercialCoverage.AddBenefitsCategory(aNewBorn , aBenefitsCategoryDetails1);
            aCommercialCoverage.RemoveBenefitsCategory(aNewBorn);
                        
            BenefitsCategoryDetails aBenefitsCategoryDetailsT = aCommercialCoverage.BenefitsCategoryDetailsWith(aOutPatient.Description);
            BenefitsCategoryDetails aBenefitsCategoryDetails2 = aCommercialCoverage.BenefitsCategoryDetailsFor(aOutPatient);

            Assert.AreEqual(
                aBenefitsCategoryDetails2.TimePeriod.ToString() ,
                aBenefitsCategoryDetailsT.TimePeriod.ToString()
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
      
        #endregion
    }
}