using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    /// <summary>
    /// Summary description for WorkersCompVerifyView.
    /// </summary>
    public class WorkersCompVerifyView : ControlView
    {
        #region Event Handlers
        private void WorkersCompVerifyView_Disposed( object sender, EventArgs e )
        {
            UnRegisterEvents();
        }

        private void WorkersCompClaimAddressVerifiedPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbClaimsAddressVerified );
        }

        private void WorkersCompInsurancePhonePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbInsurancePhone );
        }

        private void WorkersCompClaimNumberPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( this.mtbClaimNumber );
        }

        private void WorkersCompInfoRecvdFromPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbInfoRecvFrom );
        }

        private void btnClearAll_Click( object sender, EventArgs e )
        {
            ResetView();
        }


        private void mtbPPOPricingOrBroker_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.Text != String.Empty )
            {
                pPOPricingOrBroker = mtb.UnMaskedText;
            }
            else
            {
                pPOPricingOrBroker = String.Empty;
            }
        }

        private void mtbClaimNumber_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
        }

        private void mtbClaimNumber_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if( mtb.UnMaskedText != String.Empty )
            {
                claimNumberForIncident = mtb.UnMaskedText;
            }
            else
            {
                claimNumberForIncident = String.Empty;
            }
            Model_Coverage.ClaimNumberForIncident = claimNumberForIncident;
            CheckForRequiredFields();
        }

        private void cmbClaimsAddressVerified_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbClaimsAddressVerified_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbClaimsAddressVerified );
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                claimsAddressVerified = cb.SelectedItem as YesNoFlag;
            }
            else
            {
                claimsAddressVerified = null;
            }
            Model_Coverage.ClaimsAddressVerified = claimsAddressVerified;
            CheckForRequiredFields();
        }

        private void mtbInsurancePhone_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
        }

        private void mtbInsurancePhone_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if( mtb.UnMaskedText != String.Empty )
            {
                insurancePhone = mtb.UnMaskedText;
            }
            else
            {
                insurancePhone = String.Empty;
            }
            Model_Coverage.InsurancePhone = insurancePhone;
            CheckForRequiredFields();
        }

        private void cmbEmployerPaidPremium_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbEmployerPaidPremium_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                employerhasPaidPremiumsToDate = cb.SelectedItem as YesNoFlag;
            }
            else
            {
                employerhasPaidPremiumsToDate = null;
            }
        }

        private void cmbInfoRecvFrom_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbInfoRecvFrom_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbInfoRecvFrom );

            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 && cb.Text.Equals( " " ) == false )
            {
                informationReceivedSource = cb.SelectedItem as InformationReceivedSource;
            }
            else
            {
                informationReceivedSource = null;
            }
            Model_Coverage.InformationReceivedSource = informationReceivedSource;
            CheckForRequiredFields();
        }

        private void mtbRemarks_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                remarks = mtb.UnMaskedText.TrimEnd();
            }
            else
            {
                remarks = String.Empty;
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( loadingModelData )
            {
                loadingModelData = false;

                PopulateClaimsAddressVerifiedComboBox();
                PopulatePaidPremiumsComboBox();
                PopulateInfoRecvComboBox();
                PopulateAttorneyAddressView();

                this.cmbInfoRecvFrom.SelectedItem = Model_Coverage.InformationReceivedSource;
                this.informationReceivedSource = Model_Coverage.InformationReceivedSource;

                this.mtbPPOPricingOrBroker.UnMaskedText = Model_Coverage.PPOPricingOrBroker;
                this.pPOPricingOrBroker = Model_Coverage.PPOPricingOrBroker;

                this.mtbClaimNumber.UnMaskedText = Model_Coverage.ClaimNumberForIncident;
                this.claimNumberForIncident = Model_Coverage.ClaimNumberForIncident;

                this.cmbClaimsAddressVerified.SelectedItem = Model_Coverage.ClaimsAddressVerified;
                this.claimsAddressVerified = Model_Coverage.ClaimsAddressVerified;

                this.mtbInsurancePhone.UnMaskedText = Model_Coverage.InsurancePhone;
                this.insurancePhone = Model_Coverage.InsurancePhone;

                this.cmbEmployerPaidPremium.SelectedItem = Model_Coverage.EmployerhasPaidPremiumsToDate;
                this.employerhasPaidPremiumsToDate = Model_Coverage.EmployerhasPaidPremiumsToDate;

                this.mtbRemarks.UnMaskedText = Model_Coverage.Remarks;
                this.remarks = Model_Coverage.Remarks;

                RuleEngine.GetInstance().RegisterEvent( typeof( WorkersCompClaimAddressVerifiedPreferred ), Model_Coverage, new EventHandler( WorkersCompClaimAddressVerifiedPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( WorkersCompInfoRecvdFromPreferred ), Model_Coverage, new EventHandler( WorkersCompInfoRecvdFromPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( WorkersCompClaimNumberPreferred ), Model_Coverage, new EventHandler( WorkersCompClaimNumberPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( WorkersCompInsurancePhonePreferred ), Model_Coverage, new EventHandler( WorkersCompInsurancePhonePreferredEventHandler ) );

                CheckForRequiredFields();
            }
        }

        public override void UpdateModel()
        {
            if( informationReceivedSource != null )
            {
                Model_Coverage.InformationReceivedSource = informationReceivedSource;
            }

            this.mtbPPOPricingOrBroker_Validating( this.mtbPPOPricingOrBroker, null );
            if( pPOPricingOrBroker != null )
            {
                Model_Coverage.PPOPricingOrBroker = pPOPricingOrBroker;
            }

            this.mtbClaimNumber_Validating( this.mtbClaimNumber, null );
            if( claimNumberForIncident != null )
            {
                Model_Coverage.ClaimNumberForIncident = claimNumberForIncident;
            }

            if( claimsAddressVerified != null )
            {
                Model_Coverage.ClaimsAddressVerified = claimsAddressVerified;
            }

            this.mtbInsurancePhone_Validating( this.mtbInsurancePhone, null );
            if( insurancePhone != null )
            {
                Model_Coverage.InsurancePhone = insurancePhone;
            }

            if( employerhasPaidPremiumsToDate != null )
            {
                Model_Coverage.EmployerhasPaidPremiumsToDate = employerhasPaidPremiumsToDate;
            }

            this.mtbRemarks_Validating( this.mtbRemarks, null );
            if( remarks != null )
            {
                Model_Coverage.Remarks = remarks;
            }

            if( addressView1.Model_ContactPoint != null )
            {
                ContactPoint businessContactPoint = Model_Coverage.Attorney.ContactPointWith(
                    TypeOfContactPoint.NewBusinessContactPointType() );
                businessContactPoint.Address = addressView1.Model_ContactPoint.Address;
            }

            if( this.nameAndPhoneView1.Model_Person != null )
            {
                Model_Coverage.Attorney.AttorneyName = (string)nameAndPhoneView1.Model_Person.Name.FirstName.Trim().Clone();                          
            }
        }
        #endregion

        #region Properties
        [Browsable( false )]
        public WorkersCompensationCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (WorkersCompensationCoverage)this.Model;
            }
        }

        [Browsable( false )]
        public Account Account
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        [Browsable( false )]
        private RuleEngine RuleEngine
        {
            get
            {
                if( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }
        #endregion

        #region Private Methods
        private void PopulateAttorneyAddressView()
        {
            if( this.Model_Coverage != null && Model_Coverage.Attorney != null )
            {
                addressView1.KindOfTargetParty = Model_Coverage.Attorney.GetType();
                addressView1.PatientAccount = Model_Coverage.Account;
                ContactPoint businessContactPoint = new ContactPoint();
                businessContactPoint.Address = Model_Coverage.Attorney.ContactPointWith(
                    TypeOfContactPoint.NewBusinessContactPointType() ).Address;
                addressView1.Model = businessContactPoint;
                addressView1.UpdateView();

                nameAndPhoneView1.Model = Model_Coverage.Attorney;
                nameAndPhoneView1.UpdateView();
            }
        }
        /// <summary>
        /// CheckForRequiredFields - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        private void CheckForRequiredFields()
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompClaimAddressVerifiedPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompInfoRecvdFromPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompClaimNumberPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompInsurancePhonePreferred ), Model_Coverage );

        }

        private void ResetView()
        {
            cmbClaimsAddressVerified.SelectedIndex = -1;
            cmbEmployerPaidPremium.SelectedIndex = -1;
            cmbInfoRecvFrom.SelectedIndex = -1;

            mtbPPOPricingOrBroker.Text = String.Empty;
            mtbClaimNumber.Text = String.Empty;
            mtbInsurancePhone.Text = String.Empty;
            mtbRemarks.Text = String.Empty;

            this.addressView1.ResetView();
            this.nameAndPhoneView1.ResetView();
        }

        private void PopulateInfoRecvComboBox()
        {
            IInfoReceivedSourceBroker broker = BrokerFactory.BrokerOfType<IInfoReceivedSourceBroker>();
            ICollection alist = broker.AllInfoReceivedSources();

            cmbInfoRecvFrom.Items.Clear();

            foreach( InformationReceivedSource o in alist )
            {
                cmbInfoRecvFrom.Items.Add( o );
            }
        }

        private void PopulateClaimsAddressVerifiedComboBox()
        {
            cmbClaimsAddressVerified.Items.Add( blankYesNoFlag );
            cmbClaimsAddressVerified.Items.Add( yesYesNoFlag );
            cmbClaimsAddressVerified.Items.Add( noYesNoFlag );
        }

        private void PopulatePaidPremiumsComboBox()
        {
            cmbEmployerPaidPremium.Items.Add( blankYesNoFlag );
            cmbEmployerPaidPremium.Items.Add( yesYesNoFlag );
            cmbEmployerPaidPremium.Items.Add( noYesNoFlag );
        }

        private void UnRegisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( WorkersCompClaimAddressVerifiedPreferred ), Model_Coverage, WorkersCompClaimAddressVerifiedPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( WorkersCompInfoRecvdFromPreferred ), Model_Coverage, WorkersCompInfoRecvdFromPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( WorkersCompClaimNumberPreferred ), Model_Coverage, WorkersCompClaimNumberPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( WorkersCompInsurancePhonePreferred ), Model_Coverage, WorkersCompInsurancePhonePreferredEventHandler );
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureInsuranceVerificationRemarks( mtbRemarks );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel = new System.Windows.Forms.Panel();
            this.addressView1 = new PatientAccess.UI.AddressViews.AddressView();
            this.nameAndPhoneView1 = new PatientAccess.UI.CommonControls.NameAndPhoneView();
            this.lineLabel1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.cmbInfoRecvFrom = new System.Windows.Forms.ComboBox();
            this.mtbRemarks = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.btnClearAll = new LoggingButton();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.lblStaticInfoRecvFrom = new System.Windows.Forms.Label();
            this.cmbEmployerPaidPremium = new System.Windows.Forms.ComboBox();
            this.lblStaticPaidPremiums = new System.Windows.Forms.Label();
            this.mtbInsurancePhone = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticInsurancePhone = new System.Windows.Forms.Label();
            this.cmbClaimsAddressVerified = new System.Windows.Forms.ComboBox();
            this.lblStaticClaimsVerified = new System.Windows.Forms.Label();
            this.mtbClaimNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticClaimNumber = new System.Windows.Forms.Label();
            this.mtbPPOPricingOrBroker = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticNetwork = new System.Windows.Forms.Label();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.AutoScroll = true;
            this.panel.Controls.Add( this.addressView1 );
            this.panel.Controls.Add( this.nameAndPhoneView1 );
            this.panel.Controls.Add( this.lineLabel1 );
            this.panel.Controls.Add( this.cmbInfoRecvFrom );
            this.panel.Controls.Add( this.mtbRemarks );
            this.panel.Controls.Add( this.btnClearAll );
            this.panel.Controls.Add( this.lblRemarks );
            this.panel.Controls.Add( this.lblStaticInfoRecvFrom );
            this.panel.Controls.Add( this.cmbEmployerPaidPremium );
            this.panel.Controls.Add( this.lblStaticPaidPremiums );
            this.panel.Controls.Add( this.mtbInsurancePhone );
            this.panel.Controls.Add( this.lblStaticInsurancePhone );
            this.panel.Controls.Add( this.cmbClaimsAddressVerified );
            this.panel.Controls.Add( this.lblStaticClaimsVerified );
            this.panel.Controls.Add( this.mtbClaimNumber );
            this.panel.Controls.Add( this.lblStaticClaimNumber );
            this.panel.Controls.Add( this.mtbPPOPricingOrBroker );
            this.panel.Controls.Add( this.lblStaticNetwork );
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point( 0, 0 );
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size( 850, 500 );
            this.panel.TabIndex = 0;
            // 
            // addressView1
            // 
            this.addressView1.Context = null;
            this.addressView1.KindOfTargetParty = null;
            this.addressView1.Location = new System.Drawing.Point( 568, 208 );
            this.addressView1.Model = null;
            this.addressView1.Name = "addressView1";
            this.addressView1.PatientAccount = null;
            this.addressView1.Size = new System.Drawing.Size( 265, 144 );
            this.addressView1.TabIndex = 29;
            // 
            // nameAndPhoneView1
            // 
            this.nameAndPhoneView1.Location = new System.Drawing.Point( 8, 216 );
            this.nameAndPhoneView1.Model = null;
            this.nameAndPhoneView1.Model_Person = null;
            this.nameAndPhoneView1.Name = "nameAndPhoneView1";
            this.nameAndPhoneView1.NameLabel = "Attorney name:";
            this.nameAndPhoneView1.PhoneLabel = "Phone:";
            this.nameAndPhoneView1.Size = new System.Drawing.Size( 464, 56 );
            this.nameAndPhoneView1.TabIndex = 28;
            // 
            // lineLabel1
            // 
            this.lineLabel1.Caption = "";
            this.lineLabel1.Location = new System.Drawing.Point( 8, 184 );
            this.lineLabel1.Name = "lineLabel1";
            this.lineLabel1.Size = new System.Drawing.Size( 832, 18 );
            this.lineLabel1.TabIndex = 27;
            this.lineLabel1.TabStop = false;
            // 
            // cmbInfoRecvFrom
            // 
            this.cmbInfoRecvFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInfoRecvFrom.Location = new System.Drawing.Point( 626, 14 );
            this.cmbInfoRecvFrom.Name = "cmbInfoRecvFrom";
            this.cmbInfoRecvFrom.Size = new System.Drawing.Size( 185, 21 );
            this.cmbInfoRecvFrom.TabIndex = 6;
            this.cmbInfoRecvFrom.DropDown += new System.EventHandler( this.cmbInfoRecvFrom_DropDown );
            this.cmbInfoRecvFrom.SelectedIndexChanged += new System.EventHandler( this.cmbInfoRecvFrom_SelectedIndexChanged );
            // 
            // mtbRemarks
            // 
            this.mtbRemarks.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRemarks.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemarks.Location = new System.Drawing.Point( 551, 48 );
            this.mtbRemarks.Mask = "";
            this.mtbRemarks.MaxLength = 60;
            this.mtbRemarks.Multiline = true;
            this.mtbRemarks.Name = "mtbRemarks";
            this.mtbRemarks.Size = new System.Drawing.Size( 230, 48 );
            this.mtbRemarks.TabIndex = 7;
            this.mtbRemarks.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRemarks_Validating );
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point( 758, 360 );
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.TabIndex = 30;
            this.btnClearAll.Text = "&Clear All";
            this.btnClearAll.Click += new System.EventHandler( this.btnClearAll_Click );
            // 
            // lblRemarks
            // 
            this.lblRemarks.Location = new System.Drawing.Point( 495, 51 );
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size( 56, 23 );
            this.lblRemarks.TabIndex = 26;
            this.lblRemarks.Text = "Remarks:";
            // 
            // lblStaticInfoRecvFrom
            // 
            this.lblStaticInfoRecvFrom.Location = new System.Drawing.Point( 495, 17 );
            this.lblStaticInfoRecvFrom.Name = "lblStaticInfoRecvFrom";
            this.lblStaticInfoRecvFrom.Size = new System.Drawing.Size( 137, 23 );
            this.lblStaticInfoRecvFrom.TabIndex = 0;
            this.lblStaticInfoRecvFrom.Text = "Information received from:";
            // 
            // cmbEmployerPaidPremium
            // 
            this.cmbEmployerPaidPremium.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmployerPaidPremium.Location = new System.Drawing.Point( 209, 150 );
            this.cmbEmployerPaidPremium.Name = "cmbEmployerPaidPremium";
            this.cmbEmployerPaidPremium.Size = new System.Drawing.Size( 50, 21 );
            this.cmbEmployerPaidPremium.TabIndex = 5;
            this.cmbEmployerPaidPremium.DropDown += new System.EventHandler( this.cmbEmployerPaidPremium_DropDown );
            this.cmbEmployerPaidPremium.SelectedIndexChanged += new System.EventHandler( this.cmbEmployerPaidPremium_SelectedIndexChanged );
            // 
            // lblStaticPaidPremiums
            // 
            this.lblStaticPaidPremiums.Location = new System.Drawing.Point( 8, 153 );
            this.lblStaticPaidPremiums.Name = "lblStaticPaidPremiums";
            this.lblStaticPaidPremiums.Size = new System.Drawing.Size( 191, 23 );
            this.lblStaticPaidPremiums.TabIndex = 0;
            this.lblStaticPaidPremiums.Text = "Employer has paid premiums to date:";
            // 
            // mtbInsurancePhone
            // 
            this.mtbInsurancePhone.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbInsurancePhone.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbInsurancePhone.Location = new System.Drawing.Point( 209, 116 );
            this.mtbInsurancePhone.Mask = "";
            this.mtbInsurancePhone.MaxLength = 15;
            this.mtbInsurancePhone.Name = "mtbInsurancePhone";
            this.mtbInsurancePhone.Size = new System.Drawing.Size( 119, 20 );
            this.mtbInsurancePhone.TabIndex = 4;
            this.mtbInsurancePhone.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbInsurancePhone.Validating += new System.ComponentModel.CancelEventHandler( this.mtbInsurancePhone_Validating );
            this.mtbInsurancePhone.Enter += new System.EventHandler( this.mtbInsurancePhone_Enter );
            // 
            // lblStaticInsurancePhone
            // 
            this.lblStaticInsurancePhone.Location = new System.Drawing.Point( 8, 119 );
            this.lblStaticInsurancePhone.Name = "lblStaticInsurancePhone";
            this.lblStaticInsurancePhone.TabIndex = 0;
            this.lblStaticInsurancePhone.Text = "Insurance phone:";
            // 
            // cmbClaimsAddressVerified
            // 
            this.cmbClaimsAddressVerified.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClaimsAddressVerified.Location = new System.Drawing.Point( 209, 82 );
            this.cmbClaimsAddressVerified.Name = "cmbClaimsAddressVerified";
            this.cmbClaimsAddressVerified.Size = new System.Drawing.Size( 50, 21 );
            this.cmbClaimsAddressVerified.TabIndex = 3;
            this.cmbClaimsAddressVerified.DropDown += new System.EventHandler( this.cmbClaimsAddressVerified_DropDown );
            this.cmbClaimsAddressVerified.SelectedIndexChanged += new System.EventHandler( this.cmbClaimsAddressVerified_SelectedIndexChanged );
            // 
            // lblStaticClaimsVerified
            // 
            this.lblStaticClaimsVerified.Location = new System.Drawing.Point( 8, 85 );
            this.lblStaticClaimsVerified.Name = "lblStaticClaimsVerified";
            this.lblStaticClaimsVerified.Size = new System.Drawing.Size( 128, 23 );
            this.lblStaticClaimsVerified.TabIndex = 0;
            this.lblStaticClaimsVerified.Text = "Claims address verified:";
            // 
            // mtbClaimNumber
            // 
            this.mtbClaimNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbClaimNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbClaimNumber.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbClaimNumber.Location = new System.Drawing.Point( 209, 48 );
            this.mtbClaimNumber.Mask = "";
            this.mtbClaimNumber.MaxLength = 15;
            this.mtbClaimNumber.Name = "mtbClaimNumber";
            this.mtbClaimNumber.Size = new System.Drawing.Size( 130, 20 );
            this.mtbClaimNumber.TabIndex = 2;
            this.mtbClaimNumber.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbClaimNumber.Validating += new System.ComponentModel.CancelEventHandler( this.mtbClaimNumber_Validating );
            this.mtbClaimNumber.Enter += new System.EventHandler( this.mtbClaimNumber_Enter );
            // 
            // lblStaticClaimNumber
            // 
            this.lblStaticClaimNumber.Location = new System.Drawing.Point( 8, 51 );
            this.lblStaticClaimNumber.Name = "lblStaticClaimNumber";
            this.lblStaticClaimNumber.Size = new System.Drawing.Size( 144, 23 );
            this.lblStaticClaimNumber.TabIndex = 0;
            this.lblStaticClaimNumber.Text = "Claim number for incident:";
            // 
            // mtbPPOPricingOrBroker
            // 
            this.mtbPPOPricingOrBroker.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbPPOPricingOrBroker.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbPPOPricingOrBroker.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPPOPricingOrBroker.Location = new System.Drawing.Point( 209, 14 );
            this.mtbPPOPricingOrBroker.Mask = "";
            this.mtbPPOPricingOrBroker.MaxLength = 25;
            this.mtbPPOPricingOrBroker.Name = "mtbPPOPricingOrBroker";
            this.mtbPPOPricingOrBroker.Size = new System.Drawing.Size( 263, 20 );
            this.mtbPPOPricingOrBroker.TabIndex = 1;
            this.mtbPPOPricingOrBroker.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPPOPricingOrBroker.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPPOPricingOrBroker_Validating );
            // 
            // lblStaticNetwork
            // 
            this.lblStaticNetwork.Location = new System.Drawing.Point( 8, 17 );
            this.lblStaticNetwork.Name = "lblStaticNetwork";
            this.lblStaticNetwork.Size = new System.Drawing.Size( 208, 23 );
            this.lblStaticNetwork.TabIndex = 0;
            this.lblStaticNetwork.Text = "PPO network, pricing network, or broker:";
            // 
            // WorkersCompVerifyView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panel );
            this.Name = "WorkersCompVerifyView";
            this.Size = new System.Drawing.Size( 850, 500 );
            this.Disposed += new System.EventHandler( this.WorkersCompVerifyView_Disposed );
            this.panel.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public WorkersCompVerifyView()
        {
            InitializeComponent();

            ConfigureControls();

            loadingModelData = true;
            base.EnableThemesOn( this );

            blankYesNoFlag = new YesNoFlag();
            blankYesNoFlag.SetBlank();
            yesYesNoFlag = new YesNoFlag();
            yesYesNoFlag.SetYes();
            noYesNoFlag = new YesNoFlag();
            noYesNoFlag.SetNo();
            this.addressView1.EditAddressButtonText = "Edit &Address...";
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private LoggingButton btnClearAll;

        private ComboBox cmbClaimsAddressVerified;
        private ComboBox cmbEmployerPaidPremium;
        private ComboBox cmbInfoRecvFrom;

        private Label lblStaticNetwork;
        private Label lblStaticClaimNumber;
        private Label lblStaticClaimsVerified;
        private Label lblStaticInsurancePhone;
        private Label lblStaticPaidPremiums;
        private Label lblStaticInfoRecvFrom;
        private Label lblRemarks;

        private Panel panel;

        private MaskedEditTextBox mtbClaimNumber;
        private MaskedEditTextBox mtbInsurancePhone;
        private MaskedEditTextBox mtbPPOPricingOrBroker;
        private MaskedEditTextBox mtbRemarks;

        private bool loadingModelData;
        private string pPOPricingOrBroker;
        private string claimNumberForIncident;
        private string insurancePhone;
        private string remarks;
        private Account i_Account;
        private InformationReceivedSource informationReceivedSource;
        private YesNoFlag claimsAddressVerified;
        private YesNoFlag employerhasPaidPremiumsToDate;
        private YesNoFlag blankYesNoFlag;
        private YesNoFlag noYesNoFlag;
        private YesNoFlag yesYesNoFlag;
        private LineLabel lineLabel1;
        private NameAndPhoneView nameAndPhoneView1;
        private AddressView addressView1;
        private RuleEngine i_RuleEngine;
        #endregion

        #region Constants
        #endregion
    }
}
