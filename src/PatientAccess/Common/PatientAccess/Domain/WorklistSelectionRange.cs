using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for WorklistSelectionRange.
    /// </summary>
    //TODO: Create XML summary comment for WorklistSelectionRange
    [Serializable]
    public class WorklistSelectionRange : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        public override string ToString()
        {
            return Description;
        }

        public long RangeInDays
        {
            get
            {
                return i_RangeInDays;
            }
            set
            {
                i_RangeInDays = value;
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public WorklistSelectionRange() { }
        
        public WorklistSelectionRange( long rangeID, string description, long range ) :
            base( rangeID, description )
        {
            this.i_RangeInDays = range;
        }
        #endregion

        #region Data Elements
        private long i_RangeInDays;
        #endregion

        #region Constants
        public const int TODAY        = 1;
        public const int TOMORROW     = 2;
        public const int NEXT_3_DAYS  = 3;
        public const int NEXT_10_DAYS = 4;
        public const int LAST_3_DAYS  = 5;
        public const int LAST_10_DAYS = 6;
        public const int YESTERDAY    = 7;
        public const int ALL          = 8;
        public const int DATE_RANGE   = 9;
        #endregion
    }
}
