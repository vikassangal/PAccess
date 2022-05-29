using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Race : CodedReferenceValue
    {

        #region Constants

        public const string RACENATIONALITY_CONTROL = "RACENATIONALITY";
        public const string RACENATIONALITY2_CONTROL = "RACENATIONALITY2";
        public const string RACE_OTHER = "OTHER";
        public const string RACE_DECLINED = "DECLINED";
        public const string RACE_UNKNOWN = "UNKNOWN";
        public const string ZERO_CODE = "00";

        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties

        public string ParentRaceCode { get; set; }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Race()
        {
        }
        public Race( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Race( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements        
        #endregion
    }
}