using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CoverageOrderTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CoverageOrderTests
        [TestFixtureSetUp()]
        public static void SetUpCoverageOrderTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCoverageOrderTests()
        {
            co = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCoverageOrder()
        {
            co = new CoverageOrder();
            co = new CoverageOrder(0L, string.Empty);
            co = new CoverageOrder(0L, DateTime.Today, string.Empty);

            co = CoverageOrder.NewPrimaryCoverageOrder();
            Assert.AreEqual(CoverageOrder.PRIMARY_OID, co.Oid);

            co = CoverageOrder.NewSecondaryCoverageOrder();
            Assert.AreEqual(CoverageOrder.SECONDARY_OID, co.Oid);
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static CoverageOrder co = null;
        #endregion
    }
}