using System;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews
{
    /// <summary>
    /// This Form is used to show a warning to user when validating an 
    /// expired plan. With warning message, expired plan information is 
    /// displayed in this form. User is given an option to select displayed 
    /// expired plan or reject.
    /// </summary>
    public partial class ExpiredPlanContractDialog : TimeOutFormView
    {
        #region Events
        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// This method will populate UI controls which display insurance plan 
        /// information from Model.
        /// </summary>
        public override void UpdateView()
        {
            if ( Model != null )
            {
                planidLabelValue.Text = Model.PlanID;
                typeLabelValue.Text = Model.PlanType.Description;
                lobLabelValue.Text = Model.LineOfBusiness;
                planNameLabelValue.Text = Model.PlanName;
                
                if( Model.EffectiveOn != DateTime.MinValue )
                {
                    effectiveDateLabelValue.Text = Model.EffectiveOn.ToShortDateString();
                }

                if( Model.ApprovedOn != DateTime.MinValue )
                {
                    approvalDateLabelValue.Text = Model.ApprovedOn.ToShortDateString(); 
                }

                if( Model.TerminatedOn != DateTime.MinValue )
                {
                    terminationDateLabelValue.Text = Model.TerminatedOn.ToShortDateString();
                }

                if( Model.CanceledOn != DateTime.MinValue )
                {
                    cancellationDateLabelValue.Text = Model.CanceledOn.ToShortDateString();
                }             
            }
        }
        #endregion

        #region Construction And Finalization
        public ExpiredPlanContractDialog()
        {
            InitializeComponent();
            base.EnableThemesOn(this);
        }

        #endregion

        #region Private Properties
        #endregion

        #region Properties
        public new InsurancePlan Model
        {
            private get 
            {
                return base.Model as InsurancePlan;
            }

            set 
            {
                base.Model = value;
            }
        }
        #endregion

        #region Constants
        #endregion



    }
}