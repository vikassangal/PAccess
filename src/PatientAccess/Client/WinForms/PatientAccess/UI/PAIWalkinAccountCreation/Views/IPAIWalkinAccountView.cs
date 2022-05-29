using PatientAccess.Domain;

namespace PatientAccess.UI.PAIWalkinAccountCreation.Views
{
    public interface IPAIWalkinAccountView
    {
        void ReEnableFinishButtonAndFusIcon();
        void SetCursorWait();
        void EnableTODO( bool enable );
        void EnableRefreshTODOList( bool enable );
        void EnableCancel( bool enable );
        void SetActiveButtons( bool enable );
        void SetCursorDefault();
        void SetActivatingTab( string value );
        void ShowInvalidFieldsDialog( string inValidCodesSummary );
        bool ShowRequiredFieldsDialogAsNeeded(string summary);
        void RunRulesForTab();
        void SaveAccount();
        Account Model_Account { get; }
    }
}
