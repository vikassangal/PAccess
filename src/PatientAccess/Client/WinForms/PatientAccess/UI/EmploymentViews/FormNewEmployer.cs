using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.EmploymentViews
{
    /// <summary>
    /// Summary description for FormNewEmployer.
    /// </summary>
    public class FormNewEmployer : TimeOutFormView
    {
        #region Event Handlers
        private void FormNewEmployer_Load(object sender, EventArgs e)
        {
            ValidateDialog();
        }

        private void button_NewAddress_Click(object sender, EventArgs e)
        {
            OnNewAddress();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            OnNewEmployer();
        }


        private void textBox_EmployerName_TextChanged(object sender, EventArgs e)
        {
            ValidateDialog();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((this.ActiveControl == textBox_EmployerName) &&
                (textBox_EmployerName.SelectionStart == 0) &&
                (keyData == Keys.Space)
                )
            {
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        #endregion

        #region Methods

        private void OnNewEmployer()
        {
            bool addEmpSuccessful = true;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                //AddEmployer();
                addEmpSuccessful = AddEmployer();
            }
            finally
            {
                this.Cursor = Cursors.Default;

                if (!addEmpSuccessful)
                {
                    this.Owner.Show();
                }
            }
        }

        private void OnNewAddress()
        {
            this.FindForm().Owner.BringToFront();
            this.FindForm().Hide();
            this.FindForm().Owner.Hide();

            if (AddEmployer())
            {
                var currentUser = User.GetCurrent();
                var facility = currentUser.Facility;
                var ruleEngine = RuleEngine.GetInstance();
                var formAddressVerification = new EmployerFormAddressVerification(facility, ruleEngine);

                formAddressVerification.Model = newAddress;

                formAddressVerification.Owner = this.FindForm();
                formAddressVerification.UpdateView();

                try
                {
                    DialogResult dialogresult = formAddressVerification.ShowDialog(this);

                    if (dialogresult == DialogResult.OK)
                    {
                        UpdateContactPointAddress(formAddressVerification.i_AddressSelected);


                        EmployerPhoneEntryDialog empPhoneEntry = new EmployerPhoneEntryDialog();
                        empPhoneEntry.Model = this.NewEmployer;
                        empPhoneEntry.UpdateView();

                        try
                        {
                            DialogResult empDialogresult = empPhoneEntry.ShowDialog(this);

                            if (empDialogresult == DialogResult.OK)
                            {
                                this.NewEmployer.PartyContactPoint =
                                    empPhoneEntry.Model_Employer.PartyContactPoint;
                                this.UpdateView();
                            }
                        }
                        finally
                        {
                            empPhoneEntry.Dispose();
                        }
                        this.Refresh();
                        this.DialogResult = DialogResult.OK;
                    }
                }
                finally
                {
                    formAddressVerification.Dispose();
                }
            }
            else
            {
                this.Owner.Show();
            }
        }


        #endregion

        #region Properties
        public Employer NewEmployer
        {
            get
            {
                return i_NewEmployer;
            }
            private set
            {
                i_NewEmployer = value;
            }
        }

        public Employer Model_Employer
        {
            get
            {
                return (Employer)this.Model;
            }
            set
            {
                this.Model = value;
            }
        }

        public string EmployerName
        {
            get
            {
                return textBox_EmployerName.Text;
            }
        }

        public Activity Activity
        {
            private get
            {
                return i_activity;
            }
            set
            {
                i_activity = value;
            }
        }
        #endregion

        #region Private Methods
        private bool AddEmployer()
        {
            bool rc = true;

            NewEmployer = new Employer();
            NewEmployer.Name = textBox_EmployerName.Text;

            if ((NewEmployer.Name.Length > 0) && (NewEmployer.Name[0] == ' '))
            {
                MessageBox.Show(NOLEADINGSPACE_ERRORMSG, ERRORCAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                rc = false;
            }

            if (ValidateUseOfEmployerName(NewEmployer.Name))
            {
                NewEmployer.EmployerCode = -1;
                IEmployerBroker broker = BrokerFactory.BrokerOfType<IEmployerBroker>();
                NewEmployer.EmployerCode = broker.AddEmployerForApproval(NewEmployer,
                                                                          User.GetCurrent().Facility.Code,
                                                                          User.GetCurrent().SecurityUser.UPN);
            }
            else
            {
                rc = false;
            }
            return rc;
        }


        private void UpdateContactPointAddress(Address address)
        {
            if (this.NewEmployer.PartyContactPoint.Address == null)
            {
                NewEmployer.PartyContactPoint.Address =
                    new Address();
            }

            NewEmployer.PartyContactPoint.Address.Address1 = address.Address1;
            NewEmployer.PartyContactPoint.Address.Address2 = address.Address2;
            NewEmployer.PartyContactPoint.Address.City = address.City;
            NewEmployer.PartyContactPoint.Address.Country = address.Country;
            NewEmployer.PartyContactPoint.Address.County = address.County;
            NewEmployer.PartyContactPoint.Address.ZipCode.PostalCode = address.ZipCode.PostalCode;
            NewEmployer.PartyContactPoint.Address.State = address.State;
        }

        private bool ValidateUseOfEmployerName(string newEmployerName)
        {
            bool rc = true;

            SortedList employers = EmployerBroker.AllEmployersWith(User.GetCurrent().Facility.Oid,
                newEmployerName);

            if (employers.Count > 0)
            {
                string msg = String.Format(
                    EXISTING_EMPLOYERS_WARNING,
                    employers.Count,
                    newEmployerName);
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                DialogResult dialogResult = MessageBox.Show(
                    msg,
                    EXISTING_EMPLOYERS_CAPTION,
                    buttons,
                    MessageBoxIcon.Warning);

                if (dialogResult != DialogResult.Yes)
                {
                    rc = false;
                }
            }

            return rc;
        }

        private void ValidateDialog()
        {
            Validation.TextBoxValidation(textBox_EmployerName, EmployerNameIsValid, REQUIRED_EMPLOYERNAME_MSG);

            if (DialogIsValid == false && Activity != null)
            {
                if (Activity.GetType().Equals(typeof (RegistrationActivity)) ||
                    Activity.GetType().Equals(typeof (PreRegistrationActivity)) ||
                    Activity.GetType().Equals(typeof (MaintenanceActivity)) ||
                    Activity.GetType().Equals(typeof (PostMSERegistrationActivity)) ||
                    Activity.GetType().Equals(typeof (AdmitNewbornActivity))||
                    Activity.GetType().Equals( typeof( ShortMaintenanceActivity ) ) ||
                    Activity.GetType().Equals( typeof( ShortPreRegistrationActivity ) )||
                    Activity.GetType().Equals( typeof( ShortRegistrationActivity ) ) ||
                     Activity.GetType().Equals(typeof(QuickAccountCreationActivity)) ||
                    Activity.GetType().Equals(typeof(QuickAccountMaintenanceActivity ))
                    )
                {
                    UIColors.SetRequiredBgColor(textBox_EmployerName);
                }
            }
            else if (this.textBox_EmployerName.Text.Length > 0)
            {
                UIColors.SetNormalBgColor(this.textBox_EmployerName);
                Refresh();
            }

            button_OK.Enabled = DialogIsValid;
            button_NewAddress.Enabled = DialogIsValid;
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureEmployerName( this.textBox_EmployerName );
        }

        #region Private Properties
        private bool DialogIsValid
        {
            get
            {
                return EmployerNameIsValid;
            }
        }

        private bool EmployerNameIsValid
        {
            get
            {
                return textBox_EmployerName.Text.Length > 0;
            }
        }

        private IEmployerBroker EmployerBroker
        {
            get
            {
                return i_EmployerBroker;
            }
            set
            {
                i_EmployerBroker = value;
            }
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_NewAddress = new LoggingButton();
            this.button_Cancel = new LoggingButton();
            this.button_OK = new LoggingButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox_EmployerName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button_NewAddress);
            this.panel1.Controls.Add(this.button_Cancel);
            this.panel1.Controls.Add(this.button_OK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(348, 30);
            this.panel1.TabIndex = 3;
            // 
            // button_NewAddress
            // 
            this.button_NewAddress.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_NewAddress.Location = new System.Drawing.Point(10, 4);
            this.button_NewAddress.Name = "button_NewAddress";
            this.button_NewAddress.Size = new System.Drawing.Size(93, 23);
            this.button_NewAddress.TabIndex = 1;
            this.button_NewAddress.Text = "New A&ddress...";
            this.button_NewAddress.Click += new System.EventHandler(this.button_NewAddress_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(262, 4);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.TabIndex = 3;
            this.button_Cancel.Text = "Cancel";
            // 
            // button_OK
            // 
            this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_OK.Location = new System.Drawing.Point(179, 4);
            this.button_OK.Name = "button_OK";
            this.button_OK.TabIndex = 2;
            this.button_OK.Text = "OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox_EmployerName);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(348, 33);
            this.panel2.TabIndex = 1;
            // 
            // textBox_EmployerName
            // 
            this.textBox_EmployerName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_EmployerName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.textBox_EmployerName.Location = new System.Drawing.Point(100, 7);
            this.textBox_EmployerName.Mask = "";
            this.textBox_EmployerName.MaxLength = Employer.PBAR_EMP_NAME_LENGTH;
            this.textBox_EmployerName.Name = "textBox_EmployerName";
            this.textBox_EmployerName.Size = new System.Drawing.Size(180, 20);
            this.textBox_EmployerName.TabIndex = 0;
            this.textBox_EmployerName.TextChanged += new System.EventHandler(this.textBox_EmployerName_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.TabIndex = 0;
            this.label1.Text = "Employer Name:";
            // 
            // FormNewEmployer
            // 
            this.AcceptButton = this.button_NewAddress;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(348, 63);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNewEmployer";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Employer";
            this.Load += new System.EventHandler(this.FormNewEmployer_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Constructors and Finalization
        public FormNewEmployer()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            ConfigureControls();
            EmployerBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();

            ValidateDialog();

            base.EnableThemesOn(this);
        }

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
        private Panel panel1;
        private LoggingButton button_Cancel;
        private LoggingButton button_OK;
        private Panel panel2;
        private Label label1;
        private LoggingButton button_NewAddress;
        private MaskedEditTextBox textBox_EmployerName;

        private Activity i_activity;
        private Employer i_NewEmployer = null;
        private IEmployerBroker i_EmployerBroker;
        private Address newAddress = new Address(String.Empty, String.Empty, String.Empty, new ZipCode(String.Empty),
                                                                    new State(), new Country());
        #endregion

        #region Constants
        private const string
            REQUIRED_EMPLOYERNAME_MSG = "Employer Name is required",
            EXISTING_EMPLOYERS_WARNING = "{0} employers already exist with names that begin with {1}\nDo you want to continue with creating the new employer {1}?",
            EXISTING_EMPLOYERS_CAPTION = "Warning",
            NOLEADINGSPACE_ERRORMSG = "Employer Names cannot begin with a space",
            ERRORCAPTION = "ERROR";
        #endregion
    }
}
