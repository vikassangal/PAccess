using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    public class WorklistActivity
    {
        private long i_AccountNumber;
        private string i_Activity;
        private long i_MedicalRecordNumber;
        private Facility i_Facility;

        public Facility Facility
        {
            get { return i_Facility; }
            set { i_Facility = value; }
        }

        public long MedicalRecordNumber
        {
            get { return i_MedicalRecordNumber; }
            set { i_MedicalRecordNumber = value; }
        }

        public string Activity
        {
            get { return i_Activity; }
            set { i_Activity = value; }
        }


        public long AccountNumber
        {
            get { return i_AccountNumber; }
            set { i_AccountNumber = value; }
        }

        public const string
            DELETED = "D",
            INSERTED = "I",
            UPDATED = "U";
    }
}
