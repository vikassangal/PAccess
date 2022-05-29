using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class SpecialityTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown SpecialityTests
        [TestFixtureSetUp()]
        public static void SetUpSpecialityTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownSpecialityTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testSpeciality()
        {
           
            Speciality ane = new Speciality(3,ReferenceValue.NEW_VERSION,"ANE","ANE");
           
            Assert.AreEqual(
                typeof(Speciality),
                ane.GetType()
                );
                 
            Assert.AreEqual(
                "ANE",
                ane.Description
                );
            Assert.AreEqual(
                "ANE",
                ane.Code
                );
            Assert.AreEqual(
                3,
                ane.Oid
                );
            Speciality ped = new Speciality(1,ReferenceValue.NEW_VERSION,"PED","PED");
            Speciality obg = new Speciality(2,ReferenceValue.NEW_VERSION,"OBJ","OBG");
            ArrayList specialities = new ArrayList();
            specialities.Add(ane);
            specialities.Add(ped);
            specialities.Add(obg);
            Assert.AreEqual(3,
                            specialities.Count                         
                );
            Assert.IsTrue( specialities.Contains(obg) );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}