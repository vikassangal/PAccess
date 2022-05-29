using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for TransactionUpdateException.
	/// </summary>
	[Serializable()]
	public class TransactionUpdateException : EnterpriseException
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
        public TransactionUpdateException()
        {
        }

        public TransactionUpdateException(string aMessage)
            : base(aMessage)
        {
        }

        public TransactionUpdateException (string msg,Exception ex)
            : base(msg,ex)
        {
        }

        public TransactionUpdateException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }

        public TransactionUpdateException( string message, 
            Severity severity )
            : base( message, severity )
        {
        }

        public TransactionUpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
	}
}
