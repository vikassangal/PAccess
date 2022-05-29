using System;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Create Search criteria to inclde only the values needed in a search criteria 
    /// to find duplicate PreReg accounts This is used when a new PreReg account is
    /// is created , A warning is displayed with the list of existing preReg accounts
    /// for the patient on the same admit date .
    /// </summary>
    [Serializable]
    public class DuplicatePreRegAccountsSearchCriteria : SearchCriteria
    {
        #region Event Handlers
        #endregion

        #region Public Methods
        public override ValidationResult Validate()
        {
            if( this.SocialSecurityNumber != null )
            {
                if( this.SocialSecurityNumber.Series != null && this.SocialSecurityNumber.IsComplete )
                {
                    return new ValidationResult( true );
                }
                else if( this.SocialSecurityNumber.Series != null &&
                    !this.SocialSecurityNumber.IsComplete &&
                    this.SocialSecurityNumber.Series != String.Empty )
                {
                    return new ValidationResult( false, ERR_MSG_PARTIAL_SSN, SOCIAL_SECURITY_NUMBER );
                }

                if( this.MedicalRecordNumber.ToString() != String.Empty )
                {
                    return new ValidationResult( true );
                }
            }

            return new ValidationResult( false );
        }
        #endregion

        #region Properties
        public long AccountNumber
        {
            private set
            {
                i_AccountNumber = value;
            }
            get
            {
                return i_AccountNumber;
            }
        }
        public long FacilityID
        {
            private set
            {
                i_FacilityID = value;
            }
            get
            {
                return i_FacilityID;
            }
        }
        public DateTime AdmitDate
        {
            private set
            {
                i_AdmitDate = value;
            }
            get
            {
                return i_AdmitDate;
            }
        }
   
        public string FirstName
        {
            private set
            {
                i_FirstName = value;
            }
            get
            {
                return i_FirstName;
            }
        }
        public string LastName
        {
            private set
            {
                i_LastName = value;
            }
            get
            {
                return i_LastName;
            }
        }
        public SocialSecurityNumber SocialSecurityNumber
        {
            private set
            {
                i_SocialSecurityNumber = value;
            }
            get
            {
                return i_SocialSecurityNumber;
            }
        }
        public long MedicalRecordNumber
        {
            set
            {
                i_MedicalRecordNumber = value;
            }
            get
            {
                return i_MedicalRecordNumber;
            }
        }
        public DateTime DateOfBirth
        {
            private set
            {
                i_DateOfBirth = value;
            }
            get
            {
                return i_DateOfBirth;
            }
        }
    

        #endregion
        
        #region Construction and Finalization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="socialSecurityNumber"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="medicalRecordNumber"></param>
        /// <param name="accountNumber"></param>
        public DuplicatePreRegAccountsSearchCriteria( long facilityID, long accountNumber, Name name , SocialSecurityNumber  socialSecurityNumber, DateTime dateOfBirth , long medicalRecordNumber, DateTime admitDate )
           
        {
            this.AccountNumber = accountNumber;
            this.FacilityID = facilityID;
            this.FirstName = name.FirstName;
            this.LastName = name.LastName;
            this.MedicalRecordNumber = medicalRecordNumber;
            this.SocialSecurityNumber = socialSecurityNumber;
            this.AdmitDate = admitDate;
            this.DateOfBirth = dateOfBirth;
        }
        #endregion

        #region Data Elements
        private long i_AccountNumber;
        private DateTime i_AdmitDate = DateTime.MinValue;
        private string i_FirstName ;
        private string i_LastName ;
        private SocialSecurityNumber i_SocialSecurityNumber;
        private DateTime i_DateOfBirth;
        private long i_MedicalRecordNumber;
        private long i_FacilityID;
        #endregion

        #region Constants
        private const string
          SOCIAL_SECURITY_NUMBER = "SocialSecurityNumber";

        private const string
            ERR_MSG_PARTIAL_SSN = "A partial SSN cannot be used for a search.  Either provide a full SSN or delete the SSN entry.";

        #endregion

    }
}