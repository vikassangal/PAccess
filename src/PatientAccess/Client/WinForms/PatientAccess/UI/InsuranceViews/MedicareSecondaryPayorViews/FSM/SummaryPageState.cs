using System;
using System.Drawing;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews.FSM
{
    /// <summary>
    /// Summary description for SummaryPageState.
    /// </summary>
    public class SummaryPageState : FSMState
    {
        #region Construction
        public SummaryPageState( FSMState state, MSPDialog form, Account account,
                                 MedicareSecondaryPayor msp, bool tag ) : base( state )
        {
            view = new SummaryPageView( form );
            view.Location = new Point( 1, 1 );
            view.TabIndex = 0;
            view.Tag      = tag;
            view.Model    = msp;
            IsEndState    = true;
            view.Model_Account = (Account) account;
        }
        #endregion

        #region Methods
        public static SummaryPageState GetInstance( FSMState state, MSPDialog form,
                                                    Account account, MedicareSecondaryPayor msp, bool dispose )
        {
            stateExemplar = null;
            stateExemplar = new SummaryPageState( state, form, account, msp, dispose );
            return stateExemplar;
        }

        public override String GetStateString()
        {
            return strState;
        }

        public override ControlView GetView()
        {
            return view;
        }

        public override int Response
        {
            get
            {
                return view.Response;
            }
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

        public SummaryPageView View
        {
            get
            {
                return view;
            }
        }
        #endregion

        #region Data Elements
        public static SummaryPageView  view;
        public static SummaryPageState stateExemplar;
        private string                 strState = "Summary";
        #endregion
    }
}
