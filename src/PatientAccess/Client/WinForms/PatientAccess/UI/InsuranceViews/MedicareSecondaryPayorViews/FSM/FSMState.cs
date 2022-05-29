using System;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for FSMState.
    /// </summary>
    public class FSMState
    {
        #region Construction
        public FSMState()
        {
        }

        public FSMState( FSMState state )
        {
            previousState = state;
        }
        #endregion

        #region Methods
        public virtual ControlView GetView()
        {
            return view;
        }

        public virtual FSMState GetPreviousState()
        {
            return previousState;
        }

        public virtual String GetStateString()
        {
            return String.Empty;
        }

        // Events that produce a state transition are define here
        // and overridden by the states that allow those events to
        // transition to another state.
        //
        // If IsValidStimulus() returns 'true' then the overridden method
        // for the valid event is invoked to do a transtion to the next state.

        public virtual FSMState Transition( YesEvent e, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            return this;
        }
        
        public virtual FSMState Transition( NoEvent e, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            return this;
        }

        public virtual FSMState Transition( ESRDEvent e, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            return this;
        }

        public virtual FSMState Transition( DisabilityEvent e, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            return this;
        }

        public virtual FSMState Transition( AgeEvent e, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            return this;
        }
        #endregion
        
        #region Properties
        public virtual bool FormWasChanged
        {
            get
            {
                return formWasChanged;
            }
            set
            {
                formWasChanged = value;
            }
        }

        // The overridden methods check for whatever data causes a state transition
        public virtual int Response
        {
            get
            {
                return -1;
            }
        }

        // Allow the UI to know when last state is active so button states may be set
        public bool IsEndState
        {
            get
            {
                return isEndState;
            }
            set
            {
                isEndState = value;
            }
        }

        // Allow the UI to know when first state is active so button states may be set
        public bool IsIdleState
        {
            get
            {
                return isIdleState;
            }
            set
            {
                isIdleState = value;
            }
        }
        #endregion

        #region Data Elements
        private ControlView view = null;
        protected FSMState  previousState;
        private bool        isIdleState;
        private bool        isEndState;
        private bool        formWasChanged;
        #endregion
    }
}
