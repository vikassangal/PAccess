using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;

namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    public partial class PatientPortalOptInView : ControlView, IPatientPortalOptInView
    {
        public event EventHandler PortalOptedOutEvent;
        public event EventHandler SetEmailAddressAsNormalColorEvent;
        
        #region Event Handlers

        private void rbYes_Click(object sender, EventArgs e)
        {
            FirePortalOptedOutEvent(); 
            PatientPortalOptInPresenter.OptIn();
            PatientPortalOptInPresenter.ValidateEmailAddress();
        }
        
        private void rbYes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbNo.Focus();
            }
        }
        
        private void rbNo_Click( object sender, EventArgs e )
        {
            FirePortalOptedOutEvent();
            FireEmailAddressNormalColorEvent(); 
            PatientPortalOptInPresenter.OptOut();
        }

        private void rbUnable_Click(object sender, EventArgs e)
        {
            FirePortalOptedOutEvent();
            FireEmailAddressNormalColorEvent();
            PatientPortalOptInPresenter.Unable();
        }

        private void rbNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left)
            {
                rbYes.Focus();
            }
            else if (e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbUnable.Focus();
            }
           
        }

        private void rbUnable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbNo.Focus();
            }
        }
        
        private void FirePortalOptedOutEvent()
        {
            if (PortalOptedOutEvent != null)
            {
                PortalOptedOutEvent( this, null );
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
        
        public PatientPortalOptInPresenter PatientPortalOptInPresenter { get; set; }
       
        public void OptIn()
        {
            rbYes.Checked = true;
        }
        
        public void OptOut()
        {
            rbNo.Checked = true;
        }

        public void Unable()
        {
            rbUnable.Checked = true;
        }
       
        public void UnSelected()
        {
            rbYes.Checked = false;
            rbNo.Checked = false;
            rbUnable.Checked = false;
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
      
        public PatientPortalOptInView()
        {
            InitializeComponent();
            UnSelected();
        }
        
       
    }
}
