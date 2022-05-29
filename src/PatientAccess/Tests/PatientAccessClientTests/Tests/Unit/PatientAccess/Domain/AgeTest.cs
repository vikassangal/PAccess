using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class AgeTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AgeTests
        [TestFixtureSetUp()]
        public static void SetUpBrokerTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testAge()
        {

            string years = "May 1, 2001";
            DateTime MyDateTime = DateTime.Parse(years);
            DateTime dob = MyDateTime ;
            Age age = new Age() ;
        
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}