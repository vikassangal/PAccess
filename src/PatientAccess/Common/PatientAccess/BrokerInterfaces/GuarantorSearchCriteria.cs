using System;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    //TODO: Create XML summary comment for GuarantorSearchCriteria
    [Serializable]
    public class GuarantorSearchCriteria : SearchCriteria
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public virtual string FirstName
        {
            get
            {
                return i_FirstName;
            }
            set
            {
                i_FirstName = value;
            }
        }

        public virtual string LastName
        {
            get
            {
                return i_LastName;
            }
            set
            {
                i_LastName = value;
            }
        }

        public virtual Gender Gender
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

        public virtual SocialSecurityNumber SocialSecurityNumber
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
        #endregion

        #region Private Methods
        public override ValidationResult Validate()
        {
            if( this.LastName != String.Empty &&
                this.FirstName != String.Empty &&
                this.Gender != null &&
				this.Gender.Code != String.Empty )
            {
                return new ValidationResult( true );
            }
            else if( this.SocialSecurityNumber.Series != String.Empty && 
                     this.SocialSecurityNumber.IsComplete )
            {
                return new ValidationResult( true );
            }
            else if( this.SocialSecurityNumber.IsPartialSSN )
            {
                return new ValidationResult( false, ERR_MSG_SSN, SOCIAL_SECURITY_NUMBER );
            }
            else if( this.SocialSecurityNumber.Series == String.Empty &&
                   ( this.LastName == String.Empty || this.FirstName == String.Empty ||
                     ( this.Gender == null || this.Gender.Code == String.Empty ) ) )
            {
                return new ValidationResult( false, ERR_MSG_NO_SSN, LAST_NAME );
            }

            return new ValidationResult( false );
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// 
        /// </summary>
        public GuarantorSearchCriteria()
            : base()
        {
        }

        public GuarantorSearchCriteria( string HSPCode, string firstName, string lastName, Gender gender, string ssn )
            : base( HSPCode )
        {
            this.LastName               = lastName;
            this.FirstName              = firstName;
            this.Gender                 = gender;
            if( ssn != String.Empty )
            {
                this.SocialSecurityNumber = new SocialSecurityNumber( ssn );
            }
            else
            {
                this.SocialSecurityNumber = new SocialSecurityNumber();
            }
        }

        public GuarantorSearchCriteria( string HSPCode, string firstName, string lastName, Gender gender, SocialSecurityNumber ssn )
            : base( HSPCode )
        {
            this.LastName               = lastName;
            this.FirstName              = firstName;
            this.Gender                 = gender;
            this.SocialSecurityNumber   = ssn;
        }
        #endregion

        #region Data Elements
        private string                  i_FirstName;
        private string                  i_LastName;
        private Gender                  i_Gender;
        private SocialSecurityNumber    i_SocialSecurityNumber;
        #endregion

        #region Constants
        private const string
            SOCIAL_SECURITY_NUMBER  = "SocialSecurityNumber",
            FIRST_NAME              = "FirstName",
            LAST_NAME               = "LastName";

        private const string
            ERR_MSG_SSN     = "A partial SSN cannot be used for a search.  Either provide a full SSN or delete the SSN entry.",
            ERR_MSG_NO_SSN  = "When searching without a Guarantor SSN, the guarantor last name, first name, and gender are required.";
        #endregion
    }
}