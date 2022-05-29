using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CountyTests
    {
        #region Test Methods
        [Test()]
        public void TestConstructor()
        {
            County county = new County(COUNTYCODE, COUNTYDESCRIPTION);

            Assert.AreEqual(
                COUNTYCODE,
                county.Code
                );

            Assert.AreEqual(
                COUNTYDESCRIPTION,
                county.Description
                );
        }
        #endregion

        #region Data Elements

        const string COUNTYCODE = "122",
                     COUNTYDESCRIPTION = "ORANGE";

        #endregion
    }
}