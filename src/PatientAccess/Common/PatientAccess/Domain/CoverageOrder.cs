using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class CoverageOrder : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods

        public static CoverageOrder NewPrimaryCoverageOrder()
        {
            CoverageOrder primary = new CoverageOrder( PRIMARY_OID, PRIMARY_DESCRIPTION );

            return primary;
        }

        public static CoverageOrder NewSecondaryCoverageOrder()
        {
            CoverageOrder secondary = new CoverageOrder( SECONDARY_OID, SECONDARY_DESCRIPTION );

            return secondary;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public CoverageOrder() { }

        public CoverageOrder( long oid, string description ) : 
            base( oid, NEW_VERSION, description ){}
        
        public CoverageOrder( long oid, DateTime version, string description ) :
            base( oid, version, description ) {}

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        public const long PRIMARY_OID   = 1;
        public const long SECONDARY_OID = 2;
        public const string PRIMARY_DESCRIPTION = "Primary";
        private const string SECONDARY_DESCRIPTION = "Secondary";
        #endregion
    }
}