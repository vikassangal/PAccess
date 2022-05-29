using System;
using Peradigm.Framework.Domain.Exceptions;

namespace Peradigm.Framework.Domain.Time.Exceptions
{
//TODO: Create XML summary comment for InvalidParameterException
    [Serializable]
    public class InvalidParameterException : EnterpriseException
    {
        #region Constants
        private const string
            DEFAULT_MESSAGE = "Expected parameter of type {0} but was of type {1}.",
            CONTEXT_EXPECTED_PARAMETER = "ExpectedParameter.Type",
            CONTEXT_ACTUAL_PARAMETER = "ActualParameter.Type";
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
        public InvalidParameterException( object expected, object actual )
            : this( expected, actual, null )
        {
        }

        private InvalidParameterException( object expected, object actual, Exception innerException )
            : this( expected, actual, innerException, Severity.Low )
        {
        }

        public InvalidParameterException( object expected, object actual, Severity severity )
            : this( expected, actual, null, severity )
        {
        }
        		
        private InvalidParameterException( object expected, object actual, Exception innerException, Severity severity )
            : base( String.Format( DEFAULT_MESSAGE, expected.GetType().ToString(), actual.GetType().ToString() ), innerException, severity )
        {    			
            this.AddContextItem( CONTEXT_EXPECTED_PARAMETER, expected.GetType().ToString() );
            this.AddContextItem( CONTEXT_ACTUAL_PARAMETER, actual.GetType().ToString() );
        }
        #endregion

        #region Data Elements
        #endregion
    }
}