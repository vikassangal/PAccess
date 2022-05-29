using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for NoEvent.
    /// </summary>
    public class NoEvent : StateEvent
    {
        #region Construction
        public NoEvent()
        {
        }

        public NoEvent( int eventToken ) : base( eventToken )
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Used during initialization of the FSMState Machine to build the event list
        /// </summary>
        /// <returns></returns>
        public static StateEvent GetExemplar()
        {
            if( evtExemplar == null )
            {
                evtExemplar = new NoEvent();
            }
            return evtExemplar;
        }

        public override bool IsWrapperFor( int eventToken )
        {
            return eventToken.Equals( MSPEventCode.NoStimulus() );
        }

        public override FSMState Trigger( FSMState state, int eventToken, MSPDialog form,
                                          Account account, MedicareSecondaryPayor msp )
        {
            return state.Transition( new NoEvent( eventToken ), form, account, msp );
        }
        #endregion

        #region Data Elements
        private static NoEvent  evtExemplar;
        #endregion
    }
}
