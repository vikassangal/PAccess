using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for DriversLicenseView.
    /// </summary>
    public class DriversLicenseView : ControlView
    {
        #region Events
        
        public event EventHandler DriversLicenseNumberChanged;

        #endregion

        #region Event Handlers
        /// <summary>
        /// ActivateDemographicsView_Disposed - on disposing, remove any event handlers we have
        /// wired to rules
        /// </summary>
        private void DriversLicenseView_Disposed(object sender, EventArgs e)
        {
            UnRegisterEvents();
        }
        
        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>
        private void DriversLicensePreferredEventHandler(object sender, EventArgs e)
		{
			UIColors.SetPreferredBgColor( mtbDriverLicense );
		}

        private void DriversLicenseStatePreferredEventHandler(object sender, EventArgs e)
		{
			UIColors.SetPreferredBgColor( cmbLicenseState );	
		}

        private void mtbDriverLicense_TextChanged( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

			if( mtb.Text.Length == 0 )
			{ 
				CheckForRequiredFields();
			}
			else
			{                
				UIColors.SetNormalBgColor( mtb );
			}
        }

		private void mtbDriverLicense_Validating(object sender, CancelEventArgs e)
		{
			MaskedEditTextBox mtb = sender as MaskedEditTextBox;
			UIColors.SetNormalBgColor( mtb );

            if( Model_Account != null
                && Model_Account.Patient != null
                && Model_Account.Patient.DriversLicense != null )
            {
                if( mtb.Text.Length > 0 )
                {
                    Model_Account.Patient.DriversLicense.Number = mtb.Text.Trim();
                }
                else
                {
                    Model_Account.Patient.DriversLicense.Number = mtb.Text.Trim();
                    UIColors.SetNormalBgColor( cmbLicenseState );
                }
            }

			if( DriversLicenseNumberChanged != null )
			{
				DriversLicenseNumberChanged(this, null);
			}
			CheckForRequiredFields();
		}

        private void cmbLicenseState_DropDown( object sender, EventArgs e )
        {
        }

		private void cmbLicenseState_Validating(object sender, CancelEventArgs e)
		{
			UIColors.SetNormalBgColor( cmbLicenseState );
		
			RuleEngine.GetInstance().EvaluateRule( typeof( DriversLicenseStatePreferred ), Model ); 
		}

        private void cmbLicenseState_SelectedIndexChanged(object sender, EventArgs e)
        {
			UIColors.SetNormalBgColor( cmbLicenseState );
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                Model_Account.Patient.DriversLicense.State = cb.SelectedItem as State;
            }
			
			RuleEngine.GetInstance().EvaluateRule( typeof( DriversLicenseStatePreferred ), Model );

        }
        #endregion

        #region Methods


        public override void UpdateView()
        {
            if( loadingModelData )
            {
                StateComboHelper = new ReferenceValueComboBox( cmbLicenseState );
                loadingModelData = false;
                PopulateStatesComboBox(); 

				if( mtbDriverLicense.Enabled && Model_Account != null )
                {
                    mtbDriverLicense.UnMaskedText = Model_Account.Patient.DriversLicense.Number.TrimEnd();

                    if( mtbDriverLicense.Text.Length == 0 )
                    {  
                    }

                    if( Model_Account.Patient.DriversLicense.State != null 
                        && Model_Account.Patient.DriversLicense.State.Code != string.Empty)
                    {
                        cmbLicenseState.SelectedItem = Model_Account.Patient.DriversLicense.State;
                    } 
                } 
            }

			CheckLicenseRules();
        }

		public void CheckForNewBornActivity()
		{
            if ( Model_Account.Activity!=null && Model_Account.Activity.IsNewBornRelatedActivity())
			{
				UIColors.SetNormalBgColor( mtbDriverLicense );
				UIColors.SetNormalBgColor( cmbLicenseState );
				mtbDriverLicense.UnMaskedText = String.Empty;
				mtbDriverLicense.Enabled = false;
				cmbLicenseState.SelectedItem = new State();
				cmbLicenseState.Enabled = false;
			}
		}
        #endregion

        #region Properties
        public Account Model_Account
        {
            private get
            {
                return (Account)Model;
            }
            set
            {
                Model = value;
            }
        }

        public MaskedEditTextBox TextBox
        {   
            get
            {
                return mtbDriverLicense;
            }
        }

        public ComboBox ComboBox
        {   // May need to be used for searching the control
            get
            {
                return cmbLicenseState;
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
                if( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }
        #endregion

        #region Private Methods

		private void CheckLicenseRules()
		{
			UIColors.SetNormalBgColor( mtbDriverLicense );
			UIColors.SetNormalBgColor( cmbLicenseState );

			//UIColors.SetPreferredBgColor( mtbDriverLicense );
			//RuleEngine.LoadRules( Model_Account );

            if( !i_Registered )
            {
                i_Registered = true;

                RuleEngine.GetInstance().RegisterEvent( typeof( DriversLicensePreferred ), Model, new EventHandler( DriversLicensePreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( DriversLicenseStatePreferred ), Model, new EventHandler( DriversLicenseStatePreferredEventHandler ) );

            }
			CheckForRequiredFields();
		}

		private void CheckForRequiredFields()
		{
			RuleEngine.EvaluateRule( typeof( DriversLicensePreferred ), Model_Account );
			RuleEngine.EvaluateRule( typeof( DriversLicenseStatePreferred ), Model_Account );	
		}

        private void PopulateStatesComboBox()
        {
            IAddressBroker broker = new AddressBrokerProxy();
                        
            if( cmbLicenseState.Items.Count == 0 )
            {
                StateComboHelper.PopulateWithCollection(broker.AllStates(User.GetCurrent().Facility.Oid));
            }
        }

        /// <summary>
        /// UnRegisterEvents - Unregister the rules events
        /// </summary>
		private void UnRegisterEvents()
		{
            i_Registered = false;
			RuleEngine.GetInstance().UnregisterEvent( typeof( DriversLicensePreferred ), Model, new EventHandler( DriversLicensePreferredEventHandler ) );
			RuleEngine.GetInstance().UnregisterEvent( typeof( DriversLicenseStatePreferred ), Model, new EventHandler( DriversLicenseStatePreferredEventHandler ) );
		}

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.cmbLicenseState = new System.Windows.Forms.ComboBox();
			this.mtbDriverLicense = new Extensions.UI.Winforms.MaskedEditTextBox();
			this.lblDLState = new System.Windows.Forms.Label();
			this.lblDLNumber = new System.Windows.Forms.Label();
			this.groupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.cmbLicenseState);
			this.groupBox.Controls.Add(this.mtbDriverLicense);
			this.groupBox.Controls.Add(this.lblDLState);
			this.groupBox.Controls.Add(this.lblDLNumber);
			this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox.Location = new System.Drawing.Point(0, 0);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(300, 84);
			this.groupBox.TabIndex = 1;
			this.groupBox.TabStop = false;
			this.groupBox.Text = "U.S. driver\'s license";
			// 
			// cmbLicenseState
			// 
			this.cmbLicenseState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbLicenseState.Location = new System.Drawing.Point(100, 50);
			this.cmbLicenseState.MaxLength = 23;
			this.cmbLicenseState.Name = "cmbLicenseState";
			this.cmbLicenseState.Size = new System.Drawing.Size(185, 21);
			this.cmbLicenseState.TabIndex = 3;
			this.cmbLicenseState.DropDown += new System.EventHandler(this.cmbLicenseState_DropDown);
			this.cmbLicenseState.SelectedIndexChanged += new System.EventHandler(this.cmbLicenseState_SelectedIndexChanged);
			this.cmbLicenseState.Validating += new CancelEventHandler(cmbLicenseState_Validating);
			// 
			// mtbDriverLicense
			// 
			this.mtbDriverLicense.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.mtbDriverLicense.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
			this.mtbDriverLicense.KeyPressExpression = "^[a-zA-Z0-9]*";
			this.mtbDriverLicense.Location = new System.Drawing.Point(100, 18);
			this.mtbDriverLicense.Mask = string.Empty;
			this.mtbDriverLicense.MaxLength = 15;
			this.mtbDriverLicense.Name = "mtbDriverLicense";
			this.mtbDriverLicense.Size = new System.Drawing.Size(115, 20);
			this.mtbDriverLicense.TabIndex = 2;
			this.mtbDriverLicense.ValidationExpression = "^[a-zA-Z0-9]*";
			this.mtbDriverLicense.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDriverLicense_Validating);
			this.mtbDriverLicense.TextChanged += new System.EventHandler(this.mtbDriverLicense_TextChanged);
			// 
			// lblDLState
			// 
			this.lblDLState.Location = new System.Drawing.Point(8, 53);
			this.lblDLState.Name = "lblDLState";
			this.lblDLState.Size = new System.Drawing.Size(50, 23);
			this.lblDLState.TabIndex = 0;
			this.lblDLState.Text = "State:";
			// 
			// lblDLNumber
			// 
			this.lblDLNumber.Location = new System.Drawing.Point(8, 21);
			this.lblDLNumber.Name = "lblDLNumber";
			this.lblDLNumber.Size = new System.Drawing.Size(50, 23);
			this.lblDLNumber.TabIndex = 0;
			this.lblDLNumber.Text = "Number:";
			// 
			// DriversLicenseView
			// 
			this.Controls.Add(this.groupBox);
			this.Name = "DriversLicenseView";
			this.Size = new System.Drawing.Size(300, 84);
			this.Disposed += new System.EventHandler(this.DriversLicenseView_Disposed);
			this.groupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion

        #endregion

        #region Private properties
        
        private ReferenceValueComboBox StateComboHelper
        {
            get
            {
                return i_StateComboHelper;
            }
            set
            {
                i_StateComboHelper = value;
            }
        }
        
        #endregion

        #region Construction and Finalization
        public DriversLicenseView()
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
        private Container             components = null;

        private ComboBox               cmbLicenseState;
        private GroupBox               groupBox;
        private Label                  lblDLNumber;
        private Label                  lblDLState;
        private MaskedEditTextBox    mtbDriverLicense;
        private RuleEngine                                  i_RuleEngine;
        private bool                                        loadingModelData = true;
        private ReferenceValueComboBox                      i_StateComboHelper;
        private bool                                        i_Registered = false;

        #endregion

        #region Constants
		#endregion
	}
}
