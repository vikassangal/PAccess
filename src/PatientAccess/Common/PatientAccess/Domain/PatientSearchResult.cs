using System;
using System.Collections.Generic;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for PatientSearchResult.
    /// </summary>
    [Serializable]
    public class PatientSearchResult :IComparable
    {

		#region Fields 

        private Address     i_Address;
        private List<Name>  i_AkaNames = new List<Name>();
        private DateTime    i_DateOfBirth;
        private Gender      i_Gender = new Gender();
        private string      i_HspCode;
        private long        i_MedicalRecordNumber;
        private Name        i_Name;
        private string      i_SocialSecurityNumber;
        private long i_EMPIScore;

		#endregion Fields 

		#region Constructors 

        public PatientSearchResult( Name name, List<Name> akaNames, Gender gender, DateTime dob, string ssn, long mrn, Address address, string hspCode )
        {
            this.Name                   = name;
            this.AkaNames               = akaNames;
            this.Gender                 = gender;
            this.DateOfBirth            = dob;
            this.SocialSecurityNumber   = ssn;
            this.MedicalRecordNumber    = mrn;
            this.Address                = address;
            this.HspCode                = hspCode;
        }

        public PatientSearchResult( Name name, Gender gender, DateTime dob, string ssn, long mrn , Address address, string hspCode )
        {
            this.Name                   = name;
            this.Gender                 = gender;
            this.DateOfBirth            = dob;
            this.SocialSecurityNumber   = ssn;
            this.MedicalRecordNumber    = mrn;
            this.Address                = address;
            this.HspCode                = hspCode;
         }
        public PatientSearchResult()
        {
            this.Name = new Name(String.Empty,String.Empty, String.Empty );
            this.Gender = new Gender();
            this.DateOfBirth = new DateTime();
            this.SocialSecurityNumber = new SocialSecurityNumber().DisplayString;
            this.MedicalRecordNumber = 0;
            this.Address = new Address();
            this.HspCode = String.Empty;
        }
		#endregion Constructors 

		#region Properties 

      public int CompareTo(object obj)
        {
            if (obj is PatientSearchResult)
            {
                var result = (PatientSearchResult) obj;

                return EMPIScore.CompareTo(result.EMPIScore);
            }

            throw new ArgumentException("object is not a EMPIScore");
        }

        public string FormattedAddress
        {
            get
            {
                return Address.OneLineAddressLabel();
            }
        }

        public Address Address 
        {
            get
            {
                return i_Address;
            }
            set
            {
                i_Address = value;
            }
        }

        public List<Name> AkaNames
        {
            get
            {
                return i_AkaNames;
            }
            set
            {
                i_AkaNames = value;
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

        public Gender Gender
        {
            get
            {
                return i_Gender;
            }
            set
            {
                i_Gender = value;
            }
        }

        public string HspCode
        {
            get
            {
                return i_HspCode;
            }
            set
            {
                i_HspCode = value;
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

        public Name Name
        {
            get
            {
                return i_Name;
            }
             set
            {
                i_Name = value;
            }
        }

        public string FormattedName
        {
            get
            {
                return Name.AsFormattedName();
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
        public long EMPIScore
        {
            get { return i_EMPIScore; }
            set { i_EMPIScore = value; }
        }
		#endregion Properties 

    }
}
