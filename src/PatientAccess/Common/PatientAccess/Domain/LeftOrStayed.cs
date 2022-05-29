using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for LeftOrStayed.
    /// </summary>
    //TODO: Create XML summary comment for LeftOrStayed
    [Serializable]
    public class LeftOrStayed : CodedReferenceValue
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
        public LeftOrStayed( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public LeftOrStayed( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        public LeftOrStayed()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        public static readonly string BLANK = string.Empty;

        public const string LEFT = "L",
                            STAYED = "S",
                            LEFT_DESCRIPTION = "Left",
                            STAYED_DESCRIPTION = "Stayed";
        #endregion
    }
}
