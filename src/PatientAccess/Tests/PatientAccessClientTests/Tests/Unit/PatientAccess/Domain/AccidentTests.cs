using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class AccidentTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AccidentTests
        [TestFixtureSetUp()]
        public static void SetUpAccidentTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownAccidentTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestAccident()
        {
            Account anAccount = new Account() ;
            Diagnosis diagnosis = new Diagnosis();
            Accident accident = new Accident() ;
            accident.OccurredOn = DateTime.Parse("Jan 21, 2005");   
            accident.OccurredAtHour = "11:00"; 
            accident.Country = new Country(0L,DateTime.Now,"USA"); 
            accident.State = new State(0L,DateTime.Now,"Texas"); 
            accident.Kind  = new TypeOfAccident(0L,DateTime.Now,"Auto") ;
            diagnosis.Condition = accident ;
            anAccount.Diagnosis = diagnosis ;
            Accident acdt = (Accident)anAccount.Diagnosis.Condition ;
            Assert.AreEqual(
                acdt.OccurredOn.Day ,
                21);
            Assert.AreEqual(
                acdt.OccurredAtHour ,
                "11:00");
            Assert.AreEqual(
                acdt.Country.Description,
                "USA");
            Assert.AreEqual(
                acdt.State.Description,
                "Texas");
            Assert.AreEqual(
                acdt.Kind.Description,
                "Auto");

            Assert.AreEqual( acdt.OccurrenceCodeStr, string.Empty );


            Assert.AreEqual(
                typeof(Accident),
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