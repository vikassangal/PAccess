using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;

namespace PatientAccess.UI.PatientSearch
{
    public interface IPatientsAccountsPresenter
    {
        void SetSearchedAccountNumber( string accountNum );
        void UpdateView();
        void BeforeWork();


        IAccount GetSelectedAccount();
        bool IsAnAccountSelected();
        DialogResult CollectOfflineInformation( IAccount anAccount );
        void ProceedToPreMse( LooseArgs args );
        bool CreateAdditionalPreMseAccount();
        bool CreateAdditionalUCPreMseAccount();
        Activity CurrentActivity { get; }
        IAccount SelectedAccount { get; set; }
        ListView PatientAccounts { get; set; }
        Account NewAccount { get; set; }
        IAccount NewIAccount { get; set; }
        BackgroundWorker BackgroundWorkerRefresh { get; }
        BackgroundWorker BackgroundWorkerUpdate { get; }
        BackgroundWorker BackgroundWorkerEditMaintain { get; }
        BackgroundWorker BackgroundWorkerContinueActivity { get; }
        BackgroundWorker BackgroundWorkerActivatePreReg { get; }
        BackgroundWorker BackgroundWorkerCompletePostMSE { get; }
        CommandButtonManager Cbm { get; }
    }
}