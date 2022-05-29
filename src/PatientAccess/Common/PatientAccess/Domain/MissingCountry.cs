using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class UnknownCountry : Country
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
        public UnknownCountry( string code ) 
            : base( code )
        {
        }

        public UnknownCountry( string code, string description ) 
            : base( code, description )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
