using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.EmploymentViews
{
    /// <summary>
    /// Summary description for SelectEmployerAddress.
    /// </summary>
    public class SelectEmployerAddress : ControlView
    {
        #region Events
        public event EventHandler AddressSelected;
        public event EventHandler NoAddressChecked;
        public event EventHandler NoAddressUnchecked;
        #endregion

        #region Event Handlers
        private void listBox_Addresses_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedIndex != -1)
            {
                selectedContactPoint = lb.SelectedItem as ContactPoint;
                BreadCrumbLogger.GetInstance.Log("employer address selected " + selectedContactPoint.Address.OneLineAddressLabel());
            }

            if (AddressSelected != null)
            {
                AddressSelected(this, new LooseArgs(lb.SelectedIndex != -1));
            }
        }

        private void checkBoxSelectNoAddress_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.checkBoxSelectNoAddress.Checked)
            {
                this.listBox_Addresses.SelectedIndex = -1;
                this.NoAddressChecked(null, EventArgs.Empty);
            }
            else
            {
                this.NoAddressUnchecked(null, EventArgs.Empty);
            }
            listBox_Addresses.Enabled = !checkBoxSelectNoAddress.Checked;
            this.SelectNoAddressChecked = this.checkBoxSelectNoAddress.Checked;
        }

        private void listBox_Addresses_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                RenderInListItem(e, (ContactPoint)listBox_Addresses.Items[e.Index]);
            }
        }

        private void RenderInListItem(DrawItemEventArgs e, ContactPoint cp)
        {
            Address address = cp.Address;
            Color textColor = SystemColors.ControlText;

            // render background and select text color based on selected and focus states
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                {
                    e.DrawBackground();
                    e.DrawFocusRectangle();
                }
                else
                {
                    using (Brush b = new SolidBrush(SystemColors.InactiveCaption))
                    {
                        e.Graphics.FillRectangle(b, e.Bounds);
                    }
                }
                textColor = SystemColors.HighlightText;
            }
            else
            {
                e.DrawBackground();
            }

            if (this.SelectNoAddressChecked)
            {
                textColor = SystemColors.GrayText;
            }

            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                int top = 2;

                renderLine(e, address.Address1, textBrush, ref top);
                string addr2 = address.Address2;
                string postCode;

                if (addr2.Length > 0)
                {
                    renderLine(e, address.Address2, textBrush, ref top);
                }
                if (address.ZipCode.PostalCode.Length == 9)
                {
                    postCode = String.Format("{0}-{1}", address.ZipCode.ZipCodePrimary, address.ZipCode.ZipCodeExtended);
                }
                else
                {
                    postCode = address.ZipCode.PostalCode;
                }
                string cityLine = String.Format
                    ("{0}, {1} {2}"
                    , address.City
                    , address.State
                    , postCode
                    );
                renderLine(e, cityLine, textBrush, ref top);

                if (cp.PhoneNumber != null && cp.PhoneNumber.Number != String.Empty)
                {
                    string phoneLine = cp.PhoneNumber.AsFormattedString();
                    renderLine(e, phoneLine, textBrush, ref top);
                }
            }
        }

        private void renderLine(DrawItemEventArgs e, string text, SolidBrush TextBrush, ref int top)
        {
            if (top + e.Font.Height < e.Bounds.Height)
            {
                e.Graphics.DrawString(
                    text,
                    e.Font,
                    TextBrush,
                    e.Bounds.Left,
                    e.Bounds.Top + top);

                top += e.Font.Height;
            }
        }

        private void buttonNewAddress_Click(object sender, EventArgs e)
        {
            var currentUser = User.GetCurrent();
            var facility = currentUser.Facility;
            var ruleEngine = RuleEngine.GetInstance();
            
            var formAddressVerification = new EmployerFormAddressVerification(facility, ruleEngine);
            formAddressVerification.Model = newAddress;
            formAddressVerification.Owner = this.FindForm();
            this.FindForm().Hide();
            formAddressVerification.UpdateView();

            try
            {
                DialogResult dialogresult = formAddressVerification.ShowDialog(this);

                if (dialogresult == DialogResult.OK)
                {
                    UpdateContactPointAddress(formAddressVerification.i_AddressSelected);

                    EmployerPhoneEntryDialog empPhoneEntry = new EmployerPhoneEntryDialog();
                    empPhoneEntry.Model = this.Model_Employer;
                    empPhoneEntry.UpdateView();

                    try
                    {
                        DialogResult empDialogresult = empPhoneEntry.ShowDialog(this);

                        if (empDialogresult == DialogResult.OK)
                        {
                            this.Model_Employer.AddContactPoint(
                                empPhoneEntry.Model_Employer.PartyContactPoint);

                            DisplayAddressesForEmployer(this.Model_Employer, true);
                            if (formAddressVerification.Owner != null)
                                formAddressVerification.Owner.DialogResult = DialogResult.OK;
                        }
                        this.Refresh();
                    }
                    finally
                    {
                        if (this.FindForm() != null)
                        {
                            this.FindForm().Cursor = Cursors.Default;
                        }
                        empPhoneEntry.Dispose();
                    }

                    if (formAddressVerification.Owner != null)
                    {
                        formAddressVerification.Owner.Show();
                    }
                }
            }
            finally
            {
                formAddressVerification.Dispose();
            }

        }

        private void listBox_Addresses_Enter(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.Items.Count == 0)
            {
                return;
            }
            ListBox.SelectedIndexCollection collection = lb.SelectedIndices;
            if (collection.Count == 0)
            {
                lb.SetSelected(0, true);
            }
            else if (lb.Items.Count > 0)
            {
                lb.SetSelected(0, true);
            }
            lb.Refresh();
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            DisplayAddressesForEmployer(this.Model_Employer, false);
        }

        private void DisplayAddressesForEmployer(Employer employer, bool newAddressCreated)
        {
            listBox_Addresses.Items.Clear();
            buttonNewAddress.Enabled = false;
            checkBoxSelectNoAddress.Enabled = false;
            
            if (employer != null)
            {
                foreach (ContactPoint cp in employer.ContactPoints)
                {
                    listBox_Addresses.Items.Add(cp);
                }

                if (listBox_Addresses.Items.Count > 0)
                {
                    if (newAddressCreated)
                    {
                        listBox_Addresses.SetSelected(listBox_Addresses.Items.Count - 1, true);
                    }
            
                    SetControlState(true);
                    checkBoxSelectNoAddress.Enabled = true;
                }
            }
        }

        public bool IsAddressSelected()
        {
            bool result = false;
            if (listBox_Addresses.SelectedIndex != -1)
            {
                result = true;
            }
            return result;
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

        #region Properties
        public Employer Model_Employer
        {
            private get
            {
                return (Employer)this.Model;
            }
            set
            {
                this.Model = value;
            }
        }
        
        public Address SelectedAddress
        {
            get
            {
                Address result = new Address(string.Empty, string.Empty, string.Empty,
                                new ZipCode(string.Empty), new State(), new Country());

                if (listBox_Addresses.SelectedIndex > -1 && listBox_Addresses.Items.Count > 0)
                {
                    ContactPoint cp = (ContactPoint)listBox_Addresses.Items[listBox_Addresses.SelectedIndex];
                    result = cp.Address;
                }
                return result;
            }
        }

        public ContactPoint SelectedContactPoint
        {
            get
            {
                ContactPoint result = new ContactPoint();
                if (listBox_Addresses.SelectedIndex > -1 && listBox_Addresses.Items.Count > 0)
                {
                    ContactPoint cp = (ContactPoint)listBox_Addresses.Items[listBox_Addresses.SelectedIndex];
                    result = cp;
                }
                return result;
            }
        }

        public bool ButtonEnabled
        {
            set
            {
                this.buttonNewAddress.Enabled = value;
            }
        }

        public bool SelectNoAddressChecked
        {
            get
            {
                return i_SelectNoAddressChecked;
            }
            set
            {
                i_SelectNoAddressChecked = value;
            }
        }
        #endregion

        #region Private Methods
        private void UpdateContactPointAddress(Address address)
        {
            if (Model_Employer.PartyContactPoint.Address == null)
            {
                Model_Employer.PartyContactPoint.Address =
                    new Address();
            }

            Model_Employer.PartyContactPoint.Address.Address1 = address.Address1;
            Model_Employer.PartyContactPoint.Address.Address2 = address.Address2;
            Model_Employer.PartyContactPoint.Address.City = address.City;
            Model_Employer.PartyContactPoint.Address.Country = address.Country;
            Model_Employer.PartyContactPoint.Address.County = address.County;
            Model_Employer.PartyContactPoint.Address.ZipCode.PostalCode = address.ZipCode.PostalCode;
            Model_Employer.PartyContactPoint.Address.State = address.State;
        }


        private void SetControlState(bool state)
        {
            Trace.WriteLine("SetControlState( " + state + " )");
            this.buttonNewAddress.Enabled = state;
        }
        

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelListbox = new System.Windows.Forms.Panel();
            this.listBox_Addresses = new System.Windows.Forms.ListBox();
            this.panelSelectNoAddressChkbox = new System.Windows.Forms.Panel();
            this.checkBoxSelectNoAddress = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonNewAddress = new LoggingButton();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panelListbox.SuspendLayout();
            this.panelSelectNoAddressChkbox.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panelListbox);
            this.panel2.Controls.Add(this.panelSelectNoAddressChkbox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.DockPadding.Bottom = 2;
            this.panel2.DockPadding.Left = 10;
            this.panel2.DockPadding.Right = 10;
            this.panel2.DockPadding.Top = 2;
            this.panel2.Location = new System.Drawing.Point(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(338, 232);
            this.panel2.TabIndex = 1;
            // 
            // panelListbox
            // 
            this.panelListbox.Controls.Add(this.listBox_Addresses);
            this.panelListbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelListbox.Location = new System.Drawing.Point(10, 2);
            this.panelListbox.Name = "panelListbox";
            this.panelListbox.Size = new System.Drawing.Size(318, 198);
            this.panelListbox.TabIndex = 0;
            // 
            // listBox_Addresses
            // 
            this.listBox_Addresses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Addresses.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox_Addresses.IntegralHeight = false;
            this.listBox_Addresses.ItemHeight = 45;
            this.listBox_Addresses.Location = new System.Drawing.Point(0, 0);
            this.listBox_Addresses.Name = "listBox_Addresses";
            this.listBox_Addresses.ScrollAlwaysVisible = true;
            this.listBox_Addresses.Size = new System.Drawing.Size(318, 198);
            this.listBox_Addresses.TabIndex = 0;
            this.listBox_Addresses.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_Addresses_DrawItem);
            this.listBox_Addresses.Enter += new System.EventHandler(this.listBox_Addresses_Enter);
            this.listBox_Addresses.SelectedIndexChanged += new System.EventHandler(this.listBox_Addresses_SelectedIndexChanged);
            // 
            // panelSelectNoAddressChkbox
            // 
            this.panelSelectNoAddressChkbox.Controls.Add(this.checkBoxSelectNoAddress);
            this.panelSelectNoAddressChkbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSelectNoAddressChkbox.Location = new System.Drawing.Point(10, 200);
            this.panelSelectNoAddressChkbox.Name = "panelSelectNoAddressChkbox";
            this.panelSelectNoAddressChkbox.Size = new System.Drawing.Size(318, 30);
            this.panelSelectNoAddressChkbox.TabIndex = 2;
            // 
            // checkBoxSelectNoAddress
            // 
            this.checkBoxSelectNoAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxSelectNoAddress.Location = new System.Drawing.Point(0, 0);
            this.checkBoxSelectNoAddress.Name = "checkBoxSelectNoAddress";
            this.checkBoxSelectNoAddress.Size = new System.Drawing.Size(318, 30);
            this.checkBoxSelectNoAddress.TabIndex = 2;
            this.checkBoxSelectNoAddress.Text = "Select no address";
            this.checkBoxSelectNoAddress.CheckStateChanged += new System.EventHandler(this.checkBoxSelectNoAddress_CheckStateChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.buttonNewAddress);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(338, 30);
            this.panel3.TabIndex = 1;
            // 
            // buttonNewAddress
            // 
            this.buttonNewAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNewAddress.Enabled = false;
            this.buttonNewAddress.Location = new System.Drawing.Point(225, 4);
            this.buttonNewAddress.Name = "buttonNewAddress";
            this.buttonNewAddress.Size = new System.Drawing.Size(103, 23);
            this.buttonNewAddress.TabIndex = 1;
            this.buttonNewAddress.Text = "New &Address...";
            this.buttonNewAddress.Click += new System.EventHandler(this.buttonNewAddress_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(253, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Select an address, or create one if needed.";
            // 
            // SelectEmployerAddress
            // 
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Name = "SelectEmployerAddress";
            this.Size = new System.Drawing.Size(338, 262);
            this.panel2.ResumeLayout(false);
            this.panelListbox.ResumeLayout(false);
            this.panelSelectNoAddressChkbox.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Constructors and Finalization
        public SelectEmployerAddress()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private LoggingButton buttonNewAddress;

        private CheckBox checkBoxSelectNoAddress;

        private ListBox listBox_Addresses;

        private Label label2;

        private Panel panel2;
        private Panel panel3;
        private Panel panelSelectNoAddressChkbox;
        private Panel panelListbox;

        private ContactPoint selectedContactPoint;

        private bool i_SelectNoAddressChecked = false;
        private Address newAddress = new Address(String.Empty, String.Empty, String.Empty, new ZipCode(String.Empty),
                                                                            new State(), new Country());
        #endregion

        #region Constants
        private const string NEW_EMPLOYER_ADDRESS_CAPTION = "Warning for Create a New Employer Address";
        #endregion


    }
}
