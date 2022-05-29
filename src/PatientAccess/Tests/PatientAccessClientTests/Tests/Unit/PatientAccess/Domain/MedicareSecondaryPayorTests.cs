using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class MedicareSecondaryPayorTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown MedicareSecondaryPayorTests
        #endregion

        #region Test Methods
        [Test]
        public void TestGetPartiallyCopiedForwardMSPFrom_WhenOldMSPHasVersion_ShouldCopyForwardVersion()
        {
            var oldMsp = new MedicareSecondaryPayor
                             {
                                 MSPVersion = 2
                             };

            var newMsp = MedicareSecondaryPayor.GetPartiallyCopiedForwardMSPFrom(oldMsp);
            Assert.IsNotNull(newMsp, "NewMSP is null");
            Assert.AreEqual(oldMsp.MSPVersion, newMsp.MSPVersion, "MSP version did not copy forward");
        }

        [Test]
        public void TestGetPartiallyCopiedForwardMSPFrom_ShouldCopyForwardSpecialPrograms()
        {
            
            var specialProgram = new SpecialProgram { BLBenefitsStartDate = DateTime.Now };
            specialProgram.BlackLungBenefits.SetYes();
            specialProgram.VisitForBlackLung.SetYes();
            specialProgram.VisitForBlackLung.SetNo();
            specialProgram.DVAAuthorized.SetNo();
            var oldMsp = new MedicareSecondaryPayor
            {
                SpecialProgram = specialProgram
            };
            var newMsp = MedicareSecondaryPayor.GetPartiallyCopiedForwardMSPFrom(oldMsp);
            Assert.IsNotNull(newMsp.SpecialProgram, "NewMSP special program is set to null");
            Assert.IsTrue(newMsp.SpecialProgram.BlackLungBenefits.IsYes, "Special progrma BlackLungBenefits should be copied over and set to Yes.");
            Assert.IsTrue(newMsp.SpecialProgram.VisitForBlackLung.IsBlank, "Special progrma VisitForBlackLung should not be copied over and set to blank.");
            Assert.IsTrue(newMsp.SpecialProgram.DVAAuthorized.IsBlank, "Special progrma DVAAuthorized should not be copied over and set to blank.");
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}