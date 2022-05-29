using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class CellPhoneConsent : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            if (Code == BLANK)
                return BLANK;
            return Code + EQUALS + Description;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
      
        public CellPhoneConsent( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        public CellPhoneConsent()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        public static readonly string BLANK = string.Empty;

        public const string NO_CONSENT = "N",
            WRITTEN_CONSENT = "W",
            DECLINE_CONSENT = "D",
            REVOKE_CONSENT = "R",
            VERBAL_CONSENT = "V",
            DELIVARY_CONSENT = "E",
            NO_CONSENT_DESCRIPTION = "PT Not Avail/CNST Not Obtained",
            WRITTEN_CONSENT_DESCRIPTION = "Written Consent/COS Signed",
            DECLINE_CONSENT_DESCRIPTION = "Decline Consent",
            REVOKE_CONSENT_DESCRIPTION = "Revoke Previous Consent",
            VERBAL_CONSENT_DESCRIPTION = "Verbal Consent",
            ELECTRONIC_CONSENT_DESCRIPTION = "Electronic Consent";


        private const string EQUALS = "=";

        #endregion
    }
}
