using Extensions.UI.Winforms;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;

namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    public partial class COBReceivedAndIMFMReceivedView : ControlView , ICOBReceivedAndIMFMReceivedView
    {
        #region Constructor
        public COBReceivedAndIMFMReceivedView()
        {
            InitializeComponent();
            COBUnSelected();
            IMFMUnSelected();
        }

        #endregion

        #region public methods
      
        public void ShowCOBReceived()
        {
            lblCOBReceived.Visible = true;
            rbCOBYes.Visible = true;
            rbCOBNo.Visible = true;
        }

        public void HideCOBReceived()
        {
            lblCOBReceived.Visible = false;
            rbCOBYes.Visible = false;
            rbCOBNo.Visible = false;
        }

        public void ShowIMFMReceived()
        {
            lblIMFMReceived.Visible = true;
            rbIMFMYes.Visible = true;
            rbIMFMNo.Visible = true;
        }

        public void HideIMFMReceived()
        {
            lblIMFMReceived.Visible = false;
            rbIMFMYes.Visible = false;
            rbIMFMNo.Visible = false;
        }

        public void SetCOBReceivedAsYes()
        {
            rbCOBYes.Checked = true;
        }

        public void SetCOBReceivedAsNo()
        {
            rbCOBNo.Checked = true;
        }

        public void SetIMFMReceivedAsYes()
        {
            rbIMFMYes.Checked = true;
        }

        public void SetIMFMReceivedAsNo()
        {
            rbIMFMNo.Checked = true;
        }

        public void COBUnSelected()
        {
            rbCOBYes.Checked = false;
            rbCOBNo.Checked = false;
        }

        public void IMFMUnSelected()
        {
            rbIMFMYes.Checked = false;
            rbIMFMNo.Checked = false;
        }
        
        #endregion

        #region Proterties

        public ICOBReceivedAndIMFMReceivedPresenter Presenter { get; set; }
      
        #endregion

        #region Private Methods

        private void rbIMFMYes_Click(object sender, System.EventArgs e)
        {
            Presenter.SetIMFMReceivedAsYes();
        }

        private void rbIMFMNo_Click(object sender, System.EventArgs e)
        {
            Presenter.SetIMFMReceivedAsNo();
        }

        private void rbCOBYes_Click(object sender, System.EventArgs e)
        {
            Presenter.SetCOBReceivedAsYes();
        }

        private void rbCOBNo_Click(object sender, System.EventArgs e)
        {
            Presenter.SetCOBReceivedAsNo();
        }

        #endregion
    }
}
