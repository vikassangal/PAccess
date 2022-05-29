using System;
using System.Windows.Forms;
using Action = PatientAccess.UI.NewEmployersManagement.Action;

namespace PatientAccess.UI.CommonControls
{
    public static class ControlExtensions
    {
        public static void UseInvokeIfNeeded(Control control, Action action)
        {
            if (!control.IsDisposed)
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(action);
                }
                else
                {
                    action();
                }
            }
        }

    }
    public struct NMHDR
    {
        public IntPtr hwndFrom;
        public uint idFrom;
        public uint code;
    };
}