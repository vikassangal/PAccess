using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for RightCareRightPlaceRequiredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class RightCareRightPlaceRequiredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            RightCareRightPlaceRequired ruleUnderTest = new RightCareRightPlaceRequired();
            object inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new RightCareRightPlaceRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new RightCareRightPlaceRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndCreatedAfterReleaseDateValidPT_ShouldReturnTrue()
        {
            Account account = GetAccountWithPatientType3(new RegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate , new YesNoFlag(YesNoFlag.CODE_YES));
            var ruleUnderTest = new RightCareRightPlaceRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPostMSERegistrationAndCreatedDateAfterReleaseDateValidPTNullRCRP_ShouldReturnTrue()
        {
            var account = GetAccountWithPatientType3(new PostMSERegistrationActivity(), TestDateAfterRCRPReleaseDate, DateTime.Today, null);
            var ruleUnderTest = new RightCareRightPlaceRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPostMSERegistrationAndCreatedDateAfterReleaseDateValidPT_ShouldReturnFalse()
        {
            var account = GetAccountWithPatientType3(new PostMSERegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate , new YesNoFlag(YesNoFlag.CODE_BLANK));
            var ruleUnderTest = new RightCareRightPlaceRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndCreatedDateBeforeReleaseDateValidPT_ShouldReturnTrue()
        {
            var account = GetAccountWithPatientType3(new RegistrationActivity(), TestDateBeforeRCRPReleaseDate, TestDateAfterRCRPReleaseDate , new YesNoFlag(YesNoFlag.CODE_YES));
            var ruleUnderTest = new RightCareRightPlaceRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPostRegistrationAndCreatedDateBeforeReleaseDateValidPT_ShouldReturnTrue()
        {
            var account = GetAccountWithPatientType3(new PostMSERegistrationActivity(), TestDateBeforeRCRPReleaseDate, TestDateAfterRCRPReleaseDate , new YesNoFlag(YesNoFlag.CODE_YES));
            var ruleUnderTest = new RightCareRightPlaceRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPostRegistrationAndCreatedDateAfterReleaseDateInValidPT_ShouldReturnTrue()
        {
            var account = GetAccountWithPatientType2(new PostMSERegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate , new YesNoFlag(YesNoFlag.CODE_YES));
            var ruleUnderTest = new RightCareRightPlaceRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndCreatedAfterReleaseDatePTInValid_ShouldReturnTrue()
        {
            Account account = GetAccountWithPatientType2(new RegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate , new YesNoFlag(YesNoFlag.CODE_YES));
            var ruleUnderTest = new RightCareRightPlaceRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
     
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPostMSERegistration_CreatedDateAfterReleaseDate_ValidPT_AdmitDateBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccountWithPatientType3(new PostMSERegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateBeforeRCRPReleaseDate , new YesNoFlag(YesNoFlag.CODE_BLANK));
            var ruleUnderTest = new RightCareRightPlaceRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        #endregion

        #region Support Methods

        private static Account GetAccountWithPatientType3( Activity activity, DateTime accountCreatedDate, DateTime admitDate ,  YesNoFlag RCRP  )
        {
            Facility facility = new Facility(PersistentModel.NEW_OID,
                                         PersistentModel.NEW_VERSION,
                                         "DOCTORS HOSPITAL DALLAS",
                                         "DHF");
            facility["IsFacilityRCRPEnabled"] = true;
            Account account =  new Account
                       {
                Facility = facility,
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                AdmitDate =  admitDate,
                KindOfVisit = new VisitType(PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION, VisitType.EMERGENCY_PATIENT_DESC, VisitType.EMERGENCY_PATIENT),
                                          };
            account.RightCareRightPlace.RCRP = RCRP ;
            return account;

            
        }
         private static Account GetAccountWithPatientType2( Activity activity, DateTime accountCreatedDate, DateTime admitDate, YesNoFlag RCRP )
        {
            Account account =  new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                AdmitDate =  admitDate,
                KindOfVisit = new VisitType(PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT)
                                            };
             account.RightCareRightPlace.RCRP = RCRP;
             return account;
        }
        #endregion

        #region Constants

        private readonly DateTime TestDateAfterRCRPReleaseDate = DateTime.Parse( "01-05-2011" );
        private readonly DateTime TestDateBeforeRCRPReleaseDate = DateTime.Parse( "01-01-2009" );
        
        #endregion
    }
}
