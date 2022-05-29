using System;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace PatientAccess.BrokerInterfaces
{
    //TODO: Create XML summary comment for PatientSearchCriteria
    [Serializable]
    public class PatientSearchCriteria : SearchCriteria
    {
        #region Event Handlers
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new Patient and Account using the values in this SearchCriteria
        /// </summary>
        /// <returns>A new Patient object populated from my values</returns>
        public Patient CreateNewPatientWith( Activity anActivity, Facility currentFacility )
        {
            Patient newPatient = null;

            // Verify that it is legitimate to create a patient
            // for the supplied activity.
            if( anActivity.CanCreateNewPatient() )
            {
                newPatient = new Patient();
                Account newAccount = new Account();
                
                newAccount.Patient = newPatient;
                newAccount.Activity = anActivity;
                newPatient.SelectedAccount = newAccount;
                newPatient.Facility = currentFacility;

                this.PopulateAccount( newAccount );                
                this.PopulatePatient( newPatient );
            }

            return newPatient;
        }
        public void UpdateEMPIPatientDetails(Patient pbarPatient, Activity currentActivity)
        {
            var currentFacility = User.GetCurrent().Facility;
            var empiServiceBroker = BrokerFactory.BrokerOfType<IEMPIBroker>();
            currentActivity.EmpiPatient.Patient = pbarPatient;
            currentActivity.EmpiPatient = empiServiceBroker.GetEMPIPatientFor(
                currentActivity.EmpiPatient.Patient, currentFacility);
        }

        public Patient CreateNewPatientWithEMPIData(Activity anActivity, Patient empiPatient)
        {
            var newPatient = new Patient();
             
            if (anActivity.CanCreateNewPatient())
            {
                 
                UpdateEMPIPatientDetails(empiPatient, anActivity); 
                newPatient = anActivity.EmpiPatient.Patient;

                var newAccount = new Account { Activity = anActivity, Patient = newPatient };
                newAccount.OverLayEMPIData();
                newAccount.Activity.EmpiPatient = null;
                newAccount.Patient.IsNew = true;
                newAccount.Patient.MedicalRecordNumber = 0;
                newPatient.SelectedAccount = newAccount;
                newPatient.IsNew = true;
            }
            return newPatient;
        }

        public override ValidationResult Validate()
        {
            if( this.SocialSecurityNumber.Series != null && this.SocialSecurityNumber.IsComplete )
            {
                return new ValidationResult( true );
            }
            else if( this.SocialSecurityNumber.Series != null && 
                ! this.SocialSecurityNumber.IsComplete &&
                this.SocialSecurityNumber.Series != String.Empty)

            {
                return new ValidationResult( false, ERR_MSG_PARTIAL_SSN, SOCIAL_SECURITY_NUMBER );
            }

            if( this.MedicalRecordNumber != String.Empty )
            {
                return new ValidationResult( true );
            }

            if (this.AccountNumber != String.Empty)
            {
                return new ValidationResult(true);
            }

            if (this.PhoneNumber != null && !String.IsNullOrEmpty(this.PhoneNumber.AsUnformattedString()) && this.PhoneNumber.IsValid)
            {
                return new ValidationResult(true);
            }

            if( this.LastName != String.Empty &&
                ( this.FirstName != String.Empty ||
                  ( this.Gender != null && this.Gender.Description != String.Empty ) ||
                    this.MonthOfBirth != NO_MONTH ||
                    this.YearOfBirth != NO_YEAR ) )
            {
                return new ValidationResult( true );
            }
            else if( this.LastName != String.Empty )
            {
                // last name is entered, but all others are empty
                return new ValidationResult( false, ERR_MSG_LASTNAME_ONLY, LAST_NAME );
            }

            if( this.FirstName != String.Empty &&
                this.Gender != null && (
                this.MonthOfBirth != NO_MONTH ||
                this.YearOfBirth != NO_YEAR
                )
                )
            {
                return new ValidationResult( true );
            }
            else if( this.FirstName != String.Empty &&
                this.Gender == null && (
                this.MonthOfBirth == NO_MONTH ||
                this.YearOfBirth == NO_YEAR
                )
                )
            {
                return new ValidationResult( false, ERR_MSG_FNAME_GENDER_DOB, FIRST_NAME );
            }
            else if( this.Gender != null &&
                this.FirstName == String.Empty ||
                this.MonthOfBirth == NO_MONTH ||
                this.YearOfBirth == NO_YEAR                
                )
            {
                return new ValidationResult( false, ERR_MSG_FNAME_GENDER_DOB, GENDER );
            }
            else if( this.MonthOfBirth != NO_MONTH &&
                this.FirstName == String.Empty ||
                this.Gender == null ||
                this.YearOfBirth == NO_YEAR                
                )
            {
                return new ValidationResult( false, ERR_MSG_FNAME_GENDER_DOB, MONTH_OF_BIRTH );
            }
            else if( this.YearOfBirth != NO_YEAR &&
                this.FirstName == String.Empty ||
                this.Gender == null ||
                this.MonthOfBirth == NO_MONTH              
                )
            {
                return new ValidationResult( false, ERR_MSG_FNAME_GENDER_DOB, MONTH_OF_BIRTH );
            }

            return new ValidationResult( false );
        }
        #endregion

        #region Properties
        public string AccountNumber 
        {
            get
            {
                return i_AccountNumber;
            }
            set
            {
                i_AccountNumber = value;
            }
        }

        public long MonthOfBirth
        {
            get
            {
                long monthOfBirth;
                if( this.i_MonthOfBirth == 0 )
                {
                    monthOfBirth = NO_MONTH;
                }
                else
                {
                    monthOfBirth = i_MonthOfBirth;
                }
                return monthOfBirth;
            }
             set
            {
                if( value == 0 )
                {
                    i_MonthOfBirth = NO_MONTH;
                }
                else
                {
                    i_MonthOfBirth = value;
                }
            }
        }

        public long YearOfBirth
        {
            get
            {
                long yearOfBirth;
                if( this.i_YearOfBirth.ToString().Length != 4 )
                {
                    yearOfBirth = NO_YEAR;
                }
                else
                {
                    yearOfBirth = this.i_YearOfBirth;
                }
                return yearOfBirth;
            }
            set
            {
                if (value.ToString().Length == 4)
                {
                    i_YearOfBirth = value;
                }
                else
                {
                    i_YearOfBirth = NO_YEAR;
                }
            }
        }

        public long DayOfBirth
        {
            get
            {
                if (this.i_DayOfBirth <= 0)
                {
                    return NO_DAY;
                }
                return i_DayOfBirth;
            }
            set
            {
                if( value.ToString().Length >= 1 || value.ToString().Length <= 2 )
                {
                    i_DayOfBirth = value;
                }
                else
                {
                    i_DayOfBirth = NO_DAY;
                }
            }
        }

        public DateTime DateOfBirth
        {
            get
            {
                DateTime dob;
                try
                {
                    if( this.YearOfBirth != NO_YEAR && 
                        this.MonthOfBirth != NO_MONTH &&
                        this.DayOfBirth != NO_DAY )
                    {
                        dob = new DateTime( 
                            (int)this.YearOfBirth, 
                            (int)this.MonthOfBirth, 
                            (int)this.DayOfBirth 
                            );
                    }
                    else
                    {
                        dob = DateTime.MinValue;
                    }
                }
                catch
                {
                    // If we get an invalid read on a date, we do not
                    // want to throw an exception here, just return 
                    // DateTime.MinValue.
                    dob = DateTime.MinValue;
                }

                return dob;
            }
        }

        public string FirstName 
        {
            get
            {
                return i_FirstName;
            }
            private set
            {
                i_FirstName = value;
            }
        }

        public Gender Gender 
        {
            get
            {
                return i_Gender;
            }
            private set
            {
                i_Gender = value;
            }
        }

        public string LastName 
        {
            get
            {
                return i_LastName;
            }
            private set
            {
                i_LastName = value;
            }
        }

        public string MedicalRecordNumber 
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

        public SocialSecurityNumber SocialSecurityNumber 
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
        public static PatientSearchCriteria Default
        {
            get
            {
                return new PatientSearchCriteria( String.Empty, string.Empty, string.Empty, new SocialSecurityNumber(), null, -1, -1, string.Empty, string.Empty  );
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Populates a Patient with values from this instance.  NOTE: 
        /// this does not populate the MedicalRecordNumber, AccountNumber, or Facility.
        /// </summary>
        /// <param name="patient">A Patient object to be populated with first name, last name, gender, social security number and date of birth.</param>
        private void PopulatePatient( Patient patient )
        {
            patient.Name = new Name( this.FirstName, this.LastName, String.Empty );
            
            if( this.SocialSecurityNumber != null )
            {
                patient.SocialSecurityNumber = this.SocialSecurityNumber;
            }
            
            if( this.Gender != null )
            {
                patient.Sex = this.Gender;
            }
            
            if( this.DateOfBirth != DateTime.MinValue )
            {
                patient.DateOfBirth = this.DateOfBirth;
            }
        }

        private void PopulateAccount( Account newAccount )
        {
            newAccount.Pregnant = new YesNoFlag();
            newAccount.Pregnant.SetBlank( string.Empty );

            newAccount.OccurrenceSpans.Add( null );  //LPP table has 2 span spots. this is span1
            newAccount.OccurrenceSpans.Add( null );  //this is span2

            newAccount.AddOccurrenceCode( new OccurrenceCode() );  //for OCC1

            if ( newAccount.Activity.GetType() == typeof( PreRegistrationActivity ) || newAccount.Activity.GetType() == typeof( ShortPreRegistrationActivity ) )
            {
                newAccount.KindOfVisit = new VisitType(1L, ReferenceValue.NEW_VERSION,  VisitType.PREREG_PATIENT_DESC, VisitType.PREREG_PATIENT );
            }
            //need more setting for other activities since use may go to tabs randomly, not first to Diagnosis.
            

            // set the account in the occurrence code manager
            OccurrenceCodeManager ocm = OccurrenceCodeManager.Instance;
            ocm.OccurrenceCodesLoader = new OccurrenceCodesLoader();
            ocm.Account = newAccount;
        }
        #endregion

        #region Private Properties
        internal IEMPIFeatureManager EMPIFeatureManager { get; set; }
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
        public PatientSearchCriteria( string HSPCode, string firstName, string lastName, string socialSecurityNumber, Gender gender, long monthOfBirth, long yearOfBirth, string medicalRecordNumber, string accountNumber )
            : base(HSPCode)
        {
            this.FirstName              = firstName;
            this.LastName               = lastName;

            if( socialSecurityNumber != String.Empty )
            {
                this.SocialSecurityNumber   = new SocialSecurityNumber( socialSecurityNumber );
            }
            else
                this.SocialSecurityNumber = new SocialSecurityNumber();

            this.Gender                 = gender;
            this.MonthOfBirth           = monthOfBirth;
            this.YearOfBirth            = yearOfBirth;
            this.MedicalRecordNumber    = medicalRecordNumber;
            this.AccountNumber          = accountNumber;
        }

        public PatientSearchCriteria( string HSPCode, string firstName, string lastName, SocialSecurityNumber socialSecurityNumber, Gender gender, long monthOfBirth, long yearOfBirth, string medicalRecordNumber, string accountNumber )
            : base(HSPCode)
        {
            this.FirstName              = firstName;
            this.LastName               = lastName;
            this.SocialSecurityNumber   = socialSecurityNumber;
            this.Gender                 = gender;
            this.MonthOfBirth           = monthOfBirth;
            this.YearOfBirth            = yearOfBirth;
            this.MedicalRecordNumber    = medicalRecordNumber;
            this.AccountNumber          = accountNumber;
        }

        public PatientSearchCriteria(string HSPCode, string firstName, string lastName, string socialSecurityNumber,
            Gender gender, long monthOfBirth, long dayOfBirth , long yearOfBirth, string medicalRecordNumber, string accountNumber, PhoneNumber phoneNumber)
            : base(HSPCode)
        {
            this.FirstName = firstName;
            this.LastName = lastName;

            if (socialSecurityNumber != String.Empty)
            {
                this.SocialSecurityNumber = new SocialSecurityNumber(socialSecurityNumber);
            }
            else
                this.SocialSecurityNumber = new SocialSecurityNumber();

            this.Gender = gender;
            this.MonthOfBirth = monthOfBirth;
            this.YearOfBirth = yearOfBirth;
            this.DayOfBirth = dayOfBirth;
            this.MedicalRecordNumber = medicalRecordNumber;
            this.AccountNumber = accountNumber;
            this.PhoneNumber = phoneNumber;
        }

        #endregion

        #region Data Elements
        private string                  i_FirstName;
        private string                  i_LastName;
        private string                  i_MedicalRecordNumber;
        private string                  i_AccountNumber;
        private SocialSecurityNumber    i_SocialSecurityNumber;
        private PhoneNumber             i_PhoneNumber;
        private long                    i_MonthOfBirth;
        private long                    i_YearOfBirth;
        private long                    i_DayOfBirth;
        private Gender                  i_Gender;

        public PatientSearchCriteria()
        {
        }

        #endregion

        #region Constants

        public const long
            NO_MONTH    = -1L,
            NO_YEAR     = -1L ,
            NO_DAY      = -1L;

        private const string
            SOCIAL_SECURITY_NUMBER  = "SocialSecurityNumber",
            FIRST_NAME              = "FirstName",
            LAST_NAME               = "LastName",
            GENDER                  = "Gender",
            MONTH_OF_BIRTH          = "MonthOfBirth",
            YEAR_OF_BIRTH           = "YearOfBirth";

        private const string
			ERR_MSG_FNAME_GENDER_DOB = "When searching by First Name, Gender, or Date of Birth, the system requires either including all three of these, or adding at least one of the following: Last Name, MRN, Account Number, or SSN.";

        public const string
            ERR_MSG_PARTIAL_SSN      = "A partial SSN cannot be used for a search.  Either provide a full SSN or delete the SSN entry.";

        private const string
            ERR_MSG_LASTNAME_ONLY    = "When searching by Last Name, at least one other field is required.";

        #endregion
    }
}