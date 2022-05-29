using System;

namespace PatientAccess.Domain.Parties
{
    [Serializable]
    public class RelationshipType : CodedReferenceValue
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
        public RelationshipType()
        {
        }
        public RelationshipType( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public RelationshipType( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants

        public static readonly string RELATIONSHIPTYPE_EMPTY = string.Empty;

        public const string RELATIONSHIPTYPE_BLANK        = "  ",
            RELATIONSHIP_CODE_SELF                        = "01",
            RELATIONSHIPTYPE_SPOUSE                       = "02",
            RELATIONSHIPTYPE_NATURAL_CHILD                = "03",
            RELATIONSHIPTYPE_NATURAL_CHILD_NO_FINANCIA    = "04",
            RELATIONSHIPTYPE_EMPLOYEE                     = "08",
            RELATIONSHIPTYPE_UNKNOWN                      = "09",
            RELATIONSHIPTYPE_ORGANDONOR                   = "11",
            RELATIONSHIPTYPE_CADAVAR                      = "12",
            RELATIONSHIPTYPE_INJUREDPLAINTIFF             = "15",
            RELATIONSHIPTYPE_MOTHER                       = "49";

        #endregion
    }
}
