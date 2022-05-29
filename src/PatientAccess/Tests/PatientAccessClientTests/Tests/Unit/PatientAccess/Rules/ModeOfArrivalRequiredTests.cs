using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ModeOfArrivalRequiredTests
    /// </summary>
    [TestFixture]
    public class ModeOfArrivalRequiredTests
    {
        #region Test Methods
        [Test]
        [Category( "Fast" )]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new ModeOfArrivalRequired();
            object inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );

            Assert.IsTrue( actualResult );
        }

        [Test]
        [Category( "Fast" )]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        [Ignore()]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_FacilityIsCA_CreatedAfterReleaseDate_PT3_BlankModeOFArrival_ShouldReturnFalse()
        {
            var account = GetAccount( new RegistrationActivity(), VALID_FACILITY, TestDateAfterModeOfArrivalRequiredReleaseDate, ERPatientType, blankModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_FacilityIsCA_CreatedAfterReleaseDate_PT3_ValueModeOFArrival_ShouldReturnTrue()
        {
            var account = GetAccount( new RegistrationActivity(), VALID_FACILITY, TestDateAfterModeOfArrivalRequiredReleaseDate, ERPatientType, ValueModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_FacilityIsCA_CreatedAfterReleaseDate_PT2_BlankModeOFArrival_ShouldReturnTrue()
        {
            var account = GetAccount( new RegistrationActivity(), VALID_FACILITY, TestDateAfterModeOfArrivalRequiredReleaseDate, OPPatientType, blankModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_FacilityIsCA_CreatedBeforeReleaseDate_PT3_BlankModeOFArrival_ShouldReturnTrue()
        {
            var account = GetAccount( new RegistrationActivity(), VALID_FACILITY, TestDateBeforeModeOfArrivalRequiredReleaseDate, ERPatientType, blankModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_FacilityIsNonCA_CreatedAfterReleaseDate_PT3_BlankModeOFArrival_ShouldReturnTrue()
        {
            var account = GetAccount( new RegistrationActivity(), INVALID_FACILITY, TestDateAfterModeOfArrivalRequiredReleaseDate, ERPatientType, blankModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        [Ignore()]
        public void TestCanBeAppliedTo_WhenActivityPostMSERegistration_FacilityIsCA_CreatedAfterReleaseDate_PT3_BlankModeOFArrival_ShouldReturnFalse()
        {
            var account = GetAccount( new PostMSERegistrationActivity(), VALID_FACILITY, TestDateAfterModeOfArrivalRequiredReleaseDate, ERPatientType, blankModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityPostMSERegistration_FacilityIsNonCA_CreatedAfterReleaseDate_PT3_BlankModeOFArrival_ShouldReturnTrue()
        {
            Account account = GetAccount( new PostMSERegistrationActivity(), INVALID_FACILITY, TestDateAfterModeOfArrivalRequiredReleaseDate, ERPatientType, blankModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityPostMSERegistration_FacilityIsCA_CreatedBeforeReleaseDate_PT3_BlankModeOFArrival_ShouldReturnTrue()
        {
            var account = GetAccount( new PostMSERegistrationActivity(), VALID_FACILITY, TestDateBeforeModeOfArrivalRequiredReleaseDate, ERPatientType, blankModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityPostMSERegistration_FacilityIsCA_CreatedAfterReleaseDate_PT2_BlankModeOFArrival_ShouldReturnTrue()
        {
            var account = GetAccount( new PostMSERegistrationActivity(), VALID_FACILITY, TestDateAfterModeOfArrivalRequiredReleaseDate, OPPatientType, blankModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityPostMSERegistration_FacilityIsCA_CreatedAfterReleaseDate_PT2_ValueModeOFArrival_ShouldReturnTrue()
        {
            var account = GetAccount( new PostMSERegistrationActivity(), VALID_FACILITY, TestDateAfterModeOfArrivalRequiredReleaseDate, OPPatientType, ValueModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityPostMSERegistration_FacilityIsCA_CreatedDateisMin_PT3_ValueModeOFArrival_ShouldReturnTrue()
        {
            var account = GetAccount( new PostMSERegistrationActivity(), VALID_FACILITY, DateTime.MinValue, ERPatientType, ValueModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityPreMSERegistration_FacilityIsCA_CreatedDateisMin_ValueModeOFArrival_ShouldReturnTrue()
        {
            var account = GetAccount( new PreMSERegisterActivity(), VALID_FACILITY, DateTime.MinValue, ERPatientType, ValueModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        [Ignore()]
        public void TestCanBeAppliedTo_WhenActivityPreMSERegistration_FacilityIsCA_CreatedDateisMin_BlankModeOFArrival_ShouldReturnFalse()
        {
            var account = GetAccount( new PreMSERegisterActivity(), VALID_FACILITY, DateTime.MinValue, ERPatientType, blankModeOfArrival );
            var ruleUnderTest = new ModeOfArrivalRequired();

            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        #endregion

        #region Support Methods

        private static Account GetAccount( Activity activity, string facilityCode, DateTime accountCreatedDate, VisitType patientType, ModeOfArrival modeOfArrival )
        {
            Account account = new Account
                                  {
                                      Activity = activity,
                                      KindOfVisit = patientType,
                                      AccountCreatedDate = accountCreatedDate,
                                      ModeOfArrival = modeOfArrival,
                                      Facility = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith( facilityCode )
                                  };

            return account;
        }

        private readonly VisitType ERPatientType =
            new VisitType( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, VisitType.EMERGENCY_PATIENT_DESC, VisitType.EMERGENCY_PATIENT );

        private readonly VisitType OPPatientType =
            new VisitType( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT );

        private readonly ModeOfArrival blankModeOfArrival =
            new ModeOfArrival( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, String.Empty, String.Empty );

        private readonly ModeOfArrival ValueModeOfArrival =
            new ModeOfArrival( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "Hospital", "01" );

        #endregion

        #region Constants

        private readonly DateTime TestDateAfterModeOfArrivalRequiredReleaseDate = DateTime.Parse( "01-05-2011" );
        private readonly DateTime TestDateBeforeModeOfArrivalRequiredReleaseDate = DateTime.Parse( "01-01-2009" );
        private const string VALID_FACILITY = "ACO";
        private const string INVALID_FACILITY = "DHF";

        #endregion
    }
}
