using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace Extensions.OrganizationalService
{
    [Serializable]
    public class HierarchyUnavailableException : EnterpriseException
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
        public HierarchyUnavailableException( SerializationInfo info, StreamingContext context ) 
            : base( info, context )
        {
        }
		
        public HierarchyUnavailableException( String message, Exception innerException, Severity severity ) 
            : base( message, innerException, severity )
        {
        }

        public HierarchyUnavailableException( String message , Exception innerException )
            : base( message, innerException )
        {
        }

        public HierarchyUnavailableException( String message, Severity severity )
            : base( message, severity )
        {
        }

        public HierarchyUnavailableException( String message )
            : base( message )
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
