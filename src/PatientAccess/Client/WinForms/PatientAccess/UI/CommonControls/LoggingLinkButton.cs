using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.CommonControls
{
    //TODO: Create XML summary comment for LoggingButton
    public class LoggingLinkButton : Button 
    {
        #region Event Handlers
        #endregion

        #region Methods

        protected override void OnPaint(PaintEventArgs e)
        {            
            Graphics graphics = e.Graphics;

            int penWidth = 4;
            Pen pen = new Pen(Color.FromArgb(209, 228, 243), 1);

            int fontHeight = this.Font.Height;
            Font font = this.Font;

            SolidBrush brush = new SolidBrush( Color.FromArgb(209, 228, 243) );
            graphics.FillRectangle(brush, 0, 0, Width, Height);
            SolidBrush textBrush = new SolidBrush( this.ForeColor );

            graphics.DrawRectangle(pen, (int) penWidth/2, 
                (int) penWidth/2, Width - penWidth, Height - penWidth);

            graphics.DrawString(Text, font, textBrush, penWidth, 
                Height / 2 - fontHeight);
        }

        protected override void OnClick(EventArgs e)
        {   
            if( this.Message == null )
            {
                this.Message = String.Format( "Click {0} from {1}", 
                    this.Text.Replace( "&", string.Empty ), this.Parent.Name );
            }
            
            BreadCrumbLogger.GetInstance.Log( this.Message );
 
            base.OnClick (e);
        }

        #endregion

        #region Properties
        [Browsable(true)]
        private string Message
        {
            get
            {
                return i_Message;
            }
            set
            {
                i_Message = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LoggingLinkButton()
        {
        }
        #endregion

        #region Data Elements
        private string i_Message;
        #endregion

        #region Constants
        #endregion
    }
}
