using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for ESRDEvent.
    /// </summary>
    public class ESRDEvent : StateEvent
    {
        #region Construction
        public ESRDEvent()
        {
        }

        public ESRDEvent( int eventToken ) : base( eventToken )
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
                evtExemplar = new ESRDEvent();
            }
            return evtExemplar;
        }

        public override bool IsWrapperFor( int eventToken )
        {
            return eventToken.Equals( MSPEventCode.ESRDStimulus() );
        }

        public override FSMState Trigger( FSMState state, int eventToken, MSPDialog form,
                                          Account account, MedicareSecondaryPayor msp )
        {
            return state.Transition( new ESRDEvent( eventToken ), form, account, msp );
        }
        #endregion

        #region Data Elements
        private static ESRDEvent    evtExemplar;
        #endregion
    }
}
