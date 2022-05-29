using System;
using System.Diagnostics;
using System.Reflection;
using Extensions.PersistenceCommon;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{

    [Serializable]
    public class Authorization : PersistentModel, ICloneable
    {
        
        #region Constants 

        public const string AUTHORIZATION_COMPANY               = "AuthorizationCompany" ;
        public const string AUTHORIZATION_NUMBER                = "AuthorizationNumber" ;
        public const string AUTHORIZATION_STATUS                = "AuthorizationStatus" ;
        public const string AUTHORIZATION_PHONE_NUMBER          = "AuthorizationPhoneNumber" ;
        public const string EFFECTIVE_DATE                      = "EffectiveDate" ;
        public const string EXPIRATION_DATE                     = "ExpirationDate" ;
        public const string NAME_OF_COMPANY_REPRESENTATIVE      = "NameOfCompanyRepresentative" ;
        public const string NUMBER_OF_DAYS_AUTHORIZED           = "NumberOfDaysAuthorized" ;
        public const string REMARKS                             = "Remarks" ;
        public const string SERVICESAUTHORIZED                  = "ServicesAuthorized" ;

        #endregion

        #region Fields 

        private string                                      i_AuthorizationCompany = string.Empty;
        private string                                      i_AuthorizationNumber = String.Empty;
        private PhoneNumber                                 i_AuthorizationPhone = new PhoneNumber();
        private YesNotApplicableFlag                        i_AuthorizationRequired = new YesNotApplicableFlag();
        private AuthorizationStatus                         i_AuthorizationStatus = new AuthorizationStatus();
        private DateTime                                    i_EffectiveDate = new DateTime();
        private DateTime                                    i_ExpirationDate = new DateTime();
        private Name                                        i_NameOfCompanyRepresentative = 
                                                                new Name(string.Empty, string.Empty, string.Empty);
        private int                                         i_NumberOfDaysAuthorized;
        private string                                      i_PromptExt = string.Empty;
        private string                                      i_Remarks = String.Empty;
        private string                                      i_ServicesAuthorized = String.Empty;

		#endregion Fields 

		#region Constructors 

        public Authorization()
        {

            this.AuthorizationRequired.SetBlank();

        }

		#endregion Constructors 

		#region Properties 

        public string AuthorizationCompany
        {
            get
            {
                return i_AuthorizationCompany;
            }
            set
            {

                if( value != null ) value = value.Trim();
                Debug.Assert( value != null );
                this.SetAndTrack<string>( ref this.i_AuthorizationCompany, value, MethodBase.GetCurrentMethod() );

            }
        }

        public string AuthorizationNumber
	    {
		    get 
            { 
                return i_AuthorizationNumber;
            }
		    set 
            {

                Debug.Assert( value != null );
                if( value != null ) value = value.Trim();
                this.SetAndTrack<string>( ref this.i_AuthorizationNumber, value, MethodBase.GetCurrentMethod() );

            }
	    }
        
        public PhoneNumber AuthorizationPhone
        {
            get
            {
                return i_AuthorizationPhone;
            }
            set
            {
                Debug.Assert( value != null );
                this.SetAndTrack<PhoneNumber>( ref this.i_AuthorizationPhone, value, MethodBase.GetCurrentMethod() );
            }
        }

        public YesNotApplicableFlag AuthorizationRequired
        {
            get
            {

                Debug.Assert( this.i_AuthorizationRequired != null );
                return i_AuthorizationRequired;

            }
            set
            {

                Debug.Assert( value != null );
                this.SetAndTrack<YesNotApplicableFlag>( ref this.i_AuthorizationRequired, value, MethodBase.GetCurrentMethod() );

            }
        }

	    public AuthorizationStatus AuthorizationStatus
	    {
		    get 
            {
                Debug.Assert( this.i_AuthorizationStatus != null );
                return this.i_AuthorizationStatus;
            }
		    set 
            {

                Debug.Assert( value != null );
                this.SetAndTrack<AuthorizationStatus>(ref this.i_AuthorizationStatus, value, MethodBase.GetCurrentMethod() );
            }
	    }

	    public DateTime EffectiveDate
	    {

		    get 
            { 

                return i_EffectiveDate;

            }
		    set 
            {

                this.SetAndTrack<DateTime>( ref this.i_EffectiveDate, value, MethodBase.GetCurrentMethod() );

            }
	    }

	    public DateTime ExpirationDate
	    {
		    get 
            { 

                return i_ExpirationDate;
            
            }
		    set 
            {

                this.SetAndTrack<DateTime>( ref this.i_ExpirationDate, value, MethodBase.GetCurrentMethod() );

            }
	    }

	    public Name NameOfCompanyRepresentative
	    {
		    get 
            {

                Debug.Assert( this.i_ExpirationDate != null );
                return i_NameOfCompanyRepresentative;

            }
		    set 
            {
                Debug.Assert( value != null );
                this.SetAndTrack<Name>( ref this.i_NameOfCompanyRepresentative, value, MethodBase.GetCurrentMethod() );
            }
	    }

        public int NumberOfDaysAuthorized
	    {
		    get 
            { 

                return i_NumberOfDaysAuthorized;

            }
		    set 
            {

                this.SetAndTrack<int>( ref this.i_NumberOfDaysAuthorized, value, MethodBase.GetCurrentMethod() );

            }
	    }

        public string PromptExt
        {
            get
            {
                return i_PromptExt;
            }
            set
            {
                i_PromptExt = value;
            }
        }

	    public string Remarks
	    {

		    get 
            { 

                return i_Remarks;

            }
		    set 
            {
                
                Debug.Assert( value != null );
                if( !string.IsNullOrEmpty( value ) ) value = value.Trim();

                this.SetAndTrack<string>( ref this.i_Remarks, value, MethodBase.GetCurrentMethod() );

            }
	    }

	    public string ServicesAuthorized
	    {

		    get 
            { 
                return i_ServicesAuthorized;
            }
		    set 
            {

                Debug.Assert( value != null );
                if( !string.IsNullOrEmpty( value ) ) value = value.Trim();

                this.SetAndTrack<string>( ref this.i_ServicesAuthorized, value, MethodBase.GetCurrentMethod() );

            }
	    }

		#endregion Properties 

		#region Methods 

        public override object Clone()
        {
            Authorization authorization = new Authorization();
            authorization.AuthorizationNumber = this.AuthorizationNumber;
            authorization.EffectiveDate = this.EffectiveDate;
            authorization.ExpirationDate = this.ExpirationDate;
            if( this.NameOfCompanyRepresentative != null )
            {
                authorization.NameOfCompanyRepresentative = new
                    Name( this.NameOfCompanyRepresentative.FirstName,
                    this.NameOfCompanyRepresentative.LastName, String.Empty );
            }
            authorization.NumberOfDaysAuthorized = this.NumberOfDaysAuthorized;
            authorization.Remarks = this.Remarks;
            authorization.ServicesAuthorized = this.ServicesAuthorized;
            authorization.AuthorizationStatus = this.AuthorizationStatus;
            return authorization;
        }

		#endregion Methods 

    }//class

}//namespace
