using System;
using NUnit.Framework;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class ResearchStudyExpirationCalculatorTests
    {
        [Test]
        public void WhenStudyHasNoTerminationDate_StudyShouldNotBeExpired()
        {
            var admitDate = DateTime.Now.Date;
            var noTerminationDate = DateTime.MinValue;
            var calculator = new ResearchStudyExpirationCalculator( admitDate, noTerminationDate );

            Assert.IsFalse( calculator.IsStudyExpired() );
        }

        [Test]
        public void WhenStudyTerminationDateIsBeforeAdmitDate_StudyShouldBeExpired()
        {
            var admitDate = DateTime.Now.Date;
            var studyTerminationDate = admitDate - TimeSpan.FromDays( 3 );
            var calculator = new ResearchStudyExpirationCalculator( admitDate, studyTerminationDate );

            Assert.IsTrue( calculator.IsStudyExpired() );
        }

        [Test]
        public void WhenStudyTerminationDateIsAfterAdmitDate_StudyShouldNotBeExpired()
        {
            var admitDate = DateTime.Now.Date;
            var studyTerminationDate = admitDate + TimeSpan.FromDays( 3 );
            var calculator = new ResearchStudyExpirationCalculator( admitDate, studyTerminationDate );

            Assert.IsFalse( calculator.IsStudyExpired() );
        }

        [Test]
        public void WhenStudyTerminationDateIsTheSameAsTheAdmitDate_StudyShouldNotBeExpired()
        {
            var admitDate = DateTime.Now.Date;
            var studyTerminationDate = admitDate;
            var calculator = new ResearchStudyExpirationCalculator( admitDate, studyTerminationDate );

            Assert.IsFalse( calculator.IsStudyExpired() );
        }

        [Test]
        public void WhenStudyTerminationDateIsTheSameAsTheAdmitDateButAheadByHoursOnly_StudyShouldNotBeExpired()
        {
            var admitDate = DateTime.Now.Date;
            var studyTerminationDate = admitDate.Date + TimeSpan.FromHours( 2 );
            var calculator = new ResearchStudyExpirationCalculator( admitDate, studyTerminationDate );

            Assert.IsFalse( calculator.IsStudyExpired() );
        }

        [Test]
        public void WhenStudyTerminationDateIsTheSameAsTheAdmitDateButBehindByHoursOnly_StudyShouldNotBeExpired()
        {
            var admitDate = DateTime.Now.Date + TimeSpan.FromHours(2);
            var studyTerminationDate = admitDate.Date;
            var calculator = new ResearchStudyExpirationCalculator( admitDate, studyTerminationDate );

            Assert.IsFalse( calculator.IsStudyExpired() );
        }
    }
}
