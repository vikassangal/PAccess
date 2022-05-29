using System;
using System.Collections;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace Extensions.DB2Persistence
{
    [Serializable]
    public class PersistenceException : EnterpriseException
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
        public PersistenceException( string message )
            : base( message )
        {
        }

        public PersistenceException( string message, Severity severity )
            : base( message, severity )
        {
        }

		public PersistenceException( string message, Severity severity, Hashtable context )
			: base( message, severity, context )
		{
		}

        public PersistenceException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        public PersistenceException( string message, 
                                     Exception innerException,
                                     Severity severity )
            : base( message, innerException, severity )
        {
        }

		public PersistenceException( string message, 
									Exception innerException, 
									Severity severity, 
									Hashtable context )
			: base( message, innerException, severity, context )
		{
		}

        protected PersistenceException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
