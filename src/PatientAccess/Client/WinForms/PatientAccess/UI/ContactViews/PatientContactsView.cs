using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.ContactViews
{
    /// <summary>
    /// Summary description for PatientContactsView.
    /// </summary>
    public class PatientContactsView : ControlView
    {
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        private void PatientContactsView_Enter( object sender, EventArgs e )
        {
            IAccountView accountView = AccountView.GetInstance();

            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if( accountView.IsMedicareAdvisedForPatient() )
            {
                accountView.MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                    UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

                if( warningResult == DialogResult.Yes )
                {
                    if( EnableInsuranceTab != null )
                    {
                        EnableInsuranceTab( this, new LooseArgs( Model_Account ) );
                    }
                }
            }
        }

        private void PatientContactsView_Leave( object sender, EventArgs e )
        {
            this.blnLeaveRun = true;
            RuleEngine.GetInstance().EvaluateRule( typeof( OnContactsForm ), this.Model_Account );
            this.blnLeaveRun = false;
        }

        private void PatientContactsView_Disposed( object sender, EventArgs e )
        {
            this.UnregisterEventHandlers();
        }

        //---------------------Evaluate ComboBoxes -------------------------------------------------------------
        private void emergencyContactView1_EmergencyRelationshipValidating( object sender, CancelEventArgs e )
        {
            if( !this.blnLeaveRun )
            {
                UIColors.SetNormalBgColor( this.emergencyContactView1.RelationshipView.ComboBox );
                this.Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmergContact_1Rel ), this.Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmergContact_1RelChange ), this.Model_Account );
                this.emergencyContactView1.RunRules();
            }
        }
        private void emergencyContactView2_EmergencyRelationshipValidating( object sender, CancelEventArgs e )
        {
            if( !this.blnLeaveRun )
            {
                UIColors.SetNormalBgColor( this.emergencyContactView2.RelationshipView.ComboBox );
                this.Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmergContact_2Rel ), this.Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmergContact_2RelChange ), this.Model_Account );
                this.emergencyContactView2.RunRules();
            }
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }

        private void InvalidEmergContact_1RelChangeEventHandler( object sender, EventArgs e )
        {
            this.ProcessInvalidCodeEvent( this.emergencyContactView1.RelationshipView.ComboBox );
        }
        private void InvalidEmergContact_2RelChangeEventHandler( object sender, EventArgs e )
        {
            this.ProcessInvalidCodeEvent( this.emergencyContactView2.RelationshipView.ComboBox );
        }

        //-----------------------------------------------------------------

        private void InvalidEmergContact_1RelEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.emergencyContactView1.RelationshipView.ComboBox );
        }
        private void InvalidEmergContact_2RelEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.emergencyContactView2.RelationshipView.ComboBox );
        }

        #endregion

        #region Methods
        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            emergencyContactView1.Model = Model_Account;
            emergencyContactView1.Model_EmergencyContact = Model_Account.EmergencyContact1;
            emergencyContactView1.AddressView.Context = EMERGENCY_CONTACT1;
            emergencyContactView1.RegisterEvents();
            emergencyContactView1.UpdateView();
            emergencyContactView1.Focus();
            emergencyContactView1.RunRules();

            emergencyContactView2.Model = Model_Account;
            emergencyContactView2.Model_EmergencyContact = Model_Account.EmergencyContact2;
            emergencyContactView2.AddressView.Context = EMERGENCY_CONTACT2;
            emergencyContactView2.UpdateView();

            this.RegisterEventHandlers();
            this.RunRules();

        }
        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
            emergencyContactView1.UpdateModel();
            emergencyContactView2.UpdateModel();
        }
        #endregion

        #region Properties

        private Account Model_Account
        {
            get
            {
                return (Account)this.Model;
            }
        }
        #endregion

        #region Private Methods

        private void RunRules()
        {
            UIColors.SetNormalBgColor( this.emergencyContactView2.RelationshipView.ComboBox );

            this.emergencyContactView1.LoadRules();
            this.emergencyContactView1.RegisterEvents();
            this.emergencyContactView1.RunRules();

            RuleEngine.GetInstance().EvaluateRule( typeof( OnContactsForm ), this.Model_Account );
        }

        private void RegisterEventHandlers()
        {
            if( !i_Registered )
            {
                i_Registered = true;

                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmergContact_1Rel ), this.Model_Account, new EventHandler( InvalidEmergContact_1RelEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmergContact_1RelChange ), this.Model_Account, new EventHandler( InvalidEmergContact_1RelChangeEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmergContact_2Rel ), this.Model_Account, new EventHandler( InvalidEmergContact_2RelEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmergContact_2RelChange ), this.Model_Account, new EventHandler( InvalidEmergContact_2RelChangeEventHandler ) );
            }
        }

        private void UnregisterEventHandlers()
        {
            i_Registered = false;

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmergContact_1Rel ), this.Model_Account, new EventHandler( InvalidEmergContact_1RelEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmergContact_1RelChange ), this.Model_Account, new EventHandler( InvalidEmergContact_1RelChangeEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmergContact_2Rel ), this.Model_Account, new EventHandler( InvalidEmergContact_2RelEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmergContact_2RelChange ), this.Model_Account, new EventHandler( InvalidEmergContact_2RelChangeEventHandler ) );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInformation = new System.Windows.Forms.Label();
            this.emergencyContactView1 = new PatientAccess.UI.ContactViews.EmergencyContactView();
            this.emergencyContactView2 = new PatientAccess.UI.ContactViews.EmergencyContactView();
            this.SuspendLayout();
            // 
            // lblInformation
            // 
            this.lblInformation.Location = new System.Drawing.Point( 11, 11 );
            this.lblInformation.Name = "lblInformation";
            this.lblInformation.Size = new System.Drawing.Size( 696, 23 );
            this.lblInformation.TabIndex = 0;
            this.lblInformation.Text = "If the first emergency contact lives with the patient, then provide a second emer" +
                "gency contact who does not live with the patient.";
            // 
            // emergencyContactView1
            // 
            this.emergencyContactView1.GroupBoxText = "Emergency Contact 1";
            this.emergencyContactView1.Location = new System.Drawing.Point( 11, 48 );
            this.emergencyContactView1.Model = null;
            this.emergencyContactView1.Model_EmergencyContact = null;
            this.emergencyContactView1.Name = "emergencyContactView1";
            this.emergencyContactView1.Size = new System.Drawing.Size( 372, 310 );
            this.emergencyContactView1.TabIndex = 1;
            this.emergencyContactView1.EmergencyRelationshipValidating += new System.ComponentModel.CancelEventHandler( this.emergencyContactView1_EmergencyRelationshipValidating );
            // 
            // emergencyContactView2
            // 
            this.emergencyContactView2.GroupBoxText = "Emergency Contact 2";
            this.emergencyContactView2.Location = new System.Drawing.Point( 397, 48 );
            this.emergencyContactView2.Model = null;
            this.emergencyContactView2.Model_EmergencyContact = null;
            this.emergencyContactView2.Name = "emergencyContactView2";
            this.emergencyContactView2.Size = new System.Drawing.Size( 372, 310 );
            this.emergencyContactView2.TabIndex = 2;
            this.emergencyContactView2.EmergencyRelationshipValidating += new System.ComponentModel.CancelEventHandler( this.emergencyContactView2_EmergencyRelationshipValidating );
            // 
            // PatientContactsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.emergencyContactView2 );
            this.Controls.Add( this.emergencyContactView1 );
            this.Controls.Add( this.lblInformation );
            this.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.Name = "PatientContactsView";
            this.Size = new System.Drawing.Size( 1024, 380 );
            this.Disposed += new System.EventHandler( this.PatientContactsView_Disposed );
            this.Enter += new System.EventHandler( this.PatientContactsView_Enter );
            this.Leave += new System.EventHandler( this.PatientContactsView_Leave );
            this.ResumeLayout( false );

        }
        #endregion
        #endregion

        #region Construction and Finalization
        public PatientContactsView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

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
        private Label lblInformation;
        private EmergencyContactView emergencyContactView1;
        private EmergencyContactView emergencyContactView2;

        private bool i_Registered = false;
        private bool blnLeaveRun = false;

        #endregion

        #region Constants
        private const string
            EMERGENCY_CONTACT1 = "EmergencyContact1",
            EMERGENCY_CONTACT2 = "EmergencyContact2";
        #endregion

    }
}
