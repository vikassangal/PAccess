using PatientAccess.UI.HelperClasses;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// RequiredFieldsSummaryView - displays a list of fields required to complete the current activity.
    /// Fields are grouped by their composite rule class.
    /// </summary>
    [Serializable]
    public class SequesteredPatientsAlertDialog : TimeOutFormView
    {       
        public string FirstName { private get; set; }
        public string LastName {private get; set; }
        public string Phone { private get; set; }
        #region Event Handlers
        private void SequesteredPatientsAlertDialog_Load(object sender, EventArgs e)
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
            Text = "Discern: " + LastName + ", " + FirstName;
            txtErrors.Text = UIErrorMessages.SEQUESTEREDPATIENTTEXT + ' ' + Phone + '.';
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SequesteredPatientsAlertDialog));
            this.btnDismiss = new PatientAccess.UI.CommonControls.LoggingButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtErrors = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnDismiss.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDismiss.Location = new System.Drawing.Point(650, 415);
            this.btnDismiss.Message = null;
            this.btnDismiss.Name = "btnDismiss";
            this.btnDismiss.Size = new System.Drawing.Size(380, 50);
            this.btnDismiss.TabIndex = 0;
            this.btnDismiss.Text = "Dismiss for this account";
            this.btnDismiss.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.Location = new System.Drawing.Point(62, 57);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(84, 77);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // lblHeader
            // 
            this.lblHeader.ForeColor = System.Drawing.Color.Red;
            this.lblHeader.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            this.lblHeader.Location = new System.Drawing.Point(250, 27);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(690, 60);
            this.lblHeader.TabIndex = 3;
            this.lblHeader.Text = "Discern Alert - Sequestered Patient Records";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtErrors
            // 
            this.txtErrors.BackColor = System.Drawing.Color.White;
            this.txtErrors.CausesValidation = false;
            this.txtErrors.Location = new System.Drawing.Point(187, 100);
            this.txtErrors.MaxLength = 5000;
            this.txtErrors.Multiline = true;
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.ReadOnly = true;
            this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrors.Size = new System.Drawing.Size(928, 292);
            this.txtErrors.TabIndex = 4;
            this.txtErrors.WordWrap = false;
            // 
            // SequesteredPatientsAlertDialog
            // 
            this.AcceptButton = this.btnDismiss;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.CancelButton = this.btnDismiss;
            this.ClientSize = new System.Drawing.Size(1168, 512);
            this.Controls.Add(this.txtErrors);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnDismiss);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SequesteredPatientsAlertDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Discern:";
            this.Load += new System.EventHandler(this.SequesteredPatientsAlertDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Construction and Finalization
        public SequesteredPatientsAlertDialog()
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
        private LoggingButton     btnDismiss;
        private Label      lblHeader;
        private TextBox    txtErrors;
        private PictureBox pictureBox;
        #endregion

        #region Constants
        #endregion

        public void ShowAlert()
        {
            BringToFront();
            CenterToScreen();
            ShowDialog();
        }
    }
}
