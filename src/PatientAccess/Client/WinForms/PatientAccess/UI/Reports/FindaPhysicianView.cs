using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.PhysicianSearchViews;

namespace PatientAccess.UI.Reports
{
	/// <summary>
	/// Summary description for FindaPhysicianView.
	/// </summary>
	public class FindaPhysicianView : TimeOutFormView
	{
        #region Event Handlers

        private void PhysicianInquiryView_Load( object sender, EventArgs e )
        {
            if( SelectedMenu == PHYSICIANS_SEARCH )
            {
                physicianInquiryTabControl.SelectedTab = srchByPhyNameTabPage;
            }
        }


        private void CloseButton_Click( object sender, EventArgs e )
        {
            this.Close();
        }


        private void DetailsButton_Click( object sender, EventArgs e )
        {

            if(  physicianNameSearchView.SelectedPhysiciansID >  0 && physicianInquiryTabControl.SelectedIndex == 0 )
            {
                this.SelectedPhysician =  physicianNameSearchView.SelectedPhysiciansID;
            }
            else if( physicianSpecialitySearchView.SelectedPhysiciansNumber > 0 && physicianInquiryTabControl.SelectedIndex == 1 )
            {
                this.SelectedPhysician = physicianSpecialitySearchView.SelectedPhysiciansNumber; 
            }
            PhysicianDetailView physicianDetail = new PhysicianDetailView();
            physicianDetail.SelectPhysicians = this.SelectedPhysician;

			try
			{
				physicianDetail.ShowDialog( this ); 
			}
			finally
			{
				physicianDetail.Dispose();
			}
            
        }


        private void  physicianNameSearchView_EnableDetailsButton( object sender, EventArgs e )
        {
            if(  !physicianNameSearchView.enablePhyNameButton  )
            {
                this.DetailsButton.Enabled = true;
            }
            else
            {
                this.DetailsButton.Enabled = false;
            }

        }

        private void physicianSpecialitySearchView_EnableDetailsButton( object sender, EventArgs e )
        {
            if( !physicianSpecialitySearchView.enablePhySpeciallityButton )
            {
                this.DetailsButton.Enabled = true;
            }
            else
            {
                this.DetailsButton.Enabled = false;
            }
        }

        private void physicianInquirySelectedIndexChanged( object sender, EventArgs e )
           {
                if( physicianInquiryTabControl.SelectedIndex == 0 )
                {
                    physicianNameSearchView.SetFocus();
                    if(  !physicianNameSearchView.enablePhyNameButton  )
                    {
                        this.DetailsButton.Enabled = true;
                    }
                    else
                    {
                        this.DetailsButton.Enabled = false;
                    }

                }
                else
                {
                    physicianSpecialitySearchView.SetFocus();
                    physicianSpecialitySearchView.Update_view();
                    if( !physicianSpecialitySearchView.enablePhySpeciallityButton )
                    {
                        this.DetailsButton.Enabled = true;
                    }
                    else
                    {
                        this.DetailsButton.Enabled = false;
                    }
                }
            }


        #endregion
        
        #region Construction and Finalization

        /// <summary>
        /// Constructor
        /// </summary>
		public FindaPhysicianView()
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
				if( components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


        #endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FindaPhysicianView));
            this.PhysicianInquiryPanel = new System.Windows.Forms.Panel();
            this.CloseButton = new LoggingButton();
            this.DetailsButton = new LoggingButton();
            this.physicianInquiryTabControl = new System.Windows.Forms.TabControl();
            this.srchByPhyNameTabPage = new System.Windows.Forms.TabPage();
            this.physicianNameSearchView = new PatientAccess.UI.Reports.PhysicianNameSearchView();
            this.listBySpecTabPage = new System.Windows.Forms.TabPage();
            this.physicianSpecialitySearchView = new PatientAccess.UI.Reports.PhysicianSpecialtySearchView();
            this.PhysicianInquiryPanel.SuspendLayout();
            this.physicianInquiryTabControl.SuspendLayout();
            this.srchByPhyNameTabPage.SuspendLayout();
            this.listBySpecTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // PhysicianInquiryPanel
            // 
            this.PhysicianInquiryPanel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.PhysicianInquiryPanel.Controls.Add(this.CloseButton);
            this.PhysicianInquiryPanel.Controls.Add(this.DetailsButton);
            this.PhysicianInquiryPanel.Controls.Add(this.physicianInquiryTabControl);
            this.PhysicianInquiryPanel.Location = new System.Drawing.Point(0, 0);
            this.PhysicianInquiryPanel.Name = "PhysicianInquiryPanel";
            this.PhysicianInquiryPanel.Size = new System.Drawing.Size(940, 545);
            this.PhysicianInquiryPanel.TabIndex = 0;
            this.PhysicianInquiryPanel.TabStop = true;
            // 
            // CloseButton
            // 
            this.CloseButton.BackColor = System.Drawing.SystemColors.Control;
            this.CloseButton.Location = new System.Drawing.Point(832, 472);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.TabIndex = 6;
            this.CloseButton.Text = "Clo&se";
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // DetailsButton
            // 
            this.DetailsButton.BackColor = System.Drawing.SystemColors.Control;
            this.DetailsButton.Enabled = false;
            this.DetailsButton.Location = new System.Drawing.Point(744, 472);
            this.DetailsButton.Name = "DetailsButton";
            this.DetailsButton.TabIndex = 5;
            this.DetailsButton.Text = "&Details...";
            this.DetailsButton.Click += new System.EventHandler(this.DetailsButton_Click);
            // 
            // physicianInquiryTabControl
            // 
            this.physicianInquiryTabControl.Controls.Add(this.srchByPhyNameTabPage);
            this.physicianInquiryTabControl.Controls.Add(this.listBySpecTabPage);
            this.physicianInquiryTabControl.Location = new System.Drawing.Point(7, 7);
            this.physicianInquiryTabControl.Name = "physicianInquiryTabControl";
            this.physicianInquiryTabControl.SelectedIndex = 0;
            this.physicianInquiryTabControl.Size = new System.Drawing.Size(920, 440);
            this.physicianInquiryTabControl.TabIndex = 7;
            this.physicianInquiryTabControl.TabStop = false;
            this.physicianInquiryTabControl.SelectedIndexChanged += new System.EventHandler(this.physicianInquirySelectedIndexChanged);
            // 
            // srchByPhyNameTabPage
            // 
            this.srchByPhyNameTabPage.Controls.Add(this.physicianNameSearchView);
            this.srchByPhyNameTabPage.Location = new System.Drawing.Point(4, 22);
            this.srchByPhyNameTabPage.Name = "srchByPhyNameTabPage";
            this.srchByPhyNameTabPage.Size = new System.Drawing.Size(912, 414);
            this.srchByPhyNameTabPage.TabIndex = 1;
            this.srchByPhyNameTabPage.Text = "Search by Name";
            // 
            // physicianNameSearchView
            // 
            this.physicianNameSearchView.BackColor = System.Drawing.Color.White;
            this.physicianNameSearchView.Location = new System.Drawing.Point(0, 0);
            this.physicianNameSearchView.Model = null;
            this.physicianNameSearchView.Name = "physicianNameSearchView";
            this.physicianNameSearchView.SelectedPhysiciansID = ((long)(0));
            this.physicianNameSearchView.Size = new System.Drawing.Size(912, 414);
            this.physicianNameSearchView.TabIndex = 2;
            this.physicianNameSearchView.EnableDetailsButton += new System.EventHandler(this.physicianNameSearchView_EnableDetailsButton);
            // 
            // listBySpecTabPage
            // 
            this.listBySpecTabPage.Controls.Add(this.physicianSpecialitySearchView);
            this.listBySpecTabPage.Location = new System.Drawing.Point(4, 22);
            this.listBySpecTabPage.Name = "listBySpecTabPage";
            this.listBySpecTabPage.Size = new System.Drawing.Size(912, 414);
            this.listBySpecTabPage.TabIndex = 3;
            this.listBySpecTabPage.Text = "List by Specialty";
            // 
            // physicianSpecialitySearchView
            // 
            this.physicianSpecialitySearchView.BackColor = System.Drawing.Color.White;
            this.physicianSpecialitySearchView.Location = new System.Drawing.Point(0, 0);
            this.physicianSpecialitySearchView.Model = null;
            this.physicianSpecialitySearchView.Name = "physicianSpecialitySearchView";
            this.physicianSpecialitySearchView.SelectedPhysiciansNumber = ((long)(0));
            this.physicianSpecialitySearchView.Size = new System.Drawing.Size(911, 414);
            this.physicianSpecialitySearchView.TabIndex = 4;
            this.physicianSpecialitySearchView.EnableDetailsButton += new System.EventHandler(this.physicianSpecialitySearchView_EnableDetailsButton);
            // 
            // FindaPhysicianView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.ClientSize = new System.Drawing.Size(934, 513);
            this.Controls.Add(this.PhysicianInquiryPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindaPhysicianView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Physician";
            this.Load += new System.EventHandler(this.PhysicianInquiryView_Load);
            this.PhysicianInquiryPanel.ResumeLayout(false);
            this.physicianInquiryTabControl.ResumeLayout(false);
            this.srchByPhyNameTabPage.ResumeLayout(false);
            this.listBySpecTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        #region Methods
        #endregion

        #region Properties

        public string SelectedMenu
        {
            private get
            { 
                return  i_SelectedMenu;
            }
            set
            {
                 i_SelectedMenu = value;

            }
        }

	    private long SelectedPhysician
        {
            get
            { 
                return i_selectedPhysician;
            }
            set
            {
                i_selectedPhysician = value;

            }
        }


        #endregion

        #region Private Methods
        #endregion

        #region Data Elements

        private Panel PhysicianInquiryPanel;
        private TabControl physicianInquiryTabControl;
        private TabPage srchByPhyNameTabPage;
        private TabPage listBySpecTabPage;
        private Container components = null;
        private PhysicianNameSearchView  physicianNameSearchView;
        private PhysicianSearchResultsView physicianResultsView
                                                                = new PhysicianSearchResultsView(); 
        private string  i_SelectedMenu;
        private long i_selectedPhysician;
        private LoggingButton DetailsButton;
        private LoggingButton CloseButton;
        private PhysicianSpecialtySearchView physicianSpecialitySearchView;
        
        #endregion       

        #region Constants

        public const string PHYSICIANS_SEARCH = "Physicians";

        #endregion

      
    }
}
