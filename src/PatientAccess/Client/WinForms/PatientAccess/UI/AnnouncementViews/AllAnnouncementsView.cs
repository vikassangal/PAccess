using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.AnnouncementViews
{
	public class AllAnnouncementsView : ControlView
	{
        #region Event Handlers
        private void allAnnouncementsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( this.allAnnouncementsListView.SelectedItems.Count > 0 )
            {
                Announcement selectedAnnouncement = this.allAnnouncementsListView.SelectedItems[0].Tag as Announcement;
                this.AnnouncementService.DisplayAnnouncmentForEdit( selectedAnnouncement );
            }
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( this.Model != null )
            {
                allAnnouncementsListView.BeginUpdate();
                allAnnouncementsListView.Items.Clear();
                ListViewItem item;
                foreach( Announcement announcement in this.Model )
                {
                    item = new ListViewItem();
                    item.SubItems.Add( announcement.StopDate.ToString("MM/dd/yyyy") );
                    item.SubItems.Add( announcement.Description );
                    item.SubItems.Add( announcement.Author );
                    item.SubItems.Add( announcement.SaveDate.ToString("MM/dd/yyyy") );
                    item.Tag = announcement;
                    item.Text = String.Empty;
                    allAnnouncementsListView.Items.Add( item );
                }
                allAnnouncementsListView.EndUpdate();
            }
        }
        public void ResetControls()
        {
            if( allAnnouncementsListView.SelectedItems.Count > 0 )
            {
                allAnnouncementsListView.SelectedItems[0].Selected = false;
            }
        }

        #endregion

        #region Properties
        public ICollection FacilitiesToManage
        {
            get
            {
                return i_FacilitiesToManage;
            }
            set
            {
                i_FacilitiesToManage = value;
            }
        }

        public AnnouncementService AnnouncementService
        {
            private get
            {
                return i_AnnouncementService;
            }
            set
            {
                i_AnnouncementService = value;
            }
        }
        public new ICollection Model
        {
            private get
            {
                return base.Model as ICollection;
            }
            set
            {
                base.Model = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.allAnnouncementsListView = new System.Windows.Forms.ListView();
            this.stopDateColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDummy = new System.Windows.Forms.ColumnHeader();
            this.descriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.authorColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.postDateColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // allAnnouncementsListView
            // 
            this.allAnnouncementsListView.AutoArrange = false;
            this.allAnnouncementsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                                       this.columnHeaderDummy,
                                                                                                       this.stopDateColumnHeader,
                                                                                                       this.descriptionColumnHeader,
                                                                                                       this.authorColumnHeader,
                                                                                                       this.postDateColumnHeader});
            this.allAnnouncementsListView.FullRowSelect = true;
            this.allAnnouncementsListView.GridLines = true;
            this.allAnnouncementsListView.HideSelection = false;
            this.allAnnouncementsListView.Location = new System.Drawing.Point(0, 0);
            this.allAnnouncementsListView.MultiSelect = false;
            this.allAnnouncementsListView.Name = "allAnnouncementsListView";
            this.allAnnouncementsListView.Size = new System.Drawing.Size(955, 192);
            this.allAnnouncementsListView.TabIndex = 0;
            this.allAnnouncementsListView.View = System.Windows.Forms.View.Details;
            this.allAnnouncementsListView.SelectedIndexChanged += new System.EventHandler(this.allAnnouncementsListView_SelectedIndexChanged);
            // 
            // columnHeaderDummy
            // 
            this.columnHeaderDummy.Width = 0;
            // 
            // stopDateColumnHeader
            // 
            this.stopDateColumnHeader.Text = "Stop Date";
            this.stopDateColumnHeader.Width = 97;
            // 
            // descriptionColumnHeader
            // 
            this.descriptionColumnHeader.Text = "Description";
            this.descriptionColumnHeader.Width = 520;
            // 
            // authorColumnHeader
            // 
            this.authorColumnHeader.Text = "Author";
            this.authorColumnHeader.Width = 237;
            // 
            // postDateColumnHeader
            // 
            this.postDateColumnHeader.Text = "Post Date";
            this.postDateColumnHeader.Width = 97;
            // 
            // AllAnnouncementsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.allAnnouncementsListView);
            this.Name = "AllAnnouncementsView";
            this.Size = new System.Drawing.Size(955, 192);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization

        public AllAnnouncementsView()
        {
            // This call is required by the Windows Form Designer.
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
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Data Elements
		private IContainer components = null;
        private ColumnHeader descriptionColumnHeader;
        private ColumnHeader authorColumnHeader;
        private ColumnHeader stopDateColumnHeader;
        private ColumnHeader postDateColumnHeader;
        private ColumnHeader columnHeaderDummy;
        private ListView allAnnouncementsListView;
        private ICollection i_FacilitiesToManage;
        private AnnouncementService i_AnnouncementService;
        #endregion

        #region Constants
        #endregion
	}
}

