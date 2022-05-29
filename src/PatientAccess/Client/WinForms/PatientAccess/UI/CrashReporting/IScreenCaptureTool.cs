using System.Drawing;

namespace PatientAccess.UI.CrashReporting
{
    public interface IScreenCaptureTool {
        Bitmap GetScreenCapture();
    }
}