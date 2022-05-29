using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class PregnancyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PregnancyTests
        [TestFixtureSetUp()]
        public static void SetUpPregnancyTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPregnancyTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestPregnancy()
        {
            Account anAccount = new Account() ;
            Diagnosis diagnosis = new Diagnosis();
            Pregnancy pregnancy = new Pregnancy() ;
            pregnancy.LastPeriod = DateTime.Parse("Jan 21, 2005");   
            diagnosis.Condition = pregnancy ;
            anAccount.Diagnosis = diagnosis ;
            Pregnancy preg = (Pregnancy)anAccount.Diagnosis.Condition ;
            Assert.AreEqual(
                preg.LastPeriod.Day ,
                21);
          
            Assert.AreEqual(
                typeof(Pregnancy),
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