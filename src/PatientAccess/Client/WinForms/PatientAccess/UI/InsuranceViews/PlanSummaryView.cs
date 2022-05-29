using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class PlanSummaryView : ControlView
    {
        #region Events
        
        #endregion

        #region Event Handlers
        private void PlanSummaryView_Load( object sender, EventArgs e )
        {   

        }

        #endregion

        #region Methods
        public void ResetView()
        {
            lblPlanIdentifier.Text = String.Empty;
            txtBilling.Text        = String.Empty;
          //  btnEdit.Enabled        = false;
            Model_Coverage         = null;
            UpdateView();
        }

        public override void UpdateView()
        {
            if( Model_Coverage == null )
            {
                txtBilling.Clear();
                return;
            }
            else if( Model_Coverage.InsurancePlan != null )
            {
                if( this.Model_Coverage.BillingInformation.Address != null )
                {
                    string mailingLabel = this.Model_Coverage.BillingInformation.Address.AsMailingLabel();

                    // Display billing address
                    if( mailingLabel != null && mailingLabel.Length > 0 )
                    {
                        txtBilling.Text = mailingLabel;
                        //             btnEdit.Enabled = true;
                    }
                }                                
            }
            
            // Get the plan associated number
            string kindOfAssocNumber = Model_Coverage.KindOfAssociatedNumber;
            
            // Get the plan number
            string planNumber = Model_Coverage.AssociatedNumber;
            
            // Construct a plan identifier display string 
            if( kindOfAssocNumber != null && planNumber != null )
            {
                if( kindOfAssocNumber.Length > 0 && planNumber.Length > 0 )
                {
                    string planIdDisplayString = kindOfAssocNumber + ": " + planNumber;
                    lblPlanIdentifier.Text = planIdDisplayString;
                }
            }
        }
        #endregion

        #region Properties
        public Coverage Model_Coverage
        {
            private get
            {
                return (Coverage)this.Model;
            }
            set
            {
                this.Model = value;
            }
        }
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
        #endregion

        #region Private Methods
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.grpPlan = new System.Windows.Forms.GroupBox();
            this.txtBilling = new System.Windows.Forms.TextBox();
            this.lblStaticBilling = new System.Windows.Forms.Label();
            this.lblPlanIdentifier = new System.Windows.Forms.Label();
            this.grpPlan.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPlan
            // 
            this.grpPlan.Controls.Add(this.txtBilling);
            this.grpPlan.Controls.Add(this.lblStaticBilling);
            this.grpPlan.Controls.Add(this.lblPlanIdentifier);
            this.grpPlan.Location = new System.Drawing.Point(0, 0);
            this.grpPlan.Name = "grpPlan";
            this.grpPlan.Size = new System.Drawing.Size(300, 90);
            this.grpPlan.TabIndex = 0;
            this.grpPlan.TabStop = false;
            this.grpPlan.Text = "Payor Details";
            // 
            // txtBilling
            // 
            this.txtBilling.BackColor = System.Drawing.SystemColors.Window;
            this.txtBilling.Location = new System.Drawing.Point(48, 32);
            this.txtBilling.Multiline = true;
            this.txtBilling.Name = "txtBilling";
            this.txtBilling.ReadOnly = true;
            this.txtBilling.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBilling.Size = new System.Drawing.Size(160, 48);
            this.txtBilling.TabIndex = 3;
            this.txtBilling.TabStop = false;
            this.txtBilling.Text = "";
            // 
            // lblStaticBilling
            // 
            this.lblStaticBilling.Location = new System.Drawing.Point(8, 40);
            this.lblStaticBilling.Name = "lblStaticBilling";
            this.lblStaticBilling.Size = new System.Drawing.Size(38, 24);
            this.lblStaticBilling.TabIndex = 2;
            this.lblStaticBilling.Text = "Billing:";
            this.lblStaticBilling.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPlanIdentifier
            // 
            this.lblPlanIdentifier.Location = new System.Drawing.Point(8, 16);
            this.lblPlanIdentifier.Name = "lblPlanIdentifier";
            this.lblPlanIdentifier.Size = new System.Drawing.Size(282, 16);
            this.lblPlanIdentifier.TabIndex = 1;
            this.lblPlanIdentifier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PlanSummaryView
            // 
            this.Controls.Add(this.grpPlan);
            this.Name = "PlanSummaryView";
            this.Size = new System.Drawing.Size(300, 100);
            this.Load += new System.EventHandler(this.PlanSummaryView_Load);
            this.grpPlan.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PlanSummaryView()
        {
            InitializeComponent();
       //     this.btnEdit.Enabled = false;
        }

        public PlanSummaryView( object model ) : this()
        {
            this.Model = model;
      //      this.btnEdit.Enabled = false;
            this.UpdateView();
        }

        protected override void Dispose( bool disposing )
        {
            if ( IsHandleCreated )
            {
                Application.DoEvents();
            }

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
        private Container components = null;
        private GroupBox   grpPlan;
        private Label      lblStaticBilling;
        private TextBox    txtBilling;
        private Label      lblPlanIdentifier;
        
        private Account                         i_Account;
        #endregion

        #region Constants
        private const int                       PLAN_DETAILS_PAGE = 0;
        #endregion
    }
}