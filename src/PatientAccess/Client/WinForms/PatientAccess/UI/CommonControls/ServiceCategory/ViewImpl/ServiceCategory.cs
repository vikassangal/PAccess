using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls.ServiceCategory.Presenter;
using PatientAccess.UI.CommonControls.ServiceCategory.View;

namespace PatientAccess.UI.CommonControls.ServiceCategory.ViewImpl
{
    /// <summary>
    /// Summary description for ClinicView.
    /// </summary>
    /// 
    //TODO: Create XML summary comment for ClinicView
    [Serializable]
    public class ServiceCategory : ControlView, IServiceCategoryView
    {
        #region Events

        public event EventHandler ServiceCategorySelected;
        public event CancelEventHandler ServiceCategoryValidating;

        #endregion

        #region  Properties

        public ServiceCategoryPresenter ServiceCategoryPresenter { get; set; }

        public bool EnableServiceCategory
        {
            set { this.cmbServiceCategory.Enabled = value; }
        }
        public bool ShowMe
        {
            set { this.Visible = value; }
        }
        public Account Model_Account
        {
            get { return (Account)Model; }
            set { Model = value; }
        }
        public string SelectedServiceCategory
        {
            get 
            { 
                return this.cmbServiceCategory.SelectedItem.ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                   IEnumerable<ClinicServiceCategory> items = this.cmbServiceCategory.Items.Cast<ClinicServiceCategory>();
                    if (items != null)
                    {
                        ClinicServiceCategory selectedServiceCategory = items.Where(s => s.Code == value).FirstOrDefault();
                        if (selectedServiceCategory != null)
                        {
                            this.cmbServiceCategory.SelectedItem = selectedServiceCategory;
                            this.Model_Account.ServiceCategory = selectedServiceCategory;
                        }
                    }
                }
            }
        }

        #endregion

        #region Event Handlers  

        public void cmbServiceCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedServiceCategory != null)
            {
                this.Model_Account.ServiceCategory = (ClinicServiceCategory)this.cmbServiceCategory.SelectedItem;
                this.ServiceCategorySelected(this, new LooseArgs(this.cmbServiceCategory.SelectedItem as ClinicServiceCategory));
            }
            ServiceCategoryPresenter.RunRules();
        }

        public void cmbServiceCategory_Validating(object sender, CancelEventArgs e)
        {
            this.ServiceCategoryValidating(this, null);
            ServiceCategoryPresenter.RunRules();
        }

        #endregion

        #region Public Methods

        public void LoadServiceCategory(ArrayList serviceCategories)
        {
            this.cmbServiceCategory.Items.Clear();
            ClinicServiceCategory clinicServiceCategory = new ClinicServiceCategory();
            for (int i = 0; i < serviceCategories.Count; i++)
            {
                clinicServiceCategory = (ClinicServiceCategory) serviceCategories[i];
                cmbServiceCategory.Items.Add(clinicServiceCategory);
            }
        }
        public void ClearServiceCategory()
        {
            this.cmbServiceCategory.Items.Clear();
        }

        public void SetNormalBgColor()
        {
            UIColors.SetNormalBgColor(cmbServiceCategory);
            Refresh();
        }

        public void MakeRequiredBgColor()
        {
            if (cmbServiceCategory.Enabled)
            {
                UIColors.SetRequiredBgColor(cmbServiceCategory);
                Refresh();
            }
        }
        #endregion

        #region Private Methods

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbServiceCategory = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblAlternateCareFacility = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbServiceCategory
            // 
            this.cmbServiceCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbServiceCategory.Location = new System.Drawing.Point(48, 2);
            this.cmbServiceCategory.Name = "cmbServiceCategory";
            this.cmbServiceCategory.Size = new System.Drawing.Size(193, 21);
            this.cmbServiceCategory.TabIndex = 0;
            this.cmbServiceCategory.SelectedIndexChanged += new System.EventHandler(this.cmbServiceCategory_SelectedIndexChanged);
            this.cmbServiceCategory.Validating += new System.ComponentModel.CancelEventHandler(this.cmbServiceCategory_Validating);
            // 
            // lblAlternateCareFacility
            // 
            this.lblAlternateCareFacility.BackColor = System.Drawing.Color.Transparent;
            this.lblAlternateCareFacility.Location = new System.Drawing.Point(0, 1);
            this.lblAlternateCareFacility.Name = "lblAlternateCareFacility";
            this.lblAlternateCareFacility.Size = new System.Drawing.Size(49, 26);
            this.lblAlternateCareFacility.TabIndex = 24;
            this.lblAlternateCareFacility.Text = "Service Category";
            // 
            // ServiceCategory
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cmbServiceCategory);
            this.Controls.Add(this.lblAlternateCareFacility);
            this.Name = "ServiceCategory";
            this.Size = new System.Drawing.Size(242, 26);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization

        public ServiceCategory()
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (ServiceCategoryPresenter != null)
                 ServiceCategoryPresenter.UnRegisterRulesEvents();
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

        #region Data Elements

        private Container components = null;
        private Label lblAlternateCareFacility;
        private PatientAccessComboBox cmbServiceCategory;

        #endregion


        #region Constants
        #endregion
    }
}
