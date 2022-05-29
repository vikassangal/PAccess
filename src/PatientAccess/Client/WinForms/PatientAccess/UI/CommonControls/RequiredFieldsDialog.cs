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
    public class RequiredFieldsDialog : TimeOutFormView
    {
        #region Event Handlers
        private void RequiredFieldsDialog_Load(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            lblHeader.Text = i_HeaderText;
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

        public string Title
        {
            set
            {
                Text = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RequiredFieldsDialog));
            this.btnOk = new LoggingButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Location = new System.Drawing.Point(424, 240);
            this.btnOk.Name = "btnOk";
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.Location = new System.Drawing.Point(24, 24);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(32, 32);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // lblHeader
            // 
            this.lblHeader.Location = new System.Drawing.Point(72, 7);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(416, 33);
            this.lblHeader.TabIndex = 3;
            this.lblHeader.Text = "The following required fields must be completed to finish this activity:";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtErrors
            // 
            this.txtErrors.BackColor = System.Drawing.Color.White;
            this.txtErrors.CausesValidation = false;
            this.txtErrors.Location = new System.Drawing.Point(72, 48);
            this.txtErrors.MaxLength = 5000;
            this.txtErrors.Multiline = true;
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.ReadOnly = true;
            this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrors.Size = new System.Drawing.Size(424, 182);
            this.txtErrors.TabIndex = 4;
            this.txtErrors.Text = "";
            this.txtErrors.WordWrap = false;
            // 
            // RequiredFieldsDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(506, 271);
            this.Controls.Add(this.txtErrors);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RequiredFieldsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warning for Required Fields";
            this.Load += new System.EventHandler(this.RequiredFieldsDialog_Load);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public RequiredFieldsDialog()
        {
            InitializeComponent();
            EnableThemesOn( this );
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
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private LoggingButton     btnOk;
        private Label      lblHeader;
        private TextBox    txtErrors;
        private PictureBox pictureBox;
        string                                  i_ErrorText;
        string                                  i_HeaderText;
        #endregion

        #region Constants
        #endregion
    }
}
