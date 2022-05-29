using System;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for PriorAccountsRequest.
	/// </summary>
    [Serializable]
    public class PriorAccountsRequest
	{
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public long MedicalRecordNumber 
        {
            get
            {
                return medicalRecordNumber;
            }
            set
            {
                medicalRecordNumber = value;
            }
        }

        public long AccountNumber 
        {
            get
            {
                return accountNumber;
            }
            set
            {
                accountNumber = value;
            }
        }

        public long FacilityOid 
        {
            get
            {
                return facilityOid;
            }
            set
            {
                facilityOid = value;
            }
        }

        public Facility Facility 
        {
            get
            {
                return facility;
            }
            set
            {
                facility = value;
            }
        }

        public string Upn 
        {
            get
            {
                return upn;
            }
            set
            {
                upn = value;
            }
        }

        public bool IsAccountNull 
        {
            get
            {
                return isAccountNull;
            }
            set
            {
                isAccountNull = value;
            }
        }

        public bool IsPatientNull 
        {
            get
            {
                return isPatientNull;
            }
            set
            {
                isPatientNull = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PriorAccountsRequest()
        {
        }
        #endregion

        #region Data Elements
        private long facilityOid;
        private long accountNumber;
        private long medicalRecordNumber;
        private string upn;
        private Facility facility;
        private bool isAccountNull = false;
        private bool isPatientNull = false;
        #endregion

        #region Constants
        #endregion
	}
}
