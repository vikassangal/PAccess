using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class WorkersCompPayorView : ControlView
    {
        #region Event Handlers
        public event EventHandler ResetButtonClicked;

        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void WorkersCompView_Disposed(object sender, EventArgs e)
        {
            UnRegisterEvents();
        }

        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>
        private void WorkersCompPolicyNumberPreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( mskEditPolicyNumber );
        }

        private void WorkersCompPolicyNumberRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor( mskEditPolicyNumber );
        }

        private void WorkersCompEmpSupervisorRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor( mskEditSupervisor );
        }

        // End event handlers for Required/Preferred fields

        private void MaskedlditPolicyNumberValidating(object sender, CancelEventArgs e)
        {
            if( Model_Coverage == null && savedWorkersCompensationCoverage != null )
            {
                Model_Coverage = savedWorkersCompensationCoverage;
            }
            Model_Coverage.PolicyNumber = mskEditPolicyNumber.UnMaskedText;

            UIColors.SetNormalBgColor(this.mskEditPolicyNumber);

            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompPolicyNumberPreferred ), this.Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompPolicyNumberRequired ), this.Model );
        }

        private void MaskedEditSupervisorValidating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor( this.mskEditSupervisor );

            if( Model_Coverage == null && savedWorkersCompensationCoverage != null )
            {
                Model_Coverage = savedWorkersCompensationCoverage;
            }

            Model_Coverage.PatientsSupervisor = mskEditSupervisor.UnMaskedText.Trim();

            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompEmpSupervisorRequired ), this.Model_Coverage );
        }

        private void MaskEditAdjusterValidating(object sender, CancelEventArgs e)
        {
            if( Model_Coverage == null && savedWorkersCompensationCoverage != null )
            {
                Model_Coverage = savedWorkersCompensationCoverage;
            }
            Model_Coverage.InsuranceAdjuster = maskEditAdjuster.UnMaskedText.Trim();
        }

        

        private void ButonResetClick( object sender, EventArgs e )
        {
            mskEditSupervisor.UnMaskedText           = String.Empty;
            mskEditPolicyNumber.UnMaskedText         = String.Empty;
            maskEditAdjuster.UnMaskedText            = String.Empty;

            savedWorkersCompensationCoverage         = Model_Coverage;
            
            if( ResetButtonClicked != null )
            {
                ResetButtonClicked( sender, e );
            }

            UpdateModel();
            CheckForRequiredFields();
        }
        #endregion

        #region Methods

        public override void UpdateModel()
        {

            Model_Coverage.PolicyNumber                 = mskEditPolicyNumber.UnMaskedText.Trim();
            Model_Coverage.PatientsSupervisor           = mskEditSupervisor.UnMaskedText.Trim();
            Model_Coverage.InsuranceAdjuster            = maskEditAdjuster.UnMaskedText.Trim();
        }


        public override void UpdateView()
        {
            if( loadingModelData )
            {   // Initial entry to the form -- initialize controls and get the data from the model.
                loadingModelData = false;

                if( Model_Coverage == null )
                {
                    return;
                }
                if( !string.IsNullOrEmpty(Model_Coverage.PolicyNumber) )
                {
                    mskEditPolicyNumber.UnMaskedText = Model_Coverage.PolicyNumber.Trim();
                }
                if( !string.IsNullOrEmpty(Model_Coverage.PatientsSupervisor) )
                {
                    mskEditSupervisor.UnMaskedText = Model_Coverage.PatientsSupervisor.Trim();
                }
                if( !string.IsNullOrEmpty(Model_Coverage.InsuranceAdjuster))
                {
                    maskEditAdjuster.UnMaskedText = Model_Coverage.InsuranceAdjuster.Trim();
                }
                

                RuleEngine.LoadRules( Account );
                RuleEngine.GetInstance().RegisterEvent( typeof( WorkersCompPolicyNumberPreferred ), this.Model, new EventHandler( WorkersCompPolicyNumberPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( WorkersCompPolicyNumberRequired ), this.Model, new EventHandler( WorkersCompPolicyNumberRequiredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( WorkersCompEmpSupervisorRequired ), this.Model, new EventHandler( WorkersCompEmpSupervisorRequiredEventHandler ) );
             
            }

            CheckForRequiredFields();
        }
        #endregion

        #region Properties
        [Browsable(false)]
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

        [Browsable(false)]
        public Account Account
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
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
        /// <summary>
        /// CheckForRequiredFields - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void CheckForRequiredFields()
        {
            UIColors.SetNormalBgColor( this.mskEditPolicyNumber );
            UIColors.SetNormalBgColor( this.mskEditSupervisor );
            
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompPolicyNumberPreferred ), this.Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompPolicyNumberRequired ), this.Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompEmpSupervisorRequired ), this.Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( WorkersCompAuthCodePreferred ), this.Model );
           
        }

        private void UnRegisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( WorkersCompPolicyNumberPreferred ), this.Model, WorkersCompPolicyNumberPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( WorkersCompPolicyNumberRequired), this.Model, WorkersCompPolicyNumberRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( WorkersCompEmpSupervisorRequired), this.Model, WorkersCompEmpSupervisorRequiredEventHandler );
            
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.controlPanel = new System.Windows.Forms.Panel();
            this.maskEditAdjuster = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mskEditPolicyNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mskEditSupervisor = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblAdjuster = new System.Windows.Forms.Label();
            this.lblSupervisor = new System.Windows.Forms.Label();
            this.lblPolicy = new System.Windows.Forms.Label();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.authorizationPanel = new System.Windows.Forms.Panel();
            this.btnReset = new PatientAccess.UI.CommonControls.LoggingButton();
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.maskEditAdjuster);
            this.controlPanel.Controls.Add(this.mskEditPolicyNumber);
            this.controlPanel.Controls.Add(this.mskEditSupervisor);
            this.controlPanel.Controls.Add(this.lblAdjuster);
            this.controlPanel.Controls.Add(this.lblSupervisor);
            this.controlPanel.Controls.Add(this.lblPolicy);
            this.controlPanel.Location = new System.Drawing.Point(12, 32);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(380, 88);
            this.controlPanel.TabIndex = 0;
            // 
            // maskEditAdjuster
            // 
            this.maskEditAdjuster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.maskEditAdjuster.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditAdjuster.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.maskEditAdjuster.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditAdjuster.Location = new System.Drawing.Point(115, 64);
            this.maskEditAdjuster.Mask = "";
            this.maskEditAdjuster.MaxLength = 35;
            this.maskEditAdjuster.Name = "maskEditAdjuster";
            this.maskEditAdjuster.Size = new System.Drawing.Size(260, 20);
            this.maskEditAdjuster.TabIndex = 2;
            this.maskEditAdjuster.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditAdjuster.Validating += new System.ComponentModel.CancelEventHandler(this.MaskEditAdjusterValidating);
            // 
            // mskEditPolicyNumber
            // 
            this.mskEditPolicyNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mskEditPolicyNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskEditPolicyNumber.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEditPolicyNumber.Location = new System.Drawing.Point(115, 0);
            this.mskEditPolicyNumber.Mask = "";
            this.mskEditPolicyNumber.MaxLength = 12;
            this.mskEditPolicyNumber.Name = "mskEditPolicyNumber";
            this.mskEditPolicyNumber.Size = new System.Drawing.Size(96, 20);
            this.mskEditPolicyNumber.TabIndex = 0;
            this.mskEditPolicyNumber.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEditPolicyNumber.Validating += new System.ComponentModel.CancelEventHandler(this.MaskedlditPolicyNumberValidating);
            // 
            // mskEditSupervisor
            // 
            this.mskEditSupervisor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.mskEditSupervisor.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mskEditSupervisor.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskEditSupervisor.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEditSupervisor.Location = new System.Drawing.Point(115, 32);
            this.mskEditSupervisor.Mask = "";
            this.mskEditSupervisor.MaxLength = 35;
            this.mskEditSupervisor.Name = "mskEditSupervisor";
            this.mskEditSupervisor.Size = new System.Drawing.Size(260, 20);
            this.mskEditSupervisor.TabIndex = 1;
            this.mskEditSupervisor.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEditSupervisor.Validating += new System.ComponentModel.CancelEventHandler(this.MaskedEditSupervisorValidating);
            // 
            // lblAdjuster
            // 
            this.lblAdjuster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAdjuster.Location = new System.Drawing.Point(0, 67);
            this.lblAdjuster.Name = "lblAdjuster";
            this.lblAdjuster.Size = new System.Drawing.Size(100, 23);
            this.lblAdjuster.TabIndex = 2;
            this.lblAdjuster.Text = "Adjuster:";
            // 
            // lblSupervisor
            // 
            this.lblSupervisor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSupervisor.Location = new System.Drawing.Point(0, 35);
            this.lblSupervisor.Name = "lblSupervisor";
            this.lblSupervisor.Size = new System.Drawing.Size(122, 23);
            this.lblSupervisor.TabIndex = 1;
            this.lblSupervisor.Text = "Employee\'s supervisor:";
            // 
            // lblPolicy
            // 
            this.lblPolicy.Location = new System.Drawing.Point(0, 3);
            this.lblPolicy.Name = "lblPolicy";
            this.lblPolicy.Size = new System.Drawing.Size(100, 23);
            this.lblPolicy.TabIndex = 0;
            this.lblPolicy.Text = "Policy number:";
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = "Plan Information";
            this.lineLabel.Location = new System.Drawing.Point(8, 1);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(864, 18);
            this.lineLabel.TabIndex = 1;
            this.lineLabel.TabStop = false;
            // 
            // authorizationPanel
            // 
            this.authorizationPanel.Location = new System.Drawing.Point(512, 24);
            this.authorizationPanel.Name = "authorizationPanel";
            this.authorizationPanel.Size = new System.Drawing.Size(320, 56);
            this.authorizationPanel.TabIndex = 2;
            // 
            // btnReset
            // 
            this.btnReset.CausesValidation = false;
            this.btnReset.Location = new System.Drawing.Point(768, 180);
            this.btnReset.Message = null;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Clear All";
            this.btnReset.Click += new System.EventHandler(this.ButonResetClick);
            // 
            // WorkersCompPayorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.authorizationPanel);
            this.Controls.Add(this.lineLabel);
            this.Controls.Add(this.controlPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "WorkersCompPayorView";
            this.Size = new System.Drawing.Size(880, 215);
            this.Disposed += new System.EventHandler(this.WorkersCompView_Disposed);
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public WorkersCompPayorView( InsDetailPlanDetails parent )
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn( this );
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
        private Container             components = null;
        private LoggingButton                 btnReset;
        private Label                  lblPolicy;
        private Label                  lblSupervisor;
        private Label lblAdjuster;
        private Panel                  controlPanel;
        private Panel                  authorizationPanel;
        private LineLabel   lineLabel;
        private MaskedEditTextBox    mskEditSupervisor;
        private MaskedEditTextBox mskEditPolicyNumber;
        private MaskedEditTextBox    maskEditAdjuster;
        private WorkersCompensationCoverage                 savedWorkersCompensationCoverage;
        private Account                                     i_Account;
        private bool                                        loadingModelData = true;
        private RuleEngine                                  i_RuleEngine;
        #endregion

        #region Constants
        #endregion
    }
}
