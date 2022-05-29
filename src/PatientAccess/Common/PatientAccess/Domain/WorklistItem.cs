using System;
using PatientAccess.Annotations;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for WorklistItem.
    /// </summary>

    [Serializable]
    public class WorklistItem 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return i_Name;
            }
            private set
            {
                i_Name = value;
            }
        }

        public long AccountNumber
        {
            get
            {
                return i_AccountNumber;
            }
            private set
            {
                i_AccountNumber = value;
            }
        }

        public long MedicalRecordNumber
        {
            get
            {
                return i_MedicalRecordNumber;
            }
            private set
            {
                i_MedicalRecordNumber = value;
            }
        }

        public DateTime AdmitDate
        {
            get
            {
                return i_AdmitDate;
            }
            private set
            {
                i_AdmitDate = value;
            }
        }

        private DateTime DischargeDate
        {
            get
            {
                return i_DischargeDate;
            }
            set
            {
                i_DischargeDate = value;
            }
        }

        public string DischargeStatus
        {
            get
            {
                return i_DischargeStatus;
            }
            private set
            {
                i_DischargeStatus = value;
            }
        }


        public string FinancialClass
        {
            get
            {
                return i_FinancialClass;
            }
            private set
            {
                i_FinancialClass = value;
            }
        }

        public string HospitalService
        {
            get
            {
                return i_HospitalService;
            }
            private set
            {
                i_HospitalService = value;
            }
        }

        public string Clinic
        {
            get
            {
                return i_Clinic;
            }
            private set
            {
                i_Clinic = value;
            }
        }
        
        public string PrimaryPayor
        {
            get
            {
                return i_PrimaryPayor;
            }
            private set
            {
                i_PrimaryPayor = value;
            }
        }

        [UsedImplicitly] 
        public string IsLocked
        {
            get
            {
                return i_IsLocked;
            }

            private set
            {
                i_IsLocked = value;
            }        
        }

        public int ToDoCount
        {
            get
            {
                return i_ToDoCount;
            }
            private set
            {
                i_ToDoCount = value;
            }
        }

        public string Filler
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public WorklistItem()
        {
        }        

        public WorklistItem(    string name,
            long accountNumber,
            long medicalRecordNumber )
        {            

            Name                    = name;
            AccountNumber		    = accountNumber;
            MedicalRecordNumber     = medicalRecordNumber;
        }

        public WorklistItem(    string name,
            long accountNumber,
            long medicalRecordNumber,
            DateTime admitDate,
            DateTime dischargeDate,
            string primaryPayor,
            string hospitalService,
            string financialClass,
            string clinic,
            string dischargeStatus,
            string isLocked,
            int toDoCount)
        {            

            Name                    = name;
            PrimaryPayor            = primaryPayor;
            AccountNumber		    = accountNumber;
            MedicalRecordNumber     = medicalRecordNumber;
            AdmitDate		        = admitDate;
            DischargeDate		    = dischargeDate;
            HospitalService         = hospitalService;
            FinancialClass	        = financialClass;
            Clinic      		    = clinic;
            DischargeStatus         = dischargeStatus;
            IsLocked                = isLocked; 
            ToDoCount               = toDoCount;
        }

        #endregion

        #region Data Elements

        private DateTime    i_AdmitDate;
        private DateTime    i_DischargeDate;
        private long        i_AccountNumber;
        private long        i_MedicalRecordNumber;
        private string      i_Name              = string.Empty;        
        private string      i_HospitalService   = string.Empty;
        private string      i_FinancialClass    = string.Empty;
        private string      i_Clinic            = string.Empty;
        private string      i_PrimaryPayor      = string.Empty;
        private string      i_DischargeStatus   = string.Empty;
        private string      i_IsLocked          = string.Empty;
        private int         i_ToDoCount;

        #endregion

        #region Constants
        #endregion
    }
}
