using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.GuarantorViews;
using PatientAccess.UI.PreRegistrationViews; 
using PatientAccess.UI.Registration;

namespace PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl
{
    partial class PAIWalkinAccountView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ViewFactory.Instance.CreateView<PatientAccessView>().Model = null;
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions(false);

                c_singletonInstance = null;

                if (components != null)
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
                base.Dispose(disposing);
            }
            catch (ObjectDisposedException objDispEx)
            {
                c_log.Error(String.Format("Message - {0}, StackTrace - {1}", objDispEx.Message,
                    objDispEx.StackTrace));
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
            this.userContextView = new PatientAccess.UI.UserContextView();
            this.patientContextView = new PatientAccess.UI.PatientContextView();
            this.paymentView = new Extensions.UI.Winforms.ControlView();
            this.controlViewForLiability = new Extensions.UI.Winforms.ControlView();
            this.copyPartyView = new PatientAccess.UI.GuarantorViews.CopyPartyView();
            this.paiWalkinAccountCreationView = new PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl.PAIWalkinAccountCreationView();
            this.panelConfirmation = new System.Windows.Forms.Panel();
            this.tcViewTabPages = new System.Windows.Forms.TabControl();
            this.tpPAIWalkinAccountCreation = new System.Windows.Forms.TabPage();
            this.panelToDoList = new System.Windows.Forms.Panel();
            this.lvToDo = new System.Windows.Forms.ListView();
            this.chAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chWorklist = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblStaticToDoList = new System.Windows.Forms.Label();
            this.btnRefreshToDoList = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelPatientContext = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnActivateShortRegistration = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnActivateStandardRegistration = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnCancel = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.panelUserContext = new System.Windows.Forms.Panel();
            this.tcViewTabPages.SuspendLayout();
            this.tpPAIWalkinAccountCreation.SuspendLayout();
            this.panelToDoList.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelUserContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(12, 198);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(1000, 380);
            this.progressPanel1.TabIndex = 3;
            // 
            // userContextView
            // 
            this.userContextView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(137)))), ((int)(((byte)(185)))));
            this.userContextView.Description = "Preregister Patient";
            this.userContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userContextView.Location = new System.Drawing.Point(0, 0);
            this.userContextView.Model = null;
            this.userContextView.Name = "userContextView";
            this.userContextView.Size = new System.Drawing.Size(1024, 22);
            this.userContextView.TabIndex = 0;
            this.userContextView.TabStop = false;
            // 
            // patientContextView
            // 
            this.patientContextView.BackColor = System.Drawing.Color.White;
            this.patientContextView.DateOfBirthText = "";
            this.patientContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView.GenderLabelText = "";
            this.patientContextView.Location = new System.Drawing.Point(0, 0);
            this.patientContextView.Model = null;
            this.patientContextView.Name = "patientContextView";
            this.patientContextView.PatientNameText = "";
            this.patientContextView.Size = new System.Drawing.Size(1003, 24);
            this.patientContextView.SocialSecurityNumber = "";
            this.patientContextView.TabIndex = 0;
            this.patientContextView.TabStop = false;
            // 
            // paymentView
            // 
            this.paymentView.BackColor = System.Drawing.Color.White;
            this.paymentView.Location = new System.Drawing.Point(0, 0);
            this.paymentView.Model = null;
            this.paymentView.Name = "paymentView";
            this.paymentView.Size = new System.Drawing.Size(150, 150);
            this.paymentView.TabIndex = 0;
            // 
            // controlViewForLiability
            // 
            this.controlViewForLiability.Location = new System.Drawing.Point(0, 0);
            this.controlViewForLiability.Model = null;
            this.controlViewForLiability.Name = "controlViewForLiability";
            this.controlViewForLiability.Size = new System.Drawing.Size(150, 150);
            this.controlViewForLiability.TabIndex = 0;
            // 
            // copyPartyView
            // 
            this.copyPartyView.CoverageOrder = null;
            this.copyPartyView.Location = new System.Drawing.Point(0, 0);
            this.copyPartyView.Model = null;
            this.copyPartyView.Name = "copyPartyView";
            this.copyPartyView.Size = new System.Drawing.Size(178, 24);
            this.copyPartyView.TabIndex = 0;
            // 
            // paiWalkinAccountCreationView
            // 
            this.paiWalkinAccountCreationView.AdmitDate = "";
            this.paiWalkinAccountCreationView.AdmitTime = "";
            this.paiWalkinAccountCreationView.Age = "";
            this.paiWalkinAccountCreationView.BackColor = System.Drawing.Color.White;
            this.paiWalkinAccountCreationView.DateOfBirth = "";
            this.paiWalkinAccountCreationView.Location = new System.Drawing.Point(0, 0);
            this.paiWalkinAccountCreationView.Model = null;
            this.paiWalkinAccountCreationView.Name = "paiWalkinAccountCreationView";
            this.paiWalkinAccountCreationView.Size = new System.Drawing.Size(1024, 378);
            this.paiWalkinAccountCreationView.TabIndex = 2;
            this.paiWalkinAccountCreationView.RefreshTopPanel += new System.EventHandler(this.paiWalkinAccountCreationView_RefreshTopPanel);
            this.paiWalkinAccountCreationView.SetTabPageEvent += new System.EventHandler(this.SetTabPageEventHandler);
            // 
            // panelConfirmation
            // 
            this.panelConfirmation.Location = new System.Drawing.Point(0, 0);
            this.panelConfirmation.Name = "panelConfirmation";
            this.panelConfirmation.Size = new System.Drawing.Size(200, 100);
            this.panelConfirmation.TabIndex = 4;
            // 
            // tcViewTabPages
            // 
            this.tcViewTabPages.Controls.Add(this.tpPAIWalkinAccountCreation);
            this.tcViewTabPages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcViewTabPages.Location = new System.Drawing.Point(8, 176);
            this.tcViewTabPages.Name = "tcViewTabPages";
            this.tcViewTabPages.SelectedIndex = 0;
            this.tcViewTabPages.Size = new System.Drawing.Size(1008, 407);
            this.tcViewTabPages.TabIndex = 2;
            this.tcViewTabPages.SelectedIndexChanged += new System.EventHandler(this.tcViewTabPages_SelectedIndexChanged);
            // 
            // tpPAIWalkinAccountCreation
            // 
            this.tpPAIWalkinAccountCreation.Controls.Add(this.paiWalkinAccountCreationView);
            this.tpPAIWalkinAccountCreation.Location = new System.Drawing.Point(4, 29);
            this.tpPAIWalkinAccountCreation.Name = "tpPAIWalkinAccountCreation";
            this.tpPAIWalkinAccountCreation.Size = new System.Drawing.Size(1000, 374);
            this.tpPAIWalkinAccountCreation.TabIndex = 10;
            this.tpPAIWalkinAccountCreation.Text = "Walk-In Outpatient";
            // 
            // panelToDoList
            // 
            this.panelToDoList.Controls.Add(this.lvToDo);
            this.panelToDoList.Controls.Add(this.lblStaticToDoList);
            this.panelToDoList.Controls.Add(this.btnRefreshToDoList);
            this.panelToDoList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelToDoList.Location = new System.Drawing.Point(8, 57);
            this.panelToDoList.Name = "panelToDoList";
            this.panelToDoList.Size = new System.Drawing.Size(1005, 108);
            this.panelToDoList.TabIndex = 1;
            // 
            // lvToDo
            // 
            this.lvToDo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chAction,
            this.chCount,
            this.chWorklist});
            this.lvToDo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvToDo.FullRowSelect = true;
            this.lvToDo.GridLines = true;
            this.lvToDo.HideSelection = false;
            this.lvToDo.Location = new System.Drawing.Point(0, 37);
            this.lvToDo.MultiSelect = false;
            this.lvToDo.Name = "lvToDo";
            this.lvToDo.Size = new System.Drawing.Size(1003, 66);
            this.lvToDo.TabIndex = 2;
            this.lvToDo.UseCompatibleStateImageBehavior = false;
            this.lvToDo.View = System.Windows.Forms.View.Details;
            this.lvToDo.SelectedIndexChanged += new System.EventHandler(this.lvToDo_SelectedIndexChanged);
            this.lvToDo.DoubleClick += new System.EventHandler(this.lvToDo_DoubleClick);
            this.lvToDo.Enter += new System.EventHandler(this.ToDoListView_Enter);
            this.lvToDo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToDoListView_KeyDown);
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
            this.lblStaticToDoList.Location = new System.Drawing.Point(0, 11);
            this.lblStaticToDoList.Name = "lblStaticToDoList";
            this.lblStaticToDoList.Size = new System.Drawing.Size(70, 23);
            this.lblStaticToDoList.TabIndex = 1;
            this.lblStaticToDoList.Text = "To Do List";
            this.lblStaticToDoList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRefreshToDoList
            // 
            this.btnRefreshToDoList.BackColor = System.Drawing.SystemColors.Control;
            this.btnRefreshToDoList.Enabled = false;
            this.btnRefreshToDoList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshToDoList.Location = new System.Drawing.Point(928, 8);
            this.btnRefreshToDoList.Message = null;
            this.btnRefreshToDoList.Name = "btnRefreshToDoList";
            this.btnRefreshToDoList.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshToDoList.TabIndex = 1;
            this.btnRefreshToDoList.Text = "Re&fresh";
            this.btnRefreshToDoList.UseVisualStyleBackColor = false;
            this.btnRefreshToDoList.Click += new System.EventHandler(this.btnRefreshToDoList_Click);
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPatientContext.Controls.Add(this.patientContextView);
            this.panelPatientContext.Location = new System.Drawing.Point(8, 29);
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new System.Drawing.Size(1005, 26);
            this.panelPatientContext.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnActivateShortRegistration);
            this.panel1.Controls.Add(this.btnActivateStandardRegistration);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(0, 585);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1024, 35);
            this.panel1.TabIndex = 3;
            // 
            // btnActivateShortRegistration
            // 
            this.btnActivateShortRegistration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivateShortRegistration.BackColor = System.Drawing.SystemColors.Control;
            this.btnActivateShortRegistration.Location = new System.Drawing.Point(699, -1);
            this.btnActivateShortRegistration.Message = null;
            this.btnActivateShortRegistration.Name = "btnActivateShortRegistration";
            this.btnActivateShortRegistration.Size = new System.Drawing.Size(130, 34);
            this.btnActivateShortRegistration.TabIndex = 54;
            this.btnActivateShortRegistration.Text = "  Activate Account - &Diagnostic Registration";
            this.btnActivateShortRegistration.UseVisualStyleBackColor = false;
            this.btnActivateShortRegistration.Click += new System.EventHandler(this.btnActivateShortRegistration_Click);
            // 
            // btnActivateStandardRegistration
            // 
            this.btnActivateStandardRegistration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivateStandardRegistration.BackColor = System.Drawing.SystemColors.Control;
            this.btnActivateStandardRegistration.Location = new System.Drawing.Point(534, -1);
            this.btnActivateStandardRegistration.Message = null;
            this.btnActivateStandardRegistration.Name = "btnActivateStandardRegistration";
            this.btnActivateStandardRegistration.Size = new System.Drawing.Size(130, 34);
            this.btnActivateStandardRegistration.TabIndex = 53;
            this.btnActivateStandardRegistration.Text = "  Acti&vate Account - Standard Registration";
            this.btnActivateStandardRegistration.UseVisualStyleBackColor = false;
            this.btnActivateStandardRegistration.Click += new System.EventHandler(this.btnActivateStandardRegistration_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point(864, -1);
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 24);
            this.btnCancel.TabIndex = 55;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panelUserContext
            // 
            this.panelUserContext.BackColor = System.Drawing.SystemColors.Control;
            this.panelUserContext.Controls.Add(this.userContextView);
            this.panelUserContext.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUserContext.Location = new System.Drawing.Point(0, 0);
            this.panelUserContext.Name = "panelUserContext";
            this.panelUserContext.Size = new System.Drawing.Size(1024, 22);
            this.panelUserContext.TabIndex = 0;
            // 
            // PAIWalkinAccountView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.Controls.Add(this.panelUserContext);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelPatientContext);
            this.Controls.Add(this.panelToDoList);
            this.Controls.Add(this.tcViewTabPages);
            this.Controls.Add(this.panelConfirmation);
            this.Controls.Add(this.progressPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PAIWalkinAccountView";
            this.Size = new System.Drawing.Size(1024, 620);
            this.Load += new System.EventHandler(this.AccountView_Load);
            this.Leave += new System.EventHandler(this.AccountView_Leave);
            this.tcViewTabPages.ResumeLayout(false);
            this.tpPAIWalkinAccountCreation.ResumeLayout(false);
            this.panelToDoList.ResumeLayout(false);
            this.panelPatientContext.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panelUserContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Data Elements

        private LoggingButton btnRefreshToDoList;
        private ClickOnceLoggingButton btnCancel;
        private ClickOnceLoggingButton btnActivateStandardRegistration;
        private ClickOnceLoggingButton btnActivateShortRegistration;
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
        private TabPage tpPAIWalkinAccountCreation;
        private CopyPartyView copyPartyView;
        private PAIWalkinAccountCreationView paiWalkinAccountCreationView;
        private PatientContextView patientContextView;
        private UserContextView userContextView;
        private InvalidCodeFieldsDialog invalidCodeFieldsDialog;
        private InvalidCodeOptionalFieldsDialog invalidCodeOptionalFieldsDialog;
        private MaintenanceCmdView maintenanceCmdView;
        private ControlView paymentView;
        private ProgressPanel progressPanel1;
        private IRequiredFieldsSummaryView RequiredFieldSummaryView = new RequiredFieldsSummaryView();
        private RequiredFieldsDialog requiredFieldsDialog;
        private ControlView controlViewForLiability;



        # endregion
    }
}
