using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for ConditionOfService.
    /// </summary>
    //TODO: Create XML summary comment for ConditionOfService
    [Serializable]
    public class ConditionOfService : CodedReferenceValue
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
        public ConditionOfService( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public ConditionOfService( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        public ConditionOfService()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        public static readonly string BLANK = string.Empty;

        public const string YES = "Y",
                            UNABLE = "U",
                            REFUSED = "R",
                            NOT_AVAILABLE = "N",
                            YES_DESCRIPTION = "Yes",
                            UNABLE_DESCRIPTION = "No, Patient Medically Unable to Sign",
                            REFUSED_DESCRIPTION = "No, Patient Refused to Sign",
                            NOT_AVAILABLE_DESCRIPTION = "No, Patient Not Available to Sign";
        #endregion

        public bool IsYes
        {
            get { return Code == YES; }
        }
        public bool IsRefused
        {
            get { return Code == REFUSED; }
        }
        public bool IsUnable
        {
            get { return Code == UNABLE; }
        }
        public bool IsNotAvailable
        {
            get { return Code == NOT_AVAILABLE; }
        }
    }
}
