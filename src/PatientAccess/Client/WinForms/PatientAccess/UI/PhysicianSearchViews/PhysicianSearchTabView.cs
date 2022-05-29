using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PhysicianSearchViews
{
    /// <summary>
    /// Summary description for PhysicianSearchTabView.
    /// </summary>
    [Serializable]
    public class PhysicianSearchTabView : ControlView
    {
        #region Events
        public event EventHandler ResetButtonClicked;
        public event EventHandler PhysicianFound;
        public event EventHandler PhysicianNotFound;
        public event EventHandler EnableOkButton;
        public event EventHandler SetCurrentTabPage;
        #endregion

        #region Event Handlers
        private void tcPhysicianSearch_SelectedIndexChanged( object sender, EventArgs e )
        {
            string tabPage;

            if ( tcPhysicianSearch.SelectedTab == tpRecordNonstaffPhysician )
            {
                tabPage = "NONSTAFF";

                recordNonStaffPhysicianView1.Model = Model;
                recordNonStaffPhysicianView1.CallingObject = CallingObject;
                recordNonStaffPhysicianView1.PhysicianRelationshipToView = PhysicianRelationshipToView;
                recordNonStaffPhysicianView1.UpdateView();
            }
            else
            {
                tabPage = string.Empty;
            }

            SetCurrentTabPage( this, new LooseArgs( tabPage ) );
        }

        private void recordNonStaffPhysicianView1_EnableOKButton( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            YesNoFlag yesNo = args.Context as YesNoFlag;
            EnableOkButton( this, new LooseArgs( yesNo ) );
        }

        private void ResetButton_Clicked( object sender, EventArgs e )
        {
            ResetButtonClicked( this, new LooseArgs( this ) );
        }

        private void Physician_Found( object sender, EventArgs e )
        {
            PhysicianFound( this, e );
        }

        private void Physician_NotFound( object sender, EventArgs e )
        {
            PhysicianNotFound( this, e );
        }
        #endregion

        #region Public Methods
        public override void UpdateView()
        {
            searchPhysicianBySpecialty1.UpdateView();
            recordNonStaffPhysicianView1.Model = Model;
            recordNonStaffPhysicianView1.CallingObject = CallingObject;
            recordNonStaffPhysicianView1.PhysicianRelationshipToView = PhysicianRelationshipToView;
            recordNonStaffPhysicianView1.UpdateView();
        }

        public Physician NonstaffPhysician()
        {
            return recordNonStaffPhysicianView1.NonstaffPhysician();
        }
        #endregion

        #region Public Properties
        public new Account Model
        {
            private get
            {
                return ( Account )base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public string CallingObject
        {
            private get
            {
                return i_CallingObject;
            }
            set
            {
                i_CallingObject = value;
            }
        }

        public string PhysicianRelationshipToView
        {
            private get
            {
                return i_PhysicianRelationshipToView;
            }
            set
            {
                i_PhysicianRelationshipToView = value;
            }
        }
        #endregion

        #region Private Methods

        protected override void WndProc( ref Message m )
        {
            const uint WM_NOTIFY = 0x004E;
            const uint TCN_FIRST = 0xFFFFFDDA;
            const uint TCN_SELCHANGING = TCN_FIRST - 2;

            base.WndProc( ref m );

            switch ( ( uint )m.Msg )
            {
                case WM_NOTIFY:
                    {

                        int idCtrl = ( int )m.WParam;

                        NMHDR nmh = new NMHDR();
                        nmh.hwndFrom = IntPtr.Zero;
                        nmh.idFrom = 0;
                        nmh.code = 0;

                        nmh = ( NMHDR )m.GetLParam( typeof( NMHDR ) );

                        if ( nmh.code == TCN_SELCHANGING )
                        {
                            bool rc = recordNonStaffPhysicianView1.RunRules();
                            int irc = 0;
                            if ( !rc )
                            {
                                irc = 1;
                            }

                            m.Result = ( IntPtr )irc;
                        }
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tcPhysicianSearch = new System.Windows.Forms.TabControl();
            this.tpSearchByName = new System.Windows.Forms.TabPage();
            this.searchPhysicianByNameView1 = new PatientAccess.UI.PhysicianSearchViews.SearchPhysicianByNameView();
            this.tpSearchBySpecialty = new System.Windows.Forms.TabPage();
            this.searchPhysicianBySpecialty1 = new PatientAccess.UI.PhysicianSearchViews.SearchPhysicianBySpecialty();
            this.tpRecordNonstaffPhysician = new System.Windows.Forms.TabPage();
            this.recordNonStaffPhysicianView1 = new PatientAccess.UI.PhysicianSearchViews.RecordNonStaffPhysicianView();
            this.tcPhysicianSearch.SuspendLayout();
            this.tpSearchByName.SuspendLayout();
            this.tpSearchBySpecialty.SuspendLayout();
            this.tpRecordNonstaffPhysician.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcPhysicianSearch
            // 
            this.tcPhysicianSearch.Controls.Add( this.tpSearchByName );
            this.tcPhysicianSearch.Controls.Add( this.tpSearchBySpecialty );
            this.tcPhysicianSearch.Controls.Add( this.tpRecordNonstaffPhysician );
            this.tcPhysicianSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcPhysicianSearch.Location = new System.Drawing.Point( 0, 0 );
            this.tcPhysicianSearch.Name = "tcPhysicianSearch";
            this.tcPhysicianSearch.SelectedIndex = 0;
            this.tcPhysicianSearch.Size = new System.Drawing.Size( 931, 449 );
            this.tcPhysicianSearch.TabIndex = 0;
            this.tcPhysicianSearch.TabStop = false;
            this.tcPhysicianSearch.SelectedIndexChanged += new System.EventHandler( this.tcPhysicianSearch_SelectedIndexChanged );
            // 
            // tpSearchByName
            // 
            this.tpSearchByName.BackColor = System.Drawing.Color.White;
            this.tpSearchByName.Controls.Add( this.searchPhysicianByNameView1 );
            this.tpSearchByName.Location = new System.Drawing.Point( 4, 22 );
            this.tpSearchByName.Name = "tpSearchByName";
            this.tpSearchByName.Size = new System.Drawing.Size( 923, 423 );
            this.tpSearchByName.TabIndex = 0;
            this.tpSearchByName.Text = "Search by Name";
            // 
            // searchPhysicianByNameView1
            // 
            this.searchPhysicianByNameView1.BackColor = System.Drawing.Color.White;
            this.searchPhysicianByNameView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchPhysicianByNameView1.Location = new System.Drawing.Point( 0, 0 );
            this.searchPhysicianByNameView1.Model = null;
            this.searchPhysicianByNameView1.Name = "searchPhysicianByNameView1";
            this.searchPhysicianByNameView1.Size = new System.Drawing.Size( 923, 423 );
            this.searchPhysicianByNameView1.TabIndex = 1;
            this.searchPhysicianByNameView1.PhysicianFound += new System.EventHandler( this.Physician_Found );
            this.searchPhysicianByNameView1.ResetButtonClicked += new System.EventHandler( this.ResetButton_Clicked );
            this.searchPhysicianByNameView1.PhysicianNotFound += new System.EventHandler( this.Physician_NotFound );
            // 
            // tpSearchBySpecialty
            // 
            this.tpSearchBySpecialty.BackColor = System.Drawing.Color.White;
            this.tpSearchBySpecialty.Controls.Add( this.searchPhysicianBySpecialty1 );
            this.tpSearchBySpecialty.Location = new System.Drawing.Point( 4, 22 );
            this.tpSearchBySpecialty.Name = "tpSearchBySpecialty";
            this.tpSearchBySpecialty.Size = new System.Drawing.Size( 923, 423 );
            this.tpSearchBySpecialty.TabIndex = 1;
            this.tpSearchBySpecialty.Text = "List by Specialty";
            // 
            // searchPhysicianBySpecialty1
            // 
            this.searchPhysicianBySpecialty1.BackColor = System.Drawing.Color.White;
            this.searchPhysicianBySpecialty1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchPhysicianBySpecialty1.Location = new System.Drawing.Point( 0, 0 );
            this.searchPhysicianBySpecialty1.Model = null;
            this.searchPhysicianBySpecialty1.Name = "searchPhysicianBySpecialty1";
            this.searchPhysicianBySpecialty1.Size = new System.Drawing.Size( 923, 423 );
            this.searchPhysicianBySpecialty1.TabIndex = 1;
            this.searchPhysicianBySpecialty1.PhysicianFound += new System.EventHandler( this.Physician_Found );
            this.searchPhysicianBySpecialty1.ResetButtonClicked += new System.EventHandler( this.ResetButton_Clicked );
            this.searchPhysicianBySpecialty1.PhysicianNotFound += new System.EventHandler( this.Physician_NotFound );
            // 
            // tpRecordNonstaffPhysician
            // 
            this.tpRecordNonstaffPhysician.BackColor = System.Drawing.Color.White;
            this.tpRecordNonstaffPhysician.Controls.Add( this.recordNonStaffPhysicianView1 );
            this.tpRecordNonstaffPhysician.Location = new System.Drawing.Point( 4, 22 );
            this.tpRecordNonstaffPhysician.Name = "tpRecordNonstaffPhysician";
            this.tpRecordNonstaffPhysician.Size = new System.Drawing.Size( 923, 423 );
            this.tpRecordNonstaffPhysician.TabIndex = 2;
            this.tpRecordNonstaffPhysician.Text = "Record Nonstaff Physician";
            // 
            // recordNonStaffPhysicianView1
            // 
            this.recordNonStaffPhysicianView1.BackColor = System.Drawing.Color.White;
            this.recordNonStaffPhysicianView1.CallingObject = null;
            this.recordNonStaffPhysicianView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recordNonStaffPhysicianView1.Location = new System.Drawing.Point( 0, 0 );
            this.recordNonStaffPhysicianView1.Model = null;
            this.recordNonStaffPhysicianView1.Name = "recordNonStaffPhysicianView1";
            this.recordNonStaffPhysicianView1.PhysicianRelationshipToView = null;
            this.recordNonStaffPhysicianView1.Size = new System.Drawing.Size( 923, 423 );
            this.recordNonStaffPhysicianView1.TabIndex = 1;
            this.recordNonStaffPhysicianView1.EnableOKButton += new System.EventHandler( this.recordNonStaffPhysicianView1_EnableOKButton );
            // 
            // PhysicianSearchTabView
            // 
            this.BackColor = System.Drawing.Color.FromArgb( ( ( System.Byte )( 209 ) ), ( ( System.Byte )( 228 ) ), ( ( System.Byte )( 243 ) ) );
            this.Controls.Add( this.tcPhysicianSearch );
            this.Name = "PhysicianSearchTabView";
            this.Size = new System.Drawing.Size( 931, 449 );
            this.tcPhysicianSearch.ResumeLayout( false );
            this.tpSearchByName.ResumeLayout( false );
            this.tpSearchBySpecialty.ResumeLayout( false );
            this.tpRecordNonstaffPhysician.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion
        #endregion
        
        #region Construction and Finalization
        public PhysicianSearchTabView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
            EnableThemesOn( this );
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        public TabControl tcPhysicianSearch;
        private TabPage tpSearchByName;
        private TabPage tpSearchBySpecialty;
        private TabPage tpRecordNonstaffPhysician;
        private SearchPhysicianByNameView searchPhysicianByNameView1;
        private SearchPhysicianBySpecialty searchPhysicianBySpecialty1;
        public RecordNonStaffPhysicianView recordNonStaffPhysicianView1;
        private string i_CallingObject;
        private string i_PhysicianRelationshipToView;
        #endregion

        #region Constants

        public const int NON_STAFF_PHYSICIAN_TAB = 2;

        #endregion
    }
}
