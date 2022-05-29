using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class SelectInsuranceArgs : EventArgs
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties

        public object SelectedPlan
        {
            get
            {
                return i_SelectedPlan;
            }
            set
            {
                i_SelectedPlan = value;
            }
        }
        public object SelectedEmployer
        {
            get
            {
                return i_SelectedEmployer;
            }
            set
            {
                i_SelectedEmployer = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SelectInsuranceArgs( object selectedPlan, object selectedEmployer ) : base()
        {
            i_SelectedPlan      = selectedPlan;
            i_SelectedEmployer  = selectedEmployer;
        }
        public SelectInsuranceArgs():base()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants

        private object i_SelectedPlan;
        private object i_SelectedEmployer;

        #endregion
    }
}
