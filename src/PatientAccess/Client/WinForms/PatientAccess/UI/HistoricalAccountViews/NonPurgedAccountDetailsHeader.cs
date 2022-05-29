using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.HistoricalAccountViews
{
	/// <summary>
	/// Summary description for NonPurgedAccountDetailsHeader.
	/// </summary>
	public class NonPurgedAccountDetailsHeader : ControlView
	{
		#region Events
        
		#endregion

		#region Event Handler
        
		#endregion
		
		#region Construction And Finalization

		public NonPurgedAccountDetailsHeader()
		{
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.NonPurgedAccountDetailsHeaderPanel = new System.Windows.Forms.Panel();
            this.accountValueLabel = new System.Windows.Forms.Label();
            this.accountLabel = new System.Windows.Forms.Label();
            this.MRNValueLabel = new System.Windows.Forms.Label();
            this.MRNLabel = new System.Windows.Forms.Label();
            this.SSNValueLabel = new System.Windows.Forms.Label();
            this.SSNLabel = new System.Windows.Forms.Label();
            this.DOBValueLabel = new System.Windows.Forms.Label();
            this.DOBLabel = new System.Windows.Forms.Label();
            this.genderValueLabel = new System.Windows.Forms.Label();
            this.genderLabel = new System.Windows.Forms.Label();
            this.patientNameValueLabel = new System.Windows.Forms.Label();
            this.patientNameLabel = new System.Windows.Forms.Label();
            this.NonPurgedAccountDetailsHeaderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NonPurgedAccountDetailsHeaderPanel
            // 
            this.NonPurgedAccountDetailsHeaderPanel.BackColor = System.Drawing.Color.White;
            this.NonPurgedAccountDetailsHeaderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.accountValueLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.accountLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.MRNValueLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.MRNLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.SSNValueLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.SSNLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.DOBValueLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.DOBLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.genderValueLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.genderLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.patientNameValueLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Controls.Add(this.patientNameLabel);
            this.NonPurgedAccountDetailsHeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NonPurgedAccountDetailsHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.NonPurgedAccountDetailsHeaderPanel.Name = "NonPurgedAccountDetailsHeaderPanel";
            this.NonPurgedAccountDetailsHeaderPanel.Size = new System.Drawing.Size(680, 53);
            this.NonPurgedAccountDetailsHeaderPanel.TabIndex = 0;
            // 
            // accountValueLabel
            // 
            this.accountValueLabel.AutoSize = true;
            this.accountValueLabel.Location = new System.Drawing.Point(335, 32);
            this.accountValueLabel.Name = "accountValueLabel";
            this.accountValueLabel.Size = new System.Drawing.Size(0, 16);
            this.accountValueLabel.TabIndex = 11;
            // 
            // accountLabel
            // 
            this.accountLabel.AutoSize = true;
            this.accountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.accountLabel.Location = new System.Drawing.Point(272, 32);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Size = new System.Drawing.Size(50, 16);
            this.accountLabel.TabIndex = 10;
            this.accountLabel.Text = "Account:";
            // 
            // MRNValueLabel
            // 
            this.MRNValueLabel.AutoSize = true;
            this.MRNValueLabel.Location = new System.Drawing.Point(207, 32);
            this.MRNValueLabel.Name = "MRNValueLabel";
            this.MRNValueLabel.Size = new System.Drawing.Size(0, 16);
            this.MRNValueLabel.TabIndex = 9;
            // 
            // MRNLabel
            // 
            this.MRNLabel.AutoSize = true;
            this.MRNLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.MRNLabel.Location = new System.Drawing.Point(160, 32);
            this.MRNLabel.Name = "MRNLabel";
            this.MRNLabel.Size = new System.Drawing.Size(34, 16);
            this.MRNLabel.TabIndex = 8;
            this.MRNLabel.Text = "MRN:";
            // 
            // SSNValueLabel
            // 
            this.SSNValueLabel.AutoSize = true;
            this.SSNValueLabel.Location = new System.Drawing.Point(46, 32);
            this.SSNValueLabel.Name = "SSNValueLabel";
            this.SSNValueLabel.Size = new System.Drawing.Size(0, 16);
            this.SSNValueLabel.TabIndex = 7;
            // 
            // SSNLabel
            // 
            this.SSNLabel.AutoSize = true;
            this.SSNLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.SSNLabel.Location = new System.Drawing.Point(8, 32);
            this.SSNLabel.Name = "SSNLabel";
            this.SSNLabel.Size = new System.Drawing.Size(31, 16);
            this.SSNLabel.TabIndex = 6;
            this.SSNLabel.Text = "SSN:";
            // 
            // DOBValueLabel
            // 
            this.DOBValueLabel.Location = new System.Drawing.Point(549, 8);
            this.DOBValueLabel.Name = "DOBValueLabel";
            this.DOBValueLabel.TabIndex = 5;
            // 
            // DOBLabel
            // 
            this.DOBLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.DOBLabel.Location = new System.Drawing.Point(506, 8);
            this.DOBLabel.Name = "DOBLabel";
            this.DOBLabel.Size = new System.Drawing.Size(33, 16);
            this.DOBLabel.TabIndex = 4;
            this.DOBLabel.Text = "DOB:";
            // 
            // genderValueLabel
            // 
            this.genderValueLabel.AutoSize = true;
            this.genderValueLabel.Location = new System.Drawing.Point(385, 8);
            this.genderValueLabel.Name = "genderValueLabel";
            this.genderValueLabel.Size = new System.Drawing.Size(0, 16);
            this.genderValueLabel.TabIndex = 3;
            // 
            // genderLabel
            // 
            this.genderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.genderLabel.Location = new System.Drawing.Point(328, 8);
            this.genderLabel.Name = "genderLabel";
            this.genderLabel.Size = new System.Drawing.Size(47, 16);
            this.genderLabel.TabIndex = 2;
            this.genderLabel.Text = "Gender:";
            // 
            // patientNameValueLabel
            // 
            this.patientNameValueLabel.AutoSize = true;
            this.patientNameValueLabel.Location = new System.Drawing.Point(94, 8);
            this.patientNameValueLabel.Name = "patientNameValueLabel";
            this.patientNameValueLabel.Size = new System.Drawing.Size(0, 16);
            this.patientNameValueLabel.TabIndex = 1;
            // 
            // patientNameLabel
            // 
            this.patientNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.patientNameLabel.Location = new System.Drawing.Point(8, 8);
            this.patientNameLabel.Name = "patientNameLabel";
            this.patientNameLabel.Size = new System.Drawing.Size(81, 16);
            this.patientNameLabel.TabIndex = 0;
            this.patientNameLabel.Text = "Patient Name:";
            // 
            // NonPurgedAccountDetailsHeader
            // 
            this.Controls.Add(this.NonPurgedAccountDetailsHeaderPanel);
            this.Name = "NonPurgedAccountDetailsHeader";
            this.Size = new System.Drawing.Size(680, 53);
            this.NonPurgedAccountDetailsHeaderPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		#region Public Methods

		public override void UpdateView()
		{
			this.DisplayPatientDetails();
		}

		#endregion

		#region Private Methods

		private void DisplayPatientDetails()
		{
			if( this.Model.AccountNumber > 0 )
			{
				accountValueLabel.Text = this.Model.AccountNumber.ToString();
			}
			Patient patient = this.Model.Patient;
			patientNameValueLabel.Text = patient.FormattedName;
			if( patient.Sex != null )
			{
				genderValueLabel.Text = patient.Sex.Description;
			}
			if( patient.DateOfBirth != DateTime.MinValue )
			{
				DOBValueLabel.Text = patient.DateOfBirth.ToString( "MM/dd/yyyy" );
			}
			if( patient.MedicalRecordNumber > 0 )
			{
				MRNValueLabel.Text = patient.MedicalRecordNumber.ToString();
			}
			if( patient.SocialSecurityNumber != null )
			{
				SSNValueLabel.Text = patient.SocialSecurityNumber.AsFormattedString();
			}
		
		}

		#endregion

		#region Properties

		public new Account Model
		{
			private get
			{
				return base.Model as Account;
			}
			set
			{
				base.Model = value;
			}
		}

		#endregion

		#region Data Elements

		private Label patientNameLabel;
		private Label patientNameValueLabel;
		private Label genderLabel;
		private Label genderValueLabel;
		private Label DOBValueLabel;
		private Label SSNLabel;
		private Label MRNLabel;
		private Label MRNValueLabel;
		private Label accountLabel;
		private Label accountValueLabel;
		private Label SSNValueLabel;
		private Panel NonPurgedAccountDetailsHeaderPanel;
		private Label DOBLabel;
		private Container components = null;

		#endregion

		#region Constants
		#endregion
	}
}
