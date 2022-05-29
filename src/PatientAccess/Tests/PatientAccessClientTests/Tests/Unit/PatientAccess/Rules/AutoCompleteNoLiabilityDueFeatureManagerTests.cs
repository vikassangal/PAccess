
using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class AutoCompleteNoLiabilityDueFeatureManagerTests
    {
        #region Defaut Scenario

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_IsAccountCreatedBeforeImplimentationDate_ShouldReturnFalse()
        {
            var autoCompleteNoLiabilityDueFeatureManager = new AutoCompleteNoLiabilityDueFeatureManager();
            var actualResult = autoCompleteNoLiabilityDueFeatureManager.IsAccountCreatedBeforeImplementationDate(null);

            Assert.IsFalse(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_IsAccountCreatedAfterImplimentationDate_ShouldReturnFalse()
        {
            var autoCompleteNoLiabilityDueFeatureManager = new AutoCompleteNoLiabilityDueFeatureManager();
            var actualResult = autoCompleteNoLiabilityDueFeatureManager.IsAccountCreatedAfterImplementationDate(null);

            Assert.IsFalse(actualResult);
        }
   
        #endregion

        [Test]
        public void TestAutoCompleteNoLiabilityFeatureEnabled_WhenFacilityIsICE_AndAccountCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var autoAutoCompleteNoLiabilityDueFeatureManager = new AutoCompleteNoLiabilityDueFeatureManager();
            
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient);
            account.Facility = ICEFacility;
            var actualResult = autoAutoCompleteNoLiabilityDueFeatureManager.IsAccountCreatedAfterImplementationDate(account);

            Assert.IsTrue(actualResult);
        }
       
        [Test]
        public void TestAutoCompleteNoLiabilityFeatureEnabled_WhenFacilityIsNotICE_AndAccountCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var autoAutoCompleteNoLiabilityDueFeatureManager = new AutoCompleteNoLiabilityDueFeatureManager();
           
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient);
            account.Facility = DHFFacility;
            var actualResult = autoAutoCompleteNoLiabilityDueFeatureManager.IsAccountCreatedAfterImplementationDate(account);

            Assert.IsFalse(actualResult);
        }
        [Test]
        public void TestAutoCompleteNoLiabilityFeatureEnabled_WhenFacilityIsICE_AndAccountCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            var autoAutoCompleteNoLiabilityDueFeatureManager = new AutoCompleteNoLiabilityDueFeatureManager();

            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient);
            account.Facility = ICEFacility;
            var actualResult = autoAutoCompleteNoLiabilityDueFeatureManager.IsAccountCreatedBeforeImplementationDate(account);

            Assert.IsTrue(actualResult);
        }
        
        private static Account GetAccount(Activity activity, DateTime accountCreatedDate,
            VisitType kindofVisit)
        {
            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = kindofVisit
               
            };
        }
        private Facility ICEFacility
        {
            get
            {
                var facility = new Facility(98,
                    PersistentModel.NEW_VERSION,
                    "ICE",
                    "ICE");
                facility["AutoCompleteNoLiabilityDueEnabled"] = true;
                return facility;
            }
        }

        private Facility DHFFacility
        {

            get
            {
                var facility = new Facility(54,
                    PersistentModel.NEW_VERSION,
                    "DHF",
                    "DHF");
                facility["AutoCompleteNoLiabilityDueEnabled"] = null;
                return facility;
            }
        }

        #region Constants
        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new AutoCompleteNoLiabilityDueFeatureManager().FeatureStartDate.AddDays(10);
        }
        private static DateTime GetTestDateBeforeFeatureStart()
        {
            return new AutoCompleteNoLiabilityDueFeatureManager().FeatureStartDate.AddDays(-10);
        }
        #endregion
    }
}
