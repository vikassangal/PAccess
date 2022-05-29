using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;

namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    public partial class HIEShareDataFlagView : ControlView, IHIEShareDataFlagView
    {
        #region private members
        private bool loadingHIEData;
        private IHIEShareDataPresenter hIEConsentPresenter;
        #endregion

        #region Constructor

        public HIEShareDataFlagView()
        {
            InitializeComponent();
            loadingHIEData = true;
        }

        #endregion
       
        #region public methods
        public bool ShowHIEMessage
        {
            get
            {
                return !loadingHIEData;
            }
        }
        public override void UpdateView()
        {

            try
            {
                HIEShareDataPresenter.UpdateView();
            }
            finally
            {
                Cursor = Cursors.Default;
                loadingHIEData = false;
            }
        }
        
        public void AutoPopulateShareDataWithPublicHIEForRightToRestrict(bool rightToRestrictFlag)
        {
            HIEShareDataPresenter.AutoPopulateShareDataWithPublicHIEForRightToRestrict(rightToRestrictFlag);
        }

        public void ShowShareDataWithPublicHIE()
        {
            cmbShareDataWithPublicHIEFlag.Visible = true;
            lblShareDataWithPublicHieFlag.Visible = true;
        }

        public void HideShareDataWithPublicHIE()
        {
            cmbShareDataWithPublicHIEFlag.Visible = false;
            lblShareDataWithPublicHieFlag.Visible = false;
        }

        public void HideShareDataWithPCP()
        {
            cmbShareDataWithPCPFlag.Visible = false;
            lblShareDataWithPCPFlag.Visible = false;
        }

        public void ShowShareDataWithPCP()
        {
            cmbShareDataWithPCPFlag.Visible = true;
            lblShareDataWithPCPFlag.Visible = true;
        }

        public void PopulateShareDataWithPCP(IEnumerable<YesNoFlag> hieConsentFlags)
        {
            cmbShareDataWithPCPFlag.Items.Clear();
            foreach (var consent in hieConsentFlags)
            {
                cmbShareDataWithPCPFlag.Items.Add(consent);
            }
        }

        public void PopulateShareDataWithPublicHIE(IEnumerable<YesNoFlag> hieConsentFlags)
        {
            cmbShareDataWithPublicHIEFlag.Items.Clear();
            foreach (var consent in hieConsentFlags)
            {
                cmbShareDataWithPublicHIEFlag.Items.Add(consent);
            }
         }

        public void EnableShareDataWithPublicHIE()
        {
            cmbShareDataWithPublicHIEFlag.Enabled = true;
            lblShareDataWithPublicHieFlag.Enabled = true;
        }

        public bool IsShareDataPublicHIEEnabled
        {
            get { return cmbShareDataWithPublicHIEFlag.Visible && cmbShareDataWithPublicHIEFlag.Enabled; }
        }
       
        public void DisableShareDatawithPublicHIE()
        {
            cmbShareDataWithPublicHIEFlag.Enabled = false;
            lblShareDataWithPublicHieFlag.Enabled = false;
        }

        public void SetShareDataWithPublicHIEAsRequired()
        {
           
            UIColors.SetRequiredBgColor(cmbShareDataWithPublicHIEFlag);
        }

        public void SetShareDataWithPublicHIEToNormalColor()
        {
            UIColors.SetNormalBgColor(cmbShareDataWithPublicHIEFlag);
        }

        public void SetNotifyPCPDataRequiredBgColor()
        {
            UIColors.SetRequiredBgColor(cmbShareDataWithPCPFlag);
        }
        public void SetNotifyPCPToNormalColor()
        {
            UIColors.SetNormalBgColor(cmbShareDataWithPCPFlag);
        }
        #endregion

        #region Event Handlers

        private void ShareDataWithPublicHIE_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateShareDataWithPublicHIE();
        }

        private void ShareDataWithPCP_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateShareDataWithPCP();
        }
        
        #endregion

        #region Private Methods
        
        private void UpdateShareDataWithPublicHIE()
        {
            if ( cmbShareDataWithPublicHIEFlag.SelectedItem != null )
            {
               HIEShareDataPresenter.UpdateShareDataWithPublicHIE();
            }
        }

        private void UpdateShareDataWithPCP()
        {
            if ( cmbShareDataWithPCPFlag.SelectedItem != null )
            {
                HIEShareDataPresenter.UpdateShareDataWithPCP();
            }
        }
        
        #endregion

        #region Properties

        public Account PatientAccount
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

        private IHIEShareDataPresenter HIEShareDataPresenter
        {
            get
            {
                return hIEConsentPresenter = HIEShareDataPresenterFactory.GetPresenter(this,
                    new HIEConsentFeatureManager(),
                    PatientAccount);
            }
        }
        
        public YesNoFlag ShareDataWithPublicHIE
        {
            get { return cmbShareDataWithPublicHIEFlag.SelectedItem as YesNoFlag; }
            set { cmbShareDataWithPublicHIEFlag.SelectedItem = value; }
        }

        public YesNoFlag ShareDataWithPCP
        {
            get { return cmbShareDataWithPCPFlag.SelectedItem as YesNoFlag; }
            set { cmbShareDataWithPCPFlag.SelectedItem = value; }
        }
        
        #endregion
    }
}
