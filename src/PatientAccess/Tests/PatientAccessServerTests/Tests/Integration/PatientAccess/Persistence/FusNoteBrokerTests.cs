using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;
using Xstream.Core;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class FusNoteBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown FusNoteBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpFusNoteBrokerTests()
        {
            fusNoteBroker = BrokerFactory.BrokerOfType<IFUSNoteBroker>();

            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_Facility = fb.FacilityWith(900);
        }

        [TestFixtureTearDown()]
        public static void TearDownFusNoteBrokerTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        [Ignore] // ("Kevin is researching why this code is invalid")] Ignoring until HDIService Exceptions are fixed ETA Jan, 4, 2018
        public void TestFusNote()
        {
            try
            {
                IFUSNoteBroker fusNoteBroker = BrokerFactory.BrokerOfType<IFUSNoteBroker>();
                Account anAccount = new Account(30015);
                FusActivity anActivity = fusNoteBroker.FusActivityWith("CAADC");

                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility aFacility = facilityBroker.FacilityWith("ACO");

                anAccount.Facility = aFacility;

                fusNoteBroker.PostRemarksFusNote(anAccount, "PACCESS", anActivity, "This is a test", DateTime.Now);
            }   
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }        
        
        [Test()]
        public void TestAllActivityCodesHashtable()
        {
            FusActivity activityABNGB = null;
            FusActivity activityABNNC = null;
            FusActivity activityABNNP = null;

            Hashtable allActivityCodes = fusNoteBroker.AllActivityCodesHashtable();

            activityABNGB = allActivityCodes[codeABNGB] as FusActivity;
            activityABNNC = allActivityCodes[codeABNNC] as FusActivity;
            activityABNNP = allActivityCodes[codeABNNP] as FusActivity;

            Assert.IsNotNull( activityABNGB,"Did not find ABNGB FusActivity object" );
            Assert.AreEqual( descABNGB, activityABNGB.Description, "ABNGB description is incorrect" );

            Assert.IsNotNull( activityABNNC,"Did not find ABNNC FusActivity object" );
            Assert.AreEqual( descABNNC, activityABNNC.Description, "ABNNC description is incorrect" );

            Assert.IsNotNull( activityABNNP,"Did not find ABNNP FusActivity object" );
            Assert.AreEqual( descABNNP, activityABNNP.Description, "ABNNP description is incorrect" );
        }

        [Test()]
        public void TestAllWriteableActivityCodesHashtable()
        {
            FusActivity activityABNGB = null;
            FusActivity activityABNNC = null;
            FusActivity activityABNNP = null;

            Hashtable allWriteableActivityCodes = fusNoteBroker.AllWriteableActivityCodesHashtable();

            activityABNGB = allWriteableActivityCodes[codeABNGB] as FusActivity;
            activityABNNC = allWriteableActivityCodes[codeABNNC] as FusActivity;
            activityABNNP = allWriteableActivityCodes[codeABNNP] as FusActivity;

            Assert.IsNotNull( activityABNGB,"Did not find ABNGB FusActivity object" );
            Assert.AreEqual( descABNGB, activityABNGB.Description, "ABNGB description is incorrect" );
            Assert.IsTrue( activityABNGB.Writeable, "Writable flag for ABNGB activity is incorrect" );

            Assert.IsNotNull( activityABNNC,"Did not find ABNNC FusActivity object" );
            Assert.AreEqual( descABNNC, activityABNNC.Description, "ABNNC description is incorrect" );
            Assert.IsTrue( activityABNNC.Writeable, "Writable flag for ABNNC activity is incorrect" );

            Assert.IsNotNull( activityABNNP,"Did not find ABNNP FusActivity object" );
            Assert.AreEqual( descABNNP, activityABNNP.Description, "ABNNP description is incorrect" );
            Assert.IsTrue( activityABNNP.Writeable, "Writable flag for ABNNP activity is incorrect" );
        }

        [Test()]
        public void TestFusActivityWith()
        {
            FusActivity activityABNGB = fusNoteBroker.FusActivityWith( codeABNGB );

            Assert.IsNotNull( activityABNGB,"Did not find ABNGB FusActivity object" );
            Assert.AreEqual( descABNGB, activityABNGB.Description, "ABNGB description is incorrect" );
            Assert.IsTrue( activityABNGB.Writeable, "Writable flag for ABNGB activity is incorrect" );
        }

        [Test()]
        public void TestFusActivityWithInvalidActivityCode()
        {
            FusActivity activity = fusNoteBroker.FusActivityWith( INVALID_CODE_ABNNP );

            Assert.IsNotNull( activity,"Did not find AAAAA FusActivity object" );
            Assert.AreEqual( INVALID_CODE_ABNNP, activity.Code, "AAAAA code is incorrect" );
            Assert.AreEqual( INVALID_ACTIVITY_DESCRIPTION, activity.Description, "AAAAA description is incorrect" );
        }
        
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteMedicaidFusNotes()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();

            Account account = this.CreateAccount();

            GovernmentMedicaidCoverage mdCov = new GovernmentMedicaidCoverage();
            mdCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY_OID");
            mdCov.Remarks = "Medicaid Remarks - These are test remarks for the Medicaid type Insurance Category. Patient has Medicaid type insurance.";
            mdCov.Oid = 212;

            mdCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now,
                                                                            "Information Received Source - Information received from Admitted patient");

            flag.SetNo();
            mdCov.EligibilityVerified = naFlag;

            account.Insurance.AddCoverage(mdCov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteMedicareFusNotes()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();

            Account account = this.CreateAccount();

            GovernmentMedicareCoverage mcCov = new GovernmentMedicareCoverage();
            mcCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY_OID");
            mcCov.Remarks = "Medicare Remarks - These are test remarks for the Medicare type Insurance category. Patient has Medicare Insurance category. ";
            mcCov.Oid = 212;
            mcCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now, "Information Received Source - Patient ");

            flag.SetNo();
            mcCov.EligibilityVerified = naFlag;

            account.Insurance.AddCoverage(mcCov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteWorkersCompFusNotes()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();

            Account account = this.CreateAccount();

            WorkersCompensationCoverage wcCov = new WorkersCompensationCoverage();
            wcCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY_OID");
            wcCov.Remarks = "Workers Compensation Remarks - These are test remarks for the Workers Compensation type Insurance category";
            wcCov.Oid = 212;
            wcCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now, "Information received Source");

            flag.SetNo();
            wcCov.EligibilityVerified = naFlag;

            account.Insurance.AddCoverage(wcCov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteGovernmentOtherFusNotes()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();

            Account account = this.CreateAccount();

            GovernmentOtherCoverage govCov = new GovernmentOtherCoverage();
            govCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY_OID");
            govCov.BenefitsCategoryDetails = new BenefitsCategoryDetails();
            govCov.BenefitsCategoryDetails.DeductibleDollarsMet = 100.50F;
            govCov.Remarks = "Government Other Remarks - These are test remarks for the Government Other or Government Miscellaneous Insurance category";
            govCov.Oid = 212;
            govCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now, "Information Received Source");

            flag.SetNo();
            govCov.EligibilityVerified = naFlag;

            account.Insurance.AddCoverage(govCov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteCommercialFusNotesWithNullOriginalCoverage()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();

            Account account = this.CreateAccount();

            CommercialCoverage commCov = new CommercialCoverage();
            commCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY_OID");
            commCov.PPOPricingOrBroker = "PPOPricingOrBroker";
            commCov.Remarks = "Commercial Coverage Remarks - These are test remarks for the Commerical Coverage type Insurance category";
            commCov.Oid = 212;
            commCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now, "Information Received Source");

            flag.SetNo();
            commCov.EligibilityVerified = naFlag;
            commCov.ClaimsAddressVerified = flag;

            //Create Benefits Categories & Benefits Category Details
            BenefitsCategory aOutPatient = new BenefitsCategory();
            aOutPatient.Oid = 2;
            aOutPatient.Description = "OutPatient";

            BenefitsCategoryDetails opBenefitsCategoryDetails = new BenefitsCategoryDetails();
            TimePeriodFlag time = new TimePeriodFlag();
            time.SetYear();
            opBenefitsCategoryDetails.TimePeriod = time;
            YesNoFlag trueFlag = new YesNoFlag();
            trueFlag.SetYes();
            YesNoFlag falseFlag = new YesNoFlag();
            falseFlag.SetNo();
            opBenefitsCategoryDetails.DeductibleMet = trueFlag;
            opBenefitsCategoryDetails.DeductibleDollarsMet = 50.00F;
            opBenefitsCategoryDetails.CoInsurance = 100;
            opBenefitsCategoryDetails.OutOfPocket = 10.00F;
            opBenefitsCategoryDetails.OutOfPocketMet = falseFlag;
            opBenefitsCategoryDetails.OutOfPocketDollarsMet = 0.0F;
            opBenefitsCategoryDetails.AfterOutOfPocketPercent = 90;
            opBenefitsCategoryDetails.WaiveCopayIfAdmitted = trueFlag;
            opBenefitsCategoryDetails.VisitsPerYear = 2;
            opBenefitsCategoryDetails.LifeTimeMaxBenefit = 1000.0F;
            opBenefitsCategoryDetails.RemainingLifetimeValueMet = falseFlag;
            opBenefitsCategoryDetails.MaxBenefitPerVisit = 200.0F;
            opBenefitsCategoryDetails.RemainingBenefitPerVisitsMet = falseFlag;

            BenefitsCategory aNewBorn = new BenefitsCategory();
            aNewBorn.Oid = 4;
            aNewBorn.Description = "NewBorn";
            time.SetVisit();
            BenefitsCategoryDetails nbBenefitsCategoryDetails = new BenefitsCategoryDetails();
            nbBenefitsCategoryDetails.TimePeriod = time;
            nbBenefitsCategoryDetails.DeductibleMet = trueFlag;
            nbBenefitsCategoryDetails.DeductibleDollarsMet = 25.00F;
            nbBenefitsCategoryDetails.CoInsurance = 200;
            nbBenefitsCategoryDetails.OutOfPocket = 20.00F;
            nbBenefitsCategoryDetails.OutOfPocketMet = falseFlag;
            nbBenefitsCategoryDetails.OutOfPocketDollarsMet = 0.0F;
            nbBenefitsCategoryDetails.AfterOutOfPocketPercent = 80;
            nbBenefitsCategoryDetails.WaiveCopayIfAdmitted = trueFlag;
            nbBenefitsCategoryDetails.VisitsPerYear = 3;
            nbBenefitsCategoryDetails.LifeTimeMaxBenefit = 1000.0F;
            nbBenefitsCategoryDetails.RemainingLifetimeValueMet = falseFlag;
            nbBenefitsCategoryDetails.MaxBenefitPerVisit = 300.0F;
            nbBenefitsCategoryDetails.RemainingBenefitPerVisitsMet = falseFlag;

            commCov.AddBenefitsCategory(aOutPatient, opBenefitsCategoryDetails);
            commCov.AddBenefitsCategory(aNewBorn, nbBenefitsCategoryDetails);

            Payor p = new Payor();
            p.Code = "AA";
            p.Name = "AETNA";

            InsurancePlan plan = new CommercialInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "Commercial";
            plan.Payor = p;
            commCov.InsurancePlan = plan;
            commCov.WriteVerificationEntryFUSNote = true;

            account.Insurance.AddCoverage(commCov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteCommercialFusNotes()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();

            Account account = this.CreateAccount();

            // Current Coverage
            CommercialCoverage commCov = new CommercialCoverage();
            commCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY_OID");
            commCov.PPOPricingOrBroker = "PPOPricingOrBroker";
            commCov.Remarks = "Commercial Coverage Remarks - These are test remarks for the Commerical Coverage type Insurance category";
            commCov.Oid = 212;
            commCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now, "Information Received Source");

            flag.SetNo();
            commCov.EligibilityVerified = naFlag;
            commCov.ClaimsAddressVerified = flag;

            //Create Benefits Categories & Benefits Category Details
            BenefitsCategory aOutPatient = new BenefitsCategory();
            aOutPatient.Oid = 2;
            aOutPatient.Description = "OutPatient";

            BenefitsCategoryDetails opBenefitsCategoryDetails = new BenefitsCategoryDetails();
            TimePeriodFlag time = new TimePeriodFlag();
            time.SetYear();
            opBenefitsCategoryDetails.TimePeriod = time;
            YesNoFlag trueFlag = new YesNoFlag();
            trueFlag.SetYes();
            YesNoFlag falseFlag = new YesNoFlag();
            falseFlag.SetNo();
            opBenefitsCategoryDetails.DeductibleMet = trueFlag;
            opBenefitsCategoryDetails.DeductibleDollarsMet = 50.00F;
            opBenefitsCategoryDetails.CoInsurance = 100;
            opBenefitsCategoryDetails.OutOfPocket = 10.00F;
            opBenefitsCategoryDetails.OutOfPocketMet = falseFlag;
            opBenefitsCategoryDetails.OutOfPocketDollarsMet = 0.0F;
            opBenefitsCategoryDetails.AfterOutOfPocketPercent = 90;
            opBenefitsCategoryDetails.WaiveCopayIfAdmitted = trueFlag;
            opBenefitsCategoryDetails.VisitsPerYear = 2;
            opBenefitsCategoryDetails.LifeTimeMaxBenefit = 1000.0F;
            opBenefitsCategoryDetails.RemainingLifetimeValueMet = falseFlag;
            opBenefitsCategoryDetails.MaxBenefitPerVisit = 200.0F;
            opBenefitsCategoryDetails.RemainingBenefitPerVisitsMet = falseFlag;

            BenefitsCategory aNewBorn = new BenefitsCategory();
            aNewBorn.Oid = 4;
            aNewBorn.Description = "NewBorn";
            time.SetVisit();
            BenefitsCategoryDetails nbBenefitsCategoryDetails = new BenefitsCategoryDetails();
            nbBenefitsCategoryDetails.TimePeriod = time;
            nbBenefitsCategoryDetails.DeductibleMet = trueFlag;
            nbBenefitsCategoryDetails.DeductibleDollarsMet = 25.00F;
            nbBenefitsCategoryDetails.CoInsurance = 200;
            nbBenefitsCategoryDetails.OutOfPocket = 20.00F;
            nbBenefitsCategoryDetails.OutOfPocketMet = falseFlag;
            nbBenefitsCategoryDetails.OutOfPocketDollarsMet = 0.0F;
            nbBenefitsCategoryDetails.AfterOutOfPocketPercent = 80;
            nbBenefitsCategoryDetails.WaiveCopayIfAdmitted = trueFlag;
            nbBenefitsCategoryDetails.VisitsPerYear = 3;
            nbBenefitsCategoryDetails.LifeTimeMaxBenefit = 1000.0F;
            nbBenefitsCategoryDetails.RemainingLifetimeValueMet = falseFlag;
            nbBenefitsCategoryDetails.MaxBenefitPerVisit = 300.0F;
            nbBenefitsCategoryDetails.RemainingBenefitPerVisitsMet = falseFlag;

            commCov.AddBenefitsCategory(aOutPatient, opBenefitsCategoryDetails);
            commCov.AddBenefitsCategory(aNewBorn, nbBenefitsCategoryDetails);

            Payor p = new Payor();
            p.Code = "AA";
            p.Name = "AETNA";
            InsurancePlan plan = new CommercialInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "Commercial";
            plan.Payor = p;
            commCov.InsurancePlan = plan;
            commCov.WriteVerificationEntryFUSNote = true;

            account.Insurance.AddCoverage(commCov);

            // Original Coverage
            CommercialCoverage origCov = new CommercialCoverage();
            origCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY_OID");
            origCov.PPOPricingOrBroker = "PPOPricingOrBroker";
            origCov.Remarks = "Commercial Coverage Remarks - These are test remarks for the Commerical Coverage type Insurance category";
            origCov.Oid = 212;
            origCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now, "Information Received Source");

            flag.SetNo();
            origCov.EligibilityVerified = naFlag;
            origCov.ClaimsAddressVerified = flag;

            //Create Benefits Categories & Benefits Category Details
            BenefitsCategory aOrigOutPatient = new BenefitsCategory();
            aOrigOutPatient.Oid = 2;
            aOrigOutPatient.Description = "OutPatient";

            BenefitsCategoryDetails opOrigBenefitsCategoryDetails = new BenefitsCategoryDetails();
            time.SetYear();
            opOrigBenefitsCategoryDetails.TimePeriod = time;
            trueFlag.SetYes();
            falseFlag.SetNo();
            opOrigBenefitsCategoryDetails.DeductibleMet = falseFlag;
            opOrigBenefitsCategoryDetails.DeductibleDollarsMet = 5.00F;
            opOrigBenefitsCategoryDetails.CoInsurance = 100;
            opOrigBenefitsCategoryDetails.OutOfPocket = 20.00F;
            opOrigBenefitsCategoryDetails.OutOfPocketMet = trueFlag;
            opOrigBenefitsCategoryDetails.OutOfPocketDollarsMet = 0.0F;
            opOrigBenefitsCategoryDetails.AfterOutOfPocketPercent = 90;
            opOrigBenefitsCategoryDetails.WaiveCopayIfAdmitted = trueFlag;
            opOrigBenefitsCategoryDetails.VisitsPerYear = 3;
            opOrigBenefitsCategoryDetails.LifeTimeMaxBenefit = 1000.0F;
            opOrigBenefitsCategoryDetails.RemainingLifetimeValueMet = falseFlag;
            opOrigBenefitsCategoryDetails.MaxBenefitPerVisit = 200.0F;
            opOrigBenefitsCategoryDetails.RemainingBenefitPerVisitsMet = falseFlag;

            origCov.AddBenefitsCategory(aOrigOutPatient, opOrigBenefitsCategoryDetails);

            Payor op = new Payor();
            op.Code = "BC";
            op.Name = "BCBS";
            InsurancePlan oPlan = new CommercialInsurancePlan();
            oPlan.Oid = 1;
            oPlan.PlanName = "Commercial";
            oPlan.Payor = op;
            origCov.InsurancePlan = oPlan;

            account.Insurance.AddOrigCoverage(origCov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteOtherCoverageFusNotes()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();

            Account account = this.CreateAccount();

            OtherCoverage otherCov = new OtherCoverage();
            otherCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY_OID");
            otherCov.Remarks = "Other Coverage Remarks - These are test remarks for the Other Coverage type Insurance category";
            otherCov.Oid = 212;
            otherCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now, "Information Received Source");

            flag.SetNo();
            otherCov.EligibilityVerified = naFlag;

            //Create Benefits Categories & Benefits Category Details
            BenefitsCategory aOutPatient = new BenefitsCategory();
            aOutPatient.Oid = 1;
            aOutPatient.Description = "InPatient";

            BenefitsCategoryDetails opBenefitsCategoryDetails = new BenefitsCategoryDetails();
            TimePeriodFlag time = new TimePeriodFlag();
            time.SetYear();
            opBenefitsCategoryDetails.TimePeriod = time;
            YesNoFlag trueFlag = new YesNoFlag();
            trueFlag.SetYes();
            YesNoFlag falseFlag = new YesNoFlag();
            falseFlag.SetNo();
            opBenefitsCategoryDetails.DeductibleMet = trueFlag;
            opBenefitsCategoryDetails.DeductibleDollarsMet = 20.00F;
            opBenefitsCategoryDetails.CoInsurance = 200;
            opBenefitsCategoryDetails.OutOfPocket = 20.00F;
            opBenefitsCategoryDetails.OutOfPocketMet = falseFlag;
            opBenefitsCategoryDetails.OutOfPocketDollarsMet = 0.0F;
            opBenefitsCategoryDetails.AfterOutOfPocketPercent = 90;
            opBenefitsCategoryDetails.WaiveCopayIfAdmitted = trueFlag;
            opBenefitsCategoryDetails.VisitsPerYear = 2;
            opBenefitsCategoryDetails.LifeTimeMaxBenefit = 2000.0F;
            opBenefitsCategoryDetails.RemainingLifetimeValueMet = falseFlag;
            opBenefitsCategoryDetails.MaxBenefitPerVisit = 200.0F;
            opBenefitsCategoryDetails.RemainingBenefitPerVisitsMet = falseFlag;

            BenefitsCategory aNewBorn = new BenefitsCategory();
            aNewBorn.Oid = 4;
            aNewBorn.Description = "NICU";
            time.SetVisit();
            BenefitsCategoryDetails nbBenefitsCategoryDetails = new BenefitsCategoryDetails();
            nbBenefitsCategoryDetails.TimePeriod = time;
            nbBenefitsCategoryDetails.DeductibleMet = trueFlag;
            nbBenefitsCategoryDetails.DeductibleDollarsMet = 30.00F;
            nbBenefitsCategoryDetails.CoInsurance = 300;
            nbBenefitsCategoryDetails.OutOfPocket = 30.00F;
            nbBenefitsCategoryDetails.OutOfPocketMet = falseFlag;
            nbBenefitsCategoryDetails.OutOfPocketDollarsMet = 0.0F;
            nbBenefitsCategoryDetails.AfterOutOfPocketPercent = 80;
            nbBenefitsCategoryDetails.WaiveCopayIfAdmitted = trueFlag;
            nbBenefitsCategoryDetails.VisitsPerYear = 3;
            nbBenefitsCategoryDetails.LifeTimeMaxBenefit = 3000.0F;
            nbBenefitsCategoryDetails.RemainingLifetimeValueMet = falseFlag;
            nbBenefitsCategoryDetails.MaxBenefitPerVisit = 300.0F;
            nbBenefitsCategoryDetails.RemainingBenefitPerVisitsMet = falseFlag;

            otherCov.AddBenefitsCategory(aOutPatient, opBenefitsCategoryDetails);
            otherCov.AddBenefitsCategory(aNewBorn, nbBenefitsCategoryDetails);

            account.Insurance.AddCoverage(otherCov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRBVCAFusNotes()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();
            Account anAccount = this.CreateAccount();

            CommercialCoverage cov = new CommercialCoverage();
            cov.BenefitsVerified = naFlag;
            cov.AuthorizingPerson = "John Smith";
            cov.DateTimeOfVerification = new DateTime(2005, 3, 5);
            cov.WriteBenefitsVerifiedFUSNote = true;
            cov.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();

            anAccount.Insurance.AddCoverage(cov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = anAccount;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRARRAFusNotes()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();
            Account anAccount = this.CreateAccount();

            CommercialCoverage cov = new CommercialCoverage();
            cov.Authorization.AuthorizationRequired = naFlag;
            cov.Authorization.AuthorizationCompany = "Blue Cross Blue Shield";
            cov.Authorization.AuthorizationPhone = new PhoneNumber( "9729729722" );
            cov.WriteAuthRequiredFUSNote = true;
            cov.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();

            CommercialCoverage origCov = new CommercialCoverage();
            origCov.Authorization.AuthorizationRequired = naFlag;
            origCov.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();

            anAccount.Insurance.AddCoverage(cov);
            anAccount.Insurance.AddOrigCoverage(origCov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = anAccount;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWritePTRPRFUSNotes()
        {
           
            Account anAccount = this.CreateAccount();
            if( anAccount.Diagnosis != null )
            {
                anAccount.Diagnosis.isPrivateAccommodationRequested = true;
            }

            Activity activity = new RegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor( activity );
            txncoord.Account = anAccount;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestInsertFusNotesIntoAccount()
        {
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();

            Account account = this.CreateAccount();

            // Medicare Coverage
            GovernmentMedicareCoverage mcCov = new GovernmentMedicareCoverage();
            mcCov.WriteVerificationEntryFUSNote = true;
            mcCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID, "PRIMARY_OID");
            mcCov.Remarks = "Medicare Remarks - Testing Insert Fus Notes into account for the Medicare type Insurance category. Patient has Medicare Insurance category. ";
            mcCov.Oid = 212;
            mcCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now, "Information Received Source - Patient ");

            flag.SetNo();
            mcCov.EligibilityVerified = naFlag;

            account.Insurance.AddCoverage(mcCov);

            // Workers Compensation Coverage
            WorkersCompensationCoverage wcCov = new WorkersCompensationCoverage();
            wcCov.WriteVerificationEntryFUSNote = true;
            wcCov.CoverageOrder = new CoverageOrder(CoverageOrder.SECONDARY_OID, "PRIMARY_OID");
            wcCov.Remarks = "Workers Compensation Remarks - Testing Insert Fus Notes into account for the Workers Compensation type Insurance category";
            wcCov.Oid = 212;
            wcCov.InformationReceivedSource = new InformationReceivedSource(2, DateTime.Now, "Information received Source");
            wcCov.Authorization.AuthorizationRequired = naFlag;
            wcCov.Authorization.AuthorizationCompany = "Aetna Insurance";
            wcCov.Authorization.AuthorizationPhone = new PhoneNumber( "1112223333" );

            flag.SetNo();
            wcCov.EligibilityVerified = naFlag;

            account.Insurance.AddCoverage(wcCov);

            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRFCMOFusNotes()
        {
            Account anAccount = this.CreateAccount();

            FusNoteFactory fac = new FusNoteFactory();
            fac.AddRFCMONoteTo(anAccount);

            fusNoteBroker.WriteFUSNotes( anAccount, "PACCESS" );
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRFCACFusNotes()
        {
            Account anAccount = this.CreateAccount();

            FusNoteFactory fac = new FusNoteFactory();
            fac.AddRFCACNoteTo(anAccount);

            fusNoteBroker.WriteFUSNotes( anAccount, "PACCESS" );
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRCALCFusNotes()
        {
            Account anAccount = this.CreateAccount();

            FusNoteFactory fac = new FusNoteFactory();
            fac.AddRCALCNoteTo(anAccount);

            fusNoteBroker.WriteFUSNotes( anAccount, "PACCESS" );
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRFCNLFusNotes()
        {
            Account anAccount = this.CreateAccount();
            // When Total current amount due has been paid off and become zero
            anAccount.TotalCurrentAmtDue = 0M;

            FusNoteFactory fac = new FusNoteFactory();
            fac.AddRFCNLNoteTo(anAccount);

            fusNoteBroker.WriteFUSNotes( anAccount, "PACCESS" );
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestRFCNLFusNotesWithNoLiability()
        {
            Account anAccount = this.CreateAccount();
            FusNoteFactory fac = new FusNoteFactory();
            
            // When 'Patient has no Liability' check box is checked on Liability screen
            anAccount.Insurance.HasNoLiability = true;
            fac.AddRFCNLNoteTo(anAccount);

            fusNoteBroker.WriteFUSNotes( anAccount, "PACCESS" );            
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRPFCRFusNotes()
        {
            Account anAccount = this.CreateAccount();

            FusNoteFactory fac = new FusNoteFactory();
            fac.AddRPFCRNoteTo(anAccount);

            fusNoteBroker.WriteFUSNotes( anAccount, "PACCESS" );
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestInsertFinancialFusNotesIntoAccount()
        {
            Account account = this.CreateAccountWithPreviousAccounts();
            account.Diagnosis.isPrivateAccommodationRequested = true;
            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
        }
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestInsertRPFCRNoteToAccount_ShouldCreateRPFCRWhenBillhasNotDropped()
        {
            Account account = this.CreateAccount();
            account.Diagnosis.isPrivateAccommodationRequested = true;
            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();

            bool containsRPFCR = account.FusNotes.Cast<FusNote>().Any(x => x.FusActivity.Code == "RPFCR");

            Assert.IsTrue( containsRPFCR, "RPFCR fusnote should be created when Bill has not dropped" );
            
        }
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestInsertRPFCRNoteToAccount_ShouldNotCreateRPFCRWhenBillhasDropped()
        {
            Account account = this.CreateAccount();
            account.BillHasDropped = true;
            account.Diagnosis.isPrivateAccommodationRequested = true;
            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
            
            bool containsRPFCR = account.FusNotes.Cast<FusNote>().Any(x => x.FusActivity.Code == "RPFCR");
            
            Assert.IsFalse(containsRPFCR, "RPFCR fusnote should not be created when Bill has dropped");
            
        }
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestInsertRFCMONoteToAccount_ShouldCreateRPFCRWhenBillhasNotDropped()
        {
            Account account = this.CreateAccount();
            account.Diagnosis.isPrivateAccommodationRequested = true;
            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();
            
            bool containsRFCMO = account.FusNotes.Cast<FusNote>().Any(x => x.FusActivity.Code == "RFCMO");

            Assert.IsTrue(containsRFCMO, "RFCMO fusnote should be created when Bill has not dropped"); 
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestInsertRFCMONoteToAccount_ShouldNotCreateRPFCRWhenBillhasDropped()
        {
            Account account = this.CreateAccount();
            account.BillHasDropped = true;
            account.Diagnosis.isPrivateAccommodationRequested = true;
            Activity activity = new PreRegistrationActivity();
            TransactionCoordinator txncoord = TransactionCoordinator.TransactionCoordinatorFor(activity);
            txncoord.Account = account;
            txncoord.AppUser.PBAREmployeeID = "PACCESS";

            txncoord.WriteFUSNotesForAccount();

            bool containsRFCMO = account.FusNotes.Cast<FusNote>().Any(x => x.FusActivity.Code == "RFCMO");

            Assert.IsFalse( containsRFCMO, "RFCMO fusnote should not be created when Bill has dropped" );
            
        }

        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRPRTSFUSNotes()
        {

            Account anAccount = this.CreateAccount();
            ConditionOfService cosRefused = new ConditionOfService( 0, DateTime.Now, ConditionOfService.REFUSED_DESCRIPTION, ConditionOfService.REFUSED );
            anAccount.COSSigned = cosRefused;
            fusNoteFactory.AddRPRTSFUSNoteTo( anAccount );
            fusNoteBroker.WriteFUSNotes( anAccount, PBAR_EMPLOYEEID );
        }
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRPUTSFUSNotes()
        {

            Account anAccount = this.CreateAccount();
            ConditionOfService cos = new ConditionOfService( 0, DateTime.Now, ConditionOfService.UNABLE_DESCRIPTION, ConditionOfService.UNABLE );
            anAccount.COSSigned = cos;
            fusNoteFactory.AddRPUTSFUSNoteTo( anAccount );
            fusNoteBroker.WriteFUSNotes( anAccount, PBAR_EMPLOYEEID );
        }
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteRPNASFUSNotes()
        {
            Account anAccount = this.CreateAccount();
            ConditionOfService cos = new ConditionOfService( 0, DateTime.Now, ConditionOfService.NOT_AVAILABLE_DESCRIPTION, ConditionOfService.NOT_AVAILABLE );
            anAccount.COSSigned = cos;
            fusNoteFactory.AddRPNASFUSNoteTo( anAccount );
            fusNoteBroker.WriteFUSNotes( anAccount, PBAR_EMPLOYEEID );
        }
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteIICOSFUSNotes()
        {
            Account anAccount = this.CreateAccount();
            ConditionOfService cos = new ConditionOfService( 0, DateTime.Now, ConditionOfService.NOT_AVAILABLE_DESCRIPTION, ConditionOfService.NOT_AVAILABLE );
            anAccount.COSSigned = cos;
            fusNoteFactory.AddRPNASFUSNoteTo( anAccount );
            fusNoteBroker.WriteFUSNotes( anAccount, PBAR_EMPLOYEEID );
        }
        [Test()]
        [Ignore] // Ignoring until HDIService Exceptions in iTest are fixed by HDI team ETA Jan, 4, 2018
        public void TestWriteICOSCFUSNotes()
        {
            Account anAccount = this.CreateAccount();
            ConditionOfService cos = new ConditionOfService( 0, DateTime.Now, ConditionOfService.YES_DESCRIPTION, ConditionOfService.YES );
            anAccount.COSSigned = cos;
            fusNoteFactory.AddICOSCFUSNoteTo( anAccount );
            fusNoteBroker.WriteFUSNotes( anAccount, PBAR_EMPLOYEEID );
        }

        [Test]
        public void TestMergeExtensionNotesWithPatentNotes_MergedNoteCountShouldEqualNumberOfNonExtensionNotesInTheInput()
        {
            List<ExtendedFUSNote> inputFusNotes = GetInputFusNotesForMergeTest();
            
            var extensionNoteCount = inputFusNotes.Count( x => x.IsExtensionNote );
            
            var mergedFusNotes = FusNoteBroker.MergeExtensionNotesWithParentNotes( inputFusNotes );
            
            var expectedNumberOfMergedNotes = inputFusNotes.Count - extensionNoteCount;
            var actualNumberOfMergedNotes = mergedFusNotes.Count;
            
            Assert.AreEqual( expectedNumberOfMergedNotes, actualNumberOfMergedNotes );
        }

        [Test]
        public void TestMergeExtensionNotesWithPatentNotes_OutputShouldNotContainAnyExtensionNotes()
        {
            List<ExtendedFUSNote> inputFusNotes = GetInputFusNotesForMergeTest();

            var mergedFusNotes = FusNoteBroker.MergeExtensionNotesWithParentNotes( inputFusNotes );
            
            Assert.IsFalse( mergedFusNotes.Any( x => x.IsExtensionNote ), "Merged FUS notes should not contain any extension notes" );
        }

        [Test]
        public void TestMergeExtensionNotesWithPatentNotes_AllExtensionNotesShouldBeMerged()
        {
            List<ExtendedFUSNote> inputFusNotes = GetInputFusNotesForMergeTest();

            inputFusNotes.ForEach( x => x.Remarks = Guid.NewGuid().ToString() );
            var extensionNotes = inputFusNotes.Where( x => x.IsExtensionNote );
            var mergedFusNotes = FusNoteBroker.MergeExtensionNotesWithParentNotes( inputFusNotes );
            
            int numberOfNotesNotMerged = 0;
            
            foreach (var extensionNote in extensionNotes)
            {
                ExtendedFUSNote note = extensionNote;
                if (!mergedFusNotes.Any( x => x.Remarks.Contains( note.Remarks ) ))
                {
                    Debug.WriteLine("The extension note created on "+ note.CreatedOn + " by " + note.UserID + " was not merged");
                    numberOfNotesNotMerged++;
                }
            }

            Assert.AreEqual( 0, numberOfNotesNotMerged,"Not all extension notes were merged" );
        }

        #endregion

        #region Support Methods

        /// <summary>
        /// Gets the input FUS notes for merge test. This data was captured using xtream
        /// from the FUS notes for account number 2557296 in the model office environment
        /// where defect 2353 was discovered. The actual remarks of the FUS notes are
        /// replaced with GUIDs to facilitate unit tests
        /// </summary>
        /// <returns></returns>
        private static List<ExtendedFUSNote> GetInputFusNotesForMergeTest()
        {

            string fusNotesXml =

            #region sample data
 @"<System.Collections.ArrayList>
  <_items>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800190000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity>
        <i_StrategyName />
        <i_DefaultWorklistDays>0</i_DefaultWorklistDays>
        <i_MaxWorklistDays>0</i_MaxWorklistDays>
        <i_NoteType>Type07</i_NoteType>
        <i_Writeable>True</i_Writeable>
        <i_Code>CREMC</i_Code>
        <i_IsValid>True</i_IsValid>
        <i_Description>REMARKS CONTINUED</i_Description>
        <i_Oid>0</i_Oid>
        <i_Timestamp>0</i_Timestamp>
        <i_ChangedEvent null='True' />
        <i_ChangeTracker null='True' />
        <i_SecondaryProperties null='True' />
      </i_FUSActivity>
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>authorized  requestor. Any unauthorized use or disclosure of this information is prohibited.   Transaction run on 4/17/2009 at 3:42:45 PM CT by Kenneth Eric McKenzie - Tenet  - CBO/SOS, CA            </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800180000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Passport Reference Number: 20090417-3939411   NOTICE: This information is classified as individually identifiable healthcare  information and is intended strictly for the confidential use of the      </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800170000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>01/01/2003                 Period End:                      12/31/3999                 Insured or Subscriber Name:      NEWTON, HELEN I            Member ID Number:                452149989           </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800160000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Description:                     Y                          Group Number:                    080946                     Group Name:                      TEXAS INSTRUMENTS          Period Start:       </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800150000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>HEALTH BENEFIT PLAN COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer  Cov Level:                       Ind                        Insurance Type:                  Other         </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800140000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Address:                         P O BOX 660044                                                   DALLAS, TX 75266                Work Phone:                      (800) 451-0287                   --  </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800130000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Period Start:                    01/01/2003                      Period End:                      12/31/3999                      Payer Name:                      BLUE CROSS BLUE SHIELD OF TEXA       </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800120000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Description:                     Y                               Group Number:                    080946                          Group Name:                      TEXAS INSTRUMENTS                    </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800110000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer       Cov Level:                       Ind                             Insurance Type:                  Other                   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800100000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Date:                      02/12/2001                 Eligibility Begin Date:          11/01/2000                 Eligibility End Date:            04/17/2009                  -- HEALTH BENEFIT PLAN   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800090000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer  Cov Level:                       Ind                        Insurance Type:                  Medicare Part B            Added  </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800080000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity>
        <i_StrategyName />
        <i_DefaultWorklistDays>0</i_DefaultWorklistDays>
        <i_MaxWorklistDays>0</i_MaxWorklistDays>
        <i_NoteType>Type07</i_NoteType>
        <i_Writeable>True</i_Writeable>
        <i_Code>RDOTV</i_Code>
        <i_IsValid>True</i_IsValid>
        <i_Description>DATA VALIDATION</i_Description>
        <i_Oid>0</i_Oid>
        <i_Timestamp>0</i_Timestamp>
        <i_ChangedEvent null='True' />
        <i_ChangeTracker null='True' />
        <i_SecondaryProperties null='True' />
      </i_FUSActivity>
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>PRIMARY PAYOR VERIFICATION INFORMATION; PAYOR: MEDICAID-TX; INFO REC'D FROM: System electronic verification;                                                                                            </i_Remarks>
      <i_Text />
      <i_UserID>PASTXP</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800080000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Date:                      01/20/2009                 Eligibility Begin Date:          06/01/1984                 Eligibility End Date:            04/17/2009                  -- HEALTH BENEFIT PLAN   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800070000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer  Cov Level:                       Ind                        Insurance Type:                  Medicare Part A            Added  </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800060000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>MEDICARE BENE  Added Date:             09/19/2008                    Eligibility Begin Date: 11/01/2008                    Eligibility End Date:   04/17/2009                     -- HEALTH BENEFIT PLAN</i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800050000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Messages:               57 MQMB (MAO, SSI RELATED)                            Q                                                     R REGULAR                     Description:            140 MCAID QUAL</i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800040000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Status:                 ACTIVE COV                    Cov Level:              Ind                           Svc Type:               Health Bnft Plan Cov          Insurance Type:         Medicaid      </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800030000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>FEMALE                 -- ACTIVE COVERAGE  -- Cov Lvl Svc Type             Plan Cov Desc  ------- -------------------- -------------  Ind     Health Bnft Plan Cov NURSE HOME      -- COVERAGE  --      </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800020000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>452149989A            Address:          MANOR AT SEAGOVILLE                     SEAGOVILLE, TX 75159  County/Parish:    DALLAS                Date of Birth:    06/11/1919            Sex:              </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800010000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Beginning Date of Service: 04/17/2009  Ending Date of Service:    04/17/2009   -- SUBSCRIBER  -- Name:             NEWTON, HELEN I       Member ID Number: 520418826             HIC Number:            </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755800000000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='15' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Passport Health Communications Inc.  Texas Medicaid Eligibility   ---- Recipient is Eligible ----  -- SEARCH CRITERIA  -- NPI:                       1265468946  Recipient ID:              520418826   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695740000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>authorized  requestor. Any unauthorized use or disclosure of this information is prohibited.   Transaction run on 4/17/2009 at 12:51:45 PM CT by Kenneth Eric McKenzie - Tenet  - CBO/SOS, CA           </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695730000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Passport Reference Number: 20090417-3817298   NOTICE: This information is classified as individually identifiable healthcare  information and is intended strictly for the confidential use of the      </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695720000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>01/01/2003                 Period End:                      12/31/3999                 Insured or Subscriber Name:      NEWTON, HELEN I            Member ID Number:                452149989           </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695710000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Description:                     Y                          Group Number:                    080946                     Group Name:                      TEXAS INSTRUMENTS          Period Start:       </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695700000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>HEALTH BENEFIT PLAN COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer  Cov Level:                       Ind                        Insurance Type:                  Other         </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695690000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Address:                         P O BOX 660044                                                   DALLAS, TX 75266                Work Phone:                      (800) 451-0287                   --  </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695680000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Period Start:                    01/01/2003                      Period End:                      12/31/3999                      Payer Name:                      BLUE CROSS BLUE SHIELD OF TEXA       </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695670000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Description:                     Y                               Group Number:                    080946                          Group Name:                      TEXAS INSTRUMENTS                    </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695660000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer       Cov Level:                       Ind                             Insurance Type:                  Other                   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695650000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Date:                      02/12/2001                 Eligibility Begin Date:          11/01/2000                 Eligibility End Date:            04/17/2009                  -- HEALTH BENEFIT PLAN   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695640000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer  Cov Level:                       Ind                        Insurance Type:                  Medicare Part B            Added  </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695630000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Date:                      01/20/2009                 Eligibility Begin Date:          06/01/1984                 Eligibility End Date:            04/17/2009                  -- HEALTH BENEFIT PLAN   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695620000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer  Cov Level:                       Ind                        Insurance Type:                  Medicare Part A            Added  </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695610000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>MEDICARE BENE  Added Date:             09/19/2008                    Eligibility Begin Date: 11/01/2008                    Eligibility End Date:   04/17/2009                     -- HEALTH BENEFIT PLAN</i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695600000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Messages:               57 MQMB (MAO, SSI RELATED)                            Q                                                     R REGULAR                     Description:            140 MCAID QUAL</i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695590000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Status:                 ACTIVE COV                    Cov Level:              Ind                           Svc Type:               Health Bnft Plan Cov          Insurance Type:         Medicaid      </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695580000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>authorized  requestor. Any unauthorized use or disclosure of this information is prohibited.   Transaction run on 4/17/2009 at 12:51:39 PM CT by Kenneth Eric McKenzie - Tenet  - CBO/SOS, CA           </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695570000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Passport Reference Number: 20090417-3817238   NOTICE: This information is classified as individually identifiable healthcare  information and is intended strictly for the confidential use of the      </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695560000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>01/01/2003                 Period End:                      12/31/3999                 Insured or Subscriber Name:      NEWTON, HELEN I            Member ID Number:                452149989           </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695550000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Description:                     Y                          Group Number:                    080946                     Group Name:                      TEXAS INSTRUMENTS          Period Start:       </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695540000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>HEALTH BENEFIT PLAN COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer  Cov Level:                       Ind                        Insurance Type:                  Other         </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695530000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Address:                         P O BOX 660044                                                   DALLAS, TX 75266                Work Phone:                      (800) 451-0287                   --  </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695520000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Period Start:                    01/01/2003                      Period End:                      12/31/3999                      Payer Name:                      BLUE CROSS BLUE SHIELD OF TEXA       </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695510000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Description:                     Y                               Group Number:                    080946                          Group Name:                      TEXAS INSTRUMENTS                    </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0</i_Dollar1>
      <i_Dollar2>0</i_Dollar2>
      <i_Month null='True' />
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695500000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity>
        <i_StrategyName />
        <i_DefaultWorklistDays>0</i_DefaultWorklistDays>
        <i_MaxWorklistDays>0</i_MaxWorklistDays>
        <i_NoteType>Type00</i_NoteType>
        <i_Writeable>True</i_Writeable>
        <i_Code>NWACT</i_Code>
        <i_IsValid>True</i_IsValid>
        <i_Description>NEW ACCOUNT ADDED BY A/R SYSTEM</i_Description>
        <i_Oid>0</i_Oid>
        <i_Timestamp>0</i_Timestamp>
        <i_ChangedEvent null='True' />
        <i_ChangeTracker null='True' />
        <i_SecondaryProperties null='True' />
      </i_FUSActivity>
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks />
      <i_Text>041709-FC 50  PT 3 BY PM03</i_Text>
      <i_UserID>FUSYSTEM</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695500000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer       Cov Level:                       Ind                             Insurance Type:                  Other                   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695490000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Date:                      02/12/2001                 Eligibility Begin Date:          11/01/2000                 Eligibility End Date:            04/17/2009                  -- HEALTH BENEFIT PLAN   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695480000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity>
        <i_StrategyName />
        <i_DefaultWorklistDays>0</i_DefaultWorklistDays>
        <i_MaxWorklistDays>0</i_MaxWorklistDays>
        <i_NoteType>Type07</i_NoteType>
        <i_Writeable>True</i_Writeable>
        <i_Code>INPOF</i_Code>
        <i_IsValid>True</i_IsValid>
        <i_Description>NPP ON FILE</i_Description>
        <i_Oid>0</i_Oid>
        <i_Timestamp>0</i_Timestamp>
        <i_ChangedEvent null='True' />
        <i_ChangeTracker null='True' />
        <i_SecondaryProperties null='True' />
      </i_FUSActivity>
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>NPP SIGNED:10/3/2008                                                                                                                                                                                    </i_Remarks>
      <i_Text />
      <i_UserID>PAUSRM03</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695480000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer  Cov Level:                       Ind                        Insurance Type:                  Medicare Part B            Added  </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695470000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Date:                      01/20/2009                 Eligibility Begin Date:          06/01/1984                 Eligibility End Date:            04/17/2009                  -- HEALTH BENEFIT PLAN   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695460000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity>
        <i_StrategyName />
        <i_DefaultWorklistDays>0</i_DefaultWorklistDays>
        <i_MaxWorklistDays>0</i_MaxWorklistDays>
        <i_NoteType>Type07</i_NoteType>
        <i_Writeable>True</i_Writeable>
        <i_Code>ICOSC</i_Code>
        <i_IsValid>True</i_IsValid>
        <i_Description>CONDITIONS OF SERVICE OBTAINED</i_Description>
        <i_Oid>0</i_Oid>
        <i_Timestamp>0</i_Timestamp>
        <i_ChangedEvent null='True' />
        <i_ChangeTracker null='True' />
        <i_SecondaryProperties null='True' />
      </i_FUSActivity>
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COS SIGNED:Yes                                                                                                                                                                                          </i_Remarks>
      <i_Text />
      <i_UserID>PAUSRM03</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695460000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>COVERAGE  -- Eligibility or Bnft Information: Other or Additional Payer  Cov Level:                       Ind                        Insurance Type:                  Medicare Part A            Added  </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695450000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>MEDICARE BENE  Added Date:             09/19/2008                    Eligibility Begin Date: 11/01/2008                    Eligibility End Date:   04/17/2009                     -- HEALTH BENEFIT PLAN</i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695440000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='15' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>PRIMARY PAYOR VERIFICATION INFORMATION; PAYOR: MEDICAID-TX; INFO REC'D FROM: System electronic verification;                                                                                            </i_Remarks>
      <i_Text />
      <i_UserID>PAUSRM03</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695440000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Messages:               57 MQMB (MAO, SSI RELATED)                            Q                                                     R REGULAR                     Description:            140 MCAID QUAL</i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695430000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Status:                 ACTIVE COV                    Cov Level:              Ind                           Svc Type:               Health Bnft Plan Cov          Insurance Type:         Medicaid      </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695420000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='15' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>SECONDARY PAYOR VERIFICATION INFORMATION; PAYOR: MEDICAID-TX; INFO REC'D FROM: System electronic verification;                                                                                          </i_Remarks>
      <i_Text />
      <i_UserID>PAUSRM03</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695420000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>FEMALE                 -- ACTIVE COVERAGE  -- Cov Lvl Svc Type             Plan Cov Desc  ------- -------------------- -------------  Ind     Health Bnft Plan Cov NURSE HOME      -- COVERAGE  --      </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695410000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>452149989A            Address:          MANOR AT SEAGOVILLE                     SEAGOVILLE, TX 75159  County/Parish:    DALLAS                Date of Birth:    06/11/1919            Sex:              </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695400000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Beginning Date of Service: 04/17/2009  Ending Date of Service:    04/17/2009   -- SUBSCRIBER  -- Name:             NEWTON, HELEN I       Member ID Number: 520418826             HIC Number:            </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695390000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='15' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Passport Health Communications Inc.  Texas Medicaid Eligibility   ---- Recipient is Eligible ----  -- SEARCH CRITERIA  -- NPI:                       1265468946  Recipient ID:              520418826   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695390000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>FEMALE                 -- ACTIVE COVERAGE  -- Cov Lvl Svc Type             Plan Cov Desc  ------- -------------------- -------------  Ind     Health Bnft Plan Cov NURSE HOME      -- COVERAGE  --      </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695380000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>452149989A            Address:          MANOR AT SEAGOVILLE                     SEAGOVILLE, TX 75159  County/Parish:    DALLAS                Date of Birth:    06/11/1919            Sex:              </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695370000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='3' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Beginning Date of Service: 04/17/2009  Ending Date of Service:    04/17/2009   -- SUBSCRIBER  -- Name:             NEWTON, HELEN I       Member ID Number: 520418826             HIC Number:            </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <PatientAccess.Domain.ExtendedFUSNote assembly='PatientAccess.Common'>
      <i_Date1>0</i_Date1>
      <i_Date2>0</i_Date2>
      <i_Dollar1>0.00</i_Dollar1>
      <i_Dollar2>0.00</i_Dollar2>
      <i_Month>   </i_Month>
      <i_Account null='True' />
      <i_AccountNumber />
      <i_CreatedOn>633755695360000000</i_CreatedOn>
      <i_Context null='True' />
      <i_Context2 null='True' />
      <i_FUSActivity ref='15' />
      <i_Persisted>True</i_Persisted>
      <i_ManuallyEntered>False</i_ManuallyEntered>
      <i_Remarks>Passport Health Communications Inc.  Texas Medicaid Eligibility   ---- Recipient is Eligible ----  -- SEARCH CRITERIA  -- NPI:                       1265468946  Recipient ID:              520418826   </i_Remarks>
      <i_Text />
      <i_UserID>EDVSYS</i_UserID>
      <i_WorklistDate>633755412000000000</i_WorklistDate>
      <i_Oid>0</i_Oid>
      <i_Timestamp>0</i_Timestamp>
      <i_ChangedEvent null='True' />
      <i_ChangeTracker null='True' />
      <i_SecondaryProperties null='True' />
    </PatientAccess.Domain.ExtendedFUSNote>
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
    <null />
  </_items>
  <_size>66</_size>
  <_version>66</_version>
  <_syncRoot null='True' />
</System.Collections.ArrayList>
";

            #endregion

            var fusNotesFromData = new XStream().FromXml( fusNotesXml ) as ArrayList;
            return fusNotesFromData.Cast<ExtendedFUSNote>().ToList();
        }
        private Account CreateAccount()
        {
            Account account = new Account();
            account.AccountNumber = 30020;
            account.FinancialClass = new FinancialClass(279, ReferenceValue.NEW_VERSION, "COMMERCIAL INS", "20");
            account.HospitalService = new HospitalService(61, ReferenceValue.NEW_VERSION, "OTHER ICU", "48");
            account.DerivedVisitType = "PRE-REGISTER";
            account.AdmitDate = DateTime.Now;
            account.DischargeDate = DateTime.Now;
            account.KindOfVisit = new VisitType(0, ReferenceValue.NEW_VERSION, VisitType.PREREG_PATIENT_DESC, VisitType.PREREG_PATIENT);
            account.Facility = i_Facility;
            account.IsLocked = false;
            account.Activity = new PreRegistrationActivity();

            Patient patient = new Patient(
                PATIENT_OID,
                Patient.NEW_VERSION,
                this.PATIENT_NAME,
                PATIENT_MRN,
                this.PATIENT_DOB,
                this.PATIENT_SSN,
                this.PATIENT_SEX,
                i_Facility
                );
            account.Patient = patient;

            PaymentAmount aCashPaymentAmount = new PaymentAmount(300.00M, new CashPayment());
            PaymentAmount aCheckPaymentAmount = new PaymentAmount(199.76M, new CheckPayment(CHECK_NUMBER));
            PaymentAmount aCreditCardPaymentAmount =
                new PaymentAmount(250.00M, new CreditCardPayment(CreditCardProvider.Visa()));
            PaymentAmount aMoneyOrderPaymentAmount = new PaymentAmount(250.24M, new MoneyOrderPayment());

            Payment aNewPayment = new Payment();
            aNewPayment.ReceiptNumber = RECEIPT_NUMBER;
            aNewPayment.IsCurrentAccountPayment = true;
            aNewPayment.AddPayment( aCashPaymentAmount,Payment.PaymentType.Cash );
            aNewPayment.AddPayment( aCheckPaymentAmount,Payment.PaymentType.Check );
            aNewPayment.AddPayment( aCreditCardPaymentAmount,Payment.PaymentType.CreditCard1 );
            aNewPayment.AddPayment( aMoneyOrderPaymentAmount,Payment.PaymentType.MoneyOrder );
            account.Payment = aNewPayment;

            account.TotalPaid = 4000.00M;
            account.NumberOfMonthlyPayments = 12;
            account.BalanceDue = 2500.00M;
            account.TotalCurrentAmtDue = 3500.00M;
            account.PreviousTotalCurrentAmtDue = 3500.00M;
            account.OriginalNumberOfMonthlyPayments = 24;
            decimal originalAmount = account.TotalCurrentAmtDue / account.OriginalNumberOfMonthlyPayments;
            string origPayment = String.Format("{0, 10:f2}", originalAmount);
            account.OriginalMonthlyPayment = Decimal.Parse(origPayment);

            return account;
        }

        private Account CreateAccountWithPreviousAccounts()
        {
            Account account = this.CreateAccount();

            // Previous Account 1
            Account account1 = new Account();
            account1.AccountNumber = 12345;
            account1.Facility = i_Facility;
            account1.Activity = new RegistrationActivity();
            account1.Patient = account.Patient;
            PaymentAmount aCashPaymentAmount = new PaymentAmount(100.00M, new CashPayment());
            PaymentAmount aCheckPaymentAmount = new PaymentAmount(100.00M, new CheckPayment(CHECK_NUMBER));
            PaymentAmount aCreditCardPaymentAmount =
                new PaymentAmount(100.00M, new CreditCardPayment(CreditCardProvider.Visa()));
            PaymentAmount aMoneyOrderPaymentAmount = new PaymentAmount(100.00M, new MoneyOrderPayment());
            Payment aNewPayment = new Payment();
            aNewPayment.ReceiptNumber = "23456";
            aNewPayment.IsCurrentAccountPayment = false;
            aNewPayment.AddPayment( aCashPaymentAmount, Payment.PaymentType.Cash );
            aNewPayment.AddPayment( aCheckPaymentAmount, Payment.PaymentType.Check );
            aNewPayment.AddPayment( aCreditCardPaymentAmount, Payment.PaymentType.CreditCard1 );
            aNewPayment.AddPayment( aMoneyOrderPaymentAmount, Payment.PaymentType.MoneyOrder );
            account1.Payment = aNewPayment;
            account1.TotalPaid = 900.00M;
            account1.BalanceDue = 600.00M;
            account1.TotalCurrentAmtDue = 1000.00M;

            // Previous Account 2
            Account account2 = new Account();
            account2.AccountNumber = 23456;
            account2.Facility = i_Facility;
            account2.Activity = new RegistrationActivity();
            account2.Patient = account.Patient;
            PaymentAmount aCashAmount = new PaymentAmount(50.00M, new CashPayment());
            PaymentAmount aCheckAmount = new PaymentAmount(100.00M, new CheckPayment("112233"));
            PaymentAmount aCreditCardAmount =
                new PaymentAmount(200.00M, new CreditCardPayment(CreditCardProvider.Visa()));
            PaymentAmount aMoneyOrderAmount = new PaymentAmount(150.00M, new MoneyOrderPayment());
            Payment aPayment = new Payment();
            aPayment.ReceiptNumber = "222233";
            aPayment.IsCurrentAccountPayment = false;
            aPayment.AddPayment( aCashAmount,Payment.PaymentType.Cash );
            aPayment.AddPayment(aCheckAmount, Payment.PaymentType.Check);
            aPayment.AddPayment(aCreditCardAmount, Payment.PaymentType.CreditCard1 );
            aPayment.AddPayment(aMoneyOrderAmount, Payment.PaymentType.MoneyOrder );
            account2.Payment = aPayment;
            account2.TotalPaid = 800.00M;
            account2.BalanceDue = 1100.00M;
            account2.TotalCurrentAmtDue = 1600.00M;

            account.Patient.AddAccountWithNoPaymentPlanWithPayments(account1);
            account.Patient.AddAccountWithNoPaymentPlanWithPayments(account2);

            return account;
        }
        #endregion

        #region Data Elements

        private static IFUSNoteBroker fusNoteBroker = null;
       //private static  IDbConnection dbConnection;
        private static  Facility i_Facility;
  
        private SocialSecurityNumber    PATIENT_SSN = new SocialSecurityNumber("123121234");
        private Name                    PATIENT_NAME = new Name(PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI);
        private DateTime                PATIENT_DOB = new DateTime(1955, 3, 5);
        private Gender                  PATIENT_SEX = new Gender(0, DateTime.Now, "Female", "F");
        private readonly FusNoteFactory          fusNoteFactory = new FusNoteFactory();

        #endregion

        #region Data Elements

        private const string codeABNGB      = "ABNGB";
        private const string codeABNNC      = "ABNNC";
        private const string codeABNNP      = "ABNNP";

        private const string descABNGB      = "ABN NOTICE GIVEN TO BENEFICIARY";
        private const string descABNNC      = "ABN NON-COVERED DUE TO LOC OR EXCLU";
        private const string descABNNP      = "ABN NO-PAY CLAIM BILL";

        private const string
            RECEIPT_NUMBER      = "12345",
            CHECK_NUMBER        = "786",

            PATIENT_F_NAME      = "CLAIRE",
            PATIENT_L_NAME      = "FRIED",
            

            FACILILTY_NAME      = "DELRAY TEST HOSPITAL";

        private static readonly string PATIENT_MI = string.Empty;
        
        private new const string
            FACILITY_CODE       = "DEL";

        private const long
            PATIENT_OID         = 45L,
            PATIENT_MRN         = 24004,
            FACILITY_MODTYPE    = 11,
            DEL_OID             = 6;
        
        private const string 
            INVALID_CODE_ABNNP              = "AAAAA",
            INVALID_ACTIVITY_DESCRIPTION    = "* INACTIVE ACTIVITY CODE",
            PBAR_EMPLOYEEID                 = "PACCESS";

        #endregion
    }
}