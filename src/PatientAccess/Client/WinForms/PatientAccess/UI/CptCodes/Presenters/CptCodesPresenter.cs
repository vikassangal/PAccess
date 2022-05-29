using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CptCodes.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.CptCodes.Presenters
{
    public class CptCodesPresenter
    {
        #region Construction and Finalization

        public CptCodesPresenter(ICptCodesView cptCodesView, Account account, ICptCodesFeatureManager featureManager, MessageBoxAdapter messageBoxAdapter)
        {
            Guard.ThrowIfArgumentIsNull(cptCodesView, "cptCodesView");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(featureManager, "featureManager");
            Guard.ThrowIfArgumentIsNull(messageBoxAdapter, "messageBoxAdapter");

            CptCodesView = cptCodesView;
            Account = account;
            FeatureManager = featureManager;
            CptCodesView.CptCodesPresenter = this;
            MessageBoxAdapter = messageBoxAdapter;
        }

        #endregion

        #region Public Methods

        public void UpdateView()
        {
            if (FeatureManager.ShouldFeatureBeVisible(Account))
            {
                CptCodesView.ShowMe();
                PopulateCptCodesSelection();
            }

            else
            {
                CptCodesView.HideMe();
            }
        }

        public void ShowCptCodesDetailsDialog(ICptCodesDetailsView detailsView)
        {
            var cptCodesDetailsPresenter = new CptCodesDetailsPresenter(detailsView, MessageBoxAdapter, Account);
            detailsView.Presenter = cptCodesDetailsPresenter;

            CptCodesView.EnableViewButton();
            cptCodesDetailsPresenter.ShowDetails();

            if (Account.CptCodes == null || Account.CptCodes.Count == 0)
            {
                CptCodesView.UnSelected();
                CptCodesView.DisableViewButton();
            }
        }

        public void ClearCptCodes()
        {
            CptCodesView.DisableViewButton();
            Account.CptCodes.Clear();
        }

        #endregion

        #region Private Methods

        private void PopulateCptCodesSelection()
        {
            if (Account.CptCodes != null && Account.CptCodes.Count > 0)
            {
                CptCodesView.CheckYes();
                CptCodesView.EnableViewButton();
            }

            else if (!Account.IsNew)
            {
                CptCodesView.CheckNo();
                CptCodesView.DisableViewButton();
            }
        }

        #endregion

        #region Data Elements

        private ICptCodesView CptCodesView { get; set; }
        private Account Account { get; set; }
        private ICptCodesFeatureManager FeatureManager { get; set; }
        private MessageBoxAdapter MessageBoxAdapter { get; set; }

        #endregion
    }
}