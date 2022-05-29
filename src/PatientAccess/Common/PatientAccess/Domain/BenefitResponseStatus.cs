using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class BenefitResponseStatus : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public static BenefitResponseStatus NewUnknownStatus()
        {
            return new BenefitResponseStatus( UNKNOWN_OID, "UNKNOWN" );
        }

        public static BenefitResponseStatus NewAcceptedStatus()
        {
            return new BenefitResponseStatus( ACCEPTED_OID, "ACCEPTED" );
        }

        public static BenefitResponseStatus NewRejectedStatus()
        {
            return new BenefitResponseStatus( REJECTED_OID, "REJECTED" );
        }

        public static BenefitResponseStatus NewDeferredStatus()
        {
            return new BenefitResponseStatus( DEFERRED_OID, "DEFERRED" );
        }

        public static BenefitResponseStatus NewAutoAcceptedStatus()
        {
            return new BenefitResponseStatus( AUTO_ACCEPTED_OID, "AUTO ACCEPTED" );
        }


        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public BenefitResponseStatus( long oid, string description )
            : base( oid, description )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        public const long            
            UNKNOWN_OID = 0,
            ACCEPTED_OID = 1,           
            REJECTED_OID = 2,
            DEFERRED_OID = 3,
            AUTO_ACCEPTED_OID = 4;

        #endregion
    }
}
