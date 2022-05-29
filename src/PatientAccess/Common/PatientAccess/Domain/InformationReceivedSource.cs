using System;

namespace PatientAccess.Domain
{
    //TODO: Create XML summary comment for InformationReceivedSource
    [Serializable]
    public class InformationReceivedSource : CodedReferenceValue
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
        public InformationReceivedSource()
        {
            this.Code = BLANK_VERIFICATION_CODE;
        }
        public InformationReceivedSource( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public InformationReceivedSource( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        public const int BLANK_VERIFICATION_OID = 1,
                         PHONE_VERIFICATION_OID = 2,
                         SYSTEM_VERIFICATION_OID = 3,
                         OTHER_VERIFICATION_OID = 4;

        private const string BLANK_VERIFICATION_CODE = "1";
        public const string PHONE_VERIFICATION_CODE = "2";
        public const string SYSTEM_VERIFICATION_CODE = "3";
        public const string OTHER_VERIFICATION_CODE = "4";



        #endregion
    }
}
