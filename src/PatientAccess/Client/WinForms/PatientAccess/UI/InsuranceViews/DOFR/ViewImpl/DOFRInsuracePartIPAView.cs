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
    public class DOFRInsuracePartIPAView : ControlView, IDOFRInsuracePartIPAView
    {

        #region Construction and Finalization
        public DOFRInsuracePartIPAView()
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
            this.rbNo = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.rbYes = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.SuspendLayout();
            // 
            // lblDOFR
            // 
            this.lblDOFR.AutoSize = true;
            this.lblDOFR.Location = new System.Drawing.Point(-1, 5);
            this.lblDOFR.Margin = new System.Windows.Forms.Padding(0);
            this.lblDOFR.Name = "lblDOFR";
            this.lblDOFR.Size = new System.Drawing.Size(324, 20);
            this.lblDOFR.TabIndex = 1;
            this.lblDOFR.Text = "Insurance Plan part of an IPA/Medical Group";
            // 
            // rbNo
            // 
            this.rbNo.AutoCheck = false;
            this.rbNo.Location = new System.Drawing.Point(59, 21);
            this.rbNo.Name = "rbNo";
            this.rbNo.Size = new System.Drawing.Size(44, 15);
            this.rbNo.TabIndex = 4;
            this.rbNo.Text = "No";
            this.rbNo.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            this.rbNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbNo_KeyDown);
            this.rbNo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbNo_MouseDown);
            // 
            // rbYes
            // 
            this.rbYes.AutoCheck = false;
            this.rbYes.Location = new System.Drawing.Point(12, 21);
            this.rbYes.Name = "rbYes";
            this.rbYes.Size = new System.Drawing.Size(44, 15);
            this.rbYes.TabIndex = 3;
            this.rbYes.Text = "Yes";
            this.rbYes.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            this.rbYes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbYes_KeyDown);
            this.rbYes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbYes_MouseDown);
            // 
            // DOFRInsuracePartIPAView
            // 
            this.Controls.Add(this.rbNo);
            this.Controls.Add(this.rbYes);
            this.Controls.Add(this.lblDOFR);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DOFRInsuracePartIPAView";
            this.Size = new System.Drawing.Size(217, 43);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Constants

        private Label lblDOFR;
        private CommonControls.RadioButtonKeyHandler rbNo;
        private CommonControls.RadioButtonKeyHandler rbYes;

        #endregion

        #region Properties

        public DOFRInsuracePartIPAViewPresenter DOFRInsuracePartIPAViewPresenter { get; set; }
        private readonly Container components = null;
      
        public bool ShowMe
        {
            set{
                this.Visible = value;
            }
        }
        public bool rbYesChecked { get { return rbYes.Checked; } set { rbYes.Checked = value; } }
        public bool rbNoChecked { get { return rbNo.Checked; } set { rbNo.Checked = value; } }
        public bool rbYesEnabled { get { return rbYes.Enabled; } set { rbYes.Enabled = value; } }
        public bool rbNoEnabled { get { return rbNo.Enabled; } set { rbNo.Enabled = value; } }

        #endregion

        #region Event Handlers
        public event EventHandler RadioChanged;
       
        /// <summary>
        /// Fire the changed event for the radio button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RadioChanged != null)
            {
                this.RadioChanged(sender, e);
                DOFRInsuracePartIPAViewPresenter.RunRules();
            }
        }
        private void rbYes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbNo.Checked = true;
                rbYes.Checked = false;
                rbNo.Focus();
            }
        }

        private void rbNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbNo.Checked = false;
                rbYes.Checked = true;
                rbYes.Focus();
            }
        }

        private void rbYes_MouseDown(object sender, MouseEventArgs e)
        {
            rbYes.Checked = true;
            rbNo.Checked = false;
        }

        private void rbNo_MouseDown(object sender, MouseEventArgs e)
        {
            rbYes.Checked = false;
            rbNo.Checked = true;
        }

        #endregion
        #region Private Methods

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(DOFRInsuracePartIPAViewPresenter != null)
                DOFRInsuracePartIPAViewPresenter.UnRegisterRulesEvents();
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

        #region Public Methods
        public void SetNormalBgColor()
        {
            UIColors.SetNormalBgColor(rbYes);
            UIColors.SetNormalBgColor(rbNo);
            Refresh();
        }

        public void MakeRequiredBgColor()
        {
            if (rbYes.Enabled && rbNo.Enabled)
            {
                UIColors.SetRequiredBgColor(rbYes);
                UIColors.SetRequiredBgColor(rbNo);
                Refresh();
            }
        }
        #endregion


    }

}
