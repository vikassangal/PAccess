using System;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class Physician : Person, IComparable
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override string ToString()
        {
            if ( this.Name != null && !this.Name.IsEmpty())
            {
                return this.Name.AsFormattedName();
            }
            else
            {
                return this.FullName;
            }
        }

        /// Check if the NPI Luhn check-digit is correct.
        /// This code is adapted from the ISO 7812 annex.
        /// </summary>
        /// <param name="npi">The 10 or 10+5 digit NPI number that is to be validated.</param>
        /// <returns>Returns TRUE if the luhn check is OK, otherwise FALSE.</returns>
        public static bool isValidNPI( string npi )
        {
            int sum = computeNPI(npi);

            /* If the checksum mod 10 is zero then the NPI is valid */
            if( sum != INVALID_NPI_INDICATOR && ( sum % 10 ) == 0 )
                return true;
            else
                return false;
        }

        public static char checkDigitNPI( string npi9 )
        {
            /* the Luhn check-digit is the tens complement of the low order
             * digit of the sum, and then it is converted to a character
             * (both ASCII or EBCDIC) by adding it to '0'
             */
            int sum = computeNPI(npi9);
            return sum == INVALID_NPI_INDICATOR ? INVALID_NPI : (char)( ( 10 - ( sum % 10 ) ) % 10 + '0' );
        }

        #endregion

        #region Properties
        public long PhysicianNumber
        {
            get
            {
                return base.Oid;
            }
            set
            {
                base.Oid = value;
            }
        }

        public string FullName
        {
            get
            {
                return i_FullName;
            }
            private set
            {
                i_FullName = value;
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

        public string Status
        {
            get
            {
                return i_Status;
            }
            set
            {
                i_Status = value;
            }
        }

        public string AdmittingPrivileges
        {
            get
            {
                return i_AdmittingPrivileges;
            }
            set
            {
                i_AdmittingPrivileges = value;
            }
        }

        public string ActiveInactiveFlag
        {
            get
            {
                return i_ActiveInactiveFlag;
            }
            set
            {
                i_ActiveInactiveFlag = value;
            }
        }

        public DateTime DateActivated
        {
            get
            {
                return i_DateActivated;
            }
            set
            {
                i_DateActivated = value;
            }
        }

        public DateTime DateInactivated
        {
            get
            {
                return i_DateInactivated;
            }
            set
            {
                i_DateInactivated = value;
            }
        }

        public DateTime DateExcluded
        {
            get
            {
                return i_DateExcluded;
            }
            set
            {
                i_DateExcluded = value;
            }
        }

        public string ExcludedStatus
        {
            get
            {
                return i_ExcludedStatus;
            }
            set
            {
                i_ExcludedStatus = value;
            }
        }
        
        public int TotalPatients
        {
            get
            {
                return i_TotalPatients;
            }
            private set
            {
                i_TotalPatients = value;
            }
        }

        public int TotalAttendingPatients
        {
            get
            {
                return i_AttendingPatients;
            }
            private set
            {
                i_AttendingPatients = value;
            }
        }  

        public int TotalAdmittingPatients
        {
            get
            {
                return i_AdmittingPatients;
            }
            private set
            {
                i_AdmittingPatients = value;
            }
        } 

        public int TotalReferringPatients
        {
            get
            {
                return i_ReferringPatients;
            }
            private set
            {
                i_ReferringPatients = value;
            }
        } 

        public int TotalConsultingPatients
        {
            get
            {
                return i_ConsultingPatients;
            }
            private set
            {
                i_ConsultingPatients = value;
            }
        }

        public int TotalOperatingPatients
        {
            get
            {
                return i_OperatingPatients;
            }
            private set
            {
                i_OperatingPatients = value;
            }
        }

        public Speciality Specialization
        {
            get
            {
                return i_Specialization;
            }
            set
            {
                i_Specialization = value;
            }
        }

        
        public PhoneNumber PhoneNumber
        {
            get
            {
                return i_PhoneNumber;
            }
            set
            {
                i_PhoneNumber = value;
            }
        }
        
        public PhoneNumber CellPhoneNumber
        {
            get
            {
                return i_CellPhoneNumber;
            }
            set
            {
                i_CellPhoneNumber = value;
            }
        }

        public PhoneNumber PagerNumber
        {
            get
            {
                return i_PagerNumber;
            }
            set
            {
                i_PagerNumber = value;
            }
        } 

        public string StateLicense
        {
            get
            {
                return i_StateLicense;
            }
            set
            {
                i_StateLicense = value;
            }
        }

        public DriversLicense FederalLicense
        {
            get
            {
                return i_FederalLicense;
            }
            set
            {
                i_FederalLicense = value;
            }
        }

        public string Title
        {
            get
            {
                return i_Title;
            }
            set
            {
                i_Title = value;
            }
        }

        public string UPIN
        {
            get
            {
                return i_UPIN;
            }
            set
            {
                i_UPIN = value;
            }
        }

        
        public long PIN
        {
            get
            {
                return i_PIN;
            }
            set
            {
                i_PIN = value;
            }
        }

        public string MedicalGroupNumber
        {
            get
            {
                return i_MedicalGroupNumber;
            }
            set
            {
                i_MedicalGroupNumber = value;
            }
        }

		public DateTime AdmitPrivilegeSuspendDate
		{
			get
			{
				return i_AdmitPrivilegeSuspendDate;
			}
			set
			{
				i_AdmitPrivilegeSuspendDate = value;
			}
		}
        public string FormattedPhysicianNumber
        {
            get
            {
                string formattedNumber = this.PhysicianNumber.ToString();

                formattedNumber = formattedNumber.PadLeft(5,'0');

                return formattedNumber;
            }
        }

        public string NPI
        {
            get
            {
                return i_NPI;
            }
            set
            {
                i_NPI = value;
            }
        }
        #endregion

        #region Private Methods
        private static int computeNPI(string npi)
        {
            int tmp;  // Current digit of interest
            int sum;  // Hash
            int i;    // Length of NPI
            int j;    // Even-odd bit

            /* the NPI is a 10 digit number, but it could be
             * preceded by the ISO prefix for the USA (80840)
             * when stored as part of an ID card.  The prefix
             * must be accounted for, so the NPI check-digit
             * will be the same with or without prefix.
             * The magic constant for 80840 is 24.
             */
            i = npi.Length;
            if( ( i == 15 ) && ( npi.IndexOf( "80840", 0, 5 ) == 0 ) )
                sum = 0;
            else if( i == 10 )
                sum = 24;	        /* to compensate for the prefix */
            else
                return INVALID_NPI;		/* length must be 10 or 15 bytes */

            /* the algorithm calls for calculating the check-digit
             * from right to left.
             * First, intialize the odd/even counter, taking into account
             * that the rightmost digit is presumed to be the check-sum
             * so in this case the rightmost digit is even instead of
             * being odd
             */
            j = 0;
            /* now scan the NPI from right to left */
            while( i != 0 )
            {	/* only digits are valid for the NPI */
                if( !char.IsDigit( npi[i - 1] ) )
                    return INVALID_NPI;
                /* this conversion works for ASCII and EBCDIC */
                tmp = npi[i - 1] - '0';
                /* the odd positions are multiplied by 2 */
                if( ( j++ % 2 ) != 0 )
                {
                    if( ( tmp <<= 1 ) > 9 )
                    {	/* when the multiplication by 2 
						 * results in a two digit number
						 * (i.e., greater than 9) then the
						 * two digits are added up.  But we
						 * know that the left digit must be
						 * '1' and the right digit must be
						 * x mod 10.  In that case we can
						 * just subtract 10 instead of 'mod'
						 */
                        tmp -= 10;	/* 'tmp mod 10' */
                        tmp++;		/* left digit is '1' */
                    }
                }
                sum += tmp;
                i--;
            }
            return sum;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Physician(): base()
        {
        }

        public Physician( long oid, DateTime version ) : base( oid, version )
        {
        }

        public Physician( long physicianNumber, DateTime version, string fullName )
            : base( physicianNumber, version )
        {
            this.FullName         = fullName;
        }

        public Physician( long physicianNumber, DateTime version, string fullName, Name name )
            : base( physicianNumber, version, name )
        {
            this.FullName      = fullName;
        }

        public Physician( long physicianNumber, DateTime version, string fullName,
            string firstName, string lastName, string middleInitial )
            : base( physicianNumber, version )
        {
            this.i_FullName      = fullName;
            if( !(firstName.Trim().Length == 0 &&
                lastName.Trim().Length == 0 &&
                middleInitial.Trim().Length == 0 ) &&
                !(firstName == null &&
                lastName == null &&
                middleInitial == null ))
            {
                base.Name = new Name( firstName, lastName, middleInitial );
            }
        }

        public Physician( long physicianNumber, DateTime version, int totalPatients, int attendingPatients,
                          int admittingPatients,  int referringPatients, int consultingPatients,
                          int operatingPatients ) 
            : base( physicianNumber, version )
        {
            this.TotalPatients = totalPatients;
            this.TotalAdmittingPatients = admittingPatients;
            this.TotalAttendingPatients = attendingPatients;
            this.TotalConsultingPatients = consultingPatients;
            this.TotalReferringPatients = referringPatients;
            this.TotalOperatingPatients = operatingPatients;
        }

        #endregion

        #region Data Elements
        private string i_FullName;
        private Address i_Address = new Address();
        private int i_TotalPatients;
        private int i_AttendingPatients;
        private int i_AdmittingPatients;
        private int i_ReferringPatients;
        private int i_ConsultingPatients;
        private int i_OperatingPatients;
        private Speciality i_Specialization;
        private string i_StateLicense   = string.Empty;
        private DriversLicense i_FederalLicense;
        private PhoneNumber i_PhoneNumber = new PhoneNumber();
        private PhoneNumber i_PagerNumber = new PhoneNumber();
        private PhoneNumber i_CellPhoneNumber = new PhoneNumber();
        private string i_Status = string.Empty;
        private string i_AdmittingPrivileges = string.Empty;
        private string i_ActiveInactiveFlag = string.Empty;
        private DateTime i_DateActivated;
        private DateTime i_DateInactivated;
        private DateTime i_DateExcluded;
		private DateTime i_AdmitPrivilegeSuspendDate;
        private string i_ExcludedStatus = string.Empty;
        private string i_Title = string.Empty;
        private long i_PIN;
        private string i_UPIN   = string.Empty;
        private string i_MedicalGroupNumber;
        private string i_NPI = string.Empty;
        #endregion

        #region Constants

        public const long NO_PCP_NUMBER = 8887;
        public const long UNKNOWN_PCP_NUMBER = 12345;
        public const long NON_STAFF_PHYSICIAN_NUMBER = 8888;
        private const int INVALID_NPI_INDICATOR = -1;
        private const char INVALID_NPI = '!';

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {           
            if(obj is Physician) 
            {
                Physician doc = (Physician) obj;

                return this.Name.AsFormattedName().CompareTo(doc.Name.AsFormattedName());
            }
    
            throw new ArgumentException("object is not a Physician");                
        }
       
        #endregion
    }

}