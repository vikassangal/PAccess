using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class AdmitSourceTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AdmitSourceTests
        [TestFixtureSetUp()]
        public static void SetUpAdmitSourceTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownAdmitSourceTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testAdmitSources()
        {
            ArrayList  allAdmitSources = new ArrayList() ;
            AdmitSource admit = new AdmitSource(0L,
                                                DateTime.Now,
                                                "admit",
                                                "01");
            AdmitSource source = new AdmitSource(1L,
                                                 DateTime.Now,
                                                 "source",
                                                 "02");
            AdmitSource admitSource = new AdmitSource(1L,
                                                      DateTime.Now,
                                                      "admitSource",
                                                      "02");

            allAdmitSources.Add(admit);
            allAdmitSources.Add(source);
            allAdmitSources.Add(admitSource);
            Assert.AreEqual(3,
                            allAdmitSources.Count                         
                );
            Assert.IsTrue(allAdmitSources.Contains(admit) );

            Assert.AreEqual( admit.ToString(), "01 admit" );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}