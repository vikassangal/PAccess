using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.FindInsurancePlan
{
    /// <summary>
    /// Summary description for PayorBrokersView.
    /// </summary>
    public class PayorBrokersView : CoverControlView
    {
        #region Events
        public event EventHandler SelectedProviderChanged;
        public event EventHandler CloseFormOnEnterKeyEvent;
        #endregion

        #region Event Handlers
        private void CustomKeyEventHandler(object sender, EventArgs e)
        {   // Close dialog if ENTER key pressed if OK button is disabled.
            if( CloseFormOnEnterKeyEvent != null )
            {
                CloseFormOnEnterKeyEvent( this, e );
            }
        }

        private void PayorBrokersView_EnabledChanged(object sender, EventArgs e)
        {
            if( this.Enabled == false )
            {
                listView.Items.Clear();
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatedSelectedProvider();
        }
        #endregion

        #region Methods
        public void DisplayProviders( ICollection providers )
        {
            ShowCover = false;
            
            listView.Items.Clear();
            listView.BeginUpdate();

            if( providers != null )
            {
                
                foreach( AbstractProvider provider in  providers )
                {
                    ListViewItem item = new ListViewItem();
                    item.Tag = provider;
                    item.Text = String.Empty;
                    item.SubItems.Add( provider.Name );
                    item.SubItems.Add( provider.NumberOfActivePlans );
                    listView.Items.Add( item );
                }
                if( listView.Items.Count > 0 )
                {
                    listView.Items[0].Selected = true;
                }

                if( listView.Items.Count > 14 )
                {
                    this.columnHeaderNoOfPlans.Width = 137;
                }
                else
                {
                    this.columnHeaderNoOfPlans.Width = 154;
                }
            }


            listView.EndUpdate();
            UpdatedSelectedProvider();
        }

        public void Activate()
        {
            this.FindForm().ActiveControl = listView;
        }
        #endregion

        #region Properties
        public AbstractProvider SelectedProvider
        {
            get
            {
                return i_SelectedProvider;
            }
            set
            {
                if( value != i_SelectedProvider )
                {
                    i_SelectedProvider = value;
                    OnSelectedProviderChanged();
                }
            }
        }
        #endregion

        #region Private Methods
        protected virtual void OnSelectedProviderChanged() 
        {
            if( SelectedProviderChanged != null )
            {
                SelectedProviderChanged( this, EventArgs.Empty );
            }
        }

        private void UpdatedSelectedProvider()
        {
            if( listView.SelectedItems.Count > 0 )
            {
                SelectedProvider = (AbstractProvider)listView.SelectedItems[ 0 ].Tag;
            }
            else
            {
                SelectedProvider = null;
            }
        }
        #endregion

        #region Properties

        private new bool Enabled
        {
            get
            {
                return listView.Enabled;
            }
            set
            {
                listView.Enabled = value;
            }
        }

        public ListView ListView
        {
            get
            {
                return listView;
            }
        }
        #endregion

        #region Constructors and Finalization
        public PayorBrokersView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView = new PatientAccess.UI.InsuranceViews.FindInsurancePlan.ListViewKeyHdlr();
            this.columnHeaderDummy = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderPayorBroker = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderNoOfPlans = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderDummy,
            this.columnHeaderPayorBroker,
            this.columnHeaderNoOfPlans} );
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point( 0, 0 );
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size( 668, 219 );
            this.listView.TabIndex = 0;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler( this.listView_SelectedIndexChanged );
            this.listView.CustomKeyEvent += new System.EventHandler( this.CustomKeyEventHandler );
            // 
            // columnHeaderDummy
            // 
            this.columnHeaderDummy.Width = 0;
            // 
            // columnHeaderPayorBroker
            // 
            this.columnHeaderPayorBroker.Text = "Payor/Broker";
            this.columnHeaderPayorBroker.Width = 410;
            // 
            // columnHeaderNoOfPlans
            // 
            this.columnHeaderNoOfPlans.Text = "No. of Plans";
            this.columnHeaderNoOfPlans.Width = 254;
            // 
            // PayorBrokersView
            // 
            this.Controls.Add( this.listView );
            this.CoverPadding = 40;
            this.Name = "PayorBrokersView";
            this.ShowCover = true;
            this.Size = new System.Drawing.Size( 668, 219 );
            this.EnabledChanged += new System.EventHandler( this.PayorBrokersView_EnabledChanged );
            this.Controls.SetChildIndex( this.listView, 0 );
            this.ResumeLayout( false );

        }
        #endregion

        #region Data Elements
        private Container     components = null;
        private ColumnHeader   columnHeaderPayorBroker;
        private ColumnHeader   columnHeaderNoOfPlans;
        private ListViewKeyHdlr listView;
        private ColumnHeader   columnHeaderDummy;
        private AbstractProvider                    i_SelectedProvider;
        #endregion

        #region Constants
        #endregion
    }
}
