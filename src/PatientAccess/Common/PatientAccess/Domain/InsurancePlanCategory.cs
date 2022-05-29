using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for InsurancePlanCategory.
    /// </summary>
    [Serializable]
    public class InsurancePlanCategory : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string AssociatedIDLabel
        {
            get
            {
                return i_AssociatedIDLabel;
            }
            set
            {
                i_AssociatedIDLabel = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsurancePlanCategory()
        {
        }
        public InsurancePlanCategory( long oid, DateTime version, string description, string associatedIdLabel )
            : base( oid, version, description )
        {
            i_AssociatedIDLabel = associatedIdLabel;
        }

        public InsurancePlanCategory( long oid, DateTime version, string description, string code, string associatedIdLabel  )
            : base( oid, version, description, code )
        {
            i_AssociatedIDLabel = associatedIdLabel;
        }
        #endregion

        #region Data Elements
        private string i_AssociatedIDLabel = String.Empty;
        #endregion

        #region Constants
        public static long 
            GOVERNMENT_MEDICARE_OID = 3,
            GOVERNMENT_MEDICAID_OID = 4,
            SELFPAY_OID = 5;

        public const long 
            PLANCATEGORY_COMMERCIAL                 = 1,
            PLANCATEGORY_GOVERNMENT_OTHER           = 2,
            PLANCATEGORY_GOVERNMENT_MEDICARE        = 3,
            PLANCATEGORY_GOVERNMENT_MEDICAID        = 4,
            PLANCATEGORY_SELF_PAY                   = 5,
            PLANCATEGORY_WORKERS_COMPENSATION       = 6,
            PLANCATEGORY_OTHER                      = 7;

        public const long
            PBAR_PLANCATEGORY_GOVERNMENT_MEDICAID   = 1,
            PBAR_PLANCATEGORY_WORKERS_COMPENSATION  = 2,
            PBAR_PLANCATEGORY_GOVERNMENT_MEDICARE   = 3,
            PBAR_PLANCATEGORY_GOVERNMENT_OTHER      = 4,
            PBAR_PLANCATEGORY_SELF_PAY              = 5,
            PBAR_PLANCATEGORY_COMMERCIAL            = 6;

        #endregion
    }
}
