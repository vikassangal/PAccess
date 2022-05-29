using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class LanguageTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown LanguageTests
        [TestFixtureSetUp()]
        public static void SetUpLanguageTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownLanguageTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestLanguages()
        {
            ArrayList  allLanguages = new ArrayList() ;
            Language english = new Language(0L,
                                            DateTime.Now,
                                            "English",
                                            "01");
            Language french = new Language(1L,
                                           DateTime.Now,
                                           "French",
                                           "02");
            Language spanish = new Language(1L,
                                            DateTime.Now,
                                            "Spanish",
                                            "02");

            allLanguages.Add(english);
            allLanguages.Add(french);
            allLanguages.Add(spanish);
            Assert.AreEqual(3,
                            allLanguages.Count                         
                );
            Assert.IsTrue(allLanguages.Contains(english) );


        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}