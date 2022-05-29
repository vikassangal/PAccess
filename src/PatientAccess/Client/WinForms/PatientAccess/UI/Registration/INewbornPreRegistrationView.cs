using System;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.Registration
{
    public interface INewbornPreRegistrationView 
    {
        Account ModelAccount { get; set; }
        Activity CurrentActivity { get; }
        IAccount SelectedAccount { get; set; }
        IAccountView accountView { get; set; }
        EventHandler ReturnToMainScreen { get; set; }
        bool IsInDesignMode { get; }
        bool IsDisposedOrDisposing { get; }
        void SetMasterPatientIndexViewActivity(Activity activity);
        void RegisterOnReturnToMainScreen( EventHandler returnToMainScreen );
        void EnableAccountView();
        void SetCursor(Cursor cursor);
        void DisplayMasterPatientIndexView();
    }
}
