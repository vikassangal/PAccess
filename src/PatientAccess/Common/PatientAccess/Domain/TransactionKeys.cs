namespace PatientAccess.Domain
{
    public class TransactionKeys 
    {
        #region Construction and Finalization
        
        public TransactionKeys()
        {
			
        }
        
        public TransactionKeys( 
            int patientRecordNumber, 
            int logNumber, 
            int insuranceRecordNumber, 
            int otherRecordNumber,
            int daysSinceAdmission )
        {
            this.i_PatientRecordNumber = patientRecordNumber;
            this.i_LogNumber = logNumber;
            this.i_InsuranceRecordNumber = insuranceRecordNumber;
            this.i_DaysSinceAdmission = daysSinceAdmission;
            this.i_OtherRecordNumber = otherRecordNumber;
        }
        #endregion
   
        #region Properties
        public int PatientRecordNumber
        {
            get
            {
                return i_PatientRecordNumber;
            }
            set
            {
                i_PatientRecordNumber = value;
            }
        }
        public int LogNumber
        {
            get
            {
                return i_LogNumber;
            }
            set
            {
                i_LogNumber = value;
            }
        }
        public int InsuranceRecordNumber
        {
            get
            {
                return i_InsuranceRecordNumber;
            }
            set
            {
                i_InsuranceRecordNumber = value;
            }
        }
        public int OtherRecordNumber
        {
            get
            {
                return i_OtherRecordNumber;
            }
            set
            {
                i_OtherRecordNumber = value;
            }
        }
        public int DaysSinceAdmission
        {
            get
            {
                return i_DaysSinceAdmission;
            }
            set
            {
                i_DaysSinceAdmission = value;
            }
        }


        #endregion

        #region Data Elements

        private int i_PatientRecordNumber;
        private int i_LogNumber;
        private int i_InsuranceRecordNumber;
        private int i_DaysSinceAdmission;
        private int i_OtherRecordNumber;
    #endregion
    }
}
