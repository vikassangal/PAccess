using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class Pregnancy : Condition 
    {
        #region Event Handlers
        #endregion

        #region Methods
      
        #endregion

        #region Properties
        /// <summary>
        ///  Overridden from its Base class Condition. returns the <see cref="OccurrenceCode"/> for Pregnancy. 
        /// Called by <see cref="OccurrenceCodeManager"/> to GenerateOccurrenceCode for Pregnancy.
        /// </summary>
        public override string OccurrenceCodeStr
        {
            get
            {
                return OccurrenceCode.OCCURRENCECODE_LASTMENSTRUATION ;
            }
        }
        /// <summary>
        /// Date of Last menstration Occured.
        /// </summary>
        public DateTime LastPeriod 
        {
            get
            {
                return i_LastPeriod;
            }
            set
            {
                i_LastPeriod = value;
                this.CallOccurrenceCodeManager();
            }
        }
        public override DateTime OccurredOn 
        {
            get
            {
                return this.LastPeriod;
            }
            set
            {
                this.LastPeriod = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Pregnancy()
        {

        }
        #endregion

        #region Data Elements
        private DateTime i_LastPeriod;

        #endregion

        #region Constants
        #endregion
    }
}



