using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.AnnouncementViews
{
	public class AnnouncementDetailsView : ControlView
	{

        #region Event Handlers
        #endregion

        #region Methods
        public override void UpdateView()
        {
            this.announcementDetailTextBox.Text = 
                String.Format( "Author:      {0} \r\nAnnouncement: {1}",
                        this.Model.Author, this.Model.Description );
        }
        #endregion

        #region Properties
        public new Announcement Model
        {
            private get
            {
                return base.Model as Announcement;
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
            this.announcementDetailTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // announcementDetailTextBox
            // 
            UIColors.SetNormalBgColor( announcementDetailTextBox );
            this.announcementDetailTextBox.Location = new System.Drawing.Point(0, 0);
            this.announcementDetailTextBox.Multiline = true;
            this.announcementDetailTextBox.Name = "announcementDetailTextBox";
            this.announcementDetailTextBox.ReadOnly = true;
            this.announcementDetailTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.announcementDetailTextBox.Size = new System.Drawing.Size(955, 195);
            this.announcementDetailTextBox.TabIndex = 0;
            this.announcementDetailTextBox.Text = "";
            // 
            // AnnouncementDetailsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.announcementDetailTextBox);
            this.Name = "AnnouncementDetailsView";
            this.Size = new System.Drawing.Size(955, 195);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public AnnouncementDetailsView()
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
        private TextBox announcementDetailTextBox;
        private IContainer components = null;
        #endregion

        #region Constants
        #endregion

	}
}

