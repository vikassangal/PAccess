using System;

namespace PatientAccess.UI
{
    [Serializable]
    public class ReportArgs : EventArgs
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

        public object SearchCriteria
        {
            get
            {
                return i_SearchCriteria;
            }
            set
            {
                i_SearchCriteria = value;
            }
        }
        public object Summary
        {
            get
            {
                return i_Summary;
            }
            set
            {
                i_Summary = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReportArgs( object context, object searchCriteria, object summary ) : base()
        {
            i_Context = context;
            i_SearchCriteria = searchCriteria;
            i_Summary = summary;
        }
        public ReportArgs():base()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        private object i_Context;
        private object i_SearchCriteria;
        private object i_Summary;
        #endregion
    }
}
