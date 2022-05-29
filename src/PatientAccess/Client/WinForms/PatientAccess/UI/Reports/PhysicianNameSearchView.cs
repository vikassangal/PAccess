using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.Reports
{
	/// <summary>
	/// Summary description for PhysicianNameSearchView.
	/// </summary>
	public class PhysicianNameSearchView : ControlView
	{
        #region Events

        public event EventHandler EnableDetailsButton;
      
        #endregion 

        #region Event Handlers

        private void PhysicianSelected( object sender, EventArgs e )
        {
            if( EnableDetailsButton != null )
            {
                enablePhyNameButton = false;
                EnableDetailsButton( this, null );
            }
            SelectedPhysiciansID = Convert.ToInt64( ( ( LooseArgs )e ).Context.ToString() );
            
        }


	    private void DisableDetailsButton( object sender, EventArgs e )
        {
            enablePhyNameButton = true;
            if( EnableDetailsButton != null )
            {
                if( this.isphysicianthere == false )
                {
                    EnableDetailsButton( this, new LooseArgs( enablePhyNameButton ) );
                }
            }
        }

	    private void PhysiciansFound( object sender, EventArgs e )
        {
            this.physicianSearchResultsView.Model = null;
            this.physicianSearchResultsView.Model = ( ArrayList )( ( LooseArgs)e).Context;
            this.physicianSearchResultsView.Show();
            this.physicianSearchResultsView.UpdateView();
        }


	    private void ResetView( object sender, EventArgs e )
        {
            this.isphysicianthere = false;
            this.physicianSearchResultsView.ResetResultsView();
        }


	    private void NoPhysiciansFound( object sender, EventArgs e )
        {
            this.isphysicianthere = false;
            this.DisableDetailsButton( this , null );
            this.noResultsLabel.Hide();
            this.physicianSearchResultsView.NoPhysiciansFound();
        }

        private void physicianSearchByNameView_ShowNoResultsLabel(object sender, EventArgs e)
        {
            noResultsLabel.Show();
        }


        #endregion

        #region Construction and Finalization
        
        /// <summary>
        /// Constructor
        /// </summary>
		public PhysicianNameSearchView()
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
            this.searchViewPanel = new System.Windows.Forms.Panel();
            this.physicianSearchByNameView = new PatientAccess.UI.Reports.PhysicianSearchByNameView();
            this.resultsViewPanel = new System.Windows.Forms.Panel();
            this.physicianSearchResultsView = new PhysicianSearchResultsView();
            this.searchResultsLabel = new System.Windows.Forms.Label();
            this.noResultsLabel = new System.Windows.Forms.Label();
            this.searchViewPanel.SuspendLayout();
            this.resultsViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchViewPanel
            // 
            this.searchViewPanel.Controls.Add(this.physicianSearchByNameView);
            this.searchViewPanel.Location = new System.Drawing.Point(0, 0);
            this.searchViewPanel.Name = "searchViewPanel";
            this.searchViewPanel.Size = new System.Drawing.Size(920, 64);
            this.searchViewPanel.TabIndex = 0;
            this.searchViewPanel.TabStop = true;
            // 
            // physicianSearchByNameView
            // 
            this.physicianSearchByNameView.BackColor = System.Drawing.Color.White;
            this.physicianSearchByNameView.Location = new System.Drawing.Point(0, 0);
            this.physicianSearchByNameView.Model = null;
            this.physicianSearchByNameView.Name = "physicianSearchByNameView";
            this.physicianSearchByNameView.Size = new System.Drawing.Size(920, 64);
            this.physicianSearchByNameView.TabIndex = 1;
            this.physicianSearchByNameView.ResetView += new System.EventHandler(this.ResetView);
            this.physicianSearchByNameView.DisableDetailsButton += new System.EventHandler(this.DisableDetailsButton);
            this.physicianSearchByNameView.NoPhysiciansFound += new System.EventHandler(this.NoPhysiciansFound);
            this.physicianSearchByNameView.PhysiciansFound += new System.EventHandler(this.PhysiciansFound);
            this.physicianSearchByNameView.ShowNoResultsLabel += new EventHandler(physicianSearchByNameView_ShowNoResultsLabel);
            // 
            // resultsViewPanel
            // 
            this.resultsViewPanel.Controls.Add(this.physicianSearchResultsView);
            this.resultsViewPanel.Location = new System.Drawing.Point(8, 114);
            this.resultsViewPanel.Name = "resultsViewPanel";
            this.resultsViewPanel.TabIndex = 1;
            this.resultsViewPanel.Size = new System.Drawing.Size(916, 336);
            this.resultsViewPanel.TabStop = true;
            // 
            // physicianSearchResultsView
            // 
            this.physicianSearchResultsView.Location = new System.Drawing.Point(0, 0);
            this.physicianSearchResultsView.Model = null;
            this.physicianSearchResultsView.Name = "physicianSearchResultsView";
            this.physicianSearchResultsView.Size = new System.Drawing.Size(916, 336);
            this.physicianSearchResultsView.TabIndex = 2;
            this.physicianSearchResultsView.PhysicianSelected += new System.EventHandler(this.PhysicianSelected);

            // 
            // searchResultsLabel
            // 
            this.searchResultsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.searchResultsLabel.Location = new System.Drawing.Point(8, 74);
            this.searchResultsLabel.Name = "searchResultsLabel";
            this.searchResultsLabel.Size = new System.Drawing.Size(90, 16);
            this.searchResultsLabel.TabStop = false;
            this.searchResultsLabel.Text = "Search Results";
            // 
            // noResultsLabel
            // 
            this.noResultsLabel.AutoSize = true;
            this.noResultsLabel.Location = new System.Drawing.Point(8, 94);
            this.noResultsLabel.Name = "noResultsLabel";
            this.noResultsLabel.Size = new System.Drawing.Size(449, 16);
            this.noResultsLabel.TabStop = false;
            this.noResultsLabel.Text = "If no appropriate result is found, try modifying your search criteria above and search again.";
            // 
            // PhysicianNameSearchView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.noResultsLabel);
            this.Controls.Add(this.resultsViewPanel);
            this.Controls.Add(this.searchViewPanel);
            this.Controls.Add(this.searchResultsLabel);
            this.Name = "PhysicianNameSearchView";
            this.Size = new System.Drawing.Size(920, 432);
            this.searchViewPanel.ResumeLayout(false);
            this.resultsViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        #region Methods
        /// <summary>
        /// Set default focus.
        /// </summary>
        public void SetFocus()
        {
            if( physicianSearchByNameView.CanFocus )
            {
                physicianSearchByNameView.Focus();
            }
        }
        #endregion

        #region Properties

        public long SelectedPhysiciansID
        {
            get
            { 
                return i_SelectedPhysician;
            }
            set
            {
                i_SelectedPhysician = value;

            }
        }

        #endregion

        #region Data Elements

        private Panel searchViewPanel;
        private PhysicianSearchByNameView
                                                physicianSearchByNameView;
        private Panel resultsViewPanel;
        private PhysicianSearchResultsView 
                                                physicianSearchResultsView;
        private Container components = null;
        private Label searchResultsLabel;
        private Label noResultsLabel;
        private long i_SelectedPhysician;
        public bool enablePhyNameButton = true;
        private bool isphysicianthere = false;

        #endregion

        #region Constants
        #endregion

      }
}
