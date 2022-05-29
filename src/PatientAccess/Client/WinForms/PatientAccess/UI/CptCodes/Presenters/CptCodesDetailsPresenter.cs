using PatientAccess.Domain;
using PatientAccess.UI.CptCodes.Views;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;

namespace PatientAccess.UI.CptCodes.Presenters
{
    public class CptCodesDetailsPresenter
    {
        #region Construction and Finalization

        public CptCodesDetailsPresenter(ICptCodesDetailsView detailsView, IMessageBoxAdapter messageBoxAdapter, Account account)
        {
            Guard.ThrowIfArgumentIsNull(detailsView, "detailsView");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(account.CptCodes, "account.CptCodes");

            Account = account;
            MessageBoxAdapter = messageBoxAdapter;
            DetailsView = detailsView;
        }

        #endregion

        #region Public Methods

        public void ValidateCptCode(CptFields cptCode, string text)
        {
            if (text != null)
            {
                DetailsView.SetNormalColor(cptCode);

                if (IsCptCodeValid(text)) return;

                DetailsView.SetErrorColor(cptCode);

                DetailsView.SetFocus(cptCode);
            }
        }

        private bool IsCptCodeValid(string cptCode)
        {
            if (cptCode.Length > 0 && cptCode.Length != 5)
            {
                DisplayMessage(UIErrorMessages.CPTCODE_MUST_HAVE_FIVE_CHARACTERS, UIErrorMessages.ERROR);
                return false;
            }

            return true;
        }

        public void UpdateCptCodes()
        {
            if (DetailsView.CptCodes.Count == 0)
            {
                if (!messageDisplayed)
                {
                    DisplayMessage(UIErrorMessages.CPTCODE_INVALID_MESSAGE, UIErrorMessages.WARNING);
                    messageDisplayed = true;
                }
                else
                {
                    Account.CptCodes.Clear();
                    DetailsView.CloseView();
                }
            }

            else
            {
                Account.CptCodes = DetailsView.CptCodes;
                DetailsView.CloseView();
            }
        }

        public void ShowDetails()
        {
            PopulateCptCodes();
            messageDisplayed = false;
            DetailsView.ShowAsDialog();
        }

        private void PopulateCptCodes()
        {
            DetailsView.ClearCptCodes();
            var textCptCodes = new string[MAX_NUMBER_OF_CPTCODES];

            for (var i = 0; i < MAX_NUMBER_OF_CPTCODES; i++)
            {
                if (Account.CptCodes.ContainsKey(i + 1) && !string.IsNullOrEmpty(Account.CptCodes[i + 1]))
                {
                    textCptCodes[i] = Account.CptCodes[i + 1];
                }

                else
                {
                    textCptCodes[i] = string.Empty;
                }
            }

            DetailsView.SetCptCodes(textCptCodes);
        }

        #endregion

        #region Private Methods

        private void DisplayMessage(string message, string type)
        {
            MessageBoxAdapter.ShowMessageBox(message, type,
                MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                MessageBoxAdapterDefaultButton.Button1);
        }

        #endregion

        #region Data Elements

        private IMessageBoxAdapter MessageBoxAdapter { get; set; }
        private Account Account { get; set; }
        private ICptCodesDetailsView DetailsView { get; set; }
        private bool messageDisplayed;
        public const int MAX_NUMBER_OF_CPTCODES = Account.MAX_NUMBER_OF_CPTCODES;

        #endregion
    }
}