namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Base class of objects that are transitioned by the Finite FSMState Machine
    /// </summary>
    public class Target
    {
        #region Construction
        public Target()
        {
        }

        public FSMState GetState()
        {
            return state;
        }
        #endregion

        #region Methods
        public void SetState( FSMState state, bool formTagValue )
        {
            this.state = state;
            state.GetView().Tag = formTagValue;
        }

        public void SetState( FSMState state )
        {
            this.state = state;
            state.GetView().Tag = false;
        }
        #endregion

        #region Data Elements
        private FSMState  state;
        #endregion
    }
}
