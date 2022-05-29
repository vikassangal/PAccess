using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using log4net;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    /// <summary>
    /// Summary description for VerificationCoverage.
    /// </summary>
    public class VerificationCoverage : ControlView
    {
        #region Events

        public event EventHandler ShowValidationResponseEvent;
        //public event EventHandler ResponseAcceptedEvent;

        #endregion

        #region Event Handlers

        void VerificationCoverage_Leave(object sender, EventArgs e)
        {
            this.StopResponsePollTimer();
            this.StopResponseWaitTimer();
        }

        private void btnInitiate_Click(object sender, EventArgs e)
        {
            c_log.Info("IVP - Initiate benefits verification");
            bool rc = this.initiateBenefitsValidation();
        }


        /// <summary>
        /// responsePollTimer_Tick - handler for ResponsePollTimer expired event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResponsePollTimer_Tick(object sender, EventArgs e)
        {

            // see if we've got a response
            IDataValidationBroker dataValidationBroker =
                BrokerFactory.BrokerOfType<IDataValidationBroker>();

            bool resultsAreReady =
                dataValidationBroker.AreBenefitsValidationResultsAvailableFor(
                    this.Model_Coverage.DataValidationTicket.TicketId,
                    ConfigurationManager.AppSettings["BenefitsValidationUserOverride"] ??
                    User.GetCurrent().SecurityUser.UPN,
                    User.GetCurrent().Facility.Code);

            if (resultsAreReady)
            {

                this.StopResponseWaitTimer();
                this.StopResponsePollTimer();

                dataValidationBroker.SaveResponseIndicator(
                    this.Model_Coverage.DataValidationTicket.TicketId,
                    true);

                try
                {

                    BenefitsValidationResponse response = this.getValidationResponse();

                    if (response != null)
                    {
                        this.ChangeBenefitsResponseToUppercase(response);
                        Model_Coverage.DataValidationTicket = response.ReturnedDataValidationTicket;
                        this.checkValidationResponse(response);

                    }//if

                }//try
                catch (Exception ex)
                {

                    this.btnInitiate.Enabled = true;
                    this.StopResponsePollTimer();
                    this.StopResponseWaitTimer();

                    this.textBox.Clear();
                    this.textBox.Text =
                        String.Format(UIErrorMessages.EDV_SERVICE_UNAVAILABLE,
                                       Environment.NewLine,
                                       ex);

                    if (ShowValidationResponseEvent != null)
                    {
                        ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                    }

                    c_log.Info("IVP - SERVICE UNAVAILABLE!!!");

                }//catch

            }//if

        }

        /// <summary>
        /// responseWaitTimer_Tick - handler for ResponseWaitTimer expired event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResponseWaitTimer_Tick(object sender, EventArgs e)
        {
            // no response for the wait interval (up to 5 minutes)

            this.textBox.Clear();
            this.textBox.Text = NO_RESPONSE_RECEIVED;
            if (ShowValidationResponseEvent != null)
            {
                ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
            }

            c_log.Info("IVP - Response wait timer has expired.  Stop Wait/Poll TIMERS!!!");

            this.StopResponsePollTimer();
            this.StopResponseWaitTimer();

            this.btnInitiate.Enabled = true;
        }

        /// <summary>
        /// stopResponseWaitTimer - stop the wait timer
        /// </summary>
        private void StopResponseWaitTimer()
        {
            if (this.i_ResponseWaitTimer != null)
            {
                this.i_ResponseWaitTimer.Stop();
                this.i_ResponseWaitTimer.Enabled = false;
            }
        }
        #endregion

        #region Methods
        public void PopulateView()
        {
            this.lblStaticProvider.Size = new Size(125, 14);
            if (Model_Coverage.GetType() == typeof(GovernmentMedicareCoverage))
            {
                this.lblStaticProvider.Text = MEDICARE_PROVIDER;
            }
            else if (Model_Coverage.GetType() == typeof(GovernmentMedicaidCoverage))
            {
                this.lblStaticProvider.Text = MEDICAID_PROVIDER;
            }
            else
            {
                this.lblStaticProvider.Text = string.Empty;
            }

            btnInitiate.Enabled = false;

            lblPatientName.Text = Account.Patient.FormattedName;

            if (Account.Patient.DateOfBirth != DateTime.MinValue)
            {
                lblDOB.Text = String.Format("{0:D2}/{1:D2}/{2:D4}",
                    Account.Patient.DateOfBirth.Month,
                    Account.Patient.DateOfBirth.Day,
                    Account.Patient.DateOfBirth.Year);
            }
            else
            {
                lblDOB.Text = string.Empty;
            }

            if (Account.AccountNumber == 0)
            {
                lblAccount.Text = string.Empty;
            }
            else
            {
                lblAccount.Text = Account.AccountNumber.ToString();
            }

            // TLG 07/16 Add null check (see Crash Report 43940)

            if (Account.KindOfVisit != null)
            {
                lblPatientType.Text = Account.KindOfVisit.ToString();
            }

            lblFacilityID.Text = Account.Facility.FederalTaxID;

            if (Account.FinancialClass != null)
            {
                lblFinancialClass.Text = Account.FinancialClass.ToString();
            }

            if (Account.HospitalService != null)
            {
                lblHospitalService.Text = Account.HospitalService.ToString();
            }

            lblChiefComplaint.Text = Account.Diagnosis.ChiefComplaint;

            if (Model_Coverage.InsurancePlan.GetType() == typeof(GovernmentMedicaidInsurancePlan))
            {
                lblMedicaidProvider.Text = Model_Coverage.InsurancePlan.PlanID;
            }
        }

        public override void UpdateView()
        {

            this.PopulateView();

            if (this.Model_Coverage != null
                && this.Model_Coverage.DataValidationTicket != null
                && this.Model_Coverage.DataValidationTicket.TicketId != null
                && this.Model_Coverage.DataValidationTicket.TicketId.Trim() != String.Empty)
            {
                DataValidationTicket aTicket = this.Model_Coverage.DataValidationTicket;

                if (aTicket.ResultsAvailable)
                {
                    // if the user has not acted on the results that are available, then check the response to
                    // see if we need to show the dialogue - and, then proceed

                    BenefitsValidationResponse response = this.getValidationResponse();

                    if (response != null
                        && response.PayorMessage != null
                        && response.PayorMessage.Trim().Length > 0)
                    {
                        this.checkValidationResponse(response);
                    }
                    else
                    {
                        // error                        
                        this.textBox.Text = String.Format(
                            UIErrorMessages.EDV_SERVICE_UNAVAILABLE,
                            string.Empty,
                            string.Empty
                            );
                    }

                }
                else
                {
                    // no results available
                    // start the timer ( which checks the initiated time to determine if we still need to poll )

                    this.startResponseWaitTimer();

                }
            }
            else
            {
                if (this.Model_Coverage != null
                    && this.Model_Coverage.GetType() != typeof(WorkersCompensationCoverage))
                {
                    if (this.runRules())
                    {
                        this.btnInitiate.Enabled = true;
                    }
                }
            }
        }

        #endregion

        #region Properties
        [Browsable(false)]
        public Coverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (Coverage)this.Model;
            }
        }

        [Browsable(false)]
        public Account Account
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

        [Browsable(false)]
        public Button InitiateButton
        {
            get
            {
                return btnInitiate;
            }
        }
       
        #endregion

        #region Private Methods

        private void checkExistingResponse(BenefitsValidationResponse response)
        {
            if (response.ReturnedDataValidationTicket.BenefitsResponse.ResponseStatus.Oid == BenefitResponseStatus.REJECTED_OID)
            {
                if (this.runRules())
                {
                    this.btnInitiate.Enabled = true;
                }
                return;
            }

            if (response.ReturnedDataValidationTicket.BenefitsResponse.ResponseStatus.Oid == BenefitResponseStatus.ACCEPTED_OID
                || response.ReturnedDataValidationTicket.BenefitsResponse.ResponseStatus.Oid == BenefitResponseStatus.AUTO_ACCEPTED_OID)
            {
                this.textBox.Clear();

                this.textBox.Text = response.PayorMessage;

                if (ShowValidationResponseEvent != null)
                {
                    // Show the text version and parse the XML response to populate the Coverage domain object
                    // so that the VerificationEntry view is populated with a call to UpdateView.

                    ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                }

                if (this.runRules())
                {
                    this.btnInitiate.Enabled = true;
                }

                return;
            }
        }

        private void updateModelFromResponse()
        {
            // populate fields on other domain objects (e.g. Insured) from the response, including:

            if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredLastName != string.Empty)
            {
                this.Model_Coverage.Insured.Name.LastName = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredLastName;
            }
            if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredFirstName != string.Empty)
            {
                this.Model_Coverage.Insured.Name.FirstName = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredFirstName;
            }

            if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredMiddleInitial != string.Empty)
            {
                this.Model_Coverage.Insured.Name.MiddleInitial = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredMiddleInitial;
            }

            if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredDOB != string.Empty) // replace with some condition on the response DOB
            {
                if (DateValidator.IsValidDate(this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredDOB))
                {
                    this.Model_Coverage.Insured.DateOfBirth = DateTime.Parse(this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredDOB); // replace with Some value from the constraint
                }
            }

            Coverage aCoverage = this.Model_Coverage;

            if (aCoverage.GetType() == typeof(CommercialCoverage))
            {
                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID != string.Empty)
                {
                    ((CommercialCoverage)aCoverage).CertSSNID = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID;
                }
                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseGroupNumber != string.Empty)
                {
                    ((CommercialCoverage)aCoverage).GroupNumber = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseGroupNumber;
                }

                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCo != string.Empty)
                {
                    ((CommercialCoverage)aCoverage).Authorization.AuthorizationCompany = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCo;
                }

                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCoPhone != string.Empty)
                {
                    ((CommercialCoverage)aCoverage).Authorization.AuthorizationPhone = new PhoneNumber(this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCoPhone);
                }
            }
            else if (aCoverage.GetType() == typeof(GovernmentMedicaidCoverage))
            {
                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID != string.Empty)
                {
                    ((GovernmentMedicaidCoverage)aCoverage).PolicyCINNumber = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID;
                }
            }
            else if (aCoverage.GetType() == typeof(GovernmentMedicareCoverage))
            {
                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID != string.Empty)
                {
                    ((GovernmentMedicareCoverage)aCoverage).MBINumber = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID;
                }
            }
            else if (aCoverage.GetType() == typeof(GovernmentOtherCoverage))
            {
                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID != string.Empty)
                {
                    ((GovernmentOtherCoverage)aCoverage).CertSSNID = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID;
                }

                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseGroupNumber != string.Empty)
                {
                    ((GovernmentOtherCoverage)aCoverage).GroupNumber = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseGroupNumber;
                }

                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCo != string.Empty)
                {
                    ((GovernmentOtherCoverage)aCoverage).Authorization.AuthorizationCompany = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCo;
                }

                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCoPhone != string.Empty)
                {
                    ((GovernmentOtherCoverage)aCoverage).Authorization.AuthorizationPhone = new PhoneNumber(this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCoPhone);
                }

            }
            else if (aCoverage.GetType() == typeof(WorkersCompensationCoverage))
            {
                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID != string.Empty)
                {
                    ((WorkersCompensationCoverage)aCoverage).PolicyNumber = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID;
                }

                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCo != string.Empty)
                {
                    ((WorkersCompensationCoverage)aCoverage).Authorization.AuthorizationCompany = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCo;
                }

                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCoPhone != string.Empty)
                {
                    ((WorkersCompensationCoverage)aCoverage).Authorization.AuthorizationPhone = new PhoneNumber(this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseAuthCoPhone);
                }

            }
            else if (aCoverage.GetType() == typeof(SelfPayCoverage))
            {
                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID != string.Empty)
                {
                    aCoverage.Insured.SocialSecurityNumber = new SocialSecurityNumber(this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID); // "Some value from the constraint";
                }
            }
            else if (aCoverage.GetType() == typeof(OtherCoverage))
            {
                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID != string.Empty)
                {
                    ((OtherCoverage)aCoverage).CertSSNID = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID;
                }

                if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseGroupNumber != string.Empty)
                {
                    ((OtherCoverage)aCoverage).GroupNumber = this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseGroupNumber;
                }
            }
        }

        /// <summary>
        /// setCoverageConstraints - assign the coverage constraints generated from parsing the response from Data Validation... 
        /// </summary>
        private void setCoverageConstraints(CoverageConstraints constraints)
        {
            this.Model_Coverage.SetCoverageConstraints(constraints);

            this.updateModelFromResponse();
        }

        private BenefitsValidationResponse getValidationResponse()
        {
            IDataValidationBroker dataValidationBroker =
                BrokerFactory.BrokerOfType<IDataValidationBroker>();

            try
            {
                i_BenefitsValidationResponse = dataValidationBroker.GetBenefitsValidationResponse(
                      this.Model_Coverage.DataValidationTicket.TicketId,
                      User.GetCurrent().SecurityUser.UPN,
                      this.Model_Coverage.GetType());
            }

            catch (Exception ex)
            {
                this.btnInitiate.Enabled = true;
                this.textBox.Text = ex.Message;

                if (ShowValidationResponseEvent != null)
                {
                    // Show the text version and set the constraints to the parsed response from DV to populate the Coverage domain object
                    // so that the VerificationEntry view is populated via a call to UpdateView.

                    ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                }
            }

            return i_BenefitsValidationResponse;
        }

        private bool existingVerificationValues()
        {
            Coverage aCoverage = this.Model_Coverage;

            if (aCoverage.GetType() == typeof(CommercialCoverage)
                || aCoverage.GetType() == typeof(OtherCoverage))
            {
                CommercialCoverage cc = aCoverage as CommercialCoverage;

                if (cc.InformationReceivedSource != null
                    && cc.InformationReceivedSource.Description != string.Empty)
                {
                    return true;
                }
                if (cc.EligibilityPhone != string.Empty)
                {
                    return true;
                }
                if (cc.InsuranceCompanyRepName != string.Empty)
                {
                    return true;
                }
                if (cc.EffectiveDateForInsured != DateTime.MinValue)
                {
                    return true;
                }
                if (cc.TerminationDateForInsured != DateTime.MinValue)
                {
                    return true;
                }
                if (cc.ServiceForPreExistingCondition != null && cc.ServiceForPreExistingCondition.Code != string.Empty
                    && cc.ServiceForPreExistingCondition.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (cc.ServiceIsCoveredBenefit != null && cc.ServiceIsCoveredBenefit.Code != string.Empty
                    && cc.ServiceIsCoveredBenefit.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (cc.ClaimsAddressVerified != null && cc.ClaimsAddressVerified.Code != string.Empty
                    && cc.ClaimsAddressVerified.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (cc.CoordinationOfbenefits != null && cc.CoordinationOfbenefits.Code != string.Empty
                    && cc.CoordinationOfbenefits.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (cc.TypeOfVerificationRule != null
                    && cc.TypeOfVerificationRule.Description != string.Empty)
                {
                    return true;
                }
                if (cc.TypeOfProduct != null
                    && cc.TypeOfProduct.Description != string.Empty)
                {
                    return true;
                }
                if (cc.PPOPricingOrBroker != string.Empty)
                {
                    return true;
                }
                if (cc.FacilityContractedProvider != null && cc.FacilityContractedProvider.Code != string.Empty
                    && cc.FacilityContractedProvider.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (cc.AutoInsuranceClaimNumber != string.Empty)
                {
                    return true;
                }
                if (cc.AutoMedPayCoverage != null && cc.AutoMedPayCoverage.Code != string.Empty
                    && cc.AutoMedPayCoverage.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (cc.Remarks.Trim() != string.Empty)
                {
                    return true;
                }

                // spin the Benefit Category Details...

                ArrayList alCategories = new ArrayList(cc.BenefitsCategories.Values);

                foreach (BenefitsCategoryDetails bcd in alCategories)
                {
                    if (bcd.Deductible > -1)
                    {
                        return true;
                    }
                    if (bcd.TimePeriod != null && bcd.TimePeriod.Code != string.Empty)
                    {
                        return true;
                    }
                    if (bcd.DeductibleMet != null && bcd.DeductibleMet.Code != string.Empty
                        && bcd.DeductibleMet.Code != YesNoFlag.CODE_BLANK)
                    {
                        return true;
                    }
                    if (bcd.DeductibleDollarsMet > -1)
                    {
                        return true;
                    }
                    if (bcd.CoInsurance > -1)
                    {
                        return true;
                    }
                    if (bcd.OutOfPocket > -1)
                    {
                        return true;
                    }
                    if (bcd.OutOfPocketMet != null && bcd.OutOfPocketMet.Code != string.Empty
                        && bcd.OutOfPocketMet.Code != YesNoFlag.CODE_BLANK)
                    {
                        return true;
                    }
                    if (bcd.OutOfPocketDollarsMet > -1)
                    {
                        return true;
                    }
                    if (bcd.AfterOutOfPocketPercent > -1)
                    {
                        return true;
                    }
                    if (bcd.CoPay > -1)
                    {
                        return true;
                    }
                    if (bcd.WaiveCopayIfAdmitted != null && bcd.WaiveCopayIfAdmitted.Code != string.Empty
                        && bcd.WaiveCopayIfAdmitted.Code != YesNoFlag.CODE_BLANK)
                    {
                        return true;
                    }
                    if (bcd.VisitsPerYear > -1)
                    {
                        return true;
                    }
                    if (bcd.LifeTimeMaxBenefit > -1)
                    {
                        return true;
                    }
                    if (bcd.RemainingLifetimeValue > -1)
                    {
                        return true;
                    }
                    if (bcd.RemainingLifetimeValueMet != null && bcd.RemainingLifetimeValueMet.Code != string.Empty
                        && bcd.RemainingLifetimeValueMet.Code != YesNoFlag.CODE_BLANK)
                    {
                        return true;
                    }
                    if (bcd.MaxBenefitPerVisit > -1)
                    {
                        return true;
                    }
                    if (bcd.RemainingBenefitPerVisits > -1)
                    {
                        return true;
                    }
                    if (bcd.RemainingBenefitPerVisitsMet != null && bcd.RemainingBenefitPerVisitsMet.Code != string.Empty
                        && bcd.RemainingBenefitPerVisitsMet.Code != YesNoFlag.CODE_BLANK)
                    {
                        return true;
                    }
                }
            }
            else if (aCoverage.GetType() == typeof(GovernmentMedicaidCoverage))
            {
                GovernmentMedicaidCoverage gmc = aCoverage as GovernmentMedicaidCoverage;

                if (gmc.EligibilityDate != DateTime.MinValue)
                {
                    return true;
                }
                if (gmc.PatienthasMedicare != null && gmc.PatienthasMedicare.Code != string.Empty
                    && gmc.PatienthasMedicare.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (gmc.PatienthasOtherInsuranceCoverage != null && gmc.PatienthasOtherInsuranceCoverage.Code != string.Empty
                    && gmc.PatienthasOtherInsuranceCoverage.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (gmc.MedicaidCopay > -1)
                {
                    return true;
                }
                if (gmc.EVCNumber != string.Empty)
                {
                    return true;
                }
                if (gmc.InformationReceivedSource != null
                    && gmc.InformationReceivedSource.Description != string.Empty)
                {
                    return true;
                }
                if (gmc.Remarks.Trim() != string.Empty)
                {
                    return true;
                }
            }
            else if (aCoverage.GetType() == typeof(GovernmentMedicareCoverage))
            {
                GovernmentMedicareCoverage gmc = aCoverage as GovernmentMedicareCoverage;

                if (gmc.PartACoverage != null && gmc.PartACoverage.Code != string.Empty
                    && gmc.PartACoverage.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (gmc.PartACoverageEffectiveDate != DateTime.MinValue)
                {
                    return true;
                }
                if (gmc.PartBCoverage != null && gmc.PartBCoverage.Code != string.Empty
                    && gmc.PartBCoverage.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (gmc.PartBCoverageEffectiveDate != DateTime.MinValue)
                {
                    return true;
                }
                if (gmc.PatientHasMedicareHMOCoverage != null && gmc.PatientHasMedicareHMOCoverage.Code != string.Empty
                    && gmc.PatientHasMedicareHMOCoverage.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (gmc.MedicareIsSecondary != null && gmc.MedicareIsSecondary.Code != string.Empty
                    && gmc.MedicareIsSecondary.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (gmc.DateOfLastBillingActivity != DateTime.MinValue)
                {
                    return true;
                }
                if (gmc.RemainingBenefitPeriod > -1)
                {
                    return true;
                }
                if (gmc.RemainingCoInsurance > -1)
                {
                    return true;
                }
                if (gmc.RemainingLifeTimeReserve > -1)
                {
                    return true;
                }
                if (gmc.RemainingSNF > -1)
                {
                    return true;
                }
                if (gmc.RemainingSNFCoInsurance > -1)
                {
                    return true;
                }
                if (gmc.RemainingPartADeductible > -1)
                {
                    return true;
                }
                if (gmc.RemainingPartBDeductible > -1)
                {
                    return true;
                }
                if (gmc.PatientIsPartOfHospiceProgram != null && gmc.PatientIsPartOfHospiceProgram.Code != string.Empty
                    && gmc.PatientIsPartOfHospiceProgram.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (gmc.VerifiedBeneficiaryName != null && gmc.VerifiedBeneficiaryName.Code != string.Empty
                    && gmc.VerifiedBeneficiaryName.Code != YesNoFlag.CODE_BLANK)
                {
                    return true;
                }
                if (gmc.InformationReceivedSource != null
                    && gmc.InformationReceivedSource.Description != string.Empty)
                {
                    return true;
                }
                if (gmc.Remarks.Trim() != string.Empty)
                {
                    return true;
                }
            }
            else if (aCoverage.GetType() == typeof(GovernmentOtherCoverage))
            {
                GovernmentOtherCoverage goc = aCoverage as GovernmentOtherCoverage;

                if (goc.InformationReceivedSource != null
                    && goc.InformationReceivedSource.Description != string.Empty)
                {
                    return true;
                }
                if (goc.EligibilityPhone != string.Empty)
                {
                    return true;
                }
                if (goc.InsuranceCompanyRepName != string.Empty)
                {
                    return true;
                }
                if (goc.TypeOfCoverage != string.Empty)
                {
                    return true;
                }
                if (goc.EffectiveDateForInsured != DateTime.MinValue)
                {
                    return true;
                }
                if (goc.TerminationDateForInsured != DateTime.MinValue)
                {
                    return true;
                }
                if (goc.Remarks.Trim() != string.Empty)
                {
                    return true;
                }

                // spin the Benefit Category Details...

                ArrayList alCategories = new ArrayList(goc.BenefitsCategories.Values);

                foreach (BenefitsCategoryDetails bcd in alCategories)
                {
                    if (bcd.Deductible > -1)
                    {
                        return true;
                    }
                    if (bcd.DeductibleMet != null && bcd.DeductibleMet.Code != string.Empty
                        && bcd.DeductibleMet.Code != YesNoFlag.CODE_BLANK)
                    {
                        return true;
                    }
                    if (bcd.DeductibleDollarsMet > -1)
                    {
                        return true;
                    }
                    if (bcd.CoInsurance > -1)
                    {
                        return true;
                    }
                    if (bcd.OutOfPocket > -1)
                    {
                        return true;
                    }
                    if (bcd.OutOfPocketMet != null && bcd.OutOfPocketMet.Code != string.Empty
                        && bcd.OutOfPocketMet.Code != YesNoFlag.CODE_BLANK)
                    {
                        return true;
                    }
                    if (bcd.OutOfPocketDollarsMet > -1)
                    {
                        return true;
                    }
                    if (bcd.AfterOutOfPocketPercent > -1)
                    {
                        return true;
                    }
                    if (bcd.CoPay > -1)
                    {
                        return true;
                    }
                }
            }
            else if (aCoverage.GetType() == typeof(WorkersCompensationCoverage))
            {

            }
            else if (aCoverage.GetType() == typeof(SelfPayCoverage))
            {

            }

            return false;
        }

        private bool initiateBenefitsValidation()
        {
            bool rc = false;


            IDataValidationBroker dataValidationBroker =
                BrokerFactory.BrokerOfType<IDataValidationBroker>();

            DataValidationTicket dvt = null;

            AccountDetailsRequest accountDetailsRequest = new AccountDetailsRequest();
            accountDetailsRequest = this.CreateBenefitsRequestFrom(accountDetailsRequest);

            // determine if we will overwrite existing values

            if (this.existingVerificationValues())
            {
                DialogResult dr = MessageBox.Show(EXISTING_BENEFITS, "Benefits Verification", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr != DialogResult.Yes)
                {
                    return false;
                }
            }

            // compare the current request to the previous request (if existing)

            bool blnSameRequest = false;

            if (this.Model_Coverage.DataValidationTicket != null && this.Model_Coverage.DataValidationTicket.BenefitsResponse != null &&
                this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseStatus.Oid == BenefitResponseStatus.REJECTED_OID &&
                accountDetailsRequest.InsuredFirstName == this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestInsuredFirstName &&
                accountDetailsRequest.InsuredMiddleInitial == this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestInsuredMiddleInitial &&
                accountDetailsRequest.InsuredLastName == this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestInsuredLastName &&
                accountDetailsRequest.InsuredDOB.ToString("MM/dd/yyyy") == this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestInsuredDOB &&
                accountDetailsRequest.RequestPayorName == this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestPayorName &&
                accountDetailsRequest.CoverageMemberId == this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestSubscriberID &&
                accountDetailsRequest.InsuredGroupNumber == this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseGroupNumber
                )
            {
                blnSameRequest = true;
            }

            if (blnSameRequest)
            {
                // show a message box indicating that the results will likely be the same

                StringBuilder sb = new StringBuilder();
                sb.Append("There has been no change to the qualifying fields shown below. ");
                sb.Append("The previous source results will be displayed again.\r\n");
                sb.Append("To generate new source results, change one or more of the following fields:\r\n\r\n");
                sb.Append("    Insured name\r\n");
                sb.Append("    Insured DOB\r\n");

                Coverage aCoverage = this.Model_Coverage;

                if (aCoverage.GetType() == typeof(CommercialCoverage))
                {
                    sb.Append("     Cert/SSN/ID\r\n");
                }
                else if (aCoverage.GetType() == typeof(GovernmentMedicaidCoverage))
                {
                    sb.Append("     Policy/CIN\r\n");
                }
                else if (aCoverage.GetType() == typeof(GovernmentMedicareCoverage))
                {
                    sb.Append("     MBI number\r\n");
                }
                else if (aCoverage.GetType() == typeof(GovernmentOtherCoverage))
                {
                    sb.Append("     Cert/SSN/ID\r\n");
                }
                else if (aCoverage.GetType() == typeof(WorkersCompensationCoverage))
                {
                    sb.Append("     Policy number\r\n");
                }
                else if (aCoverage.GetType() == typeof(SelfPayCoverage))
                {
                    sb.Append("     SSN\r\n");
                }
                else if (aCoverage.GetType() == typeof(OtherCoverage))
                {
                    sb.Append("     Cert/SSN/ID\r\n");
                }

                sb.Append("    Payor\r\n");
                sb.Append("    Group number\r\n\r\n");
                sb.Append("Proceed with Benefits Verification?");

                DialogResult dr = MessageBox.Show(sb.ToString(), "Benefits Verification", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr != DialogResult.Yes)
                {
                    return false;
                }
            }

            this.Model_Coverage.BenefitsVerified = new YesNotApplicableFlag();

            try
            {
                c_log.Info("IVP - Get DataValidationTicket");
                dvt = dataValidationBroker.InitiateBenefitsValidation(accountDetailsRequest);
            }
            catch (SoapException se)
            {
                this.btnInitiate.Enabled = true;
                this.textBox.Text = se.Detail.ToString();

                if (ShowValidationResponseEvent != null)
                {
                    ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                }

            }
            catch (Exception ex)
            {
                this.btnInitiate.Enabled = true;

                if (ex.ToString().Contains(UIErrorMessages.EDV_UNAVAILABLE_ERROR))
                {
                    this.textBox.Text = UIErrorMessages.EDV_UNAVAILABLE_MESSAGE;
                }
                else
                {
                    this.textBox.Text = String.Format(UIErrorMessages.EDV_SERVICE_UNAVAILABLE, Environment.NewLine, ex);
                }

                if (ShowValidationResponseEvent != null)
                {
                    ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                }
                c_log.Info("IVP - FAILED to get DataValidationTicket!!!!");
                return false;
            }

            if (dvt != null)
            {
                rc = true;
                this.ChangeBenefitsResponseToUppercase(dvt);
                Model_Coverage.DataValidationTicket = dvt;
            }

            c_log.Info("IVP - Start response wait timer");
            startResponseWaitTimer();
            btnInitiate.Enabled = false;

            return rc;
        }

        private void checkValidationResponse(BenefitsValidationResponse response)
        {
            if (response.ReturnedDataValidationTicket.BenefitsResponse.ResponseStatus.Oid == BenefitResponseStatus.REJECTED_OID ||
                response.ReturnedDataValidationTicket.BenefitsResponse.ResponseStatus.Oid == BenefitResponseStatus.ACCEPTED_OID ||
                response.ReturnedDataValidationTicket.BenefitsResponse.ResponseStatus.Oid == BenefitResponseStatus.AUTO_ACCEPTED_OID)
            {
                // replace the ticket retrieved from the DB with the one we currently have... it may have already been updated

                this.Model_Coverage.DataValidationTicket = response.ReturnedDataValidationTicket;

                this.checkExistingResponse(response);
                return;
            }

            // if the data validation failed to return a response, show the message and return

            if (!response.IsSuccess)
            {
                this.textBox.Clear();

                this.textBox.Text = response.PayorMessage;

                if (ShowValidationResponseEvent != null)
                {
                    // Show the text version and set the constraints to the parsed response from DV to populate the Coverage domain object
                    // so that the VerificationEntry view is populated via a call to UpdateView.

                    ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                }

                if (this.runRules())
                {
                    this.btnInitiate.Enabled = true;
                }

                return;
            }

            // if we make it this far, we have a resposne and it was not Accepted, Auto-Accepted, or Rejected
            // it must, therefore, have a status of Unknown or Deferred
            // determine if we display the confirmation based on match criteria and/or the presence of both in- and out-of-network benefits

            bool blnDisplayConfirmation = true;

            if (
                this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestInsuredFirstName == this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredFirstName &&
                this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestInsuredMiddleInitial == this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredMiddleInitial &&
                this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestInsuredLastName == this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredLastName &&
                this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestInsuredDOB == this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseInsuredDOB &&
                this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestPayorName == this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponsePayorName &&
                this.Model_Coverage.DataValidationTicket.BenefitsResponse.RequestSubscriberID == this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseSubscriberID
                )
            {
                blnDisplayConfirmation = false;
            }

            if (blnDisplayConfirmation)
            {
                // we have a match... if the coverage category indicates Commercial or Managed Care,
                // only show the confirmation if both in-network and out-of-network values are returned

                if (this.Model_Coverage.GetType() == typeof(CommercialCoverage))
                {
                    if (response.CoverageConstraintsCollection.Count > 1)
                    {
                        blnDisplayConfirmation = true;
                    }
                }
            }

            if (!blnDisplayConfirmation)
            {
                this.textBox.Clear();

                this.textBox.Text = response.PayorMessage;

                if (i_BenefitsValidationResponse != null && i_BenefitsValidationResponse.CoverageConstraintsCollection != null
                    && i_BenefitsValidationResponse.CoverageConstraintsCollection.Count > 0)
                {
                    this.setCoverageConstraints(i_BenefitsValidationResponse.CoverageConstraintsCollection[0] as CoverageConstraints);
                }

                this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseStatus = BenefitResponseStatus.NewAutoAcceptedStatus();

                IInfoReceivedSourceBroker IRSBroker = BrokerFactory.BrokerOfType<IInfoReceivedSourceBroker>();

                this.Model_Coverage.InformationReceivedSource = IRSBroker.InfoReceivedSourceWith(InformationReceivedSource.SYSTEM_VERIFICATION_OID.ToString());
                this.Model_Coverage.WasElectronicallyVerifiedDuringCurrentActivity = true;

                if (ShowValidationResponseEvent != null)
                {
                    // Show the text version and set the constraints to the parsed response from DV to populate the Coverage domain object
                    // so that the VerificationEntry view is populated via a call to UpdateView.

                    ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                }

                if (this.runRules())
                {
                    this.btnInitiate.Enabled = true;
                }
            }
            else
            {
                // display the confirmation dialog

                using (BenefitsResponseConfirmation brc = new BenefitsResponseConfirmation())
                {

                    brc.Model = this.Model_Coverage;
                    brc.Model_BenefitsResponse = response;
                    brc.UpdateView();

                    DialogResult result = brc.ShowDialog();

                    if (result == DialogResult.Yes)
                    {
                        this.textBox.Clear();

                        this.textBox.Text = response.PayorMessage;

                        // Show the text version and set the constraints to the parsed response from DV to populate the Coverage domain object
                        // so that the VerificationEntry view is populated via a call to UpdateView.

                        if (this.Model_Coverage.DataValidationTicket.BenefitsResponse.ResponseStatus.Oid == BenefitResponseStatus.ACCEPTED_OID)
                        {
                            IInfoReceivedSourceBroker IRSBroker = BrokerFactory.BrokerOfType<IInfoReceivedSourceBroker>();

                            this.Model_Coverage.InformationReceivedSource = IRSBroker.InfoReceivedSourceWith(InformationReceivedSource.SYSTEM_VERIFICATION_OID.ToString());
                            this.Model_Coverage.WasElectronicallyVerifiedDuringCurrentActivity = true;

                            this.updateModelFromResponse();

                            this.textBox.Clear();

                            this.textBox.Text = response.PayorMessage;

                            if (ShowValidationResponseEvent != null)
                            {
                                ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                            }

                            if (this.runRules())
                            {
                                this.btnInitiate.Enabled = true;
                            }
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        this.textBox.Clear();

                        if (ShowValidationResponseEvent != null)
                        {
                            ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                        }

                        if (this.runRules())
                        {
                            this.btnInitiate.Enabled = true;
                        }
                    }
                }
            }

            // Update the DB with the current status of this request

            IDataValidationBroker aBroker = BrokerFactory.BrokerOfType<IDataValidationBroker>();
            aBroker.SaveDataValidationTicket(this.Model_Coverage.DataValidationTicket);
        }

        private AccountDetailsRequest CreateBenefitsRequestFrom(AccountDetailsRequest request)
        {
            Coverage aCoverage = this.Model_Coverage;
            Account account = AccountView.GetInstance().Model_Account;

            if (account == null && aCoverage.Account != null && aCoverage.Account.Activity != null
                && (aCoverage.Account.Activity.IsTransferOutpatientToERActivity()
                || aCoverage.Account.Activity.IsTransferERToOutpatientActivity()))
            {
                account = aCoverage.Account ;
            }

            if (aCoverage != null && account != null)
            {
                // Account Details
                request.AccountNumber = account.AccountNumber;
                request.FacilityOid = account.Facility.Oid;
                request.AdmitDate = account.AdmitDate;

                // Patient Details
                request.PatientInsuredRelationship = account.Patient.RelationshipWith(aCoverage.Insured);
                request.MedicalRecordNumber = account.Patient.MedicalRecordNumber;
                request.PatientDOB = account.Patient.DateOfBirth;
                request.PatientFirstName = account.Patient.FirstName;
                request.PatientLastName = account.Patient.LastName;
                request.PatientMidInitial = account.Patient.MiddleInitial;
                request.PatientSex = account.Patient.Sex.Code;
                request.PatientSSN = account.Patient.SocialSecurityNumber;

                // Insured Details
                Insured insured = aCoverage.Insured;

                request.InsuredDOB = insured.DateOfBirth;
                request.InsuredFirstName = insured.FirstName;
                request.InsuredLastName = insured.LastName;

                // OTD# 37121 - Assign InsuredGroupNumber from the coverage through CoverageDetailsFor() 
                // method below since we do not have any GroupNumber field on the Insured tab
                request.InsuredSex = insured.Sex.Code;
                request.InsuredPhysicalCP =
                    insured.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());

                // Coverage Details
                request = this.CoverageDetailsFor(request, aCoverage);
                request.RequestPayorName = aCoverage.InsurancePlan.Payor.Name;
            }
            else
            {
                request.IsCoverageNull = true;
                request.IsAccountNull = true;
            }

            User appUser = User.GetCurrent();
            if (appUser != null)
            {
                request.Upn = appUser.SecurityUser.UPN;
            }
            else
            {
                request.IsUserNull = true;
            }

            return request;
        }

        private AccountDetailsRequest CoverageDetailsFor(AccountDetailsRequest request, Coverage aCoverage)
        {
            request.TypeOfCoverage = aCoverage.GetType().ToString();

            if (aCoverage.GetType() == typeof(CommercialCoverage))
            {
                request.CoverageMemberId = ((CommercialCoverage)aCoverage).CertSSNID;
                request.InsuredGroupNumber = ((CommercialCoverage)aCoverage).GroupNumber;
            }
            else if (aCoverage.GetType() == typeof(GovernmentMedicaidCoverage))
            {
                request.CoverageMemberId = ((GovernmentMedicaidCoverage)aCoverage).PolicyCINNumber;
                request.CoverageIssueDate = ((GovernmentMedicaidCoverage)aCoverage).IssueDate;
            }
            else if (aCoverage.GetType() == typeof(GovernmentMedicareCoverage))
            {
                request.CoverageMemberId = ((GovernmentMedicareCoverage)aCoverage).MBINumber;
            }
            else if (aCoverage.GetType() == typeof(GovernmentOtherCoverage))
            {
                request.CoverageMemberId = ((GovernmentOtherCoverage)aCoverage).CertSSNID;
                request.InsuredGroupNumber = ((GovernmentOtherCoverage)aCoverage).GroupNumber;
            }
            else if (aCoverage.GetType() == typeof(WorkersCompensationCoverage))
            {
                request.CoverageMemberId = ((WorkersCompensationCoverage)aCoverage).PolicyNumber;
            }
            else if (aCoverage.GetType() == typeof(SelfPayCoverage))
            {
                request.CoverageMemberId = String.Empty;
            }
            else if (aCoverage.GetType() == typeof(OtherCoverage))
            {
                request.CoverageMemberId = ((OtherCoverage)aCoverage).CertSSNID;
                request.InsuredGroupNumber = ((OtherCoverage)aCoverage).GroupNumber;
            }
            else
            {
                request.CoverageMemberId = String.Empty;
            }

            request.CoverageInsurancePlanId = aCoverage.InsurancePlan.PlanID;
            request.CoverageOrderOid = aCoverage.CoverageOrder.Oid;

            return request;
        }

        /// <summary>
        /// startResponseWaitTimer - start the timer to wait for the response
        /// </summary>
        private void startResponseWaitTimer()
        {
            i_ResponseWaitTimer.Tick -= this.ResponseWaitTimer_Tick;
            textBox.Clear();
            textBox.Text = RESPONSE_PENDING;
            if (ShowValidationResponseEvent != null)
            {
                ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
            }

            int duration = 0;

            c_log.InfoFormat("IVP - Initiated On datetime = {0}", Model_Coverage.DataValidationTicket.InitiatedOn.ToString("MMddyyyy hh:mm"));
            c_log.InfoFormat("IVP - Initiated On plus 5 minutes = {0}", Model_Coverage.DataValidationTicket.InitiatedOn.AddMinutes(5).ToString("MMddyyyy hh:mm"));

            DateTime currentFacDT = this.GetCurrentFacilityDateTime(Model_Coverage.DataValidationTicket.Facility.GMTOffset,
                                                                     Model_Coverage.DataValidationTicket.Facility.DSTOffset);

            if (Model_Coverage.DataValidationTicket.InitiatedOn.AddMinutes(5) > currentFacDT)
            {
                TimeSpan ts = currentFacDT.Subtract(Model_Coverage.DataValidationTicket.InitiatedOn);
                duration = RESPONSE_WAIT_DURATION - Convert.ToInt32(ts.TotalMilliseconds);
            }
            else
            {
                duration = 0;
            }

            c_log.Info("IVP - response wait duration = " + Convert.ToString(duration));

            if (duration == 0)
            {
                textBox.Clear();

                if (ShowValidationResponseEvent != null)
                {
                    ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
                }
                btnInitiate.Enabled = true;
                return;
            }

            i_ResponseWaitTimer.Start();
            i_ResponseWaitTimer.Enabled = true;
            i_ResponseWaitTimer.Interval = duration;
            i_ResponseWaitTimer.Tick += this.ResponseWaitTimer_Tick;

            c_log.Info("IVP - Start response poll timer");
            startResponsePollTimer();
        }

        private DateTime GetCurrentFacilityDateTime(int gmtOffset, int dstOffset)
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt(gmtOffset, dstOffset);
        }

        private bool runRules()
        {
            Account anAccount = AccountView.GetInstance().Model_Account;

            string requiredFields = RuleEngine.GetInstance().GetCompositeSummary(anAccount, Model_Coverage, BENEFITS_VALIDATION);

            if (requiredFields != String.Empty)
            {
                requiredFieldSummary = new RequiredFieldsDialog();

                try
                {
                    requiredFieldSummary.ErrorText = requiredFields;
                    if (Model_Coverage.InsurancePlan.PlanType != null)
                    {
                        requiredFieldSummary.HeaderText = VALIDATION_SUMMARY_HEADER + Model_Coverage.InsurancePlan.PlanCategory.Description;
                    }
                    requiredFieldSummary.ShowDialog(this);

                    return false;
                }
                finally
                {
                    requiredFieldSummary.Dispose();
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// startResponsePollTimer - start the timer to poll every few (currently 10) seconds
        /// </summary>
        private void startResponsePollTimer()
        {
            this.i_ResponsePollTimer.Tick -= this.ResponsePollTimer_Tick;
            this.textBox.Clear();
            this.textBox.Text = RESPONSE_PENDING;
            if (ShowValidationResponseEvent != null)
            {
                ShowValidationResponseEvent(this, new LooseArgs(this.textBox.Text));
            }

            c_log.Info("IVP - Response poll interval = " + Convert.ToString(RESPONSE_POLL_INTERVAL));

            this.i_ResponsePollTimer.Start();
            this.i_ResponsePollTimer.Enabled = true;
            this.i_ResponsePollTimer.Interval = RESPONSE_POLL_INTERVAL;
            this.i_ResponsePollTimer.Tick += this.ResponsePollTimer_Tick;
        }

        /// <summary>
        /// stopResponsePollTimer - stop the polling timer
        /// </summary>
        private void StopResponsePollTimer()
        {
            if (this.i_ResponsePollTimer != null)
            {
                this.i_ResponsePollTimer.Stop();
                this.i_ResponsePollTimer.Enabled = false;
            }
        }


        private void ChangeBenefitsResponseToUppercase(BenefitsValidationResponse response)
        {
            DataValidationTicket dataValidationTicket = response.ReturnedDataValidationTicket;
            if (dataValidationTicket != null)
            {
                ChangeBenefitsResponseToUppercase(dataValidationTicket);
            }
        }


        private void ChangeBenefitsResponseToUppercase(DataValidationTicket dataValidationTicket)
        {
            DataValidationBenefitsResponse benefitsResponse = dataValidationTicket.BenefitsResponse;

            if (benefitsResponse != null)
            {
                PbarHelper.ChangeLogicalStringPropertiesToUpperCase(benefitsResponse);
            }
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox = new System.Windows.Forms.TextBox();
            this.lblStaticName = new System.Windows.Forms.Label();
            this.lblStaticType = new System.Windows.Forms.Label();
            this.lblStaticComplaint = new System.Windows.Forms.Label();
            this.lblStaticDOB = new System.Windows.Forms.Label();
            this.lblStaticFinClass = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.lblChiefComplaint = new System.Windows.Forms.Label();
            this.lblFinancialClass = new System.Windows.Forms.Label();
            this.lblDOB = new System.Windows.Forms.Label();
            this.lblStaticAccount = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblStaticHospService = new System.Windows.Forms.Label();
            this.lblHospitalService = new System.Windows.Forms.Label();
            this.lblStaticProvider = new System.Windows.Forms.Label();
            this.lblMedicaidProvider = new System.Windows.Forms.Label();
            this.lblStaticFacilityId = new System.Windows.Forms.Label();
            this.lblFacilityID = new System.Windows.Forms.Label();
            this.lblStaticVerification = new System.Windows.Forms.Label();
            this.btnInitiate = new LoggingButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(0, 0);
            this.textBox.Name = "textBox";
            this.textBox.TabIndex = 0;
            this.textBox.Text = "";
            // 
            // lblStaticName
            // 
            this.lblStaticName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticName.Location = new System.Drawing.Point(8, 4);
            this.lblStaticName.Name = "lblStaticName";
            this.lblStaticName.Size = new System.Drawing.Size(84, 16);
            this.lblStaticName.TabIndex = 0;
            this.lblStaticName.Text = "Patient name:";
            // 
            // lblStaticType
            // 
            this.lblStaticType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticType.Location = new System.Drawing.Point(680, 4);
            this.lblStaticType.Name = "lblStaticType";
            this.lblStaticType.Size = new System.Drawing.Size(75, 16);
            this.lblStaticType.TabIndex = 0;
            this.lblStaticType.Text = "Patient type:";
            // 
            // lblStaticComplaint
            // 
            this.lblStaticComplaint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticComplaint.Location = new System.Drawing.Point(8, 41);
            this.lblStaticComplaint.Name = "lblStaticComplaint";
            this.lblStaticComplaint.Size = new System.Drawing.Size(95, 12);
            this.lblStaticComplaint.TabIndex = 0;
            this.lblStaticComplaint.Text = "Chief complaint:";
            // 
            // lblStaticDOB
            // 
            this.lblStaticDOB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticDOB.Location = new System.Drawing.Point(414, 4);
            this.lblStaticDOB.Name = "lblStaticDOB";
            this.lblStaticDOB.Size = new System.Drawing.Size(35, 16);
            this.lblStaticDOB.TabIndex = 0;
            this.lblStaticDOB.Text = "DOB:";
            // 
            // lblStaticFinClass
            // 
            this.lblStaticFinClass.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticFinClass.Location = new System.Drawing.Point(8, 22);
            this.lblStaticFinClass.Name = "lblStaticFinClass";
            this.lblStaticFinClass.Size = new System.Drawing.Size(97, 14);
            this.lblStaticFinClass.TabIndex = 0;
            this.lblStaticFinClass.Text = "Financial class:";
            // 
            // lblPatientName
            // 
            this.lblPatientName.Location = new System.Drawing.Point(103, 4);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(305, 16);
            this.lblPatientName.TabIndex = 0;
            // 
            // lblPatientType
            // 
            this.lblPatientType.Location = new System.Drawing.Point(768, 4);
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size(105, 16);
            this.lblPatientType.TabIndex = 0;
            // 
            // lblChiefComplaint
            // 
            this.lblChiefComplaint.Location = new System.Drawing.Point(103, 41);
            this.lblChiefComplaint.Name = "lblChiefComplaint";
            this.lblChiefComplaint.Size = new System.Drawing.Size(513, 12);
            this.lblChiefComplaint.TabIndex = 0;
            // 
            // lblFinancialClass
            // 
            this.lblFinancialClass.Location = new System.Drawing.Point(103, 22);
            this.lblFinancialClass.Name = "lblFinancialClass";
            this.lblFinancialClass.Size = new System.Drawing.Size(172, 14);
            this.lblFinancialClass.TabIndex = 0;
            // 
            // lblDOB
            // 
            this.lblDOB.Location = new System.Drawing.Point(452, 4);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(68, 16);
            this.lblDOB.TabIndex = 0;
            this.lblDOB.Tag = "";
            // 
            // lblStaticAccount
            // 
            this.lblStaticAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticAccount.Location = new System.Drawing.Point(536, 4);
            this.lblStaticAccount.Name = "lblStaticAccount";
            this.lblStaticAccount.Size = new System.Drawing.Size(56, 16);
            this.lblStaticAccount.TabIndex = 0;
            this.lblStaticAccount.Text = "Account:";
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point(600, 4);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(69, 16);
            this.lblAccount.TabIndex = 0;
            this.lblAccount.Tag = "";
            // 
            // lblStaticHospService
            // 
            this.lblStaticHospService.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticHospService.Location = new System.Drawing.Point(320, 22);
            this.lblStaticHospService.Name = "lblStaticHospService";
            this.lblStaticHospService.Size = new System.Drawing.Size(95, 14);
            this.lblStaticHospService.TabIndex = 0;
            this.lblStaticHospService.Text = "Hospital service:";
            // 
            // lblHospitalService
            // 
            this.lblHospitalService.Location = new System.Drawing.Point(424, 22);
            this.lblHospitalService.Name = "lblHospitalService";
            this.lblHospitalService.Size = new System.Drawing.Size(224, 14);
            this.lblHospitalService.TabIndex = 0;
            // 
            // lblStaticProvider
            // 
            this.lblStaticProvider.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticProvider.Location = new System.Drawing.Point(631, 41);
            this.lblStaticProvider.Name = "lblStaticProvider";
            this.lblStaticProvider.Size = new System.Drawing.Size(119, 12);
            this.lblStaticProvider.TabIndex = 0;
            this.lblStaticProvider.Text = "Medicaid provider no:";
            // 
            // lblMedicaidProvider
            // 
            this.lblMedicaidProvider.Location = new System.Drawing.Point(760, 41);
            this.lblMedicaidProvider.Name = "lblMedicaidProvider";
            this.lblMedicaidProvider.Size = new System.Drawing.Size(112, 12);
            this.lblMedicaidProvider.TabIndex = 0;
            // 
            // lblStaticFacilityId
            // 
            this.lblStaticFacilityId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticFacilityId.Location = new System.Drawing.Point(671, 22);
            this.lblStaticFacilityId.Name = "lblStaticFacilityId";
            this.lblStaticFacilityId.Size = new System.Drawing.Size(84, 14);
            this.lblStaticFacilityId.TabIndex = 0;
            this.lblStaticFacilityId.Text = "Facility tax ID:";
            // 
            // lblFacilityID
            // 
            this.lblFacilityID.Location = new System.Drawing.Point(768, 22);
            this.lblFacilityID.Name = "lblFacilityID";
            this.lblFacilityID.Size = new System.Drawing.Size(89, 14);
            this.lblFacilityID.TabIndex = 0;
            // 
            // lblStaticVerification
            // 
            this.lblStaticVerification.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(204)), ((System.Byte)(204)), ((System.Byte)(204)));
            this.lblStaticVerification.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticVerification.Location = new System.Drawing.Point(8, 66);
            this.lblStaticVerification.Name = "lblStaticVerification";
            this.lblStaticVerification.Size = new System.Drawing.Size(224, 15);
            this.lblStaticVerification.TabIndex = 0;
            this.lblStaticVerification.Text = "System Electronic Verification Results";
            // 
            // btnInitiate
            // 
            this.btnInitiate.BackColor = System.Drawing.SystemColors.Control;
            this.btnInitiate.Location = new System.Drawing.Point(248, 2);
            this.btnInitiate.Name = "btnInitiate";
            this.btnInitiate.Size = new System.Drawing.Size(75, 22);
            this.btnInitiate.TabIndex = 1;
            this.btnInitiate.Text = "Initiate";
            this.btnInitiate.Click += new System.EventHandler(this.btnInitiate_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblStaticVerification);
            this.panel1.Controls.Add(this.lblFacilityID);
            this.panel1.Controls.Add(this.lblStaticFacilityId);
            this.panel1.Controls.Add(this.lblMedicaidProvider);
            this.panel1.Controls.Add(this.lblStaticProvider);
            this.panel1.Controls.Add(this.lblHospitalService);
            this.panel1.Controls.Add(this.lblStaticHospService);
            this.panel1.Controls.Add(this.lblAccount);
            this.panel1.Controls.Add(this.lblStaticAccount);
            this.panel1.Controls.Add(this.lblDOB);
            this.panel1.Controls.Add(this.lblFinancialClass);
            this.panel1.Controls.Add(this.lblChiefComplaint);
            this.panel1.Controls.Add(this.lblPatientType);
            this.panel1.Controls.Add(this.lblPatientName);
            this.panel1.Controls.Add(this.lblStaticFinClass);
            this.panel1.Controls.Add(this.lblStaticDOB);
            this.panel1.Controls.Add(this.lblStaticComplaint);
            this.panel1.Controls.Add(this.lblStaticType);
            this.panel1.Controls.Add(this.lblStaticName);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(885, 88);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(204)), ((System.Byte)(204)), ((System.Byte)(204)));
            this.panel2.Controls.Add(this.btnInitiate);
            this.panel2.Location = new System.Drawing.Point(0, 59);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(885, 30);
            this.panel2.TabIndex = 2;
            // 
            // VerificationCoverage
            // 
            this.AcceptButton = this.btnInitiate;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.Name = "VerificationCoverage";
            this.Size = new System.Drawing.Size(885, 93);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.Leave += new EventHandler(VerificationCoverage_Leave);

        }

        #endregion

        #endregion

        #region Construction and Finalization

        public VerificationCoverage()
        {
            InitializeComponent();
            base.EnableThemesOn(this);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            this.StopResponsePollTimer();
            this.StopResponseWaitTimer();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private static readonly ILog c_log = LogManager.GetLogger(typeof(VerificationCoverage));

        private Container components = null;

        private LoggingButton btnInitiate;

        private Label lblStaticFinClass;
        private Label lblPatientName;
        private Label lblStaticName;
        private Label lblStaticType;
        private Label lblStaticComplaint;
        private Label lblPatientType;
        private Label lblChiefComplaint;
        private Label lblFinancialClass;
        private Label lblStaticDOB;
        private Label lblDOB;
        private Label lblStaticAccount;
        private Label lblAccount;
        private Label lblStaticHospService;
        private Label lblHospitalService;
        private Label lblStaticProvider;
        private Label lblMedicaidProvider;
        private Label lblStaticFacilityId;
        private Label lblFacilityID;
        private Label lblStaticVerification;

        private RequiredFieldsDialog requiredFieldSummary;
        private BenefitsValidationResponse i_BenefitsValidationResponse;
        private Account i_Account;

        private Timer i_ResponseWaitTimer = new Timer();
        private Timer i_ResponsePollTimer = new Timer();

        private TextBox textBox;
        private Panel panel1;
        private Panel panel2;

        #endregion

        #region Constants

        // 5 minutes (in milliseconds)
        private const int RESPONSE_WAIT_DURATION = 300000;

        // 10 seconds (in milliseconds)
        private const int RESPONSE_POLL_INTERVAL = 5000;

        private const string VALIDATION_SUMMARY_HEADER = "Completion of the following fields are required to initiate a request within the coverage category ";

        private const string RESPONSE_PENDING = "Request for results initiated.  Awaiting response...";



        private const string NO_RESPONSE_RECEIVED = "No results were received within the allowed time period. Please reinitiate a request for results by clicking the Initiate button.";

        private const string MEDICARE_PROVIDER = "Medicare provider no:",
                             MEDICAID_PROVIDER = "Medicaid provider no:",
                             EXISTING_BENEFITS = "Existing benefits information may be replaced or cleared if another request is initiated. Do you want to continue?";

        private const string BENEFITS_VALIDATION = "BenefitsValidation";

        #endregion
    }
}
