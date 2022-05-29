using System;
using System.Collections;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace Extensions.Persistence
{
    [Serializable]
    public class DatabaseServiceException : PersistenceException
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
        public DatabaseServiceException( string message )
            : base( message )
        {
        }

        public DatabaseServiceException( string message, Severity severity )
            : base( message, severity )
        {
        }

        public DatabaseServiceException( string message, Severity severity, Hashtable context )
            : base( message, severity, context )
        {
        }

        public DatabaseServiceException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        public DatabaseServiceException( string message, 
                                         Exception innerException,
                                         Severity severity )
            : base( message, innerException, severity )
        {
        }

        public DatabaseServiceException( string message, 
                                         Exception innerException, 
                                         Severity severity, 
                                         Hashtable context )
            : base( message, innerException, severity, context )
        {
        }

        public DatabaseServiceException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
