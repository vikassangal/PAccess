using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace PatientAccess.UI.HistoricalAccountViews
{
    /// <summary>
    /// Summary description for PatientDemographicsEmployment.
    /// </summary>
    public class NonPurgedShortAccountDetails : ControlView
    {
        #region Event Handlers

        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            base.ProcessCmdKey( ref msg, keyData );

            switch ( keyData )
            {
                case Keys.PageDown:
                    this.i_scrollOffset += 360;
                    break;
                case Keys.PageUp:
                    this.i_scrollOffset -= 360;
                    break;
                case Keys.Down:
                    this.i_scrollOffset += 120;
                    break;
                case Keys.Up:
                    this.i_scrollOffset -= 120;
                    break;
                case Keys.Home:
                    this.i_scrollOffset -= 6000;
                    break;
                case Keys.End:
                    this.i_scrollOffset += 6000;
                    break;
                default:
                    break;
            }

            this.NonPurgedAccountDetailsPanel.AutoScrollPosition =
                new Point(
                    this.NonPurgedAccountDetailsPanel.AutoScrollPosition.X,
                    -this.NonPurgedAccountDetailsPanel.AutoScrollPosition.Y +
                    this.i_scrollOffset );

            this.i_scrollOffset = 0;

            return true;
        }

        private void PanelMouseEnter( object sender, EventArgs e )
        {
            this.NonPurgedAccountDetailsPanel.Focus();
        }

        #endregion

        #region Construction

        public NonPurgedShortAccountDetails()
        {
            InitializeComponent();

            this.conditionCodeValueLabel = new[]{ new Label(),new Label(),
                                            new Label(), new Label(),
                                            new Label(), new Label(),
                                            new Label()
                                            };

            this.occurrenceCodeValueLabel = new[]{ new Label(),new Label(),
                                                          new Label(), new Label(),
                                                          new Label(), new Label(),
                                                          new Label(), new Label()
                                                      };

            this.occurrenceCodeDateValueLabel = new[]{ new Label(),new Label(),
                                                           new Label(), new Label(),
                                                           new Label(), new Label(),
                                                           new Label(), new Label()
                                                       };
            this.billingBody.Controls.Add( this.occurrenceCodeValueLabel[0], 1, 0 );
            this.billingBody.Controls.Add(this.occurrenceCodeDateValueLabel[0], 3, 0);

            this.billingBody.Controls.Add( this.occurrenceCodeValueLabel[1], 1,1);
            this.billingBody.Controls.Add( this.occurrenceCodeDateValueLabel[1],3,1 );

            this.billingBody.Controls.Add( this.occurrenceCodeValueLabel[7],1,7 );
            this.billingBody.Controls.Add( this.occurrenceCodeValueLabel[6],1 ,6);
            this.billingBody.Controls.Add( this.occurrenceCodeValueLabel[5],1 ,5);
            this.billingBody.Controls.Add( this.occurrenceCodeValueLabel[4],1 ,4);
            this.billingBody.Controls.Add( this.occurrenceCodeValueLabel[3],1,3 );
            this.billingBody.Controls.Add( this.occurrenceCodeValueLabel[2],1,2 );

            this.billingBody.Controls.Add( this.occurrenceCodeDateValueLabel[7],3,7 );
            this.billingBody.Controls.Add( this.occurrenceCodeDateValueLabel[6] ,3,6);
            this.billingBody.Controls.Add( this.occurrenceCodeDateValueLabel[5],3,5 );
            this.billingBody.Controls.Add( this.occurrenceCodeDateValueLabel[4],3,4 );
            this.billingBody.Controls.Add( this.occurrenceCodeDateValueLabel[3],3,3 );
            this.billingBody.Controls.Add( this.occurrenceCodeDateValueLabel[2] ,3,2);

            this.billingBody.Controls.Add( this.conditionCodeValueLabel[6] ,1,14);
            this.billingBody.Controls.Add( this.conditionCodeValueLabel[5],1,13 );
            this.billingBody.Controls.Add( this.conditionCodeValueLabel[4],1,12 );
            this.billingBody.Controls.Add( this.conditionCodeValueLabel[3],1,11 );
            this.billingBody.Controls.Add( this.conditionCodeValueLabel[2],1,10 );
            this.billingBody.Controls.Add( this.conditionCodeValueLabel[1],1,9 );
            this.billingBody.Controls.Add( this.conditionCodeValueLabel[0],1,8 );

            // 
            // conditionCodeValueLabel[6]
            // 
            this.conditionCodeValueLabel[6].AutoSize = true;
            this.conditionCodeValueLabel[6].Location = new Point( 119, 5989 );
            this.conditionCodeValueLabel[6].Name = "conditionCode7ValueLabel";
            this.conditionCodeValueLabel[6].TabIndex = 380;
            
            // 
            // conditionCodeValueLabel[5]
            // 
            this.conditionCodeValueLabel[5].AutoSize = true;
            this.conditionCodeValueLabel[5].Location = new Point( 119, 5963 );
            this.conditionCodeValueLabel[5].Name = "conditionCode6ValueLabel";
            this.conditionCodeValueLabel[5].TabIndex = 379;
            
            // 
            // conditionCodeValueLabel[4]
            // 
            this.conditionCodeValueLabel[4].AutoSize = true;
            this.conditionCodeValueLabel[4].Location = new Point( 119, 5937 );
            this.conditionCodeValueLabel[4].Name = "conditionCode5ValueLabel";
            this.conditionCodeValueLabel[4].TabIndex = 478;
            
            // 
            // conditionCodeValueLabel[3]
            // 
            this.conditionCodeValueLabel[3].AutoSize = true;
            this.conditionCodeValueLabel[3].Location = new Point( 119, 5911 );
            this.conditionCodeValueLabel[3].Name = "conditionCode4ValueLabel";
            this.conditionCodeValueLabel[3].TabIndex = 477;
            
            // 
            // conditionCodeValueLabel[2]
            // 
            this.conditionCodeValueLabel[2].AutoSize = true;
            this.conditionCodeValueLabel[2].Location = new Point( 119, 5885 );
            this.conditionCodeValueLabel[2].Name = "conditionCode3ValueLabel";
            this.conditionCodeValueLabel[2].TabIndex = 476;
            
            // 
            // conditionCodeValueLabel[1]
            //
            this.conditionCodeValueLabel[1].AutoSize = true;
            this.conditionCodeValueLabel[1].Location = new Point( 119, 5859 );
            this.conditionCodeValueLabel[1].Name = "conditionCode2ValueLabel";
            this.conditionCodeValueLabel[1].TabIndex = 475;
            
            // 
            // conditionCodeValueLabel[0]
            // 
            this.conditionCodeValueLabel[0].AutoSize = true;
            this.conditionCodeValueLabel[0].Location = new Point( 119, 5833 );
            this.conditionCodeValueLabel[0].Name = "conditionCode1ValueLabel";
            this.conditionCodeValueLabel[0].TabIndex = 474;
            
            // 
            // occurrenceCodeValueLabel[7]
            // 
            this.occurrenceCodeValueLabel[7].AutoSize = true;
            this.occurrenceCodeValueLabel[7].Location = new Point( 130, 5807 );
            this.occurrenceCodeValueLabel[7].Name = "occurrenceCode8ValueLabel";
            this.occurrenceCodeValueLabel[7].TabIndex = 473;
            
            // 
            // occurrenceCodeValueLabel[6]
            // 
            this.occurrenceCodeValueLabel[6].AutoSize = true;
            this.occurrenceCodeValueLabel[6].Location = new Point( 130, 5781 );
            this.occurrenceCodeValueLabel[6].Name = "occurrenceCode7ValueLabel";
            this.occurrenceCodeValueLabel[6].TabIndex = 472;
            
            // 
            // occurrenceCodeValueLabel[5]
            // 
            this.occurrenceCodeValueLabel[5].AutoSize = true;
            this.occurrenceCodeValueLabel[5].Location = new Point( 130, 5755 );
            this.occurrenceCodeValueLabel[5].Name = "occurrenceCode6ValueLabel";
            this.occurrenceCodeValueLabel[5].TabIndex = 471;
            
            // 
            // occurrenceCodeValueLabel[4]
            // 
            this.occurrenceCodeValueLabel[4].AutoSize = true;
            this.occurrenceCodeValueLabel[4].Location = new Point( 130, 5729 );
            this.occurrenceCodeValueLabel[4].Name = "occurrenceCode5ValueLabel";
            this.occurrenceCodeValueLabel[4].TabIndex = 470;
            
            // 
            // occurrenceCodeValueLabel[3]
            // 
            this.occurrenceCodeValueLabel[3].AutoSize = true;
            this.occurrenceCodeValueLabel[3].Location = new Point( 130, 5703 );
            this.occurrenceCodeValueLabel[3].Name = "occurrenceCode4ValueLabel";
            this.occurrenceCodeValueLabel[3].TabIndex = 469;
            
            // 
            // occurrenceCodeValueLabel[2]
            // 
            this.occurrenceCodeValueLabel[2].AutoSize = true;
            this.occurrenceCodeValueLabel[2].Location = new Point( 130, 5677 );
            this.occurrenceCodeValueLabel[2].Name = "occurrenceCode3ValueLabel";
            this.occurrenceCodeValueLabel[2].TabIndex = 468;
            
            // 
            // occurrenceCodeValueLabel[1]
            // 
            this.occurrenceCodeValueLabel[1].AutoSize = true;
            this.occurrenceCodeValueLabel[1].Location = new Point( 130, 5651 );
            this.occurrenceCodeValueLabel[1].Name = "occurrenceCode2ValueLabel";
            this.occurrenceCodeValueLabel[1].TabIndex = 467;
            
            // 
            // occurrenceCodeValueLabel[0]
            // 
            this.occurrenceCodeValueLabel[0].AutoSize = true;
            this.occurrenceCodeValueLabel[0].Location = new Point( 130, 5625 );
            this.occurrenceCodeValueLabel[0].Name = "occurrenceCode1ValueLabel";
            this.occurrenceCodeValueLabel[0].TabIndex = 466;
            
            //
            // occurrenceCodeDateValueLabel[7]
            // 
            this.occurrenceCodeDateValueLabel[7].AutoSize = true;
            this.occurrenceCodeDateValueLabel[7].Location = new Point( 442, 5807 );
            this.occurrenceCodeDateValueLabel[7].Name = "occurrenceCode8DateValueLabel";
            this.occurrenceCodeDateValueLabel[7].TabIndex = 465;
            
            // 
            // occurrenceCodeDateValueLabel[6]
            // 
            this.occurrenceCodeDateValueLabel[6].AutoSize = true;
            this.occurrenceCodeDateValueLabel[6].Location = new Point( 442, 5781 );
            this.occurrenceCodeDateValueLabel[6].Name = "occurrenceCode7DateValueLabel";
            this.occurrenceCodeDateValueLabel[6].TabIndex = 464;
            
            // 
            // occurrenceCodeDateValueLabel[5]
            // 
            this.occurrenceCodeDateValueLabel[5].AutoSize = true;
            this.occurrenceCodeDateValueLabel[5].Location = new Point( 442, 5755 );
            this.occurrenceCodeDateValueLabel[5].Name = "occurrenceCode5DateValueLabel";
            this.occurrenceCodeDateValueLabel[5].TabIndex = 463;
            
            // 
            // occurrenceCodeDateValueLabel[4]
            // 
            this.occurrenceCodeDateValueLabel[4].AutoSize = true;
            this.occurrenceCodeDateValueLabel[4].Location = new Point( 442, 5729 );
            this.occurrenceCodeDateValueLabel[4].Name = "occurrenceCode5DateValueLabel";
            this.occurrenceCodeDateValueLabel[4].TabIndex = 462;
            
            // 
            // occurrenceCodeDateValueLabel[3]
            // 
            this.occurrenceCodeDateValueLabel[3].AutoSize = true;
            this.occurrenceCodeDateValueLabel[3].Location = new Point( 442, 5703 );
            this.occurrenceCodeDateValueLabel[3].Name = "occurrenceCode4DateValueLabel";
            this.occurrenceCodeDateValueLabel[3].TabIndex = 461;
            
            // 
            // occurrenceCodeDateValueLabel[2]
            // 
            this.occurrenceCodeDateValueLabel[2].AutoSize = true;
            this.occurrenceCodeDateValueLabel[2].Location = new Point( 442, 5677 );
            this.occurrenceCodeDateValueLabel[2].Name = "occurrenceCode3DateValueLabel";
            this.occurrenceCodeDateValueLabel[2].TabIndex = 460;
            
            // 
            // occurrenceCodeDateValueLabel[1]
            // 
            this.occurrenceCodeDateValueLabel[1].AutoSize = true;
            this.occurrenceCodeDateValueLabel[1].Location = new Point( 442, 5651 );
            this.occurrenceCodeDateValueLabel[1].Name = "occurrenceCode2DateValueLabel";
            this.occurrenceCodeDateValueLabel[1].TabIndex = 459;
            
            // 
            // occurrenceCodeDateValueLabel[0]
            // 
            this.occurrenceCodeDateValueLabel[0].AutoSize = true;
            this.occurrenceCodeDateValueLabel[0].Location = new Point( 442, 5625 );
            this.occurrenceCodeDateValueLabel[0].Name = "occurrenceCode1DateValueLabel";
            this.occurrenceCodeDateValueLabel[0].TabIndex = 458;
            
            
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.NonPurgedAccountDetailsPanel = new System.Windows.Forms.Panel();
            this.accountView = new System.Windows.Forms.TableLayoutPanel();
            this.billingBody = new System.Windows.Forms.TableLayoutPanel();
            this.occurrenceCode1Label = new System.Windows.Forms.Label();
            this.occurrenceCode1DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode2DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode3DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode4DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode5DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode2Label = new System.Windows.Forms.Label();
            this.occurrenceCode3Label = new System.Windows.Forms.Label();
            this.occurrenceCode8DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode4Label = new System.Windows.Forms.Label();
            this.occurrenceCode7DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode5Label = new System.Windows.Forms.Label();
            this.occurrenceCode6DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode6Label = new System.Windows.Forms.Label();
            this.occurrenceCode7Label = new System.Windows.Forms.Label();
            this.occurrenceCode8Label = new System.Windows.Forms.Label();
            this.conditionCode1Label = new System.Windows.Forms.Label();
            this.conditionCode2Label = new System.Windows.Forms.Label();
            this.conditionCode3Label = new System.Windows.Forms.Label();
            this.conditionCode4Label = new System.Windows.Forms.Label();
            this.conditionCode5Label = new System.Windows.Forms.Label();
            this.conditionCode7Label = new System.Windows.Forms.Label();
            this.conditionCode6Label = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.panel17 = new System.Windows.Forms.Panel();
            this.regulatoryBody = new System.Windows.Forms.TableLayoutPanel();
            this.nppVersionLabel = new System.Windows.Forms.Label();
            this.nppSignatureStatusLabel = new System.Windows.Forms.Label();
            this.privacyConfidentialStatusLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryHealthInformationLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryReligionValueLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryLocationValueLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel = new System.Windows.Forms.Label();
            this.nppSignatureStatusValueLabel = new System.Windows.Forms.Label();
            this.nppVersionValueLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryReligionLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryLocationLabel = new System.Windows.Forms.Label();
            this.privacyConfidentialStatusValueLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel = new System.Windows.Forms.Label();
            this.rightToRestrictLabel = new System.Windows.Forms.Label();
            this.rightToRestrictValueLabel = new System.Windows.Forms.Label();
            this.cosSignedLabel = new System.Windows.Forms.Label();
            this.cosSignedValueLabel = new System.Windows.Forms.Label();
            this.contactEmailLabel = new System.Windows.Forms.Label();
            this.contactEmailValueLabel = new System.Windows.Forms.Label();
            this.ShareDataWithPublicHIEFlagLabel = new System.Windows.Forms.Label();
            this.ShareDataWithPublicHIEValueLabel = new System.Windows.Forms.Label();
            this.ShareDataWithPCPFlagLabel = new System.Windows.Forms.Label();
            this.ShareDataWithPCPValueLabel = new System.Windows.Forms.Label();
            this.HospCommoptinlLabel = new System.Windows.Forms.Label();
            this.HospCommoptinValueLabel = new System.Windows.Forms.Label();
            this.cobReceivedFlagLabel = new System.Windows.Forms.Label();
            this.cobReceivedValueLabel = new System.Windows.Forms.Label();
            this.imfmReceivedFlagLabel = new System.Windows.Forms.Label();
            this.imfmReceivedValueLabel = new System.Windows.Forms.Label();
            this.liabilityBody = new System.Windows.Forms.TableLayoutPanel();
            this.patientHasNoLiabilityInsuredLabel = new System.Windows.Forms.Label();
            this.patientHasNoLiabilityInsuredValueLabel = new System.Windows.Forms.Label();
            this.deductibleInsuredLabel = new System.Windows.Forms.Label();
            this.deductibleInsuredValueLabel = new System.Windows.Forms.Label();
            this.coPayCoInsInsuredValueLabel = new System.Windows.Forms.Label();
            this.patientHasNoLiabilityUninsuredValueLabel = new System.Windows.Forms.Label();
            this.estimateForCurrentAmountDueUninsuredValueLabel = new System.Windows.Forms.Label();
            this.coPayCoInsInsuredLabel = new System.Windows.Forms.Label();
            this.patientHasNoLiabilityUninsuredLabel = new System.Windows.Forms.Label();
            this.estimateForCurrentAmountDueUninsuredLabel = new System.Windows.Forms.Label();
            this.regulatoryHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.regulatoryLabel = new System.Windows.Forms.Label();
            this.generalInsuranceBody = new System.Windows.Forms.TableLayoutPanel();
            this.financialClassValueLabel = new System.Windows.Forms.Label();
            this.financialClassLabel = new System.Windows.Forms.Label();
            this.motherDobValueLabel = new System.Windows.Forms.Label();
            this.motherDobLabel = new System.Windows.Forms.Label();
            this.fatherDobLabel = new System.Windows.Forms.Label();
            this.fatherDobValueLabel = new System.Windows.Forms.Label();
            this.liabilityHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.liabilityLabel = new System.Windows.Forms.Label();
            this.diagnosisAndClinicalBody = new System.Windows.Forms.TableLayoutPanel();
            this.CptCodesValueLabel = new System.Windows.Forms.Label();
            this.CptCodesLabel = new System.Windows.Forms.Label();
            this.commentsLabel = new System.Windows.Forms.Label();
            this.patientTypeLabel = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.PatientResearchStudyCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientStudyDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientResearchSponsor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientRegistryNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProofOfConsent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patientTypeValueLabel = new System.Windows.Forms.Label();
            this.reregisterLabel = new System.Windows.Forms.Label();
            this.reregisterValueLabel = new System.Windows.Forms.Label();
            this.hospitalServiceLabel = new System.Windows.Forms.Label();
            this.hospitalServiceValueLabel = new System.Windows.Forms.Label();
            this.chiefComplaintLabel = new System.Windows.Forms.Label();
            this.chiefComplaintValueLabel = new System.Windows.Forms.Label();
            this.procedureLabel = new System.Windows.Forms.Label();
            this.procedureValueLabel = new System.Windows.Forms.Label();
            this.patientVisitLabel = new System.Windows.Forms.Label();
            this.patientVisitValueLabel = new System.Windows.Forms.Label();
            this.PatientInClinicalStudyValueLabel = new System.Windows.Forms.Label();
            this.accidentTypeLabel = new System.Windows.Forms.Label();
            this.accidentTypeValueLabel = new System.Windows.Forms.Label();
            this.accidentCrimeDateLabel = new System.Windows.Forms.Label();
            this.accidentCrimeDateValueLabel = new System.Windows.Forms.Label();
            this.accidentCrimeHourLabel = new System.Windows.Forms.Label();
            this.accidentCrimeHourValueLabel = new System.Windows.Forms.Label();
            this.accidentCrimeCountryLabel = new System.Windows.Forms.Label();
            this.accidentCrimeCountryValueLabel = new System.Windows.Forms.Label();
            this.accidentCrimeStateProvinceLabel = new System.Windows.Forms.Label();
            this.accidentCrimeStateProvinceValueLabel = new System.Windows.Forms.Label();
            this.dateOfOnsetForSymptomsIllnessLabel = new System.Windows.Forms.Label();
            this.dateOfOnsetForSymptomsIllnessValueLabel = new System.Windows.Forms.Label();
            this.admitSourceLabel = new System.Windows.Forms.Label();
            this.admitSourceValueLabel = new System.Windows.Forms.Label();
            this.clinic1Label = new System.Windows.Forms.Label();
            this.clinic1ValueLabel = new System.Windows.Forms.Label();
            this.clinic2Label = new System.Windows.Forms.Label();
            this.clinic2ValueLabel = new System.Windows.Forms.Label();
            this.clinic3Label = new System.Windows.Forms.Label();
            this.clinic3ValueLabel = new System.Windows.Forms.Label();
            this.clinic4Label = new System.Windows.Forms.Label();
            this.clinic4ValueLabel = new System.Windows.Forms.Label();
            this.clinic5Label = new System.Windows.Forms.Label();
            this.clinic5ValueLabel = new System.Windows.Forms.Label();
            this.referringPhysicianLabel = new System.Windows.Forms.Label();
            this.referringPhysicianValueLabel = new System.Windows.Forms.Label();
            this.admittingPhysicianLabel = new System.Windows.Forms.Label();
            this.admittingPhysicianValueLabel = new System.Windows.Forms.Label();
            this.attendingPhysicianLabel = new System.Windows.Forms.Label();
            this.attendingPhysicianValueLabel = new System.Windows.Forms.Label();
            this.primaryCarePhysicianLabel = new System.Windows.Forms.Label();
            this.primaryCarePhysicianValueLabel = new System.Windows.Forms.Label();
            this.PatientInClinicalREsearchLBL = new System.Windows.Forms.Label();
            this.commentsValueLabel = new System.Windows.Forms.Label();
            this.diangnosisAndClinicalHeader = new System.Windows.Forms.TableLayoutPanel();
            this.diagnosisLabel = new System.Windows.Forms.Label();
            this.diagnosisPanel = new System.Windows.Forms.Panel();
            this.dempgraphicsHeader = new System.Windows.Forms.TableLayoutPanel();
            this.patientDemographicsAndEmploymentLabel = new System.Windows.Forms.Label();
            this.patientDemographicsEmploymentPanel = new System.Windows.Forms.Panel();
            this.demographicsBody = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.passportIssuingCountryValueLabel = new System.Windows.Forms.Label();
            this.patientNameLabel = new System.Windows.Forms.Label();
            this.patientNameValueLabel = new System.Windows.Forms.Label();
            this.akaLabel = new System.Windows.Forms.Label();
            this.akaValueLabel = new System.Windows.Forms.Label();
            this.admitDateTimeLabel = new System.Windows.Forms.Label();
            this.admitDateTimeValueLabel = new System.Windows.Forms.Label();
            this.dischargeDateTimeLabel = new System.Windows.Forms.Label();
            this.dischargeDateTimeValueLabel = new System.Windows.Forms.Label();
            this.dischargeDispositionLabel = new System.Windows.Forms.Label();
            this.dischargeDispositionValueLabel = new System.Windows.Forms.Label();
            this.appointmentLabel = new System.Windows.Forms.Label();
            this.appointmentValueLabel = new System.Windows.Forms.Label();
            this.genderLabel = new System.Windows.Forms.Label();
            this.genderValueLabel = new System.Windows.Forms.Label();
            this.dobLabel = new System.Windows.Forms.Label();
            this.dobValueLabel = new System.Windows.Forms.Label();
            this.ageLabel = new System.Windows.Forms.Label();
            this.ageValueLabel = new System.Windows.Forms.Label();
            this.maritalStatusLabel = new System.Windows.Forms.Label();
            this.maritalStatusValueLabel = new System.Windows.Forms.Label();
            this.raceLabel = new System.Windows.Forms.Label();
            this.raceValueLabel = new System.Windows.Forms.Label();
            this.ethnicityLabel = new System.Windows.Forms.Label();
            this.ethnicityValueLabel = new System.Windows.Forms.Label();
            this.ssnLabel = new System.Windows.Forms.Label();
            this.ssnValueLabel = new System.Windows.Forms.Label();
            this.patientMailingAddressLabel = new System.Windows.Forms.Label();
            this.patientMailingAddressValueLabel = new System.Windows.Forms.Label();
            this.contactPhoneLabel = new System.Windows.Forms.Label();
            this.contactPhoneValueLabel = new System.Windows.Forms.Label();
            this.contactCellLabel = new System.Windows.Forms.Label();
            this.contactCellValueLabel = new System.Windows.Forms.Label();
            this.driversLicenseLabel = new System.Windows.Forms.Label();
            this.driversLicenseValueLabel = new System.Windows.Forms.Label();
            this.passportLabel = new System.Windows.Forms.Label();
            this.passportValueLabel = new System.Windows.Forms.Label();
            this.languageLabel = new System.Windows.Forms.Label();
            this.languageValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact1NameLabel = new System.Windows.Forms.Label();
            this.emergencyContact1NameValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact1IsThePatientsLabel = new System.Windows.Forms.Label();
            this.emergencyContact1IsThePatientsValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact1PhoneLabel = new System.Windows.Forms.Label();
            this.emergencyContact1PhoneValueLabel = new System.Windows.Forms.Label();
            this.generalInsuranceInformationHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel16 = new System.Windows.Forms.Panel();
            this.generalInsuranceInformationLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceMSPHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel15 = new System.Windows.Forms.Panel();
            this.mspLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceMSPBody = new System.Windows.Forms.TableLayoutPanel();
            this.mspQuestionnaireSummaryConclusionLabel = new System.Windows.Forms.Label();
            this.primaryInsurancePlanHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel14 = new System.Windows.Forms.Panel();
            this.primaryInsurancePlanLabel = new System.Windows.Forms.Label();
            this.primaryInsurancePlanBody = new System.Windows.Forms.TableLayoutPanel();
            this.primaryPlanValueLabel = new System.Windows.Forms.Label();
            this.primaryPayorBrokerLabel = new System.Windows.Forms.Label();
            this.primaryPayorBrokerValueLabel = new System.Windows.Forms.Label();
            this.primaryCategoryLabel = new System.Windows.Forms.Label();
            this.primaryCategoryValueLabel = new System.Windows.Forms.Label();
            this.primaryPlanLabel = new System.Windows.Forms.Label();
            this.primaryInsurancePayorHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.primaryInsurancePayorDetailsLabel = new System.Windows.Forms.Label();
            this.primaryInsurancePayorBody = new System.Windows.Forms.TableLayoutPanel();
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel = new System.Windows.Forms.Label();
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryGroupNumberLabel = new System.Windows.Forms.Label();
            this.primaryGroupNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryMedicalGroupOrIpaNameValueLabel = new System.Windows.Forms.Label();
            this.primaryMedicalGroupOrIpaNameLabel = new System.Windows.Forms.Label();
            this.primaryBillingCoNameValueLabel = new System.Windows.Forms.Label();
            this.primaryBillingCoNameLabel = new System.Windows.Forms.Label();
            this.primaryBillingNameValueLabel = new System.Windows.Forms.Label();
            this.primaryBillingNameLabel = new System.Windows.Forms.Label();
            this.primaryBillingPhoneValueLabel = new System.Windows.Forms.Label();
            this.primaryBillingPhoneLabel = new System.Windows.Forms.Label();
            this.primaryBillingAddressValueLabel = new System.Windows.Forms.Label();
            this.primaryBillingAddressLabel = new System.Windows.Forms.Label();
            this.primaryPrecertificationNumberLabel = new System.Windows.Forms.Label();
            this.primaryPrecertificationNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryMedicaidIssueDateValueLabel = new System.Windows.Forms.Label();
            this.primaryMedicaidIssueDateLabel = new System.Windows.Forms.Label();
            this.primaryMedicalGroupOrIPAClinicValueLabel = new System.Windows.Forms.Label();
            this.primaryMedicalGroupOrIPAClinicLabel = new System.Windows.Forms.Label();
            this.primaryAdjusterValueLabel = new System.Windows.Forms.Label();
            this.primaryAdjusterLabel = new System.Windows.Forms.Label();
            this.primaryEmployeeSupervisorValueLabel = new System.Windows.Forms.Label();
            this.primaryEmployeeSupervisorLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceInsuredHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.primaryInsuranceInsuredLabel = new System.Windows.Forms.Label();
            this.primaruInsuranceInsuredBody = new System.Windows.Forms.TableLayoutPanel();
            this.primaryNameLabel = new System.Windows.Forms.Label();
            this.primaryNameValueLabel = new System.Windows.Forms.Label();
            this.primaryThePatientIsTheInsuredsLabel = new System.Windows.Forms.Label();
            this.primaryThePatientIsTheInsuredsValueLabel = new System.Windows.Forms.Label();
            this.primaryGenderLabel = new System.Windows.Forms.Label();
            this.primaryGenderValueLabel = new System.Windows.Forms.Label();
            this.primaryDobLabel = new System.Windows.Forms.Label();
            this.primaryDobValueLabel = new System.Windows.Forms.Label();
            this.primaryNationalIdValueLabel = new System.Windows.Forms.Label();
            this.primaryNationalIdLabel = new System.Windows.Forms.Label();
            this.primaryAddressValueLabel = new System.Windows.Forms.Label();
            this.primaryAddressLabel = new System.Windows.Forms.Label();
            this.primaryContactPhoneValueLabel = new System.Windows.Forms.Label();
            this.primaryContactPhoneLabel = new System.Windows.Forms.Label();
            this.primaryContactCellValueLabel = new System.Windows.Forms.Label();
            this.primaryContactCellLabel = new System.Windows.Forms.Label();
            this.primaryEmploymentStatusValueLabel = new System.Windows.Forms.Label();
            this.primaryEmploymentStatusLabel = new System.Windows.Forms.Label();
            this.primaryEmployerValueLabel = new System.Windows.Forms.Label();
            this.primaryEmployerLabel = new System.Windows.Forms.Label();
            this.primaryEmployerPhoneValueLabel = new System.Windows.Forms.Label();
            this.primaryEmployerPhoneLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceVerificationHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.primaryInsuranceVerificationLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceVerificationBody = new System.Windows.Forms.TableLayoutPanel();
            this.primaryBenefitsVerifiedLabel = new System.Windows.Forms.Label();
            this.primaryBenefitsVerifiedValueLabel = new System.Windows.Forms.Label();
            this.primaryInitiatedByValueLabel = new System.Windows.Forms.Label();
            this.primaryInitiatedByLabel = new System.Windows.Forms.Label();
            this.primaryDateValueLabel = new System.Windows.Forms.Label();
            this.primaryDateLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationRequiredLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationRequiredValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationCompanyLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationCompanyValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationPhoneLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationPhoneValueLabel = new System.Windows.Forms.Label();
            this.primaryPromptExtValueLabel = new System.Windows.Forms.Label();
            this.primaryPromptExtLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyNameLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyNameValueLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyAddressLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyAddressValueLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyPhoneNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyPhoneNumberLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentNameLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentNameValueLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentAddressValueLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentAddressLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceAuthorizatioHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.primaryInsuranceAuthorizationLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceAuthorizationBody = new System.Windows.Forms.TableLayoutPanel();
            this.primaryTrackingNumberLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationTrackingNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationNumberLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationCompanyRepresentativeNameLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel = new System.Windows.Forms.Label();
            this.primaryDaysAuthorizedValueLabel = new System.Windows.Forms.Label();
            this.primaryDaysAuthorizedLabel = new System.Windows.Forms.Label();
            this.primaryServicesAuthorizedLabel = new System.Windows.Forms.Label();
            this.primaryServicesAuthorizedValueLabel = new System.Windows.Forms.Label();
            this.primaryEffectiveDateOfAuthorizationLabel = new System.Windows.Forms.Label();
            this.primaryEffectiveDateOfAuthorizationValueLabel = new System.Windows.Forms.Label();
            this.primaryExpirationDateOfAuthorizationValueLabel = new System.Windows.Forms.Label();
            this.primaryExpirationDateOfAuthorizationLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationStatusLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationStatusValueLabel = new System.Windows.Forms.Label();
            this.primaryRemarksValueLabel = new System.Windows.Forms.Label();
            this.primaryRemarksLabel = new System.Windows.Forms.Label();
            this.secondaryInsurancePlanHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.secondaryInsurancePlanLabel = new System.Windows.Forms.Label();
            this.secondaryInsurancePlanBody = new System.Windows.Forms.TableLayoutPanel();
            this.secondaryPlanValueLabel = new System.Windows.Forms.Label();
            this.secondaryPayorBrokerValueLabel = new System.Windows.Forms.Label();
            this.secondaryCategoryValueLabel = new System.Windows.Forms.Label();
            this.secondaryCategoryLabel = new System.Windows.Forms.Label();
            this.secondaryPayorBrokerLabel = new System.Windows.Forms.Label();
            this.secondaryPlanLabel = new System.Windows.Forms.Label();
            this.SecondaryInsurancePayorHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.secondaryInsurancePayorDetailsLabel = new System.Windows.Forms.Label();
            this.secondaryInsurancePayorDetailsBody = new System.Windows.Forms.TableLayoutPanel();
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel = new System.Windows.Forms.Label();
            this.secondaryGroupNumberLabel = new System.Windows.Forms.Label();
            this.secondaryPrecertificationNumberLabel = new System.Windows.Forms.Label();
            this.secondaryMedicaidIssueDateLabel = new System.Windows.Forms.Label();
            this.secondaryEmployeeSupervisorLabel = new System.Windows.Forms.Label();
            this.secondaryAdjusterLabel = new System.Windows.Forms.Label();
            this.secondaryMedicalGroupIpaNameLabel = new System.Windows.Forms.Label();
            this.secondaryMedicalGroupIpaClinicLabel = new System.Windows.Forms.Label();
            this.secondaryBillingCoNameLabel = new System.Windows.Forms.Label();
            this.secondaryBillingNameLabel = new System.Windows.Forms.Label();
            this.secondaryBillingPhoneLabel = new System.Windows.Forms.Label();
            this.secondaryBillingAddressLabel = new System.Windows.Forms.Label();
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryGroupNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryPrecertificationNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryMedicaidIssueDateValueLabel = new System.Windows.Forms.Label();
            this.secondaryEmployeeSupervisorValueLabel = new System.Windows.Forms.Label();
            this.secondaryAdjusterValueLabel = new System.Windows.Forms.Label();
            this.secondaryMedicalGroupIpaNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryMedicalGroupIpaClinicValueLabel = new System.Windows.Forms.Label();
            this.secondaryBillingCoNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryBillingNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryBillingPhoneValueLabel = new System.Windows.Forms.Label();
            this.secondaryBillingAddressValueLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceInsuredHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.secondaryInsuranceInsuredLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceInsuredBody = new System.Windows.Forms.TableLayoutPanel();
            this.secondaryNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryGenderLabel = new System.Windows.Forms.Label();
            this.secondaryGenderValueLabel = new System.Windows.Forms.Label();
            this.secondaryThePatientIsTheInsuredLabel = new System.Windows.Forms.Label();
            this.secondaryThePatientIsTheInsuredValueLabel = new System.Windows.Forms.Label();
            this.secondaryDobValueLabel = new System.Windows.Forms.Label();
            this.secondaryDobLabel = new System.Windows.Forms.Label();
            this.secondaryNationalIdLabel = new System.Windows.Forms.Label();
            this.secondaryNationalIdValueLabel = new System.Windows.Forms.Label();
            this.secondaryAddressValueLabel = new System.Windows.Forms.Label();
            this.secondaryAddressLabel = new System.Windows.Forms.Label();
            this.secondaryContactPhoneValueLabel = new System.Windows.Forms.Label();
            this.secondaryContactPhoneLabel = new System.Windows.Forms.Label();
            this.secondaryContactCellValueLabel = new System.Windows.Forms.Label();
            this.secondaryContactCellLabel = new System.Windows.Forms.Label();
            this.secondaryEmploymentStatusValueLabel = new System.Windows.Forms.Label();
            this.secondaryEmploymentStatusLabel = new System.Windows.Forms.Label();
            this.secondaryEmployerValueLabel = new System.Windows.Forms.Label();
            this.secondaryEmployerLabel = new System.Windows.Forms.Label();
            this.secondaryEmployerPhoneValueLabel = new System.Windows.Forms.Label();
            this.secondaryEmployerPhoneLabel = new System.Windows.Forms.Label();
            this.secondaryNameLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceVerificationHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.secondaryInsuranceVerificationLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceVerificationBody = new System.Windows.Forms.TableLayoutPanel();
            this.secondaryBenefitsVerifiedValueLabel = new System.Windows.Forms.Label();
            this.secondaryBenefitsVerifiedLabel = new System.Windows.Forms.Label();
            this.secondaryInitiatedByValueLabel = new System.Windows.Forms.Label();
            this.secondaryInitiatedByLabel = new System.Windows.Forms.Label();
            this.secondaryDateValueLabel = new System.Windows.Forms.Label();
            this.secondaryDateLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationRequiredValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationRequiredLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationCompanyLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationCompanyValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationPhoneValueLabel = new System.Windows.Forms.Label();
            this.secondaryPromptExtValueLabel = new System.Windows.Forms.Label();
            this.secondaryPromptExtLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyNameLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyAddressValueLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyAddressLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyPhoneNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyPhoneNumberLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentNameLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentAddressLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationPhoneLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceAuthorizationHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.secondaryInsuranceAuthorizationLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceAuthorizationBody = new System.Windows.Forms.TableLayoutPanel();
            this.secondaryAuthorizationTrackingNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationNumberLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationCompanyRepresentativeNameLabel = new System.Windows.Forms.Label();
            this.secondaryDaysAuthorizedValueLabel = new System.Windows.Forms.Label();
            this.secondaryDaysAuthorizedLabel = new System.Windows.Forms.Label();
            this.secondaryServicesAuthorizedLabel = new System.Windows.Forms.Label();
            this.secondaryServicesAuthorizedValueLabel = new System.Windows.Forms.Label();
            this.secondaryEffectiveDateOfAuthorizationValueLabel = new System.Windows.Forms.Label();
            this.secondaryEffectiveDateOfAuthorizationLabel = new System.Windows.Forms.Label();
            this.secondaryExpirationDateOfAuthorizationValueLabel = new System.Windows.Forms.Label();
            this.secondaryExpirationDateOfAuthorizationLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationStatusValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationStatusLabel = new System.Windows.Forms.Label();
            this.secondaryRemarksValueLabel = new System.Windows.Forms.Label();
            this.secondaryRemarksLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationTrackingNumberLabel = new System.Windows.Forms.Label();
            this.guarantorHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.guarantorLabel = new System.Windows.Forms.Label();
            this.guarantorBody = new System.Windows.Forms.TableLayoutPanel();
            this.guarantorDOBValueLabel = new System.Windows.Forms.Label();
            this.guarantorDOBLabel = new System.Windows.Forms.Label();
            this.guarantorCellPhoneConsentLabel = new System.Windows.Forms.Label();
            this.thePatientIsTheGuarantorsLabel = new System.Windows.Forms.Label();
            this.thePatientIsTheGuarantorsValueLabel = new System.Windows.Forms.Label();
            this.guarantorNameLabel = new System.Windows.Forms.Label();
            this.guarantorNameValueLabel = new System.Windows.Forms.Label();
            this.guarantorSSNLabel = new System.Windows.Forms.Label();
            this.guarantorSSNValueLabel = new System.Windows.Forms.Label();
            this.usDriverLicenseLabel = new System.Windows.Forms.Label();
            this.usDriverLicenseValueLabel = new System.Windows.Forms.Label();
            this.guarantorAddressLabel = new System.Windows.Forms.Label();
            this.guarantorAddressValueLabel = new System.Windows.Forms.Label();
            this.guarantorContactPhoneValueLabel = new System.Windows.Forms.Label();
            this.guarantorContactPhoneLabel = new System.Windows.Forms.Label();
            this.guarantorContactCellValueLabel = new System.Windows.Forms.Label();
            this.guarantorContactCellLabel = new System.Windows.Forms.Label();
            this.guarantorCellPhoneConsentValueLabel = new System.Windows.Forms.Label();
            this.paymentBody = new System.Windows.Forms.TableLayoutPanel();
            this.totalCurrentAmountDueValueLabel = new System.Windows.Forms.Label();
            this.monthlyDueDateLabel = new System.Windows.Forms.Label();
            this.totalPaymentsCollectedForCurrentAccountLabel = new System.Windows.Forms.Label();
            this.totalPaymentsCollectedForCurrentAccountValueLabel = new System.Windows.Forms.Label();
            this.numberOfMonthlyPaymentsLabel = new System.Windows.Forms.Label();
            this.numberOfMonthlyPaymentsValueLabel = new System.Windows.Forms.Label();
            this.monthlyPaymentLabel = new System.Windows.Forms.Label();
            this.monthlyPaymentValueLabel = new System.Windows.Forms.Label();
            this.monthlyDueDateValueLabel = new System.Windows.Forms.Label();
            this.totalCurrentAmountDueLabel = new System.Windows.Forms.Label();
            this.PaymentHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.paymentLabel = new System.Windows.Forms.Label();
            this.billingHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel18 = new System.Windows.Forms.Panel();
            this.billingLabel = new System.Windows.Forms.Label();
            this.LeftWithoutFinancialClearanceValueLabel = new System.Windows.Forms.Label();
            this.LeftWithoutBeingSeenValueLabel = new System.Windows.Forms.Label();
            this.RCRPValueLabel = new System.Windows.Forms.Label();
            this.LeftOrStayedValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact2PhoneValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact2AddressValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact2IsThePatientsValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact2NameValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact1AddressValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact2PhoneLabel = new System.Windows.Forms.Label();
            this.emergencyContact2AddressLabel = new System.Windows.Forms.Label();
            this.emergencyContact1AddressLabel = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NonPurgedAccountDetailsPanel.SuspendLayout();
            this.accountView.SuspendLayout();
            this.billingBody.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.regulatoryBody.SuspendLayout();
            this.liabilityBody.SuspendLayout();
            this.regulatoryHeader.SuspendLayout();
            this.generalInsuranceBody.SuspendLayout();
            this.liabilityHeader.SuspendLayout();
            this.diagnosisAndClinicalBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.diangnosisAndClinicalHeader.SuspendLayout();
            this.dempgraphicsHeader.SuspendLayout();
            this.demographicsBody.SuspendLayout();
            this.generalInsuranceInformationHeader.SuspendLayout();
            this.primaryInsuranceMSPHeader.SuspendLayout();
            this.primaryInsuranceMSPBody.SuspendLayout();
            this.primaryInsurancePlanHeader.SuspendLayout();
            this.primaryInsurancePlanBody.SuspendLayout();
            this.primaryInsurancePayorHeader.SuspendLayout();
            this.primaryInsurancePayorBody.SuspendLayout();
            this.primaryInsuranceInsuredHeader.SuspendLayout();
            this.primaruInsuranceInsuredBody.SuspendLayout();
            this.primaryInsuranceVerificationHeader.SuspendLayout();
            this.primaryInsuranceVerificationBody.SuspendLayout();
            this.primaryInsuranceAuthorizatioHeader.SuspendLayout();
            this.primaryInsuranceAuthorizationBody.SuspendLayout();
            this.secondaryInsurancePlanHeader.SuspendLayout();
            this.secondaryInsurancePlanBody.SuspendLayout();
            this.SecondaryInsurancePayorHeader.SuspendLayout();
            this.secondaryInsurancePayorDetailsBody.SuspendLayout();
            this.secondaryInsuranceInsuredHeader.SuspendLayout();
            this.secondaryInsuranceInsuredBody.SuspendLayout();
            this.secondaryInsuranceVerificationHeader.SuspendLayout();
            this.secondaryInsuranceVerificationBody.SuspendLayout();
            this.secondaryInsuranceAuthorizationHeader.SuspendLayout();
            this.secondaryInsuranceAuthorizationBody.SuspendLayout();
            this.guarantorHeader.SuspendLayout();
            this.guarantorBody.SuspendLayout();
            this.paymentBody.SuspendLayout();
            this.PaymentHeader.SuspendLayout();
            this.billingHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // NonPurgedAccountDetailsPanel
            // 
            this.NonPurgedAccountDetailsPanel.AutoScroll = true;
            this.NonPurgedAccountDetailsPanel.BackColor = System.Drawing.Color.White;
            this.NonPurgedAccountDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accountView);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.LeftWithoutFinancialClearanceValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.LeftWithoutBeingSeenValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.RCRPValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.LeftOrStayedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2PhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2AddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2IsThePatientsValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2NameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1AddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2PhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2AddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1AddressLabel);
            this.NonPurgedAccountDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NonPurgedAccountDetailsPanel.Location = new System.Drawing.Point(0, 0);
            this.NonPurgedAccountDetailsPanel.Name = "NonPurgedAccountDetailsPanel";
            this.NonPurgedAccountDetailsPanel.Size = new System.Drawing.Size(908, 528);
            this.NonPurgedAccountDetailsPanel.TabIndex = 1;
            this.NonPurgedAccountDetailsPanel.TabStop = true;
            this.NonPurgedAccountDetailsPanel.MouseEnter += new System.EventHandler(this.PanelMouseEnter);
            // 
            // accountView
            // 
            this.accountView.ColumnCount = 1;
            this.accountView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.accountView.Controls.Add(this.billingBody, 0, 32);
            this.accountView.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.accountView.Controls.Add(this.regulatoryBody, 0, 38);
            this.accountView.Controls.Add(this.liabilityBody, 0, 34);
            this.accountView.Controls.Add(this.regulatoryHeader, 0, 37);
            this.accountView.Controls.Add(this.generalInsuranceBody, 0, 6);
            this.accountView.Controls.Add(this.liabilityHeader, 0, 33);
            this.accountView.Controls.Add(this.diagnosisAndClinicalBody, 0, 4);
            this.accountView.Controls.Add(this.diangnosisAndClinicalHeader, 0, 3);
            this.accountView.Controls.Add(this.dempgraphicsHeader, 0, 1);
            this.accountView.Controls.Add(this.demographicsBody, 0, 2);
            this.accountView.Controls.Add(this.generalInsuranceInformationHeader, 0, 5);
            this.accountView.Controls.Add(this.primaryInsuranceMSPHeader, 0, 7);
            this.accountView.Controls.Add(this.primaryInsuranceMSPBody, 0, 8);
            this.accountView.Controls.Add(this.primaryInsurancePlanHeader, 0, 9);
            this.accountView.Controls.Add(this.primaryInsurancePlanBody, 0, 10);
            this.accountView.Controls.Add(this.primaryInsurancePayorHeader, 0, 11);
            this.accountView.Controls.Add(this.primaryInsurancePayorBody, 0, 12);
            this.accountView.Controls.Add(this.primaryInsuranceInsuredHeader, 0, 13);
            this.accountView.Controls.Add(this.primaruInsuranceInsuredBody, 0, 14);
            this.accountView.Controls.Add(this.primaryInsuranceVerificationHeader, 0, 15);
            this.accountView.Controls.Add(this.primaryInsuranceVerificationBody, 0, 16);
            this.accountView.Controls.Add(this.primaryInsuranceAuthorizatioHeader, 0, 17);
            this.accountView.Controls.Add(this.primaryInsuranceAuthorizationBody, 0, 18);
            this.accountView.Controls.Add(this.secondaryInsurancePlanHeader, 0, 19);
            this.accountView.Controls.Add(this.secondaryInsurancePlanBody, 0, 20);
            this.accountView.Controls.Add(this.SecondaryInsurancePayorHeader, 0, 21);
            this.accountView.Controls.Add(this.secondaryInsurancePayorDetailsBody, 0, 22);
            this.accountView.Controls.Add(this.secondaryInsuranceInsuredHeader, 0, 23);
            this.accountView.Controls.Add(this.secondaryInsuranceInsuredBody, 0, 24);
            this.accountView.Controls.Add(this.secondaryInsuranceVerificationHeader, 0, 25);
            this.accountView.Controls.Add(this.secondaryInsuranceVerificationBody, 0, 26);
            this.accountView.Controls.Add(this.secondaryInsuranceAuthorizationHeader, 0, 27);
            this.accountView.Controls.Add(this.secondaryInsuranceAuthorizationBody, 0, 28);
            this.accountView.Controls.Add(this.guarantorHeader, 0, 29);
            this.accountView.Controls.Add(this.guarantorBody, 0, 30);
            this.accountView.Controls.Add(this.paymentBody, 0, 36);
            this.accountView.Controls.Add(this.PaymentHeader, 0, 35);
            this.accountView.Controls.Add(this.billingHeader, 0, 31);
            this.accountView.Location = new System.Drawing.Point(15, 1);
            this.accountView.Name = "accountView";
            this.accountView.RowCount = 39;
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.017714F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.98228F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 663F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 292F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 198F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 257F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 239F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 285F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 216F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 336F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 117F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.accountView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 731F));
            this.accountView.Size = new System.Drawing.Size(900, 5667);
            this.accountView.TabIndex = 549;
            // 
            // billingBody
            // 
            this.billingBody.ColumnCount = 4;
            this.billingBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.41373F));
            this.billingBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 257F));
            this.billingBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.billingBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.58627F));
            this.billingBody.Controls.Add(this.occurrenceCode1Label, 0, 0);
            this.billingBody.Controls.Add(this.occurrenceCode1DateLabel, 2, 0);
            this.billingBody.Controls.Add(this.occurrenceCode2DateLabel, 2, 1);
            this.billingBody.Controls.Add(this.occurrenceCode3DateLabel, 2, 2);
            this.billingBody.Controls.Add(this.occurrenceCode4DateLabel, 2, 3);
            this.billingBody.Controls.Add(this.occurrenceCode5DateLabel, 2, 4);
            this.billingBody.Controls.Add(this.occurrenceCode2Label, 0, 1);
            this.billingBody.Controls.Add(this.occurrenceCode3Label, 0, 2);
            this.billingBody.Controls.Add(this.occurrenceCode8DateLabel, 2, 7);
            this.billingBody.Controls.Add(this.occurrenceCode4Label, 0, 3);
            this.billingBody.Controls.Add(this.occurrenceCode7DateLabel, 2, 6);
            this.billingBody.Controls.Add(this.occurrenceCode5Label, 0, 4);
            this.billingBody.Controls.Add(this.occurrenceCode6DateLabel, 2, 5);
            this.billingBody.Controls.Add(this.occurrenceCode6Label, 0, 5);
            this.billingBody.Controls.Add(this.occurrenceCode7Label, 0, 6);
            this.billingBody.Controls.Add(this.occurrenceCode8Label, 0, 7);
            this.billingBody.Controls.Add(this.conditionCode1Label, 0, 8);
            this.billingBody.Controls.Add(this.conditionCode2Label, 0, 9);
            this.billingBody.Controls.Add(this.conditionCode3Label, 0, 10);
            this.billingBody.Controls.Add(this.conditionCode4Label, 0, 11);
            this.billingBody.Controls.Add(this.conditionCode5Label, 0, 12);
            this.billingBody.Controls.Add(this.conditionCode7Label, 0, 14);
            this.billingBody.Controls.Add(this.conditionCode6Label, 0, 13);
            this.billingBody.Location = new System.Drawing.Point(3, 4242);
            this.billingBody.Name = "billingBody";
            this.billingBody.RowCount = 15;
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingBody.Size = new System.Drawing.Size(597, 305);
            this.billingBody.TabIndex = 555;
            // 
            // occurrenceCode1Label
            // 
            this.occurrenceCode1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode1Label.Location = new System.Drawing.Point(3, 0);
            this.occurrenceCode1Label.Name = "occurrenceCode1Label";
            this.occurrenceCode1Label.Size = new System.Drawing.Size(127, 16);
            this.occurrenceCode1Label.TabIndex = 346;
            this.occurrenceCode1Label.Text = "Occurrence code 1:";
            // 
            // occurrenceCode1DateLabel
            // 
            this.occurrenceCode1DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode1DateLabel.Location = new System.Drawing.Point(400, 0);
            this.occurrenceCode1DateLabel.Name = "occurrenceCode1DateLabel";
            this.occurrenceCode1DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode1DateLabel.TabIndex = 347;
            this.occurrenceCode1DateLabel.Text = "Date:";
            // 
            // occurrenceCode2DateLabel
            // 
            this.occurrenceCode2DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode2DateLabel.Location = new System.Drawing.Point(400, 20);
            this.occurrenceCode2DateLabel.Name = "occurrenceCode2DateLabel";
            this.occurrenceCode2DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode2DateLabel.TabIndex = 349;
            this.occurrenceCode2DateLabel.Text = "Date:";
            // 
            // occurrenceCode3DateLabel
            // 
            this.occurrenceCode3DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode3DateLabel.Location = new System.Drawing.Point(400, 40);
            this.occurrenceCode3DateLabel.Name = "occurrenceCode3DateLabel";
            this.occurrenceCode3DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode3DateLabel.TabIndex = 351;
            this.occurrenceCode3DateLabel.Text = "Date:";
            // 
            // occurrenceCode4DateLabel
            // 
            this.occurrenceCode4DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode4DateLabel.Location = new System.Drawing.Point(400, 60);
            this.occurrenceCode4DateLabel.Name = "occurrenceCode4DateLabel";
            this.occurrenceCode4DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode4DateLabel.TabIndex = 353;
            this.occurrenceCode4DateLabel.Text = "Date:";
            // 
            // occurrenceCode5DateLabel
            // 
            this.occurrenceCode5DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode5DateLabel.Location = new System.Drawing.Point(400, 80);
            this.occurrenceCode5DateLabel.Name = "occurrenceCode5DateLabel";
            this.occurrenceCode5DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode5DateLabel.TabIndex = 355;
            this.occurrenceCode5DateLabel.Text = "Date:";
            // 
            // occurrenceCode2Label
            // 
            this.occurrenceCode2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode2Label.Location = new System.Drawing.Point(3, 20);
            this.occurrenceCode2Label.Name = "occurrenceCode2Label";
            this.occurrenceCode2Label.Size = new System.Drawing.Size(127, 16);
            this.occurrenceCode2Label.TabIndex = 348;
            this.occurrenceCode2Label.Text = "Occurrence code 2:";
            // 
            // occurrenceCode3Label
            // 
            this.occurrenceCode3Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode3Label.Location = new System.Drawing.Point(3, 40);
            this.occurrenceCode3Label.Name = "occurrenceCode3Label";
            this.occurrenceCode3Label.Size = new System.Drawing.Size(127, 16);
            this.occurrenceCode3Label.TabIndex = 350;
            this.occurrenceCode3Label.Text = "Occurrence code 3:";
            // 
            // occurrenceCode8DateLabel
            // 
            this.occurrenceCode8DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode8DateLabel.Location = new System.Drawing.Point(400, 140);
            this.occurrenceCode8DateLabel.Name = "occurrenceCode8DateLabel";
            this.occurrenceCode8DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode8DateLabel.TabIndex = 361;
            this.occurrenceCode8DateLabel.Text = "Date:";
            // 
            // occurrenceCode4Label
            // 
            this.occurrenceCode4Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode4Label.Location = new System.Drawing.Point(3, 60);
            this.occurrenceCode4Label.Name = "occurrenceCode4Label";
            this.occurrenceCode4Label.Size = new System.Drawing.Size(127, 16);
            this.occurrenceCode4Label.TabIndex = 352;
            this.occurrenceCode4Label.Text = "Occurrence code 4:";
            // 
            // occurrenceCode7DateLabel
            // 
            this.occurrenceCode7DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode7DateLabel.Location = new System.Drawing.Point(400, 120);
            this.occurrenceCode7DateLabel.Name = "occurrenceCode7DateLabel";
            this.occurrenceCode7DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode7DateLabel.TabIndex = 359;
            this.occurrenceCode7DateLabel.Text = "Date:";
            // 
            // occurrenceCode5Label
            // 
            this.occurrenceCode5Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode5Label.Location = new System.Drawing.Point(3, 80);
            this.occurrenceCode5Label.Name = "occurrenceCode5Label";
            this.occurrenceCode5Label.Size = new System.Drawing.Size(127, 16);
            this.occurrenceCode5Label.TabIndex = 354;
            this.occurrenceCode5Label.Text = "Occurrence code 5:";
            // 
            // occurrenceCode6DateLabel
            // 
            this.occurrenceCode6DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode6DateLabel.Location = new System.Drawing.Point(400, 100);
            this.occurrenceCode6DateLabel.Name = "occurrenceCode6DateLabel";
            this.occurrenceCode6DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode6DateLabel.TabIndex = 357;
            this.occurrenceCode6DateLabel.Text = "Date:";
            // 
            // occurrenceCode6Label
            // 
            this.occurrenceCode6Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode6Label.Location = new System.Drawing.Point(3, 100);
            this.occurrenceCode6Label.Name = "occurrenceCode6Label";
            this.occurrenceCode6Label.Size = new System.Drawing.Size(127, 16);
            this.occurrenceCode6Label.TabIndex = 356;
            this.occurrenceCode6Label.Text = "Occurrence code 6:";
            // 
            // occurrenceCode7Label
            // 
            this.occurrenceCode7Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode7Label.Location = new System.Drawing.Point(3, 120);
            this.occurrenceCode7Label.Name = "occurrenceCode7Label";
            this.occurrenceCode7Label.Size = new System.Drawing.Size(127, 16);
            this.occurrenceCode7Label.TabIndex = 358;
            this.occurrenceCode7Label.Text = "Occurrence code 7:";
            // 
            // occurrenceCode8Label
            // 
            this.occurrenceCode8Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode8Label.Location = new System.Drawing.Point(3, 140);
            this.occurrenceCode8Label.Name = "occurrenceCode8Label";
            this.occurrenceCode8Label.Size = new System.Drawing.Size(127, 16);
            this.occurrenceCode8Label.TabIndex = 360;
            this.occurrenceCode8Label.Text = "Occurrence code 8:";
            // 
            // conditionCode1Label
            // 
            this.conditionCode1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode1Label.Location = new System.Drawing.Point(3, 160);
            this.conditionCode1Label.Name = "conditionCode1Label";
            this.conditionCode1Label.Size = new System.Drawing.Size(127, 16);
            this.conditionCode1Label.TabIndex = 362;
            this.conditionCode1Label.Text = "Condition code 1:";
            // 
            // conditionCode2Label
            // 
            this.conditionCode2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode2Label.Location = new System.Drawing.Point(3, 180);
            this.conditionCode2Label.Name = "conditionCode2Label";
            this.conditionCode2Label.Size = new System.Drawing.Size(127, 16);
            this.conditionCode2Label.TabIndex = 363;
            this.conditionCode2Label.Text = "Condition code 2:";
            // 
            // conditionCode3Label
            // 
            this.conditionCode3Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode3Label.Location = new System.Drawing.Point(3, 200);
            this.conditionCode3Label.Name = "conditionCode3Label";
            this.conditionCode3Label.Size = new System.Drawing.Size(127, 16);
            this.conditionCode3Label.TabIndex = 364;
            this.conditionCode3Label.Text = "Condition code 3:";
            // 
            // conditionCode4Label
            // 
            this.conditionCode4Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode4Label.Location = new System.Drawing.Point(3, 220);
            this.conditionCode4Label.Name = "conditionCode4Label";
            this.conditionCode4Label.Size = new System.Drawing.Size(127, 16);
            this.conditionCode4Label.TabIndex = 365;
            this.conditionCode4Label.Text = "Condition code 4:";
            // 
            // conditionCode5Label
            // 
            this.conditionCode5Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode5Label.Location = new System.Drawing.Point(3, 240);
            this.conditionCode5Label.Name = "conditionCode5Label";
            this.conditionCode5Label.Size = new System.Drawing.Size(127, 16);
            this.conditionCode5Label.TabIndex = 366;
            this.conditionCode5Label.Text = "Condition code 5:";
            // 
            // conditionCode7Label
            // 
            this.conditionCode7Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode7Label.Location = new System.Drawing.Point(3, 280);
            this.conditionCode7Label.Name = "conditionCode7Label";
            this.conditionCode7Label.Size = new System.Drawing.Size(127, 16);
            this.conditionCode7Label.TabIndex = 368;
            this.conditionCode7Label.Text = "Condition code 7:";
            // 
            // conditionCode6Label
            // 
            this.conditionCode6Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode6Label.Location = new System.Drawing.Point(3, 260);
            this.conditionCode6Label.Name = "conditionCode6Label";
            this.conditionCode6Label.Size = new System.Drawing.Size(127, 16);
            this.conditionCode6Label.TabIndex = 367;
            this.conditionCode6Label.Text = "Condition code 6:";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.panel17, 0, 2);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 3;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(660, 2);
            this.tableLayoutPanel6.TabIndex = 583;
            // 
            // panel17
            // 
            this.panel17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel17.Location = new System.Drawing.Point(3, 23);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(591, 1);
            this.panel17.TabIndex = 441;
            // 
            // regulatoryBody
            // 
            this.regulatoryBody.ColumnCount = 2;
            this.regulatoryBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.76115F));
            this.regulatoryBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.23885F));
            this.regulatoryBody.Controls.Add(this.nppVersionLabel, 0, 0);
            this.regulatoryBody.Controls.Add(this.nppSignatureStatusLabel, 0, 1);
            this.regulatoryBody.Controls.Add(this.privacyConfidentialStatusLabel, 0, 2);
            this.regulatoryBody.Controls.Add(this.privacyOptOutPatientDirectoryNameAndAllInformationLabel, 0, 3);
            this.regulatoryBody.Controls.Add(this.privacyOptOutPatientDirectoryHealthInformationLabel, 0, 5);
            this.regulatoryBody.Controls.Add(this.privacyOptOutPatientDirectoryReligionValueLabel, 1, 6);
            this.regulatoryBody.Controls.Add(this.privacyOptOutPatientDirectoryLocationValueLabel, 1, 4);
            this.regulatoryBody.Controls.Add(this.privacyOptOutPatientDirectoryHealthInformationValueLabel, 1, 5);
            this.regulatoryBody.Controls.Add(this.nppSignatureStatusValueLabel, 1, 1);
            this.regulatoryBody.Controls.Add(this.nppVersionValueLabel, 1, 0);
            this.regulatoryBody.Controls.Add(this.privacyOptOutPatientDirectoryReligionLabel, 0, 6);
            this.regulatoryBody.Controls.Add(this.privacyOptOutPatientDirectoryLocationLabel, 0, 4);
            this.regulatoryBody.Controls.Add(this.privacyConfidentialStatusValueLabel, 1, 2);
            this.regulatoryBody.Controls.Add(this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel, 1, 3);
            this.regulatoryBody.Controls.Add(this.rightToRestrictLabel, 0, 7);
            this.regulatoryBody.Controls.Add(this.rightToRestrictValueLabel, 1, 7);
            this.regulatoryBody.Controls.Add(this.cosSignedLabel, 0, 8);
            this.regulatoryBody.Controls.Add(this.cosSignedValueLabel, 1, 8);
            this.regulatoryBody.Controls.Add(this.contactEmailLabel, 0, 9);
            this.regulatoryBody.Controls.Add(this.contactEmailValueLabel, 1, 9);
            this.regulatoryBody.Controls.Add(this.ShareDataWithPublicHIEFlagLabel, 0, 10);
            this.regulatoryBody.Controls.Add(this.ShareDataWithPublicHIEValueLabel, 1, 10);
            this.regulatoryBody.Controls.Add(this.ShareDataWithPCPFlagLabel, 0, 11);
            this.regulatoryBody.Controls.Add(this.ShareDataWithPCPValueLabel, 1, 11);
            this.regulatoryBody.Controls.Add(this.HospCommoptinlLabel, 0, 12);
            this.regulatoryBody.Controls.Add(this.HospCommoptinValueLabel, 1, 12);
            this.regulatoryBody.Controls.Add(this.cobReceivedFlagLabel, 0, 13);
            this.regulatoryBody.Controls.Add(this.cobReceivedValueLabel, 1, 13);
            this.regulatoryBody.Controls.Add(this.imfmReceivedFlagLabel, 0, 14);
            this.regulatoryBody.Controls.Add(this.imfmReceivedValueLabel, 1, 14);
            this.regulatoryBody.Location = new System.Drawing.Point(3, 4938);
            this.regulatoryBody.Name = "regulatoryBody";
            this.regulatoryBody.RowCount = 14;
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.regulatoryBody.Size = new System.Drawing.Size(762, 295);
            this.regulatoryBody.TabIndex = 552;
            // 
            // nppVersionLabel
            // 
            this.nppVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nppVersionLabel.Location = new System.Drawing.Point(3, 0);
            this.nppVersionLabel.Name = "nppVersionLabel";
            this.nppVersionLabel.Size = new System.Drawing.Size(93, 16);
            this.nppVersionLabel.TabIndex = 421;
            this.nppVersionLabel.Text = "NPP version:";
            // 
            // nppSignatureStatusLabel
            // 
            this.nppSignatureStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nppSignatureStatusLabel.Location = new System.Drawing.Point(3, 20);
            this.nppSignatureStatusLabel.Name = "nppSignatureStatusLabel";
            this.nppSignatureStatusLabel.Size = new System.Drawing.Size(137, 16);
            this.nppSignatureStatusLabel.TabIndex = 423;
            this.nppSignatureStatusLabel.Text = "NPP signature status:";
            // 
            // privacyConfidentialStatusLabel
            // 
            this.privacyConfidentialStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyConfidentialStatusLabel.Location = new System.Drawing.Point(3, 40);
            this.privacyConfidentialStatusLabel.Name = "privacyConfidentialStatusLabel";
            this.privacyConfidentialStatusLabel.Size = new System.Drawing.Size(174, 16);
            this.privacyConfidentialStatusLabel.TabIndex = 425;
            this.privacyConfidentialStatusLabel.Text = "Privacy – Confidential status:";
            // 
            // privacyOptOutPatientDirectoryNameAndAllInformationLabel
            // 
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Location = new System.Drawing.Point(3, 60);
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Name = "privacyOptOutPatientDirectoryNameAndAllInformationLabel";
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Size = new System.Drawing.Size(450, 20);
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.TabIndex = 427;
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Text = "Privacy – Opt out (exclude) from patient directory – Name and all information:";
            // 
            // privacyOptOutPatientDirectoryHealthInformationLabel
            // 
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Location = new System.Drawing.Point(3, 100);
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Name = "privacyOptOutPatientDirectoryHealthInformationLabel";
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Size = new System.Drawing.Size(437, 16);
            this.privacyOptOutPatientDirectoryHealthInformationLabel.TabIndex = 431;
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Text = "Privacy – Opt out (exclude) from patient directory – Health information:";
            // 
            // privacyOptOutPatientDirectoryReligionValueLabel
            // 
            this.privacyOptOutPatientDirectoryReligionValueLabel.AutoSize = true;
            this.privacyOptOutPatientDirectoryReligionValueLabel.Location = new System.Drawing.Point(465, 120);
            this.privacyOptOutPatientDirectoryReligionValueLabel.Name = "privacyOptOutPatientDirectoryReligionValueLabel";
            this.privacyOptOutPatientDirectoryReligionValueLabel.Size = new System.Drawing.Size(0, 17);
            this.privacyOptOutPatientDirectoryReligionValueLabel.TabIndex = 434;
            // 
            // privacyOptOutPatientDirectoryLocationValueLabel
            // 
            this.privacyOptOutPatientDirectoryLocationValueLabel.AutoSize = true;
            this.privacyOptOutPatientDirectoryLocationValueLabel.Location = new System.Drawing.Point(465, 80);
            this.privacyOptOutPatientDirectoryLocationValueLabel.Name = "privacyOptOutPatientDirectoryLocationValueLabel";
            this.privacyOptOutPatientDirectoryLocationValueLabel.Size = new System.Drawing.Size(0, 17);
            this.privacyOptOutPatientDirectoryLocationValueLabel.TabIndex = 430;
            // 
            // privacyOptOutPatientDirectoryHealthInformationValueLabel
            // 
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.AutoSize = true;
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.Location = new System.Drawing.Point(465, 100);
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.Name = "privacyOptOutPatientDirectoryHealthInformationValueLabel";
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.Size = new System.Drawing.Size(0, 17);
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.TabIndex = 432;
            // 
            // nppSignatureStatusValueLabel
            // 
            this.nppSignatureStatusValueLabel.AutoSize = true;
            this.nppSignatureStatusValueLabel.Location = new System.Drawing.Point(465, 20);
            this.nppSignatureStatusValueLabel.Name = "nppSignatureStatusValueLabel";
            this.nppSignatureStatusValueLabel.Size = new System.Drawing.Size(12, 17);
            this.nppSignatureStatusValueLabel.TabIndex = 424;
            this.nppSignatureStatusValueLabel.Text = " ";
            // 
            // nppVersionValueLabel
            // 
            this.nppVersionValueLabel.AutoSize = true;
            this.nppVersionValueLabel.Location = new System.Drawing.Point(465, 0);
            this.nppVersionValueLabel.Name = "nppVersionValueLabel";
            this.nppVersionValueLabel.Size = new System.Drawing.Size(0, 17);
            this.nppVersionValueLabel.TabIndex = 422;
            // 
            // privacyOptOutPatientDirectoryReligionLabel
            // 
            this.privacyOptOutPatientDirectoryReligionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyOptOutPatientDirectoryReligionLabel.Location = new System.Drawing.Point(3, 120);
            this.privacyOptOutPatientDirectoryReligionLabel.Name = "privacyOptOutPatientDirectoryReligionLabel";
            this.privacyOptOutPatientDirectoryReligionLabel.Size = new System.Drawing.Size(361, 16);
            this.privacyOptOutPatientDirectoryReligionLabel.TabIndex = 433;
            this.privacyOptOutPatientDirectoryReligionLabel.Text = "Privacy – Opt out (exclude) from patient directory – Religion:";
            // 
            // privacyOptOutPatientDirectoryLocationLabel
            // 
            this.privacyOptOutPatientDirectoryLocationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyOptOutPatientDirectoryLocationLabel.Location = new System.Drawing.Point(3, 80);
            this.privacyOptOutPatientDirectoryLocationLabel.Name = "privacyOptOutPatientDirectoryLocationLabel";
            this.privacyOptOutPatientDirectoryLocationLabel.Size = new System.Drawing.Size(361, 16);
            this.privacyOptOutPatientDirectoryLocationLabel.TabIndex = 429;
            this.privacyOptOutPatientDirectoryLocationLabel.Text = "Privacy – Opt out (exclude) from patient directory - Location:";
            // 
            // privacyConfidentialStatusValueLabel
            // 
            this.privacyConfidentialStatusValueLabel.AutoSize = true;
            this.privacyConfidentialStatusValueLabel.Location = new System.Drawing.Point(465, 40);
            this.privacyConfidentialStatusValueLabel.Name = "privacyConfidentialStatusValueLabel";
            this.privacyConfidentialStatusValueLabel.Size = new System.Drawing.Size(12, 17);
            this.privacyConfidentialStatusValueLabel.TabIndex = 426;
            this.privacyConfidentialStatusValueLabel.Text = " ";
            // 
            // privacyOptOutPatientDirectoryNameAndAllInformationValueLabel
            // 
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.AutoSize = true;
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.Location = new System.Drawing.Point(465, 60);
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.Name = "privacyOptOutPatientDirectoryNameAndAllInformationValueLabel";
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.Size = new System.Drawing.Size(0, 17);
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.TabIndex = 428;
            // 
            // rightToRestrictLabel
            // 
            this.rightToRestrictLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rightToRestrictLabel.Location = new System.Drawing.Point(3, 140);
            this.rightToRestrictLabel.Name = "rightToRestrictLabel";
            this.rightToRestrictLabel.Size = new System.Drawing.Size(339, 16);
            this.rightToRestrictLabel.TabIndex = 440;
            this.rightToRestrictLabel.Text = "Patient Requested Right to Restrict:";
            // 
            // rightToRestrictValueLabel
            // 
            this.rightToRestrictValueLabel.AutoSize = true;
            this.rightToRestrictValueLabel.Location = new System.Drawing.Point(465, 140);
            this.rightToRestrictValueLabel.Name = "rightToRestrictValueLabel";
            this.rightToRestrictValueLabel.Size = new System.Drawing.Size(32, 17);
            this.rightToRestrictValueLabel.TabIndex = 441;
            this.rightToRestrictValueLabel.Text = "Yes";
            // 
            // cosSignedLabel
            // 
            this.cosSignedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cosSignedLabel.Location = new System.Drawing.Point(3, 160);
            this.cosSignedLabel.Name = "cosSignedLabel";
            this.cosSignedLabel.Size = new System.Drawing.Size(71, 16);
            this.cosSignedLabel.TabIndex = 439;
            this.cosSignedLabel.Text = "COS signed:";
            // 
            // cosSignedValueLabel
            // 
            this.cosSignedValueLabel.AutoSize = true;
            this.cosSignedValueLabel.Location = new System.Drawing.Point(465, 160);
            this.cosSignedValueLabel.Name = "cosSignedValueLabel";
            this.cosSignedValueLabel.Size = new System.Drawing.Size(145, 17);
            this.cosSignedValueLabel.TabIndex = 438;
            this.cosSignedValueLabel.Text = "cosSignedValueLabel";
            // 
            // contactEmailLabel
            // 
            this.contactEmailLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contactEmailLabel.Location = new System.Drawing.Point(3, 180);
            this.contactEmailLabel.Name = "contactEmailLabel";
            this.contactEmailLabel.Size = new System.Drawing.Size(200, 16);
            this.contactEmailLabel.TabIndex = 35;
            this.contactEmailLabel.Text = "Email:";
            // 
            // contactEmailValueLabel
            // 
            this.contactEmailValueLabel.AutoSize = true;
            this.contactEmailValueLabel.Location = new System.Drawing.Point(465, 180);
            this.contactEmailValueLabel.Name = "contactEmailValueLabel";
            this.contactEmailValueLabel.Size = new System.Drawing.Size(12, 17);
            this.contactEmailValueLabel.TabIndex = 36;
            this.contactEmailValueLabel.Text = " ";
            // 
            // ShareDataWithPublicHIEFlagLabel
            // 
            this.ShareDataWithPublicHIEFlagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShareDataWithPublicHIEFlagLabel.Location = new System.Drawing.Point(3, 200);
            this.ShareDataWithPublicHIEFlagLabel.Name = "ShareDataWithPublicHIEFlagLabel";
            this.ShareDataWithPublicHIEFlagLabel.Size = new System.Drawing.Size(300, 20);
            this.ShareDataWithPublicHIEFlagLabel.TabIndex = 443;
            this.ShareDataWithPublicHIEFlagLabel.Text = "Share with Health Information Exchange:";
            // 
            // ShareDataWithPublicHIEValueLabel
            // 
            this.ShareDataWithPublicHIEValueLabel.AutoSize = true;
            this.ShareDataWithPublicHIEValueLabel.Location = new System.Drawing.Point(465, 200);
            this.ShareDataWithPublicHIEValueLabel.Name = "ShareDataWithPublicHIEValueLabel";
            this.ShareDataWithPublicHIEValueLabel.Size = new System.Drawing.Size(46, 17);
            this.ShareDataWithPublicHIEValueLabel.TabIndex = 445;
            this.ShareDataWithPublicHIEValueLabel.Text = "label2";
            // 
            // ShareDataWithPCPFlagLabel
            // 
            this.ShareDataWithPCPFlagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShareDataWithPCPFlagLabel.Location = new System.Drawing.Point(3, 220);
            this.ShareDataWithPCPFlagLabel.Name = "ShareDataWithPCPFlagLabel";
            this.ShareDataWithPCPFlagLabel.Size = new System.Drawing.Size(150, 19);
            this.ShareDataWithPCPFlagLabel.TabIndex = 444;
            this.ShareDataWithPCPFlagLabel.Text = "Notify PCP of Visit:";
            // 
            // ShareDataWithPCPValueLabel
            // 
            this.ShareDataWithPCPValueLabel.AutoSize = true;
            this.ShareDataWithPCPValueLabel.Location = new System.Drawing.Point(465, 220);
            this.ShareDataWithPCPValueLabel.Name = "ShareDataWithPCPValueLabel";
            this.ShareDataWithPCPValueLabel.Size = new System.Drawing.Size(46, 17);
            this.ShareDataWithPCPValueLabel.TabIndex = 446;
            this.ShareDataWithPCPValueLabel.Text = "label3";
            // 
            // HospCommoptinlLabel
            // 
            this.HospCommoptinlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HospCommoptinlLabel.Location = new System.Drawing.Point(3, 239);
            this.HospCommoptinlLabel.Name = "HospCommoptinlLabel";
            this.HospCommoptinlLabel.Size = new System.Drawing.Size(169, 20);
            this.HospCommoptinlLabel.TabIndex = 447;
            this.HospCommoptinlLabel.Text = "Hosp Comm Optin:";
            // 
            // HospCommoptinValueLabel
            // 
            this.HospCommoptinValueLabel.AutoSize = true;
            this.HospCommoptinValueLabel.Location = new System.Drawing.Point(465, 239);
            this.HospCommoptinValueLabel.Name = "HospCommoptinValueLabel";
            this.HospCommoptinValueLabel.Size = new System.Drawing.Size(32, 17);
            this.HospCommoptinValueLabel.TabIndex = 448;
            this.HospCommoptinValueLabel.Text = "Yes";
            // 
            // cobReceivedFlagLabel
            // 
            this.cobReceivedFlagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cobReceivedFlagLabel.Location = new System.Drawing.Point(3, 260);
            this.cobReceivedFlagLabel.Name = "cobReceivedFlagLabel";
            this.cobReceivedFlagLabel.Size = new System.Drawing.Size(115, 20);
            this.cobReceivedFlagLabel.TabIndex = 449;
            this.cobReceivedFlagLabel.Text = "COB Received:";
            // 
            // cobReceivedValueLabel
            // 
            this.cobReceivedValueLabel.AutoSize = true;
            this.cobReceivedValueLabel.Location = new System.Drawing.Point(465, 260);
            this.cobReceivedValueLabel.Name = "cobReceivedValueLabel";
            this.cobReceivedValueLabel.Size = new System.Drawing.Size(32, 17);
            this.cobReceivedValueLabel.TabIndex = 450;
            this.cobReceivedValueLabel.Text = "Yes";
            // 
            // imfmReceivedFlagLabel
            // 
            this.imfmReceivedFlagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.imfmReceivedFlagLabel.Location = new System.Drawing.Point(3, 280);
            this.imfmReceivedFlagLabel.Name = "imfmReceivedFlagLabel";
            this.imfmReceivedFlagLabel.Size = new System.Drawing.Size(119, 20);
            this.imfmReceivedFlagLabel.TabIndex = 451;
            this.imfmReceivedFlagLabel.Text = "IMFM Received:";
            // 
            // imfmReceivedValueLabel
            // 
            this.imfmReceivedValueLabel.AutoSize = true;
            this.imfmReceivedValueLabel.Location = new System.Drawing.Point(465, 280);
            this.imfmReceivedValueLabel.Name = "imfmReceivedValueLabel";
            this.imfmReceivedValueLabel.Size = new System.Drawing.Size(36, 17);
            this.imfmReceivedValueLabel.TabIndex = 452;
            this.imfmReceivedValueLabel.Text = " Yes";
            // 
            // liabilityBody
            // 
            this.liabilityBody.ColumnCount = 2;
            this.liabilityBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.57315F));
            this.liabilityBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.42685F));
            this.liabilityBody.Controls.Add(this.patientHasNoLiabilityInsuredLabel, 0, 0);
            this.liabilityBody.Controls.Add(this.patientHasNoLiabilityInsuredValueLabel, 1, 0);
            this.liabilityBody.Controls.Add(this.deductibleInsuredLabel, 0, 1);
            this.liabilityBody.Controls.Add(this.deductibleInsuredValueLabel, 1, 1);
            this.liabilityBody.Controls.Add(this.coPayCoInsInsuredValueLabel, 1, 2);
            this.liabilityBody.Controls.Add(this.patientHasNoLiabilityUninsuredValueLabel, 1, 3);
            this.liabilityBody.Controls.Add(this.estimateForCurrentAmountDueUninsuredValueLabel, 1, 4);
            this.liabilityBody.Controls.Add(this.coPayCoInsInsuredLabel, 0, 2);
            this.liabilityBody.Controls.Add(this.patientHasNoLiabilityUninsuredLabel, 0, 3);
            this.liabilityBody.Controls.Add(this.estimateForCurrentAmountDueUninsuredLabel, 0, 4);
            this.liabilityBody.Location = new System.Drawing.Point(3, 4629);
            this.liabilityBody.Name = "liabilityBody";
            this.liabilityBody.RowCount = 5;
            this.liabilityBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.liabilityBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.liabilityBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.liabilityBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.liabilityBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.liabilityBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.liabilityBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.liabilityBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.liabilityBody.Size = new System.Drawing.Size(765, 104);
            this.liabilityBody.TabIndex = 556;
            // 
            // patientHasNoLiabilityInsuredLabel
            // 
            this.patientHasNoLiabilityInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientHasNoLiabilityInsuredLabel.Location = new System.Drawing.Point(3, 0);
            this.patientHasNoLiabilityInsuredLabel.Name = "patientHasNoLiabilityInsuredLabel";
            this.patientHasNoLiabilityInsuredLabel.Size = new System.Drawing.Size(231, 16);
            this.patientHasNoLiabilityInsuredLabel.TabIndex = 384;
            this.patientHasNoLiabilityInsuredLabel.Text = "Patient has no liability (insured):";
            // 
            // patientHasNoLiabilityInsuredValueLabel
            // 
            this.patientHasNoLiabilityInsuredValueLabel.AutoSize = true;
            this.patientHasNoLiabilityInsuredValueLabel.Location = new System.Drawing.Point(313, 0);
            this.patientHasNoLiabilityInsuredValueLabel.Name = "patientHasNoLiabilityInsuredValueLabel";
            this.patientHasNoLiabilityInsuredValueLabel.Size = new System.Drawing.Size(12, 17);
            this.patientHasNoLiabilityInsuredValueLabel.TabIndex = 385;
            this.patientHasNoLiabilityInsuredValueLabel.Text = " ";
            // 
            // deductibleInsuredLabel
            // 
            this.deductibleInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deductibleInsuredLabel.Location = new System.Drawing.Point(3, 20);
            this.deductibleInsuredLabel.Name = "deductibleInsuredLabel";
            this.deductibleInsuredLabel.Size = new System.Drawing.Size(138, 16);
            this.deductibleInsuredLabel.TabIndex = 386;
            this.deductibleInsuredLabel.Text = "Deductible (insured):";
            // 
            // deductibleInsuredValueLabel
            // 
            this.deductibleInsuredValueLabel.AutoSize = true;
            this.deductibleInsuredValueLabel.Location = new System.Drawing.Point(313, 20);
            this.deductibleInsuredValueLabel.Name = "deductibleInsuredValueLabel";
            this.deductibleInsuredValueLabel.Size = new System.Drawing.Size(0, 17);
            this.deductibleInsuredValueLabel.TabIndex = 387;
            // 
            // coPayCoInsInsuredValueLabel
            // 
            this.coPayCoInsInsuredValueLabel.AutoSize = true;
            this.coPayCoInsInsuredValueLabel.Location = new System.Drawing.Point(313, 40);
            this.coPayCoInsInsuredValueLabel.Name = "coPayCoInsInsuredValueLabel";
            this.coPayCoInsInsuredValueLabel.Size = new System.Drawing.Size(0, 17);
            this.coPayCoInsInsuredValueLabel.TabIndex = 389;
            // 
            // patientHasNoLiabilityUninsuredValueLabel
            // 
            this.patientHasNoLiabilityUninsuredValueLabel.AutoSize = true;
            this.patientHasNoLiabilityUninsuredValueLabel.Location = new System.Drawing.Point(313, 60);
            this.patientHasNoLiabilityUninsuredValueLabel.Name = "patientHasNoLiabilityUninsuredValueLabel";
            this.patientHasNoLiabilityUninsuredValueLabel.Size = new System.Drawing.Size(0, 17);
            this.patientHasNoLiabilityUninsuredValueLabel.TabIndex = 391;
            // 
            // estimateForCurrentAmountDueUninsuredValueLabel
            // 
            this.estimateForCurrentAmountDueUninsuredValueLabel.AutoSize = true;
            this.estimateForCurrentAmountDueUninsuredValueLabel.Location = new System.Drawing.Point(313, 80);
            this.estimateForCurrentAmountDueUninsuredValueLabel.Name = "estimateForCurrentAmountDueUninsuredValueLabel";
            this.estimateForCurrentAmountDueUninsuredValueLabel.Size = new System.Drawing.Size(0, 17);
            this.estimateForCurrentAmountDueUninsuredValueLabel.TabIndex = 393;
            // 
            // coPayCoInsInsuredLabel
            // 
            this.coPayCoInsInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.coPayCoInsInsuredLabel.Location = new System.Drawing.Point(3, 40);
            this.coPayCoInsInsuredLabel.Name = "coPayCoInsInsuredLabel";
            this.coPayCoInsInsuredLabel.Size = new System.Drawing.Size(145, 16);
            this.coPayCoInsInsuredLabel.TabIndex = 388;
            this.coPayCoInsInsuredLabel.Text = "Co-pay/co-ins (insured):";
            // 
            // patientHasNoLiabilityUninsuredLabel
            // 
            this.patientHasNoLiabilityUninsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientHasNoLiabilityUninsuredLabel.Location = new System.Drawing.Point(3, 60);
            this.patientHasNoLiabilityUninsuredLabel.Name = "patientHasNoLiabilityUninsuredLabel";
            this.patientHasNoLiabilityUninsuredLabel.Size = new System.Drawing.Size(236, 16);
            this.patientHasNoLiabilityUninsuredLabel.TabIndex = 390;
            this.patientHasNoLiabilityUninsuredLabel.Text = "Patient has no liability (uninsured):";
            // 
            // estimateForCurrentAmountDueUninsuredLabel
            // 
            this.estimateForCurrentAmountDueUninsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.estimateForCurrentAmountDueUninsuredLabel.Location = new System.Drawing.Point(3, 80);
            this.estimateForCurrentAmountDueUninsuredLabel.Name = "estimateForCurrentAmountDueUninsuredLabel";
            this.estimateForCurrentAmountDueUninsuredLabel.Size = new System.Drawing.Size(304, 16);
            this.estimateForCurrentAmountDueUninsuredLabel.TabIndex = 392;
            this.estimateForCurrentAmountDueUninsuredLabel.Text = "Estimate for current amount due (uninsured):";
            // 
            // regulatoryHeader
            // 
            this.regulatoryHeader.ColumnCount = 1;
            this.regulatoryHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.regulatoryHeader.Controls.Add(this.panel1, 0, 1);
            this.regulatoryHeader.Controls.Add(this.regulatoryLabel, 0, 0);
            this.regulatoryHeader.Location = new System.Drawing.Point(3, 4883);
            this.regulatoryHeader.Name = "regulatoryHeader";
            this.regulatoryHeader.RowCount = 2;
            this.regulatoryHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46.42857F));
            this.regulatoryHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.57143F));
            this.regulatoryHeader.Size = new System.Drawing.Size(597, 49);
            this.regulatoryHeader.TabIndex = 551;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(3, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(591, 1);
            this.panel1.TabIndex = 440;
            // 
            // regulatoryLabel
            // 
            this.regulatoryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regulatoryLabel.Location = new System.Drawing.Point(3, 0);
            this.regulatoryLabel.Name = "regulatoryLabel";
            this.regulatoryLabel.Size = new System.Drawing.Size(100, 20);
            this.regulatoryLabel.TabIndex = 420;
            this.regulatoryLabel.Text = "Regulatory";
            // 
            // generalInsuranceBody
            // 
            this.generalInsuranceBody.ColumnCount = 2;
            this.generalInsuranceBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.61152F));
            this.generalInsuranceBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.38848F));
            this.generalInsuranceBody.Controls.Add(this.financialClassValueLabel, 1, 0);
            this.generalInsuranceBody.Controls.Add(this.financialClassLabel, 0, 0);
            this.generalInsuranceBody.Controls.Add(this.motherDobValueLabel, 1, 1);
            this.generalInsuranceBody.Controls.Add(this.motherDobLabel, 0, 1);
            this.generalInsuranceBody.Controls.Add(this.fatherDobLabel, 0, 2);
            this.generalInsuranceBody.Controls.Add(this.fatherDobValueLabel, 1, 2);
            this.generalInsuranceBody.Location = new System.Drawing.Point(3, 1261);
            this.generalInsuranceBody.Name = "generalInsuranceBody";
            this.generalInsuranceBody.RowCount = 3;
            this.generalInsuranceBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.generalInsuranceBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.generalInsuranceBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.generalInsuranceBody.Size = new System.Drawing.Size(677, 67);
            this.generalInsuranceBody.TabIndex = 582;
            // 
            // financialClassValueLabel
            // 
            this.financialClassValueLabel.AutoSize = true;
            this.financialClassValueLabel.Location = new System.Drawing.Point(128, 0);
            this.financialClassValueLabel.Name = "financialClassValueLabel";
            this.financialClassValueLabel.Size = new System.Drawing.Size(0, 17);
            this.financialClassValueLabel.TabIndex = 135;
            // 
            // financialClassLabel
            // 
            this.financialClassLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.financialClassLabel.Location = new System.Drawing.Point(3, 0);
            this.financialClassLabel.Name = "financialClassLabel";
            this.financialClassLabel.Size = new System.Drawing.Size(119, 20);
            this.financialClassLabel.TabIndex = 134;
            this.financialClassLabel.Text = "Financial class:";
            // 
            // motherDobValueLabel
            // 
            this.motherDobValueLabel.AutoSize = true;
            this.motherDobValueLabel.Location = new System.Drawing.Point(128, 20);
            this.motherDobValueLabel.Name = "motherDobValueLabel";
            this.motherDobValueLabel.Size = new System.Drawing.Size(0, 17);
            this.motherDobValueLabel.TabIndex = 137;
            // 
            // motherDobLabel
            // 
            this.motherDobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.motherDobLabel.Location = new System.Drawing.Point(3, 20);
            this.motherDobLabel.Name = "motherDobLabel";
            this.motherDobLabel.Size = new System.Drawing.Size(119, 16);
            this.motherDobLabel.TabIndex = 136;
            this.motherDobLabel.Text = "Mother’s DOB:";
            // 
            // fatherDobLabel
            // 
            this.fatherDobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fatherDobLabel.Location = new System.Drawing.Point(3, 40);
            this.fatherDobLabel.Name = "fatherDobLabel";
            this.fatherDobLabel.Size = new System.Drawing.Size(119, 16);
            this.fatherDobLabel.TabIndex = 138;
            this.fatherDobLabel.Text = "Father’s DOB:";
            // 
            // fatherDobValueLabel
            // 
            this.fatherDobValueLabel.AutoSize = true;
            this.fatherDobValueLabel.Location = new System.Drawing.Point(128, 40);
            this.fatherDobValueLabel.Name = "fatherDobValueLabel";
            this.fatherDobValueLabel.Size = new System.Drawing.Size(0, 17);
            this.fatherDobValueLabel.TabIndex = 139;
            // 
            // liabilityHeader
            // 
            this.liabilityHeader.ColumnCount = 1;
            this.liabilityHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.liabilityHeader.Controls.Add(this.panel3, 0, 2);
            this.liabilityHeader.Controls.Add(this.liabilityLabel, 0, 1);
            this.liabilityHeader.Location = new System.Drawing.Point(3, 4578);
            this.liabilityHeader.Name = "liabilityHeader";
            this.liabilityHeader.RowCount = 3;
            this.liabilityHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.liabilityHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.liabilityHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.liabilityHeader.Size = new System.Drawing.Size(597, 38);
            this.liabilityHeader.TabIndex = 555;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(3, 23);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(591, 1);
            this.panel3.TabIndex = 441;
            // 
            // liabilityLabel
            // 
            this.liabilityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.liabilityLabel.Location = new System.Drawing.Point(3, 0);
            this.liabilityLabel.Name = "liabilityLabel";
            this.liabilityLabel.Size = new System.Drawing.Size(68, 20);
            this.liabilityLabel.TabIndex = 383;
            this.liabilityLabel.Text = "Liability";
            // 
            // diagnosisAndClinicalBody
            // 
            this.diagnosisAndClinicalBody.ColumnCount = 2;
            this.diagnosisAndClinicalBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.0335F));
            this.diagnosisAndClinicalBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.9665F));
            this.diagnosisAndClinicalBody.Controls.Add(this.CptCodesValueLabel, 0, 5);
            this.diagnosisAndClinicalBody.Controls.Add(this.CptCodesLabel, 0, 5);
            this.diagnosisAndClinicalBody.Controls.Add(this.commentsLabel, 0, 6);
            this.diagnosisAndClinicalBody.Controls.Add(this.patientTypeLabel, 0, 0);
            this.diagnosisAndClinicalBody.Controls.Add(this.dataGridView2, 0, 25);
            this.diagnosisAndClinicalBody.Controls.Add(this.patientTypeValueLabel, 1, 0);
            this.diagnosisAndClinicalBody.Controls.Add(this.reregisterLabel, 0, 1);
            this.diagnosisAndClinicalBody.Controls.Add(this.reregisterValueLabel, 1, 1);
            this.diagnosisAndClinicalBody.Controls.Add(this.hospitalServiceLabel, 0, 2);
            this.diagnosisAndClinicalBody.Controls.Add(this.hospitalServiceValueLabel, 1, 2);
            this.diagnosisAndClinicalBody.Controls.Add(this.chiefComplaintLabel, 0, 3);
            this.diagnosisAndClinicalBody.Controls.Add(this.chiefComplaintValueLabel, 1, 3);
            this.diagnosisAndClinicalBody.Controls.Add(this.procedureLabel, 0, 4);
            this.diagnosisAndClinicalBody.Controls.Add(this.procedureValueLabel, 1, 4);
            this.diagnosisAndClinicalBody.Controls.Add(this.patientVisitLabel, 0, 7);
            this.diagnosisAndClinicalBody.Controls.Add(this.patientVisitValueLabel, 1, 7);
            this.diagnosisAndClinicalBody.Controls.Add(this.PatientInClinicalStudyValueLabel, 1, 24);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentTypeLabel, 0, 8);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentTypeValueLabel, 1, 8);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentCrimeDateLabel, 0, 9);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentCrimeDateValueLabel, 1, 9);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentCrimeHourLabel, 0, 10);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentCrimeHourValueLabel, 1, 10);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentCrimeCountryLabel, 0, 11);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentCrimeCountryValueLabel, 1, 11);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentCrimeStateProvinceLabel, 0, 12);
            this.diagnosisAndClinicalBody.Controls.Add(this.accidentCrimeStateProvinceValueLabel, 1, 12);
            this.diagnosisAndClinicalBody.Controls.Add(this.dateOfOnsetForSymptomsIllnessLabel, 0, 13);
            this.diagnosisAndClinicalBody.Controls.Add(this.dateOfOnsetForSymptomsIllnessValueLabel, 1, 13);
            this.diagnosisAndClinicalBody.Controls.Add(this.admitSourceLabel, 0, 14);
            this.diagnosisAndClinicalBody.Controls.Add(this.admitSourceValueLabel, 1, 14);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic1Label, 0, 15);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic1ValueLabel, 1, 15);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic2Label, 0, 16);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic2ValueLabel, 1, 16);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic3Label, 0, 17);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic3ValueLabel, 1, 17);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic4Label, 0, 18);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic4ValueLabel, 1, 18);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic5Label, 0, 19);
            this.diagnosisAndClinicalBody.Controls.Add(this.clinic5ValueLabel, 1, 19);
            this.diagnosisAndClinicalBody.Controls.Add(this.referringPhysicianLabel, 0, 20);
            this.diagnosisAndClinicalBody.Controls.Add(this.referringPhysicianValueLabel, 1, 20);
            this.diagnosisAndClinicalBody.Controls.Add(this.admittingPhysicianLabel, 0, 21);
            this.diagnosisAndClinicalBody.Controls.Add(this.admittingPhysicianValueLabel, 1, 21);
            this.diagnosisAndClinicalBody.Controls.Add(this.attendingPhysicianLabel, 0, 22);
            this.diagnosisAndClinicalBody.Controls.Add(this.attendingPhysicianValueLabel, 1, 22);
            this.diagnosisAndClinicalBody.Controls.Add(this.primaryCarePhysicianLabel, 0, 23);
            this.diagnosisAndClinicalBody.Controls.Add(this.primaryCarePhysicianValueLabel, 1, 23);
            this.diagnosisAndClinicalBody.Controls.Add(this.PatientInClinicalREsearchLBL, 0, 24);
            this.diagnosisAndClinicalBody.Controls.Add(this.commentsValueLabel, 1, 6);
            this.diagnosisAndClinicalBody.Location = new System.Drawing.Point(3, 555);
            this.diagnosisAndClinicalBody.Name = "diagnosisAndClinicalBody";
            this.diagnosisAndClinicalBody.RowCount = 26;
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.diagnosisAndClinicalBody.Size = new System.Drawing.Size(765, 657);
            this.diagnosisAndClinicalBody.TabIndex = 550;
            // 
            // CptCodesValueLabel
            // 
            this.CptCodesValueLabel.AutoSize = true;
            this.CptCodesValueLabel.Location = new System.Drawing.Point(309, 100);
            this.CptCodesValueLabel.Name = "CptCodesValueLabel";
            this.CptCodesValueLabel.Size = new System.Drawing.Size(0, 17);
            this.CptCodesValueLabel.TabIndex = 550;
            // 
            // CptCodesLabel
            // 
            this.CptCodesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CptCodesLabel.Location = new System.Drawing.Point(3, 100);
            this.CptCodesLabel.Name = "CptCodesLabel";
            this.CptCodesLabel.Size = new System.Drawing.Size(79, 16);
            this.CptCodesLabel.TabIndex = 549;
            this.CptCodesLabel.Text = "CPT Codes :";
            // 
            // commentsLabel
            // 
            this.commentsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commentsLabel.Location = new System.Drawing.Point(3, 120);
            this.commentsLabel.Name = "commentsLabel";
            this.commentsLabel.Size = new System.Drawing.Size(64, 16);
            this.commentsLabel.TabIndex = 547;
            this.commentsLabel.Text = "Comments:";
            // 
            // patientTypeLabel
            // 
            this.patientTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientTypeLabel.Location = new System.Drawing.Point(3, 0);
            this.patientTypeLabel.Name = "patientTypeLabel";
            this.patientTypeLabel.Size = new System.Drawing.Size(98, 16);
            this.patientTypeLabel.TabIndex = 68;
            this.patientTypeLabel.Text = "Patient Type:";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeColumns = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PatientResearchStudyCode,
            this.PatientStudyDescription,
            this.PatientResearchSponsor,
            this.PatientRegistryNumber,
            this.ProofOfConsent});
            this.diagnosisAndClinicalBody.SetColumnSpan(this.dataGridView2, 2);
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView2.Location = new System.Drawing.Point(3, 503);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView2.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView2.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.ShowCellErrors = false;
            this.dataGridView2.ShowCellToolTips = false;
            this.dataGridView2.ShowEditingIcon = false;
            this.dataGridView2.ShowRowErrors = false;
            this.dataGridView2.Size = new System.Drawing.Size(759, 151);
            this.dataGridView2.TabIndex = 546;
            // 
            // PatientResearchStudyCode
            // 
            this.PatientResearchStudyCode.DataPropertyName = "Code";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.PatientResearchStudyCode.DefaultCellStyle = dataGridViewCellStyle2;
            this.PatientResearchStudyCode.HeaderText = "Clinical Research Code";
            this.PatientResearchStudyCode.MinimumWidth = 150;
            this.PatientResearchStudyCode.Name = "PatientResearchStudyCode";
            this.PatientResearchStudyCode.ReadOnly = true;
            this.PatientResearchStudyCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PatientResearchStudyCode.Width = 150;
            // 
            // PatientStudyDescription
            // 
            this.PatientStudyDescription.DataPropertyName = "Description";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.PatientStudyDescription.DefaultCellStyle = dataGridViewCellStyle3;
            this.PatientStudyDescription.HeaderText = "Description";
            this.PatientStudyDescription.MinimumWidth = 200;
            this.PatientStudyDescription.Name = "PatientStudyDescription";
            this.PatientStudyDescription.ReadOnly = true;
            this.PatientStudyDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PatientStudyDescription.Width = 200;
            // 
            // PatientResearchSponsor
            // 
            this.PatientResearchSponsor.DataPropertyName = "ResearchSponsor";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.PatientResearchSponsor.DefaultCellStyle = dataGridViewCellStyle4;
            this.PatientResearchSponsor.HeaderText = "Clinical Research Sponsor";
            this.PatientResearchSponsor.MinimumWidth = 200;
            this.PatientResearchSponsor.Name = "PatientResearchSponsor";
            this.PatientResearchSponsor.ReadOnly = true;
            this.PatientResearchSponsor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PatientResearchSponsor.Width = 200;
            // 
            // PatientRegistryNumber
            // 
            this.PatientRegistryNumber.DataPropertyName = "RegistryNumber";
            this.PatientRegistryNumber.HeaderText = "Registry Number";
            this.PatientRegistryNumber.Name = "PatientRegistryNumber";
            this.PatientRegistryNumber.ReadOnly = true;
            // 
            // ProofOfConsent
            // 
            this.ProofOfConsent.DataPropertyName = "ProofOfConsent";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.ProofOfConsent.DefaultCellStyle = dataGridViewCellStyle5;
            this.ProofOfConsent.HeaderText = "Proof of Consent?";
            this.ProofOfConsent.MinimumWidth = 120;
            this.ProofOfConsent.Name = "ProofOfConsent";
            this.ProofOfConsent.ReadOnly = true;
            this.ProofOfConsent.Width = 120;
            // 
            // patientTypeValueLabel
            // 
            this.patientTypeValueLabel.AutoSize = true;
            this.patientTypeValueLabel.Location = new System.Drawing.Point(309, 0);
            this.patientTypeValueLabel.Name = "patientTypeValueLabel";
            this.patientTypeValueLabel.Size = new System.Drawing.Size(0, 17);
            this.patientTypeValueLabel.TabIndex = 69;
            // 
            // reregisterLabel
            // 
            this.reregisterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reregisterLabel.Location = new System.Drawing.Point(3, 22);
            this.reregisterLabel.Name = "reregisterLabel";
            this.reregisterLabel.Size = new System.Drawing.Size(89, 16);
            this.reregisterLabel.TabIndex = 70;
            this.reregisterLabel.Text = "Reregister:";
            // 
            // reregisterValueLabel
            // 
            this.reregisterValueLabel.AutoSize = true;
            this.reregisterValueLabel.Location = new System.Drawing.Point(309, 22);
            this.reregisterValueLabel.Name = "reregisterValueLabel";
            this.reregisterValueLabel.Size = new System.Drawing.Size(0, 17);
            this.reregisterValueLabel.TabIndex = 71;
            // 
            // hospitalServiceLabel
            // 
            this.hospitalServiceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hospitalServiceLabel.Location = new System.Drawing.Point(3, 40);
            this.hospitalServiceLabel.Name = "hospitalServiceLabel";
            this.hospitalServiceLabel.Size = new System.Drawing.Size(119, 15);
            this.hospitalServiceLabel.TabIndex = 72;
            this.hospitalServiceLabel.Text = "Hospital service:";
            // 
            // hospitalServiceValueLabel
            // 
            this.hospitalServiceValueLabel.AutoSize = true;
            this.hospitalServiceValueLabel.Location = new System.Drawing.Point(309, 40);
            this.hospitalServiceValueLabel.Name = "hospitalServiceValueLabel";
            this.hospitalServiceValueLabel.Size = new System.Drawing.Size(0, 17);
            this.hospitalServiceValueLabel.TabIndex = 73;
            // 
            // chiefComplaintLabel
            // 
            this.chiefComplaintLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chiefComplaintLabel.Location = new System.Drawing.Point(3, 60);
            this.chiefComplaintLabel.Name = "chiefComplaintLabel";
            this.chiefComplaintLabel.Size = new System.Drawing.Size(124, 16);
            this.chiefComplaintLabel.TabIndex = 78;
            this.chiefComplaintLabel.Text = "Chief complaint:";
            // 
            // chiefComplaintValueLabel
            // 
            this.chiefComplaintValueLabel.AutoSize = true;
            this.chiefComplaintValueLabel.Location = new System.Drawing.Point(309, 60);
            this.chiefComplaintValueLabel.Name = "chiefComplaintValueLabel";
            this.chiefComplaintValueLabel.Size = new System.Drawing.Size(0, 17);
            this.chiefComplaintValueLabel.TabIndex = 79;
            // 
            // procedureLabel
            // 
            this.procedureLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.procedureLabel.Location = new System.Drawing.Point(3, 80);
            this.procedureLabel.Name = "procedureLabel";
            this.procedureLabel.Size = new System.Drawing.Size(79, 16);
            this.procedureLabel.TabIndex = 520;
            this.procedureLabel.Text = "Procedure :";
            // 
            // procedureValueLabel
            // 
            this.procedureValueLabel.AutoSize = true;
            this.procedureValueLabel.Location = new System.Drawing.Point(309, 80);
            this.procedureValueLabel.Name = "procedureValueLabel";
            this.procedureValueLabel.Size = new System.Drawing.Size(0, 17);
            this.procedureValueLabel.TabIndex = 521;
            // 
            // patientVisitLabel
            // 
            this.patientVisitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientVisitLabel.Location = new System.Drawing.Point(3, 143);
            this.patientVisitLabel.Name = "patientVisitLabel";
            this.patientVisitLabel.Size = new System.Drawing.Size(200, 16);
            this.patientVisitLabel.TabIndex = 80;
            this.patientVisitLabel.Text = "The patient\'s visit is the result of:";
            // 
            // patientVisitValueLabel
            // 
            this.patientVisitValueLabel.AutoSize = true;
            this.patientVisitValueLabel.Location = new System.Drawing.Point(309, 143);
            this.patientVisitValueLabel.Name = "patientVisitValueLabel";
            this.patientVisitValueLabel.Size = new System.Drawing.Size(0, 17);
            this.patientVisitValueLabel.TabIndex = 81;
            // 
            // PatientInClinicalStudyValueLabel
            // 
            this.PatientInClinicalStudyValueLabel.AutoSize = true;
            this.PatientInClinicalStudyValueLabel.Location = new System.Drawing.Point(309, 480);
            this.PatientInClinicalStudyValueLabel.Name = "PatientInClinicalStudyValueLabel";
            this.PatientInClinicalStudyValueLabel.Size = new System.Drawing.Size(0, 17);
            this.PatientInClinicalStudyValueLabel.TabIndex = 528;
            // 
            // accidentTypeLabel
            // 
            this.accidentTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentTypeLabel.Location = new System.Drawing.Point(3, 160);
            this.accidentTypeLabel.Name = "accidentTypeLabel";
            this.accidentTypeLabel.Size = new System.Drawing.Size(102, 16);
            this.accidentTypeLabel.TabIndex = 82;
            this.accidentTypeLabel.Text = "Accident type:";
            // 
            // accidentTypeValueLabel
            // 
            this.accidentTypeValueLabel.AutoSize = true;
            this.accidentTypeValueLabel.Location = new System.Drawing.Point(309, 160);
            this.accidentTypeValueLabel.Name = "accidentTypeValueLabel";
            this.accidentTypeValueLabel.Size = new System.Drawing.Size(0, 17);
            this.accidentTypeValueLabel.TabIndex = 83;
            // 
            // accidentCrimeDateLabel
            // 
            this.accidentCrimeDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentCrimeDateLabel.Location = new System.Drawing.Point(3, 180);
            this.accidentCrimeDateLabel.Name = "accidentCrimeDateLabel";
            this.accidentCrimeDateLabel.Size = new System.Drawing.Size(157, 20);
            this.accidentCrimeDateLabel.TabIndex = 84;
            this.accidentCrimeDateLabel.Text = "Accident or crime date:";
            // 
            // accidentCrimeDateValueLabel
            // 
            this.accidentCrimeDateValueLabel.AutoSize = true;
            this.accidentCrimeDateValueLabel.Location = new System.Drawing.Point(309, 180);
            this.accidentCrimeDateValueLabel.Name = "accidentCrimeDateValueLabel";
            this.accidentCrimeDateValueLabel.Size = new System.Drawing.Size(0, 17);
            this.accidentCrimeDateValueLabel.TabIndex = 85;
            // 
            // accidentCrimeHourLabel
            // 
            this.accidentCrimeHourLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentCrimeHourLabel.Location = new System.Drawing.Point(3, 200);
            this.accidentCrimeHourLabel.Name = "accidentCrimeHourLabel";
            this.accidentCrimeHourLabel.Size = new System.Drawing.Size(149, 20);
            this.accidentCrimeHourLabel.TabIndex = 86;
            this.accidentCrimeHourLabel.Text = "Accident or crime hour:";
            // 
            // accidentCrimeHourValueLabel
            // 
            this.accidentCrimeHourValueLabel.AutoSize = true;
            this.accidentCrimeHourValueLabel.Location = new System.Drawing.Point(309, 200);
            this.accidentCrimeHourValueLabel.Name = "accidentCrimeHourValueLabel";
            this.accidentCrimeHourValueLabel.Size = new System.Drawing.Size(0, 17);
            this.accidentCrimeHourValueLabel.TabIndex = 87;
            // 
            // accidentCrimeCountryLabel
            // 
            this.accidentCrimeCountryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentCrimeCountryLabel.Location = new System.Drawing.Point(3, 220);
            this.accidentCrimeCountryLabel.Name = "accidentCrimeCountryLabel";
            this.accidentCrimeCountryLabel.Size = new System.Drawing.Size(157, 20);
            this.accidentCrimeCountryLabel.TabIndex = 88;
            this.accidentCrimeCountryLabel.Text = "Accident or crime country:";
            // 
            // accidentCrimeCountryValueLabel
            // 
            this.accidentCrimeCountryValueLabel.AutoSize = true;
            this.accidentCrimeCountryValueLabel.Location = new System.Drawing.Point(309, 220);
            this.accidentCrimeCountryValueLabel.Name = "accidentCrimeCountryValueLabel";
            this.accidentCrimeCountryValueLabel.Size = new System.Drawing.Size(0, 17);
            this.accidentCrimeCountryValueLabel.TabIndex = 89;
            // 
            // accidentCrimeStateProvinceLabel
            // 
            this.accidentCrimeStateProvinceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentCrimeStateProvinceLabel.Location = new System.Drawing.Point(3, 240);
            this.accidentCrimeStateProvinceLabel.Name = "accidentCrimeStateProvinceLabel";
            this.accidentCrimeStateProvinceLabel.Size = new System.Drawing.Size(201, 20);
            this.accidentCrimeStateProvinceLabel.TabIndex = 90;
            this.accidentCrimeStateProvinceLabel.Text = "Accident or crime state/province:";
            // 
            // accidentCrimeStateProvinceValueLabel
            // 
            this.accidentCrimeStateProvinceValueLabel.AutoSize = true;
            this.accidentCrimeStateProvinceValueLabel.Location = new System.Drawing.Point(309, 240);
            this.accidentCrimeStateProvinceValueLabel.Name = "accidentCrimeStateProvinceValueLabel";
            this.accidentCrimeStateProvinceValueLabel.Size = new System.Drawing.Size(0, 17);
            this.accidentCrimeStateProvinceValueLabel.TabIndex = 91;
            // 
            // dateOfOnsetForSymptomsIllnessLabel
            // 
            this.dateOfOnsetForSymptomsIllnessLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateOfOnsetForSymptomsIllnessLabel.Location = new System.Drawing.Point(3, 260);
            this.dateOfOnsetForSymptomsIllnessLabel.Name = "dateOfOnsetForSymptomsIllnessLabel";
            this.dateOfOnsetForSymptomsIllnessLabel.Size = new System.Drawing.Size(221, 20);
            this.dateOfOnsetForSymptomsIllnessLabel.TabIndex = 92;
            this.dateOfOnsetForSymptomsIllnessLabel.Text = "Date of onset for symptoms or illness:";
            // 
            // dateOfOnsetForSymptomsIllnessValueLabel
            // 
            this.dateOfOnsetForSymptomsIllnessValueLabel.AutoSize = true;
            this.dateOfOnsetForSymptomsIllnessValueLabel.Location = new System.Drawing.Point(309, 260);
            this.dateOfOnsetForSymptomsIllnessValueLabel.Name = "dateOfOnsetForSymptomsIllnessValueLabel";
            this.dateOfOnsetForSymptomsIllnessValueLabel.Size = new System.Drawing.Size(0, 17);
            this.dateOfOnsetForSymptomsIllnessValueLabel.TabIndex = 93;
            // 
            // admitSourceLabel
            // 
            this.admitSourceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.admitSourceLabel.Location = new System.Drawing.Point(3, 280);
            this.admitSourceLabel.Name = "admitSourceLabel";
            this.admitSourceLabel.Size = new System.Drawing.Size(85, 16);
            this.admitSourceLabel.TabIndex = 96;
            this.admitSourceLabel.Text = "Admit source:";
            // 
            // admitSourceValueLabel
            // 
            this.admitSourceValueLabel.AutoSize = true;
            this.admitSourceValueLabel.Location = new System.Drawing.Point(309, 280);
            this.admitSourceValueLabel.Name = "admitSourceValueLabel";
            this.admitSourceValueLabel.Size = new System.Drawing.Size(0, 17);
            this.admitSourceValueLabel.TabIndex = 97;
            // 
            // clinic1Label
            // 
            this.clinic1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic1Label.Location = new System.Drawing.Point(3, 300);
            this.clinic1Label.Name = "clinic1Label";
            this.clinic1Label.Size = new System.Drawing.Size(65, 16);
            this.clinic1Label.TabIndex = 102;
            this.clinic1Label.Text = "Clinic1:";
            // 
            // clinic1ValueLabel
            // 
            this.clinic1ValueLabel.AutoSize = true;
            this.clinic1ValueLabel.Location = new System.Drawing.Point(309, 300);
            this.clinic1ValueLabel.Name = "clinic1ValueLabel";
            this.clinic1ValueLabel.Size = new System.Drawing.Size(0, 17);
            this.clinic1ValueLabel.TabIndex = 103;
            // 
            // clinic2Label
            // 
            this.clinic2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic2Label.Location = new System.Drawing.Point(3, 320);
            this.clinic2Label.Name = "clinic2Label";
            this.clinic2Label.Size = new System.Drawing.Size(65, 16);
            this.clinic2Label.TabIndex = 104;
            this.clinic2Label.Text = "Clinic2:";
            // 
            // clinic2ValueLabel
            // 
            this.clinic2ValueLabel.AutoSize = true;
            this.clinic2ValueLabel.Location = new System.Drawing.Point(309, 320);
            this.clinic2ValueLabel.Name = "clinic2ValueLabel";
            this.clinic2ValueLabel.Size = new System.Drawing.Size(0, 17);
            this.clinic2ValueLabel.TabIndex = 105;
            // 
            // clinic3Label
            // 
            this.clinic3Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic3Label.Location = new System.Drawing.Point(3, 340);
            this.clinic3Label.Name = "clinic3Label";
            this.clinic3Label.Size = new System.Drawing.Size(65, 16);
            this.clinic3Label.TabIndex = 106;
            this.clinic3Label.Text = "Clinic3:";
            // 
            // clinic3ValueLabel
            // 
            this.clinic3ValueLabel.AutoSize = true;
            this.clinic3ValueLabel.Location = new System.Drawing.Point(309, 340);
            this.clinic3ValueLabel.Name = "clinic3ValueLabel";
            this.clinic3ValueLabel.Size = new System.Drawing.Size(0, 17);
            this.clinic3ValueLabel.TabIndex = 107;
            // 
            // clinic4Label
            // 
            this.clinic4Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic4Label.Location = new System.Drawing.Point(3, 360);
            this.clinic4Label.Name = "clinic4Label";
            this.clinic4Label.Size = new System.Drawing.Size(65, 16);
            this.clinic4Label.TabIndex = 108;
            this.clinic4Label.Text = "Clinic4:";
            // 
            // clinic4ValueLabel
            // 
            this.clinic4ValueLabel.AutoSize = true;
            this.clinic4ValueLabel.Location = new System.Drawing.Point(309, 360);
            this.clinic4ValueLabel.Name = "clinic4ValueLabel";
            this.clinic4ValueLabel.Size = new System.Drawing.Size(0, 17);
            this.clinic4ValueLabel.TabIndex = 109;
            // 
            // clinic5Label
            // 
            this.clinic5Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic5Label.Location = new System.Drawing.Point(3, 380);
            this.clinic5Label.Name = "clinic5Label";
            this.clinic5Label.Size = new System.Drawing.Size(65, 16);
            this.clinic5Label.TabIndex = 110;
            this.clinic5Label.Text = "Clinic5:";
            // 
            // clinic5ValueLabel
            // 
            this.clinic5ValueLabel.AutoSize = true;
            this.clinic5ValueLabel.Location = new System.Drawing.Point(309, 380);
            this.clinic5ValueLabel.Name = "clinic5ValueLabel";
            this.clinic5ValueLabel.Size = new System.Drawing.Size(0, 17);
            this.clinic5ValueLabel.TabIndex = 111;
            // 
            // referringPhysicianLabel
            // 
            this.referringPhysicianLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.referringPhysicianLabel.Location = new System.Drawing.Point(3, 400);
            this.referringPhysicianLabel.Name = "referringPhysicianLabel";
            this.referringPhysicianLabel.Size = new System.Drawing.Size(122, 16);
            this.referringPhysicianLabel.TabIndex = 113;
            this.referringPhysicianLabel.Text = "Referring physician:";
            // 
            // referringPhysicianValueLabel
            // 
            this.referringPhysicianValueLabel.AutoSize = true;
            this.referringPhysicianValueLabel.Location = new System.Drawing.Point(309, 400);
            this.referringPhysicianValueLabel.Name = "referringPhysicianValueLabel";
            this.referringPhysicianValueLabel.Size = new System.Drawing.Size(0, 17);
            this.referringPhysicianValueLabel.TabIndex = 114;
            // 
            // admittingPhysicianLabel
            // 
            this.admittingPhysicianLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.admittingPhysicianLabel.Location = new System.Drawing.Point(3, 420);
            this.admittingPhysicianLabel.Name = "admittingPhysicianLabel";
            this.admittingPhysicianLabel.Size = new System.Drawing.Size(122, 16);
            this.admittingPhysicianLabel.TabIndex = 115;
            this.admittingPhysicianLabel.Text = "Admitting physician:";
            // 
            // admittingPhysicianValueLabel
            // 
            this.admittingPhysicianValueLabel.AutoSize = true;
            this.admittingPhysicianValueLabel.Location = new System.Drawing.Point(309, 420);
            this.admittingPhysicianValueLabel.Name = "admittingPhysicianValueLabel";
            this.admittingPhysicianValueLabel.Size = new System.Drawing.Size(0, 17);
            this.admittingPhysicianValueLabel.TabIndex = 116;
            // 
            // attendingPhysicianLabel
            // 
            this.attendingPhysicianLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.attendingPhysicianLabel.Location = new System.Drawing.Point(3, 440);
            this.attendingPhysicianLabel.Name = "attendingPhysicianLabel";
            this.attendingPhysicianLabel.Size = new System.Drawing.Size(132, 16);
            this.attendingPhysicianLabel.TabIndex = 117;
            this.attendingPhysicianLabel.Text = "Attending physician:";
            // 
            // attendingPhysicianValueLabel
            // 
            this.attendingPhysicianValueLabel.AutoSize = true;
            this.attendingPhysicianValueLabel.Location = new System.Drawing.Point(309, 440);
            this.attendingPhysicianValueLabel.Name = "attendingPhysicianValueLabel";
            this.attendingPhysicianValueLabel.Size = new System.Drawing.Size(0, 17);
            this.attendingPhysicianValueLabel.TabIndex = 118;
            // 
            // primaryCarePhysicianLabel
            // 
            this.primaryCarePhysicianLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryCarePhysicianLabel.Location = new System.Drawing.Point(3, 460);
            this.primaryCarePhysicianLabel.Name = "primaryCarePhysicianLabel";
            this.primaryCarePhysicianLabel.Size = new System.Drawing.Size(139, 20);
            this.primaryCarePhysicianLabel.TabIndex = 121;
            this.primaryCarePhysicianLabel.Text = "PCP  physician:";
            // 
            // primaryCarePhysicianValueLabel
            // 
            this.primaryCarePhysicianValueLabel.AutoSize = true;
            this.primaryCarePhysicianValueLabel.Location = new System.Drawing.Point(309, 460);
            this.primaryCarePhysicianValueLabel.Name = "primaryCarePhysicianValueLabel";
            this.primaryCarePhysicianValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryCarePhysicianValueLabel.TabIndex = 122;
            // 
            // PatientInClinicalREsearchLBL
            // 
            this.PatientInClinicalREsearchLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PatientInClinicalREsearchLBL.Location = new System.Drawing.Point(3, 480);
            this.PatientInClinicalREsearchLBL.Name = "PatientInClinicalREsearchLBL";
            this.PatientInClinicalREsearchLBL.Size = new System.Drawing.Size(226, 20);
            this.PatientInClinicalREsearchLBL.TabIndex = 525;
            this.PatientInClinicalREsearchLBL.Text = "Is Patient In Clinical Research study:";
            // 
            // commentsValueLabel
            // 
            this.commentsValueLabel.Location = new System.Drawing.Point(309, 120);
            this.commentsValueLabel.Name = "commentsValueLabel";
            this.commentsValueLabel.Size = new System.Drawing.Size(453, 23);
            this.commentsValueLabel.TabIndex = 548;
            // 
            // diangnosisAndClinicalHeader
            // 
            this.diangnosisAndClinicalHeader.ColumnCount = 1;
            this.diangnosisAndClinicalHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.diangnosisAndClinicalHeader.Controls.Add(this.diagnosisLabel, 0, 0);
            this.diangnosisAndClinicalHeader.Controls.Add(this.diagnosisPanel, 0, 1);
            this.diangnosisAndClinicalHeader.Location = new System.Drawing.Point(3, 516);
            this.diangnosisAndClinicalHeader.Name = "diangnosisAndClinicalHeader";
            this.diangnosisAndClinicalHeader.RowCount = 2;
            this.diangnosisAndClinicalHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.diangnosisAndClinicalHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.diangnosisAndClinicalHeader.Size = new System.Drawing.Size(597, 33);
            this.diangnosisAndClinicalHeader.TabIndex = 550;
            // 
            // diagnosisLabel
            // 
            this.diagnosisLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.diagnosisLabel.Location = new System.Drawing.Point(3, 0);
            this.diagnosisLabel.Name = "diagnosisLabel";
            this.diagnosisLabel.Size = new System.Drawing.Size(241, 20);
            this.diagnosisLabel.TabIndex = 67;
            this.diagnosisLabel.Text = "Diagnosis and Clinical";
            // 
            // diagnosisPanel
            // 
            this.diagnosisPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.diagnosisPanel.Location = new System.Drawing.Point(3, 23);
            this.diagnosisPanel.Name = "diagnosisPanel";
            this.diagnosisPanel.Size = new System.Drawing.Size(591, 1);
            this.diagnosisPanel.TabIndex = 440;
            // 
            // dempgraphicsHeader
            // 
            this.dempgraphicsHeader.ColumnCount = 1;
            this.dempgraphicsHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dempgraphicsHeader.Controls.Add(this.patientDemographicsAndEmploymentLabel, 0, 0);
            this.dempgraphicsHeader.Controls.Add(this.patientDemographicsEmploymentPanel, 0, 1);
            this.dempgraphicsHeader.Location = new System.Drawing.Point(3, 11);
            this.dempgraphicsHeader.Name = "dempgraphicsHeader";
            this.dempgraphicsHeader.RowCount = 2;
            this.dempgraphicsHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.dempgraphicsHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dempgraphicsHeader.Size = new System.Drawing.Size(597, 39);
            this.dempgraphicsHeader.TabIndex = 548;
            // 
            // patientDemographicsAndEmploymentLabel
            // 
            this.patientDemographicsAndEmploymentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientDemographicsAndEmploymentLabel.Location = new System.Drawing.Point(3, 0);
            this.patientDemographicsAndEmploymentLabel.Name = "patientDemographicsAndEmploymentLabel";
            this.patientDemographicsAndEmploymentLabel.Size = new System.Drawing.Size(312, 20);
            this.patientDemographicsAndEmploymentLabel.TabIndex = 0;
            this.patientDemographicsAndEmploymentLabel.Text = "Patient Demographics";
            // 
            // patientDemographicsEmploymentPanel
            // 
            this.patientDemographicsEmploymentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.patientDemographicsEmploymentPanel.Location = new System.Drawing.Point(3, 23);
            this.patientDemographicsEmploymentPanel.Name = "patientDemographicsEmploymentPanel";
            this.patientDemographicsEmploymentPanel.Size = new System.Drawing.Size(591, 1);
            this.patientDemographicsEmploymentPanel.TabIndex = 439;
            // 
            // demographicsBody
            // 
            this.demographicsBody.ColumnCount = 2;
            this.demographicsBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 115.6884F));
            this.demographicsBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 350.3116F));
            this.demographicsBody.Controls.Add(this.label1, 0, 18);
            this.demographicsBody.Controls.Add(this.passportIssuingCountryValueLabel, 1, 18);
            this.demographicsBody.Controls.Add(this.patientNameLabel, 0, 0);
            this.demographicsBody.Controls.Add(this.patientNameValueLabel, 1, 0);
            this.demographicsBody.Controls.Add(this.akaLabel, 0, 1);
            this.demographicsBody.Controls.Add(this.akaValueLabel, 1, 1);
            this.demographicsBody.Controls.Add(this.admitDateTimeLabel, 0, 2);
            this.demographicsBody.Controls.Add(this.admitDateTimeValueLabel, 1, 2);
            this.demographicsBody.Controls.Add(this.dischargeDateTimeLabel, 0, 3);
            this.demographicsBody.Controls.Add(this.dischargeDateTimeValueLabel, 1, 3);
            this.demographicsBody.Controls.Add(this.dischargeDispositionLabel, 0, 4);
            this.demographicsBody.Controls.Add(this.dischargeDispositionValueLabel, 1, 4);
            this.demographicsBody.Controls.Add(this.appointmentLabel, 0, 5);
            this.demographicsBody.Controls.Add(this.appointmentValueLabel, 1, 5);
            this.demographicsBody.Controls.Add(this.genderLabel, 0, 6);
            this.demographicsBody.Controls.Add(this.genderValueLabel, 1, 6);
            this.demographicsBody.Controls.Add(this.dobLabel, 0, 7);
            this.demographicsBody.Controls.Add(this.dobValueLabel, 1, 7);
            this.demographicsBody.Controls.Add(this.ageLabel, 0, 8);
            this.demographicsBody.Controls.Add(this.ageValueLabel, 1, 8);
            this.demographicsBody.Controls.Add(this.maritalStatusLabel, 0, 9);
            this.demographicsBody.Controls.Add(this.maritalStatusValueLabel, 1, 9);
            this.demographicsBody.Controls.Add(this.raceLabel, 0, 10);
            this.demographicsBody.Controls.Add(this.raceValueLabel, 1, 10);
            this.demographicsBody.Controls.Add(this.ethnicityLabel, 0, 11);
            this.demographicsBody.Controls.Add(this.ethnicityValueLabel, 1, 11);
            this.demographicsBody.Controls.Add(this.ssnLabel, 0, 12);
            this.demographicsBody.Controls.Add(this.ssnValueLabel, 1, 12);
            this.demographicsBody.Controls.Add(this.patientMailingAddressLabel, 0, 13);
            this.demographicsBody.Controls.Add(this.patientMailingAddressValueLabel, 1, 13);
            this.demographicsBody.Controls.Add(this.contactPhoneLabel, 0, 14);
            this.demographicsBody.Controls.Add(this.contactPhoneValueLabel, 1, 14);
            this.demographicsBody.Controls.Add(this.contactCellLabel, 0, 15);
            this.demographicsBody.Controls.Add(this.contactCellValueLabel, 1, 15);
            this.demographicsBody.Controls.Add(this.driversLicenseLabel, 0, 16);
            this.demographicsBody.Controls.Add(this.driversLicenseValueLabel, 1, 16);
            this.demographicsBody.Controls.Add(this.passportLabel, 0, 17);
            this.demographicsBody.Controls.Add(this.passportValueLabel, 1, 17);
            this.demographicsBody.Controls.Add(this.languageLabel, 0, 19);
            this.demographicsBody.Controls.Add(this.languageValueLabel, 1, 19);
            this.demographicsBody.Controls.Add(this.emergencyContact1NameLabel, 0, 20);
            this.demographicsBody.Controls.Add(this.emergencyContact1NameValueLabel, 1, 20);
            this.demographicsBody.Controls.Add(this.emergencyContact1IsThePatientsLabel, 0, 21);
            this.demographicsBody.Controls.Add(this.emergencyContact1IsThePatientsValueLabel, 1, 21);
            this.demographicsBody.Controls.Add(this.emergencyContact1PhoneLabel, 0, 22);
            this.demographicsBody.Controls.Add(this.emergencyContact1PhoneValueLabel, 1, 22);
            this.demographicsBody.Location = new System.Drawing.Point(3, 56);
            this.demographicsBody.Name = "demographicsBody";
            this.demographicsBody.RowCount = 23;
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.demographicsBody.Size = new System.Drawing.Size(894, 454);
            this.demographicsBody.TabIndex = 547;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 360);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 16);
            this.label1.TabIndex = 413;
            this.label1.Text = "Passport Issuing Country:";
            // 
            // passportIssuingCountryValueLabel
            // 
            this.passportIssuingCountryValueLabel.AutoSize = true;
            this.passportIssuingCountryValueLabel.Location = new System.Drawing.Point(224, 360);
            this.passportIssuingCountryValueLabel.Name = "passportIssuingCountryValueLabel";
            this.passportIssuingCountryValueLabel.Size = new System.Drawing.Size(0, 17);
            this.passportIssuingCountryValueLabel.TabIndex = 412;
            // 
            // patientNameLabel
            // 
            this.patientNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientNameLabel.Location = new System.Drawing.Point(3, 0);
            this.patientNameLabel.Name = "patientNameLabel";
            this.patientNameLabel.Size = new System.Drawing.Size(119, 16);
            this.patientNameLabel.TabIndex = 1;
            this.patientNameLabel.Text = "Patient name:";
            // 
            // patientNameValueLabel
            // 
            this.patientNameValueLabel.AutoSize = true;
            this.patientNameValueLabel.Location = new System.Drawing.Point(224, 0);
            this.patientNameValueLabel.Name = "patientNameValueLabel";
            this.patientNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.patientNameValueLabel.TabIndex = 2;
            // 
            // akaLabel
            // 
            this.akaLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.akaLabel.Location = new System.Drawing.Point(3, 20);
            this.akaLabel.Name = "akaLabel";
            this.akaLabel.Size = new System.Drawing.Size(54, 16);
            this.akaLabel.TabIndex = 3;
            this.akaLabel.Text = "AKA:";
            // 
            // akaValueLabel
            // 
            this.akaValueLabel.AutoSize = true;
            this.akaValueLabel.Location = new System.Drawing.Point(224, 20);
            this.akaValueLabel.Name = "akaValueLabel";
            this.akaValueLabel.Size = new System.Drawing.Size(0, 17);
            this.akaValueLabel.TabIndex = 4;
            // 
            // admitDateTimeLabel
            // 
            this.admitDateTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.admitDateTimeLabel.Location = new System.Drawing.Point(3, 40);
            this.admitDateTimeLabel.Name = "admitDateTimeLabel";
            this.admitDateTimeLabel.Size = new System.Drawing.Size(119, 16);
            this.admitDateTimeLabel.TabIndex = 5;
            this.admitDateTimeLabel.Text = "Admit date/time:";
            // 
            // admitDateTimeValueLabel
            // 
            this.admitDateTimeValueLabel.AutoSize = true;
            this.admitDateTimeValueLabel.Location = new System.Drawing.Point(224, 40);
            this.admitDateTimeValueLabel.Name = "admitDateTimeValueLabel";
            this.admitDateTimeValueLabel.Size = new System.Drawing.Size(0, 17);
            this.admitDateTimeValueLabel.TabIndex = 6;
            // 
            // dischargeDateTimeLabel
            // 
            this.dischargeDateTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dischargeDateTimeLabel.Location = new System.Drawing.Point(3, 60);
            this.dischargeDateTimeLabel.Name = "dischargeDateTimeLabel";
            this.dischargeDateTimeLabel.Size = new System.Drawing.Size(143, 16);
            this.dischargeDateTimeLabel.TabIndex = 7;
            this.dischargeDateTimeLabel.Text = "Discharge date/time:";
            // 
            // dischargeDateTimeValueLabel
            // 
            this.dischargeDateTimeValueLabel.AutoSize = true;
            this.dischargeDateTimeValueLabel.Location = new System.Drawing.Point(224, 60);
            this.dischargeDateTimeValueLabel.Name = "dischargeDateTimeValueLabel";
            this.dischargeDateTimeValueLabel.Size = new System.Drawing.Size(0, 17);
            this.dischargeDateTimeValueLabel.TabIndex = 8;
            // 
            // dischargeDispositionLabel
            // 
            this.dischargeDispositionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dischargeDispositionLabel.Location = new System.Drawing.Point(3, 80);
            this.dischargeDispositionLabel.Name = "dischargeDispositionLabel";
            this.dischargeDispositionLabel.Size = new System.Drawing.Size(143, 16);
            this.dischargeDispositionLabel.TabIndex = 9;
            this.dischargeDispositionLabel.Text = "Discharge disposition:";
            // 
            // dischargeDispositionValueLabel
            // 
            this.dischargeDispositionValueLabel.AutoSize = true;
            this.dischargeDispositionValueLabel.Location = new System.Drawing.Point(224, 80);
            this.dischargeDispositionValueLabel.Name = "dischargeDispositionValueLabel";
            this.dischargeDispositionValueLabel.Size = new System.Drawing.Size(0, 17);
            this.dischargeDispositionValueLabel.TabIndex = 10;
            // 
            // appointmentLabel
            // 
            this.appointmentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appointmentLabel.Location = new System.Drawing.Point(3, 100);
            this.appointmentLabel.Name = "appointmentLabel";
            this.appointmentLabel.Size = new System.Drawing.Size(119, 16);
            this.appointmentLabel.TabIndex = 11;
            this.appointmentLabel.Text = "Appointment:";
            // 
            // appointmentValueLabel
            // 
            this.appointmentValueLabel.AutoSize = true;
            this.appointmentValueLabel.Location = new System.Drawing.Point(224, 100);
            this.appointmentValueLabel.Name = "appointmentValueLabel";
            this.appointmentValueLabel.Size = new System.Drawing.Size(0, 17);
            this.appointmentValueLabel.TabIndex = 12;
            // 
            // genderLabel
            // 
            this.genderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.genderLabel.Location = new System.Drawing.Point(3, 120);
            this.genderLabel.Name = "genderLabel";
            this.genderLabel.Size = new System.Drawing.Size(65, 16);
            this.genderLabel.TabIndex = 13;
            this.genderLabel.Text = "Gender:";
            // 
            // genderValueLabel
            // 
            this.genderValueLabel.AutoSize = true;
            this.genderValueLabel.Location = new System.Drawing.Point(224, 120);
            this.genderValueLabel.Name = "genderValueLabel";
            this.genderValueLabel.Size = new System.Drawing.Size(0, 17);
            this.genderValueLabel.TabIndex = 14;
            // 
            // dobLabel
            // 
            this.dobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dobLabel.Location = new System.Drawing.Point(3, 140);
            this.dobLabel.Name = "dobLabel";
            this.dobLabel.Size = new System.Drawing.Size(47, 16);
            this.dobLabel.TabIndex = 15;
            this.dobLabel.Text = "DOB:";
            // 
            // dobValueLabel
            // 
            this.dobValueLabel.AutoSize = true;
            this.dobValueLabel.Location = new System.Drawing.Point(224, 140);
            this.dobValueLabel.Name = "dobValueLabel";
            this.dobValueLabel.Size = new System.Drawing.Size(0, 17);
            this.dobValueLabel.TabIndex = 16;
            // 
            // ageLabel
            // 
            this.ageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ageLabel.Location = new System.Drawing.Point(3, 160);
            this.ageLabel.Name = "ageLabel";
            this.ageLabel.Size = new System.Drawing.Size(33, 16);
            this.ageLabel.TabIndex = 17;
            this.ageLabel.Text = "Age:";
            // 
            // ageValueLabel
            // 
            this.ageValueLabel.AutoSize = true;
            this.ageValueLabel.Location = new System.Drawing.Point(224, 160);
            this.ageValueLabel.Name = "ageValueLabel";
            this.ageValueLabel.Size = new System.Drawing.Size(0, 17);
            this.ageValueLabel.TabIndex = 18;
            // 
            // maritalStatusLabel
            // 
            this.maritalStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maritalStatusLabel.Location = new System.Drawing.Point(3, 180);
            this.maritalStatusLabel.Name = "maritalStatusLabel";
            this.maritalStatusLabel.Size = new System.Drawing.Size(95, 16);
            this.maritalStatusLabel.TabIndex = 19;
            this.maritalStatusLabel.Text = "Marital Status:";
            // 
            // maritalStatusValueLabel
            // 
            this.maritalStatusValueLabel.AutoSize = true;
            this.maritalStatusValueLabel.Location = new System.Drawing.Point(224, 180);
            this.maritalStatusValueLabel.Name = "maritalStatusValueLabel";
            this.maritalStatusValueLabel.Size = new System.Drawing.Size(0, 17);
            this.maritalStatusValueLabel.TabIndex = 20;
            // 
            // raceLabel
            // 
            this.raceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.raceLabel.Location = new System.Drawing.Point(3, 200);
            this.raceLabel.Name = "raceLabel";
            this.raceLabel.Size = new System.Drawing.Size(47, 16);
            this.raceLabel.TabIndex = 21;
            this.raceLabel.Text = "Race:";
            // 
            // raceValueLabel
            // 
            this.raceValueLabel.AutoSize = true;
            this.raceValueLabel.Location = new System.Drawing.Point(224, 200);
            this.raceValueLabel.Name = "raceValueLabel";
            this.raceValueLabel.Size = new System.Drawing.Size(0, 17);
            this.raceValueLabel.TabIndex = 22;
            // 
            // ethnicityLabel
            // 
            this.ethnicityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ethnicityLabel.Location = new System.Drawing.Point(3, 220);
            this.ethnicityLabel.Name = "ethnicityLabel";
            this.ethnicityLabel.Size = new System.Drawing.Size(65, 16);
            this.ethnicityLabel.TabIndex = 23;
            this.ethnicityLabel.Text = "Ethnicity:";
            // 
            // ethnicityValueLabel
            // 
            this.ethnicityValueLabel.AutoSize = true;
            this.ethnicityValueLabel.Location = new System.Drawing.Point(224, 220);
            this.ethnicityValueLabel.Name = "ethnicityValueLabel";
            this.ethnicityValueLabel.Size = new System.Drawing.Size(0, 17);
            this.ethnicityValueLabel.TabIndex = 24;
            // 
            // ssnLabel
            // 
            this.ssnLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssnLabel.Location = new System.Drawing.Point(3, 240);
            this.ssnLabel.Name = "ssnLabel";
            this.ssnLabel.Size = new System.Drawing.Size(47, 16);
            this.ssnLabel.TabIndex = 25;
            this.ssnLabel.Text = "SSN:";
            // 
            // ssnValueLabel
            // 
            this.ssnValueLabel.AutoSize = true;
            this.ssnValueLabel.Location = new System.Drawing.Point(224, 240);
            this.ssnValueLabel.Name = "ssnValueLabel";
            this.ssnValueLabel.Size = new System.Drawing.Size(0, 17);
            this.ssnValueLabel.TabIndex = 26;
            // 
            // patientMailingAddressLabel
            // 
            this.patientMailingAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientMailingAddressLabel.Location = new System.Drawing.Point(3, 260);
            this.patientMailingAddressLabel.Name = "patientMailingAddressLabel";
            this.patientMailingAddressLabel.Size = new System.Drawing.Size(207, 16);
            this.patientMailingAddressLabel.TabIndex = 29;
            this.patientMailingAddressLabel.Text = "Patient mailing address:";
            // 
            // patientMailingAddressValueLabel
            // 
            this.patientMailingAddressValueLabel.AutoSize = true;
            this.patientMailingAddressValueLabel.Location = new System.Drawing.Point(224, 260);
            this.patientMailingAddressValueLabel.Name = "patientMailingAddressValueLabel";
            this.patientMailingAddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.patientMailingAddressValueLabel.TabIndex = 30;
            // 
            // contactPhoneLabel
            // 
            this.contactPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contactPhoneLabel.Location = new System.Drawing.Point(3, 280);
            this.contactPhoneLabel.Name = "contactPhoneLabel";
            this.contactPhoneLabel.Size = new System.Drawing.Size(198, 16);
            this.contactPhoneLabel.TabIndex = 31;
            this.contactPhoneLabel.Text = "Contact (mailing address) phone:";
            // 
            // contactPhoneValueLabel
            // 
            this.contactPhoneValueLabel.AutoSize = true;
            this.contactPhoneValueLabel.Location = new System.Drawing.Point(224, 280);
            this.contactPhoneValueLabel.Name = "contactPhoneValueLabel";
            this.contactPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.contactPhoneValueLabel.TabIndex = 32;
            // 
            // contactCellLabel
            // 
            this.contactCellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contactCellLabel.Location = new System.Drawing.Point(3, 300);
            this.contactCellLabel.Name = "contactCellLabel";
            this.contactCellLabel.Size = new System.Drawing.Size(184, 16);
            this.contactCellLabel.TabIndex = 33;
            this.contactCellLabel.Text = "Contact (mailing address) cell:";
            // 
            // contactCellValueLabel
            // 
            this.contactCellValueLabel.AutoSize = true;
            this.contactCellValueLabel.Location = new System.Drawing.Point(224, 300);
            this.contactCellValueLabel.Name = "contactCellValueLabel";
            this.contactCellValueLabel.Size = new System.Drawing.Size(0, 17);
            this.contactCellValueLabel.TabIndex = 34;
            // 
            // driversLicenseLabel
            // 
            this.driversLicenseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.driversLicenseLabel.Location = new System.Drawing.Point(3, 320);
            this.driversLicenseLabel.Name = "driversLicenseLabel";
            this.driversLicenseLabel.Size = new System.Drawing.Size(143, 16);
            this.driversLicenseLabel.TabIndex = 41;
            this.driversLicenseLabel.Text = "U.S. driver\'s license:";
            // 
            // driversLicenseValueLabel
            // 
            this.driversLicenseValueLabel.AutoSize = true;
            this.driversLicenseValueLabel.Location = new System.Drawing.Point(224, 320);
            this.driversLicenseValueLabel.Name = "driversLicenseValueLabel";
            this.driversLicenseValueLabel.Size = new System.Drawing.Size(0, 17);
            this.driversLicenseValueLabel.TabIndex = 42;
            // 
            // passportLabel
            // 
            this.passportLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passportLabel.Location = new System.Drawing.Point(3, 340);
            this.passportLabel.Name = "passportLabel";
            this.passportLabel.Size = new System.Drawing.Size(75, 16);
            this.passportLabel.TabIndex = 43;
            this.passportLabel.Text = "Passport:";
            // 
            // passportValueLabel
            // 
            this.passportValueLabel.AutoSize = true;
            this.passportValueLabel.Location = new System.Drawing.Point(224, 340);
            this.passportValueLabel.Name = "passportValueLabel";
            this.passportValueLabel.Size = new System.Drawing.Size(0, 17);
            this.passportValueLabel.TabIndex = 44;
            // 
            // languageLabel
            // 
            this.languageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.languageLabel.Location = new System.Drawing.Point(3, 380);
            this.languageLabel.Name = "languageLabel";
            this.languageLabel.Size = new System.Drawing.Size(74, 16);
            this.languageLabel.TabIndex = 47;
            this.languageLabel.Text = "Language:";
            // 
            // languageValueLabel
            // 
            this.languageValueLabel.AutoSize = true;
            this.languageValueLabel.Location = new System.Drawing.Point(224, 380);
            this.languageValueLabel.Name = "languageValueLabel";
            this.languageValueLabel.Size = new System.Drawing.Size(0, 17);
            this.languageValueLabel.TabIndex = 48;
            // 
            // emergencyContact1NameLabel
            // 
            this.emergencyContact1NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact1NameLabel.Location = new System.Drawing.Point(3, 400);
            this.emergencyContact1NameLabel.Name = "emergencyContact1NameLabel";
            this.emergencyContact1NameLabel.Size = new System.Drawing.Size(168, 16);
            this.emergencyContact1NameLabel.TabIndex = 404;
            this.emergencyContact1NameLabel.Text = "Emergency Contact name:";
            // 
            // emergencyContact1NameValueLabel
            // 
            this.emergencyContact1NameValueLabel.AutoSize = true;
            this.emergencyContact1NameValueLabel.Location = new System.Drawing.Point(224, 400);
            this.emergencyContact1NameValueLabel.Name = "emergencyContact1NameValueLabel";
            this.emergencyContact1NameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.emergencyContact1NameValueLabel.TabIndex = 405;
            // 
            // emergencyContact1IsThePatientsLabel
            // 
            this.emergencyContact1IsThePatientsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact1IsThePatientsLabel.Location = new System.Drawing.Point(3, 420);
            this.emergencyContact1IsThePatientsLabel.Name = "emergencyContact1IsThePatientsLabel";
            this.emergencyContact1IsThePatientsLabel.Size = new System.Drawing.Size(215, 16);
            this.emergencyContact1IsThePatientsLabel.TabIndex = 406;
            this.emergencyContact1IsThePatientsLabel.Text = "The Contact is the Patient’s:";
            // 
            // emergencyContact1IsThePatientsValueLabel
            // 
            this.emergencyContact1IsThePatientsValueLabel.AutoSize = true;
            this.emergencyContact1IsThePatientsValueLabel.Location = new System.Drawing.Point(224, 420);
            this.emergencyContact1IsThePatientsValueLabel.Name = "emergencyContact1IsThePatientsValueLabel";
            this.emergencyContact1IsThePatientsValueLabel.Size = new System.Drawing.Size(0, 17);
            this.emergencyContact1IsThePatientsValueLabel.TabIndex = 407;
            // 
            // emergencyContact1PhoneLabel
            // 
            this.emergencyContact1PhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact1PhoneLabel.Location = new System.Drawing.Point(3, 440);
            this.emergencyContact1PhoneLabel.Name = "emergencyContact1PhoneLabel";
            this.emergencyContact1PhoneLabel.Size = new System.Drawing.Size(99, 16);
            this.emergencyContact1PhoneLabel.TabIndex = 410;
            this.emergencyContact1PhoneLabel.Text = "Contact phone:";
            // 
            // emergencyContact1PhoneValueLabel
            // 
            this.emergencyContact1PhoneValueLabel.AutoSize = true;
            this.emergencyContact1PhoneValueLabel.Location = new System.Drawing.Point(224, 440);
            this.emergencyContact1PhoneValueLabel.Name = "emergencyContact1PhoneValueLabel";
            this.emergencyContact1PhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.emergencyContact1PhoneValueLabel.TabIndex = 411;
            // 
            // generalInsuranceInformationHeader
            // 
            this.generalInsuranceInformationHeader.ColumnCount = 1;
            this.generalInsuranceInformationHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.generalInsuranceInformationHeader.Controls.Add(this.panel16, 0, 2);
            this.generalInsuranceInformationHeader.Controls.Add(this.generalInsuranceInformationLabel, 0, 1);
            this.generalInsuranceInformationHeader.Location = new System.Drawing.Point(3, 1218);
            this.generalInsuranceInformationHeader.Name = "generalInsuranceInformationHeader";
            this.generalInsuranceInformationHeader.RowCount = 3;
            this.generalInsuranceInformationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.generalInsuranceInformationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.generalInsuranceInformationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.generalInsuranceInformationHeader.Size = new System.Drawing.Size(597, 31);
            this.generalInsuranceInformationHeader.TabIndex = 581;
            // 
            // panel16
            // 
            this.panel16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel16.Location = new System.Drawing.Point(3, 23);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(591, 1);
            this.panel16.TabIndex = 441;
            // 
            // generalInsuranceInformationLabel
            // 
            this.generalInsuranceInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generalInsuranceInformationLabel.Location = new System.Drawing.Point(3, 0);
            this.generalInsuranceInformationLabel.Name = "generalInsuranceInformationLabel";
            this.generalInsuranceInformationLabel.Size = new System.Drawing.Size(252, 20);
            this.generalInsuranceInformationLabel.TabIndex = 133;
            this.generalInsuranceInformationLabel.Text = "General Insurance Information";
            // 
            // primaryInsuranceMSPHeader
            // 
            this.primaryInsuranceMSPHeader.ColumnCount = 1;
            this.primaryInsuranceMSPHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsuranceMSPHeader.Controls.Add(this.panel15, 0, 2);
            this.primaryInsuranceMSPHeader.Controls.Add(this.mspLabel, 0, 1);
            this.primaryInsuranceMSPHeader.Location = new System.Drawing.Point(3, 1334);
            this.primaryInsuranceMSPHeader.Name = "primaryInsuranceMSPHeader";
            this.primaryInsuranceMSPHeader.RowCount = 3;
            this.primaryInsuranceMSPHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.primaryInsuranceMSPHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceMSPHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsuranceMSPHeader.Size = new System.Drawing.Size(597, 28);
            this.primaryInsuranceMSPHeader.TabIndex = 579;
            // 
            // panel15
            // 
            this.panel15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel15.Location = new System.Drawing.Point(3, 23);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(591, 1);
            this.panel15.TabIndex = 441;
            // 
            // mspLabel
            // 
            this.mspLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mspLabel.Location = new System.Drawing.Point(3, 0);
            this.mspLabel.Name = "mspLabel";
            this.mspLabel.Size = new System.Drawing.Size(45, 20);
            this.mspLabel.TabIndex = 140;
            this.mspLabel.Text = "MSP";
            // 
            // primaryInsuranceMSPBody
            // 
            this.primaryInsuranceMSPBody.ColumnCount = 1;
            this.primaryInsuranceMSPBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsuranceMSPBody.Controls.Add(this.mspQuestionnaireSummaryConclusionLabel, 0, 1);
            this.primaryInsuranceMSPBody.Location = new System.Drawing.Point(3, 1376);
            this.primaryInsuranceMSPBody.Name = "primaryInsuranceMSPBody";
            this.primaryInsuranceMSPBody.RowCount = 2;
            this.primaryInsuranceMSPBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.primaryInsuranceMSPBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceMSPBody.Size = new System.Drawing.Size(597, 24);
            this.primaryInsuranceMSPBody.TabIndex = 580;
            // 
            // mspQuestionnaireSummaryConclusionLabel
            // 
            this.mspQuestionnaireSummaryConclusionLabel.AutoSize = true;
            this.mspQuestionnaireSummaryConclusionLabel.Location = new System.Drawing.Point(3, 0);
            this.mspQuestionnaireSummaryConclusionLabel.Name = "mspQuestionnaireSummaryConclusionLabel";
            this.mspQuestionnaireSummaryConclusionLabel.Size = new System.Drawing.Size(286, 17);
            this.mspQuestionnaireSummaryConclusionLabel.TabIndex = 141;
            this.mspQuestionnaireSummaryConclusionLabel.Text = "mspQuestionnaireSummaryConclusionLabel";
            // 
            // primaryInsurancePlanHeader
            // 
            this.primaryInsurancePlanHeader.ColumnCount = 1;
            this.primaryInsurancePlanHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsurancePlanHeader.Controls.Add(this.panel14, 0, 2);
            this.primaryInsurancePlanHeader.Controls.Add(this.primaryInsurancePlanLabel, 0, 1);
            this.primaryInsurancePlanHeader.Location = new System.Drawing.Point(3, 1414);
            this.primaryInsurancePlanHeader.Name = "primaryInsurancePlanHeader";
            this.primaryInsurancePlanHeader.RowCount = 3;
            this.primaryInsurancePlanHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.primaryInsurancePlanHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePlanHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsurancePlanHeader.Size = new System.Drawing.Size(597, 40);
            this.primaryInsurancePlanHeader.TabIndex = 577;
            // 
            // panel14
            // 
            this.panel14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel14.Location = new System.Drawing.Point(3, 23);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(591, 1);
            this.panel14.TabIndex = 441;
            // 
            // primaryInsurancePlanLabel
            // 
            this.primaryInsurancePlanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsurancePlanLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryInsurancePlanLabel.Name = "primaryInsurancePlanLabel";
            this.primaryInsurancePlanLabel.Size = new System.Drawing.Size(207, 20);
            this.primaryInsurancePlanLabel.TabIndex = 142;
            this.primaryInsurancePlanLabel.Text = "Primary Insurance - Plan";
            // 
            // primaryInsurancePlanBody
            // 
            this.primaryInsurancePlanBody.ColumnCount = 2;
            this.primaryInsurancePlanBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.58367F));
            this.primaryInsurancePlanBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 87.41633F));
            this.primaryInsurancePlanBody.Controls.Add(this.primaryPlanValueLabel, 1, 0);
            this.primaryInsurancePlanBody.Controls.Add(this.primaryPayorBrokerLabel, 0, 1);
            this.primaryInsurancePlanBody.Controls.Add(this.primaryPayorBrokerValueLabel, 1, 1);
            this.primaryInsurancePlanBody.Controls.Add(this.primaryCategoryLabel, 0, 2);
            this.primaryInsurancePlanBody.Controls.Add(this.primaryCategoryValueLabel, 1, 2);
            this.primaryInsurancePlanBody.Controls.Add(this.primaryPlanLabel, 0, 0);
            this.primaryInsurancePlanBody.Location = new System.Drawing.Point(3, 1461);
            this.primaryInsurancePlanBody.Name = "primaryInsurancePlanBody";
            this.primaryInsurancePlanBody.RowCount = 3;
            this.primaryInsurancePlanBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePlanBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePlanBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePlanBody.Size = new System.Drawing.Size(765, 59);
            this.primaryInsurancePlanBody.TabIndex = 578;
            // 
            // primaryPlanValueLabel
            // 
            this.primaryPlanValueLabel.AutoSize = true;
            this.primaryPlanValueLabel.Location = new System.Drawing.Point(99, 0);
            this.primaryPlanValueLabel.Name = "primaryPlanValueLabel";
            this.primaryPlanValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryPlanValueLabel.TabIndex = 144;
            // 
            // primaryPayorBrokerLabel
            // 
            this.primaryPayorBrokerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryPayorBrokerLabel.Location = new System.Drawing.Point(3, 20);
            this.primaryPayorBrokerLabel.Name = "primaryPayorBrokerLabel";
            this.primaryPayorBrokerLabel.Size = new System.Drawing.Size(90, 16);
            this.primaryPayorBrokerLabel.TabIndex = 145;
            this.primaryPayorBrokerLabel.Text = "Payor/Broker:";
            // 
            // primaryPayorBrokerValueLabel
            // 
            this.primaryPayorBrokerValueLabel.AutoSize = true;
            this.primaryPayorBrokerValueLabel.Location = new System.Drawing.Point(99, 20);
            this.primaryPayorBrokerValueLabel.Name = "primaryPayorBrokerValueLabel";
            this.primaryPayorBrokerValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryPayorBrokerValueLabel.TabIndex = 146;
            // 
            // primaryCategoryLabel
            // 
            this.primaryCategoryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryCategoryLabel.Location = new System.Drawing.Point(3, 40);
            this.primaryCategoryLabel.Name = "primaryCategoryLabel";
            this.primaryCategoryLabel.Size = new System.Drawing.Size(69, 16);
            this.primaryCategoryLabel.TabIndex = 147;
            this.primaryCategoryLabel.Text = "Category:";
            // 
            // primaryCategoryValueLabel
            // 
            this.primaryCategoryValueLabel.AutoSize = true;
            this.primaryCategoryValueLabel.Location = new System.Drawing.Point(99, 40);
            this.primaryCategoryValueLabel.Name = "primaryCategoryValueLabel";
            this.primaryCategoryValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryCategoryValueLabel.TabIndex = 148;
            // 
            // primaryPlanLabel
            // 
            this.primaryPlanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryPlanLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryPlanLabel.Name = "primaryPlanLabel";
            this.primaryPlanLabel.Size = new System.Drawing.Size(69, 16);
            this.primaryPlanLabel.TabIndex = 143;
            this.primaryPlanLabel.Text = "Plan:";
            // 
            // primaryInsurancePayorHeader
            // 
            this.primaryInsurancePayorHeader.ColumnCount = 1;
            this.primaryInsurancePayorHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsurancePayorHeader.Controls.Add(this.panel13, 0, 2);
            this.primaryInsurancePayorHeader.Controls.Add(this.primaryInsurancePayorDetailsLabel, 0, 1);
            this.primaryInsurancePayorHeader.Location = new System.Drawing.Point(3, 1534);
            this.primaryInsurancePayorHeader.Name = "primaryInsurancePayorHeader";
            this.primaryInsurancePayorHeader.RowCount = 3;
            this.primaryInsurancePayorHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.primaryInsurancePayorHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsurancePayorHeader.Size = new System.Drawing.Size(597, 28);
            this.primaryInsurancePayorHeader.TabIndex = 575;
            // 
            // panel13
            // 
            this.panel13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel13.Location = new System.Drawing.Point(3, 23);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(591, 1);
            this.panel13.TabIndex = 441;
            // 
            // primaryInsurancePayorDetailsLabel
            // 
            this.primaryInsurancePayorDetailsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsurancePayorDetailsLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryInsurancePayorDetailsLabel.Name = "primaryInsurancePayorDetailsLabel";
            this.primaryInsurancePayorDetailsLabel.Size = new System.Drawing.Size(278, 20);
            this.primaryInsurancePayorDetailsLabel.TabIndex = 149;
            this.primaryInsurancePayorDetailsLabel.Text = "Primary Insurance – Payor Details";
            // 
            // primaryInsurancePayorBody
            // 
            this.primaryInsurancePayorBody.ColumnCount = 2;
            this.primaryInsurancePayorBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.9819F));
            this.primaryInsurancePayorBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.0181F));
            this.primaryInsurancePayorBody.Controls.Add(this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel, 0, 0);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel, 1, 0);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryGroupNumberLabel, 0, 1);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryGroupNumberValueLabel, 1, 1);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryMedicalGroupOrIpaNameValueLabel, 1, 6);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryMedicalGroupOrIpaNameLabel, 0, 6);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryBillingCoNameValueLabel, 1, 8);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryBillingCoNameLabel, 0, 8);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryBillingNameValueLabel, 1, 9);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryBillingNameLabel, 0, 9);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryBillingPhoneValueLabel, 1, 10);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryBillingPhoneLabel, 0, 10);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryBillingAddressValueLabel, 1, 11);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryBillingAddressLabel, 0, 11);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryPrecertificationNumberLabel, 0, 2);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryPrecertificationNumberValueLabel, 1, 2);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryMedicaidIssueDateValueLabel, 1, 3);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryMedicaidIssueDateLabel, 0, 3);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryMedicalGroupOrIPAClinicValueLabel, 1, 7);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryMedicalGroupOrIPAClinicLabel, 0, 7);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryAdjusterValueLabel, 1, 5);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryAdjusterLabel, 0, 5);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryEmployeeSupervisorValueLabel, 1, 4);
            this.primaryInsurancePayorBody.Controls.Add(this.primaryEmployeeSupervisorLabel, 0, 4);
            this.primaryInsurancePayorBody.Location = new System.Drawing.Point(3, 1581);
            this.primaryInsurancePayorBody.Name = "primaryInsurancePayorBody";
            this.primaryInsurancePayorBody.RowCount = 12;
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsurancePayorBody.Size = new System.Drawing.Size(708, 242);
            this.primaryInsurancePayorBody.TabIndex = 576;
            // 
            // primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel
            // 
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.BackColor = System.Drawing.Color.White;
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Name = "primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel";
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size = new System.Drawing.Size(163, 16);
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.TabIndex = 150;
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text = "CERT/SSN/ID number:";
            // 
            // primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel
            // 
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location = new System.Drawing.Point(172, 0);
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Name = "primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel";
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Size = new System.Drawing.Size(200, 16);
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.TabIndex = 151;
            // 
            // primaryGroupNumberLabel
            // 
            this.primaryGroupNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryGroupNumberLabel.Location = new System.Drawing.Point(3, 20);
            this.primaryGroupNumberLabel.Name = "primaryGroupNumberLabel";
            this.primaryGroupNumberLabel.Size = new System.Drawing.Size(137, 16);
            this.primaryGroupNumberLabel.TabIndex = 152;
            this.primaryGroupNumberLabel.Text = "Group number:";
            // 
            // primaryGroupNumberValueLabel
            // 
            this.primaryGroupNumberValueLabel.AutoSize = true;
            this.primaryGroupNumberValueLabel.Location = new System.Drawing.Point(172, 20);
            this.primaryGroupNumberValueLabel.Name = "primaryGroupNumberValueLabel";
            this.primaryGroupNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryGroupNumberValueLabel.TabIndex = 153;
            // 
            // primaryMedicalGroupOrIpaNameValueLabel
            // 
            this.primaryMedicalGroupOrIpaNameValueLabel.AutoSize = true;
            this.primaryMedicalGroupOrIpaNameValueLabel.Location = new System.Drawing.Point(172, 120);
            this.primaryMedicalGroupOrIpaNameValueLabel.Name = "primaryMedicalGroupOrIpaNameValueLabel";
            this.primaryMedicalGroupOrIpaNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryMedicalGroupOrIpaNameValueLabel.TabIndex = 169;
            // 
            // primaryMedicalGroupOrIpaNameLabel
            // 
            this.primaryMedicalGroupOrIpaNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryMedicalGroupOrIpaNameLabel.Location = new System.Drawing.Point(3, 120);
            this.primaryMedicalGroupOrIpaNameLabel.Name = "primaryMedicalGroupOrIpaNameLabel";
            this.primaryMedicalGroupOrIpaNameLabel.Size = new System.Drawing.Size(163, 16);
            this.primaryMedicalGroupOrIpaNameLabel.TabIndex = 168;
            this.primaryMedicalGroupOrIpaNameLabel.Text = "Medical group/IPA name:";
            // 
            // primaryBillingCoNameValueLabel
            // 
            this.primaryBillingCoNameValueLabel.AutoSize = true;
            this.primaryBillingCoNameValueLabel.Location = new System.Drawing.Point(172, 160);
            this.primaryBillingCoNameValueLabel.Name = "primaryBillingCoNameValueLabel";
            this.primaryBillingCoNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryBillingCoNameValueLabel.TabIndex = 173;
            // 
            // primaryBillingCoNameLabel
            // 
            this.primaryBillingCoNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBillingCoNameLabel.Location = new System.Drawing.Point(3, 160);
            this.primaryBillingCoNameLabel.Name = "primaryBillingCoNameLabel";
            this.primaryBillingCoNameLabel.Size = new System.Drawing.Size(137, 16);
            this.primaryBillingCoNameLabel.TabIndex = 172;
            this.primaryBillingCoNameLabel.Text = "Billing c/o name:";
            // 
            // primaryBillingNameValueLabel
            // 
            this.primaryBillingNameValueLabel.AutoSize = true;
            this.primaryBillingNameValueLabel.Location = new System.Drawing.Point(172, 180);
            this.primaryBillingNameValueLabel.Name = "primaryBillingNameValueLabel";
            this.primaryBillingNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryBillingNameValueLabel.TabIndex = 175;
            // 
            // primaryBillingNameLabel
            // 
            this.primaryBillingNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBillingNameLabel.Location = new System.Drawing.Point(3, 180);
            this.primaryBillingNameLabel.Name = "primaryBillingNameLabel";
            this.primaryBillingNameLabel.Size = new System.Drawing.Size(137, 16);
            this.primaryBillingNameLabel.TabIndex = 174;
            this.primaryBillingNameLabel.Text = "Billing name:";
            // 
            // primaryBillingPhoneValueLabel
            // 
            this.primaryBillingPhoneValueLabel.AutoSize = true;
            this.primaryBillingPhoneValueLabel.Location = new System.Drawing.Point(172, 200);
            this.primaryBillingPhoneValueLabel.Name = "primaryBillingPhoneValueLabel";
            this.primaryBillingPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryBillingPhoneValueLabel.TabIndex = 177;
            // 
            // primaryBillingPhoneLabel
            // 
            this.primaryBillingPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBillingPhoneLabel.Location = new System.Drawing.Point(3, 200);
            this.primaryBillingPhoneLabel.Name = "primaryBillingPhoneLabel";
            this.primaryBillingPhoneLabel.Size = new System.Drawing.Size(137, 16);
            this.primaryBillingPhoneLabel.TabIndex = 176;
            this.primaryBillingPhoneLabel.Text = "Billing phone:";
            // 
            // primaryBillingAddressValueLabel
            // 
            this.primaryBillingAddressValueLabel.AutoSize = true;
            this.primaryBillingAddressValueLabel.Location = new System.Drawing.Point(172, 220);
            this.primaryBillingAddressValueLabel.Name = "primaryBillingAddressValueLabel";
            this.primaryBillingAddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryBillingAddressValueLabel.TabIndex = 179;
            // 
            // primaryBillingAddressLabel
            // 
            this.primaryBillingAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBillingAddressLabel.Location = new System.Drawing.Point(3, 220);
            this.primaryBillingAddressLabel.Name = "primaryBillingAddressLabel";
            this.primaryBillingAddressLabel.Size = new System.Drawing.Size(137, 16);
            this.primaryBillingAddressLabel.TabIndex = 178;
            this.primaryBillingAddressLabel.Text = "Billing address:";
            // 
            // primaryPrecertificationNumberLabel
            // 
            this.primaryPrecertificationNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryPrecertificationNumberLabel.Location = new System.Drawing.Point(3, 40);
            this.primaryPrecertificationNumberLabel.Name = "primaryPrecertificationNumberLabel";
            this.primaryPrecertificationNumberLabel.Size = new System.Drawing.Size(163, 16);
            this.primaryPrecertificationNumberLabel.TabIndex = 160;
            this.primaryPrecertificationNumberLabel.Text = "Precertification number:";
            // 
            // primaryPrecertificationNumberValueLabel
            // 
            this.primaryPrecertificationNumberValueLabel.AutoSize = true;
            this.primaryPrecertificationNumberValueLabel.Location = new System.Drawing.Point(172, 40);
            this.primaryPrecertificationNumberValueLabel.Name = "primaryPrecertificationNumberValueLabel";
            this.primaryPrecertificationNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryPrecertificationNumberValueLabel.TabIndex = 161;
            // 
            // primaryMedicaidIssueDateValueLabel
            // 
            this.primaryMedicaidIssueDateValueLabel.AutoSize = true;
            this.primaryMedicaidIssueDateValueLabel.Location = new System.Drawing.Point(172, 60);
            this.primaryMedicaidIssueDateValueLabel.Name = "primaryMedicaidIssueDateValueLabel";
            this.primaryMedicaidIssueDateValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryMedicaidIssueDateValueLabel.TabIndex = 163;
            // 
            // primaryMedicaidIssueDateLabel
            // 
            this.primaryMedicaidIssueDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryMedicaidIssueDateLabel.Location = new System.Drawing.Point(3, 60);
            this.primaryMedicaidIssueDateLabel.Name = "primaryMedicaidIssueDateLabel";
            this.primaryMedicaidIssueDateLabel.Size = new System.Drawing.Size(137, 16);
            this.primaryMedicaidIssueDateLabel.TabIndex = 162;
            this.primaryMedicaidIssueDateLabel.Text = "Medicaid issue date:";
            // 
            // primaryMedicalGroupOrIPAClinicValueLabel
            // 
            this.primaryMedicalGroupOrIPAClinicValueLabel.AutoSize = true;
            this.primaryMedicalGroupOrIPAClinicValueLabel.Location = new System.Drawing.Point(172, 140);
            this.primaryMedicalGroupOrIPAClinicValueLabel.Name = "primaryMedicalGroupOrIPAClinicValueLabel";
            this.primaryMedicalGroupOrIPAClinicValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryMedicalGroupOrIPAClinicValueLabel.TabIndex = 171;
            // 
            // primaryMedicalGroupOrIPAClinicLabel
            // 
            this.primaryMedicalGroupOrIPAClinicLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryMedicalGroupOrIPAClinicLabel.Location = new System.Drawing.Point(3, 140);
            this.primaryMedicalGroupOrIPAClinicLabel.Name = "primaryMedicalGroupOrIPAClinicLabel";
            this.primaryMedicalGroupOrIPAClinicLabel.Size = new System.Drawing.Size(163, 16);
            this.primaryMedicalGroupOrIPAClinicLabel.TabIndex = 170;
            this.primaryMedicalGroupOrIPAClinicLabel.Text = "Medical group/IPA clinic:";
            // 
            // primaryAdjusterValueLabel
            // 
            this.primaryAdjusterValueLabel.AutoSize = true;
            this.primaryAdjusterValueLabel.Location = new System.Drawing.Point(172, 100);
            this.primaryAdjusterValueLabel.Name = "primaryAdjusterValueLabel";
            this.primaryAdjusterValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAdjusterValueLabel.TabIndex = 167;
            // 
            // primaryAdjusterLabel
            // 
            this.primaryAdjusterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAdjusterLabel.Location = new System.Drawing.Point(3, 100);
            this.primaryAdjusterLabel.Name = "primaryAdjusterLabel";
            this.primaryAdjusterLabel.Size = new System.Drawing.Size(137, 16);
            this.primaryAdjusterLabel.TabIndex = 166;
            this.primaryAdjusterLabel.Text = "Adjuster:";
            // 
            // primaryEmployeeSupervisorValueLabel
            // 
            this.primaryEmployeeSupervisorValueLabel.AutoSize = true;
            this.primaryEmployeeSupervisorValueLabel.Location = new System.Drawing.Point(172, 80);
            this.primaryEmployeeSupervisorValueLabel.Name = "primaryEmployeeSupervisorValueLabel";
            this.primaryEmployeeSupervisorValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryEmployeeSupervisorValueLabel.TabIndex = 165;
            // 
            // primaryEmployeeSupervisorLabel
            // 
            this.primaryEmployeeSupervisorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEmployeeSupervisorLabel.Location = new System.Drawing.Point(3, 80);
            this.primaryEmployeeSupervisorLabel.Name = "primaryEmployeeSupervisorLabel";
            this.primaryEmployeeSupervisorLabel.Size = new System.Drawing.Size(137, 16);
            this.primaryEmployeeSupervisorLabel.TabIndex = 164;
            this.primaryEmployeeSupervisorLabel.Text = "Employee’s supervisor:";
            // 
            // primaryInsuranceInsuredHeader
            // 
            this.primaryInsuranceInsuredHeader.ColumnCount = 1;
            this.primaryInsuranceInsuredHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsuranceInsuredHeader.Controls.Add(this.panel12, 0, 2);
            this.primaryInsuranceInsuredHeader.Controls.Add(this.primaryInsuranceInsuredLabel, 0, 1);
            this.primaryInsuranceInsuredHeader.Location = new System.Drawing.Point(3, 1859);
            this.primaryInsuranceInsuredHeader.Name = "primaryInsuranceInsuredHeader";
            this.primaryInsuranceInsuredHeader.RowCount = 3;
            this.primaryInsuranceInsuredHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.primaryInsuranceInsuredHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceInsuredHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsuranceInsuredHeader.Size = new System.Drawing.Size(597, 31);
            this.primaryInsuranceInsuredHeader.TabIndex = 573;
            // 
            // panel12
            // 
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Location = new System.Drawing.Point(3, 23);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(591, 1);
            this.panel12.TabIndex = 441;
            // 
            // primaryInsuranceInsuredLabel
            // 
            this.primaryInsuranceInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsuranceInsuredLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryInsuranceInsuredLabel.Name = "primaryInsuranceInsuredLabel";
            this.primaryInsuranceInsuredLabel.Size = new System.Drawing.Size(230, 20);
            this.primaryInsuranceInsuredLabel.TabIndex = 180;
            this.primaryInsuranceInsuredLabel.Text = "Primary Insurance – Insured";
            // 
            // primaruInsuranceInsuredBody
            // 
            this.primaruInsuranceInsuredBody.ColumnCount = 2;
            this.primaruInsuranceInsuredBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.20513F));
            this.primaruInsuranceInsuredBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 350.7949F));
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryNameLabel, 0, 0);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryNameValueLabel, 1, 0);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryThePatientIsTheInsuredsLabel, 0, 1);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryThePatientIsTheInsuredsValueLabel, 1, 1);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryGenderLabel, 0, 2);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryGenderValueLabel, 1, 2);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryDobLabel, 0, 3);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryDobValueLabel, 1, 3);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryNationalIdValueLabel, 1, 4);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryNationalIdLabel, 0, 4);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryAddressValueLabel, 1, 5);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryAddressLabel, 0, 5);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryContactPhoneValueLabel, 1, 6);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryContactPhoneLabel, 0, 6);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryContactCellValueLabel, 1, 7);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryContactCellLabel, 0, 7);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryEmploymentStatusValueLabel, 1, 8);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryEmploymentStatusLabel, 0, 8);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryEmployerValueLabel, 1, 9);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryEmployerLabel, 0, 9);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryEmployerPhoneValueLabel, 1, 10);
            this.primaruInsuranceInsuredBody.Controls.Add(this.primaryEmployerPhoneLabel, 0, 10);
            this.primaruInsuranceInsuredBody.Location = new System.Drawing.Point(3, 1896);
            this.primaruInsuranceInsuredBody.Name = "primaruInsuranceInsuredBody";
            this.primaruInsuranceInsuredBody.RowCount = 11;
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaruInsuranceInsuredBody.Size = new System.Drawing.Size(894, 222);
            this.primaruInsuranceInsuredBody.TabIndex = 574;
            // 
            // primaryNameLabel
            // 
            this.primaryNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryNameLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryNameLabel.Name = "primaryNameLabel";
            this.primaryNameLabel.Size = new System.Drawing.Size(65, 16);
            this.primaryNameLabel.TabIndex = 181;
            this.primaryNameLabel.Text = "Name:";
            // 
            // primaryNameValueLabel
            // 
            this.primaryNameValueLabel.AutoSize = true;
            this.primaryNameValueLabel.Location = new System.Drawing.Point(169, 0);
            this.primaryNameValueLabel.Name = "primaryNameValueLabel";
            this.primaryNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryNameValueLabel.TabIndex = 182;
            // 
            // primaryThePatientIsTheInsuredsLabel
            // 
            this.primaryThePatientIsTheInsuredsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryThePatientIsTheInsuredsLabel.Location = new System.Drawing.Point(3, 20);
            this.primaryThePatientIsTheInsuredsLabel.Name = "primaryThePatientIsTheInsuredsLabel";
            this.primaryThePatientIsTheInsuredsLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryThePatientIsTheInsuredsLabel.TabIndex = 183;
            this.primaryThePatientIsTheInsuredsLabel.Text = "The Patient is the Insured’s:";
            // 
            // primaryThePatientIsTheInsuredsValueLabel
            // 
            this.primaryThePatientIsTheInsuredsValueLabel.AutoSize = true;
            this.primaryThePatientIsTheInsuredsValueLabel.Location = new System.Drawing.Point(169, 20);
            this.primaryThePatientIsTheInsuredsValueLabel.Name = "primaryThePatientIsTheInsuredsValueLabel";
            this.primaryThePatientIsTheInsuredsValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryThePatientIsTheInsuredsValueLabel.TabIndex = 184;
            // 
            // primaryGenderLabel
            // 
            this.primaryGenderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryGenderLabel.Location = new System.Drawing.Point(3, 40);
            this.primaryGenderLabel.Name = "primaryGenderLabel";
            this.primaryGenderLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryGenderLabel.TabIndex = 185;
            this.primaryGenderLabel.Text = "Gender:";
            // 
            // primaryGenderValueLabel
            // 
            this.primaryGenderValueLabel.AutoSize = true;
            this.primaryGenderValueLabel.Location = new System.Drawing.Point(169, 40);
            this.primaryGenderValueLabel.Name = "primaryGenderValueLabel";
            this.primaryGenderValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryGenderValueLabel.TabIndex = 186;
            // 
            // primaryDobLabel
            // 
            this.primaryDobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryDobLabel.Location = new System.Drawing.Point(3, 60);
            this.primaryDobLabel.Name = "primaryDobLabel";
            this.primaryDobLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryDobLabel.TabIndex = 187;
            this.primaryDobLabel.Text = "DOB:";
            // 
            // primaryDobValueLabel
            // 
            this.primaryDobValueLabel.AutoSize = true;
            this.primaryDobValueLabel.Location = new System.Drawing.Point(169, 60);
            this.primaryDobValueLabel.Name = "primaryDobValueLabel";
            this.primaryDobValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryDobValueLabel.TabIndex = 188;
            // 
            // primaryNationalIdValueLabel
            // 
            this.primaryNationalIdValueLabel.AutoSize = true;
            this.primaryNationalIdValueLabel.Location = new System.Drawing.Point(169, 80);
            this.primaryNationalIdValueLabel.Name = "primaryNationalIdValueLabel";
            this.primaryNationalIdValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryNationalIdValueLabel.TabIndex = 190;
            // 
            // primaryNationalIdLabel
            // 
            this.primaryNationalIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryNationalIdLabel.Location = new System.Drawing.Point(3, 80);
            this.primaryNationalIdLabel.Name = "primaryNationalIdLabel";
            this.primaryNationalIdLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryNationalIdLabel.TabIndex = 189;
            this.primaryNationalIdLabel.Text = "National ID:";
            // 
            // primaryAddressValueLabel
            // 
            this.primaryAddressValueLabel.AutoSize = true;
            this.primaryAddressValueLabel.Location = new System.Drawing.Point(169, 100);
            this.primaryAddressValueLabel.Name = "primaryAddressValueLabel";
            this.primaryAddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAddressValueLabel.TabIndex = 192;
            // 
            // primaryAddressLabel
            // 
            this.primaryAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAddressLabel.Location = new System.Drawing.Point(3, 100);
            this.primaryAddressLabel.Name = "primaryAddressLabel";
            this.primaryAddressLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryAddressLabel.TabIndex = 191;
            this.primaryAddressLabel.Text = "Address:";
            // 
            // primaryContactPhoneValueLabel
            // 
            this.primaryContactPhoneValueLabel.AutoSize = true;
            this.primaryContactPhoneValueLabel.Location = new System.Drawing.Point(169, 120);
            this.primaryContactPhoneValueLabel.Name = "primaryContactPhoneValueLabel";
            this.primaryContactPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryContactPhoneValueLabel.TabIndex = 194;
            // 
            // primaryContactPhoneLabel
            // 
            this.primaryContactPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryContactPhoneLabel.Location = new System.Drawing.Point(3, 120);
            this.primaryContactPhoneLabel.Name = "primaryContactPhoneLabel";
            this.primaryContactPhoneLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryContactPhoneLabel.TabIndex = 193;
            this.primaryContactPhoneLabel.Text = "Contact phone:";
            // 
            // primaryContactCellValueLabel
            // 
            this.primaryContactCellValueLabel.AutoSize = true;
            this.primaryContactCellValueLabel.Location = new System.Drawing.Point(169, 140);
            this.primaryContactCellValueLabel.Name = "primaryContactCellValueLabel";
            this.primaryContactCellValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryContactCellValueLabel.TabIndex = 196;
            // 
            // primaryContactCellLabel
            // 
            this.primaryContactCellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryContactCellLabel.Location = new System.Drawing.Point(3, 140);
            this.primaryContactCellLabel.Name = "primaryContactCellLabel";
            this.primaryContactCellLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryContactCellLabel.TabIndex = 195;
            this.primaryContactCellLabel.Text = "Contact cell:";
            // 
            // primaryEmploymentStatusValueLabel
            // 
            this.primaryEmploymentStatusValueLabel.AutoSize = true;
            this.primaryEmploymentStatusValueLabel.Location = new System.Drawing.Point(169, 160);
            this.primaryEmploymentStatusValueLabel.Name = "primaryEmploymentStatusValueLabel";
            this.primaryEmploymentStatusValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryEmploymentStatusValueLabel.TabIndex = 198;
            // 
            // primaryEmploymentStatusLabel
            // 
            this.primaryEmploymentStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEmploymentStatusLabel.Location = new System.Drawing.Point(3, 160);
            this.primaryEmploymentStatusLabel.Name = "primaryEmploymentStatusLabel";
            this.primaryEmploymentStatusLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryEmploymentStatusLabel.TabIndex = 197;
            this.primaryEmploymentStatusLabel.Text = "Employment status:";
            // 
            // primaryEmployerValueLabel
            // 
            this.primaryEmployerValueLabel.AutoSize = true;
            this.primaryEmployerValueLabel.Location = new System.Drawing.Point(169, 180);
            this.primaryEmployerValueLabel.Name = "primaryEmployerValueLabel";
            this.primaryEmployerValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryEmployerValueLabel.TabIndex = 200;
            // 
            // primaryEmployerLabel
            // 
            this.primaryEmployerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEmployerLabel.Location = new System.Drawing.Point(3, 180);
            this.primaryEmployerLabel.Name = "primaryEmployerLabel";
            this.primaryEmployerLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryEmployerLabel.TabIndex = 199;
            this.primaryEmployerLabel.Text = "Employer:";
            // 
            // primaryEmployerPhoneValueLabel
            // 
            this.primaryEmployerPhoneValueLabel.AutoSize = true;
            this.primaryEmployerPhoneValueLabel.Location = new System.Drawing.Point(169, 200);
            this.primaryEmployerPhoneValueLabel.Name = "primaryEmployerPhoneValueLabel";
            this.primaryEmployerPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryEmployerPhoneValueLabel.TabIndex = 202;
            // 
            // primaryEmployerPhoneLabel
            // 
            this.primaryEmployerPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEmployerPhoneLabel.Location = new System.Drawing.Point(3, 200);
            this.primaryEmployerPhoneLabel.Name = "primaryEmployerPhoneLabel";
            this.primaryEmployerPhoneLabel.Size = new System.Drawing.Size(160, 16);
            this.primaryEmployerPhoneLabel.TabIndex = 201;
            this.primaryEmployerPhoneLabel.Text = "Employer phone:";
            // 
            // primaryInsuranceVerificationHeader
            // 
            this.primaryInsuranceVerificationHeader.ColumnCount = 1;
            this.primaryInsuranceVerificationHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsuranceVerificationHeader.Controls.Add(this.panel11, 0, 2);
            this.primaryInsuranceVerificationHeader.Controls.Add(this.primaryInsuranceVerificationLabel, 0, 1);
            this.primaryInsuranceVerificationHeader.Location = new System.Drawing.Point(3, 2136);
            this.primaryInsuranceVerificationHeader.Name = "primaryInsuranceVerificationHeader";
            this.primaryInsuranceVerificationHeader.RowCount = 3;
            this.primaryInsuranceVerificationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.primaryInsuranceVerificationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsuranceVerificationHeader.Size = new System.Drawing.Size(597, 32);
            this.primaryInsuranceVerificationHeader.TabIndex = 571;
            // 
            // panel11
            // 
            this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel11.Location = new System.Drawing.Point(3, 23);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(591, 1);
            this.panel11.TabIndex = 441;
            // 
            // primaryInsuranceVerificationLabel
            // 
            this.primaryInsuranceVerificationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsuranceVerificationLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryInsuranceVerificationLabel.Name = "primaryInsuranceVerificationLabel";
            this.primaryInsuranceVerificationLabel.Size = new System.Drawing.Size(290, 20);
            this.primaryInsuranceVerificationLabel.TabIndex = 203;
            this.primaryInsuranceVerificationLabel.Text = "Primary Insurance – Verification";
            // 
            // primaryInsuranceVerificationBody
            // 
            this.primaryInsuranceVerificationBody.ColumnCount = 2;
            this.primaryInsuranceVerificationBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.27149F));
            this.primaryInsuranceVerificationBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.72851F));
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryBenefitsVerifiedLabel, 0, 0);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryBenefitsVerifiedValueLabel, 1, 0);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryInitiatedByValueLabel, 1, 1);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryInitiatedByLabel, 0, 1);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryDateValueLabel, 1, 2);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryDateLabel, 0, 2);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAuthorizationRequiredLabel, 0, 3);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAuthorizationRequiredValueLabel, 1, 3);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAuthorizationCompanyLabel, 0, 4);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAuthorizationCompanyValueLabel, 1, 4);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAuthorizationPhoneLabel, 0, 5);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAuthorizationPhoneValueLabel, 1, 5);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryPromptExtValueLabel, 1, 6);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryPromptExtLabel, 0, 6);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAttorneyNameLabel, 0, 7);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAttorneyNameValueLabel, 1, 7);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAttorneyAddressLabel, 0, 8);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAttorneyAddressValueLabel, 1, 8);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAttorneyPhoneNumberValueLabel, 1, 9);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAttorneyPhoneNumberLabel, 0, 9);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAutoHomeInsuranceAgentNameLabel, 0, 10);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAutoHomeInsuranceAgentNameValueLabel, 1, 10);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAutoHomeInsuranceAgentAddressValueLabel, 1, 11);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAutoHomeInsuranceAgentAddressLabel, 0, 11);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel, 1, 12);
            this.primaryInsuranceVerificationBody.Controls.Add(this.primaryAutoHomeInsuranceAgentPhoneNumberLabel, 0, 12);
            this.primaryInsuranceVerificationBody.Location = new System.Drawing.Point(3, 2174);
            this.primaryInsuranceVerificationBody.Name = "primaryInsuranceVerificationBody";
            this.primaryInsuranceVerificationBody.RowCount = 13;
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceVerificationBody.Size = new System.Drawing.Size(660, 270);
            this.primaryInsuranceVerificationBody.TabIndex = 572;
            // 
            // primaryBenefitsVerifiedLabel
            // 
            this.primaryBenefitsVerifiedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBenefitsVerifiedLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryBenefitsVerifiedLabel.Name = "primaryBenefitsVerifiedLabel";
            this.primaryBenefitsVerifiedLabel.Size = new System.Drawing.Size(124, 16);
            this.primaryBenefitsVerifiedLabel.TabIndex = 204;
            this.primaryBenefitsVerifiedLabel.Text = "Benefits verified:";
            // 
            // primaryBenefitsVerifiedValueLabel
            // 
            this.primaryBenefitsVerifiedValueLabel.AutoSize = true;
            this.primaryBenefitsVerifiedValueLabel.Location = new System.Drawing.Point(268, 0);
            this.primaryBenefitsVerifiedValueLabel.Name = "primaryBenefitsVerifiedValueLabel";
            this.primaryBenefitsVerifiedValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryBenefitsVerifiedValueLabel.TabIndex = 205;
            // 
            // primaryInitiatedByValueLabel
            // 
            this.primaryInitiatedByValueLabel.AutoSize = true;
            this.primaryInitiatedByValueLabel.Location = new System.Drawing.Point(268, 20);
            this.primaryInitiatedByValueLabel.Name = "primaryInitiatedByValueLabel";
            this.primaryInitiatedByValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryInitiatedByValueLabel.TabIndex = 207;
            // 
            // primaryInitiatedByLabel
            // 
            this.primaryInitiatedByLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInitiatedByLabel.Location = new System.Drawing.Point(3, 20);
            this.primaryInitiatedByLabel.Name = "primaryInitiatedByLabel";
            this.primaryInitiatedByLabel.Size = new System.Drawing.Size(93, 16);
            this.primaryInitiatedByLabel.TabIndex = 206;
            this.primaryInitiatedByLabel.Text = "Initiated by:";
            // 
            // primaryDateValueLabel
            // 
            this.primaryDateValueLabel.AutoSize = true;
            this.primaryDateValueLabel.Location = new System.Drawing.Point(268, 40);
            this.primaryDateValueLabel.Name = "primaryDateValueLabel";
            this.primaryDateValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryDateValueLabel.TabIndex = 209;
            // 
            // primaryDateLabel
            // 
            this.primaryDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryDateLabel.Location = new System.Drawing.Point(3, 40);
            this.primaryDateLabel.Name = "primaryDateLabel";
            this.primaryDateLabel.Size = new System.Drawing.Size(65, 16);
            this.primaryDateLabel.TabIndex = 208;
            this.primaryDateLabel.Text = "Date:";
            // 
            // primaryAuthorizationRequiredLabel
            // 
            this.primaryAuthorizationRequiredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationRequiredLabel.Location = new System.Drawing.Point(3, 60);
            this.primaryAuthorizationRequiredLabel.Name = "primaryAuthorizationRequiredLabel";
            this.primaryAuthorizationRequiredLabel.Size = new System.Drawing.Size(145, 16);
            this.primaryAuthorizationRequiredLabel.TabIndex = 210;
            this.primaryAuthorizationRequiredLabel.Text = "Authorization required:";
            // 
            // primaryAuthorizationRequiredValueLabel
            // 
            this.primaryAuthorizationRequiredValueLabel.AutoSize = true;
            this.primaryAuthorizationRequiredValueLabel.Location = new System.Drawing.Point(268, 60);
            this.primaryAuthorizationRequiredValueLabel.Name = "primaryAuthorizationRequiredValueLabel";
            this.primaryAuthorizationRequiredValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAuthorizationRequiredValueLabel.TabIndex = 211;
            // 
            // primaryAuthorizationCompanyLabel
            // 
            this.primaryAuthorizationCompanyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationCompanyLabel.Location = new System.Drawing.Point(3, 80);
            this.primaryAuthorizationCompanyLabel.Name = "primaryAuthorizationCompanyLabel";
            this.primaryAuthorizationCompanyLabel.Size = new System.Drawing.Size(164, 16);
            this.primaryAuthorizationCompanyLabel.TabIndex = 212;
            this.primaryAuthorizationCompanyLabel.Text = "Authorization company:";
            // 
            // primaryAuthorizationCompanyValueLabel
            // 
            this.primaryAuthorizationCompanyValueLabel.AutoSize = true;
            this.primaryAuthorizationCompanyValueLabel.Location = new System.Drawing.Point(268, 80);
            this.primaryAuthorizationCompanyValueLabel.Name = "primaryAuthorizationCompanyValueLabel";
            this.primaryAuthorizationCompanyValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAuthorizationCompanyValueLabel.TabIndex = 213;
            // 
            // primaryAuthorizationPhoneLabel
            // 
            this.primaryAuthorizationPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationPhoneLabel.Location = new System.Drawing.Point(3, 100);
            this.primaryAuthorizationPhoneLabel.Name = "primaryAuthorizationPhoneLabel";
            this.primaryAuthorizationPhoneLabel.Size = new System.Drawing.Size(156, 16);
            this.primaryAuthorizationPhoneLabel.TabIndex = 214;
            this.primaryAuthorizationPhoneLabel.Text = "Authorization phone:";
            // 
            // primaryAuthorizationPhoneValueLabel
            // 
            this.primaryAuthorizationPhoneValueLabel.AutoSize = true;
            this.primaryAuthorizationPhoneValueLabel.Location = new System.Drawing.Point(268, 100);
            this.primaryAuthorizationPhoneValueLabel.Name = "primaryAuthorizationPhoneValueLabel";
            this.primaryAuthorizationPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAuthorizationPhoneValueLabel.TabIndex = 215;
            // 
            // primaryPromptExtValueLabel
            // 
            this.primaryPromptExtValueLabel.AutoSize = true;
            this.primaryPromptExtValueLabel.Location = new System.Drawing.Point(268, 120);
            this.primaryPromptExtValueLabel.Name = "primaryPromptExtValueLabel";
            this.primaryPromptExtValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryPromptExtValueLabel.TabIndex = 217;
            // 
            // primaryPromptExtLabel
            // 
            this.primaryPromptExtLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryPromptExtLabel.Location = new System.Drawing.Point(3, 120);
            this.primaryPromptExtLabel.Name = "primaryPromptExtLabel";
            this.primaryPromptExtLabel.Size = new System.Drawing.Size(119, 16);
            this.primaryPromptExtLabel.TabIndex = 216;
            this.primaryPromptExtLabel.Text = "Prompt/ext:";
            // 
            // primaryAttorneyNameLabel
            // 
            this.primaryAttorneyNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAttorneyNameLabel.Location = new System.Drawing.Point(3, 140);
            this.primaryAttorneyNameLabel.Name = "primaryAttorneyNameLabel";
            this.primaryAttorneyNameLabel.Size = new System.Drawing.Size(124, 16);
            this.primaryAttorneyNameLabel.TabIndex = 218;
            this.primaryAttorneyNameLabel.Text = "Attorney name:";
            // 
            // primaryAttorneyNameValueLabel
            // 
            this.primaryAttorneyNameValueLabel.AutoSize = true;
            this.primaryAttorneyNameValueLabel.Location = new System.Drawing.Point(268, 140);
            this.primaryAttorneyNameValueLabel.Name = "primaryAttorneyNameValueLabel";
            this.primaryAttorneyNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAttorneyNameValueLabel.TabIndex = 219;
            // 
            // primaryAttorneyAddressLabel
            // 
            this.primaryAttorneyAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAttorneyAddressLabel.Location = new System.Drawing.Point(3, 160);
            this.primaryAttorneyAddressLabel.Name = "primaryAttorneyAddressLabel";
            this.primaryAttorneyAddressLabel.Size = new System.Drawing.Size(124, 16);
            this.primaryAttorneyAddressLabel.TabIndex = 220;
            this.primaryAttorneyAddressLabel.Text = "Attorney address:";
            // 
            // primaryAttorneyAddressValueLabel
            // 
            this.primaryAttorneyAddressValueLabel.AutoSize = true;
            this.primaryAttorneyAddressValueLabel.Location = new System.Drawing.Point(268, 160);
            this.primaryAttorneyAddressValueLabel.Name = "primaryAttorneyAddressValueLabel";
            this.primaryAttorneyAddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAttorneyAddressValueLabel.TabIndex = 221;
            // 
            // primaryAttorneyPhoneNumberValueLabel
            // 
            this.primaryAttorneyPhoneNumberValueLabel.AutoSize = true;
            this.primaryAttorneyPhoneNumberValueLabel.Location = new System.Drawing.Point(268, 180);
            this.primaryAttorneyPhoneNumberValueLabel.Name = "primaryAttorneyPhoneNumberValueLabel";
            this.primaryAttorneyPhoneNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAttorneyPhoneNumberValueLabel.TabIndex = 223;
            // 
            // primaryAttorneyPhoneNumberLabel
            // 
            this.primaryAttorneyPhoneNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAttorneyPhoneNumberLabel.Location = new System.Drawing.Point(3, 180);
            this.primaryAttorneyPhoneNumberLabel.Name = "primaryAttorneyPhoneNumberLabel";
            this.primaryAttorneyPhoneNumberLabel.Size = new System.Drawing.Size(163, 16);
            this.primaryAttorneyPhoneNumberLabel.TabIndex = 222;
            this.primaryAttorneyPhoneNumberLabel.Text = "Attorney phone number:";
            // 
            // primaryAutoHomeInsuranceAgentNameLabel
            // 
            this.primaryAutoHomeInsuranceAgentNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAutoHomeInsuranceAgentNameLabel.Location = new System.Drawing.Point(3, 200);
            this.primaryAutoHomeInsuranceAgentNameLabel.Name = "primaryAutoHomeInsuranceAgentNameLabel";
            this.primaryAutoHomeInsuranceAgentNameLabel.Size = new System.Drawing.Size(210, 16);
            this.primaryAutoHomeInsuranceAgentNameLabel.TabIndex = 224;
            this.primaryAutoHomeInsuranceAgentNameLabel.Text = "Auto/Home Insurance Agent name:";
            // 
            // primaryAutoHomeInsuranceAgentNameValueLabel
            // 
            this.primaryAutoHomeInsuranceAgentNameValueLabel.AutoSize = true;
            this.primaryAutoHomeInsuranceAgentNameValueLabel.Location = new System.Drawing.Point(268, 200);
            this.primaryAutoHomeInsuranceAgentNameValueLabel.Name = "primaryAutoHomeInsuranceAgentNameValueLabel";
            this.primaryAutoHomeInsuranceAgentNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAutoHomeInsuranceAgentNameValueLabel.TabIndex = 225;
            // 
            // primaryAutoHomeInsuranceAgentAddressValueLabel
            // 
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.AutoSize = true;
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.Location = new System.Drawing.Point(268, 220);
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.Name = "primaryAutoHomeInsuranceAgentAddressValueLabel";
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.TabIndex = 227;
            // 
            // primaryAutoHomeInsuranceAgentAddressLabel
            // 
            this.primaryAutoHomeInsuranceAgentAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAutoHomeInsuranceAgentAddressLabel.Location = new System.Drawing.Point(3, 220);
            this.primaryAutoHomeInsuranceAgentAddressLabel.Name = "primaryAutoHomeInsuranceAgentAddressLabel";
            this.primaryAutoHomeInsuranceAgentAddressLabel.Size = new System.Drawing.Size(222, 16);
            this.primaryAutoHomeInsuranceAgentAddressLabel.TabIndex = 226;
            this.primaryAutoHomeInsuranceAgentAddressLabel.Text = "Auto/Home Insurance Agent address:";
            // 
            // primaryAutoHomeInsuranceAgentPhoneNumberValueLabel
            // 
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.AutoSize = true;
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Location = new System.Drawing.Point(268, 240);
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Name = "primaryAutoHomeInsuranceAgentPhoneNumberValueLabel";
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.TabIndex = 229;
            // 
            // primaryAutoHomeInsuranceAgentPhoneNumberLabel
            // 
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Location = new System.Drawing.Point(3, 240);
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Name = "primaryAutoHomeInsuranceAgentPhoneNumberLabel";
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Size = new System.Drawing.Size(259, 16);
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.TabIndex = 228;
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Text = "Auto/Home Insurance Agent phone number:";
            // 
            // primaryInsuranceAuthorizatioHeader
            // 
            this.primaryInsuranceAuthorizatioHeader.ColumnCount = 1;
            this.primaryInsuranceAuthorizatioHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsuranceAuthorizatioHeader.Controls.Add(this.panel10, 0, 2);
            this.primaryInsuranceAuthorizatioHeader.Controls.Add(this.primaryInsuranceAuthorizationLabel, 0, 1);
            this.primaryInsuranceAuthorizatioHeader.Location = new System.Drawing.Point(3, 2466);
            this.primaryInsuranceAuthorizatioHeader.Name = "primaryInsuranceAuthorizatioHeader";
            this.primaryInsuranceAuthorizatioHeader.RowCount = 3;
            this.primaryInsuranceAuthorizatioHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.primaryInsuranceAuthorizatioHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizatioHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.primaryInsuranceAuthorizatioHeader.Size = new System.Drawing.Size(597, 29);
            this.primaryInsuranceAuthorizatioHeader.TabIndex = 569;
            // 
            // panel10
            // 
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Location = new System.Drawing.Point(3, 23);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(591, 1);
            this.panel10.TabIndex = 441;
            // 
            // primaryInsuranceAuthorizationLabel
            // 
            this.primaryInsuranceAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsuranceAuthorizationLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryInsuranceAuthorizationLabel.Name = "primaryInsuranceAuthorizationLabel";
            this.primaryInsuranceAuthorizationLabel.Size = new System.Drawing.Size(339, 20);
            this.primaryInsuranceAuthorizationLabel.TabIndex = 476;
            this.primaryInsuranceAuthorizationLabel.Text = "Primary Insurance - Authorization";
            // 
            // primaryInsuranceAuthorizationBody
            // 
            this.primaryInsuranceAuthorizationBody.ColumnCount = 2;
            this.primaryInsuranceAuthorizationBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.3273F));
            this.primaryInsuranceAuthorizationBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.6727F));
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryTrackingNumberLabel, 0, 0);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryAuthorizationTrackingNumberValueLabel, 1, 0);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryAuthorizationNumberLabel, 0, 1);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryAuthorizationNumberValueLabel, 1, 1);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryAuthorizationCompanyRepresentativeNameLabel, 0, 2);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryAuthorizationCompanyRepresentativeNameValueLabel, 1, 2);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryDaysAuthorizedValueLabel, 1, 3);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryDaysAuthorizedLabel, 0, 3);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryServicesAuthorizedLabel, 0, 4);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryServicesAuthorizedValueLabel, 1, 4);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryEffectiveDateOfAuthorizationLabel, 0, 5);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryEffectiveDateOfAuthorizationValueLabel, 1, 5);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryExpirationDateOfAuthorizationValueLabel, 1, 6);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryExpirationDateOfAuthorizationLabel, 0, 6);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryAuthorizationStatusLabel, 0, 7);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryAuthorizationStatusValueLabel, 1, 7);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryRemarksValueLabel, 1, 8);
            this.primaryInsuranceAuthorizationBody.Controls.Add(this.primaryRemarksLabel, 0, 8);
            this.primaryInsuranceAuthorizationBody.Location = new System.Drawing.Point(3, 2501);
            this.primaryInsuranceAuthorizationBody.Name = "primaryInsuranceAuthorizationBody";
            this.primaryInsuranceAuthorizationBody.RowCount = 9;
            this.primaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.primaryInsuranceAuthorizationBody.Size = new System.Drawing.Size(692, 190);
            this.primaryInsuranceAuthorizationBody.TabIndex = 570;
            // 
            // primaryTrackingNumberLabel
            // 
            this.primaryTrackingNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryTrackingNumberLabel.Location = new System.Drawing.Point(3, 0);
            this.primaryTrackingNumberLabel.Name = "primaryTrackingNumberLabel";
            this.primaryTrackingNumberLabel.Size = new System.Drawing.Size(127, 16);
            this.primaryTrackingNumberLabel.TabIndex = 479;
            this.primaryTrackingNumberLabel.Text = "Tracking Number:";
            // 
            // primaryAuthorizationTrackingNumberValueLabel
            // 
            this.primaryAuthorizationTrackingNumberValueLabel.AutoSize = true;
            this.primaryAuthorizationTrackingNumberValueLabel.Location = new System.Drawing.Point(288, 0);
            this.primaryAuthorizationTrackingNumberValueLabel.Name = "primaryAuthorizationTrackingNumberValueLabel";
            this.primaryAuthorizationTrackingNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAuthorizationTrackingNumberValueLabel.TabIndex = 486;
            // 
            // primaryAuthorizationNumberLabel
            // 
            this.primaryAuthorizationNumberLabel.BackColor = System.Drawing.Color.White;
            this.primaryAuthorizationNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationNumberLabel.Location = new System.Drawing.Point(3, 20);
            this.primaryAuthorizationNumberLabel.Name = "primaryAuthorizationNumberLabel";
            this.primaryAuthorizationNumberLabel.Size = new System.Drawing.Size(132, 16);
            this.primaryAuthorizationNumberLabel.TabIndex = 477;
            this.primaryAuthorizationNumberLabel.Text = "Authorization Number:";
            // 
            // primaryAuthorizationNumberValueLabel
            // 
            this.primaryAuthorizationNumberValueLabel.Location = new System.Drawing.Point(288, 20);
            this.primaryAuthorizationNumberValueLabel.Name = "primaryAuthorizationNumberValueLabel";
            this.primaryAuthorizationNumberValueLabel.Size = new System.Drawing.Size(200, 16);
            this.primaryAuthorizationNumberValueLabel.TabIndex = 484;
            // 
            // primaryAuthorizationCompanyRepresentativeNameLabel
            // 
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Location = new System.Drawing.Point(3, 40);
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Name = "primaryAuthorizationCompanyRepresentativeNameLabel";
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Size = new System.Drawing.Size(279, 16);
            this.primaryAuthorizationCompanyRepresentativeNameLabel.TabIndex = 480;
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Text = "Authorization Company Representative Name:";
            // 
            // primaryAuthorizationCompanyRepresentativeNameValueLabel
            // 
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.AutoSize = true;
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.Location = new System.Drawing.Point(288, 40);
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.Name = "primaryAuthorizationCompanyRepresentativeNameValueLabel";
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.TabIndex = 487;
            // 
            // primaryDaysAuthorizedValueLabel
            // 
            this.primaryDaysAuthorizedValueLabel.AutoSize = true;
            this.primaryDaysAuthorizedValueLabel.Location = new System.Drawing.Point(288, 60);
            this.primaryDaysAuthorizedValueLabel.Name = "primaryDaysAuthorizedValueLabel";
            this.primaryDaysAuthorizedValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryDaysAuthorizedValueLabel.TabIndex = 485;
            // 
            // primaryDaysAuthorizedLabel
            // 
            this.primaryDaysAuthorizedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryDaysAuthorizedLabel.Location = new System.Drawing.Point(3, 60);
            this.primaryDaysAuthorizedLabel.Name = "primaryDaysAuthorizedLabel";
            this.primaryDaysAuthorizedLabel.Size = new System.Drawing.Size(164, 16);
            this.primaryDaysAuthorizedLabel.TabIndex = 478;
            this.primaryDaysAuthorizedLabel.Text = "Days Authorized:";
            // 
            // primaryServicesAuthorizedLabel
            // 
            this.primaryServicesAuthorizedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryServicesAuthorizedLabel.Location = new System.Drawing.Point(3, 80);
            this.primaryServicesAuthorizedLabel.Name = "primaryServicesAuthorizedLabel";
            this.primaryServicesAuthorizedLabel.Size = new System.Drawing.Size(164, 16);
            this.primaryServicesAuthorizedLabel.TabIndex = 494;
            this.primaryServicesAuthorizedLabel.Text = "Services Authorized:";
            // 
            // primaryServicesAuthorizedValueLabel
            // 
            this.primaryServicesAuthorizedValueLabel.AutoSize = true;
            this.primaryServicesAuthorizedValueLabel.Location = new System.Drawing.Point(288, 80);
            this.primaryServicesAuthorizedValueLabel.Name = "primaryServicesAuthorizedValueLabel";
            this.primaryServicesAuthorizedValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryServicesAuthorizedValueLabel.TabIndex = 495;
            // 
            // primaryEffectiveDateOfAuthorizationLabel
            // 
            this.primaryEffectiveDateOfAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEffectiveDateOfAuthorizationLabel.Location = new System.Drawing.Point(3, 100);
            this.primaryEffectiveDateOfAuthorizationLabel.Name = "primaryEffectiveDateOfAuthorizationLabel";
            this.primaryEffectiveDateOfAuthorizationLabel.Size = new System.Drawing.Size(193, 16);
            this.primaryEffectiveDateOfAuthorizationLabel.TabIndex = 481;
            this.primaryEffectiveDateOfAuthorizationLabel.Text = "Effective Date of Authorization:";
            // 
            // primaryEffectiveDateOfAuthorizationValueLabel
            // 
            this.primaryEffectiveDateOfAuthorizationValueLabel.AutoSize = true;
            this.primaryEffectiveDateOfAuthorizationValueLabel.Location = new System.Drawing.Point(288, 100);
            this.primaryEffectiveDateOfAuthorizationValueLabel.Name = "primaryEffectiveDateOfAuthorizationValueLabel";
            this.primaryEffectiveDateOfAuthorizationValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryEffectiveDateOfAuthorizationValueLabel.TabIndex = 488;
            // 
            // primaryExpirationDateOfAuthorizationValueLabel
            // 
            this.primaryExpirationDateOfAuthorizationValueLabel.AutoSize = true;
            this.primaryExpirationDateOfAuthorizationValueLabel.Location = new System.Drawing.Point(288, 120);
            this.primaryExpirationDateOfAuthorizationValueLabel.Name = "primaryExpirationDateOfAuthorizationValueLabel";
            this.primaryExpirationDateOfAuthorizationValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryExpirationDateOfAuthorizationValueLabel.TabIndex = 489;
            // 
            // primaryExpirationDateOfAuthorizationLabel
            // 
            this.primaryExpirationDateOfAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryExpirationDateOfAuthorizationLabel.Location = new System.Drawing.Point(3, 120);
            this.primaryExpirationDateOfAuthorizationLabel.Name = "primaryExpirationDateOfAuthorizationLabel";
            this.primaryExpirationDateOfAuthorizationLabel.Size = new System.Drawing.Size(213, 16);
            this.primaryExpirationDateOfAuthorizationLabel.TabIndex = 482;
            this.primaryExpirationDateOfAuthorizationLabel.Text = "Expiration Date of Authorization:";
            // 
            // primaryAuthorizationStatusLabel
            // 
            this.primaryAuthorizationStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationStatusLabel.Location = new System.Drawing.Point(3, 140);
            this.primaryAuthorizationStatusLabel.Name = "primaryAuthorizationStatusLabel";
            this.primaryAuthorizationStatusLabel.Size = new System.Drawing.Size(193, 16);
            this.primaryAuthorizationStatusLabel.TabIndex = 483;
            this.primaryAuthorizationStatusLabel.Text = "Authorization Status:";
            // 
            // primaryAuthorizationStatusValueLabel
            // 
            this.primaryAuthorizationStatusValueLabel.AutoSize = true;
            this.primaryAuthorizationStatusValueLabel.Location = new System.Drawing.Point(288, 140);
            this.primaryAuthorizationStatusValueLabel.Name = "primaryAuthorizationStatusValueLabel";
            this.primaryAuthorizationStatusValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryAuthorizationStatusValueLabel.TabIndex = 490;
            // 
            // primaryRemarksValueLabel
            // 
            this.primaryRemarksValueLabel.AutoSize = true;
            this.primaryRemarksValueLabel.Location = new System.Drawing.Point(288, 160);
            this.primaryRemarksValueLabel.Name = "primaryRemarksValueLabel";
            this.primaryRemarksValueLabel.Size = new System.Drawing.Size(0, 17);
            this.primaryRemarksValueLabel.TabIndex = 493;
            // 
            // primaryRemarksLabel
            // 
            this.primaryRemarksLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryRemarksLabel.Location = new System.Drawing.Point(3, 160);
            this.primaryRemarksLabel.Name = "primaryRemarksLabel";
            this.primaryRemarksLabel.Size = new System.Drawing.Size(104, 16);
            this.primaryRemarksLabel.TabIndex = 492;
            this.primaryRemarksLabel.Text = "Remarks:";
            // 
            // secondaryInsurancePlanHeader
            // 
            this.secondaryInsurancePlanHeader.ColumnCount = 1;
            this.secondaryInsurancePlanHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.secondaryInsurancePlanHeader.Controls.Add(this.panel9, 0, 2);
            this.secondaryInsurancePlanHeader.Controls.Add(this.secondaryInsurancePlanLabel, 0, 1);
            this.secondaryInsurancePlanHeader.Location = new System.Drawing.Point(3, 2699);
            this.secondaryInsurancePlanHeader.Name = "secondaryInsurancePlanHeader";
            this.secondaryInsurancePlanHeader.RowCount = 3;
            this.secondaryInsurancePlanHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.secondaryInsurancePlanHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePlanHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.secondaryInsurancePlanHeader.Size = new System.Drawing.Size(597, 32);
            this.secondaryInsurancePlanHeader.TabIndex = 567;
            // 
            // panel9
            // 
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Location = new System.Drawing.Point(3, 23);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(591, 1);
            this.panel9.TabIndex = 441;
            // 
            // secondaryInsurancePlanLabel
            // 
            this.secondaryInsurancePlanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsurancePlanLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryInsurancePlanLabel.Name = "secondaryInsurancePlanLabel";
            this.secondaryInsurancePlanLabel.Size = new System.Drawing.Size(231, 20);
            this.secondaryInsurancePlanLabel.TabIndex = 230;
            this.secondaryInsurancePlanLabel.Text = "Secondary Insurance - Plan";
            // 
            // secondaryInsurancePlanBody
            // 
            this.secondaryInsurancePlanBody.ColumnCount = 2;
            this.secondaryInsurancePlanBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.32881F));
            this.secondaryInsurancePlanBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.67119F));
            this.secondaryInsurancePlanBody.Controls.Add(this.secondaryPlanValueLabel, 1, 0);
            this.secondaryInsurancePlanBody.Controls.Add(this.secondaryPayorBrokerValueLabel, 1, 1);
            this.secondaryInsurancePlanBody.Controls.Add(this.secondaryCategoryValueLabel, 1, 2);
            this.secondaryInsurancePlanBody.Controls.Add(this.secondaryCategoryLabel, 0, 2);
            this.secondaryInsurancePlanBody.Controls.Add(this.secondaryPayorBrokerLabel, 0, 1);
            this.secondaryInsurancePlanBody.Controls.Add(this.secondaryPlanLabel, 0, 0);
            this.secondaryInsurancePlanBody.Location = new System.Drawing.Point(3, 2737);
            this.secondaryInsurancePlanBody.Name = "secondaryInsurancePlanBody";
            this.secondaryInsurancePlanBody.RowCount = 3;
            this.secondaryInsurancePlanBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePlanBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePlanBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePlanBody.Size = new System.Drawing.Size(739, 64);
            this.secondaryInsurancePlanBody.TabIndex = 568;
            // 
            // secondaryPlanValueLabel
            // 
            this.secondaryPlanValueLabel.AutoSize = true;
            this.secondaryPlanValueLabel.Location = new System.Drawing.Point(108, 0);
            this.secondaryPlanValueLabel.Name = "secondaryPlanValueLabel";
            this.secondaryPlanValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryPlanValueLabel.TabIndex = 232;
            // 
            // secondaryPayorBrokerValueLabel
            // 
            this.secondaryPayorBrokerValueLabel.AutoSize = true;
            this.secondaryPayorBrokerValueLabel.Location = new System.Drawing.Point(108, 20);
            this.secondaryPayorBrokerValueLabel.Name = "secondaryPayorBrokerValueLabel";
            this.secondaryPayorBrokerValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryPayorBrokerValueLabel.TabIndex = 234;
            // 
            // secondaryCategoryValueLabel
            // 
            this.secondaryCategoryValueLabel.AutoSize = true;
            this.secondaryCategoryValueLabel.Location = new System.Drawing.Point(108, 40);
            this.secondaryCategoryValueLabel.Name = "secondaryCategoryValueLabel";
            this.secondaryCategoryValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryCategoryValueLabel.TabIndex = 236;
            // 
            // secondaryCategoryLabel
            // 
            this.secondaryCategoryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryCategoryLabel.Location = new System.Drawing.Point(3, 40);
            this.secondaryCategoryLabel.Name = "secondaryCategoryLabel";
            this.secondaryCategoryLabel.Size = new System.Drawing.Size(73, 16);
            this.secondaryCategoryLabel.TabIndex = 235;
            this.secondaryCategoryLabel.Text = "Category:";
            // 
            // secondaryPayorBrokerLabel
            // 
            this.secondaryPayorBrokerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryPayorBrokerLabel.Location = new System.Drawing.Point(3, 20);
            this.secondaryPayorBrokerLabel.Name = "secondaryPayorBrokerLabel";
            this.secondaryPayorBrokerLabel.Size = new System.Drawing.Size(99, 16);
            this.secondaryPayorBrokerLabel.TabIndex = 233;
            this.secondaryPayorBrokerLabel.Text = "Payor/Broker:";
            // 
            // secondaryPlanLabel
            // 
            this.secondaryPlanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryPlanLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryPlanLabel.Name = "secondaryPlanLabel";
            this.secondaryPlanLabel.Size = new System.Drawing.Size(56, 16);
            this.secondaryPlanLabel.TabIndex = 231;
            this.secondaryPlanLabel.Text = "Plan:";
            // 
            // SecondaryInsurancePayorHeader
            // 
            this.SecondaryInsurancePayorHeader.ColumnCount = 1;
            this.SecondaryInsurancePayorHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SecondaryInsurancePayorHeader.Controls.Add(this.panel8, 0, 2);
            this.SecondaryInsurancePayorHeader.Controls.Add(this.secondaryInsurancePayorDetailsLabel, 0, 1);
            this.SecondaryInsurancePayorHeader.Location = new System.Drawing.Point(3, 2819);
            this.SecondaryInsurancePayorHeader.Name = "SecondaryInsurancePayorHeader";
            this.SecondaryInsurancePayorHeader.RowCount = 3;
            this.SecondaryInsurancePayorHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SecondaryInsurancePayorHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SecondaryInsurancePayorHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SecondaryInsurancePayorHeader.Size = new System.Drawing.Size(597, 32);
            this.SecondaryInsurancePayorHeader.TabIndex = 565;
            // 
            // panel8
            // 
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Location = new System.Drawing.Point(3, 23);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(591, 1);
            this.panel8.TabIndex = 441;
            // 
            // secondaryInsurancePayorDetailsLabel
            // 
            this.secondaryInsurancePayorDetailsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsurancePayorDetailsLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryInsurancePayorDetailsLabel.Name = "secondaryInsurancePayorDetailsLabel";
            this.secondaryInsurancePayorDetailsLabel.Size = new System.Drawing.Size(302, 20);
            this.secondaryInsurancePayorDetailsLabel.TabIndex = 237;
            this.secondaryInsurancePayorDetailsLabel.Text = "Secondary Insurance – Payor Details";
            // 
            // secondaryInsurancePayorDetailsBody
            // 
            this.secondaryInsurancePayorDetailsBody.ColumnCount = 2;
            this.secondaryInsurancePayorDetailsBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.69683F));
            this.secondaryInsurancePayorDetailsBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.30317F));
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel, 0, 0);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryGroupNumberLabel, 0, 1);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryPrecertificationNumberLabel, 0, 2);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryMedicaidIssueDateLabel, 0, 3);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryEmployeeSupervisorLabel, 0, 4);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryAdjusterLabel, 0, 5);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryMedicalGroupIpaNameLabel, 0, 6);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryMedicalGroupIpaClinicLabel, 0, 7);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryBillingCoNameLabel, 0, 8);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryBillingNameLabel, 0, 9);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryBillingPhoneLabel, 0, 10);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryBillingAddressLabel, 0, 11);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel, 1, 0);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryGroupNumberValueLabel, 1, 1);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryPrecertificationNumberValueLabel, 1, 2);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryMedicaidIssueDateValueLabel, 1, 3);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryEmployeeSupervisorValueLabel, 1, 4);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryAdjusterValueLabel, 1, 5);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryMedicalGroupIpaNameValueLabel, 1, 6);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryMedicalGroupIpaClinicValueLabel, 1, 7);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryBillingCoNameValueLabel, 1, 8);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryBillingNameValueLabel, 1, 9);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryBillingPhoneValueLabel, 1, 10);
            this.secondaryInsurancePayorDetailsBody.Controls.Add(this.secondaryBillingAddressValueLabel, 1, 11);
            this.secondaryInsurancePayorDetailsBody.Location = new System.Drawing.Point(3, 2857);
            this.secondaryInsurancePayorDetailsBody.Name = "secondaryInsurancePayorDetailsBody";
            this.secondaryInsurancePayorDetailsBody.RowCount = 12;
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsurancePayorDetailsBody.Size = new System.Drawing.Size(597, 248);
            this.secondaryInsurancePayorDetailsBody.TabIndex = 566;
            // 
            // secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel
            // 
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Name = "secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel";
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size = new System.Drawing.Size(153, 16);
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.TabIndex = 238;
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text = "CERT/SSN/ID number:";
            // 
            // secondaryGroupNumberLabel
            // 
            this.secondaryGroupNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryGroupNumberLabel.Location = new System.Drawing.Point(3, 20);
            this.secondaryGroupNumberLabel.Name = "secondaryGroupNumberLabel";
            this.secondaryGroupNumberLabel.Size = new System.Drawing.Size(139, 16);
            this.secondaryGroupNumberLabel.TabIndex = 240;
            this.secondaryGroupNumberLabel.Text = "Group number:";
            // 
            // secondaryPrecertificationNumberLabel
            // 
            this.secondaryPrecertificationNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryPrecertificationNumberLabel.Location = new System.Drawing.Point(3, 40);
            this.secondaryPrecertificationNumberLabel.Name = "secondaryPrecertificationNumberLabel";
            this.secondaryPrecertificationNumberLabel.Size = new System.Drawing.Size(153, 16);
            this.secondaryPrecertificationNumberLabel.TabIndex = 248;
            this.secondaryPrecertificationNumberLabel.Text = "Precertification number:";
            // 
            // secondaryMedicaidIssueDateLabel
            // 
            this.secondaryMedicaidIssueDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryMedicaidIssueDateLabel.Location = new System.Drawing.Point(3, 60);
            this.secondaryMedicaidIssueDateLabel.Name = "secondaryMedicaidIssueDateLabel";
            this.secondaryMedicaidIssueDateLabel.Size = new System.Drawing.Size(153, 16);
            this.secondaryMedicaidIssueDateLabel.TabIndex = 250;
            this.secondaryMedicaidIssueDateLabel.Text = "Medicaid issue date:";
            // 
            // secondaryEmployeeSupervisorLabel
            // 
            this.secondaryEmployeeSupervisorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEmployeeSupervisorLabel.Location = new System.Drawing.Point(3, 80);
            this.secondaryEmployeeSupervisorLabel.Name = "secondaryEmployeeSupervisorLabel";
            this.secondaryEmployeeSupervisorLabel.Size = new System.Drawing.Size(153, 16);
            this.secondaryEmployeeSupervisorLabel.TabIndex = 252;
            this.secondaryEmployeeSupervisorLabel.Text = "Employee’s supervisor:";
            // 
            // secondaryAdjusterLabel
            // 
            this.secondaryAdjusterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAdjusterLabel.Location = new System.Drawing.Point(3, 100);
            this.secondaryAdjusterLabel.Name = "secondaryAdjusterLabel";
            this.secondaryAdjusterLabel.Size = new System.Drawing.Size(117, 16);
            this.secondaryAdjusterLabel.TabIndex = 254;
            this.secondaryAdjusterLabel.Text = "Adjuster:";
            // 
            // secondaryMedicalGroupIpaNameLabel
            // 
            this.secondaryMedicalGroupIpaNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryMedicalGroupIpaNameLabel.Location = new System.Drawing.Point(3, 120);
            this.secondaryMedicalGroupIpaNameLabel.Name = "secondaryMedicalGroupIpaNameLabel";
            this.secondaryMedicalGroupIpaNameLabel.Size = new System.Drawing.Size(153, 16);
            this.secondaryMedicalGroupIpaNameLabel.TabIndex = 256;
            this.secondaryMedicalGroupIpaNameLabel.Text = "Medical group/IPA name:";
            // 
            // secondaryMedicalGroupIpaClinicLabel
            // 
            this.secondaryMedicalGroupIpaClinicLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryMedicalGroupIpaClinicLabel.Location = new System.Drawing.Point(3, 140);
            this.secondaryMedicalGroupIpaClinicLabel.Name = "secondaryMedicalGroupIpaClinicLabel";
            this.secondaryMedicalGroupIpaClinicLabel.Size = new System.Drawing.Size(153, 16);
            this.secondaryMedicalGroupIpaClinicLabel.TabIndex = 258;
            this.secondaryMedicalGroupIpaClinicLabel.Text = "Medical group/IPA clinic:";
            // 
            // secondaryBillingCoNameLabel
            // 
            this.secondaryBillingCoNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBillingCoNameLabel.Location = new System.Drawing.Point(3, 160);
            this.secondaryBillingCoNameLabel.Name = "secondaryBillingCoNameLabel";
            this.secondaryBillingCoNameLabel.Size = new System.Drawing.Size(132, 16);
            this.secondaryBillingCoNameLabel.TabIndex = 260;
            this.secondaryBillingCoNameLabel.Text = "Billing c/o name:";
            // 
            // secondaryBillingNameLabel
            // 
            this.secondaryBillingNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBillingNameLabel.Location = new System.Drawing.Point(3, 180);
            this.secondaryBillingNameLabel.Name = "secondaryBillingNameLabel";
            this.secondaryBillingNameLabel.Size = new System.Drawing.Size(115, 16);
            this.secondaryBillingNameLabel.TabIndex = 262;
            this.secondaryBillingNameLabel.Text = "Billing name:";
            // 
            // secondaryBillingPhoneLabel
            // 
            this.secondaryBillingPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBillingPhoneLabel.Location = new System.Drawing.Point(3, 200);
            this.secondaryBillingPhoneLabel.Name = "secondaryBillingPhoneLabel";
            this.secondaryBillingPhoneLabel.Size = new System.Drawing.Size(117, 16);
            this.secondaryBillingPhoneLabel.TabIndex = 264;
            this.secondaryBillingPhoneLabel.Text = "Billing phone:";
            // 
            // secondaryBillingAddressLabel
            // 
            this.secondaryBillingAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBillingAddressLabel.Location = new System.Drawing.Point(3, 220);
            this.secondaryBillingAddressLabel.Name = "secondaryBillingAddressLabel";
            this.secondaryBillingAddressLabel.Size = new System.Drawing.Size(115, 16);
            this.secondaryBillingAddressLabel.TabIndex = 266;
            this.secondaryBillingAddressLabel.Text = "Billing address:";
            // 
            // secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel
            // 
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location = new System.Drawing.Point(162, 0);
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Name = "secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel";
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Size = new System.Drawing.Size(200, 16);
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.TabIndex = 239;
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Text = "secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel";
            // 
            // secondaryGroupNumberValueLabel
            // 
            this.secondaryGroupNumberValueLabel.AutoSize = true;
            this.secondaryGroupNumberValueLabel.Location = new System.Drawing.Point(162, 20);
            this.secondaryGroupNumberValueLabel.Name = "secondaryGroupNumberValueLabel";
            this.secondaryGroupNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryGroupNumberValueLabel.TabIndex = 241;
            // 
            // secondaryPrecertificationNumberValueLabel
            // 
            this.secondaryPrecertificationNumberValueLabel.AutoSize = true;
            this.secondaryPrecertificationNumberValueLabel.Location = new System.Drawing.Point(162, 40);
            this.secondaryPrecertificationNumberValueLabel.Name = "secondaryPrecertificationNumberValueLabel";
            this.secondaryPrecertificationNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryPrecertificationNumberValueLabel.TabIndex = 249;
            // 
            // secondaryMedicaidIssueDateValueLabel
            // 
            this.secondaryMedicaidIssueDateValueLabel.AutoSize = true;
            this.secondaryMedicaidIssueDateValueLabel.Location = new System.Drawing.Point(162, 60);
            this.secondaryMedicaidIssueDateValueLabel.Name = "secondaryMedicaidIssueDateValueLabel";
            this.secondaryMedicaidIssueDateValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryMedicaidIssueDateValueLabel.TabIndex = 251;
            // 
            // secondaryEmployeeSupervisorValueLabel
            // 
            this.secondaryEmployeeSupervisorValueLabel.AutoSize = true;
            this.secondaryEmployeeSupervisorValueLabel.Location = new System.Drawing.Point(162, 80);
            this.secondaryEmployeeSupervisorValueLabel.Name = "secondaryEmployeeSupervisorValueLabel";
            this.secondaryEmployeeSupervisorValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryEmployeeSupervisorValueLabel.TabIndex = 253;
            // 
            // secondaryAdjusterValueLabel
            // 
            this.secondaryAdjusterValueLabel.AutoSize = true;
            this.secondaryAdjusterValueLabel.Location = new System.Drawing.Point(162, 100);
            this.secondaryAdjusterValueLabel.Name = "secondaryAdjusterValueLabel";
            this.secondaryAdjusterValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAdjusterValueLabel.TabIndex = 255;
            // 
            // secondaryMedicalGroupIpaNameValueLabel
            // 
            this.secondaryMedicalGroupIpaNameValueLabel.AutoSize = true;
            this.secondaryMedicalGroupIpaNameValueLabel.Location = new System.Drawing.Point(162, 120);
            this.secondaryMedicalGroupIpaNameValueLabel.Name = "secondaryMedicalGroupIpaNameValueLabel";
            this.secondaryMedicalGroupIpaNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryMedicalGroupIpaNameValueLabel.TabIndex = 257;
            // 
            // secondaryMedicalGroupIpaClinicValueLabel
            // 
            this.secondaryMedicalGroupIpaClinicValueLabel.AutoSize = true;
            this.secondaryMedicalGroupIpaClinicValueLabel.Location = new System.Drawing.Point(162, 140);
            this.secondaryMedicalGroupIpaClinicValueLabel.Name = "secondaryMedicalGroupIpaClinicValueLabel";
            this.secondaryMedicalGroupIpaClinicValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryMedicalGroupIpaClinicValueLabel.TabIndex = 259;
            // 
            // secondaryBillingCoNameValueLabel
            // 
            this.secondaryBillingCoNameValueLabel.AutoSize = true;
            this.secondaryBillingCoNameValueLabel.Location = new System.Drawing.Point(162, 160);
            this.secondaryBillingCoNameValueLabel.Name = "secondaryBillingCoNameValueLabel";
            this.secondaryBillingCoNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryBillingCoNameValueLabel.TabIndex = 261;
            // 
            // secondaryBillingNameValueLabel
            // 
            this.secondaryBillingNameValueLabel.AutoSize = true;
            this.secondaryBillingNameValueLabel.Location = new System.Drawing.Point(162, 180);
            this.secondaryBillingNameValueLabel.Name = "secondaryBillingNameValueLabel";
            this.secondaryBillingNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryBillingNameValueLabel.TabIndex = 263;
            // 
            // secondaryBillingPhoneValueLabel
            // 
            this.secondaryBillingPhoneValueLabel.AutoSize = true;
            this.secondaryBillingPhoneValueLabel.Location = new System.Drawing.Point(162, 200);
            this.secondaryBillingPhoneValueLabel.Name = "secondaryBillingPhoneValueLabel";
            this.secondaryBillingPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryBillingPhoneValueLabel.TabIndex = 265;
            // 
            // secondaryBillingAddressValueLabel
            // 
            this.secondaryBillingAddressValueLabel.AutoSize = true;
            this.secondaryBillingAddressValueLabel.Location = new System.Drawing.Point(162, 220);
            this.secondaryBillingAddressValueLabel.Name = "secondaryBillingAddressValueLabel";
            this.secondaryBillingAddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryBillingAddressValueLabel.TabIndex = 267;
            // 
            // secondaryInsuranceInsuredHeader
            // 
            this.secondaryInsuranceInsuredHeader.ColumnCount = 1;
            this.secondaryInsuranceInsuredHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.secondaryInsuranceInsuredHeader.Controls.Add(this.panel7, 0, 2);
            this.secondaryInsuranceInsuredHeader.Controls.Add(this.secondaryInsuranceInsuredLabel, 0, 1);
            this.secondaryInsuranceInsuredHeader.Location = new System.Drawing.Point(3, 3114);
            this.secondaryInsuranceInsuredHeader.Name = "secondaryInsuranceInsuredHeader";
            this.secondaryInsuranceInsuredHeader.RowCount = 3;
            this.secondaryInsuranceInsuredHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.secondaryInsuranceInsuredHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.secondaryInsuranceInsuredHeader.Size = new System.Drawing.Size(597, 30);
            this.secondaryInsuranceInsuredHeader.TabIndex = 563;
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Location = new System.Drawing.Point(3, 23);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(591, 1);
            this.panel7.TabIndex = 441;
            // 
            // secondaryInsuranceInsuredLabel
            // 
            this.secondaryInsuranceInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsuranceInsuredLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryInsuranceInsuredLabel.Name = "secondaryInsuranceInsuredLabel";
            this.secondaryInsuranceInsuredLabel.Size = new System.Drawing.Size(254, 20);
            this.secondaryInsuranceInsuredLabel.TabIndex = 268;
            this.secondaryInsuranceInsuredLabel.Text = "Secondary Insurance – Insured";
            // 
            // secondaryInsuranceInsuredBody
            // 
            this.secondaryInsuranceInsuredBody.ColumnCount = 2;
            this.secondaryInsuranceInsuredBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.81575F));
            this.secondaryInsuranceInsuredBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 350.1843F));
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryNameValueLabel, 1, 0);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryGenderLabel, 0, 2);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryGenderValueLabel, 1, 2);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryThePatientIsTheInsuredLabel, 0, 1);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryThePatientIsTheInsuredValueLabel, 1, 1);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryDobValueLabel, 1, 3);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryDobLabel, 0, 3);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryNationalIdLabel, 0, 4);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryNationalIdValueLabel, 1, 4);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryAddressValueLabel, 1, 5);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryAddressLabel, 0, 5);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryContactPhoneValueLabel, 1, 6);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryContactPhoneLabel, 0, 6);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryContactCellValueLabel, 1, 7);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryContactCellLabel, 0, 7);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryEmploymentStatusValueLabel, 1, 8);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryEmploymentStatusLabel, 0, 8);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryEmployerValueLabel, 1, 9);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryEmployerLabel, 0, 9);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryEmployerPhoneValueLabel, 1, 10);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryEmployerPhoneLabel, 0, 10);
            this.secondaryInsuranceInsuredBody.Controls.Add(this.secondaryNameLabel, 0, 0);
            this.secondaryInsuranceInsuredBody.Location = new System.Drawing.Point(3, 3150);
            this.secondaryInsuranceInsuredBody.Name = "secondaryInsuranceInsuredBody";
            this.secondaryInsuranceInsuredBody.RowCount = 11;
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceInsuredBody.Size = new System.Drawing.Size(894, 230);
            this.secondaryInsuranceInsuredBody.TabIndex = 564;
            // 
            // secondaryNameValueLabel
            // 
            this.secondaryNameValueLabel.AutoSize = true;
            this.secondaryNameValueLabel.Location = new System.Drawing.Point(170, 0);
            this.secondaryNameValueLabel.Name = "secondaryNameValueLabel";
            this.secondaryNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryNameValueLabel.TabIndex = 270;
            // 
            // secondaryGenderLabel
            // 
            this.secondaryGenderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryGenderLabel.Location = new System.Drawing.Point(3, 40);
            this.secondaryGenderLabel.Name = "secondaryGenderLabel";
            this.secondaryGenderLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryGenderLabel.TabIndex = 273;
            this.secondaryGenderLabel.Text = "Gender:";
            // 
            // secondaryGenderValueLabel
            // 
            this.secondaryGenderValueLabel.AutoSize = true;
            this.secondaryGenderValueLabel.Location = new System.Drawing.Point(170, 40);
            this.secondaryGenderValueLabel.Name = "secondaryGenderValueLabel";
            this.secondaryGenderValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryGenderValueLabel.TabIndex = 274;
            // 
            // secondaryThePatientIsTheInsuredLabel
            // 
            this.secondaryThePatientIsTheInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryThePatientIsTheInsuredLabel.Location = new System.Drawing.Point(3, 20);
            this.secondaryThePatientIsTheInsuredLabel.Name = "secondaryThePatientIsTheInsuredLabel";
            this.secondaryThePatientIsTheInsuredLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryThePatientIsTheInsuredLabel.TabIndex = 271;
            this.secondaryThePatientIsTheInsuredLabel.Text = "The Patient is the Insured’s:";
            // 
            // secondaryThePatientIsTheInsuredValueLabel
            // 
            this.secondaryThePatientIsTheInsuredValueLabel.AutoSize = true;
            this.secondaryThePatientIsTheInsuredValueLabel.Location = new System.Drawing.Point(170, 20);
            this.secondaryThePatientIsTheInsuredValueLabel.Name = "secondaryThePatientIsTheInsuredValueLabel";
            this.secondaryThePatientIsTheInsuredValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryThePatientIsTheInsuredValueLabel.TabIndex = 272;
            // 
            // secondaryDobValueLabel
            // 
            this.secondaryDobValueLabel.AutoSize = true;
            this.secondaryDobValueLabel.Location = new System.Drawing.Point(170, 60);
            this.secondaryDobValueLabel.Name = "secondaryDobValueLabel";
            this.secondaryDobValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryDobValueLabel.TabIndex = 276;
            // 
            // secondaryDobLabel
            // 
            this.secondaryDobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryDobLabel.Location = new System.Drawing.Point(3, 60);
            this.secondaryDobLabel.Name = "secondaryDobLabel";
            this.secondaryDobLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryDobLabel.TabIndex = 275;
            this.secondaryDobLabel.Text = "DOB:";
            // 
            // secondaryNationalIdLabel
            // 
            this.secondaryNationalIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryNationalIdLabel.Location = new System.Drawing.Point(3, 80);
            this.secondaryNationalIdLabel.Name = "secondaryNationalIdLabel";
            this.secondaryNationalIdLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryNationalIdLabel.TabIndex = 277;
            this.secondaryNationalIdLabel.Text = "National ID:";
            // 
            // secondaryNationalIdValueLabel
            // 
            this.secondaryNationalIdValueLabel.AutoSize = true;
            this.secondaryNationalIdValueLabel.Location = new System.Drawing.Point(170, 80);
            this.secondaryNationalIdValueLabel.Name = "secondaryNationalIdValueLabel";
            this.secondaryNationalIdValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryNationalIdValueLabel.TabIndex = 278;
            // 
            // secondaryAddressValueLabel
            // 
            this.secondaryAddressValueLabel.AutoSize = true;
            this.secondaryAddressValueLabel.Location = new System.Drawing.Point(170, 100);
            this.secondaryAddressValueLabel.Name = "secondaryAddressValueLabel";
            this.secondaryAddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAddressValueLabel.TabIndex = 280;
            // 
            // secondaryAddressLabel
            // 
            this.secondaryAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAddressLabel.Location = new System.Drawing.Point(3, 100);
            this.secondaryAddressLabel.Name = "secondaryAddressLabel";
            this.secondaryAddressLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryAddressLabel.TabIndex = 279;
            this.secondaryAddressLabel.Text = "Address:";
            // 
            // secondaryContactPhoneValueLabel
            // 
            this.secondaryContactPhoneValueLabel.AutoSize = true;
            this.secondaryContactPhoneValueLabel.Location = new System.Drawing.Point(170, 120);
            this.secondaryContactPhoneValueLabel.Name = "secondaryContactPhoneValueLabel";
            this.secondaryContactPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryContactPhoneValueLabel.TabIndex = 282;
            // 
            // secondaryContactPhoneLabel
            // 
            this.secondaryContactPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryContactPhoneLabel.Location = new System.Drawing.Point(3, 120);
            this.secondaryContactPhoneLabel.Name = "secondaryContactPhoneLabel";
            this.secondaryContactPhoneLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryContactPhoneLabel.TabIndex = 281;
            this.secondaryContactPhoneLabel.Text = "Contact phone:";
            // 
            // secondaryContactCellValueLabel
            // 
            this.secondaryContactCellValueLabel.AutoSize = true;
            this.secondaryContactCellValueLabel.Location = new System.Drawing.Point(170, 140);
            this.secondaryContactCellValueLabel.Name = "secondaryContactCellValueLabel";
            this.secondaryContactCellValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryContactCellValueLabel.TabIndex = 284;
            // 
            // secondaryContactCellLabel
            // 
            this.secondaryContactCellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryContactCellLabel.Location = new System.Drawing.Point(3, 140);
            this.secondaryContactCellLabel.Name = "secondaryContactCellLabel";
            this.secondaryContactCellLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryContactCellLabel.TabIndex = 283;
            this.secondaryContactCellLabel.Text = "Contact cell:";
            // 
            // secondaryEmploymentStatusValueLabel
            // 
            this.secondaryEmploymentStatusValueLabel.AutoSize = true;
            this.secondaryEmploymentStatusValueLabel.Location = new System.Drawing.Point(170, 160);
            this.secondaryEmploymentStatusValueLabel.Name = "secondaryEmploymentStatusValueLabel";
            this.secondaryEmploymentStatusValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryEmploymentStatusValueLabel.TabIndex = 286;
            // 
            // secondaryEmploymentStatusLabel
            // 
            this.secondaryEmploymentStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEmploymentStatusLabel.Location = new System.Drawing.Point(3, 160);
            this.secondaryEmploymentStatusLabel.Name = "secondaryEmploymentStatusLabel";
            this.secondaryEmploymentStatusLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryEmploymentStatusLabel.TabIndex = 285;
            this.secondaryEmploymentStatusLabel.Text = "Employment status:";
            // 
            // secondaryEmployerValueLabel
            // 
            this.secondaryEmployerValueLabel.AutoSize = true;
            this.secondaryEmployerValueLabel.Location = new System.Drawing.Point(170, 180);
            this.secondaryEmployerValueLabel.Name = "secondaryEmployerValueLabel";
            this.secondaryEmployerValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryEmployerValueLabel.TabIndex = 288;
            // 
            // secondaryEmployerLabel
            // 
            this.secondaryEmployerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEmployerLabel.Location = new System.Drawing.Point(3, 180);
            this.secondaryEmployerLabel.Name = "secondaryEmployerLabel";
            this.secondaryEmployerLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryEmployerLabel.TabIndex = 287;
            this.secondaryEmployerLabel.Text = "Employer:";
            // 
            // secondaryEmployerPhoneValueLabel
            // 
            this.secondaryEmployerPhoneValueLabel.AutoSize = true;
            this.secondaryEmployerPhoneValueLabel.Location = new System.Drawing.Point(170, 200);
            this.secondaryEmployerPhoneValueLabel.Name = "secondaryEmployerPhoneValueLabel";
            this.secondaryEmployerPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryEmployerPhoneValueLabel.TabIndex = 290;
            // 
            // secondaryEmployerPhoneLabel
            // 
            this.secondaryEmployerPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEmployerPhoneLabel.Location = new System.Drawing.Point(3, 200);
            this.secondaryEmployerPhoneLabel.Name = "secondaryEmployerPhoneLabel";
            this.secondaryEmployerPhoneLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryEmployerPhoneLabel.TabIndex = 289;
            this.secondaryEmployerPhoneLabel.Text = "Employer phone:";
            // 
            // secondaryNameLabel
            // 
            this.secondaryNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryNameLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryNameLabel.Name = "secondaryNameLabel";
            this.secondaryNameLabel.Size = new System.Drawing.Size(161, 16);
            this.secondaryNameLabel.TabIndex = 269;
            this.secondaryNameLabel.Text = "Name:";
            // 
            // secondaryInsuranceVerificationHeader
            // 
            this.secondaryInsuranceVerificationHeader.ColumnCount = 1;
            this.secondaryInsuranceVerificationHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.secondaryInsuranceVerificationHeader.Controls.Add(this.panel6, 0, 2);
            this.secondaryInsuranceVerificationHeader.Controls.Add(this.secondaryInsuranceVerificationLabel, 0, 1);
            this.secondaryInsuranceVerificationHeader.Location = new System.Drawing.Point(3, 3389);
            this.secondaryInsuranceVerificationHeader.Name = "secondaryInsuranceVerificationHeader";
            this.secondaryInsuranceVerificationHeader.RowCount = 3;
            this.secondaryInsuranceVerificationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.secondaryInsuranceVerificationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.secondaryInsuranceVerificationHeader.Size = new System.Drawing.Size(597, 30);
            this.secondaryInsuranceVerificationHeader.TabIndex = 561;
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Location = new System.Drawing.Point(3, 23);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(591, 1);
            this.panel6.TabIndex = 441;
            // 
            // secondaryInsuranceVerificationLabel
            // 
            this.secondaryInsuranceVerificationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsuranceVerificationLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryInsuranceVerificationLabel.Name = "secondaryInsuranceVerificationLabel";
            this.secondaryInsuranceVerificationLabel.Size = new System.Drawing.Size(304, 20);
            this.secondaryInsuranceVerificationLabel.TabIndex = 291;
            this.secondaryInsuranceVerificationLabel.Text = "Secondary Insurance – Verification";
            // 
            // secondaryInsuranceVerificationBody
            // 
            this.secondaryInsuranceVerificationBody.ColumnCount = 2;
            this.secondaryInsuranceVerificationBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.62896F));
            this.secondaryInsuranceVerificationBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.37104F));
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryBenefitsVerifiedValueLabel, 1, 0);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryBenefitsVerifiedLabel, 0, 0);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryInitiatedByValueLabel, 1, 1);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryInitiatedByLabel, 0, 1);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryDateValueLabel, 1, 2);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryDateLabel, 0, 2);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAuthorizationRequiredValueLabel, 1, 3);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAuthorizationRequiredLabel, 0, 3);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAuthorizationCompanyLabel, 0, 4);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAuthorizationCompanyValueLabel, 1, 4);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAuthorizationPhoneValueLabel, 1, 5);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryPromptExtValueLabel, 1, 6);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryPromptExtLabel, 0, 6);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAttorneyNameValueLabel, 1, 7);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAttorneyNameLabel, 0, 7);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAttorneyAddressValueLabel, 1, 8);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAttorneyAddressLabel, 0, 8);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAttorneyPhoneNumberValueLabel, 1, 9);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAttorneyPhoneNumberLabel, 0, 9);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAutoHomeInsuranceAgentNameValueLabel, 1, 10);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAutoHomeInsuranceAgentNameLabel, 0, 10);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAutoHomeInsuranceAgentAddressValueLabel, 1, 11);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAutoHomeInsuranceAgentAddressLabel, 0, 11);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel, 0, 12);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel, 1, 12);
            this.secondaryInsuranceVerificationBody.Controls.Add(this.secondaryAuthorizationPhoneLabel, 0, 5);
            this.secondaryInsuranceVerificationBody.Location = new System.Drawing.Point(3, 3425);
            this.secondaryInsuranceVerificationBody.Name = "secondaryInsuranceVerificationBody";
            this.secondaryInsuranceVerificationBody.RowCount = 13;
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceVerificationBody.Size = new System.Drawing.Size(660, 266);
            this.secondaryInsuranceVerificationBody.TabIndex = 562;
            // 
            // secondaryBenefitsVerifiedValueLabel
            // 
            this.secondaryBenefitsVerifiedValueLabel.AutoSize = true;
            this.secondaryBenefitsVerifiedValueLabel.Location = new System.Drawing.Point(277, 0);
            this.secondaryBenefitsVerifiedValueLabel.Name = "secondaryBenefitsVerifiedValueLabel";
            this.secondaryBenefitsVerifiedValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryBenefitsVerifiedValueLabel.TabIndex = 293;
            // 
            // secondaryBenefitsVerifiedLabel
            // 
            this.secondaryBenefitsVerifiedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBenefitsVerifiedLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryBenefitsVerifiedLabel.Name = "secondaryBenefitsVerifiedLabel";
            this.secondaryBenefitsVerifiedLabel.Size = new System.Drawing.Size(149, 16);
            this.secondaryBenefitsVerifiedLabel.TabIndex = 292;
            this.secondaryBenefitsVerifiedLabel.Text = "Benefits verified:";
            // 
            // secondaryInitiatedByValueLabel
            // 
            this.secondaryInitiatedByValueLabel.AutoSize = true;
            this.secondaryInitiatedByValueLabel.Location = new System.Drawing.Point(277, 20);
            this.secondaryInitiatedByValueLabel.Name = "secondaryInitiatedByValueLabel";
            this.secondaryInitiatedByValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryInitiatedByValueLabel.TabIndex = 295;
            // 
            // secondaryInitiatedByLabel
            // 
            this.secondaryInitiatedByLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInitiatedByLabel.Location = new System.Drawing.Point(3, 20);
            this.secondaryInitiatedByLabel.Name = "secondaryInitiatedByLabel";
            this.secondaryInitiatedByLabel.Size = new System.Drawing.Size(104, 16);
            this.secondaryInitiatedByLabel.TabIndex = 294;
            this.secondaryInitiatedByLabel.Text = "Initiated by:";
            // 
            // secondaryDateValueLabel
            // 
            this.secondaryDateValueLabel.AutoSize = true;
            this.secondaryDateValueLabel.Location = new System.Drawing.Point(277, 40);
            this.secondaryDateValueLabel.Name = "secondaryDateValueLabel";
            this.secondaryDateValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryDateValueLabel.TabIndex = 297;
            // 
            // secondaryDateLabel
            // 
            this.secondaryDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryDateLabel.Location = new System.Drawing.Point(3, 40);
            this.secondaryDateLabel.Name = "secondaryDateLabel";
            this.secondaryDateLabel.Size = new System.Drawing.Size(68, 16);
            this.secondaryDateLabel.TabIndex = 296;
            this.secondaryDateLabel.Text = "Date:";
            // 
            // secondaryAuthorizationRequiredValueLabel
            // 
            this.secondaryAuthorizationRequiredValueLabel.AutoSize = true;
            this.secondaryAuthorizationRequiredValueLabel.Location = new System.Drawing.Point(277, 60);
            this.secondaryAuthorizationRequiredValueLabel.Name = "secondaryAuthorizationRequiredValueLabel";
            this.secondaryAuthorizationRequiredValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAuthorizationRequiredValueLabel.TabIndex = 299;
            // 
            // secondaryAuthorizationRequiredLabel
            // 
            this.secondaryAuthorizationRequiredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationRequiredLabel.Location = new System.Drawing.Point(3, 60);
            this.secondaryAuthorizationRequiredLabel.Name = "secondaryAuthorizationRequiredLabel";
            this.secondaryAuthorizationRequiredLabel.Size = new System.Drawing.Size(156, 16);
            this.secondaryAuthorizationRequiredLabel.TabIndex = 298;
            this.secondaryAuthorizationRequiredLabel.Text = "Authorization required:";
            // 
            // secondaryAuthorizationCompanyLabel
            // 
            this.secondaryAuthorizationCompanyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationCompanyLabel.Location = new System.Drawing.Point(3, 80);
            this.secondaryAuthorizationCompanyLabel.Name = "secondaryAuthorizationCompanyLabel";
            this.secondaryAuthorizationCompanyLabel.Size = new System.Drawing.Size(168, 16);
            this.secondaryAuthorizationCompanyLabel.TabIndex = 300;
            this.secondaryAuthorizationCompanyLabel.Text = "Authorization company:";
            // 
            // secondaryAuthorizationCompanyValueLabel
            // 
            this.secondaryAuthorizationCompanyValueLabel.AutoSize = true;
            this.secondaryAuthorizationCompanyValueLabel.Location = new System.Drawing.Point(277, 80);
            this.secondaryAuthorizationCompanyValueLabel.Name = "secondaryAuthorizationCompanyValueLabel";
            this.secondaryAuthorizationCompanyValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAuthorizationCompanyValueLabel.TabIndex = 301;
            // 
            // secondaryAuthorizationPhoneValueLabel
            // 
            this.secondaryAuthorizationPhoneValueLabel.AutoSize = true;
            this.secondaryAuthorizationPhoneValueLabel.Location = new System.Drawing.Point(277, 100);
            this.secondaryAuthorizationPhoneValueLabel.Name = "secondaryAuthorizationPhoneValueLabel";
            this.secondaryAuthorizationPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAuthorizationPhoneValueLabel.TabIndex = 303;
            // 
            // secondaryPromptExtValueLabel
            // 
            this.secondaryPromptExtValueLabel.AutoSize = true;
            this.secondaryPromptExtValueLabel.Location = new System.Drawing.Point(277, 120);
            this.secondaryPromptExtValueLabel.Name = "secondaryPromptExtValueLabel";
            this.secondaryPromptExtValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryPromptExtValueLabel.TabIndex = 305;
            // 
            // secondaryPromptExtLabel
            // 
            this.secondaryPromptExtLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryPromptExtLabel.Location = new System.Drawing.Point(3, 120);
            this.secondaryPromptExtLabel.Name = "secondaryPromptExtLabel";
            this.secondaryPromptExtLabel.Size = new System.Drawing.Size(124, 16);
            this.secondaryPromptExtLabel.TabIndex = 304;
            this.secondaryPromptExtLabel.Text = "Prompt/ext:";
            // 
            // secondaryAttorneyNameValueLabel
            // 
            this.secondaryAttorneyNameValueLabel.AutoSize = true;
            this.secondaryAttorneyNameValueLabel.Location = new System.Drawing.Point(277, 140);
            this.secondaryAttorneyNameValueLabel.Name = "secondaryAttorneyNameValueLabel";
            this.secondaryAttorneyNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAttorneyNameValueLabel.TabIndex = 307;
            // 
            // secondaryAttorneyNameLabel
            // 
            this.secondaryAttorneyNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAttorneyNameLabel.Location = new System.Drawing.Point(3, 140);
            this.secondaryAttorneyNameLabel.Name = "secondaryAttorneyNameLabel";
            this.secondaryAttorneyNameLabel.Size = new System.Drawing.Size(132, 16);
            this.secondaryAttorneyNameLabel.TabIndex = 306;
            this.secondaryAttorneyNameLabel.Text = "Attorney name:";
            // 
            // secondaryAttorneyAddressValueLabel
            // 
            this.secondaryAttorneyAddressValueLabel.AutoSize = true;
            this.secondaryAttorneyAddressValueLabel.Location = new System.Drawing.Point(277, 160);
            this.secondaryAttorneyAddressValueLabel.Name = "secondaryAttorneyAddressValueLabel";
            this.secondaryAttorneyAddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAttorneyAddressValueLabel.TabIndex = 309;
            // 
            // secondaryAttorneyAddressLabel
            // 
            this.secondaryAttorneyAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAttorneyAddressLabel.Location = new System.Drawing.Point(3, 160);
            this.secondaryAttorneyAddressLabel.Name = "secondaryAttorneyAddressLabel";
            this.secondaryAttorneyAddressLabel.Size = new System.Drawing.Size(139, 16);
            this.secondaryAttorneyAddressLabel.TabIndex = 308;
            this.secondaryAttorneyAddressLabel.Text = "Attorney address:";
            // 
            // secondaryAttorneyPhoneNumberValueLabel
            // 
            this.secondaryAttorneyPhoneNumberValueLabel.AutoSize = true;
            this.secondaryAttorneyPhoneNumberValueLabel.Location = new System.Drawing.Point(277, 180);
            this.secondaryAttorneyPhoneNumberValueLabel.Name = "secondaryAttorneyPhoneNumberValueLabel";
            this.secondaryAttorneyPhoneNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAttorneyPhoneNumberValueLabel.TabIndex = 311;
            // 
            // secondaryAttorneyPhoneNumberLabel
            // 
            this.secondaryAttorneyPhoneNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAttorneyPhoneNumberLabel.Location = new System.Drawing.Point(3, 180);
            this.secondaryAttorneyPhoneNumberLabel.Name = "secondaryAttorneyPhoneNumberLabel";
            this.secondaryAttorneyPhoneNumberLabel.Size = new System.Drawing.Size(168, 16);
            this.secondaryAttorneyPhoneNumberLabel.TabIndex = 310;
            this.secondaryAttorneyPhoneNumberLabel.Text = "Attorney phone number:";
            // 
            // secondaryAutoHomeInsuranceAgentNameValueLabel
            // 
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.AutoSize = true;
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.Location = new System.Drawing.Point(277, 200);
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.Name = "secondaryAutoHomeInsuranceAgentNameValueLabel";
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.TabIndex = 313;
            // 
            // secondaryAutoHomeInsuranceAgentNameLabel
            // 
            this.secondaryAutoHomeInsuranceAgentNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAutoHomeInsuranceAgentNameLabel.Location = new System.Drawing.Point(3, 200);
            this.secondaryAutoHomeInsuranceAgentNameLabel.Name = "secondaryAutoHomeInsuranceAgentNameLabel";
            this.secondaryAutoHomeInsuranceAgentNameLabel.Size = new System.Drawing.Size(210, 16);
            this.secondaryAutoHomeInsuranceAgentNameLabel.TabIndex = 312;
            this.secondaryAutoHomeInsuranceAgentNameLabel.Text = "Auto/Home Insurance Agent name:";
            // 
            // secondaryAutoHomeInsuranceAgentAddressValueLabel
            // 
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.AutoSize = true;
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.Location = new System.Drawing.Point(277, 220);
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.Name = "secondaryAutoHomeInsuranceAgentAddressValueLabel";
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.TabIndex = 315;
            // 
            // secondaryAutoHomeInsuranceAgentAddressLabel
            // 
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Location = new System.Drawing.Point(3, 220);
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Name = "secondaryAutoHomeInsuranceAgentAddressLabel";
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Size = new System.Drawing.Size(222, 16);
            this.secondaryAutoHomeInsuranceAgentAddressLabel.TabIndex = 314;
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Text = "Auto/Home Insurance Agent address:";
            // 
            // secondaryAutoHomeInsuranceAgentPhoneNumberLabel
            // 
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Location = new System.Drawing.Point(3, 240);
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Name = "secondaryAutoHomeInsuranceAgentPhoneNumberLabel";
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Size = new System.Drawing.Size(268, 16);
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.TabIndex = 316;
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Text = "Auto/Home Insurance Agent phone number:";
            // 
            // secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel
            // 
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.AutoSize = true;
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Location = new System.Drawing.Point(277, 240);
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Name = "secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel";
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.TabIndex = 317;
            // 
            // secondaryAuthorizationPhoneLabel
            // 
            this.secondaryAuthorizationPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationPhoneLabel.Location = new System.Drawing.Point(3, 100);
            this.secondaryAuthorizationPhoneLabel.Name = "secondaryAuthorizationPhoneLabel";
            this.secondaryAuthorizationPhoneLabel.Size = new System.Drawing.Size(149, 16);
            this.secondaryAuthorizationPhoneLabel.TabIndex = 302;
            this.secondaryAuthorizationPhoneLabel.Text = "Authorization phone:";
            // 
            // secondaryInsuranceAuthorizationHeader
            // 
            this.secondaryInsuranceAuthorizationHeader.ColumnCount = 1;
            this.secondaryInsuranceAuthorizationHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.secondaryInsuranceAuthorizationHeader.Controls.Add(this.panel5, 0, 2);
            this.secondaryInsuranceAuthorizationHeader.Controls.Add(this.secondaryInsuranceAuthorizationLabel, 0, 1);
            this.secondaryInsuranceAuthorizationHeader.Location = new System.Drawing.Point(3, 3710);
            this.secondaryInsuranceAuthorizationHeader.Name = "secondaryInsuranceAuthorizationHeader";
            this.secondaryInsuranceAuthorizationHeader.RowCount = 3;
            this.secondaryInsuranceAuthorizationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.secondaryInsuranceAuthorizationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.secondaryInsuranceAuthorizationHeader.Size = new System.Drawing.Size(597, 33);
            this.secondaryInsuranceAuthorizationHeader.TabIndex = 559;
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Location = new System.Drawing.Point(3, 23);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(591, 1);
            this.panel5.TabIndex = 441;
            // 
            // secondaryInsuranceAuthorizationLabel
            // 
            this.secondaryInsuranceAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsuranceAuthorizationLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryInsuranceAuthorizationLabel.Name = "secondaryInsuranceAuthorizationLabel";
            this.secondaryInsuranceAuthorizationLabel.Size = new System.Drawing.Size(339, 20);
            this.secondaryInsuranceAuthorizationLabel.TabIndex = 496;
            this.secondaryInsuranceAuthorizationLabel.Text = "Secondary  Insurance - Authorization";
            // 
            // secondaryInsuranceAuthorizationBody
            // 
            this.secondaryInsuranceAuthorizationBody.ColumnCount = 2;
            this.secondaryInsuranceAuthorizationBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.93062F));
            this.secondaryInsuranceAuthorizationBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.06938F));
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryAuthorizationTrackingNumberValueLabel, 1, 0);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryAuthorizationNumberValueLabel, 1, 1);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryAuthorizationNumberLabel, 0, 1);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryAuthorizationCompanyRepresentativeNameValueLabel, 1, 2);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryAuthorizationCompanyRepresentativeNameLabel, 0, 2);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryDaysAuthorizedValueLabel, 1, 3);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryDaysAuthorizedLabel, 0, 3);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryServicesAuthorizedLabel, 0, 4);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryServicesAuthorizedValueLabel, 1, 4);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryEffectiveDateOfAuthorizationValueLabel, 1, 5);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryEffectiveDateOfAuthorizationLabel, 0, 5);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryExpirationDateOfAuthorizationValueLabel, 1, 6);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryExpirationDateOfAuthorizationLabel, 0, 6);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryAuthorizationStatusValueLabel, 1, 7);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryAuthorizationStatusLabel, 0, 7);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryRemarksValueLabel, 1, 8);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryRemarksLabel, 0, 8);
            this.secondaryInsuranceAuthorizationBody.Controls.Add(this.secondaryAuthorizationTrackingNumberLabel, 0, 0);
            this.secondaryInsuranceAuthorizationBody.Location = new System.Drawing.Point(3, 3749);
            this.secondaryInsuranceAuthorizationBody.Name = "secondaryInsuranceAuthorizationBody";
            this.secondaryInsuranceAuthorizationBody.RowCount = 9;
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.secondaryInsuranceAuthorizationBody.Size = new System.Drawing.Size(741, 188);
            this.secondaryInsuranceAuthorizationBody.TabIndex = 560;
            // 
            // secondaryAuthorizationTrackingNumberValueLabel
            // 
            this.secondaryAuthorizationTrackingNumberValueLabel.AutoSize = true;
            this.secondaryAuthorizationTrackingNumberValueLabel.Location = new System.Drawing.Point(313, 0);
            this.secondaryAuthorizationTrackingNumberValueLabel.Name = "secondaryAuthorizationTrackingNumberValueLabel";
            this.secondaryAuthorizationTrackingNumberValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAuthorizationTrackingNumberValueLabel.TabIndex = 506;
            // 
            // secondaryAuthorizationNumberValueLabel
            // 
            this.secondaryAuthorizationNumberValueLabel.Location = new System.Drawing.Point(313, 20);
            this.secondaryAuthorizationNumberValueLabel.Name = "secondaryAuthorizationNumberValueLabel";
            this.secondaryAuthorizationNumberValueLabel.Size = new System.Drawing.Size(200, 16);
            this.secondaryAuthorizationNumberValueLabel.TabIndex = 504;
            // 
            // secondaryAuthorizationNumberLabel
            // 
            this.secondaryAuthorizationNumberLabel.BackColor = System.Drawing.Color.White;
            this.secondaryAuthorizationNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationNumberLabel.Location = new System.Drawing.Point(3, 20);
            this.secondaryAuthorizationNumberLabel.Name = "secondaryAuthorizationNumberLabel";
            this.secondaryAuthorizationNumberLabel.Size = new System.Drawing.Size(164, 16);
            this.secondaryAuthorizationNumberLabel.TabIndex = 497;
            this.secondaryAuthorizationNumberLabel.Text = "Authorization Number:";
            // 
            // secondaryAuthorizationCompanyRepresentativeNameValueLabel
            // 
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.AutoSize = true;
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.Location = new System.Drawing.Point(313, 40);
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.Name = "secondaryAuthorizationCompanyRepresentativeNameValueLabel";
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.TabIndex = 507;
            // 
            // secondaryAuthorizationCompanyRepresentativeNameLabel
            // 
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Location = new System.Drawing.Point(3, 40);
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Name = "secondaryAuthorizationCompanyRepresentativeNameLabel";
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Size = new System.Drawing.Size(304, 16);
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.TabIndex = 500;
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Text = "Authorization Company Representative Name:";
            // 
            // secondaryDaysAuthorizedValueLabel
            // 
            this.secondaryDaysAuthorizedValueLabel.AutoSize = true;
            this.secondaryDaysAuthorizedValueLabel.Location = new System.Drawing.Point(313, 60);
            this.secondaryDaysAuthorizedValueLabel.Name = "secondaryDaysAuthorizedValueLabel";
            this.secondaryDaysAuthorizedValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryDaysAuthorizedValueLabel.TabIndex = 505;
            // 
            // secondaryDaysAuthorizedLabel
            // 
            this.secondaryDaysAuthorizedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryDaysAuthorizedLabel.Location = new System.Drawing.Point(3, 60);
            this.secondaryDaysAuthorizedLabel.Name = "secondaryDaysAuthorizedLabel";
            this.secondaryDaysAuthorizedLabel.Size = new System.Drawing.Size(164, 16);
            this.secondaryDaysAuthorizedLabel.TabIndex = 498;
            this.secondaryDaysAuthorizedLabel.Text = "Days Authorized:";
            // 
            // secondaryServicesAuthorizedLabel
            // 
            this.secondaryServicesAuthorizedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryServicesAuthorizedLabel.Location = new System.Drawing.Point(3, 80);
            this.secondaryServicesAuthorizedLabel.Name = "secondaryServicesAuthorizedLabel";
            this.secondaryServicesAuthorizedLabel.Size = new System.Drawing.Size(164, 16);
            this.secondaryServicesAuthorizedLabel.TabIndex = 514;
            this.secondaryServicesAuthorizedLabel.Text = "Services Authorized:";
            // 
            // secondaryServicesAuthorizedValueLabel
            // 
            this.secondaryServicesAuthorizedValueLabel.AutoSize = true;
            this.secondaryServicesAuthorizedValueLabel.Location = new System.Drawing.Point(313, 80);
            this.secondaryServicesAuthorizedValueLabel.Name = "secondaryServicesAuthorizedValueLabel";
            this.secondaryServicesAuthorizedValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryServicesAuthorizedValueLabel.TabIndex = 515;
            // 
            // secondaryEffectiveDateOfAuthorizationValueLabel
            // 
            this.secondaryEffectiveDateOfAuthorizationValueLabel.AutoSize = true;
            this.secondaryEffectiveDateOfAuthorizationValueLabel.Location = new System.Drawing.Point(313, 100);
            this.secondaryEffectiveDateOfAuthorizationValueLabel.Name = "secondaryEffectiveDateOfAuthorizationValueLabel";
            this.secondaryEffectiveDateOfAuthorizationValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryEffectiveDateOfAuthorizationValueLabel.TabIndex = 508;
            // 
            // secondaryEffectiveDateOfAuthorizationLabel
            // 
            this.secondaryEffectiveDateOfAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEffectiveDateOfAuthorizationLabel.Location = new System.Drawing.Point(3, 100);
            this.secondaryEffectiveDateOfAuthorizationLabel.Name = "secondaryEffectiveDateOfAuthorizationLabel";
            this.secondaryEffectiveDateOfAuthorizationLabel.Size = new System.Drawing.Size(193, 16);
            this.secondaryEffectiveDateOfAuthorizationLabel.TabIndex = 501;
            this.secondaryEffectiveDateOfAuthorizationLabel.Text = "Effective Date of Authorization:";
            // 
            // secondaryExpirationDateOfAuthorizationValueLabel
            // 
            this.secondaryExpirationDateOfAuthorizationValueLabel.AutoSize = true;
            this.secondaryExpirationDateOfAuthorizationValueLabel.Location = new System.Drawing.Point(313, 120);
            this.secondaryExpirationDateOfAuthorizationValueLabel.Name = "secondaryExpirationDateOfAuthorizationValueLabel";
            this.secondaryExpirationDateOfAuthorizationValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryExpirationDateOfAuthorizationValueLabel.TabIndex = 509;
            // 
            // secondaryExpirationDateOfAuthorizationLabel
            // 
            this.secondaryExpirationDateOfAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryExpirationDateOfAuthorizationLabel.Location = new System.Drawing.Point(3, 120);
            this.secondaryExpirationDateOfAuthorizationLabel.Name = "secondaryExpirationDateOfAuthorizationLabel";
            this.secondaryExpirationDateOfAuthorizationLabel.Size = new System.Drawing.Size(213, 16);
            this.secondaryExpirationDateOfAuthorizationLabel.TabIndex = 502;
            this.secondaryExpirationDateOfAuthorizationLabel.Text = "Expiration Date of Authorization:";
            // 
            // secondaryAuthorizationStatusValueLabel
            // 
            this.secondaryAuthorizationStatusValueLabel.AutoSize = true;
            this.secondaryAuthorizationStatusValueLabel.Location = new System.Drawing.Point(313, 140);
            this.secondaryAuthorizationStatusValueLabel.Name = "secondaryAuthorizationStatusValueLabel";
            this.secondaryAuthorizationStatusValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryAuthorizationStatusValueLabel.TabIndex = 510;
            // 
            // secondaryAuthorizationStatusLabel
            // 
            this.secondaryAuthorizationStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationStatusLabel.Location = new System.Drawing.Point(3, 140);
            this.secondaryAuthorizationStatusLabel.Name = "secondaryAuthorizationStatusLabel";
            this.secondaryAuthorizationStatusLabel.Size = new System.Drawing.Size(193, 16);
            this.secondaryAuthorizationStatusLabel.TabIndex = 503;
            this.secondaryAuthorizationStatusLabel.Text = "Authorization Status:";
            // 
            // secondaryRemarksValueLabel
            // 
            this.secondaryRemarksValueLabel.AutoSize = true;
            this.secondaryRemarksValueLabel.Location = new System.Drawing.Point(313, 160);
            this.secondaryRemarksValueLabel.Name = "secondaryRemarksValueLabel";
            this.secondaryRemarksValueLabel.Size = new System.Drawing.Size(0, 17);
            this.secondaryRemarksValueLabel.TabIndex = 513;
            // 
            // secondaryRemarksLabel
            // 
            this.secondaryRemarksLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryRemarksLabel.Location = new System.Drawing.Point(3, 160);
            this.secondaryRemarksLabel.Name = "secondaryRemarksLabel";
            this.secondaryRemarksLabel.Size = new System.Drawing.Size(104, 16);
            this.secondaryRemarksLabel.TabIndex = 512;
            this.secondaryRemarksLabel.Text = "Remarks:";
            // 
            // secondaryAuthorizationTrackingNumberLabel
            // 
            this.secondaryAuthorizationTrackingNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationTrackingNumberLabel.Location = new System.Drawing.Point(3, 0);
            this.secondaryAuthorizationTrackingNumberLabel.Name = "secondaryAuthorizationTrackingNumberLabel";
            this.secondaryAuthorizationTrackingNumberLabel.Size = new System.Drawing.Size(184, 16);
            this.secondaryAuthorizationTrackingNumberLabel.TabIndex = 499;
            this.secondaryAuthorizationTrackingNumberLabel.Text = "Tracking Number:";
            // 
            // guarantorHeader
            // 
            this.guarantorHeader.ColumnCount = 1;
            this.guarantorHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.guarantorHeader.Controls.Add(this.panel4, 0, 2);
            this.guarantorHeader.Controls.Add(this.guarantorLabel, 0, 1);
            this.guarantorHeader.Location = new System.Drawing.Point(3, 3965);
            this.guarantorHeader.Name = "guarantorHeader";
            this.guarantorHeader.RowCount = 3;
            this.guarantorHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.guarantorHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.guarantorHeader.Size = new System.Drawing.Size(597, 37);
            this.guarantorHeader.TabIndex = 557;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Location = new System.Drawing.Point(3, 23);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(591, 1);
            this.panel4.TabIndex = 441;
            // 
            // guarantorLabel
            // 
            this.guarantorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorLabel.Location = new System.Drawing.Point(3, 0);
            this.guarantorLabel.Name = "guarantorLabel";
            this.guarantorLabel.Size = new System.Drawing.Size(88, 20);
            this.guarantorLabel.TabIndex = 318;
            this.guarantorLabel.Text = "Guarantor";
            // 
            // guarantorBody
            // 
            this.guarantorBody.ColumnCount = 2;
            this.guarantorBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.17588F));
            this.guarantorBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 350.8241F));
            this.guarantorBody.Controls.Add(this.guarantorDOBValueLabel, 1, 7);
            this.guarantorBody.Controls.Add(this.guarantorDOBLabel, 0, 7);
            this.guarantorBody.Controls.Add(this.guarantorCellPhoneConsentLabel, 0, 8);
            this.guarantorBody.Controls.Add(this.thePatientIsTheGuarantorsLabel, 0, 1);
            this.guarantorBody.Controls.Add(this.thePatientIsTheGuarantorsValueLabel, 1, 1);
            this.guarantorBody.Controls.Add(this.guarantorNameLabel, 0, 0);
            this.guarantorBody.Controls.Add(this.guarantorNameValueLabel, 1, 0);
            this.guarantorBody.Controls.Add(this.guarantorSSNLabel, 0, 2);
            this.guarantorBody.Controls.Add(this.guarantorSSNValueLabel, 1, 2);
            this.guarantorBody.Controls.Add(this.usDriverLicenseLabel, 0, 3);
            this.guarantorBody.Controls.Add(this.usDriverLicenseValueLabel, 1, 3);
            this.guarantorBody.Controls.Add(this.guarantorAddressLabel, 0, 4);
            this.guarantorBody.Controls.Add(this.guarantorAddressValueLabel, 1, 4);
            this.guarantorBody.Controls.Add(this.guarantorContactPhoneValueLabel, 1, 5);
            this.guarantorBody.Controls.Add(this.guarantorContactPhoneLabel, 0, 5);
            this.guarantorBody.Controls.Add(this.guarantorContactCellValueLabel, 1, 6);
            this.guarantorBody.Controls.Add(this.guarantorContactCellLabel, 0, 6);
            this.guarantorBody.Controls.Add(this.guarantorCellPhoneConsentValueLabel, 1, 8);
            this.guarantorBody.Location = new System.Drawing.Point(3, 4008);
            this.guarantorBody.Name = "guarantorBody";
            this.guarantorBody.RowCount = 9;
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.guarantorBody.Size = new System.Drawing.Size(894, 183);
            this.guarantorBody.TabIndex = 558;
            // 
            // guarantorDOBValueLabel
            // 
            this.guarantorDOBValueLabel.AutoSize = true;
            this.guarantorDOBValueLabel.Location = new System.Drawing.Point(169, 140);
            this.guarantorDOBValueLabel.Name = "guarantorDOBValueLabel";
            this.guarantorDOBValueLabel.Size = new System.Drawing.Size(0, 17);
            this.guarantorDOBValueLabel.TabIndex = 338;
            // 
            // guarantorDOBLabel
            // 
            this.guarantorDOBLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorDOBLabel.Location = new System.Drawing.Point(3, 140);
            this.guarantorDOBLabel.Name = "guarantorDOBLabel";
            this.guarantorDOBLabel.Size = new System.Drawing.Size(87, 16);
            this.guarantorDOBLabel.TabIndex = 337;
            this.guarantorDOBLabel.Text = "DOB:";
            // 
            // guarantorCellPhoneConsentLabel
            // 
            this.guarantorCellPhoneConsentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorCellPhoneConsentLabel.Location = new System.Drawing.Point(3, 160);
            this.guarantorCellPhoneConsentLabel.Name = "guarantorCellPhoneConsentLabel";
            this.guarantorCellPhoneConsentLabel.Size = new System.Drawing.Size(135, 16);
            this.guarantorCellPhoneConsentLabel.TabIndex = 335;
            this.guarantorCellPhoneConsentLabel.Text = "Cell Phone Consent:";
            // 
            // thePatientIsTheGuarantorsLabel
            // 
            this.thePatientIsTheGuarantorsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.thePatientIsTheGuarantorsLabel.Location = new System.Drawing.Point(3, 20);
            this.thePatientIsTheGuarantorsLabel.Name = "thePatientIsTheGuarantorsLabel";
            this.thePatientIsTheGuarantorsLabel.Size = new System.Drawing.Size(160, 16);
            this.thePatientIsTheGuarantorsLabel.TabIndex = 321;
            this.thePatientIsTheGuarantorsLabel.Text = "The Patient is the Guarantor’s:";
            // 
            // thePatientIsTheGuarantorsValueLabel
            // 
            this.thePatientIsTheGuarantorsValueLabel.AutoSize = true;
            this.thePatientIsTheGuarantorsValueLabel.Location = new System.Drawing.Point(169, 20);
            this.thePatientIsTheGuarantorsValueLabel.Name = "thePatientIsTheGuarantorsValueLabel";
            this.thePatientIsTheGuarantorsValueLabel.Size = new System.Drawing.Size(0, 17);
            this.thePatientIsTheGuarantorsValueLabel.TabIndex = 322;
            // 
            // guarantorNameLabel
            // 
            this.guarantorNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorNameLabel.Location = new System.Drawing.Point(3, 0);
            this.guarantorNameLabel.Name = "guarantorNameLabel";
            this.guarantorNameLabel.Size = new System.Drawing.Size(100, 16);
            this.guarantorNameLabel.TabIndex = 319;
            this.guarantorNameLabel.Text = "Name:";
            // 
            // guarantorNameValueLabel
            // 
            this.guarantorNameValueLabel.AutoSize = true;
            this.guarantorNameValueLabel.Location = new System.Drawing.Point(169, 0);
            this.guarantorNameValueLabel.Name = "guarantorNameValueLabel";
            this.guarantorNameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.guarantorNameValueLabel.TabIndex = 320;
            // 
            // guarantorSSNLabel
            // 
            this.guarantorSSNLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorSSNLabel.Location = new System.Drawing.Point(3, 40);
            this.guarantorSSNLabel.Name = "guarantorSSNLabel";
            this.guarantorSSNLabel.Size = new System.Drawing.Size(104, 16);
            this.guarantorSSNLabel.TabIndex = 325;
            this.guarantorSSNLabel.Text = "SSN:";
            // 
            // guarantorSSNValueLabel
            // 
            this.guarantorSSNValueLabel.AutoSize = true;
            this.guarantorSSNValueLabel.Location = new System.Drawing.Point(169, 40);
            this.guarantorSSNValueLabel.Name = "guarantorSSNValueLabel";
            this.guarantorSSNValueLabel.Size = new System.Drawing.Size(0, 17);
            this.guarantorSSNValueLabel.TabIndex = 326;
            // 
            // usDriverLicenseLabel
            // 
            this.usDriverLicenseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usDriverLicenseLabel.Location = new System.Drawing.Point(3, 60);
            this.usDriverLicenseLabel.Name = "usDriverLicenseLabel";
            this.usDriverLicenseLabel.Size = new System.Drawing.Size(160, 16);
            this.usDriverLicenseLabel.TabIndex = 327;
            this.usDriverLicenseLabel.Text = "U.S. driver\'s license:";
            // 
            // usDriverLicenseValueLabel
            // 
            this.usDriverLicenseValueLabel.AutoSize = true;
            this.usDriverLicenseValueLabel.Location = new System.Drawing.Point(169, 60);
            this.usDriverLicenseValueLabel.Name = "usDriverLicenseValueLabel";
            this.usDriverLicenseValueLabel.Size = new System.Drawing.Size(0, 17);
            this.usDriverLicenseValueLabel.TabIndex = 328;
            // 
            // guarantorAddressLabel
            // 
            this.guarantorAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorAddressLabel.Location = new System.Drawing.Point(3, 80);
            this.guarantorAddressLabel.Name = "guarantorAddressLabel";
            this.guarantorAddressLabel.Size = new System.Drawing.Size(91, 16);
            this.guarantorAddressLabel.TabIndex = 329;
            this.guarantorAddressLabel.Text = "Address:";
            // 
            // guarantorAddressValueLabel
            // 
            this.guarantorAddressValueLabel.AutoSize = true;
            this.guarantorAddressValueLabel.Location = new System.Drawing.Point(169, 80);
            this.guarantorAddressValueLabel.Name = "guarantorAddressValueLabel";
            this.guarantorAddressValueLabel.Size = new System.Drawing.Size(193, 17);
            this.guarantorAddressValueLabel.TabIndex = 330;
            this.guarantorAddressValueLabel.Text = "guarantorAddressValueLabel";
            // 
            // guarantorContactPhoneValueLabel
            // 
            this.guarantorContactPhoneValueLabel.AutoSize = true;
            this.guarantorContactPhoneValueLabel.Location = new System.Drawing.Point(169, 100);
            this.guarantorContactPhoneValueLabel.Name = "guarantorContactPhoneValueLabel";
            this.guarantorContactPhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.guarantorContactPhoneValueLabel.TabIndex = 332;
            // 
            // guarantorContactPhoneLabel
            // 
            this.guarantorContactPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorContactPhoneLabel.Location = new System.Drawing.Point(3, 100);
            this.guarantorContactPhoneLabel.Name = "guarantorContactPhoneLabel";
            this.guarantorContactPhoneLabel.Size = new System.Drawing.Size(100, 16);
            this.guarantorContactPhoneLabel.TabIndex = 331;
            this.guarantorContactPhoneLabel.Text = "Contact Phone:";
            // 
            // guarantorContactCellValueLabel
            // 
            this.guarantorContactCellValueLabel.AutoSize = true;
            this.guarantorContactCellValueLabel.Location = new System.Drawing.Point(169, 120);
            this.guarantorContactCellValueLabel.Name = "guarantorContactCellValueLabel";
            this.guarantorContactCellValueLabel.Size = new System.Drawing.Size(0, 17);
            this.guarantorContactCellValueLabel.TabIndex = 334;
            // 
            // guarantorContactCellLabel
            // 
            this.guarantorContactCellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorContactCellLabel.Location = new System.Drawing.Point(3, 120);
            this.guarantorContactCellLabel.Name = "guarantorContactCellLabel";
            this.guarantorContactCellLabel.Size = new System.Drawing.Size(87, 16);
            this.guarantorContactCellLabel.TabIndex = 333;
            this.guarantorContactCellLabel.Text = "Contact Cell:";
            // 
            // guarantorCellPhoneConsentValueLabel
            // 
            this.guarantorCellPhoneConsentValueLabel.AutoSize = true;
            this.guarantorCellPhoneConsentValueLabel.Location = new System.Drawing.Point(169, 160);
            this.guarantorCellPhoneConsentValueLabel.Name = "guarantorCellPhoneConsentValueLabel";
            this.guarantorCellPhoneConsentValueLabel.Size = new System.Drawing.Size(0, 17);
            this.guarantorCellPhoneConsentValueLabel.TabIndex = 336;
            // 
            // paymentBody
            // 
            this.paymentBody.ColumnCount = 2;
            this.paymentBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.81548F));
            this.paymentBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.18452F));
            this.paymentBody.Controls.Add(this.totalCurrentAmountDueValueLabel, 1, 0);
            this.paymentBody.Controls.Add(this.monthlyDueDateLabel, 0, 4);
            this.paymentBody.Controls.Add(this.totalPaymentsCollectedForCurrentAccountLabel, 0, 1);
            this.paymentBody.Controls.Add(this.totalPaymentsCollectedForCurrentAccountValueLabel, 1, 1);
            this.paymentBody.Controls.Add(this.numberOfMonthlyPaymentsLabel, 0, 2);
            this.paymentBody.Controls.Add(this.numberOfMonthlyPaymentsValueLabel, 1, 2);
            this.paymentBody.Controls.Add(this.monthlyPaymentLabel, 0, 3);
            this.paymentBody.Controls.Add(this.monthlyPaymentValueLabel, 1, 3);
            this.paymentBody.Controls.Add(this.monthlyDueDateValueLabel, 1, 4);
            this.paymentBody.Controls.Add(this.totalCurrentAmountDueLabel, 0, 0);
            this.paymentBody.Location = new System.Drawing.Point(3, 4779);
            this.paymentBody.Name = "paymentBody";
            this.paymentBody.RowCount = 5;
            this.paymentBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.paymentBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.paymentBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.paymentBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.paymentBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.paymentBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.paymentBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.paymentBody.Size = new System.Drawing.Size(660, 98);
            this.paymentBody.TabIndex = 553;
            // 
            // totalCurrentAmountDueValueLabel
            // 
            this.totalCurrentAmountDueValueLabel.AutoSize = true;
            this.totalCurrentAmountDueValueLabel.Location = new System.Drawing.Point(278, 0);
            this.totalCurrentAmountDueValueLabel.Name = "totalCurrentAmountDueValueLabel";
            this.totalCurrentAmountDueValueLabel.Size = new System.Drawing.Size(0, 17);
            this.totalCurrentAmountDueValueLabel.TabIndex = 396;
            // 
            // monthlyDueDateLabel
            // 
            this.monthlyDueDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthlyDueDateLabel.Location = new System.Drawing.Point(3, 80);
            this.monthlyDueDateLabel.Name = "monthlyDueDateLabel";
            this.monthlyDueDateLabel.Size = new System.Drawing.Size(125, 16);
            this.monthlyDueDateLabel.TabIndex = 403;
            this.monthlyDueDateLabel.Text = "Monthly Due Date:";
            // 
            // totalPaymentsCollectedForCurrentAccountLabel
            // 
            this.totalPaymentsCollectedForCurrentAccountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalPaymentsCollectedForCurrentAccountLabel.Location = new System.Drawing.Point(3, 21);
            this.totalPaymentsCollectedForCurrentAccountLabel.Name = "totalPaymentsCollectedForCurrentAccountLabel";
            this.totalPaymentsCollectedForCurrentAccountLabel.Size = new System.Drawing.Size(269, 16);
            this.totalPaymentsCollectedForCurrentAccountLabel.TabIndex = 397;
            this.totalPaymentsCollectedForCurrentAccountLabel.Text = "Total payments collected for current account:";
            // 
            // totalPaymentsCollectedForCurrentAccountValueLabel
            // 
            this.totalPaymentsCollectedForCurrentAccountValueLabel.AutoSize = true;
            this.totalPaymentsCollectedForCurrentAccountValueLabel.Location = new System.Drawing.Point(278, 21);
            this.totalPaymentsCollectedForCurrentAccountValueLabel.Name = "totalPaymentsCollectedForCurrentAccountValueLabel";
            this.totalPaymentsCollectedForCurrentAccountValueLabel.Size = new System.Drawing.Size(0, 17);
            this.totalPaymentsCollectedForCurrentAccountValueLabel.TabIndex = 398;
            // 
            // numberOfMonthlyPaymentsLabel
            // 
            this.numberOfMonthlyPaymentsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberOfMonthlyPaymentsLabel.Location = new System.Drawing.Point(3, 40);
            this.numberOfMonthlyPaymentsLabel.Name = "numberOfMonthlyPaymentsLabel";
            this.numberOfMonthlyPaymentsLabel.Size = new System.Drawing.Size(181, 16);
            this.numberOfMonthlyPaymentsLabel.TabIndex = 399;
            this.numberOfMonthlyPaymentsLabel.Text = "Number of monthly payments:";
            // 
            // numberOfMonthlyPaymentsValueLabel
            // 
            this.numberOfMonthlyPaymentsValueLabel.AutoSize = true;
            this.numberOfMonthlyPaymentsValueLabel.Location = new System.Drawing.Point(278, 40);
            this.numberOfMonthlyPaymentsValueLabel.Name = "numberOfMonthlyPaymentsValueLabel";
            this.numberOfMonthlyPaymentsValueLabel.Size = new System.Drawing.Size(0, 17);
            this.numberOfMonthlyPaymentsValueLabel.TabIndex = 400;
            // 
            // monthlyPaymentLabel
            // 
            this.monthlyPaymentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthlyPaymentLabel.Location = new System.Drawing.Point(3, 60);
            this.monthlyPaymentLabel.Name = "monthlyPaymentLabel";
            this.monthlyPaymentLabel.Size = new System.Drawing.Size(117, 16);
            this.monthlyPaymentLabel.TabIndex = 401;
            this.monthlyPaymentLabel.Text = "Monthly payment:";
            // 
            // monthlyPaymentValueLabel
            // 
            this.monthlyPaymentValueLabel.AutoSize = true;
            this.monthlyPaymentValueLabel.Location = new System.Drawing.Point(278, 60);
            this.monthlyPaymentValueLabel.Name = "monthlyPaymentValueLabel";
            this.monthlyPaymentValueLabel.Size = new System.Drawing.Size(0, 17);
            this.monthlyPaymentValueLabel.TabIndex = 402;
            // 
            // monthlyDueDateValueLabel
            // 
            this.monthlyDueDateValueLabel.AutoSize = true;
            this.monthlyDueDateValueLabel.Location = new System.Drawing.Point(278, 80);
            this.monthlyDueDateValueLabel.Name = "monthlyDueDateValueLabel";
            this.monthlyDueDateValueLabel.Size = new System.Drawing.Size(46, 17);
            this.monthlyDueDateValueLabel.TabIndex = 404;
            this.monthlyDueDateValueLabel.Text = "label2";
            // 
            // totalCurrentAmountDueLabel
            // 
            this.totalCurrentAmountDueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalCurrentAmountDueLabel.Location = new System.Drawing.Point(3, 0);
            this.totalCurrentAmountDueLabel.Name = "totalCurrentAmountDueLabel";
            this.totalCurrentAmountDueLabel.Size = new System.Drawing.Size(204, 16);
            this.totalCurrentAmountDueLabel.TabIndex = 395;
            this.totalCurrentAmountDueLabel.Text = "Total current amount due:";
            // 
            // PaymentHeader
            // 
            this.PaymentHeader.ColumnCount = 1;
            this.PaymentHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.PaymentHeader.Controls.Add(this.panel2, 0, 2);
            this.PaymentHeader.Controls.Add(this.paymentLabel, 0, 1);
            this.PaymentHeader.Location = new System.Drawing.Point(3, 4746);
            this.PaymentHeader.Name = "PaymentHeader";
            this.PaymentHeader.RowCount = 3;
            this.PaymentHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.PaymentHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.PaymentHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.PaymentHeader.Size = new System.Drawing.Size(597, 27);
            this.PaymentHeader.TabIndex = 554;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(3, 23);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(591, 1);
            this.panel2.TabIndex = 441;
            // 
            // paymentLabel
            // 
            this.paymentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.paymentLabel.Location = new System.Drawing.Point(3, 0);
            this.paymentLabel.Name = "paymentLabel";
            this.paymentLabel.Size = new System.Drawing.Size(83, 20);
            this.paymentLabel.TabIndex = 394;
            this.paymentLabel.Text = "Payment";
            // 
            // billingHeader
            // 
            this.billingHeader.ColumnCount = 1;
            this.billingHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.billingHeader.Controls.Add(this.panel18, 0, 2);
            this.billingHeader.Controls.Add(this.billingLabel, 0, 1);
            this.billingHeader.Location = new System.Drawing.Point(3, 4208);
            this.billingHeader.Name = "billingHeader";
            this.billingHeader.RowCount = 3;
            this.billingHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.billingHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.billingHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.billingHeader.Size = new System.Drawing.Size(597, 28);
            this.billingHeader.TabIndex = 556;
            // 
            // panel18
            // 
            this.panel18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel18.Location = new System.Drawing.Point(3, 23);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(591, 1);
            this.panel18.TabIndex = 441;
            // 
            // billingLabel
            // 
            this.billingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.billingLabel.Location = new System.Drawing.Point(3, 0);
            this.billingLabel.Name = "billingLabel";
            this.billingLabel.Size = new System.Drawing.Size(56, 20);
            this.billingLabel.TabIndex = 345;
            this.billingLabel.Text = "Billing";
            // 
            // LeftWithoutFinancialClearanceValueLabel
            // 
            this.LeftWithoutFinancialClearanceValueLabel.AutoSize = true;
            this.LeftWithoutFinancialClearanceValueLabel.Location = new System.Drawing.Point(210, 2108);
            this.LeftWithoutFinancialClearanceValueLabel.Name = "LeftWithoutFinancialClearanceValueLabel";
            this.LeftWithoutFinancialClearanceValueLabel.Size = new System.Drawing.Size(0, 17);
            this.LeftWithoutFinancialClearanceValueLabel.TabIndex = 544;
            // 
            // LeftWithoutBeingSeenValueLabel
            // 
            this.LeftWithoutBeingSeenValueLabel.AutoSize = true;
            this.LeftWithoutBeingSeenValueLabel.Location = new System.Drawing.Point(167, 2085);
            this.LeftWithoutBeingSeenValueLabel.Name = "LeftWithoutBeingSeenValueLabel";
            this.LeftWithoutBeingSeenValueLabel.Size = new System.Drawing.Size(0, 17);
            this.LeftWithoutBeingSeenValueLabel.TabIndex = 542;
            // 
            // RCRPValueLabel
            // 
            this.RCRPValueLabel.AutoSize = true;
            this.RCRPValueLabel.Location = new System.Drawing.Point(69, 2042);
            this.RCRPValueLabel.Name = "RCRPValueLabel";
            this.RCRPValueLabel.Size = new System.Drawing.Size(0, 17);
            this.RCRPValueLabel.TabIndex = 540;
            // 
            // LeftOrStayedValueLabel
            // 
            this.LeftOrStayedValueLabel.AutoSize = true;
            this.LeftOrStayedValueLabel.Location = new System.Drawing.Point(112, 2063);
            this.LeftOrStayedValueLabel.Name = "LeftOrStayedValueLabel";
            this.LeftOrStayedValueLabel.Size = new System.Drawing.Size(0, 17);
            this.LeftOrStayedValueLabel.TabIndex = 538;
            // 
            // emergencyContact2PhoneValueLabel
            // 
            this.emergencyContact2PhoneValueLabel.AutoSize = true;
            this.emergencyContact2PhoneValueLabel.Location = new System.Drawing.Point(754, 1011);
            this.emergencyContact2PhoneValueLabel.Name = "emergencyContact2PhoneValueLabel";
            this.emergencyContact2PhoneValueLabel.Size = new System.Drawing.Size(0, 17);
            this.emergencyContact2PhoneValueLabel.TabIndex = 419;
            // 
            // emergencyContact2AddressValueLabel
            // 
            this.emergencyContact2AddressValueLabel.AutoSize = true;
            this.emergencyContact2AddressValueLabel.Location = new System.Drawing.Point(701, 985);
            this.emergencyContact2AddressValueLabel.Name = "emergencyContact2AddressValueLabel";
            this.emergencyContact2AddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.emergencyContact2AddressValueLabel.TabIndex = 417;
            // 
            // emergencyContact2IsThePatientsValueLabel
            // 
            this.emergencyContact2IsThePatientsValueLabel.AutoSize = true;
            this.emergencyContact2IsThePatientsValueLabel.Location = new System.Drawing.Point(827, 959);
            this.emergencyContact2IsThePatientsValueLabel.Name = "emergencyContact2IsThePatientsValueLabel";
            this.emergencyContact2IsThePatientsValueLabel.Size = new System.Drawing.Size(0, 17);
            this.emergencyContact2IsThePatientsValueLabel.TabIndex = 415;
            // 
            // emergencyContact2NameValueLabel
            // 
            this.emergencyContact2NameValueLabel.AutoSize = true;
            this.emergencyContact2NameValueLabel.Location = new System.Drawing.Point(818, 933);
            this.emergencyContact2NameValueLabel.Name = "emergencyContact2NameValueLabel";
            this.emergencyContact2NameValueLabel.Size = new System.Drawing.Size(0, 17);
            this.emergencyContact2NameValueLabel.TabIndex = 413;
            // 
            // emergencyContact1AddressValueLabel
            // 
            this.emergencyContact1AddressValueLabel.AutoSize = true;
            this.emergencyContact1AddressValueLabel.Location = new System.Drawing.Point(701, 881);
            this.emergencyContact1AddressValueLabel.Name = "emergencyContact1AddressValueLabel";
            this.emergencyContact1AddressValueLabel.Size = new System.Drawing.Size(0, 17);
            this.emergencyContact1AddressValueLabel.TabIndex = 409;
            // 
            // emergencyContact2PhoneLabel
            // 
            this.emergencyContact2PhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact2PhoneLabel.Location = new System.Drawing.Point(644, 1011);
            this.emergencyContact2PhoneLabel.Name = "emergencyContact2PhoneLabel";
            this.emergencyContact2PhoneLabel.Size = new System.Drawing.Size(104, 16);
            this.emergencyContact2PhoneLabel.TabIndex = 418;
            this.emergencyContact2PhoneLabel.Text = "Contact phone:";
            // 
            // emergencyContact2AddressLabel
            // 
            this.emergencyContact2AddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact2AddressLabel.Location = new System.Drawing.Point(644, 985);
            this.emergencyContact2AddressLabel.Name = "emergencyContact2AddressLabel";
            this.emergencyContact2AddressLabel.Size = new System.Drawing.Size(51, 16);
            this.emergencyContact2AddressLabel.TabIndex = 416;
            this.emergencyContact2AddressLabel.Text = "Address:";
            // 
            // emergencyContact1AddressLabel
            // 
            this.emergencyContact1AddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact1AddressLabel.Location = new System.Drawing.Point(644, 881);
            this.emergencyContact1AddressLabel.Name = "emergencyContact1AddressLabel";
            this.emergencyContact1AddressLabel.Size = new System.Drawing.Size(51, 16);
            this.emergencyContact1AddressLabel.TabIndex = 408;
            this.emergencyContact1AddressLabel.Text = "Address:";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Code";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn1.HeaderText = "Clinical Research Code";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 150;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Description";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewTextBoxColumn2.HeaderText = "Description";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 200;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 200;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "ResearchSponsor";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewTextBoxColumn3.HeaderText = "Clinical Research Sponsor";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 200;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn3.Width = 200;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "ResearchStudy.Code";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridViewTextBoxColumn4.HeaderText = "Research Study Code";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 150;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "ResearchStudy.Description";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridViewTextBoxColumn5.HeaderText = "Description";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 300;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn5.Width = 300;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "ResearchSponsor";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle13;
            this.dataGridViewTextBoxColumn6.HeaderText = "Research Sponsor";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 150;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn6.Width = 150;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "ProofOfConsent";
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = dataGridViewCellStyle14;
            this.dataGridViewTextBoxColumn7.HeaderText = "Proof Of Consent?";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 130;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn7.Width = 130;
            // 
            // NonPurgedShortAccountDetails
            // 
            this.Controls.Add(this.NonPurgedAccountDetailsPanel);
            this.Name = "NonPurgedShortAccountDetails";
            this.Size = new System.Drawing.Size(908, 528);
            this.NonPurgedAccountDetailsPanel.ResumeLayout(false);
            this.NonPurgedAccountDetailsPanel.PerformLayout();
            this.accountView.ResumeLayout(false);
            this.billingBody.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.regulatoryBody.ResumeLayout(false);
            this.regulatoryBody.PerformLayout();
            this.liabilityBody.ResumeLayout(false);
            this.liabilityBody.PerformLayout();
            this.regulatoryHeader.ResumeLayout(false);
            this.generalInsuranceBody.ResumeLayout(false);
            this.generalInsuranceBody.PerformLayout();
            this.liabilityHeader.ResumeLayout(false);
            this.diagnosisAndClinicalBody.ResumeLayout(false);
            this.diagnosisAndClinicalBody.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.diangnosisAndClinicalHeader.ResumeLayout(false);
            this.dempgraphicsHeader.ResumeLayout(false);
            this.demographicsBody.ResumeLayout(false);
            this.demographicsBody.PerformLayout();
            this.generalInsuranceInformationHeader.ResumeLayout(false);
            this.primaryInsuranceMSPHeader.ResumeLayout(false);
            this.primaryInsuranceMSPBody.ResumeLayout(false);
            this.primaryInsuranceMSPBody.PerformLayout();
            this.primaryInsurancePlanHeader.ResumeLayout(false);
            this.primaryInsurancePlanBody.ResumeLayout(false);
            this.primaryInsurancePlanBody.PerformLayout();
            this.primaryInsurancePayorHeader.ResumeLayout(false);
            this.primaryInsurancePayorBody.ResumeLayout(false);
            this.primaryInsurancePayorBody.PerformLayout();
            this.primaryInsuranceInsuredHeader.ResumeLayout(false);
            this.primaruInsuranceInsuredBody.ResumeLayout(false);
            this.primaruInsuranceInsuredBody.PerformLayout();
            this.primaryInsuranceVerificationHeader.ResumeLayout(false);
            this.primaryInsuranceVerificationBody.ResumeLayout(false);
            this.primaryInsuranceVerificationBody.PerformLayout();
            this.primaryInsuranceAuthorizatioHeader.ResumeLayout(false);
            this.primaryInsuranceAuthorizationBody.ResumeLayout(false);
            this.primaryInsuranceAuthorizationBody.PerformLayout();
            this.secondaryInsurancePlanHeader.ResumeLayout(false);
            this.secondaryInsurancePlanBody.ResumeLayout(false);
            this.secondaryInsurancePlanBody.PerformLayout();
            this.SecondaryInsurancePayorHeader.ResumeLayout(false);
            this.secondaryInsurancePayorDetailsBody.ResumeLayout(false);
            this.secondaryInsurancePayorDetailsBody.PerformLayout();
            this.secondaryInsuranceInsuredHeader.ResumeLayout(false);
            this.secondaryInsuranceInsuredBody.ResumeLayout(false);
            this.secondaryInsuranceInsuredBody.PerformLayout();
            this.secondaryInsuranceVerificationHeader.ResumeLayout(false);
            this.secondaryInsuranceVerificationBody.ResumeLayout(false);
            this.secondaryInsuranceVerificationBody.PerformLayout();
            this.secondaryInsuranceAuthorizationHeader.ResumeLayout(false);
            this.secondaryInsuranceAuthorizationBody.ResumeLayout(false);
            this.secondaryInsuranceAuthorizationBody.PerformLayout();
            this.guarantorHeader.ResumeLayout(false);
            this.guarantorBody.ResumeLayout(false);
            this.guarantorBody.PerformLayout();
            this.paymentBody.ResumeLayout(false);
            this.paymentBody.PerformLayout();
            this.PaymentHeader.ResumeLayout(false);
            this.billingHeader.ResumeLayout(false);
            this.ResumeLayout(false);

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

        #region Public Methods

        public override void UpdateView()
        {
            DisplayNonPurgedAccountDetails();
        }

        #endregion

        #region Private Methods

        private void DisplayNonPurgedAccountDetails()
        {
            patient = Model.Patient;
            account = Model;
            var primaryCarePhysicianFeatureManager = new PrimaryCarePhysicianFeatureManager();
            if ( primaryCarePhysicianFeatureManager.IsPrimaryCarePhysicianEnabledForDate( account.AccountCreatedDate ) )
            {
                this.primaryCarePhysicianLabel.Text = "Primary Care physician:";
            }
            else
            {
                this.primaryCarePhysicianLabel.Text = "Other physician:";
            }
            PatientDemographicsAndEmploymentDetails();
            DiagnosisDetails();
            ClinicalDetails();
            DisplayClinicalResearchStudiesList();
            GeneralInsuranceInformationDetails();
            MSPDetails();
            PrimaryInsurancePlanDetails();
            PrimaryInsurancePayorDetails();
            PrimaryInsuranceInsuredDetails();
            PrimaryInsuranceVerificationDetails();
            PrimaryInsuranceAuthorizationDetails();
            SecondaryInsurancePlanDetails();
            SecondaryInsurancePayorDetails();
            SecondaryInsuranceInsuredDetails();
            SecondaryInsuranceVerificationDetails();
            SecondaryInsuranceAuthorizationDetails();
            GuarantorDetails();
            BillingDetails();
            LiabilityDetails();
            PaymentDetails();
            ContactsDetails();
            RegulatoryDetails();
        }

        private void DisplayClinicalResearchStudiesList()
        {
            int studiesCount = account.ClinicalResearchStudies.Count();
            List<ConsentedResearchStudy> researchStudiesList = account.ClinicalResearchStudies.ToList();

            for ( int i = studiesCount; i < 10; i++ )
            {
                ResearchStudy study = new ResearchStudy( String.Empty, String.Empty, String.Empty );
                ConsentedResearchStudy consentedResearchStudy = new ConsentedResearchStudy( study, YesNoFlag.Blank );
                researchStudiesList.Add( consentedResearchStudy );
            }

            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = new List<ConsentedResearchStudy>( researchStudiesList );
        }

        private void PatientDemographicsAndEmploymentDetails()
        {
            patientNameValueLabel.Text = patient.AsFormattedNameWithSuffix;
            if ( patient.Aliases.Count > 0 )
            {
                akaValueLabel.Text = ( ( Name )patient.Aliases[FIRST_ALIAS] )
                    .AsFormattedName();
            }
            if ( account.AdmitDate != DateTime.MinValue )
            {
                admitDateTimeValueLabel.Text = account.AdmitDate.ToString( "MM/dd/yyyy HH:mm" );
            }
           
            if ( account.DischargeDate != DateTime.MinValue )
            {
                dischargeDateTimeValueLabel.Text = account.DischargeDate.ToString( "MM/dd/yyyy HH:mm" );
            }
            if ( account.DischargeDisposition != null )
            {
                dischargeDispositionValueLabel.Text = account.DischargeDisposition.
                    Description;
            }
            if ( account.ScheduleCode != null )
            {
                appointmentValueLabel.Text = account.ScheduleCode.ToCodedString();
            }
            if ( patient.Sex != null )
            {
                genderValueLabel.Text = patient.Sex.Description;
            }

            if (patient.DateOfBirth != DateTime.MinValue)
            {
                dobValueLabel.Text = patient.DateOfBirth.ToString("MM/dd/yyyy");
            }

            ageValueLabel.Text = patient.AgeAt( DateTime.Today );
            if ( patient.MaritalStatus != null )
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

            if ( patient.SocialSecurityNumber != null )
            {
                ssnValueLabel.Text = patient.SocialSecurityNumber.AsFormattedString();
            }
            if ( account.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() )
                .Address != null )
            {
                patientMailingAddressValueLabel.Text = account.Patient.ContactPointWith(
                    TypeOfContactPoint.NewMailingContactPointType() ).Address.
                    OneLineAddressLabelNoCountry();
            }
            if ( account.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() )
                .PhoneNumber != null )
            {
                contactPhoneValueLabel.Text = account.Patient.ContactPointWith(
                    TypeOfContactPoint.NewMailingContactPointType() ).PhoneNumber.
                    AsFormattedString();
            }
            if ( account.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() ).
                CellPhoneNumber != null )
            {
                contactCellValueLabel.Text = account.Patient.ContactPointWith(
                    TypeOfContactPoint.NewMailingContactPointType() ).CellPhoneNumber.
                    AsFormattedString();
            }
            if (account.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).EmailAddress != null)
            {
                
                contactEmailValueLabel.Text = account.Patient.ContactPointWith(
                    TypeOfContactPoint.NewMailingContactPointType()).EmailAddress.Uri;
            }

            if ( patient.DriversLicense != null )
            {
                driversLicenseValueLabel.Text = patient.DriversLicense.Number;
            }
            if ( patient.DriversLicense.State != null )
            {
                driversLicenseValueLabel.Text += " " + patient.DriversLicense.State.Code;
            }
            if ( patient.Passport != null )
            {
                passportValueLabel.Text = patient.Passport.Number.Trim();
            }
            if ( patient.Passport != null && patient.Passport.Country != null )
            {
                passportIssuingCountryValueLabel.Text = patient.Passport.Country.Description;
            }

           
            if ( patient.Language != null )
            {
                languageValueLabel.Text = patient.Language.Description;
            }
            

        }

        private void DiagnosisDetails()
        {
            account.SetUCCVisitType();
            if ( account.KindOfVisit != null )
            {
                patientTypeValueLabel.Text = account.KindOfVisit.ToCodedString();
            }
            if ( account.Reregister != null )
            {
                reregisterValueLabel.Text = account.Reregister.Description;
            }
            if ( account.HospitalService != null )
            {
                hospitalServiceValueLabel.Text = account.HospitalService.ToCodedString();
            }
           

            if ( account.Diagnosis != null )
            {
                chiefComplaintValueLabel.Text = account.Diagnosis.ChiefComplaint;
                procedureValueLabel.Text = account.Diagnosis.Procedure;
                CptCodesValueLabel.Text = account.CptCodes.Count > 0 ? "Yes" : "No";
                commentsValueLabel.Text = account.ClinicalComments;
                if ( account.Diagnosis.Condition != null )
                {
                    if ( account.Diagnosis.Condition.IsAccidentOrCrime() )
                    {
                        if (account.Diagnosis.Condition.GetType() == typeof(Accident))
                        {
                            Accident accident = (Accident)account.Diagnosis.Condition;
                            if (accident.Kind != null)
                            {
                                accidentTypeValueLabel.Text = accident.Kind.Description;
                            }
                        }
                        TimeAndLocationBoundCondition condition =
                            ( TimeAndLocationBoundCondition )account.Diagnosis.Condition;
                        if ( condition.OccurredOn != DateTime.MinValue )
                        {
                            accidentCrimeDateValueLabel.Text = condition.OccurredOn.ToString( "MM/dd/yyyy" );
                        }

                        accidentCrimeHourValueLabel.Text = condition.GetOccurredHour();
                            
                        if ( condition.Country != null )
                        {
                            accidentCrimeCountryValueLabel.Text =
                                condition.Country.ToString();
                        }
                        if ( condition.State != null )
                        {
                            accidentCrimeStateProvinceValueLabel.Text =
                                condition.State.Code + "-" +
                                condition.State.DisplayString;
                        }
                    }
                }
            }

            this.PopulateOnsetDate();

            if ( account.AdmitSource != null )
            {
                admitSourceValueLabel.Text = account.AdmitSource.DisplayString;
            }
           
            if ( account.Clinics != null && account.Clinics.Count > 0 )
            {
                if ( account.Clinics[0] != null )
                {
                    clinic1ValueLabel.Text = account.Clinics[0].ToString();
                }
                if ( account.Clinics[1] != null )
                {
                    clinic2ValueLabel.Text = account.Clinics[1].ToString();
                }
                if ( account.Clinics[2] != null )
                {
                    clinic3ValueLabel.Text = account.Clinics[2].ToString();
                }
                if ( account.Clinics[3] != null )
                {
                    clinic4ValueLabel.Text = account.Clinics[3].ToString();
                }
                if ( account.Clinics[4] != null )
                {
                    clinic5ValueLabel.Text = account.Clinics[4].ToString();
                }
            }
        }
       
        private void ClinicalDetails()
        {
            if ( account.ReferringPhysician != null )
            {
                referringPhysicianValueLabel.Text = String.Format( "{0:00000} {1}",
                    account.ReferringPhysician.PhysicianNumber,
                    account.ReferringPhysician.FormattedName );
            }
            if ( account.AdmittingPhysician != null )
            {
                admittingPhysicianValueLabel.Text = String.Format( "{0:00000} {1}",
                    account.AdmittingPhysician.PhysicianNumber,
                    account.AdmittingPhysician.FormattedName );
            }
            if ( account.AttendingPhysician != null )
            {
                attendingPhysicianValueLabel.Text = String.Format( "{0:00000} {1}",
                    account.AttendingPhysician.PhysicianNumber,
                    account.AttendingPhysician.FormattedName );
            }
            
            if ( account.PrimaryCarePhysician != null )
            {
                primaryCarePhysicianValueLabel.Text = String.Format( "{0:00000} {1}",
                    account.PrimaryCarePhysician.PhysicianNumber,
                    account.PrimaryCarePhysician.FormattedName );
            }
           
            
            if ( account.IsPatientInClinicalResearchStudy != null )
            {
                PatientInClinicalStudyValueLabel.Text = account.IsPatientInClinicalResearchStudy.Description;
            }
            if ( account.RightCareRightPlace != null && account.RightCareRightPlace.RCRP != null )
            {
                RCRPValueLabel.Text = account.RightCareRightPlace.RCRP.Description;
            }
            if ( account.RightCareRightPlace != null && account.RightCareRightPlace.LeftOrStayed != null )
            {
                LeftOrStayedValueLabel.Text = account.RightCareRightPlace.LeftOrStayed.Description;
            }
            if ( account.LeftWithOutBeingSeen != null )
            {
                LeftWithoutBeingSeenValueLabel.Text = account.LeftWithOutBeingSeen.Description;
            }
            if ( account.LeftWithoutFinancialClearance != null )
            {
                LeftWithoutFinancialClearanceValueLabel.Text = account.LeftWithoutFinancialClearance.Description;
            }
        }

        private void GeneralInsuranceInformationDetails()
        {
            if ( account.FinancialClass != null )
            {
                financialClassValueLabel.Text = account.FinancialClass.ToCodedString();
            }
            if ( patient.MothersDateOfBirth != DateTime.MinValue )
            {
                motherDobValueLabel.Text = patient.MothersDateOfBirth.ToString( "MM/dd/yyyy" );
            }
            if ( patient.FathersDateOfBirth != DateTime.MinValue )
            {
                fatherDobValueLabel.Text = patient.FathersDateOfBirth.ToString( "MM/dd/yyyy" );
            }
        }

        private void MSPDetails()
        {
            if ( account.MedicareSecondaryPayor.HasBeenCompleted )
            {
                MSPRecommendation mspRecommendation =
                    account.MedicareSecondaryPayor.MakeRecommendation();

                if ( mspRecommendation != null )
                {
                    if ( mspRecommendation.IsMedicareRecommended )
                    {
                        mspQuestionnaireSummaryConclusionLabel.Text = PRIMARY_MSP;
                    }
                    else
                    {
                        mspQuestionnaireSummaryConclusionLabel.Text = NOT_PRIMARY_MSP;
                    }
                }
                else
                {
                    mspQuestionnaireSummaryConclusionLabel.Text = NO_MSP;
                }
            }
            else
            {
                mspQuestionnaireSummaryConclusionLabel.Text = NO_MSP;
            }
        }

        private void PrimaryInsurancePlanDetails()
        {
            if ( account.Insurance != null )
            {
                Coverage coverage = PrimaryCoverage();
                if ( coverage != null )
                {
                    if ( coverage.InsurancePlan != null )
                    {
                        primaryPlanValueLabel.Text = coverage.InsurancePlan.PlanName;
                        primaryPayorBrokerValueLabel.Text = coverage.InsurancePlan.Payor.Name;
                        primaryCategoryValueLabel.Text = coverage.InsurancePlan.PlanCategory
                            .Description;
                    }
                }
            }
        }

        private void PrimaryInsurancePayorDetails()
        {
            if ( account.Insurance != null )
            {
                Coverage coverage = PrimaryCoverage();
                if ( coverage != null )
                {
                    if ( coverage.InsurancePlan.GetType() == typeof( CommercialInsurancePlan ) )
                    {
                        CommercialCoverage commercialCoverage = ( CommercialCoverage )coverage;
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.
                            Text = commercialCoverage.CertSSNID;
                        primaryGroupNumberValueLabel.Text = commercialCoverage.GroupNumber;
                        primaryAuthorizationTrackingNumberValueLabel.Text = commercialCoverage.
                            TrackingNumber;
                        primaryPrecertificationNumberValueLabel.Text = commercialCoverage.
                            PreCertNumber.ToString();
                        if ( account.MedicalGroupIPA != null )
                        {
                            primaryMedicalGroupOrIpaNameValueLabel.Text =
                                account.MedicalGroupIPA.Name;
                            foreach ( Clinic clinic in account.MedicalGroupIPA.Clinics )
                            {
                                primaryMedicalGroupOrIPAClinicValueLabel.Text = clinic.Name;
                            }
                        }
                        if ( commercialCoverage.BillingInformation != null )
                        {
                            primaryBillingCoNameValueLabel.Text = commercialCoverage.
                                BillingInformation.BillingCOName;
                            primaryBillingNameValueLabel.Text = commercialCoverage.
                                BillingInformation.BillingName;
                            if ( commercialCoverage.BillingInformation.PhoneNumber != null )
                            {
                                primaryBillingPhoneValueLabel.Text = commercialCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( commercialCoverage.BillingInformation.Address != null )
                            {
                                primaryBillingAddressValueLabel.Text = commercialCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }

                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( GovernmentMedicaidInsurancePlan ) )
                    {
                        GovernmentMedicaidCoverage governmentMedicaidCoverage
                            = ( GovernmentMedicaidCoverage )coverage;
                        primaryAuthorizationTrackingNumberValueLabel.Text = governmentMedicaidCoverage.
                            TrackingNumber;
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "Policy/CIN number:";
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 116, 16 );

                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                            new Point( 130, 2155 );
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Text =
                            governmentMedicaidCoverage.PolicyCINNumber;
                        if ( governmentMedicaidCoverage.IssueDate != DateTime.MinValue )
                        {
                            primaryMedicaidIssueDateValueLabel.Text = governmentMedicaidCoverage.
                                IssueDate.ToString( "MM/dd/yyyy" );
                        }
                        if ( account.MedicalGroupIPA != null )
                        {
                            primaryMedicalGroupOrIpaNameValueLabel.Text =
                                account.MedicalGroupIPA.Name;
                            foreach ( Clinic clinic in account.MedicalGroupIPA.Clinics )
                            {
                                primaryMedicalGroupOrIPAClinicValueLabel.Text = clinic.Name;
                            }
                        }
                        if ( governmentMedicaidCoverage.BillingInformation != null )
                        {
                            primaryBillingCoNameValueLabel.Text = governmentMedicaidCoverage.
                                BillingInformation.BillingCOName;
                            primaryBillingNameValueLabel.Text = governmentMedicaidCoverage.
                                BillingInformation.BillingName;
                            if ( governmentMedicaidCoverage.BillingInformation.PhoneNumber != null )
                            {
                                primaryBillingPhoneValueLabel.Text = governmentMedicaidCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( governmentMedicaidCoverage.BillingInformation.Address != null )
                            {
                                primaryBillingAddressValueLabel.Text = governmentMedicaidCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }

                    }
                    if ( coverage.InsurancePlan.GetType() == ( typeof( WorkersCompensationInsurancePlan ) ) )
                    {
                        WorkersCompensationCoverage workersCompensationCoverage =
                            ( WorkersCompensationCoverage )coverage;

                        //Workers compensation policy number
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "Workers compensation policy number:";
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 215, 16 );

                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                            new Point( 228, 2155 );
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Text =
                            workersCompensationCoverage.PolicyNumber;
                        primaryAdjusterValueLabel.Text = workersCompensationCoverage.InsuranceAdjuster;
                        primaryEmployeeSupervisorValueLabel.Text = workersCompensationCoverage.PatientsSupervisor;
                        if ( this.account.MedicalGroupIPA != null )
                        {
                            primaryMedicalGroupOrIpaNameValueLabel.Text =
                                this.account.MedicalGroupIPA.Name;
                            foreach ( Clinic clinic in this.account.MedicalGroupIPA.Clinics )
                            {
                                primaryMedicalGroupOrIPAClinicValueLabel.Text = clinic.Name;
                            }
                        }
                        if ( workersCompensationCoverage.BillingInformation != null )
                        {
                            primaryBillingCoNameValueLabel.Text = workersCompensationCoverage.
                                BillingInformation.BillingCOName;
                            primaryBillingNameValueLabel.Text = workersCompensationCoverage.
                                BillingInformation.BillingName;
                            if ( workersCompensationCoverage.BillingInformation.PhoneNumber != null )
                            {
                                primaryBillingPhoneValueLabel.Text = workersCompensationCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( workersCompensationCoverage.BillingInformation.Address != null )
                            {
                                primaryBillingAddressValueLabel.Text = workersCompensationCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }

                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( GovernmentOtherInsurancePlan ) )
                    {
                        GovernmentOtherCoverage governmentOtherCoverage =
                            ( GovernmentOtherCoverage )coverage;
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel
                            .Text = governmentOtherCoverage.CertSSNID;
                        primaryGroupNumberValueLabel.Text = governmentOtherCoverage.GroupNumber;
                        primaryAuthorizationTrackingNumberValueLabel.Text = governmentOtherCoverage.
                             TrackingNumber;
                        primaryPrecertificationNumberValueLabel.Text = governmentOtherCoverage.
                            PreCertNumber.ToString();
                        if ( this.account.MedicalGroupIPA != null )
                        {
                            primaryMedicalGroupOrIpaNameValueLabel.Text =
                                this.account.MedicalGroupIPA.Name;
                            foreach ( Clinic clinic in this.account.MedicalGroupIPA.Clinics )
                            {
                                primaryMedicalGroupOrIPAClinicValueLabel.Text = clinic.Name;
                            }
                        }
                        if ( governmentOtherCoverage.BillingInformation != null )
                        {
                            primaryBillingCoNameValueLabel.Text = governmentOtherCoverage.
                                BillingInformation.BillingCOName;
                            primaryBillingNameValueLabel.Text = governmentOtherCoverage.
                                BillingInformation.BillingName;
                            primaryBillingPhoneValueLabel.Text = governmentOtherCoverage.
                                BillingInformation.PhoneNumber.AsFormattedString();
                            if ( governmentOtherCoverage.BillingInformation.Address != null )
                            {
                                primaryBillingAddressValueLabel.Text = governmentOtherCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }
                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( GovernmentMedicareInsurancePlan ) )
                    {
                        GovernmentMedicareCoverage governmentMedicareCoverage =
                            ( GovernmentMedicareCoverage )coverage;

                        //HIC number
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "MBI number:";
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 76, 16 );

                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                            new Point( 88, 2155 );
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Text =
                            governmentMedicareCoverage.FormattedMBINumber;
                       

                        if ( governmentMedicareCoverage.BillingInformation != null )
                        {
                            primaryBillingCoNameValueLabel.Text = governmentMedicareCoverage.
                                BillingInformation.BillingCOName;
                            primaryBillingNameValueLabel.Text = governmentMedicareCoverage.
                                BillingInformation.BillingName;
                            if ( governmentMedicareCoverage.BillingInformation.PhoneNumber != null )
                            {
                                primaryBillingPhoneValueLabel.Text = governmentMedicareCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( governmentMedicareCoverage.BillingInformation.Address != null )
                            {
                                primaryBillingAddressValueLabel.Text = governmentMedicareCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }

                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( SelfPayInsurancePlan ) )
                    {
                        SelfPayCoverage selfPayCoverage = ( SelfPayCoverage )coverage;
                        if ( selfPayCoverage.BillingInformation != null )
                        {
                            primaryBillingCoNameValueLabel.Text = selfPayCoverage.
                                BillingInformation.BillingCOName;
                            primaryBillingNameValueLabel.Text = selfPayCoverage.
                                BillingInformation.BillingName;
                            if ( selfPayCoverage.BillingInformation.PhoneNumber != null )
                            {
                                primaryBillingPhoneValueLabel.Text = selfPayCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( selfPayCoverage.BillingInformation.Address != null )
                            {
                                primaryBillingAddressValueLabel.Text = selfPayCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }

                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( OtherInsurancePlan ) )
                    {
                        OtherCoverage otherCoverage = ( OtherCoverage )coverage;
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.
                            Text = otherCoverage.CertSSNID;
                        primaryGroupNumberValueLabel.Text = otherCoverage.GroupNumber;
                        primaryAuthorizationTrackingNumberValueLabel.Text = otherCoverage.
                            TrackingNumber;
                        primaryPrecertificationNumberValueLabel.Text = otherCoverage.
                            PreCertNumber.ToString();
                        if ( this.account.MedicalGroupIPA != null )
                        {
                            primaryMedicalGroupOrIpaNameValueLabel.Text =
                                this.account.MedicalGroupIPA.Name;
                            foreach ( Clinic clinic in this.account.MedicalGroupIPA.Clinics )
                            {
                                primaryMedicalGroupOrIPAClinicValueLabel.Text = clinic.Name;
                            }
                        }
                        if ( otherCoverage.BillingInformation != null )
                        {
                            primaryBillingCoNameValueLabel.Text = otherCoverage.
                                BillingInformation.BillingCOName;
                            primaryBillingNameValueLabel.Text = otherCoverage.
                                BillingInformation.BillingName;
                            if ( otherCoverage.BillingInformation.PhoneNumber != null )
                            {
                                primaryBillingPhoneValueLabel.Text = otherCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( otherCoverage.BillingInformation.Address != null )
                            {
                                primaryBillingAddressValueLabel.Text = otherCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }
                    }
                }
            }
        }

        private void PrimaryInsuranceInsuredDetails()
        {
            if ( account.PrimaryInsured != null )
            {
                primaryNameValueLabel.Text = account.PrimaryInsured.Name.AsFormattedNameWithSuffix();
                RelationshipType relationShip = account.PrimaryInsured.
                    RelationshipWith( account.PrimaryInsured );
                if ( relationShip != null )
                {
                    primaryThePatientIsTheInsuredsValueLabel.Text = relationShip.Description;
                }
                Gender gender = account.PrimaryInsured.Sex;
                if ( gender != null )
                {
                    primaryGenderValueLabel.Text = gender.Description;
                }
                if ( account.PrimaryInsured.DateOfBirth != DateTime.MinValue )
                {
                    primaryDobValueLabel.Text = account.PrimaryInsured.
                        DateOfBirth.ToString( "MM/dd/yyyy" );
                }
                primaryNationalIdValueLabel.Text = account.PrimaryInsured.NationalID;
                ContactPoint cp = account.PrimaryInsured.ContactPointWith
                    ( TypeOfContactPoint.NewPhysicalContactPointType() );
                if ( cp != null && cp.Address != null )
                {
                    primaryAddressValueLabel.Text = cp.Address.OneLineAddressLabel();
                }
                if ( cp != null && cp.PhoneNumber != null )
                {
                    primaryContactPhoneValueLabel.Text = cp.PhoneNumber.AsFormattedString();
                }
                cp = account.PrimaryInsured.ContactPointWith(
                    TypeOfContactPoint.NewMobileContactPointType() );
                if ( cp != null && cp.PhoneNumber != null )
                {
                    primaryContactCellValueLabel.Text = cp.PhoneNumber.AsFormattedString();
                }
                if ( account.PrimaryInsured.Employment != null )
                {
                    if ( account.PrimaryInsured.Employment.Status != null )
                    {
                        primaryEmploymentStatusValueLabel.Text = account.PrimaryInsured
                            .Employment.Status.Description;
                    }
                    if ( account.PrimaryInsured.Employment.Employer != null )
                    {
                        primaryEmployerValueLabel.Text = account.PrimaryInsured
                            .Employment.Employer.Name;
                    }
                    if ( account.PrimaryInsured.Employment.Employer.PartyContactPoint.PhoneNumber != null )
                    {
                        primaryEmployerPhoneValueLabel.Text =
                            account.PrimaryInsured.Employment.Employer.PartyContactPoint.PhoneNumber.AsFormattedString();
                    }
                }
            }
        }

        private void PrimaryInsuranceVerificationDetails()
        {
            if ( account.Insurance != null )
            {
                Coverage coverage = PrimaryCoverage();
                if ( coverage != null )
                {
                    if ( coverage.BenefitsVerified != null )
                    {
                        primaryBenefitsVerifiedValueLabel.Text = coverage.
                            BenefitsVerified.Description;
                    }
                    primaryInitiatedByValueLabel.Text = coverage.AuthorizingPerson;
                    if ( coverage.DateTimeOfVerification != DateTime.MinValue )
                    {
                        primaryDateValueLabel.Text = coverage.
                            DateTimeOfVerification.ToString( "MM/dd/yyyy" );
                    }
                    if ( coverage.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
                    {
                        CoverageGroup coverageGroup = coverage as CoverageGroup;
                        if ( coverageGroup.Authorization.AuthorizationRequired != null )
                        {
                            primaryAuthorizationRequiredValueLabel.Text = coverageGroup.Authorization.
                                AuthorizationRequired.Description;
                        }
                        primaryAuthorizationCompanyValueLabel.Text = coverageGroup.Authorization.AuthorizationCompany;
                        primaryAuthorizationPhoneValueLabel.Text = coverageGroup.Authorization.AuthorizationPhone.ToString();
                        primaryPromptExtValueLabel.Text = coverageGroup.Authorization.PromptExt;
                    }
                    if ( coverage.Attorney != null )
                    {
                        primaryAttorneyNameValueLabel.Text = coverage.Attorney.FirstName;

                        if ( coverage.Attorney.ContactPointWith
                            ( TypeOfContactPoint.NewBusinessContactPointType() ).Address != null )
                        {
                            primaryAttorneyAddressValueLabel.Text = coverage.Attorney.ContactPointWith
                                ( TypeOfContactPoint.NewBusinessContactPointType() ).Address.
                                OneLineAddressLabel();
                        }
                        if ( coverage.Attorney.ContactPointWith
                            ( TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber != null )
                        {
                            primaryAttorneyPhoneNumberValueLabel.Text = coverage.Attorney.ContactPointWith
                                ( TypeOfContactPoint.NewBusinessContactPointType() ).
                                PhoneNumber.AsFormattedString();
                        }
                    }
                    if ( coverage.InsuranceAgent != null )
                    {
                        primaryAutoHomeInsuranceAgentNameValueLabel.Text = coverage.InsuranceAgent.FirstName;
                        if ( coverage.InsuranceAgent.ContactPointWith
                            ( TypeOfContactPoint.NewBusinessContactPointType() ).Address != null )
                        {
                            primaryAutoHomeInsuranceAgentAddressValueLabel.Text = coverage.InsuranceAgent.ContactPointWith
                                ( TypeOfContactPoint.NewBusinessContactPointType() ).Address.OneLineAddressLabel();
                        }
                        if ( coverage.InsuranceAgent.ContactPointWith
                            ( TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber != null )
                        {
                            primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Text = coverage.InsuranceAgent.ContactPointWith
                                ( TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber.AsFormattedString();
                        }

                    }
                }
            }
        }

        private void PrimaryInsuranceAuthorizationDetails()
        {
            Coverage coverage = PrimaryCoverage();
            if ( coverage != null )
            {
                if ( coverage.GetType() == typeof( CommercialCoverage ) ||
                         coverage.GetType() == typeof( OtherCoverage ) ||
                         coverage.GetType() == typeof( GovernmentOtherCoverage ) ||
                         coverage.GetType() == typeof( GovernmentMedicaidCoverage ) ||
                         coverage.GetType() == typeof( WorkersCompensationCoverage ) )
                {
                    if (coverage.GetType() == typeof(CommercialCoverage) ||
                        coverage.GetType() == typeof(OtherCoverage) ||
                        coverage.GetType() == typeof(GovernmentOtherCoverage))
                    {
                        primaryAuthorizationTrackingNumberValueLabel.Text =
                            ((CoverageForCommercialOther) coverage).TrackingNumber;
                    }

                    if (coverage.GetType() == typeof(GovernmentMedicaidCoverage))
                    {
                        primaryAuthorizationTrackingNumberValueLabel.Text =
                            ((GovernmentMedicaidCoverage) coverage).TrackingNumber;
                    }

                    Authorization authorization = ( ( CoverageGroup )coverage ).Authorization;
                    if ( authorization != null )
                    {
                        primaryAuthorizationNumberValueLabel.Text = authorization.AuthorizationNumber;
                        primaryDaysAuthorizedValueLabel.Text = authorization.NumberOfDaysAuthorized.ToString();
                        primaryAuthorizationCompanyRepresentativeNameValueLabel.Text =
                            String.Format( "{0}, {1}", authorization.NameOfCompanyRepresentative.FirstName.Trim(), authorization.NameOfCompanyRepresentative.LastName.Trim() );
                        primaryServicesAuthorizedValueLabel.Text = authorization.
                            ServicesAuthorized;
                        primaryEffectiveDateOfAuthorizationValueLabel.Text =
                            authorization.EffectiveDate.ToString( "MM/dd/yyyy" );
                        primaryExpirationDateOfAuthorizationValueLabel.Text =
                            authorization.ExpirationDate.ToString( "MM/dd/yyyy" );
                        primaryAuthorizationStatusValueLabel.Text = authorization.AuthorizationStatus.Description;
                        primaryRemarksValueLabel.Text = authorization.Remarks;
                    }
                }

                PrimaryInsuranceAuthorizationForGovernmentMedicare53544Details(coverage);
            }
        }

        private void PrimaryInsuranceAuthorizationForGovernmentMedicare53544Details(Coverage coverage)
        {
            if (coverage != null)
            {
                if (coverage.GetType() == typeof(GovernmentMedicareCoverage))
                {
                    var medicareCoverage = (GovernmentMedicareCoverage) coverage;
                    if (medicareCoverage != null && medicareCoverage.IsMedicareCoverageValidForAuthorization)
                    {
                        primaryAuthorizationTrackingNumberValueLabel.Text =
                            medicareCoverage.TrackingNumber;
                        var authorization = medicareCoverage.Authorization;
                        if (authorization != null)
                        {
                            primaryAuthorizationNumberValueLabel.Text = authorization.AuthorizationNumber;
                            primaryDaysAuthorizedValueLabel.Text = authorization.NumberOfDaysAuthorized.ToString();
                            primaryAuthorizationCompanyRepresentativeNameValueLabel.Text =
                                String.Format("{0}, {1}", authorization.NameOfCompanyRepresentative.FirstName.Trim(),
                                    authorization.NameOfCompanyRepresentative.LastName.Trim());
                            primaryServicesAuthorizedValueLabel.Text = authorization.ServicesAuthorized;
                            primaryEffectiveDateOfAuthorizationValueLabel.Text =
                                authorization.EffectiveDate.ToString("MM/dd/yyyy");
                            primaryExpirationDateOfAuthorizationValueLabel.Text =
                                authorization.ExpirationDate.ToString("MM/dd/yyyy");
                            primaryAuthorizationStatusValueLabel.Text = authorization.AuthorizationStatus.Description;
                            primaryRemarksValueLabel.Text = authorization.Remarks;
                        }
                    }
                }
            }
        }

        private void SecondaryInsurancePlanDetails()
        {
            if ( account.Insurance != null )
            {
                Coverage coverage = SecondaryCoverage();
                if ( coverage != null )
                {
                    if ( coverage.InsurancePlan != null )
                    {
                        secondaryPlanValueLabel.Text = coverage.InsurancePlan.PlanName;
                        secondaryPayorBrokerValueLabel.Text = coverage.InsurancePlan.
                            Payor.Name;
                        secondaryCategoryValueLabel.Text = coverage.InsurancePlan.
                            PlanCategory.Description;
                    }
                }
            }
        }

        private void SecondaryInsurancePayorDetails()
        {
            if ( account.Insurance != null )
            {
                Coverage coverage = SecondaryCoverage();
                if ( coverage != null )
                {
                    if ( coverage.InsurancePlan.GetType() == typeof( CommercialInsurancePlan ) )
                    {
                        CommercialCoverage commercialCoverage = ( CommercialCoverage )coverage;
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.
                            Text = commercialCoverage.CertSSNID;
                        secondaryGroupNumberValueLabel.Text = commercialCoverage.GroupNumber;
                        secondaryPrecertificationNumberValueLabel.Text = commercialCoverage.
                            PreCertNumber.ToString();

                        if ( commercialCoverage.BillingInformation != null )
                        {
                            secondaryBillingCoNameValueLabel.Text = commercialCoverage.
                                BillingInformation.BillingCOName;
                            secondaryBillingNameValueLabel.Text = commercialCoverage.
                                BillingInformation.BillingName;
                            if ( commercialCoverage.BillingInformation.PhoneNumber != null )
                            {
                                secondaryBillingPhoneValueLabel.Text = commercialCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( commercialCoverage.BillingInformation.Address != null )
                            {
                                secondaryBillingAddressValueLabel.Text = commercialCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }


                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( GovernmentMedicaidInsurancePlan ) )
                    {
                        GovernmentMedicaidCoverage governmentMedicaidCoverage
                            = ( GovernmentMedicaidCoverage )coverage;

                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "Policy/CIN number:";
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 116, 16 );

                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                            new Point( 130, 3603 );
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Text =
                            governmentMedicaidCoverage.PolicyCINNumber;

                        if ( governmentMedicaidCoverage.IssueDate != DateTime.MinValue )
                        {
                            secondaryMedicaidIssueDateValueLabel.Text = governmentMedicaidCoverage.
                                IssueDate.ToString( "MM/dd/yyyy" );
                        }
                        if ( governmentMedicaidCoverage.BillingInformation != null )
                        {
                            secondaryBillingCoNameValueLabel.Text = governmentMedicaidCoverage.
                                BillingInformation.BillingCOName;
                            secondaryBillingNameValueLabel.Text = governmentMedicaidCoverage.
                                BillingInformation.BillingName;
                            if ( governmentMedicaidCoverage.BillingInformation.PhoneNumber != null )
                            {
                                secondaryBillingPhoneValueLabel.Text = governmentMedicaidCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( governmentMedicaidCoverage.BillingInformation.Address != null )
                            {
                                secondaryBillingAddressValueLabel.Text = governmentMedicaidCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }

                    }
                    if ( coverage.InsurancePlan.GetType() == ( typeof( WorkersCompensationInsurancePlan ) ) )
                    {
                        WorkersCompensationCoverage workersCompensationCoverage =
                            ( WorkersCompensationCoverage )coverage;

                        //Workers compensation policy number
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "Workers compensation policy number:";
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 215, 16 );

                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                            new Point( 228, 3603 );
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Text =
                            workersCompensationCoverage.PolicyNumber;

                        secondaryAdjusterValueLabel.Text = workersCompensationCoverage.InsuranceAdjuster;
                        secondaryEmployeeSupervisorValueLabel.Text = workersCompensationCoverage.PatientsSupervisor;
                        if ( this.account.MedicalGroupIPA != null )
                        {
                            secondaryMedicalGroupIpaNameValueLabel.Text =
                                this.account.MedicalGroupIPA.Name;
                            foreach ( Clinic clinic in this.account.MedicalGroupIPA.Clinics )
                            {
                                secondaryMedicalGroupIpaClinicValueLabel.Text = clinic.Name;
                            }
                        }
                        if ( workersCompensationCoverage.BillingInformation != null )
                        {
                            secondaryBillingCoNameValueLabel.Text = workersCompensationCoverage.
                                BillingInformation.BillingCOName;
                            secondaryBillingNameValueLabel.Text = workersCompensationCoverage.
                                BillingInformation.BillingName;
                            if ( workersCompensationCoverage.BillingInformation.PhoneNumber != null )
                            {
                                secondaryBillingPhoneValueLabel.Text = workersCompensationCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( workersCompensationCoverage.BillingInformation.Address != null )
                            {
                                secondaryBillingAddressValueLabel.Text = workersCompensationCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }

                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( GovernmentOtherInsurancePlan ) )
                    {
                        GovernmentOtherCoverage governmentOtherCoverage =
                            ( GovernmentOtherCoverage )coverage;
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel
                            .Text = governmentOtherCoverage.CertSSNID;
                        secondaryGroupNumberValueLabel.Text = governmentOtherCoverage.GroupNumber;
                        secondaryPrecertificationNumberValueLabel.Text = governmentOtherCoverage.
                            PreCertNumber.ToString();
                        if ( this.account.MedicalGroupIPA != null )
                        {
                            secondaryMedicalGroupIpaNameValueLabel.Text =
                                this.account.MedicalGroupIPA.Name;
                            foreach ( Clinic clinic in this.account.MedicalGroupIPA.Clinics )
                            {
                                secondaryMedicalGroupIpaClinicValueLabel.Text = clinic.Name;
                            }
                        }
                        if ( governmentOtherCoverage.BillingInformation != null )
                        {
                            secondaryBillingCoNameValueLabel.Text = governmentOtherCoverage.
                                BillingInformation.BillingCOName;
                            secondaryBillingNameValueLabel.Text = governmentOtherCoverage.
                                BillingInformation.BillingName;
                            secondaryBillingPhoneValueLabel.Text = governmentOtherCoverage.
                                BillingInformation.PhoneNumber.AsFormattedString();
                            if ( governmentOtherCoverage.BillingInformation.Address != null )
                            {
                                secondaryBillingAddressValueLabel.Text = governmentOtherCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }


                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( GovernmentMedicareInsurancePlan ) )
                    {
                        GovernmentMedicareCoverage governmentMedicareCoverage =
                            ( GovernmentMedicareCoverage )coverage;

                        //HIC number
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "MBI number:";
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 76, 16 );

                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                            new Point( 88, 3603 );
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Text =
                            governmentMedicareCoverage.FormattedMBINumber;

                       

                        if ( governmentMedicareCoverage.BillingInformation != null )
                        {
                            secondaryBillingCoNameValueLabel.Text = governmentMedicareCoverage.
                                BillingInformation.BillingCOName;
                            secondaryBillingNameValueLabel.Text = governmentMedicareCoverage.
                                BillingInformation.BillingName;
                            if ( governmentMedicareCoverage.BillingInformation.PhoneNumber != null )
                            {
                                secondaryBillingPhoneValueLabel.Text = governmentMedicareCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( governmentMedicareCoverage.BillingInformation.Address != null )
                            {
                                secondaryBillingAddressValueLabel.Text = governmentMedicareCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }

                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( SelfPayInsurancePlan ) )
                    {
                        SelfPayCoverage selfPayCoverage = ( SelfPayCoverage )coverage;
                        selfPayCoverage = ( SelfPayCoverage )coverage;
                        if ( selfPayCoverage.BillingInformation != null )
                        {
                            secondaryBillingCoNameValueLabel.Text = selfPayCoverage.
                                BillingInformation.BillingCOName;
                            secondaryBillingNameValueLabel.Text = selfPayCoverage.
                                BillingInformation.BillingName;
                            if ( selfPayCoverage.BillingInformation.PhoneNumber != null )
                            {
                                secondaryBillingPhoneValueLabel.Text = selfPayCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( selfPayCoverage.BillingInformation.Address != null )
                            {
                                secondaryBillingAddressValueLabel.Text = selfPayCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }
                    }

                    if ( coverage.InsurancePlan.GetType() == typeof( OtherInsurancePlan ) )
                    {
                        OtherCoverage otherCoverage = ( OtherCoverage )coverage;
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.
                            Text = otherCoverage.CertSSNID;
                        secondaryGroupNumberValueLabel.Text = otherCoverage.GroupNumber;

                        secondaryPrecertificationNumberValueLabel.Text = otherCoverage.
                            PreCertNumber.ToString();
                        if ( this.account.MedicalGroupIPA != null )
                        {
                            secondaryMedicalGroupIpaNameValueLabel.Text =
                                this.account.MedicalGroupIPA.Name;
                            foreach ( Clinic clinic in this.account.MedicalGroupIPA.Clinics )
                            {
                                secondaryMedicalGroupIpaClinicValueLabel.Text = clinic.Name;
                            }
                        }
                        if ( otherCoverage.BillingInformation != null )
                        {
                            secondaryBillingCoNameValueLabel.Text = otherCoverage.
                                BillingInformation.BillingCOName;
                            secondaryBillingNameValueLabel.Text = otherCoverage.
                                BillingInformation.BillingName;
                            if ( otherCoverage.BillingInformation.PhoneNumber != null )
                            {
                                secondaryBillingPhoneValueLabel.Text = otherCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if ( otherCoverage.BillingInformation.Address != null )
                            {
                                secondaryBillingAddressValueLabel.Text = otherCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }
                    }
                }
            }
        }

        private void SecondaryInsuranceInsuredDetails()
        {
            if ( account.SecondaryInsured != null )
            {
                secondaryNameValueLabel.Text = account.SecondaryInsured.Name.AsFormattedNameWithSuffix();
                RelationshipType relationShip = account.SecondaryInsured.
                    RelationshipWith( account.SecondaryInsured );
                if ( relationShip != null )
                {
                    secondaryThePatientIsTheInsuredValueLabel.Text = relationShip.Description;
                }
                Gender gender = account.SecondaryInsured.Sex;
                if ( gender != null )
                {
                    secondaryGenderValueLabel.Text = gender.Description;
                }
                if ( account.SecondaryInsured.DateOfBirth != DateTime.MinValue )
                {
                    secondaryDobValueLabel.Text = account.SecondaryInsured.
                        DateOfBirth.ToString( "MM/dd/yyyy" );
                }
                secondaryNationalIdValueLabel.Text = account.SecondaryInsured.NationalID;
                ContactPoint cp = account.SecondaryInsured.ContactPointWith
                    ( TypeOfContactPoint.NewPhysicalContactPointType() );
                if ( cp.Address != null )
                {
                    secondaryAddressValueLabel.Text = cp.Address.OneLineAddressLabel();
                }
                if ( cp.PhoneNumber != null )
                {
                    secondaryContactPhoneValueLabel.Text = cp.PhoneNumber.AsFormattedString();
                }
                cp = account.SecondaryInsured.ContactPointWith(
                    TypeOfContactPoint.NewMobileContactPointType() );
                if ( cp.CellPhoneNumber != null )
                {
                    secondaryContactCellValueLabel.Text = cp.PhoneNumber.AsFormattedString();
                }
                if ( account.SecondaryInsured.Employment != null )
                {
                    if ( account.SecondaryInsured.Employment.Status != null )
                    {
                        secondaryEmploymentStatusValueLabel.Text = account.SecondaryInsured
                            .Employment.Status.Description;
                    }
                    if ( account.SecondaryInsured.Employment.Employer != null )
                    {
                        secondaryEmployerValueLabel.Text = account.SecondaryInsured
                            .Employment.Employer.Name;
                    }

                    if ( account.SecondaryInsured.Employment.Employer.PartyContactPoint.PhoneNumber != null )
                    {
                        secondaryEmployerPhoneValueLabel.Text =
                            account.SecondaryInsured.Employment.Employer.PartyContactPoint.PhoneNumber.AsFormattedString
                                ();
                    }
                }
            }
        }

        private void SecondaryInsuranceVerificationDetails()
        {
            if ( account.Insurance != null )
            {
                Coverage coverage = SecondaryCoverage();
                if ( coverage != null )
                {
                    if ( coverage.BenefitsVerified != null )
                    {
                        secondaryBenefitsVerifiedValueLabel.Text = coverage.
                            BenefitsVerified.Description;
                    }
                    secondaryInitiatedByValueLabel.Text = coverage.AuthorizingPerson;
                    if ( coverage.DateTimeOfVerification != DateTime.MinValue )
                    {
                        secondaryDateValueLabel.Text = coverage.DateTimeOfVerification
                            .ToString( "MM/dd/yyyy" );
                    }
                    if ( coverage.GetType().IsSubclassOf( typeof( CoverageGroup ) ) )
                    {
                        CoverageGroup coverageGroup = coverage as CoverageGroup;
                        if ( coverageGroup.Authorization.AuthorizationRequired != null )
                        {
                            secondaryAuthorizationRequiredValueLabel.Text = coverageGroup.Authorization.
                                AuthorizationRequired.Description;
                        }
                        secondaryAuthorizationCompanyValueLabel.Text = coverageGroup.Authorization.AuthorizationCompany;
                        secondaryAuthorizationPhoneValueLabel.Text = coverageGroup.Authorization.AuthorizationPhone.ToString();
                        secondaryPromptExtValueLabel.Text = coverageGroup.Authorization.PromptExt;
                    }

                    if ( coverage.Attorney != null )
                    {
                        secondaryAttorneyNameValueLabel.Text = coverage.Attorney.FirstName;
                        if ( coverage.Attorney.ContactPointWith
                            ( TypeOfContactPoint.NewBusinessContactPointType() ).Address != null )
                        {
                            secondaryAttorneyAddressValueLabel.Text = coverage.Attorney.ContactPointWith
                                ( TypeOfContactPoint.NewBusinessContactPointType() ).Address.
                                OneLineAddressLabel();
                        }
                        if ( coverage.Attorney.ContactPointWith
                            ( TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber != null )
                        {
                            secondaryAttorneyPhoneNumberValueLabel.Text = coverage.Attorney.ContactPointWith
                                ( TypeOfContactPoint.NewBusinessContactPointType() ).
                                PhoneNumber.AsFormattedString();
                        }
                    }
                    if ( coverage.InsuranceAgent != null )
                    {
                        secondaryAutoHomeInsuranceAgentNameValueLabel.Text = coverage.InsuranceAgent.FirstName;
                        if ( coverage.InsuranceAgent.ContactPointWith
                            ( TypeOfContactPoint.NewBusinessContactPointType() ).Address != null )
                        {
                            secondaryAutoHomeInsuranceAgentAddressValueLabel.Text = coverage.InsuranceAgent.ContactPointWith
                                ( TypeOfContactPoint.NewBusinessContactPointType() ).Address.OneLineAddressLabel();
                        }
                        if ( coverage.InsuranceAgent.ContactPointWith
                            ( TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber != null )
                        {
                            secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Text = coverage.InsuranceAgent.ContactPointWith
                                ( TypeOfContactPoint.NewBusinessContactPointType() ).PhoneNumber.AsFormattedString();
                        }

                    }
                }
            }
        }

        private void SecondaryInsuranceAuthorizationDetails()
        {
            Coverage coverage = SecondaryCoverage();

            if ( coverage != null )
            {
                if ( coverage.GetType() == typeof( CommercialCoverage ) ||
                         coverage.GetType() == typeof( OtherCoverage ) ||
                         coverage.GetType() == typeof( GovernmentOtherCoverage ) ||
                         coverage.GetType() == typeof( GovernmentMedicaidCoverage ) ||
                         coverage.GetType() == typeof( WorkersCompensationCoverage ) )
                {
                    if ( coverage.GetType() == typeof( CommercialCoverage ) ||
                         coverage.GetType() == typeof( OtherCoverage ) ||
                              coverage.GetType() == typeof( GovernmentOtherCoverage ) )
                    {
                        secondaryAuthorizationTrackingNumberValueLabel.Text =
                            ( ( CoverageForCommercialOther )coverage ).TrackingNumber;
                    }

                    if (coverage.GetType() == typeof(GovernmentMedicaidCoverage))
                    {
                        secondaryAuthorizationTrackingNumberValueLabel.Text =
                            ((GovernmentMedicaidCoverage) coverage).TrackingNumber;
                    }

                    Authorization authorization = ( ( CoverageGroup )coverage ).Authorization;
                    if ( authorization != null )
                    {
                        secondaryAuthorizationNumberValueLabel.Text = authorization.AuthorizationNumber;
                        secondaryDaysAuthorizedValueLabel.Text = authorization.NumberOfDaysAuthorized.ToString();
                        secondaryAuthorizationCompanyRepresentativeNameValueLabel.Text =
                            String.Format( "{0}, {1}", authorization.NameOfCompanyRepresentative.FirstName.Trim(), authorization.NameOfCompanyRepresentative.LastName.Trim() );
                        secondaryServicesAuthorizedValueLabel.Text = authorization.
                            ServicesAuthorized;
                        secondaryEffectiveDateOfAuthorizationValueLabel.Text =
                            authorization.EffectiveDate.ToString( "MM/dd/yyyy" );
                        secondaryExpirationDateOfAuthorizationValueLabel.Text =
                            authorization.ExpirationDate.ToString( "MM/dd/yyyy" );
                        secondaryAuthorizationStatusValueLabel.Text = authorization.AuthorizationStatus.Description;
                        secondaryRemarksValueLabel.Text = authorization.Remarks;
                    }
                }

                SecondaryInsuranceAuthorizationForGovernmentmedicare53544Details(coverage);
            }
        }

        private void SecondaryInsuranceAuthorizationForGovernmentmedicare53544Details(Coverage coverage)
        {
            if (coverage != null)
            {
                if (coverage.GetType() == typeof(GovernmentMedicareCoverage))
                {
                    var secondaryMedicareCoverage = ((GovernmentMedicareCoverage) coverage);
                    if (secondaryMedicareCoverage != null &&
                        secondaryMedicareCoverage.IsMedicareCoverageValidForAuthorization)
                    {
                        var authorization = secondaryMedicareCoverage.Authorization;
                        secondaryAuthorizationTrackingNumberValueLabel.Text =
                            secondaryMedicareCoverage.TrackingNumber;

                        if (authorization != null)
                        {
                            secondaryAuthorizationNumberValueLabel.Text = authorization.AuthorizationNumber;
                            secondaryDaysAuthorizedValueLabel.Text = authorization.NumberOfDaysAuthorized.ToString();
                            secondaryAuthorizationCompanyRepresentativeNameValueLabel.Text =
                                String.Format("{0}, {1}", authorization.NameOfCompanyRepresentative.FirstName.Trim(),
                                    authorization.NameOfCompanyRepresentative.LastName.Trim());
                            secondaryServicesAuthorizedValueLabel.Text = authorization.ServicesAuthorized;
                            secondaryEffectiveDateOfAuthorizationValueLabel.Text =
                                authorization.EffectiveDate.ToString("MM/dd/yyyy");
                            secondaryExpirationDateOfAuthorizationValueLabel.Text =
                                authorization.ExpirationDate.ToString("MM/dd/yyyy");
                            secondaryAuthorizationStatusValueLabel.Text = authorization.AuthorizationStatus.Description;
                            secondaryRemarksValueLabel.Text = authorization.Remarks;
                        }
                    }
                }
            }
        }

        private void GuarantorDetails()
        {
            if ( account.Guarantor != null )
            {
                foreach ( Relationship relationship in account.Guarantor.Relationships )
                {
                    thePatientIsTheGuarantorsValueLabel.Text = relationship.Type.
                        Description;
                    break;
                }
                guarantorNameValueLabel.Text = account.Guarantor.
                    Name.AsFormattedNameWithSuffix();
               
                if ( account.Guarantor.SocialSecurityNumber != null )
                {
                    guarantorSSNValueLabel.Text = account.Guarantor.SocialSecurityNumber
                        .AsFormattedString();
                }
                if ( account.Guarantor.DriversLicense != null )
                {
                    usDriverLicenseValueLabel.Text =
                        account.Guarantor.DriversLicense.Number;
                }
                if ( account.Guarantor.DriversLicense.State != null )
                {
                    usDriverLicenseValueLabel.Text += " " +
                        account.Guarantor.DriversLicense.State.Code;
                }
                ContactPoint cp = account.Guarantor.ContactPointWith(
                    TypeOfContactPoint.NewMailingContactPointType() );

                ContactPoint mobileContactPoint = account.Guarantor.ContactPointWith(
                    TypeOfContactPoint.NewMobileContactPointType() );

                if ( cp.Address != null )
                {
                    guarantorAddressValueLabel.Text = cp.Address.OneLineAddressLabel();
                }
                if ( cp.PhoneNumber != null )
                {
                    guarantorContactPhoneValueLabel.Text =
                        cp.PhoneNumber.AsFormattedString();
                }
                if ( mobileContactPoint.PhoneNumber != null )
                {
                    guarantorContactCellValueLabel.Text =
                        mobileContactPoint.PhoneNumber.AsFormattedString();
                }
                if (mobileContactPoint.CellPhoneConsent != null)
                {
                    guarantorCellPhoneConsentValueLabel.Text =
                        mobileContactPoint.CellPhoneConsent.Description;
                }
                if (account.Guarantor.DateOfBirth != DateTime.MinValue)
                {
                    guarantorDOBValueLabel.Text = account.Guarantor.DateOfBirth.ToString("MM/dd/yyyy");
                }
            }
        }

        private void BillingDetails()
        {
            int occuranceCount = 0;
            int conditionCodesCount = 0;
            if ( account.OccurrenceCodes != null )
            {
                foreach ( OccurrenceCode occ in account.OccurrenceCodes )
                {
                    if ( occ != null )
                    {
                        occurrenceCodeValueLabel[occuranceCount].Text = occ.ToCodedString();
                        if ( occ.OccurrenceDate != DateTime.MinValue )
                        {
                            occurrenceCodeDateValueLabel[occuranceCount].Text =
                                occ.OccurrenceDate.ToString( "MM/dd/yyyy" );
                        }
                        occuranceCount++;
                    }
                }
            }

            if ( account.ConditionCodes != null )
            {
                foreach ( ConditionCode co in account.ConditionCodes )
                {
                    if ( co != null )
                    {
                        conditionCodeValueLabel[conditionCodesCount].Text = co.ToCodedString();
                        conditionCodesCount++;
                    }
                }
            }
            
        }

        private void LiabilityDetails()
        {
            decimal valueEstimatedForInsured = 0m;
            FinancialClassesBrokerProxy financialClassesBroker = new FinancialClassesBrokerProxy();
            bool isUninsured = financialClassesBroker.IsUninsured( account.Facility.Oid, account.FinancialClass );

            if ( account.Insurance != null )
            {
                patientHasNoLiabilityInsuredValueLabel.Text = account.Insurance.HasNoLiability.ToString();
                deductibleInsuredValueLabel.Text = DOLLAR_SIGN + account.Insurance.PrimaryDeductible;
                coPayCoInsInsuredValueLabel.Text = DOLLAR_SIGN + account.Insurance.PrimaryCopay;
                patientHasNoLiabilityUninsuredValueLabel.Text = account.Insurance.HasNoLiability.ToString();
            }
            if ( isUninsured )
                estimateForCurrentAmountDueUninsuredValueLabel.Text = DOLLAR_SIGN + account.TotalCurrentAmtDue;
            else
                estimateForCurrentAmountDueUninsuredValueLabel.Text = DOLLAR_SIGN + valueEstimatedForInsured;
        }

        private void PaymentDetails()
        {
            totalCurrentAmountDueValueLabel.Text = DOLLAR_SIGN + account.TotalCurrentAmtDue;
            totalPaymentsCollectedForCurrentAccountValueLabel.Text = DOLLAR_SIGN + account.TotalPaid;
            numberOfMonthlyPaymentsValueLabel.Text = account.NumberOfMonthlyPayments.ToString();

            decimal monthlyPayment = 0m;
            if (account.NumberOfMonthlyPayments != 0)
            {
                monthlyPayment = (account.TotalCurrentAmtDue - account.TotalPaid)/account.NumberOfMonthlyPayments;
                monthlyPaymentValueLabel.Text = DOLLAR_SIGN + monthlyPayment.ToString("###,###,##0.00");
            }
            else
            {
                monthlyPaymentValueLabel.Text = DOLLAR_SIGN + monthlyPayment;
            }
            if (!String.IsNullOrEmpty(account.DayOfMonthPaymentDue) && account.DayOfMonthPaymentDue != "0")
            {
                monthlyDueDateValueLabel.Text = account.DayOfMonthPaymentDue;
            }
            else
            {
                monthlyDueDateValueLabel.Text = String.Empty;
            }
        }

        private void ContactsDetails()
        {
            emergencyContact1NameValueLabel.Text = account.EmergencyContact1.Name;
            foreach ( Relationship relatioship in account.EmergencyContact1.Relationships )
            {
                emergencyContact1IsThePatientsValueLabel.Text = relatioship.Type.
                    Description;
                break;
            }

            ContactPoint cp1 = account.EmergencyContact1.ContactPointWith
                ( TypeOfContactPoint.NewPhysicalContactPointType() );
            if ( cp1.Address != null )
            {
                emergencyContact1AddressValueLabel.Text = cp1.Address.OneLineAddressLabel();
            }
            if ( cp1.PhoneNumber != null )
            {
                emergencyContact1PhoneValueLabel.Text = cp1.PhoneNumber.AsFormattedString();
            }
            emergencyContact2NameValueLabel.Text = account.EmergencyContact2.Name;
            foreach ( Relationship relatioship in account.EmergencyContact2.Relationships )
            {
                emergencyContact2IsThePatientsValueLabel.Text = relatioship.Type.
                    Description;
                break;
            }
            ContactPoint cp2 = account.EmergencyContact2.ContactPointWith
                ( TypeOfContactPoint.NewPhysicalContactPointType() );
            if ( cp2.Address != null )
            {
                emergencyContact2AddressValueLabel.Text = cp2.Address.OneLineAddressLabel();
            }
            if ( cp2.PhoneNumber != null )
            {
                emergencyContact2PhoneValueLabel.Text = cp2.PhoneNumber.AsFormattedString();
            }
        }
        private void RegulatoryDetails()
        {
            if (account.Patient.NoticeOfPrivacyPracticeDocument != null &&
                account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion != null)
            {
                nppVersionValueLabel.Text = account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion.DisplayString;
                if (account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus != null)
                {
                    if (account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus.Code == SignatureStatus.SIGNED &&
                        account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate != DateTime.MinValue)
                    {
                        nppSignatureStatusValueLabel.Text =
                            account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus.Description.Trim()
                            + LABEL_SPACE
                            + account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate.ToShortDateString();
                    }
                    else
                    {
                        nppSignatureStatusValueLabel.Text =
                            account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus.Description.Trim();
                    }
                }
            }
            if (account.ConfidentialityCode != null)
            {
                privacyConfidentialStatusValueLabel.Text = account.
                    ConfidentialityCode.Description;
            }
            privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.Text
                = account.OptOutName.ToString();
            privacyOptOutPatientDirectoryLocationValueLabel.Text
                = account.OptOutLocation.ToString();
            privacyOptOutPatientDirectoryHealthInformationValueLabel.Text
                = account.OptOutHealthInformation.ToString();
            privacyOptOutPatientDirectoryReligionValueLabel.Text
                = account.OptOutReligion.ToString();
            var rightToRestrictFeatureManager = new RightToRestrictFeatureManager();
            if ( account.RightToRestrict != null &&
                rightToRestrictFeatureManager.IsRightToRestrictEnabledForAccountCreatedDate(
                    account.AccountCreatedDate ))
            {
                rightToRestrictValueLabel.Text = account.RightToRestrict.IsYes
                    ? account.RightToRestrict.Description
                    : YesNoFlag.DESCRIPTION_NO;
            }
            if (account.COSSigned != null)
            {
                cosSignedValueLabel.Text = account.COSSigned.Description;
            }
            if (account.ShareDataWithPublicHieFlag != null)
            {
                ShareDataWithPublicHIEValueLabel.Text = account.ShareDataWithPublicHieFlag.Description;
            }
            if (account.ShareDataWithPCPFlag != null)
            {
                ShareDataWithPCPValueLabel.Text = account.ShareDataWithPCPFlag.Description;
            }
            var HospCommOptInFeatureManager = new HospitalCommunicationOptInFeatureManager();
            if (account.Patient.HospitalCommunicationOptIn != null &&
                HospCommOptInFeatureManager.ShouldFeatureBeEnabled(account))
            {
                HospCommoptinValueLabel.Text = account.Patient.HospitalCommunicationOptIn.Description;
            }
            else
            {
                HospCommoptinValueLabel.Text = String.Empty;
            }
            var cobReceivedAndIMFMReceivedFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            if (account.COBReceived != null &&
                cobReceivedAndIMFMReceivedFeatureManager.IsCOBReceivedEnabledForAccount(account))
            {
                cobReceivedValueLabel.Text = account.COBReceived.Description;
            }
            else
            {
                cobReceivedValueLabel.Text = string.Empty;
            }
            if (account.IMFMReceived != null &&
                cobReceivedAndIMFMReceivedFeatureManager.IsIMFMReceivedEnabledForAccount(account))
            {
                imfmReceivedValueLabel.Text = account.IMFMReceived.Description;
            }
            else
            {
                imfmReceivedValueLabel.Text = string.Empty;
            }
        }

        private Coverage PrimaryCoverage()
        {
            Coverage primaryCoverage = account.Insurance.CoverageFor
                ( CoverageOrder.PRIMARY_OID );
            return primaryCoverage;
        }
        private Coverage SecondaryCoverage()
        {
            Coverage secondaryCoverage = account.Insurance.CoverageFor
                ( CoverageOrder.SECONDARY_OID );
            return secondaryCoverage;
        }

        private void PopulateOnsetDate()
        {
            foreach ( OccurrenceCode occ in this.account.OccurrenceCodes )
            {
                if ( occ.Code == ONSET_OF_SYMPTOMS_ILLNESS )
                {
                    if ( occ.OccurrenceDate != DateTime.MinValue )
                    {
                        this.dateOfOnsetForSymptomsIllnessValueLabel.Text = occ.OccurrenceDate.ToString( "MM/dd/yyyy" );
                    }
                    break;
                }
            }
        }
        #endregion

        #region Data Elements

        private Label patientDemographicsAndEmploymentLabel;
        private Label patientNameLabel;
        private Label patientNameValueLabel;
        private Label akaLabel;
        private Label akaValueLabel;
        private Label admitDateTimeLabel;
        private Label admitDateTimeValueLabel;
        private Label appointmentLabel;
        private Label appointmentValueLabel;
        private Label genderLabel;
        private Label genderValueLabel;
        private Label dobLabel;
        private Label dobValueLabel;
        private Label ageLabel;
        private Label ageValueLabel;
        private Label maritalStatusLabel;
        private Label maritalStatusValueLabel;
        private Label raceLabel;
        private Label raceValueLabel;
        private Label ethnicityLabel;
        private Label ethnicityValueLabel;
        private Label ssnLabel;
        private Label ssnValueLabel;
        private Label patientMailingAddressLabel;
        private Label patientMailingAddressValueLabel;
        private Label contactPhoneLabel;
        private Label contactPhoneValueLabel;
        private Label contactCellLabel;
        private Label contactCellValueLabel;
        private Label driversLicenseLabel;
        private Label driversLicenseValueLabel;
        private Label passportLabel;
        private Label passportValueLabel;
        private Label languageLabel;
        private Label languageValueLabel;
        private Label patientTypeLabel;
        private Label patientTypeValueLabel;
        private Label reregisterLabel;
        private Label reregisterValueLabel;
        private Label hospitalServiceLabel;
        private Label hospitalServiceValueLabel;
        private Label chiefComplaintLabel;
        private Label chiefComplaintValueLabel;
        private Label patientVisitLabel;
        private Label patientVisitValueLabel;
        private Label accidentTypeLabel;
        private Label accidentTypeValueLabel;
        private Label accidentCrimeDateLabel;
        private Label accidentCrimeDateValueLabel;
        private Label accidentCrimeHourLabel;
        private Label accidentCrimeCountryLabel;
        private Label accidentCrimeStateProvinceLabel;
        private Label dateOfOnsetForSymptomsIllnessLabel;
        private Label admitSourceLabel;
        private Label clinic1Label;
        private Label clinic2Label;
        private Label accidentCrimeHourValueLabel;
        private Label accidentCrimeCountryValueLabel;
        private Label accidentCrimeStateProvinceValueLabel;
        private Label dateOfOnsetForSymptomsIllnessValueLabel;
        private Label admitSourceValueLabel;
        private Label clinic3Label;
        private Label clinic4Label;
        private Label clinic5Label;
        private Label clinic1ValueLabel;
        private Label clinic2ValueLabel;
        private Label clinic3ValueLabel;
        private Label clinic4ValueLabel;
        private Label clinic5ValueLabel;
        private Label referringPhysicianLabel;
        private Label admittingPhysicianLabel;
        private Label attendingPhysicianLabel;
        private Label referringPhysicianValueLabel;
        private Label admittingPhysicianValueLabel;
        private Label attendingPhysicianValueLabel;
        private Label primaryCarePhysicianLabel;
        private Label primaryCarePhysicianValueLabel;
        private Label generalInsuranceInformationLabel;
        private Label financialClassLabel;
        private Label motherDobLabel;
        private Label fatherDobLabel;
        private Label financialClassValueLabel;
        private Label motherDobValueLabel;
        private Label fatherDobValueLabel;
        private Label mspLabel;
        private Label primaryInsurancePlanLabel;
        private Label primaryInsurancePayorDetailsLabel;
        private Label mspQuestionnaireSummaryConclusionLabel;
        private Label primaryInsuranceInsuredLabel;
        private Label primaryInsuranceVerificationLabel;
        private Label secondaryInsurancePlanLabel;
        private Label secondaryInsurancePayorDetailsLabel;
        private Label primaryBillingAddressValueLabel;
        private Label primaryBillingPhoneValueLabel;
        private Label primaryBillingNameValueLabel;
        private Label primaryBillingCoNameValueLabel;
        private Label primaryMedicalGroupOrIPAClinicValueLabel;
        private Label primaryMedicalGroupOrIpaNameValueLabel;
        private Label primaryAdjusterValueLabel;
        private Label primaryEmployeeSupervisorValueLabel;
        private Label primaryMedicaidIssueDateValueLabel;
        private Label primaryPrecertificationNumberValueLabel;
        private Label primaryGroupNumberValueLabel;
        private Label primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel;
        private Label primaryBillingAddressLabel;
        private Label primaryBillingPhoneLabel;
        private Label primaryBillingNameLabel;
        private Label primaryBillingCoNameLabel;
        private Label primaryMedicalGroupOrIPAClinicLabel;
        private Label primaryMedicalGroupOrIpaNameLabel;
        private Label primaryAdjusterLabel;
        private Label primaryEmployeeSupervisorLabel;
        private Label primaryMedicaidIssueDateLabel;
        private Label primaryPrecertificationNumberLabel;
        private Label primaryGroupNumberLabel;
        private Label primaryCategoryValueLabel;
        private Label primaryPayorBrokerValueLabel;
        private Label primaryPlanValueLabel;
        private Label primaryCategoryLabel;
        private Label primaryPayorBrokerLabel;
        private Label primaryPlanLabel;
        private Label primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel;
        private Label primaryNameLabel;
        private Label primaryThePatientIsTheInsuredsLabel;
        private Label primaryGenderLabel;
        private Label primaryNameValueLabel;
        private Label primaryThePatientIsTheInsuredsValueLabel;
        private Label primaryGenderValueLabel;
        private Label secondaryInsuranceInsuredLabel;
        private Label secondaryInsuranceVerificationLabel;
        private Label guarantorLabel;
        private Label primaryContactCellValueLabel;
        private Label primaryContactPhoneValueLabel;
        private Label primaryAddressValueLabel;
        private Label primaryNationalIdValueLabel;
        private Label primaryDobValueLabel;
        private Label primaryEmploymentStatusLabel;
        private Label primaryContactCellLabel;
        private Label primaryContactPhoneLabel;
        private Label primaryAddressLabel;
        private Label primaryNationalIdLabel;
        private Label primaryDobLabel;
        private Label secondaryCategoryValueLabel;
        private Label secondaryPayorBrokerValueLabel;
        private Label secondaryPlanValueLabel;
        private Label secondaryCategoryLabel;
        private Label secondaryPayorBrokerLabel;
        private Label secondaryPlanLabel;
        private Label primaryAutoHomeInsuranceAgentPhoneNumberValueLabel;
        private Label primaryAutoHomeInsuranceAgentAddressValueLabel;
        private Label primaryAutoHomeInsuranceAgentNameValueLabel;
        private Label primaryAttorneyPhoneNumberValueLabel;
        private Label primaryAttorneyAddressValueLabel;
        private Label primaryAttorneyNameValueLabel;
        private Label primaryPromptExtValueLabel;
        private Label primaryAuthorizationPhoneValueLabel;
        private Label primaryAuthorizationCompanyValueLabel;
        private Label primaryAuthorizationRequiredValueLabel;
        private Label primaryDateValueLabel;
        private Label primaryInitiatedByValueLabel;
        private Label primaryBenefitsVerifiedValueLabel;
        private Label primaryAutoHomeInsuranceAgentPhoneNumberLabel;
        private Label primaryAutoHomeInsuranceAgentAddressLabel;
        private Label primaryAutoHomeInsuranceAgentNameLabel;
        private Label primaryAttorneyPhoneNumberLabel;
        private Label primaryAttorneyAddressLabel;
        private Label primaryAttorneyNameLabel;
        private Label primaryPromptExtLabel;
        private Label primaryAuthorizationPhoneLabel;
        private Label primaryAuthorizationCompanyLabel;
        private Label primaryAuthorizationRequiredLabel;
        private Label primaryDateLabel;
        private Label primaryInitiatedByLabel;
        private Label primaryBenefitsVerifiedLabel;
        private Label primaryEmployerPhoneValueLabel;
        private Label primaryEmployerValueLabel;
        private Label primaryEmploymentStatusValueLabel;
        private Label primaryEmployerPhoneLabel;
        private Label primaryEmployerLabel;
        private Label secondaryAdjusterValueLabel;
        private Label secondaryEmployeeSupervisorValueLabel;
        private Label secondaryMedicaidIssueDateValueLabel;
        private Label secondaryPrecertificationNumberValueLabel;
        private Label secondaryGroupNumberValueLabel;
        private Label secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel;
        private Label secondaryMedicalGroupIpaNameLabel;
        private Label secondaryAdjusterLabel;
        private Label secondaryEmployeeSupervisorLabel;
        private Label secondaryMedicaidIssueDateLabel;
        private Label secondaryPrecertificationNumberLabel;
        private Label secondaryGroupNumberLabel;
        private Label secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel;
        private Label secondaryNameValueLabel;
        private Label secondaryNameLabel;
        private Label secondaryBillingPhoneValueLabel;
        private Label secondaryBillingNameValueLabel;
        private Label secondaryBillingCoNameValueLabel;
        private Label secondaryMedicalGroupIpaClinicValueLabel;
        private Label secondaryMedicalGroupIpaNameValueLabel;
        private Label secondaryBillingPhoneLabel;
        private Label secondaryBillingNameLabel;
        private Label secondaryBillingCoNameLabel;
        private Label secondaryMedicalGroupIpaClinicLabel;
        private Label secondaryAuthorizationPhoneValueLabel;
        private Label secondaryAuthorizationCompanyValueLabel;
        private Label secondaryAuthorizationRequiredValueLabel;
        private Label secondaryDateValueLabel;
        private Label secondaryInitiatedByValueLabel;
        private Label secondaryBenefitsVerifiedValueLabel;
        private Label secondaryAuthorizationPhoneLabel;
        private Label secondaryAuthorizationCompanyLabel;
        private Label secondaryAuthorizationRequiredLabel;
        private Label secondaryDateLabel;
        private Label secondaryInitiatedByLabel;
        private Label secondaryBenefitsVerifiedLabel;
        private Label secondaryEmployerPhoneValueLabel;
        private Label secondaryEmployerValueLabel;
        private Label secondaryEmploymentStatusValueLabel;
        private Label secondaryContactCellValueLabel;
        private Label secondaryContactPhoneValueLabel;
        private Label secondaryAddressValueLabel;
        private Label secondaryNationalIdValueLabel;
        private Label secondaryDobValueLabel;
        private Label secondaryGenderValueLabel;
        private Label secondaryThePatientIsTheInsuredValueLabel;
        private Label secondaryEmployerPhoneLabel;
        private Label secondaryEmployerLabel;
        private Label secondaryEmploymentStatusLabel;
        private Label secondaryContactCellLabel;
        private Label secondaryContactPhoneLabel;
        private Label secondaryAddressLabel;
        private Label secondaryNationalIdLabel;
        private Label secondaryDobLabel;
        private Label secondaryGenderLabel;
        private Label secondaryThePatientIsTheInsuredLabel;
        private Label secondaryPromptExtLabel;
        private Label secondaryAutoHomeInsuranceAgentPhoneNumberLabel;
        private Label secondaryAutoHomeInsuranceAgentAddressLabel;
        private Label secondaryAutoHomeInsuranceAgentNameLabel;
        private Label secondaryAttorneyPhoneNumberLabel;
        private Label secondaryAttorneyAddressLabel;
        private Label secondaryAttorneyNameLabel;
        private Label secondaryPromptExtValueLabel;
        private Label secondaryAttorneyNameValueLabel;
        private Label secondaryAttorneyAddressValueLabel;
        private Label secondaryAttorneyPhoneNumberValueLabel;
        private Label secondaryAutoHomeInsuranceAgentNameValueLabel;
        private Label secondaryAutoHomeInsuranceAgentAddressValueLabel;
        private Label secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel;

        private Label billingLabel;
        private Label liabilityLabel;
        private Label guarantorNameLabel;
        private Label thePatientIsTheGuarantorsLabel;
        private Label guarantorSSNLabel;
        private Label usDriverLicenseLabel;
        private Label guarantorAddressLabel;
        private Label guarantorContactPhoneLabel;
        private Label guarantorContactCellLabel;
        private Label guarantorNameValueLabel;
        private Label thePatientIsTheGuarantorsValueLabel;
        private Label guarantorSSNValueLabel;
        private Label usDriverLicenseValueLabel;
        private Label guarantorAddressValueLabel;
        private Label guarantorContactPhoneValueLabel;
        private Label guarantorContactCellValueLabel;
        private Label occurrenceCode1Label;
        private Label occurrenceCode2Label;
        private Label occurrenceCode3Label;
        private Label occurrenceCode4Label;
        private Label occurrenceCode5Label;
        private Label occurrenceCode6Label;
        private Label occurrenceCode7Label;
        private Label occurrenceCode8Label;
        private Label conditionCode1Label;
        private Label conditionCode2Label;
        private Label conditionCode3Label;
        private Label conditionCode4Label;
        private Label conditionCode5Label;
        private Label conditionCode6Label;
        private Label conditionCode7Label;
        private Label patientHasNoLiabilityInsuredLabel;
        private Label deductibleInsuredLabel;
        private Label coPayCoInsInsuredLabel;
        private Label patientHasNoLiabilityUninsuredLabel;
        private Label estimateForCurrentAmountDueUninsuredLabel;
        private Label paymentLabel;
        private Label totalCurrentAmountDueLabel;
        private Label totalPaymentsCollectedForCurrentAccountLabel;
        private Label numberOfMonthlyPaymentsLabel;
        private Label monthlyPaymentLabel;
        private Label emergencyContact1NameLabel;
        private Label emergencyContact1IsThePatientsLabel;
        private Label emergencyContact1AddressLabel;
        private Label emergencyContact1PhoneLabel;
        private Label emergencyContact2AddressLabel;
        private Label emergencyContact2PhoneLabel;
        private Label regulatoryLabel;
        private Label nppVersionLabel;
        private Label privacyConfidentialStatusLabel;
        private Label privacyOptOutPatientDirectoryNameAndAllInformationLabel;
        private Label privacyOptOutPatientDirectoryLocationLabel;
        private Label privacyOptOutPatientDirectoryHealthInformationLabel;
        private Label privacyOptOutPatientDirectoryReligionLabel;
        private Label patientHasNoLiabilityInsuredValueLabel;
        private Label deductibleInsuredValueLabel;
        private Label coPayCoInsInsuredValueLabel;
        private Label patientHasNoLiabilityUninsuredValueLabel;
        private Label estimateForCurrentAmountDueUninsuredValueLabel;
        private Label totalCurrentAmountDueValueLabel;
        private Label totalPaymentsCollectedForCurrentAccountValueLabel;
        private Label numberOfMonthlyPaymentsValueLabel;
        private Label monthlyPaymentValueLabel;
        private Label emergencyContact1NameValueLabel;
        private Label emergencyContact1IsThePatientsValueLabel;
        private Label emergencyContact1AddressValueLabel;
        private Label emergencyContact1PhoneValueLabel;
        private Label emergencyContact2NameValueLabel;
        private Label emergencyContact2IsThePatientsValueLabel;
        private Label emergencyContact2AddressValueLabel;
        private Label emergencyContact2PhoneValueLabel;
        private Label nppVersionValueLabel;
        private Label privacyConfidentialStatusValueLabel;
        private Label privacyOptOutPatientDirectoryNameAndAllInformationValueLabel;
        private Label privacyOptOutPatientDirectoryLocationValueLabel;
        private Label privacyOptOutPatientDirectoryHealthInformationValueLabel;
        private Label privacyOptOutPatientDirectoryReligionValueLabel;
        private Label secondaryBillingAddressValueLabel;
        private Label secondaryBillingAddressLabel;
        private Panel NonPurgedAccountDetailsPanel;
        private Label dischargeDateTimeLabel;
        private Label dischargeDateTimeValueLabel;
        private Label dischargeDispositionLabel;
        private Label dischargeDispositionValueLabel;
        private Label diagnosisLabel;

        private Label[] occurrenceCodeValueLabel;
        private Label[] conditionCodeValueLabel;
        private Label[] occurrenceCodeDateValueLabel;

        private Label nppSignatureStatusLabel;
        private Label nppSignatureStatusValueLabel;
        private Panel diagnosisPanel;
        private Container components = null;
        private Panel patientDemographicsEmploymentPanel;

        private Label occurrenceCode1DateLabel;
        private Label occurrenceCode2DateLabel;
        private Label occurrenceCode3DateLabel;
        private Label occurrenceCode4DateLabel;
        private Label occurrenceCode6DateLabel;
        private Label occurrenceCode5DateLabel;
        private Label occurrenceCode7DateLabel;
        private Label occurrenceCode8DateLabel;

        private Account account = new Account();
        private Patient patient = new Patient();
        private Authorization authorization = new Authorization();
        private int i_scrollOffset = 0;

        #endregion

        #region Constants

        private const int FIRST_ALIAS = 0;
        private const string PRIMARY_MSP =
        "Based on questions and responses, Medicare is the primary payor.";
        private const string NOT_PRIMARY_MSP =
        "Based on questions and responses, Medicare is not the primary payor.";
        private const string NO_MSP = "No MSP summary is available.";
        private const string DOLLAR_SIGN = "$";
        private const string ONSET_OF_SYMPTOMS_ILLNESS = "11";
        private Label primaryAuthorizationStatusValueLabel;
        private Label primaryExpirationDateOfAuthorizationValueLabel;
        private Label primaryEffectiveDateOfAuthorizationValueLabel;
        private Label primaryAuthorizationCompanyRepresentativeNameValueLabel;
        private Label primaryAuthorizationTrackingNumberValueLabel;
        private Label primaryDaysAuthorizedValueLabel;
        private Label primaryAuthorizationNumberValueLabel;
        private Label primaryAuthorizationStatusLabel;
        private Label primaryExpirationDateOfAuthorizationLabel;
        private Label primaryEffectiveDateOfAuthorizationLabel;
        private Label primaryAuthorizationCompanyRepresentativeNameLabel;
        private Label primaryTrackingNumberLabel;
        private Label primaryDaysAuthorizedLabel;
        private Label primaryAuthorizationNumberLabel;
        private Label primaryInsuranceAuthorizationLabel;
        private Label primaryRemarksValueLabel;
        private Label primaryRemarksLabel;
        private Label primaryServicesAuthorizedValueLabel;
        private Label primaryServicesAuthorizedLabel;
        private Label secondaryServicesAuthorizedValueLabel;
        private Label secondaryServicesAuthorizedLabel;
        private Label secondaryRemarksValueLabel;
        private Label secondaryRemarksLabel;
        private Label secondaryAuthorizationStatusValueLabel;
        private Label secondaryExpirationDateOfAuthorizationValueLabel;
        private Label secondaryEffectiveDateOfAuthorizationValueLabel;
        private Label secondaryAuthorizationCompanyRepresentativeNameValueLabel;
        private Label secondaryAuthorizationTrackingNumberValueLabel;
        private Label secondaryDaysAuthorizedValueLabel;
        private Label secondaryAuthorizationNumberValueLabel;
        private Label secondaryAuthorizationStatusLabel;
        private Label secondaryExpirationDateOfAuthorizationLabel;
        private Label secondaryEffectiveDateOfAuthorizationLabel;
        private Label secondaryAuthorizationCompanyRepresentativeNameLabel;
        private Label secondaryAuthorizationTrackingNumberLabel;
        private Label secondaryDaysAuthorizedLabel;
        private Label secondaryAuthorizationNumberLabel;
        private Label secondaryInsuranceAuthorizationLabel;
        private Label procedureValueLabel;
        private Label procedureLabel;
        private Label PatientInClinicalREsearchLBL;
        private Label PatientInClinicalStudyValueLabel;
        private Label LeftOrStayedValueLabel;
        private Label RCRPValueLabel;
        private Label LeftWithoutBeingSeenValueLabel;
        private Label LeftWithoutFinancialClearanceValueLabel;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridView dataGridView2;
        private DataGridViewTextBoxColumn PatientResearchStudyCode;
        private DataGridViewTextBoxColumn PatientStudyDescription;
        private DataGridViewTextBoxColumn PatientResearchSponsor;
        private DataGridViewTextBoxColumn PatientRegistryNumber;
        private DataGridViewTextBoxColumn ProofOfConsent;
        private TableLayoutPanel demographicsBody;
        private TableLayoutPanel dempgraphicsHeader;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private TableLayoutPanel accountView;
        private TableLayoutPanel diangnosisAndClinicalHeader;
        private TableLayoutPanel diagnosisAndClinicalBody;
        private TableLayoutPanel regulatoryBody;
        
        private TableLayoutPanel regulatoryHeader;
        private Panel panel1;
        private TableLayoutPanel PaymentHeader;
        
        private TableLayoutPanel paymentBody;
        private Panel panel2;
        private TableLayoutPanel liabilityBody;
        private TableLayoutPanel liabilityHeader;
        private Panel panel3;
        private TableLayoutPanel guarantorBody;
        private TableLayoutPanel guarantorHeader;
        private Panel panel4;
        private TableLayoutPanel secondaryInsuranceAuthorizationHeader;
        private Panel panel5;
        private TableLayoutPanel secondaryInsuranceAuthorizationBody;
        private TableLayoutPanel secondaryInsuranceVerificationHeader;
        private Panel panel6;
        private TableLayoutPanel secondaryInsuranceVerificationBody;
        private TableLayoutPanel secondaryInsuranceInsuredBody;
        private TableLayoutPanel secondaryInsuranceInsuredHeader;
        private Panel panel7;
        private TableLayoutPanel secondaryInsurancePayorDetailsBody;
        private TableLayoutPanel SecondaryInsurancePayorHeader;
        private Panel panel8;
        private TableLayoutPanel secondaryInsurancePlanBody;
        private TableLayoutPanel secondaryInsurancePlanHeader;
        private Panel panel9;
        private TableLayoutPanel primaryInsuranceAuthorizationBody;
        private TableLayoutPanel primaryInsuranceAuthorizatioHeader;
        private Panel panel10;
        private TableLayoutPanel primaryInsuranceVerificationBody;
        private TableLayoutPanel primaryInsuranceVerificationHeader;
        private Panel panel11;
        private TableLayoutPanel primaryInsuranceInsuredHeader;
        private Panel panel12;
        private TableLayoutPanel primaruInsuranceInsuredBody;
        private TableLayoutPanel primaryInsurancePayorHeader;
        private Panel panel13;
        private TableLayoutPanel primaryInsurancePayorBody;
        private TableLayoutPanel primaryInsurancePlanBody;
        private TableLayoutPanel primaryInsurancePlanHeader;
        private Panel panel14;
        private TableLayoutPanel primaryInsuranceMSPBody;
        private TableLayoutPanel primaryInsuranceMSPHeader;
        private Panel panel15;
        private TableLayoutPanel generalInsuranceBody;
        private TableLayoutPanel generalInsuranceInformationHeader;
        private Panel panel16;
        private TableLayoutPanel billingHeader;
        private Panel panel18;
        private TableLayoutPanel tableLayoutPanel6;
        private Panel panel17;
        private TableLayoutPanel billingBody;
        private Label passportIssuingCountryValueLabel;
        private Label label1;
        private Label commentsLabel;
        private Label commentsValueLabel;
        private Label cosSignedLabel;
        private Label cosSignedValueLabel;
        private Label rightToRestrictLabel;
        private Label rightToRestrictValueLabel;
        private Label CptCodesValueLabel;
        private Label CptCodesLabel;
        private Label guarantorCellPhoneConsentLabel;
        private Label guarantorCellPhoneConsentValueLabel;
        private Label ShareDataWithPublicHIEFlagLabel;
        private Label ShareDataWithPCPFlagLabel;
        private Label ShareDataWithPublicHIEValueLabel;
        private Label ShareDataWithPCPValueLabel;
        private Label HospCommoptinlLabel;
        private Label HospCommoptinValueLabel;
        private Label cobReceivedFlagLabel;
        private Label cobReceivedValueLabel;
        private Label imfmReceivedFlagLabel;
        private Label imfmReceivedValueLabel;
        private Label contactEmailLabel;
        private Label contactEmailValueLabel;
       
        private const string LABEL_SPACE = "  ";
        private Label monthlyDueDateLabel;
        private Label guarantorDOBValueLabel;
        private Label guarantorDOBLabel;
        //private Label primaryMBIValueLabel;
        //private Label primaryMBILabel;
        //private Label secondaryMBIValueLabel;
        //private Label secondaryMBILabel;
        private Label monthlyDueDateValueLabel;
       
        #endregion
         
      

     }
}
