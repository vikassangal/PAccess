using System.Windows.Forms;

namespace PatientAccess.UI
{
    public class WindowsFormsApplicationAdapter : IWindowsFormsApplicationAdapter
    {
        public void Exit()
        {
            Application.Exit();
        }
    }
}