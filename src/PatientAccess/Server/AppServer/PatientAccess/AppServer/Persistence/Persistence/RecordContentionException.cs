using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace Extensions.Persistence
{
    [Serializable]
    public class RecordContentionException : EnterpriseException
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// Property allocated to return an updated version of the object that caused the
        /// concurrency and contention issue.  If this property is not set, null will be
        /// returned.
        /// </summary>
        private object UpdatedObject
        {
            get
            {
                return i_UpdatedObject;
            }
            set
            {
                i_UpdatedObject = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public RecordContentionException( string message, 
                                          Exception innerException,
                                          Severity severity )
            : base( message, innerException, severity )
        {
        }

        public RecordContentionException( string message, Severity severity, object updatedValue )
            : base( message, severity )
        {
            this.UpdatedObject = updatedValue;
        }

        public RecordContentionException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
        #endregion

        #region Data Elements
        private object i_UpdatedObject;
        #endregion
    }
}
