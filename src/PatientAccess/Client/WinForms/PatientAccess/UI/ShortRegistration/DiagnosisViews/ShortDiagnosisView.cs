using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.ClinicalViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.ServiceCategory.Presenter;
using PatientAccess.UI.CptCodes.Presenters;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews.DOFR.Presenter;

namespace PatientAccess.UI.ShortRegistration.DiagnosisViews
{
    /// <summary>
    /// Summary description for ShortDiagnosisView.
    /// </summary>
    [Serializable]
    public class ShortDiagnosisView : ControlView, IShortDiagnosisView, IClinicalTrialsView
    {
        #region Delegates
        private delegate void DisplayAbstractCondition();
        #endregion

        #region Events
        public event EventHandler EnableInsuranceTab;
 
        #endregion

        #region Event Handlers

        void DiagnosisView_Load( object sender, EventArgs e )
        {
            patientTypeHSVLocationView.ClearSelectedClinics += patientTypeHSVLocationView_ClearSelectedClinics;
            patientTypeHSVLocationView.SelectPreviouslyStoredClinicValue += patientTypeHSVLocationView_SelectPreviouslyStoredClinicValue;
            patientTypeHSVLocationView.PatientTypeChanged += EvaluateMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage;
            patientTypeHSVLocationView.PatientTypeChanged += EnableOrDisableProcedureField;
            patientTypeHSVLocationView.PatientTypeChanged += SetDOFRInitiatedForPTChange;
            patientTypeHSVLocationView.HSVChanged += SetDOFRInitiatedForHSVChange;
        }

        private void EvaluateMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage( object sender, VisitTypeEventArgs e )
        {
            if ( !loadingModelData )
            {
                RuleEngine.OneShotRuleEvaluation<MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage>(
                    Model, MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler );
            }
        }

        private void MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler( object sender, EventArgs e )
        {
            var messageDisplayHandler = new ErrorMessageDisplayHandler( Model );
            messageDisplayHandler.DisplayOkWarningMessageFor( typeof( MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage ) );
        }

        private void DiagnosisView_Leave( object sender, EventArgs e )
        {
            blnLeaveRun = true;
            RuleEngine.EvaluateRule( typeof( OnDiagnosisForm ), Model );
            blnLeaveRun = false;
        }

        /// <summary>
        /// Enables the or disable procedure field.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PatientAccess.UI.VisitTypeEventArgs"/> instance containing the event data.</param>
        private void EnableOrDisableProcedureField( object sender, VisitTypeEventArgs e )
        {
            ShortDiagnosisViewPresenter.HandleProcedureField( e.VisitType, true );
        }

        private void NoPrimaryMedicareForAutoAccidentEventHandler( object sender, EventArgs e )
        {
            IErrorMessageDisplayHandler messageDisplayHandler = new ErrorMessageDisplayHandler( Model );
            DialogResult warningResult =
            messageDisplayHandler.DisplayYesNoErrorMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );
            if ( warningResult == DialogResult.No )
            {
                return;
            }
            if ( EnableInsuranceTab != null )
            {
                EnableInsuranceTab( this, new LooseArgs( Model ) );
            }
        }

        private void patientTypeHSVLocationView_ClearSelectedClinics( object sender, EventArgs e )
        {
            Model.KindOfVisit = patientTypeHSVLocationView.Model.KindOfVisit;
            ClearSelectedClinics( Model.KindOfVisit );
        }

        private void patientTypeHSVLocationView_SelectPreviouslyStoredClinicValue( object sender, EventArgs e )
        {
            SelectPreviouslyStoredClinicValue();
        }

        private void DiagnosisView_Enter( object sender, EventArgs e )
        {
            IAccountView accountView = AccountView.GetInstance();

            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if ( accountView.IsMedicareAdvisedForPatient() )
            {
                accountView.MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                    UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

                if ( warningResult == DialogResult.Yes )
                {
                    if ( EnableInsuranceTab != null )
                    {
                        EnableInsuranceTab( this, new LooseArgs( Model ) );
                    }
                }
            }
            RegisterEvents();
        }

        private void mtbComplaint_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbComplaint );
            Model.Diagnosis.ChiefComplaint = mtbComplaint.Text;

            RuleEngine.EvaluateRule( typeof( ChiefComplaintRequired ), Model );
        }

        private void mtbAccidentCrimeDate_Validating( object sender, CancelEventArgs e )
        {
            if ( mtbAccidentCrimeDate.UnMaskedText == String.Empty )
            {
                //set OccurredOn datetime value to min datetime value.
                if ( Model.Diagnosis.Condition.GetType() == typeof( Accident ) ||
                    Model.Diagnosis.Condition.GetType() == typeof( Crime ) )
                {
                    TimeAndLocationBoundCondition condition = ( TimeAndLocationBoundCondition )Model.Diagnosis.Condition;
                    condition.OccurredOn = DateTime.MinValue;
                }
            }

            if ( dateTimePickerAccident.Focused == false )
            {
                if ( VerifyAccidentCrimeDate() )
                {
                    CheckForRequiredAccidentFields();
                }

                bool accidentCrimeDateRulePassed = ApplyAdmitDateReqdWithAccidentCrimeDateRule();
                if ( accidentCrimeDateRulePassed == false )
                {
                    SetAccidentCrimeDateErrBgColor();
                }
            }
        }

        private void mtbOnsetDate_Validating( object sender, CancelEventArgs e )
        {
            bool illnessOnSetRulePassed = true;
            if ( dateTimePickerSickness.Focused == false )
            {
                if ( VerifyOnsetDate() )
                {
                    UIColors.SetNormalBgColor( mtbOnsetDate );
                    Refresh();
                    CheckForRequiredAccidentFields();
               
                }
                illnessOnSetRulePassed = ApplyAdmitDateReqdWithIllnessOnSetDateRule();
            }
            if ( illnessOnSetRulePassed == false )
            {
                SetOnsetDateErrBgColor();
            }
        }

        /// <summary>
        /// private handler for rule: AccidentOrCrimeDateWithNoAdmitDate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccidentOrCrimeDateWithNoAdmitDateEventHandler( object sender, EventArgs e )
        {
            GetFocusAndHighlightRedError( mtbAccidentCrimeDate );
            SetAccidentCrimeDateErrBgColor();
            ShowErrorMessageBox( UIErrorMessages.OCCURRENCE_CODE_DATE_FUTURE_ERRMSG );
            EnableAccidentCrimeDetails();
            CheckForRequiredAccidentFields();
        }

        /// <summary>
        /// private handler for rule: OnsetOfSymptomsOrIllnessWithNoAdmitDate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnsetOfSymptomsOrIllnessWithNoAdmitDateEventHandler( object sender, EventArgs e )
        {
            GetFocusAndHighlightRedError( mtbOnsetDate );
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
            CheckForRequiredAccidentFields();
        }

        private void cmbCountry_Validating( object sender, CancelEventArgs e )
        {
            CheckForRequiredAccidentFields();
        }

        private void cmbState_Validating( object sender, CancelEventArgs e )
        {
            CheckForRequiredAccidentFields();
        }

        private void cmbAdmitSrc_SelectedIndexChanged( object sender, EventArgs e )
        {
            HandleAdmitSourceSelectedIndexChanged( cmbAdmitSrc.SelectedItem as AdmitSource );
        }

        private void HandleAdmitSourceSelectedIndexChanged( AdmitSource newAdmitSource )
        {
            if ( newAdmitSource != null )
            {
                Model.AdmitSource = newAdmitSource;
            }

            RuleEngine.EvaluateRule( typeof( AdmitSourceRequired ), Model ); 
        }

        private void clinicView1_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            Model.Clinics[0] = selectedHospitalClinic;
            HandleClinicIndexChange( clinicView1.cmbClinic );
            RuleEngine.EvaluateRule( typeof( DiagnosisClinicOneRequired ), Model );
            ServiceCategoryPresenter.UpdateView();
            DOFRInitiatePresenter.SetDOFRInitiated(this.Model);
        }

        private void clinicView2_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            Model.Clinics[1] = selectedHospitalClinic;
            HandleClinicIndexChange( clinicView2.cmbClinic );
        }

        private void clinicView3_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            Model.Clinics[2] = selectedHospitalClinic;
            HandleClinicIndexChange( clinicView3.cmbClinic );
        }

        private void clinicView4_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            Model.Clinics[3] = selectedHospitalClinic;
            HandleClinicIndexChange( clinicView4.cmbClinic );
        }

        private void clinicView5_HospitalClinicSelected( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            HospitalClinic selectedHospitalClinic = args.Context as HospitalClinic;
            Model.Clinics[4] = selectedHospitalClinic;
            HandleClinicIndexChange( clinicView5.cmbClinic );
        }
        /// <summary>
        /// Click on Radio Button Accident
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbAccident_Click( object sender, EventArgs e )
        {
            if ( Model.Diagnosis.Condition == null || Model.Diagnosis.Condition.GetType() != typeof( Accident ) )
            {
                Accident accident = new Accident();
                Model.Diagnosis.Condition = accident;
                cmbState.SelectedIndex = 0;
            }

            ClearOccurrenceCodes();
            DisplayAccident();
            CheckForRequiredAccidentFields();
        }

        /// <summary>
        /// Click on Radio Button Crime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbCrime_Click( object sender, EventArgs e )
        {
            cmbAccidentType.Enabled = false;
            CheckPreviousCondition();

            if ( Model.Diagnosis.Condition == null || Model.Diagnosis.Condition.GetType() != typeof( Crime ) )
            {
                Crime crime = new Crime();
                Model.Diagnosis.Condition = crime;
                cmbState.SelectedIndex = 0;
            }

            ClearOccurrenceCodes();
            DisplayCrime();
            CheckForRequiredAccidentFields();
        }

        /// <summary>
        /// Click on Radio Button None
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNone_Click( object sender, EventArgs e )
        {
            bool onSetEnabled = true;
            CheckPreviousCondition();

            if ( Model.Diagnosis.Condition == null || Model.Diagnosis.Condition.GetType() != typeof( UnknownCondition ) )
            {
                UnknownCondition unKnownCondition = new UnknownCondition { Onset = DateTime.MinValue };
                Model.Diagnosis.Condition = unKnownCondition;
            }
            VerifyOnsetDate();
            ClearOccurrenceCodes();
            DisplayCondition();

            // TLG 04/21/2006 - not necessary
            //LocalUpdateView();

            CheckForRequiredAccidentFields();

            if ( typeof( AdmitNewbornActivity ).Equals( Model.Activity.GetType() ) )
            {
                onSetEnabled = false;
                UIColors.SetDisabledDarkBgColor( mtbOnsetDate );
            }

            mtbOnsetDate.Enabled = onSetEnabled;
            dateTimePickerSickness.Enabled = onSetEnabled;
        }

        private void rbAccident_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyData == Keys.Right || e.KeyData == Keys.Down )
            {
                rbCrime.Focus();
            }
            else if ( e.KeyData == Keys.Left || e.KeyData == Keys.Up )
            {
                rbNone.Focus();
            }
        }

        private void rbCrime_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyData == Keys.Left || e.KeyData == Keys.Up )
            {
                rbAccident.Focus();
            }
            else if ( e.KeyData == Keys.Right || e.KeyData == Keys.Down )
            {
                rbNone.Focus();
            }
        }

        private void rbNone_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyData == Keys.Right || e.KeyData == Keys.Down )
            {
                rbAccident.Focus();
            }
            else if ( e.KeyData == Keys.Left || e.KeyData == Keys.Up )
            {
                rbCrime.Focus();
            }
        }

        /// <summary>
        /// Private helper method.
        /// </summary>
        private bool ApplyAdmitDateReqdWithAccidentCrimeDateRule()
        {
            RuleEngine.RegisterEvent( typeof( AccidentOrCrimeDateWithNoAdmitDate ),
                new EventHandler( AccidentOrCrimeDateWithNoAdmitDateEventHandler ) );

            bool success = RuleEngine.EvaluateRule( typeof( AccidentOrCrimeDateWithNoAdmitDate ), Model );

            RuleEngine.UnregisterEvent( typeof( AccidentOrCrimeDateWithNoAdmitDate ),
                new EventHandler( AccidentOrCrimeDateWithNoAdmitDateEventHandler ) );
            return success;
        }

        /// <summary>
        /// Private helper method.
        /// </summary>
        private bool ApplyAdmitDateReqdWithIllnessOnSetDateRule()
        {
            RuleEngine.RegisterEvent( typeof( OnsetOfSymptomsOrIllnessWithNoAdmitDate ),
                new EventHandler( OnsetOfSymptomsOrIllnessWithNoAdmitDateEventHandler ) );

            bool success = RuleEngine.EvaluateRule( typeof( OnsetOfSymptomsOrIllnessWithNoAdmitDate ), Model );

            RuleEngine.UnregisterEvent( typeof( OnsetOfSymptomsOrIllnessWithNoAdmitDate ),
                new EventHandler( OnsetOfSymptomsOrIllnessWithNoAdmitDateEventHandler ) );
            return success;
        }

        /// <summary>
        /// Date Time Picker Accident CloseUp
        /// </summary>
        private void dateTimePickerAccident_CloseUp( object sender, EventArgs e )
        {
            SetAccidentCrimeDateNormalBgColor();
            SetAccidentCrimeDate();

            if ( dateTimePickerAccident.Checked )
            {
                DateTime dt = dateTimePickerAccident.Value;
                mtbAccidentCrimeDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
                accidentDateTxt = mtbAccidentCrimeDate.Text;
                VerifyAccidentCrimeDate();
            }
            else
            {
                mtbAccidentCrimeDate.Text = String.Empty;
                accidentDateTxt = String.Empty;
            }

            bool wasAccidentCrimeDateVerified = ApplyAdmitDateReqdWithAccidentCrimeDateRule();
            if ( wasAccidentCrimeDateVerified == false )
            {
                SetAccidentCrimeDateErrBgColor();
            }
            else
            {
                CheckForRequiredAccidentFields();
            }
        }

        /// <summary>
        /// Date Time Picker Close Up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePickerSickness_CloseUp( object sender, EventArgs e )
        {
            SetOnsetDateNormalBgColor();
            if ( dateTimePickerSickness.Checked )
            {
                DateTime dt = dateTimePickerSickness.Value;
                mtbOnsetDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
                onsetDateTxt = mtbOnsetDate.Text;
                VerifyOnsetDate();
            }
            else
            {
                mtbOnsetDate.Text = String.Empty;
                onsetDateTxt = String.Empty;
            }

            bool wasOnSetDateVerified = ApplyAdmitDateReqdWithIllnessOnSetDateRule();
            if ( wasOnSetDateVerified == false )
            {
                SetOnsetDateErrBgColor();
            }
        }

        /// <summary>
        /// Combo Box Accident Type Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbAccidentType_SelectedIndexChanged( object sender, EventArgs e )
        {
            Accident accident = ( Accident )Model.Diagnosis.Condition;
            if ( cmbAccidentType.SelectedItem != null &&
                 cmbAccidentType.SelectedItem.ToString() != String.Empty )
            {
                TypeOfAccident typeOfAccident = ( ( DictionaryEntry )cmbAccidentType.SelectedItem ).Value as TypeOfAccident;
                accident.Kind = typeOfAccident;
                accidentType = typeOfAccident;

                if ( typeOfAccident != null )
                {
                    if ( typeOfAccident.Oid == TypeOfAccident.EMPLOYMENT_RELATED ||
                        typeOfAccident.Description == "Employment Related" )
                    {
                        IsValidAccidentTypeForInsurance();
                    }
                    if ( !loadingModelData )
                    {
                        EvaluateNoMedicarePrimaryPayorForAutoAccidentRule( NoPrimaryMedicareForAutoAccidentEventHandler );
                    }
                }
            }
            else
            {
                accident.Kind = null;
            }

            Model.Diagnosis.Condition = accident;

            CheckForRequiredAccidentFields();
        }

        private bool EvaluateNoMedicarePrimaryPayorForAutoAccidentRule( EventHandler eventHandler )
        {
            return RuleEngine.OneShotRuleEvaluation<NoMedicarePrimaryPayorForAutoAccident>( Model, eventHandler );
        }

        /// <summary>
        /// Evaluates the procedure required rule.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns></returns>
        public bool EvaluateProcedureRequiredRule( EventHandler eventHandler )
        {
            return RuleEngine.OneShotRuleEvaluation<ProcedureRequired>( Model, eventHandler );
        }

        /// <summary>
        /// Combo Box Hour Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbHour_SelectedIndexChanged( object sender, EventArgs e )
        {
            SetAccidentCrimeHour();
        }

        /// <summary>
        /// Combo Box Country Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCountry_SelectedIndexChanged( object sender, EventArgs e )
        {
            SetAccidentCrimeCountry( ( Country )cmbCountry.SelectedItem );
        }

        /// <summary>
        /// Combo Box State Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbState_SelectedIndexChanged( object sender, EventArgs e )
        {
            //State state = ((DictionaryEntry)cmbState.SelectedItem).Value as State;
            if ( cmbState.SelectedIndex >= 0 )
            {
                State state = ( State )cmbState.SelectedItem;
                if ( state != null && ( rbAccident.Checked || rbCrime.Checked ) )
                {
                    SetAccidentCrimeState( state );
                }
            }
        }

        private void btnViewClinicalTrialsDetails_Click( object sender, EventArgs e )
        {
            ClinicalTrialsPresenter.ShowDetails();
        }

        private void cboPatientInClinicalResearch_SelectedIndexChanged( object sender, EventArgs e )
        {
            ClinicalTrialsPresenter.EvaluateClinicalResearchFieldRules();
        }

        private void cboPatientInClinicalResearch_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( cboPatientInClinicalResearch );

            ClinicalTrialsPresenter.EvaluateClinicalResearchFieldRules();
        }

        void cboPatientInClinicalResearch_SelectionChangeCommitted( object sender, EventArgs e )
        {
            // This variable is set to avoid issues caused by using a mouse to get focus of the combo box 
            // and then tabbing out of the control to make a selection, or using an arrow key and pressing
            // enter to make a selection. The SelectionChangeCommitted event does not fire if the user tabs
            // in and out. This is a known defect with the combo box control. The DropDownClosed event
            // handler and boolean variable are used as a work-around under the above defined scenarios.
            // References:
            // http://social.msdn.microsoft.com/Forums/en-US/winformsdatacontrols/thread/ad430abc-5ebc-4309-bc43-4e0b2fa8f327
            // https://connect.microsoft.com/VisualStudio/feedback/details/95320/dropdownlist-event-selectionchangecommitted-broken

            userChangedIsPatientInClinicalResearchStudy = true;

            if ( cboPatientInClinicalResearch.SelectedItem != null )
            {
                ClinicalTrialsPresenter.UserChangedPatientInClinicalTrialsTo(
                    ( ( YesNoFlag )cboPatientInClinicalResearch.SelectedItem ) );
            }
        }

        void cboPatientInClinicalResearch_DropDownClosed( object sender, EventArgs e )
        {
            if ( !userChangedIsPatientInClinicalResearchStudy )
            {
                if ( cboPatientInClinicalResearch.SelectedItem != null )
                {
                    ClinicalTrialsPresenter.UserChangedPatientInClinicalTrialsTo(
                        ( ( YesNoFlag )cboPatientInClinicalResearch.SelectedItem ) );

                    UIColors.SetNormalBgColor( cboPatientInClinicalResearch );

                    ClinicalTrialsPresenter.EvaluateClinicalResearchFieldRules();
                }
            }
            userChangedIsPatientInClinicalResearchStudy = false;
        }

        private void DiagnosisView_Disposed( object sender, EventArgs e )
        {
            UnregisterEvents();
        }

        private void ChiefComplaintRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbComplaint );
        }

        /// <summary>
        /// Procedures the required event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ProcedureRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbProcedure );
        }

        private void AccidentTypeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbAccidentType );
        }

        private void DateOfAccidentOrCrimeRequiredEventHandler( object sender, EventArgs e )
        {
            if ( mtbAccidentCrimeDate.Enabled )
            {
                UIColors.SetRequiredBgColor( mtbAccidentCrimeDate );
            }
        }

        private void HourOfAccidentOrCrimeRequiredEventHandler( object sender, EventArgs e )
        {
            if ( cmbHour.Enabled )
            {
                UIColors.SetRequiredBgColor( cmbHour );
            }
        }
 

        private void CountryOfAccidentOrCrimeRequiredEventHandler( object sender, EventArgs e )
        {
            if ( cmbCountry.Enabled )
            {
                UIColors.SetRequiredBgColor( cmbCountry );
            }
        }

       
        private void StateOfAccidentOrCrimeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbState );
        }

      

        private void OnsetDateOfSymptomsOrIllnessRequiredEventHandler( object sender, EventArgs e )
        {
            mtbOnsetDate.Enabled = true;
            UIColors.SetRequiredBgColor( mtbOnsetDate );
        }

        private void DiagnosisClinicOneRequiredEventHandler( object sender, EventArgs e )
        {
            clinicView1.cmbClinic.Enabled = true;
            UIColors.SetRequiredBgColor( clinicView1.cmbClinic );
        }

        private void AdmitSourceRequiredEventHandler( object sender, EventArgs e )
        {
            cmbAdmitSrc.Enabled = true;
            UIColors.SetRequiredBgColor( cmbAdmitSrc );
        }

      

        private void PatientInClinicalResearchStudyRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cboPatientInClinicalResearch );
        }

        private void PatientInClinicalResearchStudyPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cboPatientInClinicalResearch );
        }

        //---------------------Evaluate ComboBoxes -----------------------------------
        private void cmbAdmitSrc_Validating( object sender, CancelEventArgs e )
        {
            HandleAdmitSourceSelectedIndexChanged( cmbAdmitSrc.SelectedItem as AdmitSource );
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( cmbAdmitSrc );
                Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidAdmitSourceCode ), Model );
                RuleEngine.EvaluateRule( typeof( InvalidAdmitSourceCodeChange ), Model );
            }
            RuleEngine.EvaluateRule( typeof( AdmitSourceRequired ), Model );
            
        }

        private void clinicView1_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( clinicView1.cmbClinic );
                Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_1Code ), Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_1CodeChange ), Model );
                RuleEngine.EvaluateRule( typeof( DiagnosisClinicOneRequired ), Model );
            }
        }
        private void clinicView2_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( clinicView2.cmbClinic );
                Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_2Code ), Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_2CodeChange ), Model );
            }
        }
        private void clinicView3_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( clinicView3.cmbClinic );
                Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_3Code ), Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_3CodeChange ), Model );
            }
        }
        private void clinicView4_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( clinicView4.cmbClinic );
                Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_4Code ), Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_4CodeChange ), Model );
            }
        }
        private void clinicView5_HospitalClinicValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( clinicView5.cmbClinic );
                Refresh();
                RuleEngine.EvaluateRule( typeof( InvalidClinic_5Code ), Model );
                RuleEngine.EvaluateRule( typeof( InvalidClinic_5CodeChange ), Model );
            }
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if ( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }

        private void InvalidAdmitSourceCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbAdmitSrc );
        }
        private void InvalidClinic1CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( clinicView1.cmbClinic );
        }
        private void InvalidClinic2CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( clinicView2.cmbClinic );
        }
        private void InvalidClinic3CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( clinicView3.cmbClinic );
        }
        private void InvalidClinic4CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( clinicView4.cmbClinic );
        }
        private void InvalidClinic5CodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( clinicView5.cmbClinic );
        }

        //----------------------------------------------

        private void InvalidAdmitSourceCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbAdmitSrc );
        }
        private void InvalidClinic2CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( clinicView2.cmbClinic );
        }
        private void InvalidClinic3CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( clinicView3.cmbClinic );
        }
        private void InvalidClinic4CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( clinicView4.cmbClinic );
        }
        private void InvalidClinic5CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( clinicView5.cmbClinic );
        }
        private void InvalidClinic1CodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( clinicView1.cmbClinic );
        }
        /// <summary>
        /// Handles the Validating event of the mtb_Procedure control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void mtb_Procedure_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbProcedure );
            ShortDiagnosisViewPresenter.UpdateProcedureField( mtbProcedure.Text );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disable Condition Dependencies
        /// </summary>
        private void DisableConditionDependencies()
        {
            cmbAccidentType.Enabled = false;
            mtbAccidentCrimeDate.Enabled = false;
            dateTimePickerAccident.Enabled = false;
            cmbHour.Enabled = false;
            cmbCountry.Enabled = false;
            cmbState.Enabled = false;
            UIColors.SetDisabledDarkBgColor( cmbAccidentType );
            UIColors.SetDisabledDarkBgColor( mtbAccidentCrimeDate );
            UIColors.SetDisabledDarkBgColor( cmbHour );
            UIColors.SetDisabledDarkBgColor( cmbCountry );
            UIColors.SetDisabledDarkBgColor( cmbState );
        }

        /// <summary>
        /// Display Condition
        /// </summary>
        private void DisplayCondition()
        {
            DisableConditionDependencies();
            DisplayAbstractCondition displayMethodPointer = DisplayMethodForCurrentCondition();
            displayMethodPointer();
        }

        /// <summary>
        /// Retrieves the correct delegate to use based on the current Model's Type.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Cannot retrieve the delegate for displaying a Condition when Diagnosis is null</exception>
        private DisplayAbstractCondition DisplayMethodForCurrentCondition()
        {
            if ( Model.Diagnosis == null || Model.Diagnosis.Condition == null )
            {
                throw new InvalidOperationException( "Cannot retrieve the delegate for displaying a Condition when Diagnosis is null" );
            }
            return ( DisplayAbstractCondition )ConditionMap[Model.Diagnosis.Condition.GetType()];
        }

        /// <summary>
        /// Re-displays the view when the Model is changed.
        /// </summary>
        public override void UpdateView()
        {
            ShortDiagnosisViewPresenter = new ShortDiagnosisViewPresenter( this, Model.Activity );
            ClinicalTrialsPresenter = new ClinicalTrialsPresenter( this, ClinicalTrialsDetailsView, new ClinicalTrialsFeatureManager( ConfigurationManager.AppSettings ), Model, User.GetCurrent().Facility.Oid, BrokerFactory.BrokerOfType<IResearchStudyBroker>() );
            CptCodesPresenter = new CptCodesPresenter(cptCodesView, Model, new CptCodesFeatureManager(), new MessageBoxAdapter());
            RegisterRequiredAndPreferredRules();
            physicianSelectionView1.Model = Model;
            physicianSelectionView1.UpdateView();
            
            try
            {
                if ( Model.Activity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity )
                    && Model.KindOfVisit != null
                    && Model.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                {
                    Model.KindOfVisit = ( VisitType )PatientTypes[0];
                }

                Cursor = Cursors.AppStarting;
                StateComboHelper = new ReferenceValueComboBox( cmbState );

                facility = User.GetCurrent().Facility;

                clinicView1.cmbClinic.Name = "cmbClinic1";
                clinicView2.cmbClinic.Name = "cmbClinic2";
                clinicView3.cmbClinic.Name = "cmbClinic3";
                clinicView4.cmbClinic.Name = "cmbClinic4";
                clinicView5.cmbClinic.Name = "cmbClinic5";

                LoadClinicCombos();

                PopulateAdmitSource();

                if ( clinicView1.cmbClinic.Items.Count < 1 )
                {
                    PopulateClinics();
                    //Load Service Category Dropdown 
                    ServiceCategoryPresenter.UpdateView();
                }

                ShortDiagnosisViewPresenter.HandleProcedureField( Model.KindOfVisit, true );
                ClinicalTrialsPresenter.HandleClinicalResearchFields( Model.AdmitDate );

                patientTypeHSVLocationView.Model = Model;
                patientTypeHSVLocationView.UpdateView();
                mtbComments.Text = Model.ClinicalComments;

                CptCodesPresenter.UpdateView();

                if ( loadingModelData )
                {
                    RegisterEvents();
                }

                LocalUpdateView();
                if ( loadingModelData )
                {
                    loadingModelData = false;
                }
                
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

     

        /// <summary>
        /// Puts data into the Model from the controls on the view.
        /// </summary>
        public override void UpdateModel()
        {
            
        }

        public void PopulateClinicalResearchField()
        {
            cboPatientInClinicalResearch.Items.Clear();

            var blank = new YesNoFlag();
            blank.SetBlank( String.Empty );
            cboPatientInClinicalResearch.Items.Add( blank );

            var yes = new YesNoFlag();
            yes.SetYes( "Yes" );
            cboPatientInClinicalResearch.Items.Add( yes );

            var no = new YesNoFlag();
            no.SetNo( "No" );
            cboPatientInClinicalResearch.Items.Add( no );

            if ( Model.IsPatientInClinicalResearchStudy != null )
            {
                cboPatientInClinicalResearch.SelectedIndex =
                    cboPatientInClinicalResearch.FindString(
                        Model.IsPatientInClinicalResearchStudy.Description.ToUpper() );
            }
            else
            {
                cboPatientInClinicalResearch.SelectedIndex = 0;
            }
        }

        public void ShowClinicalResearchFieldDisabled()
        {
            if ( cboPatientInClinicalResearch.Items.Count > 0 )
            {
                if ( Model.IsPatientInClinicalResearchStudy != null )
                {
                    cboPatientInClinicalResearch.SelectedIndex =
                        cboPatientInClinicalResearch.FindString(
                            Model.IsPatientInClinicalResearchStudy.Description.ToUpper() );
                }
                else
                {
                    cboPatientInClinicalResearch.SelectedIndex = 0;
                }
            }

            UIColors.SetDisabledDarkBgColor( cboPatientInClinicalResearch );

            cboPatientInClinicalResearch.Enabled = false;
        }

        public void ShowClinicalResearchFieldEnabled()
        {
            cboPatientInClinicalResearch.Enabled = true;
            UIColors.SetNormalBgColor( cboPatientInClinicalResearch );
        }

        public void ShowClinicalResearchFieldsAsVisible( bool show )
        {
            lblPatientUnderResearchStudy.Visible = show;
            cboPatientInClinicalResearch.Visible = show;
        }

        #endregion

        #region Properties

        private IClinicalTrialsDetailsView ClinicalTrialsDetailsView { get; set; }
        private IClinicalTrialsPresenter ClinicalTrialsPresenter { get; set; }
        private CptCodesPresenter CptCodesPresenter { get; set; }

        public string NursingStationCode
        {
            get
            {
                return i_NursingStationCode;
            }
            set
            {
                i_NursingStationCode = value;
            }
        }

        private Hashtable ConditionMap
        {
            get
            {
                return i_ConditionMap;
            }
        }

        /// <summary>
        /// Gets or sets the diagnosis view presenter.
        /// </summary>
        /// <value>The diagnosis view presenter.</value>
        private IShortDiagnosisViewPresenter ShortDiagnosisViewPresenter
        {
            get
            {
                return diagnosisViewPresenter;
            }

            set
            {
                diagnosisViewPresenter = value;
            }
        }

        public new Account Model
        {
            get
            {
                return ( Account )base.Model;
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

        public YesNoFlag IsPatientInClinicalResearchStudy
        {
            get { return ( YesNoFlag )cboPatientInClinicalResearch.SelectedItem; }
            set
            {
                if ( cboPatientInClinicalResearch.Items.Count != 0 )
                {
                    cboPatientInClinicalResearch.SelectedIndex =
                        cboPatientInClinicalResearch.FindString( value.Description );
                }
            }
        }

        public bool ViewDetailsCommandVisible
        {
            get
            {
                return btnViewClinicalTrialsDetails.Visible;
            }
            set
            {
                btnViewClinicalTrialsDetails.Visible = value;
            }
        }

        public bool ViewDetailsCommandEnabled
        {
            get
            {
                return btnViewClinicalTrialsDetails.Enabled;
            }
            set
            {
                btnViewClinicalTrialsDetails.Enabled = value;
            }
        }


        public bool GetConfirmationForDiscardingPatientStudies()
        {
            var result = MessageBox.Show( UIErrorMessages.WILL_LOSE_CLINICALTRIALS_DATA_ON_CLINICAL_VIEW_SCREEN_WARNING_MESSAGE, "Warning!",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2 );
            if ( result == DialogResult.Yes )
            {
                return true;
            }
            
            return false;
        }

        #endregion

        #region Private Methods

        private void CheckPreviousCondition()
        {
            if ( Model.Diagnosis.Condition.GetType() == typeof( Accident ) )
            {
                cmbAccidentType.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Clears the procedure field on UI.
        /// </summary>
        public void ClearProcedureField()
        {
            mtbProcedure.Text = String.Empty;
        }

        public void ShowProcedureDisabled()
        {
            UIColors.SetDisabledDarkBgColor( mtbProcedure );
            mtbProcedure.Enabled = false;
        }
        /// <summary>
        /// Shows the procedure enabled on UI.
        /// </summary>
        public void ShowProcedureEnabled()
        {
            UIColors.SetNormalBgColor( mtbProcedure );
            mtbProcedure.Enabled = true;
            RuleEngine.EvaluateRule( typeof( ProcedureRequired ), Model );
        }
        /// <summary>
        /// Determines whether [has procedure data].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has procedure data]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasProcedureData()
        {
            return ( Model.Diagnosis.Procedure != null && !String.IsNullOrEmpty( Model.Diagnosis.Procedure ) );
        }
        private void RegisterRequiredAndPreferredRules()
        {
            RuleEngine.RegisterEvent( typeof( ChiefComplaintRequired ), new EventHandler( ChiefComplaintRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( ProcedureRequired ), new EventHandler( ProcedureRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( AccidentTypeRequired ), new EventHandler( AccidentTypeRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( DateOfAccidentOrCrimeRequired ), new EventHandler( DateOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( HourOfAccidentOrCrimeRequired ), new EventHandler( HourOfAccidentOrCrimeRequiredEventHandler ) );

            RuleEngine.RegisterEvent( typeof( CountryOfAccidentOrCrimeRequired ), new EventHandler( CountryOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( StateOfAccidentOrCrimeRequired ), new EventHandler( StateOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( OnsetDateOfSymptomsOrIllnessRequired ), new EventHandler( OnsetDateOfSymptomsOrIllnessRequiredEventHandler ) );

            RuleEngine.RegisterEvent( typeof( DiagnosisClinicOneRequired ), new EventHandler( DiagnosisClinicOneRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( AdmitSourceRequired ), new EventHandler( AdmitSourceRequiredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( PatientInClinicalstudyPreferred ),
               new EventHandler( PatientInClinicalResearchStudyPreferredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( PatientInClinicalstudyRequired ),
                new EventHandler( PatientInClinicalResearchStudyRequiredEventHandler ) );
        }
        private void UnRegisterRequiredAndPreferredRules()
        {
            RuleEngine.UnregisterEvent( typeof( ChiefComplaintRequired ), new EventHandler( ChiefComplaintRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( ProcedureRequired ), new EventHandler( ProcedureRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( AccidentTypeRequired ), new EventHandler( AccidentTypeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( DateOfAccidentOrCrimeRequired ), new EventHandler( DateOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( HourOfAccidentOrCrimeRequired ), new EventHandler( HourOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( CountryOfAccidentOrCrimeRequired ), new EventHandler( CountryOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( StateOfAccidentOrCrimeRequired ), new EventHandler( StateOfAccidentOrCrimeRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( OnsetDateOfSymptomsOrIllnessRequired ), new EventHandler( OnsetDateOfSymptomsOrIllnessRequiredEventHandler ) );

            RuleEngine.UnregisterEvent( typeof( DiagnosisClinicOneRequired ), new EventHandler( DiagnosisClinicOneRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( AdmitSourceRequired ), new EventHandler( AdmitSourceRequiredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( PatientInClinicalstudyPreferred ),
           new EventHandler( PatientInClinicalResearchStudyPreferredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( PatientInClinicalstudyRequired ),
                new EventHandler( PatientInClinicalResearchStudyRequiredEventHandler ) );
        }

        private void RegisterEvents()
        {
            if ( i_Registered )
            {
                return;
            }

            i_Registered = true;
            RegisterRequiredAndPreferredRules();
            RuleEngine.RegisterEvent( typeof( InvalidAdmitSourceCode ), new EventHandler( InvalidAdmitSourceCodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_1Code ), new EventHandler( InvalidClinic1CodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_2Code ), new EventHandler( InvalidClinic2CodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_3Code ), new EventHandler( InvalidClinic3CodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_4Code ), new EventHandler( InvalidClinic4CodeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_5Code ), new EventHandler( InvalidClinic5CodeEventHandler ) );

            RuleEngine.RegisterEvent( typeof( InvalidAdmitSourceCodeChange ), new EventHandler( InvalidAdmitSourceCodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_1CodeChange ), new EventHandler( InvalidClinic1CodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_2CodeChange ), new EventHandler( InvalidClinic2CodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_3CodeChange ), new EventHandler( InvalidClinic3CodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_4CodeChange ), new EventHandler( InvalidClinic4CodeChangeEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InvalidClinic_5CodeChange ), new EventHandler( InvalidClinic5CodeChangeEventHandler ) );
           
        }

        private void UnregisterEvents()
        {
            i_Registered = false;

            UnRegisterRequiredAndPreferredRules();
             RuleEngine.UnregisterEvent( typeof( InvalidAdmitSourceCode ), new EventHandler( InvalidAdmitSourceCodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_1Code ), new EventHandler( InvalidClinic1CodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_2Code ), new EventHandler( InvalidClinic2CodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_3Code ), new EventHandler( InvalidClinic3CodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_4Code ), new EventHandler( InvalidClinic4CodeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_5Code ), new EventHandler( InvalidClinic5CodeEventHandler ) );

            RuleEngine.UnregisterEvent( typeof( InvalidAdmitSourceCodeChange ), new EventHandler( InvalidAdmitSourceCodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_1CodeChange ), new EventHandler( InvalidClinic1CodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_2CodeChange ), new EventHandler( InvalidClinic2CodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_3CodeChange ), new EventHandler( InvalidClinic3CodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_4CodeChange ), new EventHandler( InvalidClinic4CodeChangeEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InvalidClinic_5CodeChange ), new EventHandler( InvalidClinic5CodeChangeEventHandler ) );
       
        }

        private void IsValidAccidentTypeForInsurance()
        {
            Coverage primaryCoverage = Model.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
            if ( primaryCoverage != null && primaryCoverage.InsurancePlan != null )
            {
                WorkersCompensationInsurancePlan workcomp = new WorkersCompensationInsurancePlan();
                if ( primaryCoverage.InsurancePlan.GetType() != workcomp.GetType() )
                {
                    MessageBox.Show( UIErrorMessages.DIAGNOSIS_ACCIDENTTYPE_INS_CHK, "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
            }
        }

        private void HandleClinicIndexChange( ComboBox cmbClinic )
        {
            RemoveClinicHandler();
            ComboValueSet( cmbClinic );
            AddClinicHandler();

            RuleEngine.RegisterEvent( typeof( DiagnosisClinicOneRequired ), new EventHandler( DiagnosisClinicOneRequiredEventHandler ) );
        }

        private void ComboValueSet( ComboBox combo )
        {
            if ( combo != null )
            {
                ResetAllSelection();
            }
        }

        private void ResetAllSelection()
        {
            Hashtable tempHolder = new Hashtable();

            foreach ( ComboBox combo in combos.Values )
            {
                if ( combo.SelectedIndex > 0 )
                {
                    tempHolder.Add( combo.Name, combo.SelectedItem );
                }
            }

            LoadCombosData( currentCombo );

            foreach ( object key in tempHolder.Keys )
            {
                ComboBox combo = ( ( ComboBox )combos[key] );
                Object comboValue = tempHolder[key];

                ReplaceItem( combo as PatientAccessComboBox, comboValue );

                RemoveValueFromCombos( tempHolder[key], ( ComboBox )combos[key] );
            }
        }

        private static void ReplaceItem( PatientAccessComboBox combo, object comboValue )
        {
            if ( combo != null )
            {
                combo.SelectedItem = comboValue as HospitalClinic;
            }

        }

        private void LoadClinicCombos()
        {
            AddClinicComboBox( clinicView1.cmbClinic );
            AddClinicComboBox( clinicView2.cmbClinic );
            AddClinicComboBox( clinicView3.cmbClinic );
            AddClinicComboBox( clinicView4.cmbClinic );
            AddClinicComboBox( clinicView5.cmbClinic );
        }

        private void LoadCombosData( ComboBox comboBox )
        {
            foreach ( ComboBox combo in combos.Values )
            {
                if ( combo.Name != comboBox.Name )
                {
                    combo.Items.Clear();

                    foreach ( object row in i_AllClinics )
                    {
                        combo.Items.Add( row );
                    }
                    if ( combo.Items.Count > 0 )
                    {
                        combo.SelectedIndex = 0;
                    }
                    combo.Refresh();
                }
            }
        }

        private void RemoveValueFromCombos( object comboValue, ComboBox excludeCombo )
        {
            foreach ( ComboBox combo in combos.Values )
            {
                if ( excludeCombo != combo )
                {
                    int i = combo.FindString( comboValue.ToString() );
                    if ( i > 0 )
                    {
                        combo.Items.RemoveAt( i );
                    }
                }
            }
        }

        private void AddClinicComboBox( ComboBox comboBox )
        {
            if ( !combos.ContainsKey( comboBox.Name ) )
            {
                combos.Add( comboBox.Name, comboBox );
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
            int upBound = Model.OccurrenceCodes.Count - 1;
            for ( int i = upBound; i > -1; i-- )
            {
                OccurrenceCode occ = ( ( OccurrenceCode )Model.OccurrenceCodes[i] );
                if ( occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO_NO_FAULT ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_EMPLOYER_REL ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_OTHER ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_ACCIDENT_TORT_LIABILITY ||
                    occ.Code == OccurrenceCode.OCCURRENCECODE_CRIME )
                {
                    Model.RemoveOccurrenceCode( occ );
                }
            }

            SortOccurrencesByCode sortOccCodes = new SortOccurrencesByCode();
            ( ( ArrayList )Model.OccurrenceCodes ).Sort( sortOccCodes );
        }

        private void RunInvalidValuesRules()
        {
            UIColors.SetNormalBgColor( cmbAdmitSrc );
            UIColors.SetNormalBgColor( clinicView1.cmbClinic );
            UIColors.SetNormalBgColor( clinicView2.cmbClinic );
            UIColors.SetNormalBgColor( clinicView3.cmbClinic );
            UIColors.SetNormalBgColor( clinicView4.cmbClinic );
            UIColors.SetNormalBgColor( clinicView5.cmbClinic );
             
            RuleEngine.EvaluateRule( typeof( DiagnosisClinicOneRequired ), Model );
            RuleEngine.EvaluateRule( typeof( OnShortDiagnosisForm ), Model );
        }

        private void CheckForRequiredAccidentFields()
        {
            // reset all fields that might have error, preferred, or required backgrounds        
            UnRegisterRequiredAndPreferredRules();
            RegisterRequiredAndPreferredRules();
            UIColors.SetNormalBgColor( cmbAccidentType );
            UIColors.SetNormalBgColor( mtbAccidentCrimeDate );
            UIColors.SetNormalBgColor( cmbHour );
            UIColors.SetNormalBgColor( cmbCountry );
            UIColors.SetNormalBgColor( cmbState );
            UIColors.SetNormalBgColor( mtbOnsetDate );
            UIColors.SetNormalBgColor( dateTimePickerSickness );

            RuleEngine.EvaluateRule( typeof( AccidentTypeRequired ), Model );
            RuleEngine.EvaluateRule( typeof( DateOfAccidentOrCrimeRequired ), Model );
            RuleEngine.EvaluateRule( typeof( HourOfAccidentOrCrimeRequired ), Model ); 
            RuleEngine.EvaluateRule( typeof( CountryOfAccidentOrCrimeRequired ), Model ); 
            RuleEngine.EvaluateRule( typeof( StateOfAccidentOrCrimeRequired ), Model ); 
            RuleEngine.EvaluateRule( typeof( OnsetDateOfSymptomsOrIllnessRequired ), Model );
        }

        private void EnableDisableClinics( bool isEnabled )
        {
            UIColors.SetNormalBgColor( clinicView1.cmbClinic );
            clinicView1.cmbClinic.Enabled = isEnabled;
            clinicView2.cmbClinic.Enabled = isEnabled;
            clinicView3.cmbClinic.Enabled = isEnabled;
            clinicView4.cmbClinic.Enabled = isEnabled;
            clinicView5.cmbClinic.Enabled = isEnabled;
            ServiceCategoryPresenter.EnableOrDisableServiceCategory(isEnabled);
        }

        private void ClearSelectedClinics( VisitType visitType )
        {
            if ( visitType != null && visitType.Code != null
                && visitType.Code != VisitType.INPATIENT )
            {
                EnableDisableClinics( true );
                return;
            }

            Clinic newClinic = new Clinic();
            clinicView1.cmbClinic.SelectedItem = newClinic;
            clinicView2.cmbClinic.SelectedItem = newClinic;
            clinicView3.cmbClinic.SelectedItem = newClinic;
            clinicView4.cmbClinic.SelectedItem = newClinic;
            clinicView5.cmbClinic.SelectedItem = newClinic;
            ServiceCategoryPresenter.ClearServiceCategory();
            EnableDisableClinics( false );
        }

        private void LocalUpdateView()
        {
            if ( Model.Activity == null )
            {   // If the AccountView was invoked from a Worklist
                //  Action, there will be no activity
                return;
            }

            foreach ( OccurrenceCode occ in Model.OccurrenceCodes )
            {
                if ( occ.Code == "11" )
                {
                    if ( occ.OccurrenceDate != DateTime.MinValue )
                    {
                        mtbOnsetDate.UnMaskedText = occ.OccurrenceDate.ToString( "MMddyyyy" );
                    }
                    break;
                }
            }

            if ( cmbCountry.Items.Count == 0 )
            {
                DisplayCountries();
            }

            if ( cmbState.Items.Count == 0 )
            {
                DisplayStates();
            }

            DisplayChiefComplaint();
            DisplayAccidentTypes();
            DisplayCondition();


            if ( Model.Activity.GetType().Equals( typeof( ShortPreRegistrationActivity ) ) ||
                 ( Model.Activity.GetType().Equals( typeof( ShortMaintenanceActivity ) ) &&
                   Model.KindOfVisit != null && Model.KindOfVisit.Code == VisitType.PREREG_PATIENT ) )
            {
                int newbornIndex = cmbAdmitSrc.FindString( "L NEWBORN" );
                if ( newbornIndex != -1 )
                {
                    cmbAdmitSrc.Items.RemoveAt( newbornIndex );
                }
            }

            RunInvalidValuesRules();
        }


        private void PopulateClinics()
        {
            clinicView1.LabelText = "Clinic 1:";
            clinicView1.PatientFacility = facility;
            clinicView1.UpdateView();
            i_AllClinics = clinicView1.AllHospitalClinics();

            clinicView2.LabelText = "Clinic 2:";
            clinicView2.ListOfClinics = i_AllClinics;
            clinicView2.UpdateView();

            clinicView3.LabelText = "Clinic 3:";
            clinicView3.ListOfClinics = i_AllClinics;
            clinicView3.UpdateView();

            clinicView4.LabelText = "Clinic 4:";
            clinicView4.ListOfClinics = i_AllClinics;
            clinicView4.UpdateView();

            clinicView5.LabelText = "Clinic 5:";
            clinicView5.ListOfClinics = i_AllClinics;
            clinicView5.UpdateView();

            if ( SetDefaultClinic() )
            {
                SelectPreviouslyStoredClinicValue();
            }
        }

        private void SelectPreviouslyStoredClinicValue()
        {
            clinicView1.SetClinicValueFromModel( Model.Clinics[0] as HospitalClinic );
            clinicView2.SetClinicValueFromModel( Model.Clinics[1] as HospitalClinic );
            clinicView3.SetClinicValueFromModel( Model.Clinics[2] as HospitalClinic );
            clinicView4.SetClinicValueFromModel( Model.Clinics[3] as HospitalClinic );
            clinicView5.SetClinicValueFromModel( Model.Clinics[4] as HospitalClinic );
            ServiceCategoryPresenter.SetSelectedServiceCategory(string.IsNullOrEmpty(this.Model.EmbosserCard) ? string.Empty : this.Model.EmbosserCard);
        }

        private bool SetDefaultClinic()
        {
            bool setDefaultClinicValue = false;

            switch ( Model.Activity.GetType().Name )
            {
                case "RegistrationActivity":
                case "ShortRegistrationActivity":
                case "PreRegistrationActivity":
                case "ShortPreRegistrationActivity":
                case "AdmitNewbornActivity":
                    break;
                case "PostMSERegistrationActivity":
                    setDefaultClinicValue = true;
                    break;
                case "MaintenanceActivity":
                case "ShortMaintenanceActivity":
                    if ( Model.KindOfVisit != null
                        && Model.KindOfVisit.Code != VisitType.INPATIENT )
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
            ArrayList allSources = ( ArrayList )brokerProxy.AllTypesOfAdmitSources( User.GetCurrent().Facility.Oid );

            cmbAdmitSrc.Items.Clear();

            foreach ( AdmitSource source in allSources )
            {
                cmbAdmitSrc.Items.Add( source );
            }

            if ( Model.AdmitSource != null )
            {
                cmbAdmitSrc.SelectedItem = Model.AdmitSource;
            }

            // OTD# 37055 fix - Admit source field should not be editable for a Newborn account
            if ( Model.Activity.GetType().Equals( typeof( AdmitNewbornActivity ) ) )
            {
                cmbAdmitSrc.Enabled = false;
            }
        }

        /// <summary>
        /// Display Countries
        /// </summary>
        private void DisplayCountries()
        {
            if ( cmbCountry.Items.Count == 0 )
            {
                IAddressBroker broker = new AddressBrokerProxy();
                CountryComboHelper = new ReferenceValueComboBox( cmbCountry );

                CountryComboHelper.PopulateWithCollection( broker.AllCountries( Model.Facility.Oid ) );
                cmbCountry.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Display States
        /// </summary>
        private void DisplayStates()
        {
            if ( cmbState.Items.Count == 0 )
            {
                IAddressBroker broker = new AddressBrokerProxy();

                if ( cmbState.Items.Count == 0 )
                {
                    StateComboHelper.PopulateWithCollection(broker.AllStates(User.GetCurrent().Facility.Oid));
                }

                foreach ( ContactPoint contactPoint in Model.Facility.ContactPoints )
                {
                    if ( contactPoint != null && contactPoint.TypeOfContactPoint.ToString() == TypeOfContactPoint.NewPhysicalContactPointType().ToString() )
                    {
                        cmbState.Text = contactPoint.Address.State.Code + "-" +
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
            if ( cmbAccidentType.Items.Count == 0 )
            {
                OccuranceCodeBrokerProxy brokerProxy = new OccuranceCodeBrokerProxy();
                IList accidentTypeList = ( IList )brokerProxy.GetAccidentTypes( User.GetCurrent().Facility.Oid );

                if ( accidentTypeList == null )
                {
                    MessageBox.Show( "IOccuranceCodeBroker.GetAccidentTypes() returned empty list.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }

                cmbAccidentType.ValueMember = "Value";
                cmbAccidentType.DisplayMember = "Key";
                //Add Default blank
                cmbAccidentType.Items.Add( String.Empty );

                DictionaryEntry other = new DictionaryEntry( "Other", TypeOfAccident.OTHER );

                foreach ( TypeOfAccident typeOfAccident in accidentTypeList )
                {
                    DictionaryEntry newEntry = new DictionaryEntry( GetAccidentTypeDisplayString( typeOfAccident ),
                            typeOfAccident );

                    if ( typeOfAccident.Oid == TypeOfAccident.OTHER )
                    {
                        other = newEntry;
                    }

                    cmbAccidentType.Items.Add( newEntry );
                }

                cmbAccidentType.Sorted = true;
                cmbAccidentType.Items.Remove( other );
                cmbAccidentType.Sorted = false;
                cmbAccidentType.Items.Add( other );
            }
        }

        private static string GetAccidentTypeDisplayString( TypeOfAccident typeOfAccident )
        {
            string accidentTypeDisplayString;

            if ( typeOfAccident.Description == "AutoNoFaultInsurance" )
            {
                accidentTypeDisplayString = "Auto no fault insurance";
            }
            else if ( typeOfAccident.Description == "Employment Related" )
            {
                accidentTypeDisplayString = "Employment-related";
            }
            else if ( typeOfAccident.Description == "Tort Liability" )
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
            mtbComplaint.Text = Model.Diagnosis.ChiefComplaint.Trim();
        }
        /// <summary>
        /// Display Procedure
        /// </summary>
        void IShortDiagnosisView.PopulateProcedure()
        {
            mtbProcedure.Text = Model.Diagnosis.Procedure != null ? Model.Diagnosis.Procedure.Trim() : String.Empty;
        }

        /// <summary>
        /// Set Accident Crime Date
        /// </summary>

        private void SetAccidentCrimeDate()
        {
            TimeAndLocationBoundCondition condition = ( TimeAndLocationBoundCondition )Model.Diagnosis.Condition;
            condition.OccurredOn = Convert.ToDateTime( dateTimePickerAccident.Text );
        }

        /// <summary>
        /// Set Accident Crime Hour
        /// </summary>
        private void SetAccidentCrimeHour()
        {
            TimeAndLocationBoundCondition condition = ( TimeAndLocationBoundCondition )Model.Diagnosis.Condition;

            condition.OccurredAtHour = String.Empty;
            if ( cmbHour.SelectedItem != null )
            {
                condition.OccurredAtHour = cmbHour.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Set Accident Crime Country
        /// </summary>
        /// <param name="country"></param>
        private void SetAccidentCrimeCountry( Country country )
        {
            if ( rbAccident.Checked || rbCrime.Checked )
            {
                TimeAndLocationBoundCondition condition = ( TimeAndLocationBoundCondition )Model.Diagnosis.Condition;
                condition.Country = country;
            }
        }

        /// <summary>
        /// Set Accident Crime State
        /// </summary>
        /// <param name="state"></param>
        private void SetAccidentCrimeState( State state )
        {
            TimeAndLocationBoundCondition condition = ( TimeAndLocationBoundCondition )Model.Diagnosis.Condition;
            condition.State = state;
        }

        /// <summary>
        /// Set Onset Date Error Bg Color
        /// </summary>
        private void SetOnsetDateErrBgColor()
        {
            UIColors.SetErrorBgColor( mtbOnsetDate );
        }

        /// <summary>
        /// Set Onset Date Normal Bg Color
        /// </summary>
        private void SetOnsetDateNormalBgColor()
        {
            UIColors.SetNormalBgColor( mtbOnsetDate );
            Refresh();
        }

        private void SetAccidentCrimeDateErrBgColor()
        {
            UIColors.SetErrorBgColor( mtbAccidentCrimeDate );
        }

        private void SetAccidentCrimeDateNormalBgColor()
        {
            UIColors.SetNormalBgColor( mtbAccidentCrimeDate );
            Refresh();
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
            if ( null != control )
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
            if ( null != control )
            {
                UIColors.SetErrorBgColor( control );
            }
        }

        /// <summary>
        /// Verify Accident Crime Date
        /// </summary>
        private bool VerifyAccidentCrimeDate()
        {
            SetAccidentCrimeDateNormalBgColor();
            if ( mtbAccidentCrimeDate.UnMaskedText.Length == 0
                || mtbAccidentCrimeDate.UnMaskedText ==
                String.Empty )
            {
                return true;
            }

            if ( mtbAccidentCrimeDate.Text.Length != 10 )
            {
                mtbAccidentCrimeDate.Focus();
                SetAccidentCrimeDateErrBgColor();
                MessageBox.Show( UIErrorMessages.DIAGNOSIS_ACCIDENTCRIME_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return false;
            }

            string month = mtbAccidentCrimeDate.Text.Substring( 0, 2 );
            string day = mtbAccidentCrimeDate.Text.Substring( 3, 2 );
            string year = mtbAccidentCrimeDate.Text.Substring( 6, 4 );

            verifyMonth = Convert.ToInt32( month );
            verifyDay = Convert.ToInt32( day );
            verifyYear = Convert.ToInt32( year );

            try
            {   // Check the date entered is not in the future
                DateTime theDate = new DateTime( verifyYear, verifyMonth, verifyDay );

                if ( DateValidator.IsValidDate( theDate ) == false )
                {
                    GetFocusAndHighlightRedError( mtbAccidentCrimeDate );
                    ShowErrorMessageBox( UIErrorMessages.DIAGNOSIS_ACCIDENTCRIME_INVALID_ERRMSG );
                    return false;
                }
            }
            catch ( ArgumentOutOfRangeException )
            {
                GetFocusAndHighlightRedError( mtbAccidentCrimeDate );
                ShowErrorMessageBox( UIErrorMessages.DIAGNOSIS_ACCIDENTCRIME_INVALID_ERRMSG );
                return false;
            }

            TimeAndLocationBoundCondition condition = ( TimeAndLocationBoundCondition )Model.Diagnosis.Condition;
            condition.OccurredOn = Convert.ToDateTime( mtbAccidentCrimeDate.Text );

            return true;
        }

        /// <summary>
        /// Verify Onset Date
        /// </summary>
        private bool VerifyOnsetDate()
        {
            SetOnsetDateNormalBgColor();
            if ( mtbOnsetDate.UnMaskedText.Length == 0 ||
                mtbOnsetDate.UnMaskedText == String.Empty )
            {
                OccurrenceCode occ11 = new OccurrenceCode( PersistentModel.NEW_OID,
                    PersistentModel.NEW_VERSION,
                    string.Empty,
                    "11" );
                Model.RemoveOccurrenceCode( occ11 );
                return true;
            }

            if ( mtbOnsetDate.Text.Length != 10 )
            {
                mtbOnsetDate.Focus();
                SetOnsetDateErrBgColor();
                MessageBox.Show( UIErrorMessages.DIAGNOSIS_ONSETDATE_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return false;
            }

            string month = mtbOnsetDate.Text.Substring( 0, 2 );
            string day = mtbOnsetDate.Text.Substring( 3, 2 );
            string year = mtbOnsetDate.Text.Substring( 6, 4 );

            verifyMonth = Convert.ToInt32( month );
            verifyDay = Convert.ToInt32( day );
            verifyYear = Convert.ToInt32( year );

            try
            {   // Check the date entered is not in the future
                DateTime theDate = new DateTime( verifyYear, verifyMonth, verifyDay );

                if ( !DateValidator.IsValidDate( theDate ) )
                {
                    GetFocusAndHighlightRedError( mtbOnsetDate );
                    ShowErrorMessageBox( UIErrorMessages.DIAGNOSIS_ONSETDATE_INVALID_ERRMSG );
                    return false;
                }

                OccuranceCodeBrokerProxy brokerProxy = new OccuranceCodeBrokerProxy();
                OccurrenceCode occ11 = brokerProxy.OccurrenceCodeWith( User.GetCurrent().Facility.Oid, OccurrenceCode.OCCURRENCECODE_ILLNESS );
                occ11.OccurrenceDate = theDate;
                Model.AddOccurrenceCode( ( OccurrenceCode )( occ11.Clone() ) );
            }
            catch ( ArgumentOutOfRangeException )
            {
                GetFocusAndHighlightRedError( mtbOnsetDate );
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
            mtbAccidentCrimeDate.UnMaskedText = String.Empty;
            cmbHour.SelectedIndex = -1;
            cmbCountry.SelectedIndex = -1;
            cmbState.SelectedIndex = -1;

            rbNone.Checked = true;
            if ( typeof( AdmitNewbornActivity ).Equals( Model.Activity.GetType() ) )
            {
                onSetEnabled = false;
            }

            mtbOnsetDate.Enabled = onSetEnabled;
            dateTimePickerSickness.Enabled = onSetEnabled;
        }


        /// <summary>
        /// Display Accident
        /// </summary>
        private void DisplayAccident()
        {
            rbAccident.Checked = true;
            cmbAccidentType.Enabled = true;

            try
            {
                if ( Model.Diagnosis != null && Model.Diagnosis.Condition != null )
                {
                    Accident accident = Model.Diagnosis.Condition as Accident;
                    if ( accident != null && accident.Kind != null )
                    {
                        int index = cmbAccidentType.FindStringExact( GetAccidentTypeDisplayString( accident.Kind ) );
                        if ( index != -1 )
                        {
                            cmbAccidentType.SelectedIndex = index;
                        }
                    }

                    if ( accident != null )
                    {
                        accidentType = accident.Kind;
                    }
                    EnableAccidentCrimeDetails();
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
            rbCrime.Checked = true;

            try
            {
                if ( Model.Diagnosis != null && Model.Diagnosis.Condition != null )
                {
                    EnableAccidentCrimeDetails();
                }
            }
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
            rbNone.Checked = true;
            bool onSetEnabled = true;

            try
            {
                if ( Model.Diagnosis != null && Model.Diagnosis.Condition != null )
                {
                    UnknownCondition unKnownCondition = new UnknownCondition();
                    Model.Diagnosis.Condition = unKnownCondition;

                    if ( typeof( AdmitNewbornActivity ).Equals( Model.Activity.GetType() ) )
                    {
                        onSetEnabled = false;
                    }

                    mtbOnsetDate.Enabled = onSetEnabled;
                    dateTimePickerSickness.Enabled = onSetEnabled;
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
            i_ConditionMap.Add( typeof( Illness ), new DisplayAbstractCondition( DisplayIllness ) );
            i_ConditionMap.Add( typeof( Accident ), new DisplayAbstractCondition( DisplayAccident ) );
            i_ConditionMap.Add( typeof( Crime ), new DisplayAbstractCondition( DisplayCrime ) );
            i_ConditionMap.Add( typeof( UnknownCondition ), new DisplayAbstractCondition( DisplayNone ) );
        }

        /// <summary>
        /// Enable Accident Crime Details
        /// </summary>
        private void EnableAccidentCrimeDetails()
        {
            TimeAndLocationBoundCondition condition = ( TimeAndLocationBoundCondition )Model.Diagnosis.Condition;

            if ( condition.OccurredOn == DateTime.MinValue )
            {
                mtbAccidentCrimeDate.Text = String.Empty;
            }
            else
            {
                mtbAccidentCrimeDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}",
                    condition.OccurredOn.Month,
                    condition.OccurredOn.Day,
                    condition.OccurredOn.Year );
            }

            accidentDateTxt = mtbAccidentCrimeDate.Text;

            if ( condition.OccurredAtHour == String.Empty )
            {
                cmbHour.SelectedIndex = 0;
            }
            else
            {
                var searchValue = condition.GetOccurredHour();

                cmbHour.SelectedIndex = cmbHour.FindString( searchValue );
            }

            if ( condition.Country != null )
            {
                cmbCountry.SelectedItem = condition.Country;
            }

            if ( condition.State != null )
            {
                cmbState.SelectedItem = condition.State;
            }

            if ( typeof( AdmitNewbornActivity ).Equals( Model.Activity.GetType() ) )
            {
                mtbAccidentCrimeDate.Enabled = false;
                dateTimePickerAccident.Enabled = false;
                UIColors.SetDisabledDarkBgColor( mtbOnsetDate );
            }
            else
            {
                mtbAccidentCrimeDate.Enabled = true;
                dateTimePickerAccident.Enabled = true;
            }

            cmbHour.Enabled = true;
            cmbCountry.Enabled = true;
            cmbState.Enabled = true;
            SetAccidentCrimeControlsNormalBackGroundColor();
        }

        private void SetAccidentCrimeControlsNormalBackGroundColor()
        {
            if ( rbAccident.Checked )
            {
                UIColors.SetNormalBgColor( cmbAccidentType );
            }

            UIColors.SetNormalBgColor( mtbAccidentCrimeDate );
            UIColors.SetNormalBgColor( cmbHour );
            UIColors.SetNormalBgColor( cmbCountry );
            UIColors.SetNormalBgColor( cmbState );
            UIColors.SetNormalBgColor( mtbOnsetDate );
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureChiefComplaint( mtbComplaint );
            MaskedEditTextBoxBuilder.ConfigureProcedure( mtbProcedure );
            MaskedEditTextBoxBuilder.ConfigureClinicalViewComments( mtbComments );
        }

        #endregion

        #region Internal Properties
        #endregion

        #region Private Properties

        private ArrayList PatientTypes
        {
            get
            {
                if ( i_PatientTypes == null )
                {
                    string activityType = string.Empty;
                    string associatedActivityType = string.Empty;
                    string kindOfVisitCode = string.Empty;
                    string financialClassCode = string.Empty;
                    string locationBedCode = string.Empty;

                    Activity activity = Model.Activity;
                    if ( activity != null )
                    {
                        activityType = activity.GetType().ToString();
                        if ( activity.AssociatedActivityType != null )
                        {
                            associatedActivityType = activity.AssociatedActivityType.ToString();
                        }
                    }
                    if ( Model.KindOfVisit != null )
                    {
                        kindOfVisitCode = Model.KindOfVisit.Code;
                    }
                    if ( Model.FinancialClass != null )
                    {
                        financialClassCode = Model.FinancialClass.Code;
                    }
                    if ( Model.Location != null &&
                        Model.Location.Bed != null )
                    {
                        locationBedCode = Model.Location.Bed.Code;
                    }

                    IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
                    i_PatientTypes = patientBroker.PatientTypesFor( activityType, associatedActivityType, kindOfVisitCode,
                                                                         financialClassCode, locationBedCode, Model.Facility.Oid);
                }

                return i_PatientTypes;
            }
        }

        private ReferenceValueComboBox StateComboHelper
        {
            get
            {
                return i_StateComboHelper;
            }
            set
            {
                i_StateComboHelper = value;
            }
        }


        private ReferenceValueComboBox CountryComboHelper
        {
            get
            {
                return i_CountryComboHelper;
            }
            set
            {
                i_CountryComboHelper = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShortDiagnosisView));
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
            this.lblAdmit = new System.Windows.Forms.Label();
            this.gbxClinics = new System.Windows.Forms.GroupBox();
            this.clinicView5 = new PatientAccess.UI.CommonControls.ClinicView();
            this.clinicView4 = new PatientAccess.UI.CommonControls.ClinicView();
            this.clinicView3 = new PatientAccess.UI.CommonControls.ClinicView();
            this.clinicView2 = new PatientAccess.UI.CommonControls.ClinicView();
            this.clinicView1 = new PatientAccess.UI.CommonControls.ClinicView();
            this.mtbComplaint = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblProcedure = new System.Windows.Forms.Label();
            this.mtbProcedure = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.btnViewClinicalTrialsDetails = new System.Windows.Forms.Button();
            this.lblPatientUnderResearchStudy = new System.Windows.Forms.Label();
            this.mtbComments = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblComments = new System.Windows.Forms.Label();
            this.physicianSelectionView1 = new PatientAccess.UI.ShortRegistration.CommonControls.PhysicianSelectionView();
            this.cboPatientInClinicalResearch = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.patientTypeHSVLocationView = new PatientAccess.UI.ShortRegistration.DiagnosisViews.ShortPatientTypeHSVLocationView();
            this.cmbAdmitSrc = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbAccidentType = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.rbNone = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.rbCrime = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.rbAccident = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.cptCodesView = new PatientAccess.UI.CptCodes.ViewImpl.CptCodesView();
            this.serviceCategory1 = new PatientAccess.UI.CommonControls.ServiceCategory.ViewImpl.ServiceCategory();
            this.gbxDetails.SuspendLayout();
            this.gbxClinics.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblComplaint
            // 
            this.lblComplaint.Location = new System.Drawing.Point(6, 57);
            this.lblComplaint.Name = "lblComplaint";
            this.lblComplaint.Size = new System.Drawing.Size(88, 14);
            this.lblComplaint.TabIndex = 0;
            this.lblComplaint.Text = "Chief complaint:";
            // 
            // lblVisitResult
            // 
            this.lblVisitResult.Location = new System.Drawing.Point(325, 4);
            this.lblVisitResult.Name = "lblVisitResult";
            this.lblVisitResult.Size = new System.Drawing.Size(171, 15);
            this.lblVisitResult.TabIndex = 0;
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
            this.gbxDetails.Location = new System.Drawing.Point(324, 63);
            this.gbxDetails.Name = "gbxDetails";
            this.gbxDetails.Size = new System.Drawing.Size(304, 127);
            this.gbxDetails.TabIndex = 11;
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
            this.dateTimePickerSickness.Location = new System.Drawing.Point(579, 192);
            this.dateTimePickerSickness.Name = "dateTimePickerSickness";
            this.dateTimePickerSickness.Size = new System.Drawing.Size(21, 20);
            this.dateTimePickerSickness.TabIndex = 13;
            this.dateTimePickerSickness.TabStop = false;
            this.dateTimePickerSickness.CloseUp += new System.EventHandler(this.dateTimePickerSickness_CloseUp);
            // 
            // lblOnset
            // 
            this.lblOnset.Location = new System.Drawing.Point(323, 193);
            this.lblOnset.Name = "lblOnset";
            this.lblOnset.Size = new System.Drawing.Size(193, 16);
            this.lblOnset.TabIndex = 0;
            this.lblOnset.Text = "Date of onset for symptoms or illness:";
            // 
            // lblType
            // 
            this.lblType.Location = new System.Drawing.Point(414, 22);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(34, 16);
            this.lblType.TabIndex = 0;
            this.lblType.Text = "Type:";
            // 
            // mtbOnsetDate
            // 
            this.mtbOnsetDate.KeyPressExpression = "^\\d*$";
            this.mtbOnsetDate.Location = new System.Drawing.Point(510, 192);
            this.mtbOnsetDate.Mask = "  /  /";
            this.mtbOnsetDate.MaxLength = 10;
            this.mtbOnsetDate.Name = "mtbOnsetDate";
            this.mtbOnsetDate.Size = new System.Drawing.Size(70, 20);
            this.mtbOnsetDate.TabIndex = 12;
            this.mtbOnsetDate.ValidationExpression = resources.GetString("mtbOnsetDate.ValidationExpression");
            this.mtbOnsetDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbOnsetDate_Validating);
            // 
            // lblAdmit
            // 
            this.lblAdmit.Location = new System.Drawing.Point(324, 214);
            this.lblAdmit.Name = "lblAdmit";
            this.lblAdmit.Size = new System.Drawing.Size(74, 18);
            this.lblAdmit.TabIndex = 0;
            this.lblAdmit.Text = "Admit source:";
            // 
            // gbxClinics
            // 
            this.gbxClinics.Controls.Add(this.clinicView5);
            this.gbxClinics.Controls.Add(this.clinicView4);
            this.gbxClinics.Controls.Add(this.clinicView3);
            this.gbxClinics.Controls.Add(this.clinicView2);
            this.gbxClinics.Controls.Add(this.clinicView1);
            this.gbxClinics.Location = new System.Drawing.Point(675, 8);
            this.gbxClinics.Name = "gbxClinics";
            this.gbxClinics.Size = new System.Drawing.Size(261, 155);
            this.gbxClinics.TabIndex = 14;
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
            // mtbComplaint
            // 
            this.mtbComplaint.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbComplaint.Location = new System.Drawing.Point(9, 71);
            this.mtbComplaint.Mask = "";
            this.mtbComplaint.MaxLength = 74;
            this.mtbComplaint.Multiline = true;
            this.mtbComplaint.Name = "mtbComplaint";
            this.mtbComplaint.Size = new System.Drawing.Size(285, 45);
            this.mtbComplaint.TabIndex = 4;
            this.mtbComplaint.Validating += new System.ComponentModel.CancelEventHandler(this.mtbComplaint_Validating);
            // 
            // lblProcedure
            // 
            this.lblProcedure.Location = new System.Drawing.Point(6, 119);
            this.lblProcedure.Name = "lblProcedure";
            this.lblProcedure.Size = new System.Drawing.Size(75, 11);
            this.lblProcedure.TabIndex = 0;
            this.lblProcedure.Text = "Procedure :";
            // 
            // mtbProcedure
            // 
            this.mtbProcedure.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbProcedure.Location = new System.Drawing.Point(9, 134);
            this.mtbProcedure.Mask = "";
            this.mtbProcedure.MaxLength = 74;
            this.mtbProcedure.Multiline = true;
            this.mtbProcedure.Name = "mtbProcedure";
            this.mtbProcedure.Size = new System.Drawing.Size(285, 44);
            this.mtbProcedure.TabIndex = 5;
            this.mtbProcedure.Validating += new System.ComponentModel.CancelEventHandler(this.mtb_Procedure_Validating);
            // 
            // btnViewClinicalTrialsDetails
            // 
            this.btnViewClinicalTrialsDetails.Location = new System.Drawing.Point(884, 192);
            this.btnViewClinicalTrialsDetails.Name = "btnViewClinicalTrialsDetails";
            this.btnViewClinicalTrialsDetails.Size = new System.Drawing.Size(75, 23);
            this.btnViewClinicalTrialsDetails.TabIndex = 16;
            this.btnViewClinicalTrialsDetails.Text = "View Details";
            this.btnViewClinicalTrialsDetails.UseVisualStyleBackColor = true;
            this.btnViewClinicalTrialsDetails.Visible = false;
            this.btnViewClinicalTrialsDetails.Click += new System.EventHandler(this.btnViewClinicalTrialsDetails_Click);
            // 
            // lblPatientUnderResearchStudy
            // 
            this.lblPatientUnderResearchStudy.BackColor = System.Drawing.Color.White;
            this.lblPatientUnderResearchStudy.Location = new System.Drawing.Point(636, 197);
            this.lblPatientUnderResearchStudy.Name = "lblPatientUnderResearchStudy";
            this.lblPatientUnderResearchStudy.Size = new System.Drawing.Size(201, 16);
            this.lblPatientUnderResearchStudy.TabIndex = 0;
            this.lblPatientUnderResearchStudy.Text = "Is patient in a Clinical Research Study? :";
            // 
            // mtbComments
            // 
            this.mtbComments.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbComments.Location = new System.Drawing.Point(9, 193);
            this.mtbComments.Mask = "";
            this.mtbComments.MaxLength = 120;
            this.mtbComments.Multiline = true;
            this.mtbComments.Name = "mtbComments";
            this.mtbComments.Size = new System.Drawing.Size(285, 41);
            this.mtbComments.TabIndex = 6;
            this.mtbComments.Validating += new System.ComponentModel.CancelEventHandler(this.mtbComments_Validating);
            // 
            // lblComments
            // 
            this.lblComments.Location = new System.Drawing.Point(6, 179);
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size(83, 21);
            this.lblComments.TabIndex = 20;
            this.lblComments.Text = "Comments:";
            // 
            // physicianSelectionView1
            // 
            this.physicianSelectionView1.BackColor = System.Drawing.Color.White;
            this.physicianSelectionView1.Location = new System.Drawing.Point(0, 235);
            this.physicianSelectionView1.Margin = new System.Windows.Forms.Padding(1);
            this.physicianSelectionView1.Model = null;
            this.physicianSelectionView1.Name = "physicianSelectionView1";
            this.physicianSelectionView1.Size = new System.Drawing.Size(936, 153);
            this.physicianSelectionView1.TabIndex = 19;
            // 
            // cboPatientInClinicalResearch
            // 
            this.cboPatientInClinicalResearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPatientInClinicalResearch.DropDownWidth = 3;
            this.cboPatientInClinicalResearch.Location = new System.Drawing.Point(834, 194);
            this.cboPatientInClinicalResearch.MaxLength = 3;
            this.cboPatientInClinicalResearch.Name = "cboPatientInClinicalResearch";
            this.cboPatientInClinicalResearch.Size = new System.Drawing.Size(47, 21);
            this.cboPatientInClinicalResearch.TabIndex = 15;
            this.cboPatientInClinicalResearch.SelectedIndexChanged += new System.EventHandler(this.cboPatientInClinicalResearch_SelectedIndexChanged);
            this.cboPatientInClinicalResearch.SelectionChangeCommitted += new System.EventHandler(this.cboPatientInClinicalResearch_SelectionChangeCommitted);
            this.cboPatientInClinicalResearch.DropDownClosed += new System.EventHandler(this.cboPatientInClinicalResearch_DropDownClosed);
            this.cboPatientInClinicalResearch.Validating += new System.ComponentModel.CancelEventHandler(this.cboPatientInClinicalResearch_Validating);
            // 
            // patientTypeHSVLocationView
            // 
            this.patientTypeHSVLocationView.Location = new System.Drawing.Point(3, 0);
            this.patientTypeHSVLocationView.Model = null;
            this.patientTypeHSVLocationView.Name = "patientTypeHSVLocationView";
            this.patientTypeHSVLocationView.NursingStationCode = null;
            this.patientTypeHSVLocationView.Size = new System.Drawing.Size(308, 57);
            this.patientTypeHSVLocationView.TabIndex = 1;
            // 
            // cmbAdmitSrc
            // 
            this.cmbAdmitSrc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdmitSrc.Location = new System.Drawing.Point(418, 214);
            this.cmbAdmitSrc.Name = "cmbAdmitSrc";
            this.cmbAdmitSrc.Size = new System.Drawing.Size(192, 21);
            this.cmbAdmitSrc.TabIndex = 18;
            this.cmbAdmitSrc.SelectedIndexChanged += new System.EventHandler(this.cmbAdmitSrc_SelectedIndexChanged);
            this.cmbAdmitSrc.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAdmitSrc_Validating);
            // 
            // cmbAccidentType
            // 
            this.cmbAccidentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccidentType.Location = new System.Drawing.Point(450, 20);
            this.cmbAccidentType.Name = "cmbAccidentType";
            this.cmbAccidentType.Size = new System.Drawing.Size(139, 21);
            this.cmbAccidentType.TabIndex = 8;
            this.cmbAccidentType.SelectedIndexChanged += new System.EventHandler(this.cmbAccidentType_SelectedIndexChanged);
            // 
            // rbNone
            // 
            this.rbNone.Location = new System.Drawing.Point(417, 47);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(116, 15);
            this.rbNone.TabIndex = 10;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "None of the above";
            this.rbNone.Click += new System.EventHandler(this.rbNone_Click);
            this.rbNone.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbNone_KeyDown);
            // 
            // rbCrime
            // 
            this.rbCrime.Location = new System.Drawing.Point(341, 45);
            this.rbCrime.Name = "rbCrime";
            this.rbCrime.Size = new System.Drawing.Size(67, 15);
            this.rbCrime.TabIndex = 9;
            this.rbCrime.TabStop = true;
            this.rbCrime.Text = "Crime";
            this.rbCrime.Click += new System.EventHandler(this.rbCrime_Click);
            this.rbCrime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbCrime_KeyDown);
            // 
            // rbAccident
            // 
            this.rbAccident.Location = new System.Drawing.Point(341, 26);
            this.rbAccident.Name = "rbAccident";
            this.rbAccident.Size = new System.Drawing.Size(66, 15);
            this.rbAccident.TabIndex = 7;
            this.rbAccident.TabStop = true;
            this.rbAccident.Text = "Accident";
            this.rbAccident.Click += new System.EventHandler(this.rbAccident_Click);
            this.rbAccident.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbAccident_KeyDown);
            // 
            // cptCodesView
            // 
            this.cptCodesView.Location = new System.Drawing.Point(636, 216);
            this.cptCodesView.Model = null;
            this.cptCodesView.Name = "cptCodesView";
            this.cptCodesView.Size = new System.Drawing.Size(200, 25);
            this.cptCodesView.TabIndex = 17;
            // 
            // serviceCategory1
            // 
            this.serviceCategory1.BackColor = System.Drawing.Color.White;
            this.serviceCategory1.Location = new System.Drawing.Point(681, 165);
            this.serviceCategory1.Model = null;
            this.serviceCategory1.Name = "serviceCategory1";
            this.serviceCategory1.ServiceCategoryPresenter = null;
            this.serviceCategory1.Size = new System.Drawing.Size(242, 27);
            this.serviceCategory1.TabIndex = 21;
            this.serviceCategory1.ServiceCategorySelected += new System.EventHandler(this.serviceCategory1_ServiceCategorySelected);
            this.serviceCategory1.ServiceCategoryValidating += new System.ComponentModel.CancelEventHandler(this.serviceCategory1_ServiceCategoryValidating);
            // 
            // ShortDiagnosisView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.serviceCategory1);
            this.Controls.Add(this.cptCodesView);
            this.Controls.Add(this.mtbComments);
            this.Controls.Add(this.lblComments);
            this.Controls.Add(this.physicianSelectionView1);
            this.Controls.Add(this.btnViewClinicalTrialsDetails);
            this.Controls.Add(this.cboPatientInClinicalResearch);
            this.Controls.Add(this.lblPatientUnderResearchStudy);
            this.Controls.Add(this.mtbProcedure);
            this.Controls.Add(this.lblProcedure);
            this.Controls.Add(this.patientTypeHSVLocationView);
            this.Controls.Add(this.mtbComplaint);
            this.Controls.Add(this.cmbAdmitSrc);
            this.Controls.Add(this.lblAdmit);
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
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ShortDiagnosisView";
            this.Size = new System.Drawing.Size(983, 384);
            this.Load += new System.EventHandler(this.DiagnosisView_Load);
            this.Enter += new System.EventHandler(this.DiagnosisView_Enter);
            this.Leave += new System.EventHandler(this.DiagnosisView_Leave);
            this.Disposed += new System.EventHandler(this.DiagnosisView_Disposed);
            this.gbxDetails.ResumeLayout(false);
            this.gbxDetails.PerformLayout();
            this.gbxClinics.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Construction and Finalization
        public ShortDiagnosisView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            ConfigureControls();

            PopulateConditionMap();
            ClinicalTrialsDetailsView = new ClinicalTrialsDetailsView();
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

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        //added        
        private Container components = null;

        private ArrayList i_AllClinics = new ArrayList();
        private Facility facility;
        private TypeOfAccident accidentType;

        private PatientAccessComboBox cmbCountry;
        private PatientAccessComboBox cmbState;
        private PatientAccessComboBox cmbHour;
        private PatientAccessComboBox cmbAccidentType;
        private PatientAccessComboBox cmbAdmitSrc;

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

        private readonly ComboBox currentCombo = new ComboBox();

        private DateTimePicker dateTimePickerAccident;
        private DateTimePicker dateTimePickerSickness;

        private GroupBox gbxDetails;
        private GroupBox gbxClinics;

        private Label lblComplaint;
        private Label lblVisitResult;
        private Label lblDate;
        private Label lblHour;
        private Label lblCountry;
        private Label lblState;
        private Label lblOnset;
        private Label lblType;
        private Label lblAdmit;

        private readonly Hashtable combos = new Hashtable();
        private readonly Hashtable i_ConditionMap = new Hashtable();

        private bool i_Registered;
        private bool blnLeaveRun;
        private int verifyMonth;
        private int verifyDay;
        private int verifyYear;
        private bool loadingModelData = true;

        private string onsetDateTxt = String.Empty;
        internal string lastMenstrualDateTxt = String.Empty;
        private string accidentDateTxt = String.Empty;

        string i_NursingStationCode;

        private ArrayList i_PatientTypes;

        private MaskedEditTextBox mtbComplaint;
        private ShortPatientTypeHSVLocationView patientTypeHSVLocationView;
        private IShortDiagnosisViewPresenter diagnosisViewPresenter;
        private Label lblProcedure;
        private MaskedEditTextBox mtbProcedure;
        private Button btnViewClinicalTrialsDetails;
        private PatientAccessComboBox cboPatientInClinicalResearch;
        private Label lblPatientUnderResearchStudy;
        private bool userChangedIsPatientInClinicalResearchStudy;
        private CommonControls.PhysicianSelectionView physicianSelectionView1;
        private MaskedEditTextBox mtbComments;
        private Label lblComments;
        private CptCodes.ViewImpl.CptCodesView cptCodesView;
        private UI.CommonControls.ServiceCategory.ViewImpl.ServiceCategory serviceCategory1;
        private readonly IRuleEngine ruleEngine = Rules.RuleEngine.GetInstance();

        #endregion

        private void mtbComments_Validating(object sender, CancelEventArgs e)
        {
            Model.ClinicalComments = mtbComments.UnMaskedText;
        }

        #region Constants
        #endregion

        private void serviceCategory1_ServiceCategorySelected(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            ClinicServiceCategory selectedServiceCategory = args.Context as ClinicServiceCategory;
            if (selectedServiceCategory != null)
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
