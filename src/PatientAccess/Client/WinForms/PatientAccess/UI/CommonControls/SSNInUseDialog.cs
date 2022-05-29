using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for SSNInUseDialog.
    /// </summary>
    public class SSNInUseDialog : TimeOutFormView
    {
        #region Event Handlers
        private void listViewSelectedIndexChanged( object sender, EventArgs e )
        {
            // The list must be non-selectable, but setting it to Enabled = false makes
            // it impossible for the scroll bars to work since that puts the list in a 
            // state where the user can't interact with the list control.  So, the list
            // must be Enabled and we'll just unselect the selected item here.
            ListView.SelectedListViewItemCollection collection = listView.SelectedItems;
            if( collection.Count > 0 )
            {   // First item in collection is the selected index for single selection listbox
                ListViewItem item = collection[ 0 ];
                if( item.Selected == true )
                {
                    item.Selected = false;
                }
            }
        }

        private void SSNInUseDialogLoad( object sender, EventArgs e )
        {
            if( Model_Patient != null )
            {
                string name = String.Format( "{0}, {1} {2}", Model_Patient.LastName,
                                                             Model_Patient.FirstName,
                                                             Model_Patient.MiddleInitial );
                lblName.Text = name;
                lblSSN.Text  = Model_Patient.SocialSecurityNumber.PrintString;
            }

            foreach( PatientSearchResult aPatientSearchResult in this._PatientSearchResults )
            {
                ListViewItem item = new ListViewItem( aPatientSearchResult.Name.AsFormattedName() );
                item.SubItems.Add( aPatientSearchResult.MedicalRecordNumber.ToString() );
                listView.Items.Add( item );
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Properties

        private Patient Model_Patient
        {
            get
            {
                return this.Model as Patient;
            }
        }
        #endregion

        #region Private Methods

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelControl = new System.Windows.Forms.Panel();
            this.btnCancel = new LoggingButton();
            this.btnOK = new LoggingButton();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHdrName = new System.Windows.Forms.ColumnHeader();
            this.columnHdrMRN = new System.Windows.Forms.ColumnHeader();
            this.lblTextMessage = new System.Windows.Forms.Label();
            this.lblSSN = new System.Windows.Forms.Label();
            this.lblStaticSSN = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblStaticName = new System.Windows.Forms.Label();
            this.panelControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl
            // 
            this.panelControl.Controls.Add(this.btnCancel);
            this.panelControl.Controls.Add(this.btnOK);
            this.panelControl.Controls.Add(this.listView);
            this.panelControl.Controls.Add(this.lblTextMessage);
            this.panelControl.Controls.Add(this.lblSSN);
            this.panelControl.Controls.Add(this.lblStaticSSN);
            this.panelControl.Controls.Add(this.lblName);
            this.panelControl.Controls.Add(this.lblStaticName);
            this.panelControl.Location = new System.Drawing.Point(20, 19);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(360, 203);
            this.panelControl.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(192, 182);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 21);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "INVISIBLE";
            this.btnCancel.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(284, 182);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 21);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                       this.columnHdrName,
                                                                                       this.columnHdrMRN});
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.Location = new System.Drawing.Point(0, 75);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(360, 88);
            this.listView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listView.TabIndex = 1;
            this.listView.TabStop = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listViewSelectedIndexChanged);
            // 
            // columnHdrName
            // 
            this.columnHdrName.Text = "Patient Name";
            this.columnHdrName.Width = 260;
            // 
            // columnHdrMRN
            // 
            this.columnHdrMRN.Text = "MRN";
            this.columnHdrMRN.Width = 96;
            // 
            // lblTextMessage
            // 
            this.lblTextMessage.Location = new System.Drawing.Point(0, 48);
            this.lblTextMessage.Name = "lblTextMessage";
            this.lblTextMessage.Size = new System.Drawing.Size(360, 22);
            this.lblTextMessage.TabIndex = 0;
            this.lblTextMessage.Text = "The SSN listed above is in use by one or more patients listed below.";
            // 
            // lblSSN
            // 
            this.lblSSN.Location = new System.Drawing.Point(38, 24);
            this.lblSSN.Name = "lblSSN";
            this.lblSSN.Size = new System.Drawing.Size(96, 16);
            this.lblSSN.TabIndex = 0;
            // 
            // lblStaticSSN
            // 
            this.lblStaticSSN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticSSN.Location = new System.Drawing.Point(0, 24);
            this.lblStaticSSN.Name = "lblStaticSSN";
            this.lblStaticSSN.Size = new System.Drawing.Size(50, 16);
            this.lblStaticSSN.TabIndex = 0;
            this.lblStaticSSN.Text = "SSN:";
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(88, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(264, 16);
            this.lblName.TabIndex = 0;
            // 
            // lblStaticName
            // 
            this.lblStaticName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticName.Location = new System.Drawing.Point(0, 0);
            this.lblStaticName.Name = "lblStaticName";
            this.lblStaticName.Size = new System.Drawing.Size(80, 16);
            this.lblStaticName.TabIndex = 0;
            this.lblStaticName.Text = "Patient Name:";
            // 
            // SSNInUseDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(402, 239);
            this.Controls.Add(this.panelControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SSNInUseDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SSN in Use";
            this.Load += new System.EventHandler(this.SSNInUseDialogLoad);
            this.panelControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Construction and Finalization
        public SSNInUseDialog( PatientSearchResponse patientSearchResponse )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this._PatientSearchResults = patientSearchResponse.PatientSearchResults;
            base.EnableThemesOn( this );
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
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container     components = null;
        
        private LoggingButton         btnOK;

        private ColumnHeader   columnHdrName;
        private ColumnHeader   columnHdrMRN;

        private Label          lblStaticName;
        private Label          lblName;
        private Label          lblStaticSSN;
        private Label          lblSSN;
        private Label          lblTextMessage;
        
        private ListView       listView;
        
        private Panel          panelControl;
        private LoggingButton         btnCancel;
        private List<PatientSearchResult> _PatientSearchResults;
        #endregion
    }
}
