using System;
using System.Drawing;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for MedicareEntitlementState.
    /// </summary>
    public class MedicareEntitlementState : FSMState
    {
        #region Construction
        public MedicareEntitlementState( FSMState state, MSPDialog form, Account account,
            MedicareSecondaryPayor msp, bool tag ) : base( state )
        {
            view = new MedicareEntitlementView( form );
            view.Location      = new Point( 1, 1 );
            view.Model         = msp;
            view.Model_Account = account;
            view.TabIndex      = 0;
            view.Tag           = tag;
        }
        #endregion

        #region Methods
        public static MedicareEntitlementState GetInstance( FSMState state, MSPDialog form,
            Account account, MedicareSecondaryPayor msp, bool dispose )
        {
            if( dispose || stateExemplar == null )
            {
                stateExemplar = null;
                stateExemplar = new MedicareEntitlementState( state, form, account, msp, dispose );
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

        public override FSMState Transition( ESRDEvent eventStimulus, MSPDialog form,
            Account account, MedicareSecondaryPayor msp )
        {
            if( View.FormChanged )
            {
                ControlView cv = ESRDEntitlementPage1State.GetInstance( this, form, account, msp, false ).View;
                cv.Dispose();
            }
            ESRDEntitlementPage1State nextState = ESRDEntitlementPage1State.GetInstance( this, form, account, msp, view.FormChanged );

            if( View.FormChanged )
            {
                View.FormChanged = false;
                nextState.View.FormChanged = true;
            }

            nextState.View.UpdateView();
            return nextState;
        }

        public override FSMState Transition( DisabilityEvent eventStimulus, MSPDialog form,
                                             Account account, MedicareSecondaryPayor msp )
        {
            if( View.FormChanged )
            {
                ControlView cv = DisabilityEntitlementPage1State.GetInstance( this, form, account, msp, false ).View;
                cv.Dispose();
            }
            DisabilityEntitlementPage1State nextState = DisabilityEntitlementPage1State.GetInstance( this, form, account, msp, View.FormChanged );

            if( View.FormChanged )
            {
                View.FormChanged = false;
                nextState.View.FormChanged = true;
            }

            nextState.View.UpdateView();
            return nextState;
        }

        public override FSMState Transition( AgeEvent eventStimulus, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            if( View.FormChanged )
            {
                ControlView cv = AgeEntitlementPage1State.GetInstance( this, form, account, msp, false ).View;
                cv.Dispose();
            }
            AgeEntitlementPage1State nextState = AgeEntitlementPage1State.GetInstance( this, form, account, msp, View.FormChanged );

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

        public MedicareEntitlementView View
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
        public static MedicareEntitlementView  view;
        public static MedicareEntitlementState stateExemplar;
        private string                         strState = "Medicare Entitlement";
        #endregion
    }
}
