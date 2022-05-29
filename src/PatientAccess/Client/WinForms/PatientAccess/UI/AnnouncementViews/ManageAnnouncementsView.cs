using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.AnnouncementViews
{
	public class ManageAnnouncementsView : ControlView
	{
        #region Event Handlers
        private void ManageAnnouncementsView_Load( object sender, EventArgs e )
        {
            addEditAnnouncementsView.UpdateView();
        }

        private void manageAnnouncementTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( manageAnnouncementTabControl.SelectedTab == addEditTabPage )
            {
                addEditAnnouncementsView.UpdateView();
            }
            if( manageAnnouncementTabControl.SelectedTab == viewTabPage )
            {
                announcementsView.UpdateView();
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
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
            this.announcementViewPanel = new System.Windows.Forms.Panel();
            this.manageAnnouncementTabControl = new System.Windows.Forms.TabControl();
            this.viewTabPage = new System.Windows.Forms.TabPage();
            this.addEditTabPage = new System.Windows.Forms.TabPage();
            this.addEditAnnouncementsView = new PatientAccess.UI.AnnouncementViews.AddEditAnnouncementsView();
            this.announcementsView = new PatientAccess.UI.AnnouncementViews.AnnouncementsView();
            this.contextLabel = new PatientAccess.UI.UserContextView();
            this.announcementViewPanel.SuspendLayout();
            this.manageAnnouncementTabControl.SuspendLayout();
            this.viewTabPage.SuspendLayout();
            this.addEditTabPage.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // announcementViewPanel
            // 
            this.announcementViewPanel.BackColor = System.Drawing.Color.White;
            this.announcementViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.announcementViewPanel.Controls.Add(this.manageAnnouncementTabControl);
            this.announcementViewPanel.Location = new System.Drawing.Point(10, 25);
            this.announcementViewPanel.Name = "announcementViewPanel";
            this.announcementViewPanel.Size = new System.Drawing.Size(1002, 585);
            this.announcementViewPanel.TabIndex = 1;
            // 
            // manageAnnouncementTabControl
            // 
            this.manageAnnouncementTabControl.Controls.Add(this.viewTabPage);
            this.manageAnnouncementTabControl.Controls.Add(this.addEditTabPage);
            this.manageAnnouncementTabControl.ItemSize = new System.Drawing.Size(79, 18);
            this.manageAnnouncementTabControl.Location = new System.Drawing.Point(8, 8);
            this.manageAnnouncementTabControl.Name = "manageAnnouncementTabControl";
            this.manageAnnouncementTabControl.SelectedIndex = 0;
            this.manageAnnouncementTabControl.Size = new System.Drawing.Size(985, 570);
            this.manageAnnouncementTabControl.TabIndex = 0;
            this.manageAnnouncementTabControl.SelectedIndexChanged += new System.EventHandler(this.manageAnnouncementTabControl_SelectedIndexChanged);
            // 
            // viewTabPage
            // 
            this.viewTabPage.BackColor = System.Drawing.Color.White;
            this.viewTabPage.Controls.Add(this.announcementsView);
            this.viewTabPage.Location = new System.Drawing.Point(4, 22);
            this.viewTabPage.Name = "viewTabPage";
            this.viewTabPage.Size = new System.Drawing.Size(977, 544);
            this.viewTabPage.TabIndex = 0;
            this.viewTabPage.Text = "View";
            // 
            // addEditTabPage
            // 
            this.addEditTabPage.BackColor = System.Drawing.Color.White;
            this.addEditTabPage.Controls.Add(this.addEditAnnouncementsView);
            this.addEditTabPage.Location = new System.Drawing.Point(4, 22);
            this.addEditTabPage.Name = "addEditTabPage";
            this.addEditTabPage.Size = new System.Drawing.Size(977, 544);
            this.addEditTabPage.TabIndex = 1;
            this.addEditTabPage.Text = "Add/Edit";
            this.addEditTabPage.Visible = false;
            // 
            // addEditAnnouncementsView
            // 
            this.addEditAnnouncementsView.AnnouncementService = null;
            this.addEditAnnouncementsView.BackColor = System.Drawing.Color.White;
            this.addEditAnnouncementsView.Location = new System.Drawing.Point(0, 0);
            this.addEditAnnouncementsView.Model = null;
            this.addEditAnnouncementsView.Name = "addEditAnnouncementsView";
            this.addEditAnnouncementsView.Size = new System.Drawing.Size(968, 550);
            this.addEditAnnouncementsView.TabIndex = 0;
            // 
            // announcementsView
            // 
            this.announcementsView.AnnouncementService = null;
            this.announcementsView.BackColor = System.Drawing.Color.White;
            this.announcementsView.Location = new System.Drawing.Point(0, 0);
            this.announcementsView.Model = null;
            this.announcementsView.Name = "announcementsView";
            this.announcementsView.ShowManageButton = false;
            this.announcementsView.Size = new System.Drawing.Size(965, 550);
            this.announcementsView.TabIndex = 0;
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = System.Drawing.SystemColors.Control;
            this.contextLabel.Description = " Manage  Announcements";
            this.contextLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextLabel.Location = new System.Drawing.Point(0, 0);
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new System.Drawing.Size(1024, 23);
            this.contextLabel.TabIndex = 4;
            
            // 
            // ManageAnnouncementsView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.announcementViewPanel);
            this.Controls.Add(this.contextLabel);
            this.Name = "ManageAnnouncementsView";
            this.Size = new System.Drawing.Size(1024, 620);
            this.announcementViewPanel.ResumeLayout(false);
            this.manageAnnouncementTabControl.ResumeLayout(false);
            this.viewTabPage.ResumeLayout(false);
            this.addEditTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.Load +=new EventHandler(ManageAnnouncementsView_Load);

        }
        
        #endregion

        #region Construction and Finalization
        public ManageAnnouncementsView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
            base.EnableThemesOn( this );


            AnnouncementService announcementService = new AnnouncementService();
            announcementsView.AnnouncementService = announcementService;
            addEditAnnouncementsView.AnnouncementService = announcementService;
            this.manageAnnouncementTabControl.SelectedTab = this.addEditTabPage;
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

        private Panel announcementViewPanel;
        private UserContextView contextLabel;
        private TabControl manageAnnouncementTabControl;
        private TabPage viewTabPage;
        private TabPage addEditTabPage;
        private AnnouncementsView announcementsView;
        private AddEditAnnouncementsView addEditAnnouncementsView;
        private IContainer components = null;

        #endregion

        #region Constants
        #endregion
   
	}
}

