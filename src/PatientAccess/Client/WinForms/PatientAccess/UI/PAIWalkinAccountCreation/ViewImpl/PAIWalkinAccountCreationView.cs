using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CptCodes.Presenters;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews;
using PatientAccess.UI.PAIWalkinAccountCreation.Presenters;
using PatientAccess.UI.PAIWalkinAccountCreation.Views;
using PatientAccess.UI.DemographicsViews.Presenters;

namespace PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl
{
    public partial class PAIWalkinAccountCreationView : ControlView, IPAIWalkinAccountCreationView
    {
        #region Events

        public event EventHandler RefreshTopPanel;
        public event EventHandler SetTabPageEvent;

        #endregion

        #region Event Handlers
        /// <summary>
        /// Fired from FinancialClassesView when the MSP Wizard does a button click
        /// </summary>
        private void TabSelectedEventHandler( object sender, EventArgs e )
        {
            if ( SetTabPageEvent != null )
            {
                var args = (LooseArgs)e;
                var index = (int)args.Context;
                SetTabPageEvent( this, new LooseArgs( index ) );
            }
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

        private void PAIWalkinAccountCreationViewOnEnter( object sender, EventArgs e )
        {
            leavingView = false;
            RegisterEvents();
            RuleEngine.EvaluateRule( typeof( OnPAIWalkinAccountCreationForm ), ModelAccount );
        }

        private void PAIWalkinAccountCreationViewOnLeave( object sender, EventArgs e )
        {
            blnLeaveRun = true;

            leavingView = true;
            paiWalkinAccountCreationPresenter.UpdateAdmitDate();
            sequesteredPatientPresenter.IsPatientSequestered();
            RuleEngine.EvaluateRule( typeof( OnPAIWalkinAccountCreationForm ), ModelAccount );

            blnLeaveRun = false;

            UnregisterEvents();
        }

        private void LastNameOnValidating( object sender, CancelEventArgs e )
        {
            RegisterRequriedRuleEvents();
            var mtb = (MaskedEditTextBox)sender;
            UIColors.SetNormalBgColor( mtb );
            if ( ModelAccount != null && ModelAccount.Patient != null )
            {
                paiWalkinAccountCreationPresenter.UpdateLastName( mtb.Text.Trim() );
            }
            if ( RefreshTopPanel != null )
            {
                RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
            }
        }
      
        private void FirstNameOnValidating( object sender, CancelEventArgs e )
        {

            RegisterRequriedRuleEvents();
            var mtb = (MaskedEditTextBox)sender;
            UIColors.SetNormalBgColor( mtb );
            if ( ModelAccount != null && ModelAccount.Patient != null )
            {
                paiWalkinAccountCreationPresenter.UpdateFirstName( mtb.Text.Trim() );
            }
            if ( RefreshTopPanel != null )
            {
                RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
            }
        }

        private void MiddleInitialOnValidating( object sender, CancelEventArgs e )
        {
            var mtb = (MaskedEditTextBox)sender;
            paiWalkinAccountCreationPresenter.UpdateMiddleInitial( mtb.Text.Trim() );
        }

        /// <summary>
        /// Handles the Validating event of the mtb_Procedure control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void ProcedureOnValidating( object sender, CancelEventArgs e )
        {
            RegisterRequriedRuleEvents();
            UIColors.SetNormalBgColor( mtbProcedure );
            paiWalkinAccountCreationPresenter.UpdateProcedureField( mtbProcedure.Text );
        }

        private void ComplaintOnValidating( object sender, CancelEventArgs e )
        {
            RegisterRequriedRuleEvents();
            UIColors.SetNormalBgColor( mtbComplaint );
            paiWalkinAccountCreationPresenter.UpdateChiefComplaintField( mtbComplaint.Text );

        }

        private void AdmitDateOnValidating( object sender, CancelEventArgs e )
        {

            RegisterRequriedRuleEvents();
            if ( ActiveControl == null
                && mtbAdmitDate.Text.Length == 10 )
            {
                return;
            }

            if ( !blnLeaveRun )
            {
                SetAdmitDateNormalBgColor();
            }

            if ( dateTimePicker.Focused )
            {
                return;
            }

            if ( !paiWalkinAccountCreationPresenter.VerifyAdmitDate() )
            {
                SetAdmitDateErrBgColor();
                mtbAdmitDate.Focus();
                return;
            }

            if ( !blnLeaveRun )
            {
                paiWalkinAccountCreationPresenter.UpdateAdmitDate();
            }

            if ( !dateTimePicker.Focused
                && !blnLeaveRun )
            {
                paiWalkinAccountCreationPresenter.RunAdmitDateValidationRules();
            }

            if ( mtbAdmitDate.UnMaskedText != string.Empty )
            {
                AdmitDateToInsuranceValidation.CheckAdmitDateToInsurance( ModelAccount, Name );
            }

            CheckAdmitDateToAuthorization();

            if ( !loadingModelData )
            {
                ssnView.UpdateDefaultSocialSecurityNumberForAdmitDate();
            }
        }

        private void AdmitTimeOnValidating( object sender, CancelEventArgs e )
        {
            RegisterRequriedRuleEvents();
            SetAdmitTimeNormalBgColor();
            paiWalkinAccountCreationPresenter.UpdateAdmitTime();
        }

        private void DateOfBirthOnValidating( object sender, CancelEventArgs e )
        {
            RegisterRequriedRuleEvents();
            UIColors.SetNormalBgColor( mtbDateOfBirth );
            if ( !paiWalkinAccountCreationPresenter.UpdateDateOfBirth() )
            {
                UIColors.SetErrorBgColor( mtbDateOfBirth );
                mtbDateOfBirth.Focus();
            }
            if ( RefreshTopPanel != null )
            {
                RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
            }
            if (!loadingModelData)
            {
                ssnView.ResetSSNControl();
            }
        }

        private void SsnViewOnSsnNumberChanged( object sender, EventArgs e )
        {
            if ( RefreshTopPanel != null )
            {
                RefreshTopPanel( this, new LooseArgs( ModelAccount.Patient ) );
            }
        }

        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void PAIWalkinAccountCreationViewOnDisposed( object sender, EventArgs e )
        {
            UnregisterEvents();
        }

        private void CheckAdmitDateToAuthorization()
        {
            Cursor = Cursors.WaitCursor;
            DialogResult result;

            RuleEngine.RegisterEvent( typeof( AdmitDateToAuthorizationDateRange ), ModelAccount, Name, null );
            RuleEngine.RegisterEvent( typeof( AdmitDateEarlierThanAuthEffectiveDate ), ModelAccount, Name, null );
            RuleEngine.RegisterEvent( typeof( AdmitDateLaterThanAuthExpirationDate ), ModelAccount, Name, null );

            bool isAdmiteDateToAuthorzationDates = RuleEngine.EvaluateRule( typeof( AdmitDateToAuthorizationDateRange ), ModelAccount, Name );
            bool isAdmiteDateEarlierThanAuthEffectiveDate = RuleEngine.EvaluateRule( typeof( AdmitDateEarlierThanAuthEffectiveDate ), ModelAccount, Name );
            bool isAdmiteDateLaterThanAuthExpirationDate = RuleEngine.EvaluateRule( typeof( AdmitDateLaterThanAuthExpirationDate ), ModelAccount, Name );

            if ( admitDateWarning && !isAdmiteDateToAuthorzationDates )
            {
                result = MessageBox.Show( UIErrorMessages.AUTHORIZATION_ADMIT_DATE_OUT_OF_RANGE, "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                admitDateWarning = false;
                if ( result == DialogResult.No )
                {
                    mtbAdmitDate.Focus();
                }
            }
            else if ( effectiveGreaterThanAdmitDateWarning && !isAdmiteDateEarlierThanAuthEffectiveDate )
            {
                result = MessageBox.Show( UIErrorMessages.ADMIT_DATE_EARLIER_THAN_AUTHORIZATION_EFFECTIVE_DATE, "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                effectiveGreaterThanAdmitDateWarning = false;
                if ( result == DialogResult.No )
                {
                    mtbAdmitDate.Focus();
                }
            }
            else if ( expirationLesserThanAdmitDateWarning && !isAdmiteDateLaterThanAuthExpirationDate )
            {
                result = MessageBox.Show( UIErrorMessages.ADMIT_DATE_LATER_THAN_AUTHORIZATION_EXPIRATION_DATE, "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                expirationLesserThanAdmitDateWarning = false;
                if ( result == DialogResult.No )
                {
                    mtbAdmitDate.Focus();
                }
            }

            RuleEngine.UnregisterEvent( typeof( AdmitDateToAuthorizationDateRange ), ModelAccount, null );
            RuleEngine.UnregisterEvent( typeof( AdmitDateEarlierThanAuthEffectiveDate ), ModelAccount, null );
            RuleEngine.UnregisterEvent( typeof( AdmitDateLaterThanAuthExpirationDate ), ModelAccount, null );

            RuleEngine.ClearActionsForRule( typeof( AdmitDateToAuthorizationDateRange ) );
            RuleEngine.ClearActionsForRule( typeof( AdmitDateEarlierThanAuthEffectiveDate ) );
            RuleEngine.ClearActionsForRule( typeof( AdmitDateLaterThanAuthExpirationDate ) );

            Cursor = Cursors.Arrow;
        }

        private void DateTimePickerOnCloseUp( object sender, EventArgs e )
        {
            SetAdmitDateNormalBgColor();

            DateTime dt = dateTimePicker.Value;
            mtbAdmitDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );

            if ( ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT &&
                ModelAccount.Activity.GetType().Equals( typeof( PAIWalkinOutpatientCreationActivity ) ) )
            {
                mtbAdmitTime.UnMaskedText = String.Format( "{0:D2}{1:D2}", dt.Hour, dt.Minute );
            }

            mtbAdmitDate.Focus();
        }

        //---------------------Evaluate ComboBoxes -----------------------------------
        private void AppointmentOnValidating( object sender, CancelEventArgs e )
        {
            RegisterRequriedRuleEvents();
            AppointmentOnSelectedIndexChanged( sender, e );
        }

        //--------------------- Invalid values in Combo boxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( Control comboBox )
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

        private void InvalidScheduleCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( cmbAppointment );
        }

        private void InvalidScheduleCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( cmbAppointment );
        }

        //----------------------------------------------------------------------
        #endregion // Event Handlers

        #region Rule Event Handlers
        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>

        private void AdmitDateTodayOrGreaterEventHandler( object sender, EventArgs e )
        {
            if ( ModelAccount.Activity.GetType() == typeof( PAIWalkinOutpatientCreationActivity ) )
            {
                UIColors.SetErrorBgColor( mtbAdmitDate );
                MessageBox.Show( UIErrorMessages.ADMIT_RANGE_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtbAdmitDate.Focus();
                Refresh();
            }

        }

        private void AdmitDateEnteredFiveDaysPastEventHandler( object sender, EventArgs e )
        {
            if ( ModelAccount.Activity.GetType() == typeof( PAIWalkinOutpatientCreationActivity ) )
            {
                if ( ModelAccount.KindOfVisit == null )
                {
                    return;
                }
                if ( ModelAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                {
                    UIColors.SetErrorBgColor( mtbAdmitDate );
                    MessageBox.Show( UIErrorMessages.EDIT_PREREG_ACCOUNT_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbAdmitDate.Focus();
                }
                else
                {
                    UIColors.SetErrorBgColor( mtbAdmitDate );
                    MessageBox.Show( UIErrorMessages.EDIT_ACCOUNT_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbAdmitDate.Focus();
                }
            }
        }

        private void AdmitDateEnteredFutureDateEventHandler( object sender, EventArgs e )
        {
            if ( ModelAccount.Activity.GetType() == typeof( PAIWalkinOutpatientCreationActivity ) )
            {
                if ( ModelAccount.KindOfVisit == null )
                {
                    return;
                }

                if ( ModelAccount.KindOfVisit.Code != VisitType.PREREG_PATIENT )
                {
                    ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ERRMSG );
                }
            }
        }

        delegate void TextBoxDelegate( MaskedEditTextBox aTextbox, string message );

        private static void TextBoxAsync( MaskedEditTextBox aTextbox, string message )
        {
            MessageBox.Show( message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            aTextbox.Focus();
            UIColors.SetErrorBgColor( aTextbox );
        }

        private static void ProcessTextboxErrorEvent( MaskedEditTextBox aTextbox, string message )
        {
            try
            {
                TextBoxDelegate d = TextBoxAsync;
                aTextbox.BeginInvoke( d, new object[] { aTextbox, message } );
            }
            catch
            {
                // intentionally left blank - we get exception when async call for 
                // previous account hasn't returned back with results yet and we already jumped to another activity 
            }
        }

        private void AdmitDateFiveDaysPastEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbAdmitDate );

            if ( ModelAccount.Activity.GetType() == typeof( PAIWalkinOutpatientCreationActivity ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ERRMSG );
            }
        }

        private void AdmitDateFutureDateEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbAdmitDate );

            if ( ModelAccount.Activity.GetType() == typeof( PAIWalkinOutpatientCreationActivity ) )
            {
                ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_RANGE_ERRMSG );
            }
        }

        private void InValidDateOfBirthEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbDateOfBirth );
            MessageBox.Show( UIErrorMessages.DOB_OUT_OF_RANGE, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1 );
            mtbDateOfBirth.Focus();
        }

        private void AdmitDateWithinSpecifiedSpanEventHandler( object sender, EventArgs e )
        {
            UIColors.SetErrorBgColor( mtbAdmitDate );

            ProcessTextboxErrorEvent( mtbAdmitDate, UIErrorMessages.ADMIT_DATE_TOO_FAR_IN_FUTURE );
        }

        private void LastNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbLastName );
        }

        private void FirstNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbFirstName );
        }

        private void DateOfBirthRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbDateOfBirth );
        }

        private void AdmitDateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbAdmitDate );
        }

        private void AdmitTimeRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbAdmitTime );
        }


        private void AppointmentOnSelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( cmbAppointment );

            var cbAppointment = (ComboBox)sender;
            paiWalkinAccountCreationPresenter.UpdateAppointment( (ScheduleCode)cbAppointment.SelectedItem );


        }


        private void AppointmentRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbAppointment );
        }

        private void PlanIDInPrimaryDisplayPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( insuranceView.lblPlanID );
            UIColors.SetPreferredBgColor( insuranceView.lblPlanName );
        }

        private void PlanIDInPrimaryDisplayRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( insuranceView.lblPlanID );
            UIColors.SetRequiredBgColor( insuranceView.lblPlanName );
        }

        #endregion

        #region Methods
        public void PopulateGender()
        {
        }

        public void PopulateFirstName()
        {
            mtbFirstName.Text = ModelAccount.Patient.FirstName != null
                                    ? ModelAccount.Patient.FirstName.Trim()
                                    : String.Empty;
        }

        public void SetAdmitDateError()
        {
            SetAdmitDateErrBgColor();
            mtbAdmitDate.Focus();
        }

        public void SetAdmitTimeError()
        {
            SetAdmitTimeErrBgColor();
            mtbAdmitTime.Focus();
        }

        public void DisplayMessageForMedicareAdvise()
        {
            AccountView.GetInstance().MedicareOver65Checked = true;

            DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION_DURING_QUICK_ACCOUNT_CREATION,
                                                          UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

            if ( warningResult == DialogResult.Yes )
            {
                if ( leavingView )
                {
                    AccountView.GetInstance().Over65Check = true;
                    leavingView = false;
                }
                insuranceView.SetDefaultFocus();
            }
        }

        public void PopulateLastName()
        {
            mtbLastName.Text = ModelAccount.Patient.LastName != null
                                  ? ModelAccount.Patient.LastName.Trim()
                                  : String.Empty;

        }

        public void PopulateMiddleInitial()
        {
            mtbMiddleInitial.Text = ModelAccount.Patient.MiddleInitial != null
                                        ? ModelAccount.Patient.MiddleInitial.Trim()
                                        : String.Empty;
        }

        /// <summary>
        /// Display Procedure
        /// </summary>
        public void PopulateProcedure()
        {
            mtbProcedure.Text = ModelAccount.Diagnosis.Procedure != null ? ModelAccount.Diagnosis.Procedure.Trim() : String.Empty;
        }

        /// <summary>
        /// Display Procedure
        /// </summary>
        public void PopulateChiefComplaint()
        {
            mtbComplaint.Text = ModelAccount.Diagnosis.ChiefComplaint != null ? ModelAccount.Diagnosis.ChiefComplaint.Trim() : String.Empty;
        }

        public void PopulateScheduleCodeComboBox( ArrayList scheduleCodes )
        {
            cmbAppointment.Items.Clear();
            var walkinAppointment = new ScheduleCode();
            foreach ( ScheduleCode schedule in scheduleCodes )
            {
                cmbAppointment.Items.Add( schedule );
                if ( schedule.Code == ScheduleCode.WALKIN_WITH_AN_ORDER )
                {
                    walkinAppointment = schedule;
                }
            }
            paiWalkinAccountCreationPresenter.UpdateAppointment( walkinAppointment );
        }

        public void SetFocusToAdmitTime()
        {
            mtbAdmitTime.Focus();
        }

        public void SetNormalColorForDateOfBirth()
        {
            UIColors.SetNormalBgColor( mtbDateOfBirth );
        }

        public override void UpdateView()
        {
            sequesteredPatientPresenter = new SequesteredPatientPresenter(new SequesteredPatientFeatureManager(), ModelAccount);
            sequesteredPatientPresenter.IsPatientSequestered();

            paiWalkinAccountCreationPresenter = new PAIWalkinAccountCreationPresenter( this, new MessageBoxAdapter(), ModelAccount, RuleEngine.GetInstance() );
            insuranceView.Presenter = new PAIWalkinInsurancePresenter( insuranceView, ModelAccount, RuleEngine.GetInstance() );
            ssnView.SsnFactory = new SsnFactoryCreator( ModelAccount ).GetSsnFactory();
            CptCodesPresenter = new CptCodesPresenter( cptCodesView, ModelAccount, new CptCodesFeatureManager(), new MessageBoxAdapter() );

            if ( loadingModelData )
            {
                PopulateGenderAndBirthGender();

                paiWalkinAccountCreationPresenter.UpdateView();
                patientTypeHSVLocationView.Model = ModelAccount;
                patientTypeHSVLocationView.UpdateView();

                CptCodesPresenter.UpdateView();

                physicianSelectionView1.Model = ModelAccount;
                physicianSelectionView1.UpdateView();

                UpdateInsuranceView();
          
                if ( ModelAccount.Patient == null )
                {
                    MessageBox.Show( "No patient data is present!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return;
                }
 
                ssnView.Model = ModelAccount.Patient;
                ssnView.ModelAccount = ModelAccount;
                ssnView.UpdateView();
                RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFiveDaysPast ), Model );
                RuleEngine.EvaluateRule( typeof( AdmitDateEnteredFutureDate ), Model );
            }

            mtbLastName.Focus();

            RegisterEvents();
            this.insuranceView.BringToFront();
            RunRules();
            this.birthGenderView.BringToFront();
            if ( mtbAdmitDate.Text != "  /  /"
                && mtbAdmitDate.Text.Length != 10 )
            {
                SetAdmitDateErrBgColor();
            }

            if ( mtbAdmitDate.UnMaskedText == "01010001" )
            {
                mtbAdmitDate.UnMaskedText = string.Empty;
            }

            if ( mtbDateOfBirth.UnMaskedText == "01010001" )
            {
                mtbDateOfBirth.UnMaskedText = string.Empty;
            }

            if ( mtbAdmitTime.UnMaskedText == "0000" )
            {
                mtbAdmitTime.UnMaskedText = string.Empty;
            }

            loadingModelData = false;
        }

        #endregion

        #region Properties

        public string DateOfBirth
        {
            get { return mtbDateOfBirth.UnMaskedText.Trim(); }
            set { mtbDateOfBirth.Text = value; }
        }
        public string DateOfBirthText
        {
            get { return mtbDateOfBirth.Text; }
        }

        public ScheduleCode ScheduleCode
        {
            set { cmbAppointment.SelectedItem = value; }
        }

        public SSNControl SSNView
        {
            get { return this.ssnView; }
        }

        public string AdmitTime
        {
            get { return mtbAdmitTime.UnMaskedText.Trim(); }
            set { mtbAdmitTime.UnMaskedText = value; }
        }


        public string AdmitDate
        {
            get { return mtbAdmitDate.UnMaskedText.Trim(); }
            set { mtbAdmitDate.UnMaskedText = value; }
        }

        public Account ModelAccount
        {
            get
            {
                return (Account)Model;
            }
        }

        public string AdmitDateText
        {
            get { return mtbAdmitDate.Text.Trim(); }
        }

        public string Age
        {
            set { lblPatientAge.Text = value; }
            get { return lblPatientAge.Text; }
        }

        public IRequiredFieldsSummaryPresenter RequiredFieldsSummaryPresenter
        {
            set
            {
                requiredFieldsSummaryPresenter = value;
                if ( requiredFieldsSummaryPresenter != null )
                {
                    EventHandler eventHandler = TabSelectedEventHandler;
                    if ( !requiredFieldsSummaryPresenter.IsTabEventRegistered( eventHandler ) )
                    {
                        requiredFieldsSummaryPresenter.TabSelectedEvent += eventHandler;
                    }
                }
            }
        }

        private RuleEngine RuleEngine
        {
            get { return ruleEngine ?? ( ruleEngine = RuleEngine.GetInstance() ); }
        }

        private IPAIWalkinAccountCreationPresenter paiWalkinAccountCreationPresenter { get; set; }

        private CptCodesPresenter CptCodesPresenter { get; set; }
        private GenderViewPresenter PatientGenderViewPresenter { get; set; }
        private GenderViewPresenter BirthGenderViewPresenter { get; set; }


        #endregion

        #region Private Methods
        private void EnableBirthGender()
        {
            lblBirthGender.Enabled = true;
            lblBirthGender.Visible = true;
            birthGenderView.Enabled = true;
            birthGenderView.Visible = true;
        }

        private void PopulateGenderAndBirthGender()
        {
            PatientGenderViewPresenter = new GenderViewPresenter(genderView, ModelAccount, Gender.PATIENT_GENDER);
            PatientGenderViewPresenter.RefreshTopPanel += new System.EventHandler(gendersView_RefreshTopPanel);
            PatientGenderViewPresenter.UpdateView();

            EnableBirthGender();
            BirthGenderViewPresenter =
                new GenderViewPresenter(birthGenderView, ModelAccount, Gender.BIRTH_GENDER);
            BirthGenderViewPresenter.UpdateView();
        }

        private void gendersView_RefreshTopPanel(object sender, EventArgs e)
        {
            if (RefreshTopPanel != null)
            {
                RefreshTopPanel(this, new LooseArgs(ModelAccount.Patient));
            }
        }

        private void ResetFinancialClassesView( Coverage primaryCoverage )
        {

            insuranceView.financialClassesView.Model_Account = ModelAccount;
            insuranceView.financialClassesView.Model = primaryCoverage;
            insuranceView.financialClassesView.UpdateView();
        }

        private void CatchCoverageResetClickedEvent( Coverage aCoverage )
        {
            CoverageReset( aCoverage );
        }

        private void CoverageReset( Coverage aCoverage )
        {
            if ( aCoverage.CoverageOrder.Oid == CoverageOrder.SECONDARY_OID )
            {
                ModelAccount.DeletedSecondaryCoverage = aCoverage;
            }

            ModelAccount.Insurance.RemoveCoverage( aCoverage );

            if ( aCoverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
            {
                // remove the coverage from the FC view
                insuranceView.financialClassesView.Model_Coverage = null;
                insuranceView.financialClassesView.ResetFinancialClass();
                // reset HIC on 2ndary 
            }

            var rule = new MSPQuestionaireRequired();

            // find the remaining coverage (if any) and determine if MSP should be cleared

            bool isMedicare = false;

            if ( ModelAccount.Insurance.Coverages.Count > 0 )
            {
                long covOrder;

                if ( aCoverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                {
                    covOrder = CoverageOrder.SECONDARY_OID;
                }
                else
                {
                    covOrder = CoverageOrder.PRIMARY_OID;
                }

                Coverage remainingCoverage = ModelAccount.Insurance.CoverageFor( covOrder );

                isMedicare = rule.InsuranceIsMedicare( ModelAccount.FinancialClass,
                    ModelAccount.KindOfVisit,
                    ModelAccount.HospitalService,
                    remainingCoverage );
            }

            if ( !isMedicare )
            {
                ModelAccount.MedicareSecondaryPayor = new MedicareSecondaryPayor();
            }
            savedPrimaryModelCoverage = null;
            insuranceView.ResetCoverage();
            RunRules();
            ViewFactory.Instance.CreateView<PatientAccessView>().SetPreviousDocumentOptions();
        }
        private void CatchCoverageEvent( ICollection coverages )
        {
            insuranceView.ResetView();

            foreach ( Coverage coverage in coverages )
            {
                if ( coverage == null )
                {
                    continue;
                }

                if ( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                {
                    savedPrimaryModelCoverage = coverage.DeepCopy();
                    insuranceView.Model = coverage;
                    insuranceView.UpdateView();
                    insuranceView.financialClassesView.Model_Account = ModelAccount;
                    insuranceView.financialClassesView.Model = coverage;
                    insuranceView.financialClassesView.UpdateView();
                }

            }

            RunRules();
            insuranceView.financialClassesView.RunRules();
        }

        private void CatchPlanSelectedEvent( Coverage newCoverage )
        {
            newCoverage.IsNew = true;
            newCoverage.Account.Facility = User.GetCurrent().Facility;

            // The user has chosen a new InsurancePlan... this means that the existing
            // coverage instance is no longer valid.  We need to capture the CoverageOrder
            // (so we know if it's Primary or Secondary), capture the chosen Plan,
            // remove the old coverage and add a new one (based on the Category associated
            // with the newly chosen plan)!

            bool showCoverageDialog = false;

            // default just to intantiate

            Coverage oldCoverage = new OtherCoverage();

            // See if patient already had a plan and it is different from the selected plan

            if ( newCoverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID
                && savedPrimaryModelCoverage != null
                && newCoverage.InsurancePlan.PlanID != savedPrimaryModelCoverage.InsurancePlan.PlanID )
            {
                oldCoverage = savedPrimaryModelCoverage;
                showCoverageDialog = true;
            }


            if ( showCoverageDialog )
            {
                // PlanChangeDialog will give the user the opportunity to import   
                // some of the information on the old coverage to the new coverage,
                // or cancel the change completely.

                using ( var dialog = new PlanChangeDialog( oldCoverage, newCoverage, ModelAccount ) )
                {
                    dialog.ShowDialog( this );
                    if ( dialog.DialogResult == DialogResult.OK )
                    {
                        // Update the saved plan ID
                        if ( newCoverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                        {
                            ModelAccount.Insurance.RemovePrimaryCoverage();
                            ModelAccount.Insurance.AddCoverage( newCoverage );
                        }
                        else if ( newCoverage.CoverageOrder.Oid.Equals( CoverageOrder.SECONDARY_OID ) )
                        {
                            ModelAccount.Insurance.RemoveSecondaryCoverage();
                            ModelAccount.Insurance.AddCoverage( newCoverage );
                        }
                        ResetFinancialClassesView( newCoverage );
                    }
                    else // Cancel change
                    {
                        if ( newCoverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                        {
                            ModelAccount.Insurance.RemovePrimaryCoverage();
                            ModelAccount.Insurance.AddCoverage( savedPrimaryModelCoverage );
                        }

                        ResetFinancialClassesView( savedPrimaryModelCoverage );
                    }
                }
            }

            else
            {
                ModelAccount.Insurance.AddCoverage( newCoverage );
            }

            UpdateInsuranceView();
            insuranceView.financialClassesView.RunRules();

            // Re-set the previously scanned documents icons/menu options as the plan category may have changed

            ViewFactory.Instance.CreateView<PatientAccessView>().SetPreviousDocumentOptions();

        }

        private void UpdateInsuranceView()
        {
            Insurance insurance = ModelAccount.Insurance;

            insuranceView.Account = ModelAccount;
            insuranceView.UpdateView();


            // The account is needed in order to save the Mother's and Father's DOB 
            // entered in the FinancialClassesView, to the Account.Patient object 
            insuranceView.financialClassesView.Model_Account = ModelAccount;
            insuranceView.financialClassesView.ResetFinancialClass();
            if ( insurance == null )
            {   // TODO: Add error log message here
                return;
            }
            // Iterate over the coverage collection and populate
            // the primary and secondary insurance screens.

            ICollection coverageCollection = insurance.Coverages;

            if ( coverageCollection == null )
            {
                return;
            }

            foreach ( Coverage coverage in coverageCollection )
            {
                if ( coverage == null )
                {
                    continue;
                }
                if ( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                {
                    savedPrimaryModelCoverage = coverage.DeepCopy();

                    // The account is needed in order to populate the 'CopyPartyView' 
                    // in the InsuranceDetails dialog box which is invoked from the
                    // InsuredSummaryView's Edit button.

                    primaryCoveragePlanID.Append( coverage.InsurancePlan.PlanID );
                    insuranceView.Model = coverage;
                    insuranceView.UpdateView();
                    insuranceView.financialClassesView.Model_Account = ModelAccount;
                    insuranceView.financialClassesView.Model = coverage;
                    insuranceView.financialClassesView.UpdateView();
                }
            }

            RunRules();
        }

        /// <summary>
        /// RunRules - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void RunRules()
        {
            RegisterRequriedRuleEvents();
            UIColors.SetNormalBgColor( mtbLastName );
            UIColors.SetNormalBgColor( mtbFirstName );
            UIColors.SetNormalBgColor( mtbAdmitDate );
            UIColors.SetNormalBgColor( mtbAdmitTime );
            UIColors.SetNormalBgColor( mtbDateOfBirth );
            UIColors.SetNormalBgColor( cmbAppointment );
            UIColors.SetNormalBgColor( insuranceView.lblPlanID );
            UIColors.SetNormalBgColor( insuranceView.lblPlanName );

            RuleEngine.EvaluateRule( typeof( OnPAIWalkinAccountCreationForm ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( FirstNameRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( LastNameRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( GenderRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( DateOfBirthRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( AdmitDateRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( AdmitTimeRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( AppointmentRequired ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( PlanIDInPrimaryDisplayPreferred ), ModelAccount );
            RuleEngine.EvaluateRule( typeof( PlanIDInPrimaryDisplayRequired ), ModelAccount );

            ssnView.RunRules();
            PatientGenderViewPresenter.RunRules(); 

        }

        private void SetAdmitTimeErrBgColor()
        {
            UIColors.SetErrorBgColor(mtbAdmitTime);
        }

        private void SetAdmitDateErrBgColor()
        {
            UIColors.SetErrorBgColor(mtbAdmitDate);
        }
        public void SetDateOfBirthErrBgColor()
        {
            UIColors.SetErrorBgColor( mtbDateOfBirth );
        }
        private void SetAdmitDateNormalBgColor()
        {
            UIColors.SetNormalBgColor( mtbAdmitDate );
            Refresh();
        }

        private void SetAdmitTimeNormalBgColor()
        {
            UIColors.SetNormalBgColor( mtbAdmitTime );
            Refresh();
        }

        private void RegisterRequriedRuleEvents()
        {
            UnregisterEvents();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            if ( registered )
            {
                return;
            }

            registered = true;
            RuleEngine.RegisterEvent( typeof( LastNameRequired ), Model, LastNameRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( FirstNameRequired ), Model, FirstNameRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( DateOfBirthRequired ), Model, DateOfBirthRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitTimeRequired ), Model, AdmitTimeRequiredEventHandler );
            RegisterAdmitDateValidationRules();
            RuleEngine.RegisterEvent( typeof( AppointmentRequired ), Model, AppointmentRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( ChiefComplaintRequired ), ChiefComplaintRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( ProcedureRequired ), ProcedureRequiredEventHandler );

            RuleEngine.RegisterEvent( typeof( PlanIDInPrimaryDisplayPreferred ), Model, PlanIDInPrimaryDisplayPreferredEventHandler );
            RuleEngine.RegisterEvent( typeof( PlanIDInPrimaryDisplayRequired ), Model, PlanIDInPrimaryDisplayRequiredEventHandler );

            RuleEngine.RegisterEvent( typeof( InvalidScheduleCode ), Model, InvalidScheduleCodeEventHandler );
            RuleEngine.RegisterEvent( typeof( InvalidScheduleCodeChange ), Model, InvalidScheduleCodeChangeEventHandler );
            RuleEngine.RegisterEvent( typeof( InValidDateOfBirth ), Model, InValidDateOfBirthEventHandler );
            ssnView.RegisterRules();
        }

        private void UnregisterEvents()
        {
            registered = false;

            RuleEngine.UnregisterEvent( typeof( LastNameRequired ), Model, LastNameRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( FirstNameRequired ), Model, FirstNameRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( DateOfBirthRequired ), Model, DateOfBirthRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitTimeRequired ), Model, AdmitTimeRequiredEventHandler );
            UnRegisterAdmitDateValidationRules();
            RuleEngine.UnregisterEvent( typeof( AppointmentRequired ), Model, AppointmentRequiredEventHandler );


            RuleEngine.UnregisterEvent( typeof( ChiefComplaintRequired ),
                                       ChiefComplaintRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( ProcedureRequired ), ProcedureRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( PlanIDInPrimaryDisplayPreferred ), Model, PlanIDInPrimaryDisplayPreferredEventHandler );
            RuleEngine.UnregisterEvent( typeof( PlanIDInPrimaryDisplayRequired ), Model, PlanIDInPrimaryDisplayRequiredEventHandler );

            RuleEngine.UnregisterEvent( typeof( InValidDateOfBirth ), Model, InValidDateOfBirthEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidScheduleCode ), Model, InvalidScheduleCodeEventHandler );
            RuleEngine.UnregisterEvent( typeof( InvalidScheduleCodeChange ), Model, InvalidScheduleCodeChangeEventHandler );
        }

        private void RegisterAdmitDateValidationRules()
        {
            RuleEngine.RegisterEvent( typeof( AdmitDateFiveDaysPast ), Model, AdmitDateFiveDaysPastEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateEnteredFiveDaysPast ), Model, mtbAdmitDate, AdmitDateEnteredFiveDaysPastEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateEnteredFutureDate ), Model, mtbAdmitDate, AdmitDateEnteredFutureDateEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateTodayOrGreater ), Model, AdmitDateTodayOrGreaterEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateFutureDate ), Model, AdmitDateFutureDateEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.RegisterEvent( typeof( AdmitDateWithinSpecifiedSpan ), Model, AdmitDateWithinSpecifiedSpanEventHandler );
        }

        private void UnRegisterAdmitDateValidationRules()
        {
            RuleEngine.UnregisterEvent( typeof( AdmitDateTodayOrGreater ), Model, AdmitDateTodayOrGreaterEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateFutureDate ), Model, AdmitDateFutureDateEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateRequired ), Model, AdmitDateRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateWithinSpecifiedSpan ), Model, AdmitDateWithinSpecifiedSpanEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitTimeRequired ), Model, AdmitTimeRequiredEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateEnteredFiveDaysPast ), Model, AdmitDateEnteredFiveDaysPastEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateEnteredFutureDate ), Model, AdmitDateEnteredFutureDateEventHandler );
            RuleEngine.UnregisterEvent( typeof( AdmitDateFiveDaysPast ), Model, AdmitDateFiveDaysPastEventHandler );
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbFirstName );
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbLastName );

            MaskedEditTextBoxBuilder.ConfigureMIAndSuffix( mtbMiddleInitial );

            MaskedEditTextBoxBuilder.ConfigureChiefComplaint( mtbProcedure );
            MaskedEditTextBoxBuilder.ConfigureProcedure( mtbComplaint );

        }
        #endregion

        #region Construction and Finalization
        public PAIWalkinAccountCreationView()
        {
            InitializeComponent();
            ConfigureControls();
        }


        #endregion

        #region Data Elements

        private Coverage savedPrimaryModelCoverage;
        private readonly StringBuilder primaryCoveragePlanID = new StringBuilder();
        private bool loadingModelData = true;
        private bool registered;
        private RuleEngine ruleEngine;
        private IRequiredFieldsSummaryPresenter requiredFieldsSummaryPresenter;
        private SequesteredPatientPresenter sequesteredPatientPresenter;
        private bool blnLeaveRun;
        private bool admitDateWarning = true;
        private bool effectiveGreaterThanAdmitDateWarning = true;
        private bool expirationLesserThanAdmitDateWarning = true;
        private bool leavingView;

        #endregion

        #region Constants
        #endregion


        
    }
}
