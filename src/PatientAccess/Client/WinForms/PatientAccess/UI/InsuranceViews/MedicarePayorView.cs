using System;
using System.ComponentModel; 
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls; 
using PatientAccess.UI.InsuranceViews.Presenters;
using PatientAccess.UI.InsuranceViews.Views;


namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class MedicarePayorView : ControlView , IMBIView
    {
        #region Event Handlers

        public event EventHandler ResetButtonClicked;
        
        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void MedicarePayorView_Disposed(object sender, EventArgs e)
        {
            UnRegisterEvents();
            if (!MBINumberPresenter.ValidateMBINumber())
            {
                mtbMBINumber.UnMaskedText = "";
                MBINumberPresenter.UpdateModel();
            }
        }

        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>
       
        private void MedicareMBINumberPreferredEventHandler(object sender, EventArgs e)
        {
           
            SetMedicareMBINumberPreferred();
        }

        private void SetMedicareMBINumberPreferred()
        {
            UIColors.SetPreferredBgColor(mtbMBINumber);
        }

        private void SetMBINumberError()
        {
            UIColors.SetErrorBgColor(mtbMBINumber);
        }

        public void SetMBINumberNormalColor()
        {
            UIColors.SetNormalBgColor(this.mtbMBINumber);
        }
 
        public void setFocusToMBINumber()
        {
            mtbMBINumber.Focus();
        }

        private void MedicareMBINumberRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(mtbMBINumber);
        }

        private void MBINumber_Validating(object sender, CancelEventArgs e)
        {
            if (MBINumberPresenter.ValidateMBINumber())
            {

                if (Model_Coverage == null && savedGovernmentMedicareCoverage != null)
                {
                    Model_Coverage = savedGovernmentMedicareCoverage;
                }

                this.CheckForRequiredFields();
            }
        }

        private void btnReset_Click( object sender, EventArgs e )
        {
            
            mtbMBINumber.UnMaskedText = String.Empty;
            savedGovernmentMedicareCoverage = Model_Coverage;

            if( ResetButtonClicked != null )
            {
                ResetButtonClicked( sender, e );
            }

            //parentForm.ResetView();
            UpdateModel();
            CheckForRequiredFields();
        }
        
        #endregion

        #region Methods

        public override void UpdateView()
        {
            if (loadingModelData)
            {
                MBINumberPresenter = new MBIPresenter(this, new MessageBoxAdapter(), Model_Coverage,
                    RuleEngine.GetInstance());
                // Initial entry to the form -- initialize controls and get the data from the model.
                loadingModelData = false;
             
                MBINumberPresenter.UpdateView();
                 
                RuleEngine.LoadRules(Account);

                RuleEngine.GetInstance()
                    .RegisterEvent(typeof (MBINumberRequired), this.Model,
                        new EventHandler(MedicareMBINumberRequiredEventHandler));
                RuleEngine.GetInstance()
                    .RegisterEvent(typeof (MBINumberPreferred), this.Model,
                        new EventHandler(MedicareMBINumberPreferredEventHandler));
                
            }

            CheckForRequiredFields();
        }
        private void RegisterRules()
        {
          
            RuleEngine.RegisterEvent(typeof(MBINumberRequired), this.Model,
                 new EventHandler(MedicareMBINumberRequiredEventHandler));
            RuleEngine.RegisterEvent(typeof(MBINumberPreferred), this.Model,
                new EventHandler(MedicareMBINumberPreferredEventHandler));
        }

        public override void UpdateModel()
        {
            MBINumberPresenter.UpdateModel();
        }

        public void EnbleMBINumber()
        {
            this.lblMBINumber.Enabled = true;
            this.mtbMBINumber.Enabled = true;
        }
        public void DisableMBINumber()
        {
            this.lblMBINumber.Enabled = false;
            this.mtbMBINumber.Enabled = false;
            this.mtbMBINumber.UnMaskedText = string.Empty;
        }
      
        #endregion

        #region Properties
        [Browsable(false)]
        public GovernmentMedicareCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (GovernmentMedicareCoverage)this.Model;
            }
        }

        [Browsable(false)]
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

        public void ResetMBINumber()
        {
            MBINumber = String.Empty;
        }
 
        [Browsable(false)]
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
        public void CheckForRequiredFields()
        {
            
            UIColors.SetNormalBgColor(mtbMBINumber);
            RegisterRules();
            RuleEngine.GetInstance().EvaluateRule(typeof(MBINumberPreferred), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(MBINumberRequired), this.Model);
        }

        private void UnRegisterEvents()
        { 
            RuleEngine.GetInstance().UnregisterEvent(typeof(MBINumberRequired), this.Model, new EventHandler(MedicareMBINumberRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(MBINumberPreferred), this.Model, new EventHandler(MedicareMBINumberPreferredEventHandler));
     
        }
       
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
           
            this.btnReset = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.mtbMBINumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblMBINumber = new System.Windows.Forms.Label();
            this.SuspendLayout();
           
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(768, 180);
            this.btnReset.Message = null;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Clear All";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = "Plan Information";
            this.lineLabel.Location = new System.Drawing.Point(8, 1);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(864, 18);
            this.lineLabel.TabIndex = 0;
            this.lineLabel.TabStop = false;
            // 
            // mtbMBINumber
            // 
            this.mtbMBINumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mtbMBINumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMBINumber.KeyPressExpression = "^[a-zA-Z0-9]*$";
            this.mtbMBINumber.Location = new System.Drawing.Point(113, 43);
            this.mtbMBINumber.MaxLength = 13;
            this.mtbMBINumber.Mask = "    -   -    ";
            this.mtbMBINumber.Name = "mtbMBINumber";
            this.mtbMBINumber.Size = new System.Drawing.Size(96, 39);
            this.mtbMBINumber.TabIndex = 1;
            this.mtbMBINumber.ValidationExpression = "^[a-zA-Z0-9]*$";
            this.mtbMBINumber.Validating += new System.ComponentModel.CancelEventHandler(this.MBINumber_Validating);
 
            // 
            // lblMBINumber
            // 
            this.lblMBINumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMBINumber.Location = new System.Drawing.Point(32, 48);
            this.lblMBINumber.Name = "lblMBINumber";
            this.lblMBINumber.Size = new System.Drawing.Size(100, 29);
            this.lblMBINumber.TabIndex = 15;
            this.lblMBINumber.Text = "MBI:";
            // 
            // MedicarePayorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mtbMBINumber);
            this.Controls.Add(this.lblMBINumber);
           
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lineLabel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MedicarePayorView";
            this.Size = new System.Drawing.Size(880, 215);
            this.Disposed += new System.EventHandler(this.MedicarePayorView_Disposed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
 
        #endregion

        #region Construction and Finalization
        public MedicarePayorView( InsDetailPlanDetails parent )
        {
            parentForm = parent;
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
         
            base.EnableThemesOn( this );
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
        private LineLabel   lineLabel;
      
        private Account                                     i_Account;
        private GovernmentMedicareCoverage                  savedGovernmentMedicareCoverage;
        private InsDetailPlanDetails                        parentForm;
        private bool loadingModelData = true;
        private MaskedEditTextBox mtbMBINumber;
        private Label lblMBINumber;
        private RuleEngine                                  i_RuleEngine;
        private MBIPresenter MBINumberPresenter;
      
        #endregion

        #region Constants

        #endregion
 
        public string MBINumber
        {
            get { return mtbMBINumber.UnMaskedText.Trim(); }
            set { mtbMBINumber.UnMaskedText = value; }
        }

        void IMBIView.SetMBINumberError()
        {
            SetMBINumberError();

        }
    }
}
