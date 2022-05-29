using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class UnemployedType : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods

        public static UnemployedType NewNeverEmployedType()
        {
            return new UnemployedType( NEVER_EMPLOYED_OID, "NEVER EMPLOYED" );
        }

        public static UnemployedType NewRetiredType()
        {
            return new UnemployedType( RETIRED_OID, "RETIRED" );
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public UnemployedType() { }
       
        private UnemployedType( long oid, string description ) :
            base( oid, description ) { }
       
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const long
            NEVER_EMPLOYED_OID      = 0,
            RETIRED_OID             = 1;

        #endregion
    }
}
