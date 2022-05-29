using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.HistoricalAccountViews
{
	/// <summary>
	/// Summary description for HistoricalPatientAccountsView.
	/// </summary>
	public class HistoricalPatientAccountsView : ControlView
	{

        #region Events

        public event EventHandler AccountSelected;

        #endregion

        #region Event Handlers

        private void patientAccountsListView_GotFocus(object sender, EventArgs e)
        {
            bool rowSelected = false;
            foreach( ListViewItem item in patientAccountsListView.Items )
            {
                if( item.Selected )
                {
                    rowSelected = true;
                    break;
                }
            }
            if( !rowSelected && patientAccountsListView.Items.Count != 0 )
            {
                patientAccountsListView.Items[0].Selected = true;
            }
        }
        
        private void patientAccountsListView_DoubleClick(object sender, EventArgs e)
        {
            GetSelectedAccount();
        }

        private void patientAccountsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Enter )
            {
                GetSelectedAccount();
            }
        }

        private void patientAccountsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void patientAccountsListView_MouseMove(object sender, MouseEventArgs e)
        {
            bool mouseOutOfColumn   = true;
            bool mouseOutOfRow      = true;
            int currentColumn       = -1;
            int currentRow          = -1;
            int heightOfTitle       = 0;
            int headerWidth         = 0;
            int numberOfRows        = patientAccountsListView.Items.Count;

            if( numberOfRows == 0 )
            {
                return;
            }
            
            if( numberOfRows > 0 )
            {
                heightOfTitle = patientAccountsListView.Items[0].GetBounds( ItemBoundsPortion.Label ).Top;
            }

            foreach( ColumnHeader header in patientAccountsListView.Columns )
            {   
                currentColumn++;
                headerWidth = headerWidth + header.Width;
                if( headerWidth > e.X )
                {
                    mouseOutOfColumn = false;
                    break;
                }
            }

            if( mouseOutOfColumn )
            {
                return;
            }

            foreach( ListViewItem item in patientAccountsListView.Items )
            {
                currentRow++;
                heightOfTitle = heightOfTitle + item.Bounds.Height;

                if( heightOfTitle > e.Y )
                {
                    mouseOutOfRow = false;
                    break;
                }
            }

            if ( mouseOutOfRow ) 
            {
                return;
            }

            patientAccountsToolTip.SetToolTip( 
                patientAccountsListView, 
                patientAccountsListView.Items[currentRow].SubItems[currentColumn].Text );
        }

        #endregion

        #region Methods
        
        public override void UpdateView()
        {
            DisplayAccounts();
            base.UpdateView();
        }

        public void FocusOnGrid()
        {
            if( patientAccountsListView.Items.Count > 0 )
            {
                patientAccountsListView.Focus();
            }
        }

        public void GetSelectedAccount()
        {
            foreach( ListViewItem item in patientAccountsListView.Items )
            {
                if( item.Selected )
                {
                    AccountProxy acountProxy = 
                        patientAccountsListView.SelectedItems[0].Tag as AccountProxy;
                    if( AccountSelected != null )
                    {
                        AccountSelected( this, new LooseArgs( acountProxy ) );
                    }
                    break;
                }
            }
        } 

        public AccountProxy SelectedAccount
        {
            get
            {   
                if( patientAccountsListView.SelectedItems.Count > 0 ) 
                {
                    return patientAccountsListView.SelectedItems[0].Tag as AccountProxy;
                }
                return null;
            }
        }
        
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private void DisplayAccounts()
        {
            ArrayList accounts = (ArrayList)Model;
            patientAccountsListView.BeginUpdate( );
            patientAccountsListView.Items.Clear();
            if( accounts != null && accounts.Count > 0 )
            {
                foreach( AccountProxy accountProxy in accounts )
                {
                    ListViewItem item = new ListViewItem {Text = "", Tag = accountProxy};

                    item.SubItems.Add( accountProxy.ConfidentialFlag );
                    item.SubItems.Add( accountProxy.AccountNumber.ToString() );

                    item.SubItems.Add(accountProxy.KindOfVisit != null 
                                        ? accountProxy.KindOfVisit.ToCodedString() 
                                        : "");

                    item.SubItems.Add(accountProxy.HospitalService != null
                                          ? accountProxy.HospitalService.ToCodedString()
                                          : "");

                    item.SubItems.Add(accountProxy.AdmitDate != DateTime.MinValue
                                          ? accountProxy.AdmitDate.ToString("MM/dd/yyyy HH:mm")
                                          : String.Empty);

                    item.SubItems.Add(accountProxy.DischargeDate != DateTime.MinValue
                                          ? accountProxy.DischargeDate.ToString("MM/dd/yyyy")
                                          : String.Empty);

                    item.SubItems.Add(accountProxy.DischargeDisposition != null
                                          ? accountProxy.DischargeDisposition.ToCodedString()
                                          : "");

                    item.SubItems.Add( accountProxy.Patient.MedicalRecordNumber.ToString() );
                    
                    if( accountProxy.MultiSiteFlag != null && accountProxy.MultiSiteFlag.ToUpper() == "Y" )
                    {
                        item.SubItems.Add( accountProxy.SiteCode );
                    } 
                    else
                    {
                        item.SubItems.Add( "" );                       
                    }
                    patientAccountsListView.Items.Add( item );
                }
                patientAccountsToolTip.AutoPopDelay = 50;
                patientAccountsListView.Focus();
                DefaultSort();
                patientAccountsListView.Items[0].Selected = true;
                patientAccountsListView.EndUpdate( );
            }
            else
            {
                patientAccountsListView.TabStop = false;
            }
        }

        private ListViewSortManager PatientAccountsListViewSortManager()
        {
            // Array of IComparer for each row.
            Type[] CompareTypes = new[] 
                {
                    typeof( ListViewTextSort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewInt32Sort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewDateSort ),
                    typeof( ListViewDateSort ),
                    typeof( ListViewTextSort ),
                    typeof( ListViewInt32Sort ),
                    typeof( ListViewTextSort )                   
                };

            return new ListViewSortManager( patientAccountsListView, CompareTypes );
        }

        private void DefaultSort()
        {
            SortManager.Sort( 5, SortOrder.Ascending );
        }
        #endregion

        #region Private Properties
        
        private ListViewSortManager SortManager
        {
            get
            {
                return i_SortManager;
            }
            set
            {
                i_SortManager = value;
            }
        }

        #endregion

        #region Construction and Finalization

        public HistoricalPatientAccountsView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            SortManager = PatientAccountsListViewSortManager();
            EnableThemesOn( this );
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
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
            this.components = new System.ComponentModel.Container();
            this.patientAccountsListView = new System.Windows.Forms.ListView();
            this.chDummy = new System.Windows.Forms.ColumnHeader();
            this.chConfidential = new System.Windows.Forms.ColumnHeader();
            this.chAccount = new System.Windows.Forms.ColumnHeader();
            this.chPatientType = new System.Windows.Forms.ColumnHeader();
            this.chHospitalService = new System.Windows.Forms.ColumnHeader();
            this.chAdmitDateTime = new System.Windows.Forms.ColumnHeader();
            this.chDischargeDate = new System.Windows.Forms.ColumnHeader();
            this.chDischargeDisposition = new System.Windows.Forms.ColumnHeader();
            this.chOriginalMRN = new System.Windows.Forms.ColumnHeader();
            this.chSite = new System.Windows.Forms.ColumnHeader();
            this.patientAccountsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // patientAccountsListView
            // 
            this.patientAccountsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                                      this.chDummy,
                                                                                                      this.chConfidential,
                                                                                                      this.chAccount,
                                                                                                      this.chPatientType,
                                                                                                      this.chHospitalService,
                                                                                                      this.chAdmitDateTime,
                                                                                                      this.chDischargeDate,
                                                                                                      this.chDischargeDisposition,
                                                                                                      this.chOriginalMRN,
                                                                                                      this.chSite});
            this.patientAccountsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientAccountsListView.FullRowSelect = true;
            this.patientAccountsListView.GridLines = true;
            this.patientAccountsListView.HideSelection = false;
            this.patientAccountsListView.Location = new System.Drawing.Point(0, 0);
            this.patientAccountsListView.MultiSelect = false;
            this.patientAccountsListView.Name = "patientAccountsListView";
            this.patientAccountsListView.Size = new System.Drawing.Size(979, 457);
            this.patientAccountsListView.TabIndex = 0;
            this.patientAccountsListView.View = System.Windows.Forms.View.Details;
            this.patientAccountsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.patientAccountsListView_KeyDown);
            this.patientAccountsListView.GotFocus += new System.EventHandler(this.patientAccountsListView_GotFocus);
            this.patientAccountsListView.DoubleClick += new System.EventHandler(this.patientAccountsListView_DoubleClick);
            this.patientAccountsListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.patientAccountsListView_MouseMove);
            this.patientAccountsListView.SelectedIndexChanged += new System.EventHandler(this.patientAccountsListView_SelectedIndexChanged);
            // 
            // chDummy
            // 
            this.chDummy.Text = "";
            this.chDummy.Width = 0;
            // 
            // chConfidential
            // 
            this.chConfidential.Text = "Confid";
            this.chConfidential.Width = 65;
            // 
            // chAccount
            // 
            this.chAccount.Text = "Account";
            this.chAccount.Width = 82;
            // 
            // chPatientType
            // 
            this.chPatientType.Text = "PT";
            this.chPatientType.Width = 93;
            // 
            // chHospitalService
            // 
            this.chHospitalService.Text = "Hospital Service";
            this.chHospitalService.Width = 170;
            // 
            // chAdmitDateTime
            // 
            this.chAdmitDateTime.Text = "Admit Date/Time";
            this.chAdmitDateTime.Width = 122;
            // 
            // chDischargeDate
            // 
            this.chDischargeDate.Text = "Disch Date";
            this.chDischargeDate.Width = 94;
            // 
            // chDischargeDisposition
            // 
            this.chDischargeDisposition.Text = "Discharge Disposition";
            this.chDischargeDisposition.Width = 163;
            // 
            // chOriginalMRN
            // 
            this.chOriginalMRN.Text = "Original MRN";
            this.chOriginalMRN.Width = 104;
            // 
            // chSite
            // 
            this.chSite.Text = "Site";
            // 
            // patientAccountsToolTip
            // 
            this.patientAccountsToolTip.AutoPopDelay = 200;
            this.patientAccountsToolTip.InitialDelay = 200;
            this.patientAccountsToolTip.ReshowDelay = 200;
            // 
            // HistoricalPatientAccountsView
            // 
            this.Controls.Add(this.patientAccountsListView);
            this.Name = "HistoricalPatientAccountsView";
            this.Size = new System.Drawing.Size(979, 457);
            this.ResumeLayout(false);

        }

        #endregion

        #region Data Elements

        private ListView patientAccountsListView;
        private ColumnHeader chConfidential;
        private ColumnHeader chAccount;
        private ColumnHeader chPatientType;
        private ColumnHeader chHospitalService;
        private ColumnHeader chAdmitDateTime;
        private ColumnHeader chDischargeDate;
        private ColumnHeader chDischargeDisposition;
        private ColumnHeader chOriginalMRN;
        private ColumnHeader chSite;
        private ColumnHeader chDummy;
        private ToolTip patientAccountsToolTip;
        private IContainer components;
        private ListViewSortManager i_SortManager;

        #endregion

        #region Constants
        #endregion

    }
}
