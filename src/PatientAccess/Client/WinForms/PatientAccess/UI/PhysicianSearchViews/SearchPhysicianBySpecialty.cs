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
	/// Summary description for SearchPhysicianBySpecialty.
	/// </summary>
	//TODO: Create XML summary comment for SearchPhysicianBySpecialty
	[Serializable]
	public class SearchPhysicianBySpecialty : ControlView
	{
		#region Events
		//public event EventHandler EnableDetailsButton;
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
			if( EnableDetailsButton != null && this.searchForPhysicianResultsView1.Model != null )
			{
				enablePhySpeciallityButton = false;
				EnableDetailsButton( this,null );
			}
			*/
			//SelectedPhysiciansNumber = Convert.ToInt64( ( ( LooseArgs )e ).Context.ToString() );
		}

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            Physician physician = (Physician)this.searchForPhysicianResultsView1.physicianResultsListView.SelectedItems[0].Tag;
            this.ShowDetails( physician.PhysicianNumber);
        }

		private void physicianSearchBySpecialtyView1_PhysiciansFound(object sender, EventArgs e)
		{
//			this.isphysicianthere = true;
//			this.searchForPhysicianResultsView1.Model = null;
			this.searchForPhysicianResultsView1.Model = ( ArrayList )( ( LooseArgs ) e ).Context;
			//this.searchForPhysicianResultsView1.Show();
			this.searchForPhysicianResultsView1.UpdateView();
		}

		private void physicianSearchBySpecialtyView1_NoPhysiciansFound(object sender, EventArgs e)
		{
//			this.isphysicianthere = false;
            this.searchForPhysicianResultsView1.Model = null;
	        this.searchForPhysicianResultsView1.UpdateView();
            this.PhysicianNotFound( this, e );
			//this.physicianSearchResultsView.NoPhysiciansFound();
		}

		private void physicianSearchBySpecialtyView1_DisableDetailsButton(object sender, EventArgs e)
		{
			this.btnViewDetails.Enabled = false;
			/* fsw
			enablePhySpeciallityButton = true;
			if( EnableDetailsButton != null )
			{
				if( this.isphysicianthere == false )
				{
					EnableDetailsButton( this,new LooseArgs( enablePhySpeciallityButton ) );
				}
			}
			*/
		}

		private void physicianSearchBySpecialtyView1_ResetView(object sender, EventArgs e)
		{
//			this.isphysicianthere = false;
			this.searchForPhysicianResultsView1.ResetSearchResultsView();
            this.ResetButtonClicked( this, new LooseArgs( this ) );
		}
		#endregion

		#region Methods
        public override void UpdateView()
        {
            physicianSearchBySpecialtyView1.LoadAllSpecialities();
            this.physicianSearchBySpecialtyView1.FillComboBox();               
        }


        private void searchForPhysicianResultsView1_NoPhysicianSelected(object sender, EventArgs e)
        {
            this.btnViewDetails.Enabled = false;
        }
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



		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.searchForPhysicianResultsView1 = new PatientAccess.UI.PhysicianSearchViews.SearchForPhysicianResultsView();
            this.label1 = new System.Windows.Forms.Label();
            this.physicianSearchBySpecialtyView1 = new PatientAccess.UI.Reports.PhysicianSearchBySpecialtyView();
            this.btnViewDetails = new LoggingButton();
            this.SuspendLayout();
            // 
            // searchForPhysicianResultsView1
            // 
            this.searchForPhysicianResultsView1.Location = new System.Drawing.Point(10, 118);
            this.searchForPhysicianResultsView1.Model = null;
            this.searchForPhysicianResultsView1.Name = "searchForPhysicianResultsView1";
            this.searchForPhysicianResultsView1.Size = new System.Drawing.Size(899, 215);
            this.searchForPhysicianResultsView1.TabIndex = 1;
            this.searchForPhysicianResultsView1.NoPhysicianSelected += new System.EventHandler(this.searchForPhysicianResultsView1_NoPhysicianSelected);
            this.searchForPhysicianResultsView1.PhysicianSelected += new System.EventHandler(this.searchForPhysicianResultsView1_PhysicianSelected);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(17, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(876, 34);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select a physician and click OK.  If no appropriate result is found, try searchin" +
                "g by name.  Or click the Record Nonstaff Physician tab to specify a physician wh" +
                "o does not appear in the results.";
            // 
            // physicianSearchBySpecialtyView1
            // 
            this.physicianSearchBySpecialtyView1.BackColor = System.Drawing.Color.White;
            this.physicianSearchBySpecialtyView1.Location = new System.Drawing.Point(8, 9);
            this.physicianSearchBySpecialtyView1.Model = null;
            this.physicianSearchBySpecialtyView1.Name = "physicianSearchBySpecialtyView1";
            this.physicianSearchBySpecialtyView1.Size = new System.Drawing.Size(809, 47);
            this.physicianSearchBySpecialtyView1.TabIndex = 0;
            this.physicianSearchBySpecialtyView1.ResetView += new System.EventHandler(this.physicianSearchBySpecialtyView1_ResetView);
            this.physicianSearchBySpecialtyView1.DisableDetailsButton += new System.EventHandler(this.physicianSearchBySpecialtyView1_DisableDetailsButton);
            this.physicianSearchBySpecialtyView1.NoPhysiciansFound += new System.EventHandler(this.physicianSearchBySpecialtyView1_NoPhysiciansFound);
            this.physicianSearchBySpecialtyView1.PhysiciansFound += new System.EventHandler(this.physicianSearchBySpecialtyView1_PhysiciansFound);
            // 
            // btnViewDetails
            // 
            this.btnViewDetails.BackColor = System.Drawing.SystemColors.Control;
            this.btnViewDetails.Enabled = false;
            this.btnViewDetails.Location = new System.Drawing.Point(832, 332);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Size = new System.Drawing.Size(77, 23);
            this.btnViewDetails.TabIndex = 2;
            this.btnViewDetails.Text = "View Details";
            this.btnViewDetails.Click += new System.EventHandler(this.btnViewDetails_Click);
            // 
            // SearchPhysicianBySpecialty
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnViewDetails);
            this.Controls.Add(this.physicianSearchBySpecialtyView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.searchForPhysicianResultsView1);
            this.Name = "SearchPhysicianBySpecialty";
            this.Size = new System.Drawing.Size(912, 422);
            this.ResumeLayout(false);

        }
		#endregion
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public SearchPhysicianBySpecialty()
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
		private SearchForPhysicianResultsView searchForPhysicianResultsView1;
		private Label label1;
		private PhysicianSearchBySpecialtyView physicianSearchBySpecialtyView1;
//		private bool isphysicianthere = false;
		private LoggingButton btnViewDetails;
		public bool enablePhySpeciallityButton = true;
		#endregion


		#region Constants
		#endregion
	}
}
