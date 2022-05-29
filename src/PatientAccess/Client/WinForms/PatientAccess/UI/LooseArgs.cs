using System;

namespace PatientAccess.UI
{
    [Serializable]
    public class LooseArgs : EventArgs
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public object Context
        {
            get
            {
                return i_Context;
            }
            set
            {
                i_Context = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LooseArgs( object context ) : base()
        {
            this.Context = context;
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        private object i_Context;
        #endregion
    }
}
