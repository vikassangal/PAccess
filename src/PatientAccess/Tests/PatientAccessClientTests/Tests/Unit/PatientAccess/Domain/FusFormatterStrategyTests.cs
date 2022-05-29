using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class FUSFormatterStrategyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown FUSFormatterStrategyTests
        [TestFixtureSetUp()]
        public static void SetUpFUSFormatterStrategyTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownFUSFormatterStrategyTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCommercialFusFormatter()
        {
            Account anAccount = new Account(); 
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag  naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();
            anAccount.AccountNumber = 385713 ;
            
            CommercialCoverage cCov = new CommercialCoverage();
            cCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID,"PRIMARY_OID");
            cCov.Remarks = "Commercial FUS Remarks";
            cCov.PPOPricingOrBroker = "PPOPricingOrBroker" ;
            cCov.Oid  =212;
            cCov.InformationReceivedSource = new InformationReceivedSource(2,DateTime.Now,"InformationReceivedSource");
            flag.SetNo();
            cCov.EligibilityVerified = naFlag;
            cCov.ClaimsAddressVerified = flag ;
            cCov.EffectiveDateForInsured = new DateTime( 2000, 1, 1 );
            cCov.InsurancePlan = new CommercialInsurancePlan();
            cCov.InsurancePlan.Payor = new Payor();
            cCov.InsurancePlan.Payor.Code = "00";
            cCov.InsurancePlan.PlanSuffix = "00";            

            anAccount.Insurance.AddCoverage(cCov);

            CommercialCoverage origCov = new CommercialCoverage();
            origCov.CoverageOrder = new CoverageOrder( CoverageOrder.PRIMARY_OID, "PRIMARY_OID" );
            origCov.Remarks = "Original WorkerCompCove Remarks";
            origCov.PPOPricingOrBroker = "PPOPricingOrBroker";
            origCov.Oid = 212;
            origCov.InformationReceivedSource = new InformationReceivedSource( 2, DateTime.Now, "Orig. InformationReceivedSource" );
            flag.SetNo();
            origCov.EligibilityVerified = naFlag;
            origCov.ClaimsAddressVerified = flag;
            origCov.EffectiveDateForInsured = new DateTime( 2000, 1, 2 );
            origCov.InsurancePlan = new GovernmentMedicaidInsurancePlan();
            origCov.InsurancePlan.Payor = new Payor();
            origCov.InsurancePlan.Payor.Code = "00";
            origCov.InsurancePlan.PlanSuffix = "00";            

            anAccount.Insurance.AddOrigCoverage( origCov );

            FusNoteFactory fac = new FusNoteFactory();
            // Test values comparison for both coverages to write FUS
            fac.AddRBVCANoteTo( anAccount, cCov, origCov );
            IList fusNotesCreated = null ;
            foreach( FusNote note  in anAccount.FusNotes )
            {
                fusNotesCreated = note.CreateFUSNoteMessages(anAccount);
            }
            if( fusNotesCreated != null )
            {
                foreach( string msg in fusNotesCreated )
                {
                    Assert.IsNotNull( msg );
                }
            }
        }

        [Test()]
        public void TestCommercialFusFormatterWithNullOriginalCoverage()
        {
            Account anAccount = new Account();
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();
            anAccount.AccountNumber = 385713;

            CommercialCoverage cCov = new CommercialCoverage();
            cCov.CoverageOrder = new CoverageOrder( CoverageOrder.PRIMARY_OID, "PRIMARY_OID" );
            cCov.Remarks = "Commercial With Null Coverage Remarks";
            cCov.PPOPricingOrBroker = "PPOPricingOrBroker";
            cCov.Oid = 212;
            cCov.InformationReceivedSource = new InformationReceivedSource( 2, DateTime.Now, "InformationReceivedSource" );
            flag.SetNo();
            cCov.EligibilityVerified = naFlag;
            cCov.ClaimsAddressVerified = flag;
            cCov.EffectiveDateForInsured = new DateTime( 2000, 1, 1 );
            
            anAccount.Insurance.AddCoverage( cCov );

            FusNoteFactory fac = new FusNoteFactory();
            // Test FUS notes creation with null original coverage
            fac.AddRBVCANoteTo( anAccount, cCov, new CommercialCoverage() );
            IList fusNotesCreated = null;
            foreach( FusNote note in anAccount.FusNotes )
            {
                fusNotesCreated = note.CreateFUSNoteMessages( anAccount );
            }
            if( fusNotesCreated != null )
            {
                foreach( string msg in fusNotesCreated )
                {
                    Assert.IsNotNull( msg );
                }
            }
        }

        [Test()]
        public void TestMedicareFusFormatter()
        {
            Account anAccount = new Account(); 
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag  naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();
            anAccount.AccountNumber = 385713 ;
            GovernmentMedicareCoverage mcCov = new GovernmentMedicareCoverage();
            mcCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID,"PRIMARY_OID");
            mcCov.RemainingPartADeductible = 500;
            mcCov.RemainingPartBDeductible = 300;
            mcCov.Remarks = "Medicare FUS Remarks";
           
            mcCov.Oid  =212;
            
            mcCov.InformationReceivedSource = new InformationReceivedSource(2,DateTime.Now,"InformationReceivedSource");
            
            flag.SetNo();
            mcCov.EligibilityVerified = naFlag;
         
            anAccount.Insurance.AddCoverage( mcCov );
            FusNoteFactory fac = new FusNoteFactory();
            fac.AddMCWFINoteTo( anAccount, mcCov, new GovernmentMedicareCoverage() );
            fac.AddRBVCANoteTo( anAccount, mcCov, new GovernmentMedicareCoverage() );
            IList fusNotesCreated = null ;
            foreach( FusNote note  in anAccount.FusNotes )
            {
                fusNotesCreated = note.CreateFUSNoteMessages( anAccount );
                if( fusNotesCreated != null )
                {
                    if( note.FusActivity.Code == "MCWFI" )
                    {
                        Assert.IsTrue((( ArrayList )fusNotesCreated ).Contains( "REMAINING PART-A DED: $500.00; " ));
                        Assert.IsTrue((( ArrayList )fusNotesCreated ).Contains( "REMAINING PART-B DED: $300.00; " ));
                    }
                    foreach( string msg in fusNotesCreated )
                    {
                        Assert.IsNotNull( msg );
                    }
                }
            }
        }

        [Test()]
        public void TestMedicaidFusFormatter()
        {
            Account anAccount = new Account(); 
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag  naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();
            anAccount.AccountNumber = 385713 ;
            GovernmentMedicaidCoverage mdCov = new GovernmentMedicaidCoverage();
            mdCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID,"PRIMARY_OID");
            mdCov.Remarks = "Medicaid FUS Remarks";
            
            mdCov.Oid  =212;
           
            mdCov.InformationReceivedSource = new InformationReceivedSource(2,DateTime.Now,
                                                                            "InformationReceivedSource - These are test remarks for Medicaid type insurance coverage");

            flag.SetNo();
            mdCov.EligibilityVerified = naFlag;
            mdCov.InsurancePlan = new GovernmentMedicaidInsurancePlan();
            mdCov.InsurancePlan.Payor = new Payor();
            mdCov.InsurancePlan.Payor.Code = "00";
            mdCov.InsurancePlan.PlanSuffix = "00";            

            anAccount.Insurance.AddCoverage(mdCov);
            FusNoteFactory fac = new FusNoteFactory();
            fac.AddRBVCANoteTo( anAccount, mdCov, new GovernmentMedicaidCoverage() );
            IList fusNotesCreated = null ;
            foreach( FusNote note  in anAccount.FusNotes )
            {
                fusNotesCreated = note.CreateFUSNoteMessages(anAccount);
            }
            if( fusNotesCreated != null )
            {
                foreach( string msg in fusNotesCreated )
                {
                    Assert.IsNotNull(msg);
                }
            }
        }
        [Test()]
        public void TestGovernmentMiscFusFormatter()
        {
            Account anAccount = new Account(); 
            YesNoFlag flag = new YesNoFlag();
            YesNotApplicableFlag  naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            flag.SetYes();
            anAccount.AccountNumber = 385713 ;
            GovernmentOtherCoverage goCov = new GovernmentOtherCoverage();
            goCov.CoverageOrder = new CoverageOrder(CoverageOrder.PRIMARY_OID,"PRIMARY_OID");
            goCov.Remarks = "Government Misc FUS Remarks";
            
            goCov.Oid  =212;
          
            goCov.InformationReceivedSource = new InformationReceivedSource(2,DateTime.Now,"InformationReceivedSource");
            goCov.InsurancePlan = new GovernmentOtherInsurancePlan();
            goCov.InsurancePlan.Payor = new Payor();
            goCov.InsurancePlan.Payor.Code = "00";
            goCov.InsurancePlan.PlanSuffix = "00";            

            flag.SetNo();
     
            anAccount.Insurance.AddCoverage(goCov);
            FusNoteFactory fac = new FusNoteFactory();
            fac.AddRBVCANoteTo( anAccount, goCov, new GovernmentOtherCoverage() );
            IList fusNotesCreated = null ;
            foreach( FusNote note  in anAccount.FusNotes )
            {
                fusNotesCreated = note.CreateFUSNoteMessages(anAccount);
            }
            if( fusNotesCreated != null )
            {
                foreach( string msg in fusNotesCreated )
                {
                    Assert.IsNotNull(msg);
                }
            }
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}