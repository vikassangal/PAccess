using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for FinancialClassType.
    /// </summary>
    //TODO: Create XML summary comment for FinancialClassType
    [Serializable]
    public class FinancialClassType : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods

        public static FinancialClassType NewStandardFCType()
        {
            return new FinancialClassType( STANDARD_OID, "Standard" );
        }

        public static FinancialClassType NewUninsuredFCType()
        {
            return new FinancialClassType( UNINSURED_OID, "Uninsured" );
        }

        public override string ToString()
        {   
            return String.Format("{0} {1}", Code, Description);
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FinancialClassType()
        {
        }

        private FinancialClassType( long oid, string description )
            : base( oid, description, oid.ToString(), DateTime.MinValue )
        {
        }

        public FinancialClassType( long oid, DateTime version, string description )
            : base( oid, version, description, oid.ToString() )
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private const long
            STANDARD_OID = 1,
            UNINSURED_OID  = 2;

        #endregion
    }
}
