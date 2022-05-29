using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain.InterFacilityTransfer;

namespace PatientAccess.UI.InterfacilityTransfer
{
    /// <summary>
    /// RequiredFieldsSummaryView - displays a list of fields required to complete the current activity.
    /// Fields are grouped by their composite rule class.
    /// </summary>
    [Serializable]
    public class InterfacilityPopup : Form
    {
        #region Event Handlers
        private void RequiredFieldsDialog_Load(object sender, EventArgs e)
        {
            UpdateView();
        }


        private void btnYes_Click(object sender, EventArgs e)
        {
            CancelActivity = false;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelActivity = true;
            this.Close();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Methods
        public void UpdateView()
        {
            lblWarningMSG.Text = i_Message1;

            if (SetOKButton)
            {
                btnCancel.Visible = false;
                btnYes.Visible = false;
                btn_OK.Visible = true;
            }
            else
            {
                btnCancel.Visible = true;
                btnYes.Visible = true;
                btn_OK.Visible = false;
            }
            //Bind From Hospital and From Account
        }

        private Button btn_No;
        #endregion

        #region Properties

        /// <summary>
        /// Label header text
        /// </summary>
        public string HeaderText
        {
            get
            {
                return i_Message1;
            }
            set
            {
                i_Message1 = value;
            }
        }

        public string Title
        {
            set
            {
                Text = value;
            }
        }

        public bool SetOKButton
        {
            get { return i_SetOKButton; }
            set { i_SetOKButton = value; }
        }
        public bool SetCancelButton
        {
            get { return i_SetCancelButton; }
            set { i_SetCancelButton = value; }
        }
        public bool SetYesButton
        {
            get { return i_SetYesButton; }
            set { i_SetYesButton = value; }
        }
        public bool SetNoButton
        {
            get { return i_SetNoButton; }
            set { i_SetNoButton = value; }
        }

        public InterFacilityTransferAccount interFacilityTransferAccount
        {
            get; set;
        }
        public bool CancelActivity;
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnYes = new System.Windows.Forms.Button();
            this.lblFromHosp = new System.Windows.Forms.Label();
            this.lblWarningMSG = new System.Windows.Forms.Label();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_No = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(417, 84);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 29);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point(318, 84);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(75, 29);
            this.btnYes.TabIndex = 8;
            this.btnYes.TabStop = false;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // lblFromHosp
            // 
            this.lblFromHosp.AutoSize = true;
            this.lblFromHosp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFromHosp.Location = new System.Drawing.Point(9, 115);
            this.lblFromHosp.Name = "lblFromHosp";
            this.lblFromHosp.Size = new System.Drawing.Size(0, 20);
            this.lblFromHosp.TabIndex = 4;
            // 
            // lblWarningMSG
            // 
            this.lblWarningMSG.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarningMSG.Location = new System.Drawing.Point(9, 9);
            this.lblWarningMSG.Name = "lblWarningMSG";
            this.lblWarningMSG.Size = new System.Drawing.Size(493, 72);
            this.lblWarningMSG.TabIndex = 3;
            this.lblWarningMSG.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(321, 84);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 29);
            this.btn_OK.TabIndex = 11;
            this.btn_OK.TabStop = false;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Visible = false;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_No
            // 
            this.btn_No.Location = new System.Drawing.Point(417, 84);
            this.btn_No.Name = "btn_No";
            this.btn_No.Size = new System.Drawing.Size(75, 29);
            this.btn_No.TabIndex = 12;
            this.btn_No.Text = "NO";
            this.btn_No.UseVisualStyleBackColor = true;
            this.btn_No.Visible = false;
            this.btn_No.Click += new System.EventHandler(this.btn_No_Click);
            // 
            // InterfacilityPopup
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(522, 121);
            this.Controls.Add(this.btn_No);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.lblFromHosp);
            this.Controls.Add(this.lblWarningMSG);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InterfacilityPopup";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Interfacility Transfer";
            this.Load += new System.EventHandler(this.RequiredFieldsDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Construction and Finalization
        public InterfacilityPopup()
        {
            InitializeComponent();
            interFacilityTransferAccount = new InterFacilityTransferAccount();
            //EnableThemesOn(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private Label lblWarningMSG;
        private Label lblFromHosp;
        private Button btnYes;
        private Button btnCancel;
        public string i_Message2, i_Message1;
        public bool i_SetOKButton, i_SetYesButton, i_SetCancelButton, i_SetNoButton;
        #endregion

        #region Constants
        #endregion


        private void btn_No_Click(object sender, EventArgs e)
        {
            CancelActivity = true;
        }

        private Button btn_OK;
    }
}
