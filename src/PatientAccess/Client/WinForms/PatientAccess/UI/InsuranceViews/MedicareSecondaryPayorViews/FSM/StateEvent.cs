using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Base class for all event objects.
    /// </summary>
    public abstract class StateEvent
    {
        #region Construction
        public StateEvent()
        {
        }

        public StateEvent( int token )
        {
            eventToken = token;
        }
        #endregion

        #region Methods
        // Determines if the event token is a valid stimulus for a state transition
        public virtual bool IsWrapperFor( int e )
        {
            return true;
        }
        // Triggers a state transition
        public virtual FSMState Trigger( FSMState s, int stimulus, MSPDialog form,
                                         Account account, MedicareSecondaryPayor msp )
        {
            return null;
        }
        #endregion

        #region Data Elements
        private int eventToken;
        #endregion
    }
}
