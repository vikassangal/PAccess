using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// Summary description for SearchView.
    /// </summary>
    [Serializable]
    public sealed class SearchView : LoggingControlView
    {
        #region Events
        //public event EventHandler ReturnToMainScreen;
        #endregion

        #region Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchView_Load( object sender, EventArgs e )
        {
            patientSearchResultsView.Hide();
            Focus();

            if ( !IsInDesignMode && this.CurrentActivity != null )
            {
                lblActivity.Text = CurrentActivity.ContextDescription;
                string newbornActivityDesc = new AdmitNewbornActivity().ContextDescription;
                string preNewbornActivityDesc = new PreAdmitNewbornActivity().ContextDescription;
                if ( CurrentActivity.ContextDescription != newbornActivityDesc && CurrentActivity.ContextDescription != preNewbornActivityDesc )
                {
                    FindAPatientView.ShowNewbornSearchLabel();
                }
            }
            findAPatientView.CurrentActivity = CurrentActivity;
            findAPatientView.SetFocus();
        }

        private void findPatientTabs_SelectedIndexChanged( object sender, EventArgs e )
        {
            ResetResultsView( sender, e );

            if ( findPatientTabs.SelectedIndex == 0 )
            {
                findAPatientView.SetFocus();
            }
            else
            {
                findPatientByGuarantorView.SetFocus();
            }
        }

        public void ShowPanel()
        {
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
        }

        public void HidePanel()
        {
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();

            if ( patientSearchResultsView != null
                && patientSearchResultsView.PatientSearchResults != null
                && patientSearchResultsView.PatientSearchResults.Count > 0 )
            {
                this.viewAccountsButton.Focus();
            }
            else
            {
                this.patientSearchResultsView.FocusCreateButton();
            }
        }

        private void PatientsFound( object sender, EventArgs e )
        {
            this.doPatientsFound( sender, e );
        }

        private void doPatientsFound( object sender, EventArgs e )
        {
            switch ( sender.ToString() )
            {
                case "PatientAccess.UI.PatientSearch.FindAPatientView":
                    patientSearchResultsView.SearchByType = "FindAPatientView";
                    break;
                case "PatientAccess.UI.PatientSearch.FindPatientByGuarantorView":
                    patientSearchResultsView.SearchByType = "FindPatientByGuarantorView";
                    break;
            }

            List<PatientSearchResult> searchResults =
                ( (LooseArgs)e ).Context as List<PatientSearchResult>;

            patientSearchResultsView.Model = null;
            patientSearchResultsView.EMPIResults = HasEMPIResults;
            if (HasEMPIResults && searchResults != null )
            {
                patientSearchResultsView.Model = new List<PatientSearchResult>(searchResults.OrderByDescending(EMPIScore => EMPIScore)); 
            }
            else
            {
                patientSearchResultsView.Model = searchResults;
            }
            patientSearchResultsView.Show();
            patientSearchResultsView.UpdateView();

            this.btnCancel.Enabled = true;
            if ( searchResults.Count > 0 )
            {
                this.patientSearchResultsView.Focus();
                viewAccountsButton.Enabled = true;
                this.viewAccountsButton.Focus();
            }
            else
            {
                this.viewAccountsButton.Enabled = false;
            }

            if (patientSearchResultsView.EMPIResults)
            { 
                patientSearchResultsView.SetListViewDefaultItemSelected();
                patientSearchResultsView.FocusOnGrid();
            }
        }
        
        private void ResetResultsView( object sender, EventArgs e )
        {
            patientSearchResultsView.Hide();
            viewAccountsButton.Enabled = false;
            this.btnCancel.Enabled = false;
        }

        private void viewAccountsButton_Click( object sender, EventArgs e )
        {
            patientSearchResultsView.GetSelectedPatientFromPatientSearchResult();
        }

        private void patientSearchResultsView_PatientSelected( object sender, EventArgs e )
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.CurrentActivity = this.GetMPIV().CurrentActivity;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            // TLG 03/15/2006
            // from this screen, unconditionally return to the main screen

            if ( ParentForm is PatientAccessView )
            {
                ( (PatientAccessView)this.ParentForm ).ReLoad();
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// UpdateView method.
        /// </summary>
        /// 
        public override void UpdateView()
        {
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }

        #endregion

        #region Properties

        public PatientSearchResultsView PatientSearchResultsView
        {
            get
            {
                return patientSearchResultsView;
            }
        }

        public FindAPatientView FindAPatientView
        {
            get
            {
                return this.findAPatientView;
            }
        }

        public TabPage SelectedTab
        {
            get
            {
                return this.findPatientTabs.SelectedTab;
            }
        }

        public Activity CurrentActivity
        {
            get
            {
                return currentActivity;
            }
            set
            {
                currentActivity = value;
            }
        }

        #endregion

        #region Private Methods
        private MasterPatientIndexView GetMPIV()
        {
            return this.ParentControl as MasterPatientIndexView;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Initialize SearchView, call SearchView_Load in Event Handlers.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchView));
            this.resultsBackPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblSeacrhNotesMessage = new System.Windows.Forms.Label();
            this.lblSearchNotes = new System.Windows.Forms.Label();
            this.findPatientTabs = new System.Windows.Forms.TabControl();
            this.findByPatientTab = new System.Windows.Forms.TabPage();
            this.findAPatientView = new PatientAccess.UI.PatientSearch.FindAPatientView();
            this.findByGuarantorTab = new System.Windows.Forms.TabPage();
            this.findPatientByGuarantorView = new PatientAccess.UI.PatientSearch.FindPatientByGuarantorView();
            this.lblActivity = new System.Windows.Forms.Label();
            this.lblStaticActivity = new System.Windows.Forms.Label();
            this.patientSearchResultsView = new PatientAccess.UI.PatientSearch.PatientSearchResultsView();
            this.contextLabel = new PatientAccess.UI.UserContextView();
            this.btnCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.viewAccountsButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.pnlTransition = new System.Windows.Forms.Panel();
            this.resultsBackPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.findPatientTabs.SuspendLayout();
            this.findByPatientTab.SuspendLayout();
            this.findByGuarantorTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // resultsBackPanel
            // 
            this.resultsBackPanel.BackColor = System.Drawing.Color.Black;
            this.resultsBackPanel.Controls.Add(this.panel1);
            this.resultsBackPanel.Location = new System.Drawing.Point(10, 25);
            this.resultsBackPanel.Name = "resultsBackPanel";
            this.resultsBackPanel.Size = new System.Drawing.Size(1004, 557);
            this.resultsBackPanel.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblSeacrhNotesMessage);
            this.panel1.Controls.Add(this.lblSearchNotes);
            this.panel1.Controls.Add(this.findPatientTabs);
            this.panel1.Controls.Add(this.lblActivity);
            this.panel1.Controls.Add(this.lblStaticActivity);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1002, 555);
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
            // 
            // lblSeacrhNotesMessage
            // 
            this.lblSeacrhNotesMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeacrhNotesMessage.Location = new System.Drawing.Point(447, 5);
            this.lblSeacrhNotesMessage.Name = "lblSeacrhNotesMessage";
            this.lblSeacrhNotesMessage.Size = new System.Drawing.Size(538, 60);
            this.lblSeacrhNotesMessage.TabIndex = 2;
            this.lblSeacrhNotesMessage.Text = "When searching by Last name, the full name is required along with the Date of Birth. SSN or Phone number searches can be used instead of Last name search.         Account number or MRN are also valid search criteria.";
            // 
            // lblSearchNotes
            // 
            this.lblSearchNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchNotes.Location = new System.Drawing.Point(335, 25);
            this.lblSearchNotes.Name = "lblSearchNotes";
            this.lblSearchNotes.Size = new System.Drawing.Size(120, 16);
            this.lblSearchNotes.TabIndex = 1;
            this.lblSearchNotes.Text = "Search Notes:";
            // 
            // findPatientTabs
            // 
            this.findPatientTabs.Controls.Add(this.findByPatientTab);
            this.findPatientTabs.Controls.Add(this.findByGuarantorTab);
            this.findPatientTabs.Location = new System.Drawing.Point(10, 48);
            this.findPatientTabs.Name = "findPatientTabs";
            this.findPatientTabs.SelectedIndex = 0;
            this.findPatientTabs.Size = new System.Drawing.Size(984, 200);
            this.findPatientTabs.TabIndex = 0;
            this.findPatientTabs.SelectedIndexChanged += new System.EventHandler(this.findPatientTabs_SelectedIndexChanged);
            // 
            // findByPatientTab
            // 
            this.findByPatientTab.Controls.Add(this.findAPatientView);
            this.findByPatientTab.Location = new System.Drawing.Point(4, 22);
            this.findByPatientTab.Name = "findByPatientTab";
            this.findByPatientTab.Size = new System.Drawing.Size(976, 174);
            this.findByPatientTab.TabIndex = 0;
            this.findByPatientTab.Text = "Search by Patient";
            // 
            // findAPatientView
            // 
            this.findAPatientView.BackColor = System.Drawing.Color.White;
            this.findAPatientView.Location = new System.Drawing.Point(0, 0);
            this.findAPatientView.Model = null;
            this.findAPatientView.Name = "findAPatientView";
            this.findAPatientView.Size = new System.Drawing.Size(1010, 176);
            this.findAPatientView.TabIndex = 0;
            this.findAPatientView.PatientsFound += new System.EventHandler(this.PatientsFound);
            this.findAPatientView.SearchReset += new System.EventHandler(this.ResetResultsView);
            // 
            // findByGuarantorTab
            // 
            this.findByGuarantorTab.Controls.Add(this.findPatientByGuarantorView);
            this.findByGuarantorTab.Location = new System.Drawing.Point(4, 22);
            this.findByGuarantorTab.Name = "findByGuarantorTab";
            this.findByGuarantorTab.Size = new System.Drawing.Size(976, 174);
            this.findByGuarantorTab.TabIndex = 0;
            this.findByGuarantorTab.Text = "Search by Guarantor";
            // 
            // findPatientByGuarantorView
            // 
            this.findPatientByGuarantorView.BackColor = System.Drawing.Color.White;
            this.findPatientByGuarantorView.Location = new System.Drawing.Point(0, 0);
            this.findPatientByGuarantorView.Model = null;
            this.findPatientByGuarantorView.Name = "findPatientByGuarantorView";
            this.findPatientByGuarantorView.Size = new System.Drawing.Size(1010, 176);
            this.findPatientByGuarantorView.TabIndex = 0;
            this.findPatientByGuarantorView.PatientsFound += new System.EventHandler(this.PatientsFound);
            this.findPatientByGuarantorView.SearchReset += new System.EventHandler(this.ResetResultsView);
            // 
            // lblActivity
            // 
            this.lblActivity.Location = new System.Drawing.Point(71, 9);
            this.lblActivity.Name = "lblActivity";
            this.lblActivity.Size = new System.Drawing.Size(300, 23);
            this.lblActivity.TabIndex = 0;
            this.lblActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticActivity
            // 
            this.lblStaticActivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStaticActivity.Location = new System.Drawing.Point(10, 9);
            this.lblStaticActivity.Name = "lblStaticActivity";
            this.lblStaticActivity.Size = new System.Drawing.Size(55, 23);
            this.lblStaticActivity.TabIndex = 0;
            this.lblStaticActivity.Text = "Activity:";
            this.lblStaticActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // patientSearchResultsView
            // 
            this.patientSearchResultsView.BackColor = System.Drawing.Color.White;
            this.patientSearchResultsView.Location = new System.Drawing.Point(22, 283);
            this.patientSearchResultsView.Model = null;
            this.patientSearchResultsView.Name = "patientSearchResultsView";
            this.patientSearchResultsView.Size = new System.Drawing.Size(984, 292);
            this.patientSearchResultsView.TabIndex = 2;
            this.patientSearchResultsView.PatientSelectedFromSearchResult += new System.EventHandler<PatientAccess.UI.PatientSearch.PatientSelectedEventArgs>(this.patientSearchResultsView_PatientSelected);
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = System.Drawing.SystemColors.Control;
            this.contextLabel.Description = "  Find a Patient";
            this.contextLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextLabel.Location = new System.Drawing.Point(0, 0);
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new System.Drawing.Size(1024, 23);
            this.contextLabel.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(939, 589);
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // viewAccountsButton
            // 
            this.viewAccountsButton.Enabled = false;
            this.viewAccountsButton.Location = new System.Drawing.Point(835, 589);
            this.viewAccountsButton.Message = null;
            this.viewAccountsButton.Name = "viewAccountsButton";
            this.viewAccountsButton.Size = new System.Drawing.Size(95, 23);
            this.viewAccountsButton.TabIndex = 3;
            this.viewAccountsButton.Text = "&View Accounts";
            this.viewAccountsButton.Click += new System.EventHandler(this.viewAccountsButton_Click);
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(15, 284);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(992, 290);
            this.progressPanel1.TabIndex = 5;
            // 
            // pnlTransition
            // 
            this.pnlTransition.BackColor = System.Drawing.Color.White;
            this.pnlTransition.Location = new System.Drawing.Point(2, 1);
            this.pnlTransition.Name = "pnlTransition";
            this.pnlTransition.Size = new System.Drawing.Size(1018, 584);
            this.pnlTransition.TabIndex = 1;
            // 
            // SearchView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.viewAccountsButton);
            this.Controls.Add(this.contextLabel);
            this.Controls.Add(this.patientSearchResultsView);
            this.Controls.Add(this.resultsBackPanel);
            this.Controls.Add(this.progressPanel1);
            this.Controls.Add(this.pnlTransition);
            this.Name = "SearchView";
            this.Size = new System.Drawing.Size(1024, 620);
            this.Load += new System.EventHandler(this.SearchView_Load);
            this.resultsBackPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.findPatientTabs.ResumeLayout(false);
            this.findByPatientTab.ResumeLayout(false);
            this.findByGuarantorTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        private ControlView ParentControl
        {
            get
            {
                return parentControl;
            }
            set
            {
                parentControl = value;
            }
        }
        private bool HasEMPIResults
        {
            get
            {
                return FindAPatientView != null && FindAPatientView.PatientSearchResponse != null &&
                       FindAPatientView.PatientSearchResponse.HasEMPIResults;
            }
        }
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construction for SearchView, 
        /// calls InitializeComponent in Private Methods region.
        /// </summary>
        public SearchView()
        {
            InitializeComponent();

            EnableThemesOn( this );

            this.Refresh();
            this.SetStyle( ControlStyles.ResizeRedraw, true );

            if ( this.GetMPIV() != null )
            {
                this.patientSearchResultsView.PatientSelectedFromSearchResult += this.GetMPIV().OnPatientSelectedFromSearchResult;
            }
        }

        public SearchView( ControlView parent ) : this()
        {
            this.ParentControl = parent;

            this.CurrentActivity = this.GetMPIV().CurrentActivity;

            if ( this.CurrentActivity != null )
            {
                this.Message = String.Format( "{0} activity - {1}", this.Name, this.CurrentActivity.ContextDescription );
            }

            if ( this.GetMPIV() != null )
            {
                this.patientSearchResultsView.PatientSelectedFromSearchResult += this.GetMPIV().OnPatientSelectedFromSearchResult;
            }

        }
        /// <summary>
        /// Dispose method.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements

        private readonly Container components = null;

        private UserContextView contextLabel;
        private FindPatientByGuarantorView findPatientByGuarantorView;
        private FindAPatientView findAPatientView;
        private PatientSearchResultsView patientSearchResultsView;
        private ProgressPanel progressPanel1;
        private Activity currentActivity;

        private Panel resultsBackPanel;
        private Panel panel1;
        private Panel pnlTransition;

        private TabControl findPatientTabs;

        private TabPage findByPatientTab;
        private TabPage findByGuarantorTab;

        private ControlView parentControl;
        private Label lblStaticActivity;
        private Label lblActivity;

        private LoggingButton btnCancel;
        private Label lblSeacrhNotesMessage;
        private Label lblSearchNotes;
        private LoggingButton viewAccountsButton;

        #endregion

        #region Constants
        
        #endregion
    }
}