using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.WorklistViews
{
    /// <summary>
    /// Summary description for PreRegWorklistView.
    /// </summary>
    public class PreRegWorklistView : WorklistView
    {
        #region Events

        public new event EventHandler WorklistSelectedIndexEvent;
        public new event EventHandler WorklistEvent;
        public new event EventHandler AccountNameEvent;

        #endregion

        #region Event Handlers
       
        private void PreRegWorklistView_AccountNameEvent(object sender, EventArgs e)
        {           
            this.AccountNameEvent( this, e );
        }

        private void PreRegWorklistView_WorklistEvent(object sender, EventArgs e)
        {
            this.WorklistEvent( this, e );
        }

        private void PreRegWorklistView_WorklistSelectedIndexEvent(object sender, EventArgs e)
        {
            this.WorklistSelectedIndexEvent( this, e );
        }

        #endregion

        #region Methods

        /// <summary>
        /// PopulateWorklist - have the base populate the view
        /// </summary>
        public override void PopulateWorklist()
        {
            base.Worklist = WorklistBroker.PreRegWorklist();

            base.PopulateWorklist();          
        }

        public override void UpdateView()
        {
            FilterWorklistView.Model_WorklistSetting = WorklistBroker.PreRegWorklistSettings( GetUser().SecurityUser.TenetID ); 

            if( FilterWorklistView.Model_WorklistSetting == null )
            {
                FilterWorklistView.Model_WorklistSetting = new WorklistSettings();
                SetDefaultWorkllistSettings();
            }

            worklistRanges = WorklistBroker.GetPreRegWorklistRanges();

            if( worklistRanges != null && worklistRanges.Count > 0 )
            {
                FilterWorklistView.PeriodComboBox.Items.Clear();

                foreach( WorklistSelectionRange range in worklistRanges )
                {
                    FilterWorklistView.PeriodComboBox.Items.Add( range );
                }
            }
            
            FilterWorklistView.UpdateView();
            FilterWorklistView.Focus();

            PopulateWorklist();
        }

        public AccountProxy GetLatestAccountProxyForSelectedAccount()
        {
            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            int rowNum = base.DataGridBindingManager.Position;

            if (rowNum < 0)
            {
                return new AccountProxy();
            }

            ArrayList items = (ArrayList)base.DataGridWorklistAccounts.DataSource;
            WorklistItem wi = items[rowNum] as WorklistItem;

            long accountNumber = wi.AccountNumber;
            long medRecNo = wi.MedicalRecordNumber;

            AccountProxy aProxy = broker.AccountProxyFor(User.GetCurrent().Facility.Code, medRecNo, accountNumber);

            return aProxy;
        }

        #endregion

        #region Properties


        #endregion

        #region Private Methods
        
        protected override void ShowWorklistRequestEvent( object sender, EventArgs e )
        {
            WorklistBroker.SavePreRegWorklistSettings( base.GetUser().SecurityUser.TenetID, FilterWorklistView.Model_WorklistSetting );

            base.ShowWorklistRequestEvent(sender, e);
        }

        protected override void SetDefaultWorkllistSettings()
        {
            this.FilterWorklistView.Model_WorklistSetting = WorklistBroker.DefaultPreRegWorklistSettings();
          
            base.SetDefaultWorkllistSettings();
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PreRegWorklistView));
            this.toolTipName = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // toolTipName
            // 
            this.toolTipName.AutoPopDelay = 1000;
            this.toolTipName.InitialDelay = 1000;
            this.toolTipName.ReshowDelay  = 1000;
            this.toolTipName.ShowAlways   = true;
            // 
            // PreRegWorklistView
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Name = "PreRegWorklistView";
            this.Size = new System.Drawing.Size(1000, 300);
            this.ResumeLayout(false);
        }

        #endregion

        #endregion

        #region Construction and Finalization

        public PreRegWorklistView()
        {
            this.HeaderText = HEADER_TEXT;

            InitializeComponent();

            base.EnableThemesOn( this );
            base.AccountNameEvent +=new EventHandler(PreRegWorklistView_AccountNameEvent);
            base.WorklistEvent +=new EventHandler(PreRegWorklistView_WorklistEvent);
            base.WorklistSelectedIndexEvent +=new EventHandler(PreRegWorklistView_WorklistSelectedIndexEvent);           
        }

        public PreRegWorklistView( WorklistsView  parentForm )
        {
            // This call is required by the Windows.Forms Form Designer.
            
            InitializeComponent();

            base.EnableThemesOn( this );
            base.AccountNameEvent +=new EventHandler(PreRegWorklistView_AccountNameEvent);
            base.WorklistEvent +=new EventHandler(PreRegWorklistView_WorklistEvent);
            base.WorklistSelectedIndexEvent +=new EventHandler(PreRegWorklistView_WorklistSelectedIndexEvent);
            base.WorklistsView = parentForm;            
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

        #region Data Elements
        
        private IContainer            components;
        private ToolTip                toolTipName;
        private ArrayList                                   worklistRanges;

        #endregion

        #region Constants

        private const string HEADER_TEXT = "Preregistration Worklist";

        #endregion
    }
}
