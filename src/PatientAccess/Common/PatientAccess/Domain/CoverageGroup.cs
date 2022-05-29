using System;
using System.Diagnostics;
using PatientAccess.Annotations;

namespace PatientAccess.Domain
{
    [Serializable]
    [UsedImplicitly]
    public abstract class CoverageGroup : Coverage
    {

		#region Fields 

        private Authorization i_Authorization = new Authorization();

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverageGroup"/> class.
        /// </summary>
        public CoverageGroup(): base() {}

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets or sets the authorization.
        /// </summary>
        /// <value>The authorization.</value>
        public Authorization Authorization
        {

            get
            {

                return this.i_Authorization;

            }
            set
            {

                Debug.Assert( value != null );
                Authorization oldValue = this.i_Authorization;

                oldValue.ChangedListeners -= RaiseChangedEvent;

                i_Authorization = value ?? new Authorization();

                this.i_Authorization.ChangedListeners += 
                    new Changed( this.RaiseChainedChangedEvent );

                if( this.i_Authorization != oldValue  )
                    this.RaiseChangedEvent( "Authorization", oldValue, value );

                

            }

        }

		#endregion Properties 

		#region Methods 


        public override void RemoveAuthorization()
        {
            base.RemoveAuthorization();
            this.Authorization = new Authorization();
            this.ForceUnChangedStatusFor( "Authorization" );
        }

        protected virtual bool HaveAuthorizationFieldsChanged
        {
            get
            {
                return this.Authorization.HasChangedFor(AUTHORIZATION_COMPANY) ||
                       this.Authorization.HasChangedFor(AUTHORIZATION_NUMBER) ||
                       this.Authorization.HasChangedFor(AUTHORIZATION_STATUS) ||
                       this.Authorization.HasChangedFor(EFFECTIVE_DATE) ||
                       this.Authorization.HasChangedFor(EXPIRATION_DATE) ||
                       this.Authorization.HasChangedFor(NAME_OF_COMPANY_REPRESENTATIVE) ||
                       this.Authorization.HasChangedFor(NUMBER_OF_DAYS_AUTHORIZED) ||
                       this.Authorization.HasChangedFor(REMARKS) ||
                       this.Authorization.HasChangedFor(SERVICESAUTHORIZED);
            }
        }

        #endregion Methods 

    }

}

