using System;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace PatientAccess.UI.PreMSEViews
{
    internal class PreMseDemographicsPresenter : IPreMseDemographicsPresenter
    {
        #region Fields
        #endregion Fields

        #region Constructors

        internal PreMseDemographicsPresenter( IPreMseDemographicsView view, Account modelAccount, IRuleEngine ruleEngine )
        {
            View = view;
            Account = modelAccount;
            RuleEngine = ruleEngine;
        }

        #endregion Constructors

        #region Properties

        private IPreMseDemographicsView View { get; set; }
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
        private bool ShowAndPopulateOtherLanguage( Language selectedLanguage )
        {
            return Account.Facility != null && IsFacilityInCalifornia( Account.Facility ) && selectedLanguage.IsOtherLanguage();
        }
        public void UpdateOtherLanguage( string otherLanguageText )
        {
            if ( Account != null && Account.Patient != null && Account.Patient.Language != null )
            {
                Account.Patient.OtherLanguage = otherLanguageText;
            }
            EvaluateOtherLanguageRequired();
        }

        private static bool IsFacilityInCalifornia( IFacility facility )
        {
            return facility.IsFacilityInState( State.CALIFORNIA_CODE );
        }

        #endregion Public Methods

        #region Private Methods


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