using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class SocialSecurityNumberStatus : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override string ToString()
        {
            return Description.ToUpper();
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Properties

        public bool IsUnknownSSNStatus
        {
            get { return Description.ToUpper() == UNKNOWN; }
        }

        public bool IsNoneSSNStatus
        {
            get { return Description.ToUpper() == NONE; }
        }

        public bool IsKnownSSNStatus
        {
            get { return Description.ToUpper() == KNOWN; }
        }

        public bool IsNewbornSSNStatus
        {
            get { return Description.ToUpper() == NEWBORN; }
        }

        public bool IsRefusedSSNStatus
        {
            get { return Description.ToUpper() == REFUSED; }
        }

        public static SocialSecurityNumberStatus KnownSSNStatus
        {
            get
            {
                return new SocialSecurityNumberStatus( 1, NEW_VERSION, KNOWN );
            }
        }

        public static SocialSecurityNumberStatus UnknownSSNStatus
        {
            get
            {
                return new SocialSecurityNumberStatus( 1, NEW_VERSION, UNKNOWN );
            }
        }

        public static SocialSecurityNumberStatus NoneSSNStatus
        {
            get
            {
                return new SocialSecurityNumberStatus( 1, NEW_VERSION, NONE );
            }
        }

        public static SocialSecurityNumberStatus NewbornSSNStatus
        {
            get
            {
                return new SocialSecurityNumberStatus( 1, NEW_VERSION, NEWBORN );
            }
        }
        public static SocialSecurityNumberStatus RefusedSSNStatus
        {
            get
            {
                return new SocialSecurityNumberStatus(1, NEW_VERSION, REFUSED);
            }
        }
        #endregion

        #region Construction and Finalization
        public SocialSecurityNumberStatus()
        {
        }
        public SocialSecurityNumberStatus( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        public const string
            KNOWN = "KNOWN",
            UNKNOWN = "UNKNOWN",
            NONE = "NONE",
            NEWBORN = "NEWBORN",
            REFUSED = "REFUSED";

        #endregion
    }
}