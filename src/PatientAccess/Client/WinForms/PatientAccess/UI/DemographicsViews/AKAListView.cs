using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.DemographicsViews
{
	/// <summary>
	/// Summary description for AKAListView.
	/// </summary>
	public class AKAListView : ControlView
	{
        #region Events
        public event EventHandler AKASelectedEvent;
        public event EventHandler AKADoubleClickEvent;

        #endregion

        #region Event Handlers
        private void AKAListView_Enter(object sender, EventArgs e)
        {
            GetSelectedAka();
        }

        private void listView_SelectedIndexChanged( object sender, EventArgs e )
        {
            GetSelectedAka();
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            AkaListDoubleClick();
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Enter )
            {
                AkaListDoubleClick();
            }
        }

        #endregion

        #region Methods
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

        public void FocusOnGrid()
        {
            if( this.listView.Items.Count > 0 )
            {
                this.listView.Focus();
            }
        }

        public override void UpdateView()
        {

            listView.Items.Clear();

            foreach( Name aliasName in  this.Model_Patient.Aliases )
            {
                if( aliasName != null )
                {
                    ListViewItem item = new ListViewItem();
                    item.Tag = aliasName;

                    item.Text = aliasName.AsFormattedName();
                    item.SubItems.Add( aliasName.EntryDate.Date.ToShortDateString() );
                    item.SubItems.Add( ShowAKA( aliasName.IsConfidential ) );

                    listView.Items.Add( item );
                }
            }
            
            if( listView.Items.Count > 0 )
            {
                ListView.ListViewItemCollection lvic = listView.Items;

                for( int index = 0; index < lvic.Count; index++ )
                {
                    ListViewItem lvi = lvic[ index ];
                    Name nm = lvi.Tag as Name;

                    if(this.SelectedAliasName == null)
                    {
                        listView.Items[0].Selected = true;
                        listView.Select();
                    }

                    if( nm.Equals(this.SelectedAliasName) )
                    {
                        listView.Items[ index ].Selected = true;
                        listView.Select();
                        break;
                    }
                }
            }

        } 

        
        #endregion

        #region Properties

	    private Patient Model_Patient
        {
            get
            {
                return ( Patient )this.Model;
            }
        }
        public Name SelectedAliasName
        {
            private get
            {
                return i_AliasName;
            }
            set
            {
                this.i_AliasName = value;
            }
        }
        #endregion

        #region Private Methods

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView = new System.Windows.Forms.ListView();
            this.columnHdrAKA = new System.Windows.Forms.ColumnHeader();
            this.columnHdrDateAkaAdded = new System.Windows.Forms.ColumnHeader();
            this.columnHdrShowAka = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                       this.columnHdrAKA,
                                                                                       this.columnHdrDateAkaAdded,
                                                                                       this.columnHdrShowAka});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(672, 168);
            this.listView.TabIndex = 1;
            this.listView.TabStop = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnHdrAKA
            // 
            this.columnHdrAKA.Text = "AKA";
            this.columnHdrAKA.Width = 351;
            // 
            // columnHdrDateAkaAdded
            // 
            this.columnHdrDateAkaAdded.Text = "Date AKA Added";
            this.columnHdrDateAkaAdded.Width = 163;
            // 
            // columnHdrShowAka
            // 
            this.columnHdrShowAka.Text = "Show AKA";
            this.columnHdrShowAka.Width = 138;
            // 
            // AKAListView
            // 
            this.Controls.Add(this.listView);
            this.Name = "AKAListView";
            this.Size = new System.Drawing.Size(672, 168);
            this.Enter += new System.EventHandler(this.AKAListView_Enter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            this.ResumeLayout(false);

        }
        #endregion

        private void GetSelectedAka()
        {
            if(AKASelectedEvent != null && this.listView.SelectedItems.Count > 0)
            {                
                Name  aName = this.listView.SelectedItems[0].Tag as Name;
                AKASelectedEvent( this, new LooseArgs( aName ) );
            }
        }

        private void AkaListDoubleClick()
        {
            if(AKADoubleClickEvent != null && this.listView.SelectedItems.Count > 0)
            {                
                Name  aName = this.listView.SelectedItems[0].Tag as Name;
                AKADoubleClickEvent( this, new LooseArgs( aName ) );
            }
        }

        private string ShowAKA(bool isConfidential)
        {
            //if isConfidential then we don't show AKA data => ShowAKA=NO
            if( isConfidential )
            {
                return NO; 
            }
            else
            {
                return YES;   
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public AKAListView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

        }		
        #endregion

        private ListView listView;
        private ColumnHeader columnHdrAKA;
        private ColumnHeader columnHdrDateAkaAdded;
        private ColumnHeader columnHdrShowAka;

        #region Data Elements
        private Container components = null;
        private Name i_AliasName = null;
        #endregion

        #region Constants
        private const string YES = "Y";
        private const string NO = "N";

        #endregion
	}
}
