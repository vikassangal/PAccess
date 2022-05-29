using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for PassportView.
    /// </summary>
    public class PassportView : ControlView
    {
        #region Events

        public event EventHandler PassportNumberChanged;

        #endregion

        #region Event Handlers
        private void PassportView_Disposed(object sender, EventArgs e)
        {
            UnRegisterEvents();
        }
        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>
        private void PassportCountryRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(cmbIssuingCountry);
        }

        private void mtbPassportNumber_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(this.mtbPassportNumber);
            UIColors.SetNormalBgColor(this.cmbIssuingCountry);

            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if (Model_Account != null
                && Model_Account.Patient != null
                && Model_Account.Patient.Passport != null)
            {
                // SR 39493 - change requirement, remove restriction on passport number field
                if ( mtb.Text.Trim().Length > 12 )
                {
                    Model_Account.Patient.Passport.Number = mtb.Text.Trim().Substring(0, 12);
                }
                else
                {
                    Model_Account.Patient.Passport.Number = mtb.Text.Trim();
                }
                
            }

            if (this.PassportNumberChanged != null)
            {
                this.PassportNumberChanged(this, null);
            }
            this.CheckForRequiredFields();
        }
       
        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigurePassport(mtbPassportNumber);
        }
        
        private void cmbIssuingCountry_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(this.cmbIssuingCountry); 
            RuleEngine.GetInstance().EvaluateRule(typeof(PassportCountryRequired), this.Model); 
        }

        private void cmbIssuingCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor(this.cmbIssuingCountry);
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex != -1)
            {
                Model_Account.Patient.Passport.Country = cb.SelectedItem as Country;
            }

            RuleEngine.GetInstance().EvaluateRule( typeof( PassportCountryRequired ), this.Model );
        }
        #endregion

        #region Methods

        public override void UpdateView()
        {
            if (loadingModelData)
            {
                CountryComboHelper = new ReferenceValueComboBox(this.cmbIssuingCountry);
                loadingModelData = false;
                PopulateCountriesComboBox();

                if (mtbPassportNumber.Enabled && Model_Account != null)
                {
                    mtbPassportNumber.UnMaskedText = Model_Account.Patient.Passport.Number.TrimEnd();

                    if (Model_Account.Patient.Passport.Country != null
                        && Model_Account.Patient.Passport.Country.Code != string.Empty)
                    {
                        cmbIssuingCountry.SelectedIndexChanged -= new EventHandler(this.cmbIssuingCountry_SelectedIndexChanged);
                        cmbIssuingCountry.SelectedItem = Model_Account.Patient.Passport.Country;
                        cmbIssuingCountry.SelectedIndexChanged += new EventHandler(this.cmbIssuingCountry_SelectedIndexChanged);
                    }
                }
                else if (Model_Account != null)
                {
                    Model_Account.Patient.Passport = new Passport();
                }
            }

            this.CheckPassportRules();
        }

        public void CheckForNewBornActivity()
        {
            if (this.Model_Account != null && this.Model_Account.Activity.IsNewBornRelatedActivity())
            {
                UIColors.SetNormalBgColor(this.mtbPassportNumber);
                UIColors.SetNormalBgColor(this.cmbIssuingCountry);
                this.mtbPassportNumber.UnMaskedText = String.Empty;
                this.mtbPassportNumber.Enabled = false;
                this.cmbIssuingCountry.SelectedItem = new Country();
                this.cmbIssuingCountry.Enabled = false;
            }
        }

        public void CheckForValidDriverLicense()
        {
            if (this.Model_Account != null && this.Model_Account.Patient != null &&
                this.Model_Account.Patient.DriversLicense != null &&
                this.Model_Account.Patient.DriversLicense.Number != null &&
                this.Model_Account.Patient.DriversLicense.Number.Trim() != string.Empty)
            {
                this.mtbPassportNumber.UnMaskedText = string.Empty;
                this.mtbPassportNumber.Enabled = false;
                this.cmbIssuingCountry.SelectedIndexChanged -= new EventHandler(this.cmbIssuingCountry_SelectedIndexChanged);
                this.cmbIssuingCountry.SelectedItem = new Country();
                this.cmbIssuingCountry.SelectedIndexChanged += new EventHandler(this.cmbIssuingCountry_SelectedIndexChanged);
                UIColors.SetNormalBgColor(this.cmbIssuingCountry);
                this.cmbIssuingCountry.Enabled = false;
                this.Model_Account.Patient.Passport = new Passport();
            }
            else
            {
                this.mtbPassportNumber.Enabled = true;
                this.cmbIssuingCountry.Enabled = true;
            }
        }
        #endregion

        #region Properties
        public Account Model_Account
        {
            private get
            {
                return (Account)this.Model;
            }
            set
            {
                this.Model = value;
            }
        }

        public MaskedEditTextBox TextBox
        {
            get
            {
                return mtbPassportNumber;
            }
        }

        public ComboBox ComboBox
        {   // May need to be used for searching the control
            get
            {
                return cmbIssuingCountry;
            }
        }

        public string GroupBoxText
        {   // Allows the setting of custom text
            set
            {
                groupBox.Text = value;
            }
        }

        private RuleEngine RuleEngine
        {
            get
            {
                if (i_RuleEngine == null)
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }
        #endregion

        #region Private Methods
        private void CheckPassportRules()
        {
            UIColors.SetNormalBgColor(mtbPassportNumber);
            UIColors.SetNormalBgColor(cmbIssuingCountry);

            if (!i_Registered)
            {
                i_Registered = true;

                RuleEngine.GetInstance().RegisterEvent(typeof(PassportCountryRequired), this.Model, new EventHandler(PassportCountryRequiredEventHandler));

            }

            CheckForRequiredFields();
        }

        /// <summary>
        /// CheckForRequiredFields - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void CheckForRequiredFields()
        {
            RuleEngine.EvaluateRule(typeof(PassportCountryRequired), this.Model_Account);
        }

        private void PopulateCountriesComboBox()
        {
            IAddressBroker broker = new AddressBrokerProxy();

            if (this.cmbIssuingCountry.Items.Count == 0)
            {
                CountryComboHelper.PopulateWithCollection(broker.AllCountries(this.Model_Account.Facility.Oid));
            }
        }

        private void UnRegisterEvents()
        {
            i_Registered = false;
            RuleEngine.GetInstance().UnregisterEvent(typeof(PassportCountryRequired), this.Model, new EventHandler(PassportCountryRequiredEventHandler));
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.cmbIssuingCountry = new System.Windows.Forms.ComboBox();
            this.mtbPassportNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblIssuingCountry = new System.Windows.Forms.Label();
            this.lblPassportNumber = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.cmbIssuingCountry);
            this.groupBox.Controls.Add(this.mtbPassportNumber);
            this.groupBox.Controls.Add(this.lblIssuingCountry);
            this.groupBox.Controls.Add(this.lblPassportNumber);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(300, 84);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Passport";
            // 
            // cmbIssuingCountry
            // 
            this.cmbIssuingCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIssuingCountry.Location = new System.Drawing.Point(100, 50);
            this.cmbIssuingCountry.Name = "cmbIssuingCountry";
            this.cmbIssuingCountry.Size = new System.Drawing.Size(187, 21);
            this.cmbIssuingCountry.TabIndex = 3;
            this.cmbIssuingCountry.Validating += new System.ComponentModel.CancelEventHandler(this.cmbIssuingCountry_Validating);
            this.cmbIssuingCountry.SelectedIndexChanged += new System.EventHandler(this.cmbIssuingCountry_SelectedIndexChanged);
            // 
            // mtbPassportNumber
            // 
            this.mtbPassportNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbPassportNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbPassportNumber.KeyPressExpression = "^.*";
            this.mtbPassportNumber.Location = new System.Drawing.Point(100, 18);
            this.mtbPassportNumber.Mask = "";
            this.mtbPassportNumber.MaxLength = 12;
            this.mtbPassportNumber.Name = "mtbPassportNumber";
            this.mtbPassportNumber.Size = new System.Drawing.Size(115, 20);
            this.mtbPassportNumber.TabIndex = 2;
            this.mtbPassportNumber.ValidationExpression = "^.*";
            this.mtbPassportNumber.Validating += new System.ComponentModel.CancelEventHandler(this.mtbPassportNumber_Validating);
            // 
            // lblIssuingCountry
            // 
            this.lblIssuingCountry.Location = new System.Drawing.Point(8, 53);
            this.lblIssuingCountry.Name = "lblIssuingCountry";
            this.lblIssuingCountry.Size = new System.Drawing.Size(86, 23);
            this.lblIssuingCountry.TabIndex = 0;
            this.lblIssuingCountry.Text = "Issuing country:";
            // 
            // lblPassportNumber
            // 
            this.lblPassportNumber.Location = new System.Drawing.Point(8, 21);
            this.lblPassportNumber.Name = "lblPassportNumber";
            this.lblPassportNumber.Size = new System.Drawing.Size(50, 23);
            this.lblPassportNumber.TabIndex = 0;
            this.lblPassportNumber.Text = "Number:";
            // 
            // PassportView
            // 
            this.Controls.Add(this.groupBox);
            this.Name = "PassportView";
            this.Size = new System.Drawing.Size(300, 84);
            this.Disposed += new System.EventHandler(this.PassportView_Disposed);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private properties

        private ReferenceValueComboBox CountryComboHelper
        {
            get
            {
                return i_CountryComboHelper;
            }
            set
            {
                i_CountryComboHelper = value;
            }
        }

        #endregion

        #region Construction and Finalization
        public PassportView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            ConfigureControls();
        }

        private static void ShowErrorMessageBox(string msg)
        {
            MessageBox.Show(msg, "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
            MessageBoxDefaultButton.Button1);
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

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private ComboBox cmbIssuingCountry;
        private GroupBox groupBox;
        private Label lblPassportNumber;
        private Label lblIssuingCountry;
        private MaskedEditTextBox mtbPassportNumber;
        private RuleEngine i_RuleEngine;
        private bool loadingModelData = true;
        private ReferenceValueComboBox i_CountryComboHelper;
        private bool i_Registered = false;

        #endregion

        #region Constants
        #endregion
    }
}
