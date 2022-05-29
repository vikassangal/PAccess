using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.DemographicsViews
{
    /// <summary>
    /// Summary description for ManageAKADialog.
    /// </summary>
    public class ManageAKADialog : TimeOutFormView
    {
		#region Events
		public event EventHandler UpdateAKAName;
		#endregion

        #region Event Handlers
        private void btnEdit_Click( object sender, EventArgs e )
        {
            ShowAKAEditEntryDialog();
        }
        private void btnDelete_Click( object sender, EventArgs e )
        {
            DeleteAKAEntry();
        }

        private void btnClose_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnAdd_Click( object sender, EventArgs e )
        {
            ShowAKAAddEntryDialog();
        }

        private void akaListView1_AKASelectedEvent( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            this.SelectedAliasName = args.Context as Name;
            ActivateButtons( true );
            this.akaListView1.FocusOnGrid();
        }

        private void akaListView1_AKADoubleClickEvent(object sender, EventArgs e)
        {
             ShowAKAEditEntryDialog();
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
        /// <summary>
        /// UpdateView()
        /// </summary>
        public override void UpdateView()
        {
            PopulatePatientSummaryView();
            PopulateAKAListView();
        }

        /// <summary>
        /// UpdateModel()
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel();
        }
        #endregion

        #region Properties
        public Patient Model_Patient
        {
            get
            {
                return ( Patient )base.Model;
            }
            set
            {
                this.Model = value;
            }
        }
        #endregion

        #region Private Methods
        private void DeleteAKAEntry()
        {
            Name aName = this.SelectedAliasName;
            this.Model_Patient.RemoveAlias( aName );
            ActivateButtons( false );
            PopulateAKAListView();
			this.UpdateAKAName(this, null);
        }

        private void ShowAKAEditEntryDialog()
        {
            AKAEntryDialog aed = new AKAEntryDialog();
            aed.Text = EDITAKA;
            aed.AliasName = this.SelectedAliasName;
            aed.Model = this.Model_Patient;
            aed.UpdateModel();
            aed.UpdateView();

			try
			{
				aed.ShowDialog( this ); 

				if( aed.DialogResult == DialogResult.OK )
				{
					this.Model_Patient.RemoveAlias( this.SelectedAliasName );
					this.Model_Patient.AddAlias( aed.AliasName );
					this.SelectedAliasName = aed.AliasName;
					ActivateButtons( false );
					this.UpdateModel();
					this.UpdateAKAName(this, null);
				}
				this.UpdateView();
			}
			finally
			{
				aed.Dispose();
			}
        }

        private void ShowAKAAddEntryDialog()
        {
            AKAEntryDialog aed = new AKAEntryDialog();
            aed.Text = ADDAKA;
            aed.Model = this.Model_Patient;
            aed.UpdateModel();
            aed.UpdateView();

			try
			{
				aed.ShowDialog( this ); 

				if( aed.DialogResult == DialogResult.OK )
				{
					this.Model_Patient.AddAlias( aed.AliasName );  
					this.SelectedAliasName = aed.AliasName;
					ActivateButtons( true );
					this.UpdateModel(); 
					this.UpdateAKAName(this, null);
				}
				this.UpdateView();
			}
			finally
			{
				aed.Dispose();
			}
        }

        private void PopulatePatientSummaryView()
        {
            this.patientSummaryView1.Model =  this.Model_Patient;
            this.patientSummaryView1.UpdateView();
        }

        private void PopulateAKAListView()
        {
            if( this.Model_Patient.HasAliases() )
            {
                this.panelAKAlist.Visible = true;
                this.panelAkaListErrorMEssage.Visible = false;

                this.akaListView1.Model = this.Model_Patient;
                this.akaListView1.SelectedAliasName = this.SelectedAliasName;
                this.akaListView1.UpdateView();
            }
            else
            {
                this.panelAKAlist.Visible = false;
                this.panelAkaListErrorMEssage.Visible = true;
            }
        }

        private void ActivateButtons( bool status )
        {
            this.btnEdit.Enabled = status;
            this.btnDelete.Enabled = status;
            if( status )
            {
                this.AcceptButton = this.btnEdit;
            }
            else
            {
                this.AcceptButton = this.btnClose;
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.panelPatientSummary = new System.Windows.Forms.Panel();
			this.patientSummaryView1 = new PatientAccess.UI.DemographicsViews.PatientSummaryView();
			this.panelAKAlist = new System.Windows.Forms.Panel();
			this.akaListView1 = new PatientAccess.UI.DemographicsViews.AKAListView();
			this.lblAKANames = new System.Windows.Forms.Label();
			this.btnAdd = new LoggingButton();
			this.btnEdit = new LoggingButton();
			this.btnDelete = new LoggingButton();
			this.btnClose = new LoggingButton();
			this.panelAkaListErrorMEssage = new System.Windows.Forms.Panel();
			this.lblAkaListErrorMessage = new System.Windows.Forms.Label();
			this.panelPatientSummary.SuspendLayout();
			this.panelAKAlist.SuspendLayout();
			this.panelAkaListErrorMEssage.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelPatientSummary
			// 
			this.panelPatientSummary.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
			this.panelPatientSummary.Controls.Add(this.patientSummaryView1);
			this.panelPatientSummary.Location = new System.Drawing.Point(8, 16);
			this.panelPatientSummary.Name = "panelPatientSummary";
			this.panelPatientSummary.Size = new System.Drawing.Size(656, 168);
			this.panelPatientSummary.TabIndex = 0;
			// 
			// patientSummaryView1
			// 
			this.patientSummaryView1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
			this.patientSummaryView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.patientSummaryView1.Location = new System.Drawing.Point(0, 0);
			this.patientSummaryView1.Model = null;
			this.patientSummaryView1.Name = "patientSummaryView1";
			this.patientSummaryView1.Size = new System.Drawing.Size(656, 168);
			this.patientSummaryView1.TabIndex = 0;
			this.patientSummaryView1.TabStop = false;
			// 
			// panelAKAlist
			// 
			this.panelAKAlist.Controls.Add(this.akaListView1);
			this.panelAKAlist.Location = new System.Drawing.Point(8, 248);
			this.panelAKAlist.Name = "panelAKAlist";
			this.panelAKAlist.Size = new System.Drawing.Size(656, 160);
			this.panelAKAlist.TabIndex = 1;
			// 
			// akaListView1
			// 
			this.akaListView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.akaListView1.Location = new System.Drawing.Point(0, 0);
			this.akaListView1.Model = null;
			this.akaListView1.Name = "akaListView1";
			this.akaListView1.SelectedAliasName = null;
			this.akaListView1.Size = new System.Drawing.Size(656, 160);
			this.akaListView1.TabIndex = 2;
			// 
			// lblAKANames
			// 
			this.lblAKANames.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblAKANames.Location = new System.Drawing.Point(8, 215);
			this.lblAKANames.Name = "lblAKANames";
			this.lblAKANames.Size = new System.Drawing.Size(100, 16);
			this.lblAKANames.TabIndex = 2;
			this.lblAKANames.Text = "AKA Names";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(589, 207);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 1;
			this.btnAdd.Text = "&Add...";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.Enabled = false;
			this.btnEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnEdit.Location = new System.Drawing.Point(416, 416);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.TabIndex = 3;
			this.btnEdit.Text = "&Edit...";
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Enabled = false;
			this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnDelete.Location = new System.Drawing.Point(502, 416);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.TabIndex = 4;
			this.btnDelete.Text = "&Delete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnClose.Location = new System.Drawing.Point(588, 416);
			this.btnClose.Name = "btnClose";
			this.btnClose.TabIndex = 5;
			this.btnClose.Text = "Close";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// panelAkaListErrorMEssage
			// 
			this.panelAkaListErrorMEssage.BackColor = System.Drawing.Color.White;
			this.panelAkaListErrorMEssage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelAkaListErrorMEssage.Controls.Add(this.lblAkaListErrorMessage);
			this.panelAkaListErrorMEssage.Location = new System.Drawing.Point(8, 248);
			this.panelAkaListErrorMEssage.Name = "panelAkaListErrorMEssage";
			this.panelAkaListErrorMEssage.Size = new System.Drawing.Size(656, 160);
			this.panelAkaListErrorMEssage.TabIndex = 7;
			this.panelAkaListErrorMEssage.Visible = false;
			// 
			// lblAkaListErrorMessage
			// 
			this.lblAkaListErrorMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblAkaListErrorMessage.Location = new System.Drawing.Point(0, 0);
			this.lblAkaListErrorMessage.Name = "lblAkaListErrorMessage";
			this.lblAkaListErrorMessage.Size = new System.Drawing.Size(654, 158);
			this.lblAkaListErrorMessage.TabIndex = 0;
			// 
			// ManageAKADialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(674, 448);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lblAKANames);
			this.Controls.Add(this.panelPatientSummary);
			this.Controls.Add(this.panelAKAlist);
			this.Controls.Add(this.panelAkaListErrorMEssage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ManageAKADialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Manage AKAs";
			this.panelPatientSummary.ResumeLayout(false);
			this.panelAKAlist.ResumeLayout(false);
			this.panelAkaListErrorMEssage.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion
        #endregion

        #region Private Properties
        private Name SelectedAliasName
        {
            get
            {
                return i_selectedAliasName;
            }
            set
            {
                this.i_selectedAliasName = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public ManageAKADialog()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
            base.EnableThemesOn( this );
            this.akaListView1.AKASelectedEvent += new EventHandler( akaListView1_AKASelectedEvent );
            this.akaListView1.AKADoubleClickEvent += new EventHandler(akaListView1_AKADoubleClickEvent);
            this.lblAkaListErrorMessage.Text = AKALIST_ERRMSG;
            AcceptButton = this.btnClose;
        }
        #endregion

        #region Data Elements
        private Container components = null;  
        private Panel panelPatientSummary;
        private Panel panelAKAlist;
        private Label lblAKANames;
        private LoggingButton btnAdd;
        private LoggingButton btnEdit;
        private LoggingButton btnDelete;
        private LoggingButton btnClose;
        private PatientSummaryView patientSummaryView1;
        private AKAListView akaListView1;
        private Name i_selectedAliasName = null;
        private Panel panelAkaListErrorMEssage;
        private Label lblAkaListErrorMessage;
        #endregion
 
        #region Constants
        private const string ADDAKA = "Add AKA";
        private const string EDITAKA = "Edit AKA";
        private const string AKALIST_ERRMSG = "No previous AKA names recorded for this patient.";
        #endregion
    }
}
