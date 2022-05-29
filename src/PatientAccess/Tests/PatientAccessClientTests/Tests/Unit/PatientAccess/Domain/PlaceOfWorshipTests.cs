using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class PlaceOfWorshipTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PlaceOfWorshipTests
        [TestFixtureSetUp()]
        public static void SetUpPlaceOfWorshipTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPlaceOfWorshipTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testPlaceOfWorship()
        {
           
            PlaceOfWorship pow1 = new PlaceOfWorship(3,ReferenceValue.NEW_VERSION,"Saint EA Seaton Catholic","Saint");
           
            Assert.AreEqual(
                typeof(PlaceOfWorship),
                pow1.GetType()
                );
                 
            Assert.AreEqual(
                "Saint EA Seaton Catholic",
                pow1.Description
                );
            Assert.AreEqual(
                "Saint",
                pow1.Code
                );
            Assert.AreEqual(
                3,
                pow1.Oid
                );
            PlaceOfWorship pow2 = new PlaceOfWorship(3,ReferenceValue.NEW_VERSION,"Saint Marks Catholic","Catholic");
            PlaceOfWorship pow3 = new PlaceOfWorship(3,ReferenceValue.NEW_VERSION,"Saint Marks Lutheran","Lutheran");
            ArrayList pows = new ArrayList();
            pows.Add(pow1);
            pows.Add(pow2);
            pows.Add(pow3);
            Assert.AreEqual(3,
                            pows.Count                         
                );
            Assert.IsTrue(pows.Contains(pow2) );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}