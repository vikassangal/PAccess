using System;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    public abstract class SearchCriteria : object
    {
        #region Event Handlers
        #endregion

        #region Methods
        public abstract ValidationResult Validate();
        #endregion

        #region Properties
        public string HSPCode
        {
            get
            {
                return i_HSPCode;
            }
            set
            {
                i_HSPCode = value;
            }
        }

        public long HSPNumber
        {
            get
            {
                return i_HSPNumber;
            }
            set
            {
                i_HSPNumber = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SearchCriteria()
        {
        }
        
        public SearchCriteria(string HSPCode)
        {
            this.HSPCode = HSPCode;
        }

        public SearchCriteria(long HSPNumber)
        {
            this.HSPNumber = HSPNumber;
        }
        #endregion

        #region Data Elements
        private string i_HSPCode;
        private long i_HSPNumber;
        #endregion

        #region Constants
        #endregion
    }
}
