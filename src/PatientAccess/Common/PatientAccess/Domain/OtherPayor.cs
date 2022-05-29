using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class OtherPayor : Payor
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string NewPayorName
        {
            get
            {
                return i_NewPayorName;
            }
            set
            {
                i_NewPayorName = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OtherPayor()
        {
        }

        public OtherPayor( string newPayorName )
        {
            this.NewPayorName = newPayorName;
        }
        #endregion

        #region Data Elements
        private string i_NewPayorName = String.Empty;
        #endregion

        #region Constants
        #endregion
    }
}
