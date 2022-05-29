using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.InsuranceViews.MSP2;

namespace Tests.Unit.PatientAccess.UI.MedicareSecondaryPayor
{
    [TestFixture]
    [Category( "Fast" )]
    public class LiabilityInsurerPageTests
    {
        [Test]
        public void TestGetAccidentDateForDisplay_WhenAccidentDateIsMinValue_ShouldReturnEmptyString()
        {
            var liabilityInsurerPage = new LiabilityInsurerPage();
            Account account = new Account();
            account.MedicareSecondaryPayor.LiabilityInsurer.AccidentDate = DateTime.MinValue;

            liabilityInsurerPage.Model_Account = account;
            string accidentDate = liabilityInsurerPage.GetAccidentDateForDisplay();

            Assert.IsTrue( accidentDate == string.Empty );
        }

        [Test]
        public void TestGetAccidentDateForDisplay_WhenAccidentDateIsNotMinValue_ShouldReturnActualDateAsString()
        {
            var liabilityInsurerPage = new LiabilityInsurerPage();
            Account account = new Account();
            account.MedicareSecondaryPayor.LiabilityInsurer.AccidentDate = new DateTime( 2010, 01, 01 );

            liabilityInsurerPage.Model_Account = account;
            string accidentDate = liabilityInsurerPage.GetAccidentDateForDisplay();

            Assert.IsTrue( accidentDate == "01/01/2010" );
        }
    }
}
