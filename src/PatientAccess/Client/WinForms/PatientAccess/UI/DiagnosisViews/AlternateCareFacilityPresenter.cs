using PatientAccess.Domain;
using PatientAccess.Rules;

namespace PatientAccess.UI.DiagnosisViews
{
    /// <summary>
    /// 
    /// </summary>
    public class AlternateCareFacilityPresenter : IAlternateCareFacilityPresenter
    {
        public AlternateCareFacilityPresenter( IAlternateCareFacilityView view,
                                               IAlternateCareFacilityFeatureManager alternateCareFacilityFeatureManager )
        {
            View = view;
            AlternateCareFacilityFeatureManager = alternateCareFacilityFeatureManager;
            RuleEngine = RuleEngine.GetInstance();
        }
        
        #region IAlternateCareFacilityPresenter Members

        /// <summary>
        /// Updates the alternate care facility.
        /// </summary>
        /// <param name="alternateCareFacility">The alternate care facility.</param>
        public void UpdateAlternateCareFacility( string alternateCareFacility )
        {
            View.Model.AlternateCareFacility = alternateCareFacility;
            EvaluateAlternateCareFacilityRule();
        }

        /// <summary>
        /// Handles the alternate care facility.
        /// </summary>
        public void HandleAlternateCareFacility()
        {
            if (ModelAccount != null && !ModelAccount.Activity.IsNewBornRelatedActivity())
            {
                View.ShowAlternateCareFacilityVisible();
               
                bool alternateCareFacilityEnabledForAccountCreatedDate = AlternateCareFacilityFeatureManager.
                    IsAlternateCareFacilityEnabledForDate( ModelAccount.AccountCreatedDate );
                
                bool alternateCareFacilityEnabledForAdmitDate = AlternateCareFacilityFeatureManager.
                    IsAlternateCareFacilityEnabledForDate( ModelAccount.AdmitDate);

                View.PopulateAlternateCareFacility();

                if ( alternateCareFacilityEnabledForAccountCreatedDate && alternateCareFacilityEnabledForAdmitDate )
                {
                    View.ShowAlternateCareFacilityEnabled();
                }
                else
                {
                    if ( !alternateCareFacilityEnabledForAccountCreatedDate )
                    {
                        View.ClearAlternateCareFacility();
                    }
                  
                    View.ShowAlternateCareFacilityDisabled();
                }
            }
            else
            {
                View.ShowAlternateCareFacilityNotVisible();
            }
            EvaluateAlternateCareFacilityRule();
        }

        #endregion
        #region private Members

        public void EvaluateAlternateCareFacilityRule()
        {
            RuleEngine.EvaluateRule(typeof(AlternateCareFacilityRequired), ModelAccount);
        }
        #endregion private Members
        #region Properties
        private IAlternateCareFacilityView View { get; set; }
        private RuleEngine RuleEngine { get; set; }
        private Account ModelAccount
        {
            get
            {
                return View.Model;
            }
        }

        private IAlternateCareFacilityFeatureManager AlternateCareFacilityFeatureManager { get; set; }
        #endregion Properties
    }
}