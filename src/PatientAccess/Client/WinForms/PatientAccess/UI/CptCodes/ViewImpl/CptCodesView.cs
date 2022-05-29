using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.CptCodes.Presenters;
using PatientAccess.UI.CptCodes.Views;

namespace PatientAccess.UI.CptCodes.ViewImpl
{
    public partial class CptCodesView : ControlView, ICptCodesView
    {
        
        #region Event Handlers
      
        private void rbYes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbNo.Checked = true;
                rbYes.Checked = false;
                DisableViewButton();
                CptCodesPresenter.ClearCptCodes();
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

                var cptCodesDetailsView = new CptCodesDetailsView();
                
                CptCodesPresenter.ShowCptCodesDetailsDialog(cptCodesDetailsView);
                
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            var cptCodesDetailsView = new CptCodesDetailsView();

            CptCodesPresenter.ShowCptCodesDetailsDialog(cptCodesDetailsView);
        }

        private void rbYes_MouseDown(object sender, MouseEventArgs e)
        {
            rbYes.Checked = true;
            rbNo.Checked = false;
            
            var cptCodesDetailsView = new CptCodesDetailsView();
            
            CptCodesPresenter.ShowCptCodesDetailsDialog(cptCodesDetailsView);
        }

        private void rbNo_MouseDown(object sender, MouseEventArgs e)
        {
            rbYes.Checked = false;
            rbNo.Checked = true;
            CptCodesPresenter.ClearCptCodes();
        }

        #endregion

        # region Properties

        public CptCodesPresenter CptCodesPresenter { private get; set; }

        #endregion
        
        #region Data Elements
        #endregion

        #region Public Methods

        public void CheckYes()
        {
            rbYes.Checked = true;
        }

        public void CheckNo()
        {
            rbNo.Checked = true;
        }

        public void UnSelected()
        {
            rbYes.Checked = false;
            rbNo.Checked = false;
        }

        public void DisableViewButton()
        {
            btnView.Enabled = false;
        }

        public void EnableViewButton()
        {
            btnView.Enabled = true;
        }

        public void ShowMe()
        {
            Visible = true;
        }

        public void HideMe()
        {
            Visible = false;
        }

        public CptCodesView()
        {
            InitializeComponent();
            UnSelected();
            DisableViewButton();
        }

        #endregion

        #region Private Methods
        #endregion
    }
}
