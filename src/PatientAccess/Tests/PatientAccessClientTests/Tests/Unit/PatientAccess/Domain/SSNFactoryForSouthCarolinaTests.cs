using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category("Fast")]
    public class SSNFactoryForSouthCarolinaTests
    {
        #region Constants
        private const string INVALID_UNKNOWN_SSN = "123456789";

        #endregion

        #region Test Methods

        #region Test GetValidSocialSecurityNumberFor Method
        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForSouthCarolina_WhenStatusIsNone()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.SouthCarolina, ADMIT_DATE, SocialSecurityNumberStatus.NoneSSNStatus);
            Assert.IsTrue(SocialSecurityNumber.IsSouthCarolinaNone(ssn.ToString()), "ssn should be 000000009");
        }
        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForSouthCarolina_WhenStatusIsUnknown()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.SouthCarolina, ADMIT_DATE, SocialSecurityNumberStatus.UnknownSSNStatus);
            Assert.IsTrue(SocialSecurityNumber.IsSouthCarolinaUnknown(ssn.ToString()), "ssn should be 000000001");
        }
        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForSouthCarolinaWhenStatusIsNewborn()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.SouthCarolina, ADMIT_DATE, SocialSecurityNumberStatus.NewbornSSNStatus);
            Assert.IsTrue(SocialSecurityNumber.IsSouthCarolinaNewBorn(ssn.ToString()), "ssn should be 000000002");
        }
        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForSouthCarolinaWhenStatusIsRefused()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.SouthCarolina, ADMIT_DATE, SocialSecurityNumberStatus.RefusedSSNStatus);
            Assert.IsTrue(SocialSecurityNumber.IsSouthCarolinaRefused(ssn.ToString()), "ssn should be 000000009");
        }
        #endregion

        #region Test IsSSNValidForSSNStatus

        [Test]
        public void TestIsSSNValidForSSNStatus_SouthCarolina_Unknown()
        {
            var result = ssnFactory.IsSSNValidForSSNStatus(
                SocialSecurityNumber.SOUTHCAROLINA_UNKNOWN_SSN, SocialSecurityNumberStatus.UnknownSSNStatus);
            Assert.IsTrue(result, "valid");
        }
      
        #endregion

        #region Test GetValidatedSocialSecurityNumberUsing

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaNone_ShouldReturn000000009AndNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaNoneRefusedSSN, State.SouthCarolina, ADMIT_DATE, 10);

            Assert.IsTrue(SocialSecurityNumber.IsSouthCarolinaNone(ssn.UnformattedSocialSecurityNumber));
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaNewborn_ForBelow2Yrs_ShouldReturn000000002AndNewbornStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaNewbornSSN, State.SouthCarolina, ADMIT_DATE, 1);

            Assert.IsTrue(SocialSecurityNumber.IsSouthCarolinaNewBorn(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsNewbornSSNStatus);
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaNewborn_ForAbove2Yrs_ShouldReturnknownStatus
            ()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaNewbornSSN, State.SouthCarolina, ADMIT_DATE, 10); 
            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaUnknown_ForAge10Yrs_ShouldReturn000000001AndUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaUnknownSSN, State.SouthCarolina, ADMIT_DATE, 10);

            Assert.IsTrue(SocialSecurityNumber.IsSouthCarolinaUnknown(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaUnknown_ForAge1Yr_ShouldReturn00000001AndNewbornStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaUnknownSSN, State.SouthCarolina, ADMIT_DATE, 1);

            Assert.IsTrue(ssn.SSNStatus.IsNewbornSSNStatus);
        }
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaOldUnknown999999999_ForAge1Yr_ShouldReturnNewBornStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaOldUnknownSSN, State.SouthCarolina, ADMIT_DATE, 1);

            Assert.IsTrue(ssn.SSNStatus.IsNewbornSSNStatus);
        }
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaOldUnknown999999999_ForAge1Yr_ShouldReturn000000002Number()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaOldUnknownSSN, State.SouthCarolina, ADMIT_DATE, 1);

            Assert.IsTrue(ssn.Equals(SocialSecurityNumber.SOUTHCAROLINA_NEWBORN_SSN));
        }
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaOldUnknown999999999_ForAge3Yr_ShouldReturnUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaOldUnknownSSN, State.SouthCarolina, ADMIT_DATE, 3);

            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaOldUnknown999999999_ForAge3Yr_ShouldReturn000000001Number()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaOldUnknownSSN, State.SouthCarolina, ADMIT_DATE, 3);

            Assert.IsTrue(ssn.Equals(SocialSecurityNumber.SouthCarolinaUnknownSSN));
        }
        #endregion

        #region Test GetValidatedSocialSecurityNumberUsingwithoutAge_ForGuarantorViews

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaNewborn_ForGuarantor_ShouldReturnUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaNewbornSSN, State.SouthCarolina, ADMIT_DATE);

            Assert.IsTrue(SocialSecurityNumber.IsSouthCarolinaUnknown(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForSouthCarolinaUnknown_NoAge_ShouldReturnUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.SouthCarolinaUnknownSSN, State.SouthCarolina, ADMIT_DATE);

            Assert.IsTrue(SocialSecurityNumber.IsSouthCarolinaUnknown(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }

        #endregion

        #endregion

        #region Data Elements
        private readonly SouthCarolinaSsnFactory ssnFactory = new SouthCarolinaSsnFactory();
        private readonly DateTime ADMIT_DATE = new DateTime(2010, 11, 11);
        #endregion
    }
}
