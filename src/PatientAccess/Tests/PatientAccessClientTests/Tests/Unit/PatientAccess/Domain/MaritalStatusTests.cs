using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class MaritalStatusTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown MaritalStatusTests
        [TestFixtureSetUp()]
        public static void SetUpMaritalStatusTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownMaritalStatusTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestMaritalStatuses()
        {
            ArrayList  allMaritalStatuses = new ArrayList() ;
            MaritalStatus married = new MaritalStatus(0L,
                                                      DateTime.Now,
                                                      "Married",
                                                      "01");
            MaritalStatus unmarried = new MaritalStatus(1L,
                                                        DateTime.Now,
                                                        "Unmarried",
                                                        "02");
            MaritalStatus divorced = new MaritalStatus(2L,
                                                       DateTime.Now,
                                                       "Divorced",
                                                       "02");

            allMaritalStatuses.Add(married);
            allMaritalStatuses.Add(unmarried);
            allMaritalStatuses.Add(divorced);
            Assert.AreEqual(3,
                            allMaritalStatuses.Count                         
                );
            Assert.IsTrue(allMaritalStatuses.Contains(divorced) );


        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}