using System.Windows.Forms;

namespace PatientAccess.UI
{
    public interface IMessageBoxAdapter
    {
        DialogResult ShowMessageBox(string messageText);
        DialogResult ShowMessageBox(string messageText, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
        DialogResult ShowMessageBox(string messageText, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
        MessageBoxAdapterResult ShowMessageBox( string messageText, string caption, MessageBoxAdapterButtons buttons, MessageBoxAdapterIcon icon, MessageBoxAdapterDefaultButton defaultButton );
        DialogResult Show(string s, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
    }

    public enum MessageBoxAdapterButtons
    {
        OK,
        OKCancel,
        AbortRetryIgnore,
        YesNoCancel,
        YesNo,
        RetryCancel,
    }

    public enum MessageBoxAdapterResult
    {
        None,
        OK,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No,
    }

    public enum MessageBoxAdapterIcon
    {
        None = 0,
        Error = 16,
        Hand = 16,
        Stop = 16,
        Question = 32,
        Exclamation = 48,
        Warning = 48,
        Asterisk = 64,
        Information = 64,
    }

    public enum MessageBoxAdapterDefaultButton
    {
        Button1 = 0,
        Button2 = 256,
        Button3 = 512,
    }
}