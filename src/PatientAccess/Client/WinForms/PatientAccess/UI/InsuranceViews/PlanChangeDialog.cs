using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews
{
    /// <summary>
    /// Summary description for PlanChangeDialog.
    /// </summary>
    public class PlanChangeDialog : TimeOutFormView
    {
        #region Event Handlers

        private void btnCancel_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Set the states of the Insured and Verification checkboxes.
        /// </summary>
        private void PlanChangeDialog_Load(object sender, EventArgs e)
        {
            Debug.Assert( newCoverage != null );
            Debug.Assert( oldCoverage != null );

            lblInsuredName.Text = oldCoverage.Insured.Name.AsFormattedName();
            if( oldCoverage.Insured.Employment != null 
                && oldCoverage.Insured.Employment.Employer != null)
            {
                lblEmployer.Text = oldCoverage.Insured.Employment.Employer.Name;
            }

            if( lblInsuredName.Text.Length == 0 )
                lblInsuredName.Text = "Not available";
            if( lblEmployer.Text.Length == 0 )
                lblEmployer.Text = "Not available";
            InsuredCheckboxEnabled = !newCoverage.IsEmployerPlan;

            // if the user chose the new plan via the Select by Employer (Covered Group), then
            // don't allow them to retain the old employer

            if( newCoverage.Insured != null
                && newCoverage.Insured.Employment != null
                && newCoverage.Insured.Employment.Employer != null
                && newCoverage.Insured.Employment.Employer.Name != string.Empty)
            {
                this.ckbInsuredEmployer.Checked     = false;
                this.ckbInsuredEmployer.Enabled     = false;
            }

            // Set the "Retain Insurance Verification" checkbox based on if  
            // the new plan is in the same coverage category as the old plan.
            VerificationCheckboxEnabled = oldCoverage.GetType().Equals( newCoverage.GetType() );
        }

        /// <summary>
        /// Execute the checkbox choices
        /// </summary>
        private void btnContinue_Click(object sender, EventArgs e)
        {            
            if( oldCoverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
            {
                this.Model_Account.FinancialClass = new FinancialClass();
            }
            
            if( ckbPersonalInfo.Checked )
            {   // Copy the Insured object data to the new coverage
                foreach( Relationship r in oldCoverage.Insured.Relationships )
                {
                    newCoverage.Insured.AddRelationship( r );
                }
                foreach( ContactPoint c in oldCoverage.Insured.ContactPoints )
                {
                    newCoverage.Insured.AddContactPoint( c );
                }

                newCoverage.Insured.Name        = oldCoverage.Insured.Name;
                newCoverage.Insured.Sex         = oldCoverage.Insured.Sex;
                newCoverage.Insured.DateOfBirth = oldCoverage.Insured.DateOfBirth;
                newCoverage.Insured.NationalID  = oldCoverage.Insured.NationalID;
            }
            else
            {
                newCoverage.Insured = new Insured();
            }

            if( ckbInsuredEmployer.Checked && oldCoverage.Insured.Employment != null )
            {
                newCoverage.Insured.Employment = (Employment)oldCoverage.Insured.Employment.Clone();
            }
            else
            {
                if( this.ckbInsuredEmployer.Enabled == true )
                {
                    newCoverage.Insured.Employment = new Employment();
                }                
            }
            if( ckbRetainVerification.Checked )
            {
                RetainInsuredVerification();
            }

            newCoverage.IsNew   = true;
        }
        #endregion

        #region Properties

        private Account Model_Account
        {
            get
            {
                return i_Model_Account;
            }
            set
            {
                this.i_Model_Account = value;
            }
        }

        private bool InsuredCheckboxEnabled
        {
            set
            {
                ckbInsuredEmployer.Enabled = ckbInsuredEmployer.Checked = value;
            }
        }

        private bool VerificationCheckboxEnabled
        {
            set
            {
                ckbRetainVerification.Enabled = ckbRetainVerification.Checked = value;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// If the coverages are of the same type, the user has the option
        /// to import data from the old coverage to the new coverage.
        /// </summary>
        private void RetainInsuredVerification()
        {
            if( newCoverage == null || newCoverage.InsurancePlan == null )
            {
                return;
            }
            if( oldCoverage == null || oldCoverage.InsurancePlan == null )
            {
                return;
            }

            if( newCoverage.InsurancePlan.GetType() == typeof( CommercialInsurancePlan ) )
            {
                CommercialCoverage newcc            = (CommercialCoverage)newCoverage;
                CommercialCoverage oldcc            = (CommercialCoverage)oldCoverage;
                if(oldcc == null)
                {
                    return;
                }
                newcc.Authorization.AuthorizationCompany          = oldcc.Authorization.AuthorizationCompany;
                newcc.AuthorizingPerson             = oldcc.AuthorizingPerson;
                newcc.Authorization.AuthorizationPhone = oldcc.Authorization.AuthorizationPhone;
                if (oldcc.Authorization.AuthorizationRequired != null)
                {
                    newcc.Authorization.AuthorizationRequired = (YesNotApplicableFlag)oldcc.Authorization.AuthorizationRequired.Clone();
                }
                else
                {
                    newcc.Authorization.AuthorizationRequired = new YesNotApplicableFlag();
                }
                if( oldcc.BenefitsVerified != null)
                {
                    newcc.BenefitsVerified              = (YesNotApplicableFlag)oldcc.BenefitsVerified.Clone();
                }
                else
                {
                     newcc.BenefitsVerified              = new YesNotApplicableFlag();
                }
                newcc.CoPay                         = oldcc.CoPay;
                newcc.DateTimeOfVerification        = oldcc.DateTimeOfVerification;
                newcc.Deductible                    = oldcc.Deductible;
                newcc.InformationReceivedSource     = oldcc.InformationReceivedSource;
                newcc.Authorization.PromptExt = oldcc.Authorization.PromptExt;
                newcc.Remarks                       = oldcc.Remarks;

                newcc.AutoInsuranceClaimNumber      =
                    oldcc.AutoInsuranceClaimNumber;
                if(oldcc.AutoMedPayCoverage != null)
                {
                    newcc.AutoMedPayCoverage            =
                        (YesNoFlag)oldcc.AutoMedPayCoverage.Clone();
                }
                else
                {
                   newcc.AutoMedPayCoverage = new YesNoFlag();
                }

                if(oldcc.ClaimsAddressVerified != null)
                {
                    newcc.ClaimsAddressVerified         =
                        (YesNoFlag)oldcc.ClaimsAddressVerified.Clone();
                }
                else
                {
                newcc.ClaimsAddressVerified         =  new YesNoFlag();            
                }

                if(oldcc.CoordinationOfbenefits != null)
                {
                    newcc.CoordinationOfbenefits        =
                        (YesNoFlag)oldcc.CoordinationOfbenefits.Clone();
                }
                else
                {
                    newcc.CoordinationOfbenefits        = new YesNoFlag();
                }

                newcc.EffectiveDateForInsured       =
                    oldcc.EffectiveDateForInsured;

                newcc.EligibilityPhone              =
                    oldcc.EligibilityPhone;

                if(oldcc.FacilityContractedProvider != null)
                {
                    newcc.FacilityContractedProvider    =
                        (YesNoFlag)oldcc.FacilityContractedProvider.Clone();
                }
                else
                {
                     newcc.FacilityContractedProvider    = new YesNoFlag();
                }

                newcc.InsuranceCompanyRepName       =
                    oldcc.InsuranceCompanyRepName;

                newcc.PPOPricingOrBroker            = 
                    oldcc.PPOPricingOrBroker;

                if(oldcc.ServiceForPreExistingCondition != null)
                {
                    newcc.ServiceForPreExistingCondition =
                        (YesNoFlag)oldcc.ServiceForPreExistingCondition.Clone();
                }
                else
                {
                    newcc.ServiceForPreExistingCondition = new YesNoFlag();
                }

                if(oldcc.ServiceIsCoveredBenefit != null)
                {
                    newcc.ServiceIsCoveredBenefit       =
                        (YesNoFlag)oldcc.ServiceIsCoveredBenefit.Clone();
                }
                else
                {
                    newcc.ServiceIsCoveredBenefit       = new YesNoFlag();
                }

                newcc.TerminationDateForInsured     =
                    oldcc.TerminationDateForInsured;

				if( (TypeOfVerificationRule)oldcc.TypeOfVerificationRule != null )
				{
					newcc.TypeOfVerificationRule    =
						(TypeOfVerificationRule)oldcc.TypeOfVerificationRule.Clone();
				}
				else
				{
					newcc.TypeOfVerificationRule    = new TypeOfVerificationRule();
				}
//                newcc.TypeOfVerificationRule        =
//                    (TypeOfVerificationRule)oldcc.TypeOfVerificationRule.Clone();
                if(oldcc.TypeOfProduct != null)
                {
                    newcc.TypeOfProduct                 =
                        (TypeOfProduct)oldcc.TypeOfProduct.Clone();
                }
                else
                {
                     newcc.TypeOfProduct                 = new TypeOfProduct();
                }

                if(oldcc.Attorney != null)
                {
                    newcc.Attorney                      = oldcc.Attorney;
                }
                else
                {
                    newcc.Attorney                      =  new Attorney();
                }
                if(oldcc.InsuranceAgent != null )
                {
                    newcc.InsuranceAgent                = oldcc.InsuranceAgent;
                }
                else
                {
                    newcc.InsuranceAgent                = new InsuranceAgent();
                }

                newcc.BenefitsCategories            = (Hashtable)oldcc.BenefitsCategories.Clone();               
                Hashtable benefitsCategories        = oldcc.BenefitsCategories;
            }
            else if( newCoverage.InsurancePlan.GetType() == typeof( GovernmentMedicaidInsurancePlan ) )
            {
                newCoverage.InformationReceivedSource = oldCoverage.InformationReceivedSource;
                newCoverage.Remarks                   = oldCoverage.Remarks;

                (newCoverage as GovernmentMedicaidCoverage).EligibilityDate =
                    (oldCoverage as GovernmentMedicaidCoverage).EligibilityDate;

                (newCoverage as GovernmentMedicaidCoverage).EVCNumber =
                    (oldCoverage as GovernmentMedicaidCoverage).EVCNumber;

                (newCoverage as GovernmentMedicaidCoverage).MedicaidCopay =
                    (oldCoverage as GovernmentMedicaidCoverage).MedicaidCopay;

                (newCoverage as GovernmentMedicaidCoverage).PatienthasMedicare =
                    (YesNoFlag)(oldCoverage as GovernmentMedicaidCoverage).PatienthasMedicare.Clone();

                (newCoverage as GovernmentMedicaidCoverage).PatienthasOtherInsuranceCoverage =
                    (YesNoFlag)(oldCoverage as GovernmentMedicaidCoverage).PatienthasOtherInsuranceCoverage.Clone();
            }
            else if( newCoverage.InsurancePlan.GetType() == typeof( GovernmentMedicareInsurancePlan ) )
            {
                newCoverage.InformationReceivedSource = oldCoverage.InformationReceivedSource;
                newCoverage.Remarks                   = oldCoverage.Remarks;

                (newCoverage as GovernmentMedicareCoverage).DateOfLastBillingActivity =
                    (oldCoverage as GovernmentMedicareCoverage).DateOfLastBillingActivity;

                (newCoverage as GovernmentMedicareCoverage).MedicareIsSecondary =
                    (YesNoFlag)(oldCoverage as GovernmentMedicareCoverage).MedicareIsSecondary.Clone();

                (newCoverage as GovernmentMedicareCoverage).PartACoverage =
                    (YesNoFlag)(oldCoverage as GovernmentMedicareCoverage).PartACoverage.Clone();

                (newCoverage as GovernmentMedicareCoverage).PartBCoverage =
                    (YesNoFlag)(oldCoverage as GovernmentMedicareCoverage).PartBCoverage.Clone();

                (newCoverage as GovernmentMedicareCoverage).PatientHasMedicareHMOCoverage =
                    (YesNoFlag)(oldCoverage as GovernmentMedicareCoverage).PatientHasMedicareHMOCoverage.Clone();

                (newCoverage as GovernmentMedicareCoverage).PatientIsPartOfHospiceProgram =
                    (YesNoFlag)(oldCoverage as GovernmentMedicareCoverage).PatientIsPartOfHospiceProgram.Clone();

                (newCoverage as GovernmentMedicareCoverage).PartACoverageEffectiveDate =
                    (oldCoverage as GovernmentMedicareCoverage).PartACoverageEffectiveDate;

                (newCoverage as GovernmentMedicareCoverage).PartBCoverageEffectiveDate =
                    (oldCoverage as GovernmentMedicareCoverage).PartBCoverageEffectiveDate;

                (newCoverage as GovernmentMedicareCoverage).RemainingBenefitPeriod =
                    (oldCoverage as GovernmentMedicareCoverage).RemainingBenefitPeriod;

                (newCoverage as GovernmentMedicareCoverage).RemainingCoInsurance =
                    (oldCoverage as GovernmentMedicareCoverage).RemainingCoInsurance;

                (newCoverage as GovernmentMedicareCoverage).RemainingLifeTimeReserve =
                    (oldCoverage as GovernmentMedicareCoverage).RemainingLifeTimeReserve;

                (newCoverage as GovernmentMedicareCoverage).RemainingSNF =
                    (oldCoverage as GovernmentMedicareCoverage).RemainingSNF;

                (newCoverage as GovernmentMedicareCoverage).RemainingSNFCoInsurance =
                    (oldCoverage as GovernmentMedicareCoverage).RemainingSNFCoInsurance;

                (newCoverage as GovernmentMedicareCoverage).RemainingPartADeductible =
                    (oldCoverage as GovernmentMedicareCoverage).RemainingPartADeductible;

                (newCoverage as GovernmentMedicareCoverage).RemainingPartBDeductible =
                    (oldCoverage as GovernmentMedicareCoverage).RemainingPartBDeductible;

                (newCoverage as GovernmentMedicareCoverage).VerifiedBeneficiaryName =
                    (YesNoFlag)(oldCoverage as GovernmentMedicareCoverage).VerifiedBeneficiaryName.Clone();
            }
            else if( newCoverage.InsurancePlan.GetType() == typeof( GovernmentOtherInsurancePlan ) )
            {
                GovernmentOtherCoverage newcc            = (GovernmentOtherCoverage)newCoverage;
                GovernmentOtherCoverage oldcc            = (GovernmentOtherCoverage)oldCoverage;

                newcc.InformationReceivedSource = oldCoverage.InformationReceivedSource;
                newcc.Remarks                   = oldCoverage.Remarks;

                newcc.BenefitsCategoryDetails =
                    oldcc.BenefitsCategoryDetails;

                newcc.EffectiveDateForInsured =
                    oldcc.EffectiveDateForInsured;

                newcc.EligibilityPhone =
                    oldcc.EligibilityPhone;

                newcc.InsuranceCompanyRepName =
                    oldcc.InsuranceCompanyRepName;

                newcc.TerminationDateForInsured =
                    oldcc.TerminationDateForInsured;

                newcc.TypeOfCoverage =
                    oldcc.TypeOfCoverage;
            }
            else if( newCoverage.InsurancePlan.GetType() == typeof( SelfPayInsurancePlan ) )
            {
                (newCoverage as SelfPayCoverage).PatientHasMedicaid =
                    (YesNoFlag)(oldCoverage as SelfPayCoverage).PatientHasMedicaid.Clone();

                (newCoverage as SelfPayCoverage).InsuranceInfoUnavailable =
                    (YesNoFlag)(oldCoverage as SelfPayCoverage).InsuranceInfoUnavailable.Clone();
            }
            else if( newCoverage.InsurancePlan.GetType() == typeof( WorkersCompensationInsurancePlan ) )
            {
                newCoverage.InformationReceivedSource = oldCoverage.InformationReceivedSource;
                newCoverage.Remarks                   = oldCoverage.Remarks;

                (newCoverage as WorkersCompensationCoverage).ClaimsAddressVerified =
                    (YesNoFlag)(oldCoverage as WorkersCompensationCoverage).ClaimsAddressVerified.Clone();

                (newCoverage as WorkersCompensationCoverage).ClaimNumberForIncident =
                    (oldCoverage as WorkersCompensationCoverage).ClaimNumberForIncident;

                (newCoverage as WorkersCompensationCoverage).EmployerhasPaidPremiumsToDate =
                    (YesNoFlag)(oldCoverage as WorkersCompensationCoverage).EmployerhasPaidPremiumsToDate.Clone();

                (newCoverage as WorkersCompensationCoverage).InsurancePhone =
                    (oldCoverage as WorkersCompensationCoverage).InsurancePhone;

                (newCoverage as WorkersCompensationCoverage).PPOPricingOrBroker =
                    (oldCoverage as WorkersCompensationCoverage).PPOPricingOrBroker;
            }
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.backPanel = new System.Windows.Forms.Panel();
			this.mainPanel = new System.Windows.Forms.Panel();
			this.lblPayor = new System.Windows.Forms.Label();
			this.ckbRetainVerification = new System.Windows.Forms.CheckBox();
			this.lblStaticInsVerify = new System.Windows.Forms.Label();
			this.ckbInsuredEmployer = new System.Windows.Forms.CheckBox();
			this.lblStaticRelationship = new System.Windows.Forms.Label();
			this.ckbPersonalInfo = new System.Windows.Forms.CheckBox();
			this.lblStaticInsured = new System.Windows.Forms.Label();
			this.lblStaticPayorDetails = new System.Windows.Forms.Label();
			this.lblEmployer = new System.Windows.Forms.Label();
			this.lblStaticEmployer = new System.Windows.Forms.Label();
			this.lblInsuredName = new System.Windows.Forms.Label();
			this.lblStaticInsuredName = new System.Windows.Forms.Label();
			this.lblInstructions1 = new System.Windows.Forms.Label();
			this.btnCancel = new LoggingButton();
			this.btnContinue = new LoggingButton();
			this.backPanel.SuspendLayout();
			this.mainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// backPanel
			// 
			this.backPanel.BackColor = System.Drawing.Color.Black;
			this.backPanel.Controls.Add(this.mainPanel);
			this.backPanel.Location = new System.Drawing.Point(10, 10);
			this.backPanel.Name = "backPanel";
			this.backPanel.Size = new System.Drawing.Size(442, 420);
			this.backPanel.TabIndex = 0;
			// 
			// mainPanel
			// 
			this.mainPanel.BackColor = System.Drawing.Color.White;
			this.mainPanel.Controls.Add(this.lblPayor);
			this.mainPanel.Controls.Add(this.ckbRetainVerification);
			this.mainPanel.Controls.Add(this.lblStaticInsVerify);
			this.mainPanel.Controls.Add(this.ckbInsuredEmployer);
			this.mainPanel.Controls.Add(this.lblStaticRelationship);
			this.mainPanel.Controls.Add(this.ckbPersonalInfo);
			this.mainPanel.Controls.Add(this.lblStaticInsured);
			this.mainPanel.Controls.Add(this.lblStaticPayorDetails);
			this.mainPanel.Controls.Add(this.lblEmployer);
			this.mainPanel.Controls.Add(this.lblStaticEmployer);
			this.mainPanel.Controls.Add(this.lblInsuredName);
			this.mainPanel.Controls.Add(this.lblStaticInsuredName);
			this.mainPanel.Controls.Add(this.lblInstructions1);
			this.mainPanel.Location = new System.Drawing.Point(1, 1);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(440, 418);
			this.mainPanel.TabIndex = 1;
			// 
			// lblPayor
			// 
			this.lblPayor.Location = new System.Drawing.Point(58, 143);
			this.lblPayor.Name = "lblPayor";
			this.lblPayor.Size = new System.Drawing.Size(166, 23);
			this.lblPayor.TabIndex = 0;
			this.lblPayor.Text = "All payor details will be cleared.";
			// 
			// ckbRetainVerification
			// 
			this.ckbRetainVerification.Enabled = false;
			this.ckbRetainVerification.Location = new System.Drawing.Point(58, 387);
			this.ckbRetainVerification.Name = "ckbRetainVerification";
			this.ckbRetainVerification.Size = new System.Drawing.Size(288, 24);
			this.ckbRetainVerification.TabIndex = 4;
			this.ckbRetainVerification.Text = "Retain all Insurance Verification information, if any";
			// 
			// lblStaticInsVerify
			// 
			this.lblStaticInsVerify.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblStaticInsVerify.Location = new System.Drawing.Point(15, 361);
			this.lblStaticInsVerify.Name = "lblStaticInsVerify";
			this.lblStaticInsVerify.Size = new System.Drawing.Size(128, 23);
			this.lblStaticInsVerify.TabIndex = 0;
			this.lblStaticInsVerify.Text = "Insurance Verification";
			// 
			// ckbInsuredEmployer
			// 
			this.ckbInsuredEmployer.Enabled = false;
			this.ckbInsuredEmployer.Location = new System.Drawing.Point(58, 325);
			this.ckbInsuredEmployer.Name = "ckbInsuredEmployer";
			this.ckbInsuredEmployer.Size = new System.Drawing.Size(192, 24);
			this.ckbInsuredEmployer.TabIndex = 3;
			this.ckbInsuredEmployer.Text = "Retain Insured\'s employer, if any";
			// 
			// lblStaticRelationship
			// 
			this.lblStaticRelationship.Location = new System.Drawing.Point(96, 228);
			this.lblStaticRelationship.Name = "lblStaticRelationship";
			this.lblStaticRelationship.Size = new System.Drawing.Size(64, 92);
			this.lblStaticRelationship.TabIndex = 0;
			this.lblStaticRelationship.Text = "Relationship Name   Address Contact Gender    DOB National ID";
			// 
			// ckbPersonalInfo
			// 
			this.ckbPersonalInfo.Checked = true;
			this.ckbPersonalInfo.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckbPersonalInfo.Location = new System.Drawing.Point(58, 202);
			this.ckbPersonalInfo.Name = "ckbPersonalInfo";
			this.ckbPersonalInfo.Size = new System.Drawing.Size(250, 24);
			this.ckbPersonalInfo.TabIndex = 2;
			this.ckbPersonalInfo.Text = "Retain Insured\'s personal information, if any";
			// 
			// lblStaticInsured
			// 
			this.lblStaticInsured.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblStaticInsured.Location = new System.Drawing.Point(14, 176);
			this.lblStaticInsured.Name = "lblStaticInsured";
			this.lblStaticInsured.Size = new System.Drawing.Size(85, 23);
			this.lblStaticInsured.TabIndex = 0;
			this.lblStaticInsured.Text = "Insured";
			// 
			// lblStaticPayorDetails
			// 
			this.lblStaticPayorDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblStaticPayorDetails.Location = new System.Drawing.Point(14, 117);
			this.lblStaticPayorDetails.Name = "lblStaticPayorDetails";
			this.lblStaticPayorDetails.Size = new System.Drawing.Size(85, 23);
			this.lblStaticPayorDetails.TabIndex = 0;
			this.lblStaticPayorDetails.Text = "Payor Details";
			// 
			// lblEmployer
			// 
			this.lblEmployer.Location = new System.Drawing.Point(106, 80);
			this.lblEmployer.Name = "lblEmployer";
			this.lblEmployer.Size = new System.Drawing.Size(325, 23);
			this.lblEmployer.TabIndex = 0;
			// 
			// lblStaticEmployer
			// 
			this.lblStaticEmployer.Location = new System.Drawing.Point(14, 80);
			this.lblStaticEmployer.Name = "lblStaticEmployer";
			this.lblStaticEmployer.Size = new System.Drawing.Size(95, 23);
			this.lblStaticEmployer.TabIndex = 0;
			this.lblStaticEmployer.Text = "Insured employer:";
			// 
			// lblInsuredName
			// 
			this.lblInsuredName.Location = new System.Drawing.Point(106, 53);
			this.lblInsuredName.Name = "lblInsuredName";
			this.lblInsuredName.Size = new System.Drawing.Size(326, 23);
			this.lblInsuredName.TabIndex = 0;
			// 
			// lblStaticInsuredName
			// 
			this.lblStaticInsuredName.Location = new System.Drawing.Point(14, 53);
			this.lblStaticInsuredName.Name = "lblStaticInsuredName";
			this.lblStaticInsuredName.Size = new System.Drawing.Size(80, 23);
			this.lblStaticInsuredName.TabIndex = 0;
			this.lblStaticInsuredName.Text = "Insured name:";
			// 
			// lblInstructions1
			// 
			this.lblInstructions1.Location = new System.Drawing.Point(14, 14);
			this.lblInstructions1.Name = "lblInstructions1";
			this.lblInstructions1.Size = new System.Drawing.Size(426, 31);
			this.lblInstructions1.TabIndex = 0;
			this.lblInstructions1.Text = "To complete the insurance plan change, make any desired modifications to the sele" +
				"ctions and click Continue.  To abandon the insurance plan change, click Cancel.";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(377, 440);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
            this.btnCancel.Click +=new EventHandler(btnCancel_Click);
			// 
			// btnContinue
			// 
			this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnContinue.Location = new System.Drawing.Point(292, 440);
			this.btnContinue.Name = "btnContinue";
			this.btnContinue.TabIndex = 2;
			this.btnContinue.Text = "Contin&ue >";
			this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
			// 
			// PlanChangeDialog
			// 
			this.AcceptButton = this.btnContinue;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(464, 473);
			this.Controls.Add(this.btnContinue);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.backPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PlanChangeDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Complete the Insurance Plan Change";
			this.Load += new System.EventHandler(this.PlanChangeDialog_Load);
			this.backPanel.ResumeLayout(false);
			this.mainPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion

        #region Construction and Finalization
        public PlanChangeDialog( Coverage oldCoverage, Coverage newCoverage, Account theAccount )
        {
            this.newCoverage    = newCoverage;
            this.oldCoverage    = oldCoverage;
            this.Model_Account  = theAccount;

            //
            // Required for Windows Form Designer support
            //
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

        #region Data Elements
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private LoggingButton     btnCancel;
        private LoggingButton     btnContinue;

        private CheckBox   ckbInsuredEmployer;
        private CheckBox   ckbPersonalInfo;
        private CheckBox   ckbRetainVerification;

        private Label      lblInstructions1;
        private Label      lblStaticInsuredName;
        private Label      lblInsuredName;
        private Label      lblStaticEmployer;
        private Label      lblEmployer;
        private Label      lblStaticPayorDetails;
        private Label      lblStaticInsured;
        private Label      lblStaticRelationship;
        private Label      lblStaticInsVerify;
        private Label      lblPayor;

        private Panel      backPanel;
        private Panel      mainPanel;

        private Coverage                        newCoverage;
        private Coverage                        oldCoverage;

        private Account                         i_Model_Account;
        #endregion

    }
}
