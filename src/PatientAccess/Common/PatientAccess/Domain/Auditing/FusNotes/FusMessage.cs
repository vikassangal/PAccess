using System;

namespace PatientAccess.Domain.Auditing.FusNotes
{
	/// <summary>
	/// Summary description for FusMessage.
	/// </summary>
	//TODO: Create XML summary comment for FusMessage
    [Serializable]
    public class FusMessage 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string Code
        {
            get
            {
                return i_Code;
            }
            set
            {
                i_Code = value;
            }
        }
        
        public string Message
        {
            get
            {
                return i_Message;
            }
            set
            {
                i_Message = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FusMessage()
        {
        }
        #endregion

        #region Data Elements
        private string i_Code;
        private string i_Message;
        #endregion

        #region Constants
        #endregion
    }
}
