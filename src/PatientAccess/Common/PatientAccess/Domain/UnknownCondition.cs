using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class UnknownCondition : Condition
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion
	
        #region Properties
        public override string OccurrenceCodeStr
        {
            get
            {
               return string.Empty;
            }
        }

		// Date when the Unkown condition first started. 
		public DateTime Onset 
		{
		    private get
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
        public UnknownCondition()
        {
            i_Onset = DateTime.MinValue;

        }
        #endregion

        #region Data Elements
		private DateTime i_Onset;
        #endregion

        #region Constants
        #endregion
    }
}

