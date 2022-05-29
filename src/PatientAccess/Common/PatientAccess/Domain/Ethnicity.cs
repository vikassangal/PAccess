using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Ethnicity : CodedReferenceValue
    {
        #region Constants

        public const string ETHNICITY_PROPERTY = "ETHNICITY";
        public const string ETHNICITY2_PROPERTY = "ETHNICITY2";
        public const string ZERO_CODE = "00";
        #endregion

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
        public Ethnicity()
        {
        }
        public Ethnicity( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Ethnicity( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
         public string ParentEthnicityCode { get; set; }
        #endregion

        #region Data Elements        
        #endregion
    }
}