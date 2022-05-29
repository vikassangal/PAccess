using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Extensions.Exceptions;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// Summary description for LogonFacilityAutoWidthComboBox.
	/// </summary>
	public class LogonFacilityAutoWidthComboBox : ComboBox
	{
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the HandleCreated event of the LogonFacilityAutoWidthComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LogonFacilityAutoWidthComboBox_HandleCreated(object sender, EventArgs e)
        {
            UpdateDropDownWidth();
        }

        /// <summary>
        /// Handles the Invalidate event of the LogonFacilityAutoWidthComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LogonFacilityAutoWidthComboBox_Invalidate(object sender, InvalidateEventArgs e)
        {
            UpdateDropDownWidth();
        }

        /// <summary>
        /// Updates the width of the drop down.
        /// </summary>
        private void UpdateDropDownWidth()
        {
            //Create a GDI+ drawing surface to measure string widths
            Graphics ds = this.CreateGraphics();

            //Float to hold largest single item width
            float maxWidth;
    
            try
            {
                maxWidth = 0;

                //Iterate over each item, measuring the maximum width
                //of the DisplayMember strings
                //foreach(object item in this.Items)
                //foreach(DictionaryEntry facility in this.Items)
                foreach(Facility item in this.Items)
                {
                    //maxWidth = Math.Max(maxWidth, ds.MeasureString(item.ToString(), this.Font).Width);
                    //PatientAccess.Domain.Facility aFacility = (PatientAccess.Domain.Facility)facility.Value;
                    //maxWidth = Math.Max(maxWidth, ds.MeasureString(aFacility.CodeAndDescription, this.Font).Width);
                    maxWidth = Math.Max(maxWidth, ds.MeasureString(item.CodeAndDescription, this.Font).Width);
                }

                //Add a buffer for some white space
                //around the text
                maxWidth +=20;

                //round maxWidth and cast to an int
                int newWidth = (int)Decimal.Round((decimal)maxWidth,0);

                //If the width is bigger than the screen, ensure
                //we stay within the bounds of the screen
                if (newWidth > Screen.GetWorkingArea(this).Width)
                {
                    newWidth = Screen.GetWorkingArea(this).Width;
                }

                //Only change the default width if it's smaller
                //than the newly calculated width
                if (newWidth > initialDropDownWidth)
                {
                    this.DropDownWidth = newWidth;
                }

                //Clean up the drawing surface
                ds.Dispose();
            
            }
            catch( Exception e )
            {
                c_log.Error( "Error updating drop down width:" + Environment.NewLine + e.Message, e);
                throw new EnterpriseException( e.Message, Severity.Low );
            }
        }

        /// <summary>
        /// Override the Window WndProc method to move the drop-down box.
        /// </summary>
        /// <param name="m">The m.</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CTLCOLORLISTBOX)
            {
                // Make sure we are inbounds of the screen
                int left = this.PointToScreen(new Point(0, 0)).X;
		                
                //Only do this if the dropdown is going off right edge of screen
                if (this.DropDownWidth > Screen.PrimaryScreen.WorkingArea.Width - left)
                {
                    // Get the current combo position and size
                    Rectangle comboRect = this.RectangleToScreen(this.ClientRectangle);

                    int dropHeight = 0;
                    int topOfDropDown = 0;
                    int leftOfDropDown = 0;

                    //Calculate dropped list height
                    for (int i=0;(i<this.Items.Count && i<this.MaxDropDownItems);i++)
                    {
                        dropHeight += this.ItemHeight;
                    }

                    //Set top position of the dropped list if 
                    //it goes off the bottom of the screen
                    if (dropHeight > Screen.PrimaryScreen.WorkingArea.Height -
                        this.PointToScreen(new Point(0, 0)).Y)
                    {
                        topOfDropDown = comboRect.Top - dropHeight - 2;
                    }
                    else
                    {
                        topOfDropDown = comboRect.Bottom;
                    }
			
                    //Calculate shifted left position
                    leftOfDropDown = comboRect.Left - (this.DropDownWidth -
                        (Screen.PrimaryScreen.WorkingArea.Width - left));

                    SetWindowPos(m.LParam,
                        IntPtr.Zero,
                        leftOfDropDown,
                        topOfDropDown,
                        0,
                        0,
                        SWP_NOSIZE);
                }
            }

            base.WndProc (ref m);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Private Properties
        #endregion
        
        #region Construction and Finalization
        /// <summary>
        /// Initializes a new instance of the <see cref="LogonFacilityAutoWidthComboBox"/> class.
        /// </summary>
        public LogonFacilityAutoWidthComboBox()
        {
            InitializeComponent();

            initialDropDownWidth = this.DropDownWidth;

            this.HandleCreated += new EventHandler( LogonFacilityAutoWidthComboBox_HandleCreated );
            this.Invalidated += new InvalidateEventHandler( LogonFacilityAutoWidthComboBox_Invalidate );
        }
        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        #region Static Members
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( LogonFacilityAutoWidthComboBox ) );
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private const int SWP_NOSIZE = 0x1;
        private const UInt32 WM_CTLCOLORLISTBOX = 0x0134;
        //Store the default width to perform check in UpdateDropDownWidth
        private int initialDropDownWidth = 0;
        #endregion

        #region Constants
        #endregion
    }
}