using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.HistoricalAccountViews
{
	/// <summary>
	/// Summary description for HistoricalPatientDemographicsSummaryView.
	/// </summary>
    public class HistoricalPatientDemographicsSummaryView : ControlView
    {
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.DisplayDemographicDetails();
        }

        #endregion

        #region Properties
        public new AccountProxy Model
        {
            private get
            {
                return base.Model as AccountProxy;
            }
            set
            {
                base.Model = value;
            }
        }
        #endregion

        #region Private Methods
        
        private void DisplayDemographicDetails()
        {
            string addressLine1     = String.Empty;
            string addressLine2     = String.Empty;
            string phoneNumber      = String.Empty;
            string cellPhoneNumber  = String.Empty;

            Patient patient = this.Model.Patient;
           
            lastUpdatedValueLabel.Text = patient.LastUpdated.ToString( "MM/dd/yyyy" );

            patientNameValueLabel.Text = patient.FormattedName + LABEL_SPACE + patient.Suffix;
            
            if( patient.Aliases.Count > 0 )
            {
                aKAValueLabel.Text = ( (Name)patient.Aliases[FIRST_ALIAS] ).AsFormattedName();
            }
            if( patient.MedicalRecordNumber > 0 )
            {
                mRNValueLabel.Text = patient.MedicalRecordNumber.ToString();
            }
            if( patient.SocialSecurityNumber != null )
            {
                sSNValueLabel.Text = patient.SocialSecurityNumber.ToString();
            }           
            if( patient.Sex != null )
            {
                genderValueLabel.Text = patient.Sex.Description;
            }
            if( patient.DateOfBirth != DateTime.MinValue )
            {
                dOBValueLabel.Text = patient.DateOfBirth.ToString( "MM/dd/yyyy" );
            }

            currentAgeValueLabel.Text = 
                    patient.AgeAt( DateTime.Today ).PadLeft( 4, '0' ).ToUpper();

            if( patient.MaritalStatus != null )
            {
                maritalStatusValueLabel.Text = patient.MaritalStatus.Description;
            }

            if (!String.IsNullOrEmpty(patient.PrintRaceString))
            {
                raceValueLabel.Text = patient.PrintRaceString;
            }

            if (!String.IsNullOrEmpty(patient.PrintEthnicityString))
            {
                ethnicityValueLabel.Text = patient.PrintEthnicityString;
            }

            foreach( ContactPoint contactPoint in patient.ContactPoints )
            {
                addressLine1    = String.Empty;
                addressLine2    = String.Empty;
                phoneNumber     = String.Empty;
                cellPhoneNumber = String.Empty;

                if( contactPoint.Address != null )
                {
                    addressLine1 = contactPoint.Address.Address1;
                    addressLine2 = contactPoint.Address.City;
                    if( contactPoint.Address.State != null )
                    {
                        addressLine2 += ", " + contactPoint.Address.State.Code;
                    }
                    if( contactPoint.Address.ZipCode.ZipCodePrimary != "" )
                    {
                        addressLine2 += " " + contactPoint.Address.ZipCode.ZipCodePrimary;
                    }
                    if( contactPoint.Address.ZipCode.ZipCodeExtended != "" )
                    {
                        addressLine2 += "-" + contactPoint.Address.ZipCode.ZipCodeExtended;
                    }
                    if( contactPoint.PhoneNumber != null )
                    {
                        phoneNumber = contactPoint.PhoneNumber.AsFormattedString();
                    }
                    if( contactPoint.CellPhoneNumber != null )
                    {
                        cellPhoneNumber = contactPoint.CellPhoneNumber.AsFormattedString();
                    }
                }
                if( contactPoint.TypeOfContactPoint.Oid == TypeOfContactPoint.MAILING_OID )
                {
                    mailingStreetValueLabel.Text        = addressLine1;
                    mailingCityStateZipValueLabel.Text  = addressLine2;
                    mailingPhoneValueLabel.Text         = phoneNumber;
                    mailingCellValueLabel.Text          = cellPhoneNumber;
                }
                if( contactPoint.TypeOfContactPoint.Oid == TypeOfContactPoint.PHYSICAL_OID )
                {
                    physicalStreetValueLabel.Text       = addressLine1;
                    physicalCityStateZipValueLabel.Text = addressLine2;
                    physicalPhoneValueLabel.Text        = phoneNumber;
                }
            }
            if( patient.DriversLicense != null )
            {
                driverLicenseValueLabel.Text = patient.DriversLicense.Number;
                if( patient.DriversLicense.State != null )
                {
                    driverLicenseValueLabel.Text += " " + patient.DriversLicense.State.Code;
                }
            }
            if (patient.Passport != null)
            {
                passportValueLabel.Text = patient.Passport.Number;
                if (patient.Passport.Country != null)
                {
                    passportValueLabel.Text += " " + patient.Passport.Country.Code;
                }
            }
            maidenNameValueLabel.Text   = patient.MaidenName;
            motherNameValueLabel.Text   = patient.MothersName;
            fatherNameValueLabel.Text   = patient.FathersName;
            if( this.Model.Bloodless != null )
            {
                bloodlessValueLabel.Text    = this.Model.Bloodless.Code;
            }
            dNRValueLabel.Text          = patient.DoNotResuscitate.Code;
            if( patient.PreviousMRN > 0 )
            {
                previousMRNValueLabel.Text  = patient.PreviousMRN.ToString();
            }
            if( this.Model.Patient.NoticeOfPrivacyPracticeDocument != null &&
                this.Model.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion != null )
            {
                nPPValueLabel.Text          = this.Model.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion.DisplayString;
                if( this.Model.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus != null )
                {
                    if( this.Model.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus.Code == SignatureStatus.SIGNED &&
                        this.Model.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate != DateTime.MinValue )
                    {
                        nPPSignatureStatusValueLabel.Text = 
                            this.Model.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus.Description.Trim()
                            + LABEL_SPACE
                            + this.Model.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate.ToShortDateString();
                    }
                    else
                    {
                        nPPSignatureStatusValueLabel.Text = 
                            this.Model.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus.Description.Trim();
                    }
                }
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public HistoricalPatientDemographicsSummaryView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
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

        #region Component Designer generated code
        
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.demographicsHeaderLabel = new System.Windows.Forms.Label();
            this.demographicDetailsPanel = new System.Windows.Forms.Panel();
            this.nPPSignatureStatusValueLabel = new System.Windows.Forms.Label();
            this.nPPSignatureStatusLabel = new System.Windows.Forms.Label();
            this.previousMRNValueLabel = new System.Windows.Forms.Label();
            this.nPPValueLabel = new System.Windows.Forms.Label();
            this.previousMRNLabel = new System.Windows.Forms.Label();
            this.nPPLabel = new System.Windows.Forms.Label();
            this.dNRValueLabel = new System.Windows.Forms.Label();
            this.dNRLabel = new System.Windows.Forms.Label();
            this.fatherNameValueLabel = new System.Windows.Forms.Label();
            this.bloodlessValueLabel = new System.Windows.Forms.Label();
            this.fatherNameLabel = new System.Windows.Forms.Label();
            this.bloodlessLabel = new System.Windows.Forms.Label();
            this.physicalPhoneValueLabel = new System.Windows.Forms.Label();
            this.physicalPhoneLabel = new System.Windows.Forms.Label();
            this.physicalCityStateZipValueLabel = new System.Windows.Forms.Label();
            this.physicalStreetValueLabel = new System.Windows.Forms.Label();
            this.physicalAddressLabel = new System.Windows.Forms.Label();
            this.mailingCellValueLabel = new System.Windows.Forms.Label();
            this.mailingPhoneValueLabel = new System.Windows.Forms.Label();
            this.mailingCellLabel = new System.Windows.Forms.Label();
            this.mailingPhoneLabel = new System.Windows.Forms.Label();
            this.maidenNameValueLabel = new System.Windows.Forms.Label();
            this.motherNameValueLabel = new System.Windows.Forms.Label();
            this.driverLicenseValueLabel = new System.Windows.Forms.Label();
            this.driverLicenseLabel = new System.Windows.Forms.Label();
            this.passportValueLabel = new System.Windows.Forms.Label();
            this.passportLabel = new System.Windows.Forms.Label();
            this.maidenNameLabel = new System.Windows.Forms.Label();
            this.motherNameLabel = new System.Windows.Forms.Label();
            this.mailingCityStateZipValueLabel = new System.Windows.Forms.Label();
            this.mailingStreetValueLabel = new System.Windows.Forms.Label();
            this.mailingAddressLabel = new System.Windows.Forms.Label();
            this.ethnicityValueLabel = new System.Windows.Forms.Label();
            this.raceValueLabel = new System.Windows.Forms.Label();
            this.maritalStatusValueLabel = new System.Windows.Forms.Label();
            this.currentAgeValueLabel = new System.Windows.Forms.Label();
            this.dOBValueLabel = new System.Windows.Forms.Label();
            this.genderValueLabel = new System.Windows.Forms.Label();
            this.sSNValueLabel = new System.Windows.Forms.Label();
            this.mRNValueLabel = new System.Windows.Forms.Label();
            this.aKAValueLabel = new System.Windows.Forms.Label();
            this.patientNameValueLabel = new System.Windows.Forms.Label();
            this.ethnicityLabel = new System.Windows.Forms.Label();
            this.raceLabel = new System.Windows.Forms.Label();
            this.maritalStatusLabel = new System.Windows.Forms.Label();
            this.currentAgeLabel = new System.Windows.Forms.Label();
            this.dOBLabel = new System.Windows.Forms.Label();
            this.genderLabel = new System.Windows.Forms.Label();
            this.sSNLabel = new System.Windows.Forms.Label();
            this.mRNLabel = new System.Windows.Forms.Label();
            this.aKALabel = new System.Windows.Forms.Label();
            this.patientNameLabel = new System.Windows.Forms.Label();
            this.lastUpdatedLabel = new System.Windows.Forms.Label();
            this.lastUpdatedValueLabel = new System.Windows.Forms.Label();
            this.demographicDetailsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // demographicsHeaderLabel
            // 
            this.demographicsHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.demographicsHeaderLabel.Location = new System.Drawing.Point(14, 7);
            this.demographicsHeaderLabel.Name = "demographicsHeaderLabel";
            this.demographicsHeaderLabel.Size = new System.Drawing.Size(140, 16);
            this.demographicsHeaderLabel.TabIndex = 0;
            this.demographicsHeaderLabel.Text = "Demographics Summary";
            // 
            // demographicDetailsPanel
            // 
            this.demographicDetailsPanel.BackColor = System.Drawing.Color.White;
            this.demographicDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.demographicDetailsPanel.Controls.Add(this.nPPSignatureStatusValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.nPPSignatureStatusLabel);
            this.demographicDetailsPanel.Controls.Add(this.previousMRNValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.nPPValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.previousMRNLabel);
            this.demographicDetailsPanel.Controls.Add(this.nPPLabel);
            this.demographicDetailsPanel.Controls.Add(this.dNRValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.dNRLabel);
            this.demographicDetailsPanel.Controls.Add(this.fatherNameValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.bloodlessValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.fatherNameLabel);
            this.demographicDetailsPanel.Controls.Add(this.bloodlessLabel);
            this.demographicDetailsPanel.Controls.Add(this.physicalPhoneValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.physicalPhoneLabel);
            this.demographicDetailsPanel.Controls.Add(this.physicalCityStateZipValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.physicalStreetValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.physicalAddressLabel);
            this.demographicDetailsPanel.Controls.Add(this.mailingCellValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.mailingPhoneValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.mailingCellLabel);
            this.demographicDetailsPanel.Controls.Add(this.mailingPhoneLabel);
            this.demographicDetailsPanel.Controls.Add(this.maidenNameValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.motherNameValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.driverLicenseValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.driverLicenseLabel);
            this.demographicDetailsPanel.Controls.Add(this.passportValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.passportLabel);
            this.demographicDetailsPanel.Controls.Add(this.maidenNameLabel);
            this.demographicDetailsPanel.Controls.Add(this.motherNameLabel);
            this.demographicDetailsPanel.Controls.Add(this.mailingCityStateZipValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.mailingStreetValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.mailingAddressLabel);
            this.demographicDetailsPanel.Controls.Add(this.ethnicityValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.raceValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.maritalStatusValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.currentAgeValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.dOBValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.genderValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.sSNValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.mRNValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.aKAValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.patientNameValueLabel);
            this.demographicDetailsPanel.Controls.Add(this.ethnicityLabel);
            this.demographicDetailsPanel.Controls.Add(this.raceLabel);
            this.demographicDetailsPanel.Controls.Add(this.maritalStatusLabel);
            this.demographicDetailsPanel.Controls.Add(this.currentAgeLabel);
            this.demographicDetailsPanel.Controls.Add(this.dOBLabel);
            this.demographicDetailsPanel.Controls.Add(this.genderLabel);
            this.demographicDetailsPanel.Controls.Add(this.sSNLabel);
            this.demographicDetailsPanel.Controls.Add(this.mRNLabel);
            this.demographicDetailsPanel.Controls.Add(this.aKALabel);
            this.demographicDetailsPanel.Controls.Add(this.patientNameLabel);
            this.demographicDetailsPanel.Location = new System.Drawing.Point(7, 30);
            this.demographicDetailsPanel.Name = "demographicDetailsPanel";
            this.demographicDetailsPanel.Size = new System.Drawing.Size(866, 240);
            this.demographicDetailsPanel.TabIndex = 1;
            // 
            // nPPSignatureStatusValueLabel
            // 
            this.nPPSignatureStatusValueLabel.AutoSize = true;
            this.nPPSignatureStatusValueLabel.Location = new System.Drawing.Point(673, 217);
            this.nPPSignatureStatusValueLabel.Name = "nPPSignatureStatusValueLabel";
            this.nPPSignatureStatusValueLabel.Size = new System.Drawing.Size(0, 16);
            this.nPPSignatureStatusValueLabel.TabIndex = 51;
            // 
            // nPPSignatureStatusLabel
            // 
            this.nPPSignatureStatusLabel.AutoSize = true;
            this.nPPSignatureStatusLabel.Location = new System.Drawing.Point(566, 217);
            this.nPPSignatureStatusLabel.Name = "nPPSignatureStatusLabel";
            this.nPPSignatureStatusLabel.Size = new System.Drawing.Size(107, 16);
            this.nPPSignatureStatusLabel.TabIndex = 50;
            this.nPPSignatureStatusLabel.Text = "NPP signature status:";
            // 
            // previousMRNValueLabel
            // 
            this.previousMRNValueLabel.AutoSize = true;
            this.previousMRNValueLabel.Location = new System.Drawing.Point(673, 175);
            this.previousMRNValueLabel.Name = "previousMRNValueLabel";
            this.previousMRNValueLabel.Size = new System.Drawing.Size(0, 16);
            this.previousMRNValueLabel.TabIndex = 49;
            // 
            // nPPValueLabel
            // 
            this.nPPValueLabel.AutoSize = true;
            this.nPPValueLabel.Location = new System.Drawing.Point(673, 196);
            this.nPPValueLabel.Name = "nPPValueLabel";
            this.nPPValueLabel.Size = new System.Drawing.Size(0, 16);
            this.nPPValueLabel.TabIndex = 48;
            // 
            // previousMRNLabel
            // 
            this.previousMRNLabel.AutoSize = true;
            this.previousMRNLabel.Location = new System.Drawing.Point(566, 175);
            this.previousMRNLabel.Name = "previousMRNLabel";
            this.previousMRNLabel.Size = new System.Drawing.Size(80, 16);
            this.previousMRNLabel.TabIndex = 47;
            this.previousMRNLabel.Text = "Previous MRN:";
            // 
            // nPPLabel
            // 
            this.nPPLabel.AutoSize = true;
            this.nPPLabel.Location = new System.Drawing.Point(566, 196);
            this.nPPLabel.Name = "nPPLabel";
            this.nPPLabel.Size = new System.Drawing.Size(31, 16);
            this.nPPLabel.TabIndex = 46;
            this.nPPLabel.Text = "NPP:";
            // 
            // dNRValueLabel
            // 
            this.dNRValueLabel.AutoSize = true;
            this.dNRValueLabel.Location = new System.Drawing.Point(673, 154);
            this.dNRValueLabel.Name = "dNRValueLabel";
            this.dNRValueLabel.Size = new System.Drawing.Size(0, 16);
            this.dNRValueLabel.TabIndex = 45;
            // 
            // dNRLabel
            // 
            this.dNRLabel.AutoSize = true;
            this.dNRLabel.Location = new System.Drawing.Point(566, 154);
            this.dNRLabel.Name = "dNRLabel";
            this.dNRLabel.Size = new System.Drawing.Size(32, 16);
            this.dNRLabel.TabIndex = 44;
            this.dNRLabel.Text = "DNR:";
            // 
            // fatherNameValueLabel
            // 
            this.fatherNameValueLabel.AutoSize = true;
            this.fatherNameValueLabel.Location = new System.Drawing.Point(673, 112);
            this.fatherNameValueLabel.Name = "fatherNameValueLabel";
            this.fatherNameValueLabel.Size = new System.Drawing.Size(0, 16);
            this.fatherNameValueLabel.TabIndex = 43;
            // 
            // bloodlessValueLabel
            // 
            this.bloodlessValueLabel.AutoSize = true;
            this.bloodlessValueLabel.Location = new System.Drawing.Point(673, 133);
            this.bloodlessValueLabel.Name = "bloodlessValueLabel";
            this.bloodlessValueLabel.Size = new System.Drawing.Size(0, 16);
            this.bloodlessValueLabel.TabIndex = 42;
            // 
            // fatherNameLabel
            // 
            this.fatherNameLabel.AutoSize = true;
            this.fatherNameLabel.Location = new System.Drawing.Point(566, 112);
            this.fatherNameLabel.Name = "fatherNameLabel";
            this.fatherNameLabel.Size = new System.Drawing.Size(79, 16);
            this.fatherNameLabel.TabIndex = 41;
            this.fatherNameLabel.Text = "Father\'s name:";
            // 
            // bloodlessLabel
            // 
            this.bloodlessLabel.AutoSize = true;
            this.bloodlessLabel.Location = new System.Drawing.Point(566, 133);
            this.bloodlessLabel.Name = "bloodlessLabel";
            this.bloodlessLabel.Size = new System.Drawing.Size(57, 16);
            this.bloodlessLabel.TabIndex = 40;
            this.bloodlessLabel.Text = "Bloodless:";
            // 
            // physicalPhoneValueLabel
            // 
            this.physicalPhoneValueLabel.AutoSize = true;
            this.physicalPhoneValueLabel.Location = new System.Drawing.Point(425, 217);
            this.physicalPhoneValueLabel.Name = "physicalPhoneValueLabel";
            this.physicalPhoneValueLabel.Size = new System.Drawing.Size(0, 16);
            this.physicalPhoneValueLabel.TabIndex = 39;
            // 
            // physicalPhoneLabel
            // 
            this.physicalPhoneLabel.AutoSize = true;
            this.physicalPhoneLabel.Location = new System.Drawing.Point(385, 217);
            this.physicalPhoneLabel.Name = "physicalPhoneLabel";
            this.physicalPhoneLabel.Size = new System.Drawing.Size(40, 16);
            this.physicalPhoneLabel.TabIndex = 38;
            this.physicalPhoneLabel.Text = "Phone:";
            // 
            // physicalCityStateZipValueLabel
            // 
            this.physicalCityStateZipValueLabel.AutoSize = true;
            this.physicalCityStateZipValueLabel.Location = new System.Drawing.Point(359, 196);
            this.physicalCityStateZipValueLabel.Name = "physicalCityStateZipValueLabel";
            this.physicalCityStateZipValueLabel.Size = new System.Drawing.Size(0, 16);
            this.physicalCityStateZipValueLabel.TabIndex = 37;
            // 
            // physicalStreetValueLabel
            // 
            this.physicalStreetValueLabel.AutoSize = true;
            this.physicalStreetValueLabel.Location = new System.Drawing.Point(359, 175);
            this.physicalStreetValueLabel.Name = "physicalStreetValueLabel";
            this.physicalStreetValueLabel.Size = new System.Drawing.Size(0, 16);
            this.physicalStreetValueLabel.TabIndex = 36;
            // 
            // physicalAddressLabel
            // 
            this.physicalAddressLabel.AutoSize = true;
            this.physicalAddressLabel.Location = new System.Drawing.Point(270, 175);
            this.physicalAddressLabel.Name = "physicalAddressLabel";
            this.physicalAddressLabel.Size = new System.Drawing.Size(93, 16);
            this.physicalAddressLabel.TabIndex = 35;
            this.physicalAddressLabel.Text = "Physical address:";
            // 
            // mailingCellValueLabel
            // 
            this.mailingCellValueLabel.AutoSize = true;
            this.mailingCellValueLabel.Location = new System.Drawing.Point(425, 154);
            this.mailingCellValueLabel.Name = "mailingCellValueLabel";
            this.mailingCellValueLabel.Size = new System.Drawing.Size(0, 16);
            this.mailingCellValueLabel.TabIndex = 34;
            // 
            // mailingPhoneValueLabel
            // 
            this.mailingPhoneValueLabel.AutoSize = true;
            this.mailingPhoneValueLabel.Location = new System.Drawing.Point(425, 133);
            this.mailingPhoneValueLabel.Name = "mailingPhoneValueLabel";
            this.mailingPhoneValueLabel.Size = new System.Drawing.Size(0, 16);
            this.mailingPhoneValueLabel.TabIndex = 33;
            // 
            // mailingCellLabel
            // 
            this.mailingCellLabel.AutoSize = true;
            this.mailingCellLabel.Location = new System.Drawing.Point(385, 154);
            this.mailingCellLabel.Name = "mailingCellLabel";
            this.mailingCellLabel.Size = new System.Drawing.Size(27, 16);
            this.mailingCellLabel.TabIndex = 32;
            this.mailingCellLabel.Text = "Cell:";
            // 
            // mailingPhoneLabel
            // 
            this.mailingPhoneLabel.AutoSize = true;
            this.mailingPhoneLabel.Location = new System.Drawing.Point(385, 133);
            this.mailingPhoneLabel.Name = "mailingPhoneLabel";
            this.mailingPhoneLabel.Size = new System.Drawing.Size(40, 16);
            this.mailingPhoneLabel.TabIndex = 31;
            this.mailingPhoneLabel.Text = "Phone:";
            // 
            // maidenNameValueLabel
            // 
            this.maidenNameValueLabel.AutoSize = true;
            this.maidenNameValueLabel.Location = new System.Drawing.Point(673, 70);
            this.maidenNameValueLabel.Name = "maidenNameValueLabel";
            this.maidenNameValueLabel.Size = new System.Drawing.Size(0, 16);
            this.maidenNameValueLabel.TabIndex = 30;
            // 
            // motherNameValueLabel
            // 
            this.motherNameValueLabel.AutoSize = true;
            this.motherNameValueLabel.Location = new System.Drawing.Point(673, 91);
            this.motherNameValueLabel.Name = "motherNameValueLabel";
            this.motherNameValueLabel.Size = new System.Drawing.Size(0, 16);
            this.motherNameValueLabel.TabIndex = 29;
            // 
            // driverLicenseValueLabel
            // 
            this.driverLicenseValueLabel.AutoSize = true;
            this.driverLicenseValueLabel.Location = new System.Drawing.Point(673, 28);
            this.driverLicenseValueLabel.Name = "driverLicenseValueLabel";
            this.driverLicenseValueLabel.Size = new System.Drawing.Size(0, 16);
            this.driverLicenseValueLabel.TabIndex = 28;
            // 
            // driverLicenseLabel
            // 
            this.driverLicenseLabel.AutoSize = true;
            this.driverLicenseLabel.Location = new System.Drawing.Point(566, 28);
            this.driverLicenseLabel.Name = "driverLicenseLabel";
            this.driverLicenseLabel.Size = new System.Drawing.Size(107, 16);
            this.driverLicenseLabel.TabIndex = 27;
            this.driverLicenseLabel.Text = "U.S. driver\'s license:";
            // 
            // passportValueLabel
            // 
            this.passportValueLabel.AutoSize = true;
            this.passportValueLabel.Location = new System.Drawing.Point(673, 49);
            this.passportValueLabel.Name = "passportValueLabel";
            this.passportValueLabel.Size = new System.Drawing.Size(0, 16);
            this.passportValueLabel.TabIndex = 53;
            // 
            // passportLabel
            // 
            this.passportLabel.AutoSize = true;
            this.passportLabel.Location = new System.Drawing.Point(566, 49);
            this.passportLabel.Name = "passportLabel";
            this.passportLabel.Size = new System.Drawing.Size(107, 16);
            this.passportLabel.TabIndex = 52;
            this.passportLabel.Text = "Passport:";
            // 
            // maidenNameLabel
            // 
            this.maidenNameLabel.AutoSize = true;
            this.maidenNameLabel.Location = new System.Drawing.Point(566, 70);
            this.maidenNameLabel.Name = "maidenNameLabel";
            this.maidenNameLabel.Size = new System.Drawing.Size(76, 16);
            this.maidenNameLabel.TabIndex = 26;
            this.maidenNameLabel.Text = "Maiden name:";
            // 
            // motherNameLabel
            // 
            this.motherNameLabel.AutoSize = true;
            this.motherNameLabel.Location = new System.Drawing.Point(566, 91);
            this.motherNameLabel.Name = "motherNameLabel";
            this.motherNameLabel.Size = new System.Drawing.Size(82, 16);
            this.motherNameLabel.TabIndex = 25;
            this.motherNameLabel.Text = "Mother\'s name:";
            // 
            // mailingCityStateZipValueLabel
            // 
            this.mailingCityStateZipValueLabel.AutoSize = true;
            this.mailingCityStateZipValueLabel.Location = new System.Drawing.Point(359, 112);
            this.mailingCityStateZipValueLabel.Name = "mailingCityStateZipValueLabel";
            this.mailingCityStateZipValueLabel.Size = new System.Drawing.Size(0, 16);
            this.mailingCityStateZipValueLabel.TabIndex = 24;
            // 
            // mailingStreetValueLabel
            // 
            this.mailingStreetValueLabel.AutoSize = true;
            this.mailingStreetValueLabel.BackColor = System.Drawing.Color.White;
            this.mailingStreetValueLabel.Location = new System.Drawing.Point(359, 91);
            this.mailingStreetValueLabel.Name = "mailingStreetValueLabel";
            this.mailingStreetValueLabel.Size = new System.Drawing.Size(0, 16);
            this.mailingStreetValueLabel.TabIndex = 23;
            // 
            // mailingAddressLabel
            // 
            this.mailingAddressLabel.AutoSize = true;
            this.mailingAddressLabel.Location = new System.Drawing.Point(270, 91);
            this.mailingAddressLabel.Name = "mailingAddressLabel";
            this.mailingAddressLabel.Size = new System.Drawing.Size(87, 16);
            this.mailingAddressLabel.TabIndex = 22;
            this.mailingAddressLabel.Text = "Mailing address:";
            // 
            // ethnicityValueLabel
            // 
            this.ethnicityValueLabel.AutoSize = true;
            this.ethnicityValueLabel.Location = new System.Drawing.Point(82, 196);
            this.ethnicityValueLabel.Name = "ethnicityValueLabel";
            this.ethnicityValueLabel.Size = new System.Drawing.Size(0, 16);
            this.ethnicityValueLabel.TabIndex = 21;
            // 
            // raceValueLabel
            // 
            this.raceValueLabel.AutoSize = true;
            this.raceValueLabel.Location = new System.Drawing.Point(82, 175);
            this.raceValueLabel.Name = "raceValueLabel";
            this.raceValueLabel.Size = new System.Drawing.Size(0, 16);
            this.raceValueLabel.TabIndex = 20;
            // 
            // maritalStatusValueLabel
            // 
            this.maritalStatusValueLabel.AutoSize = true;
            this.maritalStatusValueLabel.Location = new System.Drawing.Point(82, 154);
            this.maritalStatusValueLabel.Name = "maritalStatusValueLabel";
            this.maritalStatusValueLabel.Size = new System.Drawing.Size(0, 16);
            this.maritalStatusValueLabel.TabIndex = 19;
            // 
            // currentAgeValueLabel
            // 
            this.currentAgeValueLabel.AutoSize = true;
            this.currentAgeValueLabel.Location = new System.Drawing.Point(82, 133);
            this.currentAgeValueLabel.Name = "currentAgeValueLabel";
            this.currentAgeValueLabel.Size = new System.Drawing.Size(0, 16);
            this.currentAgeValueLabel.TabIndex = 18;
            // 
            // dOBValueLabel
            // 
            this.dOBValueLabel.AutoSize = true;
            this.dOBValueLabel.Location = new System.Drawing.Point(82, 112);
            this.dOBValueLabel.Name = "dOBValueLabel";
            this.dOBValueLabel.Size = new System.Drawing.Size(0, 16);
            this.dOBValueLabel.TabIndex = 17;
            // 
            // genderValueLabel
            // 
            this.genderValueLabel.AutoSize = true;
            this.genderValueLabel.Location = new System.Drawing.Point(82, 91);
            this.genderValueLabel.Name = "genderValueLabel";
            this.genderValueLabel.Size = new System.Drawing.Size(0, 16);
            this.genderValueLabel.TabIndex = 16;
            // 
            // sSNValueLabel
            // 
            this.sSNValueLabel.AutoSize = true;
            this.sSNValueLabel.Location = new System.Drawing.Point(82, 70);
            this.sSNValueLabel.Name = "sSNValueLabel";
            this.sSNValueLabel.Size = new System.Drawing.Size(0, 16);
            this.sSNValueLabel.TabIndex = 15;
            // 
            // mRNValueLabel
            // 
            this.mRNValueLabel.AutoSize = true;
            this.mRNValueLabel.Location = new System.Drawing.Point(82, 49);
            this.mRNValueLabel.Name = "mRNValueLabel";
            this.mRNValueLabel.Size = new System.Drawing.Size(0, 16);
            this.mRNValueLabel.TabIndex = 14;
            // 
            // aKAValueLabel
            // 
            this.aKAValueLabel.AutoSize = true;
            this.aKAValueLabel.Location = new System.Drawing.Point(82, 30);
            this.aKAValueLabel.Name = "aKAValueLabel";
            this.aKAValueLabel.Size = new System.Drawing.Size(0, 16);
            this.aKAValueLabel.TabIndex = 13;
            // 
            // patientNameValueLabel
            // 
            this.patientNameValueLabel.AutoSize = true;
            this.patientNameValueLabel.Location = new System.Drawing.Point(82, 7);
            this.patientNameValueLabel.Name = "patientNameValueLabel";
            this.patientNameValueLabel.Size = new System.Drawing.Size(0, 16);
            this.patientNameValueLabel.TabIndex = 11;
            // 
            // ethnicityLabel
            // 
            this.ethnicityLabel.AutoSize = true;
            this.ethnicityLabel.Location = new System.Drawing.Point(7, 196);
            this.ethnicityLabel.Name = "ethnicityLabel";
            this.ethnicityLabel.Size = new System.Drawing.Size(50, 16);
            this.ethnicityLabel.TabIndex = 10;
            this.ethnicityLabel.Text = "Ethnicity:";
            // 
            // raceLabel
            // 
            this.raceLabel.AutoSize = true;
            this.raceLabel.Location = new System.Drawing.Point(7, 175);
            this.raceLabel.Name = "raceLabel";
            this.raceLabel.Size = new System.Drawing.Size(34, 16);
            this.raceLabel.TabIndex = 9;
            this.raceLabel.Text = "Race:";
            // 
            // maritalStatusLabel
            // 
            this.maritalStatusLabel.AutoSize = true;
            this.maritalStatusLabel.Location = new System.Drawing.Point(7, 154);
            this.maritalStatusLabel.Name = "maritalStatusLabel";
            this.maritalStatusLabel.Size = new System.Drawing.Size(75, 16);
            this.maritalStatusLabel.TabIndex = 8;
            this.maritalStatusLabel.Text = "Marital status:";
            // 
            // currentAgeLabel
            // 
            this.currentAgeLabel.AutoSize = true;
            this.currentAgeLabel.Location = new System.Drawing.Point(7, 133);
            this.currentAgeLabel.Name = "currentAgeLabel";
            this.currentAgeLabel.Size = new System.Drawing.Size(67, 16);
            this.currentAgeLabel.TabIndex = 7;
            this.currentAgeLabel.Text = "Current age:";
            // 
            // dOBLabel
            // 
            this.dOBLabel.AutoSize = true;
            this.dOBLabel.Location = new System.Drawing.Point(7, 112);
            this.dOBLabel.Name = "dOBLabel";
            this.dOBLabel.Size = new System.Drawing.Size(32, 16);
            this.dOBLabel.TabIndex = 6;
            this.dOBLabel.Text = "DOB:";
            // 
            // genderLabel
            // 
            this.genderLabel.AutoSize = true;
            this.genderLabel.Location = new System.Drawing.Point(7, 91);
            this.genderLabel.Name = "genderLabel";
            this.genderLabel.Size = new System.Drawing.Size(45, 16);
            this.genderLabel.TabIndex = 5;
            this.genderLabel.Text = "Gender:";
            // 
            // sSNLabel
            // 
            this.sSNLabel.AutoSize = true;
            this.sSNLabel.Location = new System.Drawing.Point(7, 70);
            this.sSNLabel.Name = "sSNLabel";
            this.sSNLabel.Size = new System.Drawing.Size(31, 16);
            this.sSNLabel.TabIndex = 4;
            this.sSNLabel.Text = "SSN:";
            // 
            // mRNLabel
            // 
            this.mRNLabel.AutoSize = true;
            this.mRNLabel.Location = new System.Drawing.Point(7, 49);
            this.mRNLabel.Name = "mRNLabel";
            this.mRNLabel.Size = new System.Drawing.Size(33, 16);
            this.mRNLabel.TabIndex = 3;
            this.mRNLabel.Text = "MRN:";
            // 
            // aKALabel
            // 
            this.aKALabel.AutoSize = true;
            this.aKALabel.Location = new System.Drawing.Point(7, 30);
            this.aKALabel.Name = "aKALabel";
            this.aKALabel.Size = new System.Drawing.Size(30, 16);
            this.aKALabel.TabIndex = 2;
            this.aKALabel.Text = "AKA:";
            // 
            // patientNameLabel
            // 
            this.patientNameLabel.AutoSize = true;
            this.patientNameLabel.Location = new System.Drawing.Point(7, 7);
            this.patientNameLabel.Name = "patientNameLabel";
            this.patientNameLabel.Size = new System.Drawing.Size(74, 16);
            this.patientNameLabel.TabIndex = 0;
            this.patientNameLabel.Text = "Patient name:";
            // 
            // lastUpdatedLabel
            // 
            this.lastUpdatedLabel.AutoSize = true;
            this.lastUpdatedLabel.Location = new System.Drawing.Point(732, 7);
            this.lastUpdatedLabel.Name = "lastUpdatedLabel";
            this.lastUpdatedLabel.Size = new System.Drawing.Size(73, 16);
            this.lastUpdatedLabel.TabIndex = 2;
            this.lastUpdatedLabel.Text = "Last updated:";
            // 
            // lastUpdatedValueLabel
            // 
            this.lastUpdatedValueLabel.AutoSize = true;
            this.lastUpdatedValueLabel.Location = new System.Drawing.Point(805, 7);
            this.lastUpdatedValueLabel.Name = "lastUpdatedValueLabel";
            this.lastUpdatedValueLabel.Size = new System.Drawing.Size(0, 16);
            this.lastUpdatedValueLabel.TabIndex = 3;
            // 
            // HistoricalPatientDemographicsSummaryView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.lastUpdatedValueLabel);
            this.Controls.Add(this.lastUpdatedLabel);
            this.Controls.Add(this.demographicDetailsPanel);
            this.Controls.Add(this.demographicsHeaderLabel);
            this.Name = "HistoricalPatientDemographicsSummaryView";
            this.Size = new System.Drawing.Size(880, 277);
            this.demographicDetailsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        
        #endregion

        #region Data Elements

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private Label demographicsHeaderLabel;
        private Label patientNameLabel;
        private Label maritalStatusLabel;
        private Label raceLabel;
        private Label aKALabel;
        private Label mRNLabel;
        private Label sSNLabel;
        private Label genderLabel;
        private Label dOBLabel;
        private Panel demographicDetailsPanel;
        private Label currentAgeLabel;
        private Label ethnicityLabel;
        private Label ethnicityValueLabel;
        private Label raceValueLabel;
        private Label maritalStatusValueLabel;
        private Label currentAgeValueLabel;
        private Label dOBValueLabel;
        private Label genderValueLabel;
        private Label sSNValueLabel;
        private Label mRNValueLabel;
        private Label aKAValueLabel;
        private Label patientNameValueLabel;
        private Label mailingAddressLabel;
        private Label motherNameLabel;
        private Label maidenNameLabel;
        private Label driverLicenseLabel;
        private Label driverLicenseValueLabel;
        private Label passportLabel;
        private Label passportValueLabel;
        private Label mailingCityStateZipValueLabel;
        private Label maidenNameValueLabel;
        private Label motherNameValueLabel;
        private Label mailingPhoneLabel;
        private Label mailingCellLabel;
        private Label mailingPhoneValueLabel;
        private Label mailingCellValueLabel;
        private Label physicalPhoneValueLabel;
        private Label physicalPhoneLabel;
        private Label physicalCityStateZipValueLabel;
        private Label physicalStreetValueLabel;
        private Label physicalAddressLabel;
        private Label fatherNameValueLabel;
        private Label bloodlessValueLabel;
        private Label fatherNameLabel;
        private Label bloodlessLabel;
        private Label previousMRNValueLabel;
        private Label nPPValueLabel;
        private Label previousMRNLabel;
        private Label nPPLabel;
        private Label dNRValueLabel;
        private Label dNRLabel;
        private Label lastUpdatedLabel;
        private Label lastUpdatedValueLabel;
        private Label mailingStreetValueLabel;
        private Label nPPSignatureStatusLabel;
        private Label nPPSignatureStatusValueLabel;

        #endregion

        #region Constants

        private const int FIRST_ALIAS = 0;
        private const string LABEL_SPACE = " ";
        #endregion

    }
}
