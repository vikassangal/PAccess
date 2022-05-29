using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class TypeOfContactPoint : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        
        public static TypeOfContactPoint NewMailingContactPointType()
        {
            return new TypeOfContactPoint( MAILING_OID, "MAILING" );
        }

        public static TypeOfContactPoint NewEmployerContactPointType()
        {
            return new TypeOfContactPoint( EMPLOYER_OID, "EMPLOYER" );
        }

        public static TypeOfContactPoint NewMobileContactPointType()
        {
            return new TypeOfContactPoint( MOBILE_OID, "CELL" );
        }

        public static TypeOfContactPoint NewBillingContactPointType()
        {
            return new TypeOfContactPoint( BILLING_OID, "BILLING" );
        }

        public static TypeOfContactPoint NewBusinessContactPointType()
        {
            return new TypeOfContactPoint( BUSINESS_OID, "BUSINESS" );
        }

        public static TypeOfContactPoint NewPhysicalContactPointType()
        {
            return new TypeOfContactPoint( PHYSICAL_OID, "PHYSICAL" );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public TypeOfContactPoint() { }

        public TypeOfContactPoint( long oid, string description ) :
            base( oid, description ){}

        #endregion

        #region Data Elements
        #endregion

        #region Constants

        public const long
            //HOME_OID    = 0,    // DEPRICATED
            MAILING_OID = 0,
            //WORK_OID    = 1,    // DEPRICATED
            EMPLOYER_OID = 1,
            MOBILE_OID  = 2,
            BILLING_OID = 3,
            FAX_OID     = 4;

        private const long
            //HOME_OID    = 0,    // DEPRICATED
            BUSINESS_OID = 5;

        public const long
            //HOME_OID    = 0,    // DEPRICATED
            PHYSICAL_OID = 6;

        public const string PHYSICAL_CONTACTPOINT_DESCRIPTION = "PHYSICAL";
        public const string MAILING_CONTACTPOINT_DESCRIPTION = "MAILING";
        #endregion
    }
}
