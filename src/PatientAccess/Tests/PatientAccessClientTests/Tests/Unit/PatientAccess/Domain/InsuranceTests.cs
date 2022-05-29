using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class InsuranceTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown InsuranceTests
        [TestFixtureSetUp()]
        public static void SetUpInsuranceTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownInsuranceTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
    
        public void TestCoverageFor()

        {
            CoverageOrder primary = new CoverageOrder( 1, "Primary" );
            Assert.AreEqual(
                primary.Description,
                "Primary"
                );
            CoverageOrder secondary = new CoverageOrder( 2, "Secondary" );
            Assert.AreEqual(
                secondary.Description,
                "Secondary");
       

            CommercialCoverage coverage1 = new CommercialCoverage();
            coverage1.CoverageOrder = primary;
            coverage1.Oid = 1;

            Assert.AreEqual(
                coverage1.CoverageOrder,
                primary);
            Assert.AreEqual(
                coverage1.Oid,
                1);
       

            GovernmentOtherCoverage coverage2 = new GovernmentOtherCoverage();
            coverage2.CoverageOrder = secondary ;
            coverage2.Oid = 2;
            Assert.AreEqual(
                coverage2.CoverageOrder,
                secondary);
            Assert.AreEqual(
                coverage1.Oid,
                1);

            Insurance insurance = new Insurance();
            insurance.AddCoverage(coverage1);
            Assert.AreEqual(
                insurance.Coverages.Count,
                1);
            insurance.AddCoverage(coverage2);
            Assert.AreEqual(
                insurance.Coverages.Count,
                2);

            insurance.RemoveCoverage(coverage1);
            Assert.AreEqual(
                insurance.Coverages.Count,
                1);
            Coverage  coverage = insurance.CoverageFor( secondary );
            Assert.AreEqual(
                coverage.CoverageOrder,
                coverage2.CoverageOrder
                );

            coverage = insurance.CoverageFor(CoverageOrder.SECONDARY_OID);
            Assert.AreEqual(
                coverage.CoverageOrder,
                coverage2.CoverageOrder
                );
        }

        [Test()]
        public void TestSetAsPrimaryAndSecondary()
        {
            CoverageOrder primary = new CoverageOrder( 1, "Primary" );
            CoverageOrder secondary = new CoverageOrder( 2, "Secondary" );
            CommercialCoverage commercialCoverage = new CommercialCoverage();
            commercialCoverage.CoverageOrder = secondary;
            Assert.AreEqual(
                commercialCoverage.CoverageOrder,
                secondary
                );
            commercialCoverage.Oid = 3;
            Insurance insurance2 = new Insurance();
            insurance2.AddCoverage(commercialCoverage);
                   
            GovernmentOtherCoverage govtCoverage = new GovernmentOtherCoverage();
            govtCoverage.CoverageOrder = primary;
            Assert.AreEqual(
                govtCoverage.CoverageOrder,
                primary
                );
           
            govtCoverage.Oid = 4;
            insurance2.AddCoverage(govtCoverage);

            insurance2.SetAsPrimary(commercialCoverage);

            Assert.AreEqual(
                commercialCoverage.CoverageOrder,
                primary
                );
           
            Assert.AreEqual(
                govtCoverage.CoverageOrder,
                secondary
                );
           
                              
            insurance2.SetAsSecondary(commercialCoverage);
            Assert.AreEqual(
                commercialCoverage.CoverageOrder,
                secondary
                );
           
            Assert.AreEqual(
                govtCoverage.CoverageOrder,
                primary
                );
        }

        [Test()]
        public void TestSetAsPrimaryCoverage()
        {
            Insurance ins = new Insurance();

            Coverage c1 = new CommercialCoverage();
            c1.CoverageOrder = new CoverageOrder( 1L, "Primary" );

            Coverage c2 = new CommercialCoverage();
            c2.CoverageOrder = new CoverageOrder( 2L, "Secondary" );

            ins.AddCoverage( c1 );
            ins.AddCoverage( c2 );

            Assert.AreEqual( 
                2, 
                ins.Coverages.Count,
                "There should only be two coverages"
                );

            Assert.AreEqual(
                new CoverageOrder( 1L, "Primary" ),
                c1.CoverageOrder,
                "c1 should be the Primary coverage"
                );

            Assert.AreEqual(
                new CoverageOrder( 2L, "Secondary" ),
                c2.CoverageOrder,
                "c2 should be the Secondary coverage"
                );

            ins.SetAsPrimary( c2 );

            Assert.AreEqual( 
                2, 
                ins.Coverages.Count,
                "There should still only be two coverages"
                );

            Assert.AreEqual(
                new CoverageOrder( 2L, "Secondary" ),
                c1.CoverageOrder,
                "c1 should be the Secondary coverage now"
                );
            
            Assert.AreEqual(
                new CoverageOrder( 1L, "Primary" ),
                c2.CoverageOrder,
                "c2 should be the Primary coverage now"
                );

            Assert.AreEqual(
                "Primary",
                c2.CoverageOrder.Description
                );
        }

        [Test()]
        [ExpectedException( typeof( ArgumentException ) )]
        public void TestInvalidAttemptToSetCoverageAsPrimary()
        {
            Insurance ins = new Insurance();
            ins.SetAsPrimary( new CommercialCoverage() );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}