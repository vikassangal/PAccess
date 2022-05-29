using System;

namespace PatientAccess.Domain
{
    //TODO: Create XML summary comment for TimePeriodFlag
    [Serializable]
    public class TimePeriodFlag : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public void SetYear()
        {
            base.Code = "Y";
            base.Description = "Year";
        }
        public void SetVisit()
        {
            base.Code = "V";
            base.Description = "Visit";
        }
        public void SetBlank()
        {
            base.Code = "B";
            base.Description = " ";
        }

        private void SetPeriod(string period)
        {
            switch (period.Trim())
            {
                case "Y":
                    this.SetYear();
                    break;
                case "V":
                    this.SetVisit();
                    break;
                case "B":
                case "":
                    this.SetBlank();
                    break;
                default: 
                    throw new ApplicationException("Invalid Time period detected: " + period);

            }

        }
        #endregion

        #region Properties
      
        #endregion

        #region Private Methods
      
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public TimePeriodFlag()
        {
        }
        public TimePeriodFlag(string period)
        {
            this.SetPeriod(period);
        }
     
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        
        #endregion
    }
}
