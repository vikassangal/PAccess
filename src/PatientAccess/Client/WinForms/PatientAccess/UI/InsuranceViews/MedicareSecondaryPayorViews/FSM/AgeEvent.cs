using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for AgeEvent.
    /// </summary>
    public class AgeEvent : StateEvent
    {
        #region Construction
        public AgeEvent()
        {
        }

        public AgeEvent( int eventToken ) : base( eventToken )
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
                evtExemplar = new AgeEvent();
            }
            return evtExemplar;
        }

        public override bool IsWrapperFor( int eventToken )
        {
            return eventToken.Equals( MSPEventCode.AgeStimulus() );
        }

        public override FSMState Trigger( FSMState state, int eventToken, MSPDialog form,
                                          Account account, MedicareSecondaryPayor msp )
        {
            return state.Transition( new AgeEvent( eventToken ), form, account, msp );
        }
        #endregion

        #region Data Elements
        private static AgeEvent  evtExemplar;
        #endregion
    }
}
