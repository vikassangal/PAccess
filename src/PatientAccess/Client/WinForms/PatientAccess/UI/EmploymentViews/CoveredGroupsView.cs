using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.InsuranceViews.FindInsurancePlan;

namespace PatientAccess.UI.EmploymentViews
{
	/// <summary>
	/// Summary description for CoveredGroupsView.
	/// </summary>
    public class CoveredGroupsView  : ControlView
    {
        #region Events
        public event EventHandler SelectedCoveredGroupChanged;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Raise event for dialog's parent to catch new group
        /// </summary>
        private void coveredGroupListView1_SelectedCoveredGroupChanged( object sender, EventArgs e )
        {
            if( SelectedCoveredGroupChanged != null ) 
            {
                LooseArgs args = (LooseArgs) e;
                CoveredGroup cgp =  args.Context as CoveredGroup; 
                if( cgp != null && cgp.Employer != null )
                {
                    this.Model = cgp.Employer.AsEmployer();
                }
                else
                {
                    this.Model = null;
                }
               
                SelectedCoveredGroupChanged( this, args );
            }
        }  
        #endregion

        #region Methods
        public override void UpdateView()
        {
            this.labelPayorBrokerData.Text =  InsurancePlan.Payor.Name;
            this.labelPlanData.Text =  InsurancePlan.PlanName;
            
            IInsuranceBroker insBroker =
                BrokerFactory.BrokerOfType<IInsuranceBroker>();

            ICollection coveredGroups = insBroker.CoveredGroupsFor( this.InsurancePlan.PlanID, this.InsurancePlan.EffectiveOn, 
                this.InsurancePlan.ApprovedOn, Account.Facility.Oid, Account.AdmitDate );      
      
            this.coveredGroupListView1.DisplayCoveredGroups( coveredGroups );

            this.coveredGroupListView1.Focus();
        }
        #endregion

        #region Properties
        public Employer Model_Employer
        {
            get
            {
                if( this.Model != null )
                {
                    return (Employer)this.Model;
                }
                else
                    return null;                
            }
            set
            {
                this.Model = value;
            }
        }
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

	    private InsurancePlan InsurancePlan
        {
            get
            {
                if( Account != null )
                {
                    return Account.Insurance.CoverageFor( 
                        CoverageOrder.PRIMARY_OID ).InsurancePlan;
                }
                else
                {
                    return null;
                }
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
            this.lineLabel1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.panelCoveredgroupHeader = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.labelPlanData = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelPayorBrokerData = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.coveredGroupListView1 = new PatientAccess.UI.InsuranceViews.FindInsurancePlan.CoveredGroupListView();
            this.panelCoveredgroupHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // lineLabel1
            // 
            this.lineLabel1.Caption = "Covered Groups";
            this.lineLabel1.Location = new System.Drawing.Point(8, 8);
            this.lineLabel1.Name = "lineLabel1";
            this.lineLabel1.Size = new System.Drawing.Size(368, 18);
            this.lineLabel1.TabIndex = 0;
            this.lineLabel1.TabStop = false;
            // 
            // panelCoveredgroupHeader
            // 
            this.panelCoveredgroupHeader.Controls.Add(this.label5);
            this.panelCoveredgroupHeader.Controls.Add(this.labelPlanData);
            this.panelCoveredgroupHeader.Controls.Add(this.label3);
            this.panelCoveredgroupHeader.Controls.Add(this.labelPayorBrokerData);
            this.panelCoveredgroupHeader.Controls.Add(this.label1);
            this.panelCoveredgroupHeader.Controls.Add(this.lineLabel1);
            this.panelCoveredgroupHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCoveredgroupHeader.Location = new System.Drawing.Point(0, 0);
            this.panelCoveredgroupHeader.Name = "panelCoveredgroupHeader";
            this.panelCoveredgroupHeader.Size = new System.Drawing.Size(384, 128);
            this.panelCoveredgroupHeader.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(368, 40);
            this.label5.TabIndex = 5;
            this.label5.Text = "The covered groups listed below are associated with the selected plan. Select a c" +
                "overed group if appropriate, or click the Employers tab to select an employer th" +
                "at is not listed below.";
            // 
            // labelPlanData
            // 
            this.labelPlanData.Location = new System.Drawing.Point(88, 56);
            this.labelPlanData.Name = "labelPlanData";
            this.labelPlanData.Size = new System.Drawing.Size(264, 16);
            this.labelPlanData.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Plan:";
            // 
            // labelPayorBrokerData
            // 
            this.labelPayorBrokerData.Location = new System.Drawing.Point(88, 32);
            this.labelPayorBrokerData.Name = "labelPayorBrokerData";
            this.labelPayorBrokerData.Size = new System.Drawing.Size(224, 16);
            this.labelPayorBrokerData.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Payor/broker:";
            // 
            // coveredGroupListView1
            // 
            this.coveredGroupListView1.CoveredGroups = null;
            this.coveredGroupListView1.CoverMessage = "No Items Found";
            this.coveredGroupListView1.CoverPadding = 40;            
            this.coveredGroupListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.coveredGroupListView1.DockPadding.All = 5;
            this.coveredGroupListView1.Location = new System.Drawing.Point(0, 128);
            this.coveredGroupListView1.Model = null;
            this.coveredGroupListView1.Name = "coveredGroupListView1";
            this.coveredGroupListView1.SelectedCoveredGroup = null;
            this.coveredGroupListView1.ShowCover = true;
            this.coveredGroupListView1.Size = new System.Drawing.Size(384, 360);
            this.coveredGroupListView1.TabIndex = 4;
            // 
            // CoveredGroupsView
            // 
            this.Controls.Add(this.coveredGroupListView1);
            this.Controls.Add(this.panelCoveredgroupHeader);
            this.Name = "CoveredGroupsView";
            this.Size = new System.Drawing.Size(384, 488);
            this.panelCoveredgroupHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

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

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CoveredGroupsView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            this.coveredGroupListView1.SelectedCoveredGroupChanged += 
                new EventHandler(coveredGroupListView1_SelectedCoveredGroupChanged);

        }
        #endregion

        #region Data Elements
        private LineLabel lineLabel1;
        private Label label1;
        private Label label3;
        private Label label5;
        private Label labelPayorBrokerData;
        private Label labelPlanData;
        private Panel panelCoveredgroupHeader;
        private Account i_Account = null;
        private CoveredGroupListView coveredGroupListView1;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        #endregion

        #region Constants
        #endregion
    }
}
