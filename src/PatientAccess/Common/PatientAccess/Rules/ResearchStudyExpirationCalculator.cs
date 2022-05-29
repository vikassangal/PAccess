using System;

namespace PatientAccess.Rules
{
    public class ResearchStudyExpirationCalculator
    {
        private readonly DateTime admitDate;
        private readonly DateTime studyTerminationDate;
        private readonly bool studyHasNoTerminationDate;

        public ResearchStudyExpirationCalculator( DateTime admitDate, DateTime studyTerminationDate )
        {
            this.admitDate = RemoveTimePartOf( admitDate );
            this.studyTerminationDate = RemoveTimePartOf( studyTerminationDate ); 
            studyHasNoTerminationDate = studyTerminationDate == DateTime.MinValue;
        }

        public bool IsStudyExpired()
        {
            if ( studyHasNoTerminationDate )
            {
                return false;
            }
            
            return studyTerminationDate < admitDate;
        }

        private static DateTime RemoveTimePartOf( DateTime dateTime )
        {
            return dateTime.Date;
        }
    }
}