using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace Extensions.Configuration
{
	/// <summary>
	/// Summary description for MessageNotFoundException.
	/// </summary>
    [Serializable]
    public class MessageNotFoundException : EnterpriseException
    {
        #region Constants
        private const string 
            DEFAULT_MESSAGE = "A message with the specified key was not found in the message catalog.";
        #endregion

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
        public MessageNotFoundException()
            : this( DEFAULT_MESSAGE )
        {
        }

        private MessageNotFoundException( string message )
            : this( message, Severity.Low )
        {
        }

        private MessageNotFoundException( string message , 
                                         Severity severity )
            : this( message, null, severity )
        {
        }

        public MessageNotFoundException( string message,
                                         Exception innerException )
            : this( message, innerException, Severity.Low )
        {
        }

        private MessageNotFoundException( string message, 
                                         Exception innerException,
                                         Severity severity )
            : base( message, innerException )
        {
            this.Severity = severity;
        }

        public MessageNotFoundException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
