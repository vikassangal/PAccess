using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// RemainingErrorsSummary - displays a list of fields required to complete the current activity.
    /// Fields are grouped by their composite rule class.
    /// </summary>
    [Serializable]
    public class InsuranceErrorsDialog : TimeOutFormView
    {
        #region Event Handlers
        


        #endregion

        #region Methods
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( InsuranceErrorsDialog ) );
            this.btnOk = new PatientAccess.UI.CommonControls.LoggingButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.btnHiddenCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.txtErrors = new System.Windows.Forms.TextBox();
            ( (System.ComponentModel.ISupportInitialize)( this.pictureBox ) ).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point( 520, 336 );
            this.btnOk.Message = null;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size( 75, 23 );
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler( this.btnOk_Click );
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ( (System.Drawing.Image)( resources.GetObject( "pictureBox.Image" ) ) );
            this.pictureBox.Location = new System.Drawing.Point( 24, 9 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 32, 32 );
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // lblHeader
            // 
            this.lblHeader.Location = new System.Drawing.Point( 72, 8 );
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size( 512, 24 );
            this.lblHeader.TabIndex = 3;
            this.lblHeader.Text = "The following required fields must be completed to finish this activity:";
            // 
            // btnHiddenCancel
            // 
            this.btnHiddenCancel.CausesValidation = false;
            this.btnHiddenCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnHiddenCancel.Location = new System.Drawing.Point( 424, 336 );
            this.btnHiddenCancel.Message = null;
            this.btnHiddenCancel.Name = "btnHiddenCancel";
            this.btnHiddenCancel.Size = new System.Drawing.Size( 75, 23 );
            this.btnHiddenCancel.TabIndex = 0;
            this.btnHiddenCancel.TabStop = false;
            this.btnHiddenCancel.Text = "Hidden";
            this.btnHiddenCancel.Visible = false;
            // 
            // txtErrors
            // 
            this.txtErrors.BackColor = System.Drawing.Color.White;
            this.txtErrors.CausesValidation = false;
            this.txtErrors.Location = new System.Drawing.Point( 72, 48 );
            this.txtErrors.MaxLength = 5000;
            this.txtErrors.Multiline = true;
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.ReadOnly = true;
            this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrors.Size = new System.Drawing.Size( 520, 280 );
            this.txtErrors.TabIndex = 4;
            this.txtErrors.WordWrap = false;
            // 
            // InsuranceErrorsDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.CancelButton = this.btnHiddenCancel;
            this.ClientSize = new System.Drawing.Size( 602, 368 );
            this.Controls.Add( this.txtErrors );
            this.Controls.Add( this.btnHiddenCancel );
            this.Controls.Add( this.lblHeader );
            this.Controls.Add( this.pictureBox );
            this.Controls.Add( this.btnOk );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InsuranceErrorsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warning for Remaining Errors";
            ( (System.ComponentModel.ISupportInitialize)( this.pictureBox ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }
        #endregion

        #region Construction and Finalization

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
            base.Dispose( disposing );
        }
        #endregion
 
        #region Event Handlers
        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.lblHeader.Text     = this.i_HeaderText;
            this.txtErrors.Text     = this.i_ErrorText;
            this.Title              = this.i_Title;
        }

        #endregion

        #region Properties

        public string ErrorText
        {
            get
            {
                return i_ErrorText;
            }
            set
            {               
                i_ErrorText = value;
            }
        }

        /// <summary>
        /// Label header text
        /// </summary>
        public string HeaderText
        {
            get
            {
                return i_HeaderText;
            }
            set
            {
                i_HeaderText = value;
            }
        }

        private string Title
        {
            get
            {
                return i_Title;
            }
            set
            {
                this.i_Title = value;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties        
        #endregion

        #region Construction and Finalization

        public InsuranceErrorsDialog()
        {
            this.InitializeComponent();
            base.EnableThemesOn( this );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private LoggingButton     btnOk;
        private Label      lblHeader;
        private TextBox    txtErrors;
        private PictureBox pictureBox;
        private LoggingButton     btnHiddenCancel;
        string                                  i_ErrorText;
        string                                  i_HeaderText;
        string                                  i_Title;

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        #region Constants
        #endregion


    }
}
