using System;
using System.Windows.Forms;

namespace PatientAccess.UI
{
    /// <summary>
    /// Summary description for Validation.
    /// </summary>
    public class Validation
    {
        public static void TextBoxValidation( Control control, bool isValid, string message)
        {
            string tooltipMsg = isValid
                ? ""
                : message;

            toolTip.SetToolTip( control, isValid ? String.Empty : message );
        }

        private static ToolTip toolTip = CreateToolTip();

        private static ToolTip CreateToolTip()
        {
            ToolTip toolTip = new ToolTip();

            toolTip.AutoPopDelay = 0;
            toolTip.InitialDelay = 0;
            toolTip.ReshowDelay = 0;

            //toolTip.ShowAlways = true;

            return toolTip;
        }

    }
}
