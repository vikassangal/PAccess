using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace PatientAccess.UI.HistoricalAccountViews
{
	/// <summary>
	/// Summary description for HistoricalPatientAccountSummaryView.
	/// </summary>
	public class HistoricalPatientAccountSummaryView : ControlView
	{
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override void UpdateView()
        {
           
            this.DisplayAccountDetails();
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private void DisplayAccountDetails()
        {
            int clinicCount = 0;

            AccountProxy accountProxy = this.Model as AccountProxy;
            var primaryCarePhysicianFeatureManager = new PrimaryCarePhysicianFeatureManager();
            if (primaryCarePhysicianFeatureManager.IsPrimaryCarePhysicianEnabledForDate(accountProxy.AccountCreatedDate))
            {
                this.pcpPhysicianLabel.Text = PRIMARYCAREPHYSICIAN_LABEL;
            }
            else
            {
                this.pcpPhysicianLabel.Text = OTHERPHYSICIAN_LABEL;
            }
            lastUpdatedValueLabel.Text  = accountProxy.LastUpdated.ToString( "MM/dd/yyyy" );          
            if( accountProxy.AccountNumber > 0 )
            {
                accountValueLabel.Text      = accountProxy.AccountNumber.ToString();
            }
            confidentialValueLabel.Text = accountProxy.ConfidentialFlag;
            ageAtVisitValueLabel.Text   = accountProxy.Patient.
                            AgeAt( accountProxy.AdmitDate ).PadLeft( 4, '0').ToUpper();
            if( accountProxy.KindOfVisit != null )
            {
                patientTypeValueLabel.Text = accountProxy.KindOfVisit.ToCodedString();
            }
            if( accountProxy.HospitalService != null )
            {
                hospitalServiceValueLabel.Text = accountProxy.HospitalService.ToCodedString();
            }
            if( accountProxy.Diagnosis != null )
            {
                chiefComplaintValueLabel.Text = accountProxy.Diagnosis.ChiefComplaint;
            }
            if( accountProxy.AdmitDate != DateTime.MinValue )
            {
                admitDateValueLabel.Text = accountProxy.AdmitDate.ToString( "MM/dd/yyyy" );
                admitTimeValueLabel.Text = accountProxy.AdmitDate.ToString( "HH:mm" );
            }
            if( accountProxy.AdmitSource != null )
            {
                admitSourceValueLabel.Text = accountProxy.AdmitSource.ToCodedString();
            }
            if( accountProxy.DischargeDate != DateTime.MinValue )
            {
                dischargeDateValueLabel.Text = accountProxy.DischargeDate.ToString( "MM/dd/yyyy" );
                dischargeTimeValueLabel.Text = accountProxy.DischargeDate.ToString( "HH:mm" );
            }
            if( accountProxy.DischargeDisposition != null )
            {
                dischargeDispositionValueLabel.Text = accountProxy.DischargeDisposition.ToCodedString();
            }
            
            if( accountProxy.ReferringPhysician != null )
            {
                refPhysicianValueLabel.Text = String.Format( "{0} {1}", 
                    accountProxy.ReferringPhysician.PhysicianNumber,
                    accountProxy.ReferringPhysician.FormattedName );
            }
            if( accountProxy.AttendingPhysician != null )
            {
                attPhysicianValueLabel.Text = String.Format( "{0} {1}", 
                    accountProxy.AttendingPhysician.PhysicianNumber,
                    accountProxy.AttendingPhysician.FormattedName );
            }
            if( accountProxy.AdmittingPhysician != null )
            {
                admPhysicianValueLabel.Text = String.Format( "{0} {1}", 
                    accountProxy.AdmittingPhysician.PhysicianNumber,
                    accountProxy.AdmittingPhysician.FormattedName );
            }
            if( accountProxy.OperatingPhysician != null )
            {
                oprPhysicianValueLabel.Text = String.Format( "{0} {1}", 
                    accountProxy.OperatingPhysician.PhysicianNumber,
                    accountProxy.OperatingPhysician.FormattedName );
            }
            if( accountProxy.PrimaryCarePhysician != null )
            {
                pcpPhysicianValueLabel.Text = String.Format( "{0} {1}", 
                    accountProxy.PrimaryCarePhysician.PhysicianNumber,
                    accountProxy.PrimaryCarePhysician.FormattedName );
            }
            seenInEDValueLabel.Text = accountProxy.SeenInED;

            foreach( HospitalClinic hospitalClinic in accountProxy.HospitalClinics )
            {
                if( hospitalClinic != null )
                {
                    clinicValueLabel[clinicCount].Text = hospitalClinic.ToCodedString();
                    clinicCount++;
                }
            }
            if( accountProxy.MultiSiteFlag.ToUpper() == "Y" )
            {
                siteValueLabel.Text = accountProxy.SiteCode;
            }           
            if( accountProxy.FinancialClass != null )
            {
                financialClassValueLabel.Text = accountProxy.FinancialClass.ToCodedString();
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public HistoricalPatientAccountSummaryView()
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
            this.accountDetailsPanel = new System.Windows.Forms.Panel();
            this.ageAtVisitValueLabel = new System.Windows.Forms.Label();
            this.chiefComplaintValueLabel = new System.Windows.Forms.Label();
            this.hospitalServiceValueLabel = new System.Windows.Forms.Label();
            this.patientTypeValueLabel = new System.Windows.Forms.Label();
            this.confidentialValueLabel = new System.Windows.Forms.Label();
            this.accountValueLabel = new System.Windows.Forms.Label();
            this.admitDateValueLabel = new System.Windows.Forms.Label();
            this.dischargeDispositionValueLabel = new System.Windows.Forms.Label();
            this.dischargeDateValueLabel = new System.Windows.Forms.Label();
            this.admitSourceValueLabel = new System.Windows.Forms.Label();
            this.financialClassValueLabel = new System.Windows.Forms.Label();
            this.siteValueLabel = new System.Windows.Forms.Label();
            this.clinicValueLabel = new System.Windows.Forms.Label[5];
            this.clinicValueLabel[0] = new System.Windows.Forms.Label();
            this.clinicValueLabel[1] = new System.Windows.Forms.Label();
            this.clinicValueLabel[2] = new System.Windows.Forms.Label();
            this.clinicValueLabel[3] = new System.Windows.Forms.Label();
            this.clinicValueLabel[4] = new System.Windows.Forms.Label();
            this.seenInEDValueLabel = new System.Windows.Forms.Label();
            this.pcpPhysicianValueLabel = new System.Windows.Forms.Label();
            this.oprPhysicianValueLabel = new System.Windows.Forms.Label();
            this.admPhysicianValueLabel = new System.Windows.Forms.Label();
            this.attPhysicianValueLabel = new System.Windows.Forms.Label();
            this.refPhysicianValueLabel = new System.Windows.Forms.Label();
            this.clinic3Label = new System.Windows.Forms.Label();
            this.clinic2Label = new System.Windows.Forms.Label();
            this.clinic1Label = new System.Windows.Forms.Label();
            this.clinic4Label = new System.Windows.Forms.Label();
            this.financialClassLabel = new System.Windows.Forms.Label();
            this.siteLabel = new System.Windows.Forms.Label();
            this.clinic5Label = new System.Windows.Forms.Label();
            this.seenInEDLabel = new System.Windows.Forms.Label();
            this.pcpPhysicianLabel = new System.Windows.Forms.Label();
            this.oprPhysicianLabel = new System.Windows.Forms.Label();
            this.admPhysicianLabel = new System.Windows.Forms.Label();
            this.attPhysicianLabel = new System.Windows.Forms.Label();
            this.refPhysicianLabel = new System.Windows.Forms.Label();
            this.admitDateLabel = new System.Windows.Forms.Label();
            this.dischargeDispositionLabel = new System.Windows.Forms.Label();
            this.dischargeDateLabel = new System.Windows.Forms.Label();
            this.admitSourceLabel = new System.Windows.Forms.Label();
            this.ageAtVisitLabel = new System.Windows.Forms.Label();
            this.chiefComplaintLabel = new System.Windows.Forms.Label();
            this.hospitalServiceLabel = new System.Windows.Forms.Label();
            this.patientTypeLabel = new System.Windows.Forms.Label();
            this.confidentialLabel = new System.Windows.Forms.Label();
            this.accountLabel = new System.Windows.Forms.Label();
            this.lastUpdatedLabel = new System.Windows.Forms.Label();
            this.accountHeaderLabel = new System.Windows.Forms.Label();
            this.lastUpdatedValueLabel = new System.Windows.Forms.Label();
            this.admitTimeLabel = new System.Windows.Forms.Label();
            this.dischargeTimeLabel = new System.Windows.Forms.Label();
            this.admitTimeValueLabel = new System.Windows.Forms.Label();
            this.dischargeTimeValueLabel = new System.Windows.Forms.Label();
            this.accountDetailsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // accountDetailsPanel
            // 
            this.accountDetailsPanel.BackColor = System.Drawing.Color.White;
            this.accountDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.accountDetailsPanel.Controls.Add(this.dischargeTimeValueLabel);
            this.accountDetailsPanel.Controls.Add(this.admitTimeValueLabel);
            this.accountDetailsPanel.Controls.Add(this.dischargeTimeLabel);
            this.accountDetailsPanel.Controls.Add(this.admitTimeLabel);
            this.accountDetailsPanel.Controls.Add(this.ageAtVisitValueLabel);
            this.accountDetailsPanel.Controls.Add(this.chiefComplaintValueLabel);
            this.accountDetailsPanel.Controls.Add(this.hospitalServiceValueLabel);
            this.accountDetailsPanel.Controls.Add(this.patientTypeValueLabel);
            this.accountDetailsPanel.Controls.Add(this.confidentialValueLabel);
            this.accountDetailsPanel.Controls.Add(this.accountValueLabel);
            this.accountDetailsPanel.Controls.Add(this.admitDateValueLabel);
            this.accountDetailsPanel.Controls.Add(this.dischargeDispositionValueLabel);
            this.accountDetailsPanel.Controls.Add(this.dischargeDateValueLabel);
            this.accountDetailsPanel.Controls.Add(this.admitSourceValueLabel);
            this.accountDetailsPanel.Controls.Add(this.clinicValueLabel[0]);
            this.accountDetailsPanel.Controls.Add(this.clinicValueLabel[1]);
            this.accountDetailsPanel.Controls.Add(this.clinicValueLabel[2]);
            this.accountDetailsPanel.Controls.Add(this.clinicValueLabel[3]);
            this.accountDetailsPanel.Controls.Add(this.clinicValueLabel[4]);
            this.accountDetailsPanel.Controls.Add(this.financialClassValueLabel);
            this.accountDetailsPanel.Controls.Add(this.siteValueLabel);
            this.accountDetailsPanel.Controls.Add(this.seenInEDValueLabel);
            this.accountDetailsPanel.Controls.Add(this.pcpPhysicianValueLabel);
            this.accountDetailsPanel.Controls.Add(this.oprPhysicianValueLabel);
            this.accountDetailsPanel.Controls.Add(this.admPhysicianValueLabel);
            this.accountDetailsPanel.Controls.Add(this.attPhysicianValueLabel);
            this.accountDetailsPanel.Controls.Add(this.refPhysicianValueLabel);
            this.accountDetailsPanel.Controls.Add(this.clinic3Label);
            this.accountDetailsPanel.Controls.Add(this.clinic2Label);
            this.accountDetailsPanel.Controls.Add(this.clinic1Label);
            this.accountDetailsPanel.Controls.Add(this.clinic4Label);
            this.accountDetailsPanel.Controls.Add(this.financialClassLabel);
            this.accountDetailsPanel.Controls.Add(this.siteLabel);
            this.accountDetailsPanel.Controls.Add(this.clinic5Label);
            this.accountDetailsPanel.Controls.Add(this.seenInEDLabel);
            this.accountDetailsPanel.Controls.Add(this.pcpPhysicianLabel);
            this.accountDetailsPanel.Controls.Add(this.oprPhysicianLabel);
            this.accountDetailsPanel.Controls.Add(this.admPhysicianLabel);
            this.accountDetailsPanel.Controls.Add(this.attPhysicianLabel);
            this.accountDetailsPanel.Controls.Add(this.refPhysicianLabel);
            this.accountDetailsPanel.Controls.Add(this.admitDateLabel);
            this.accountDetailsPanel.Controls.Add(this.dischargeDispositionLabel);
            this.accountDetailsPanel.Controls.Add(this.dischargeDateLabel);
            this.accountDetailsPanel.Controls.Add(this.admitSourceLabel);
            this.accountDetailsPanel.Controls.Add(this.ageAtVisitLabel);
            this.accountDetailsPanel.Controls.Add(this.chiefComplaintLabel);
            this.accountDetailsPanel.Controls.Add(this.hospitalServiceLabel);
            this.accountDetailsPanel.Controls.Add(this.patientTypeLabel);
            this.accountDetailsPanel.Controls.Add(this.confidentialLabel);
            this.accountDetailsPanel.Controls.Add(this.accountLabel);
            this.accountDetailsPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.accountDetailsPanel.Location = new System.Drawing.Point(7, 30);
            this.accountDetailsPanel.Name = "accountDetailsPanel";
            this.accountDetailsPanel.Size = new System.Drawing.Size(866, 282);
            this.accountDetailsPanel.TabIndex = 0;
            // 
            // ageAtVisitValueLabel
            // 
            this.ageAtVisitValueLabel.AutoSize = true;
            this.ageAtVisitValueLabel.Location = new System.Drawing.Point(120, 49);
            this.ageAtVisitValueLabel.Name = "ageAtVisitValueLabel";
            this.ageAtVisitValueLabel.Size = new System.Drawing.Size(0, 16);
            this.ageAtVisitValueLabel.TabIndex = 45;
            // 
            // chiefComplaintValueLabel
            // 
            this.chiefComplaintValueLabel.Location = new System.Drawing.Point(120, 112);
            this.chiefComplaintValueLabel.Name = "chiefComplaintValueLabel";
            this.chiefComplaintValueLabel.Size = new System.Drawing.Size(208, 80);
            this.chiefComplaintValueLabel.TabIndex = 44;
            // 
            // hospitalServiceValueLabel
            // 
            this.hospitalServiceValueLabel.AutoSize = true;
            this.hospitalServiceValueLabel.Location = new System.Drawing.Point(120, 91);
            this.hospitalServiceValueLabel.Name = "hospitalServiceValueLabel";
            this.hospitalServiceValueLabel.Size = new System.Drawing.Size(0, 16);
            this.hospitalServiceValueLabel.TabIndex = 43;
            // 
            // patientTypeValueLabel
            // 
            this.patientTypeValueLabel.AutoSize = true;
            this.patientTypeValueLabel.Location = new System.Drawing.Point(120, 70);
            this.patientTypeValueLabel.Name = "patientTypeValueLabel";
            this.patientTypeValueLabel.Size = new System.Drawing.Size(0, 16);
            this.patientTypeValueLabel.TabIndex = 42;
            // 
            // confidentialValueLabel
            // 
            this.confidentialValueLabel.AutoSize = true;
            this.confidentialValueLabel.Location = new System.Drawing.Point(120, 28);
            this.confidentialValueLabel.Name = "confidentialValueLabel";
            this.confidentialValueLabel.Size = new System.Drawing.Size(0, 16);
            this.confidentialValueLabel.TabIndex = 41;
            // 
            // accountValueLabel
            // 
            this.accountValueLabel.AutoSize = true;
            this.accountValueLabel.Location = new System.Drawing.Point(120, 7);
            this.accountValueLabel.Name = "accountValueLabel";
            this.accountValueLabel.Size = new System.Drawing.Size(0, 16);
            this.accountValueLabel.TabIndex = 40;
            // 
            // admitDateValueLabel
            // 
            this.admitDateValueLabel.AutoSize = true;
            this.admitDateValueLabel.Location = new System.Drawing.Point(120, 196);
            this.admitDateValueLabel.Name = "admitDateValueLabel";
            this.admitDateValueLabel.Size = new System.Drawing.Size(0, 16);
            this.admitDateValueLabel.TabIndex = 39;
            // 
            // dischargeDispositionValueLabel
            // 
            this.dischargeDispositionValueLabel.AutoSize = true;
            this.dischargeDispositionValueLabel.Location = new System.Drawing.Point(120, 259);
            this.dischargeDispositionValueLabel.Name = "dischargeDispositionValueLabel";
            this.dischargeDispositionValueLabel.Size = new System.Drawing.Size(0, 16);
            this.dischargeDispositionValueLabel.TabIndex = 38;
            // 
            // dischargeDateValueLabel
            // 
            this.dischargeDateValueLabel.AutoSize = true;
            this.dischargeDateValueLabel.Location = new System.Drawing.Point(120, 238);
            this.dischargeDateValueLabel.Name = "dischargeDateValueLabel";
            this.dischargeDateValueLabel.Size = new System.Drawing.Size(0, 16);
            this.dischargeDateValueLabel.TabIndex = 37;
            // 
            // admitSourceValueLabel
            // 
            this.admitSourceValueLabel.AutoSize = true;
            this.admitSourceValueLabel.Location = new System.Drawing.Point(120, 217);
            this.admitSourceValueLabel.Name = "admitSourceValueLabel";
            this.admitSourceValueLabel.Size = new System.Drawing.Size(0, 16);
            this.admitSourceValueLabel.TabIndex = 36;
            // 
            // clinic3ValueLabel
            // 
            this.clinicValueLabel[2].AutoSize = true;
            this.clinicValueLabel[2].Location = new System.Drawing.Point(472, 175);
            this.clinicValueLabel[2].Name = "clinic3ValueLabel";
            this.clinicValueLabel[2].Size = new System.Drawing.Size(0, 16);
            this.clinicValueLabel[2].TabIndex = 35;
            // 
            // clinic2ValueLabel
            // 
            this.clinicValueLabel[1].AutoSize = true;
            this.clinicValueLabel[1].Location = new System.Drawing.Point(472, 154);
            this.clinicValueLabel[1].Name = "clinic2ValueLabel";
            this.clinicValueLabel[1].Size = new System.Drawing.Size(0, 16);
            this.clinicValueLabel[1].TabIndex = 34;
            // 
            // clinic1ValueLabel
            // 
            this.clinicValueLabel[0].AutoSize = true;
            this.clinicValueLabel[0].Location = new System.Drawing.Point(472, 133);
            this.clinicValueLabel[0].Name = "clinic1ValueLabel";
            this.clinicValueLabel[0].Size = new System.Drawing.Size(0, 16);
            this.clinicValueLabel[0].TabIndex = 33;
            // 
            // clinic4ValueLabel
            // 
            this.clinicValueLabel[3].AutoSize = true;
            this.clinicValueLabel[3].Location = new System.Drawing.Point(472, 196);
            this.clinicValueLabel[3].Name = "clinic4ValueLabel";
            this.clinicValueLabel[3].Size = new System.Drawing.Size(0, 16);
            this.clinicValueLabel[3].TabIndex = 32;
            // 
            // financialClassValueLabel
            // 
            this.financialClassValueLabel.AutoSize = true;
            this.financialClassValueLabel.Location = new System.Drawing.Point(472, 259);
            this.financialClassValueLabel.Name = "financialClassValueLabel";
            this.financialClassValueLabel.Size = new System.Drawing.Size(0, 16);
            this.financialClassValueLabel.TabIndex = 31;
            // 
            // siteValueLabel
            // 
            this.siteValueLabel.AutoSize = true;
            this.siteValueLabel.Location = new System.Drawing.Point(472, 238);
            this.siteValueLabel.Name = "siteValueLabel";
            this.siteValueLabel.Size = new System.Drawing.Size(0, 16);
            this.siteValueLabel.TabIndex = 30;
            // 
            // clinic5ValueLabel
            // 
            this.clinicValueLabel[4].AutoSize = true;
            this.clinicValueLabel[4].Location = new System.Drawing.Point(472, 217);
            this.clinicValueLabel[4].Name = "clinic5ValueLabel";
            this.clinicValueLabel[4].Size = new System.Drawing.Size(0, 16);
            this.clinicValueLabel[4].TabIndex = 29;
            // 
            // seenInEDValueLabel
            // 
            this.seenInEDValueLabel.AutoSize = true;
            this.seenInEDValueLabel.Location = new System.Drawing.Point(472, 112);
            this.seenInEDValueLabel.Name = "seenInEDValueLabel";
            this.seenInEDValueLabel.Size = new System.Drawing.Size(0, 16);
            this.seenInEDValueLabel.TabIndex = 28;
            // 
            // pcpPhysicianValueLabel
            // 
            this.pcpPhysicianValueLabel.AutoSize = true;
            this.pcpPhysicianValueLabel.Location = new System.Drawing.Point(472, 91);
            this.pcpPhysicianValueLabel.Name = "pcpPhysicianValueLabel";
            this.pcpPhysicianValueLabel.Size = new System.Drawing.Size(0, 16);
            this.pcpPhysicianValueLabel.TabIndex = 27;
            // 
            // oprPhysicianValueLabel
            // 
            this.oprPhysicianValueLabel.AutoSize = true;
            this.oprPhysicianValueLabel.Location = new System.Drawing.Point(472, 70);
            this.oprPhysicianValueLabel.Name = "oprPhysicianValueLabel";
            this.oprPhysicianValueLabel.Size = new System.Drawing.Size(0, 16);
            this.oprPhysicianValueLabel.TabIndex = 26;
            // 
            // admPhysicianValueLabel
            // 
            this.admPhysicianValueLabel.AutoSize = true;
            this.admPhysicianValueLabel.Location = new System.Drawing.Point(472, 49);
            this.admPhysicianValueLabel.Name = "admPhysicianValueLabel";
            this.admPhysicianValueLabel.Size = new System.Drawing.Size(0, 16);
            this.admPhysicianValueLabel.TabIndex = 25;
            // 
            // attPhysicianValueLabel
            // 
            this.attPhysicianValueLabel.AutoSize = true;
            this.attPhysicianValueLabel.Location = new System.Drawing.Point(472, 28);
            this.attPhysicianValueLabel.Name = "attPhysicianValueLabel";
            this.attPhysicianValueLabel.Size = new System.Drawing.Size(0, 16);
            this.attPhysicianValueLabel.TabIndex = 24;
            // 
            // refPhysicianValueLabel
            // 
            this.refPhysicianValueLabel.AutoSize = true;
            this.refPhysicianValueLabel.Location = new System.Drawing.Point(472, 7);
            this.refPhysicianValueLabel.Name = "refPhysicianValueLabel";
            this.refPhysicianValueLabel.Size = new System.Drawing.Size(0, 16);
            this.refPhysicianValueLabel.TabIndex = 23;
            // 
            // clinic3Label
            // 
            this.clinic3Label.AutoSize = true;
            this.clinic3Label.Location = new System.Drawing.Point(392, 175);
            this.clinic3Label.Name = "clinic3Label";
            this.clinic3Label.Size = new System.Drawing.Size(45, 16);
            this.clinic3Label.TabIndex = 22;
            this.clinic3Label.Text = "Clinic 3:";
            // 
            // clinic2Label
            // 
            this.clinic2Label.AutoSize = true;
            this.clinic2Label.Location = new System.Drawing.Point(392, 154);
            this.clinic2Label.Name = "clinic2Label";
            this.clinic2Label.Size = new System.Drawing.Size(45, 16);
            this.clinic2Label.TabIndex = 21;
            this.clinic2Label.Text = "Clinic 2:";
            // 
            // clinic1Label
            // 
            this.clinic1Label.AutoSize = true;
            this.clinic1Label.Location = new System.Drawing.Point(392, 133);
            this.clinic1Label.Name = "clinic1Label";
            this.clinic1Label.Size = new System.Drawing.Size(45, 16);
            this.clinic1Label.TabIndex = 20;
            this.clinic1Label.Text = "Clinic 1:";
            // 
            // clinic4Label
            // 
            this.clinic4Label.AutoSize = true;
            this.clinic4Label.Location = new System.Drawing.Point(392, 196);
            this.clinic4Label.Name = "clinic4Label";
            this.clinic4Label.Size = new System.Drawing.Size(45, 16);
            this.clinic4Label.TabIndex = 19;
            this.clinic4Label.Text = "Clinic 4:";
            // 
            // financialClassLabel
            // 
            this.financialClassLabel.AutoSize = true;
            this.financialClassLabel.Location = new System.Drawing.Point(392, 259);
            this.financialClassLabel.Name = "financialClassLabel";
            this.financialClassLabel.Size = new System.Drawing.Size(82, 16);
            this.financialClassLabel.TabIndex = 18;
            this.financialClassLabel.Text = "Financial class:";
            // 
            // siteLabel
            // 
            this.siteLabel.AutoSize = true;
            this.siteLabel.Location = new System.Drawing.Point(392, 238);
            this.siteLabel.Name = "siteLabel";
            this.siteLabel.Size = new System.Drawing.Size(27, 16);
            this.siteLabel.TabIndex = 17;
            this.siteLabel.Text = "Site:";
            // 
            // clinic5Label
            // 
            this.clinic5Label.AutoSize = true;
            this.clinic5Label.Location = new System.Drawing.Point(392, 217);
            this.clinic5Label.Name = "clinic5Label";
            this.clinic5Label.Size = new System.Drawing.Size(45, 16);
            this.clinic5Label.TabIndex = 16;
            this.clinic5Label.Text = "Clinic 5:";
            // 
            // seenInEDLabel
            // 
            this.seenInEDLabel.AutoSize = true;
            this.seenInEDLabel.Location = new System.Drawing.Point(392, 112);
            this.seenInEDLabel.Name = "seenInEDLabel";
            this.seenInEDLabel.Size = new System.Drawing.Size(64, 16);
            this.seenInEDLabel.TabIndex = 15;
            this.seenInEDLabel.Text = "Seen in ED:";
            // 
            // pcpPhysicianLabel
            // 
            this.pcpPhysicianLabel.AutoSize = true;
            this.pcpPhysicianLabel.Location = new System.Drawing.Point(392, 91);
            this.pcpPhysicianLabel.Name = "pcpPhysicianLabel";
            this.pcpPhysicianLabel.Size = new System.Drawing.Size(77, 16);
            this.pcpPhysicianLabel.TabIndex = 14;
            this.pcpPhysicianLabel.Text = "PCP Physician:";
            // 
            // oprPhysicianLabel
            // 
            this.oprPhysicianLabel.AutoSize = true;
            this.oprPhysicianLabel.Location = new System.Drawing.Point(392, 70);
            this.oprPhysicianLabel.Name = "oprPhysicianLabel";
            this.oprPhysicianLabel.Size = new System.Drawing.Size(78, 16);
            this.oprPhysicianLabel.TabIndex = 13;
            this.oprPhysicianLabel.Text = "Opr Physician:";
            // 
            // admPhysicianLabel
            // 
            this.admPhysicianLabel.AutoSize = true;
            this.admPhysicianLabel.Location = new System.Drawing.Point(392, 49);
            this.admPhysicianLabel.Name = "admPhysicianLabel";
            this.admPhysicianLabel.Size = new System.Drawing.Size(82, 16);
            this.admPhysicianLabel.TabIndex = 12;
            this.admPhysicianLabel.Text = "Adm Physician:";
            // 
            // attPhysicianLabel
            // 
            this.attPhysicianLabel.AutoSize = true;
            this.attPhysicianLabel.Location = new System.Drawing.Point(392, 28);
            this.attPhysicianLabel.Name = "attPhysicianLabel";
            this.attPhysicianLabel.Size = new System.Drawing.Size(73, 16);
            this.attPhysicianLabel.TabIndex = 11;
            this.attPhysicianLabel.Text = "Att Physician:";
            // 
            // refPhysicianLabel
            // 
            this.refPhysicianLabel.AutoSize = true;
            this.refPhysicianLabel.Location = new System.Drawing.Point(392, 7);
            this.refPhysicianLabel.Name = "refPhysicianLabel";
            this.refPhysicianLabel.Size = new System.Drawing.Size(77, 16);
            this.refPhysicianLabel.TabIndex = 10;
            this.refPhysicianLabel.Text = "Ref Physician:";
            // 
            // admitDateLabel
            // 
            this.admitDateLabel.AutoSize = true;
            this.admitDateLabel.Location = new System.Drawing.Point(7, 196);
            this.admitDateLabel.Name = "admitDateLabel";
            this.admitDateLabel.Size = new System.Drawing.Size(61, 16);
            this.admitDateLabel.TabIndex = 9;
            this.admitDateLabel.Text = "Admit date:";
            // 
            // dischargeDispositionLabel
            // 
            this.dischargeDispositionLabel.AutoSize = true;
            this.dischargeDispositionLabel.Location = new System.Drawing.Point(7, 259);
            this.dischargeDispositionLabel.Name = "dischargeDispositionLabel";
            this.dischargeDispositionLabel.Size = new System.Drawing.Size(115, 16);
            this.dischargeDispositionLabel.TabIndex = 8;
            this.dischargeDispositionLabel.Text = "Discharge disposition:";
            // 
            // dischargeDateLabel
            // 
            this.dischargeDateLabel.AutoSize = true;
            this.dischargeDateLabel.Location = new System.Drawing.Point(7, 238);
            this.dischargeDateLabel.Name = "dischargeDateLabel";
            this.dischargeDateLabel.Size = new System.Drawing.Size(83, 16);
            this.dischargeDateLabel.TabIndex = 7;
            this.dischargeDateLabel.Text = "Discharge date:";
            // 
            // admitSourceLabel
            // 
            this.admitSourceLabel.AutoSize = true;
            this.admitSourceLabel.Location = new System.Drawing.Point(7, 217);
            this.admitSourceLabel.Name = "admitSourceLabel";
            this.admitSourceLabel.Size = new System.Drawing.Size(73, 16);
            this.admitSourceLabel.TabIndex = 6;
            this.admitSourceLabel.Text = "Admit source:";
            // 
            // ageAtVisitLabel
            // 
            this.ageAtVisitLabel.AutoSize = true;
            this.ageAtVisitLabel.Location = new System.Drawing.Point(7, 49);
            this.ageAtVisitLabel.Name = "ageAtVisitLabel";
            this.ageAtVisitLabel.Size = new System.Drawing.Size(63, 16);
            this.ageAtVisitLabel.TabIndex = 5;
            this.ageAtVisitLabel.Text = "Age at visit:";
            // 
            // chiefComplaintLabel
            // 
            this.chiefComplaintLabel.AutoSize = true;
            this.chiefComplaintLabel.Location = new System.Drawing.Point(7, 112);
            this.chiefComplaintLabel.Name = "chiefComplaintLabel";
            this.chiefComplaintLabel.Size = new System.Drawing.Size(85, 16);
            this.chiefComplaintLabel.TabIndex = 4;
            this.chiefComplaintLabel.Text = "Chief complaint:";
            // 
            // hospitalServiceLabel
            // 
            this.hospitalServiceLabel.AutoSize = true;
            this.hospitalServiceLabel.Location = new System.Drawing.Point(7, 91);
            this.hospitalServiceLabel.Name = "hospitalServiceLabel";
            this.hospitalServiceLabel.Size = new System.Drawing.Size(87, 16);
            this.hospitalServiceLabel.TabIndex = 3;
            this.hospitalServiceLabel.Text = "Hospital service:";
            // 
            // patientTypeLabel
            // 
            this.patientTypeLabel.AutoSize = true;
            this.patientTypeLabel.Location = new System.Drawing.Point(7, 70);
            this.patientTypeLabel.Name = "patientTypeLabel";
            this.patientTypeLabel.Size = new System.Drawing.Size(67, 16);
            this.patientTypeLabel.TabIndex = 2;
            this.patientTypeLabel.Text = "Patient type:";
            // 
            // confidentialLabel
            // 
            this.confidentialLabel.AutoSize = true;
            this.confidentialLabel.Location = new System.Drawing.Point(7, 28);
            this.confidentialLabel.Name = "confidentialLabel";
            this.confidentialLabel.Size = new System.Drawing.Size(67, 16);
            this.confidentialLabel.TabIndex = 1;
            this.confidentialLabel.Text = "Confidential:";
            // 
            // accountLabel
            // 
            this.accountLabel.AutoSize = true;
            this.accountLabel.Location = new System.Drawing.Point(7, 7);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Size = new System.Drawing.Size(48, 16);
            this.accountLabel.TabIndex = 0;
            this.accountLabel.Text = "Account:";
            // 
            // lastUpdatedLabel
            // 
            this.lastUpdatedLabel.AutoSize = true;
            this.lastUpdatedLabel.Location = new System.Drawing.Point(732, 7);
            this.lastUpdatedLabel.Name = "lastUpdatedLabel";
            this.lastUpdatedLabel.Size = new System.Drawing.Size(73, 16);
            this.lastUpdatedLabel.TabIndex = 4;
            this.lastUpdatedLabel.Text = "Last updated:";
            // 
            // accountHeaderLabel
            // 
            this.accountHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.accountHeaderLabel.Location = new System.Drawing.Point(14, 7);
            this.accountHeaderLabel.Name = "accountHeaderLabel";
            this.accountHeaderLabel.Size = new System.Drawing.Size(106, 16);
            this.accountHeaderLabel.TabIndex = 3;
            this.accountHeaderLabel.Text = "Account Summary";
            // 
            // lastUpdatedValueLabel
            // 
            this.lastUpdatedValueLabel.AutoSize = true;
            this.lastUpdatedValueLabel.Location = new System.Drawing.Point(805, 7);
            this.lastUpdatedValueLabel.Name = "lastUpdatedValueLabel";
            this.lastUpdatedValueLabel.Size = new System.Drawing.Size(0, 16);
            this.lastUpdatedValueLabel.TabIndex = 5;
            // 
            // admitTimeLabel
            // 
            this.admitTimeLabel.AutoSize = true;
            this.admitTimeLabel.Location = new System.Drawing.Point(190, 196);
            this.admitTimeLabel.Name = "admitTimeLabel";
            this.admitTimeLabel.Size = new System.Drawing.Size(33, 16);
            this.admitTimeLabel.TabIndex = 46;
            this.admitTimeLabel.Text = "Time:";
            // 
            // dischargeTimeLabel
            // 
            this.dischargeTimeLabel.AutoSize = true;
            this.dischargeTimeLabel.Location = new System.Drawing.Point(190, 238);
            this.dischargeTimeLabel.Name = "dischargeTimeLabel";
            this.dischargeTimeLabel.Size = new System.Drawing.Size(33, 16);
            this.dischargeTimeLabel.TabIndex = 47;
            this.dischargeTimeLabel.Text = "Time:";
            // 
            // admitTimeValueLabel
            // 
            this.admitTimeValueLabel.AutoSize = true;
            this.admitTimeValueLabel.Location = new System.Drawing.Point(230, 196);
            this.admitTimeValueLabel.Name = "admitTimeValueLabel";
            this.admitTimeValueLabel.Size = new System.Drawing.Size(0, 16);
            this.admitTimeValueLabel.TabIndex = 48;
            // 
            // dischargeTimeValueLabel
            // 
            this.dischargeTimeValueLabel.AutoSize = true;
            this.dischargeTimeValueLabel.Location = new System.Drawing.Point(230, 238);
            this.dischargeTimeValueLabel.Name = "dischargeTimeValueLabel";
            this.dischargeTimeValueLabel.Size = new System.Drawing.Size(0, 16);
            this.dischargeTimeValueLabel.TabIndex = 49;
            // 
            // HistoricalPatientAccountSummaryView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.lastUpdatedValueLabel);
            this.Controls.Add(this.lastUpdatedLabel);
            this.Controls.Add(this.accountHeaderLabel);
            this.Controls.Add(this.accountDetailsPanel);
            this.Name = "HistoricalPatientAccountSummaryView";
            this.Size = new System.Drawing.Size(880, 319);
            this.accountDetailsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Data Elements

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private Label lastUpdatedLabel;
        private Label accountHeaderLabel;
        private Panel accountDetailsPanel;
        private Label lastUpdatedValueLabel;
        private Label clinic3Label;
        private Label clinic2Label;
        private Label clinic1Label;
        private Label clinic4Label;
        private Label financialClassLabel;
        private Label siteLabel;
        private Label clinic5Label;
        private Label seenInEDLabel;
        private Label pcpPhysicianLabel;
        private Label oprPhysicianLabel;
        private Label admPhysicianLabel;
        private Label attPhysicianLabel;
        private Label refPhysicianLabel;
        private Label admitDateLabel;
        private Label dischargeDispositionLabel;
        private Label dischargeDateLabel;
        private Label admitSourceLabel;
        private Label ageAtVisitLabel;
        private Label chiefComplaintLabel;
        private Label hospitalServiceLabel;
        private Label patientTypeLabel;
        private Label confidentialLabel;
        private Label accountLabel;
        private Label financialClassValueLabel;
        private Label siteValueLabel;
        private Label[] clinicValueLabel;
        private Label seenInEDValueLabel;
        private Label pcpPhysicianValueLabel;
        private Label oprPhysicianValueLabel;
        private Label admPhysicianValueLabel;
        private Label attPhysicianValueLabel;
        private Label refPhysicianValueLabel;
        private Label admitDateValueLabel;
        private Label dischargeDispositionValueLabel;
        private Label dischargeDateValueLabel;
        private Label admitSourceValueLabel;
        private Label ageAtVisitValueLabel;
        private Label chiefComplaintValueLabel;
        private Label hospitalServiceValueLabel;
        private Label patientTypeValueLabel;
        private Label confidentialValueLabel;
        private Label admitTimeLabel;
        private Label dischargeTimeLabel;
        private Label admitTimeValueLabel;
        private Label dischargeTimeValueLabel;
        private Label accountValueLabel;

        #endregion

        #region Constants

	    private const string PRIMARYCAREPHYSICIAN_LABEL = "PCP Physician:";
	    private const string OTHERPHYSICIAN_LABEL = "Oth Physician:";
	
        #endregion

    }
}
