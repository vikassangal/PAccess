using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class DiagnosisTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown DiagnosisTests
        [TestFixtureSetUp()]
        public static void SetUpDiagnosisTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownDiagnosisTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestDiagnosis()
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

            Assert.AreEqual(
                typeof(Accident),
                anAccount.Diagnosis.Condition.GetType()
                );


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
            Crime crime = new Crime() ;
            diagnosis.Condition = crime ;
            anAccount.Diagnosis = diagnosis ;
            Crime cr = (Crime)anAccount.Diagnosis.Condition ;
            
            Assert.AreEqual(
                typeof(Crime),
                anAccount.Diagnosis.Condition.GetType()
                );

            UnknownCondition unknown = new UnknownCondition() ;
            diagnosis.Condition = unknown ;
            anAccount.Diagnosis = diagnosis ;
            UnknownCondition unk = (UnknownCondition)anAccount.Diagnosis.Condition ;
            
            Assert.AreEqual(
                typeof(UnknownCondition),
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