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
    public class InvalidCodeOptionalFieldsDialog : TimeOutFormView
    {
        #region Event Handlers

        private void btnOk_Click( object sender, EventArgs e )
        {
            Dispose();
            return;
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(InvalidCodeOptionalFieldsDialog));
            this.btnNo = new LoggingButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelHeader2 = new System.Windows.Forms.Label();
            this.labelHeader3 = new System.Windows.Forms.Label();
            this.labelHeader4 = new System.Windows.Forms.Label();
            this.btnYes = new LoggingButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // btnNo
            // 
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Location = new System.Drawing.Point(328, 488);
            this.btnNo.Name = "btnNo";
            this.btnNo.TabIndex = 2;
            this.btnNo.Text = "No";
            this.btnNo.Click += new System.EventHandler(this.btnOk_Click);
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
            this.lblHeader.Location = new System.Drawing.Point(64, 8);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(360, 56);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "The fields listed have values that are not in use by the system. If desired, " +
                "visit any field listed to select a valid value or blank. Because the account has a discharge " +
                "date, it is not mandatory to change these values before completing this activity.";
            // 
            // txtErrors
            // 
            this.txtErrors.BackColor = System.Drawing.Color.White;
            this.txtErrors.CausesValidation = false;
            this.txtErrors.Location = new System.Drawing.Point(56, 208);
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
            this.pictureBox1.Location = new System.Drawing.Point(8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // labelHeader2
            // 
            this.labelHeader2.Location = new System.Drawing.Point(64, 80);
            this.labelHeader2.Name = "labelHeader2";
            this.labelHeader2.Size = new System.Drawing.Size(344, 24);
            this.labelHeader2.TabIndex = 0;
            this.labelHeader2.Text = "Do you want to complete the activity without changing the values?";
            // 
            // labelHeader3
            // 
            this.labelHeader3.Location = new System.Drawing.Point(64, 112);
            this.labelHeader3.Name = "labelHeader3";
            this.labelHeader3.Size = new System.Drawing.Size(192, 23);
            this.labelHeader3.TabIndex = 0;
            this.labelHeader3.Text = "Click Yes to complete the activity.";
            // 
            // labelHeader4
            // 
            this.labelHeader4.Location = new System.Drawing.Point(64, 144);
            this.labelHeader4.Name = "labelHeader4";
            this.labelHeader4.Size = new System.Drawing.Size(280, 23);
            this.labelHeader4.TabIndex = 0;
            this.labelHeader4.Text = "Click No to stay in the activity and change the values.";
            // 
            // btnYes
            // 
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.Location = new System.Drawing.Point(232, 488);
            this.btnYes.Name = "btnYes";
            this.btnYes.TabIndex = 1;
            this.btnYes.Text = "Yes";
            this.btnYes.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(16, 8);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // InvalidCodeOptionalFieldsDialog
            // 
            this.AcceptButton = this.btnNo;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.btnNo;
            this.ClientSize = new System.Drawing.Size(418, 527);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.labelHeader4);
            this.Controls.Add(this.labelHeader3);
            this.Controls.Add(this.labelHeader2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtErrors);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InvalidCodeOptionalFieldsDialog";
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

        public InvalidCodeOptionalFieldsDialog()
        {
            InitializeComponent();
            EnableThemesOn( this );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private Label      lblHeader;
        private TextBox    txtErrors;
        private PictureBox pictureBox;
        string                                  i_ErrorText;
        private PictureBox pictureBox1;
        private LoggingButton btnNo;
        private Label labelHeader2;
        private Label labelHeader3;
        private Label labelHeader4;
        private LoggingButton btnYes;
        private PictureBox pictureBox2;
        string                                  i_HeaderText;

        #endregion

        #region Constants
        #endregion


    }
}
