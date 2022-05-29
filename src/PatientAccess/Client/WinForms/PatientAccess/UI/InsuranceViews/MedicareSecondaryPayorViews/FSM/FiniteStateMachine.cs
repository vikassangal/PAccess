using System;
using System.Collections;
using System.Windows.Forms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for FiniteStateMachine.
    /// </summary>
    public class FiniteStateMachine
    {
        #region Construction
        public FiniteStateMachine( MSPDialog mainForm )
        {
            parentForm = mainForm;
            target = new Target();
            eventList = new ArrayList();

            // Build the event list the machine uses
            AddEvent( AgeEvent.GetExemplar() );
            AddEvent( DisabilityEvent.GetExemplar() );
            AddEvent( ESRDEvent.GetExemplar() );
            AddEvent( NoEvent.GetExemplar() );
            AddEvent( YesEvent.GetExemplar() );
        }
        #endregion

        #region Methods
        /// <summary>
        /// Builds the list of Event objects used in state transitions by ConsumeEvent.
        /// </summary>
        private void AddEvent( StateEvent e )
        {
            if( eventList.Contains( e ) == false )
            {
                eventList.Add( e );
            }
        }

        /// <summary>
        /// Takes an event stimulus and moves the state machine to the next state.
        /// </summary>
        public void ConsumeEvent( int stimulus )
        {
            foreach( StateEvent anEvent in eventList )
            {
                if( anEvent.IsWrapperFor( stimulus ) )
                {
                    FSMState state = target.GetState();

                    if( state != null )
                    {
                        // Do a state transition and set the target to the new state
                        target.SetState( anEvent.Trigger( state, stimulus, parentForm, account, medPayor ) );
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Destroys the views and sets the static instance to null
        /// </summary>
        public void Dispose()
        {
            if( WelcomeScreenState.view != null )
            {
                WelcomeScreenView view = WelcomeScreenState.view;
                view.Dispose();
                WelcomeScreenState.stateExemplar = null;
            }
            if( SummaryPageState.view != null )
            {
                SummaryPageView view = SummaryPageState.view;
                view.Dispose();
                SummaryPageState.stateExemplar = null;
            }
            if( SpecialProgramState.view != null )
            {
                SpecialProgramView view = SpecialProgramState.view;
                view.Dispose();
                SpecialProgramState.stateExemplar = null;
            }
            if( MedicareEntitlementState.view != null )
            {
                MedicareEntitlementView view = MedicareEntitlementState.view;
                view.Dispose();
                MedicareEntitlementState.stateExemplar = null;
            }
            if( LiabilityInsurerState.view != null )
            {
                LiabilityInsurerView view = LiabilityInsurerState.view;
                view.Dispose();
                LiabilityInsurerState.stateExemplar = null;
            }
            if( ESRDEntitlementPage1State.view != null )
            {
                ESRDEntitlementPage1View view = ESRDEntitlementPage1State.view;
                view.Dispose();
                ESRDEntitlementPage1State.stateExemplar = null;
            }
            if( ESRDEntitlementPage2State.view != null )
            {
                ESRDEntitlementPage2View view = ESRDEntitlementPage2State.view;
                view.Dispose();
                ESRDEntitlementPage2State.stateExemplar = null;
            }
            if( DisabilityEntitlementPage1State.view != null )
            {
                DisabilityEntitlementPage1View view = DisabilityEntitlementPage1State.view;
                view.Dispose();
                DisabilityEntitlementPage1State.stateExemplar = null;
            }
            if( DisabilityEntitlementPage2State.view != null )
            {
                DisabilityEntitlementPage2View view = DisabilityEntitlementPage2State.view;
                view.Dispose();
                DisabilityEntitlementPage2State.stateExemplar = null;
            }
            if( DisabilityEntitlementPage3State.view != null )
            {
                DisabilityEntitlementPage3View view = DisabilityEntitlementPage3State.view;
                view.Dispose();
                DisabilityEntitlementPage3State.stateExemplar = null;
            }
            if( AgeEntitlementPage1State.view != null )
            {
                AgeEntitlementPage1View view = AgeEntitlementPage1State.view;
                view.Dispose();
                AgeEntitlementPage1State.stateExemplar = null;
            }
            if( AgeEntitlementPage2State.view != null )
            {
                AgeEntitlementPage2View view = AgeEntitlementPage2State.view;
                view.Dispose();
                AgeEntitlementPage2State.stateExemplar = null;
            }
        }

        /// <summary>
        /// Returns the target's previous state
        /// </summary>
        public FSMState GetPreviousState()
        {
            return target.GetState().GetPreviousState();
        }

        /// <summary>
        /// Returns the target's current state
        /// </summary>
        public FSMState GetState()
        {
            return target.GetState();
        }

        /// <summary>
        /// Returns the current state's UI parentForm object.
        /// </summary>
        public UserControl GetStateView()
        {
            return target.GetState().GetView();
        }

        /// <summary>
        /// This string is used to find the LinkLabel to display at top of
        /// dialog when a state transition occurs.
        /// </summary>
        public virtual String GetStateString()
        {
            return target.GetState().GetStateString();
        }

        /// <summary>
        /// Sets starting point of the state machine.
        /// </summary>
        public void SetIdleState( bool showWelcomeScreen, Account account, MedicareSecondaryPayor msp )
        {
            this.account = account;
            medPayor     = msp;

            if( showWelcomeScreen )
            {
                target.SetState( new WelcomeScreenState( parentForm ), false );
                WelcomeScreenState.view.UpdateView();
            }
            else
            {   // It's necessary to set the static state instance here since the state's 
                // GetInstance method is not called when this state is the starting point.
                SpecialProgramState.stateExemplar = new SpecialProgramState( parentForm, account, medPayor );
                target.SetState( SpecialProgramState.stateExemplar, false );
                SpecialProgramState.view.UpdateView();
            }
        }

        public void SetSummaryState( Account account )
        {
            this.account = account;
            medPayor     = null;
            SummaryPageState.GetInstance( null, parentForm, account, null, false );
            target.SetState( SummaryPageState.stateExemplar, false );
        }
        /// <summary>
        /// This is used to set the state on a Back button click or clicking
        /// on a LinkLabel displayed at the top of the dialog.  The boolean
        /// flag when true, tells the state machine to instantiate a new state
        /// object if the user makes a change to the parentForm of the state we set here.
        /// </summary>
        public void SetState( FSMState state, bool formTagValue )
        {
            target.SetState( state, formTagValue );
        }
        #endregion

        #region Data Elements
        private ArrayList               eventList;
        private Account                 account;
        private MedicareSecondaryPayor  medPayor;
        private MSPDialog               parentForm;
        private Target                  target;
        #endregion
    }
}
