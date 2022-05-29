using System;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    public interface IErrorMessageDisplayHandler
    {
        DialogResult DisplayYesNoErrorMessageFor( Type ruleType );
        void DisplayOkWarningMessageFor(Type ruleType);
    }
}