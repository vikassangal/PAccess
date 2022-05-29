using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Reports;

namespace PatientAccess.UI.PhysicianSearchViews
{
	/// <summary>
	/// Summary description for SearchPhysicianByNameView.
	/// </summary>
	//TODO: Create XML summary comment for SearchPhysicianByNameView
	[Serializable]
	public class SearchPhysicianByNameView : ControlView
	{
		#region Events
//		public event EventHandler EnableDetailsButton; //not used
        public event EventHandler ResetButtonClicked;
        public event EventHandler PhysicianFound;
        public event EventHandler PhysicianNotFound;
		#endregion 

		#region Event Handlers
		private void searchForPhysicianResultsView1_PhysicianSelected(object sender, EventArgs e)
		{
			this.btnViewDetails.Enabled = true;

            this.PhysicianFound( this, e );
			/* fsw
			if( EnableDetailsButton != null )
			{

				//enablePhyNameButton = false;
				//EnableDetailsButton( this, null );
			}
			*/
			//SelectedPhysicians = Convert.ToInt64( ( ( LooseArgs )e ).Context.ToString() );
		}

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            Physician physician = (Physician)this.searchForPhysicianResultsView1.physicianResultsListView.SelectedItems[0].Tag;
            this.ShowDetails( physician.PhysicianNumber);
        }

		private void physicianSearchByNameView1_ResetView(object sender, EventArgs e)
		{
			this.searchForPhysicianResultsView1.ResetSearchResultsView();
            this.btnViewDetails.Enabled = false;
            this.ResetButtonClicked( this, new LooseArgs( this ) );
		}

		private void physicianSearchByNameView1_PhysiciansFound(object sender, EventArgs e)
		{
			this.searchForPhysicianResultsView1.Model = ( ArrayList )( ( LooseArgs)e).Context;
			this.searchForPhysicianResultsView1.UpdateView();
		}

		private void physicianSearchByNameView1_NoPhysiciansFound(object sender, EventArgs e)
		{
			
            this.searchForPhysicianResultsView1.Model = null;
            this.searchForPhysicianResultsView1.UpdateView();
            this.PhysicianNotFound( this, e );
		}

		private void physicianSearchByNameView1_ShowNoResultsLabel(object sender, EventArgs e)
		{

		}

	    private void physicianSearchByNameView1_DisableDetailsButton(object sender, EventArgs e)
		{
			this.btnViewDetails.Enabled = false;
		}
		#endregion

		#region Methods
		#endregion

		#region Properties
		#endregion

		#region Private Methods
        private void ShowDetails( long physicianNumber )
        {
            PhysicianDetailView physicianDetailView = new PhysicianDetailView();

            this.Cursor = Cursors.WaitCursor; 
            physicianDetailView.SelectPhysicians = physicianNumber;
			
			try
			{
				physicianDetailView.ShowDialog( this );
			}
			finally
			{
				this.Cursor = Cursors.Default;
				physicianDetailView.Dispose();
			}
        }

        private void searchForPhysicianResultsView1_NoPhysicianSelected(object sender, EventArgs e)
        {
            this.btnViewDetails.Enabled = false;
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.searchForPhysicianResultsView1 = new PatientAccess.UI.PhysicianSearchViews.SearchForPhysicianResultsView();
            this.physicianSearchByNameView1 = new PatientAccess.UI.Reports.PhysicianSearchByNameView();
            this.label3 = new System.Windows.Forms.Label();
            this.btnViewDetails = new LoggingButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(17, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(878, 39);
            this.label2.TabIndex = 0;
            this.label2.Text = "Select a physician and click OK.  If no appropriate result is found, try modifyin" +
                "g your search criteria above and search again.  Or click the Record Nonstaff Phy" +
                "sician tab to specify a physician who does not appear in the results.";
            // 
            // searchForPhysicianResultsView1
            // 
            this.searchForPhysicianResultsView1.Location = new System.Drawing.Point(10, 127);
            this.searchForPhysicianResultsView1.Model = null;
            this.searchForPhysicianResultsView1.Name = "searchForPhysicianResultsView1";
            this.searchForPhysicianResultsView1.Size = new System.Drawing.Size(897, 212);
            this.searchForPhysicianResultsView1.TabIndex = 2;
            this.searchForPhysicianResultsView1.NoPhysicianSelected += new System.EventHandler(this.searchForPhysicianResultsView1_NoPhysicianSelected);
            this.searchForPhysicianResultsView1.PhysicianSelected += new System.EventHandler(this.searchForPhysicianResultsView1_PhysicianSelected);
            // 
            // physicianSearchByNameView1
            // 
            this.physicianSearchByNameView1.BackColor = System.Drawing.Color.White;
            this.physicianSearchByNameView1.Location = new System.Drawing.Point(9, -1);
            this.physicianSearchByNameView1.Model = null;
            this.physicianSearchByNameView1.Name = "physicianSearchByNameView1";
            this.physicianSearchByNameView1.Size = new System.Drawing.Size(887, 62);
            this.physicianSearchByNameView1.TabIndex = 1;
            this.physicianSearchByNameView1.ResetView += new System.EventHandler(this.physicianSearchByNameView1_ResetView);
            this.physicianSearchByNameView1.Load += new System.EventHandler(this.physicianSearchByNameView1_Load);
            this.physicianSearchByNameView1.DisableDetailsButton += new System.EventHandler(this.physicianSearchByNameView1_DisableDetailsButton);
            this.physicianSearchByNameView1.ShowNoResultsLabel += new System.EventHandler(this.physicianSearchByNameView1_ShowNoResultsLabel);
            this.physicianSearchByNameView1.NoPhysiciansFound += new System.EventHandler(this.physicianSearchByNameView1_NoPhysiciansFound);
            this.physicianSearchByNameView1.PhysiciansFound += new System.EventHandler(this.physicianSearchByNameView1_PhysiciansFound);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Search Results";
            // 
            // btnViewDetails
            // 
            this.btnViewDetails.BackColor = System.Drawing.SystemColors.Control;
            this.btnViewDetails.Enabled = false;
            this.btnViewDetails.Location = new System.Drawing.Point(832, 339);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.TabIndex = 3;
            this.btnViewDetails.Text = "View Details";
            this.btnViewDetails.Click += new System.EventHandler(this.btnViewDetails_Click);
            // 
            // SearchPhysicianByNameView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnViewDetails);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.physicianSearchByNameView1);
            this.Controls.Add(this.searchForPhysicianResultsView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SearchPhysicianByNameView";
            this.Size = new System.Drawing.Size(910, 373);
            this.ResumeLayout(false);

        }
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public SearchPhysicianByNameView()
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
		private Label label2;
		private SearchForPhysicianResultsView searchForPhysicianResultsView1;
		private PhysicianSearchByNameView physicianSearchByNameView1;
		public bool enablePhyNameButton = true;
		private Label label3;
		private LoggingButton btnViewDetails;

        private void physicianSearchByNameView1_Load(object sender, EventArgs e)
        {
        
        }
//fsw		private bool isphysicianthere = false;
		#endregion

		#region Constants
		#endregion
	}
}
