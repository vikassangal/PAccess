using System;
using System.Collections;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for AddressValidationResult.
	/// </summary>
	//TODO: Create XML summary comment for AddressValidationResult
    [Serializable]
    public class AddressValidationResult
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string ReturnMessage
        {
            get
            {
                return i_ReturnMessage;
            }
        }
        public string ReturnCode
        {
            get
            {
                return i_ReturnCode;
            }
        }
            public ArrayList Addresses
        {
            get
            {
                return i_Addresses;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AddressValidationResult(ArrayList Addresses, string returnCode, string returnMessage )
        {
            this.i_Addresses = Addresses;
            this.i_ReturnCode = returnCode;
            this.i_ReturnMessage = returnMessage;
        }
        #endregion

        #region Data Elements
        private ArrayList i_Addresses;
        private string i_ReturnCode;
        private string i_ReturnMessage;
        #endregion

        #region Constants
        #endregion
    }
}
