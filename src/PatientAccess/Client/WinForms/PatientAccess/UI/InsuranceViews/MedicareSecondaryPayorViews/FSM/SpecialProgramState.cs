using System;
using System.Drawing;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for SpecialProgramState.
    /// </summary>
    public class SpecialProgramState : FSMState
    {
        #region Construction
        public SpecialProgramState( MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            view = new SpecialProgramView( form );
            view.Location      = new Point( 1, 1 );
            view.Model         = msp;
            view.Model_Account = account;
            view.TabIndex      = 0;
            IsIdleState        = true;
        }

        public SpecialProgramState( FSMState state,  MSPDialog form, Account account,
                                    MedicareSecondaryPayor msp, bool tag ) : base( state )
        {
            view = new SpecialProgramView( form );
            view.Location = new Point( 1, 1 );
            view.Tag      = tag;
            view.TabIndex = 0;
            view.Model    = msp;
            view.Model_Account = account;
        }
        #endregion

        #region Methods
        public static SpecialProgramState GetInstance( FSMState state, MSPDialog form,
                                                       Account account, MedicareSecondaryPayor msp, bool dispose )
        {
            if( dispose || stateExemplar == null )
            {
                stateExemplar = null;
                stateExemplar = new SpecialProgramState( state, form, account, msp, dispose );
            }
            return stateExemplar;
        }

        public override ControlView GetView()
        {
            return view;
        }

        public override String GetStateString()
        {
            return strState;
        }

        public override FSMState Transition( YesEvent eventStimulus, MSPDialog form,
                                             Account account, MedicareSecondaryPayor msp )
        {
            if( View.FormChanged )
            {
                ControlView cv = MedicareEntitlementState.GetInstance( this, form, account, msp, false ).View;
                cv.Dispose();
            }
            MedicareEntitlementState nextState = MedicareEntitlementState.GetInstance( this, form, account, msp, View.FormChanged );

            if( View.FormChanged )
            {
                View.FormChanged = false;
                nextState.View.FormChanged = true;
            }
            nextState.View.UpdateView();
            return nextState;
        }

        public override FSMState Transition( NoEvent eventStimulus, MSPDialog form,
            Account account, MedicareSecondaryPayor msp )
        {
            if( View.FormChanged )
            {
                ControlView cv = LiabilityInsurerState.GetInstance( this, form, account, msp, false ).View;
                cv.Dispose();
            }
            LiabilityInsurerState nextState = LiabilityInsurerState.GetInstance( this, form, account, msp, View.FormChanged );

            if( View.FormChanged )
            {
                View.FormChanged = false;
                nextState.View.FormChanged = true;
            }

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

        public SpecialProgramView View
        {
            get
            {
                return view;
            }
        }

        public override int Response
        {
            get
            {
                return view.Response;
            }
        }
        #endregion

        #region Data Elements
        public static SpecialProgramView   view;
        public static SpecialProgramState  stateExemplar;
        private string                     strState = "Special Programs";
        #endregion
    }
}
