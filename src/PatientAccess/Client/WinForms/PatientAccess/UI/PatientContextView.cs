using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI
{
//TODO: Create XML summary comment for PatientContextView
    [Serializable]
    public class PatientContextView : ControlView
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
				}

        public override void UpdateView()
        {
            //AbstractPatient patient = this.Model as AbstractPatient;
            var patient = this.Model as Patient;
            if (patient != null && patient.PatientContextHeaderData != null && IsEMPIFeatureEnabled )
            {
                UpdateDisplayPatientData(patient);
            }
            else if(patient != null)
            {
                UpdatePatientData(patient);
            }
       
            
            if( i_Account != null )
            {
                ViewFactory.Instance.CreateView<PatientAccessView>().Model = this.Account;
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( true );

                this.i_AccountNumberLabel.Visible = true;
                this.i_AccountNumber.Visible      = true;
                if( i_Account.AccountNumber == 0 )
                {
                    this.i_AccountNumber.Text = String.Empty;
                }
                else
                {
                    this.i_AccountNumber.Text         = this.i_Account.AccountNumber.ToString();
                }
            }
            else
            {
                this.i_AccountNumberLabel.Visible = false;
                this.i_AccountNumber.Visible      = false;
            }

            base.UpdateView();
        }
        private void UpdatePatientData(Patient patient)
        {
            if (patient != null)
            {
                this.i_PatientName.Text = patient.Name.AsFormattedName();
                this.i_Gender.Text = patient.Sex.Description;
                this.i_DateOfBirth.Text = String.Empty;
                if (patient.DateOfBirth != DateTime.MinValue)
                {
                    this.i_DateOfBirth.Text = patient.DateOfBirth.ToString("MM/dd/yyyy");
                }

                this.i_SocialSecurityNumber.Text = patient.SocialSecurityNumber.AsFormattedString();
                if (patient.MedicalRecordNumber == 0)
                {
                    this.i_MedicalRecordNumber.Text = String.Empty;
                }
                else
                {
                    this.i_MedicalRecordNumber.Text = patient.MedicalRecordNumber.ToString();
                }
            }

            if (patient != null && patient.HasAliases())
            {
                this.i_AKA.Text = ((Name)patient.Aliases[FIRST_ALIAS]).AsFormattedName();
            }
            else
            {
                this.i_AKA.Text = string.Empty;
            }
        }
        private void UpdateDisplayPatientData(Patient patient)
        {
            if (patient != null)
            {
                this.i_PatientName.Text = patient.PatientContextHeaderData.PatientName.AsFormattedName();
                this.i_Gender.Text = patient.PatientContextHeaderData.Sex.Description;
                this.i_DateOfBirth.Text = String.Empty;
                if (patient.PatientContextHeaderData.DOB != DateTime.MinValue)
                {
                    this.i_DateOfBirth.Text = patient.PatientContextHeaderData.DOB.ToString("MM/dd/yyyy");
                }

                this.i_SocialSecurityNumber.Text = patient.PatientContextHeaderData.SSN.AsFormattedString();
                if (patient.PatientContextHeaderData.MRN == 0)
                {
                    this.i_MedicalRecordNumber.Text = String.Empty;
                }
                else
                {
                    this.i_MedicalRecordNumber.Text = patient.PatientContextHeaderData.MRN.ToString();
                }
            }

            if (patient != null && patient.PatientContextHeaderData.AKA != String.Empty)
            {
                this.i_AKA.Text = patient.PatientContextHeaderData.AKA;
            }
            else
            {
                this.i_AKA.Text = string.Empty;
            }
        }
        #endregion

        #region Properties
        public IAccount Account
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }
		public string GenderLabelText
		{
			get
			{
				return i_Gender.Text;
			}
			set
			{
				i_Gender.Text = value;
			}
		}
		public string PatientNameText
		{
			get
			{
				return i_PatientName.Text;
			}
			set
			{
				i_PatientName.Text = value;
			}
		}
		public string DateOfBirthText
		{
			get
			{
				return i_DateOfBirth.Text;
			}
			set
			{
				i_DateOfBirth.Text = value;
			}
		}
        private bool IsEMPIFeatureEnabled
        {
            get{ EMPIFeatureManager = new EMPIFeatureManager();
                return this.i_Account != null && i_Account.Activity != null &&
                       EMPIFeatureManager.IsEMPIFeatureEnabled(i_Account.Activity);
            }
        }
		public string SocialSecurityNumber
		{
			get
			{
				return i_SocialSecurityNumber.Text;
			}
			set
			{
				i_SocialSecurityNumber.Text = value;
			}
		}
		
        #endregion

        #region Private Methods
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.i_PatientNameLabel = new System.Windows.Forms.Label();
            this.i_PatientName = new System.Windows.Forms.Label();
            this.i_GenderLabel = new System.Windows.Forms.Label();
            this.i_Gender = new System.Windows.Forms.Label();
            this.i_DateOfBirthLabel = new System.Windows.Forms.Label();
            this.i_DateOfBirth = new System.Windows.Forms.Label();
            this.i_SocialSecurityNumberLabel = new System.Windows.Forms.Label();
            this.i_SocialSecurityNumber = new System.Windows.Forms.Label();
            this.i_MedicalRecordNumberLabel = new System.Windows.Forms.Label();
            this.i_MedicalRecordNumber = new System.Windows.Forms.Label();
            this.i_AccountNumber = new System.Windows.Forms.Label();
            this.i_AccountNumberLabel = new System.Windows.Forms.Label();
            this.i_AKALabel = new System.Windows.Forms.Label();
            this.i_AKA = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // i_PatientNameLabel
            // 
            this.i_PatientNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.i_PatientNameLabel.Location = new System.Drawing.Point(3, 6);
            this.i_PatientNameLabel.Name = "i_PatientNameLabel";
            this.i_PatientNameLabel.Size = new System.Drawing.Size(81, 16);
            this.i_PatientNameLabel.TabIndex = 0;
            this.i_PatientNameLabel.Text = "Patient name:";
            // 
            // i_PatientName
            // 
            this.i_PatientName.Location = new System.Drawing.Point(88, 6);
            this.i_PatientName.Name = "i_PatientName";
            this.i_PatientName.Size = new System.Drawing.Size(268, 16);
            this.i_PatientName.TabIndex = 1;
            // 
            // i_GenderLabel
            // 
            this.i_GenderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.i_GenderLabel.Location = new System.Drawing.Point(372, 6);
            this.i_GenderLabel.Name = "i_GenderLabel";
            this.i_GenderLabel.Size = new System.Drawing.Size(49, 16);
            this.i_GenderLabel.TabIndex = 2;
            this.i_GenderLabel.Text = "Gender:";
            // 
            // i_Gender
            // 
            this.i_Gender.Location = new System.Drawing.Point(421, 6);
            this.i_Gender.Name = "i_Gender";
            this.i_Gender.Size = new System.Drawing.Size(61, 16);
            this.i_Gender.TabIndex = 3;
            // 
            // i_DateOfBirthLabel
            // 
            this.i_DateOfBirthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.i_DateOfBirthLabel.Location = new System.Drawing.Point(499, 6);
            this.i_DateOfBirthLabel.Name = "i_DateOfBirthLabel";
            this.i_DateOfBirthLabel.Size = new System.Drawing.Size(34, 16);
            this.i_DateOfBirthLabel.TabIndex = 4;
            this.i_DateOfBirthLabel.Text = "DOB:";
            this.i_DateOfBirthLabel.Click += new System.EventHandler(this.i_DateOfBirthLabel_Click);
            // 
            // i_DateOfBirth
            // 
            this.i_DateOfBirth.Location = new System.Drawing.Point(533, 6);
            this.i_DateOfBirth.Name = "i_DateOfBirth";
            this.i_DateOfBirth.Size = new System.Drawing.Size(76, 16);
            this.i_DateOfBirth.TabIndex = 5;
            // 
            // i_SocialSecurityNumberLabel
            // 
            this.i_SocialSecurityNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.i_SocialSecurityNumberLabel.Location = new System.Drawing.Point(621, 6);
            this.i_SocialSecurityNumberLabel.Name = "i_SocialSecurityNumberLabel";
            this.i_SocialSecurityNumberLabel.Size = new System.Drawing.Size(34, 16);
            this.i_SocialSecurityNumberLabel.TabIndex = 6;
            this.i_SocialSecurityNumberLabel.Text = "SSN:";
            // 
            // i_SocialSecurityNumber
            // 
            this.i_SocialSecurityNumber.Location = new System.Drawing.Point(656, 6);
            this.i_SocialSecurityNumber.Name = "i_SocialSecurityNumber";
            this.i_SocialSecurityNumber.Size = new System.Drawing.Size(68, 16);
            this.i_SocialSecurityNumber.TabIndex = 7;
            // 
            // i_MedicalRecordNumberLabel
            // 
            this.i_MedicalRecordNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.i_MedicalRecordNumberLabel.Location = new System.Drawing.Point(745, 6);
            this.i_MedicalRecordNumberLabel.Name = "i_MedicalRecordNumberLabel";
            this.i_MedicalRecordNumberLabel.Size = new System.Drawing.Size(38, 16);
            this.i_MedicalRecordNumberLabel.TabIndex = 8;
            this.i_MedicalRecordNumberLabel.Text = "MRN:";
            // 
            // i_MedicalRecordNumber
            // 
            this.i_MedicalRecordNumber.Location = new System.Drawing.Point(781, 6);
            this.i_MedicalRecordNumber.Name = "i_MedicalRecordNumber";
            this.i_MedicalRecordNumber.Size = new System.Drawing.Size(61, 16);
            this.i_MedicalRecordNumber.TabIndex = 9;
            // 
            // i_AccountNumber
            // 
            this.i_AccountNumber.Location = new System.Drawing.Point(899, 6);
            this.i_AccountNumber.Name = "i_AccountNumber";
            this.i_AccountNumber.Size = new System.Drawing.Size(66, 16);
            this.i_AccountNumber.TabIndex = 10;
            this.i_AccountNumber.Click += new System.EventHandler(this.i_AccountNumber_Click);
            // 
            // i_AccountNumberLabel
            // 
            this.i_AccountNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.i_AccountNumberLabel.Location = new System.Drawing.Point(844, 6);
            this.i_AccountNumberLabel.Name = "i_AccountNumberLabel";
            this.i_AccountNumberLabel.Size = new System.Drawing.Size(55, 16);
            this.i_AccountNumberLabel.TabIndex = 11;
            this.i_AccountNumberLabel.Text = "Account:";
            // 
            // i_AKALabel
            // 
            this.i_AKALabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.i_AKALabel.Location = new System.Drawing.Point(3, 30);
            this.i_AKALabel.Name = "i_AKALabel";
            this.i_AKALabel.Size = new System.Drawing.Size(31, 16);
            this.i_AKALabel.TabIndex = 12;
            this.i_AKALabel.Text = "AKA:";
            // 
            // i_AKA
            // 
            this.i_AKA.Location = new System.Drawing.Point(38, 30);
            this.i_AKA.Name = "i_AKA";
            this.i_AKA.Size = new System.Drawing.Size(303, 16);
            this.i_AKA.TabIndex = 13;
            // 
            // PatientContextView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.i_AKA);
            this.Controls.Add(this.i_AKALabel);
            this.Controls.Add(this.i_Gender);
            this.Controls.Add(this.i_GenderLabel);
            this.Controls.Add(this.i_AccountNumber);
            this.Controls.Add(this.i_AccountNumberLabel);
            this.Controls.Add(this.i_MedicalRecordNumber);
            this.Controls.Add(this.i_MedicalRecordNumberLabel);
            this.Controls.Add(this.i_SocialSecurityNumber);
            this.Controls.Add(this.i_SocialSecurityNumberLabel);
            this.Controls.Add(this.i_DateOfBirth);
            this.Controls.Add(this.i_DateOfBirthLabel);
            this.Controls.Add(this.i_PatientName);
            this.Controls.Add(this.i_PatientNameLabel);
            this.Name = "PatientContextView";
            this.Size = new System.Drawing.Size(979, 53);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        private IEMPIFeatureManager EMPIFeatureManager { get; set; }
        #endregion

        #region Construction and Finalization
        public PatientContextView()
        {
            this.InitializeComponent();
        }

        public PatientContextView( AbstractPatient aPatient ) : this()
        {
            this.Model = aPatient;
            this.UpdateView();
        }

        public PatientContextView( AbstractPatient aPatient, IAccount currentAccount )
            : this( aPatient )
        {
            this.i_Account = currentAccount;
        }
        #endregion

        #region Data Elements
        private IAccount                   i_Account;
        private Label i_PatientNameLabel;
        private Label i_PatientName;
        private Label i_GenderLabel;
        private Label i_Gender;
        private Label i_DateOfBirthLabel;
        private Label i_DateOfBirth;
        private Label i_SocialSecurityNumberLabel;
        private Label i_SocialSecurityNumber;
        private Label i_MedicalRecordNumberLabel;
        private Label i_MedicalRecordNumber;
        private Label i_AccountNumber;
        private Label i_AccountNumberLabel;
        private Label i_AKALabel;
        private Label i_AKA;
        private Container components = null;
        #endregion

        #region Constants
        private const int FIRST_ALIAS = 0;
        #endregion

		private void i_DateOfBirthLabel_Click(object sender, EventArgs e)
		{
		
		}

		private void i_AccountNumber_Click(object sender, EventArgs e)
		{
		
		}
    }
}
