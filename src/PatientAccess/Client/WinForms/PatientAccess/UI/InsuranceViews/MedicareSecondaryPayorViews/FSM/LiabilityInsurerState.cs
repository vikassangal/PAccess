using System;
using System.Drawing;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for LiabilityInsurerState.
    /// </summary>
    public class LiabilityInsurerState : FSMState
    {
        #region Construction
        public LiabilityInsurerState( FSMState state, MSPDialog form, Account account,
            MedicareSecondaryPayor msp, bool tag ) : base( state )
        {
            view = new LiabilityInsurerView( form );
            view.Location      = new Point( 1, 1 );
            view.Model         = msp;
            view.Model_Account = account;
            view.TabIndex      = 0;
            view.Tag           = tag;
        }
        #endregion

        #region Methods
        public static LiabilityInsurerState GetInstance( FSMState state, MSPDialog form,
            Account account, MedicareSecondaryPayor msp, bool dispose )
        {
            if( dispose || stateExemplar == null )
            {
                stateExemplar = null;
                stateExemplar = new LiabilityInsurerState( state, form, account, msp, dispose );
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

        public override FSMState Transition( YesEvent stimulus, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            if( View.FormChanged )
            {
                ControlView cv = MedicareEntitlementState.GetInstance( this, form, account, msp, false ).View;
                cv.Dispose();
            }
            MedicareEntitlementState nextState = MedicareEntitlementState.GetInstance( this, form, account, msp, view.FormChanged );

            if( view.FormChanged )
            {
                view.FormChanged = false;
                nextState.View.FormChanged = true;
            }
            
            nextState.View.UpdateView();
            return nextState;
        }

        public override FSMState Transition( NoEvent stimulus, MSPDialog form, Account account, MedicareSecondaryPayor msp )
        {
            if( View.FormChanged )
            {
                ControlView cv = MedicareEntitlementState.GetInstance( this, form, account, msp, false ).View;
                cv.Dispose();
            }
            MedicareEntitlementState nextState = MedicareEntitlementState.GetInstance( this, form, account, msp, view.FormChanged );

            if( view.FormChanged )
            {
                view.FormChanged = false;
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

        public LiabilityInsurerView View
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
        public static LiabilityInsurerView  view;
        public static LiabilityInsurerState stateExemplar;
        private string                      strState = "Liability Insurer";
        #endregion
    }
}
