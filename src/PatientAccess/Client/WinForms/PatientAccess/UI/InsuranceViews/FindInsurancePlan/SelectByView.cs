using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.FindInsurancePlan
{
    /// <summary>
    /// Summary description for SelectByView.
    /// </summary>
    public class SelectByView : ControlView
    {
        #region Events

        public event EventHandler SelectedInsurancePlanChanged;
        public event EventHandler CloseFormEvent;
        public event EventHandler ListViewLostFocusEvent;

        #endregion

        #region Event Handlers

        void SelectByView_Leave( object sender, EventArgs e )
        {
            // if other payor broker specified, push to domain 

            if( this.SelectedInsurancePlan != null && !string.IsNullOrEmpty( this.OtherPayorBroker ) )
            {                
                SelectedInsurancePlan.Payor.Name = this.OtherPayorBroker;
            }
        }

        private void listView_LostFocus(object sender, EventArgs e)
        {
            if( ListViewLostFocusEvent != null )
            {
                ListViewLostFocusEvent( this, e );
            }
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyData.Equals( Keys.Enter ) )
            {
                CloseFormEvent( this, null );
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;
            SelectedPlanChanged();
        }

        private void SelectByView_Load(object sender, EventArgs e)
        {
            SearchText = "";
        }
        
        private void ListView_Enter(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;
            if( lv.Items.Count == 0 )
            {
                return;
            }
            ListView.SelectedListViewItemCollection collection = lv.SelectedItems;
            if( collection.Count == 0 )
            {
                lv.Items[0].Selected = true;
                lv.Refresh();
            }
        }

        #endregion

        #region Methods
        public virtual void Activate()
        {
        }

        public virtual void ActivateSearchField()
        {
        }
        #endregion

        #region Properties
        public string LineLabelCaption
        {
            set
            {
                lineLabel.Caption = value;
            }
        }

        public Account PatientAccount
        {
            get
            {
                return i_PatientAccount;
            }
            set
            {
                i_PatientAccount = value;
            }
        }

        public InsurancePlan SelectedInsurancePlan
        {
            get
            {
                return i_SelectedInsurancePlan;
            }
            set
            {
                if( value != i_SelectedInsurancePlan )
                {
                    i_SelectedInsurancePlan = value;
                    OnSelectedInsurancePlanChanged();
                }
            }
        }

        public virtual bool PlanCanBeApplied
        {
            get
            {
                return false;
            }
        }

        public virtual string SearchText
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public string OtherPayorBroker
        {
            private get
            {
                return i_OtherPayorBroker;
            }
            set
            {
                i_OtherPayorBroker = value;
            }
        }

        #endregion

        #region Private Methods

        protected virtual void PopulateInsurancePlans( ICollection plans )
        {
            DisplayPlans( plans );
        }

        protected virtual void ResetControls()
        {
            PopulateInsurancePlans( null );
        }

        private void DisplayPlans( ICollection insurancePlans )
        {
            listView.BeginUpdate();            

            if( insurancePlans != null )
            {
                //( (ArrayList)insurancePlans ).Sort();

                listView.Items.Clear();
                ListViewItem item;
                foreach( InsurancePlan insurancePlan in insurancePlans )
                {
                    item = new ListViewItem();

                    item.Tag = insurancePlan;

                    item.Text = "";
                    if( !insurancePlan.IsValidPlanForAdmitDate( PatientAccount.AdmitDate ) )
                    {
                        item.ImageIndex = 0;
                        item.ToolTipText = UIErrorMessages.PLAN_EXPIRED_WARNING_MESSAGE;
                    }
                
                    item.SubItems.Add( insurancePlan.PlanID );
                    item.SubItems.Add( insurancePlan.PlanName );
                    if( insurancePlan.PlanType != null ) // 1769 fix
                    {
                        item.SubItems.Add( insurancePlan.PlanType.ToString() );
                    }
                    item.SubItems.Add( insurancePlan.LineOfBusiness );
                    if( insurancePlan.EffectiveOn != DateTime.MinValue )
                    {
                        item.SubItems.Add( insurancePlan.EffectiveOn.ToShortDateString() );
                    }
                    else
                    {
                        item.SubItems.Add( String.Empty );
                    }

                    if( insurancePlan.ApprovedOn != DateTime.MinValue )
                    {
                        item.SubItems.Add( insurancePlan.ApprovedOn.ToShortDateString() );
                    }
                    else
                    {
                        item.SubItems.Add( String.Empty );
                    }

                    if( insurancePlan.TerminatedOn != DateTime.MinValue )
                    {
                        item.SubItems.Add( insurancePlan.TerminatedOn.ToShortDateString() );
                    }
                    else
                    {
                        item.SubItems.Add( String.Empty );
                    }

                    if( insurancePlan.CanceledOn != DateTime.MinValue )
                    {
                        item.SubItems.Add( insurancePlan.CanceledOn.ToShortDateString() );
                    }
                    else
                    {
                        item.SubItems.Add( String.Empty );
                    }
                    if( insurancePlan.IsValidPlanForAdmitDate( PatientAccount.AdmitDate ) )
                    {
                        item.SubItems.Add( ICON_EXIST );
                    }
                    else
                    {
                        item.SubItems.Add( ICON_DOES_NOT_EXIST );
                    }
                    listView.Items.Add( item );
                }
            }

            SelectedPlanChanged();

            listView.EndUpdate();

            columnHeaderName.Width = listView.ClientSize.Width 
                - columnHeaderID.Width
                - columnHeaderType.Width
                - columnHeaderLOB.Width
                - columnHeaderAppDate.Width
                - columnHeaderCanDate.Width
                - columnHeaderEffDate.Width
                - columnHeaderExp.Width
                - columnHeaderTerDate.Width;
        }

        private void SelectedPlanChanged()
        {
            if( listView.SelectedItems.Count > 0 )
            {
                InsurancePlan aPlan = (InsurancePlan)listView.SelectedItems[ 0 ].Tag;
                SelectedInsurancePlan = this.InsuranceBroker.PlanWith( aPlan.PlanID, aPlan.EffectiveOn, aPlan.ApprovedOn, User.GetCurrent().Facility.Oid );
            }
            else
            {
                SelectedInsurancePlan = null;
            }
        }

        protected virtual void OnSelectedInsurancePlanChanged() 
        {
            if( SelectedInsurancePlanChanged != null )
            {
                SelectedInsurancePlanChanged( this, EventArgs.Empty );
            }
        }

        #endregion

        #region Private Properties

        protected IInsuranceBroker InsuranceBroker
        {
            get
            {
                if( i_InsuranceBroker == null )
                {
                    i_InsuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
                }
                return i_InsuranceBroker;
            }
            set
            {
                i_InsuranceBroker = value;
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( SelectByView ) );
            this.panelPlansMaster = new System.Windows.Forms.Panel();
            this.panelPlansDetail = new System.Windows.Forms.Panel();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeaderExp = new System.Windows.Forms.ColumnHeader( "(none)" );
            this.columnHeaderID = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderType = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderLOB = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderEffDate = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderAppDate = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTerDate = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderCanDate = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDisplayToolTip = new System.Windows.Forms.ColumnHeader();
            this.imageList = new System.Windows.Forms.ImageList( this.components );
            this.lineLabelPanel = new System.Windows.Forms.Panel();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.panelPlansMaster.SuspendLayout();
            this.panelPlansDetail.SuspendLayout();
            this.lineLabelPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPlansMaster
            // 
            this.panelPlansMaster.Controls.Add( this.panelPlansDetail );
            this.panelPlansMaster.Controls.Add( this.lineLabelPanel );
            this.panelPlansMaster.Location = new System.Drawing.Point( 0, 350 );
            this.panelPlansMaster.Name = "panelPlansMaster";
            this.panelPlansMaster.Size = new System.Drawing.Size( 684, 125 );
            this.panelPlansMaster.TabIndex = 8;
            // 
            // panelPlansDetail
            // 
            this.panelPlansDetail.Controls.Add( this.listView );
            this.panelPlansDetail.Location = new System.Drawing.Point( 8, 23 );
            this.panelPlansDetail.Name = "panelPlansDetail";
            this.panelPlansDetail.Size = new System.Drawing.Size( 668, 102 );
            this.panelPlansDetail.TabIndex = 1;
            // 
            // listView
            // 
            this.listView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderExp,
            this.columnHeaderID,
            this.columnHeaderName,
            this.columnHeaderType,
            this.columnHeaderLOB,
            this.columnHeaderEffDate,
            this.columnHeaderAppDate,
            this.columnHeaderTerDate,
            this.columnHeaderCanDate,
            this.columnHeaderDisplayToolTip} );
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point( 0, 0 );
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new System.Drawing.Size( 668, 102 );
            this.listView.SmallImageList = this.imageList;
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.Enter += new System.EventHandler( this.ListView_Enter );
            this.listView.SelectedIndexChanged += new System.EventHandler( this.listView_SelectedIndexChanged );
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler( this.listView_KeyDown );
            this.listView.LostFocus += new System.EventHandler( this.listView_LostFocus );
            // 
            // columnHeaderExp
            // 
            this.columnHeaderExp.Text = "Exp";
            this.columnHeaderExp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderExp.Width = 35;
            // 
            // columnHeaderID
            // 
            this.columnHeaderID.Text = "ID";
            this.columnHeaderID.Width = 53;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 175;
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            this.columnHeaderType.Width = 57;
            // 
            // columnHeaderLOB
            // 
            this.columnHeaderLOB.Text = "LOB";
            this.columnHeaderLOB.Width = 72;
            // 
            // columnHeaderEffDate
            // 
            this.columnHeaderEffDate.Text = "Eff Date";
            this.columnHeaderEffDate.Width = 69;
            // 
            // columnHeaderAppDate
            // 
            this.columnHeaderAppDate.Text = "App Date";
            this.columnHeaderAppDate.Width = 71;
            // 
            // columnHeaderTerDate
            // 
            this.columnHeaderTerDate.Text = "Ter Date";
            this.columnHeaderTerDate.Width = 73;
            // 
            // columnHeaderCanDate
            // 
            this.columnHeaderCanDate.Text = "Can Date";
            this.columnHeaderCanDate.Width = 68;
            // 
            // columnHeaderDisplayToolTip
            // 
            this.columnHeaderDisplayToolTip.Width = 0;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ( (System.Windows.Forms.ImageListStreamer)( resources.GetObject( "imageList.ImageStream" ) ) );
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName( 0, "ExpiredInsurancePlan.bmp" );
            // 
            // lineLabelPanel
            // 
            this.lineLabelPanel.Controls.Add( this.lineLabel );
            this.lineLabelPanel.Location = new System.Drawing.Point( 8, 0 );
            this.lineLabelPanel.Name = "lineLabelPanel";
            this.lineLabelPanel.Size = new System.Drawing.Size( 668, 23 );
            this.lineLabelPanel.TabIndex = 0;
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = "label1";
            this.lineLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lineLabel.Location = new System.Drawing.Point( 0, 0 );
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size( 668, 23 );
            this.lineLabel.TabIndex = 0;
            this.lineLabel.TabStop = false;
            // 
            // SelectByView
            // 
            this.Controls.Add( this.panelPlansMaster );
            this.Name = "SelectByView";
            this.Size = new System.Drawing.Size( 684, 481 );
            this.Load += new System.EventHandler( this.SelectByView_Load );
            this.panelPlansMaster.ResumeLayout( false );
            this.panelPlansDetail.ResumeLayout( false );
            this.lineLabelPanel.ResumeLayout( false );
            this.Leave += new EventHandler( SelectByView_Leave );
            this.ResumeLayout( false );

        }


        #endregion

        #region Constructors and Finalization
        public SelectByView()
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

        #region Data Elements

        private string                                      i_OtherPayorBroker = string.Empty;

        private IContainer                                  components;

        private Panel                  lineLabelPanel;
        private Panel                  panelPlansMaster;
        private Panel                  panelPlansDetail;

        protected ListView listView;
        private ColumnHeader           columnHeaderID;
        private ColumnHeader           columnHeaderName;
        private ColumnHeader           columnHeaderType;
        private ColumnHeader           columnHeaderLOB;

        private LineLabel lineLabel;

        private IInsuranceBroker                            i_InsuranceBroker;
        private InsurancePlan                               i_SelectedInsurancePlan;
        private ColumnHeader columnHeaderEffDate;
        private ColumnHeader columnHeaderAppDate;
        private ColumnHeader columnHeaderTerDate;
        private ColumnHeader columnHeaderCanDate;
        private ColumnHeader columnHeaderExp;
        private ColumnHeader columnHeaderDisplayToolTip;
        private ImageList imageList;
        private Account                                     i_PatientAccount;
        #endregion

        #region Constants
        private const string ICON_EXIST = "1";
        private const string ICON_DOES_NOT_EXIST = "0";
        #endregion
    }
}
