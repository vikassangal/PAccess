using System;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
	/// <summary>
	/// Summary description for VerificationResponse.
	/// </summary>

    [Serializable]
    public class VerificationResponse : ControlView
    {        
        #region Event Handlers


        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            this.richTextBox = new RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.BorderStyle = BorderStyle.None;
            this.richTextBox.Location = new Point( 0, 0 );
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new Size( 874, 135 );
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            // 
            // VerificationResponse
            // 
            this.Controls.Add( this.richTextBox );
            this.Font = new Font( "Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.Name = "VerificationResponse";
            this.Size = new Size( 874, 135 );
            this.ResumeLayout( false );

        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public VerificationResponse()
        {
            this.InitializeComponent();
        }
        
        #endregion

        #region Data Elements
        
        public RichTextBox richTextBox;

        #endregion

        #region Constants
        #endregion
    }
}
