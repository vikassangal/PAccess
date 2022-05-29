using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.EmploymentViews
{
	/// <summary>
	/// Summary description for EmployerPhoneEntryDialog.
	/// </summary>
	public class EmployerPhoneEntryDialog : TimeOutFormView
	{
        #region Event Handlers
        private void okButton_Click(object sender, EventArgs e)
        {
            this.Cursor =  Cursors.WaitCursor;
            IAddressBroker broker = BrokerFactory.BrokerOfType<IAddressBroker>();
            //broker.SaveEmployerAddress( Model_Employer, User.GetCurrent().Facility );
            broker.SaveNewEmployerAddressForApproval(Model_Employer, User.GetCurrent().Facility.Code );
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void phoneNumberControl_Validating( object sender, CancelEventArgs e )
        {
            if( this.phoneNumberControl.AreaCode.Length == 3 && this.phoneNumberControl.PhoneNumber.Length != 7 )
            {
                MessageBox.Show( UIErrorMessages.PHONE_NUMBER_REQUIRED,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );

                this.phoneNumberControl.FocusPhoneNumber();
            }
            else
            {
                Model_Employer.PartyContactPoint.PhoneNumber =
                        this.phoneNumberControl.Model;
            }                      
        }

        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.mtblabelEmployerNameText.Text = this.Model_Employer.Name;
            this.mtblabelEmployerAddressText.Text =
                this.Model_Employer.PartyContactPoint.Address.AsMailingLabel();
            this.phoneNumberControl.Model =
                 this.Model_Employer.PartyContactPoint.PhoneNumber;
        }
        #endregion

        #region Properties
        public Employer Model_Employer
        {
            get
            {
                return this.Model as Employer;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( EmployerPhoneEntryDialog ) );
            this.okButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.cancelButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mtblabelEmployerNameText = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtblabelEmployerAddressText = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.optionalTextlabel = new System.Windows.Forms.Label();
            this.labelPhone = new System.Windows.Forms.Label();
            this.labelAddress = new System.Windows.Forms.Label();
            this.labelEmployer = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point( 351, 6 );
            this.okButton.Message = null;
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 69, 23 );
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler( this.okButton_Click );
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.cancelButton.Location = new System.Drawing.Point( 429, 6 );
            this.cancelButton.Message = null;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 69, 23 );
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler( this.cancelButton_Click );
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add( this.phoneNumberControl );
            this.panel1.Controls.Add( this.mtblabelEmployerNameText );
            this.panel1.Controls.Add( this.mtblabelEmployerAddressText );
            this.panel1.Controls.Add( this.optionalTextlabel );
            this.panel1.Controls.Add( this.labelPhone );
            this.panel1.Controls.Add( this.labelAddress );
            this.panel1.Controls.Add( this.labelEmployer );
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point( 5, 5 );
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding( 5 );
            this.panel1.Size = new System.Drawing.Size( 504, 127 );
            this.panel1.TabIndex = 9;
            // 
            // mtblabelEmployerNameText
            // 
            this.mtblabelEmployerNameText.Enabled = false;
            this.mtblabelEmployerNameText.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtblabelEmployerNameText.Location = new System.Drawing.Point( 111, 4 );
            this.mtblabelEmployerNameText.Mask = "";
            this.mtblabelEmployerNameText.Name = "mtblabelEmployerNameText";
            this.mtblabelEmployerNameText.Size = new System.Drawing.Size( 373, 20 );
            this.mtblabelEmployerNameText.TabIndex = 14;
            // 
            // mtblabelEmployerAddressText
            // 
            this.mtblabelEmployerAddressText.Enabled = false;
            this.mtblabelEmployerAddressText.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtblabelEmployerAddressText.Location = new System.Drawing.Point( 111, 32 );
            this.mtblabelEmployerAddressText.Mask = "";
            this.mtblabelEmployerAddressText.Multiline = true;
            this.mtblabelEmployerAddressText.Name = "mtblabelEmployerAddressText";
            this.mtblabelEmployerAddressText.Size = new System.Drawing.Size( 260, 59 );
            this.mtblabelEmployerAddressText.TabIndex = 13;
            // 
            // optionalTextlabel
            // 
            this.optionalTextlabel.Location = new System.Drawing.Point( 206, 100 );
            this.optionalTextlabel.Name = "optionalTextlabel";
            this.optionalTextlabel.Size = new System.Drawing.Size( 58, 17 );
            this.optionalTextlabel.TabIndex = 12;
            this.optionalTextlabel.Text = "(Optional)";
            // 
            // labelPhone
            // 
            this.labelPhone.Location = new System.Drawing.Point( 6, 100 );
            this.labelPhone.Name = "labelPhone";
            this.labelPhone.Size = new System.Drawing.Size( 100, 17 );
            this.labelPhone.TabIndex = 9;
            this.labelPhone.Text = "Phone for address:";
            // 
            // labelAddress
            // 
            this.labelAddress.Location = new System.Drawing.Point( 6, 32 );
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size( 100, 15 );
            this.labelAddress.TabIndex = 8;
            this.labelAddress.Text = "Address:";
            // 
            // labelEmployer
            // 
            this.labelEmployer.Location = new System.Drawing.Point( 6, 6 );
            this.labelEmployer.Name = "labelEmployer";
            this.labelEmployer.Size = new System.Drawing.Size( 100, 17 );
            this.labelEmployer.TabIndex = 7;
            this.labelEmployer.Text = "Employer:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add( this.okButton );
            this.panel2.Controls.Add( this.cancelButton );
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point( 5, 132 );
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding( 5 );
            this.panel2.Size = new System.Drawing.Size( 504, 36 );
            this.panel2.TabIndex = 10;
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point( 108, 94 );
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 10;
            this.phoneNumberControl.Validating +=new CancelEventHandler(phoneNumberControl_Validating);
            // 
            // EmployerPhoneEntryDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.ClientSize = new System.Drawing.Size( 514, 173 );
            this.ControlBox = false;
            this.Controls.Add( this.panel2 );
            this.Controls.Add( this.panel1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmployerPhoneEntryDialog";
            this.Padding = new System.Windows.Forms.Padding( 5 );
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Phone Entry For Employer Address";
            this.panel1.ResumeLayout( false );
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout( false );
            this.ResumeLayout( false );

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
        public EmployerPhoneEntryDialog()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private Panel panel1;
        private LoggingButton okButton;
        private LoggingButton cancelButton;
        private Label optionalTextlabel;
        private Panel panel2;
        private Label labelPhone;
        private Label labelAddress;
        private Label labelEmployer;
        private MaskedEditTextBox mtblabelEmployerAddressText;
        private PhoneNumberControl phoneNumberControl;
        private MaskedEditTextBox mtblabelEmployerNameText;
        #endregion

        #region Constants
        #endregion	
	}
}
