using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class SSNFactoryTests
    {
        #region Constants

        private const string INVALID_UNKNOWN_SSN    = "123456789";
        private const string VALID_SSN              = "111223333";
        private const string INVALID_SSN            = "1112223333";
        #endregion

        #region Test Methods

        #region Test GetValidSocialSecurityNumberFor Method
        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForFloridaNone_Pre01012010_SsnShouldBe5555555555()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor( 
                State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.NoneSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsFloridaNoneOldSSN( ssn.ToString() ), "ssn should be 555555555" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForFloridaUnknown_Pre01012010_SsnShouldBe777777777()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.UnknownSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsFloridaUnknownSSNPre01012010( ssn.ToString() ), "ssn should be 777777777" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForFloridaNewborn_Pre01012010_SsnShouldBe000000000()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.NewbornSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsFloridaNewbornOldSSN( ssn.ToString() ), "ssn should be 000000000" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForFloridaNewborn_Post01012010_SsnShouldBe777777777()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.NewbornSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsFloridaUnknownSSNPre01012010( ssn.UnformattedSocialSecurityNumber ), 
                "ssn should be 777-77-7777" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForFloridaNewbornWithNoAdmitDate_SsnShouldBe777777777()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.Florida, DateTime.MinValue, SocialSecurityNumberStatus.NewbornSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsFloridaUnknownSSNPre01012010( ssn.UnformattedSocialSecurityNumber ), 
                "ssn should be 777-77-7777" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForFloridaUnknownPost01012010_SsnShouldBe777777777()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.UnknownSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsFloridaUnknownSSNPre01012010( ssn.UnformattedSocialSecurityNumber ), "ssn should be 777-77-7777" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberForWithNoneStatus_ForCalifornia_SsnShouldBe000000001()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.California, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.NoneSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn.UnformattedSocialSecurityNumber ), "ssn status should be same" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberForWithUnKnownStatus_ForCalifornia_SsnShouldBe000000001()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.California, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.UnknownSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn.UnformattedSocialSecurityNumber ), "ssn status should be same" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberForWithNoneStatus_ForNonFloridaNonCalifornia_SsnShouldBe000000001()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.NonFloridaNonCalifornia, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.NoneSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn.UnformattedSocialSecurityNumber ), "ssn status should be same" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberForWithUnKnownStatus_ForNonFloridaNonCalifornia_SsnShouldBe999999999()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.NonFloridaNonCalifornia, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.UnknownSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsNonFloridaNewbornUnknownSSN( ssn.UnformattedSocialSecurityNumber ) );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberForWithNewbornStatus_ForNonFloridaNonCalifornia_SsnShouldBe999999999()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.NonFloridaNonCalifornia, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, SocialSecurityNumberStatus.NewbornSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsNonFloridaNewbornUnknownSSN( ssn.UnformattedSocialSecurityNumber ) );
        }
        #endregion

        #region Test IsSocialSecurityNumberValid

        [Test]
        public void TestIsSocialSecurityNumberValidForFloridaUnknown_Pre01012010()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus(
                SocialSecurityNumber.FLORIDA_UNKNOWN_PRE_01012010_SSN, UnknownSSNStatus, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE);
            bool invalid = ssnFactory.IsSSNValidForSSNStatus( 
                INVALID_UNKNOWN_SSN, UnknownSSNStatus, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE);

            Assert.IsTrue( valid, "valid" );
            Assert.IsTrue( !invalid, "invalid" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValidForFloridaNone_Pre01012010()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus( SocialSecurityNumber.FLORIDA_NONE_SSN, NoneSSNStatus, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue( valid, "valid" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValidForFloridaNewborn_Pre01012010()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus( SocialSecurityNumber.FLORIDA_NEWBORN_SSN, NewbornSSNStatus, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue( valid, "valid" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValid_ForKnownStatus()
        {
            bool isValid = ssnFactory.IsSSNValidForSSNStatus( VALID_SSN, KnownSSNStatus, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue( isValid, "SSN Should be valid for Known Status" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValid_ForFloridaWhenAdmitDateIsMinValue()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus( SocialSecurityNumber.FLORIDA_UNKNOWN_NONE_NEWBORN_POST_01012010_SSN,
                            UnknownSSNStatus, State.Florida, DateTime.MinValue);
            bool invalid = ssnFactory.IsSSNValidForSSNStatus(
                            INVALID_UNKNOWN_SSN, UnknownSSNStatus, State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);

            Assert.IsTrue( valid, "valid" );
            Assert.IsTrue( !invalid, "invalid" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValid_ForFloridaPost01012010()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus( SocialSecurityNumber.FLORIDA_UNKNOWN_NONE_NEWBORN_POST_01012010_SSN,
                            UnknownSSNStatus, State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);
            bool invalid = ssnFactory.IsSSNValidForSSNStatus(
                            INVALID_UNKNOWN_SSN, UnknownSSNStatus, State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);

            Assert.IsTrue( valid, "valid" );
            Assert.IsTrue( !invalid, "invalid" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValidFor_ForCaliforniaAndNoneSSNStatus_WithValidSSN_ShouldReturnTrue()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus( 
                SocialSecurityNumber.NON_FLORIDA_NONE_SSN, NoneSSNStatus, State.California, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue( valid, "valid" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValidFor_ForCaliforniaAndNoneSSNStatus_WithInvalidSSN_ShouldReturnFalse()
        {
            bool invalid = ssnFactory.IsSSNValidForSSNStatus(
                INVALID_SSN, NoneSSNStatus, State.California, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsFalse( invalid, "Should be False" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValidFor_ForCaliforniaAndUnknownSSNStatus_WithValidSSN_ShouldReturnTrue()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus( SocialSecurityNumber.NON_FLORIDA_NONE_SSN, 
                UnknownSSNStatus, State.California, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue( valid, "valid" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValidFor_ForCaliforniaAndUnknownSSNStatus_WithInvalidSSN_ShouldReturnFalse()
        {
            bool invalid = ssnFactory.IsSSNValidForSSNStatus(
                INVALID_SSN, UnknownSSNStatus, State.California, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsFalse( invalid, "Should be False" );
        }
        #endregion

        #region Test GetValidatedSocialSecurityNumberUsing

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForFloridaNonePre01012010_ShouldReturn555555555AndNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing( 
                SocialSecurityNumber.FloridaNoneSSN, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, 2);

            Assert.IsTrue( SocialSecurityNumber.IsFloridaNoneOldSSN( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsNoneSSNStatus );
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForFloridaNonePost01012010_ForAgeAbove2Yrs_ShouldReturn777777777AndNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.FloridaNoneSSN, State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE, 3);

            Assert.IsTrue( SocialSecurityNumber.IsFloridaUnknownNoneNewbornSSNPost01012010( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsNoneSSNStatus );
        }

        [Test]
        public void
            TestGetValidatedSocialSecurityNumber_ForFloridaNone_WhenAdmitDateIsMinValue_AgeBelow2Yrs_ShouldReturn777777777AndNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.FloridaNoneSSN, State.Florida, DateTime.MinValue, 0);

            Assert.IsTrue( SocialSecurityNumber.IsFloridaUnknownNoneNewbornSSNPost01012010( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsNoneSSNStatus );
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForFloridaNonePost01012010_ForAgeBelow2Yrs_ShouldReturn777777777AndNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.FloridaNoneSSN, State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE, 1);

            Assert.IsTrue( SocialSecurityNumber.IsFloridaUnknownNoneNewbornSSNPost01012010( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsNoneSSNStatus );
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForFloridaNewbornPost01012010_ForAge2Yrs_ShouldReturn777777777AndNewbornStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.FloridaNewbornSSN, State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE, 2);

            Assert.IsTrue( SocialSecurityNumber.IsFloridaUnknownNoneNewbornSSNPost01012010( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsNewbornSSNStatus );
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForFloridaUnknownPre01012010_ShouldReturn777777777AndUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.FloridaUnknownPre01012010SSN, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, 2);

            Assert.IsTrue( SocialSecurityNumber.IsFloridaUnknownSSNPre01012010( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsUnknownSSNStatus );
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForFloridaKnownPre01012010_ShouldReturnKnownSSNAndKnownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                new SocialSecurityNumber( VALID_SSN ), State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE, 2);

            Assert.IsTrue(ssnFactory.IsKnownSSN(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue( ssn.SSNStatus.IsKnownSSNStatus );
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForCaliforniaNone_ShouldReturn000000001AndUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.NonFloridaNoneSSN, State.California, POST_FLORIDA_AHCA_RULE_ADMIT_DATE, 2);

            Assert.IsTrue( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsUnknownSSNStatus );
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForNonCaliforniaNonFloridaNone_ShouldReturn000000001AndNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.NonFloridaNoneSSN, State.NonFloridaNonCalifornia, POST_FLORIDA_AHCA_RULE_ADMIT_DATE, 2);

            Assert.IsTrue( SocialSecurityNumber.IsNonFloridaNoneSSN( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsNoneSSNStatus );
        }

        #endregion
        #region Test GetValidatedSocialSecurityNumberUsingWithoutAge
 
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForFloridaNonePost01012010_NoAge_ShouldReturn777777777AndNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.FloridaNoneSSN, State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);

            Assert.IsTrue(SocialSecurityNumber.IsFloridaUnknownNoneNewbornSSNPost01012010(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsNoneSSNStatus);
        }


        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForFloridaUnknown_NoAge_Pre01012010_ShouldReturn777777777AndUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.FloridaUnknownPre01012010SSN, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE);

            Assert.IsTrue(SocialSecurityNumber.IsFloridaUnknownSSNPre01012010(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForFloridaKnownNoAge_ShouldReturnKnownSSNAndKnownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                new SocialSecurityNumber(VALID_SSN), State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE);

            Assert.IsTrue(ssnFactory.IsKnownSSN(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsKnownSSNStatus);
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForCaliforniaNone_NoAge_ShouldReturn000000001AndUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.NonFloridaNoneSSN, State.California, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);

            Assert.IsTrue(SocialSecurityNumber.IsNonFloridaNoneSSN(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForNonCaliforniaNewborn_NoAge_ShouldReturn000000001AndNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.NonFloridaNewbornOrUnknownSSN, State.NonFloridaNonCalifornia, POST_FLORIDA_AHCA_RULE_ADMIT_DATE, 2);

            Assert.IsTrue(SocialSecurityNumber.IsNonFloridaNewbornUnknownSSN(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }

        #endregion
        #region Test GetSSNStatusForUnvalidatedSSNUsing_ForGuarantorViews

        [Test]
        public void TestGetSSNStatusForUnvalidatedSSNUsing_ForGuarantorViews_ForFloridaNonePost01012010_NoAge_ShouldReturn777777777AndNoneStatus()
        {
            var ssn = ssnFactory.GetSSNStatusForUnvalidatedSSNUsing(
                SocialSecurityNumber.FloridaNoneSSN.UnformattedSocialSecurityNumber, State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);

            Assert.IsTrue(ssn.IsNoneSSNStatus);
        }


        [Test]
        public void TestGetSSNStatusForUnvalidatedSSNUsing_ForFloridaUnknown_NoAge_Pre01012010_ShouldReturn777777777AndUnknownStatus()
        {
            var ssn = ssnFactory.GetSSNStatusForUnvalidatedSSNUsing(
                SocialSecurityNumber.FloridaUnknownPre01012010SSN.UnformattedSocialSecurityNumber, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue(ssn.IsUnknownSSNStatus);
        }

        [Test]
        public void TestGetSSNStatusForUnvalidatedSSNUsing_ForFloridaKnownNoAge_ShouldReturnKnownSSNAndKnownStatus()
        {
            var ssn = ssnFactory.GetSSNStatusForUnvalidatedSSNUsing(
                new SocialSecurityNumber(VALID_SSN).UnformattedSocialSecurityNumber, State.Florida, PRE_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue(ssn.IsKnownSSNStatus);
        }
        [Test]
        public void TestGetSSNStatusForUnvalidatedSSNUsing_ForFloridaNewbornNoAge_ShouldReturnKnownSSNAndNoneStatus()
        {
            var ssn = ssnFactory.GetSSNStatusForUnvalidatedSSNUsing(
               SocialSecurityNumber.FloridaUnknownNoneNewbornPost01012010SSN.UnformattedSocialSecurityNumber, State.Florida, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue(ssn.IsUnknownSSNStatus);
        }
        
        [Test]
        public void TestGetSSNStatusForUnvalidatedSSNUsing_ForCaliforniaNone_NoAge_ShouldReturn000000001AndUnknownStatus()
        {
            var ssn = ssnFactory.GetSSNStatusForUnvalidatedSSNUsing(
                SocialSecurityNumber.NonFloridaNoneSSN.UnformattedSocialSecurityNumber, State.California, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue(ssn.IsUnknownSSNStatus);
        }

        [Test]
        public void TestGetSSNStatusForUnvalidatedSSNUsing_ForNonCaliforniaNewborn_NoAge_ShouldReturn000000001AndNoneStatus()
        {
            var ssn = ssnFactory.GetSSNStatusForUnvalidatedSSNUsing(
                SocialSecurityNumber.NonFloridaNewbornOrUnknownSSN.UnformattedSocialSecurityNumber, State.NonFloridaNonCalifornia, POST_FLORIDA_AHCA_RULE_ADMIT_DATE);
            Assert.IsTrue(ssn.IsUnknownSSNStatus);
        }

        #endregion
        #endregion

        #region Properties

        private static SocialSecurityNumberStatus KnownSSNStatus
        {
            get
            {
                return SocialSecurityNumberStatus.KnownSSNStatus;
            }
        }

        private static SocialSecurityNumberStatus UnknownSSNStatus
        {
            get
            {
                return SocialSecurityNumberStatus.UnknownSSNStatus;
            }
        }

        private static SocialSecurityNumberStatus NoneSSNStatus
        {
            get
            {
                return SocialSecurityNumberStatus.NoneSSNStatus;
            }
        }

        private static SocialSecurityNumberStatus NewbornSSNStatus
        {
            get
            {
                return SocialSecurityNumberStatus.NewbornSSNStatus;
            }
        }

        #endregion

        #region Data Elements

        private readonly SSNFactory ssnFactory = new SSNFactory();

        private readonly DateTime PRE_FLORIDA_AHCA_RULE_ADMIT_DATE = new DateTime( 2001, 01, 01 );
        private readonly DateTime POST_FLORIDA_AHCA_RULE_ADMIT_DATE = new DateTime( 2010, 11, 11 );
        #endregion
    }
}
