using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Language : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public bool IsOtherLanguage()
        {
            return Description.Trim().Equals( OTHER_LANGUAGE );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Language()
        {
        }
        public Language( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Language( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        #endregion
        #region Language Constants
        public const string OTHER_LANGUAGE = "OTHER";
        public const string OTHER_LANGUAGE_CODE = "OT";
        #endregion
    }
}