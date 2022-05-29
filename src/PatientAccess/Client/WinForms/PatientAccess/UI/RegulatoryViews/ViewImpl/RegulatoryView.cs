using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;
using System.Text.RegularExpressions;
using PatientAccess.Utilities;
using System.Collections.Generic;
using PatientAccess.UI.CommonControls.Email.Presenters;
using PatientAccess.UI.CommonControls.Email.Views;




namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    /// <summary>
    /// Summary description for RegulatoryView.
    /// </summary>
    public class RegulatoryView : ControlView, IRegulatoryView, IEmailAddressView, IEmailReasonView
    {
        #region Events

        public event EventHandler NPPSelected;
        public event EventHandler COSSignedSelected;
       // public event EventHandler EmailChanged;
        

        #endregion

        #region Event Handlers

        private void RegulatoryView_Leave(object sender, EventArgs e)
        {
            blnLeaveRun = true;

            RuleEngine.GetInstance().EvaluateRule(typeof (OnRegulatoryForm), Model_Account);

            blnLeaveRun = false;
        }

        private void RegulatoryView_Disposed(object sender, EventArgs e)
        {
            // unwire the event handlers
            unregisterEventHandlers();
        }

        private void COSSignedRequiredEventHandler(object sender, EventArgs e)
        {
            setRequiredBgColor(cmbCosSigned);
        }

        private void COSSignedPreferredEventHandler(object sender, EventArgs e)
        {
            setPreferredBgColor(cmbCosSigned);
        }

        private void NPPVersionRequiredEventHandler(object sender, EventArgs e)
        {
            setRequiredBgColor(cmbNppVersion);
        }

        private void NPPVersionPreferredEventHandler(object sender, EventArgs e)
        {
            setPreferredBgColor(cmbNppVersion);
        }

        private void NPPSignatureStatusRequiredEventHandler(object sender, EventArgs e)
        {
            rbSigned.Enabled = true;
            rbUnableToSign.Enabled = true;
            rbRefusedToSign.Enabled = true;
        }

        private void NPPSignedOnDateRequiredEventHandler(object sender, EventArgs e)
        {
            setRequiredBgColor(mtbSignedOnDate);
        }

        private void NPPSignedOnDatePreferredEventHandler(object sender, EventArgs e)
        {
            setPreferredBgColor(mtbSignedOnDate);
        }

        private void mtbEmailAddress_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = (MaskedEditTextBox)sender;
            UIColors.SetNormalBgColor(mtb);
            // check if only valid email special characters have been entered or pasted
            if (mtb.Text != string.Empty && emailKeyPressExpression.IsMatch(mtb.Text) == false)
            {   // Prevent cursor from advancing to the next control
                e.Cancel = true;
                UIColors.SetErrorBgColor(mtb);
                MessageBox.Show(UIErrorMessages.ONLY_VALID_EMAIL_CHARACTERS_ALLOWED, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                mtb.Focus();
                return;
            }

            // check if entered email is in the correct email format
            if (mtb.Text != string.Empty &&
                (emailValidationExpression.IsMatch(mtb.Text) == false ||
                 EmailAddressPresenter.IsGenericEmailAddress(mtb.Text))
                )
            {
                // Prevent cursor from advancing to the next control
                e.Cancel = true;
                UIColors.SetErrorBgColor(mtb);
                MessageBox.Show(UIErrorMessages.EMAIL_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                mtb.Focus();
            }
            else
            {
              
                ContactPoint mailingContactPoint = Model_Account.Patient.ContactPointWith(
                       TypeOfContactPoint.NewMailingContactPointType());
                mailingContactPoint.EmailAddress = (mtb.Text == String.Empty) ? new EmailAddress() : new EmailAddress(mtb.Text);
                RuleEngine.OneShotRuleEvaluation<EmailAddressRequired>(Model_Account, EmailAddressRequiredEventHandler);
                RuleEngine.OneShotRuleEvaluation<EmailAddressPreferred>(Model_Account, EmailAddressPreferredEventhandler);

                
            }
        }

        private void cmbEmailReason_SelectedIdexchanged(object sender, EventArgs e)
        {
            EmailReasonPresenter.UpdateEmailReason();
            if ( emailReasonSelected )
            {
                if (Model_Account.Patient.EmailReason.IsDeclined || Model_Account.Patient.EmailReason.IsRemoved)
                {
                    if (!loadingEmailReasonData && !EmailReasonAutoSelected)
                    {
                        ValidateEmailReasonDeclinedRule();
                    }

                }
            }
        }

        private void ValidateEmailReasonDeclinedRule()
        {
            if (IsOptinFeatureEnabled)
            {
                if (Model_Account.PatientPortalOptIn.IsNo && Model_Account.Patient.HospitalCommunicationOptIn.IsYes)
                {
                    emailReasonSelected = false;
                    HospitalCommunicationOptInPresenter.ValidateEmailAddress();
                }
                else if (Model_Account.PatientPortalOptIn.IsYes && Model_Account.Patient.HospitalCommunicationOptIn.IsNo)
                {
                    emailReasonSelected = false;
                    PatientPortalOptInPresenter.ValidateEmailAddress();
                }
            }
        }

        private bool IsOptinFeatureEnabled
        {
            get
            {
                return HospitalCommunicationOptInPresenter.IsFeatureEnabled &&
                       PatientPortalOptInPresenter.IsFeatureEnabled();
            }
        }
        private void cmbEmailReason_Validating(object sender, CancelEventArgs e)
        {
            EmailReasonPresenter.ValidateEmailReason();
        }
        private void cmbEmailReason_KeyUp(object sender, KeyEventArgs e)
        {
            emailReasonSelected = true;
        }
        private void cmbEmailReason_KeyDown(object sender, KeyEventArgs e)
        {
            emailReasonSelected = true;
        }
        private void cmbEmailReason_MouseUp(object sender, MouseEventArgs e)
        {
            emailReasonSelected = true;
        }
        private void cmbEmailReason_MouseDown(object sender, MouseEventArgs e)
        {
            emailReasonSelected = true;
        }
       
        private void EmailAddressRequiredEventHandler(object sender, EventArgs e)
        {
            SetEmailAddressAsRequired();
        }
        
        private void EmailReasonRequiredEventhandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(cmbEmailReason);
        }
        private void EmailReasonPreferredEventhandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor(cmbEmailReason);
        }
        private void EmailAddressPreferredEventhandler(object sender, EventArgs e)
        {
            SetEmailAddressAsPreferred();
        }

        private void NotifyPCPDataRequiredEventhandler(object sender, EventArgs e)
        {
            hieShareDataFlagView.SetNotifyPCPDataRequiredBgColor();
        }

        private void ShareHIEDataRequiredEventhandler(object sender, EventArgs e)
        {
            hieShareDataFlagView.SetShareDataWithPublicHIEAsRequired();
        }

        /// <summary>
        /// Model_Account_Changed - listener for property changed events on the Model_Account instance
        /// This event is fired if the KindOfVisit or HospitalService properties were changed.  If so,
        /// we conditionally set required fields on this control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="Aspect">the Property name of the property that changed</param>
        /// <param name="OldValue">the old property value</param>
        /// <param name="NewValue">the new property value</param>
        private void Model_Account_Changed(object sender, string Aspect, object OldValue, object NewValue)
        {
            if (Aspect == "KindOfVisit")
            {
                handlePatientTypeChanged();
            }
            else if (Aspect == "HospitalService")
            {
                handleHospitalServiceChanged();
            }
        }

        private void cmbNppVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            NPPDocument.NPPVersion =
                cmbNppVersion.SelectedItem as NPPVersion;

            if (NPPSelected != null)
            {
                NPPSelected(this, null);
            }

            cmbNppVersion_Validating(this, null);

            PopulateNPP();

            CheckForRequiredNppDocumentFields();
        }

        private void PopulateNPP()
        {
            if (NPPDocument.NPPVersion != null
                && NPPDocument.NPPVersion.ToString() != String.Empty)
            {
                EnableSignatureStatus(true);
                if (NPPDocument.SignatureStatus.IsEmptyStatus())
                {
                    rbSigned.Checked = true;
                    NPPDocument.SignatureStatus.SetSigned();
                    setRequiredBgColor(mtbSignedOnDate);
                }
                else
                {
                    mtbSignedOnDate.Enabled = NPPDocument.SignatureStatus.IsSignedStatus();
                    dtpSignedOnDate.Enabled = NPPDocument.SignatureStatus.IsSignedStatus();
                }
            }
            else
            {
                EnableSignatureStatus(false);
                rbSigned.Checked = false;
                rbUnableToSign.Checked = false;
                rbRefusedToSign.Checked = false;
                NPPDocument.SignatureStatus.SetToEmpty();
                setNormalBgColor(mtbSignedOnDate);
            }
        }

        /// <summary>
        /// Click on Radio Button - Signed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbSigned_Click(object sender, EventArgs e)
        {
            if (NPPDocument.SignatureStatus == null)
            {
                NPPDocument.SignatureStatus = new SignatureStatus();
            }

            rbSigned.Checked = true;

            if (NPPDocument.SignedOnDate == DateTime.MinValue)
            {
                signedOnDateTxt = string.Empty;
                mtbSignedOnDate.UnMaskedText = string.Empty;
            }
            else
            {
                mtbSignedOnDate.UnMaskedText = String.Format("{0:D2}{1:D2}{2:D4}",
                                                             NPPDocument.SignedOnDate.Month,
                                                             NPPDocument.SignedOnDate.Day,
                                                             NPPDocument.SignedOnDate.Year);
                signedOnDateTxt = mtbSignedOnDate.Text;
            }

            NPPDocument.SignatureStatus.SetSigned();

            mtbSignedOnDate.Enabled = true;
            dtpSignedOnDate.Enabled = true;

            setNormalBgColor(mtbSignedOnDate);

            RuleEngine.GetInstance().EvaluateRule(typeof (NPPSignedOnDateRequired), Model_Account);
            RuleEngine.GetInstance().EvaluateRule(typeof(NPPSignedOnDatePreferred), Model_Account);
        }


        /// <summary>
        /// Click on Radio Button - Patient unable to Sign
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbUnableToSign_Click(object sender, EventArgs e)
        {
            if (NPPDocument.SignatureStatus == null)
            {
                NPPDocument.SignatureStatus = new SignatureStatus();
            }

            NPPDocument.SignatureStatus.SetUnableToSign();
            NPPDocument.SignedOnDate = DateTime.MinValue;

            mtbSignedOnDate.Enabled = false;
            mtbSignedOnDate.UnMaskedText = string.Empty;
            dtpSignedOnDate.Enabled = false;
            setNormalBgColor(mtbSignedOnDate);
        }

        /// <summary>
        /// Click on Radio Button - Patient refused to Sign
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbRefusedToSign_Click(object sender, EventArgs e)
        {
            if (NPPDocument.SignatureStatus == null)
            {
                NPPDocument.SignatureStatus = new SignatureStatus();
            }

            NPPDocument.SignatureStatus.SetRefusedToSign();
            NPPDocument.SignedOnDate = DateTime.MinValue;

            mtbSignedOnDate.Enabled = false;
            mtbSignedOnDate.UnMaskedText = string.Empty;
            dtpSignedOnDate.Enabled = false;
            setNormalBgColor(mtbSignedOnDate);
        }

        private void rbSigned_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Right || e.KeyData == Keys.Down)
            {
                rbUnableToSign.Focus();
            }
            else if (e.KeyData == Keys.Left || e.KeyData == Keys.Up)
            {
                rbRefusedToSign.Focus();
            }
        }

        private void rbUnableToSign_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Up)
            {
                rbSigned.Focus();
            }
            else if (e.KeyData == Keys.Right || e.KeyData == Keys.Down)
            {
                rbRefusedToSign.Focus();
            }
        }

        private void rbRefusedToSign_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Right || e.KeyData == Keys.Down)
            {
                rbSigned.Focus();
            }
            else if (e.KeyData == Keys.Left || e.KeyData == Keys.Up)
            {
                rbUnableToSign.Focus();
            }
        }


        private void cmbConfidentialityStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbConfidentialityStatus.SelectedIndex != -1)
            {
                Model_Account.ConfidentialityCode = cmbConfidentialityStatus.SelectedItem as ConfidentialCode;
            }

            cmbConfidentialityStatus_Validating(this, null);
        }

        private void cmbCosSigned_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConditionOfService conditionOfService =
                cmbCosSigned.SelectedItem as ConditionOfService ?? new ConditionOfService();

            Model_Account.COSSigned = conditionOfService;

            if (COSSignedSelected != null)
            {
                COSSignedSelected(this, null);
            }

            cmbCosSigned_Validating(this, null);

            COSsignedHandler.HandleCOSSignedSelected();
        }

        private void cmbFacilityFlag_SelectedIndexChanged(object sender, EventArgs e)
        {
            Model_Account.FacilityDeterminedFlag = cmbFacilityFlag.SelectedItem as FacilityDeterminedFlag;

            cmbFacilityFlag_Validating(this, null);
        }

        private void ckbAllInformation_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            optOutAllInformation = cb.Checked;
            if (cb.Checked)
            {
                ckbLocation.Checked = true;
                ckbLocation.Enabled = false;

                ckbHealthInfo.Checked = true;
                ckbHealthInfo.Enabled = false;

                ckbReligion.Checked = true;
                ckbReligion.Enabled = false;
            }
            else
            {
                ckbLocation.Checked = false;
                ckbLocation.Enabled = true;

                ckbHealthInfo.Checked = false;
                ckbHealthInfo.Enabled = true;

                ckbReligion.Checked = false;
                ckbReligion.Enabled = true;
            }
            Model_Account.OptOutName = optOutAllInformation;
        }

        private void ckbLocation_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            optOutLocation = cb.Checked;
            if (optOutAllInformation && cb.Checked == false)
            {
                ckbAllInformation.Checked = false;
            }
            Model_Account.OptOutLocation = optOutLocation;
        }

        private void ckbHealthInfo_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            optOutHealthInformation = cb.Checked;
            if (optOutAllInformation && cb.Checked == false)
            {
                ckbAllInformation.Checked = false;
            }
            Model_Account.OptOutHealthInformation = optOutHealthInformation;
        }

        private void ckbReligion_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            optOutReligion = cb.Checked;
            if (optOutAllInformation && cb.Checked == false)
            {
                ckbAllInformation.Checked = false;
            }
            Model_Account.OptOutReligion = optOutReligion;
        }

        //---------------------Evaluate ComboBoxes -------------------------------------------------------------
        private void cmbNppVersion_Validating(object sender, CancelEventArgs e)
        {
            if (!blnLeaveRun)
            {
                UIColors.SetNormalBgColor(cmbNppVersion);
                Refresh();
                RuleEngine.GetInstance().EvaluateRule(typeof (InvalidNPPVersion), Model_Account);
                RuleEngine.GetInstance().EvaluateRule(typeof (InvalidNPPVersionChange), Model_Account);
            }
            RuleEngine.GetInstance().EvaluateRule(typeof (NPPVersionRequired), Model_Account);
            RuleEngine.GetInstance().EvaluateRule(typeof (NPPVersionPreferred), Model_Account);
            RuleEngine.GetInstance().EvaluateRule(typeof (NPPSignatureStatusRequired), Model_Account);
        }

        private void cmbConfidentialityStatus_Validating(object sender, CancelEventArgs e)
        {
            if (!blnLeaveRun)
            {
                setNormalBgColor(cmbConfidentialityStatus);
                RuleEngine.GetInstance().EvaluateRule(typeof (InvalidConfidentialStatus), Model_Account);
                RuleEngine.GetInstance().EvaluateRule(typeof (InvalidConfidentialStatusChange), Model_Account);
            }
        }

        private void cmbCosSigned_Validating(object sender, CancelEventArgs e)
        {
            if (!blnLeaveRun)
            {
                UIColors.SetNormalBgColor(cmbCosSigned);
                Refresh();
                RuleEngine.GetInstance().EvaluateRule(typeof (InvalidCOS), Model_Account);
                RuleEngine.GetInstance().EvaluateRule(typeof (InvalidCOSChange), Model_Account);
            }
            RuleEngine.GetInstance().EvaluateRule(typeof (COSSignedPreferred), Model_Account);
            RuleEngine.GetInstance().EvaluateRule(typeof (COSSignedRequired), Model_Account);
        }

        private void cmbFacilityFlag_Validating(object sender, CancelEventArgs e)
        {
            if (!blnLeaveRun)
            {
                UIColors.SetNormalBgColor(cmbFacilityFlag);
                Refresh();
                RuleEngine.GetInstance().EvaluateRule(typeof (InvalidFacilityDeterminedFlag), Model_Account);
                RuleEngine.GetInstance().EvaluateRule(typeof (InvalidFacilityDeterminedFlagChange), Model_Account);
            }
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private void ProcessInvalidCodeEvent(PatientAccessComboBox comboBox)
        {
            UIColors.SetDeactivatedBgColor(comboBox);

            MessageBox.Show(UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1);

            if (!comboBox.Focused)
            {
                comboBox.Focus();
            }
        }

        private void InvalidNPPVersionChangeEventHandler(object sender, EventArgs e)
        {
            ProcessInvalidCodeEvent(cmbNppVersion);
        }

        private void InvalidConfidentialStatusChangeEventHandler(object sender, EventArgs e)
        {
            ProcessInvalidCodeEvent(cmbConfidentialityStatus);
        }

        private void InvalidCOSChangeEventHandler(object sender, EventArgs e)
        {
            ProcessInvalidCodeEvent(cmbCosSigned);
        }

        private void InvalidFacilityDeterminedFlagChangeEventHandler(object sender, EventArgs e)
        {
            ProcessInvalidCodeEvent(cmbFacilityFlag);
        }

        //-----------------------------------------------------------------

        private void InvalidNPPVersionEventHandler(object sender, EventArgs e)
        {
            UIColors.SetDeactivatedBgColor(cmbNppVersion);
        }

        private void NPPSignedOnDateInvalidEventHandler(object sender, EventArgs e)
        {
            setErrorBgColor(mtbSignedOnDate);

            string msgError = string.Format(UIErrorMessages.NPP_DATE_SIGNED_TOO_EARLIER_ERRMSG,
                                            Model_Account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion.NPPDate.
                                                ToString(@"MM/dd/yy"));

            MessageBox.Show(msgError, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1);

            if (!mtbSignedOnDate.Focused)
            {
                mtbSignedOnDate.Focus();
            }
        }

        private void InvalidConfidentialStatusEventHandler(object sender, EventArgs e)
        {
            UIColors.SetDeactivatedBgColor(cmbConfidentialityStatus);
        }

        private void InvalidCOSEventHandler(object sender, EventArgs e)
        {
            UIColors.SetDeactivatedBgColor(cmbCosSigned);
        }

        private void InvalidFacilityDeterminedFlagEventHandler(object sender, EventArgs e)
        {
            UIColors.SetDeactivatedBgColor(cmbFacilityFlag);
        }

        private void PopulateEmailAddress()
        {
            var mailingContactPoint =
                Model_Account.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
            if (mailingContactPoint != null && mailingContactPoint.EmailAddress != null)
            {
                mtbEmail.UnMaskedText = mailingContactPoint.EmailAddress.ToString();
            }
        }

        private void mtbSignedOnDate_Validating(object sender, CancelEventArgs e)
        {
            if (!blnLeaveRun)
            {
                if (dtpSignedOnDate.Focused == false)
                {
                    CheckForRequiredNppDocumentFields();
                }
            }
        }

        /// <summary>
        /// Date Time Picker Signed On Date CloseUp
        /// </summary>
        private void dtpSignedOnDate_CloseUp(object sender, EventArgs e)
        {
            if (dtpSignedOnDate.Checked)
            {
                DateTime dt = dtpSignedOnDate.Value;
                mtbSignedOnDate.UnMaskedText = String.Format("{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year);
            }
            else
            {
                mtbSignedOnDate.UnMaskedText = String.Empty;
            }

            signedOnDateTxt = mtbSignedOnDate.Text;

            CheckForRequiredNppDocumentFields();
        }

        #endregion

        #region Methods

        public override void UpdateView()
        {
            if (Model_Account == null)
            {
                return;
            }

            RightToRestrictPresenter =
                new RightToRestrictPresenter(this, new RightToRestrictFeatureManager(), Model_Account);
            HospitalCommunicationOptInPresenter = new HospitalCommunicationOptInPresenter( hospitalCommunicationOptInView, new MessageBoxAdapter(), Model_Account,
                new HospitalCommunicationOptInFeatureManager(), RuleEngine.GetInstance());
            PatientPortalOptInPresenter = new PatientPortalOptInPresenter(patientPortalOptInView, new MessageBoxAdapter(), Model_Account,
                new PatientPortalOptInFeatureManager(), RuleEngine.GetInstance());
            AuthorizeAdditionalPortalUsersPresenter = new AuthorizeAdditionalPortalUsersPresenter(AuthorizeAdditionalPortalUsersView, Model_Account,
                new AuthorizePortalUserFeatureManager());
            COSsignedHandler =
                new COSSignedHandler(this, RuleEngine.GetInstance(), Model_Account);
            RegulatoryPresenter = 
                new RegulatoryPresenter(this, Model_Account);

            if (loadingModelData)
            {
                if (Model_Account.Activity.IsMaintenanceActivity() ||
                    Model_Account.Activity.IsShortMaintenanceActivity()
                )
                {
                    COSsignedHandler.LoadingEditMaintain = true;
                }
            }
            COBReceivedAndIMFMReceivedPresenter = new COBReceivedAndIMFMReceivedPresenter(
                cobReceivedAndIMFMReceivedView, this, new COBReceivedAndIMFMReceivedFeatureManager(), Model_Account);
            loadingEmailReasonData = true;

            EmailAddressPresenter = new EmailAddressPresenter(this, Model_Account, RuleEngine.GetInstance());
            var emailReasonBroker = BrokerFactory.BrokerOfType<IEmailReasonBroker>();
            EmailReasonPresenter = new EmailReasonPresenter(this, Model_Account, emailReasonBroker);
            EmailReasonPresenter.PopulateEmailReason();
            HideEmailReason();
            HandleEmailAddress();
            RegisterOptInEvents();
            HospitalCommunicationOptInPresenter.UpdateView();
            PatientPortalOptInPresenter.UpdateView();
            AuthorizeAdditionalPortalUsersView.ModelAccount = Model_Account;
            AuthorizeAdditionalPortalUsersView.UpdateView();
            cmbCosSigned.Enabled = IsCosRequired;
            if (Model_Account.Patient != null)
            {
                PopulateEmailAddress();
                if (loadingModelData)
                {
                    SavedPatientPortalOptIn = Model_Account.PatientPortalOptIn.Code;
                    SavedHospitalCommunincationOptIn = Model_Account.Patient.HospitalCommunicationOptIn.Code;
                }
            }
            if (loadingModelData)
            {
                loadingModelData = false;
                COSsignedHandler.LoadingModel = true;
                PopulateFacilityFlagComboBox();
                if (IsCosRequired)
                {
                    PopulateCOSComboBox();
                }
                PopulateNppComboBox();
                PopulateConfidentialityStatusComboBox();
            }
            if (IsCosRequired)
            {
                if (Model_Account.COSSigned != null
                    && Model_Account.COSSigned.Code.Trim() != string.Empty)
                {
                    cmbCosSigned.SelectedItem = Model_Account.COSSigned;
                }
                else // default based on the PT/HSV code
                {
                    if (Model_Account.KindOfVisit != null
                        && Model_Account.KindOfVisit.Code == VisitType.NON_PATIENT)
                    {
                        cmbCosSigned.SelectedIndex = cmbCosSigned.FindString("Yes");
                    }
                    else if (Model_Account.HospitalService != null
                             && (
                                 Model_Account.HospitalService.Code == "SP" ||
                                 Model_Account.HospitalService.Code == "LB" ||
                                 Model_Account.HospitalService.Code == "AB"
                             )
                    )
                    {
                        cmbCosSigned.SelectedIndex = cmbCosSigned.FindString("Yes");
                    }
                    else if (cmbCosSigned.Items.Count > 0)
                    {
                        cmbCosSigned.SelectedIndex = 0;
                    }
                }
                COSsignedHandler.LoadingModel = false;
            }
            if (Model_Account.FacilityDeterminedFlag != null)
            {
                cmbFacilityFlag.SelectedItem = Model_Account.FacilityDeterminedFlag;
            }
            if (Model_Account.ConfidentialityCode != null)
            {
                cmbConfidentialityStatus.SelectedItem = Model_Account.ConfidentialityCode;
            }
            if (NPPDocument != null)
            {
                if (NPPDocument.NPPVersion != null)
                {
                    // EventHandler for cmbNppVersion has to be temporarily removed    
                    // to prevent mtbSignedOnDate from being wiped out
                    cmbNppVersion.SelectedIndexChanged -= cmbNppVersion_SelectedIndexChanged;
                    cmbNppVersion.SelectedItem = NPPDocument.NPPVersion;
                    PopulateNPP();
                    cmbNppVersion.SelectedIndexChanged += cmbNppVersion_SelectedIndexChanged;
                }

                if (NPPDocument.SignatureStatus != null)
                {
                    SelectSignatureStatus();
                }
                if (rbSigned.Checked)
                {
                    if (NPPDocument.SignedOnDate == DateTime.MinValue)
                    {
                        mtbSignedOnDate.UnMaskedText = string.Empty;
                    }
                    else
                    {
                        mtbSignedOnDate.UnMaskedText = String.Format("{0:D2}{1:D2}{2:D4}",
                            NPPDocument.SignedOnDate.Month,
                            NPPDocument.SignedOnDate.Day,
                            NPPDocument.SignedOnDate.Year);
                    }

                    signedOnDateTxt = mtbSignedOnDate.Text;
                }
            }

            ckbLocation.Checked = Model_Account.OptOutLocation;
            ckbHealthInfo.Checked = Model_Account.OptOutHealthInformation;
            ckbReligion.Checked = Model_Account.OptOutReligion;
            ckbAllInformation.Checked = Model_Account.OptOutName;
            hieShareDataFlagView.PatientAccount = Model_Account;
            RightToRestrictPresenter.UpdateView();
            hieShareDataFlagView.UpdateView();
            COBReceivedAndIMFMReceivedPresenter.UpdateView();
            RegulatoryPresenter.SetShareDataWithHIEAndPCPLocation();
            if (!accountChangedListenerWired)
            {
                // associate a Change listener that will listen for modifications to properties on our
                // Account instance (Model_Account) 
                accountChangedListenerWired = true;
            }
            loadingEmailReasonData = false;
            // wire the event handlers
            registerEventHandlers();
            runRules();
        }
        
        public void SetCOBReceivedLocationForPreregistrationAccount()
        {
            cobReceivedAndIMFMReceivedView.Location = new System.Drawing.Point(343, 125);
            cobReceivedAndIMFMReceivedView.TabIndex = 0;
            hieShareDataFlagView.TabIndex = 0;
            hieShareDataFlagView.TabStop = false;
            cobReceivedAndIMFMReceivedView.BringToFront();
            
        }
        public void SetCOBReceivedLocationForRegistrationAccount()
        {
            cobReceivedAndIMFMReceivedView.Location = new System.Drawing.Point(343, 274);
            cobReceivedAndIMFMReceivedView.BringToFront();
        }
        public void SetIMFMReceivedLocationForPreregistrationAccount()
        {
            cobReceivedAndIMFMReceivedView.Location = new System.Drawing.Point(343, 118);
            cobReceivedAndIMFMReceivedView.TabIndex = 0;
            hieShareDataFlagView.TabIndex = 0;
            hieShareDataFlagView.TabStop = false;
            cobReceivedAndIMFMReceivedView.BringToFront();
        }
        public void SetIMFMReceivedLocationForRegistrationAccount()
        {
            cobReceivedAndIMFMReceivedView.Location = new System.Drawing.Point(343, 274);
            cobReceivedAndIMFMReceivedView.BringToFront();
        }
        public void SetHIEShareDataAndPCPFlagLocation()
        {
            if (Model_Account.Activity.IsPreRegistrationActivity() ||
                Model_Account.KindOfVisit.IsPreRegistrationPatient)
            {
                hieShareDataFlagView.Location = new System.Drawing.Point(346, 160);
                hieShareDataFlagView.TabIndex = 11;
                AuthorizeAdditionalPortalUsersView.TabIndex = 0;
                hieShareDataFlagView.TabStop = true;

            }
            else if (Model_Account.Activity.IsPreMSEAndUCPreMSEAccount)
            {
                hieShareDataFlagView.Location = new System.Drawing.Point(343, 60);
            }
        }

        public override void UpdateModel()
        {
        }
        
        public void EnableRightToRestrict()
        {
            ckbRightToRestrict.Show();
            ckbRightToRestrict.Enabled = true;
        }
        public void DisableRightToRestrict()
        {
            ckbRightToRestrict.Hide();
            ckbRightToRestrict.Enabled = false;
        }
        public void SetRightToRestrict()
        {
            ckbRightToRestrict.Checked = true;
        }
        public void UnSetRightToRestrict()
        {
            ckbRightToRestrict.Checked = false;
        }

        private void SelectSignatureStatus()
        {
            switch (NPPDocument.SignatureStatus.Code)
            {
                case SignatureStatus.SIGNED:
                    rbSigned.Checked = true;
                    mtbSignedOnDate.Enabled = true;
                    dtpSignedOnDate.Enabled = true;
                    rbUnableToSign.Checked = false;
                    rbRefusedToSign.Checked = false;
                    break;
                case SignatureStatus.UNABLE_TO_SIGN:
                    rbUnableToSign.Checked = true;
                    rbSigned.Checked = false;
                    rbRefusedToSign.Checked = false;
                    mtbSignedOnDate.Enabled = false;
                    dtpSignedOnDate.Enabled = false;
                    break;
                case SignatureStatus.REFUSED_TO_SIGN:
                    rbRefusedToSign.Checked = true;
                    rbUnableToSign.Checked = false;
                    rbSigned.Checked = false;
                    mtbSignedOnDate.Enabled = false;
                    dtpSignedOnDate.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void EnableSignatureStatus(bool setButton)
        {
            rbSigned.Enabled = setButton;
            rbUnableToSign.Enabled = setButton;
            rbRefusedToSign.Enabled = setButton;

            mtbSignedOnDate.UnMaskedText = string.Empty;
            mtbSignedOnDate.Enabled = setButton;
            dtpSignedOnDate.Enabled = setButton;
        }

        /// <summary>
        /// Verify NPP Signed on Date
        /// </summary>
        private bool VerifyNPPSignedOnDate()
        {
            setNormalBgColor(mtbSignedOnDate);

            if (mtbSignedOnDate.UnMaskedText.Length == 0
                || mtbSignedOnDate.UnMaskedText ==
                String.Empty)
            {
                NPPDocument.SignedOnDate = DateTime.MinValue;
                return true;
            }

            if (mtbSignedOnDate.Text.Length != 10)
            {
                mtbSignedOnDate.Focus();
                setErrorBgColor(mtbSignedOnDate);
                MessageBox.Show(UIErrorMessages.NPP_DATE_SIGNED_INVALID_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
                return false;
            }

            string month = mtbSignedOnDate.Text.Substring(0, 2);
            string day = mtbSignedOnDate.Text.Substring(3, 2);
            string year = mtbSignedOnDate.Text.Substring(6, 4);

            verifyMonth = Convert.ToInt32(month);
            verifyDay = Convert.ToInt32(day);
            verifyYear = Convert.ToInt32(year);

            try
            {
                // Check the date entered is not in the future
                var theDate = new DateTime(verifyYear, verifyMonth, verifyDay);

                if (theDate > GetCurrentFacilityDateTime())
                {
                    mtbSignedOnDate.Focus();
                    setErrorBgColor(mtbSignedOnDate);
                    MessageBox.Show(UIErrorMessages.NPP_DATE_SIGNED_FUTURE_ERRMSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                    return false;
                }
                else if (DateValidator.IsValidDate(theDate) == false)
                {
                    mtbSignedOnDate.Focus();
                    setErrorBgColor(mtbSignedOnDate);
                    MessageBox.Show(UIErrorMessages.NPP_DATE_SIGNED_NOT_EXIST_ERRMSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                    return false;
                }
                else if (theDate < earliestDate)
                {
                    mtbSignedOnDate.Focus();
                    setErrorBgColor(mtbSignedOnDate);
                    MessageBox.Show(UIErrorMessages.NPP_DATE_SIGNED_OUT_OF_RANGE, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                    MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                mtbSignedOnDate.Focus();
                setErrorBgColor(mtbSignedOnDate);
                MessageBox.Show(UIErrorMessages.NPP_DATE_SIGNED_NOT_EXIST_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
                return false;
            }

            NPPDocument.SignedOnDate = Convert.ToDateTime(mtbSignedOnDate.Text);

            return true;
        }

        private DateTime GetCurrentFacilityDateTime()
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt(User.GetCurrent().Facility.GMTOffset,
                                     User.GetCurrent().Facility.DSTOffset);
        }

        private void CheckForRequiredNppDocumentFields()
        {
            if (VerifyNPPSignedOnDate())
            {
                // reset all fields that might have error, preferred, or required backgrounds          

                UIColors.SetNormalBgColor(cmbNppVersion);
                UIColors.SetNormalBgColor(mtbSignedOnDate);

                RuleEngine.GetInstance().EvaluateRule(typeof (OnRegulatoryForm), Model_Account);
            }
        }

        public void AutoPopulateShareDataWithPublicHIEForRightToRestrict(bool isRightToRestrictChecked)
        {
            hieShareDataFlagView.AutoPopulateShareDataWithPublicHIEForRightToRestrict(isRightToRestrictChecked);
        }

        public void HospCommOptIn()
        {
            HospitalCommunicationOptInPresenter.OptIn();
        }

        public void HospCommOptOut()
        { 
            HospitalCommunicationOptInPresenter.OptOut();
        }

        public void EnableHospComm()
        {
            HospitalCommunicationOptInPresenter.EnableMe();
        }

        public void DisableHospComm()
        {
            HospitalCommunicationOptInPresenter.DisableMe();
        }

        public void PatientPortalOptIn()
        { 
            PatientPortalOptInPresenter.OptIn();
        }

        public void PatientPortalOptOut()
        { 
            PatientPortalOptInPresenter.OptOut();
        }
        public void EnablePatientPortal()
        {
            PatientPortalOptInPresenter.EnableMe();
        }
        public void DisablePatientPortal()
        {
            PatientPortalOptInPresenter.DisableMe();
        }
        public void EnableAuthorizeAdditionalPortalUser()
        {
            AuthorizeAdditionalPortalUsersPresenter.EnableMe();
        }
        public void DisableAuthorizeAdditionalPortalUser()
        {
            AuthorizeAdditionalPortalUsersPresenter.DisableMe();
        }
        public void UpdateHospitalCommunicationView()
        {
            HospitalCommunicationOptInPresenter.UpdateView();
        }
        public void UpdatePatientPortalView()
        {
            PatientPortalOptInPresenter.UpdateView();
        }
        public void UnSelectAuthorizePatientPortalUser()
        {
            AuthorizeAdditionalPortalUsersPresenter.UnSelectAuthorizeAdditionalPatientPortalUser();
        }
        public void SetEmailReasonToPatientDeclined()
        {
            var declinedEmailReason = new EmailReason() {Code = string.Empty, Description = String.Empty};
            declinedEmailReason.SetDeclined();
            EmailReasonAutoSelected = true;
            cmbEmailReason.SelectedItem = declinedEmailReason;
            EmailReasonAutoSelected = false;
        }

        private bool EmailReasonAutoSelected { get; set; }

        public void DisableEmailReason()
        {
            lblEmaiReason.Enabled = false;
            cmbEmailReason.Enabled = false;
        }
        public void EnableEmailReason()
        {
            lblEmaiReason.Enabled = true;
            cmbEmailReason.Enabled = true;
        }
        public void DisableEmail()
        {
            lblEmail.Enabled = false;
            mtbEmail.Enabled = false;
        }
        public void EnableEmail()
        {
            lblEmail.Enabled = true;
            mtbEmail.Enabled = true;
        }
        public void DoNotShowEmailAddress()
        {
            mtbEmail.Visible = false;
            lblEmail.Visible = false;
        }
        public void SetEmailAddressToNormalColor()
        {
            UIColors.SetNormalBgColor(mtbEmail);
        }
        public void SetEmailReasonNormal()
        {
            UIColors.SetNormalBgColor(cmbEmailReason);
        }
        public void SetEmailReasonToRequired()
        {
            UIColors.SetRequiredBgColor(cmbEmailReason);
        }
        public void SetEmailReasonToPreferred()
        {
            UIColors.SetPreferredBgColor(cmbEmailReason);
        }
        public void PopulateEmailReasonSelections(IEnumerable<EmailReason> emailReasonValues)
       {
           foreach (var emailReason in emailReasonValues)
           {
               cmbEmailReason.Items.Add(emailReason);
           }
           if (Model_Account.Patient.EmailReason != null && Model_Account.Patient.EmailReason.Code != string.Empty)
           {
               cmbEmailReason.SelectedItem = Model_Account.Patient.EmailReason;
           }

           else
           {
               cmbEmailReason.SelectedIndex = 0;
           }

          }
        public EmailReason EmailReason
        {
            get
            {
                return cmbEmailReason.SelectedItem as EmailReason ?? new EmailReason();

            }
        }
        public void ClearEmailReasonSelectionValues()
        {
            cmbEmailReason.Items.Clear();
        }
        public void SetGuarantorEmailAddressAsRequired(){}
        public void SetGuarantorEmailAddressAsPreferred(){}
        public ContactPoint Model_ContactPoint
        {
            get
            {
                if (Model != null)
                {
                    return (ContactPoint)Model;
                }

                return null;
            }
            set
            {
                Model = value;
            }
        }
        public void SetEmailAddressAsRequired()
        {
            UIColors.SetRequiredBgColor(mtbEmail);
        }
        public void SetEmailAddressAsPreferred()
        {
            UIColors.SetPreferredBgColor(mtbEmail);
        }
        public void SetEmailAddressAsNormal()
        {
            UIColors.SetNormalBgColor(mtbEmail);
        }
        public void SetEmailReasonAsNormal()
        {
            UIColors.SetNormalBgColor(cmbEmailReason);
        }
        public void PopulatePatientPortalOptInValue()
        {
            PatientPortalOptInPresenter.UpdateOptInSelectionOnTheView(SavedPatientPortalOptIn);
        }

        public void PopulateHospCommunicationOptInValue()
        {
            HospitalCommunicationOptInPresenter.UpdateOptInSelectionOnTheView(SavedHospitalCommunincationOptIn);
        }

       
        #endregion

        #region Properties

        public Account Model_Account
        {
            get { return (Account) Model; }
        }
        public ConditionOfService COSSignedFlag
        {
            get { return cmbCosSigned.SelectedItem as ConditionOfService; }
        }
        private NoticeOfPrivacyPracticeDocument NPPDocument
        {
            get { return ((Account) Model).Patient.NoticeOfPrivacyPracticeDocument; }
        }

        internal bool IsCosRequired
        {
            private get { return isCosRequired; }
            set { isCosRequired = value; }
        }
         
        #endregion

        #region Private Methods

        private void registerEventHandlers()
        {
            if (!i_Registered)
            {
                i_Registered = true;

                RuleEngine.GetInstance().RegisterEvent(typeof (COSSignedPreferred),
                                                       new EventHandler(COSSignedPreferredEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof (COSSignedRequired),
                                                       new EventHandler(COSSignedRequiredEventHandler));

                RuleEngine.GetInstance().RegisterEvent(typeof (NPPVersionRequired),
                                                       new EventHandler(NPPVersionRequiredEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof(NPPVersionPreferred),
                                                       new EventHandler(NPPVersionPreferredEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof (InvalidNPPVersion),
                                                       new EventHandler(InvalidNPPVersionEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof (InvalidNPPVersionChange),
                                                       new EventHandler(InvalidNPPVersionChangeEventHandler));

                RuleEngine.GetInstance().RegisterEvent(typeof (NPPSignatureStatusRequired),
                                                       new EventHandler(NPPSignatureStatusRequiredEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof(NPPSignedOnDatePreferred),
                                                       new EventHandler(NPPSignedOnDatePreferredEventHandler));

                RuleEngine.GetInstance().RegisterEvent(typeof (NPPSignedOnDateRequired),
                                                       new EventHandler(NPPSignedOnDateRequiredEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof (NPPSignedOnDateInvalid),
                                                       new EventHandler(NPPSignedOnDateInvalidEventHandler));

                RuleEngine.GetInstance().RegisterEvent(typeof (InvalidConfidentialStatus),
                                                       new EventHandler(InvalidConfidentialStatusEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof (InvalidConfidentialStatusChange),
                                                       new EventHandler(InvalidConfidentialStatusChangeEventHandler));

                RuleEngine.GetInstance().RegisterEvent(typeof (InvalidCOS), new EventHandler(InvalidCOSEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof (InvalidCOSChange),
                                                       new EventHandler(InvalidCOSChangeEventHandler));

                RuleEngine.GetInstance().RegisterEvent(typeof (InvalidFacilityDeterminedFlag),
                                                       new EventHandler(InvalidFacilityDeterminedFlagEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof (InvalidFacilityDeterminedFlagChange),
                                                       new EventHandler(InvalidFacilityDeterminedFlagChangeEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof(EmailAddressRequired),
                                         new EventHandler(EmailAddressRequiredEventHandler));
                RuleEngine.GetInstance().RegisterEvent(typeof(EmailAddressPreferred),
                                         new EventHandler(EmailAddressPreferredEventhandler));
                RuleEngine.GetInstance().RegisterEvent(typeof(EmailReasonRequired),
                                         new EventHandler(EmailReasonRequiredEventhandler));
                RuleEngine.GetInstance().RegisterEvent(typeof(EmailReasonPreferred), 
                                         new EventHandler(EmailReasonPreferredEventhandler));
                RuleEngine.GetInstance().RegisterEvent(typeof(NotifyPCPDataRequired),
                                         new EventHandler(NotifyPCPDataRequiredEventhandler));
                RuleEngine.GetInstance().RegisterEvent(typeof(ShareDataWithPublicHIERequired), 
                                        new EventHandler(ShareHIEDataRequiredEventhandler));
            }
        }

        private void unregisterEventHandlers()
        {
            i_Registered = false;

            RuleEngine.GetInstance().UnregisterEvent(typeof (COSSignedRequired),
                                                     new EventHandler(COSSignedRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (COSSignedPreferred),
                                                     new EventHandler(COSSignedPreferredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (NPPVersionRequired),
                                                     new EventHandler(NPPVersionRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(NPPVersionPreferred),
                                                     new EventHandler(NPPVersionPreferredEventHandler));

            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidNPPVersion),
                                                     new EventHandler(InvalidNPPVersionEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidNPPVersionChange),
                                                     new EventHandler(InvalidNPPVersionChangeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (NPPSignedOnDateInvalid),
                                                     new EventHandler(NPPSignedOnDateInvalidEventHandler));

            RuleEngine.GetInstance().UnregisterEvent(typeof (NPPSignatureStatusRequired),
                                                     new EventHandler(NPPSignatureStatusRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (NPPSignedOnDatePreferred),
                                                     new EventHandler(NPPSignedOnDatePreferredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (NPPSignedOnDateRequired),
                                                     new EventHandler(NPPSignedOnDateRequiredEventHandler));

            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidConfidentialStatus),
                                                     new EventHandler(InvalidConfidentialStatusEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidConfidentialStatusChange),
                                                     new EventHandler(InvalidConfidentialStatusChangeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidCOS), new EventHandler(InvalidCOSEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidCOSChange),
                                                     new EventHandler(InvalidCOSChangeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidFacilityDeterminedFlag),
                                                     new EventHandler(InvalidFacilityDeterminedFlagEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof (InvalidFacilityDeterminedFlagChange),
                                                     new EventHandler(InvalidFacilityDeterminedFlagChangeEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(EmailAddressRequired),
                                          new EventHandler(EmailAddressRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(EmailAddressPreferred),
                                          new EventHandler(EmailAddressPreferredEventhandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(EmailReasonRequired),
                                     new EventHandler(EmailReasonRequiredEventhandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(EmailReasonPreferred),
                                     new EventHandler(EmailReasonPreferredEventhandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(NotifyPCPDataRequired),
                                     new EventHandler(NotifyPCPDataRequiredEventhandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(ShareDataWithPublicHIERequired),
                                        new EventHandler(ShareHIEDataRequiredEventhandler));
        }

        private void runRules()
        {
            setNormalBgColor(cmbNppVersion);
            setNormalBgColor(mtbSignedOnDate);
            setNormalBgColor(cmbConfidentialityStatus);
            setNormalBgColor(cmbCosSigned);
            setNormalBgColor(cmbFacilityFlag);
            hieShareDataFlagView.SetNotifyPCPToNormalColor();
            RuleEngine.GetInstance().EvaluateRule(typeof (COSSignedPreferred), Model_Account);

            RuleEngine.GetInstance().EvaluateRule(typeof (OnRegulatoryForm), Model_Account);
            RuleEngine.GetInstance().EvaluateRule(typeof(NotifyPCPDataRequired), Model_Account);
            RuleEngine.GetInstance().EvaluateRule(typeof(ShareDataWithPublicHIERequired), Model_Account);
        }

        /// <summary>
        /// handlePatientTypeChanged - the KindOfVisit (PatientType) property on the account has changed,
        /// check to see if other fields are required based on the PatientType selected
        /// </summary>
        private void handlePatientTypeChanged()
        {
            // if PT != 9, NPP and COSSigned are required

            if (patientTypeRequiresNPP())
            {
                setRequiredBgColor(cmbNppVersion);
                setRequiredBgColor(cmbCosSigned);
            }
            else
            {
                setNormalBgColor(cmbNppVersion);
                setNormalBgColor(cmbCosSigned);
            }
        }

        /// <summary>
        /// patientTypeRequiresNPP - determine if the PT selected dictates that NPPVersion is required.
        /// Return true or false.
        /// </summary>
        /// <returns></returns>
        private bool patientTypeRequiresNPP()
        {
            if (Model_Account.KindOfVisit != null
                && Model_Account.KindOfVisit.Code != VisitType.NON_PATIENT
                && (cmbNppVersion.SelectedItem == null
                    || cmbCosSigned.SelectedItem == null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// handleHospitalServiceChanged - the HospitalService property on the account has changed,
        /// check to see if other fields are required based on the new value
        /// </summary>
        private void handleHospitalServiceChanged()
        {
            if (Model_Account.HospitalService == null
                || Model_Account.HospitalService.Code.Equals("SP")
                || Model_Account.HospitalService.Code.Equals("LB")
                || Model_Account.HospitalService.Code.Equals("AB"))
            {
                // not required, unless already required due to PT

                if (!patientTypeRequiresNPP())
                {
                    setNormalBgColor(cmbNppVersion);
                    setNormalBgColor(cmbCosSigned);
                }
            }
            else
            {
                // NPPVersion and COSSigned are required 

                if (cmbNppVersion.SelectedItem == null
                    || cmbNppVersion.SelectedItem.ToString() == string.Empty)
                {
                    setRequiredBgColor(cmbNppVersion);
                }
                else
                {
                    setNormalBgColor(cmbNppVersion);
                }

                if (cmbCosSigned.SelectedItem == null
                    || cmbCosSigned.SelectedItem.ToString() == string.Empty)
                {
                    setRequiredBgColor(cmbCosSigned);
                }
                else
                {
                    setNormalBgColor(cmbCosSigned);
                }
            }
        }

        private void PopulateCOSComboBox()
        {
            var broker = BrokerFactory.BrokerOfType<IConditionOfServiceBroker>();
            cmbCosSigned.Items.Clear();

            foreach (ConditionOfService o in broker.AllConditionsOfService())
            {
                cmbCosSigned.Items.Add(o);
            }
        }

        private void PopulateFacilityFlagComboBox()
        {
            cmbFacilityFlag.Items.Clear();
            var broker = new FacilityFlagBrokerProxy();

            foreach (FacilityDeterminedFlag flag in broker.FacilityFlagsFor(User.GetCurrent().Facility.Oid))
            {
                cmbFacilityFlag.Items.Add(flag);
            }

            if (cmbFacilityFlag.Items.Count <= 1)
            {
                cmbFacilityFlag.Enabled = false;
            }
        }

        private void PopulateConfidentialityStatusComboBox()
        {
            var broker = BrokerFactory.BrokerOfType<IConfidentialCodeBroker>();
            cmbConfidentialityStatus.Items.Clear();

            foreach (ConfidentialCode o in broker.ConfidentialCodesFor(User.GetCurrent().Facility.Oid))
            {
                cmbConfidentialityStatus.Items.Add(o);
            }
        }

        private void PopulateNppComboBox()
        {
            var broker = BrokerFactory.BrokerOfType<INPPVersionBroker>();
            cmbNppVersion.Items.Clear();

            foreach (NPPVersion version in broker.NPPVersionsFor(User.GetCurrent().Facility.Oid))
            {
                cmbNppVersion.Items.Add(version);
            }
        }

        /// <summary>
        /// setNormalBgColor - set the background color to 'normal' for the control passed.
        /// </summary>
        /// <param name="field"></param>
        private void setNormalBgColor(Control field)
        {
            UIColors.SetNormalBgColor(field);
            Refresh();
        }

        /// <summary>
        /// setRequiredBgColor - set the background color to the required field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        private void setRequiredBgColor(Control field)
        {
            UIColors.SetRequiredBgColor(field);
            Refresh();
        }

        /// <summary>
        /// setPreferredBgColor - set the background color to the preferred field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        private void setPreferredBgColor(Control field)
        {
            UIColors.SetPreferredBgColor(field);
            Refresh();
        }

        /// <summary>
        /// setErrorBgColor - set the background color to the error field color for the control passed.
        /// </summary>
        /// <param name="field"></param>
        private void setErrorBgColor(Control field)
        {
            UIColors.SetErrorBgColor(field);
            Refresh();
        }
        private void SetEmailAddressAsNormalColorEventHandler(object sender, EventArgs e)
        {
            SetEmailAddressAsNormal();
        }
        private void DoNotShowEmailReason()
        {
            lblEmaiReason.Visible = false;
            cmbEmailReason.Visible = false;
        }
        private void RegisterOptInEvents()
        {
            patientPortalOptInView.SetEmailAddressAsNormalColorEvent += new EventHandler(this.SetEmailAddressAsNormalColorEventHandler);
            hospitalCommunicationOptInView.SetEmailAddressAsNormalColorEvent += new EventHandler(this.SetEmailAddressAsNormalColorEventHandler);
        }

        private RuleEngine RuleEngine
        {
            get { return i_RuleEngine ?? (i_RuleEngine = RuleEngine.GetInstance()); }
        }

        private IEmailReasonFeatureManager emailFeatureManager = new EmailReasonFeatureManager();
        private IEmailAddressFeatureManager emailAddressFeatureManager = new EmailAddressFeatureManager();
        private void HideEmailReason()
        {
            if (Model_Account.HideEmailReason ||
                !emailFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(Model_Account))
            {
                DoNotShowEmailReason();
            }
        }
        private void HandleEmailAddress()
        {
            var makeEmailAddressVisible = emailAddressFeatureManager.ShouldFeatureBeEnabled(Model_Account);
            if (Model_Account.HideEmailAddress || !makeEmailAddressVisible)
            {
                DoNotShowEmailAddress();
            }
        }
       
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegulatoryView));
            this.lblNppVersion = new System.Windows.Forms.Label();
            this.hieShareDataFlagView = new PatientAccess.UI.RegulatoryViews.ViewImpl.HIEShareDataFlagView();
            this.cobReceivedAndIMFMReceivedView = new PatientAccess.UI.RegulatoryViews.ViewImpl.COBReceivedAndIMFMReceivedView();
            this.cmbNppVersion = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.grpPrivacyOptions = new System.Windows.Forms.GroupBox();
            this.ckbReligion = new System.Windows.Forms.CheckBox();
            this.ckbHealthInfo = new System.Windows.Forms.CheckBox();
            this.ckbLocation = new System.Windows.Forms.CheckBox();
            this.ckbAllInformation = new System.Windows.Forms.CheckBox();
            this.lblOptOutText = new System.Windows.Forms.Label();
            this.cmbConfidentialityStatus = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblConfidentStatus = new System.Windows.Forms.Label();
            this.lblCosSigned = new System.Windows.Forms.Label();
            this.cmbCosSigned = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.mtbEmail = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbEmailReason = new System.Windows.Forms.ComboBox();
            this.lblEmaiReason = new System.Windows.Forms.Label();
            this.patientPortalOptInView = new PatientAccess.UI.RegulatoryViews.ViewImpl.PatientPortalOptInView();
            this.AuthorizeAdditionalPortalUsersView = new PatientAccess.UI.RegulatoryViews.ViewImpl.AuthorizeAdditionalPortalUsersView();
            this.hospitalCommunicationOptInView = new PatientAccess.UI.RegulatoryViews.ViewImpl.HospitalCommunicationOptInView();
            this.lblFacility = new System.Windows.Forms.Label();
            this.cmbFacilityFlag = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.grpNPP = new System.Windows.Forms.GroupBox();
            this.dtpSignedOnDate = new System.Windows.Forms.DateTimePicker();
            this.mtbSignedOnDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.rbRefusedToSign = new System.Windows.Forms.RadioButton();
            this.rbUnableToSign = new System.Windows.Forms.RadioButton();
            this.rbSigned = new System.Windows.Forms.RadioButton();
            this.lblSignatureStatus = new System.Windows.Forms.Label();
            this.ckbRightToRestrict = new System.Windows.Forms.CheckBox();
            this.grpPrivacyOptions.SuspendLayout();
            this.grpNPP.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNppVersion
            // 
            this.lblNppVersion.Location = new System.Drawing.Point(8, 24);
            this.lblNppVersion.Name = "lblNppVersion";
            this.lblNppVersion.Size = new System.Drawing.Size(80, 13);
            this.lblNppVersion.TabIndex = 0;
            this.lblNppVersion.Text = "NPP Version:";
            // 
            // hieShareDataFlagView
            // 
            this.hieShareDataFlagView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.hieShareDataFlagView.Location = new System.Drawing.Point(345, 220);
            this.hieShareDataFlagView.Margin = new System.Windows.Forms.Padding(5);
            this.hieShareDataFlagView.Model = null;
            this.hieShareDataFlagView.Name = "hieShareDataFlagView";
            this.hieShareDataFlagView.PatientAccount = null;
            this.hieShareDataFlagView.ShareDataWithPCP = null;
            this.hieShareDataFlagView.ShareDataWithPublicHIE = null;
            this.hieShareDataFlagView.Size = new System.Drawing.Size(241, 53);
            this.hieShareDataFlagView.TabIndex = 13;
            // 
            // cobReceivedAndIMFMReceivedView
            // 
            this.cobReceivedAndIMFMReceivedView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.cobReceivedAndIMFMReceivedView.Location = new System.Drawing.Point(345, 265);
            this.cobReceivedAndIMFMReceivedView.Margin = new System.Windows.Forms.Padding(4);
            this.cobReceivedAndIMFMReceivedView.Model = null;
            this.cobReceivedAndIMFMReceivedView.Name = "cobReceivedAndIMFMReceivedView";
            this.cobReceivedAndIMFMReceivedView.Presenter = null;
            this.cobReceivedAndIMFMReceivedView.Size = new System.Drawing.Size(219, 30);
            this.cobReceivedAndIMFMReceivedView.TabIndex = 14;
            this.cobReceivedAndIMFMReceivedView.TabStop = false;
            // 
            // cmbNppVersion
            // 
            this.cmbNppVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNppVersion.Location = new System.Drawing.Point(88, 20);
            this.cmbNppVersion.Name = "cmbNppVersion";
            this.cmbNppVersion.Size = new System.Drawing.Size(135, 39);
            this.cmbNppVersion.TabIndex = 0;
            this.cmbNppVersion.SelectedIndexChanged += new System.EventHandler(this.cmbNppVersion_SelectedIndexChanged);
            this.cmbNppVersion.Validating += new System.ComponentModel.CancelEventHandler(this.cmbNppVersion_Validating);
            // 
            // grpPrivacyOptions
            // 
            this.grpPrivacyOptions.Controls.Add(this.ckbReligion);
            this.grpPrivacyOptions.Controls.Add(this.ckbHealthInfo);
            this.grpPrivacyOptions.Controls.Add(this.ckbLocation);
            this.grpPrivacyOptions.Controls.Add(this.ckbAllInformation);
            this.grpPrivacyOptions.Controls.Add(this.lblOptOutText);
            this.grpPrivacyOptions.Controls.Add(this.cmbConfidentialityStatus);
            this.grpPrivacyOptions.Controls.Add(this.lblConfidentStatus);
            this.grpPrivacyOptions.Location = new System.Drawing.Point(0, 146);
            this.grpPrivacyOptions.Name = "grpPrivacyOptions";
            this.grpPrivacyOptions.Size = new System.Drawing.Size(304, 152);
            this.grpPrivacyOptions.TabIndex = 2;
            this.grpPrivacyOptions.TabStop = false;
            this.grpPrivacyOptions.Text = "Privacy options";
            // 
            // ckbReligion
            // 
            this.ckbReligion.Location = new System.Drawing.Point(120, 126);
            this.ckbReligion.Name = "ckbReligion";
            this.ckbReligion.Size = new System.Drawing.Size(150, 16);
            this.ckbReligion.TabIndex = 5;
            this.ckbReligion.Text = "Religion";
            this.ckbReligion.CheckedChanged += new System.EventHandler(this.ckbReligion_CheckedChanged);
            // 
            // ckbHealthInfo
            // 
            this.ckbHealthInfo.Location = new System.Drawing.Point(120, 106);
            this.ckbHealthInfo.Name = "ckbHealthInfo";
            this.ckbHealthInfo.Size = new System.Drawing.Size(150, 16);
            this.ckbHealthInfo.TabIndex = 4;
            this.ckbHealthInfo.Text = "Health information";
            this.ckbHealthInfo.CheckedChanged += new System.EventHandler(this.ckbHealthInfo_CheckedChanged);
            // 
            // ckbLocation
            // 
            this.ckbLocation.Location = new System.Drawing.Point(120, 86);
            this.ckbLocation.Name = "ckbLocation";
            this.ckbLocation.Size = new System.Drawing.Size(150, 16);
            this.ckbLocation.TabIndex = 3;
            this.ckbLocation.Text = "Location";
            this.ckbLocation.CheckedChanged += new System.EventHandler(this.ckbLocation_CheckedChanged);
            // 
            // ckbAllInformation
            // 
            this.ckbAllInformation.Location = new System.Drawing.Point(120, 66);
            this.ckbAllInformation.Name = "ckbAllInformation";
            this.ckbAllInformation.Size = new System.Drawing.Size(150, 16);
            this.ckbAllInformation.TabIndex = 2;
            this.ckbAllInformation.Text = "Name and all information";
            this.ckbAllInformation.CheckedChanged += new System.EventHandler(this.ckbAllInformation_CheckedChanged);
            // 
            // lblOptOutText
            // 
            this.lblOptOutText.Location = new System.Drawing.Point(8, 44);
            this.lblOptOutText.Name = "lblOptOutText";
            this.lblOptOutText.Size = new System.Drawing.Size(232, 23);
            this.lblOptOutText.TabIndex = 0;
            this.lblOptOutText.Text = "Opt out (exclude) from patient directory:";
            // 
            // cmbConfidentialityStatus
            // 
            this.cmbConfidentialityStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConfidentialityStatus.Location = new System.Drawing.Point(121, 16);
            this.cmbConfidentialityStatus.Name = "cmbConfidentialityStatus";
            this.cmbConfidentialityStatus.Size = new System.Drawing.Size(150, 39);
            this.cmbConfidentialityStatus.TabIndex = 1;
            this.cmbConfidentialityStatus.SelectedIndexChanged += new System.EventHandler(this.cmbConfidentialityStatus_SelectedIndexChanged);
            this.cmbConfidentialityStatus.Validating += new System.ComponentModel.CancelEventHandler(this.cmbConfidentialityStatus_Validating);
            // 
            // lblConfidentStatus
            // 
            this.lblConfidentStatus.Location = new System.Drawing.Point(8, 20);
            this.lblConfidentStatus.Name = "lblConfidentStatus";
            this.lblConfidentStatus.Size = new System.Drawing.Size(104, 23);
            this.lblConfidentStatus.TabIndex = 0;
            this.lblConfidentStatus.Text = "Confidential status:";
            // 
            // lblCosSigned
            // 
            this.lblCosSigned.Location = new System.Drawing.Point(345, 24);
            this.lblCosSigned.Name = "lblCosSigned";
            this.lblCosSigned.Size = new System.Drawing.Size(72, 13);
            this.lblCosSigned.TabIndex = 3;
            this.lblCosSigned.Text = "COS signed:";
            // 
            // cmbCosSigned
            // 
            this.cmbCosSigned.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCosSigned.Location = new System.Drawing.Point(453, 24);
            this.cmbCosSigned.Name = "cmbCosSigned";
            this.cmbCosSigned.Size = new System.Drawing.Size(214, 39);
            this.cmbCosSigned.TabIndex = 7;
            this.cmbCosSigned.SelectedIndexChanged += new System.EventHandler(this.cmbCosSigned_SelectedIndexChanged);
            this.cmbCosSigned.Validating += new System.ComponentModel.CancelEventHandler(this.cmbCosSigned_Validating);
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(345, 55);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(40, 23);
            this.lblEmail.TabIndex = 12;
            this.lblEmail.Text = "Email:";
            // 
            // mtbEmail
            // 
            this.mtbEmail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbEmail.Location = new System.Drawing.Point(453, 55);
            this.mtbEmail.Mask = "";
            this.mtbEmail.MaxLength = 64;
            this.mtbEmail.Name = "mtbEmail";
            this.mtbEmail.Size = new System.Drawing.Size(158, 38);
            this.mtbEmail.TabIndex = 8;
            this.mtbEmail.Validating += new System.ComponentModel.CancelEventHandler(this.mtbEmailAddress_Validating);
               
            // 
            // cmbEmailReason
            // 
            this.cmbEmailReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmailReason.Location = new System.Drawing.Point(453, 86);
            this.cmbEmailReason.Name = "cmbEmailReason";
            this.cmbEmailReason.Size = new System.Drawing.Size(170, 39);
            this.cmbEmailReason.TabIndex = 9;
            this.cmbEmailReason.SelectedIndexChanged += new System.EventHandler(this.cmbEmailReason_SelectedIdexchanged);
            this.cmbEmailReason.Validating += new System.ComponentModel.CancelEventHandler(this.cmbEmailReason_Validating);
            this.cmbEmailReason.KeyUp += new System.Windows.Forms.KeyEventHandler(cmbEmailReason_KeyUp);
            this.cmbEmailReason.KeyDown += new System.Windows.Forms.KeyEventHandler(cmbEmailReason_KeyDown);
            this.cmbEmailReason.MouseUp += new System.Windows.Forms.MouseEventHandler(cmbEmailReason_MouseUp);
            this.cmbEmailReason.MouseDown += new System.Windows.Forms.MouseEventHandler(cmbEmailReason_MouseDown);

            // 
            // lblEmaiReason
            // 
            this.lblEmaiReason.Location = new System.Drawing.Point(345, 86);
            this.lblEmaiReason.Name = "lblEmaiReason";
            this.lblEmaiReason.Size = new System.Drawing.Size(107, 27);
            this.lblEmaiReason.TabIndex = 16;
            this.lblEmaiReason.Text = "Email Reason:";
            // 
            // patientPortalOptInView
            // 
            this.patientPortalOptInView.Location = new System.Drawing.Point(345, 142);
            this.patientPortalOptInView.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.patientPortalOptInView.Model = null;
            this.patientPortalOptInView.Name = "patientPortalOptInView";
            this.patientPortalOptInView.PatientPortalOptInPresenter = null;
            this.patientPortalOptInView.Size = new System.Drawing.Size(564, 35);
            this.patientPortalOptInView.TabIndex = 11;
            // 
            // AuthorizeAdditionalPortalUsersView
            // 
            this.AuthorizeAdditionalPortalUsersView.Location = new System.Drawing.Point(345, 177);
            this.AuthorizeAdditionalPortalUsersView.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.AuthorizeAdditionalPortalUsersView.Model = null;
            this.AuthorizeAdditionalPortalUsersView.Name = "AuthorizeAdditionalPortalUsersView";
            this.AuthorizeAdditionalPortalUsersView.AuthorizeAdditionalPortalUsersPresenter = null;
            this.AuthorizeAdditionalPortalUsersView.Size = new System.Drawing.Size(564, 35);
            this.AuthorizeAdditionalPortalUsersView.TabIndex = 12;
            // 
            // hospitalCommunicationOptInView
            // 
            this.hospitalCommunicationOptInView.Location = new System.Drawing.Point(341, 109);
            this.hospitalCommunicationOptInView.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.hospitalCommunicationOptInView.Model = null;
            this.hospitalCommunicationOptInView.Name = "hospitalCommunicationOptInView";
            this.hospitalCommunicationOptInView.Presenter = null;
            this.hospitalCommunicationOptInView.Size = new System.Drawing.Size(500, 35);
            this.hospitalCommunicationOptInView.TabIndex = 10;
            // 
            // lblFacility
            // 
            this.lblFacility.Location = new System.Drawing.Point(8, 335);
            this.lblFacility.Name = "lblFacility";
            this.lblFacility.Size = new System.Drawing.Size(155, 26);
            this.lblFacility.TabIndex = 5;
            this.lblFacility.Text = "Facility-determined flag:";
            // 
            // cmbFacilityFlag
            // 
            this.cmbFacilityFlag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFacilityFlag.Location = new System.Drawing.Point(130, 335);
            this.cmbFacilityFlag.Name = "cmbFacilityFlag";
            this.cmbFacilityFlag.Size = new System.Drawing.Size(183, 39);
            this.cmbFacilityFlag.TabIndex = 6;
            this.cmbFacilityFlag.SelectedIndexChanged += new System.EventHandler(this.cmbFacilityFlag_SelectedIndexChanged);
            this.cmbFacilityFlag.Validating += new System.ComponentModel.CancelEventHandler(this.cmbFacilityFlag_Validating);
            // 
            // grpNPP
            // 
            this.grpNPP.Controls.Add(this.dtpSignedOnDate);
            this.grpNPP.Controls.Add(this.mtbSignedOnDate);
            this.grpNPP.Controls.Add(this.rbRefusedToSign);
            this.grpNPP.Controls.Add(this.rbUnableToSign);
            this.grpNPP.Controls.Add(this.rbSigned);
            this.grpNPP.Controls.Add(this.lblSignatureStatus);
            this.grpNPP.Location = new System.Drawing.Point(0, 0);
            this.grpNPP.Name = "grpNPP";
            this.grpNPP.Size = new System.Drawing.Size(304, 136);
            this.grpNPP.TabIndex = 1;
            this.grpNPP.TabStop = false;
            this.grpNPP.Text = "Notice of Privacy Practice";
            // 
            // dtpSignedOnDate
            // 
            this.dtpSignedOnDate.Enabled = false;
            this.dtpSignedOnDate.Location = new System.Drawing.Point(200, 68);
            this.dtpSignedOnDate.Name = "dtpSignedOnDate";
            this.dtpSignedOnDate.Size = new System.Drawing.Size(21, 38);
            this.dtpSignedOnDate.TabIndex = 2;
            this.dtpSignedOnDate.CloseUp += new System.EventHandler(this.dtpSignedOnDate_CloseUp);
            // 
            // mtbSignedOnDate
            // 
            this.mtbSignedOnDate.Enabled = false;
            this.mtbSignedOnDate.KeyPressExpression = "^\\d*$";
            this.mtbSignedOnDate.Location = new System.Drawing.Point(128, 68);
            this.mtbSignedOnDate.Mask = "  /  /";
            this.mtbSignedOnDate.MaxLength = 10;
            this.mtbSignedOnDate.Name = "mtbSignedOnDate";
            this.mtbSignedOnDate.Size = new System.Drawing.Size(72, 38);
            this.mtbSignedOnDate.TabIndex = 1;
            this.mtbSignedOnDate.ValidationExpression = resources.GetString("mtbSignedOnDate.ValidationExpression");
            this.mtbSignedOnDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbSignedOnDate_Validating);
            // 
            // rbRefusedToSign
            // 
            this.rbRefusedToSign.Enabled = false;
            this.rbRefusedToSign.Location = new System.Drawing.Point(32, 112);
            this.rbRefusedToSign.Name = "rbRefusedToSign";
            this.rbRefusedToSign.Size = new System.Drawing.Size(161, 20);
            this.rbRefusedToSign.TabIndex = 4;
            this.rbRefusedToSign.Text = "Patient unwilling to sign";
            this.rbRefusedToSign.Click += new System.EventHandler(this.rbRefusedToSign_Click);
            this.rbRefusedToSign.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbRefusedToSign_KeyDown);
            // 
            // rbUnableToSign
            // 
            this.rbUnableToSign.Enabled = false;
            this.rbUnableToSign.Location = new System.Drawing.Point(32, 90);
            this.rbUnableToSign.Name = "rbUnableToSign";
            this.rbUnableToSign.Size = new System.Drawing.Size(136, 20);
            this.rbUnableToSign.TabIndex = 3;
            this.rbUnableToSign.Text = "Patient unable to sign";
            this.rbUnableToSign.Click += new System.EventHandler(this.rbUnableToSign_Click);
            this.rbUnableToSign.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbUnableToSign_KeyDown);
            // 
            // rbSigned
            // 
            this.rbSigned.Enabled = false;
            this.rbSigned.Location = new System.Drawing.Point(32, 68);
            this.rbSigned.Name = "rbSigned";
            this.rbSigned.Size = new System.Drawing.Size(104, 20);
            this.rbSigned.TabIndex = 0;
            this.rbSigned.Text = "NPP Signed";
            this.rbSigned.Click += new System.EventHandler(this.rbSigned_Click);
            this.rbSigned.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbSigned_KeyDown);
            // 
            // lblSignatureStatus
            // 
            this.lblSignatureStatus.Location = new System.Drawing.Point(8, 48);
            this.lblSignatureStatus.Name = "lblSignatureStatus";
            this.lblSignatureStatus.Size = new System.Drawing.Size(117, 13);
            this.lblSignatureStatus.TabIndex = 0;
            this.lblSignatureStatus.Text = "NPP signature status:";
            // 
            // ckbRightToRestrict
            // 
            this.ckbRightToRestrict.Location = new System.Drawing.Point(8, 306);
            this.ckbRightToRestrict.Name = "ckbRightToRestrict";
            this.ckbRightToRestrict.Size = new System.Drawing.Size(296, 22);
            this.ckbRightToRestrict.TabIndex = 4;
            this.ckbRightToRestrict.Text = "Patient Requested Right to Restrict";
            this.ckbRightToRestrict.CheckedChanged += new System.EventHandler(this.ckbRightToRestrict_CheckedChanged);
            // 
            // RegulatoryView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ckbRightToRestrict);
            this.Controls.Add(this.cmbFacilityFlag);
            this.Controls.Add(this.lblFacility);
            this.Controls.Add(this.cmbCosSigned);
            this.Controls.Add(this.lblCosSigned);
            this.Controls.Add(this.grpPrivacyOptions);
            this.Controls.Add(this.cmbNppVersion);
            this.Controls.Add(this.lblNppVersion);
            this.Controls.Add(this.grpNPP);
            this.Controls.Add(this.hieShareDataFlagView);
            this.Controls.Add(this.cobReceivedAndIMFMReceivedView);
            this.Controls.Add(this.hospitalCommunicationOptInView);
            this.Controls.Add(this.patientPortalOptInView);
            this.Controls.Add(this.AuthorizeAdditionalPortalUsersView);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.mtbEmail);
            this.Controls.Add(this.lblEmaiReason);
            this.Controls.Add(this.cmbEmailReason);
            this.Location = new System.Drawing.Point(0, 1);
            this.Name = "RegulatoryView";
            this.Size = new System.Drawing.Size(1024, 380);
            this.Leave += new System.EventHandler(this.RegulatoryView_Leave);
            this.Disposed += new System.EventHandler(this.RegulatoryView_Disposed);
            this.grpPrivacyOptions.ResumeLayout(false);
            this.grpNPP.ResumeLayout(false);
            this.grpNPP.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #endregion

        #region Construction and Finalization

        public RegulatoryView()
        {
            InitializeComponent();
            emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            emailKeyPressExpression = new Regex(RegularExpressions.EmailValidCharactersExpression);
        }



        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (accountChangedListenerWired)
                {
                    Model_Account.ChangedListeners -= Model_Account_Changed;
                }

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
        private readonly Container components = null;

        private RightToRestrictPresenter RightToRestrictPresenter { get; set; }
        private RegulatoryPresenter RegulatoryPresenter { get; set; }
        private COBReceivedAndIMFMReceivedPresenter COBReceivedAndIMFMReceivedPresenter { get; set; }
        private HospitalCommunicationOptInPresenter HospitalCommunicationOptInPresenter { get; set; }
        private PatientPortalOptInPresenter PatientPortalOptInPresenter { get; set; }
        private AuthorizeAdditionalPortalUsersPresenter AuthorizeAdditionalPortalUsersPresenter { get; set; }
        private COSSignedHandler COSsignedHandler { get; set; }

        private readonly DateTime earliestDate = new DateTime(1800, 01, 01);
        private bool accountChangedListenerWired;
        private bool blnLeaveRun;

        private CheckBox ckbAllInformation;
        private CheckBox ckbHealthInfo;
        private CheckBox ckbLocation;
        private CheckBox ckbReligion;
        private PatientAccessComboBox cmbConfidentialityStatus;
        private PatientAccessComboBox cmbCosSigned;
        private PatientAccessComboBox cmbFacilityFlag;
        private PatientAccessComboBox cmbNppVersion;
        private DateTimePicker dtpSignedOnDate;
        private GroupBox grpNPP;

        private GroupBox grpPrivacyOptions;
        private bool i_Registered;
        private bool isCosRequired = true;
        private Label lblConfidentStatus;

        private Label lblCosSigned;
        private Label lblFacility;
        private Label lblNppVersion;
        private Label lblOptOutText;
        private Label lblSignatureStatus;
        private HIEShareDataFlagView hieShareDataFlagView;
        private COBReceivedAndIMFMReceivedView cobReceivedAndIMFMReceivedView;
        private bool loadingModelData = true;
        private bool loadingEmailReasonData = true;
        private bool emailReasonSelected;
      
        private MaskedEditTextBox mtbSignedOnDate;
        private bool optOutAllInformation;
        private bool optOutHealthInformation;
        private bool optOutLocation;
        private bool optOutReligion;
        private RadioButton rbRefusedToSign;
        private RadioButton rbSigned;
        private RadioButton rbUnableToSign;
        private string signedOnDateTxt;
        private int verifyDay;
        private int verifyMonth;
        private CheckBox ckbRightToRestrict;
        private int verifyYear;
        private Label lblEmail;
        private MaskedEditTextBox mtbEmail;
        private ComboBox cmbEmailReason;
        private Label lblEmaiReason;
        private readonly Regex emailValidationExpression;
        private readonly Regex emailKeyPressExpression;
        
        private EmailAddressPresenter EmailAddressPresenter { get; set; }
        private EmailReasonPresenter EmailReasonPresenter { get; set; }
        private HospitalCommunicationOptInView hospitalCommunicationOptInView;
        private PatientPortalOptInView patientPortalOptInView;
        private AuthorizeAdditionalPortalUsersView AuthorizeAdditionalPortalUsersView;
        private RuleEngine i_RuleEngine;
        private string SavedPatientPortalOptIn { get; set; }
        private string SavedHospitalCommunincationOptIn { get; set; }

        #endregion

        private void ckbRightToRestrict_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null)
                return;
            RightToRestrictPresenter.SetRightToRestrict(cb.Checked);
        }

        #region Constants

        #endregion


       
    }
}