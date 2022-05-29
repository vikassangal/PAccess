using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class LeftOrStayedPlaceTests
    {
        #region Constants
        private const int PARAM_OID = 0;
        #endregion

        #region SetUp and TearDown LeftOrStayedPlaceTests
        [TestFixtureSetUp()]
        public static void SetUpLeftOrStayedPlaceTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownLeftOrStayedPlaceTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestLeftOrStayedForBlank()
        {

             ls = new LeftOrStayed( PARAM_OID, ReferenceValue.NEW_VERSION,
                                                            LeftOrStayed.BLANK, LeftOrStayed.BLANK );

            Assert.AreEqual( String.Empty, ls.Description.Trim() );
            Assert.AreEqual( String.Empty, ls.Code.Trim() );
            Assert.AreEqual( 0, ls.Oid );
        }


        [Test()]
        public void TestLeftOrStayedForLeft()
        {
            ls = new LeftOrStayed( PARAM_OID, ReferenceValue.NEW_VERSION, LeftOrStayed.LEFT_DESCRIPTION,
                                         LeftOrStayed.LEFT );
           Assert.AreEqual( "Left", ls.Description );
           Assert.AreEqual( "L", ls.Code );
           Assert.AreEqual( 0, ls.Oid );
        }
        [Test()]
        public void TestLeftOrStayedForStayed()
        {

             ls = new LeftOrStayed( PARAM_OID, ReferenceValue.NEW_VERSION,
                                                           LeftOrStayed.STAYED_DESCRIPTION, LeftOrStayed.STAYED);
             Assert.AreEqual("Stayed", ls.Description);
            Assert.AreEqual( "S", ls.Code );
            Assert.AreEqual( 0, ls.Oid );
        }

 
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private LeftOrStayed ls = new LeftOrStayed();
        #endregion
    }
}