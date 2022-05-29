using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.AnnouncementViews
{
    public class CurrentAnnouncementsView : ControlView
    {
        #region Event Handlers
        private void announcementsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( this.announcementsListView.SelectedItems.Count > 0 )
            {
                this.AnnouncementService.DisplaySelectedAnnouncement
                    ( announcementsListView.SelectedItems[0].Tag as Announcement );
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( this.Model != null )
            {
                announcementsListView.BeginUpdate();
                announcementsListView.Items.Clear();
                ListViewItem item;
                foreach( Announcement announcement in this.Model )
                {
                    item = new ListViewItem();
                    item.Tag = announcement;
                    item.Text = String.Empty;
                    item.SubItems.Add( announcement.Description );
                    item.SubItems.Add( announcement.Author );
                    item.SubItems.Add( announcement.SaveDate.ToString("MM/dd/yyyy") );
                    announcementsListView.Items.Add( item );
                }
                if( announcementsListView.Items.Count > 0 )
                {
                    announcementsListView.Items[0].Selected = true;
                    announcementsListView.Select();
                }
                announcementsListView.EndUpdate();
            }
        }
        #endregion

        #region Properties
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
            this.announcementsListView = new System.Windows.Forms.ListView();
            this.descriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDummy = new System.Windows.Forms.ColumnHeader();
            this.authorColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.postDateColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // announcementsListView
            // 
            this.announcementsListView.AutoArrange = false;
            this.announcementsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                                    this.columnHeaderDummy,
                                                                                                    this.descriptionColumnHeader,
                                                                                                    this.authorColumnHeader,
                                                                                                    this.postDateColumnHeader});
            this.announcementsListView.FullRowSelect = true;
            this.announcementsListView.GridLines = true;
            this.announcementsListView.HideSelection = false;
            this.announcementsListView.Location = new System.Drawing.Point(0, 0);
            this.announcementsListView.MultiSelect = false;
            this.announcementsListView.Name = "announcementsListView";
            this.announcementsListView.Size = new System.Drawing.Size(955, 290);
            this.announcementsListView.TabIndex = 0;
            this.announcementsListView.View = System.Windows.Forms.View.Details;
            this.announcementsListView.SelectedIndexChanged += new System.EventHandler(this.announcementsListView_SelectedIndexChanged);
            // 
            // columnHeaderDummy
            // 
            this.columnHeaderDummy.Width = 0;
            // 
            // descriptionColumnHeader
            // 
            this.descriptionColumnHeader.Text = "Description";
            this.descriptionColumnHeader.Width = 620;
            // 
            // authorColumnHeader
            // 
            this.authorColumnHeader.Text = "Author";
            this.authorColumnHeader.Width = 219;
            // 
            // postDateColumnHeader
            // 
            this.postDateColumnHeader.Text = "Post Date";
            this.postDateColumnHeader.Width = 96;
            // 
            // CurrentAnnouncementsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.announcementsListView);
            this.Name = "CurrentAnnouncementsView";
            this.Size = new System.Drawing.Size(955, 290);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public CurrentAnnouncementsView()
        {
            // This call is required by the Windows Form Designer.
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
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private ListView announcementsListView;
        private ColumnHeader authorColumnHeader;
        private ColumnHeader descriptionColumnHeader;
        private ColumnHeader columnHeaderDummy;
        private IContainer components = null;
        private ColumnHeader postDateColumnHeader;
        private AnnouncementService i_AnnouncementService;
        #endregion

        #region Constants
        #endregion

    }
}
