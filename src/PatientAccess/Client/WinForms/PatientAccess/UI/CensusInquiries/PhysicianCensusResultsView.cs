using System;
using System.Collections;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Census by Physician result view
    /// </summary>
    [Serializable]
    public class PhysicianCensusResultsView : ControlView
    {

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.searchResultsLabel = new System.Windows.Forms.Label();
            this.physicianGridControl = new PatientAccess.UI.CommonControls.GridControl();
            this.physicianResultsPanel = new System.Windows.Forms.Panel();
            this.searchResultsPanel = new System.Windows.Forms.Panel();
            this.noPhysiciansLabel = new System.Windows.Forms.Label();
            this.physicianResultsPanel.SuspendLayout();
            this.searchResultsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchResultsLabel
            // 
            this.searchResultsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.searchResultsLabel.Location = new System.Drawing.Point(7, 7);
            this.searchResultsLabel.Name = "searchResultsLabel";
            this.searchResultsLabel.Size = new System.Drawing.Size(88, 16);
            this.searchResultsLabel.TabIndex = 0;
            this.searchResultsLabel.Text = "Search Results";
            // 
            // physicianGridControl
            // 
            this.physicianGridControl.Location = new System.Drawing.Point(0, 0);
            this.physicianGridControl.Model = null;
            this.physicianGridControl.Name = "physicianGridControl";
            this.physicianGridControl.Size = new System.Drawing.Size(402, 117);
            this.physicianGridControl.TabIndex = 1;
            this.physicianGridControl.GridControl_Click += new PatientAccess.UI.CommonControls.GridControl.UltraGridClickEventHandler(this.physicianGridControl_GridControl_Click);
            this.physicianGridControl.GridControl_DoubleClick += new PatientAccess.UI.CommonControls.GridControl.UltraGridDoubleClickEventHandler( this.physicianGridControl_GridControl_DoubleClick );
            // 
            // physicianResultsPanel
            // 
            this.physicianResultsPanel.Controls.Add(this.searchResultsPanel);
            this.physicianResultsPanel.Controls.Add(this.searchResultsLabel);
            this.physicianResultsPanel.Location = new System.Drawing.Point(0, 0);
            this.physicianResultsPanel.Name = "physicianResultsPanel";
            this.physicianResultsPanel.Size = new System.Drawing.Size(416, 167);
            this.physicianResultsPanel.TabIndex = 0;
            // 
            // searchResultsPanel
            // 
            this.searchResultsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchResultsPanel.Controls.Add(this.physicianGridControl);
            this.searchResultsPanel.Controls.Add(this.noPhysiciansLabel);
            this.searchResultsPanel.Location = new System.Drawing.Point(7, 30);
            this.searchResultsPanel.Name = "searchResultsPanel";
            this.searchResultsPanel.Size = new System.Drawing.Size(402, 117);
            this.searchResultsPanel.TabIndex = 3;
            // 
            // noPhysiciansLabel
            // 
            this.noPhysiciansLabel.Location = new System.Drawing.Point(7, 7);
            this.noPhysiciansLabel.Name = "noPhysiciansLabel";
            this.noPhysiciansLabel.Size = new System.Drawing.Size(330, 16);
            this.noPhysiciansLabel.TabIndex = 0;
            this.noPhysiciansLabel.Text = "No physicians were found for the physician information entered.";
            // 
            // PhysicianCensusResultsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.physicianResultsPanel);
            this.Name = "PhysicianCensusResultsView";
            this.Size = new System.Drawing.Size(416, 167);
            this.physicianResultsPanel.ResumeLayout(false);
            this.searchResultsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        #region Event Handlers

        public event EventHandler PhysicianSelectionChanged;
        public event EventHandler PhysicianDoubleClicked;

        private void physicianGridControl_GridControl_Click( UltraGridRow ultraGridRow )
        {
            i_SelectedPhysicianName = ultraGridRow.Cells[0].Value.ToString();

            if( PhysicianSelectionChanged != null )
            {
                PhysicianSelectionChanged( this, 
                    new LooseArgs( ultraGridRow.Cells[GRIDCOL_NUMBER].Value ) );
            }
            
            PreviousSelectedPhysicianNumber = Convert.ToInt64( ultraGridRow.Cells[GRIDCOL_NUMBER].Value );
        }

        private void physicianGridControl_GridControl_DoubleClick( UltraGridRow ultraGridRow )
        {
            this.Cursor = Cursors.WaitCursor;
            i_SelectedPhysicianName = ultraGridRow.Cells[0].Value.ToString();

            if( PhysicianDoubleClicked != null )
            {
                PhysicianDoubleClicked( this,
                    new LooseArgs( ultraGridRow.Cells[GRIDCOL_NUMBER].Value ) );
            }

            PreviousSelectedPhysicianNumber = Convert.ToInt64( ultraGridRow.Cells[GRIDCOL_NUMBER].Value );
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.noPhysiciansLabel.Visible = false;
            this.physicianGridControl.Visible = true;
            FillDataSource();
            physicianGridControl.Focus(); 
        }

        public void NoPhysiciansFound()
        {
            this.physicianGridControl.Visible = false;
            this.noPhysiciansLabel.Visible = true;
            PreviousSelectedPhysicianNumber = 0;
        }

        public void ResetPhysicianResults()
        {
            this.noPhysiciansLabel.Visible = false;
            this.physicianGridControl.Visible = true;
            dataSource.Rows.Clear();
            if( PhysicianSelectionChanged != null )
            {
                PhysicianSelectionChanged( this, 
                    new LooseArgs( ( object )0 ) );
            }
        }

        public void ResetPreviousSelectedPhysician()
        {
            PreviousSelectedPhysicianNumber = 0;
        }
        public void SetRowSelectionActiveAppearance()
        {
           physicianGridControl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            physicianGridControl.SetRowSelectionDimAppearance();
        }
        #endregion

        #region Properties
        public string SelectedPhysicianName
        {
            get
            {
                return i_SelectedPhysicianName;
            }
            set
            {
                i_SelectedPhysicianName = value;
            }
        }
        #endregion

        #region Private Methods
        
        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();
            this.dataSource.Band.Key = "Physician_Band";
            this.dataSource.Band.Columns.Add( "Physician", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Number", typeof( long ) );
        }

        private void update_Model()
        {
            physicianGridControl.CensusGrid.DataSource = dataSource;
        }

        private void CustomizeGridLayout()
        {
            this.physicianGridControl.CensusGrid.DisplayLayout.Bands[0].Columns[GRIDCOL_NUMBER].Width = 20;
            this.physicianGridControl.Visible = true;
            this.physicianGridControl.Show();
        }

        private void FillDataSource()
        {
            object [] physicianList = new object[2];  // 2 = total columns in grid
            ArrayList all = (ArrayList)this.Model;
            int i = 0;

            foreach( Physician physician in all )
            {
                physicianList[0] = physician.ToString();
                physicianList[1] = physician.Oid;
                dataSource.Rows.Add( physicianList );
                if( PreviousSelectedPhysicianNumber != 0 &&
                    physician.Oid == PreviousSelectedPhysicianNumber )
                {
                    physicianGridControl.CensusGrid.ActiveRow = 
                        (UltraGridRow)physicianGridControl.CensusGrid.Rows[i];
                }
                i++;
            }
        }

        #endregion

        #region Private Properties

        private long PreviousSelectedPhysicianNumber
        {
            get
            {
                return previousSelectedPhysician;
            }
            set
            {
                previousSelectedPhysician = value;
            }
        }

        #endregion

        #region Construction and Finalization

        public PhysicianCensusResultsView()
        {
            this.InitializeComponent();
            this.noPhysiciansLabel.Visible = false;
            CreateDataStructure();
            update_Model();
            CustomizeGridLayout();
            base.EnableThemesOn( this );
        }

        #endregion

        #region Data Elements

        private UltraDataSource dataSource;
        private Label searchResultsLabel;
        private Panel physicianResultsPanel;
        private Label noPhysiciansLabel;
        private Panel searchResultsPanel;   
        private GridControl physicianGridControl;
        private long previousSelectedPhysician = 0;
        private string i_SelectedPhysicianName;

        #endregion

        #region Constants
        private const int GRIDCOL_NUMBER = 1;
        #endregion
    }
}