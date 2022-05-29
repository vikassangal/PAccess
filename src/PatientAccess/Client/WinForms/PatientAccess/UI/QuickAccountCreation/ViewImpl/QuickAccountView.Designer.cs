using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.GuarantorViews;
using PatientAccess.UI.PreRegistrationViews;
using PatientAccess.UI.Registration;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    partial class QuickAccountView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                ViewFactory.Instance.CreateView<PatientAccessView>().Model = null;
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );

                c_singletonInstance = null;

                if ( components != null )
                {
                    components.Dispose();
                }

                // cancel the background workers here...
                CancelBackgroundWorkers();

                // SR 41094 - January 2008 Release 
                // Disabled all icons/menu options for previously scanned documents

                ViewFactory.Instance.CreateView<PatientAccessView>().DisablePreviousDocumentOptions();
            }
            //This is a temporary fix until the cause for tabcontrol 
            //ObjectDiposedException issue is figured out.
            try
            {
                base.Dispose( disposing );
            }
            catch ( ObjectDisposedException objDispEx )
            {
                c_log.Error( String.Format( "Message - {0}, StackTrace - {1}", objDispEx.Message,
                                          objDispEx.StackTrace ) );
            }
        }


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( QuickAccountView ) );
            this.userContextView = new PatientAccess.UI.UserContextView();
            this.patientContextView = new PatientAccess.UI.PatientContextView();

            this.paymentView = new Extensions.UI.Winforms.ControlView();
            this.controlViewForLiability = new Extensions.UI.Winforms.ControlView();
            this.copyPartyView = new PatientAccess.UI.GuarantorViews.CopyPartyView();

            this.quickAccountCreationView = new QuickAccountCreationView();
            this.registerConfirmationView1 = new PatientAccess.UI.Registration.RegisterConfirmationView();
            this.panelConfirmation = new System.Windows.Forms.Panel();
            this.tcViewTabPages = new System.Windows.Forms.TabControl();
            this.tpQuickAccountCreation = new System.Windows.Forms.TabPage();

            this.panelToDoList = new System.Windows.Forms.Panel();
            this.lvToDo = new System.Windows.Forms.ListView();
            this.chAction = new System.Windows.Forms.ColumnHeader();
            this.chCount = new System.Windows.Forms.ColumnHeader();
            this.chWorklist = new System.Windows.Forms.ColumnHeader();
            this.lblStaticToDoList = new System.Windows.Forms.Label();
            this.btnRefreshToDoList = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelPatientContext = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnFinish = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();

            this.btnCancel = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnNext = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnBack = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelUserContext = new System.Windows.Forms.Panel();
            this.panelConfirmation.SuspendLayout();
            this.tcViewTabPages.SuspendLayout();
            this.tpQuickAccountCreation.SuspendLayout();

            this.panelToDoList.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelUserContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView
            // 
            this.userContextView.BackColor = System.Drawing.Color.FromArgb( ( (System.Byte)( 94 ) ), ( (System.Byte)( 137 ) ),
                                                                           ( (System.Byte)( 185 ) ) );
            this.userContextView.Description = "Preregister Patient";
            this.userContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userContextView.Location = new System.Drawing.Point( 0, 0 );
            this.userContextView.Model = null;
            this.userContextView.Name = "userContextView";
            this.userContextView.Size = new System.Drawing.Size( 1024, 22 );
            this.userContextView.TabIndex = 0;
            this.userContextView.TabStop = false;
            // 
            // patientContextView
            // 
            this.patientContextView.Account = null;
            this.patientContextView.BackColor = System.Drawing.Color.White;
            this.patientContextView.DateOfBirthText = "";
            this.patientContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView.GenderLabelText = "";
            this.patientContextView.Location = new System.Drawing.Point( 0, 0 );
            this.patientContextView.Model = null;
            this.patientContextView.Name = "patientContextView";
            this.patientContextView.PatientNameText = "";
            this.patientContextView.Size = new System.Drawing.Size( 1003, 24 );
            this.patientContextView.SocialSecurityNumber = "";
            this.patientContextView.TabIndex = 0;
            this.patientContextView.TabStop = false;

            // 
            // paymentView
            // 
            this.paymentView.BackColor = System.Drawing.Color.White;
            this.paymentView.Location = new System.Drawing.Point( 0, 0 );
            this.paymentView.Model = null;
            this.paymentView.Name = "paymentView";
            this.paymentView.TabIndex = 0;
            // 
            // controlViewForLiability
            // 
            this.controlViewForLiability.Location = new System.Drawing.Point( 0, 0 );
            this.controlViewForLiability.Model = null;
            this.controlViewForLiability.Name = "controlViewForLiability";
            this.controlViewForLiability.TabIndex = 0;
            // 
            // copyPartyView
            // 
            this.copyPartyView.CoverageOrder =
                ( (PatientAccess.Domain.CoverageOrder)( resources.GetObject( "copyPartyView.CoverageOrder" ) ) );
            this.copyPartyView.KindOfTargetParty = null;
            this.copyPartyView.Location = new System.Drawing.Point( 0, 0 );
            this.copyPartyView.Model = null;
            this.copyPartyView.Name = "copyPartyView";
            this.copyPartyView.Size = new System.Drawing.Size( 178, 24 );
            this.copyPartyView.TabIndex = 0;


            // 
            // quickAccountCreationView
            // 
            this.quickAccountCreationView.BackColor = System.Drawing.Color.White;
            this.quickAccountCreationView.Location = new System.Drawing.Point( 0, 0 );
            this.quickAccountCreationView.Model = null;
            this.quickAccountCreationView.Name = "quickAccountCreationView";
            this.quickAccountCreationView.Size = new System.Drawing.Size( 1024, 378 );
            this.quickAccountCreationView.TabIndex = 2;
            this.quickAccountCreationView.RefreshTopPanel += new System.EventHandler( this.quickAccountCreationView_RefreshTopPanel );
            this.quickAccountCreationView.SetTabPageEvent += new System.EventHandler(this.SetTabPageEventHandler);

            // 
            // registerConfirmationView1
            // 
            this.registerConfirmationView1.BackColor = System.Drawing.Color.White;
            this.registerConfirmationView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.registerConfirmationView1.Location = new System.Drawing.Point( 0, 0 );
            this.registerConfirmationView1.Model = null;
            this.registerConfirmationView1.Name = "registerConfirmationView1";
            this.registerConfirmationView1.Size = new System.Drawing.Size( 1006, 526 );
            this.registerConfirmationView1.TabIndex = 0;
            this.registerConfirmationView1.RepeatActivity +=
                new System.EventHandler( this.registerConfirmationView1_RepeatActivity );
            this.registerConfirmationView1.CloseActivity +=
                new System.EventHandler( this.registerConfirmationView1_CloseActivity );
            this.registerConfirmationView1.EditAccount +=
                new System.EventHandler( this.registerConfirmationView1_EditAccount );
            //
            // panelConfirmation
            // 
            this.panelConfirmation.BackColor = System.Drawing.Color.White;
            this.panelConfirmation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelConfirmation.Controls.Add( this.registerConfirmationView1 );
            this.panelConfirmation.Location = new System.Drawing.Point( 8, 56 );
            this.panelConfirmation.Name = "panelConfirmation";
            this.panelConfirmation.Size = new System.Drawing.Size( 1008, 528 );
            this.panelConfirmation.TabIndex = 4;
            // 
            // tcViewTabPages
            // 
            this.tcViewTabPages.Controls.Add( this.tpQuickAccountCreation );

            this.tcViewTabPages.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                               System.Drawing.FontStyle.Regular,
                                                               System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.tcViewTabPages.Location = new System.Drawing.Point( 8, 176 );
            this.tcViewTabPages.Name = "tcViewTabPages";
            this.tcViewTabPages.SelectedIndex = 0;
            this.tcViewTabPages.Size = new System.Drawing.Size( 1008, 407 );
            this.tcViewTabPages.TabIndex = 2;
            this.tcViewTabPages.SelectedIndexChanged += new System.EventHandler( this.tcViewTabPages_SelectedIndexChanged );
            // 
            // tpQuickAccountCreation
            // 
            this.tpQuickAccountCreation.Controls.Add( this.quickAccountCreationView );
            this.tpQuickAccountCreation.Location = new System.Drawing.Point( 4, 22 );
            this.tpQuickAccountCreation.Name = "tpQuickAccountCreation";
            this.tpQuickAccountCreation.Size = new System.Drawing.Size( 1000, 381 );
            this.tpQuickAccountCreation.TabIndex = 10;
            this.tpQuickAccountCreation.Text = "Quick Account Creation";




            // panelToDoList
            // 
            this.panelToDoList.Controls.Add( this.lvToDo );
            this.panelToDoList.Controls.Add( this.lblStaticToDoList );
            this.panelToDoList.Controls.Add( this.btnRefreshToDoList );
            this.panelToDoList.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                              System.Drawing.FontStyle.Bold,
                                                              System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.panelToDoList.Location = new System.Drawing.Point( 8, 57 );
            this.panelToDoList.Name = "panelToDoList";
            this.panelToDoList.Size = new System.Drawing.Size( 1005, 108 );
            this.panelToDoList.TabIndex = 1;
            // 
            // lvToDo
            // 
            this.lvToDo.Columns.AddRange( new System.Windows.Forms.ColumnHeader[]
                                             {
                                                 this.chAction,
                                                 this.chCount,
                                                 this.chWorklist
                                             } );
            this.lvToDo.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular,
                                                       System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.lvToDo.FullRowSelect = true;
            this.lvToDo.GridLines = true;
            this.lvToDo.HideSelection = false;
            this.lvToDo.Location = new System.Drawing.Point( 0, 37 );
            this.lvToDo.MultiSelect = false;
            this.lvToDo.Name = "lvToDo";
            this.lvToDo.Size = new System.Drawing.Size( 1003, 66 );
            this.lvToDo.TabIndex = 2;
            this.lvToDo.View = System.Windows.Forms.View.Details;
            this.lvToDo.KeyDown += new System.Windows.Forms.KeyEventHandler( this.ToDoListView_KeyDown );
            this.lvToDo.DoubleClick += new System.EventHandler( this.lvToDo_DoubleClick );
            this.lvToDo.Enter += new System.EventHandler( this.ToDoListView_Enter );
            this.lvToDo.SelectedIndexChanged += new System.EventHandler( this.lvToDo_SelectedIndexChanged );
            // 
            // chAction
            // 
            this.chAction.Text = "Action Item";
            this.chAction.Width = 650;
            // 
            // chCount
            // 
            this.chCount.Text = "Count";
            this.chCount.Width = 75;
            // 
            // chWorklist
            // 
            this.chWorklist.Text = "Worklist";
            this.chWorklist.Width = 250;
            // 
            // lblStaticToDoList
            // 
            this.lblStaticToDoList.Location = new System.Drawing.Point( 0, 11 );
            this.lblStaticToDoList.Name = "lblStaticToDoList";
            this.lblStaticToDoList.Size = new System.Drawing.Size( 70, 23 );
            this.lblStaticToDoList.TabIndex = 1;
            this.lblStaticToDoList.Text = "To Do List";
            this.lblStaticToDoList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRefreshToDoList
            // 
            this.btnRefreshToDoList.BackColor = System.Drawing.SystemColors.Control;
            this.btnRefreshToDoList.Enabled = false;
            this.btnRefreshToDoList.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                                   System.Drawing.FontStyle.Regular,
                                                                   System.Drawing.GraphicsUnit.Point,
                                                                   ( (System.Byte)( 0 ) ) );
            this.btnRefreshToDoList.Location = new System.Drawing.Point( 928, 8 );
            this.btnRefreshToDoList.Name = "btnRefreshToDoList";
            this.btnRefreshToDoList.TabIndex = 1;
            this.btnRefreshToDoList.Text = "Re&fresh";
            this.btnRefreshToDoList.Click += new System.EventHandler( this.btnRefreshToDoList_Click );
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPatientContext.Controls.Add( this.patientContextView );
            this.panelPatientContext.Location = new System.Drawing.Point( 8, 29 );
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new System.Drawing.Size( 1005, 26 );
            this.panelPatientContext.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.btnFinish );

            this.panel1.Controls.Add( this.btnCancel );
            this.panel1.Controls.Add( this.btnNext );
            this.panel1.Controls.Add( this.btnBack );
            this.panel1.Location = new System.Drawing.Point( 0, 585 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 1024, 35 );
            this.panel1.TabIndex = 3;
            // 
            // btnFinish
            // 
            this.btnFinish.Anchor =
                ( (System.Windows.Forms.AnchorStyles)
                 ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnFinish.BackColor = System.Drawing.SystemColors.Control;
            this.btnFinish.Location = new System.Drawing.Point( 937, 6 );
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size( 75, 24 );
            this.btnFinish.TabIndex = 53;
            this.btnFinish.Text = "Fini&sh";
            this.btnFinish.Click += new System.EventHandler( this.btnFinish_Click );

            this.btnCancel.Anchor =
                ( (System.Windows.Forms.AnchorStyles)
                 ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point( 686, 6 );
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 24 );
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );

            // 
            // btnNext
            // 
            this.btnNext.Anchor =
                ( (System.Windows.Forms.AnchorStyles)
                 ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnNext.BackColor = System.Drawing.SystemColors.Control;
            this.btnNext.Location = new System.Drawing.Point( 851, 6 );
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size( 75, 24 );
            this.btnNext.TabIndex = 52;
            this.btnNext.Text = "&Next >";

            // 
            // btnBack
            // 
            this.btnBack.Anchor =
                ( (System.Windows.Forms.AnchorStyles)
                 ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnBack.BackColor = System.Drawing.SystemColors.Control;
            this.btnBack.Location = new System.Drawing.Point( 771, 6 );
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size( 75, 24 );
            this.btnBack.TabIndex = 51;
            this.btnBack.Text = "< &Back";


            // 
            // panelUserContext
            // 
            this.panelUserContext.BackColor = System.Drawing.SystemColors.Control;
            this.panelUserContext.Controls.Add( this.userContextView );
            this.panelUserContext.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUserContext.Location = new System.Drawing.Point( 0, 0 );
            this.panelUserContext.Name = "panelUserContext";
            this.panelUserContext.Size = new System.Drawing.Size( 1024, 22 );
            this.panelUserContext.TabIndex = 0;
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point( 12, 198 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size( 1000, 380 );
            this.progressPanel1.TabIndex = 3;
            // 
            // AccountView
            // 
            this.Load += new EventHandler( AccountView_Load );
            this.Leave += new EventHandler( AccountView_Leave );
            this.BackColor = System.Drawing.Color.FromArgb( ( (System.Byte)( 209 ) ), ( (System.Byte)( 228 ) ),
                                                           ( (System.Byte)( 243 ) ) );
            this.Controls.Add( this.panelUserContext );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.panelPatientContext );
            this.Controls.Add( this.panelToDoList );
            this.Controls.Add( this.tcViewTabPages );
            this.Controls.Add( this.panelConfirmation );
            this.Controls.Add( this.progressPanel1 );
            this.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular,
                                                System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.Name = "QuickAccountView";
            this.Size = new System.Drawing.Size( 1024, 620 );
            this.panelConfirmation.ResumeLayout( false );
            this.tcViewTabPages.ResumeLayout( false );
            this.tpQuickAccountCreation.ResumeLayout( false );


            this.panelToDoList.ResumeLayout( false );
            this.panelPatientContext.ResumeLayout( false );
            this.panel1.ResumeLayout( false );
            this.panelUserContext.ResumeLayout( false );
            this.ResumeLayout( false );
        }

        #endregion
       
        #region Data Elements

        private LoggingButton btnRefreshToDoList;
        private ClickOnceLoggingButton btnCancel;
        private ClickOnceLoggingButton btnFinish;
        private LoggingButton btnNext;
        private LoggingButton btnBack;
        private ColumnHeader chAction;
        private ColumnHeader chCount;
        private ColumnHeader chWorklist;
        private Label lblStaticToDoList;
        private ListView lvToDo;
        private Panel panelToDoList;
        private Panel panelPatientContext;
        private Panel panel1;
        private Panel panelUserContext;
        private Panel panelConfirmation;
        private TabControl tcViewTabPages;
        private TabPage tpQuickAccountCreation;
        private CopyPartyView copyPartyView;
        private QuickAccountCreationView quickAccountCreationView;
        private PatientContextView patientContextView;
        private UserContextView userContextView;
        private InvalidCodeFieldsDialog invalidCodeFieldsDialog;
        private InvalidCodeOptionalFieldsDialog invalidCodeOptionalFieldsDialog;
        private RegisterConfirmationView registerConfirmationView1;
        private MaintenanceCmdView maintenanceCmdView;
        private ControlView paymentView;
        private ProgressPanel progressPanel1;
        private IRequiredFieldsSummaryView RequiredFieldSummaryView = new RequiredFieldsSummaryView();
        private RequiredFieldsDialog requiredFieldsDialog;
        private ShowDuplicatePreRegAccountsDialog showDuplicatePreRegAccountsDialog;
        private ControlView controlViewForLiability;

        

        # endregion

    }
}
