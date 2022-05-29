using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.ServiceCategory.Presenter;
using PatientAccess.UI.CptCodes.Presenters;
using PatientAccess.UI.CptCodes.ViewImpl;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews.DOFR.Presenter;

namespace PatientAccess.UI.DiagnosisViews
{
    /// <summary>
    /// Summary description for DiagnosisView.
    /// </summary>
    [Serializable]
    public class DiagnosisView : ControlView, IEDLogView, IDiagnosisView, IAlternateCareFacilityView
    {
        #region Delegates
        public delegate void DisplayAbstractCondition();
        #endregion

        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers
 
        void DiagnosisView_Load( object sender, EventArgs e )
        {
            this.patientTypeHSVLocationView.SetTenetCare += this.patientTypeHSVLocationView_SetTenetCare;
            this.patientTypeHSVLocationView.ClearSelectedClinics += this.patientTypeHSVLocationView_ClearSelectedClinics;
            this.patientTypeHSVLocationView.SelectPreviouslyStoredClinicValue += this.patientTypeHSVLocationView_SelectPreviouslyStoredClinicValue;
            patientTypeHSVLocationView.PatientTypeChanged += EnableOrDisableEdLogFields;
            patientTypeHSVLocationView.PatientTypeChanged += EvaluateMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage;
            patientTypeHSVLocationView.PatientTypeChanged += EnableOrDisableProcedureField;
            patientTypeHSVLocationView.PatientTypeChanged += SetTabStopForInpatient;
            patientTypeHSVLocationView.PatientTypeChanged += SetDOFRInitiatedForPTChange;
            this.patientTypeHSVLocationView.HSVChanged += SetDOFRInitiatedForHSVChange;
        }

        private void EvaluateMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage( object sender, VisitTypeEventArgs e )
        {
            if (!loadingModelData)
            {
                RuleEngine.OneShotRuleEvaluation<MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage>(
                    Model, MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler );
            }
        }
 
        private void MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler( object sender, EventArgs e )
        {
            var  messageDisplayHandler = new ErrorMessageDisplayHandler( Model );
            messageDisplayHandler.DisplayOkWarningMessageFor( typeof( MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage ) );

        }

        private void DiagnosisView_Leave( object sender, EventArgs e )
        {
            this.blnLeaveRun = true;
            RuleEngine.EvaluateRule( typeof( OnDiagnosisForm ), this.Model );
            this.blnLeaveRun = false;
        }

        internal void EnableOrDisableEdLogFields( object sender, VisitTypeEventArgs e )
        {
            bool edLogFieldsHaveData = this.HasData();

            const bool isViewBeingUpdated = false;

            EdLogDisplayPresenter.UpdateEDLogDisplay( e.VisitType, isViewBeingUpdated, edLogFieldsHaveData );
        }

        /// <summary>
        /// Enables the or disable procedure field.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PatientAccess.UI.VisitTypeEventArgs"/> instance containing the event data.</param>
        private void EnableOrDisableProcedureField(object sender, VisitTypeEventArgs e)
        {
            this.DiagnosisViewPresenter.HandleProcedureField(e.VisitType,true);
        }

        private void NoPrimaryMedicareForAutoAccidentEventHandler( object sender, EventArgs e )
        {
            IErrorMessageDisplayHandler messageDisplayHandler = new ErrorMessageDisplayHandler( Model );
            DialogResult warningResult =
            messageDisplayHandler.DisplayYesNoErrorMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );
            if (warningResult == DialogResult.No)
            {
                return;
            }
            if (EnableInsuranceTab != null)
            {
                EnableInsuranceTab( this, new LooseArgs( Model ) );
            }
        }

        private void patientTypeHSVLocationView_SetTenetCare( object sender, EventArgs e )
        {
            this.Model.KindOfVisit = this.patientTypeHSVLocationView.Model.KindOfVisit;
            if (this.Model.KindOfVisit == null || this.Model.KindOfVisit.Code != VisitType.OUTPATIENT)
            {
                this.SetTenetCareStatus( false );
                this.cmbTenetCare.SelectedIndex = 0;
            }
            else
            {
                this.IsTenetCareFacility();
            }
        }

        private void patientTypeHSVLocationView_ClearSelectedClinics( object sender, EventArgs e )
        {
            this.Model.KindOfVisit = this.patientTypeHSVLocationView.Model.KindOfVisit;
            this.ClearSelectedClinics( this.Model.KindOfVisit );
        }

        private void patientTypeHSVLocationView_SelectPreviouslyStoredClinicValue( object sender, EventArgs e )
        {
            this.SelectPreviouslyStoredClinicValue();
        }

        private void SetTenetCareStatus( bool isEnabled )
        {
            UIColors.SetNormalBgColor( this.cmbTenetCare );
            this.cmbTenetCare.Enabled = isEnabled;

            if (this.Model.TenetCare != null)
            {
                for (int i = 0; i < this.cmbTenetCare.Items.Count; i++)
                {
                    YesNoFlag yesNoFlag = this.cmbTenetCare.Items[i] as YesNoFlag;
                    if (yesNoFlag != null && Model.TenetCare.Code == yesNoFlag.Code)
                    {
                        this.cmbTenetCare.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void IsTenetCareFacility()
        {
            if (this.Model.Facility.TenetCare.Code != YesNoFlag.CODE_YES)
            {
                this.cmbTenetCare.Visible = false;
                this.lblTenetCare.Visible = false;
            }
            else
            {
                if (this.Model.KindOfVisit == null || this.Model.KindOfVisit.Code != VisitType.OUTPATIENT)
                {
                    this.SetTenetCareStatus( false );
                    this.cmbTenetCare.SelectedIndex = 0;
                }
                else
                {
                    this.SetTenetCareStatus( true );
                    RuleEngine.EvaluateRule( typeof( TenetCareRequired ), this.Model );
                }
            }
        }

        private void DiagnosisView_Enter( object sender, EventArgs e )
        {
            IAccountView accountView = AccountView.GetInstance();

            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if (accountView.IsMedicareAdvisedForPatient())
            {
                accountView.MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                    UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

                if (warningResult == DialogResult.Yes)
                {
                    if (this.EnableInsuranceTab != null)
                    {
                        this.EnableInsuranceTab( this, new LooseArgs( this.Model ) );
                    }
                }
            }
        }

        private void mtbComplaint_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( this.mtbComplaint );
            this.Model.Diagnosis.ChiefComplaint = this.mtbComplaint.Text;

            RuleEngine.EvaluateRule( typeof( ChiefComplaintRequired ), this.Model );
        }

        private void mtbAccidentCrimeDate_Validating( object sender, CancelEventArgs e )
        {
            if (this.mtbAccidentCrimeDate.UnMaskedText == String.Empty)
            {
                //set OccurredOn datetime value to min datetime value.
                if (this.Model.Diagnosis.Condition.GetType() == typeof( Accident ) ||
                    this.Model.Diagnosis.Condition.GetType() == typeof( Crime ))
                {
                    TimeAndLocationBoundCondition condition = (TimeAndLocationBoundCondition)this.Model.Diagnosis.Condition;
                    condition.OccurredOn = DateTime.MinValue;
                }
            }

            if (this.dateTimePickerAccident.Focused == false)
            {
                if (this.VerifyAccidentCrimeDate())
                {
                    this.CheckForRequiredAccidentFields();
                }

                bool accidentCrimeDateRulePassed = ApplyAdmitDateReqdWithAccidentCrimeDateRule();
                if (accidentCrimeDateRulePassed == false)
                {
                    this.SetAccidentCrimeDateErrBgColor();
                }
            }
        }

        private void mtbOnsetDate_Validating( object sender, CancelEventArgs e )
        {
            bool illnessOnSetRulePassed = true;
            if (this.dateTimePickerSickness.Focused == false)
            {
                if (this.VerifyOnsetDate())
                {
                    UIColors.SetNormalBgColor( this.mtbOnsetDate );
                    this.Refresh();
                    RuleEngine.EvaluateRule( typeof( OnsetDateOfSymptomsOrIllnessRequired ), this.Model );
                }
                illnessOnSetRulePassed = this.ApplyAdmitDateReqdWithIllnessOnSetDateRule();
            }
            if (illnessOnSetRulePassed == false)
            {
                this.SetOnsetDateErrBgColor();
            }
        }

        /// <summary>
        /// private handler for rule: AccidentOrCrimeDateWithNoAdmitDate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccidentOrCrimeDateWithNoAdmitDateEventHandler( object sender, EventArgs e )
        {
            GetFocusAndHighlightRedError( this.mtbAccidentCrimeDate );
            SetAccidentCrimeDateErrBgColor();
            ShowErrorMessageBox( UIErrorMessages.OCCURRENCE_CODE_DATE_FUTURE_ERRMSG );
            this.EnableAccidentCrimeDetails();
            this.CheckForRequiredAccidentFields();
        }

        /// <summary>
        /// private handler for rule: OnsetOfSymptomsOrIllnessWithNoAdmitDate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnsetOfSymptomsOrIllnessWithNoAdmitDateEventHandler( object sender, EventArgs e )
        {
            GetFocusAndHighlightRedError( this.mtbOnsetDate );
            ShowErrorMessageBox( UIErrorMessages.OCCURRENCE_CODE_DATE_FUTURE_ERRMSG );
        }

        /// <summary>
        /// private method to show a customized error message box.
        /// </summary>
        private static void ShowErrorMessageBox( string msg )
        {
            MessageBox.Show( msg, "Error",
                             MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                             MessageBoxDefaultButton.Button1 );
        }

        private void cmbHour_Validating( object sender, CancelEventArgs e )
        {
            this.CheckForRequiredAccidentFields();
        }

        private void cmbCountry_Validating( object sender, CancelEventArgs e )
        {
            this.CheckForRequiredAccidentFields();
        }

        private void cmbState_Validating( object sender, CancelEventArgs e )
        {
            this.CheckForRequiredAccidentFields();
        }

        private void mtbArrivalTime_Validating( object sender, CancelEventArgs e )
        {
            this.IsValidateArrivalTime();
        }

        private void cmbModeOfArrival_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedModeOfArrival = this.cmbModeOfArrival.SelectedItem as ModeOfArrival;
            if (selectedModeOfArrival != null)
            {
                EdLogDisplayPresenter.UpdateSelectedModeOfArrival(selectedModeOfArrival);
            }
        }

       
        private void cmbReferralType_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.selectedReferralType = this.cmbReferralType.SelectedItem as ReferralType;
            if (this.selectedReferralType != null)
            {
                this.Model.ReferralType = this.selectedReferralType;
            }
        }

        private void cmbReferralFacility_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.selectedReferralFacility = this.cmbReferralFacility.SelectedItem as ReferralFacility;
            if (this.selectedReferralFacility != null)
            {
                this.Model.ReferralFacility = this.selectedReferralFacility;
            }
        }

        private void cmbReadmitCode_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.selectedReAdmitCode = this.cmbReadmitCode.SelectedItem as ReAdmitCode;
            if (this.selectedReAdmitCode != null)
            {
                this.Model.ReAdmitCode = this.selectedReAdmitCode;
            }
        }

        private void cmbTenetCare_SelectedIndexChanged_1( object sender, EventArgs e )
        {
            this.Model.TenetCare = this.cmbTenetCare.SelectedItem as YesNoFlag;

            RuleEngine.RegisterEvent( typeof( TenetCareRequired ), new EventHandler( this.TenetCareRequiredEventHandler ) );
        }

        private void cmbTenetCare_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbTenetCare );
            RuleEngine.EvaluateRule( typeof( TenetCareRequired ), this.Model );
        }

        private void cmbAdmitSrc_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleAdmitSourceSelectedIndexChanged(this.cmbAdmitSrc.SelectedItem as AdmitSource);
        }

        private void HandleAdmitSourceSelectedIndexChanged(AdmitSource newAdmitSource)
        {
            if (newAdmitSource != null)
            {
                this.Model.AdmitSource = newAdmitSource;
            }

            RuleEngine.EvaluateRule(typeof(AdmitSourceRequired), this.Model);
            RuleEngine.EvaluateRule(typeof(AdmitSourcePreferred), this.Model);
            AlternateCareFacilityPresenter.HandleAlternateCareFacility();
        }
        private void clinicView1_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            this.Model.Clinics[0] = selectedHospitalClinic;
            this.HandleClinicIndexChange( this.clinicView1.cmbClinic );
            RuleEngine.EvaluateRule( typeof( DiagnosisClinicOneRequired ), this.Model );
            ServiceCategoryPresenter.UpdateView();
            DOFRInitiatePresenter.SetDOFRInitiated(this.Model);
        }

        private void clinicView2_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            this.Model.Clinics[1] = selectedHospitalClinic;
            this.HandleClinicIndexChange( this.clinicView2.cmbClinic );
        }

        private void clinicView3_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            this.Model.Clinics[2] = selectedHospitalClinic;
            this.HandleClinicIndexChange( this.clinicView3.cmbClinic );
        }

        private void clinicView4_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            this.Model.Clinics[3] = selectedHospitalClinic;
            this.HandleClinicIndexChange( this.clinicView4.cmbClinic );
        }

        private void clinicView5_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            this.Model.Clinics[4] = selectedHospitalClinic;
            this.HandleClinicIndexChange( this.clinicView5.cmbClinic );
        }
        /// <summary>
        /// Click on Radio Button Accident
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbAccident_Click( object sender, EventArgs e )
        {
            if (this.Model.Diagnosis.Condition == null || this.Model.Diagnosis.Condition.GetType() != typeof( Accident ))
            {
                Accident accident = new Accident();
                this.Model.Diagnosis.Condition = accident;
                this.cmbState.SelectedIndex = 0;
            }

            this.ClearOccurrenceCodes();
            this.DisplayAccident();
            this.CheckForRequiredAccidentFields();
        }

        /// <summary>
        /// Click on Radio Button Crime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbCrime_Click( object sender, EventArgs e )
        {
            this.cmbAccidentType.Enabled = false;
            this.CheckPreviousCondition();

            if (this.Model.Diagnosis.Condition == null || this.Model.Diagnosis.Condition.GetType() != typeof( Crime ))
            {
                Crime crime = new Crime();
                this.Model.Diagnosis.Condition = crime;
                this.cmbState.SelectedIndex = 0;
            }

            this.ClearOccurrenceCodes();
            this.DisplayCrime();
            this.CheckForRequiredAccidentFields();
        }

        /// <summary>
        /// Click on Radio Button None
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNone_Click( object sender, EventArgs e )
        {
            bool onSetEnabled = true;
            this.CheckPreviousCondition();

            if (this.Model.Diagnosis.Condition == null || this.Model.Diagnosis.Condition.GetType() != typeof( UnknownCondition ))
            {
                UnknownCondition unKnownCondition = new UnknownCondition { Onset = DateTime.MinValue };
                this.Model.Diagnosis.Condition = unKnownCondition;
            }

            this.ClearOccurrenceCodes();
            this.DisplayCondition();

            // TLG 04/21/2006 - not necessary
            //this.LocalUpdateView();

            this.CheckForRequiredAccidentFields();

            if (typeof( AdmitNewbornActivity ).Equals( this.Model.Activity.GetType() ) )
            {
                onSetEnabled = false;
                UIColors.SetDisabledDarkBgColor( this.mtbOnsetDate );
            }

            this.mtbOnsetDate.Enabled = onSetEnabled;
            this.dateTimePickerSickness.Enabled = onSetEnabled;
        }

        private void rbAccident_KeyDown( object sender, KeyEventArgs e )
        {
            if (e.KeyData == Keys.Right || e.KeyData == Keys.Down)
            {
                this.rbCrime.Focus();
            }
            else if (e.KeyData == Keys.Left || e.KeyData == Keys.Up)
            {
                this.rbNone.Focus();
            }
        }

        private void rbCrime_KeyDown( object sender, KeyEventArgs e )
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Up)
            {
                this.rbAccident.Focus();
            }
            else if (e.KeyData == Keys.Right || e.KeyData == Keys.Down)
            {
                this.rbNone.Focus();
            }
        }

        private void rbNone_KeyDown( object sender, KeyEventArgs e )
        {
            if (e.KeyData == Keys.Right || e.KeyData == Keys.Down)
            {
                this.rbAccident.Focus();
            }
            else if (e.KeyData == Keys.Left || e.KeyData == Keys.Up)
            {
                this.rbCrime.Focus();
            }
        }

        /// <summary>
        /// Private helper method.
        /// </summary>
        private bool ApplyAdmitDateReqdWithAccidentCrimeDateRule()
        {
            RuleEngine.RegisterEvent( typeof( AccidentOrCrimeDateWithNoAdmitDate ),
                new EventHandler( this.AccidentOrCrimeDateWithNoAdmitDateEventHandler ) );

            bool success = RuleEngine.EvaluateRule( typeof( AccidentOrCrimeDateWithNoAdmitDate ), Model );

            RuleEngine.UnregisterEvent( typeof( AccidentOrCrimeDateWithNoAdmitDate ),
                new EventHandler( this.AccidentOrCrimeDateWithNoAdmitDateEventHandler ) );
            return success;
        }

        /// <summary>
        /// Private helper method.
        /// </summary>
        private bool ApplyAdmitDateReqdWithIllnessOnSetDateRule()
        {
            RuleEngine.RegisterEvent( typeof( OnsetOfSymptomsOrIllnessWithNoAdmitDate ),
                new EventHandler( this.OnsetOfSymptomsOrIllnessWithNoAdmitDateEventHandler ) );

            bool success = RuleEngine.EvaluateRule( typeof( OnsetOfSymptomsOrIllnessWithNoAdmitDate ), Model );

            RuleEngine.UnregisterEvent( typeof( OnsetOfSymptomsOrIllnessWithNoAdmitDate ),
                new EventHandler( this.OnsetOfSymptomsOrIllnessWithNoAdmitDateEventHandler ) );
            return success;
        }

        /// <summary>
        /// Date Time Picker Accident CloseUp
        /// </summary>
        private void dateTimePickerAccident_CloseUp( object sender, EventArgs e )
        {
            this.SetAccidentCrimeDateNormalBgColor();
            this.SetAccidentCrimeDate();

            if (this.dateTimePickerAccident.Checked)
            {
                DateTime dt = this.dateTimePickerAccident.Value;
                this.mtbAccidentCrimeDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
                this.accidentDateTxt = this.mtbAccidentCrimeDate.Text;
                this.VerifyAccidentCrimeDate();
            }
            else
            {
                this.mtbAccidentCrimeDate.Text = String.Empty;
                this.accidentDateTxt = String.Empty;
            }

            bool wasAccidentCrimeDateVerified = this.ApplyAdmitDateReqdWithAccidentCrimeDateRule();
            if (wasAccidentCrimeDateVerified == false)
            {
                this.SetAccidentCrimeDateErrBgColor();
            }
            else
            {
                this.CheckForRequiredAccidentFields();
            }
        }

        /// <summary>
        /// Date Time Picker Close Up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePickerSickness_CloseUp( object sender, EventArgs e )
        {
            this.SetOnsetDateNormalBgColor();
            if (this.dateTimePickerSickness.Checked)
            {
                DateTime dt = this.dateTimePickerSickness.Value;
                this.mtbOnsetDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
                this.onsetDateTxt = this.mtbOnsetDate.Text;
                this.VerifyOnsetDate();
            }
            else
            {
                this.mtbOnsetDate.Text = String.Empty;
                this.onsetDateTxt = String.Empty;
            }

            bool wasOnSetDateVerified = this.ApplyAdmitDateReqdWithIllnessOnSetDateRule();
            if (wasOnSetDateVerified == false)
            {
                this.SetOnsetDateErrBgColor();
            }
        }

        /// <summary>
        /// Combo Box Accident Type Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbAccidentType_SelectedIndexChanged( object sender, EventArgs e )
        {
            Accident accident = (Accident)this.Model.Diagnosis.Condition;
            if (this.cmbAccidentType.SelectedItem != null &&
                 this.cmbAccidentType.SelectedItem.ToString() != String.Empty)
            {
                TypeOfAccident typeOfAccident = ( (DictionaryEntry)this.cmbAccidentType.SelectedItem ).Value as TypeOfAccident;
                accident.Kind = typeOfAccident;
                this.accidentType = typeOfAccident;

                if (typeOfAccident != null)
                {
                    if (typeOfAccident.Oid == TypeOfAccident.EMPLOYMENT_RELATED ||
                        typeOfAccident.Description == "Employment Related")
                    {
                        this.IsValidAccidentTypeForInsurance();
                    }
                    if (!loadingModelData)
                    {
                        EvaluateNoMedicarePrimaryPayorForAutoAccidentRule( NoPrimaryMedicareForAutoAccidentEventHandler );
                    }
                }
            }
            else
            {
                accident.Kind = null;
            }

            this.Model.Diagnosis.Condition = accident;

            this.CheckForRequiredAccidentFields();
        }

        public bool EvaluateNoMedicarePrimaryPayorForAutoAccidentRule( EventHandler eventHandler )
        {
            return RuleEngine.OneShotRuleEvaluation<NoMedicarePrimaryPayorForAutoAccident>( Model, eventHandler );
        }

        /// <summary>
        /// Evaluates the procedure required rule.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns></returns>
        public bool EvaluateProcedureRequiredRule(EventHandler eventHandler)
        {
            return RuleEngine.OneShotRuleEvaluation<ProcedureRequired>(Model, eventHandler);
        }

        /// <summary>
        /// Combo Box Hour Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbHour_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.SetAccidentCrimeHour();
        }

        /// <summary>
        /// Combo Box Country Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCountry_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.SetAccidentCrimeCountry( (Country)this.cmbCountry.SelectedItem );
        }

        /// <summary>
        /// Combo Box State Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbState_SelectedIndexChanged( object sender, EventArgs e )
        {
            //State state = ((DictionaryEntry)cmbState.SelectedItem).Value as State;
            if (this.cmbState.SelectedIndex >= 0)
            {
                State state = (State)this.cmbState.SelectedItem;
                if (state != null && ( this.rbAccident.Checked || this.rbCrime.Checked ))
                {
                    this.SetAccidentCrimeState( state );
                }
            }
        }

        private void DiagnosisView_Disposed( object sender, EventArgs e )
        {
            UnregisterEvents();
        }

        private void ChiefComplaintRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( this.mtbComplaint );
        }
        public void MakeModeOfArrivalRequired()
        {
            UIColors.SetRequiredBgColor(this.cmbModeOfArrival);
        }
        /// <summary>
        /// Procedures the required event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ProcedureRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.mtbProcedure);
        }

        private void AccidentTypeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( this.cmbAccidentType );
        }

        private void DateOfAccidentOrCrimeRequiredEventHandler( object sender, EventArgs e )
        {
            if (this.mtbAccidentCrimeDate.Enabled)
            {
                UIColors.SetRequiredBgColor( this.mtbAccidentCrimeDate );
            }
        }

        private void HourOfAccidentOrCrimeRequiredEventHandler( object sender, EventArgs e )
        {
            if (this.cmbHour.Enabled)
            {
                UIColors.SetRequiredBgColor( this.cmbHour );
            }
        }

        private void HourOfAccidentOrCrimePreferredEventHandler( object sender, EventArgs e )
        {
            if (this.cmbHour.Enabled)
            {
                UIColors.SetPreferredBgColor( this.cmbHour );
            }
        }

        private void CountryOfAccidentOrCrimeRequiredEventHandler( object sender, EventArgs e )
        {
            if (this.cmbCountry.Enabled)
            {
                UIColors.SetRequiredBgColor( this.cmbCountry );
            }
        }

        private void CountryOfAccidentOrCrimePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( this.cmbCountry );
        }

        private void StateOfAccidentOrCrimeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( this.cmbState );
        }

        private void StateOfAccidentOrCrimePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( this.cmbState );
        }

        private void OnsetDateOfSymptomsOrIllnessRequiredEventHandler( object sender, EventArgs e )
        {
            this.mtbOnsetDate.Enabled = true;
            UIColors.SetRequiredBgColor( this.mtbOnsetDate );
        }

        private void DiagnosisClinicOneRequiredEventHandler( object sender, EventArgs e )
        {
            this.clinicView1.cmbClinic.Enabled = true;
            UIColors.SetRequiredBgColor( this.clinicView1.cmbClinic );
        }

        private void AdmitSourceRequiredEventHandler( object sender, EventArgs e )
        {
            this.cmbAdmitSrc.Enabled = true;
            UIColors.SetRequiredBgColor( this.cmbAdmitSrc );
        }

        private void AdmitSourcePreferredEventHandler( object sender, EventArgs e )
        {
            this.cmbAdmitSrc.Enabled = true;
            UIColors.SetPreferredBgColor( this.cmbAdmitSrc );
        }

        private void AlternateCareFacilityRequiredEventHandler( object sender, EventArgs e )
        {
            cmbAlternateCareFacility.Enabled = true;
            UIColors.SetRequiredBgColor( cmbAlternateCareFacility );
        }

        private void TenetCareRequiredEventHandler( object sender, EventArgs e )
        {
            this.cmbTenetCare.Enabled = true;
            UIColors.SetRequiredBgColor( this.cmbTenetCare );
        }

        private void cmbReferralSrc_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.selectedReferralSrc = this.cmbReferralSrc.SelectedItem as ReferralSource;
            if (this.selectedReferralSrc != null)
            {
                this.Model.ReferralSource = this.selectedReferralSrc;
            }
        }

        //---------------------Evaluate ComboBoxes -----------------------------------
        private void cmbAdmitSrc_Validating( object sender, CancelEventArgs e )
        {
            HandleAdmitSourceSelectedIndexChanged(this.cmbAdmitSrc.SelectedItem as AdmitSource);
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.cmbAdmitSrc );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidAdmitSourceCode ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidAdmitSourceCodeChange ), this.Model );
            }
            RuleEngine.EvaluateRule( typeof( AdmitSourceRequired ), this.Model );
            RuleEngine.EvaluateRule( typeof( AdmitSourcePreferred ), this.Model );
            
        }
        private void cmbReferralSrc_Validating( object sender, CancelEventArgs e )
        {
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.cmbReferralSrc );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidReferalSourceCode ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidReferalSourceCodeChange ), this.Model );
            }
        }
        private void cmbModeOfArrival_Validating( object sender, CancelEventArgs e )
        {
           if (!this.blnLeaveRun)
            {
                this.Refresh();
                UIColors.SetNormalBgColor(this.cmbModeOfArrival);
                var selectedModeOfArrival = this.cmbModeOfArrival.SelectedItem as ModeOfArrival;
                if (selectedModeOfArrival != null)
                {
                    EdLogDisplayPresenter.UpdateSelectedModeOfArrival(selectedModeOfArrival);
                }
                RuleEngine.EvaluateRule( typeof( InvalidModeOfArrival ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidModeOfArrivalChange ), this.Model );
            }
        }
        private void cmbReferralType_Validating( object sender, CancelEventArgs e )
        {
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.cmbReferralType );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidReferralType ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidReferralTypeChange ), this.Model );
            }
        }
        private void cmbReferralFacility_Validating( object sender, CancelEventArgs e )
        {
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.cmbReferralFacility );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidReferralFacility ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidReferralFacilityChange ), this.Model );
            }
        }
        private void cmbReadmitCode_Validating( object sender, CancelEventArgs e )
        {
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.cmbReadmitCode );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidReAdmitCode ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidReAdmitCodeChange ), this.Model );
            }
        }
        private void clinicView1_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.clinicView1.cmbClinic );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_1Code ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_1CodeChange ), this.Model );
                RuleEngine.EvaluateRule( typeof( DiagnosisClinicOneRequired ), this.Model );
            }
        }
        private void clinicView2_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.clinicView2.cmbClinic );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_2Code ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_2CodeChange ), this.Model );
            }
        }
        private void clinicView3_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.clinicView3.cmbClinic );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_3Code ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_3CodeChange ), this.Model );
            }
        }
        private void clinicView4_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.clinicView4.cmbClinic );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_4Code ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_4CodeChange ), this.Model );
            }
        }
        private void clinicView5_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if (!this.blnLeaveRun)
            {
                UIColors.SetNormalBgColor( this.clinicView5.cmbClinic );
                this.Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_5Code ), this.Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_5CodeChange ), this.Model );
            }
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if (!comboBox.Focused)
            {
                comboBox.Focus();
            }
        }

        private void InvalidAdmitSourceCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.cmbAdmitSrc );
        }
        private void InvalidReferalSourceCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.cmbReferralSrc );
        }
        private void InvalidModeOfArrivalChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.cmbModeOfArrival );
        }
        private void InvalidReferralTypeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.cmbReferralType );
        }
        private void InvalidReferralFacilityChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.cmbReferralFacility );
        }
        private void InvalidReAdmitCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.cmbReadmitCode );
        }

        private void InvalidClinic1CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.clinicView1.cmbClinic );
        }
        private void InvalidClinic2CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.clinicView2.cmbClinic );
        }
        private void InvalidClinic3CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.clinicView3.cmbClinic );
        }
        private void InvalidClinic4CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.clinicView4.cmbClinic );
        }
        private void InvalidClinic5CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( this.clinicView5.cmbClinic );
        }

        //----------------------------------------------

        private void InvalidAdmitSourceCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.cmbAdmitSrc );
        }
        private void InvalidReferalSourceCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.cmbReferralSrc );
        }
        private void InvalidModeOfArrivalEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.cmbModeOfArrival );
        }
        private void InvalidReferralTypeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.cmbReferralType );
        }
        private void InvalidReferralFacilityEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.cmbReferralFacility );
        }
        private void InvalidReAdmitCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.cmbReadmitCode );
        }

        private void InvalidClinic2CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.clinicView2.cmbClinic );
        }
        private void InvalidClinic3CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.clinicView3.cmbClinic );
        }
        private void InvalidClinic4CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.clinicView4.cmbClinic );
        }
        private void InvalidClinic5CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.clinicView5.cmbClinic );
        }
        private void InvalidClinic1CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.clinicView1.cmbClinic );
        }
        /// <summary>
        /// Handles the Validating event of the mtb_Procedure control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void mtb_Procedure_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(this.mtbProcedure);
            this.DiagnosisViewPresenter.UpdateProcedureField(this.mtbProcedure.Text);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cmbAlternateCareFacility control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cmbAlternateCareFacility_SelectedIndexChanged( object sender, EventArgs e )
        {
            string selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if ( selectedAlternateCare != null )
            {
                AlternateCareFacilityPresenter.UpdateAlternateCareFacility( selectedAlternateCare );
            }
        }

        private void cmbAlternateCareFacility_Validating( object sender, CancelEventArgs e )
        {
            string selectedAlternateCare = cmbAlternateCareFacility.SelectedItem as string;
            if (selectedAlternateCare != null)
            {
                AlternateCareFacilityPresenter.UpdateAlternateCareFacility(selectedAlternateCare);
            }
            UIColors.SetNormalBgColor( cmbAlternateCareFacility );
            AlternateCareFacilityPresenter.EvaluateAlternateCareFacilityRule();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Disable Condition Dependencies
        /// </summary>
        private void DisableConditionDependencies()
        {
            //TODO: Disable all controls (and set default values) related to conditions and the chief complaint
            this.cmbAccidentType.Enabled = false;
            this.mtbAccidentCrimeDate.Enabled = false;
            this.dateTimePickerAccident.Enabled = false;
            this.cmbHour.Enabled = false;
            this.cmbCountry.Enabled = false;
            this.cmbState.Enabled = false;
            UIColors.SetDisabledDarkBgColor( this.cmbAccidentType );
            UIColors.SetDisabledDarkBgColor( this.mtbAccidentCrimeDate );
            UIColors.SetDisabledDarkBgColor( this.cmbHour );
            UIColors.SetDisabledDarkBgColor( this.cmbCountry );
            UIColors.SetDisabledDarkBgColor( this.cmbState );
        }

        /// <summary>
        /// Display Condition
        /// </summary>
        private void DisplayCondition()
        {
            this.DisableConditionDependencies();
            DisplayAbstractCondition displayMethodPointer = this.DisplayMethodForCurrentCondition();
            displayMethodPointer();
        }

        /// <summary>
        /// Retrieves the correct delegate to use based on the current Model's Type.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Cannot retrieve the delegate for displaying a Condition when Diagnosis is null</exception>
        public DisplayAbstractCondition DisplayMethodForCurrentCondition()
        {
            if (this.Model.Diagnosis == null || this.Model.Diagnosis.Condition == null)
            {
                throw new InvalidOperationException( "Cannot retrieve the delegate for displaying a Condition when Diagnosis is null" );
            }
            return (DisplayAbstractCondition)ConditionMap[this.Model.Diagnosis.Condition.GetType()];
        }

        /// <summary>
        /// Re-displays the view when the Model is changed.
        /// </summary>
        public override void UpdateView()
        {
            EdLogDisplayPresenter = new EDLogsDisplayPresenter(this, Model, Rules.RuleEngine.GetInstance());
            DiagnosisViewPresenter = new DiagnosisViewPresenter(this, Model.Activity);
            AlternateCareFacilityPresenter = new AlternateCareFacilityPresenter( this, new AlternateCareFacilityFeatureManager() );
            

            CptCodesPresenter = new CptCodesPresenter( cptCodesView, Model, new CptCodesFeatureManager(), new MessageBoxAdapter() );

            try
            {
                if (this.Model.Activity.AssociatedActivityType == typeof (ActivatePreRegistrationActivity)
                    && this.Model.KindOfVisit != null
                    && this.Model.KindOfVisit.Code == VisitType.PREREG_PATIENT)
                {
                    this.Model.KindOfVisit = (VisitType) this.PatientTypes[0];
                }

                this.Cursor = Cursors.AppStarting;
                this.StateComboHelper = new ReferenceValueComboBox(this.cmbState);

                this.facility = User.GetCurrent().Facility;

                this.clinicView1.cmbClinic.Name = "cmbClinic1";
                this.clinicView2.cmbClinic.Name = "cmbClinic2";
                this.clinicView3.cmbClinic.Name = "cmbClinic3";
                this.clinicView4.cmbClinic.Name = "cmbClinic4";
                this.clinicView5.cmbClinic.Name = "cmbClinic5";

                this.PopulateTenetCare();

                this.LoadClinicCombos();

                this.PopulateAdmitSource();
                this.PopulateReferralSource();

                if (this.clinicView1.cmbClinic.Items.Count < 1)
                {
                    this.PopulateClinics();
                    //Load Service Category Dropdown 
                    ServiceCategoryPresenter.UpdateView();
                }

                this.EdLogDisplayPresenter.PopulateEdLogData();
                bool edLogFieldsHaveData = this.HasData(); 
                EdLogDisplayPresenter.UpdateEDLogDisplay(Model.KindOfVisit, true, edLogFieldsHaveData);
                DiagnosisViewPresenter.HandleProcedureField( Model.KindOfVisit, true );

                //TODO: Populate previous selected value for TenetCare from model.
                //this.Model.TENETCARE will be added to the account
                this.IsTenetCareFacility();
                this.patientTypeHSVLocationView.Model = this.Model;
                this.patientTypeHSVLocationView.UpdateView();

                CptCodesPresenter.UpdateView();

                if (this.loadingModelData)
                {
                    RegisterEvents();
                }

                this.LocalUpdateView();
                if (loadingModelData)
                {
                    loadingModelData = false;
                }
              
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Puts data into the Model from the controls on the view.
        /// </summary>
        public override void UpdateModel()
        {
        }

        #endregion

        #region Properties

        public string NursingStationCode
        {
            get
            {
                return this.i_NursingStationCode;
            }
            set
            {
                this.i_NursingStationCode = value;
            }
        }

        public Hashtable ConditionMap
        {
            get
            {
                return this.i_ConditionMap;
            }
        }

        internal IEDLogsDisplayPresenter EdLogDisplayPresenter
        {
            private get
            {
                return this.edLogDisplayPresenter;
            }

            set
            {
                this.edLogDisplayPresenter = value;
            }
        }
        /// <summary>
        /// Gets or sets the diagnosis view presenter.
        /// </summary>
        /// <value>The diagnosis view presenter.</value>
        private IDiagnosisViewPresenter DiagnosisViewPresenter
        {
            get
            {
                return this.diagnosisViewPresenter;
            }

            set
            {
                this.diagnosisViewPresenter = value;
            }
        }

        
        private CptCodesPresenter CptCodesPresenter { get; set; }

        private IAlternateCareFacilityPresenter AlternateCareFacilityPresenter
        {
            get
            {
                return alternateCareFacilityPresenter;
            }

            set
            {
                alternateCareFacilityPresenter = value;
            }
        }
        public new Account Model
        {
            get
            {
                return (Account)base.Model;
            }
            set
            {
                base.Model = value;
            }
        }
        public ServiceCategoryPresenter ServiceCategoryPresenter
        {
            get
            {
                ServiceCategoryPresenter serviceCategoryPresenter = this.serviceCategory1.ServiceCategoryPresenter;
                if (serviceCategoryPresenter == null)
                {
                    var serviceCategoryBroker = BrokerFactory.BrokerOfType<IServiceCategoryBroker>();

                    serviceCategoryPresenter = new ServiceCategoryPresenter(this.serviceCategory1, Model, serviceCategoryBroker, DOFRFeatureManager.GetInstance());
                }
                return serviceCategoryPresenter;
            }
        }
        #endregion

        #region Private Methods



        private void SetTabStopForInpatient(object sender, VisitTypeEventArgs e)
        {
            if (Model.KindOfVisit.Code == VisitType.INPATIENT)
            {
                gbxClinics.TabStop = false;
                gbxEDLog.TabStop = false;
                clinicView1.TabStop = false;
                clinicView2.TabStop = false;
                clinicView3.TabStop = false;
                clinicView4.TabStop = false;
                clinicView5.TabStop = false;
                mtbArrivalTime.TabStop = false;
                cmbModeOfArrival.TabStop = false;
                cmbReferralType.TabStop = false;
                cmbReferralFacility.TabStop = false;
                cmbReadmitCode.TabStop = false;
                lblArrivalTime.TabStop = false;
                lblModeOfArrival.TabStop = false;
                lblReferralType.TabStop = false;
                lblReferralActivity.TabStop = false;
                lblReAdmitCode.TabStop = false;
            }
            else
            {
                gbxClinics.TabStop = true;
                gbxEDLog.TabStop = true;
                clinicView1.TabStop = true;
                clinicView2.TabStop = true;
                clinicView3.TabStop = true;
                clinicView4.TabStop = true;
                clinicView5.TabStop = true;
                mtbArrivalTime.TabStop = true;
                cmbModeOfArrival.TabStop = true;
                cmbReferralType.TabStop = true;
                cmbReferralFacility.TabStop = true;
                cmbReadmitCode.TabStop = true;
            }
        }

       

        private void CheckPreviousCondition()
        {
            if (this.Model.Diagnosis.Condition.GetType() == typeof( Accident ))
            {
                this.cmbAccidentType.SelectedIndex = -1;
            }
        }

        private void PopulateTenetCare()
        {
            if (this.cmbTenetCare.Items.Count > 0)
            {
                return;
            }

            YesNoFlag yesNoFlag = new YesNoFlag( String.Empty );
            this.cmbTenetCare.Items.Add( yesNoFlag );
            yesNoFlag = new YesNoFlag( "Y" );
            this.cmbTenetCare.Items.Add( yesNoFlag );
            yesNoFlag = new YesNoFlag( "N" );
            this.cmbTenetCare.Items.Add( yesNoFlag );

            if (this.Model.TenetCare != null)
            {
                this.cmbTenetCare.SelectedItem = this.Model.TenetCare;
            }
        }

        public void SetArrivalTime(string displayedTimeFormat)
        {
            mtbArrivalTime.Text = displayedTimeFormat;
        }

        public void ClearArrivalTimeMask()
        {
            mtbArrivalTime.UnMaskedText = string.Empty;
        }

        public void ShowEnabledForUCCPostMse()
        {
            EnableEdLogViewForUCCPostMse();
        }

        public void DoNotShow()
        {
            this.gbxEDLog.Visible = false;
        }

        public void ShowDisabled()
        {
            this.gbxEDLog.Visible = true;
            this.EnableEdLogView( false );
        }

        public void ShowEnabled()
        {
            this.gbxEDLog.Visible = true;
            this.EnableEdLogView( true ); 
        }

        public bool HasData()
        {
            return EdLogFieldHasArrivalTimeData() || EdLogFieldHasModeOfArrivalData() ||
                   EdLogFieldHasReferralTypeData() || EdLogFieldHasReferralFacilityData() ||
                   EdLogFieldHasReAdmitCodeData();
        }

        private bool EdLogFieldHasReAdmitCodeData()
        {
            return ( this.Model.ReAdmitCode != null && !String.IsNullOrEmpty( this.Model.ReAdmitCode.Code ) );
        }

        private bool EdLogFieldHasReferralFacilityData()
        {
            return ( this.Model.ReferralFacility != null && !String.IsNullOrEmpty( this.Model.ReferralFacility.Code ) );
        }

        private bool EdLogFieldHasReferralTypeData()
        {
            return ( this.Model.ReferralType != null && !String.IsNullOrEmpty( this.Model.ReferralType.Code ) );
        }

        private bool EdLogFieldHasModeOfArrivalData()
        {
            return ( this.Model.ModeOfArrival != null && !String.IsNullOrEmpty( this.Model.ModeOfArrival.Code ) );
        }

        private bool EdLogFieldHasArrivalTimeData()
        {
            return this.Model.ArrivalTime != DateTime.MinValue;
        }

        private void EnableEdLogView( bool isEnabled )
        {
            this.mtbArrivalTime.Enabled = isEnabled;
            this.cmbModeOfArrival.Enabled = isEnabled;
            this.cmbReferralType.Enabled = isEnabled;
            this.cmbReferralFacility.Enabled = isEnabled;
            this.cmbReadmitCode.Enabled = isEnabled;
        }
        private void EnableEdLogViewForUCCPostMse()
        {
            gbxEDLog.Visible = true;
            mtbArrivalTime.Enabled = true;
            cmbModeOfArrival.Enabled = true;
            cmbReferralType.Enabled = false;
            cmbReferralFacility.Enabled = false;
            cmbReadmitCode.Enabled = false;
        }
        public void ClearFields()
        {
            this.mtbArrivalTime.UnMaskedText = string.Empty;
            ClearIfNoItems( cmbModeOfArrival );
            ClearIfNoItems( cmbReadmitCode );
            ClearIfNoItems( cmbReferralFacility );
            ClearIfNoItems( cmbReferralType );
        }

        //TODO-AC Candidate for refactoring to a common location
        private static void ClearIfNoItems( ComboBox comboBox )
        {
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Clears the procedure field on UI.
        /// </summary>
        public void ClearProcedureField()
        {
            this.mtbProcedure.Text = String.Empty;
        }
        
        public void ShowProcedureDisabled()
        {
            UIColors.SetDisabledDarkBgColor(this.mtbProcedure);
            this.mtbProcedure.Enabled = false;
        }
            /// <summary>
            /// Shows the procedure enabled on UI.
            /// </summary>
        public void ShowProcedureEnabled()
        {
            UIColors.SetNormalBgColor(this.mtbProcedure);
            this.mtbProcedure.Enabled = true;
            this.RuleEngine.EvaluateRule(typeof(ProcedureRequired), this.Model);
        }
        /// <summary>
        /// Determines whether [has procedure data].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has procedure data]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasProcedureData()
        {
            return (this.Model.Diagnosis.Procedure != null && !String.IsNullOrEmpty(this.Model.Diagnosis.Procedure));
        }

        public void SetModeOfArrival(ModeOfArrival modeOfArrival)
        {
            cmbModeOfArrival.SelectedItem = modeOfArrival;
        }

        public void PopulateArrivalModes(ArrayList arrivalModes)
        {
            cmbModeOfArrival.Items.Clear();
            if (arrivalModes.Count < 1)
            {
                cmbModeOfArrival.Items.Add( new ModeOfArrival() );
            }

            foreach (ModeOfArrival arrivalMode in arrivalModes)
            {
                cmbModeOfArrival.Items.Add( arrivalMode );
            }
        }

        public void SetReferralType(ReferralType referralType)
        {
            cmbReferralType.SelectedItem = referralType;
        }

        public void PopulateReferralTypes(ArrayList referralTypes)
        {
            cmbReferralType.Items.Clear();

            foreach (ReferralType referralType1 in referralTypes)
            {
                cmbReferralType.Items.Add( referralType1 );
            }
        }

        public void SetReferralFacility(ReferralFacility referralFacility)
        {
            cmbReferralFacility.SelectedItem = referralFacility;
        }

        public void PopulateReferralFacilities(ICollection referralFacilities)
        {
            cmbReferralFacility.Items.Clear();

            foreach (ReferralFacility referralFacility in referralFacilities)
            {
                cmbReferralFacility.Items.Add( referralFacility );
            }
        }

        public void SetReadmitCode(ReAdmitCode admitCode)
        {
            cmbReadmitCode.SelectedItem = admitCode;
        }

        public void PopulateReadmitCode(ICollection readmitCodes)
        {
            cmbReadmitCode.Items.Clear();

            foreach (ReAdmitCode readmitCode in readmitCodes)
            {
                cmbReadmitCode.Items.Add( readmitCode );
            }
        }

        private void IsValidateArrivalTime()
        {
            if (!DateValidator.IsValidTime( this.mtbArrivalTime ))
            {
                this.mtbArrivalTime.Focus();
                UIColors.SetErrorBgColor( this.mtbArrivalTime );
            }
            else
            {

                UIColors.SetNormalBgColor( this.mtbArrivalTime );

                int admitHour = 0;
                int admitMinute = 0;

                if (this.mtbArrivalTime.Text.Length == 5)
                {
                    admitHour = Convert.ToInt32( this.mtbArrivalTime.Text.Substring( 0, 2 ) );
                    admitMinute = Convert.ToInt32( this.mtbArrivalTime.Text.Substring( 3, 2 ) );
                }

                try
                {   // 
                    DateTime theDate = DateTime.MinValue;

                    if (admitHour != 0 || admitMinute != 0)
                    {
                        theDate = new DateTime( this.Model.ArrivalTime.Year,
                            this.Model.ArrivalTime.Month,
                            this.Model.ArrivalTime.Day,
                            admitHour,
                            admitMinute, 0 );
                    }

                    if (DateValidator.IsValidDate( theDate ) == false)
                    {
                        UIColors.SetErrorBgColor( this.mtbArrivalTime );
                        MessageBox.Show( UIErrorMessages.TIME_NOT_VALID_MSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        this.mtbArrivalTime.Focus();
                    }
                    else
                    {
                        this.Model.ArrivalTime = theDate;
                    }
                }
                catch
                {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                    // an invalid year, month, or day.  Simply set field to error color.
                    UIColors.SetErrorBgColor( this.mtbArrivalTime );
                    MessageBox.Show( UIErrorMessages.ADMIT_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    this.mtbArrivalTime.Focus();
                }
            }
        }

        private void RegisterEvents()
        {
            if (this.i_Registered)
            {
                return;
            }

            this.i_Registered = true;

            RuleEngine.RegisterEvent( typeof( ChiefComplaintRequired ), new EventHandler( this.ChiefComplaintRequiredEventHandler ) );
            RuleEngine.RegisterEvent(typeof(ProcedureRequired), new EventHandler(this.ProcedureRequiredEventHandler));
            RuleEngine.RegisterEvent( typeof( AccidentTypeRequired ), new EventHandler( this.AccidentTypeRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( DateOfAccidentOrCrimeRequired ), new EventHandler( this.DateOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( HourOfAccidentOrCrimeRequired ), new EventHandler( this.HourOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( HourOfAccidentOrCrimePreferred ), new EventHandler( this.HourOfAccidentOrCrimePreferredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( CountryOfAccidentOrCrimeRequired ), new EventHandler( this.CountryOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( CountryOfAccidentOrCrimePreferred ), new EventHandler( this.CountryOfAccidentOrCrimePreferredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( StateOfAccidentOrCrimeRequired ), new EventHandler( this.StateOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( StateOfAccidentOrCrimePreferred ), new EventHandler( this.StateOfAccidentOrCrimePreferredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( OnsetDateOfSymptomsOrIllnessRequired ), new EventHandler( this.OnsetDateOfSymptomsOrIllnessRequiredEventHandler ) );

            RuleEngine.RegisterEvent( typeof( DiagnosisClinicOneRequired ), new EventHandler( this.DiagnosisClinicOneRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( AdmitSourceRequired ), new EventHandler( this.AdmitSourceRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( AdmitSourcePreferred ), new EventHandler( this.AdmitSourcePreferredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( TenetCareRequired ), new EventHandler( this.TenetCareRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( AlternateCareFacilityRequired ), new EventHandler(this.AlternateCareFacilityRequiredEventHandler));
            RuleEngine.RegisterEvent( typeof( InvalidAdmitSourceCode ), new EventHandler( this.InvalidAdmitSourceCodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidReferalSourceCode ), new EventHandler( this.InvalidReferalSourceCodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidModeOfArrival ), new EventHandler( this.InvalidModeOfArrivalEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidReferralType ), new EventHandler( this.InvalidReferralTypeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidReferralFacility ), new EventHandler( this.InvalidReferralFacilityEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidReAdmitCode ), new EventHandler( this.InvalidReAdmitCodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_1Code ), new EventHandler( InvalidClinic1CodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_2Code ), new EventHandler( InvalidClinic2CodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_3Code ), new EventHandler( InvalidClinic3CodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_4Code ), new EventHandler( InvalidClinic4CodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_5Code ), new EventHandler( InvalidClinic5CodeEventHandler ) );

            RuleEngine.RegisterEvent( typeof( InvalidAdmitSourceCodeChange ), new EventHandler( this.InvalidAdmitSourceCodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidReferalSourceCodeChange ), new EventHandler( this.InvalidReferalSourceCodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidModeOfArrivalChange ), new EventHandler( this.InvalidModeOfArrivalChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidReferralTypeChange ), new EventHandler( this.InvalidReferralTypeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidReferralFacilityChange ), new EventHandler( this.InvalidReferralFacilityChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidReAdmitCodeChange ), new EventHandler( this.InvalidReAdmitCodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_1CodeChange ), new EventHandler( InvalidClinic1CodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_2CodeChange ), new EventHandler( InvalidClinic2CodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_3CodeChange ), new EventHandler( InvalidClinic3CodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_4CodeChange ), new EventHandler( InvalidClinic4CodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_5CodeChange ), new EventHandler( InvalidClinic5CodeChangeEventHandler ) );
            edLogDisplayPresenter.RegisterEDLogRules();
        }

        private void UnregisterEvents()
        {
            this.i_Registered = false;

            RuleEngine.UnregisterEvent( typeof( ChiefComplaintRequired ), new EventHandler( this.ChiefComplaintRequiredEventHandler ) );
            RuleEngine.UnregisterEvent(typeof(ProcedureRequired), new EventHandler(this.ProcedureRequiredEventHandler));
            RuleEngine.UnregisterEvent( typeof( AccidentTypeRequired ), new EventHandler( this.AccidentTypeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( DateOfAccidentOrCrimeRequired ), new EventHandler( this.DateOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( HourOfAccidentOrCrimeRequired ), new EventHandler( this.HourOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( HourOfAccidentOrCrimePreferred ), new EventHandler( this.HourOfAccidentOrCrimePreferredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( CountryOfAccidentOrCrimeRequired ), new EventHandler( this.CountryOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( CountryOfAccidentOrCrimePreferred ), new EventHandler( this.CountryOfAccidentOrCrimePreferredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( StateOfAccidentOrCrimeRequired ), new EventHandler( this.StateOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( StateOfAccidentOrCrimePreferred ), new EventHandler( this.StateOfAccidentOrCrimePreferredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( OnsetDateOfSymptomsOrIllnessRequired ), new EventHandler( this.OnsetDateOfSymptomsOrIllnessRequiredEventHandler ) );

            RuleEngine.UnregisterEvent( typeof( DiagnosisClinicOneRequired ), new EventHandler( this.DiagnosisClinicOneRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( AdmitSourceRequired ), new EventHandler( this.AdmitSourceRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( AdmitSourcePreferred ), new EventHandler( this.AdmitSourcePreferredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( TenetCareRequired ), new EventHandler( this.TenetCareRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( AlternateCareFacilityRequired ), new EventHandler( this.AlternateCareFacilityRequiredEventHandler ));
            RuleEngine.UnregisterEvent( typeof( InvalidAdmitSourceCode ), new EventHandler( this.InvalidAdmitSourceCodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidReferalSourceCode ), new EventHandler( this.InvalidReferalSourceCodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidModeOfArrival ), new EventHandler( this.InvalidModeOfArrivalEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidReferralType ), new EventHandler( this.InvalidReferralTypeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidReferralFacility ), new EventHandler( this.InvalidReferralFacilityEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidReAdmitCode ), new EventHandler( this.InvalidReAdmitCodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_1Code ), new EventHandler( InvalidClinic1CodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_2Code ), new EventHandler( InvalidClinic2CodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_3Code ), new EventHandler( InvalidClinic3CodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_4Code ), new EventHandler( InvalidClinic4CodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_5Code ), new EventHandler( InvalidClinic5CodeEventHandler ) );

            RuleEngine.UnregisterEvent( typeof( InvalidAdmitSourceCodeChange ), new EventHandler( this.InvalidAdmitSourceCodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidReferalSourceCodeChange ), new EventHandler( this.InvalidReferalSourceCodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidModeOfArrivalChange ), new EventHandler( this.InvalidModeOfArrivalChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidReferralTypeChange ), new EventHandler( this.InvalidReferralTypeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidReferralFacilityChange ), new EventHandler( this.InvalidReferralFacilityChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidReAdmitCodeChange ), new EventHandler( this.InvalidReAdmitCodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_1CodeChange ), new EventHandler( InvalidClinic1CodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_2CodeChange ), new EventHandler( InvalidClinic2CodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_3CodeChange ), new EventHandler( InvalidClinic3CodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_4CodeChange ), new EventHandler( InvalidClinic4CodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_5CodeChange ), new EventHandler( InvalidClinic5CodeChangeEventHandler ) );
            edLogDisplayPresenter.UnRegisterEDLogRules();
           }

        private void IsValidAccidentTypeForInsurance()
        {
            Coverage primaryCoverage = this.Model.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
            if (primaryCoverage != null && primaryCoverage.InsurancePlan != null)
            {
                WorkersCompensationInsurancePlan workcomp = new WorkersCompensationInsurancePlan();
                if (primaryCoverage.InsurancePlan.GetType() != workcomp.GetType())
                {
                    MessageBox.Show( UIErrorMessages.DIAGNOSIS_ACCIDENTTYPE_INS_CHK, "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
            }
        }

        private void HandleClinicIndexChange( ComboBox cmbClinic )
        {
            this.RemoveClinicHandler();
            this.ComboValueSet( cmbClinic );
            this.AddClinicHandler();

            RuleEngine.RegisterEvent( typeof( DiagnosisClinicOneRequired ), new EventHandler( this.DiagnosisClinicOneRequiredEventHandler ) );
        }

        private void ComboValueSet(ComboBox combo)
        {
            if (combo != null)
            {
                this.ResetAllSelection();
            }
        }

        private void ResetAllSelection()
        {
            Hashtable tempHolder = new Hashtable();

            foreach (ComboBox combo in this.combos.Values)
            {
                if (combo.SelectedIndex > 0)
                {
                    tempHolder.Add( combo.Name, combo.SelectedItem );
                }
            }

            this.LoadCombosData( this.currentCombo );

            foreach (object key in tempHolder.Keys)
            {
                ComboBox combo = ( (ComboBox)combos[key] );
                Object comboValue = tempHolder[key];

                ReplaceItem( combo as PatientAccessComboBox, comboValue );

                this.RemoveValueFromCombos( tempHolder[key], (ComboBox)combos[key] );
            }
        }

        private static void ReplaceItem( PatientAccessComboBox combo, object comboValue )
        {
            if (combo != null)
            {
                combo.SelectedItem = comboValue as HospitalClinic;
            }

        }

        private void LoadClinicCombos()
        {
            this.AddClinicComboBox( this.clinicView1.cmbClinic );
            this.AddClinicComboBox( this.clinicView2.cmbClinic );
            this.AddClinicComboBox( this.clinicView3.cmbClinic );
            this.AddClinicComboBox( this.clinicView4.cmbClinic );
            this.AddClinicComboBox( this.clinicView5.cmbClinic );
        }

        private void LoadCombosData( ComboBox comboBox )
        {
            foreach (ComboBox combo in this.combos.Values)
            {
                if (combo.Name != comboBox.Name)
                {
                    combo.Items.Clear();

                    foreach (object row in this.i_AllClinics)
                    {
                        combo.Items.Add( row );
                    }
                    if (combo.Items.Count > 0)
                    {
                        combo.SelectedIndex = 0;
                    }
                    combo.Refresh();
                }
            }
        }

        private void RemoveValueFromCombos( object comboValue, ComboBox excludeCombo )
        {
            foreach (ComboBox combo in this.combos.Values)
            {
                if (excludeCombo != combo)
                {
                    int i = combo.FindString( comboValue.ToString() );
                    if (i > 0)
                    {
                        combo.Items.RemoveAt( i );
                    }
                }
            }
        }

        private void AddClinicComboBox( ComboBox comboBox )
        {
            if (!this.combos.ContainsKey( comboBox.Name ))
            {
                this.combos.Add( comboBox.Name, comboBox );
            }
        }

        private void RemoveClinicHandler()
        {
            clinicView1.cmbClinic.SelectedIndexChanged -= clinicView1.cmbClinic_SelectedIndexChanged;
            clinicView2.cmbClinic.SelectedIndexChanged -= clinicView2.cmbClinic_SelectedIndexChanged;
            clinicView3.cmbClinic.SelectedIndexChanged -= clinicView3.cmbClinic_SelectedIndexChanged;
            clinicView4.cmbClinic.SelectedIndexChanged -= clinicView4.cmbClinic_SelectedIndexChanged;
            clinicView5.cmbClinic.SelectedIndexChanged -= clinicView5.cmbClinic_SelectedIndexChanged;
        }

        private void AddClinicHandler()
        {
            clinicView1.cmbClinic.SelectedIndexChanged += clinicView1.cmbClinic_SelectedIndexChanged;
            clinicView2.cmbClinic.SelectedIndexChanged += clinicView2.cmbClinic_SelectedIndexChanged;
            clinicView3.cmbClinic.SelectedIndexChanged += clinicView3.cmbClinic_SelectedIndexChanged;
            clinicView4.cmbClinic.SelectedIndexChanged += clinicView4.cmbClinic_SelectedIndexChanged;
            clinicView5.cmbClinic.SelectedIndexChanged += clinicView5.cmbClinic_SelectedIndexChanged;
        }

        private void ClearOccurrenceCodes()
        {
            int upBound = this.Model.OccurrenceCodes.Count - 1;
            for (int i = upBound; i > -1; i--)
            {
                OccurrenceCode occ = ( (OccurrenceCode)this.Model.OccurrenceCodes[i] );
                if (occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO_NO_FAULT ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_EMPLOYER_REL ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_OTHER ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_TORT_LIABILITY ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_CRIME)
                {
                    this.Model.RemoveOccurrenceCode( occ );
                }
            }

            var occurrenceCodeComparerByCodeAndDate = new OccurrenceCodeComparerByCodeAndDate();
            ( (ArrayList)Model.OccurrenceCodes ).Sort( occurrenceCodeComparerByCodeAndDate );
        }

        private void RunInvalidValuesRules()
        {
            UIColors.SetNormalBgColor( this.cmbAdmitSrc );
            UIColors.SetNormalBgColor( this.cmbReferralSrc );
            UIColors.SetNormalBgColor( this.cmbModeOfArrival );
            UIColors.SetNormalBgColor( this.cmbReferralType );
            UIColors.SetNormalBgColor( this.cmbReferralFacility );
            UIColors.SetNormalBgColor( this.cmbReadmitCode );
            UIColors.SetNormalBgColor( this.clinicView1.cmbClinic );
            UIColors.SetNormalBgColor( this.clinicView2.cmbClinic );
            UIColors.SetNormalBgColor( this.clinicView3.cmbClinic );
            UIColors.SetNormalBgColor( this.clinicView4.cmbClinic );
            UIColors.SetNormalBgColor( this.clinicView5.cmbClinic );

            RuleEngine.EvaluateRule( typeof( AdmitSourcePreferred ), this.Model );
            RuleEngine.EvaluateRule( typeof( OnDiagnosisForm ), this.Model );
        }

        private void CheckForRequiredAccidentFields()
        {
            // reset all fields that might have error, preferred, or required backgrounds          

            UIColors.SetNormalBgColor( this.cmbAccidentType );
            UIColors.SetNormalBgColor( this.mtbAccidentCrimeDate );
            UIColors.SetNormalBgColor( this.cmbHour );
            UIColors.SetNormalBgColor( this.cmbCountry );
            UIColors.SetNormalBgColor( this.cmbState );
            UIColors.SetNormalBgColor( this.mtbOnsetDate );
            UIColors.SetNormalBgColor( this.dateTimePickerSickness );

            RuleEngine.EvaluateRule( typeof( AccidentTypeRequired ), this.Model );
            RuleEngine.EvaluateRule( typeof( DateOfAccidentOrCrimeRequired ), this.Model );
            RuleEngine.EvaluateRule( typeof( HourOfAccidentOrCrimeRequired ), this.Model );
            RuleEngine.EvaluateRule( typeof( HourOfAccidentOrCrimePreferred ), this.Model );
            RuleEngine.EvaluateRule( typeof( CountryOfAccidentOrCrimeRequired ), this.Model );
            RuleEngine.EvaluateRule( typeof( CountryOfAccidentOrCrimePreferred ), this.Model );
            RuleEngine.EvaluateRule( typeof( StateOfAccidentOrCrimeRequired ), this.Model );
            RuleEngine.EvaluateRule( typeof( StateOfAccidentOrCrimePreferred ), this.Model );
            RuleEngine.EvaluateRule( typeof( OnsetDateOfSymptomsOrIllnessRequired ), this.Model ); 
        }

        private void EnableDisableClinics( bool isEnabled )
        {
            UIColors.SetNormalBgColor( this.clinicView1.cmbClinic );
            this.clinicView1.cmbClinic.Enabled = isEnabled;
            this.clinicView2.cmbClinic.Enabled = isEnabled;
            this.clinicView3.cmbClinic.Enabled = isEnabled;
            this.clinicView4.cmbClinic.Enabled = isEnabled;
            this.clinicView5.cmbClinic.Enabled = isEnabled;
            ServiceCategoryPresenter.EnableOrDisableServiceCategory(isEnabled);
        }

        private void ClearSelectedClinics( VisitType visitType )
        {
            if (visitType != null && visitType.Code != null
                && visitType.Code != VisitType.INPATIENT)
            {
                  this.EnableDisableClinics(true);
                  return;
            }

            Clinic newClinic = new Clinic();
            this.clinicView1.cmbClinic.SelectedItem = newClinic;
            this.clinicView2.cmbClinic.SelectedItem = newClinic;
            this.clinicView3.cmbClinic.SelectedItem = newClinic;
            this.clinicView4.cmbClinic.SelectedItem = newClinic;
            this.clinicView5.cmbClinic.SelectedItem = newClinic;
            ServiceCategoryPresenter.ClearServiceCategory();
            this.EnableDisableClinics( false );
        }

        private void LocalUpdateView()
        {
            if (this.Model.Activity == null)
            {   // If the AccountView was invoked from a Worklist
                //  Action, there will be no activity
                return;
            }

            foreach (OccurrenceCode occ in this.Model.OccurrenceCodes)
            {
                if (occ.Code == "11")
                {
                    if (occ.OccurrenceDate != DateTime.MinValue)
                    {
                        this.mtbOnsetDate.UnMaskedText = occ.OccurrenceDate.ToString( "MMddyyyy" );
                    }
                    break;
                }
            }

            if (this.cmbCountry.Items.Count == 0)
            {
                this.DisplayCountries();
            }

            if (this.cmbState.Items.Count == 0)
            {
                this.DisplayStates();
            }

            this.DisplayChiefComplaint();
            this.DisplayAccidentTypes();
            this.DisplayCondition();


            if (this.Model.Activity.GetType().Equals( typeof( PreRegistrationActivity ) ) ||
                this.Model.Activity.GetType().Equals( typeof( PostMSERegistrationActivity ) ) ||
                ( this.Model.Activity.GetType().Equals( typeof( MaintenanceActivity ) ) &&
                 this.Model.KindOfVisit != null && this.Model.KindOfVisit.Code == VisitType.PREREG_PATIENT
                  && (Model.Activity.AssociatedActivityType==null || Model.Activity.AssociatedActivityType!=typeof(PreAdmitNewbornActivity)) ))
            {
                int newbornIndex = this.cmbAdmitSrc.FindString( "L NEWBORN" );
                if (newbornIndex != -1)
                {
                    this.cmbAdmitSrc.Items.RemoveAt( newbornIndex );
                }
            }

            this.RunInvalidValuesRules();
        }


        private void PopulateClinics()
        {
            this.clinicView1.LabelText = "Clinic 1:";
            this.clinicView1.PatientFacility = this.facility;
            this.clinicView1.UpdateView();
            this.i_AllClinics = this.clinicView1.AllHospitalClinics();

            this.clinicView2.LabelText = "Clinic 2:";
            this.clinicView2.ListOfClinics = this.i_AllClinics;
            this.clinicView2.UpdateView();

            this.clinicView3.LabelText = "Clinic 3:";
            this.clinicView3.ListOfClinics = this.i_AllClinics;
            this.clinicView3.UpdateView();

            this.clinicView4.LabelText = "Clinic 4:";
            this.clinicView4.ListOfClinics = this.i_AllClinics;
            this.clinicView4.UpdateView();

            this.clinicView5.LabelText = "Clinic 5:";
            this.clinicView5.ListOfClinics = this.i_AllClinics;
            this.clinicView5.UpdateView();

            if (this.SetDefaultClinic())
            {
                this.SelectPreviouslyStoredClinicValue();
            }
        }

        private void SelectPreviouslyStoredClinicValue()
        {
            this.clinicView1.SetClinicValueFromModel( this.Model.Clinics[0] as HospitalClinic );
            this.clinicView2.SetClinicValueFromModel( this.Model.Clinics[1] as HospitalClinic );
            this.clinicView3.SetClinicValueFromModel( this.Model.Clinics[2] as HospitalClinic );
            this.clinicView4.SetClinicValueFromModel( this.Model.Clinics[3] as HospitalClinic );
            this.clinicView5.SetClinicValueFromModel( this.Model.Clinics[4] as HospitalClinic );
            ServiceCategoryPresenter.SetSelectedServiceCategory(string.IsNullOrEmpty(this.Model.EmbosserCard) ? string.Empty : this.Model.EmbosserCard);
        }

        private bool SetDefaultClinic()
        {
            bool setDefaultClinicValue = false;

            switch (this.Model.Activity.GetType().Name)
            {
                case "PreRegistrationActivity":
                case "AdmitNewbornActivity":
                    break;
                case "PreAdmitNewbornActivity":
                    break;
                case "PostMSERegistrationActivity":
                    setDefaultClinicValue = true;
                    break;
                case "UCCPostMseRegistrationActivity":
                    setDefaultClinicValue = true;
                    break;
                case "RegistrationActivity":
                    break;
                case "MaintenanceActivity":
                    if (this.Model.KindOfVisit != null
                        && this.Model.KindOfVisit.Code != VisitType.INPATIENT
                        && (Model.Activity.AssociatedActivityType== null ||Model.Activity.AssociatedActivityType!=typeof(PreAdmitNewbornActivity)))
                    {
                        setDefaultClinicValue = true;
                    }
                    break;
                default:
                    break;
            }

            return setDefaultClinicValue;
        }

        private void PopulateAdmitSource()
        {
            AdmitSourceBrokerProxy brokerProxy = new AdmitSourceBrokerProxy();
            ArrayList allSources = (ArrayList)brokerProxy.AllTypesOfAdmitSources( User.GetCurrent().Facility.Oid );

            this.cmbAdmitSrc.Items.Clear();

            for (int i = 0; i < allSources.Count; i++)
            {
                AdmitSource source = (AdmitSource)allSources[i];
                this.cmbAdmitSrc.Items.Add( source );
            }

            if (this.Model.AdmitSource != null)
            {
                this.cmbAdmitSrc.SelectedItem = this.Model.AdmitSource;
            }

            // OTD# 37055 fix - Admit source field should not be editable for a Newborn account
            if (Model.Activity!=null && Model.Activity.IsNewBornRelatedActivity())
            {
                this.cmbAdmitSrc.Enabled = false;
            }
        }

        public void PopulateAlternateCareFacility()
        {
            var alternateCareFacilityBroker = 
                BrokerFactory.BrokerOfType<IAlternateCareFacilityBroker>();
            var allAlternateCareFacilities = 
                alternateCareFacilityBroker.AllAlternateCareFacilities( User.GetCurrent().Facility.Oid );

            cmbAlternateCareFacility.Items.Clear();

            foreach( var alternateCareFacility in allAlternateCareFacilities )
            {
                cmbAlternateCareFacility.Items.Add( alternateCareFacility );
            }
            
            // If the value is not in the list, add it as a valid choice. This will
            // prevent data loss in the event that the value stored with the account
            // is removed from the lookup table
            if( !cmbAlternateCareFacility.Items.Contains( Model.AlternateCareFacility ) )
            {
                cmbAlternateCareFacility.Items.Add( Model.AlternateCareFacility );
            }
             
            cmbAlternateCareFacility.SelectedItem = Model.AlternateCareFacility;
        }

        public void ClearAlternateCareFacility()
        {
            if ( cmbAlternateCareFacility.Items.Count > 0 )
            {
                cmbAlternateCareFacility.SelectedIndex = 0;
            }
        }

        public void ShowAlternateCareFacilityDisabled()
        {
           UIColors.SetDisabledDarkBgColor( cmbAlternateCareFacility );
           cmbAlternateCareFacility.Enabled = false;
        }

        public void ShowAlternateCareFacilityEnabled()
        {
            UIColors.SetNormalBgColor( cmbAlternateCareFacility );
            cmbAlternateCareFacility.Enabled = true;
        }

        public void ShowAlternateCareFacilityVisible()
        {
            cmbAlternateCareFacility.Visible = true;
            lblAlternateCareFacility.Visible = true; 
        }

        public void ShowAlternateCareFacilityNotVisible()
        {
            cmbAlternateCareFacility.Visible = false;
            lblAlternateCareFacility.Visible = false;
        }

        private void PopulateReferralSource()
        {
            ReferralSourceBrokerProxy brokerProxy = new ReferralSourceBrokerProxy();
            ICollection allSources = brokerProxy.AllReferralSources( User.GetCurrent().Facility.Oid );

            this.cmbReferralSrc.Items.Clear();

            foreach (ReferralSource source in allSources)
            {
                this.cmbReferralSrc.Items.Add( source );
            }

            if (this.Model.ReferralSource != null)
            {
                this.cmbReferralSrc.SelectedItem = this.Model.ReferralSource;
            }
        }

        /// <summary>
        /// Display Countries
        /// </summary>
        private void DisplayCountries()
        {
            if (this.cmbCountry.Items.Count == 0)
            {
                IAddressBroker broker = new AddressBrokerProxy();
                this.CountryComboHelper = new ReferenceValueComboBox( this.cmbCountry );

                this.CountryComboHelper.PopulateWithCollection( broker.AllCountries( this.Model.Facility.Oid ) );
                this.cmbCountry.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Display States
        /// </summary>
        private void DisplayStates()
        {
            if (this.cmbState.Items.Count == 0)
            {
                IAddressBroker broker = new AddressBrokerProxy();

                if (this.cmbState.Items.Count == 0)
                {
                    this.StateComboHelper.PopulateWithCollection(broker.AllStates(User.GetCurrent().Facility.Oid));
                }

                foreach (ContactPoint contactPoint in this.Model.Facility.ContactPoints)
                {
                    if (contactPoint != null && contactPoint.TypeOfContactPoint.ToString() == TypeOfContactPoint.NewPhysicalContactPointType().ToString())
                    {
                        this.cmbState.Text = contactPoint.Address.State.Code + "-" +
                            contactPoint.Address.State.DisplayString;
                    }
                }
            }
        }

        /// <summary>
        /// Display Accident Types
        /// </summary>
        private void DisplayAccidentTypes()
        {
            if (this.cmbAccidentType.Items.Count == 0)
            {
                OccuranceCodeBrokerProxy brokerProxy = new OccuranceCodeBrokerProxy();
                IList accidentTypeList = (IList)brokerProxy.GetAccidentTypes( User.GetCurrent().Facility.Oid );

                if (accidentTypeList == null)
                {
                    MessageBox.Show( "IOccuranceCodeBroker.GetAccidentTypes() returned empty list.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }

                this.cmbAccidentType.ValueMember = "Value";
                this.cmbAccidentType.DisplayMember = "Key";
                //Add Default blank
                this.cmbAccidentType.Items.Add( String.Empty );

                DictionaryEntry other = new DictionaryEntry( "Other", TypeOfAccident.OTHER );

                foreach (TypeOfAccident typeOfAccident in accidentTypeList)
                {
                    DictionaryEntry newEntry = new DictionaryEntry( GetAccidentTypeDisplayString( typeOfAccident ),
                            typeOfAccident );

                    if (typeOfAccident.Oid == TypeOfAccident.OTHER)
                    {
                        other = newEntry;
                    }

                    this.cmbAccidentType.Items.Add( newEntry );
                }

                this.cmbAccidentType.Sorted = true;
                this.cmbAccidentType.Items.Remove( other );
                this.cmbAccidentType.Sorted = false;
                this.cmbAccidentType.Items.Add( other );
            }
        }

        private static string GetAccidentTypeDisplayString( TypeOfAccident typeOfAccident )
        {
            string accidentTypeDisplayString;

            if (typeOfAccident.Description == "AutoNoFaultInsurance")
            {
                accidentTypeDisplayString = "Auto no fault insurance";
            }
            else if (typeOfAccident.Description == "Employment Related")
            {
                accidentTypeDisplayString = "Employment-related";
            }
            else if (typeOfAccident.Description == "Tort Liability")
            {
                accidentTypeDisplayString = "Tort liability";
            }
            else
            {
                accidentTypeDisplayString = typeOfAccident.Description;
            }

            return accidentTypeDisplayString;
        }

        /// <summary>
        /// Display Chief Complaint
        /// </summary>
        private void DisplayChiefComplaint()
        {
            this.mtbComplaint.Text = this.Model.Diagnosis.ChiefComplaint.Trim();
        }
        /// <summary>
        /// Display Procedure
        /// </summary>
        void IDiagnosisView.PopulateProcedure()
        {
            if (this.Model.Diagnosis.Procedure != null)
            {
                this.mtbProcedure.Text = this.Model.Diagnosis.Procedure.Trim();
            }
            else
            {
                this.mtbProcedure.Text = String.Empty;
            }
        }

        /// <summary>
        /// Set Accident Crime Date
        /// </summary>

        private void SetAccidentCrimeDate()
        {
            TimeAndLocationBoundCondition condition = (TimeAndLocationBoundCondition)this.Model.Diagnosis.Condition;
            condition.OccurredOn = Convert.ToDateTime( this.dateTimePickerAccident.Text );
        }

        /// <summary>
        /// Set Accident Crime Hour
        /// </summary>
        private void SetAccidentCrimeHour()
        {
            TimeAndLocationBoundCondition condition = (TimeAndLocationBoundCondition)this.Model.Diagnosis.Condition;

            condition.OccurredAtHour = String.Empty;
            if (this.cmbHour.SelectedItem != null)
            {
                condition.OccurredAtHour = this.cmbHour.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Set Accident Crime Country
        /// </summary>
        /// <param name="country"></param>
        private void SetAccidentCrimeCountry( Country country )
        {
            if (this.rbAccident.Checked || this.rbCrime.Checked)
            {
                TimeAndLocationBoundCondition condition = (TimeAndLocationBoundCondition)this.Model.Diagnosis.Condition;
                condition.Country = country;
            }
        }

        /// <summary>
        /// Set Accident Crime State
        /// </summary>
        /// <param name="state"></param>
        private void SetAccidentCrimeState( State state )
        {
            TimeAndLocationBoundCondition condition = (TimeAndLocationBoundCondition)this.Model.Diagnosis.Condition;
            condition.State = state;
        }

        /// <summary>
        /// Set Onset Date Error Bg Color
        /// </summary>
        private void SetOnsetDateErrBgColor()
        {
            UIColors.SetErrorBgColor( this.mtbOnsetDate );
        }

        /// <summary>
        /// Set Onset Date Normal Bg Color
        /// </summary>
        private void SetOnsetDateNormalBgColor()
        {
            UIColors.SetNormalBgColor( this.mtbOnsetDate );
            this.Refresh();
        }

        private void SetAccidentCrimeDateErrBgColor()
        {
            UIColors.SetErrorBgColor( this.mtbAccidentCrimeDate );
        }

        private void SetAccidentCrimeDateNormalBgColor()
        {
            UIColors.SetNormalBgColor( this.mtbAccidentCrimeDate );
            this.Refresh();
        }

        /// <summary>
        /// Purpose of this method is to facilitate rule execution by ensuring 
        /// that a control, passed in as a parameter, has the focus set 
        /// to itself and then its background color set to red (an error).
        /// 
        /// </summary>
        /// <param name="control">name of control that needs to be highlighted red.</param>
        private static void GetFocusAndHighlightRedError( Control control )
        {
            if (null != control)
            {
                control.Focus();
                SetErrorBackgroundColor( control );
            }
        }

        /// <summary>
        /// Purpose of this method is to set the color of a masked 
        /// edit text box to red indicating there is an error with the field.
        /// </summary>
        /// <param name="control"></param>
        private static void SetErrorBackgroundColor( Control control )
        {
            if (null != control)
            {
                UIColors.SetErrorBgColor( control );
            }
        }

        /// <summary>
        /// Verify Accident Crime Date
        /// </summary>
        private bool VerifyAccidentCrimeDate()
        {
            this.SetAccidentCrimeDateNormalBgColor();
            if (this.mtbAccidentCrimeDate.UnMaskedText.Length == 0
                || this.mtbAccidentCrimeDate.UnMaskedText ==
                String.Empty)
            {
                return true;
            }

            if (this.mtbAccidentCrimeDate.Text.Length != 10)
            {
                this.mtbAccidentCrimeDate.Focus();
                this.SetAccidentCrimeDateErrBgColor();
                MessageBox.Show( UIErrorMessages.DIAGNOSIS_ACCIDENTCRIME_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return false;
            }

            string month = this.mtbAccidentCrimeDate.Text.Substring( 0, 2 );
            string day = this.mtbAccidentCrimeDate.Text.Substring( 3, 2 );
            string year = this.mtbAccidentCrimeDate.Text.Substring( 6, 4 );

            this.verifyMonth = Convert.ToInt32( month );
            this.verifyDay = Convert.ToInt32( day );
            this.verifyYear = Convert.ToInt32( year );

            try
            {   // Check the date entered is not in the future
                DateTime theDate = new DateTime( this.verifyYear, this.verifyMonth, this.verifyDay );

                if (DateValidator.IsValidDate( theDate ) == false)
                {
                    GetFocusAndHighlightRedError( this.mtbAccidentCrimeDate );
                    ShowErrorMessageBox( UIErrorMessages.DIAGNOSIS_ACCIDENTCRIME_INVALID_ERRMSG );
                    return false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                GetFocusAndHighlightRedError( this.mtbAccidentCrimeDate );
                ShowErrorMessageBox( UIErrorMessages.DIAGNOSIS_ACCIDENTCRIME_INVALID_ERRMSG );
                return false;
            }

            TimeAndLocationBoundCondition condition = (TimeAndLocationBoundCondition)this.Model.Diagnosis.Condition;
            condition.OccurredOn = Convert.ToDateTime( this.mtbAccidentCrimeDate.Text );

            return true;
        }

        /// <summary>
        /// Verify Onset Date
        /// </summary>
        private bool VerifyOnsetDate()
        {
            this.SetOnsetDateNormalBgColor();
            if (this.mtbOnsetDate.UnMaskedText.Length == 0 ||
                this.mtbOnsetDate.UnMaskedText == String.Empty)
            {
                OccurrenceCode occ11 = new OccurrenceCode( PersistentModel.NEW_OID,
                    PersistentModel.NEW_VERSION,
                    string.Empty,
                    "11" );
                this.Model.RemoveOccurrenceCode( occ11 );
                return true;
            }

            if (this.mtbOnsetDate.Text.Length != 10)
            {
                this.mtbOnsetDate.Focus();
                this.SetOnsetDateErrBgColor();
                MessageBox.Show( UIErrorMessages.DIAGNOSIS_ONSETDATE_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return false;
            }

            string month = this.mtbOnsetDate.Text.Substring( 0, 2 );
            string day = this.mtbOnsetDate.Text.Substring( 3, 2 );
            string year = this.mtbOnsetDate.Text.Substring( 6, 4 );

            this.verifyMonth = Convert.ToInt32( month );
            this.verifyDay = Convert.ToInt32( day );
            this.verifyYear = Convert.ToInt32( year );

            try
            {   // Check the date entered is not in the future
                DateTime theDate = new DateTime( this.verifyYear, this.verifyMonth, this.verifyDay );

                if (!DateValidator.IsValidDate( theDate ))
                {
                    GetFocusAndHighlightRedError( this.mtbOnsetDate );
                    ShowErrorMessageBox( UIErrorMessages.DIAGNOSIS_ONSETDATE_INVALID_ERRMSG );
                    return false;
                }

                OccuranceCodeBrokerProxy brokerProxy = new OccuranceCodeBrokerProxy();
                OccurrenceCode occ11 = brokerProxy.OccurrenceCodeWith( User.GetCurrent().Facility.Oid, OccurrenceCode.OCCURRENCECODE_ILLNESS );
                occ11.OccurrenceDate = theDate;
                this.Model.AddOccurrenceCode( (OccurrenceCode)( occ11.Clone() ) );
            }
            catch (ArgumentOutOfRangeException)
            {
                GetFocusAndHighlightRedError( this.mtbOnsetDate );
                ShowErrorMessageBox( UIErrorMessages.DIAGNOSIS_ONSETDATE_INVALID_ERRMSG );
                return false;
            }

            return true;
        }


        /// <summary>
        /// Display Illness. not used
        /// </summary>
        private void DisplayIllness()
        {
            bool onSetEnabled = true;
            this.mtbAccidentCrimeDate.UnMaskedText = String.Empty;
            this.cmbHour.SelectedIndex = -1;
            this.cmbCountry.SelectedIndex = -1;
            this.cmbState.SelectedIndex = -1;

            this.rbNone.Checked = true;
            if (typeof( AdmitNewbornActivity ).Equals( this.Model.Activity.GetType() ) )
            {
                onSetEnabled = false;
            }

            this.mtbOnsetDate.Enabled = onSetEnabled;
            this.dateTimePickerSickness.Enabled = onSetEnabled;
        }


        /// <summary>
        /// Display Accident
        /// </summary>
        private void DisplayAccident()
        {
            this.rbAccident.Checked = true;
            this.cmbAccidentType.Enabled = true;

            try
            {
                if (this.Model.Diagnosis != null && this.Model.Diagnosis.Condition != null)
                {
                    Accident accident = (Accident)this.Model.Diagnosis.Condition;
                    if (accident != null && accident.Kind != null)
                    {
                        int index = this.cmbAccidentType.FindStringExact( GetAccidentTypeDisplayString( accident.Kind ) );
                        if (index != -1)
                        {
                            this.cmbAccidentType.SelectedIndex = index;
                        }
                    }

                    if (accident != null)
                    {
                        this.accidentType = accident.Kind;
                    }
                    this.EnableAccidentCrimeDetails();
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Display Crime
        /// </summary>
        private void DisplayCrime()
        {
            this.rbCrime.Checked = true;

            try
            {
                if (this.Model.Diagnosis != null && this.Model.Diagnosis.Condition != null)
                {
                    this.EnableAccidentCrimeDetails();
                }
            }
            //            catch( Exception ex )
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Display None
        /// </summary>
        private void DisplayNone()
        {
            this.rbNone.Checked = true;
            bool onSetEnabled = true;

            try
            {
                if (this.Model.Diagnosis != null && this.Model.Diagnosis.Condition != null)
                {
                    UnknownCondition unKnownCondition = new UnknownCondition();
                    this.Model.Diagnosis.Condition = unKnownCondition;

                    if (typeof( AdmitNewbornActivity ).Equals( this.Model.Activity.GetType() ) )
                    {
                        onSetEnabled = false;
                    }

                    this.mtbOnsetDate.Enabled = onSetEnabled;

                    this.dateTimePickerSickness.Enabled = onSetEnabled;
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Populate Condition Map, this one to be public just for Nunit test success
        /// </summary>
        private void PopulateConditionMap()
        {
            //          i_ConditionMap = new Hashtable();
            //            i_ConditionMap.Add( typeof( Pregnancy ), new DisplayAbstractCondition( DisplayPregnancy ) );
            this.i_ConditionMap.Add( typeof( Illness ), new DisplayAbstractCondition( this.DisplayIllness ) );
            this.i_ConditionMap.Add( typeof( Accident ), new DisplayAbstractCondition( this.DisplayAccident ) );
            this.i_ConditionMap.Add( typeof( Crime ), new DisplayAbstractCondition( this.DisplayCrime ) );
            this.i_ConditionMap.Add( typeof( UnknownCondition ), new DisplayAbstractCondition( this.DisplayNone ) );
        }

        /// <summary>
        /// Enable Accident Crime Details
        /// </summary>
        private void EnableAccidentCrimeDetails()
        {
            TimeAndLocationBoundCondition condition = (TimeAndLocationBoundCondition)this.Model.Diagnosis.Condition;

            if (condition.OccurredOn == DateTime.MinValue)
            {
                this.mtbAccidentCrimeDate.Text = String.Empty;
            }
            else
            {
                this.mtbAccidentCrimeDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}",
                    condition.OccurredOn.Month,
                    condition.OccurredOn.Day,
                    condition.OccurredOn.Year );
            }

            this.accidentDateTxt = this.mtbAccidentCrimeDate.Text;

            if (condition.OccurredAtHour == String.Empty)
            {
                this.cmbHour.SelectedIndex = 0;
            }
            else
            {
                var searchValue = condition.GetOccurredHour();

                this.cmbHour.SelectedIndex = this.cmbHour.FindString( searchValue );
            }

            if (condition.Country != null)
            {
                this.cmbCountry.SelectedItem = condition.Country;
            }

            if (condition.State != null)
            {
                this.cmbState.SelectedItem = condition.State;
            }

            //SR 1557, do not disable AccidentCrimeDate for Create/Edit/Activate Pre-Admit Newborn.
            //And leave Register Newborn for now.
            if (Model.Activity!=null && Model.Activity.IsAdmitNewbornActivity() 
                    && !(Model.Activity.AssociatedActivityType !=null && Model.Activity.AssociatedActivityType==typeof(ActivatePreRegistrationActivity)))
            {
                UIColors.SetDisabledDarkBgColor( this.mtbOnsetDate );
            }
             
            this.mtbAccidentCrimeDate.Enabled = true;
            this.dateTimePickerAccident.Enabled = true;
            

            this.cmbHour.Enabled = true;
            this.cmbCountry.Enabled = true;
            this.cmbState.Enabled = true;
            this.SetAccidentCrimeControlsNormalBackGroundColor();
        }

        private void SetAccidentCrimeControlsNormalBackGroundColor()
        {
            if (this.rbAccident.Checked)
            {
                UIColors.SetNormalBgColor( this.cmbAccidentType );
            }

            UIColors.SetNormalBgColor( this.mtbAccidentCrimeDate );
            UIColors.SetNormalBgColor( this.cmbHour );
            UIColors.SetNormalBgColor( this.cmbCountry );
            UIColors.SetNormalBgColor( this.cmbState );
            UIColors.SetNormalBgColor( this.mtbOnsetDate );
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureChiefComplaint( mtbComplaint );
            MaskedEditTextBoxBuilder.ConfigureProcedure( mtbProcedure );
        }

        #endregion

        #region Internal Properties
        #endregion

        #region Private Properties

        private ArrayList PatientTypes
        {
            get
            {
                if (this.i_PatientTypes == null)
                {
                    string activityType = string.Empty;
                    string associatedActivityType = string.Empty;
                    string kindOfVisitCode = string.Empty;
                    string financialClassCode = string.Empty;
                    string locationBedCode = string.Empty;

                    Activity activity = this.Model.Activity;
                    if (activity != null)
                    {
                        activityType = activity.GetType().ToString();
                        if (activity.AssociatedActivityType != null)
                        {
                            associatedActivityType = activity.AssociatedActivityType.ToString();
                        }
                    }
                    if (this.Model.KindOfVisit != null)
                    {
                        kindOfVisitCode = this.Model.KindOfVisit.Code;
                    }
                    if (this.Model.FinancialClass != null)
                    {
                        financialClassCode = this.Model.FinancialClass.Code;
                    }
                    if (this.Model.Location != null &&
                        this.Model.Location.Bed != null)
                    {
                        locationBedCode = this.Model.Location.Bed.Code;
                    }

                    IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
                    this.i_PatientTypes = patientBroker.PatientTypesFor( activityType, associatedActivityType, kindOfVisitCode,
                                                                         financialClassCode, locationBedCode, this.Model.Facility.Oid );
                }

                return this.i_PatientTypes;
            }
        }

        private ReferenceValueComboBox StateComboHelper
        {
            get
            {
                return this.i_StateComboHelper;
            }
            set
            {
                this.i_StateComboHelper = value;
            }
        }


        private ReferenceValueComboBox CountryComboHelper
        {
            get
            {
                return this.i_CountryComboHelper;
            }
            set
            {
                this.i_CountryComboHelper = value;
            }
        }

        private IRuleEngine RuleEngine
        {
            get { return ruleEngine; }
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagnosisView));
            this.lblComplaint = new System.Windows.Forms.Label();
            this.lblVisitResult = new System.Windows.Forms.Label();
            this.gbxDetails = new System.Windows.Forms.GroupBox();
            this.mtbAccidentCrimeDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbHour = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbState = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbCountry = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblCountry = new System.Windows.Forms.Label();
            this.lblHour = new System.Windows.Forms.Label();
            this.dateTimePickerAccident = new System.Windows.Forms.DateTimePicker();
            this.lblState = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.dateTimePickerSickness = new System.Windows.Forms.DateTimePicker();
            this.lblOnset = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.mtbOnsetDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblTenetCare = new System.Windows.Forms.Label();
            this.lblAdmit = new System.Windows.Forms.Label();
            this.lblReferral = new System.Windows.Forms.Label();
            this.gbxClinics = new System.Windows.Forms.GroupBox();
            this.clinicView5 = new PatientAccess.UI.CommonControls.ClinicView();
            this.clinicView4 = new PatientAccess.UI.CommonControls.ClinicView();
            this.clinicView3 = new PatientAccess.UI.CommonControls.ClinicView();
            this.clinicView2 = new PatientAccess.UI.CommonControls.ClinicView();
            this.clinicView1 = new PatientAccess.UI.CommonControls.ClinicView();
            this.gbxEDLog = new System.Windows.Forms.GroupBox();
            this.mtbArrivalTime = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbReadmitCode = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbReferralFacility = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbReferralType = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbModeOfArrival = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblReAdmitCode = new System.Windows.Forms.Label();
            this.lblReferralActivity = new System.Windows.Forms.Label();
            this.lblReferralType = new System.Windows.Forms.Label();
            this.lblModeOfArrival = new System.Windows.Forms.Label();
            this.lblArrivalTime = new System.Windows.Forms.Label();
            this.mtbComplaint = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblProcedure = new System.Windows.Forms.Label();
            this.mtbProcedure = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblAlternateCareFacility = new System.Windows.Forms.Label();
            this.cmbAlternateCareFacility = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.patientTypeHSVLocationView = new PatientAccess.UI.CommonControls.PatientTypeHSVLocationView();
            this.cmbReferralSrc = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbAdmitSrc = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbTenetCare = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbAccidentType = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.rbNone = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.rbCrime = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.rbAccident = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.cptCodesView = new PatientAccess.UI.CptCodes.ViewImpl.CptCodesView();
            this.serviceCategory1 = new PatientAccess.UI.CommonControls.ServiceCategory.ViewImpl.ServiceCategory();
            this.gbxDetails.SuspendLayout();
            this.gbxClinics.SuspendLayout();
            this.gbxEDLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblComplaint
            // 
            this.lblComplaint.Location = new System.Drawing.Point(3, 241);
            this.lblComplaint.Name = "lblComplaint";
            this.lblComplaint.Size = new System.Drawing.Size(88, 14);
            this.lblComplaint.TabIndex = 0;
            this.lblComplaint.Text = "Chief complaint:";
            // 
            // lblVisitResult
            // 
            this.lblVisitResult.Location = new System.Drawing.Point(353, 8);
            this.lblVisitResult.Name = "lblVisitResult";
            this.lblVisitResult.Size = new System.Drawing.Size(171, 15);
            this.lblVisitResult.TabIndex = 4;
            this.lblVisitResult.Text = "The Patient\'s visit is the result of:";
            // 
            // gbxDetails
            // 
            this.gbxDetails.Controls.Add(this.mtbAccidentCrimeDate);
            this.gbxDetails.Controls.Add(this.cmbHour);
            this.gbxDetails.Controls.Add(this.cmbState);
            this.gbxDetails.Controls.Add(this.cmbCountry);
            this.gbxDetails.Controls.Add(this.lblCountry);
            this.gbxDetails.Controls.Add(this.lblHour);
            this.gbxDetails.Controls.Add(this.dateTimePickerAccident);
            this.gbxDetails.Controls.Add(this.lblState);
            this.gbxDetails.Controls.Add(this.lblDate);
            this.gbxDetails.Location = new System.Drawing.Point(352, 79);
            this.gbxDetails.Name = "gbxDetails";
            this.gbxDetails.Size = new System.Drawing.Size(310, 127);
            this.gbxDetails.TabIndex = 9;
            this.gbxDetails.TabStop = false;
            this.gbxDetails.Text = "Accident or crime details";
            // 
            // mtbAccidentCrimeDate
            // 
            this.mtbAccidentCrimeDate.KeyPressExpression = "^\\d*$";
            this.mtbAccidentCrimeDate.Location = new System.Drawing.Point(91, 22);
            this.mtbAccidentCrimeDate.Mask = "  /  /";
            this.mtbAccidentCrimeDate.MaxLength = 10;
            this.mtbAccidentCrimeDate.Name = "mtbAccidentCrimeDate";
            this.mtbAccidentCrimeDate.Size = new System.Drawing.Size(70, 20);
            this.mtbAccidentCrimeDate.TabIndex = 1;
            this.mtbAccidentCrimeDate.ValidationExpression = resources.GetString("mtbAccidentCrimeDate.ValidationExpression");
            this.mtbAccidentCrimeDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbAccidentCrimeDate_Validating);
            // 
            // cmbHour
            // 
            this.cmbHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHour.Items.AddRange(new object[] {
            "",
            "Unknown",
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.cmbHour.Location = new System.Drawing.Point(91, 48);
            this.cmbHour.Name = "cmbHour";
            this.cmbHour.Size = new System.Drawing.Size(92, 21);
            this.cmbHour.TabIndex = 3;
            this.cmbHour.SelectedIndexChanged += new System.EventHandler(this.cmbHour_SelectedIndexChanged);
            this.cmbHour.Validating += new System.ComponentModel.CancelEventHandler(this.cmbHour_Validating);
            // 
            // cmbState
            // 
            this.cmbState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbState.Location = new System.Drawing.Point(91, 99);
            this.cmbState.Name = "cmbState";
            this.cmbState.Size = new System.Drawing.Size(205, 21);
            this.cmbState.TabIndex = 5;
            this.cmbState.SelectedIndexChanged += new System.EventHandler(this.cmbState_SelectedIndexChanged);
            this.cmbState.Validating += new System.ComponentModel.CancelEventHandler(this.cmbState_Validating);
            // 
            // cmbCountry
            // 
            this.cmbCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCountry.Location = new System.Drawing.Point(91, 73);
            this.cmbCountry.Name = "cmbCountry";
            this.cmbCountry.Size = new System.Drawing.Size(205, 21);
            this.cmbCountry.TabIndex = 4;
            this.cmbCountry.SelectedIndexChanged += new System.EventHandler(this.cmbCountry_SelectedIndexChanged);
            this.cmbCountry.Validating += new System.ComponentModel.CancelEventHandler(this.cmbCountry_Validating);
            // 
            // lblCountry
            // 
            this.lblCountry.Location = new System.Drawing.Point(8, 77);
            this.lblCountry.Name = "lblCountry";
            this.lblCountry.Size = new System.Drawing.Size(49, 16);
            this.lblCountry.TabIndex = 0;
            this.lblCountry.Text = "Country:";
            // 
            // lblHour
            // 
            this.lblHour.Location = new System.Drawing.Point(8, 51);
            this.lblHour.Name = "lblHour";
            this.lblHour.Size = new System.Drawing.Size(34, 16);
            this.lblHour.TabIndex = 0;
            this.lblHour.Text = "Hour:";
            // 
            // dateTimePickerAccident
            // 
            this.dateTimePickerAccident.Location = new System.Drawing.Point(160, 22);
            this.dateTimePickerAccident.Name = "dateTimePickerAccident";
            this.dateTimePickerAccident.Size = new System.Drawing.Size(22, 20);
            this.dateTimePickerAccident.TabIndex = 2;
            this.dateTimePickerAccident.TabStop = false;
            this.dateTimePickerAccident.CloseUp += new System.EventHandler(this.dateTimePickerAccident_CloseUp);
            // 
            // lblState
            // 
            this.lblState.Location = new System.Drawing.Point(8, 103);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(81, 16);
            this.lblState.TabIndex = 0;
            this.lblState.Text = "State/Province:";
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(8, 27);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(36, 16);
            this.lblDate.TabIndex = 0;
            this.lblDate.Text = "Date:";
            // 
            // dateTimePickerSickness
            // 
            this.dateTimePickerSickness.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dateTimePickerSickness.Location = new System.Drawing.Point(616, 212);
            this.dateTimePickerSickness.Name = "dateTimePickerSickness";
            this.dateTimePickerSickness.Size = new System.Drawing.Size(21, 20);
            this.dateTimePickerSickness.TabIndex = 12;
            this.dateTimePickerSickness.TabStop = false;
            this.dateTimePickerSickness.CloseUp += new System.EventHandler(this.dateTimePickerSickness_CloseUp);
            // 
            // lblOnset
            // 
            this.lblOnset.Location = new System.Drawing.Point(360, 215);
            this.lblOnset.Name = "lblOnset";
            this.lblOnset.Size = new System.Drawing.Size(193, 16);
            this.lblOnset.TabIndex = 10;
            this.lblOnset.Text = "Date of onset for symptoms or illness:";
            // 
            // lblType
            // 
            this.lblType.Location = new System.Drawing.Point(442, 34);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(34, 16);
            this.lblType.TabIndex = 21;
            this.lblType.Text = "Type:";
            // 
            // mtbOnsetDate
            // 
            this.mtbOnsetDate.KeyPressExpression = "^\\d*$";
            this.mtbOnsetDate.Location = new System.Drawing.Point(547, 212);
            this.mtbOnsetDate.Mask = "  /  /";
            this.mtbOnsetDate.MaxLength = 10;
            this.mtbOnsetDate.Name = "mtbOnsetDate";
            this.mtbOnsetDate.Size = new System.Drawing.Size(70, 20);
            this.mtbOnsetDate.TabIndex = 11;
            this.mtbOnsetDate.ValidationExpression = resources.GetString("mtbOnsetDate.ValidationExpression");
            this.mtbOnsetDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbOnsetDate_Validating);
            // 
            // lblTenetCare
            // 
            this.lblTenetCare.Location = new System.Drawing.Point(361, 269);
            this.lblTenetCare.Name = "lblTenetCare";
            this.lblTenetCare.Size = new System.Drawing.Size(61, 13);
            this.lblTenetCare.TabIndex = 0;
            this.lblTenetCare.Text = "TenetCare:";
            // 
            // lblAdmit
            // 
            this.lblAdmit.Location = new System.Drawing.Point(361, 293);
            this.lblAdmit.Name = "lblAdmit";
            this.lblAdmit.Size = new System.Drawing.Size(74, 18);
            this.lblAdmit.TabIndex = 0;
            this.lblAdmit.Text = "Admit source:";
            // 
            // lblReferral
            // 
            this.lblReferral.Location = new System.Drawing.Point(361, 343);
            this.lblReferral.Name = "lblReferral";
            this.lblReferral.Size = new System.Drawing.Size(85, 13);
            this.lblReferral.TabIndex = 0;
            this.lblReferral.Text = "Referral source:";
            // 
            // gbxClinics
            // 
            this.gbxClinics.Controls.Add(this.clinicView5);
            this.gbxClinics.Controls.Add(this.clinicView4);
            this.gbxClinics.Controls.Add(this.clinicView3);
            this.gbxClinics.Controls.Add(this.clinicView2);
            this.gbxClinics.Controls.Add(this.clinicView1);
            this.gbxClinics.Location = new System.Drawing.Point(675, 0);
            this.gbxClinics.Name = "gbxClinics";
            this.gbxClinics.Size = new System.Drawing.Size(261, 162);
            this.gbxClinics.TabIndex = 18;
            this.gbxClinics.TabStop = false;
            this.gbxClinics.Text = "Clinics";
            // 
            // clinicView5
            // 
            this.clinicView5.BackColor = System.Drawing.Color.White;
            this.clinicView5.LabelText = "Clinic 1:";
            this.clinicView5.Location = new System.Drawing.Point(6, 129);
            this.clinicView5.Model = null;
            this.clinicView5.Name = "clinicView5";
            this.clinicView5.Size = new System.Drawing.Size(242, 21);
            this.clinicView5.TabIndex = 4;
            this.clinicView5.HospitalClinicSelected += new System.EventHandler(this.clinicView5_HospitalClinicSelected);
            this.clinicView5.HospitalClinicValidating += new System.ComponentModel.CancelEventHandler(this.clinicView5_HospitalClinicValidating);
            // 
            // clinicView4
            // 
            this.clinicView4.BackColor = System.Drawing.Color.White;
            this.clinicView4.LabelText = "Clinic 1:";
            this.clinicView4.Location = new System.Drawing.Point(6, 102);
            this.clinicView4.Model = null;
            this.clinicView4.Name = "clinicView4";
            this.clinicView4.Size = new System.Drawing.Size(242, 21);
            this.clinicView4.TabIndex = 3;
            this.clinicView4.HospitalClinicSelected += new System.EventHandler(this.clinicView4_HospitalClinicSelected);
            this.clinicView4.HospitalClinicValidating += new System.ComponentModel.CancelEventHandler(this.clinicView4_HospitalClinicValidating);
            // 
            // clinicView3
            // 
            this.clinicView3.BackColor = System.Drawing.Color.White;
            this.clinicView3.LabelText = "Clinic 1:";
            this.clinicView3.Location = new System.Drawing.Point(6, 75);
            this.clinicView3.Model = null;
            this.clinicView3.Name = "clinicView3";
            this.clinicView3.Size = new System.Drawing.Size(242, 21);
            this.clinicView3.TabIndex = 2;
            this.clinicView3.HospitalClinicSelected += new System.EventHandler(this.clinicView3_HospitalClinicSelected);
            this.clinicView3.HospitalClinicValidating += new System.ComponentModel.CancelEventHandler(this.clinicView3_HospitalClinicValidating);
            // 
            // clinicView2
            // 
            this.clinicView2.BackColor = System.Drawing.Color.White;
            this.clinicView2.LabelText = "Clinic 1:";
            this.clinicView2.Location = new System.Drawing.Point(6, 48);
            this.clinicView2.Model = null;
            this.clinicView2.Name = "clinicView2";
            this.clinicView2.Size = new System.Drawing.Size(242, 21);
            this.clinicView2.TabIndex = 1;
            this.clinicView2.HospitalClinicSelected += new System.EventHandler(this.clinicView2_HospitalClinicSelected);
            this.clinicView2.HospitalClinicValidating += new System.ComponentModel.CancelEventHandler(this.clinicView2_HospitalClinicValidating);
            // 
            // clinicView1
            // 
            this.clinicView1.BackColor = System.Drawing.Color.White;
            this.clinicView1.LabelText = "Clinic 1:";
            this.clinicView1.Location = new System.Drawing.Point(6, 21);
            this.clinicView1.Model = null;
            this.clinicView1.Name = "clinicView1";
            this.clinicView1.Size = new System.Drawing.Size(242, 21);
            this.clinicView1.TabIndex = 0;
            this.clinicView1.HospitalClinicSelected += new System.EventHandler(this.clinicView1_HospitalClinicSelected);
            this.clinicView1.HospitalClinicValidating += new System.ComponentModel.CancelEventHandler(this.clinicView1_HospitalClinicValidating);
            // 
            // gbxEDLog
            // 
            this.gbxEDLog.Controls.Add(this.mtbArrivalTime);
            this.gbxEDLog.Controls.Add(this.cmbReadmitCode);
            this.gbxEDLog.Controls.Add(this.cmbReferralFacility);
            this.gbxEDLog.Controls.Add(this.cmbReferralType);
            this.gbxEDLog.Controls.Add(this.cmbModeOfArrival);
            this.gbxEDLog.Controls.Add(this.lblReAdmitCode);
            this.gbxEDLog.Controls.Add(this.lblReferralActivity);
            this.gbxEDLog.Controls.Add(this.lblReferralType);
            this.gbxEDLog.Controls.Add(this.lblModeOfArrival);
            this.gbxEDLog.Controls.Add(this.lblArrivalTime);
            this.gbxEDLog.Location = new System.Drawing.Point(675, 193);
            this.gbxEDLog.Name = "gbxEDLog";
            this.gbxEDLog.Size = new System.Drawing.Size(261, 153);
            this.gbxEDLog.TabIndex = 19;
            this.gbxEDLog.TabStop = false;
            this.gbxEDLog.Text = "Emergency Department Log Information";
            // 
            // mtbArrivalTime
            // 
            this.mtbArrivalTime.KeyPressExpression = "^\\d*$";
            this.mtbArrivalTime.Location = new System.Drawing.Point(94, 23);
            this.mtbArrivalTime.Mask = "  :";
            this.mtbArrivalTime.MaxLength = 5;
            this.mtbArrivalTime.Name = "mtbArrivalTime";
            this.mtbArrivalTime.Size = new System.Drawing.Size(37, 20);
            this.mtbArrivalTime.TabIndex = 1;
            this.mtbArrivalTime.ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$";
            this.mtbArrivalTime.Validating += new System.ComponentModel.CancelEventHandler(this.mtbArrivalTime_Validating);
            // 
            // cmbReadmitCode
            // 
            this.cmbReadmitCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReadmitCode.Location = new System.Drawing.Point(94, 123);
            this.cmbReadmitCode.Name = "cmbReadmitCode";
            this.cmbReadmitCode.Size = new System.Drawing.Size(158, 21);
            this.cmbReadmitCode.TabIndex = 5;
            this.cmbReadmitCode.SelectedIndexChanged += new System.EventHandler(this.cmbReadmitCode_SelectedIndexChanged);
            this.cmbReadmitCode.Validating += new System.ComponentModel.CancelEventHandler(this.cmbReadmitCode_Validating);
            // 
            // cmbReferralFacility
            // 
            this.cmbReferralFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReferralFacility.Location = new System.Drawing.Point(94, 98);
            this.cmbReferralFacility.Name = "cmbReferralFacility";
            this.cmbReferralFacility.Size = new System.Drawing.Size(158, 21);
            this.cmbReferralFacility.TabIndex = 4;
            this.cmbReferralFacility.SelectedIndexChanged += new System.EventHandler(this.cmbReferralFacility_SelectedIndexChanged);
            this.cmbReferralFacility.Validating += new System.ComponentModel.CancelEventHandler(this.cmbReferralFacility_Validating);
            // 
            // cmbReferralType
            // 
            this.cmbReferralType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReferralType.Location = new System.Drawing.Point(94, 73);
            this.cmbReferralType.Name = "cmbReferralType";
            this.cmbReferralType.Size = new System.Drawing.Size(158, 21);
            this.cmbReferralType.TabIndex = 3;
            this.cmbReferralType.SelectedIndexChanged += new System.EventHandler(this.cmbReferralType_SelectedIndexChanged);
            this.cmbReferralType.Validating += new System.ComponentModel.CancelEventHandler(this.cmbReferralType_Validating);
            // 
            // cmbModeOfArrival
            // 
            this.cmbModeOfArrival.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModeOfArrival.Location = new System.Drawing.Point(94, 48);
            this.cmbModeOfArrival.Name = "cmbModeOfArrival";
            this.cmbModeOfArrival.Size = new System.Drawing.Size(158, 21);
            this.cmbModeOfArrival.TabIndex = 2;
            this.cmbModeOfArrival.SelectedIndexChanged += new System.EventHandler(this.cmbModeOfArrival_SelectedIndexChanged);
            this.cmbModeOfArrival.Validating += new System.ComponentModel.CancelEventHandler(this.cmbModeOfArrival_Validating);
            // 
            // lblReAdmitCode
            // 
            this.lblReAdmitCode.Location = new System.Drawing.Point(9, 126);
            this.lblReAdmitCode.Name = "lblReAdmitCode";
            this.lblReAdmitCode.Size = new System.Drawing.Size(81, 16);
            this.lblReAdmitCode.TabIndex = 0;
            this.lblReAdmitCode.Text = "Re-admit code:";
            // 
            // lblReferralActivity
            // 
            this.lblReferralActivity.Location = new System.Drawing.Point(9, 101);
            this.lblReferralActivity.Name = "lblReferralActivity";
            this.lblReferralActivity.Size = new System.Drawing.Size(85, 16);
            this.lblReferralActivity.TabIndex = 0;
            this.lblReferralActivity.Text = "Referral facility:";
            // 
            // lblReferralType
            // 
            this.lblReferralType.Location = new System.Drawing.Point(9, 76);
            this.lblReferralType.Name = "lblReferralType";
            this.lblReferralType.Size = new System.Drawing.Size(72, 16);
            this.lblReferralType.TabIndex = 0;
            this.lblReferralType.Text = "Referral type:";
            // 
            // lblModeOfArrival
            // 
            this.lblModeOfArrival.Location = new System.Drawing.Point(9, 51);
            this.lblModeOfArrival.Name = "lblModeOfArrival";
            this.lblModeOfArrival.Size = new System.Drawing.Size(85, 16);
            this.lblModeOfArrival.TabIndex = 0;
            this.lblModeOfArrival.Text = "Mode of arrival:";
            // 
            // lblArrivalTime
            // 
            this.lblArrivalTime.Location = new System.Drawing.Point(9, 26);
            this.lblArrivalTime.Name = "lblArrivalTime";
            this.lblArrivalTime.Size = new System.Drawing.Size(65, 16);
            this.lblArrivalTime.TabIndex = 0;
            this.lblArrivalTime.Text = "Arrival time:";
            // 
            // mtbComplaint
            // 
            this.mtbComplaint.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbComplaint.Location = new System.Drawing.Point(3, 257);
            this.mtbComplaint.Mask = "";
            this.mtbComplaint.MaxLength = 74;
            this.mtbComplaint.Multiline = true;
            this.mtbComplaint.Name = "mtbComplaint";
            this.mtbComplaint.Size = new System.Drawing.Size(285, 48);
            this.mtbComplaint.TabIndex = 2;
            this.mtbComplaint.Validating += new System.ComponentModel.CancelEventHandler(this.mtbComplaint_Validating);
            // 
            // lblProcedure
            // 
            this.lblProcedure.Location = new System.Drawing.Point(3, 308);
            this.lblProcedure.Name = "lblProcedure";
            this.lblProcedure.Size = new System.Drawing.Size(75, 11);
            this.lblProcedure.TabIndex = 22;
            this.lblProcedure.Text = "Procedure :";
            // 
            // mtbProcedure
            // 
            this.mtbProcedure.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbProcedure.Location = new System.Drawing.Point(3, 322);
            this.mtbProcedure.Mask = "";
            this.mtbProcedure.MaxLength = 74;
            this.mtbProcedure.Multiline = true;
            this.mtbProcedure.Name = "mtbProcedure";
            this.mtbProcedure.Size = new System.Drawing.Size(285, 48);
            this.mtbProcedure.TabIndex = 3;
            this.mtbProcedure.Validating += new System.ComponentModel.CancelEventHandler(this.mtb_Procedure_Validating);
            // 
            // lblAlternateCareFacility
            // 
            this.lblAlternateCareFacility.Location = new System.Drawing.Point(361, 312);
            this.lblAlternateCareFacility.Name = "lblAlternateCareFacility";
            this.lblAlternateCareFacility.Size = new System.Drawing.Size(82, 27);
            this.lblAlternateCareFacility.TabIndex = 23;
            this.lblAlternateCareFacility.Text = "Nursing home/ Alt care facility";
            // 
            // cmbAlternateCareFacility
            // 
            this.cmbAlternateCareFacility.DisplayMember = "Description";
            this.cmbAlternateCareFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlternateCareFacility.Location = new System.Drawing.Point(452, 315);
            this.cmbAlternateCareFacility.MaxLength = 27;
            this.cmbAlternateCareFacility.Name = "cmbAlternateCareFacility";
            this.cmbAlternateCareFacility.Size = new System.Drawing.Size(192, 21);
            this.cmbAlternateCareFacility.TabIndex = 16;
            this.cmbAlternateCareFacility.SelectedIndexChanged += new System.EventHandler(this.cmbAlternateCareFacility_SelectedIndexChanged);
            this.cmbAlternateCareFacility.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAlternateCareFacility_Validating);
            // 
            // patientTypeHSVLocationView
            // 
            this.patientTypeHSVLocationView.Location = new System.Drawing.Point(3, 0);
            this.patientTypeHSVLocationView.Model = null;
            this.patientTypeHSVLocationView.Name = "patientTypeHSVLocationView";
            this.patientTypeHSVLocationView.Size = new System.Drawing.Size(343, 237);
            this.patientTypeHSVLocationView.TabIndex = 1;
            // 
            // cmbReferralSrc
            // 
            this.cmbReferralSrc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReferralSrc.Location = new System.Drawing.Point(452, 341);
            this.cmbReferralSrc.Name = "cmbReferralSrc";
            this.cmbReferralSrc.Size = new System.Drawing.Size(192, 21);
            this.cmbReferralSrc.TabIndex = 17;
            this.cmbReferralSrc.SelectedIndexChanged += new System.EventHandler(this.cmbReferralSrc_SelectedIndexChanged);
            this.cmbReferralSrc.Validating += new System.ComponentModel.CancelEventHandler(this.cmbReferralSrc_Validating);
            // 
            // cmbAdmitSrc
            // 
            this.cmbAdmitSrc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdmitSrc.Location = new System.Drawing.Point(452, 290);
            this.cmbAdmitSrc.Name = "cmbAdmitSrc";
            this.cmbAdmitSrc.Size = new System.Drawing.Size(192, 21);
            this.cmbAdmitSrc.TabIndex = 15;
            this.cmbAdmitSrc.SelectedIndexChanged += new System.EventHandler(this.cmbAdmitSrc_SelectedIndexChanged);
            this.cmbAdmitSrc.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAdmitSrc_Validating);
            // 
            // cmbTenetCare
            // 
            this.cmbTenetCare.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTenetCare.Location = new System.Drawing.Point(452, 265);
            this.cmbTenetCare.Name = "cmbTenetCare";
            this.cmbTenetCare.Size = new System.Drawing.Size(54, 21);
            this.cmbTenetCare.TabIndex = 14;
            this.cmbTenetCare.SelectedIndexChanged += new System.EventHandler(this.cmbTenetCare_SelectedIndexChanged_1);
            this.cmbTenetCare.Validating += new System.ComponentModel.CancelEventHandler(this.cmbTenetCare_Validating);
            // 
            // cmbAccidentType
            // 
            this.cmbAccidentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccidentType.Location = new System.Drawing.Point(478, 30);
            this.cmbAccidentType.Name = "cmbAccidentType";
            this.cmbAccidentType.Size = new System.Drawing.Size(139, 21);
            this.cmbAccidentType.TabIndex = 5;
            this.cmbAccidentType.SelectedIndexChanged += new System.EventHandler(this.cmbAccidentType_SelectedIndexChanged);
            // 
            // rbNone
            // 
            this.rbNone.Location = new System.Drawing.Point(445, 58);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(116, 15);
            this.rbNone.TabIndex = 8;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "None of the above";
            this.rbNone.Click += new System.EventHandler(this.rbNone_Click);
            this.rbNone.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbNone_KeyDown);
            // 
            // rbCrime
            // 
            this.rbCrime.Location = new System.Drawing.Point(369, 58);
            this.rbCrime.Name = "rbCrime";
            this.rbCrime.Size = new System.Drawing.Size(67, 15);
            this.rbCrime.TabIndex = 7;
            this.rbCrime.TabStop = true;
            this.rbCrime.Text = "Crime";
            this.rbCrime.Click += new System.EventHandler(this.rbCrime_Click);
            this.rbCrime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbCrime_KeyDown);
            // 
            // rbAccident
            // 
            this.rbAccident.Location = new System.Drawing.Point(369, 33);
            this.rbAccident.Name = "rbAccident";
            this.rbAccident.Size = new System.Drawing.Size(66, 15);
            this.rbAccident.TabIndex = 6;
            this.rbAccident.TabStop = true;
            this.rbAccident.Text = "Accident";
            this.rbAccident.Click += new System.EventHandler(this.rbAccident_Click);
            this.rbAccident.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbAccident_KeyDown);
            // 
            // cptCodesView
            // 
            this.cptCodesView.Location = new System.Drawing.Point(675, 351);
            this.cptCodesView.Model = null;
            this.cptCodesView.Name = "cptCodesView";
            this.cptCodesView.Size = new System.Drawing.Size(261, 25);
            this.cptCodesView.TabIndex = 20;
            // 
            // serviceCategory1
            // 
            this.serviceCategory1.BackColor = System.Drawing.Color.White;
            this.serviceCategory1.Location = new System.Drawing.Point(681, 163);
            this.serviceCategory1.Model = null;
            this.serviceCategory1.Name = "serviceCategory1";
            this.serviceCategory1.ServiceCategoryPresenter = null;
            this.serviceCategory1.Size = new System.Drawing.Size(242, 27);
            this.serviceCategory1.TabIndex = 24;
            this.serviceCategory1.ServiceCategorySelected += new System.EventHandler(this.serviceCategory1_ServiceCategorySelected);
            this.serviceCategory1.ServiceCategoryValidating += new System.ComponentModel.CancelEventHandler(this.serviceCategory1_ServiceCategoryValidating);
            // 
            // DiagnosisView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.serviceCategory1);
            this.Controls.Add(this.cmbAlternateCareFacility);
            this.Controls.Add(this.lblAlternateCareFacility);
            this.Controls.Add(this.mtbProcedure);
            this.Controls.Add(this.lblProcedure);
            this.Controls.Add(this.patientTypeHSVLocationView);
            this.Controls.Add(this.mtbComplaint);
            this.Controls.Add(this.gbxEDLog);
            this.Controls.Add(this.cmbReferralSrc);
            this.Controls.Add(this.cmbAdmitSrc);
            this.Controls.Add(this.cmbTenetCare);
            this.Controls.Add(this.lblReferral);
            this.Controls.Add(this.lblAdmit);
            this.Controls.Add(this.lblTenetCare);
            this.Controls.Add(this.mtbOnsetDate);
            this.Controls.Add(this.cmbAccidentType);
            this.Controls.Add(this.dateTimePickerSickness);
            this.Controls.Add(this.gbxDetails);
            this.Controls.Add(this.rbNone);
            this.Controls.Add(this.rbCrime);
            this.Controls.Add(this.rbAccident);
            this.Controls.Add(this.lblVisitResult);
            this.Controls.Add(this.lblComplaint);
            this.Controls.Add(this.lblOnset);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.gbxClinics);
            this.Controls.Add(this.cptCodesView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DiagnosisView";
            this.Size = new System.Drawing.Size(983, 425);
            this.Load += new System.EventHandler(this.DiagnosisView_Load);
            this.Enter += new System.EventHandler(this.DiagnosisView_Enter);
            this.Leave += new System.EventHandler(this.DiagnosisView_Leave);
            this.Disposed += new System.EventHandler(this.DiagnosisView_Disposed);
            this.gbxDetails.ResumeLayout(false);
            this.gbxDetails.PerformLayout();
            this.gbxClinics.ResumeLayout(false);
            this.gbxEDLog.ResumeLayout(false);
            this.gbxEDLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Construction and Finalization
        public DiagnosisView()
        {
            // This call is required by the Windows.Forms Form Designer.
            this.InitializeComponent();
            ConfigureControls();

            this.PopulateConditionMap();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        //added        
        private Container components = null;

        private ArrayList i_AllClinics = new ArrayList();
        private ReferralSource selectedReferralSrc;
        private ReferralType selectedReferralType;
        private ReferralFacility selectedReferralFacility;
        private ReAdmitCode selectedReAdmitCode;
        private Facility facility;
        internal TypeOfAccident accidentType;

        private PatientAccessComboBox cmbCountry;
        private PatientAccessComboBox cmbState;
        private PatientAccessComboBox cmbHour;
        private PatientAccessComboBox cmbAccidentType;
        private PatientAccessComboBox cmbTenetCare;
        private PatientAccessComboBox cmbAdmitSrc;
        private PatientAccessComboBox cmbReferralSrc;
        private PatientAccessComboBox cmbReadmitCode;
        private PatientAccessComboBox cmbReferralType;
        private PatientAccessComboBox cmbModeOfArrival;
        private PatientAccessComboBox cmbReferralFacility;

        private ClinicView clinicView1;
        private ClinicView clinicView2;
        private ClinicView clinicView3;
        private ClinicView clinicView4;
        private ClinicView clinicView5;

        private ReferenceValueComboBox i_CountryComboHelper;
        private ReferenceValueComboBox i_StateComboHelper;

        private RadioButtonKeyHandler rbCrime;
        private RadioButtonKeyHandler rbAccident;
        private RadioButtonKeyHandler rbNone;

        private MaskedEditTextBox mtbOnsetDate;
        private MaskedEditTextBox mtbAccidentCrimeDate;
        private MaskedEditTextBox mtbArrivalTime;

        private readonly ComboBox currentCombo = new ComboBox();

        private DateTimePicker dateTimePickerAccident;
        private DateTimePicker dateTimePickerSickness;

        private GroupBox gbxDetails;
        private GroupBox gbxClinics;
        private GroupBox gbxEDLog;

        private Label lblComplaint;
        private Label lblVisitResult;
        private Label lblDate;
        private Label lblHour;
        private Label lblCountry;
        private Label lblState;
        private Label lblOnset;
        private Label lblType;
        private Label lblTenetCare;
        private Label lblAdmit;
        private Label lblReferral;
        private Label lblArrivalTime;
        private Label lblModeOfArrival;
        private Label lblReferralType;
        private Label lblReferralActivity;
        private Label lblReAdmitCode;

        private readonly Hashtable combos = new Hashtable();
        private readonly Hashtable i_ConditionMap = new Hashtable();

        private bool i_Registered = false;
        private bool blnLeaveRun = false;
        private int verifyMonth;
        private int verifyDay;
        private int verifyYear;
        private bool loadingModelData = true;

        internal string onsetDateTxt = String.Empty;
        internal string lastMenstrualDateTxt = String.Empty;
        internal string accidentDateTxt = String.Empty;

        string i_NursingStationCode;

        private ArrayList i_PatientTypes;

        private MaskedEditTextBox mtbComplaint;
        private PatientTypeHSVLocationView patientTypeHSVLocationView;
        private IEDLogsDisplayPresenter edLogDisplayPresenter;
        private IDiagnosisViewPresenter diagnosisViewPresenter;
        private Label lblProcedure;
        private MaskedEditTextBox mtbProcedure;
        private PatientAccessComboBox cmbAlternateCareFacility;
        private Label lblAlternateCareFacility;
        private readonly IRuleEngine ruleEngine = Rules.RuleEngine.GetInstance();
        private IAlternateCareFacilityPresenter alternateCareFacilityPresenter;
        private CommonControls.ServiceCategory.ViewImpl.ServiceCategory serviceCategory1;
        private CptCodesView cptCodesView;

        #endregion

        #region Constants
        #endregion

        private void serviceCategory1_ServiceCategorySelected(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            ClinicServiceCategory selectedServiceCategory = args.Context as ClinicServiceCategory;
            if(selectedServiceCategory !=null)
             this.Model.EmbosserCard = selectedServiceCategory.Code;

            DOFRInitiatePresenter.SetDOFRInitiated(this.Model);
        }

        private void serviceCategory1_ServiceCategoryValidating(object sender, CancelEventArgs e)
        {

        }
        private void SetDOFRInitiatedForPTChange(object sender, VisitTypeEventArgs e)
        {
            DOFRInitiatePresenter.SetDOFRInitiated(this.Model);
        }
        private void SetDOFRInitiatedForHSVChange(object sender, EventArgs e)
        {
            DOFRInitiatePresenter.SetDOFRInitiated(this.Model);
        }
    }
}
