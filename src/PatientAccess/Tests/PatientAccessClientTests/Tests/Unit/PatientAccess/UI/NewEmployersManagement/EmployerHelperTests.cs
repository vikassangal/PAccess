using NUnit.Framework;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.NewEmployersManagement;

namespace Tests.Unit.PatientAccess.UI.NewEmployersManagement
{
    /// <summary>
    ///This is a test class for EmployerHelperTests and is intended
    ///to contain all EmployerHelperTests Unit Tests
    ///</summary>
    [TestFixture]
    [Category( "Fast" )]
    public class EmployerHelperTests
    {
        [Test()]
        public void EmployerAddressHasStreetAndCityTest_EmployerHasBothCityAndStreetInAddress_ShouldReturnFalse()
        {
            Employer employer = EmployerUtilities.GetEmployerWithFullAddress();

            bool employerAddressHasStreetAndCity = EmployerHelper.EmployerAddressHasStreetAndCity(employer);
            Assert.IsTrue(employerAddressHasStreetAndCity);

        }

        [Test()]
        public void EmployerAddressHasStreetAndCityTest_EmployerHasStreetButNoCityInAddress_ShouldReturnFalse()
        {
            Employer employer = EmployerUtilities.GetEmployerWithFullAddress();

            EmployerUtilities.SetEmployerAddressCityTo(employer, null);

            bool employerAddressHasStreetAndCity = EmployerHelper.EmployerAddressHasStreetAndCity(employer);
            Assert.IsFalse(employerAddressHasStreetAndCity);
        }

        [Test()]
        public void EmployerAddressHasStreetAndCityTest_EmployerHasCityButNoStreetInAddress_ShouldReturnFalse()
        {
            Employer employer = EmployerUtilities.GetEmployerWithFullAddress();

            EmployerUtilities.SetEmployerAddressLine1To(employer, null);
            EmployerUtilities.SetEmployerAddressLine2To(employer, string.Empty);

            bool employerAddressHasStreetAndCity = EmployerHelper.EmployerAddressHasStreetAndCity(employer);
            Assert.IsFalse(employerAddressHasStreetAndCity);
        }
        
        [Test()]
        public void EmployerAddressHasStreetAndCityTest_EmployerDoesNotHaveCityOrStreetInAddress_ShouldReturnFalse()
        {
            Employer employer = EmployerUtilities.GetEmployerWithFullAddress();

            EmployerUtilities.SetEmployerAddressCityTo(employer, string.Empty);
            EmployerUtilities.SetEmployerAddressLine1To(employer, null);
            EmployerUtilities.SetEmployerAddressLine2To(employer, string.Empty);

            bool employerAddressHasStreetAndCity = EmployerHelper.EmployerAddressHasStreetAndCity(employer);
            Assert.IsFalse(employerAddressHasStreetAndCity);
        }
    }
}