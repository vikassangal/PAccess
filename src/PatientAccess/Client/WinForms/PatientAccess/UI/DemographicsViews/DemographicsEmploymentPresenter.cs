using System;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace PatientAccess.UI.DemographicsViews
{
    internal class DemographicsEmploymentPresenter : IDemographicsEmploymentPresenter
    {

        #region Constructors

        internal DemographicsEmploymentPresenter( IDemographicsEmploymentView view, Account modelAccount, IRuleEngine ruleEngine )
        {
            View = view;
            Account = modelAccount;
            RuleEngine = ruleEngine;
        }

        #endregion Constructors

        #region Properties

        internal IDemographicsEmploymentView View { get; private set; }

        private IRuleEngine RuleEngine { get; set; }

        private Account Account { get; set; }

        #endregion Properties

        #region Public Methods

        public void SelectedLanguageChanged( Language selectedLanguage )
        {
            var showAndPopulateOtherLanguage = ShowAndPopulateOtherLanguage( selectedLanguage );

            if ( showAndPopulateOtherLanguage )
            {
                View.OtherLanguageVisibleAndEnabled = true;
                View.PopulateOtherLanguage();
            }

            else
            {
                View.OtherLanguageVisibleAndEnabled = false;
                View.ClearOtherLanguage();
            }

            EvaluateOtherLanguageRequired();
        }

        public void UpdateOtherLanguage( string otherLanguage )
        {
            if ( Account != null && Account.Patient != null && Account.Patient.Language != null )
            {
                Account.Patient.OtherLanguage = otherLanguage;
            }

            EvaluateOtherLanguageRequired();
        }

        #endregion Public Methods

        #region Private Methods

        private bool ShowAndPopulateOtherLanguage( Language selectedLanguage )
        {
            return Account.Facility != null && IsFacilityInCalifornia( Account.Facility ) && selectedLanguage.IsOtherLanguage();
        }

        private static bool IsFacilityInCalifornia( IFacility facility )
        {
            facility.SetFacilityStateCode();
            return facility.IsFacilityInState( State.CALIFORNIA_CODE );
        }

        private void EvaluateOtherLanguageRequired()
        {
            RuleEngine.OneShotRuleEvaluation<OtherLanguageRequired>( Account, OtherLanguageRequiredEvent );
        }
        private void OtherLanguageRequiredEvent( object sender, EventArgs e )
        {
            View.MakeOtherLanguageRequired();
        }
        public void RegisterOtherLanguageRequiredRule()
        {
            RuleEngine.RegisterEvent( typeof( OtherLanguageRequired ), new EventHandler( OtherLanguageRequiredEvent ) );
        }

        public void UnRegisterOtherLanguageRequiredRule()
        {
            RuleEngine.UnregisterEvent( typeof( OtherLanguageRequired ), new EventHandler( OtherLanguageRequiredEvent ) );
        }
        #endregion Private Methods
    }
}