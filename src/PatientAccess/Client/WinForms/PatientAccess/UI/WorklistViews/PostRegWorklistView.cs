using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;

namespace PatientAccess.UI.WorklistViews
{
    /// <summary>
    /// Summary description for PostRegWorklistView.
    /// </summary>
    public class PostRegWorklistView : WorklistView
    {
        #region Events

        public new event EventHandler WorklistSelectedIndexEvent;
        public new event EventHandler WorklistEvent;
        public new event EventHandler AccountNameEvent;

        #endregion

        #region Event Handlers

        private void PostRegWorklistView_AccountNameEvent(object sender, EventArgs e)
        {           
            this.AccountNameEvent( this, e );
        }

        private void PostRegWorklistView_WorklistEvent(object sender, EventArgs e)
        {
            this.WorklistEvent( this, e );
        }

        private void PostRegWorklistView_WorklistSelectedIndexEvent(object sender, EventArgs e)
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
            base.Worklist = WorklistBroker.PostRegWorklist();

            base.PopulateWorklist();          
        }

        public override void UpdateView()
        {
            FilterWorklistView.Model_WorklistSetting = WorklistBroker.PostRegWorklistSettings( GetUser().SecurityUser.TenetID ); 

            if( FilterWorklistView.Model_WorklistSetting == null )
            {
                FilterWorklistView.Model_WorklistSetting = new WorklistSettings();
                SetDefaultWorkllistSettings();
            }

            worklistRanges = WorklistBroker.GetPostRegWorklistRanges();

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

        #endregion

        #region Properties


        #endregion

        #region Private Methods
        
        protected override void ShowWorklistRequestEvent( object sender, EventArgs e )
        {
            WorklistBroker.SavePostRegWorklistSettings( base.GetUser().SecurityUser.TenetID, FilterWorklistView.Model_WorklistSetting );

            base.ShowWorklistRequestEvent(sender, e);
        }

        protected override void SetDefaultWorkllistSettings()
        {
            this.FilterWorklistView.Model_WorklistSetting = WorklistBroker.DefaultPostRegWorklistSettings();
          
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
            this.toolTipName = new System.Windows.Forms.ToolTip(this.components);
            // 
            // PostRegWorklistView
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Name = "PostRegWorklistView";
        }
        #endregion

        #endregion

        #region Construction and Finalization

        public PostRegWorklistView()
        {
            this.HeaderText = HEADER_TEXT;

            InitializeComponent();

            base.EnableThemesOn( this );
            base.AccountNameEvent +=new EventHandler(PostRegWorklistView_AccountNameEvent);
            base.WorklistEvent +=new EventHandler(PostRegWorklistView_WorklistEvent);
            base.WorklistSelectedIndexEvent +=new EventHandler(PostRegWorklistView_WorklistSelectedIndexEvent);           
        }

        public PostRegWorklistView( WorklistsView  parentForm )
        {
            // This call is required by the Windows.Forms Form Designer.
            
            InitializeComponent();

            base.EnableThemesOn( this );
            base.AccountNameEvent +=new EventHandler(PostRegWorklistView_AccountNameEvent);
            base.WorklistEvent +=new EventHandler(PostRegWorklistView_WorklistEvent);
            base.WorklistSelectedIndexEvent +=new EventHandler(PostRegWorklistView_WorklistSelectedIndexEvent);
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

        private const string                                HEADER_TEXT = "Postregistration Worklist";

        #endregion

    }
}


