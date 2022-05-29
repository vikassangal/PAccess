using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Persistence.AccountCopy;
using NUnit.Framework;
using PatientAccess.Domain.UCCRegistration;

namespace Tests.Integration.PatientAccess.Persistence.AccountCopy
{
    [TestFixture]
    [Category( "Fast" )]
    public class AccountCopyStrategyTests
    {
        #region Constants
        private const string ADDRESS1 = "335 Nicholson Dr.",
                             ADDRESS2 = "32321",
                             CITY = "Austin",
                             POSTALCODE = "60505";

        private const string CHIEF_COMPLAINT = "Soar Throat";
        private const string PEROT_EMPLOYER = "PerotSystems";
        private const string PEROT_ID = "001";
        private const string PEROT_EMP_ID = "111";
        private const string PEROT_OCCUPATION = "Accountant";
        private const string CISCO_EMPLOYER = "CiscoSystems";
        private const string CISCO_EMP_ID = "222";
        private const string CISCO_OCCUPATION = "HR";
        private const string CISCO_ID = "002";

        private const string PATIENTIPACODE = "HERTI",
                             PATIENTIPACLINICCODE = "01";
        private const string IPACODE = "02",
                             IPACLINICCODE = "44";
        private const string DHF_FACILITY_CODE = "DHF";

        private const int AGE_ENTITLEMENT           = 1,
                          DISABILITY_ENTITLEMENT    = 2,
                          ESRD_ENTITLEMENT          = 3;

        #endregion

        #region SetUp and TearDown AccountCopyStrategyTests
        [TestFixtureSetUp]
        public static void SetUpAccountCopyStrategyTests()
        {
            DHF_FACILITY = facilityBroker.FacilityWith(DHF_FACILITY_CODE);
        }

        [TestFixtureTearDown]
        public static void TearDownAccountCopyStrategyTests()
        {
        }
        #endregion

        #region Test Methods
        [Test]
        public void TestIPACopyForward_WhenOldAccountHasIPA()
        {
            var account1 = GetAccountWithIPA( null );
            account1.Activity = new RegistrationActivity();
            var account2 = copyStrategy.CopyAccount( account1 );

            Assert.AreEqual( account1.Patient.MedicalGroupIPA.Code, account2.MedicalGroupIPA.Code, "Medical Group IPA does not copy forward" );
        }

        [Test]
        public void TestIPACopyForward_WhenAccountCreatedOnActivatedPreRegAccount()
        {
            var account1 = GetAccountWithIPA( new ActivatePreRegistrationActivity() );
            var account2 = copyStrategy.CopyAccount( account1 );

            Assert.AreEqual( account1.Patient.MedicalGroupIPA.Code, account2.MedicalGroupIPA.Code, "Medical Group IPA does not copy forward" );
        }

        [Test]
        public void TestIPACopyForward_WhenAccountCreatedOnAPostMseAccount()
        {
            var account1 = GetAccountWithIPA( new PostMSERegistrationActivity() );
            var account2 = copyStrategy.CopyAccount( account1 );

            Assert.AreEqual( account1.Patient.MedicalGroupIPA.Code, account2.MedicalGroupIPA.Code, "Medical Group IPA does not copy forward" );
        }

        [Test]
        public void TestEditGeneralInformationUsing_IsNewBornFlagShouldBeFalse()
        {
            var oldAccount = new Account();
            oldAccount.Activity = new RegistrationActivity();
            var copyStrategy = new BinaryCloneAccountCopyStrategy();
            var newAccount = copyStrategy.CopyAccount(oldAccount);
            Assert.IsFalse(newAccount.IsNewBorn, "IsNewBorn flag should be false");
        }

        [Test]
        public void TestEditClinicalUsing_WhenOldAccountResearchStudiesCountIsNotZero_NewAccountResearchStudiesShouldBeZero()
        {
            Account oldAccount = new Account();

            ResearchStudy study1 = new ResearchStudy( "TEST01", "RESEARCH STUDY 1", "RESEARCH SPONSOR 1" );
            ResearchStudy study2 = new ResearchStudy( "TEST02", "RESEARCH STUDY 2", "RESEARCH SPONSOR 2" );
            ResearchStudy study3 = new ResearchStudy( "TEST03", "RESEARCH STUDY 3", "RESEARCH SPONSOR 3" );

            ConsentedResearchStudy research1 = new ConsentedResearchStudy( study1, YesNoFlag.Yes );
            ConsentedResearchStudy research2 = new ConsentedResearchStudy( study2, YesNoFlag.No );
            ConsentedResearchStudy research3 = new ConsentedResearchStudy( study3, YesNoFlag.Yes );

            oldAccount.AddConsentedResearchStudy( research1 );
            oldAccount.AddConsentedResearchStudy( research2 );
            oldAccount.AddConsentedResearchStudy( research3 );
            oldAccount.Activity = new RegistrationActivity();
            AccountCopyStrategy copyStrategy = new BinaryCloneAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount( oldAccount );

            Assert.IsNotNull( newAccount.ClinicalResearchStudies );
            Assert.IsTrue( newAccount.ClinicalResearchStudies.Count() == 0 );
        }

        [Test]
        public void TestMSPCopyForward_WhenOldMSPIsNull_ShouldCreateANewMSP()
        {
            GetOldAccountFor( new RegistrationActivity(), EmploymentStatus.EMPLOYED_FULL_TIME_CODE, AGE_ENTITLEMENT );
            OldAccount.MedicareSecondaryPayor = null;

            AccountCopyStrategy copyStrategy = new BinaryCloneAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount( OldAccount );

            Assert.IsNotNull( newAccount.MedicareSecondaryPayor );
            Assert.AreNotEqual( OldAccount.MedicareSecondaryPayor, newAccount.MedicareSecondaryPayor );
        }

        [Test]
        public void TestMSPCopyForward_ForAdmitNewBornActivity_ShouldNotCopyOverAnyMSPValues()
        {
            GetOldAccountFor( new AdmitNewbornActivity(), EmploymentStatus.EMPLOYED_FULL_TIME_CODE, AGE_ENTITLEMENT );

            AccountCopyStrategy copyStrategy = new BinaryCloneAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount( OldAccount );

            MedicareSecondaryPayor newMsp = newAccount.MedicareSecondaryPayor;

            Assert.IsNotNull( newMsp );
            Assert.IsNull( newMsp.MedicareEntitlement );
            AssertSpecialProgramForNonActivatePreRegActivity( newMsp.SpecialProgram, OldMsp.SpecialProgram, OldAccount.Activity );
            AssertLiabilityInsurerForNonActivatePreRegActivity( newMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
        }

        [Test]
        public void TestMSPCopyForward_ForPreAdmitNewBornActivity_ShouldNotCopyOverAnyMSPValues()
        {
            GetOldAccountFor( new PreAdmitNewbornActivity(), EmploymentStatus.EMPLOYED_FULL_TIME_CODE, AGE_ENTITLEMENT );

            AccountCopyStrategy copyStrategy = new BinaryCloneAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount( OldAccount );

            MedicareSecondaryPayor newMsp = newAccount.MedicareSecondaryPayor;

            Assert.IsNotNull( newMsp );
            Assert.IsNull( newMsp.MedicareEntitlement );
            AssertSpecialProgramForNonActivatePreRegActivity( newMsp.SpecialProgram, OldMsp.SpecialProgram, OldAccount.Activity );
            AssertLiabilityInsurerForNonActivatePreRegActivity( newMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
        }


        [Test]
        public void TestMSPCopyForward_ForPreMseFromReg_ShouldCopyNotOverAnyMSPValues()
        {
            GetOldAccountFor( new UCCPreMSERegistrationActivity(), EmploymentStatus.EMPLOYED_FULL_TIME_CODE, AGE_ENTITLEMENT );

            PreMseAccountCopyStrategy copyStrategy = new PreMseAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount( OldAccount );

            MedicareSecondaryPayor newMsp = newAccount.MedicareSecondaryPayor;

            Assert.IsNotNull( newMsp );
            Assert.IsNull( newMsp.MedicareEntitlement );
            AssertSpecialProgramForNonActivatePreRegActivity( newMsp.SpecialProgram, OldMsp.SpecialProgram, OldAccount.Activity );
            AssertLiabilityInsurerForNonActivatePreRegActivity( newMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
        }

        [Test]
        public void TestMSPCopyForward_ForPreMseFromPreMse_ShouldNotCopyOverAnyMSPValues()
        {
            GetOldAccountFor( new UCCPreMSERegistrationActivity(), EmploymentStatus.EMPLOYED_FULL_TIME_CODE, AGE_ENTITLEMENT );
            
            PreMseAccountCopyStrategy copyStrategy = new PreMseAccountCopyStrategy();
            Account preMseFromRegAccount = copyStrategy.CopyAccount( OldAccount );

            Account preMseFromPreMseAccount = copyStrategy.CopyAccount( preMseFromRegAccount );

            MedicareSecondaryPayor newMsp = preMseFromPreMseAccount.MedicareSecondaryPayor;

            Assert.IsNotNull( newMsp );
            Assert.IsNull( newMsp.MedicareEntitlement );
            AssertSpecialProgramForNonActivatePreRegActivity( newMsp.SpecialProgram, OldMsp.SpecialProgram, OldAccount.Activity );
            AssertLiabilityInsurerForNonActivatePreRegActivity( newMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
        }

        [Test]
        public void TestMSPCopyForward_PostMseForPreMseFromPreMse_ShouldNotCopyOverAnyMSPValues()
        {
            GetOldAccountFor( new PostMSERegistrationActivity(), EmploymentStatus.EMPLOYED_FULL_TIME_CODE, AGE_ENTITLEMENT );

            PreMseAccountCopyStrategy copyStrategy = new PreMseAccountCopyStrategy();
            Account preMseFromRegAccount = copyStrategy.CopyAccount( OldAccount );

            Account preMseFromPreMseAccount = copyStrategy.CopyAccount( preMseFromRegAccount );

            AccountCopyStrategy postMseCopyStrategy = new BinaryCloneAccountCopyStrategy();
            Account postMseForPreMseFromPreMseAccount = postMseCopyStrategy.CopyAccount( preMseFromPreMseAccount );

            MedicareSecondaryPayor newMsp = postMseForPreMseFromPreMseAccount.MedicareSecondaryPayor;

            Assert.IsNotNull( newMsp );
            Assert.IsNull( newMsp.MedicareEntitlement );
            AssertSpecialProgramForNonActivatePreRegActivity( newMsp.SpecialProgram, OldMsp.SpecialProgram, preMseFromPreMseAccount.Activity );
            AssertLiabilityInsurerForNonActivatePreRegActivity( newMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
        }

        [Test]
        public void TestMSPCopyForward_PostMseForPreMseFromReg_ShouldNotCopyOverMSPValues_ValuesWillBePulledOnlyThroughBroker()
        {
            GetOldAccountFor( new PostMSERegistrationActivity(), EmploymentStatus.EMPLOYED_FULL_TIME_CODE, AGE_ENTITLEMENT );

            PreMseAccountCopyStrategy copyStrategy = new PreMseAccountCopyStrategy();
            Account preMseFromRegAccount = copyStrategy.CopyAccount( OldAccount );

            AccountCopyStrategy postMseCopyStrategy = new BinaryCloneAccountCopyStrategy();
            Account postMseForPreMseFromRegAccount = postMseCopyStrategy.CopyAccount( preMseFromRegAccount );

            MedicareSecondaryPayor newMsp = postMseForPreMseFromRegAccount.MedicareSecondaryPayor;

            Assert.IsNotNull( newMsp );
            Assert.IsNull( newMsp.MedicareEntitlement );
            AssertSpecialProgramForNonActivatePreRegActivity( 
                newMsp.SpecialProgram, OldMsp.SpecialProgram, postMseForPreMseFromRegAccount.Activity );
            AssertLiabilityInsurerForNonActivatePreRegActivity( newMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
        }

        [Test]
        public void TestCPTCodes_ForRegisterNewBorn_ShouldNotCopyOverCPTCodes()
        {
            var mothersAccount = GetAccountWithCPTCodes(new RegistrationActivity());

            var newBornAccountCopyStrategy = new NewbornAccountCopyStrategy();
            var babyAccount = newBornAccountCopyStrategy.CopyAccount(mothersAccount);

            Assert.AreNotEqual(0, mothersAccount.CptCodes.Count, "mothers account should have more than zero cpt codes");
            Assert.AreEqual(0, babyAccount.CptCodes.Count, "CPT codes should not copy forward to new born account");
        }

        [Test]
        public void TestCPTCodes_ForPreRegistration_ShouldNotCopyOverCPTCodes()
        {
            var preRegAccount = GetAccountWithCPTCodes(new PreRegistrationActivity());

            var accountCopyStrategy = new  BinaryCloneAccountCopyStrategy();
            var newAccount = accountCopyStrategy.CopyAccount(preRegAccount);

            Assert.AreNotEqual(0, preRegAccount.CptCodes.Count, "pre reg account should have more than zero CPT codes");
            Assert.AreEqual(0, newAccount.CptCodes.Count, "CPT codes should not copy forward to new account");
        }

        [Test]
        public void TestMSPCopyForward_WhenOldAccountHasAgeEntitlementMSP()
        {
            foreach ( Activity activity in ActivityList )
            {
                TestMSPForAgeEntitlement( activity, EmploymentStatus.EMPLOYED_FULL_TIME_CODE );
                TestMSPForAgeEntitlement( activity, EmploymentStatus.RETIRED_CODE );
                TestMSPForAgeEntitlement( activity, EmploymentStatus.NOT_EMPLOYED_CODE );
            }
        }

        private static void TestMSPForAgeEntitlement( Activity activity, string employmentStatus )
        {
            GetOldAccountFor( activity, employmentStatus, AGE_ENTITLEMENT );

            AccountCopyStrategy copyStrategy = new BinaryCloneAccountCopyStrategy();
            NewAccount = copyStrategy.CopyAccount( OldAccount );

            Assert.IsNotNull( NewMsp );
            Assert.IsNotNull( NewMsp.MedicareEntitlement );

            if ( activity.GetType() == typeof( ActivatePreRegistrationActivity ) )
            {
                AssertGroupHealthPlanCoverageForAgeEntitlementAndActivatePreRegActivity( NewAgeEnt, OldAgeEnt );
                AssertSpecialProgramForActivatePreRegActivity( NewMsp.SpecialProgram, OldMsp.SpecialProgram );
                AssertLiabilityInsurerForActivatePreRegActivity( NewMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
                AssertPatientAndSpouseEmploymentForActivatePreRegActivity(
                    NewPatientEmployment, OldPatientEmployment, NewSpouseEmployment, OldSpouseEmployment );
            }
            else
            {
                AssertGroupHealthPlanCoverageForAgeEntitlementAndNonActivatePreRegActivity( NewAgeEnt, OldAgeEnt );
                AssertSpecialProgramForNonActivatePreRegActivity( NewMsp.SpecialProgram, OldMsp.SpecialProgram, activity );
                AssertLiabilityInsurerForNonActivatePreRegActivity( NewMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
                AssertPatientAndSpouseEmploymentForNonActivatePreRegActivity(
                    NewPatientEmployment, OldPatientEmployment, NewSpouseEmployment, OldSpouseEmployment );

                if( employmentStatus == EmploymentStatus.RETIRED_CODE )
                {
                    Assert.IsTrue( NewPatientEmployment.RetiredDate == OldPatientEmployment.RetiredDate );
                    Assert.IsTrue( NewSpouseEmployment.RetiredDate == OldSpouseEmployment.RetiredDate );
                }
                else
                {
                    Assert.IsTrue( NewPatientEmployment.RetiredDate == DateTime.MinValue );
                    Assert.IsTrue( NewSpouseEmployment.RetiredDate == DateTime.MinValue );
                }
            }
        }

        [Test]
        public void TestMSPCopyForward_WhenOldAccountHasDisabilityEntitlementMSP()
        {
            foreach ( Activity activity in ActivityList )
            {
                TestMSPForDisabilityEntitlement( activity, EmploymentStatus.EMPLOYED_FULL_TIME_CODE );
                TestMSPForDisabilityEntitlement( activity, EmploymentStatus.RETIRED_CODE );
                TestMSPForDisabilityEntitlement( activity, EmploymentStatus.NOT_EMPLOYED_CODE );
            }
        }

        private static void TestMSPForDisabilityEntitlement( Activity activity, string employmentStatus )
        {
            GetOldAccountFor( activity, employmentStatus, DISABILITY_ENTITLEMENT );

            AccountCopyStrategy copyStrategy = new BinaryCloneAccountCopyStrategy();
            NewAccount = copyStrategy.CopyAccount( OldAccount );

            Assert.IsNotNull( NewMsp );
            Assert.IsNotNull( NewMsp.MedicareEntitlement );

            if ( activity.GetType() == typeof( ActivatePreRegistrationActivity ) )
            {
                AssertDisabilityEntitlementValuesForActivatePreRegActivity( NewDisabilityEnt, OldDisabilityEnt );
                AssertSpecialProgramForActivatePreRegActivity( NewMsp.SpecialProgram, OldMsp.SpecialProgram );
                AssertLiabilityInsurerForActivatePreRegActivity( NewMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
                AssertPatientAndSpouseEmploymentForActivatePreRegActivity(
                    NewPatientEmployment, OldPatientEmployment, NewSpouseEmployment, OldSpouseEmployment );
            }
            else
            {
                AssertDisabilityEntitlementValuesForNonActivatePreRegActivity( NewDisabilityEnt, OldDisabilityEnt );
                AssertSpecialProgramForNonActivatePreRegActivity( NewMsp.SpecialProgram, OldMsp.SpecialProgram, activity );
                AssertLiabilityInsurerForNonActivatePreRegActivity( NewMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
                AssertPatientAndSpouseEmploymentForNonActivatePreRegActivity(
                    NewPatientEmployment, OldPatientEmployment, NewSpouseEmployment, OldSpouseEmployment );

                if ( employmentStatus == EmploymentStatus.RETIRED_CODE )
                {
                    Assert.IsTrue( NewPatientEmployment.RetiredDate == OldPatientEmployment.RetiredDate );
                    Assert.IsTrue( NewSpouseEmployment.RetiredDate == OldSpouseEmployment.RetiredDate );
                }
                else
                {
                    Assert.IsTrue( NewPatientEmployment.RetiredDate == DateTime.MinValue );
                    Assert.IsTrue( NewSpouseEmployment.RetiredDate == DateTime.MinValue );
                }
            }
        }

        [Test]
        public void TestMSPCopyForward_WhenOldAccountHasESRDEntitlementMSP()
        {
            foreach ( Activity activity in ActivityList )
            {
                TestMSPForESRDEntitlement( activity, EmploymentStatus.EMPLOYED_FULL_TIME_CODE );
                TestMSPForESRDEntitlement( activity, EmploymentStatus.RETIRED_CODE );
                TestMSPForESRDEntitlement( activity, EmploymentStatus.NOT_EMPLOYED_CODE );
            }
        }

        private static void TestMSPForESRDEntitlement( Activity activity, string employmentStatus )
        {
            GetOldAccountFor( activity, employmentStatus, ESRD_ENTITLEMENT );

            AccountCopyStrategy copyStrategy = new BinaryCloneAccountCopyStrategy();
            NewAccount = copyStrategy.CopyAccount( OldAccount );

            Assert.IsNotNull( NewMsp );
            Assert.IsNotNull( NewMsp.MedicareEntitlement );

            if ( activity.GetType() == typeof( ActivatePreRegistrationActivity ) )
            {
                AssertESRDValuesForActivatePreRegActivity( NewESRDEnt, OldESRDEnt );
                AssertSpecialProgramForActivatePreRegActivity( NewMsp.SpecialProgram, OldMsp.SpecialProgram );
                AssertLiabilityInsurerForActivatePreRegActivity( NewMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
            }
            else
            {
                AssertESRDValuesForNonActivatePreRegActivity( NewESRDEnt, OldESRDEnt, activity );
                AssertSpecialProgramForNonActivatePreRegActivity( NewMsp.SpecialProgram, OldMsp.SpecialProgram, activity );
                AssertLiabilityInsurerForNonActivatePreRegActivity( NewMsp.LiabilityInsurer, OldMsp.LiabilityInsurer );
            }
        }

        [Test]
        public void TestPCPCopyForward_WhenMostRecentAccCreationDateIsGreaterThan60DaysInThePast_ThenPCPShouldNotCopyForward()
        {
            var aDateMoreThan60DaysInThePast = GetAPastDateLessThan(61);
            var anAccount = GetAnAccountWithPhysicianRelationshipAdded( aDateMoreThan60DaysInThePast );

            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);

            Assert.IsNull( copiedtoAccount.PrimaryCarePhysician, "Primary Care Physician should not be copied forward" );
        }

        [Test]
        public void TestPCPCopyForward_WhenMostRecentAccCreationDateIsEqualTo60DaysInThePast_ThenPCPShouldNotCopyForward()
        {
            var aDate60DaysInThePast = GetAPastDateLessThan(60);
            var anAccount = GetAnAccountWithPhysicianRelationshipAdded( aDate60DaysInThePast );

            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);

            Assert.IsNull( copiedtoAccount.PrimaryCarePhysician, "Primary Care Physician should not be copied forward" );
        }

        [Test]
        public void TestPCPCopyForward_WhenMostRecentAccCreationDateIsLessThan60DaysInThePast_ThenPCPShouldCopyForward()
        {
            var aDateLessThan60DaysInThePast = GetAPastDateLessThan(59);
            var anAccount = GetAnAccountWithPhysicianRelationshipAdded( aDateLessThan60DaysInThePast );

            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);

            Assert.AreEqual(copiedtoAccount.PrimaryCarePhysician, anAccount.PrimaryCarePhysician, "Primary Care Physician should be copied forward");
        }

        [Test]
        public void TestPCPCopyForward_WhenMostRecentAccWithNOPCP_ThenPCPShouldNotCopyForward()
        {
            var anAccount = new Account
            {
                Patient = { MostRecentAccountCreationDate = GetAPastDateLessThan(59) }
            };
            
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            anAccount.Facility = facilityBroker.FacilityWith("DEL");

            anAccount.Patient.MedicalRecordNumber = 24004;
            anAccount.Patient.MostRecentAccountNumber = 4477677;
            anAccount.Activity = new RegistrationActivity();

            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);

            Assert.IsNull(copiedtoAccount.PrimaryCarePhysician, "Primary Care Physician should be null");
        }

        [Test]
        public void TestPCPCopyForward_WhenMostRecentAccWithUNKNOWNPCP_ThenPCPShouldNotCopyForward()
        {
            var anAccount = new Account
            {
                Patient = { MostRecentAccountCreationDate = GetAPastDateLessThan(59) }
            };

            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            anAccount.Facility = facilityBroker.FacilityWith("DEL");

            anAccount.Patient.MedicalRecordNumber = 219005;
            anAccount.Patient.MostRecentAccountNumber = 20020921;
            anAccount.Activity = new RegistrationActivity();

            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);

            Assert.IsNull(copiedtoAccount.PrimaryCarePhysician, "Primary Care Physician should be null");
        }

        [Test]
        public void TestHIEConsentFlag_IsNo_WhenActivityIsRegistration_ShouldNotCopyForward_ShouldDefaultToYes()
        {
            var flag = new YesNoFlag();
            flag.SetNo();
            var anAccount = new Account
                {
                    Patient = new Patient(),
                    Activity = new RegistrationActivity(),
                    ShareDataWithPublicHieFlag = flag
                };

            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);
            Assert.IsFalse(copiedtoAccount.ShareDataWithPublicHieFlag.Code.Equals(flag.Code), "HIE consent is copied forward");
        }
        [Test]
        public void TestHIEPhysicianFlag_IsNo_WhenActivityIsRegistration_ShouldNotCopyForward_ShouldDefaultToYes()
        {
            var flag = new YesNoFlag();
            flag.SetNo();
            var anAccount = new Account
                {
                    Patient = new Patient(),
                    Activity = new RegistrationActivity(),
                    ShareDataWithPCPFlag = flag
                };
            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);
            Assert.IsFalse(copiedtoAccount.ShareDataWithPublicHieFlag.Code.Equals(flag.Code), "HIE Physician consent is copied forward");
        }
        [Test]
        public void TestHIEConsentFlag_IsNo_WhenActivityIsPOSTMSE_ShouldNotCopyForward_ShouldDefaultToYes()
        {
            var flag = new YesNoFlag();
            flag.SetNo();
            var anAccount = new Account
            {
                Patient = new Patient(),
                Activity = new PostMSERegistrationActivity(),
                ShareDataWithPublicHieFlag = flag
            };

            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);
            Assert.IsFalse(copiedtoAccount.ShareDataWithPublicHieFlag.Code.Equals(flag.Code), "HIE consent is copied forward");
        }
        [Test]
        public void TestHIEPhysicianFlag_IsNo_WhenActivityIsPOSTMSE_ShouldNotCopyForward_ShouldDefaultToYes()
        {
            var flag = new YesNoFlag();
            flag.SetNo();
            var anAccount = new Account
            {
                Patient = new Patient(),
                Activity = new PostMSERegistrationActivity(),
                ShareDataWithPCPFlag = flag
            };
            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);
            Assert.IsFalse(copiedtoAccount.ShareDataWithPublicHieFlag.Code.Equals(flag.Code), "HIE Physician consent is copied forward");
        }
        [Test]
        public void TestHIEConsentFlag_IsNo_WhenActivityIsShortRegistration_ShouldNotCopyForward_ShouldDefaultToYes()
        {
            var flag = new YesNoFlag();
            flag.SetNo();
            var anAccount = new Account
            {
                Patient = new Patient(),
                Activity = new ShortRegistrationActivity(),
                ShareDataWithPublicHieFlag = flag
            };

            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);
            Assert.IsFalse(copiedtoAccount.ShareDataWithPublicHieFlag.Code.Equals(flag.Code), "HIE consent is copied forward");
        }
        [Test]
        public void TestHIEPhysicianFlag_IsNo_WhenActivityIsShortRegistration_ShouldNotCopyForward_ShouldDefaultToYes()
        {
            var flag = new YesNoFlag();
            flag.SetNo();
            var anAccount = new Account
            {
                Patient = new Patient(),
                Activity = new ShortRegistrationActivity(),
                ShareDataWithPCPFlag = flag
            };
            var copiedtoAccount = accountCopyBroker.CreateAccountCopyFor(anAccount);
            Assert.IsFalse(copiedtoAccount.ShareDataWithPublicHieFlag.Code.Equals(flag.Code), "HIE Physician consent is copied forward");
        }
        #endregion

        #region Support Methods

        private Account GetAccountWithIPA( Activity activity )
        {
            var account = new Account { Activity = activity };

            var medicalGroupIpa = insuranceBroker.IPAWith( DHF_FACILITY.Oid, IPACODE, IPACLINICCODE );
            var patientMedicalGroupIpa = insuranceBroker.IPAWith( DHF_FACILITY.Oid, PATIENTIPACODE, PATIENTIPACLINICCODE );
            account.MedicalGroupIPA = medicalGroupIpa;
            account.Patient.MedicalGroupIPA = patientMedicalGroupIpa;
            
            return account;
        }

        private static void GetOldAccountFor(Activity activity, string employmentStatus, int entitlementType)
        {
            Employment patientEmployment;
            Employment spouseEmployment;
            OldAccount = new Account
            {
                AccountNumber = 111,
                Diagnosis = GetDiagnosis(),
                Activity = activity,
                MedicareSecondaryPayor = new MedicareSecondaryPayor 
                                            { MSPVersion = 2,
                                              SpecialProgram = GetSpecialProgram(),
                                              LiabilityInsurer = GetLiabilityInsurer() }
            };

            switch ( entitlementType )
            {
                case AGE_ENTITLEMENT:
                    OldAccount.MedicareSecondaryPayor.MedicareEntitlement = GetAgeEntitlement();
                    break;
                case DISABILITY_ENTITLEMENT:
                    OldAccount.MedicareSecondaryPayor.MedicareEntitlement = GetDisabilityEntitlement();
                    break;
                case ESRD_ENTITLEMENT:
                    OldAccount.MedicareSecondaryPayor.MedicareEntitlement = GetESRDEntitlement();
                    break;
            }

            switch ( employmentStatus )
            {
                case EmploymentStatus.RETIRED_CODE:
                    patientEmployment = GetRetiredEmployment();
                    spouseEmployment = GetRetiredEmployment();
                    break;
                case EmploymentStatus.NOT_EMPLOYED_CODE:
                    patientEmployment = new Employment { Status = EmploymentStatus.NewNotEmployed() };
                    spouseEmployment = new Employment { Status = EmploymentStatus.NewNotEmployed() };
                    break;
                default:
                    patientEmployment = GetPatientFullTimeEmployment();
                    spouseEmployment = GetSpouseFullTimeEmployment();
                    break;
            }

            OldAccount.MedicareSecondaryPayor.MedicareEntitlement.PatientEmployment = patientEmployment;
            OldAccount.MedicareSecondaryPayor.MedicareEntitlement.SpouseEmployment = spouseEmployment;
        }

        private Account GetAccountWithCPTCodes(Activity activity)
        {
            var cptCodes = new Dictionary<int, string> {{1, "f1111"}, {5, "a5555"}};
            var accountWithCPTCodes = new Account
            {
                AccountNumber = 111,
                CptCodes = cptCodes,
                Activity = activity,
                Facility = DHF_FACILITY
            };
            return accountWithCPTCodes;
        }

        private static Employment GetPatientFullTimeEmployment()
        {
            Address address = new Address( ADDRESS1,
                                          ADDRESS2,
                                          CITY,
                                          ZipCode,
                                          State,
                                          Country,
                                          County
                );

            Employment employment = new Employment
            {
                Employer = new Employer( 1L, DateTime.Now, PEROT_EMPLOYER, PEROT_ID, 100 ),
                EmployeeID = PEROT_EMP_ID,
                PhoneNumber = PerotPhone,
                Occupation = PEROT_OCCUPATION,
                Status = EmploymentStatus.NewFullTimeEmployed()
            };

            employment.Employer.PartyContactPoint =
                new ContactPoint( TypeOfContactPoint.NewBusinessContactPointType() ) { Address = address };

            return employment;
        }

        private static Employment GetRetiredEmployment()
        {
            Address address = new Address( ADDRESS1, ADDRESS2, CITY, ZipCode, State, Country, County );

            Employment employment = new Employment
            {
                Employer = new Employer( 1L, DateTime.Now, PEROT_EMPLOYER, PEROT_ID, 100 ),
                EmployeeID = PEROT_EMP_ID,
                PhoneNumber = PerotPhone,
                Occupation = PEROT_OCCUPATION,
                Status = EmploymentStatus.NewRetired(),
                RetiredDate = PerotRetiredDate
            };
            employment.Employer.PartyContactPoint =
                new ContactPoint( TypeOfContactPoint.NewBusinessContactPointType() ) { Address = address };

            return employment;
        }

        private static Employment GetSpouseFullTimeEmployment()
        {
            Address address = new Address( ADDRESS1, ADDRESS2, CITY, ZipCode, State, Country, County );

            Employment employment = new Employment
            {
                Employer = new Employer( 1L, DateTime.Now, CISCO_EMPLOYER, CISCO_ID, 100 ),
                EmployeeID = CISCO_EMP_ID,
                PhoneNumber = CiscoPhone,
                Occupation = CISCO_OCCUPATION,
                Status = EmploymentStatus.NewFullTimeEmployed()
            };
            employment.Employer.PartyContactPoint =
                new ContactPoint( TypeOfContactPoint.NewBusinessContactPointType() ) { Address = address };

            return employment;
        }

        private static SpecialProgram GetSpecialProgram()
        {
            SpecialProgram program = new SpecialProgram
             {
                 BlackLungBenefits = YesFlag,
                 BLBenefitsStartDate = BLBenefitsStartDate,
                 VisitForBlackLung = YesFlag,
                 GovernmentProgram = YesFlag,
                 DVAAuthorized = YesFlag,
                 WorkRelated = NoFlag
             };

            return program;
        }

        private static LiabilityInsurer GetLiabilityInsurer()
        {
            LiabilityInsurer liabilityInsurer = new LiabilityInsurer
            {
                NonWorkRelated = YesFlag,
                NoFaultInsuranceAvailable = YesFlag,
                LiabilityInsuranceAvailable = YesFlag
            };

            return liabilityInsurer;
        }

        private static Diagnosis GetDiagnosis()
        {
            Diagnosis diagnosis = new Diagnosis();
            Illness illness = new Illness { Onset = OnsetDate };
            diagnosis.ChiefComplaint = CHIEF_COMPLAINT;
            diagnosis.Condition = illness;

            return diagnosis;
        }

        private static AgeEntitlement GetAgeEntitlement()
        {
            AgeEntitlement ageEntitlement = new AgeEntitlement
            {
                GroupHealthPlanCoverage = YesFlag,
                GroupHealthPlanType = GroupHealthPlanType.NewBothType(),
                GHPEmploysX = YesFlag,
                GHPSpouseEmploysX = NoFlag
            };

            return ageEntitlement;
        }

        private static DisabilityEntitlement GetDisabilityEntitlement()
        {
            DisabilityEntitlement disabilityEntitlement = new DisabilityEntitlement
              {
                  FamilyMemberGHPFlag = YesFlag,
                  FamilyMemberGHPEmploysMoreThanXFlag = NoFlag,
                  GroupHealthPlanCoverage = YesFlag,
                  GHPEmploysMoreThanXFlag = YesFlag,
                  GHPCoverageOtherThanSpouse = YesFlag,
                  SpouseGHPEmploysMoreThanXFlag = NoFlag,
                  GroupHealthPlanType = GroupHealthPlanType.NewSpouseType()
              };

            return disabilityEntitlement;
        }

        private static ESRDEntitlement GetESRDEntitlement()
        {
            ESRDEntitlement esrdEntitlement = new ESRDEntitlement
              {
                  GroupHealthPlanCoverage = YesFlag,
                  DialysisTreatment = YesFlag,
                  DialysisTrainingStartDate = DialysisTrainingStartDate,
                  DialysisDate = DialysisDate,
                  KidneyTransplant = YesFlag,
                  TransplantDate = TransplantDate,
                  BasedOnESRD = YesFlag,

                  WithinCoordinationPeriod = YesFlag,
                  ESRDandAgeOrDisability = YesFlag,
                  BasedOnAgeOrDisability = NoFlag,
                  ProvisionAppliesFlag = NoFlag
              };

            return esrdEntitlement;
        }

        private Account GetAnAccountWithPhysicianRelationshipAdded(DateTime mostRecentAccCreationDate)
        {
            var anAccount = new Account
                {
                    Patient = {MostRecentAccountCreationDate = mostRecentAccCreationDate}
                };

            var primarCarePhysician = new Physician
                {
                    FirstName = "PrimaryCare", 
                    LastName = "Physician", 
                    PhysicianNumber = 1L
                };

            var primaryCareRelationship = new PhysicianRelationship( PhysicianRole.PrimaryCare().Role(), primarCarePhysician);

            anAccount.AddPhysicianRelationship(primaryCareRelationship);
            anAccount.Activity = new RegistrationActivity();

            return anAccount;
        }

        private DateTime GetAPastDateLessThan(int numberOfDays)
        {
            return facility.GetCurrentDateTime().Subtract(TimeSpan.FromDays(numberOfDays));
        }
        #endregion

        #region MSP Assertions

        private static void AssertPatientAndSpouseEmploymentForNonActivatePreRegActivity( Employment newPatientEmployment, Employment oldPatientEmployment, Employment newSpouseEmployment, Employment oldSpouseEmployment )
        {
            // Patient Employment 
            Assert.IsNotNull( newPatientEmployment );
            Assert.IsTrue( newPatientEmployment.PhoneNumber.ToString() == String.Empty );
            Assert.IsTrue( newPatientEmployment.Occupation == String.Empty );
            Assert.AreNotEqual( oldPatientEmployment.Employer, newPatientEmployment.Employer );
            if( oldPatientEmployment.Status.Code == EmploymentStatus.RETIRED_CODE ||
                oldPatientEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE )
            {
                Assert.AreEqual( oldPatientEmployment.Status.Code, newPatientEmployment.Status.Code );
            }
            else
            {
                Assert.AreNotEqual( oldPatientEmployment.Status.Code, newPatientEmployment.Status.Code );
            }

            // Spouse Employment 
            Assert.IsNotNull( newSpouseEmployment );
            Assert.IsTrue( newSpouseEmployment.PhoneNumber.ToString() == String.Empty );
            Assert.IsTrue( newSpouseEmployment.Occupation == String.Empty );
            Assert.AreNotEqual( oldSpouseEmployment.Employer, newSpouseEmployment.Employer );
            if ( oldSpouseEmployment.Status.Code == EmploymentStatus.RETIRED_CODE ||
                 oldSpouseEmployment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE )
            {
                Assert.AreEqual( oldSpouseEmployment.Status.Code, newSpouseEmployment.Status.Code );
            }
            else
            {
                Assert.AreNotEqual( oldSpouseEmployment.Status.Code, newSpouseEmployment.Status.Code );
            }
        }

        private static void AssertPatientAndSpouseEmploymentForActivatePreRegActivity( Employment newPatientEmployment, Employment oldPatientEmployment, Employment newSpouseEmployment, Employment oldSpouseEmployment )
        {
            // Patient Employment 
            Assert.IsNotNull( newPatientEmployment );
            Assert.AreEqual( oldPatientEmployment.PhoneNumber, newPatientEmployment.PhoneNumber );
            Assert.AreEqual( oldPatientEmployment.Occupation, newPatientEmployment.Occupation );
            Assert.AreEqual( oldPatientEmployment.RetiredDate, newPatientEmployment.RetiredDate );
            Assert.AreEqual( oldPatientEmployment.Employer, newPatientEmployment.Employer );
            Assert.AreEqual( oldPatientEmployment.Status, newPatientEmployment.Status );

            // Spouse Employment 
            Assert.IsNotNull( newSpouseEmployment );
            Assert.AreEqual( oldSpouseEmployment.PhoneNumber, newSpouseEmployment.PhoneNumber );
            Assert.AreEqual( oldSpouseEmployment.Occupation, newSpouseEmployment.Occupation );
            Assert.AreEqual( oldSpouseEmployment.RetiredDate, newSpouseEmployment.RetiredDate );
            Assert.AreEqual( oldSpouseEmployment.Employer, newSpouseEmployment.Employer );
            Assert.AreEqual( oldSpouseEmployment.Status, newSpouseEmployment.Status );
        }

        private static void AssertSpecialProgramForNonActivatePreRegActivity( SpecialProgram newProgram, SpecialProgram oldProgram, Activity activity )
        {
            Assert.AreNotEqual( oldProgram.VisitForBlackLung, newProgram.VisitForBlackLung );
            if ( activity.GetType() == typeof( AdmitNewbornActivity ) ||
                activity.GetType() == typeof( PreAdmitNewbornActivity ) ||
                 activity.GetType() == typeof( UCCPreMSERegistrationActivity ) ||
                 activity.GetType() == typeof( PostMSERegistrationActivity ) )
            {
                Assert.AreNotEqual( oldProgram.BlackLungBenefits, newProgram.BlackLungBenefits );
                Assert.AreNotEqual( oldProgram.BLBenefitsStartDate, newProgram.BLBenefitsStartDate );
            }
            else
            {
                Assert.AreEqual( oldProgram.BlackLungBenefits, newProgram.BlackLungBenefits );
                Assert.AreEqual( oldProgram.BLBenefitsStartDate, newProgram.BLBenefitsStartDate );
            }
            Assert.AreNotEqual( oldProgram.GovernmentProgram, newProgram.GovernmentProgram );
            Assert.AreNotEqual( oldProgram.DVAAuthorized, newProgram.DVAAuthorized );
            Assert.AreNotEqual( oldProgram.WorkRelated, newProgram.WorkRelated );
        }

        private static void AssertSpecialProgramForActivatePreRegActivity( SpecialProgram newProgram, SpecialProgram oldProgram )
        {
            Assert.AreEqual( oldProgram.VisitForBlackLung, newProgram.VisitForBlackLung );
            Assert.AreEqual( oldProgram.BlackLungBenefits, newProgram.BlackLungBenefits );
            Assert.AreEqual( oldProgram.BLBenefitsStartDate, newProgram.BLBenefitsStartDate );
            Assert.AreEqual( oldProgram.GovernmentProgram, newProgram.GovernmentProgram );
            Assert.AreEqual( oldProgram.DVAAuthorized, newProgram.DVAAuthorized );
            Assert.AreEqual( oldProgram.WorkRelated, newProgram.WorkRelated );
        }

        private static void AssertLiabilityInsurerForNonActivatePreRegActivity( LiabilityInsurer newLiability, LiabilityInsurer oldLiability )
        {
            Assert.AreNotEqual( oldLiability.NonWorkRelated, newLiability.NonWorkRelated );
            Assert.AreNotEqual( oldLiability.NoFaultInsuranceAvailable, newLiability.NoFaultInsuranceAvailable );
            Assert.AreNotEqual( oldLiability.LiabilityInsuranceAvailable, newLiability.LiabilityInsuranceAvailable );
        }

        private static void AssertLiabilityInsurerForActivatePreRegActivity( LiabilityInsurer newLiability, LiabilityInsurer oldLiability )
        {
            Assert.AreEqual( oldLiability.NonWorkRelated, newLiability.NonWorkRelated );
            Assert.AreEqual( oldLiability.NoFaultInsuranceAvailable, newLiability.NoFaultInsuranceAvailable );
            Assert.AreEqual( oldLiability.LiabilityInsuranceAvailable, newLiability.LiabilityInsuranceAvailable );
        }

        #endregion

        #region Age Entitlement Specific Assertions

        private static void AssertGroupHealthPlanCoverageForAgeEntitlementAndNonActivatePreRegActivity( 
            AgeEntitlement newAgeEnt, AgeEntitlement oldAgeEnt )
        {
            Assert.AreNotEqual( oldAgeEnt.GroupHealthPlanCoverage, newAgeEnt.GroupHealthPlanCoverage );
            Assert.AreNotEqual( oldAgeEnt.GroupHealthPlanType, newAgeEnt.GroupHealthPlanType );
            Assert.AreNotEqual( oldAgeEnt.GHPEmploysX, newAgeEnt.GHPEmploysX );
            Assert.AreNotEqual( oldAgeEnt.GHPSpouseEmploysX, newAgeEnt.GHPSpouseEmploysX );
        }

        private static void AssertGroupHealthPlanCoverageForAgeEntitlementAndActivatePreRegActivity( 
            AgeEntitlement newAgeEnt, AgeEntitlement oldAgeEnt )
        {
            Assert.AreEqual( oldAgeEnt.GroupHealthPlanCoverage, newAgeEnt.GroupHealthPlanCoverage );
            Assert.AreEqual( oldAgeEnt.GroupHealthPlanType, newAgeEnt.GroupHealthPlanType );
            Assert.AreEqual( oldAgeEnt.GHPEmploysX, newAgeEnt.GHPEmploysX );
            Assert.AreEqual( oldAgeEnt.GHPSpouseEmploysX, newAgeEnt.GHPSpouseEmploysX );
        }

        #endregion

        #region Disability Entitlement Specific Assertions

        private static void AssertDisabilityEntitlementValuesForNonActivatePreRegActivity(
            DisabilityEntitlement newDisabilityEnt, DisabilityEntitlement oldDisabilityEnt )
        {
            Assert.AreNotEqual( oldDisabilityEnt.GroupHealthPlanCoverage, newDisabilityEnt.GroupHealthPlanCoverage );
            Assert.AreNotEqual( oldDisabilityEnt.GroupHealthPlanType, newDisabilityEnt.GroupHealthPlanType );
            Assert.AreNotEqual( oldDisabilityEnt.GHPEmploysMoreThanXFlag, newDisabilityEnt.GHPEmploysMoreThanXFlag );
            Assert.AreNotEqual( oldDisabilityEnt.GHPCoverageOtherThanSpouse, newDisabilityEnt.GHPCoverageOtherThanSpouse );
            Assert.AreNotEqual( oldDisabilityEnt.SpouseGHPEmploysMoreThanXFlag, newDisabilityEnt.SpouseGHPEmploysMoreThanXFlag );
            Assert.AreNotEqual( oldDisabilityEnt.FamilyMemberGHPFlag, newDisabilityEnt.FamilyMemberGHPFlag );
            Assert.AreNotEqual( oldDisabilityEnt.FamilyMemberGHPEmploysMoreThanXFlag, newDisabilityEnt.FamilyMemberGHPEmploysMoreThanXFlag );
        }

        private static void AssertDisabilityEntitlementValuesForActivatePreRegActivity(
            DisabilityEntitlement newDisabilityEnt, DisabilityEntitlement oldDisabilityEnt )
        {
            Assert.AreEqual( oldDisabilityEnt.GroupHealthPlanCoverage, newDisabilityEnt.GroupHealthPlanCoverage );
            Assert.AreEqual( oldDisabilityEnt.GroupHealthPlanType, newDisabilityEnt.GroupHealthPlanType );
            Assert.AreEqual( oldDisabilityEnt.GHPEmploysMoreThanXFlag, newDisabilityEnt.GHPEmploysMoreThanXFlag );
            Assert.AreEqual( oldDisabilityEnt.GHPCoverageOtherThanSpouse, newDisabilityEnt.GHPCoverageOtherThanSpouse );
            Assert.AreEqual( oldDisabilityEnt.SpouseGHPEmploysMoreThanXFlag, newDisabilityEnt.SpouseGHPEmploysMoreThanXFlag );
            Assert.AreEqual( oldDisabilityEnt.FamilyMemberGHPFlag, newDisabilityEnt.FamilyMemberGHPFlag );
            Assert.AreEqual( oldDisabilityEnt.FamilyMemberGHPEmploysMoreThanXFlag, newDisabilityEnt.FamilyMemberGHPEmploysMoreThanXFlag );
        }

        #endregion

        #region ESRD Entitlement Specific Assertions

        private static void AssertESRDValuesForNonActivatePreRegActivity(
            ESRDEntitlement newESRDEnt, ESRDEntitlement oldESRDEnt, Activity activity )
        {
            Assert.AreNotEqual( oldESRDEnt.GroupHealthPlanCoverage, newESRDEnt.GroupHealthPlanCoverage );
            if( oldESRDEnt.KidneyTransplant.Code == YesNoFlag.CODE_YES )
            {
                Assert.AreEqual( oldESRDEnt.KidneyTransplant, newESRDEnt.KidneyTransplant );
            }
            else
            {
                Assert.AreNotEqual( oldESRDEnt.KidneyTransplant, newESRDEnt.KidneyTransplant );
            }

            if ( oldESRDEnt.DialysisTreatment.Code == YesNoFlag.CODE_YES )
            {
                Assert.AreEqual( oldESRDEnt.DialysisTreatment, newESRDEnt.DialysisTreatment );
            }
            else
            {
                Assert.AreNotEqual( oldESRDEnt.DialysisTreatment, newESRDEnt.DialysisTreatment );
            }
            Assert.AreNotEqual( oldESRDEnt.DialysisTrainingStartDate, newESRDEnt.DialysisTrainingStartDate );
            Assert.AreNotEqual( oldESRDEnt.BasedOnAgeOrDisability, newESRDEnt.BasedOnAgeOrDisability );

            if ( oldESRDEnt.WithinCoordinationPeriod.Code == YesNoFlag.CODE_YES )
            {
                Assert.AreEqual( oldESRDEnt.WithinCoordinationPeriod, newESRDEnt.WithinCoordinationPeriod );
                if ( oldESRDEnt.BasedOnAgeOrDisability.Code == YesNoFlag.CODE_YES )
                {
                    Assert.AreEqual( oldESRDEnt.BasedOnAgeOrDisability, newESRDEnt.BasedOnAgeOrDisability );
                    Assert.AreEqual( oldESRDEnt.BasedOnESRD, newESRDEnt.BasedOnESRD );
                }
                else
                {
                    Assert.AreNotEqual( oldESRDEnt.BasedOnAgeOrDisability, newESRDEnt.BasedOnAgeOrDisability );
                }
            }
            else
            {
                Assert.AreNotEqual( oldESRDEnt.WithinCoordinationPeriod, newESRDEnt.WithinCoordinationPeriod );
            }
            
            Assert.AreNotEqual( oldESRDEnt.ProvisionAppliesFlag, newESRDEnt.ProvisionAppliesFlag );

            if ( activity.GetType() == typeof( AdmitNewbornActivity ) )
            {
                Assert.AreNotEqual( oldESRDEnt.TransplantDate, oldESRDEnt.TransplantDate );
                Assert.AreNotEqual( oldESRDEnt.DialysisDate, oldESRDEnt.DialysisDate );
                Assert.AreNotEqual( oldESRDEnt.BasedOnESRD, newESRDEnt.BasedOnESRD );
            }
            else
            {
                Assert.AreEqual( oldESRDEnt.TransplantDate, oldESRDEnt.TransplantDate );
                Assert.AreEqual( oldESRDEnt.DialysisDate, oldESRDEnt.DialysisDate );
            }
        }

        private static void AssertESRDValuesForActivatePreRegActivity( ESRDEntitlement newESRDEnt, ESRDEntitlement oldESRDEnt )
        {
            Assert.AreEqual( oldESRDEnt.GroupHealthPlanCoverage, newESRDEnt.GroupHealthPlanCoverage );
            Assert.AreEqual( oldESRDEnt.GroupHealthPlanType, newESRDEnt.GroupHealthPlanType );
            Assert.AreEqual( oldESRDEnt.KidneyTransplant, newESRDEnt.KidneyTransplant );
            Assert.AreEqual( oldESRDEnt.DialysisTreatment, newESRDEnt.DialysisTreatment );
            Assert.AreEqual( oldESRDEnt.DialysisTrainingStartDate, newESRDEnt.DialysisTrainingStartDate );
            Assert.AreEqual( oldESRDEnt.BasedOnAgeOrDisability, newESRDEnt.BasedOnAgeOrDisability );
            Assert.AreEqual( oldESRDEnt.WithinCoordinationPeriod, newESRDEnt.WithinCoordinationPeriod );
            Assert.AreEqual( oldESRDEnt.BasedOnESRD, newESRDEnt.BasedOnESRD );
            Assert.AreEqual( oldESRDEnt.ProvisionAppliesFlag, newESRDEnt.ProvisionAppliesFlag );
        }

        #endregion

        #region Properties

        private static AgeEntitlement NewAgeEnt
        {
            get { return ( AgeEntitlement )NewMsp.MedicareEntitlement; }
        }

        private static AgeEntitlement OldAgeEnt
        {
            get { return ( AgeEntitlement )OldMsp.MedicareEntitlement; }
        }

        private static DisabilityEntitlement NewDisabilityEnt
        {
            get { return ( DisabilityEntitlement )NewMsp.MedicareEntitlement; }
        }

        private static DisabilityEntitlement OldDisabilityEnt
        {
            get { return ( DisabilityEntitlement )OldMsp.MedicareEntitlement; }
        }

        private static ESRDEntitlement NewESRDEnt
        {
            get { return ( ESRDEntitlement )NewMsp.MedicareEntitlement; }
        }

        private static ESRDEntitlement OldESRDEnt
        {
            get { return ( ESRDEntitlement )OldMsp.MedicareEntitlement; }
        }

        private static Employment NewPatientEmployment
        {
            get { return NewMsp.MedicareEntitlement.PatientEmployment; }
        }

        private static Employment NewSpouseEmployment
        {
            get { return NewMsp.MedicareEntitlement.SpouseEmployment; }
        }

        private static Employment OldPatientEmployment
        {
            get { return OldMsp.MedicareEntitlement.PatientEmployment; }
        }

        private static Employment OldSpouseEmployment
        {
            get { return OldMsp.MedicareEntitlement.SpouseEmployment; }
        }

        private static MedicareSecondaryPayor OldMsp
        {
            get { return OldAccount.MedicareSecondaryPayor; }
        }

        private static MedicareSecondaryPayor NewMsp
        {
            get { return NewAccount.MedicareSecondaryPayor; }
        }

        #endregion

        #region Data Elements
        private static readonly DateTime DialysisTrainingStartDate = new DateTime( 2008, 01, 01 );
        private static readonly DateTime DialysisDate = new DateTime( 2007, 01, 01 );
        private static readonly DateTime TransplantDate = new DateTime( 2008, 02, 02 );
        private static readonly DateTime OnsetDate = new DateTime( 2005, 01, 01 );
        private static readonly DateTime BLBenefitsStartDate = new DateTime( 2001, 01, 01 );
        private static readonly PhoneNumber CiscoPhone = new PhoneNumber( "9728881111" );

        private static readonly State State = new State( 0L,
                                                         PersistentModel.NEW_VERSION,
                                                         "TEXAS",
                                                         "TX" );

        private static readonly Country Country = new Country( 0L,
                                                               PersistentModel.NEW_VERSION,
                                                               "United States",
                                                               "USA" );

        private static readonly County County = new County( 0L,
                                                            PersistentModel.NEW_VERSION,
                                                            "ORANGE",
                                                            "122" );

        private static readonly ZipCode ZipCode = new ZipCode( POSTALCODE );
        private static readonly PhoneNumber PerotPhone = new PhoneNumber( "9725770000" );
        private static readonly DateTime PerotRetiredDate = new DateTime( 2005, 01, 01 );
        private static readonly YesNoFlag YesFlag = new YesNoFlag( YesNoFlag.CODE_YES );
        private static readonly YesNoFlag NoFlag = new YesNoFlag( YesNoFlag.CODE_NO );

        private static Account OldAccount = new Account();
        private static Account NewAccount = new Account();
        private static readonly Activity[] ActivityArray = new Activity[] 
                                                            { new RegistrationActivity(), 
                                                              new PreRegistrationActivity(), 
                                                              new ActivatePreRegistrationActivity()
                                                            };
        private static readonly ArrayList ActivityList = new ArrayList( ActivityArray );
        private readonly Facility facility = new Facility();
        private readonly IAccountCopyBroker accountCopyBroker = BrokerFactory.BrokerOfType<IAccountCopyBroker>();
        private readonly IInsuranceBroker insuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
        private readonly AccountCopyStrategy copyStrategy = new BinaryCloneAccountCopyStrategy();
        private static Facility DHF_FACILITY = new Facility();
        private static readonly IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        #endregion
    }
}
