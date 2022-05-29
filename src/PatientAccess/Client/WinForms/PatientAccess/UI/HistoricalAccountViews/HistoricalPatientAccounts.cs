using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Reports.FaceSheet;

namespace PatientAccess.UI.HistoricalAccountViews
{
	/// <summary>
	/// Summary description for HistoricalPatientAccounts.
	/// </summary>
	public class HistoricalPatientAccounts : TimeOutFormView
	{
        #region Events
        #endregion

        #region Event Handlers
        
        private void backToSearchButton_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }
        
        private void detailsButton_Click(object sender, EventArgs e)
        {
            if( IsPBARAvailable() )
            {
                historicalPatientAccountsView.GetSelectedAccount();
            }
            else
            {
                MessageBox.Show( UIErrorMessages.PBAR_UNAVAILABLE_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1 );
            }
        }

        private void historicalPatientAccountsView_AccountSelected(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                AccountProxy accountProxy = ((LooseArgs)e).Context as AccountProxy;

                if( accountProxy != null )
                {
                    if ( accountProxy.IsPurged.Code.Equals( "Y" ) )
                    {
                        HistoricalPatientAccountDetails historicalPatientAccountDetails
                            = new HistoricalPatientAccountDetails();
                        try
                        {
                            historicalPatientAccountDetails.Model = accountProxy;
                            historicalPatientAccountDetails.UpdateView();
                            historicalPatientAccountDetails.ShowDialog( this );
                        }
                        finally
                        {
                            historicalPatientAccountDetails.Dispose();
                        }
                    }
                    else
                    {
                        LoadNonPurgedAccountDetails(accountProxy);
                        if ( AccountDetails.IsShortRegisteredNonDayCareAccount() )
                        {
                            var nonPurgedAccountView = new NonPurgedShortAccountView();
                            try
                            {
                                nonPurgedAccountView.Model = AccountDetails;
                                nonPurgedAccountView.UpdateView();
                                nonPurgedAccountView.ShowDialog(this);
                            }
                            finally
                            {
                                nonPurgedAccountView.Dispose();
                            }
                        }
                        else
                        {
                            var nonPurgedAccountView = new NonPurgedAccountView();
                            try
                            {
                                nonPurgedAccountView.Model = AccountDetails;
                                nonPurgedAccountView.UpdateView();
                                nonPurgedAccountView.ShowDialog( this );
                            }
                            finally
                            {
                                nonPurgedAccountView.Dispose();
                            }
                        }
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

	    private void LoadNonPurgedAccountDetails(AccountProxy accountProxy)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();
                AccountDetails = broker.AccountDetailsFor( accountProxy );
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void btnPrintFaceSheet_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                IAccount account = historicalPatientAccountsView.SelectedAccount;
                bool shouldPrint = true;
                if( account != null )
                {
                    if( IsPBARAvailable( ) )
                    {
                        if( ( (AccountProxy)account ).IsPurged.Code.Equals( "Y" ) )
                        {
                            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();
                            account = broker.MPIAccountDetailsFor( account as AccountProxy );
                        }
                        else
                        {
                            LoadNonPurgedAccountDetails(account as AccountProxy);
                            account = AccountDetails;
                        }
                    }
                    else
                    {
                        shouldPrint = false;

                        MessageBox.Show(UIErrorMessages.PBAR_UNAVAILABLE_MSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1);
                    }

                    if( shouldPrint )
                    {
                        var faceSheetPrintService = new PrintService( account );
                        faceSheetPrintService.Print( );
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Methods

        public override void UpdateView()
        {
            patientContextView.Model = Model as AbstractPatient;
            patientContextView.UpdateView();
            
            LoadPatientAccounts();
            historicalPatientAccountsView.Model = PatientAccounts;
            historicalPatientAccountsView.UpdateView();
            historicalPatientAccountsView.FocusOnGrid();
        }

        #endregion

        #region Properties 
        #endregion

        #region Private Methods

        private static bool IsPBARAvailable()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            return facilityBroker.IsDatabaseAvailableFor( User.GetCurrent().Facility.ConnectionSpec.ServerIP );
        }

        private void LoadPatientAccounts()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();
                Patient aPatient = new Patient( Model as AbstractPatient);
                i_PatientAccounts = broker.MPIAccountsFor( aPatient );
                detailsButton.Enabled = i_PatientAccounts.Count > 0;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

	    #endregion

        #region Private Properties

        public Activity CurrentActivity
        {
            get
            {
                return i_CurrentActivity;
            }
            set
            {
                i_CurrentActivity = value;
            }
        }

            private ICollection PatientAccounts 
        {
            get 
            {
                return i_PatientAccounts;
            }
        }

	    private IAccount AccountDetails { get; set; }

	    #endregion

        #region Construction and Finalization
        
        public HistoricalPatientAccounts()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            EnableThemesOn( this );
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HistoricalPatientAccounts));
            this.detailsButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.cancelButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.backToSearchButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.historicalPatientAccountsPanel = new System.Windows.Forms.Panel();
            this.AccountInfoPanel = new System.Windows.Forms.Panel();
            this.historicalPatientAccountsView = new PatientAccess.UI.HistoricalAccountViews.HistoricalPatientAccountsView();
            this.patientInfoPanel = new System.Windows.Forms.Panel();
            this.patientContextView = new PatientAccess.UI.PatientContextView();
            this.btnPrintFaceSheet = new PatientAccess.UI.CommonControls.LoggingButton();
            this.historicalPatientAccountsPanel.SuspendLayout();
            this.AccountInfoPanel.SuspendLayout();
            this.patientInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // detailsButton
            // 
            this.detailsButton.Enabled = false;
            this.detailsButton.Location = new System.Drawing.Point(831, 538);
            this.detailsButton.Message = null;
            this.detailsButton.Name = "detailsButton";
            this.detailsButton.TabIndex = 2;
            this.detailsButton.Text = "&Details";
            this.detailsButton.Click += new System.EventHandler(this.detailsButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(911, 538);
            this.cancelButton.Message = null;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            // 
            // backToSearchButton
            // 
            this.backToSearchButton.Location = new System.Drawing.Point(8, 538);
            this.backToSearchButton.Message = null;
            this.backToSearchButton.Name = "backToSearchButton";
            this.backToSearchButton.Size = new System.Drawing.Size(100, 23);
            this.backToSearchButton.TabIndex = 1;
            this.backToSearchButton.Text = "< &Back to Search";
            this.backToSearchButton.Click += new System.EventHandler(this.backToSearchButton_Click);
            // 
            // historicalPatientAccountsPanel
            // 
            this.historicalPatientAccountsPanel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.historicalPatientAccountsPanel.Controls.Add(this.AccountInfoPanel);
            this.historicalPatientAccountsPanel.Controls.Add(this.patientInfoPanel);
            this.historicalPatientAccountsPanel.Location = new System.Drawing.Point(7, 7);
            this.historicalPatientAccountsPanel.Name = "historicalPatientAccountsPanel";
            this.historicalPatientAccountsPanel.Size = new System.Drawing.Size(979, 524);
            this.historicalPatientAccountsPanel.TabIndex = 0;
            // 
            // AccountInfoPanel
            // 
            this.AccountInfoPanel.BackColor = System.Drawing.Color.White;
            this.AccountInfoPanel.Controls.Add(this.historicalPatientAccountsView);
            this.AccountInfoPanel.Location = new System.Drawing.Point(0, 60);
            this.AccountInfoPanel.Name = "AccountInfoPanel";
            this.AccountInfoPanel.Size = new System.Drawing.Size(979, 464);
            this.AccountInfoPanel.TabIndex = 0;
            // 
            // historicalPatientAccountsView
            // 
            this.historicalPatientAccountsView.Location = new System.Drawing.Point(0, 0);
            this.historicalPatientAccountsView.Model = null;
            this.historicalPatientAccountsView.Name = "historicalPatientAccountsView";
            this.historicalPatientAccountsView.Size = new System.Drawing.Size(979, 457);
            this.historicalPatientAccountsView.TabIndex = 0;
            this.historicalPatientAccountsView.AccountSelected += new System.EventHandler(this.historicalPatientAccountsView_AccountSelected);
            // 
            // patientInfoPanel
            // 
            this.patientInfoPanel.BackColor = System.Drawing.Color.White;
            this.patientInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.patientInfoPanel.Controls.Add(this.patientContextView);
            this.patientInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.patientInfoPanel.Name = "patientInfoPanel";
            this.patientInfoPanel.Size = new System.Drawing.Size(979, 51);
            this.patientInfoPanel.TabIndex = 0;
            // 
            // patientContextView
            // 
            this.patientContextView.Account = null;
            this.patientContextView.BackColor = System.Drawing.Color.White;
            this.patientContextView.DateOfBirthText = "";
            this.patientContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView.GenderLabelText = "";
            this.patientContextView.Location = new System.Drawing.Point(0, 0);
            this.patientContextView.Model = null;
            this.patientContextView.Name = "patientContextView";
            this.patientContextView.PatientNameText = "";
            this.patientContextView.Size = new System.Drawing.Size(977, 49);
            this.patientContextView.SocialSecurityNumber = "";
            this.patientContextView.TabIndex = 0;
            this.patientContextView.TabStop = false;
            // 
            // btnPrintFaceSheet
            // 
            this.btnPrintFaceSheet.Location = new System.Drawing.Point(721, 538);
            this.btnPrintFaceSheet.Message = null;
            this.btnPrintFaceSheet.Name = "btnPrintFaceSheet";
            this.btnPrintFaceSheet.Size = new System.Drawing.Size(103, 23);
            this.btnPrintFaceSheet.TabIndex = 4;
            this.btnPrintFaceSheet.Text = "Pr&int Face Sheet";
            this.btnPrintFaceSheet.Click += new System.EventHandler(this.btnPrintFaceSheet_Click);
            // 
            // HistoricalPatientAccounts
            // 
            this.AcceptButton = this.detailsButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(993, 568);
            this.Controls.Add(this.btnPrintFaceSheet);
            this.Controls.Add(this.historicalPatientAccountsPanel);
            this.Controls.Add(this.backToSearchButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.detailsButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistoricalPatientAccounts";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "View Account - Account History";
            this.historicalPatientAccountsPanel.ResumeLayout(false);
            this.AccountInfoPanel.ResumeLayout(false);
            this.patientInfoPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        
        #endregion

        #region Data Elements

        private LoggingButton cancelButton;
        private LoggingButton detailsButton;
        private LoggingButton backToSearchButton;
        private Panel historicalPatientAccountsPanel;
        private Panel patientInfoPanel;
        private Panel AccountInfoPanel;
        private PatientContextView patientContextView;
        private HistoricalPatientAccountsView historicalPatientAccountsView;
        private ICollection i_PatientAccounts;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private LoggingButton btnPrintFaceSheet;

        private Activity i_CurrentActivity;

        #endregion

        #region Constants
        #endregion

    }
}
