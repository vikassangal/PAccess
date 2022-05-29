using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.DemographicsViews
{
	/// <summary>
	/// Summary description for PatientSummaryView.
	/// </summary>
	public class PatientSummaryView  : ControlView
	{
        #region Event Handlers
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
            Patient patient = this.Model_Patient;
            this.lblPatientName.Text = patient.FormattedName;
            this.lblSSN.Text = patient.SocialSecurityNumber.AsFormattedString();
            if( patient.MedicalRecordNumber != 0 )
            {
                this.lblMRN.Text = patient.MedicalRecordNumber.ToString();
            }
            if( patient.SelectedAccount != null && !patient.SelectedAccount.IsNew )
            {
                this.lblAccount.Text = patient.SelectedAccount.AccountNumber.ToString();
            }
            if( patient.DateOfBirth != DateTime.MinValue)
            {
                this.lblDOB.Text = patient.DateOfBirth.ToString("MM/dd/yyyy" );
            }
            this.lblGender.Text = patient.Sex.ToString();
        }


        #endregion

        #region Properties

	    private Patient Model_Patient
        {
            get
            {
                return (Patient)this.Model;
            }
        }
        #endregion

        #region Private Methods
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpBoxPatientSummary = new System.Windows.Forms.GroupBox();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblMRN = new System.Windows.Forms.Label();
            this.lblSSN = new System.Windows.Forms.Label();
            this.lblDOB = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblStaticAcount = new System.Windows.Forms.Label();
            this.lblStaticMRN = new System.Windows.Forms.Label();
            this.lblStaticSSN = new System.Windows.Forms.Label();
            this.lblStaticDOB = new System.Windows.Forms.Label();
            this.lblStaticGender = new System.Windows.Forms.Label();
            this.lblStaticPatientName = new System.Windows.Forms.Label();
            this.grpBoxPatientSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBoxPatientSummary
            // 
            this.grpBoxPatientSummary.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.grpBoxPatientSummary.Controls.Add(this.lblAccount);
            this.grpBoxPatientSummary.Controls.Add(this.lblMRN);
            this.grpBoxPatientSummary.Controls.Add(this.lblSSN);
            this.grpBoxPatientSummary.Controls.Add(this.lblDOB);
            this.grpBoxPatientSummary.Controls.Add(this.lblGender);
            this.grpBoxPatientSummary.Controls.Add(this.lblPatientName);
            this.grpBoxPatientSummary.Controls.Add(this.lblStaticAcount);
            this.grpBoxPatientSummary.Controls.Add(this.lblStaticMRN);
            this.grpBoxPatientSummary.Controls.Add(this.lblStaticSSN);
            this.grpBoxPatientSummary.Controls.Add(this.lblStaticDOB);
            this.grpBoxPatientSummary.Controls.Add(this.lblStaticGender);
            this.grpBoxPatientSummary.Controls.Add(this.lblStaticPatientName);
            this.grpBoxPatientSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBoxPatientSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.grpBoxPatientSummary.Location = new System.Drawing.Point(0, 0);
            this.grpBoxPatientSummary.Name = "grpBoxPatientSummary";
            this.grpBoxPatientSummary.Size = new System.Drawing.Size(504, 168);
            this.grpBoxPatientSummary.TabIndex = 0;
            this.grpBoxPatientSummary.TabStop = false;
            this.grpBoxPatientSummary.Text = "Patient Summary";
            // 
            // lblAccount
            // 
            this.lblAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblAccount.Location = new System.Drawing.Point(88, 144);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(141, 16);
            this.lblAccount.TabIndex = 11;
            // 
            // lblMRN
            // 
            this.lblMRN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblMRN.Location = new System.Drawing.Point(88, 120);
            this.lblMRN.Name = "lblMRN";
            this.lblMRN.Size = new System.Drawing.Size(141, 16);
            this.lblMRN.TabIndex = 10;
            // 
            // lblSSN
            // 
            this.lblSSN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblSSN.Location = new System.Drawing.Point(88, 96);
            this.lblSSN.Name = "lblSSN";
            this.lblSSN.Size = new System.Drawing.Size(136, 16);
            this.lblSSN.TabIndex = 9;
            // 
            // lblDOB
            // 
            this.lblDOB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblDOB.Location = new System.Drawing.Point(88, 72);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(144, 16);
            this.lblDOB.TabIndex = 8;
            // 
            // lblGender
            // 
            this.lblGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblGender.Location = new System.Drawing.Point(88, 48);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(61, 16);
            this.lblGender.TabIndex = 7;
            // 
            // lblPatientName
            // 
            this.lblPatientName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblPatientName.Location = new System.Drawing.Point(88, 24);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(384, 16);
            this.lblPatientName.TabIndex = 6;
            // 
            // lblStaticAcount
            // 
            this.lblStaticAcount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticAcount.Location = new System.Drawing.Point(8, 144);
            this.lblStaticAcount.Name = "lblStaticAcount";
            this.lblStaticAcount.Size = new System.Drawing.Size(80, 16);
            this.lblStaticAcount.TabIndex = 5;
            this.lblStaticAcount.Text = "Account:";
            // 
            // lblStaticMRN
            // 
            this.lblStaticMRN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticMRN.Location = new System.Drawing.Point(8, 120);
            this.lblStaticMRN.Name = "lblStaticMRN";
            this.lblStaticMRN.Size = new System.Drawing.Size(80, 16);
            this.lblStaticMRN.TabIndex = 4;
            this.lblStaticMRN.Text = "MRN:";
            // 
            // lblStaticSSN
            // 
            this.lblStaticSSN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticSSN.Location = new System.Drawing.Point(8, 96);
            this.lblStaticSSN.Name = "lblStaticSSN";
            this.lblStaticSSN.Size = new System.Drawing.Size(80, 16);
            this.lblStaticSSN.TabIndex = 3;
            this.lblStaticSSN.Text = "SSN:";
            // 
            // lblStaticDOB
            // 
            this.lblStaticDOB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticDOB.Location = new System.Drawing.Point(8, 72);
            this.lblStaticDOB.Name = "lblStaticDOB";
            this.lblStaticDOB.Size = new System.Drawing.Size(80, 16);
            this.lblStaticDOB.TabIndex = 2;
            this.lblStaticDOB.Text = "DOB:";
            // 
            // lblStaticGender
            // 
            this.lblStaticGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticGender.Location = new System.Drawing.Point(8, 48);
            this.lblStaticGender.Name = "lblStaticGender";
            this.lblStaticGender.Size = new System.Drawing.Size(80, 16);
            this.lblStaticGender.TabIndex = 1;
            this.lblStaticGender.Text = "Gender:";
            // 
            // lblStaticPatientName
            // 
            this.lblStaticPatientName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticPatientName.Location = new System.Drawing.Point(8, 24);
            this.lblStaticPatientName.Name = "lblStaticPatientName";
            this.lblStaticPatientName.Size = new System.Drawing.Size(80, 16);
            this.lblStaticPatientName.TabIndex = 0;
            this.lblStaticPatientName.Text = "Patient Name:";
            // 
            // PatientSummaryView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.grpBoxPatientSummary);
            this.Name = "PatientSummaryView";
            this.Size = new System.Drawing.Size(504, 168);
            this.grpBoxPatientSummary.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PatientSummaryView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }
        #endregion

        #region Data Elements
        private GroupBox grpBoxPatientSummary;
        private Label lblStaticPatientName;
        private Label lblStaticGender;
        private Label lblStaticDOB;
        private Label lblStaticSSN;
        private Label lblStaticMRN;
        private Label lblStaticAcount;
        private Label lblPatientName;
        private Label lblGender;
        private Label lblDOB;
        private Label lblSSN;
        private Label lblMRN;
        private Label lblAccount;
        private Container components = null;
        #endregion

        #region Constants
        #endregion

	}
}
