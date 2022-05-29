using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.ContactViews
{
    /// <summary>
    /// Summary description for EmergencyContactView.
    /// </summary>
    [Serializable]
    public class EmergencyContactView : ControlView
    {
        #region Events
        public event CancelEventHandler EmergencyRelationshipValidating;
        #endregion

        #region Event Handlers
        private void relationshipView_EmergencyRelationshipValidating( object sender, CancelEventArgs e )
        {
            if( EmergencyRelationshipValidating != null )
            {
                EmergencyRelationshipValidating( this, null );
            }
        }

        private void addressView_AddressChanged(object sender, EventArgs e)
        {
            ContactPoint generalContactPoint = Model_EmergencyContact.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
            generalContactPoint.Address = Model_EmergencyContact.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType()).Address;

            if (addressView.Context == "EmergencyContact1")
            {
                RuleEngine.GetInstance().EvaluateRule(typeof(ContactAreaCodeRequired), Model_EmergencyContact);
                RuleEngine.GetInstance().EvaluateRule(typeof(ContactAreaCodePreferred), Model_EmergencyContact);
                RuleEngine.GetInstance().EvaluateRule(typeof(ContactPhoneRequired), Model_EmergencyContact);
                RuleEngine.GetInstance().EvaluateRule(typeof(ContactPhonePreferred), Model_EmergencyContact);
                RuleEngine.GetInstance().EvaluateRule(typeof(ContactAddressPreferred), Model_EmergencyContact);
            }
        }

        void addressView_AreaCodeChanged( object sender, EventArgs e )
        {
            ContactPoint generalContactPoint = Model_EmergencyContact.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            generalContactPoint.PhoneNumber = Model_EmergencyContact.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() ).PhoneNumber;

            if( addressView.Context == "EmergencyContact1" )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodeRequired ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodePreferred ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhoneRequired ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhonePreferred ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactAddressPreferred ), Model_EmergencyContact);
            }
        }

        private void addressView_PhoneNumberChanged( object sender, EventArgs e )
        {
            ContactPoint generalContactPoint = Model_EmergencyContact.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            generalContactPoint.PhoneNumber = Model_EmergencyContact.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() ).PhoneNumber;

            if( addressView.Context == "EmergencyContact1" )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodeRequired ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodePreferred ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhoneRequired ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhonePreferred ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactAddressPreferred ), Model_EmergencyContact);
            }
        }

        private void ContactNameRequiredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if( aControl == maskEditName )
            {
                UIColors.SetRequiredBgColor( aControl );
                Refresh();
            }
        }

        private void ContactRelationshipRequiredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if( aControl == relationshipView.ComboBox )
            {
                UIColors.SetRequiredBgColor( aControl );
                Refresh();
            }
        }

        private void ContactNamePreferredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if( aControl == maskEditName )
            {
                UIColors.SetPreferredBgColor( aControl );
                Refresh();
            }
        }

        private void ContactRelationshipPreferredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if( aControl == relationshipView.ComboBox )
            {
                UIColors.SetPreferredBgColor( aControl );
                Refresh();
            }
        }

        private void RelationshipSelected( object sender, EventArgs e )
        {
            LooseArgs args = e as LooseArgs;
            RelationshipType relationshipType = args.Context as RelationshipType;

            if( relationshipType == null )
            {
                relationshipType = new RelationshipType();
            }

            if( Model_EmergencyContact != null )
            {
                Relationship relationship = new Relationship( relationshipType,
                    Model_Account.Patient.GetType(), Model_EmergencyContact.GetType() );

                Model_Account.Patient.RemoveRelationship( i_OriginalRelationship );
                Model_Account.Patient.AddRelationship( relationship );

                Model_EmergencyContact.RemoveRelationship( i_OriginalRelationship );
                Model_EmergencyContact.AddRelationship( relationship );
                Model_EmergencyContact.RelationshipType = relationshipType;

                i_OriginalRelationship = relationship;
            }


            if( addressView.Context == "EmergencyContact1" )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactRelationshipRequired ), Model_EmergencyContact, relationshipView.ComboBox );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactRelationshipPreferred ), Model_EmergencyContact, relationshipView.ComboBox );
            }
        }

        private void EmergencyContactView_Leave( object sender, EventArgs e )
        {
            if( !DesignMode && maskEditName.Text != String.Empty )
            {
                Model_EmergencyContact.Name = maskEditName.UnMaskedText;
            }
            else
            {
                Model_EmergencyContact.Name = String.Empty;
            }
        }

        private void EmergencyContactView_Disposed( object sender, EventArgs e )
        {
            UnregisterEvents();
        }

        private void maskEditName_Validating( object sender, CancelEventArgs e )
        {
            Model_EmergencyContact.Name = maskEditName.Text.Trim();
            RunRules();
        }

       private void ResetButtonClick( object sender, EventArgs e )
       {
            maskEditName.UnMaskedText = String.Empty;
            Model_EmergencyContact.Name = String.Empty;

            addressView.ResetView();
            relationshipView.ResetView();

            RunRules();
       }
       
        private void ConfigureControls()
       {
           MaskedEditTextBoxBuilder.ConfigureEmergencyContactName( maskEditName );
       }

        #endregion

        #region Methods
      

        public void LoadRules()
        {
            if( !i_RulesLoaded )
            {
                i_RulesLoaded = true;
                i_EventsRegistered = false;

                RuleEngine.GetInstance().LoadRules( Model_Account );
            }
        }

        public void RunRules()
        {
            if( addressView.Context == "EmergencyContact1" )
            {
                UIColors.SetNormalBgColor( maskEditName );
                UIColors.SetNormalBgColor( relationshipView.ComboBox );
                Refresh();

                RuleEngine.GetInstance().EvaluateRule( typeof( ContactNameRequired ), Model_EmergencyContact, maskEditName );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactRelationshipRequired ), Model_EmergencyContact, relationshipView.ComboBox );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhoneRequired ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodeRequired ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactAddressPreferred ), Model_EmergencyContact);

                RuleEngine.GetInstance().EvaluateRule( typeof( ContactNamePreferred ), Model_EmergencyContact, maskEditName );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactRelationshipPreferred ), Model_EmergencyContact, relationshipView.ComboBox );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhonePreferred ), Model_EmergencyContact );
                RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodePreferred ), Model_EmergencyContact );
            }
        }

        public void RegisterEvents()
        {
            if( !i_EventsRegistered )
            {
                i_EventsRegistered = true;

                RuleEngine.GetInstance().RegisterEvent( typeof( ContactNameRequired ), Model_EmergencyContact, maskEditName, ContactNameRequiredEventHandler );

                RuleEngine.GetInstance().RegisterEvent( typeof( ContactRelationshipRequired ), Model_EmergencyContact, relationshipView.ComboBox, ContactRelationshipRequiredEventHandler );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactPhoneRequired ), Model_EmergencyContact, new EventHandler( addressView.PhoneRequiredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactAreaCodeRequired ), Model_EmergencyContact, new EventHandler( addressView.AreaCodeRequiredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactAddressPreferred ), Model_EmergencyContact, new EventHandler( addressView.AddressPreferredEventHandler ) );

                RuleEngine.GetInstance().RegisterEvent( typeof( ContactNamePreferred ), Model_EmergencyContact, maskEditName, ContactNamePreferredEventHandler );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactRelationshipPreferred ), Model_EmergencyContact, relationshipView.ComboBox, ContactRelationshipPreferredEventHandler );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactPhonePreferred ), Model_EmergencyContact, new EventHandler( addressView.PhonePreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactAreaCodePreferred ), Model_EmergencyContact, new EventHandler(addressView.AreaCodePreferredEventHandler));
            }
        }

        public void UnregisterEvents()
        {
            i_EventsRegistered = false;

            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactNameRequired ), Model_EmergencyContact, ContactNameRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactRelationshipRequired ), Model_EmergencyContact, ContactRelationshipRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactPhoneRequired ), Model_EmergencyContact, addressView.PhoneRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactAddressPreferred ), Model_EmergencyContact, addressView.AddressPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactAreaCodeRequired ), Model_EmergencyContact, addressView.AreaCodeRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactNamePreferred ), Model_EmergencyContact, ContactNamePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactRelationshipPreferred ), Model_EmergencyContact, ContactRelationshipPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactPhonePreferred ), Model_EmergencyContact, addressView.PhonePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactAreaCodePreferred ), Model_EmergencyContact, addressView.AreaCodePreferredEventHandler );
        }

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            if( Model_Account != null && Model_EmergencyContact != null )
            {
                relationshipView.Model = Model_EmergencyContact.Relationships;
                relationshipView.PartyForRelationships = Model_EmergencyContact;
                maskEditName.UnMaskedText = Model_EmergencyContact.Name;

                foreach( Relationship r in Model_EmergencyContact.Relationships )
                {
                    i_OriginalRelationship = r;
                    break;
                }
            }
            else
            {
                relationshipView.Model = null;
            }
            relationshipView.UpdateView();

            if( Model_Account != null && Model_EmergencyContact != null )
            {
                addressView.KindOfTargetParty = Model_EmergencyContact.GetType();
                addressView.PatientAccount = Model_Account;
                addressView.Model_ContactPoint =
                    Model_EmergencyContact.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            }
            else
            {
                addressView.PatientAccount = null;
                addressView.Model_ContactPoint = null;
            }

            addressView.UpdateView();

            // do NOT run rules here... rules for this form are invoked from the parent.
            //this.RunRules();
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }
        #endregion

        #region Properties
        [Browsable( false )]
        private Account Model_Account
        {
            get
            {
                return (Account)Model;
            }
        }

        [Browsable( false )]
        public EmergencyContact Model_EmergencyContact
        {
            private get
            {
                return emergencyContact;
            }
            set
            {
                emergencyContact = value;
            }
        }

        public string GroupBoxText
        {
            get
            {
                return groupBox.Text;
            }
            set
            {
                groupBox.Text = value;
            }
        }

        [Browsable( false )]
        public AddressView AddressView
        {
            get
            {
                return addressView;
            }
            set
            {
                addressView = value;
            }
        }

        [Browsable( false )]
        public RelationshipView RelationshipView
        {
            get
            {
                return relationshipView;
            }
            set
            {
                relationshipView = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.relationshipView = new PatientAccess.UI.CommonControls.RelationshipView();
            this.btnReset = new LoggingButton();
            this.addressView = new PatientAccess.UI.AddressViews.AddressView();
            this.lblLastFirst = new System.Windows.Forms.Label();
            this.maskEditName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add( this.relationshipView );
            this.groupBox.Controls.Add( this.btnReset );
            this.groupBox.Controls.Add( this.addressView );
            this.groupBox.Controls.Add( this.lblLastFirst );
            this.groupBox.Controls.Add( this.maskEditName );
            this.groupBox.Controls.Add( this.lblName );
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point( 0, 0 );
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size( 372, 310 );
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            // 
            // relationshipView
            // 
            this.relationshipView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.relationshipView.LabelText = "The Contact is the Patient\'s:";
            this.relationshipView.Location = new System.Drawing.Point( 11, 60 );
            this.relationshipView.Model = null;
            this.relationshipView.Name = "relationshipView";
            this.relationshipView.PartyForRelationships = null;
            this.relationshipView.PatientIs = null;
            this.relationshipView.RelationshipName = "";
            this.relationshipView.Size = new System.Drawing.Size( 338, 24 );
            this.relationshipView.TabIndex = 2;
            this.relationshipView.RelationshipSelected += new System.EventHandler( this.RelationshipSelected );
            this.relationshipView.RelationshipValidating += new System.ComponentModel.CancelEventHandler( this.relationshipView_EmergencyRelationshipValidating );
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point( 284, 272 );
            this.btnReset.Name = "btnReset";
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Clear";
            this.btnReset.Click += new System.EventHandler( this.ResetButtonClick );
            // 
            // addressView
            // 
            this.addressView.Context = null;
            this.addressView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.addressView.KindOfTargetParty = null;
            this.addressView.Location = new System.Drawing.Point( 11, 85 );
            this.addressView.Model = null;
            this.addressView.Mode = AddressViews.AddressView.AddressMode.PHONE;
            this.addressView.ShowStatus = false;
            this.addressView.Name = "addressView";
            this.addressView.PatientAccount = null;
            this.addressView.TabIndex = 3;
            this.addressView.IsAddressWithStreet2 = false;
            this.addressView.PhoneNumberChanged += new EventHandler( addressView_PhoneNumberChanged );
            this.addressView.AreaCodeChanged += new EventHandler( addressView_AreaCodeChanged );
            this.addressView.AddressChanged += new EventHandler( addressView_AddressChanged );
            // 
            // lblLastFirst
            // 
            this.lblLastFirst.Location = new System.Drawing.Point( 55, 44 );
            this.lblLastFirst.Name = "lblLastFirst";
            this.lblLastFirst.TabIndex = 0;
            this.lblLastFirst.Text = "Last, First";
            // 
            // maskEditName
            // 
            this.maskEditName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.maskEditName.Location = new System.Drawing.Point( 49, 18 );
            this.maskEditName.Mask = "";
            this.maskEditName.MaxLength = 25;
            this.maskEditName.Name = "maskEditName";
            this.maskEditName.Size = new System.Drawing.Size( 200, 20 );
            this.maskEditName.TabIndex = 1;
            this.maskEditName.Validating += new System.ComponentModel.CancelEventHandler( this.maskEditName_Validating );
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point( 8, 21 );
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size( 56, 23 );
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            // 
            // EmergencyContactView
            // 
            this.Controls.Add( this.groupBox );
            this.Name = "EmergencyContactView";
            this.Size = new System.Drawing.Size( 372, 310 );
            this.Disposed += new System.EventHandler( this.EmergencyContactView_Disposed );
            this.Leave += new System.EventHandler( this.EmergencyContactView_Leave );
            this.groupBox.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        #region Construction and Finalization
        public EmergencyContactView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            ConfigureControls();
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
        private LoggingButton btnReset;
        private GroupBox groupBox;
        private Label lblName;
        private Label lblLastFirst;
        private MaskedEditTextBox maskEditName;
        private RelationshipView relationshipView;
        private AddressView addressView;

        private EmergencyContact emergencyContact;
        private Relationship i_OriginalRelationship;
        private bool i_EventsRegistered = false;
        private bool i_RulesLoaded = false;
        #endregion

        #region Constants
        #endregion
    }
}
