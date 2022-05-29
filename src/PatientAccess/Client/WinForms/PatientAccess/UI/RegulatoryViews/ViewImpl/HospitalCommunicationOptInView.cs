using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;


namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    public partial class HospitalCommunicationOptInView : ControlView, IHospitalCommunicationOptInView
    {
        public event EventHandler HospitalCommunicationOptInEvent;
        public event EventHandler SetEmailAddressAsNormalColorEvent;
        #region Event Handlers

        private void rbYes_Click(object sender, EventArgs e)
        {
            FireHospitalCommunicationOptInEvent();
            Presenter.OptIn();
            Presenter.ValidateEmailAddress();
        }
        private void rbYes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbNo.Focus();
            }
        }

        private void rbNo_Click(object sender, EventArgs e)
        {
            FireHospitalCommunicationOptInEvent();
            FireEmailAddressNormalColorEvent(); 
            Presenter.OptOut();
        }
        private void rbNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbYes.Focus();
            }
        }
        private void FireHospitalCommunicationOptInEvent()
        {
            if (HospitalCommunicationOptInEvent != null)
            {
                HospitalCommunicationOptInEvent(this, null);
            }
        }

        private void FireEmailAddressNormalColorEvent()
        {
            if (SetEmailAddressAsNormalColorEvent != null)
            {
                SetEmailAddressAsNormalColorEvent(this, null);
            }
        }
        #endregion
        public void OptIn()
        {
            rbYes.Checked = true;
        }

        public void OptOut()
        {
            rbNo.Checked = true;
        }

        public void UnSelected()
        {
            rbYes.Checked = false;
            rbNo.Checked = false;
        }

        public void ShowMe()
        {
            Visible = true;
        }

        public void HideMe()
        {
            Visible = false;
        }

        public void EnableMe()
        {
            Enabled = true;
        }
        public void DisableMe()
        {
            Enabled = false;
        }
         
        public HospitalCommunicationOptInPresenter Presenter { get; set; }
        public HospitalCommunicationOptInView()
        {
            InitializeComponent();
            UnSelected();
        }
    }
}
