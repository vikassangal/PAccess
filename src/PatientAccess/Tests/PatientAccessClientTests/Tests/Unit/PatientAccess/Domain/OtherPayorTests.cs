using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class OtherPayorTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown OtherPayorTests
        [TestFixtureSetUp()]
        public static void SetUpOtherPayorTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownOtherPayorTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestConstructors()
        {
            OtherPayor op = new OtherPayor();
            Assert.AreEqual(
                String.Empty,
                op.NewPayorName
                );

            op.NewPayorName = "Foo";
            Assert.AreEqual(
                "Foo",
                op.NewPayorName
                );

            OtherPayor op2 = new OtherPayor( "Some Foo" );
            Assert.AreEqual(
                "Some Foo",
                op2.NewPayorName
                );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}