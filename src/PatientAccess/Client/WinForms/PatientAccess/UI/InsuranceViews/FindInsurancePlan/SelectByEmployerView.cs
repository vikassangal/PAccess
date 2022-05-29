using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.FindInsurancePlan
{
    /// <summary>
    /// Summary description for SelectByEmployerView.
    /// </summary>
    public class SelectByEmployerView : SelectByView
    {
        #region Events
        
        public event EventHandler CoveredGroupSelected;
        public event EventHandler CloseFormOnEnterKeyEvent;

        #endregion

        #region Event Handlers
        private void CloseFormOnEnterKeyEventHandler(object sender, EventArgs e)
        {   // Close dialog if ENTER key pressed if OK button is disabled.
            if( CloseFormOnEnterKeyEvent != null )
            {
                CloseFormOnEnterKeyEvent( this, e );
            }
        }

        private void searchField_PerformSearch(object sender, EventArgs e)
        {
            if( searchField.SearchText.Length > 0 )
            {
                this.FindForm().Cursor = Cursors.WaitCursor;
                try
                {
                    ICollection coveredGroups = InsuranceBroker.CoveredGroupsMatching( searchField.SearchText, PatientAccount.Facility.Oid, PatientAccount.AdmitDate );
                    coveredGroupListView.DisplayCoveredGroups( coveredGroups );

                    if( coveredGroups.Count > 0 )
                    {
                        coveredGroupListView.Activate();
                    }
                    else
                    {
                        listView.Items.Clear();
                    }
                }
                catch( RemotingTimeoutException )
                {
                    MessageBox.Show( UIErrorMessages.INSURANCE_SEARCH_TIMEOUT_MSG );
                }
                finally
                {
                    this.FindForm().Cursor = Cursors.Default;
                }
            }
            else if( CloseFormOnEnterKeyEvent != null )
            {   // Close dialog if ENTER key pressed
                CloseFormOnEnterKeyEvent( this, null );
            }
        }

        private void employerListView_SelectedCoveredGroupChanged(object sender, EventArgs e)
        {
            if( coveredGroupListView.SelectedCoveredGroup.Employer != null )
            {
                this.selectedEmployer = coveredGroupListView.SelectedCoveredGroup.Employer.AsEmployer() as Employer;
            }
            
            if( this.CoveredGroupSelected != null )
            {
                this.CoveredGroupSelected(this, new SelectInsuranceArgs(null, this.selectedEmployer));
            }
            base.PopulateInsurancePlans( coveredGroupListView.SelectedCoveredGroup.InsurancePlans );
        }
        #endregion

        #region Methods
        public override void Activate()
        {
            LineLabelCaption = "Plans from selected employer";
            ResetControls();
        }

        public override void ActivateSearchField()
        {
            searchField.Activate();
        }
        #endregion

        #region Properties
        public override bool PlanCanBeApplied
        {
            get
            {
                return SelectedInsurancePlan != null;
            }
        }

        public override string SearchText
        {
            get
            {
                return searchField.SearchText;
            }
            set
            {
                searchField.SearchText = value;
            }
        }
        #endregion

        #region Private Methods
        protected override void ResetControls()
        {
            coveredGroupListView.DisplayCoveredGroups( null );
            base.ResetControls();
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.coveredGroupListView = new PatientAccess.UI.InsuranceViews.FindInsurancePlan.CoveredGroupListView();
            this.lockingListView = new PatientAccess.UI.CommonControls.LockingListView();
            this.colHeaderName = new System.Windows.Forms.ColumnHeader();
            this.colHeaderNatlID = new System.Windows.Forms.ColumnHeader();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.searchField = new PatientAccess.UI.CommonControls.SearchField();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add( this.coveredGroupListView );
            this.panelMain.Controls.Add( this.lockingListView );
            this.panelMain.Location = new System.Drawing.Point( 8, 60 );
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size( 668, 277 );
            this.panelMain.TabIndex = 1;
            // 
            // coveredGroupListView
            // 
            this.coveredGroupListView.CoveredGroups = null;
            this.coveredGroupListView.CoverMessage = "No Items Found";
            this.coveredGroupListView.CoverPadding = 40;
            this.coveredGroupListView.Location = new System.Drawing.Point( 0, 20 );
            this.coveredGroupListView.Model = null;
            this.coveredGroupListView.Name = "coveredGroupListView";
            this.coveredGroupListView.SelectedCoveredGroup = null;
            this.coveredGroupListView.ShowCover = true;
            this.coveredGroupListView.Size = new System.Drawing.Size( 668, 257 );
            this.coveredGroupListView.TabIndex = 1;
            this.coveredGroupListView.CloseFormOnEnterKeyEvent += new System.EventHandler( this.CloseFormOnEnterKeyEventHandler );
            this.coveredGroupListView.SelectedCoveredGroupChanged += new System.EventHandler( this.employerListView_SelectedCoveredGroupChanged );
            // 
            // lockingListView
            // 
            this.lockingListView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.colHeaderName,
            this.colHeaderNatlID} );
            this.lockingListView.Location = new System.Drawing.Point( 0, 0 );
            this.lockingListView.LockColumnSize = true;
            this.lockingListView.Name = "lockingListView";
            this.lockingListView.Scrollable = false;
            this.lockingListView.Size = new System.Drawing.Size( 667, 20 );
            this.lockingListView.TabIndex = 0;
            this.lockingListView.TabStop = false;
            this.lockingListView.UseCompatibleStateImageBehavior = false;
            this.lockingListView.View = System.Windows.Forms.View.Details;
            // 
            // colHeaderName
            // 
            this.colHeaderName.Text = "Employer";
            this.colHeaderName.Width = 410;
            // 
            // colHeaderNatlID
            // 
            this.colHeaderNatlID.Text = "National ID";
            this.colHeaderNatlID.Width = 254;
            // 
            // lineLabel2
            // 
            this.lineLabel2.Caption = "Employers with active plans";
            this.lineLabel2.Location = new System.Drawing.Point( 5, 5 );
            this.lineLabel2.Name = "lineLabel2";
            this.lineLabel2.Size = new System.Drawing.Size( 668, 19 );
            this.lineLabel2.TabIndex = 0;
            this.lineLabel2.TabStop = false;
            // 
            // searchField
            // 
            this.searchField.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.searchField.Location = new System.Drawing.Point( 8, 30 );
            this.searchField.Mask = "";
            this.searchField.MaxFieldLength = 25;
            this.searchField.Model = null;
            this.searchField.Name = "searchField";
            this.searchField.SearchText = "";
            this.searchField.Size = new System.Drawing.Size( 565, 23 );
            this.searchField.TabIndex = 0;
            this.searchField.TextBoxEnabled = true;
            this.searchField.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.searchField.PerformSearch += new System.EventHandler( this.searchField_PerformSearch );
            // 
            // SelectByEmployerView
            // 
            this.Controls.Add( this.searchField );
            this.Controls.Add( this.panelMain );
            this.Controls.Add( this.lineLabel2 );
            this.Name = "SelectByEmployerView";
            this.Size = new System.Drawing.Size( 684, 458 );
            this.Controls.SetChildIndex( this.lineLabel2, 0 );
            this.Controls.SetChildIndex( this.panelMain, 0 );
            this.Controls.SetChildIndex( this.searchField, 0 );
            this.panelMain.ResumeLayout( false );
            this.ResumeLayout( false );

		}
        #endregion
        #endregion

        #region Constructors and Finalization
        public SelectByEmployerView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
			this.searchField.Button.Text = "Sear&ch";
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

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container                     components = null;

        private Panel                          panelMain;

        private LineLabel           lineLabel2;
        private SearchField         searchField;

        private ColumnHeader                   colHeaderName;
        private ColumnHeader                   colHeaderNatlID;

        private CoveredGroupListView coveredGroupListView;

        private LockingListView     lockingListView;

        private Employer                                            selectedEmployer = new Employer();
        #endregion

        #region Constants
        #endregion
    }
}
