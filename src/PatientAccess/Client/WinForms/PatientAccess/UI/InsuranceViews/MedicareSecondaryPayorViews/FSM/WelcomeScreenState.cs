using System;
using System.Drawing;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for WelcomeScreenState.
    /// </summary>
    public class WelcomeScreenState : FSMState
    {
        #region Construction
        public WelcomeScreenState( MSPDialog form )
        {
            view = new WelcomeScreenView( form );
            view.Location = new Point( 1, 1 );
            view.TabIndex = 0;
            IsIdleState   = true;
        }
        #endregion

        #region Methods
        public override ControlView GetView()
        {
            return view;
        }

        public override FSMState GetPreviousState()
        {
            return previousState;
        }

        public override String GetStateString()
        {
            return strState;
        }

        public override int Response
        {
            get
            {
                return 1;
            }
        }

        public override FSMState Transition( YesEvent eventStimulus, MSPDialog form,
                                             Account account, MedicareSecondaryPayor msp )
        {
            SpecialProgramState nextState = SpecialProgramState.GetInstance( this, form, account, msp, false );
            nextState.View.UpdateView();
            return nextState;
        }
        #endregion

        #region Properties
        public override bool FormWasChanged
        {
            get
            {
                return view.FormChanged;
            }
            set
            {
                view.FormChanged = value;
            }
        }

        public WelcomeScreenView View
        {
            get
            {
                return view;
            }
        }
        #endregion

        #region Data Elements
        public static WelcomeScreenView  view;
        public static WelcomeScreenState stateExemplar;
        private string                   strState = "WelcomeScreenState";
        #endregion
    }
}
