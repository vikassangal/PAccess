using System;
using System.ComponentModel;
using System.Windows.Forms; 
using PatientAccess.BrokerInterfaces; 
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// RequiredFieldsSummaryView - displays a list of fields required to complete the current activity.
    /// Fields are grouped by their composite rule class.
    /// </summary>
    [Serializable]
    public class EMPISearchFieldsDialog : TimeOutFormView, IEMPISearchFieldsDialog
    {
        #region Event Handlers
        private void EMPISearchRequiredFieldsDialog_Load(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
           
            if (EMPISearchFieldsDialogPresenter.ValidateDobEntry()  && EMPISearchFieldsDialogPresenter.ValidateSsn())
            {
                EMPISearchFieldsDialogPresenter.UpdateSearchCriteria();
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.None;
            }
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            EMPISearchFieldsDialogPresenter = new EMPISearchFieldsDialogPresenter(this, SearchCriteria); 
            EMPISearchFieldsDialogPresenter.ValidateSsn();
            EMPISearchFieldsDialogPresenter.ValidateMonth();
            EMPISearchFieldsDialogPresenter.ValidateDay();
            EMPISearchFieldsDialogPresenter.ValidateYear();
            EMPISearchFieldsDialogPresenter.SetPhoneNumberPreferredColor();
        }

        #endregion

        #region Properties

        public PatientSearchCriteria SearchCriteria { get; set; }

        public string SSNText
        {
            get { return axMskSsn.UnMaskedText; }
        }

        public PhoneNumberControl PhoneNumberControl
        {
            get { return phoneNumberControl; }
        }

        public string YearText
        {
            get { return mtbDobY.Text; }
        }

        public string MonthText
        {
            get { return mtbDobM.Text; }
            set { mtbDobM.Text = value; }
        }

        public string DayText
        {
            get { return mtbDobD.Text; }
            set { mtbDobD.Text = value; }
        }

        public void SetSSNNormalColor()
        {
            UIColors.SetNormalBgColor(axMskSsn);
        }

        public void SetSSNErrorColor()
        {
            UIColors.SetErrorBgColor(axMskSsn);
        }

        public void SetFocusToSSN()
        {
            axMskSsn.Focus();
        }

        public void SetSSNPreferredColor()
        {
            if (axMskSsn.UnMaskedText.Trim().Length == 0)
            {
                UIColors.SetPreferredBgColor(axMskSsn);
            }
            else
            {
                SetSSNNormalColor();
            }
        }

        public void SetPreferredColorToMonth()
        {
           UIColors.SetPreferredBgColor(mtbDobM);
        }

        public void SetPreferredColorToDay()
        {
            UIColors.SetPreferredBgColor(mtbDobD);
        }

        public void SetErrorColorToYear()
        {
            UIColors.SetErrorBgColor(mtbDobY);
        }
        public void SetErrorColorToMonth()
        {
            UIColors.SetErrorBgColor(mtbDobM);
        }

        public void SetErrorColorToDay()
        {
            UIColors.SetErrorBgColor(mtbDobD);
        }

        public void SetFocusToYear()
        {
            mtbDobY.Focus();
        }
        public void SetFocusToMonth()
        {
            mtbDobM.Focus();
        }
        public void SetFocusToDay()
        {
            mtbDobD.Focus();
        }
        public void SetPreferredColorToYear()
        {
            UIColors.SetPreferredBgColor(mtbDobY);
        }

        public void SetNormalColorToYear()
        {
            UIColors.SetNormalBgColor(mtbDobY);
        }

        public void SetNormalColorToDay()
        {
            UIColors.SetNormalBgColor(mtbDobD);
        }

        public void SetNormalColorToMonth()
        {
            UIColors.SetNormalBgColor(mtbDobM);
        }

        public void SetDobErrBgColor()
        {
            SetErrorColorToDay();
            SetErrorColorToMonth();
            SetErrorColorToYear();
        }
        public void SetDobPreferredColor()
        {
            SetPreferredColorToDay();
            SetPreferredColorToMonth();
            SetPreferredColorToYear();
        }
        public void SetDOBNormalColor()
        {
            SetNormalColorToDay();
            SetNormalColorToMonth();
            SetNormalColorToYear();
        }
        private EMPISearchFieldsDialogPresenter EMPISearchFieldsDialogPresenter
        {
            get { return iEMPISearchFieldsDialogPresenter; }
            set { iEMPISearchFieldsDialogPresenter = value; }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EMPISearchFieldsDialog));
            this.btnOk = new PatientAccess.UI.CommonControls.LoggingButton();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtEMPIDialog = new System.Windows.Forms.TextBox();
            this.lblSlash2 = new System.Windows.Forms.Label();
            this.lblSlash1 = new System.Windows.Forms.Label();
            this.mtbDobY = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDobD = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDobM = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblDateOfBirth = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.axMskSsn = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSsn = new System.Windows.Forms.Label();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblOr3 = new System.Windows.Forms.Label();
            this.lblOr1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Location = new System.Drawing.Point(461, 342);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOk.Message = null;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 28);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.Location = new System.Drawing.Point(32, 30);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(43, 39);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // lblHeader
            // 
            this.lblHeader.BackColor = System.Drawing.Color.White;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(120, 1);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(389, 73);
            this.lblHeader.TabIndex = 10;
            this.lblHeader.Text = "In order to search EMPI for a patient located in the enterprise that was not foun" +
    "d in PBAR, PAS needs  one of the following search criteria:";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtEMPIDialog
            // 
            this.txtEMPIDialog.BackColor = System.Drawing.Color.White;
            this.txtEMPIDialog.CausesValidation = false;
            this.txtEMPIDialog.Location = new System.Drawing.Point(9, 80);
            this.txtEMPIDialog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtEMPIDialog.MaxLength = 5000;
            this.txtEMPIDialog.Multiline = true;
            this.txtEMPIDialog.Name = "txtEMPIDialog";
            this.txtEMPIDialog.ReadOnly = true;
            this.txtEMPIDialog.Size = new System.Drawing.Size(566, 256);
            this.txtEMPIDialog.TabIndex = 30;
            this.txtEMPIDialog.TabStop = false;
            this.txtEMPIDialog.Validating += new System.ComponentModel.CancelEventHandler(this.DOBDay_Validating);
            // 
            // lblSlash2
            // 
            this.lblSlash2.BackColor = System.Drawing.Color.White;
            this.lblSlash2.Location = new System.Drawing.Point(303, 177);
            this.lblSlash2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSlash2.Name = "lblSlash2";
            this.lblSlash2.Size = new System.Drawing.Size(8, 21);
            this.lblSlash2.TabIndex = 4;
            this.lblSlash2.Text = "/";
            // 
            // lblSlash1
            // 
            this.lblSlash1.BackColor = System.Drawing.Color.White;
            this.lblSlash1.Location = new System.Drawing.Point(261, 177);
            this.lblSlash1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSlash1.Name = "lblSlash1";
            this.lblSlash1.Size = new System.Drawing.Size(9, 22);
            this.lblSlash1.TabIndex = 2;
            this.lblSlash1.Text = "/";
            // 
            // mtbDobY
            // 
            this.mtbDobY.KeyPressExpression = "^\\d*$";
            this.mtbDobY.Location = new System.Drawing.Point(313, 177);
            this.mtbDobY.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mtbDobY.Mask = "";
            this.mtbDobY.MaxLength = 4;
            this.mtbDobY.Name = "mtbDobY";
            this.mtbDobY.Size = new System.Drawing.Size(39, 22);
            this.mtbDobY.TabIndex = 5;
            this.mtbDobY.ValidationExpression = "^\\d*$";
            this.mtbDobY.TextChanged += new System.EventHandler(this.DOBYear_Changed);
            this.mtbDobY.Enter += new System.EventHandler(this.DOBYear_Enter);
            this.mtbDobY.Validating += new System.ComponentModel.CancelEventHandler(this.DOBYear_Validating);
            // 
            // mtbDobD
            // 
            this.mtbDobD.KeyPressExpression = "^[0-2][0-9]*|3[0-1]*|[0-9]$";
            this.mtbDobD.Location = new System.Drawing.Point(272, 177);
            this.mtbDobD.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mtbDobD.Mask = "";
            this.mtbDobD.MaxLength = 2;
            this.mtbDobD.Name = "mtbDobD";
            this.mtbDobD.Size = new System.Drawing.Size(23, 22);
            this.mtbDobD.TabIndex = 3;
            this.mtbDobD.ValidationExpression = "^[0-2][0-9]|3[0-1]|[0-9]$";
            this.mtbDobD.TextChanged += new System.EventHandler(this.DOBDay_Changed);
            this.mtbDobD.Enter += new System.EventHandler(this.DOBDay_Enter);
            this.mtbDobD.Validating += new System.ComponentModel.CancelEventHandler(this.DOBDay_Validating);
            // 
            // mtbDobM
            // 
            this.mtbDobM.KeyPressExpression = "^0[0-9]*|1[0-2]*|[0-9]$";
            this.mtbDobM.Location = new System.Drawing.Point(232, 177);
            this.mtbDobM.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mtbDobM.Mask = "";
            this.mtbDobM.MaxLength = 2;
            this.mtbDobM.Name = "mtbDobM";
            this.mtbDobM.Size = new System.Drawing.Size(23, 22);
            this.mtbDobM.TabIndex = 1;
            this.mtbDobM.ValidationExpression = "^0[0-9]|1[0-2]|[0-9]$";
            this.mtbDobM.TextChanged += new System.EventHandler(this.DOBMonth_changed);
            this.mtbDobM.Enter += new System.EventHandler(this.DOBMonth_Enter);
            this.mtbDobM.Validating += new System.ComponentModel.CancelEventHandler(this.DOBMonth_Validating);
            // 
            // lblDateOfBirth
            // 
            this.lblDateOfBirth.BackColor = System.Drawing.Color.White;
            this.lblDateOfBirth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateOfBirth.Location = new System.Drawing.Point(148, 182);
            this.lblDateOfBirth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDateOfBirth.Name = "lblDateOfBirth";
            this.lblDateOfBirth.Size = new System.Drawing.Size(95, 20);
            this.lblDateOfBirth.TabIndex = 12;
            this.lblDateOfBirth.Text = "DOB:";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(237, 202);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 23);
            this.label6.TabIndex = 13;
            this.label6.Text = "mm/dd/yyyy";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // axMskSsn
            // 
            this.axMskSsn.KeyPressExpression = "^\\d*$";
            this.axMskSsn.Location = new System.Drawing.Point(232, 97);
            this.axMskSsn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.axMskSsn.Mask = "   -  -";
            this.axMskSsn.MaxLength = 11;
            this.axMskSsn.Name = "axMskSsn";
            this.axMskSsn.Size = new System.Drawing.Size(99, 22);
            this.axMskSsn.TabIndex = 0;
            this.axMskSsn.ValidationExpression = "^\\d*$"; 
            this.axMskSsn.Validating += new System.ComponentModel.CancelEventHandler(this.SSN_Validating);
            // 
            // lblSsn
            // 
            this.lblSsn.BackColor = System.Drawing.Color.White;
            this.lblSsn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSsn.Location = new System.Drawing.Point(148, 97);
            this.lblSsn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSsn.Name = "lblSsn";
            this.lblSsn.Size = new System.Drawing.Size(43, 20);
            this.lblSsn.TabIndex = 17;
            this.lblSsn.Text = "SSN:";
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.BackColor = System.Drawing.Color.White;
            this.phoneNumberControl.Location = new System.Drawing.Point(232, 272);
            this.phoneNumberControl.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.phoneNumberControl.Model = ((PatientAccess.Domain.Parties.PhoneNumber)(resources.GetObject("phoneNumberControl.Model")));
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size(125, 33);
            this.phoneNumberControl.TabIndex = 6;
            this.phoneNumberControl.AreaCodeChanged += new System.EventHandler(this.phoneNumberControl_AreaCodeChanged);
            this.phoneNumberControl.PhoneNumberChanged += new System.EventHandler(this.phoneNumberControl_PhoneNumberChanged);
            this.phoneNumberControl.Enter += new System.EventHandler(this.PhoneNumber_Enter);
            this.phoneNumberControl.Leave += new System.EventHandler(this.PhoneNumber_Leave);
            this.phoneNumberControl.Validating += new System.ComponentModel.CancelEventHandler(this.PhoneNumber_Validating);
            // 
            // lblPhone
            // 
            this.lblPhone.BackColor = System.Drawing.Color.White;
            this.lblPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPhone.Location = new System.Drawing.Point(148, 281);
            this.lblPhone.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(67, 20);
            this.lblPhone.TabIndex = 26;
            this.lblPhone.Text = "Phone:";
            // 
            // lblOr3
            // 
            this.lblOr3.BackColor = System.Drawing.Color.White;
            this.lblOr3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOr3.Location = new System.Drawing.Point(255, 236);
            this.lblOr3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOr3.Name = "lblOr3";
            this.lblOr3.Size = new System.Drawing.Size(53, 20);
            this.lblOr3.TabIndex = 29;
            this.lblOr3.Text = "OR";
            // 
            // lblOr1
            // 
            this.lblOr1.BackColor = System.Drawing.Color.White;
            this.lblOr1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOr1.Location = new System.Drawing.Point(255, 139);
            this.lblOr1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOr1.Name = "lblOr1";
            this.lblOr1.Size = new System.Drawing.Size(53, 20);
            this.lblOr1.TabIndex = 27;
            this.lblOr1.Text = "OR";
            // 
            // EMPISearchRequiredFieldsDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(577, 385);
            this.Controls.Add(this.lblOr3);
            this.Controls.Add(this.lblOr1);
            this.Controls.Add(this.phoneNumberControl);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.axMskSsn);
            this.Controls.Add(this.lblSsn);
            this.Controls.Add(this.lblSlash2);
            this.Controls.Add(this.lblSlash1);
            this.Controls.Add(this.mtbDobY);
            this.Controls.Add(this.mtbDobD);
            this.Controls.Add(this.mtbDobM);
            this.Controls.Add(this.lblDateOfBirth);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtEMPIDialog);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EMPISearchFieldsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warning for Required Fields";
            this.Load += new System.EventHandler(this.EMPISearchRequiredFieldsDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Construction and Finalization
        public EMPISearchFieldsDialog()
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
        private readonly Container components = null;
        private LoggingButton     btnOk;
        private Label      lblHeader;
        private TextBox    txtEMPIDialog;
        private PictureBox pictureBox; 
        private Label lblSlash2;
        private Label lblSlash1;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbDobY;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbDobD;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbDobM;
        private Label lblDateOfBirth;
        private Label label6;
        private Extensions.UI.Winforms.MaskedEditTextBox axMskSsn;
        private Label lblSsn;
        private PhoneNumberControl phoneNumberControl;
        private Label lblOr3;
        private Label lblOr1;
        private Label lblPhone;
        private EMPISearchFieldsDialogPresenter iEMPISearchFieldsDialogPresenter;

        #endregion

        private void phoneNumberControl_AreaCodeChanged(object sender, EventArgs e)
        {
            if (phoneNumberControl.AreaCode.Length == 0)
            {
                EMPISearchFieldsDialogPresenter.SetPhoneNumberPreferredColor();
            }
        }

        private void phoneNumberControl_PhoneNumberChanged(object sender, EventArgs e)
        {
            if (phoneNumberControl.PhoneNumber.Length == 0)
            {
                EMPISearchFieldsDialogPresenter.SetPhoneNumberPreferredColor();
            }
        }

        private void PhoneNumber_Validating(object sender, CancelEventArgs e)
        {
            EMPISearchFieldsDialogPresenter.SetPhoneNumberPreferredColor();
        }
  
        private void SSN_Validating(object sender, CancelEventArgs e)
        {
            if (!EMPISearchFieldsDialogPresenter.ValidateSsn())
            {
               EMPISearchFieldsDialogPresenter.ShowInvalidSSNErrorMessage();
            }
        }

        private void DOBMonth_Enter(object sender, EventArgs e)
        {
            mtbDobM.SelectionStart = mtbDobM.TextLength;
        }

        private void DOBMonth_changed(object sender, EventArgs e)
        {
            if (mtbDobM.TextLength == 2)
            {
                mtbDobD.Focus();
            }
        }

        private void DOBMonth_Validating(object sender, CancelEventArgs e)
        {
           EMPISearchFieldsDialogPresenter.ValidateMonth();
        }

        private void DOBDay_Enter(object sender, EventArgs e)
        {
            mtbDobD.SelectionStart = mtbDobD.TextLength;
        }

        private void DOBDay_Changed(object sender, EventArgs e)
        {
            if (mtbDobD.TextLength == 2)
            {
                mtbDobY.Focus();
            }
        }

        private void DOBDay_Validating(object sender, CancelEventArgs e)
        {
            EMPISearchFieldsDialogPresenter.ValidateDay();
        }

        private void DOBYear_Enter(object sender, EventArgs e)
        {
            mtbDobY.SelectionStart = mtbDobY.TextLength;
        }

        private void DOBYear_Validating(object sender, CancelEventArgs e)
        {
            EMPISearchFieldsDialogPresenter.ValidateDobEntry();
        }

        private void DOBYear_Changed(object sender, EventArgs e)
        {

        }

        private void PhoneNumber_Enter(object sender, EventArgs e)
        {
            EMPISearchFieldsDialogPresenter.SetPhoneNumberPreferredColor();
        }

        private void PhoneNumber_Leave(object sender, EventArgs e)
        {
            EMPISearchFieldsDialogPresenter.SetPhoneNumberPreferredColor();
        }

        #region Constants
        #endregion

    }
}
