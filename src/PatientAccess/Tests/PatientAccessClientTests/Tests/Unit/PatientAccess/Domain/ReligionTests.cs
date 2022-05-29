using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ReligionTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ReligionTests
        [TestFixtureSetUp()]
        public static void SetUpReligionTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownReligionTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testReligion()
        {
           
            Religion religion1 = new Religion(3,ReferenceValue.NEW_VERSION,"ADVENTIST","ADVENTIST");
           
            Assert.AreEqual(
                typeof(Religion),
                religion1.GetType()
                );
                 
            Assert.AreEqual(
                "ADVENTIST",
                religion1.Description
                );
            Assert.AreEqual(
                "ADVENTIST",
                religion1.Code
                );
            Assert.AreEqual(
                3,
                religion1.Oid
                );
            Religion religion2 = new Religion(3,ReferenceValue.NEW_VERSION,"ASSEMBLY OF GOD","ASSEMBLY OF GOD");
            Religion religion3 = new Religion(3,ReferenceValue.NEW_VERSION,"Jewish","Jewish");
            ArrayList religions = new ArrayList();
            religions.Add(religion1);
            religions.Add(religion2);
            religions.Add(religion3);
            Assert.AreEqual(3,
                            religions.Count                         
                );
            Assert.IsTrue(religions.Contains(religion2) );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}