using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Persistence.AccountCopy;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture()]
    public class CoverageDefaultsTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CoverageDefaultsTests
        [TestFixtureSetUp()]
        public static void SetUpCoverageDefaultsTests()
        {
            cd = new CoverageDefaults();
            service1 = new HospitalService(4, ReferenceValue.NEW_VERSION, "BLOOD TRANSFUSION", "63");
            service2 = new HospitalService(2, ReferenceValue.NEW_VERSION, "BLOOD TRANSFUSION", "43");
           
            visitType1 = new VisitType(0, DateTime.Now, "Inpatient", VisitType.INPATIENT_DESC);
            visitType2 = new VisitType(0, DateTime.Now, "OutPatient", VisitType.OUTPATIENT_DESC);
            
        }

        [TestFixtureTearDown()]
        public static void TearDownCoverageDefaultsTests()
        {
            cd = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        [Category( "Fast" )]
        public void TestCoverageDefaults()
        {
            Account anAccount = new Account();
            anAccount.Activity = new TransferBedSwapActivity();
            
            Coverage anCoverage = new CommercialCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            anCoverage = new GovernmentMedicareCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);
            
            cd.SetCoverageDefaultsForActivity(anAccount);

            anAccount.Insurance.RemovePrimaryCoverage();
            anAccount.Insurance.RemoveSecondaryCoverage();

            anCoverage = new GovernmentMedicaidCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            anCoverage = new SelfPayCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            cd.SetCoverageDefaultsForActivity(anAccount);
        }

        [Test()]
        [Category( "Fast" )]
        public void SetCoverageDefaultsForActivity_ShouldNotSetForPreadmitNewborn()
        {
            Account anAccount = new Account();
            anAccount.Activity = new PreAdmitNewbornActivity();

            Coverage anCoverage = new SelfPayCoverage();
            anCoverage.AuthorizingPerson = "Tester1";
            anCoverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();

            anAccount.Insurance.AddCoverage( anCoverage );

            anCoverage = new GovernmentMedicareCoverage();
            anCoverage.AuthorizingPerson = "Tester2";
            anCoverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            anAccount.Insurance.AddCoverage( anCoverage );

            cd.SetCoverageDefaultsForActivity( anAccount );
            Assert.AreSame( anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ).AuthorizingPerson, "Tester1", "Primary Coverage should not be reset" );
            Assert.AreSame( anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID).AuthorizingPerson, "Tester2", "Secondary Coverage should not be reset" );

        }
        [Test()]
        public void SetCoverageDefaultsForActivity_ShouldNotSetForDiagnosticRegistration()
        {
            Account anAccount = new Account();
            anAccount.Activity = new ShortRegistrationActivity();
            anAccount.HospitalService = service1;
            anAccount.KindOfVisit = visitType1;

            Coverage anCoverage = new SelfPayCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            anCoverage = new GovernmentMedicareCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            AccountCopyStrategy copyStrategy = new QuickAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount(anAccount);

            newAccount.HospitalService = service2;
            newAccount.KindOfVisit = visitType2;

            newAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).AuthorizingPerson = "Tester1";
            newAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID).AuthorizingPerson = "Tester2";

            cd.SetCoverageDefaultsForActivity(newAccount);
            Assert.AreSame(newAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).AuthorizingPerson, "Tester1", "Primary Coverage should not be reset");
            Assert.AreSame(newAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID).AuthorizingPerson, "Tester2", "Secondary Coverage should not be reset");

        }
        [Test()]
        public void SetCoverageDefaultsForActivity_ShouldNotSetForQuickAccountCreationActivity()
        {
            Account anAccount = new Account();
            anAccount.Activity = new QuickAccountCreationActivity();
            anAccount.HospitalService = service1;
            anAccount.KindOfVisit = visitType1;

            Coverage anCoverage = new SelfPayCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            anCoverage = new GovernmentMedicareCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            AccountCopyStrategy copyStrategy = new QuickAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount(anAccount);

            newAccount.HospitalService = service2;
            newAccount.KindOfVisit = visitType2;

            newAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).AuthorizingPerson = "Tester1";
            newAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID).AuthorizingPerson = "Tester2";

            cd.SetCoverageDefaultsForActivity(newAccount);
            Assert.AreSame(newAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).AuthorizingPerson, "Tester1", "Primary Coverage should not be reset");
            Assert.AreSame(newAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID).AuthorizingPerson, "Tester2", "Secondary Coverage should not be reset");

        }
        [Test()]
        [Category( "Fast" )]
        public void SetCoverageDefaultsForActivity_ShouldSetForTransferSwap()
        {
            Account anAccount = new Account();
            anAccount.Activity = new TransferBedSwapActivity();

            Coverage anCoverage = new SelfPayCoverage();
            anCoverage.AuthorizingPerson = "Tester1";
            anCoverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();

            anAccount.Insurance.AddCoverage( anCoverage );

            anCoverage = new GovernmentMedicareCoverage();
            anCoverage.AuthorizingPerson = "Tester2";
            anCoverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            anAccount.Insurance.AddCoverage( anCoverage );

            cd.SetCoverageDefaultsForActivity( anAccount );
            Assert.AreSame( anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID ).AuthorizingPerson, "", "Primary Coverage should be reset" );
            Assert.AreSame( anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID ).AuthorizingPerson, "", "Secondary Coverage should be reset" );
        }

        [Test()]
        public void SetCoverageDefaultsForActivity_ShouldSetForShortMaintenanceActivity()
        {
            Account anAccount = new Account();
            anAccount.Activity = new ShortMaintenanceActivity();

            anAccount.HospitalService = service1;
            anAccount.KindOfVisit = visitType1;

            Coverage anCoverage = new SelfPayCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            anCoverage = new GovernmentMedicareCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            AccountCopyStrategy copyStrategy = new QuickAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount(anAccount);

            newAccount.HospitalService = service2;
            newAccount.KindOfVisit = visitType2;

            newAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).AuthorizingPerson = "Tester1";
            newAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID).AuthorizingPerson = "Tester2";

            cd.SetCoverageDefaultsForActivity(newAccount);
            Assert.AreSame(newAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).AuthorizingPerson, "", "Primary Coverage should be reset");
            Assert.AreSame(newAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID).AuthorizingPerson, "", "Secondary Coverage should be reset");
        }

        [Test()]
        public void SetCoverageDefaultsForActivity_ShouldSetForQuickMaintenanceActivity()
        {
            Account anAccount = new Account();
            anAccount.Activity = new QuickAccountMaintenanceActivity();
            anAccount.HospitalService = service1;
            anAccount.KindOfVisit = visitType1;

            Coverage anCoverage = new SelfPayCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            anCoverage = new GovernmentMedicareCoverage();
            anCoverage.CoverageOrder = CoverageOrder.NewSecondaryCoverageOrder();
            anAccount.Insurance.AddCoverage(anCoverage);

            AccountCopyStrategy copyStrategy = new QuickAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount(anAccount);

            newAccount.HospitalService = service2;
            newAccount.KindOfVisit = visitType2;

            newAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).AuthorizingPerson = "Tester1";
            newAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID).AuthorizingPerson = "Tester2";

            cd.SetCoverageDefaultsForActivity(newAccount);
            Assert.AreSame(newAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).AuthorizingPerson, "", "Primary Coverage should be reset");
            Assert.AreSame(newAccount.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID).AuthorizingPerson, "", "Secondary Coverage should be reset");
        }
        
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static CoverageDefaults cd = null;
        private static HospitalService service1 = null;
        private static HospitalService service2 = null;
        private static VisitType visitType1 = null;
        private static VisitType visitType2 = null;

        #endregion
    }
}