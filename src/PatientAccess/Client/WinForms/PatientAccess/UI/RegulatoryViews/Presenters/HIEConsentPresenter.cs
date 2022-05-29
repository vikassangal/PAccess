using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Views;
using PatientAccess.Utilities;


namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class HIEConsentPresenter : IHIEShareDataPresenter
    {
        #region Constructors

        public HIEConsentPresenter(IHIEShareDataFlagView view, IHIEConsentFeatureManager featureManager, Account account)
        {
            Guard.ThrowIfArgumentIsNull(view, "view");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(featureManager, "featureManager");

            View = view;
            FeatureManager = featureManager;
            Account = account;
        }

        #endregion

        #region private methods

        private void HideShareDataWithPublicHIE()
        {
            View.HideShareDataWithPublicHIE();
            View.HideShareDataWithPCP();
        }

        private void ShowShareDataWithPublicHIE()
        {
            bool HIEConsentFeatureEnabled = FeatureManager.IsHIEConsentFeatureManagerEnabled(Account);

            if (Account != null && HIEConsentFeatureEnabled)
            {
                View.ShowShareDataWithPublicHIE();
                View.ShowShareDataWithPCP();
                View.PopulateShareDataWithPublicHIE(HieConsentFlags);
                View.PopulateShareDataWithPCP(HieConsentFlags);
                View.ShareDataWithPublicHIE = Account.ShareDataWithPublicHieFlag;
                View.ShareDataWithPCP = Account.ShareDataWithPCPFlag;
            }
        }

        #endregion

        #region public methods
        public void HandleNotifyPCP()
        {
             
        }
        public void UpdateShareDataWithPublicHIE()
        {
            Account.ShareDataWithPublicHieFlag = View.ShareDataWithPublicHIE;
        }

        public void UpdateShareDataWithPCP()
        {
            Account.ShareDataWithPCPFlag = View.ShareDataWithPCP;
        }

        public void AutoPopulateShareDataWithPublicHIEForRightToRestrict(bool rightToRestrictChecked){}

        public void AutoPopulateShareDataWithPublicHIEForShareDatawithPCP(bool selectedShareDataWithPCP) { }
        
        public void UpdateView()
        {
            HideShareDataWithPublicHIE();
            ShowShareDataWithPublicHIE();
        }
        
        #endregion

        #region Properties

        public IHIEShareDataFlagView View { get; set; }

        private IHIEConsentFeatureManager FeatureManager { get; set; }

        private Account Account { get; set; }


        private static readonly YesNoFlag[] HieConsentFlags =
        {
            YesNoFlag.Yes,
            YesNoFlag.No
        };

        #endregion Properties
     }

}