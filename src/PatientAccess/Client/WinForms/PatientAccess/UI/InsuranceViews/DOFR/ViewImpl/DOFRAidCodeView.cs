using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.InsuranceViews.DOFR.Presenter;
using PatientAccess.UI.InsuranceViews.DOFR.View;
using System.Collections.Generic;
using System.Linq;

namespace PatientAccess.UI.InsuranceViews.DOFR.ViewImpl
{
    /// <summary>
    /// Summary description for GenderView.
    /// </summary>
    public class DOFRAidCodeView : ControlView, IDOFRAidCodeView
    {

        #region Construction and Finalization

        public DOFRAidCodeView()
        {
            // This call is required by the Windows.Forms Form Designer.
            
            InitializeComponent();
        }
       
        #endregion

        #region Properties

        public DOFRAidCodePresenter DOFRAidCodePresenter { get; set; }
        public bool ShowMe
        {
            set{
                this.Visible = value;
            }
        }
        public bool rbExpansionChecked { get { return rbExpansion.Checked; } set { rbExpansion.Checked = value; } }
        public bool rbNonExpansionChecked { get { return rbNonExpansion.Checked; } set { rbNonExpansion.Checked = value; } }
        public AidCode SelectedAidCode { get  { return this.cmbAidCode.SelectedItem as AidCode; }}
        public string SetSelectedAidCode
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    IEnumerable<AidCode> items = this.cmbAidCode.Items.Cast<AidCode>();
                    if (items != null)
                    {
                        AidCode selectedAidCode = items.Where(s => s.Description == value).FirstOrDefault();
                        if (selectedAidCode != null)
                            this.cmbAidCode.SelectedItem = selectedAidCode;
                    }
                }
            }
        }
       
        #endregion

        #region Event Handlers

        public event EventHandler RadioChanged;
        public event EventHandler AidCodeSelectedIndexChanged;

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
                DOFRAidCodePresenter.RunRules();
            }
        }

        public void cmbAidCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.AidCodeSelectedIndexChanged != null)
            {
                this.AidCodeSelectedIndexChanged(this, new LooseArgs(this.cmbAidCode.SelectedItem as AidCode));
            }
            if (SelectedAidCode != null)
            {
                
                EnableDisableRadioButtons(true);
                if (SelectedAidCode.Code == DOFRAPIRequest.Expansion)
                {
                    rbExpansion.Checked = true;
                    rbNonExpansion.Checked = false;
                }
                else if (SelectedAidCode.Code == DOFRAPIRequest.NonExpansion)
                {
                    rbNonExpansion.Checked = true;
                    rbExpansion.Checked = false;
                }
            }
            else
                EnableDisableRadioButtons(false);
            DOFRAidCodePresenter.RunRules();
        }

        public void cmbAidCode_Validating(object sender, CancelEventArgs e)
        {
            DOFRAidCodePresenter.RunRules();
        }
        private void EnableDisableRadioButtons(bool value)
        {
            rbExpansion.Enabled = value;
            rbNonExpansion.Enabled = value;
        }
        private void rbExpansion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbNonExpansion.Checked = true;
                rbExpansion.Checked = false;
                rbNonExpansion.Focus();
            }
        }

        private void rbNonExpansion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbNonExpansion.Checked = false;
                rbExpansion.Checked = true;
                rbExpansion.Focus();
            }
        }

        private void rbExpansion_MouseDown(object sender, MouseEventArgs e)
        {
            rbExpansion.Checked = true;
            rbNonExpansion.Checked = false;
        }

        private void rbNonExpansion_MouseDown(object sender, MouseEventArgs e)
        {
            rbExpansion.Checked = false;
            rbNonExpansion.Checked = true;
        }

        #endregion

        #region Public Methods
        public void LoadAidCodes(ArrayList aidCodes)
        {
            this.cmbAidCode.Items.Clear();
            AidCode aidCode = new AidCode();
            for (int i = 0; i < aidCodes.Count; i++)
            {
                aidCode = (AidCode)aidCodes[i];
                cmbAidCode.Items.Add(aidCode);
            }
        }
        public void ClearAidCodes()
        {
            this.cmbAidCode.SelectedIndex = -1;
            rbNonExpansion.Checked = false;
            rbExpansion.Checked = false;
            EnableDisableRadioButtons(false);
        }

        public void SetNormalBgColor()
        {
            UIColors.SetNormalBgColor(cmbAidCode);
            Refresh();
        }

        public void MakeRequiredBgColor()
        {
            if (cmbAidCode.Enabled)
            {
                UIColors.SetRequiredBgColor(cmbAidCode);
                Refresh();
            }
        }
        #endregion

        #region Private Methods
        
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(DOFRAidCodePresenter != null)
                DOFRAidCodePresenter.UnRegisterRulesEvents();
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

        #region Constants

        private CommonControls.RadioButtonKeyHandler rbNonExpansion;
        private CommonControls.RadioButtonKeyHandler rbExpansion;
        private Label lblDOFR;
        private CommonControls.PatientAccessComboBox cmbAidCode;
        private readonly Container components = null;

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rbNonExpansion = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.rbExpansion = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.lblDOFR = new System.Windows.Forms.Label();
            this.cmbAidCode = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.SuspendLayout();
            // 
            // rbNonExpansion
            // 
            this.rbNonExpansion.AutoCheck = false;
            this.rbNonExpansion.Enabled = false;
            this.rbNonExpansion.Location = new System.Drawing.Point(173, 4);
            this.rbNonExpansion.Name = "rbNonExpansion";
            this.rbNonExpansion.Size = new System.Drawing.Size(97, 18);
            this.rbNonExpansion.TabIndex = 4;
            this.rbNonExpansion.Text = "Non-Expansion";
            this.rbNonExpansion.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            this.rbNonExpansion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbNonExpansion_KeyDown);
            this.rbNonExpansion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbNonExpansion_MouseDown);
            // 
            // rbExpansion
            // 
            this.rbExpansion.AutoCheck = false;
            this.rbExpansion.Enabled = false;
            this.rbExpansion.Location = new System.Drawing.Point(100, 3);
            this.rbExpansion.Name = "rbExpansion";
            this.rbExpansion.Size = new System.Drawing.Size(74, 20);
            this.rbExpansion.TabIndex = 3;
            this.rbExpansion.Text = "Expansion";
            this.rbExpansion.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            this.rbExpansion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbExpansion_KeyDown);
            this.rbExpansion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbExpansion_MouseDown);
            // 
            // lblDOFR
            // 
            this.lblDOFR.AutoSize = true;
            this.lblDOFR.Location = new System.Drawing.Point(1, 6);
            this.lblDOFR.Margin = new System.Windows.Forms.Padding(0);
            this.lblDOFR.Name = "lblDOFR";
            this.lblDOFR.Size = new System.Drawing.Size(53, 13);
            this.lblDOFR.TabIndex = 6;
            this.lblDOFR.Text = "Aid Code:";
            // 
            // cmbAidCode
            // 
            this.cmbAidCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAidCode.Location = new System.Drawing.Point(53, 2);
            this.cmbAidCode.Name = "cmbAidCode";
            this.cmbAidCode.Size = new System.Drawing.Size(39, 21);
            this.cmbAidCode.TabIndex = 7;
            this.cmbAidCode.SelectedIndexChanged += new System.EventHandler(this.cmbAidCode_SelectedIndexChanged);
            this.cmbAidCode.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAidCode_Validating);
            // 
            // DOFRAidCodeView
            // 
            this.Controls.Add(this.cmbAidCode);
            this.Controls.Add(this.lblDOFR);
            this.Controls.Add(this.rbNonExpansion);
            this.Controls.Add(this.rbExpansion);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DOFRAidCodeView";
            this.Size = new System.Drawing.Size(275, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }

}
