using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.AnnouncementViews
{
	public class AnnouncementsBaseView : ControlView
	{
        #region Events
        public event EventHandler ShowManageAnnouncementView;
        #endregion

        #region Event Handlers
        private void AnnouncementsBaseView_Load(object sender, EventArgs e)
        {
            bool canManage = false;
            canManage = announcementService.HasAddEditPrivileges();
            announcementsView.ShowManageButton = canManage;
            announcementsView.UpdateView();
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods

	    private void OnButtonClicked( object sender, EventArgs e )
        {
            if(  ShowManageAnnouncementView != null)
            {
                ShowManageAnnouncementView( this, e );
            }
        }

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
            this.contextLabel = new PatientAccess.UI.UserContextView();
            this.announcementViewPanel = new System.Windows.Forms.Panel();
            this.announcementsView = new PatientAccess.UI.AnnouncementViews.AnnouncementsView();
            this.announcementViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = System.Drawing.SystemColors.Control;
            this.contextLabel.Description = "  Announcements";
            this.contextLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextLabel.Location = new System.Drawing.Point(0, 0);
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new System.Drawing.Size(1024, 23);
            this.contextLabel.TabIndex = 1;
            // 
            // announcementViewPanel
            // 
            this.announcementViewPanel.BackColor = System.Drawing.Color.White;
            this.announcementViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.announcementViewPanel.Controls.Add(this.announcementsView);
            this.announcementViewPanel.Location = new System.Drawing.Point(12, 28);
            this.announcementViewPanel.Name = "announcementViewPanel";
            this.announcementViewPanel.Size = new System.Drawing.Size(997, 577);
            this.announcementViewPanel.TabIndex = 2;
            // 
            // announcementsView
            // 
            this.announcementsView.AnnouncementService = null;
            this.announcementsView.BackColor = System.Drawing.Color.White;
            this.announcementsView.Location = new System.Drawing.Point(1, -1);
            this.announcementsView.Model = null;
            this.announcementsView.Name = "announcementsView";
            this.announcementsView.Size = new System.Drawing.Size(1002, 580);
            this.announcementsView.TabIndex = 0;
            this.announcementsView.ShowManageAnnouncement += new EventHandler( this.OnButtonClicked );
            // 
            // AnnouncementsBaseView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.announcementViewPanel);
            this.Controls.Add(this.contextLabel);
            this.Name = "AnnouncementsBaseView";
            this.Size = new System.Drawing.Size(1024, 620);
            this.Load += new System.EventHandler(this.AnnouncementsBaseView_Load);
            this.announcementViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public AnnouncementsBaseView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            
            base.EnableThemesOn( this );

            this.Refresh();
            this.SetStyle( ControlStyles.ResizeRedraw, true );
            announcementService = new AnnouncementService();
            announcementsView.AnnouncementService = announcementService;
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
        private UserContextView contextLabel;
        private Panel announcementViewPanel;
        private AnnouncementsView announcementsView;
        private IContainer components = null;
        private AnnouncementService announcementService;
        #endregion

        #region Constants
        #endregion
	}
}

