using System;
using System.Drawing;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for ESRDEntitlementPage2State.
    /// </summary>
    public class ESRDEntitlementPage2State : FSMState
    {
        #region Construction
        public ESRDEntitlementPage2State( FSMState state, MSPDialog form, Account account,
            MedicareSecondaryPayor msp, bool tag ) : base( state )
        {
            view = new ESRDEntitlementPage2View( form );
            view.Location      = new Point( 1, 1 );
            view.Model         = msp;
            view.Model_Account = account;
            view.TabIndex      = 0;
            view.Tag           = tag;
        }
        #endregion

        #region Methods
        public static ESRDEntitlementPage2State GetInstance( FSMState state, MSPDialog form,
            Account account, MedicareSecondaryPayor msp, bool dispose )
        {
            if( dispose || stateExemplar == null )
            {
                stateExemplar = null;
                stateExemplar = new ESRDEntitlementPage2State( state, form, account, msp, dispose );
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

        public override FSMState Transition( YesEvent stimulus, MSPDialog form,
            Account account, MedicareSecondaryPayor msp )
        {
            if( View.FormChanged )
            {
                ControlView cv = SummaryPageState.GetInstance( this, form, account, msp, false ).View;
                cv.Dispose();
            }
            SummaryPageState nextState = SummaryPageState.GetInstance( this, form, account, msp, View.FormChanged );

            if( View.FormChanged )
            {
                View.FormChanged = false;
                nextState.View.FormChanged = true;
            }

            nextState.View.UpdateView();
            return nextState;
        }

        public override FSMState Transition( NoEvent stimulus, MSPDialog form,
            Account account, MedicareSecondaryPayor msp )
        {
            if( View.FormChanged )
            {
                ControlView cv = SummaryPageState.GetInstance( this, form, account, msp, false ).View;
                cv.Dispose();
            }
            SummaryPageState nextState = SummaryPageState.GetInstance( this, form, account, msp, View.FormChanged );

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

        public ESRDEntitlementPage2View View
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
        public static ESRDEntitlementPage2View  view;
        public static ESRDEntitlementPage2State stateExemplar;
        private string                          strState = "Entitlement by ESRD";
        #endregion
    }
}
