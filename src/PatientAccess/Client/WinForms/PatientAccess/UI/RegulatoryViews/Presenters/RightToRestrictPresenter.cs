using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class RightToRestrictPresenter
    {
        public RightToRestrictPresenter( IRegulatoryView view, IRightToRestrictFeatureManager featureManager, Account account )
        {
            Guard.ThrowIfArgumentIsNull( view, "view" );
            Guard.ThrowIfArgumentIsNull( account, "account" );
            Guard.ThrowIfArgumentIsNull( featureManager, "featureManager" );

            View = view;
            FeatureManager = featureManager;
            Account = account;
        }

        public void SetRightToRestrict( bool isRightToRestrictChecked )
        {
            Account.RightToRestrict = isRightToRestrictChecked ? YesNoFlag.Yes : YesNoFlag.Blank;
            AutoPopulateShareDataWithPublicHIE(isRightToRestrictChecked);
        }

        private void AutoPopulateShareDataWithPublicHIE(bool isRightToRestrictChecked)
        {
            View.AutoPopulateShareDataWithPublicHIEForRightToRestrict(isRightToRestrictChecked);
        }
        public void UpdateView()
        {
            View.DisableRightToRestrict();
            EnableRightToRestrict();
        }

        private void EnableRightToRestrict()
        {
            bool rightToRestrictEnabledForDate = FeatureManager.IsRightToRestrictEnabledForAccountCreatedDate( Account.AccountCreatedDate );

            if ( Account != null && rightToRestrictEnabledForDate )
            {
                View.EnableRightToRestrict();
                UpdateRightToRestrictSelectionOnTheView();
            }
        }

        private void UpdateRightToRestrictSelectionOnTheView()
        {
            if ( Account.RightToRestrict != null && Account.RightToRestrict.IsYes )
            {
                View.SetRightToRestrict();
            }

            else
            {
                View.UnSetRightToRestrict();
            }
        }

        #region Properties

        public IRegulatoryView View { get; private set; }
        private Account Account { get; set; }
        private IRightToRestrictFeatureManager FeatureManager { get; set; }

        #endregion Properties
    }
}