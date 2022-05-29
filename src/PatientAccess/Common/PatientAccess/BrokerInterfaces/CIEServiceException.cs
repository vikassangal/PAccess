using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for HDIServiceException.
	/// </summary>
	//TODO: Create XML summary comment for HDIServiceException
    [Serializable]
    public class HDIServiceException : EnterpriseException
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public HDIServiceException()
        {
        }
        public HDIServiceException(string aMessage)
            : base(CIEMESSAGE + aMessage)
        {
            
        }public HDIServiceException (string msg,Exception ex)
             : base(CIEMESSAGE + msg,ex)
        {
        }
        public HDIServiceException( string message, 
            Exception innerException,
            Severity severity )
            : base( CIEMESSAGE + message, innerException, severity )
        {
        }
        public HDIServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }
        #endregion

        #region Data Elements
       #endregion

        #region Constants
        private const string CIEMESSAGE = "CIE Service Exception: ";
        #endregion
    }
}
