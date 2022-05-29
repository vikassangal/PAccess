using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for RightCareRightPlace.
    /// </summary>
    //TODO: Create XML summary comment for RightCareRightPlace
    [Serializable]
    public class RightCareRightPlace : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public LeftOrStayed LeftOrStayed
        {
            get
            {
                return i_LeftOrStayed;
            }
            set
            {
                i_LeftOrStayed = value;
            }
        }
       
        public YesNoFlag RCRP { get; set; }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties

        private LeftOrStayed i_LeftOrStayed = new LeftOrStayed();
        #endregion

        #region Construction and Finalization
        public RightCareRightPlace( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
            RCRP = YesNoFlag.Blank;
        }

        public RightCareRightPlace( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
            RCRP = YesNoFlag.Blank;
        }
        public RightCareRightPlace()
        {
            RCRP = YesNoFlag.Blank;
        }
        #endregion
    }
}
