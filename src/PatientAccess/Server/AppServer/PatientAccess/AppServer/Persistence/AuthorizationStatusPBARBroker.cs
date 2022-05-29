using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    [Serializable]
    public class AuthorizationStatusPBARBroker : MarshalByRefObject, IAuthorizationStatusBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        public ICollection AllAuthorizationStatuses()
        {
            return new ArrayList( authStatArray ) ;
        }

        public AuthorizationStatus AuthorizationStatusWith( string code )
        {
            if ( null == code )
            {
                throw new ArgumentNullException( "Parameter, code, should not be null." ) ;
            }
            AuthorizationStatus result = null ;
            foreach ( AuthorizationStatus authStatus in authStatArray )
            {
                if ( authStatus.Code.Equals( code ) )
                {
                    result = authStatus;
                    break;
                }
            }
            return result ;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AuthorizationStatusPBARBroker()
            : base()
        {
        }

        #endregion

        #region Data Elements

        private static readonly AuthorizationStatus[] authStatArray = new AuthorizationStatus[] { 
                new AuthorizationStatus( 4L, " ", " " ),
                new AuthorizationStatus( 1L, "APPROVED", "A" ),
                new AuthorizationStatus( 2L, "DENIED", "D" ),
                new AuthorizationStatus( 3L, "PENDING", "P" )
            };

        #endregion

    }
}
