using PatientAccess.Rules;

using System;

namespace PatientAccess.UI.CommonControls
{
    partial class SSNControl
    {
        #region Construction and Finalization

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();

                RuleEngine.GetInstance().UnregisterEvent( typeof( PersonSSNPreferred ), Model_Person, PersonSSNPreferredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( PersonSSNRequired ), Model_Person, PersonSSNRequiredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( SocialSecurityNumberRequired ), Model_Person, PersonSSNRequiredEventHandler );
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpSSN = new System.Windows.Forms.GroupBox();
            this.mtbSSNNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblSSNNumber = new System.Windows.Forms.Label();
            this.cmbSSNStatus = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblSSNStatus = new System.Windows.Forms.Label();
            this.grpSSN.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSSN
            // 
            this.grpSSN.Controls.Add( this.mtbSSNNumber );
            this.grpSSN.Controls.Add( this.lblSSNNumber );
            this.grpSSN.Controls.Add( this.cmbSSNStatus );
            this.grpSSN.Controls.Add( this.lblSSNStatus );
            this.grpSSN.Location = new System.Drawing.Point( 0, 0 );
            this.grpSSN.Name = "grpSSN";
            this.grpSSN.Size = new System.Drawing.Size( 190, 72 );
            this.grpSSN.TabIndex = 0;
            this.grpSSN.TabStop = false;
            this.grpSSN.Text = "SSN";
            // 
            // mtbSSNNumber
            // 
            this.mtbSSNNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbSSNNumber.KeyPressExpression = "^\\d*$";
            this.mtbSSNNumber.Location = new System.Drawing.Point( 75, 41 );
            this.mtbSSNNumber.Mask = "   -  -";
            this.mtbSSNNumber.MaxLength = 11;
            this.mtbSSNNumber.Name = "mtbSSNNumber";
            this.mtbSSNNumber.Size = new System.Drawing.Size( 100, 20 );
            this.mtbSSNNumber.TabIndex = 2;
            this.mtbSSNNumber.ValidationExpression = "^\\d*$";
            this.mtbSSNNumber.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSsnNumber_Validating );
            // 
            // lblSSNNumber
            // 
            this.lblSSNNumber.Location = new System.Drawing.Point( 8, 44 );
            this.lblSSNNumber.Name = "lblSSNNumber";
            this.lblSSNNumber.Size = new System.Drawing.Size( 50, 17 );
            this.lblSSNNumber.TabIndex = 0;
            this.lblSSNNumber.Text = "Number:";
            // 
            // cmbSSNStatus
            // 
            this.cmbSSNStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSSNStatus.Location = new System.Drawing.Point( 75, 15 );
            this.cmbSSNStatus.Name = "cmbSSNStatus";
            this.cmbSSNStatus.Size = new System.Drawing.Size( 100, 21 );
            this.cmbSSNStatus.TabIndex = 1;
            this.cmbSSNStatus.SelectedIndexChanged += new System.EventHandler( this.cmbSsnStatus_SelectedIndexChanged );
            this.cmbSSNStatus.Validating += new System.ComponentModel.CancelEventHandler( this.cmbSsnStatus_Validating );
            // 
            // lblSSNStatus
            // 
            this.lblSSNStatus.Location = new System.Drawing.Point( 8, 18 );
            this.lblSSNStatus.Name = "lblSSNStatus";
            this.lblSSNStatus.Size = new System.Drawing.Size( 50, 23 );
            this.lblSSNStatus.TabIndex = 0;
            this.lblSSNStatus.Text = "Status:";
            // 
            // SSNControl
            // 
            this.Controls.Add( this.grpSSN );
            this.Name = "SSNControl";
            this.Size = new System.Drawing.Size( 193, 77 );
            this.Load += new System.EventHandler( this.SSNControl_Load );
            this.grpSSN.ResumeLayout( false );
            this.grpSSN.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        #region Data Elements

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.GroupBox grpSSN;
        private System.Windows.Forms.Label lblSSNStatus;
        public PatientAccessComboBox cmbSSNStatus;
        private System.Windows.Forms.Label lblSSNNumber;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbSSNNumber;

        #endregion
    }
}