using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Crime : TimeAndLocationBoundCondition
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// Overridden from its Base class Condition. returns the occurrence Code for Crime. 
        /// Called by <see cref="OccurrenceCodeManager"/> to Generate <see cref="OccurrenceCode"/> 
        public override string OccurrenceCodeStr
        {
            get
            {
                return OccurrenceCode.OCCURRENCECODE_CRIME ;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Crime()
        {
        }
        #endregion

        #region Data Elements
          
        #endregion

        #region Constants
        #endregion
    }
}



