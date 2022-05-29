using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Rules;

namespace PatientAccess.UI.FinancialCounselingViews
{
    /// <summary>
    /// Summary description for InsuredFinancialCounselingView.
    /// </summary>
    //TODO: Create XML summary comment for InsuredFinancialCounselingView
    [Serializable]
    public class InsuredFinancialCounselingView : ControlView
    {
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        private void InsuredFinancialCounselingView_Enter( object sender, EventArgs e )
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
                        EnableInsuranceTab( this, new LooseArgs( Model ) );
                    }
                }
            }
        }

        private void InsuredFinancialCounselingView_Validating( object sender, CancelEventArgs e )
        {
            this.Model.TotalCurrentAmtDue = this.insuredAmountDueView1.TotalCurrentAmountDue;
            this.Model.RequestedPayment = this.insuredAmountDueView1.RequestedPayment;
        }

        private void previousAccountsView1_ChangeCursorToWaiting(object sender, EventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
        }

        private void previousAccountsView1_ChangeCursorToDefault(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            this.insuredAmountDueView1.AccountsWithoutPlanTotal = previousAccountsView1.AccountsWithoutPlanTotalBalance;                    
            this.insuredAmountDueView1.CalculateAndDisplayTotal();
        }
        #endregion

        #region Methods
        /// <summary>
        /// UpdateView with data from model
        /// </summary>
        public override void UpdateView()
        {
            this.previousAccountsView1.Model = this.Model;
            this.previousAccountsView1.UpdateView();
    
            this.insuredAmountDueView1.Model = this.Model.Insurance;
            this.insuredAmountDueView1.AccountsWithoutPlanTotal = previousAccountsView1.AccountsWithoutPlanTotalBalance;

            if( this.Model.PrimaryInsured != null )
            {
                this.insuredAmountDueView1.InsuredsName = this.Model.PrimaryInsured.FormattedName;
            }
            else
            {
                this.insuredAmountDueView1.InsuredsName = String.Empty;
            }
            if (Model.FinancialClass.Code == FinancialClass.MCARE_MCD_CROSSOVER_CODE)
            {
                insuredAmountDueView1.DisablePatientHasNoLiability();
            }
            else
            {
                insuredAmountDueView1.EnablePatientHasNoLiability();
            }

            UpdatePatientLiabilityForFinancialClass17();
            
            Model.Insurance.HasNoLiability = Model.FinancialClass.Code == FinancialClass.MCARE_MCD_CROSSOVER_CODE || Model.Insurance.HasNoLiability;
            
            this.insuredAmountDueView1.UpdateView();

            this.creditAdviceView1.Model = this.Model;
            this.creditAdviceView1.IsInsuredPatient = true;
            this.creditAdviceView1.UpdateView();
            lblInsuredPatient.Visible = true;
            lblInsuredPatient.BringToFront();
        }

        private void UpdatePatientLiabilityForFinancialClass17()
        {
            // Auto Complete No Liability Due flag rule for phoenix facility
            if ( Model != null && Model.FinancialClass!=null )
            {
                if ( AutoCompleteNoLiabilityDueFeatureManager.IsAccountCreatedAfterImplementationDate(Model)
                    && Model.FinancialClass.IsMedicaidManagedFC17 )
                {
                    ResetPatientLiability();
                }
                if ( AutoCompleteNoLiabilityDueFeatureManager.IsAccountCreatedBeforeImplementationDate(Model) )
                {
                   if (Model.HasFinancialClassChanged
                        && Model.FinancialClass.IsMedicaidManagedFC17 &&
                       AutoCompleteNoLiabilityDueFeatureManager.IsFeatureEnabledForToday )
                   {
                       ResetPatientLiability();
                   }
                }
            }
        }

        private void ResetPatientLiability()
        {
            insuredAmountDueView1.DisablePatientHasNoLiability();
            Model.Insurance.HasNoLiability = true;

        }
        /// <summary>
        /// UpdateModel method
        /// </summary>
        public override void UpdateModel()
        {   

        }
        #endregion

        #region Properties

        private new Account Model
        {
            get
            {
                return (Account)base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        #endregion

        #region Private Methods

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInsuredPatient = new System.Windows.Forms.Label();
            this.creditAdviceView1 = new PatientAccess.UI.FinancialCounselingViews.CreditAdviceView();
            this.insuredAmountDueView1 = new PatientAccess.UI.FinancialCounselingViews.InsuredAmountDueView();
            this.previousAccountsView1 = new PatientAccess.UI.FinancialCounselingViews.PreviousAccountsView();
            this.SuspendLayout();
            // 
            // lblInsuredPatient
            // 
            this.lblInsuredPatient.Font = TitleFont;
            this.lblInsuredPatient.ForeColor = TitleColor;
            this.lblInsuredPatient.Location = new System.Drawing.Point(9, 7);
            this.lblInsuredPatient.Name = "lblInsuredPatient";
            this.lblInsuredPatient.Size = new System.Drawing.Size(173, 26);
            this.lblInsuredPatient.TabIndex = 0;
            this.lblInsuredPatient.Text = "Insured Patient";
            // 
            // creditAdviceView1
            // 
            this.creditAdviceView1.BackColor = System.Drawing.Color.White;
            this.creditAdviceView1.IsInsuredPatient = false;
            this.creditAdviceView1.Location = new System.Drawing.Point(611, 207);
            this.creditAdviceView1.Model = null;
            this.creditAdviceView1.Model_Account = null;
            this.creditAdviceView1.Name = "creditAdviceView1";
            this.creditAdviceView1.Size = new System.Drawing.Size(382, 146);
            this.creditAdviceView1.TabIndex = 3;
            // 
            // insuredAmountDueView1
            // 
            this.insuredAmountDueView1.AccountsWithoutPlanTotal = new System.Decimal(new int[] {
                                                                                                   0,
                                                                                                   0,
                                                                                                   0,
                                                                                                   0});
            this.insuredAmountDueView1.BackColor = System.Drawing.Color.White;
            this.insuredAmountDueView1.InsuredsName = null;
            this.insuredAmountDueView1.Location = new System.Drawing.Point(607, 30);
            this.insuredAmountDueView1.Model = null;
            this.insuredAmountDueView1.Name = "insuredAmountDueView1";
            this.insuredAmountDueView1.RequestedPayment = new System.Decimal(new int[] {
                                                                                           0,
                                                                                           0,
                                                                                           0,
                                                                                           0});
            this.insuredAmountDueView1.Size = new System.Drawing.Size(387, 171);
            this.insuredAmountDueView1.TabIndex = 2;
            this.insuredAmountDueView1.TotalCurrentAmountDue = new System.Decimal(new int[] {
                                                                                                0,
                                                                                                0,
                                                                                                0,
                                                                                                0});
            // 
            // previousAccountsView1
            // 
            this.previousAccountsView1.AccountsWithoutPlanTotalBalance = new System.Decimal(new int[] {
                                                                                                          0,
                                                                                                          0,
                                                                                                          0,
                                                                                                          0});
            this.previousAccountsView1.BackColor = System.Drawing.Color.White;
            this.previousAccountsView1.Location = new System.Drawing.Point(0, 29);
            this.previousAccountsView1.Model = null;
            this.previousAccountsView1.Name = "previousAccountsView1";
            this.previousAccountsView1.Size = new System.Drawing.Size(605, 347);
            this.previousAccountsView1.TabIndex = 1;
            this.previousAccountsView1.ChangeCursorToWaiting += new System.EventHandler(this.previousAccountsView1_ChangeCursorToWaiting);
            this.previousAccountsView1.ChangeCursorToDefault += new System.EventHandler(this.previousAccountsView1_ChangeCursorToDefault);
            // 
            // InsuredFinancialCounselingView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.previousAccountsView1);
            this.Controls.Add(this.insuredAmountDueView1);
            this.Controls.Add(this.creditAdviceView1);
            this.Controls.Add(this.lblInsuredPatient);
            this.Name = "InsuredFinancialCounselingView";
            this.Size = new System.Drawing.Size(1005, 383);
            this.Enter += new System.EventHandler( this.InsuredFinancialCounselingView_Enter );
            this.Validating += new System.ComponentModel.CancelEventHandler( this.InsuredFinancialCounselingView_Validating );
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        
        private IAutoCompleteNoLiabilityDueFeatureManager AutoCompleteNoLiabilityDueFeatureManager
        {
            get
            {
                return autoCompleteNoLiabilityDueFeatureManager ?? (autoCompleteNoLiabilityDueFeatureManager =
                           new AutoCompleteNoLiabilityDueFeatureManager());
            }
        }
        
        #endregion

        #region Construction and Finalization
        public InsuredFinancialCounselingView() : base()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
            base.EnableThemesOn( this );
        }

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
        private Container components = null;
        private PreviousAccountsView previousAccountsView1;
        private CreditAdviceView creditAdviceView1;
        private InsuredAmountDueView insuredAmountDueView1;
        private Label lblInsuredPatient;
        private IAutoCompleteNoLiabilityDueFeatureManager autoCompleteNoLiabilityDueFeatureManager;
        #endregion

        #region Constants
        private static readonly System.Drawing.Font TitleFont = new System.Drawing.Font( "Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, 0 );
        private static readonly System.Drawing.Color TitleColor = System.Drawing.Color.FromArgb( 255, 255, 153, 0 );
        #endregion
    }
}
