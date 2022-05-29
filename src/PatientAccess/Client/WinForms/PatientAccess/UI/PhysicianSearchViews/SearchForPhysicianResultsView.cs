using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.PhysicianSearchViews
{
	/// <summary>
	/// Summary description for SearchForPhysicianResultsView.
	/// </summary>
	//TODO: Create XML summary comment for SearchForPhysicianResultsView
	[Serializable]
	public class SearchForPhysicianResultsView : ControlView
	{
		#region
		public event EventHandler PhysicianSelected;
        public event EventHandler NoPhysicianSelected;
		#endregion

		#region Event Handlers
		private void physicianSearchResultsView1_SelectedIndexChanged(object sender, EventArgs e)
		{
            Physician ps =  null;

            if( this.physicianResultsListView.SelectedItems.Count > 0 )
            {
                this.PhysicianSelected( this, new LooseArgs( this.physicianResultsListView.SelectedItems[0].Tag as Physician ) );
                //            this.PhysicianSelected( this, new LooseArgs( this.physicianResultsListView ) );

                ps = this.physicianResultsListView.SelectedItems[0].Tag as Physician;
                if( ps != null )
                {
                    BreadCrumbLogger.GetInstance.Log( string.Format( "{0} physician selected", ps.PhysicianNumber ) );
                }
            }
            else
            {
                this.NoPhysicianSelected( this, null );
            }
		}

        private void rbActive_CheckedChanged(object sender, EventArgs e)
        {
            RefillData();
        }

        private void rbInactive_CheckedChanged(object sender, EventArgs e)
        {
            RefillData();
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            RefillData();
        }
		#endregion

		#region public Methods

		public void ResetSearchResultsView()
		{
            this.Model = null;
            this.physicianResultsListView.Items.Clear();
            this.columnExcld.Width = DEFAULT_RIGHT_COLOUMN_WIDTH;
            this.physicianResultsListView.Visible = true;
            this.physicianResultsListView.Show();
            this.panelNoPhysicianMsgFoundBorder.Visible = false;

		}

        public override void UpdateView()
        {
            FillPhysiciansInListView();
        }
		#endregion

		#region public Properties
        public new ArrayList Model
        {
            private get
            {
                return i_PhysicianList;
            }
            set
            {
                i_PhysicianList = value;
            }
        }

		#endregion

		#region Private Methods

        private void FillPhysiciansInListView()
        {
            ArrayList allPhysicians = this.Model != null? this.Model : null;
            ArrayList physicians = new ArrayList();

            if( allPhysicians != null )
            {
                foreach( Physician physician in allPhysicians )
                {
                    if( this.rbActive.Checked && physician.ActiveInactiveFlag == "Active" )
                    {
                        physicians.Add( physician );
                        continue;
                    }
                     
                    if( this.rbInactive.Checked && physician.ActiveInactiveFlag != "Active" )
                    {
                        physicians.Add( physician );
                        continue;
                    }
       
                    if( this.rbAll.Checked )
                    {
                        physicians.Add( physician );
                    }                
                }

            }
            if( physicians != null && physicians.Count > 0 )
            {
                this.panelNoPhysicianMsgFoundBorder.Visible = false;
                this.physicianResultsListView.Visible = true;

                foreach( Physician physician in physicians )
                {                    
                    ListViewItem item = new ListViewItem();
                    item.Tag = physician;
                    item.SubItems.Add( physician.PhysicianNumber.ToString() );
                    item.SubItems.Add( physician.ToString() );
                    if( physician.Specialization != null )
                    {
                        item.SubItems.Add( String.Format( "{0}",
                            physician.Specialization.AsFormattedSpeciality ) );
                    }
                    else
                    {
                        item.SubItems.Add( "" );
                    }
                    item.SubItems.Add( physician.Status );                    
                    item.SubItems.Add( physician.ActiveInactiveFlag );
                    item.SubItems.Add( physician.AdmittingPrivileges );
                    item.SubItems.Add( physician.ExcludedStatus );

//                    item.SubItems.Add( "test" );



                    this.physicianResultsListView.Items.Add( item );
                         
                }
                if( this.physicianResultsListView.Items.Count > MAX_ROWS_VISIBLE  )
                {
                    this.columnExcld.Width = NEW_RIGHT_COLOUMN_WIDTH;
                }
                this.physicianResultsListView.Focus();
                this.physicianResultsListView.Items[0].Selected = true;
            }

            else
            {
                this.physicianResultsListView.TabStop = false;
                this.NoPhysiciansFound();
            }
        }


	    private void NoPhysiciansFound()
        {
            this.lblNoPhysicianFoundMsg.Text = UIErrorMessages.NO_PHYSICIANS_FOUND;
            this.panelNoPhysicianMsgFoundBorder.Visible = true;
            this.physicianResultsListView.Visible = false;
            this.NoPhysicianSelected( this, null );
        }

        private void RefillData()
        {
            this.physicianResultsListView.Items.Clear();
            this.FillPhysiciansInListView();
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.rbActive = new System.Windows.Forms.RadioButton();
			this.rbInactive = new System.Windows.Forms.RadioButton();
			this.rbAll = new System.Windows.Forms.RadioButton();
			this.physicianResultsListView = new System.Windows.Forms.ListView();
			this.columnZeroLen = new System.Windows.Forms.ColumnHeader();
			this.columnPhysicianNumber = new System.Windows.Forms.ColumnHeader();
			this.columnPhysicianName = new System.Windows.Forms.ColumnHeader();
			this.columnSpecialty = new System.Windows.Forms.ColumnHeader();
			this.columnStatus = new System.Windows.Forms.ColumnHeader();
			this.columnActive = new System.Windows.Forms.ColumnHeader();
			this.columnPrivileges = new System.Windows.Forms.ColumnHeader();
			this.columnExcld = new System.Windows.Forms.ColumnHeader();
			this.panelNoPhysicianMsgFoundBorder = new System.Windows.Forms.Panel();
			this.lblNoPhysicianFoundMsg = new System.Windows.Forms.Label();
			this.panelNoPhysicianMsgFoundBorder.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Display:";
			// 
			// rbActive
			// 
			this.rbActive.Checked = true;
			this.rbActive.Location = new System.Drawing.Point(52, 3);
			this.rbActive.Name = "rbActive";
			this.rbActive.Size = new System.Drawing.Size(54, 24);
			this.rbActive.TabIndex = 1;
			this.rbActive.TabStop = true;
			this.rbActive.Text = "Active";
			this.rbActive.CheckedChanged += new System.EventHandler(this.rbActive_CheckedChanged);
			// 
			// rbInactive
			// 
			this.rbInactive.Location = new System.Drawing.Point(109, 3);
			this.rbInactive.Name = "rbInactive";
			this.rbInactive.Size = new System.Drawing.Size(61, 24);
			this.rbInactive.TabIndex = 2;
			this.rbInactive.Text = "Inactive";
			this.rbInactive.CheckedChanged += new System.EventHandler(this.rbInactive_CheckedChanged);
			// 
			// rbAll
			// 
			this.rbAll.Location = new System.Drawing.Point(173, 3);
			this.rbAll.Name = "rbAll";
			this.rbAll.Size = new System.Drawing.Size(36, 24);
			this.rbAll.TabIndex = 3;
			this.rbAll.Text = "All";
			this.rbAll.CheckedChanged += new System.EventHandler(this.rbAll_CheckedChanged);
			// 
			// physicianResultsListView
			// 
			this.physicianResultsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																									   this.columnZeroLen,
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
			this.physicianResultsListView.Location = new System.Drawing.Point(0, 27);
			this.physicianResultsListView.MultiSelect = false;
			this.physicianResultsListView.Name = "physicianResultsListView";
			this.physicianResultsListView.Size = new System.Drawing.Size(898, 178);
			this.physicianResultsListView.TabIndex = 4;
			this.physicianResultsListView.View = System.Windows.Forms.View.Details;
			this.physicianResultsListView.SelectedIndexChanged += new System.EventHandler(this.physicianSearchResultsView1_SelectedIndexChanged);
			// 
			// columnZeroLen
			// 
			this.columnZeroLen.Text = "";
			this.columnZeroLen.Width = 0;
			// 
			// columnPhysicianNumber
			// 
			this.columnPhysicianNumber.Text = "Number";
			this.columnPhysicianNumber.Width = 64;
			// 
			// columnPhysicianName
			// 
			this.columnPhysicianName.Text = "Physician Name";
			this.columnPhysicianName.Width = 241;
			// 
			// columnSpecialty
			// 
			this.columnSpecialty.Text = "Specialty";
			this.columnSpecialty.Width = 205;
			// 
			// columnStatus
			// 
			this.columnStatus.Text = "Status";
			this.columnStatus.Width = 79;
			// 
			// columnActive
			// 
			this.columnActive.Text = "Active/Inactive";
			this.columnActive.Width = 90;
			// 
			// columnPrivileges
			// 
			this.columnPrivileges.Text = "Adm Privileges";
			this.columnPrivileges.Width = 86;
			// 
			// columnExcld
			// 
			this.columnExcld.Text = "Excld from Adm/Att";
			this.columnExcld.Width = 128;
			// 
			// panelNoPhysicianMsgFoundBorder
			// 
			this.panelNoPhysicianMsgFoundBorder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelNoPhysicianMsgFoundBorder.Controls.Add(this.lblNoPhysicianFoundMsg);
			this.panelNoPhysicianMsgFoundBorder.Location = new System.Drawing.Point(2, 29);
			this.panelNoPhysicianMsgFoundBorder.Name = "panelNoPhysicianMsgFoundBorder";
			this.panelNoPhysicianMsgFoundBorder.Size = new System.Drawing.Size(895, 174);
			this.panelNoPhysicianMsgFoundBorder.TabIndex = 5;
			this.panelNoPhysicianMsgFoundBorder.Visible = false;
			// 
			// lblNoPhysicianFoundMsg
			// 
			this.lblNoPhysicianFoundMsg.Location = new System.Drawing.Point(1, 3);
			this.lblNoPhysicianFoundMsg.Name = "lblNoPhysicianFoundMsg";
			this.lblNoPhysicianFoundMsg.Size = new System.Drawing.Size(680, 23);
			this.lblNoPhysicianFoundMsg.TabIndex = 0;
			// 
			// SearchForPhysicianResultsView
			// 
			this.Controls.Add(this.rbAll);
			this.Controls.Add(this.rbInactive);
			this.Controls.Add(this.rbActive);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.physicianResultsListView);
			this.Controls.Add(this.panelNoPhysicianMsgFoundBorder);
			this.Name = "SearchForPhysicianResultsView";
			this.Size = new System.Drawing.Size(902, 217);
			this.panelNoPhysicianMsgFoundBorder.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public SearchForPhysicianResultsView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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
		private Container components = null;
		private Label label1;
		private RadioButton rbActive;
		private RadioButton rbInactive;
		private RadioButton rbAll;
        public ListView physicianResultsListView;
        private ColumnHeader columnPhysicianNumber;
        private ColumnHeader columnPhysicianName;
        private ColumnHeader columnSpecialty;
        private ColumnHeader columnStatus;
        private ColumnHeader columnActive;
        private ColumnHeader columnPrivileges;
        private ColumnHeader columnExcld;
        private ArrayList i_PhysicianList;
        
		#endregion

		#region Constants
        private const int MAX_ROWS_VISIBLE = 11,
                          DEFAULT_RIGHT_COLOUMN_WIDTH = 128,
                          NEW_RIGHT_COLOUMN_WIDTH = 112;
		#endregion

        private Label lblNoPhysicianFoundMsg;
        private ColumnHeader columnZeroLen;
        private Panel panelNoPhysicianMsgFoundBorder;

	}
}
