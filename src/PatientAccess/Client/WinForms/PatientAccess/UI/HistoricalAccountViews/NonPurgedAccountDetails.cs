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
using PatientAccess.Domain.Specialized;
using PatientAccess.Rules;

namespace PatientAccess.UI.HistoricalAccountViews
{
    /// <summary>
    /// Summary description for PatientDemographicsEmployment.
    /// </summary>
    public class NonPurgedAccountDetails : ControlView
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

        public NonPurgedAccountDetails()
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
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.conditionCodeValueLabel[6] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.conditionCodeValueLabel[5] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.conditionCodeValueLabel[4] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.conditionCodeValueLabel[3] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.conditionCodeValueLabel[2] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.conditionCodeValueLabel[1] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.conditionCodeValueLabel[0] );

            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeValueLabel[7] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeValueLabel[6] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeValueLabel[5] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeValueLabel[4] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeValueLabel[3] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeValueLabel[2] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeValueLabel[1] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeValueLabel[0] );

            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeDateValueLabel[7] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeDateValueLabel[6] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeDateValueLabel[5] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeDateValueLabel[4] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeDateValueLabel[3] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeDateValueLabel[2] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeDateValueLabel[1] );
            this.NonPurgedAccountDetailsPanel.Controls.Add( this.occurrenceCodeDateValueLabel[0] );
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
            this.guarantorDOBValuelabel = new System.Windows.Forms.Label();
            this.guarantorDOBLabel = new System.Windows.Forms.Label();
            this.NonPurgedAccountDetailsPanel = new System.Windows.Forms.Panel();
            this.HospCommoptinlLabel = new System.Windows.Forms.Label();
            this.monthlyDueDateValueLabel = new System.Windows.Forms.Label();
            this.monthlyDueDateLabel = new System.Windows.Forms.Label();
            this.shareDataWithPCPFlagLabel = new System.Windows.Forms.Label();
            this.shareDataWithPCPValueLabel = new System.Windows.Forms.Label();
            this.cobReceivedFlagLabel = new System.Windows.Forms.Label();
            this.cobReceivedValueLabel = new System.Windows.Forms.Label();
            this.imfmReceivedFlagLabel = new System.Windows.Forms.Label();
            this.imfmReceivedValueLabel = new System.Windows.Forms.Label();
            this.shareDataWithPublicHieFlagLabel = new System.Windows.Forms.Label();
            this.shareDataWithPublicHieValueLabel = new System.Windows.Forms.Label();
            this.HospCommoptinValueLabel = new System.Windows.Forms.Label();
            this.guarantorCellPhoneConsentValueLabel = new System.Windows.Forms.Label();
            this.guarantorCellPhoneConsentLabel = new System.Windows.Forms.Label();
            this.CptCodesValueLabel = new System.Windows.Forms.Label();
            this.CptCodesLabel = new System.Windows.Forms.Label();
            this.rightToRestrictLabel = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.PatientResearchStudyCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientStudyDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientResearchSponsor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientRegistryNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProofOfConsent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LeftWithoutFinancialClearanceValueLabel = new System.Windows.Forms.Label();
            this.LeftWithoutFinancialClearanceLabel = new System.Windows.Forms.Label();
            this.LeftWithoutBeingSeenValueLabel = new System.Windows.Forms.Label();
            this.LeftWithoutBeingSeenLabel = new System.Windows.Forms.Label();
            this.RCRPValueLabel = new System.Windows.Forms.Label();
            this.RCRPLabel = new System.Windows.Forms.Label();
            this.LeftOrStayedValueLabel = new System.Windows.Forms.Label();
            this.LeftOrStayedLabel = new System.Windows.Forms.Label();
            this.alternatecareFacilityValueLabel = new System.Windows.Forms.Label();
            this.AlternatecareFacilityLabel = new System.Windows.Forms.Label();
            this.PatientInClinicalStudyValueLabel = new System.Windows.Forms.Label();
            this.PatientInClinicalREsearchLBL = new System.Windows.Forms.Label();
            this.procedureValueLabel = new System.Windows.Forms.Label();
            this.procedureLabel = new System.Windows.Forms.Label();
            this.preopDateValueLabel = new System.Windows.Forms.Label();
            this.preopDateLabel = new System.Windows.Forms.Label();
            this.clinicalTrialValueLabel = new System.Windows.Forms.Label();
            this.clinicalTrialLabel = new System.Windows.Forms.Label();
            this.secondaryServicesAuthorizedValueLabel = new System.Windows.Forms.Label();
            this.secondaryServicesAuthorizedLabel = new System.Windows.Forms.Label();
            this.secondaryRemarksValueLabel = new System.Windows.Forms.Label();
            this.secondaryRemarksLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceAuthorizationPanel = new System.Windows.Forms.Panel();
            this.secondaryAuthorizationStatusValueLabel = new System.Windows.Forms.Label();
            this.secondaryExpirationDateOfAuthorizationValueLabel = new System.Windows.Forms.Label();
            this.secondaryEffectiveDateOfAuthorizationValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationTrackingNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryDaysAuthorizedValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationStatusLabel = new System.Windows.Forms.Label();
            this.secondaryExpirationDateOfAuthorizationLabel = new System.Windows.Forms.Label();
            this.secondaryEffectiveDateOfAuthorizationLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationCompanyRepresentativeNameLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationTrackingNumberLabel = new System.Windows.Forms.Label();
            this.secondaryDaysAuthorizedLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationNumberLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceAuthorizationLabel = new System.Windows.Forms.Label();
            this.primaryServicesAuthorizedValueLabel = new System.Windows.Forms.Label();
            this.primaryServicesAuthorizedLabel = new System.Windows.Forms.Label();
            this.primaryRemarksValueLabel = new System.Windows.Forms.Label();
            this.primaryRemarksLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceAuthorizationPanel = new System.Windows.Forms.Panel();
            this.primaryAuthorizationStatusValueLabel = new System.Windows.Forms.Label();
            this.primaryExpirationDateOfAuthorizationValueLabel = new System.Windows.Forms.Label();
            this.primaryEffectiveDateOfAuthorizationValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationTrackingNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryDaysAuthorizedValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationStatusLabel = new System.Windows.Forms.Label();
            this.primaryExpirationDateOfAuthorizationLabel = new System.Windows.Forms.Label();
            this.primaryEffectiveDateOfAuthorizationLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationCompanyRepresentativeNameLabel = new System.Windows.Forms.Label();
            this.primaryTrackingNumberLabel = new System.Windows.Forms.Label();
            this.primaryDaysAuthorizedLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationNumberLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceAuthorizationLabel = new System.Windows.Forms.Label();
            this.nppSignatureStatusValueLabel = new System.Windows.Forms.Label();
            this.nppSignatureStatusLabel = new System.Windows.Forms.Label();
            this.spanCode2ValueLabel = new System.Windows.Forms.Label();
            this.spanCode1ValueLabel = new System.Windows.Forms.Label();
            this.occurrenceCode8DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode7DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode5DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode6DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode4DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode3DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode2DateLabel = new System.Windows.Forms.Label();
            this.occurrenceCode1DateLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyPhoneNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyAddressValueLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryPromptExtValueLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentAddressLabel = new System.Windows.Forms.Label();
            this.secondaryAutoHomeInsuranceAgentNameLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyPhoneNumberLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyAddressLabel = new System.Windows.Forms.Label();
            this.secondaryAttorneyNameLabel = new System.Windows.Forms.Label();
            this.regulatoryPanel = new System.Windows.Forms.Panel();
            this.contactsPanel = new System.Windows.Forms.Panel();
            this.paymentPanel = new System.Windows.Forms.Panel();
            this.liabilityPanel = new System.Windows.Forms.Panel();
            this.billingPanel = new System.Windows.Forms.Panel();
            this.guarantorPanel = new System.Windows.Forms.Panel();
            this.secondaryInsuranceVerificationPanel = new System.Windows.Forms.Panel();
            this.secondaryInsuranceInsuredPanel = new System.Windows.Forms.Panel();
            this.secondaryInsurancePayorDetailsPanel = new System.Windows.Forms.Panel();
            this.secondaryInsurancePlanPanel = new System.Windows.Forms.Panel();
            this.primaryInsuranceVerificationPanel = new System.Windows.Forms.Panel();
            this.primaryInsuranceInsuredPanel = new System.Windows.Forms.Panel();
            this.primaryInsurancePayorDetailsPanel = new System.Windows.Forms.Panel();
            this.primaryInsurancePlanPanel = new System.Windows.Forms.Panel();
            this.mspPanel = new System.Windows.Forms.Panel();
            this.generalInsuranceInformationPanel2 = new System.Windows.Forms.Panel();
            this.clinicalPanel = new System.Windows.Forms.Panel();
            this.diagnosisPanel = new System.Windows.Forms.Panel();
            this.patientDemographicsEmploymentPanel = new System.Windows.Forms.Panel();
            this.dischargeDispositionValueLabel = new System.Windows.Forms.Label();
            this.dischargeDispositionLabel = new System.Windows.Forms.Label();
            this.dischargeDateTimeValueLabel = new System.Windows.Forms.Label();
            this.dischargeDateTimeLabel = new System.Windows.Forms.Label();
            this.contactPhysicalAddressPhoneValueLabel = new System.Windows.Forms.Label();
            this.contactPhysicalAddressPhoneLabel = new System.Windows.Forms.Label();
            this.patientPhysicalAddressValueLabel = new System.Windows.Forms.Label();
            this.patientPhysicalAddressLabel = new System.Windows.Forms.Label();
            this.facilityDeterminedFlagValueLabel = new System.Windows.Forms.Label();
            this.cosSignedValueLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryReligionValueLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryLocationValueLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel = new System.Windows.Forms.Label();
            this.privacyConfidentialStatusValueLabel = new System.Windows.Forms.Label();
            this.nppVersionValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact2PhoneValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact2AddressValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact2IsThePatientsValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact2NameValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact1PhoneValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact1AddressValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact1IsThePatientsValueLabel = new System.Windows.Forms.Label();
            this.emergencyContact1NameValueLabel = new System.Windows.Forms.Label();
            this.monthlyPaymentValueLabel = new System.Windows.Forms.Label();
            this.numberOfMonthlyPaymentsValueLabel = new System.Windows.Forms.Label();
            this.totalPaymentsCollectedForCurrentAccountValueLabel = new System.Windows.Forms.Label();
            this.totalCurrentAmountDueValueLabel = new System.Windows.Forms.Label();
            this.estimateForCurrentAmountDueUninsuredValueLabel = new System.Windows.Forms.Label();
            this.patientHasNoLiabilityUninsuredValueLabel = new System.Windows.Forms.Label();
            this.coPayCoInsInsuredValueLabel = new System.Windows.Forms.Label();
            this.deductibleInsuredValueLabel = new System.Windows.Forms.Label();
            this.patientHasNoLiabilityInsuredValueLabel = new System.Windows.Forms.Label();
            this.facilityDeterminedFlagLabel = new System.Windows.Forms.Label();
            this.cosSignedLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryReligionLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryHealthInformationLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryLocationLabel = new System.Windows.Forms.Label();
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel = new System.Windows.Forms.Label();
            this.privacyConfidentialStatusLabel = new System.Windows.Forms.Label();
            this.nppVersionLabel = new System.Windows.Forms.Label();
            this.regulatoryLabel = new System.Windows.Forms.Label();
            this.emergencyContact2PhoneLabel = new System.Windows.Forms.Label();
            this.emergencyContact2AddressLabel = new System.Windows.Forms.Label();
            this.emergencyContact2IsThePatientsLabel = new System.Windows.Forms.Label();
            this.emergencyContact2NameLabel = new System.Windows.Forms.Label();
            this.emergencyContact1PhoneLabel = new System.Windows.Forms.Label();
            this.emergencyContact1AddressLabel = new System.Windows.Forms.Label();
            this.emergencyContact1IsThePatientsLabel = new System.Windows.Forms.Label();
            this.emergencyContact1NameLabel = new System.Windows.Forms.Label();
            this.contactsLabel = new System.Windows.Forms.Label();
            this.monthlyPaymentLabel = new System.Windows.Forms.Label();
            this.numberOfMonthlyPaymentsLabel = new System.Windows.Forms.Label();
            this.totalPaymentsCollectedForCurrentAccountLabel = new System.Windows.Forms.Label();
            this.totalCurrentAmountDueLabel = new System.Windows.Forms.Label();
            this.paymentLabel = new System.Windows.Forms.Label();
            this.estimateForCurrentAmountDueUninsuredLabel = new System.Windows.Forms.Label();
            this.patientHasNoLiabilityUninsuredLabel = new System.Windows.Forms.Label();
            this.coPayCoInsInsuredLabel = new System.Windows.Forms.Label();
            this.deductibleInsuredLabel = new System.Windows.Forms.Label();
            this.patientHasNoLiabilityInsuredLabel = new System.Windows.Forms.Label();
            this.liabilityLabel = new System.Windows.Forms.Label();
            this.spanCode2TodateValueLabel = new System.Windows.Forms.Label();
            this.spanCode2FromdateValueLabel = new System.Windows.Forms.Label();
            this.spanCode1FacilityValueLabel = new System.Windows.Forms.Label();
            this.spanCode1TodateValueLabel = new System.Windows.Forms.Label();
            this.spanCode1FromdateValueLabel = new System.Windows.Forms.Label();
            this.spanCode2TodateLabel = new System.Windows.Forms.Label();
            this.spanCode2FromdateLabel = new System.Windows.Forms.Label();
            this.spanCode2Label = new System.Windows.Forms.Label();
            this.spanCode1FacilityLabel = new System.Windows.Forms.Label();
            this.spanCode1TodateLabel = new System.Windows.Forms.Label();
            this.spanCode1FromdateLabel = new System.Windows.Forms.Label();
            this.spanCode1Label = new System.Windows.Forms.Label();
            this.conditionCode7Label = new System.Windows.Forms.Label();
            this.conditionCode6Label = new System.Windows.Forms.Label();
            this.conditionCode5Label = new System.Windows.Forms.Label();
            this.conditionCode4Label = new System.Windows.Forms.Label();
            this.conditionCode3Label = new System.Windows.Forms.Label();
            this.conditionCode2Label = new System.Windows.Forms.Label();
            this.conditionCode1Label = new System.Windows.Forms.Label();
            this.occurrenceCode8Label = new System.Windows.Forms.Label();
            this.occurrenceCode7Label = new System.Windows.Forms.Label();
            this.occurrenceCode6Label = new System.Windows.Forms.Label();
            this.occurrenceCode5Label = new System.Windows.Forms.Label();
            this.occurrenceCode4Label = new System.Windows.Forms.Label();
            this.occurrenceCode3Label = new System.Windows.Forms.Label();
            this.occurrenceCode2Label = new System.Windows.Forms.Label();
            this.occurrenceCode1Label = new System.Windows.Forms.Label();
            this.guarantorOccIndustryValueLabel = new System.Windows.Forms.Label();
            this.guarantorEmployerPhoneValueLabel = new System.Windows.Forms.Label();
            this.guarantorEmployerValueLabel = new System.Windows.Forms.Label();
            this.guarantorEmploymentStatusValueLabel = new System.Windows.Forms.Label();
            this.guarantorContactEmailValueLabel = new System.Windows.Forms.Label();
            this.guarantorContactCellValueLabel = new System.Windows.Forms.Label();
            this.guarantorContactPhoneValueLabel = new System.Windows.Forms.Label();
            this.guarantorAddressValueLabel = new System.Windows.Forms.Label();
            this.usDriverLicenseValueLabel = new System.Windows.Forms.Label();
            this.guarantorSSNValueLabel = new System.Windows.Forms.Label();
            this.guarantorGenderValueLabel = new System.Windows.Forms.Label();
            this.thePatientIsTheGuarantorsValueLabel = new System.Windows.Forms.Label();
            this.guarantorNameValueLabel = new System.Windows.Forms.Label();
            this.billingLabel = new System.Windows.Forms.Label();
            this.guarantorOccIndustryLabel = new System.Windows.Forms.Label();
            this.guarantorEmployerPhoneLabel = new System.Windows.Forms.Label();
            this.guarantorEmployerLabel = new System.Windows.Forms.Label();
            this.guarantorEmploymentStatusLabel = new System.Windows.Forms.Label();
            this.guarantorContactEmailLabel = new System.Windows.Forms.Label();
            this.guarantorContactCellLabel = new System.Windows.Forms.Label();
            this.guarantorContactPhoneLabel = new System.Windows.Forms.Label();
            this.guarantorAddressLabel = new System.Windows.Forms.Label();
            this.usDriverLicenseLabel = new System.Windows.Forms.Label();
            this.guarantorSSNLabel = new System.Windows.Forms.Label();
            this.guarantorGenderLabel = new System.Windows.Forms.Label();
            this.thePatientIsTheGuarantorsLabel = new System.Windows.Forms.Label();
            this.guarantorNameLabel = new System.Windows.Forms.Label();
            this.secondaryPromptExtLabel = new System.Windows.Forms.Label();
            this.secondaryBillingAddressValueLabel = new System.Windows.Forms.Label();
            this.secondaryBillingAddressLabel = new System.Windows.Forms.Label();
            this.guarantorLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationPhoneValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationCompanyValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationRequiredValueLabel = new System.Windows.Forms.Label();
            this.secondaryDateValueLabel = new System.Windows.Forms.Label();
            this.secondaryInitiatedByValueLabel = new System.Windows.Forms.Label();
            this.secondaryBenefitsVerifiedValueLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationPhoneLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationCompanyLabel = new System.Windows.Forms.Label();
            this.secondaryAuthorizationRequiredLabel = new System.Windows.Forms.Label();
            this.secondaryDateLabel = new System.Windows.Forms.Label();
            this.secondaryInitiatedByLabel = new System.Windows.Forms.Label();
            this.secondaryBenefitsVerifiedLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceVerificationLabel = new System.Windows.Forms.Label();
            this.secondaryEmployerPhoneValueLabel = new System.Windows.Forms.Label();
            this.secondaryEmployerValueLabel = new System.Windows.Forms.Label();
            this.secondaryEmploymentStatusValueLabel = new System.Windows.Forms.Label();
            this.secondaryContactCellValueLabel = new System.Windows.Forms.Label();
            this.secondaryContactPhoneValueLabel = new System.Windows.Forms.Label();
            this.secondaryAddressValueLabel = new System.Windows.Forms.Label();
            this.secondaryNationalIdValueLabel = new System.Windows.Forms.Label();
            this.secondaryDobValueLabel = new System.Windows.Forms.Label();
            this.secondaryGenderValueLabel = new System.Windows.Forms.Label();
            this.secondaryThePatientIsTheInsuredValueLabel = new System.Windows.Forms.Label();
            this.secondaryNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryEmployerPhoneLabel = new System.Windows.Forms.Label();
            this.secondaryEmployerLabel = new System.Windows.Forms.Label();
            this.secondaryEmploymentStatusLabel = new System.Windows.Forms.Label();
            this.secondaryContactCellLabel = new System.Windows.Forms.Label();
            this.secondaryContactPhoneLabel = new System.Windows.Forms.Label();
            this.secondaryAddressLabel = new System.Windows.Forms.Label();
            this.secondaryNationalIdLabel = new System.Windows.Forms.Label();
            this.secondaryDobLabel = new System.Windows.Forms.Label();
            this.secondaryGenderLabel = new System.Windows.Forms.Label();
            this.secondaryThePatientIsTheInsuredLabel = new System.Windows.Forms.Label();
            this.secondaryNameLabel = new System.Windows.Forms.Label();
            this.secondaryBillingPhoneValueLabel = new System.Windows.Forms.Label();
            this.secondaryBillingNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryBillingCoNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryMedicalGroupIpaClinicValueLabel = new System.Windows.Forms.Label();
            this.secondaryMedicalGroupIpaNameValueLabel = new System.Windows.Forms.Label();
            this.secondaryAdjusterValueLabel = new System.Windows.Forms.Label();
            this.secondaryEmployeeSupervisorValueLabel = new System.Windows.Forms.Label();
            this.secondaryMedicaidIssueDateValueLabel = new System.Windows.Forms.Label();
            this.secondaryPrecertificationNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryGroupNumberValueLabel = new System.Windows.Forms.Label();
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel = new System.Windows.Forms.Label();
            //this.secondaryMBIValueLabel = new System.Windows.Forms.Label();
            this.secondaryInsuranceInsuredLabel = new System.Windows.Forms.Label();
            this.secondaryBillingPhoneLabel = new System.Windows.Forms.Label();
            this.secondaryBillingNameLabel = new System.Windows.Forms.Label();
            this.secondaryBillingCoNameLabel = new System.Windows.Forms.Label();
            this.secondaryMedicalGroupIpaClinicLabel = new System.Windows.Forms.Label();
            this.secondaryMedicalGroupIpaNameLabel = new System.Windows.Forms.Label();
            this.secondaryAdjusterLabel = new System.Windows.Forms.Label();
            this.secondaryEmployeeSupervisorLabel = new System.Windows.Forms.Label();
            this.secondaryMedicaidIssueDateLabel = new System.Windows.Forms.Label();
            this.secondaryPrecertificationNumberLabel = new System.Windows.Forms.Label();
            this.secondaryGroupNumberLabel = new System.Windows.Forms.Label();
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel = new System.Windows.Forms.Label();
            //this.secondaryMBILabel = new System.Windows.Forms.Label();
            this.secondaryInsurancePayorDetailsLabel = new System.Windows.Forms.Label();
            this.secondaryCategoryValueLabel = new System.Windows.Forms.Label();
            this.secondaryPayorBrokerValueLabel = new System.Windows.Forms.Label();
            this.secondaryPlanValueLabel = new System.Windows.Forms.Label();
            this.secondaryCategoryLabel = new System.Windows.Forms.Label();
            this.secondaryPayorBrokerLabel = new System.Windows.Forms.Label();
            this.secondaryPlanLabel = new System.Windows.Forms.Label();
            this.secondaryInsurancePlanLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentAddressValueLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentNameValueLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyPhoneNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyAddressValueLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyNameValueLabel = new System.Windows.Forms.Label();
            this.primaryPromptExtValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationPhoneValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationCompanyValueLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationRequiredValueLabel = new System.Windows.Forms.Label();
            this.primaryDateValueLabel = new System.Windows.Forms.Label();
            this.primaryInitiatedByValueLabel = new System.Windows.Forms.Label();
            this.primaryBenefitsVerifiedValueLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentAddressLabel = new System.Windows.Forms.Label();
            this.primaryAutoHomeInsuranceAgentNameLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyPhoneNumberLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyAddressLabel = new System.Windows.Forms.Label();
            this.primaryAttorneyNameLabel = new System.Windows.Forms.Label();
            this.primaryPromptExtLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationPhoneLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationCompanyLabel = new System.Windows.Forms.Label();
            this.primaryAuthorizationRequiredLabel = new System.Windows.Forms.Label();
            this.primaryDateLabel = new System.Windows.Forms.Label();
            this.primaryInitiatedByLabel = new System.Windows.Forms.Label();
            this.primaryBenefitsVerifiedLabel = new System.Windows.Forms.Label();
            this.primaryEmployerPhoneValueLabel = new System.Windows.Forms.Label();
            this.primaryEmployerValueLabel = new System.Windows.Forms.Label();
            this.primaryEmploymentStatusValueLabel = new System.Windows.Forms.Label();
            this.primaryContactCellValueLabel = new System.Windows.Forms.Label();
            this.primaryContactPhoneValueLabel = new System.Windows.Forms.Label();
            this.primaryAddressValueLabel = new System.Windows.Forms.Label();
            this.primaryNationalIdValueLabel = new System.Windows.Forms.Label();
            this.primaryDobValueLabel = new System.Windows.Forms.Label();
            this.primaryGenderValueLabel = new System.Windows.Forms.Label();
            this.primaryThePatientIsTheInsuredsValueLabel = new System.Windows.Forms.Label();
            this.primaryNameValueLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceVerificationLabel = new System.Windows.Forms.Label();
            this.primaryEmployerPhoneLabel = new System.Windows.Forms.Label();
            this.primaryEmployerLabel = new System.Windows.Forms.Label();
            this.primaryEmploymentStatusLabel = new System.Windows.Forms.Label();
            this.primaryContactCellLabel = new System.Windows.Forms.Label();
            this.primaryContactPhoneLabel = new System.Windows.Forms.Label();
            this.primaryAddressLabel = new System.Windows.Forms.Label();
            this.primaryNationalIdLabel = new System.Windows.Forms.Label();
            this.primaryDobLabel = new System.Windows.Forms.Label();
            this.primaryGenderLabel = new System.Windows.Forms.Label();
            this.primaryThePatientIsTheInsuredsLabel = new System.Windows.Forms.Label();
            this.primaryNameLabel = new System.Windows.Forms.Label();
            this.primaryBillingAddressValueLabel = new System.Windows.Forms.Label();
            this.primaryBillingPhoneValueLabel = new System.Windows.Forms.Label();
            this.primaryBillingNameValueLabel = new System.Windows.Forms.Label();
            this.primaryBillingCoNameValueLabel = new System.Windows.Forms.Label();
            this.primaryMedicalGroupOrIPAClinicValueLabel = new System.Windows.Forms.Label();
            this.primaryMedicalGroupOrIpaNameValueLabel = new System.Windows.Forms.Label();
            this.primaryAdjusterValueLabel = new System.Windows.Forms.Label();
            this.primaryEmployeeSupervisorValueLabel = new System.Windows.Forms.Label();
            this.primaryMedicaidIssueDateValueLabel = new System.Windows.Forms.Label();
            this.primaryPrecertificationNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryGroupNumberValueLabel = new System.Windows.Forms.Label();
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel = new System.Windows.Forms.Label();
            //this.primaryMBIValueLabel = new System.Windows.Forms.Label();
            this.primaryInsuranceInsuredLabel = new System.Windows.Forms.Label();
            this.primaryBillingAddressLabel = new System.Windows.Forms.Label();
            this.primaryBillingPhoneLabel = new System.Windows.Forms.Label();
            this.primaryBillingNameLabel = new System.Windows.Forms.Label();
            this.primaryBillingCoNameLabel = new System.Windows.Forms.Label();
            this.primaryMedicalGroupOrIPAClinicLabel = new System.Windows.Forms.Label();
            this.primaryMedicalGroupOrIpaNameLabel = new System.Windows.Forms.Label();
            this.primaryAdjusterLabel = new System.Windows.Forms.Label();
            this.primaryEmployeeSupervisorLabel = new System.Windows.Forms.Label();
            this.primaryMedicaidIssueDateLabel = new System.Windows.Forms.Label();
            this.primaryPrecertificationNumberLabel = new System.Windows.Forms.Label();
            this.primaryGroupNumberLabel = new System.Windows.Forms.Label();
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel = new System.Windows.Forms.Label();
            //this.primaryMBILabel = new System.Windows.Forms.Label();
            this.primaryInsurancePayorDetailsLabel = new System.Windows.Forms.Label();
            this.primaryCategoryValueLabel = new System.Windows.Forms.Label();
            this.primaryPayorBrokerValueLabel = new System.Windows.Forms.Label();
            this.primaryPlanValueLabel = new System.Windows.Forms.Label();
            this.primaryCategoryLabel = new System.Windows.Forms.Label();
            this.primaryPayorBrokerLabel = new System.Windows.Forms.Label();
            this.primaryPlanLabel = new System.Windows.Forms.Label();
            this.primaryInsurancePlanLabel = new System.Windows.Forms.Label();
            this.mspQuestionnaireSummaryConclusionLabel = new System.Windows.Forms.Label();
            this.mspLabel = new System.Windows.Forms.Label();
            this.fatherDobValueLabel = new System.Windows.Forms.Label();
            this.motherDobValueLabel = new System.Windows.Forms.Label();
            this.financialClassValueLabel = new System.Windows.Forms.Label();
            this.fatherDobLabel = new System.Windows.Forms.Label();
            this.motherDobLabel = new System.Windows.Forms.Label();
            this.financialClassLabel = new System.Windows.Forms.Label();
            this.generalInsuranceInformationLabel = new System.Windows.Forms.Label();
            this.commentsValueLabel = new System.Windows.Forms.Label();
            this.ResistantOrganismValueLabel = new System.Windows.Forms.Label();
            this.pregnantValueLabel = new System.Windows.Forms.Label();
            this.bloodlessValueLabel = new System.Windows.Forms.Label();
            this.smokerValueLabel = new System.Windows.Forms.Label();
            this.primaryCarePhysicianValueLabel = new System.Windows.Forms.Label();
            this.operatingPhysicianValueLabel = new System.Windows.Forms.Label();
            this.commentsLabel = new System.Windows.Forms.Label();
            this.ResistantOrganismLabel = new System.Windows.Forms.Label();
            this.pregnantLabel = new System.Windows.Forms.Label();
            this.bloodlessLabel = new System.Windows.Forms.Label();
            this.smokerLabel = new System.Windows.Forms.Label();
            this.primaryCarePhysicianLabel = new System.Windows.Forms.Label();
            this.operatingPhysicianLabel = new System.Windows.Forms.Label();
            this.clinicalLabel = new System.Windows.Forms.Label();
            this.attendingPhysicianValueLabel = new System.Windows.Forms.Label();
            this.admittingPhysicianValueLabel = new System.Windows.Forms.Label();
            this.referringPhysicianValueLabel = new System.Windows.Forms.Label();
            this.attendingPhysicianLabel = new System.Windows.Forms.Label();
            this.admittingPhysicianLabel = new System.Windows.Forms.Label();
            this.referringPhysicianLabel = new System.Windows.Forms.Label();
            this.clinic5ValueLabel = new System.Windows.Forms.Label();
            this.clinic4ValueLabel = new System.Windows.Forms.Label();
            this.clinic3ValueLabel = new System.Windows.Forms.Label();
            this.clinic2ValueLabel = new System.Windows.Forms.Label();
            this.clinic1ValueLabel = new System.Windows.Forms.Label();
            this.referralSourceValueLabel = new System.Windows.Forms.Label();
            this.clinic5Label = new System.Windows.Forms.Label();
            this.clinic4Label = new System.Windows.Forms.Label();
            this.clinic3Label = new System.Windows.Forms.Label();
            this.modeOfArrivalValueLabel = new System.Windows.Forms.Label();
            this.admitSourceValueLabel = new System.Windows.Forms.Label();
            this.tenetCareValueLabel = new System.Windows.Forms.Label();
            this.dateOfOnsetForSymptomsIllnessValueLabel = new System.Windows.Forms.Label();
            this.accidentCrimeStateProvinceValueLabel = new System.Windows.Forms.Label();
            this.accidentCrimeCountryValueLabel = new System.Windows.Forms.Label();
            this.accidentCrimeHourValueLabel = new System.Windows.Forms.Label();
            this.clinic2Label = new System.Windows.Forms.Label();
            this.clinic1Label = new System.Windows.Forms.Label();
            this.referralSourceLabel = new System.Windows.Forms.Label();
            this.modeOfArrivalLabel = new System.Windows.Forms.Label();
            this.admitSourceLabel = new System.Windows.Forms.Label();
            this.tenetCareLabel = new System.Windows.Forms.Label();
            this.dateOfOnsetForSymptomsIllnessLabel = new System.Windows.Forms.Label();
            this.accidentCrimeStateProvinceLabel = new System.Windows.Forms.Label();
            this.accidentCrimeCountryLabel = new System.Windows.Forms.Label();
            this.accidentCrimeHourLabel = new System.Windows.Forms.Label();
            this.accidentCrimeDateValueLabel = new System.Windows.Forms.Label();
            this.accidentCrimeDateLabel = new System.Windows.Forms.Label();
            this.accidentTypeValueLabel = new System.Windows.Forms.Label();
            this.accidentTypeLabel = new System.Windows.Forms.Label();
            this.patientVisitValueLabel = new System.Windows.Forms.Label();
            this.patientVisitLabel = new System.Windows.Forms.Label();
            this.chiefComplaintValueLabel = new System.Windows.Forms.Label();
            this.chiefComplaintLabel = new System.Windows.Forms.Label();
            this.accomodationcodeValueLabelL = new System.Windows.Forms.Label();
            this.accomodationcodeLabelL = new System.Windows.Forms.Label();
            this.assignedBedValueLabel = new System.Windows.Forms.Label();
            this.assignedBedLabel = new System.Windows.Forms.Label();
            this.hospitalServiceValueLabel = new System.Windows.Forms.Label();
            this.hospitalServiceLabel = new System.Windows.Forms.Label();
            this.reregisterValueLabel = new System.Windows.Forms.Label();
            this.reregisterLabel = new System.Windows.Forms.Label();
            this.patientTypeValueLabel = new System.Windows.Forms.Label();
            this.patientTypeLabel = new System.Windows.Forms.Label();
            this.diagnosisLabel = new System.Windows.Forms.Label();
            this.employeeIDValueLabel = new System.Windows.Forms.Label();
            this.employeeIDlabel = new System.Windows.Forms.Label();
            this.occIndustryValueLabel = new System.Windows.Forms.Label();
            this.occIndustryLabel = new System.Windows.Forms.Label();
            this.employerPhoneValueLabel = new System.Windows.Forms.Label();
            this.employerPhoneLabel = new System.Windows.Forms.Label();
            this.employerValueLabel = new System.Windows.Forms.Label();
            this.employerlabel = new System.Windows.Forms.Label();
            this.employmentStatusValueLabel = new System.Windows.Forms.Label();
            this.employmentStatusLabel = new System.Windows.Forms.Label();
            this.valuablesCollectedValuelabel = new System.Windows.Forms.Label();
            this.valuablesCollectedLabel = new System.Windows.Forms.Label();
            this.placeOfWorshipValueLabel = new System.Windows.Forms.Label();
            this.placeOfWorshipLabel = new System.Windows.Forms.Label();
            this.clergyMayVisitValueLabel = new System.Windows.Forms.Label();
            this.clergyMayVisitLabel = new System.Windows.Forms.Label();
            this.religionValueLabel = new System.Windows.Forms.Label();
            this.religionLabel = new System.Windows.Forms.Label();
            this.languageValueLabel = new System.Windows.Forms.Label();
            this.languageLabel = new System.Windows.Forms.Label();
            this.placeOfBirthValueLabel = new System.Windows.Forms.Label();
            this.placeOfBirthLabel = new System.Windows.Forms.Label();
            this.driversLicenseValueLabel = new System.Windows.Forms.Label();
            this.driversLicenseLabel = new System.Windows.Forms.Label();
            this.passportValueLabel = new System.Windows.Forms.Label();
            this.passportLabel = new System.Windows.Forms.Label();
            this.contactEmailValueLabel = new System.Windows.Forms.Label();
            this.contactEmailLabel = new System.Windows.Forms.Label();
            this.contactCellValueLabel = new System.Windows.Forms.Label();
            this.contactCellLabel = new System.Windows.Forms.Label();
            this.contactPhoneValueLabel = new System.Windows.Forms.Label();
            this.contactPhoneLabel = new System.Windows.Forms.Label();
            this.patientMailingAddressValueLabel = new System.Windows.Forms.Label();
            this.patientMailingAddressLabel = new System.Windows.Forms.Label();
            this.nationalIdValueLabel = new System.Windows.Forms.Label();
            this.nationalIdLabel = new System.Windows.Forms.Label();
            this.ssnValueLabel = new System.Windows.Forms.Label();
            this.ssnLabel = new System.Windows.Forms.Label();
            this.ethnicityValueLabel = new System.Windows.Forms.Label();
            this.ethnicityLabel = new System.Windows.Forms.Label();
            this.raceValueLabel = new System.Windows.Forms.Label();
            this.raceLabel = new System.Windows.Forms.Label();
            this.maritalStatusValueLabel = new System.Windows.Forms.Label();
            this.maritalStatusLabel = new System.Windows.Forms.Label();
            this.ageValueLabel = new System.Windows.Forms.Label();
            this.ageLabel = new System.Windows.Forms.Label();
            this.rightToRestrictValueLabel = new System.Windows.Forms.Label();
            this.dobLabel = new System.Windows.Forms.Label();
            this.genderValueLabel = new System.Windows.Forms.Label();
            this.genderLabel = new System.Windows.Forms.Label();
            this.appointmentValueLabel = new System.Windows.Forms.Label();
            this.appointmentLabel = new System.Windows.Forms.Label();
            this.admitDateTimeValueLabel = new System.Windows.Forms.Label();
            this.admitDateTimeLabel = new System.Windows.Forms.Label();
            this.akaValueLabel = new System.Windows.Forms.Label();
            this.akaLabel = new System.Windows.Forms.Label();
            this.patientNameValueLabel = new System.Windows.Forms.Label();
            this.dobValueLabel = new System.Windows.Forms.Label();
            this.patientNameLabel = new System.Windows.Forms.Label();
            this.patientDemographicsAndEmploymentLabel = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NonPurgedAccountDetailsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // guarantorDOBValuelabel
            // 
            this.guarantorDOBValuelabel.AutoSize = true;
            this.guarantorDOBValuelabel.Location = new System.Drawing.Point(44, 5311);
            this.guarantorDOBValuelabel.Name = "guarantorDOBValuelabel";
            this.guarantorDOBValuelabel.Size = new System.Drawing.Size(10, 13);
            this.guarantorDOBValuelabel.TabIndex = 562;
            this.guarantorDOBValuelabel.Text = " ";
            // 
            // guarantorDOBLabel
            // 
            this.guarantorDOBLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorDOBLabel.Location = new System.Drawing.Point(7, 5311);
            this.guarantorDOBLabel.Name = "guarantorDOBLabel";
            this.guarantorDOBLabel.Size = new System.Drawing.Size(31, 16);
            this.guarantorDOBLabel.TabIndex = 561;
            this.guarantorDOBLabel.Text = "DOB:";
            // 
            // NonPurgedAccountDetailsPanel
            // 
            this.NonPurgedAccountDetailsPanel.AutoScroll = true;
            this.NonPurgedAccountDetailsPanel.BackColor = System.Drawing.Color.White;
            this.NonPurgedAccountDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.HospCommoptinlLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorDOBValuelabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorDOBLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.monthlyDueDateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.monthlyDueDateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.shareDataWithPCPFlagLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.shareDataWithPCPValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.cobReceivedFlagLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.cobReceivedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.imfmReceivedFlagLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.imfmReceivedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.shareDataWithPublicHieFlagLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.shareDataWithPublicHieValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.HospCommoptinValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorCellPhoneConsentValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorCellPhoneConsentLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.CptCodesValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.CptCodesLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.rightToRestrictLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.dataGridView2);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.LeftWithoutFinancialClearanceValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.LeftWithoutFinancialClearanceLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.LeftWithoutBeingSeenValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.LeftWithoutBeingSeenLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.RCRPValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.RCRPLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.LeftOrStayedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.LeftOrStayedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.alternatecareFacilityValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.AlternatecareFacilityLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.PatientInClinicalStudyValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.PatientInClinicalREsearchLBL);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.procedureValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.procedureLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.preopDateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.preopDateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinicalTrialValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinicalTrialLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryServicesAuthorizedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryServicesAuthorizedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryRemarksValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryRemarksLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsuranceAuthorizationPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationStatusValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryExpirationDateOfAuthorizationValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEffectiveDateOfAuthorizationValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationCompanyRepresentativeNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationTrackingNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryDaysAuthorizedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationStatusLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryExpirationDateOfAuthorizationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEffectiveDateOfAuthorizationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationCompanyRepresentativeNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationTrackingNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryDaysAuthorizedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsuranceAuthorizationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryServicesAuthorizedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryServicesAuthorizedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryRemarksValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryRemarksLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsuranceAuthorizationPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationStatusValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryExpirationDateOfAuthorizationValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEffectiveDateOfAuthorizationValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationCompanyRepresentativeNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationTrackingNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryDaysAuthorizedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationStatusLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryExpirationDateOfAuthorizationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEffectiveDateOfAuthorizationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationCompanyRepresentativeNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryTrackingNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryDaysAuthorizedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsuranceAuthorizationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.nppSignatureStatusValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.nppSignatureStatusLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode2ValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode1ValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode8DateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode7DateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode5DateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode6DateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode4DateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode3DateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode2DateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode1DateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAutoHomeInsuranceAgentAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAutoHomeInsuranceAgentNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAttorneyPhoneNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAttorneyAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAttorneyNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryPromptExtValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAutoHomeInsuranceAgentAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAutoHomeInsuranceAgentNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAttorneyPhoneNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAttorneyAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAttorneyNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.regulatoryPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactsPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.paymentPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.liabilityPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.billingPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsuranceVerificationPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsuranceInsuredPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsurancePayorDetailsPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsurancePlanPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsuranceVerificationPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsuranceInsuredPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsurancePayorDetailsPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsurancePlanPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.mspPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.generalInsuranceInformationPanel2);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinicalPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.diagnosisPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientDemographicsEmploymentPanel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.dischargeDispositionValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.dischargeDispositionLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.dischargeDateTimeValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.dischargeDateTimeLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactPhysicalAddressPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactPhysicalAddressPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientPhysicalAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientPhysicalAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.facilityDeterminedFlagValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.cosSignedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyOptOutPatientDirectoryReligionValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyOptOutPatientDirectoryHealthInformationValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyOptOutPatientDirectoryLocationValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyConfidentialStatusValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.nppVersionValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2PhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2AddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2IsThePatientsValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2NameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1PhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1AddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1IsThePatientsValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1NameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.monthlyPaymentValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.numberOfMonthlyPaymentsValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.totalPaymentsCollectedForCurrentAccountValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.totalCurrentAmountDueValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.estimateForCurrentAmountDueUninsuredValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientHasNoLiabilityUninsuredValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.coPayCoInsInsuredValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.deductibleInsuredValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientHasNoLiabilityInsuredValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.facilityDeterminedFlagLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.cosSignedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyOptOutPatientDirectoryReligionLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyOptOutPatientDirectoryHealthInformationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyOptOutPatientDirectoryLocationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyOptOutPatientDirectoryNameAndAllInformationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.privacyConfidentialStatusLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.nppVersionLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.regulatoryLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2PhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2AddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2IsThePatientsLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact2NameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1PhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1AddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1IsThePatientsLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.emergencyContact1NameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactsLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.monthlyPaymentLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.numberOfMonthlyPaymentsLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.totalPaymentsCollectedForCurrentAccountLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.totalCurrentAmountDueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.paymentLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.estimateForCurrentAmountDueUninsuredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientHasNoLiabilityUninsuredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.coPayCoInsInsuredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.deductibleInsuredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientHasNoLiabilityInsuredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.liabilityLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode2TodateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode2FromdateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode1FacilityValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode1TodateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode1FromdateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode2TodateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode2FromdateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode2Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode1FacilityLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode1TodateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode1FromdateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.spanCode1Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.conditionCode7Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.conditionCode6Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.conditionCode5Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.conditionCode4Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.conditionCode3Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.conditionCode2Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.conditionCode1Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode8Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode7Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode6Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode5Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode4Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode3Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode2Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occurrenceCode1Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorOccIndustryValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorEmployerPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorEmployerValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorEmploymentStatusValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorContactEmailValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorContactCellValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorContactPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.usDriverLicenseValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorSSNValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorGenderValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.thePatientIsTheGuarantorsValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.billingLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorOccIndustryLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorEmployerPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorEmployerLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorEmploymentStatusLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorContactEmailLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorContactCellLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorContactPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.usDriverLicenseLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorSSNLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorGenderLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.thePatientIsTheGuarantorsLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryPromptExtLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBillingAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBillingAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.guarantorLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationCompanyValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationRequiredValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryDateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInitiatedByValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBenefitsVerifiedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationCompanyLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAuthorizationRequiredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryDateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInitiatedByLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBenefitsVerifiedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsuranceVerificationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEmployerPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEmployerValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEmploymentStatusValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryContactCellValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryContactPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryNationalIdValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryDobValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryGenderValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryThePatientIsTheInsuredValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEmployerPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEmployerLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEmploymentStatusLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryContactCellLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryContactPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryNationalIdLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryDobLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryGenderLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryThePatientIsTheInsuredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBillingPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBillingNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBillingCoNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryMedicalGroupIpaClinicValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryMedicalGroupIpaNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAdjusterValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEmployeeSupervisorValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryMedicaidIssueDateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryPrecertificationNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryGroupNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel);
            //this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryMBIValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsuranceInsuredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBillingPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBillingNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryBillingCoNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryMedicalGroupIpaClinicLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryMedicalGroupIpaNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryAdjusterLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryEmployeeSupervisorLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryMedicaidIssueDateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryPrecertificationNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryGroupNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel);
            //this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryMBILabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsurancePayorDetailsLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryCategoryValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryPayorBrokerValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryPlanValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryCategoryLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryPayorBrokerLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryPlanLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.secondaryInsurancePlanLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAutoHomeInsuranceAgentAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAutoHomeInsuranceAgentNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAttorneyPhoneNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAttorneyAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAttorneyNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryPromptExtValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationCompanyValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationRequiredValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryDateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInitiatedByValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBenefitsVerifiedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAutoHomeInsuranceAgentPhoneNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAutoHomeInsuranceAgentAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAutoHomeInsuranceAgentNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAttorneyPhoneNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAttorneyAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAttorneyNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryPromptExtLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationCompanyLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAuthorizationRequiredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryDateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInitiatedByLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBenefitsVerifiedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEmployerPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEmployerValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEmploymentStatusValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryContactCellValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryContactPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryNationalIdValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryDobValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryGenderValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryThePatientIsTheInsuredsValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsuranceVerificationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEmployerPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEmployerLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEmploymentStatusLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryContactCellLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryContactPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryNationalIdLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryDobLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryGenderLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryThePatientIsTheInsuredsLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBillingAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBillingPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBillingNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBillingCoNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryMedicalGroupOrIPAClinicValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryMedicalGroupOrIpaNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAdjusterValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEmployeeSupervisorValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryMedicaidIssueDateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryPrecertificationNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryGroupNumberValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel);
            //this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryMBIValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsuranceInsuredLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBillingAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBillingPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBillingNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryBillingCoNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryMedicalGroupOrIPAClinicLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryMedicalGroupOrIpaNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryAdjusterLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryEmployeeSupervisorLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryMedicaidIssueDateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryPrecertificationNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryGroupNumberLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel);
            //this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryMBILabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsurancePayorDetailsLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryCategoryValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryPayorBrokerValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryPlanValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryCategoryLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryPayorBrokerLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryPlanLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryInsurancePlanLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.mspQuestionnaireSummaryConclusionLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.mspLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.fatherDobValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.motherDobValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.financialClassValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.fatherDobLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.motherDobLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.financialClassLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.generalInsuranceInformationLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.commentsValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.ResistantOrganismValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.pregnantValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.bloodlessValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.smokerValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryCarePhysicianValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.operatingPhysicianValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.commentsLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.ResistantOrganismLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.pregnantLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.bloodlessLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.smokerLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.primaryCarePhysicianLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.operatingPhysicianLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinicalLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.attendingPhysicianValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.admittingPhysicianValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.referringPhysicianValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.attendingPhysicianLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.admittingPhysicianLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.referringPhysicianLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic5ValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic4ValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic3ValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic2ValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic1ValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.referralSourceValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic5Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic4Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic3Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.modeOfArrivalValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.admitSourceValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.tenetCareValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.dateOfOnsetForSymptomsIllnessValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentCrimeStateProvinceValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentCrimeCountryValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentCrimeHourValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic2Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clinic1Label);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.referralSourceLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.modeOfArrivalLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.admitSourceLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.tenetCareLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.dateOfOnsetForSymptomsIllnessLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentCrimeStateProvinceLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentCrimeCountryLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentCrimeHourLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentCrimeDateValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentCrimeDateLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentTypeValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accidentTypeLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientVisitValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientVisitLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.chiefComplaintValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.chiefComplaintLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accomodationcodeValueLabelL);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.accomodationcodeLabelL);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.assignedBedValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.assignedBedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.hospitalServiceValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.hospitalServiceLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.reregisterValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.reregisterLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientTypeValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientTypeLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.diagnosisLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.employeeIDValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.employeeIDlabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occIndustryValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.occIndustryLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.employerPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.employerPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.employerValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.employerlabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.employmentStatusValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.employmentStatusLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.valuablesCollectedValuelabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.valuablesCollectedLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.placeOfWorshipValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.placeOfWorshipLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clergyMayVisitValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.clergyMayVisitLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.religionValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.religionLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.languageValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.languageLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.placeOfBirthValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.placeOfBirthLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.driversLicenseValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.driversLicenseLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.passportValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.passportLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactEmailValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactEmailLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactCellValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactCellLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactPhoneValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.contactPhoneLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientMailingAddressValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientMailingAddressLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.nationalIdValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.nationalIdLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.ssnValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.ssnLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.ethnicityValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.ethnicityLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.raceValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.raceLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.maritalStatusValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.maritalStatusLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.ageValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.ageLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.rightToRestrictValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.dobLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.genderValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.genderLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.appointmentValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.appointmentLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.admitDateTimeValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.admitDateTimeLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.akaValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.akaLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientNameValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.dobValueLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientNameLabel);
            this.NonPurgedAccountDetailsPanel.Controls.Add(this.patientDemographicsAndEmploymentLabel);
            this.NonPurgedAccountDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NonPurgedAccountDetailsPanel.Location = new System.Drawing.Point(0, 0);
            this.NonPurgedAccountDetailsPanel.Name = "NonPurgedAccountDetailsPanel";
            this.NonPurgedAccountDetailsPanel.Size = new System.Drawing.Size(908, 540);
            this.NonPurgedAccountDetailsPanel.TabIndex = 1;
            this.NonPurgedAccountDetailsPanel.TabStop = true;
            this.NonPurgedAccountDetailsPanel.MouseEnter += new System.EventHandler(this.PanelMouseEnter);
            // 
            // HospCommoptinlLabel
            // 
            this.HospCommoptinlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HospCommoptinlLabel.Location = new System.Drawing.Point(7, 7005);
            this.HospCommoptinlLabel.Name = "HospCommoptinlLabel";
            this.HospCommoptinlLabel.Size = new System.Drawing.Size(118, 16);
            this.HospCommoptinlLabel.TabIndex = 41;
            this.HospCommoptinlLabel.Text = "Hosp Comm Optin:";
            // 
            // monthlyDueDateValueLabel
            // 
            this.monthlyDueDateValueLabel.AutoSize = true;
            this.monthlyDueDateValueLabel.Location = new System.Drawing.Point(130, 6385);
            this.monthlyDueDateValueLabel.Name = "monthlyDueDateValueLabel";
            this.monthlyDueDateValueLabel.Size = new System.Drawing.Size(35, 13);
            this.monthlyDueDateValueLabel.TabIndex = 560;
            this.monthlyDueDateValueLabel.Text = "label1";
            // 
            // monthlyDueDateLabel
            // 
            this.monthlyDueDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthlyDueDateLabel.Location = new System.Drawing.Point(7, 6385);
            this.monthlyDueDateLabel.Name = "monthlyDueDateLabel";
            this.monthlyDueDateLabel.Size = new System.Drawing.Size(117, 16);
            this.monthlyDueDateLabel.TabIndex = 559;
            this.monthlyDueDateLabel.Text = "Monthly Due Date:";
            // 
            // shareDataWithPCPFlagLabel
            // 
            this.shareDataWithPCPFlagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shareDataWithPCPFlagLabel.Location = new System.Drawing.Point(7, 6984);
            this.shareDataWithPCPFlagLabel.Name = "shareDataWithPCPFlagLabel";
            this.shareDataWithPCPFlagLabel.Size = new System.Drawing.Size(136, 16);
            this.shareDataWithPCPFlagLabel.TabIndex = 555;
            this.shareDataWithPCPFlagLabel.Text = "Notify PCP of Visit:";
            // 
            // shareDataWithPCPValueLabel
            // 
            this.shareDataWithPCPValueLabel.AutoSize = true;
            this.shareDataWithPCPValueLabel.Location = new System.Drawing.Point(143, 6984);
            this.shareDataWithPCPValueLabel.Name = "shareDataWithPCPValueLabel";
            this.shareDataWithPCPValueLabel.Size = new System.Drawing.Size(10, 13);
            this.shareDataWithPCPValueLabel.TabIndex = 556;
            this.shareDataWithPCPValueLabel.Text = " ";
            // 
            // cobReceivedFlagLabel
            // 
            this.cobReceivedFlagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cobReceivedFlagLabel.Location = new System.Drawing.Point(8, 7025);
            this.cobReceivedFlagLabel.Name = "cobReceivedFlagLabel";
            this.cobReceivedFlagLabel.Size = new System.Drawing.Size(98, 16);
            this.cobReceivedFlagLabel.TabIndex = 557;
            this.cobReceivedFlagLabel.Text = "COB Received:";
            // 
            // cobReceivedValueLabel
            // 
            this.cobReceivedValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cobReceivedValueLabel.Location = new System.Drawing.Point(110, 7025);
            this.cobReceivedValueLabel.Name = "cobReceivedValueLabel";
            this.cobReceivedValueLabel.Size = new System.Drawing.Size(40, 16);
            this.cobReceivedValueLabel.TabIndex = 558;
            this.cobReceivedValueLabel.Text = " ";
            // 
            // imfmReceivedFlagLabel
            // 
            this.imfmReceivedFlagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.imfmReceivedFlagLabel.Location = new System.Drawing.Point(7, 7045);
            this.imfmReceivedFlagLabel.Name = "imfmReceivedFlagLabel";
            this.imfmReceivedFlagLabel.Size = new System.Drawing.Size(102, 26);
            this.imfmReceivedFlagLabel.TabIndex = 559;
            this.imfmReceivedFlagLabel.Text = "IMFM Received:";
            // 
            // imfmReceivedValueLabel
            // 
            this.imfmReceivedValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.imfmReceivedValueLabel.Location = new System.Drawing.Point(110, 7045);
            this.imfmReceivedValueLabel.Name = "imfmReceivedValueLabel";
            this.imfmReceivedValueLabel.Size = new System.Drawing.Size(33, 17);
            this.imfmReceivedValueLabel.TabIndex = 563;
            this.imfmReceivedValueLabel.Text = " ";
            // 
            // shareDataWithPublicHieFlagLabel
            // 
            this.shareDataWithPublicHieFlagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shareDataWithPublicHieFlagLabel.Location = new System.Drawing.Point(7, 6965);
            this.shareDataWithPublicHieFlagLabel.Name = "shareDataWithPublicHieFlagLabel";
            this.shareDataWithPublicHieFlagLabel.Size = new System.Drawing.Size(300, 16);
            this.shareDataWithPublicHieFlagLabel.TabIndex = 553;
            this.shareDataWithPublicHieFlagLabel.Text = "Share with Health Information Exchange:";
            // 
            // shareDataWithPublicHieValueLabel
            // 
            this.shareDataWithPublicHieValueLabel.AutoSize = true;
            this.shareDataWithPublicHieValueLabel.Location = new System.Drawing.Point(310, 6965);
            this.shareDataWithPublicHieValueLabel.Name = "shareDataWithPublicHieValueLabel";
            this.shareDataWithPublicHieValueLabel.Size = new System.Drawing.Size(10, 13);
            this.shareDataWithPublicHieValueLabel.TabIndex = 554;
            this.shareDataWithPublicHieValueLabel.Text = " ";
            // 
            // HospCommoptinValueLabel
            // 
            this.HospCommoptinValueLabel.AutoSize = true;
            this.HospCommoptinValueLabel.Location = new System.Drawing.Point(134, 7005);
            this.HospCommoptinValueLabel.Name = "HospCommoptinValueLabel";
            this.HospCommoptinValueLabel.Size = new System.Drawing.Size(10, 13);
            this.HospCommoptinValueLabel.TabIndex = 42;
            this.HospCommoptinValueLabel.Text = " ";
            // 
            // guarantorCellPhoneConsentValueLabel
            // 
            this.guarantorCellPhoneConsentValueLabel.AutoSize = true;
            this.guarantorCellPhoneConsentValueLabel.Location = new System.Drawing.Point(130, 5442);
            this.guarantorCellPhoneConsentValueLabel.Name = "guarantorCellPhoneConsentValueLabel";
            this.guarantorCellPhoneConsentValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorCellPhoneConsentValueLabel.TabIndex = 552;
            // 
            // guarantorCellPhoneConsentLabel
            // 
            this.guarantorCellPhoneConsentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorCellPhoneConsentLabel.Location = new System.Drawing.Point(7, 5442);
            this.guarantorCellPhoneConsentLabel.Name = "guarantorCellPhoneConsentLabel";
            this.guarantorCellPhoneConsentLabel.Size = new System.Drawing.Size(135, 16);
            this.guarantorCellPhoneConsentLabel.TabIndex = 551;
            this.guarantorCellPhoneConsentLabel.Text = "Cell Phone Consent:";
            // 
            // CptCodesValueLabel
            // 
            this.CptCodesValueLabel.AutoSize = true;
            this.CptCodesValueLabel.Location = new System.Drawing.Point(93, 1086);
            this.CptCodesValueLabel.Name = "CptCodesValueLabel";
            this.CptCodesValueLabel.Size = new System.Drawing.Size(0, 13);
            this.CptCodesValueLabel.TabIndex = 550;
            // 
            // CptCodesLabel
            // 
            this.CptCodesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CptCodesLabel.Location = new System.Drawing.Point(8, 1086);
            this.CptCodesLabel.Name = "CptCodesLabel";
            this.CptCodesLabel.Size = new System.Drawing.Size(94, 16);
            this.CptCodesLabel.TabIndex = 549;
            this.CptCodesLabel.Text = "CPT Codes :";
            // 
            // rightToRestrictLabel
            // 
            this.rightToRestrictLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rightToRestrictLabel.Location = new System.Drawing.Point(7, 6877);
            this.rightToRestrictLabel.Name = "rightToRestrictLabel";
            this.rightToRestrictLabel.Size = new System.Drawing.Size(210, 16);
            this.rightToRestrictLabel.TabIndex = 547;
            this.rightToRestrictLabel.Text = "Patient Requested Right to Restrict:";
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
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView2.Location = new System.Drawing.Point(10, 1791);
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
            this.dataGridView2.Size = new System.Drawing.Size(762, 239);
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
            // LeftWithoutFinancialClearanceValueLabel
            // 
            this.LeftWithoutFinancialClearanceValueLabel.AutoSize = true;
            this.LeftWithoutFinancialClearanceValueLabel.Location = new System.Drawing.Point(210, 2108);
            this.LeftWithoutFinancialClearanceValueLabel.Name = "LeftWithoutFinancialClearanceValueLabel";
            this.LeftWithoutFinancialClearanceValueLabel.Size = new System.Drawing.Size(10, 13);
            this.LeftWithoutFinancialClearanceValueLabel.TabIndex = 544;
            this.LeftWithoutFinancialClearanceValueLabel.Text = " ";
            // 
            // LeftWithoutFinancialClearanceLabel
            // 
            this.LeftWithoutFinancialClearanceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LeftWithoutFinancialClearanceLabel.Location = new System.Drawing.Point(7, 2108);
            this.LeftWithoutFinancialClearanceLabel.Name = "LeftWithoutFinancialClearanceLabel";
            this.LeftWithoutFinancialClearanceLabel.Size = new System.Drawing.Size(226, 16);
            this.LeftWithoutFinancialClearanceLabel.TabIndex = 543;
            this.LeftWithoutFinancialClearanceLabel.Text = "Left without financial clearance:";
            // 
            // LeftWithoutBeingSeenValueLabel
            // 
            this.LeftWithoutBeingSeenValueLabel.AutoSize = true;
            this.LeftWithoutBeingSeenValueLabel.Location = new System.Drawing.Point(167, 2085);
            this.LeftWithoutBeingSeenValueLabel.Name = "LeftWithoutBeingSeenValueLabel";
            this.LeftWithoutBeingSeenValueLabel.Size = new System.Drawing.Size(10, 13);
            this.LeftWithoutBeingSeenValueLabel.TabIndex = 542;
            this.LeftWithoutBeingSeenValueLabel.Text = " ";
            // 
            // LeftWithoutBeingSeenLabel
            // 
            // 
            this.LeftWithoutBeingSeenLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LeftWithoutBeingSeenLabel.Location = new System.Drawing.Point(7, 2085);
            this.LeftWithoutBeingSeenLabel.Name = "LeftWithoutBeingSeenLabel";
            this.LeftWithoutBeingSeenLabel.Size = new System.Drawing.Size(226, 16);
            this.LeftWithoutBeingSeenLabel.TabIndex = 541;
            this.LeftWithoutBeingSeenLabel.Text = "Left without being seen :";
            // 
            // RCRPValueLabel
            // 
            this.RCRPValueLabel.AutoSize = true;
            this.RCRPValueLabel.Location = new System.Drawing.Point(69, 2042);
            this.RCRPValueLabel.Name = "RCRPValueLabel";
            this.RCRPValueLabel.Size = new System.Drawing.Size(10, 13);
            this.RCRPValueLabel.TabIndex = 540;
            this.RCRPValueLabel.Text = " ";
            // 
            // RCRPLabel
            // 
            this.RCRPLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RCRPLabel.Location = new System.Drawing.Point(7, 2042);
            this.RCRPLabel.Name = "RCRPLabel";
            this.RCRPLabel.Size = new System.Drawing.Size(125, 13);
            this.RCRPLabel.TabIndex = 539;
            this.RCRPLabel.Text = "RCRP:";
            // 
            // LeftOrStayedValueLabel
            // 
            this.LeftOrStayedValueLabel.AutoSize = true;
            this.LeftOrStayedValueLabel.Location = new System.Drawing.Point(112, 2063);
            this.LeftOrStayedValueLabel.Name = "LeftOrStayedValueLabel";
            this.LeftOrStayedValueLabel.Size = new System.Drawing.Size(10, 13);
            this.LeftOrStayedValueLabel.TabIndex = 538;
            this.LeftOrStayedValueLabel.Text = " ";
            // 
            // LeftOrStayedLabel
            // 
            this.LeftOrStayedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LeftOrStayedLabel.Location = new System.Drawing.Point(7, 2063);
            this.LeftOrStayedLabel.Name = "LeftOrStayedLabel";
            this.LeftOrStayedLabel.Size = new System.Drawing.Size(125, 13);
            this.LeftOrStayedLabel.TabIndex = 537;
            this.LeftOrStayedLabel.Text = "Left or Stayed :";
            // 
            // alternatecareFacilityValueLabel
            // 
            this.alternatecareFacilityValueLabel.AutoSize = true;
            this.alternatecareFacilityValueLabel.Location = new System.Drawing.Point(238, 1323);
            this.alternatecareFacilityValueLabel.Name = "alternatecareFacilityValueLabel";
            this.alternatecareFacilityValueLabel.Size = new System.Drawing.Size(0, 13);
            this.alternatecareFacilityValueLabel.TabIndex = 536;
            // 
            // AlternatecareFacilityLabel
            // 
            this.AlternatecareFacilityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AlternatecareFacilityLabel.Location = new System.Drawing.Point(7, 1323);
            this.AlternatecareFacilityLabel.Name = "AlternatecareFacilityLabel";
            this.AlternatecareFacilityLabel.Size = new System.Drawing.Size(227, 15);
            this.AlternatecareFacilityLabel.TabIndex = 535;
            this.AlternatecareFacilityLabel.Text = "Nursing Home/Alternate Care Facility:";
            // 
            // PatientInClinicalStudyValueLabel
            // 
            this.PatientInClinicalStudyValueLabel.AutoSize = true;
            this.PatientInClinicalStudyValueLabel.Location = new System.Drawing.Point(240, 1776);
            this.PatientInClinicalStudyValueLabel.Name = "PatientInClinicalStudyValueLabel";
            this.PatientInClinicalStudyValueLabel.Size = new System.Drawing.Size(0, 13);
            this.PatientInClinicalStudyValueLabel.TabIndex = 528;
            // 
            // PatientInClinicalREsearchLBL
            // 
            this.PatientInClinicalREsearchLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PatientInClinicalREsearchLBL.Location = new System.Drawing.Point(8, 1776);
            this.PatientInClinicalREsearchLBL.Name = "PatientInClinicalREsearchLBL";
            this.PatientInClinicalREsearchLBL.Size = new System.Drawing.Size(226, 16);
            this.PatientInClinicalREsearchLBL.TabIndex = 525;
            this.PatientInClinicalREsearchLBL.Text = "Is Patient In Clinical Research study:";
            // 
            // procedureValueLabel
            // 
            this.procedureValueLabel.AutoSize = true;
            this.procedureValueLabel.Location = new System.Drawing.Point(92, 1064);
            this.procedureValueLabel.Name = "procedureValueLabel";
            this.procedureValueLabel.Size = new System.Drawing.Size(10, 13);
            this.procedureValueLabel.TabIndex = 521;
            this.procedureValueLabel.Text = " ";
            // 
            // procedureLabel
            // 
            this.procedureLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.procedureLabel.Location = new System.Drawing.Point(7, 1064);
            this.procedureLabel.Name = "procedureLabel";
            this.procedureLabel.Size = new System.Drawing.Size(94, 16);
            this.procedureLabel.TabIndex = 520;
            this.procedureLabel.Text = "Procedure :";
            // 
            // preopDateValueLabel
            // 
            this.preopDateValueLabel.AutoSize = true;
            this.preopDateValueLabel.Location = new System.Drawing.Point(94, 101);
            this.preopDateValueLabel.Name = "preopDateValueLabel";
            this.preopDateValueLabel.Size = new System.Drawing.Size(10, 13);
            this.preopDateValueLabel.TabIndex = 519;
            this.preopDateValueLabel.Text = " ";
            // 
            // preopDateLabel
            // 
            this.preopDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.preopDateLabel.Location = new System.Drawing.Point(7, 101);
            this.preopDateLabel.Name = "preopDateLabel";
            this.preopDateLabel.Size = new System.Drawing.Size(95, 16);
            this.preopDateLabel.TabIndex = 518;
            this.preopDateLabel.Text = "Pre-op Date:";
            // 
            // clinicalTrialValueLabel
            // 
            this.clinicalTrialValueLabel.AutoSize = true;
            this.clinicalTrialValueLabel.Location = new System.Drawing.Point(88, 1392);
            this.clinicalTrialValueLabel.Name = "clinicalTrialValueLabel";
            this.clinicalTrialValueLabel.Size = new System.Drawing.Size(10, 13);
            this.clinicalTrialValueLabel.TabIndex = 517;
            this.clinicalTrialValueLabel.Text = " ";
            // 
            // clinicalTrialLabel
            // 
            this.clinicalTrialLabel.AutoSize = true;
            this.clinicalTrialLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinicalTrialLabel.Location = new System.Drawing.Point(7, 1391);
            this.clinicalTrialLabel.Name = "clinicalTrialLabel";
            this.clinicalTrialLabel.Size = new System.Drawing.Size(77, 13);
            this.clinicalTrialLabel.TabIndex = 516;
            this.clinicalTrialLabel.Text = "Clinical trial:";
            // 
            // secondaryServicesAuthorizedValueLabel
            // 
            this.secondaryServicesAuthorizedValueLabel.AutoSize = true;
            this.secondaryServicesAuthorizedValueLabel.Location = new System.Drawing.Point(139, 5075);
            this.secondaryServicesAuthorizedValueLabel.Name = "secondaryServicesAuthorizedValueLabel";
            this.secondaryServicesAuthorizedValueLabel.Size = new System.Drawing.Size(10, 13);
            this.secondaryServicesAuthorizedValueLabel.TabIndex = 515;
            this.secondaryServicesAuthorizedValueLabel.Text = " ";
            // 
            // secondaryServicesAuthorizedLabel
            // 
            this.secondaryServicesAuthorizedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryServicesAuthorizedLabel.Location = new System.Drawing.Point(7, 5075);
            this.secondaryServicesAuthorizedLabel.Name = "secondaryServicesAuthorizedLabel";
            this.secondaryServicesAuthorizedLabel.Size = new System.Drawing.Size(164, 16);
            this.secondaryServicesAuthorizedLabel.TabIndex = 514;
            this.secondaryServicesAuthorizedLabel.Text = "Services Authorized";
            // 
            // secondaryRemarksValueLabel
            // 
            this.secondaryRemarksValueLabel.AutoSize = true;
            this.secondaryRemarksValueLabel.Location = new System.Drawing.Point(69, 5179);
            this.secondaryRemarksValueLabel.Name = "secondaryRemarksValueLabel";
            this.secondaryRemarksValueLabel.Size = new System.Drawing.Size(10, 13);
            this.secondaryRemarksValueLabel.TabIndex = 513;
            this.secondaryRemarksValueLabel.Text = " ";
            // 
            // secondaryRemarksLabel
            // 
            this.secondaryRemarksLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryRemarksLabel.Location = new System.Drawing.Point(7, 5179);
            this.secondaryRemarksLabel.Name = "secondaryRemarksLabel";
            this.secondaryRemarksLabel.Size = new System.Drawing.Size(104, 16);
            this.secondaryRemarksLabel.TabIndex = 512;
            this.secondaryRemarksLabel.Text = "Remarks";
            // 
            // secondaryInsuranceAuthorizationPanel
            // 
            this.secondaryInsuranceAuthorizationPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.secondaryInsuranceAuthorizationPanel.Location = new System.Drawing.Point(7, 4960);
            this.secondaryInsuranceAuthorizationPanel.Name = "secondaryInsuranceAuthorizationPanel";
            this.secondaryInsuranceAuthorizationPanel.Size = new System.Drawing.Size(600, 1);
            this.secondaryInsuranceAuthorizationPanel.TabIndex = 511;
            // 
            // secondaryAuthorizationStatusValueLabel
            // 
            this.secondaryAuthorizationStatusValueLabel.AutoSize = true;
            this.secondaryAuthorizationStatusValueLabel.Location = new System.Drawing.Point(134, 5153);
            this.secondaryAuthorizationStatusValueLabel.Name = "secondaryAuthorizationStatusValueLabel";
            this.secondaryAuthorizationStatusValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAuthorizationStatusValueLabel.TabIndex = 510;
            // 
            // secondaryExpirationDateOfAuthorizationValueLabel
            // 
            this.secondaryExpirationDateOfAuthorizationValueLabel.AutoSize = true;
            this.secondaryExpirationDateOfAuthorizationValueLabel.Location = new System.Drawing.Point(200, 5127);
            this.secondaryExpirationDateOfAuthorizationValueLabel.Name = "secondaryExpirationDateOfAuthorizationValueLabel";
            this.secondaryExpirationDateOfAuthorizationValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryExpirationDateOfAuthorizationValueLabel.TabIndex = 509;
            // 
            // secondaryEffectiveDateOfAuthorizationValueLabel
            // 
            this.secondaryEffectiveDateOfAuthorizationValueLabel.AutoSize = true;
            this.secondaryEffectiveDateOfAuthorizationValueLabel.Location = new System.Drawing.Point(200, 5101);
            this.secondaryEffectiveDateOfAuthorizationValueLabel.Name = "secondaryEffectiveDateOfAuthorizationValueLabel";
            this.secondaryEffectiveDateOfAuthorizationValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryEffectiveDateOfAuthorizationValueLabel.TabIndex = 508;
            // 
            // secondaryAuthorizationCompanyRepresentativeNameValueLabel
            // 
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.AutoSize = true;
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.Location = new System.Drawing.Point(271, 5023);
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.Name = "secondaryAuthorizationCompanyRepresentativeNameValueLabel";
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAuthorizationCompanyRepresentativeNameValueLabel.TabIndex = 507;
            // 
            // secondaryAuthorizationTrackingNumberValueLabel
            // 
            this.secondaryAuthorizationTrackingNumberValueLabel.AutoSize = true;
            this.secondaryAuthorizationTrackingNumberValueLabel.Location = new System.Drawing.Point(114, 4971);
            this.secondaryAuthorizationTrackingNumberValueLabel.Name = "secondaryAuthorizationTrackingNumberValueLabel";
            this.secondaryAuthorizationTrackingNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAuthorizationTrackingNumberValueLabel.TabIndex = 506;
            // 
            // secondaryDaysAuthorizedValueLabel
            // 
            this.secondaryDaysAuthorizedValueLabel.AutoSize = true;
            this.secondaryDaysAuthorizedValueLabel.Location = new System.Drawing.Point(111, 5049);
            this.secondaryDaysAuthorizedValueLabel.Name = "secondaryDaysAuthorizedValueLabel";
            this.secondaryDaysAuthorizedValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryDaysAuthorizedValueLabel.TabIndex = 505;
            // 
            // secondaryAuthorizationNumberValueLabel
            // 
            this.secondaryAuthorizationNumberValueLabel.Location = new System.Drawing.Point(139, 4997);
            this.secondaryAuthorizationNumberValueLabel.Name = "secondaryAuthorizationNumberValueLabel";
            this.secondaryAuthorizationNumberValueLabel.Size = new System.Drawing.Size(200, 16);
            this.secondaryAuthorizationNumberValueLabel.TabIndex = 504;
            // 
            // secondaryAuthorizationStatusLabel
            // 
            this.secondaryAuthorizationStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationStatusLabel.Location = new System.Drawing.Point(8, 5153);
            this.secondaryAuthorizationStatusLabel.Name = "secondaryAuthorizationStatusLabel";
            this.secondaryAuthorizationStatusLabel.Size = new System.Drawing.Size(193, 16);
            this.secondaryAuthorizationStatusLabel.TabIndex = 503;
            this.secondaryAuthorizationStatusLabel.Text = "Authorization Status";
            // 
            // secondaryExpirationDateOfAuthorizationLabel
            // 
            this.secondaryExpirationDateOfAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryExpirationDateOfAuthorizationLabel.Location = new System.Drawing.Point(7, 5127);
            this.secondaryExpirationDateOfAuthorizationLabel.Name = "secondaryExpirationDateOfAuthorizationLabel";
            this.secondaryExpirationDateOfAuthorizationLabel.Size = new System.Drawing.Size(213, 16);
            this.secondaryExpirationDateOfAuthorizationLabel.TabIndex = 502;
            this.secondaryExpirationDateOfAuthorizationLabel.Text = "Expiration Date of Authorization";
            // 
            // secondaryEffectiveDateOfAuthorizationLabel
            // 
            this.secondaryEffectiveDateOfAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEffectiveDateOfAuthorizationLabel.Location = new System.Drawing.Point(7, 5101);
            this.secondaryEffectiveDateOfAuthorizationLabel.Name = "secondaryEffectiveDateOfAuthorizationLabel";
            this.secondaryEffectiveDateOfAuthorizationLabel.Size = new System.Drawing.Size(193, 16);
            this.secondaryEffectiveDateOfAuthorizationLabel.TabIndex = 501;
            this.secondaryEffectiveDateOfAuthorizationLabel.Text = "Effective Date of Authorization";
            // 
            // secondaryAuthorizationCompanyRepresentativeNameLabel
            // 
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Location = new System.Drawing.Point(7, 5023);
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Name = "secondaryAuthorizationCompanyRepresentativeNameLabel";
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Size = new System.Drawing.Size(275, 16);
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.TabIndex = 500;
            this.secondaryAuthorizationCompanyRepresentativeNameLabel.Text = "Authorization Company Representative Name";
            // 
            // secondaryAuthorizationTrackingNumberLabel
            // 
            this.secondaryAuthorizationTrackingNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationTrackingNumberLabel.Location = new System.Drawing.Point(7, 4971);
            this.secondaryAuthorizationTrackingNumberLabel.Name = "secondaryAuthorizationTrackingNumberLabel";
            this.secondaryAuthorizationTrackingNumberLabel.Size = new System.Drawing.Size(127, 16);
            this.secondaryAuthorizationTrackingNumberLabel.TabIndex = 499;
            this.secondaryAuthorizationTrackingNumberLabel.Text = "Tracking Number";
            // 
            // secondaryDaysAuthorizedLabel
            // 
            this.secondaryDaysAuthorizedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryDaysAuthorizedLabel.Location = new System.Drawing.Point(7, 5049);
            this.secondaryDaysAuthorizedLabel.Name = "secondaryDaysAuthorizedLabel";
            this.secondaryDaysAuthorizedLabel.Size = new System.Drawing.Size(164, 16);
            this.secondaryDaysAuthorizedLabel.TabIndex = 498;
            this.secondaryDaysAuthorizedLabel.Text = "Days Authorized";
            // 
            // secondaryAuthorizationNumberLabel
            // 
            this.secondaryAuthorizationNumberLabel.BackColor = System.Drawing.Color.White;
            this.secondaryAuthorizationNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationNumberLabel.Location = new System.Drawing.Point(7, 4997);
            this.secondaryAuthorizationNumberLabel.Name = "secondaryAuthorizationNumberLabel";
            this.secondaryAuthorizationNumberLabel.Size = new System.Drawing.Size(132, 16);
            this.secondaryAuthorizationNumberLabel.TabIndex = 497;
            this.secondaryAuthorizationNumberLabel.Text = "Authorization Number";
            // 
            // secondaryInsuranceAuthorizationLabel
            // 
            this.secondaryInsuranceAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsuranceAuthorizationLabel.Location = new System.Drawing.Point(7, 4935);
            this.secondaryInsuranceAuthorizationLabel.Name = "secondaryInsuranceAuthorizationLabel";
            this.secondaryInsuranceAuthorizationLabel.Size = new System.Drawing.Size(339, 20);
            this.secondaryInsuranceAuthorizationLabel.TabIndex = 496;
            this.secondaryInsuranceAuthorizationLabel.Text = "Secondary  Insurance - Authorization";
            // 
            // primaryServicesAuthorizedValueLabel
            // 
            this.primaryServicesAuthorizedValueLabel.AutoSize = true;
            this.primaryServicesAuthorizedValueLabel.Location = new System.Drawing.Point(139, 3627);
            this.primaryServicesAuthorizedValueLabel.Name = "primaryServicesAuthorizedValueLabel";
            this.primaryServicesAuthorizedValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryServicesAuthorizedValueLabel.TabIndex = 495;
            // 
            // primaryServicesAuthorizedLabel
            // 
            this.primaryServicesAuthorizedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryServicesAuthorizedLabel.Location = new System.Drawing.Point(7, 3627);
            this.primaryServicesAuthorizedLabel.Name = "primaryServicesAuthorizedLabel";
            this.primaryServicesAuthorizedLabel.Size = new System.Drawing.Size(164, 16);
            this.primaryServicesAuthorizedLabel.TabIndex = 494;
            this.primaryServicesAuthorizedLabel.Text = "Services Authorized:";
            // 
            // primaryRemarksValueLabel
            // 
            this.primaryRemarksValueLabel.AutoSize = true;
            this.primaryRemarksValueLabel.Location = new System.Drawing.Point(69, 3731);
            this.primaryRemarksValueLabel.Name = "primaryRemarksValueLabel";
            this.primaryRemarksValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryRemarksValueLabel.TabIndex = 493;
            // 
            // primaryRemarksLabel
            // 
            this.primaryRemarksLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryRemarksLabel.Location = new System.Drawing.Point(7, 3731);
            this.primaryRemarksLabel.Name = "primaryRemarksLabel";
            this.primaryRemarksLabel.Size = new System.Drawing.Size(104, 16);
            this.primaryRemarksLabel.TabIndex = 492;
            this.primaryRemarksLabel.Text = "Remarks:";
            // 
            // primaryInsuranceAuthorizationPanel
            // 
            this.primaryInsuranceAuthorizationPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.primaryInsuranceAuthorizationPanel.Location = new System.Drawing.Point(7, 3512);
            this.primaryInsuranceAuthorizationPanel.Name = "primaryInsuranceAuthorizationPanel";
            this.primaryInsuranceAuthorizationPanel.Size = new System.Drawing.Size(600, 1);
            this.primaryInsuranceAuthorizationPanel.TabIndex = 491;
            // 
            // primaryAuthorizationStatusValueLabel
            // 
            this.primaryAuthorizationStatusValueLabel.AutoSize = true;
            this.primaryAuthorizationStatusValueLabel.Location = new System.Drawing.Point(134, 3705);
            this.primaryAuthorizationStatusValueLabel.Name = "primaryAuthorizationStatusValueLabel";
            this.primaryAuthorizationStatusValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAuthorizationStatusValueLabel.TabIndex = 490;
            // 
            // primaryExpirationDateOfAuthorizationValueLabel
            // 
            this.primaryExpirationDateOfAuthorizationValueLabel.AutoSize = true;
            this.primaryExpirationDateOfAuthorizationValueLabel.Location = new System.Drawing.Point(200, 3679);
            this.primaryExpirationDateOfAuthorizationValueLabel.Name = "primaryExpirationDateOfAuthorizationValueLabel";
            this.primaryExpirationDateOfAuthorizationValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryExpirationDateOfAuthorizationValueLabel.TabIndex = 489;
            // 
            // primaryEffectiveDateOfAuthorizationValueLabel
            // 
            this.primaryEffectiveDateOfAuthorizationValueLabel.AutoSize = true;
            this.primaryEffectiveDateOfAuthorizationValueLabel.Location = new System.Drawing.Point(200, 3653);
            this.primaryEffectiveDateOfAuthorizationValueLabel.Name = "primaryEffectiveDateOfAuthorizationValueLabel";
            this.primaryEffectiveDateOfAuthorizationValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryEffectiveDateOfAuthorizationValueLabel.TabIndex = 488;
            // 
            // primaryAuthorizationCompanyRepresentativeNameValueLabel
            // 
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.AutoSize = true;
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.Location = new System.Drawing.Point(271, 3575);
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.Name = "primaryAuthorizationCompanyRepresentativeNameValueLabel";
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAuthorizationCompanyRepresentativeNameValueLabel.TabIndex = 487;
            // 
            // primaryAuthorizationTrackingNumberValueLabel
            // 
            this.primaryAuthorizationTrackingNumberValueLabel.AutoSize = true;
            this.primaryAuthorizationTrackingNumberValueLabel.Location = new System.Drawing.Point(114, 3523);
            this.primaryAuthorizationTrackingNumberValueLabel.Name = "primaryAuthorizationTrackingNumberValueLabel";
            this.primaryAuthorizationTrackingNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAuthorizationTrackingNumberValueLabel.TabIndex = 486;
            // 
            // primaryDaysAuthorizedValueLabel
            // 
            this.primaryDaysAuthorizedValueLabel.AutoSize = true;
            this.primaryDaysAuthorizedValueLabel.Location = new System.Drawing.Point(111, 3601);
            this.primaryDaysAuthorizedValueLabel.Name = "primaryDaysAuthorizedValueLabel";
            this.primaryDaysAuthorizedValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryDaysAuthorizedValueLabel.TabIndex = 485;
            // 
            // primaryAuthorizationNumberValueLabel
            // 
            this.primaryAuthorizationNumberValueLabel.Location = new System.Drawing.Point(139, 3549);
            this.primaryAuthorizationNumberValueLabel.Name = "primaryAuthorizationNumberValueLabel";
            this.primaryAuthorizationNumberValueLabel.Size = new System.Drawing.Size(200, 16);
            this.primaryAuthorizationNumberValueLabel.TabIndex = 484;
            // 
            // primaryAuthorizationStatusLabel
            // 
            this.primaryAuthorizationStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationStatusLabel.Location = new System.Drawing.Point(8, 3705);
            this.primaryAuthorizationStatusLabel.Name = "primaryAuthorizationStatusLabel";
            this.primaryAuthorizationStatusLabel.Size = new System.Drawing.Size(193, 16);
            this.primaryAuthorizationStatusLabel.TabIndex = 483;
            this.primaryAuthorizationStatusLabel.Text = "Authorization Status:";
            // 
            // primaryExpirationDateOfAuthorizationLabel
            // 
            this.primaryExpirationDateOfAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryExpirationDateOfAuthorizationLabel.Location = new System.Drawing.Point(7, 3679);
            this.primaryExpirationDateOfAuthorizationLabel.Name = "primaryExpirationDateOfAuthorizationLabel";
            this.primaryExpirationDateOfAuthorizationLabel.Size = new System.Drawing.Size(213, 16);
            this.primaryExpirationDateOfAuthorizationLabel.TabIndex = 482;
            this.primaryExpirationDateOfAuthorizationLabel.Text = "Expiration Date of Authorization:";
            // 
            // primaryEffectiveDateOfAuthorizationLabel
            // 
            this.primaryEffectiveDateOfAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEffectiveDateOfAuthorizationLabel.Location = new System.Drawing.Point(7, 3653);
            this.primaryEffectiveDateOfAuthorizationLabel.Name = "primaryEffectiveDateOfAuthorizationLabel";
            this.primaryEffectiveDateOfAuthorizationLabel.Size = new System.Drawing.Size(193, 16);
            this.primaryEffectiveDateOfAuthorizationLabel.TabIndex = 481;
            this.primaryEffectiveDateOfAuthorizationLabel.Text = "Effective Date of Authorization:";
            // 
            // primaryAuthorizationCompanyRepresentativeNameLabel
            // 
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Location = new System.Drawing.Point(7, 3575);
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Name = "primaryAuthorizationCompanyRepresentativeNameLabel";
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Size = new System.Drawing.Size(275, 16);
            this.primaryAuthorizationCompanyRepresentativeNameLabel.TabIndex = 480;
            this.primaryAuthorizationCompanyRepresentativeNameLabel.Text = "Authorization Company Representative Name:";
            // 
            // primaryTrackingNumberLabel
            // 
            this.primaryTrackingNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryTrackingNumberLabel.Location = new System.Drawing.Point(7, 3523);
            this.primaryTrackingNumberLabel.Name = "primaryTrackingNumberLabel";
            this.primaryTrackingNumberLabel.Size = new System.Drawing.Size(127, 16);
            this.primaryTrackingNumberLabel.TabIndex = 479;
            this.primaryTrackingNumberLabel.Text = "Tracking Number:";
            // 
            // primaryDaysAuthorizedLabel
            // 
            this.primaryDaysAuthorizedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryDaysAuthorizedLabel.Location = new System.Drawing.Point(7, 3601);
            this.primaryDaysAuthorizedLabel.Name = "primaryDaysAuthorizedLabel";
            this.primaryDaysAuthorizedLabel.Size = new System.Drawing.Size(164, 16);
            this.primaryDaysAuthorizedLabel.TabIndex = 478;
            this.primaryDaysAuthorizedLabel.Text = "Days Authorized:";
            // 
            // primaryAuthorizationNumberLabel
            // 
            this.primaryAuthorizationNumberLabel.BackColor = System.Drawing.Color.White;
            this.primaryAuthorizationNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationNumberLabel.Location = new System.Drawing.Point(7, 3549);
            this.primaryAuthorizationNumberLabel.Name = "primaryAuthorizationNumberLabel";
            this.primaryAuthorizationNumberLabel.Size = new System.Drawing.Size(132, 16);
            this.primaryAuthorizationNumberLabel.TabIndex = 477;
            this.primaryAuthorizationNumberLabel.Text = "Authorization Number:";
            // 
            // primaryInsuranceAuthorizationLabel
            // 
            this.primaryInsuranceAuthorizationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsuranceAuthorizationLabel.Location = new System.Drawing.Point(7, 3487);
            this.primaryInsuranceAuthorizationLabel.Name = "primaryInsuranceAuthorizationLabel";
            this.primaryInsuranceAuthorizationLabel.Size = new System.Drawing.Size(339, 20);
            this.primaryInsuranceAuthorizationLabel.TabIndex = 476;
            this.primaryInsuranceAuthorizationLabel.Text = "Primary Insurance - Authorization";
            // 
            // nppSignatureStatusValueLabel
            // 
            this.nppSignatureStatusValueLabel.AutoSize = true;
            this.nppSignatureStatusValueLabel.Location = new System.Drawing.Point(143, 6721);
            this.nppSignatureStatusValueLabel.Name = "nppSignatureStatusValueLabel";
            this.nppSignatureStatusValueLabel.Size = new System.Drawing.Size(0, 13);
            this.nppSignatureStatusValueLabel.TabIndex = 424;
            // 
            // nppSignatureStatusLabel
            // 
            this.nppSignatureStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nppSignatureStatusLabel.Location = new System.Drawing.Point(7, 6721);
            this.nppSignatureStatusLabel.Name = "nppSignatureStatusLabel";
            this.nppSignatureStatusLabel.Size = new System.Drawing.Size(137, 16);
            this.nppSignatureStatusLabel.TabIndex = 423;
            this.nppSignatureStatusLabel.Text = "NPP signature status:";
            // 
            // spanCode2ValueLabel
            // 
            this.spanCode2ValueLabel.AutoSize = true;
            this.spanCode2ValueLabel.Location = new System.Drawing.Point(90, 6067);
            this.spanCode2ValueLabel.Name = "spanCode2ValueLabel";
            this.spanCode2ValueLabel.Size = new System.Drawing.Size(0, 13);
            this.spanCode2ValueLabel.TabIndex = 378;
            // 
            // spanCode1ValueLabel
            // 
            this.spanCode1ValueLabel.AutoSize = true;
            this.spanCode1ValueLabel.Location = new System.Drawing.Point(90, 6015);
            this.spanCode1ValueLabel.Name = "spanCode1ValueLabel";
            this.spanCode1ValueLabel.Size = new System.Drawing.Size(0, 13);
            this.spanCode1ValueLabel.TabIndex = 370;
            // 
            // occurrenceCode8DateLabel
            // 
            this.occurrenceCode8DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode8DateLabel.Location = new System.Drawing.Point(400, 5807);
            this.occurrenceCode8DateLabel.Name = "occurrenceCode8DateLabel";
            this.occurrenceCode8DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode8DateLabel.TabIndex = 361;
            this.occurrenceCode8DateLabel.Text = "Date:";
            // 
            // occurrenceCode7DateLabel
            // 
            this.occurrenceCode7DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode7DateLabel.Location = new System.Drawing.Point(400, 5781);
            this.occurrenceCode7DateLabel.Name = "occurrenceCode7DateLabel";
            this.occurrenceCode7DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode7DateLabel.TabIndex = 359;
            this.occurrenceCode7DateLabel.Text = "Date:";
            // 
            // occurrenceCode5DateLabel
            // 
            this.occurrenceCode5DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode5DateLabel.Location = new System.Drawing.Point(400, 5729);
            this.occurrenceCode5DateLabel.Name = "occurrenceCode5DateLabel";
            this.occurrenceCode5DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode5DateLabel.TabIndex = 355;
            this.occurrenceCode5DateLabel.Text = "Date:";
            // 
            // occurrenceCode6DateLabel
            // 
            this.occurrenceCode6DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode6DateLabel.Location = new System.Drawing.Point(400, 5755);
            this.occurrenceCode6DateLabel.Name = "occurrenceCode6DateLabel";
            this.occurrenceCode6DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode6DateLabel.TabIndex = 357;
            this.occurrenceCode6DateLabel.Text = "Date:";
            // 
            // occurrenceCode4DateLabel
            // 
            this.occurrenceCode4DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode4DateLabel.Location = new System.Drawing.Point(400, 5703);
            this.occurrenceCode4DateLabel.Name = "occurrenceCode4DateLabel";
            this.occurrenceCode4DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode4DateLabel.TabIndex = 353;
            this.occurrenceCode4DateLabel.Text = "Date:";
            // 
            // occurrenceCode3DateLabel
            // 
            this.occurrenceCode3DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode3DateLabel.Location = new System.Drawing.Point(400, 5677);
            this.occurrenceCode3DateLabel.Name = "occurrenceCode3DateLabel";
            this.occurrenceCode3DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode3DateLabel.TabIndex = 351;
            this.occurrenceCode3DateLabel.Text = "Date:";
            // 
            // occurrenceCode2DateLabel
            // 
            this.occurrenceCode2DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode2DateLabel.Location = new System.Drawing.Point(400, 5651);
            this.occurrenceCode2DateLabel.Name = "occurrenceCode2DateLabel";
            this.occurrenceCode2DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode2DateLabel.TabIndex = 349;
            this.occurrenceCode2DateLabel.Text = "Date:";
            // 
            // occurrenceCode1DateLabel
            // 
            this.occurrenceCode1DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode1DateLabel.Location = new System.Drawing.Point(400, 5625);
            this.occurrenceCode1DateLabel.Name = "occurrenceCode1DateLabel";
            this.occurrenceCode1DateLabel.Size = new System.Drawing.Size(40, 16);
            this.occurrenceCode1DateLabel.TabIndex = 347;
            this.occurrenceCode1DateLabel.Text = "Date:";
            // 
            // secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel
            // 
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.AutoSize = true;
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Location = new System.Drawing.Point(256, 4905);
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Name = "secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel";
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAutoHomeInsuranceAgentPhoneNumberValueLabel.TabIndex = 317;
            // 
            // secondaryAutoHomeInsuranceAgentAddressValueLabel
            // 
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.AutoSize = true;
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.Location = new System.Drawing.Point(224, 4879);
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.Name = "secondaryAutoHomeInsuranceAgentAddressValueLabel";
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAutoHomeInsuranceAgentAddressValueLabel.TabIndex = 315;
            // 
            // secondaryAutoHomeInsuranceAgentNameValueLabel
            // 
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.AutoSize = true;
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.Location = new System.Drawing.Point(208, 4853);
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.Name = "secondaryAutoHomeInsuranceAgentNameValueLabel";
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAutoHomeInsuranceAgentNameValueLabel.TabIndex = 313;
            // 
            // secondaryAttorneyPhoneNumberValueLabel
            // 
            this.secondaryAttorneyPhoneNumberValueLabel.AutoSize = true;
            this.secondaryAttorneyPhoneNumberValueLabel.Location = new System.Drawing.Point(152, 4827);
            this.secondaryAttorneyPhoneNumberValueLabel.Name = "secondaryAttorneyPhoneNumberValueLabel";
            this.secondaryAttorneyPhoneNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAttorneyPhoneNumberValueLabel.TabIndex = 311;
            // 
            // secondaryAttorneyAddressValueLabel
            // 
            this.secondaryAttorneyAddressValueLabel.AutoSize = true;
            this.secondaryAttorneyAddressValueLabel.Location = new System.Drawing.Point(112, 4801);
            this.secondaryAttorneyAddressValueLabel.Name = "secondaryAttorneyAddressValueLabel";
            this.secondaryAttorneyAddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAttorneyAddressValueLabel.TabIndex = 309;
            // 
            // secondaryAttorneyNameValueLabel
            // 
            this.secondaryAttorneyNameValueLabel.AutoSize = true;
            this.secondaryAttorneyNameValueLabel.Location = new System.Drawing.Point(104, 4775);
            this.secondaryAttorneyNameValueLabel.Name = "secondaryAttorneyNameValueLabel";
            this.secondaryAttorneyNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAttorneyNameValueLabel.TabIndex = 307;
            // 
            // secondaryPromptExtValueLabel
            // 
            this.secondaryPromptExtValueLabel.AutoSize = true;
            this.secondaryPromptExtValueLabel.Location = new System.Drawing.Point(88, 4749);
            this.secondaryPromptExtValueLabel.Name = "secondaryPromptExtValueLabel";
            this.secondaryPromptExtValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryPromptExtValueLabel.TabIndex = 305;
            // 
            // secondaryAutoHomeInsuranceAgentPhoneNumberLabel
            // 
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Location = new System.Drawing.Point(8, 4905);
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Name = "secondaryAutoHomeInsuranceAgentPhoneNumberLabel";
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Size = new System.Drawing.Size(249, 16);
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.TabIndex = 316;
            this.secondaryAutoHomeInsuranceAgentPhoneNumberLabel.Text = "Auto/Home Insurance Agent phone number:";
            // 
            // secondaryAutoHomeInsuranceAgentAddressLabel
            // 
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Location = new System.Drawing.Point(8, 4879);
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Name = "secondaryAutoHomeInsuranceAgentAddressLabel";
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Size = new System.Drawing.Size(222, 16);
            this.secondaryAutoHomeInsuranceAgentAddressLabel.TabIndex = 314;
            this.secondaryAutoHomeInsuranceAgentAddressLabel.Text = "Auto/Home Insurance Agent address:";
            // 
            // secondaryAutoHomeInsuranceAgentNameLabel
            // 
            this.secondaryAutoHomeInsuranceAgentNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAutoHomeInsuranceAgentNameLabel.Location = new System.Drawing.Point(8, 4853);
            this.secondaryAutoHomeInsuranceAgentNameLabel.Name = "secondaryAutoHomeInsuranceAgentNameLabel";
            this.secondaryAutoHomeInsuranceAgentNameLabel.Size = new System.Drawing.Size(210, 16);
            this.secondaryAutoHomeInsuranceAgentNameLabel.TabIndex = 312;
            this.secondaryAutoHomeInsuranceAgentNameLabel.Text = "Auto/Home Insurance Agent name:";
            // 
            // secondaryAttorneyPhoneNumberLabel
            // 
            this.secondaryAttorneyPhoneNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAttorneyPhoneNumberLabel.Location = new System.Drawing.Point(8, 4827);
            this.secondaryAttorneyPhoneNumberLabel.Name = "secondaryAttorneyPhoneNumberLabel";
            this.secondaryAttorneyPhoneNumberLabel.Size = new System.Drawing.Size(137, 16);
            this.secondaryAttorneyPhoneNumberLabel.TabIndex = 310;
            this.secondaryAttorneyPhoneNumberLabel.Text = "Attorney phone number:";
            // 
            // secondaryAttorneyAddressLabel
            // 
            this.secondaryAttorneyAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAttorneyAddressLabel.Location = new System.Drawing.Point(8, 4801);
            this.secondaryAttorneyAddressLabel.Name = "secondaryAttorneyAddressLabel";
            this.secondaryAttorneyAddressLabel.Size = new System.Drawing.Size(104, 16);
            this.secondaryAttorneyAddressLabel.TabIndex = 308;
            this.secondaryAttorneyAddressLabel.Text = "Attorney address:";
            // 
            // secondaryAttorneyNameLabel
            // 
            this.secondaryAttorneyNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAttorneyNameLabel.Location = new System.Drawing.Point(8, 4775);
            this.secondaryAttorneyNameLabel.Name = "secondaryAttorneyNameLabel";
            this.secondaryAttorneyNameLabel.Size = new System.Drawing.Size(91, 16);
            this.secondaryAttorneyNameLabel.TabIndex = 306;
            this.secondaryAttorneyNameLabel.Text = "Attorney name:";
            // 
            // regulatoryPanel
            // 
            this.regulatoryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.regulatoryPanel.Location = new System.Drawing.Point(7, 6685);
            this.regulatoryPanel.Name = "regulatoryPanel";
            this.regulatoryPanel.Size = new System.Drawing.Size(600, 1);
            this.regulatoryPanel.TabIndex = 457;
            // 
            // contactsPanel
            // 
            this.contactsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.contactsPanel.Location = new System.Drawing.Point(7, 6436);
            this.contactsPanel.Name = "contactsPanel";
            this.contactsPanel.Size = new System.Drawing.Size(600, 1);
            this.contactsPanel.TabIndex = 456;
            // 
            // paymentPanel
            // 
            this.paymentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paymentPanel.Location = new System.Drawing.Point(7, 6289);
            this.paymentPanel.Name = "paymentPanel";
            this.paymentPanel.Size = new System.Drawing.Size(600, 1);
            this.paymentPanel.TabIndex = 455;
            // 
            // liabilityPanel
            // 
            this.liabilityPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.liabilityPanel.Location = new System.Drawing.Point(7, 6122);
            this.liabilityPanel.Name = "liabilityPanel";
            this.liabilityPanel.Size = new System.Drawing.Size(600, 1);
            this.liabilityPanel.TabIndex = 454;
            // 
            // billingPanel
            // 
            this.billingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.billingPanel.Location = new System.Drawing.Point(7, 5614);
            this.billingPanel.Name = "billingPanel";
            this.billingPanel.Size = new System.Drawing.Size(600, 1);
            this.billingPanel.TabIndex = 453;
            // 
            // guarantorPanel
            // 
            this.guarantorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.guarantorPanel.Location = new System.Drawing.Point(7, 5234);
            this.guarantorPanel.Name = "guarantorPanel";
            this.guarantorPanel.Size = new System.Drawing.Size(600, 1);
            this.guarantorPanel.TabIndex = 452;
            // 
            // secondaryInsuranceVerificationPanel
            // 
            this.secondaryInsuranceVerificationPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.secondaryInsuranceVerificationPanel.Location = new System.Drawing.Point(7, 4582);
            this.secondaryInsuranceVerificationPanel.Name = "secondaryInsuranceVerificationPanel";
            this.secondaryInsuranceVerificationPanel.Size = new System.Drawing.Size(600, 1);
            this.secondaryInsuranceVerificationPanel.TabIndex = 451;
            // 
            // secondaryInsuranceInsuredPanel
            // 
            this.secondaryInsuranceInsuredPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.secondaryInsuranceInsuredPanel.Location = new System.Drawing.Point(7, 4256);
            this.secondaryInsuranceInsuredPanel.Name = "secondaryInsuranceInsuredPanel";
            this.secondaryInsuranceInsuredPanel.Size = new System.Drawing.Size(600, 1);
            this.secondaryInsuranceInsuredPanel.TabIndex = 450;
            // 
            // secondaryInsurancePayorDetailsPanel
            // 
            this.secondaryInsurancePayorDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.secondaryInsurancePayorDetailsPanel.Location = new System.Drawing.Point(7, 3904);
            this.secondaryInsurancePayorDetailsPanel.Name = "secondaryInsurancePayorDetailsPanel";
            this.secondaryInsurancePayorDetailsPanel.Size = new System.Drawing.Size(600, 1);
            this.secondaryInsurancePayorDetailsPanel.TabIndex = 449;
            // 
            // secondaryInsurancePlanPanel
            // 
            this.secondaryInsurancePlanPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.secondaryInsurancePlanPanel.Location = new System.Drawing.Point(7, 3784);
            this.secondaryInsurancePlanPanel.Name = "secondaryInsurancePlanPanel";
            this.secondaryInsurancePlanPanel.Size = new System.Drawing.Size(600, 1);
            this.secondaryInsurancePlanPanel.TabIndex = 448;
            // 
            // primaryInsuranceVerificationPanel
            // 
            this.primaryInsuranceVerificationPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.primaryInsuranceVerificationPanel.Location = new System.Drawing.Point(7, 3134);
            this.primaryInsuranceVerificationPanel.Name = "primaryInsuranceVerificationPanel";
            this.primaryInsuranceVerificationPanel.Size = new System.Drawing.Size(600, 1);
            this.primaryInsuranceVerificationPanel.TabIndex = 447;
            // 
            // primaryInsuranceInsuredPanel
            // 
            this.primaryInsuranceInsuredPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.primaryInsuranceInsuredPanel.Location = new System.Drawing.Point(7, 2808);
            this.primaryInsuranceInsuredPanel.Name = "primaryInsuranceInsuredPanel";
            this.primaryInsuranceInsuredPanel.Size = new System.Drawing.Size(600, 1);
            this.primaryInsuranceInsuredPanel.TabIndex = 446;
            // 
            // primaryInsurancePayorDetailsPanel
            // 
            this.primaryInsurancePayorDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.primaryInsurancePayorDetailsPanel.Location = new System.Drawing.Point(7, 2456);
            this.primaryInsurancePayorDetailsPanel.Name = "primaryInsurancePayorDetailsPanel";
            this.primaryInsurancePayorDetailsPanel.Size = new System.Drawing.Size(600, 1);
            this.primaryInsurancePayorDetailsPanel.TabIndex = 445;
            // 
            // primaryInsurancePlanPanel
            // 
            this.primaryInsurancePlanPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.primaryInsurancePlanPanel.Location = new System.Drawing.Point(7, 2338);
            this.primaryInsurancePlanPanel.Name = "primaryInsurancePlanPanel";
            this.primaryInsurancePlanPanel.Size = new System.Drawing.Size(600, 1);
            this.primaryInsurancePlanPanel.TabIndex = 444;
            // 
            // mspPanel
            // 
            this.mspPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mspPanel.Location = new System.Drawing.Point(7, 2272);
            this.mspPanel.Name = "mspPanel";
            this.mspPanel.Size = new System.Drawing.Size(600, 1);
            this.mspPanel.TabIndex = 443;
            // 
            // generalInsuranceInformationPanel2
            // 
            this.generalInsuranceInformationPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.generalInsuranceInformationPanel2.Location = new System.Drawing.Point(7, 2161);
            this.generalInsuranceInformationPanel2.Name = "generalInsuranceInformationPanel2";
            this.generalInsuranceInformationPanel2.Size = new System.Drawing.Size(600, 1);
            this.generalInsuranceInformationPanel2.TabIndex = 442;
            // 
            // clinicalPanel
            // 
            this.clinicalPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clinicalPanel.Location = new System.Drawing.Point(10, 1552);
            this.clinicalPanel.Name = "clinicalPanel";
            this.clinicalPanel.Size = new System.Drawing.Size(600, 1);
            this.clinicalPanel.TabIndex = 441;
            // 
            // diagnosisPanel
            // 
            this.diagnosisPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.diagnosisPanel.Location = new System.Drawing.Point(3, 912);
            this.diagnosisPanel.Name = "diagnosisPanel";
            this.diagnosisPanel.Size = new System.Drawing.Size(600, 1);
            this.diagnosisPanel.TabIndex = 440;
            // 
            // patientDemographicsEmploymentPanel
            // 
            this.patientDemographicsEmploymentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.patientDemographicsEmploymentPanel.Location = new System.Drawing.Point(7, 32);
            this.patientDemographicsEmploymentPanel.Name = "patientDemographicsEmploymentPanel";
            this.patientDemographicsEmploymentPanel.Size = new System.Drawing.Size(600, 1);
            this.patientDemographicsEmploymentPanel.TabIndex = 439;
            // 
            // dischargeDispositionValueLabel
            // 
            this.dischargeDispositionValueLabel.AutoSize = true;
            this.dischargeDispositionValueLabel.Location = new System.Drawing.Point(140, 145);
            this.dischargeDispositionValueLabel.Name = "dischargeDispositionValueLabel";
            this.dischargeDispositionValueLabel.Size = new System.Drawing.Size(10, 13);
            this.dischargeDispositionValueLabel.TabIndex = 10;
            this.dischargeDispositionValueLabel.Text = " ";
            // 
            // dischargeDispositionLabel
            // 
            this.dischargeDispositionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dischargeDispositionLabel.Location = new System.Drawing.Point(7, 145);
            this.dischargeDispositionLabel.Name = "dischargeDispositionLabel";
            this.dischargeDispositionLabel.Size = new System.Drawing.Size(127, 16);
            this.dischargeDispositionLabel.TabIndex = 9;
            this.dischargeDispositionLabel.Text = "Discharge disposition:";
            // 
            // dischargeDateTimeValueLabel
            // 
            this.dischargeDateTimeValueLabel.AutoSize = true;
            this.dischargeDateTimeValueLabel.Location = new System.Drawing.Point(132, 122);
            this.dischargeDateTimeValueLabel.Name = "dischargeDateTimeValueLabel";
            this.dischargeDateTimeValueLabel.Size = new System.Drawing.Size(10, 13);
            this.dischargeDateTimeValueLabel.TabIndex = 8;
            this.dischargeDateTimeValueLabel.Text = " ";
            // 
            // dischargeDateTimeLabel
            // 
            this.dischargeDateTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dischargeDateTimeLabel.Location = new System.Drawing.Point(7, 122);
            this.dischargeDateTimeLabel.Name = "dischargeDateTimeLabel";
            this.dischargeDateTimeLabel.Size = new System.Drawing.Size(119, 16);
            this.dischargeDateTimeLabel.TabIndex = 7;
            this.dischargeDateTimeLabel.Text = "Discharge date/time:";
            // 
            // contactPhysicalAddressPhoneValueLabel
            // 
            this.contactPhysicalAddressPhoneValueLabel.AutoSize = true;
            this.contactPhysicalAddressPhoneValueLabel.Location = new System.Drawing.Point(215, 466);
            this.contactPhysicalAddressPhoneValueLabel.Name = "contactPhysicalAddressPhoneValueLabel";
            this.contactPhysicalAddressPhoneValueLabel.Size = new System.Drawing.Size(10, 13);
            this.contactPhysicalAddressPhoneValueLabel.TabIndex = 40;
            this.contactPhysicalAddressPhoneValueLabel.Text = " ";
            // 
            // contactPhysicalAddressPhoneLabel
            // 
            this.contactPhysicalAddressPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contactPhysicalAddressPhoneLabel.Location = new System.Drawing.Point(9, 466);
            this.contactPhysicalAddressPhoneLabel.Name = "contactPhysicalAddressPhoneLabel";
            this.contactPhysicalAddressPhoneLabel.Size = new System.Drawing.Size(200, 16);
            this.contactPhysicalAddressPhoneLabel.TabIndex = 39;
            this.contactPhysicalAddressPhoneLabel.Text = "Contact (physical address) phone:";
            // 
            // patientPhysicalAddressValueLabel
            // 
            this.patientPhysicalAddressValueLabel.AutoSize = true;
            this.patientPhysicalAddressValueLabel.Location = new System.Drawing.Point(158, 441);
            this.patientPhysicalAddressValueLabel.Name = "patientPhysicalAddressValueLabel";
            this.patientPhysicalAddressValueLabel.Size = new System.Drawing.Size(10, 13);
            this.patientPhysicalAddressValueLabel.TabIndex = 38;
            this.patientPhysicalAddressValueLabel.Text = " ";
            // 
            // patientPhysicalAddressLabel
            // 
            this.patientPhysicalAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientPhysicalAddressLabel.Location = new System.Drawing.Point(9, 441);
            this.patientPhysicalAddressLabel.Name = "patientPhysicalAddressLabel";
            this.patientPhysicalAddressLabel.Size = new System.Drawing.Size(143, 16);
            this.patientPhysicalAddressLabel.TabIndex = 37;
            this.patientPhysicalAddressLabel.Text = "Patient physical address:";
            // 
            // facilityDeterminedFlagValueLabel
            // 
            this.facilityDeterminedFlagValueLabel.AutoSize = true;
            this.facilityDeterminedFlagValueLabel.Location = new System.Drawing.Point(155, 6920);
            this.facilityDeterminedFlagValueLabel.Name = "facilityDeterminedFlagValueLabel";
            this.facilityDeterminedFlagValueLabel.Size = new System.Drawing.Size(0, 13);
            this.facilityDeterminedFlagValueLabel.TabIndex = 438;
            // 
            // cosSignedValueLabel
            // 
            this.cosSignedValueLabel.AutoSize = true;
            this.cosSignedValueLabel.Location = new System.Drawing.Point(84, 6898);
            this.cosSignedValueLabel.Name = "cosSignedValueLabel";
            this.cosSignedValueLabel.Size = new System.Drawing.Size(0, 13);
            this.cosSignedValueLabel.TabIndex = 436;
            // 
            // privacyOptOutPatientDirectoryReligionValueLabel
            // 
            this.privacyOptOutPatientDirectoryReligionValueLabel.AutoSize = true;
            this.privacyOptOutPatientDirectoryReligionValueLabel.Location = new System.Drawing.Point(364, 6851);
            this.privacyOptOutPatientDirectoryReligionValueLabel.Name = "privacyOptOutPatientDirectoryReligionValueLabel";
            this.privacyOptOutPatientDirectoryReligionValueLabel.Size = new System.Drawing.Size(0, 13);
            this.privacyOptOutPatientDirectoryReligionValueLabel.TabIndex = 434;
            // 
            // privacyOptOutPatientDirectoryHealthInformationValueLabel
            // 
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.AutoSize = true;
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.Location = new System.Drawing.Point(417, 6825);
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.Name = "privacyOptOutPatientDirectoryHealthInformationValueLabel";
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.Size = new System.Drawing.Size(0, 13);
            this.privacyOptOutPatientDirectoryHealthInformationValueLabel.TabIndex = 432;
            // 
            // privacyOptOutPatientDirectoryLocationValueLabel
            // 
            this.privacyOptOutPatientDirectoryLocationValueLabel.AutoSize = true;
            this.privacyOptOutPatientDirectoryLocationValueLabel.Location = new System.Drawing.Point(356, 6799);
            this.privacyOptOutPatientDirectoryLocationValueLabel.Name = "privacyOptOutPatientDirectoryLocationValueLabel";
            this.privacyOptOutPatientDirectoryLocationValueLabel.Size = new System.Drawing.Size(0, 13);
            this.privacyOptOutPatientDirectoryLocationValueLabel.TabIndex = 430;
            // 
            // privacyOptOutPatientDirectoryNameAndAllInformationValueLabel
            // 
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.AutoSize = true;
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.Location = new System.Drawing.Point(452, 6773);
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.Name = "privacyOptOutPatientDirectoryNameAndAllInformationValueLabel";
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.Size = new System.Drawing.Size(0, 13);
            this.privacyOptOutPatientDirectoryNameAndAllInformationValueLabel.TabIndex = 428;
            // 
            // privacyConfidentialStatusValueLabel
            // 
            this.privacyConfidentialStatusValueLabel.AutoSize = true;
            this.privacyConfidentialStatusValueLabel.Location = new System.Drawing.Point(187, 6747);
            this.privacyConfidentialStatusValueLabel.Name = "privacyConfidentialStatusValueLabel";
            this.privacyConfidentialStatusValueLabel.Size = new System.Drawing.Size(0, 13);
            this.privacyConfidentialStatusValueLabel.TabIndex = 426;
            // 
            // nppVersionValueLabel
            // 
            this.nppVersionValueLabel.AutoSize = true;
            this.nppVersionValueLabel.Location = new System.Drawing.Point(90, 6695);
            this.nppVersionValueLabel.Name = "nppVersionValueLabel";
            this.nppVersionValueLabel.Size = new System.Drawing.Size(0, 13);
            this.nppVersionValueLabel.TabIndex = 422;
            // 
            // emergencyContact2PhoneValueLabel
            // 
            this.emergencyContact2PhoneValueLabel.AutoSize = true;
            this.emergencyContact2PhoneValueLabel.Location = new System.Drawing.Point(117, 6629);
            this.emergencyContact2PhoneValueLabel.Name = "emergencyContact2PhoneValueLabel";
            this.emergencyContact2PhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.emergencyContact2PhoneValueLabel.TabIndex = 419;
            // 
            // emergencyContact2AddressValueLabel
            // 
            this.emergencyContact2AddressValueLabel.AutoSize = true;
            this.emergencyContact2AddressValueLabel.Location = new System.Drawing.Point(64, 6603);
            this.emergencyContact2AddressValueLabel.Name = "emergencyContact2AddressValueLabel";
            this.emergencyContact2AddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.emergencyContact2AddressValueLabel.TabIndex = 417;
            // 
            // emergencyContact2IsThePatientsValueLabel
            // 
            this.emergencyContact2IsThePatientsValueLabel.AutoSize = true;
            this.emergencyContact2IsThePatientsValueLabel.Location = new System.Drawing.Point(190, 6577);
            this.emergencyContact2IsThePatientsValueLabel.Name = "emergencyContact2IsThePatientsValueLabel";
            this.emergencyContact2IsThePatientsValueLabel.Size = new System.Drawing.Size(0, 13);
            this.emergencyContact2IsThePatientsValueLabel.TabIndex = 415;
            // 
            // emergencyContact2NameValueLabel
            // 
            this.emergencyContact2NameValueLabel.AutoSize = true;
            this.emergencyContact2NameValueLabel.Location = new System.Drawing.Point(181, 6551);
            this.emergencyContact2NameValueLabel.Name = "emergencyContact2NameValueLabel";
            this.emergencyContact2NameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.emergencyContact2NameValueLabel.TabIndex = 413;
            // 
            // emergencyContact1PhoneValueLabel
            // 
            this.emergencyContact1PhoneValueLabel.AutoSize = true;
            this.emergencyContact1PhoneValueLabel.Location = new System.Drawing.Point(112, 6525);
            this.emergencyContact1PhoneValueLabel.Name = "emergencyContact1PhoneValueLabel";
            this.emergencyContact1PhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.emergencyContact1PhoneValueLabel.TabIndex = 411;
            // 
            // emergencyContact1AddressValueLabel
            // 
            this.emergencyContact1AddressValueLabel.AutoSize = true;
            this.emergencyContact1AddressValueLabel.Location = new System.Drawing.Point(64, 6499);
            this.emergencyContact1AddressValueLabel.Name = "emergencyContact1AddressValueLabel";
            this.emergencyContact1AddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.emergencyContact1AddressValueLabel.TabIndex = 409;
            // 
            // emergencyContact1IsThePatientsValueLabel
            // 
            this.emergencyContact1IsThePatientsValueLabel.AutoSize = true;
            this.emergencyContact1IsThePatientsValueLabel.Location = new System.Drawing.Point(185, 6473);
            this.emergencyContact1IsThePatientsValueLabel.Name = "emergencyContact1IsThePatientsValueLabel";
            this.emergencyContact1IsThePatientsValueLabel.Size = new System.Drawing.Size(0, 13);
            this.emergencyContact1IsThePatientsValueLabel.TabIndex = 407;
            // 
            // emergencyContact1NameValueLabel
            // 
            this.emergencyContact1NameValueLabel.AutoSize = true;
            this.emergencyContact1NameValueLabel.Location = new System.Drawing.Point(181, 6447);
            this.emergencyContact1NameValueLabel.Name = "emergencyContact1NameValueLabel";
            this.emergencyContact1NameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.emergencyContact1NameValueLabel.TabIndex = 405;
            // 
            // monthlyPaymentValueLabel
            // 
            this.monthlyPaymentValueLabel.AutoSize = true;
            this.monthlyPaymentValueLabel.Location = new System.Drawing.Point(130, 6363);
            this.monthlyPaymentValueLabel.Name = "monthlyPaymentValueLabel";
            this.monthlyPaymentValueLabel.Size = new System.Drawing.Size(137, 13);
            this.monthlyPaymentValueLabel.TabIndex = 402;
            this.monthlyPaymentValueLabel.Text = "monthlyPaymentValueLabel";
            // 
            // numberOfMonthlyPaymentsValueLabel
            // 
            this.numberOfMonthlyPaymentsValueLabel.AutoSize = true;
            this.numberOfMonthlyPaymentsValueLabel.Location = new System.Drawing.Point(194, 6341);
            this.numberOfMonthlyPaymentsValueLabel.Name = "numberOfMonthlyPaymentsValueLabel";
            this.numberOfMonthlyPaymentsValueLabel.Size = new System.Drawing.Size(189, 13);
            this.numberOfMonthlyPaymentsValueLabel.TabIndex = 400;
            this.numberOfMonthlyPaymentsValueLabel.Text = "numberOfMonthlyPaymentsValueLabel";
            // 
            // totalPaymentsCollectedForCurrentAccountValueLabel
            // 
            this.totalPaymentsCollectedForCurrentAccountValueLabel.AutoSize = true;
            this.totalPaymentsCollectedForCurrentAccountValueLabel.Location = new System.Drawing.Point(273, 6319);
            this.totalPaymentsCollectedForCurrentAccountValueLabel.Name = "totalPaymentsCollectedForCurrentAccountValueLabel";
            this.totalPaymentsCollectedForCurrentAccountValueLabel.Size = new System.Drawing.Size(259, 13);
            this.totalPaymentsCollectedForCurrentAccountValueLabel.TabIndex = 398;
            this.totalPaymentsCollectedForCurrentAccountValueLabel.Text = "totalPaymentsCollectedForCurrentAccountValueLabel";
            // 
            // totalCurrentAmountDueValueLabel
            // 
            this.totalCurrentAmountDueValueLabel.AutoSize = true;
            this.totalCurrentAmountDueValueLabel.Location = new System.Drawing.Point(162, 6297);
            this.totalCurrentAmountDueValueLabel.Name = "totalCurrentAmountDueValueLabel";
            this.totalCurrentAmountDueValueLabel.Size = new System.Drawing.Size(170, 13);
            this.totalCurrentAmountDueValueLabel.TabIndex = 396;
            this.totalCurrentAmountDueValueLabel.Text = "totalCurrentAmountDueValueLabel";
            // 
            // estimateForCurrentAmountDueUninsuredValueLabel
            // 
            this.estimateForCurrentAmountDueUninsuredValueLabel.AutoSize = true;
            this.estimateForCurrentAmountDueUninsuredValueLabel.Location = new System.Drawing.Point(268, 6237);
            this.estimateForCurrentAmountDueUninsuredValueLabel.Name = "estimateForCurrentAmountDueUninsuredValueLabel";
            this.estimateForCurrentAmountDueUninsuredValueLabel.Size = new System.Drawing.Size(0, 13);
            this.estimateForCurrentAmountDueUninsuredValueLabel.TabIndex = 393;
            // 
            // patientHasNoLiabilityUninsuredValueLabel
            // 
            this.patientHasNoLiabilityUninsuredValueLabel.AutoSize = true;
            this.patientHasNoLiabilityUninsuredValueLabel.Location = new System.Drawing.Point(213, 6211);
            this.patientHasNoLiabilityUninsuredValueLabel.Name = "patientHasNoLiabilityUninsuredValueLabel";
            this.patientHasNoLiabilityUninsuredValueLabel.Size = new System.Drawing.Size(0, 13);
            this.patientHasNoLiabilityUninsuredValueLabel.TabIndex = 391;
            // 
            // coPayCoInsInsuredValueLabel
            // 
            this.coPayCoInsInsuredValueLabel.AutoSize = true;
            this.coPayCoInsInsuredValueLabel.Location = new System.Drawing.Point(158, 6185);
            this.coPayCoInsInsuredValueLabel.Name = "coPayCoInsInsuredValueLabel";
            this.coPayCoInsInsuredValueLabel.Size = new System.Drawing.Size(0, 13);
            this.coPayCoInsInsuredValueLabel.TabIndex = 389;
            // 
            // deductibleInsuredValueLabel
            // 
            this.deductibleInsuredValueLabel.AutoSize = true;
            this.deductibleInsuredValueLabel.Location = new System.Drawing.Point(151, 6159);
            this.deductibleInsuredValueLabel.Name = "deductibleInsuredValueLabel";
            this.deductibleInsuredValueLabel.Size = new System.Drawing.Size(0, 13);
            this.deductibleInsuredValueLabel.TabIndex = 387;
            // 
            // patientHasNoLiabilityInsuredValueLabel
            // 
            this.patientHasNoLiabilityInsuredValueLabel.AutoSize = true;
            this.patientHasNoLiabilityInsuredValueLabel.Location = new System.Drawing.Point(200, 6133);
            this.patientHasNoLiabilityInsuredValueLabel.Name = "patientHasNoLiabilityInsuredValueLabel";
            this.patientHasNoLiabilityInsuredValueLabel.Size = new System.Drawing.Size(0, 13);
            this.patientHasNoLiabilityInsuredValueLabel.TabIndex = 385;
            // 
            // facilityDeterminedFlagLabel
            // 
            this.facilityDeterminedFlagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.facilityDeterminedFlagLabel.Location = new System.Drawing.Point(7, 6920);
            this.facilityDeterminedFlagLabel.Name = "facilityDeterminedFlagLabel";
            this.facilityDeterminedFlagLabel.Size = new System.Drawing.Size(150, 16);
            this.facilityDeterminedFlagLabel.TabIndex = 437;
            this.facilityDeterminedFlagLabel.Text = "Facility-determined flag:";
            // 
            // cosSignedLabel
            // 
            this.cosSignedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cosSignedLabel.Location = new System.Drawing.Point(7, 6898);
            this.cosSignedLabel.Name = "cosSignedLabel";
            this.cosSignedLabel.Size = new System.Drawing.Size(71, 16);
            this.cosSignedLabel.TabIndex = 435;
            this.cosSignedLabel.Text = "COS signed:";
            // 
            // privacyOptOutPatientDirectoryReligionLabel
            // 
            this.privacyOptOutPatientDirectoryReligionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyOptOutPatientDirectoryReligionLabel.Location = new System.Drawing.Point(7, 6851);
            this.privacyOptOutPatientDirectoryReligionLabel.Name = "privacyOptOutPatientDirectoryReligionLabel";
            this.privacyOptOutPatientDirectoryReligionLabel.Size = new System.Drawing.Size(351, 16);
            this.privacyOptOutPatientDirectoryReligionLabel.TabIndex = 433;
            this.privacyOptOutPatientDirectoryReligionLabel.Text = "Privacy – Opt out (exclude) from patient directory – Religion:";
            // 
            // privacyOptOutPatientDirectoryHealthInformationLabel
            // 
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Location = new System.Drawing.Point(7, 6825);
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Name = "privacyOptOutPatientDirectoryHealthInformationLabel";
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Size = new System.Drawing.Size(404, 16);
            this.privacyOptOutPatientDirectoryHealthInformationLabel.TabIndex = 431;
            this.privacyOptOutPatientDirectoryHealthInformationLabel.Text = "Privacy – Opt out (exclude) from patient directory – Health information:";
            // 
            // privacyOptOutPatientDirectoryLocationLabel
            // 
            this.privacyOptOutPatientDirectoryLocationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyOptOutPatientDirectoryLocationLabel.Location = new System.Drawing.Point(7, 6799);
            this.privacyOptOutPatientDirectoryLocationLabel.Name = "privacyOptOutPatientDirectoryLocationLabel";
            this.privacyOptOutPatientDirectoryLocationLabel.Size = new System.Drawing.Size(343, 16);
            this.privacyOptOutPatientDirectoryLocationLabel.TabIndex = 429;
            this.privacyOptOutPatientDirectoryLocationLabel.Text = "Privacy – Opt out (exclude) from patient directory - Location:";
            // 
            // privacyOptOutPatientDirectoryNameAndAllInformationLabel
            // 
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Location = new System.Drawing.Point(7, 6773);
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Name = "privacyOptOutPatientDirectoryNameAndAllInformationLabel";
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Size = new System.Drawing.Size(439, 16);
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.TabIndex = 427;
            this.privacyOptOutPatientDirectoryNameAndAllInformationLabel.Text = "Privacy – Opt out (exclude) from patient directory – Name and all information:";
            // 
            // privacyConfidentialStatusLabel
            // 
            this.privacyConfidentialStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyConfidentialStatusLabel.Location = new System.Drawing.Point(7, 6747);
            this.privacyConfidentialStatusLabel.Name = "privacyConfidentialStatusLabel";
            this.privacyConfidentialStatusLabel.Size = new System.Drawing.Size(174, 16);
            this.privacyConfidentialStatusLabel.TabIndex = 425;
            this.privacyConfidentialStatusLabel.Text = "Privacy – Confidential status:";
            // 
            // nppVersionLabel
            // 
            this.nppVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nppVersionLabel.Location = new System.Drawing.Point(7, 6695);
            this.nppVersionLabel.Name = "nppVersionLabel";
            this.nppVersionLabel.Size = new System.Drawing.Size(93, 16);
            this.nppVersionLabel.TabIndex = 421;
            this.nppVersionLabel.Text = "NPP version:";
            // 
            // regulatoryLabel
            // 
            this.regulatoryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regulatoryLabel.Location = new System.Drawing.Point(7, 6659);
            this.regulatoryLabel.Name = "regulatoryLabel";
            this.regulatoryLabel.Size = new System.Drawing.Size(100, 20);
            this.regulatoryLabel.TabIndex = 420;
            this.regulatoryLabel.Text = "Regulatory";
            // 
            // emergencyContact2PhoneLabel
            // 
            this.emergencyContact2PhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact2PhoneLabel.Location = new System.Drawing.Point(7, 6629);
            this.emergencyContact2PhoneLabel.Name = "emergencyContact2PhoneLabel";
            this.emergencyContact2PhoneLabel.Size = new System.Drawing.Size(104, 16);
            this.emergencyContact2PhoneLabel.TabIndex = 418;
            this.emergencyContact2PhoneLabel.Text = "Contact phone:";
            // 
            // emergencyContact2AddressLabel
            // 
            this.emergencyContact2AddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact2AddressLabel.Location = new System.Drawing.Point(7, 6603);
            this.emergencyContact2AddressLabel.Name = "emergencyContact2AddressLabel";
            this.emergencyContact2AddressLabel.Size = new System.Drawing.Size(51, 16);
            this.emergencyContact2AddressLabel.TabIndex = 416;
            this.emergencyContact2AddressLabel.Text = "Address:";
            // 
            // emergencyContact2IsThePatientsLabel
            // 
            this.emergencyContact2IsThePatientsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact2IsThePatientsLabel.Location = new System.Drawing.Point(7, 6577);
            this.emergencyContact2IsThePatientsLabel.Name = "emergencyContact2IsThePatientsLabel";
            this.emergencyContact2IsThePatientsLabel.Size = new System.Drawing.Size(177, 16);
            this.emergencyContact2IsThePatientsLabel.TabIndex = 414;
            this.emergencyContact2IsThePatientsLabel.Text = "The Contact is the Patient’s:";
            // 
            // emergencyContact2NameLabel
            // 
            this.emergencyContact2NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact2NameLabel.Location = new System.Drawing.Point(7, 6551);
            this.emergencyContact2NameLabel.Name = "emergencyContact2NameLabel";
            this.emergencyContact2NameLabel.Size = new System.Drawing.Size(168, 16);
            this.emergencyContact2NameLabel.TabIndex = 412;
            this.emergencyContact2NameLabel.Text = "Emergency Contact 2 name:";
            // 
            // emergencyContact1PhoneLabel
            // 
            this.emergencyContact1PhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact1PhoneLabel.Location = new System.Drawing.Point(7, 6525);
            this.emergencyContact1PhoneLabel.Name = "emergencyContact1PhoneLabel";
            this.emergencyContact1PhoneLabel.Size = new System.Drawing.Size(99, 16);
            this.emergencyContact1PhoneLabel.TabIndex = 410;
            this.emergencyContact1PhoneLabel.Text = "Contact phone:";
            // 
            // emergencyContact1AddressLabel
            // 
            this.emergencyContact1AddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact1AddressLabel.Location = new System.Drawing.Point(7, 6499);
            this.emergencyContact1AddressLabel.Name = "emergencyContact1AddressLabel";
            this.emergencyContact1AddressLabel.Size = new System.Drawing.Size(51, 16);
            this.emergencyContact1AddressLabel.TabIndex = 408;
            this.emergencyContact1AddressLabel.Text = "Address:";
            // 
            // emergencyContact1IsThePatientsLabel
            // 
            this.emergencyContact1IsThePatientsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact1IsThePatientsLabel.Location = new System.Drawing.Point(7, 6473);
            this.emergencyContact1IsThePatientsLabel.Name = "emergencyContact1IsThePatientsLabel";
            this.emergencyContact1IsThePatientsLabel.Size = new System.Drawing.Size(172, 16);
            this.emergencyContact1IsThePatientsLabel.TabIndex = 406;
            this.emergencyContact1IsThePatientsLabel.Text = "The Contact is the Patient’s:";
            // 
            // emergencyContact1NameLabel
            // 
            this.emergencyContact1NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emergencyContact1NameLabel.Location = new System.Drawing.Point(7, 6447);
            this.emergencyContact1NameLabel.Name = "emergencyContact1NameLabel";
            this.emergencyContact1NameLabel.Size = new System.Drawing.Size(168, 16);
            this.emergencyContact1NameLabel.TabIndex = 404;
            this.emergencyContact1NameLabel.Text = "Emergency Contact 1 name:";
            // 
            // contactsLabel
            // 
            this.contactsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contactsLabel.Location = new System.Drawing.Point(7, 6411);
            this.contactsLabel.Name = "contactsLabel";
            this.contactsLabel.Size = new System.Drawing.Size(83, 20);
            this.contactsLabel.TabIndex = 403;
            this.contactsLabel.Text = "Contacts";
            // 
            // monthlyPaymentLabel
            // 
            this.monthlyPaymentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthlyPaymentLabel.Location = new System.Drawing.Point(7, 6363);
            this.monthlyPaymentLabel.Name = "monthlyPaymentLabel";
            this.monthlyPaymentLabel.Size = new System.Drawing.Size(117, 16);
            this.monthlyPaymentLabel.TabIndex = 401;
            this.monthlyPaymentLabel.Text = "Monthly payment:";
            // 
            // numberOfMonthlyPaymentsLabel
            // 
            this.numberOfMonthlyPaymentsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberOfMonthlyPaymentsLabel.Location = new System.Drawing.Point(7, 6341);
            this.numberOfMonthlyPaymentsLabel.Name = "numberOfMonthlyPaymentsLabel";
            this.numberOfMonthlyPaymentsLabel.Size = new System.Drawing.Size(181, 16);
            this.numberOfMonthlyPaymentsLabel.TabIndex = 399;
            this.numberOfMonthlyPaymentsLabel.Text = "Number of monthly payments:";
            // 
            // totalPaymentsCollectedForCurrentAccountLabel
            // 
            this.totalPaymentsCollectedForCurrentAccountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalPaymentsCollectedForCurrentAccountLabel.Location = new System.Drawing.Point(7, 6319);
            this.totalPaymentsCollectedForCurrentAccountLabel.Name = "totalPaymentsCollectedForCurrentAccountLabel";
            this.totalPaymentsCollectedForCurrentAccountLabel.Size = new System.Drawing.Size(260, 16);
            this.totalPaymentsCollectedForCurrentAccountLabel.TabIndex = 397;
            this.totalPaymentsCollectedForCurrentAccountLabel.Text = "Total payments collected for current account:";
            // 
            // totalCurrentAmountDueLabel
            // 
            this.totalCurrentAmountDueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalCurrentAmountDueLabel.Location = new System.Drawing.Point(7, 6297);
            this.totalCurrentAmountDueLabel.Name = "totalCurrentAmountDueLabel";
            this.totalCurrentAmountDueLabel.Size = new System.Drawing.Size(149, 16);
            this.totalCurrentAmountDueLabel.TabIndex = 395;
            this.totalCurrentAmountDueLabel.Text = "Total current amount due:";
            // 
            // paymentLabel
            // 
            this.paymentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.paymentLabel.Location = new System.Drawing.Point(7, 6267);
            this.paymentLabel.Name = "paymentLabel";
            this.paymentLabel.Size = new System.Drawing.Size(83, 20);
            this.paymentLabel.TabIndex = 394;
            this.paymentLabel.Text = "Payment";
            // 
            // estimateForCurrentAmountDueUninsuredLabel
            // 
            this.estimateForCurrentAmountDueUninsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.estimateForCurrentAmountDueUninsuredLabel.Location = new System.Drawing.Point(7, 6237);
            this.estimateForCurrentAmountDueUninsuredLabel.Name = "estimateForCurrentAmountDueUninsuredLabel";
            this.estimateForCurrentAmountDueUninsuredLabel.Size = new System.Drawing.Size(255, 16);
            this.estimateForCurrentAmountDueUninsuredLabel.TabIndex = 392;
            this.estimateForCurrentAmountDueUninsuredLabel.Text = "Estimate for current amount due (uninsured):";
            // 
            // patientHasNoLiabilityUninsuredLabel
            // 
            this.patientHasNoLiabilityUninsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientHasNoLiabilityUninsuredLabel.Location = new System.Drawing.Point(7, 6211);
            this.patientHasNoLiabilityUninsuredLabel.Name = "patientHasNoLiabilityUninsuredLabel";
            this.patientHasNoLiabilityUninsuredLabel.Size = new System.Drawing.Size(200, 16);
            this.patientHasNoLiabilityUninsuredLabel.TabIndex = 390;
            this.patientHasNoLiabilityUninsuredLabel.Text = "Patient has no liability (uninsured):";
            // 
            // coPayCoInsInsuredLabel
            // 
            this.coPayCoInsInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.coPayCoInsInsuredLabel.Location = new System.Drawing.Point(7, 6185);
            this.coPayCoInsInsuredLabel.Name = "coPayCoInsInsuredLabel";
            this.coPayCoInsInsuredLabel.Size = new System.Drawing.Size(145, 16);
            this.coPayCoInsInsuredLabel.TabIndex = 388;
            this.coPayCoInsInsuredLabel.Text = "Co-pay/co-ins (insured):";
            // 
            // deductibleInsuredLabel
            // 
            this.deductibleInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deductibleInsuredLabel.Location = new System.Drawing.Point(7, 6159);
            this.deductibleInsuredLabel.Name = "deductibleInsuredLabel";
            this.deductibleInsuredLabel.Size = new System.Drawing.Size(138, 16);
            this.deductibleInsuredLabel.TabIndex = 386;
            this.deductibleInsuredLabel.Text = "Deductible (insured):";
            // 
            // patientHasNoLiabilityInsuredLabel
            // 
            this.patientHasNoLiabilityInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientHasNoLiabilityInsuredLabel.Location = new System.Drawing.Point(7, 6133);
            this.patientHasNoLiabilityInsuredLabel.Name = "patientHasNoLiabilityInsuredLabel";
            this.patientHasNoLiabilityInsuredLabel.Size = new System.Drawing.Size(187, 16);
            this.patientHasNoLiabilityInsuredLabel.TabIndex = 384;
            this.patientHasNoLiabilityInsuredLabel.Text = "Patient has no liability (insured):";
            // 
            // liabilityLabel
            // 
            this.liabilityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.liabilityLabel.Location = new System.Drawing.Point(7, 6095);
            this.liabilityLabel.Name = "liabilityLabel";
            this.liabilityLabel.Size = new System.Drawing.Size(68, 20);
            this.liabilityLabel.TabIndex = 383;
            this.liabilityLabel.Text = "Liability";
            // 
            // spanCode2TodateValueLabel
            // 
            this.spanCode2TodateValueLabel.AutoSize = true;
            this.spanCode2TodateValueLabel.Location = new System.Drawing.Point(530, 6067);
            this.spanCode2TodateValueLabel.Name = "spanCode2TodateValueLabel";
            this.spanCode2TodateValueLabel.Size = new System.Drawing.Size(0, 13);
            this.spanCode2TodateValueLabel.TabIndex = 382;
            // 
            // spanCode2FromdateValueLabel
            // 
            this.spanCode2FromdateValueLabel.AutoSize = true;
            this.spanCode2FromdateValueLabel.Location = new System.Drawing.Point(408, 6067);
            this.spanCode2FromdateValueLabel.Name = "spanCode2FromdateValueLabel";
            this.spanCode2FromdateValueLabel.Size = new System.Drawing.Size(0, 13);
            this.spanCode2FromdateValueLabel.TabIndex = 380;
            // 
            // spanCode1FacilityValueLabel
            // 
            this.spanCode1FacilityValueLabel.AutoSize = true;
            this.spanCode1FacilityValueLabel.Location = new System.Drawing.Point(138, 6041);
            this.spanCode1FacilityValueLabel.Name = "spanCode1FacilityValueLabel";
            this.spanCode1FacilityValueLabel.Size = new System.Drawing.Size(0, 13);
            this.spanCode1FacilityValueLabel.TabIndex = 376;
            // 
            // spanCode1TodateValueLabel
            // 
            this.spanCode1TodateValueLabel.AutoSize = true;
            this.spanCode1TodateValueLabel.Location = new System.Drawing.Point(530, 6015);
            this.spanCode1TodateValueLabel.Name = "spanCode1TodateValueLabel";
            this.spanCode1TodateValueLabel.Size = new System.Drawing.Size(0, 13);
            this.spanCode1TodateValueLabel.TabIndex = 374;
            // 
            // spanCode1FromdateValueLabel
            // 
            this.spanCode1FromdateValueLabel.AutoSize = true;
            this.spanCode1FromdateValueLabel.Location = new System.Drawing.Point(408, 6015);
            this.spanCode1FromdateValueLabel.Name = "spanCode1FromdateValueLabel";
            this.spanCode1FromdateValueLabel.Size = new System.Drawing.Size(0, 13);
            this.spanCode1FromdateValueLabel.TabIndex = 372;
            // 
            // spanCode2TodateLabel
            // 
            this.spanCode2TodateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spanCode2TodateLabel.Location = new System.Drawing.Point(475, 6067);
            this.spanCode2TodateLabel.Name = "spanCode2TodateLabel";
            this.spanCode2TodateLabel.Size = new System.Drawing.Size(55, 16);
            this.spanCode2TodateLabel.TabIndex = 381;
            this.spanCode2TodateLabel.Text = "To date:";
            // 
            // spanCode2FromdateLabel
            // 
            this.spanCode2FromdateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spanCode2FromdateLabel.Location = new System.Drawing.Point(340, 6067);
            this.spanCode2FromdateLabel.Name = "spanCode2FromdateLabel";
            this.spanCode2FromdateLabel.Size = new System.Drawing.Size(68, 16);
            this.spanCode2FromdateLabel.TabIndex = 379;
            this.spanCode2FromdateLabel.Text = "From date:";
            // 
            // spanCode2Label
            // 
            this.spanCode2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spanCode2Label.Location = new System.Drawing.Point(7, 6067);
            this.spanCode2Label.Name = "spanCode2Label";
            this.spanCode2Label.Size = new System.Drawing.Size(89, 16);
            this.spanCode2Label.TabIndex = 377;
            this.spanCode2Label.Text = "Span code 2:";
            // 
            // spanCode1FacilityLabel
            // 
            this.spanCode1FacilityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spanCode1FacilityLabel.Location = new System.Drawing.Point(7, 6041);
            this.spanCode1FacilityLabel.Name = "spanCode1FacilityLabel";
            this.spanCode1FacilityLabel.Size = new System.Drawing.Size(125, 16);
            this.spanCode1FacilityLabel.TabIndex = 375;
            this.spanCode1FacilityLabel.Text = "Span code 1 facility:";
            // 
            // spanCode1TodateLabel
            // 
            this.spanCode1TodateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spanCode1TodateLabel.Location = new System.Drawing.Point(475, 6015);
            this.spanCode1TodateLabel.Name = "spanCode1TodateLabel";
            this.spanCode1TodateLabel.Size = new System.Drawing.Size(55, 16);
            this.spanCode1TodateLabel.TabIndex = 373;
            this.spanCode1TodateLabel.Text = "To date:";
            // 
            // spanCode1FromdateLabel
            // 
            this.spanCode1FromdateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spanCode1FromdateLabel.Location = new System.Drawing.Point(340, 6015);
            this.spanCode1FromdateLabel.Name = "spanCode1FromdateLabel";
            this.spanCode1FromdateLabel.Size = new System.Drawing.Size(68, 16);
            this.spanCode1FromdateLabel.TabIndex = 371;
            this.spanCode1FromdateLabel.Text = "From date:";
            // 
            // spanCode1Label
            // 
            this.spanCode1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spanCode1Label.Location = new System.Drawing.Point(7, 6015);
            this.spanCode1Label.Name = "spanCode1Label";
            this.spanCode1Label.Size = new System.Drawing.Size(83, 16);
            this.spanCode1Label.TabIndex = 369;
            this.spanCode1Label.Text = "Span code 1:";
            // 
            // conditionCode7Label
            // 
            this.conditionCode7Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode7Label.Location = new System.Drawing.Point(7, 5989);
            this.conditionCode7Label.Name = "conditionCode7Label";
            this.conditionCode7Label.Size = new System.Drawing.Size(106, 16);
            this.conditionCode7Label.TabIndex = 368;
            this.conditionCode7Label.Text = "Condition code 7:";
            // 
            // conditionCode6Label
            // 
            this.conditionCode6Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode6Label.Location = new System.Drawing.Point(7, 5963);
            this.conditionCode6Label.Name = "conditionCode6Label";
            this.conditionCode6Label.Size = new System.Drawing.Size(106, 16);
            this.conditionCode6Label.TabIndex = 367;
            this.conditionCode6Label.Text = "Condition code 6:";
            // 
            // conditionCode5Label
            // 
            this.conditionCode5Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode5Label.Location = new System.Drawing.Point(7, 5937);
            this.conditionCode5Label.Name = "conditionCode5Label";
            this.conditionCode5Label.Size = new System.Drawing.Size(106, 16);
            this.conditionCode5Label.TabIndex = 366;
            this.conditionCode5Label.Text = "Condition code 5:";
            // 
            // conditionCode4Label
            // 
            this.conditionCode4Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode4Label.Location = new System.Drawing.Point(7, 5911);
            this.conditionCode4Label.Name = "conditionCode4Label";
            this.conditionCode4Label.Size = new System.Drawing.Size(106, 16);
            this.conditionCode4Label.TabIndex = 365;
            this.conditionCode4Label.Text = "Condition code 4:";
            // 
            // conditionCode3Label
            // 
            this.conditionCode3Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode3Label.Location = new System.Drawing.Point(7, 5885);
            this.conditionCode3Label.Name = "conditionCode3Label";
            this.conditionCode3Label.Size = new System.Drawing.Size(106, 16);
            this.conditionCode3Label.TabIndex = 364;
            this.conditionCode3Label.Text = "Condition code 3:";
            // 
            // conditionCode2Label
            // 
            this.conditionCode2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode2Label.Location = new System.Drawing.Point(7, 5859);
            this.conditionCode2Label.Name = "conditionCode2Label";
            this.conditionCode2Label.Size = new System.Drawing.Size(106, 16);
            this.conditionCode2Label.TabIndex = 363;
            this.conditionCode2Label.Text = "Condition code 2:";
            // 
            // conditionCode1Label
            // 
            this.conditionCode1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionCode1Label.Location = new System.Drawing.Point(7, 5833);
            this.conditionCode1Label.Name = "conditionCode1Label";
            this.conditionCode1Label.Size = new System.Drawing.Size(106, 16);
            this.conditionCode1Label.TabIndex = 362;
            this.conditionCode1Label.Text = "Condition code 1:";
            // 
            // occurrenceCode8Label
            // 
            this.occurrenceCode8Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode8Label.Location = new System.Drawing.Point(7, 5807);
            this.occurrenceCode8Label.Name = "occurrenceCode8Label";
            this.occurrenceCode8Label.Size = new System.Drawing.Size(117, 16);
            this.occurrenceCode8Label.TabIndex = 360;
            this.occurrenceCode8Label.Text = "Occurrence code 8:";
            // 
            // occurrenceCode7Label
            // 
            this.occurrenceCode7Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode7Label.Location = new System.Drawing.Point(7, 5781);
            this.occurrenceCode7Label.Name = "occurrenceCode7Label";
            this.occurrenceCode7Label.Size = new System.Drawing.Size(117, 16);
            this.occurrenceCode7Label.TabIndex = 358;
            this.occurrenceCode7Label.Text = "Occurrence code 7:";
            // 
            // occurrenceCode6Label
            // 
            this.occurrenceCode6Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode6Label.Location = new System.Drawing.Point(7, 5755);
            this.occurrenceCode6Label.Name = "occurrenceCode6Label";
            this.occurrenceCode6Label.Size = new System.Drawing.Size(117, 16);
            this.occurrenceCode6Label.TabIndex = 356;
            this.occurrenceCode6Label.Text = "Occurrence code 6:";
            // 
            // occurrenceCode5Label
            // 
            this.occurrenceCode5Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode5Label.Location = new System.Drawing.Point(7, 5729);
            this.occurrenceCode5Label.Name = "occurrenceCode5Label";
            this.occurrenceCode5Label.Size = new System.Drawing.Size(117, 16);
            this.occurrenceCode5Label.TabIndex = 354;
            this.occurrenceCode5Label.Text = "Occurrence code 5:";
            // 
            // occurrenceCode4Label
            // 
            this.occurrenceCode4Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode4Label.Location = new System.Drawing.Point(7, 5703);
            this.occurrenceCode4Label.Name = "occurrenceCode4Label";
            this.occurrenceCode4Label.Size = new System.Drawing.Size(117, 16);
            this.occurrenceCode4Label.TabIndex = 352;
            this.occurrenceCode4Label.Text = "Occurrence code 4:";
            // 
            // occurrenceCode3Label
            // 
            this.occurrenceCode3Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode3Label.Location = new System.Drawing.Point(7, 5677);
            this.occurrenceCode3Label.Name = "occurrenceCode3Label";
            this.occurrenceCode3Label.Size = new System.Drawing.Size(117, 16);
            this.occurrenceCode3Label.TabIndex = 350;
            this.occurrenceCode3Label.Text = "Occurrence code 3:";
            // 
            // occurrenceCode2Label
            // 
            this.occurrenceCode2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode2Label.Location = new System.Drawing.Point(7, 5651);
            this.occurrenceCode2Label.Name = "occurrenceCode2Label";
            this.occurrenceCode2Label.Size = new System.Drawing.Size(117, 16);
            this.occurrenceCode2Label.TabIndex = 348;
            this.occurrenceCode2Label.Text = "Occurrence code 2:";
            // 
            // occurrenceCode1Label
            // 
            this.occurrenceCode1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occurrenceCode1Label.Location = new System.Drawing.Point(7, 5625);
            this.occurrenceCode1Label.Name = "occurrenceCode1Label";
            this.occurrenceCode1Label.Size = new System.Drawing.Size(117, 16);
            this.occurrenceCode1Label.TabIndex = 346;
            this.occurrenceCode1Label.Text = "Occurrence code 1:";
            // 
            // guarantorOccIndustryValueLabel
            // 
            this.guarantorOccIndustryValueLabel.AutoSize = true;
            this.guarantorOccIndustryValueLabel.Location = new System.Drawing.Point(97, 5562);
            this.guarantorOccIndustryValueLabel.Name = "guarantorOccIndustryValueLabel";
            this.guarantorOccIndustryValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorOccIndustryValueLabel.TabIndex = 344;
            // 
            // guarantorEmployerPhoneValueLabel
            // 
            this.guarantorEmployerPhoneValueLabel.AutoSize = true;
            this.guarantorEmployerPhoneValueLabel.Location = new System.Drawing.Point(115, 5541);
            this.guarantorEmployerPhoneValueLabel.Name = "guarantorEmployerPhoneValueLabel";
            this.guarantorEmployerPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorEmployerPhoneValueLabel.TabIndex = 342;
            // 
            // guarantorEmployerValueLabel
            // 
            this.guarantorEmployerValueLabel.AutoSize = true;
            this.guarantorEmployerValueLabel.Location = new System.Drawing.Point(74, 5514);
            this.guarantorEmployerValueLabel.Name = "guarantorEmployerValueLabel";
            this.guarantorEmployerValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorEmployerValueLabel.TabIndex = 340;
            // 
            // guarantorEmploymentStatusValueLabel
            // 
            this.guarantorEmploymentStatusValueLabel.AutoSize = true;
            this.guarantorEmploymentStatusValueLabel.Location = new System.Drawing.Point(131, 5489);
            this.guarantorEmploymentStatusValueLabel.Name = "guarantorEmploymentStatusValueLabel";
            this.guarantorEmploymentStatusValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorEmploymentStatusValueLabel.TabIndex = 338;
            // 
            // guarantorContactEmailValueLabel
            // 
            this.guarantorContactEmailValueLabel.AutoSize = true;
            this.guarantorContactEmailValueLabel.Location = new System.Drawing.Point(108, 5465);
            this.guarantorContactEmailValueLabel.Name = "guarantorContactEmailValueLabel";
            this.guarantorContactEmailValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorContactEmailValueLabel.TabIndex = 336;
            // 
            // guarantorContactCellValueLabel
            // 
            this.guarantorContactCellValueLabel.AutoSize = true;
            this.guarantorContactCellValueLabel.Location = new System.Drawing.Point(97, 5420);
            this.guarantorContactCellValueLabel.Name = "guarantorContactCellValueLabel";
            this.guarantorContactCellValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorContactCellValueLabel.TabIndex = 334;
            // 
            // guarantorContactPhoneValueLabel
            // 
            this.guarantorContactPhoneValueLabel.AutoSize = true;
            this.guarantorContactPhoneValueLabel.Location = new System.Drawing.Point(109, 5398);
            this.guarantorContactPhoneValueLabel.Name = "guarantorContactPhoneValueLabel";
            this.guarantorContactPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorContactPhoneValueLabel.TabIndex = 332;
            // 
            // guarantorAddressValueLabel
            // 
            this.guarantorAddressValueLabel.AutoSize = true;
            this.guarantorAddressValueLabel.Location = new System.Drawing.Point(64, 5377);
            this.guarantorAddressValueLabel.Name = "guarantorAddressValueLabel";
            this.guarantorAddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorAddressValueLabel.TabIndex = 330;
            // 
            // usDriverLicenseValueLabel
            // 
            this.usDriverLicenseValueLabel.AutoSize = true;
            this.usDriverLicenseValueLabel.Location = new System.Drawing.Point(135, 5357);
            this.usDriverLicenseValueLabel.Name = "usDriverLicenseValueLabel";
            this.usDriverLicenseValueLabel.Size = new System.Drawing.Size(0, 13);
            this.usDriverLicenseValueLabel.TabIndex = 328;
            // 
            // guarantorSSNValueLabel
            // 
            this.guarantorSSNValueLabel.AutoSize = true;
            this.guarantorSSNValueLabel.Location = new System.Drawing.Point(44, 5331);
            this.guarantorSSNValueLabel.Name = "guarantorSSNValueLabel";
            this.guarantorSSNValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorSSNValueLabel.TabIndex = 326;
            // 
            // guarantorGenderValueLabel
            // 
            this.guarantorGenderValueLabel.AutoSize = true;
            this.guarantorGenderValueLabel.Location = new System.Drawing.Point(60, 5291);
            this.guarantorGenderValueLabel.Name = "guarantorGenderValueLabel";
            this.guarantorGenderValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorGenderValueLabel.TabIndex = 324;
            // 
            // thePatientIsTheGuarantorsValueLabel
            // 
            this.thePatientIsTheGuarantorsValueLabel.AutoSize = true;
            this.thePatientIsTheGuarantorsValueLabel.Location = new System.Drawing.Point(185, 5267);
            this.thePatientIsTheGuarantorsValueLabel.Name = "thePatientIsTheGuarantorsValueLabel";
            this.thePatientIsTheGuarantorsValueLabel.Size = new System.Drawing.Size(0, 13);
            this.thePatientIsTheGuarantorsValueLabel.TabIndex = 322;
            // 
            // guarantorNameValueLabel
            // 
            this.guarantorNameValueLabel.AutoSize = true;
            this.guarantorNameValueLabel.Location = new System.Drawing.Point(52, 5243);
            this.guarantorNameValueLabel.Name = "guarantorNameValueLabel";
            this.guarantorNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.guarantorNameValueLabel.TabIndex = 320;
            // 
            // billingLabel
            // 
            this.billingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.billingLabel.Location = new System.Drawing.Point(7, 5589);
            this.billingLabel.Name = "billingLabel";
            this.billingLabel.Size = new System.Drawing.Size(56, 20);
            this.billingLabel.TabIndex = 345;
            this.billingLabel.Text = "Billing";
            // 
            // guarantorOccIndustryLabel
            // 
            this.guarantorOccIndustryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorOccIndustryLabel.Location = new System.Drawing.Point(7, 5562);
            this.guarantorOccIndustryLabel.Name = "guarantorOccIndustryLabel";
            this.guarantorOccIndustryLabel.Size = new System.Drawing.Size(84, 16);
            this.guarantorOccIndustryLabel.TabIndex = 343;
            this.guarantorOccIndustryLabel.Text = "Occ/Industry:";
            // 
            // guarantorEmployerPhoneLabel
            // 
            this.guarantorEmployerPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorEmployerPhoneLabel.Location = new System.Drawing.Point(7, 5541);
            this.guarantorEmployerPhoneLabel.Name = "guarantorEmployerPhoneLabel";
            this.guarantorEmployerPhoneLabel.Size = new System.Drawing.Size(105, 16);
            this.guarantorEmployerPhoneLabel.TabIndex = 341;
            this.guarantorEmployerPhoneLabel.Text = "Employer phone:";
            // 
            // guarantorEmployerLabel
            // 
            this.guarantorEmployerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorEmployerLabel.Location = new System.Drawing.Point(7, 5514);
            this.guarantorEmployerLabel.Name = "guarantorEmployerLabel";
            this.guarantorEmployerLabel.Size = new System.Drawing.Size(57, 16);
            this.guarantorEmployerLabel.TabIndex = 339;
            this.guarantorEmployerLabel.Text = "Employer:";
            // 
            // guarantorEmploymentStatusLabel
            // 
            this.guarantorEmploymentStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorEmploymentStatusLabel.Location = new System.Drawing.Point(7, 5489);
            this.guarantorEmploymentStatusLabel.Name = "guarantorEmploymentStatusLabel";
            this.guarantorEmploymentStatusLabel.Size = new System.Drawing.Size(118, 16);
            this.guarantorEmploymentStatusLabel.TabIndex = 337;
            this.guarantorEmploymentStatusLabel.Text = "Employment status:";
            // 
            // guarantorContactEmailLabel
            // 
            this.guarantorContactEmailLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorContactEmailLabel.Location = new System.Drawing.Point(7, 5465);
            this.guarantorContactEmailLabel.Name = "guarantorContactEmailLabel";
            this.guarantorContactEmailLabel.Size = new System.Drawing.Size(95, 16);
            this.guarantorContactEmailLabel.TabIndex = 335;
            this.guarantorContactEmailLabel.Text = "Contact email:";
            // 
            // guarantorContactCellLabel
            // 
            this.guarantorContactCellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorContactCellLabel.Location = new System.Drawing.Point(7, 5420);
            this.guarantorContactCellLabel.Name = "guarantorContactCellLabel";
            this.guarantorContactCellLabel.Size = new System.Drawing.Size(87, 16);
            this.guarantorContactCellLabel.TabIndex = 333;
            this.guarantorContactCellLabel.Text = "Contact Cell:";
            // 
            // guarantorContactPhoneLabel
            // 
            this.guarantorContactPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorContactPhoneLabel.Location = new System.Drawing.Point(7, 5398);
            this.guarantorContactPhoneLabel.Name = "guarantorContactPhoneLabel";
            this.guarantorContactPhoneLabel.Size = new System.Drawing.Size(100, 16);
            this.guarantorContactPhoneLabel.TabIndex = 331;
            this.guarantorContactPhoneLabel.Text = "Contact Phone:";
            // 
            // guarantorAddressLabel
            // 
            this.guarantorAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorAddressLabel.Location = new System.Drawing.Point(7, 5377);
            this.guarantorAddressLabel.Name = "guarantorAddressLabel";
            this.guarantorAddressLabel.Size = new System.Drawing.Size(51, 16);
            this.guarantorAddressLabel.TabIndex = 329;
            this.guarantorAddressLabel.Text = "Address:";
            // 
            // usDriverLicenseLabel
            // 
            this.usDriverLicenseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usDriverLicenseLabel.Location = new System.Drawing.Point(7, 5357);
            this.usDriverLicenseLabel.Name = "usDriverLicenseLabel";
            this.usDriverLicenseLabel.Size = new System.Drawing.Size(122, 16);
            this.usDriverLicenseLabel.TabIndex = 327;
            this.usDriverLicenseLabel.Text = "U.S. driver\'s license:";
            // 
            // guarantorSSNLabel
            // 
            this.guarantorSSNLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorSSNLabel.Location = new System.Drawing.Point(7, 5331);
            this.guarantorSSNLabel.Name = "guarantorSSNLabel";
            this.guarantorSSNLabel.Size = new System.Drawing.Size(31, 16);
            this.guarantorSSNLabel.TabIndex = 325;
            this.guarantorSSNLabel.Text = "SSN:";
            // 
            // guarantorGenderLabel
            // 
            this.guarantorGenderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorGenderLabel.Location = new System.Drawing.Point(7, 5291);
            this.guarantorGenderLabel.Name = "guarantorGenderLabel";
            this.guarantorGenderLabel.Size = new System.Drawing.Size(47, 16);
            this.guarantorGenderLabel.TabIndex = 323;
            this.guarantorGenderLabel.Text = "Gender:";
            // 
            // thePatientIsTheGuarantorsLabel
            // 
            this.thePatientIsTheGuarantorsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.thePatientIsTheGuarantorsLabel.Location = new System.Drawing.Point(7, 5267);
            this.thePatientIsTheGuarantorsLabel.Name = "thePatientIsTheGuarantorsLabel";
            this.thePatientIsTheGuarantorsLabel.Size = new System.Drawing.Size(180, 16);
            this.thePatientIsTheGuarantorsLabel.TabIndex = 321;
            this.thePatientIsTheGuarantorsLabel.Text = "The Patient is the Guarantor’s:";
            // 
            // guarantorNameLabel
            // 
            this.guarantorNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorNameLabel.Location = new System.Drawing.Point(7, 5243);
            this.guarantorNameLabel.Name = "guarantorNameLabel";
            this.guarantorNameLabel.Size = new System.Drawing.Size(39, 16);
            this.guarantorNameLabel.TabIndex = 319;
            this.guarantorNameLabel.Text = "Name:";
            // 
            // secondaryPromptExtLabel
            // 
            this.secondaryPromptExtLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryPromptExtLabel.Location = new System.Drawing.Point(7, 4749);
            this.secondaryPromptExtLabel.Name = "secondaryPromptExtLabel";
            this.secondaryPromptExtLabel.Size = new System.Drawing.Size(65, 16);
            this.secondaryPromptExtLabel.TabIndex = 304;
            this.secondaryPromptExtLabel.Text = "Prompt/ext:";
            // 
            // secondaryBillingAddressValueLabel
            // 
            this.secondaryBillingAddressValueLabel.AutoSize = true;
            this.secondaryBillingAddressValueLabel.Location = new System.Drawing.Point(105, 4201);
            this.secondaryBillingAddressValueLabel.Name = "secondaryBillingAddressValueLabel";
            this.secondaryBillingAddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryBillingAddressValueLabel.TabIndex = 267;
            // 
            // secondaryBillingAddressLabel
            // 
            this.secondaryBillingAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBillingAddressLabel.Location = new System.Drawing.Point(7, 4201);
            this.secondaryBillingAddressLabel.Name = "secondaryBillingAddressLabel";
            this.secondaryBillingAddressLabel.Size = new System.Drawing.Size(92, 16);
            this.secondaryBillingAddressLabel.TabIndex = 266;
            this.secondaryBillingAddressLabel.Text = "Billing address:";
            // 
            // guarantorLabel
            // 
            this.guarantorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guarantorLabel.Location = new System.Drawing.Point(7, 5209);
            this.guarantorLabel.Name = "guarantorLabel";
            this.guarantorLabel.Size = new System.Drawing.Size(88, 20);
            this.guarantorLabel.TabIndex = 318;
            this.guarantorLabel.Text = "Guarantor";
            // 
            // secondaryAuthorizationPhoneValueLabel
            // 
            this.secondaryAuthorizationPhoneValueLabel.AutoSize = true;
            this.secondaryAuthorizationPhoneValueLabel.Location = new System.Drawing.Point(132, 4723);
            this.secondaryAuthorizationPhoneValueLabel.Name = "secondaryAuthorizationPhoneValueLabel";
            this.secondaryAuthorizationPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAuthorizationPhoneValueLabel.TabIndex = 303;
            // 
            // secondaryAuthorizationCompanyValueLabel
            // 
            this.secondaryAuthorizationCompanyValueLabel.AutoSize = true;
            this.secondaryAuthorizationCompanyValueLabel.Location = new System.Drawing.Point(147, 4697);
            this.secondaryAuthorizationCompanyValueLabel.Name = "secondaryAuthorizationCompanyValueLabel";
            this.secondaryAuthorizationCompanyValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAuthorizationCompanyValueLabel.TabIndex = 301;
            // 
            // secondaryAuthorizationRequiredValueLabel
            // 
            this.secondaryAuthorizationRequiredValueLabel.AutoSize = true;
            this.secondaryAuthorizationRequiredValueLabel.Location = new System.Drawing.Point(143, 4671);
            this.secondaryAuthorizationRequiredValueLabel.Name = "secondaryAuthorizationRequiredValueLabel";
            this.secondaryAuthorizationRequiredValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAuthorizationRequiredValueLabel.TabIndex = 299;
            // 
            // secondaryDateValueLabel
            // 
            this.secondaryDateValueLabel.AutoSize = true;
            this.secondaryDateValueLabel.Location = new System.Drawing.Point(45, 4645);
            this.secondaryDateValueLabel.Name = "secondaryDateValueLabel";
            this.secondaryDateValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryDateValueLabel.TabIndex = 297;
            // 
            // secondaryInitiatedByValueLabel
            // 
            this.secondaryInitiatedByValueLabel.AutoSize = true;
            this.secondaryInitiatedByValueLabel.Location = new System.Drawing.Point(85, 4619);
            this.secondaryInitiatedByValueLabel.Name = "secondaryInitiatedByValueLabel";
            this.secondaryInitiatedByValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryInitiatedByValueLabel.TabIndex = 295;
            // 
            // secondaryBenefitsVerifiedValueLabel
            // 
            this.secondaryBenefitsVerifiedValueLabel.AutoSize = true;
            this.secondaryBenefitsVerifiedValueLabel.Location = new System.Drawing.Point(112, 4593);
            this.secondaryBenefitsVerifiedValueLabel.Name = "secondaryBenefitsVerifiedValueLabel";
            this.secondaryBenefitsVerifiedValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryBenefitsVerifiedValueLabel.TabIndex = 293;
            // 
            // secondaryAuthorizationPhoneLabel
            // 
            this.secondaryAuthorizationPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationPhoneLabel.Location = new System.Drawing.Point(7, 4723);
            this.secondaryAuthorizationPhoneLabel.Name = "secondaryAuthorizationPhoneLabel";
            this.secondaryAuthorizationPhoneLabel.Size = new System.Drawing.Size(119, 16);
            this.secondaryAuthorizationPhoneLabel.TabIndex = 302;
            this.secondaryAuthorizationPhoneLabel.Text = "Authorization phone:";
            // 
            // secondaryAuthorizationCompanyLabel
            // 
            this.secondaryAuthorizationCompanyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationCompanyLabel.Location = new System.Drawing.Point(7, 4697);
            this.secondaryAuthorizationCompanyLabel.Name = "secondaryAuthorizationCompanyLabel";
            this.secondaryAuthorizationCompanyLabel.Size = new System.Drawing.Size(134, 16);
            this.secondaryAuthorizationCompanyLabel.TabIndex = 300;
            this.secondaryAuthorizationCompanyLabel.Text = "Authorization company:";
            // 
            // secondaryAuthorizationRequiredLabel
            // 
            this.secondaryAuthorizationRequiredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAuthorizationRequiredLabel.Location = new System.Drawing.Point(7, 4671);
            this.secondaryAuthorizationRequiredLabel.Name = "secondaryAuthorizationRequiredLabel";
            this.secondaryAuthorizationRequiredLabel.Size = new System.Drawing.Size(130, 16);
            this.secondaryAuthorizationRequiredLabel.TabIndex = 298;
            this.secondaryAuthorizationRequiredLabel.Text = "Authorization required:";
            // 
            // secondaryDateLabel
            // 
            this.secondaryDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryDateLabel.Location = new System.Drawing.Point(7, 4645);
            this.secondaryDateLabel.Name = "secondaryDateLabel";
            this.secondaryDateLabel.Size = new System.Drawing.Size(32, 16);
            this.secondaryDateLabel.TabIndex = 296;
            this.secondaryDateLabel.Text = "Date:";
            // 
            // secondaryInitiatedByLabel
            // 
            this.secondaryInitiatedByLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInitiatedByLabel.Location = new System.Drawing.Point(7, 4619);
            this.secondaryInitiatedByLabel.Name = "secondaryInitiatedByLabel";
            this.secondaryInitiatedByLabel.Size = new System.Drawing.Size(72, 16);
            this.secondaryInitiatedByLabel.TabIndex = 294;
            this.secondaryInitiatedByLabel.Text = "Initiated by:";
            // 
            // secondaryBenefitsVerifiedLabel
            // 
            this.secondaryBenefitsVerifiedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBenefitsVerifiedLabel.Location = new System.Drawing.Point(7, 4593);
            this.secondaryBenefitsVerifiedLabel.Name = "secondaryBenefitsVerifiedLabel";
            this.secondaryBenefitsVerifiedLabel.Size = new System.Drawing.Size(99, 16);
            this.secondaryBenefitsVerifiedLabel.TabIndex = 292;
            this.secondaryBenefitsVerifiedLabel.Text = "Benefits verified:";
            // 
            // secondaryInsuranceVerificationLabel
            // 
            this.secondaryInsuranceVerificationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsuranceVerificationLabel.Location = new System.Drawing.Point(7, 4557);
            this.secondaryInsuranceVerificationLabel.Name = "secondaryInsuranceVerificationLabel";
            this.secondaryInsuranceVerificationLabel.Size = new System.Drawing.Size(304, 20);
            this.secondaryInsuranceVerificationLabel.TabIndex = 291;
            this.secondaryInsuranceVerificationLabel.Text = "Secondary Insurance – Verification";
            // 
            // secondaryEmployerPhoneValueLabel
            // 
            this.secondaryEmployerPhoneValueLabel.AutoSize = true;
            this.secondaryEmployerPhoneValueLabel.Location = new System.Drawing.Point(113, 4527);
            this.secondaryEmployerPhoneValueLabel.Name = "secondaryEmployerPhoneValueLabel";
            this.secondaryEmployerPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryEmployerPhoneValueLabel.TabIndex = 290;
            // 
            // secondaryEmployerValueLabel
            // 
            this.secondaryEmployerValueLabel.AutoSize = true;
            this.secondaryEmployerValueLabel.Location = new System.Drawing.Point(70, 4501);
            this.secondaryEmployerValueLabel.Name = "secondaryEmployerValueLabel";
            this.secondaryEmployerValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryEmployerValueLabel.TabIndex = 288;
            // 
            // secondaryEmploymentStatusValueLabel
            // 
            this.secondaryEmploymentStatusValueLabel.AutoSize = true;
            this.secondaryEmploymentStatusValueLabel.Location = new System.Drawing.Point(128, 4475);
            this.secondaryEmploymentStatusValueLabel.Name = "secondaryEmploymentStatusValueLabel";
            this.secondaryEmploymentStatusValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryEmploymentStatusValueLabel.TabIndex = 286;
            // 
            // secondaryContactCellValueLabel
            // 
            this.secondaryContactCellValueLabel.AutoSize = true;
            this.secondaryContactCellValueLabel.Location = new System.Drawing.Point(89, 4449);
            this.secondaryContactCellValueLabel.Name = "secondaryContactCellValueLabel";
            this.secondaryContactCellValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryContactCellValueLabel.TabIndex = 284;
            // 
            // secondaryContactPhoneValueLabel
            // 
            this.secondaryContactPhoneValueLabel.AutoSize = true;
            this.secondaryContactPhoneValueLabel.Location = new System.Drawing.Point(104, 4423);
            this.secondaryContactPhoneValueLabel.Name = "secondaryContactPhoneValueLabel";
            this.secondaryContactPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryContactPhoneValueLabel.TabIndex = 282;
            // 
            // secondaryAddressValueLabel
            // 
            this.secondaryAddressValueLabel.AutoSize = true;
            this.secondaryAddressValueLabel.Location = new System.Drawing.Point(64, 4397);
            this.secondaryAddressValueLabel.Name = "secondaryAddressValueLabel";
            this.secondaryAddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAddressValueLabel.TabIndex = 280;
            // 
            // secondaryNationalIdValueLabel
            // 
            this.secondaryNationalIdValueLabel.AutoSize = true;
            this.secondaryNationalIdValueLabel.Location = new System.Drawing.Point(86, 4371);
            this.secondaryNationalIdValueLabel.Name = "secondaryNationalIdValueLabel";
            this.secondaryNationalIdValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryNationalIdValueLabel.TabIndex = 278;
            // 
            // secondaryDobValueLabel
            // 
            this.secondaryDobValueLabel.AutoSize = true;
            this.secondaryDobValueLabel.Location = new System.Drawing.Point(46, 4345);
            this.secondaryDobValueLabel.Name = "secondaryDobValueLabel";
            this.secondaryDobValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryDobValueLabel.TabIndex = 276;
            // 
            // secondaryGenderValueLabel
            // 
            this.secondaryGenderValueLabel.AutoSize = true;
            this.secondaryGenderValueLabel.Location = new System.Drawing.Point(60, 4319);
            this.secondaryGenderValueLabel.Name = "secondaryGenderValueLabel";
            this.secondaryGenderValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryGenderValueLabel.TabIndex = 274;
            // 
            // secondaryThePatientIsTheInsuredValueLabel
            // 
            this.secondaryThePatientIsTheInsuredValueLabel.AutoSize = true;
            this.secondaryThePatientIsTheInsuredValueLabel.Location = new System.Drawing.Point(186, 4293);
            this.secondaryThePatientIsTheInsuredValueLabel.Name = "secondaryThePatientIsTheInsuredValueLabel";
            this.secondaryThePatientIsTheInsuredValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryThePatientIsTheInsuredValueLabel.TabIndex = 272;
            // 
            // secondaryNameValueLabel
            // 
            this.secondaryNameValueLabel.AutoSize = true;
            this.secondaryNameValueLabel.Location = new System.Drawing.Point(52, 4267);
            this.secondaryNameValueLabel.Name = "secondaryNameValueLabel";
            this.secondaryNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryNameValueLabel.TabIndex = 270;
            // 
            // secondaryEmployerPhoneLabel
            // 
            this.secondaryEmployerPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEmployerPhoneLabel.Location = new System.Drawing.Point(7, 4527);
            this.secondaryEmployerPhoneLabel.Name = "secondaryEmployerPhoneLabel";
            this.secondaryEmployerPhoneLabel.Size = new System.Drawing.Size(100, 16);
            this.secondaryEmployerPhoneLabel.TabIndex = 289;
            this.secondaryEmployerPhoneLabel.Text = "Employer phone:";
            // 
            // secondaryEmployerLabel
            // 
            this.secondaryEmployerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEmployerLabel.Location = new System.Drawing.Point(7, 4501);
            this.secondaryEmployerLabel.Name = "secondaryEmployerLabel";
            this.secondaryEmployerLabel.Size = new System.Drawing.Size(57, 16);
            this.secondaryEmployerLabel.TabIndex = 287;
            this.secondaryEmployerLabel.Text = "Employer:";
            // 
            // secondaryEmploymentStatusLabel
            // 
            this.secondaryEmploymentStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEmploymentStatusLabel.Location = new System.Drawing.Point(7, 4475);
            this.secondaryEmploymentStatusLabel.Name = "secondaryEmploymentStatusLabel";
            this.secondaryEmploymentStatusLabel.Size = new System.Drawing.Size(115, 16);
            this.secondaryEmploymentStatusLabel.TabIndex = 285;
            this.secondaryEmploymentStatusLabel.Text = "Employment status:";
            // 
            // secondaryContactCellLabel
            // 
            this.secondaryContactCellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryContactCellLabel.Location = new System.Drawing.Point(7, 4449);
            this.secondaryContactCellLabel.Name = "secondaryContactCellLabel";
            this.secondaryContactCellLabel.Size = new System.Drawing.Size(76, 16);
            this.secondaryContactCellLabel.TabIndex = 283;
            this.secondaryContactCellLabel.Text = "Contact cell:";
            // 
            // secondaryContactPhoneLabel
            // 
            this.secondaryContactPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryContactPhoneLabel.Location = new System.Drawing.Point(7, 4423);
            this.secondaryContactPhoneLabel.Name = "secondaryContactPhoneLabel";
            this.secondaryContactPhoneLabel.Size = new System.Drawing.Size(91, 16);
            this.secondaryContactPhoneLabel.TabIndex = 281;
            this.secondaryContactPhoneLabel.Text = "Contact phone:";
            // 
            // secondaryAddressLabel
            // 
            this.secondaryAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAddressLabel.Location = new System.Drawing.Point(7, 4397);
            this.secondaryAddressLabel.Name = "secondaryAddressLabel";
            this.secondaryAddressLabel.Size = new System.Drawing.Size(51, 16);
            this.secondaryAddressLabel.TabIndex = 279;
            this.secondaryAddressLabel.Text = "Address:";
            // 
            // secondaryNationalIdLabel
            // 
            this.secondaryNationalIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryNationalIdLabel.Location = new System.Drawing.Point(7, 4371);
            this.secondaryNationalIdLabel.Name = "secondaryNationalIdLabel";
            this.secondaryNationalIdLabel.Size = new System.Drawing.Size(73, 16);
            this.secondaryNationalIdLabel.TabIndex = 277;
            this.secondaryNationalIdLabel.Text = "National ID:";
            // 
            // secondaryDobLabel
            // 
            this.secondaryDobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryDobLabel.Location = new System.Drawing.Point(7, 4345);
            this.secondaryDobLabel.Name = "secondaryDobLabel";
            this.secondaryDobLabel.Size = new System.Drawing.Size(33, 16);
            this.secondaryDobLabel.TabIndex = 275;
            this.secondaryDobLabel.Text = "DOB:";
            // 
            // secondaryGenderLabel
            // 
            this.secondaryGenderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryGenderLabel.Location = new System.Drawing.Point(7, 4319);
            this.secondaryGenderLabel.Name = "secondaryGenderLabel";
            this.secondaryGenderLabel.Size = new System.Drawing.Size(47, 16);
            this.secondaryGenderLabel.TabIndex = 273;
            this.secondaryGenderLabel.Text = "Gender:";
            // 
            // secondaryThePatientIsTheInsuredLabel
            // 
            this.secondaryThePatientIsTheInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryThePatientIsTheInsuredLabel.Location = new System.Drawing.Point(7, 4293);
            this.secondaryThePatientIsTheInsuredLabel.Name = "secondaryThePatientIsTheInsuredLabel";
            this.secondaryThePatientIsTheInsuredLabel.Size = new System.Drawing.Size(173, 16);
            this.secondaryThePatientIsTheInsuredLabel.TabIndex = 271;
            this.secondaryThePatientIsTheInsuredLabel.Text = "The Patient is the Insured’s:";
            // 
            // secondaryNameLabel
            // 
            this.secondaryNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryNameLabel.Location = new System.Drawing.Point(7, 4267);
            this.secondaryNameLabel.Name = "secondaryNameLabel";
            this.secondaryNameLabel.Size = new System.Drawing.Size(39, 16);
            this.secondaryNameLabel.TabIndex = 269;
            this.secondaryNameLabel.Text = "Name:";
            // 
            // secondaryBillingPhoneValueLabel
            // 
            this.secondaryBillingPhoneValueLabel.AutoSize = true;
            this.secondaryBillingPhoneValueLabel.Location = new System.Drawing.Point(95, 4175);
            this.secondaryBillingPhoneValueLabel.Name = "secondaryBillingPhoneValueLabel";
            this.secondaryBillingPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryBillingPhoneValueLabel.TabIndex = 265;
            // 
            // secondaryBillingNameValueLabel
            // 
            this.secondaryBillingNameValueLabel.AutoSize = true;
            this.secondaryBillingNameValueLabel.Location = new System.Drawing.Point(85, 4149);
            this.secondaryBillingNameValueLabel.Name = "secondaryBillingNameValueLabel";
            this.secondaryBillingNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryBillingNameValueLabel.TabIndex = 263;
            // 
            // secondaryBillingCoNameValueLabel
            // 
            this.secondaryBillingCoNameValueLabel.AutoSize = true;
            this.secondaryBillingCoNameValueLabel.Location = new System.Drawing.Point(111, 4123);
            this.secondaryBillingCoNameValueLabel.Name = "secondaryBillingCoNameValueLabel";
            this.secondaryBillingCoNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryBillingCoNameValueLabel.TabIndex = 261;
            // 
            // secondaryMedicalGroupIpaClinicValueLabel
            // 
            this.secondaryMedicalGroupIpaClinicValueLabel.AutoSize = true;
            this.secondaryMedicalGroupIpaClinicValueLabel.Location = new System.Drawing.Point(169, 4097);
            this.secondaryMedicalGroupIpaClinicValueLabel.Name = "secondaryMedicalGroupIpaClinicValueLabel";
            this.secondaryMedicalGroupIpaClinicValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryMedicalGroupIpaClinicValueLabel.TabIndex = 259;
            // 
            // secondaryMedicalGroupIpaNameValueLabel
            // 
            this.secondaryMedicalGroupIpaNameValueLabel.AutoSize = true;
            this.secondaryMedicalGroupIpaNameValueLabel.Location = new System.Drawing.Point(171, 4071);
            this.secondaryMedicalGroupIpaNameValueLabel.Name = "secondaryMedicalGroupIpaNameValueLabel";
            this.secondaryMedicalGroupIpaNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryMedicalGroupIpaNameValueLabel.TabIndex = 257;
            // 
            // secondaryAdjusterValueLabel
            // 
            this.secondaryAdjusterValueLabel.AutoSize = true;
            this.secondaryAdjusterValueLabel.Location = new System.Drawing.Point(64, 4045);
            this.secondaryAdjusterValueLabel.Name = "secondaryAdjusterValueLabel";
            this.secondaryAdjusterValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryAdjusterValueLabel.TabIndex = 255;
            // 
            // secondaryEmployeeSupervisorValueLabel
            // 
            this.secondaryEmployeeSupervisorValueLabel.AutoSize = true;
            this.secondaryEmployeeSupervisorValueLabel.Location = new System.Drawing.Point(146, 4019);
            this.secondaryEmployeeSupervisorValueLabel.Name = "secondaryEmployeeSupervisorValueLabel";
            this.secondaryEmployeeSupervisorValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryEmployeeSupervisorValueLabel.TabIndex = 253;
            // 
            // secondaryMedicaidIssueDateValueLabel
            // 
            this.secondaryMedicaidIssueDateValueLabel.AutoSize = true;
            this.secondaryMedicaidIssueDateValueLabel.Location = new System.Drawing.Point(132, 3993);
            this.secondaryMedicaidIssueDateValueLabel.Name = "secondaryMedicaidIssueDateValueLabel";
            this.secondaryMedicaidIssueDateValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryMedicaidIssueDateValueLabel.TabIndex = 251;
            // 
            // secondaryPrecertificationNumberValueLabel
            // 
            this.secondaryPrecertificationNumberValueLabel.AutoSize = true;
            this.secondaryPrecertificationNumberValueLabel.Location = new System.Drawing.Point(149, 3967);
            this.secondaryPrecertificationNumberValueLabel.Name = "secondaryPrecertificationNumberValueLabel";
            this.secondaryPrecertificationNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryPrecertificationNumberValueLabel.TabIndex = 249;
            // 
            // secondaryGroupNumberValueLabel
            // 
            this.secondaryGroupNumberValueLabel.AutoSize = true;
            this.secondaryGroupNumberValueLabel.Location = new System.Drawing.Point(96, 3941);
            this.secondaryGroupNumberValueLabel.Name = "secondaryGroupNumberValueLabel";
            this.secondaryGroupNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryGroupNumberValueLabel.TabIndex = 241;
            // 
            // secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel
            // 
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location = new System.Drawing.Point(146, 3915);
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Name = "secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel";
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Size = new System.Drawing.Size(200, 16);
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.TabIndex = 239;
            // 
            // secondaryInsuranceInsuredLabel
            // 
            this.secondaryInsuranceInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsuranceInsuredLabel.Location = new System.Drawing.Point(7, 4231);
            this.secondaryInsuranceInsuredLabel.Name = "secondaryInsuranceInsuredLabel";
            this.secondaryInsuranceInsuredLabel.Size = new System.Drawing.Size(254, 20);
            this.secondaryInsuranceInsuredLabel.TabIndex = 268;
            this.secondaryInsuranceInsuredLabel.Text = "Secondary Insurance – Insured";
            // 
            // secondaryBillingPhoneLabel
            // 
            this.secondaryBillingPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBillingPhoneLabel.Location = new System.Drawing.Point(7, 4175);
            this.secondaryBillingPhoneLabel.Name = "secondaryBillingPhoneLabel";
            this.secondaryBillingPhoneLabel.Size = new System.Drawing.Size(82, 16);
            this.secondaryBillingPhoneLabel.TabIndex = 264;
            this.secondaryBillingPhoneLabel.Text = "Billing phone:";
            // 
            // secondaryBillingNameLabel
            // 
            this.secondaryBillingNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBillingNameLabel.Location = new System.Drawing.Point(7, 4149);
            this.secondaryBillingNameLabel.Name = "secondaryBillingNameLabel";
            this.secondaryBillingNameLabel.Size = new System.Drawing.Size(72, 16);
            this.secondaryBillingNameLabel.TabIndex = 262;
            this.secondaryBillingNameLabel.Text = "Billing name:";
            // 
            // secondaryBillingCoNameLabel
            // 
            this.secondaryBillingCoNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryBillingCoNameLabel.Location = new System.Drawing.Point(7, 4123);
            this.secondaryBillingCoNameLabel.Name = "secondaryBillingCoNameLabel";
            this.secondaryBillingCoNameLabel.Size = new System.Drawing.Size(98, 16);
            this.secondaryBillingCoNameLabel.TabIndex = 260;
            this.secondaryBillingCoNameLabel.Text = "Billing c/o name:";
            // 
            // secondaryMedicalGroupIpaClinicLabel
            // 
            this.secondaryMedicalGroupIpaClinicLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryMedicalGroupIpaClinicLabel.Location = new System.Drawing.Point(7, 4097);
            this.secondaryMedicalGroupIpaClinicLabel.Name = "secondaryMedicalGroupIpaClinicLabel";
            this.secondaryMedicalGroupIpaClinicLabel.Size = new System.Drawing.Size(156, 16);
            this.secondaryMedicalGroupIpaClinicLabel.TabIndex = 258;
            this.secondaryMedicalGroupIpaClinicLabel.Text = "Medical group/IPA clinic:";
            // 
            // secondaryMedicalGroupIpaNameLabel
            // 
            this.secondaryMedicalGroupIpaNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryMedicalGroupIpaNameLabel.Location = new System.Drawing.Point(7, 4071);
            this.secondaryMedicalGroupIpaNameLabel.Name = "secondaryMedicalGroupIpaNameLabel";
            this.secondaryMedicalGroupIpaNameLabel.Size = new System.Drawing.Size(158, 16);
            this.secondaryMedicalGroupIpaNameLabel.TabIndex = 256;
            this.secondaryMedicalGroupIpaNameLabel.Text = "Medical group/IPA name:";
            // 
            // secondaryAdjusterLabel
            // 
            this.secondaryAdjusterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryAdjusterLabel.Location = new System.Drawing.Point(7, 4045);
            this.secondaryAdjusterLabel.Name = "secondaryAdjusterLabel";
            this.secondaryAdjusterLabel.Size = new System.Drawing.Size(51, 16);
            this.secondaryAdjusterLabel.TabIndex = 254;
            this.secondaryAdjusterLabel.Text = "Adjuster:";
            // 
            // secondaryEmployeeSupervisorLabel
            // 
            this.secondaryEmployeeSupervisorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryEmployeeSupervisorLabel.Location = new System.Drawing.Point(7, 4019);
            this.secondaryEmployeeSupervisorLabel.Name = "secondaryEmployeeSupervisorLabel";
            this.secondaryEmployeeSupervisorLabel.Size = new System.Drawing.Size(133, 16);
            this.secondaryEmployeeSupervisorLabel.TabIndex = 252;
            this.secondaryEmployeeSupervisorLabel.Text = "Employee’s supervisor:";
            // 
            // secondaryMedicaidIssueDateLabel
            // 
            this.secondaryMedicaidIssueDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryMedicaidIssueDateLabel.Location = new System.Drawing.Point(7, 3993);
            this.secondaryMedicaidIssueDateLabel.Name = "secondaryMedicaidIssueDateLabel";
            this.secondaryMedicaidIssueDateLabel.Size = new System.Drawing.Size(119, 16);
            this.secondaryMedicaidIssueDateLabel.TabIndex = 250;
            this.secondaryMedicaidIssueDateLabel.Text = "Medicaid issue date:";
            // 
            // secondaryPrecertificationNumberLabel
            // 
            this.secondaryPrecertificationNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryPrecertificationNumberLabel.Location = new System.Drawing.Point(7, 3967);
            this.secondaryPrecertificationNumberLabel.Name = "secondaryPrecertificationNumberLabel";
            this.secondaryPrecertificationNumberLabel.Size = new System.Drawing.Size(136, 16);
            this.secondaryPrecertificationNumberLabel.TabIndex = 248;
            this.secondaryPrecertificationNumberLabel.Text = "Precertification number:";
            // 
            // secondaryGroupNumberLabel
            // 
            this.secondaryGroupNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryGroupNumberLabel.Location = new System.Drawing.Point(7, 3941);
            this.secondaryGroupNumberLabel.Name = "secondaryGroupNumberLabel";
            this.secondaryGroupNumberLabel.Size = new System.Drawing.Size(83, 16);
            this.secondaryGroupNumberLabel.TabIndex = 240;
            this.secondaryGroupNumberLabel.Text = "Group number:";
            //// 
            //// secondaryMBILabel
            //// 
            //this.secondaryMBILabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.secondaryMBILabel.Location = new System.Drawing.Point(7, 3889);
            //this.secondaryMBILabel.Name = "secondaryMBILabel";
            //this.secondaryMBILabel.Size = new System.Drawing.Size(132, 16);
            //this.secondaryMBILabel.TabIndex = 238;
            //this.secondaryMBILabel.Visible = false;
            //this.secondaryMBILabel.Text = "MBI:";
            // 
            // secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel
            // 
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Location = new System.Drawing.Point(7, 3915);
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Name = "secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel";
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size = new System.Drawing.Size(132, 16);
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.TabIndex = 238;
            this.secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text = "CERT/SSN/ID number:";
            // 
            // secondaryInsurancePayorDetailsLabel
            // 
            this.secondaryInsurancePayorDetailsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsurancePayorDetailsLabel.Location = new System.Drawing.Point(7, 3879);
            this.secondaryInsurancePayorDetailsLabel.Name = "secondaryInsurancePayorDetailsLabel";
            this.secondaryInsurancePayorDetailsLabel.Size = new System.Drawing.Size(302, 20);
            this.secondaryInsurancePayorDetailsLabel.TabIndex = 237;
            this.secondaryInsurancePayorDetailsLabel.Text = "Secondary Insurance – Payor Details";
            // 
            // secondaryCategoryValueLabel
            // 
            this.secondaryCategoryValueLabel.AutoSize = true;
            this.secondaryCategoryValueLabel.Location = new System.Drawing.Point(68, 3849);
            this.secondaryCategoryValueLabel.Name = "secondaryCategoryValueLabel";
            this.secondaryCategoryValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryCategoryValueLabel.TabIndex = 236;
            // 
            // secondaryPayorBrokerValueLabel
            // 
            this.secondaryPayorBrokerValueLabel.AutoSize = true;
            this.secondaryPayorBrokerValueLabel.Location = new System.Drawing.Point(96, 3823);
            this.secondaryPayorBrokerValueLabel.Name = "secondaryPayorBrokerValueLabel";
            this.secondaryPayorBrokerValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryPayorBrokerValueLabel.TabIndex = 234;
            // 
            // secondaryPlanValueLabel
            // 
            this.secondaryPlanValueLabel.AutoSize = true;
            this.secondaryPlanValueLabel.Location = new System.Drawing.Point(44, 3797);
            this.secondaryPlanValueLabel.Name = "secondaryPlanValueLabel";
            this.secondaryPlanValueLabel.Size = new System.Drawing.Size(0, 13);
            this.secondaryPlanValueLabel.TabIndex = 232;
            // 
            // secondaryCategoryLabel
            // 
            this.secondaryCategoryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryCategoryLabel.Location = new System.Drawing.Point(7, 3849);
            this.secondaryCategoryLabel.Name = "secondaryCategoryLabel";
            this.secondaryCategoryLabel.Size = new System.Drawing.Size(55, 16);
            this.secondaryCategoryLabel.TabIndex = 235;
            this.secondaryCategoryLabel.Text = "Category:";
            // 
            // secondaryPayorBrokerLabel
            // 
            this.secondaryPayorBrokerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryPayorBrokerLabel.Location = new System.Drawing.Point(7, 3823);
            this.secondaryPayorBrokerLabel.Name = "secondaryPayorBrokerLabel";
            this.secondaryPayorBrokerLabel.Size = new System.Drawing.Size(83, 16);
            this.secondaryPayorBrokerLabel.TabIndex = 233;
            this.secondaryPayorBrokerLabel.Text = "Payor/Broker:";
            // 
            // secondaryPlanLabel
            // 
            this.secondaryPlanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryPlanLabel.Location = new System.Drawing.Point(7, 3797);
            this.secondaryPlanLabel.Name = "secondaryPlanLabel";
            this.secondaryPlanLabel.Size = new System.Drawing.Size(31, 16);
            this.secondaryPlanLabel.TabIndex = 231;
            this.secondaryPlanLabel.Text = "Plan:";
            // 
            // secondaryInsurancePlanLabel
            // 
            this.secondaryInsurancePlanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondaryInsurancePlanLabel.Location = new System.Drawing.Point(7, 3761);
            this.secondaryInsurancePlanLabel.Name = "secondaryInsurancePlanLabel";
            this.secondaryInsurancePlanLabel.Size = new System.Drawing.Size(231, 20);
            this.secondaryInsurancePlanLabel.TabIndex = 230;
            this.secondaryInsurancePlanLabel.Text = "Secondary Insurance - Plan";
            // 
            // primaryAutoHomeInsuranceAgentPhoneNumberValueLabel
            // 
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.AutoSize = true;
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Location = new System.Drawing.Point(262, 3457);
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Name = "primaryAutoHomeInsuranceAgentPhoneNumberValueLabel";
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAutoHomeInsuranceAgentPhoneNumberValueLabel.TabIndex = 229;
            // 
            // primaryAutoHomeInsuranceAgentAddressValueLabel
            // 
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.AutoSize = true;
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.Location = new System.Drawing.Point(235, 3431);
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.Name = "primaryAutoHomeInsuranceAgentAddressValueLabel";
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAutoHomeInsuranceAgentAddressValueLabel.TabIndex = 227;
            // 
            // primaryAutoHomeInsuranceAgentNameValueLabel
            // 
            this.primaryAutoHomeInsuranceAgentNameValueLabel.AutoSize = true;
            this.primaryAutoHomeInsuranceAgentNameValueLabel.Location = new System.Drawing.Point(223, 3405);
            this.primaryAutoHomeInsuranceAgentNameValueLabel.Name = "primaryAutoHomeInsuranceAgentNameValueLabel";
            this.primaryAutoHomeInsuranceAgentNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAutoHomeInsuranceAgentNameValueLabel.TabIndex = 225;
            // 
            // primaryAttorneyPhoneNumberValueLabel
            // 
            this.primaryAttorneyPhoneNumberValueLabel.AutoSize = true;
            this.primaryAttorneyPhoneNumberValueLabel.Location = new System.Drawing.Point(150, 3379);
            this.primaryAttorneyPhoneNumberValueLabel.Name = "primaryAttorneyPhoneNumberValueLabel";
            this.primaryAttorneyPhoneNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAttorneyPhoneNumberValueLabel.TabIndex = 223;
            // 
            // primaryAttorneyAddressValueLabel
            // 
            this.primaryAttorneyAddressValueLabel.AutoSize = true;
            this.primaryAttorneyAddressValueLabel.Location = new System.Drawing.Point(117, 3353);
            this.primaryAttorneyAddressValueLabel.Name = "primaryAttorneyAddressValueLabel";
            this.primaryAttorneyAddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAttorneyAddressValueLabel.TabIndex = 221;
            // 
            // primaryAttorneyNameValueLabel
            // 
            this.primaryAttorneyNameValueLabel.AutoSize = true;
            this.primaryAttorneyNameValueLabel.Location = new System.Drawing.Point(104, 3327);
            this.primaryAttorneyNameValueLabel.Name = "primaryAttorneyNameValueLabel";
            this.primaryAttorneyNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAttorneyNameValueLabel.TabIndex = 219;
            // 
            // primaryPromptExtValueLabel
            // 
            this.primaryPromptExtValueLabel.AutoSize = true;
            this.primaryPromptExtValueLabel.Location = new System.Drawing.Point(78, 3301);
            this.primaryPromptExtValueLabel.Name = "primaryPromptExtValueLabel";
            this.primaryPromptExtValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryPromptExtValueLabel.TabIndex = 217;
            // 
            // primaryAuthorizationPhoneValueLabel
            // 
            this.primaryAuthorizationPhoneValueLabel.AutoSize = true;
            this.primaryAuthorizationPhoneValueLabel.Location = new System.Drawing.Point(132, 3275);
            this.primaryAuthorizationPhoneValueLabel.Name = "primaryAuthorizationPhoneValueLabel";
            this.primaryAuthorizationPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAuthorizationPhoneValueLabel.TabIndex = 215;
            // 
            // primaryAuthorizationCompanyValueLabel
            // 
            this.primaryAuthorizationCompanyValueLabel.AutoSize = true;
            this.primaryAuthorizationCompanyValueLabel.Location = new System.Drawing.Point(147, 3249);
            this.primaryAuthorizationCompanyValueLabel.Name = "primaryAuthorizationCompanyValueLabel";
            this.primaryAuthorizationCompanyValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAuthorizationCompanyValueLabel.TabIndex = 213;
            // 
            // primaryAuthorizationRequiredValueLabel
            // 
            this.primaryAuthorizationRequiredValueLabel.AutoSize = true;
            this.primaryAuthorizationRequiredValueLabel.Location = new System.Drawing.Point(143, 3223);
            this.primaryAuthorizationRequiredValueLabel.Name = "primaryAuthorizationRequiredValueLabel";
            this.primaryAuthorizationRequiredValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAuthorizationRequiredValueLabel.TabIndex = 211;
            // 
            // primaryDateValueLabel
            // 
            this.primaryDateValueLabel.AutoSize = true;
            this.primaryDateValueLabel.Location = new System.Drawing.Point(45, 3197);
            this.primaryDateValueLabel.Name = "primaryDateValueLabel";
            this.primaryDateValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryDateValueLabel.TabIndex = 209;
            // 
            // primaryInitiatedByValueLabel
            // 
            this.primaryInitiatedByValueLabel.AutoSize = true;
            this.primaryInitiatedByValueLabel.Location = new System.Drawing.Point(85, 3171);
            this.primaryInitiatedByValueLabel.Name = "primaryInitiatedByValueLabel";
            this.primaryInitiatedByValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryInitiatedByValueLabel.TabIndex = 207;
            // 
            // primaryBenefitsVerifiedValueLabel
            // 
            this.primaryBenefitsVerifiedValueLabel.AutoSize = true;
            this.primaryBenefitsVerifiedValueLabel.Location = new System.Drawing.Point(112, 3145);
            this.primaryBenefitsVerifiedValueLabel.Name = "primaryBenefitsVerifiedValueLabel";
            this.primaryBenefitsVerifiedValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryBenefitsVerifiedValueLabel.TabIndex = 205;
            // 
            // primaryAutoHomeInsuranceAgentPhoneNumberLabel
            // 
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Location = new System.Drawing.Point(7, 3457);
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Name = "primaryAutoHomeInsuranceAgentPhoneNumberLabel";
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Size = new System.Drawing.Size(249, 16);
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.TabIndex = 228;
            this.primaryAutoHomeInsuranceAgentPhoneNumberLabel.Text = "Auto/Home Insurance Agent phone number:";
            // 
            // primaryAutoHomeInsuranceAgentAddressLabel
            // 
            this.primaryAutoHomeInsuranceAgentAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAutoHomeInsuranceAgentAddressLabel.Location = new System.Drawing.Point(7, 3431);
            this.primaryAutoHomeInsuranceAgentAddressLabel.Name = "primaryAutoHomeInsuranceAgentAddressLabel";
            this.primaryAutoHomeInsuranceAgentAddressLabel.Size = new System.Drawing.Size(222, 16);
            this.primaryAutoHomeInsuranceAgentAddressLabel.TabIndex = 226;
            this.primaryAutoHomeInsuranceAgentAddressLabel.Text = "Auto/Home Insurance Agent address:";
            // 
            // primaryAutoHomeInsuranceAgentNameLabel
            // 
            this.primaryAutoHomeInsuranceAgentNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAutoHomeInsuranceAgentNameLabel.Location = new System.Drawing.Point(7, 3405);
            this.primaryAutoHomeInsuranceAgentNameLabel.Name = "primaryAutoHomeInsuranceAgentNameLabel";
            this.primaryAutoHomeInsuranceAgentNameLabel.Size = new System.Drawing.Size(210, 16);
            this.primaryAutoHomeInsuranceAgentNameLabel.TabIndex = 224;
            this.primaryAutoHomeInsuranceAgentNameLabel.Text = "Auto/Home Insurance Agent name:";
            // 
            // primaryAttorneyPhoneNumberLabel
            // 
            this.primaryAttorneyPhoneNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAttorneyPhoneNumberLabel.Location = new System.Drawing.Point(7, 3379);
            this.primaryAttorneyPhoneNumberLabel.Name = "primaryAttorneyPhoneNumberLabel";
            this.primaryAttorneyPhoneNumberLabel.Size = new System.Drawing.Size(137, 16);
            this.primaryAttorneyPhoneNumberLabel.TabIndex = 222;
            this.primaryAttorneyPhoneNumberLabel.Text = "Attorney phone number:";
            // 
            // primaryAttorneyAddressLabel
            // 
            this.primaryAttorneyAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAttorneyAddressLabel.Location = new System.Drawing.Point(7, 3353);
            this.primaryAttorneyAddressLabel.Name = "primaryAttorneyAddressLabel";
            this.primaryAttorneyAddressLabel.Size = new System.Drawing.Size(104, 16);
            this.primaryAttorneyAddressLabel.TabIndex = 220;
            this.primaryAttorneyAddressLabel.Text = "Attorney address:";
            // 
            // primaryAttorneyNameLabel
            // 
            this.primaryAttorneyNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAttorneyNameLabel.Location = new System.Drawing.Point(7, 3327);
            this.primaryAttorneyNameLabel.Name = "primaryAttorneyNameLabel";
            this.primaryAttorneyNameLabel.Size = new System.Drawing.Size(91, 16);
            this.primaryAttorneyNameLabel.TabIndex = 218;
            this.primaryAttorneyNameLabel.Text = "Attorney name:";
            // 
            // primaryPromptExtLabel
            // 
            this.primaryPromptExtLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryPromptExtLabel.Location = new System.Drawing.Point(7, 3301);
            this.primaryPromptExtLabel.Name = "primaryPromptExtLabel";
            this.primaryPromptExtLabel.Size = new System.Drawing.Size(65, 16);
            this.primaryPromptExtLabel.TabIndex = 216;
            this.primaryPromptExtLabel.Text = "Prompt/ext:";
            // 
            // primaryAuthorizationPhoneLabel
            // 
            this.primaryAuthorizationPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationPhoneLabel.Location = new System.Drawing.Point(7, 3275);
            this.primaryAuthorizationPhoneLabel.Name = "primaryAuthorizationPhoneLabel";
            this.primaryAuthorizationPhoneLabel.Size = new System.Drawing.Size(119, 16);
            this.primaryAuthorizationPhoneLabel.TabIndex = 214;
            this.primaryAuthorizationPhoneLabel.Text = "Authorization phone:";
            // 
            // primaryAuthorizationCompanyLabel
            // 
            this.primaryAuthorizationCompanyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationCompanyLabel.Location = new System.Drawing.Point(7, 3249);
            this.primaryAuthorizationCompanyLabel.Name = "primaryAuthorizationCompanyLabel";
            this.primaryAuthorizationCompanyLabel.Size = new System.Drawing.Size(134, 16);
            this.primaryAuthorizationCompanyLabel.TabIndex = 212;
            this.primaryAuthorizationCompanyLabel.Text = "Authorization company:";
            // 
            // primaryAuthorizationRequiredLabel
            // 
            this.primaryAuthorizationRequiredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAuthorizationRequiredLabel.Location = new System.Drawing.Point(7, 3223);
            this.primaryAuthorizationRequiredLabel.Name = "primaryAuthorizationRequiredLabel";
            this.primaryAuthorizationRequiredLabel.Size = new System.Drawing.Size(130, 16);
            this.primaryAuthorizationRequiredLabel.TabIndex = 210;
            this.primaryAuthorizationRequiredLabel.Text = "Authorization required:";
            // 
            // primaryDateLabel
            // 
            this.primaryDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryDateLabel.Location = new System.Drawing.Point(7, 3197);
            this.primaryDateLabel.Name = "primaryDateLabel";
            this.primaryDateLabel.Size = new System.Drawing.Size(32, 16);
            this.primaryDateLabel.TabIndex = 208;
            this.primaryDateLabel.Text = "Date:";
            // 
            // primaryInitiatedByLabel
            // 
            this.primaryInitiatedByLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInitiatedByLabel.Location = new System.Drawing.Point(7, 3171);
            this.primaryInitiatedByLabel.Name = "primaryInitiatedByLabel";
            this.primaryInitiatedByLabel.Size = new System.Drawing.Size(72, 16);
            this.primaryInitiatedByLabel.TabIndex = 206;
            this.primaryInitiatedByLabel.Text = "Initiated by:";
            // 
            // primaryBenefitsVerifiedLabel
            // 
            this.primaryBenefitsVerifiedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBenefitsVerifiedLabel.Location = new System.Drawing.Point(7, 3145);
            this.primaryBenefitsVerifiedLabel.Name = "primaryBenefitsVerifiedLabel";
            this.primaryBenefitsVerifiedLabel.Size = new System.Drawing.Size(99, 16);
            this.primaryBenefitsVerifiedLabel.TabIndex = 204;
            this.primaryBenefitsVerifiedLabel.Text = "Benefits verified:";
            // 
            // primaryEmployerPhoneValueLabel
            // 
            this.primaryEmployerPhoneValueLabel.AutoSize = true;
            this.primaryEmployerPhoneValueLabel.Location = new System.Drawing.Point(113, 3079);
            this.primaryEmployerPhoneValueLabel.Name = "primaryEmployerPhoneValueLabel";
            this.primaryEmployerPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryEmployerPhoneValueLabel.TabIndex = 202;
            // 
            // primaryEmployerValueLabel
            // 
            this.primaryEmployerValueLabel.AutoSize = true;
            this.primaryEmployerValueLabel.Location = new System.Drawing.Point(70, 3053);
            this.primaryEmployerValueLabel.Name = "primaryEmployerValueLabel";
            this.primaryEmployerValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryEmployerValueLabel.TabIndex = 200;
            // 
            // primaryEmploymentStatusValueLabel
            // 
            this.primaryEmploymentStatusValueLabel.AutoSize = true;
            this.primaryEmploymentStatusValueLabel.Location = new System.Drawing.Point(128, 3027);
            this.primaryEmploymentStatusValueLabel.Name = "primaryEmploymentStatusValueLabel";
            this.primaryEmploymentStatusValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryEmploymentStatusValueLabel.TabIndex = 198;
            // 
            // primaryContactCellValueLabel
            // 
            this.primaryContactCellValueLabel.AutoSize = true;
            this.primaryContactCellValueLabel.Location = new System.Drawing.Point(89, 3001);
            this.primaryContactCellValueLabel.Name = "primaryContactCellValueLabel";
            this.primaryContactCellValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryContactCellValueLabel.TabIndex = 196;
            // 
            // primaryContactPhoneValueLabel
            // 
            this.primaryContactPhoneValueLabel.AutoSize = true;
            this.primaryContactPhoneValueLabel.Location = new System.Drawing.Point(104, 2975);
            this.primaryContactPhoneValueLabel.Name = "primaryContactPhoneValueLabel";
            this.primaryContactPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryContactPhoneValueLabel.TabIndex = 194;
            // 
            // primaryAddressValueLabel
            // 
            this.primaryAddressValueLabel.AutoSize = true;
            this.primaryAddressValueLabel.Location = new System.Drawing.Point(64, 2949);
            this.primaryAddressValueLabel.Name = "primaryAddressValueLabel";
            this.primaryAddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAddressValueLabel.TabIndex = 192;
            // 
            // primaryNationalIdValueLabel
            // 
            this.primaryNationalIdValueLabel.AutoSize = true;
            this.primaryNationalIdValueLabel.Location = new System.Drawing.Point(86, 2923);
            this.primaryNationalIdValueLabel.Name = "primaryNationalIdValueLabel";
            this.primaryNationalIdValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryNationalIdValueLabel.TabIndex = 190;
            // 
            // primaryDobValueLabel
            // 
            this.primaryDobValueLabel.AutoSize = true;
            this.primaryDobValueLabel.Location = new System.Drawing.Point(46, 2897);
            this.primaryDobValueLabel.Name = "primaryDobValueLabel";
            this.primaryDobValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryDobValueLabel.TabIndex = 188;
            // 
            // primaryGenderValueLabel
            // 
            this.primaryGenderValueLabel.AutoSize = true;
            this.primaryGenderValueLabel.Location = new System.Drawing.Point(60, 2871);
            this.primaryGenderValueLabel.Name = "primaryGenderValueLabel";
            this.primaryGenderValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryGenderValueLabel.TabIndex = 186;
            // 
            // primaryThePatientIsTheInsuredsValueLabel
            // 
            this.primaryThePatientIsTheInsuredsValueLabel.AutoSize = true;
            this.primaryThePatientIsTheInsuredsValueLabel.Location = new System.Drawing.Point(176, 2845);
            this.primaryThePatientIsTheInsuredsValueLabel.Name = "primaryThePatientIsTheInsuredsValueLabel";
            this.primaryThePatientIsTheInsuredsValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryThePatientIsTheInsuredsValueLabel.TabIndex = 184;
            // 
            // primaryNameValueLabel
            // 
            this.primaryNameValueLabel.AutoSize = true;
            this.primaryNameValueLabel.Location = new System.Drawing.Point(52, 2819);
            this.primaryNameValueLabel.Name = "primaryNameValueLabel";
            this.primaryNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryNameValueLabel.TabIndex = 182;
            // 
            // primaryInsuranceVerificationLabel
            // 
            this.primaryInsuranceVerificationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsuranceVerificationLabel.Location = new System.Drawing.Point(7, 3109);
            this.primaryInsuranceVerificationLabel.Name = "primaryInsuranceVerificationLabel";
            this.primaryInsuranceVerificationLabel.Size = new System.Drawing.Size(290, 20);
            this.primaryInsuranceVerificationLabel.TabIndex = 203;
            this.primaryInsuranceVerificationLabel.Text = "Primary Insurance – Verification";
            // 
            // primaryEmployerPhoneLabel
            // 
            this.primaryEmployerPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEmployerPhoneLabel.Location = new System.Drawing.Point(7, 3079);
            this.primaryEmployerPhoneLabel.Name = "primaryEmployerPhoneLabel";
            this.primaryEmployerPhoneLabel.Size = new System.Drawing.Size(100, 16);
            this.primaryEmployerPhoneLabel.TabIndex = 201;
            this.primaryEmployerPhoneLabel.Text = "Employer phone:";
            // 
            // primaryEmployerLabel
            // 
            this.primaryEmployerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEmployerLabel.Location = new System.Drawing.Point(7, 3053);
            this.primaryEmployerLabel.Name = "primaryEmployerLabel";
            this.primaryEmployerLabel.Size = new System.Drawing.Size(57, 16);
            this.primaryEmployerLabel.TabIndex = 199;
            this.primaryEmployerLabel.Text = "Employer:";
            // 
            // primaryEmploymentStatusLabel
            // 
            this.primaryEmploymentStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEmploymentStatusLabel.Location = new System.Drawing.Point(7, 3027);
            this.primaryEmploymentStatusLabel.Name = "primaryEmploymentStatusLabel";
            this.primaryEmploymentStatusLabel.Size = new System.Drawing.Size(115, 16);
            this.primaryEmploymentStatusLabel.TabIndex = 197;
            this.primaryEmploymentStatusLabel.Text = "Employment status:";
            // 
            // primaryContactCellLabel
            // 
            this.primaryContactCellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryContactCellLabel.Location = new System.Drawing.Point(7, 3001);
            this.primaryContactCellLabel.Name = "primaryContactCellLabel";
            this.primaryContactCellLabel.Size = new System.Drawing.Size(76, 16);
            this.primaryContactCellLabel.TabIndex = 195;
            this.primaryContactCellLabel.Text = "Contact cell:";
            // 
            // primaryContactPhoneLabel
            // 
            this.primaryContactPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryContactPhoneLabel.Location = new System.Drawing.Point(7, 2975);
            this.primaryContactPhoneLabel.Name = "primaryContactPhoneLabel";
            this.primaryContactPhoneLabel.Size = new System.Drawing.Size(91, 16);
            this.primaryContactPhoneLabel.TabIndex = 193;
            this.primaryContactPhoneLabel.Text = "Contact phone:";
            // 
            // primaryAddressLabel
            // 
            this.primaryAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAddressLabel.Location = new System.Drawing.Point(7, 2949);
            this.primaryAddressLabel.Name = "primaryAddressLabel";
            this.primaryAddressLabel.Size = new System.Drawing.Size(51, 16);
            this.primaryAddressLabel.TabIndex = 191;
            this.primaryAddressLabel.Text = "Address:";
            // 
            // primaryNationalIdLabel
            // 
            this.primaryNationalIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryNationalIdLabel.Location = new System.Drawing.Point(7, 2923);
            this.primaryNationalIdLabel.Name = "primaryNationalIdLabel";
            this.primaryNationalIdLabel.Size = new System.Drawing.Size(73, 16);
            this.primaryNationalIdLabel.TabIndex = 189;
            this.primaryNationalIdLabel.Text = "National ID:";
            // 
            // primaryDobLabel
            // 
            this.primaryDobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryDobLabel.Location = new System.Drawing.Point(7, 2897);
            this.primaryDobLabel.Name = "primaryDobLabel";
            this.primaryDobLabel.Size = new System.Drawing.Size(33, 16);
            this.primaryDobLabel.TabIndex = 187;
            this.primaryDobLabel.Text = "DOB:";
            // 
            // primaryGenderLabel
            // 
            this.primaryGenderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryGenderLabel.Location = new System.Drawing.Point(7, 2871);
            this.primaryGenderLabel.Name = "primaryGenderLabel";
            this.primaryGenderLabel.Size = new System.Drawing.Size(47, 16);
            this.primaryGenderLabel.TabIndex = 185;
            this.primaryGenderLabel.Text = "Gender:";
            // 
            // primaryThePatientIsTheInsuredsLabel
            // 
            this.primaryThePatientIsTheInsuredsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryThePatientIsTheInsuredsLabel.Location = new System.Drawing.Point(7, 2845);
            this.primaryThePatientIsTheInsuredsLabel.Name = "primaryThePatientIsTheInsuredsLabel";
            this.primaryThePatientIsTheInsuredsLabel.Size = new System.Drawing.Size(163, 16);
            this.primaryThePatientIsTheInsuredsLabel.TabIndex = 183;
            this.primaryThePatientIsTheInsuredsLabel.Text = "The Patient is the Insured’s:";
            // 
            // primaryNameLabel
            // 
            this.primaryNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryNameLabel.Location = new System.Drawing.Point(7, 2819);
            this.primaryNameLabel.Name = "primaryNameLabel";
            this.primaryNameLabel.Size = new System.Drawing.Size(39, 16);
            this.primaryNameLabel.TabIndex = 181;
            this.primaryNameLabel.Text = "Name:";
            // 
            // primaryBillingAddressValueLabel
            // 
            this.primaryBillingAddressValueLabel.AutoSize = true;
            this.primaryBillingAddressValueLabel.Location = new System.Drawing.Point(105, 2753);
            this.primaryBillingAddressValueLabel.Name = "primaryBillingAddressValueLabel";
            this.primaryBillingAddressValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryBillingAddressValueLabel.TabIndex = 179;
            // 
            // primaryBillingPhoneValueLabel
            // 
            this.primaryBillingPhoneValueLabel.AutoSize = true;
            this.primaryBillingPhoneValueLabel.Location = new System.Drawing.Point(95, 2727);
            this.primaryBillingPhoneValueLabel.Name = "primaryBillingPhoneValueLabel";
            this.primaryBillingPhoneValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryBillingPhoneValueLabel.TabIndex = 177;
            // 
            // primaryBillingNameValueLabel
            // 
            this.primaryBillingNameValueLabel.AutoSize = true;
            this.primaryBillingNameValueLabel.Location = new System.Drawing.Point(85, 2701);
            this.primaryBillingNameValueLabel.Name = "primaryBillingNameValueLabel";
            this.primaryBillingNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryBillingNameValueLabel.TabIndex = 175;
            // 
            // primaryBillingCoNameValueLabel
            // 
            this.primaryBillingCoNameValueLabel.AutoSize = true;
            this.primaryBillingCoNameValueLabel.Location = new System.Drawing.Point(111, 2675);
            this.primaryBillingCoNameValueLabel.Name = "primaryBillingCoNameValueLabel";
            this.primaryBillingCoNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryBillingCoNameValueLabel.TabIndex = 173;
            // 
            // primaryMedicalGroupOrIPAClinicValueLabel
            // 
            this.primaryMedicalGroupOrIPAClinicValueLabel.AutoSize = true;
            this.primaryMedicalGroupOrIPAClinicValueLabel.Location = new System.Drawing.Point(169, 2649);
            this.primaryMedicalGroupOrIPAClinicValueLabel.Name = "primaryMedicalGroupOrIPAClinicValueLabel";
            this.primaryMedicalGroupOrIPAClinicValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryMedicalGroupOrIPAClinicValueLabel.TabIndex = 171;
            // 
            // primaryMedicalGroupOrIpaNameValueLabel
            // 
            this.primaryMedicalGroupOrIpaNameValueLabel.AutoSize = true;
            this.primaryMedicalGroupOrIpaNameValueLabel.Location = new System.Drawing.Point(171, 2623);
            this.primaryMedicalGroupOrIpaNameValueLabel.Name = "primaryMedicalGroupOrIpaNameValueLabel";
            this.primaryMedicalGroupOrIpaNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryMedicalGroupOrIpaNameValueLabel.TabIndex = 169;
            // 
            // primaryAdjusterValueLabel
            // 
            this.primaryAdjusterValueLabel.AutoSize = true;
            this.primaryAdjusterValueLabel.Location = new System.Drawing.Point(64, 2597);
            this.primaryAdjusterValueLabel.Name = "primaryAdjusterValueLabel";
            this.primaryAdjusterValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryAdjusterValueLabel.TabIndex = 167;
            // 
            // primaryEmployeeSupervisorValueLabel
            // 
            this.primaryEmployeeSupervisorValueLabel.AutoSize = true;
            this.primaryEmployeeSupervisorValueLabel.Location = new System.Drawing.Point(146, 2571);
            this.primaryEmployeeSupervisorValueLabel.Name = "primaryEmployeeSupervisorValueLabel";
            this.primaryEmployeeSupervisorValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryEmployeeSupervisorValueLabel.TabIndex = 165;
            // 
            // primaryMedicaidIssueDateValueLabel
            // 
            this.primaryMedicaidIssueDateValueLabel.AutoSize = true;
            this.primaryMedicaidIssueDateValueLabel.Location = new System.Drawing.Point(132, 2545);
            this.primaryMedicaidIssueDateValueLabel.Name = "primaryMedicaidIssueDateValueLabel";
            this.primaryMedicaidIssueDateValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryMedicaidIssueDateValueLabel.TabIndex = 163;
            // 
            // primaryPrecertificationNumberValueLabel
            // 
            this.primaryPrecertificationNumberValueLabel.AutoSize = true;
            this.primaryPrecertificationNumberValueLabel.Location = new System.Drawing.Point(149, 2519);
            this.primaryPrecertificationNumberValueLabel.Name = "primaryPrecertificationNumberValueLabel";
            this.primaryPrecertificationNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryPrecertificationNumberValueLabel.TabIndex = 161;
            // 
            // primaryGroupNumberValueLabel
            // 
            this.primaryGroupNumberValueLabel.AutoSize = true;
            this.primaryGroupNumberValueLabel.Location = new System.Drawing.Point(96, 2493);
            this.primaryGroupNumberValueLabel.Name = "primaryGroupNumberValueLabel";
            this.primaryGroupNumberValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryGroupNumberValueLabel.TabIndex = 153;
            // // 
            //// primaryMBIValueLabel
            //// 
            //this.primaryMBIValueLabel.Location = new System.Drawing.Point(46, 2435);
            //this.primaryMBIValueLabel.Name = "primaryMBIValueLabel";
            //this.primaryMBIValueLabel.Size = new System.Drawing.Size(200, 16);
            //this.primaryMBIValueLabel.TabIndex = 150;
            //this.primaryMBIValueLabel.Visible = false;
            //// 
            //// secondaryMBIValueLabel
            //// 
            //this.secondaryMBIValueLabel.Location = new System.Drawing.Point(46, 2435);
            //this.secondaryMBIValueLabel.Name = "secondaryMBIValueLabel";
            //this.secondaryMBIValueLabel.Size = new System.Drawing.Size(200, 16);
            //this.secondaryMBIValueLabel.TabIndex = 150;
            //this.secondaryMBIValueLabel.Visible = false;
            // 
            // primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel
            // 
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location = new System.Drawing.Point(146, 2467);
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Name = "primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel";
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Size = new System.Drawing.Size(200, 16);
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.TabIndex = 151;
            // 
            // primaryInsuranceInsuredLabel
            // 
            this.primaryInsuranceInsuredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsuranceInsuredLabel.Location = new System.Drawing.Point(7, 2783);
            this.primaryInsuranceInsuredLabel.Name = "primaryInsuranceInsuredLabel";
            this.primaryInsuranceInsuredLabel.Size = new System.Drawing.Size(230, 20);
            this.primaryInsuranceInsuredLabel.TabIndex = 180;
            this.primaryInsuranceInsuredLabel.Text = "Primary Insurance – Insured";
            // 
            // primaryBillingAddressLabel
            // 
            this.primaryBillingAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBillingAddressLabel.Location = new System.Drawing.Point(7, 2753);
            this.primaryBillingAddressLabel.Name = "primaryBillingAddressLabel";
            this.primaryBillingAddressLabel.Size = new System.Drawing.Size(92, 16);
            this.primaryBillingAddressLabel.TabIndex = 178;
            this.primaryBillingAddressLabel.Text = "Billing address:";
            // 
            // primaryBillingPhoneLabel
            // 
            this.primaryBillingPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBillingPhoneLabel.Location = new System.Drawing.Point(7, 2727);
            this.primaryBillingPhoneLabel.Name = "primaryBillingPhoneLabel";
            this.primaryBillingPhoneLabel.Size = new System.Drawing.Size(82, 16);
            this.primaryBillingPhoneLabel.TabIndex = 176;
            this.primaryBillingPhoneLabel.Text = "Billing phone:";
            // 
            // primaryBillingNameLabel
            // 
            this.primaryBillingNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBillingNameLabel.Location = new System.Drawing.Point(7, 2701);
            this.primaryBillingNameLabel.Name = "primaryBillingNameLabel";
            this.primaryBillingNameLabel.Size = new System.Drawing.Size(72, 16);
            this.primaryBillingNameLabel.TabIndex = 174;
            this.primaryBillingNameLabel.Text = "Billing name:";
            // 
            // primaryBillingCoNameLabel
            // 
            this.primaryBillingCoNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryBillingCoNameLabel.Location = new System.Drawing.Point(7, 2675);
            this.primaryBillingCoNameLabel.Name = "primaryBillingCoNameLabel";
            this.primaryBillingCoNameLabel.Size = new System.Drawing.Size(98, 16);
            this.primaryBillingCoNameLabel.TabIndex = 172;
            this.primaryBillingCoNameLabel.Text = "Billing c/o name:";
            // 
            // primaryMedicalGroupOrIPAClinicLabel
            // 
            this.primaryMedicalGroupOrIPAClinicLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryMedicalGroupOrIPAClinicLabel.Location = new System.Drawing.Point(7, 2649);
            this.primaryMedicalGroupOrIPAClinicLabel.Name = "primaryMedicalGroupOrIPAClinicLabel";
            this.primaryMedicalGroupOrIPAClinicLabel.Size = new System.Drawing.Size(156, 16);
            this.primaryMedicalGroupOrIPAClinicLabel.TabIndex = 170;
            this.primaryMedicalGroupOrIPAClinicLabel.Text = "Medical group/IPA clinic:";
            // 
            // primaryMedicalGroupOrIpaNameLabel
            // 
            this.primaryMedicalGroupOrIpaNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryMedicalGroupOrIpaNameLabel.Location = new System.Drawing.Point(7, 2623);
            this.primaryMedicalGroupOrIpaNameLabel.Name = "primaryMedicalGroupOrIpaNameLabel";
            this.primaryMedicalGroupOrIpaNameLabel.Size = new System.Drawing.Size(158, 16);
            this.primaryMedicalGroupOrIpaNameLabel.TabIndex = 168;
            this.primaryMedicalGroupOrIpaNameLabel.Text = "Medical group/IPA name:";
            // 
            // primaryAdjusterLabel
            // 
            this.primaryAdjusterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryAdjusterLabel.Location = new System.Drawing.Point(7, 2597);
            this.primaryAdjusterLabel.Name = "primaryAdjusterLabel";
            this.primaryAdjusterLabel.Size = new System.Drawing.Size(51, 16);
            this.primaryAdjusterLabel.TabIndex = 166;
            this.primaryAdjusterLabel.Text = "Adjuster:";
            // 
            // primaryEmployeeSupervisorLabel
            // 
            this.primaryEmployeeSupervisorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryEmployeeSupervisorLabel.Location = new System.Drawing.Point(7, 2571);
            this.primaryEmployeeSupervisorLabel.Name = "primaryEmployeeSupervisorLabel";
            this.primaryEmployeeSupervisorLabel.Size = new System.Drawing.Size(133, 16);
            this.primaryEmployeeSupervisorLabel.TabIndex = 164;
            this.primaryEmployeeSupervisorLabel.Text = "Employee’s supervisor:";
            // 
            // primaryMedicaidIssueDateLabel
            // 
            this.primaryMedicaidIssueDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryMedicaidIssueDateLabel.Location = new System.Drawing.Point(7, 2545);
            this.primaryMedicaidIssueDateLabel.Name = "primaryMedicaidIssueDateLabel";
            this.primaryMedicaidIssueDateLabel.Size = new System.Drawing.Size(119, 16);
            this.primaryMedicaidIssueDateLabel.TabIndex = 162;
            this.primaryMedicaidIssueDateLabel.Text = "Medicaid issue date:";
            // 
            // primaryPrecertificationNumberLabel
            // 
            this.primaryPrecertificationNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryPrecertificationNumberLabel.Location = new System.Drawing.Point(7, 2519);
            this.primaryPrecertificationNumberLabel.Name = "primaryPrecertificationNumberLabel";
            this.primaryPrecertificationNumberLabel.Size = new System.Drawing.Size(136, 16);
            this.primaryPrecertificationNumberLabel.TabIndex = 160;
            this.primaryPrecertificationNumberLabel.Text = "Precertification number:";
            // 
            // primaryGroupNumberLabel
            // 
            this.primaryGroupNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryGroupNumberLabel.Location = new System.Drawing.Point(7, 2493);
            this.primaryGroupNumberLabel.Name = "primaryGroupNumberLabel";
            this.primaryGroupNumberLabel.Size = new System.Drawing.Size(83, 16);
            this.primaryGroupNumberLabel.TabIndex = 152;
            this.primaryGroupNumberLabel.Text = "Group number:";
            //// 
            //// MBI Number
            //// 
            //this.primaryMBILabel.BackColor = System.Drawing.Color.White;
            //this.primaryMBILabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.primaryMBILabel.Location = new System.Drawing.Point(7, 2467);
            //this.primaryMBILabel.Name = "primaryMBILabel";
            //this.primaryMBILabel.Size = new System.Drawing.Size(100, 16);
            //this.primaryMBILabel.TabIndex = 150;
            //this.primaryMBILabel.Visible = false;
            //this.primaryMBILabel.Text = "MBI: ";
            // 
            // primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel
            // 
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.BackColor = System.Drawing.Color.White;
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Location = new System.Drawing.Point(7, 2467);
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Name = "primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel";
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size = new System.Drawing.Size(132, 16);
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.TabIndex = 150;
            this.primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text = "CERT/SSN/ID number:";
            // 
            // primaryInsurancePayorDetailsLabel
            // 
            this.primaryInsurancePayorDetailsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsurancePayorDetailsLabel.Location = new System.Drawing.Point(7, 2431);
            this.primaryInsurancePayorDetailsLabel.Name = "primaryInsurancePayorDetailsLabel";
            this.primaryInsurancePayorDetailsLabel.Size = new System.Drawing.Size(278, 20);
            this.primaryInsurancePayorDetailsLabel.TabIndex = 149;
            this.primaryInsurancePayorDetailsLabel.Text = "Primary Insurance – Payor Details";
            // 
            // primaryCategoryValueLabel
            // 
            this.primaryCategoryValueLabel.AutoSize = true;
            this.primaryCategoryValueLabel.Location = new System.Drawing.Point(68, 2401);
            this.primaryCategoryValueLabel.Name = "primaryCategoryValueLabel";
            this.primaryCategoryValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryCategoryValueLabel.TabIndex = 148;
            // 
            // primaryPayorBrokerValueLabel
            // 
            this.primaryPayorBrokerValueLabel.AutoSize = true;
            this.primaryPayorBrokerValueLabel.Location = new System.Drawing.Point(96, 2375);
            this.primaryPayorBrokerValueLabel.Name = "primaryPayorBrokerValueLabel";
            this.primaryPayorBrokerValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryPayorBrokerValueLabel.TabIndex = 146;
            // 
            // primaryPlanValueLabel
            // 
            this.primaryPlanValueLabel.AutoSize = true;
            this.primaryPlanValueLabel.Location = new System.Drawing.Point(44, 2349);
            this.primaryPlanValueLabel.Name = "primaryPlanValueLabel";
            this.primaryPlanValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryPlanValueLabel.TabIndex = 144;
            // 
            // primaryCategoryLabel
            // 
            this.primaryCategoryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryCategoryLabel.Location = new System.Drawing.Point(7, 2401);
            this.primaryCategoryLabel.Name = "primaryCategoryLabel";
            this.primaryCategoryLabel.Size = new System.Drawing.Size(55, 16);
            this.primaryCategoryLabel.TabIndex = 147;
            this.primaryCategoryLabel.Text = "Category:";
            // 
            // primaryPayorBrokerLabel
            // 
            this.primaryPayorBrokerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryPayorBrokerLabel.Location = new System.Drawing.Point(7, 2375);
            this.primaryPayorBrokerLabel.Name = "primaryPayorBrokerLabel";
            this.primaryPayorBrokerLabel.Size = new System.Drawing.Size(83, 16);
            this.primaryPayorBrokerLabel.TabIndex = 145;
            this.primaryPayorBrokerLabel.Text = "Payor/Broker:";
            // 
            // primaryPlanLabel
            // 
            this.primaryPlanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryPlanLabel.Location = new System.Drawing.Point(7, 2349);
            this.primaryPlanLabel.Name = "primaryPlanLabel";
            this.primaryPlanLabel.Size = new System.Drawing.Size(31, 16);
            this.primaryPlanLabel.TabIndex = 143;
            this.primaryPlanLabel.Text = "Plan:";
            // 
            // primaryInsurancePlanLabel
            // 
            this.primaryInsurancePlanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryInsurancePlanLabel.Location = new System.Drawing.Point(7, 2313);
            this.primaryInsurancePlanLabel.Name = "primaryInsurancePlanLabel";
            this.primaryInsurancePlanLabel.Size = new System.Drawing.Size(207, 20);
            this.primaryInsurancePlanLabel.TabIndex = 142;
            this.primaryInsurancePlanLabel.Text = "Primary Insurance - Plan";
            // 
            // mspQuestionnaireSummaryConclusionLabel
            // 
            this.mspQuestionnaireSummaryConclusionLabel.AutoSize = true;
            this.mspQuestionnaireSummaryConclusionLabel.Location = new System.Drawing.Point(7, 2277);
            this.mspQuestionnaireSummaryConclusionLabel.Name = "mspQuestionnaireSummaryConclusionLabel";
            this.mspQuestionnaireSummaryConclusionLabel.Size = new System.Drawing.Size(0, 13);
            this.mspQuestionnaireSummaryConclusionLabel.TabIndex = 141;
            // 
            // mspLabel
            // 
            this.mspLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mspLabel.Location = new System.Drawing.Point(7, 2251);
            this.mspLabel.Name = "mspLabel";
            this.mspLabel.Size = new System.Drawing.Size(45, 20);
            this.mspLabel.TabIndex = 140;
            this.mspLabel.Text = "MSP";
            // 
            // fatherDobValueLabel
            // 
            this.fatherDobValueLabel.AutoSize = true;
            this.fatherDobValueLabel.Location = new System.Drawing.Point(97, 2221);
            this.fatherDobValueLabel.Name = "fatherDobValueLabel";
            this.fatherDobValueLabel.Size = new System.Drawing.Size(0, 13);
            this.fatherDobValueLabel.TabIndex = 139;
            // 
            // motherDobValueLabel
            // 
            this.motherDobValueLabel.AutoSize = true;
            this.motherDobValueLabel.Location = new System.Drawing.Point(97, 2195);
            this.motherDobValueLabel.Name = "motherDobValueLabel";
            this.motherDobValueLabel.Size = new System.Drawing.Size(0, 13);
            this.motherDobValueLabel.TabIndex = 137;
            // 
            // financialClassValueLabel
            // 
            this.financialClassValueLabel.AutoSize = true;
            this.financialClassValueLabel.Location = new System.Drawing.Point(97, 2170);
            this.financialClassValueLabel.Name = "financialClassValueLabel";
            this.financialClassValueLabel.Size = new System.Drawing.Size(0, 13);
            this.financialClassValueLabel.TabIndex = 135;
            // 
            // fatherDobLabel
            // 
            this.fatherDobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fatherDobLabel.Location = new System.Drawing.Point(8, 2221);
            this.fatherDobLabel.Name = "fatherDobLabel";
            this.fatherDobLabel.Size = new System.Drawing.Size(87, 16);
            this.fatherDobLabel.TabIndex = 138;
            this.fatherDobLabel.Text = "Father’s DOB:";
            // 
            // motherDobLabel
            // 
            this.motherDobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.motherDobLabel.Location = new System.Drawing.Point(7, 2195);
            this.motherDobLabel.Name = "motherDobLabel";
            this.motherDobLabel.Size = new System.Drawing.Size(88, 16);
            this.motherDobLabel.TabIndex = 136;
            this.motherDobLabel.Text = "Mother’s DOB:";
            // 
            // financialClassLabel
            // 
            this.financialClassLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.financialClassLabel.Location = new System.Drawing.Point(7, 2170);
            this.financialClassLabel.Name = "financialClassLabel";
            this.financialClassLabel.Size = new System.Drawing.Size(92, 16);
            this.financialClassLabel.TabIndex = 134;
            this.financialClassLabel.Text = "Financial class:";
            // 
            // generalInsuranceInformationLabel
            // 
            this.generalInsuranceInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generalInsuranceInformationLabel.Location = new System.Drawing.Point(7, 2138);
            this.generalInsuranceInformationLabel.Name = "generalInsuranceInformationLabel";
            this.generalInsuranceInformationLabel.Size = new System.Drawing.Size(252, 20);
            this.generalInsuranceInformationLabel.TabIndex = 133;
            this.generalInsuranceInformationLabel.Text = "General Insurance Information";
            // 
            // commentsValueLabel
            // 
            this.commentsValueLabel.Location = new System.Drawing.Point(98, 1747);
            this.commentsValueLabel.Name = "commentsValueLabel";
            this.commentsValueLabel.Size = new System.Drawing.Size(520, 28);
            this.commentsValueLabel.TabIndex = 132;
            // 
            // ResistantOrganismValueLabel
            // 
            this.ResistantOrganismValueLabel.AutoSize = true;
            this.ResistantOrganismValueLabel.Location = new System.Drawing.Point(130, 1726);
            this.ResistantOrganismValueLabel.Name = "ResistantOrganismValueLabel";
            this.ResistantOrganismValueLabel.Size = new System.Drawing.Size(0, 13);
            this.ResistantOrganismValueLabel.TabIndex = 130;
            // 
            // pregnantValueLabel
            // 
            this.pregnantValueLabel.AutoSize = true;
            this.pregnantValueLabel.Location = new System.Drawing.Point(98, 1705);
            this.pregnantValueLabel.Name = "pregnantValueLabel";
            this.pregnantValueLabel.Size = new System.Drawing.Size(0, 13);
            this.pregnantValueLabel.TabIndex = 128;
            // 
            // bloodlessValueLabel
            // 
            this.bloodlessValueLabel.AutoSize = true;
            this.bloodlessValueLabel.Location = new System.Drawing.Point(101, 1684);
            this.bloodlessValueLabel.Name = "bloodlessValueLabel";
            this.bloodlessValueLabel.Size = new System.Drawing.Size(0, 13);
            this.bloodlessValueLabel.TabIndex = 126;
            // 
            // smokerValueLabel
            // 
            this.smokerValueLabel.AutoSize = true;
            this.smokerValueLabel.Location = new System.Drawing.Point(98, 1663);
            this.smokerValueLabel.Name = "smokerValueLabel";
            this.smokerValueLabel.Size = new System.Drawing.Size(0, 13);
            this.smokerValueLabel.TabIndex = 124;
            // 
            // primaryCarePhysicianValueLabel
            // 
            this.primaryCarePhysicianValueLabel.AutoSize = true;
            this.primaryCarePhysicianValueLabel.Location = new System.Drawing.Point(147, 1642);
            this.primaryCarePhysicianValueLabel.Name = "primaryCarePhysicianValueLabel";
            this.primaryCarePhysicianValueLabel.Size = new System.Drawing.Size(0, 13);
            this.primaryCarePhysicianValueLabel.TabIndex = 122;
            // 
            // operatingPhysicianValueLabel
            // 
            this.operatingPhysicianValueLabel.AutoSize = true;
            this.operatingPhysicianValueLabel.Location = new System.Drawing.Point(130, 1621);
            this.operatingPhysicianValueLabel.Name = "operatingPhysicianValueLabel";
            this.operatingPhysicianValueLabel.Size = new System.Drawing.Size(0, 13);
            this.operatingPhysicianValueLabel.TabIndex = 120;
            // 
            // commentsLabel
            // 
            this.commentsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commentsLabel.Location = new System.Drawing.Point(8, 1747);
            this.commentsLabel.Name = "commentsLabel";
            this.commentsLabel.Size = new System.Drawing.Size(64, 16);
            this.commentsLabel.TabIndex = 131;
            this.commentsLabel.Text = "Comments:";
            // 
            // ResistantOrganismLabel
            // 
            this.ResistantOrganismLabel.AutoSize = true;
            this.ResistantOrganismLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResistantOrganismLabel.Location = new System.Drawing.Point(8, 1726);
            this.ResistantOrganismLabel.Name = "ResistantOrganismLabel";
            this.ResistantOrganismLabel.Size = new System.Drawing.Size(120, 13);
            this.ResistantOrganismLabel.TabIndex = 129;
            this.ResistantOrganismLabel.Text = "Resistant Organism:";
            // 
            // pregnantLabel
            // 
            this.pregnantLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pregnantLabel.Location = new System.Drawing.Point(8, 1705);
            this.pregnantLabel.Name = "pregnantLabel";
            this.pregnantLabel.Size = new System.Drawing.Size(64, 13);
            this.pregnantLabel.TabIndex = 127;
            this.pregnantLabel.Text = "Pregnant:";
            // 
            // bloodlessLabel
            // 
            this.bloodlessLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bloodlessLabel.Location = new System.Drawing.Point(8, 1684);
            this.bloodlessLabel.Name = "bloodlessLabel";
            this.bloodlessLabel.Size = new System.Drawing.Size(59, 16);
            this.bloodlessLabel.TabIndex = 125;
            this.bloodlessLabel.Text = "Bloodless:";
            // 
            // smokerLabel
            // 
            this.smokerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.smokerLabel.Location = new System.Drawing.Point(8, 1663);
            this.smokerLabel.Name = "smokerLabel";
            this.smokerLabel.Size = new System.Drawing.Size(48, 16);
            this.smokerLabel.TabIndex = 123;
            this.smokerLabel.Text = "Smoker:";
            // 
            // primaryCarePhysicianLabel
            // 
            this.primaryCarePhysicianLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.primaryCarePhysicianLabel.Location = new System.Drawing.Point(8, 1642);
            this.primaryCarePhysicianLabel.Name = "primaryCarePhysicianLabel";
            this.primaryCarePhysicianLabel.Size = new System.Drawing.Size(133, 21);
            this.primaryCarePhysicianLabel.TabIndex = 121;
            this.primaryCarePhysicianLabel.Text = "Primary Care physician:";
            // 
            // operatingPhysicianLabel
            // 
            this.operatingPhysicianLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.operatingPhysicianLabel.Location = new System.Drawing.Point(8, 1621);
            this.operatingPhysicianLabel.Name = "operatingPhysicianLabel";
            this.operatingPhysicianLabel.Size = new System.Drawing.Size(119, 16);
            this.operatingPhysicianLabel.TabIndex = 119;
            this.operatingPhysicianLabel.Text = "Operating physician:";
            // 
            // clinicalLabel
            // 
            this.clinicalLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinicalLabel.Location = new System.Drawing.Point(7, 1529);
            this.clinicalLabel.Name = "clinicalLabel";
            this.clinicalLabel.Size = new System.Drawing.Size(66, 20);
            this.clinicalLabel.TabIndex = 112;
            this.clinicalLabel.Text = "Clinical";
            // 
            // attendingPhysicianValueLabel
            // 
            this.attendingPhysicianValueLabel.AutoSize = true;
            this.attendingPhysicianValueLabel.Location = new System.Drawing.Point(130, 1600);
            this.attendingPhysicianValueLabel.Name = "attendingPhysicianValueLabel";
            this.attendingPhysicianValueLabel.Size = new System.Drawing.Size(0, 13);
            this.attendingPhysicianValueLabel.TabIndex = 118;
            // 
            // admittingPhysicianValueLabel
            // 
            this.admittingPhysicianValueLabel.AutoSize = true;
            this.admittingPhysicianValueLabel.Location = new System.Drawing.Point(130, 1579);
            this.admittingPhysicianValueLabel.Name = "admittingPhysicianValueLabel";
            this.admittingPhysicianValueLabel.Size = new System.Drawing.Size(0, 13);
            this.admittingPhysicianValueLabel.TabIndex = 116;
            // 
            // referringPhysicianValueLabel
            // 
            this.referringPhysicianValueLabel.AutoSize = true;
            this.referringPhysicianValueLabel.Location = new System.Drawing.Point(130, 1558);
            this.referringPhysicianValueLabel.Name = "referringPhysicianValueLabel";
            this.referringPhysicianValueLabel.Size = new System.Drawing.Size(0, 13);
            this.referringPhysicianValueLabel.TabIndex = 114;
            // 
            // attendingPhysicianLabel
            // 
            this.attendingPhysicianLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.attendingPhysicianLabel.Location = new System.Drawing.Point(8, 1600);
            this.attendingPhysicianLabel.Name = "attendingPhysicianLabel";
            this.attendingPhysicianLabel.Size = new System.Drawing.Size(116, 16);
            this.attendingPhysicianLabel.TabIndex = 117;
            this.attendingPhysicianLabel.Text = "Attending physician:";
            // 
            // admittingPhysicianLabel
            // 
            this.admittingPhysicianLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.admittingPhysicianLabel.Location = new System.Drawing.Point(8, 1579);
            this.admittingPhysicianLabel.Name = "admittingPhysicianLabel";
            this.admittingPhysicianLabel.Size = new System.Drawing.Size(116, 16);
            this.admittingPhysicianLabel.TabIndex = 115;
            this.admittingPhysicianLabel.Text = "Admitting physician:";
            // 
            // referringPhysicianLabel
            // 
            this.referringPhysicianLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.referringPhysicianLabel.Location = new System.Drawing.Point(8, 1558);
            this.referringPhysicianLabel.Name = "referringPhysicianLabel";
            this.referringPhysicianLabel.Size = new System.Drawing.Size(116, 16);
            this.referringPhysicianLabel.TabIndex = 113;
            this.referringPhysicianLabel.Text = "Referring physician:";
            // 
            // clinic5ValueLabel
            // 
            this.clinic5ValueLabel.AutoSize = true;
            this.clinic5ValueLabel.Location = new System.Drawing.Point(60, 1503);
            this.clinic5ValueLabel.Name = "clinic5ValueLabel";
            this.clinic5ValueLabel.Size = new System.Drawing.Size(0, 13);
            this.clinic5ValueLabel.TabIndex = 111;
            // 
            // clinic4ValueLabel
            // 
            this.clinic4ValueLabel.AutoSize = true;
            this.clinic4ValueLabel.Location = new System.Drawing.Point(60, 1480);
            this.clinic4ValueLabel.Name = "clinic4ValueLabel";
            this.clinic4ValueLabel.Size = new System.Drawing.Size(0, 13);
            this.clinic4ValueLabel.TabIndex = 109;
            // 
            // clinic3ValueLabel
            // 
            this.clinic3ValueLabel.AutoSize = true;
            this.clinic3ValueLabel.Location = new System.Drawing.Point(60, 1457);
            this.clinic3ValueLabel.Name = "clinic3ValueLabel";
            this.clinic3ValueLabel.Size = new System.Drawing.Size(0, 13);
            this.clinic3ValueLabel.TabIndex = 107;
            // 
            // clinic2ValueLabel
            // 
            this.clinic2ValueLabel.AutoSize = true;
            this.clinic2ValueLabel.Location = new System.Drawing.Point(60, 1434);
            this.clinic2ValueLabel.Name = "clinic2ValueLabel";
            this.clinic2ValueLabel.Size = new System.Drawing.Size(0, 13);
            this.clinic2ValueLabel.TabIndex = 105;
            // 
            // clinic1ValueLabel
            // 
            this.clinic1ValueLabel.AutoSize = true;
            this.clinic1ValueLabel.Location = new System.Drawing.Point(60, 1411);
            this.clinic1ValueLabel.Name = "clinic1ValueLabel";
            this.clinic1ValueLabel.Size = new System.Drawing.Size(0, 13);
            this.clinic1ValueLabel.TabIndex = 103;
            // 
            // referralSourceValueLabel
            // 
            this.referralSourceValueLabel.AutoSize = true;
            this.referralSourceValueLabel.Location = new System.Drawing.Point(109, 1369);
            this.referralSourceValueLabel.Name = "referralSourceValueLabel";
            this.referralSourceValueLabel.Size = new System.Drawing.Size(0, 13);
            this.referralSourceValueLabel.TabIndex = 101;
            // 
            // clinic5Label
            // 
            this.clinic5Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic5Label.Location = new System.Drawing.Point(7, 1503);
            this.clinic5Label.Name = "clinic5Label";
            this.clinic5Label.Size = new System.Drawing.Size(43, 16);
            this.clinic5Label.TabIndex = 110;
            this.clinic5Label.Text = "Clinic5:";
            // 
            // clinic4Label
            // 
            this.clinic4Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic4Label.Location = new System.Drawing.Point(7, 1480);
            this.clinic4Label.Name = "clinic4Label";
            this.clinic4Label.Size = new System.Drawing.Size(43, 16);
            this.clinic4Label.TabIndex = 108;
            this.clinic4Label.Text = "Clinic4:";
            // 
            // clinic3Label
            // 
            this.clinic3Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic3Label.Location = new System.Drawing.Point(7, 1457);
            this.clinic3Label.Name = "clinic3Label";
            this.clinic3Label.Size = new System.Drawing.Size(43, 16);
            this.clinic3Label.TabIndex = 106;
            this.clinic3Label.Text = "Clinic3:";
            // 
            // modeOfArrivalValueLabel
            // 
            this.modeOfArrivalValueLabel.AutoSize = true;
            this.modeOfArrivalValueLabel.Location = new System.Drawing.Point(107, 1346);
            this.modeOfArrivalValueLabel.Name = "modeOfArrivalValueLabel";
            this.modeOfArrivalValueLabel.Size = new System.Drawing.Size(0, 13);
            this.modeOfArrivalValueLabel.TabIndex = 99;
            // 
            // admitSourceValueLabel
            // 
            this.admitSourceValueLabel.AutoSize = true;
            this.admitSourceValueLabel.Location = new System.Drawing.Point(96, 1300);
            this.admitSourceValueLabel.Name = "admitSourceValueLabel";
            this.admitSourceValueLabel.Size = new System.Drawing.Size(0, 13);
            this.admitSourceValueLabel.TabIndex = 97;
            // 
            // tenetCareValueLabel
            // 
            this.tenetCareValueLabel.AutoSize = true;
            this.tenetCareValueLabel.Location = new System.Drawing.Point(79, 1277);
            this.tenetCareValueLabel.Name = "tenetCareValueLabel";
            this.tenetCareValueLabel.Size = new System.Drawing.Size(0, 13);
            this.tenetCareValueLabel.TabIndex = 95;
            // 
            // dateOfOnsetForSymptomsIllnessValueLabel
            // 
            this.dateOfOnsetForSymptomsIllnessValueLabel.AutoSize = true;
            this.dateOfOnsetForSymptomsIllnessValueLabel.Location = new System.Drawing.Point(228, 1254);
            this.dateOfOnsetForSymptomsIllnessValueLabel.Name = "dateOfOnsetForSymptomsIllnessValueLabel";
            this.dateOfOnsetForSymptomsIllnessValueLabel.Size = new System.Drawing.Size(0, 13);
            this.dateOfOnsetForSymptomsIllnessValueLabel.TabIndex = 93;
            // 
            // accidentCrimeStateProvinceValueLabel
            // 
            this.accidentCrimeStateProvinceValueLabel.AutoSize = true;
            this.accidentCrimeStateProvinceValueLabel.Location = new System.Drawing.Point(208, 1229);
            this.accidentCrimeStateProvinceValueLabel.Name = "accidentCrimeStateProvinceValueLabel";
            this.accidentCrimeStateProvinceValueLabel.Size = new System.Drawing.Size(0, 13);
            this.accidentCrimeStateProvinceValueLabel.TabIndex = 91;
            // 
            // accidentCrimeCountryValueLabel
            // 
            this.accidentCrimeCountryValueLabel.AutoSize = true;
            this.accidentCrimeCountryValueLabel.Location = new System.Drawing.Point(164, 1205);
            this.accidentCrimeCountryValueLabel.Name = "accidentCrimeCountryValueLabel";
            this.accidentCrimeCountryValueLabel.Size = new System.Drawing.Size(0, 13);
            this.accidentCrimeCountryValueLabel.TabIndex = 89;
            // 
            // accidentCrimeHourValueLabel
            // 
            this.accidentCrimeHourValueLabel.AutoSize = true;
            this.accidentCrimeHourValueLabel.Location = new System.Drawing.Point(145, 1181);
            this.accidentCrimeHourValueLabel.Name = "accidentCrimeHourValueLabel";
            this.accidentCrimeHourValueLabel.Size = new System.Drawing.Size(0, 13);
            this.accidentCrimeHourValueLabel.TabIndex = 87;
            // 
            // clinic2Label
            // 
            this.clinic2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic2Label.Location = new System.Drawing.Point(7, 1434);
            this.clinic2Label.Name = "clinic2Label";
            this.clinic2Label.Size = new System.Drawing.Size(43, 16);
            this.clinic2Label.TabIndex = 104;
            this.clinic2Label.Text = "Clinic2:";
            // 
            // clinic1Label
            // 
            this.clinic1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clinic1Label.Location = new System.Drawing.Point(7, 1411);
            this.clinic1Label.Name = "clinic1Label";
            this.clinic1Label.Size = new System.Drawing.Size(47, 16);
            this.clinic1Label.TabIndex = 102;
            this.clinic1Label.Text = "Clinic1:";
            // 
            // referralSourceLabel
            // 
            this.referralSourceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.referralSourceLabel.Location = new System.Drawing.Point(7, 1368);
            this.referralSourceLabel.Name = "referralSourceLabel";
            this.referralSourceLabel.Size = new System.Drawing.Size(98, 16);
            this.referralSourceLabel.TabIndex = 100;
            this.referralSourceLabel.Text = "Referral source:";
            // 
            // modeOfArrivalLabel
            // 
            this.modeOfArrivalLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modeOfArrivalLabel.Location = new System.Drawing.Point(7, 1345);
            this.modeOfArrivalLabel.Name = "modeOfArrivalLabel";
            this.modeOfArrivalLabel.Size = new System.Drawing.Size(96, 16);
            this.modeOfArrivalLabel.TabIndex = 98;
            this.modeOfArrivalLabel.Text = "Mode of arrival:";
            // 
            // admitSourceLabel
            // 
            this.admitSourceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.admitSourceLabel.Location = new System.Drawing.Point(7, 1300);
            this.admitSourceLabel.Name = "admitSourceLabel";
            this.admitSourceLabel.Size = new System.Drawing.Size(85, 16);
            this.admitSourceLabel.TabIndex = 96;
            this.admitSourceLabel.Text = "Admit source:";
            // 
            // tenetCareLabel
            // 
            this.tenetCareLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tenetCareLabel.Location = new System.Drawing.Point(7, 1277);
            this.tenetCareLabel.Name = "tenetCareLabel";
            this.tenetCareLabel.Size = new System.Drawing.Size(68, 16);
            this.tenetCareLabel.TabIndex = 94;
            this.tenetCareLabel.Text = "TenetCare:";
            // 
            // dateOfOnsetForSymptomsIllnessLabel
            // 
            this.dateOfOnsetForSymptomsIllnessLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateOfOnsetForSymptomsIllnessLabel.Location = new System.Drawing.Point(7, 1254);
            this.dateOfOnsetForSymptomsIllnessLabel.Name = "dateOfOnsetForSymptomsIllnessLabel";
            this.dateOfOnsetForSymptomsIllnessLabel.Size = new System.Drawing.Size(215, 16);
            this.dateOfOnsetForSymptomsIllnessLabel.TabIndex = 92;
            this.dateOfOnsetForSymptomsIllnessLabel.Text = "Date of onset for symptoms or illness:";
            // 
            // accidentCrimeStateProvinceLabel
            // 
            this.accidentCrimeStateProvinceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentCrimeStateProvinceLabel.Location = new System.Drawing.Point(7, 1229);
            this.accidentCrimeStateProvinceLabel.Name = "accidentCrimeStateProvinceLabel";
            this.accidentCrimeStateProvinceLabel.Size = new System.Drawing.Size(195, 16);
            this.accidentCrimeStateProvinceLabel.TabIndex = 90;
            this.accidentCrimeStateProvinceLabel.Text = "Accident or crime state/province:";
            // 
            // accidentCrimeCountryLabel
            // 
            this.accidentCrimeCountryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentCrimeCountryLabel.Location = new System.Drawing.Point(7, 1205);
            this.accidentCrimeCountryLabel.Name = "accidentCrimeCountryLabel";
            this.accidentCrimeCountryLabel.Size = new System.Drawing.Size(151, 16);
            this.accidentCrimeCountryLabel.TabIndex = 88;
            this.accidentCrimeCountryLabel.Text = "Accident or crime country:";
            // 
            // accidentCrimeHourLabel
            // 
            this.accidentCrimeHourLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentCrimeHourLabel.Location = new System.Drawing.Point(7, 1181);
            this.accidentCrimeHourLabel.Name = "accidentCrimeHourLabel";
            this.accidentCrimeHourLabel.Size = new System.Drawing.Size(132, 16);
            this.accidentCrimeHourLabel.TabIndex = 86;
            this.accidentCrimeHourLabel.Text = "Accident or crime hour:";
            // 
            // accidentCrimeDateValueLabel
            // 
            this.accidentCrimeDateValueLabel.AutoSize = true;
            this.accidentCrimeDateValueLabel.Location = new System.Drawing.Point(145, 1157);
            this.accidentCrimeDateValueLabel.Name = "accidentCrimeDateValueLabel";
            this.accidentCrimeDateValueLabel.Size = new System.Drawing.Size(0, 13);
            this.accidentCrimeDateValueLabel.TabIndex = 85;
            // 
            // accidentCrimeDateLabel
            // 
            this.accidentCrimeDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentCrimeDateLabel.Location = new System.Drawing.Point(7, 1157);
            this.accidentCrimeDateLabel.Name = "accidentCrimeDateLabel";
            this.accidentCrimeDateLabel.Size = new System.Drawing.Size(132, 16);
            this.accidentCrimeDateLabel.TabIndex = 84;
            this.accidentCrimeDateLabel.Text = "Accident or crime date:";
            // 
            // accidentTypeValueLabel
            // 
            this.accidentTypeValueLabel.AutoSize = true;
            this.accidentTypeValueLabel.Location = new System.Drawing.Point(115, 1133);
            this.accidentTypeValueLabel.Name = "accidentTypeValueLabel";
            this.accidentTypeValueLabel.Size = new System.Drawing.Size(10, 13);
            this.accidentTypeValueLabel.TabIndex = 83;
            this.accidentTypeValueLabel.Text = " ";
            // 
            // accidentTypeLabel
            // 
            this.accidentTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accidentTypeLabel.Location = new System.Drawing.Point(7, 1133);
            this.accidentTypeLabel.Name = "accidentTypeLabel";
            this.accidentTypeLabel.Size = new System.Drawing.Size(83, 16);
            this.accidentTypeLabel.TabIndex = 82;
            this.accidentTypeLabel.Text = "Accident type:";
            // 
            // patientVisitValueLabel
            // 
            this.patientVisitValueLabel.AutoSize = true;
            this.patientVisitValueLabel.Location = new System.Drawing.Point(221, 1109);
            this.patientVisitValueLabel.Name = "patientVisitValueLabel";
            this.patientVisitValueLabel.Size = new System.Drawing.Size(0, 13);
            this.patientVisitValueLabel.TabIndex = 81;
            // 
            // patientVisitLabel
            // 
            this.patientVisitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientVisitLabel.Location = new System.Drawing.Point(7, 1109);
            this.patientVisitLabel.Name = "patientVisitLabel";
            this.patientVisitLabel.Size = new System.Drawing.Size(193, 16);
            this.patientVisitLabel.TabIndex = 80;
            this.patientVisitLabel.Text = "The patient\'s visit is the result of:";
            // 
            // chiefComplaintValueLabel
            // 
            this.chiefComplaintValueLabel.AutoSize = true;
            this.chiefComplaintValueLabel.Location = new System.Drawing.Point(129, 1044);
            this.chiefComplaintValueLabel.Name = "chiefComplaintValueLabel";
            this.chiefComplaintValueLabel.Size = new System.Drawing.Size(10, 13);
            this.chiefComplaintValueLabel.TabIndex = 79;
            this.chiefComplaintValueLabel.Text = " ";
            // 
            // chiefComplaintLabel
            // 
            this.chiefComplaintLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chiefComplaintLabel.Location = new System.Drawing.Point(7, 1044);
            this.chiefComplaintLabel.Name = "chiefComplaintLabel";
            this.chiefComplaintLabel.Size = new System.Drawing.Size(94, 16);
            this.chiefComplaintLabel.TabIndex = 78;
            this.chiefComplaintLabel.Text = "Chief complaint:";
            // 
            // accomodationcodeValueLabelL
            // 
            this.accomodationcodeValueLabelL.AutoSize = true;
            this.accomodationcodeValueLabelL.Location = new System.Drawing.Point(164, 1022);
            this.accomodationcodeValueLabelL.Name = "accomodationcodeValueLabelL";
            this.accomodationcodeValueLabelL.Size = new System.Drawing.Size(0, 13);
            this.accomodationcodeValueLabelL.TabIndex = 77;
            // 
            // accomodationcodeLabelL
            // 
            this.accomodationcodeLabelL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accomodationcodeLabelL.Location = new System.Drawing.Point(7, 1022);
            this.accomodationcodeLabelL.Name = "accomodationcodeLabelL";
            this.accomodationcodeLabelL.Size = new System.Drawing.Size(128, 16);
            this.accomodationcodeLabelL.TabIndex = 76;
            this.accomodationcodeLabelL.Text = "Accommodation code:";
            // 
            // assignedBedValueLabel
            // 
            this.assignedBedValueLabel.AutoSize = true;
            this.assignedBedValueLabel.Location = new System.Drawing.Point(125, 998);
            this.assignedBedValueLabel.Name = "assignedBedValueLabel";
            this.assignedBedValueLabel.Size = new System.Drawing.Size(10, 13);
            this.assignedBedValueLabel.TabIndex = 75;
            this.assignedBedValueLabel.Text = " ";
            // 
            // assignedBedLabel
            // 
            this.assignedBedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.assignedBedLabel.Location = new System.Drawing.Point(7, 998);
            this.assignedBedLabel.Name = "assignedBedLabel";
            this.assignedBedLabel.Size = new System.Drawing.Size(85, 16);
            this.assignedBedLabel.TabIndex = 74;
            this.assignedBedLabel.Text = "Assigned Bed:";
            // 
            // hospitalServiceValueLabel
            // 
            this.hospitalServiceValueLabel.AutoSize = true;
            this.hospitalServiceValueLabel.Location = new System.Drawing.Point(125, 974);
            this.hospitalServiceValueLabel.Name = "hospitalServiceValueLabel";
            this.hospitalServiceValueLabel.Size = new System.Drawing.Size(10, 13);
            this.hospitalServiceValueLabel.TabIndex = 73;
            this.hospitalServiceValueLabel.Text = " ";
            // 
            // hospitalServiceLabel
            // 
            this.hospitalServiceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hospitalServiceLabel.Location = new System.Drawing.Point(7, 974);
            this.hospitalServiceLabel.Name = "hospitalServiceLabel";
            this.hospitalServiceLabel.Size = new System.Drawing.Size(96, 16);
            this.hospitalServiceLabel.TabIndex = 72;
            this.hospitalServiceLabel.Text = "Hospital service:";
            // 
            // reregisterValueLabel
            // 
            this.reregisterValueLabel.AutoSize = true;
            this.reregisterValueLabel.Location = new System.Drawing.Point(108, 950);
            this.reregisterValueLabel.Name = "reregisterValueLabel";
            this.reregisterValueLabel.Size = new System.Drawing.Size(10, 13);
            this.reregisterValueLabel.TabIndex = 71;
            this.reregisterValueLabel.Text = " ";
            // 
            // reregisterLabel
            // 
            this.reregisterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reregisterLabel.Location = new System.Drawing.Point(7, 950);
            this.reregisterLabel.Name = "reregisterLabel";
            this.reregisterLabel.Size = new System.Drawing.Size(62, 16);
            this.reregisterLabel.TabIndex = 70;
            this.reregisterLabel.Text = "Reregister:";
            // 
            // patientTypeValueLabel
            // 
            this.patientTypeValueLabel.AutoSize = true;
            this.patientTypeValueLabel.Location = new System.Drawing.Point(111, 926);
            this.patientTypeValueLabel.Name = "patientTypeValueLabel";
            this.patientTypeValueLabel.Size = new System.Drawing.Size(10, 13);
            this.patientTypeValueLabel.TabIndex = 69;
            this.patientTypeValueLabel.Text = " ";
            // 
            // patientTypeLabel
            // 
            this.patientTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientTypeLabel.Location = new System.Drawing.Point(7, 926);
            this.patientTypeLabel.Name = "patientTypeLabel";
            this.patientTypeLabel.Size = new System.Drawing.Size(79, 16);
            this.patientTypeLabel.TabIndex = 68;
            this.patientTypeLabel.Text = "Patient Type:";
            // 
            // diagnosisLabel
            // 
            this.diagnosisLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.diagnosisLabel.Location = new System.Drawing.Point(5, 878);
            this.diagnosisLabel.Name = "diagnosisLabel";
            this.diagnosisLabel.Size = new System.Drawing.Size(89, 20);
            this.diagnosisLabel.TabIndex = 67;
            this.diagnosisLabel.Text = "Diagnosis";
            // 
            // employeeIDValueLabel
            // 
            this.employeeIDValueLabel.AutoSize = true;
            this.employeeIDValueLabel.Location = new System.Drawing.Point(98, 846);
            this.employeeIDValueLabel.Name = "employeeIDValueLabel";
            this.employeeIDValueLabel.Size = new System.Drawing.Size(10, 13);
            this.employeeIDValueLabel.TabIndex = 66;
            this.employeeIDValueLabel.Text = " ";
            // 
            // employeeIDlabel
            // 
            this.employeeIDlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.employeeIDlabel.Location = new System.Drawing.Point(9, 846);
            this.employeeIDlabel.Name = "employeeIDlabel";
            this.employeeIDlabel.Size = new System.Drawing.Size(92, 16);
            this.employeeIDlabel.TabIndex = 65;
            this.employeeIDlabel.Text = "Employee ID:";
            // 
            // occIndustryValueLabel
            // 
            this.occIndustryValueLabel.AutoSize = true;
            this.occIndustryValueLabel.Location = new System.Drawing.Point(118, 806);
            this.occIndustryValueLabel.Name = "occIndustryValueLabel";
            this.occIndustryValueLabel.Size = new System.Drawing.Size(10, 13);
            this.occIndustryValueLabel.TabIndex = 64;
            this.occIndustryValueLabel.Text = " ";
            // 
            // occIndustryLabel
            // 
            this.occIndustryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occIndustryLabel.Location = new System.Drawing.Point(9, 806);
            this.occIndustryLabel.Name = "occIndustryLabel";
            this.occIndustryLabel.Size = new System.Drawing.Size(91, 16);
            this.occIndustryLabel.TabIndex = 63;
            this.occIndustryLabel.Text = "Occ/Industry:";
            // 
            // employerPhoneValueLabel
            // 
            this.employerPhoneValueLabel.AutoSize = true;
            this.employerPhoneValueLabel.Location = new System.Drawing.Point(128, 771);
            this.employerPhoneValueLabel.Name = "employerPhoneValueLabel";
            this.employerPhoneValueLabel.Size = new System.Drawing.Size(10, 13);
            this.employerPhoneValueLabel.TabIndex = 62;
            this.employerPhoneValueLabel.Text = " ";
            // 
            // employerPhoneLabel
            // 
            this.employerPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.employerPhoneLabel.Location = new System.Drawing.Point(9, 771);
            this.employerPhoneLabel.Name = "employerPhoneLabel";
            this.employerPhoneLabel.Size = new System.Drawing.Size(107, 13);
            this.employerPhoneLabel.TabIndex = 61;
            this.employerPhoneLabel.Text = "Employer phone:";
            // 
            // employerValueLabel
            // 
            this.employerValueLabel.AutoSize = true;
            this.employerValueLabel.Location = new System.Drawing.Point(98, 736);
            this.employerValueLabel.Name = "employerValueLabel";
            this.employerValueLabel.Size = new System.Drawing.Size(10, 13);
            this.employerValueLabel.TabIndex = 60;
            this.employerValueLabel.Text = " ";
            // 
            // employerlabel
            // 
            this.employerlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.employerlabel.Location = new System.Drawing.Point(9, 736);
            this.employerlabel.Name = "employerlabel";
            this.employerlabel.Size = new System.Drawing.Size(69, 16);
            this.employerlabel.TabIndex = 59;
            this.employerlabel.Text = "Employer:";
            // 
            // employmentStatusValueLabel
            // 
            this.employmentStatusValueLabel.AutoSize = true;
            this.employmentStatusValueLabel.Location = new System.Drawing.Point(108, 701);
            this.employmentStatusValueLabel.Name = "employmentStatusValueLabel";
            this.employmentStatusValueLabel.Size = new System.Drawing.Size(10, 13);
            this.employmentStatusValueLabel.TabIndex = 58;
            this.employmentStatusValueLabel.Text = " ";
            // 
            // employmentStatusLabel
            // 
            this.employmentStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.employmentStatusLabel.Location = new System.Drawing.Point(9, 701);
            this.employmentStatusLabel.Name = "employmentStatusLabel";
            this.employmentStatusLabel.Size = new System.Drawing.Size(77, 16);
            this.employmentStatusLabel.TabIndex = 57;
            this.employmentStatusLabel.Text = "Employment status:";
            // 
            // valuablesCollectedValuelabel
            // 
            this.valuablesCollectedValuelabel.AutoSize = true;
            this.valuablesCollectedValuelabel.Location = new System.Drawing.Point(158, 670);
            this.valuablesCollectedValuelabel.Name = "valuablesCollectedValuelabel";
            this.valuablesCollectedValuelabel.Size = new System.Drawing.Size(10, 13);
            this.valuablesCollectedValuelabel.TabIndex = 56;
            this.valuablesCollectedValuelabel.Text = " ";
            // 
            // valuablesCollectedLabel
            // 
            this.valuablesCollectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valuablesCollectedLabel.Location = new System.Drawing.Point(9, 670);
            this.valuablesCollectedLabel.Name = "valuablesCollectedLabel";
            this.valuablesCollectedLabel.Size = new System.Drawing.Size(129, 16);
            this.valuablesCollectedLabel.TabIndex = 55;
            this.valuablesCollectedLabel.Text = "Valuables collected:";
            // 
            // placeOfWorshipValueLabel
            // 
            this.placeOfWorshipValueLabel.AutoSize = true;
            this.placeOfWorshipValueLabel.Location = new System.Drawing.Point(128, 641);
            this.placeOfWorshipValueLabel.Name = "placeOfWorshipValueLabel";
            this.placeOfWorshipValueLabel.Size = new System.Drawing.Size(10, 13);
            this.placeOfWorshipValueLabel.TabIndex = 54;
            this.placeOfWorshipValueLabel.Text = " ";
            // 
            // placeOfWorshipLabel
            // 
            this.placeOfWorshipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.placeOfWorshipLabel.Location = new System.Drawing.Point(9, 641);
            this.placeOfWorshipLabel.Name = "placeOfWorshipLabel";
            this.placeOfWorshipLabel.Size = new System.Drawing.Size(109, 16);
            this.placeOfWorshipLabel.TabIndex = 53;
            this.placeOfWorshipLabel.Text = "Place of worship:";
            // 
            // clergyMayVisitValueLabel
            // 
            this.clergyMayVisitValueLabel.AutoSize = true;
            this.clergyMayVisitValueLabel.Location = new System.Drawing.Point(112, 616);
            this.clergyMayVisitValueLabel.Name = "clergyMayVisitValueLabel";
            this.clergyMayVisitValueLabel.Size = new System.Drawing.Size(10, 13);
            this.clergyMayVisitValueLabel.TabIndex = 52;
            this.clergyMayVisitValueLabel.Text = " ";
            // 
            // clergyMayVisitLabel
            // 
            this.clergyMayVisitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clergyMayVisitLabel.Location = new System.Drawing.Point(9, 616);
            this.clergyMayVisitLabel.Name = "clergyMayVisitLabel";
            this.clergyMayVisitLabel.Size = new System.Drawing.Size(107, 16);
            this.clergyMayVisitLabel.TabIndex = 51;
            this.clergyMayVisitLabel.Text = "Clergy may visit:";
            // 
            // religionValueLabel
            // 
            this.religionValueLabel.AutoSize = true;
            this.religionValueLabel.Location = new System.Drawing.Point(78, 591);
            this.religionValueLabel.Name = "religionValueLabel";
            this.religionValueLabel.Size = new System.Drawing.Size(10, 13);
            this.religionValueLabel.TabIndex = 50;
            this.religionValueLabel.Text = " ";
            // 
            // religionLabel
            // 
            this.religionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.religionLabel.Location = new System.Drawing.Point(9, 591);
            this.religionLabel.Name = "religionLabel";
            this.religionLabel.Size = new System.Drawing.Size(60, 16);
            this.religionLabel.TabIndex = 49;
            this.religionLabel.Text = "Religion:";
            // 
            // languageValueLabel
            // 
            this.languageValueLabel.AutoSize = true;
            this.languageValueLabel.Location = new System.Drawing.Point(90, 566);
            this.languageValueLabel.Name = "languageValueLabel";
            this.languageValueLabel.Size = new System.Drawing.Size(10, 13);
            this.languageValueLabel.TabIndex = 48;
            this.languageValueLabel.Text = " ";
            // 
            // languageLabel
            // 
            this.languageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.languageLabel.Location = new System.Drawing.Point(9, 566);
            this.languageLabel.Name = "languageLabel";
            this.languageLabel.Size = new System.Drawing.Size(69, 16);
            this.languageLabel.TabIndex = 47;
            this.languageLabel.Text = "Language:";
            // 
            // placeOfBirthValueLabel
            // 
            this.placeOfBirthValueLabel.AutoSize = true;
            this.placeOfBirthValueLabel.Location = new System.Drawing.Point(106, 541);
            this.placeOfBirthValueLabel.Name = "placeOfBirthValueLabel";
            this.placeOfBirthValueLabel.Size = new System.Drawing.Size(10, 13);
            this.placeOfBirthValueLabel.TabIndex = 46;
            this.placeOfBirthValueLabel.Text = " ";
            // 
            // placeOfBirthLabel
            // 
            this.placeOfBirthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.placeOfBirthLabel.Location = new System.Drawing.Point(9, 541);
            this.placeOfBirthLabel.Name = "placeOfBirthLabel";
            this.placeOfBirthLabel.Size = new System.Drawing.Size(91, 13);
            this.placeOfBirthLabel.TabIndex = 45;
            this.placeOfBirthLabel.Text = "Place of birth:";
            // 
            // driversLicenseValueLabel
            // 
            this.driversLicenseValueLabel.AutoSize = true;
            this.driversLicenseValueLabel.Location = new System.Drawing.Point(142, 491);
            this.driversLicenseValueLabel.Name = "driversLicenseValueLabel";
            this.driversLicenseValueLabel.Size = new System.Drawing.Size(10, 13);
            this.driversLicenseValueLabel.TabIndex = 44;
            this.driversLicenseValueLabel.Text = " ";
            // 
            // driversLicenseLabel
            // 
            this.driversLicenseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.driversLicenseLabel.Location = new System.Drawing.Point(9, 491);
            this.driversLicenseLabel.Name = "driversLicenseLabel";
            this.driversLicenseLabel.Size = new System.Drawing.Size(136, 16);
            this.driversLicenseLabel.TabIndex = 43;
            this.driversLicenseLabel.Text = "U.S. driver\'s license:";
            // 
            // passportValueLabel
            // 
            this.passportValueLabel.AutoSize = true;
            this.passportValueLabel.Location = new System.Drawing.Point(78, 516);
            this.passportValueLabel.Name = "passportValueLabel";
            this.passportValueLabel.Size = new System.Drawing.Size(10, 13);
            this.passportValueLabel.TabIndex = 44;
            this.passportValueLabel.Text = " ";
            // 
            // passportLabel
            // 
            this.passportLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passportLabel.Location = new System.Drawing.Point(10, 516);
            this.passportLabel.Name = "passportLabel";
            this.passportLabel.Size = new System.Drawing.Size(69, 16);
            this.passportLabel.TabIndex = 43;
            this.passportLabel.Text = "Passport:";
            // 
            // contactEmailValueLabel
            // 
            this.contactEmailValueLabel.AutoSize = true;
            this.contactEmailValueLabel.Location = new System.Drawing.Point(45, 6943);
            this.contactEmailValueLabel.Name = "contactEmailValueLabel";
            this.contactEmailValueLabel.Size = new System.Drawing.Size(10, 13);
            this.contactEmailValueLabel.TabIndex = 36;
            this.contactEmailValueLabel.Text = " ";
            // 
            // contactEmailLabel
            // 
            this.contactEmailLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contactEmailLabel.Location = new System.Drawing.Point(7, 6943);
            this.contactEmailLabel.Name = "contactEmailLabel";
            this.contactEmailLabel.Size = new System.Drawing.Size(200, 16);
            this.contactEmailLabel.TabIndex = 35;
            this.contactEmailLabel.Text = "Email:";
            // 
            // contactCellValueLabel
            // 
            this.contactCellValueLabel.AutoSize = true;
            this.contactCellValueLabel.Location = new System.Drawing.Point(197, 416);
            this.contactCellValueLabel.Name = "contactCellValueLabel";
            this.contactCellValueLabel.Size = new System.Drawing.Size(10, 13);
            this.contactCellValueLabel.TabIndex = 34;
            this.contactCellValueLabel.Text = " ";
            // 
            // contactCellLabel
            // 
            this.contactCellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contactCellLabel.Location = new System.Drawing.Point(7, 416);
            this.contactCellLabel.Name = "contactCellLabel";
            this.contactCellLabel.Size = new System.Drawing.Size(184, 16);
            this.contactCellLabel.TabIndex = 33;
            this.contactCellLabel.Text = "Contact (mailing address) cell:";
            // 
            // contactPhoneValueLabel
            // 
            this.contactPhoneValueLabel.AutoSize = true;
            this.contactPhoneValueLabel.Location = new System.Drawing.Point(211, 391);
            this.contactPhoneValueLabel.Name = "contactPhoneValueLabel";
            this.contactPhoneValueLabel.Size = new System.Drawing.Size(10, 13);
            this.contactPhoneValueLabel.TabIndex = 32;
            this.contactPhoneValueLabel.Text = " ";
            // 
            // contactPhoneLabel
            // 
            this.contactPhoneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contactPhoneLabel.Location = new System.Drawing.Point(7, 391);
            this.contactPhoneLabel.Name = "contactPhoneLabel";
            this.contactPhoneLabel.Size = new System.Drawing.Size(198, 16);
            this.contactPhoneLabel.TabIndex = 31;
            this.contactPhoneLabel.Text = "Contact (mailing address) phone:";
            // 
            // patientMailingAddressValueLabel
            // 
            this.patientMailingAddressValueLabel.AutoSize = true;
            this.patientMailingAddressValueLabel.Location = new System.Drawing.Point(148, 365);
            this.patientMailingAddressValueLabel.Name = "patientMailingAddressValueLabel";
            this.patientMailingAddressValueLabel.Size = new System.Drawing.Size(10, 13);
            this.patientMailingAddressValueLabel.TabIndex = 30;
            this.patientMailingAddressValueLabel.Text = " ";
            // 
            // patientMailingAddressLabel
            // 
            this.patientMailingAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientMailingAddressLabel.Location = new System.Drawing.Point(7, 365);
            this.patientMailingAddressLabel.Name = "patientMailingAddressLabel";
            this.patientMailingAddressLabel.Size = new System.Drawing.Size(135, 16);
            this.patientMailingAddressLabel.TabIndex = 29;
            this.patientMailingAddressLabel.Text = "Patient mailing address:";
            // 
            // nationalIdValueLabel
            // 
            this.nationalIdValueLabel.AutoSize = true;
            this.nationalIdValueLabel.Location = new System.Drawing.Point(84, 344);
            this.nationalIdValueLabel.Name = "nationalIdValueLabel";
            this.nationalIdValueLabel.Size = new System.Drawing.Size(10, 13);
            this.nationalIdValueLabel.TabIndex = 28;
            this.nationalIdValueLabel.Text = " ";
            // 
            // nationalIdLabel
            // 
            this.nationalIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nationalIdLabel.Location = new System.Drawing.Point(7, 344);
            this.nationalIdLabel.Name = "nationalIdLabel";
            this.nationalIdLabel.Size = new System.Drawing.Size(71, 16);
            this.nationalIdLabel.TabIndex = 27;
            this.nationalIdLabel.Text = "National ID:";
            // 
            // ssnValueLabel
            // 
            this.ssnValueLabel.AutoSize = true;
            this.ssnValueLabel.Location = new System.Drawing.Point(44, 319);
            this.ssnValueLabel.Name = "ssnValueLabel";
            this.ssnValueLabel.Size = new System.Drawing.Size(10, 13);
            this.ssnValueLabel.TabIndex = 26;
            this.ssnValueLabel.Text = " ";
            // 
            // ssnLabel
            // 
            this.ssnLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssnLabel.Location = new System.Drawing.Point(7, 319);
            this.ssnLabel.Name = "ssnLabel";
            this.ssnLabel.Size = new System.Drawing.Size(31, 16);
            this.ssnLabel.TabIndex = 25;
            this.ssnLabel.Text = "SSN:";
            // 
            // ethnicityValueLabel
            // 
            this.ethnicityValueLabel.AutoSize = true;
            this.ethnicityValueLabel.Location = new System.Drawing.Point(65, 299);
            this.ethnicityValueLabel.Name = "ethnicityValueLabel";
            this.ethnicityValueLabel.Size = new System.Drawing.Size(10, 13);
            this.ethnicityValueLabel.TabIndex = 24;
            this.ethnicityValueLabel.Text = " ";
            // 
            // ethnicityLabel
            // 
            this.ethnicityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ethnicityLabel.Location = new System.Drawing.Point(7, 299);
            this.ethnicityLabel.Name = "ethnicityLabel";
            this.ethnicityLabel.Size = new System.Drawing.Size(52, 16);
            this.ethnicityLabel.TabIndex = 23;
            this.ethnicityLabel.Text = "Ethnicity:";
            // 
            // raceValueLabel
            // 
            this.raceValueLabel.AutoSize = true;
            this.raceValueLabel.Location = new System.Drawing.Point(48, 276);
            this.raceValueLabel.Name = "raceValueLabel";
            this.raceValueLabel.Size = new System.Drawing.Size(10, 13);
            this.raceValueLabel.TabIndex = 22;
            this.raceValueLabel.Text = " ";
            // 
            // raceLabel
            // 
            this.raceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.raceLabel.Location = new System.Drawing.Point(7, 276);
            this.raceLabel.Name = "raceLabel";
            this.raceLabel.Size = new System.Drawing.Size(35, 16);
            this.raceLabel.TabIndex = 21;
            this.raceLabel.Text = "Race:";
            // 
            // maritalStatusValueLabel
            // 
            this.maritalStatusValueLabel.AutoSize = true;
            this.maritalStatusValueLabel.Location = new System.Drawing.Point(98, 255);
            this.maritalStatusValueLabel.Name = "maritalStatusValueLabel";
            this.maritalStatusValueLabel.Size = new System.Drawing.Size(10, 13);
            this.maritalStatusValueLabel.TabIndex = 20;
            this.maritalStatusValueLabel.Text = " ";
            // 
            // maritalStatusLabel
            // 
            this.maritalStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maritalStatusLabel.Location = new System.Drawing.Point(7, 255);
            this.maritalStatusLabel.Name = "maritalStatusLabel";
            this.maritalStatusLabel.Size = new System.Drawing.Size(85, 16);
            this.maritalStatusLabel.TabIndex = 19;
            this.maritalStatusLabel.Text = "Marital Status:";
            // 
            // ageValueLabel
            // 
            this.ageValueLabel.AutoSize = true;
            this.ageValueLabel.Location = new System.Drawing.Point(41, 232);
            this.ageValueLabel.Name = "ageValueLabel";
            this.ageValueLabel.Size = new System.Drawing.Size(10, 13);
            this.ageValueLabel.TabIndex = 18;
            this.ageValueLabel.Text = " ";
            // 
            // ageLabel
            // 
            this.ageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ageLabel.Location = new System.Drawing.Point(7, 232);
            this.ageLabel.Name = "ageLabel";
            this.ageLabel.Size = new System.Drawing.Size(28, 16);
            this.ageLabel.TabIndex = 17;
            this.ageLabel.Text = "Age:";
            // 
            // rightToRestrictValueLabel
            // 
            this.rightToRestrictValueLabel.AutoSize = true;
            this.rightToRestrictValueLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rightToRestrictValueLabel.Location = new System.Drawing.Point(217, 6877);
            this.rightToRestrictValueLabel.Name = "rightToRestrictValueLabel";
            this.rightToRestrictValueLabel.Size = new System.Drawing.Size(10, 13);
            this.rightToRestrictValueLabel.TabIndex = 548;
            this.rightToRestrictValueLabel.Text = " ";
            // 
            // dobLabel
            // 
            this.dobLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dobLabel.Location = new System.Drawing.Point(7, 211);
            this.dobLabel.Name = "dobLabel";
            this.dobLabel.Size = new System.Drawing.Size(33, 16);
            this.dobLabel.TabIndex = 15;
            this.dobLabel.Text = "DOB:";
            // 
            // genderValueLabel
            // 
            this.genderValueLabel.AutoSize = true;
            this.genderValueLabel.Location = new System.Drawing.Point(60, 188);
            this.genderValueLabel.Name = "genderValueLabel";
            this.genderValueLabel.Size = new System.Drawing.Size(10, 13);
            this.genderValueLabel.TabIndex = 14;
            this.genderValueLabel.Text = " ";
            // 
            // genderLabel
            // 
            this.genderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.genderLabel.Location = new System.Drawing.Point(7, 188);
            this.genderLabel.Name = "genderLabel";
            this.genderLabel.Size = new System.Drawing.Size(47, 16);
            this.genderLabel.TabIndex = 13;
            this.genderLabel.Text = "Gender:";
            // 
            // appointmentValueLabel
            // 
            this.appointmentValueLabel.AutoSize = true;
            this.appointmentValueLabel.Location = new System.Drawing.Point(87, 166);
            this.appointmentValueLabel.Name = "appointmentValueLabel";
            this.appointmentValueLabel.Size = new System.Drawing.Size(10, 13);
            this.appointmentValueLabel.TabIndex = 12;
            this.appointmentValueLabel.Text = " ";
            // 
            // appointmentLabel
            // 
            this.appointmentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appointmentLabel.Location = new System.Drawing.Point(7, 166);
            this.appointmentLabel.Name = "appointmentLabel";
            this.appointmentLabel.Size = new System.Drawing.Size(74, 16);
            this.appointmentLabel.TabIndex = 11;
            this.appointmentLabel.Text = "Appointment";
            // 
            // admitDateTimeValueLabel
            // 
            this.admitDateTimeValueLabel.AutoSize = true;
            this.admitDateTimeValueLabel.Location = new System.Drawing.Point(108, 80);
            this.admitDateTimeValueLabel.Name = "admitDateTimeValueLabel";
            this.admitDateTimeValueLabel.Size = new System.Drawing.Size(10, 13);
            this.admitDateTimeValueLabel.TabIndex = 6;
            this.admitDateTimeValueLabel.Text = " ";
            // 
            // admitDateTimeLabel
            // 
            this.admitDateTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.admitDateTimeLabel.Location = new System.Drawing.Point(7, 80);
            this.admitDateTimeLabel.Name = "admitDateTimeLabel";
            this.admitDateTimeLabel.Size = new System.Drawing.Size(95, 16);
            this.admitDateTimeLabel.TabIndex = 5;
            this.admitDateTimeLabel.Text = "Admit date/time:";
            // 
            // akaValueLabel
            // 
            this.akaValueLabel.AutoSize = true;
            this.akaValueLabel.Location = new System.Drawing.Point(53, 59);
            this.akaValueLabel.Name = "akaValueLabel";
            this.akaValueLabel.Size = new System.Drawing.Size(10, 13);
            this.akaValueLabel.TabIndex = 4;
            this.akaValueLabel.Text = " ";
            // 
            // akaLabel
            // 
            this.akaLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.akaLabel.Location = new System.Drawing.Point(7, 59);
            this.akaLabel.Name = "akaLabel";
            this.akaLabel.Size = new System.Drawing.Size(31, 16);
            this.akaLabel.TabIndex = 3;
            this.akaLabel.Text = "AKA:";
            // 
            // patientNameValueLabel
            // 
            this.patientNameValueLabel.AutoSize = true;
            this.patientNameValueLabel.Location = new System.Drawing.Point(94, 38);
            this.patientNameValueLabel.Name = "patientNameValueLabel";
            this.patientNameValueLabel.Size = new System.Drawing.Size(10, 13);
            this.patientNameValueLabel.TabIndex = 2;
            this.patientNameValueLabel.Text = " ";
            // 
            // dobValueLabel
            // 
            this.dobValueLabel.AutoSize = true;
            this.dobValueLabel.Location = new System.Drawing.Point(46, 211);
            this.dobValueLabel.Name = "dobValueLabel";
            this.dobValueLabel.Size = new System.Drawing.Size(10, 13);
            this.dobValueLabel.TabIndex = 16;
            this.dobValueLabel.Text = " ";
            // 
            // patientNameLabel
            // 
            this.patientNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientNameLabel.Location = new System.Drawing.Point(7, 38);
            this.patientNameLabel.Name = "patientNameLabel";
            this.patientNameLabel.Size = new System.Drawing.Size(81, 16);
            this.patientNameLabel.TabIndex = 1;
            this.patientNameLabel.Text = "Patient name:";
            // 
            // patientDemographicsAndEmploymentLabel
            // 
            this.patientDemographicsAndEmploymentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientDemographicsAndEmploymentLabel.Location = new System.Drawing.Point(7, 7);
            this.patientDemographicsAndEmploymentLabel.Name = "patientDemographicsAndEmploymentLabel";
            this.patientDemographicsAndEmploymentLabel.Size = new System.Drawing.Size(312, 20);
            this.patientDemographicsAndEmploymentLabel.TabIndex = 0;
            this.patientDemographicsAndEmploymentLabel.Text = "Patient Demographics and Employment";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "ResearchStudy.Code";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle8;
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
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle9;
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
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle10;
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
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridViewTextBoxColumn7.HeaderText = "Proof Of Consent?";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 130;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn7.Width = 130;
            // 
            // NonPurgedAccountDetails
            // 
            this.Controls.Add(this.NonPurgedAccountDetailsPanel);
            this.Name = "NonPurgedAccountDetails";
            this.Size = new System.Drawing.Size(908, 528);
            this.NonPurgedAccountDetailsPanel.ResumeLayout(false);
            this.NonPurgedAccountDetailsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
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
                akaValueLabel.Text = ( (Name)patient.Aliases[FIRST_ALIAS] )
                    .AsFormattedName();
            }
            if ( account.AdmitDate != DateTime.MinValue )
            {
                admitDateTimeValueLabel.Text = account.AdmitDate.ToString( "MM/dd/yyyy HH:mm" );
            }
            if ( account.PreopDate != DateTime.MinValue )
            {
                preopDateValueLabel.Text = account.PreopDate.ToString( "MM/dd/yyyy" );
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
            if ( patient.DateOfBirth != DateTime.MinValue )
            {
                dobValueLabel.Text = patient.DateOfBirth.ToString( "MM/dd/yyyy" );
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
            nationalIdValueLabel.Text = patient.NationalID;
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
            contactEmailValueLabel.Text = account.Patient.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() ).EmailAddress.Uri;

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
                passportValueLabel.Text += " " + patient.Passport.Country.Code;
            }

            placeOfBirthValueLabel.Text = patient.PlaceOfBirth;
            if ( account.Patient.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() ).
                Address != null )
            {
                patientPhysicalAddressValueLabel.Text = account.Patient.ContactPointWith(
                    TypeOfContactPoint.NewPhysicalContactPointType() ).Address.
                    OneLineAddressLabel();
            }
            if ( account.Patient.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType()
                ).PhoneNumber != null )
            {
                contactPhysicalAddressPhoneValueLabel.Text = account.Patient.ContactPointWith(
                    TypeOfContactPoint.NewPhysicalContactPointType() ).PhoneNumber.
                    AsFormattedString();
            }
            if ( patient.Language != null )
            {
                languageValueLabel.Text = patient.Language.Description;
            }
            if ( patient.Religion != null )
            {
                religionValueLabel.Text = patient.Religion.Description;
            }
            if ( account.ClergyVisit != null )
            {
                clergyMayVisitValueLabel.Text = account.ClergyVisit.Description;
            }
            if ( patient.PlaceOfWorship != null )
            {
                placeOfWorshipValueLabel.Text = patient.PlaceOfWorship.Description;
            }
            if ( account.ValuablesAreTaken != null )
            {
                valuablesCollectedValuelabel.Text = account.ValuablesAreTaken.
                    Description;
            }
            if ( patient.Employment != null )
            {
                if ( patient.Employment.Status != null )
                {
                    employmentStatusValueLabel.Text = patient.Employment.Status.
                        Description;
                }
            }
            if ( patient.Employment != null )
            {
                if ( patient.Employment.Employer != null )
                {
                    employerValueLabel.Text = patient.Employment.Employer.Name;
                }
                if ( patient.Employment.PhoneNumber != null )
                {
                    employerPhoneValueLabel.Text = patient.Employment.Employer.
                        PartyContactPoint.PhoneNumber.AsFormattedString();
                }
                employeeIDValueLabel.Text = patient.Employment.EmployeeID;
                occIndustryValueLabel.Text = patient.Employment.Occupation;
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
            if ( account.Location != null )
            {
                assignedBedValueLabel.Text = account.Location.FormattedLocation;
                if ( account.Location.Bed != null &&
                    account.Location.Bed.Accomodation != null )
                {
                    accomodationcodeValueLabelL.Text =
                        account.Location.Bed.Accomodation.DisplayString;
                }
            }

            if ( account.Diagnosis != null )
            {
                chiefComplaintValueLabel.Text = account.Diagnosis.ChiefComplaint;
                procedureValueLabel.Text = account.Diagnosis.Procedure;
                CptCodesValueLabel.Text = account.CptCodes.Count > 0 ? "Yes" : "No";
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
                            (TimeAndLocationBoundCondition)account.Diagnosis.Condition;
                        if ( condition.OccurredOn != DateTime.MinValue )
                        {
                            accidentCrimeDateValueLabel.Text =
                                String.Format( "{0:D2}/{1:D2}/{2:D4}",
                                condition.OccurredOn.Month,
                                condition.OccurredOn.Day,
                                condition.OccurredOn.Year );
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

            if ( account.TenetCare != null )
            {
                tenetCareValueLabel.Text = account.TenetCare.Description;
            }
            if ( account.AdmitSource != null )
            {
                admitSourceValueLabel.Text = account.AdmitSource.DisplayString;
            }
            if ( account.AlternateCareFacility != null )
            {
                alternatecareFacilityValueLabel.Text = account.AlternateCareFacility;
            }
            if ( account.ModeOfArrival != null )
            {
                modeOfArrivalValueLabel.Text = account.ModeOfArrival.Description;
            }
            if ( account.ReferralSource != null )
            {
                referralSourceValueLabel.Text = account.ReferralSource.Description;
            }

            // HACK: This should not be in the view, but a subclass or some other external
            // mechanism since it is not broadly applicable to the application
            this.PopulateClinicalTrialValueLabel();

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


        /// <summary>
        /// Populates the clinical trial value label.
        /// </summary>
        private void PopulateClinicalTrialValueLabel()
        {
            if ( !this.account.HasExtendedProperty( ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS ) )
            {
                return;
            }

            if ( this.account[ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS].ToString() == YesNoFlag.CODE_YES )
            {
                this.clinicalTrialValueLabel.Text = "Yes";
            }
            else
            {
                this.clinicalTrialValueLabel.Text = "No";
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
            if ( account.OperatingPhysician != null )
            {
                operatingPhysicianValueLabel.Text = String.Format( "{0:00000} {1}",
                    account.OperatingPhysician.PhysicianNumber,
                    account.OperatingPhysician.FormattedName );
            }
            if ( account.PrimaryCarePhysician != null )
            {
                primaryCarePhysicianValueLabel.Text = String.Format( "{0:00000} {1}",
                    account.PrimaryCarePhysician.PhysicianNumber,
                    account.PrimaryCarePhysician.FormattedName );
            }
            if ( account.Smoker != null )
            {
                smokerValueLabel.Text = account.Smoker.Description;
            }
            if ( account.Bloodless != null )
            {
                if ( account.Bloodless.Code.ToUpper() == YesNoFlag.CODE_YES )
                {
                    bloodlessValueLabel.Text = "YES, DESIRES TREATMENT WITHOUT BLOOD";
                }
                else if ( account.Bloodless.Code.ToUpper() == YesNoFlag.CODE_NO )
                {
                    bloodlessValueLabel.Text = "NO, DESIRES TREATMENT WITH BLOOD";
                }
            }
            if ( account.Pregnant != null )
            {
                pregnantValueLabel.Text = account.Pregnant.Description;
            }

            if ( account.ResistantOrganism != null )
                ResistantOrganismValueLabel.Text = account.ResistantOrganism;

            commentsValueLabel.Text = account.ClinicalComments;

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
                    if ( coverage.InsurancePlan != null && !account.IsUrgentCarePreMse )
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
            if (account.Insurance != null && !account.IsUrgentCarePreMse)
            {
                Coverage coverage = PrimaryCoverage();
                if ( coverage != null )
                {
                    if ( coverage.InsurancePlan.GetType() == typeof( CommercialInsurancePlan ) )
                    {
                        CommercialCoverage commercialCoverage = (CommercialCoverage)coverage;
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
                            = (GovernmentMedicaidCoverage)coverage;
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
                            (WorkersCompensationCoverage)coverage;

                        //Workers compensation policy number
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "Workers compensation policy number:";
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 215, 16 );

                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                            new Point( 228, 2467 );
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
                            (GovernmentOtherCoverage)coverage;
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
                            (GovernmentMedicareCoverage)coverage;
                    

                        //HIC number
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "MBI number: ";
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 76, 16 );
                        //primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Location =
                        //    new System.Drawing.Point(7, 2480);

                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                           new Point(88, 2467);
                        primaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Text =
                            governmentMedicareCoverage.FormattedMBINumber;
 

                        if (governmentMedicareCoverage.BillingInformation != null)
                        {
                            primaryBillingCoNameValueLabel.Text = governmentMedicareCoverage.
                                BillingInformation.BillingCOName;
                            primaryBillingNameValueLabel.Text = governmentMedicareCoverage.
                                BillingInformation.BillingName;
                            if (governmentMedicareCoverage.BillingInformation.PhoneNumber != null)
                            {
                                primaryBillingPhoneValueLabel.Text = governmentMedicareCoverage.
                                    BillingInformation.PhoneNumber.AsFormattedString();
                            }
                            if (governmentMedicareCoverage.BillingInformation.Address != null)
                            {
                                primaryBillingAddressValueLabel.Text = governmentMedicareCoverage.
                                    BillingInformation.Address.OneLineAddressLabel();
                            }
                        }

                    }
                    if ( coverage.InsurancePlan.GetType() == typeof( SelfPayInsurancePlan ) )
                    {
                        SelfPayCoverage selfPayCoverage = (SelfPayCoverage)coverage;
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
                        OtherCoverage otherCoverage = (OtherCoverage)coverage;
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
            if (account.PrimaryInsured != null && !account.IsUrgentCarePreMse)
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
            if (account.Insurance != null && !account.IsUrgentCarePreMse)
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

                    Authorization authorization = ( (CoverageGroup)coverage ).Authorization;
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
                    var medicareCoverage = (GovernmentMedicareCoverage)coverage;
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
                        CommercialCoverage commercialCoverage = (CommercialCoverage)coverage;
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
                            = (GovernmentMedicaidCoverage)coverage;

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
                            (WorkersCompensationCoverage)coverage;

                        //Workers compensation policy number
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "Workers compensation policy number:";
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 215, 16 );

                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                            new Point( 228, 3915 );
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
                            (GovernmentOtherCoverage)coverage;
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
                            (GovernmentMedicareCoverage)coverage;

                        //MBI number
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Text =
                            "MBI number: ";
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel.Size =
                            new Size( 76, 16 );
                        
                        secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberValueLabel.Location =
                            new Point(88, 3915);
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
                        SelfPayCoverage selfPayCoverage = (SelfPayCoverage)coverage;
                        selfPayCoverage = (SelfPayCoverage)coverage;
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
                        OtherCoverage otherCoverage = (OtherCoverage)coverage;
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
                if ( cp.CellPhoneNumber != null )
                {
                    secondaryContactCellValueLabel.Text = cp.CellPhoneNumber.AsFormattedString();
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
                    secondaryEmployerPhoneValueLabel.Text = account.SecondaryInsured.
                        Employment.PhoneNumber.AsFormattedString();
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
                    if (coverage.GetType() == typeof(CommercialCoverage) ||
                        coverage.GetType() == typeof(OtherCoverage) ||
                        coverage.GetType() == typeof(GovernmentOtherCoverage))
                    {
                        secondaryAuthorizationTrackingNumberValueLabel.Text =
                            ((CoverageForCommercialOther) coverage).TrackingNumber;
                    }

                    if (coverage.GetType() == typeof(GovernmentMedicaidCoverage))
                    {
                        secondaryAuthorizationTrackingNumberValueLabel.Text =
                            ((GovernmentMedicaidCoverage) coverage).TrackingNumber;
                    }

                    Authorization authorization = ( (CoverageGroup)coverage ).Authorization;
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
                    var secondaryMedicareCoverage = ((GovernmentMedicareCoverage)coverage);
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
                if ( account.Guarantor.Sex != null )
                {
                    guarantorGenderValueLabel.Text = account.Guarantor.Sex.Description;
                }
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
                if ( cp.EmailAddress != null )
                {
                    guarantorContactEmailValueLabel.Text = account.Guarantor.
                        ContactPointWith( cp.TypeOfContactPoint ).EmailAddress.Uri;
                }
                if ( account.Guarantor.Employment != null )
                {
                    if ( account.Guarantor.Employment.Status != null )
                    {
                        guarantorEmploymentStatusValueLabel.Text = account.Guarantor.
                            Employment.Status.Description;
                    }
                    if ( account.Guarantor.Employment.Employer != null )
                    {
                        guarantorEmployerValueLabel.Text = account.Guarantor.Employment
                            .Employer.Name;
                    }
                    guarantorEmployerPhoneValueLabel.Text =
                        account.Guarantor.Employment.Employer.PartyContactPoint.PhoneNumber.AsFormattedString();

                    guarantorOccIndustryValueLabel.Text = account.Guarantor.Employment.Occupation;
                }
                if (account.Guarantor.DateOfBirth != DateTime.MinValue)
                {
                    guarantorDOBValuelabel.Text = account.Guarantor.DateOfBirth.ToString("MM/dd/yyyy");
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
            if ( account.OccurrenceSpans[0] != null )
            {
                OccurrenceSpan spanCode1 = (OccurrenceSpan)account.OccurrenceSpans[0];
                if ( spanCode1.SpanCode != null )
                {
                    spanCode1ValueLabel.Text = spanCode1.SpanCode.DisplayString;
                }
                if ( spanCode1.FromDate != DateTime.MinValue )
                {
                    spanCode1FromdateValueLabel.Text = spanCode1.FromDate.ToString( "MM/dd/yyyy" );
                }
                if ( spanCode1.ToDate != DateTime.MinValue )
                {
                    spanCode1TodateValueLabel.Text = spanCode1.ToDate.ToString( "MM/dd/yyyy" );
                }
                spanCode1FacilityValueLabel.Text = spanCode1.Facility;
            }
            if ( account.OccurrenceSpans[1] != null )
            {
                OccurrenceSpan spanCode2 = (OccurrenceSpan)account.OccurrenceSpans[1];
                if ( spanCode2.SpanCode != null )
                {
                    spanCode2ValueLabel.Text = spanCode2.SpanCode.DisplayString;
                }
                if ( spanCode2.FromDate != DateTime.MinValue )
                {
                    spanCode2FromdateValueLabel.Text = spanCode2.FromDate.ToString( "MM/dd/yyyy" );
                }
                if ( spanCode2.ToDate != DateTime.MinValue )
                {
                    spanCode2TodateValueLabel.Text = spanCode2.ToDate.ToString( "MM/dd/yyyy" );
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
            if ( account.NumberOfMonthlyPayments != 0 )
            {
                monthlyPayment = ( account.TotalCurrentAmtDue - account.TotalPaid ) / account.NumberOfMonthlyPayments;
                monthlyPaymentValueLabel.Text = DOLLAR_SIGN + monthlyPayment.ToString( "###,###,##0.00" );
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
            if ( account.Patient.NoticeOfPrivacyPracticeDocument != null &&
                account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion != null )
            {
                nppVersionValueLabel.Text = account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion.DisplayString;
                if ( account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus != null )
                {
                    if ( account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus.Code == SignatureStatus.SIGNED &&
                        account.Patient.NoticeOfPrivacyPracticeDocument.SignedOnDate != DateTime.MinValue )
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
            if ( account.ConfidentialityCode != null )
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
            if ( account.COSSigned != null )
            {
                cosSignedValueLabel.Text = account.COSSigned.Description;
            }
            if ( account.FacilityDeterminedFlag != null )
            {
                facilityDeterminedFlagValueLabel.Text = account.FacilityDeterminedFlag.
                    Description;
            }
            if (account.ShareDataWithPublicHieFlag != null)
            {
                shareDataWithPublicHieValueLabel.Text = account.ShareDataWithPublicHieFlag.Description;
            }
            if (account.ShareDataWithPCPFlag != null)
            {
               shareDataWithPCPValueLabel.Text = account.ShareDataWithPCPFlag.Description;
            }
            var HospCommOptInFeatureManager = new HospitalCommunicationOptInFeatureManager();
            if (account.Patient.HospitalCommunicationOptIn != null && HospCommOptInFeatureManager.ShouldFeatureBeEnabled(account))
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
        private Label nationalIdLabel;
        private Label nationalIdValueLabel;
        private Label patientMailingAddressLabel;
        private Label patientMailingAddressValueLabel;
        private Label contactPhoneLabel;
        private Label contactPhoneValueLabel;
        private Label contactCellLabel;
        private Label contactCellValueLabel;
        private Label contactEmailLabel;
        private Label contactEmailValueLabel;
        private Label driversLicenseLabel;
        private Label driversLicenseValueLabel;
        private Label passportLabel;
        private Label passportValueLabel;
        private Label placeOfBirthLabel;
        private Label placeOfBirthValueLabel;
        private Label languageLabel;
        private Label languageValueLabel;
        private Label religionLabel;
        private Label religionValueLabel;
        private Label clergyMayVisitLabel;
        private Label clergyMayVisitValueLabel;
        private Label placeOfWorshipLabel;
        private Label valuablesCollectedLabel;
        private Label valuablesCollectedValuelabel;
        private Label employmentStatusLabel;
        private Label employmentStatusValueLabel;
        private Label employerlabel;
        private Label employerValueLabel;
        private Label employerPhoneLabel;
        private Label employerPhoneValueLabel;
        private Label occIndustryLabel;
        private Label occIndustryValueLabel;
        private Label employeeIDlabel;
        private Label employeeIDValueLabel;
        private Label patientTypeLabel;
        private Label patientTypeValueLabel;
        private Label reregisterLabel;
        private Label reregisterValueLabel;
        private Label hospitalServiceLabel;
        private Label hospitalServiceValueLabel;
        private Label assignedBedLabel;
        private Label assignedBedValueLabel;
        private Label accomodationcodeLabelL;
        private Label accomodationcodeValueLabelL;
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
        private Label tenetCareLabel;
        private Label admitSourceLabel;
        private Label modeOfArrivalLabel;
        private Label referralSourceLabel;
        private Label clinic1Label;
        private Label clinic2Label;
        private Label accidentCrimeHourValueLabel;
        private Label accidentCrimeCountryValueLabel;
        private Label accidentCrimeStateProvinceValueLabel;
        private Label dateOfOnsetForSymptomsIllnessValueLabel;
        private Label tenetCareValueLabel;
        private Label admitSourceValueLabel;
        private Label modeOfArrivalValueLabel;
        private Label clinic3Label;
        private Label clinic4Label;
        private Label clinic5Label;
        private Label referralSourceValueLabel;
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
        private Label clinicalLabel;
        private Label operatingPhysicianLabel;
        private Label primaryCarePhysicianLabel;
        private Label smokerLabel;
        private Label bloodlessLabel;
        private Label pregnantLabel;
        private Label ResistantOrganismLabel;
        private Label commentsLabel;
        private Label operatingPhysicianValueLabel;
        private Label primaryCarePhysicianValueLabel;
        private Label smokerValueLabel;
        private Label bloodlessValueLabel;
        private Label pregnantValueLabel;
        private Label ResistantOrganismValueLabel;
        private Label commentsValueLabel;
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
        private Label placeOfWorshipValueLabel;
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
        //private Label primaryMBIValueLabel;
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
        //private Label primaryMBILabel;
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
        //private Label secondaryMBIValueLabel;
        private Label secondaryMedicalGroupIpaNameLabel;
        private Label secondaryAdjusterLabel;
        private Label secondaryEmployeeSupervisorLabel;
        private Label secondaryMedicaidIssueDateLabel;
        private Label secondaryPrecertificationNumberLabel;
        private Label secondaryGroupNumberLabel;
        private Label secondaryCertSsnIdPolicyCinHicWorkersCompensationPolicyNumberLabel;
        //private Label secondaryMBILabel;
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
        private Label guarantorGenderLabel;
        private Label guarantorSSNLabel;
        private Label usDriverLicenseLabel;
        private Label guarantorAddressLabel;
        private Label guarantorContactPhoneLabel;
        private Label guarantorContactCellLabel;
        private Label guarantorContactEmailLabel;
        private Label guarantorEmploymentStatusLabel;
        private Label guarantorEmployerLabel;
        private Label guarantorEmployerPhoneLabel;
        private Label guarantorOccIndustryLabel;
        private Label guarantorNameValueLabel;
        private Label thePatientIsTheGuarantorsValueLabel;
        private Label guarantorGenderValueLabel;
        private Label guarantorSSNValueLabel;
        private Label usDriverLicenseValueLabel;
        private Label guarantorAddressValueLabel;
        private Label guarantorContactPhoneValueLabel;
        private Label guarantorContactCellValueLabel;
        private Label guarantorContactEmailValueLabel;
        private Label guarantorEmploymentStatusValueLabel;
        private Label guarantorEmployerValueLabel;
        private Label guarantorEmployerPhoneValueLabel;
        private Label guarantorOccIndustryValueLabel;
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
        private Label spanCode1Label;
        private Label spanCode1FromdateLabel;
        private Label spanCode1TodateLabel;
        private Label spanCode1FromdateValueLabel;
        private Label spanCode1TodateValueLabel;
        private Label spanCode1FacilityLabel;
        private Label spanCode2Label;
        private Label spanCode2FromdateLabel;
        private Label spanCode2TodateLabel;
        private Label spanCode1FacilityValueLabel;
        private Label spanCode2FromdateValueLabel;
        private Label spanCode2TodateValueLabel;
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
        private Label contactsLabel;
        private Label emergencyContact1NameLabel;
        private Label emergencyContact1IsThePatientsLabel;
        private Label emergencyContact1AddressLabel;
        private Label emergencyContact1PhoneLabel;
        private Label emergencyContact2NameLabel;
        private Label emergencyContact2IsThePatientsLabel;
        private Label emergencyContact2AddressLabel;
        private Label emergencyContact2PhoneLabel;
        private Label regulatoryLabel;
        private Label nppVersionLabel;
        private Label privacyConfidentialStatusLabel;
        private Label privacyOptOutPatientDirectoryNameAndAllInformationLabel;
        private Label privacyOptOutPatientDirectoryLocationLabel;
        private Label privacyOptOutPatientDirectoryHealthInformationLabel;
        private Label privacyOptOutPatientDirectoryReligionLabel;
        private Label cosSignedLabel;
        private Label facilityDeterminedFlagLabel;
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
        private Label cosSignedValueLabel;
        private Label facilityDeterminedFlagValueLabel;
        private Label secondaryBillingAddressValueLabel;
        private Label secondaryBillingAddressLabel;
        private Panel NonPurgedAccountDetailsPanel;
        private Label patientPhysicalAddressLabel;
        private Label patientPhysicalAddressValueLabel;
        private Label contactPhysicalAddressPhoneLabel;
        private Label contactPhysicalAddressPhoneValueLabel;
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

        private Panel clinicalPanel;
        private Panel diagnosisPanel;
        private Panel generalInsuranceInformationPanel2;
        private Panel mspPanel;
        private Panel primaryInsurancePlanPanel;
        private Panel primaryInsurancePayorDetailsPanel;
        private Panel primaryInsuranceInsuredPanel;
        private Panel primaryInsuranceVerificationPanel;
        private Panel secondaryInsurancePlanPanel;
        private Panel secondaryInsurancePayorDetailsPanel;
        private Panel secondaryInsuranceInsuredPanel;
        private Panel secondaryInsuranceVerificationPanel;
        private Panel guarantorPanel;
        private Panel billingPanel;
        private Panel liabilityPanel;
        private Panel paymentPanel;
        private Panel contactsPanel;
        private Panel regulatoryPanel;
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
        private Label spanCode1ValueLabel;
        private Label spanCode2ValueLabel;

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
        private Panel primaryInsuranceAuthorizationPanel;
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
        private Panel secondaryInsuranceAuthorizationPanel;
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
        private Label clinicalTrialLabel;
        private Label clinicalTrialValueLabel;
        private Label preopDateValueLabel;
        private Label preopDateLabel;
        private Label procedureValueLabel;
        private Label procedureLabel;
        private Label PatientInClinicalREsearchLBL;
        private Label PatientInClinicalStudyValueLabel;
        private Label alternatecareFacilityValueLabel;
        private Label AlternatecareFacilityLabel;
        private Label LeftOrStayedValueLabel;
        private Label LeftOrStayedLabel;
        private Label RCRPValueLabel;
        private Label RCRPLabel;
        private Label LeftWithoutBeingSeenValueLabel;
        private Label LeftWithoutBeingSeenLabel;
        private Label LeftWithoutFinancialClearanceValueLabel;
        private Label LeftWithoutFinancialClearanceLabel;
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
        private Label rightToRestrictValueLabel;
        private Label rightToRestrictLabel;
        private Label CptCodesValueLabel;
        private Label CptCodesLabel;
        private Label guarantorCellPhoneConsentValueLabel;
        private Label guarantorCellPhoneConsentLabel;
        private Label shareDataWithPCPFlagLabel;
        private Label shareDataWithPCPValueLabel;
        private Label shareDataWithPublicHieFlagLabel;
        private Label shareDataWithPublicHieValueLabel;
        private Label HospCommoptinValueLabel;
        private Label cobReceivedFlagLabel;
        private Label cobReceivedValueLabel;
        private Label imfmReceivedFlagLabel;
        private Label imfmReceivedValueLabel;
        private const string LABEL_SPACE = "  ";
        private Label monthlyDueDateValueLabel;
        private Label guarantorDOBValuelabel;
        private Label guarantorDOBLabel;
        private Label HospCommoptinlLabel;
        private Label monthlyDueDateLabel;
        #endregion
 
    }
}
