using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CrimeTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CrimeTests
        [TestFixtureSetUp()]
        public static void SetUpCrimeTests()
        {
            crime = new Crime();
        }

        [TestFixtureTearDown()]
        public static void TearDownCrimeTests()
        {
            crime = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCrime()
        {
            Assert.AreEqual(crime.OccurrenceCodeStr, OccurrenceCode.OCCURRENCECODE_CRIME); 
        
            Account anAccount = new Account() ;
            Diagnosis diagnosis = new Diagnosis();
            diagnosis.Condition = crime ;
            anAccount.Diagnosis = diagnosis ;
            
            Assert.AreEqual(
                typeof(Crime),
                anAccount.Diagnosis.Condition.GetType()
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static Crime crime = null;
        #endregion
    }
}