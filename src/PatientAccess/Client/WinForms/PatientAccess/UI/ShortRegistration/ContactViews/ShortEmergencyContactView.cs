using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.ShortRegistration.ContactViews
{
    [Serializable]
    public class ShortEmergencyContactView : ControlView
    {
        #region Events
        public event CancelEventHandler EmergencyRelationshipValidating;
        #endregion

        #region Event Handlers
        private void relationshipView_EmergencyRelationshipValidating( object sender, CancelEventArgs e )
        {
            if ( EmergencyRelationshipValidating != null )
            {
                EmergencyRelationshipValidating( this, null );
            }
        }

        private void ContactNameRequiredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = ( PropertyChangedArgs )e;
            Control aControl = args.Context as Control;

            if ( aControl == maskEditName )
            {
                UIColors.SetRequiredBgColor( aControl );
                Refresh();
            }
        }

        private void ContactRelationshipRequiredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = ( PropertyChangedArgs )e;
            Control aControl = args.Context as Control;

            if ( aControl == relationshipView.ComboBox )
            {
                UIColors.SetRequiredBgColor( aControl );
                Refresh();
            }
        }

        private void ContactNamePreferredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = ( PropertyChangedArgs )e;
            Control aControl = args.Context as Control;

            if ( aControl == maskEditName )
            {
                UIColors.SetPreferredBgColor( aControl );
                Refresh();
            }
        }

        private void ContactRelationshipPreferredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = ( PropertyChangedArgs )e;
            Control aControl = args.Context as Control;

            if ( aControl == relationshipView.ComboBox )
            {
                UIColors.SetPreferredBgColor( aControl );
                Refresh();
            }
        }

        private void RelationshipSelected( object sender, EventArgs e )
        {
            LooseArgs args = e as LooseArgs;
            RelationshipType relationshipType = null;

            if ( args != null )
            {
                relationshipType = args.Context as RelationshipType;
            }

            if ( relationshipType == null )
            {
                relationshipType = new RelationshipType();
            }

            if ( Model_EmergencyContact != null )
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

            RuleEngine.GetInstance().EvaluateRule( typeof( ContactRelationshipRequired ), Model_EmergencyContact, relationshipView.ComboBox );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactRelationshipPreferred ), Model_EmergencyContact, relationshipView.ComboBox );
        }

        private void EmergencyContactView_Leave( object sender, EventArgs e )
        {
            if ( !DesignMode && maskEditName.Text != String.Empty )
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

        private void phoneNumberControl_AreaCodeChanged( object sender, EventArgs e )
        {
            if ( EmergencyContactPoint != null && EmergencyContactPoint.PhoneNumber == null )
            {
                EmergencyContactPoint.PhoneNumber = new PhoneNumber { AreaCode = phoneNumberControl.Model.AreaCode };
            }

            RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodeRequired ), Model_EmergencyContact );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodePreferred ), Model_EmergencyContact );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhoneRequired ), Model_EmergencyContact );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhonePreferred ), Model_EmergencyContact );
        }

        private void phoneNumberControl_PhoneNumberChanged( object sender, EventArgs e )
        {
            if ( EmergencyContactPoint != null && EmergencyContactPoint.PhoneNumber == null )
            {
                EmergencyContactPoint.PhoneNumber = new PhoneNumber { Number = phoneNumberControl.Model.Number };
            }

            RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodeRequired ), Model_EmergencyContact );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodePreferred ), Model_EmergencyContact );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhoneRequired ), Model_EmergencyContact );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhonePreferred ), Model_EmergencyContact );
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureEmergencyContactName( maskEditName );
        }

        private void ClearControls()
        {
            phoneNumberControl.SetNormalColor();
            phoneNumberControl.AreaCode = string.Empty;
            phoneNumberControl.PhoneNumber = string.Empty;
        }

        private void AreaCodeRequiredEventHandler( object sender, EventArgs e )
        {
            phoneNumberControl.SetAreaCodeRequiredColor();
        }

        private void AreaCodePreferredEventHandler( object sender, EventArgs e )
        {
            phoneNumberControl.SetAreaCodePreferredColor();
        }

        private void PhoneRequiredEventHandler( object sender, EventArgs e )
        {
            phoneNumberControl.SetPhoneNumberRequiredColor();
        }

        private void PhonePreferredEventHandler( object sender, EventArgs e )
        {
            phoneNumberControl.SetPhoneNumberPreferredColor();
        }

        #endregion

        #region Methods


        public void LoadRules()
        {
            if ( !i_RulesLoaded )
            {
                i_RulesLoaded = true;
                i_EventsRegistered = false;

                RuleEngine.GetInstance().LoadRules( Model_Account );
            }
        }

        public void RunRules()
        {
            UIColors.SetNormalBgColor( maskEditName );
            UIColors.SetNormalBgColor( relationshipView.ComboBox );
            Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof( ContactNameRequired ), Model_EmergencyContact, maskEditName );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactRelationshipRequired ), Model_EmergencyContact, relationshipView.ComboBox );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhoneRequired ), Model_EmergencyContact );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodeRequired ), Model_EmergencyContact );

            RuleEngine.GetInstance().EvaluateRule( typeof( ContactNamePreferred ), Model_EmergencyContact, maskEditName );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactRelationshipPreferred ), Model_EmergencyContact, relationshipView.ComboBox );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactPhonePreferred ), Model_EmergencyContact );
            RuleEngine.GetInstance().EvaluateRule( typeof( ContactAreaCodePreferred ), Model_EmergencyContact );
        }

        public void RegisterEvents()
        {
            if ( !i_EventsRegistered )
            {
                i_EventsRegistered = true;

                RuleEngine.GetInstance().RegisterEvent( typeof( ContactNameRequired ), Model_EmergencyContact, maskEditName, ContactNameRequiredEventHandler );

                RuleEngine.GetInstance().RegisterEvent( typeof( ContactRelationshipRequired ), Model_EmergencyContact, relationshipView.ComboBox, ContactRelationshipRequiredEventHandler );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactPhoneRequired ), Model_EmergencyContact, new EventHandler( PhoneRequiredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactAreaCodeRequired ), Model_EmergencyContact, new EventHandler( AreaCodeRequiredEventHandler ) );

                RuleEngine.GetInstance().RegisterEvent( typeof( ContactNamePreferred ), Model_EmergencyContact, maskEditName, ContactNamePreferredEventHandler );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactRelationshipPreferred ), Model_EmergencyContact, relationshipView.ComboBox, ContactRelationshipPreferredEventHandler );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactPhonePreferred ), Model_EmergencyContact, new EventHandler( PhonePreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( ContactAreaCodePreferred ), Model_EmergencyContact, new EventHandler( AreaCodePreferredEventHandler ) );
            }
        }

        public void UnregisterEvents()
        {
            i_EventsRegistered = false;

            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactNameRequired ), Model_EmergencyContact, ContactNameRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactRelationshipRequired ), Model_EmergencyContact, ContactRelationshipRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactPhoneRequired ), Model_EmergencyContact, PhoneRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactAreaCodeRequired ), Model_EmergencyContact, AreaCodeRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactNamePreferred ), Model_EmergencyContact, ContactNamePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactRelationshipPreferred ), Model_EmergencyContact, ContactRelationshipPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactPhonePreferred ), Model_EmergencyContact, PhonePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( ContactAreaCodePreferred ), Model_EmergencyContact, AreaCodePreferredEventHandler );
        }

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            if ( Model_Account != null && Model_EmergencyContact != null )
            {
                relationshipView.Model = Model_EmergencyContact.Relationships;
                relationshipView.PartyForRelationships = Model_EmergencyContact;
                maskEditName.UnMaskedText = Model_EmergencyContact.Name;

                foreach ( Relationship r in Model_EmergencyContact.Relationships )
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

            if ( Model_Account != null && Model_EmergencyContact != null )
            {
                EmergencyContactPoint =
                    Model_EmergencyContact.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            }
            else
            {
                EmergencyContactPoint = null;
            }

            ClearControls();

            if ( EmergencyContactPoint != null && EmergencyContactPoint.PhoneNumber != null )
            {
                phoneNumberControl.Model = EmergencyContactPoint.PhoneNumber;
            }

            phoneNumberControl.RunRules();

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
                return ( Account )Model;
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

        [Browsable( false )]
        private ContactPoint EmergencyContactPoint
        {
            get
            {
                if ( contactPoint == null )
                {
                    return Model_EmergencyContact.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
                }

                return contactPoint;
            }
            set
            {
                contactPoint = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ShortEmergencyContactView ) );
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.relationshipView = new PatientAccess.UI.CommonControls.RelationshipView();
            this.lblLastFirst = new System.Windows.Forms.Label();
            this.maskEditName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add( this.lblPhone );
            this.groupBox.Controls.Add( this.phoneNumberControl );
            this.groupBox.Controls.Add( this.relationshipView );
            this.groupBox.Controls.Add( this.lblLastFirst );
            this.groupBox.Controls.Add( this.maskEditName );
            this.groupBox.Controls.Add( this.lblName );
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point( 0, 0 );
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size( 372, 105 );
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Emergency Contact";
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point( 8, 76 );
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size( 41, 13 );
            this.lblPhone.TabIndex = 4;
            this.lblPhone.Text = "Phone:";
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point( 49, 70 );
            this.phoneNumberControl.Model = ( ( PatientAccess.Domain.Parties.PhoneNumber )( resources.GetObject( "phoneNumberControl.Model" ) ) );
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 3;
            this.phoneNumberControl.AreaCodeChanged += new System.EventHandler( phoneNumberControl_AreaCodeChanged );
            this.phoneNumberControl.PhoneNumberChanged += new System.EventHandler( phoneNumberControl_PhoneNumberChanged );
            // 
            // relationshipView
            // 
            this.relationshipView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
            this.relationshipView.LabelText = "The Contact is the Patient\'s:";
            this.relationshipView.Location = new System.Drawing.Point( 5, 43 );
            this.relationshipView.Model = null;
            this.relationshipView.Name = "relationshipView";
            this.relationshipView.PartyForRelationships = null;
            this.relationshipView.PatientIs = null;
            this.relationshipView.RelationshipName = "";
            this.relationshipView.Size = new System.Drawing.Size( 290, 24 );
            this.relationshipView.ComboBox.Location = new System.Drawing.Point( 0, 0 );
            this.relationshipView.ComboBox.Size = new System.Drawing.Size(145, 24);
            this.relationshipView.TabIndex = 2;
            this.relationshipView.RelationshipSelected += new System.EventHandler( this.RelationshipSelected );
            this.relationshipView.RelationshipValidating += new System.ComponentModel.CancelEventHandler( this.relationshipView_EmergencyRelationshipValidating );
            // 
            // lblLastFirst
            // 
            this.lblLastFirst.Location = new System.Drawing.Point( 250, 21 );
            this.lblLastFirst.Name = "lblLastFirst";
            this.lblLastFirst.Size = new System.Drawing.Size( 45, 20 );
            this.lblLastFirst.TabIndex = 0;
            this.lblLastFirst.Text = "Last, First";
            // 
            // maskEditName
            // 
            this.maskEditName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.maskEditName.Location = new System.Drawing.Point( 45, 18 );
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
            // ShortEmergencyContactView
            // 
            this.Controls.Add( this.groupBox );
            this.Name = "ShortEmergencyContactView";
            this.Size = new System.Drawing.Size( 372, 105 );
            this.Leave += new System.EventHandler( this.EmergencyContactView_Leave );
            this.Disposed += new System.EventHandler( this.EmergencyContactView_Disposed );
            this.groupBox.ResumeLayout( false );
            this.groupBox.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        #region Construction and Finalization
        public ShortEmergencyContactView()
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
            if ( disposing )
            {
                if ( components != null )
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
        private GroupBox groupBox;
        private Label lblName;
        private Label lblLastFirst;
        private MaskedEditTextBox maskEditName;
        private RelationshipView relationshipView;

        private EmergencyContact emergencyContact;
        private ContactPoint contactPoint;
        private Relationship i_OriginalRelationship;
        private bool i_EventsRegistered;
        private Label lblPhone;
        private PhoneNumberControl phoneNumberControl;
        private bool i_RulesLoaded;
        #endregion

        #region Constants
        #endregion
    }

    [Serializable]
    public class NewClass : ControlView
    {
    
    }
}


