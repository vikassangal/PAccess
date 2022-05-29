using System;
using System.Drawing;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for ESRDEntitlementPage1State.
    /// </summary>
    public class ESRDEntitlementPage1State : FSMState
    {
        #region Construction
        public ESRDEntitlementPage1State( FSMState state, MSPDialog form, Account account,
            MedicareSecondaryPayor msp, bool tag ) : base( state )
        {
            view = new ESRDEntitlementPage1View( form );
            view.Location      = new Point( 1, 1 );
            view.Model         = msp;
            view.Model_Account = account;
            view.TabIndex      = 0;
            view.Tag           = tag;
        }
        #endregion

        #region Methods
        public static ESRDEntitlementPage1State GetInstance( FSMState state, MSPDialog form,
            Account account, MedicareSecondaryPayor msp, bool dispose )
        {
            if( dispose || stateExemplar == null )
            {
                stateExemplar = null;
                stateExemplar = new ESRDEntitlementPage1State( state, form, account, msp, dispose );
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

        public override FSMState Transition( YesEvent eventStimulus, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            return NextState(form, account, msp);
        }

        private FSMState NextState(MSPDialog form, Account account, MedicareSecondaryPayor msp)
        {
            if (View.FormChanged)
            {
                ControlView cv = ESRDEntitlementPage2State.GetInstance(this, form, account, msp, false).View;
                cv.Dispose();
            }
            ESRDEntitlementPage2State nextState = ESRDEntitlementPage2State.GetInstance(this, form, account, msp,
                                                                                        View.FormChanged);

            if (View.FormChanged)
            {
                View.FormChanged = false;
                nextState.View.FormChanged = true;
            }

            nextState.View.UpdateView();
            return nextState;
        }

        public override FSMState Transition( NoEvent eventStimulus, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            return NextState(form, account, msp);
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

        public ESRDEntitlementPage1View View
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
        public static ESRDEntitlementPage1View  view;
        public static ESRDEntitlementPage1State stateExemplar;
        private string                          strState = "Entitlement by ESRD";
        #endregion
    }
}
