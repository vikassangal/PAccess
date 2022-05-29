using System;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    public class ValidationResult : object
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public bool IsValid
        {
            get
            {
                return i_IsValid;
            }
        }

        public string Message
        {
            get
            {
                return i_Message;
            }
        }

        public string AspectInError
        {
            get
            {
                return i_AspectInError;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ValidationResult( bool state, string message, string aspectInError )
        {
            this.i_IsValid       = state;
            this.i_Message       = message;
            this.i_AspectInError = aspectInError;
        }

        public ValidationResult( bool state, string message )
            : this( state, message, String.Empty )
        {
        }

        public ValidationResult( bool state )
            : this( state, String.Empty, String.Empty )
        {
        }
        #endregion

        #region Data Elements
        private bool i_IsValid;
        private string i_Message;
        private string i_AspectInError;
        #endregion

        #region Constants
        #endregion
    }
}
