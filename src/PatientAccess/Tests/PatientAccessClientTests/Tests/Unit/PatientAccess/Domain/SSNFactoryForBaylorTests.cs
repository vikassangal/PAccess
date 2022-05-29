using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class SSNFactoryForBaylorTests
    {
        #region Constants
        private const string INVALID_UNKNOWN_SSN = "123456789";
        
        #endregion

        #region Test Methods

        #region Test GetValidSocialSecurityNumberFor Method
        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForBaylorNone ()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.NonFloridaNonCalifornia, ADMIT_DATE , SocialSecurityNumberStatus.NoneSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsBaylorNoneNewBornSSN( ssn.ToString() ), "ssn should be 000000000" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForBaylorUnknown()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.NonFloridaNonCalifornia, ADMIT_DATE, SocialSecurityNumberStatus.UnknownSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsBaylorUnknownSSN( ssn.ToString() ), "ssn should be 999999999" );
        }

        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForBaylorNewborn ()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.NonFloridaNonCalifornia, ADMIT_DATE, SocialSecurityNumberStatus.NewbornSSNStatus);
            Assert.IsTrue( SocialSecurityNumber.IsBaylorNoneNewBornSSN( ssn.ToString() ), "ssn should be 000000000" );
        }
        [Test]
        public void TestGetValidSocialSecurityNumberFor_ForBaylorRefused()
        {
            SocialSecurityNumber ssn = ssnFactory.GetValidSocialSecurityNumberFor(
                State.NonFloridaNonCalifornia, ADMIT_DATE, SocialSecurityNumberStatus.RefusedSSNStatus);
            Assert.IsTrue(SocialSecurityNumber.IsBaylorRefusedSSN(ssn.ToString()), "ssn should be 888888888");
        }

       
      
      
        #endregion

        #region Test IsSocialSecurityNumberValid

        [Test]
        public void TestIsSocialSecurityNumberValidForBaylorUnknown()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus(
                SocialSecurityNumber.BAYLOR_UNKNOWN_SSN, SocialSecurityNumberStatus.UnknownSSNStatus, State.NonFloridaNonCalifornia, ADMIT_DATE);
            bool invalid = ssnFactory.IsSSNValidForSSNStatus(
                INVALID_UNKNOWN_SSN, SocialSecurityNumberStatus.UnknownSSNStatus, State.NonFloridaNonCalifornia, ADMIT_DATE);

            Assert.IsTrue( valid, "valid" );
            Assert.IsTrue( !invalid, "invalid" );
        }

        [Test]
        public void TestIsSocialSecurityNumberValidForBaylorNone()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus(
                SocialSecurityNumber.BAYLOR_NONE_NEWBORN_SSN, SocialSecurityNumberStatus.NoneSSNStatus, State.NonFloridaNonCalifornia, ADMIT_DATE);
            bool invalid = ssnFactory.IsSSNValidForSSNStatus(
                INVALID_UNKNOWN_SSN, SocialSecurityNumberStatus.NoneSSNStatus, State.NonFloridaNonCalifornia, ADMIT_DATE);

            Assert.IsTrue(valid, "valid");
            Assert.IsTrue(!invalid, "invalid");
        }

        [Test]
        public void TestIsSocialSecurityNumberValidForBaylorNewborn()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus(
                SocialSecurityNumber.BAYLOR_NONE_NEWBORN_SSN, SocialSecurityNumberStatus.NewbornSSNStatus, State.NonFloridaNonCalifornia, ADMIT_DATE);
            bool invalid = ssnFactory.IsSSNValidForSSNStatus(
                INVALID_UNKNOWN_SSN, SocialSecurityNumberStatus.NewbornSSNStatus, State.NonFloridaNonCalifornia, ADMIT_DATE);

            Assert.IsTrue(valid, "valid");
            Assert.IsTrue(!invalid, "invalid");
        }
        [Test]
        public void TestIsSocialSecurityNumberValidForBaylorRefused()
        {
            bool valid = ssnFactory.IsSSNValidForSSNStatus(
                SocialSecurityNumber.BAYLOR_REFUSED_SSN, SocialSecurityNumberStatus.RefusedSSNStatus, State.NonFloridaNonCalifornia, ADMIT_DATE);
            bool invalid = ssnFactory.IsSSNValidForSSNStatus(
                INVALID_UNKNOWN_SSN, SocialSecurityNumberStatus.RefusedSSNStatus, State.NonFloridaNonCalifornia, ADMIT_DATE);

            Assert.IsTrue(valid, "valid");
            Assert.IsTrue(!invalid, "invalid");
        }
  
        #endregion

        #region Test GetValidatedSocialSecurityNumberUsing

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForBaylorNone_ShouldReturn000000000AndNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.BaylorNoneNewbornSSN, State.NonFloridaNonCalifornia, ADMIT_DATE, 10);

            Assert.IsTrue( SocialSecurityNumber.IsBaylorNoneNewBornSSN( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsNoneSSNStatus );
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForBaylorNewborn_ForAgeAbove1Yrs_ShouldReturn000000000AndNewbornStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.BaylorNoneNewbornSSN, State.NonFloridaNonCalifornia, ADMIT_DATE, 1);

            Assert.IsTrue( SocialSecurityNumber.IsBaylorNoneNewBornSSN( ssn.UnformattedSocialSecurityNumber ) );
            Assert.IsTrue( ssn.SSNStatus.IsNewbornSSNStatus );
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForBaylorUnknown_ForAgeAbove1Yrs_ShouldReturn999999999AndUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.BaylorUnknownSSN, State.NonFloridaNonCalifornia, ADMIT_DATE, 10);

            Assert.IsTrue(SocialSecurityNumber.IsBaylorUnknownSSN(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForBaylorRefused_ForAgeAbove1Yrs_ShouldReturn999999999AndUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.BaylorRefusedSSN, State.NonFloridaNonCalifornia, ADMIT_DATE, 10);

            Assert.IsTrue(SocialSecurityNumber.IsBaylorRefusedSSN(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsRefusedSSNStatus);
        }
        #endregion  
        #region Test GetValidatedSocialSecurityNumberUsingwithoutAge_ForGuarantorViews
     
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForBaylorNewborn_NoAge_ShouldReturn000000000AnNoneStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.BaylorNoneNewbornSSN, State.NonFloridaNonCalifornia, ADMIT_DATE);

            Assert.IsTrue(SocialSecurityNumber.IsBaylorNoneNewBornSSN(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsNoneSSNStatus);
        }

        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForBaylorUnknown_NoAge_ShouldReturn999999999AndUnknownStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.BaylorUnknownSSN, State.NonFloridaNonCalifornia, ADMIT_DATE);

            Assert.IsTrue(SocialSecurityNumber.IsBaylorUnknownSSN(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsUnknownSSNStatus);
        }
        [Test]
        public void TestGetValidatedSocialSecurityNumber_ForBaylorRefused_NoAge_ShouldReturn999999999AndRefusedStatus()
        {
            var ssn = ssnFactory.GetValidatedSocialSecurityNumberUsing(
                SocialSecurityNumber.BaylorRefusedSSN, State.NonFloridaNonCalifornia, ADMIT_DATE, 10);

            Assert.IsTrue(SocialSecurityNumber.IsBaylorRefusedSSN(ssn.UnformattedSocialSecurityNumber));
            Assert.IsTrue(ssn.SSNStatus.IsRefusedSSNStatus);
        }

        #endregion
        #region Test GetSSNStatusForUnvalidatedSSNUsing_ForGuarantorViews

        [Test]
        public void TestGetSSNStatusForUnvalidatedSSNUsing_ForBaylorNewborn_NoAge_ShouldReturnNoneStatus()
        {
            var ssn = ssnFactory.GetSSNStatusForUnvalidatedSSNUsing(
                SocialSecurityNumber.BaylorNoneNewbornSSN.UnformattedSocialSecurityNumber);
            Assert.IsTrue(ssn.IsNoneSSNStatus);
        }

        [Test]
        public void TestGetSSNStatusForUnvalidatedSSNUsing_ForBaylorUnknown_NoAge_ShouldReturn999999999AndUnknownStatus()
        {
            var ssn = ssnFactory.GetSSNStatusForUnvalidatedSSNUsing(
                SocialSecurityNumber.BaylorUnknownSSN.UnformattedSocialSecurityNumber);
            Assert.IsTrue(ssn.IsUnknownSSNStatus);
        }
        [Test]
        public void TestGetSSNStatusForUnvalidatedSSNUsing_ForBaylorRefused_NoAge_ShouldReturn999999999AndRefusedStatus()
        {
            var ssn = ssnFactory.GetSSNStatusForUnvalidatedSSNUsing(
                SocialSecurityNumber.BaylorRefusedSSN.UnformattedSocialSecurityNumber);
            Assert.IsTrue(ssn.IsRefusedSSNStatus);
        }


        #endregion
        #endregion
   
        #region Properties
        #endregion

        #region Data Elements
        private readonly BaylorSsnFactory ssnFactory = new BaylorSsnFactory();
        private readonly DateTime  ADMIT_DATE = new DateTime( 2010, 11, 11 );
        #endregion
    }
}
