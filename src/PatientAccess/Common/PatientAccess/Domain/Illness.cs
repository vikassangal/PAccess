using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Illness : TimeAndLocationBoundCondition
    {
        #region Event Handlers
        #endregion

        #region Methods
    
        #endregion

        #region Properties
        /// Overridden from its Base class Condition. returns the <see cref="OccurrenceCode"/> for Illness. 
        /// Called by <see cref="OccurrenceCodeManager"/> to Generate <see cref="OccurrenceCode"/>.
        public override string OccurrenceCodeStr
        {
            get
            {
                return OccurrenceCode.OCCURRENCECODE_ILLNESS ;
            }
        }
        // Date when the Illness first started. 
        public DateTime Onset 
        {
            get
            {
                return i_Onset;
            }
            set
            {
                i_Onset = value;
                this.CallOccurrenceCodeManager();

            }
        }
        public override DateTime OccurredOn 
        {
            get
            {
                return this.Onset;
            }
            set
            {
                this.Onset = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Illness()
        {

        }
        #endregion

        #region Data Elements
        private DateTime i_Onset;
        #endregion

        #region Constants
        #endregion
    }
}


