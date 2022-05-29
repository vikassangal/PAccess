using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.InsuranceViews;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    partial class QuickInsuranceView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblStaticPlanId = new System.Windows.Forms.Label();
            this.lblPlanID = new System.Windows.Forms.Label();
            this.lblStaticPlanName = new System.Windows.Forms.Label();
            this.lblPlanName = new System.Windows.Forms.Label();
            this.btnEdit = new PatientAccess.UI.CommonControls.LoggingButton();
            this.findAPlanView = new PatientAccess.UI.InsuranceViews.FindAPlanView();
            this.btnReset = new PatientAccess.UI.CommonControls.LoggingButton();
            this.financialClassesView = new PatientAccess.UI.QuickAccountCreation.ViewImpl.QACFinancialClassesView();
            this.SuspendLayout();
            // 
            // lblStaticPlanId
            // 
            this.lblStaticPlanId.Location = new System.Drawing.Point(8, -1);
            this.lblStaticPlanId.Name = "lblStaticPlanId";
            this.lblStaticPlanId.Size = new System.Drawing.Size(48, 23);
            this.lblStaticPlanId.TabIndex = 0;
            this.lblStaticPlanId.Text = "Plan ID:";
            this.lblStaticPlanId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPlanID
            // 
            this.lblPlanID.Location = new System.Drawing.Point(70, -2);
            this.lblPlanID.Name = "lblPlanID";
            this.lblPlanID.Size = new System.Drawing.Size(150, 20);
            this.lblPlanID.TabIndex = 0;
            this.lblPlanID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticPlanName
            // 
            this.lblStaticPlanName.Location = new System.Drawing.Point(8, 20);
            this.lblStaticPlanName.Name = "lblStaticPlanName";
            this.lblStaticPlanName.Size = new System.Drawing.Size(65, 23);
            this.lblStaticPlanName.TabIndex = 0;
            this.lblStaticPlanName.Text = "Plan Name:";
            this.lblStaticPlanName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPlanName
            // 
            this.lblPlanName.Location = new System.Drawing.Point(70, 20);
            this.lblPlanName.Name = "lblPlanName";
            this.lblPlanName.Size = new System.Drawing.Size(285, 20);
            this.lblPlanName.TabIndex = 0;
            this.lblPlanName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPlanName.UseMnemonic = false;
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(288, 49);
            this.btnEdit.Message = null;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Edit...";
            this.btnEdit.Click += new System.EventHandler(this.EditOnClick);
            // 
            // findAPlanView
            // 
            this.findAPlanView.IsPrimary = true;
            this.findAPlanView.Location = new System.Drawing.Point(8, 40);
            this.findAPlanView.Model = null;
            this.findAPlanView.Name = "findAPlanView";
            this.findAPlanView.PatientAccount = null;
            this.findAPlanView.Size = new System.Drawing.Size(264, 75);
            this.findAPlanView.TabIndex = 0;
            this.findAPlanView.PlanSelectedEvent += new System.EventHandler<SelectInsuranceArgs>(this.FindAPlanViewOnPlanSelectedEvent);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(288, 85);
            this.btnReset.Message = null;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Clear All";
            this.btnReset.Click += new System.EventHandler(this.ResetOnClick);
            // 
            // financialClassesView
            // 
            this.financialClassesView.Location = new System.Drawing.Point(-5, 116);
            this.financialClassesView.Model = null;
            this.financialClassesView.Name = "financialClassesView";
            this.financialClassesView.Size = new System.Drawing.Size(296, 44);
            this.financialClassesView.TabIndex = 1;
            // 
            // QuickInsuranceView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblPlanName);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.findAPlanView);
            this.Controls.Add(this.lblStaticPlanName);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblPlanID);
            this.Controls.Add(this.lblStaticPlanId);
            this.Controls.Add(this.financialClassesView);
            this.Name = "QuickInsuranceView";
            this.Size = new System.Drawing.Size(374, 159);
            this.Load += new System.EventHandler(this.QuickInsuranceViewOnLoad);
            this.ResumeLayout(false);

        }
        #endregion
        #region Data Elements
         
        // Static text labels on left side of form
        private Label lblStaticPlanId;
        private Label lblStaticPlanName;
        // Dynamic labels updated with insurance information
        public Label lblPlanID;
        public Label lblPlanName;
        private LoggingButton btnReset;
        // Nested view objects
        private FindAPlanView findAPlanView;
        private LoggingButton btnEdit;
        public QACFinancialClassesView financialClassesView;
        private InsuranceDetails insuranceDetailsDialog;

        #endregion
    }
}
