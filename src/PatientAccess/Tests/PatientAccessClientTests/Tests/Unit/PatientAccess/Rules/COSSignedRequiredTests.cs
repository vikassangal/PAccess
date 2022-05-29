using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for COSSignedRequiredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class COSSignedRequiredTests
    {
        #region SetUp COSSignedRequiredTests
        [TestFixtureSetUp()]
        public static void SetUpConditionOfServiceBrokerTests()
        {
            i_cosbroker = BrokerFactory.BrokerOfType<IConditionOfServiceBroker>();
        }

        #endregion

        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationActivity_PT9_BlankCOSSigned_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), String.Empty);
            account.KindOfVisit = new VisitType(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION, "PATIENT TYPE 9", VisitType.NON_PATIENT);
            var ruleUnderTest = new COSSignedRequired();
            var result = ruleUnderTest.CanBeAppliedTo(account);
            Assert.IsFalse( result );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationActivity_HSVisSP_BlankCOSSigned_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), String.Empty);
            account.HospitalService = SP;
            var ruleUnderTest = new COSSignedRequired();
            var result = ruleUnderTest.CanBeAppliedTo(account);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreMSEActivity_PT9_BlankCOSSigned_ShouldReturnTrue()
        {
            var account = GetAccount(new PreMSERegisterActivity(), String.Empty);
            account.KindOfVisit = new VisitType(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION, "PATIENT TYPE 9", VisitType.NON_PATIENT);
            var ruleUnderTest = new COSSignedRequired();
            var result = ruleUnderTest.CanBeAppliedTo(account);
            Assert.IsTrue(result);
        }

       #endregion

        #region Support Methods

        private static Account GetAccount(Activity activity, String COSCode)
        {
           var account = new Account
                       {
                           Activity = activity,
                           COSSigned = i_cosbroker.ConditionOfServiceWith(COSCode)
                       };
            return account;

        }

        #endregion

        #region Data Elements
        private static IConditionOfServiceBroker i_cosbroker;
        private readonly HospitalService SP = new HospitalService
        {
            Code = HospitalService.SPECIMEN_ONLY,
            Description = "SPECIMEN ONLY"
        };
        #endregion
    }
}
