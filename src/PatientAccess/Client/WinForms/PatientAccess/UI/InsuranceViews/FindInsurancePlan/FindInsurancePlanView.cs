using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.FindInsurancePlan
{
    /// <summary>
    /// Summary description for FindInsurancePlanView.
    /// </summary>
    public class FindInsurancePlanView : TimeOutFormView
    {
        #region Event Handlers

        private void selectByEmployerView_CoveredGroupSelected( object sender, EventArgs e )
        {
            if( e != null )
            {
                SelectInsuranceArgs args = e as SelectInsuranceArgs; 

                selectedEmployer = args.SelectedEmployer as Employer;            
            }
        }

        /// <summary>
        /// If focus is on top list control and user hits ENTER key, close the dialog.
        /// </summary>
        private void CloseFormOnEnterKeyEventHandler( object sender, EventArgs e )
        {
            //Close();
        }

        private void ListViewLostFocusEventHandler( object sender, EventArgs e )
        {
            if( button_Cancel.CanFocus && button_OK.Focused == false )
            {
                button_Cancel.Focus();
            }
        }

        /// <summary>
        /// Event handler for SelectByView.CloseFormEvent which is raised when the
        /// user hits the ENTER key on the SelectByView.ListView
        /// </summary>
        private void CloseFormEventHandler( object sender, EventArgs e )
        {
            Close();
        }

        private void NoMedicarePrimaryForAutoAccidentEventHandler( object sender, EventArgs e )
        {
            IErrorMessageDisplayHandler messageDisplayHandler = new ErrorMessageDisplayHandler( Model as Account );
            DialogResult warningResult =
            messageDisplayHandler.DisplayYesNoErrorMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );

            if( warningResult != DialogResult.Yes )
            {
                DialogResult = DialogResult.OK;
                return;
            }

            PreviousSelectedInsurancePlan = ModifiedInsurancePlan;
        }

        private void tabControl_SelectedIndexChanged( object sender, EventArgs e )
        {
            ActivateTab();
            UpdateControlState();
        }

        private void FindInsurancePlanView_Load( object sender, EventArgs e )
        {
            ActivateTab();
            ActivateSearchField();
            UpdateControlState();
        }

        private void SelectedInsurancePlanChanged( object sender, EventArgs e )
        {
            UpdateControlState();
        }

        private void UpdateControlState()
        {
            button_OK.Enabled = ( CurrentView.PlanCanBeApplied );
        }

        private void OKButton_Click( object sender, EventArgs e )
        {
            RegisterRulesEvents();
            if( IsPrimary &&
                ModifiedInsurancePlan != null &&
                PreviousSelectedInsurancePlan != ModifiedInsurancePlan )
            {
                if( !RunRules() )
                {
                    UnRegisterRulesEvents();
                    return;
                }
            }

            UnRegisterRulesEvents();
            DialogResult = DialogResult.OK;
        }
        #endregion

        #region Methods
        #endregion

        #region Properties

        public Employer SelectedEmployer
        {
            get
            {
                return ( DialogResult == DialogResult.OK )
                    ? selectedEmployer : null;
            }
        }

        public InsurancePlan SelectedInsurancePlan
        {
            get
            {
                return ( DialogResult == DialogResult.OK )
                    ? CurrentView.SelectedInsurancePlan
                    : null;
            }
        }

        private InsurancePlan ModifiedInsurancePlan
        {
            get
            {
                return CurrentView.SelectedInsurancePlan;
            }
        }

        public bool IsPrimary
        {
            private get
            {
                return i_IsPrimary;
            }
            set
            {
                i_IsPrimary = value;
            }
        }

        public bool EnableOkButton
        {
            get { return button_OK.Enabled; } 
            set { button_OK.Enabled = value; }
        }

        #endregion

        #region Private Methods
        private void ActivateTab()
        {
            SelectByView prevView = CurrentView;

            if( tabControl.SelectedTab == tabPage_SelectByPayor )
            {
                CurrentView = selectByPayorBrokerView;
            }
            else
            {
                CurrentView = selectByEmployerView;
            }

            if( prevView != null )
            {
                CurrentView.SearchText = prevView.SearchText;
            }

            CurrentView.Activate();
        }

        private void ActivateSearchField()
        {
            CurrentView.ActivateSearchField();
        }

        private void RegisterRulesEvents()
        {
            RuleEngine.RegisterEvent<NoMedicarePrimaryPayorForAutoAccident>( Model, ModifiedInsurancePlan, NoMedicarePrimaryForAutoAccidentEventHandler) ;
        }

        private void UnRegisterRulesEvents()
        {
            // UNREGISTER EVENTS             
            RuleEngine.UnregisterEvent<NoMedicarePrimaryPayorForAutoAccident>( Model, NoMedicarePrimaryForAutoAccidentEventHandler );
        }

        private bool RunRules()
        {
            bool passedAllRules = RuleEngine.EvaluateRule<NoMedicarePrimaryPayorForAutoAccident>( Model, ModifiedInsurancePlan );

            return passedAllRules;
        }
        #endregion

        #region Private Properties
        public SelectByView CurrentView
        {
            get
            {
                return i_CurrentView;
            }
            private set
            {
                i_CurrentView = value;
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

        private InsurancePlan PreviousSelectedInsurancePlan
        {
            get
            {
                return i_PreviousSelectedInsurancePlan;
            }
            set
            {
                i_PreviousSelectedInsurancePlan = value;
            }
        }

        public MessageDisplayStateManager MessageDisplayStateManager
        {
            get
            {
                return IMessageDisplayStateManager;
            }

            internal set
            {
                IMessageDisplayStateManager = value;
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
            this.tabControl = new System.Windows.Forms.TabControl();

            this.tabPage_SelectByPayor = new System.Windows.Forms.TabPage();            
            this.tabPage_SelectByEmployer = new System.Windows.Forms.TabPage();

            this.selectByPayorBrokerView = new PatientAccess.UI.InsuranceViews.FindInsurancePlan.SelectByPayorBrokerView();
            this.selectByEmployerView = new PatientAccess.UI.InsuranceViews.FindInsurancePlan.SelectByEmployerView();

            this.panelBottom = new System.Windows.Forms.Panel();
            this.button_Cancel = new LoggingButton();
            this.button_OK = new LoggingButton();

            this.tabControl.SuspendLayout();
            this.tabPage_SelectByPayor.SuspendLayout();
            this.tabPage_SelectByEmployer.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add( this.tabPage_SelectByPayor );
            this.tabControl.Controls.Add( this.tabPage_SelectByEmployer );

            this.tabControl.Location = new System.Drawing.Point(8, 8);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(692, 511);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler( this.tabControl_SelectedIndexChanged );
            // 
            // selectByPayorBrokerView
            // 
            this.selectByPayorBrokerView.BackColor = System.Drawing.Color.White;
            this.selectByPayorBrokerView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectByPayorBrokerView.Location = new System.Drawing.Point(0, 0);
            this.selectByPayorBrokerView.Model = null;
            this.selectByPayorBrokerView.Name = "selectByPayorBrokerView";
            this.selectByPayorBrokerView.SearchText = "";
            this.selectByPayorBrokerView.SelectedInsurancePlan = null;
            this.selectByPayorBrokerView.Size = new System.Drawing.Size(584, 485);
            this.selectByPayorBrokerView.TabIndex = 0;
            this.selectByPayorBrokerView.ListViewLostFocusEvent += new System.EventHandler( this.ListViewLostFocusEventHandler );
            this.selectByPayorBrokerView.CloseFormOnEnterKeyEvent += new System.EventHandler( CloseFormOnEnterKeyEventHandler );
            this.selectByPayorBrokerView.SelectedInsurancePlanChanged += new System.EventHandler( this.SelectedInsurancePlanChanged );
            this.selectByPayorBrokerView.CloseFormEvent += new System.EventHandler( this.CloseFormEventHandler );           
            // 
            // selectByEmployerView
            // 
            this.selectByEmployerView.BackColor = System.Drawing.Color.White;
            this.selectByEmployerView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectByEmployerView.Location = new System.Drawing.Point(0, 0);
            this.selectByEmployerView.Model = null;
            this.selectByEmployerView.Name = "selectByEmployerView";
            this.selectByEmployerView.SearchText = "";
            this.selectByEmployerView.SelectedInsurancePlan = null;
            this.selectByEmployerView.Size = new System.Drawing.Size(584, 485);
            this.selectByEmployerView.TabIndex = 0;
            this.selectByEmployerView.CloseFormOnEnterKeyEvent += new System.EventHandler( CloseFormOnEnterKeyEventHandler );
            this.selectByEmployerView.SelectedInsurancePlanChanged += new System.EventHandler( this.SelectedInsurancePlanChanged );
            this.selectByEmployerView.CloseFormEvent += new System.EventHandler( this.CloseFormEventHandler );
            this.selectByEmployerView.CoveredGroupSelected += new EventHandler( selectByEmployerView_CoveredGroupSelected );

            // 
            // tabPage_SelectByPayor
            // 
            this.tabPage_SelectByPayor.BackColor = System.Drawing.Color.White;
            this.tabPage_SelectByPayor.Controls.Add(this.selectByPayorBrokerView);
            this.tabPage_SelectByPayor.Location = new System.Drawing.Point(4, 22);
            this.tabPage_SelectByPayor.Name = "tabPage_SelectByPayor";
            this.tabPage_SelectByPayor.Size = new System.Drawing.Size(684, 485);
            this.tabPage_SelectByPayor.TabIndex = 0;
            this.tabPage_SelectByPayor.Text = "Select by Payor/Broker";            
            // 
            // tabPage_SelectByEmployer
            // 
            this.tabPage_SelectByEmployer.BackColor = System.Drawing.Color.White;
            this.tabPage_SelectByEmployer.Controls.Add(this.selectByEmployerView);
            this.tabPage_SelectByEmployer.Location = new System.Drawing.Point(4, 22);
            this.tabPage_SelectByEmployer.Name = "tabPage_SelectByEmployer";
            this.tabPage_SelectByEmployer.Size = new System.Drawing.Size(684, 485);
            this.tabPage_SelectByEmployer.TabIndex = 1;
            this.tabPage_SelectByEmployer.Text = "Select by Employer";
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.button_Cancel);
            this.panelBottom.Controls.Add(this.button_OK);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 531);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(609, 39);
            this.panelBottom.TabIndex = 1;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(524, 8);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.TabIndex = 3;
            this.button_Cancel.Text = "Cancel";
            // 
            // button_OK
            // 
            this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            //this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_OK.Location = new System.Drawing.Point(441, 8);
            this.button_OK.Name = "button_OK";
            this.button_OK.TabIndex = 2;
            this.button_OK.Text = "OK";
            this.button_OK.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // FindInsurancePlanView
            // 
            this.AcceptButton = this.button_OK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(706, 570);
            this.Controls.Add( this.tabControl );
            this.Controls.Add( this.panelBottom );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindInsurancePlanView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find Insurance Plan";
            this.Load += new System.EventHandler( this.FindInsurancePlanView_Load );
            this.tabControl.ResumeLayout(false);
            this.tabPage_SelectByPayor.ResumeLayout(false);
            this.tabPage_SelectByEmployer.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Constructors and Finalization
        public FindInsurancePlanView()
        {
            InitializeComponent();

            EnableThemesOn( this );
        }

        public FindInsurancePlanView(Account anAccount)
        {
            Model = anAccount;

            InitializeComponent();

            selectByEmployerView.PatientAccount = anAccount;
            selectByPayorBrokerView.PatientAccount = anAccount;

            if( anAccount.Insurance != null &&
                anAccount.Insurance.PrimaryCoverage != null )
            {
                PreviousSelectedInsurancePlan = anAccount.Insurance.PrimaryCoverage.InsurancePlan;
            }

            EnableThemesOn( this );
        }

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

        #region Data Elements
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container               components = null;

        private LoggingButton           button_Cancel;
        private LoggingButton           button_OK;

        private TabControl              tabControl;

        private Panel                   panelBottom;

        private TabPage                 tabPage_SelectByPayor;
        private TabPage                 tabPage_SelectByEmployer;

        private SelectByView            i_CurrentView;
        private Employer                selectedEmployer = new Employer();

        private SelectByEmployerView    selectByEmployerView;
        private SelectByPayorBrokerView selectByPayorBrokerView;
        private RuleEngine              i_RuleEngine;
        private bool                    i_IsPrimary;
        private InsurancePlan           i_PreviousSelectedInsurancePlan;
        private MessageDisplayStateManager   IMessageDisplayStateManager;

        #endregion

        #region Constants
        #endregion

    }
}
