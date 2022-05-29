using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    public class TimeAndLocationBoundConditionTests
    {
        [Test, Sequential]
        [Category( "Fast" )]
        public void TestGetOccurredHour(
            [Values( "2400", "9900", "1300", "130", "13", "3" )] string accidentHour,
            [Values( "Unknown", "Unknown", "13", "01", "13", "03" )] string expectedResult )
        {
            var accident = new Accident { OccurredOn = DateTime.Now, OccurredAtHour = accidentHour };
            var result = accident.GetOccurredHour();
            Assert.IsTrue( result == expectedResult, string.Format( "Occurred hour {0} should return {1}.", accidentHour, expectedResult ) );
        }
    }
}
