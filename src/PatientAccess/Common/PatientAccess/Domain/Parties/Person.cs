using System;
using System.Collections;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public abstract class Person : Party
    {
        #region Methods
        /// <summary>
        /// Returns string the displays patient's age in either days, months, or years, 
        /// depending on their age.
        /// </summary>
        public string Age()
        {
            return CalculateAge( DateTime.Now );
        }

        public string GetEmailAddress()
        {
            var contactPoint = ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());

            return (contactPoint != null && contactPoint.EmailAddress != null)
                       ? contactPoint.EmailAddress.ToString()
                       : string.Empty;
        }


        /// <summary>
        /// Returns string that displays patient's age in either days, months, or years, 
        /// for a given date range.
        /// </summary>
        public string AgeAt( DateTime aDate )
        {
            return CalculateAge( aDate );
        }

        /// <summary>
        /// Returns the age of a person in days for a given Age 
        /// that is calculated using AgeAt / Age methods.
        /// </summary>
        public int AgeInDays(string Age)
        {           
            try
            {
                switch ( Age.Substring((Age.Length)-1,1) )
                {
                    case "y" :
                    {
                        int age = Convert.ToInt32( Age.Substring(0, (Age.Length) - 1) ) * 365 ;
                        return age;
                    }
                    case "m" :
                    {
                        int age = Convert.ToInt32( Age.Substring(0, (Age.Length) - 2) ) * (365 / 12);
                        return age;
                    }
                    default :
                    {
                        int age = Convert.ToInt32( Age.Substring(0, (Age.Length) - 2) );
                        return age;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the age of a person in years for a given Age 
        /// that is calculated using CalculateAge method.
        /// </summary>
        public int AgeInYearsFor( DateTime aDate )
        {
            try
            {
                string ageString = this.CalculateAge( aDate );
                if( ageString != String.Empty )
                {
                    switch( ageString.Substring( ( ageString.Length ) - 1, 1 ) )
                    {
                        case "y":
                            {
                                int age = Convert.ToInt32( ageString.Substring( 0, ( ageString.Length ) - 1 ) );
                                return age;
                            }
                        default:
                            {
                                // less than 1yr old
                                return 0;
                            }
                    }
                }
                else
                {
                    // less than 1yr old
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public override bool Equals( object obj )
        {
            if( obj == null )
            {
                return false;
            }
            Person person = obj as Person;
            
            bool license = (person.DriversLicense == null && this.DriversLicense == null ? true : 
                person.DriversLicense == null && this.DriversLicense != null ? false :
                person.DriversLicense != null && this.DriversLicense == null ? false :
                person.DriversLicense.Equals( this.DriversLicense ));
            
            bool passport = (person.Passport == null && this.Passport == null ? true :
                person.Passport == null && this.Passport != null ? false :
                person.Passport != null && this.Passport == null ? false :
                person.Passport.Equals(this.Passport));

            bool employment = true;

            bool sex = (person.Sex == null && this.Sex == null ? true : 
                person.Sex == null && this.Sex != null ? false :
                person.Sex != null && this.Sex == null ? false :
                person.Sex.Equals( this.Sex ));

            bool ssn = (person.SocialSecurityNumber == null && this.SocialSecurityNumber == null ? true : 
                person.SocialSecurityNumber == null && this.SocialSecurityNumber != null ? false :
                person.SocialSecurityNumber != null && this.SocialSecurityNumber == null ? false :
                person.SocialSecurityNumber.Equals( this.SocialSecurityNumber ));

            bool result = (license && passport && employment && sex && ssn &&
                    person.DateOfBirth.Equals( this.DateOfBirth ) &&
                    person.Name.Equals( this.Name ) &&
                    person.NationalID.Equals( this.NationalID ) &&
                    person.FirstName.Equals( this.FirstName ) &&
                    person.LastName.Equals( this.LastName ));
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Properties
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

        public DriversLicense DriversLicense 
        {
            get
            {
                if( i_DriversLicense == null )
                {
                    i_DriversLicense = DriversLicense.NewMissingDriversLicense();
                }
                return i_DriversLicense;
            }
            set
            {
                i_DriversLicense = value;
            }
        }
        
        public Passport Passport
        {
            get
            {
                if (i_Passport == null)
                {
                    i_Passport = new Passport();
                }
                return i_Passport;
            }
            set
            {
                i_Passport = value;
            }
        }

        public Employment Employment
        {
            get
            {
                if( i_Employment == null )
                {
                    i_Employment = new Employment( this );
                }
                return i_Employment;
            }
            set
            {
                i_Employment = value;
            }
        }

        public virtual Name Name 
        { 
            get
            {
                return this.i_Name;
            }
            set
            {
                this.i_Name = value;
            }
        }

        public string NationalID 
        {
            get
            {
                return i_NationalID.Trim();
            }
            set
            {
                i_NationalID = value;
            }
        }
        
        public virtual string FirstName
        {
            get
            {
                return this.Name.FirstName;
            }
            set
            {
                this.Name.FirstName = value;
            }
        }

        public virtual string FormattedName 
        { 
            get
            {
                return this.Name.AsFormattedName();
            }
        }

        public virtual string LastName
        {
            get
            {
                return this.Name.LastName;
            }
            set
            {
                this.Name.LastName = value;
            }
        }

        public virtual Gender Sex 
        {
            get
            {
                return i_Sex;
            }
            set
            {
                this.i_Sex = value;
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

        public EmailReason EmailReason
        {
            get
            {
            var contactPoint = ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());   
            return (contactPoint != null && contactPoint.EmailReason != null)
                       ? contactPoint.EmailReason
                       : null;
            }
            set
            {
                var contactPoint = ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
                contactPoint.EmailReason = value;
            }
        }

        #endregion

        #region Private Methods
        

        /// <summary>
        /// Calculates a patient's age in either days, months, or years, 
        /// for a given date range and returns calculation in a string.
        /// Private method called from Age() and AgeAt().
        /// </summary>
        private string CalculateAge( DateTime dateOfEndRange )
        {
            string displayString;       // Formatted string that's returned to the caller
            int    numberOfDaysOld = 0; // Age in days if patient is less than 31 days old

            int    endYear    = dateOfEndRange.Year;
            int    endMonth   = dateOfEndRange.Month;
            int    endDay     = dateOfEndRange.Day;

            int    birthYear  = DateOfBirth.Year;
            int    birthMonth = DateOfBirth.Month;
            int    birthDay   = DateOfBirth.Day;

            const int DAYS_IN_YEAR = 365;

            // Check for valid input data
            if( dateOfEndRange < DateOfBirth 
                || DateOfBirth == DateTime.MinValue )
            {
                displayString = String.Empty;
                return displayString;
            }

            if( endYear != birthYear )
            {   // Figure out if they've had a birthday this year
                bool hadBirthdayThisYear = false;

                if( endMonth > birthMonth )
                {
                    hadBirthdayThisYear = true;
                }
                else if( endMonth < birthMonth )
                {
                    hadBirthdayThisYear = false;
                }
                else if( birthMonth == endMonth )
                {
                    if( birthDay <= endDay )
                    {
                        hadBirthdayThisYear = true;
                    }
                    else
                    {
                        hadBirthdayThisYear = false;
                    }
                }

                if( hadBirthdayThisYear )
                {
                    int ageInYears = endYear - birthYear;
                    displayString = String.Format( "{0}{1}", ageInYears,
                        ageInYears == 1 ? "y" : "y" );
                }
                else
                {   
                    int ageInYears = endYear - birthYear - 1;

                    if( ageInYears > 0 )
                    {
                        displayString = String.Format( "{0}{1}", ageInYears,
                            ageInYears == 1 ? "y" : "y" );
                    }
                    else
                    {
                        numberOfDaysOld = DAYS_IN_YEAR - this.DateOfBirth.DayOfYear;
                        numberOfDaysOld += dateOfEndRange.DayOfYear;

                        if( DateTime.IsLeapYear( birthYear ) )
                        {
                            numberOfDaysOld++;
                        }
                        if( numberOfDaysOld >= 31 )
                        {
                            int ageInMonths = 12 - birthMonth + endMonth;
                            displayString = String.Format( "{0}{1}", ageInMonths,
                                ageInMonths == 1 ? "m" : "m" );
                        }
                        else
                        {
                            displayString = String.Format( "{0}{1}", numberOfDaysOld,
                                numberOfDaysOld == 1 ? "d" : "d" );
                        }
                    }
                }
            }
            else  // Less then 1 year old
            {
                int dayOfYearBorn   = this.DateOfBirth.DayOfYear;
                int todaysDayOfYear = dateOfEndRange.DayOfYear;
                numberOfDaysOld     = todaysDayOfYear - dayOfYearBorn;

                if( numberOfDaysOld < 31 )
                {
                    displayString = String.Format( "{0}{1}",  numberOfDaysOld,
                        numberOfDaysOld == 1 ? "d" : "d" );
                }
                else
                {
                    int ageInMonths = endMonth - birthMonth;
                    displayString = String.Format( "{0}{1}", ageInMonths,
                        ageInMonths == 1 ? "m" : "m" );
                }
            }

            return displayString;
        }

        /// <summary>
        /// Creates and initializes the hash table used by DayOfYear() method.
        /// </summary>
        private void InitializeCalendarTable()
        {
            calendarTable = new Hashtable( 12 );
            calendarTable.Add( 1,  31 );
            calendarTable.Add( 3,  31 );
            calendarTable.Add( 4,  30 );
            calendarTable.Add( 5,  31 );
            calendarTable.Add( 6,  30 );
            calendarTable.Add( 7,  31 );
            calendarTable.Add( 8,  31 );
            calendarTable.Add( 9,  30 );
            calendarTable.Add( 10, 31 );
            calendarTable.Add( 11, 30 );
            calendarTable.Add( 12, 31 );
        }
        #endregion

        #region Construction and Finalization
        public Person() : base()
        {
            InitializeCalendarTable();
        }

        public Person( long oid, DateTime version ) : base( oid, version )
        {
            this.Employment = new Employment( this );
            InitializeCalendarTable();
        }

        public Person( long oid, DateTime version, Name nameOfPerson ) : this( oid, version )
        {
            this.Name = nameOfPerson;
            InitializeCalendarTable();
        }
        #endregion

        #region Data Elements
        private Name                    i_Name = new Name( String.Empty, String.Empty, String.Empty );
        private Gender                  i_Sex = new Gender();
        private SocialSecurityNumber    i_SocialSecurityNumber = new SocialSecurityNumber(String.Empty);
        private DriversLicense          i_DriversLicense = new DriversLicense(String.Empty,new State());
        private Passport                i_Passport = new Passport(string.Empty, new Country());
        private Employment              i_Employment  = new Employment();
        private DateTime                i_DateOfBirth;
        private string                  i_NationalID  = String.Empty;
        private Hashtable               calendarTable;
        private EmailReason             i_EmailReason = new EmailReason();
        #endregion

        #region Constants
        #endregion        
    }
}
