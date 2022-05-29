using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.InterfacilityTransfer;

namespace PatientAccess.UI.DischargeViews
{
    public partial class InterfacilityWarningPopup : TimeOutFormView
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

        private void btnContinue_Click(object sender, EventArgs e)
        {
            FromAccount = new Account();
            if (cmb_FromAccount.SelectedIndex > 0 && cmb_FromHOSP.SelectedIndex > 0)
            {
                facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility FromFacility = facilityBroker.FacilityWith(cmb_FromHOSP.SelectedValue.ToString());
                FromAccount.Patient.InterFacilityTransferAccount.FromAccountNumber = Convert.ToInt32(cmb_FromAccount.Text);
                FromAccount.Patient.InterFacilityTransferAccount.FromFacilityOid = Convert.ToInt32(FromFacility.Oid);
                CancelActivity = true;
                Close();
            }
            else
            {
                if(cmb_FromAccount.SelectedIndex <= 0 && cmb_FromHOSP.SelectedIndex <= 0)
                {
                    CancelActivity = false;
                    Close();
                }
                else if (cmb_FromAccount.SelectedIndex <= 0)
                {
                    cmb_FromAccount.Focus();
                }
                else
                {
                    cmb_FromHOSP.Focus();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelActivity = false;
            this.Close();

        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            lblWarningMSG.Text = i_HeaderText;
            interfacilityPannel = new InterfacilityPannel();
            interfacilityPannel.TranferToAccount = TranferPatient.InterFacilityTransferAccount;
            DataTable dtHospital;
            dtHospital = interfacilityPannel.FillFromHospital(User.GetCurrent().Facility);
            cmb_FromHOSP.Items.Clear();
            cmb_FromHOSP.DisplayMember = "HospitalName";
            cmb_FromHOSP.ValueMember = "HSPCode";
            cmb_FromHOSP.DataSource = dtHospital;
        }

        public void CheckInTransferTabel(string selectedfacility)
        {
            

            var interfacilityTransferBroker = BrokerFactory.BrokerOfType<IInterfacilityTransferBroker>();
            facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility FromFacility = facilityBroker.FacilityWith(selectedfacility.Trim());
            DataTable dtAccountDDN = new DataTable();
            dtAccountDDN.Columns.Add("AccountNumber");
            DataTable dt =
                interfacilityTransferBroker.GETIFXRFROMHOSPITALACCOUNTSFOR(TranferPatient.MedicalRecordNumber,
                    FromFacility);
            DataTable dtAccounts =
                interfacilityTransferBroker.GetAccountsForPatient(TranferPatient.MedicalRecordNumber,
                    FromFacility);
            DataRow dataRow1 = dtAccountDDN.NewRow();
            dataRow1[0] = "";
            dtAccountDDN.Rows.Add(dataRow1);
            if (dt.Rows.Count > 1)
            {
                foreach (DataRow dtRow in dt.Rows)
                {
                    foreach (DataRow AccountsRow in dtAccounts.Rows)
                    {
                        if (dtRow[0].ToString() != AccountsRow[0].ToString() 
                            && dtRow[0].ToString()!="" &&  AccountsRow[0].ToString()!="")
                        {
                            DataRow dataRow = dtAccountDDN.NewRow();
                            dataRow[0] = AccountsRow[0];
                            dtAccountDDN.Rows.Add(dataRow);
                            dtAccountDDN.AcceptChanges();
                        }
                    }
                }
            }
            else
            {
                foreach (DataRow AccountsRow in dtAccounts.Rows)
                {
                    if (AccountsRow[0].ToString() != "")
                    {
                        DataRow dataRow = dtAccountDDN.NewRow();
                        dataRow[0] = AccountsRow[0];
                        dtAccountDDN.Rows.Add(dataRow);
                        dtAccountDDN.AcceptChanges();
                    }
                }
            }

            cmb_FromAccount.DataSource = dtAccountDDN;
            cmb_FromAccount.DisplayMember = "AccountNumber";
        }
        #endregion

        #region Properties

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

        public Patient TranferPatient
        {
            get; set;
        }

        public Account FromAccount
        {
            get; set;
        }


        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        private PatientAccountsViewPresenter Presenter { get; set; }
        public bool CancelActivity;
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
            this.btnContinue = new System.Windows.Forms.Button();
            this.cmb_FromHOSP = new System.Windows.Forms.ComboBox();
            this.cmb_FromAccount = new System.Windows.Forms.ComboBox();
            this.lblFromAccount = new System.Windows.Forms.Label();
            this.lblFromHosp = new System.Windows.Forms.Label();
            this.lblWarningMSG = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(518, 200);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 29);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.Location = new System.Drawing.Point(410, 200);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(90, 29);
            this.btnContinue.TabIndex = 3;
            this.btnContinue.Text = "Continue >";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // cmb_FromHOSP
            // 
            this.cmb_FromHOSP.FormattingEnabled = true;
            this.cmb_FromHOSP.Location = new System.Drawing.Point(135, 142);
            this.cmb_FromHOSP.Name = "cmb_FromHOSP";
            this.cmb_FromHOSP.Size = new System.Drawing.Size(162, 24);
            this.cmb_FromHOSP.TabIndex = 1;
            this.cmb_FromHOSP.SelectedIndexChanged += new System.EventHandler(this.cmb_FromHOSP_SelectedIndexChanged);
            // 
            // cmb_FromAccount
            // 
            this.cmb_FromAccount.FormattingEnabled = true;
            this.cmb_FromAccount.Location = new System.Drawing.Point(472, 142);
            this.cmb_FromAccount.Name = "cmb_FromAccount";
            this.cmb_FromAccount.Size = new System.Drawing.Size(121, 24);
            this.cmb_FromAccount.TabIndex = 2;
            this.cmb_FromAccount.SelectedIndexChanged += new System.EventHandler(this.cmb_FromAccount_SelectedIndexChanged);
            // 
            // lblFromAccount
            // 
            this.lblFromAccount.AutoSize = true;
            this.lblFromAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFromAccount.Location = new System.Drawing.Point(347, 142);
            this.lblFromAccount.Name = "lblFromAccount";
            this.lblFromAccount.Size = new System.Drawing.Size(119, 20);
            this.lblFromAccount.TabIndex = 9;
            this.lblFromAccount.Text = "From Account:";
            // 
            // lblFromHosp
            // 
            this.lblFromHosp.AutoSize = true;
            this.lblFromHosp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFromHosp.Location = new System.Drawing.Point(9, 142);
            this.lblFromHosp.Name = "lblFromHosp";
            this.lblFromHosp.Size = new System.Drawing.Size(120, 20);
            this.lblFromHosp.TabIndex = 7;
            this.lblFromHosp.Text = "From Hospital:";
            // 
            // lblWarningMSG
            // 
            this.lblWarningMSG.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarningMSG.Location = new System.Drawing.Point(9, 12);
            this.lblWarningMSG.Name = "lblWarningMSG";
            this.lblWarningMSG.Size = new System.Drawing.Size(584, 94);
            this.lblWarningMSG.TabIndex = 8;
            this.lblWarningMSG.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InterfacilityWarningPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(611, 242);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.cmb_FromHOSP);
            this.Controls.Add(this.cmb_FromAccount);
            this.Controls.Add(this.lblFromAccount);
            this.Controls.Add(this.lblFromHosp);
            this.Controls.Add(this.lblWarningMSG);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InterfacilityWarningPopup";
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
        public InterfacilityWarningPopup()
        {
            InitializeComponent();
            EnableThemesOn(this);
            FromAccount = new Account();
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
        private Label lblFromAccount;
        private ComboBox cmb_FromAccount;
        private ComboBox cmb_FromHOSP;
        private Button btnContinue;
        private Button btnCancel;
        string i_HeaderText;
        InterfacilityPannel interfacilityPannel;
        IFacilityBroker facilityBroker;
        #endregion

        #region Constants
        #endregion

        private void cmb_FromHOSP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_FromHOSP.SelectedIndex > 0)
            {
                if (cmb_FromHOSP.SelectedValue.ToString() != "")
                {
                    btnContinue.Enabled = false;
                    CheckInTransferTabel(cmb_FromHOSP.SelectedValue.ToString());
                }
                else
                {
                    cmb_FromAccount.DataSource = null;

                    cmb_FromAccount.SelectedIndex = -1;
                }
            }
            else
            {
                cmb_FromAccount.DataSource = null;
            }

            SetContinueButton();
        }

        private void cmb_FromAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetContinueButton();
        }

        private void SetContinueButton()
        {
            if ((cmb_FromAccount.SelectedIndex > 0 && cmb_FromHOSP.SelectedIndex > 0) 
                || (cmb_FromHOSP.SelectedIndex <= 0 && cmb_FromAccount.SelectedIndex <= 0))
            {
                btnContinue.Enabled = true;
            }           
            else
            {
                btnContinue.Enabled = false;
            }
        }
    }
}