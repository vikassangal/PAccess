using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain.ADTEvents
{
    /// <summary>
    /// Summary description for ADTPatient
    /// </summary>
    [Serializable]
    public class ADTPatient : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
     

       
        #endregion

        #region Properties
       
       
        public string MaritalStatusCode
        {
            get
            {
                return i_MaritalStatusCode;
            }
            set
            {
                i_MaritalStatusCode = value;
            }
        }
        public long MedicalRecordNumber
        {
            get
            {
                return i_MedicalRecordNumber;
            }
            set
            {
                i_MedicalRecordNumber = value;
            }
        }
        public Facility Facility
        {
            get
            {
                return i_Facility;
            }
            set
            {
                i_Facility = value;
            }
        }
        public long Accountnumber
        {
            get
            {
                return i_Accountnumber;
            }
            set
            {
                i_Accountnumber = value;
            }
        }
        public int PersonID
        {
            get
            {
                return i_PersonID;
            }
            set
            {
                i_PersonID = value;
            }
        }
        public int VisitID
        {
            get
            {
                return i_VisitID;
            }
            set
            {
                i_VisitID = value;
            }
        }
        public int PatientID
        {
            get
            {
                return i_PatientID;
            }
            set
            {
                i_PatientID = value;
            }
        }
        public string RaceCode
        {
            get
            {
                return i_RaceCode;
            }
            set
            {
                i_RaceCode = value;
            }
        }
        public string EthnicityCode
        {
            get
            {
                return i_EthnicityCode;
            }
            set
            {
                i_EthnicityCode = value;
            }
        }
        public string DriversLicenseNumber
        {
            get
            {
                return i_DriversLicenseNumber.Substring( 0, 
                    i_DriversLicenseNumber.Length - 2 );
            }
            set
            {
                i_DriversLicenseNumber = value;
            }
        }
        public string DriversLicenseStateCode
        {
            get
            {
                return i_DriversLicenseNumber.Substring(
                    i_DriversLicenseNumber.Length - 2, 2 );
            }
            set
            {
                i_DriversLicenseStateCode = value;
            }
        }

        public string PlaceOfBirth
        {
            get
            {
                return i_PlaceOfBirth;
            }
            set
            {
                i_PlaceOfBirth = value;
            }
        }
        public string LanguageCode
        {
            get
            {
                return i_LanguageCode;
            }
            set
            {
                i_LanguageCode = value;
            }
        }
        public string ReligionCode
        {
            get
            {
                return i_ReligionCode;
            }
            set
            {
                i_ReligionCode = value;
            }
        }

        public string EmailAddress
        {
            get
            {
                return i_EmailAddress;
            }
            set
            {
                i_EmailAddress = value;
            }
        }

        public string NationalCode
        {
            get
            {
                return i_NationalCode;
            }
            set
            {
                i_NationalCode = value;
            }
        }
        public DateTime DateOfBirth
        {
            get
            {
                return i_DateOfBirth;
            }
            set
            {
                i_DateOfBirth = value;
            }
        }
        public string ReligiousCongregationCode
        {
            get
            {
                return i_ReligiousCongregationCode;
            }
            set
            {
                i_ReligiousCongregationCode = value;
            }
        }
        public string NPPVersionCode
        {
            get
            {
                return i_NPPVersionCode;
            }
            set
            {
                i_NPPVersionCode = value;
            }
        }
        public DateTime NPPDate
        {
            get
            {
                return i_NPPDate;
            }
            set
            {
                i_NPPDate = value;
            }
        }
      
        public int DateOfBirthYear
        {
            get
            {
                return i_DateOfBirth.Year;
            }
            
        }
        public int DateOfBirthMonth
        {
            get
            {
                return i_DateOfBirth.Month;
            }
           
        }
        public string GenderCode
        {
            get
            {
                return i_GenderCode;
            }
            set
            {
                i_GenderCode = value;
            }

        }
        public string SocialSecurityNumber
        {
            get
            {
                return i_SocialSecurityNumber;
            }
            set
            {
                i_SocialSecurityNumber = value;
            }

        }
        public string FinancialClassCode
        {
            get
            {
                return i_FinancialClassCode;
            }
            set
            {
                i_FinancialClassCode = value;
            }

        }
        public YesNoFlag  BloodLess
        {
            get
            {
                return i_BloodLess;
            }
            set
            {
                i_BloodLess = value;
            }

        }
        public YesNoFlag  Smoking
        {
            get
            {
                return i_Smoking;
            }
            set
            {
                i_Smoking = value;
            }

        }
        public void AddContactPoint( ContactPoint aContactPoint )
        {
            if( !this.ContactPoints.Contains( aContactPoint ) )
            {
                this.ContactPoints.Add( aContactPoint );
            }
        }

        private ArrayList ContactPoints
         {
             get
             {
                 return i_ContactPoints;
             }
             set
             {
                 i_ContactPoints = value;
             }
         }
        /// <summary>
        /// /////////////
        /// </summary>
        /// 
        private void AddName( Name aName )
        {
            if( !this.Names.Contains( aName ) )
            {
                this.Names.Add( aName );
            }
        }

        public void AddNames( ICollection names )
        {
            foreach (Name nm in names)
            {
                AddName(nm);
            }           
        }

        private ArrayList Names
        {
            get
            {
                return i_Names;
            }
            set
            {
                i_Names = value;
            }
        }
        #endregion

        #region Private Methods
   

        #endregion

        #region Construction and Finalization
      
        public ADTPatient() : base()
        {
        }
        #endregion

        #region Data Elements
        private long  i_MedicalRecordNumber;
        private Facility  i_Facility = new Facility();
        private long  i_Accountnumber;

        private ArrayList i_Names    = new ArrayList();
        private ArrayList i_ContactPoints    = new ArrayList();
        private int  i_PersonID  ;
        private int  i_VisitID  ;
        private string  i_EmailAddress = string.Empty;
        private int  i_PatientID  ;
        private string   i_MaritalStatusCode = string.Empty;
        private string   i_RaceCode = string.Empty;
        private string   i_EthnicityCode = string.Empty;
        private string   i_DriversLicenseNumber = string.Empty;
        private string   i_DriversLicenseStateCode = string.Empty;
        private string   i_PlaceOfBirth = string.Empty;
        private string   i_LanguageCode =  string.Empty;
        private string   i_ReligionCode =  string.Empty;
        private DateTime  i_DateOfBirth = DateTime.Now ;
        private string  i_NationalCode = string.Empty;
        private string  i_ReligiousCongregationCode = string.Empty;
        private YesNoFlag  i_Smoking = new YesNoFlag();
        private string  i_NPPVersionCode = string.Empty;
        private DateTime   i_NPPDate = DateTime.Now;
        private string     i_FinancialClassCode = string.Empty;
        
        private YesNoFlag  i_BloodLess= new YesNoFlag();
        private string     i_GenderCode = string.Empty;
        private string     i_SocialSecurityNumber = string.Empty;     

     
        #endregion

        #region Constants
        #endregion
    }
}
