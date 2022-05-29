using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.PhysicianSearchViews
{
	/// <summary>
	/// Summary description for PhysicianSearchResultsView.
	/// </summary>
    public class PhysicianSearchResultsView_notused : ControlView
    {
        #region Events

        public event EventHandler PhysicianSelected;

        #endregion

        #region Event Handlers
        
        private void physicianResultsListView_GotFocus(object sender, EventArgs e)
        {
            physicianResultsListView.Items[0].Selected = true;
            if( PhysicianSelected != null )
            {
                PhysicianSelected( this,
                    new LooseArgs( physicianResultsListView.Items[0].SubItems[ ROW_SELECTED ].Text) );
            }
            SelectedPhysicianNumber = Convert.ToInt64( physicianResultsListView.Items[0].SubItems[ ROW_SELECTED ].Text );          
        }

        private void physicianResultsListView_Click(object sender, EventArgs e)
        {
            foreach( ListViewItem item in physicianResultsListView.Items )
            {
                if( item.Selected )
                {
                    
                    itemSelected = itemSelected+1;
                   if( PhysicianSelected != null )
                    {
                        PhysicianSelected( this,
                            new LooseArgs( item.SubItems[ itemSelected ].Text) );

                    }
                    SelectedPhysicianNumber = Convert.ToInt64( item.SubItems[ itemSelected ].Text ) ;
                    BreadCrumbLogger.GetInstance.Log( String.Format( "{0} Physician selected from the search results", 
                        SelectedPhysicianNumber ) );

                    break;

                }
            }

        }
       
        private void physicianResultsListView_DoubleClick(object sender, EventArgs e)
        {
            PhysicianDetailView physicianDetail = new PhysicianDetailView();
            foreach( ListViewItem item in physicianResultsListView.Items )
            {
                
                if( item.Selected )
                {
                    
                    itemSelected = itemSelected+1;
                    SelectedPhysicianNumber = Convert.ToInt64( item.SubItems[ itemSelected ].Text );

                    BreadCrumbLogger.GetInstance.Log( String.Format( "{0} Physician selected from the search results", 
                        SelectedPhysicianNumber ) );

                    break;

                }
                
                
            }
            physicianDetail.SelectPhysicians = this.SelectedPhysicianNumber;
			try
			{
				physicianDetail.ShowDialog( this );
			}
			finally
			{
				physicianDetail.Dispose();
			}
        }

        private void physicianResultsListView_MouseMove(object sender, MouseEventArgs e)
        {
            bool mouseOutOfColumn   = true;
            bool mouseOutOfRow      = true;
            int currentColumn       = -1;
            int currentRow          = -1;
            int heightOfTitle       = 0;
            int headerWidth         = 0;
            int numberOfRows        = physicianResultsListView.Items.Count;

            if( numberOfRows == 0 )
            {
                return;
            }
            
            if( numberOfRows > 0 )
            {
                heightOfTitle = physicianResultsListView.Items[0].GetBounds( ItemBoundsPortion.Label ).Top;
            }

            foreach( ColumnHeader header in physicianResultsListView.Columns )
            {   
                currentColumn++;
                headerWidth = headerWidth + header.Width;
                if( headerWidth > e.X && currentColumn <= 3)
                {
                    mouseOutOfColumn = false;
                    break;
                }
            }

            if( mouseOutOfColumn )
            {
                return;
            }

            foreach( ListViewItem item in physicianResultsListView.Items )
            {
                currentRow++;
                heightOfTitle = heightOfTitle + item.Bounds.Height;

                if( heightOfTitle > e.Y )
                {
                    mouseOutOfRow = false;
                    break;
                }
            }

            if ( mouseOutOfRow ) 
            {
                return;
            }

            this.phyicianResultViewtoolTip.SetToolTip( 
                physicianResultsListView, 
                physicianResultsListView.Items[currentRow].SubItems[currentColumn].Text );
        }

   #endregion       

        #region Methods

        public override void UpdateView()
        {
            FillDataSource();
        }

        #endregion

        #region Properties

	    private new ArrayList Model
        {
            get
            {
                return i_PhysicianList;
            }
            set
            {
                i_PhysicianList = value;
            }
        }
        private long SelectedPhysicianNumber
        {
            get
            {
                return i_SelectedPhysicianNumber ;
            }
            set
            {
                i_SelectedPhysicianNumber = value;
            }
        }


        #endregion

        #region Private Methods

        private void FillDataSource()
        {
            ArrayList physicians = this.Model;
            if( physicians != null && physicians.Count > 0 )
            {
                foreach( Physician physician in physicians )
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = string.Empty;
                    item.Tag = physician;
                    item.SubItems.Add( physician.PhysicianNumber.ToString() );
                    item.SubItems.Add( physician.ToString() );
                    if( physician.Specialization != null )
                    {
                        item.SubItems.Add( String.Format( "{0} {1}",
                            physician.Specialization.Code,physician.Specialization.Description ) );
                    }
                    else
                    {
                        item.SubItems.Add( string.Empty );
                    }
                    physicianResultsListView.Items.Add( item );
                         
                }

                this.physicianResultsListView.Focus();
                this.physicianResultsListView.Items[0].Selected = true;
            }

            else
            {
                this.physicianResultsListView .TabStop = false;
            }
        }

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {   
			this.components = new System.ComponentModel.Container();
			this.physicianResultsPanel = new System.Windows.Forms.Panel();
			this.noPhysiciansFoundLabel = new System.Windows.Forms.Label();
			this.physicianResultsListView = new System.Windows.Forms.ListView();
			this.columnWithZeroWidth = new System.Windows.Forms.ColumnHeader();
			this.columnPhysicianNumber = new System.Windows.Forms.ColumnHeader();
			this.columnPhysicianName = new System.Windows.Forms.ColumnHeader();
			this.columnSpecialty = new System.Windows.Forms.ColumnHeader();
			this.columnStatus = new System.Windows.Forms.ColumnHeader();
			this.columnActive = new System.Windows.Forms.ColumnHeader();
			this.columnPrivileges = new System.Windows.Forms.ColumnHeader();
			this.columnExcld = new System.Windows.Forms.ColumnHeader();
			this.phyicianResultViewtoolTip = new System.Windows.Forms.ToolTip(this.components);
			this.physicianResultsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// physicianResultsPanel
			// 
			this.physicianResultsPanel.Controls.Add(this.noPhysiciansFoundLabel);
			this.physicianResultsPanel.Controls.Add(this.physicianResultsListView);
			this.physicianResultsPanel.Location = new System.Drawing.Point(0, 0);
			this.physicianResultsPanel.Name = "physicianResultsPanel";
			this.physicianResultsPanel.Size = new System.Drawing.Size(880, 184);
			this.physicianResultsPanel.TabIndex = 0;
			this.physicianResultsPanel.TabStop = true;
			// 
			// noPhysiciansFoundLabel
			// 
			this.noPhysiciansFoundLabel.AutoSize = true;
			this.noPhysiciansFoundLabel.Location = new System.Drawing.Point(0, 0);
			this.noPhysiciansFoundLabel.Name = "noPhysiciansFoundLabel";
			this.noPhysiciansFoundLabel.Size = new System.Drawing.Size(0, 16);
			this.noPhysiciansFoundLabel.TabIndex = 0;
			// 
			// physicianResultsListView
			// 
			this.physicianResultsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																									   this.columnWithZeroWidth,
																									   this.columnPhysicianNumber,
																									   this.columnPhysicianName,
																									   this.columnSpecialty,
																									   this.columnStatus,
																									   this.columnActive,
																									   this.columnPrivileges,
																									   this.columnExcld});
			this.physicianResultsListView.FullRowSelect = true;
			this.physicianResultsListView.GridLines = true;
			this.physicianResultsListView.HideSelection = false;
			this.physicianResultsListView.Location = new System.Drawing.Point(0, 0);
			this.physicianResultsListView.MultiSelect = false;
			this.physicianResultsListView.Name = "physicianResultsListView";
			this.physicianResultsListView.Size = new System.Drawing.Size(879, 183);
			this.physicianResultsListView.TabIndex = 0;
			this.physicianResultsListView.View = System.Windows.Forms.View.Details;
			this.physicianResultsListView.Click += new System.EventHandler(this.physicianResultsListView_Click);
			this.physicianResultsListView.GotFocus += new System.EventHandler(this.physicianResultsListView_GotFocus);
			this.physicianResultsListView.DoubleClick += new System.EventHandler(this.physicianResultsListView_DoubleClick);
			this.physicianResultsListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.physicianResultsListView_MouseMove);
			// 
			// columnWithZeroWidth
			// 
			this.columnWithZeroWidth.Text = string.Empty;
			this.columnWithZeroWidth.Width = 0;
			// 
			// columnPhysicianNumber
			// 
			this.columnPhysicianNumber.Text = "Number";
			this.columnPhysicianNumber.Width = 70;
			// 
			// columnPhysicianName
			// 
			this.columnPhysicianName.Text = "Physician Name";
			this.columnPhysicianName.Width = 200;
			// 
			// columnSpecialty
			// 
			this.columnSpecialty.Text = "Specialty";
			this.columnSpecialty.Width = 175;
			// 
			// columnStatus
			// 
			this.columnStatus.Text = "Status";
			this.columnStatus.Width = 75;
			// 
			// columnActive
			// 
			this.columnActive.Text = "Active/Inactive";
			this.columnActive.Width = 95;
			// 
			// columnPrivileges
			// 
			this.columnPrivileges.Text = "Adm Privileges";
			this.columnPrivileges.Width = 90;
			// 
			// columnExcld
			// 
			this.columnExcld.Text = "Excld from Adm/Att";
			this.columnExcld.Width = 170;
			// 
			// PhysicianSearchResultsView_notused
			// 
			this.Controls.Add(this.physicianResultsPanel);
			this.Name = "PhysicianSearchResultsView_notused";
			this.Size = new System.Drawing.Size(888, 240);
			this.physicianResultsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

        #endregion

        #region Construction and Finalization

        /// <summary>
        /// Constructor
        /// </summary>
        public PhysicianSearchResultsView_notused()
        {
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
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }


        #endregion

        #region Data Elements

        private Panel physicianResultsPanel ;
        public ListView physicianResultsListView;
        private ArrayList i_PhysicianList;
        private Label noPhysiciansFoundLabel;
        private long i_SelectedPhysicianNumber ;
        private ColumnHeader columnPhysicianNumber;
        private ColumnHeader columnPhysicianName;
        private ColumnHeader columnSpecialty;
		private ColumnHeader columnActive;
		private ColumnHeader columnExcld;
		private ColumnHeader columnPrivileges;
		private ColumnHeader columnStatus;
        private ColumnHeader columnWithZeroWidth;
//        private System.Windows.Forms.ColumnHeader columnWithNoLabel;
        private ToolTip phyicianResultViewtoolTip;
        private IContainer components;
        private int itemSelected = 0;


        #endregion       
           
        #region Constants

        private const int ROW_SELECTED = 1;

        #endregion
        

      }
}
