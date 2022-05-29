using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Admissions, Discharges and Transfers Census Results View
    /// </summary>
    [Serializable]
    public class ADTCensusResultsView : ControlView
    {
        #region Events
        #endregion

        #region Event Handlers
        
        private void ADTCensusResultsView_Load(object sender, EventArgs e)
        {
            this.progressPanel1.Visible     = false;
        }

        private void GridControlClick( UltraGridRow ultraGridRow )
        {
            i_previousSelectedAccount = 
                ultraGridRow.Cells[COL_PATIENT_ACCOUNT].Value.ToString();
        }
        
        private void BeforeSortOrderChange( object sender, BeforeSortChangeEventArgs e )
        {
            e.Cancel = sortingNotAllowed;
        }
        
        #endregion

        #region Methods

        public void BeforeWork(object sender, EventArgs e)
        {
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
            this.adtCensusResultsViewPanel.Visible = false;
        }

        public void AfterWork(object sender, EventArgs e)
        {
            if (this.IsDisposed || this.Disposing)
                return;

            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
            this.adtCensusResultsViewPanel.Visible = true;
        }

        /// <summary>
        /// Updates the ADT Census Results View based on ADT type and 
        /// sort parameter.
        /// </summary>
        public override void UpdateView()
        {
            string selectedADTReport = String.Empty;
            
            selectedADTReport  = ADTReportsHashtable[this.ADTType].ToString();

            Assembly assembly  = Assembly.GetExecutingAssembly();
            Type ADTReportType = assembly.GetType( selectedADTReport );

            ADTCensusResults ADTResults  = 
                (ADTCensusResults)Activator.CreateInstance( ADTReportType );
            
            ADTResults.AccountProxies           = null;
            ADTResults.AccountProxies           = this.Model;
            ADTResults.ADTType                  = this.ADTType;
            ADTResults.PreviousSelectedAccount  = this.PreviousSelectedAccount;

            ADTResults.UpdateGrid( adtResultGridControl );
            ADTResults.UpdateView();

            this.noAccountsFoundLabel.Visible = false;
            this.adtResultGridControl.Visible = true;

            if( !resetMode )
            {
                SortChanged();
            }
            resetMode  = false;
        }

        /// <summary>
        /// Displays no accounts found message.
        /// </summary>
        public void NoAccountsFound()
        {
            this.adtCensusResultsViewPanel.Visible = true;
            this.noAccountsFoundLabel.Visible = true;
            this.i_previousSelectedAccount = String.Empty;
            this.adtResultGridControl.Hide();
            this.adtCensusResultsViewPanel.Visible = true;
        }

        /// <summary>
        /// Sorts the ADT Census Results View data, based on sort parameter.
        /// </summary>
        public void SortChanged()
        {
            UltraGridBand ADT_Band;
            ADT_Band = adtResultGridControl.CensusGrid.DisplayLayout.Bands[ADT_BAND];

            sortingNotAllowed = false;
            if( i_SortByColumn.Equals( SORT_BY_TIME ) )
            {
                ADT_Band.Columns[COL_PATIENT_NAME_ACCOUNT_Hidden].SortIndicator = SortIndicator.None;
                ADT_Band.Columns[COL_TRANSACTION_TIME_Hidden].SortIndicator = SortIndicator.Ascending;
            }
            else
            {
                ADT_Band.Columns[COL_TRANSACTION_TIME_Hidden].SortIndicator = SortIndicator.None;
                ADT_Band.Columns[COL_PATIENT_NAME_ACCOUNT_Hidden ].SortIndicator  = SortIndicator.Ascending;
            }

            sortingNotAllowed = true;
        }

        /// <summary>
        /// Sets the ADT Census Results View to default view.
        /// </summary>
        public void ResetView()
        {
            this.noAccountsFoundLabel.Visible = false;
            this.adtResultGridControl.Visible = true;
            ((UltraDataSource)this.adtResultGridControl.CensusGrid.DataSource).Rows.Clear();
            this.i_previousSelectedAccount = String.Empty;
            resetMode = true;
            UpdateView();
        }
        public void SetRowSelectionActiveAppearance()
        {
            adtResultGridControl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            adtResultGridControl.SetRowSelectionDimAppearance();
        }
        #endregion

        #region Properties
        
        public new ArrayList Model
        {
            private get
            {
                return base.Model as ArrayList;
            }
            set
            {
                base.Model = value;
            }
        }
        
        public string ADTType
        {
            private get
            {
                return i_ADTType;
            }
            set
            {
                i_ADTType = value;
            }
        }

        public string SortByColumn
        {
            get
            {
                return i_SortByColumn;
            }
            set
            {
                i_SortByColumn = value;
            }
        }
      
        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the ADT Census Results grid control to the view and sets the grid to 
        /// display admissions data.
        /// </summary>
        private void WireUpADTCensusResultView()
        {
            this.adtResultGridControl = new GridControl();
            this.adtResultGridControl.BackColor = Color.White;
            this.adtResultGridControl.Location = new Point(0, 0);
            this.adtResultGridControl.Model = null;
            this.adtResultGridControl.Dock = DockStyle.Fill;
            this.adtResultGridControl.Name = "adtResultGridControl";
            this.adtResultGridControl.Size = new Size(896, 198);
            this.adtResultGridControl.TabIndex = 0;
            this.adtResultGridControl.GridControl_Click += 
                new GridControl.UltraGridClickEventHandler
                ( this.GridControlClick );
            this.adtResultGridControl.GridControl_BeforeSortOrderChange +=
                new GridControl.BeforeSortChangeEventHandler
                ( this.BeforeSortOrderChange );
            this.adtCensusResultsViewPanel.Controls.Add(this.adtResultGridControl);
        }

        private void PopulateADTReportsList()
        {
            ADTReportsHashtable = new Hashtable();
            ADTReportsHashtable.Add( ADT_TYPE_ADMISSIONS, ADT_ADMISSION_RESULTS );
            ADTReportsHashtable.Add( ADT_TYPE_DISCHARGES, ADT_DISCHARGE_RESULTS );
            ADTReportsHashtable.Add( ADT_TYPE_TRANSFERS, ADT_TRANSFER_RESULTS );
            ADTReportsHashtable.Add( ADT_TYPE_ALL, ADT_ALLADT_RESULTS );
        }

        #endregion

        #region Private Properties

        private string PreviousSelectedAccount
        {
            get 
            {
                return i_previousSelectedAccount;
            }
        }

        #endregion

        #region Windows Form Designer generated code
        
        private void InitializeComponent()
        {
            this.adtCensusResultsViewPanel = new System.Windows.Forms.Panel();
            this.noAccountsFoundLabel = new System.Windows.Forms.Label();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.adtCensusResultsViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // adtCensusResultsViewPanel
            // 
            this.adtCensusResultsViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.adtCensusResultsViewPanel.Controls.Add(this.noAccountsFoundLabel);
            this.adtCensusResultsViewPanel.Location = new System.Drawing.Point(10, 0);
            this.adtCensusResultsViewPanel.Name = "adtCensusResultsViewPanel";
            this.adtCensusResultsViewPanel.Size = new System.Drawing.Size(896, 173);
            this.adtCensusResultsViewPanel.TabIndex = 0;
            // 
            // noAccountsFoundLabel
            // 
            this.noAccountsFoundLabel.Location = new System.Drawing.Point(10, 5);
            this.noAccountsFoundLabel.Name = "noAccountsFoundLabel";
            this.noAccountsFoundLabel.Size = new System.Drawing.Size(274, 16);
            this.noAccountsFoundLabel.TabIndex = 1;
            this.noAccountsFoundLabel.Visible = false;
            this.noAccountsFoundLabel.Text = MSG_NO_PATIENTS_FOUND;
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(0, 0);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(912, 168);
            this.progressPanel1.TabIndex = 1;
            // 
            // ADTCensusResultsView
            // 
            this.Load += new EventHandler(ADTCensusResultsView_Load);
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.progressPanel1);
            this.Controls.Add(this.adtCensusResultsViewPanel);
            this.Name = "ADTCensusResultsView";
            this.Size = new System.Drawing.Size(906, 173);
            this.adtCensusResultsViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Construction and Finalization
        
        public ADTCensusResultsView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
            PopulateADTReportsList();
            i_ADTType       = ADT_TYPE_ADMISSIONS;
            i_SortByColumn  = SORT_BY_TIME;
            WireUpADTCensusResultView();
            this.UpdateView();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        
        #endregion        

        #region Data Elements
        
        private Container                     components = null;

        private ProgressPanel       progressPanel1;        
        private GridControl                                         adtResultGridControl;

        private Panel                          adtCensusResultsViewPanel;

        private Label                          noAccountsFoundLabel;
        
        private Hashtable                                           ADTReportsHashtable;

        private string                                              i_previousSelectedAccount = null;
        private string                                              i_ADTType;
        private string                                              i_SortByColumn;

        private bool                                                resetMode = false;
        private bool                                                sortingNotAllowed = true;

        #endregion

        #region Constants

        private const string MSG_NO_PATIENTS_FOUND 
            =  "No patients were found based on the selected criteria.";

        private const string
            ADT_TYPE_ADMISSIONS = "A",
            ADT_TYPE_DISCHARGES = "D",
            ADT_TYPE_TRANSFERS = "T",
            ADT_TYPE_ALL = "E",

            ADT_ADMISSION_RESULTS = "PatientAccess.UI.CensusInquiries.ADTCensusAdmissionResults",
            ADT_DISCHARGE_RESULTS = "PatientAccess.UI.CensusInquiries.ADTCensusDischargeResults",
            ADT_TRANSFER_RESULTS = "PatientAccess.UI.CensusInquiries.ADTCensusTransferResults",
            ADT_ALLADT_RESULTS = "PatientAccess.UI.CensusInquiries.ADTCensusAllADTResults",

            ADT_BAND = "ADT_Band",
            COL_PATIENT_ACCOUNT = "Account",
            COL_TRANSACTION_TIME_Hidden = "Time_Hidden",
            COL_PATIENT_NAME_ACCOUNT_Hidden = "Patient_Hidden",

            SORT_BY_TIME = "Time";

        #endregion

    }
}
