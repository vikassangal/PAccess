using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for InsurancePlanTypes.
    /// </summary>
    //TODO: Create XML summary comment for InsurancePlanTypes
    [Serializable]
    public class InsurancePlanType : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsurancePlanType()
        {
        }
        public InsurancePlanType( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public InsurancePlanType( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        #endregion
    }
}
