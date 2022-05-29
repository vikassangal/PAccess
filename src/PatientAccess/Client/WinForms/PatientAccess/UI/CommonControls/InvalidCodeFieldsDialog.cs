using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// RequiredFieldsSummaryView - displays a list of fields required to complete the current activity.
    /// Fields are grouped by their composite rule class.
    /// </summary>
    [Serializable]
    public class InvalidCodeFieldsDialog : TimeOutFormView
    {
        #region Event Handlers

        private void btnOk_Click( object sender, EventArgs e )
        {
            Dispose();
        }

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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(InvalidCodeFieldsDialog));
            this.btnOk = new LoggingButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(344, 304);
            this.btnOk.Name = "btnOk";
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(24, 24);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(32, 32);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // lblHeader
            // 
            this.lblHeader.Location = new System.Drawing.Point(56, 8);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(360, 40);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "The fields listed have values that are not in use by the system. Visit each field " +
                "listed and select a valid value or blank before completeing this activity.";
            // 
            // txtErrors
            // 
            this.txtErrors.BackColor = System.Drawing.Color.White;
            this.txtErrors.CausesValidation = false;
            this.txtErrors.Location = new System.Drawing.Point(56, 64);
            this.txtErrors.MaxLength = 5000;
            this.txtErrors.Multiline = true;
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.ReadOnly = true;
            this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrors.Size = new System.Drawing.Size(344, 208);
            this.txtErrors.TabIndex = 0;
            this.txtErrors.TabStop = false;
            this.txtErrors.Text = "";
            this.txtErrors.WordWrap = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // InvalidCodeFieldsDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.ClientSize = new System.Drawing.Size(430, 351);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtErrors);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InvalidCodeFieldsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warning for Deactivated Values";
            this.ResumeLayout(false);

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
            txtErrors.Text = i_ErrorText;
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

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties        
        #endregion

        #region Construction and Finalization

        public InvalidCodeFieldsDialog()
        {
            InitializeComponent();
            EnableThemesOn( this );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private LoggingButton     btnOk;
        private Label      lblHeader;
        private TextBox    txtErrors;
        private PictureBox pictureBox;
        string                                  i_ErrorText;
        private PictureBox pictureBox1;
        string                                  i_HeaderText;

        #endregion

        #region Constants
        #endregion


    }
}
