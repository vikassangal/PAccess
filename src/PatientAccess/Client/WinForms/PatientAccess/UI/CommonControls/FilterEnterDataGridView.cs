using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// By default the <see cref="DataGridView"/> swallows the Enter key and
    /// does not let the form's default button get clicked when the user presses
    /// the enter key. This behavior is by design. This class provides a
    /// workaround for this issue
    /// http://social.msdn.microsoft.com/Forums/en-US/winformsdatacontrols/thread/bab211e2-8ba1-44e6-b660-9598913f68a0
    /// </summary>
    public class FilterEnterDataGridView : DataGridView
    {
        protected override bool ProcessDialogKey( Keys keyData )
        {

            if ( keyData == Keys.Enter )
            {
                var form = FindForm();

                if ( form != null && form.AcceptButton != null )
                {
                    form.AcceptButton.PerformClick();
                }

                return true;
            }

            return base.ProcessDialogKey( keyData );
        }

        protected override bool ProcessDataGridViewKey( KeyEventArgs e )
        {

            if ( e.KeyData == Keys.Enter )
            {
                var form = FindForm();

                if ( form != null && form.AcceptButton != null )
                {
                    form.AcceptButton.PerformClick();
                }

                return true;
            }

            return base.ProcessDataGridViewKey( e );
        }
    }
}
