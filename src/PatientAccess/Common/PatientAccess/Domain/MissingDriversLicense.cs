using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class MissingDriversLicense : DriversLicense, INullable
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public override bool IsNull()
        {
            return true;
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        internal MissingDriversLicense()
            : base( string.Empty, new State() )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
