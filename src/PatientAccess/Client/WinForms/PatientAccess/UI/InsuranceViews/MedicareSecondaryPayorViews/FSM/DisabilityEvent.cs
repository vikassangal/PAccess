using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for DisabilityEvent.
    /// </summary>
    public class DisabilityEvent : StateEvent
    {
        #region Construction
        public DisabilityEvent()
        {
        }

        public DisabilityEvent( int eventToken ) : base( eventToken )
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
                evtExemplar = new DisabilityEvent();
            }
            return evtExemplar;
        }
        
        public override bool IsWrapperFor( int eventToken )
        {
            return eventToken.Equals( MSPEventCode.DisabilityStimulus() );
        }

        public override FSMState Trigger( FSMState state, int eventToken, MSPDialog form,
                                          Account account, MedicareSecondaryPayor msp )
        {
            return state.Transition( new DisabilityEvent( eventToken ), form, account, msp );
        }
        #endregion

        #region Data Elements
        private static DisabilityEvent  evtExemplar;
        #endregion
    }
}
