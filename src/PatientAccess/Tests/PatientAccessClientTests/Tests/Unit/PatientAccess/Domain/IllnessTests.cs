using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class IllnessTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown IllnessTests
        [TestFixtureSetUp()]
        public static void SetUpIllnessTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownIllnessTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestIllness()
        {
            Account anAccount = new Account() ;
            Diagnosis diagnosis = new Diagnosis();
            Illness illness = new Illness() ;
            illness.Onset = DateTime.Parse("Jan 21, 2005");            
            diagnosis.ChiefComplaint = "Soar Throat";
            diagnosis.Condition = illness ;
            anAccount.Diagnosis = diagnosis ;
            Illness patientCond = (Illness)anAccount.Diagnosis.Condition ;
            Assert.AreEqual(
                patientCond.Onset.Day,
                21);
            Assert.AreEqual(
                typeof(Illness),
                anAccount.Diagnosis.Condition.GetType()
                );

        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}