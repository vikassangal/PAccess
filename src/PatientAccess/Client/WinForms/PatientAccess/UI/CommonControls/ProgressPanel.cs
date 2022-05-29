using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{   

    /// <summary>
    /// Summary description for ProgressPanel.
    /// </summary>
    public class ProgressPanel : UserControl
    {

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string ProgressText
        {
            set
            {
                this.lblText.Text = value;
            }
        }
        #endregion

        #region Private Methods
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if( !this.DesignMode )
            {
                this.pictureBox1.Enabled = true;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            
            foreach( Control c in this.Controls )
            {
                if( c != null )
                {
                    try
                    {
                        c.Dispose();
                    }
                    catch( Exception )
                    {
                        //c_log.Error( "BrokerFactory registered a remoting channel at " + DateTime.Now + ", in BrokerOfType as ChannelServices.RegisterChannel( channel ) Complete : '" + channel.ToString() + "'" );
                        //Swallow the exception.
                    }
                }
            }

            //base.Dispose( disposing );
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ProgressPanel()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }
        #endregion

        #region Data Elements
        private Container     components = null;

        private Label          lblText;

        private PictureBox                          pictureBox1;
        #endregion

        #region Constants
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ProgressPanel ) );
            this.lblText = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ( (System.ComponentModel.ISupportInitialize)( this.pictureBox1 ) ).BeginInit();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.BackColor = System.Drawing.Color.White;
            this.lblText.Font = new System.Drawing.Font( "Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblText.ForeColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 0 ) ) ) ), ( (int)( ( (byte)( 0 ) ) ) ), ( (int)( ( (byte)( 75 ) ) ) ) );
            this.lblText.Location = new System.Drawing.Point( 40, 18 );
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size( 287, 23 );
            this.lblText.TabIndex = 1;
            this.lblText.Text = "Processing...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Image = ( (System.Drawing.Image)( resources.GetObject( "pictureBox1.Image" ) ) );
            this.pictureBox1.Location = new System.Drawing.Point( 19, 20 );
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size( 16, 16 );
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // ProgressPanel
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.pictureBox1 );
            this.Controls.Add( this.lblText );
            this.Name = "ProgressPanel";
            this.Size = new System.Drawing.Size( 751, 413 );
            ( (System.ComponentModel.ISupportInitialize)( this.pictureBox1 ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }
        #endregion
    }
}
