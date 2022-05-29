using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms; 
using PatientAccess.UI.InsuranceViews.DOFR.Presenter;
using PatientAccess.UI.InsuranceViews.DOFR.View;

namespace PatientAccess.UI.InsuranceViews.DOFR.ViewImpl
{
    /// <summary>
    /// Summary description for GenderView.
    /// </summary>
    public class DOFRInitiateView : ControlView, IDOFRInitiateView
    {

        #region Construction and Finalization
        public DOFRInitiateView()
        {
            // This call is required by the Windows.Forms Form Designer.
            
            InitializeComponent();
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDOFR = new System.Windows.Forms.Label();
            this.btnDOFRInitiate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDOFR
            // 
            this.lblDOFR.AutoSize = true;
            this.lblDOFR.Location = new System.Drawing.Point(0, 5);
            this.lblDOFR.Margin = new System.Windows.Forms.Padding(0);
            this.lblDOFR.Name = "lblDOFR";
            this.lblDOFR.Size = new System.Drawing.Size(116, 13);
            this.lblDOFR.TabIndex = 1;
            this.lblDOFR.Text = "Decision Analysis Tool:";
            // 
            // btnDOFRInitiate
            // 
            this.btnDOFRInitiate.Enabled = false;
            this.btnDOFRInitiate.Location = new System.Drawing.Point(111, 1);
            this.btnDOFRInitiate.Name = "btnDOFRInitiate";
            this.btnDOFRInitiate.Size = new System.Drawing.Size(79, 23);
            this.btnDOFRInitiate.TabIndex = 0;
            this.btnDOFRInitiate.Text = "Initiate DOFR";
            this.btnDOFRInitiate.UseVisualStyleBackColor = true;
            this.btnDOFRInitiate.Click += new System.EventHandler(this.btnDOFRInitiate_Click);
            // 
            // DOFRInitiateView
            // 
            this.Controls.Add(this.btnDOFRInitiate);
            this.Controls.Add(this.lblDOFR);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DOFRInitiateView";
            this.Size = new System.Drawing.Size(191, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Constants

        private Label lblDOFR;
        private Button btnDOFRInitiate;

        #endregion

        #region Properties

        public DOFRInitiatePresenter DOFRInitiatePresenter { get; set; }
        private readonly Container components = null;
        public bool EnableInitiateButton {
            set {
                this.btnDOFRInitiate.Enabled = value;
            } 
        }
        public bool ShowMe
        {
            set{
                this.Visible = value;
            }
        }

        #endregion


        #region Private Methods

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(DOFRInitiatePresenter != null)
                DOFRInitiatePresenter.UnRegisterRulesEvents();
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion
        private void btnDOFRInitiate_Click(object sender, EventArgs e)
        {
            DOFRInitiatePresenter.HandleDOFRInitiateClick();
        }
        #region Public Methods
      
        #endregion
    }

}
