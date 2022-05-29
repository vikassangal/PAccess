using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.DemographicsViews
{
    /// <summary>
    /// Summary description for AddAKA.
    /// </summary>
    public class AKAEntryDialog : TimeOutFormView
    {

        #region Event Handlers
        private void btnCancel_Click( object sender, EventArgs e )
        {
            this.Dispose();
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            this.akaView1.UpdateModel();
            this.AliasName = akaView1.Model_Name;
            this.DialogResult = DialogResult.OK;
        }

        private void akaView1_TextEntryEvent( object sender, EventArgs e )
        {
            if( akaView1.FirstName.Length > 0 && akaView1.LastName.Length > 0 )
            {
                this.btnOk.Enabled = true;
                this.AcceptButton = this.btnOk;
            }
            else
            {
                this.btnOk.Enabled = false;
                this.AcceptButton = this.btnCancel;
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

        public override void UpdateView()
        {
            PopulatePatientSummaryView();
            PopulateAKAView();
        }

        public override void UpdateModel()
        {
            this.akaView1.UpdateModel();
        }
        #endregion

        #region Properties

        private Patient Model_Patient
        {
            get
            {
                return ( Patient )this.Model;
            }
            set
            {
                this.Model = value;
            }
        }

        public Name AliasName
        {
            get
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
        private void PopulatePatientSummaryView()
        {
            this.patientSummaryView1.Model =  this.Model_Patient;
            this.patientSummaryView1.UpdateView();
        }

        private void PopulateAKAView()
        {
            if( this.Text == EDITAKA )
            {
                this.akaView1.Model = this.AliasName;
                this.akaView1.UpdateView();
            }
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelPatientSummary = new System.Windows.Forms.Panel();
            this.patientSummaryView1 = new PatientAccess.UI.DemographicsViews.PatientSummaryView();
            this.panelAddAka = new System.Windows.Forms.Panel();
            this.akaView1 = new PatientAccess.UI.DemographicsViews.AKAView();
            this.lblAkaName = new System.Windows.Forms.Label();
            this.btnOk = new LoggingButton();
            this.btnCancel = new LoggingButton();
            this.panelPatientSummary.SuspendLayout();
            this.panelAddAka.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPatientSummary
            // 
            this.panelPatientSummary.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.panelPatientSummary.Controls.Add(this.patientSummaryView1);
            this.panelPatientSummary.Location = new System.Drawing.Point(8, 8);
            this.panelPatientSummary.Name = "panelPatientSummary";
            this.panelPatientSummary.Size = new System.Drawing.Size(576, 168);
            this.panelPatientSummary.TabIndex = 0;
            // 
            // patientSummaryView1
            // 
            this.patientSummaryView1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.patientSummaryView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientSummaryView1.Location = new System.Drawing.Point(0, 0);
            this.patientSummaryView1.Model = null;
            this.patientSummaryView1.Name = "patientSummaryView1";
            this.patientSummaryView1.Size = new System.Drawing.Size(576, 168);
            this.patientSummaryView1.TabIndex = 0;
            this.patientSummaryView1.TabStop = false;
            // 
            // panelAddAka
            // 
            this.panelAddAka.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelAddAka.Controls.Add(this.akaView1);
            this.panelAddAka.Location = new System.Drawing.Point(8, 216);
            this.panelAddAka.Name = "panelAddAka";
            this.panelAddAka.Size = new System.Drawing.Size(576, 104);
            this.panelAddAka.TabIndex = 1;
            // 
            // akaView1
            // 
            this.akaView1.BackColor = System.Drawing.Color.White;
            this.akaView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.akaView1.Location = new System.Drawing.Point(0, 0);
            this.akaView1.Model = null;
            this.akaView1.Name = "akaView1";
            this.akaView1.Size = new System.Drawing.Size(574, 102);
            this.akaView1.TabIndex = 0;
            // 
            // lblAkaName
            // 
            this.lblAkaName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblAkaName.Location = new System.Drawing.Point(8, 194);
            this.lblAkaName.Name = "lblAkaName";
            this.lblAkaName.Size = new System.Drawing.Size(112, 16);
            this.lblAkaName.TabIndex = 2;
            this.lblAkaName.Text = "AKA Name";
            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(426, 328);
            this.btnOk.Name = "btnOk";
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(510, 328);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // AKAEntryDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(592, 356);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblAkaName);
            this.Controls.Add(this.panelAddAka);
            this.Controls.Add(this.panelPatientSummary);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AKAEntryDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.panelPatientSummary.ResumeLayout(false);
            this.panelAddAka.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AKAEntryDialog()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            base.EnableThemesOn( this );

            this.akaView1.TextEntryEvent +=new EventHandler( akaView1_TextEntryEvent );
            AcceptButton = this.btnCancel;
        }
        #endregion


        #region Data Elements
        private PatientSummaryView patientSummaryView1;
        private Label lblAkaName;
        private LoggingButton btnOk;
        private LoggingButton btnCancel;
        private Panel panelPatientSummary;
        private Panel panelAddAka;
        private AKAView akaView1;
        private Container components = null;
        private Name i_AliasName = null;
        #endregion
 
        #region Constants
        private const string EDITAKA = "Edit AKA";
        #endregion
    }
}
