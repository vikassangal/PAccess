using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.HistoricalAccountViews
{
	/// <summary>
	/// Summary description for HistoricalPatientAccountDetails.
	/// </summary>
    public class HistoricalPatientAccountDetails : TimeOutFormView
    {
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.LoadPatientAccountDetails();

            if( this.AccountDetails != null )
            {
                if( this.AccountDetails.Patient != null )
                {
                    this.historicalPatientDemographicsSummaryView.Model = this.AccountDetails;
                    this.historicalPatientDemographicsSummaryView.UpdateView();
                }
                this.historicalPatientAccountSummaryView.Model = this.AccountDetails;
                this.historicalPatientAccountSummaryView.UpdateView();
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private void LoadPatientAccountDetails()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                IAccountBroker broker = 
                    BrokerFactory.BrokerOfType<IAccountBroker>();
                this.AccountDetails = broker.MPIAccountDetailsFor( this.Model as AccountProxy );
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Private Properties

        private AccountProxy AccountDetails
        {
            get
            {
                return i_AccountProxy;
            }
            set 
            {
                i_AccountProxy = value;
            }
        }
        #endregion

        #region Construction and Finalization

        public HistoricalPatientAccountDetails()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            base.EnableThemesOn( this );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Windows Form Designer generated code
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HistoricalPatientAccountDetails));
            this.demographicSummaryPanel = new System.Windows.Forms.Panel();
            this.historicalPatientDemographicsSummaryView = new PatientAccess.UI.HistoricalAccountViews.HistoricalPatientDemographicsSummaryView();
            this.accountSummaryPanel = new System.Windows.Forms.Panel();
            this.historicalPatientAccountSummaryView = new PatientAccess.UI.HistoricalAccountViews.HistoricalPatientAccountSummaryView();
            this.closeButton = new LoggingButton();
            this.demographicSummaryPanel.SuspendLayout();
            this.accountSummaryPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // demographicSummaryPanel
            // 
            this.demographicSummaryPanel.Controls.Add(this.historicalPatientDemographicsSummaryView);
            this.demographicSummaryPanel.Location = new System.Drawing.Point(0, 0);
            this.demographicSummaryPanel.Name = "demographicSummaryPanel";
            this.demographicSummaryPanel.Size = new System.Drawing.Size(880, 284);
            this.demographicSummaryPanel.TabIndex = 0;
            // 
            // historicalPatientDemographicsSummaryView
            // 
            this.historicalPatientDemographicsSummaryView.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.historicalPatientDemographicsSummaryView.Location = new System.Drawing.Point(0, 7);
            this.historicalPatientDemographicsSummaryView.Model = null;
            this.historicalPatientDemographicsSummaryView.Name = "historicalPatientDemographicsSummaryView";
            this.historicalPatientDemographicsSummaryView.Size = new System.Drawing.Size(880, 277);
            this.historicalPatientDemographicsSummaryView.TabIndex = 0;
            // 
            // accountSummaryPanel
            // 
            this.accountSummaryPanel.Controls.Add(this.historicalPatientAccountSummaryView);
            this.accountSummaryPanel.Location = new System.Drawing.Point(0, 284);
            this.accountSummaryPanel.Name = "accountSummaryPanel";
            this.accountSummaryPanel.Size = new System.Drawing.Size(880, 312);
            this.accountSummaryPanel.TabIndex = 1;
            // 
            // historicalPatientAccountSummaryView
            // 
            this.historicalPatientAccountSummaryView.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.historicalPatientAccountSummaryView.Location = new System.Drawing.Point(0, 0);
            this.historicalPatientAccountSummaryView.Model = null;
            this.historicalPatientAccountSummaryView.Name = "historicalPatientAccountSummaryView";
            this.historicalPatientAccountSummaryView.Size = new System.Drawing.Size(880, 319);
            this.historicalPatientAccountSummaryView.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(797, 603);
            this.closeButton.Name = "closeButton";
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "&Close";
            // 
            // HistoricalPatientAccountDetails
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(880, 633);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.accountSummaryPanel);
            this.Controls.Add(this.demographicSummaryPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistoricalPatientAccountDetails";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "View Account - Details";
            this.demographicSummaryPanel.ResumeLayout(false);
            this.accountSummaryPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Data Elements
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private Panel demographicSummaryPanel;
        private Panel accountSummaryPanel;
        private HistoricalPatientDemographicsSummaryView historicalPatientDemographicsSummaryView;
        private LoggingButton closeButton;
        private HistoricalPatientAccountSummaryView historicalPatientAccountSummaryView;
        private AccountProxy i_AccountProxy;

        #endregion

		#region Constants
        #endregion

	}
}
