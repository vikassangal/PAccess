using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.AnnouncementViews
{
	public class AnnouncementsView : ControlView
	{
        #region Events
        public event EventHandler ShowManageAnnouncement;
        #endregion

        #region Event Handlers
        private void AnnouncementsView_Load(object sender, EventArgs e)
        {
            currentAnnouncementsView.AnnouncementService = this.AnnouncementService;
            this.AnnouncementService.AnnouncementDetailsView = announcementDetailsView;
            //this.UpdateView();
        }
        private void manageButton_Click(object sender, EventArgs e)
        {
            if(  ShowManageAnnouncement != null )
            {
                ShowManageAnnouncement( this, e );
            }
            
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            ICollection announcements = this.AnnouncementService.GetCurrentAnnouncements();
            this.manageButton.Visible = this.ShowManageButton;
            if( announcements != null && announcements.Count > 0 )
            {
                currentAnnouncementsView.Model = announcements;
                currentAnnouncementsView.UpdateView();
                noAnnouncementLabel.Visible = false;
            }
            else
            {
                currentAnnouncementsView.Visible = false;
                announcementDetailsView.Visible = false;
                noAnnouncementLabel.Visible = true;
            }
        }

        #endregion

        #region Properties
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
        public bool ShowManageButton
        {
            private get
            {
                return i_ShowManageButton;
            }
            set
            {
                i_ShowManageButton = value;
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
			this.currentAnnouncementPanel = new System.Windows.Forms.Panel();
			this.noAnnouncementLabel = new System.Windows.Forms.Label();
			this.currentAnnouncementsView = new PatientAccess.UI.AnnouncementViews.CurrentAnnouncementsView();
			this.showDetailsLabel = new System.Windows.Forms.Label();
			this.announcementDetailViewPanel = new System.Windows.Forms.Panel();
			this.announcementDetailsView = new PatientAccess.UI.AnnouncementViews.AnnouncementDetailsView();
			this.manageButton = new LoggingButton();
			this.currentAnnouncementPanel.SuspendLayout();
			this.announcementDetailViewPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// currentAnnouncementPanel
			// 
			this.currentAnnouncementPanel.BackColor = System.Drawing.Color.White;
			this.currentAnnouncementPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.currentAnnouncementPanel.Controls.Add(this.noAnnouncementLabel);
			this.currentAnnouncementPanel.Controls.Add(this.currentAnnouncementsView);
			this.currentAnnouncementPanel.Location = new System.Drawing.Point(10, 12);
			this.currentAnnouncementPanel.Name = "currentAnnouncementPanel";
			this.currentAnnouncementPanel.Size = new System.Drawing.Size(955, 290);
			this.currentAnnouncementPanel.TabIndex = 0;
			// 
			// noAnnouncementLabel
			// 
			this.noAnnouncementLabel.Location = new System.Drawing.Point(10, 10);
			this.noAnnouncementLabel.Name = "noAnnouncementLabel";
			this.noAnnouncementLabel.Size = new System.Drawing.Size(176, 23);
			this.noAnnouncementLabel.TabIndex = 2;
			this.noAnnouncementLabel.Text = "No announcements are in effect.";
			this.noAnnouncementLabel.Visible = false;
			// 
			// currentAnnouncementsView
			// 
			this.currentAnnouncementsView.AnnouncementService = null;
			this.currentAnnouncementsView.BackColor = System.Drawing.Color.White;
			this.currentAnnouncementsView.Location = new System.Drawing.Point(0, 0);
			this.currentAnnouncementsView.Model = null;
			this.currentAnnouncementsView.Name = "currentAnnouncementsView";
			this.currentAnnouncementsView.Size = new System.Drawing.Size(955, 290);
			this.currentAnnouncementsView.TabIndex = 0;
			// 
			// showDetailsLabel
			// 
			this.showDetailsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.showDetailsLabel.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(51)), ((System.Byte)(204)));
			this.showDetailsLabel.Location = new System.Drawing.Point(10, 320);
			this.showDetailsLabel.Name = "showDetailsLabel";
			this.showDetailsLabel.Size = new System.Drawing.Size(100, 15);
			this.showDetailsLabel.TabIndex = 2;
			this.showDetailsLabel.Text = "Show Details";
			// 
			// announcementDetailViewPanel
			// 
			this.announcementDetailViewPanel.BackColor = System.Drawing.Color.White;
			this.announcementDetailViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.announcementDetailViewPanel.Controls.Add(this.announcementDetailsView);
			this.announcementDetailViewPanel.Location = new System.Drawing.Point(10, 340);
			this.announcementDetailViewPanel.Name = "announcementDetailViewPanel";
			this.announcementDetailViewPanel.Size = new System.Drawing.Size(955, 195);
			this.announcementDetailViewPanel.TabIndex = 3;
			// 
			// announcementDetailsView
			// 
			this.announcementDetailsView.BackColor = System.Drawing.Color.White;
			this.announcementDetailsView.Location = new System.Drawing.Point(0, 0);
			this.announcementDetailsView.Model = null;
			this.announcementDetailsView.Name = "announcementDetailsView";
			this.announcementDetailsView.Size = new System.Drawing.Size(970, 195);
			this.announcementDetailsView.TabIndex = 3;
			// 
			// manageButton
			// 
			this.manageButton.BackColor = System.Drawing.SystemColors.Control;
			this.manageButton.Location = new System.Drawing.Point(890, 545);
			this.manageButton.Name = "manageButton";
			this.manageButton.TabIndex = 1;
			this.manageButton.Text = "&Manage";
			this.manageButton.Visible = false;
			this.manageButton.Click += new System.EventHandler(this.manageButton_Click);
			// 
			// AnnouncementsView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.manageButton);
			this.Controls.Add(this.announcementDetailViewPanel);
			this.Controls.Add(this.showDetailsLabel);
			this.Controls.Add(this.currentAnnouncementPanel);
			this.Name = "AnnouncementsView";
			this.Size = new System.Drawing.Size(990, 575);
			this.Load += new System.EventHandler(this.AnnouncementsView_Load);
			this.currentAnnouncementPanel.ResumeLayout(false);
			this.announcementDetailViewPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion

        #region Construction and Finalization
        public AnnouncementsView()
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
        private Panel currentAnnouncementPanel;
        private Label showDetailsLabel;
        private Panel announcementDetailViewPanel;
        private CurrentAnnouncementsView currentAnnouncementsView;
        private IContainer components = null;
        private AnnouncementService i_AnnouncementService;
        private LoggingButton manageButton;
        private Label noAnnouncementLabel;
        private AnnouncementDetailsView announcementDetailsView;
        private bool i_ShowManageButton = false;
        #endregion

        #region Constants
        #endregion

	}
}

