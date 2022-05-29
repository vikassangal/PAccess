using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Persistence.Utilities
{
    /// <summary>
    /// Summary description for DateFormatException.
    /// </summary>
    [Serializable]
    public class DateFormatException : EnterpriseException
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
        public DateFormatException()
            : base()
    {
    }

        public DateFormatException( string message )
            : base( message )
    {
    }

        public DateFormatException( string message, Exception innerException )
            : base( message, innerException )
    {
    }

        public DateFormatException( string message, Severity severity )
            : base( message, severity )
    {
    }


        public DateFormatException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
    {
    }

        public DateFormatException( SerializationInfo info, StreamingContext context )
            : base( info, context )
    {
    }
        #endregion

        #region Data Elements
        #endregion
    }
}
