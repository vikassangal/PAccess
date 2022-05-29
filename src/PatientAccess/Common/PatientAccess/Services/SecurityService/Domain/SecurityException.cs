using System;
using Peradigm.Framework.Domain.Exceptions;

namespace Extensions.SecurityService.Domain
{
    [Serializable]
    public class SecurityException : EnterpriseException
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
        public SecurityException()
            : base()
        {
        }
        public SecurityException( string message )
            : base( message )
        {
        }

        public SecurityException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        public SecurityException( string message, Severity severity )
            : base( message, severity )
        {
        }


        public SecurityException( string message, 
                                  Exception innerException,
                                  Severity severity )
           : base( message, innerException, severity )
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
