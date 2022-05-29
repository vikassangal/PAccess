using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class MonthlyDueDateFeatureManagerrTests
    {
        [SetUp]
        public void SetUpFeatureManager()
        {
            monthlyDueDateFeatureManager = new MonthlyDueDateFeatureManager();
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullFacility_ShouldReturnFalse()
        {

            var actualResult = monthlyDueDateFeatureManager.IsMonthlyDueDateEnabled(null);
            Assert.IsFalse(actualResult);
        }

        [Test]
        public void TestRegistrationEmailReasonVisible_ICEFacility_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity());
            Assert.IsFalse(monthlyDueDateFeatureManager.IsMonthlyDueDateEnabled(ICEFacility));
        }

        [Test]
        public void TestRegistrationEmailReasonVisible_DHFFacility_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity());
            Assert.IsTrue(monthlyDueDateFeatureManager.IsMonthlyDueDateEnabled(DHFFacility));
        }
        
        #region Support Methods
        private static Account GetAccount(Activity activity)
        {
            return new Account
            {
                Activity = activity, 
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
                facility["MonthlyDueDateEnabled"] = null;
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
                facility["MonthlyDueDateEnabled"] = true;
                return facility;
            }
        }

        #endregion

        #region constants

        private MonthlyDueDateFeatureManager monthlyDueDateFeatureManager;

        #endregion
        #region Data Elements 
        #endregion
    }

}
