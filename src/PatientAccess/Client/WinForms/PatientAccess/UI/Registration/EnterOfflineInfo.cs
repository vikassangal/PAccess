using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.Registration
{
    /// <summary>
    /// EnterOfflineInfo - capture the Medical record number and/or the AccountNumber for a paper registration
    /// </summary>
    [Serializable]
    public class EnterOfflineInfo : TimeOutFormView
    {
        #region Event Handlers

        private void btnOK_Click(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(this.mtbMRN);
            UIColors.SetNormalBgColor(this.mtbAccount);

            bool rc = true;

            Cursor storedCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            long medicalRecordNumber    = 0;
            long accountNumber          = 0;;

            // do some validations

            if( this.mtbMRN.UnMaskedText.Trim().Length > 0 )
            {
                medicalRecordNumber = Convert.ToInt64(this.mtbMRN.UnMaskedText);
            }
            
            if( this.mtbAccount.UnMaskedText.Trim().Length > 0 )
            {
                accountNumber       = Convert.ToInt64(this.mtbAccount.UnMaskedText);
            }
            
            // see if MRN is in valid range and not used

            if( this.mtbMRN.Enabled )
            {
                try
                {
                    IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

                    // call MRN validation method
                    broker.VerifyOfflineMRN( medicalRecordNumber, User.GetCurrent().Facility.Oid );
                } 
                catch(OfflineMRNInvalidException )
                {
                    UIColors.SetErrorBgColor(this.mtbMRN);
                    MessageBox.Show(UIErrorMessages.OFFLINE_MRN_INVALID, "Error",                         
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1 );                
                    rc = false;
                }
                catch(OfflineMRNAlreadyUsedException )
                {
                    UIColors.SetErrorBgColor(this.mtbMRN);
                    MessageBox.Show(UIErrorMessages.OFFLINE_MRN_ALREADY_USED, "Error",                         
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1 );
                    rc = false;
                    
                }
                catch( Exception )
                {
                    
                }
                finally
                {
                    this.Cursor = storedCursor;
                }
            }
            
            if( rc )
            {                                          
                this.Cursor = Cursors.WaitCursor;

                // see if AccountNumber is in valid range and not used

                try
                {
                    IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

                    // call Account number validation method
                    broker.VerifyOfflineAccountNumber( accountNumber, User.GetCurrent().Facility.Oid );           
                }
                catch( OfflineAccountInvalidException )
                {
                    UIColors.SetErrorBgColor(this.mtbAccount);
                    MessageBox.Show(UIErrorMessages.OFFLINE_ACCOUNT_INVALID, "Error",                         
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1 );
                    rc = false;
                }
                catch( OfflineAccountAlreadyUsedException )
                {
                    UIColors.SetErrorBgColor(this.mtbAccount);
                    MessageBox.Show(UIErrorMessages.OFFLINE_ACCOUNT_ALREADY_USED, "Error",                         
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1 );
                    rc = false;
                }
                finally
                {
                    this.Cursor = storedCursor;
                }   
            }

            if( rc )
            {               
                this.DialogResult = DialogResult.OK;
                // if all validations passed...

                this.Model_Patient.MedicalRecordNumber                      = medicalRecordNumber;            

                if( this.Model_Patient.SelectedAccount != null )
                {
                    this.Model_Patient.SelectedAccount.AccountNumber        = accountNumber;
                }

                if( this.Model_IAccount != null )
                {
                    this.Model_IAccount.AccountNumber                       = accountNumber;
                }

                this.Dispose();    
            }
        }

        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.btnOK.Enabled = false;

            if( this.Model_Patient == null  )
            {
                this.Model_Patient = this.Model_IAccount.Patient;
            }

            if( this.Model_Patient.MedicalRecordNumber <= 0  )
            {                
                this.mtbMRN.Focus();
            }
            else
            {
                this.mtbMRN.Text    = this.Model_Patient.MedicalRecordNumber.ToString();;
                this.mtbMRN.Enabled = false;
                this.mtbAccount.Focus();
            }                        
        }

        #endregion

        #region Properties

        public Patient Model_Patient
        {
            private get
            {
                return this.i_Model_Patient;
            }
            set
            {
                this.i_Model_Patient = value;
            }
        }

        public IAccount Model_IAccount
        {
            private get
            {
                return this.i_Model_IAccount;
            }
            set
            {
                this.i_Model_IAccount = value;
            }
        }

        #endregion

        #region Private Methods

        #region Windows Form Designer generated code
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblHeading = new System.Windows.Forms.Label();
            this.lblMRN = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.mtbMRN = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbAccount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.btnOK = new LoggingButton();
            this.btnCancel = new LoggingButton();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            this.lblHeading.Location = new System.Drawing.Point(25, 17);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(311, 13);
            this.lblHeading.TabIndex = 0;
            this.lblHeading.Text = "Enter the number(s) below from the offline paper registration.";
            // 
            // lblMRN
            // 
            this.lblMRN.Location = new System.Drawing.Point(25, 48);
            this.lblMRN.Name = "lblMRN";
            this.lblMRN.Size = new System.Drawing.Size(36, 16);
            this.lblMRN.TabIndex = 0;
            this.lblMRN.Text = "MRN:";
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point(25, 71);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(49, 12);
            this.lblAccount.TabIndex = 0;
            this.lblAccount.Text = "Account:";
            // 
            // mtbMRN
            // 
            this.mtbMRN.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMRN.KeyPressExpression = "^\\d*$";
            this.mtbMRN.Location = new System.Drawing.Point(81, 43);
            this.mtbMRN.Mask = "";
            this.mtbMRN.MaxLength = 9;
            this.mtbMRN.Name = "mtbMRN";
            this.mtbMRN.Size = new System.Drawing.Size(62, 20);
            this.mtbMRN.TabIndex = 1;
            this.mtbMRN.ValidationExpression = "^\\d*$";
            this.mtbMRN.TextChanged += new System.EventHandler(this.mtbMRN_TextChanged);
            // 
            // mtbAccount
            // 
            this.mtbAccount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAccount.KeyPressExpression = "^\\d*$";
            this.mtbAccount.Location = new System.Drawing.Point(81, 68);
            this.mtbAccount.Mask = "";
            this.mtbAccount.MaxLength = 9;
            this.mtbAccount.Name = "mtbAccount";
            this.mtbAccount.Size = new System.Drawing.Size(62, 20);
            this.mtbAccount.TabIndex = 2;
            this.mtbAccount.ValidationExpression = "^\\d*$";
            this.mtbAccount.TextChanged += new System.EventHandler(this.mtbAccount_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.Location = new System.Drawing.Point(181, 105);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(264, 105);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // EnterOfflineInfo
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.ClientSize = new System.Drawing.Size(345, 134);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.mtbAccount);
            this.Controls.Add(this.mtbMRN);
            this.Controls.Add(this.lblAccount);
            this.Controls.Add(this.lblMRN);
            this.Controls.Add(this.lblHeading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterOfflineInfo";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter Offline MRN and Account Number";
            this.ResumeLayout(false);

        }
        #endregion

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

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public EnterOfflineInfo()
        {
            this.InitializeComponent();
            base.EnableThemesOn( this );
        }

        #endregion

        #region Data Elements

        private Label                  lblHeading;
        private Label                  lblMRN;
        private Label                  lblAccount;

        private MaskedEditTextBox    mtbMRN;
        private MaskedEditTextBox    mtbAccount;

        private LoggingButton                 btnOK;
        private LoggingButton                 btnCancel;

        private Container             components = null;

        private Patient                                     i_Model_Patient = null;
        private IAccount                                    i_Model_IAccount = null;

        #endregion

        #region Constants
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void mtbMRN_TextChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(this.mtbMRN);
            if( this.mtbMRN.Text.Trim() != string.Empty
                && this.mtbAccount.Text.Trim() != string.Empty)
            {
                this.btnOK.Enabled = true;
            }
            else
            {
                this.btnOK.Enabled = false;
            }
        }

        private void mtbAccount_TextChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(this.mtbAccount);
            if( this.mtbMRN.Text.Trim() != string.Empty
                && this.mtbAccount.Text.Trim() != string.Empty)
            {
                this.btnOK.Enabled = true;
            }
            else
            {
                this.btnOK.Enabled = false;
            }
        }
    }
}
	
		
		



	

