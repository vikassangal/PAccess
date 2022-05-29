using System;
using System.Drawing;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for DisabilityEntitlementPage3State.
    /// </summary>
    public class DisabilityEntitlementPage3State : FSMState
    {
        #region Construction
        public DisabilityEntitlementPage3State( FSMState state, MSPDialog form,
            Account account, MedicareSecondaryPayor msp, bool tag ) : base( state )
        {
            view = new DisabilityEntitlementPage3View( form );
            view.Location      = new Point( 1, 1 );
            view.Model         = msp;
            view.Model_Account = account;
            view.TabIndex      = 0;
            view.Tag           = tag;
        }
        #endregion

        #region Methods
        public static DisabilityEntitlementPage3State GetInstance( FSMState state, MSPDialog form,
            Account account, MedicareSecondaryPayor msp, bool dispose )
        {
            if( dispose || stateExemplar == null )
            {
                stateExemplar = null;
                stateExemplar = new DisabilityEntitlementPage3State( state, form, account, msp, dispose );
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

        public DisabilityEntitlementPage3View View
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
        public static DisabilityEntitlementPage3View  view;
        public static DisabilityEntitlementPage3State stateExemplar;
        private string                                strState = "Entitlement by Disability";
        #endregion
    }
}
