using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class WorkersCompFusFormatterTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown WorkersCompFusFormatterTests
        [TestFixtureSetUp()]
        public static void SetUpWorkersCompFusFormatterTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownWorkersCompFusFormatterTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestWorkersCompFusFormatter()
        {
            Account anAccount = new Account(); 
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag  naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();
            anAccount.AccountNumber = 385713 ;
            WorkersCompensationCoverage wCov = new WorkersCompensationCoverage();
            wCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID,"PRIMARY_OID");
            wCov.Remarks = "WorkerCompCove Remarks";
            wCov.PPOPricingOrBroker = "PPOPricingOrBroker" ;
            wCov.Oid  =212;
            wCov.InsurancePhone = "37846384683" ;
            wCov.InformationReceivedSource = new InformationReceivedSource(2,DateTime.Now,"InformationmationreceivedSource");
            wCov.EmployerhasPaidPremiumsToDate = flag;
            flag.SetNo();
            wCov.EligibilityVerified = naFlag;
            wCov.ClaimsAddressVerified = flag ;
            wCov.ClaimNumberForIncident = "jsdh78sydsd";
            anAccount.Insurance.AddCoverage(wCov);
            FusNoteFactory fac = new FusNoteFactory();
            fac.AddRBVCANoteTo( anAccount, wCov, new WorkersCompensationCoverage() );
            IList fusNotesCreated = null ;
            foreach( FusNote note  in anAccount.FusNotes )
            {
                fusNotesCreated = note.CreateFUSNoteMessages(anAccount);
            }
            foreach( string msg in fusNotesCreated )
            {
                Assert.IsNotNull(msg);
            }
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}