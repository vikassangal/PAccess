using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FinancialCounselingViews
{
    /// <summary>
    /// Summary description for UnInsuredFinancialCounselingView.
    /// </summary>
    //TODO: Create XML summary comment for UnInsuredFinancialCounselingView
    [Serializable]
    public class UnInsuredFinancialCounselingView : ControlView
    {
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        private void UnInsuredFinancialCounselingView_Enter( object sender, EventArgs e )
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

        private void UnInsuredFinancialCounselingView_Validating( object sender, CancelEventArgs e )
        {
            this.Model.TotalCurrentAmtDue = this.unInsuredAmountDueView1.EstimatedAmountDue;
            this.Model.RequestedPayment = this.unInsuredAmountDueView1.RequestedPayment;
            this.Model.ResourceListProvided = this.unInsuredAmountDueView1.ResourceListProvided;
        }

        private void previousAccountsView1_ChangeCursorToWaiting(object sender, EventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
        }

        private void previousAccountsView1_ChangeCursorToDefault(object sender, EventArgs e)
        {        
            this.Cursor = Cursors.Default;
            this.unInsuredAmountDueView1.AccountsWithoutPlanTotal = previousAccountsView1.AccountsWithoutPlanTotalBalance;
            this.unInsuredAmountDueView1.CalculateRequestedPayment();
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

            this.unInsuredAmountDueView1.Model = this.Model.Insurance;
            this.unInsuredAmountDueView1.EstimatedAmountDue = this.Model.TotalCurrentAmtDue;
            this.unInsuredAmountDueView1.AccountsWithoutPlanTotal = previousAccountsView1.AccountsWithoutPlanTotalBalance;
            this.unInsuredAmountDueView1.ResourceListProvided = this.Model.ResourceListProvided;
            this.unInsuredAmountDueView1.UpdateView();

            this.creditAdviceView1.Model = this.Model;
            this.creditAdviceView1.IsInsuredPatient = false;
            this.creditAdviceView1.UpdateView();

            this.lblUnInsured.Visible = true;
            this.lblUnInsured.BringToFront();
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
            this.lblUnInsured = new System.Windows.Forms.Label();
            this.previousAccountsView1 = new PatientAccess.UI.FinancialCounselingViews.PreviousAccountsView();
            this.unInsuredAmountDueView1 = new PatientAccess.UI.FinancialCounselingViews.UnInsuredAmountDueView();
            this.creditAdviceView1 = new PatientAccess.UI.FinancialCounselingViews.CreditAdviceView();
            this.SuspendLayout();
            // 
            // lblUnInsured
            // 
            this.lblUnInsured.Font = TITLE_FONT;
            this.lblUnInsured.ForeColor = TITLE_COLOR;
            this.lblUnInsured.Location = new System.Drawing.Point(8, 7);
            this.lblUnInsured.Name = "lblUnInsured";
            this.lblUnInsured.Size = new System.Drawing.Size(213, 26);
            this.lblUnInsured.TabIndex = 0;
            this.lblUnInsured.Text = "Uninsured Patient";
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
            this.previousAccountsView1.Size = new System.Drawing.Size(601, 347);
            this.previousAccountsView1.TabIndex = 1;
            this.previousAccountsView1.ChangeCursorToWaiting += new System.EventHandler(this.previousAccountsView1_ChangeCursorToWaiting);
            this.previousAccountsView1.ChangeCursorToDefault += new System.EventHandler(this.previousAccountsView1_ChangeCursorToDefault);
            // 
            // unInsuredAmountDueView1
            // 
            this.unInsuredAmountDueView1.AccountsWithoutPlanTotal = new System.Decimal(new int[] {
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0});
            this.unInsuredAmountDueView1.BackColor = System.Drawing.Color.White;
            this.unInsuredAmountDueView1.EstimatedAmountDue = new System.Decimal(new int[] {
                                                                                               0,
                                                                                               0,
                                                                                               0,
                                                                                               0});
            this.unInsuredAmountDueView1.Location = new System.Drawing.Point(607, 30);
            this.unInsuredAmountDueView1.Model = null;
            this.unInsuredAmountDueView1.Name = "unInsuredAmountDueView1";
            this.unInsuredAmountDueView1.RequestedPayment = new System.Decimal(new int[] {
                                                                                             0,
                                                                                             0,
                                                                                             0,
                                                                                             0});
            this.unInsuredAmountDueView1.ResourceListProvided = null;
            this.unInsuredAmountDueView1.Size = new System.Drawing.Size(266, 168);
            this.unInsuredAmountDueView1.TabIndex = 2;
            // 
            // creditAdviceView1
            // 
            this.creditAdviceView1.BackColor = System.Drawing.Color.White;
            this.creditAdviceView1.IsInsuredPatient = false;
            this.creditAdviceView1.Location = new System.Drawing.Point(611, 207);
            this.creditAdviceView1.Model = null;
            this.creditAdviceView1.Model_Account = null;
            this.creditAdviceView1.Name = "creditAdviceView1";
            this.creditAdviceView1.Size = new System.Drawing.Size(361, 148);
            this.creditAdviceView1.TabIndex = 3;
            // 
            // UnInsuredFinancialCounselingView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.creditAdviceView1);
            this.Controls.Add(this.unInsuredAmountDueView1);
            this.Controls.Add(this.previousAccountsView1);
            this.Controls.Add(this.lblUnInsured);
            this.Name = "UnInsuredFinancialCounselingView";
            this.Size = new System.Drawing.Size(982, 379);
            this.Enter += new System.EventHandler( this.UnInsuredFinancialCounselingView_Enter );
            this.Validating += new System.ComponentModel.CancelEventHandler( this.UnInsuredFinancialCounselingView_Validating );
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public UnInsuredFinancialCounselingView() : base()
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
        private Label lblUnInsured;
        private PreviousAccountsView previousAccountsView1;
        private UnInsuredAmountDueView unInsuredAmountDueView1;
        private CreditAdviceView creditAdviceView1;
        #endregion

        #region Constants
        private static System.Drawing.Font TITLE_FONT = new System.Drawing.Font( "Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, 0 );
        private static System.Drawing.Color TITLE_COLOR = System.Drawing.Color.FromArgb(255, 255, 153, 0);

        #endregion
    }
}
