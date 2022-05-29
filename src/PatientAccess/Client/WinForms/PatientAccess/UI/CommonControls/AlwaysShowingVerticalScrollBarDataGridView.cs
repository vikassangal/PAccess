using System;
using System.Drawing;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// The <see cref="DataGridView"/> does not have a property to enable the vertical scroll bars to always show.
    /// This class provides a work around for that scenario
    /// Ref:http://social.msdn.microsoft.com/forums/en-US/winformsdatacontrols/thread/516851de-ac00-4442-b846-44734dcbd7f5/
    /// </summary>
    public class AlwaysShowingVerticalScrollBarDataGridView : FilterEnterDataGridView
    {
        public AlwaysShowingVerticalScrollBarDataGridView()
        {
            VerticalScrollBar.Visible = true;
            VerticalScrollBar.VisibleChanged += VerticalScrollBar_VisibleChanged;
        }

        void VerticalScrollBar_VisibleChanged( object sender, EventArgs e )
        {
            if ( !VerticalScrollBar.Visible )
            {
                int width = VerticalScrollBar.Width;
                VerticalScrollBar.Location = new Point( ClientRectangle.Width - width, 1 );
                //if the horizontal scroll bar needs to be taken into account then subtract its height as well
                //VerticalScrollBar.Size = new Size( width, ClientRectangle.Height - 1 - HorizontalScrollBar.Height );
                VerticalScrollBar.Size = new Size( width, ClientRectangle.Height - 1 );
                VerticalScrollBar.Show();
            }
        }
    }
}