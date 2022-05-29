using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for YesEvent.
    /// </summary>
    public class YesEvent : StateEvent
    {
        #region Construction
        public YesEvent()
        {
        }

        public YesEvent( int eventToken ) : base( eventToken )
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Used during initialization of the FSMState Machine to build the event list
        /// </summary>
        /// <returns>StateEvent</returns>
        public static StateEvent GetExemplar()
        {
            if( evtExemplar == null )
            {
                evtExemplar = new YesEvent();
            }
            return evtExemplar;
        }

        /// <summary>
        /// Determines a valid stimulus for transition from this state.
        /// </summary>
        /// <returns>bool</returns>
        public override bool IsWrapperFor( int eventToken )
        {
            return eventToken.Equals( MSPEventCode.YesStimulus() );
        }

        public override FSMState Trigger( FSMState state, int eventToken, MSPDialog form,
                                          Account account, MedicareSecondaryPayor msp )
        {
            return state.Transition( new YesEvent( eventToken ), form, account, msp );
        }
        #endregion

        #region Data Elements
        private static YesEvent     evtExemplar;
        #endregion
    }
}
