using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.AnnouncementViews
{
	public class AddEditAnnouncementsView : ControlView
	{

        #region Event Handlers
        private void AddEditAnnouncementsView_Load(object sender, EventArgs e)
        {
            facilitiesToManage = this.AnnouncementService.GetFacilitiesToManage();
            addEditAnnouncementView.FacilitiesToManage = facilitiesToManage;
            ICollection allRoles = this.AnnouncementService.GetAllRoles();
            addEditAnnouncementView.Roles = allRoles;
            allAnnouncementsView.AnnouncementService = this.AnnouncementService;
            this.AnnouncementService.AddEditAnnouncementView = addEditAnnouncementView;
            this.AnnouncementService.AddEditAnnouncementsView = this;
            addEditAnnouncementView.AnnouncementService = this.AnnouncementService;
            this.addEditAnnouncementView.ResetControls();
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {           
            ICollection announcements = this.AnnouncementService.GetAllAnnouncementsFor( facilitiesToManage );
            allAnnouncementsView.Model = announcements;
            allAnnouncementsView.UpdateView();
        }

        public void ResetControls()
        {
            allAnnouncementsView.ResetControls();
            addEditAnnouncementView.ResetControls();
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
            this.allAnnouncementPanel = new System.Windows.Forms.Panel();
            this.allAnnouncementsView = new PatientAccess.UI.AnnouncementViews.AllAnnouncementsView();
            this.addEditPanel = new System.Windows.Forms.Panel();
            this.addEditAnnouncementView = new PatientAccess.UI.AnnouncementViews.AddEditAnnouncementView();
            this.allAnnouncementPanel.SuspendLayout();
            this.addEditPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // allAnnouncementPanel
            // 
            this.allAnnouncementPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.allAnnouncementPanel.Controls.Add(this.allAnnouncementsView);
            this.allAnnouncementPanel.Location = new System.Drawing.Point(8, 12);
            this.allAnnouncementPanel.Name = "allAnnouncementPanel";
            this.allAnnouncementPanel.Size = new System.Drawing.Size(958, 192);
            this.allAnnouncementPanel.TabIndex = 0;
            // 
            // allAnnouncementsView
            // 
            this.allAnnouncementsView.AnnouncementService = null;
            this.allAnnouncementsView.BackColor = System.Drawing.Color.White;
            this.allAnnouncementsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.allAnnouncementsView.FacilitiesToManage = null;
            this.allAnnouncementsView.Location = new System.Drawing.Point(0, 0);
            this.allAnnouncementsView.Model = null;
            this.allAnnouncementsView.Name = "allAnnouncementsView";
            this.allAnnouncementsView.Size = new System.Drawing.Size(956, 190);
            this.allAnnouncementsView.TabIndex = 0;
            // 
            // addEditPanel
            // 
            this.addEditPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addEditPanel.Controls.Add(this.addEditAnnouncementView);
            this.addEditPanel.Location = new System.Drawing.Point(8, 224);
            this.addEditPanel.Name = "addEditPanel";
            this.addEditPanel.Size = new System.Drawing.Size(958, 312);
            this.addEditPanel.TabIndex = 1;
            // 
            // addEditAnnouncementView
            // 
            this.addEditAnnouncementView.AnnouncementService = null;
            this.addEditAnnouncementView.BackColor = System.Drawing.Color.White;
            this.addEditAnnouncementView.FacilitiesToManage = null;
            this.addEditAnnouncementView.Location = new System.Drawing.Point(0, 0);
            this.addEditAnnouncementView.Model = null;
            this.addEditAnnouncementView.Name = "addEditAnnouncementView";
            this.addEditAnnouncementView.Roles = null;
            this.addEditAnnouncementView.Size = new System.Drawing.Size(970, 304);
            this.addEditAnnouncementView.TabIndex = 1;
            // 
            // AddEditAnnouncementsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.addEditPanel);
            this.Controls.Add(this.allAnnouncementPanel);
            this.Name = "AddEditAnnouncementsView";
            this.Size = new System.Drawing.Size(975, 550);
            this.Load += new System.EventHandler(this.AddEditAnnouncementsView_Load);
            this.allAnnouncementPanel.ResumeLayout(false);
            this.addEditPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public AddEditAnnouncementsView()
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
        private IContainer components = null;
        private Panel allAnnouncementPanel;
        private Panel addEditPanel;
        private AllAnnouncementsView allAnnouncementsView;
        private AddEditAnnouncementView addEditAnnouncementView;
        private AnnouncementService i_AnnouncementService;
        private ICollection facilitiesToManage;
        #endregion

        #region Constants
        #endregion
	}
}

