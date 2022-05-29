using System;
using System.Drawing;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// PatientAccessDataGrid - overrides several events... namely:
	///     1. OnMouseUp - to prevent the sort indicator from disappearing
	///     2. VertScollBar.VisibleChanged event to always show the scrollbar.
	/// </summary>
    public class PatientAccessDataGrid : DataGrid
    {
        public PatientAccessDataGrid()
        {
            this.VertScrollBar.Visible = true; 
            this.VertScrollBar.VisibleChanged += new EventHandler(ShowScrollBars); 
        } 


        protected override bool ProcessDialogKey(Keys keyData)
        {
            // if the user pressed the enter key, handle it in the parent class
            // if they pressed tab or shift/tab, make it process like arrow down and arrow up, respectively
            // otherwise, process the key as normal

            if( keyData == Keys.Return )
            {
                return true;
            }
            else if( keyData.ToString() == "Shift, Tab"
                || keyData.ToString() == "Tab, Shift" )
            {
                base.ProcessDialogKey(Keys.Up);
            }
            else if( keyData == Keys.Tab )
            {
                base.ProcessDialogKey(Keys.Down);
            }
            else
            {
                base.ProcessDialogKey (keyData);               
            }

            return true;
        }

        private void ShowScrollBars(object sender, EventArgs e) 
        { 
            if(!this.VertScrollBar.Visible) 
            { 
                int width = this.VertScrollBar.Width; 
 
                this.VertScrollBar.Location = new Point(this.ClientRectangle.Width - width - BORDERWIDTH, 0); 
                this.VertScrollBar.Size = new Size(width, this.ClientRectangle.Height - BORDERWIDTH); 
 
                this.VertScrollBar.VisibleChanged -= new EventHandler(ShowScrollBars);
                
                this.VertScrollBar.Show();                     

                this.VertScrollBar.VisibleChanged += new EventHandler(ShowScrollBars); 
            } 
        }  

        private int BORDERWIDTH = 2;       

    }
}
