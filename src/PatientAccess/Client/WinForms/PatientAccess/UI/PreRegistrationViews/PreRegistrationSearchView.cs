using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.PatientSearch;
using log4net;

namespace PatientAccess.UI.PreRegistrationViews
{
    /// <summary>
    /// Summary description for PreRegistrationSearchView.
    /// </summary>
    public class PreRegistrationSearchView : ControlView
    {
        #region Event Handlers

        private void AccountSelectedEventHandler(object sender, EventArgs e)
        {
            Cursor storedCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                var accountProxy = ((LooseArgs)e).Context as AccountProxy;

                if (accountProxy != null)
                {
                    Account realAccount = accountProxy.AsAccount( new CancelPreRegActivity() );
                    ClearControls();

                    cancelPreRegistrationView = new CancelPreRegistrationView(this);
                    cancelPreRegistrationView.Model = realAccount;

                    SuspendLayout();
                    cancelPreRegistrationView.Dock = DockStyle.Fill;
                    ResumeLayout(false);
                    Controls.Add(cancelPreRegistrationView);
                }
            }

            finally
            {
                this.Cursor = storedCursor;
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Properties

        private Activity CurrentActivity
        {
            get
            {
                if (i_CurrentActivity == null)
                {
                    i_CurrentActivity = new PreRegistrationActivity();
                }
                return i_CurrentActivity;
            }
            set
            {
                i_CurrentActivity = value;
            }
        }
        #endregion

        #region Private Methods

        private void ClearControls()
        {
            foreach (Control control in this.Controls)
            {
                if (control != null)
                {
                    try
                    {
                        control.Dispose();
                    }
                    catch (Exception ex)
                    {
                        c_log.Error("Failed to dispose of a control; " + ex.Message, ex);
                    }
                }
            }
            Controls.Clear();
        }
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.masterPatientIndexView = new PatientAccess.UI.PatientSearch.MasterPatientIndexView();
            this.masterPatientIndexView.CurrentActivity = this.CurrentActivity;
            this.SuspendLayout();
            // 
            // masterPatientIndexView
            // 
            this.masterPatientIndexView.Location = new System.Drawing.Point(0, 0);
            this.masterPatientIndexView.Model = null;
            this.masterPatientIndexView.Name = "masterPatientIndexView";
            this.masterPatientIndexView.Size = new System.Drawing.Size(1024, 512);
            this.masterPatientIndexView.Dock = DockStyle.Fill;
            this.masterPatientIndexView.TabIndex = 0;
            // 
            // PreRegistrationView
            // 
            this.Controls.Add(this.masterPatientIndexView);
            this.Name = "CancelPreRegistrationView";
            this.Size = new System.Drawing.Size(1024, 512);
            this.ResumeLayout(false);
        }
        #endregion
        #endregion

        #region Construction and Finalization
        public PreRegistrationSearchView(Activity inActivity)
        {
            this.CurrentActivity = inActivity;

            InitializeComponent();
            SearchEventAggregator.GetInstance().AccountSelected += this.AccountSelectedEventHandler;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            SearchEventAggregator.GetInstance().AccountSelected -= this.AccountSelectedEventHandler;

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
        private static readonly ILog c_log = LogManager.GetLogger(typeof(PreRegistrationSearchView));

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private CancelPreRegistrationView cancelPreRegistrationView;
        private MasterPatientIndexView masterPatientIndexView;
        private Activity i_CurrentActivity;
        #endregion

        #region Constants
        #endregion
    }
}
