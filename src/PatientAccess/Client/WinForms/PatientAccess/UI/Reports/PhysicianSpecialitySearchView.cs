using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.Reports
{
	public class PhysicianSpecialtySearchView : ControlView
	{

        #region Events

        public event EventHandler EnableDetailsButton;

        #endregion

        #region Event Handlers

	    private void PhysiciansFound( object sender, EventArgs e )
        {
            this.isphysicianthere = true;
            this.physicianSearchResultsView.Model = null;
            this.physicianSearchResultsView.Model = ( ArrayList )( ( LooseArgs ) e ).Context;
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
            this.physicianSearchResultsView.NoPhysiciansFound();
        }

        private void PhysicianSelected( object sender, EventArgs e )
        {
            if( EnableDetailsButton != null && this.physicianSearchResultsView.Model != null )
            {
                enablePhySpeciallityButton = false;
                EnableDetailsButton( this,null );
            }
            SelectedPhysiciansNumber = Convert.ToInt64( ( ( LooseArgs )e ).Context.ToString() );
        }

	    private void DisableDetailsButton( object sender, EventArgs e )
        {
            enablePhySpeciallityButton = true;
            if( EnableDetailsButton != null )
            {
                if( this.isphysicianthere == false )
                {
                    EnableDetailsButton( this,new LooseArgs( enablePhySpeciallityButton ) );
                }
            }
        }

        #endregion

        #region Construction And Finalization
        
        /// <summary>
        /// Constructor
        /// </summary>
		public PhysicianSpecialtySearchView()
		{
			InitializeComponent();
            base.EnableThemesOn( this );
        }
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
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
            this.searchSpecialityPanel = new System.Windows.Forms.Panel();
            this.physicianSearchBySpecialityView = new PhysicianSearchBySpecialtyView();
            this.resultViewpanel = new System.Windows.Forms.Panel();
            this.physicianSearchResultsView = new PhysicianSearchResultsView();
            this.searchSpecialityPanel.SuspendLayout();
            this.resultViewpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchSpecialityPanel
            // 
            this.searchSpecialityPanel.Controls.Add( physicianSearchBySpecialityView );
            this.searchSpecialityPanel.Location = new System.Drawing.Point(0, 0);
            this.searchSpecialityPanel.Name = "searchSpecialityPanel";
            this.searchSpecialityPanel.TabIndex = 0;
            this.searchSpecialityPanel.Size = new System.Drawing.Size(920, 70);
            this.searchSpecialityPanel.TabStop = true;
            // 
            // physicianSearchBySpecialityView
            // 
            this.physicianSearchBySpecialityView.Location = new System.Drawing.Point(0, 0);
            this.physicianSearchBySpecialityView.Name = "physicianSearchBySpecialityView";
            this.physicianSearchBySpecialityView.Size = new System.Drawing.Size(920, 70);
            this.physicianSearchBySpecialityView.TabIndex = 0;
            this.physicianSearchBySpecialityView.PhysiciansFound += new EventHandler( this.PhysiciansFound );
            this.physicianSearchBySpecialityView.NoPhysiciansFound += new EventHandler( this.NoPhysiciansFound );
            this.physicianSearchBySpecialityView.ResetView += new EventHandler( this.ResetView );
            this.physicianSearchBySpecialityView.DisableDetailsButton += new EventHandler( this.DisableDetailsButton );
            // 
            // resultViewpanel
            // 
            this.resultViewpanel.Controls.Add( physicianSearchResultsView );
            this.resultViewpanel.Location = new System.Drawing.Point(8, 114);
            this.resultViewpanel.Name = "resultViewpanel";
            this.resultViewpanel.TabIndex = 1;
            this.resultViewpanel.Size = new System.Drawing.Size(904, 345);
            this.resultViewpanel.TabStop = true;
            // 
            // physicianSearchResultsView
            // 
            physicianSearchResultsView.Location = new System.Drawing.Point(0, 0);
            physicianSearchResultsView.Model = null;
            physicianSearchResultsView.Name = "physicianResultsView1";
            physicianSearchResultsView.Size = new System.Drawing.Size(904, 345);
            physicianSearchResultsView.TabIndex = 0;
            physicianSearchResultsView.PhysicianSelected += new EventHandler( this.PhysicianSelected );
            // 
            // PhysicianSpecialtySearchView
            // 
            this.Controls.Add(this.resultViewpanel);
            this.Controls.Add(this.searchSpecialityPanel);
            this.Name = "PhysicianSpecialityView";
            this.Size = new System.Drawing.Size(920, 432);
            this.searchSpecialityPanel.ResumeLayout(false);
            this.resultViewpanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.BackColor = System.Drawing.Color.White;

        }

		#endregion

        #region Methods
        /// <summary>
        /// Set default focus.
        /// </summary>
        public void SetFocus()
        {
            if( physicianSearchBySpecialityView.CanFocus )
            {
                physicianSearchBySpecialityView.Focus();
            }
        }

        public void Update_view()
        {
            physicianSearchBySpecialityView.FillComboBox();
        }
        #endregion

        #region Properties

        public long SelectedPhysiciansNumber
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

        #region Data Elements
        
        private Container components = null;
        private Panel searchSpecialityPanel;
        private Panel resultViewpanel;
        private PhysicianSearchBySpecialtyView 
                                                physicianSearchBySpecialityView;
        private PhysicianSearchResultsView 
                                                physicianSearchResultsView;
        private long i_selectedPhysician;
        public bool enablePhySpeciallityButton = true;
        private bool isphysicianthere = false;

        #endregion

        #region Constants
        #endregion
	}
}
